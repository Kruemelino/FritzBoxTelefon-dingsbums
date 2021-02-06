Imports System.Xml.Serialization
<Serializable()>
<XmlRoot("phonebooks")> Public Class FritzBoxXMLTelefonbücher
    Inherits NotifyBase

    Private _Telefonbuch As ObservableCollectionEx(Of FritzBoxXMLTelefonbuch)

    <XmlElement("phonebook")> Public Property Telefonbuch As ObservableCollectionEx(Of FritzBoxXMLTelefonbuch)
        Get
            Return _Telefonbuch
        End Get
        Set
            SetProperty(_Telefonbuch, Value)
        End Set
    End Property

    <XmlIgnore> Private ReadOnly Property AlleKontakte As List(Of FritzBoxXMLKontakt)
        Get
            Dim tmpKontakte As New List(Of FritzBoxXMLKontakt)

            For Each tmpTelefonbuch As FritzBoxXMLTelefonbuch In Telefonbuch
                tmpKontakte.AddRange(tmpTelefonbuch.Kontakte)
            Next
            Return tmpKontakte
        End Get
    End Property

    <XmlIgnore> Public ReadOnly Property GetKontaktByTelNr(TelNr As Telefonnummer) As FritzBoxXMLKontakt
        Get
            Return AlleKontakte.Find(Function(TV) TV.Telefonie.Nummern.Where(Function(AB) TelNr.Equals(AB.Nummer)).Any)
        End Get
    End Property
End Class
