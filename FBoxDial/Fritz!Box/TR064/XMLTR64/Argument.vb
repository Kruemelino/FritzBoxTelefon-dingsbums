Imports System.Xml.Serialization

Friend Structure ArgumentDirection
    Friend Shared [IN] As String = "in"
    Friend Shared OUT As String = "out"
End Structure

<Serializable()>
Public Class Argument
    <XmlElement("name")> Public Property Name As String
    <XmlElement("direction")> Public Property Direction As String
    '<XmlElement("relatedStateVariable")> Public Property RelatedStateVariable As String
End Class
