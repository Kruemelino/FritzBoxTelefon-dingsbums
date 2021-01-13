Imports System.Net
Imports System.IO
Imports System.Xml

Friend Module SOAPBaseFunctions
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
#Region "HTTP"
    Friend Function FritzBoxGet(ByVal Link As String, ByRef FBError As Boolean) As String
        Dim UniformResourceIdentifier As New Uri(Link)
        Dim retVal As String = DfltStringEmpty

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
                            retVal = .DownloadString(UniformResourceIdentifier)
                            FBError = False
                        Catch exANE As ArgumentNullException
                            FBError = True
                            NLogger.Error(exANE)
                        Catch exWE As WebException
                            FBError = True
                            NLogger.Error(exWE, "Link: {0}", Link)
                        End Try
                    End With
                End Using
            Case Else
                NLogger.Warn("Uri.Scheme: {0}", UniformResourceIdentifier.Scheme)
        End Select
        Return retVal
    End Function

    Friend Function FritzBoxPOST(ByVal SOAPAction As String, ByVal urlFull As String, ByVal ServiceType As String, ByVal SOAPXML As XmlDocument) As String

        FritzBoxPOST = DfltStringEmpty
        Dim ErrorText As String = DfltStringEmpty
        Dim fbURI As New Uri(urlFull)

        Using webClient As New WebClient
            With webClient
                ' Header festlegen
                With .Headers
                    .Add(HttpRequestHeader.ContentType, P_SOAPContentType)
                    .Add(HttpRequestHeader.UserAgent, P_SOAPUserAgent)
                    .Add(HttpRequestHeader.KeepAlive, False.ToString)
                    .Add("SOAPACTION", $"""{ServiceType}#{SOAPAction}""")
                End With

                ' Zeichencodierung auf das Fritz!Box default setzen
                .Encoding = Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox)

                ' Zugangsdaten felstlegen
                Using Crypter As New Rijndael
                    ' Wenn der UserName leer ist muss der Default-Wert ermittelt werden.
                    .Credentials = New NetworkCredential(If(XMLData.POptionen.TBBenutzer.IsStringEmpty, FritzBoxDefault.DfltFritzBoxUser, XMLData.POptionen.TBBenutzer), Crypter.DecryptString128Bit(XMLData.POptionen.TBPasswort, DefaultWerte.DfltDeCryptKey))
                End Using

                Try
                    FritzBoxPOST = .UploadString(fbURI, SOAPXML.InnerXml)
                Catch ex As WebException When ex.Message.Contains("606")
                    ErrorText = $"SOAP Interner-Fehler 606: {SOAPAction} ""Action not authorized"""
                    NLogger.Error(ex)

                Catch ex As WebException When ex.Message.Contains("500")
                    ErrorText = $"SOAP Interner-Fehler 500: {SOAPAction}"
                    NLogger.Error(ex)

                Catch ex As WebException When ex.Message.Contains("713")
                    ErrorText = $"SOAP Interner-Fehler 713: {SOAPAction} ""Invalid array index"""
                    NLogger.Error(ex)

                Catch ex As WebException When ex.Message.Contains("820")
                    ErrorText = $"SOAP Interner-Fehler 820: {SOAPAction} ""Internal Error """
                    NLogger.Error(ex)

                Catch ex As WebException When ex.Message.Contains("401")
                    ErrorText = $"SOAP Login-Fehler 401: {SOAPAction} ""Unauthorized"""
                    NLogger.Error(ex)

                Catch exWE As WebException
                    NLogger.Error(exWE, "Link: {0}", {SOAPAction})
                    ErrorText = $"WebException: {exWE.Message}"

                Catch ex As Exception
                    ErrorText = ex.Message
                    NLogger.Error(ex)

                End Try
            End With
        End Using

        If ErrorText.IsNotStringEmpty Then FritzBoxPOST = "<FEHLER>" & ErrorText.Replace("<", "CHR(60)").Replace(">", "CHR(62)") & "</FEHLER>"

    End Function
#End Region

    Friend Function GetSOAPXMLFile(ByVal Pfad As String) As XmlDocument
        Dim Fehler As Boolean = True
        Dim retVal As String
        Dim XMLFile As New XmlDocument

        retVal = FritzBoxGet(Pfad, Fehler)

        If Not Fehler Then
            XMLFile.LoadXml(retVal)
            GetSOAPXMLFile = XMLFile
        Else
            XMLFile.LoadXml("<FEHLER/>")
            GetSOAPXMLFile = XMLFile
        End If
    End Function

End Module