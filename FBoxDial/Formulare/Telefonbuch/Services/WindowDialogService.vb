Imports Microsoft.Win32
''' <summary>
''' Das Telefonbuch ist von Tosker erstellt worden und auf Youtube und Github bereitgestellt:
''' https://github.com/Tosker/ContactBook-Tutorial
''' Es wurde duch Kruemelino zu Zwecken der Anzeige von Fritz!Box Kontakten angepasst und erweitert.
''' </summary>
Public Class WindowDialogService
    Implements IDialogService

    Public Function OpenFile(filter As String) As String Implements IDialogService.OpenFile
        Dim dialog = New OpenFileDialog()

        If dialog.ShowDialog() = True Then
            Return dialog.FileName
        End If

        Return Nothing
    End Function

    Public Function ShowMessageBox(Frage As String) As Windows.MessageBoxResult Implements IDialogService.ShowMessageBox

        Return Windows.MessageBox.Show(Frage, My.Resources.strDefLongName, Windows.MessageBoxButton.YesNo)

    End Function
End Class

