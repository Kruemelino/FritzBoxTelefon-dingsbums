Imports System.Xml.Serialization

<Serializable()> Public Class FritzBoxXMLNummer

    <XmlAttribute("type")> Public Property Typ As String
    <XmlAttribute("vanity")> Public Property Vanity As String
    <XmlAttribute("prio")> Public Property Prio As String
    <XmlAttribute("quickdial")> Public Property Schnellwahl As String
    <XmlText()> Public Property Nummer As String

End Class
