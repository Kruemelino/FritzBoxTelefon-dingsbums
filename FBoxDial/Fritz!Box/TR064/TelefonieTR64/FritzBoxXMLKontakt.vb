Imports System.Xml.Serialization
<Serializable()> Public Class FritzBoxXMLKontakt
    <XmlElement("category")> Public Property Kategorie As String
    <XmlElement("person")> Public Property Person As FritzBoxXMLPerson
    <XmlElement("uniqueid")> Public Property Uniqueid As String
    <XmlElement("telephony")> Public Property Telefonie As FritzBoxXMLTelefonie
End Class
