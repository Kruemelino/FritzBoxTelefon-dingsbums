Imports System.Data

<Obsolete> Public Class TelBuchDataRow
    Inherits DataRow

    Friend Property FritzBoxNummer As FritzBoxXMLNummer
    Friend Property FritzBoxEmail As FritzBoxXMLEmail
    Friend Property FritzBoxKontakt As FritzBoxXMLKontakt
    Public Sub New(rb As DataRowBuilder)
        MyBase.New(rb)
    End Sub

End Class
