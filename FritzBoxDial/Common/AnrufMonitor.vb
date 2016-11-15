Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Friend Class AnrufMonitor
#Region "BackgroundWorker"
    Private WithEvents BWStartTCPReader As BackgroundWorker
    Private WithEvents BWActivateCallmonitor As BackgroundWorker
#End Region

#Region "Timer"
    Private WithEvents TimerReStart As System.Timers.Timer
    Private WithEvents TimerCheckAnrMon As System.Timers.Timer
#End Region

#Region "Eigene Klassen"
    Private C_XML As XML
    Private C_GUI As GraphicalUserInterface
    Private C_OlI As OutlookInterface
    Private C_KF As KontaktFunktionen
    Private C_DP As DataProvider
    Private C_hf As Helfer
    Private C_Popup As Popup
#End Region

#Region "Eigene Formulare"
    Private F_RWS As formRWSuche
    Private F_AnrListImport As formImportAnrList
#End Region

#Region "NetworkStream"
    Private Shared AnrMonStream As NetworkStream
#End Region

#Region "Properties"
    ''' <summary>
    ''' Klasse für das Anrufmonitor- und Stoppuhr-Popup
    ''' </summary>
    Public Property P_PopUp() As Popup
        Get
            Return C_Popup
        End Get
        Set(ByVal value As Popup)
            C_Popup = value
        End Set
    End Property

    ''' <summary>
    ''' Klasse für die Anruflistenauswertung
    ''' </summary>
    Public Property P_FormAnrList() As formImportAnrList
        Get
            Return F_AnrListImport
        End Get
        Set(ByVal value As formImportAnrList)
            F_AnrListImport = value
        End Set
    End Property

    ''' <summary>
    ''' Angabe, ob der Anrufmonitor aktiv ist.
    ''' </summary>
    Friend Property AnrMonAktiv() As Boolean
        Get
            Return bAnrMonAktiv
        End Get
        Set(ByVal value As Boolean)
            bAnrMonAktiv = value
        End Set
    End Property

    ''' <summary>
    ''' Angabe, ob es beim Aufbau des Anrufmonitors ein Fehler gab.
    ''' </summary>
    Friend Property AnrMonError() As Boolean
        Get
            Return bAnrMonError
        End Get
        Set(ByVal value As Boolean)
            bAnrMonError = value
        End Set
    End Property

    ''' <summary>
    ''' Angabe, ob der Anrufmonitor von Phoner verwendet werden soll.
    ''' </summary>
    Friend Property AnrMonPhoner() As Boolean
        Get
            Return bAnrMonPhoner
        End Get
        Set(ByVal value As Boolean)
            bAnrMonPhoner = value
        End Set
    End Property

    ''' <summary>
    '''Der letzte Anrufer.
    ''' </summary>
    Friend Property LetzterAnrufer As C_Telefonat
        Get
            Return TLetzterAnrufer
        End Get
        Set(ByVal value As C_Telefonat)
            TLetzterAnrufer = value
        End Set
    End Property
#End Region

#Region "Enumerationen"
    Enum AnrMonEvent
        AnrMonRING = 0
        AnrMonCALL = 2
        AnrMonCONNECT = 3
        AnrMonDISCONNECT = 4
    End Enum
#End Region

#Region "Strukturen"
    Private Structure DefAnrMon
#If OVer = 11 Then
        Private Dummy As String
#End If
        Friend Const FB_RING As String = "RING"
        Friend Const FB_CALL As String = "CALL"
        Friend Const FB_CONNECT As String = "CONNECT"
        Friend Const FB_DISCONNECT As String = "DISCONNECT"
    End Structure
#End Region

#Region "Globale Variablen"
    Private bAnrMonAktiv As Boolean                    ' damit 'AnrMonAktion' nur einmal aktiv ist
    Private bAnrMonError As Boolean
    Private bAnrMonPhoner As Boolean = False
    Private TLetzterAnrufer As C_Telefonat
    Private RestartCounter As Integer
#End Region

    Friend Sub New(ByVal DataProvoderKlasse As DataProvider, ByVal RWS As formRWSuche, ByVal HelferKlasse As Helfer, ByVal KontaktKlasse As KontaktFunktionen, ByVal InterfacesKlasse As GraphicalUserInterface, ByVal OutlInter As OutlookInterface, ByVal PopupKlasse As Popup, ByVal XMLKlasse As XML)
        C_XML = XMLKlasse
        C_DP = DataProvoderKlasse
        C_hf = HelferKlasse
        C_KF = KontaktKlasse
        C_GUI = InterfacesKlasse
        F_RWS = RWS
        C_OlI = OutlInter
        C_Popup = PopupKlasse
        AnrMonStartStopp()
    End Sub

#Region "BackgroundWorker"
    Private Sub BWStartTCPReader_DoWork(sender As Object, e As DoWorkEventArgs) Handles BWStartTCPReader.DoWork
        Dim IPAddresse As IPAddress = IPAddress.Loopback
        Dim ReceiveThread As Thread
        Dim RemoteEP As IPEndPoint
        Dim IPHostInfo As IPHostEntry
        Dim FBAnrMonPort As Integer
        Dim AnrMonTCPSocket As Socket

        C_hf.ThreadSleep(500)

        If C_DP.P_CBPhonerAnrMon Then
            FBAnrMonPort = DataProvider.P_DefaultPhonerAnrMonPort
            'IPAddresse = IPAddress.Loopback ' 127.0.0.1 ' Wert bei "Dim" schon gesetzt
        Else
            FBAnrMonPort = DataProvider.P_DefaultFBAnrMonPort
            If Not IPAddress.TryParse(C_DP.P_TBFBAdr, IPAddresse) Then
                ' Versuche über Default-IP zur Fritz!Box zu gelangen
                IPHostInfo = Dns.GetHostEntry(DataProvider.P_Def_FritzBoxAdress)
                IPAddresse = IPAddress.Parse(IPHostInfo.AddressList(0).ToString) ' Kann auch IPv6 sein
            End If
        End If

        RemoteEP = New IPEndPoint(IPAddresse, FBAnrMonPort)
        AnrMonTCPSocket = New Sockets.Socket(IPAddresse.AddressFamily, Sockets.SocketType.Stream, Sockets.ProtocolType.Tcp)

        Try
            AnrMonTCPSocket.Connect(RemoteEP)
        Catch SocketError As SocketException
            Select Case SocketError.SocketErrorCode
                Case Sockets.SocketError.ConnectionRefused
                    If FBAnrMonPort = DataProvider.P_DefaultFBAnrMonPort Then
                        'Es konnte keine Verbindung hergestellt werden, da der Zielcomputer die Verbindung verweigerte.
                        If C_hf.MsgBox(DataProvider.P_AnrMon_MsgBox_AnrMonStart1, MsgBoxStyle.YesNo, DataProvider.P_AnrMon_MsgBox_AnrMonStart2) = MsgBoxResult.Yes Then
                            BWActivateCallmonitor = New BackgroundWorker
                            With BWActivateCallmonitor
                                .RunWorkerAsync()
                            End With
                        Else
                            C_hf.LogFile(DataProvider.P_AnrMon_Log_AnrMonStart1)
                        End If
                    End If
                Case Else
                    C_hf.LogFile(DataProvider.P_AnrMon_Log_AnrMonStart2(SocketError.Message))
                    AnrMonError = True
                    e.Result = False
            End Select
        Catch
            C_hf.LogFile(DataProvider.P_AnrMon_Log_AnrMonStart3)

            AnrMonError = True
            e.Result = False
            Throw
        End Try

        If AnrMonTCPSocket.Connected Then
            AnrMonStream = New NetworkStream(AnrMonTCPSocket)
            If AnrMonStream.CanRead Then
                ReceiveThread = New Thread(AddressOf AnrMonAktion)
                With ReceiveThread
                    .IsBackground = True
                    .Start()
                    AnrMonAktiv = .IsAlive
                End With
                ' Timer AnrufmonitorCheck starten
                If Not C_DP.P_CBPhonerAnrMon Then TimerCheckAnrMon = C_hf.SetTimer(TimeSpan.FromMinutes(DataProvider.P_Def_CheckAnrMonIntervall).TotalMilliseconds)
                e.Result = AnrMonAktiv
            Else
                AnrMonError = True
                e.Result = False
            End If
        Else
            AnrMonError = True
            e.Result = False
        End If
    End Sub

    Private Sub BWStartTCPReader_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BWStartTCPReader.RunWorkerCompleted

        AnrMonAktiv = CBool(e.Result)
        AnrMonError = Not AnrMonAktiv
#If OVer < 14 Then
        C_GUI.SetAnrMonButton()
#Else
        C_GUI.RefreshRibbon()
#End If
        If AnrMonAktiv Then
            'If TimerReStart IsNot Nothing Then
            '    TimerReStart = C_hf.KillTimer(TimerReStart)
            '    C_hf.LogFile(DataProvider.P_AnrMon_Log_AnrMonStart4)
            'End If
        Else
            C_hf.LogFile(DataProvider.P_AnrMon_Log_AnrMonStart5)
        End If
        BWStartTCPReader.Dispose()
    End Sub

    Sub BWActivateCallmonitor_DoWork() Handles BWActivateCallmonitor.DoWork
        C_GUI.P_CallClient.Wählbox(Nothing, DataProvider.P_Def_TelCodeActivateFritzBoxCallMonitor, DataProvider.P_Def_LeerString, True)
        Do
            Windows.Forms.Application.DoEvents()
        Loop Until C_GUI.P_CallClient.ListFormWählbox.Count = 0
    End Sub
#End Region

#Region "Timer"
    Private Sub TimerCheckAnrMon_Elapsed(sender As Object, e As Timers.ElapsedEventArgs) Handles TimerCheckAnrMon.Elapsed
        ' Es kann sein, dass die Verbindung zur FB abreißt. Z. B. wenn die VPN unterbrochen ist. 

        Dim IPAddresse As IPAddress = IPAddress.Loopback
        Dim RemoteEP As IPEndPoint
        Dim IPHostInfo As IPHostEntry
        Dim CheckAnrMonTCPSocket As Socket

        If Not IPAddress.TryParse(C_DP.P_TBFBAdr, IPAddresse) Then
            ' Versuche über Default-IP zur Fritz!Box zu gelangen
            IPHostInfo = Dns.GetHostEntry(DataProvider.P_Def_FritzBoxAdress)
            IPAddresse = IPAddress.Parse(IPHostInfo.AddressList(0).ToString) ' Kann auch IPv6 sein
        End If

        RemoteEP = New IPEndPoint(IPAddresse, DataProvider.P_DefaultFBAnrMonPort)
        CheckAnrMonTCPSocket = New Sockets.Socket(IPAddresse.AddressFamily, Sockets.SocketType.Stream, Sockets.ProtocolType.Tcp)

        Try
            CheckAnrMonTCPSocket.Connect(RemoteEP)
        Catch Err As SocketException
            C_hf.LogFile(DataProvider.P_AnrMon_Log_AnrMonTimer4)
            AnrMonStartStopp()
            AnrMonError = True
            ' Erneute Verbindung aufbauen per Timer
            Restart(True)
        End Try

        CheckAnrMonTCPSocket.Close()
        RemoteEP = Nothing
        IPHostInfo = Nothing

#If OVer < 14 Then
        C_GUI.SetAnrMonButton()
#Else
        C_GUI.RefreshRibbon()
#End If
    End Sub

    Private Sub TimerReStart_Elapsed(sender As Object, e As Timers.ElapsedEventArgs) Handles TimerReStart.Elapsed
        Dim ReStartError As Boolean

        If RestartCounter < DataProvider.P_Def_TryMaxRestart Then
            If C_DP.P_CBForceFBAddr Then
                C_hf.httpGET("http://" & C_DP.P_TBFBAdr, C_DP.P_EncodingFritzBox, ReStartError)
            Else
                ReStartError = Not C_hf.Ping(C_DP.P_TBFBAdr)
            End If

            If ReStartError Then
                ' Fehler! Verbindung zur Fritz!Box konnte nach dem Verbindungsverlust noch nicht wieder aufgebaut werden. Weitere Versuche werden folgen.
                C_hf.LogFile(DataProvider.P_ReStart_Log_Timer1)
                RestartCounter += 1
            Else
                ' Erfolg! Verbindung zur Fritz!Box konnte nach dem Verbindungsverlust wieder aufgebaut werden.
                C_hf.LogFile(DataProvider.P_ReStart_Log_Timer2)

                ' Beende Timer
                TimerReStart = C_hf.KillTimer(TimerReStart)

                ' Starte Anrufmonitor
                If C_DP.P_CBAnrMonAuto And C_DP.P_CBUseAnrMon Then
                    C_hf.LogFile(DataProvider.P_ReStart_Log_Timer4)
                    AnrMonStartStopp()
                End If

                ' Auswertung der Anrufliste anstoßen
                If F_AnrListImport IsNot Nothing AndAlso C_DP.P_CBAutoAnrList Then
                    C_hf.LogFile(DataProvider.P_ReStart_Log_Timer5)
                    F_AnrListImport.StartAuswertung(False)
                End If
            End If
        Else
            ' Fehler! Verbindung zur Fritz!Box konnte final nach dem Verbindungsverlust nicht wieder aufgebaut werden.
            C_hf.LogFile(DataProvider.P_ReStart_Log_Timer3)
            TimerReStart = C_hf.KillTimer(TimerReStart)
        End If
    End Sub
#End Region

#Region "Anrufmonitor Grundlagen"

    Friend Sub AnrMonStartStopp()
        Dim FBFehler As Boolean

        ' Beenden
        If AnrMonAktiv Then
            ' Timer stoppen, TCP/IP-Verbindung(schließen)
            AnrMonAktiv = False
            If TimerCheckAnrMon IsNot Nothing Then
                C_hf.KillTimer(TimerCheckAnrMon)
                TimerCheckAnrMon = Nothing
            End If

            If AnrMonStream IsNot Nothing Then
                AnrMonStream.Close()
                AnrMonStream = Nothing
            End If

#If OVer < 14 Then
            C_GUI.SetAnrMonButton()
#Else
            C_GUI.RefreshRibbon()
#End If
        Else
            ' TCP/IP-Verbindung öffnen
            If C_DP.P_CBUseAnrMon Then
                ' Prüfe ob Fritz!Box erreichbar
                If C_DP.P_CBForceFBAddr Then
                    C_hf.httpGET("http://" & C_DP.P_TBFBAdr, C_DP.P_EncodingFritzBox, FBFehler)
                Else
                    FBFehler = Not C_hf.Ping(C_DP.P_TBFBAdr)
                End If

                If Not FBFehler Or C_DP.P_CBPhonerAnrMon Then
                    BWStartTCPReader = New BackgroundWorker
                    With BWStartTCPReader
                        .WorkerReportsProgress = True
                        .RunWorkerAsync()
                    End With
                Else
                    AnrMonAktiv = False
                    AnrMonError = True
                End If
            End If
        End If
    End Sub

    Friend Sub Restart(ByVal UseTimer As Boolean)
        If UseTimer Then
            If TimerReStart Is Nothing Then
                RestartCounter = 1
                TimerReStart = C_hf.SetTimer(DataProvider.P_Def_ReStartIntervall)
            Else
                TimerReStart = C_hf.KillTimer(TimerReStart)
                Restart(True)
            End If
        Else
            AnrMonStartStopp() ' Ausschalten
            AnrMonStartStopp() ' Einschalten 
        End If
    End Sub
#End Region

#Region "Anrufmonitor"
    ''' <summary>
    ''' Hauptfunktion des Anrufmonitors. Ruft, je nach eingehenden String, die jeweilige Funktion auf.
    ''' </summary>
    Private Sub AnrMonAktion()
        ' schaut in der FritzBox im Port 1012 nach und startet entsprechende Unterprogramme
        Dim r As New StreamReader(AnrMonStream)
        Dim FBStatus As String  ' Status-String der FritzBox
        Dim aktZeile() As String  ' aktuelle Zeile im Status-String

        Do
            If AnrMonStream.DataAvailable And AnrMonAktiv Then
                FBStatus = r.ReadLine
                Select Case FBStatus
                    Case DataProvider.P_AnrMon_AnrMonPhonerWelcome '"Welcome to Phoner"
                        AnrMonPhoner = True
                    Case DataProvider.P_AnrMon_AnrMonPhonerError '"Sorry, too many clients"
                        C_hf.LogFile(DataProvider.P_AnrMon_Log_AnrMonPhoner1)
                    Case Else
                        C_hf.LogFile("AnrMonAktion: " & FBStatus)
                        aktZeile = Split(FBStatus, ";", , CompareMethod.Text)
                        If Not aktZeile.Length = 1 Then
                            'Schauen ob "RING", "CALL", "CONNECT" oder "DISCONNECT" übermittelt wurde
                            Select Case CStr(aktZeile.GetValue(1))
                                Case DefAnrMon.FB_RING '"RING"
                                    AnrMonRING(aktZeile)
                                Case DefAnrMon.FB_CALL '"CALL"
                                    AnrMonCALL(aktZeile)
                                Case DefAnrMon.FB_CONNECT '"CONNECT"
                                    AnrMonCONNECT(aktZeile)
                                Case DefAnrMon.FB_DISCONNECT '"DISCONNECT"
                                    AnrMonDISCONNECT(aktZeile)
                            End Select
                        End If
                End Select
            End If
            C_hf.ThreadSleep(50)
            Windows.Forms.Application.DoEvents()
        Loop While AnrMonAktiv
        r.Close()
        r = Nothing
    End Sub

    ''' <summary>
    ''' Behandelt den vom Anrufmonitor der Fritz!Box erhaltener String für RING.
    ''' Routie wertet einen eingehenden Anruf aus.
    ''' </summary>
    ''' <param name="FBStatus">String: Vom Anrufmonitor der Fritz!Box erhaltener String für RING
    ''' FBStatus(0): Uhrzeit
    ''' FBStatus(1): RING, wird nicht verwendet
    ''' FBStatus(2): Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
    ''' FBStatus(3): Eingehende Telefonnummer, TelNr
    ''' FBStatus(4): Angerufene eigene Telefonnummer, MSN
    ''' FBStatus(5): Anschluss, SIP...
    ''' </param>
    Friend Sub AnrMonRING(ByVal FBStatus As String())

        Dim MSN As String = C_hf.EigeneVorwahlenEntfernen(FBStatus(4))
        Dim ID As Integer = CInt(FBStatus(2))

        Dim Telefonat As C_Telefonat

        ' Prüfe ob Telefonatsliste bereits eine nicht beendetes Telefonat mit gleicher ID enthalten ist
        Telefonat = C_Popup.TelefonatsListe.Find(Function(tmpTel) tmpTel.ID = ID And Not tmpTel.Beendet)
        If Telefonat IsNot Nothing Then
            ' Eigentlich (!) sollte er hier nicht reinlaufen
            C_hf.LogFile(DataProvider.P_AnrMon_Log_TelList1("RING", CStr(ID)))
            ' Wenn ein Telefonat hier gefunden wurde, dann muss es bereits beendet sein. Ansonsten hätte die Fritz!Box eine Andere ID gesendet
            ' Wenn das Telefonat eine Stoppuhr ond/oder eine Anrufmonitor besitzt, dann ist das Telefonat nicht aus der Liste zu entfernen.
            If Telefonat.PopupAnrMon Is Nothing And Telefonat.PopupStoppuhr Is Nothing Then
                C_Popup.TelefonatsListe.Remove(Telefonat)
            Else
                Telefonat.Beendet = True
            End If
            ' Telefonat aufräumen
            Telefonat = Nothing
        End If

        ' Anruf nur anzeigen, wenn die MSN stimmt
        If C_DP.P_CLBTelNr.Contains(MSN) Or AnrMonPhoner Then
            Telefonat = New C_Telefonat
            C_Popup.TelefonatsListe.Add(Telefonat)

            With Telefonat
                .Typ = C_Telefonat.AnrufRichtung.Eingehend
                Try
                    .Zeit = CDate(FBStatus(0))
                Catch ex As InvalidCastException
                    C_hf.LogFile("AnrMonRING: Das von der Fritz!Box übermitteltet Datum " & FBStatus(0) & " kann nicht in ein Date-Datentyp umgewandelt werden. Die Systemzeit wird verwendet.")
                    .Zeit = System.DateTime.Now
                End Try

                .MSN = MSN
                .TelName = C_hf.TelefonName(.MSN)
                .ID = ID
                .TelNr = FBStatus(3)
                .Online = C_hf.IIf(.ID < DataProvider.P_Def_AnrListIDOffset, True, False)
                .RingTime = DataProvider.P_Def_ErrorMinusOne_Integer
                ' Phoner
                If AnrMonPhoner Then
                    Dim PhonerTelNr As Helfer.Telefonnummer
                    Dim pos As Integer = InStr(.TelNr, "@", CompareMethod.Text)
                    If Not pos = 0 Then
                        .TelNr = Left(.TelNr, pos - 1)
                    Else
                        PhonerTelNr = C_hf.TelNrTeile(.TelNr)
                        If Not PhonerTelNr.Ortsvorwahl = DataProvider.P_Def_LeerString Then .TelNr = PhonerTelNr.Ortsvorwahl & Mid(.TelNr, InStr(.TelNr, ")", CompareMethod.Text) + 2)
                        If Not PhonerTelNr.Landesvorwahl = DataProvider.P_Def_LeerString Then .TelNr = PhonerTelNr.Landesvorwahl & Mid(.TelNr, 2)
                    End If
                    .TelNr = C_hf.nurZiffern(.TelNr)
                End If
                ' Ende Phoner

                If Len(.TelNr) = 0 Then .TelNr = DataProvider.P_Def_StringUnknown

                ' Daten für Anzeige im Anrurfmonitor speichern
                If .Online AndAlso Not C_OlI.VollBildAnwendungAktiv Then
                    .AnrMonAusblenden = True
                    LetzterAnrufer = Telefonat
                    C_Popup.AnrMonEinblenden(Telefonat)
                End If

                ' Daten in den Kontakten suchen und per Rückwärtssuche ermitteln
                If Not .TelNr = DataProvider.P_Def_StringUnknown Then
                    ' Anrufer in den Outlook-Kontakten suchen
                    .olContact = C_KF.KontaktSuche(.TelNr, DataProvider.P_Def_ErrorMinusOne_String, .KontaktID, .StoreID, C_DP.P_CBKHO)
                    If .olContact IsNot Nothing Then
                        .Anrufer = .olContact.FullName
                        .Firma = .olContact.CompanyName
                        If C_DP.P_CBIgnoTelNrFormat Then .TelNr = C_hf.FormatTelNr(.TelNr)
                    Else
                        ' Anrufer per Rückwärtssuche ermitteln
                        If C_DP.P_CBRWS AndAlso F_RWS.AnrMonRWS(Telefonat) Then

                            If C_DP.P_CBKErstellen Then
                                ' Im folgenden wird automatisch ein Kontakt erstellt, der durch die Rückwärtssuche ermittlt wurde. 
                                ' Dies geschieht nur, wenn es gewünscht ist.
                                .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, True)
                                .vCard = DataProvider.P_Def_LeerString
                                .Firma = .olContact.CompanyName
                                .Anrufer = .olContact.FullName 'Replace(.olContact.FullName & " (" & .Companies & ")", " ()", "")
                            Else
                                .Anrufer = ReadFNfromVCard(.vCard)
                                .Anrufer = Replace(.Anrufer, Chr(13), "", , , CompareMethod.Text)
                                If .Anrufer.StartsWith("Firma") Then .Anrufer = Mid(.Anrufer, Len("Firma"))
                                .Anrufer = Trim(.Anrufer)
                            End If

                        End If
                        'Formatiere die Telefonnummer
                        .TelNr = C_hf.FormatTelNr(.TelNr)
                    End If
                    ' Hier Anrufmonitor aktualisieren! Nicht beim Journalimport!
                    If Telefonat.PopupAnrMon IsNot Nothing Then
                        C_Popup.UpdateAnrMon(Telefonat)
                    End If

                    LetzterAnrufer = Telefonat
                    SpeichereLetzerAnrufer(Telefonat)

                    ' Update der Liste bei der Listenauswertung nur wenn gewünscht
                    If (.ID < DataProvider.P_Def_AnrListIDOffset) OrElse (.ID >= DataProvider.P_Def_AnrListIDOffset And C_DP.P_CBAnrListeUpdateCallLists) Then
                        C_GUI.UpdateList(DataProvider.P_Def_NameListRING, Telefonat)
                    End If

#If OVer < 14 Then
                    If C_DP.P_CBSymbAnrListe Then C_GUI.FillPopupItems(DataProvider.P_Def_NameListRING)
#End If
                End If
                ' Kontakt anzeigen
                If C_DP.P_CBAnrMonZeigeKontakt And .Online Then
                    If .olContact Is Nothing Then .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False)

#If Not OVer = 11 Then
                    If C_DP.P_CBNote Then C_KF.AddNote(.olContact)
#End If
                    Try
                        ' Anscheinend wird nach dem Einblenden ein Save ausgeführt, welchses eine Indizierung zur Folge hat.
                        ' Grund für den Save-Forgang ist unbekannt.
                        .olContact.Display()
                    Catch Err As Exception
                        C_hf.LogFile(DataProvider.P_AnrMon_Log_AnrMon1("AnrMonRING", Err.Message))
                    End Try
                End If

                'Notizeintag
#If Not OVer = 11 Then
                If C_DP.P_CBNote AndAlso .olContact IsNot Nothing Then
                    C_KF.FillNote(AnrMonEvent.AnrMonRING, Telefonat, C_DP.P_CBAnrMonZeigeKontakt)
                End If
#End If
            End With
        End If
    End Sub '(AnrMonRING)

    ''' <summary>
    ''' Behandelt den vom Anrufmonitor der Fritz!Box erhaltener String für CALL.
    ''' Diese Routine wertet einen ausgehenden Anruf aus.
    ''' </summary>
    ''' <param name="FBStatus">String: Vom Anrufmonitor der Fritz!Box erhaltener String für CALL
    ''' FBStatus(0): Uhrzeit
    ''' FBStatus(1): CALL, wird nicht verwendet
    ''' FBStatus(2): Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
    ''' FBStatus(3): Nebenstellennummer, eindeutige Zuordnung des Telefons
    ''' FBStatus(4): Ausgehende eigene Telefonnummer, MSN
    ''' FBStatus(5): die gewählte Rufnummer
    ''' </param>
    Friend Sub AnrMonCALL(ByVal FBStatus As String())

        Dim MSN As String = C_hf.EigeneVorwahlenEntfernen(FBStatus(4))  ' Ausgehende eigene Telefonnummer, MSN
        Dim ID As Integer = CInt(FBStatus(2))
        Dim xPathTeile As New ArrayList
        Dim Telefonat As C_Telefonat

        ' Prüfe ob Telefonatsliste bereits eine nicht beendetes Telefonat mit gleicher ID enthalten ist
        Telefonat = C_Popup.TelefonatsListe.Find(Function(tmpTel) tmpTel.ID = ID And Not tmpTel.Beendet)
        If Telefonat IsNot Nothing Then
            ' Eigentlich (!) sollte er hier nicht reinlaufen
            C_hf.LogFile(DataProvider.P_AnrMon_Log_TelList1("CALL", CStr(ID)))
            ' Wenn ein Telefonat hier gefunden wurde, dann muss es bereits beendet sein. Ansonsten hätte die Fritz!Box eine Andere ID gesendet
            ' Wenn das Telefonat eine Stoppuhr ond/oder eine Anrufmonitor besitz, dann ist das Telefonat nicht aus der Liste zu entfernen.
            If Telefonat.PopupAnrMon Is Nothing And Telefonat.PopupStoppuhr Is Nothing Then
                C_Popup.TelefonatsListe.Remove(Telefonat)
            Else
                Telefonat.Beendet = True
            End If
            ' Telefonat aufräumen
            Telefonat = Nothing
        End If

        If C_DP.P_CLBTelNr.Contains(MSN) Or AnrMonPhoner Then
            Telefonat = New C_Telefonat
            With Telefonat
                Try
                    .Zeit = CDate(FBStatus(0))
                Catch ex As InvalidCastException
                    C_hf.LogFile("AnrMonCALL: Das von der Fritz!Box übermitteltet Datum " & FBStatus(0) & " kann nicht in ein Date-Datentyp umgewandelt werden. Die Systemzeit wird verwendet.")
                    .Zeit = System.DateTime.Now
                End Try
                .ID = ID
                .NSN = CInt(FBStatus(3))
                .MSN = MSN
                .Typ = C_Telefonat.AnrufRichtung.Ausgehend
                .Online = C_hf.IIf(.ID < DataProvider.P_Def_AnrListIDOffset, True, False)
                .RingTime = DataProvider.P_Def_ErrorMinusOne_Integer
                ' Problem DECT/IP-Telefone: keine MSN  über Anrufmonitor eingegangen. Aus Datei ermitteln.
                If .MSN = DataProvider.P_Def_LeerString Then
                    Select Case .NSN
                        Case 0 To 2 ' FON1-3
                            .NSN += 1
                        Case 10 To 19 ' DECT
                            .NSN += 50
                    End Select
                    Select Case .NSN
                        Case 3, 4, 5, 36, 37
                            .MSN = DataProvider.P_Def_ErrorMinusOne_String
                        Case Else
                            With xPathTeile
                                .Clear()
                                .Add("Telefone")
                                .Add("Telefone")
                                .Add("*")
                                .Add("Telefon[@Dialport = """ & Telefonat.NSN & """]")
                                .Add("TelNr")
                                Telefonat.MSN = C_XML.Read(C_DP.XMLDoc, xPathTeile, "")
                            End With
                    End Select
                End If

                .TelNr = C_hf.nurZiffern(FBStatus(5))
                If .TelNr = DataProvider.P_Def_LeerString Then .TelNr = DataProvider.P_Def_StringUnknown
                ' CbC-Vorwahl entfernen
                If .TelNr.StartsWith("0100") Then .TelNr = Right(.TelNr, Len(.TelNr) - 6)
                If .TelNr.StartsWith("010") Then .TelNr = Right(.TelNr, Len(.TelNr) - 5)
                If Not .TelNr.StartsWith("0") And Not .TelNr.StartsWith("11") And Not .TelNr.StartsWith("+") Then .TelNr = C_DP.P_TBVorwahl & .TelNr

                ' Doppelkreuz (#) entfernen
                .TelNr = .TelNr.Trim(Chr(35)) ' "#" 
                ' Daten zurücksetzen

                If Not .TelNr = DataProvider.P_Def_StringUnknown Then
                    .olContact = C_KF.KontaktSuche(.TelNr, DataProvider.P_Def_ErrorMinusOne_String, .KontaktID, .StoreID, C_DP.P_CBKHO)
                    If Telefonat.olContact IsNot Nothing Then
                        .Anrufer = Replace(.olContact.FullName & " (" & .olContact.CompanyName & ")", " ()", "")
                        If C_DP.P_CBIgnoTelNrFormat Then .TelNr = C_hf.FormatTelNr(.TelNr)
                    Else
                        ' .Anrufer per Rückwärtssuche ermitteln
                        If C_DP.P_CBRWS AndAlso F_RWS.AnrMonRWS(Telefonat) Then

                            If C_DP.P_CBKErstellen Then
                                ' Im folgenden wird automatisch ein Kontakt erstellt, der durch die Rückwärtssuche ermittlt wurde. 
                                ' Dies geschieht nur, wenn es gewünscht ist.
                                .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, True)
                                .vCard = DataProvider.P_Def_LeerString
                                .Firma = .olContact.CompanyName
                                .Anrufer = Replace(.olContact.FullName & " (" & .Firma & ")", " ()", "")
                            Else
                                .Anrufer = ReadFNfromVCard(.vCard)
                                .Anrufer = Replace(.Anrufer, Chr(13), "", , , CompareMethod.Text)
                                If InStr(1, .Anrufer, "Firma", CompareMethod.Text) = 1 Then .Anrufer = Right(.Anrufer, Len(.Anrufer) - 5)
                                .Anrufer = Trim(.Anrufer)
                            End If

                        End If
                        .TelNr = C_hf.FormatTelNr(.TelNr)
                    End If
                End If
                ' Daten im Menü für Wahlwiederholung speichern
                ' Update der Liste bei der Listenauswertung nur wenn gewünscht
                If (.ID < DataProvider.P_Def_AnrListIDOffset) OrElse (.ID >= DataProvider.P_Def_AnrListIDOffset And C_DP.P_CBAnrListeUpdateCallLists) Then
                    C_GUI.UpdateList(DataProvider.P_Def_NameListCALL, Telefonat)
                End If
#If OVer < 14 Then
                If C_DP.P_CBSymbWwdh Then C_GUI.FillPopupItems(DataProvider.P_Def_NameListCALL)
#End If
                ' Kontakt öffnen
                If C_DP.P_CBAnrMonZeigeKontakt And .Online Then
                    If .olContact Is Nothing Then
                        .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False)
                    End If
#If OVer > 11 Then
                    If C_DP.P_CBNote Then C_KF.AddNote(.olContact)
#End If
                    Try
                        .olContact.Display()
                    Catch Err As Exception
                        C_hf.LogFile(DataProvider.P_AnrMon_Log_AnrMon1("AnrMonCALL", Err.Message))
                    End Try
                End If
                'Notizeintag
#If OVer > 11 Then
                If C_DP.P_CBNote AndAlso .olContact IsNot Nothing Then
                    C_KF.FillNote(AnrMonEvent.AnrMonCALL, Telefonat, C_DP.P_CBAnrMonZeigeKontakt)
                End If
#End If
            End With
            xPathTeile = Nothing
            C_Popup.TelefonatsListe.Add(Telefonat)
        End If
    End Sub '(AnrMonCALL)

    ''' <summary>
    ''' Behandelt den vom Anrufmonitor der Fritz!Box erhaltener String für CONNECT
    ''' Diese Routine wertet eine Zustande gekommene Verbindung aus.
    ''' </summary>
    ''' <param name="FBStatus">String(): Vom Anrufmonitor der Fritz!Box erhaltener String für CONNECT
    ''' FBStatus(0): Uhrzeit
    ''' FBStatus(1): CONNECT, wird nicht verwendet
    ''' FBStatus(2): Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
    ''' FBStatus(3): Nebenstellennummer, eindeutige Zuordnung des Telefons
    ''' FBStatus(4): Gewählte Nummer Telefonnummer bzw. eingehende Telefonnummer
    ''' </param>
    Friend Sub AnrMonCONNECT(ByVal FBStatus As String())

        Dim xPathTeile As New ArrayList
        Dim Telefonat As C_Telefonat
        Dim tmpDate As Date
        ' 140824:
        ' Achtung: ID Reicht nicht aus.

        Telefonat = C_Popup.TelefonatsListe.Find(Function(tmpTel) tmpTel.ID = CInt(FBStatus(2)) And Not tmpTel.Beendet)
        If Telefonat IsNot Nothing Then
            With Telefonat
                ' Temporärer Test ob Nummern identisch

                ' Nurzuffern und eigene Vorwahl für Vergleich entfernen.
                If Not C_hf.EigeneVorwahlenEntfernen(C_hf.nurZiffern(.TelNr)).Equals(CStr(FBStatus.GetValue(4)).Replace("#", DataProvider.P_Def_LeerString)) Then
                    C_hf.LogFile("AnrMonCONNECT: Verbundene Nummer nicht mit hinterlegter Nummer identisch: " & .TelNr & " <> " & FBStatus(4))
                End If

                .Angenommen = True

                Try
                    tmpDate = CDate(FBStatus(0))
                Catch ex As InvalidCastException
                    C_hf.LogFile("AnrMonCONNECT: Das von der Fritz!Box übermitteltet Datum " & FBStatus(0) & " kann nicht in ein Date-Datentyp umgewandelt werden. Die Systemzeit wird verwendet.")
                    tmpDate = System.DateTime.Now
                End Try

                .RingTime = CType(.Zeit - tmpDate, TimeSpan).TotalSeconds
                .Zeit = tmpDate

                If AnrMonPhoner Then
                    '.telname = "Phoner"
                Else
                    .NSN = CInt(FBStatus(3))

                    Select Case .NSN
                        Case 0 To 2 ' FON1-3
                            .NSN += 1
                        Case 10 To 19 ' DECT
                            .NSN += 50
                    End Select

                    Select Case .NSN
                        Case 3
                            .TelName = "Durchwahl"
                        Case 4
                            .TelName = "ISDN Gerät"
                        Case 5
                            .TelName = "Fax (intern/PC)"
                        Case 36
                            .TelName = "Data S0"
                        Case 37
                            .TelName = "Data PC"
                        Case Else
                            With xPathTeile
                                .Clear()
                                .Add("Telefone")
                                .Add("Telefone")
                                .Add("*")
                                .Add("Telefon[@Dialport = """ & Telefonat.NSN & """]")
                                .Add("TelName")
                            End With
                            .TelName = C_XML.Read(C_DP.XMLDoc, xPathTeile, "")
                    End Select
                End If

                If .Typ = C_Telefonat.AnrufRichtung.Eingehend Then
                    LetzterAnrufer = Telefonat
                    SpeichereLetzerAnrufer(Telefonat)
                End If

                ' StoppUhr einblenden
                If C_DP.P_CBStoppUhrEinblenden And .Online Then
                    If (Not .NSN = 5) Or (.NSN = 5 And Not C_DP.P_CBStoppUhrIgnIntFax) Then
                        C_hf.LogFile(DataProvider.P_AnrMon_Log_AnrMonStoppUhr1)
                        C_Popup.StoppuhrEinblenden(Telefonat)
                    Else
                        C_hf.LogFile(DataProvider.P_AnrMon_Log_AnrMonStoppUhr2)
                    End If
                End If

                'Notizeintag
#If Not OVer = 11 Then
                If C_DP.P_CBNote Then
                    If .olContact IsNot Nothing Then
                        C_KF.FillNote(AnrMonEvent.AnrMonCONNECT, Telefonat, C_DP.P_CBAnrMonZeigeKontakt)
                    End If
                End If
#End If
            End With
        End If

        xPathTeile = Nothing
    End Sub '(AnrMonCONNECT)

    ''' <summary>
    ''' Behandelt den vom Anrufmonitor der Fritz!Box erhaltener String für DISCONNECT
    ''' </summary>
    ''' <param name="FBStatus">String: Vom Anrufmonitor der Fritz!Box erhaltener String für DISCONNECT
    ''' FBStatus(0): Uhrzeit
    ''' FBStatus(1): DISCONNECT, wird nicht verwendet
    ''' FBStatus(2): Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
    ''' FBStatus(3): Dauer des Telefonates
    ''' </param>
    Friend Sub AnrMonDISCONNECT(ByVal FBStatus As String())

        Dim CallDirection As String = DataProvider.P_Def_LeerString
        Dim ZeitOutlookBeendet As Date = C_DP.P_StatOLClosedZeit

        Dim xPathTeile As New ArrayList
        Dim Telefonat As C_Telefonat
        Dim tmpDate As Date

        Telefonat = C_Popup.TelefonatsListe.Find(Function(tmpTel) tmpTel.ID = CInt(FBStatus(2)) And Not tmpTel.Beendet)

        If Telefonat IsNot Nothing Then
            With Telefonat
                ' Setze Telefonat auf Beendet
                .Beendet = True

                ' Bestimmte die Zeit, des Klingelns [(DIS)CONNECT) - RING]
                If .RingTime = DataProvider.P_Def_ErrorMinusOne_Integer Then
                    Try
                        tmpDate = CDate(FBStatus(0))
                    Catch ex As InvalidCastException
                        C_hf.LogFile("AnrMonDISCONNECT: Das von der Fritz!Box übermitteltet Datum " & FBStatus(0) & " kann nicht in ein Date-Datentyp umgewandelt werden. Die Systemzeit wird verwendet.")
                        tmpDate = System.DateTime.Now
                    End Try

                    .RingTime = CType(.Zeit - tmpDate, TimeSpan).TotalSeconds
                End If

                ' Regel für verpasstes Telefonat:
                ' 1. Flag "Angenommen" ist false
                ' 2. Flag "Angenommen" ist True, und RingTime > Grenzwert aus Einstellungen (Angenommen)

                ' Setze Verpasst-Marker
                If Not .Angenommen Xor C_DP.P_TBAnrBeantworterTimeout <= .RingTime Then
                    .Verpasst = True
                End If

                If C_DP.P_CBJournal Then
                    .Dauer = C_hf.IIf(CInt(FBStatus(3)) <= 30, 31, CInt(FBStatus(3))) \ 60
                    .Body = DataProvider.P_AnrMon_AnrMonDISCONNECT_JournalBody(.TelNr, .Angenommen)
                    If Not .vCard = DataProvider.P_Def_LeerString And Not .vCard = DataProvider.P_Def_ErrorMinusTwo_String Then
                        .Firma = ReadFromVCard(.vCard, "ORG", "")
                        .Body += DataProvider.P_AnrMon_AnrMonDISCONNECT_Journal & vbCrLf & .vCard & vbCrLf
                    Else
                        If .olContact IsNot Nothing Then
                            If .olContact.FullName = DataProvider.P_Def_LeerString Then
                                .Anrufer = C_hf.IIf(.olContact.Companies = DataProvider.P_Def_LeerString, .TelNr, .Firma)
                            Else
                                .Anrufer = .olContact.FullName
                            End If

                            If .Firma = DataProvider.P_Def_LeerString Then
                                If Not .olContact.HomeAddress = DataProvider.P_Def_LeerString Then
                                    .Body += DataProvider.P_AnrMon_Journal_Kontaktdaten & _
                                        DataProvider.P_Def_EineNeueZeile & .Anrufer & _
                                        DataProvider.P_Def_EineNeueZeile & .Firma & _
                                        DataProvider.P_Def_EineNeueZeile & .olContact.HomeAddress & _
                                        DataProvider.P_Def_EineNeueZeile
                                End If
                            Else
                                If Not .olContact.BusinessAddress = DataProvider.P_Def_LeerString Then
                                    .Body += DataProvider.P_AnrMon_Journal_Kontaktdaten & _
                                        DataProvider.P_Def_EineNeueZeile & .Anrufer & _
                                        DataProvider.P_Def_EineNeueZeile & .olContact.BusinessAddress & _
                                        DataProvider.P_Def_EineNeueZeile
                                End If
                            End If
                        End If
                    End If

                    If .Angenommen Then
                        With xPathTeile
                            .Clear()
                            .Add("Telefone")
                            .Add("Telefone")
                            .Add("*")
                            .Add("Telefon[TelName = """ & Telefonat.TelName & """]")
                        End With

                        If C_XML.GetProperXPath(C_DP.XMLDoc, xPathTeile) Then
                            ' xPathTeile hat sich durch GetProperXPath geändert.
                            xPathTeile.Add(C_hf.IIf(Telefonat.Typ = C_Telefonat.AnrufRichtung.Eingehend, C_Telefonat.AnrufRichtung.Eingehend.ToString, C_Telefonat.AnrufRichtung.Ausgehend.ToString))

                            With C_DP
                                C_XML.Write(.XMLDoc, xPathTeile, CStr(CInt(C_XML.Read(.XMLDoc, xPathTeile, CStr(0))) + Telefonat.Dauer * 60))
                            End With
                        End If

                        CallDirection = C_hf.IIf(Telefonat.Typ = C_Telefonat.AnrufRichtung.Eingehend, DataProvider.P_Def_Journal_Text_Eingehend, DataProvider.P_Def_Journal_Text_Ausgehend)
                    Else
                        If .Typ = C_Telefonat.AnrufRichtung.Eingehend Then
                            C_DP.P_StatVerpasst += 1
                            CallDirection = DataProvider.P_Def_Journal_Text_Verpasst
                        Else
                            C_DP.P_StatNichtErfolgreich += 1
                            CallDirection = DataProvider.P_Def_Journal_Text_NichtErfolgreich
                        End If
                    End If

                    .Categories = .TelName & "; " & String.Join("; ", DataProvider.P_AnrMon_Journal_Def_Categories.ToArray)
                    .Subject = CallDirection & C_hf.IIf(.Anrufer = DataProvider.P_Def_LeerString, .TelNr, .Anrufer & " (" & .TelNr & ")") & C_hf.IIf(Split(.TelName, ";", , CompareMethod.Text).Length = 1, DataProvider.P_Def_LeerString, " (" & .TelName & ")")

                    C_OlI.ErstelleJournalEintrag(Telefonat)
                    C_DP.P_StatJournal += 1

                    ' Erstellung des Journaleintrages abgeschlossen
                End If

                ' Setze ZeitOutlookBeendet auf Jetzt + 1 Munitem wenn Anrufzeit nach der vorhanden ZeitOutlookBeendet oder ZeitOutlookBeendet Jetzt ust
                If .Zeit > ZeitOutlookBeendet Or ZeitOutlookBeendet = System.DateTime.Now Then C_DP.P_StatOLClosedZeit = System.DateTime.Now.AddMinutes(1)

                ' Stoppuhr anhalten
                If C_DP.P_CBStoppUhrEinblenden And .Online AndAlso Telefonat.PopupStoppuhr IsNot Nothing Then
                    Telefonat.PopupStoppuhr.StoppuhrStopp()
                End If

                ' Sonderbehandlung für eingehende Telefonate
                If .Typ = C_Telefonat.AnrufRichtung.Eingehend Then

                    LetzterAnrufer = Telefonat
                    SpeichereLetzerAnrufer(Telefonat)

                    If .Verpasst Then
                        C_hf.LogFile("Ein verpasstes Telefonat wurde erkannt: ID: " & .ID & ", " & .TelNr)
                        ' Verpassten Anruf über Anrufmonitor anzeigen
                        If C_DP.P_CBAnrListeShowAnrMon Then
                            .AnrMonAusblenden = False
                            ' Prüfung ob PopUp vorhanden
                            If .PopupAnrMon IsNot Nothing AndAlso Not C_OlI.VollBildAnwendungAktiv() Then
                                'Nein: Neu erstellen
                                ' ToDo Offline Telefonate prüfen
                                C_hf.LogFile("Der Anrufmonitor wurde erneut zur Signalisierung eines verpassten Anrufes eingeblendet.")
                                C_Popup.AnrMonEinblenden(Telefonat)
                            End If
                        End If
                    End If

                    If .Online OrElse (Not .Online And C_DP.P_CBAnrListeUpdateCallLists) Then
                        C_GUI.UpdateList(DataProvider.P_Def_NameListRING, Telefonat)
                    End If

                    If C_DP.P_CBAnrMonCloseAtDISSCONNECT And .AnrMonAusblenden And .PopupAnrMon IsNot Nothing Then
                        .PopupAnrMon.Hide()
                    End If

                End If

                'Notizeintag
#If OVer > 11 Then
                If C_DP.P_CBNote Then
                    If .olContact IsNot Nothing Then
                        C_KF.FillNote(AnrMonEvent.AnrMonDISCONNECT, Telefonat, C_DP.P_CBAnrMonZeigeKontakt)
                    End If
                End If
#End If
                If .PopupAnrMon Is Nothing And .PopupStoppuhr Is Nothing Then
                    C_Popup.TelefonatsListe.Remove(Telefonat)
                Else
                    C_hf.LogFile("AnrMonDISCONNECT: Telefonat " & .ID & ":" & .TelNr & " nicht aus der Liste entfernt. AnrMon: " & CStr(.PopupAnrMon Is Nothing) & " Stoppuhr: " & CStr(.PopupStoppuhr Is Nothing))
                End If
            End With
        Else
            If C_DP.P_CBJournal And C_DP.P_CLBTelNr.Contains(FBStatus(3)) Then
                C_hf.LogFile("AnrMonDISCONNECT: " & DataProvider.P_AnrMon_AnrMonDISCONNECT_Error)
                ' Wenn Anruf vor dem Outlookstart begonnen wurde, wurde er nicht nachträglich importiert.
                Try
                    tmpDate = CDate(FBStatus(0))
                Catch ex As InvalidCastException
                    C_hf.LogFile("AnrMonDISCONNECT: Das von der Fritz!Box übermitteltet Datum " & FBStatus(0) & " kann nicht in ein Date-Datentyp umgewandelt werden. Die Systemzeit wird verwendet.")
                    tmpDate = System.DateTime.Now
                End Try

                Dim DauerAnruf As Integer = C_hf.IIf(CInt(FBStatus(3)) <= 30, 31, CInt(FBStatus(3))) \ 60
                tmpDate = tmpDate.AddSeconds(-1 * (tmpDate.Second + DauerAnruf + 70))
                If tmpDate < ZeitOutlookBeendet Then C_DP.P_StatOLClosedZeit = tmpDate
            End If
        End If

        xPathTeile = Nothing
    End Sub '(AnrMonDISCONNECT)
#End Region

#Region "LetzterAnrufer"
    ''' <summary>
    ''' Speichert den letzten Anrufer ab, den das Addin registriert hat.
    ''' Dies wird benötigt, damit das Addin nach dem Neustart von Outlook, weiß welchen Anrufer es einblenden soll.
    ''' </summary>
    ''' <param name="Telefonat">Das Telefonat, welches gespeichert werden soll.</param>
    Private Sub SpeichereLetzerAnrufer(ByVal Telefonat As C_Telefonat)
        Dim xPathTeile As New ArrayList
        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList

        With Telefonat
            ' Uhrzeit
            NodeNames.Add("Zeit")
            NodeValues.Add(.Zeit)

            ' Anrufername
            If Not .Anrufer = DataProvider.P_Def_LeerString Then
                NodeNames.Add("Anrufer")
                NodeValues.Add(.Anrufer)
            End If

            ' TelNr
            NodeNames.Add("TelNr")
            NodeValues.Add(.TelNr)

            ' MSN
            NodeNames.Add("MSN")
            NodeValues.Add(.MSN)

            ' StoreID
            If Not .StoreID = DataProvider.P_Def_LeerString Then
                NodeNames.Add("StoreID")
                NodeValues.Add(.StoreID)
            End If

            ' KontaktID
            If Not .KontaktID = DataProvider.P_Def_LeerString Then
                NodeNames.Add("KontaktID")
                NodeValues.Add(.KontaktID)
            End If

            ' vCard
            If Not .vCard = DataProvider.P_Def_LeerString Then
                NodeNames.Add("vCard")
                NodeValues.Add(.vCard)
            End If

            ' TelName
            If Not .TelName = DataProvider.P_Def_LeerString Then
                NodeNames.Add("TelName")
                NodeValues.Add(.TelName)
            End If

            ' Companies
            If Not .Firma = DataProvider.P_Def_LeerString Then
                NodeNames.Add("Companies")
                NodeValues.Add(.Firma)
            End If

            AttributeNames.Add("ID")
            AttributeValues.Add("0")
            'AttributeValues.Add(.ID)

            xPathTeile.Add("LetzterAnrufer")
            'xPathTeile.Add("Letzter")
        End With
        With C_DP
            '.Write(xPathTeile, CStr(LetzterAnrufer.ID))
            'xPathTeile.Remove("Letzter")
            C_XML.AppendNode(.XMLDoc, xPathTeile, C_XML.CreateXMLNode(.XMLDoc, "Eintrag", NodeNames, NodeValues, AttributeNames, AttributeValues))
        End With

        xPathTeile = Nothing
        NodeNames = Nothing
        NodeValues = Nothing
        AttributeNames = Nothing
        AttributeValues = Nothing
    End Sub

    ''' <summary>
    ''' Lädt den letzten Anrufer ab, den das Addin registriert hat.
    ''' </summary>
    ''' <returns>Telefonat</returns>
    Friend Function LadeLetzterAnrufer() As C_Telefonat
        LadeLetzterAnrufer = New C_Telefonat
        Dim xPathTeile As New ArrayList
        Dim ListNodeNames As New ArrayList
        Dim ListNodeValues As New ArrayList

        ' Zeit
        ListNodeNames.Add("Zeit")
        ListNodeValues.Add(System.DateTime.Now)

        ' Anrufer
        ListNodeNames.Add("Anrufer")
        ListNodeValues.Add(DataProvider.P_Def_LeerString)

        ' TelNr
        ListNodeNames.Add("TelNr")
        ListNodeValues.Add(DataProvider.P_Def_LeerString)

        ' MSN
        ListNodeNames.Add("MSN")
        ListNodeValues.Add(DataProvider.P_Def_LeerString)

        ' StoreID
        ListNodeNames.Add("StoreID")
        ListNodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String)

        ' KontaktID
        ListNodeNames.Add("KontaktID")
        ListNodeValues.Add(DataProvider.P_Def_ErrorMinusOne_String)

        ' vCard
        ListNodeNames.Add("vCard")
        ListNodeValues.Add(DataProvider.P_Def_LeerString)

        ' TelName
        ListNodeNames.Add("TelName")
        ListNodeValues.Add(DataProvider.P_Def_LeerString)

        ' Companies
        ListNodeNames.Add("Companies")
        ListNodeValues.Add(DataProvider.P_Def_LeerString)

        LadeLetzterAnrufer.ID = 0 'CInt(C_DP.Read("LetzterAnrufer", "Letzter", "0"))
        With xPathTeile
            .Add("LetzterAnrufer")
            .Add("Eintrag")
        End With
        C_XML.ReadXMLNode(C_DP.XMLDoc, xPathTeile, ListNodeNames, ListNodeValues, "ID", CStr(LadeLetzterAnrufer.ID))
        With LadeLetzterAnrufer
            .Beendet = True
            .Zeit = CDate(ListNodeValues.Item(ListNodeNames.IndexOf("Zeit")))
            .Anrufer = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("Anrufer")))
            .TelNr = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("TelNr")))
            .MSN = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("MSN")))
            .StoreID = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("StoreID")))
            .KontaktID = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("KontaktID")))
            .vCard = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("vCard")))
            .TelName = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("TelName")))
            .Firma = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("Companies")))
            .AnrMonAusblenden = True
            If .TelName = DataProvider.P_Def_ErrorMinusOne_String Then .TelName = C_hf.TelefonName(.MSN)

            If C_OlI.OutlookApplication IsNot Nothing Then
                If Not .StoreID = DataProvider.P_Def_ErrorMinusOne_String Then
                    .olContact = C_KF.GetOutlookKontakt(.KontaktID, .StoreID)
                    ' Löche Daten, wenn Kontakt nicht mehr auffindbar ist
                    If .olContact Is Nothing Then
                        .KontaktID = DataProvider.P_Def_LeerString
                        .StoreID = DataProvider.P_Def_LeerString
                        SpeichereLetzerAnrufer(LadeLetzterAnrufer)
                    End If

                ElseIf Not .vCard = DataProvider.P_Def_ErrorMinusOne_String Then
                    'prüfen ob das Sinnvoll ist:
                    '.olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False)
                End If
            End If
        End With
        xPathTeile = Nothing
    End Function
#End Region
End Class
