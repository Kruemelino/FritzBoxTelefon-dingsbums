Imports System.Windows.Forms
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Collections

Public Class formWählbox
    Private WithEvents BWLogin As New System.ComponentModel.BackgroundWorker

    Private C_XML As MyXML
    Private hf As Helfer
    Private FBox As FritzBox

    Private Client As New Sockets.TcpClient()
    Private GUI As GraphicalUserInterface
    Private WithEvents TimerSchließen As System.Timers.Timer
    Private CallNr As System.Threading.Thread

    Delegate Sub SchließeForm()
    Delegate Sub DlgStatusText()
    Delegate Sub DlgAnAus()

    'Private DateiPfad As String
    Private StatusText As String ' Wird für Delegaten DlgStatusText benötigt
    Private AnAus As Boolean ' Wird für Delegaten DlgAnAus benötigt
    Private Element As Control ' Wird für Delegaten DlgAnAus benötigt
    Private WählboxBereit As Boolean = False ' Erst wenn True, kann gewählt werden
    Private SID As String
    Private bDirektwahl As Boolean
    Private LandesVorwahl As String
    Private Nebenstellen As String()
    ' Phoner
    Private C_Phoner As PhonerInterface
    Private PhonerCall As Boolean = False
    Private UsePhonerOhneFritzBox As Boolean = False
    Private PhonerFon As Integer = -1

    Structure Argument
        Dim TelNr As String
        Dim clir As Boolean
        Dim festnetz As Boolean
        Dim fonanschluss As String
    End Structure

    Public Sub New(ByVal Direktwahl As Boolean, _
                   ByVal XMLKlasse As MyXML, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal InterfacesKlasse As GraphicalUserInterface, _
                   ByVal FritzBoxKlasse As FritzBox, _
                   ByVal PhonerKlasse As PhonerInterface)

        ' Dieser Aufruf ist für den Windows Form-Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie C_XMLtialisierungen nach dem C_XMLtializeComponent()-Aufruf hinzu.
        C_XML = XMLKlasse
        hf = HelferKlasse
        FBox = FritzBoxKlasse

        GUI = InterfacesKlasse
        bDirektwahl = Direktwahl

        C_Phoner = PhonerKlasse

        SID = FBox.sDefaultSID
        Me.FrameDirektWahl.Visible = bDirektwahl
        Me.FrameDirektWahl.Location = New Drawing.Point(12, 3)
        Me.Focus()
        Me.KeyPreview = Not bDirektwahl
    End Sub

    Private Sub formWählbox_FormClosing(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.FormClosing
        If Not UsePhonerOhneFritzBox Then ThisAddIn.fBox.FBLogout(SID)
        Me.Dispose(True)
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
    Private Sub SortiereNebenstellen()
        Nebenstellen = Split(C_XML.Read("Telefone", "EingerichteteTelefone", "1;2;3;51;52;53;54;55;56;57;58;50;60;61;62;63;64;65;66;67;68;69"), ";", , CompareMethod.Text)
        Dim FaxListe() As String = Split(C_XML.Read("Telefone", "EingerichteteFax", "-1"), ";", , CompareMethod.Text)

        Nebenstellen = (From x In Nebenstellen Where Not x Like "2#" Select x).ToArray  ' Ip-Telefone entfernen
        Nebenstellen = (From x In Nebenstellen Where Not x Like "60#" Select x).ToArray ' TAM entfernen
        Dim Fax As String
        For Each Fax In FaxListe ' Faxgeräte entfernen
            Nebenstellen = (From x In Nebenstellen Where Not x Like Fax Select x).ToArray
        Next
    End Sub

    Private Sub formWählbox_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Startwerte eintragen

        Dim selIndex As Integer = CInt(C_XML.Read("Optionen", "Anschluss", "0"))
        Dim StandardTelefon As Integer = CInt(C_XML.Read("Telefone", "CBStandardTelefon", "-1"))
        Dim tmpStr As String
        Dim ZeigeDialPort As Boolean = CBool(C_XML.Read("Optionen", "CBDialPort", "False"))
        'Dim TelName() As String

        LandesVorwahl = C_XML.Read("Optionen", "TBLandesVW", "0049")
        SortiereNebenstellen()

        For Each Nebenstelle In Nebenstellen
            tmpStr = Split(C_XML.Read("Telefone", Nebenstelle, "-1;"), ";", , CompareMethod.Text)(2) & CStr(IIf(ZeigeDialPort, " (" & Nebenstelle & ")", vbNullString))
            With Me.ComboBoxFon
                .Items.Add(tmpStr)

                If CInt(Nebenstelle) = StandardTelefon Then StandardTelefon = .Items.Count - 1
            End With
        Next


        If Not selIndex = StandardTelefon And Not StandardTelefon = -1 Then selIndex = StandardTelefon
        BWLogin.RunWorkerAsync()
        'Falls Telefone geändert haben
        If selIndex >= Me.ComboBoxFon.Items.Count Then
            selIndex = Me.ComboBoxFon.Items.Count - 1
            Me.ComboBoxFon.ForeColor = Drawing.Color.Red
            Me.ListTel.Enabled = True
            Me.ComboBoxFon.Enabled = True
            WählboxBereit = True
        End If

        ' Phoner
        If C_XML.Read("Phoner", "CBPhoner", "False") = "True" Then
            If C_Phoner.PhonerReady() Then
                Dim SIP_Nr As Integer = CInt(C_XML.Read("Phoner", "PhonerTelNameIndex", "0"))
                Me.ComboBoxFon.Items.Add("Phoner")
                PhonerFon = Me.ComboBoxFon.Items.Count - 1
                If SIP_Nr = StandardTelefon Then
                    StandardTelefon = PhonerFon
                End If
            End If
        End If
        ' End Phoner 

        Me.ComboBoxFon.SelectedIndex = selIndex
        Me.checkCBC.Enabled = CBool(IIf(C_XML.Read("Optionen", "CBCbCunterbinden", "False") = "False", True, False))
        Me.checkNetz.Checked = CBool(IIf(C_XML.Read("Optionen", "Festnetz", "False") = "True", True, False))
        Me.checkCLIR.Checked = CBool(IIf(C_XML.Read("Optionen", "CLIR", "False") = "True", True, False))
        Me.checkCBC.Checked = CBool(IIf(C_XML.Read("Optionen", "CBCallByCall", "False") = "True", True, False))
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
    End Sub

#Region "Button"
    Private Sub cancelCallButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cancelCallButton.Click
        ' Bricht den Aufbau der Telefonverbindung ab

        ' Abbruch-Button wieder verstecken
        cancelCallButton.Visible = False
        ' Abbruch ausführen
        If Not PhonerCall Then Me.LabelStatus.Text = FBox.SendDialRequestToBox("ATH", Nebenstellen(Me.ComboBoxFon.SelectedIndex), True)
        ' Bemerkung: Anstatt ATH kann auch einfach ein Leerzeichen oder ein Buchstabe, oder #
        ' gesendet werden (nur keine Nummer), was alles zu einem Verbindungsabbruch führt.
        ' ATH entspricht lediglich dem AT-Kommando das früher über Port1011 des telefond für
        ' das Auflegen benutzt wurde, daher hab ich es hier verwendet, auch wenn es gar nicht
        ' ausgewertet wird.
        ' Kruemelino 130812: ATH wird nicht mehr verwendet.
        TimerSchließen.Stop()
        ListTel.ClearSelection() ' Ein erneutes Wählen ermöglichen
    End Sub

    Private Sub LLBiligertelefonieren_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LLBiligertelefonieren.LinkClicked
        System.Diagnostics.Process.Start(Me.LLBiligertelefonieren.Text)
    End Sub

    Private Sub ButtonZeigeKontakt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonZeigeKontakt.Click
        ' blendet den Kontakteintrag des Anrufers ein
        ' ist kein Kontakt vorhanden, dann wird einer angelegt und mit den vCard-Daten ausgefüllt

        Dim TelNr As String      ' Telefonnummer
        Dim KontaktID As String = "-1"
        Dim StoreID As String = "-1"
        Dim KontaktDaten() As String = Split(CStr(Me.Tag) & ";" & ListTel.Rows(0).Cells(1).Value.ToString, ";", , CompareMethod.Text)
        TelNr = ListTel.Rows(0).Cells(2).Value.ToString '''''''''''' überprüfen, welcher Wert hier übernommen wird zur TelNr.-Erkennung!!

        If KontaktDaten(0) = "-1" Then KontaktDaten(1) = "-1"

        ThisAddIn.WClient.ZeigeKontakt(KontaktDaten)
        Me.CloseButton.Focus()
    End Sub

    Private Sub ButtonWeiter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonWeiter.Click
        Dim row(2) As String
        row(0) = "1" 'Index Zeile im DataGrid
        row(2) = hf.nurZiffern(Me.TelNrBox.Text, LandesVorwahl)
        With Me
            .Text = "Anruf: " & row(2)
            .Tag = "-1"
            With .ListTel.Rows
                .Add(row)
                .Item(.Count - 1).Selected = True
            End With
        End With
    End Sub

    Private Sub TelNrBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TelNrBox.TextChanged
        Dim TelNr As String = hf.nurZiffern(Me.TelNrBox.Text, LandesVorwahl)
        Me.ButtonWeiter.Enabled = Len(TelNr) > 0
        Me.LabelCheckTest.Text = "Diese Telefonnumer wird gewählt: " & TelNr
    End Sub

    Sub Form_Close() Handles CloseButton.Click
        If Not TimerSchließen Is Nothing Then hf.KillTimer(TimerSchließen)
        Me.Close()
        Me.Dispose()
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
            If Not tempArray(i) = vbNullString Then tempArray(i) = tempArray(i) & " "
        Next
        Return Replace(Trim(Strings.Join(tempArray, "")), " ,", ",", , , CompareMethod.Text)
    End Function

    Sub AutoSchließen()
        If Me.InvokeRequired Then
            Dim D As New SchließeForm(AddressOf AutoSchließen)
            Me.Invoke(D)
        Else
            Me.Close()
        End If
    End Sub
#End Region

#Region "Timer"
    Private Sub TimerSchließen_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerSchließen.Elapsed
        hf.KillTimer(TimerSchließen)
        AutoSchließen()
    End Sub
#End Region

#Region "Datagrid"
    Private Sub ListTel_SelectionChanged(ByVal sender As System.Object, ByVal e As EventArgs)
        If WählboxBereit Then
            Dim code As String
            Dim LandesVW As String
            Dim cbcHTML As String
            Dim myurl As String
            Dim CheckMobil As Boolean = True
            Dim HTMLFehler As ErrObject = Nothing
            ' C_XMLtiiert den Anruf, wenn eine Nummer ausgewählt wurde
            If Not Me.checkCBC.Checked Then
                Me.cancelCallButton.Visible = True
                Me.cancelCallButton.Focus()
                Me.ComboBoxFon.Enabled = False
                Me.ListTel.Enabled = False
                ' Prüfung ob es sich bei der gewählten nummer um eine Mobilnummer handelt.
                If CBool(C_XML.Read("Optionen", "CBCheckMobil", "True")) Then
                    If Not ListTel.SelectedRows.Count = 0 Then
                        If hf.Mobilnummer(CStr(ListTel.SelectedRows.Item(0).Cells(2).Value.ToString)) Then
                            CheckMobil = CBool(IIf(hf.FBDB_MsgBox("Sie sind dabei eine Mobilnummer anzurufen. Fortsetzen?", MsgBoxStyle.YesNo, "formWählbox.Start") = vbYes, True, False))
                        End If
                    End If
                End If
                If CheckMobil Then
                    Me.LabelStatus.Text = "Bitte warten" & vbNewLine & "Ihr Anruf wird vorbereitet"
                    Start()
                End If
            Else
                LandesVW = C_XML.Read("Optionen", "TBLandesVW", "0049")
                code = hf.nurZiffern(CStr(ListTel.SelectedRows.Item(0).Cells(2).Value.ToString), LandesVW) 'Ergebnis sind nur Ziffern, die eigene Landesvorwahl wird durch "0" ersetzt
                Me.LabelStatus.Text = "Bitte warten..."
                ' Ermitteln der URL für ein Orts- oder  Ferngespräch
                Dim Vorwahl As String = C_XML.Read("Optionen", "TBVorwahl", "") 'Vorwahl ermitteln
                If Vorwahl = Mid(code, 1, Len(Vorwahl)) And Not Vorwahl = "" Then
                    ' Wenn die Vorwahl nicht der eigenen Vorwahl entspricht, ändere die URL
                    myurl = "http://www.billiger-telefonieren.de/festnetz/schnellrechner/"
                    cbcHTML = hf.httpWrite(myurl, "rechnen=true&p_zielvorwahl=58&p_typ%5B%5D=1&p_takt=-1", System.Text.Encoding.Default)
                Else
                    myurl = String.Concat("http://www.billiger-telefonieren.de/tarife/nummer.php3?num=", code)
                    cbcHTML = hf.httpRead(myurl, System.Text.Encoding.Default, HTMLFehler)
                    If Not HTMLFehler Is Nothing Then
                        hf.LogFile("FBError (formWählbox.ListTel_SelectionChanged): " & Err.Number & " - " & Err.Description & " - " & myurl)
                    End If
                End If : Vorwahl = Nothing
                Me.LLBiligertelefonieren.Text = myurl
                CbCBilligerTelefonieren(code, cbcHTML)
                Me.Height = 515
                myurl = Nothing
            End If
        End If
    End Sub

    Private Sub listCbCAnbieter_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Start()
    End Sub
#End Region

#Region "Wählen"
    Private Sub Start()
        If Not ListTel.SelectedRows.Count = 0 Then
            Dim ID As Argument
            Dim StandardTelefon As Integer = CInt(C_XML.Read("Telefone", "CBStandardTelefon", "-1"))

            CallNr = New System.Threading.Thread(AddressOf dialNumber)
            With ID
                .TelNr = CStr(ListTel.SelectedRows.Item(0).Cells(2).Value.ToString)
                .clir = Me.checkCLIR.Checked
                .festnetz = Me.checkNetz.Checked
                If Me.ComboBoxFon.Text = "Phoner" Then
                    .fonanschluss = "-2"
                Else
                    .fonanschluss = Nebenstellen(Me.ComboBoxFon.SelectedIndex)
                End If
            End With

            CallNr.Start(ID)

            ' Einstellungen (Welcher Anschluss, CLIR, Festnetz...) speichern

            If StandardTelefon = -1 Then
                C_XML.Write("Optionen", "Anschluss", CStr(ComboBoxFon.SelectedIndex), False)
            Else
                C_XML.Write("Optionen", "Anschluss", CStr(StandardTelefon), False)
            End If
            C_XML.Write("Optionen", "Festnetz", CStr(checkNetz.Checked), False)
            C_XML.Write("Optionen", "CLIR", CStr(checkCLIR.Checked), True)
            ' Timer zum automatischen Schließen des Fensters starten
            If CBool(C_XML.Read("Optionen", "CBAutoClose", CStr(True))) Then
                TimerSchließen = hf.SetTimer(CDbl(C_XML.Read("Optionen", "TBEnblDauer", CStr(10))) * 1000)
            End If
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
        Dim Amt As String  ' Amtsvorwahl
        Dim LandesVW As String = C_XML.Read("Optionen", "TBLandesVW", "0049") ' eigene Landesvorwahl
        Dim nameStart As Integer ' Position des Namens im Fenstertitel
        Dim index As Integer ' Zählvariable
        Dim KontaktID As String
        Dim StoreID As String

        nameStart = InStr(Me.Text, "ruf: ") + 5
        If Not nameStart = 5 And Not Number = "ATH" And ThisAddIn.AnrMon.AnrMonAktiv Then
            ' Symbolleisteneintrag für Wahlwiederholung vornehmen
            ' nur wenn Timer aus ist sonst macht das 'AnrMonCALL'
            index = CInt(C_XML.Read("Wwdh", "Index", "0"))
            If Not CStr(Me.Tag) = "-1" Then
                KontaktID = Mid(CStr(Me.Tag), 1, InStr(1, CStr(Me.Tag), ";", CompareMethod.Text) - 1)
                StoreID = Mid(CStr(Me.Tag), InStr(1, CStr(Me.Tag), ";", CompareMethod.Text) + 1)
            Else
                KontaktID = "-1"
                StoreID = "-1"
            End If

            If Not hf.nurZiffern(C_XML.Read("Wwdh", "TelNr" & Trim(Str((index + 9) Mod 10)), ""), LandesVW) = hf.nurZiffern(Number, LandesVW) Then
                Dim StrArr() As String = {Mid(Me.Text, nameStart), Number, CStr(System.DateTime.Now), CStr((index + 1) Mod 10), StoreID, KontaktID}
                C_XML.Write("Wwdh", "WwdhEintrag" & index, Join(StrArr, ";"), False)
                C_XML.Write("Wwdh", "Index", CStr((index + 1) Mod 10), True)
#If OVer < 14 Then
                If C_XML.Read( "Optionen", "CBSymbWwdh", "False") = "True" Then GUI.FillPopupItems("Wwdh")
#End If
            End If
        End If

        Amt = C_XML.Read("Optionen", "TBAmt", "")
        Amt = CStr(IIf(Amt = "-1", "", Amt))
        LandesVW = C_XML.Read("Optionen", "TBLandesVW", "0049")
        Code = hf.nurZiffern(Number, LandesVW) 'Ergebnis sind nur Ziffern, die eigene Landesvorwahl wird durch "0" ersetzt
        'LogFile("Rufnummer " & Code & " wurde ausgewählt")
        If C_XML.Read("Optionen", "CBVoIPBuster", "False") = "True" Then
            ' Änderung von "HardyX9" zur Nutzung des Scriptes mit VoIPBuster
            ' Dadurch wird die Länderkennung 0049 immer mitgewählt
            If Not Mid(Code, 1, 2) = "00" Then Code = Replace(Code, "0", LandesVW, 1, 1)
            hf.LogFile("VoIPBuster umgewandelte Rufnummer lautet: " & Code)
        End If
        If Me.checkCBC.Checked Then Code = CStr(listCbCAnbieter.SelectedRows.Item(0).Cells(2).Value.ToString) & Code
        ' Amtsholungsziffer voranstellen
        Code = Amt & Code

        If Not UsePhonerOhneFritzBox Then
            If CLIR Then Code = "*31#" & Code
            If Festnetz Then Code = "*11#" & Code
            ' Sagt der FB dass die Nummer jetzt zuende ist
            Code = Code & "#"
        End If
        ' Jetzt Code an Box bzw. Phoner senden
        If (CDbl(Telefonanschluss) >= 20 And CDbl(Telefonanschluss) <= 29) Or CDbl(Telefonanschluss) = -2 Then
            hf.LogFile("Folgende Nummer wird zum Wählen an Phoner gesendet: " & Code)
            StatusText = C_Phoner.DialPhoner(Code)
        Else
            hf.LogFile("Folgende Nummer wird zum Wählen an die Box gesendet: " & Code & " über Anschluss: " & Telefonanschluss)
            StatusText = FBox.SendDialRequestToBox(Code, Telefonanschluss, False)
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
            hf.LogFile("Call-by-Call Vorwahlen erhalten")
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
                        row(4) = Replace(HTMLTagsEntfernen(Daten(4)), "Call-by-Call", vbNullString, , , CompareMethod.Text) ' Tarif
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
            hf.LogFile("Eine Call-by-Call Vorwahlen erhalten für " & TelNr)
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
        SID = FBox.FBLogin(True) ' Falls Login fehlgeschlagen ist, wird "-1" zurückgegeben oder die DefaultSID
        If Not SID = FBox.sDefaultSID Then
            StatusText = "Der Wählclient ist bereit."
            WählboxBereit = True
            Element = Me.ListTel
            AnAus = True
            SetEnabled()
        Else
            StatusText = "Login fehlgeschlagen"
            hf.LogFile("BWLogin: Login fehlgeschlagen")
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
            If SID = "-1" Or SID = FBox.sDefaultSID Then
                If Not BWLogin.IsBusy Then BWLogin.RunWorkerAsync()
                WählboxBereit = False
                Me.LabelStatus.Text = "Bitte warten..."
                Me.ListTel.Enabled = False
            End If
        End If
    End Sub
#End Region

End Class