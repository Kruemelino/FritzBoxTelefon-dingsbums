Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Threading

Public Class AnrMonViewModel
    Inherits NotifyBase
    Implements IViewModelBase

    Private Property DialogService As IDialogService
    Private Property DatenService As IAnrMonService
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend Property Instance As Dispatcher Implements IViewModelBase.Instance

#Region "Eigenschaften"

    Private _AnrMonTelefonat As Telefonat
    Public Property AnrMonTelefonat As Telefonat
        Get
            Return _AnrMonTelefonat
        End Get
        Set
            SetProperty(_AnrMonTelefonat, Value)

            ' Eventhandler für Veränderungen am Telefonat starten
            AddHandler AnrMonTelefonat.PropertyChanged, AddressOf TelefonatChanged

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

    Private _AnrMonEigeneTelNr As String
    Public Property AnrMonEigeneTelNr As String
        Get
            Return _AnrMonEigeneTelNr
        End Get
        Set
            SetProperty(_AnrMonEigeneTelNr, Value)
        End Set
    End Property

    Private _AnrMonTelNr As String
    Public Property AnrMonTelNr As String
        Get
            Return _AnrMonTelNr
        End Get
        Set
            SetProperty(_AnrMonTelNr, Value)
            OnPropertyChanged(NameOf(ZeigeTelNr))
        End Set
    End Property

    Private _AnrMonAnrufer As String
    Public Property AnrMonAnrufer As String
        Get
            Return _AnrMonAnrufer
        End Get
        Set
            SetProperty(_AnrMonAnrufer, Value)
            OnPropertyChanged(NameOf(ZeigeAnruferName))
            OnPropertyChanged(NameOf(ZeigeTelNr))
        End Set
    End Property

    Private _AnrMonExInfo As String
    Public Property AnrMonExInfo As String
        Get
            Return _AnrMonExInfo
        End Get
        Set
            SetProperty(_AnrMonExInfo, Value)
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

    Private _ForeColor As String
    Public Property ForeColor As String
        Get
            Return _ForeColor
        End Get
        Set
            SetProperty(_ForeColor, Value)
        End Set
    End Property
#End Region

#Region "Visibility Eigenschaften"
    Public ReadOnly Property ZeigeBild As Boolean
        Get
            Return XMLData.POptionen.CBAnrMonContactImage And Kontaktbild IsNot Nothing
        End Get
    End Property
    Public ReadOnly Property ZeigeTelNr As Boolean
        Get
            Return AnrMonTelefonat IsNot Nothing AndAlso (Not AnrMonTelefonat.NrUnterdrückt And AnrMonTelefonat.AnruferName.IsNotStringNothingOrEmpty)
        End Get
    End Property
    Public ReadOnly Property ZeigeAnruferName As Boolean
        Get
            Return AnrMonTelefonat IsNot Nothing AndAlso AnrMonTelefonat.AnruferName.IsNotStringNothingOrEmpty
        End Get
    End Property
    Public ReadOnly Property ZeigeExInfo As Boolean
        Get
            Return AnrMonTelefonat IsNot Nothing AndAlso AnrMonTelefonat.AnrMonExInfo.IsNotStringNothingOrEmpty
        End Get
    End Property

    Public ReadOnly Property ReCallEnabled As Boolean
        Get
            Return AnrMonTelefonat IsNot Nothing AndAlso Not AnrMonTelefonat.NrUnterdrückt
        End Get
    End Property
#End Region

#Region "ICommand"
    Public Property CloseCommand As RelayCommand
    Public Property CallCommand As RelayCommand
    Public Property ShowContactCommand As RelayCommand
    Public Property ClosingCommand As RelayCommand
    Public Property BlockCommand As RelayCommand
#End Region

    Public Sub New()

        ' Init Command
        CloseCommand = New RelayCommand(AddressOf Close)
        CallCommand = New RelayCommand(AddressOf [Call], AddressOf CanCall)
        ShowContactCommand = New RelayCommand(AddressOf ShowContact)
        BlockCommand = New RelayCommand(AddressOf BlockNumber, AddressOf CanBlock)

        ' Window Command
        ClosingCommand = New RelayCommand(AddressOf Closing)

        ' Interface
        DatenService = New AnrMonService
        DialogService = New DialogService

        If XMLData.POptionen.CBSetAnrMonBColor Then
            BackgroundColor = XMLData.POptionen.TBAnrMonBColorHex
            ForeColor = XMLData.POptionen.TBAnrMonFColorHex
        Else
            BackgroundColor = CType(Globals.ThisAddIn.WPFApplication.FindResource("BackgroundColor"), SolidColorBrush).Color.ToString()
            ForeColor = CType(Globals.ThisAddIn.WPFApplication.FindResource("ControlDefaultForeground"), SolidColorBrush).Color.ToString()
        End If
    End Sub

    Private Async Sub LadeDaten()
        NLogger.Trace("LadeDaten")
        ' Setze Anzuzeigende Werte
        With AnrMonTelefonat

            ' Anruferzeit festlegen: Beginn des Telefonates
            Zeit = .ZeitBeginn

            ' Anrufende Telefonnummer
            AnrMonTelNr = .GegenstelleTelNr?.Formatiert

            ' Anrufer Name setzen
            AnrMonAnrufer = .AnruferName

            ' Eigene Telefonnummer setzen
            If .EigeneTelNr Is Nothing AndAlso .OutEigeneTelNr.IsNotStringNothingOrEmpty Then
                .EigeneTelNr = New Telefonnummer With {.SetNummer = AnrMonTelefonat.OutEigeneTelNr}
            End If
            AnrMonEigeneTelNr = .EigeneTelNr?.Einwahl

            ' Erweiterte Informationen setzen (Firma oder Name des Ortsnetzes, Land)
            AnrMonExInfo = .AnrMonExInfo

            ' Setze das Kontaktbild
            If Kontaktbild Is Nothing Then
                Kontaktbild = Await Instance.Invoke(Function() DatenService.LadeBild(AnrMonTelefonat))
            End If

        End With
        ' Forcing the CommandManager to raise the RequerySuggested event
        CommandManager.InvalidateRequerySuggested()

    End Sub

    Private Async Sub UpdateData()
        ' Lade das Kontaktbild, wenn a) Option gesetzt ist oder b) ein TellowsErgebnis vorliegt und das Bild noch nicht geladen wurde
        If Kontaktbild Is Nothing Then Kontaktbild = Await DatenService.LadeBild(AnrMonTelefonat)

        If AnrMonTelefonat.TellowsResult IsNot Nothing AndAlso XMLData.POptionen.CBTellowsAnrMonColor Then
            With AnrMonTelefonat.TellowsResult
                If .Score.IsLargerOrEqual(XMLData.POptionen.CBTellowsAnrMonMinScore) And .Comments.IsLargerOrEqual(XMLData.POptionen.CBTellowsAnrMonMinComments) Then
                    ' Einfärben des Hintergrundes
                    BackgroundColor = .ScoreColor
                End If
            End With
        End If
    End Sub

#Region "Event Callback"
    Private Sub TelefonatChanged(sender As Object, e As PropertyChangedEventArgs)
        NLogger.Trace($"AnrMonVM: Eigenschaft {e.PropertyName} verändert.")
        With AnrMonTelefonat
            Select Case e.PropertyName
                Case NameOf(Telefonat.AnruferName)
                    AnrMonAnrufer = .AnruferName
                Case NameOf(Telefonat.Firma), NameOf(Telefonat.AnrMonExInfo)
                    AnrMonExInfo = .AnrMonExInfo
                Case NameOf(Telefonat.OlKontakt), NameOf(Telefonat.FBTelBookKontakt), NameOf(Telefonat.TellowsResult)
                    Instance.Invoke(Sub() UpdateData())
                Case Else
                    ' Nix tun
            End Select
        End With
    End Sub
#End Region

#Region "ICommand Callback"
    Private Sub Close(o As Object)
        NLogger.Debug("Anrufmonitor wird durch Nutzer geschlossen.")
        CType(o, Window).Close()
    End Sub

    Private Sub [Call](o As Object)
        AnrMonTelefonat?.Rückruf()
    End Sub

    Private Function CanCall(o As Object) As Boolean
        Return ReCallEnabled
    End Function
    Private Sub ShowContact(o As Object)
        AnrMonTelefonat?.ZeigeKontakt()
    End Sub

    Private Sub Closing(o As Object)
        NLogger.Debug($"AnrMonViewModel Closing")
        ' Ereignishandler entfernen
        RemoveHandler AnrMonTelefonat.PropertyChanged, AddressOf TelefonatChanged
    End Sub

    Private Sub BlockNumber(o As Object)

        If DialogService.ShowMessageBox(String.Format(Localize.LocAnrMon.strQuestionBlockNumber, AnrMonTelefonat.GegenstelleTelNr.Formatiert)) = Windows.MessageBoxResult.Yes Then
            DatenService.BlockNumber(AnrMonTelefonat.GegenstelleTelNr)
        End If

    End Sub
    ''' <summary>
    ''' Gibt zurück, ob der Anrufer auf die Sperrliste gesetzt werden kann.
    ''' Dies ist nicht möglich, wenn der Kontakt in Outlook oder den Fritz!Box Telefonbüchern gefunden wurde.
    ''' Ebenso ist es nicht möglich, wenn die Nummer unterdrückt ist.
    ''' </summary>
    Private Function CanBlock(o As Object) As Boolean
        Return AnrMonTelefonat IsNot Nothing AndAlso AnrMonTelefonat.AnruferUnbekannt
    End Function
#End Region
End Class
