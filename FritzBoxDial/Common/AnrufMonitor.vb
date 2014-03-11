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

#Region "ArrayList"
    Private AnrMonList As New ArrayList
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
    Public Property AnrMonPhoner() As Boolean
        Get
            Return _AnrMonPhoner
        End Get
        Set(ByVal value As Boolean)
            _AnrMonPhoner = value
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

    Private StandbyCounter As Integer
    Private _AnrMonAktiv As Boolean                    ' damit 'AnrMonAktion' nur einmal aktiv ist
    Private _AnrMonError As Boolean
    Private _AnrMonPhoner As Boolean = False
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

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

#Region "BackgroundWorker"
    Private Sub BWAnrMonEinblenden_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWAnrMonEinblenden.DoWork
        Dim Telefonat As C_Telefonat = CType(e.Argument, C_Telefonat)

        'Dim ID As Integer = CInt(e.Argument)
        AnrMonList.Add(New formAnrMon(Telefonat, True, C_DP, C_hf, Me, C_OlI, C_KF))
        Dim a As Integer
        Do
            a = AnrMonList.Count - 1
            For i = 0 To a
                If i < AnrMonList.Count Then
                    If CType(AnrMonList(i), formAnrMon).AnrmonClosed Then
                        AnrMonList.RemoveAt(i)
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
            If Not TimerReStart.Enabled Then
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
        AnrMonStartStopp()
        AnrMonStartStopp()
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
    ''' <param name="AnrMonAnzeigen">Boolean: Soll Anrufmonitor angezeigt werden. Bei Journalimport nicht, ansonsten ja (unabhängig von der Einstellung des Users)</param>
    ''' <param name="StoppUhrAnzeigen">Boolean: Soll StoppUhr angezeigt werden. Bei Journalimport nicht, ansonsten ja (unabhängig von der Einstellung des Users)</param>
    ''' <remarks></remarks>
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

        Dim RWSSuccess As Boolean = False    ' 'true' wenn die Rückwärtssuche erfolgreich war
        Dim LetzterAnrufer(5) As String
        Dim RWSIndex As Boolean
        Dim FullName As String = C_DP.P_Def_StringEmpty
        Dim CompanyName As String = C_DP.P_Def_StringEmpty
        Dim MSN As String = C_hf.OrtsVorwahlEntfernen(CStr(FBStatus.GetValue(4)), C_DP.P_TBVorwahl)

        Dim Telefonat As C_Telefonat
        Dim xPathTeile As New ArrayList

        With xPathTeile
            .Add("Telefone")
            .Add("Nummern")
            .Add("*")
            .Add("[. = """ & C_hf.OrtsVorwahlEntfernen(MSN, C_DP.P_TBVorwahl) & """]")
            .Add("@Checked")
        End With
        ' Anruf nur anzeigen, wenn die MSN stimmt
        If C_hf.IsOneOf("1", Split(C_DP.Read(xPathTeile, "0;") & ";", ";", , CompareMethod.Text)) Or AnrMonPhoner Then

            Telefonat = New C_Telefonat
            With Telefonat
                .Typ = C_Telefonat.JournalTyp.Eingehend
                .Zeit = CDate(FBStatus.GetValue(0))
                .MSN = MSN

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
                    .TelNr = C_hf.nurZiffern(.TelNr, C_DP.P_TBLandesVW)
                End If
                ' Ende Phoner

                If Len(.TelNr) = 0 Then .TelNr = C_DP.P_Def_StringUnknown
                LetzterAnrufer(0) = CStr(FBStatus.GetValue(0)) 'Zeit
                LetzterAnrufer(1) = .Anrufer
                LetzterAnrufer(2) = .TelNr
                LetzterAnrufer(3) = .MSN
                SpeichereLetzerAnrufer(CStr(.ID), LetzterAnrufer)
                ' Daten für Anzeige im Anrurfmonitor speichern
                If ShowForms And Not C_OlI.VollBildAnwendungAktiv Then
                    BWAnrMonEinblenden = New BackgroundWorker
                    BWAnrMonEinblenden.RunWorkerAsync(Telefonat)
                End If

                ' Daten in den Kontakten suchen und per Rückwärtssuche ermitteln
                If Not .TelNr = C_DP.P_Def_StringUnknown Then
                    ' Anrufer in den Outlook-Kontakten suchen
                    .olContact = C_KF.KontaktSuche(.KontaktID, .StoreID, C_DP.P_CBKHO, .TelNr, "", C_DP.P_TBLandesVW)
                    If Not Telefonat.olContact Is Nothing Then
                        'If Not C_KF.KontaktSuche(KontaktID, StoreID, C_DP.P_CBKHO, TelNr, "", C_DP.P_TBLandesVW) Is Nothing Then
                        C_KF.KontaktInformation(Telefonat.olContact, FullName:=FullName, CompanyName:=CompanyName)
                        .Anrufer = Replace(FullName & " (" & CompanyName & ")", " ()", "")
                        If C_DP.P_CBIgnoTelNrFormat Then .TelNr = C_hf.formatTelNr(.TelNr)
                    Else
                        ' Anrufer per Rückwärtssuche ermitteln
                        If C_DP.P_CBRWS Then
                            .vCard = C_DP.P_Def_ErrorMinusOne
                            If RWSIndex Then
                                With xPathTeile
                                    .Clear()
                                    .Add("CBRWSIndex")
                                    .Add("Eintrag[@ID=""" & Telefonat.TelNr & """]")
                                End With
                                .vCard = C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne)
                            End If
                            If .vCard = C_DP.P_Def_ErrorMinusOne Then
                                Select Case C_DP.P_ComboBoxRWS
                                    Case 0
                                        RWSSuccess = F_RWS.RWS11880(.TelNr, .vCard)
                                    Case 1
                                        RWSSuccess = F_RWS.RWSDasTelefonbuch(.TelNr, .vCard)
                                    Case 2
                                        RWSSuccess = F_RWS.RWStelsearch(.TelNr, .vCard)
                                    Case 3
                                        RWSSuccess = F_RWS.RWSAlle(.TelNr, .vCard)
                                End Select
                                ' Im folgenden wird automatisch ein Kontakt erstellt, der durch die Rückwärtssuche ermittlt wurde. 
                                ' Dies geschieht nur, wenn es gewünscht ist.
                                If RWSSuccess And C_DP.P_CBKErstellen Then
                                    With Telefonat
                                        .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, True)
                                        .vCard = C_DP.P_Def_StringEmpty
                                        C_KF.KontaktInformation(.olContact, FullName:=FullName, CompanyName:=CompanyName)
                                    End With
                                    .Anrufer = Replace(FullName & " (" & CompanyName & ")", " ()", "")
                                    RWSSuccess = False
                                End If
                            Else
                                RWSSuccess = Not .vCard = C_DP.P_Def_ErrorMinusTwo
                            End If

                            If RWSSuccess Then
                                .Anrufer = ReadFNfromVCard(.vCard)
                                .Anrufer = Replace(.Anrufer, Chr(13), "", , , CompareMethod.Text)
                                If InStr(1, .Anrufer, "Firma", CompareMethod.Text) = 1 Then .Anrufer = Right(.Anrufer, Len(.Anrufer) - 5)
                                .Anrufer = Trim(.Anrufer)
                                'KontaktID = C_DP.P_Def_ErrorMinusOne & Anrufer & ";" & vCard
                            End If

                            If RWSIndex Then
                                xPathTeile.Item(xPathTeile.Count - 1) = "Eintrag"
                                C_DP.Write(xPathTeile, .vCard, "ID", .TelNr)
                            End If
                        End If
                        .TelNr = C_hf.formatTelNr(.TelNr)
                    End If

                    LetzterAnrufer(1) = .Anrufer
                    LetzterAnrufer(2) = .TelNr
                    LetzterAnrufer(4) = .StoreID
                    LetzterAnrufer(5) = .KontaktID 'FEHLER, wen vcard enthalten
                    SpeichereLetzerAnrufer(CStr(.ID), LetzterAnrufer)
                    UpdateList("RingList", .Anrufer, .TelNr, FBStatus(0), .StoreID, .KontaktID)
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
                    If C_DP.P_CBNote Then
                        If .olContact Is Nothing Then
                            .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .TelNr, False)
                        End If
                        C_KF.AddNote(.olContact)
                    End If
                    .olContact.Display()
                End If
                'Notizeintag
                If C_DP.P_CBNote Then
                    If Not .olContact Is Nothing Then
                        C_KF.FillNote(AnrMonEvent.AnrMonRING, Telefonat, C_DP.P_CBAnrMonZeigeKontakt)
                    End If
                End If
            End With
            TelefonatsListe.Add(Telefonat)
        End If
    End Sub '(AnrMonRING)

    ''' <summary>
    ''' Behandelt den vom Anrufmonitor der Fritz!Box erhaltener String für CALL
    ''' </summary>
    ''' <param name="FBStatus">String: Vom Anrufmonitor der Fritz!Box erhaltener String für CALL</param>
    ''' <param name="StoppUhrAnzeigen">Boolean: Soll StoppUhr angezeigt werden. Bei Journalimport nicht, ansonsten ja (unabhängig von der Einstellung des Users)</param>
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

        Dim MSN As String = C_hf.OrtsVorwahlEntfernen(CStr(FBStatus.GetValue(4)), C_DP.P_TBVorwahl)  ' Ausgehende eigene Telefonnummer, MSN
        Dim RWSSuccess As Boolean                              ' 'true' wenn die Rückwärtssuche erfolgreich war
        Dim RWSIndex As Boolean
        Dim xPathTeile As New ArrayList
        Dim Telefonat As C_Telefonat
        Dim FullName As String = C_DP.P_Def_StringEmpty

        With xPathTeile
            .Clear()
            .Add("Telefone")
            .Add("Nummern")
            .Add("*")
            .Add("[. = """ & Replace(MSN, ";", """ or . = """, , , CompareMethod.Text) & """]")
            .Add("@Checked")
        End With

        If C_hf.IsOneOf("1", Split(C_DP.Read(xPathTeile, "0;") & ";", ";", , CompareMethod.Text)) Or AnrMonPhoner Then
            Telefonat = New C_Telefonat
            With Telefonat
                .Zeit = CDate(FBStatus.GetValue(0))
                .ID = CInt(FBStatus.GetValue(2))
                .NSN = CLng(FBStatus.GetValue(3))
                .MSN = CStr(FBStatus.GetValue(4))
                .Typ = C_Telefonat.JournalTyp.Ausgehend

                ' Problem DECT/IP-Telefone: keine MSN  über Anrufmonitor eingegangen. Aus Datei ermitteln.
                If MSN = C_DP.P_Def_StringEmpty Then
                    Select Case .NSN
                        Case 0 To 2 ' FON1-3
                            .NSN += 1
                        Case 10 To 19 ' DECT
                            .NSN += 50
                    End Select
                    Select Case .NSN
                        Case 3, 4, 5, 36, 37
                            ' Diese komischen Dinger werden ignoriert:
                            ' 3=Durchwahl
                            ' 4=ISDN Gerät
                            ' 5=Fax (intern/PC)
                            ' 36=Data S0
                            ' 37=Data PC
                            MSN = C_DP.P_Def_ErrorMinusOne
                        Case Else
                            With xPathTeile
                                .Add("Telefone")
                                .Add("Telefone")
                                .Add("*")
                                .Add("Telefon[@Dialport = """ & Telefonat.NSN & """]")
                                .Add("TelNr")
                            End With
                            MSN = C_DP.Read(xPathTeile, "")
                    End Select
                End If

                .TelNr = C_hf.nurZiffern(CStr(FBStatus.GetValue(5)), C_DP.P_TBLandesVW)
                If .TelNr = C_DP.P_Def_StringEmpty Then .TelNr = C_DP.P_Def_StringUnknown
                ' CbC-Vorwahl entfernen
                If Left(.TelNr, 4) = "0100" Then .TelNr = Right(.TelNr, Len(.TelNr) - 6)
                If Left(.TelNr, 3) = "010" Then .TelNr = Right(.TelNr, Len(.TelNr) - 5)
                If Not Left(.TelNr, 1) = "0" And Not Left(.TelNr, 2) = "11" And Not Left(.TelNr, 1) = "+" Then .TelNr = C_DP.P_TBVorwahl & .TelNr
                ' Raute entfernen
                If Right(.TelNr, 1) = "#" Then .TelNr = Left(.TelNr, Len(.TelNr) - 1)
                ' Daten zurücksetzen

                If Not .TelNr = C_DP.P_Def_StringUnknown Then
                    .olContact = C_KF.KontaktSuche(.KontaktID, .StoreID, C_DP.P_CBKHO, .TelNr, "", C_DP.P_TBLandesVW)
                    If Not Telefonat.olContact Is Nothing Then
                        C_KF.KontaktInformation(.olContact, FullName:=FullName, CompanyName:=.Companies)
                        .Anrufer = Replace(FullName & " (" & .Companies & ")", " ()", "")
                        If C_DP.P_CBIgnoTelNrFormat Then .TelNr = C_hf.formatTelNr(.TelNr)
                    Else
                        ' .Anrufer per Rückwärtssuche ermitteln
                        If C_DP.P_CBRWS Then
                            .vCard = C_DP.P_Def_ErrorMinusOne
                            RWSIndex = C_DP.P_CBRWSIndex
                            If RWSIndex Then
                                With xPathTeile
                                    .Clear()
                                    .Add("CBRWSIndex")
                                    .Add("Eintrag[@ID=""" & Telefonat.TelNr & """]")
                                End With
                                .vCard = C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne)
                            End If
                            If .vCard = C_DP.P_Def_ErrorMinusOne Then
                                Select Case C_DP.P_ComboBoxRWS
                                    Case 0
                                        RWSSuccess = F_RWS.RWS11880(.TelNr, .vCard)
                                    Case 1
                                        RWSSuccess = F_RWS.RWSDasTelefonbuch(.TelNr, .vCard)
                                    Case 2
                                        RWSSuccess = F_RWS.RWStelsearch(.TelNr, .vCard)
                                    Case 3
                                        RWSSuccess = F_RWS.RWSAlle(.TelNr, .vCard)
                                End Select
                                ' Im folgenden wird automatisch ein Kontakt erstellt, der durch die Rückwärtssuche ermittlt wurde. 
                                ' Dies geschieht nur, wenn es gewünscht ist.
                                If RWSSuccess And C_DP.P_CBKErstellen Then
                                    With Telefonat
                                        .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .vCard, .TelNr, True)
                                        .vCard = C_DP.P_Def_StringEmpty
                                        C_KF.KontaktInformation(.olContact, FullName:=FullName, CompanyName:=.Companies)
                                    End With
                                    .Anrufer = Replace(FullName & " (" & .Companies & ")", " ()", "")
                                    RWSSuccess = False
                                End If
                            Else
                                RWSSuccess = Not .vCard = C_DP.P_Def_ErrorMinusTwo
                            End If

                            If RWSSuccess Then
                                .Anrufer = ReadFNfromVCard(.vCard)
                                .Anrufer = Replace(.Anrufer, Chr(13), "", , , CompareMethod.Text)
                                If InStr(1, .Anrufer, "Firma", CompareMethod.Text) = 1 Then
                                    .Anrufer = Right(.Anrufer, Len(.Anrufer) - 5)
                                End If
                                .Anrufer = Trim(.Anrufer)

                                'KontaktID = C_DP.P_Def_ErrorMinusOne & .Anrufer & ";" & .vCard
                            Else
                                .vCard = C_DP.P_Def_ErrorMinusTwo
                            End If
                            If RWSIndex Then
                                xPathTeile.Item(xPathTeile.Count - 1) = "Eintrag"
                                C_DP.Write(xPathTeile, .vCard, "ID", .TelNr)
                            End If
                        End If
                        .TelNr = C_hf.formatTelNr(.TelNr)
                    End If
                End If
                ' Daten im Menü für Wahlwiederholung speichern
                UpdateList("CallList", .Anrufer, .TelNr, FBStatus(0), .StoreID, .KontaktID)      ' Hier geht es schief
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
                    If C_DP.P_CBNote Then
                        If .olContact Is Nothing Then
                            .olContact = C_KF.ErstelleKontakt(.KontaktID, .StoreID, .TelNr, False)
                        End If
                        C_KF.AddNote(.olContact)
                    End If
                    .olContact.Display()
                End If
                'Notizeintag
                If C_DP.P_CBNote Then
                    If Not .olContact Is Nothing Then
                        C_KF.FillNote(AnrMonEvent.AnrMonCALL, Telefonat, C_DP.P_CBAnrMonZeigeKontakt)
                    End If
                End If
            End With
            TelefonatsListe.Add(Telefonat)
        End If
    End Sub '(AnrMonCALL)

    ''' <summary>
    ''' Behandelt den vom Anrufmonitor der Fritz!Box erhaltener String für CONNECT
    ''' </summary>
    ''' <param name="FBStatus">String: Vom Anrufmonitor der Fritz!Box erhaltener String für CONNECT</param>
    ''' <param name="StoppUhrAnzeigen">Boolean: Soll StoppUhr angezeigt werden. Bei Journalimport nicht, ansonsten ja (unabhängig von der Einstellung des Users)</param>
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

                .Zeit = CDate(FBStatus.GetValue(0))
                .NSN = CInt(FBStatus.GetValue(3))

                If C_DP.P_CBJournal Or (C_DP.P_CBStoppUhrEinblenden And ShowForms) Then
                    If .MSN = C_DP.P_Def_ErrorMinusOne Then
                        ' Wenn Journal nicht erstellt wird, muss MSN anderweitig ermittelt werden.
                        Select Case .NSN
                            Case 0 To 2 ' FON1-3
                                .NSN += 1
                            Case 10 To 19 ' DECT
                                .NSN += 50
                        End Select
                        Select Case .NSN
                            Case 3, 4, 5, 36, 37
                                ' Diese komischen Dinger werden ignoriert:
                                ' 3=Durchwahl
                                ' 4=ISDN Gerät
                                ' 5=Fax (intern/PC)
                                ' 36=Data S0
                                ' 37=Data PC
                                .MSN = C_DP.P_Def_ErrorMinusOne
                            Case Else
                                With xPathTeile
                                    .Clear()
                                    .Add("Telefone")
                                    .Add("Telefone")
                                    .Add("*")
                                    .Add("Telefon[@Dialport = """ & Telefonat.NSN & """]")
                                    .Add("TelNr")
                                    Telefonat.MSN = C_DP.Read(xPathTeile, "")
                                    .Item(.Count - 1) = "TelName"
                                    Telefonat.TelName = C_DP.Read(xPathTeile, "")
                                End With
                        End Select
                    End If

                    With xPathTeile
                        .Clear()
                        .Add("Telefone")
                        .Add("Nummern")
                        .Add("*")
                        .Add("[. = """ & Replace(Telefonat.MSN, ";", """ or . = """, , , CompareMethod.Text) & """]")
                        .Add("@Checked")
                    End With

                    If C_hf.IsOneOf("1", Split(C_DP.Read(xPathTeile, "0;") & ";", ";", , CompareMethod.Text)) Or AnrMonPhoner Then
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
                    End If
                    'End If
                End If
                'Notizeintag
                If C_DP.P_CBNote Then
                    If Not .olContact Is Nothing Then
                        C_KF.FillNote(AnrMonEvent.AnrMonCONNECT, Telefonat, C_DP.P_CBAnrMonZeigeKontakt)
                    End If
                End If
            End With
        End If
    End Sub '(AnrMonCONNECT)

    ''' <summary>
    ''' Behandelt den vom Anrufmonitor der Fritz!Box erhaltener String für DISCONNECT
    ''' </summary>
    ''' <param name="FBStatus">String: Vom Anrufmonitor der Fritz!Box erhaltener String für DISCONNECT</param>
    ''' <param name="StoppUhrAnzeigen">Boolean: Soll StoppUhr angezeigt werden. Bei Journalimport nicht, ansonsten ja (unabhängig von der Einstellung des Users)</param>
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

        Dim FullName As String = C_DP.P_Def_StringEmpty
        Dim HomeAddress As String = C_DP.P_Def_StringEmpty
        Dim BusinessAddress As String = C_DP.P_Def_StringEmpty

        Dim xPathTeile As New ArrayList
        Dim Telefonat As C_Telefonat

        Telefonat = TelefonatsListe.Find(Function(JE) JE.ID = CInt(FBStatus.GetValue(2)))

        If Not Telefonat Is Nothing Then
            With Telefonat
                NSN = .NSN
                .Dauer = CInt(IIf(CInt(FBStatus.GetValue(3)) <= 30, 31, CInt(FBStatus.GetValue(3)))) \ 60

                If Not .MSN = Nothing Then
                    With xPathTeile
                        .Add("Telefone")
                        .Add("Nummern")
                        .Add("*")
                        .Add("[. = """ & C_hf.OrtsVorwahlEntfernen(Telefonat.MSN, C_DP.P_TBVorwahl) & """]")
                        .Add("@Checked")
                    End With

                    If C_hf.IsOneOf("1", Split(C_DP.Read(xPathTeile, "0;") & ";", ";", , CompareMethod.Text)) Or AnrMonPhoner Then

                        .Body = "Tel.-Nr.: " & .TelNr & vbCrLf & "Status: " & CStr(IIf(.Dauer = 0, "nicht ", C_DP.P_Def_StringEmpty)) & "angenommen" & vbCrLf & vbCrLf
                        If Not .vCard = C_DP.P_Def_StringEmpty Then

                            .Companies = ReadFromVCard(.vCard, "ORG", "")
                            .Body += "Kontaktdaten (vCard):" & vbCrLf & .vCard & vbCrLf
                        Else
                            If Not .olContact Is Nothing Then
                                C_KF.KontaktInformation(.olContact, FullName:=FullName, CompanyName:=.Companies, BusinessAddress:=BusinessAddress, HomeAddress:=HomeAddress)

                                If FullName = C_DP.P_Def_StringEmpty Then
                                    If .Companies = C_DP.P_Def_StringEmpty Then
                                        .Anrufer = .TelNr
                                    Else
                                        .Anrufer = .Companies
                                    End If
                                Else
                                    .Anrufer = FullName
                                End If
                                If .Companies = C_DP.P_Def_StringEmpty Then
                                    If Not HomeAddress = C_DP.P_Def_StringEmpty Then
                                        .Body += "Kontaktdaten:" & vbCrLf & .Anrufer _
                                            & vbCrLf & .Companies & vbCrLf & HomeAddress & vbCrLf
                                    End If
                                Else
                                    If Not BusinessAddress = C_DP.P_Def_StringEmpty Then
                                        .Body += "Kontaktdaten:" & vbCrLf & .Anrufer _
                                            & vbCrLf & .Companies & vbCrLf & BusinessAddress & vbCrLf
                                    End If
                                End If
                            End If
                        End If

                        If C_DP.P_CBJournal Then

                            Select Case .Typ
                                Case C_Telefonat.JournalTyp.Eingehend
                                    If .Dauer = 0 Then
                                        C_DP.P_StatVerpasst += 1
                                        CallDirection = "Verpasster Anruf von"
                                    Else
                                        CallDirection = "Eingehender Anruf von"
                                    End If
                                Case C_Telefonat.JournalTyp.Ausgehend
                                    If .Dauer = 0 Then
                                        C_DP.P_StatNichtErfolgreich += 1
                                        CallDirection = "Nicht erfolgreicher Anruf zu"
                                    Else
                                        CallDirection = "Ausgehender Anruf zu"
                                    End If
                            End Select

                            .Categories = .TelName & "; FritzBox Anrufmonitor; Telefonanrufe"
                            .Subject = CallDirection & " " & .Anrufer & CStr(IIf(.Anrufer = .TelNr, C_DP.P_Def_StringEmpty, " (" & .TelNr & ")")) & CStr(IIf(Split(.TelName, ";", , CompareMethod.Text).Length = 1, C_DP.P_Def_StringEmpty, " (" & .TelName & ")"))

                            C_OlI.ErstelleJournalEintrag(Telefonat)
                        End If

                        'Statistik
                        If .Dauer > 0 Then
                            With xPathTeile
                                .Item(.Count - 1) = C_Telefonat.JournalTyp.Ausgehend.ToString
                            End With
                            With C_DP
                                .Write(xPathTeile, CStr(CInt(.Read(xPathTeile, CStr(0))) + Telefonat.Dauer * 60))
                            End With
                        End If
                        C_DP.P_StatJournal += 1

                        If .Zeit > SchließZeit Or SchließZeit = System.DateTime.Now Then C_DP.P_StatOLClosedZeit = System.DateTime.Now.AddMinutes(1)
                    End If
                Else
                    C_hf.LogFile("AnrMonDISCONNECT: Ein unvollständiges Telefonat wurde registriert.")
                    With xPathTeile
                        .Add("Telefone")
                        .Add("Nummern")
                        .Add("*")
                        .Add("[. = """ & C_hf.OrtsVorwahlEntfernen(Telefonat.MSN, C_DP.P_TBVorwahl) & """]")
                        .Add("@Checked")
                    End With
                    If C_DP.P_CBJournal And C_hf.IsOneOf("1", Split(C_DP.Read(xPathTeile, "0;") & ";", ";", , CompareMethod.Text)) Then
                        ' Wenn Anruf vor dem Outlookstart begonnen wurde, wurde er nicht nachträglich importiert.
                        Dim ZeitAnruf As Date = CDate(FBStatus(0))
                        ZeitAnruf = ZeitAnruf.AddSeconds(-1 * (ZeitAnruf.Second + .Dauer + 70))
                        If ZeitAnruf < SchließZeit Then C_DP.P_StatOLClosedZeit = ZeitAnruf
                        C_hf.LogFile("AnrMonDISCONNECT: Journalimport wird gestartet")
                        Dim formjournalimort As New formJournalimport(Me, C_hf, C_DP, False)
                    End If
                End If

                If C_DP.P_CBStoppUhrEinblenden And ShowForms Then StoppUhrDaten(.ID).Abbruch = True
                'Notizeintag
                If C_DP.P_CBNote Then
                    If Not .olContact Is Nothing Then
                        C_KF.FillNote(AnrMonEvent.AnrMonDISCONNECT, Telefonat, C_DP.P_CBAnrMonZeigeKontakt)
                    End If
                End If
            End With
        End If
        C_hf.NAR(Telefonat.olContact)
        Telefonat.olContact = Nothing
        TelefonatsListe.Remove(Telefonat)

    End Sub '(AnrMonDISCONNECT)
#End Region

#Region "LetzterAnrufer"
    Sub SpeichereLetzerAnrufer(ByVal ID As String, ByVal LA As String())
        'LA(0) = Zeit
        'LA(1) = Anrufer
        'LA(2) = TelNr
        'LA(3) = MSN
        'LA(4) = StoreID
        'LA(5) = KontaktID

        Dim LANodeNames As New ArrayList
        Dim LANodeValues As New ArrayList
        Dim xPathTeile As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList

        ' Uhrzeit
        LANodeNames.Add("Zeit")
        LANodeValues.Add(LA(0))

        ' Anrufername
        If Not LA(1) = C_DP.P_Def_StringEmpty Then
            LANodeNames.Add("Anrufer")
            LANodeValues.Add(LA(1))
        End If

        ' TelNr
        LANodeNames.Add("TelNr")
        LANodeValues.Add(LA(2))

        ' MSN
        LANodeNames.Add("MSN")
        LANodeValues.Add(LA(3))

        ' StoreID
        If Not LA(4) = C_DP.P_Def_StringEmpty Then
            LANodeNames.Add("StoreID")
            LANodeValues.Add(LA(4))
        End If

        ' KontaktID
        If Not LA(4) = C_DP.P_Def_StringEmpty Then
            LANodeNames.Add("KontaktID")
            LANodeValues.Add(LA(5))
        End If

        AttributeNames.Add("ID")
        AttributeValues.Add(ID)

        xPathTeile.Add("LetzterAnrufer")
        xPathTeile.Add("Letzter")

        With C_DP
            .Write(xPathTeile, ID)
            xPathTeile.Remove("Letzter")
            .AppendNode(xPathTeile, .CreateXMLNode("Eintrag", LANodeNames, LANodeValues, AttributeNames, AttributeValues))
        End With
        xPathTeile = Nothing
        LANodeNames = Nothing
        LANodeValues = Nothing
        AttributeNames = Nothing
        AttributeValues = Nothing
    End Sub
#End Region

#Region "RingCallList"
    Sub UpdateList(ByVal ListName As String, _
                   ByVal Anrufer As String, _
                   ByVal TelNr As String, _
                   ByVal Zeit As String, _
                   ByVal StoreID As String, _
                   ByVal KontaktID As String)

        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList
        Dim xPathTeile As New ArrayList
        Dim index As Integer              ' Zählvariable

        index = CInt(C_DP.Read(ListName, "Index", "0"))

        xPathTeile.Add(ListName)
        xPathTeile.Add("Eintrag[@ID=""" & index - 1 & """]")
        xPathTeile.Add("TelNr")
        If Not C_hf.TelNrVergleich(C_DP.Read(xPathTeile, "0"), TelNr) Then

            If Not Anrufer = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("Anrufer")
                NodeValues.Add(Anrufer)
            End If

            If Not TelNr = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("TelNr")
                NodeValues.Add(TelNr)
            End If

            If Not Zeit = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("Zeit")
                NodeValues.Add(Zeit)
            End If

            NodeNames.Add("Index")
            NodeValues.Add(CStr((index + 1) Mod 10))

            If Not StoreID = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("StoreID")
                NodeValues.Add(StoreID)
            End If

            If Not KontaktID = C_DP.P_Def_StringEmpty Then
                NodeNames.Add("KontaktID")
                NodeValues.Add(KontaktID)
            End If

            AttributeNames.Add("ID")
            AttributeValues.Add(CStr(index))

            With C_DP
                xPathTeile.Clear() 'RemoveRange(0, xPathTeile.Count)
                xPathTeile.Add(ListName)
                xPathTeile.Add("Index")
                .Write(xPathTeile, CStr((index + 1) Mod 10))
                xPathTeile.Remove("Index")
                .AppendNode(xPathTeile, .CreateXMLNode("Eintrag", NodeNames, NodeValues, AttributeNames, AttributeValues))
            End With
        Else
            ' Zeit anpassen
            If Not Zeit = C_DP.P_Def_StringEmpty Then
                xPathTeile.Item(xPathTeile.Count - 1) = "Zeit"
                C_DP.Write(xPathTeile, Zeit)
            End If
        End If
        xPathTeile = Nothing
        NodeNames = Nothing
        NodeValues = Nothing
        AttributeNames = Nothing
        AttributeValues = Nothing
#If OVer > 12 Then
        C_GUI.RefreshRibbon()
#End If

    End Sub
#End Region

End Class
