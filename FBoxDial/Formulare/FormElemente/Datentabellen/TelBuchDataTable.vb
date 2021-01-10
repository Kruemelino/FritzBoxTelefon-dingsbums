Imports System.Data
<Obsolete> Public Class TelBuchDataTable
    Inherits DataTable

    Friend Property Kontakt As FritzBoxXMLKontakt

    Protected Overrides Function NewRowFromBuilder(ByVal builder As DataRowBuilder) As DataRow
        Return New TelBuchDataRow(builder)
    End Function

    Default Public ReadOnly Property Rows_myObj(ByVal index As Integer) As TelBuchDataRow
        Get
            Return CType(Rows(index), TelBuchDataRow)
        End Get
    End Property
End Class
