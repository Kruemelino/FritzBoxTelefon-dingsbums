Imports System.Windows
Imports System.Windows.Controls
Public Class UserCtrlKontaktwahl
    Inherits UserControl

    Public Shared ReadOnly DialEvent As RoutedEvent = EventManager.RegisterRoutedEvent("Dial", RoutingStrategy.Bubble, GetType(RoutedEventHandler), GetType(UserCtrlKontaktwahl))

    Public Custom Event Dial As RoutedEventHandler
        AddHandler(value As RoutedEventHandler)
            Me.AddHandler(DialEvent, value)
        End AddHandler

        RemoveHandler(value As RoutedEventHandler)
            Me.RemoveHandler(DialEvent, value)
        End RemoveHandler

        RaiseEvent(sender As Object, e As RoutedEventArgs)
            Me.RaiseEvent(e)
        End RaiseEvent
    End Event

    Private Sub DGNummern_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles DGNummern.SelectionChanged

        ' Prüfe, ob es sich bei dem ausgewählten Objekt um eine Telefonnummer handelt.
        If e.AddedItems.Count.AreEqual(1) AndAlso TypeOf (e.AddedItems)(0) Is Telefonnummer Then
            With CType(DataContext, WählClientViewModel)
                ' Telefonnummer in ViewModel schreiben
                .TelNr = CType(e.AddedItems(0), Telefonnummer)
            End With
        End If

        [RaiseEvent](New RoutedEventArgs(DialEvent))

    End Sub
End Class