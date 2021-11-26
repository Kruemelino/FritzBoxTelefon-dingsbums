Imports System.Threading
Imports System.Windows
Imports System.Windows.Markup

Public Class StoppUhrWPF
    Inherits Window
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property WindowHelper As WindowHelper

#Region "Event"
    Public Event Geschlossen(sender As Object, e As EventArgs)
#End Region
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)

        ' Erzeuge die Klasse für das automatische Ausblenden
        WindowHelper = New WindowHelper(Me)
    End Sub

    ''' <summary>
    ''' Startet das automatische Ausblenden der Stoppuhr
    ''' </summary>
    Friend Sub StarteAusblendTimer(Intervall As TimeSpan)
        ' Timer für das Ausblenden starten
        WindowHelper.StartTimer(False, Intervall)
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

#End Region
End Class
