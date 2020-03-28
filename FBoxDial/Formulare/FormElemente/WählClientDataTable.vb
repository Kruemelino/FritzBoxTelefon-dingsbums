Imports System.Data

Friend Class WählClientDataTable
    Inherits DataTable

    Protected Overrides Function NewRowFromBuilder(ByVal builder As DataRowBuilder) As DataRow
        Return New WählClientDataRow(builder)
    End Function

    Default Public ReadOnly Property RowsmyObj(ByVal index As Integer) As WählClientDataRow
        Get
            Return CType(Me.Rows(index), WählClientDataRow)
        End Get
    End Property
End Class
