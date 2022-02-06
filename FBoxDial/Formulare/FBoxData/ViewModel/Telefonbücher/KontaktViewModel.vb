Imports System.Windows.Input
Imports System.Windows.Data

''' <summary>
''' Das Telefonbuch ist von Tosker erstellt worden und auf Youtube und Github bereitgestellt:
''' https://github.com/Tosker/ContactBook-Tutorial
''' Es wurde duch Kruemelino zu Zwecken der Anzeige von Fritz!Box Kontakten angepasst und erweitert.
''' </summary>
Public Class KontaktViewModel
    Inherits NotifyBase

    Private Property DatenService As IFBoxDataService
    Private Property DialogService As IDialogService

#Region "Fritz!Box Telefonbuch Kontakte"

    Private _FBoxKontakt As ContactViewModel
    Public Property FBoxKontakt As ContactViewModel
        Get
            Return _FBoxKontakt
        End Get
        Set
            IsEditMode = False
            SetProperty(_FBoxKontakt, Value)
        End Set
    End Property

    Private _FBoxTelefonbuch As PhonebookViewModel
    Public Property FBoxTelefonbuch As PhonebookViewModel
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
        With CType(o, ContactViewModel)

            If .Kontakt.IstTelefon Then
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
    Public Property AddContact As ICommand
    Public Property DeleteCommand As ICommand
    Public Property DialCommand As ICommand
    Public Property AddNumber As ICommand
    Public Property AddMail As ICommand
    Public Property RemoveNumber As ICommand
    Public Property RemoveMail As ICommand
#End Region

    Public Sub New(dataService As IFBoxDataService, dialogService As IDialogService)
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

    Public Sub SetupFilter(Telefonbuch As PhonebookViewModel)

        FBoxTelefonbuch = Telefonbuch

        View = CType(CollectionViewSource.GetDefaultView(FBoxTelefonbuch.Contacts), ListCollectionView)

        View.Filter = New Predicate(Of Object)(AddressOf Filter)

    End Sub

#Region "ICommand Callback"
#Region "Kontakt löschen"
    Private Sub Delete(o As Object)
        If FBoxTelefonbuch.Telefonbuch.Rufsperren Then
            If DialogService.ShowMessageBox(Localize.LocFBoxData.strQuestionDeleteCallBarring) = Windows.MessageBoxResult.Yes Then
                ' Lösche die Rufsperre auf der Fritz!Box
                If DatenService.DeleteRufsperre(FBoxKontakt.Uniqueid) Then
                    ' Entferne den Kontakt ...
                    ' ... im Fritz!Box Telefonbuch
                    FBoxTelefonbuch.Telefonbuch.DeleteKontakt(FBoxKontakt.Kontakt)
                    ' ... in der ObservableCollection
                    FBoxTelefonbuch.Contacts.Remove(FBoxKontakt)
                End If
            End If

        Else

            If DialogService.ShowMessageBox(String.Format(Localize.LocFBoxData.strQuestionDeleteContact, FBoxKontakt.Person.RealName, FBoxTelefonbuch.Name)) = Windows.MessageBoxResult.Yes Then
                ' lösche den Kontakt auf der Box
                If DatenService.DeleteKontakt(FBoxTelefonbuch.ID, FBoxKontakt.Uniqueid) Then
                    ' Entferne den Kontakt ...
                    ' ... im Fritz!Box Telefonbuch
                    FBoxTelefonbuch.Telefonbuch.DeleteKontakt(FBoxKontakt.Kontakt)
                    ' ... in der ObservableCollection
                    FBoxTelefonbuch.Contacts.Remove(FBoxKontakt)
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

        ' Neues Kontaktelement erzeugen und den initialen Namen hinzufügen
        Dim NeuerKontakt As FBoxAPI.Contact = CreateContact("N/A")

        ' Neues KontaktViewModel erzeugen
        Dim NeuesKontaktVM = New ContactViewModel(DatenService, NeuerKontakt)

        ' Füge den Kontakt hinzu ...
        ' ... dem Fritz!Box Telefonbuch
        FBoxTelefonbuch.Telefonbuch.AddContact(NeuesKontaktVM.Kontakt)
        ' ... der ObservableCollection
        FBoxTelefonbuch.Contacts.Add(NeuesKontaktVM)

        ' Zeige den Kontakt an
        FBoxKontakt = NeuesKontaktVM

        ' Schalte den Editiermodus ein
        IsEditMode = True
    End Sub

    Private Function CanAddKontakt(o As Object) As Boolean
        Return IsDisplayMode AndAlso FBoxTelefonbuch IsNot Nothing AndAlso FBoxTelefonbuch.Contacts IsNot Nothing
    End Function
#End Region

#Region "Kontakt aktualisieren"
    Private Sub Update(o As Object)
        ' Hier wird das Favorite / Wichtiger Kontakt gespeichert.
        If IsDisplayMode Then
            FBoxKontakt.Uniqueid = DatenService.SetKontakt(FBoxTelefonbuch.ID, FBoxKontakt.Kontakt.GetXMLKontakt)
        End If

    End Sub
#End Region

#Region "Kontakt speichern"
    Private Sub Save(o As Object)
        If FBoxTelefonbuch.Telefonbuch.Rufsperren Then
            FBoxKontakt.Uniqueid = DatenService.SetRufsperre(FBoxKontakt.Kontakt)
        Else
            ' Lade den Kontakt hoch und setze die UID
            FBoxKontakt.Uniqueid = DatenService.SetKontakt(FBoxTelefonbuch.ID, FBoxKontakt.Kontakt.GetXMLKontakt)
        End If

        ' Lösche den Clone
        FBoxKontakt.KontaktKlone = Nothing

        ' Beende den Editiermodus
        IsEditMode = False
        OnPropertyChanged(NameOf(FBoxKontakt))
    End Sub
#End Region

#Region "Kontakt editieren"
    Private Sub CancelEdit(o As Object)
        ' Setze den Clone zurück
        FBoxKontakt = New ContactViewModel(DatenService, XMLClone(FBoxKontakt.KontaktKlone))

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
            FBoxKontakt.KontaktKlone = XMLClone(FBoxKontakt.Kontakt)

            IsEditMode = Not IsEditMode
        End If

    End Sub
#End Region

#Region "Kontakt Telefonnummer hinzufügen/entfernen"
    Private Sub AddTelNr(o As Object)
        ' Ein neues Nummernelement erzeugen
        Dim NeueNummer As New FBoxAPI.NumberType

        ' Ein neues NummerViewModel erzeugen
        Dim NummernVM As New NumberViewModel(NeueNummer)

        With FBoxKontakt.Telefonie
            ' Nummern VM der ObservableCollection hinzufügen
            .Nummern.Add(NummernVM)

            If .Telefonie Is Nothing Then .Telefonie = New FBoxAPI.Telephony
            If .Telefonie.Numbers Is Nothing Then .Telefonie.Numbers = New List(Of FBoxAPI.NumberType)

            ' Nummer dem Modell hinzufügen
            .Telefonie.Numbers.Add(NeueNummer)
        End With
    End Sub
    Private Sub RemoveTelNr(o As Object)
        FBoxKontakt.Telefonie.Nummern.Remove(CType(o, NumberViewModel))
    End Sub
#End Region

#Region "Kontakt E-Mail hinzufügen/entfernen"
    Private Sub AddEMail(o As Object)
        ' Ein neues Mailelement erzeugen
        Dim NeueMail As New FBoxAPI.Email

        ' Ein neues MailViewModel erzeugen
        Dim MailVM As New EMailViewModel(NeueMail)

        With FBoxKontakt.Telefonie
            ' Nummern VM der ObservableCollection hinzufügen
            .Emails.Add(MailVM)

            If .Telefonie Is Nothing Then .Telefonie = New FBoxAPI.Telephony
            If .Telefonie.Emails Is Nothing Then .Telefonie.Emails = New List(Of FBoxAPI.Email)

            ' Mail dem Modell hinzufügen
            .Telefonie.Emails.Add(NeueMail)
        End With
    End Sub
    Private Sub RemoveEMail(o As Object)
        FBoxKontakt.Telefonie.Emails.Remove(CType(o, EMailViewModel))
    End Sub

#End Region

#Region "Kontakt anrufen"
    Private Sub Dial(o As Object)
        DatenService.Dial(FBoxKontakt.Kontakt)
    End Sub

    Private Function CanDial(o As Object) As Boolean
        Return IsDisplayMode AndAlso FBoxTelefonbuch IsNot Nothing AndAlso
                                     FBoxKontakt IsNot Nothing AndAlso
                                     FBoxKontakt.Telefonie IsNot Nothing AndAlso
                                     FBoxKontakt.Telefonie.Nummern.Any
    End Function
#End Region
#End Region

End Class
