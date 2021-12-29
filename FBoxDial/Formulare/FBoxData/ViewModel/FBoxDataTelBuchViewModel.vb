''' <summary>
''' Das Telefonbuch ist von Tosker erstellt worden und auf Youtube und Github bereitgestellt:
''' https://github.com/Tosker/ContactBook-Tutorial
''' Es wurde duch Kruemelino zu Zwecken der Anzeige von Fritz!Box Kontakten angepasst und erweitert.
''' </summary>
Public Class FBoxDataTelBuchViewModel
    Inherits NotifyBase
    Implements IFBoxData

    Private Property DatenService As IFBoxDataService
    Private Property DialogService As IDialogService

    Public ReadOnly Property Name As String Implements IFBoxData.Name
        Get
            Return Localize.LocFBoxData.strTelBuch
        End Get
    End Property

    Private _FBoxDataVM As FBoxDataViewModel
    Public Property FBoxDataVM As FBoxDataViewModel Implements IFBoxData.FBoxDataVM
        Get
            Return _FBoxDataVM
        End Get
        Set
            SetProperty(_FBoxDataVM, Value)
        End Set
    End Property

    Public Property InitialSelected As Boolean = False Implements IFBoxData.InitialSelected

    Private _currentView As Object
    Public Property CurrentView As Object
        Get
            Return _currentView
        End Get
        Set
            SetProperty(_currentView, Value)
        End Set
    End Property

    Private _bookVM As TelefonbuchViewModel
    Public Property BookVM As TelefonbuchViewModel
        Get
            Return _bookVM
        End Get
        Set
            SetProperty(_bookVM, Value)
        End Set
    End Property

    Public Sub New(dataService As IFBoxDataService, dialogService As IDialogService)
        ' Interface
        _DatenService = dataService
        _DialogService = dialogService
    End Sub

    Public Async Sub Init() Implements IFBoxData.Init
        BookVM = New TelefonbuchViewModel(DatenService, DialogService)
        CurrentView = BookVM

        BookVM.InitTelefonbücher(Await DatenService.GetTelefonbücher())
    End Sub
End Class
