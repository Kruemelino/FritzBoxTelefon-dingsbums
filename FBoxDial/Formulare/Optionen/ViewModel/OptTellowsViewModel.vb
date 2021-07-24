Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Public Class OptTellowsViewModel
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
            Return Localize.LocOptionen.strTellows
        End Get
    End Property

    Public Property InitialSelected As Boolean = False Implements IPageViewModel.InitialSelected
#Region "tellows Test"
    Private _TBTestTellowsInput As String
    Public Property TBTestTellowsInput As String
        Get
            Return _TBTestTellowsInput
        End Get
        Set
            SetProperty(_TBTestTellowsInput, Value)
        End Set
    End Property

    Private _TellowsResponse As TellowsResponse
    Public Property TellowsResponse As TellowsResponse
        Get
            Return _TellowsResponse
        End Get
        Set
            SetProperty(_TellowsResponse, Value)
        End Set
    End Property

    Private _TellowsPartnerInfo As TellowsPartnerInfo
    Public Property TellowsPartnerInfo As TellowsPartnerInfo
        Get
            Return _TellowsPartnerInfo
        End Get
        Set
            SetProperty(_TellowsPartnerInfo, Value)
        End Set
    End Property

    Private _ImageData As ImageSource
    Public Property ImageData As ImageSource
        Get
            Return _ImageData
        End Get
        Set
            SetProperty(_ImageData, Value)
        End Set
    End Property
#End Region

    Public Property TellowsAccountInfoCommand As RelayCommand
    Public Property TellowsLiveAPICommand As RelayCommand
    Public Sub New()

        TellowsLiveAPICommand = New RelayCommand(AddressOf StartLiveAPI, AddressOf CanUsetellows)
        TellowsAccountInfoCommand = New RelayCommand(AddressOf LadeAccountDaten, AddressOf CanUsetellows)

        ' Interface
        DatenService = New OptionenService
    End Sub

    Private Function CanUsetellows(o As Object) As Boolean
        Return OptVM.TBTellowsAPIKey.IsNotStringNothingOrEmpty
    End Function

#Region "Account Daten"

    Private Async Sub LadeAccountDaten(o As Object)
        Using Crypter As New Rijndael
            With Crypter
                TellowsPartnerInfo = Await DatenService.GetTellowsAccountData(.SecureStringToMD5(.DecryptString(OptVM.TBTellowsAPIKey, DfltTellowsDeCryptKey), Encoding.Default))
            End With
        End Using
    End Sub
#End Region

#Region "LiveAPI"
    Private Async Sub StartLiveAPI(o As Object)

        Using Crypter As New Rijndael
            With Crypter
                ' Setze Ergebnis
                TellowsResponse = Await DatenService.GetTellowsLiveAPIData(TBTestTellowsInput, .SecureStringToMD5(.DecryptString(OptVM.TBTellowsAPIKey, DfltTellowsDeCryptKey), Encoding.Default))
            End With
        End Using

        ' Lade Bild
        If TellowsResponse IsNot Nothing AndAlso TellowsResponse.Score.IsInRange(1, 9) Then
            ImageData = New BitmapImage(New Uri($"pack://application:,,,/{My.Resources.strDefLongName};component/Tellows/Resources/score{TellowsResponse.Score}.png", UriKind.Absolute))
        End If
    End Sub
#End Region
End Class
