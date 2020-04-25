Imports System.Windows.Forms
Imports System.Data
Imports System.ComponentModel

Public Class FormTelefonbücher
    Private Enum SubDGVTyp
        Kontak = 0
        Telefonnummern = 1
        EMail = 2
    End Enum

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

    Private Function ConvertToDataTable(ByVal Telefonbucheintrag As FritzBoxXMLKontakt, ByVal Typ As SubDGVTyp) As TelBuchDataTable
        Dim DGVTabelle As New TelBuchDataTable
        Dim DatenZeile As TelBuchDataRow

        If Telefonbucheintrag IsNot Nothing Then
            With DGVTabelle
                .Kontakt = Telefonbucheintrag
                Select Case Typ
                    Case SubDGVTyp.Telefonnummern
                        ' Spalten zur Datentabelle hinzufügen 
                        .Columns.Add("Prio", GetType(Boolean))
                        .Columns.Add("Nummer", GetType(String))
                        .Columns.Add("Typ", GetType(String))
                        ' Zeilen hinzufügen
                        For Each Eintrag As FritzBoxXMLNummer In Telefonbucheintrag.Telefonie.Nummern
                            DatenZeile = CType(.Rows.Add(If(Eintrag.Prio.IsNotStringEmpty, CBool(Eintrag.Prio), False), Eintrag.Nummer, Eintrag.Typ), TelBuchDataRow)
                            DatenZeile.FritzBoxNummer = Eintrag
                            'DatenZeile.Eintrag = Telefonbucheintrag
                        Next
                    Case SubDGVTyp.EMail
                        ' Spalten zur Datentabelle hinzufügen 
                        .Columns.Add("EMail", GetType(String))
                        ' Zeilen hinzufügen
                        For Each Eintrag As FritzBoxXMLEmail In Telefonbucheintrag.Telefonie.Dienste.Emails
                            DatenZeile = CType(.Rows.Add(Eintrag.EMail), TelBuchDataRow)
                            DatenZeile.FritzBoxEmail = Eintrag
                        Next
                End Select

            End With
        End If
        Return DGVTabelle
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
                    SetWerteDGV(.Telefonbucheintrag, SubDGVTyp.Telefonnummern)
                    ' E-Mails auflisten
                    SetWerteDGV(.Telefonbucheintrag, SubDGVTyp.EMail)
                End With
            End If
        End If
    End Sub

    ' Private Sub SetNummernDGV(ByVal TelefonbuchEintragTelefonie As FritzBoxXMLTelefonie)
    Private Sub SetWerteDGV(ByVal TelefonbuchEintrag As FritzBoxXMLKontakt, ByVal Typ As SubDGVTyp)
        If TelefonbuchEintrag IsNot Nothing Then

            Select Case Typ
                Case SubDGVTyp.Telefonnummern
                    With DGVTelefonnummern
                        If .Columns.Count.IsZero Then
                            ' Spalten hinzufügen
                            .AddCheckBoxColumn("Prio", "Prio")
                            .AddEditTextColumn("Nummer", "Telefonnummer", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.Fill)
                            .AddComboBoxColumn("Typ", "Typ", FritzBoxDefault.PDfltTelBuchTelTyp, DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.Fill)
                            '.AddTextColumn("Vanity", "Vanity", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.AllCells)
                            '.AddTextColumn("Schnellwahl", "Schnellwahl", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.AllCells)
                        End If

                        ' Datenquelle generieren setzen
                        .DataSource = New BindingSource With {.DataSource = ConvertToDataTable(TelefonbuchEintrag, Typ)}

                        .Enabled = True
                        .ClearSelection()
                    End With
                Case SubDGVTyp.EMail
                    With DGVEMail

                        If .Columns.Count.IsZero Then
                            ' Spalten hinzufügen
                            .AddEditTextColumn("EMail", "E-Mail Adresse", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.Fill)
                        End If

                        ' Datenquelle generieren setzen
                        .DataSource = New BindingSource With {.DataSource = ConvertToDataTable(TelefonbuchEintrag, Typ)}
                        .Enabled = True
                        .ClearSelection()
                    End With
            End Select

        End If
    End Sub

    Private Sub DGV_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DGVTelefonnummern.CellEndEdit, DGVEMail.CellEndEdit


        Dim dgv As FBoxDataGridView = CType(sender, FBoxDataGridView)
        Dim dgvR As DataGridViewRow = dgv.Rows(e.RowIndex)
        Dim DatenZeilenAnsicht As DataRowView = CType(dgvR.DataBoundItem, DataRowView)        ' kann leer sein

        If DatenZeilenAnsicht IsNot Nothing Then
            ' Erhalte die Datentabelle
            Dim Datentabelle As TelBuchDataTable = CType(DatenZeilenAnsicht.DataView.Table, TelBuchDataTable)
            ' Erhalte die Datenzeile in der Datentabelle
            Dim Datenzeile As TelBuchDataRow = CType(DatenZeilenAnsicht.Row, TelBuchDataRow)
            ' Wenn es sich um eine neu hinzugefügte Zeile handelt, ist das Telefonnummern bzw. E-Mail -element nicht gesetzt
            Select Case dgv.Name
                Case DGVTelefonnummern.Name

                    If Datenzeile.FritzBoxNummer Is Nothing Then
                        ' Erstelle ein neues Telefonnummernelement
                        Datenzeile.FritzBoxNummer = New FritzBoxXMLNummer
                        ' Prüfe ob der hintelegte Kontakt eine Liste von Telefonnummern hat
                        If Datentabelle.Kontakt.Telefonie.Nummern Is Nothing Then Datentabelle.Kontakt.Telefonie.Nummern = New List(Of FritzBoxXMLNummer)
                        ' Füge die Telefonnummer dem hinterlegten Kontakt hinzu
                        Datentabelle.Kontakt.Telefonie.Nummern.Add(Datenzeile.FritzBoxNummer)
                    End If

                    ' Schleife durch die Eigenschaften um Felder zu setzen
                    Try
                        For Each PropertyInfo As Reflection.PropertyInfo In Datenzeile.FritzBoxNummer.GetType.GetProperties
                            If dgv.Columns.Contains(PropertyInfo.Name) Then
                                PropertyInfo.SetValue(Datenzeile.FritzBoxNummer, dgv.Item(PropertyInfo.Name, e.RowIndex).Value.ToString)
                            End If
                        Next
                    Catch ex As Exception
                        Stop
                    End Try

                Case DGVEMail.Name
                    If Datenzeile.FritzBoxEmail Is Nothing Then
                        ' Erstelle eine neue E-Mail Adressen
                        Datenzeile.FritzBoxEmail = New FritzBoxXMLEmail
                        ' Prüfe ob der hintelegte Kontakt eine Liste von E-Mail Adressen hat
                        If Datentabelle.Kontakt.Telefonie.Dienste.Emails Is Nothing Then Datentabelle.Kontakt.Telefonie.Dienste.Emails = New List(Of FritzBoxXMLEmail)
                        ' Füge die Telefonnummer dem hinterlegten Kontakt hinzu
                        Datentabelle.Kontakt.Telefonie.Dienste.Emails.Add(Datenzeile.FritzBoxEmail)
                    End If

                    ' Schleife durch die Eigenschaften um Felder zu setzen
                    Try
                        For Each PropertyInfo As Reflection.PropertyInfo In Datenzeile.FritzBoxEmail.GetType.GetProperties
                            If dgv.Columns.Contains(PropertyInfo.Name) Then
                                PropertyInfo.SetValue(Datenzeile.FritzBoxEmail, dgv.Item(PropertyInfo.Name, e.RowIndex).Value.ToString)
                            End If
                        Next
                    Catch ex As Exception
                        Stop
                    End Try
            End Select

        End If
        dgv.EndEdit()
    End Sub

    Private Sub DGVTelefonnummern_DefaultValuesNeeded(sender As Object, e As DataGridViewRowEventArgs) Handles DGVTelefonnummern.DefaultValuesNeeded
        With CType(sender, FBoxDataGridView)
            If .Columns.Contains("Typ") Then
                e.Row.Cells("Typ").Value = FritzBoxDefault.PDfltTelBuchTelTyp.First.Key
            End If
        End With
    End Sub


#End Region


End Class