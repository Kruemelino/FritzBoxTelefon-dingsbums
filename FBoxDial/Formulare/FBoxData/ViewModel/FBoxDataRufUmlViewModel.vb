Public Class FBoxDataRufUmlViewModel
    Inherits NotifyBase
    Implements IFBoxData

    Public ReadOnly Property Name As String Implements IFBoxData.Name
        Get
            Return Localize.LocFBoxData.strRufUml
        End Get
    End Property

    Private _FBoxDataVM As FBoxDataViewModel
    Public Property FBoxDataVM As FBoxDataViewModel Implements IFBoxData.FBoxDataVM
        Get
            Return _FBoxDataVM
        End Get
        Set(value As FBoxDataViewModel)
            SetProperty(_FBoxDataVM, value)
        End Set
    End Property

    Public Property InitialSelected As Boolean = False Implements IFBoxData.InitialSelected

    Private Property DatenService As IFBoxDataService
    'Private Property DialogService As IDialogService


#Region "Listen"
    Private _RufUmlItemVM As RufUmlViewModel
    Public Property RufUmlItemVM As RufUmlViewModel
        Get
            Return _RufUmlItemVM
        End Get
        Set
            SetProperty(_RufUmlItemVM, Value)
        End Set
    End Property

    Private _RufUmlListe As ObservableCollectionEx(Of RufUmlViewModel)
    Public Property RufUmlListe As ObservableCollectionEx(Of RufUmlViewModel)
        Get
            Return _RufUmlListe
        End Get
        Set
            SetProperty(_RufUmlListe, Value)
        End Set
    End Property
#End Region

    Public Sub New(dataService As IFBoxDataService)
        _DatenService = dataService
        '_DialogService = dialogService
    End Sub
    Public Async Sub Init() Implements IFBoxData.Init

        RufUmlListe = New ObservableCollectionEx(Of RufUmlViewModel)

        Dim RufUml As DeflectionList = Await DatenService.GestDeflectionList

        If RufUml IsNot Nothing Then
            RufUmlListe.AddRange(RufUml.DeflectionListe.Select(Function(Defl) New RufUmlViewModel(DatenService, Defl)))
        End If

    End Sub


End Class
