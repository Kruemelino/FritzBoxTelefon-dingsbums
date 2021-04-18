Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Friend Class Phoner
    Implements IDisposable

    Private disposedValue As Boolean
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private ReadOnly Property PhonerEndpoint As IPAddress = IPAddress.Loopback
    Private ReadOnly Property PhonerEndpointPort As Integer = 2012
    Friend ReadOnly Property PhonerReady As Boolean = Process.GetProcessesByName(PhonerProgressName).Length.IsNotZero
#Region "Phoner Strings"
    Private Const PhonerProgressName As String = "phoner"
    Private Const PhonerLogin As String = "Login"
    Private Const PhonerWelcomeMessage As String = "Welcome to Phoner"
    Private Const PhonerChallenge As String = "Challenge="
    Private Const PhonerResponse As String = "Response="
    Private Const PhonerCONNECT As String = "CONNECT"
    Private Const PhonerDISCONNECT As String = "DISCONNECT"
#End Region

    Friend Function CheckPhonerAuth() As Boolean
        Return DialPhoner(DfltStringEmpty, False, True)
    End Function

    Friend Function Dial(DialCode As String, Hangup As Boolean) As Boolean
        Return DialPhoner(DialCode, Hangup, False)
    End Function
    ''' <summary>
    ''' Initiiert ein Telefonat über Phoner
    ''' </summary>
    ''' <param name="DialCode">Die zu wählende Nummer</param>
    ''' <param name="Hangup">Angabe, ob der Rufaufbau beendet werden soll.</param>
    ''' <param name="Check">Angabe, ob nur die Authentifizierung mit Phoner überprüft werden soll.</param>
    ''' <returns></returns>
    Private Function DialPhoner(DialCode As String, Hangup As Boolean, Check As Boolean) As Boolean

        DialPhoner = False

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
                                        Response = Crypter.GetMd5Hash(Challange & Crypter.DecryptString128Bit(XMLData.POptionen.TBPhonerPasswort, DfltPhonerDeCryptKey), Encoding.ASCII).ToUpper
                                    End Using
                                    NLogger.Debug($"Phoner-Challange: {Challange}, Phoner-Response: {Response}")

                                    SW.WriteLine(PhonerResponse & Response)
                                    Thread.Sleep(50)
                                    If .DataAvailable Then
                                        NLogger.Debug("Authentifizierung erfolgreich")
                                        If Not Check Then
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
                                        End If
                                        DialPhoner = True

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

    ' Finalizer nur überschreiben, wenn "Dispose(disposing As Boolean)" Code für die Freigabe nicht verwalteter Ressourcen enthält
    ' Protected Overrides Sub Finalize()
    '     ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        ' Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
End Class
