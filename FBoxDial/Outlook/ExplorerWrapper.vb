Imports Microsoft.Office.Interop.Outlook

Friend Class ExplorerWrapper
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Private Property Explorer As Explorer

    Friend Sub New(e As Explorer)
        Explorer = e

        If Explorer IsNot Nothing Then
            NLogger.Debug("Ein neues Explorer-Fenster wird geöffnet")

            AddHandler Explorer.Close, AddressOf Explorer_Close
            AddHandler Explorer.BeforeItemPaste, AddressOf OutlookExplorer_BeforeItemPaste
            AddHandler Explorer.SelectionChange, AddressOf Explorer_SelectionChange

        End If

    End Sub

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
        RemoveHandler Explorer.Close, AddressOf Explorer_Close
        RemoveHandler Explorer.SelectionChange, AddressOf Explorer_SelectionChange
        RemoveHandler Explorer.BeforeItemPaste, AddressOf OutlookExplorer_BeforeItemPaste

        Globals.ThisAddIn.ExplorerWrappers.Remove(Explorer)
    End Sub
End Class
