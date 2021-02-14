''' <summary>
''' Das Telefonbuch ist von Tosker erstellt worden und auf Youtube und Github bereitgestellt:
''' https://github.com/Tosker/ContactBook-Tutorial
''' Es wurde duch Kruemelino zu Zwecken der Anzeige von Fritz!Box Kontakten angepasst und erweitert.
''' </summary>
Public Class AppViewModel
    Inherits NotifyBase

    Private _currentView As Object

    Public Property CurrentView As Object
        Get
            Return _currentView
        End Get
        Set
            SetProperty(_currentView, Value)
        End Set
    End Property

    Private _bookVM As BookViewModel

    Public Property BookVM As BookViewModel
        Get
            Return _bookVM
        End Get
        Set
            SetProperty(_bookVM, Value)
        End Set
    End Property

    Public Sub New()
        Dim dataService = New ContactDataService()
        Dim dialogService = New WindowDialogService()
        BookVM = New BookViewModel(dataService, dialogService)
        CurrentView = BookVM
    End Sub
End Class
