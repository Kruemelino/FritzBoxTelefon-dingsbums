Imports System.Windows.Input
Imports System.Windows.Data

''' <summary>
''' Das Telefonbuch ist von Tosker erstellt worden und auf Youtube und Github bereitgestellt:
''' https://github.com/Tosker/ContactBook-Tutorial
''' Es wurde duch Kruemelino zu Zwecken der Anzeige von Fritz!Box Kontakten angepasst und erweitert.
''' </summary>
Public Class ContactsViewModel
    Inherits NotifyBase

    Private Property DatenService As IContactDataService
    Private Property DialogService As IDialogService

#Region "Fritz!Box Telefonbuch Kontakte"

    Private _FBoxKontakt As FritzBoxXMLKontakt
    Public Property FBoxKontakt As FritzBoxXMLKontakt
        Get
            Return _FBoxKontakt
        End Get
        Set
            IsEditMode = False
            SetProperty(_FBoxKontakt, Value)
        End Set
    End Property

    Private _FBoxKontaktClone As FritzBoxXMLKontakt
    Public Property FBoxKontaktClone As FritzBoxXMLKontakt
        Get
            Return _FBoxKontaktClone
        End Get
        Set
            SetProperty(_FBoxKontaktClone, Value)
        End Set
    End Property

    Private _FBoxTelefonbuch As FritzBoxXMLTelefonbuch
    Public Property FBoxTelefonbuch As FritzBoxXMLTelefonbuch
        Get
            Return _FBoxTelefonbuch
        End Get
        Set
            SetProperty(_FBoxTelefonbuch, Value)
        End Set
    End Property

#End Region

#Region "Contact Mode: Edit/Display"
    Private _isContactEditMode As Boolean
    Public Property IsEditMode As Boolean
        Get
            Return _isContactEditMode
        End Get
        Set
            SetProperty(_isContactEditMode, Value)
            OnPropertyChanged(NameOf(IsDisplayMode))
        End Set
    End Property

    Public ReadOnly Property IsDisplayMode As Boolean
        Get
            Return Not _isContactEditMode
        End Get
    End Property
#End Region
#Region "Filtern"
    Public Property View As ListCollectionView

    Private _FilterName As String
    Public Property FilterName As String
        Get
            Return _FilterName
        End Get
        Set
            SetProperty(_FilterName, Value)
            View?.Refresh()
        End Set
    End Property

    Public Function Filter(o As Object) As Boolean
        With CType(o, FritzBoxXMLKontakt)

            If .IstTelefon Then
                ' Telefone werden immer weggefiltet
                Return False
            Else
                Return Not FilterName.IsNotStringNothingOrEmpty OrElse .Person.RealName.ToLower.Contains(FilterName.ToLower)
            End If
        End With
    End Function

#End Region
#Region "ICommand"
    Public Property EditCommand As ICommand
    Public Property SaveCommand As ICommand
    Public Property CancelCommand As ICommand
    Public Property UpdateCommand As ICommand
    'Public Property BrowseImageCommand As ICommand
    Public Property AddContact As ICommand
    Public Property DeleteCommand As ICommand
    Public Property DialCommand As ICommand
    Public Property AddNumber As ICommand
    Public Property AddMail As ICommand
    Public Property RemoveNumber As ICommand
    Public Property RemoveMail As ICommand
#End Region

    Public Sub New(dataService As IContactDataService, dialogService As IDialogService)
        _DatenService = dataService
        _DialogService = dialogService

        EditCommand = New RelayCommand(AddressOf Edit, AddressOf CanEdit)
        SaveCommand = New RelayCommand(AddressOf Save, AddressOf IsEdit)
        CancelCommand = New RelayCommand(AddressOf CancelEdit, AddressOf IsEdit)
        UpdateCommand = New RelayCommand(AddressOf Update)

        DeleteCommand = New RelayCommand(AddressOf Delete, AddressOf CanDelete)
        DialCommand = New RelayCommand(AddressOf Dial, AddressOf CanDial)
        AddContact = New RelayCommand(AddressOf AddKontakt, AddressOf CanAddKontakt)

        AddNumber = New RelayCommand(AddressOf AddTelNr)
        AddMail = New RelayCommand(AddressOf AddEMail)
        RemoveNumber = New RelayCommand(AddressOf RemoveTelNr)
        RemoveMail = New RelayCommand(AddressOf RemoveEMail)

    End Sub

    Public Sub LadeKontakte(Telefonbuch As FritzBoxXMLTelefonbuch)

        FBoxTelefonbuch = Telefonbuch

        Telefonbuch.LadeKonaktbilder()

        View = CType(CollectionViewSource.GetDefaultView(FBoxTelefonbuch.Kontakte), ListCollectionView)

        View.Filter = New Predicate(Of Object)(AddressOf Filter)

    End Sub

#Region "ICommand Callback"
#Region "Kontakt löschen"
    Private Sub Delete(o As Object)
        If FBoxTelefonbuch.Rufsperren Then
            If DialogService.ShowMessageBox(Localize.resTelefonbuch.strQuestionDeleteCallBarring) = Windows.MessageBoxResult.Yes Then
                ' Lösche die Rufsperre auf der Fritz!Box
                If DatenService.DeleteRufsperre(FBoxKontakt.Uniqueid) Then
                    FBoxTelefonbuch.DeleteKontakt(FBoxKontakt)
                End If
            End If

        Else

            If DialogService.ShowMessageBox(String.Format(Localize.resTelefonbuch.strQuestionDeleteContact, FBoxKontakt.Person.RealName, FBoxTelefonbuch.Name)) = Windows.MessageBoxResult.Yes Then
                ' lösche den Kontakt auf der Box
                If DatenService.DeleteKontakt(FBoxTelefonbuch.ID, FBoxKontakt.Uniqueid) Then
                    FBoxTelefonbuch.DeleteKontakt(FBoxKontakt)
                End If
            End If

        End If

    End Sub
    Private Function CanDelete(o As Object) As Boolean
        Return FBoxKontakt IsNot Nothing
    End Function
#End Region

#Region "Kontakt hinzufügen"
    Private Sub AddKontakt(o As Object)
        Dim NeuerKontakt = New FritzBoxXMLKontakt
        NeuerKontakt.Person.RealName = "N/A"

        FBoxTelefonbuch.AddContact(NeuerKontakt)

        FBoxKontakt = NeuerKontakt

        IsEditMode = True
    End Sub

    Private Function CanAddKontakt(o As Object) As Boolean
        Return IsDisplayMode AndAlso FBoxTelefonbuch IsNot Nothing AndAlso FBoxTelefonbuch.Kontakte IsNot Nothing
    End Function
#End Region

#Region "Kontakt aktualisieren"
    Private Sub Update(o As Object)
        ' Hier wird das Favorite / Wichtiger Kontakt gespeichert.
        If IsDisplayMode Then
            FBoxKontakt.Uniqueid = DatenService.SetKontakt(FBoxTelefonbuch.ID, FBoxKontakt.GetXMLKontakt)
        End If

    End Sub
#End Region

#Region "Kontakt speichern"
    Private Sub Save(o As Object)
        If FBoxTelefonbuch.Rufsperren Then
            FBoxKontakt.Uniqueid = DatenService.SetRufsperre(FBoxKontakt)
        Else
            ' Lade den Kontakt hoch und setze die UID
            FBoxKontakt.Uniqueid = DatenService.SetKontakt(FBoxTelefonbuch.ID, FBoxKontakt.GetXMLKontakt)
        End If

        ' Lösche den Clone
        FBoxKontaktClone = Nothing

        ' Beende den Editiermodus
        IsEditMode = False
        OnPropertyChanged(NameOf(FBoxKontakt))
    End Sub
#End Region

#Region "Kontakt editieren"
    Private Sub CancelEdit(o As Object)
        ' Setze den Clone zurück
        FBoxKontakt = XMLClone(FBoxKontaktClone)

        ' Lösche den Clone
        FBoxKontaktClone = Nothing

        ' Beende den Editiermodus
        IsEditMode = False
        OnPropertyChanged(NameOf(FBoxKontakt))
    End Sub

    Private Function IsEdit(o As Object) As Boolean
        Return IsEditMode
    End Function

    Private Function CanEdit(o As Object) As Boolean
        Return FBoxKontakt IsNot Nothing And Not IsEditMode
        'Return Not IsEditMode
    End Function

    Private Sub Edit(o As Object)
        If IsEditMode Then
            ' Aktuelle Änderungen speichern
            Save(o)
        Else
            ' Erstelle einen Clone des aktuellen Kontakte
            FBoxKontaktClone = XMLClone(FBoxKontakt)

            IsEditMode = Not IsEditMode
        End If

    End Sub
#End Region

#Region "Kontakt Telefonnummer hinzufügen/entfernen"
    Private Sub AddTelNr(o As Object)
        If FBoxKontakt.Telefonie Is Nothing Then FBoxKontakt.Telefonie = New FritzBoxXMLTelefonie
        If FBoxKontakt.Telefonie.Nummern Is Nothing Then FBoxKontakt.Telefonie.Nummern = New ObservableCollectionEx(Of FritzBoxXMLNummer)

        FBoxKontakt.Telefonie.Nummern.Add(New FritzBoxXMLNummer)
    End Sub
    Private Sub RemoveTelNr(o As Object)
        FBoxKontakt.Telefonie.Nummern.Remove(CType(o, FritzBoxXMLNummer))
    End Sub
#End Region

#Region "Kontakt E-Mail hinzufügen/entfernen"
    Private Sub AddEMail(o As Object)
        If FBoxKontakt.Telefonie Is Nothing Then FBoxKontakt.Telefonie = New FritzBoxXMLTelefonie
        If FBoxKontakt.Telefonie.Emails Is Nothing Then FBoxKontakt.Telefonie.Emails = New ObservableCollectionEx(Of FritzBoxXMLEmail)

        FBoxKontakt.Telefonie.Emails.Add(New FritzBoxXMLEmail)
    End Sub
    Private Sub RemoveEMail(o As Object)
        FBoxKontakt.Telefonie.Emails.Remove(CType(o, FritzBoxXMLEmail))
    End Sub

#End Region

#Region "Kontakt anrufen"
    Private Sub Dial(o As Object)
        DatenService.Dial(FBoxKontakt)
    End Sub

    Private Function CanDial(o As Object) As Boolean
        Return IsDisplayMode AndAlso FBoxTelefonbuch IsNot Nothing AndAlso FBoxTelefonbuch.Kontakte IsNot Nothing
    End Function
#End Region
#End Region

End Class
