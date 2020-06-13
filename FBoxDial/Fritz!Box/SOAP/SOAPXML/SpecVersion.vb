Imports System.Xml.Serialization
<Serializable()>
Public Class SpecVersion
    <XmlElement("major")> Public Property Major As Integer
    <XmlElement("minor")> Public Property Minor As Integer
End Class
