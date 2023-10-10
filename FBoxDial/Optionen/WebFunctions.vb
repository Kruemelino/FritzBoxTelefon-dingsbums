Imports System.Net

Friend Module WebFunctions

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Netzwerkfunktionen"
    ''' <summary>
    ''' Führt einen Ping zur Gegenstelle aus.
    ''' </summary>
    ''' <param name="IPAdresse">IP-Adresse Netzwerkname der Gegenstelle. Rückgabe der IP-Adresse</param>
    ''' <returns>Boolean</returns>
    Friend Function Ping(ByRef IPAdresse As String) As Boolean
        Ping = False

        'Dim IPHostInfo As IPHostEntry
        Dim PingSender As New NetworkInformation.Ping()
        Dim Options As New NetworkInformation.PingOptions() With {.DontFragment = True}
        Dim PingReply As NetworkInformation.PingReply = Nothing

        Dim buffer As Byte() = Encoding.ASCII.GetBytes(String.Empty)

        Try
            PingReply = PingSender.Send(IPAdresse, Math.Max(XMLData.POptionen.TBNetworkTimeout, 100), buffer, Options)
        Catch ex As Exception
            NLogger.Warn(ex, $"Ping zu {IPAdresse} nicht erfolgreich")
            Ping = False
        End Try

        If PingReply IsNot Nothing Then
            With PingReply
                If .Status = NetworkInformation.IPStatus.Success Then
                    If .Address.AddressFamily = Sockets.AddressFamily.InterNetworkV6 Then
                        IPAdresse = $"[{ .Address}]"
                        ''Zugehörige IPv4 ermitteln
                        'IPHostInfo = Dns.GetHostEntry(.Address)
                        'For Each _IPAddress As IPAddress In IPHostInfo.AddressList
                        '    If _IPAddress.AddressFamily = Sockets.AddressFamily.InterNetwork Then
                        '        IPAdresse = _IPAddress.ToString

                        '        NLogger.Info($"IPv6: { .Address}, IPv4: {IPAdresse}")
                        '        Exit For
                        '    End If
                        'Next
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
                NLogger.Warn(ex, $"Die Adresse '{InputIP}' kann nicht zugeordnet werden.")
                ValidIP = XMLData.POptionen.TBFBAdr
            End Try
        End If

    End Function

#End Region

End Module
