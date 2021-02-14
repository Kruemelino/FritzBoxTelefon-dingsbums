Imports System.Xml.Serialization
<Serializable()>
<XmlRoot("phonebooks"), XmlType("phonebooks")> Public Class FritzBoxXMLTelefonbücher
    Inherits NotifyBase

    Private _Telefonbücher As ObservableCollectionEx(Of FritzBoxXMLTelefonbuch)

    <XmlElement("phonebook")> Public Property Telefonbücher As ObservableCollectionEx(Of FritzBoxXMLTelefonbuch)
        Get
            Return _Telefonbücher
        End Get
        Set
            SetProperty(_Telefonbücher, Value)
        End Set
    End Property

    Friend Function Find(TelNr As Telefonnummer) As FritzBoxXMLKontakt
        ' Suche alle Telefonbücher mit einem entsprechenden Kontakt
        Dim Bücher As IEnumerable(Of FritzBoxXMLTelefonbuch) = Telefonbücher.Where(Function(B) B.ContainsNumber(TelNr))

        If Bücher.Any Then
            ' Extrahiere einen Kontakt mit dieser Nummer
            Return Bücher.First.FindbyNumber(TelNr).First
        Else
            Return Nothing
        End If

    End Function
End Class
