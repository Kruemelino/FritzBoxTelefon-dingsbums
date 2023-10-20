Public Module PhonerLite
#Region "PhonerLite Strings"
    Private Const PhonerLiteProgressName As String = "PhonerLite"
#End Region

#Region "PhonerLite Commandline"
    ''' <summary>
    ''' Hang up all calls: phonerlite.exe hangup:
    ''' </summary>
    Private Const CommandHangUp As String = "hangup:"

    ''' <summary>
    ''' Start a Call: phonerlite.exe callto:0123456789
    ''' </summary>
    Private Const CommandCallTo As String = "callto:"
#End Region

#Region "Eigenschften"
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private ReadOnly Property PhonerLiteReady As Boolean
        Get
            Return Process.GetProcessesByName(PhonerLiteProgressName).Length.IsNotZero
        End Get
    End Property

#End Region

    Friend Function PhonerLiteGetExecutablePath() As String
        Dim ProcressPhonerLite As Process() = Process.GetProcessesByName(PhonerLiteProgressName)

        If ProcressPhonerLite.Length.IsNotZero Then

            NLogger.Debug(String.Format(Localize.LocWählclient.strSoftPhoneBereit, PhonerLiteProgressName))

            ' Ermittle Pfad zur ausgeführten MicroSIP.exe
            Return ProcressPhonerLite.First.MainModule.FileName
        Else
            Return String.Empty
        End If
    End Function

    Private Sub PhonerLiteStart(Connector As IIPPhoneConnector)
        NLogger.Debug(String.Format(Localize.LocWählclient.strSoftPhoneNichtBereit, PhonerLiteProgressName))

        If Connector.ConnectionUriCall.IsNotStringNothingOrEmpty Then
            ' Starte PhonerLite
            Try
                Process.Start(Connector.ConnectionUriCall)

                NLogger.Info(String.Format(Localize.LocWählclient.strSoftPhoneGestartet, PhonerLiteProgressName))
            Catch ex As ComponentModel.Win32Exception
                NLogger.Warn(ex)
            Catch ex As ObjectDisposedException
                NLogger.Warn(ex)
            Catch ex As IO.FileNotFoundException
                NLogger.Warn(ex)
            End Try

        End If
    End Sub

    ''' <summary>
    ''' Initiiert ein Telefonat über PhonerLite
    ''' </summary>
    ''' <param name="DialCode">Die zu wählende Nummer</param>
    ''' <param name="Hangup">Angabe, ob der Rufaufbau beendet werden soll.</param>
    Friend Function Dial(Connector As IIPPhoneConnector, DialCode As String, Hangup As Boolean) As Boolean

        Dial = False
        If Connector.Type = IPPhoneConnectorType.PhonerLite Then
            If Not PhonerLiteReady Then PhonerLiteStart(Connector)

            If PhonerLiteReady Then
                ' Wählkommando senden
                If Hangup Then
                    ' Abbruch des Rufaufbaues mittels Parameter
                    Process.Start(Connector.ConnectionUriCall, CommandHangUp)

                    NLogger.Debug(Localize.LocWählclient.strSoftPhoneAbbruch)
                Else
                    If Connector.AppendSuffix Then DialCode += "#"

                    ' Aufbau des Telefonates mittels Parameter 
                    Process.Start(Connector.ConnectionUriCall, $"{CommandCallTo}{DialCode}")

                    NLogger.Debug(String.Format(Localize.LocWählclient.strSoftPhoneErfolgreich, DialCode, PhonerLiteProgressName))
                End If
                ' Gib Rückmeldung, damit Wählclient kein Fehler ausgibt
                Return True
            Else
                ' PhonerLite nicht verfügbar
                NLogger.Debug(String.Format(Localize.LocWählclient.strSoftPhoneNichtBereit, PhonerLiteProgressName))
            End If

        End If
    End Function

#Region "CLI Unterstützung - nicht weiter verfolgt"
    'Private Const PhonerLitePasswordResponse As String = "Password:"
    'Private Const PhonerLiteReadyResponse As String = "PL>"

    'Friend Function Dial(Connector As IIPPhoneConnector, DialCode As String, Hangup As Boolean) As Boolean

    '    Dial = False
    '    If Connector.Type = IPPhoneConnectorType.PhonerLite Then
    '        If PhonerLiteReady Then
    '            Using PhonerLiteTcpClient As New TcpClient()

    '                Try
    '                    PhonerLiteTcpClient.Connect(IPAddress.Loopback, Connector.Port)

    '                    NLogger.Info($"Verbindung zum CLI von PhonerLite aufgebaut (Port: {Connector.Port}")
    '                Catch ex As Exception

    '                    NLogger.Error(ex)
    '                End Try

    '                If PhonerLiteTcpClient.Connected Then

    '                    Dim PhonerLiteDatenstrom As NetworkStream = PhonerLiteTcpClient.GetStream

    '                    With PhonerLiteDatenstrom
    '                        If .CanWrite Then
    '                            Using SW As New StreamWriter(PhonerLiteDatenstrom, Encoding.ASCII)

    '                                SW.AutoFlush = True

    '                                Using SR As New StreamReader(PhonerLiteDatenstrom, Encoding.ASCII)

    '                                    ' Lese die Willkommensnachricht von PhonerLite
    '                                    Dim buffer(54) As Char
    '                                    Dim Response As String

    '                                    SR.Read(buffer, 0, buffer.Length)
    '                                    Response = New String(buffer).Trim

    '                                    NLogger.Info($"PhonerLite: {Response.RemoveLineBreaks}")

    '                                    Thread.Sleep(50)

    '                                    If Response.EndsWith(PhonerLitePasswordResponse) Then
    '                                        ' Die Eingabe eines Passwortes ist erforderlich.

    '                                        Using Crypter As New Rijndael
    '                                            With Crypter
    '                                                ' Übermittler das Passwort im Klartext an Phoner Lite
    '                                                SW.Write(.SecureStringToString(.DecryptString(Connector.Passwort, My.Resources.strDfltPhonerDeCryptKey), Encoding.ASCII) & vbCr)

    '                                                Thread.Sleep(50)

    '                                                ' Ermittle die Antwort
    '                                                SR.Read(buffer, 0, buffer.Length)
    '                                                Response = New String(buffer).Trim

    '                                                NLogger.Info($"PhonerLite: {Response.RemoveLineBreaks}")

    '                                            End With
    '                                        End Using
    '                                    Else
    '                                        Response = PhonerLiteReadyResponse
    '                                    End If

    '                                    If Response.Contains(PhonerLiteReadyResponse) Then
    '                                        ' Wählkommando senden
    '                                        If Hangup Then
    '                                            ' Abbruch des Rufaufbaues mittels DISCONNECT
    '                                            SW.WriteLine($"HookOn")
    '                                            NLogger.Debug(Localize.LocWählclient.strSoftPhoneAbbruch)
    '                                        Else
    '                                            If Connector.AppendSuffix Then DialCode += "#"

    '                                            ' Aufbau des Telefonates mittels CONNECT
    '                                            SW.WriteLine($"SetNumber {DialCode} & HookOff")

    '                                            NLogger.Debug(String.Format(Localize.LocWählclient.strSoftPhoneErfolgreich, DialCode, PhonerLiteProgressName))
    '                                        End If

    '                                        Dial = True
    '                                    End If

    '                                    SW.WriteLine($"quit")
    '                                End Using
    '                            End Using
    '                        Else
    '                            NLogger.Error(Localize.LocWählclient.strPhonerReadonly)
    '                        End If
    '                    End With

    '                End If

    '                ' TCP-Client schließen und aufräumen
    '                With PhonerLiteTcpClient
    '                    .Close()
    '                    .Dispose()
    '                End With
    '            End Using
    '        Else
    '            ' Phoner nicht verfügbar
    '            NLogger.Warn(Localize.LocWählclient.strPhonerNichtBereit)
    '        End If

    '    End If
    'End Function
#End Region
End Module
