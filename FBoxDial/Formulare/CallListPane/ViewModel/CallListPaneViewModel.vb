Public Class CallListPaneViewModel
    Inherits NotifyBase
    Private Property DatenService As IAnrMonService
    Public Sub New(dataService As IAnrMonService)
        ' Interface
        _DatenService = dataService
    End Sub

#Region "Felder"
    Public Property MissedCallList As New ObservableCollectionEx(Of MissedCallViewModel)

    Private _MissedCall As MissedCallViewModel
    Public Property MissedCall As MissedCallViewModel
        Get
            Return _MissedCall
        End Get
        Set
            SetProperty(_MissedCall, Value)
        End Set
    End Property
#End Region

    Friend Sub RemoveMissedCall(CallVM As MissedCallViewModel)
        DatenService.RemoveMissedCall(CallVM)
    End Sub

End Class
