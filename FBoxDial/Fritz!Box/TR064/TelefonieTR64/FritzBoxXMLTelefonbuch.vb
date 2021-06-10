Imports System.Threading.Tasks
Imports System.Windows.Media
Imports System.Xml.Serialization
<Serializable(), XmlType("phonebook")> Public Class FritzBoxXMLTelefonbuch
    Inherits NotifyBase
    Public Sub New()
        Kontakte = New ObservableCollectionEx(Of FritzBoxXMLKontakt)
    End Sub

#Region "Fritz!Box Eigenschaften"

    Private _Owner As String
    <XmlAttribute("owner")> Public Property Owner As String
        Get
            Return _Owner
        End Get
        Set
            SetProperty(_Owner, Value)
        End Set
    End Property

    Private _Name As String
    <XmlAttribute("name")> Public Property Name As String
        Get
            Return _Name
        End Get
        Set
            SetProperty(_Name, Value)
        End Set
    End Property

    Private _Zeitstempel As String
    <XmlElement("timestamp")> Public Property Zeitstempel As String
        Get
            Return _Zeitstempel
        End Get
        Set
            SetProperty(_Zeitstempel, Value)
        End Set
    End Property

    Private _Kontakte As ObservableCollectionEx(Of FritzBoxXMLKontakt)
    <XmlElement("contact")> Public Property Kontakte As ObservableCollectionEx(Of FritzBoxXMLKontakt)
        Get
            Return _Kontakte
        End Get
        Set
            SetProperty(_Kontakte, Value)
        End Set
    End Property
#End Region

#Region "Eigene Eigenschaften"

    Private _ID As Integer
    <XmlIgnore> Friend Property ID As Integer
        Get
            Return _ID
        End Get
        Set
            SetProperty(_ID, Value)
        End Set
    End Property

    Private _IsBookEditMode As Boolean
    <XmlIgnore> Public Property IsBookEditMode As Boolean
        Get
            Return _IsBookEditMode
        End Get
        Set
            SetProperty(_IsBookEditMode, Value)
            OnPropertyChanged(NameOf(IsBookDisplayMode))
        End Set
    End Property

    <XmlIgnore> Public ReadOnly Property IsBookDisplayMode As Boolean
        Get
            Return Not IsBookEditMode
        End Get
    End Property

    <XmlIgnore> Friend Property Rufsperren As Boolean = False

#End Region

#Region "Funktionen"

    ''' <summary>
    ''' Fügt den übergebenen Kontakt hinzu. 
    ''' Kontakte mit der selben ID werden entfernt (sollte beim Aktualisieren nur einer sein.
    ''' </summary>
    ''' <param name="Kontakt"></param>
    Friend Sub AddContact(Kontakt As FritzBoxXMLKontakt)
        With Kontakte
            ' Kontakt hinzufügen
            .Add(Kontakt)
        End With
    End Sub

    ''' <summary>
    ''' Entfernt einen Kontakt aus der Liste.
    ''' </summary>
    ''' <param name="Kontakt">Der zu entfernende Kontakt.</param>
    Friend Sub DeleteKontakt(Kontakt As FritzBoxXMLKontakt)
        With Kontakte
            ' Kontake entfernen
            .Remove(Kontakt)
        End With
    End Sub

    ''' <summary>
    ''' Entfernt eine Auflistung von Kontakten aus dem Telefonbuch
    ''' </summary>
    ''' <param name="RemoveKontakte">Liste der zu entfernenden Kontakte.</param>
    Friend Sub DeleteKontakte(RemoveKontakte As List(Of FritzBoxXMLKontakt))
        With Kontakte
            ' Kontake entfernen
            .RemoveRange(RemoveKontakte)
        End With
    End Sub

    ''' <summary>
    ''' Durchsucht die Kontakte nach der übergebenen Telefonnummer.
    ''' </summary>
    ''' <param name="TelNr">Zu suchende Telefonnummer</param>
    ''' <returns>Eine Auflistung aller infrage kommenden Kontakte.</returns>
    Friend Function FindbyNumber(TelNr As Telefonnummer) As IEnumerable(Of FritzBoxXMLKontakt)
        Return Kontakte.Where(Function(K)
                                  ' interne Telefone sollen nicht duchsucht werden
                                  Return Not K.IstTelefon AndAlso K.Telefonie.Nummern.Where(Function(N) TelNr.Equals(N.Nummer)).Any
                              End Function)
    End Function

    ''' <summary>
    ''' Gibt an, ob das Telefonbuch einen Kontakt mit der gesuchten Telefonnummer enthält.
    ''' </summary>
    ''' <param name="TelNr">Zu suchende Telefonnummer</param>
    Friend Function ContainsNumber(TelNr As Telefonnummer) As Boolean
        Return Kontakte.Where(Function(K)
                                  ' interne Telefone sollen nicht duchsucht werden
                                  Return Not K.IstTelefon AndAlso K.Telefonie.Nummern.Where(Function(N) TelNr.Equals(N.Nummer)).Any
                              End Function).Any
    End Function

    Friend Async Sub LadeKonaktBilder()

        Dim SessionID As String = FritzBoxDefault.DfltFritzBoxSessionID

        Using fbtr064 As New SOAP.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, XMLData.POptionen.Anmeldeinformationen)

            If fbtr064.GetSessionID(SessionID) Then

                ' Schleife duch alle Kontakte
                For Each Kontakt In Kontakte
                    With Kontakt
                        If .Person IsNot Nothing AndAlso .Person.ImageURL.IsNotStringNothingOrEmpty Then
                            ' Setze den Pfad zum Bild zusammen
                            Dim u As New Uri($"https://{XMLData.POptionen.ValidFBAdr}:{SOAP.DfltTR064PortSSL}{ .Person.ImageURL}&{SessionID}")
                            Dim b As Byte() = {}

                            ' Lade das Bild herunter
                            b = Await SOAP.DownloadDataTaskAsync(u)
                            If b.Any Then
                                Dim biImg As New Imaging.BitmapImage()
                                Dim ms As New IO.MemoryStream(b)
                                .Person.ImageData = New Imaging.BitmapImage()

                                With biImg
                                    .BeginInit()
                                    .StreamSource = ms
                                    .EndInit()
                                End With

                                .Person.ImageData = biImg
                            End If

                        End If
                    End With

                Next

            End If
        End Using

    End Sub

#End Region

End Class
