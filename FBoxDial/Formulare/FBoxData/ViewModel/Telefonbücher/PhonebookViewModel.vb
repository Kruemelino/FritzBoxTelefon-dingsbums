Imports System.Windows.Threading

Public Class PhonebookViewModel
    Inherits NotifyBase
    Private Property DatenService As IFBoxDataService
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Models"
    Public Property Telefonbuch As PhonebookEx
#End Region

#Region "Fritz!Box Eigenschaften"

    Private _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set
            SetProperty(_Name, Value)
            Telefonbuch.Phonebook.Name = Value
        End Set
    End Property

    Public Property Contacts As ObservableCollectionEx(Of ContactViewModel)
#End Region

    Public Sub New(dataservice As IFBoxDataService, phonebook As PhonebookEx)

        DatenService = dataservice

        _Telefonbuch = phonebook

        _Name = Telefonbuch.Phonebook.Name

        _ID = Telefonbuch.ID

        Contacts = New ObservableCollectionEx(Of ContactViewModel)(Telefonbuch.Phonebook.Contacts.Select(Function(C) New ContactViewModel(C)))

        LadeBilder()
    End Sub

#Region "Eigene Eigenschaften"
    Friend Property ID As Integer

    Private _IsBookEditMode As Boolean
    Public Property IsBookEditMode As Boolean
        Get
            Return _IsBookEditMode
        End Get
        Set
            SetProperty(_IsBookEditMode, Value)
            OnPropertyChanged(NameOf(IsBookDisplayMode))
        End Set
    End Property

    Public ReadOnly Property IsBookDisplayMode As Boolean
        Get
            Return Not IsBookEditMode
        End Get
    End Property

#End Region

#Region "Eigene Routinen"
    Private Async Sub LadeBilder()

        For Each Contact In Contacts
            Contact.Person.ImageData = Await Dispatcher.CurrentDispatcher.Invoke(Async Function() As Threading.Tasks.Task(Of Windows.Media.ImageSource)
                                                                                     Return Await DatenService.LadeKontaktbild(Contact.Person.Person)
                                                                                 End Function)

        Next
    End Sub
#End Region

End Class
