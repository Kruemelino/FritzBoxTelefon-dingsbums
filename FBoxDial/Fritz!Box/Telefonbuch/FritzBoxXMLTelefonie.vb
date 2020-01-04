Imports System.Xml.Serialization
<Serializable()> Public Class FritzBoxXMLTelefonie
    <XmlElement("services")> Public Property Dienste As FritzBoxXMLServices
    <XmlElement("number")> Public Property Nummern As List(Of FritzBoxXMLNummer)
End Class
