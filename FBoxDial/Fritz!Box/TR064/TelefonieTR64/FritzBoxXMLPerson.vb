Imports System.Xml.Serialization
<Serializable()> Public Class FritzBoxXMLPerson
    <XmlElement("realName")> Public Property RealName As String
    <XmlElement("imageURL")> Public Property ImageURL As String
End Class
