Imports System.ComponentModel
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Threading

Public Class MissedCallViewModel
    Inherits NotifyBase
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IAnrMonService
    Private Property DialogService As IDialogService
    Friend Property Instance As Dispatcher

#Region "Visibility Eigenschaften"
    Public ReadOnly Property ZeigeBild As Boolean
        Get
            Return XMLData.POptionen.CBAnrMonContactImage And Kontaktbild IsNot Nothing
        End Get
    End Property

    Public ReadOnly Property ReCallEnabled As Boolean
        Get
            Return VerpasstesTelefonat IsNot Nothing AndAlso Not VerpasstesTelefonat.NrUnterdrückt
        End Get
    End Property

    Public ReadOnly Property TAMMessageAvailable As Boolean
        Get
            Return VerpasstesTelefonat IsNot Nothing AndAlso VerpasstesTelefonat.TAMMessagePath.IsNotStringNothingOrEmpty
        End Get
    End Property

    ''' <summary>
    ''' Gibt zurück, ob der Anrufer auf die Sperrliste gesetzt werden kann.
    ''' Dies ist nicht möglich, wenn der Kontakt in Outlook oder den Fritz!Box Telefonbüchern gefunden wurde.
    ''' Ebenso ist es nicht möglich, wenn die Nummer unterdrückt ist.
    ''' </summary>
    Public ReadOnly Property ZeigeBlockButton As Boolean
        Get
            Return VerpasstesTelefonat IsNot Nothing AndAlso (VerpasstesTelefonat.AnruferUnbekannt And Not VerpasstesTelefonat.NrUnterdrückt)
        End Get
    End Property
#End Region

#Region "Eigenschaften"

    Private _VerpasstesTelefonat As Telefonat
    Public Property VerpasstesTelefonat As Telefonat
        Get
            Return _VerpasstesTelefonat
        End Get
        Set
            SetProperty(_VerpasstesTelefonat, Value)

            ' Eventhandler für Veränderungen am Telefonat starten
            AddHandler VerpasstesTelefonat.PropertyChanged, AddressOf TelefonatChanged

            ' Daten laden
            LadeDaten()
        End Set
    End Property

    Private _Zeit As Date
    Public Property Zeit As Date
        Get
            Return _Zeit
        End Get
        Set
            SetProperty(_Zeit, Value)
        End Set
    End Property

    Private _EigeneTelNr As String
    Public Property EigeneTelNr As String
        Get
            Return _EigeneTelNr
        End Get
        Set
            SetProperty(_EigeneTelNr, Value)
        End Set
    End Property

    Private _TelNr As String
    Public Property TelNr As String
        Get
            Return _TelNr
        End Get
        Set
            SetProperty(_TelNr, Value)
        End Set
    End Property

    Private _Anrufer As String
    Public Property Anrufer As String
        Get
            Return _Anrufer
        End Get
        Set
            SetProperty(_Anrufer, Value)
        End Set
    End Property

    Private _ExInfo As String
    Public Property ExInfo As String
        Get
            Return _ExInfo
        End Get
        Set
            SetProperty(_ExInfo, Value)
        End Set
    End Property

    Private _MainInfo As String
    Public Property MainInfo As String
        Get
            Return _MainInfo
        End Get
        Set
            SetProperty(_MainInfo, Value)
        End Set
    End Property

    Private _AnzahlAnrufe As Integer = 1
    Public Property AnzahlAnrufe As Integer
        Get
            Return _AnzahlAnrufe
        End Get
        Set
            SetProperty(_AnzahlAnrufe, Value)
            AnzuzeigendeDaten()
        End Set
    End Property

    Private _Kontaktbild As Imaging.BitmapImage
    Public Property Kontaktbild As Imaging.BitmapImage
        Get
            Return _Kontaktbild
        End Get
        Set
            SetProperty(_Kontaktbild, Value)
            ' Veränderung der Visibility-Eigenschaft signalisieren
            OnPropertyChanged(NameOf(ZeigeBild))
        End Set
    End Property

    Private _BackgroundColor As String = CType(Globals.ThisAddIn.WPFApplication.FindResource("BackgroundColor"), SolidColorBrush).Color.ToString()
    Public Property BackgroundColor As String
        Get
            Return _BackgroundColor
        End Get
        Set
            SetProperty(_BackgroundColor, Value)
        End Set
    End Property

    Private _ForeColor As String = CType(Globals.ThisAddIn.WPFApplication.FindResource("ControlDefaultForeground"), SolidColorBrush).Color.ToString()
    Public Property ForeColor As String
        Get
            Return _ForeColor
        End Get
        Set
            SetProperty(_ForeColor, Value)
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

#Region "ICommand"
    Public Property CloseCommand As RelayCommand
    Public Property CallCommand As RelayCommand
    Public Property ShowContactCommand As RelayCommand
    Public Property BlockCommand As RelayCommand
    Public Property PlayMessageCommand As RelayCommand
#End Region
    Public Sub New(dataService As IAnrMonService, dialogservice As IDialogService)

        ' Interface
        _DatenService = dataService
        _DialogService = dialogservice
        ' Init Command
        CloseCommand = New RelayCommand(AddressOf Close)
        CallCommand = New RelayCommand(AddressOf [Call], AddressOf CanCall)
        ShowContactCommand = New RelayCommand(AddressOf ShowContact)
        BlockCommand = New RelayCommand(AddressOf BlockNumber)
        PlayMessageCommand = New RelayCommand(AddressOf PlayMessage)

    End Sub

    Private Async Sub LadeDaten()
        NLogger.Trace("LadeDaten MissedCallViewModel")
        ' Setze Anzuzeigende Werte
        With VerpasstesTelefonat

            ' Setze die anzuzeigenden Daten des Telefonates
            AnzuzeigendeDaten()

            ' Eigene Telefonnummer setzen
            If .EigeneTelNr Is Nothing AndAlso .OutEigeneTelNr.IsNotStringNothingOrEmpty Then
                ' Wenn die Daten aus der Einstellungsdatei bezogen wurden
                ' Kann das hier überhaupt passieren?
                .EigeneTelNr = New Telefonnummer With {.SetNummer = VerpasstesTelefonat.OutEigeneTelNr}
            End If

            ' Hintergrundfarbe festlegen
            DatenService.GetColors(BackgroundColor, ForeColor, .EigeneTelNr, False, False)

            EigeneTelNr = .EigeneTelNr?.Einwahl

            ' Setze das Kontaktbild
            If Kontaktbild Is Nothing Then
                Kontaktbild = Await DatenService.LadeBild(VerpasstesTelefonat)
            End If

        End With
        ' Forcing the CommandManager to raise the RequerySuggested event
        CommandManager.InvalidateRequerySuggested()

    End Sub

    Private Async Sub UpdateData()
        ' Lade das Kontaktbild, wenn a) Option gesetzt ist oder b) ein TellowsErgebnis vorliegt und das Bild noch nicht geladen wurde
        ' If Kontaktbild Is Nothing Then Kontaktbild = Await DatenService.LadeBild(VerpasstesTelefonat)
        If Kontaktbild Is Nothing Then Kontaktbild = Await Instance.Invoke(Function() DatenService.LadeBild(VerpasstesTelefonat))

        With VerpasstesTelefonat
            ' Hintergrundfarbe festlegen, falls VIP
            If .OlKontakt IsNot Nothing Then
                DatenService.GetColors(BackgroundColor, ForeColor, .EigeneTelNr, False, .OlKontakt.IsVIP)
            End If

            If .TellowsResult IsNot Nothing AndAlso XMLData.POptionen.CBTellowsAnrMonColor Then
                With .TellowsResult
                    If .Score.IsLargerOrEqual(XMLData.POptionen.CBTellowsAnrMonMinScore) And .Comments.IsLargerOrEqual(XMLData.POptionen.CBTellowsAnrMonMinComments) Then
                        ' Einfärben des Hintergrundes
                        BackgroundColor = .ScoreColor
                    End If
                End With
            End If

        End With

        OnPropertyChanged(NameOf(ZeigeBlockButton))

    End Sub

#Region "Event Callback"
    Private Sub TelefonatChanged(sender As Object, e As PropertyChangedEventArgs)
        NLogger.Trace($"MissedCallViewModel: Eigenschaft {e.PropertyName} verändert.")
        With VerpasstesTelefonat
            Select Case e.PropertyName
                Case NameOf(Telefonat.AnruferName), NameOf(Telefonat.Firma), NameOf(Telefonat.ZeitBeginn)
                    AnzuzeigendeDaten()
                Case NameOf(Telefonat.OlKontakt), NameOf(Telefonat.FBTelBookKontakt), NameOf(Telefonat.TellowsResult)
                    UpdateData()
                Case NameOf(Telefonat.TAMMessagePath)
                    OnPropertyChanged(NameOf(TAMMessageAvailable))
                Case Else
                    ' Nix tun
            End Select
        End With
    End Sub
#End Region

#Region "ICommand Callback"
    Private Sub Close(o As Object)
        NLogger.Debug($"CallListPaneItem: {VerpasstesTelefonat.NameGegenstelle} wird durch Nutzer entfernt.")
        DatenService.RemoveMissedCall(Me)
    End Sub

    Private Sub [Call](o As Object)
        ' Rückruf 
        VerpasstesTelefonat?.Rückruf()
        NLogger.Debug($"CallListPaneItem: {VerpasstesTelefonat.NameGegenstelle} wird durch Nutzer zurückgerufen.")
        DatenService.RemoveMissedCall(Me)
    End Sub

    Private Function CanCall(o As Object) As Boolean
        Return ReCallEnabled
    End Function
    Private Sub ShowContact(o As Object)
        VerpasstesTelefonat?.ZeigeKontakt()
    End Sub

    Private Sub BlockNumber(o As Object)
        If DialogService.ShowMessageBox(String.Format(Localize.LocAnrMon.strQuestionBlockNumber, VerpasstesTelefonat.GegenstelleTelNr.Formatiert)) = Windows.MessageBoxResult.Yes Then
            DatenService.BlockNumber(VerpasstesTelefonat.GegenstelleTelNr)
            NLogger.Debug($"CallListPaneItem: {VerpasstesTelefonat.NameGegenstelle} wird durch Nutzer blockiert.")
            DatenService.RemoveMissedCall(Me)
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
            If MessageURL.IsStringNothingOrEmpty Then MessageURL = DatenService.CompleteURL(VerpasstesTelefonat.TAMMessagePath)
            ' Spiele die Message ab.
            DatenService.PlayMessage(MessageURL)

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

#End Region

    Private Sub AnzuzeigendeDaten()
        If VerpasstesTelefonat IsNot Nothing Then
            ' Unterscheidung der anzuzeigenden Daten
            With VerpasstesTelefonat

                ' Anruferzeit festlegen: Beginn des Telefonates
                Zeit = .ZeitBeginn

                ' Eine Telefonnummer ist nicht vorhanden 
                If .GegenstelleTelNr.Unterdrückt Then
                    MainInfo = Localize.LocAnrMon.strNrUnterdrückt
                Else
                    ' Setze die Telefonnummer
                    TelNr = .GegenstelleTelNr.Formatiert

                    ' Ort der Nummer
                    ExInfo = .GegenstellenNummerLocation

                    ' Nur wenn eine Telefonnummer vorhanden ist, können Daten ausgegeben werden
                    If .AnruferName.IsNotStringNothingOrEmpty Then

                        ' Setze den Anrufernamen
                        MainInfo = .AnruferName

                        ' Erweiterte Informationen
                        If .Firma.IsNotStringNothingOrEmpty Then
                            ' Firmennamen ausgeben
                            ExInfo = .Firma
                        End If

                    ElseIf .Firma.IsNotStringNothingOrEmpty Then
                        ' Setze den Firmennamen
                        MainInfo = .Firma

                    Else
                        ' Setze die Telefonnummer
                        TelNr = Nothing

                        ' Setze die Telefonnummer als Hauptinformation
                        MainInfo = .GegenstelleTelNr.Formatiert
                    End If
                End If

                ' Anzahl Anrufe aktualisieren
                If AnzahlAnrufe.AreDifferentTo(1) Then
                    MainInfo = $"({ AnzahlAnrufe}x) {MainInfo}"
                End If
            End With
        End If
    End Sub

End Class
