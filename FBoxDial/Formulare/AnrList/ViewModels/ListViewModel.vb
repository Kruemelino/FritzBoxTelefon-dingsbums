Imports System.Threading.Tasks

Public Class ListViewModel
    Inherits NotifyBase
    'Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IListService

#Region "Window Eigenschaften"

    Private _pageViewModels As List(Of IPageListViewModel)
    Public ReadOnly Property PageViewModels As List(Of IPageListViewModel)
        Get
            If _pageViewModels Is Nothing Then _pageViewModels = New List(Of IPageListViewModel)()
            Return _pageViewModels
        End Get
    End Property

    Private _currentPageViewModel As IPageListViewModel
    Public Property CurrentPageViewModel As IPageListViewModel
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

    Private _NutzeTellows As Boolean
    Public Property NutzeTellows As Boolean
        Get
            Return _NutzeTellows
        End Get
        Set
            SetProperty(_NutzeTellows, Value)
        End Set
    End Property
#End Region

#Region "Listen"
    ''' <summary>
    ''' Returns Or sets a list as FritzBoxXMLCall             
    ''' </summary>
    Private _CallList As ObservableCollectionEx(Of FritzBoxXMLCall)
    Public Property CallList As ObservableCollectionEx(Of FritzBoxXMLCall)
        Get
            Return _CallList
        End Get
        Set
            SetProperty(_CallList, Value)
        End Set
    End Property

    ''' <summary>
    ''' Returns Or sets a list as TellowsScoreListEntry             
    ''' </summary>
    Private _TellowsList As ObservableCollectionEx(Of TellowsScoreListEntry)
    Public Property TellowsList As ObservableCollectionEx(Of TellowsScoreListEntry)
        Get
            Return _TellowsList
        End Get
        Set
            SetProperty(_TellowsList, Value)
        End Set
    End Property
#End Region

#Region "ICommand"
    Public Property LoadedCommand As RelayCommand
    Public Property NavigateCommand As RelayCommand
#End Region

    Public Sub New()
        ' Interface
        DatenService = New ListService

        ' Window Command
        LoadedCommand = New RelayCommand(AddressOf LadeDaten)
        NavigateCommand = New RelayCommand(AddressOf Navigate)

        ' Child Views
        With PageViewModels
            .Add(New AnrListViewModel())
            .Add(New TellowsViewModel())
        End With

        ' Lade die erste Seite
        Navigate(PageViewModels.First)

    End Sub


#Region "ICommand Callback"
    Private Sub Navigate(o As Object)
        If TypeOf o Is IPageListViewModel Then

            ' Setze das gewählte ViewModel/View
            CurrentPageViewModel = CType(o, IPageListViewModel)

            ' Weise dieses ViewModel zu
            CurrentPageViewModel.ListVM = Me
        End If
    End Sub

    ''' <summary>
    ''' Tritt auf, wenn das Element ausgerichtet und gerendert sowie zur Interaktion vorbereitet wurde.
    ''' </summary>
    Private Async Sub LadeDaten(o As Object)
        Dim TaskAnrList As Task(Of FritzBoxXMLCallList) = DatenService.GetAnrufListe
        Dim TaskTellows As Task(Of List(Of TellowsScoreListEntry)) = DatenService.GetTellowsScoreList

        Await Task.WhenAll({TaskAnrList, TaskTellows})
        ' Initiiere die Anrufliste
        CallList = New ObservableCollectionEx(Of FritzBoxXMLCall)
        ' Lade die Anrufliste
        CallList.AddRange(TaskAnrList.Result?.Calls)

        ' Initiiere die Anrufliste
        TellowsList = New ObservableCollectionEx(Of TellowsScoreListEntry)
        ' Lade die tellows ScoreList
        TellowsList.AddRange(Await TaskTellows)

        NutzeTellows = TellowsList.Any

        ' Initiiere alle Pageviewmodels
        PageViewModels.ForEach(Sub(P) P.Init())
        ' Schalte das ContentControl frei
        DatenGeladen = True
    End Sub
#End Region
End Class
