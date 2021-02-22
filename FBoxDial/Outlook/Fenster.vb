Imports Microsoft.Office.Interop
'Imports System.Windows
'Imports System.Windows.Interop

Friend Module Fenster
#Region "Properties"
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property OInsp As Outlook.Inspector
#End Region
    '''' <summary>
    '''' Prüft, ob ein Vollbildanwendung aktiv ist.
    '''' </summary>
    '''' <returns>True, wenn Vollbildanwendung erkannt, fals wenn nicht</returns>
    'Public Function VollBildAnwendungAktiv() As Boolean

    '    'Detect if the current app is running in full screen
    '    Dim AppBounds As RECT
    '    Dim screenBounds As Drawing.Rectangle
    '    Dim hWnd As IntPtr

    '    VollBildAnwendungAktiv = False

    '    'get the dimensions of the active window
    '    hWnd = UnSaveMethods.GetForegroundWindow()

    '    If Not hWnd = IntPtr.Zero Then
    '        ' Check we haven't picked up the desktop or the shell
    '        If Not (hWnd.Equals(UnSaveMethods.GetDesktopWindow) Or hWnd.Equals(UnSaveMethods.GetShellWindow)) Then

    '            AppBounds = UnSaveMethods.GetWindowRect(hWnd)
    '            'determine if window is fullscreen
    '            screenBounds = Forms.Screen.FromHandle(hWnd).Bounds
    '            If (AppBounds.Bottom - AppBounds.Top).AreEqual(screenBounds.Height) And (AppBounds.Right - AppBounds.Left).AreEqual(screenBounds.Width) Then

    '                VollBildAnwendungAktiv = True
    '                NLogger.Info("Eine aktive Vollbildanwendung wurde detektiert.")
    '            End If
    '        End If
    '    End If
    'End Function

    ''' <summary>
    ''' Sinn der Routine ist es einen aktiven Inspector wieder aktiv zu schalten, da der Anrufmonitor diesen deaktiviert.
    ''' Nachdem der Anrufmonitor eingeblendet wurde, muss der Inspector wieder aktiviert werden.
    ''' Zuvor müssen zwei Dinge geprüft werden:
    ''' 1. Hat ein Outlookfenster (Inspector) gerade den Focus: (.ActiveWindow Is .ActiveInspector)
    ''' 2. Ist das aktuell aktive Fenster der Inspector (OutlookSecurity.GetWindowText(OutlookSecurity.GetForegroundWindow) = .ActiveInspector.Caption)
    ''' Um den ganzen vorgang abschließen zu können, wird der Inspector zwischengespeichert und nachdem der Anrufmonitor eingeblendet wurde wieder aktiviert.
    ''' </summary>
    ''' <param name="Activate">Gibt an, ob der Inspector aktiviert werden soll (true) oder ob er gespeichert werden soll (false)</param>
    Friend Sub KeepoInspActivated(Activate As Boolean)

        If ThisAddIn.OutookApplication IsNot Nothing Then
            If Activate Then
                If OInsp IsNot Nothing Then
                    If Not OInsp.WindowState = Outlook.OlWindowState.olMinimized Then
                        With OInsp
                            NLogger.Debug($"Outlook Inspektor '{ .Caption}' reaktiviert.")
                            .Activate()
                        End With
                        OInsp = Nothing
                    End If
                End If
            Else
                If OInsp Is Nothing Then
                    With ThisAddIn.OutookApplication
                        If .ActiveWindow Is .ActiveInspector Then
                            If UnSaveMethods.GetWindowText(UnSaveMethods.GetForegroundWindow) = .ActiveInspector.Caption Then
                                NLogger.Debug($"Aktiver Outlook Inspektor '{ .ActiveInspector.Caption}' detektiert.")
                                OInsp = .ActiveInspector()
                            End If
                        End If
                    End With
                End If
            End If
        End If
    End Sub

End Module
