Imports System.ComponentModel
Imports System.Windows
Imports System.Windows.Input

''' <summary>
''' https://www.codeproject.com/Articles/73251/Handling-a-Window-s-Closed-and-Closing-events-in-t
''' </summary>
Public Class WindowBehavior

#Region "Loaded"

    Public Shared Function GetLoaded(obj As DependencyObject) As ICommand
        Return CType(obj.GetValue(LoadedProperty), ICommand)
    End Function

    Public Shared Sub SetLoaded(obj As DependencyObject, value As ICommand)
        obj.SetValue(LoadedProperty, value)
    End Sub

    Public Shared ReadOnly LoadedProperty As DependencyProperty = DependencyProperty.RegisterAttached("Loaded", GetType(ICommand), GetType(WindowBehavior), New UIPropertyMetadata(New PropertyChangedCallback(AddressOf LoadedChanged)))

    Private Shared Sub LoadedChanged(target As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim window As Window = TryCast(target, Window)

        If window IsNot Nothing Then

            If e.NewValue IsNot Nothing Then
                AddHandler window.Loaded, AddressOf Window_Loaded
            Else
                RemoveHandler window.Loaded, AddressOf Window_Loaded
            End If
        End If
    End Sub
    Private Shared Sub Window_Loaded(sender As Object, e As EventArgs)
        Dim loaded As ICommand = GetLoaded(TryCast(sender, Window))

        If loaded IsNot Nothing Then loaded.Execute(Nothing)
    End Sub
#End Region

#Region "Closed"
    Public Shared Function GetClosed(obj As DependencyObject) As ICommand
        Return CType(obj.GetValue(ClosedProperty), ICommand)
    End Function

    Public Shared Sub SetClosed(obj As DependencyObject, value As ICommand)
        obj.SetValue(ClosedProperty, value)
    End Sub

    Public Shared ReadOnly ClosedProperty As DependencyProperty = DependencyProperty.RegisterAttached("Closed", GetType(ICommand), GetType(WindowBehavior), New UIPropertyMetadata(New PropertyChangedCallback(AddressOf ClosedChanged)))

    Private Shared Sub ClosedChanged(target As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim window As Window = TryCast(target, Window)

        If window IsNot Nothing Then

            If e.NewValue IsNot Nothing Then
                AddHandler window.Closed, AddressOf Window_Closed
            Else
                RemoveHandler window.Closed, AddressOf Window_Closed
            End If
        End If
    End Sub
    Private Shared Sub Window_Closed(sender As Object, e As EventArgs)
        Dim closed As ICommand = GetClosed(TryCast(sender, Window))

        If closed IsNot Nothing Then closed.Execute(Nothing)
    End Sub
#End Region

#Region "Closing"
    Public Shared Function GetClosing(obj As DependencyObject) As ICommand
        Return CType(obj.GetValue(ClosingProperty), ICommand)
    End Function

    Public Shared Sub SetClosing(obj As DependencyObject, value As ICommand)
        obj.SetValue(ClosingProperty, value)
    End Sub

    Public Shared ReadOnly ClosingProperty As DependencyProperty = DependencyProperty.RegisterAttached("Closing", GetType(ICommand), GetType(WindowBehavior), New UIPropertyMetadata(New PropertyChangedCallback(AddressOf ClosingChanged)))

    Private Shared Sub ClosingChanged(target As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        Dim window As Window = TryCast(target, Window)

        If window IsNot Nothing Then

            If e.NewValue IsNot Nothing Then
                AddHandler window.Closing, AddressOf Window_Closing
            Else
                RemoveHandler window.Closing, AddressOf Window_Closing
            End If
        End If
    End Sub

    Public Shared Function GetCancelClosing(obj As DependencyObject) As ICommand
        Return CType(obj.GetValue(CancelClosingProperty), ICommand)
    End Function

    Public Shared Sub SetCancelClosing(obj As DependencyObject, value As ICommand)
        obj.SetValue(CancelClosingProperty, value)
    End Sub

    Public Shared ReadOnly CancelClosingProperty As DependencyProperty = DependencyProperty.RegisterAttached("CancelClosing", GetType(ICommand), GetType(WindowBehavior))

    Private Shared Sub Window_Closing(sender As Object, e As CancelEventArgs)
        Dim closing As ICommand = GetClosing(TryCast(sender, Window))

        If closing IsNot Nothing Then

            If closing.CanExecute(Nothing) Then
                closing.Execute(Nothing)
            Else
                Dim cancelClosing As ICommand = GetCancelClosing(TryCast(sender, Window))

                If cancelClosing IsNot Nothing Then cancelClosing.Execute(Nothing)

                e.Cancel = True
            End If
        End If
    End Sub
#End Region

End Class
