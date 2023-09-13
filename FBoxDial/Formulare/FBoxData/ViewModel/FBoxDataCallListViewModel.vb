Imports System.Collections
Imports System.Threading

Public Class FBoxDataCallListViewModel
    Inherits NotifyBase
    Implements IFBoxData
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Public ReadOnly Property Name As String Implements IFBoxData.Name
        Get
            Return Localize.LocFBoxData.strAnrList
        End Get
    End Property

    Private Property DebugBeginnLadeDaten As Date Implements IFBoxData.DebugBeginnLadeDaten

    Private _FBoxDataVM As FBoxDataViewModel
    Public Property FBoxDataVM As FBoxDataViewModel Implements IFBoxData.FBoxDataVM
        Get
            Return _FBoxDataVM
        End Get
        Set
            SetProperty(_FBoxDataVM, Value)
        End Set
    End Property

    Public Property InitialSelected As Boolean = True Implements IFBoxData.InitialSelected
    Private Property DatenService As IFBoxDataService
    Private Property DialogService As IDialogService

#Region "ICommand"
    Public Property CancelCommand As RelayCommand
    Public Property ImportCommand As RelayCommand
    Public Property SelectAllCommand As RelayCommand
    Public Property BlockCommand As RelayCommand
    Public Property CallCommand As RelayCommand
    Public Property ShowContactCommand As RelayCommand
    Public Property AppointmentCommand As RelayCommand
#End Region

#Region "Properties"

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

    Private _IsAktiv As Boolean = False

    Public Property IsAktiv As Boolean
        Get
            Return _IsAktiv
        End Get
        Set
            SetProperty(_IsAktiv, Value)
            OnPropertyChanged(NameOf(IsNotAktiv))
        End Set
    End Property

    Public ReadOnly Property IsNotAktiv As Boolean
        Get
            Return Not _IsAktiv
        End Get
    End Property
#End Region

#Region "Listen"
    ''' <summary>
    ''' Returns Or sets a list as FritzBoxXMLCall             
    ''' </summary>
    Public Property CallList As New ObservableCollectionEx(Of CallViewModel)
#End Region

    Public Sub New(dataService As IFBoxDataService, dialogService As IDialogService)
        ' Interface
        _DatenService = dataService
        _DialogService = dialogService

        ' Commands
        CancelCommand = New RelayCommand(AddressOf CancelProcess)
        ImportCommand = New RelayCommand(AddressOf JournalImport)
        SelectAllCommand = New RelayCommand(AddressOf SelectAll)
        BlockCommand = New RelayCommand(AddressOf BlockNumbers)
        CallCommand = New RelayCommand(AddressOf Dial, AddressOf CanDial)
        ShowContactCommand = New RelayCommand(AddressOf ShowContact, AddressOf CanShowContact)
        AppointmentCommand = New RelayCommand(AddressOf AddAppointment)
    End Sub

    Private Async Sub Init() Implements IFBoxData.Init

        ' Dummyeintrag. Ansonsten wird das FilteredDataGrid nicht ordentlich geladen
        CallList.Add(New CallViewModel(DatenService) With {.CallItem = New FBoxAPI.Call With {.Name = Localize.LocFBoxData.strDataError,
                                                                                              .[Date] = Now.ToString("g"),
                                                                                              .Duration = "00:00"}})

        ' Setze Startzeitpunkt = Zeitpunkt letzter Import
        StartDatum = DatenService.GetLastImport
        StartZeit = StartDatum.TimeOfDay

        ' Setze Endzeitpunkt = Jetzt
        EndDatum = Now.Date
        EndZeit = Now.TimeOfDay

        With Await DatenService.GetCallList
            If .Calls.Any Then
                CallList.Clear()
                CallList.AddRange(From CallItem In .Calls Select New CallViewModel(DatenService) With {.CallItem = CallItem})
            End If
            ' Debugmeldung
            NLogger.Debug($"Ende: Lade Daten für {Name} in {(Date.Now - DebugBeginnLadeDaten).TotalSeconds} Sekunden")
        End With
    End Sub

#Region "ICommand Callback"

#Region "SelectAll"
    Private Sub SelectAll(o As Object)
        For Each Anruf In CallList
            Anruf.Export = CBool(o)
        Next
    End Sub

    Private Sub SelectItems()

        If CallList IsNot Nothing AndAlso CallList.Any Then

            ' Ausgewählten Zeitraum ermitteln
            ' Startpunkt
            Dim ImportStart As Date = StartDatum.Add(StartZeit)

            ' Endzeitpunkt
            Dim ImportEnde As Date = EndDatum.Add(EndZeit)

            Dim AusgewählteAnrufe As IEnumerable(Of CallViewModel)

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
#End Region

#Region "Cancel"
    Private Property CTS As CancellationTokenSource
    Private Sub CancelProcess(o As Object)
        CTS?.Cancel()
        NLogger.Debug("Manueller Journalimport abgebrochen.")
    End Sub
#End Region

#Region "Journalimport"
    Private Async Sub JournalImport(o As Object)

        Dim AusgewählteAnrufe As IEnumerable(Of FBoxAPI.Call) = CallList.Where(Function(x) x.Export = True).Select(Function(Anruf) Anruf.CallItem)

        If AusgewählteAnrufe.Any Then

            ' Aktiv-Flag setzen
            IsAktiv = True

            ' Setze aktuellen Wert für Progressbar
            ImportProgressValue = 0

            ' Setze Progressbar Maximum
            ImportProgressMax = AusgewählteAnrufe.Count

            NLogger.Debug($"Starte manueller Import mit {ImportProgressMax} Einträgen.")

            CTS = New CancellationTokenSource

            Dim progressIndicator = New Progress(Of Integer)(Sub(status) ImportProgressValue += status)

            Try
                ' Erstellung der Sperrliste in der Fritz!Box anstoßen
                Await DatenService.ErstelleEinträge(AusgewählteAnrufe, CTS.Token, progressIndicator)

            Catch ex As OperationCanceledException
                NLogger.Debug(ex)
            End Try

            If Not CTS.Token.IsCancellationRequested Then
                ' Progressbar auf Max setzen:
                ImportProgressValue = ImportProgressMax
            End If

            ' Aktiv-Flag setzen
            IsAktiv = False

            ' CancellationTokenSource auflösen
            CTS.Dispose()
        End If

    End Sub

#End Region

#Region "Sperrlist"
    Private Sub BlockNumbers(o As Object)

        Dim BlockNumbers As IEnumerable(Of String) = From a In CType(o, IList).Cast(Of CallViewModel)().ToList Select a.Gegenstelle

        If DialogService.ShowMessageBox(String.Format(Localize.LocFBoxData.strQuestionBlockNumber, String.Join(", ", BlockNumbers))) = Windows.MessageBoxResult.Yes Then
            DatenService.BlockNumbers(BlockNumbers)
        End If

    End Sub
#End Region

#Region "Kontakt Anrufen"
    Private Sub Dial(o As Object)
        DatenService.CallXMLContact((From a In CType(o, IList).Cast(Of CallViewModel)()).ToList.First.CallItem)
    End Sub

    Private Function CanDial(o As Object) As Boolean
        If o IsNot Nothing Then
            Dim XMLKontaktListe As IEnumerable(Of CallViewModel) = From a In CType(o, IList).Cast(Of CallViewModel)().ToList

            Return XMLKontaktListe.Count.AreEqual(1) AndAlso XMLKontaktListe.First.Gegenstelle.IsNotStringNothingOrEmpty
        Else
            Return False
        End If
    End Function
#End Region

#Region "Kontakt Anzeigen"
    Private Sub ShowContact(o As Object)
        Dim AnrufListeListe As IEnumerable(Of CallViewModel) = From a In CType(o, IList).Cast(Of CallViewModel)().ToList

        For Each Anruf In AnrufListeListe
            DatenService.ShowXMLContact(Anruf.CallItem)
        Next
    End Sub

    Private Function CanShowContact(o As Object) As Boolean
        If o IsNot Nothing Then
            Dim AnrufListeListe As IEnumerable(Of CallViewModel) = From a In CType(o, IList).Cast(Of CallViewModel)().ToList
            Return AnrufListeListe.Any AndAlso AnrufListeListe.First.Gegenstelle.IsNotStringNothingOrEmpty
        Else
            Return False
        End If
    End Function
#End Region

#Region "Anruftermin"
    Private Sub AddAppointment(o As Object)
        Dim AnrufListeListe As IEnumerable(Of CallViewModel) = From a In CType(o, IList).Cast(Of CallViewModel)().ToList

        For Each Anruf In AnrufListeListe
            DatenService.SetAppointment(Anruf.CallItem)
        Next
    End Sub
#End Region
#End Region
End Class
