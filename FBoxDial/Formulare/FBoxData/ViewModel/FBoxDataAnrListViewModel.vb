﻿Imports System.Collections
Imports System.Threading

Public Class FBoxDataAnrListViewModel
    Inherits NotifyBase
    Implements IFBoxData

    Public ReadOnly Property Name As String Implements IFBoxData.Name
        Get
            Return Localize.LocFBoxData.strAnrList
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

    Public Property InitialSelected As Boolean = True Implements IFBoxData.InitialSelected
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IFBoxDataService
    Private Property DialogService As IDialogService

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
    Private _CallList As ObservableCollectionEx(Of TR064.FritzBoxXMLCall)
    Public Property CallList As ObservableCollectionEx(Of TR064.FritzBoxXMLCall)
        Get
            Return _CallList
        End Get
        Set
            SetProperty(_CallList, Value)
        End Set
    End Property

    ''' <summary>
    ''' Returns Or sets a list as TellowsScoreListEntry             
    ''' </summary>
    Private _TellowsList As ObservableCollectionEx(Of TellowsScoreListEntry)
    Public Property TellowsList As ObservableCollectionEx(Of TellowsScoreListEntry)
        Get
            Return _TellowsList
        End Get
        Set
            SetProperty(_TellowsList, Value)
        End Set
    End Property
#End Region

#Region "ICommand"
    Public Property CancelCommand As RelayCommand
    Public Property ImportCommand As RelayCommand
    Public Property SelectAllCommand As RelayCommand
    Public Property BlockCommand As RelayCommand
    Public Property CallCommand As RelayCommand
    Public Property ShowContactCommand As RelayCommand
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
        CallCommand = New RelayCommand(AddressOf [Call], AddressOf CanCall)
        ShowContactCommand = New RelayCommand(AddressOf ShowContact, AddressOf CanShowContact)

    End Sub

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

            Dim AusgewählteAnrufe As IEnumerable(Of TR064.FritzBoxXMLCall)

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
    Private Async Sub Init() Implements IFBoxData.Init

        Dim TaskAnrList As TR064.FritzBoxXMLCallList = Await DatenService.GetAnrufListe

        ' Initiiere die Anrufliste
        CallList = New ObservableCollectionEx(Of TR064.FritzBoxXMLCall)
        ' Lade die Anrufliste
        CallList.AddRange(TaskAnrList?.Calls)

        ' Setze Startzeitpunkt = Zeitpunkt letzter Import
        StartDatum = DatenService.GetLastImport
        StartZeit = StartDatum.TimeOfDay

        ' Setze Endzeitpunkt = Jetzt
        EndDatum = Now.Date
        EndZeit = Now.TimeOfDay

    End Sub

    Private Sub CancelProcess(o As Object)
        CTS?.Cancel()
        NLogger.Debug("Manueller Journalimport abgebrochen.")
    End Sub

#End Region

#Region "Cancel"
    Private Property CTS As CancellationTokenSource
#End Region

#Region "Journalimport"
    Private Async Sub JournalImport(o As Object)

        Dim AusgewählteAnrufe As IEnumerable(Of TR064.FritzBoxXMLCall) = CallList.Where(Function(x) x.Export = True)

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

        Dim BlockNumbers As IEnumerable(Of String) = From a In CType(o, IList).Cast(Of TR064.FritzBoxXMLCall)().ToList Select a.Gegenstelle

        If DialogService.ShowMessageBox(String.Format(Localize.LocFBoxData.strQuestionBlockNumber, String.Join(", ", BlockNumbers))) = Windows.MessageBoxResult.Yes Then
            DatenService.BlockNumbers(BlockNumbers)
        End If

    End Sub
#End Region

#Region "Kontakt Anrufen"
    Private Sub [Call](o As Object)
        Dim XMLKontakt As TR064.FritzBoxXMLCall = (From a In CType(o, IList).Cast(Of TR064.FritzBoxXMLCall)()).ToList.First
        DatenService.CallXMLContact(XMLKontakt)
    End Sub

    Private Function CanCall(o As Object) As Boolean
        If o IsNot Nothing Then
            Dim XMLKontaktListe As IEnumerable(Of TR064.FritzBoxXMLCall) = From a In CType(o, IList).Cast(Of TR064.FritzBoxXMLCall)().ToList

            Return XMLKontaktListe.Count.AreEqual(1) AndAlso XMLKontaktListe.First.Gegenstelle.IsNotStringNothingOrEmpty
        Else
            Return False
        End If
    End Function
#End Region

#Region "Kontakt Anzeigen"
    Private Sub ShowContact(o As Object)
        Dim XMLKontaktListe As IEnumerable(Of TR064.FritzBoxXMLCall) = From a In CType(o, IList).Cast(Of TR064.FritzBoxXMLCall)().ToList

        For Each XMLKontakt In XMLKontaktListe
            DatenService.ShowXMLContact(XMLKontakt)
        Next
    End Sub

    Private Function CanShowContact(o As Object) As Boolean
        If o IsNot Nothing Then
            Dim XMLKontaktListe As IEnumerable(Of TR064.FritzBoxXMLCall) = From a In CType(o, IList).Cast(Of TR064.FritzBoxXMLCall)().ToList

            Return XMLKontaktListe.First.Gegenstelle.IsNotStringNothingOrEmpty
        Else
            Return False
        End If
    End Function

#End Region
End Class
