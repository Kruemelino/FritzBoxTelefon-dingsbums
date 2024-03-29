﻿Imports System.Threading
Imports Microsoft.Office.Interop.Outlook

Public Class OptSearchContactViewModel
    Inherits NotifyBase
    Implements IPageViewModel

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IOptionenService

    Private Const Verwendung As OutlookOrdnerVerwendung = OutlookOrdnerVerwendung.KontaktSuche
    Private Const ItemType As OlItemType = OlItemType.olContactItem

#Region "Felder"

    Private _OptVM As OptionenViewModel
    Public Property OptVM As OptionenViewModel Implements IPageViewModel.OptVM
        Get
            Return _OptVM
        End Get
        Set
            SetProperty(_OptVM, Value)
        End Set
    End Property

    Public ReadOnly Property Name As String Implements IPageViewModel.Name
        Get
            Return Localize.LocOptionen.strSearchContact
        End Get
    End Property

    Public Property InitialSelected As Boolean = False Implements IPageViewModel.InitialSelected

    Private Property RootVM As OutlookFolderViewModel

    Public ReadOnly Property Root As OutlookFolderViewModel
        Get
            If RootVM Is Nothing Then RootVM = New OutlookFolderViewModel(DatenService.GetOutlookStoreRootFolder, ItemType, Verwendung)

            RootVM.OptVM = OptVM
            Return RootVM
        End Get
    End Property

    Private _IndexProgressValue As Double
    Public Property IndexProgressValue As Double
        Get
            Return _IndexProgressValue
        End Get
        Set
            SetProperty(_IndexProgressValue, Value)
        End Set
    End Property

    Private _IndexProgressMax As Double
    Public Property IndexProgressMax As Double
        Get
            Return _IndexProgressMax
        End Get
        Set
            SetProperty(_IndexProgressMax, Value)
        End Set
    End Property

    Private _IndexModus As Boolean = True
    Public Property IndexModus As Boolean
        Get
            Return _IndexModus
        End Get
        Set
            SetProperty(_IndexModus, Value)
        End Set
    End Property

    Private _IndexStatus As String
    Public Property IndexStatus As String
        Get
            Return _IndexStatus
        End Get
        Set
            SetProperty(_IndexStatus, Value)
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

    Private _ExIndexStatus As String
    Public Property ExIndexStatus As String
        Get
            Return _ExIndexStatus
        End Get
        Set
            SetProperty(_ExIndexStatus, Value)
        End Set
    End Property

#End Region

#Region "ICommand"
    Public Property CancelCommand As RelayCommand
    Public Property IndexCommand As RelayCommand
#End Region

#Region "Cancel"
    Private Property CTS As CancellationTokenSource
#End Region

    Public Sub New(ds As IOptionenService)
        ' Commands
        CancelCommand = New RelayCommand(AddressOf CancelImport)
        IndexCommand = New RelayCommand(AddressOf StartIndex)

        ' Interface
        _DatenService = ds
    End Sub

#Region "ICommand Callback"
    Private Sub CancelImport(o As Object)

        CTS?.Cancel()
        NLogger.Debug("Manueller Journalimport abgebrochen.")

    End Sub

    Private Async Sub StartIndex(o As Object)

        CTS = New CancellationTokenSource
        Dim progressIndicator = New Progress(Of String)(Sub(status)
                                                            IndexProgressValue += 1
                                                            IndexStatus = $"{Localize.LocOptionen.strIndexStatus}: {IndexProgressValue}/{IndexProgressMax}"
                                                            ExIndexStatus += status & Environment.NewLine
                                                        End Sub)
        ' Aktiv-Flag setzen
        IsAktiv = True

        Dim OrdnerListe As List(Of MAPIFolder) = OptVM.OutlookOrdnerListe.FindAll(OutlookOrdnerVerwendung.KontaktSuche).Select(Function(S) S.MAPIFolder).ToList

        ' Überprüfe, ob Ordner für die Kontaktsuche ausgewählt sind
        If Not OrdnerListe.Any Then
            NLogger.Info("Es wurde kein Outlookordner für die Kontaktsuche gewählt. Füge Standardkontaktordner hinzu.")
            OrdnerListe.Add(New OutlookOrdner(GetDefaultMAPIFolder(OlDefaultFolders.olFolderContacts), OutlookOrdnerVerwendung.KontaktSuche).MAPIFolder)
        End If

        ' Füge die Unterordner hinzu
        If OptVM.CBSucheUnterordner Then AddOutlookChildFolders(OrdnerListe, OlItemType.olContactItem)

        ' Setze Progressbar Maximum
        IndexProgressMax = DatenService.ZähleOutlookKontakte(OrdnerListe)

        NLogger.Debug($"Manuelle {If(IndexModus, "Indizierung", "Deindizierung")} von {IndexProgressMax} Kontakten in {OrdnerListe.Count} Ordnern gestartet.")

        ' Starte die Indizierung
        IndexProgressValue = 0
        IndexStatus = $"{Localize.LocOptionen.strIndexStatus}: {IndexProgressValue}/{IndexProgressMax}"

        Try
            ' Start der Indizierung
            NLogger.Debug($"Manuelle {If(IndexModus, "Indizierung", "Deindizierung")} von {Await DatenService.Indexer(OrdnerListe, IndexModus, CTS.Token, progressIndicator)} Kontakten in {OrdnerListe.Count} Ordnern beendet.")
        Catch ex As OperationCanceledException
            NLogger.Debug(ex)
        End Try

        ExIndexStatus += $"{If(IndexModus, "Indizierung", "Deindizierung")} abgeschlossen."

        ' Aktiv-Flag setzen
        IsAktiv = False

        ' CancellationTokenSource auflösen
        CTS.Dispose()
    End Sub

#End Region

End Class
