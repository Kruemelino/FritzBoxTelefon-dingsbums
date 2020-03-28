Imports System.Data

Public Class TelGeräteListDataTable
    Inherits DataTable

    Protected Overrides Function NewRowFromBuilder(ByVal builder As DataRowBuilder) As DataRow
        Return New TelGeräteListDataRow(builder)
    End Function

    Default Public ReadOnly Property RowsmyObj(ByVal index As Integer) As TelGeräteListDataRow
        Get
            Return CType(Me.Rows(index), TelGeräteListDataRow)
        End Get
    End Property
End Class
