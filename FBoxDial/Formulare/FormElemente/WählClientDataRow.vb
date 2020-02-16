Imports System.Data

Public Class WählClientDataRow
    Inherits DataRow

    Friend Property TelNr As Telefonnummer

    Public Sub New(ByVal rb As DataRowBuilder)
        MyBase.New(rb)
    End Sub
End Class

Public Class WählClientDataTable
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
