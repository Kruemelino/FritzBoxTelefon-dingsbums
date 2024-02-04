Public Class OptConnURIViewModel
    Inherits NotifyBase
    Implements IConnectorVM

    ' Private Property DatenService As IOptionenService
    ' Private Property DialogService As IDialogService

    Public Sub New() '(dataService As IOptionenService, dialogService As IDialogService)
        ' Interface
        ' _DatenService = dataService
        ' _DialogService = DialogService
        ' Commands

        ' Model
    End Sub


    Private _OptVM As OptionenViewModel
    Public Property OptVM As OptionenViewModel Implements IConnectorVM.OptVM
        Get
            Return _OptVM
        End Get
        Set
            SetProperty(_OptVM, Value)
        End Set
    End Property

    Public ReadOnly Property Name As String Implements IConnectorVM.Name
        Get
            Return resEnum.IPConnURI
        End Get
    End Property

    Public Property Connector As New IPPhoneConnector With {.Type = IPPhoneConnectorType.URI} Implements IConnectorVM.Connector

    Private Sub Init(C As IPPhoneConnector, O As OptionenViewModel) Implements IConnectorVM.Init
        Connector = C
        OptVM = O
    End Sub
End Class
