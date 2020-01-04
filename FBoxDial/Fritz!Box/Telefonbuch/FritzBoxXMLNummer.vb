Imports System.Xml.Serialization
Imports FBoxDial

<Serializable()> Public Class FritzBoxXMLNummer

    <XmlAttribute("classifier")> Public Property Type As String
    <XmlAttribute("vanity")> Public Property Vanity As String
    <XmlAttribute("prio")> Public Property Prio As String
    <XmlText()> Public Property Nummer As String
End Class
