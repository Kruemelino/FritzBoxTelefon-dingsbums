Imports System.Windows.Forms
Imports Microsoft.Office.Interop

<Obsolete> Friend Class OlOrdnerTreeNode
    Inherits TreeNode

    Friend Property OutlookStore As Outlook.Store
    Friend Property OutlookFolder As Outlook.MAPIFolder
    Friend Property OutlookItemType As Outlook.OlItemType
    Friend Property Durchsuchen As Boolean
    Friend Property XMLEintrag As OutlookOrdner
    Friend Property AutoCheckSubNodes As Boolean
    Friend Property StoreNode As Boolean
    Friend Property Verwendung As OutlookOrdnerVerwendung

    Friend Sub SetImageKey()
        ' If OutlookFolder.DefaultItemType = Outlook.OlItemType.olContactItem Then
        If OutlookFolder.DefaultItemType = OutlookItemType Then

            If XMLEintrag IsNot Nothing Then
                ImageKey = "Checked"
                ForeColor = Drawing.Color.Blue
                Durchsuchen = True

            Else
                ' Kontaktsuche Einbeziehung der Unterordner
                If AutoCheckSubNodes AndAlso Parent IsNot Nothing Then
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
            ' Wenn Unterordner durchsucht werden sollen, müssen alle nachfolgenden Ordner markiert werden.
            If XMLData.POptionen.CBSucheUnterordner Then
                ' Unterknoten rekursiv überarbeiten
                For Each tmpnode As OlOrdnerTreeNode In Nodes
                    tmpnode.SetImageKey()
                Next
            End If

        Else
            ImageKey = "Disabled"
            ForeColor = If(StoreNode, Drawing.Color.Empty, Drawing.Color.DarkGray)
        End If
        SelectedImageKey = ImageKey
    End Sub

    ''' <summary>
    ''' Erweitert einen Outlook-Treenode um die jeweiligen Unterordner des Outlook-Ordners
    ''' </summary>
    Friend Sub Erweitern()
        If Nodes.Count.IsZero And OutlookFolder.Folders.Count.IsNotZero Then

            'Schleife durch jeden Ornder dieses Outlook-Ordners
            For Each Ordner As Outlook.MAPIFolder In OutlookFolder.Folders
                ' Dimensioniere ein neues TreeNode für Outlook-Ordner
                Dim olTreeNode As New OlOrdnerTreeNode With {.Text = Ordner.Name,
                                                             .OutlookStore = OutlookStore,
                                                             .OutlookFolder = Ordner,
                                                             .OutlookItemType = OutlookItemType,
                                                             .Name = $"{OutlookStore.StoreID}{Ordner.EntryID}",
                                                             .AutoCheckSubNodes = AutoCheckSubNodes,
                                                             .StoreNode = False,
                                                             .Verwendung = Verwendung}

                ' Prüfe ob der Ordner aus den Einstellungen heraus verarbeitet werden soll
                With olTreeNode
                    If .OutlookFolder.DefaultItemType = OutlookItemType Then ' AndAlso .XMLEintrag Is Nothing Then
                        .XMLEintrag = XMLData.POptionen.OutlookOrdner.Find(.OutlookStore.StoreID, .OutlookFolder.EntryID, Verwendung)
                    End If
                    ' Setze das Icon
                    .SetImageKey()
                End With

                Nodes.Add(olTreeNode)
            Next
            'Sortieren
            Nodes.Sort(True, False)
            ' Setze das Image
            SetImageKey()
            ' Erweitern
            Expand()
        End If
    End Sub

End Class
