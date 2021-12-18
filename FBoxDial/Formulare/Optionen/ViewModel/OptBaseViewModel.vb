Public Class OptBaseViewModel
    Inherits NotifyBase
    Implements IPageViewModel

    Private Property DatenService As IOptionenService

    Private _OptVM As OptionenViewModel
    Public Property OptVM As OptionenViewModel Implements IPageViewModel.OptVM
        Get
            Return _OptVM
        End Get
        Set
            SetProperty(_OptVM, Value)
        End Set
    End Property

    Public ReadOnly Property Name As String Implements IPageViewModel.Name
        Get
            Return Localize.LocOptionen.strBaseConfig
        End Get
    End Property

    Public Property InitialSelected As Boolean = True Implements IPageViewModel.InitialSelected

    Public Property UpdateUserListCommand As RelayCommand

    Public Sub New()
        ' Commands
        UpdateUserListCommand = New RelayCommand(AddressOf UpdateUserList, AddressOf CanUpdateUserList)

        ' Interface
        DatenService = New OptionenService
    End Sub

    Private Function CanUpdateUserList(o As Object) As Boolean
        Return OptVM IsNot Nothing AndAlso OptVM.TBFBAdr.IsNotStringNothingOrEmpty
    End Function

    Private Sub UpdateUserList(o As Object)
        With OptVM
            ' Merke dir den alten Benutzernamen
            Dim OldUser As String = .TBBenutzer
            ' Lade die aktuellen Nutzernamen herunter
            .CBoxBenutzer = DatenService.LadeFBoxUser(ValidIP(.TBFBAdr))
            ' Prüfe, ob es einen Benutzer mit dem alten namen gibt
            If .CBoxBenutzer.Where(Function(User) User.UserName.IsEqual(OldUser)).Any Then
                ' Setze den alten User als den neuen
                .TBBenutzer = OldUser
            End If

        End With
    End Sub
End Class
