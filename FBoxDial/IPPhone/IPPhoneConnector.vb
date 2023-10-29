Imports System.Xml.Serialization

<Serializable()>
Public Class IPPhoneConnector
    Inherits NotifyBase
    Implements IIPPhoneConnector

#Region "Eigenschaften"
    <XmlElement()> Public Property Type As IPPhoneConnectorType Implements IIPPhoneConnector.Type

    <XmlElement()> Public Property ConnectedPhoneID As Integer Implements IIPPhoneConnector.ConnectedPhoneID

    Private _ConnectionUriCall As String
    <XmlElement()> Public Property ConnectionUriCall As String Implements IIPPhoneConnector.ConnectionUriCall
        Get
            Return _ConnectionUriCall
        End Get
        Set
            SetProperty(_ConnectionUriCall, Value)
        End Set
    End Property

    Private _ConnectionUriCancel As String
    <XmlElement()> Public Property ConnectionUriCancel As String Implements IIPPhoneConnector.ConnectionUriCancel
        Get
            Return _ConnectionUriCancel
        End Get
        Set
            SetProperty(_ConnectionUriCancel, Value)
        End Set
    End Property

#Region "Softphone CMD"
    Private _Name As String
    <XmlAttribute()> Public Property Name As String Implements IIPPhoneConnector.Name
        Get
            Return _Name
        End Get
        Set
            SetProperty(_Name, Value)
        End Set
    End Property

    Private _CommandCallTo As String
    <XmlElement()> Public Property CommandCallTo As String Implements IIPPhoneConnector.CommandCallTo
        Get
            Return _CommandCallTo
        End Get
        Set
            SetProperty(_CommandCallTo, Value)
        End Set
    End Property

    Private _CommandHangUp As String
    <XmlElement()> Public Property CommandHangUp As String Implements IIPPhoneConnector.CommandHangUp
        Get
            Return _CommandHangUp
        End Get
        Set
            SetProperty(_CommandHangUp, Value)
        End Set
    End Property
#End Region

#Region "Authentication"
    Private _AuthenticationRequired As Boolean = True
    <XmlElement()> Public Property AuthenticationRequired As Boolean Implements IIPPhoneConnector.AuthenticationRequired
        Get
            Return _AuthenticationRequired
        End Get
        Set
            SetProperty(_AuthenticationRequired, Value)
        End Set
    End Property

    Private _UserName As String
    <XmlElement()> Public Property UserName As String Implements IIPPhoneConnector.UserName
        Get
            Return _UserName
        End Get
        Set
            SetProperty(_UserName, Value)
        End Set
    End Property

    Private _Passwort As String
    <XmlElement()> Public Property Passwort As String Implements IIPPhoneConnector.Passwort
        Get
            Return _Passwort
        End Get
        Set
            SetProperty(_Passwort, Value)
        End Set
    End Property
#End Region

    Private _AppendSuffix As Boolean
    <XmlElement()> Public Property AppendSuffix As Boolean Implements IIPPhoneConnector.AppendSuffix
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

            Case IPPhoneConnectorType.CMD
                Return IPPhoneCMD.Dial(Me, DialCode, Hangup)

            Case IPPhoneConnectorType.URI
                Return Await IPPhoneURI.Dial(Me, DialCode, Hangup)

            Case Else
                Return False
        End Select
    End Function

End Class
