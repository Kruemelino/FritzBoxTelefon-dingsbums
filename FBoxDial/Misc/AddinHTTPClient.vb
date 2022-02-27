Imports System.Threading.Tasks
Imports System.Net.Http
Imports HttpClientFactoryLite

''' <summary>
''' Klasse für einen httpClient. Wird aktuell verwendet für Tellows, Rückwärtssuche
''' </summary>
Friend Class AddinHTTPClient
    Implements IDisposable

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private ReadOnly Property ClientFactory As IHttpClientFactory

    Friend Sub New()

        ClientFactory = New HttpClientFactory

        ClientFactory.Register("FritzBoxDial",
                               Function(C) C.ConfigurePrimaryHttpMessageHandler(Function() New HttpClientHandler With {.UseProxy = False}))

    End Sub

#Region "GET"
    ''' <summary>
    ''' Lädt die angeforderte Ressource als <see cref="String"/> synchron herunter. Die herunterzuladende Ressource ist als <see cref="Uri"/> angegeben.
    ''' </summary>
    ''' <param name="UniformResourceIdentifier">Ein <see cref="Uri"/>-Objekt, das den herunterzuladenden URI enthält.</param>
    Friend Async Function GetString(UniformResourceIdentifier As Uri, ZeichenCodierung As Encoding) As Task(Of String)

        Dim RequestMessage As New HttpRequestMessage With {.Method = HttpMethod.Get,
                                                           .RequestUri = UniformResourceIdentifier}

        Return Await GetString(RequestMessage, ZeichenCodierung)
    End Function

    Friend Async Function GetString(RequestMessage As HttpRequestMessage, ZeichenCodierung As Encoding) As Task(Of String)
        Dim Response As String = String.Empty

        Dim ResponseMessage As HttpResponseMessage = Await ClientGetCore(RequestMessage)

        If ResponseMessage?.IsSuccessStatusCode Then
            Try
                Dim buffer = Await ResponseMessage.Content.ReadAsByteArrayAsync()
                Response = ZeichenCodierung.GetString(buffer, 0, buffer.Length)

                NLogger.Trace($"Get: '{RequestMessage.RequestUri.AbsoluteUri}'; Resonse: {Response}")
            Catch ex As Exception
                NLogger.Error(ex, $"HttpClient Response nicht lesbar: {RequestMessage.RequestUri.AbsoluteUri}")
            End Try
        Else
            NLogger.Warn($"HttpClient nicht erfolgreich: StatusCode: {ResponseMessage.StatusCode}, ReasonPhrase: '{ResponseMessage.ReasonPhrase}' bei {RequestMessage.RequestUri.AbsoluteUri}")
        End If
        Return Response
    End Function

    Private Async Function ClientGetCore(RequestMessage As HttpRequestMessage) As Task(Of HttpResponseMessage)

        Dim Client As HttpClient = ClientFactory.CreateClient("FritzBoxDial")

        Try
            Return Await Client.SendAsync(RequestMessage)
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

        Return Nothing
    End Function

    ''' <summary>
    ''' Lädt eine Datei auf das Dateisystem herunter. Wird momentan für Tellows benötigt.
    ''' </summary>
    ''' <param name="RequestMessage"></param>
    ''' <param name="DateiPfad"></param>
    Friend Async Function GetFile(RequestMessage As HttpRequestMessage, DateiPfad As String) As Task(Of Boolean)

        Dim ResponseMessage As HttpResponseMessage = Await ClientGetCore(RequestMessage)

        ' Speichere die Datei
        If ResponseMessage?.IsSuccessStatusCode Then
            Using s As IO.Stream = Await ResponseMessage.Content.ReadAsStreamAsync
                Dim fileinfo As New IO.FileInfo(DateiPfad)
                Using fs As IO.FileStream = fileinfo.OpenWrite
                    Await s.CopyToAsync(fs)
                End Using
            End Using
            Return True
        Else
            Return False
        End If
    End Function
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then : End If
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
