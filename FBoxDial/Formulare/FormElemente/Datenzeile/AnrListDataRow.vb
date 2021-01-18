Imports System.Data

<Obsolete> Friend Class AnrListDataRow
    Inherits DataRow

    Friend Property AnrListCall As FritzBoxXMLCall

    Public Sub New(rb As DataRowBuilder)
        MyBase.New(rb)
    End Sub
End Class


