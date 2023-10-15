Imports System.Xml.Serialization

<Serializable()>
Public Class IPPhoneConnector
    Inherits NotifyBase
    Implements IIPPhoneConnector

#Region "Eigenschaften"
    Public Property Type As IPPhoneConnectorType Implements IIPPhoneConnector.Type

    Public Property ConnectedPhoneID As Integer Implements IIPPhoneConnector.ConnectedPhoneID

    Private _Port As Integer
    Public Property Port As Integer Implements IIPPhoneConnector.Port
        Get
            Return _Port
        End Get
        Set
            SetProperty(_Port, Value)
        End Set
    End Property

    Private _ConnectionUriCall As String
    Public Property ConnectionUriCall As String Implements IIPPhoneConnector.ConnectionUriCall
        Get
            Return _ConnectionUriCall
        End Get
        Set
            SetProperty(_ConnectionUriCall, Value)
        End Set
    End Property

    Private _ConnectionUriCancel As String
    Public Property ConnectionUriCancel As String Implements IIPPhoneConnector.ConnectionUriCancel
        Get
            Return _ConnectionUriCancel
        End Get
        Set
            SetProperty(_ConnectionUriCancel, Value)
        End Set
    End Property

#Region "Authentication"
    Private _AuthenticationRequired As Boolean = True
    Public Property AuthenticationRequired As Boolean Implements IIPPhoneConnector.AuthenticationRequired
        Get
            Return _AuthenticationRequired
        End Get
        Set
            SetProperty(_AuthenticationRequired, Value)
        End Set
    End Property

    Private _UserName As String
    Public Property UserName As String Implements IIPPhoneConnector.UserName
        Get
            Return _UserName
        End Get
        Set
            SetProperty(_UserName, Value)
        End Set
    End Property

    Private _Passwort As String
    Public Property Passwort As String Implements IIPPhoneConnector.Passwort
        Get
            Return _Passwort
        End Get
        Set
            SetProperty(_Passwort, Value)
        End Set
    End Property
#End Region

    Private _AppendSuffix As Boolean
    Public Property AppendSuffix As Boolean Implements IIPPhoneConnector.AppendSuffix
        Get
            Return _AppendSuffix
        End Get
        Set
            SetProperty(_AppendSuffix, Value)
        End Set
    End Property

#End Region

    Friend Async Function Dial(DialCode As String, Hangup As Boolean) As Threading.Tasks.Task(Of Boolean) Implements IIPPhoneConnector.Dial
        Select Case Type
            Case IPPhoneConnectorType.Phoner
                Return Phoner.Dial(Me, DialCode, Hangup)

            Case IPPhoneConnectorType.PhonerLite
                Return PhonerLite.Dial(Me, DialCode, Hangup)

            Case IPPhoneConnectorType.MicroSIP
                Return MicroSIP.Dial(Me, DialCode, Hangup)

            Case IPPhoneConnectorType.URI
                Return Await IPPhoneURI.Dial(Me, DialCode, Hangup)

            Case Else
                Return False
        End Select
    End Function

End Class
