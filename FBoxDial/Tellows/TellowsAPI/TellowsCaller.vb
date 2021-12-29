Imports System.Xml.Serialization

<Serializable(), XmlType("caller")> Public Class TellowsCaller
    ''' <summary>
    ''' Tag for caller
    ''' </summary>
    <XmlElement("name")> Public Property Name As String

    ''' <summary>
    ''' amount of users, that used this tag
    ''' </summary>
    <XmlElement("count")> Public Property Count As Integer
End Class
