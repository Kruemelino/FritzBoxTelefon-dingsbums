Imports System.Windows
Imports System.Windows.Markup
Imports System.Threading

Public Class AnrMonWPF
    Inherits Window
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property WindowHelper As WindowHelper

#Region "Event"
    Public Event Geschlossen(sender As Object, e As EventArgs)
#End Region

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)

        ' Erzeuge die Klasse für das automatische Ausblenden
        WindowHelper = New WindowHelper(Me, TimeSpan.FromSeconds(XMLData.POptionen.TBEnblDauer))
    End Sub

    ''' <summary>
    ''' Tritt ein, wenn dieses <see cref="FrameworkElement"/> initialisiert wird. Dieses Ereignis geht mit Fällen einher, 
    ''' in denen sich der Wert der <see cref="FrameworkElement.IsInitialized"/>-Eigenschaft von false (oder nicht definiert) in true ändert.
    ''' </summary>
    Private Sub AnrMonWPF_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        NLogger.Trace("Initialized")
        Const AbstandAnrMon As Integer = 10

        ' Setze Startposition
        ' X-Koordinate
        Left = SystemParameters.WorkArea.Right - Width - AbstandAnrMon

        ' Y-Koordinate
        Top = SystemParameters.WorkArea.Bottom - Height - AbstandAnrMon - ThisAddIn.OffeneAnrMonWPF.Count * (AbstandAnrMon + Height)

        ' Outlook Inspektoren beachten
        KeepoInspActivated(False)
    End Sub

    ''' <summary>
    ''' Tritt auf, wenn das Element ausgerichtet und gerendert sowie zur Interaktion vorbereitet wurde.
    ''' </summary>
    Private Sub AnrMonWPF_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        NLogger.Trace("Loaded")

        ' Blende den Anrufmonitor Topmost, aber ohne Aktivierung
        SetWindowPosPopUp(New Interop.WindowInteropHelper(Me).Handle)

        NLogger.Debug("Anrufmonitor positioniert")

        ' Outlook Inspektor reaktivieren
        KeepoInspActivated(True)

        ' Timer für das Ausblenden starten
        If XMLData.POptionen.CBAutoClose Then WindowHelper.StartTimer(True)

    End Sub
    ''' <summary>
    ''' Tritt kurz vor dem Schließen des Fensters auf.
    ''' </summary>
    Private Sub AnrMonWPF_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        RaiseEvent Geschlossen(Me, e)
    End Sub

End Class
