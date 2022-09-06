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
        Select Case CStr(Parameter)
            Case Localize.LocOptionen.strAnrMon
                ' Farbdefinition für den Anrufmonitor löschen.
                OptVM.TBAnrMonBColor = Nothing
                OptVM.TBAnrMonFColor = Nothing

            Case Localize.LocOptionen.strStoppuhr
                ' Farbdefinition für die Stoppuhr löschen.
                OptVM.TBStoppUhrBColor = Nothing
                OptVM.TBStoppUhrFColor = Nothing

            Case Localize.LocOptionen.strVIP
                ' Farbdefinition für die VIP löschen.
                OptVM.TBVIPBColor = Nothing
                OptVM.TBVIPBColor = Nothing

            Case Else
                ' Farbdefinition für die Telefonnummern löschen
                For Each TelNr In OptVM.TelNrListe.Where(Function(T) T.Einwahl = CStr(Parameter))
                    ' Sollte nur eine sein

                    TelNr.EigeneNummerInfo.TBBackgoundColor = Nothing
                    TelNr.EigeneNummerInfo.TBForegoundColor = Nothing

                Next

        End Select

    End Sub
End Class
