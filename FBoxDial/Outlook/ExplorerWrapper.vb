Imports Microsoft.Office.Interop.Outlook
Imports Microsoft.Office.Tools
Imports System.Windows.Forms.Integration
Imports System.Windows.Forms

Friend Class ExplorerWrapper
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property OlExplorer As Explorer
    Private Property CallListPane As CustomTaskPane
    Friend Property CallListPaneVM As CallListPaneViewModel
    Friend Property Datenservice As IAnrMonService
    Friend Property Dialogservice As IDialogService
    Private Property PaneDispatcher As Windows.Threading.Dispatcher

    Friend Sub New(e As Explorer)
        OlExplorer = e

        If OlExplorer IsNot Nothing Then
            NLogger.Debug("Ein neues Explorer-Fenster wird geöffnet")
            ' Füge drei Ereignishandler des Explorers hinzu
            AddHandler OlExplorer.Close, AddressOf Explorer_Close
            AddHandler OlExplorer.BeforeItemPaste, AddressOf OutlookExplorer_BeforeItemPaste
            AddHandler OlExplorer.SelectionChange, AddressOf Explorer_SelectionChange

            ' CallListPane 

            If XMLData.POptionen.CBShowMissedCallPane Then
                Dim UC As New UserControl

                Datenservice = New AnrMonService
                Dialogservice = New DialogService

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
                AddHandler CallListPane.VisibleChanged, AddressOf CallListPane_VisibleChanged

                NLogger.Debug("Ein neues CallListPane wurde erstellt")

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
    Friend Sub HideCallListPane()
        If CallListPane IsNot Nothing Then
            CallListPane.Visible = False
        Else
            NLogger.Warn("Das CallListPane kann nicht ausgeblendet werden, da es Nothing (null) ist.")
        End If
    End Sub

    Private Sub CallListPane_VisibleChanged(sender As Object, e As EventArgs)
        ' Leere die Liste, wenn das Pane geschlossen wird.
        If XMLData.POptionen.CBClearCallPaneAtClose And Not CallListPane.Visible Then CallListPaneVM.MissedCallList.Clear()
    End Sub

    Friend Sub AddMissedCall(MissedCall As Telefonat)
        PaneDispatcher.Invoke(Sub()
                                  ' Blende das Pane ein
                                  ShowCallListPane()

                                  ' Füge das Telefonat als verpasstes Element hinzu
                                  CallListPaneVM.MissedCallList.Add(New MissedCallViewModel(Datenservice, Dialogservice) With {.VerpasstesTelefonat = MissedCall})

                                  ' Sortiere die Liste
                                  CallListPaneVM.MissedCallList.SortDescending(Function(T) T.Zeit)
                              End Sub)
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

                ' Wenn es sich um Kontakte handelt, dann (de-)indiziere den Kontakt
                If TypeOf ClipboardObject Is ContactItem Then

                    IndiziereKontakt(CType(ClipboardObject, ContactItem), Target, True)

                End If
            Next
        End If
    End Sub

    Private Sub Explorer_SelectionChange()
        Globals.ThisAddIn.POutlookRibbons.RefreshRibbon()
    End Sub

    Private Sub Explorer_Close()

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
