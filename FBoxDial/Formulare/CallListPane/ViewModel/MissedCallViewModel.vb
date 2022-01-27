Imports System.ComponentModel
Imports System.Windows.Input
Imports System.Windows.Media

Public Class MissedCallViewModel
    Inherits NotifyBase
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IAnrMonService
    Private Property DialogService As IDialogService

#Region "Visibility Eigenschaften"
    Public ReadOnly Property ZeigeBild As Boolean
        Get
            Return XMLData.POptionen.CBAnrMonContactImage And Kontaktbild IsNot Nothing
        End Get
    End Property
    Public ReadOnly Property ZeigeTelNr As Boolean
        Get
            Return VerpasstesTelefonat IsNot Nothing AndAlso (Not VerpasstesTelefonat.NrUnterdrückt And VerpasstesTelefonat.AnruferName.IsNotStringNothingOrEmpty)
        End Get
    End Property
    Public ReadOnly Property ZeigeAnruferName As Boolean
        Get
            Return VerpasstesTelefonat IsNot Nothing AndAlso VerpasstesTelefonat.AnruferName.IsNotStringNothingOrEmpty
        End Get
    End Property
    Public ReadOnly Property ZeigeExInfo As Boolean
        Get
            Return VerpasstesTelefonat IsNot Nothing AndAlso VerpasstesTelefonat.AnrMonExInfo.IsNotStringNothingOrEmpty
        End Get
    End Property

    Public ReadOnly Property ReCallEnabled As Boolean
        Get
            Return VerpasstesTelefonat IsNot Nothing AndAlso Not VerpasstesTelefonat.NrUnterdrückt
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
            OnPropertyChanged(NameOf(ZeigeTelNr))
        End Set
    End Property

    Private _Anrufer As String
    Public Property Anrufer As String
        Get
            Return _Anrufer
        End Get
        Set
            SetProperty(_Anrufer, Value)
            OnPropertyChanged(NameOf(ZeigeAnruferName))
            OnPropertyChanged(NameOf(ZeigeTelNr))
        End Set
    End Property

    Private _ExInfo As String
    Public Property ExInfo As String
        Get
            Return _ExInfo
        End Get
        Set
            SetProperty(_ExInfo, Value)
            OnPropertyChanged(NameOf(ZeigeExInfo))
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

    Private _BackgroundColor As String
    Public Property BackgroundColor As String
        Get
            Return _BackgroundColor
        End Get
        Set
            SetProperty(_BackgroundColor, Value)
        End Set
    End Property

#End Region


#Region "ICommand"
    Public Property CloseCommand As RelayCommand
    Public Property CallCommand As RelayCommand
    Public Property ShowContactCommand As RelayCommand
    Public Property BlockCommand As RelayCommand
#End Region

    Public Sub New(dataService As IAnrMonService, dialogservice As IDialogService)

        ' Interface
        _DatenService = dataService
        _DialogService = dialogservice
        ' Init Command
        CloseCommand = New RelayCommand(AddressOf Close)
        CallCommand = New RelayCommand(AddressOf [Call], AddressOf CanCall)
        ShowContactCommand = New RelayCommand(AddressOf ShowContact)
        BlockCommand = New RelayCommand(AddressOf BlockNumber, AddressOf CanBlock)
    End Sub

    Private Async Sub LadeDaten()
        NLogger.Trace("LadeDaten MissedCallViewModel")
        ' Setze Anzuzeigende Werte
        With VerpasstesTelefonat

            ' Anruferzeit festlegen: Beginn des Telefonates
            Zeit = .ZeitBeginn

            ' Anrufende Telefonnummer
            TelNr = .GegenstelleTelNr?.Formatiert

            ' Anrufer Name setzen
            Anrufer = .AnruferName

            ' Eigene Telefonnummer setzen
            If .EigeneTelNr Is Nothing AndAlso .OutEigeneTelNr.IsNotStringNothingOrEmpty Then
                .EigeneTelNr = New Telefonnummer With {.SetNummer = VerpasstesTelefonat.OutEigeneTelNr}
            End If
            EigeneTelNr = .EigeneTelNr?.Einwahl

            ' Erweiterte Informationen setzen (Firma oder Name des Ortsnetzes, Land)
            ExInfo = .AnrMonExInfo

            ' Setze das Kontaktbild
            If Kontaktbild Is Nothing Then
                'Kontaktbild = Await Instance.Invoke(Function() DatenService.LadeBild(AnrMonTelefonat))
                Kontaktbild = Await DatenService.LadeBild(VerpasstesTelefonat)
            End If

        End With
        ' Forcing the CommandManager to raise the RequerySuggested event
        CommandManager.InvalidateRequerySuggested()

    End Sub

    Private Async Sub UpdateData()
        ' Lade das Kontaktbild, wenn a) Option gesetzt ist oder b) ein TellowsErgebnis vorliegt und das Bild noch nicht geladen wurde
        If Kontaktbild Is Nothing Then Kontaktbild = Await DatenService.LadeBild(VerpasstesTelefonat)

        If VerpasstesTelefonat.TellowsResult IsNot Nothing AndAlso XMLData.POptionen.CBTellowsAnrMonColor Then
            With VerpasstesTelefonat.TellowsResult
                If .Score.IsLargerOrEqual(XMLData.POptionen.CBTellowsAnrMonMinScore) And .Comments.IsLargerOrEqual(XMLData.POptionen.CBTellowsAnrMonMinComments) Then
                    ' Einfärben des Hintergrundes
                    BackgroundColor = .ScoreColor
                End If
            End With
        End If
    End Sub

#Region "Event Callback"
    Private Sub TelefonatChanged(sender As Object, e As PropertyChangedEventArgs)
        NLogger.Trace($"MissedCallViewModel: Eigenschaft {e.PropertyName} verändert.")
        With VerpasstesTelefonat
            Select Case e.PropertyName
                Case NameOf(Telefonat.AnruferName)
                    Anrufer = .AnruferName
                Case NameOf(Telefonat.Firma), NameOf(Telefonat.AnrMonExInfo)
                    ExInfo = .AnrMonExInfo
                Case NameOf(Telefonat.OlKontakt), NameOf(Telefonat.FBTelBookKontakt), NameOf(Telefonat.TellowsResult)
                    UpdateData()
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
    ''' <summary>
    ''' Gibt zurück, ob der Anrufer auf die Sperrliste gesetzt werden kann.
    ''' Dies ist nicht möglich, wenn der Kontakt in Outlook oder den Fritz!Box Telefonbüchern gefunden wurde.
    ''' Ebenso ist es nicht möglich, wenn die Nummer unterdrückt ist.
    ''' </summary>
    Private Function CanBlock(o As Object) As Boolean
        Return VerpasstesTelefonat IsNot Nothing AndAlso VerpasstesTelefonat.AnruferUnbekannt
    End Function
#End Region


End Class
