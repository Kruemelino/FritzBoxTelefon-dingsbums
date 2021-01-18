Imports System.Data

<Obsolete> Public Class TelGeräteListDataTable
    Inherits DataTable

    Protected Overrides Function NewRowFromBuilder(builder As DataRowBuilder) As DataRow
        Return New TelGeräteListDataRow(builder)
    End Function

    Default Public ReadOnly Property RowsmyObj(index As Integer) As TelGeräteListDataRow
        Get
            Return CType(Me.Rows(index), TelGeräteListDataRow)
        End Get
    End Property
End Class
