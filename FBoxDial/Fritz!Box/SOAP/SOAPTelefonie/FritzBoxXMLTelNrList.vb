Imports System.Xml.Serialization
<Serializable()>
<XmlRoot("List")> Public Class FritzBoxXMLTelNrList
    <XmlElement("Item")> Public Property TelNr As List(Of FritzBoxXMLTelNr)
End Class
