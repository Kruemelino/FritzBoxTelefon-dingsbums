Imports System.Timers
Imports System.IO.Path

Friend Class formAnrMon
    Private C_DP As DataProvider
    Private C_hf As Helfer
    Private C_OLI As OutlookInterface
    Private C_KF As Contacts
    Private C_Telefonat As C_Telefonat

    Private WithEvents TimerAktualisieren As Timer
    Public AnrmonClosed As Boolean

    Public Sub New(ByVal TelefonatKlasse As C_Telefonat, _
                   ByVal Aktualisieren As Boolean, _
                   ByVal DataProviderKlasse As DataProvider, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal AnrufMon As AnrufMonitor, _
                   ByVal OutlInter As OutlookInterface, _
                   ByVal KontaktFunktionen As Contacts)

        InitializeComponent()
        C_hf = HelferKlasse
        C_DP = DataProviderKlasse
        C_OLI = OutlInter
        C_KF = KontaktFunktionen
        C_Telefonat = TelefonatKlasse
        'aID = iAnrufID

        AnrMonausfüllen()
        AnrmonClosed = False

        Dim OInsp As Outlook.Inspector = Nothing
        If Aktualisieren Then ' hä?

            TimerAktualisieren = C_hf.SetTimer(100)
            If TimerAktualisieren Is Nothing Then
                C_hf.LogFile("formAnrMon.New: TimerNeuStart nicht gestartet")
            End If
        End If
        C_OLI.InspectorVerschieben(True)

        With PopUpAnrMon
            .ShowDelay = C_DP.P_TBEnblDauer * 1000
            .AutoAusblenden = C_DP.P_CBAutoClose
            .PositionsKorrektur = New Drawing.Size(C_DP.P_TBAnrMonX, C_DP.P_TBAnrMonY)
            .EffektMove = C_DP.P_CBAnrMonMove
            .EffektTransparenz = C_DP.P_CBAnrMonTransp

            .Startpunkt = CType(C_DP.P_CBoxAnrMonStartPosition, FritzBoxDial.PopUpAnrMon.eStartPosition) 'FritzBoxDial.PopUpAnrMon.eStartPosition.BottomRight
            .MoveDirecktion = CType(C_DP.P_CBoxAnrMonMoveDirection, FritzBoxDial.PopUpAnrMon.eMoveDirection) 'FritzBoxDial.PopUpAnrMon.eMoveDirection.X

            .EffektMoveGeschwindigkeit = 44 - C_DP.P_TBAnrMonMoveGeschwindigkeit * 4
            .Popup()
        End With
        C_OLI.InspectorVerschieben(False)
    End Sub

    Sub AnrMonausfüllen()

        With PopUpAnrMon
            If C_Telefonat.TelNr = C_DP.P_Def_StringUnknown Then
                With .OptionsMenu
                    .Items("ToolStripMenuItemRückruf").Enabled = False ' kein Rückruf im Fall 1
                    .Items("ToolStripMenuItemKopieren").Enabled = False ' in dem Fall sinnlos
                    .Items("ToolStripMenuItemKontaktöffnen").Text = "Einen neuen Kontakt erstellen"
                End With
            End If
            ' Uhrzeit des Telefonates eintragen
            .Uhrzeit = C_Telefonat.Zeit.ToString
            ' Telefonnamen eintragen
            .TelName = C_Telefonat.TelName & CStr(IIf(C_DP.P_CBShowMSN, " (" & C_Telefonat.MSN & ")", C_DP.P_Def_StringEmpty))

            If Not C_Telefonat.olContact Is Nothing Or Not C_Telefonat.vCard = C_DP.P_Def_StringEmpty Then

                If Not TimerAktualisieren Is Nothing Then TimerAktualisieren = C_hf.KillTimer(TimerAktualisieren)
                ' Kontakt einblenden wenn in Outlook gefunden
                With C_Telefonat
                    If Not .vCard = C_DP.P_Def_StringEmpty And .olContact Is Nothing Then
                        .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False)
                    End If
                End With
                Try
                    C_KF.KontaktInformation(C_Telefonat.olContact, .AnrName, .Firma)
                    If C_DP.P_CBAnrMonContactImage Then
                        Dim BildPfad = C_KF.KontaktBild(C_Telefonat.olContact)
                        If Not BildPfad = C_DP.P_Def_StringEmpty Then
                            PopUpAnrMon.Image = Drawing.Image.FromFile(BildPfad)
                            ' Seitenverhältnisse anpassen
                            Dim Bildgröße As New Drawing.Size(PopUpAnrMon.ImageSize.Width, CInt((PopUpAnrMon.ImageSize.Width * PopUpAnrMon.Image.Size.Height) / PopUpAnrMon.Image.Size.Width))
                            PopUpAnrMon.ImageSize = Bildgröße
                        End If
                    End If
                Catch ex As Exception
                    C_hf.LogFile("formAnrMon: Fehler beim Öffnen des Kontaktes " & C_Telefonat.Anrufer & " (" & ex.Message & ")")
                    .Firma = C_DP.P_Def_StringEmpty
                    If C_Telefonat.Anrufer = C_DP.P_Def_StringEmpty Then
                        .TelNr = C_DP.P_Def_StringEmpty
                        .AnrName = C_Telefonat.TelNr
                    Else
                        .TelNr = C_Telefonat.TelNr
                        .AnrName = C_Telefonat.Anrufer
                    End If
                End Try
                .TelNr = C_Telefonat.TelNr
            Else
                .Firma = C_DP.P_Def_StringEmpty
                If C_Telefonat.Anrufer = C_DP.P_Def_StringEmpty Then
                    .TelNr = C_DP.P_Def_StringEmpty
                    .AnrName = C_Telefonat.TelNr
                Else
                    .TelNr = C_Telefonat.TelNr
                    .AnrName = C_Telefonat.Anrufer
                End If
            End If
        End With
    End Sub

    Private Sub PopUpAnrMon_Close() Handles PopUpAnrMon.Close
        PopUpAnrMon.Hide()
    End Sub

    Private Sub ToolStripMenuItemRückruf_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ToolStripMenuItemRückruf.Click
        ThisAddIn.P_WClient.Rueckruf(C_Telefonat)
    End Sub

    Private Sub ToolStripMenuItemKopieren_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ToolStripMenuItemKopieren.Click
        With PopUpAnrMon
            My.Computer.Clipboard.SetText(.AnrName & CStr(IIf(Len(.TelNr) = 0, "", " (" & .TelNr & ")")))
        End With
    End Sub

    Private Sub PopUpAnrMon_Closed() Handles PopUpAnrMon.Closed
        AnrmonClosed = True
        If Not TimerAktualisieren Is Nothing Then TimerAktualisieren = C_hf.KillTimer(TimerAktualisieren)
    End Sub

    Private Sub ToolStripMenuItemKontaktöffnen_Click() Handles ToolStripMenuItemKontaktöffnen.Click, PopUpAnrMon.LinkClick
        ' blendet den Kontakteintrag des Anrufers ein
        ' ist kein Kontakt vorhanden, dann wird einer angelegt und mit den vCard-Daten ausgefüllt
        With C_Telefonat
            If Not .olContact Is Nothing Then
                .olContact.Display()
            Else
                C_KF.ZeigeKontakt(.KontaktID, .StoreID, .TelNr)
            End If
        End With
    End Sub

    Private Sub TimerAktualisieren_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerAktualisieren.Elapsed
        Dim VergleichString As String = PopUpAnrMon.AnrName
        AnrMonausfüllen()
        If Not VergleichString = PopUpAnrMon.AnrName Then TimerAktualisieren = C_hf.KillTimer(TimerAktualisieren)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
