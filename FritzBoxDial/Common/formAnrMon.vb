Imports System.Timers
Imports System.IO.Path
Imports System.Drawing

Friend Class formAnrMon
    Private C_DP As DataProvider
    Private C_hf As Helfer
    Private C_OLI As OutlookInterface
    Private C_KF As Contacts
    Private C_AnrMon As AnrufMonitor

    Private WithEvents TimerAktualisieren As Timer
    Public AnrmonClosed As Boolean

    Public Sub New(ByVal Aktualisieren As Boolean, _
                   ByVal DataProviderKlasse As DataProvider, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal AnrufMonitorKlasse As AnrufMonitor, _
                   ByVal OutlInter As OutlookInterface, _
                   ByVal KontaktFunktionen As Contacts)

        InitializeComponent()
        C_hf = HelferKlasse
        C_DP = DataProviderKlasse
        C_OLI = OutlInter
        C_KF = KontaktFunktionen
        C_AnrMon = AnrufMonitorKlasse
        'aID = iAnrufID

        AnrMonausf�llen()
        AnrmonClosed = False

        Dim OInsp As Outlook.Inspector = Nothing
        If Aktualisieren Then
            TimerAktualisieren = C_hf.SetTimer(100)
            If TimerAktualisieren Is Nothing Then
                C_hf.LogFile("formAnrMon_New: TimerNeuStart nicht gestartet")
            End If
        End If
        C_OLI.KeepoInspAtivated(False)

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
        C_OLI.KeepoInspAtivated(True)
    End Sub

    Sub AnrMonausf�llen()
        With PopUpAnrMon
            If C_AnrMon.LetzterAnrufer.TelNr = C_DP.P_Def_StringUnknown Then
                With .OptionsMenu
                    .Items("ToolStripMenuItemR�ckruf").Enabled = False ' kein R�ckruf im Fall 1
                    .Items("ToolStripMenuItemKopieren").Enabled = False ' in dem Fall sinnlos
                    .Items("ToolStripMenuItemKontakt�ffnen").Text = "Einen neuen Kontakt erstellen"
                End With
            End If
            ' Uhrzeit des Telefonates eintragen
            .Uhrzeit = C_AnrMon.LetzterAnrufer.Zeit.ToString
            ' Telefonnamen eintragen
            .TelName = C_AnrMon.LetzterAnrufer.TelName & CStr(IIf(C_DP.P_CBShowMSN, " (" & C_AnrMon.LetzterAnrufer.MSN & ")", C_DP.P_Def_StringEmpty))

            If Not TimerAktualisieren Is Nothing Then TimerAktualisieren = C_hf.KillTimer(TimerAktualisieren)
            ' Kontakt einblenden wenn in Outlook gefunden
            With C_AnrMon.LetzterAnrufer
                If .olContact Is Nothing Then
                    ''kontakt erstellen, wenn vcard vorhanden
                    'If Not .vCard = C_DP.P_Def_StringEmpty Then
                    '    .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False)
                    'End If
                Else
                    'Kontaktbild ermitteln
                    If C_DP.P_CBAnrMonContactImage Then
                        C_AnrMon.LetzterAnrufer.PfadKontaktBild = C_KF.KontaktBild(C_AnrMon.LetzterAnrufer.olContact)
                        If Not C_AnrMon.LetzterAnrufer.PfadKontaktBild = C_DP.P_Def_StringEmpty Then
                            Using fs As New IO.FileStream(C_AnrMon.LetzterAnrufer.PfadKontaktBild, IO.FileMode.Open)
                                PopUpAnrMon.Image = Image.FromStream(fs)
                            End Using
                            'Kontaktbild l�schen
                            C_KF.DelKontaktBild(C_AnrMon.LetzterAnrufer.PfadKontaktBild)
                            ' Seitenverh�ltnisse anpassen
                            Dim Bildgr��e As New Size(PopUpAnrMon.ImageSize.Width, CInt((PopUpAnrMon.ImageSize.Width * PopUpAnrMon.Image.Size.Height) / PopUpAnrMon.Image.Size.Width))
                            PopUpAnrMon.ImageSize = Bildgr��e
                        End If
                    End If
                End If
            End With

            If C_AnrMon.LetzterAnrufer.Anrufer = C_DP.P_Def_StringEmpty Then
                .TelNr = C_DP.P_Def_StringEmpty
                .AnrName = C_AnrMon.LetzterAnrufer.TelNr
            Else
                .TelNr = C_AnrMon.LetzterAnrufer.TelNr
                .AnrName = C_AnrMon.LetzterAnrufer.Anrufer
            End If
            .Firma = C_AnrMon.LetzterAnrufer.Companies
        End With
    End Sub

    Private Function GetImage(path As String) As Image
        If Not IO.File.Exists(path) Then Throw New IO.FileNotFoundException
        Using fs As New IO.FileStream(path, IO.FileMode.Open)
            Return Image.FromStream(fs)
            'Return CType(, Image)
        End Using
    End Function

    Private Sub PopUpAnrMon_Close() Handles PopUpAnrMon.Close
        PopUpAnrMon.Hide()
    End Sub

    Private Sub ToolStripMenuItemR�ckruf_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ToolStripMenuItemR�ckruf.Click
        ThisAddIn.P_WClient.Rueckruf(C_AnrMon.LetzterAnrufer)
    End Sub

    Private Sub ToolStripMenuItemKopieren_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ToolStripMenuItemKopieren.Click
        With PopUpAnrMon
            My.Computer.Clipboard.SetText(.AnrName & CStr(IIf(Len(.TelNr) = 0, "", " (" & .TelNr & ")")))
        End With
    End Sub

    Private Sub ToolStripMenuItemKontakt�ffnen_Click() Handles ToolStripMenuItemKontakt�ffnen.Click, PopUpAnrMon.LinkClick
        ' blendet den Kontakteintrag des Anrufers ein
        ' ist kein Kontakt vorhanden, dann wird einer angelegt und mit den vCard-Daten ausgef�llt
        With C_AnrMon.LetzterAnrufer
            If Not .KontaktID = C_DP.P_Def_ErrorMinusOne And Not .StoreID = C_DP.P_Def_ErrorMinusOne Then
                .olContact = C_KF.GetOutlookKontakt(.KontaktID, .StoreID)
            End If
            If Not .olContact Is Nothing Then
                .olContact.Display()
            Else
                C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False).Display()
            End If
        End With
    End Sub

    Private Sub TimerAktualisieren_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerAktualisieren.Elapsed
        Dim VergleichString As String = PopUpAnrMon.AnrName
        AnrMonausf�llen()
        If Not VergleichString = PopUpAnrMon.AnrName Then TimerAktualisieren = C_hf.KillTimer(TimerAktualisieren)
    End Sub

    Private Sub PopUpAnrMon_Closed() Handles PopUpAnrMon.Closed
        AnrmonClosed = True
        If Not TimerAktualisieren Is Nothing Then TimerAktualisieren = C_hf.KillTimer(TimerAktualisieren)
    End Sub

    'Protected Overrides Sub Finalize()
    '    MyBase.Finalize()
    'End Sub
End Class
