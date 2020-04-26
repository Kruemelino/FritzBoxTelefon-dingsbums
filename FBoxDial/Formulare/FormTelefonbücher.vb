Imports System.Windows.Forms
Imports System.Data
Imports System.ComponentModel

Public Class FormTelefonbücher
    Private Enum SubDGVTyp
        Kontakt = 0
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
                    .AddHiddenTextColumn("uniqueid", GetType(String))
                    .AddTextColumn("RealName", "Name", DataGridViewContentAlignment.MiddleLeft, GetType(String), DataGridViewAutoSizeColumnMode.Fill)
                    .AddTextColumn("Nummer", "Telefonnummer", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.Fill)
                    .AddTextColumn("Typ", "Typ", DataGridViewContentAlignment.MiddleRight, GetType(String), DataGridViewAutoSizeColumnMode.AllCells)
                    .AddImageColumn("Löschen", PDfltStringEmpty)
                End If

                ' Datenquelle generieren setzen
                .DataSource = New BindingSource With {.DataSource = ConvertToDataTable(Telefonbuch.Kontakte, SubDGVTyp.Kontakt)}
                .Enabled = True
            End With
        End If
    End Sub

    Private Function ConvertToDataTable(ByVal Telefonbucheinträge As List(Of FritzBoxXMLKontakt), ByVal Typ As SubDGVTyp) As TelBuchDataTable
        Dim DGVTabelle As New TelBuchDataTable
        Dim DatenZeile As TelBuchDataRow

        If Telefonbucheinträge.Any Then
            With DGVTabelle
                Select Case Typ
                    Case SubDGVTyp.Kontakt

                        ' Spalten zur Datentabelle hinzufügen 
                        .Columns.Add("uniqueid", GetType(String))
                        .Columns.Add("RealName", GetType(String))
                        .Columns.Add("Nummer", GetType(String))
                        .Columns.Add("Typ", GetType(String))
                        ' Zeilen hinzufügen
                        For Each Eintrag As FritzBoxXMLKontakt In Telefonbucheinträge
                            DatenZeile = CType(.Rows.Add(Eintrag.Uniqueid, Eintrag.Person.RealName, Eintrag.Telefonie.GetFirstNumber?.Nummer, Eintrag.Telefonie.GetFirstNumber?.Typ), TelBuchDataRow)
                            DatenZeile.FritzBoxKontakt = Eintrag
                        Next
                End Select

            End With
        End If
        Return DGVTabelle
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

    Private Sub DGVTelBuchEinträge_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DGVTelBuchEinträge.DataBindingComplete
        For Each Spalte As DataGridViewColumn In CType(sender, FBoxDataGridView).Columns
            Spalte.SortMode = DataGridViewColumnSortMode.Programmatic
        Next
    End Sub

    Private Sub DGVTelBuchEinträge_SelectionChanged(sender As Object, e As EventArgs) Handles DGVTelBuchEinträge.SelectionChanged
        Dim SelektierteZeilen As DataGridViewSelectedRowCollection = CType(sender, FBoxDataGridView).SelectedRows

        If SelektierteZeilen.Count.IsPositive Then
            ' Für den ersten selektierten Eintrag die Details anzeigen
            Dim DatenZeilenAnsicht As DataRowView = CType(SelektierteZeilen.Item(0).DataBoundItem, DataRowView)
            If DatenZeilenAnsicht IsNot Nothing Then
                ' Erhalte die Datentabelle
                Dim Datentabelle As TelBuchDataTable = CType(DatenZeilenAnsicht.DataView.Table, TelBuchDataTable)
                ' Erhalte die Datenzeile in der Datentabelle
                Dim Datenzeile As TelBuchDataRow = CType(DatenZeilenAnsicht.Row, TelBuchDataRow)

                If Datenzeile.FritzBoxKontakt IsNot Nothing Then

                    ' Name des Kontaktes anzeigen
                    TBName.Text = Datenzeile.FritzBoxKontakt.Person.RealName
                    ' Telefonnummern darstellen
                    SetWerteDGV(Datenzeile.FritzBoxKontakt, SubDGVTyp.Telefonnummern)
                    ' E-Mails auflisten
                    SetWerteDGV(Datenzeile.FritzBoxKontakt, SubDGVTyp.EMail)

                End If
            End If
        End If
    End Sub

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
                            .AddImageColumn("Löschen", PDfltStringEmpty)
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
                            .AddImageColumn("Löschen", PDfltStringEmpty)
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

        Dim DatenZeilenAnsicht As DataRowView = CType(dgvR.DataBoundItem, DataRowView)

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

    Private Sub DGV_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DGVTelefonnummern.CellFormatting, DGVEMail.CellFormatting, DGVTelBuchEinträge.CellFormatting
        With CType(sender, FBoxDataGridView)

            If e.RowIndex.IsLargerOrEqual(0) Then
                If Not .Rows(e.RowIndex).IsNewRow Then
                    If .Columns.Contains("Löschen") Then
                        If e.ColumnIndex.AreEqual(.Columns.Item("Löschen").Index) Then
                            e.Value = My.Resources.Cancel
                        End If
                    End If
                Else
                    e.Value = BlankImage()
                End If
                e.FormattingApplied = True
            End If
        End With
    End Sub

    Private Sub DGV_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DGVTelefonnummern.CellClick, DGVEMail.CellClick
        With CType(sender, FBoxDataGridView)

            If e.RowIndex.IsLargerOrEqual(0) And e.ColumnIndex.IsLargerOrEqual(0) And Not .Rows(e.RowIndex).IsNewRow Then

                Select Case .Columns(e.ColumnIndex).Name
                    Case "Löschen"
                        Dim dgvR As DataGridViewRow = .Rows(e.RowIndex)
                        Dim DatenZeilenAnsicht As DataRowView = CType(dgvR.DataBoundItem, DataRowView)

                        If DatenZeilenAnsicht IsNot Nothing Then
                            ' Erhalte die Datentabelle
                            Dim Datentabelle As TelBuchDataTable = CType(DatenZeilenAnsicht.DataView.Table, TelBuchDataTable)
                            ' Erhalte die Datenzeile in der Datentabelle
                            Dim Datenzeile As TelBuchDataRow = CType(DatenZeilenAnsicht.Row, TelBuchDataRow)

                            Select Case .Name
                                Case DGVTelefonnummern.Name
                                    ' Telefonnummer löschen
                                    Datentabelle.Kontakt.Telefonie.Nummern.Remove(Datenzeile.FritzBoxNummer)
                                Case DGVEMail.Name
                                    ' E-Mail löschen
                                    Datentabelle.Kontakt.Telefonie.Dienste.Emails.Remove(Datenzeile.FritzBoxEmail)

                            End Select
                            ' Zeile aus dem DatagridView entferen 
                            .Rows.Remove(dgvR)

                        End If

                        .EndEdit()
                End Select
            End If
        End With
    End Sub

#End Region


End Class