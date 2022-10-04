Imports Microsoft.Office.Interop

Friend Module Fenster
#Region "Properties"
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property OInsp As Outlook.Inspector
#End Region

    ''' <summary>
    ''' Positioniert das Fenster mit dem <paramref name="hWnd"/> Topmost, jedoch mit folgenden Einschrängkungen:
    ''' <list type="bullet">
    ''' <item>DoNotActivate</item>
    ''' <item>IgnoreMove</item>
    ''' <item>IgnoreResize</item>
    ''' <item>DoNotChangeOwnerZOrder</item>
    ''' </list>
    ''' </summary>
    ''' <param name="hWnd">Window-Hanlde des einzublendenden Fensters</param>
    Friend Sub SetWindowPosPopUp(hWnd As IntPtr)
        ' Blende den Anrufmonitor Topmost, aber ohne Aktivierung, 
        UnsafeNativeMethods.SetWindowPos(hWnd,
                                         HWndInsertAfterFlags.HWND_TOPMOST,
                                         0, 0, 0, 0,
                                         SetWindowPosFlags.DoNotActivate Or
                                         SetWindowPosFlags.IgnoreMove Or
                                         SetWindowPosFlags.IgnoreResize Or
                                         SetWindowPosFlags.DoNotChangeOwnerZOrder)
    End Sub
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

        If Globals.ThisAddIn.Application IsNot Nothing Then
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
                    With Globals.ThisAddIn.Application
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

    Friend Function AddWindow(Of T As Windows.Window)() As T
        ' Blendet ein neue Kontaktsuche ein
        Dim AddinFenster As T = CType(Globals.ThisAddIn.AddinWindows.Find(Function(Window) TypeOf Window Is T), T)

        If AddinFenster Is Nothing Then
            ' Neues Window generieren
            AddinFenster = CType(Activator.CreateInstance(GetType(T)), T)
            ' Ereignishandler hinzufügen
            AddHandler AddinFenster.Closed, AddressOf Window_Closed
            ' Window in die Liste aufnehmen
            Globals.ThisAddIn.AddinWindows.Add(AddinFenster)

            NLogger.Debug($"Neues Fenster für '{AddinFenster.GetType.Name}' erzeugt.")
        Else
            NLogger.Debug($" Fenster für '{AddinFenster.GetType.Name}' bereits vorhanden.")

            AddinFenster.Activate()
        End If
        Return AddinFenster
    End Function

    Friend Sub Window_Closed(sender As Object, e As EventArgs)

        ' Window der Variable zuweisen
        Dim Window As Windows.Window = CType(sender, Windows.Window)
        ' Ereignishandler entfernen
        RemoveHandler Window.Closed, AddressOf Window_Closed
        ' Window aus der Liste entfernen
        Globals.ThisAddIn.AddinWindows.Remove(Window)

        NLogger.Debug($"Fenster '{Window.GetType.Name}' aus der Gesamtliste entfernt.")
    End Sub

End Module
