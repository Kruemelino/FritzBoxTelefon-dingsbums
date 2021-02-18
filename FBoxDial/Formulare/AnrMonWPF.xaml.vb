Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Markup

Public Class AnrMonWPF
    Inherits Window
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property AbstandAnrMon As Integer = 10
#Region "Dispatcher Timer"
    ''' <summary>
    ''' Timer für das automatische Ausblenden des Anrufmonitors.
    ''' So bald die gewählte Zeit erreicht ist, wird der Anrtufmonitor ausgeblendet.
    ''' Wenn die Maus sich auf dem Fenster befindet, wird der Timer unterbrochen.
    ''' Sobald sich die Maus vom dem Fenster entfernt, wird der Timer fortgesetzt.
    ''' </summary>
    Private AnrMonTimer As Threading.DispatcherTimer
#End Region

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Language = XmlLanguage.GetLanguage(System.Threading.Thread.CurrentThread.CurrentCulture.Name)

    End Sub

#Region "Timer"

    Private Property StartTime As Date
    Private Property PauseTime As Date
    Private Property TotalTimePaused As TimeSpan

    Private Sub AnrmonTimerTick(sender As Object, e As EventArgs)
        If Now.Subtract(StartTime).Subtract(TotalTimePaused).TotalMilliseconds.IsLargerOrEqual(XMLData.POptionen.TBEnblDauer * 1000) Then
            NLogger.Debug($"Anrufmonitor nach {XMLData.POptionen.TBEnblDauer} + {TotalTimePaused.TotalSeconds} Sekunden geschlossen.")

            ' Fenster schließen
            Close()
        End If
    End Sub

#End Region

#Region "Event"
    Public Event Geschlossen(sender As Object, e As EventArgs)
#End Region

    ''' <summary>
    ''' Tritt auf, wenn ein Fenster In den Vordergrund gesetzt wird.
    ''' </summary>
    Private Sub AnrMonWPF_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        NLogger.Debug("Activated")
        '' Blende den Anrufmonitor Topmost, aber ohne Focus ein
        'UnsafeNativeMethods.SetWindowPos(New Windows.Interop.WindowInteropHelper(Me).Handle,
        '                                 HWndInsertAfterFlags.HWND_TOPMOST, 0, 0, 0, 0,
        '                                 CType(SetWindowPosFlags.DoNotActivate + SetWindowPosFlags.IgnoreMove + SetWindowPosFlags.IgnoreResize + SetWindowPosFlags.DoNotChangeOwnerZOrder, SetWindowPosFlags))
        ' Setze Startposition
        ' X-Koordinate
        Left = SystemParameters.WorkArea.Right - Width - AbstandAnrMon

        ' Y-Koordinate
        Top = SystemParameters.WorkArea.Bottom - Height - AbstandAnrMon - ThisAddIn.OffeneAnrMonWPF.Count * (AbstandAnrMon + Height)

        ' Timer für das Ausblenden starten
        If XMLData.POptionen.CBAutoClose AndAlso AnrMonTimer Is Nothing Then
            AnrMonTimer = New Threading.DispatcherTimer

            With AnrMonTimer
                ' Intervall festlegen
                .Interval = TimeSpan.FromMilliseconds(10)

                ' Ereignishandler festlegen
                AddHandler .Tick, AddressOf AnrmonTimerTick
                ' Startzeit festlegen
                StartTime = Date.Now

                'Timer Starten
                .Start()

            End With

            NLogger.Debug($"Timer für automatisches Ausblenden gestartet.")
        End If
    End Sub

    ''' <summary>
    ''' Tritt kurz vor dem Schließen des Fensters auf.
    ''' </summary>
    Private Sub AnrMonWPF_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        RaiseEvent Geschlossen(Me, e)
    End Sub

    ''' <summary>
    ''' Tritt unmittelbar nach dem Aufruf von Close() auf und kann behandelt werden, um das Schließen des Fensters abzubrechen.
    ''' </summary>
    Private Sub AnrMonWPF_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If AnrMonTimer IsNot Nothing Then
            With AnrMonTimer
                ' Stoppe den Timer
                .Stop()
                ' Ereignishandler entfernen
                RemoveHandler .Tick, AddressOf AnrmonTimerTick
            End With

            NLogger.Debug("Timer für Anrufmonitor gestoppt.")
        End If
    End Sub

    ''' <summary>
    ''' Tritt auf, wenn der Mauszeiger in den Bereich dieses Elements eintritt.
    ''' </summary>
    Private Sub AnrMonWPF_MouseEnter(sender As Object, e As MouseEventArgs) Handles Me.MouseEnter
        If AnrMonTimer IsNot Nothing Then
            ' Merke dir die aktuelle Zeit
            PauseTime = Now
            ' Halte den Timer an
            AnrMonTimer.IsEnabled = False

            NLogger.Debug("Timer angehalten.")
        End If
    End Sub

    ''' <summary>
    ''' Tritt auf, wenn der Mauszeiger den Bereich dieses Elements verlässt.
    ''' </summary>
    Private Sub AnrMonWPF_MouseLeave(sender As Object, e As MouseEventArgs) Handles Me.MouseLeave
        If AnrMonTimer IsNot Nothing Then

            ' Merke die Zeit, die die Maus auf dem Anrufmonitor war.
            TotalTimePaused = TotalTimePaused.Add(Now.Subtract(PauseTime))
            ' Reaktiviere den Timer
            AnrMonTimer.IsEnabled = True

            NLogger.Debug($"Timer nach {Now.Subtract(PauseTime).TotalSeconds} Sekunden fortgesetzt.")
        End If
    End Sub

End Class
