Imports System.Threading
Imports Microsoft.Office.Interop.Outlook

Public Class OptSearchContactViewModel
    Inherits NotifyBase
    Implements IPageViewModel

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IOptionenService
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

    Private Property RootVM As OutlookFolderViewModel = New OutlookFolderViewModel(OlItemType.olContactItem, OutlookOrdnerVerwendung.KontaktSuche)

    Public ReadOnly Property Root As OutlookFolderViewModel
        Get
            RootVM.OptVM = OptVM
            Return RootVM
        End Get
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

#End Region

#Region "ICommand"
    Public Property CancelCommand As RelayCommand
    Public Property IndexCommand As RelayCommand
#End Region

    Public Sub New()
        ' Commands
        CancelCommand = New RelayCommand(AddressOf CancelImport)
        IndexCommand = New RelayCommand(AddressOf StartIndex)

        ' Interface
        DatenService = New OptionenService
    End Sub

#Region "ICommand Callback"
    Private Sub CancelImport(o As Object)

        CancelationPending = True
        DatenService.CancelationPending = True
        NLogger.Debug("Manueller Journalimport abgebrochen.")

    End Sub
    Private Async Sub StartIndex(obj As Object)
        ' Setze das CancelationPending zurück
        CancelationPending = False

        ' Starte die Indizierung
        ReSetProgressbar()

        ' Aktiv-Flag setzen
        IsAktiv = True

        ' Für Ereignishandler hinzu
        AddHandler DatenService.IndexStatus, AddressOf SetProgressbar

        Dim OutlookOrdner As OutlookOrdnerListe = OptVM.OutlookOrdnerListe

        ' Überprüfe, ob Ordner für die Kontaktsuche ausgewählt sind
        If Not OutlookOrdner.FindAll(OutlookOrdnerVerwendung.KontaktSuche).Any Then
            OutlookOrdner.Add(New OutlookOrdner(OutlookOrdner.GetDefaultMAPIFolder(OlDefaultFolders.olFolderContacts), OutlookOrdnerVerwendung.KontaktSuche))

            NLogger.Debug($"Es wurde kein Outlookordner für die Kontaktsuche gewählt. Füge Standardkontaktordner hinzu.")
        End If

        Dim IndexTasks As New List(Of Tasks.Task)

        ' Verarbeite alle Ordner die der Kontaktsuche entsprechen
        For Each Ordner In OutlookOrdner.FindAll(OutlookOrdnerVerwendung.KontaktSuche)
            ' Erhöhe das Maximum der Progressbar
            SetProgressbarMax(DatenService.ZähleOutlookKontakte(Ordner.MAPIFolder))

            ' Starte das Indizieren
            IndexTasks.Add(Tasks.Task.Run(Sub() DatenService.Indexer(Ordner.MAPIFolder, IndexModus, OptVM.CBSucheUnterordner)))

            ' Frage Cancelation ab
            If CancelationPending Then Exit For
        Next

        ' Warte den Abschluss der Indizierung ab
        Await Tasks.Task.WhenAll(IndexTasks)

        ' Entferne Ereignishandler 
        RemoveHandler DatenService.IndexStatus, AddressOf SetProgressbar

        ' Aktiv-Flag setzen
        IsAktiv = False
    End Sub

#End Region

#Region "Hilfsfunktionen"
    Private Sub ReSetProgressbar()
        IndexProgressValue = 0
        IndexProgressMax = 0
        IndexStatus = $"{Localize.LocOptionen.strIndexStatus}: {IndexProgressValue}/{IndexProgressMax}"
    End Sub

    Private Sub SetProgressbar(sender As Object, e As NotifyEventArgs(Of Integer))
        IndexProgressValue += e.Value
        IndexStatus = $"{Localize.LocOptionen.strIndexStatus}: {IndexProgressValue}/{IndexProgressMax}"
    End Sub

    Private Sub SetProgressbarMax(NeuesMaximum As Integer)
        IndexProgressMax += NeuesMaximum
    End Sub

#End Region
End Class
