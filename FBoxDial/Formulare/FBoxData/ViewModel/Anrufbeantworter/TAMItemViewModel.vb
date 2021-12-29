Public Class TAMItemViewModel
    Inherits NotifyBase

    Private Property DatenService As IFBoxDataService

#Region "ICommand"
    Public Property ToggleCommand As RelayCommand
#End Region

#Region "Model TAMItem"
    Public Property TAMItem As FBoxAPI.TAMItem
#End Region

#Region "Nachrichten"
    Public Property MessageListe As ObservableCollectionEx(Of TAMMessageViewModel)
#End Region

#Region "Eigenschaften"
    Private _Enable As Boolean
    Public Property Enable As Boolean
        Get
            Return _Enable
        End Get
        Set
            SetProperty(_Enable, Value)
        End Set
    End Property
#End Region

    Public Sub New(dataService As IFBoxDataService, dialogService As IDialogService, tam As FBoxAPI.TAMItem)
        ' Interface
        _DatenService = dataService

        ' Commands
        ToggleCommand = New RelayCommand(AddressOf ToggleTAMEnableState)

        ' Lege den TAM fest
        _TAMItem = tam

        ' Setze den Einschaltzustand des TAM
        Enable = TAMItem.Enable

        ' Lade die zugehörigen Nachrichten
        MessageListe = New ObservableCollectionEx(Of TAMMessageViewModel)(DatenService.GetMessagges(TAMItem).Select(Function(m) New TAMMessageViewModel(dataService, dialogService) With {.TAMVM = Me, .Message = m, .Neu = m.[New]}))

    End Sub

    Friend Sub ToggleTAMEnableState(o As Object)
        Enable = DatenService.ToggleTAM(TAMItem)
    End Sub

End Class
