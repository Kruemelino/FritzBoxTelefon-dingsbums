Imports System.Collections

Public Class AnrListViewModel
    Inherits NotifyBase
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IAnrListService
    Private Property DialogService As IDialogService
#Region "Felder"

    ''' <summary>
    ''' Returns Or sets a list as FritzBoxXMLCall             
    ''' </summary>
    Private _CallList As ObservableCollectionEx(Of FritzBoxXMLCall)
    Public Property CallList As ObservableCollectionEx(Of FritzBoxXMLCall)
        Get
            Return _CallList
        End Get
        Set
            SetProperty(_CallList, Value)
        End Set
    End Property

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
    Public Property LoadedCommand As RelayCommand
    Public Property SelectAllCommand As RelayCommand
    Public Property BlockCommand As RelayCommand
#End Region

    Public Sub New()
        ' Commands
        CancelCommand = New RelayCommand(AddressOf CancelImport)
        ImportCommand = New RelayCommand(AddressOf Import)
        SelectAllCommand = New RelayCommand(AddressOf SelectAll)
        BlockCommand = New RelayCommand(AddressOf BlockNumbers)
        ' Window Command
        LoadedCommand = New RelayCommand(AddressOf Loaded)

        ' Interface
        DatenService = New AnrListService
        DialogService = New DialogService

    End Sub



    Private Sub SelectAll(obj As Object)

        For Each Anruf In CallList
            Anruf.Export = CBool(obj)
        Next

    End Sub

    Private Sub SelectItems()

        If CallList IsNot Nothing AndAlso CallList.Any Then

            ' Ausgewählten Zeitraum ermitteln
            ' Startpunkt
            Dim ImportStart As Date = StartDatum.Add(StartZeit)

            ' Endzeitpunkt
            Dim ImportEnde As Date = EndDatum.Add(EndZeit)

            Dim AusgewählteAnrufe As IEnumerable(Of FritzBoxXMLCall)

            ' Ermittle alle Einträge, die im ausgewählten Bereich liegen
            AusgewählteAnrufe = CallList.Where(Function(x) ImportStart <= x.Datum And x.Datum <= ImportEnde)

            ' Entferne die Exportmarkierung, bei allen Einträgen, die nicht im Bereich liegen
            For Each Anruf In CallList.Except(AusgewählteAnrufe)
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
    Private Async Sub Loaded(obj As Object)

        ' Setze Startzeitpunkt = Zeitpunkt letzter Import
        StartDatum = DatenService.GetLastImport
        StartZeit = StartDatum.TimeOfDay

        ' Setze Endzeitpunkt = Jetzt
        EndDatum = Now.Date
        EndZeit = Now.TimeOfDay

        ' Initiiere die Anrufliste
        CallList = New ObservableCollectionEx(Of FritzBoxXMLCall)
        ' Lade die Anrufliste
        CallList.AddRange((Await DatenService.GetAnrufListe)?.Calls)

        ' Selektiere Alle Anrufe im Startzeitraum 
        SelectItems()
    End Sub

    Private Sub CancelImport(o As Object)
        CancelationPending = True
        NLogger.Debug("Manueller Journalimport abgebrochen.")
    End Sub

    Private Sub Import(o As Object)

        ' Abbruch Flag setzen
        CancelationPending = False

        Dim AusgewählteAnrufe As IEnumerable(Of FritzBoxXMLCall) = CallList.Where(Function(x) x.Export = True)

        If AusgewählteAnrufe.Any Then
            ' Setze aktuellen Wert für Progressbar
            ImportProgressValue = 0
            ' Setze Progressbar Maximum
            ImportProgressMax = AusgewählteAnrufe.Count

            NLogger.Debug($"Starte manueller Journalimport mit {ImportProgressMax} Einträgen.")

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

    Private Sub BlockNumbers(o As Object)

        Dim BlockNumbers As IEnumerable(Of String) = From a In CType(o, IList).Cast(Of FritzBoxXMLCall)().ToList Select a.Gegenstelle

        String.Join(", ", BlockNumbers)

        If DialogService.ShowMessageBox(String.Format(Localize.LocAnrList.strQuestionBlockNumber, String.Join(", ", BlockNumbers))) = Windows.MessageBoxResult.Yes Then
            DatenService.BlockNumbers(BlockNumbers)
        End If

    End Sub
#End Region

End Class
