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

    Private _CBoxBenutzer As ObservableCollectionEx(Of FBoxAPI.User)
    Public Property CBoxBenutzer As ObservableCollectionEx(Of FBoxAPI.User)
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
    Public ReadOnly Property DfltDeCryptKey As String = My.Resources.strDfltDeCryptKey
    Public ReadOnly Property DfltIPPhoneDeCryptKey As String = My.Resources.strDfltIPPhoneDeCryptKey
    Public ReadOnly Property DfltPhonerDeCryptKey As String = My.Resources.strDfltPhonerDeCryptKey
    Public ReadOnly Property DfltTellowsDeCryptKey As String = My.Resources.strDfltTellowsDeCryptKey
#End Region

#Region "Grunddaten Telefonie"
    Private _TBOrtsKZ As String
    Public Property TBOrtsKZ As String
        Get
            Return _TBOrtsKZ
        End Get
        Set
            SetProperty(_TBOrtsKZ, Value)
        End Set
    End Property

    Private _TBLandesKZ As String
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
    Public Property TBTelNrMaske As String
        Get
            Return _TBTelNrMaske
        End Get
        Set
            SetProperty(_TBTelNrMaske, Value)
        End Set
    End Property

    Private _CBTelNrGruppieren As Boolean
    Public Property CBTelNrGruppieren As Boolean
        Get
            Return _CBTelNrGruppieren
        End Get
        Set
            SetProperty(_CBTelNrGruppieren, Value)
        End Set
    End Property

    Private _CBintl As Boolean
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
    Public Property CBAnrMonAuto As Boolean
        Get
            Return _CBAnrMonAuto
        End Get
        Set
            SetProperty(_CBAnrMonAuto, Value)
        End Set
    End Property

    Private _CBAutoClose As Boolean
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

    Private _TBEnblDauer As Integer
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

    Private _CBAnrMonHideCONNECT As Boolean
    ''' <summary>
    ''' Angabe, ob der Anrufmonitor bei Rufannahme ausgeblendet werden soll.
    ''' </summary>
    Public Property CBAnrMonHideCONNECT As Boolean
        Get
            Return _CBAnrMonHideCONNECT
        End Get
        Set
            SetProperty(_CBAnrMonHideCONNECT, Value)
        End Set
    End Property

    Private _CBAnrMonZeigeKontakt As Boolean
    ''' <summary>
    ''' Angabe, ob der Kontakt angezeigt werden soll
    ''' </summary>
    Public Property CBAnrMonZeigeKontakt As Boolean
        Get
            Return _CBAnrMonZeigeKontakt
        End Get
        Set
            SetProperty(_CBAnrMonZeigeKontakt, Value)
        End Set
    End Property

    Private _CBIsTAMMissed As Boolean = True
    ''' <summary>
    ''' Angabe, ob Anrufe, die an einen Anrufbeantworter gegangen sind, als verpasst behandelt werden sollen.
    ''' </summary>
    Public Property CBIsTAMMissed As Boolean
        Get
            Return _CBIsTAMMissed
        End Get
        Set
            SetProperty(_CBIsTAMMissed, Value)
        End Set
    End Property

    Private _CBAnrMonBlockNr As Boolean
    ''' <summary>
    ''' Angabe, ob der Anrufmonitor eingeblendet werden soll, falls sich der Anrufer auf der Sperrliste befindet
    ''' </summary>
    Public Property CBAnrMonBlockNr As Boolean
        Get
            Return _CBAnrMonBlockNr
        End Get
        Set
            SetProperty(_CBAnrMonBlockNr, Value)
        End Set
    End Property

    Private _CBAnrMonContactImage As Boolean
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

    Private _CBSetAnrMonBColor As Boolean
    ''' <summary>
    ''' Angabe, ob die Farben des Anrufmonitors geändert werden soll
    ''' </summary>
    Public Property CBSetAnrMonBColor As Boolean
        Get
            Return _CBSetAnrMonBColor
        End Get
        Set
            SetProperty(_CBSetAnrMonBColor, Value)
        End Set
    End Property

    Private _TBAnrMonBColor As Media.Color
    ''' <summary>
    ''' Hintergrundfarbe als Farbobjekt
    ''' </summary>
    Public Property TBAnrMonBColor As Media.Color
        Get
            Return _TBAnrMonBColor
        End Get
        Set
            SetProperty(_TBAnrMonBColor, Value)
        End Set
    End Property
    ''' <summary>
    ''' Schriftfarbe als HEX-String #00000000
    ''' </summary>
    Public Property TBAnrMonBColorHex As String
        Get
            Return _TBAnrMonBColor.ToString
        End Get
        Set
            SetProperty(_TBAnrMonBColor, CType(Media.ColorConverter.ConvertFromString(Value), Media.Color))
        End Set
    End Property

    Private _TBAnrMonFColor As Media.Color
    ''' <summary>
    ''' Schriftfarbe als Farbobjekt
    ''' </summary>
    Public Property TBAnrMonFColor As Media.Color
        Get
            Return _TBAnrMonFColor
        End Get
        Set
            SetProperty(_TBAnrMonFColor, Value)
        End Set
    End Property

    ''' <summary>
    ''' Schriftfarbe als HEX-String #00000000
    ''' </summary>
    Public Property TBAnrMonFColorHex As String
        Get
            Return _TBAnrMonFColor.ToString
        End Get
        Set
            SetProperty(_TBAnrMonFColor, CType(Media.ColorConverter.ConvertFromString(Value), Media.Color))
        End Set
    End Property

    Private _TBAnrMonModPosX As Double
    ''' <summary>
    ''' Positionskorrektur des Anrufmonitors in X-Richtung
    ''' </summary>
    Public Property TBAnrMonModPosX As Double
        Get
            Return _TBAnrMonModPosX
        End Get
        Set
            SetProperty(_TBAnrMonModPosX, Value)
        End Set
    End Property

    Private _TBAnrMonModPosY As Double
    ''' <summary>
    ''' Positionskorrektur des Anrufmonitors in Y-Richtung
    ''' </summary>
    Public Property TBAnrMonModPosY As Double
        Get
            Return _TBAnrMonModPosY
        End Get
        Set
            SetProperty(_TBAnrMonModPosY, Value)
        End Set
    End Property

    Private _TBAnrMonAbstand As Double
    ''' <summary>
    ''' Grundabstand des Anrufmonitors
    ''' </summary>
    Public Property TBAnrMonAbstand As Double
        Get
            Return _TBAnrMonAbstand
        End Get
        Set
            SetProperty(_TBAnrMonAbstand, Value)
        End Set
    End Property

    Private _CBShowMissedCallPane As Boolean
    ''' <summary>
    ''' Angabe, ob verpasste Anrufe im CallPane angezeigt werden sollen.
    ''' </summary>
    Public Property CBShowMissedCallPane As Boolean
        Get
            Return _CBShowMissedCallPane
        End Get
        Set
            SetProperty(_CBShowMissedCallPane, Value)
        End Set
    End Property

    Private _CBCloseEmptyCallPane As Boolean
    ''' <summary>
    ''' Angabe, ob das CallPane automatisch geschlossen werden soll, wenn Anrufliste leer ist.
    ''' </summary>
    Public Property CBCloseEmptyCallPane As Boolean
        Get
            Return _CBCloseEmptyCallPane
        End Get
        Set
            SetProperty(_CBCloseEmptyCallPane, Value)
        End Set
    End Property

    Private _CBClearCallPaneAtClose As Boolean
    ''' <summary>
    ''' Angabe, ob beim Schließen des CallPane alle enthaltenen Anrufe entfernt werden sollen.
    ''' </summary>
    Public Property CBClearCallPaneAtClose As Boolean
        Get
            Return _CBClearCallPaneAtClose
        End Get
        Set
            SetProperty(_CBClearCallPaneAtClose, Value)
        End Set
    End Property

    Private _TBCallPaneStartWidth As Integer
    ''' <summary>
    ''' Gibt die Standardbreite des Pane bei Start an.
    ''' </summary>
    Public Property TBCallPaneStartWidth As Integer
        Get
            Return _TBCallPaneStartWidth
        End Get
        Set
            SetProperty(_TBCallPaneStartWidth, Value)
        End Set
    End Property

    Private _CBShowCallPaneAtStart As Boolean
    ''' <summary>
    ''' Angabe, ob Seiten Fenster bei Outlookstart bereits eingeblendet werden soll.
    ''' </summary>
    Public Property CBShowCallPaneAtStart As Boolean
        Get
            Return _CBShowCallPaneAtStart
        End Get
        Set
            SetProperty(_CBShowCallPaneAtStart, Value)
        End Set
    End Property
#End Region

#Region "Stoppuhr"
    Private _CBStoppUhrEinblenden As Boolean
    ''' <summary>
    ''' Angabe, ob die Stoppuhr angezeigt werden soll
    ''' </summary>
    Public Property CBStoppUhrEinblenden As Boolean
        Get
            Return _CBStoppUhrEinblenden
        End Get
        Set
            SetProperty(_CBStoppUhrEinblenden, Value)
        End Set
    End Property

    Private _CBStoppUhrAusblenden As Boolean
    ''' <summary>
    ''' Angabe, ob die Stoppuhr nach dem Telefonat automatisch ausgeblendet werden soll
    ''' </summary>
    Public Property CBStoppUhrAusblenden As Boolean
        Get
            Return _CBStoppUhrAusblenden
        End Get
        Set
            SetProperty(_CBStoppUhrAusblenden, Value)
        End Set
    End Property

    Private _TBStoppUhrAusblendverzögerung As Integer
    ''' <summary>
    ''' Zeitangabe, nachdem die Stoppuhr ausgeblendet werden soll. (Korresbondiert zu <see cref="CBStoppUhrAusblenden"/>)
    ''' </summary>
    Public Property TBStoppUhrAusblendverzögerung As Integer
        Get
            Return _TBStoppUhrAusblendverzögerung
        End Get
        Set
            SetProperty(_TBStoppUhrAusblendverzögerung, Value)
        End Set
    End Property

    Private _CBSetStoppUhrBColor As Boolean
    ''' <summary>
    ''' Angabe, ob die Farben der Stoppuhr geändert werden soll
    ''' </summary>
    Public Property CBSetStoppUhrBColor As Boolean
        Get
            Return _CBSetStoppUhrBColor
        End Get
        Set
            SetProperty(_CBSetStoppUhrBColor, Value)
        End Set
    End Property

    Private _TBStoppUhrBColor As Media.Color
    ''' <summary>
    ''' Hintergrundfarbe als Farbobjekt
    ''' </summary>
    Public Property TBStoppUhrBColor As Media.Color
        Get
            Return _TBStoppUhrBColor
        End Get
        Set
            SetProperty(_TBStoppUhrBColor, Value)
        End Set
    End Property

    ''' <summary>
    ''' Hintergrundfarbe als HEX-String #00000000
    ''' </summary>
    Public Property TBStoppUhrBColorHex As String
        Get
            Return _TBStoppUhrBColor.ToString
        End Get
        Set
            SetProperty(_TBStoppUhrBColor, CType(Media.ColorConverter.ConvertFromString(Value), Media.Color))
        End Set
    End Property

    Private _TBStoppUhrFColor As Media.Color
    ''' <summary>
    ''' Schriftfarbe als Farbobjekt
    ''' </summary>
    Public Property TBStoppUhrFColor As Media.Color
        Get
            Return _TBStoppUhrFColor
        End Get
        Set
            SetProperty(_TBStoppUhrFColor, Value)
        End Set
    End Property

    ''' <summary>
    ''' Schriftfarbe als HEX-String #00000000
    ''' </summary>
    Public Property TBStoppUhrFColorHex As String
        Get
            Return _TBStoppUhrFColor.ToString
        End Get
        Set
            SetProperty(_TBStoppUhrFColor, CType(Media.ColorConverter.ConvertFromString(Value), Media.Color))
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
    Public Property CBRWS As Boolean
        Get
            Return _CBRWS
        End Get
        Set
            SetProperty(_CBRWS, Value)
        End Set
    End Property

    Private _CBKErstellen As Boolean
    Public Property CBKErstellen As Boolean
        Get
            Return _CBKErstellen
        End Get
        Set
            SetProperty(_CBKErstellen, Value)
        End Set
    End Property

    Private _CBRWSIndex As Boolean
    Public Property CBRWSIndex As Boolean
        Get
            Return _CBRWSIndex
        End Get
        Set
            SetProperty(_CBRWSIndex, Value)
        End Set
    End Property

    Private _CBNoContactNotes As Boolean
    Public Property CBNoContactNotes As Boolean
        Get
            Return _CBNoContactNotes
        End Get
        Set
            SetProperty(_CBNoContactNotes, Value)
        End Set
    End Property

#End Region

#Region "Einstellungen für die Kontaktsuche - tellows"
    Private _TBTellowsAPIKey As String
    Public Property TBTellowsAPIKey As String
        Get
            Return _TBTellowsAPIKey
        End Get
        Set
            SetProperty(_TBTellowsAPIKey, Value)
        End Set
    End Property

    Private _CBTellows As Boolean
    Public Property CBTellows As Boolean
        Get
            Return _CBTellows
        End Get
        Set
            SetProperty(_CBTellows, Value)
        End Set
    End Property

    Private _CBTellowsAnrMonMinScore As Integer
    Public Property CBTellowsAnrMonMinScore As Integer
        Get
            Return _CBTellowsAnrMonMinScore
        End Get
        Set
            SetProperty(_CBTellowsAnrMonMinScore, Value)
        End Set
    End Property

    Private _CBTellowsAnrMonMinComments As Integer
    Public Property CBTellowsAnrMonMinComments As Integer
        Get
            Return _CBTellowsAnrMonMinComments
        End Get
        Set
            SetProperty(_CBTellowsAnrMonMinComments, Value)
        End Set
    End Property

    Private _CBTellowsEntryNumberCount As Integer
    Public Property CBTellowsEntryNumberCount As Integer
        Get
            Return _CBTellowsEntryNumberCount
        End Get
        Set
            SetProperty(_CBTellowsEntryNumberCount, Value)
        End Set
    End Property

    Private _CBTellowsAnrMonColor As Boolean
    Public Property CBTellowsAnrMonColor As Boolean
        Get
            Return _CBTellowsAnrMonColor
        End Get
        Set
            SetProperty(_CBTellowsAnrMonColor, Value)
        End Set
    End Property

    Private _CBTellowsAutoFBBlockList As Boolean
    Public Property CBTellowsAutoFBBlockList As Boolean
        Get
            Return _CBTellowsAutoFBBlockList
        End Get
        Set
            SetProperty(_CBTellowsAutoFBBlockList, Value)
        End Set
    End Property

    Private _CBTellowsAutoUpdateScoreList As Boolean
    Public Property CBTellowsAutoUpdateScoreList As Boolean
        Get
            Return _CBTellowsAutoUpdateScoreList
        End Get
        Set
            SetProperty(_CBTellowsAutoUpdateScoreList, Value)
        End Set
    End Property

    Private _CBTellowsAutoScoreFBBlockList As Integer
    Public Property CBTellowsAutoScoreFBBlockList As Integer
        Get
            Return _CBTellowsAutoScoreFBBlockList
        End Get
        Set
            SetProperty(_CBTellowsAutoScoreFBBlockList, Value)
        End Set
    End Property

    Public ReadOnly Property CBoxTellowsScore As IEnumerable(Of Integer) = {1, 2, 3, 4, 5, 6, 7, 8, 9}
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
    Private _CBJournalBlockNr As Boolean
    Public Property CBJournalBlockNr As Boolean
        Get
            Return _CBJournalBlockNr
        End Get
        Set
            SetProperty(_CBJournalBlockNr, Value)
        End Set
    End Property
#End Region

#Region "Auswertung der Fritz!box Anrufliste - Auswertung der Anrufliste"
    Private _CBAutoAnrList As Boolean
    Public Property CBAutoAnrList As Boolean
        Get
            Return _CBAutoAnrList
        End Get
        Set
            SetProperty(_CBAutoAnrList, Value)
        End Set
    End Property

    Private _CBAnrListeUpdateCallLists As Boolean
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

#Region "Telefoniedaten"
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

    Private _TelGerListe As ObservableCollectionEx(Of Telefoniegerät)
    Public Property TelGeräteListe As ObservableCollectionEx(Of Telefoniegerät)
        Get
            Return _TelGerListe
        End Get
        Set
            SetProperty(_TelGerListe, Value)
        End Set
    End Property

    Private _IPPhoneConnectorList As ObservableCollectionEx(Of IPPhoneConnector)
    Public Property IPPhoneConnectorList As ObservableCollectionEx(Of IPPhoneConnector)
        Get
            Return _IPPhoneConnectorList
        End Get
        Set
            SetProperty(_IPPhoneConnectorList, Value)
        End Set
    End Property
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

#Region "Tweaks"
    Private _CBKeyboard As Boolean
    Public Property CBKeyboard As Boolean
        Get
            Return _CBKeyboard
        End Get
        Set
            SetProperty(_CBKeyboard, Value)
        End Set
    End Property

    Private _CBKeyboardModifierShift As Boolean
    Public Property CBKeyboardModifierShift As Boolean
        Get
            Return _CBKeyboardModifierShift
        End Get
        Set
            SetProperty(_CBKeyboardModifierShift, Value)
        End Set
    End Property

    Private _CBKeyboardModifierControl As Boolean
    Public Property CBKeyboardModifierControl As Boolean
        Get
            Return _CBKeyboardModifierControl
        End Get
        Set
            SetProperty(_CBKeyboardModifierControl, Value)
        End Set
    End Property

    Private _CBDisableMailCheck As Boolean
    Public Property CBDisableMailCheck As Boolean
        Get
            Return _CBDisableMailCheck
        End Get
        Set
            SetProperty(_CBDisableMailCheck, Value)
        End Set
    End Property

    Private _TBNetworkTimeout As Integer
    Public Property TBNetworkTimeout As Integer
        Get
            Return _TBNetworkTimeout
        End Get
        Set
            SetProperty(_TBNetworkTimeout, Value)
        End Set
    End Property

#End Region

#Region "Design"
    Private _CBoxDesignMode As DesignModes
    Public Property CBoxDesignMode As DesignModes
        Get
            Return _CBoxDesignMode
        End Get
        Set
            SetProperty(_CBoxDesignMode, Value)
        End Set
    End Property
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

        ' Theme
        DatenService.UpdateTheme()

        ' Child ViewModel
        With PageViewModels
            .Add(New OptBaseViewModel(DatenService))
            .Add(New OptAnrMonViewModel())
            .Add(New OptDialerViewModel())
            .Add(New OptJournalViewModel(DatenService))
            .Add(New OptSearchContactViewModel(DatenService))
            .Add(New OptCreateContactViewModel(DatenService))
            .Add(New OptTelephonyViewModel(DatenService))
            .Add(New OptIPPhonesViewModel(DatenService))
            .Add(New OptTellowsViewModel(DatenService))
            .Add(New OptInfoViewModel())
            .Add(New OptTestViewModel(DatenService))
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

        Dim LadeTask As Task = Task.Run(Sub()
                                            ' Schleife durch alle Properties dieser Klasse
                                            For Each ViewModelPropertyInfo As PropertyInfo In [GetType].GetProperties
                                                ' Suche das passende Property in den Optionen
                                                Dim OptionPropertyInfo As PropertyInfo = Array.Find(XMLData.POptionen.GetType.GetProperties, Function(PropertyInfo As PropertyInfo) PropertyInfo.Name.IsEqual(ViewModelPropertyInfo.Name))

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

        ' IPPhoneConnectoren
        IPPhoneConnectorList = New ObservableCollectionEx(Of IPPhoneConnector)
        IPPhoneConnectorList.AddRange(XMLData.PTelefonie.IPTelefone)

        ' Ornderliste überwachter Ordner
        OutlookOrdnerListe = New OutlookOrdnerListe
        OutlookOrdnerListe.AddRange(XMLData.POptionen.OutlookOrdner.OrdnerListe)

        Await LadeTask

        ' Fritz!Box Benutzer laden
        CBoxBenutzer = DatenService.LadeFBoxUser()

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
                                                                 Dim OptionPropertyInfo As PropertyInfo = Array.Find(XMLData.POptionen.GetType.GetProperties,
                                                                                                                     Function(PropertyInfo As PropertyInfo) PropertyInfo.Name.IsEqual(ViewModelPropertyInfo.Name))

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

        ' IPPhoneConnectoren
        With XMLData.PTelefonie.IPTelefone
            ' Die Telefoniegeräte in den Optionen löschen
            .Clear()
            ' Die Telefoniegeräte aus den Viewmodel setzen
            .AddRange(IPPhoneConnectorList)
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
            TaskList.Add(Task.Run(Sub() DatenService.Indexer(MAPIFolderList, False, Nothing, Nothing)))

            ' indiziere:
            MAPIFolderList = OutlookOrdnerListe.FindAll(OutlookOrdnerVerwendung.KontaktSuche).Except(.FindAll(OutlookOrdnerVerwendung.KontaktSuche)).Select(Function(S) S.MAPIFolder).ToList

            ' Füge die Unterordner hinzu
            If CBSucheUnterordner Then AddChildFolders(MAPIFolderList, Outlook.OlItemType.olContactItem)
            TaskList.Add(Task.Run(Sub() DatenService.Indexer(MAPIFolderList, True, Nothing, Nothing)))

        End With

        With XMLData.POptionen
            .OutlookOrdner = OutlookOrdnerListe

            ' Loglevel Aktualisieren
            SetLogLevel(.CBoxMinLogLevel)
        End With

        ' Anmeldeinformationen für Fritz!Box aktualisieren
        Globals.ThisAddIn.FBoxTR064.UpdateCredential(FritzBoxDefault.Anmeldeinformationen)

        ' Tastenkombination setzen
        Globals.ThisAddIn.SetupKeyboardHooking()

        ' Speichern in Datei anstoßen
        XmlSerializeToFile(XMLData, IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, $"{My.Resources.strDefShortName}.xml"))

        Await Task.WhenAll(TaskList)
    End Sub
#End Region
End Class
