Imports System.Xml.Serialization

<Serializable()> Public Class FritzBoxXMLServices
    <XmlElement("email")> Public Property Emails As List(Of FritzBoxXMLEmail)

    Friend ReadOnly Property GetFirstEMail As FritzBoxXMLEmail
        Get
            Return If(Emails.Any, Emails.First, Nothing)
        End Get
    End Property
End Class
