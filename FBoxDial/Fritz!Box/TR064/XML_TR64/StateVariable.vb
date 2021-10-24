Imports System.Xml.Serialization

Namespace SOAP
    <Serializable()>
    Public Class StateVariable
        <XmlElement("name")> Public Property Name As String
        <XmlElement("dataType")> Public Property DataType As String
        <XmlElement("defaultValue")> Public Property DefaultValue As String
        <XmlAttribute("sendEvents")> Public Property SendEvents As String
        <XmlArray("allowedValueList")> <XmlArrayItem("allowedValue")> Public Property ServiceStateTable As List(Of String)
        <XmlElement("allowedValueRange")> Public Property [AllowedValueRange] As AllowedValueRange
    End Class
End Namespace

