Public Class OptDialerViewModel
    Inherits NotifyBase
    Implements IPageViewModel

    Private Property DatenService As IOptionenService

    Private _OptVM As OptionenViewModel
    Public Property OptVM As OptionenViewModel Implements IPageViewModel.OptVM
        Get
            Return _OptVM
        End Get
        Set
            SetProperty(_OptVM, Value)
        End Set
    End Property

    Public ReadOnly Property Name As String Implements IPageViewModel.Name
        Get
            Return Localize.LocOptionen.strDialer
        End Get
    End Property

    Public Property InitialSelected As Boolean = False Implements IPageViewModel.InitialSelected

#Region "ICommand"
    Public Property RegisterCommand As RelayCommand
#End Region

    Public Sub New(ds As IOptionenService)
        ' Interface
        _DatenService = ds

        ' Commands
        RegisterCommand = New RelayCommand(AddressOf RegisterApp)
    End Sub

    Private Sub RegisterApp(o As Object)
        DatenService.RegisterApp()
    End Sub
End Class
