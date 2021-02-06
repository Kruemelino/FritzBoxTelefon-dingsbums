Imports System.Windows.Input


Public Class ContactsViewModel
    Inherits NotifyBase

    Private _Contacts As ObservableCollectionEx(Of FritzBoxXMLKontakt), _EditCommand As ICommand, _SaveCommand As ICommand, _UpdateCommand As ICommand, _BrowseImageCommand As ICommand, _AddCommand As ICommand, _DeleteCommand As ICommand
    Private _selectedContact As FritzBoxXMLKontakt

    Public Property SelectedContact As FritzBoxXMLKontakt
        Get
            Return _selectedContact
        End Get
        Set
            SetProperty(_selectedContact, Value)
        End Set
    End Property

    Private _isEditMode As Boolean

    Public Property IsEditMode As Boolean
        Get
            Return _isEditMode
        End Get
        Set
            SetProperty(_isEditMode, Value)
            MyBase.OnPropertyChanged("IsDisplayMode")
        End Set
    End Property

    Public ReadOnly Property IsDisplayMode As Boolean
        Get
            Return Not _isEditMode
        End Get
    End Property

    Public Property Contacts As ObservableCollectionEx(Of FritzBoxXMLKontakt)
        Get
            Return _Contacts
        End Get
        Set
            _Contacts = Value
        End Set
    End Property

    Public Property EditCommand As ICommand
        Get
            Return _EditCommand
        End Get
        Private Set(value As ICommand)
            _EditCommand = value
        End Set
    End Property

    Public Property SaveCommand As ICommand
        Get
            Return _SaveCommand
        End Get
        Private Set(value As ICommand)
            _SaveCommand = value
        End Set
    End Property

    Public Property UpdateCommand As ICommand
        Get
            Return _UpdateCommand
        End Get
        Private Set(value As ICommand)
            _UpdateCommand = value
        End Set
    End Property

    Public Property BrowseImageCommand As ICommand
        Get
            Return _BrowseImageCommand
        End Get
        Private Set(value As ICommand)
            _BrowseImageCommand = value
        End Set
    End Property

    Public Property AddCommand As ICommand
        Get
            Return _AddCommand
        End Get
        Private Set(value As ICommand)
            _AddCommand = value
        End Set
    End Property

    Public Property DeleteCommand As ICommand
        Get
            Return _DeleteCommand
        End Get
        Private Set(value As ICommand)
            _DeleteCommand = value
        End Set
    End Property

    'Private _dataService As IContactDataService
    'Private _dialogService As IDialogService

    Public Sub New() '(dataService As IContactDataService, dialogService As IDialogService)
        Contacts = New ObservableCollectionEx(Of FritzBoxXMLKontakt)
        '_dataService = dataService
        '_dialogService = dialogService
        EditCommand = New RelayCommand(Sub() Edit(), Function() IsEdit())
        SaveCommand = New RelayCommand(AddressOf Save)
        UpdateCommand = New RelayCommand(AddressOf Update)
        BrowseImageCommand = New RelayCommand(AddressOf BrowseImage)
        AddCommand = New RelayCommand(AddressOf Add)
        DeleteCommand = New RelayCommand(AddressOf Delete)
    End Sub

    Private Sub Delete(obj As Object)
        Contacts.Remove(SelectedContact)
        Save(obj)
    End Sub

    Private Function CanDelete() As Boolean
        Return If(SelectedContact Is Nothing, False, True)
    End Function

    Private Sub Add(obj As Object)
        Dim newContact = New FritzBoxXMLKontakt
        Contacts.Add(newContact)
        SelectedContact = newContact
    End Sub

    Private Sub BrowseImage(obj As Object)
        'Dim filePath = _dialogService.OpenFile("Image files|*.bmp;*.jpg;*.jpeg;*.png|All files")
        'SelectedContact.ImagePath = filePath
    End Sub

    Private Sub Update(obj As Object)
        '_dataService.Save(Contacts)
    End Sub

    Private Sub Save(obj As Object)
        '_dataService.Save(Contacts)
        'IsEditMode = False
        'OnPropertyChanged("SelectedContact")
    End Sub

    Private Function IsEdit() As Boolean
        Return IsEditMode
    End Function

    Private Function CanEdit() As Boolean
        If SelectedContact Is Nothing Then Return False
        Return Not IsEditMode
    End Function

    Private Sub Edit()
        IsEditMode = True
    End Sub

    Public Sub LoadContacts()

    End Sub

    Private Async Sub LadeTelefonbücher()
        If ThisAddIn.PhoneBookXML Is Nothing OrElse ThisAddIn.PhoneBookXML.Telefonbuch Is Nothing Then
            ' Telefonbücher asynchron herunterladen
            ThisAddIn.PhoneBookXML = Await LadeFritzBoxTelefonbücher()
        End If

        Contacts.AddRange(ThisAddIn.PhoneBookXML.Telefonbuch.First.Kontakte)
        MyBase.OnPropertyChanged("Contacts")
    End Sub
End Class
