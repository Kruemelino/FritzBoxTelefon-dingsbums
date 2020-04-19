Imports System.Collections
Imports System.ComponentModel
Imports System.Reflection
 
	Public Class PropertyComparer(Of T)
		Implements IComparer(Of T)
		Private ReadOnly comparer As IComparer
		Private propertyDescriptor As PropertyDescriptor
		Private reverse As Integer

		Public Sub New([property] As PropertyDescriptor, direction As ListSortDirection)
		propertyDescriptor = [property]
		Dim comparerForPropertyType As Type = GetType(Comparer(Of )).MakeGenericType([property].PropertyType)
		comparer = DirectCast(comparerForPropertyType.InvokeMember("Default", BindingFlags.[Static] Or BindingFlags.GetProperty Or BindingFlags.[Public], Nothing, Nothing, Nothing), IComparer)
		SetListSortDirection(direction)
	End Sub

#Region "IComparer<T> Members"

	Public Function Compare(x As T, y As T) As Integer Implements IComparer(Of T).Compare
		Return reverse * comparer.Compare(propertyDescriptor.GetValue(x), propertyDescriptor.GetValue(y))
	End Function

#End Region

	Private Sub SetPropertyDescriptor(descriptor As PropertyDescriptor)
		propertyDescriptor = descriptor
	End Sub

	Private Sub SetListSortDirection(direction As ListSortDirection)
		reverse = If(direction = ListSortDirection.Ascending, 1, -1)
	End Sub

	Public Sub SetPropertyAndDirection(descriptor As PropertyDescriptor, direction As ListSortDirection)
		SetPropertyDescriptor(descriptor)
		SetListSortDirection(direction)
	End Sub
	End Class
  
			  

																 