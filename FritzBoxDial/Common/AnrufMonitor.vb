Imports System.Net
Imports System.IO
Imports System.ComponentModel
Imports System.Net.Sockets
Imports System.Threading
Imports System.Collections.Generic

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
    Private C_KF As Contacts
    Private C_DP As DataProvider
    Private C_hf As Helfer
    Private C_Popup As Popup
#End Region

#Region "Eigene Formulare"
    Private F_RWS As formRWSuche
#End Region

#Region "NetworkStream"
    Private Shared AnrMonStream As NetworkStream
#End Region

#Region "Properties"
    Friend Property AnrMonAktiv() As Boolean
        Get
            Return _AnrMonAktiv
        End Get
        Set(ByVal value As Boolean)
            _AnrMonAktiv = value
        End Set
    End Property
    Friend Property AnrMonError() As Boolean
        Get
            Return _AnrMonError
        End Get
        Set(ByVal value As Boolean)
            _AnrMonError = value
        End Set
    End Property
    Friend Property AnrMonPhoner() As Boolean
        Get
            Return _AnrMonPhoner
        End Get
        Set(ByVal value As Boolean)
            _AnrMonPhoner = value
        End Set
    End Property
    Friend Property LetzterAnrufer As C_Telefonat
        Get
            Return _LetzterAnrufer
        End Get
        Set(ByVal value As C_Telefonat)
            _LetzterAnrufer = value
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
        Private Dymmy As String
#End If
        Friend Const FB_RING As String = "RING"
        Friend Const FB_CALL As String = "CALL"
        Friend Const FB_CONNECT As String = "CONNECT"
        Friend Const FB_DISCONNECT As String = "DISCONNECT"
    End Structure
#End Region

#Region "Globale Variablen"

    Private StandbyCounter As Integer
    Private _AnrMonAktiv As Boolean                    ' damit 'AnrMonAktion' nur einmal aktiv ist
    Private _AnrMonError As Boolean
    Private _AnrMonPhoner As Boolean = False
    Private _LetzterAnrufer As C_Telefonat
#End Region

    Friend Sub New(ByVal DataProvoderKlasse As DataProvider, _
                   ByVal RWS As formRWSuche, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal KontaktKlasse As Contacts, _
                   ByVal InterfacesKlasse As GraphicalUserInterface, _
                   ByVal OutlInter As OutlookInterface, _
                   ByVal PopupKlasse As Popup, _
                   ByVal XMLKlasse As XML)

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
            FBAnrMonPort = C_DP.P_DefaultPhonerAnrMonPort
            'IPAddresse = IPAddress.Loopback ' 127.0.0.1 ' Wert bei "Dim" schon gesetzt
        Else
            FBAnrMonPort = C_DP.P_DefaultFBAnrMonPort
            If Not IPAddress.TryParse(C_DP.P_TBFBAdr, IPAddresse) Then
                ' Versuche über Default-IP zur Fritz!Box zu gelangen
                IPHostInfo = Dns.GetHostEntry(C_DP.P_Def_FritzBoxAdress)
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
                    If FBAnrMonPort = C_DP.P_DefaultFBAnrMonPort Then
                        'Es konnte keine Verbindung hergestellt werden, da der Zielcomputer die Verbindung verweigerte.
                        If C_hf.FBDB_MsgBox(C_DP.P_AnrMon_MsgBox_AnrMonStart1, MsgBoxStyle.YesNo, C_DP.P_AnrMon_MsgBox_AnrMonStart2) = MsgBoxResult.Yes Then
                            BWActivateCallmonitor = New BackgroundWorker
                            With BWActivateCallmonitor
                                .RunWorkerAsync()
                            End With
                        Else
                            C_hf.LogFile(C_DP.P_AnrMon_Log_AnrMonStart1)
                        End If
                    End If
                Case Else
                    C_hf.LogFile(C_DP.P_AnrMon_Log_AnrMonStart2(SocketError.Message))
                    AnrMonError = True
                    e.Result = False
            End Select
        Catch
            C_hf.LogFile(C_DP.P_AnrMon_Log_AnrMonStart3)

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
                If Not C_DP.P_CBPhonerAnrMon Then TimerCheckAnrMon = C_hf.SetTimer(TimeSpan.FromMinutes(C_DP.P_Def_CheckAnrMonIntervall).TotalMilliseconds)
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
        C_GUI.SetAnrMonButton(AnrMonAktiv)
#Else
        C_GUI.RefreshRibbon()
#End If
        If AnrMonAktiv Then
            If TimerReStart IsNot Nothing Then
                TimerReStart = C_hf.KillTimer(TimerReStart)
                C_hf.LogFile(C_DP.P_AnrMon_Log_AnrMonStart4)
            End If
        Else
            C_hf.LogFile(C_DP.P_AnrMon_Log_AnrMonStart5)
        End If
        BWStartTCPReader.Dispose()
    End Sub

    Sub BWActivateCallmonitor_DoWork() Handles BWActivateCallmonitor.DoWork
        C_GUI.P_CallClient.Wählbox(Nothing, C_DP.P_Def_TelCodeActivateFritzBoxCallMonitor, C_DP.P_Def_StringEmpty, True)
        Do
            Windows.Forms.Application.DoEvents()
        Loop Until C_GUI.P_CallClient._listFormWählbox.Count = 0
    End Sub
#End Region

#Region "Timer"
    Private Sub TimerReStartStandBy_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerReStart.Elapsed
        AnrMonAktiv = False
        If StandbyCounter < C_DP.P_Def_TryMaxRestart Then
            If C_DP.P_CBForceFBAddr Then
                C_hf.httpGET("http://" & C_DP.P_TBFBAdr, C_hf.GetEncoding(C_DP.P_EncodeingFritzBox), AnrMonError)
            Else
                AnrMonError = Not C_hf.Ping(C_DP.P_TBFBAdr)
            End If

            If AnrMonError Then
                C_hf.LogFile(C_DP.P_AnrMon_Log_AnrMonTimer1)
                StandbyCounter += 1
            Else
                C_hf.LogFile(C_DP.P_AnrMon_Log_AnrMonTimer2)
                AnrMonStartStopp()
                If C_DP.P_CBJournal Then
                    Dim formjournalimort As New formJournalimport(Me, C_hf, C_DP, C_XML, False)
                End If
            End If
        Else
            C_hf.LogFile(C_DP.P_AnrMon_Log_AnrMonTimer3)
            TimerReStart = C_hf.KillTimer(TimerReStart)
        End If
    End Sub

    Private Sub TimerCheckAnrMon_Elapsed(sender As Object, e As Timers.ElapsedEventArgs) Handles TimerCheckAnrMon.Elapsed
        ' Es kann sein, dass die Verbindung zur FB abreißt. Z. B. wenn die VPN unterbrochen ist. 

        Dim IPAddresse As IPAddress = IPAddress.Loopback
        Dim RemoteEP As IPEndPoint
        Dim IPHostInfo As IPHostEntry
        Dim CheckAnrMonTCPSocket As Socket

        If Not IPAddress.TryParse(C_DP.P_TBFBAdr, IPAddresse) Then
            ' Versuche über Default-IP zur Fritz!Box zu gelangen
            IPHostInfo = Dns.GetHostEntry(C_DP.P_Def_FritzBoxAdress)
            IPAddresse = IPAddress.Parse(IPHostInfo.AddressList(0).ToString) ' Kann auch IPv6 sein
        End If

        RemoteEP = New IPEndPoint(IPAddresse, C_DP.P_DefaultFBAnrMonPort)
        CheckAnrMonTCPSocket = New Sockets.Socket(IPAddresse.AddressFamily, Sockets.SocketType.Stream, Sockets.ProtocolType.Tcp)

        Try
            CheckAnrMonTCPSocket.Connect(RemoteEP)
        Catch Err As SocketException
            C_hf.LogFile(C_DP.P_AnrMon_Log_AnrMonTimer4)
            AnrMonStartStopp()
            AnrMonError = True
            If TimerReStart IsNot Nothing AndAlso Not TimerReStart.Enabled Then
                StandbyCounter = 1
                TimerReStart = C_hf.SetTimer(C_DP.P_Def_ReStartIntervall)
            End If
        End Try

        CheckAnrMonTCPSocket.Close()
        RemoteEP = Nothing
        IPHostInfo = Nothing

#If OVer < 14 Then
        C_GUI.SetAnrMonButton(True)
#Else
        C_GUI.RefreshRibbon()
#End If
    End Sub
#End Region

#Region "Anrufmonitor Grundlagen"

    Friend Sub AnrMonStartStopp()
        If AnrMonAktiv Then
            ' Timer stoppen, TCP/IP-Verbindung(schließen)
            AnrMonAktiv = False
            If TimerCheckAnrMon IsNot Nothing Then
                With TimerCheckAnrMon
                    .Stop()
                    .Dispose()
                End With
                TimerCheckAnrMon = Nothing
            End If

            If AnrMonStream IsNot Nothing Then
                With AnrMonStream
                    .Close()
                    '.Dispose()
                End With
                AnrMonStream = Nothing
            End If

#If OVer < 14 Then
            C_GUI.SetAnrMonButton(false)
#Else
            C_GUI.RefreshRibbon()
#End If
        Else
            ' TCP/IP-Verbindung öffnen
            If C_DP.P_CBUseAnrMon Then
                'If C_DP.P_CBAnrMonAuto And C_DP.P_CBUseAnrMon Then

                If C_hf.Ping(C_DP.P_TBFBAdr) Or C_DP.P_CBForceFBAddr Or C_DP.P_CBPhonerAnrMon Then
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

    Function AnrMonStartNachStandby() As Boolean
        AnrMonAktiv = False
        AnrMonError = True
#If OVer < 14 Then
        C_GUI.SetAnrMonButton(AnrMonAktiv)
#Else
        C_GUI.RefreshRibbon()
#End If
        AnrMonStartNachStandby = False

        If C_DP.P_CBAnrMonAuto And C_DP.P_CBUseAnrMon And TimerReStart Is Nothing Then
            StandbyCounter = 1
            TimerReStart = C_hf.SetTimer(C_DP.P_Def_ReStartIntervall)
        End If
    End Function

    Friend Sub AnrMonReStart()
        AnrMonStartStopp() ' Ausschalten
        AnrMonStartStopp() ' Einschalten
    End Sub

#End Region

#Region "Anrufmonitor"
    ''' <summary>
    ''' Hauptfunktion des Anrufmonitors. Ruft, je nach eingehenden String, die jeweilige Funktion auf.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AnrMonAktion()
        ' schaut in der FritzBox im Port 1012 nach und startet entsprechende Unterprogramme
        Dim r As New StreamReader(AnrMonStream)
        Dim FBStatus As String  ' Status-String der FritzBox
        Dim aktZeile() As String  ' aktuelle Zeile im Status-String

        Do
            If AnrMonStream.DataAvailable And AnrMonAktiv Then
                FBStatus = r.ReadLine
                Select Case FBStatus
                    Case C_DP.P_AnrMon_AnrMonPhonerWelcome '"Welcome to Phoner"
                        AnrMonPhoner = True
                    Case C_DP.P_AnrMon_AnrMonPhonerError '"Sorry, too many clients"
                        C_hf.LogFile(C_DP.P_AnrMon_Log_AnrMonPhoner1)
                    Case Else
                        C_hf.LogFile("AnrMonAktion: " & FBStatus)
                        aktZeile = Split(FBStatus, ";", , CompareMethod.Text)
                        If Not aktZeile.Length = 1 Then
                            'Schauen ob "RING", "CALL", "CONNECT" oder "DISCONNECT" übermittelt wurde
                            Select Case CStr(aktZeile.GetValue(1))
                                Case DefAnrMon.FB_RING '"RING"
                                    AnrMonRING(aktZeile, True)
                                Case DefAnrMon.FB_CALL '"CALL"
                                    AnrMonCALL(aktZeile, True)
                                Case DefAnrMon.FB_CONNECT '"CONNECT"
                                    AnrMonCONNECT(aktZeile, True)
                                Case DefAnrMon.FB_DISCONNECT '"DISCONNECT"
                                    AnrMonDISCONNECT(aktZeile, True)
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
    ''' Behandelt den vom Anrufmonitor der Fritz!Box erhaltener String für RING
    ''' </summary>
    ''' <param name="FBStatus">String: Vom Anrufmonitor der Fritz!Box erhaltener String für RING</param>
    ''' <param name="ShowForms">Boolean: Soll Anrufmonitor/StoppUhr angezeigt werden. Bei Journalimport nicht, ansonsten ja (unabhängig von der Einstellung des Users)</param>
    Friend Sub AnrMonRING(ByVal FBStatus As String(), ByVal ShowForms As Boolean)
        ' wertet einen eingehenden Anruf aus
        ' Parameter: FBStatus (String ()):   Status-String der FritzBox
        '            anzeigen (Boolean):  nur bei 'true' wird 'AnrMonEinblenden' ausgeführt
        ' FBStatus(0): Uhrzeit
        ' FBStatus(1): RING, wird nicht verwendet
        ' FBStatus(2): Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
        ' FBStatus(3): Eingehende Telefonnummer, TelNr
        ' FBStatus(4): Angerufene eigene Telefonnummer, MSN
        ' FBStatus(5): Anschluss, SIP...

        Dim MSN As String = C_hf.EigeneVorwahlenEntfernen(CStr(FBStatus.GetValue(4)))
        Dim ID As Integer = CInt(FBStatus.GetValue(2))

        Dim Telefonat As C_Telefonat

        ' Prüfe ob Telefonatsliste bereits eine nicht beendetes Telefonat mit gleicher ID enthalten ist
        Telefonat = C_Popup.TelefonatsListe.Find(Function(tmpTel) tmpTel.ID = ID And Not tmpTel.Beendet)
        If Telefonat IsNot Nothing Then
            ' Eigentlich (!) sollte er hier nicht reinlaufen
            C_hf.LogFile(C_DP.P_AnrMon_Log_TelList1("RING", CStr(ID)))
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

        ' Anruf nur anzeigen, wenn die MSN stimmt
        If C_hf.IsOneOf(MSN, C_DP.P_CLBTelNr) Or AnrMonPhoner Then

            Telefonat = New C_Telefonat
            C_Popup.TelefonatsListe.Add(Telefonat)

            With Telefonat
                .Typ = C_Telefonat.AnrufRichtung.Eingehend
                .Zeit = CDate(FBStatus.GetValue(0))
                .MSN = MSN
                .TelName = C_hf.TelefonName(.MSN)
                .ID = ID
                .TelNr = CStr(FBStatus.GetValue(3))
                ' Phoner
                If AnrMonPhoner Then
                    Dim PhonerTelNr() As String
                    Dim pos As Integer = InStr(.TelNr, "@", CompareMethod.Text)
                    If Not pos = 0 Then
                        .TelNr = Left(.TelNr, pos - 1)
                    Else
                        PhonerTelNr = C_hf.TelNrTeile(.TelNr)
                        If Not PhonerTelNr(1) = C_DP.P_Def_StringEmpty Then .TelNr = PhonerTelNr(1) & Mid(.TelNr, InStr(.TelNr, ")", CompareMethod.Text) + 2)
                        If Not PhonerTelNr(0) = C_DP.P_Def_StringEmpty Then .TelNr = PhonerTelNr(0) & Mid(.TelNr, 2)
                    End If
                    .TelNr = C_hf.nurZiffern(.TelNr)
                End If
                ' Ende Phoner

                If Len(.TelNr) = 0 Then .TelNr = C_DP.P_Def_StringUnknown

                ' Daten für Anzeige im Anrurfmonitor speichern

                If ShowForms AndAlso Not C_OlI.VollBildAnwendungAktiv Then
                    LetzterAnrufer = Telefonat
                    C_Popup.AnrMonEinblenden(Telefonat)
                End If

                ' Daten in den Kontakten suchen und per Rückwärtssuche ermitteln
                If Not .TelNr = C_DP.P_Def_StringUnknown Then
                    ' Anrufer in den Outlook-Kontakten suchen
                    .olContact = C_KF.KontaktSuche(.TelNr, C_DP.P_Def_ErrorMinusOne_String, .KontaktID, .StoreID, C_DP.P_CBKHO)
                    If .olContact IsNot Nothing Then
                        .Anrufer = .olContact.FullName
                        .Companies = .olContact.CompanyName
                        If C_DP.P_CBIgnoTelNrFormat Then .TelNr = C_hf.formatTelNr(.TelNr)
                    Else
                        ' Anrufer per Rückwärtssuche ermitteln
                        If C_DP.P_CBRWS AndAlso F_RWS.AnrMonRWS(Telefonat) Then

                            If C_DP.P_CBKErstellen Then
                                ' Im folgenden wird automatisch ein Kontakt erstellt, der durch die Rückwärtssuche ermittlt wurde. 
                                ' Dies geschieht nur, wenn es gewünscht ist.
                                .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, True)
                                .vCard = C_DP.P_Def_StringEmpty
                                .Companies = .olContact.CompanyName
                                .Anrufer = .olContact.FullName 'Replace(.olContact.FullName & " (" & .Companies & ")", " ()", "")
                            Else
                                .Anrufer = ReadFNfromVCard(.vCard)
                                .Anrufer = Replace(.Anrufer, Chr(13), "", , , CompareMethod.Text)
                                If .Anrufer.StartsWith("Firma") Then .Anrufer = Mid(.Anrufer, Len("Firma"))
                                .Anrufer = Trim(.Anrufer)
                            End If

                        End If
                        'Formatiere die Telefonnummer
                        .TelNr = C_hf.formatTelNr(.TelNr)
                    End If
                    ' Hier Anrufmonitor aktualisieren! Nicht beim Journalimport!
                    If Telefonat.PopupAnrMon IsNot Nothing Then
                        C_Popup.UpdateAnrMon(Telefonat)
                    End If


                    LetzterAnrufer = Telefonat
                    SpeichereLetzerAnrufer(Telefonat)
                    C_GUI.UpdateList(C_DP.P_Def_NameListRING, Telefonat)
#If OVer < 14 Then
                    If C_DP.P_CBSymbAnrListe Then C_GUI.FillPopupItems(C_DP.P_Def_NameListRING)
#End If
                End If
                ' Kontakt anzeigen
                If C_DP.P_CBAnrMonZeigeKontakt And ShowForms Then
                    If .olContact Is Nothing Then .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False)

#If Not OVer = 11 Then
                    If C_DP.P_CBNote Then C_KF.AddNote(.olContact)
#End If
                    Try
                        ' Anscheinend wird nach dem Einblenden ein Save ausgeführt, welchses eine Indizierung zur Folge hat.
                        ' Grund für den Save-Forgang ist unbekannt.
                        .olContact.Display()
                    Catch Err As Exception
                        C_hf.LogFile(C_DP.P_AnrMon_Log_AnrMon1("AnrMonRING", Err.Message))
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
    ''' Behandelt den vom Anrufmonitor der Fritz!Box erhaltener String für CALL
    ''' </summary>
    ''' <param name="FBStatus">String: Vom Anrufmonitor der Fritz!Box erhaltener String für CALL</param>
    ''' <param name="ShowForms">Boolean: Soll StoppUhr angezeigt werden. Bei Journalimport nicht, ansonsten ja (unabhängig von der Einstellung des Users)</param>
    ''' <remarks></remarks>
    Friend Sub AnrMonCALL(ByVal FBStatus As String(), ByVal ShowForms As Boolean)
        ' wertet einen ausgehenden Anruf aus
        ' Parameter: FBStatus (String()):  Status-String der FritzBox

        ' FBStatus(0): Uhrzeit
        ' FBStatus(1): CALL, wird nicht verwendet
        ' FBStatus(2): Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
        ' FBStatus(3): Nebenstellennummer, eindeutige Zuordnung des Telefons
        ' FBStatus(4): Ausgehende eigene Telefonnummer, MSN
        ' FBStatus(5): die gewählte Rufnummer

        Dim MSN As String = C_hf.EigeneVorwahlenEntfernen(CStr(FBStatus.GetValue(4)))  ' Ausgehende eigene Telefonnummer, MSN
        Dim ID As Integer = CInt(FBStatus.GetValue(2))
        Dim xPathTeile As New ArrayList
        Dim Telefonat As C_Telefonat

        ' Prüfe ob Telefonatsliste bereits eine nicht beendetes Telefonat mit gleicher ID enthalten ist
        Telefonat = C_Popup.TelefonatsListe.Find(Function(tmpTel) tmpTel.ID = ID And Not tmpTel.Beendet)
        If Telefonat IsNot Nothing Then
            ' Eigentlich (!) sollte er hier nicht reinlaufen
            C_hf.LogFile(C_DP.P_AnrMon_Log_TelList1("CALL", CStr(ID)))
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

        If C_hf.IsOneOf(MSN, C_DP.P_CLBTelNr) Or AnrMonPhoner Then
            Telefonat = New C_Telefonat
            With Telefonat
                .Zeit = CDate(FBStatus.GetValue(0))
                .ID = ID
                .NSN = CLng(FBStatus.GetValue(3))
                .MSN = MSN 'CStr(FBStatus.GetValue(4))
                .Typ = C_Telefonat.AnrufRichtung.Ausgehend

                ' Problem DECT/IP-Telefone: keine MSN  über Anrufmonitor eingegangen. Aus Datei ermitteln.
                If .MSN = C_DP.P_Def_StringEmpty Then
                    Select Case .NSN
                        Case 0 To 2 ' FON1-3
                            .NSN += 1
                        Case 10 To 19 ' DECT
                            .NSN += 50
                    End Select
                    Select Case .NSN
                        Case 3, 4, 5, 36, 37
                            .MSN = C_DP.P_Def_ErrorMinusOne_String
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
                .TelNr = C_hf.nurZiffern(CStr(FBStatus.GetValue(5)))
                If .TelNr = C_DP.P_Def_StringEmpty Then .TelNr = C_DP.P_Def_StringUnknown
                ' CbC-Vorwahl entfernen
                If .TelNr.StartsWith("0100") Then .TelNr = Right(.TelNr, Len(.TelNr) - 6)
                If .TelNr.StartsWith("010") Then .TelNr = Right(.TelNr, Len(.TelNr) - 5)
                If Not .TelNr.StartsWith("0") And Not .TelNr.StartsWith("11") And Not .TelNr.StartsWith("+") Then .TelNr = C_DP.P_TBVorwahl & .TelNr
                ' Raute entfernen
                If Right(.TelNr, 1) = "#" Then .TelNr = Left(.TelNr, Len(.TelNr) - 1)
                ' Daten zurücksetzen

                If Not .TelNr = C_DP.P_Def_StringUnknown Then
                    .olContact = C_KF.KontaktSuche(.TelNr, C_DP.P_Def_ErrorMinusOne_String, .KontaktID, .StoreID, C_DP.P_CBKHO)
                    If Telefonat.olContact IsNot Nothing Then
                        .Anrufer = Replace(.olContact.FullName & " (" & .olContact.CompanyName & ")", " ()", "")
                        If C_DP.P_CBIgnoTelNrFormat Then .TelNr = C_hf.formatTelNr(.TelNr)
                    Else
                        ' .Anrufer per Rückwärtssuche ermitteln
                        If C_DP.P_CBRWS AndAlso F_RWS.AnrMonRWS(Telefonat) Then

                            If C_DP.P_CBKErstellen Then
                                ' Im folgenden wird automatisch ein Kontakt erstellt, der durch die Rückwärtssuche ermittlt wurde. 
                                ' Dies geschieht nur, wenn es gewünscht ist.
                                .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, True)
                                .vCard = C_DP.P_Def_StringEmpty
                                .Companies = .olContact.CompanyName
                                .Anrufer = Replace(.olContact.FullName & " (" & .Companies & ")", " ()", "")
                            Else
                                .Anrufer = ReadFNfromVCard(.vCard)
                                .Anrufer = Replace(.Anrufer, Chr(13), "", , , CompareMethod.Text)
                                If InStr(1, .Anrufer, "Firma", CompareMethod.Text) = 1 Then .Anrufer = Right(.Anrufer, Len(.Anrufer) - 5)
                                .Anrufer = Trim(.Anrufer)
                            End If

                        End If
                        .TelNr = C_hf.formatTelNr(.TelNr)
                    End If
                End If
                ' Daten im Menü für Wahlwiederholung speichern
                C_GUI.UpdateList(C_DP.P_Def_NameListCALL, Telefonat)
#If OVer < 14 Then
                If C_DP.P_CBSymbWwdh Then C_GUI.FillPopupItems(C_DP.P_Def_NameListCALL)
#End If
                ' Kontakt öffnen
                If C_DP.P_CBAnrMonZeigeKontakt And ShowForms Then
                    If .olContact Is Nothing Then
                        .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False)
                    End If
#If Not OVer = 11 Then
                    If C_DP.P_CBNote Then C_KF.AddNote(.olContact)
#End If
                    Try
                        .olContact.Display()
                    Catch Err As Exception
                        C_hf.LogFile(C_DP.P_AnrMon_Log_AnrMon1("AnrMonCALL", Err.Message))
                    End Try
                End If
                'Notizeintag
#If Not OVer = 11 Then
                If C_DP.P_CBNote AndAlso .olContact IsNot Nothing Then
                    C_KF.FillNote(AnrMonEvent.AnrMonCALL, Telefonat, C_DP.P_CBAnrMonZeigeKontakt)
                End If
#End If
            End With
            C_Popup.TelefonatsListe.Add(Telefonat)
        End If
    End Sub '(AnrMonCALL)

    ''' <summary>
    ''' Behandelt den vom Anrufmonitor der Fritz!Box erhaltener String für CONNECT
    ''' </summary>
    ''' <param name="FBStatus">String: Vom Anrufmonitor der Fritz!Box erhaltener String für CONNECT</param>
    ''' <param name="ShowForms">Boolean: Soll StoppUhr angezeigt werden. Bei Journalimport nicht, ansonsten ja (unabhängig von der Einstellung des Users)</param>
    ''' <remarks></remarks>
    Friend Sub AnrMonCONNECT(ByVal FBStatus As String(), ByVal ShowForms As Boolean)
        ' wertet eine Zustande gekommene Verbindung aus
        ' Parameter: FBStatus (String()):  Status-String der FritzBox

        ' FBStatus(0): Uhrzeit
        ' FBStatus(1): CONNECT, wird nicht verwendet
        ' FBStatus(2): Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
        ' FBStatus(3): Nebenstellennummer, eindeutige Zuordnung des Telefons
        ' FBStatus(4): Gewählte Nummer Telefonnummer bzw. eingehende Telefonnummer

        Dim xPathTeile As New ArrayList
        Dim Telefonat As C_Telefonat

        ' 140824:
        ' Achtung: ID Reicht nicht aus.

        Telefonat = C_Popup.TelefonatsListe.Find(Function(tmpTel) tmpTel.ID = CInt(FBStatus.GetValue(2)) And Not tmpTel.Beendet)
        If Telefonat IsNot Nothing Then
            With Telefonat
                ' Temporärer Test ob Nummern identisch
                If Not C_hf.nurZiffern(.TelNr) = CStr(FBStatus.GetValue(4)) Then
                    C_hf.LogFile("AnrMonCONNECT: Verbundene Nummer nicht mit hinterlegter Nummer identisch: " & .TelNr & " <> " & CStr(FBStatus.GetValue(4)))
                End If
                .Angenommen = True
                .Zeit = CDate(FBStatus.GetValue(0))

                If AnrMonPhoner Then
                    '.telname = "Phoner"
                Else
                    .NSN = CInt(FBStatus.GetValue(3))

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
                If C_DP.P_CBStoppUhrEinblenden And ShowForms Then
                    If (Not .NSN = 5) Or (.NSN = 5 And Not C_DP.P_CBStoppUhrIgnIntFax) Then
                        C_hf.LogFile(C_DP.P_AnrMon_Log_AnrMonStoppUhr1)
                        C_Popup.StoppuhrEinblenden(Telefonat)
                    Else
                        C_hf.LogFile(C_DP.P_AnrMon_Log_AnrMonStoppUhr2)
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
    End Sub '(AnrMonCONNECT)

    ''' <summary>
    ''' Behandelt den vom Anrufmonitor der Fritz!Box erhaltener String für DISCONNECT
    ''' </summary>
    ''' <param name="FBStatus">String: Vom Anrufmonitor der Fritz!Box erhaltener String für DISCONNECT</param>
    ''' <param name="ShowForms">Boolean: Soll StoppUhr angezeigt werden. Bei Journalimport nicht, ansonsten ja (unabhängig von der Einstellung des Users)</param>
    ''' <remarks></remarks>
    Friend Sub AnrMonDISCONNECT(ByVal FBStatus As String(), ByVal ShowForms As Boolean)
        ' legt den Journaleintrag (und/oder Kontakt) an
        ' Parameter: FBStatus (String):     Status-String der FritzBox

        ' FBStatus(0): Uhrzeit
        ' FBStatus(1): DISCONNECT, wird nicht verwendet
        ' FBStatus(2): Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
        ' FBStatus(3): Dauer des Telefonates

        Dim CallDirection As String = C_DP.P_Def_StringEmpty
        Dim SchließZeit As Date = C_DP.P_StatOLClosedZeit

        Dim xPathTeile As New ArrayList
        Dim Telefonat As C_Telefonat

        Telefonat = C_Popup.TelefonatsListe.Find(Function(tmpTel) tmpTel.ID = CInt(FBStatus.GetValue(2)) And Not tmpTel.Beendet)

        If Telefonat IsNot Nothing Then
            With Telefonat
                .Beendet = True
                .Dauer = CInt(IIf(CInt(FBStatus.GetValue(3)) <= 30, 31, CInt(FBStatus.GetValue(3)))) \ 60
                .Body = C_DP.P_AnrMon_AnrMonDISCONNECT_JournalBody(.TelNr, .Angenommen)
                If Not .vCard = C_DP.P_Def_StringEmpty And Not .vCard = C_DP.P_Def_ErrorMinusTwo_String Then
                    .Companies = ReadFromVCard(.vCard, "ORG", "")
                    .Body += C_DP.P_AnrMon_AnrMonDISCONNECT_Journal & vbCrLf & .vCard & vbCrLf
                Else
                    If .olContact IsNot Nothing Then
                        If .olContact.FullName = C_DP.P_Def_StringEmpty Then
                            .Anrufer = CStr(IIf(.olContact.Companies = C_DP.P_Def_StringEmpty, .TelNr, .Companies))
                        Else
                            .Anrufer = .olContact.FullName
                        End If

                        If .Companies = C_DP.P_Def_StringEmpty Then
                            If Not .olContact.HomeAddress = C_DP.P_Def_StringEmpty Then
                                .Body += C_DP.P_AnrMon_Journal_Kontaktdaten & vbCrLf & .Anrufer & vbCrLf & .Companies & vbCrLf & .olContact.HomeAddress & vbCrLf
                            End If
                        Else
                            If Not .olContact.BusinessAddress = C_DP.P_Def_StringEmpty Then
                                .Body += C_DP.P_AnrMon_Journal_Kontaktdaten & vbCrLf & .Anrufer & vbCrLf & .Companies & vbCrLf & .olContact.BusinessAddress & vbCrLf
                            End If
                        End If
                    End If
                End If

                If C_DP.P_CBJournal Then

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
                            xPathTeile.Add(CStr(IIf(Telefonat.Typ = C_Telefonat.AnrufRichtung.Eingehend, C_Telefonat.AnrufRichtung.Eingehend.ToString, C_Telefonat.AnrufRichtung.Ausgehend.ToString)))

                            With C_DP
                                C_XML.Write(.XMLDoc, xPathTeile, CStr(CInt(C_XML.Read(.XMLDoc, xPathTeile, CStr(0))) + Telefonat.Dauer * 60))
                            End With
                        End If

                        CallDirection = CStr(IIf(Telefonat.Typ = C_Telefonat.AnrufRichtung.Eingehend, C_DP.P_Def_Journal_Text_Eingehend, C_DP.P_Def_Journal_Text_Ausgehend))
                    Else
                        If .Typ = C_Telefonat.AnrufRichtung.Eingehend Then
                            C_DP.P_StatVerpasst += 1
                            CallDirection = C_DP.P_Def_Journal_Text_Verpasst
                        Else
                            C_DP.P_StatNichtErfolgreich += 1
                            CallDirection = C_DP.P_Def_Journal_Text_NichtErfolgreich
                        End If
                    End If

                    .Categories = .TelName & "; " & String.Join("; ", C_DP.P_AnrMon_Journal_Def_Categories)
                    .Subject = CallDirection & CStr(IIf(.Anrufer = C_DP.P_Def_StringEmpty, .TelNr, .Anrufer & " (" & .TelNr & ")")) & CStr(IIf(Split(.TelName, ";", , CompareMethod.Text).Length = 1, C_DP.P_Def_StringEmpty, " (" & .TelName & ")"))

                    C_OlI.ErstelleJournalEintrag(Telefonat)
                    C_DP.P_StatJournal += 1
                End If

                If .Zeit > SchließZeit Or SchließZeit = System.DateTime.Now Then C_DP.P_StatOLClosedZeit = System.DateTime.Now.AddMinutes(1)

                If C_DP.P_CBStoppUhrEinblenden And ShowForms AndAlso Telefonat.PopupStoppuhr IsNot Nothing Then
                    Telefonat.PopupStoppuhr.StoppuhrStopp()
                End If

                If .PopupAnrMon IsNot Nothing And C_DP.P_CBAnrMonCloseAtDISSCONNECT Then
                    .PopupAnrMon.Hide()
                End If

                If .Typ = C_Telefonat.AnrufRichtung.Eingehend Then
                    LetzterAnrufer = Telefonat
                    SpeichereLetzerAnrufer(Telefonat)
                End If

                'Notizeintag
#If Not OVer = 11 Then
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
            If C_DP.P_CBJournal And C_hf.IsOneOf(CStr(FBStatus.GetValue(3)), C_DP.P_CLBTelNr) Then
                C_hf.LogFile("AnrMonDISCONNECT: " & C_DP.P_AnrMon_AnrMonDISCONNECT_Error)
                ' Wenn Anruf vor dem Outlookstart begonnen wurde, wurde er nicht nachträglich importiert.
                Dim ZeitAnruf As Date = CDate(FBStatus(0))
                Dim DauerAnruf As Integer = CInt(IIf(CInt(FBStatus.GetValue(3)) <= 30, 31, CInt(FBStatus.GetValue(3)))) \ 60
                ZeitAnruf = ZeitAnruf.AddSeconds(-1 * (ZeitAnruf.Second + DauerAnruf + 70))
                If ZeitAnruf < SchließZeit Then C_DP.P_StatOLClosedZeit = ZeitAnruf
                Dim formjournalimort As New formJournalimport(Me, C_hf, C_DP, C_XML, False)
            End If
        End If
    End Sub '(AnrMonDISCONNECT)
#End Region

#Region "LetzterAnrufer"
    ''' <summary>
    ''' Speichert den letzten Anrufer ab, den das Addin registriert hat.
    ''' Dies wird benötigt, damit das Addin nach dem Neustart von Outlook, weiß welchen Anrufer es einblenden soll.
    ''' </summary>
    ''' <param name="Telefonat">Das Telefonat, welches gespeichert werden soll.</param>
    ''' <remarks></remarks>
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
            If Not .Anrufer = C_DP.P_Def_StringEmpty Then
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
            If Not .StoreID = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("StoreID")
                NodeValues.Add(.StoreID)
            End If

            ' KontaktID
            If Not .KontaktID = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("KontaktID")
                NodeValues.Add(.KontaktID)
            End If

            ' vCard
            If Not .vCard = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("vCard")
                NodeValues.Add(.vCard)
            End If

            ' TelName
            If Not .TelName = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("TelName")
                NodeValues.Add(.TelName)
            End If

            ' Companies
            If Not .Companies = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("Companies")
                NodeValues.Add(.Companies)
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
    ''' <remarks></remarks>
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
        ListNodeValues.Add(C_DP.P_Def_StringEmpty)

        ' TelNr
        ListNodeNames.Add("TelNr")
        ListNodeValues.Add(C_DP.P_Def_StringEmpty)

        ' MSN
        ListNodeNames.Add("MSN")
        ListNodeValues.Add(C_DP.P_Def_StringEmpty)

        ' StoreID
        ListNodeNames.Add("StoreID")
        ListNodeValues.Add(C_DP.P_Def_ErrorMinusOne_String)

        ' KontaktID
        ListNodeNames.Add("KontaktID")
        ListNodeValues.Add(C_DP.P_Def_ErrorMinusOne_String)

        ' vCard
        ListNodeNames.Add("vCard")
        ListNodeValues.Add(C_DP.P_Def_StringEmpty)

        ' TelName
        ListNodeNames.Add("TelName")
        ListNodeValues.Add(C_DP.P_Def_StringEmpty)

        ' Companies
        ListNodeNames.Add("Companies")
        ListNodeValues.Add(C_DP.P_Def_StringEmpty)

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
            .Companies = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("Companies")))

            If .TelName = C_DP.P_Def_ErrorMinusOne_String Then .TelName = C_hf.TelefonName(.MSN)

            If C_OlI.OutlookApplication IsNot Nothing Then
                If Not .StoreID = C_DP.P_Def_ErrorMinusOne_String Then
                    .olContact = C_KF.GetOutlookKontakt(.KontaktID, .StoreID)
                ElseIf Not .vCard = C_DP.P_Def_ErrorMinusOne_String Then
                    'prüfen ob das Sinnvoll ist:
                    '.olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False)
                End If
            End If
        End With
        xPathTeile = Nothing
    End Function
#End Region
End Class
