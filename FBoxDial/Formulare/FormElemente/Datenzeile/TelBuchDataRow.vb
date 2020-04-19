Imports System.Data
Public Class TelBuchDataRow
    Inherits DataRow

    Friend Property Telefonbucheintrag As FritzBoxXMLKontakt

    Public Sub New(ByVal rb As DataRowBuilder)
        MyBase.New(rb)
    End Sub
End Class
