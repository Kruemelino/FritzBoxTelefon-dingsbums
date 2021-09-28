Imports System.Threading
Imports System.Windows
Imports System.Windows.Markup

Public Class TAMWPF
    Inherits Window

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        ' Startup Position festlegen
        WindowStartupLocation = WindowStartupLocation.CenterScreen

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)

        ' Blende das Window ein
        Show()
    End Sub
End Class
