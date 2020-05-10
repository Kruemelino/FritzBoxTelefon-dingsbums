Imports System.IO
Imports System.Net
Imports System.Net.Sockets

Public Class Phoner
    Implements IDisposable

    Private Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
    Private ReadOnly Property PhonerEndpoint As IPAddress = IPAddress.Loopback
    Private ReadOnly Property PhonerEndpointPort As Integer = 2012

    Friend ReadOnly Property PhonerReady As Boolean = Not Process.GetProcessesByName("phoner").Length = 0
    Friend ReadOnly Property PhonerLiteReady As Boolean = Not Process.GetProcessesByName("phonerlite").Length = 0
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
    Friend Event SetStatus(ByVal Status As String)
#End Region

    Friend Function DialPhoner(ByVal DialCode As String, ByVal Hangup As Boolean) As Boolean

        DialPhoner = False

        If PhonerLiteReady Then
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
                                    Dim Challange As String = SR.ReadLine.RegExReplace($"^{PhonerChallenge}", PDfltStringEmpty)

                                    ' Bei Phoner Authentifizieren md5(ChallengePasswort)
                                    Dim Response As String
                                    Using Crypter As New Rijndael
                                        Response = Crypter.getMd5Hash(Challange & Crypter.DecryptString128Bit(XMLData.POptionen.PTBPhonerPasswort), Encoding.ASCII).ToUpper
                                    End Using
                                    NLogger.Debug($"Phoner-Challange: {Challange}, Phoner-Response: {Response}")

                                    SW.WriteLine(PhonerResponse & Response)
                                    If .DataAvailable Then
                                        ' Wähllkomando senden
                                        If Hangup Then
                                            ' Abbruch des Rufaufbaues mittels DISCONNECT
                                            SW.WriteLine(PhonerDISCONNECT)
                                            NLogger.Debug(PPhonerAbbruch)
                                        Else
                                            ' Aufbau des Telefonates mittels CONNECT
                                            SW.WriteLine($"{PhonerCONNECT} {DialCode}")
                                            NLogger.Debug(PPhonerErfolgreich(DialCode))
                                        End If
                                        DialPhoner = True

                                    Else
                                        NLogger.Warn(PPhonerPasswowrtFalsch)
                                        RaiseEvent SetStatus(PPhonerPasswowrtFalsch)
                                    End If
                                Else
                                    NLogger.Warn(PPhonerZuAlt)
                                    RaiseEvent SetStatus(PPhonerZuAlt)
                                End If

                            End Using
                        End Using
                    Else
                        NLogger.Error(PPhonerReadonly)
                        RaiseEvent SetStatus(PPhonerReadonly)
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
            RaiseEvent SetStatus(PPhonerNichtBereit)
            NLogger.Warn(PPhonerNichtBereit)
        End If
    End Function


#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' TODO: große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
