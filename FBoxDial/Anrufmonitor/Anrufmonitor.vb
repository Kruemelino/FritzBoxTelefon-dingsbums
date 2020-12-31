Imports System.Net
Imports System.Net.Sockets
Imports System.Timers

Friend Class Anrufmonitor
#Region "Eigenschaften"
    ''' <summary>
    ''' Angabe, ob der TCP-Client zur Fritz!Box verbunden ist.
    ''' </summary>
    Friend ReadOnly Property Aktiv As Boolean
        Get
            If AnrMonTCPClient Is Nothing Then
                Return False
            Else
                Return AnrMonTCPClient.Verbunden
            End If
        End Get
    End Property
    Friend Property AktiveTelefonate As List(Of Telefonat)
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
#End Region

#Region "Konstanten"
    Private Const AnrMon_RING As String = "RING"
    Private Const AnrMon_CALL As String = "CALL"
    Private Const AnrMon_CONNECT As String = "CONNECT"
    Private Const AnrMon_DISCONNECT As String = "DISCONNECT"
    Private Const AnrMon_Delimiter As String = ";"
#End Region

#Region "Timer"
    Private WithEvents TimerAnrMonReStart As Timers.Timer
    Private Property RestartTimerIterations As Integer
#End Region
    Private WithEvents AnrMonTCPClient As AnrMonClient

    Public Sub New()
        AktiveTelefonate = New List(Of Telefonat)
    End Sub

    Friend Sub StartAnrMon()
        ' Starte den Anrufmonitor
        Dim IP As IPAddress = IPAddress.Loopback

        If IPAddress.TryParse(XMLData.POptionen.ValidFBAdr, IP) Then
            Dim TC As New TcpClient With {.ExclusiveAddressUse = False}

            Try
                TC.Connect(New IPEndPoint(IP, FritzBoxDefault.DfltFBAnrMonPort))
            Catch ex As SocketException
                TC.Close()
                NLogger.Error("Anrufmonitor", ex)
            End Try

            If TC.Connected Then
                ' Info Message für das Log
                NLogger.Info("Anrufmonitor verbunden zu {0}:{1}", IP.ToString, FritzBoxDefault.DfltFBAnrMonPort)
                AnrMonTCPClient = New AnrMonClient(TC)

                ' Verbinden
                AnrMonTCPClient.Connect()
            Else

                ' Info Message für das Log
                NLogger.Info("Anrufmonitor nicht verbunden zu {0}:{1}", IP.ToString, FritzBoxDefault.DfltFBAnrMonPort)
            End If
        End If
        ' Ribbon aktualisieren
        ThisAddIn.POutlookRibbons.RefreshRibbon()
    End Sub

    Friend Sub StoppAnrMon()
        If AnrMonTCPClient?.Verbunden Then
            ' TCP-Client trennen
            AnrMonTCPClient.Disconnect()

            ' Info Message für das Log
            NLogger.Debug("Anrufmonitor gewollt angehalten")
        End If
        ' Ribbon aktualisieren
        ThisAddIn.POutlookRibbons.RefreshRibbon()
    End Sub

    Private Sub AnrMonTCPClient_Disposed(Sender As AnrMonClient) Handles AnrMonTCPClient.Disposed
        'Aktiv = False
        ThisAddIn.POutlookRibbons.RefreshRibbon()
        NLogger.Info("Anrufmonitor getrennt von {0}:{1}", XMLData.POptionen.ValidFBAdr, FritzBoxDefault.DfltFBAnrMonPort)
    End Sub

#Region "Anrufmonitor Standby PowerMode"
    Friend Sub RestartOnResume()
        ' Falls der Anrufmonitor aktiv sein sollte, dann halte ihn sicherheitshalber an.
        If Aktiv Then StoppAnrMon()

        If TimerAnrMonReStart IsNot Nothing Then
            NLogger.Debug("Timer für Reaktivierung nach Standby ist nicht Nothing und wird neu gestartet.")

            ' Timer stoppen und auf Nothing setzen
            TimerAnrMonReStart = KillTimer(TimerAnrMonReStart)
        End If

        ' Initiiere einen neuen Timer
        NLogger.Debug("Timer für Reaktivierung nach Standby wird gestartet.")

        ' Setze die Zählvariable auf 0
        RestartTimerIterations = 0

        ' Initiiere den Timer mit Intervall von 2 Sekunden
        TimerAnrMonReStart = SetTimer(DfltReStartIntervall)

        ' Starte den Timer
        TimerAnrMonReStart.Start()

    End Sub

    Private Sub TimerAnrMonReStart_Elapsed(sender As Object, e As ElapsedEventArgs) Handles TimerAnrMonReStart.Elapsed
        ' Prüfe, ob die maximale Anzahl an Durchläufen (15) noch nicht erreicht wurde
        If RestartTimerIterations.IsLess(DfltTryMaxRestart) Then
            ' Wenn der Anrufmonitor aktiv ist, dann hat das wiederverbinden geklappt.
            If Aktiv Then
                ' Halte den TImer an und löse ihn auf
                With TimerAnrMonReStart
                    .Stop()
                    .Dispose()
                End With
                ' Statusmeldung
                NLogger.Info("Anrufmonitor konnte nach {0} Versuchen erfolgreich neu gestartet werden.", RestartTimerIterations)
            Else
                ' Erhöhe den Wert der durchgeführten Iterationen
                RestartTimerIterations += 1
                ' Statusmeldung
                NLogger.Debug("Timer: Starte {0}. Versuch den Anrufmonitor zu starten.", RestartTimerIterations)
                ' Starte den nächsten Versuch den Anrufmonitor zu verbinden
                StartAnrMon()
            End If
        Else
            ' Es konnte keine Verbindung zur Fritz!Box aufgebaut werden.
            NLogger.Error("Anrufmonitor konnte nach {0} Versuchen nicht neu gestartet werden.", RestartTimerIterations)

            ' Halte den TImer an und löse ihn auf
            TimerAnrMonReStart = KillTimer(TimerAnrMonReStart)
        End If
        ' Ribbon aktualisieren
        ThisAddIn.POutlookRibbons.RefreshRibbon()
    End Sub
#End Region

#Region "Anrufmonitor"
    Private Sub AnrMonTCPClient_Message(sender As Object, e As NotifyEventArgs(Of String)) Handles AnrMonTCPClient.Message

        Dim AktivesTelefonat As Telefonat
        Dim FBStatus As String = e.Value.RegExRemove("\r\n?|\n") ' Entferne den Zeilenumbruch
        Dim FBStatusSplit As String() = FBStatus.Split(AnrMon_Delimiter)

        ' Hier die Daten des Fritz!Box Anrufmonitors weitergeben
        NLogger.Info("AnrMonAktion: {0}", FBStatus)

        'Schauen ob "RING", "CALL", "CONNECT" oder "DISCONNECT" übermittelt wurde
        Select Case FBStatusSplit(1)
            Case AnrMon_RING
                ' Neues Telefonat erzeugen und Daten des Anrufmonitors übergeben
                AktivesTelefonat = New Telefonat With {.SetAnrMonRING = FBStatusSplit}
                ' Füge das Telefonat der Liste hinzu
                AktiveTelefonate.Add(AktivesTelefonat)

            Case AnrMon_CALL
                ' Neues Telefonat erzeugen und Daten des Anrufmonitors übergeben
                AktivesTelefonat = New Telefonat With {.SetAnrMonCALL = FBStatusSplit}
                ' Füge das Telefonat der Liste hinzu
                AktiveTelefonate.Add(AktivesTelefonat)

            Case AnrMon_CONNECT
                ' Vorhandenes Telefonat ermitteln und Daten des Anrufmonitors übergeben
                AktivesTelefonat = AktiveTelefonate.Find(Function(TE) TE.ID.AreEqual(CInt(FBStatusSplit(2))))
                If AktivesTelefonat IsNot Nothing Then AktivesTelefonat.SetAnrMonCONNECT = FBStatusSplit

            Case AnrMon_DISCONNECT
                ' Vorhandenes Telefonat ermitteln und Daten des Anrufmonitors übergeben
                AktivesTelefonat = AktiveTelefonate.Find(Function(TE) TE.ID.AreEqual(CInt(FBStatusSplit(2))))
                If AktivesTelefonat IsNot Nothing Then AktivesTelefonat.SetAnrMonDISCONNECT = FBStatusSplit
                ' Das Gespräch ist beendet. Entferne dieses Telefonat aus der Liste aktiver Telefonate
                AktiveTelefonate.Remove(AktivesTelefonat)

        End Select
    End Sub

#End Region

End Class
