﻿Imports Microsoft.Win32
''' <summary>
''' Das Telefonbuch ist von Tosker erstellt worden und auf Youtube und Github bereitgestellt:
''' https://github.com/Tosker/ContactBook-Tutorial
''' Es wurde duch Kruemelino zu Zwecken der Anzeige von Fritz!Box Kontakten angepasst und erweitert.
''' </summary>
Public Class DialogService
    Implements IDialogService

    Public Function OpenFile(filter As String, Optional initialDirectory As String = "") As String Implements IDialogService.OpenFile
        Dim dialog = New OpenFileDialog() With {.Multiselect = False,
                                                .Filter = filter,
                                                .InitialDirectory = If(initialDirectory.IsStringNothingOrEmpty, String.Empty, IO.Path.GetDirectoryName(initialDirectory))}

        Return If(dialog.ShowDialog() = True, dialog.FileName, String.Empty)

    End Function

    Public Function SaveFile(filter As String, Optional initialDirectory As String = "", Optional initialFilename As String = "") As String Implements IDialogService.SaveFile
        Dim dialog = New SaveFileDialog() With {.Filter = filter,
                                                .InitialDirectory = IO.Path.GetDirectoryName(initialDirectory),
                                                .AddExtension = True, .FileName = initialFilename}

        Return If(dialog.ShowDialog() = True, dialog.FileName, String.Empty)

    End Function

    Public Function ShowMessageBox(Frage As String) As Windows.MessageBoxResult Implements IDialogService.ShowMessageBox

        Return Windows.MessageBox.Show(Frage, My.Resources.strDefLongName, Windows.MessageBoxButton.YesNo)

    End Function
End Class

