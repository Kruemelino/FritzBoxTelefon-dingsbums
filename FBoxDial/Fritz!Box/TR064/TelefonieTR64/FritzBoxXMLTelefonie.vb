Imports System.Xml.Serialization
<Serializable()> Public Class FritzBoxXMLTelefonie
    Inherits NotifyBase

    Private _Nummern As ObservableCollectionEx(Of FritzBoxXMLNummer)
    Private _Emails As ObservableCollectionEx(Of FritzBoxXMLEmail)

    <XmlArray("services"), XmlArrayItem("email")> Public Property Emails As ObservableCollectionEx(Of FritzBoxXMLEmail)
        Get
            Return _Emails
        End Get
        Set
            SetProperty(_Emails, Value)
        End Set
    End Property

    <XmlElement("number")> Public Property Nummern As ObservableCollectionEx(Of FritzBoxXMLNummer)
        Get
            Return _Nummern
        End Get
        Set
            SetProperty(_Nummern, Value)
        End Set
    End Property
End Class
