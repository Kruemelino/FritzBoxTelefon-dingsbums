Imports System.Threading
Imports System.Windows
Imports System.Windows.Markup

Public Class WählclientWPF
    Inherits Window
    Private Property WindowHelper As WindowHelper

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        ' Startup Position festlegen
        WindowStartupLocation = WindowStartupLocation.CenterScreen

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)

        ' Erzeuge die Klasse für das automatische Ausblenden
        WindowHelper = New WindowHelper(Me)
    End Sub

    ''' <summary>
    ''' Startet das automatische Ausblenden des Wählfensters.
    ''' </summary>
    Friend Sub StarteAusblendTimer(Intervall As TimeSpan)
        ' Timer für das Ausblenden starten
        WindowHelper.StartTimer(False, Intervall)
    End Sub

End Class
