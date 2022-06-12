Public Class OptIPPhonesViewModel
    Inherits NotifyBase
    Implements IPageViewModel
    Private Property DialogService As IDialogService
    Private Property DatenService As IOptionenService

    Private _OptVM As OptionenViewModel
    Public Property OptVM As OptionenViewModel Implements IPageViewModel.OptVM
        Get
            Return _OptVM
        End Get
        Set
            SetProperty(_OptVM, Value)

            ' Übergib das OptionenVM an die Clients
            For Each IPPhone In IPPhones
                IPPhone.OptVM = _OptVM
            Next
        End Set
    End Property

    Public ReadOnly Property Name As String Implements IPageViewModel.Name
        Get
            Return Localize.LocOptionen.strIPPhone
        End Get
    End Property

    Public Property InitialSelected As Boolean = False Implements IPageViewModel.InitialSelected

    Public Sub New(ds As IOptionenService)
        ' Commands

        ' Interface
        DialogService = New DialogService
        _DatenService = ds

        ' Lade Daten
        LadeSipClients()
    End Sub

    Private Sub LadeSipClients()

        ' Lade die SIPO Clients von der Fritz!Box
        Dim SIPPhoneList As FBoxAPI.SIPClientList = DatenService.GetSIPClients()

        If SIPPhoneList.SIPClients.Any Then
            ' Überführe die heruntergeladenen Daten in eine Liste an ViewModels
            IPPhones.AddRange(SIPPhoneList.SIPClients.Select(Function(SIP) New OptIPPhoneViewModel(DatenService, DialogService, SIP)))
        End If
    End Sub

#Region "Properties"
    Public Property IPPhones As New ObservableCollectionEx(Of OptIPPhoneViewModel)
#End Region
End Class
