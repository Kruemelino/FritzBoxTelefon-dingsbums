Imports System.ComponentModel
Imports System.Net
Imports System.Net.Sockets

Friend Class Anrufmonitor

    Private Property NLogger As NLog.Logger = LogManager.GetCurrentClassLogger

    Private WithEvents AnrMonTCPClient As AnrMonClient

    Friend Property Aktiv As Boolean
    Friend Shared ReadOnly Property AnrMon_RING As String = "RING"
    Friend Shared ReadOnly Property AnrMon_CALL As String = "CALL"
    Friend Shared ReadOnly Property AnrMon_CONNECT As String = "CONNECT"
    Friend Shared ReadOnly Property AnrMon_DISCONNECT As String = "DISCONNECT"
    Friend Shared ReadOnly Property AnrMon_Delimiter As String = ";"


    Private BWAnrMonPopUpList As List(Of BackgroundWorker)
    Friend Property AktiveTelefonate As List(Of Telefonat)

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
                Dim EP As IPEndPoint = New IPEndPoint(IP, FritzBoxDefault.PDfltFBAnrMonPort)
                Dim TC As New TcpClient With {.ExclusiveAddressUse = False}

                Try
                    TC.Connect(EP)
                Catch ex As SocketException
                    TC.Close()
                    NLogger.Error("Anrufmonitor", ex)
                End Try

                If TC.Connected Then
                    NLogger.Info("Anrufmonitor verbunden zu {0}:{1}", IP.ToString, FritzBoxDefault.PDfltFBAnrMonPort)
                    AnrMonTCPClient = New AnrMonClient(TC)

                    ' Verbinden
                    AnrMonTCPClient.Connect()

                    ' Ribbon umschalten
                    ThisAddIn.POutlookRibbons.RefreshRibbon()

                    Aktiv = True
                Else
                    NLogger.Info("Anrufmonitor nicht verbunden zu {0}:{1}", IP.ToString, FritzBoxDefault.PDfltFBAnrMonPort)
                End If

            End If
        End If

    End Sub

    Friend Sub StopAnrMon()
        If AnrMonTCPClient?.Verbunden Then
            AnrMonTCPClient.Disconnect()
            Aktiv = False
        End If
    End Sub


    Private Sub AnrMonTCPClient_Disposed(Sender As AnrMonClient) Handles AnrMonTCPClient.Disposed
        Aktiv = False
        ThisAddIn.POutlookRibbons.RefreshRibbon()
        NLogger.Info("Anrufmonitor getrennt von {0}:{1}", XMLData.POptionen.PValidFBAdr, FritzBoxDefault.PDfltFBAnrMonPort)
    End Sub

#Region "Anrufmonitor"
    Private Sub AnrMonTCPClient_Message(sender As Object, e As NotifyEventArgs(Of String)) Handles AnrMonTCPClient.Message

        Dim AktivesTelefonat As Telefonat
        Dim FBStatus As String = e.Value.RegExReplace("\r\n?|\n", PDfltStringEmpty) ' Entferne den Zeilenumbruch
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
        Dim BWAnrMonPopUp As New BackgroundWorker

        If BWAnrMonPopUpList Is Nothing Then BWAnrMonPopUpList = New List(Of BackgroundWorker)
        With BWAnrMonPopUp
            ' Füge Ereignishandler hinzu
            AddHandler .DoWork, AddressOf BWAnrMonPopUp_DoWork
            AddHandler .RunWorkerCompleted, AddressOf BWAnrMonPopUp_RunWorkerCompleted
            ' Setze Flags
            .WorkerSupportsCancellation = False
            .WorkerReportsProgress = False
            ' Und los...
            NLogger.Debug("Starte {0}. Backgroundworker für Anrufmonitor: {1}  {2}", BWAnrMonPopUpList.Count + 1, AktivesTelefonat.GegenstelleTelNr.Unformatiert, AktivesTelefonat.Anrufer)
            .RunWorkerAsync(AktivesTelefonat)
        End With

        ' Füge dern Backgroundworker der Liste hinzu
        BWAnrMonPopUpList.Add(BWAnrMonPopUp)
    End Sub

    Private Sub BWAnrMonPopUp_DoWork(sender As Object, e As DoWorkEventArgs)
        Dim AktivesTelefonat As Telefonat = CType(e.Argument, Telefonat)

        If Not VollBildAnwendungAktiv() Then
            If AktivesTelefonat.AnrMonPopUp Is Nothing Then
                NLogger.Debug("Blende einen neuen Anrufmonitor ein")
                ' Blende einen neuen Anrufmonitor ein
                AktivesTelefonat.AnrMonPopUp = New Popup
                AktivesTelefonat.AnrMonPopUp.AnrMonEinblenden(AktivesTelefonat)

                While AktivesTelefonat.AnrMonPopUp.Eingeblendet
                    Windows.Forms.Application.DoEvents()
                    Threading.Thread.Sleep(100)
                End While

            Else
                NLogger.Debug("Aktualisiere den Anrufmonitor")
                ' Aktualisiere den Anrufmonitor
                AktivesTelefonat.AnrMonPopUp.UpdateAnrMon(AktivesTelefonat)
            End If
        End If
    End Sub

    Private Sub BWAnrMonPopUp_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs)
        Dim BWAnrMonPopUp As BackgroundWorker = CType(sender, BackgroundWorker)

        ' Backgroundworker aus der Liste entfernen
        BWAnrMonPopUpList.Remove(BWAnrMonPopUp)

        With BWAnrMonPopUp
            ' Ereignishandler entfernen
            RemoveHandler .DoWork, AddressOf BWAnrMonPopUp_DoWork
            RemoveHandler .RunWorkerCompleted, AddressOf BWAnrMonPopUp_RunWorkerCompleted

            BWAnrMonPopUp.Dispose()
            NLogger.Debug("Backgroundworker für Anrufmonitor aufgelöst")
        End With

        ' Liste leeren, wenn kein Element mehr enthalten
        If Not BWAnrMonPopUpList.Any Then
            BWAnrMonPopUpList = Nothing
            NLogger.Debug("Die Liste der Backgroundworker für Anrufmonitor wurde aufgelöst.")
        End If
    End Sub
#End Region

End Class
