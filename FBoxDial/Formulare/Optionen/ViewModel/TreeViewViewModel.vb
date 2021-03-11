Imports Microsoft.Office.Interop.Outlook
' TODO: Dies ist das ViewModel!
Public Class TreeViewViewModel
    Inherits NotifyBase

#Region "Eigenschaften"
    Private _OutlookFolder As MAPIFolder
    Friend Property OutlookFolder As MAPIFolder
        Get
            Return _OutlookFolder
        End Get
        Set
            SetProperty(_OutlookFolder, Value)
        End Set
    End Property

    Private _OutlookItemType As OlItemType
    Friend Property OutlookItemType As OlItemType
        Get
            Return _OutlookItemType
        End Get
        Set
            SetProperty(_OutlookItemType, Value)
        End Set
    End Property

    Private _Unterordner As New ObservableCollectionEx(Of TreeViewViewModel)
    Public Property Unterordner As ObservableCollectionEx(Of TreeViewViewModel)
        Get
            Return _Unterordner
        End Get
        Set
            SetProperty(_Unterordner, Value)
        End Set
    End Property

    Private _Überwacht As Boolean

    Public Property Überwacht As Boolean
        Get
            Return _Überwacht
        End Get
        Set
            SetProperty(_Überwacht, Value)
        End Set
    End Property

    Public ReadOnly Property IsItemFolder As Boolean
        Get
            Return OutlookItemType = TreeViewSelectionOutlookItemType
        End Get
    End Property

    Private _TreeViewSelectionOutlookItemType As OlItemType
    Friend Property TreeViewSelectionOutlookItemType As OlItemType
        Get
            Return _TreeViewSelectionOutlookItemType
        End Get
        Set
            _TreeViewSelectionOutlookItemType = Value
        End Set
    End Property

    Private _Title As String
    Public Property Title As String
        Get
            Return _Title
        End Get
        Set
            _Title = Value
        End Set
    End Property
#End Region

End Class
