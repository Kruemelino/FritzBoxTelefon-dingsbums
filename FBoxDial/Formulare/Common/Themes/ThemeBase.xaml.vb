Imports System.Windows
Partial Public Class ThemeBase
    Private Sub CloseWindow_Event(sender As Object, e As RoutedEventArgs)
        If e.Source IsNot Nothing Then
            Try
                CloseWind(Window.GetWindow(CType(e.Source, FrameworkElement)))
            Catch
            End Try
        End If
    End Sub

    Private Sub AutoMinimize_Event(sender As Object, e As RoutedEventArgs)
        If e.Source IsNot Nothing Then

            Try
                MaximizeRestore(Window.GetWindow(CType(e.Source, FrameworkElement)))
            Catch
            End Try
        End If
    End Sub

    Private Sub Minimize_Event(sender As Object, e As RoutedEventArgs)
        If e.Source IsNot Nothing Then
            Try
                MinimizeWind(Window.GetWindow(CType(e.Source, FrameworkElement)))
            Catch
            End Try
        End If
    End Sub

    Public Sub CloseWind(window As Window)
        window.Close()
    End Sub

    Public Sub MaximizeRestore(window As Window)
        If window.WindowState = WindowState.Maximized Then
            window.WindowState = WindowState.Normal
        ElseIf window.WindowState = WindowState.Normal Then
            window.WindowState = WindowState.Maximized
        End If
    End Sub

    Public Sub MinimizeWind(window As Window)
        window.WindowState = WindowState.Minimized
    End Sub
End Class
