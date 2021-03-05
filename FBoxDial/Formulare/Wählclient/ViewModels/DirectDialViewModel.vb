Public Class DirectDialViewModel
    Inherits NotifyBase
    Private Property DatenService As IDialService

    Private _DialDirektWahlList As New ObservableCollectionEx(Of Telefonnummer)
    Public Property DialDirektWahlList As ObservableCollectionEx(Of Telefonnummer)
        Get
            Return _DialDirektWahlList
        End Get
        Set
            SetProperty(_DialDirektWahlList, Value)
        End Set
    End Property

    Private _DialVM As WählClientViewModel
    Public Property DialVM As WählClientViewModel
        Get
            Return _DialVM
        End Get
        Set
            SetProperty(_DialVM, Value)
        End Set
    End Property

    Private _TelNr As Telefonnummer
    Public Property TelNr As Telefonnummer
        Get
            Return _TelNr
        End Get
        Set
            SetProperty(_TelNr, Value)
        End Set
    End Property

    Public Sub New(WählclientVM As WählClientViewModel, DS As IDialService)
        DialVM = WählclientVM

        DatenService = DS

        SetData()
    End Sub

    Private Sub SetData()
        DialDirektWahlList = New ObservableCollectionEx(Of Telefonnummer)
        DialDirektWahlList.AddRange(DatenService.GetLastTelNr)

        If TelNr Is Nothing Then DialVM.Name = Localize.LocWählclient.strDirect
    End Sub
End Class
