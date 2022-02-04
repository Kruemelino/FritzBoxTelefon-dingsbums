Public Class TAMMessageViewModel
    Inherits NotifyBase
    Private Property DatenService As IFBoxDataService
    Private Property DialogService As IDialogService
    Friend Property TAMVM As TAMItemViewModel

#Region "ICommand"
    Public Property MarkMessageCommand As RelayCommand
    Public Property PlayMessageCommand As RelayCommand
    Public Property DeleteMessageCommand As RelayCommand
    Public Property DownloadMessageCommand As RelayCommand
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

    Private _IsPlaying As Boolean
    Public Property IsPlaying As Boolean
        Get
            Return _IsPlaying
        End Get
        Set
            SetProperty(_IsPlaying, Value)
        End Set
    End Property

    Public Property MessageURL As String

#End Region

    Public Sub New(dataService As IFBoxDataService, dialogService As IDialogService)

        ' Interface
        _DatenService = dataService
        _DialogService = dialogService

        ' Commands
        MarkMessageCommand = New RelayCommand(AddressOf MarkMessage)
        PlayMessageCommand = New RelayCommand(AddressOf PlayMessage)
        DeleteMessageCommand = New RelayCommand(AddressOf DeleteMessage)
        DownloadMessageCommand = New RelayCommand(AddressOf DownloadMessage)

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

        If CBool(o) Then
            ' Playback Stoppen
            ' Setze das Flag, dass das Abhören der Message abgebrochen wird.
            IsPlaying = False

            DatenService.StoppMessage(MessageURL)
        Else
            ' Ereignishandler hinzufügem
            AddHandler DatenService.SoundFinished, AddressOf DatenService_SoundFinished
            ' Setze das Flag, dass die Message abgehört wird.
            IsPlaying = True
            ' Ermittle die komplette URL
            If MessageURL.IsStringNothingOrEmpty Then MessageURL = DatenService.CompleteURL(Message.Path)
            ' Spiele die Message ab.
            DatenService.PlayMessage(MessageURL)
            ' Setze die Message auf abgehört
            ' If Neu Then MarkMessage(o)
        End If

    End Sub

    Private Sub DatenService_SoundFinished(sender As Object, e As NotifyEventArgs(Of String))

        ' Prüfe, ob die beendete Wiedergabe zu dieser TAM Message gehört.
        If e.Value.IsEqual(MessageURL) Then
            ' Enferne Ereignishandler
            RemoveHandler DatenService.SoundFinished, AddressOf DatenService_SoundFinished
            ' Setze die Message auf abgehört
            If Neu Then MarkMessage(Nothing)
            ' Setze das Flag, dass die Message nicht mehr abgehört wird.
            IsPlaying = False
        End If

    End Sub

    Private Sub DownloadMessage(o As Object)
        Dim Pfad As String = DialogService.SaveFile("WAV Audio|*.wav",
                                                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                                    $"TAM{Message.Tam}_{Message.Number}_{CDate(Message.Date):yyMMdd-HHmm}.wav")

        If Pfad.IsNotStringNothingOrEmpty Then
            ' Ermittle die komplette URL
            If MessageURL.IsStringNothingOrEmpty Then MessageURL = DatenService.CompleteURL(Message.Path)
            ' Herunterladen
            DatenService.DownloadMessage(MessageURL, Pfad)
        End If
    End Sub
#End Region
End Class
