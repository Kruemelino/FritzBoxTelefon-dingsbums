﻿Imports System.Windows
Imports System.Threading
Imports System.Windows.Markup

Public Class OptionenWPF
    Inherits Window

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Startup Position festlegen
        WindowStartupLocation = WindowStartupLocation.CenterScreen

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)

        ' Blende das Window ein
        Show()
    End Sub

End Class