Imports System.Threading.Tasks

Public Interface IIPPhoneConnector

    ''' <summary>
    ''' Typ des Connectors
    ''' </summary>
    ReadOnly Property Type As IPPhoneConnectorType

    ''' <summary>
    ''' ID des verbundenen Telefones
    ''' </summary>
    Property ConnectedPhoneID As Integer

    ''' <summary>
    ''' String zur Anwahl des Telefones
    ''' </summary>
    Property ConnectionUriCall As String
    Property ConnectionUriCancel As String

    Property AuthenticationRequired As Boolean
    'Property AuthenticationType As IPPhoneAuthType
    Property UserName As String
    Property Passwort As String

    ''' <summary>
    ''' Angabe, ob die Raute # an die zu wählende Nummer angehangen werden soll.
    ''' </summary>
    Property AppendSuffix As Boolean

    ReadOnly Property IPPhoneReady As Boolean
    Function Dial(DialCode As String, Hangup As Boolean) As Task(Of Boolean)

End Interface
