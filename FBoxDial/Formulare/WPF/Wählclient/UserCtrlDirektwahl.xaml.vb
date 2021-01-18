Imports System.Windows
Imports System.Windows.Controls
Public Class UserCtrlDirektwahl
    Inherits UserControl

    Public Shared ReadOnly DialEvent As RoutedEvent = EventManager.RegisterRoutedEvent("Dial", RoutingStrategy.Bubble, GetType(RoutedEventHandler), GetType(UserCtrlDirektwahl))

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

    Private Sub BDirektwahl_Click(sender As Object, e As RoutedEventArgs) Handles BDirektwahl.Click

        ' Prüfe ob es sich um eine gültige Eingabe handelt
        If CBoxDirektwahl.Text.IsNotStringNothingOrEmpty Then
            With CType(DataContext, WählClientViewModel)
                ' Telefonnummer in ViewModel schreiben
                .TelNr = New Telefonnummer With {.SetNummer = CBoxDirektwahl.Text}
            End With

            [RaiseEvent](New RoutedEventArgs(DialEvent))

        End If
    End Sub
End Class