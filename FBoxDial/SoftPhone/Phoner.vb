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
    Friend ReadOnly Property PhonerReady As Boolean = Process.GetProcessesByName("phoner").Length.IsNotZero
#Region "Phoner Strings"
    ''' <summary>
    ''' Login
    ''' </summary>
    Private ReadOnly Property PhonerLogin As String = "Login"

    ''' <summary>
    ''' Welcome to Phoner
    ''' </summary>
    Private ReadOnly Property PhonerWelcomeMessage As String = "Welcome to Phoner"

    ''' <summary>
    ''' Challenge=
    ''' </summary>
    Private ReadOnly Property PhonerChallenge As String = "Challenge="

    ''' <summary>
    ''' Response=
    ''' </summary>
    Private ReadOnly Property PhonerResponse As String = "Response="

    ''' <summary>
    ''' CONNECT
    ''' </summary>
    Private ReadOnly Property PhonerCONNECT As String = "CONNECT"

    ''' <summary>
    ''' DISCONNECT
    ''' </summary>
    Private ReadOnly Property PhonerDISCONNECT As String = "DISCONNECT"
#End Region

#Region "Event"
    ''' <summary>
    ''' Event zum setzen des Status
    ''' </summary>
    ''' <param name="Status">Text, welcher Angezeigt werden soll</param>
    Friend Event SetStatus(Status As String)
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
                                        Response = Crypter.GetMd5Hash(Challange & Crypter.DecryptString128Bit(XMLData.POptionen.TBPhonerPasswort, DefaultWerte.DfltDeCryptKey), Encoding.ASCII).ToUpper
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
                                                NLogger.Debug(SoftPhoneAbbruch)
                                            Else
                                                ' Aufbau des Telefonates mittels CONNECT
                                                SW.WriteLine($"{PhonerCONNECT} {DialCode}")
                                                NLogger.Debug(SoftPhoneErfolgreich(DialCode, "Phoner"))
                                            End If
                                        End If
                                        DialPhoner = True

                                    Else
                                        NLogger.Warn(PhonerPasswortFalsch)
                                        RaiseEvent SetStatus(PhonerPasswortFalsch)
                                    End If
                                Else
                                    NLogger.Warn(PhonerZuAlt)
                                    RaiseEvent SetStatus(PhonerZuAlt)
                                End If

                            End Using
                        End Using
                    Else
                        NLogger.Error(PhonerReadonly)
                        RaiseEvent SetStatus(PhonerReadonly)
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
            RaiseEvent SetStatus(PhonerNichtBereit)
            NLogger.Warn(PhonerNichtBereit)
        End If
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
            End If

            ' TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
            ' TODO: Große Felder auf NULL setzen
            disposedValue = True
        End If
    End Sub

    ' ' TODO: Finalizer nur überschreiben, wenn "Dispose(disposing As Boolean)" Code für die Freigabe nicht verwalteter Ressourcen enthält
    ' Protected Overrides Sub Finalize()
    '     ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
