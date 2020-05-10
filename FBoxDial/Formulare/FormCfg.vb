Imports System.ComponentModel
Imports System.Data
Imports System.Windows.Forms
Imports Microsoft.Office.Interop

Public Class FormCfg
    Implements IDisposable
    Private Shared Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

    Private FritzBoxDaten As FritzBoxData

    Private BWIndexerList As List(Of BackgroundWorker)

    Private IndizierteOrdner As List(Of IndizerterOrdner)

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
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

            ElseIf ctrl.GetType().Equals(GetType(TreeView)) Then
                Select Case ctrl.Name
                    Case TreeViewKontakte.Name
                        ' lade die Liste der zu indizierenden Ordner
                        If XMLData.POptionen.IndizerteOrdner Is Nothing Then XMLData.POptionen.IndizerteOrdner = New IndizieOrdnerListe
                        If XMLData.POptionen.IndizerteOrdner.OrdnerListe Is Nothing Then XMLData.POptionen.IndizerteOrdner.OrdnerListe = New List(Of IndizerterOrdner)
                        ' Als Kopie
                        IndizierteOrdner = New List(Of IndizerterOrdner)
                        IndizierteOrdner.AddRange(XMLData.POptionen.IndizerteOrdner.OrdnerListe)
                End Select
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
                                    tmpPropertyInfo.SetValue(XMLData.POptionen, Crypt.EncryptString128Bit(CType(ctrl, MaskedTextBox).Text))
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
                    ' Telefoniegerät finden
                    With CType(CType(ctrl, ComboBox).SelectedItem, Telefoniegerät)
                        .IsPhoner = True
                    End With

                End If


            ElseIf ctrl.GetType().Equals(GetType(CheckedListBox)) Then
                For Each tmpTelNr As Telefonnummer In XMLData.PTelefonie.Telefonnummern
                    tmpTelNr.Überwacht = CLBTelNr.CheckedItems.Contains(tmpTelNr)
                Next

            ElseIf ctrl.GetType().Equals(GetType(TreeView)) Then
                Select Case ctrl.Name
                    Case TreeViewKontakte.Name
                        ' Deindiziere die entfernen Ordner
                        StarteIndizierung(XMLData.POptionen.IndizerteOrdner.OrdnerListe.Except(IndizierteOrdner), False)

                        '' Indiziere alle neu hinzugefügten Ordner
                        StarteIndizierung(IndizierteOrdner.Except(XMLData.POptionen.IndizerteOrdner.OrdnerListe), True)

                        ' Speicher die Liste der zu indizierenden Ordner. leere die alten Daten
                        XMLData.POptionen.IndizerteOrdner.OrdnerListe.Clear()
                        ' kopiere alle Einträge 
                        XMLData.POptionen.IndizerteOrdner.OrdnerListe.AddRange(IndizierteOrdner)
                End Select

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
                                                                       BKontOrdLaden.Click,
                                                                       BIndizierungStart.Click,
                                                                       BIndizierungAbbrechen.Click,
                                                                       BRWSTest.Click

        Select Case CType(sender, Button).Name
            Case BOK.Name, BApply.Name
                ' Formulardaten in zurück in Properties
                Speichern(Me)
                ' Valid-IP neu ermitteln
                XMLData.POptionen.PValidFBAdr = ValidIP(XMLData.POptionen.PTBFBAdr)
                ' Properties in Datei umwandeln
                XMLData.Speichern
            Case BTestLogin.Name
                '' Überführe das eingegebene Passwort in die Property
                'Using Crypt As Rijndael = New Rijndael
                '    XMLData.POptionen.PTBPasswort = Crypt.EncryptString128Bit(TBPasswort.Text)
                'End Using

                ' Zum Testen der verschiedener Funktionen
            Case BTelefonliste.Name
                ' Formulardaten in Properties speichern
                Speichern(Me)
                ' Indizierung starten
                StarteEinlesen()
            Case BIndizierungStart.Name
                ' Formulardaten in Properties speichern
                ' Speichern(Me)
                ' Indizierung starten
                StarteIndizierung(IndizierteOrdner, RadioButtonErstelle.Checked)
            Case BIndizierungAbbrechen.Name
                ' Indizierung abbrechen
                If BWIndexerList IsNot Nothing AndAlso BWIndexerList.Any Then
                    BWIndexerList.ForEach(Sub(r) r.CancelAsync())
                End If
                ' Buttons wieder umschalten
                BIndizierungAbbrechen.Enabled = False
                BIndizierungStart.Enabled = True
            Case BXML.Name
                ' XML-Datei mit Systemstandard öffnen
                Process.Start(IO.Path.Combine(XMLData.POptionen.PArbeitsverzeichnis, PDfltConfig_FileName))
            Case BRWSTest.Name
                If IsNumeric(TBRWSTest.Text) Then
                    Dim vCard As String = Await StartRWS(New Telefonnummer() With {.SetNummer = TBRWSTest.Text}, False)
                    If Not vCard.StartsWith(PDfltBegin_vCard) Then vCard = PRWSTestKeinEintrag
                    MsgBox(PRWSTest(TBRWSTest.Text, vCard), MsgBoxStyle.Information, "Test der Rückwärtssuche")
                End If
            Case BKontOrdLaden.Name
                KontaktOrdnerLaden()
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
        End Select

    End Sub
#End Region

    Private Sub FormCfg_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If ThisAddIn.PAnrufmonitor IsNot Nothing Then
            For Each T As Telefonat In ThisAddIn.PAnrufmonitor.AktiveTelefonate.FindAll(Function(TEL) TEL.AnrMonSimuliert)
                If T.AnrMonPopUp IsNot Nothing Then
                    T.AnrMonPopUp.AnrMonAusblenden()
                End If
            Next
            ThisAddIn.PAnrufmonitor.AktiveTelefonate.RemoveAll(Function(TEL) TEL.AnrMonSimuliert)
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

#Region "AnrMonSim"
    Private Sub AnrMonSim_ValueChanged(sender As Object, e As EventArgs) Handles DTPAnrMonSimRING.ValueChanged, DTPAnrMonSimCALL.ValueChanged, DTPAnrMonSimCONNECT.ValueChanged, DTPAnrMonSimDISCONNECT.ValueChanged,
                                                                                 TBAnrMonSimRINGID.TextChanged, TBAnrMonSimCALLID.TextChanged, TBAnrMonSimCONNECTID.TextChanged, TBAnrMonSimDISCONNECTID.TextChanged,
                                                                                 TBAnrMonSimRINGAugTelNr.TextChanged, TBAnrMonSimCALLAugTelNr.TextChanged, TBAnrMonSimCONNECTAugTelNr.TextChanged,
                                                                                 CBoxAnrMonSimRINGEigTelNr.SelectedIndexChanged, CBoxAnrMonSimCALLEigTelNr.SelectedIndexChanged,
                                                                                 CBoxAnrMonSimRINGSIPID.SelectedIndexChanged, CBoxAnrMonSimCALLSIPID.SelectedIndexChanged,
                                                                                 CBoxAnrMonSimCALLNSTID.SelectedIndexChanged, CBoxAnrMonSimCONNECTNSTID.SelectedIndexChanged,
                                                                                 TBAnrMonSimDISCONNECTDauer.TextChanged
        Select Case CType(sender, Control).Name
            Case DTPAnrMonSimRING.Name, TBAnrMonSimRINGID.Name, TBAnrMonSimRINGAugTelNr.Name, CBoxAnrMonSimRINGEigTelNr.Name, CBoxAnrMonSimRINGSIPID.Name
                '         0        ; 1  ;2;    3     ;  4   ; 5  ; 6
                ' 23.06.18 13:20:24;RING;1;0123456789;987654;SIP4;
                LAnrMonSimLabelRING.Text = String.Join(Anrufmonitor.AnrMon_Delimiter, DTPAnrMonSimRING.Value, Anrufmonitor.AnrMon_RING, TBAnrMonSimRINGID.Text, TBAnrMonSimRINGAugTelNr.Text, CBoxAnrMonSimRINGEigTelNr.SelectedValue, CBoxAnrMonSimRINGSIPID.SelectedText) & Anrufmonitor.AnrMon_Delimiter

            Case DTPAnrMonSimCALL.Name, TBAnrMonSimCALLID.Name, CBoxAnrMonSimCALLNSTID.Name, CBoxAnrMonSimCALLEigTelNr.Name, TBAnrMonSimCALLAugTelNr.Name, CBoxAnrMonSimCALLSIPID.Name
                '         0        ; 1  ;2;3;  4   ;    5     ; 6  ; 7
                ' 23.06.18 13:20:24;CALL;3;4;987654;0123456789;SIP0;
                LAnrMonSimLabelCALL.Text = String.Join(Anrufmonitor.AnrMon_Delimiter, DTPAnrMonSimCALL.Value, Anrufmonitor.AnrMon_CALL, TBAnrMonSimCALLID.Text, CBoxAnrMonSimCALLNSTID.SelectedValue, CBoxAnrMonSimCALLEigTelNr.Text, TBAnrMonSimCALLAugTelNr.Text, CBoxAnrMonSimCALLSIPID.SelectedText) & Anrufmonitor.AnrMon_Delimiter

            Case DTPAnrMonSimCONNECT.Name, TBAnrMonSimCONNECTID.Name, CBoxAnrMonSimCONNECTNSTID.Name, TBAnrMonSimCONNECTAugTelNr.Text
                '         0        ;   1   ;2;3 ;    4     ; 5 
                ' 23.06.18 13:20:44;CONNECT;1;40;0123456789;
                LAnrMonSimLabelCONNECT.Text = String.Join(Anrufmonitor.AnrMon_Delimiter, DTPAnrMonSimCONNECT.Value, Anrufmonitor.AnrMon_CONNECT, TBAnrMonSimCONNECTID.Text, CBoxAnrMonSimCONNECTNSTID.SelectedValue, TBAnrMonSimCONNECTAugTelNr.Text) & Anrufmonitor.AnrMon_Delimiter

            Case DTPAnrMonSimDISCONNECT.Name, TBAnrMonSimDISCONNECTID.Name, TBAnrMonSimDISCONNECTDauer.Name
                '         0        ;   1      ;2;3; 4
                ' 23.06.18 13:20:52;DISCONNECT;1;9;
                LAnrMonSimLabelDISCONNECT.Text = String.Join(Anrufmonitor.AnrMon_Delimiter, DTPAnrMonSimDISCONNECT.Value, Anrufmonitor.AnrMon_DISCONNECT, TBAnrMonSimDISCONNECTID.Text, TBAnrMonSimDISCONNECTDauer.Text) & Anrufmonitor.AnrMon_Delimiter

        End Select
    End Sub

    Private Sub BAnrMonSim_Click(sender As Object, e As EventArgs) Handles BAnrMonSimRING.Click, BAnrMonSimCALL.Click, BAnrMonSimCONNECT.Click, BAnrMonSimDISCONNECT.Click

        Select Case CType(sender, Control).Name
            Case BAnrMonSimRING.Name
                ThisAddIn.PAnrufmonitor.AnrMonSimulation(LAnrMonSimLabelRING.Text)
            Case BAnrMonSimCALL.Name
                ThisAddIn.PAnrufmonitor.AnrMonSimulation(LAnrMonSimLabelCALL.Text)
            Case BAnrMonSimCONNECT.Name
                ThisAddIn.PAnrufmonitor.AnrMonSimulation(LAnrMonSimLabelCONNECT.Text)
            Case BAnrMonSimDISCONNECT.Name
                ThisAddIn.PAnrufmonitor.AnrMonSimulation(LAnrMonSimLabelDISCONNECT.Text)
        End Select

    End Sub
#End Region

#Region "Indizierung"

    Private Structure Indizierungsdaten
        Dim Erstellen As Boolean
        Dim olFolder As Outlook.MAPIFolder
    End Structure

    Private Sub StarteIndizierung(ByVal OrdnerListe As IEnumerable(Of IndizerterOrdner), ByVal Erstellen As Boolean)
        ' Initialisiere die Progressbar
        InitProgressbar(0)

        If OrdnerListe.Any Then

            If BWIndexerList Is Nothing Then BWIndexerList = New List(Of BackgroundWorker)

            ' Schleife durch jeden Ordner der indiziert werden soll
            For Each Ordner As IndizerterOrdner In OrdnerListe

                ' Buttons einschalten
                BIndizierungAbbrechen.Enabled = True
                BIndizierungStart.Enabled = False

                Dim BWIndexer As New BackgroundWorker

                With BWIndexer
                    ' Fürge Ereignishandler hinzu
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

#Region "TreeView Outlook Kontaktordner"
    Private Sub KontaktOrdnerLaden()
        If TreeViewKontakte.Nodes.Count.IsZero Then
            ' Lade ImageList
            TreeViewKontakte.ImageList = New ImageList
            With TreeViewKontakte.ImageList.Images
                .Add("Disabled", My.Resources.CheckboxDisable)
                .Add("Mix", My.Resources.CheckboxMix)
                .Add("Checked", My.Resources.CheckBox)
                .Add("Uncheck", My.Resources.CheckboxUncheck)
            End With

            TreeViewKontakte.Nodes.Clear()
            ' Lade Outlook Store
            For Each Store As Outlook.Store In ThisAddIn.POutookApplication.Session.Stores
                Dim olTreeNode As New OlOrdnerTreeNode With {.Text = $"{Store.GetRootFolder.Name} ({Store.ExchangeStoreType})", .OutlookStore = Store, .OutlookFolder = Store.GetRootFolder, .ImageKey = "Disabled"}
                TreeViewKontakte.Nodes.Add(olTreeNode)
            Next
            ' Sortieren
            TreeViewKontakte.Nodes.Sort(True, False)
        End If
    End Sub

    Private Sub TreeViewKontakte_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles TreeViewKontakte.NodeMouseClick

        Dim olBaseTreeNode As OlOrdnerTreeNode = CType(e.Node, OlOrdnerTreeNode)
        With olBaseTreeNode

            If .TreeView.HitTest(e.Location).Location = TreeViewHitTestLocations.Image Then

                If .OutlookFolder.DefaultItemType = Outlook.OlItemType.olContactItem Then
                    If .XMLEintrag Is Nothing Then
                        .XMLEintrag = New IndizerterOrdner With {.Name = olBaseTreeNode.OutlookFolder.Name, .FolderID = olBaseTreeNode.OutlookFolder.EntryID, .StoreID = olBaseTreeNode.OutlookFolder.StoreID}
                        IndizierteOrdner.Add(.XMLEintrag)

                    Else
                        .XMLEintrag = Nothing
                        IndizierteOrdner.Remove(IndizierteOrdner.Find(Function(Ordner) Ordner.FolderID.AreEqual(.OutlookFolder.EntryID) And Ordner.StoreID.AreEqual(.OutlookStore.StoreID)))

                    End If
                End If

                ' ImageKey setzen
                .SetImageKey()
            End If
            ' Lade alle direkten Unterordner
            .Erweitern()
        End With
    End Sub





#End Region
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using p As New Phoner
            p.DialPhoner("0123", False)
        End Using
    End Sub

End Class


