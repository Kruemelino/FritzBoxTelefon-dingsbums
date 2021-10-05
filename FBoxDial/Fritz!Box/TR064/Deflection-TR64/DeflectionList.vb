Imports System.Xml.Serialization
<Serializable()>
<XmlRoot("List")> Public Class DeflectionList
    <XmlElement("Item")> Public Property DeflectionListe As List(Of DeflectionInfo)

    Public Sub New()
        DeflectionListe = New List(Of DeflectionInfo)
    End Sub
End Class
