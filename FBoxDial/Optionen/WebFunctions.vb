Imports System.Net
Imports System.Threading.Tasks
Friend Module WebFunctions

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Private Const DefaultHeaderKeepAlive As Boolean = False
    Private Const DefaultHeaderUserAgent As String = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko"
    Private Const DefaultMinTimout As Integer = 100

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

        Dim buffer As Byte() = Encoding.ASCII.GetBytes(String.Empty)

        Try
            PingReply = PingSender.Send(IPAdresse, Math.Max(XMLData.POptionen.TBNetworkTimeout, DefaultMinTimout), buffer, Options)
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
    ''' <summary>
    ''' Lädt die angeforderte Ressource als <see cref="String"/> synchron herunter. Die herunterzuladende Ressource ist als <see cref="Uri"/> angegeben.
    ''' </summary>
    ''' <param name="UniformResourceIdentifier">Ein <see cref="Uri"/>-Objekt, das den herunterzuladenden URI enthält.</param>
    ''' <param name="Response">Ein <see cref="String"/> mit der angeforderten Ressource.</param>
    ''' <param name="ZeichenCodierung">(Optional) Legt die <see cref="Encoding"/> für den Download von Zeichenfolgen fest.</param>
    ''' <param name="Headers">(Optional) Zusätzliche Header für den Download von Zeichenfolgen</param>
    ''' <returns>Boolean, je nach Erfolg der Abfrage.</returns>
    Friend Function DownloadString(UniformResourceIdentifier As Uri, ByRef Response As String, Optional ZeichenCodierung As Encoding = Nothing, Optional Headers As WebHeaderCollection = Nothing) As Boolean

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Select Case UniformResourceIdentifier.Scheme
            Case Uri.UriSchemeHttp, Uri.UriSchemeHttps

                Using webClient As New WebClient With {.Proxy = Nothing,
                                                       .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache),
                                                       .Encoding = If(ZeichenCodierung, Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox))}
                    With webClient

                        With .Headers
                            .Set(HttpRequestHeader.UserAgent, DefaultHeaderUserAgent)
                            .Set(HttpRequestHeader.KeepAlive, DefaultHeaderKeepAlive.ToString)
                            
                            If Headers IsNot Nothing Then .Add(Headers)
                        End With

                        Try
                            Response = .DownloadString(UniformResourceIdentifier)
                            Return True

                        Catch ex As ArgumentNullException
                            ' Der address-Parameter ist null.
                            NLogger.Error(ex, "Der address-Parameter ist null.")

                        Catch ex As WebException
                            ' Der durch Kombinieren von BaseAddress und address gebildete URI ist ungültig.
                            ' - oder -
                            ' Fehler beim Herunterladen der Ressource.

                            NLogger.Error(ex, $"Link: {UniformResourceIdentifier.AbsoluteUri} ")

                        Catch ex As NotSupportedException
                            ' Die Methode wurde gleichzeitig für mehrere Threads aufgerufen.
                            NLogger.Error(ex, "Die Methode wurde gleichzeitig für mehrere Threads aufgerufen.")

                        End Try
                    End With
                End Using
            Case Else
                NLogger.Warn($"Uri.Scheme: {UniformResourceIdentifier.Scheme}")

        End Select
        Return False
    End Function

    ''' <summary>
    ''' Lädt die angeforderte Ressource als <see cref="String"/> asynchron herunter. Die herunterzuladende Ressource ist als <see cref="Uri"/> angegeben.
    ''' </summary>
    ''' <param name="UniformResourceIdentifier">Ein <see cref="Uri"/>-Objekt, das den herunterzuladenden URI enthält.</param>
    ''' <param name="ZeichenCodierung">(Optional) Legt die <see cref="Encoding"/> für den Download von Zeichenfolgen fest.</param>
    ''' <param name="Headers">(Optional) Zusätzliche Header für den Download von Zeichenfolgen</param>
    ''' <param name="IgnoreWebExcepton">Angabe, ob eine <see cref="WebException"/> ignoriert werden soll.</param>
    ''' <returns>Das <see cref="Task"/>-Objekt, das den asynchronen Vorgang darstellt.</returns>
    Friend Async Function DownloadStringTaskAsync(UniformResourceIdentifier As Uri, Optional ZeichenCodierung As Encoding = Nothing, Optional Headers As WebHeaderCollection = Nothing, Optional IgnoreWebExcepton As Boolean = False) As Task(Of String)

        Dim retVal As String = String.Empty

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        If UniformResourceIdentifier.IsAbsoluteUri Or UniformResourceIdentifier.IsFile Then
            Select Case UniformResourceIdentifier.Scheme
                Case Uri.UriSchemeHttp, Uri.UriSchemeHttps

                    Using webClient As New WebClient With {.Proxy = Nothing,
                                                           .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache),
                                                           .Encoding = If(ZeichenCodierung, Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox))}
                        With webClient

                            With .Headers
                                .Set(HttpRequestHeader.UserAgent, DefaultHeaderUserAgent)
                                .Set(HttpRequestHeader.KeepAlive, DefaultHeaderKeepAlive.ToString)
                                If Headers IsNot Nothing Then .Add(Headers)
                            End With

                            Try
                                retVal = Await .DownloadStringTaskAsync(UniformResourceIdentifier)
                                NLogger.Trace($"{UniformResourceIdentifier.AbsoluteUri} - {retVal}")

                            Catch ex As ArgumentNullException
                                ' Der address-Parameter ist null.
                                NLogger.Error(ex, "Der address-Parameter ist null.")

                            Catch ex As WebException
                                ' Der durch Kombinieren von BaseAddress und address gebildete URI ist ungültig.
                                ' - oder -
                                ' Fehler beim Herunterladen der Ressource.

                                If IgnoreWebExcepton Then
                                    ' Nix tun
                                    NLogger.Debug($"Aufruf von {UniformResourceIdentifier.AbsoluteUri} liefert einen Fehler.")
                                Else
                                    ' Fehlermeldung ins Log schreiben
                                    NLogger.Error(ex, $"Link: {UniformResourceIdentifier.AbsoluteUri}")
                                End If

                            End Try
                        End With
                    End Using

                Case Uri.UriSchemeFile

                    retVal = Await Task.Run(Function()
                                                Try
                                                    Return IO.File.ReadAllText(UniformResourceIdentifier.LocalPath)
                                                Catch ex As Exception
                                                    NLogger.Warn(ex, $"Lokale Datei {UniformResourceIdentifier.LocalPath} kann nicht gelesen werden.")
                                                    Return String.Empty
                                                End Try
                                            End Function)
                Case Else
                    NLogger.Warn($"Uri.Scheme: {UniformResourceIdentifier.Scheme}")
            End Select
        End If


        Return retVal
    End Function

    ''' <summary>
    ''' Lädt die angeforderte Ressource als <see cref="Byte"/>-Array asynchron herunter. Die herunterzuladende Ressource ist als <see cref="Uri"/> angegeben.
    ''' </summary>
    ''' <param name="UniformResourceIdentifier">Ein <see cref="Uri"/>-Objekt, das den herunterzuladenden URI enthält.</param>
    ''' <param name="ZeichenCodierung">(Optional) Legt die <see cref="Encoding"/> für den Download von Zeichenfolgen fest.</param>
    ''' <param name="Headers">(Optional) Zusätzliche Header für den Download von Zeichenfolgen</param>
    ''' <returns>Das <see cref="Task"/>-Objekt, das den asynchronen Vorgang darstellt.</returns>
    Friend Async Function DownloadDataTaskAsync(UniformResourceIdentifier As Uri, Optional ZeichenCodierung As Encoding = Nothing, Optional Headers As WebHeaderCollection = Nothing) As Task(Of Byte())

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Select Case UniformResourceIdentifier.Scheme
            Case Uri.UriSchemeHttp, Uri.UriSchemeHttps

                Using webClient As New WebClient With {.Proxy = Nothing,
                                                       .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache),
                                                       .Encoding = If(ZeichenCodierung, Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox))}
                    With webClient

                        With .Headers
                            .Set(HttpRequestHeader.UserAgent, DefaultHeaderUserAgent)
                            .Set(HttpRequestHeader.KeepAlive, DefaultHeaderKeepAlive.ToString)
                            If Headers IsNot Nothing Then .Add(Headers)
                        End With

                        Try
                            Return Await .DownloadDataTaskAsync(UniformResourceIdentifier)

                        Catch ex As ArgumentNullException
                            ' Der address-Parameter ist null.
                            NLogger.Error(ex, "Der address-Parameter ist null.")
                            Return {}

                        Catch ex As WebException
                            ' Der durch Kombinieren von BaseAddress und address gebildete URI ist ungültig.
                            ' - oder -
                            ' Fehler beim Herunterladen der Ressource.
                            NLogger.Error(ex, $"Link: {UniformResourceIdentifier.AbsoluteUri} ")
                            Return {}

                        End Try
                    End With
                End Using
            Case Else

                Return {}
                NLogger.Warn($"Uri.Scheme: {UniformResourceIdentifier.Scheme}")
        End Select

    End Function

    ''' <summary>
    ''' Lädt die angegebene Ressource in eine lokale Datei als asynchroner Vorgang mithilfe eines <see cref="Task"/>-Objekt herunter.
    ''' </summary>
    ''' <param name="UniformResourceIdentifier">Ein <see cref="Uri"/>-Objekt, das den herunterzuladenden URI enthält.</param>
    ''' <param name="DateiName">Der Name der Datei, die auf dem lokalen Computer platziert werden soll.</param>
    ''' <param name="ZeichenCodierung">(Optional) Legt die <see cref="Encoding"/> für den Download von Zeichenfolgen fest.</param>
    ''' <param name="Headers">(Optional) Zusätzliche Header für den Download von Zeichenfolgen</param>
    ''' <returns>Boolean, je nach Erfolg der Abfrage.</returns>
    Friend Async Function DownloadToFileTaskAsync(UniformResourceIdentifier As Uri, DateiName As String, Optional ZeichenCodierung As Encoding = Nothing, Optional Headers As WebHeaderCollection = Nothing) As Task(Of Boolean)

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Select Case UniformResourceIdentifier.Scheme
            Case Uri.UriSchemeHttp, Uri.UriSchemeHttps

                Using webClient As New WebClient With {.Proxy = Nothing,
                                                       .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache),
                                                       .Encoding = If(ZeichenCodierung, Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox))}
                    With webClient

                        With .Headers
                            .Set(HttpRequestHeader.UserAgent, DefaultHeaderUserAgent)
                            .Set(HttpRequestHeader.KeepAlive, DefaultHeaderKeepAlive.ToString)
                            If Headers IsNot Nothing Then .Add(Headers)
                        End With

                        Try
                            Await .DownloadFileTaskAsync(UniformResourceIdentifier, DateiName)

                            Return True
                        Catch ex As ArgumentNullException
                            ' Der address-Parameter ist null.
                            NLogger.Error(ex, "Der address-Parameter ist null.")

                        Catch ex As WebException
                            ' Der durch Kombinieren von BaseAddress und address gebildete URI ist ungültig.
                            ' - oder -
                            ' Fehler beim Herunterladen der Ressource.
                            NLogger.Error(ex, $"Link: {UniformResourceIdentifier.AbsoluteUri}")

                        Catch ex As InvalidOperationException
                            'Die von fileName angegebene lokale Datei wird von einem anderen Thread verwendet.
                            NLogger.Error(ex, $"Die von {DateiName} angegebene lokale Datei wird von einem anderen Thread verwendet.")

                        End Try
                    End With
                End Using
            Case Else
                NLogger.Warn($"Uri.Scheme: {UniformResourceIdentifier.Scheme}")

        End Select
        Return False
    End Function

    Friend Function GetStreamTaskAsync(UniformResourceIdentifier As Uri) As Task(Of IO.Stream)

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Select Case UniformResourceIdentifier.Scheme
            Case Uri.UriSchemeHttp, Uri.UriSchemeHttps

                Using webClient As New WebClient With {.Proxy = Nothing,
                                                       .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)}
                    With webClient

                        With .Headers
                            .Set(HttpRequestHeader.UserAgent, DefaultHeaderUserAgent)
                            .Set(HttpRequestHeader.KeepAlive, DefaultHeaderKeepAlive.ToString)
                        End With

                        Try
                            Return .OpenReadTaskAsync(UniformResourceIdentifier)

                        Catch ex As WebException
                            ' Der durch Kombinieren von BaseAddress und address gebildete URI ist ungültig.
                            ' - oder -
                            ' Fehler beim Herunterladen der Ressource.
                            NLogger.Error(ex, $"Link: {UniformResourceIdentifier.AbsoluteUri} ")
                            Return Nothing

                        Catch ex As ArgumentNullException
                            ' Der address-Parameter ist null.
                            NLogger.Error(ex, "Der address-Parameter ist null.")
                            Return Nothing

                        End Try
                    End With
                End Using
            Case Else

                NLogger.Warn($"Uri.Scheme: {UniformResourceIdentifier.Scheme}")
                Return Nothing
        End Select
    End Function

#End Region

#Region "POST"
    ''' <summary>
    ''' Lädt die angegebene Zeichenfolge in die angegebene Ressource hoch.
    ''' </summary>
    ''' <param name="UniformResourceIdentifier">Der <see cref="Uri"/> der Ressource, die die Zeichenfolge empfangen soll.</param>
    ''' <param name="PostData">Die Uploadzeichenfolge.</param>
    ''' <param name="NC">Legt die Netzwerkanmeldeinformationen als <see cref="ICredentials"/> fest, die an den Host gesendet und für die Authentifizierung der Anforderung verwendet wird.</param>
    ''' <param name="Response">Ein <see cref="String"/>, der die vom Server gesendete Antwort enthält.</param>
    ''' <param name="Headers">(Optional) Zusätzliche Header für den Download von Zeichenfolgen</param>
    ''' <param name="ZeichenCodierung">(Optional) Legt die <see cref="Encoding"/> für den Download von Zeichenfolgen fest.</param>
    ''' <returns>Boolean, je nach Erfolg der Abfrage.</returns>
    Friend Function UploadData(UniformResourceIdentifier As Uri, PostData As String, NC As NetworkCredential, ByRef Response As String, Optional Headers As WebHeaderCollection = Nothing, Optional ZeichenCodierung As Encoding = Nothing) As Boolean

        Response = String.Empty

        Using webClient As New WebClient With {.Credentials = NC,
                                               .Encoding = If(ZeichenCodierung, Encoding.GetEncoding(FritzBoxDefault.DfltCodePageFritzBox))}
            With webClient

                With .Headers
                    .Set(HttpRequestHeader.UserAgent, DefaultHeaderUserAgent)
                    .Set(HttpRequestHeader.KeepAlive, DefaultHeaderKeepAlive.ToString)
                    If Headers IsNot Nothing Then .Add(Headers)
                End With

                Try
                    Response = .UploadString(UniformResourceIdentifier, PostData)
                    Return True
                Catch ex As ArgumentException
                    ' Der address-Parameter ist null.
                    ' - oder -
                    ' Der Data - Parameter ist null.
                    NLogger.Error(ex, $"URI: ' {UniformResourceIdentifier.AbsoluteUri} '; Data: '{PostData}' ")
                Catch ex As WebException
                    ' Der durch Kombinieren von BaseAddress und address gebildete URI ist ungültig.
                    ' - oder -
                    ' Der Server, der Host dieser Ressource ist, hat nicht geantwortet.
                    NLogger.Error(ex, $"URI: ' {UniformResourceIdentifier.AbsoluteUri} '; Data: '{PostData}' ")
                End Try
            End With
        End Using

        Return False
    End Function
#End Region

#End Region

End Module
