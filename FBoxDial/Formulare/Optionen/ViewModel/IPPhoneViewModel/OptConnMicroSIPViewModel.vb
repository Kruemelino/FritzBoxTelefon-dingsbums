Public Class OptConnMicroSIPViewModel
    Inherits NotifyBase
    Implements IConnectorVM

    Private Property DatenService As IOptionenService
    Private Property DialogService As IDialogService

    Public Property MicroSIPPathCommand As RelayCommand
    Public Sub New(dataService As IOptionenService, dialogService As IDialogService)
        ' Interface
        _DatenService = dataService
        _DialogService = dialogService
        ' Commands
        MicroSIPPathCommand = New RelayCommand(AddressOf GetMicroSIPPath)
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
            Return resEnum.IPConnMicroSIP
        End Get
    End Property

    Public Property Connector As New IPPhoneConnector With {.Type = IPPhoneConnectorType.MicroSIP,
                                                            .Passwort = String.Empty} Implements IConnectorVM.Connector

    Private Sub GetMicroSIPPath(o As Object)
        ' Initialen Pfad ermitteln
        If Connector.ConnectionUriCall.IsStringNothingOrEmpty Then Connector.ConnectionUriCall = MicroSIPGetExecutablePath()

        Dim Dateipfad As String = DialogService.OpenFile("MicroSIP.exe (.exe)|*.exe", Connector.ConnectionUriCall)
        If Dateipfad.IsNotStringNothingOrEmpty Then Connector.ConnectionUriCall = Dateipfad
    End Sub

End Class
