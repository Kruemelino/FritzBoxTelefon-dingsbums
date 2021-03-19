Imports Microsoft.Office.Interop.Outlook

Public Class OptSearchContactViewModel
    Inherits NotifyBase
    Implements IPageViewModel

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
            Return Localize.LocOptionen.strSearchContact
        End Get
    End Property

    Private Property RootVM As OutlookFolderViewModel = New OutlookFolderViewModel(OlItemType.olContactItem, OutlookOrdnerVerwendung.KontaktSuche)

    Public ReadOnly Property Root As OutlookFolderViewModel
        Get
            RootVM.OptVM = OptVM
            Return RootVM
        End Get
    End Property
End Class
