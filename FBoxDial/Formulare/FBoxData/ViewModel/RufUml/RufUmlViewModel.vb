Public Class RufUmlViewModel
    Inherits NotifyBase
    'Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IFBoxDataService
    'Private Property DialogService As IDialogService

    Private _CurrentRufUml As DeflectionInfo
    Public Property CurrentRufUml As DeflectionInfo
        Get
            Return _CurrentRufUml
        End Get
        Set
            SetProperty(_CurrentRufUml, Value)
        End Set
    End Property

#Region "ICommand"
    Public Property ToggleCommand As RelayCommand
#End Region

    Public Sub New(dataService As IFBoxDataService, cRufUml As DeflectionInfo)

        ' Interface
        _DatenService = dataService
        '_DialogService = dialogService

        ' Daten
        _CurrentRufUml = cRufUml

        ' Commands
        ToggleCommand = New RelayCommand(AddressOf ToggleRufumleitung)
    End Sub

    Private Sub ToggleRufumleitung(o As Object)
        DatenService.ToggleRufuml(CurrentRufUml)
    End Sub
End Class
