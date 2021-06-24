Imports Microsoft.Office.Interop.Outlook

''' <summary>
''' In Anlehnung an Dirk Bahle: <code>https://www.codeproject.com/Articles/1224943/Advanced-WPF-TreeView-in-Csharp-VB-Net-Part-of-n</code>
''' </summary>
Public Class OlFolderViewModel
    Inherits NotifyBase
    Implements IOlFolderViewModel

#Region "Eigenschaften"
    Friend Property OutlookFolder As MAPIFolder Implements IOlFolderViewModel.OutlookFolder
    Private Property TargetFolderType As OlItemType

    Private _Folders As ObservableCollectionEx(Of IOlFolderViewModel)
    ''' <summary>
    ''' Lädt die Child-<see cref="MAPIFolder"/> dieses Ordners 
    ''' </summary>
    Public ReadOnly Property Folders As IEnumerable(Of IOlFolderViewModel) Implements IOlFolderViewModel.ChildFolders
        Get
            If _Folders Is Nothing Then

                _Folders = New ObservableCollectionEx(Of IOlFolderViewModel)

                _Folders.AddRange(From Folder In OutlookFolder.Folders Select New OlFolderViewModel(Me, CType(Folder, MAPIFolder), TargetFolderType))

            End If

            Return _Folders
        End Get
    End Property

    ''' <summary>
    ''' Gets the number of children stored below this item.
    ''' </summary>
    Public ReadOnly Property ChildrenCount As Integer Implements IOlFolderViewModel.ChildFolderCount
        Get
            Return OutlookFolder.Folders.Count
        End Get
    End Property

    ''' <summary>
    ''' Gets the name of this item.
    ''' </summary>
    Public ReadOnly Property Name As String Implements IOlFolderViewModel.Name
        Get
            Return OutlookFolder.Name
        End Get
    End Property

    Private _isChecked As Boolean? = False
    ''' <summary>
    ''' Gets/sets the state of the associated UI toggle (ex. CheckBox).
    ''' 
    ''' The return value is calculated based on the check state of all
    ''' child IOlFolderViewModel. Setting this property to true or false
    ''' will set all children to the same check state, and setting it 
    ''' to any value will cause the parent to verify its check state.
    ''' </summary>
    Public Property IsChecked As Boolean? Implements IOlFolderViewModel.IsChecked
        Get
            Return _isChecked
        End Get
        Set
            SetProperty(_isChecked, Value)
        End Set
    End Property

    Public ReadOnly Property IsEnabled As Boolean Implements IOlFolderViewModel.IsEnabled
        Get
            Return OutlookFolder.DefaultItemType = TargetFolderType
        End Get
    End Property

    Private ReadOnly _parent As IOlFolderViewModel
    ''' <summary>
    ''' Gets or sets the Parent of this item
    ''' (Or null if there Is no parent - are you looking at a root item?)
    ''' </summary>
    Public Property Parent As IOlFolderViewModel Implements IOlFolderViewModel.Parent
        Get
            Return _parent
        End Get
        Set
            SetProperty(Value, _parent)
        End Set
    End Property

#End Region
    Public Sub New(ParentVM As IOlFolderViewModel, OlFolder As MAPIFolder, olItemType As OlItemType)
        _parent = ParentVM

        TargetFolderType = olItemType

        OutlookFolder = OlFolder

    End Sub

    Public Sub New(OlFolder As MAPIFolder, olItemType As OlItemType)

        OutlookFolder = OlFolder

        TargetFolderType = olItemType
    End Sub

End Class

