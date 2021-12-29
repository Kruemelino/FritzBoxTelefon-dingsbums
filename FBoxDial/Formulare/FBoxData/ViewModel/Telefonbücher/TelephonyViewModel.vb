Public Class TelephonyViewModel
    Inherits NotifyBase

#Region "Models"
    Public Property Telefonie As FBoxAPI.Telephony
#End Region

    Public Property Emails As ObservableCollectionEx(Of EMailViewModel)
    Public Property Nummern As ObservableCollectionEx(Of NumberViewModel)


    Public Sub New(telephony As FBoxAPI.Telephony)
        _Telefonie = telephony
        ' Setze Felder
        Emails = New ObservableCollectionEx(Of EMailViewModel)(Telefonie.Emails.Select(Function(e) New EMailViewModel(e)))
        Nummern = New ObservableCollectionEx(Of NumberViewModel)(Telefonie.Numbers.Select(Function(n) New NumberViewModel(n)))
    End Sub
End Class
