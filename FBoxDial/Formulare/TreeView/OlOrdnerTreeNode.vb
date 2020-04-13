Imports System.Windows.Forms
Imports Microsoft.Office.Interop

Friend Class OlOrdnerTreeNode
    Inherits TreeNode

    Friend Property OutlookStore As Outlook.Store
    Friend Property OutlookFolder As Outlook.MAPIFolder
    Friend Property Durchsuchen As Boolean
    Friend Property XMLEintrag As IndizerterOrdner

    Friend Sub SetImageKey()
        If OutlookFolder.DefaultItemType = Outlook.OlItemType.olContactItem Then

            If XMLEintrag IsNot Nothing Then
                ImageKey = "Checked"
                ForeColor = Drawing.Color.Blue
                Durchsuchen = True

            Else

                ' Kontaktsuche einbeziehung der Unterordner
                If XMLData.POptionen.PCBSucheUnterordner AndAlso Parent IsNot Nothing Then
                    With CType(Parent, OlOrdnerTreeNode)
                        If .XMLEintrag IsNot Nothing Or .Durchsuchen Then
                            ImageKey = "Mix"
                            Durchsuchen = True
                        Else
                            ImageKey = "Uncheck"
                            Durchsuchen = False
                        End If
                    End With

                Else
                    ImageKey = "Uncheck"
                    Durchsuchen = False
                End If

                'ImageKey = "Uncheck"
                ForeColor = Drawing.Color.Empty
            End If

            ' Wenn unterordner durchsucht werden sollen, müssen alle nachfolgenden Ordner markiert werden.
            If XMLData.POptionen.PCBSucheUnterordner Then
                ' Unterknoten rekursiv überarbeiten
                For Each tmpnode As OlOrdnerTreeNode In Nodes
                    tmpnode.SetImageKey()
                Next
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
