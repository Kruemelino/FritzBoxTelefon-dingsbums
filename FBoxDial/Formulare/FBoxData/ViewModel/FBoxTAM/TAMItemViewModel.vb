Public Class TAMItemViewModel
    Inherits NotifyBase

    Private Property DatenService As IFBoxDataService
#Region "ICommand"
    Public Property ToggleCommand As RelayCommand
#End Region

#Region "ViewModel"
    Private _TAMMessageVM As TAMMessageViewModel
    Public Property TAMMessageVM As TAMMessageViewModel
        Get
            Return _TAMMessageVM
        End Get
        Set
            SetProperty(_TAMMessageVM, Value)
        End Set
    End Property

    Private _MessageListe As ObservableCollectionEx(Of TAMMessageViewModel)
    Public Property MessageListe As ObservableCollectionEx(Of TAMMessageViewModel)
        Get
            Return _MessageListe
        End Get
        Set
            SetProperty(_MessageListe, Value)
        End Set
    End Property
#End Region

#Region "Eigenschaften"
    Private _CurrentTAM As TAMItem
    Public Property CurrentTAM As TAMItem
        Get
            Return _CurrentTAM
        End Get
        Set
            SetProperty(_CurrentTAM, Value)
        End Set
    End Property

    Private _CurrentMessage As FritzBoxXMLMessage
    Public Property CurrentMessage As FritzBoxXMLMessage
        Get
            Return _CurrentMessage
        End Get
        Set
            SetProperty(_CurrentMessage, Value)
        End Set
    End Property
#End Region

    Public Sub New(dataService As IFBoxDataService, dialogService As IDialogService, cTAM As TAMItem)
        ' Interface
        _DatenService = dataService

        _CurrentTAM = cTAM

        ' Commands
        ToggleCommand = New RelayCommand(AddressOf ToggleTAMEnableState)

        If CurrentTAM.MessageList IsNot Nothing Then
            MessageListe = New ObservableCollectionEx(Of TAMMessageViewModel)
            MessageListe.AddRange(CurrentTAM.MessageList.Messages.Select(Function(ABM) New TAMMessageViewModel(DatenService, dialogService) With {.CurrentMessage = ABM, .TAMVM = Me}))
        End If
    End Sub

    Friend Sub ToggleTAMEnableState(o As Object)
        DatenService.ToggleTAM(CurrentTAM)
    End Sub

End Class
