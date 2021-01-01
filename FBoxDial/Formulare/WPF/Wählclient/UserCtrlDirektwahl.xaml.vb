Imports System.Windows
Imports System.Windows.Controls
Public Class UserCtrlDirektwahl
    Inherits UserControl

    Public Shared ReadOnly DialEvent As RoutedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, GetType(RoutedEventHandler), GetType(UserCtrlDirektwahl))

    Public Custom Event Dial As RoutedEventHandler
        AddHandler(ByVal value As RoutedEventHandler)
            Me.AddHandler(DialEvent, value)
        End AddHandler

        RemoveHandler(ByVal value As RoutedEventHandler)
            Me.RemoveHandler(DialEvent, value)
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Me.RaiseEvent(e)
        End RaiseEvent
    End Event

    Private Sub BDirektwahl_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BDirektwahl.Click

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