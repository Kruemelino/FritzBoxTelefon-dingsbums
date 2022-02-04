Public Class CallViewModel
    Inherits NotifyBase

    Private Property DatenService As IFBoxDataService

#Region "Model"
    Private _Call As FBoxAPI.Call
    Public Property CallItem As FBoxAPI.Call
        Get
            Return _Call
        End Get
        Set
            SetProperty(_Call, Value)
        End Set
    End Property
#End Region

#Region "ICommand"
    Public Property PlayMessageCommand As RelayCommand
    Public Property DownloadFaxCommand As RelayCommand
#End Region

#Region "Eigene Eigenschaften"
    Private _Export As Boolean

    Public Property Export As Boolean
        Get
            Return _Export
        End Get
        Set
            SetProperty(_Export, Value)
        End Set
    End Property

    ''' <summary>
    ''' Gibt die Gegenstellennummer (ferne Nummer, NICHT die eigene Nummer) zurück.
    ''' Es wird je nach Telefonatstyp <c>Type</c> unterschschieden werden.    ''' 
    ''' </summary>
    Public ReadOnly Property Gegenstelle As String
        Get
            Select Case CallItem.Type
                Case 1, 2, 9, 10
                    Return CallItem.Caller
                Case Else '3, 11
                    Return CallItem.Called
            End Select
        End Get
    End Property

    Public ReadOnly Property Typ As Integer
        Get
            Return CallItem.Type
        End Get
    End Property

    ''' <summary>
    ''' Gibt die Eigene Nummer zurück.
    ''' </summary>
    Public ReadOnly Property EigeneNummer As String
        Get
            Return $"{CallItem.CalledNumber}{CallItem.CallerNumber}"
        End Get
    End Property

    Public ReadOnly Property Name As String
        Get
            Return CallItem.Name
        End Get
    End Property

    Public ReadOnly Property Datum As Date
        Get
            Return CDate(CallItem.[Date])
        End Get
    End Property

    Public ReadOnly Property NurDatum As Date
        Get
            Return CDate(CallItem.[Date]).Date
        End Get
    End Property

    Public ReadOnly Property NurZeit As String
        Get
            Return CDate(CallItem.[Date]).ToShortTimeString
        End Get
    End Property

    Public ReadOnly Property Dauer As String
        Get
            Return CDate(CallItem.Duration).ToShortTimeString
            'With CDate(CallItem.Duration)
            '    Return New TimeSpan(.Hour, .Minute, .Second)
            'End With
        End Get
    End Property

    Public ReadOnly Property Gerät As String
        Get
            Return CallItem.Device
        End Get
    End Property

    Public ReadOnly Property ButtonVisible As Boolean
        Get
            Return CallItem.Path.IsNotStringNothingOrEmpty
        End Get
    End Property

    Public ReadOnly Property TAMMessageAvailable As Boolean
        Get
            Return ButtonVisible AndAlso CallItem.Path.Contains("rec")
        End Get
    End Property

    Public ReadOnly Property FaxAvailable As Boolean
        Get
            Return ButtonVisible AndAlso CallItem.Path.Contains("fax")
        End Get
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

    Public Sub New(dataService As IFBoxDataService)
        ' Interface
        _DatenService = dataService

        ' Commands
        PlayMessageCommand = New RelayCommand(AddressOf PlayMessage)
        DownloadFaxCommand = New RelayCommand(AddressOf DownloadFax)
    End Sub

    Private Sub DownloadFax(o As Object)
        DatenService.DownloadFax(CallItem)
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
            If MessageURL.IsStringNothingOrEmpty Then MessageURL = DatenService.CompleteURL(CallItem.Path)
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
            ' Setze das Flag, dass die Message nicht mehr abgehört wird.
            IsPlaying = False
        End If
    End Sub
End Class
