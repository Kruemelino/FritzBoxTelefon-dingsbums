Imports System.Windows.Forms
Imports System.Net
Imports System.IO
Imports System.Text

Friend Class formWählbox
    Private WithEvents BWLogin As New System.ComponentModel.BackgroundWorker

    Private C_DP As DataProvider
    Private C_hf As Helfer
    Private C_FBox As FritzBox
    Private C_GUI As GraphicalUserInterface
    Private C_Phoner As PhonerInterface
    Private C_KF As Contacts
    Private Client As New Sockets.TcpClient()
    Private WithEvents TimerSchließen As System.Timers.Timer
    Private CallNr As System.Threading.Thread

    Delegate Sub SchließeForm()
    Delegate Sub DlgStatusText()
    Delegate Sub DlgAnAus()

    Private StatusText As String ' Wird für Delegaten DlgStatusText benötigt
    Private AnAus As Boolean ' Wird für Delegaten DlgAnAus benötigt
    Private Element As Control ' Wird für Delegaten DlgAnAus benötigt
    Private WählboxBereit As Boolean = False ' Erst wenn True, kann gewählt werden
    Private SID As String
    Private bDirektwahl As Boolean
    Private _Dialing As Boolean = False
    Private Nebenstellen As String()
    ' Phoner
    Private PhonerCall As Boolean = False
    Private UsePhonerOhneFritzBox As Boolean = False
    Private PhonerFon As Integer = -1

    Structure Argument
        Dim TelNr As String
        Dim clir As Boolean
        Dim festnetz As Boolean
        Dim fonanschluss As String
    End Structure

#Region "Properties"
    Public Property P_Dialing() As Boolean
        Get
            Return _Dialing
        End Get
        Set(ByVal value As Boolean)
            _Dialing = value
        End Set
    End Property
#End Region

    Public Sub New(ByVal Direktwahl As Boolean, _
                   ByVal XMLKlasse As DataProvider, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal InterfacesKlasse As GraphicalUserInterface, _
                   ByVal FritzBoxKlasse As FritzBox, _
                   ByVal PhonerKlasse As PhonerInterface, _
                   ByVal KontaktFunktionen As Contacts)

        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        C_DP = XMLKlasse
        C_hf = HelferKlasse
        C_FBox = FritzBoxKlasse
        C_KF = KontaktFunktionen
        C_GUI = InterfacesKlasse
        bDirektwahl = Direktwahl

        C_Phoner = PhonerKlasse

        SID = C_DP.P_Def_SessionID
        Me.FrameDirektWahl.Visible = bDirektwahl
        Me.FrameDirektWahl.Location = New Drawing.Point(12, 3)
        Me.Focus()
        Me.KeyPreview = Not bDirektwahl
    End Sub

    Private Sub formWählbox_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If WählboxBereit Then
            If (e.KeyCode >= Keys.D1 And e.KeyCode <= Keys.D9) Or (e.KeyCode >= Keys.NumPad1 And e.KeyCode <= Keys.NumPad9) Then
                Dim gedrückteZahl As Integer = e.KeyCode - 48 * CInt(IIf(e.KeyCode > 96, 2, 1))
                If Not gedrückteZahl > Me.ListTel.RowCount Then
                    Me.ListTel.Rows.Item(gedrückteZahl - 1).Selected = True
                End If
            End If
        End If
    End Sub
    Private Sub formWählbox_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Startwerte eintragen

        Dim tmpStr As String
        Dim DialPort As String
        Dim SIP_Nr As Integer
        Dim xPathTeile As New ArrayList
        With xPathTeile
            .Add("Telefone")
            .Add("Telefone")
            .Add("*")
            .Add("Telefon")
            .Add("[@Dialport < 600 and not(@Dialport > 19 and @Dialport < 49) and not(@Fax = 1)]") ' Keine Anrufbeantworter, kein Fax
            .Add("TelName")

            Nebenstellen = Split(C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne & ";"), ";", , CompareMethod.Text)

            For Each Nebenstelle In Nebenstellen
                .Item(.Count - 2) = "[TelName = """ & Nebenstelle & """]"
                .Item(.Count - 1) = "@Dialport"
                DialPort = C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne)
                tmpStr = Nebenstelle & CStr(IIf(C_DP.P_CBDialPort, " (" & DialPort & ")", C_DP.P_Def_StringEmpty))
                Me.ComboBoxFon.Items.Add(tmpStr)
                .Item(.Count - 1) = "@Standard"
                If CBool(C_DP.Read(xPathTeile, "False")) Then C_DP.P_TelAnschluss = Me.ComboBoxFon.Items.Count - 1
            Next
        End With
        xPathTeile = Nothing
        Me.ComboBoxFon.SelectedIndex = C_DP.P_TelAnschluss

        If Not BWLogin.IsBusy Then BWLogin.RunWorkerAsync()

        ' Phoner
        If C_DP.P_CBPhoner Then
            If C_Phoner.PhonerReady() Then
                SIP_Nr = C_DP.P_PhonerTelNameIndex
                Me.ComboBoxFon.Items.Add("Phoner")
                PhonerFon = Me.ComboBoxFon.Items.Count - 1
            End If
        End If
        ' End Phoner 
        Me.ListTel.Enabled = True
        Me.ComboBoxFon.Enabled = True
        WählboxBereit = True

        Me.checkCBC.Enabled = Not C_DP.P_CBCbCunterbinden
        Me.checkNetz.Checked = C_DP.P_TelFestnetz
        Me.checkCLIR.Checked = C_DP.P_TelCLIR
        Me.checkCBC.Checked = C_DP.P_CBCallByCall
        If checkCBC.Checked Then
            Me.Height = 515
        Else
            Me.Height = 283    ' Zuerst schalten wir auf klein, damit die CallbyCall-
        End If

        ListTel.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        ListTel.ClearSelection()
        If bDirektwahl Then
            Me.TelNrBox.Focus()
        Else
            Me.Focus()
        End If
        ' der AddHandler darf erst jetzt rein (kein Handles ListTel.SelectionChanged!!) weil wir
        ' sonst beim Laden der Form dieses Event schon auslösen würden!
        AddHandler ListTel.SelectionChanged, AddressOf ListTel_SelectionChanged
        AddHandler ComboBoxFon.SelectedIndexChanged, AddressOf ComboBoxFon_SelectedIndexChanged
        AddHandler BVIP.CheckedChanged, AddressOf BVIP_CheckedChanged
    End Sub

#Region "Button"
    Private Sub cancelCallButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cancelCallButton.Click
        ' Bricht den Aufbau der Telefonverbindung ab

        ' Abbruch-Button wieder verstecken
        cancelCallButton.Visible = False
        ' Abbruch ausführen
        If P_Dialing And Not PhonerCall Then
            Me.LabelStatus.Text = C_FBox.SendDialRequestToBox(C_DP.P_Def_StringEmpty, Nebenstellen(Me.ComboBoxFon.SelectedIndex), True)
        End If
        P_Dialing = False
        TimerSchließen.Stop()
        ListTel.ClearSelection() ' Ein erneutes Wählen ermöglichen
    End Sub

    Private Sub LLBiligertelefonieren_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LLBiligertelefonieren.LinkClicked
        System.Diagnostics.Process.Start(Me.LLBiligertelefonieren.Text)
    End Sub

    Private Sub ButtonZeigeKontakt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonZeigeKontakt.Click
        ' blendet den Kontakteintrag des Anrufers ein
        ' ist kein Kontakt vorhanden, dann wird einer angelegt und mit den vCard-Daten ausgefüllt

        Dim KontaktDaten() As String = Split(CStr(Me.Tag) & ";" & ListTel.Rows(0).Cells(1).Value.ToString, ";", , CompareMethod.Text)
        If KontaktDaten(0) = C_DP.P_Def_ErrorMinusOne Then KontaktDaten(1) = C_DP.P_Def_ErrorMinusOne

        C_KF.ZeigeKontakt(KontaktDaten(0), KontaktDaten(1), KontaktDaten(2), C_DP.P_Def_StringEmpty)
        Me.CloseButton.Focus()
    End Sub

    Private Sub ButtonWeiter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonWeiter.Click
        Dim row(2) As String
        row(0) = "1" 'Index Zeile im DataGrid
        row(2) = C_hf.nurZiffern(Me.TelNrBox.Text, C_DP.P_TBLandesVW)
        With Me
            .Text = "Anruf: " & row(2)
            .Tag = C_DP.P_Def_ErrorMinusOne
            With .ListTel.Rows
                .Add(row)
                .Item(.Count - 1).Selected = True
            End With
        End With
    End Sub

    Private Sub TelNrBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TelNrBox.TextChanged
        Dim TelNr As String = C_hf.nurZiffern(Me.TelNrBox.Text, C_DP.P_TBLandesVW)
        Me.ButtonWeiter.Enabled = Len(TelNr) > 0
        Me.LabelCheckTest.Text = "Diese Telefonnumer wird gewählt: " & TelNr
    End Sub

    Private Sub CloseButton_Click() Handles CloseButton.Click
        Me.Hide()
        If Not TimerSchließen Is Nothing Then TimerSchließen = C_hf.KillTimer(TimerSchließen)
        If Not UsePhonerOhneFritzBox Then ThisAddIn.P_FritzBox.FBLogOut(SID)
        Me.Close()
        Me.Dispose(True)
    End Sub
#End Region

#Region "Helfer"
    Function HTMLTagsEntfernen(ByVal code As String) As String
        Dim tempArray() As String
        Dim i As Integer
        Dim pos As Integer
        code = Replace(code, Chr(13), "", , , CompareMethod.Text)
        code = Replace(code, Chr(10), "", , , CompareMethod.Text)
        tempArray = Split(code, ">", , CompareMethod.Text)
        For i = LBound(tempArray) To UBound(tempArray)
            pos = InStr(tempArray(i), "<", CompareMethod.Text)
            If pos = 0 Then
                tempArray(i) = Trim(tempArray(i))
            Else
                tempArray(i) = Trim(Strings.Left(tempArray(i), InStr(tempArray(i), "<", CompareMethod.Text) - 1))
            End If
            If Not tempArray(i) = C_DP.P_Def_StringEmpty Then tempArray(i) = tempArray(i) & " "
        Next
        Return Replace(Trim(Strings.Join(tempArray, "")), " ,", ",", , , CompareMethod.Text)
    End Function

    Sub AutoClose()
        If Me.InvokeRequired Then
            Dim D As New SchließeForm(AddressOf AutoClose)
            Me.Invoke(D)
        Else
            Me.Close()
        End If
    End Sub

    Function GetDialport(ByVal Nebenstelle As String) As String
        GetDialport = C_DP.P_Def_ErrorMinusOne
        Dim tmpint As Double
        Dim xPathTeile As New ArrayList
        With xPathTeile
            .Add("Telefone")
            .Add("Telefone")
            .Add("*")
            .Add("Telefon")
            .Add("[not(@Dialport > 599) and TelName = """ & Nebenstelle & """]")
            .Add("@Dialport")
            tmpint = CDbl(C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne))
        End With

        If Not tmpint = -1 Then
            Select Case tmpint
                Case 1 To 4
                    tmpint -= 1
            End Select
        End If
        Return CStr(tmpint)
    End Function
#End Region

#Region "Timer"
    Private Sub TimerSchließen_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerSchließen.Elapsed
        TimerSchließen = C_hf.KillTimer(TimerSchließen)
        AutoClose()
    End Sub
#End Region

#Region "Datagrid"
    Private Sub ListTel_SelectionChanged(ByVal sender As System.Object, ByVal e As EventArgs)
        If WählboxBereit Then
            Dim code As String
            Dim cbcHTML As String
            Dim myurl As String
            Dim CheckMobil As Boolean = True
            Dim HTMLFehler As ErrObject = Nothing

            If Not Me.checkCBC.Checked Then
                Me.cancelCallButton.Visible = True
                Me.cancelCallButton.Focus()
                Me.ComboBoxFon.Enabled = False
                Me.ListTel.Enabled = False
                ' Prüfung ob es sich bei der gewählten nummer um eine Mobilnummer handelt.
                If C_DP.P_CBCheckMobil Then
                    If Not ListTel.SelectedRows.Count = 0 Then
                        If C_hf.Mobilnummer(CStr(ListTel.SelectedRows.Item(0).Cells(2).Value.ToString)) Then
                            CheckMobil = CBool(IIf(C_hf.FBDB_MsgBox("Sie sind dabei eine Mobilnummer anzurufen. Fortsetzen?", MsgBoxStyle.YesNo, "formWählbox.Start") = vbYes, True, False))
                        End If
                    End If
                End If
                If CheckMobil Then
                    Me.LabelStatus.Text = "Bitte warten" & vbNewLine & "Ihr Anruf wird vorbereitet"
                    StarteDialVorgang()
                End If
            Else
                code = C_hf.nurZiffern(CStr(ListTel.SelectedRows.Item(0).Cells(2).Value.ToString), C_DP.P_TBLandesVW) 'Ergebnis sind nur Ziffern, die eigene Landesvorwahl wird durch "0" ersetzt
                Me.LabelStatus.Text = "Bitte warten..."
                ' Ermitteln der URL für ein Orts- oder  Ferngespräch
                If C_DP.P_TBVorwahl = Mid(code, 1, Len(C_DP.P_TBVorwahl)) And Not C_DP.P_TBVorwahl = C_DP.P_Def_StringEmpty Then
                    ' Wenn die Vorwahl nicht der eigenen Vorwahl entspricht, ändere die URL
                    myurl = "http://www.billiger-telefonieren.de/festnetz/schnellrechner/"
                    code = "rechnen=true&p_zielvorwahl=58&p_typ%5B%5D=1&p_takt=-1"
                Else
                    myurl = "http://www.billiger-telefonieren.de/vorwahlrechner/"
                    code = "num=" & code & "&berechnen=berechnen"
                End If

                cbcHTML = C_hf.httpPOST(myurl, code, System.Text.Encoding.Default)
                If Not HTMLFehler Is Nothing Then C_hf.LogFile("FBError (formWählbox.ListTel_SelectionChanged): " & Err.Number & " - " & Err.Description & " - " & myurl)
                Me.LLBiligertelefonieren.Text = myurl
                CbCBilligerTelefonieren(code, cbcHTML)
                Me.Height = 515
                myurl = Nothing
            End If
        End If
    End Sub

    Private Sub listCbCAnbieter_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        StarteDialVorgang()
    End Sub
#End Region

#Region "Wählen"

    Private Sub StarteDialVorgang()
        If Not ListTel.SelectedRows.Count = 0 Then
            Dim ID As Argument
            P_Dialing = True
            CallNr = New System.Threading.Thread(AddressOf dialNumber)
            With ID
                .TelNr = CStr(ListTel.SelectedRows.Item(0).Cells(2).Value.ToString)
                .clir = Me.checkCLIR.Checked
                .festnetz = Me.checkNetz.Checked
                If Me.ComboBoxFon.Text = "Phoner" Then
                    .fonanschluss = "-2"
                Else
                    .fonanschluss = GetDialport(Nebenstellen(Me.ComboBoxFon.SelectedIndex))
                End If
            End With

            CallNr.Start(ID)

            ' Einstellungen (Welcher Anschluss, CLIR, Festnetz...) speichern
            C_DP.P_TelAnschluss = ComboBoxFon.SelectedIndex
            C_DP.P_TelFestnetz = checkNetz.Checked
            C_DP.P_TelCLIR = checkCLIR.Checked
            ' Timer zum automatischen Schließen des Fensters starten
            If C_DP.P_CBAutoClose Then TimerSchließen = C_hf.SetTimer(C_DP.P_TBEnblDauer * 1000)
            cancelCallButton.Enabled = True
        End If
    End Sub

    Private Function dialNumber(ByVal AnrufEigenschaften As Object) As String
        ' bereitet die Telefonnummer zum Verbindungsaufbau vor
        ' Parameter:  Number (String):      zu wählende Nummer
        '             clir (Boolean):       Rufnummer unterdrücken?
        '             festnetz (Boolean):   Festnetz verwenden?
        '             fonanschluss (long):  Welcher Anschluss wird verwendet?
        ' Rückgabewert (String):            Antworttext (Status)
        Dim Übergabe As Argument
        Dim Number As String
        Dim CLIR As Boolean
        Dim Festnetz As Boolean
        Dim Telefonanschluss As String

        Übergabe = CType(AnrufEigenschaften, Argument)
        With Übergabe
            Number = .TelNr
            CLIR = .clir
            Festnetz = .festnetz
            Telefonanschluss = .fonanschluss
        End With


        Dim Code As String  ' zu wählende Nummer
        Dim nameStart As Integer ' Position des Namens im Fenstertitel
        Dim index As Integer ' Zählvariable
        Dim KontaktID As String
        Dim StoreID As String

        nameStart = InStr(Me.Text, "ruf: ") + 5
        If Not nameStart = 5 And Not Number = "ATH" And ThisAddIn.P_AnrMon.AnrMonAktiv Then
            ' Symbolleisteneintrag für Wahlwiederholung vornehmen
            ' nur wenn Timer aus ist sonst macht das 'AnrMonCALL'
            index = CInt(C_DP.Read("Wwdh", "Index", "0"))
            If Not CStr(Me.Tag) = C_DP.P_Def_ErrorMinusOne Then
                KontaktID = Mid(CStr(Me.Tag), 1, InStr(1, CStr(Me.Tag), ";", CompareMethod.Text) - 1)
                StoreID = Mid(CStr(Me.Tag), InStr(1, CStr(Me.Tag), ";", CompareMethod.Text) + 1)
            Else
                KontaktID = C_DP.P_Def_ErrorMinusOne
                StoreID = C_DP.P_Def_ErrorMinusOne
            End If

            If Not C_hf.nurZiffern(C_DP.Read("Wwdh", "TelNr" & Trim(Str((index + 9) Mod 10)), ""), C_DP.P_TBLandesVW) = C_hf.nurZiffern(Number, C_DP.P_TBLandesVW) Then
                Dim xPathTeile() As String = {Mid(Me.Text, nameStart), Number, CStr(System.DateTime.Now), CStr((index + 1) Mod 10), StoreID, KontaktID}
                C_DP.Write("Wwdh", "WwdhEintrag" & index, Join(xPathTeile, ";"))
                C_DP.Write("Wwdh", "Index", CStr((index + 1) Mod 10))
#If OVer < 14 Then
                If C_DP.Read("Optionen", "CBSymbWwdh", "False") = "True" Then C_GUI.FillPopupItems("Wwdh")
#End If
            End If
        End If

        Code = C_hf.nurZiffern(Number, C_DP.P_TBLandesVW) 'Ergebnis sind nur Ziffern, die eigene Landesvorwahl wird durch "0" ersetzt
        'LogFile("Rufnummer " & Code & " wurde ausgewählt")
        If C_DP.P_CBVoIPBuster Then
            ' Änderung von "HardyX9" zur Nutzung des Scriptes mit VoIPBuster
            ' Dadurch wird die Länderkennung 0049 immer mitgewählt
            If Not Mid(Code, 1, 2) = "00" Then Code = Replace(Code, "0", C_DP.P_TBLandesVW, 1, 1)
            C_hf.LogFile("VoIPBuster umgewandelte Rufnummer lautet: " & Code)
        End If
        If Me.checkCBC.Checked Then Code = CStr(listCbCAnbieter.SelectedRows.Item(0).Cells(2).Value.ToString) & Code
        ' Amtsholungsziffer voranstellen
        Code = CStr(IIf(C_DP.P_TBAmt = C_DP.P_Def_ErrorMinusOne, "", C_DP.P_TBAmt)) & Code

        If Not UsePhonerOhneFritzBox Then
            If CLIR Then Code = "*31#" & Code
            If Festnetz Then Code = "*11#" & Code
            ' Sagt der FB dass die Nummer jetzt zuende ist
            Code = Code & "#"
        End If
        ' Jetzt Code an Box bzw. Phoner senden
        If (CDbl(Telefonanschluss) >= 20 And CDbl(Telefonanschluss) <= 29) Or CDbl(Telefonanschluss) = -2 Then
            C_hf.LogFile("Folgende Nummer wird zum Wählen an Phoner gesendet: " & Code)
            StatusText = C_Phoner.DialPhoner(Code)
        Else
            C_hf.LogFile("Folgende Nummer wird zum Wählen an die Box gesendet: " & Code & " über Anschluss: " & Telefonanschluss)
            StatusText = C_FBox.SendDialRequestToBox(Code, Telefonanschluss, False)
        End If

        dialNumber = StatusText
        SetStatusText()
    End Function

    Private Sub SetStatusText()
        If Me.InvokeRequired Then
            Dim D As New DlgStatusText(AddressOf SetStatusText)
            Invoke(D)
        Else
            Me.LabelStatus.Text = StatusText
        End If
    End Sub

    Private Sub SetEnabled()
        If Element.InvokeRequired Then
            Dim D As New DlgAnAus(AddressOf SetEnabled)
            Invoke(D)
        Else
            Element.Enabled = AnAus
        End If
    End Sub

#End Region

#Region "CbC"
    Public Sub CbCBilligerTelefonieren(ByVal TelNr As String, ByVal cbcHTML As String)
        ' sucht auf 'billiger-telefonieren.de' nach Call-by-Call-Vorwahlen
        ' Parameter  TelNr (String):  Telefonnummer des Anzurufenden
        Dim SuchString(3) As String

        Dim pos As Integer, pos1 As Integer, pos2 As Integer
        Dim j As Integer = 0

        cbcHTML = Replace(cbcHTML, Chr(34), "'", , , CompareMethod.Text) 'die "-Zeichen entfernen zum besseren Durchsuchen.
        SuchString(0) = "Kosten für ein Telefonat mit dem Ziel "
        SuchString(1) = "<td>"
        SuchString(2) = "</td>"
        With Me.listCbCAnbieter
            Do While Not .RowCount = 1
                .Rows.Remove(.Rows(0))
                Windows.Forms.Application.DoEvents()
            Loop
        End With
        pos = InStr(1, cbcHTML, SuchString(0), CompareMethod.Text)
        If Not pos = 0 Then ' wenn wir hier einen Match gefunden haben...
            C_hf.LogFile("Call-by-Call Vorwahlen erhalten")
            pos1 = pos + Len(SuchString(0))
            pos2 = InStr(pos1, cbcHTML, "<", CompareMethod.Text)
            Me.Gespraechsart.Text = "Zone: " & Replace(Mid(cbcHTML, pos1, pos2 - pos1), "*", "", , , CompareMethod.Text)

            Dim Tabelle As String
            pos1 = InStr(pos1, cbcHTML, "<table class='hover_table'>", CompareMethod.Text)
            pos2 = InStr(pos1, cbcHTML, "</table>", CompareMethod.Text) + Len("</table>")
            Tabelle = Mid(cbcHTML, pos1, pos2 - pos1)
            With Me.listCbCAnbieter
                Dim Tarife As String() = Split(Tabelle, "</tr>", , CompareMethod.Text)
                Dim Daten As String()
                Dim row(.ColumnCount - 1) As String
                For Each CbCVorwahl In Tarife
                    Daten = Split(CbCVorwahl, "</td>", , CompareMethod.Text)
                    If Not Daten.Length = 1 Then
                        row(0) = HTMLTagsEntfernen(Daten(0)) ' Nummer
                        row(1) = Replace(HTMLTagsEntfernen(Daten(1)), "&euro;", ChrW(&H20AC), , , CompareMethod.Text) ' Ct/min
                        row(2) = HTMLTagsEntfernen(Daten(2)) ' Zugang
                        row(3) = HTMLTagsEntfernen(Daten(3)) ' Takt
                        row(4) = Replace(HTMLTagsEntfernen(Daten(4)), "Call-by-Call", C_DP.P_Def_StringEmpty, , , CompareMethod.Text) ' Tarif
                        row(5) = HTMLTagsEntfernen(Daten(5)) ' Bemerkung
                        .Rows.Add(row)
                    End If
                Next
                .Item(1, .Rows.Count - 1).Value = "EOL"
            End With
        Else
            With Me.listCbCAnbieter
                .Item(1, .Rows.Count - 1).Value = "EOL"
                .Item(5, .Rows.Count - 1).Value = "Keine Vorwahl gefunden."
            End With
            C_hf.LogFile("Keine Call-by-Call Vorwahlen erhalten für " & TelNr)
        End If
        listCbCAnbieter.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        listCbCAnbieter.ClearSelection()
        AddHandler listCbCAnbieter.SelectionChanged, AddressOf listCbCAnbieter_SelectionChanged
    End Sub '(CbCBilligerTelefonieren)

    Private Sub checkCBC_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles checkCBC.CheckedChanged
        If checkCBC.Checked Then
            If Not Me.listCbCAnbieter.RowCount = 1 Then Me.Height = 515
        Else
            Me.Height = 283    ' Zuerst schalten wir auf klein, damit die CallbyCall-
        End If
    End Sub
#End Region

#Region "Login"
    Private Sub BWLogin_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWLogin.DoWork
        Element = Me.ComboBoxFon
        AnAus = False
        SetEnabled()
        SID = C_FBox.FBLogIn(True) ' Falls Login fehlgeschlagen ist, wird "-1" zurückgegeben oder die DefaultSID
        If Not SID = C_DP.P_Def_SessionID Then
            StatusText = "Der Wählclient ist bereit."
            WählboxBereit = True
            Element = Me.ListTel
            AnAus = True
            SetEnabled()
        Else
            StatusText = "Login fehlgeschlagen"
            C_hf.LogFile("BWLogin: Login fehlgeschlagen")
            Element = Me.ListTel
            Enabled = False
            SetEnabled()
        End If
        SetStatusText()
    End Sub

    Private Sub BWLogin_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BWLogin.RunWorkerCompleted
        Element = Me.ComboBoxFon
        AnAus = True
        SetEnabled()
    End Sub
#End Region

#Region "Änderungen"
    Private Sub ComboBoxFon_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBoxFon.SelectedIndexChanged
        If Me.ComboBoxFon.SelectedIndex = PhonerFon Then
            Me.checkCLIR.Enabled = False
            Me.checkNetz.Enabled = False
        Else
            Me.checkCLIR.Enabled = True
            Me.checkNetz.Enabled = True
            If SID = C_DP.P_Def_ErrorMinusOne Or SID = C_DP.P_Def_SessionID Then
                If Not BWLogin.IsBusy Then BWLogin.RunWorkerAsync()
                WählboxBereit = False
                Me.LabelStatus.Text = "Bitte warten..."
                Me.ListTel.Enabled = False
            End If
        End If
    End Sub
#End Region

    Private Sub BVIP_CheckedChanged(sender As Object, e As EventArgs)
        Dim KontaktDaten() As String = Split(CStr(Me.Tag) & ";" & ListTel.Rows(0).Cells(1).Value.ToString, ";", , CompareMethod.Text)
        If Not KontaktDaten(0) = C_DP.P_Def_ErrorMinusOne Then
            If Not BVIP.Checked Then
                C_GUI.RemoveVIP(KontaktDaten(0), KontaktDaten(1))
            Else
                C_GUI.AddVIP(KontaktDaten(0), KontaktDaten(1))
            End If
        End If
    End Sub
End Class