Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Reflection
Imports System.Threading.Tasks
Imports System.Windows.Forms

Public Class FormAnrList
    Implements IDisposable

    Private Property IList As ImageList
    Private Property Anrufliste As FritzBoxXMLCallList
    Private Property Source As BindingSource

    Private Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
    Private WithEvents BWImport As BackgroundWorker

#Region "Delegaten"
    Private Delegate Sub DelgSetProgressbar(ByVal Anzahl As Integer)
    Private Delegate Sub DelgSetFrei(ByVal Freigabe As Boolean)
#End Region
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        ' Setze den Startzeitpunkt mit der Schließzeit des Addins
        If XMLData.POptionen.PLetzterJournalEintrag > StartDatum.MinDate AndAlso XMLData.POptionen.PLetzterJournalEintrag < StartDatum.MaxDate Then
            StartDatum.Value = XMLData.POptionen.PLetzterJournalEintrag
            StartZeit.Value = XMLData.POptionen.PLetzterJournalEintrag
        Else
            StartDatum.Value = Now
            StartZeit.Value = Now
        End If

        ' Setze den Endzeitpunkt mit jetzigen Zeit
        EndDatum.Value = Now
        EndZeit.Value = Now

        ' Initialisiere die Image List
        If IList Is Nothing Then IList = New ImageList
        With IList.Images
            .Add("1", My.Resources.call_received) ' incoming
            .Add("2", My.Resources.call_missed)   ' missed
            .Add("3", My.Resources.call_made)     ' outgoing
            .Add("9", My.Resources.call_received) ' active incoming
            .Add("10", My.Resources.call_missed)  ' rejected incoming
            .Add("11", My.Resources.call_made)    ' active outgoing
        End With

        ' Initialisiere das DGV
        InitDGV()
    End Sub

#Region "DataGridView"
    Private Async Sub InitDGV()
        Anrufliste = Await LadeFritzBoxAnrufliste()
        SetTelDGV(Anrufliste)
    End Sub

    Private Sub SetTelDGV(ByVal Anrufliste As FritzBoxXMLCallList)
        If Anrufliste IsNot Nothing Then
            With DGVAnrListe
                .AddCheckBoxColumn("Check", "*")
                .AddImageColumn("Image", PDfltStringEmpty)
                .AddHiddenTextColumn("ID", GetType(Integer))
                .AddHiddenTextColumn("Type", GetType(String))
                .AddTextColumn("Datum", "Datum", DataGridViewContentAlignment.MiddleLeft, GetType(String), DataGridViewAutoSizeColumnMode.AllCells)
                .AddTextColumn("Name", "Name", DataGridViewContentAlignment.MiddleLeft, GetType(String), DataGridViewAutoSizeColumnMode.Fill)
                .AddTextColumn("EigeneNummer", "Eigene Nr.", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.Fill)
                .AddTextColumn("Gegenstelle", "Gegenstelle", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.Fill)
                .AddTextColumn("Duration", "Dauer", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.AllCells)
                .AddTextColumn("Device", "Gerät", DataGridViewContentAlignment.MiddleLeft, GetType(String), DataGridViewAutoSizeColumnMode.AllCells)
                .AddTextColumn("Port", "Port", DataGridViewContentAlignment.MiddleRight, GetType(Integer), DataGridViewAutoSizeColumnMode.AllCells)

                ' Datentabelle füllen
                Source = New BindingSource With {.DataSource = ConvertToDataTable(Anrufliste.Calls)}
                .DataSource = Source
                .Enabled = True
            End With
        End If
    End Sub

    Private Function ConvertToDataTable(ByVal Anrufliste As List(Of FritzBoxXMLCall)) As DataTable
        Dim Datentabelle As New AnrListDataTable()
        Dim Datenfelder As List(Of PropertyInfo)

        Dim Datenzeile As AnrListDataRow
        Dim PropInfo As PropertyInfo

        ' Überprüfung, ob Einträge übergeben wurden
        If Anrufliste.Any Then
            ' Spalten hinzufügen. Hier wird auch die Reihenfolge festgelegt, wie sie im DataGridView erscheinen
            With Datentabelle.Columns
                .Add("Check", GetType(Boolean))
                .Add("ID", GetType(String))
                .Item("ID").Unique = False
                .Add("Type", GetType(String))
                .Add("Datum", GetType(Date))
                .Add("Name", GetType(String))
                .Add("EigeneNummer", GetType(String))
                .Add("Gegenstelle", GetType(String))
                .Add("Duration", GetType(TimeSpan))
                .Add("Device", GetType(String))
                .Add("Port", GetType(String))
            End With

            ' Primary Key setzen (Zum Suchen in der Datatable)
            Datentabelle.PrimaryKey = {Datentabelle.Columns.Item("ID")}
            Datenfelder = Anrufliste.First.GetType.GetProperties.ToList
            ' Zeilen hinzufügen
            For Each Eintrage As FritzBoxXMLCall In Anrufliste
                Datenzeile = CType(Datentabelle.NewRow(), AnrListDataRow)

                Datenzeile.AnrListCall = Eintrage
                For Each Datenfeld As PropertyInfo In Datenfelder
                    PropInfo = Eintrage.GetType.GetProperty(Datenfeld.Name)
                    Select Case Datenfeld.Name
                        Case "Check", "Datum", "Duration", "Type", "ID", "Name", "Device", "EigeneNummer", "Gegenstelle", "Port"
                            Datenzeile(Datenfeld.Name) = PropInfo.GetValue(Eintrage)
                        Case Else
                            ' keine Daten hinzufügen
                    End Select

                Next
                Datentabelle.Rows.Add(Datenzeile)
            Next
        End If
        Return Datentabelle
    End Function

    Private Sub DGVAnrListe_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DGVAnrListe.DataBindingComplete
        Dim dgv As DataGridView = TryCast(sender, FBoxDataGridView)

        If dgv IsNot Nothing Then
            Freischalten(True)
            dgv.ClearSelection()
        End If
    End Sub

    Private Sub DGVAnrListe_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DGVAnrListe.CellFormatting

        With CType(sender, FBoxDataGridView)

            If e.RowIndex.IsLargerOrEqual(0) Then

                If .Columns.Contains("Image") Then
                    If e.ColumnIndex.AreEqual(.Columns.Item("Image").Index) Then
                        e.Value = IList.Images(.Rows.Item(e.RowIndex).Cells.Item("Type").Value.ToString)
                    End If
                End If

                If .Columns.Contains("Datum") Then
                    If e.ColumnIndex.AreEqual(.Columns.Item("Datum").Index) Then
                        e.Value = CDate(e.Value).ToString("dd.MM.yy HH:mm")
                    End If
                End If

                If .Columns.Contains("Duration") Then
                    If e.ColumnIndex.AreEqual(.Columns.Item("Duration").Index) Then
                        e.Value = CType(e.Value, TimeSpan).ToString("hh\:mm")
                    End If
                End If

                If .Columns.Contains("Check") Then
                    If e.ColumnIndex.AreEqual(.Columns.Item("Check").Index) Then
                        e.Value = CBool(e.Value)
                    End If
                End If

                e.FormattingApplied = True
            End If
        End With
    End Sub

#End Region
    Sub Datum_ValueChanged(sender As Object, e As EventArgs) Handles StartDatum.ValueChanged, EndDatum.ValueChanged, StartZeit.ValueChanged, EndZeit.ValueChanged
        Dim unused = Datum_ValueChangedAsync()
    End Sub

    Private Async Function Datum_ValueChangedAsync() As Task

        If Anrufliste IsNot Nothing Then
            Dim DatumZeitAnfang As Date
            Dim DatumZeitEnde As Date

            DatumZeitAnfang = StartDatum.Value.Date.AddHours(StartZeit.Value.Hour).AddMinutes(StartZeit.Value.Minute).AddSeconds(StartZeit.Value.Second)
            DatumZeitEnde = EndDatum.Value.Date.AddHours(EndZeit.Value.Hour).AddMinutes(EndZeit.Value.Minute).AddSeconds(EndZeit.Value.Second)

            If DatumZeitAnfang < DatumZeitEnde Then
                DGVAnrListe.SuspendLayout()
                Await CheckRows(DatumZeitAnfang, DatumZeitEnde)
                DGVAnrListe.Update()
                DGVAnrListe.ResumeLayout()
            End If
        End If

    End Function

    Private Function CheckRows(ByVal DatumZeitAnfang As Date, ByVal DatumZeitEnde As Date) As Task
        Return Task.Run(Sub()
                            Dim DatenZeilen As List(Of AnrListDataRow)
                            Dim Abfrage As ParallelQuery(Of AnrListDataRow)

                            DatenZeilen = CType(Source.DataSource, DataTable).Rows.Cast(Of AnrListDataRow)().ToList()

                            ' Alle Unchecken, welche Außerhalb der Zeiten liegen
                            Abfrage = From Datenreihe In DatenZeilen.AsParallel() Where Datenreihe.Field(Of Boolean)("Check") And (DatumZeitAnfang > Datenreihe.Field(Of Date)("Datum") Or DatumZeitEnde < Datenreihe.Field(Of Date)("Datum")) Select Datenreihe
                            Abfrage.ForAll(Sub(r) r.SetField("Check", False))

                            ' Jetzt alle Markieren, die innerhalb liegen
                            Abfrage = From Datenreihe In DatenZeilen.AsParallel() Where Not Datenreihe.Field(Of Boolean)("Check") And (DatumZeitAnfang <= Datenreihe.Field(Of Date)("Datum") And DatumZeitEnde >= Datenreihe.Field(Of Date)("Datum")) Select Datenreihe
                            Abfrage.ForAll(Sub(r) r.SetField("Check", True))

                        End Sub)
    End Function

    Private Sub ButtonStart_Click(sender As Object, e As EventArgs) Handles ButtonStart.Click
        BWImport = New BackgroundWorker

        Dim DatenZeilen As List(Of AnrListDataRow)
        Dim Abfrage As IEnumerable(Of AnrListDataRow)

        DatenZeilen = CType(Source.DataSource, DataTable).Rows.Cast(Of AnrListDataRow)().ToList()

        Abfrage = From Datenreihe In DatenZeilen.AsEnumerable() Where Datenreihe.Field(Of Boolean)("Check") Select Datenreihe

        ' Form-Elemente Deaktivieren
        Freischalten(False)

        ' Progressbar initialisieren
        With ProgressBarAnrListe
            .Value = 0
            .Minimum = 0
            .Maximum = Abfrage.Count
        End With

        With BWImport
            ' Abbruch-Eigenschaft setzen

            .WorkerReportsProgress = True
            .WorkerSupportsCancellation = True
            .RunWorkerAsync(Abfrage)
        End With
    End Sub

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        If BWImport IsNot Nothing AndAlso BWImport.IsBusy Then BWImport.CancelAsync()
    End Sub

    Private Sub Freischalten(ByVal Freigabe As Boolean)
        ' Form-Elemente Deaktivieren
        DGVAnrListe.Enabled = Freigabe
        GBoxStartZeit.Enabled = Freigabe
        GBoxEndZeit.Enabled = Freigabe
        ButtonStart.Enabled = Freigabe

        ButtonCancel.Enabled = Not Freigabe
    End Sub

#Region "Backgroundworkter - Import"
    Private Sub BWImport_DoWork(sender As Object, e As DoWorkEventArgs) Handles BWImport.DoWork

        For Each Datensatz As AnrListDataRow In CType(e.Argument, IEnumerable(Of AnrListDataRow))

            ' Datensatz in ein Telefonat wandeln und nachfolgend den Journaleintrag erstellen
            Datensatz.AnrListCall.ErstelleTelefonat.ErstelleJournalEintrag()

            ' Progressbar aktualisieren
            BWImport.ReportProgress(Datensatz.AnrListCall.ID)

            If BWImport.CancellationPending Then Exit For
        Next

    End Sub

    Private Sub BWImport_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BWImport.ProgressChanged
        If InvokeRequired Then
            Invoke(New DelgSetProgressbar(AddressOf SetProgressbar), 1)
        Else
            SetProgressbar(1)
        End If
    End Sub

    Private Sub SetProgressbar(ByVal Wert As Integer)
        If ProgressBarAnrListe.Value.IsLess(ProgressBarAnrListe.Maximum) Then
            ProgressBarAnrListe.Value += Wert
        End If
    End Sub

    Private Sub BWImport_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BWImport.RunWorkerCompleted
        If InvokeRequired Then
            If Not BWImport.CancellationPending Then
                Invoke(New DelgSetProgressbar(AddressOf SetProgressbar), ProgressBarAnrListe.Maximum - ProgressBarAnrListe.Value)
                Invoke(New DelgSetFrei(AddressOf Freischalten), True)
            End If
        Else
            SetProgressbar(ProgressBarAnrListe.Maximum - ProgressBarAnrListe.Value)
            Freischalten(True)
        End If
    End Sub
#End Region

End Class