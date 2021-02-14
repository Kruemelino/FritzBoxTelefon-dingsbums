Imports System.Windows.Input
Imports FBoxDial.Localize.resTelefonbuch

''' <summary>
''' Das Telefonbuch ist von Tosker erstellt worden und auf Youtube und Github bereitgestellt:
''' https://github.com/Tosker/ContactBook-Tutorial
''' Es wurde duch Kruemelino zu Zwecken der Anzeige von Fritz!Box Kontakten angepasst und erweitert.
''' </summary>
Public Class BookViewModel
    Inherits NotifyBase

    Private Property DatenService As IContactDataService
    Private Property DialogService As IDialogService
#Region "Fritz!Box Telefonbücher"
    Private _Telefonbücher As ObservableCollectionEx(Of FritzBoxXMLTelefonbuch)
    Public Property Telefonbücher As ObservableCollectionEx(Of FritzBoxXMLTelefonbuch)
        Get
            Return _Telefonbücher
        End Get
        Private Set
            SetProperty(_Telefonbücher, Value)
        End Set
    End Property

    Private _Telefonbuch As FritzBoxXMLTelefonbuch
    Public Property Telefonbuch As FritzBoxXMLTelefonbuch
        Get
            Return _Telefonbuch
        End Get
        Private Set
            SetProperty(_Telefonbuch, Value)
        End Set
    End Property
#End Region

#Region "ViewModel"
    Private _contactsVM As ContactsViewModel
    Public Property ContactsVM As ContactsViewModel
        Get
            Return _contactsVM
        End Get
        Set
            SetProperty(_contactsVM, Value)
        End Set
    End Property
#End Region

#Region "ICommand"
    Private _LadeFritzBoxTelefonbücher As ICommand
    Public Property LadeFritzBoxTelefonbücher As ICommand
        Get
            Return _LadeFritzBoxTelefonbücher
        End Get
        Private Set
            _LadeFritzBoxTelefonbücher = Value
        End Set
    End Property

    Private _LadeFritzBoxKontakte As ICommand
    Public Property LadeFritzBoxKontakte As ICommand
        Get
            Return _LadeFritzBoxKontakte
        End Get
        Private Set
            _LadeFritzBoxKontakte = Value
        End Set
    End Property

    Private _NeuesFritzBoxTelefonbuch As ICommand
    Public Property NeuesFritzBoxTelefonbuch As ICommand
        Get
            Return _NeuesFritzBoxTelefonbuch
        End Get
        Private Set
            _NeuesFritzBoxTelefonbuch = Value
        End Set
    End Property

    Private _LöscheFritzBoxTelefonbuch As ICommand
    Public Property LöscheFritzBoxTelefonbuch As ICommand
        Get
            Return _LöscheFritzBoxTelefonbuch
        End Get
        Private Set
            _LöscheFritzBoxTelefonbuch = Value
        End Set
    End Property

    Private _ReNameTelefonbuch As ICommand
    Public Property NeuerTelefonbuchName As ICommand
        Get
            Return _ReNameTelefonbuch
        End Get
        Private Set
            _ReNameTelefonbuch = Value
        End Set
    End Property

#End Region

    Public Sub New(IDataService As IContactDataService, IDialogeService As IDialogService)
        ContactsVM = New ContactsViewModel(IDataService, IDialogeService)
        DatenService = IDataService
        DialogService = IDialogeService

        LadeFritzBoxTelefonbücher = New RelayCommand(AddressOf LadeTelefonbücher)
        LadeFritzBoxKontakte = New RelayCommand(AddressOf LadeKontakte)

        NeuesFritzBoxTelefonbuch = New RelayCommand(AddressOf NeuesTelefonbuch, AddressOf CanAdd)
        LöscheFritzBoxTelefonbuch = New RelayCommand(AddressOf LöscheTelefonbuch, AddressOf CanRemove)
        NeuerTelefonbuchName = New RelayCommand(AddressOf TelefonbuchErstellen, AddressOf CanName)

        'LadeTelefonbücher(Nothing)

    End Sub

#Region "ICommad Callback"
    Private Sub LadeKontakte(o As Object)

        Telefonbuch = CType(o, FritzBoxXMLTelefonbuch)
        With ContactsVM
            .LadeKontakte(Telefonbuch)
        End With
    End Sub


    Private Async Sub LadeTelefonbücher(o As Object)
        ' Lade Fritz!Box Telefonbücher herunter
        LoadTelefonbücher(Await DatenService.GetTelefonbücher())

    End Sub

    Public Sub LoadTelefonbücher(Bücher As FritzBoxXMLTelefonbücher)
        If Bücher IsNot Nothing AndAlso Bücher.Telefonbücher IsNot Nothing Then
            Telefonbücher = New ObservableCollectionEx(Of FritzBoxXMLTelefonbuch)(Bücher.Telefonbücher)
            OnPropertyChanged(NameOf(Telefonbücher))

            If Telefonbücher.Any Then LadeKontakte(Telefonbücher.First)

        End If
    End Sub

    Private Sub NeuesTelefonbuch(o As Object)

        ' Füge im Viewmodel ein neues Telefonbuch hinzu.
        Telefonbücher.Add(New FritzBoxXMLTelefonbuch With {.Name = "TELEFONBUCHNAME", .IsBookEditMode = True, .ID = -1})

    End Sub
    Private Function CanAdd(o As Object) As Boolean
        Return Telefonbücher IsNot Nothing
    End Function

    Private Sub LöscheTelefonbuch(o As Object)
        With CType(o, FritzBoxXMLTelefonbuch)
            Dim Löschen As Boolean = False

            If .ID.IsZero Then
                Löschen = DialogService.ShowMessageBox(String.Format(strQuestionBookDeleteID0, .Name)) = Windows.MessageBoxResult.Yes
            Else
                Löschen = DialogService.ShowMessageBox(String.Format(strQuestionBookDelete, .Name)) = Windows.MessageBoxResult.Yes
            End If

            If Löschen Then
                If DatenService.DeleteTelefonbuch(.ID) Then
                    Telefonbücher.Remove(CType(o, FritzBoxXMLTelefonbuch))
                End If
            End If
        End With
    End Sub
    Private Function CanRemove(o As Object) As Boolean
        Return True
    End Function

    Private Async Sub TelefonbuchErstellen(o As Object)
        With CType(o, FritzBoxXMLTelefonbuch)
            ' Schalte den Editiermodus aus.
            .IsBookEditMode = Not .IsBookEditMode
            ' Der Nutzer hat einen Namen festgelegt.
            ' Erstelle ein Telefonbuch mit dem gewählten Namen

            Dim NeuesTelefonbuch As FritzBoxXMLTelefonbuch = Await DatenService.AddTelefonbuch(.Name)

            If NeuesTelefonbuch IsNot Nothing Then
                ' Das neue Telefonbuch wurde angelegt.
                ' Setze die neue ID von der Box.
                .ID = NeuesTelefonbuch.ID

            End If

            OnPropertyChanged(NameOf(Telefonbücher))
            LadeKontakte(NeuesTelefonbuch)

        End With

    End Sub
    Private Function CanName(o As Object) As Boolean
        Dim Buch As FritzBoxXMLTelefonbuch = CType(o, FritzBoxXMLTelefonbuch)
        Return Telefonbücher IsNot Nothing AndAlso Buch.Name.IsNotStringEmpty And Not Telefonbücher.Where(Function(TB)
                                                                                                              Return TB.ID.AreDifferentTo(-1) And TB.Name.AreEqual(Buch.Name)
                                                                                                          End Function).Any
    End Function
#End Region

End Class
