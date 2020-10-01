Imports System.Collections.ObjectModel


''' <summary>
''' Expanded ObservableCollection to include some List(Of T) And sort Methods e.g. myCol.Sort(x => x.Name)
''' </summary>
<Serializable>
Public Class ObservableCollectionEx(Of T)
    Inherits ObservableCollection(Of T)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal l As List(Of T))
        MyBase.New(l)
    End Sub

    Public Sub New(ByVal l As IEnumerable(Of T))
        MyBase.New(l)
    End Sub

#Region "Sort"

    ''' <summary>
    ''' Sorts the items of the collection in ascending order according to a key.
    ''' </summary>
    ''' <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    ''' <param name="keySelector">A function to extract a key from an item.</param>
    Public Sub Sort(Of TKey)(ByVal keySelector As Func(Of T, TKey))
        InternalSort(Items.OrderBy(keySelector))
    End Sub

    ''' <summary>
    ''' Sorts the items of the collection in descending order according to a key.
    ''' </summary>
    ''' <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    ''' <param name="keySelector">A function to extract a key from an item.</param>
    Public Sub SortDescending(Of TKey)(ByVal keySelector As Func(Of T, TKey))
        InternalSort(Items.OrderByDescending(keySelector))
    End Sub

    ''' <summary>
    ''' Sorts the items of the collection in ascending order according to a key.
    ''' </summary>
    ''' <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
    ''' <param name="keySelector">A function to extract a key from an item.</param>
    ''' <param name="comparer">An <see cref="IComparer(Of T)"/> to compare keys.</param>
    Public Sub Sort(Of TKey)(ByVal keySelector As Func(Of T, TKey), ByVal comparer As IComparer(Of TKey))
        InternalSort(Items.OrderBy(keySelector, comparer))
    End Sub

    ''' <summary>
    ''' Moves the items of the collection so that their orders are the same as those of the items provided.
    ''' </summary>
    ''' <param name="sortedItems">An <see cref="IEnumerable(Of T)"/> to provide item orders.</param>
    Private Sub InternalSort(ByVal sortedItems As IEnumerable(Of T))
        Try
            Dim sortedItemsList = sortedItems.ToList()

            For Each Item As T In sortedItemsList
                Move(IndexOf(Item), sortedItemsList.IndexOf(Item))
            Next
        Catch
        End Try
    End Sub
#End Region

    Public Sub RemoveRange(ByVal List As IEnumerable(Of T))
        For Each Item As T In List
            Remove(Item)
        Next
    End Sub

End Class
