Public Class OptNumberViewModel
    Inherits NotifyBase

    Private Property DatenService As IOptionenService
    Private Property DialogService As IDialogService

    Public Sub New(dataService As IOptionenService, dialogService As IDialogService, sipNr As FBoxAPI.SIPTelNr)
        ' Interface
        _DatenService = dataService
        _DialogService = dialogService
        ' Commands

        ' Model
        SIPTelNr = sipNr
    End Sub
#Region "Model IPPhone"
    Public Property SIPTelNr As FBoxAPI.SIPTelNr
#End Region

    ReadOnly Property Type As TypeEnumSIP
        Get
            Return CType(SIPTelNr?.Type, TypeEnumSIP)
        End Get
    End Property
End Class
