Imports System.Net
Imports System.Net.Sockets
Imports System.Timers

Friend Class Anrufmonitor

#Region "Eigenschaften"
    ''' <summary>
    ''' Angabe, ob der TCP-Client zur Fritz!Box verbunden ist.
    ''' </summary>
    Friend ReadOnly Property Aktiv As Boolean
        Get
            Return AnrMonTCPClient IsNot Nothing AndAlso AnrMonTCPClient.Verbunden
        End Get
    End Property
    Friend Property AktiveTelefonate As List(Of Telefonat)
    Private Property FBoxIP As String
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
#End Region

#Region "Konstanten"
    Private Const AnrMon_RING As String = "RING"
    Private Const AnrMon_CALL As String = "CALL"
    Private Const AnrMon_CONNECT As String = "CONNECT"
    Private Const AnrMon_DISCONNECT As String = "DISCONNECT"
    Private Const AnrMon_Delimiter As String = ";"
    Private Const AnrMon_Port As Integer = 1012
#End Region

    Private WithEvents AnrMonTCPClient As AnrMonClient

    Public Sub New()
        AktiveTelefonate = New List(Of Telefonat)
    End Sub

    Private Sub BeginnStartAnrMon()
        ' Starte den Anrufmonitor
        Dim IP As IPAddress = IPAddress.Loopback
        FBoxIP = If(XMLData.POptionen.CBFBSecAdr, XMLData.POptionen.TBFBSecAdr, XMLData.POptionen.ValidFBAdr)

        If IPAddress.TryParse(FBoxIP, IP) Then
            If Ping(IP.ToString) Then
                Dim TC As New TcpClient With {.ExclusiveAddressUse = False}

                Try
                    TC.Connect(New IPEndPoint(IP, AnrMon_Port))
                Catch ex As SocketException
                    ' Connection refused.
                    ' No Connection could be made because the target computer actively refused it. This usually results from trying To connect To a service that Is inactive On the foreign host—that Is, one with no server application running.
                    If ex.SocketErrorCode = SocketError.ConnectionRefused Then
                        NLogger.Warn($"Der Anrufmonitor kann nicht verbunden werden, da der Fritz!Box CallMonitor (Port {AnrMon_Port}) nicht aktiviert ist (Telefonecode #96*5* zum aktivieren).")
                    Else
                        NLogger.Error(ex, "TcpClient.Connect")
                    End If

                End Try

                If TC.Connected Then
                    ' Info Message für das Log
                    NLogger.Info($"Anrufmonitor verbunden zu {IP}:{AnrMon_Port}")
                    AnrMonTCPClient = New AnrMonClient(TC)

                    ' Verbinden
                    AnrMonTCPClient.Connect()
                Else
                    TC.Close()
                    ' Info Message für das Log
                    NLogger.Warn($"Anrufmonitor nicht verbunden zu {IP}:{AnrMon_Port}")
                End If
            End If
        End If
        ' Ribbon aktualisieren
        Globals.ThisAddIn.POutlookRibbons.RefreshRibbon()
    End Sub

    ''' <summary>
    ''' Trennt die Verbindung zum Callmonitor der Fritz!Box
    ''' </summary>
    Friend Sub Stopp()
        If AnrMonTCPClient?.Verbunden Then
            ' TCP-Client trennen
            AnrMonTCPClient.Disconnect()

            ' Info Message für das Log
            NLogger.Debug("Anrufmonitor gewollt angehalten")
        End If
        ' Ribbon aktualisieren
        Globals.ThisAddIn.POutlookRibbons.RefreshRibbon()
    End Sub

    ''' <summary>
    ''' Baut die Verbindung zum Callmonitor der Fritz!Box auf.
    ''' </summary>
    Friend Sub Start()
        ' Falls der Anrufmonitor aktiv sein sollte, dann halte ihn sicherheitshalber an.
        If Aktiv Then Stopp()

        BeginnStartAnrMon()

    End Sub

#Region "Anrufmonitor Events"
    Private Sub AnrMonTCPClient_Message(sender As Object, e As NotifyEventArgs(Of String)) Handles AnrMonTCPClient.Message

        Dim AktivesTelefonat As Telefonat

        ' Schleife durch alle eingegangenen Meldungen des Fritz!Box Anrufmonitors. Es kann sein, dass mehrere Meldungen gleichzeitig kommen.
        ' Leere Zeilen werden übergangen.
        For Each AnrMonStatus In e.Value.Split(vbCrLf).Where(Function(S) S.IsNotStringNothingOrEmpty)
            ' Hier die Daten des Fritz!Box Anrufmonitors weitergeben
            NLogger.Info($"AnrMonAktion: {AnrMonStatus}")

            Dim FBStatusSplit As String() = AnrMonStatus.Split(AnrMon_Delimiter)

            'Schauen ob "RING", "CALL", "CONNECT" oder "DISCONNECT" übermittelt wurde
            Select Case FBStatusSplit(1)
                Case AnrMon_RING
                    ' Neues Telefonat erzeugen und Daten des Anrufmonitors übergeben
                    AktivesTelefonat = New Telefonat With {.SetAnrMonRING = FBStatusSplit}
                    ' Füge das Telefonat der Liste hinzu
                    AktiveTelefonate.Add(AktivesTelefonat)

                Case AnrMon_CALL
                    ' Neues Telefonat erzeugen und Daten des Anrufmonitors übergeben
                    AktivesTelefonat = New Telefonat With {.SetAnrMonCALL = FBStatusSplit}
                    ' Füge das Telefonat der Liste hinzu
                    AktiveTelefonate.Add(AktivesTelefonat)

                Case AnrMon_CONNECT
                    ' Vorhandenes Telefonat ermitteln und Daten des Anrufmonitors übergeben
                    AktivesTelefonat = AktiveTelefonate.Find(Function(TE) TE.ID.AreEqual(CInt(FBStatusSplit(2))))
                    If AktivesTelefonat IsNot Nothing Then AktivesTelefonat.SetAnrMonCONNECT = FBStatusSplit

                Case AnrMon_DISCONNECT
                    ' Vorhandenes Telefonat ermitteln und Daten des Anrufmonitors übergeben
                    AktivesTelefonat = AktiveTelefonate.Find(Function(TE) TE.ID.AreEqual(CInt(FBStatusSplit(2))))
                    If AktivesTelefonat IsNot Nothing Then AktivesTelefonat.SetAnrMonDISCONNECT = FBStatusSplit
                    ' Das Gespräch ist beendet. Entferne dieses Telefonat aus der Liste aktiver Telefonate
                    If AktiveTelefonate.Remove(AktivesTelefonat) Then
                        NLogger.Trace($"Telefonat {AktivesTelefonat.ID} aus Gesamtliste entfernt (Verbleibend: {AktiveTelefonate.Count}).")
                    End If

                Case Else
                    Exit Select
            End Select
        Next

    End Sub

    Private Sub AnrMonTCPClient_ErrorOccured(Sender As AnrMonClient) Handles AnrMonTCPClient.ErrorOccured
        NLogger.Warn($"Anrufmonitor wurde unerwartet getrennt von {FBoxIP}:{AnrMon_Port}")
        ' Wieververbinden versuchen
        Start()
    End Sub

    Private Sub AnrMonTCPClient_Disposed(Sender As AnrMonClient) Handles AnrMonTCPClient.Disposed
        'Aktiv = False
        Globals.ThisAddIn.POutlookRibbons.RefreshRibbon()
        NLogger.Info($"Anrufmonitor getrennt von {FBoxIP}:{AnrMon_Port}")
    End Sub
#End Region

End Class
