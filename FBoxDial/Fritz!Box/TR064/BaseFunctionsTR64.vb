Imports System.Net
Imports System.Xml

Namespace SOAP

    Friend Module BaseFunctionsTR64
        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
#Region "HTTP"
        Friend Function FritzBoxGet(UniformResourceIdentifier As Uri, ByRef Response As String) As Boolean

            ' Ping zur Fritz!Box
            If Ping(UniformResourceIdentifier.Host) Then

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

                Select Case UniformResourceIdentifier.Scheme
                    Case Uri.UriSchemeHttp, Uri.UriSchemeHttps

                        Using webClient As New WebClient
                            With webClient
                                ' kein Proxy
                                .Proxy = Nothing

                                ' kein Cache
                                .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)

                                ' Header festlegen
                                .Headers.Add(HttpRequestHeader.KeepAlive, "False")

                                ' Zeichencodierung auf das Fritz!Box default setzen
                                .Encoding = Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox)

                                Try
                                    Response = .DownloadString(UniformResourceIdentifier)
                                    Return True

                                Catch exANE As ArgumentNullException
                                    NLogger.Error(exANE)
                                    Return False

                                Catch exWE As WebException
                                    NLogger.Error(exWE, $"Link: {UniformResourceIdentifier.AbsoluteUri}")
                                    Return False

                                End Try
                            End With
                        End Using
                    Case Else
                        NLogger.Warn($"Uri.Scheme: {UniformResourceIdentifier.Scheme}")
                        Return False

                End Select
            Else
                NLogger.Warn($"Ping zur Fritz!Box '{UniformResourceIdentifier.Host}'  nicht erfolgreich")
                Return False

            End If
        End Function

        Friend Async Function DownloadDataTaskAsync(UniformResourceIdentifier As Uri) As Threading.Tasks.Task(Of Byte())

            ' Ping zur Fritz!Box
            If Ping(UniformResourceIdentifier.Host) Then

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

                Select Case UniformResourceIdentifier.Scheme
                    Case Uri.UriSchemeHttp, Uri.UriSchemeHttps

                        Using webClient As New WebClient
                            With webClient
                                ' kein Proxy
                                .Proxy = Nothing

                                ' kein Cache
                                .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)

                                ' Header festlegen
                                .Headers.Add(HttpRequestHeader.KeepAlive, "False")

                                ' Zeichencodierung auf das Fritz!Box default setzen
                                .Encoding = Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox)

                                Try
                                    Return Await .DownloadDataTaskAsync(UniformResourceIdentifier)

                                Catch exANE As ArgumentNullException
                                    NLogger.Error(exANE)
                                    Return {}

                                Catch exWE As WebException
                                    Return {}
                                    NLogger.Error(exWE, $"Link: {UniformResourceIdentifier.AbsoluteUri}")

                                End Try
                            End With
                        End Using
                    Case Else
                        Return {}
                        NLogger.Warn($"Uri.Scheme: {UniformResourceIdentifier.Scheme}")

                End Select
            Else
                Return {}
                NLogger.Warn($"Ping zur Fritz!Box '{UniformResourceIdentifier.Host}'  nicht erfolgreich")
            End If
        End Function

        Friend Async Function DownloadToFileTaskAsync(UniformResourceIdentifier As Uri, DateiName As String) As Threading.Tasks.Task

            ' Ping zur Fritz!Box
            If Ping(UniformResourceIdentifier.Host) Then

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

                Select Case UniformResourceIdentifier.Scheme
                    Case Uri.UriSchemeHttp, Uri.UriSchemeHttps

                        Using webClient As New WebClient
                            With webClient
                                ' kein Proxy
                                .Proxy = Nothing

                                ' kein Cache
                                .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)

                                ' Header festlegen
                                .Headers.Add(HttpRequestHeader.KeepAlive, "False")

                                ' Zeichencodierung auf das Fritz!Box default setzen
                                .Encoding = Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox)

                                Try
                                    Await .DownloadFileTaskAsync(UniformResourceIdentifier, DateiName)

                                Catch exANE As ArgumentNullException
                                    NLogger.Error(exANE)

                                Catch exWE As WebException
                                    NLogger.Error(exWE, $"Link: {UniformResourceIdentifier.AbsoluteUri}")

                                Catch exIOE As InvalidOperationException
                                    NLogger.Error(exIOE)

                                End Try
                            End With
                        End Using
                    Case Else
                        NLogger.Warn($"Uri.Scheme: {UniformResourceIdentifier.Scheme}")

                End Select
            Else
                NLogger.Warn($"Ping zur Fritz!Box '{UniformResourceIdentifier.Host}'  nicht erfolgreich")
            End If
        End Function

        Friend Function FritzBoxPOST(UniformResourceIdentifier As Uri, SOAPAction As String, ServiceType As String, SOAPXML As XmlDocument, NC As NetworkCredential, ByRef Response As String) As Boolean

            Response = DfltStringEmpty

            Using webClient As New WebClient
                With webClient
                    ' Header festlegen
                    With .Headers
                        .Add(HttpRequestHeader.ContentType, TR064ContentType)
                        .Add(HttpRequestHeader.UserAgent, TR064UserAgent)
                        .Add(HttpRequestHeader.KeepAlive, False.ToString)
                        .Add("SOAPACTION", $"""{ServiceType}#{SOAPAction}""")
                    End With

                    ' Zeichencodierung auf das Fritz!Box default setzen
                    .Encoding = Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox)

                    ' Zugangsdaten festlegen. Es kann sein, dass ein Login nicht immer notwendig ist.
                    If NC IsNot Nothing Then .Credentials = NC

                    Try
                        Response = .UploadString(UniformResourceIdentifier, SOAPXML.InnerXml)
                        Return True

                    Catch ex As WebException When ex.Message.Contains("606")
                        Response = $"TR-064 Interner-Fehler 606: {SOAPAction} ""Action not authorized"""
                        NLogger.Error(ex, Response)

                    Catch ex As WebException When ex.Message.Contains("500")
                        Response = $"TR-064 Interner-Fehler 500: {SOAPAction}"
                        NLogger.Error(ex, Response)

                    Catch ex As WebException When ex.Message.Contains("713")
                        Response = $"TR-064 Interner-Fehler 713: {SOAPAction} ""Invalid array index"""
                        NLogger.Error(ex, Response)

                    Catch ex As WebException When ex.Message.Contains("820")
                        Response = $"TR-064 Interner-Fehler 820: {SOAPAction} ""Internal Error"""
                        NLogger.Error(ex)

                    Catch ex As WebException When ex.Message.Contains("401")
                        Response = $"TR-064 Login-Fehler 401: {SOAPAction} ""Unauthorized"""
                        NLogger.Error(ex, Response)

                    Catch exWE As WebException
                        Response = $"WebException: {exWE.Message}"
                        NLogger.Error(exWE, $"Action: {SOAPAction}")

                    Catch ex As Exception
                        Response = ex.Message
                        NLogger.Error(ex)

                    End Try
                End With
            End Using

            Return False
        End Function


#End Region

    End Module

End Namespace