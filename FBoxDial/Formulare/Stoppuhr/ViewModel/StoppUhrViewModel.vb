﻿Imports System.ComponentModel
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Threading

Public Class StoppUhrViewModel
    Inherits NotifyBase

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IAnrMonService
    Private Property Timer As DispatcherTimer
    Private Property StoppUhr As Stopwatch
#Region "Felder"

    Private _StoppUhrTelefonat As Telefonat
    Public Property StoppUhrTelefonat As Telefonat
        Get
            Return _StoppUhrTelefonat
        End Get
        Set
            SetProperty(_StoppUhrTelefonat, Value)
            ' Daten laden
            LadeDaten()

            ' Eventhandler für Veränderungen am Telefonat starten
            AddHandler StoppUhrTelefonat.PropertyChanged, AddressOf TelefonatChanged
        End Set
    End Property

    Private _Beginn As Date
    Public Property Beginn As Date
        Get
            Return _Beginn
        End Get
        Set
            SetProperty(_Beginn, Value)
        End Set
    End Property

    Private _Ende As Date
    Public Property Ende As Date
        Get
            Return _Ende
        End Get
        Set
            SetProperty(_Ende, Value)
        End Set
    End Property

    Private _Dauer As TimeSpan
    Public Property Dauer As TimeSpan
        Get
            Return _Dauer
        End Get
        Set
            SetProperty(_Dauer, Value)
        End Set
    End Property

    Private _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set
            SetProperty(_Name, Value)
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

    Private _EigeneTelNr As String
    Public Property EigeneTelNr As String
        Get
            Return _EigeneTelNr
        End Get
        Set
            SetProperty(_EigeneTelNr, Value)
        End Set
    End Property

    Private _StartStoppuhr As Boolean
    Public Property StartStoppuhr As Boolean
        Get
            Return _StartStoppuhr
        End Get
        Set
            SetProperty(_StartStoppuhr, Value)

            If _StartStoppuhr Then
                ' Starte die Stoppuhr
                Start()

            Else
                ' Halte die Stoppuhr an
                Stopp()
            End If

        End Set
    End Property

    Private _Eingehend As Boolean
    Public Property Eingehend As Boolean
        Get
            Return _Eingehend
        End Get
        Set
            SetProperty(_Eingehend, Value)
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

    Private _PosTop As Integer
    Public Property PosTop As Integer
        Get
            Return _PosTop
        End Get
        Set
            SetProperty(_PosTop, Value)
        End Set
    End Property

    Private _PosLeft As Integer
    Public Property PosLeft As Integer
        Get
            Return _PosLeft
        End Get
        Set
            SetProperty(_PosLeft, Value)
        End Set
    End Property
#End Region

#Region "ICommand"
    Public Property ShowContactCommand As RelayCommand
    Public Property ClosingCommand As RelayCommand
    Public Property LocationChangedCommand As RelayCommand
#End Region

    Public Sub New()

        ' Init Command
        ShowContactCommand = New RelayCommand(AddressOf ShowContact)
        ' Window Command
        ClosingCommand = New RelayCommand(AddressOf Closing)
        LocationChangedCommand = New RelayCommand(AddressOf LocationChanged)
        ' Interface
        DatenService = New AnrMonService
    End Sub

    Private Sub LadeDaten()
        ' Setze Anzuzeigende Werte

        With StoppUhrTelefonat

            ' Anruferzeit festlegen: Beginn des Telefonates
            Beginn = .ZeitBeginn

            ' Anrufende Telefonnummer
            TelNr = .GegenstelleTelNr?.Formatiert

            ' Eigene Telefonnummer
            EigeneTelNr = .EigeneTelNr?.Einwahl

            ' Anrufer Name setzen
            Name = .NameGegenstelle

            ' Anrufrichtung festlegen
            Eingehend = .AnrufRichtung = Telefonat.AnrufRichtungen.Eingehend

            ' Hintergrundfarbe festlegen
            DatenService.GetColors(BackgroundColor, ForeColor, .EigeneTelNr, True, If(.OlKontakt?.IsVIP, False))

            ' Position festlegen
            PosTop = XMLData.POptionen.StoppUhrPosTop
            PosLeft = XMLData.POptionen.StoppUhrPosLeft

            ' Starte die Stoppuhr
            If StoppUhr Is Nothing Then
                ' Stoppuhr initialisieren
                StoppUhr = New Stopwatch
                ' Starten
                StartStoppuhr = True
            Else
                If StoppUhr.IsRunning AndAlso .Beendet Then
                    NLogger.Debug($"Stoppuhr nach {StoppUhr.Elapsed.TotalSeconds} angehalten")

                    ' Stoppuhr anhalten
                    StartStoppuhr = False

                    ' Anruferzeit festlegen: Ende des Telefonates
                    Ende = .ZeitEnde
                End If
            End If
        End With
        ' Forcing the CommandManager to raise the RequerySuggested event
        CommandManager.InvalidateRequerySuggested()

    End Sub

#Region "Event Callback"
    Private Sub TelefonatChanged(sender As Object, e As PropertyChangedEventArgs)
        NLogger.Trace($"StoppuhrVM: Eigenschaft {e.PropertyName} verändert.")
        Dispatcher.CurrentDispatcher.Invoke(Sub()
                                                NLogger.Trace("Aktualisiere VM")

                                                LadeDaten()
                                            End Sub)
    End Sub

    Private Sub Closing(o As Object)
        NLogger.Debug($"StoppuhrViewModel Closing")

        ' Stoppuhr anhalten
        StartStoppuhr = False

        ' Ereignishandler entfernen
        RemoveHandler StoppUhrTelefonat.PropertyChanged, AddressOf TelefonatChanged
    End Sub

    Private Sub LocationChanged(obj As Object)
        ' Position speichern
        XMLData.POptionen.StoppUhrPosTop = PosTop
        XMLData.POptionen.StoppUhrPosLeft = PosLeft
    End Sub
#End Region

    ''' <summary>
    ''' Routine zum Starten der Stoppuhr
    ''' </summary>
    Private Sub Start()
        ' Timer initialisieren
        Timer = New DispatcherTimer

        With Timer
            ' Intervall festlegen
            .Interval = TimeSpan.FromMilliseconds(100)

            ' Ereignishandler festlegen
            AddHandler .Tick, AddressOf StoppUhrTick

            ' Timer starten
            .Start()

            NLogger.Debug($"Timer für Stoppuhr gestartet")
        End With

        ' Stoppuhr starten
        StoppUhr.Start()

        NLogger.Debug($"Stoppuhr gestartet")
    End Sub

    ''' <summary>
    ''' Routine zum Beenden der Stoppuhr
    ''' </summary>
    Private Sub Stopp()

        With Timer
            ' Timer beenden
            .Stop()

            ' Ereignishandler entfernen
            RemoveHandler .Tick, AddressOf StoppUhrTick

            NLogger.Debug($"Timer für Stoppuhr angehalten")
        End With

        ' Stoppuhr anhalten
        StoppUhr.Stop()

        NLogger.Debug($"Stoppuhr angehalten")
    End Sub

#Region "Stoppuhr"
    Private Sub StoppUhrTick(sender As Object, e As EventArgs)
        If StoppUhr.IsRunning Then Dauer = StoppUhr.Elapsed
    End Sub
#End Region

#Region "ICommand Callback"
    Private Sub ShowContact(o As Object)
        StoppUhrTelefonat?.ZeigeKontakt()
    End Sub
#End Region
End Class


