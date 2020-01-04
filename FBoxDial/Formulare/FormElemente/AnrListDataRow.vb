Imports System.Data

Public Class AnrListDataRow
    Inherits DataRow

    Friend Property AnrListCall As FritzBoxXMLCall

    Public Sub New(ByVal rb As DataRowBuilder)
        MyBase.New(rb)
    End Sub
End Class

Public Class AnrListDataTable
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