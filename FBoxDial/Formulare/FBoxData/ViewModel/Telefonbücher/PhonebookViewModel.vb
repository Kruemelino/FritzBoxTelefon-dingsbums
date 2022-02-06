Imports System.Windows.Threading

Public Class PhonebookViewModel
    Inherits NotifyBase
    Private Property DatenService As IFBoxDataService
    ' Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

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

    End Sub

    ''' <summary>
    ''' Lädt die Kontakte in die ObservableCollection, falls noch nicht geschehen. 
    ''' </summary>
    Private Sub LadeKontakte()
        If Contacts Is Nothing Then
            Contacts = New ObservableCollectionEx(Of ContactViewModel)(Telefonbuch.Phonebook.Contacts.Select(Function(C) New ContactViewModel(DatenService, C)))
        End If
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

    Private _IsSelected As Boolean
    Public Property IsSelected As Boolean
        Get
            Return _IsSelected
        End Get
        Set
            SetProperty(_IsSelected, Value)
            ' Wenn das Telefonbuch selektiert wird, sollen die Kontakte geladen werden.
            If Value Then LadeKontakte()
        End Set
    End Property
#End Region

End Class
