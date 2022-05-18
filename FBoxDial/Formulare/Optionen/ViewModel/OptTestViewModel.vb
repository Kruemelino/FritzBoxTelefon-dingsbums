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

    Public Property InitialSelected As Boolean = False Implements IPageViewModel.InitialSelected

#Region "ICommand"
    Public Property TestTelNrCommand As RelayCommand
    Public Property TestRWSCommand As RelayCommand
    Public Property TestUserListCommand As RelayCommand
    Public Property TestLoginCommand As RelayCommand
    Public Property TestKontaktsucheCommand As RelayCommand
    Public Property TestAnrMonCommand As RelayCommand
#End Region
    Public Sub New(ds As IOptionenService)
        ' Commands
        TestTelNrCommand = New RelayCommand(AddressOf StartTelNrTest)
        TestRWSCommand = New RelayCommand(AddressOf StartRWSTest, AddressOf CanRunTestRWS)
        TestKontaktsucheCommand = New RelayCommand(AddressOf StartKontaktsucheTest, AddressOf CanRunTestKontaktsuche)
        TestAnrMonCommand = New RelayCommand(AddressOf StartAnrMonTest, AddressOf CanRunAnrMonTest)

        ' Interface
        _DatenService = ds
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
                Return String.Empty
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
#End Region

#Region "Test Login"
    Private _CBoxBenutzer As ObservableCollectionEx(Of FBoxAPI.User)
    Public Property CBoxBenutzer As ObservableCollectionEx(Of FBoxAPI.User)
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

#End Region

#Region "Test der Kontaktsuche"
    Private _TBTestKontaktsucheInput As String
    Public Property TBTestKontaktsucheInput As String
        Get
            Return _TBTestKontaktsucheInput
        End Get
        Set
            SetProperty(_TBTestKontaktsucheInput, Value)
        End Set
    End Property

    Private _TBTestKontaktsucheOutput As String
    Public Property TBTestKontaktsucheOutput As String
        Get
            Return _TBTestKontaktsucheOutput
        End Get
        Set
            SetProperty(_TBTestKontaktsucheOutput, Value)
        End Set
    End Property
    Private Function CanRunTestKontaktsuche(o As Object) As Boolean
        Return TBTestKontaktsucheInput.IsNotStringNothingOrEmpty
    End Function

    Private Sub TestKontaktsucheStatus(sender As Object, e As String)
        TBTestKontaktsucheOutput += e & Environment.NewLine
    End Sub

    Private Sub StartKontaktsucheTest(o As Object)
        ' Vorheriges Ergebnis löschen
        TBTestKontaktsucheOutput = String.Empty

        ' Ereignishandler hinzufügen
        AddHandler DatenService.Status, AddressOf TestKontaktsucheStatus
        AddHandler DatenService.BeendetKontaktsuche, AddressOf TestKontaktsucheBeendet

        ' Test der Kontaktsuche
        DatenService.StartKontaktsucheTest(TBTestKontaktsucheInput)
    End Sub

    Private Sub TestKontaktsucheBeendet(sender As Object, e As NotifyEventArgs(Of Boolean))
        ' Ereignishandler hinzufügen
        RemoveHandler DatenService.Status, AddressOf TestKontaktsucheStatus
        RemoveHandler DatenService.BeendetKontaktsuche, AddressOf TestKontaktsucheBeendet

        ' Finales Ergebnis schreiben
        If Not e.Value Then
            TBTestKontaktsucheOutput += Environment.NewLine & String.Format(Localize.LocOptionen.strTestRWSNoResult, TBTestKontaktsucheInput)
        End If
    End Sub
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

    Private Sub RWSTestStatus(sender As Object, e As String)
        TBTestRWSOutput += e & Environment.NewLine
    End Sub

    Private Sub StartRWSTest(o As Object)
        ' Vorheriges Ergebnis löschen
        TBTestRWSOutput = String.Empty

        ' Ereignishandler hinzufügen
        AddHandler DatenService.Status, AddressOf RWSTestStatus
        AddHandler DatenService.BeendetRWS, AddressOf RWSTestBeendet
        ' RWS Test Starten
        DatenService.StartRWSTest(TBTestRWSInput)
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

#Region "Test des Anrufmonitors"

    Private Function CanRunAnrMonTest(obj As Object) As Boolean
        Return DatenService.TelefoniedatenEingelesen
    End Function

    Private _TBTestAnrMonInput As String
    Public Property TBTestAnrMonInput As String
        Get
            Return _TBTestAnrMonInput
        End Get
        Set
            SetProperty(_TBTestAnrMonInput, Value)
        End Set
    End Property

    Private _TBTestAnrMonCONNECT As Boolean
    Public Property TBTestAnrMonCONNECT As Boolean
        Get
            Return _TBTestAnrMonCONNECT
        End Get
        Set
            SetProperty(_TBTestAnrMonCONNECT, Value)
        End Set
    End Property

    Private _RBBRnd As Boolean = True
    Public Property RBBRnd As Boolean
        Get
            Return _RBBRnd
        End Get
        Set
            SetProperty(_RBBRnd, Value)
        End Set
    End Property

    Private _RBBRndOutlook As Boolean = False
    Public Property RBBRndOutlook As Boolean
        Get
            Return _RBBRndOutlook
        End Get
        Set
            SetProperty(_RBBRndOutlook, Value)
        End Set
    End Property

    Private _RBBRndFBox As Boolean = False
    Public Property RBBRndFBox As Boolean
        Get
            Return _RBBRndFBox
        End Get
        Set
            SetProperty(_RBBRndFBox, Value)
        End Set
    End Property

    Private _RBBRndTellows As Boolean = False
    Public Property RBBRndTellows As Boolean
        Get
            Return _RBBRndTellows
        End Get
        Set
            SetProperty(_RBBRndTellows, Value)
        End Set
    End Property

    Private _RBBCLIR As Boolean = False
    Public Property RBBCLIR As Boolean
        Get
            Return _RBBCLIR
        End Get
        Set
            SetProperty(_RBBCLIR, Value)
        End Set
    End Property

    Private _CBoxAnrMonGeräteID As Integer = -1
    Public Property CBoxAnrMonGeräteID As Integer
        Get
            Return _CBoxAnrMonGeräteID
        End Get
        Set
            SetProperty(_CBoxAnrMonGeräteID, Value)
        End Set
    End Property
    Private Sub StartAnrMonTest(obj As Object)
        DatenService.StartAnrMonTest(TBTestAnrMonInput,
                                     TBTestAnrMonCONNECT,
                                     RBBRnd,
                                     RBBRndOutlook,
                                     RBBRndFBox,
                                     RBBRndTellows,
                                     RBBCLIR,
                                     CBoxAnrMonGeräteID)
    End Sub
#End Region
End Class
