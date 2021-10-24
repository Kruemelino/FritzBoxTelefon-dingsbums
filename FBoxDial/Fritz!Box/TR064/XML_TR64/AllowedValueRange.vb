Imports System.Xml.Serialization

Namespace TR064
    <Serializable()> Public Class AllowedValueRange
        <XmlElement("minimum")> Public Property Minimum As String
        <XmlElement("maximum")> Public Property Maximum As String
        <XmlElement("step")> Public Property [Step] As String
    End Class
End Namespace

