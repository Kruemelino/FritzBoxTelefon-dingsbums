Imports System.Timers
Imports System.IO.Path

Friend Class formAnrMon
    Private C_DP As DataProvider
    Private C_hf As Helfer
    Private C_AnrMon As AnrufMonitor
    Private C_OLI As OutlookInterface
    Private C_KF As Contacts

    Private WithEvents TimerAktualisieren As Timer

    Private aID As Integer

    Private TelefonName As String
    Private TelNr As String              ' TelNr des Anrufers
    Private KontaktID As String              ' KontaktID des Anrufers
    Private StoreID As String
    Private MSN As String

    Public AnrmonClosed As Boolean



    Public Sub New(ByVal iAnrufID As Integer, _
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
        C_AnrMon = AnrufMon
        C_KF = KontaktFunktionen

        aID = iAnrufID

        AnrMonausfüllen()
        AnrmonClosed = False

        Dim OInsp As Outlook.Inspector = Nothing
        If Aktualisieren Then
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
        ' Diese Funktion nimmt Daten aus der Registry und öffnet 'formAnMon'.
        Dim AnrName As String              ' Name des Anrufers
        Dim Uhrzeit As String
        'LA(0) = Zeit
        'LA(1) = Anrufer
        'LA(2) = TelNr
        'LA(3) = MSN
        'LA(4) = StoreID
        'LA(5) = KontaktID

        Dim xPathTeile As New ArrayList
        With xPathTeile
            .Add("LetzterAnrufer")
            .Add("Eintrag[@ID = """ & aID & """]")
            .Add("Zeit")
            Uhrzeit = C_DP.Read(xPathTeile, CStr(DateTime.Now))

            .Item(.Count - 1) = "Anrufer"
            AnrName = C_DP.Read(xPathTeile, "")

            .Item(.Count - 1) = "TelNr"
            TelNr = C_DP.Read(xPathTeile, C_DP.P_Def_StringUnknown)

            .Item(.Count - 1) = "MSN"
            MSN = C_DP.Read(xPathTeile, "")

            .Item(.Count - 1) = "StoreID"
            StoreID = C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne)

            .Item(.Count - 1) = "KontaktID"
            KontaktID = C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne)
        End With

        TelefonName = C_AnrMon.TelefonName(MSN)
        With PopUpAnrMon
            If TelNr = C_DP.P_Def_StringUnknown Then
                With .OptionsMenu
                    .Items("ToolStripMenuItemRückruf").Enabled = False ' kein Rückruf im Fall 1
                    .Items("ToolStripMenuItemKopieren").Enabled = False ' in dem Fall sinnlos
                    .Items("ToolStripMenuItemKontaktöffnen").Text = "Einen neuen Kontakt erstellen"
                End With
            End If
            ' Uhrzeit des Telefonates eintragen
            .Uhrzeit = Uhrzeit
            ' Telefonnamen eintragen
            .TelName = TelefonName & CStr(IIf(C_DP.P_CBShowMSN, " (" & MSN & ")", C_DP.P_Def_StringEmpty))

            If Not Strings.Left(KontaktID, 2) = C_DP.P_Def_ErrorMinusOne Then
                If Not TimerAktualisieren Is Nothing Then TimerAktualisieren = C_hf.KillTimer(TimerAktualisieren)
                ' Kontakt einblenden wenn in Outlook gefunden
                Try
                    C_OLI.KontaktInformation(KontaktID, StoreID, PopUpAnrMon.AnrName, PopUpAnrMon.Firma)
                    If C_DP.P_CBAnrMonContactImage Then
                        Dim BildPfad = C_OLI.KontaktBild(KontaktID, StoreID)
                        If Not BildPfad = C_DP.P_Def_StringEmpty Then
                            PopUpAnrMon.Image = Drawing.Image.FromFile(BildPfad)
                            ' Seitenverhältnisse anpassen
                            Dim Bildgröße As New Drawing.Size(PopUpAnrMon.ImageSize.Width, CInt((PopUpAnrMon.ImageSize.Width * PopUpAnrMon.Image.Size.Height) / PopUpAnrMon.Image.Size.Width))
                            PopUpAnrMon.ImageSize = Bildgröße
                        End If
                    End If
                Catch ex As Exception
                    C_hf.LogFile("formAnrMon: Fehler beim Öffnen des Kontaktes " & AnrName & " (" & ex.Message & ")")
                    .Firma = C_DP.P_Def_StringEmpty
                    If AnrName = C_DP.P_Def_StringEmpty Then
                        .TelNr = C_DP.P_Def_StringEmpty
                        .AnrName = TelNr
                    Else
                        .TelNr = TelNr
                        .AnrName = AnrName
                    End If
                End Try

                .TelNr = TelNr
            Else
                .Firma = C_DP.P_Def_StringEmpty
                If AnrName = C_DP.P_Def_StringEmpty Then
                    .TelNr = C_DP.P_Def_StringEmpty
                    .AnrName = TelNr
                Else
                    .TelNr = TelNr
                    .AnrName = AnrName
                End If
            End If
        End With
    End Sub

    Private Sub PopUpAnrMon_Close() Handles PopUpAnrMon.Close
        PopUpAnrMon.Hide()
    End Sub

    Private Sub ToolStripMenuItemRückruf_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ToolStripMenuItemRückruf.Click
        ThisAddIn.P_WClient.Rueckruf(aID)
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
        C_KF.ZeigeKontakt(KontaktID, StoreID, TelNr, True)
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
