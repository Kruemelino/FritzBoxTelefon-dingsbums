Public Class OptIPPhoneViewModel
    Inherits NotifyBase

    Private Property DatenService As IOptionenService
    Private Property DialogService As IDialogService

    Public Sub New(dataService As IOptionenService, dialogService As IDialogService, sip As FBoxAPI.SIPClient)
        ' Interface
        _DatenService = dataService
        _DialogService = dialogService
        ' Commands

        ' Model
        IPPhoneItem = sip

        LadeTelefonDaten()
    End Sub

    Private _OptVM As OptionenViewModel
    Public Property OptVM As OptionenViewModel
        Get
            Return _OptVM
        End Get
        Set
            SetProperty(_OptVM, Value)

            ' Ermittle den zugehörigen Connector
            SetIPPhoneConnector()
        End Set
    End Property

#Region "Models"
    Public Property IPPhoneItem As FBoxAPI.SIPClient
#End Region

#Region "Properties"
    Public Property Numbers As New ObservableCollectionEx(Of OptNumberViewModel)

    Private _connViewModels As List(Of IConnectorVM)
    Public ReadOnly Property ConnectorViewModels As List(Of IConnectorVM)
        Get
            If _connViewModels Is Nothing Then _connViewModels = New List(Of IConnectorVM)()
            Return _connViewModels
        End Get
    End Property

    Private _ConnectorVM As IConnectorVM
    Public Property ConnectorVM As IConnectorVM
        Get
            Return _ConnectorVM
        End Get
        Set
            SetProperty(_ConnectorVM, Value)

            SetConnector()
        End Set
    End Property
#End Region

    ''' <summary>
    ''' Ermittle die Daten mit Hilfe der TR-064-Schnittstelle
    ''' </summary>
    Private Sub LadeTelefonDaten()
        ' OptVM liegt zu diesem Zeitpunkt noch nicht vor

        ' Connectoren hinzufügen
        With ConnectorViewModels
            .Add(New OptConnURIViewModel) '(DatenService, DialogService))
            .Add(New OptConnPhonerViewModel(DatenService, DialogService))
            .Add(New OptConnMicroSIPViewModel(DatenService, DialogService))
        End With

        ' Lade die eingehenden Nummern
        If IPPhoneItem.InComingNumbers.Any Then
            Numbers.AddRange(IPPhoneItem.InComingNumbers.Select(Function(Nr) New OptNumberViewModel(DatenService, DialogService, Nr)))
        End If

    End Sub

    Private Sub SetIPPhoneConnector()
        ' Ermittle den Connector, welches dieses Gerät zugeordnet ist
        If OptVM.IPPhoneConnectorList.Any Then
            Dim Liste As IEnumerable(Of IPPhoneConnector) = OptVM.IPPhoneConnectorList.Where(Function(IPP) IPP.ConnectedPhoneID.Equals(IPPhoneItem.ClientIndex))

            If Liste.Any Then
                ' ViewModel zuweisen
                _ConnectorVM = ConnectorViewModels.Where(Function(VM) VM.Connector.Type = Liste.First.Type).First

                ' Zuweisen
                With _ConnectorVM
                    .Connector = Liste.First
                    .OptVM = OptVM
                End With

            End If
        End If
    End Sub

    Private Sub SetConnector()
        If OptVM.IPPhoneConnectorList.Any Then
            ' Suche einen vorhandenen Connector für dieses Telefon in den Einstellungsdaten
            Dim Liste As IEnumerable(Of IPPhoneConnector) = OptVM.IPPhoneConnectorList.Where(Function(C) C.ConnectedPhoneID.AreEqual(IPPhoneItem.ClientIndex))

            ' Entferne alle vorhandenen Connectoren
            If Liste.Any Then OptVM.IPPhoneConnectorList.RemoveRange(Liste)

        End If

        If ConnectorVM IsNot Nothing Then
            With ConnectorVM
                .OptVM = OptVM
                .Connector.ConnectedPhoneID = IPPhoneItem.ClientIndex
            End With

            ' Füge den Connector zu den Einstellungsdaten hinzu
            OptVM.IPPhoneConnectorList.Add(ConnectorVM.Connector)
        End If

    End Sub

End Class
