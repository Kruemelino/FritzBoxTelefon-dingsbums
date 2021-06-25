Imports System.ComponentModel
Imports Microsoft.Office.Interop.Outlook

''' <summary>
''' Defines an interface for a viemodel that can be bound to a treeviewitem
''' with a checkbox.<br/>
''' In Anlehnung an Dirk Bahle: <code>https://www.codeproject.com/Articles/1224943/Advanced-WPF-TreeView-in-Csharp-VB-Net-Part-of-n</code>
''' </summary>
Public Interface IOlFolderViewModel
    Inherits INotifyPropertyChanged

    ''' <summary>
    ''' Gets the children items underneath this item.
    ''' </summary>
    ReadOnly Property ChildFolders As IEnumerable(Of IOlFolderViewModel)

    ''' <summary>
    ''' Gets the number of children stored below this item.
    ''' </summary>
    ReadOnly Property ChildFolderCount As Integer

    ''' <summary>
    ''' Gets/sets the state of the associated UI toggle (ex. CheckBox).
    ''' 
    ''' The return value is calculated based on the check state of all
    ''' child FooViewModels. Setting this property to true or false
    ''' will set all children to the same check state, and setting it 
    ''' to any value will cause the parent to verify its check state.
    ''' </summary>
    Property IsChecked As Boolean?

    ReadOnly Property IsNotCheckedOrIndeterminate As Boolean
    ReadOnly Property IsCheckedOrIndeterminate As Boolean
    ReadOnly Property IsCheckedTrue As Boolean
    ReadOnly Property IsCheckedFalse As Boolean
    ''' <summary>
    ''' Gets the name of this item.
    ''' </summary>
    ReadOnly Property Name As String

    ''' <summary>
    ''' Gets the current parent of this item.
    ''' </summary>
    Property Parent As IOlFolderViewModel

    ReadOnly Property IsEnabled As Boolean

    Property OutlookFolder As MAPIFolder

End Interface
