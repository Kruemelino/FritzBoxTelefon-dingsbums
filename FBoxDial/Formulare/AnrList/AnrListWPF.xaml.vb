Imports System.Threading
Imports System.Windows
Imports System.Windows.Markup

Partial Public Class AnrListWPF
    Inherits Window

    'Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)

    End Sub

End Class


