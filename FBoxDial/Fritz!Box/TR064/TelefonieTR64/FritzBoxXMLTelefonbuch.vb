Imports System.Xml.Serialization
<Serializable()> Public Class FritzBoxXMLTelefonbuch
    Inherits NotifyBase

    Private _Kontakte As ObservableCollectionEx(Of FritzBoxXMLKontakt)
    Private _Zeitstempel As String
    Private _Owner As String
    Private _Name As String
    Private _ID As Integer

    <XmlAttribute("owner")> Public Property Owner As String
        Get
            Return _Owner
        End Get
        Set
            SetProperty(_Owner, Value)
        End Set
    End Property

    <XmlAttribute("name")> Public Property Name As String
        Get
            Return _Name
        End Get
        Set
            SetProperty(_Name, Value)
        End Set
    End Property

    <XmlElement("timestamp")> Public Property Zeitstempel As String
        Get
            Return _Zeitstempel
        End Get
        Set
            SetProperty(_Zeitstempel, Value)
        End Set
    End Property

    <XmlElement("contact")> Public Property Kontakte As ObservableCollectionEx(Of FritzBoxXMLKontakt)
        Get
            Return _Kontakte
        End Get
        Set
            SetProperty(_Kontakte, Value)
        End Set
    End Property

    <XmlIgnore> Friend Property ID As Integer
        Get
            Return _ID
        End Get
        Set
            SetProperty(_ID, Value)
        End Set
    End Property
End Class
