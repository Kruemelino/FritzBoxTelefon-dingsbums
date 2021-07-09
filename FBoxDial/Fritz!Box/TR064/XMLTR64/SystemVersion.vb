Imports System.Xml.Serialization

Namespace SOAP
    <Serializable()>
    Public Class SystemVersion
        <XmlElement("HW")> Public Property HW As Integer
        <XmlElement("Major")> Public Property Major As Integer
        <XmlElement("Minor")> Public Property Minor As Integer
        <XmlElement("Patch")> Public Property Patch As Integer
        <XmlElement("Buildnumber")> Public Property Buildnumber As Integer
        <XmlElement("Display")> Public Property Display As String
    End Class
End Namespace
