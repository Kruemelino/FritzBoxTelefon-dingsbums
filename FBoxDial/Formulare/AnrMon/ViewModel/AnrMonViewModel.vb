Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Media.Imaging
Imports System.Windows.Threading

Public Class AnrMonViewModel
    Inherits NotifyBase
    Private Property DialogService As IDialogService
    Private Property DatenService As IAnrMonService
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Eigenschaften"

    Private _AnrMonTelefonat As Telefonat
    Public Property AnrMonTelefonat As Telefonat
        Get
            Return _AnrMonTelefonat
        End Get
        Set
            SetProperty(_AnrMonTelefonat, Value)
            ' Daten laden
            LadeDaten()

            ' Eventhandler für Veränderungen am Telefonat starten
            AddHandler AnrMonTelefonat.PropertyChanged, AddressOf TelefonatChanged
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
        End Set
    End Property

    Private _AnrMonFirma As String
    Public Property AnrMonFirma As String
        Get
            Return _AnrMonFirma
        End Get
        Set
            SetProperty(_AnrMonFirma, Value)
            OnPropertyChanged(NameOf(ZeigeFirma))
        End Set
    End Property

    Private _Kontaktbild As BitmapImage
    Public Property Kontaktbild As BitmapImage
        Get
            Return _Kontaktbild
        End Get
        Set
            SetProperty(_Kontaktbild, Value)
            ' Veränderung der Visibility-Eigenschaft signalisieren
            OnPropertyChanged(NameOf(ZeigeBild))
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
            Return AnrMonTelefonat IsNot Nothing AndAlso (Not AnrMonTelefonat.NrUnterdrückt Or AnrMonTelefonat.AnruferName.IsNotStringNothingOrEmpty)
        End Get
    End Property
    Public ReadOnly Property ZeigeAnruferName As Boolean
        Get
            Return AnrMonTelefonat IsNot Nothing AndAlso AnrMonTelefonat.AnruferName.IsNotStringNothingOrEmpty
        End Get
    End Property
    Public ReadOnly Property ZeigeFirma As Boolean
        Get
            Return AnrMonTelefonat IsNot Nothing AndAlso AnrMonTelefonat.Firma.IsNotStringNothingOrEmpty
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
    End Sub

    Private Sub LadeDaten()
        ' Setze Anzuzeigende Werte

        ' Anruferzeit festlegen: Beginn des Telefonates
        Zeit = AnrMonTelefonat.ZeitBeginn

        ' Anrufende Telefonnummer
        AnrMonTelNr = AnrMonTelefonat.GegenstelleTelNr?.Formatiert

        ' Anrufer Name setzen
        AnrMonAnrufer = AnrMonTelefonat.AnruferName

        ' Eigene Telefonnummer setzen
        AnrMonEigeneTelNr = AnrMonTelefonat.EigeneTelNr?.Einwahl

        ' Firmeninformationen setzen
        AnrMonFirma = AnrMonTelefonat.Firma

        ' Setze das Kontaktbild, falls ein Outlookkontakt verfügbar ist.
        If Kontaktbild Is Nothing AndAlso AnrMonTelefonat.OlKontakt IsNot Nothing Then
            ' Speichere das Kontaktbild in einem temporären Ordner
            Dim BildPfad As String = KontaktFunktionen.KontaktBild(AnrMonTelefonat.OlKontakt)

            ' Überführe das Bild in das BitmapImage
            If BildPfad.IsNotStringNothingOrEmpty Then
                ' Kontaktbild laden
                Kontaktbild = New BitmapImage
                With Kontaktbild
                    .BeginInit()
                    .CacheOption = BitmapCacheOption.OnLoad
                    .UriSource = New Uri(BildPfad)
                    .EndInit()
                End With
                'Lösche das Kontaktbild aus dem temprären Ordner
                DelKontaktBild(BildPfad)
            End If
        End If

        ' Forcing the CommandManager to raise the RequerySuggested event
        CommandManager.InvalidateRequerySuggested()

    End Sub

#Region "Event Callback"
    Private Sub TelefonatChanged(sender As Object, e As PropertyChangedEventArgs)
        NLogger.Trace($"AnrMonVM: Eigenschaft {e.PropertyName} verändert.")
        Dispatcher.CurrentDispatcher.Invoke(Sub()
                                                NLogger.Trace("Aktualisiere VM")
                                                LadeDaten()
                                            End Sub)
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
            DatenService.BlockNumbers(AnrMonTelefonat.GegenstelleTelNr)
        End If

    End Sub
    Private Function CanBlock(o As Object) As Boolean
        Return AnrMonTelefonat IsNot Nothing AndAlso (Not AnrMonTelefonat.NrUnterdrückt And AnrMonTelefonat.AnruferName.IsStringNothingOrEmpty)
    End Function
#End Region
End Class
