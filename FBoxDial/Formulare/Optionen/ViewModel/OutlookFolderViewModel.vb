﻿Imports System.Windows.Input
Imports Microsoft.Office.Interop.Outlook

''' <summary>
''' In Anlehnung. Dirk Bahle
''' <code>https://www.codeproject.com/Articles/1224943/Advanced-WPF-TreeView-in-Csharp-VB-Net-Part-of-n</code>
''' </summary>
Public Class OutlookFolderViewModel
    Inherits NotifyBase
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Property OutlookItemType As OlItemType
    Friend Property Verwendung As OutlookOrdnerVerwendung

    Private _OptVM As OptionenViewModel
    Public Property OptVM As OptionenViewModel
        Get
            Return _OptVM
        End Get
        Set
            SetProperty(_OptVM, Value)

            ' Stoße das Setzen der Haken an
            SetUserFolder(True)
        End Set
    End Property

    Private _DatenGeladen As Boolean
    Public Property DatenGeladen As Boolean
        Get
            Return _DatenGeladen
        End Get
        Set
            SetProperty(_DatenGeladen, Value)
        End Set
    End Property

    Private ReadOnly _Stores As ObservableCollectionEx(Of OlFolderViewModel) = Nothing
    Public ReadOnly Property Stores As IEnumerable(Of OlFolderViewModel)
        Get
            Return _Stores
        End Get
    End Property

    Private _CheckItemCommand As ICommand
    ''' <summary>
    ''' Gets a command that checks all children And parent items
    ''' in a tree view in dependency of the check state of the
    ''' item in the command parameter.
    ''' </summary>
    Public ReadOnly Property CheckItemCommand As ICommand
        Get
            If _CheckItemCommand Is Nothing Then
                _CheckItemCommand = New RelayCommand(Of Object)(Sub(p)
                                                                    Dim param = TryCast(p, OlFolderViewModel)

                                                                    If param IsNot Nothing Then CheckItemCommand_Executed(param)
                                                                End Sub)
            End If

            Return _CheckItemCommand
        End Get
    End Property

    Public Sub New(OutlookStoreRootFolder As IEnumerable(Of MAPIFolder), OlItemType As OlItemType, Usage As OutlookOrdnerVerwendung)
        OutlookItemType = OlItemType
        Verwendung = Usage

        _Stores = New ObservableCollectionEx(Of OlFolderViewModel)

        ' Lade die Root-Folder jedes Outlook Stores Outlook
        _Stores.AddRange(From F In OutlookStoreRootFolder Select New OlFolderViewModel(F, OutlookItemType))
    End Sub

    ''' <summary>
    ''' Method executes when the corresponding command executes to re-evaluate
    ''' all items below And above a recently checked Or unchecked item.
    ''' </summary>
    Friend Sub CheckItemCommand_Executed(ChangedItem As OlFolderViewModel)

        AddFolder(ChangedItem)

        Dim TreeViewOutlookOrdner = TreeLib.BreadthFirst.Traverse.LevelOrder(ChangedItem.Folders, Function(i) i.ChildFolders)
        ' All children of the checked/unchecked item have to assume it's state
        If Verwendung = OutlookOrdnerVerwendung.KontaktSuche AndAlso OptVM?.CBSucheUnterordner Then
            For Each item In TreeViewOutlookOrdner
                TryCast(item.Node, OlFolderViewModel).IsChecked = ChangedItem.IsChecked
            Next
        End If

        ' Verarbeite die übergeordneten Elemente
        ' Wenn es keine nachfolgenden (Child) Elemente gibt
        ' Wenn keines der nachfolgenden (Child) Elemente True oder indeterminate ist
        If ChangedItem.ChildrenCount.IsZero OrElse ChangedItem.Folders.Where(Function(F) F.IsCheckedOrIndeterminate).Any Then
            ' Visit each parent in turn And determine their correct states

            Dim parentItem As IOlFolderViewModel = ChangedItem.Parent
            While parentItem IsNot Nothing
                ResetParentItemState(TryCast(parentItem, OlFolderViewModel))

                parentItem = parentItem.Parent
            End While
        End If

    End Sub

    ''' <summary>
    ''' Resets an item according to the states of its children. Call this method when
    ''' the given item needs to re-evaluate its state because one of its children
    ''' has just changed its state.
    ''' </summary>
    ''' <param name="Folder"></param>
    Private Sub ResetParentItemState(Folder As OlFolderViewModel)

        If Folder IsNot Nothing AndAlso Folder.IsNotCheckedOrIndeterminate AndAlso Folder.ChildrenCount.IsNotZero Then

            Dim itemChildren = Folder.Folders.ToArray()

            Dim firstChild As Boolean? = itemChildren(0).IsChecked

            ' Unterscheidung, nach Anzahl der Child-Elemente
            ' Wenn nur ein Element da ist, dann setze den Status auf Nothing (indeterminate)
            ' Wenn mehrere Elemente da sind, dann prüfe, ob es unterschiedliche Status gibt
            If itemChildren.Length.AreEqual(1) Then

                If firstChild Is Nothing Or firstChild Then

                    Folder.IsChecked = Nothing
                    NLogger.Debug($"Setze Checkbox für den Outlook Ordner {Folder.Name} ({Verwendung}) auf unbestimmt (indeterminate).")

                Else
                    Folder.IsChecked = False
                End If
            Else
                ' Fälle: 
                ' Alle Unterordner sind nicht selektiert: False
                ' Es gibt unterschiedliche Stati: indeterminate
                ' Alle Unterordner sind selektiert: indeterminate

                If itemChildren.Where(Function(F) F.IsCheckedFalse).Count.AreEqual(itemChildren.Count) Then
                    Folder.IsChecked = False

                    NLogger.Debug($"Setze Checkbox für den Outlook Ordner {Folder.Name} ({Verwendung}) auf false.")
                Else
                    Folder.IsChecked = Nothing

                    NLogger.Debug($"Setze Checkbox für den Outlook Ordner {Folder.Name} ({Verwendung}) auf unbestimmt (indeterminate).")
                End If

            End If
        End If
    End Sub

    ''' <summary>
    ''' Selektiert nach dem Start die durch den User gewählten Ordner.
    ''' </summary>
    Private Sub SetUserFolder(IsChecked As Boolean)

        If OptVM IsNot Nothing Then

            Dim TreeViewOutlookOrdner = TreeLib.BreadthFirst.Traverse.LevelOrder(Of IOlFolderViewModel)(Stores, Function(i) i.ChildFolders.Where(Function(oFolder) oFolder.OutlookFolder.DefaultItemType = OutlookItemType))

            NLogger.Debug($"Beginne das Setzen der Optionen im TreeView für '{Verwendung}' ({IsChecked}).")

            For Each Ordner As OutlookOrdner In OptVM.OutlookOrdnerListe.FindAll(Verwendung)
                ' Vorfilter: Ermittle alle Konten für den passenden Store
                Dim StoreNode = TreeViewOutlookOrdner.Where(Function(olStore) olStore.Node.OutlookFolder.StoreID.IsEqual(Ordner.StoreID))

                NLogger.Debug($"Verarbeite Ordner {Ordner.Name} für '{Verwendung}'.")

                ' Überprüfe, ob es den Ordner in Outlook gibt
                If Ordner.Exists Then

                    ' Suche den Ordner
                    Dim node = StoreNode.Where(Function(olFolderNode) olFolderNode.Node.OutlookFolder.EntryID.IsEqual(Ordner.FolderID))

                    If node?.Any Then
                        NLogger.Debug($"Knoten im TreeView gefunden.")
                        With node.First
                            ' Setze das Checkmark
                            .Node.IsChecked = IsChecked

                            ' Führe das Setzen aller benachbarter Knoten aus.
                            CheckItemCommand_Executed(CType(.Node, OlFolderViewModel))
                        End With

                    End If
                End If

            Next
            ' Setze das Flag, dass alle Daten geladen wurden
            DatenGeladen = True
        End If
    End Sub

    ''' <summary>
    ''' Hier erfolgt die Logik zur Setzung der Ordnermarkierungen.
    ''' <list type="bullet">
    ''' <item>Unterscheidung nach Verwendung (<see cref="OutlookOrdnerVerwendung"/>): Bei Kontaktsuche ist Mehrfachauswahl möglich, bei den anderen nicht.
    ''' </item>
    ''' </list>
    ''' </summary>
    ''' <param name="ChangedItem">Der veränderte Ordner als <see cref="OlFolderViewModel"/></param>
    Private Sub AddFolder(ChangedItem As OlFolderViewModel)
        If OptVM IsNot Nothing Then
            With ChangedItem

                Dim tmpfold As New OutlookOrdner(.OutlookFolder, Verwendung)

                ' Überprüfe, ob dieser Ordner noch verwendet werden soll
                If Not .IsChecked Then

                    ' Entferne den Ordner von der Liste
                    OptVM.OutlookOrdnerListe.Remove(tmpfold)
                    NLogger.Debug($"Ordner '{tmpfold.Name}' für die Verwendung '{Verwendung}' entfernt.")

                Else

                    ' Bei Kontaktsuche ist Mehrfachauswahl möglich, bei den anderen nicht.
                    ' Wenn die Verwendung KontaktErstellung oder Journalerstellung, dann entferne alle anderen gewählten Ordner
                    If Not Verwendung = OutlookOrdnerVerwendung.KontaktSuche Then
                        SetUserFolder(False)

                        ' Durch die Routine wird auch dieser Ordner auf False gesetzt.
                        ChangedItem.IsChecked = True
                    End If

                    ' Speichere den Ordner
                    OptVM.OutlookOrdnerListe.Add(tmpfold)
                    NLogger.Debug($"Ordner '{tmpfold.Name}' für die Verwendung '{Verwendung}' hinzugefügt.")

                End If

            End With

        End If

    End Sub

End Class

