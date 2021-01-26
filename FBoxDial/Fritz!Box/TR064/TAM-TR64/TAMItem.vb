Imports System.Xml.Serialization
<Serializable()> Public Class TAMItem
    <XmlElement("Index")> Public Property Index As Integer
    <XmlElement("Display")> Public Property Display As Boolean
    <XmlElement("Enable")> Public Property Enable As Boolean
    <XmlElement("Name")> Public Property Name As String

End Class

