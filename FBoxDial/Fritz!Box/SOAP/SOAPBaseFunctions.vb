Imports System.Net
Imports System.IO
Imports System.Xml

Friend Module SOAPBaseFunctions

#Region "HTTP"
    Public Function FritzBoxGet(ByVal Link As String, ByRef FBError As Boolean) As String
        Dim UniformResourceIdentifier As New Uri(Link)
        Dim retVal As String = PDfltStringEmpty

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Select Case UniformResourceIdentifier.Scheme
            Case Uri.UriSchemeHttp, Uri.UriSchemeHttps

                Using webClient As New WebClient
                    With webClient
                        .Proxy = Nothing
                        .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)
                        .Headers.Add(HttpRequestHeader.KeepAlive, "False")
                        Try
                            retVal = .DownloadString(UniformResourceIdentifier)
                            FBError = False
                        Catch exANE As ArgumentNullException
                            FBError = True
                            LogFile("httpGET_WebClient: " & exANE.Message)
                        Catch exWE As WebException
                            FBError = True
                            LogFile("httpGET_WebClient: " & exWE.Message & " - Link: " & Link)
                        End Try
                    End With
                End Using
            Case Else
                LogFile("Uri.Scheme: " & UniformResourceIdentifier.Scheme)
        End Select
        Return retVal
    End Function

    Friend Function FritzBoxPOST(ByVal SOAPAction As String, ByVal urlFull As String, ByVal ServiceType As String, ByVal SOAPXML As String) As XmlDocument

        Dim RetVal As New XmlDocument

        Dim ErrorText As String = PDfltStringEmpty
        Dim fbPostBytes As Byte()

        Dim fbURI As New Uri(urlFull)

        Dim tmpUsername As String

        fbPostBytes = Encoding.UTF8.GetBytes(SOAPXML)

        ' Wenn der UserName leer ist muss der Default-Wert ermittelt werden.
        tmpUsername = If(XMLData.POptionen.PTBBenutzer.IsStringEmpty, FritzBoxDefault.PDfltFritzBoxUser, XMLData.POptionen.PTBBenutzer)

        With CType(WebRequest.Create(fbURI), HttpWebRequest)
            Using Crypter As New Rijndael
                .Credentials = New NetworkCredential(tmpUsername, Crypter.DecryptString128Bit(XMLData.POptionen.PTBPasswort))
            End Using

            .Proxy = Nothing
            .KeepAlive = False
            .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)
            .Method = WebRequestMethods.Http.Post
            .Headers.Add("SOAPACTION", """" + ServiceType + "#" + SOAPAction + """")
            .ContentType = P_SOAPContentType
            .UserAgent = P_SOAPUserAgent
            .ContentLength = fbPostBytes.Length

            With .GetRequestStream
                .Write(fbPostBytes, 0, fbPostBytes.Length)
                .Close()
            End With

            Try
                With New StreamReader(.GetResponse.GetResponseStream())
                    RetVal.LoadXml(.ReadToEnd())
                End With
            Catch ex As WebException When ex.Message.Contains("606")
                ErrorText = "SOAP Interner-Fehler 606: " & SOAPAction & """ Action not authorized"""
                'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
            Catch ex As WebException When ex.Message.Contains("500")
                ErrorText = "SOAP Interner-Fehler 500: " & SOAPAction & vbNewLine & vbNewLine & "Method: " & .Method.ToString & vbNewLine & "SOAPACTION: " & """" + ServiceType + "#" + SOAPAction + """" & vbNewLine & "ContentType: " & .ContentType.ToString & vbNewLine & "UserAgent: " & .UserAgent.ToString & vbNewLine & "ContentLength: " & .ContentLength.ToString & vbNewLine & vbNewLine & SOAPXML
                'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
            Catch ex As WebException When ex.Message.Contains("713")
                ErrorText = "SOAP Interner-Fehler 713: " & SOAPAction & """ Invalid array index"""
                'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
            Catch ex As WebException When ex.Message.Contains("820")
                ErrorText = "SOAP Interner-Fehler 820: " & SOAPAction & """ Internal error """
                'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
            Catch ex As WebException When ex.Message.Contains("401")
                ErrorText = "SOAP Login-Fehler 401: " & SOAPAction & """ Unauthorized"""
                'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
            End Try
        End With

        If Not ErrorText = "" Then RetVal.LoadXml("<FEHLER>" & ErrorText.Replace("<", "CHR(60)").Replace(">", "CHR(62)") & "</FEHLER>")
        Return RetVal
    End Function

    Friend Function FritzBoxPOSTClient(ByVal SOAPAction As String, ByVal urlFull As String, ByVal ServiceType As String, ByVal SOAPXML As String) As String

        Dim RetVal As String = ""

        Dim ErrorText As String = PDfltStringEmpty
        Dim fbPostBytes As Byte()

        Dim fbURI As New Uri(urlFull)

        Dim tmpUsername As String

        fbPostBytes = Encoding.UTF8.GetBytes(SOAPXML)

        ' Wenn der UserName leer ist muss der Default-Wert ermittelt werden.
        tmpUsername = If(XMLData.POptionen.PTBBenutzer.IsStringEmpty, FritzBoxDefault.PDfltFritzBoxUser, XMLData.POptionen.PTBBenutzer)
        Using webClient As New WebClient
            With webClient
                ' Header festlegen
                With .Headers
                    .Add(HttpRequestHeader.ContentType, P_SOAPContentType)
                    .Add(HttpRequestHeader.UserAgent, P_SOAPUserAgent)
                    .Add(HttpRequestHeader.KeepAlive, False.ToString)
                    .Add("SOAPACTION", """" + ServiceType + "#" + SOAPAction + """")
                End With
                ' Zugangsdaten felstelgen
                Using Crypter As New Rijndael
                    .Credentials = New NetworkCredential(tmpUsername, Crypter.DecryptString128Bit(XMLData.POptionen.PTBPasswort))
                End Using

                Try
                    RetVal = .UploadString(fbURI, SOAPXML)
                Catch ex As WebException When ex.Message.Contains("606")
                    ErrorText = "SOAP Interner-Fehler 606: " & SOAPAction & """ Action not authorized"""
                    'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
                Catch ex As WebException When ex.Message.Contains("500")
                    ErrorText = "SOAP Interner-Fehler 500: " & SOAPAction ' & vbNewLine & vbNewLine & "Method: " & .Method.ToString & vbNewLine & "SOAPACTION: " & """" + ServiceType + "#" + SOAPAction + """" & vbNewLine & "ContentType: " & .ContentType.ToString & vbNewLine & "UserAgent: " & .UserAgent.ToString & vbNewLine & "ContentLength: " & .ContentLength.ToString & vbNewLine & vbNewLine & SOAPXML
                    'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
                Catch ex As WebException When ex.Message.Contains("713")
                    ErrorText = "SOAP Interner-Fehler 713: " & SOAPAction & """ Invalid array index"""
                    'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
                Catch ex As WebException When ex.Message.Contains("820")
                    ErrorText = "SOAP Interner-Fehler 820: " & SOAPAction & """ Internal error """
                    'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
                Catch ex As WebException When ex.Message.Contains("401")
                    ErrorText = "SOAP Login-Fehler 401: " & SOAPAction & """ Unauthorized"""
                    'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
                Catch ex As Exception
                    ErrorText = ex.Message
                    MsgBox(ErrorText, MsgBoxStyle.Exclamation, "SOAP POST Client")
                End Try
            End With
        End Using

        If Not ErrorText = "" Then RetVal = "<FEHLER>" & ErrorText.Replace("<", "CHR(60)").Replace(">", "CHR(62)") & "</FEHLER>"
        Return RetVal
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