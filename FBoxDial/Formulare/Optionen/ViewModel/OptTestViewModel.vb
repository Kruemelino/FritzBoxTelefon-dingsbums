Public Class OptTestViewModel
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
            Return Localize.LocOptionen.strTest
        End Get
    End Property

#Region "ICommand"
    Public Property TestTelNrCommand As RelayCommand
    Public Property TestRWSCommand As RelayCommand
    Public Property TestUserListCommand As RelayCommand
    Public Property TestLoginCommand As RelayCommand

#End Region
    Public Sub New()
        ' Commands
        TestTelNrCommand = New RelayCommand(AddressOf StartTelNrTest)
        TestRWSCommand = New RelayCommand(AddressOf StartRWSTest, AddressOf CanRunTestRWS)
        TestUserListCommand = New RelayCommand(AddressOf StartLoadUserListTest, AddressOf CanLoadUserList)
        TestLoginCommand = New RelayCommand(AddressOf StartLoginTest, AddressOf CanStartLoginTest)
        ' Interface
        DatenService = New OptionenService
    End Sub

    Private Sub StartTelNrTest(o As Object)
        TestTelNr = New Telefonnummer With {.SetNummer = TBTestTelNrInput}

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

    Private Sub StartRWSTest(o As Object)
        ' Vorheriges Ergebnis löschen
        TBTestRWSOutput = DfltStringEmpty

        ' Ereignishandler hinzufügen
        AddHandler DatenService.Status, AddressOf RWSTestStatus
        AddHandler DatenService.BeendetRWS, AddressOf RWSTestBeendet
        ' RWS Test Starten
        DatenService.StartRWSTest(TBTestRWSInput)
    End Sub

#Region "Telefonnummerntest"
    Private Property TestTelNr As Telefonnummer

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
            Return TestTelNr?.Unformatiert
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrLKZ As String
        Get
            Return TestTelNr?.Landeskennzahl
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrLKZID As String
        Get
            Return TestTelNr?.AreaCode
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrLKZName As String
        Get
            If TestTelNr?.AreaCode.IsNotStringNothingOrEmpty Then
                Return Localize.Länder.ResourceManager.GetString(TestTelNr.AreaCode)
            Else
                Return DfltStringEmpty
            End If
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrONKZ As String
        Get
            Return TestTelNr?.Ortskennzahl
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrONKZName As String
        Get
            Return TestTelNr?.Location
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrEinwahl As String
        Get
            Return TestTelNr?.Einwahl
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrDurchwahl As String
        Get
            Return TestTelNr?.Durchwahl
        End Get
    End Property

    Public ReadOnly Property TBTestTelNrFormatiert As String
        Get
            Return TestTelNr?.Formatiert
        End Get
    End Property
#End Region

#Region "Test der Rückwärtssuche"
    Private _TBTestRWSInput As String
    Public Property TBTestRWSInput As String
        Get
            Return _TBTestRWSInput
        End Get
        Set
            SetProperty(_TBTestRWSInput, Value)
        End Set
    End Property

    Private _TBTestRWSOutput As String
    Public Property TBTestRWSOutput As String
        Get
            Return _TBTestRWSOutput
        End Get
        Set
            SetProperty(_TBTestRWSOutput, Value)
        End Set
    End Property

    Private Function CanRunTestRWS(obj As Object) As Boolean
        Return TBTestRWSInput.IsNotStringNothingOrEmpty
    End Function

    Private Sub RWSTestStatus(sender As Object, e As NotifyEventArgs(Of String))
        TBTestRWSOutput += e.Value & Environment.NewLine
    End Sub
    Private Sub RWSTestBeendet(sender As Object, e As NotifyEventArgs(Of Boolean))
        ' Ereignishandler hinzufügen
        RemoveHandler DatenService.Status, AddressOf RWSTestStatus
        RemoveHandler DatenService.BeendetRWS, AddressOf RWSTestBeendet

        ' Finales Ergebnis schreiben
        If Not e.Value Then
            TBTestRWSOutput += Environment.NewLine & String.Format(Localize.LocOptionen.strTestRWSNoResult, TBTestRWSInput)
        End If
    End Sub
#End Region

#Region "Test Login"
    Private _CBoxBenutzer As ObservableCollectionEx(Of FritzBoxXMLUser)
    Public Property CBoxBenutzer As ObservableCollectionEx(Of FritzBoxXMLUser)
        Get
            Return _CBoxBenutzer
        End Get
        Set
            SetProperty(_CBoxBenutzer, Value)
        End Set
    End Property

    Private _TBFBAdr As String
    Public Property TBFBAdr As String
        Get
            Return _TBFBAdr
        End Get
        Set
            SetProperty(_TBFBAdr, Value)
        End Set
    End Property

    Private _TBBenutzer As String
    Public Property TBBenutzer As String
        Get
            Return _TBBenutzer
        End Get
        Set
            SetProperty(_TBBenutzer, Value)
        End Set
    End Property

    Private _TBTestLoginOutput As String
    Public Property TBTestLoginOutput As String
        Get
            Return _TBTestLoginOutput
        End Get
        Set
            SetProperty(_TBTestLoginOutput, Value)
        End Set
    End Property

    Private Function CanLoadUserList(o As Object) As Boolean
        Return TBFBAdr.IsNotStringNothingOrEmpty
    End Function

    Private Function CanStartLoginTest(o As Object) As Boolean
        Return TBBenutzer.IsNotStringNothingOrEmpty
    End Function

    Private Sub StartLoadUserListTest(o As Object)
        ' Vorheriges Ergebnis löschen
        TBTestLoginOutput = DfltStringEmpty

        ' Ereignishandler hinzufügen
        AddHandler DatenService.Status, AddressOf LoginTestStatus
        AddHandler DatenService.BeendetLogin, AddressOf LoginTestBeendet
        ' Lade die aktuellen Nutzernamen herunter
        CBoxBenutzer = DatenService.LadeFBoxUser(ValidIP(TBFBAdr))

    End Sub

    Private Sub StartLoginTest(o As Object)
        ' Ereignishandler hinzufügen
        AddHandler DatenService.Status, AddressOf LoginTestStatus
        AddHandler DatenService.BeendetLogin, AddressOf LoginTestBeendet

        DatenService.StartLoginTest(ValidIP(TBFBAdr), TBBenutzer, CType(o, Windows.Controls.PasswordBox).SecurePassword)
    End Sub
    Private Sub LoginTestStatus(sender As Object, e As NotifyEventArgs(Of String))
        TBTestLoginOutput += e.Value & Environment.NewLine
    End Sub

    Private Sub LoginTestBeendet(sender As Object, e As NotifyEventArgs(Of Boolean))
        ' Ereignishandler entfernen
        RemoveHandler DatenService.Status, AddressOf LoginTestStatus
        RemoveHandler DatenService.BeendetLogin, AddressOf LoginTestBeendet
    End Sub

#End Region
End Class
