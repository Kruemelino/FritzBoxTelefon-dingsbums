Imports System.Xml.Serialization
<Serializable()>
<XmlRoot("List")> Public Class SIPTelNrList
    <XmlElement("Item")> Public Property TelNrList As List(Of SIPTelNr)
End Class
