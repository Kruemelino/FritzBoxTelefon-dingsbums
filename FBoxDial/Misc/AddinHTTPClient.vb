﻿Imports System.Threading.Tasks
Imports System.Net.Http
Imports System.Security
Imports HttpClientFactoryLite

''' <summary>
''' Klasse für einen httpClient. Wird aktuell verwendet für Tellows, Rückwärtssuche
''' </summary>
Friend Class AddinHTTPClient
    Implements IDisposable
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property RegisteredClientHandler As Dictionary(Of String, HttpClientHandler)
    Private ReadOnly Property ClientFactory As IHttpClientFactory

    Friend Sub New()

        ClientFactory = New HttpClientFactory

        RegisteredClientHandler = New Dictionary(Of String, HttpClientHandler)

    End Sub

    Friend Sub RegisterClient(Key As String, ClientHandler As HttpClientHandler)
        ' Proxy generell ausschalten
        ClientHandler.UseProxy = False

        ' Ist bereits ein Eintrag mit diesem Key enthalten?
        If RegisteredClientHandler.Keys.Contains(Key) Then
            NLogger.Debug($"Bereits registrierter Client mit Key '{Key}' aktualisiert ({RegisteredClientHandler.Count})")

            ' Überscheibe den vorhandenen ClientHandler
            RegisteredClientHandler(Key) = ClientHandler

        Else
            ' Füge den neuen Clienthandler in das Dictionary 
            RegisteredClientHandler.Add(Key, ClientHandler)

            ClientFactory.Register(Key, Function(O) O.ConfigurePrimaryHttpMessageHandler(Function() RegisteredClientHandler(Key)))


            NLogger.Debug($"Client mit Key '{Key}' registriert ({RegisteredClientHandler.Count})")
        End If

    End Sub

#Region "GET"
    ''' <summary>
    ''' Lädt die angeforderte Ressource als <see cref="String"/> synchron herunter. Die herunterzuladende Ressource ist als <see cref="Uri"/> angegeben.
    ''' </summary>
    Friend Async Function GetString(ClientKey As String, RequestMessage As HttpRequestMessage, ZeichenCodierung As Encoding) As Task(Of String)
        Dim Response As String = String.Empty

        With Await ClientGetCore(ClientKey, RequestMessage)
            If .IsSuccessStatusCode Then
                Try
                    Dim buffer = Await .Content.ReadAsByteArrayAsync()
                    Response = ZeichenCodierung.GetString(buffer, 0, buffer.Length)

                    NLogger.Trace($"Get: '{RequestMessage.RequestUri.AbsoluteUri}'; Resonse: {Response}")
                Catch ex As Exception
                    NLogger.Error(ex, $"HttpClient Response nicht lesbar: {RequestMessage.RequestUri.AbsoluteUri}")
                End Try
            Else
                NLogger.Warn($"HttpClient nicht erfolgreich: StatusCode: { .StatusCode}, ReasonPhrase: '{ .ReasonPhrase}' bei {RequestMessage.RequestUri.AbsoluteUri}")
            End If
        End With

        Return Response
    End Function

    Private Async Function ClientGetCore(ClientKey As String, RequestMessage As HttpRequestMessage) As Task(Of HttpResponseMessage)

        With ClientFactory.CreateClient(ClientKey)
            Try
                Return Await .SendAsync(RequestMessage)
            Catch ex As ArgumentNullException
                ' RequestMessage is Nothing
                NLogger.Error(ex, RequestMessage.RequestUri.AbsoluteUri)
            Catch ex As InvalidOperationException
                ' Der requestUri muss ein absoluter URI sein, oder BaseAddress muss festgelegt werden.
                NLogger.Error(ex, RequestMessage.RequestUri.AbsoluteUri)
            Catch ex As HttpRequestException
                ' Die Anforderung konnte wg. eines zugrunde liegenden Problems wie Netzwerkkonnektivität, DNS-Fehler, Überprüfung des Serverzertifikats (oder Timeout – nur .NET Framework) nicht durchgeführt werden.
                NLogger.Error(ex, RequestMessage.RequestUri.AbsoluteUri)
            Catch ex As TaskCanceledException
                ' Nur .NET Core und .NET 5 und höher: Die Anforderung ist aufgrund eines Timeouts fehlgeschlagen.
                NLogger.Error(ex, RequestMessage.RequestUri.AbsoluteUri)
            End Try
        End With

        ' Standard-Fehler
        Return New HttpResponseMessage(Net.HttpStatusCode.BadRequest) 
    End Function

    Friend Async Function GetStream(ClientKey As String, RequestUri As Uri) As Task(Of IO.Stream)

        With ClientFactory.CreateClient(ClientKey)
            Try
                Return Await .GetStreamAsync(RequestUri)
            Catch ex As ArgumentNullException
                ' RequestMessage is Nothing
                NLogger.Error(ex, RequestUri.AbsoluteUri)
            Catch ex As HttpRequestException
                ' Die Anforderung konnte wg. eines zugrunde liegenden Problems wie Netzwerkkonnektivität, DNS-Fehler, Überprüfung des Serverzertifikats (oder Timeout – nur .NET Framework) nicht durchgeführt werden.
                NLogger.Error(ex, RequestUri.AbsoluteUri)
            End Try
        End With

        Return Nothing
    End Function

#End Region

#Region "POST"
    Friend Async Function PostString(ClientKey As String, UniformResourceIdentifier As Uri, Content As HttpContent, ZeichenCodierung As Encoding) As Task(Of String)
        Dim Response As String = String.Empty

        With Await ClientPostCore(ClientKey, UniformResourceIdentifier, Content)
            If .IsSuccessStatusCode Then
                Try
                    Dim buffer = Await .Content.ReadAsByteArrayAsync()
                    Response = ZeichenCodierung.GetString(buffer, 0, buffer.Length)

                    NLogger.Trace($"Post: '{UniformResourceIdentifier.AbsoluteUri}'; Resonse: {Response}")
                Catch ex As Exception
                    NLogger.Error(ex, $"HttpClient Response nicht lesbar: {UniformResourceIdentifier.AbsoluteUri}")
                End Try
            Else
                NLogger.Warn($"HttpClient nicht erfolgreich: StatusCode: { .StatusCode}, ReasonPhrase: '{ .ReasonPhrase}' bei {UniformResourceIdentifier.AbsoluteUri}")
            End If
        End With

        Return Response
    End Function

    Private Async Function ClientPostCore(ClientKey As String, UniformResourceIdentifier As Uri, Content As HttpContent) As Task(Of HttpResponseMessage)

        With ClientFactory.CreateClient(ClientKey)
            Try
                Return Await .PostAsync(UniformResourceIdentifier, Content)
            Catch ex As InvalidOperationException
                ' Der requestUri muss ein absoluter URI sein, oder BaseAddress muss festgelegt werden.
                NLogger.Warn(ex, UniformResourceIdentifier.AbsoluteUri)
            Catch ex As HttpRequestException
                ' Die Anforderung konnte wg. eines zugrunde liegenden Problems wie Netzwerkkonnektivität, DNS-Fehler, Überprüfung des Serverzertifikats oder Timeout nicht durchgeführt werden.
                NLogger.Warn(ex, UniformResourceIdentifier.AbsoluteUri)
            Catch ex As TaskCanceledException
                ' .NET Core und .NET 5 und höher nur: Die Anforderung ist aufgrund eines Timeouts fehlgeschlagen.
                NLogger.Warn(ex, UniformResourceIdentifier.AbsoluteUri)
            End Try
        End With

        ' Standard-Fehler
        Return New HttpResponseMessage(Net.HttpStatusCode.BadRequest)
    End Function
#End Region

#Region "Authentication"
    '''' <summary>
    '''' Ermittelt den Header Parameter für die Digest Authentification<br/>
    '''' <see href="link">https://en.wikipedia.org/wiki/Digest_access_authentication</see><br/>
    '''' <see href="link">https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/WWW-Authenticate</see>
    '''' </summary>
    '''' <param name="Uri">URI für den Zugriff</param>
    '''' <param name="Method">Methode</param>
    '''' <param name="UserName">Benutzername für den Zugriff</param>
    '''' <param name="Password">Passwort des Users als <see cref="SecureString"/></param>
    '''' <param name="AuthenticateHeader">WWW-Authenticate Header</param>
    'Private Function DigestHeaderParameter(Uri As Uri,
    '                                       Method As HttpMethod,
    '                                       UserName As String,
    '                                       Password As String,
    '                                       EncryptionKey As String,
    '                                       EntityBody As String,
    '                                       AuthenticateHeader As WWWAuthenticatorHeader) As String

    '    Dim NonceCount As Integer = 1
    '    Dim ClientNonce As String = New Random().[Next](123400, 9999999).ToString()

    '    Using Crypter As New Rijndael
    '        With Crypter

    '            ' Der Präfix wird vorangestellt.
    '            Dim HA1 As String = .SecureStringToHash(.DecryptString(Password, EncryptionKey),
    '                                                    Encoding.UTF8,
    '                                                    AuthenticateHeader.AlgorithmName,
    '                                                    $"{UserName}:{AuthenticateHeader.Realm}:")

    '            If AuthenticateHeader.IsSessionAuth Then
    '                HA1 = .StringToHash($"{HA1}:{AuthenticateHeader.Nonce}:{ClientNonce}",
    '                                             AuthenticateHeader.AlgorithmName,
    '                                             Encoding.UTF8)
    '            End If

    '            NLogger.Debug($"HA1: {HA1} (für {Uri.AbsoluteUri} )")

    '            Dim A2 As String = $"{Method}:{Uri.AbsolutePath}"
    '            If AuthenticateHeader.IsIntegrityProtection Then A2 += $":{ .StringToHash(EntityBody, AuthenticateHeader.AlgorithmName, Encoding.UTF8)}"

    '            Dim HA2 As String = .StringToHash(A2, AuthenticateHeader.AlgorithmName, Encoding.UTF8)
    '            NLogger.Debug($"HA2: {HA2} (für {Uri.AbsoluteUri} )")

    '            Dim Response As String = .StringToHash($"{HA1}:{AuthenticateHeader.Nonce}:{NonceCount:00000000}:{ClientNonce}:{AuthenticateHeader.QoP}:{HA2}",
    '                                                  AuthenticateHeader.AlgorithmName,
    '                                                  Encoding.UTF8)

    '            NLogger.Debug($"Response: {Response} (für {Uri.AbsoluteUri} )")

    '            If AuthenticateHeader.Userhash Then
    '                UserName = .StringToHash($"{UserName}:{AuthenticateHeader.Realm}", AuthenticateHeader.AlgorithmName, Encoding.UTF8)
    '            End If

    '            Return AuthenticateHeader.GetClientResponseHeader(UserName, Response, ClientNonce, NonceCount, Uri.AbsolutePath)
    '        End With
    '    End Using

    'End Function

    'Private Function BasicHeaderParameter(UserName As String,
    '                                      Password As String,
    '                                      EncryptionKey As String) As String
    '    Using Crypter As New Rijndael
    '        With Crypter
    '            ' Der Präfix wird vorangestellt.
    '            Return .SecureStringToBase64String(.DecryptString(Password, EncryptionKey), Encoding.UTF8, $"{UserName}:")
    '        End With

    '    End Using
    'End Function

    'Private Function CloneBeforeContentSet(req As HttpRequestMessage) As HttpRequestMessage
    '    Dim clone As New HttpRequestMessage(req.Method, req.RequestUri) With {.Content = req.Content,
    '                                                                          .Version = req.Version}

    '    For Each prop As KeyValuePair(Of String, Object) In req.Properties
    '        clone.Properties.Add(prop)
    '    Next

    '    For Each header As KeyValuePair(Of String, IEnumerable(Of String)) In req.Headers
    '        clone.Headers.TryAddWithoutValidation(header.Key, header.Value)
    '    Next

    '    Return clone
    'End Function
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                RegisteredClientHandler.Values.ToList.ForEach(Sub(H) H.Dispose())
                RegisteredClientHandler.Clear()

            End If
            disposedValue = True
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
