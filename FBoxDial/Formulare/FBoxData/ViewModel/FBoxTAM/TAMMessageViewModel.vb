Public Class TAMMessageViewModel
    Inherits NotifyBase
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IFBoxDataService
    Private Property DialogService As IDialogService
#Region "ICommand"
    Public Property MarkMessageCommand As RelayCommand
    Public Property PlayMessageCommand As RelayCommand
    Public Property DeleteMessageCommand As RelayCommand
#End Region

#Region "ViewModel"
    Private _TAMVM As TAMItemViewModel
    Public Property TAMVM As TAMItemViewModel
        Get
            Return _TAMVM
        End Get
        Set
            SetProperty(_TAMVM, Value)
        End Set
    End Property
#End Region

#Region "Eigenschaften"
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

    Public Sub New(dataService As IFBoxDataService, dialogService As IDialogService)

        ' Interface
        _DatenService = dataService
        _DialogService = dialogService

        ' Commands
        MarkMessageCommand = New RelayCommand(AddressOf MarkMessage)
        PlayMessageCommand = New RelayCommand(AddressOf PlayMessage)
        DeleteMessageCommand = New RelayCommand(AddressOf DeleteMessage)
    End Sub

#Region "ICommand Callback"
    Private Sub MarkMessage(o As Object)
        If DatenService.MarkMessage(CurrentMessage) Then
            NLogger.Info($"Anrufbeantworter Message {CurrentMessage.Index} auf {If(CurrentMessage.[New], "neu", "abgehört")} gesetzt.")
            ' Alles OK
        End If
    End Sub

    Private Sub DeleteMessage(o As Object)
        If DialogService.ShowMessageBox(Localize.LocFBoxData.strQuestionDeleteMessage) = Windows.MessageBoxResult.Yes Then
            If DatenService.DeleteMessage(CurrentMessage) Then
                ' Lösche die Nachricht in dem Formular
                TAMVM.MessageListe.Remove(Me)
            End If
        End If
    End Sub

    Private Sub PlayMessage(o As Object)
        DatenService.PlayMessage(CurrentMessage)

        ' Setze die Message auf abgehört
        If CurrentMessage.[New] Then
            CurrentMessage.[New] = False
            MarkMessage(o)
        End If

    End Sub
#End Region
End Class
