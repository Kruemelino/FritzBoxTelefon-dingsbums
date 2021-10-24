Imports System.Windows.Threading
Imports Microsoft.Office.Interop
Public Class WählClientViewModel
    Inherits NotifyBase
    ' Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend Property Wählclient As FritzBoxWählClient
    Private Property DatenService As IDialService
    Private Property DialogService As IDialogService
    Friend Property Instance As Dispatcher
#Region "Eigenschaften"
    Private _currentView As Object
    Public Property CurrentView As Object
        Get
            Return _currentView
        End Get
        Set
            SetProperty(_currentView, Value)
        End Set
    End Property

    Private _ContactDialVM As ContactDialViewModel
    Public Property ContactDialVM As ContactDialViewModel
        Get
            Return _ContactDialVM
        End Get
        Set
            SetProperty(_ContactDialVM, Value)
        End Set
    End Property

    Private _DirectDialVM As DirectDialViewModel
    Public Property DirectDialVM As DirectDialViewModel
        Get
            Return _DirectDialVM
        End Get
        Set
            SetProperty(_DirectDialVM, Value)
        End Set
    End Property

    Private _IsContactDial As Boolean
    Public Property IsContactDial As Boolean
        Get
            Return _IsContactDial
        End Get
        Set
            SetProperty(_IsContactDial, Value)
            OnPropertyChanged(NameOf(IsDirectDial))

            ' Lade weitere Daten des Formulars
            SetData()
        End Set
    End Property

    Public ReadOnly Property IsDirectDial As Boolean
        Get
            Return Not _IsContactDial
        End Get
    End Property

    Private _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set
            SetProperty(_Name, Value)
        End Set
    End Property

    Private _DialDeviceList As New ObservableCollectionEx(Of Telefoniegerät)
    Public Property DialDeviceList As ObservableCollectionEx(Of Telefoniegerät)
        Get
            Return _DialDeviceList
        End Get
        Set
            SetProperty(_DialDeviceList, Value)
        End Set
    End Property

    Private _TelGerät As Telefoniegerät
    Public Property TelGerät As Telefoniegerät
        Get
            Return _TelGerät
        End Get
        Set
            SetProperty(_TelGerät, Value)
        End Set
    End Property

    Private _TelNr As Telefonnummer
    ''' <summary>
    ''' Telefonnummer, die gewählt werden soll.
    ''' </summary>
    Public Property TelNr As Telefonnummer
        Get
            Return _TelNr
        End Get
        Set
            SetProperty(_TelNr, Value)

            Dial(_TelNr)
        End Set
    End Property

    Private _CLIR As Boolean
    Public Property CLIR As Boolean
        Get
            Return _CLIR
        End Get
        Set
            SetProperty(_CLIR, Value)
        End Set
    End Property

    Private _Status As String
    Public Property Status As String
        Get
            Return _Status
        End Get
        Set
            SetProperty(_Status, Value)
        End Set
    End Property

    Private _IsDialing As Boolean
    Public Property IsDialing As Boolean
        Get
            Return _IsDialing
        End Get
        Set
            SetProperty(_IsDialing, Value)

            OnPropertyChanged(NameOf(IsNotDialing))
        End Set
    End Property

    Public ReadOnly Property IsNotDialing As Boolean
        Get
            Return Not _IsDialing
        End Get
    End Property
#End Region

#Region "SetProperties"
    Friend WriteOnly Property SetOutlookKontakt As Outlook.ContactItem
        Set
            ContactDialVM.OKontakt = Value
        End Set
    End Property
    Friend WriteOnly Property SetOutlookExchangeUser As Outlook.ExchangeUser
        Set
            ContactDialVM.OExchangeNutzer = Value
        End Set
    End Property
    Friend WriteOnly Property SetOutlookFBoxXMLKontakt As TR064.FritzBoxXMLKontakt
        Set
            ContactDialVM.FBoxXMLKontakt = Value
        End Set
    End Property
    Friend WriteOnly Property SetTelNr As Telefonnummer
        Set
            DirectDialVM.TelNr = Value

            Name = String.Format(Localize.LocWählclient.strHeader, Value.Formatiert)
        End Set
    End Property
#End Region

#Region "ICommand"
    Public Property CancelCommand As RelayCommand
    Public Property DialCommand As RelayCommand
#End Region

    Public Sub New()
        ' Commands
        CancelCommand = New RelayCommand(AddressOf CancelCall)
        DialCommand = New RelayCommand(AddressOf Dial, AddressOf CanDial)
        ' Interface
        DatenService = New DialService
        DialogService = New DialogService
    End Sub

    Private Sub SetData()
        ' Setze ViewModel
        If IsContactDial Then
            ContactDialVM = New ContactDialViewModel(Me, DatenService, Instance)
            ' CurrentView zuweisen
            CurrentView = ContactDialVM

        Else
            DirectDialVM = New DirectDialViewModel(Me, DatenService)
            ' CurrentView zuweisen
            CurrentView = DirectDialVM

        End If

        ' Lade Telefoniegeräte
        DialDeviceList.AddRange(DatenService.GetDialabePhones)

        ' Lade selektiertes Gerät
        TelGerät = DatenService.GetSelectedPhone

        ' Setze CLIR
        CLIR = DatenService.GetCLIR

        ' Wenn das TelGerät Nothing sein sollte, dann nimm das erste in der Auflistung DialDeviceList
        If TelGerät Is Nothing And DialDeviceList.Count.IsNotZero Then TelGerät = DialDeviceList.First

    End Sub

#Region "ICommand Callback"
    Private Async Sub CancelCall(o As Object)
        ' initialen Abbruch.Status setzen
        Status = Localize.LocWählclient.strStatusHangUp

        ' Breche den Wahlvorgang ab
        IsDialing = Not Await Wählclient.DialTelNr(Nothing, TelGerät, CLIR, True)

    End Sub

    Private Function CanDial(o As Object) As Boolean

        If TelGerät IsNot Nothing Then
            Select Case True
                Case TypeOf o Is String
                    Return CStr(o).IsNotStringNothingOrEmpty

                Case TypeOf o Is Telefonnummer
                    Return CType(o, Telefonnummer) IsNot Nothing

                Case Else
                    Return False

            End Select
        Else
            Return False
        End If

    End Function
    Private Async Sub Dial(o As Object)

        ' Telefonnummernobjekt generieren
        Dim DialTelNr As Telefonnummer

        Select Case True
            Case TypeOf o Is String
                DialTelNr = New Telefonnummer With {.SetNummer = CStr(o)}

            Case TypeOf o Is Telefonnummer
                DialTelNr = CType(o, Telefonnummer)

            Case Else
                DialTelNr = Nothing

        End Select

        ' Wenn es keine Mobilnummer ist
        ' Wenn es eine Mobilnummer ist und nicht gefragt werden muss
        ' wenn es eine Mobilnummer ist und aie Antwort Ja is
        If Not DialTelNr.IstMobilnummer OrElse Not DatenService.GetMobil OrElse
               DialogService.ShowMessageBox(String.Format(Localize.LocWählclient.strQMobil, DialTelNr.Formatiert)) = Windows.MessageBoxResult.Yes Then

            ' Ja
            IsDialing = True

            ' initialen Status setzen
            Status = Localize.LocWählclient.strStatusWait

            ' Wählvorgang einleiten
            If Wählclient IsNot Nothing And DialTelNr IsNot Nothing Then

                IsDialing = Await Wählclient.DialTelNr(DialTelNr, TelGerät, CLIR, False)

                Status = If(IsDialing, Localize.LocWählclient.strStatusPickUp, Localize.LocWählclient.strStatusError)

            End If

        Else
            IsDialing = False
            ' Nein
        End If

    End Sub
#End Region

End Class
