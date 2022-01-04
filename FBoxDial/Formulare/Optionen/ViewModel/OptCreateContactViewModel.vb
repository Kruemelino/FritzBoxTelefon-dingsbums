Imports Microsoft.Office.Interop.Outlook

Public Class OptCreateContactViewModel
    Inherits NotifyBase
    Implements IPageViewModel

    Private Property DatenService As IOptionenService

    Private Const Verwendung As OutlookOrdnerVerwendung = OutlookOrdnerVerwendung.KontaktSpeichern
    Private Const ItemType As OlItemType = OlItemType.olContactItem

#Region "Felder"
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

    Private Property RootVM As OutlookFolderViewModel
    Public ReadOnly Property Root As OutlookFolderViewModel
        Get
            If RootVM Is Nothing Then RootVM = New OutlookFolderViewModel(DatenService.GetOutlookStoreRootFolder, ItemType, Verwendung)

            RootVM.OptVM = OptVM
            Return RootVM
        End Get
    End Property
#End Region

    Public Sub New(ds As IOptionenService)
        ' Interface
        _DatenService = ds
    End Sub

End Class
