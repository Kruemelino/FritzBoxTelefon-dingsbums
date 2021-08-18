Imports System.Windows.Media.Imaging
Imports System.Windows.Threading
Imports Microsoft.Office.Interop

Public Class ContactDialViewModel
    Inherits NotifyBase
    Implements IViewModelBase
    Private Property DatenService As IDialService
    Friend Property Instance As Dispatcher Implements IViewModelBase.Instance
#Region "Eigenschaften"

    Private _DialVM As WählClientViewModel
    Public Property DialVM As WählClientViewModel
        Get
            Return _DialVM
        End Get
        Set
            SetProperty(_DialVM, Value)
        End Set
    End Property

    Private _DialNumberList As New ObservableCollectionEx(Of Telefonnummer)
    Public Property DialNumberList As ObservableCollectionEx(Of Telefonnummer)
        Get
            Return _DialNumberList
        End Get
        Set
            SetProperty(_DialNumberList, Value)
        End Set
    End Property

    Private _Kontaktbild As BitmapImage
    Public Property Kontaktbild As BitmapImage
        Get
            Return _Kontaktbild
        End Get
        Set
            SetProperty(_Kontaktbild, Value)
        End Set
    End Property

    Private _OKontakt As Outlook.ContactItem
    Public Property OKontakt As Outlook.ContactItem
        Get
            Return _OKontakt
        End Get
        Set
            SetProperty(_OKontakt, Value)

            SetData(_OKontakt)
        End Set
    End Property

    Private _OExchangeNutzer As Outlook.ExchangeUser
    Public Property OExchangeNutzer As Outlook.ExchangeUser
        Get
            Return _OExchangeNutzer
        End Get
        Set
            SetProperty(_OExchangeNutzer, Value)

            SetData(_OExchangeNutzer)
        End Set
    End Property

    Private _FBoxXMLKontakt As FritzBoxXMLKontakt
    Public Property FBoxXMLKontakt As FritzBoxXMLKontakt
        Get
            Return _FBoxXMLKontakt
        End Get
        Set
            SetProperty(_FBoxXMLKontakt, Value)

            SetData(_FBoxXMLKontakt)
        End Set
    End Property

    Public ReadOnly Property IsVIP As Boolean
        Get
            Return OKontakt IsNot Nothing AndAlso OKontakt.IsVIP
        End Get
    End Property

    Public ReadOnly Property VIPEnabled As Boolean
        Get
            Return OKontakt IsNot Nothing
        End Get
    End Property
    Public ReadOnly Property ZeigeBild As Boolean
        Get
            Return XMLData.POptionen.CBAnrMonContactImage And Kontaktbild IsNot Nothing
        End Get
    End Property
#End Region

#Region "ICommand"
    Public Property ShowContactCommand As RelayCommand
    Public Property VIPCommand As RelayCommand
#End Region

    Public Sub New(WählclientVM As WählClientViewModel, DS As IDialService, i As Dispatcher)
        DialVM = WählclientVM
        DatenService = DS
        Instance = i
        ' Init Command

        ShowContactCommand = New RelayCommand(AddressOf ShowContact, AddressOf CanShow)
        VIPCommand = New RelayCommand(AddressOf ToggleVIP)

    End Sub

    Private Sub SetData(olKontakt As Outlook.ContactItem)

        With olKontakt

            ' Telefonnummern des Kontaktes setzen 
            DialNumberList.AddRange(.GetKontaktTelNrList)

            ' Kopfdaten setzen
            DialVM.Name = String.Format(Localize.LocWählclient.strHeader, $"{ .FullName}{If(.CompanyName.IsNotStringEmpty, $" ({ .CompanyName})", DfltStringEmpty)}")

            ' Kontaktbild anzeigen

            ' Setze das Kontaktbild
            Instance.Invoke(Sub() Kontaktbild = olKontakt.KontaktBildEx)

        End With
    End Sub

    Private Sub SetData(olExchangeUser As Outlook.ExchangeUser)
        With olExchangeUser

            ' Telefonnummern des Kontaktes setzen 
            DialNumberList.AddRange(.GetKontaktTelNrList)

            ' Kopfdaten setzen
            DialVM.Name = String.Format(Localize.LocWählclient.strHeader, $"{ .Name}{If(.CompanyName.IsNotStringEmpty, $" ({ .CompanyName})", DfltStringEmpty)}")
        End With
    End Sub

    Private Sub SetData(FBoxXMLKontakt As FritzBoxXMLKontakt)
        With FBoxXMLKontakt

            ' Telefonnummern des Kontaktes setzen 
            DialNumberList.AddRange(.GetKontaktTelNrList)

            ' Kopfdaten setzen
            DialVM.Name = String.Format(Localize.LocWählclient.strHeader, $"{ .Person.RealName}")

            ' Kontaktbild anzeigen
            Instance.Invoke(Async Function()
                                Kontaktbild = Await FBoxXMLKontakt.KontaktBildEx
                                OnPropertyChanged(NameOf(ZeigeBild))
                            End Function)

        End With
    End Sub

#Region "ICommand Callback"
    Private Function CanShow(obj As Object) As Boolean
        ' Nur für Outlook Kontakte und ExchangeUser
        Return OKontakt IsNot Nothing Or OExchangeNutzer IsNot Nothing
    End Function


    Private Sub ShowContact(o As Object)
        ' Outlook Kontakt anzeigen
        If OKontakt IsNot Nothing Then OKontakt.Display()
        ' Outlook ExchangeUser anzeigen
        If OExchangeNutzer IsNot Nothing Then OExchangeNutzer.Details()
    End Sub

    Private Sub ToggleVIP(o As Object)
        OKontakt?.ToggleVIP()

        OnPropertyChanged(NameOf(IsVIP))
    End Sub

#End Region
End Class
