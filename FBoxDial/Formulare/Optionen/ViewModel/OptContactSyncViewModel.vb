Imports System.Windows.Threading
Imports Microsoft.Office.Interop.Outlook
Public Class OptContactSyncViewModel
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

            ' InitRoutines
            LadeDaten()

        End Set
    End Property

    Public ReadOnly Property Name As String Implements IPageViewModel.Name
        Get
            Return Localize.LocOptionen.strContactSync
        End Get
    End Property

    Public Property InitialSelected As Boolean = False Implements IPageViewModel.InitialSelected

#Region "ICommand"
    Public Property ContactSyncAddCommand As RelayCommand
#End Region
    Public Sub New(ds As IOptionenService)
        ' Interface
        DialogService = New DialogService
        _DatenService = ds
        ' Commands
        ContactSyncAddCommand = New RelayCommand(AddressOf AddSyncSetup)

    End Sub

#Region "Properties"
    Public Property SyncSetups As New ObservableCollectionEx(Of OptSyncSetupViewModel)
    Public Property OutlookContactFolders As New ObservableCollectionEx(Of OutlookOrdner)
    Public Property FBoxPhoneBooks As New ObservableCollectionEx(Of PhonebookEx)
#End Region

    Private Sub LadeDaten()
        OutlookContactFolders.Clear()
        ' Lade Outlook-Ordner
        OutlookContactFolders.AddRange(DatenService.LadeOutlookKontaktFolder(OlItemType.olContactItem, OutlookOrdnerVerwendung.FBoxSync))

        FBoxPhoneBooks.Clear()
        ' Lade Fritz!Box Telefonbücher
        FBoxPhoneBooks.AddRange((DatenService.LadeFritzBoxTelefonbücher)?.Where(Function(F) Not (F.IsDAV Or F.CallBarringBook)))

        ' Lade gespeicherte Setups
        SyncSetups.AddRange(OptVM.OutlookOrdnerListe.FindAll(OutlookOrdnerVerwendung.FBoxSync) _
                                                    .Select(Function(O) New OptSyncSetupViewModel(DatenService, DialogService, Me) With
                                                        {.SetOrdner = O,
                                                         .FBoxTelefonbuch = FBoxPhoneBooks.Where(Function(FB) FB.ID.AreEqual(O.FBoxSyncOptions.FBoxSyncID)).First}))

    End Sub

    Private Sub AddSyncSetup(o As Object)
        SyncSetups.Add(New OptSyncSetupViewModel(DatenService, DialogService, Me))
    End Sub

    Friend Sub RemoveSyncSetup(OlVM As OptSyncSetupViewModel)
        OptVM.OutlookOrdnerListe.Remove(OlVM.OlOrdner)

        SyncSetups.Remove(OlVM)
    End Sub
End Class
