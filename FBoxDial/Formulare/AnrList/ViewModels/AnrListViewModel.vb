Imports System.Collections

Public Class AnrListViewModel
    Inherits NotifyBase
    Implements IPageListViewModel
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IListService
    Private Property DialogService As IDialogService
#Region "Felder"
    Public ReadOnly Property Name As String Implements IPageListViewModel.Name
        Get
            Return Localize.LocAnrList.strFBoxAnrufliste
        End Get
    End Property

    Private _ListVM As ListViewModel
    Public Property ListVM As ListViewModel Implements IPageListViewModel.ListVM
        Get
            Return _ListVM
        End Get
        Set
            SetProperty(_ListVM, Value)
        End Set
    End Property

    Public Property InitialSelected As Boolean = True Implements IPageListViewModel.InitialSelected

    Private _StartDatum As Date
    Public Property StartDatum As Date
        Get
            Return _StartDatum
        End Get
        Set
            SetProperty(_StartDatum, Value)

            ' Selektiere Alle Anrufe im ausgewählten Zeitraum
            SelectItems()
        End Set
    End Property

    Private _StartZeit As TimeSpan
    Public Property StartZeit As TimeSpan
        Get
            Return _StartZeit
        End Get
        Set
            SetProperty(_StartZeit, Value)

            ' Selektiere Alle Anrufe im ausgewählten Zeitraum
            SelectItems()
        End Set
    End Property

    Private _EndDatum As Date
    Public Property EndDatum As Date
        Get
            Return _EndDatum
        End Get
        Set
            SetProperty(_EndDatum, Value)

            ' Selektiere Alle Anrufe im ausgewählten Zeitraum
            SelectItems()
        End Set
    End Property

    Private _EndZeit As TimeSpan

    Public Property EndZeit As TimeSpan
        Get
            Return _EndZeit
        End Get
        Set
            SetProperty(_EndZeit, Value)

            ' Selektiere Alle Anrufe im ausgewählten Zeitraum
            SelectItems()
        End Set
    End Property

    Private _CancelationPending As Boolean
    Private Property CancelationPending As Boolean
        Get
            Return _CancelationPending
        End Get
        Set
            SetProperty(_CancelationPending, Value)
        End Set
    End Property

    Private _ImportProgressValue As Double
    Public Property ImportProgressValue As Double
        Get
            Return _ImportProgressValue
        End Get
        Set
            SetProperty(_ImportProgressValue, Value)
        End Set
    End Property

    Private _ImportProgressMax As Double
    Public Property ImportProgressMax As Double
        Get
            Return _ImportProgressMax
        End Get
        Set
            SetProperty(_ImportProgressMax, Value)
        End Set
    End Property
#End Region

#Region "ICommand"
    Public Property CancelCommand As RelayCommand
    Public Property ImportCommand As RelayCommand
    Public Property SelectAllCommand As RelayCommand
    Public Property BlockCommand As RelayCommand
#End Region

    Public Sub New()
        ' Commands
        CancelCommand = New RelayCommand(AddressOf CancelProcess)
        ImportCommand = New RelayCommand(AddressOf JournalImport)
        SelectAllCommand = New RelayCommand(AddressOf SelectAll)
        BlockCommand = New RelayCommand(AddressOf BlockNumbers)

        ' Interface
        DatenService = New ListService
        DialogService = New DialogService

    End Sub

    Private Sub SelectAll(o As Object)
        For Each Anruf In ListVM.CallList
            Anruf.Export = CBool(o)
        Next
    End Sub

    Private Sub SelectItems()

        If ListVM.CallList IsNot Nothing AndAlso ListVM.CallList.Any Then

            ' Ausgewählten Zeitraum ermitteln
            ' Startpunkt
            Dim ImportStart As Date = StartDatum.Add(StartZeit)

            ' Endzeitpunkt
            Dim ImportEnde As Date = EndDatum.Add(EndZeit)

            Dim AusgewählteAnrufe As IEnumerable(Of FritzBoxXMLCall)

            ' Ermittle alle Einträge, die im ausgewählten Bereich liegen
            AusgewählteAnrufe = ListVM.CallList.Where(Function(x) ImportStart <= x.Datum And x.Datum <= ImportEnde)

            ' Entferne die Exportmarkierung, bei allen Einträgen, die nicht im Bereich liegen
            For Each Anruf In ListVM.CallList.Except(AusgewählteAnrufe)
                Anruf.Export = False
            Next

            ' Füge die Exportmarkierung, bei allen Einträgen, die im Bereich liegen hinzu
            For Each Anruf In AusgewählteAnrufe
                Anruf.Export = True
            Next

        End If
    End Sub

#Region "ICommand Callback"
    ''' <summary>
    ''' Tritt auf, wenn das Element ausgerichtet und gerendert sowie zur Interaktion vorbereitet wurde.
    ''' </summary>
    Private Sub Init() Implements IPageListViewModel.Init

        ' Setze Startzeitpunkt = Zeitpunkt letzter Import
        StartDatum = DatenService.GetLastImport
        StartZeit = StartDatum.TimeOfDay

        ' Setze Endzeitpunkt = Jetzt
        EndDatum = Now.Date
        EndZeit = Now.TimeOfDay

    End Sub

    Private Sub CancelProcess(o As Object)
        CancelationPending = True
        NLogger.Debug("Manueller Journalimport abgebrochen.")
    End Sub

#Region "Journalimport"
    Private Sub JournalImport(o As Object)
        ' TODO CTS = New CancellationTokenSource analog TellowsViewmodel
        ' TODO Dim progressIndicator = New Progress(Of Integer)(Sub(status) BlockProgressValue += status)
        ' Abbruch Flag setzen
        CancelationPending = False

        Dim AusgewählteAnrufe As IEnumerable(Of FritzBoxXMLCall) = ListVM.CallList.Where(Function(x) x.Export = True)

        If AusgewählteAnrufe.Any Then
            ' Setze aktuellen Wert für Progressbar
            ImportProgressValue = 0
            ' Setze Progressbar Maximum
            ImportProgressMax = AusgewählteAnrufe.Count

            NLogger.Debug($"Starte manueller Import mit {ImportProgressMax} Einträgen.")

            Threading.Tasks.Task.Run(Sub()
                                         For Each Anruf In AusgewählteAnrufe
                                             If CancelationPending Then Exit For

                                             ' Erhöhe Wert für Progressbar
                                             ImportProgressValue += 1

                                             DatenService.ErstelleEintrag(Anruf)

                                         Next
                                     End Sub)

        End If

    End Sub

#End Region

#Region "Sperrlist"
    Private Sub BlockNumbers(o As Object)

        Dim BlockNumbers As IEnumerable(Of String) = From a In CType(o, IList).Cast(Of FritzBoxXMLCall)().ToList Select a.Gegenstelle

        If DialogService.ShowMessageBox(String.Format(Localize.LocAnrList.strQuestionBlockNumber, String.Join(", ", BlockNumbers))) = Windows.MessageBoxResult.Yes Then
            DatenService.BlockNumbers(BlockNumbers)
        End If

    End Sub
#End Region

#End Region

End Class
