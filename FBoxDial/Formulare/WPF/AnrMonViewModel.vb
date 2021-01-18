Imports System.Windows.Media.Imaging
Imports Microsoft.Office.Interop
Public Class AnrMonViewModel
    Inherits NotifyBase

    Private _Zeit As Date '{Binding ZeitBeginn, Mode=OneWay, StringFormat=\{0:F\}}
    Public Property Zeit As Date
        Get
            Return _Zeit
        End Get
        Set
            SetProperty(_Zeit, Value)
        End Set
    End Property

    Private _AnrMonTelName As String
    Public Property AnrMonTelName As String
        Get
            Return _AnrMonTelName
        End Get
        Set
            SetProperty(_AnrMonTelName, Value)
        End Set
    End Property

    Private _AnrMonTelNr As String
    Public Property AnrMonTelNr As String
        Get
            Return _AnrMonTelNr
        End Get
        Set
            SetProperty(_AnrMonTelNr, Value)
        End Set
    End Property

    Private _AnrMonAnrufer As String
    Public Property AnrMonAnrufer As String
        Get
            Return _AnrMonAnrufer
        End Get
        Set
            SetProperty(_AnrMonAnrufer, Value)
        End Set
    End Property

    Private _AnrMonFirma As String
    Public Property AnrMonFirma As String
        Get
            Return _AnrMonFirma
        End Get
        Set
            SetProperty(_AnrMonFirma, Value)
        End Set
    End Property

    Private _AnrMonClipboard As String
    Public Property AnrMonClipboard As String
        Get
            Return _AnrMonClipboard
        End Get
        Set
            SetProperty(_AnrMonClipboard, Value)
        End Set
    End Property

    Private _OKontakt As Outlook.ContactItem
    Public Property OKontakt As Outlook.ContactItem
        Get
            Return _OKontakt
        End Get
        Set
            SetProperty(_OKontakt, Value)
        End Set
    End Property

    'Private _OExchangeNutzer As Outlook.ExchangeUser
    'Public Property OExchangeNutzer As Outlook.ExchangeUser
    '    Get
    '        Return _OExchangeNutzer
    '    End Get
    '    Set
    '        SetProperty(_OExchangeNutzer, value)
    '    End Set
    'End Property

    Private _Kontaktbild As BitmapImage
    Public Property Kontaktbild As BitmapImage
        Get
            Return _Kontaktbild
        End Get
        Set
            SetProperty(_Kontaktbild, Value)
        End Set
    End Property
End Class
