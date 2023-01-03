Imports Microsoft.Office.Interop
Imports Microsoft.Office.Interop.Outlook

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
                ' Parent Folder Name zuweisen
                ParentFolder = String.Format(Localize.LocKontaktsuche.strToolTippParentFolder, If(CType(OlKontakt.Parent, MAPIFolder).Name, "Unbekannt"))
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

    Private _ParentFolder As String
    Public Property ParentFolder As String
        Get
            Return _ParentFolder
        End Get
        Set
            SetProperty(_ParentFolder, Value)
        End Set
    End Property
#End Region

End Class
