Imports System.Xml.Serialization

Namespace TR064
    <Serializable()>
    <XmlRoot("phonebooks"), XmlType("phonebooks")> Public Class FritzBoxXMLTelefonbücher
        Inherits NotifyBase

        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

        Public Sub New()
            Telefonbücher = New ObservableCollectionEx(Of FritzBoxXMLTelefonbuch)
        End Sub

        Private _Telefonbücher As ObservableCollectionEx(Of FritzBoxXMLTelefonbuch)
        <XmlElement("phonebook")> Public Property Telefonbücher As ObservableCollectionEx(Of FritzBoxXMLTelefonbuch)
            Get
                Return _Telefonbücher
            End Get
            Set
                SetProperty(_Telefonbücher, Value)
            End Set
        End Property

        <XmlIgnore> Friend Property NurHeaderDaten As Boolean

        Friend Function Find(TelNr As Telefonnummer) As FritzBoxXMLKontakt
            NLogger.Debug($"Starte Kontaktsuche in den Fritz!Box Telefonbüchern für Telefonnummer '{TelNr.Unformatiert}'.")

            ' Suche alle Telefonbücher mit einem entsprechenden Kontakt
            Dim Bücher As IEnumerable(Of FritzBoxXMLTelefonbuch) = Telefonbücher.Where(Function(B) B.ContainsNumber(TelNr))

            If Bücher.Any Then
                NLogger.Debug($"Telefonnummer {TelNr.Unformatiert} in {Bücher.Count} Buch/Büchern gefunden.")
                ' Extrahiere einen Kontakt mit dieser Nummer
                Return Bücher.First.FindbyNumber(TelNr).First
            Else
                Return Nothing
            End If

        End Function

    End Class
End Namespace

