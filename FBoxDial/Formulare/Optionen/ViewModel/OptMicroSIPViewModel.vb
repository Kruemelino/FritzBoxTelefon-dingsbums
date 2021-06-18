Public Class OptMicroSIPViewModel
    Inherits NotifyBase
    Implements IPageViewModel

    Private _OptVM As OptionenViewModel
    Private Property DialogService As IDialogService
    Private Property DatenService As IOptionenService

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
            Return Localize.LocOptionen.strMicroSIP
        End Get
    End Property

    Public Property MicroSIPPathCommand As RelayCommand

    Public Sub New()
        ' Commands
        MicroSIPPathCommand = New RelayCommand(AddressOf GetMicroSIPPath)

        ' Interface
        DialogService = New DialogService
        DatenService = New OptionenService
    End Sub

    Private Sub GetMicroSIPPath(o As Object)
        ' Initialen Pfad ermitteln
        Dim Dateipfad As String = DialogService.OpenFile("MicroSIP.exe (.exe)|*.exe", DatenService.GetMicroSIPExecutablePath)
        If Dateipfad.IsNotStringNothingOrEmpty Then OptVM.TBMicroSIPPath = Dateipfad
    End Sub
End Class
