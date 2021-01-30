Imports System.ComponentModel
Imports System.Threading
Imports System.Windows
Imports System.Windows.Interop
Imports System.Windows.Markup
Imports System.Windows.Threading

Public Class StoppUhrWPF
    Inherits Window

    Private WithEvents CtrlKontaktWahl As UserCtrlKontaktwahl
    Private WithEvents CtrlDirektWahl As UserCtrlDirektwahl
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Private Property Timer As DispatcherTimer
    Private Property StoppUhr As Stopwatch

#Region "Event"
    Public Event Geschlossen(sender As Object, e As EventArgs)
#End Region

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)
        ' Startup Position festlegen
        'WindowStartupLocation = WindowStartupLocation.CenterScreen

        ' DispatcherTimer initialisieren
        Timer = New DispatcherTimer

        'Stoppuhr initialisieren
        StoppUhr = New Stopwatch

        Show()
    End Sub

    Friend Sub ShowStoppUhr(Tlfnt As Telefonat)

        ' Fülle das Viewmodel
        With CType(DataContext, StoppUhrViewModel)
            .Beginn = Tlfnt.ZeitBeginn
            .Name = If(Tlfnt.AnruferName.IsNotStringNothingOrEmpty, Tlfnt.AnruferName, Tlfnt.GegenstelleTelNr?.Formatiert)
            .AutomatischAusblenden = XMLData.POptionen.CBStoppUhrAusblenden
            .Ausblendverzögerung = XMLData.POptionen.TBStoppUhrAusblendverzögerung
            .Tlfnt = Tlfnt

            NLogger.Debug($"Stoppuhr gestartet: { .Name}")
        End With

        ' Notwendigkeit unklar. Funktioniert aber gut.
        UnsafeNativeMethods.SetWindowPos(New WindowInteropHelper(Me).Handle, HWndInsertAfterFlags.HWND_TOPMOST, 0, 0, 0, 0, CType(SetWindowPosFlags.DoNotActivate + SetWindowPosFlags.IgnoreMove + SetWindowPosFlags.IgnoreResize + SetWindowPosFlags.DoNotChangeOwnerZOrder, SetWindowPosFlags))

        ' Stoppuhr einblenden
        Start()
        Show()
    End Sub

    Friend Sub Start()

        With Timer
            .Interval = New TimeSpan(0, 0, 0, 0, 100)
            AddHandler .Tick, AddressOf Timer_TickStoppUhr
            .Start()
        End With

        StoppUhr.Start()
    End Sub

    Friend Sub Stopp()

        Dispatcher.Invoke(Sub()

                              With CType(DataContext, StoppUhrViewModel)
                                  NLogger.Debug($"Stoppuhr angehalten: { .Name}")

                                  'Stoppuhr anhalten
                                  StoppUhr.Stop()

                                  ' Endzeit des Telefonates setzen
                                  .Ende = Now

                                  ' Den Timer der Stoppuhr anhalten
                                  With Timer
                                      ' Ereignishandler entfernen
                                      RemoveHandler .Tick, AddressOf Timer_TickStoppUhr
                                      .Stop()
                                  End With

                                  If .AutomatischAusblenden Then

                                      If .Ausblendverzögerung.IsPositive Then
                                          Dim Intervall As Integer = .Ausblendverzögerung
                                          With Timer
                                              .Interval = New TimeSpan(0, 0, 0, Intervall)
                                              AddHandler .Tick, AddressOf Timer_TickClose
                                              .Start()
                                          End With
                                      Else
                                          Close()
                                      End If
                                  End If

                              End With
                          End Sub)
    End Sub


    Sub Timer_TickStoppUhr(sender As Object, e As EventArgs)
        If StoppUhr.IsRunning Then
            clocktxtblock.Text = StoppUhr.Elapsed.ToString("hh\:mm\:ss")
        End If
    End Sub

    Sub Timer_TickClose(sender As Object, e As EventArgs)
        Close()
    End Sub

    Private Sub StoppUhrWPF_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        RaiseEvent Geschlossen(Me, EventArgs.Empty)
    End Sub

    Private Sub StoppUhrWPF_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        NLogger.Debug($"Stoppuhr geschlossen: {CType(DataContext, StoppUhrViewModel).Name}")
        ' Timer anhalten
        If Timer.IsEnabled Then
            ' Ereignishandler entfernen
            RemoveHandler Timer.Tick, AddressOf Timer_TickStoppUhr
            RemoveHandler Timer.Tick, AddressOf Timer_TickClose
            Timer.Stop()
        End If
        'Stoppuhr anhalten
        If StoppUhr.IsRunning Then StoppUhr.Stop()
    End Sub
    Private Sub BContact_Click(sender As Object, e As RoutedEventArgs)
        CType(DataContext, StoppUhrViewModel).Tlfnt?.ZeigeKontakt()
    End Sub
End Class
