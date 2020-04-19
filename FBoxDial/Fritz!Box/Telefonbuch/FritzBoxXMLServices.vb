Imports System.Xml.Serialization
Imports System.ComponentModel

<Serializable()> Public Class FritzBoxXMLServices
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    <XmlElement("email")> Public Property Emails As List(Of FritzBoxXMLEmail)

    Friend ReadOnly Property GetFirstEMail As FritzBoxXMLEmail
        Get
            Return If(Emails.Any, Emails.First, Nothing)
        End Get
    End Property
End Class
