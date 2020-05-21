Imports System.Net
Imports System.Net.Sockets

Friend Class Anrufmonitor

    Private Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

    Private WithEvents AnrMonTCPClient As AnrMonClient

    Friend Property Aktiv As Boolean
    Friend Shared ReadOnly Property AnrMon_RING As String = "RING"
    Friend Shared ReadOnly Property AnrMon_CALL As String = "CALL"
    Friend Shared ReadOnly Property AnrMon_CONNECT As String = "CONNECT"
    Friend Shared ReadOnly Property AnrMon_DISCONNECT As String = "DISCONNECT"
    Friend Shared ReadOnly Property AnrMon_Delimiter As String = ";"

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
                    Aktiv = True
                    ThisAddIn.POutlookRibbons.RefreshRibbon()
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

    Friend Sub AnrMonSimulation(ByVal AnrMonSim As String)
        'If AnrMonSim.IsNotStringEmpty Then TCPr_DataAvailable(AnrMonSim, True)
    End Sub

#Region "Anrufmonitor"
    Private Sub AnrMonTCPClient_Message(sender As Object, e As NotifyEventArgs(Of String)) Handles AnrMonTCPClient.Message

        Dim tmpTelefonat As Telefonat
        Dim FBStatus As String = e.Value
        Dim FBStatusSplit As String() = FBStatus.Split(AnrMon_Delimiter)

        ' Hier die Daten des Fritz!Box Anrufmonitors weitergeben
        NLogger.Info("AnrMonAktion: {0}", FBStatus)

        'Schauen ob "RING", "CALL", "CONNECT" oder "DISCONNECT" übermittelt wurde
        Select Case FBStatusSplit(1)
            Case AnrMon_RING
                ' Neues Telefonat erzeugen und Daten des Anrufmonitors übergeben
                tmpTelefonat = New Telefonat With {.SetAnrMonRING = FBStatusSplit}
                AktiveTelefonate.Add(tmpTelefonat)

                ' Halte den Thread am Leben. Ansonsten wird der Anrufmonitor nicht korrekt eingeblendet
                If tmpTelefonat.EigeneTelNr.Überwacht And Not False Then
                    While tmpTelefonat.AnrMonPopUp.Eingeblendet
                        Windows.Forms.Application.DoEvents()
                    End While
                End If
            Case AnrMon_CALL
                ' Neues Telefonat erzeugen und Daten des Anrufmonitors übergeben
                tmpTelefonat = New Telefonat With {.SetAnrMonCALL = FBStatusSplit}
                AktiveTelefonate.Add(tmpTelefonat)

            Case AnrMon_CONNECT
                ' Vorhandenes Telefonat ermitteln und Daten des Anrufmonitors übergeben
                tmpTelefonat = AktiveTelefonate.Find(Function(TE) TE.ID.AreEqual(CInt(FBStatusSplit(2))))
                If tmpTelefonat IsNot Nothing Then tmpTelefonat.SetAnrMonCONNECT = FBStatusSplit

            Case AnrMon_DISCONNECT
                ' Vorhandenes Telefonat ermitteln und Daten des Anrufmonitors übergeben
                tmpTelefonat = AktiveTelefonate.Find(Function(TE) TE.ID.AreEqual(CInt(FBStatusSplit(2))))
                If tmpTelefonat IsNot Nothing Then tmpTelefonat.SetAnrMonDISCONNECT = FBStatusSplit
                ' Das Gespräch ist beendet. Entferne dieses Telefonat aus der Liste aktiver Telefonate
                AktiveTelefonate.Remove(tmpTelefonat)

        End Select


    End Sub

#End Region

End Class
