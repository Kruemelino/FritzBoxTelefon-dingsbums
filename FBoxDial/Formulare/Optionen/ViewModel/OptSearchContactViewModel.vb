Imports System.Threading
Imports Microsoft.Office.Interop

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

    Public Property InitialSelected As Boolean = False Implements IPageViewModel.InitialSelected

    Private Property RootVM As OutlookFolderViewModel = New OutlookFolderViewModel(Outlook.OlItemType.olContactItem, OutlookOrdnerVerwendung.KontaktSuche)

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
    ' TODO in Datenservice verschieben
    Private Async Sub StartIndex(obj As Object)

        ' TODO CTS = New CancellationTokenSource analog TellowsViewmodel
        ' TODO Dim progressIndicator = New Progress(Of Integer)(Sub(status) BlockProgressValue += status)


        ' Setze das CancelationPending zurück
        CancelationPending = False

        ' Starte die Indizierung
        ReSetProgressbar()

        ' Aktiv-Flag setzen
        IsAktiv = True

        ' Für Ereignishandler hinzu
        AddHandler DatenService.IndexStatus, AddressOf SetProgressbar

        Dim OrdnerListe As List(Of OutlookOrdner) = OptVM.OutlookOrdnerListe.FindAll(OutlookOrdnerVerwendung.KontaktSuche)

        ' Überprüfe, ob Ordner für die Kontaktsuche ausgewählt sind
        If Not OrdnerListe.Any Then
            NLogger.Debug($"Es wurde kein Outlookordner für die Kontaktsuche gewählt. Füge Standardkontaktordner hinzu.")
            OrdnerListe.Add(New OutlookOrdner(GetDefaultMAPIFolder(Outlook.OlDefaultFolders.olFolderContacts), OutlookOrdnerVerwendung.KontaktSuche))
        End If

        Dim IndexTasks As New List(Of Tasks.Task)

        ' Erzeuge eine Liste der Ordner, die der Nutzer ausgewählt hat
        Dim MAPIFolderList As List(Of Outlook.MAPIFolder) = OrdnerListe.Select(Function(S) S.MAPIFolder).ToList

        ' Füge die Unterordner hinzu
        If OptVM.CBSucheUnterordner Then AddChildFolders(MAPIFolderList, Outlook.OlItemType.olContactItem)

        ' Verarbeite alle Ordner die der Kontaktsuche entsprechen
        For Each Ordner In MAPIFolderList
            ' Erhöhe das Maximum der Progressbar
            SetProgressbarMax(DatenService.ZähleOutlookKontakte(Ordner))

            ' Starte das Indizieren
            IndexTasks.Add(Tasks.Task.Run(Sub() DatenService.Indexer(Ordner, IndexModus, OptVM.CBSucheUnterordner)))

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
