
Imports System.Threading

Public Class TellowsViewModel
    Inherits NotifyBase
    Implements IPageListViewModel
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IListService

#Region "Felder"
    Public ReadOnly Property Name As String Implements IPageListViewModel.Name
        Get
            Return Localize.LocAnrList.strTellows
        End Get
    End Property

    Private _ListVM As ListViewModel
    Public Property ListVM As ListViewModel Implements IPageListViewModel.ListVM
        Get
            Return _ListVM
        End Get
        Set
            SetProperty(_ListVM, Value)
        End Set
    End Property

    Public Property InitialSelected As Boolean = False Implements IPageListViewModel.InitialSelected

    Private _BlockProgressValue As Double
    Public Property BlockProgressValue As Double
        Get
            Return _BlockProgressValue
        End Get
        Set
            SetProperty(_BlockProgressValue, Value)
        End Set
    End Property

    Private _BlockProgressMax As Double
    Public Property BlockProgressMax As Double
        Get
            Return _BlockProgressMax
        End Get
        Set
            SetProperty(_BlockProgressMax, Value)
        End Set
    End Property

    Private _IsAktiv As Boolean = False
    Public Property IsAktiv As Boolean
        Get
            Return _IsAktiv
        End Get
        Set
            SetProperty(_IsAktiv, Value)
            OnPropertyChanged(NameOf(IsNotAktiv))
        End Set
    End Property

    Public ReadOnly Property IsNotAktiv As Boolean
        Get
            Return Not _IsAktiv
        End Get
    End Property
#End Region

#Region "Eigenschaften tellows"
    Private _CBoxTellowsScoreFBBlockList As Integer = XMLData.POptionen.CBTellowsAutoScoreFBBlockList
    Public Property CBoxTellowsScoreFBBlockList As Integer
        Get
            Return _CBoxTellowsScoreFBBlockList
        End Get
        Set
            SetProperty(_CBoxTellowsScoreFBBlockList, Value)
        End Set
    End Property

    Private _TBTellowsEntryNumberCount As Integer = 10
    Public Property TBTellowsEntryNumberCount As Integer
        Get
            Return _TBTellowsEntryNumberCount
        End Get
        Set
            SetProperty(_TBTellowsEntryNumberCount, Value)
        End Set
    End Property
    Public ReadOnly Property CBoxTellowsScore As IEnumerable(Of Integer) = {1, 2, 3, 4, 5, 6, 7, 8, 9}
#End Region

#Region "ICommand"
    Public Property CancelCommand As RelayCommand
    Public Property BlockCommand As RelayCommand
#End Region

#Region "Cancel"
    Private Property CTS As CancellationTokenSource
#End Region


    Public Sub New()
        ' Commands
        CancelCommand = New RelayCommand(AddressOf CancelProcess)
        BlockCommand = New RelayCommand(AddressOf BlockNumbers)

        ' Interface
        DatenService = New ListService

    End Sub

    Public Sub Init() Implements IPageListViewModel.Init

    End Sub

#Region "ICommand Callback"
    Private Async Sub BlockNumbers(o As Object)

        ' Starte die Indizierung
        BlockProgressValue = 0
        BlockProgressMax = 0

        ' Aktiv-Flag setzen
        IsAktiv = True

        ' Setze das Maximum
        BlockProgressMax = ListVM.TellowsList.Where(Function(Eintrag) Eintrag.Score.IsLargerOrEqual(CBoxTellowsScoreFBBlockList)).Count

        CTS = New CancellationTokenSource
        Dim progressIndicator = New Progress(Of Integer)(Sub(status) BlockProgressValue += status)

        Try
            ' Erstellung der Sperrliste in der Fritz!Box anstoßen
            Await DatenService.BlockTellowsNumbers(CBoxTellowsScoreFBBlockList, TBTellowsEntryNumberCount, ListVM.TellowsList, CTS.Token, progressIndicator)

        Catch ex As OperationCanceledException
            NLogger.Debug(ex)
        End Try

        If Not CTS.Token.IsCancellationRequested Then
            ' Progressbar auf Max setzen:
            BlockProgressValue = BlockProgressMax
        End If

        ' Aktiv-Flag setzen
        IsAktiv = False

        ' CancellationTokenSource auflösen
        CTS.Dispose()
    End Sub

    Private Sub CancelProcess(o As Object)
        CTS?.Cancel()
        NLogger.Info($"Übertragung der tellows Score Liste in die Fritz!Box Rufsperre abgebrochen.")
    End Sub
#End Region

End Class
