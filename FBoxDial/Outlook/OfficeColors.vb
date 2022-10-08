Imports System.ComponentModel
Imports System.Drawing
Imports Microsoft.Win32

<TypeConverter(GetType(EnumDescriptionTypeConverter))>
Public Enum DesignModes
    <LocalizedDescription("DesignOffice", GetType(resEnum))> Office
    <LocalizedDescription("DesignDark", GetType(resEnum))> Dark
    <LocalizedDescription("DesignLight", GetType(resEnum))> Light
End Enum

Friend Module OfficeColors

    Private ReadOnly Property ThemeLibName As String = "EasyWPFThemeLib"
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    ''' <summary>
    ''' Aktualisiert das Farbthema.
    ''' </summary>
    Friend Sub UpdateTheme()

        Dim ThemeName As String = "Light"

        Select Case XMLData.POptionen.CBoxDesignMode
            Case DesignModes.Office
                ThemeName = GetThemebyOffice()
            Case DesignModes.Dark, DesignModes.Light
                ThemeName = XMLData.POptionen.CBoxDesignMode.ToString()

        End Select

        SetResourceDictionary(GetActualTheme, New Uri($"pack://application:,,,/{ThemeLibName};component/Themes/{ThemeName}Theme.xaml"))

    End Sub

    Private Function GetOfficeThemeID() As Integer

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

        Return OfficeTheme
    End Function

    Private Function GetThemebyOffice() As String
        Return If(GetOfficeThemeID.AreEqual(4), "Dark", "Light")
    End Function

    ''' <summary>
    ''' Gibt die Hintergrundfarbe des Office-Themes zurück. Dies ist nicht sonderlich schön, aber momentan nicht anders machbar.
    ''' </summary>
    Friend Function GetOfficeBackGroundColor() As Color

        Select Case GetOfficeThemeID()
            Case 3 ' 2e2e2e
                Return ColorTranslator.FromHtml("#FF2E2E2E")
            Case 4 ' 0a0a0a
                Return ColorTranslator.FromHtml("#FF0A0A0A")
            Case Else ' f0f0f0
                Return ColorTranslator.FromHtml("#FFF0F0F0")
        End Select

    End Function

    Friend Sub ToogleTheme()
        ' Ermittle das aktuelle Theme
        ' Ermittle das aktuelle Theme Entweder DarkTheme oder LightTheme
        Dim ActualTheme As Windows.ResourceDictionary = GetActualTheme()

        If ActualTheme IsNot Nothing Then
            SetResourceDictionary(ActualTheme, New Uri($"pack://application:,,,/{ThemeLibName};component/Themes/{If(ActualTheme.Source.AbsoluteUri.EndsWith("LightTheme.xaml"), "Dark", "Light")}Theme.xaml"))
        Else
            ' Setze auf Standard zurück
            UpdateTheme()
        End If

    End Sub

    ''' <summary>
    ''' Setzt das Farbthema entsprechend dem übergebenden Parameter <paramref name="ThemeUri"/>.
    ''' </summary>
    Private Sub SetResourceDictionary(Theme As Windows.ResourceDictionary, ThemeUri As Uri)

        With Globals.ThisAddIn.WPFApplication.Resources.MergedDictionaries

            If Theme Is Nothing Then
                ' Es ist etwas schief gelaufen. Setze zurück.
                NLogger.Warn($"Setze Theme zurück auf: {ThemeUri}")
                Try
                    .Clear()
                    .Add(New Windows.ResourceDictionary With {.Source = ThemeUri})
                Catch ex As Exception
                    NLogger.Warn(ex, $"Fehler beim Zurücksetzen des Themes.")
                End Try

            Else
                ' Normalfall. Es sollten grundsätzlich nur ein Theme vorhanden sein!
                If Not Theme.Source.Equals(ThemeUri) Then
                    NLogger.Trace($"Wechsel Theme auf: {ThemeUri}")
                    Try
                        Theme.Source = ThemeUri
                    Catch ex As Exception
                        NLogger.Warn(ex, $"Fehler beim Wechsel des Themes.")
                    End Try

                End If
            End If

        End With
    End Sub

    ''' <summary>
    ''' Ermittelt das aktuell eingestellte Farbthema.
    ''' </summary>
    Private Function GetActualTheme() As Windows.ResourceDictionary

        With Globals.ThisAddIn.WPFApplication.Resources.MergedDictionaries
            Dim RDColours As IEnumerable(Of Windows.ResourceDictionary) = .Where(Function(rd) rd.Source.AbsoluteUri.EndsWith("Theme.xaml"))

            If RDColours.Count.AreEqual(1) Then
                Return RDColours.First
            Else
                ' Irgendwas ist schiefgegangen
                NLogger.Warn($"Es wurden { .Count} WPF Themes in den Resourcen gefunden.")
                Return Nothing
            End If

        End With

    End Function

End Module
