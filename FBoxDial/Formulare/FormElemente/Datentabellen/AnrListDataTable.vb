Imports System.Data

<Obsolete> Friend Class AnrListDataTable
    Inherits DataTable

    Protected Overrides Function NewRowFromBuilder(builder As DataRowBuilder) As DataRow
        Return New AnrListDataRow(builder)
    End Function

    Default Public ReadOnly Property Rows_myObj(index As Integer) As AnrListDataRow
        Get
            Return CType(Me.Rows(index), AnrListDataRow)
        End Get
    End Property
End Class