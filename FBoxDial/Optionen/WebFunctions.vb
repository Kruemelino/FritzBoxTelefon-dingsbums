Imports System.Net
Imports System.Threading.Tasks
Friend Module WebFunctions

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private ReadOnly Property DfltHeader As WebHeaderCollection = New WebHeaderCollection From {{HttpRequestHeader.KeepAlive, False.ToString}}

#Region "Netzwerkfunktionen"
    ''' <summary>
    ''' Führt einen Ping zur Gegenstelle aus.
    ''' </summary>
    ''' <param name="IPAdresse">IP-Adresse Netzwerkname der Gegenstelle. Rückgabe der IP-Adresse</param>
    ''' <returns>Boolean</returns>
    Friend Function Ping(ByRef IPAdresse As String) As Boolean
        Ping = False

        Dim IPHostInfo As IPHostEntry
        Dim PingSender As New NetworkInformation.Ping()
        Dim Options As New NetworkInformation.PingOptions() With {.DontFragment = True}
        Dim PingReply As NetworkInformation.PingReply = Nothing

        Dim buffer As Byte() = Encoding.ASCII.GetBytes(DfltStringEmpty)
        Dim timeout As Integer = 120

        Try
            PingReply = PingSender.Send(IPAdresse, timeout, buffer, Options)
        Catch ex As Exception
            NLogger.Warn(ex, $"Ping zu {IPAdresse} nicht erfolgreich")
            Ping = False
        End Try

        If PingReply IsNot Nothing Then
            With PingReply
                If .Status = NetworkInformation.IPStatus.Success Then
                    If .Address.AddressFamily = Sockets.AddressFamily.InterNetworkV6 Then
                        'Zugehörige IPv4 ermitteln
                        IPHostInfo = Dns.GetHostEntry(.Address)
                        For Each _IPAddress As IPAddress In IPHostInfo.AddressList
                            If _IPAddress.AddressFamily = Sockets.AddressFamily.InterNetwork Then
                                IPAdresse = _IPAddress.ToString
                                ' Prüfen ob es eine generel gültige lokale IPv6 Adresse gibt: fd00::2665:11ff:fed8:6086
                                ' und wie die zu ermitteln ist
                                NLogger.Info($"IPv6: { .Address}, IPv4: {IPAdresse}")
                                Exit For
                            End If
                        Next
                    Else
                        IPAdresse = .Address.ToString
                    End If
                    Ping = True
                Else
                    NLogger.Warn($"Ping zu '{IPAdresse}' nicht erfolgreich: { .Status}")
                    Ping = False
                End If
            End With
        End If
        PingSender.Dispose()
        'Options = Nothing
        'PingSender = Nothing
    End Function

    ''' <summary>
    ''' Wandelt die eingegebene IP-Adresse in eine für dieses Addin gültige IPAdresse.
    ''' IPv4 und IPv6 müssen differenziert behandelt werden.
    ''' Für Anrufmonitor ist es egal ob IPv4 oder IPv6 da der RemoteEndPoint ein IPAddress-Objekt verwendet.
    ''' Die HTML/URL müssen gesondert beachtet werden. Dafün muss die IPv6 in eckige Klammern gesetzt werden.
    ''' 
    ''' Möglicher Input:
    ''' IPv4: Nichts unternehmen
    ''' IPv6: 
    ''' String, der aufgelöst werden kann z.B. "fritz.box"
    ''' String, der nicht aufgelöst werden kann
    ''' </summary>
    ''' <param name="InputIP">IP-Adresse</param>
    ''' <returns>Korrekte IP-Adresse</returns>
    Friend Function ValidIP(InputIP As String) As String
        Dim IPAddresse As IPAddress = Nothing
        Dim IPHostInfo As IPHostEntry

        ValidIP = FritzBoxDefault.DfltFritzBoxAdress

        If IPAddress.TryParse(InputIP, IPAddresse) Then
            Select Case IPAddresse.AddressFamily
                Case Sockets.AddressFamily.InterNetworkV6
                    ValidIP = $"[{IPAddresse}]"
                Case Sockets.AddressFamily.InterNetwork
                    ValidIP = IPAddresse.ToString
                Case Else
                    NLogger.Warn($"Die IP '{InputIP}' kann nicht zugeordnet werden.")
                    ValidIP = InputIP
            End Select
        Else
            Try
                IPHostInfo = Dns.GetHostEntry(InputIP)
                For Each IPAddresse In IPHostInfo.AddressList
                    If IPAddresse.AddressFamily = Sockets.AddressFamily.InterNetwork Then
                        ValidIP = IPAddresse.ToString
                    End If
                Next
            Catch ex As Exception
                NLogger.Warn(ex, $"Die Adresse '{XMLData.POptionen.TBFBAdr}' kann nicht zugeordnet werden.")
                ValidIP = XMLData.POptionen.TBFBAdr
            End Try
        End If

    End Function
#Region "GET"
    Friend Function DownloadString(UniformResourceIdentifier As Uri, ByRef Response As String, Optional ZeichenCodierung As Encoding = Nothing, Optional Headers As WebHeaderCollection = Nothing) As Boolean

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Select Case UniformResourceIdentifier.Scheme
            Case Uri.UriSchemeHttp, Uri.UriSchemeHttps

                If Headers Is Nothing Then
                    Headers = DfltHeader
                Else
                    Headers.Add(DfltHeader)
                End If

                Using webClient As New WebClient With {.Proxy = Nothing,
                                                       .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache),
                                                       .Headers = Headers,
                                                       .Encoding = If(ZeichenCodierung, Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox))}
                    With webClient

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

    End Function

    Friend Async Function DownloadStringTaskAsync(Link As String, Optional ZeichenCodierung As Encoding = Nothing, Optional Headers As WebHeaderCollection = Nothing) As Task(Of String)
        Return Await DownloadStringTaskAsync(New Uri(Link), ZeichenCodierung, Headers)
    End Function

    Friend Async Function DownloadStringTaskAsync(UniformResourceIdentifier As Uri, Optional ZeichenCodierung As Encoding = Nothing, Optional Headers As WebHeaderCollection = Nothing) As Task(Of String)

        Dim retVal As String = DfltStringEmpty

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        If UniformResourceIdentifier.IsAbsoluteUri Or UniformResourceIdentifier.IsFile Then
            Select Case UniformResourceIdentifier.Scheme
                Case Uri.UriSchemeHttp, Uri.UriSchemeHttps

                    If Headers Is Nothing Then
                        Headers = DfltHeader
                    Else
                        Headers.Add(DfltHeader)
                    End If

                    Using webClient As New WebClient With {.Proxy = Nothing,
                                                           .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache),
                                                           .Headers = Headers,
                                                           .Encoding = If(ZeichenCodierung, Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox))}
                        With webClient

                            Try
                                retVal = Await .DownloadStringTaskAsync(UniformResourceIdentifier)
                                NLogger.Trace($"{UniformResourceIdentifier.AbsoluteUri} - {retVal}")

                            Catch exANE As ArgumentNullException
                                NLogger.Error(exANE)

                            Catch exWE As WebException
                                NLogger.Error(exWE, $"Link: {UniformResourceIdentifier.AbsoluteUri}")
                            Catch ex As Exception
                                Stop
                            End Try
                        End With
                    End Using

                Case Uri.UriSchemeFile

                    retVal = Await Task.Run(Function()
                                                Try
                                                    Return IO.File.ReadAllText(UniformResourceIdentifier.LocalPath)
                                                Catch ex As Exception
                                                    NLogger.Warn(ex, $"Lokale Datei {UniformResourceIdentifier.LocalPath} kann nicht gelesen werden.")
                                                    Return DfltStringEmpty
                                                End Try
                                            End Function)
                Case Else
                    NLogger.Warn($"Uri.Scheme: {UniformResourceIdentifier.Scheme}")
            End Select
        End If


        Return retVal
    End Function

    Friend Async Function DownloadDataTaskAsync(UniformResourceIdentifier As Uri, Optional ZeichenCodierung As Encoding = Nothing, Optional Headers As WebHeaderCollection = Nothing) As Task(Of Byte())

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Select Case UniformResourceIdentifier.Scheme
            Case Uri.UriSchemeHttp, Uri.UriSchemeHttps

                If Headers Is Nothing Then
                    Headers = DfltHeader
                Else
                    Headers.Add(DfltHeader)
                End If

                Using webClient As New WebClient With {.Proxy = Nothing,
                                                       .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache),
                                                       .Headers = Headers,
                                                       .Encoding = If(ZeichenCodierung, Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox))}
                    With webClient

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

    End Function

    Friend Async Function DownloadToFileTaskAsync(UniformResourceIdentifier As Uri, DateiName As String, Optional ZeichenCodierung As Encoding = Nothing, Optional Headers As WebHeaderCollection = Nothing) As Task(Of Boolean)

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Select Case UniformResourceIdentifier.Scheme
            Case Uri.UriSchemeHttp, Uri.UriSchemeHttps

                If Headers Is Nothing Then
                    Headers = DfltHeader
                Else
                    Headers.Add(DfltHeader)
                End If

                Using webClient As New WebClient With {.Proxy = Nothing,
                                                       .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache),
                                                       .Headers = Headers,
                                                       .Encoding = If(ZeichenCodierung, Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox))}
                    With webClient

                        Try
                            Await .DownloadFileTaskAsync(UniformResourceIdentifier, DateiName)
                            Return True
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
        Return False
    End Function
#End Region

#Region "POST"
    Friend Function UploadData(UniformResourceIdentifier As Uri, PostData As String, NC As NetworkCredential, ByRef Response As String, Optional Headers As WebHeaderCollection = Nothing, Optional ZeichenCodierung As Encoding = Nothing) As Boolean

        Response = DfltStringEmpty

        If Headers Is Nothing Then
            Headers = DfltHeader
        Else
            Headers.Add(DfltHeader)
        End If

        Using webClient As New WebClient With {.Credentials = NC,
                                               .Headers = Headers,
                                               .Encoding = If(ZeichenCodierung, Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox))}
            With webClient

                Try
                    Response = .UploadString(UniformResourceIdentifier, PostData)
                    Return True
                Catch ex As ArgumentException
                    ' Der address-Parameter ist null.
                    ' - oder -
                    ' Der Data - Parameter ist null.
                    NLogger.Error(ex)
                Catch ex As WebException
                    ' Der durch Kombinieren von BaseAddress und address gebildete URI ist ungültig.
                    ' - oder -
                    ' Der Server, der Host dieser Ressource ist, hat nicht geantwortet.
                    NLogger.Error(ex)
                End Try
            End With
        End Using

        Return False
    End Function
#End Region

#End Region

End Module
