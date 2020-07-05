Imports System.ComponentModel
Imports System.Data
Imports System.Windows.Forms
Imports Microsoft.Office.Interop

Public Class FormCfg
    Implements IDisposable
    Private Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Private WithEvents FritzBoxDaten As FritzBoxData

    Private BWIndexerList As List(Of BackgroundWorker)

    'Private OutlookOrdnerListe As List(Of OutlookOrdner)

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        FBDB_MP.TabPages.Remove(PAnrMonSim)

        Ausfüllen(Me)
    End Sub
#Region "Delegaten"
    'Private Delegate Sub DelgSetValue()
    Private Delegate Sub DelgSetProgressbar(ByVal Anzahl As Integer)
#End Region

    Private Async Sub Ausfüllen(ByVal m_Control As Control)
        Dim tmpPropertyInfo As Reflection.PropertyInfo

        For Each ctrl As Control In m_Control.Controls

            If ctrl.Controls.Count > 0 Then
                Ausfüllen(ctrl)
            End If

            tmpPropertyInfo = Array.Find(XMLData.POptionen.GetType.GetProperties, Function(PropertyInfo As Reflection.PropertyInfo) PropertyInfo.Name.AreEqual("P" & ctrl.Name))

            If ctrl.GetType().Equals(GetType(TextBox)) Or
               ctrl.GetType().Equals(GetType(MaskedTextBox)) Or
               ctrl.GetType().Equals(GetType(CheckBox)) Then

                If tmpPropertyInfo IsNot Nothing Then
                    Select Case ctrl.GetType
                        Case GetType(TextBox)
                            CType(ctrl, TextBox).Text = tmpPropertyInfo.GetValue(XMLData.POptionen).ToString
                        Case GetType(MaskedTextBox)
                            If tmpPropertyInfo.GetValue(XMLData.POptionen)?.ToString.Length.IsNotZero Then
                                CType(ctrl, MaskedTextBox).Text = "1234"
                            End If
                        Case GetType(CheckBox)
                            CType(ctrl, CheckBox).Checked = CBool(tmpPropertyInfo.GetValue(XMLData.POptionen).ToString)
                    End Select
                End If

                If ctrl.Name.AreEqual(TBLogging.Name) Then
                    Dim LogDatei As String = IO.Path.Combine(XMLData.POptionen.PArbeitsverzeichnis, PDfltLog_FileName)
                    LinkLogFile.Text = LogDatei

                    With My.Computer.FileSystem
                        If .FileExists(LogDatei) Then
                            Using reader As New IO.StreamReader(LogDatei)
                                TBLogging.Text = Await reader.ReadToEndAsync
                            End Using
                        End If
                    End With
                End If

            ElseIf ctrl.GetType().Equals(GetType(FBoxDataGridView)) Then
                ' Datagridview der Telefoniegeräte
                SetTelDGV()
            ElseIf ctrl.GetType().Equals(GetType(CheckedListBox)) Then
                ' CheckedListBox der zu überwachenden Nummern
                SetCheckedListBox(CType(ctrl, CheckedListBox))
            ElseIf ctrl.GetType().Equals(GetType(DateTimePicker)) Then
                ' Anrufmonitor Simulation
                ctrl.Text = Date.Now.ToString
            ElseIf ctrl.GetType().Equals(GetType(ComboBox)) Then

                If tmpPropertyInfo IsNot Nothing Then
                    SetComboBox(CType(ctrl, ComboBox), tmpPropertyInfo.GetValue(XMLData.POptionen).ToString)
                Else
                    ' Anrufmonitor Simulation
                    SetComboBox(CType(ctrl, ComboBox))
                End If

            ElseIf ctrl.GetType().Equals(GetType(OlOrdnerTreeView)) Then

                ' Eigentlich unnätig, aber aus irgendeinem Grund können die Felder Nothing sein
                If XMLData.POptionen.OutlookOrdner Is Nothing Then XMLData.POptionen.OutlookOrdner = New OutlookOrdnerListe
                If XMLData.POptionen.OutlookOrdner.OrdnerListe Is Nothing Then XMLData.POptionen.OutlookOrdner.OrdnerListe = New List(Of OutlookOrdner)

                Dim VTyp As OutlookOrdnerVerwendung
                Dim olfldrTV As OlOrdnerTreeView = CType(ctrl, OlOrdnerTreeView)

                Select Case True
                    Case olfldrTV Is TreeViewKontakteSuche
                        VTyp = OutlookOrdnerVerwendung.KontaktSuche
                    Case olfldrTV Is TreeViewJournal
                        VTyp = OutlookOrdnerVerwendung.JournalSpeichern
                    Case olfldrTV Is TreeViewKontakteErstellen
                        VTyp = OutlookOrdnerVerwendung.KontaktSpeichern
                End Select

                olfldrTV.CheckedOlFolders = XMLData.POptionen.OutlookOrdner.FindAll(VTyp)

            End If
        Next
    End Sub

    Sub Speichern(ByVal m_Control As Control)
        Dim tmpPropertyInfo As Reflection.PropertyInfo

        For Each ctrl As Control In m_Control.Controls

            If ctrl.Controls.Count > 0 Then
                Speichern(ctrl)
            End If

            If ctrl.GetType().Equals(GetType(TextBox)) Or
               ctrl.GetType().Equals(GetType(MaskedTextBox)) Or
               ctrl.GetType().Equals(GetType(CheckBox)) Or
               ctrl.GetType().Equals(GetType(ComboBox)) Then

                tmpPropertyInfo = Array.Find(XMLData.POptionen.GetType.GetProperties, Function(PropertyInfo As Reflection.PropertyInfo) PropertyInfo.Name.AreEqual("P" & ctrl.Name))

                If tmpPropertyInfo IsNot Nothing Then
                    Select Case ctrl.GetType
                        Case GetType(TextBox)
                            Select Case tmpPropertyInfo.PropertyType
                                Case GetType(Integer)
                                    tmpPropertyInfo.SetValue(XMLData.POptionen, CType(ctrl, TextBox).Text.ToInt)
                                Case GetType(String)
                                    tmpPropertyInfo.SetValue(XMLData.POptionen, CType(ctrl, TextBox).Text)
                            End Select

                        Case GetType(MaskedTextBox)
                            If CType(ctrl, MaskedTextBox).Text.AreNotEqual("1234") Then
                                Using Crypt As Rijndael = New Rijndael
                                    tmpPropertyInfo.SetValue(XMLData.POptionen, Crypt.EncryptString128Bit(CType(ctrl, MaskedTextBox).Text, If(ctrl.Name.AreEqual(TBPasswort.Name), DefaultWerte.PDfltDeCryptKey, DefaultWerte.PDfltDeCryptKeyPhoner)))
                                End Using
                            End If

                        Case GetType(CheckBox)
                            tmpPropertyInfo.SetValue(XMLData.POptionen, CType(ctrl, CheckBox).Checked)

                        Case GetType(ComboBox)
                            Select Case ctrl.Name
                                Case CBoxPhonerSIP.Name

                                Case Else
                                    tmpPropertyInfo.SetValue(XMLData.POptionen, CType(ctrl, ComboBox).SelectedItem.ToString)
                            End Select

                    End Select
                End If

                If ctrl Is CBoxPhonerSIP Then

                    ' allen SIP-Telefonen den Phoner Flag auf False setzen
                    XMLData.PTelefonie.Telefoniegeräte.Where(Function(t) t.TelTyp = DfltWerteTelefonie.TelTypen.IP).ToList.ForEach(Sub(tr) tr.IsPhoner = False)

                    ' Telefoniegerät finden
                    With CType(ctrl, ComboBox)
                        If .SelectedItem IsNot Nothing Then
                            CType(.SelectedItem, Telefoniegerät).IsPhoner = True
                        End If
                    End With

                End If


            ElseIf ctrl.GetType().Equals(GetType(CheckedListBox)) Then
                For Each tmpTelNr As Telefonnummer In XMLData.PTelefonie.Telefonnummern
                    tmpTelNr.Überwacht = CLBTelNr.CheckedItems.Contains(tmpTelNr)
                Next

            ElseIf ctrl.GetType().Equals(GetType(OlOrdnerTreeView)) Then
                Dim VTyp As OutlookOrdnerVerwendung
                Dim olfldrTV As OlOrdnerTreeView = CType(ctrl, OlOrdnerTreeView)
                Select Case True
                    Case olfldrTV Is TreeViewKontakteSuche
                        VTyp = OutlookOrdnerVerwendung.KontaktSuche

                        ' Deindiziere die entfernten Ordner
                        StarteIndizierung(XMLData.POptionen.OutlookOrdner.FindAll(VTyp).Except(olfldrTV.CheckedOlFolders), False)

                        ' Indiziere alle neu hinzugefügten Ordner
                        StarteIndizierung(olfldrTV.CheckedOlFolders.Except(XMLData.POptionen.OutlookOrdner.FindAll(VTyp)), True)

                    Case olfldrTV Is TreeViewJournal
                        VTyp = OutlookOrdnerVerwendung.JournalSpeichern

                    Case olfldrTV Is TreeViewKontakteErstellen
                        VTyp = OutlookOrdnerVerwendung.KontaktSpeichern
                End Select

                ' Speichere den Verwendungstypen
                olfldrTV.CheckedOlFolders.ForEach(Sub(T) T.Typ = VTyp)

                ' Entferne aus den Einstellungen alle Ordner mit dem Typen
                XMLData.POptionen.OutlookOrdner.RemoveAll(VTyp)

                ' Kopiere alle neuen Ordner in die Liste
                XMLData.POptionen.OutlookOrdner.AddRange(olfldrTV.CheckedOlFolders)


            ElseIf ctrl.GetType().Equals(GetType(FBoxDataGridView)) Then
                If ctrl Is DGVTelList Then
                    ' Standard-Telefon ermitteln.
                    With CType(ctrl, FBoxDataGridView)
                        Dim DatenZeilen As List(Of TelGeräteListDataRow) = CType(CType(.DataSource, BindingSource).DataSource, TelGeräteListDataTable).Rows.Cast(Of TelGeräteListDataRow)().ToList()

                        For Each Datenreihe As TelGeräteListDataRow In DatenZeilen
                            Datenreihe.Gerät.StdTelefon = Datenreihe.Field(Of Boolean)("Check")
                        Next
                    End With
                End If
            End If
        Next
    End Sub

#Region "Button Click"
    Private Async Sub Button_Click(sender As Object, e As EventArgs) Handles BOK.Click,
                                                                       BApply.Click,
                                                                       BXML.Click,
                                                                       BTestLogin.Click,
                                                                       BTelefonliste.Click,
                                                                       BReset.Click,
                                                                       BArbeitsverzeichnis.Click,
                                                                       BAbbruch.Click,
                                                                       BIndizierungStart.Click,
                                                                       BIndizierungAbbrechen.Click,
                                                                       BRWSTest.Click,
                                                                       BPhonerTest.Click,
                                                                       BKontaktOrdnerSuche.Click,
                                                                       BKontaktOrdnerErstellen.Click,
                                                                       BJournalOrdnerErstellen.Click

        Select Case True'CType(sender, Button).Name
            Case sender Is BOK, sender Is BApply
                ' Formulardaten in zurück in Properties
                Speichern(Me)
                ' Valid-IP neu ermitteln
                XMLData.POptionen.PValidFBAdr = ValidIP(XMLData.POptionen.PTBFBAdr)
                ' Properties in Datei umwandeln
                XMLData.Speichern(IO.Path.Combine(XMLData.POptionen.PArbeitsverzeichnis, $"{PDfltAddin_KurzName}.xml"))
            Case sender Is BTestLogin
                '' Überführe das eingegebene Passwort in die Property
                'Using Crypt As Rijndael = New Rijndael
                '    XMLData.POptionen.PTBPasswort = Crypt.EncryptString128Bit(TBPasswort.Text)
                'End Using

                ' Zum Testen der verschiedener Funktionen
            Case sender Is BTelefonliste
                ' Formulardaten in Properties speichern
                Speichern(Me)
                ' Indizierung starten
                StarteEinlesen()

            Case sender Is BIndizierungStart
                ' Formulardaten in Properties speichern
                ' Speichern(Me)
                ' Indizierung starten
                StarteIndizierung(TreeViewKontakteSuche.CheckedOlFolders, RadioButtonErstelle.Checked)

            Case sender Is BIndizierungAbbrechen
                ' Indizierung abbrechen
                If BWIndexerList IsNot Nothing AndAlso BWIndexerList.Any Then
                    BWIndexerList.ForEach(Sub(r) r.CancelAsync())
                End If
                ' Buttons wieder umschalten
                BIndizierungAbbrechen.Enabled = False
                BIndizierungStart.Enabled = True

            Case sender Is BXML
                ' XML-Datei mit Systemstandard öffnen
                Process.Start(IO.Path.Combine(XMLData.POptionen.PArbeitsverzeichnis, PDfltConfig_FileName))

            Case sender Is BRWSTest
                If IsNumeric(TBRWSTest.Text) Then
                    Dim vCard As String = Await StartRWS(New Telefonnummer() With {.SetNummer = TBRWSTest.Text}, False)
                    If Not vCard.StartsWith(PDfltBegin_vCard) Then vCard = PRWSTestKeinEintrag
                    MsgBox(PRWSTest(TBRWSTest.Text, vCard), MsgBoxStyle.Information, "Test der Rückwärtssuche")
                End If

            Case sender Is BKontaktOrdnerSuche
                TreeViewKontakteSuche.AddOutlookBaseNodes(Outlook.OlItemType.olContactItem, OutlookOrdnerVerwendung.KontaktSuche, True, CBSucheUnterordner.Checked)

            Case sender Is BJournalOrdnerErstellen
                TreeViewJournal.AddOutlookBaseNodes(Outlook.OlItemType.olJournalItem, OutlookOrdnerVerwendung.JournalSpeichern, False, False)

            Case sender Is BKontaktOrdnerErstellen
                TreeViewKontakteErstellen.AddOutlookBaseNodes(Outlook.OlItemType.olContactItem, OutlookOrdnerVerwendung.KontaktSpeichern, False, False)

            Case sender Is BPhonerTest
                ' Formulardaten in Properties speichern
                Speichern(Me)

                Using p As New Phoner

                    MsgBox($"Die Authentifizierung mit Phoner war {If(p.CheckPhonerAuth, PDfltStringEmpty, "nicht ")}erfolgreich.", MsgBoxStyle.Information, "Phoner Authentifizierungstest")
                End Using
        End Select
    End Sub

    Private Sub LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLogFile.LinkClicked, LinkPhoner.LinkClicked
        Select Case CType(sender, LinkLabel).Name
            Case LinkLogFile.Name
                Process.Start(IO.Path.Combine(XMLData.POptionen.PArbeitsverzeichnis, PDfltLog_FileName))

            Case LinkPhoner.Name
                Process.Start("http://www.phoner.de/")
        End Select
    End Sub

#End Region

#Region "CheckedChanged"
    Private Sub CheckedChanged(sender As Object, e As EventArgs) Handles CBPhoner.CheckedChanged
        Select Case CType(sender, CheckBox).Name
            Case CBPhoner.Name
                TBPhonerPasswort.Enabled = CBPhoner.Checked
                LPassworPhoner.Enabled = CBPhoner.Checked
                CBoxPhonerSIP.Enabled = CBPhoner.Checked
                LPhonerSIPTelefon.Enabled = CBPhoner.Checked
                BPhonerTest.Enabled = CBPhoner.Checked
        End Select

    End Sub
#End Region

    Private Sub FormCfg_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If ThisAddIn.PAnrufmonitor IsNot Nothing Then
            'For Each T As Telefonat In ThisAddIn.PAnrufmonitor.AktiveTelefonate.FindAll(Function(TEL) TEL.AnrMonSimuliert)
            '    If T.AnrMonPopUp IsNot Nothing Then
            '        T.AnrMonPopUp.AnrMonAusblenden()
            '    End If
            'Next
            'ThisAddIn.PAnrufmonitor.AktiveTelefonate.RemoveAll(Function(TEL) TEL.AnrMonSimuliert)
        End If
    End Sub

    Private Sub SetCheckedListBox(ByVal CLB As CheckedListBox)

        With CLB
            .DataBindings.Clear()
            .DataSource = XMLData.PTelefonie.Telefonnummern
            .DisplayMember = NameOf(Telefonnummer.Unformatiert)
            .ValueMember = NameOf(Telefonnummer.Überwacht)

            For i As Integer = 0 To .Items.Count - 1
                .SetItemChecked(i, CType(.Items(i), Telefonnummer).Überwacht)
            Next
        End With

    End Sub

    Private Sub SetComboBox(ByVal CBox As ComboBox, ByVal SelektiertesElement As String)
        With CBox
            .DataBindings.Clear()
            Select Case CBox.Name
                Case CBoxMinLogLevel.Name
                    .SelectedItem = SelektiertesElement
            End Select
        End With
    End Sub

    Private Sub SetComboBox(ByVal CBox As ComboBox)
        With CBox
            .DataBindings.Clear()
            Select Case CBox.Name
                Case CBoxAnrMonSimRINGEigTelNr.Name, CBoxAnrMonSimCALLEigTelNr.Name
                    .DataSource = XMLData.PTelefonie.Telefonnummern
                    .DisplayMember = NameOf(Telefonnummer.Einwahl)
                    .ValueMember = NameOf(Telefonnummer.Einwahl)
                Case CBoxAnrMonSimCALLNSTID.Name, CBoxAnrMonSimCONNECTNSTID.Name
                    .DataSource = XMLData.PTelefonie.Telefoniegeräte
                    .DisplayMember = NameOf(Telefoniegerät.Name)
                    .ValueMember = NameOf(Telefoniegerät.AnrMonID)
                Case CBoxPhonerSIP.Name
                    .DataSource = XMLData.PTelefonie.Telefoniegeräte.Where(Function(t) t.TelTyp = DfltWerteTelefonie.TelTypen.IP).ToList
                    .DisplayMember = NameOf(Telefoniegerät.Name)
                    .ValueMember = NameOf(Telefoniegerät.AnrMonID)
                    .SelectedItem = XMLData.PTelefonie.Telefoniegeräte.Find(Function(t) t.TelTyp = DfltWerteTelefonie.TelTypen.IP And t.IsPhoner)
            End Select
        End With
    End Sub

    Friend Sub StarteEinlesen()
        If Ping(XMLData.POptionen.PValidFBAdr) Then
            If FritzBoxDaten Is Nothing Then FritzBoxDaten = New FritzBoxData
            FritzBoxDaten.FritzBoxDatenJSON()
            ' Fülle das Datagridview

            SetTelDGV()
            SetCheckedListBox(CLBTelNr)
        End If
    End Sub

    Private Sub SetTelDGV()

        With DGVTelList
            .DataBindings.Clear()
            .Columns.Clear()
            ' Spalten hinzufügen
            .AddCheckBoxColumn("Check", "*")
            .AddTextColumn("Nr", "Nr.", DataGridViewContentAlignment.MiddleRight, GetType(Integer), DataGridViewAutoSizeColumnMode.AllCells)
            .AddTextColumn("ID", "Dialport", DataGridViewContentAlignment.MiddleRight, GetType(Integer), DataGridViewAutoSizeColumnMode.AllCells)
            .AddTextColumn("AnrMonID", "Anrufmonitor ID", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.AllCells)
            .AddTextColumn("Name", "Telefonname", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.Fill)
            .AddTextColumn("ENummern", "Eingehende Nummern", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.Fill)

            ' Datentabelle füllen
            .DataSource = New BindingSource With {.DataSource = ConvertToDataTable()}
            .Enabled = True
        End With

    End Sub

    Private Function ConvertToDataTable() As TelGeräteListDataTable
        Dim Datentabelle As New TelGeräteListDataTable
        Dim DatenZeile As TelGeräteListDataRow

        With Datentabelle.Columns
            .Add("Check", GetType(Boolean))
            .Add("Nr", GetType(Integer))
            .Add("ID", GetType(Integer))
            .Add("AnrMonID", GetType(Integer))
            .Add("Name", GetType(String))
            .Add("ENummern", GetType(String))
        End With
        ' Primary Key setzen (Zum Suchen in der Datatable)
        Datentabelle.PrimaryKey = {Datentabelle.Columns.Item("ID")}

        With Datentabelle
            ' Zeilen hinzufügen
            If XMLData.PTelefonie IsNot Nothing AndAlso XMLData.PTelefonie.Telefoniegeräte IsNot Nothing Then
                For Each TelGerät As Telefoniegerät In XMLData.PTelefonie.Telefoniegeräte

                    DatenZeile = CType(.Rows.Add(TelGerät.StdTelefon, XMLData.PTelefonie.Telefoniegeräte.IndexOf(TelGerät) + 1, TelGerät.Dialport, TelGerät.AnrMonID, TelGerät.Name, If(TelGerät.StrEinTelNr IsNot Nothing, String.Join(PDflt1NeueZeile, TelGerät.StrEinTelNr), PDfltStringEmpty)), TelGeräteListDataRow)
                    DatenZeile.Gerät = TelGerät

                Next
            End If
        End With
        Return Datentabelle
    End Function


    Private Sub FritzBoxDaten_Status(sender As Object, e As NotifyEventArgs(Of String)) Handles FritzBoxDaten.Status
        TSSL_Telefone.Text = e.Value
    End Sub

#Region "AnrMonSim"
    'Private Sub AnrMonSim_ValueChanged(sender As Object, e As EventArgs) Handles DTPAnrMonSimRING.ValueChanged, DTPAnrMonSimCALL.ValueChanged, DTPAnrMonSimCONNECT.ValueChanged, DTPAnrMonSimDISCONNECT.ValueChanged,
    '                                                                             TBAnrMonSimRINGID.TextChanged, TBAnrMonSimCALLID.TextChanged, TBAnrMonSimCONNECTID.TextChanged, TBAnrMonSimDISCONNECTID.TextChanged,
    '                                                                             TBAnrMonSimRINGAugTelNr.TextChanged, TBAnrMonSimCALLAugTelNr.TextChanged, TBAnrMonSimCONNECTAugTelNr.TextChanged,
    '                                                                             CBoxAnrMonSimRINGEigTelNr.SelectedIndexChanged, CBoxAnrMonSimCALLEigTelNr.SelectedIndexChanged,
    '                                                                             CBoxAnrMonSimRINGSIPID.SelectedIndexChanged, CBoxAnrMonSimCALLSIPID.SelectedIndexChanged,
    '                                                                             CBoxAnrMonSimCALLNSTID.SelectedIndexChanged, CBoxAnrMonSimCONNECTNSTID.SelectedIndexChanged,
    '                                                                             TBAnrMonSimDISCONNECTDauer.TextChanged
    '    Select Case CType(sender, Control).Name
    '        Case DTPAnrMonSimRING.Name, TBAnrMonSimRINGID.Name, TBAnrMonSimRINGAugTelNr.Name, CBoxAnrMonSimRINGEigTelNr.Name, CBoxAnrMonSimRINGSIPID.Name
    '            '         0        ; 1  ;2;    3     ;  4   ; 5  ; 6
    '            ' 23.06.18 13:20:24;RING;1;0123456789;987654;SIP4;
    '            LAnrMonSimLabelRING.Text = String.Join(Anrufmonitor.AnrMon_Delimiter, DTPAnrMonSimRING.Value, Anrufmonitor.AnrMon_RING, TBAnrMonSimRINGID.Text, TBAnrMonSimRINGAugTelNr.Text, CBoxAnrMonSimRINGEigTelNr.SelectedValue, CBoxAnrMonSimRINGSIPID.SelectedText) & Anrufmonitor.AnrMon_Delimiter

    '        Case DTPAnrMonSimCALL.Name, TBAnrMonSimCALLID.Name, CBoxAnrMonSimCALLNSTID.Name, CBoxAnrMonSimCALLEigTelNr.Name, TBAnrMonSimCALLAugTelNr.Name, CBoxAnrMonSimCALLSIPID.Name
    '            '         0        ; 1  ;2;3;  4   ;    5     ; 6  ; 7
    '            ' 23.06.18 13:20:24;CALL;3;4;987654;0123456789;SIP0;
    '            LAnrMonSimLabelCALL.Text = String.Join(Anrufmonitor.AnrMon_Delimiter, DTPAnrMonSimCALL.Value, Anrufmonitor.AnrMon_CALL, TBAnrMonSimCALLID.Text, CBoxAnrMonSimCALLNSTID.SelectedValue, CBoxAnrMonSimCALLEigTelNr.Text, TBAnrMonSimCALLAugTelNr.Text, CBoxAnrMonSimCALLSIPID.SelectedText) & Anrufmonitor.AnrMon_Delimiter

    '        Case DTPAnrMonSimCONNECT.Name, TBAnrMonSimCONNECTID.Name, CBoxAnrMonSimCONNECTNSTID.Name, TBAnrMonSimCONNECTAugTelNr.Text
    '            '         0        ;   1   ;2;3 ;    4     ; 5 
    '            ' 23.06.18 13:20:44;CONNECT;1;40;0123456789;
    '            LAnrMonSimLabelCONNECT.Text = String.Join(Anrufmonitor.AnrMon_Delimiter, DTPAnrMonSimCONNECT.Value, Anrufmonitor.AnrMon_CONNECT, TBAnrMonSimCONNECTID.Text, CBoxAnrMonSimCONNECTNSTID.SelectedValue, TBAnrMonSimCONNECTAugTelNr.Text) & Anrufmonitor.AnrMon_Delimiter

    '        Case DTPAnrMonSimDISCONNECT.Name, TBAnrMonSimDISCONNECTID.Name, TBAnrMonSimDISCONNECTDauer.Name
    '            '         0        ;   1      ;2;3; 4
    '            ' 23.06.18 13:20:52;DISCONNECT;1;9;
    '            LAnrMonSimLabelDISCONNECT.Text = String.Join(Anrufmonitor.AnrMon_Delimiter, DTPAnrMonSimDISCONNECT.Value, Anrufmonitor.AnrMon_DISCONNECT, TBAnrMonSimDISCONNECTID.Text, TBAnrMonSimDISCONNECTDauer.Text) & Anrufmonitor.AnrMon_Delimiter

    '    End Select
    'End Sub

    'Private Sub BAnrMonSim_Click(sender As Object, e As EventArgs) Handles BAnrMonSimRING.Click, BAnrMonSimCALL.Click, BAnrMonSimCONNECT.Click, BAnrMonSimDISCONNECT.Click

    '    'Select Case CType(sender, Control).Name
    '    '    Case BAnrMonSimRING.Name
    '    '        ThisAddIn.PAnrufmonitor.AnrMonSimulation(LAnrMonSimLabelRING.Text)
    '    '    Case BAnrMonSimCALL.Name
    '    '        ThisAddIn.PAnrufmonitor.AnrMonSimulation(LAnrMonSimLabelCALL.Text)
    '    '    Case BAnrMonSimCONNECT.Name
    '    '        ThisAddIn.PAnrufmonitor.AnrMonSimulation(LAnrMonSimLabelCONNECT.Text)
    '    '    Case BAnrMonSimDISCONNECT.Name
    '    '        ThisAddIn.PAnrufmonitor.AnrMonSimulation(LAnrMonSimLabelDISCONNECT.Text)
    '    'End Select

    'End Sub
#End Region

#Region "Indizierung"

    Private Structure Indizierungsdaten
        Dim Erstellen As Boolean
        Dim olFolder As Outlook.MAPIFolder
    End Structure

    Private Sub StarteIndizierung(ByVal OrdnerListe As IEnumerable(Of OutlookOrdner), ByVal Erstellen As Boolean)
        ' Initialisiere die Progressbar
        InitProgressbar(0)

        If OrdnerListe.Any Then

            If BWIndexerList Is Nothing Then BWIndexerList = New List(Of BackgroundWorker)

            ' Schleife durch jeden Ordner der indiziert werden soll
            For Each Ordner As OutlookOrdner In OrdnerListe

                ' Buttons einschalten
                BIndizierungAbbrechen.Enabled = True
                BIndizierungStart.Enabled = False

                Dim BWIndexer As New BackgroundWorker

                With BWIndexer
                    ' Füge Ereignishandler hinzu
                    AddHandler .DoWork, AddressOf BWIndexer_DoWork
                    AddHandler .ProgressChanged, AddressOf BWIndexer_ProgressChanged
                    AddHandler .RunWorkerCompleted, AddressOf BWIndexer_RunWorkerCompleted

                    ' Setze Flags
                    .WorkerSupportsCancellation = True
                    .WorkerReportsProgress = True
                    ' Und los...
                    NLogger.Debug("Starte {0}. Backgroundworker für Kontaktindizierung im Ordner {1}.", BWIndexerList.Count, Ordner.Name)
                    .RunWorkerAsync(New Indizierungsdaten With {.Erstellen = Erstellen, .olFolder = Ordner.MAPIFolder})
                End With

                ' Füge dern Backgroundworker der Liste hinzu
                BWIndexerList.Add(BWIndexer)
            Next
        End If
    End Sub

    Private Sub BWIndexer_DoWork(sender As Object, e As DoWorkEventArgs)
        Dim BWIndexer As BackgroundWorker = CType(sender, BackgroundWorker)

        Dim Daten As Indizierungsdaten = CType(e.Argument, Indizierungsdaten)
        Dim AddtoMaxValue As Integer = ZähleOutlookKontakte(Daten.olFolder)

        If InvokeRequired Then
            Invoke(New DelgSetProgressbar(AddressOf SetProgressbarMax), AddtoMaxValue)
        Else
            SetProgressbarMax(AddtoMaxValue)
        End If

        KontaktIndexer(Daten.olFolder, Daten.Erstellen, BWIndexer)
    End Sub

    Private Sub KontaktIndexer(ByVal Ordner As Outlook.MAPIFolder, ByVal Erstellen As Boolean, ByVal BWIndexer As BackgroundWorker)

        Dim aktKontakt As Outlook.ContactItem  ' aktueller Kontakt

        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then

            For Each item In Ordner.Items
                If BWIndexer IsNot Nothing AndAlso BWIndexer.CancellationPending Then Exit For

                ' nur Kontakte werden durchsucht
                If TypeOf item Is Outlook.ContactItem Then
                    aktKontakt = CType(item, Outlook.ContactItem)

                    If Erstellen Then
                        IndiziereKontakt(aktKontakt)
                    Else
                        DeIndiziereKontakt(aktKontakt)
                    End If

                    aktKontakt.Speichern

                    aktKontakt.ReleaseComObject

                End If

                If BWIndexer?.IsBusy Then BWIndexer.ReportProgress(1)
            Next

            If Not Erstellen Then
                ' Entfernt alle Indizierungseinträge aus den Ordnern des Kontaktelementes.
                DeIndizierungOrdner(Ordner)
            End If

            ' Unterordner werden rekursiv durchsucht und indiziert
            If XMLData.POptionen.PCBSucheUnterordner Then
                Dim iOrdner As Integer = 1
                Do While (iOrdner.IsLessOrEqual(Ordner.Folders.Count)) Or (BWIndexer IsNot Nothing AndAlso BWIndexer.CancellationPending)
                    KontaktIndexer(Ordner.Folders.Item(iOrdner), Erstellen, BWIndexer)
                    iOrdner += 1
                Loop
            End If

            Ordner.ReleaseComObject
        End If

    End Sub

    Private Sub BWIndexer_ProgressChanged(sender As Object, e As ProgressChangedEventArgs)
        'Dim BWIndexer As BackgroundWorker = CType(sender, BackgroundWorker)
        If InvokeRequired Then
            Invoke(New DelgSetProgressbar(AddressOf SetProgressbar), e.ProgressPercentage)
        Else
            SetProgressbar(e.ProgressPercentage)
        End If
    End Sub

    Private Sub BWIndexer_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs)
        Dim BWIndexer As BackgroundWorker = CType(sender, BackgroundWorker)

        ' Backgroundworker aus der Liste entfernen
        BWIndexerList.Remove(BWIndexer)

        With BWIndexer
            ' Ereignishandler entfernen
            RemoveHandler .DoWork, AddressOf BWIndexer_DoWork
            RemoveHandler .ProgressChanged, AddressOf BWIndexer_ProgressChanged
            RemoveHandler .RunWorkerCompleted, AddressOf BWIndexer_RunWorkerCompleted

            BWIndexer.Dispose()
        End With
        NLogger.Info("Indizierung eines Ordners ist abgeschlossen.")

        ' Liste leeren, wenn kein Element mehr enthalten
        If Not BWIndexerList.Any Then
            BWIndexerList = Nothing
            NLogger.Info("Die komplette Indizierung ist abgeschlossen.")

            BIndizierungAbbrechen.Enabled = False
            BIndizierungStart.Enabled = True
        End If

    End Sub

    Private Sub InitProgressbar(ByVal Initialwert As Integer)
        ProgressBarIndex.Value = Initialwert
        ProgressBarIndex.Maximum = Initialwert
        LabelAnzahl.Text = $"Status: {Initialwert}/{ProgressBarIndex.Maximum}"
    End Sub

    Private Sub SetProgressbar(ByVal Anzahl As Integer)
        ProgressBarIndex.Value += Anzahl
        LabelAnzahl.Text = $"Status: {ProgressBarIndex.Value}/{ProgressBarIndex.Maximum}"
    End Sub

    Private Sub SetProgressbarMax(ByVal NeuesMaximum As Integer)
        ProgressBarIndex.Maximum += NeuesMaximum
    End Sub

#End Region

End Class


