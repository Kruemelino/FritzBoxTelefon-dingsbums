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

            Dim httpClientKey As String = $"{Connector.Type}{Connector.ConnectedPhoneID:00}"

            If Connector.AuthenticationRequired Then

                Using Crypter As New Rijndael
                    With Globals.ThisAddIn.FBoxhttpClient
                        .RegisterClient(httpClientKey,
                                        New Http.HttpClientHandler With {.Credentials = New NetworkCredential(Connector.UserName, Crypter.DecryptString(Connector.Passwort, My.Resources.strDfltIPPhoneDeCryptKey))})
                    End With
                End Using

                NLogger.Debug(Await Globals.ThisAddIn.FBoxhttpClient.GetString(httpClientKey, RequestMessage, Encoding.UTF8))

            Else
                ' Eine Authentifizierung ist nicht nötig
                Globals.ThisAddIn.FBoxhttpClient.RegisterClient(httpClientKey, New Http.HttpClientHandler)

                NLogger.Debug(Await Globals.ThisAddIn.FBoxhttpClient.GetString(httpClientKey, RequestMessage, Encoding.UTF8))

            End If

            ' Gib Rückmeldung, damit Wählclient kein Fehler ausgibt
            Return True
        Else
            Return False
        End If

    End Function

End Module
