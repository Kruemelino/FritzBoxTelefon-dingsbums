Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Friend Class Phoner
    Implements IDisposable
    Implements IIPPhone

#Region "Phoner Strings"
    Private Const PhonerProgressName As String = "phoner"
    Private Const PhonerLogin As String = "Login"
    Private Const PhonerWelcomeMessage As String = "Welcome to Phoner"
    Private Const PhonerChallenge As String = "Challenge="
    Private Const PhonerResponse As String = "Response="
    Private Const PhonerCONNECT As String = "CONNECT"
    Private Const PhonerDISCONNECT As String = "DISCONNECT"
#End Region

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private ReadOnly Property PhonerEndpoint As IPAddress = IPAddress.Loopback
    Private ReadOnly Property PhonerEndpointPort As Integer = 2012

    Friend ReadOnly Property PhonerReady As Boolean Implements IIPPhone.IPPhoneReady
        Get
            Return Process.GetProcessesByName(PhonerProgressName).Length.IsNotZero
        End Get
    End Property

    ''' <summary>
    ''' Initiiert ein Telefonat über Phoner
    ''' </summary>
    ''' <param name="DialCode">Die zu wählende Nummer</param>
    ''' <param name="Hangup">Angabe, ob der Rufaufbau beendet werden soll.</param>
    ''' <returns></returns>
    Friend Function Dial(DialCode As String, Hangup As Boolean) As Boolean Implements IIPPhone.Dial

        Dial = False

        If PhonerReady Then
            Using PhonerTcpClient As New TcpClient
                PhonerTcpClient.Connect(PhonerEndpoint, PhonerEndpointPort)

                Dim PhonerDatenstrom As NetworkStream = PhonerTcpClient.GetStream

                Dim Daten As String

                With PhonerDatenstrom
                    If .CanWrite Then
                        Using SW As New StreamWriter(PhonerDatenstrom)
                            SW.AutoFlush = True
                            Using SR As New StreamReader(PhonerDatenstrom)
                                ' Authentifizierunt einleiten
                                SW.WriteLine(PhonerLogin)
                                ' Hole die Phoner Welcome Message
                                Daten = SR.ReadLine
                                NLogger.Debug($"Phoner-Welcome:  {Daten}")
                                If Daten.AreEqual(PhonerWelcomeMessage) Then
                                    ' Ermittle die Phoner Challenge
                                    Dim Challange As String = SR.ReadLine.RegExReplace($"^{PhonerChallenge}", DfltStringEmpty)

                                    ' Bei Phoner Authentifizieren md5(ChallengePasswort)
                                    Dim Response As String
                                    Using Crypter As New Rijndael
                                        With Crypter
                                            Response = .SecureStringToMD5(.DecryptString(XMLData.POptionen.TBPhonerPasswort, DfltPhonerDeCryptKey), Encoding.ASCII, Challange).ToUpper
                                        End With
                                    End Using
                                    NLogger.Debug($"Phoner-Challange: {Challange}, Phoner-Response: {Response}")

                                    SW.WriteLine(PhonerResponse & Response)
                                    Thread.Sleep(50)
                                    If .DataAvailable Then
                                        NLogger.Debug("Authentifizierung erfolgreich")

                                        ' Wählkommando senden
                                        If Hangup Then
                                            ' Abbruch des Rufaufbaues mittels DISCONNECT
                                            SW.WriteLine(PhonerDISCONNECT)
                                            NLogger.Debug(Localize.LocWählclient.strSoftPhoneAbbruch)
                                        Else
                                            ' Aufbau des Telefonates mittels CONNECT
                                            SW.WriteLine($"{PhonerCONNECT} {DialCode}")
                                            NLogger.Debug(String.Format(Localize.LocWählclient.strSoftPhoneAbbruch, DialCode, PhonerProgressName))
                                        End If

                                        Dial = True

                                    Else
                                        NLogger.Warn(Localize.LocWählclient.strPhonerPasswortFalsch)
                                    End If
                                Else
                                    NLogger.Warn(Localize.LocWählclient.strPhonerZuAlt)
                                End If

                            End Using
                        End Using
                    Else
                        NLogger.Error(Localize.LocWählclient.strPhonerReadonly)
                    End If
                    ' Datenstrom schließen und aufräumen
                    .Close()
                    .Dispose()
                End With
                ' TCP-Client schließen und aufräumen
                With PhonerTcpClient
                    .Close()
                    .Dispose()
                End With
            End Using
        Else
            ' Phoner nicht verfügbar
            NLogger.Warn(Localize.LocWählclient.strPhonerNichtBereit)
        End If
    End Function

    Private disposedValue As Boolean
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' Verwalteten Zustand (verwaltete Objekte) bereinigen
            End If

            ' Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
            ' Große Felder auf NULL setzen
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        ' Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
End Class
