Imports System.Windows.Controls

Public Class UserCtrlTelefone
    Inherits UserControl

    Private WithEvents FritzBoxDaten As Telefonie

    Private Sub BTelefonie_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BTelefonie.Click
        ' Speichern der Daten
        With CType(DataContext, OptionenViewModel)
            .Speichern()
            .EinlesenInaktiv = False
        End With


        If Ping(XMLData.POptionen.ValidFBAdr) Then
            FritzBoxDaten = New Telefonie
            ' Einlesen starten
            FritzBoxDaten.GetFritzBoxDaten()
        End If

    End Sub

    Private Sub FritzBoxDaten_Status(sender As Object, e As NotifyEventArgs(Of String)) Handles FritzBoxDaten.Status
        Status.AppendText(e.Value & Environment.NewLine)
    End Sub

    Private Sub FritzBoxDaten_Beendet() Handles FritzBoxDaten.Beendet
        With CType(DataContext, OptionenViewModel)
            .LadeDaten()
        End With

    End Sub
End Class
