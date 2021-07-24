Imports Microsoft.Office.Interop.Outlook

Public Class OptCreateContactViewModel
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
            Return Localize.LocOptionen.strCreateContact
        End Get
    End Property

    Public Property InitialSelected As Boolean = False Implements IPageViewModel.InitialSelected

    Private Property RootVM As OutlookFolderViewModel = New OutlookFolderViewModel(OlItemType.olContactItem, OutlookOrdnerVerwendung.KontaktSpeichern)

    Public ReadOnly Property Root As OutlookFolderViewModel
        Get
            RootVM.OptVM = OptVM
            Return RootVM
        End Get
    End Property
End Class
