Public Class FBoxDataRufUmlViewModel
    Inherits NotifyBase
    Implements IFBoxData

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Public ReadOnly Property Name As String Implements IFBoxData.Name
        Get
            Return Localize.LocFBoxData.strRufUml
        End Get
    End Property

    Private Property DebugBeginnLadeDaten As Date Implements IFBoxData.DebugBeginnLadeDaten

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

#Region "Listen"
    Public Property RufUmlListe As ObservableCollectionEx(Of RufUmlViewModel)
#End Region

    Public Sub New(dataService As IFBoxDataService)
        _DatenService = dataService
    End Sub
    Private Async Sub Init() Implements IFBoxData.Init

        Dim FBoxDeflections As FBoxAPI.DeflectionList = Await DatenService.GetDeflectionList
        If FBoxDeflections IsNot Nothing AndAlso FBoxDeflections.Deflections IsNot Nothing Then

            RufUmlListe = New ObservableCollectionEx(Of RufUmlViewModel)(FBoxDeflections.Deflections.Select(Function(Defl) New RufUmlViewModel(DatenService) With {.Deflection = Defl, .Enable = Defl.Enable}))

            ' Debugmeldung
            NLogger.Debug($"Ende: Lade Daten für {Name} in {(Date.Now - DebugBeginnLadeDaten).TotalSeconds} Sekunden")

        End If

    End Sub
End Class