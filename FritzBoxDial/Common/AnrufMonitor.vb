Imports System.Net
Imports System.IO
Imports System.ComponentModel
Imports System.Net.Sockets
Imports System.Threading
Imports System.Collections.Generic

Friend Class AnrufMonitor
#Region "BackgroundWorker"
    Private WithEvents BWAnrMonEinblenden As BackgroundWorker
    Private WithEvents BWStoppuhrEinblenden As BackgroundWorker
    Private WithEvents BWStartTCPReader As BackgroundWorker
    Private WithEvents BWActivateCallmonitor As BackgroundWorker
#End Region

#Region "Timer"

    Private WithEvents TimerReStart As System.Timers.Timer
    Private WithEvents TimerCheckAnrMon As System.Timers.Timer
#End Region

#Region "Eigene Klassen"
    Private C_GUI As GraphicalUserInterface
    Private C_OlI As OutlookInterface
    Private C_KF As Contacts
    Private C_DP As DataProvider
    Private C_hf As Helfer
#End Region

#Region "Eigene Formulare"
    Private F_Config As formCfg
    Private F_RWS As formRWSuche
    Private F_StoppUhr As formStoppUhr
#End Region

#Region "Thread"
    Private ReceiveThread As Thread
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

#Region "Strukturen"
    Structure StructStoppUhr
        Dim Anruf As String
        Dim Abbruch As Boolean
        Dim StartZeit As String
        Dim Richtung As String
        Dim MSN As String
    End Structure

    Enum AnrMonEvent
        AnrMonRING = 0
        AnrMonCALL = 2
        AnrMonCONNECT = 3
        AnrMonDISCONNECT = 4
    End Enum
#End Region

#Region "Globale Variablen"
    Private StoppUhrDaten(5) As StructStoppUhr
    Private TelefonatsListe As New List(Of C_Telefonat)
    Private AnrMonList As New List(Of formAnrMon)

    Private StandbyCounter As Integer
    Private _AnrMonAktiv As Boolean                    ' damit 'AnrMonAktion' nur einmal aktiv ist
    Private _AnrMonError As Boolean
    Private _AnrMonPhoner As Boolean = False
    Private _LetzterAnrufer As C_Telefonat
#End Region

    Public Sub New(ByVal DataProvoderKlasse As DataProvider, _
                   ByVal RWS As formRWSuche, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal KontaktKlasse As Contacts, _
                   ByVal InterfacesKlasse As GraphicalUserInterface, _
                   ByVal OutlInter As OutlookInterface)

        C_DP = DataProvoderKlasse
        C_hf = HelferKlasse
        C_KF = KontaktKlasse
        C_GUI = InterfacesKlasse
        F_RWS = RWS
        C_OlI = OutlInter

        AnrMonStartStopp()
    End Sub

#Region "BackgroundWorker"
    Private Sub BWAnrMonEinblenden_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWAnrMonEinblenden.DoWork
        Dim Telefonat As C_Telefonat = CType(e.Argument, C_Telefonat)
        Telefonat.FormAnrMon = New formAnrMon(True, C_DP, C_hf, Me, C_OlI, C_KF)
        AnrMonList.Add(Telefonat.FormAnrMon)
        Dim a As Integer
        Do
            a = AnrMonList.Count - 1
            For i = 0 To a
                If i < AnrMonList.Count Then
                    If CType(AnrMonList(i), formAnrMon).AnrmonClosed Then
                        AnrMonList.Remove(Telefonat.FormAnrMon)
                        i = 0
                        a = AnrMonList.Count - 1
                    Else
                        C_hf.ThreadSleep(2)
                        Windows.Forms.Application.DoEvents()
                    End If
                End If
            Next
            Windows.Forms.Application.DoEvents()
        Loop Until (AnrMonList.Count = 0)
    End Sub

    Private Sub BWStoppuhrEinblenden_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWStoppuhrEinblenden.DoWork
        Dim ID As Integer = CInt(e.Argument)
        Dim WarteZeit As Integer
        Dim Beendet As Boolean = False
        Dim StartPosition As System.Drawing.Point
        Dim x As Integer = 0
        Dim y As Integer = 0

        If C_DP.P_CBStoppUhrAusblenden Then
            WarteZeit = C_DP.P_TBStoppUhr
        Else
            WarteZeit = -1
        End If

        StartPosition = New System.Drawing.Point(C_DP.P_CBStoppUhrX, C_DP.P_CBStoppUhrY)
        For Each Bildschirm In Windows.Forms.Screen.AllScreens
            x += Bildschirm.Bounds.Size.Width
            y += Bildschirm.Bounds.Size.Height
        Next
        With StartPosition
            If .X > x Or .Y > y Then
                .X = CInt((Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 100) / 2)
                .Y = CInt((Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 50) / 2)
            End If
        End With

        With StoppUhrDaten(ID)
            Dim frmStUhr As New formStoppUhr(.Anruf, .StartZeit, .Richtung, WarteZeit, StartPosition, .MSN)
            C_hf.LogFile("Stoppuhr gestartet - ID: " & ID & ", Anruf: " & .Anruf)
            BWStoppuhrEinblenden.WorkerSupportsCancellation = True
            Do Until frmStUhr.StUhrClosed
                If Not Beendet And .Abbruch Then
                    frmStUhr.Stopp()
                    Beendet = True
                End If
                C_hf.ThreadSleep(20)
                Windows.Forms.Application.DoEvents()
            Loop
            C_DP.P_CBStoppUhrX = frmStUhr.Position.X
            C_DP.P_CBStoppUhrY = frmStUhr.Position.Y
            frmStUhr = Nothing
        End With
    End Sub

    Private Sub BWStartTCPReader_DoWork(sender As Object, e As DoWorkEventArgs) Handles BWStartTCPReader.DoWork
        C_hf.ThreadSleep(500)
        Dim IPAddresse As IPAddress = IPAddress.Loopback
        Dim ReceiveThread As Thread
        Dim RemoteEP As IPEndPoint
        Dim IPHostInfo As IPHostEntry
        Dim FBAnrMonPort As Integer
        Dim AnrMonTCPSocket As Socket


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
                        If C_hf.FBDB_MsgBox("Der Anrufmonitor kann nicht gestartet werden, da die Fritz!Box die Verbindung verweigert." & C_DP.P_Def_NeueZeile & _
                                            "Dies ist meist der Fall, wenn der Fritz!Box Callmonitor deaktiviert ist. Mit dem Telefoncode """ & C_DP.P_Def_TelCodeActivateFritzBoxCallMonitor & _
                                            """ kann dieser aktiviert werden." & C_DP.P_Def_NeueZeile & "Soll versucht werden, den Fritz!Box Callmonitor über die Direktwahl zu aktivieren? (Danach kann der Anrufmonitor manuell aktiviert werden.)" _
                                         , MsgBoxStyle.YesNo, "Soll der Fritz!Box Callmonitor aktiviert werden?") = MsgBoxResult.Yes Then

                            BWActivateCallmonitor = New BackgroundWorker
                            With BWActivateCallmonitor
                                .RunWorkerAsync()
                            End With
                        Else
                            C_hf.LogFile("Das automatische Aktivieren des Fritz!Box Callmonitor wurde übersprungen.")
                        End If
                    End If
                Case Else
                    C_hf.LogFile("TCP Verbindung nicht aufgebaut: " & SocketError.Message)
                    AnrMonError = True
                    e.Result = False
            End Select
        Catch Err As Exception
            C_hf.LogFile("TCP Verbindung nicht aufgebaut: " & Err.Message)

            AnrMonError = True
            e.Result = False
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
            If Not TimerReStart Is Nothing Then
                TimerReStart = C_hf.KillTimer(TimerReStart)
                C_hf.LogFile("Anrufmonitor nach StandBy wiederaufgebaut.")
            End If
        Else
            C_hf.LogFile("BWStartTCPReader_RunWorkerCompleted: Es ist ein TCP/IP Fehler aufgetreten.")
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
                C_hf.httpGET(C_DP.P_TBFBAdr, C_hf.GetEncoding(C_DP.P_EncodeingFritzBox), AnrMonError)
            Else
                AnrMonError = Not C_hf.Ping(C_DP.P_TBFBAdr)
            End If

            If AnrMonError Then
                C_hf.LogFile("Fritz!Box nach StandBy noch nicht verfügbar.")
                StandbyCounter += 1
            Else
                C_hf.LogFile("Fritz!Box nach StandBy wieder verfügbar. Initialisiere Anrufmonitor...")
                AnrMonStartStopp()
                If C_DP.P_CBJournal Then Dim formjournalimort As New formJournalimport(Me, C_hf, C_DP, False)
            End If
        Else
            C_hf.LogFile("Reaktivierung des Anrufmonitors nicht erfolgreich.")
            TimerReStart = C_hf.KillTimer(TimerReStart)
        End If
    End Sub

    Private Sub TimerCheckAnrMon_Elapsed(sender As Object, e As Timers.ElapsedEventArgs) Handles TimerCheckAnrMon.Elapsed
        ' Es kann sein, dass die Verbindung zur FB abreißt. Z. B. wenn die VPN unterbrochen ist. 

        Dim IPAddresse As IPAddress = IPAddress.Loopback
        Dim RemoteEP As IPEndPoint
        Dim IPHostInfo As IPHostEntry
        Dim FBAnrMonPort As Integer
        Dim CheckAnrMonTCPSocket As Socket

        FBAnrMonPort = C_DP.P_DefaultFBAnrMonPort
        If Not IPAddress.TryParse(C_DP.P_TBFBAdr, IPAddresse) Then
            ' Versuche über Default-IP zur Fritz!Box zu gelangen
            IPHostInfo = Dns.GetHostEntry(C_DP.P_Def_FritzBoxAdress)
            IPAddresse = IPAddress.Parse(IPHostInfo.AddressList(0).ToString) ' Kann auch IPv6 sein
        End If

        RemoteEP = New IPEndPoint(IPAddresse, C_DP.P_DefaultFBAnrMonPort)
        CheckAnrMonTCPSocket = New Sockets.Socket(IPAddresse.AddressFamily, Sockets.SocketType.Stream, Sockets.ProtocolType.Tcp)

        Try
            CheckAnrMonTCPSocket.Connect(RemoteEP)
        Catch Err As Exception
            C_hf.LogFile("Die TCP-Verbindung zum Fritz!Box Anrufmonitor wurde verloren.")
            AnrMonStartStopp()
            AnrMonError = True
            If Not TimerReStart Is Nothing AndAlso Not TimerReStart.Enabled Then
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
            If Not TimerCheckAnrMon Is Nothing Then
                With TimerCheckAnrMon
                    .Stop()
                    .Dispose()
                End With
                TimerCheckAnrMon = Nothing
            End If

            If Not AnrMonStream Is Nothing Then
                With AnrMonStream
                    .Close()
                    .Dispose()
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
            If C_DP.P_CBAnrMonAuto And C_DP.P_CBUseAnrMon Then

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
                    Case "Welcome to Phoner"
                        AnrMonPhoner = True
                    Case "Sorry, too many clients"
                        C_hf.LogFile("AnrMonAktion, Phoner: ""Sorry, too many clients""")
                    Case Else
                        C_hf.LogFile("AnrMonAktion: " & FBStatus)
                        aktZeile = Split(FBStatus, ";", , CompareMethod.Text)
                        If Not aktZeile.Length = 1 Then
                            'Schauen ob "RING", "CALL", "CONNECT" oder "DISCONNECT" übermittelt wurde
                            Select Case CStr(aktZeile.GetValue(1))
                                Case "RING"
                                    AnrMonRING(aktZeile, True)
                                Case "CALL"
                                    AnrMonCALL(aktZeile, True)
                                Case "CONNECT"
                                    AnrMonCONNECT(aktZeile, True)
                                Case "DISCONNECT"
                                    AnrMonDISCONNECT(aktZeile, True)
                            End Select
                        End If
                End Select
            End If
            C_hf.ThreadSleep(50)
            Windows.Forms.Application.DoEvents()
        Loop Until Not AnrMonAktiv
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
        ' FBStatus(5): ???

        Dim MSN As String = C_hf.OrtsVorwahlEntfernen(CStr(FBStatus.GetValue(4)))

        Dim Telefonat As C_Telefonat
        Dim xPathTeile As New ArrayList

        ' Anruf nur anzeigen, wenn die MSN stimmt
        If C_hf.IsOneOf(MSN, C_DP.P_CLBTelNr) Or AnrMonPhoner Then

            Telefonat = New C_Telefonat
            With Telefonat
                .Typ = C_Telefonat.AnrufRichtung.Eingehend
                .Zeit = CDate(FBStatus.GetValue(0))
                .MSN = MSN
                .TelName = C_hf.TelefonName(.MSN)
                .ID = CInt(FBStatus.GetValue(2))
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
                    BWAnrMonEinblenden = New BackgroundWorker
                    BWAnrMonEinblenden.RunWorkerAsync(Telefonat)
                End If

                ' Daten in den Kontakten suchen und per Rückwärtssuche ermitteln
                If Not .TelNr = C_DP.P_Def_StringUnknown Then
                    ' Anrufer in den Outlook-Kontakten suchen
                    .olContact = C_KF.KontaktSuche(.TelNr, C_DP.P_Def_ErrorMinusOne_String, .KontaktID, .StoreID, C_DP.P_CBKHO)
                    If Not .olContact Is Nothing Then
                        .Anrufer = .olContact.FullName ' Replace(.olContact.FullName & " (" & .olContact.CompanyName & ")", " ()", "")
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
                                If InStr(1, .Anrufer, "Firma", CompareMethod.Text) = 1 Then .Anrufer = Right(.Anrufer, Len(.Anrufer) - 5)
                                .Anrufer = Trim(.Anrufer)
                            End If

                        End If
                        .TelNr = C_hf.formatTelNr(.TelNr)
                    End If

                    LetzterAnrufer = Telefonat
                    SpeichereLetzerAnrufer(Telefonat)
                    C_GUI.UpdateList(C_DP.P_Def_NameListRING, Telefonat)
#If OVer < 14 Then
                If C_DP.P_CBSymbAnrListe Then C_GUI.FillPopupItems("AnrListe")
#End If
                End If
                'StoppUhr
                If C_DP.P_CBStoppUhrEinblenden And ShowForms Then
                    With StoppUhrDaten(.ID)
                        .Richtung = "Anruf von:"
                        If Telefonat.Anrufer = C_DP.P_Def_StringEmpty Then
                            .Anruf = Telefonat.TelNr
                        Else
                            .Anruf = Telefonat.Anrufer
                        End If
                    End With
                End If

                ' Kontakt anzeigen
                If C_DP.P_CBAnrMonZeigeKontakt And ShowForms Then
                    If .olContact Is Nothing Then
                        .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, False)
                    End If
#If Not OVer = 11 Then
                    If C_DP.P_CBNote Then C_KF.AddNote(.olContact)
#End If
                    Try
                        ' Anscheinend wird nach dem Einblenden ein Save ausgeführt, welchses eine Indizierung zur Folge hat.
                        ' Grund für den Save-Forgang ist unbekannt.
                        .olContact.Display()
                    Catch ex As Exception
                        C_hf.LogFile("AnrMonRING: Kontakt kann nicht angezeigt werden. Grund: " & ex.Message)
                    End Try
                End If

                'Notizeintag
#If Not OVer = 11 Then
                If C_DP.P_CBNote AndAlso Not .olContact Is Nothing Then
                    C_KF.FillNote(AnrMonEvent.AnrMonRING, Telefonat, C_DP.P_CBAnrMonZeigeKontakt)
                End If
#End If
            End With
            TelefonatsListe.Add(Telefonat)
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

        Dim MSN As String = C_hf.OrtsVorwahlEntfernen(CStr(FBStatus.GetValue(4)))  ' Ausgehende eigene Telefonnummer, MSN
        Dim xPathTeile As New ArrayList
        Dim Telefonat As C_Telefonat

        If C_hf.IsOneOf(MSN, C_DP.P_CLBTelNr) Or AnrMonPhoner Then
            Telefonat = New C_Telefonat
            With Telefonat
                .Zeit = CDate(FBStatus.GetValue(0))
                .ID = CInt(FBStatus.GetValue(2))
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
                                Telefonat.MSN = C_DP.Read(xPathTeile, "")
                            End With
                    End Select
                End If
                .TelNr = C_hf.nurZiffern(CStr(FBStatus.GetValue(5)))
                If .TelNr = C_DP.P_Def_StringEmpty Then .TelNr = C_DP.P_Def_StringUnknown
                ' CbC-Vorwahl entfernen
                If Left(.TelNr, 4) = "0100" Then .TelNr = Right(.TelNr, Len(.TelNr) - 6)
                If Left(.TelNr, 3) = "010" Then .TelNr = Right(.TelNr, Len(.TelNr) - 5)
                If Not Left(.TelNr, 1) = "0" And Not Left(.TelNr, 2) = "11" And Not Left(.TelNr, 1) = "+" Then .TelNr = C_DP.P_TBVorwahl & .TelNr
                ' Raute entfernen
                If Right(.TelNr, 1) = "#" Then .TelNr = Left(.TelNr, Len(.TelNr) - 1)
                ' Daten zurücksetzen

                If Not .TelNr = C_DP.P_Def_StringUnknown Then
                    .olContact = C_KF.KontaktSuche(.TelNr, C_DP.P_Def_ErrorMinusOne_String, .KontaktID, .StoreID, C_DP.P_CBKHO)
                    If Not Telefonat.olContact Is Nothing Then
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
            If C_DP.P_CBSymbWwdh Then C_GUI.FillPopupItems("Wwdh")
#End If
                'StoppUhr
                If C_DP.P_CBStoppUhrEinblenden And ShowForms Then
                    With StoppUhrDaten(.ID)
                        .Richtung = "Anruf zu:"
                        If Telefonat.Anrufer = C_DP.P_Def_StringEmpty Then
                            .Anruf = Telefonat.TelNr
                        Else
                            .Anruf = Telefonat.Anrufer
                        End If
                    End With
                End If
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
                    Catch ex As Exception
                        C_hf.LogFile("AnrMonCALL: Kontakt kann nicht angezeigt werden. Grund: " & ex.Message)
                    End Try
                End If
                'Notizeintag
#If Not OVer = 11 Then
                If C_DP.P_CBNote AndAlso Not .olContact Is Nothing Then
                    C_KF.FillNote(AnrMonEvent.AnrMonCALL, Telefonat, C_DP.P_CBAnrMonZeigeKontakt)
                End If
#End If
            End With
            TelefonatsListe.Add(Telefonat)
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
        ' FBStatus(3): 

        Dim xPathTeile As New ArrayList
        Dim Telefonat As C_Telefonat

        Telefonat = TelefonatsListe.Find(Function(JE) JE.ID = CInt(FBStatus.GetValue(2)))
        If Not Telefonat Is Nothing Then
            With Telefonat
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
                            .TelName = C_DP.Read(xPathTeile, "")
                    End Select

                End If
                If .Typ = C_Telefonat.AnrufRichtung.Eingehend Then
                    LetzterAnrufer = Telefonat
                    SpeichereLetzerAnrufer(Telefonat)
                End If

                ' StoppUhr einblenden
                If C_DP.P_CBStoppUhrEinblenden And ShowForms Then
                    C_hf.LogFile("StoppUhr wird eingeblendet.")
                    With StoppUhrDaten(.ID)
                        .MSN = CStr(IIf(Telefonat.TelName = C_DP.P_Def_StringEmpty, Telefonat.MSN, Telefonat.TelName))
                        .StartZeit = String.Format("{0:00}:{1:00}:{2:00}", System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second)
                        .Abbruch = False
                    End With
                    BWStoppuhrEinblenden = New BackgroundWorker
                    With BWStoppuhrEinblenden
                        .WorkerSupportsCancellation = True
                        .RunWorkerAsync(Telefonat.ID)
                    End With
                End If
                'Notizeintag
#If Not OVer = 11 Then
                If C_DP.P_CBNote Then
                    If Not .olContact Is Nothing Then
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
        Dim NSN As Long = -1
        Dim SchließZeit As Date = C_DP.P_StatOLClosedZeit

        Dim xPathTeile As New ArrayList
        Dim Telefonat As C_Telefonat

        Telefonat = TelefonatsListe.Find(Function(JE) JE.ID = CInt(FBStatus.GetValue(2)))

        If Not Telefonat Is Nothing Then
            With Telefonat
                NSN = .NSN
                .Dauer = CInt(IIf(CInt(FBStatus.GetValue(3)) <= 30, 31, CInt(FBStatus.GetValue(3)))) \ 60

                .Body = "Tel.-Nr.: " & .TelNr & vbCrLf & "Status: " & CStr(IIf(.Angenommen, C_DP.P_Def_StringEmpty, "nicht ")) & "angenommen" & vbCrLf & vbCrLf
                If Not .vCard = C_DP.P_Def_StringEmpty Then

                    .Companies = ReadFromVCard(.vCard, "ORG", "")
                    .Body += "Kontaktdaten (vCard):" & vbCrLf & .vCard & vbCrLf

                Else
                    If Not .olContact Is Nothing Then

                        If .olContact.FullName = C_DP.P_Def_StringEmpty Then
                            .Anrufer = CStr(IIf(.olContact.Companies = C_DP.P_Def_StringEmpty, .TelNr, .Companies))
                        Else
                            .Anrufer = .olContact.FullName
                        End If

                        If .Companies = C_DP.P_Def_StringEmpty Then
                            If Not .olContact.HomeAddress = C_DP.P_Def_StringEmpty Then
                                .Body += "Kontaktdaten:" & vbCrLf & .Anrufer & vbCrLf & .Companies & vbCrLf & .olContact.HomeAddress & vbCrLf
                            End If
                        Else
                            If Not .olContact.BusinessAddress = C_DP.P_Def_StringEmpty Then
                                .Body += "Kontaktdaten:" & vbCrLf & .Anrufer & vbCrLf & .Companies & vbCrLf & .olContact.BusinessAddress & vbCrLf
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
                            .Add("Telefon")
                            .Add("[TelName = """ & Telefonat.TelName & """]")
                            C_DP.GetProperXPath(xPathTeile)

                            If Telefonat.Typ = C_Telefonat.AnrufRichtung.Eingehend Then
                                .Add(C_Telefonat.AnrufRichtung.Eingehend.ToString)
                                CallDirection = C_DP.P_Def_Journal_Text_Eingehend
                            Else
                                .Add(C_Telefonat.AnrufRichtung.Ausgehend.ToString)
                                CallDirection = C_DP.P_Def_Journal_Text_Ausgehend
                            End If
                        End With

                        With C_DP
                            .Write(xPathTeile, CStr(CInt(.Read(xPathTeile, CStr(0))) + Telefonat.Dauer * 60))
                        End With
                    Else
                        If .Typ = C_Telefonat.AnrufRichtung.Eingehend Then
                            C_DP.P_StatVerpasst += 1
                            CallDirection = C_DP.P_Def_Journal_Text_Verpasst
                        Else
                            C_DP.P_StatNichtErfolgreich += 1
                            CallDirection = C_DP.P_Def_Journal_Text_NichtErfolgreich
                        End If
                    End If

                    .Categories = .TelName & "; FritzBox Anrufmonitor; Telefonanrufe"
                    .Subject = CallDirection & " " & .Anrufer & CStr(IIf(.Anrufer = .TelNr, C_DP.P_Def_StringEmpty, " (" & .TelNr & ")")) & CStr(IIf(Split(.TelName, ";", , CompareMethod.Text).Length = 1, C_DP.P_Def_StringEmpty, " (" & .TelName & ")"))

                    C_OlI.ErstelleJournalEintrag(Telefonat)
                    C_DP.P_StatJournal += 1
                End If

                If .Zeit > SchließZeit Or SchließZeit = System.DateTime.Now Then C_DP.P_StatOLClosedZeit = System.DateTime.Now.AddMinutes(1)

                If C_DP.P_CBStoppUhrEinblenden And ShowForms Then StoppUhrDaten(.ID).Abbruch = True

                If .Typ = C_Telefonat.AnrufRichtung.Eingehend Then
                    LetzterAnrufer = Telefonat
                    SpeichereLetzerAnrufer(Telefonat)
                End If

                'Notizeintag
#If Not OVer = 11 Then
                If C_DP.P_CBNote Then
                    If Not .olContact Is Nothing Then
                        C_KF.FillNote(AnrMonEvent.AnrMonDISCONNECT, Telefonat, C_DP.P_CBAnrMonZeigeKontakt)
                    End If
                End If
#End If
            End With
            TelefonatsListe.Remove(Telefonat)
        Else
            If C_DP.P_CBJournal And C_hf.IsOneOf(CStr(FBStatus.GetValue(3)), C_DP.P_CLBTelNr) Then
                C_hf.LogFile("AnrMonDISCONNECT: Ein unvollständiges Telefonat wurde registriert.")
                ' Wenn Anruf vor dem Outlookstart begonnen wurde, wurde er nicht nachträglich importiert.
                Dim ZeitAnruf As Date = CDate(FBStatus(0))
                Dim DauerAnruf As Integer = CInt(IIf(CInt(FBStatus.GetValue(3)) <= 30, 31, CInt(FBStatus.GetValue(3)))) \ 60
                ZeitAnruf = ZeitAnruf.AddSeconds(-1 * (ZeitAnruf.Second + DauerAnruf + 70))
                If ZeitAnruf < SchließZeit Then C_DP.P_StatOLClosedZeit = ZeitAnruf
                C_hf.LogFile("AnrMonDISCONNECT: Journalimport wird gestartet")
                Dim formjournalimort As New formJournalimport(Me, C_hf, C_DP, False)
            End If
        End If
    End Sub '(AnrMonDISCONNECT)
#End Region

#Region "LetzterAnrufer"
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
            .AppendNode(xPathTeile, .CreateXMLNode("Eintrag", NodeNames, NodeValues, AttributeNames, AttributeValues))
        End With

        xPathTeile = Nothing
        NodeNames = Nothing
        NodeValues = Nothing
        AttributeNames = Nothing
        AttributeValues = Nothing
    End Sub

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
        C_DP.ReadXMLNode(xPathTeile, ListNodeNames, ListNodeValues, "ID", CStr(LadeLetzterAnrufer.ID))
        With LadeLetzterAnrufer

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
            If Not C_OlI.OutlookApplication Is Nothing Then
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
