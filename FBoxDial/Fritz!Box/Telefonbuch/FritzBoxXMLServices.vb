Imports System.Xml.Serialization
<Serializable()> Public Class FritzBoxXMLServices
    <XmlElement("email")> Public Property Emails As List(Of FritzBoxXMLEmail)
End Class
