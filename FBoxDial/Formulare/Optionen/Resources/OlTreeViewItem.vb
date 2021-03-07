Imports Microsoft.Office.Interop
' TODO: Dies ist das ViewModel!
Public Class OlTreeViewItem
    Inherits NotifyBase

#Region "Eigenschaften Outlook"
    Friend Property OutlookFolder As Outlook.MAPIFolder
    Friend Property OutlookItemType As Outlook.OlItemType
#End Region
    Public ReadOnly Property IsItemFolder As Boolean
        Get
            Return OutlookItemType = TreeViewSelectionOutlookItemType
        End Get
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

    Friend Property TreeViewSelectionOutlookItemType As Outlook.OlItemType
    Public Property Title As String

    Private _Unterordner As New ObservableCollectionEx(Of OlTreeViewItem)
    Public Property Unterordner As ObservableCollectionEx(Of OlTreeViewItem)
        Get
            Return _Unterordner
        End Get
        Set
            SetProperty(_Unterordner, Value)
        End Set
    End Property
End Class
