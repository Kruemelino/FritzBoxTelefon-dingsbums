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
    Property UserName As String
    Property Passwort As String

    ''' <summary>
    ''' Angabe, ob die Raute # an die zu wählende Nummer angehangen werden soll.
    ''' </summary>
    Property AppendSuffix As Boolean

    ''' <summary>
    ''' Funktion zum absetzen des Wählkomandos
    ''' </summary>
    ''' <param name="DialCode">Zu wählende Nummer</param>
    ''' <param name="Hangup">Angabe, ob das Wählen abgebrochen werden soll</param>
    Function Dial(DialCode As String, Hangup As Boolean) As Task(Of Boolean)

End Interface
