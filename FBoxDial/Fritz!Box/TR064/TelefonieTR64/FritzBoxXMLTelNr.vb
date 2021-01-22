Imports System.Xml.Serialization
<Serializable()> Public Class FritzBoxXMLTelNr
	<XmlElement("Number")> Public Property Number As String
	<XmlElement("Type")> Public Property Type As String
	<XmlElement("Index")> Public Property Index As Integer
	<XmlElement("Name")> Public Property Name As String
End Class
