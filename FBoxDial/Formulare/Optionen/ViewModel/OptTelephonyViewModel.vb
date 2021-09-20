Public Class OptTelephonyViewModel
    Inherits NotifyBase
    Implements IPageViewModel
    Private Property DatenService As IOptionenService

#Region "Eigenschaften"
    Private _OptVM As OptionenViewModel
    Public Property OptVM As OptionenViewModel Implements IPageViewModel.OptVM
        Get
            Return _OptVM
        End Get
        Set
            SetProperty(_OptVM, Value)
        End Set
    End Property

    Public ReadOnly Property Name As String Implements IPageViewModel.Name
        Get
            Return Localize.LocOptionen.strPhone
        End Get
    End Property

    Public Property InitialSelected As Boolean = False Implements IPageViewModel.InitialSelected

    Private _IsAktiv As Boolean
    Public Property IsAktiv As Boolean
        Get
            Return _IsAktiv
        End Get
        Set
            SetProperty(_IsAktiv, Value)
            OnPropertyChanged(NameOf(IsNotAktiv))
        End Set
    End Property

    Public ReadOnly Property IsNotAktiv As Boolean
        Get
            Return Not _IsAktiv
        End Get
    End Property

    Private _EinlesenStatus As String
    Public Property EinlesenStatus As String
        Get
            Return _EinlesenStatus
        End Get
        Set
            SetProperty(_EinlesenStatus, Value)
        End Set
    End Property

#End Region

#Region "Eigenschaften TAM"
    Private _TAMListe As ObservableCollectionEx(Of TAMItem)
    Public Property TAMListe As ObservableCollectionEx(Of TAMItem)
        Get
            Return _TAMListe
        End Get
        Set
            SetProperty(_TAMListe, Value)
        End Set
    End Property

    Private _TAM As TAMItem
    Public Property TAM As TAMItem
        Get
            Return _TAM
        End Get
        Set
            SetProperty(_TAM, Value)
        End Set
    End Property
#End Region

#Region "ICommand"
    Public Property ImportCommand As RelayCommand
#End Region

    Public Sub New()
        ' Commands
        ImportCommand = New RelayCommand(AddressOf StartImport)

        ' Interface
        DatenService = New OptionenService


        TAMListe = New ObservableCollectionEx(Of TAMItem)
        TAMListe.AddRange(DatenService.GetTAMList.TAMListe)
    End Sub

    Private Sub StartImport(obj As Object)
        ' Speichern der Daten
        OptVM.Speichern()

        ' Deaktiviere den Button
        IsAktiv = True

        ' Ereignishandler hinzufügen
        AddHandler DatenService.Beendet, AddressOf FritzBoxDatenBeendet
        AddHandler DatenService.Status, AddressOf FritzBoxDatenStatus

        DatenService.StartImport()

    End Sub

    Private Sub FritzBoxDatenBeendet(sender As Object, e As NotifyEventArgs(Of Telefonie))

        ' Überführe die neu eingelesenen Daten in das Optionen-Viewmodel
        With e.Value

            ' Führe die neu eingelesenen Telefoniegeräte in das aktuelle Viewmodel
            OptVM.TelGeräteListe.Clear()
            OptVM.TelGeräteListe.AddRange(.Telefoniegeräte)

            ' Führe die neu eingelesenen Telefonnummern in das aktuelle Viewmodel
            OptVM.TelNrListe.Clear()
            OptVM.TelNrListe.AddRange(.Telefonnummern)

            ' Landeskennzahl (LKZ) übernehmen
            OptVM.TBLandesKZ = .LKZ

            'Ortskennzahl(OKZ) übernehmen
            OptVM.TBOrtsKZ = .OKZ

        End With

        ' Aktiviere den Button
        IsAktiv = False

        ' Ereignishandler entfernen
        RemoveHandler DatenService.Beendet, AddressOf FritzBoxDatenBeendet
        RemoveHandler DatenService.Status, AddressOf FritzBoxDatenStatus
    End Sub

    Private Sub FritzBoxDatenStatus(sender As Object, e As NotifyEventArgs(Of String))
        EinlesenStatus += e.Value & Environment.NewLine
    End Sub

End Class
