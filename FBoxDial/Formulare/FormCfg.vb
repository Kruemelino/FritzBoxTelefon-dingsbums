Imports System.ComponentModel
Imports System.Data
Imports System.Windows.Forms
Imports Microsoft.Office.Interop

Public Class FormCfg
    Implements IDisposable
    Private Shared Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

    Private FritzBoxDaten As FritzBoxData

    Private WithEvents BWIndexer As BackgroundWorker
    Private WithEvents BWTreeView As BackgroundWorker
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Ausfüllen(Me)
    End Sub
#Region "Delegaten"
    Private Delegate Sub DelgSetValue()
    Private Delegate Sub DelgSetProgressbarMax(ByVal Anzahl As Integer)
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
                            If tmpPropertyInfo.GetValue(XMLData.POptionen).ToString.Length.IsNotZero Then
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

            ElseIf ctrl.GetType().Equals(GetType(DataGridView)) Then
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
                If ctrl.Name.AreEqual(TVOutlookContact.Name) Then
                    ' Treeview für Kontaktordner
                    ' Treeview zurücksetzen
                    With TVOutlookContact
                        .Enabled = False
                        If .Nodes.Count > 0 Then .Nodes.Clear()
                    End With
                    ' Backgroundworker starten
                    BWTreeView = New BackgroundWorker
                    With BWTreeView
                        .WorkerReportsProgress = False
                        .RunWorkerAsync(True)
                    End With
                End If
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
                            tmpPropertyInfo.SetValue(XMLData.POptionen, CType(ctrl, ComboBox).SelectedItem.ToString)

                    End Select
                End If

            ElseIf ctrl.GetType().Equals(GetType(CheckedListBox)) Then
                For Each tmpTelNr As Telefonnummer In XMLData.PTelefonie.Telefonnummern
                    tmpTelNr.Überwacht = CLBTelNr.CheckedItems.Contains(tmpTelNr)
                Next

            ElseIf ctrl.GetType().Equals(GetType(TreeView)) Then
                If ctrl.Name.AreEqual(TVOutlookContact.Name) Then

                    If TVOutlookContact.SelectedNode IsNot Nothing Then
                        Dim tmpStr() As String = TVOutlookContact.SelectedNode.Tag.ToString.Split(";")
                        XMLData.POptionen.PTVKontaktOrdnerEntryID = tmpStr(0)
                        XMLData.POptionen.PTVKontaktOrdnerStoreID = tmpStr(1)
                    End If

                End If
            ElseIf ctrl.GetType().Equals(GetType(DataGridView)) Then
                If ctrl.Name.AreEqual(DGVTelList.Name) Then
                    ' Standard-Telefon ermitteln.
                    With CType(ctrl, DataGridView)
                        Dim DatenZeilen As List(Of TelGeräteListDataRow) = CType(CType(.DataSource, BindingSource).DataSource, TelGeräteListDataTable).Rows.Cast(Of TelGeräteListDataRow)().ToList()

                        For Each Datenreihe As TelGeräteListDataRow In DatenZeilen
                            Datenreihe.Gerät.StdTelefon = Datenreihe.Field(Of Boolean)("Std")
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
                                                                       BIndizierungStart.Click,
                                                                       BIndizierungAbbrechen.Click,
                                                                       BArbeitsverzeichnis.Click,
                                                                       BAbbruch.Click,
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
                StarteEinlesen()
            Case BIndizierungStart.Name
                StarteIndizierung()
            Case BIndizierungAbbrechen.Name
                BWIndexer.CancelAsync()
                BIndizierungAbbrechen.Enabled = False
                BIndizierungStart.Enabled = True
            Case BXML.Name
                ' XML-Datei mit Systemstandard öffnen
                Process.Start(IO.Path.Combine(XMLData.POptionen.PArbeitsverzeichnis, PDfltConfig_FileName))
            Case BRWSTest.Name
                If IsNumeric(TBRWSTest.Text) Then

                    Using RWS As New Rückwärtssuche
                        Dim vCard As String = Await RWS.StartRWS(New Telefonnummer() With {.SetNummer = TBRWSTest.Text}, False)

                        If Not vCard.StartsWith(PDfltBegin_vCard) Then vCard = PRWSTestKeinEintrag

                        MsgBox(PRWSTest(TBRWSTest.Text, vCard), MsgBoxStyle.Information, "Test der Rückwärtssuche")
                    End Using
                End If
        End Select
    End Sub

    Private Sub LinkLogFile_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLogFile.LinkClicked
        Process.Start(IO.Path.Combine(XMLData.POptionen.PArbeitsverzeichnis, PDfltLog_FileName))
    End Sub

#End Region

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
                Case Me.CBoxMinLogLevel.Name
                    .SelectedItem = SelektiertesElement
            End Select
        End With
    End Sub

    Private Sub SetComboBox(ByVal CBox As ComboBox)
        With CBox
            .DataBindings.Clear()
            Select Case CBox.Name
                Case Me.CBoxAnrMonSimRINGEigTelNr.Name, Me.CBoxAnrMonSimCALLEigTelNr.Name
                    .DataSource = XMLData.PTelefonie.Telefonnummern
                    .DisplayMember = NameOf(Telefonnummer.Einwahl)
                    .ValueMember = NameOf(Telefonnummer.Einwahl)
                Case Me.CBoxAnrMonSimCALLNSTID.Name, CBoxAnrMonSimCONNECTNSTID.Name
                    .DataSource = XMLData.PTelefonie.Telefoniegeräte
                    .DisplayMember = NameOf(Telefoniegerät.Name)
                    .ValueMember = NameOf(Telefoniegerät.AnrMonID)
            End Select
        End With
    End Sub

    Friend Async Sub StarteEinlesen()
        If Ping(XMLData.POptionen.PValidFBAdr) Then
            If FritzBoxDaten Is Nothing Then FritzBoxDaten = New FritzBoxData
            Await FritzBoxDaten.FritzBoxDatenJSON
            ' Fülle das Datagridview

            SetTelDGV()
            SetCheckedListBox(Me.CLBTelNr)
        End If
    End Sub

    Private Sub SetTelDGV()

        With DGVTelList
            .EnableDoubleBuffered(True)
            With .Columns
                .Add(NewCheckBoxColumn("Std", "Std", "Std", True))
                .Add(NewTextColumn("Nr", "Nr.", "Nr", True, DataGridViewContentAlignment.MiddleRight, GetType(Integer), DataGridViewAutoSizeColumnMode.AllCells))
                .Add(NewTextColumn("ID", "Dialport", "ID", True, DataGridViewContentAlignment.MiddleRight, GetType(Integer), DataGridViewAutoSizeColumnMode.AllCells))
                .Add(NewTextColumn("AnrMonID", "Anrufmonitor ID", "AnrMonID", True, DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.AllCells))
                .Add(NewTextColumn("Name", "Telefonname", "Name", True, DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.Fill))
                .Add(NewTextColumn("ENummern", "Eingehende Nummern", "ENummern", True, DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.Fill))

            End With

            ' Datentabelle füllen
            .DataSource = New BindingSource With {.DataSource = ConvertToDataTable()}
            .Enabled = True
        End With

    End Sub

    Private Function ConvertToDataTable() As TelGeräteListDataTable
        Dim Datentabelle As New TelGeräteListDataTable
        Dim DatenZeile As TelGeräteListDataRow

        With Datentabelle.Columns
            .Add("Std", GetType(Boolean))
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
    Private Sub StarteIndizierung()
        BWIndexer = New BackgroundWorker

        ProgressBarIndex.Value = 0

        BIndizierungAbbrechen.Enabled = True
        BIndizierungStart.Enabled = False

        With BWIndexer
            .WorkerSupportsCancellation = True
            .WorkerReportsProgress = True
            .RunWorkerAsync()
        End With
    End Sub

    Private Async Sub BWIndexer_DoWork(sender As Object, e As DoWorkEventArgs) Handles BWIndexer.DoWork
        Dim MaxValue As Integer = Await ZähleOutlookKontakte()

        Dim Value As Integer = If(RadioButtonErstelle.Checked, 0, MaxValue)

        If InvokeRequired Then
            ' Set Maximum Value
            Dim D1 As New DelgSetProgressbarMax(AddressOf SetProgressbarMax)
            Invoke(D1, MaxValue)
            ' Set  Value
            Dim D2 As New DelgSetProgressbarMax(AddressOf SetProgressbar)
            Invoke(D2, Value)
        Else
            ' Set Maximum Value
            SetProgressbarMax(MaxValue)
            ' Set  Value
            SetProgressbar(Value)
        End If

        Using ki As New KontaktIndizierer
            KontaktIndexer(Nothing, ThisAddIn.POutookApplication.GetNamespace("MAPI"), ki, RadioButtonErstelle.Checked)
        End Using
    End Sub

    Private Sub KontaktIndexer(ByVal Ordner As Outlook.MAPIFolder, ByVal NamensRaum As Outlook.NameSpace, ByVal KI As KontaktIndizierer, ByVal Erstellen As Boolean)

        Dim iOrdner As Integer    ' Zählvariable für den aktuellen Ordner
        Dim aktKontakt As Outlook.ContactItem  ' aktueller Kontakt

        ' Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        If NamensRaum IsNot Nothing Then
            Dim j As Integer = 1
            Do While (j <= NamensRaum.Folders.Count)
                KontaktIndexer(CType(NamensRaum.Folders.Item(j), Outlook.MAPIFolder), Nothing, KI, Erstellen)
                j += 1
            Loop
        Else
            If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem And Not BWIndexer.CancellationPending Then
                For Each item In Ordner.Items
                    ' nur Kontakte werden durchsucht
                    If TypeOf item Is Outlook.ContactItem Then
                        aktKontakt = CType(item, Outlook.ContactItem)
                        If Erstellen Then
                            KI.IndiziereKontakt(aktKontakt)
                            BWIndexer.ReportProgress(1)
                        Else
                            KI.DeIndiziereKontakt(aktKontakt)
                            BWIndexer.ReportProgress(-1)
                        End If

                        aktKontakt.Save()

                        If BWIndexer.CancellationPending Then Exit For
                    Else
                        BWIndexer.ReportProgress(1)
                    End If
                Next

                If Not Erstellen Then
                    ' Entfernt alle Indizierungseinträge aus den Ordnern aus einem Kontaktelement.
                    KI.DeIndizierungOrdner(Ordner)
                End If
            End If

            ' Unterordner werden rekursiv durchsucht
            iOrdner = 1
            Do While (iOrdner.IsLessOrEqual(Ordner.Folders.Count)) And Not BWIndexer.CancellationPending
                KontaktIndexer(CType(Ordner.Folders.Item(iOrdner), Outlook.MAPIFolder), Nothing, KI, Erstellen)
                iOrdner += 1
            Loop
        End If

    End Sub

    Private Sub BWIndexer_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BWIndexer.ProgressChanged
        If InvokeRequired Then
            Dim D As New DelgSetProgressbarMax(AddressOf SetProgressbar)
            Invoke(D, e.ProgressPercentage)
        Else
            SetProgressbar(e.ProgressPercentage)
        End If
    End Sub

    Private Sub BWIndexer_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWIndexer.RunWorkerCompleted
        If InvokeRequired Then
            Dim D As New DelgSetProgressbarMax(AddressOf SetProgressbarMax)
            Invoke(D, ProgressBarIndex.Maximum)
        Else
            SetProgressbarMax(ProgressBarIndex.Maximum)
        End If
        BWIndexer.Dispose()
        NLogger.Info("{0}Indizierung abgeschlossen: {1} von {2} Kontakten.", If(RadioButtonEntfernen.Checked, "De-", ""), ProgressBarIndex.Value, ProgressBarIndex.Maximum)
    End Sub

    Private Sub SetProgressbar(ByVal Wert As Integer)
        ProgressBarIndex.Value += Wert
        LabelAnzahl.Text = String.Format("Status: {0}/{1}", ProgressBarIndex.Value, ProgressBarIndex.Maximum)
    End Sub

    Private Sub SetProgressbarMax(ByVal Anzahl As Integer)
        ProgressBarIndex.Maximum = Anzahl
        LabelAnzahl.Text = String.Format("Status: {0}/{1}", 0, Anzahl)
    End Sub
#End Region

#Region "TreeView Outlook Kontaktordner"
    Private Sub DelSetTreeView()
        If InvokeRequired Then
            Dim D As New DelgSetValue(AddressOf DelSetTreeView)
            Invoke(D)
        Else
            GetKontaktOrdnerInTreeView(TVOutlookContact)
        End If
    End Sub

    Private Sub BWTreeView_DoWork(sender As Object, e As DoWorkEventArgs) Handles BWTreeView.DoWork
        DelSetTreeView()
    End Sub

    Private Sub BWTreeView_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BWTreeView.RunWorkerCompleted
        With TVOutlookContact
            Dim tmpNode As TreeNode()
            tmpNode = .Nodes.Find(XMLData.POptionen.PTVKontaktOrdnerEntryID & ";" & XMLData.POptionen.PTVKontaktOrdnerStoreID, True)
            If Not tmpNode.Length = 0 Then
                .SelectedNode = tmpNode(0)
                .SelectedNode.Checked = True
            End If
            .ExpandAll()
            .Enabled = True
        End With
    End Sub

    Private Sub TVOutlookContact_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles TVOutlookContact.NodeMouseClick
        If e.Node.Checked Then
            With TVOutlookContact
                UnCheckAllNodes(TVOutlookContact.Nodes(0))
                If e.Node IsNot TVOutlookContact.Nodes(0) Then
                    e.Node.Checked = True
                End If
            End With
        End If
    End Sub

    Private Sub UnCheckAllNodes(ByVal TN As TreeNode)
        TN.Checked = False
        For Each sNode As TreeNode In TN.Nodes
            UnCheckAllNodes(sNode)
        Next
    End Sub

    Private Sub CheckedChanged(sender As Object, e As EventArgs) Handles CBTelNrGruppieren.CheckedChanged, CBCloseWClient.CheckedChanged, CBUseAnrMon.CheckedChanged
        Select Case CType(sender, CheckBox).Name
            Case CBTelNrGruppieren.Name
                TBTelNrMaske.Enabled = CBTelNrGruppieren.Checked
                LTelNrMaske.Enabled = CBTelNrGruppieren.Checked
            Case CBCloseWClient.Name
                TBWClientEnblDauer.Enabled = CBCloseWClient.Checked
                LWClientEnblDauer.Enabled = CBCloseWClient.Checked
            Case CBUseAnrMon.Name
                PanelAnrMon.Enabled = CBUseAnrMon.Checked
        End Select




    End Sub


#End Region
End Class


