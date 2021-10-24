Imports System.Xml.Serialization
<Serializable()> Public Class SIPTelNr
	<XmlElement("Number")> Public Property Number As String
	<XmlElement("Type")> Public Property Type As EType
	<XmlElement("Index")> Public Property Index As String
	<XmlElement("Name")> Public Property Name As String

End Class

Public Enum EType
	eAllCalls = 0
	eGSM = 2
	eISDN = 4
	eNone = 8
	ePOTS = 16
	eVoIP = 32
End Enum