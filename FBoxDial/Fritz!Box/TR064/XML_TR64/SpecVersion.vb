Imports System.Xml.Serialization

Namespace TR064
    <Serializable()>
    Public Class SpecVersion
        <XmlElement("major")> Public Property Major As Integer
        <XmlElement("minor")> Public Property Minor As Integer
    End Class
End Namespace