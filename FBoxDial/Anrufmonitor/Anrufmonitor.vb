Friend Class Anrufmonitor

    Private WithEvents TCPr As TCPReader

    Friend Property Aktiv As Boolean

    Friend Shared ReadOnly Property AnrMon_RING As String = "RING"
    Friend Shared ReadOnly Property AnrMon_CALL As String = "CALL"
    Friend Shared ReadOnly Property AnrMon_CONNECT As String = "CONNECT"
    Friend Shared ReadOnly Property AnrMon_DISCONNECT As String = "DISCONNECT"
    Friend Shared ReadOnly Property AnrMon_Delimiter As String = ";"

    Private Property AktiveTelefonate As List(Of Telefonat)

    Public Sub New()
        AktiveTelefonate = New List(Of Telefonat)
    End Sub

    Friend Sub StartStopAnrMon()
        If Aktiv Then
            If TCPr IsNot Nothing Then TCPr.Disconnect = True
            Aktiv = False
        Else
            If TCPr Is Nothing Then
                ' Starte TCP-Verbindung zur Fritz!Box
                TCPr = New TCPReader(XMLData.POptionen.PValidFBAdr, FritzBoxDefault.PDfltFBAnrMonPort)
            End If
        End If
    End Sub

    Private Sub TCPr_Connected() Handles TCPr.Connected
        Aktiv = TCPr.Verbunden
        ThisAddIn.POutlookRibbons.RefreshRibbon()
    End Sub

    Friend Sub AnrMonSimulation(ByVal AnrMonSim As String)
        If AnrMonSim.IsNotStringEmpty Then TCPr_DataAvailable(AnrMonSim)
    End Sub


#Region "Anrufmonitor"
    Private Sub TCPr_DataAvailable(FBStatus As String) Handles TCPr.DataAvailable

        Dim tmpTelefonat As Telefonat

        Dim FBStatusSplit As String() = FBStatus.Split(AnrMon_Delimiter)

        ' Hier die Daten des Fritz!Box Anrufmonitors weitergeben

        LogFile("AnrMonAktion: " & FBStatus)

        'Schauen ob "RING", "CALL", "CONNECT" oder "DISCONNECT" übermittelt wurde
        Select Case FBStatusSplit(1)
            Case AnrMon_RING
                ' Neues Telefonat erzeugen und Daten des Anrufmonitors übergeben
                tmpTelefonat = New Telefonat With {.SetAnrMonRING = FBStatusSplit}
                AktiveTelefonate.Add(tmpTelefonat)

            Case AnrMon_CALL
                ' Neues Telefonat erzeugen und Daten des Anrufmonitors übergeben
                tmpTelefonat = New Telefonat With {.SetAnrMonCALL = FBStatusSplit}
                AktiveTelefonate.Add(tmpTelefonat)

            Case AnrMon_CONNECT
                ' Vorhandenes Telefonat ermitteln und Daten des Anrufmonitors übergeben
                tmpTelefonat = AktiveTelefonate.Find(Function(TE) TE.ID.AreEqual(CInt(FBStatusSplit(2))))
                If tmpTelefonat IsNot Nothing Then
                    tmpTelefonat.SetAnrMonCONNECT = FBStatusSplit
                Else

                End If
            Case AnrMon_DISCONNECT
                ' Vorhandenes Telefonat ermitteln und Daten des Anrufmonitors übergeben
                tmpTelefonat = AktiveTelefonate.Find(Function(TE) TE.ID.AreEqual(CInt(FBStatusSplit(2))))
                If tmpTelefonat IsNot Nothing Then
                    tmpTelefonat.SetAnrMonDISCONNECT = FBStatusSplit
                Else

                End If
                ' Das Gespräch ist beendet. Entferne dieses Telefonat aus der Liste aktiver Telefonate
                AktiveTelefonate.Remove(tmpTelefonat)
        End Select
    End Sub

#End Region
End Class
