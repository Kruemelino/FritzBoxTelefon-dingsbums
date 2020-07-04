Imports System.ComponentModel
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Timers

Friend Class Anrufmonitor
#Region "Eigenschaften"
    Friend Property Aktiv As Boolean
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
    Private WithEvents TimerPowerModeResume As Timers.Timer
    Private Property RestartTimerIterations As Integer
#End Region
    Private WithEvents AnrMonTCPClient As AnrMonClient

    Public Sub New()
        AktiveTelefonate = New List(Of Telefonat)
    End Sub

    Friend Sub StartStopAnrMon()
        If AnrMonTCPClient?.Verbunden Then
            ' Halte den Anrufmonitor an
            AnrMonTCPClient.Disconnect()
            Aktiv = False
        Else
            ' Starte den Anrufmonitor
            Dim IP As IPAddress = IPAddress.Loopback

            If IPAddress.TryParse(XMLData.POptionen.PValidFBAdr, IP) Then
                Dim TC As New TcpClient With {.ExclusiveAddressUse = False}

                Try
                    TC.Connect(New IPEndPoint(IP, FritzBoxDefault.PDfltFBAnrMonPort))
                Catch ex As SocketException
                    TC.Close()
                    NLogger.Error("Anrufmonitor", ex)
                End Try

                If TC.Connected Then
                    ' Info Message für das Log
                    NLogger.Info("Anrufmonitor verbunden zu {0}:{1}", IP.ToString, FritzBoxDefault.PDfltFBAnrMonPort)
                    AnrMonTCPClient = New AnrMonClient(TC)

                    ' Verbinden
                    AnrMonTCPClient.Connect()

                    ' Statuseigenschaft setzen
                    Aktiv = True
                Else
                    ' Statuseigenschaft setzen
                    Aktiv = False

                    ' Info Message für das Log
                    NLogger.Info("Anrufmonitor nicht verbunden zu {0}:{1}", IP.ToString, FritzBoxDefault.PDfltFBAnrMonPort)
                End If
            Else
                ' Statuseigenschaft setzen
                Aktiv = False
            End If
        End If
        ' Ribbon aktualisieren
        ThisAddIn.POutlookRibbons.RefreshRibbon()
    End Sub

    Friend Sub Stopp()
        If AnrMonTCPClient?.Verbunden Then
            ' TCP-Client trennen
            AnrMonTCPClient.Disconnect()

            ' Info Message für das Log
            NLogger.Debug("Anrufmonitor abgehalten")
        End If

        ' Statuseigenschaft setzen
        Aktiv = False
    End Sub

    Friend Sub RestartOnResume()
        ' Falls der Anrufmonitor aktiv sein sollte, dann halte ihn sicherheitshalber an.
        If Aktiv Then Stopp()


        ' Initiiere einen neuen Timer
        If TimerPowerModeResume Is Nothing Then
            ' Setze die Zählvariable auf 0
            RestartTimerIterations = 0

            ' Initiiere den Timer mit Intervall von 3 Sekunden
            TimerPowerModeResume = SetTimer(PDfltReStartIntervall)

            ' Starte den Timer
            TimerPowerModeResume.Start()
        End If
    End Sub


    Private Sub AnrMonTCPClient_Disposed(Sender As AnrMonClient) Handles AnrMonTCPClient.Disposed
        Aktiv = False
        ThisAddIn.POutlookRibbons.RefreshRibbon()
        NLogger.Info("Anrufmonitor getrennt von {0}:{1}", XMLData.POptionen.PValidFBAdr, FritzBoxDefault.PDfltFBAnrMonPort)
    End Sub

#Region "Timer PowerMode Resume"
    Private Sub TimerPowerModeResume_Elapsed(sender As Object, e As ElapsedEventArgs) Handles TimerPowerModeResume.Elapsed
        ' Prüfe, ob die maximale Anzahl an Durchläufen (15) noch nicht erreicht wurde
        If RestartTimerIterations.IsLess(PDfltTryMaxRestart) Then
            ' Wenn der Anrufmonitor aktiv ist, dann hat das wiederverbinden geklappt.
            If Aktiv Then
                ' Halte den TImer an und löse ihn auf
                With TimerPowerModeResume
                    .Stop()
                    .Dispose()
                End With
                ' Statusmeldung
                NLogger.Info("Anrufmonitor nach PowerMode Resume gestartet.")
            Else
                ' Erhöhe den Wert der durchgeführten Iterationen
                RestartTimerIterations += 1
                ' Starte den nächsten Versuch den Anrufmonitor zu verbinden
                StartStopAnrMon()
            End If
        Else
            ' Es konnte keine Verbindung zur Fritz!Box aufgebaut werden.
            NLogger.Error("Anrufmonitor nach PowerMode Resume nicht gestartet.")
            ' Halte den TImer an und löse ihn auf
            With TimerPowerModeResume
                .Stop()
                .Dispose()
            End With
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
                AktivesTelefonat = New Telefonat
                ' Füge Ereignishandler hinzu
                AddHandler AktivesTelefonat.Popup, AddressOf AnrMon_Popup
                ' Setze die übergebenen Daten
                AktivesTelefonat.SetAnrMonRING = FBStatusSplit
                ' Füge das Telefonat der Liste hinzu
                AktiveTelefonate.Add(AktivesTelefonat)

            Case AnrMon_CALL
                ' Neues Telefonat erzeugen und Daten des Anrufmonitors übergeben
                AktivesTelefonat = New Telefonat With {.SetAnrMonCALL = FBStatusSplit}
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

    ''' <summary>
    ''' Routine zum Initialisieren der Einblendung des Anrfomitors
    ''' </summary>
    Private Sub AnrMon_Popup(AktivesTelefonat As Telefonat)
        Dim t = New Thread(Sub()
                               If Not VollBildAnwendungAktiv() Then
                                   If AktivesTelefonat.AnrMonPopUp Is Nothing Then
                                       NLogger.Debug("Blende einen neuen Anrufmonitor ein")
                                       ' Blende einen neuen Anrufmonitor ein
                                       AktivesTelefonat.AnrMonPopUp = New Popup
                                       AktivesTelefonat.AnrMonPopUp.AnrMonEinblenden(AktivesTelefonat)

                                       While AktivesTelefonat.AnrMonPopUp.Eingeblendet
                                           Windows.Forms.Application.DoEvents()
                                           Thread.Sleep(100)
                                       End While

                                   Else
                                       NLogger.Debug("Aktualisiere den Anrufmonitor")
                                       ' Aktualisiere den Anrufmonitor
                                       AktivesTelefonat.AnrMonPopUp.UpdateAnrMon(AktivesTelefonat)
                                   End If
                               End If
                           End Sub)
        t.SetApartmentState(ApartmentState.STA)
        t.Start()
    End Sub



#End Region

End Class
