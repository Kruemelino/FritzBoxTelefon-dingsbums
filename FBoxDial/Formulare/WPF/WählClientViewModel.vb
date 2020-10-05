Imports System.Windows.Media.Imaging
Imports Microsoft.Office.Interop

Public Class WählClientViewModel
    Inherits NotifyBase

    Private _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set(value As String)
            SetProperty(_Name, value)
        End Set
    End Property

    Private _CLIR As Boolean
    Public Property CLIR As Boolean
        Get
            Return _CLIR
        End Get
        Set(value As Boolean)
            SetProperty(_CLIR, value)
        End Set
    End Property

    Private _OKontakt As Outlook.ContactItem
    Public Property OKontakt As Outlook.ContactItem
        Get
            Return _OKontakt
        End Get
        Set(value As Outlook.ContactItem)
            SetProperty(_OKontakt, value)
        End Set
    End Property

    Private _OExchangeNutzer As Outlook.ExchangeUser
    Public Property OExchangeNutzer As Outlook.ExchangeUser
        Get
            Return _OExchangeNutzer
        End Get
        Set(value As Outlook.ExchangeUser)
            SetProperty(_OExchangeNutzer, value)
        End Set
    End Property

    Private _TelNr As Telefonnummer
    Public Property TelNr As Telefonnummer
        Get
            Return _TelNr
        End Get
        Set(value As Telefonnummer)
            SetProperty(_TelNr, value)
        End Set
    End Property

    ''' <summary>
    ''' Returns Or sets a list as Telefonnummern             
    ''' </summary>
    Private _DialNumberList As New ObservableCollectionEx(Of Telefonnummer)
    Public Property DialNumberList As ObservableCollectionEx(Of Telefonnummer)
        Get
            Return _DialNumberList
        End Get
        Set(value As ObservableCollectionEx(Of Telefonnummer))
            SetProperty(_DialNumberList, value)
        End Set
    End Property

    Private _DialDeviceList As New ObservableCollectionEx(Of Telefoniegerät)
    Public Property DialDeviceList As ObservableCollectionEx(Of Telefoniegerät)
        Get
            Return _DialDeviceList
        End Get
        Set(value As ObservableCollectionEx(Of Telefoniegerät))
            SetProperty(_DialDeviceList, value)
        End Set
    End Property

    Private _StdGerät As Telefoniegerät
    Public Property StdGerät As Telefoniegerät
        Get
            Return _StdGerät
        End Get
        Set(value As Telefoniegerät)
            SetProperty(_StdGerät, value)
        End Set
    End Property

    Private _Kontaktbild As BitmapImage
    Public Property Kontaktbild As BitmapImage
        Get
            Return _Kontaktbild
        End Get
        Set(value As BitmapImage)
            SetProperty(_Kontaktbild, value)
        End Set
    End Property
End Class


