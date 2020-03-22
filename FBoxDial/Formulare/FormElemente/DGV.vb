Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms

Friend Module DGV

    <Extension> Friend Sub EnableDoubleBuffered(ByVal dgv As DataGridView, ByVal Einschalten As Boolean)
        dgv.[GetType].GetProperty("DoubleBuffered", BindingFlags.Instance Or BindingFlags.NonPublic).SetValue(dgv, Einschalten, Nothing)
    End Sub

    Friend Function NewTextColumn(ByVal Name As String, ByVal HeaderText As String, ByVal DataPropertyName As String, ByVal Visible As Boolean, ByVal CellAlignment As DataGridViewContentAlignment, ByVal ValueType As Type, ByVal AutoSizeMode As DataGridViewAutoSizeColumnMode) As DataGridViewTextBoxColumn
        NewTextColumn = New DataGridViewTextBoxColumn With {.Name = Name,
                                                            .HeaderText = HeaderText,
                                                            .DataPropertyName = DataPropertyName,
                                                            .Visible = Visible,
                                                            .ValueType = ValueType,
                                                            .AutoSizeMode = AutoSizeMode
                                                           }
        With NewTextColumn
            .DefaultCellStyle.Alignment = CellAlignment

            .HeaderCell.Style.Alignment = CellAlignment
        End With
    End Function

    Friend Function NewCheckBoxColumn(ByVal Name As String, ByVal HeaderText As String, ByVal DataPropertyName As String, ByVal Visible As Boolean) As DataGridViewCheckBoxColumn
        NewCheckBoxColumn = New DataGridViewCheckBoxColumn With {.Name = Name,
                                                                 .HeaderText = HeaderText,
                                                                 .Visible = Visible,
                                                                 .DataPropertyName = DataPropertyName,
                                                                 .TrueValue = True,
                                                                 .FalseValue = False,
                                                                 .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                                                                }
    End Function

    Friend Function NewImageColumn(ByVal Name As String, ByVal HeaderText As String, ByVal Visible As Boolean) As DataGridViewImageColumn
        NewImageColumn = New DataGridViewImageColumn With {.Name = Name,
                                                           .HeaderText = HeaderText,
                                                           .Visible = Visible,
                                                           .AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                                                          }
    End Function
End Module
