Imports System.Windows
Imports System.Windows.Input
Imports Microsoft.Xaml.Behaviors

''' <summary>
''' Captures and eats MouseWheel events so that a nested <see cref="UIElement"/> does not
''' prevent an outer scrollable control from scrolling.<br/>
''' <c>https://stackoverflow.com/a/7003338</c>
''' </summary>
Public NotInheritable Class PassthroughMouseWheelBehavior
    Inherits Behavior(Of UIElement)

    Protected Overrides Sub OnAttached()
        MyBase.OnAttached()

        AddHandler AssociatedObject.PreviewMouseWheel, AddressOf AssociatedObject_PreviewMouseWheel
    End Sub

    Protected Overrides Sub OnDetaching()
        RemoveHandler AssociatedObject.PreviewMouseWheel, AddressOf AssociatedObject_PreviewMouseWheel

        MyBase.OnDetaching()
    End Sub

    Private Sub AssociatedObject_PreviewMouseWheel(ByVal sender As Object, ByVal e As MouseWheelEventArgs)
        e.Handled = True
        Dim e2 = New MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) With {.RoutedEvent = UIElement.MouseWheelEvent}
        AssociatedObject?.[RaiseEvent](e2)
    End Sub
End Class