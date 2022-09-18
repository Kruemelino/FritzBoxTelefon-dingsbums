Public Class OptAnrMonViewModel
    Inherits NotifyBase
    Implements IPageViewModel

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
            Return Localize.LocOptionen.strAnrMon
        End Get
    End Property

    Public Property InitialSelected As Boolean = False Implements IPageViewModel.InitialSelected

    Public Property ResetColorCommand As RelayCommand

    Public Sub New()
        ResetColorCommand = New RelayCommand(AddressOf ResetColor)
    End Sub

    Private Sub ResetColor(Parameter As Object)

        With CType(Parameter, Farbdefinition)
            .TBBackgoundColor = Nothing
            .TBForegoundColor = Nothing
        End With

    End Sub
End Class
