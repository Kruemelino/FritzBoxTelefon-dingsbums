Imports System.Reflection

Public Class OptionenViewModel
    Inherits NotifyBase
    Private Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Eigenschaften"
#Region "Grunddaten"
#Region "Grunddaten Fritz!Box"
    Private _TBFBAdr As String
    Private _TBBenutzer As String
    Private _TBPasswort As String

    Public Property TBFBAdr As String
        Get
            Return _TBFBAdr
        End Get
        Set
            SetProperty(_TBFBAdr, Value)
        End Set
    End Property

    Public Property TBBenutzer As String
        Get
            Return _TBBenutzer
        End Get
        Set
            SetProperty(_TBBenutzer, Value)
        End Set
    End Property

    Public Property TBPasswort As String
        Get
            Return _TBPasswort
        End Get
        Set
            SetProperty(_TBPasswort, Value)
        End Set
    End Property

    Public ReadOnly Property AddinVersion As String = $"Info V{Assembly.GetExecutingAssembly.GetName.Version}"
#End Region

#Region "Grunddaten Telefonie"
    Private _TBOrtsKZ As String
    Private _TBLandesKZ As String

    Public Property TBOrtsKZ As String
        Get
            Return _TBOrtsKZ
        End Get
        Set
            SetProperty(_TBOrtsKZ, Value)
        End Set
    End Property

    Public Property TBLandesKZ As String
        Get
            Return _TBLandesKZ
        End Get
        Set
            SetProperty(_TBLandesKZ, Value)
        End Set
    End Property
#End Region

#Region "Formatierung von Telefonnummern"
    Private _TBTelNrMaske As String
    Private _CBTelNrGruppieren As Boolean
    Private _CBintl As Boolean

    Public Property TBTelNrMaske As String
        Get
            Return _TBTelNrMaske
        End Get
        Set
            SetProperty(_TBTelNrMaske, Value)
        End Set
    End Property

    Public Property CBTelNrGruppieren As Boolean
        Get
            Return _CBTelNrGruppieren
        End Get
        Set
            SetProperty(_CBTelNrGruppieren, Value)
        End Set
    End Property

    Public Property CBintl As Boolean
        Get
            Return _CBintl
        End Get
        Set
            SetProperty(_CBintl, Value)
        End Set
    End Property
#End Region

#Region "Anruflisten"
    Private _TBNumEntryList As Integer
    Public Property TBNumEntryList As Integer
        Get
            Return _TBNumEntryList
        End Get
        Set
            SetProperty(_TBNumEntryList, Value)
        End Set
    End Property
#End Region

#End Region

#Region "Einstellungen für den Anrufmonitor"
    Private _CBAnrMonAuto As Boolean
    Private _CBAutoClose As Boolean
    Private _TBEnblDauer As Integer
    Private _CBAnrMonZeigeKontakt As Boolean
    Private _CBAnrMonContactImage As Boolean

    Public Property CBAnrMonAuto As Boolean
        Get
            Return _CBAnrMonAuto
        End Get
        Set
            SetProperty(_CBAnrMonAuto, Value)
        End Set
    End Property
    ''' <summary>
    ''' Angabe, ob der Anrufmonitor automatisch geschlossen werden soll.
    ''' </summary>
    Public Property CBAutoClose As Boolean
        Get
            Return _CBAutoClose
        End Get
        Set
            SetProperty(_CBAutoClose, Value)
        End Set
    End Property

    ''' <summary>
    ''' Einblenddauer des Anrufmonitors in Sekunden.
    ''' </summary>
    Public Property TBEnblDauer As Integer
        Get
            Return _TBEnblDauer
        End Get
        Set
            SetProperty(_TBEnblDauer, Value)
        End Set
    End Property

    ''' <summary>
    ''' Angabe, ob der Kontakt Angezeigt werden soll
    ''' </summary>
    Public Property CBAnrMonZeigeKontakt As Boolean
        Get
            Return _CBAnrMonZeigeKontakt
        End Get
        Set
            SetProperty(_CBAnrMonZeigeKontakt, Value)
        End Set
    End Property

    ''' <summary>
    ''' Angabe, ob ein Kontaktbild angezeigt werden soll.
    ''' </summary>
    Public Property CBAnrMonContactImage As Boolean
        Get
            Return _CBAnrMonContactImage
        End Get
        Set
            SetProperty(_CBAnrMonContactImage, Value)
        End Set
    End Property

    ''' <summary>
    ''' Returns Or sets a list as Telefonnummern             
    ''' </summary>
    Private _TelNrListe As ObservableCollectionEx(Of Telefonnummer)

    Public Property TelNrListe As ObservableCollectionEx(Of Telefonnummer)
        Get
            Return _TelNrListe
        End Get
        Set(value As ObservableCollectionEx(Of Telefonnummer))
            SetProperty(_TelNrListe, value)
        End Set
    End Property

#End Region

#Region "Einstellung für die Wählhilfe"
    Private _CBForceDialLKZ As Boolean
    Private _TBAmt As String
    Private _CBCheckMobil As Boolean
    Private _CBCLIR As Boolean
    Private _CBCloseWClient As Boolean
    Private _TBWClientEnblDauer As Integer

    Public Property CBForceDialLKZ As Boolean
        Get
            Return _CBForceDialLKZ
        End Get
        Set
            SetProperty(_CBForceDialLKZ, Value)
        End Set
    End Property

    ''' <summary>
    ''' Gibt an, ob eine Amtsholung stets mitgewählt werden soll. Die Amtsholung wird in den Einstellungen festgelegt.
    ''' </summary>
    Public Property TBAmt As String
        Get
            Return _TBAmt
        End Get
        Set
            SetProperty(_TBAmt, Value)
        End Set
    End Property

    Public Property CBCheckMobil As Boolean
        Get
            Return _CBCheckMobil
        End Get
        Set
            SetProperty(_CBCheckMobil, Value)
        End Set
    End Property

    Public Property CBCLIR As Boolean
        Get
            Return _CBCLIR
        End Get
        Set
            SetProperty(_CBCLIR, Value)
        End Set
    End Property

    Public Property CBCloseWClient As Boolean
        Get
            Return _CBCloseWClient
        End Get
        Set
            SetProperty(_CBCloseWClient, Value)
        End Set
    End Property

    Public Property TBWClientEnblDauer As Integer
        Get
            Return _TBWClientEnblDauer
        End Get
        Set
            SetProperty(_TBWClientEnblDauer, Value)
        End Set
    End Property
#End Region

#Region "Einstellungen für die Kontaktsuche"
#Region "Einstellungen für die Kontaktsuche - Kontaktsuche in Outlook (Indizierung)"
    Private _CBSucheUnterordner As Boolean
    Private _CBKontaktSucheFritzBox As Boolean
    Private _OutlookOrdner As ObservableCollectionEx(Of OutlookOrdner)
    Public Property CBSucheUnterordner As Boolean
        Get
            Return _CBSucheUnterordner
        End Get
        Set
            SetProperty(_CBSucheUnterordner, Value)
        End Set
    End Property

    Public Property CBKontaktSucheFritzBox As Boolean
        Get
            Return _CBKontaktSucheFritzBox
        End Get
        Set
            SetProperty(_CBKontaktSucheFritzBox, Value)
        End Set
    End Property
    Public Property OutlookOrdnerVM As ObservableCollectionEx(Of OutlookOrdner)
        Get
            Return _OutlookOrdner
        End Get
        Set
            SetProperty(_OutlookOrdner, Value)
        End Set
    End Property
#End Region

#Region "Einstellungen für die Kontaktsuche - Rückwärtssuche (RWS)"
    Private _CBRWS As Boolean
    Private _CBKErstellen As Boolean
    Private _CBRWSIndex As Boolean

    Public Property CBRWS As Boolean
        Get
            Return _CBRWS
        End Get
        Set
            SetProperty(_CBRWS, Value)
        End Set
    End Property

    Public Property CBKErstellen As Boolean
        Get
            Return _CBKErstellen
        End Get
        Set
            SetProperty(_CBKErstellen, Value)
        End Set
    End Property

    Public Property PCBRWSIndex As Boolean
        Get
            Return _CBRWSIndex
        End Get
        Set
            SetProperty(_CBRWSIndex, Value)
        End Set
    End Property

#End Region

#End Region

#Region "Auswertung der Fritz!box Anrufliste"
#Region "Auswertung der Fritz!box Anrufliste - Outlook Journal"
    Private _CBJournal As Boolean
    Public Property CBJournal As Boolean
        Get
            Return _CBJournal
        End Get
        Set
            _CBJournal = Value
        End Set
    End Property
#End Region

    '#Region "Auswertung der Fritz!box Anrufliste - Auswertung der Anrufliste"
    '    Public Property LetzterJournalEintrag As Date
    '    Public Property LetzterJournalEintragID As Integer
    '    Public Property CBAutoAnrList As Boolean
    '    Public Property CBAnrListeUpdateCallLists As Boolean
    '#End Region
#End Region

#Region "Telefoniegeräte"
    Private _TelGerListe As ObservableCollectionEx(Of Telefoniegerät)

    Public Property TelGeräteListe As ObservableCollectionEx(Of Telefoniegerät)
        Get
            Return _TelGerListe
        End Get
        Set(value As ObservableCollectionEx(Of Telefoniegerät))
            SetProperty(_TelGerListe, value)
        End Set
    End Property
#End Region
#Region "SoftPhones"

#Region "Phoner"
    Private _TBPhonerPasswort As String
    Private _CBPhoner As Boolean

    Public Property TBPhonerPasswort As String
        Get
            Return _TBPhonerPasswort
        End Get
        Set
            _TBPhonerPasswort = Value
        End Set
    End Property

    Public Property CBPhoner As Boolean
        Get
            Return _CBPhoner
        End Get
        Set
            _CBPhoner = Value
        End Set
    End Property
#End Region
#Region "MicroSIP"
    Private _TBMicroSIPPath As String
    Private _CBMicroSIP As Boolean

    Public Property TBMicroSIPPath As String
        Get
            Return _TBMicroSIPPath
        End Get
        Set
            _TBMicroSIPPath = Value
        End Set
    End Property
    Public Property CBMicroSIP As Boolean
        Get
            Return _CBMicroSIP
        End Get
        Set
            _CBMicroSIP = Value
        End Set
    End Property
#End Region

#End Region

#Region "Logging"
    Private _CBoxMinLogLevel As String

    Public Property CBoxMinLogLevel As String
        Get
            Return _CBoxMinLogLevel
        End Get
        Set
            _CBoxMinLogLevel = Value
        End Set
    End Property

    Public ReadOnly Property CBoxLogLevel As IEnumerable(Of LogLevel) = LogLevel.AllLoggingLevels
    Public ReadOnly Property PfadArbeitsverzeichnis As String = XMLData.POptionen.Arbeitsverzeichnis
#End Region

#End Region

    Public Sub New()
        LadeDaten()

    End Sub

    ''' <summary>
    ''' Lädt die daten aus den <see cref="Optionen"/> in dieses Viewmodel.
    ''' </summary>
    Friend Sub LadeDaten()
        NLogger.Debug("Lade die Daten aus der XML-Datei in das ViewModel Optionen")

        ' Schleife durch alle Properties dieser Klasse
        For Each ViewModelPropertyInfo As PropertyInfo In [GetType].GetProperties
            ' Suche das passende Property in den Optionen
            With Array.Find(XMLData.POptionen.GetType.GetProperties, Function(PropertyInfo As PropertyInfo) PropertyInfo.Name.AreEqual(ViewModelPropertyInfo.Name))

                If ViewModelPropertyInfo.CanWrite Then
                    ViewModelPropertyInfo.SetValue(Me,?.GetValue(XMLData.POptionen))
                    NLogger.Debug("Feld {0} mit Wert {1} geladen", ViewModelPropertyInfo.Name, ViewModelPropertyInfo.GetValue(Me))
                End If

            End With
        Next

        ' Anrufmonitor Liste zu überwachender Telefonnummern
        TelNrListe = New ObservableCollectionEx(Of Telefonnummer)
        TelNrListe.AddRange(XMLData.PTelefonie.Telefonnummern)

        ' Telefoniegeräteliste
        TelGeräteListe = New ObservableCollectionEx(Of Telefoniegerät)
        TelGeräteListe.AddRange(XMLData.PTelefonie.Telefoniegeräte)

        ' Ornderliste überwachter Ordner
        OutlookOrdnerVM = New ObservableCollectionEx(Of OutlookOrdner)
        OutlookOrdnerVM.AddRange(XMLData.POptionen.OutlookOrdner.OrdnerListe)
    End Sub

    ''' <summary>
    ''' Speichert die Daten aus diesem ViewModel zurück in die <see cref="Optionen"/>.
    ''' </summary>
    Friend Sub Speichern()
        NLogger.Debug("Speichere die Daten aus dem ViewModel Optionen in die XML-Datei")
        ' Schleife durch alle Properties dieser Klasse
        For Each ViewModelPropertyInfo As PropertyInfo In [GetType].GetProperties
            ' Suche das passende Property in den Optionen
            Dim OptionPropertyInfo As PropertyInfo = Array.Find(XMLData.POptionen.GetType.GetProperties, Function(PropertyInfo As PropertyInfo) PropertyInfo.Name.AreEqual(ViewModelPropertyInfo.Name))

            If OptionPropertyInfo IsNot Nothing Then
                With OptionPropertyInfo
                    ?.SetValue(XMLData.POptionen, ViewModelPropertyInfo.GetValue(Me))
                    NLogger.Debug("Feld {0} mit Wert {1} übergeben: {2}", ViewModelPropertyInfo.Name, ViewModelPropertyInfo.GetValue(Me),?.GetValue(XMLData.POptionen))

                End With
            End If
        Next

        ' Gültige IP-Adresse für die Fritz!Box ablegen
        XMLData.POptionen.ValidFBAdr = ValidIP(XMLData.POptionen.TBFBAdr)

        ' Anrufmonitor Liste zu überwachender Telefonnummern
        With XMLData.PTelefonie.Telefonnummern
            ' Die Telefonnummern in den Optionen löschen
            .Clear()
            ' Die Telefonnummern aus den Viewmodel setzen
            .AddRange(TelNrListe)
        End With

        ' Telefoniegeräteliste
        With XMLData.PTelefonie.Telefoniegeräte
            ' Die Telefoniegeräte in den Optionen löschen
            .Clear()
            ' Die Telefoniegeräte aus den Viewmodel setzen
            .AddRange(TelGeräteListe)
        End With

        ' Ornderliste überwachter Ordner
        With XMLData.POptionen.OutlookOrdner.OrdnerListe
            ' Die Ordner in den Optionen löschen
            .Clear()
            ' Die Ordner aus den Viewmodel setzen
            .AddRange(OutlookOrdnerVM)
        End With

        ' Speichern in Datei anstoßen
        Serializer.Speichern(XMLData, IO.Path.Combine(XMLData.POptionen.Arbeitsverzeichnis, $"{My.Resources.strDefShortName}.xml"))
    End Sub
End Class
