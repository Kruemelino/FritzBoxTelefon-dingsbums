Imports System.Data

Friend Class AnrListDataTable
    Inherits DataTable

    Protected Overrides Function NewRowFromBuilder(ByVal builder As DataRowBuilder) As DataRow
        Return New AnrListDataRow(builder)
    End Function

    Default Public ReadOnly Property Rows_myObj(ByVal index As Integer) As AnrListDataRow
        Get
            Return CType(Me.Rows(index), AnrListDataRow)
        End Get
    End Property
End Class