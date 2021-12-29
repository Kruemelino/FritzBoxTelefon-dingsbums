Public Class TAMMessageViewModel
    Inherits NotifyBase
    Private Property DatenService As IFBoxDataService
    Private Property DialogService As IDialogService
    Friend Property TAMVM As TAMItemViewModel

#Region "ICommand"
    Public Property MarkMessageCommand As RelayCommand
    Public Property PlayMessageCommand As RelayCommand
    Public Property DeleteMessageCommand As RelayCommand
#End Region

#Region "Model"
    Public Property Message As FBoxAPI.Message
#End Region

#Region "ViewModel"
    Private _Neu As Boolean
    Public Property Neu As Boolean
        Get
            Return _Neu
        End Get
        Set
            SetProperty(_Neu, Value)
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
        Neu = DatenService.MarkMessage(Message)
    End Sub

    Private Sub DeleteMessage(o As Object)
        If DialogService.ShowMessageBox(Localize.LocFBoxData.strQuestionDeleteMessage) = Windows.MessageBoxResult.Yes Then
            If DatenService.DeleteMessage(Message) Then
                ' Lösche die Nachricht in dem Formular
                TAMVM.MessageListe.Remove(Me)
            End If
        End If
    End Sub

    Private Sub PlayMessage(o As Object)
        DatenService.PlayMessage(Message)

        ' Setze die Message auf abgehört
        If Neu Then MarkMessage(o)
    End Sub
#End Region
End Class
