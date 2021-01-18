Imports System.Xml.Serialization
<Serializable()>
<XmlRoot("phonebooks")> Public Class FritzBoxXMLTelefonbücher
    <XmlElement("phonebook")> Public Property Telefonbuch As List(Of FritzBoxXMLTelefonbuch)

    Private ReadOnly Property AlleKontakte As List(Of FritzBoxXMLKontakt)
        Get
            Dim tmpKontakte As New List(Of FritzBoxXMLKontakt)

            For Each tmpTelefonbuch As FritzBoxXMLTelefonbuch In Telefonbuch
                tmpKontakte.AddRange(tmpTelefonbuch.Kontakte)
            Next
            Return tmpKontakte
        End Get
    End Property

    Public ReadOnly Property GetKontaktByTelNr(TelNr As Telefonnummer) As FritzBoxXMLKontakt
        Get
            Return AlleKontakte.Find(Function(TV) TV.Telefonie.Nummern.Exists(Function(AB) TelNr.Equals(AB.Nummer)))
        End Get
    End Property
End Class
