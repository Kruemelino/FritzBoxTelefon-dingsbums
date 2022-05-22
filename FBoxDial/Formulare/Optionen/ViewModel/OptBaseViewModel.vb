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
    Public Property DesignTestFormCommand As RelayCommand
    Public Property ToogleDesignCommand As RelayCommand

    Public Sub New(ds As IOptionenService)
        ' Commands
        UpdateUserListCommand = New RelayCommand(AddressOf UpdateUserList, AddressOf CanUpdateUserList)
        DesignTestFormCommand = New RelayCommand(AddressOf ShowTestDesignForm)
        ToogleDesignCommand = New RelayCommand(AddressOf ToogleDesign)

        ' Interface
        _DatenService = ds
    End Sub

    Private Function CanUpdateUserList(o As Object) As Boolean
        Return OptVM IsNot Nothing AndAlso OptVM.TBFBAdr.IsNotStringNothingOrEmpty
    End Function

    Private Sub UpdateUserList(o As Object)
        With OptVM
            ' Merke dir den alten Benutzernamen
            Dim OldUser As String = .TBBenutzer
            ' Lade die aktuellen Nutzernamen herunter
            .CBoxBenutzer = DatenService.LadeFBoxUser()
            ' Prüfe, ob es einen Benutzer mit dem alten namen gibt
            If .CBoxBenutzer.Where(Function(User) User.UserName.IsEqual(OldUser)).Any Then
                ' Setze den alten User als den neuen
                .TBBenutzer = OldUser
            End If

        End With
    End Sub

    Private Sub ShowTestDesignForm(o As Object)
        DatenService.ShowDesignTest()
    End Sub

    Private Sub ToogleDesign(o As Object)
        DatenService.ToogleDesign()
    End Sub
End Class
