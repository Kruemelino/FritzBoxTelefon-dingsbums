﻿Imports Microsoft.Office.Interop.Outlook
Imports Microsoft.Office.Tools
Imports System.Windows.Forms.Integration
Imports System.Windows.Forms

Friend Class ExplorerWrapper
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property OlExplorer As Explorer
    Private Property AktuellerOrdner As MAPIFolder
    Private Property CallListPane As CustomTaskPane
    Private Property CallListPaneVM As CallListPaneViewModel
    Private Property Datenservice As IAnrMonService
    Private Property Dialogservice As IDialogService
    Private Property PaneDispatcher As Windows.Threading.Dispatcher
    Private Property OlSelectedItems As List(Of OutlookItemWrapper)
    Private Property OlPastedItem As OutlookItemWrapper
    Friend ReadOnly Property PaneVisible As Boolean
        Get
            Return CallListPane IsNot Nothing AndAlso CallListPane.Visible
        End Get
    End Property
    Friend ReadOnly Property PaneItemsAny As Boolean
        Get
            Return CallListPaneVM IsNot Nothing AndAlso CallListPaneVM.MissedCallList.Any
        End Get
    End Property

    Friend Sub New(e As Explorer)
        OlExplorer = e

        OlSelectedItems = New List(Of OutlookItemWrapper)

        If OlExplorer IsNot Nothing Then
            NLogger.Debug("Ein neues Explorer-Fenster wird geöffnet")
            ' Füge drei Ereignishandler des Explorers hinzu
            AddHandler OlExplorer.Close, AddressOf Explorer_Close
            AddHandler OlExplorer.BeforeItemPaste, AddressOf OutlookExplorer_BeforeItemPaste
            AddHandler OlExplorer.SelectionChange, AddressOf Explorer_SelectionChange

            ' CallListPane 
            If XMLData.POptionen.CBShowMissedCallPane Then

                Datenservice = New AnrMonService
                Dialogservice = New DialogService

                If Not GetExistingCallListPane() Then

                    Dim UC As New UserControl 'With {.BackColor = GetOfficeBackGroundColor()}
                    ' Dark: 0a0a0a
                    ' Light: f0f0f0
                    ' DarkGray: 2e2e2e

                    CallListPaneVM = New CallListPaneViewModel(Datenservice)

                    ' Erstelle ein neues WPF Pane
                    Dim WPFChild As New CallListPaneView With {.DataContext = CallListPaneVM}

                    ' Merke den Dispatcher
                    PaneDispatcher = WPFChild.Dispatcher

                    ' Bette in dem UserControl den ElementHost als Container für das eigentliche NotePaneView ein.
                    UC.Controls.Add(New ElementHost With {.Child = WPFChild,
                                                          .Dock = DockStyle.Fill,
                                                          .AutoSize = True})

                    ' Pane hinzufügen
                    CallListPane = Globals.ThisAddIn.CustomTaskPanes.Add(UC, Localize.LocCallListPane.strPaneHead, OlExplorer)

                    NLogger.Debug("Ein neues CallListPane wurde erstellt")
                End If

                AddHandler CallListPane.VisibleChanged, AddressOf CallListPane_VisibleChanged

                If XMLData.POptionen.CBShowCallPaneAtStart Then ShowCallListPane()

            End If

        End If

    End Sub

#Region "CallListPane"
    ''' <summary>
    ''' Blendet das CallListPane ein
    ''' </summary>
    Friend Sub ShowCallListPane()
        If CallListPane IsNot Nothing Then
            ' Lege die Breite fest, falls der Pane nicht eigeblendet ist
            If Not CallListPane.Visible Then CallListPane.Width = XMLData.POptionen.TBCallPaneStartWidth
            ' Blende das CallListPane ein
            CallListPane.Visible = True
        Else
            NLogger.Warn("Das CallListPane kann nicht eingeblendet werden, da es Nothing (null) ist.")
        End If
    End Sub

    ''' <summary>
    ''' Blendet das CallListPane aus
    ''' </summary>
    Private Sub HideCallListPane()
        If CallListPane IsNot Nothing Then
            CallListPane.Visible = False
        Else
            NLogger.Warn("Das CallListPane kann nicht ausgeblendet werden, da es Nothing (null) ist.")
        End If
    End Sub

    Private Sub CallListPane_VisibleChanged(sender As Object, e As EventArgs)
        ' Leere die Liste, wenn das Pane geschlossen wird.
        If XMLData.POptionen.CBClearCallPaneAtClose And Not CallListPane.Visible Then CallListPaneVM.MissedCallList.Clear()

        Globals.ThisAddIn.POutlookRibbons.Invalidate()
    End Sub

    ''' <summary>
    ''' Ermittelt einen vorhandenes CallListPane
    ''' </summary>
    Private Function GetExistingCallListPane() As Boolean

        ' Vergleiche nach Namen
        With Globals.ThisAddIn.CustomTaskPanes.Where(Function(P) P.Window.Equals(OlExplorer) And P.Title.IsEqual(Localize.LocCallListPane.strPaneHead))
            If .Any Then
                ' Es sollte nur Pane geben geben
                CallListPane = .First

                With CallListPane.Control.Controls

                    ' Sofern Controls vorhanden sind und das erste Element vom Typ ElementHost ist
                    If .Count.IsNotZero AndAlso TypeOf .Item(0) Is ElementHost Then

                        ' Weise die fehlenden Eigenschaften dem ExplorerWrapper hinzu
                        With CType(CType(.Item(0), ElementHost).Child, CallListPaneView)
                            ' Ermittle den Dispatcher
                            PaneDispatcher = .Dispatcher

                            ' Ermittle das Viewmodel
                            CallListPaneVM = CType(.DataContext, CallListPaneViewModel)
                        End With

                        NLogger.Debug("Ein vorhandenes CallListPane wurde ermittelt")

                        Return True
                    End If

                End With
            End If
        End With

        ' Standard-Rückgabewert
        Return False
    End Function

    ''' <summary>
    ''' Fügt ein Eintrag zu der Liste verpasster Telefonate hinzu.
    ''' </summary>
    ''' <param name="MissedCall">Telefonat, welches hinzugefügt werden soll.</param>
    Friend Sub AddMissedCall(MissedCall As Telefonat)
        PaneDispatcher?.Invoke(Sub()
                                   ' Blende das Pane ein
                                   ShowCallListPane()

                                   Dim AnrMonList = CallListPaneVM.MissedCallList.Where(Function(T) T.VerpasstesTelefonat.GegenstelleTelNr.Equals(MissedCall.GegenstelleTelNr) And
                                                                                                    T.VerpasstesTelefonat.EigeneTelNr.Equals(MissedCall.EigeneTelNr))

                                   If XMLData.POptionen.CBAnrMonHideMultipleCall AndAlso AnrMonList.Any Then
                                       With AnrMonList.First
                                           ' Setze den Zähler hoch
                                           .AnzahlAnrufe += 1
                                           ' Aktualisiere die Zeit
                                           .VerpasstesTelefonat.ZeitBeginn = MissedCall.ZeitBeginn
                                       End With
                                   Else
                                       ' Füge das Telefonat als verpasstes Element hinzu
                                       CallListPaneVM.MissedCallList.Add(New MissedCallViewModel(Datenservice, Dialogservice) With {.VerpasstesTelefonat = MissedCall,
                                                                                                                                    .Instance = PaneDispatcher})
                                   End If

                                   ' Sortiere die Liste
                                   CallListPaneVM.MissedCallList.SortDescending(Function(T) T.Zeit)
                               End Sub)
    End Sub

    ''' <summary>
    ''' Entfernt ein Eintrag aus der Liste verpasster Telefonate.
    ''' </summary>
    ''' <param name="MissedCall">Telefonat, welches entfernt werden soll.</param>
    Friend Sub RemoveMissedCall(MissedCall As Telefonat)
        If CallListPaneVM IsNot Nothing Then
            With CallListPaneVM.MissedCallList
                ' Finde alle passenden Einträge und entferne diese
                NLogger.Debug($"Verpasster Anruf {MissedCall.NameGegenstelle} ({MissedCall.ZeitBeginn}) wird aus dem CallPane des entfernt.")

                PaneDispatcher?.Invoke(Sub() .RemoveRange(.Where(Function(C) C.VerpasstesTelefonat.Equals(MissedCall))))

                ' Schließe das Pane, wenn gewünscht
                If Not .Any And XMLData.POptionen.CBCloseEmptyCallPane Then HideCallListPane()
            End With
        End If
    End Sub

#End Region

#Region "Explorer Evendhandler"
    ''' <summary>
    ''' Tritt ein, wenn ein Outlook-Element eingefügt wird.
    ''' </summary>
    Private Sub OutlookExplorer_BeforeItemPaste(ByRef ClipboardContent As Object, Target As MAPIFolder, ByRef Cancel As Boolean)
        ' Ist der Inhalt eine Selection? (Im Besten Fall eine Anzahl an Kontakten)
        If TypeOf ClipboardContent Is Selection Then

            ' Schleife durch alle Elemente der selektierten Objekte
            For Each ClipboardObject As Object In CType(ClipboardContent, Selection)

                ' Wenn es sich um Kontakte handelt, dann 
                If TypeOf ClipboardObject Is ContactItem Then

                    ' Füge einen Ereignishandler für das Target hinzu
                    AddHandler Target.Items.ItemAdd, AddressOf Target_ItemAdd

                    With CType(ClipboardObject, ContactItem)

                        ' Entferne alle Synchronisierungsdaten aus dem Kontakt
                        '.DeleteUniqueID

                        ' Synchronisiere den Kontakt
                        '.SyncKontakt(Target, True)

                        ' (de-)indiziere den Kontakt
                        '.IndiziereKontakt(Target)
                    End With

                End If
            Next
        End If
    End Sub

    Private Sub Target_ItemAdd(Item As Object)
        ' Wenn es sich um Kontakte handelt, dann 
        If TypeOf Item Is ContactItem Then

            With CType(Item, ContactItem)
                ' Entfernt den Ereignishandler für das eingefügte Element hinzu
                RemoveHandler .ParentFolder.Items.ItemAdd, AddressOf Target_ItemAdd

                ' Entferne alle Synchronisierungsdaten aus dem Kontakt
                .DeleteUniqueID

                OlPastedItem = New OutlookItemWrapper(Item)

                .Speichern()
            End With

        End If
    End Sub

    Private Sub Explorer_SelectionChange()
        Globals.ThisAddIn.POutlookRibbons.RefreshRibbon()

        ClearSelectedItemList()

        If OlExplorer.Selection.Count.IsNotZero Then
            For Each Item In OlExplorer.Selection
                OlSelectedItems.Add(New OutlookItemWrapper(Item))
            Next
        End If
    End Sub

    Private Sub ClearSelectedItemList()
        ' Entferne die Verweise auf die Eventhandler
        OlSelectedItems.ForEach(Sub(ItemWrapper) ItemWrapper.Auflösen())

        ' Leere die Liste
        OlSelectedItems.Clear()
    End Sub

    ''' <summary>
    ''' Entfernt einen <see cref="OutlookItemWrapper"/> aus der Liste der selektierten Outlook-Items. 
    ''' Der Abgleich der gleichheit erfolgt über die <see cref="ContactItem.EntryID"/> des <paramref name="CurrentItem"/> und der <see cref="MAPIFolder.StoreID"/> des Ordners indem sich das <paramref name="CurrentItem"/> befindet.
    ''' </summary>
    ''' <param name="CurrentItem">Das gegenständliche Item</param>
    Friend Sub RemoveSelectedItem(CurrentItem As OutlookItemWrapper)
        ' Suche anhand der EntryID und StoreID den passenden ItemWrapper.
        Dim EqualItemWrapper As OutlookItemWrapper = OlSelectedItems.Find(Function(IW) IW.Equals(CurrentItem))

        ' Wenn ein passender ItemWrapper gefunden wurde, dann löse ihn auf und entferne ihn aus der Liste
        If EqualItemWrapper IsNot Nothing Then
            EqualItemWrapper.Auflösen()
            OlSelectedItems.Remove(EqualItemWrapper)
        End If
    End Sub

    Private Sub Explorer_Close()
        ClearSelectedItemList()

        ' Entferne Pane
        If CallListPane IsNot Nothing Then
            ' Leere die Liste 
            CallListPaneVM.MissedCallList.Clear()

            ' Entferne Pane Eventhandler
            RemoveHandler CallListPane.VisibleChanged, AddressOf CallListPane_VisibleChanged
            NLogger.Debug("Das CallListPane wird aus den CustomTaskPanes entfernt.")
            ' Entferne Pane
            Globals.ThisAddIn.CustomTaskPanes.Remove(CallListPane)
        End If

        ' Entferne Explorer Eventhandler
        RemoveHandler OlExplorer.Close, AddressOf Explorer_Close
        RemoveHandler OlExplorer.SelectionChange, AddressOf Explorer_SelectionChange
        RemoveHandler OlExplorer.BeforeItemPaste, AddressOf OutlookExplorer_BeforeItemPaste
        Globals.ThisAddIn.ExplorerWrappers.Remove(OlExplorer)

        ' Gib das OfficeObjekt frei
        ReleaseComObject(OlExplorer)
        ' Setze Eigenschaften auf Nothing
        CallListPane = Nothing
    End Sub

#End Region

End Class
