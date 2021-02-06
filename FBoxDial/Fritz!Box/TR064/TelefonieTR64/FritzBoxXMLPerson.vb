Imports System.Xml.Serialization
<Serializable()> Public Class FritzBoxXMLPerson
    Inherits NotifyBase

    Private _RealName As String
    Private _ImageURL As String

    <XmlElement("realName")> Public Property RealName As String
        Get
            Return _RealName
        End Get
        Set
            SetProperty(_RealName, Value)
        End Set
    End Property

    <XmlElement("imageURL")> Public Property ImageURL As String
        Get
            Return _ImageURL
        End Get
        Set
            SetProperty(_ImageURL, Value)
        End Set
    End Property
End Class
