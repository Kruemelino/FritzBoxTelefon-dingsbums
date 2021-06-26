Imports System.Reflection
Imports System.Windows
Imports System.Threading.Tasks
Imports Microsoft.Office.Interop
''' <summary>
''' https://rachel53461.wordpress.com/2011/12/18/navigation-with-mvvm-2/
''' </summary>
Public Class OptionenViewModel
    Inherits NotifyBase
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IOptionenService

#Region "Addin Eigenschaften"
#Region "Grunddaten"
#Region "Grunddaten Fritz!Box"

    Private _TBFBAdr As String
    Public Property TBFBAdr As String
        Get
            Return _TBFBAdr
        End Get
        Set
            SetProperty(_TBFBAdr, Value)
        End Set
    End Property

    Private _TBBenutzer As String
    Public Property TBBenutzer As String
        Get
            Return _TBBenutzer
        End Get
        Set
            SetProperty(_TBBenutzer, Value)
        End Set
    End Property

    Private _CBoxBenutzer As ObservableCollectionEx(Of FritzBoxXMLUser)
    Public Property CBoxBenutzer As ObservableCollectionEx(Of FritzBoxXMLUser)
        Get
            Return _CBoxBenutzer
        End Get
        Set
            SetProperty(_CBoxBenutzer, Value)
        End Set
    End Property

    Private _TBPasswort As String
    Public Property TBPasswort As String
        Get
            Return _TBPasswort
        End Get
        Set
            SetProperty(_TBPasswort, Value)
        End Set
    End Property

    Public ReadOnly Property AddinVersion As String = $"Info V{Assembly.GetExecutingAssembly.GetName.Version}"
    Public ReadOnly Property DfltDeCryptKey As String = DfltWerteAllgemein.DfltDeCryptKey
    Public ReadOnly Property DfltPhonerDeCryptKey As String = DfltWerteAllgemein.DfltPhonerDeCryptKey
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
    Private _CBAnrMonVollbildAnzeigen As Boolean

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
    ''' Angabe, ob der Anrufmonitor bei Vollbildanwendungen eingeblendet werden soll.
    ''' </summary>
    Public Property CBAnrMonVollbildAnzeigen As Boolean
        Get
            Return _CBAnrMonVollbildAnzeigen
        End Get
        Set
            SetProperty(_CBAnrMonVollbildAnzeigen, Value)
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
        Set
            SetProperty(_TelNrListe, Value)
        End Set
    End Property

#End Region

#Region "Stoppuhr"
    Private _CBStoppUhrEinblenden As Boolean
    Public Property CBStoppUhrEinblenden As Boolean
        Get
            Return _CBStoppUhrEinblenden
        End Get
        Set
            SetProperty(_CBStoppUhrEinblenden, Value)
        End Set
    End Property

    Private _CBStoppUhrAusblenden As Boolean
    Public Property CBStoppUhrAusblenden As Boolean
        Get
            Return _CBStoppUhrAusblenden
        End Get
        Set
            SetProperty(_CBStoppUhrAusblenden, Value)
        End Set
    End Property

    Private _TBStoppUhrAusblendverzögerung As Integer
    Public Property TBStoppUhrAusblendverzögerung As Integer
        Get
            Return _TBStoppUhrAusblendverzögerung
        End Get
        Set
            SetProperty(_TBStoppUhrAusblendverzögerung, Value)
        End Set
    End Property
#End Region

#Region "Einstellung für die Wählhilfe"
    Private _CBForceDialLKZ As Boolean
    Private _TBPräfix As String
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
    Public Property TBPräfix As String
        Get
            Return _TBPräfix
        End Get
        Set
            SetProperty(_TBPräfix, Value)
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
    Public Property CBSucheUnterordner As Boolean
        Get
            Return _CBSucheUnterordner
        End Get
        Set
            SetProperty(_CBSucheUnterordner, Value)
        End Set
    End Property

    Private _CBKontaktSucheFritzBox As Boolean
    Public Property CBKontaktSucheFritzBox As Boolean
        Get
            Return _CBKontaktSucheFritzBox
        End Get
        Set
            SetProperty(_CBKontaktSucheFritzBox, Value)
        End Set
    End Property

    Private _OutlookOrdner As OutlookOrdnerListe
    Public Property OutlookOrdnerListe As OutlookOrdnerListe
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

    Public Property CBRWSIndex As Boolean
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
            SetProperty(_CBJournal, Value)
        End Set
    End Property
#End Region

#Region "Auswertung der Fritz!box Anrufliste - Auswertung der Anrufliste"
    Private _CBAutoAnrList As Boolean
    Private _CBAnrListeUpdateCallLists As Boolean
    Public Property CBAutoAnrList As Boolean
        Get
            Return _CBAutoAnrList
        End Get
        Set
            SetProperty(_CBAutoAnrList, Value)
        End Set
    End Property

    Public Property CBAnrListeUpdateCallLists As Boolean
        Get
            Return _CBAnrListeUpdateCallLists
        End Get
        Set
            SetProperty(_CBAnrListeUpdateCallLists, Value)
        End Set
    End Property
#End Region
#End Region

#Region "Telefoniegeräte"
    Private _TelGerListe As ObservableCollectionEx(Of Telefoniegerät)
    Public Property TelGeräteListe As ObservableCollectionEx(Of Telefoniegerät)
        Get
            Return _TelGerListe
        End Get
        Set
            SetProperty(_TelGerListe, Value)
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
            SetProperty(_TBPhonerPasswort, Value)
        End Set
    End Property

    Public Property CBPhoner As Boolean
        Get
            Return _CBPhoner
        End Get
        Set
            SetProperty(_CBPhoner, Value)
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
            SetProperty(_TBMicroSIPPath, Value)
        End Set
    End Property
    Public Property CBMicroSIP As Boolean
        Get
            Return _CBMicroSIP
        End Get
        Set
            SetProperty(_CBMicroSIP, Value)
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
            SetProperty(_CBoxMinLogLevel, Value)
        End Set
    End Property

    Public ReadOnly Property CBoxLogLevel As IEnumerable(Of LogLevel) = LogLevel.AllLoggingLevels
    Public ReadOnly Property PfadArbeitsverzeichnis As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName)
#End Region

#End Region

#Region "Window Eigenschaften"

    Private _pageViewModels As List(Of IPageViewModel)
    Public ReadOnly Property PageViewModels As List(Of IPageViewModel)
        Get
            If _pageViewModels Is Nothing Then _pageViewModels = New List(Of IPageViewModel)()
            Return _pageViewModels
        End Get
    End Property

    Private _currentPageViewModel As IPageViewModel
    Public Property CurrentPageViewModel As IPageViewModel
        Get
            Return _currentPageViewModel
        End Get
        Set
            SetProperty(_currentPageViewModel, Value)
        End Set
    End Property

    Private _DatenGeladen As Boolean
    Public Property DatenGeladen As Boolean
        Get
            Return _DatenGeladen
        End Get
        Set
            SetProperty(_DatenGeladen, Value)
        End Set
    End Property
#End Region

#Region "ICommand"
    Public Property SaveCommand As RelayCommand
    Public Property CancelCommand As RelayCommand
    Public Property UndoCommand As RelayCommand
    Public Property LoadedCommand As RelayCommand
    Public Property NavigateCommand As RelayCommand

#End Region

    Public Sub New()
        ' Commands
        SaveCommand = New RelayCommand(AddressOf Save)
        CancelCommand = New RelayCommand(AddressOf Cancel)
        UndoCommand = New RelayCommand(AddressOf Undo)

        ' Window Command
        LoadedCommand = New RelayCommand(AddressOf LadeDaten)
        NavigateCommand = New RelayCommand(AddressOf Navigate)

        ' Interface
        DatenService = New OptionenService

        ' Chield Views
        With PageViewModels
            .Add(New OptBaseViewModel())
            .Add(New OptAnrMonViewModel())
            .Add(New OptDialerViewModel())
            .Add(New OptJournalViewModel())
            .Add(New OptSearchContactViewModel())
            .Add(New OptCreateContactViewModel())
            .Add(New OptTelephonyViewModel())
            .Add(New OptPhonerViewModel())
            .Add(New OptMicroSIPViewModel())
            .Add(New OptInfoViewModel())
            .Add(New OptTestViewModel())
        End With

        ' Lade die Grundeinstellungen
        Navigate(PageViewModels.First)

    End Sub

#Region "ICommand Callback"
    Private Sub Navigate(o As Object)
        If TypeOf o Is IPageViewModel Then

            ' Setze das gewählte ViewModel/View
            CurrentPageViewModel = CType(o, IPageViewModel)

            ' Weise dieses ViewModel zu
            CurrentPageViewModel.OptVM = Me

        End If
    End Sub

    Private Sub Save(o As Object)
        NLogger.Debug("User: Optionen Speichern")

        Speichern()

        CType(o, Window).Close()
    End Sub

    Private Sub Undo(o As Object)
        NLogger.Debug("User: Optionen Reset")

        LadeDaten(o)
    End Sub

    Private Sub Cancel(o As Object)
        NLogger.Debug("User: Optionen Cancel")
        CType(o, Window).Close()
    End Sub
#End Region

#Region "Laden/Speichern"
    ''' <summary>
    ''' Lädt die daten aus den <see cref="Optionen"/> in dieses Viewmodel.
    ''' </summary>
    Friend Async Sub LadeDaten(o As Object)
        NLogger.Debug("Lade die Daten aus der XML-Datei in das ViewModel Optionen")

        Dim T As Task = Task.Run(Sub()
                                     ' Schleife durch alle Properties dieser Klasse
                                     For Each ViewModelPropertyInfo As PropertyInfo In [GetType].GetProperties
                                         ' Suche das passende Property in den Optionen
                                         Dim OptionPropertyInfo As PropertyInfo = Array.Find(XMLData.POptionen.GetType.GetProperties, Function(PropertyInfo As PropertyInfo) PropertyInfo.Name.AreEqual(ViewModelPropertyInfo.Name))

                                         If OptionPropertyInfo IsNot Nothing Then
                                             Try
                                                 If ViewModelPropertyInfo.CanWrite Then
                                                     ViewModelPropertyInfo.SetValue(Me, OptionPropertyInfo.GetValue(XMLData.POptionen))
                                                     OnPropertyChanged(ViewModelPropertyInfo.Name)
                                                     NLogger.Trace($"Feld {ViewModelPropertyInfo.Name} mit Wert '{ViewModelPropertyInfo.GetValue(Me)}' geladen.")
                                                 End If
                                             Catch ex As Exception
                                                 NLogger.Error(ex, $"Fehler beim Laden des Feldes {ViewModelPropertyInfo.Name}.")
                                             End Try
                                         End If
                                     Next
                                 End Sub)

        ' Landes- und Ortskennzahl aus der Telefonie holen
        TBLandesKZ = XMLData.PTelefonie.LKZ
        TBOrtsKZ = XMLData.PTelefonie.OKZ

        ' Anrufmonitor Liste zu überwachender Telefonnummern
        TelNrListe = New ObservableCollectionEx(Of Telefonnummer)
        TelNrListe.AddRange(XMLData.PTelefonie.Telefonnummern)

        ' Telefoniegeräteliste
        TelGeräteListe = New ObservableCollectionEx(Of Telefoniegerät)
        TelGeräteListe.AddRange(XMLData.PTelefonie.Telefoniegeräte)

        ' Ornderliste überwachter Ordner
        OutlookOrdnerListe = New OutlookOrdnerListe
        OutlookOrdnerListe.AddRange(XMLData.POptionen.OutlookOrdner.OrdnerListe)

        Await T
        ' Fritz!Box Benutzer laden
        CBoxBenutzer = DatenService.LadeFBoxUser(TBFBAdr)

        ' Aktiviere die Eingabemaske, nachdem alle Daten geladen wurden
        DatenGeladen = True
        NLogger.Debug("Die Daten aus der XML-Datei wurden in das ViewModel Optionen geladen.")
    End Sub

    ''' <summary>
    ''' Speichert die Daten aus diesem ViewModel zurück in die <see cref="Optionen"/>.
    ''' </summary>
    Friend Async Sub Speichern()
        NLogger.Debug("Speichere die Daten aus dem ViewModel Optionen in die XML-Datei")

        Dim TaskList As New List(Of Task) From {
                    Task.Run(Sub()
                                 ' Schleife durch alle Properties dieser Klasse
                                 For Each ViewModelPropertyInfo As PropertyInfo In [GetType].GetProperties
                                     ' Suche das passende Property in den Optionen
                                     Dim OptionPropertyInfo As PropertyInfo = Array.Find(XMLData.POptionen.GetType.GetProperties, Function(PropertyInfo As PropertyInfo) PropertyInfo.Name.AreEqual(ViewModelPropertyInfo.Name))

                                     If OptionPropertyInfo IsNot Nothing Then

                                         OptionPropertyInfo.SetValue(XMLData.POptionen, ViewModelPropertyInfo.GetValue(Me))
                                         NLogger.Trace($"Feld {ViewModelPropertyInfo.Name} mit Wert '{ViewModelPropertyInfo.GetValue(Me)}' geschrieben.")

                                     End If
                                 Next
                             End Sub)}

        ' Landes- und Ortskennzahl in die Telefonie schreiben
        XMLData.PTelefonie.LKZ = TBLandesKZ
        XMLData.PTelefonie.OKZ = TBOrtsKZ

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

        ' Ordnerliste überwachter Ordner
        With XMLData.POptionen.OutlookOrdner

            ' Lösche alle nichtmehr existenten Ordner
            .ClearNotExisting()
            OutlookOrdnerListe.ClearNotExisting()

            ' deindiziere:
            Dim MAPIFolderList As List(Of Outlook.MAPIFolder) = .FindAll(OutlookOrdnerVerwendung.KontaktSuche).Except(OutlookOrdnerListe.FindAll(OutlookOrdnerVerwendung.KontaktSuche)).Select(Function(S) S.MAPIFolder).ToList

            ' Füge die Unterordner hinzu
            If CBSucheUnterordner Then AddChildFolders(MAPIFolderList, Outlook.OlItemType.olContactItem)

            For Each Folder In MAPIFolderList
                NLogger.Debug($"Deindiziere Odner {Folder.Name}")
                TaskList.Add(Task.Run(Sub() DatenService.Indexer(Folder, False, CBSucheUnterordner)))
            Next

            ' indiziere:
            MAPIFolderList = OutlookOrdnerListe.FindAll(OutlookOrdnerVerwendung.KontaktSuche).Except(.FindAll(OutlookOrdnerVerwendung.KontaktSuche)).Select(Function(S) S.MAPIFolder).ToList

            ' Füge die Unterordner hinzu
            If CBSucheUnterordner Then AddChildFolders(MAPIFolderList, Outlook.OlItemType.olContactItem)

            For Each Folder In MAPIFolderList
                NLogger.Debug($"Indiziere Odner {Folder.Name}")
                TaskList.Add(Task.Run(Sub() DatenService.Indexer(Folder, True, CBSucheUnterordner)))
            Next

        End With

        XMLData.POptionen.OutlookOrdner = OutlookOrdnerListe

        ' Loglevel Aktualisieren
        SetLogLevel()

        ' Speichern in Datei anstoßen
        Serializer.Speichern(XMLData, IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, $"{My.Resources.strDefShortName}.xml"))

        Await Task.WhenAll(TaskList)
    End Sub
#End Region
End Class
