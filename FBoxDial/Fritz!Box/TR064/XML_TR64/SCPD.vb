Imports System.Xml.Serialization

Namespace TR064
    <Serializable()>
    <XmlRoot("scpd", Namespace:="urn:dslforum-org:service-1-0", IsNullable:=False)>
    Public Class ServiceControlProtocolDefinition
        <XmlElement("specVersion")> Public Property [SpecVersion] As SpecVersion
        <XmlArray("actionList")> <XmlArrayItem("action")> Public Property ActionList As List(Of Action)
        <XmlArray("serviceStateTable")> <XmlArrayItem("stateVariable")> Public Property ServiceStateTable As List(Of StateVariable)
    End Class
End Namespace

