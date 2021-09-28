Public Class TAMListViewModel
    Inherits NotifyBase
    'Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As ITAMService
#Region "ICommand"
    Public Property LoadedCommand As RelayCommand
#End Region
#Region "ViewModel"

    Private _TAMItemVM As TAMItemViewModel
    Public Property TAMItemVM As TAMItemViewModel
        Get
            Return _TAMItemVM
        End Get
        Set
            SetProperty(_TAMItemVM, Value)
        End Set
    End Property

    Private _TAMListe As ObservableCollectionEx(Of TAMItemViewModel)
    Public Property TAMListe As ObservableCollectionEx(Of TAMItemViewModel)
        Get
            Return _TAMListe
        End Get
        Set
            SetProperty(_TAMListe, Value)
        End Set
    End Property

#End Region

    Public Sub New()

        ' Interface
        DatenService = New TAMService

        ' Window Command
        LoadedCommand = New RelayCommand(AddressOf LadeDaten)

    End Sub

    Private Async Sub LadeDaten(obj As Object)
        TAMListe = New ObservableCollectionEx(Of TAMItemViewModel)

        Dim ABs As TAMList = Await DatenService.GetTAMList

        TAMListe.AddRange(ABs.TAMListe.Select(Function(TAM) New TAMItemViewModel(DatenService, TAM)))

    End Sub
End Class
