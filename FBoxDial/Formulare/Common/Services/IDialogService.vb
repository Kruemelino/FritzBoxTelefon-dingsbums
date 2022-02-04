''' <summary>
''' Das Telefonbuch ist von Tosker erstellt worden und auf Youtube und Github bereitgestellt:
''' https://github.com/Tosker/ContactBook-Tutorial
''' Es wurde duch Kruemelino zu Zwecken der Anzeige von Fritz!Box Kontakten angepasst und erweitert.
''' </summary>
Public Interface IDialogService
    Function OpenFile(filter As String, Optional initialDirectory As String = "") As String
    Function SaveFile(filter As String, Optional initialDirectory As String = "", Optional initialFilename As String = "") As String
    Function ShowMessageBox(message As String) As Windows.MessageBoxResult
End Interface
