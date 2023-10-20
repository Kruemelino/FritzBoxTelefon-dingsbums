Public Class OptConnPhonerLiteViewModel
    Inherits NotifyBase
    Implements IConnectorVM

    Private Property DatenService As IOptionenService
    Private Property DialogService As IDialogService
    Public Property PhonerLitePathCommand As RelayCommand

    Public Sub New(dataService As IOptionenService, dialogService As IDialogService)
        ' Interface
        _DatenService = dataService
        _DialogService = dialogService
        ' Commands
        PhonerLitePathCommand = New RelayCommand(AddressOf GetPhonerLitePath)
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
            Return resEnum.IPConnPhonerLite
        End Get
    End Property

    Public Property Connector As New IPPhoneConnector With {.Type = IPPhoneConnectorType.PhonerLite} Implements IConnectorVM.Connector

    Private Sub GetPhonerLitePath(o As Object)
        ' Initialen Pfad ermitteln
        If Connector.ConnectionUriCall.IsStringNothingOrEmpty Then Connector.ConnectionUriCall = PhonerLiteGetExecutablePath()

        Dim Dateipfad As String = DialogService.OpenFile("PhonerLite.exe (.exe)|*.exe", Connector.ConnectionUriCall)
        If Dateipfad.IsNotStringNothingOrEmpty Then Connector.ConnectionUriCall = Dateipfad
    End Sub
End Class
