Imports System.Windows.Forms
Imports Microsoft.Office.Interop
Imports System.Reflection

Public Class TreeViewEx
    Inherits TreeView


#Region "Constructors"

    Sub New()
        ' Double Buffered einschalten
        [GetType].GetProperty("DoubleBuffered", BindingFlags.Instance Or BindingFlags.NonPublic).SetValue(Me, True, Nothing)

        ' Lade ImageList
        ImageList = New ImageList
        With ImageList.Images
            .Add("Disabled", My.Resources.CheckboxDisable)
            .Add("Mix", My.Resources.CheckboxMix)
            .Add("Checked", My.Resources.CheckBox)
            .Add("Uncheck", My.Resources.CheckboxUncheck)
        End With
    End Sub

#End Region
    Private Property OutlookOrdnerType As Outlook.OlItemType
    Private Property IsMultiSelect As Boolean
    Friend Property CheckedOlFolders As List(Of OutlookOrdner)

    Friend Sub AddOutlookBaseNodes(OutlookItemType As Outlook.OlItemType, MultiSelect As Boolean, CheckSubNodes As Boolean)
        ' setze eigene Eigenschaften
        IsMultiSelect = MultiSelect
        OutlookOrdnerType = OutlookItemType

        ' Füge für jeden Outlook Store ein Treenode hinzu
        If Nodes.Count.IsZero Then
            Nodes.Clear()
            ' Lade Outlook Store
            For Each Store As Outlook.Store In ThisAddIn.POutookApplication.Session.Stores
                Dim olTreeNode As New OlOrdnerTreeNode With {.Text = $"{Store.GetRootFolder.Name} ({Store.ExchangeStoreType})",
                                                             .OutlookStore = Store,
                                                             .OutlookFolder = Store.GetRootFolder,
                                                             .ImageKey = "Disabled",
                                                             .OutlookItemType = OutlookOrdnerType,
                                                             .AutoCheckSubNodes = CheckSubNodes,
                                                             .StoreNode = True}
                Nodes.Add(olTreeNode)
            Next
            ' Sortieren
            Nodes.Sort(True, False)
        End If
    End Sub

    Private Sub OlOrdnerTreeView_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles Me.NodeMouseClick

        Dim olBaseTreeNode As OlOrdnerTreeNode = CType(e.Node, OlOrdnerTreeNode)

        With olBaseTreeNode
            If .OutlookFolder.DefaultItemType = OutlookOrdnerType Then

                If .TreeView.HitTest(e.Location).Location = TreeViewHitTestLocations.Image Then

                    If .XMLEintrag Is Nothing Then
                        .XMLEintrag = New OutlookOrdner With {.Name = olBaseTreeNode.OutlookFolder.Name, .FolderID = olBaseTreeNode.OutlookFolder.EntryID, .StoreID = olBaseTreeNode.OutlookFolder.StoreID}
                        ' Initiiere die Liste, falls noch nicht geschehen
                        If CheckedOlFolders Is Nothing Then CheckedOlFolders = New List(Of OutlookOrdner)

                        ' Wenn nur ein Ordner ausgewählt werden darf, dann lösche alle anderen aus der Liste
                        If Not IsMultiSelect Then
                            ' Schleife durch alle möglichen Ordner (für den Fall, dass doch mehr als einer in der Liste)

                            Do While CheckedOlFolders.Any
                                With CheckedOlFolders(0)
                                    ' Schleife durch alle Treenodes, um sie zu entchecken (für den Fall, dass doch mehr als einer in der Liste)
                                    For Each CheckedNode As OlOrdnerTreeNode In Nodes.Find($"{ .StoreID()}{ .FolderID}", True)
                                        CheckedNode.XMLEintrag = Nothing
                                        CheckedNode.SetImageKey()
                                    Next
                                    ' Entferne den Ordner aus der Liste
                                    CheckedOlFolders.Remove(CheckedOlFolders(0))
                                End With
                            Loop
                        End If

                        ' Ordner hinzufügen
                        CheckedOlFolders.Add(.XMLEintrag)
                    Else
                        ' Ordner entfernen
                        CheckedOlFolders.Remove(.XMLEintrag)
                        ' XML-Element leeren
                        .XMLEintrag = Nothing
                    End If

                    ' ImageKey setzen
                    .SetImageKey()
                End If
            End If
            ' Lade alle direkten Unterordner
            .Erweitern()
        End With
    End Sub

End Class