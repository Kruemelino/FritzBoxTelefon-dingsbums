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
    Private Const AnrMon_Port As Integer = 1012
#End Region

#Region "Timer"
    Private Property TimerAnrMonReStart As Timer
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
                TC.Connect(New IPEndPoint(IP, AnrMon_Port))
            Catch ex As SocketException
                ' Connection refused.
                ' No Connection could be made because the target computer actively refused it. This usually results from trying To connect To a service that Is inactive On the foreign host—that Is, one with no server application running.
                If ex.SocketErrorCode = SocketError.ConnectionRefused Then
                    NLogger.Warn("Der Anrufmonitor kann nicht verbunden werden, da der Fritz!Box CallMonitor (Port 1012) nicht aktiviert ist (Telefonecode #96*5* zum aktivieren).")
                Else
                    NLogger.Error(ex, "TcpClient.Connect")
                End If

            End Try

            If TC.Connected Then
                ' Info Message für das Log
                NLogger.Info($"Anrufmonitor verbunden zu {IP}:{AnrMon_Port}")
                AnrMonTCPClient = New AnrMonClient(TC)

                ' Verbinden
                AnrMonTCPClient.Connect()
            Else
                TC.Close()
                ' Info Message für das Log
                NLogger.Warn($"Anrufmonitor nicht verbunden zu {IP}:{AnrMon_Port}")
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

#Region "Anrufmonitor Standby PowerMode"
    Friend Sub Reaktivieren()
        ' Falls der Anrufmonitor aktiv sein sollte, dann halte ihn sicherheitshalber an.
        If Aktiv Then StoppAnrMon()

        If TimerAnrMonReStart IsNot Nothing Then
            NLogger.Debug("Timer für Reaktivierung ist nicht Nothing und wird neu gestartet.")

            ' Ereignishandler entfernen
            RemoveHandler TimerAnrMonReStart.Elapsed, AddressOf TimerAnrMonReStart_Elapsed

            ' Timer stoppen und auflösen
            With TimerAnrMonReStart
                .Stop()
                .AutoReset = False
                .Enabled = False
                .Dispose()
            End With
        End If

        ' Initiiere einen neuen Timer
        NLogger.Debug("Timer für Reaktivierung wird gestartet.")

        ' Setze die Zählvariable auf 0
        RestartTimerIterations = 0

        ' Initiiere den Timer mit Intervall von 2 Sekunden
        TimerAnrMonReStart = New Timer
        With TimerAnrMonReStart
            .Interval = DfltReStartIntervall
            .AutoReset = True
            .Enabled = True
            ' Starte den Timer
            .Start()
        End With

        ' Ereignishandler hinzufügen
        AddHandler TimerAnrMonReStart.Elapsed, AddressOf TimerAnrMonReStart_Elapsed
    End Sub

    Private Sub TimerAnrMonReStart_Elapsed(sender As Object, e As ElapsedEventArgs)
        ' Prüfe, ob die maximale Anzahl an Durchläufen (15) noch nicht erreicht wurde
        If RestartTimerIterations.IsLess(DfltTryMaxRestart) Then
            ' Wenn der Anrufmonitor aktiv ist, dann hat das Wiederverbinden geklappt.
            If Aktiv Then
                ' Halte den TImer an und löse ihn auf
                With TimerAnrMonReStart
                    .Stop()
                    .Dispose()
                End With
                ' Statusmeldung
                NLogger.Info($"Anrufmonitor konnte nach {RestartTimerIterations} Versuchen erfolgreich neu gestartet werden.")
            Else
                ' Erhöhe den Wert der durchgeführten Iterationen
                RestartTimerIterations += 1
                ' Statusmeldung
                NLogger.Debug($"Timer: Starte {RestartTimerIterations}. Versuch den Anrufmonitor zu starten.")
                ' Starte den nächsten Versuch den Anrufmonitor zu verbinden
                StartAnrMon()
            End If
        Else
            ' Es konnte keine Verbindung zur Fritz!Box aufgebaut werden.
            NLogger.Warn($"Anrufmonitor konnte nach {RestartTimerIterations} Versuchen nicht neu gestartet werden.")

            ' Ereignishandler entfernen
            RemoveHandler TimerAnrMonReStart.Elapsed, AddressOf TimerAnrMonReStart_Elapsed

            ' Timer stoppen und auflösen
            With TimerAnrMonReStart
                .Stop()
                .AutoReset = False
                .Enabled = False
                .Dispose()
            End With
        End If
        ' Ribbon aktualisieren
        ThisAddIn.POutlookRibbons.RefreshRibbon()
    End Sub
#End Region

#Region "Anrufmonitor Events"
    Private Sub AnrMonTCPClient_Message(sender As Object, e As NotifyEventArgs(Of String)) Handles AnrMonTCPClient.Message

        Dim AktivesTelefonat As Telefonat
        Dim FBStatus As String = e.Value.RegExRemove("\r\n?|\n") ' Entferne den Zeilenumbruch
        Dim FBStatusSplit As String() = FBStatus.Split(AnrMon_Delimiter)

        ' Hier die Daten des Fritz!Box Anrufmonitors weitergeben
        NLogger.Info($"AnrMonAktion: {FBStatus}")

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

    Private Sub AnrMonTCPClient_ErrorOccured(Sender As AnrMonClient) Handles AnrMonTCPClient.ErrorOccured
        NLogger.Warn($"Anrufmonitor wurde unerwartet getrennt von {XMLData.POptionen.ValidFBAdr}:{AnrMon_Port}")
        ' Wieververbinden versuchen
        Reaktivieren()
    End Sub

    Private Sub AnrMonTCPClient_Disposed(Sender As AnrMonClient) Handles AnrMonTCPClient.Disposed
        'Aktiv = False
        ThisAddIn.POutlookRibbons.RefreshRibbon()
        NLogger.Info($"Anrufmonitor getrennt von {XMLData.POptionen.ValidFBAdr}:{AnrMon_Port}")
    End Sub
#End Region

End Class
