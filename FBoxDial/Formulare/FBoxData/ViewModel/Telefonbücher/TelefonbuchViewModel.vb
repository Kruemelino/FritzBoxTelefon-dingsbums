Imports System.Collections
Imports System.Windows.Input

''' <summary>
''' Das Telefonbuch ist von Tosker erstellt worden und auf Youtube und Github bereitgestellt:
''' https://github.com/Tosker/ContactBook-Tutorial
''' Es wurde duch Kruemelino zu Zwecken der Anzeige von Fritz!Box Kontakten angepasst und erweitert.
''' </summary>
Public Class TelefonbuchViewModel
    Inherits NotifyBase

    Private Property DatenService As IFBoxDataService
    Private Property DialogService As IDialogService
#Region "Fritz!Box Telefonbücher"
    Public Property Telefonbücher As New ObservableCollectionEx(Of PhonebookViewModel)

    Private _Telefonbuch As PhonebookViewModel
    Public Property Telefonbuch As PhonebookViewModel
        Get
            Return _Telefonbuch
        End Get
        Private Set
            SetProperty(_Telefonbuch, Value)
        End Set
    End Property
#End Region

#Region "ViewModel"
    Private _contactsVM As KontaktViewModel
    Public Property ContactsVM As KontaktViewModel
        Get
            Return _contactsVM
        End Get
        Set
            SetProperty(_contactsVM, Value)
        End Set
    End Property
#End Region

#Region "ICommand"
    Public Property LadeFritzBoxTelefonbücher As ICommand
    Public Property LadeFritzBoxKontakte As ICommand
    Public Property NeuesFritzBoxTelefonbuch As ICommand
    Public Property LöscheFritzBoxTelefonbuch As ICommand
    Public Property NeuerTelefonbuchName As ICommand
    Public Property LöscheFritzBoxKontakte As ICommand

#End Region

    Public Sub New(IDataService As IFBoxDataService, IDialogeService As IDialogService)
        ContactsVM = New KontaktViewModel(IDataService, IDialogeService)
        DatenService = IDataService
        DialogService = IDialogeService

        LadeFritzBoxTelefonbücher = New RelayCommand(AddressOf LadeTelefonbücher)
        LadeFritzBoxKontakte = New RelayCommand(AddressOf LadeKontakte)

        NeuesFritzBoxTelefonbuch = New RelayCommand(AddressOf NeuesTelefonbuch, AddressOf CanAdd)
        LöscheFritzBoxTelefonbuch = New RelayCommand(AddressOf LöscheTelefonbuch, AddressOf CanRemove)
        NeuerTelefonbuchName = New RelayCommand(AddressOf TelefonbuchUmbenennen, AddressOf CanName)
        LöscheFritzBoxKontakte = New RelayCommand(AddressOf LöscheKontakte, AddressOf CanDelete)

    End Sub

#Region "ICommand Callback"
#Region "Telefonbücher Laden"
    Private Async Sub LadeTelefonbücher(o As Object)
        ' leere die Collection
        Telefonbücher.Clear()

        ' Lade Fritz!Box Telefonbücher herunter
        InitTelefonbücher(Await DatenService.GetTelefonbücher())
    End Sub

    Friend Sub InitTelefonbücher(Bücher As IEnumerable(Of PhonebookEx))
        If Bücher IsNot Nothing Then
            Telefonbücher.AddRange(Bücher.Select(Function(pb) New PhonebookViewModel(DatenService, pb)))

            If Telefonbücher.Any Then LadeKontakte(Telefonbücher.First)
        End If
    End Sub
#End Region

#Region "Telefonbuch anlegen"
    Private Sub NeuesTelefonbuch(o As Object)
        ' Erzeuge ein neues Telefonbuch
        Dim Telefonbuch As New PhonebookEx(New FBoxAPI.Phonebook With {.Contacts = New List(Of FBoxAPI.Contact)})

        ' Füge im Viewmodel ein neues Telefonbuch hinzu.
        Telefonbücher.Add(New PhonebookViewModel(DatenService, Telefonbuch) With {.Name = "TELEFONBUCHNAME", .IsBookEditMode = True, .ID = -1})

    End Sub
    Private Function CanAdd(o As Object) As Boolean
        Return Telefonbücher IsNot Nothing
    End Function
#End Region

#Region "Telefonbuch umbenennen"
    Private Async Sub TelefonbuchUmbenennen(o As Object)
        With CType(o, PhonebookViewModel)
            ' Schalte den Editiermodus aus.
            .IsBookEditMode = Not .IsBookEditMode
            ' Der Nutzer hat einen Namen festgelegt.
            ' Erstelle ein Telefonbuch mit dem gewählten Namen

            Dim NeuesTelefonbuch As PhonebookEx = Await DatenService.AddTelefonbuch(.Name)

            If NeuesTelefonbuch IsNot Nothing Then
                ' Das neue Telefonbuch wurde angelegt.
                ' Setze die neue ID von der Box.
                .ID = NeuesTelefonbuch.ID

            End If

            OnPropertyChanged(NameOf(Telefonbücher))
            LadeKontakte(o)

        End With

    End Sub
    Private Function CanName(o As Object) As Boolean
        Dim Buch = CType(o, PhonebookViewModel)
        Return Telefonbücher IsNot Nothing AndAlso Buch IsNot Nothing AndAlso
                                                   Buch.Name.IsNotStringNothingOrEmpty AndAlso Not Telefonbücher.Where(Function(TB)
                                                                                                                           Return TB.ID.AreDifferentTo(-1) And TB.Name.IsEqual(Buch.Name)
                                                                                                                       End Function).Any
    End Function
#End Region

#Region "Telefonbuch löschen"
    Private Sub LöscheTelefonbuch(o As Object)
        With CType(o, PhonebookViewModel)
            Dim Löschen As Boolean = False

            If .ID.IsZero Then
                Löschen = DialogService.ShowMessageBox(String.Format(Localize.LocFBoxData.strQuestionBookDeleteID0, .Name)) = Windows.MessageBoxResult.Yes
            Else
                Löschen = DialogService.ShowMessageBox(String.Format(Localize.LocFBoxData.strQuestionBookDelete, .Name)) = Windows.MessageBoxResult.Yes
            End If

            If Löschen Then
                If DatenService.DeleteTelefonbuch(.ID) Then
                    Telefonbücher.Remove(CType(o, PhonebookViewModel))
                End If
            End If
        End With
    End Sub
    Private Function CanRemove(o As Object) As Boolean
        Dim Buch = CType(o, PhonebookViewModel)
        Return Buch IsNot Nothing AndAlso Not Buch.Telefonbuch.Rufsperren
    End Function
#End Region

#Region "Kontakte Laden"
    Private Sub LadeKontakte(o As Object)

        Telefonbuch = CType(o, PhonebookViewModel)

        Telefonbuch.IsSelected = True

        ContactsVM.LadeKontakte(Telefonbuch)

    End Sub

#End Region

#Region "Kontakte löschen"
    Private Function CanDelete(obj As Object) As Boolean
        Return Telefonbuch IsNot Nothing
    End Function

    Private Sub LöscheKontakte(o As Object)
        Dim Kontakte As IEnumerable(Of ContactViewModel) = From a In CType(o, IList).Cast(Of ContactViewModel)
        Dim CList As IEnumerable(Of FBoxAPI.Contact) = Kontakte.Select(Function(C) C.Kontakt)

        If Telefonbuch.Telefonbuch.Rufsperren Then
            If DialogService.ShowMessageBox(String.Format(Localize.LocFBoxData.strQuestionDeleteCallBarrings, Kontakte.Count)) = Windows.MessageBoxResult.Yes Then
                ' Lösche die Einträge der Rufsperre auf der Fritz!Box
                If DatenService.DeleteRufsperren(CList) Then
                    ' Entferne die Kontate aus den Datenobjekten
                    Telefonbuch.Telefonbuch.DeleteKontakte(CList)
                    Telefonbuch.Contacts.RemoveRange(Kontakte)
                End If
            End If

        Else

            If DialogService.ShowMessageBox(String.Format(Localize.LocFBoxData.strQuestionDeleteContacts, Kontakte.Count, Telefonbuch.Name)) = Windows.MessageBoxResult.Yes Then
                ' Lösche die Einträge in dem Telefonbuch auf der Fritz!Box
                If DatenService.DeleteKontakte(Telefonbuch.ID, CList) Then
                    ' Entferne die Kontate aus den Datenobjekten
                    Telefonbuch.Telefonbuch.DeleteKontakte(CList)
                    Telefonbuch.Contacts.RemoveRange(Kontakte)
                End If
            End If

        End If

        CList = Nothing
        Kontakte = Nothing
    End Sub
#End Region
#End Region
End Class
