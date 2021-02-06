Imports System.Windows.Input

Public Class BookViewModel
    Inherits NotifyBase

    Private _LoadContactsCommand As ICommand
    Private _contactsVM As ContactsViewModel

    Public Property ContactsVM As ContactsViewModel
        Get
            Return _contactsVM
        End Get
        Set
            SetProperty(_contactsVM, Value)
        End Set
    End Property

    Public Property LoadContactsCommand As ICommand
        Get
            Return _LoadContactsCommand
        End Get
        Set
            _LoadContactsCommand = Value
        End Set
    End Property

    Public Sub New()
        ContactsVM = New ContactsViewModel()
        LoadContactsCommand = New RelayCommand(AddressOf LoadContacts)
    End Sub

    Private Sub LoadContacts(obj As Object)

        ContactsVM.LoadContacts()
    End Sub

End Class
