Imports Microsoft.Win32

Module OfficeColors

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Sub UpdateTheme()

        Dim OfficeVersion As Integer = FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName).ProductMajorPart
        Dim OfficeThemeKey As String = "UI Theme"
        ' 0: 
        ' 1:
        ' 2: 
        ' 3: Dunkelgrau
        ' 4: Schwarz
        ' 5: Weiß
        ' 6: System - Wechsel zwischen Schwarz und Weiß
        ' 7: Bunt

        Dim OfficeTheme As Integer = 5
        '  FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName).ProductMajorPart
        Using key = Registry.CurrentUser.OpenSubKey($"Software\Microsoft\Office\{OfficeVersion}.0\Common", False)
            OfficeTheme = CInt(key.GetValue(OfficeThemeKey))
        End Using

        NLogger.Debug($"Office UI Theme: {OfficeTheme}")

        If OfficeTheme = 6 Then
            Using key = Registry.CurrentUser.OpenSubKey($"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", False)
                Try
                    OfficeTheme = If(CInt(key.GetValue("AppsUseLightTheme")).IsZero, 4, 5)
                    NLogger.Debug($"Windows AppsUseLightTheme: {OfficeTheme - 4}")
                Catch

                End Try

            End Using
        End If

        SetResourceDictionary(If(OfficeTheme.AreEqual(4), "Dark", "Light"))

    End Sub

    Private Sub SetResourceDictionary(ThemeMode As String)
        Dim ThemeUri As New Uri($"pack://application:,,,/Fritz!Box Telefon-Dingsbums;component/Formulare/Common/Themes/{ThemeMode}Theme.xaml")

        With Globals.ThisAddIn.WPFApplication.Resources.MergedDictionaries

            ' Finde das Theme. Entweder DarkTheme oder LightTheme
            Dim RDColours As IEnumerable(Of Windows.ResourceDictionary) = .Where(Function(rd) rd.Source.AbsoluteUri.EndsWith("Theme.xaml"))

            If RDColours.Count.AreEqual(1) Then
                ' Es sollten grundsätzlich (nur) ein Theme vorhanden sein!
                If Not .First.Source.Equals(ThemeUri) Then
                    NLogger.Trace($"Wechsel Theme auf: {ThemeUri}")
                    Try
                        .First.Source = ThemeUri
                    Catch ex As Exception
                        NLogger.Warn(ex, $"Fehler beim Wechsel des Themes.")
                    End Try

                End If
            Else
                ' Irgendwas ist schiefgegangen
                NLogger.Warn($"Es wurden { .Count} WPF Themes in den Resourcen gefunden. Setze Themes zurück: {ThemeUri}")
                Try
                    .Clear()
                    .Add(New Windows.ResourceDictionary With {.Source = ThemeUri})
                Catch ex As Exception
                    NLogger.Warn(ex, $"Fehler beim Zurücksetzen des Themes.")
                End Try
            End If
        End With
    End Sub

End Module
