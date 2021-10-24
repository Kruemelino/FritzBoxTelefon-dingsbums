Imports System.Xml.Serialization
<Serializable()>
<XmlRoot("Root"), XmlType("Root")> Public Class FritzBoxXMLMessageList
    <XmlElement("Message")> Public Property Messages As List(Of FritzBoxXMLMessage)
End Class
