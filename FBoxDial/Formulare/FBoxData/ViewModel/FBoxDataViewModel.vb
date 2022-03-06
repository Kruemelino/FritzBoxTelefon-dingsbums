Imports System.Threading.Tasks
Public Class FBoxDataViewModel
    Inherits NotifyBase
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IFBoxDataService
    Private Property DialogService As IDialogService

#Region "Window Eigenschaften"

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

#Region "ViewModel"
    Private _pageViewModels As List(Of IFBoxData)
    Public ReadOnly Property PageViewModels As List(Of IFBoxData)
        Get
            If _pageViewModels Is Nothing Then _pageViewModels = New List(Of IFBoxData)()
            Return _pageViewModels
        End Get
    End Property

    Private _currentPageViewModel As IFBoxData
    Public Property CurrentPageViewModel As IFBoxData
        Get
            Return _currentPageViewModel
        End Get
        Set
            SetProperty(_currentPageViewModel, Value)
        End Set
    End Property
#End Region

#Region "ICommand"
    Public Property LoadedCommand As RelayCommand
    Public Property ClosedCommand As RelayCommand
    Public Property NavigateCommand As RelayCommand
#End Region

    Public Sub New()
        DatenGeladen = False

        ' Window Command
        LoadedCommand = New RelayCommand(AddressOf LadeDaten)
        ClosedCommand = New RelayCommand(AddressOf EntladeDaten)
        NavigateCommand = New RelayCommand(AddressOf Navigate)

        ' Interface
        DatenService = New FBoxDataService
        DialogService = New DialogService

        ' Themes
        DatenService.UpdateTheme()

        ' Child Views
        With PageViewModels
            .Add(New FBoxDataCallListViewModel(DatenService, DialogService) With {.FBoxDataVM = Me})
            .Add(New FBoxDataTAMViewModel(DatenService, DialogService) With {.FBoxDataVM = Me})
            .Add(New FBoxDataTelBuchViewModel(DatenService, DialogService) With {.FBoxDataVM = Me})
            .Add(New FBoxDataRufUmlViewModel(DatenService) With {.FBoxDataVM = Me})
            .Add(New FBoxDataTellowsViewModel(DatenService) With {.FBoxDataVM = Me})
        End With

    End Sub

#Region "ICommand Callback"
    Private Sub Navigate(o As Object)
        If TypeOf o Is IFBoxData Then

            ' Setze das gewählte ViewModel/View
            CurrentPageViewModel = CType(o, IFBoxData)

        End If
    End Sub

    Friend Sub LadeDaten(o As Object)

        ' Initiiere alle Pageviewmodels
        PageViewModels.ForEach(Sub(P)
                                   ' Debug: Zeitstempel merken
                                   P.DebugBeginnLadeDaten = Now

                                   ' Initiierung der Daten anstoßen
                                   P.Init()
                                   ' Lade die Grundeinstellungen
                                   If P.InitialSelected Then Navigate(P)
                               End Sub)

        ' Aktiviere die Eingabemaske, nachdem alle Daten geladen wurden
        DatenGeladen = True

        NLogger.Debug("Daten geladen")
    End Sub

    Friend Sub EntladeDaten(o As Object)

        DatenService.TR064HttpClient()

        ' Deaktiviere die Eingabemaske, nachdem alle Daten geladen wurden
        DatenGeladen = False
    End Sub
#End Region

End Class
