Public Class OptTestViewModel
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
            Return Localize.LocOptionen.strTest
        End Get
    End Property

#Region "ICommand"
    Public Property TestTelNrCommand As RelayCommand
#End Region
    Public Sub New()
        ' Commands
        TestTelNrCommand = New RelayCommand(AddressOf StartImport)
    End Sub

    Private Sub StartImport(o As Object)
        TestTelNr.SetNummer = _TBTestTelNrInput

        OnPropertyChanged(NameOf(TBTestTelNrUnformatiert))
        OnPropertyChanged(NameOf(TBTestTelNrLKZ))
        OnPropertyChanged(NameOf(TBTestTelNrLKZID))
        OnPropertyChanged(NameOf(TBTestTelNrLKZName))
        OnPropertyChanged(NameOf(TBTestTelNrONKZ))
        OnPropertyChanged(NameOf(TBTestTelNrONKZName))
        OnPropertyChanged(NameOf(TBTestTelNrEinwahl))
        OnPropertyChanged(NameOf(TBTestTelNrDurchwahl))
        OnPropertyChanged(NameOf(TBTestTelNrFormatiert))
    End Sub

#Region "Telefonnummerntest"
    Private Property TestTelNr As New Telefonnummer

    Private _TBTestTelNrInput As String
    Public Property TBTestTelNrInput As String
        Get
            Return _TBTestTelNrInput
        End Get
        Set
            SetProperty(_TBTestTelNrInput, Value)
        End Set
    End Property

    Public ReadOnly Property TBTestTelNrUnformatiert As String
        Get
            Return TestTelNr.Unformatiert
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrLKZ As String
        Get
            Return TestTelNr.Landeskennzahl
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrLKZID As String
        Get
            Return TestTelNr.AreaCode
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrLKZName As String
        Get
            If TestTelNr.AreaCode.IsNotStringNothingOrEmpty Then
                Return Localize.Länder.ResourceManager.GetString(TestTelNr.AreaCode)
            Else
                Return DfltStringEmpty
            End If
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrONKZ As String
        Get
            Return TestTelNr.Ortskennzahl
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrONKZName As String
        Get
            Return TestTelNr.Location
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrEinwahl As String
        Get
            Return TestTelNr.Einwahl
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrDurchwahl As String
        Get
            Return TestTelNr.Durchwahl
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrFormatiert As String
        Get
            Return TestTelNr.Formatiert
        End Get
    End Property
#End Region
End Class
