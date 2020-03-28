Imports System.Data

Friend Class AnrListDataRow
    Inherits DataRow

    Friend Property AnrListCall As FritzBoxXMLCall

    Public Sub New(ByVal rb As DataRowBuilder)
        MyBase.New(rb)
    End Sub
End Class


