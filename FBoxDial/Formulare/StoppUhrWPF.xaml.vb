Imports System.ComponentModel
Imports System.Threading
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Markup

Public Class StoppUhrWPF
    Inherits Window

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Event"
    Public Event Geschlossen(sender As Object, e As EventArgs)
#End Region

#Region "Dispatcher Timer"
    ''' <summary>
    ''' Timer für das automatische Ausblenden der Stoppuhr.
    ''' So bald die gewählte Zeit erreicht ist, wird die Stoppuhr ausgeblendet.
    ''' Wenn die Maus sich auf dem Fenster befindet, wird der Timer unterbrochen.
    ''' Sobald sich die Maus vom dem Fenster entfernt, wird der Timer fortgesetzt.
    ''' </summary>
    Private AusblendTimer As Threading.DispatcherTimer
#End Region
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)

    End Sub

#Region "Window Events"
    ''' <summary>
    ''' Tritt ein, wenn dieses <see cref="FrameworkElement"/> initialisiert wird. Dieses Ereignis geht mit Fällen einher, 
    ''' in denen sich der Wert der <see cref="FrameworkElement.IsInitialized"/>-Eigenschaft von false (oder nicht definiert) in true ändert.
    ''' </summary>
    Private Sub StoppUhrWPF_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        NLogger.Trace("Initialized")

        ' Outlook Inspektoren beachten
        KeepoInspActivated(False)
    End Sub

    ''' <summary>
    ''' Tritt auf, wenn das Element ausgerichtet und gerendert sowie zur Interaktion vorbereitet wurde.
    ''' </summary>
    Private Sub StoppUhrWPF_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        NLogger.Trace("Loaded")

        ' Blende den Stoppuhr Topmost, aber ohne Aktivierung
        SetWindowPosPopUp(New Interop.WindowInteropHelper(Me).Handle)

        NLogger.Debug("Stoppuhr positioniert")

        ' Outlook Inspektor reaktivieren
        KeepoInspActivated(True)
    End Sub

    Private Sub StoppUhrWPF_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        RaiseEvent Geschlossen(Me, e)
    End Sub

    Private Sub StoppUhrWPF_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

        If AusblendTimer IsNot Nothing Then
            With AusblendTimer
                ' Stoppe den Timer
                .Stop()
                ' Ereignishandler entfernen
                RemoveHandler .Tick, AddressOf AusblendTimerTick
            End With

            NLogger.Debug("Timer für das automatische Ausblenden gestoppt.")
        End If

    End Sub

    ''' <summary>
    ''' Tritt auf, wenn der Mauszeiger in den Bereich dieses Elements eintritt.
    ''' </summary>
    Private Sub StoppUhrWPF_MouseEnter(sender As Object, e As MouseEventArgs) Handles Me.MouseEnter
        If AusblendTimer IsNot Nothing Then
            ' Merke dir die aktuelle Zeit
            PauseTime = Now
            ' Halte den Timer an
            AusblendTimer.IsEnabled = False

            NLogger.Debug("Timer angehalten.")
        End If
    End Sub

    ''' <summary>
    ''' Tritt auf, wenn der Mauszeiger den Bereich dieses Elements verlässt.
    ''' </summary>
    Private Sub StoppUhrWPF_MouseLeave(sender As Object, e As MouseEventArgs) Handles Me.MouseLeave
        If AusblendTimer IsNot Nothing Then

            ' Merke die Zeit, die die Maus auf dem Anrufmonitor war.
            TotalTimePaused = TotalTimePaused.Add(Now.Subtract(PauseTime))
            ' Reaktiviere den Timer
            AusblendTimer.IsEnabled = True

            NLogger.Debug($"Timer nach {Now.Subtract(PauseTime).TotalSeconds} Sekunden fortgesetzt.")
        End If
    End Sub
#End Region

#Region "Timer für das automatische Ausblenden"
    Private Property StartTime As Date
    Private Property PauseTime As Date
    Private Property TotalTimePaused As TimeSpan
    Private Property EinblendZeit As TimeSpan

    ''' <summary>
    ''' Startet das automatische Ausblenden der Stoppuhr
    ''' </summary>
    ''' <param name="Verzögerung">Angabe is Sekunden, wie lange das Fenster eingeblendet sein soll.</param>
    Friend Sub StarteAusblendTimer(Verzögerung As Integer)
        ' Der Timer muss über den Dispatcher gestartet werde, da er ansonsten nicht ausgelöst wird.
        Dispatcher.Invoke(Sub()
                              ' Timer initialisieren
                              AusblendTimer = New Threading.DispatcherTimer

                              ' Einblendzeit übergeben
                              EinblendZeit = TimeSpan.FromSeconds(Verzögerung)

                              With AusblendTimer
                                  ' Intervall festlegen
                                  .Interval = TimeSpan.FromMilliseconds(100)
                                  ' Ereignishandler festlegen
                                  AddHandler .Tick, AddressOf AusblendTimerTick

                                  ' Startzeit festlegen
                                  StartTime = Date.Now

                                  'Timer Starten
                                  .Start()

                              End With


                          End Sub)
        NLogger.Debug($"Timer für automatisches Ausblenden gestartet.")
    End Sub


    Friend Sub AusblendTimerTick(sender As Object, e As EventArgs)
        If Now.Subtract(StartTime).Subtract(TotalTimePaused).TotalMilliseconds.IsLargerOrEqual(EinblendZeit.TotalMilliseconds) Then
            NLogger.Debug($"Popup {Name} nach {EinblendZeit.TotalSeconds} + {TotalTimePaused.TotalSeconds} Sekunden geschlossen.")

            ' Fenster schließen
            Close()
        End If
    End Sub


#End Region
End Class
