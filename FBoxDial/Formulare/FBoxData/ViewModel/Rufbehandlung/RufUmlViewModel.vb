Public Class RufUmlViewModel
    Inherits NotifyBase
    'Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IFBoxDataService
    'Private Property DialogService As IDialogService

#Region "Model"
    Public Property Deflection As FBoxAPI.Deflection
#End Region
    Private _Enable As Boolean
    Public Property Enable As Boolean
        Get
            Return _Enable
        End Get
        Set
            SetProperty(_Enable, Value)
        End Set
    End Property

    Public ReadOnly Property Type As TypeEnum
        Get
            [Enum].TryParse(Deflection.Type.ToString, Type)
        End Get
    End Property

    Public ReadOnly Property Mode As ModeEnum
        Get
            [Enum].TryParse(Deflection.Mode.ToString, Mode)
        End Get
    End Property

    Public ReadOnly Property TelefonbuchName As String
        Get
            Return If(Type = TypeEnum.fromPB, DatenService.GetTelefonbuchName(Deflection.PhonebookID.ToInt), Nothing)
        End Get
    End Property

    Public ReadOnly Property IsFromPB As Boolean
        Get
            Return Type = TypeEnum.fromPB
        End Get
    End Property
#Region "ICommand"
    Public Property ToggleCommand As RelayCommand
#End Region

    Public Sub New(dataService As IFBoxDataService)

        ' Interface
        _DatenService = dataService
        '_DialogService = dialogService

        ' Commands
        ToggleCommand = New RelayCommand(AddressOf ToggleRufumleitung)
    End Sub

    Private Sub ToggleRufumleitung(o As Object)
        DatenService.ToggleRufuml(Deflection)
    End Sub
End Class
