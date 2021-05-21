Imports System.Xml.Serialization

Namespace SOAP
    <Serializable()>
    Public Class Icon

        <XmlElement("mimetype")> Public Property Mimetype As String
        <XmlElement("width")> Public Property Width As Integer
        <XmlElement("height")> Public Property Height As Integer
        <XmlElement("depth")> Public Property Depth As Integer
        <XmlElement("url")> Public Property URL As String

    End Class
End Namespace

