Public Class FBoxDataViewModel
    Inherits NotifyBase
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IFBoxDataService
    Private Property DialogService As IDialogService
#Region "Window Eigenschaften"

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


#End Region

#Region "ICommand"
    Public Property LoadedCommand As RelayCommand
    Public Property ClosedCommand As RelayCommand
    Public Property NavigateCommand As RelayCommand
#End Region
    Public Sub New()
        ' Window Command
        LoadedCommand = New RelayCommand(AddressOf LadeDaten)
        ClosedCommand = New RelayCommand(AddressOf EntladeDaten)
        NavigateCommand = New RelayCommand(AddressOf Navigate)

        ' Interface
        DatenService = New FBoxDataService
        DialogService = New DialogService

        ' Child Views
        With PageViewModels
            .Add(New FBoxDataAnrListViewModel(DatenService, DialogService) With {.FBoxDataVM = Me})
            .Add(New FBoxDataTAMViewModel(DatenService, DialogService) With {.FBoxDataVM = Me})
            .Add(New FBoxDataTelBuchViewModel(DatenService, DialogService) With {.FBoxDataVM = Me})
            .Add(New FBoxDataRufUmlViewModel(DatenService) With {.FBoxDataVM = Me})
            .Add(New FBoxDataTellowsViewModel(DatenService) With {.FBoxDataVM = Me})
        End With

        ' Lade die Grundeinstellungen
        Navigate(PageViewModels.First)

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
        PageViewModels.ForEach(Sub(P) P.Init())

        ' Aktiviere die Eingabemaske, nachdem alle Daten geladen wurden
        DatenGeladen = True
    End Sub

    Friend Sub EntladeDaten(o As Object)

        DatenService.Finalize()

        ' Deaktiviere die Eingabemaske, nachdem alle Daten geladen wurden
        DatenGeladen = False
    End Sub
#End Region

End Class
