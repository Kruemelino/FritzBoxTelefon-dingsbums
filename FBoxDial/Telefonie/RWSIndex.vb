Imports System.Xml.Serialization
<Serializable()> Public Class RWSIndexEntry
    <XmlElement> Public Property VCard As String
    <XmlElement> Public Property TelNr As String
    <XmlAttribute> Public Property Datum As Date
    <XmlAttribute> Public Property Suchmaschine As String
End Class