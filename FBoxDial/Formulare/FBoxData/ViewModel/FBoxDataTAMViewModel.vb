Public Class FBoxDataTAMViewModel
    Inherits NotifyBase
    Implements IFBoxData

    Private Property DatenService As IFBoxDataService
    Private Property DialogService As IDialogService

    Public ReadOnly Property Name As String Implements IFBoxData.Name
        Get
            Return Localize.LocFBoxData.strTAM
        End Get
    End Property

    Private _FBoxDataVM As FBoxDataViewModel
    Public Property FBoxDataVM As FBoxDataViewModel Implements IFBoxData.FBoxDataVM
        Get
            Return _FBoxDataVM
        End Get
        Set(value As FBoxDataViewModel)
            SetProperty(_FBoxDataVM, value)
        End Set
    End Property

    Public Property InitialSelected As Boolean = False Implements IFBoxData.InitialSelected

#Region "Listen"
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
    Public Sub New(dataService As IFBoxDataService, dialogService As IDialogService)
        ' Interface
        _DatenService = dataService
        _DialogService = dialogService
    End Sub
    Private Async Sub Init() Implements IFBoxData.Init
        TAMListe = New ObservableCollectionEx(Of TAMItemViewModel)

        Dim ABs As TAMList = Await DatenService.GetTAMList

        TAMListe.AddRange(ABs.TAMListe.Select(Function(TAM) New TAMItemViewModel(DatenService, DialogService, TAM)))
    End Sub
End Class
