Imports System.Xml.Serialization
Namespace TR064
    <Serializable()>
    <XmlRoot("root"), XmlType("root")> Public Class FritzBoxXMLCallList
        <XmlElement("timestamp")> Public Property Zeitstempel As String
        <XmlElement("Call")> Public Property Calls As List(Of FritzBoxXMLCall)
    End Class
End Namespace
