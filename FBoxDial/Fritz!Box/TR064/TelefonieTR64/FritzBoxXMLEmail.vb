Imports System.Xml.Serialization
<Serializable()> Public Class FritzBoxXMLEmail
    <XmlAttribute("classifier")> Public Property Klassifizierer As String
    <XmlText()> Public Property EMail As String


End Class
