Imports System.Windows
Imports System.Windows.Controls
Imports Microsoft.Office.Interop

Partial Public Class OptionsOLFolderTV
    Inherits UserControl

#Region "OutlookOlItemType"

    Public Property OutlookOlItemType As Outlook.OlItemType
        Get
            Return CType(GetValue(OutlookOlItemTypeProperty), Outlook.OlItemType)
        End Get
        Set(ByVal value As Outlook.OlItemType)
            SetValue(OutlookOlItemTypeProperty, value)
        End Set
    End Property

    Public Shared ReadOnly OutlookOlItemTypeProperty As DependencyProperty = DependencyProperty.Register("OutlookOlItemType", GetType(Outlook.OlItemType), GetType(OptionsOLFolderTV), New PropertyMetadata(Outlook.OlItemType.olMailItem))

#End Region

#Region "ÜberwachteOrdnerListe"
    Public Property ÜberwachteOrdnerListe As ObservableCollectionEx(Of OutlookOrdner)
        Get
            Return CType(GetValue(ÜberwachteOrdnerListeProperty), ObservableCollectionEx(Of OutlookOrdner))
        End Get
        Set(ByVal value As ObservableCollectionEx(Of OutlookOrdner))
            SetValue(ÜberwachteOrdnerListeProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ÜberwachteOrdnerListeProperty As DependencyProperty = DependencyProperty.Register("ÜberwachteOrdnerListe", GetType(ObservableCollectionEx(Of OutlookOrdner)), GetType(OptionsOLFolderTV), New PropertyMetadata(New ObservableCollectionEx(Of OutlookOrdner)))
#End Region

#Region "Verwendung"

    Public Property Verwendung As OutlookOrdnerVerwendung
        Get
            Return CType(GetValue(VerwendungProperty), OutlookOrdnerVerwendung)
        End Get
        Set(ByVal value As OutlookOrdnerVerwendung)
            SetValue(VerwendungProperty, value)
        End Set
    End Property

    Public Shared ReadOnly VerwendungProperty As DependencyProperty = DependencyProperty.Register("Verwendung", GetType(OutlookOrdnerVerwendung), GetType(OptionsOLFolderTV), New PropertyMetadata(OutlookOrdnerVerwendung.KontaktSuche))

#End Region

    Public Sub New()
        InitializeComponent()

    End Sub

    'Private Sub AddOutlookBaseNodes(OutlookItemType As Outlook.OlItemType, Verwendung As OutlookOrdnerVerwendung, MultiSelect As Boolean, CheckSubNodes As Boolean)
    Private Sub AddOutlookBaseNodes()

        With olFldrTV
            .Items.Clear()
            ' Füge für jeden Outlook Store ein Treenode hinzu
            If .Items.Count.IsZero Then
                ' Lade Outlook Store
                For Each OutlookStore As Outlook.Store In ThisAddIn.POutookApplication.Session.Stores
                    .Items.Add(StoreTreeItem(OutlookStore))
                Next
            End If
        End With
    End Sub

    Private Sub TreeView_Expanded(sender As Object, e As RoutedEventArgs)

        Dim olTreeViewItem As TreeViewItem = CType(e.OriginalSource, TreeViewItem)

        With olTreeViewItem
            With CType(olTreeViewItem.DataContext, OlTreeViewItem)

                If .Unterordner.Count = 1 AndAlso .Unterordner.First.Title.AreEqual("Loading...") Then
                    .Unterordner.Clear()

                    For Each OutlookFolder As Outlook.MAPIFolder In .OutlookFolder.Folders
                        .Unterordner.Add(FolderTreeItem(OutlookFolder))
                    Next

                End If
            End With
        End With
    End Sub

    Private Function StoreTreeItem(ByVal OutlookStore As Outlook.Store) As OlTreeViewItem
        ' Dim olTreeViewItem As New OlTreeViewItem(OutlookStore.GetRootFolder) With {.Title = $"{OutlookStore.GetRootFolder.Name} ({OutlookStore.ExchangeStoreType})"}
        Dim olTreeViewItem As New OlTreeViewItem() With {.Title = $"{OutlookStore.GetRootFolder.Name} ({OutlookStore.ExchangeStoreType})",
                                                         .OutlookFolder = OutlookStore.GetRootFolder,
                                                         .OutlookItemType = OutlookOlItemType.olTaskItem,
                                                         .TreeViewSelectionOutlookItemType = Nothing,
                                                         .Überwacht = False
                                                        }

        If OutlookStore.GetRootFolder.Folders.Count.IsNotZero Then olTreeViewItem.Unterordner.Add(New OlTreeViewItem With {.Title = "Loading..."})

        Return olTreeViewItem
    End Function

    Private Function FolderTreeItem(ByVal OutlookFolder As Outlook.MAPIFolder) As OlTreeViewItem
        Dim olTreeViewItem As New OlTreeViewItem With {.Title = $"{OutlookFolder.Name}",
                                                       .OutlookFolder = OutlookFolder,
                                                       .OutlookItemType = OutlookFolder.DefaultItemType,
                                                       .TreeViewSelectionOutlookItemType = OutlookOlItemType,
                                                       .Überwacht = OrdnerÜberwacht(OutlookFolder)
                                                      }

        If OutlookFolder.Folders.Count.IsNotZero Then olTreeViewItem.Unterordner.Add(New OlTreeViewItem With {.Title = "Loading..."})
        Return olTreeViewItem
    End Function

    Private Function OrdnerÜberwacht(ByVal OutlookFolder As Outlook.MAPIFolder) As Boolean
        If OutlookFolder.DefaultItemType = OutlookOlItemType Then
            Return ÜberwachteOrdnerListe.Where(Function(O) O.Typ = Verwendung AndAlso O.StoreID.AreEqual(OutlookFolder.StoreID) AndAlso O.FolderID.AreEqual(OutlookFolder.EntryID)).Any
        Else
            Return False
        End If
    End Function

    Private Sub CheckBox_CheckedChanged(sender As Object, e As RoutedEventArgs)

        Dim CheckBoxNode As CheckBox = CType(sender, CheckBox)

        With CheckBoxNode
            With CType(.DataContext, OlTreeViewItem)
                Dim tmpfold As OutlookOrdner = New OutlookOrdner(.OutlookFolder, Verwendung)

                Select Case True
                    Case e.RoutedEvent Is Primitives.ToggleButton.CheckedEvent

                        ' Unterscheidung nach Verwendung:
                        ' Bei Kontaktsuche ist Mehrfachauswahl möglich, bei den anderen nicht.
                        If Verwendung = OutlookOrdnerVerwendung.KontaktSuche Then
                            ' Den ausgewählten Ordner hinzufügen, falls er nicht schon dabei ist
                            If Not ÜberwachteOrdnerListe.Where(Function(Ordner) Ordner.Equals(tmpfold)).Any Then ÜberwachteOrdnerListe.Add(tmpfold)

                        Else
                            ' Alle Ordner mit der passenden Verwendung finden
                            Dim tmOrdList As List(Of OutlookOrdner) = ÜberwachteOrdnerListe.Where(Function(Ordner) Ordner.Typ = Verwendung).ToList

                            ' Entferne alle Ordner mit der Verwendung
                            ÜberwachteOrdnerListe.RemoveRange(tmOrdList)

                            ' Alle anderen selektierten Knoten deselektieren
                            UnCheckAllExeptFolder(olFldrTV.Items, tmpfold)

                            ' Den ausgewählten Ordner hinzufügen,
                            ÜberwachteOrdnerListe.Add(tmpfold)
                        End If


                    Case e.RoutedEvent Is Primitives.ToggleButton.UncheckedEvent
                        ' Den ausgewählten Ordner entfernen
                        ÜberwachteOrdnerListe.Remove(tmpfold)

                End Select

            End With
        End With
    End Sub

    Sub UnCheckAllExeptFolder(Ordnerliste As ItemCollection, Folder As OutlookOrdner)

        For Each Ornder As OlTreeViewItem In Ordnerliste
            Ornder.Überwacht = Folder.Equals(Ornder.OutlookFolder, Verwendung)
            UnCheckAllExeptFolder(Ornder.Unterordner, Folder)
        Next

    End Sub

    Sub UnCheckAllExeptFolder(Ordnerliste As ObservableCollectionEx(Of OlTreeViewItem), Folder As OutlookOrdner)

        For Each Ornder As OlTreeViewItem In Ordnerliste
            Ornder.Überwacht = Folder.Equals(Ornder.OutlookFolder, Verwendung)
            UnCheckAllExeptFolder(Ornder.Unterordner, Folder)
        Next

    End Sub

End Class