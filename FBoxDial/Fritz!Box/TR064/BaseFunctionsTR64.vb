Imports System.Net
Imports System.Xml

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
                                FritzBoxGet = True

                            Catch exANE As ArgumentNullException
                                NLogger.Error(exANE)
                                FritzBoxGet = False

                            Catch exWE As WebException
                                FritzBoxGet = False
                                NLogger.Error(exWE, $"Link: {UniformResourceIdentifier.AbsoluteUri}")

                            End Try
                        End With
                    End Using
                Case Else
                    FritzBoxGet = False
                    NLogger.Warn($"Uri.Scheme: {UniformResourceIdentifier.Scheme}")

            End Select
        Else
            FritzBoxGet = False
            NLogger.Warn($"Ping zur Fritz!Box '{UniformResourceIdentifier.Host}'  nicht erfolgreich")
        End If
    End Function

    Friend Function FritzBoxPOST(UniformResourceIdentifier As Uri, SOAPAction As String, ServiceType As String, SOAPXML As XmlDocument, ByRef Response As String) As Boolean

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

                ' Zugangsdaten felstlegen
                Using Crypter As New Rijndael
                    ' Wenn der UserName leer ist muss der Default-Wert ermittelt werden.
                    .Credentials = New NetworkCredential(If(XMLData.POptionen.TBBenutzer.IsStringNothingOrEmpty, FritzBoxDefault.DfltFritzBoxUser, XMLData.POptionen.TBBenutzer), Crypter.DecryptString128Bit(XMLData.POptionen.TBPasswort, DefaultWerte.DfltDeCryptKey))
                End Using

                Try
                    Response = .UploadString(UniformResourceIdentifier, SOAPXML.InnerXml)
                    FritzBoxPOST = True

                Catch ex As WebException When ex.Message.Contains("606")
                    Response = $"TR-064 Interner-Fehler 606: {SOAPAction} ""Action not authorized"""
                    NLogger.Error(ex, Response)
                    FritzBoxPOST = False

                Catch ex As WebException When ex.Message.Contains("500")
                    Response = $"TR-064 Interner-Fehler 500: {SOAPAction}"
                    NLogger.Error(ex, Response)
                    FritzBoxPOST = False

                Catch ex As WebException When ex.Message.Contains("713")
                    Response = $"TR-064 Interner-Fehler 713: {SOAPAction} ""Invalid array index"""
                    NLogger.Error(ex, Response)
                    FritzBoxPOST = False

                Catch ex As WebException When ex.Message.Contains("820")
                    Response = $"TR-064 Interner-Fehler 820: {SOAPAction} ""Internal Error """
                    NLogger.Error(ex)
                    FritzBoxPOST = False

                Catch ex As WebException When ex.Message.Contains("401")
                    Response = $"TR-064 Login-Fehler 401: {SOAPAction} ""Unauthorized"""
                    NLogger.Error(ex, Response)
                    FritzBoxPOST = False

                Catch exWE As WebException
                    Response = $"WebException: {exWE.Message}"
                    NLogger.Error(exWE, $"Link: {SOAPAction}")
                    FritzBoxPOST = False

                Catch ex As Exception
                    Response = ex.Message
                    NLogger.Error(ex)
                    FritzBoxPOST = False
                End Try
            End With
        End Using


    End Function


#End Region

End Module