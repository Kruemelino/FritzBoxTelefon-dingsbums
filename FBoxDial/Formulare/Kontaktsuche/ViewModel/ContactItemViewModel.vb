Imports Microsoft.Office.Interop
Public Class ContactItemViewModel
    Inherits NotifyBase

#Region "Properties"
    Private _OlKontakt As Outlook.ContactItem
    Public Property OlKontakt As Outlook.ContactItem
        Get
            Return _OlKontakt
        End Get
        Set
            SetProperty(_OlKontakt, Value)

            If OlKontakt IsNot Nothing Then
                ' FullName zuweisen
                FullName = OlKontakt.FullName
                ' CompanyName zuweisen
                CompanyName = OlKontakt.CompanyName

            End If
        End Set
    End Property

    Private _FullName As String
    Public Property FullName As String
        Get
            Return _FullName
        End Get
        Set
            SetProperty(_FullName, Value)
        End Set
    End Property

    Private _CompanyName As String
    Public Property CompanyName As String
        Get
            Return _CompanyName
        End Get
        Set
            SetProperty(_CompanyName, Value)
        End Set
    End Property
#End Region

End Class
