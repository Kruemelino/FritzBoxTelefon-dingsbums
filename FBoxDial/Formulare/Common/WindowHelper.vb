Imports System.Windows
Imports System.Windows.Input

Friend Class WindowHelper
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property Fenster As Window
    Private Property StartTime As Date
    Private Property PauseTime As Date
    Private Property TotalTimePaused As TimeSpan
    Private Property Ausblendverzögerung As TimeSpan

#Region "Dispatcher Timer"
    ''' <summary>
    ''' Timer für das automatische Ausblenden des Fensters.
    ''' So bald die gewählte Zeit erreicht ist, wird das Fenster ausgeblendet.
    ''' Wenn die Maus sich auf dem Fenster befindet, wird der Timer unterbrochen.
    ''' Sobald sich die Maus vom dem Fenster entfernt, wird der Timer fortgesetzt.
    ''' </summary>
    Private AusblendTimer As Threading.DispatcherTimer
#End Region

    Public Sub New(wndw As Window, Intervall As TimeSpan)
        Fenster = wndw
        Ausblendverzögerung = Intervall

        ' Ereignishandler hinzufügen
        AddHandler Fenster.MouseEnter, AddressOf Fenster_MouseEnter
        AddHandler Fenster.MouseLeave, AddressOf Fenster_MouseLeave
    End Sub

    Friend Sub StartTimer()
        AusblendTimer = New Threading.DispatcherTimer

        With AusblendTimer
            ' Intervall festlegen
            .Interval = TimeSpan.FromMilliseconds(100)

            ' Ereignishandler festlegen
            AddHandler .Tick, AddressOf TimerTick

            ' Startzeit festlegen
            StartTime = Date.Now

            'Timer Starten
            .Start()

        End With

        NLogger.Debug($"Timer für automatisches Ausblenden gestartet.")
    End Sub

    Private Sub TimerTick(sender As Object, e As EventArgs)
        If TimeSpan.Compare(Now.Subtract(StartTime).Subtract(TotalTimePaused), Ausblendverzögerung).IsLargerOrEqual(0) Then
            NLogger.Debug($"Fenster nach {XMLData.POptionen.TBEnblDauer} + {TotalTimePaused.TotalSeconds} Sekunden geschlossen.")

            With AusblendTimer
                ' Stoppe den Timer
                .Stop()

                ' Ereignishandler entfernen
                RemoveHandler .Tick, AddressOf TimerTick

            End With

            ' Ereignishandler entfernen
            RemoveHandler Fenster.MouseEnter, AddressOf Fenster_MouseEnter
            RemoveHandler Fenster.MouseLeave, AddressOf Fenster_MouseLeave

            NLogger.Debug("Timer für Schließen des Fensters gestoppt.")

            ' Fenster schließen
            Fenster.Close()
        End If
    End Sub
    Private Sub Fenster_MouseEnter(sender As Object, e As MouseEventArgs)
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
    Private Sub Fenster_MouseLeave(sender As Object, e As MouseEventArgs)
        If AusblendTimer IsNot Nothing Then

            ' Merke die Zeit, die die Maus auf dem Anrufmonitor war.
            TotalTimePaused = TotalTimePaused.Add(Now.Subtract(PauseTime))
            ' Reaktiviere den Timer
            AusblendTimer.IsEnabled = True

            NLogger.Debug($"Timer nach {Now.Subtract(PauseTime).TotalSeconds} Sekunden fortgesetzt.")
        End If
    End Sub
End Class
