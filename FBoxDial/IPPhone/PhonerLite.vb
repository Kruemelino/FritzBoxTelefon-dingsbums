Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Public Module PhonerLite
#Region "PhonerLite Strings"
    Private Const PhonerLiteProgressName As String = "PhonerLite"
#End Region

#Region "Eigenschften"
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private ReadOnly Property PhonerLiteReady As Boolean
        Get
            Return Process.GetProcessesByName(PhonerLiteProgressName).Length.IsNotZero
        End Get
    End Property

#End Region

    ''' <summary>
    ''' Initiiert ein Telefonat über PhonerLite
    ''' </summary>
    ''' <param name="DialCode">Die zu wählende Nummer</param>
    ''' <param name="Hangup">Angabe, ob der Rufaufbau beendet werden soll.</param>
    ''' <returns></returns>
    Friend Function Dial(Connector As IIPPhoneConnector, DialCode As String, Hangup As Boolean) As Boolean

        Dial = False
        If Connector.Type = IPPhoneConnectorType.PhonerLite Then
            If PhonerLiteReady Then
                Using PhonerLiteTcpClient As New TcpClient()

                    Try
                        PhonerLiteTcpClient.Connect(IPAddress.Loopback, Connector.Port)

                        NLogger.Info($"Verbindung zum CLI von PhonerLite aufgebaut (Port: {Connector.Port}")
                    Catch ex As Exception

                        NLogger.Error(ex)
                    End Try

                    If PhonerLiteTcpClient.Connected Then

                        Dim PhonerLiteDatenstrom As NetworkStream = PhonerLiteTcpClient.GetStream

                        With PhonerLiteDatenstrom
                            If .CanWrite Then
                                Using SW As New StreamWriter(PhonerLiteDatenstrom)
                                    SW.AutoFlush = True
                                    'Using SR As New StreamReader(PhonerLiteDatenstrom)

                                    Thread.Sleep(50)

                                    ' Wählkommando senden
                                    If Hangup Then
                                        ' Abbruch des Rufaufbaues mittels DISCONNECT
                                        SW.WriteLine($"HookOn")
                                        NLogger.Debug(Localize.LocWählclient.strSoftPhoneAbbruch)
                                    Else
                                        If Connector.AppendSuffix Then DialCode += "#"

                                        ' Aufbau des Telefonates mittels CONNECT
                                        SW.WriteLine($"SetNumber {DialCode} & HookOff")

                                        NLogger.Debug(String.Format(Localize.LocWählclient.strSoftPhoneErfolgreich, DialCode, PhonerLiteProgressName))
                                    End If

                                    Dial = True

                                    'End Using
                                End Using
                            Else
                                NLogger.Error(Localize.LocWählclient.strPhonerReadonly)
                            End If
                        End With

                    End If

                    ' TCP-Client schließen und aufräumen
                    With PhonerLiteTcpClient
                        .Close()
                        .Dispose()
                    End With
                End Using
            Else
                ' Phoner nicht verfügbar
                NLogger.Warn(Localize.LocWählclient.strPhonerNichtBereit)
            End If

        End If
    End Function

End Module
