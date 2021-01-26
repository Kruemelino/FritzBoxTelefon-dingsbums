Imports System.Windows.Controls

Public Class UserCtrlTelefone
    Inherits UserControl

    Private WithEvents FritzBoxDaten As Telefonie

    Private Sub BTelefonie_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BTelefonie.Click
        ' Speichern der Daten
        CType(DataContext, OptionenViewModel).Speichern()

        If Ping(XMLData.POptionen.ValidFBAdr) Then
            FritzBoxDaten = New Telefonie
            ' Einlesen starten
            FritzBoxDaten.GetFritzBoxDaten()
        End If

    End Sub

    Private Sub FritzBoxDaten_Status(sender As Object, e As NotifyEventArgs(Of String)) Handles FritzBoxDaten.Status
        LabelStatus.Text = e.Value
    End Sub

    Private Sub FritzBoxDaten_Beendet() Handles FritzBoxDaten.Beendet
        CType(DataContext, OptionenViewModel).LadeDaten()
    End Sub
End Class
