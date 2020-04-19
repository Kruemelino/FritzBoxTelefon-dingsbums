Imports System.Windows.Forms
Imports System.Data
Imports System.ComponentModel

Public Class FormTelefonbücher
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        LadeTelefonbücher()


    End Sub

    Private Async Sub LadeTelefonbücher()
        If ThisAddIn.PPhoneBookXML Is Nothing Then ThisAddIn.PPhoneBookXML = Await LadeFritzBoxTelefonbücher()

        If ThisAddIn.PPhoneBookXML IsNot Nothing AndAlso ThisAddIn.PPhoneBookXML.Telefonbuch IsNot Nothing AndAlso ThisAddIn.PPhoneBookXML.Telefonbuch.Any Then
            For Each TelBuch In ThisAddIn.PPhoneBookXML.Telefonbuch
                LCTelefonbücher.AddTelefonbuch(TelBuch)
            Next
        End If
    End Sub

    Private Sub LCTelefonbücher_ItemClick(sender As Object, Index As Integer) Handles LCTelefonbücher.ItemClick
        ' DataGridView reseten
        With DGVTelBuchEinträge
            .DataSource = Nothing
            .Rows.Clear()
        End With

        ' Einträge des Telefonbuches in das DatagridView übertragen
        SetTelBuchDGV(CType(sender, TelBuchListControl).Telefonbuch)

    End Sub

#Region "DataGridView"
    Private Sub SetTelBuchDGV(ByVal Telefonbuch As FritzBoxXMLTelefonbuch)
        If Telefonbuch IsNot Nothing Then
            With DGVTelBuchEinträge

                If .Columns.Count.IsZero Then
                    ' Spalten hinzufügen
                    .AddHiddenTextColumn("uniqueid", "uniqueid", GetType(String))
                    .AddTextColumn("RealName", "Name", DataGridViewContentAlignment.MiddleLeft, GetType(String), DataGridViewAutoSizeColumnMode.Fill)
                    .AddTextColumn("Nummer", "Telefonnummer", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.Fill)
                    .AddTextColumn("Typ", "Typ", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.AllCells)
                End If

                ' Datenquelle generieren setzen
                .DataSource = New BindingSource With {.DataSource = New SortableBindingList(Of DGVDatenZeileTelbucheintrag)(ConvertToDataList(Telefonbuch.Kontakte))}
                .Enabled = True
            End With
        End If
    End Sub

    Private Function ConvertToDataList(ByVal Telefonbucheinträge As List(Of FritzBoxXMLKontakt)) As List(Of DGVDatenZeileTelbucheintrag)
        Dim DGVList As New List(Of DGVDatenZeileTelbucheintrag)()
        If Telefonbucheinträge.Any Then
            For Each Eintrag As FritzBoxXMLKontakt In Telefonbucheinträge
                DGVList.Add(New DGVDatenZeileTelbucheintrag With {.Uniqueid = Eintrag.Uniqueid,
                                                                  .RealName = Eintrag.Person.RealName,
                                                                  .Nummer = Eintrag.Telefonie.GetFirstNumber?.Nummer,
                                                                  .Typ = Eintrag.Telefonie.GetFirstNumber?.Typ,
                                                                  .Telefonbucheintrag = Eintrag
                                                                 })
            Next
        End If
        Return DGVList
    End Function

    Private Function ConvertToDataList(ByVal Telefonnummerneinträge As List(Of FritzBoxXMLNummer)) As List(Of DGVDatenZeileNummernEintrag)
        Dim DGVList As New List(Of DGVDatenZeileNummernEintrag)()
        If Telefonnummerneinträge.Any Then
            For Each Eintrag As FritzBoxXMLNummer In Telefonnummerneinträge
                DGVList.Add(New DGVDatenZeileNummernEintrag With {.Nummer = Eintrag.Nummer,
                                                                  .Typ = Eintrag.Typ,
                                                                  .Prio = If(Eintrag.Prio.IsNotStringEmpty, CBool(Eintrag.Prio), False),
                                                                  .Telefonbucheintrag = Eintrag})
                '.Schnellwahl = Eintrag.Schnellwahl,
                '                                                  .Vanity = Eintrag.Vanity,
            Next
        End If
        Return DGVList
    End Function

    Private Function ConvertToDataList(ByVal TelefonEMaileinträge As List(Of FritzBoxXMLEmail)) As List(Of DGVDatenZeileEMailEintrag)
        Dim DGVList As New List(Of DGVDatenZeileEMailEintrag)()
        If TelefonEMaileinträge.Any Then
            For Each Eintrag As FritzBoxXMLEmail In TelefonEMaileinträge
                DGVList.Add(New DGVDatenZeileEMailEintrag With {.EMail = Eintrag.EMail, .EMailEintrag = Eintrag})
            Next
        End If
        Return DGVList
    End Function

#Region "DGV Sortierung"
    Private Sub DGVColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DGVTelBuchEinträge.ColumnHeaderMouseClick

        Dim dGVSortOrder As ListSortDirection

        With CType(sender, FBoxDataGridView)

            If .SortedColumn Is Nothing Then
                ' DGV wurde noch nicht sortiert
                ' Sortierreihenfolge auf Ascending festlegen 
                dGVSortOrder = ListSortDirection.Ascending
            Else
                ' DGV ist sortiert
                If .Columns(e.ColumnIndex) Is .SortedColumn Then
                    ' Sortierreihenfolge drehen
                    dGVSortOrder = If(.SortOrder = SortOrder.Ascending, ListSortDirection.Descending, ListSortDirection.Ascending)
                Else
                    ' Sortierreihenfolge auf Ascending festlegen 
                    dGVSortOrder = ListSortDirection.Ascending
                    ' Alte Spalte zurücksetzen
                    .SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None
                End If
            End If
            .Columns(e.ColumnIndex).HeaderCell.SortGlyphDirection = CType(dGVSortOrder, SortOrder)
            .Sort(.Columns(e.ColumnIndex), dGVSortOrder)
        End With
    End Sub
#End Region
    Private Sub DGVTelBuchEinträge_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DGVTelBuchEinträge.DataBindingComplete
        For Each Spalte As DataGridViewColumn In CType(sender, FBoxDataGridView).Columns
            Spalte.SortMode = DataGridViewColumnSortMode.Programmatic
        Next
    End Sub

    Private Sub DGVTelBuchEinträge_SelectionChanged(sender As Object, e As EventArgs) Handles DGVTelBuchEinträge.SelectionChanged
        Dim SelektierteZeilen As DataGridViewSelectedRowCollection = CType(sender, FBoxDataGridView).SelectedRows

        If SelektierteZeilen.Count.IsPositive Then
            ' Für den ersten selektierten Eintrag die Details anzeigen
            Dim Telefonbucheintrag As DGVDatenZeileTelbucheintrag = CType(SelektierteZeilen.Item(0).DataBoundItem, DGVDatenZeileTelbucheintrag)
            If Telefonbucheintrag IsNot Nothing Then
                With Telefonbucheintrag
                    ' Name des Kontaktes anzeigen
                    TBName.Text = .RealName
                    ' Telefonnummern darstellen
                    SetNummernDGV(.Telefonbucheintrag.Telefonie)
                    ' E-Mails auflisten
                    SetEMailDGV(.Telefonbucheintrag.Telefonie.Dienste)
                End With
            End If
        End If
    End Sub

    Private Sub SetNummernDGV(ByVal TelefonbuchEintragTelefonie As FritzBoxXMLTelefonie)
        If TelefonbuchEintragTelefonie IsNot Nothing Then
            With DGVTelefonnummern
                If .Rows.Count.IsPositive Then .Rows.Clear()

                If .Columns.Count.IsZero Then
                    ' Spalten hinzufügen
                    .AddCheckBoxColumn("Prio", "Prio")
                    .AddEditTextColumn("Nummer", "Telefonnummer", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.Fill)
                    .AddComboBoxColumn("Typ", "Typ", FritzBoxDefault.PDfltTelBuchTelTyp, DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.Fill)
                    '.AddTextColumn("Vanity", "Vanity", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.AllCells)
                    '.AddTextColumn("Schnellwahl", "Schnellwahl", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.AllCells)
                End If

                ' Datenquelle generieren setzen
                .DataSource = New BindingSource With {.DataSource = New SortableBindingList(Of DGVDatenZeileNummernEintrag)(ConvertToDataList(TelefonbuchEintragTelefonie.Nummern))}
                .Enabled = True
                .ClearSelection()
            End With
        End If
    End Sub

    Private Sub SetEMailDGV(ByVal TelefonbuchEintragDienste As FritzBoxXMLServices)
        If TelefonbuchEintragDienste IsNot Nothing Then
            With DGVEMail
                If .Rows.Count.IsPositive Then .Rows.Clear()

                If .Columns.Count.IsZero Then
                    ' Spalten hinzufügen
                    .AddEditTextColumn("EMail", "E-Mail Adresse", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.Fill)
                End If

                ' Datenquelle generieren setzen
                .DataSource = New BindingSource With {.DataSource = New SortableBindingList(Of DGVDatenZeileEMailEintrag)(ConvertToDataList(TelefonbuchEintragDienste.Emails))}
                .Enabled = True
                .ClearSelection()
            End With
        End If
    End Sub

    Private Sub DGVEMail_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DGVEMail.CellEndEdit

        With CType(sender, FBoxDataGridView)
            .EndEdit()
            Dim Eintrag As DGVDatenZeileEMailEintrag = CType(.Rows(e.RowIndex).DataBoundItem, DGVDatenZeileEMailEintrag)
            If Eintrag IsNot Nothing Then
                ' Schleife durch die Eigenschaften
                For Each PropertyInfo As Reflection.PropertyInfo In Eintrag.EMailEintrag.GetType.GetProperties
                    If .Columns.Contains(PropertyInfo.Name) Then
                        PropertyInfo.SetValue(Eintrag.EMailEintrag, .Item(PropertyInfo.Name, e.RowIndex).Value.ToString)
                    Else
                        Stop
                    End If
                Next
            Else
                'neuer Eintrag?
            End If

        End With
    End Sub

    Private Sub DGVTelefonnummern_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DGVTelefonnummern.CellEndEdit

        With CType(sender, FBoxDataGridView)
            .EndEdit()
            Dim Eintrag As DGVDatenZeileNummernEintrag = CType(.Rows(e.RowIndex).DataBoundItem, DGVDatenZeileNummernEintrag)

            If Eintrag IsNot Nothing Then
                ' Schleife durch die Eigenschaften
                For Each PropertyInfo As Reflection.PropertyInfo In Eintrag.Telefonbucheintrag.GetType.GetProperties
                    If .Columns.Contains(PropertyInfo.Name) Then
                        PropertyInfo.SetValue(Eintrag.Telefonbucheintrag, .Item(PropertyInfo.Name, e.RowIndex).Value.ToString)
                    End If
                Next
            Else
                'neuer Eintrag?
            End If
        End With
    End Sub

#End Region


End Class