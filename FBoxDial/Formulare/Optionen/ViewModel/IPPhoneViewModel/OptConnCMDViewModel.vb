Imports System.Runtime.InteropServices.ComTypes

Public Class OptConnCMDViewModel
    Inherits NotifyBase
    Implements IConnectorVM

    Private Property DatenService As IOptionenService
    Private Property DialogService As IDialogService
    Public Property GetExecutableCommand As RelayCommand

    Public Sub New(dataService As IOptionenService, dialogService As IDialogService)
        ' Interface
        _DatenService = dataService
        _DialogService = dialogService
        ' Commands
        GetExecutableCommand = New RelayCommand(AddressOf GetExecutablePath)
    End Sub

    Private _OptVM As OptionenViewModel
    Public Property OptVM As OptionenViewModel Implements IConnectorVM.OptVM
        Get
            Return _OptVM
        End Get
        Set
            SetProperty(_OptVM, Value)
        End Set
    End Property

    Public ReadOnly Property Name As String Implements IConnectorVM.Name
        Get
            Return resEnum.IPConnCMD
        End Get
    End Property

    Public Property Connector As New IPPhoneConnector With {.Type = IPPhoneConnectorType.CMD} Implements IConnectorVM.Connector

    Private _SelectedSoftPhone As SoftPhones
    Public Property SelectedSoftPhone As SoftPhones
        Get
            Return _SelectedSoftPhone
        End Get
        Set
            SetProperty(_SelectedSoftPhone, Value)

            With Connector
                ' Setze die bekannten Daten für PhonerLite, MicroSIP etc.
                Select Case _SelectedSoftPhone
                    Case SoftPhones.PhonerLite
                        .CommandCallTo = "callto:"
                        .CommandHangUp = "hangup:"
                        .Name = SoftPhones.PhonerLite.ToString

                    Case SoftPhones.MicroSIP
                        .CommandCallTo = String.Empty
                        .CommandHangUp = "/hangupall:"
                        .Name = SoftPhones.MicroSIP.ToString
                    Case Else
                        .CommandCallTo = String.Empty
                        .CommandHangUp = String.Empty
                        .Name = String.Empty

                End Select
            End With
        End Set
    End Property

    Private Sub GetExecutablePath(o As Object)
        ' Initialen Pfad ermitteln
        Dim InitialDirectory As String = String.Empty

        If Connector.ConnectionUriCall.IsStringNothingOrEmpty And Connector.UserName.IsNotStringNothingOrEmpty Then
            ' Ermittle den Pfad anhand des Prozessnamens
            ' TODO: Was passiert, wenn Prozess nicht läuft
            InitialDirectory = DatenService.SoftPhoneGetExecutablePath(Connector.Name)
        Else
            ' Ein Pfad ist im Connector hinterlegt.
            InitialDirectory = Connector.ConnectionUriCall
        End If

        Dim Dateipfad As String = DialogService.OpenFile(".exe (.exe)|*.exe", InitialDirectory)
        If Dateipfad.IsNotStringNothingOrEmpty Then Connector.ConnectionUriCall = Dateipfad
    End Sub
End Class
