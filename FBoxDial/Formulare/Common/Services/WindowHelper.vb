Imports System.Threading
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
    Private AusblendDispatcherTimer As Threading.DispatcherTimer

    Private AusblendTimer As Timers.Timer

    Private Dispatcher As Boolean

    Public Sub New(wndw As Window)
        Fenster = wndw

        ' Ereignishandler hinzufügen
        AddHandler Fenster.MouseEnter, AddressOf Fenster_MouseEnter
        AddHandler Fenster.MouseLeave, AddressOf Fenster_MouseLeave
    End Sub

    Friend Sub StartTimer(StartDispatcher As Boolean, Intervall As TimeSpan)

        Ausblendverzögerung = Intervall

        Dispatcher = StartDispatcher

        If Dispatcher Then
            ' Starte einen Dispatcher Timer
            AusblendDispatcherTimer = New Threading.DispatcherTimer

            With AusblendDispatcherTimer
                ' Intervall festlegen
                .Interval = TimeSpan.FromMilliseconds(100)

                ' Ereignishandler festlegen
                AddHandler .Tick, AddressOf TimerTick

                ' Startzeit festlegen
                StartTime = Date.Now

                'Timer Starten
                .Start()

            End With

        Else
            ' Starte einen Timers.Timer
            AusblendTimer = New Timers.Timer

            With AusblendTimer
                ' Intervall festlegen
                .Interval = 100

                ' Ereignishandler festlegen
                AddHandler .Elapsed, AddressOf TimerTick

                ' Startzeit festlegen
                StartTime = Date.Now

                'Timer Starten
                .Start()

            End With
        End If

        NLogger.Debug($"Timer für automatisches Ausblenden nach {Ausblendverzögerung.TotalSeconds} Sekunden gestartet.")
    End Sub

    Private Sub TimerTick(sender As Object, e As EventArgs)
        If TimeSpan.Compare(Now.Subtract(StartTime).Subtract(TotalTimePaused), Ausblendverzögerung).IsLargerOrEqual(0) Then
            NLogger.Debug($"Fenster nach {Now.Subtract(StartTime).TotalSeconds} + {TotalTimePaused.TotalSeconds} Sekunden geschlossen.")

            If Dispatcher Then
                With AusblendDispatcherTimer
                    ' Stoppe den Timer
                    .Stop()

                    ' Ereignishandler entfernen
                    RemoveHandler .Tick, AddressOf TimerTick

                End With
            Else
                With AusblendTimer
                    ' Stoppe den Timer
                    .Stop()

                    ' Ereignishandler entfernen
                    RemoveHandler .Elapsed, AddressOf TimerTick

                End With

            End If

            ' Ereignishandler entfernen
            RemoveHandler Fenster.MouseEnter, AddressOf Fenster_MouseEnter
            RemoveHandler Fenster.MouseLeave, AddressOf Fenster_MouseLeave

            NLogger.Debug("Timer für Schließen des Fensters gestoppt.")

            ' Fenster schließen
            If Fenster.Dispatcher.CheckAccess() Then
                Fenster.Close()
            Else
                Fenster.Dispatcher.Invoke(New ThreadStart(AddressOf Fenster.Close), Threading.DispatcherPriority.Normal)
            End If

        End If
    End Sub
    Private Sub Fenster_MouseEnter(sender As Object, e As MouseEventArgs)
        If AusblendDispatcherTimer IsNot Nothing Then
            ' Merke dir die aktuelle Zeit
            PauseTime = Now
            ' Halte den Timer an
            AusblendDispatcherTimer.IsEnabled = False

            NLogger.Debug("Timer angehalten.")
        End If
    End Sub

    ''' <summary>
    ''' Tritt auf, wenn der Mauszeiger den Bereich dieses Elements verlässt.
    ''' </summary>
    Private Sub Fenster_MouseLeave(sender As Object, e As MouseEventArgs)
        If AusblendDispatcherTimer IsNot Nothing Then

            ' Merke die Zeit, die die Maus auf dem Anrufmonitor war.
            TotalTimePaused = TotalTimePaused.Add(Now.Subtract(PauseTime))
            ' Reaktiviere den Timer
            AusblendDispatcherTimer.IsEnabled = True

            NLogger.Debug($"Timer nach {Now.Subtract(PauseTime).TotalSeconds} Sekunden fortgesetzt.")
        End If
    End Sub

    Friend Sub Close()
        RemoveHandler Fenster.MouseEnter, AddressOf Fenster_MouseEnter
        RemoveHandler Fenster.MouseLeave, AddressOf Fenster_MouseLeave

        NLogger.Debug("Timer für Schließen des Fensters gestoppt.")

        ' Fenster schließen
        If Fenster.Dispatcher.CheckAccess() Then
            Fenster.Close()
        Else
            Fenster.Dispatcher.Invoke(New ThreadStart(AddressOf Fenster.Close), Threading.DispatcherPriority.Normal)
        End If
    End Sub
#End Region

#Region "Position"
    Friend Function GetAnrMonPosition(Width As Double, Height As Double) As Point

        Return New Point With {
            .X = SystemParameters.WorkArea.Right - Width - XMLData.POptionen.TBAnrMonAbstand - XMLData.POptionen.TBAnrMonModPosX,
            .Y = SystemParameters.WorkArea.Bottom - Height - XMLData.POptionen.TBAnrMonAbstand - XMLData.POptionen.TBAnrMonModPosY - (Globals.ThisAddIn.OffeneAnrMonWPF.Count * (XMLData.POptionen.TBAnrMonAbstand + Height))
        }

    End Function
#End Region
End Class
