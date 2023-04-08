Imports System.Threading
Imports Microsoft.Office.Interop.Outlook

Public Class OptSyncSetupViewModel
    Inherits NotifyBase

    Private Property DatenService As IOptionenService
    Private Property DialogService As IDialogService
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Public ReadOnly Property ParentViewModel As OptContactSyncViewModel

    Public Property ContactSyncRemoveCommand As RelayCommand
    Public Property StartSyncCommand As RelayCommand
    Public Property CancelSyncCommand As RelayCommand
    Public Sub New(dataService As IOptionenService, dialogService As IDialogService, Parent As OptContactSyncViewModel)
        ' Interface
        _DatenService = dataService
        _DialogService = dialogService

        _ParentViewModel = Parent

        ' Commands
        ContactSyncRemoveCommand = New RelayCommand(AddressOf RemoveSyncSetup)
        StartSyncCommand = New RelayCommand(AddressOf StartSync, AddressOf CanStartSync)
        CancelSyncCommand = New RelayCommand(AddressOf CancelSync, AddressOf CanCancelSync)

    End Sub

#Region "Properties"
    Private _OlOrdner As OutlookOrdner
    Public Property OlOrdner As OutlookOrdner
        Get
            Return _OlOrdner
        End Get
        Set
            ' den alten Ordner entfernen
            ParentViewModel.OptVM.OutlookOrdnerListe.Remove(_OlOrdner)

            SetProperty(_OlOrdner, Value)

            If _OlOrdner IsNot Nothing Then

                With _OlOrdner
                    ' Initialisiere die Synchronisationsoptionen, falls nötig
                    If .FBoxSyncOptions Is Nothing Then .FBoxSyncOptions = New SyncOptions

                    ' Setze das ausgewählte Fritz!Box Telefonbuch 
                    If FBoxTelefonbuch IsNot Nothing Then .FBoxSyncOptions.FBoxSyncID = _FBoxTelefonbuch.ID

                    ' Setze den ausgewählten Synchronisationsmodus
                    If Not _Modus = 0 Then .FBoxSyncOptions.FBoxSyncMode = _Modus

                    ' Setze die ausgewählte Einstellung zu den Unterordnern
                    .FBoxSyncOptions.FBoxCBSyncStartUp = _CBSyncStartUp

                End With

                ParentViewModel.OptVM.OutlookOrdnerListe.Add(_OlOrdner)

            End If

        End Set
    End Property

    Private _CBSyncStartUp As Boolean
    Public Property CBSyncStartUp As Boolean
        Get
            Return _CBSyncStartUp
        End Get
        Set
            SetProperty(_CBSyncStartUp, Value)

            If _OlOrdner IsNot Nothing Then
                With _OlOrdner
                    ' Initialisiere die Synchronisationsoptionen, falls nötig
                    If .FBoxSyncOptions Is Nothing Then .FBoxSyncOptions = New SyncOptions

                    ' Setze die ausgewählte Einstellung zu den Unterordnern
                    .FBoxSyncOptions.FBoxCBSyncStartUp = Value
                End With

            End If
        End Set
    End Property

    Private _FBoxTelefonbuch As PhonebookEx
    Public Property FBoxTelefonbuch As PhonebookEx
        Get
            Return _FBoxTelefonbuch
        End Get
        Set
            SetProperty(_FBoxTelefonbuch, Value)

            If Not (_FBoxTelefonbuch Is Nothing Or _OlOrdner Is Nothing) Then
                With _OlOrdner
                    ' Initialisiere die Synchronisationsoptionen, falls nötig
                    If .FBoxSyncOptions Is Nothing Then .FBoxSyncOptions = New SyncOptions

                    ' Setze das ausgewählte Fritz!Box Telefonbuch 
                    .FBoxSyncOptions.FBoxSyncID = _FBoxTelefonbuch.ID
                End With

            End If

        End Set
    End Property

    Private _Modus As SyncMode
    Public Property Modus As SyncMode
        Get
            Return _Modus
        End Get
        Set
            SetProperty(_Modus, Value)

            If _OlOrdner IsNot Nothing Then
                With _OlOrdner
                    ' Initialisiere die Synchronisationsoptionen, falls nötig
                    If .FBoxSyncOptions Is Nothing Then .FBoxSyncOptions = New SyncOptions

                    ' Setze den ausgewählten Synchronisationsmodus
                    .FBoxSyncOptions.FBoxSyncMode = _Modus

                End With
            End If
        End Set
    End Property

    Private _SyncProgressValue As Double = 0
    Public Property SyncProgressValue As Double
        Get
            Return _SyncProgressValue
        End Get
        Set
            SetProperty(_SyncProgressValue, Value)
        End Set
    End Property

    Private _SyncProgressMax As Double = 100
    Public Property SyncProgressMax As Double
        Get
            Return _SyncProgressMax
        End Get
        Set
            SetProperty(_SyncProgressMax, Value)
        End Set
    End Property

    Private _SyncStatus As String
    Public Property SyncStatus As String
        Get
            Return _SyncStatus
        End Get
        Set
            SetProperty(_SyncStatus, Value)
        End Set
    End Property

    Private _ExSyncStatus As String
    Public Property ExSyncStatus As String
        Get
            Return _ExSyncStatus
        End Get
        Set
            SetProperty(_ExSyncStatus, Value)
        End Set
    End Property

    Private _IsAktiv As Boolean = False
    Public Property IsAktiv As Boolean
        Get
            Return _IsAktiv
        End Get
        Set
            SetProperty(_IsAktiv, Value)
        End Set
    End Property
#End Region

#Region "Cancel"
    Private Property CTS As CancellationTokenSource
#End Region

    Private Function CanCancelSync(o As Object) As Boolean
        Return IsAktiv
    End Function

    Private Sub CancelSync(obj As Object)
        CTS?.Cancel()
        NLogger.Debug("Kontaktsynchronisation abgebrochen.")
        IsAktiv = False
    End Sub

    Private Async Sub StartSync(o As Object)
        CTS = New CancellationTokenSource
        Dim progressIndicator = New Progress(Of String)(Sub(status)
                                                            SyncProgressValue += 1
                                                            SyncStatus = $"{Localize.LocOptionen.strIndexStatus}: {SyncProgressValue}/{SyncProgressMax}"
                                                            ExSyncStatus += status & Environment.NewLine
                                                            NLogger.Info(status)
                                                        End Sub)
        ' Aktiv-Flag setzen
        IsAktiv = True

        Dim OrdnerListe As New List(Of MAPIFolder) From {OlOrdner.MAPIFolder}

        ' Füge die Unterordner hinzu
        ' If CBSyncUnterordner Then AddChildFolders(OrdnerListe, OlItemType.olContactItem)

        ' Setze Progressbar Maximum
        SyncProgressMax = DatenService.ZähleOutlookKontakte(OrdnerListe)

        ' Starte die Synchronisation
        SyncProgressValue = 0
        SyncStatus = $"{Localize.LocOptionen.strIndexStatus}: {SyncProgressValue}/{SyncProgressMax}"

        Try
            ' Start der Synchronisation
            NLogger.Debug($"Synchronisation von {Await DatenService.Synchronisierer(OrdnerListe, FBoxTelefonbuch, Modus, CTS.Token, progressIndicator)} Kontakten beendet.")
        Catch ex As OperationCanceledException
            NLogger.Debug(ex)
        End Try

        ExSyncStatus += "Synchronisation abgeschlossen."

        ' Aktiv-Flag setzen
        IsAktiv = False

        ' CancellationTokenSource auflösen
        CTS.Dispose()

    End Sub
    Private Function CanStartSync(obj As Object) As Boolean
        Return Not (FBoxTelefonbuch Is Nothing Or OlOrdner Is Nothing Or Modus = 0)
    End Function

    Private Sub RemoveSyncSetup(o As Object)
        ParentViewModel.RemoveSyncSetup(Me)
    End Sub
End Class
