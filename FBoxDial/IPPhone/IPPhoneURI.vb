Imports System.Net
Imports System.Threading.Tasks

Friend Module IPPhoneURI

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Async Function Dial(Connector As IIPPhoneConnector, DialCode As String, Hangup As Boolean) As Task(Of Boolean)

        If Connector IsNot Nothing AndAlso Connector.Type = IPPhoneConnectorType.URI Then

            Dim RequestMessage As New Http.HttpRequestMessage With {.Method = Http.HttpMethod.Get}

            ' Wählkommando vorbereiten
            If Hangup Then
                RequestMessage.RequestUri = New Uri(Connector.ConnectionUriCancel)

                NLogger.Debug(Localize.LocWählclient.strSoftPhoneAbbruch)
            Else

                If Connector.AppendSuffix Then DialCode += "#"

                RequestMessage.RequestUri = New Uri(Connector.ConnectionUriCall.Replace("{TelNr}", DialCode))

                NLogger.Debug(String.Format(Localize.LocWählclient.strSoftPhoneErfolgreich, DialCode, RequestMessage.RequestUri.AbsoluteUri))
            End If


            If Connector.AuthenticationRequired Then
                'Select Case Connector.AuthenticationType
                '    Case IPPhoneAuthType.Snom
                NLogger.Debug(Await Globals.ThisAddIn.FBoxhttpClient.GetStringWithAuth(RequestMessage, Encoding.UTF8, Connector.UserName, Connector.Passwort, My.Resources.strDfltAuthTestDeCryptKey))

                '        'Case IPPhoneAuthType.Grandstream


                'End Select

            Else
                ' Eine Authentifizierung ist nicht nötig
                NLogger.Debug(Await Globals.ThisAddIn.FBoxhttpClient.GetString(RequestMessage, Encoding.UTF8))

            End If

            ' Gib Rückmeldung, damit Wählclient kein Fehler ausgibt
            Return True
        Else
            Return False
        End If

    End Function

End Module
