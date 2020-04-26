Imports System.ComponentModel
Imports System.Data
Imports System.Reflection
Imports System.Windows.Forms

Public Class FBoxDataGridView
    Inherits DataGridView
    Private Property ScaleFaktor As Drawing.SizeF
    Private Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
    Private Property Sortierung As SortOrder
    Public Sub New()
        ' Double Buffered einschalten
        [GetType].GetProperty("DoubleBuffered", BindingFlags.Instance Or BindingFlags.NonPublic).SetValue(Me, True, Nothing)
        ' Scaling ermitteln
        ScaleFaktor = GetScaling()
        ' Sortierung initial festlegen
        Sortierung = SortOrder.None
    End Sub

#Region "Spalten"
    Friend Overloads Sub AddTextColumn(ByVal Name As String, ByVal HeaderText As String, ByVal CellAlignment As DataGridViewContentAlignment, ByVal ValueType As Type, ByVal AutoSizeMode As DataGridViewAutoSizeColumnMode)
        Dim NewTextColumn As New DataGridViewTextBoxColumn With {.Name = Name,
                                                                 .HeaderText = HeaderText,
                                                                 .DataPropertyName = Name,
                                                                 .ValueType = ValueType,
                                                                 .AutoSizeMode = AutoSizeMode,
                                                                 .ReadOnly = True
                                                                }

        With NewTextColumn
            .DefaultCellStyle.Alignment = CellAlignment
            .HeaderCell.Style.Alignment = CellAlignment
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
        End With

        Columns.Add(NewTextColumn)
    End Sub
    Friend Overloads Sub AddEditTextColumn(ByVal Name As String, ByVal HeaderText As String, ByVal CellAlignment As DataGridViewContentAlignment, ByVal ValueType As Type, ByVal AutoSizeMode As DataGridViewAutoSizeColumnMode)
        Dim NewTextColumn As New DataGridViewTextBoxColumn With {.Name = Name,
                                                                 .HeaderText = HeaderText,
                                                                 .DataPropertyName = Name,
                                                                 .ValueType = ValueType,
                                                                 .AutoSizeMode = AutoSizeMode,
                                                                 .ReadOnly = False
                                                                }

        With NewTextColumn
            .DefaultCellStyle.Alignment = CellAlignment
            .HeaderCell.Style.Alignment = CellAlignment
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
        End With

        Columns.Add(NewTextColumn)
    End Sub

    Friend Overloads Sub AddTextColumn(ByVal Name As String, ByVal HeaderText As String, ByVal CellAlignment As DataGridViewContentAlignment, ByVal ValueType As Type, ByVal Width As Integer)
        Dim NewTextColumn As New DataGridViewTextBoxColumn With {.Name = Name,
                                                                 .HeaderText = HeaderText,
                                                                 .DataPropertyName = Name,
                                                                 .ValueType = ValueType,
                                                                 .Width = CInt(Width * ScaleFaktor.Width),
                                                                 .ReadOnly = True
                                                                }

        With NewTextColumn
            .DefaultCellStyle.Alignment = CellAlignment
            .HeaderCell.Style.Alignment = CellAlignment
        End With

        Columns.Add(NewTextColumn)
    End Sub


    Friend Sub AddHiddenTextColumn(ByVal Name As String, ByVal ValueType As Type)
        Dim NewHiddenTextColumn As New DataGridViewTextBoxColumn With {.Name = Name,
                                                                       .DataPropertyName = Name,
                                                                       .Visible = False,
                                                                       .ValueType = ValueType,
                                                                       .ReadOnly = True
                                                                      }

        Columns.Add(NewHiddenTextColumn)
    End Sub

    Friend Sub AddCheckBoxColumn(ByVal Name As String, ByVal HeaderText As String)
        Dim NewCheckBoxColumn As New DataGridViewCheckBoxColumn With {.Name = Name,
                                                                      .HeaderText = HeaderText,
                                                                      .DataPropertyName = Name,
                                                                      .TrueValue = True,
                                                                      .FalseValue = False,
                                                                      .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                                                                     }

        Columns.Add(NewCheckBoxColumn)
    End Sub

    Friend Overloads Sub AddComboBoxColumn(ByVal Name As String, ByVal HeaderText As String, ByVal Einträge As List(Of KeyValuePair(Of String, String)), ByVal CellAlignment As DataGridViewContentAlignment, ByVal ValueType As Type, ByVal AutoSizeMode As DataGridViewAutoSizeColumnMode)
        Dim NewComboBoxColumn As New DataGridViewComboBoxColumn With {.Name = Name,
                                                                 .HeaderText = HeaderText,
                                                                 .DataPropertyName = Name,
                                                                 .ValueType = ValueType,
                                                                 .AutoSizeMode = AutoSizeMode,
                                                                 .ReadOnly = False
                                                                }

        With NewComboBoxColumn
            .DefaultCellStyle.Alignment = CellAlignment
            .HeaderCell.Style.Alignment = CellAlignment
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
            .DataSource = Einträge
            .ValueMember = "Key"
            .DisplayMember = "Value"
        End With

        Columns.Add(NewComboBoxColumn)
    End Sub

    Friend Sub AddImageColumn(ByVal Name As String, ByVal HeaderText As String)
        Dim NewImageColumn As New DataGridViewImageColumn With {.Name = Name,
                                                                .HeaderText = HeaderText,
                                                                .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                                                               }

        With NewImageColumn
            .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            .DefaultCellStyle.WrapMode = DataGridViewTriState.True
        End With
        Columns.Add(NewImageColumn)
    End Sub

#End Region
#Region "Sortierung"
    Protected Overrides Sub OnColumnHeaderMouseClick(ByVal e As DataGridViewCellMouseEventArgs)
        Dim dGVSortOrder As ListSortDirection

        If SortedColumn Is Nothing Then
            ' DGV wurde noch nicht sortiert
            ' Sortierreihenfolge auf Ascending festlegen 
            dGVSortOrder = ListSortDirection.Ascending
        Else
            ' DGV ist sortiert
            If Columns(e.ColumnIndex) Is SortedColumn Then
                ' Sortierreihenfolge drehen
                dGVSortOrder = If(SortOrder = SortOrder.Ascending, ListSortDirection.Descending, ListSortDirection.Ascending)
            Else
                ' Sortierreihenfolge auf Ascending festlegen 
                dGVSortOrder = ListSortDirection.Ascending
                ' Alte Spalte zurücksetzen
                SortedColumn.HeaderCell.SortGlyphDirection = SortOrder.None
            End If
            End If
        Columns(e.ColumnIndex).HeaderCell.SortGlyphDirection = CType(dGVSortOrder, SortOrder)
        Sort(Columns(e.ColumnIndex), dGVSortOrder)

    End Sub
#End Region

    Private Sub DGVAnrListe_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles Me.CellPainting
        'Dim dgv As DataGridView = TryCast(sender, DataGridView)

        If Me IsNot Nothing AndAlso e.RowIndex.IsLargerOrEqual(0) Then
            ' Prüfe, ob es eine Check-Spalte gibt.
            If Columns.Contains("Check") Then
                Dim dgvRow As DataGridViewRow = Rows(e.RowIndex)
                ' Zeilen, die eine Checkbox haben (Name "Check") sollen farbig hinterlegt werden.
                If CType(dgvRow.Cells.Item("Check").Value, Boolean) Then
                    dgvRow.DefaultCellStyle.BackColor = PDfltCheckBackColor
                Else
                    dgvRow.DefaultCellStyle.BackColor = DefaultBackColor
                End If
            End If
        End If

    End Sub

    Private Sub DGVAnrListe_ColumnAdded(sender As Object, e As DataGridViewColumnEventArgs) Handles Me.ColumnAdded
        e.Column.SortMode = DataGridViewColumnSortMode.NotSortable
    End Sub

    Private Sub DGVAnrListe_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles Me.DataError
        NLogger.Error(e.Exception)
    End Sub

    ' https://stackoverflow.com/questions/11843488/how-to-detect-datagridview-checkbox-event-change
    Private Sub FBoxDataGridView_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellValueChanged

        If Me IsNot Nothing AndAlso e.RowIndex.IsLargerOrEqual(0) Then
            ' Prüfe, ob Mehrfachauswahl möglich ist und auch ob es eine Check-Spalte gibt 
            If Not MultiSelect AndAlso Columns.Contains("Check") AndAlso Columns.Contains("ID") AndAlso e.ColumnIndex.AreEqual(Columns.Item("Check").Index) Then
                Dim dgvRow As DataGridViewRow = Rows(e.RowIndex)
                If CType(dgvRow.Cells.Item("Check").Value, Boolean) Then
                    ' Alle anderen Zeilen deselektieren (dürfte hier nur eine sein, da kein Multiselect)

                    Dim DatenZeilen As List(Of DataRow)
                    Dim Abfrage As ParallelQuery(Of DataRow)

                    ' ID Merken: anhand dieser wird gemerkt, welche Zeile der Nutzer angeklickt hat. Ansonsten wird sie gleich darauf wieder abgehakt.
                    Dim SelID As Integer = CType(dgvRow.Cells.Item("ID").Value, Integer)

                    DatenZeilen = CType(CType(DataSource, BindingSource).DataSource, DataTable).Rows.Cast(Of DataRow)().ToList()

                    Abfrage = From Datenreihe In DatenZeilen.AsParallel() Where Datenreihe.Field(Of Boolean)("Check") And Datenreihe.Field(Of Integer)("ID").AreDifferent(SelID) Select Datenreihe
                    Abfrage.ForAll(Sub(r) r.SetField("Check", False))
                End If
            End If
        End If
    End Sub

    Private Sub FBoxDataGridView_CellMouseUp(sender As Object, e As DataGridViewCellMouseEventArgs) Handles Me.CellMouseUp
        If Me IsNot Nothing AndAlso e.RowIndex.IsLargerOrEqual(0) Then
            ' Prüfe, ob Mehrfachauswahl möglich ist und auch ob es eine Check-Spalte gibt 
            If Not MultiSelect AndAlso Columns.Contains("Check") AndAlso e.ColumnIndex.AreEqual(Columns.Item("Check").Index) Then
                EndEdit()
            End If
        End If
    End Sub

End Class
