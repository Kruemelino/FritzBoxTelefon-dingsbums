Imports System.Windows.Forms
Imports Microsoft.Office.Interop

Friend Class OlOrdnerTreeNode
    Inherits TreeNode

    Friend Property OutlookStore As Outlook.Store
    Friend Property OutlookFolder As Outlook.MAPIFolder

    Friend XMLEintrag As IndizerterOrdner
    Friend Sub SetImageKey()
        If OutlookFolder.DefaultItemType = Outlook.OlItemType.olContactItem Then

            If XMLEintrag IsNot Nothing Then
                ImageKey = "Checked"
                ForeColor = Drawing.Color.Blue
            Else
                ImageKey = "Uncheck"
                ForeColor = Drawing.Color.Empty
            End If
        Else
            ImageKey = "Disabled"
            ForeColor = Drawing.Color.DarkGray
        End If
        SelectedImageKey = ImageKey
    End Sub

    Friend Sub Erweitern()
        If Nodes.Count.IsZero And OutlookFolder.Folders.Count.IsNotZero Then
            For Each Ordner As Outlook.MAPIFolder In OutlookFolder.Folders

                Dim olTreeNode As New OlOrdnerTreeNode With {.Text = Ordner.Name, .OutlookStore = OutlookStore, .OutlookFolder = Ordner}
                ' Prüfe ob der ordner aus den Einstellungen heraus indiziert werden soll
                With olTreeNode
                    If .OutlookFolder.DefaultItemType = Outlook.OlItemType.olContactItem AndAlso .XMLEintrag Is Nothing Then
                        .XMLEintrag = XMLData.POptionen.IndizerteOrdner.OrdnerListe.Find(Function(eintrag) eintrag.FolderID.AreEqual(.OutlookFolder.EntryID) And eintrag.StoreID.AreEqual(.OutlookStore.StoreID))
                    End If
                    .SetImageKey()
                End With
                Nodes.Add(olTreeNode)
            Next
            'Sortieren
            Nodes.Sort(True, False)
            ' Setze das Immage
            SetImageKey()
            ' Erweitern
            Expand()

        End If
    End Sub



End Class
