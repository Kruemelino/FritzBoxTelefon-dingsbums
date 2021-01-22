Imports System.Xml.Serialization
<Serializable()> Public Class FritzBoxXMLTelefonbuch
    <XmlAttribute("owner")> Public Property Owner As String
    <XmlAttribute("name")> Public Property Name As String
    <XmlElement("timestamp")> Public Property Zeitstempel As String
    <XmlElement("contact")> Public Property Kontakte As List(Of FritzBoxXMLKontakt)
    <XmlIgnore> Friend Property ID As String
End Class
