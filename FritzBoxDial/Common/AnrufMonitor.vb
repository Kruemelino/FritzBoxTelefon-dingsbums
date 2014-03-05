Imports System.Net
Imports System.IO
Imports System.ComponentModel
Imports System.Net.Sockets
Imports System.Threading
Imports System.Collections.Generic

Friend Class AnrufMonitor
    Private WithEvents BWAnrMonEinblenden As BackgroundWorker
    Private WithEvents BWStoppuhrEinblenden As BackgroundWorker
    Private WithEvents BWStartTCPReader As BackgroundWorker
    Private WithEvents TimerReStart As System.Timers.Timer
    Private WithEvents TimerCheckAnrMon As System.Timers.Timer

    Private ReceiveThread As Thread
    Private AnrMonList As New ArrayList
    Private Shared AnrMonStream As NetworkStream
    Private STUhrDaten(5) As StructStoppUhr

    Private C_GUI As GraphicalUserInterface
    Private C_OlI As OutlookInterface
    Private C_KF As Contacts
    Private C_DP As DataProvider
    Private C_hf As Helfer
    Private F_Config As formCfg
    Private F_RWS As formRWSuche
    Private F_StoppUhr As formStoppUhr

    Private StandbyCounter As Integer

    Private _AnrMonAktiv As Boolean                    ' damit 'AnrMonAktion' nur einmal aktiv ist
    Private _AnrMonError As Boolean
    Private _AnrMonPhoner As Boolean = False

    Private TelefonatsListe As New List(Of C_Telefonat)

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
        Dim ID As Integer = CInt(e.Argument)
        AnrMonList.Add(New formAnrMon(CInt(ID), True, C_DP, C_hf, Me, C_OlI, C_KF))
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

        With STUhrDaten(ID)
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

    Friend Function TelefonName(ByVal MSN As String) As String
        TelefonName = C_DP.P_Def_StringEmpty
        If Not MSN = C_DP.P_Def_StringEmpty Then
            If Not AnrMonPhoner Then
                Dim xPathTeile As New ArrayList
                With xPathTeile
                    .Add("Telefone")
                    .Add("Telefone")
                    .Add("*")
                    .Add("Telefon")
                    .Add("[TelNr = """ & MSN & """ and not(@Dialport > 599)]") ' Keine Anrufbeantworter
                    .Add("TelName")
                End With
                TelefonName = Replace(C_DP.Read(xPathTeile, ""), ";", ", ")
                xPathTeile = Nothing
            Else
                TelefonName = "Phoner" ' ,  werden danach entfernt.
            End If
        End If
    End Function
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
        Dim TelNr As String            ' ermittelte TelNr

        Dim ID As Integer            ' ID des Telefonats
        Dim rws As Boolean = False    ' 'true' wenn die Rückwärtssuche erfolgreich war
        Dim LetzterAnrufer(5) As String
        Dim RWSIndex As Boolean

        Dim Anrufer As String = C_DP.P_Def_StringEmpty           ' ermittelter Anrufer
        Dim vCard As String = C_DP.P_Def_StringEmpty           ' vCard des Anrufers
        Dim KontaktID As String = C_DP.P_Def_StringEmpty             ' ID der Kontaktdaten des Anrufers
        Dim StoreID As String = C_DP.P_Def_StringEmpty           ' ID des Ordners, in dem sich der Kontakt befindet
        Dim FullName As String = C_DP.P_Def_StringEmpty
        Dim CompanyName As String = C_DP.P_Def_StringEmpty
        Dim MSN As String = C_hf.OrtsVorwahlEntfernen(CStr(FBStatus.GetValue(4)), C_DP.P_TBVorwahl)

        Dim Telefonat As C_Telefonat

        ' Anruf nur anzeigen, wenn die MSN stimmt
        Dim xPathTeile As New ArrayList
        With xPathTeile
            .Add("Telefone")
            .Add("Nummern")
            .Add("*")
            .Add("[. = """ & C_hf.OrtsVorwahlEntfernen(MSN, C_DP.P_TBVorwahl) & """]")
            .Add("@Checked")
        End With

        If C_hf.IsOneOf("1", Split(C_DP.Read(xPathTeile, "0;") & ";", ";", , CompareMethod.Text)) Or AnrMonPhoner Then
            'If C_hf.IsOneOf(C_hf.OrtsVorwahlEntfernen(MSN, Vorwahl), Split(checkstring, ";", , CompareMethod.Text)) Or AnrMonPhoner Then

            ID = CInt(FBStatus.GetValue(2))
            TelNr = CStr(FBStatus.GetValue(3))
            ' Phoner
            If AnrMonPhoner Then
                Dim PhonerTelNr() As String
                Dim pos As Integer = InStr(TelNr, "@", CompareMethod.Text)
                If Not pos = 0 Then
                    TelNr = Left(TelNr, pos - 1)
                Else
                    PhonerTelNr = C_hf.TelNrTeile(TelNr)
                    If Not PhonerTelNr(1) = C_DP.P_Def_StringEmpty Then TelNr = PhonerTelNr(1) & Mid(TelNr, InStr(TelNr, ")", CompareMethod.Text) + 2)
                    If Not PhonerTelNr(0) = C_DP.P_Def_StringEmpty Then TelNr = PhonerTelNr(0) & Mid(TelNr, 2)
                End If
                TelNr = C_hf.nurZiffern(TelNr, C_DP.P_TBLandesVW)
            End If
            ' Ende Phoner

            If Len(TelNr) = 0 Then TelNr = C_DP.P_Def_StringUnknown
            LetzterAnrufer(0) = CStr(FBStatus.GetValue(0)) 'Zeit
            LetzterAnrufer(1) = Anrufer
            LetzterAnrufer(2) = TelNr
            LetzterAnrufer(3) = MSN
            SpeichereLetzerAnrufer(CStr(ID), LetzterAnrufer)
            ' Daten für Anzeige im Anrurfmonitor speichern
            If ShowForms And Not C_OlI.VollBildAnwendungAktiv Then
                BWAnrMonEinblenden = New BackgroundWorker
                BWAnrMonEinblenden.RunWorkerAsync(ID)
            End If

            ' Daten in den Kontakten suchen und per Rückwärtssuche ermitteln
            If Not TelNr = C_DP.P_Def_StringUnknown Then
                ' Anrufer in den Outlook-Kontakten suchen
                If C_OlI.StarteKontaktSuche(KontaktID, StoreID, C_DP.P_CBKHO, TelNr, "", C_DP.P_TBLandesVW) Then
                    C_OlI.KontaktInformation(KontaktID, StoreID, FullName:=FullName, CompanyName:=CompanyName)
                    Anrufer = Replace(FullName & " (" & CompanyName & ")", " ()", "")
                    If C_DP.P_CBIgnoTelNrFormat Then TelNr = C_hf.formatTelNr(TelNr)
                Else
                    ' Anrufer per Rückwärtssuche ermitteln
                    If C_DP.P_CBRWS Then
                        If RWSIndex Then
                            With xPathTeile
                                .Clear()
                                .Add("CBRWSIndex")
                                .Add("Eintrag[@ID=""" & TelNr & """]")
                            End With
                            vCard = C_DP.Read(xPathTeile, Nothing)
                        End If
                        If vCard = Nothing Then
                            Select Case C_DP.P_ComboBoxRWS
                                Case 0
                                    rws = F_RWS.RWS11880(TelNr, vCard)
                                Case 1
                                    rws = F_RWS.RWSDasTelefonbuch(TelNr, vCard)
                                Case 2
                                    rws = F_RWS.RWStelsearch(TelNr, vCard)
                                Case 3
                                    rws = F_RWS.RWSAlle(TelNr, vCard)
                            End Select
                            'Im folgenden wird automatisch ein Kontakt erstellt, der durch die Rückwärtssuche ermittlt wurde. Dies geschieht nur, wenn es gewünscht ist.
                            If rws And C_DP.P_CBKErstellen Then
                                C_KF.ErstelleKontakt(KontaktID, StoreID, vCard, TelNr)
                                C_OlI.KontaktInformation(KontaktID, StoreID, FullName:=FullName, CompanyName:=CompanyName)
                                Anrufer = Replace(FullName & " (" & CompanyName & ")", " ()", "")
                                rws = False
                            End If
                        Else
                            rws = True
                        End If

                        If rws Then
                            Anrufer = ReadFNfromVCard(vCard)
                            Anrufer = Replace(Anrufer, Chr(13), "", , , CompareMethod.Text)
                            If InStr(1, Anrufer, "Firma", CompareMethod.Text) = 1 Then Anrufer = Right(Anrufer, Len(Anrufer) - 5)
                            Anrufer = Trim(Anrufer)
                            If RWSIndex Then
                                xPathTeile.Item(xPathTeile.Count - 1) = "Eintrag"
                                C_DP.Write(xPathTeile, vCard, "ID", TelNr)
                            End If
                            KontaktID = C_DP.P_Def_ErrorMinusOne & Anrufer & ";" & vCard
                        End If
                    End If
                    TelNr = C_hf.formatTelNr(TelNr)
                End If

                LetzterAnrufer(1) = Anrufer
                LetzterAnrufer(2) = TelNr
                LetzterAnrufer(4) = StoreID
                LetzterAnrufer(5) = KontaktID
                SpeichereLetzerAnrufer(CStr(ID), LetzterAnrufer)
                UpdateList("RingList", Anrufer, TelNr, FBStatus(0), StoreID, KontaktID)
#If OVer < 14 Then
                If C_DP.P_CBSymbAnrListe Then C_GUI.FillPopupItems("AnrListe")
#End If
            End If
            'StoppUhr
            If C_DP.P_CBStoppUhrEinblenden And ShowForms Then
                With STUhrDaten(ID)
                    .Richtung = "Anruf von:"
                    If Anrufer = C_DP.P_Def_StringEmpty Then
                        .Anruf = TelNr
                    Else
                        .Anruf = Anrufer
                    End If
                End With
            End If

            ' Telefonatdaten intern sichern
            Telefonat = New C_Telefonat
            With Telefonat
                .ID = ID
                .Typ = C_Telefonat.JournalTyp.Eingehend
                .Zeit = CDate(FBStatus.GetValue(0))
                .MSN = MSN
                .TelNr = TelNr
                .KontaktID = KontaktID
                .StoreID = StoreID
                .olContact = C_KF.ZeigeKontakt(KontaktID, StoreID, TelNr, False)
            End With
            TelefonatsListe.Add(Telefonat)

            ' Kontakt öffnen
            If C_DP.P_CBAnrMonZeigeKontakt And ShowForms Then
                C_KF.AddNote(C_KF.ZeigeKontakt(KontaktID, StoreID, TelNr, True))
            End If

            'Notizeintag
            If C_DP.P_CBNote Then
                With Telefonat
                    C_KF.FillNote(AnrMonEvent.AnrMonRING, .olContact, CStr(FBStatus.GetValue(0)), .TelNr, CDbl(C_DP.P_Def_ErrorMinusOne), C_DP.P_CBAnrMonZeigeKontakt)
                End With
            End If
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

        Dim ID As Integer = CInt(FBStatus.GetValue(2))  ' ID des Telefonats
        Dim NSN As Long = CLng(FBStatus.GetValue(3)) ' Nebenstellennummer des Telefonates
        Dim MSN As String = C_hf.OrtsVorwahlEntfernen(CStr(FBStatus.GetValue(4)), C_DP.P_TBVorwahl)  ' Ausgehende eigene Telefonnummer, MSN
        Dim TelNr As String                             ' ermittelte TelNr
        Dim Anrufer As String = C_DP.P_Def_StringEmpty            ' ermittelter Anrufer
        Dim vCard As String = C_DP.P_Def_StringEmpty                        ' vCard des Anrufers
        Dim KontaktID As String = C_DP.P_Def_ErrorMinusOne & ";"                 ' ID der Kontaktdaten des Anrufers
        Dim StoreID As String = C_DP.P_Def_ErrorMinusOne                    ' ID des Ordners, in dem sich der Kontakt befindet

        Dim rws As Boolean                              ' 'true' wenn die Rückwärtssuche erfolgreich war
        Dim RWSIndex As Boolean
        Dim xPathTeile As New ArrayList

        Dim Telefonat As C_Telefonat
        ' Problem DECT/IP-Telefone: keine MSN  über Anrufmonitor eingegangen. Aus Datei ermitteln.
        If MSN = C_DP.P_Def_StringEmpty Then
            Select Case NSN
                Case 0 To 2 ' FON1-3
                    NSN += 1
                Case 10 To 19 ' DECT
                    NSN += 50
            End Select
            Select Case NSN
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
                        .Add("Telefon[@Dialport = """ & NSN & """]")
                        .Add("TelNr")
                    End With
                    MSN = C_DP.Read(xPathTeile, "")
            End Select
        End If

        With xPathTeile
            .Clear()
            .Add("Telefone")
            .Add("Nummern")
            .Add("*")
            .Add("[. = """ & Replace(MSN, ";", """ or . = """, , , CompareMethod.Text) & """]")
            .Add("@Checked")
        End With

        If C_hf.IsOneOf("1", Split(C_DP.Read(xPathTeile, "0;") & ";", ";", , CompareMethod.Text)) Or AnrMonPhoner Then

            TelNr = C_hf.nurZiffern(CStr(FBStatus.GetValue(5)), C_DP.P_TBLandesVW)
            If TelNr = C_DP.P_Def_StringEmpty Then TelNr = C_DP.P_Def_StringUnknown
            ' CbC-Vorwahl entfernen
            If Left(TelNr, 4) = "0100" Then TelNr = Right(TelNr, Len(TelNr) - 6)
            If Left(TelNr, 3) = "010" Then TelNr = Right(TelNr, Len(TelNr) - 5)
            If Not Left(TelNr, 1) = "0" And Not Left(TelNr, 2) = "11" And Not Left(TelNr, 1) = "+" Then TelNr = C_DP.P_TBVorwahl & TelNr
            ' Raute entfernen
            If Right(TelNr, 1) = "#" Then TelNr = Left(TelNr, Len(TelNr) - 1)
            ' Daten zurücksetzen
            'Anrufer = TelNr
            If Not TelNr = C_DP.P_Def_StringUnknown Then
                Dim FullName As String = C_DP.P_Def_StringEmpty
                Dim CompanyName As String = C_DP.P_Def_StringEmpty
                ' Anrufer in den Outlook-Kontakten suchen
                If C_OlI.StarteKontaktSuche(KontaktID, StoreID, C_DP.P_CBKHO, TelNr, "", C_DP.P_TBLandesVW) Then
                    C_OlI.KontaktInformation(KontaktID, StoreID, FullName:=FullName, CompanyName:=CompanyName)
                    Anrufer = Replace(FullName & " (" & CompanyName & ")", " ()", "")
                    If C_DP.P_CBIgnoTelNrFormat Then TelNr = C_hf.formatTelNr(TelNr)
                Else
                    ' Anrufer per Rückwärtssuche ermitteln
                    If C_DP.P_CBRWS Then
                        RWSIndex = C_DP.P_CBRWSIndex
                        If RWSIndex Then
                            With xPathTeile
                                .Clear()
                                .Add("CBRWSIndex")
                                .Add("Eintrag[@ID=""" & TelNr & """]")
                            End With
                            vCard = C_DP.Read(xPathTeile, Nothing)
                        End If
                        If vCard = Nothing Then
                            Select Case C_DP.P_ComboBoxRWS
                                Case 0
                                    rws = F_RWS.RWS11880(TelNr, vCard)
                                Case 1
                                    rws = F_RWS.RWSDasTelefonbuch(TelNr, vCard)
                                Case 2
                                    rws = F_RWS.RWStelsearch(TelNr, vCard)
                                Case 3
                                    rws = F_RWS.RWSAlle(TelNr, vCard)
                            End Select
                            'Im folgenden wird automatisch ein Kontakt erstellt, der durch die Rückwärtssuche ermittlt wurde. Dies geschieht nur, wenn es gewünscht ist.
                            If rws And C_DP.P_CBKErstellen Then
                                C_KF.ErstelleKontakt(KontaktID, StoreID, vCard, TelNr)
                                C_OlI.KontaktInformation(KontaktID, StoreID, FullName:=FullName, CompanyName:=CompanyName)
                                Anrufer = Replace(FullName & " (" & CompanyName & ")", " ()", "")
                            End If
                        Else
                            rws = True
                        End If
                        If rws And KontaktID = C_DP.P_Def_ErrorMinusOne & ";" Then
                            Anrufer = ReadFNfromVCard(vCard)
                            Anrufer = Replace(Anrufer, Chr(13), "", , , CompareMethod.Text)
                            If InStr(1, Anrufer, "Firma", CompareMethod.Text) = 1 Then
                                Anrufer = Right(Anrufer, Len(Anrufer) - 5)
                            End If
                            Anrufer = Trim(Anrufer)
                            If RWSIndex Then
                                xPathTeile.Item(xPathTeile.Count - 1) = "Eintrag"
                                C_DP.Write(xPathTeile, vCard, "ID", TelNr)
                            End If
                            KontaktID = C_DP.P_Def_ErrorMinusOne & Anrufer & ";" & vCard
                        End If
                    End If
                    TelNr = C_hf.formatTelNr(TelNr)
                End If
            End If
            ' Daten im Menü für Wahlwiederholung speichern
            UpdateList("CallList", Anrufer, TelNr, FBStatus(0), StoreID, KontaktID)
#If OVer < 14 Then
            If C_DP.P_CBSymbWwdh Then C_GUI.FillPopupItems("Wwdh")
#End If
            'StoppUhr
            If C_DP.P_CBStoppUhrEinblenden And ShowForms Then
                With STUhrDaten(ID)
                    .Richtung = "Anruf zu:"
                    If Anrufer = C_DP.P_Def_StringEmpty Then
                        .Anruf = TelNr
                    Else
                        .Anruf = Anrufer
                    End If
                End With
            End If
            ' Journal
            Telefonat = New C_Telefonat
            With Telefonat
                .ID = ID
                .Typ = C_Telefonat.JournalTyp.Ausgehend
                .Zeit = CDate(FBStatus.GetValue(0))
                .MSN = MSN
                .TelNr = TelNr
                .KontaktID = KontaktID
                .StoreID = StoreID
                .NSN = CLng(FBStatus.GetValue(3))
                .olContact = C_KF.ZeigeKontakt(KontaktID, StoreID, TelNr, False)
            End With
            TelefonatsListe.Add(Telefonat)
            ' Kontakt öffnen
            If C_DP.P_CBAnrMonZeigeKontakt And ShowForms Then
                C_KF.AddNote(C_KF.ZeigeKontakt(KontaktID, StoreID, TelNr, True))
            End If
            'Notizeintag
            If C_DP.P_CBNote Then
                With Telefonat
                    C_KF.FillNote(AnrMonEvent.AnrMonCALL, .olContact, CStr(FBStatus.GetValue(0)), .TelNr, CDbl(C_DP.P_Def_ErrorMinusOne), C_DP.P_CBAnrMonZeigeKontakt)
                End With
            End If

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
        Dim MSN As String = C_DP.P_Def_ErrorMinusOne
        Dim NSN As Long
        Dim ID As Integer
        Dim Zeit As String
        Dim TelName As String = C_DP.P_Def_StringEmpty

        Dim Telefonat As C_Telefonat
        ID = CInt(FBStatus.GetValue(2))
        NSN = CInt(FBStatus.GetValue(3))
        Telefonat = TelefonatsListe.Find(Function(JE) JE.ID = ID)
        If Not Telefonat Is Nothing Then
            With Telefonat
                MSN = .MSN
                .NSN = NSN
                .Zeit = CDate(FBStatus.GetValue(0))
            End With
        End If

        If C_DP.P_CBJournal Or (C_DP.P_CBStoppUhrEinblenden And ShowForms) Then
            If MSN = C_DP.P_Def_ErrorMinusOne Then
                ' Wenn Journal nicht erstellt wird, muss MSN anderweitig ermittelt werden.
                Select Case NSN
                    Case 0 To 2 ' FON1-3
                        NSN += 1
                    Case 10 To 19 ' DECT
                        NSN += 50
                End Select
                Select Case NSN
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
                            .Clear()
                            .Add("Telefone")
                            .Add("Telefone")
                            .Add("*")
                            .Add("Telefon[@Dialport = """ & NSN & """]")
                            .Add("TelNr")
                            MSN = C_DP.Read(xPathTeile, "")
                            .Item(.Count - 1) = "TelName"
                            TelName = C_DP.Read(xPathTeile, "")
                        End With
                End Select
            End If

            If MSN = C_DP.P_Def_ErrorMinusOne Then
                C_hf.LogFile("Ein unvollständiges Telefonat wurde registriert.")
            Else
                With xPathTeile
                    .Clear()
                    .Add("Telefone")
                    .Add("Nummern")
                    .Add("*")
                    .Add("[. = """ & Replace(MSN, ";", """ or . = """, , , CompareMethod.Text) & """]")
                    .Add("@Checked")
                End With

                If C_hf.IsOneOf("1", Split(C_DP.Read(xPathTeile, "0;") & ";", ";", , CompareMethod.Text)) Or AnrMonPhoner Then
                    ' StoppUhr einblenden
                    If C_DP.P_CBStoppUhrEinblenden And ShowForms Then
                        C_hf.LogFile("StoppUhr wird eingeblendet.")
                        With System.DateTime.Now
                            Zeit = String.Format("{0:00}:{1:00}:{2:00}", .Hour, .Minute, .Second)
                        End With
                        With STUhrDaten(ID)
                            .MSN = CStr(IIf(TelName = C_DP.P_Def_StringEmpty, MSN, TelName))
                            .StartZeit = Zeit
                            .Abbruch = False
                        End With
                        BWStoppuhrEinblenden = New BackgroundWorker
                        With BWStoppuhrEinblenden
                            .WorkerSupportsCancellation = True
                            .RunWorkerAsync(ID)
                        End With
                    End If
                End If
            End If
        End If
        'Notizeintag
        If C_DP.P_CBNote Then
            With Telefonat
                C_KF.FillNote(AnrMonEvent.AnrMonCONNECT, .olContact, CStr(FBStatus.GetValue(0)), .TelNr, .Dauer, False)
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

        Dim ID As Integer = CInt(FBStatus.GetValue(2))          ' ID des Telefonats
        Dim Dauer As Integer = CInt(FBStatus.GetValue(3))          ' Dauer des Telefonats in s
        Dim AnrName As String              ' Name des Telefonpartners
        Dim Firma As String              ' Firma des Telefonpartners
        Dim Body As String              ' Text des Journaleintrags
        Dim vCard As String              ' vCard des Telefonpartners
        Dim TypString As String = C_DP.P_Def_StringEmpty
        ' die zum Anruf gehörende MSN oder VoIP-Nr
        Dim TelName As String
        Dim tmpTelName As String = C_DP.P_Def_StringEmpty

        Dim NSN As Long = -1
        Dim Zeit As Date
        Dim Typ As C_Telefonat.JournalTyp
        Dim MSN As String = C_DP.P_Def_StringEmpty
        Dim TelNr As String = C_DP.P_Def_StringEmpty
        Dim StoreID As String = C_DP.P_Def_StringEmpty
        Dim KontaktID As String = C_DP.P_Def_StringEmpty

        Dim SchließZeit As Date = C_DP.P_StatOLClosedZeit
        Dim xPathTeile As New ArrayList
        Dim Telefonat As C_Telefonat

        Telefonat = TelefonatsListe.Find(Function(JE) JE.ID = ID)
        If Not Telefonat Is Nothing Then
            With Telefonat
                ID = .ID
                NSN = .NSN
                Zeit = .Zeit
                Typ = .Typ
                MSN = .MSN
                TelNr = .TelNr
                KontaktID = .KontaktID
                StoreID = .StoreID
                .Dauer = CInt(IIf(Dauer <= 30, 31, Dauer)) \ 60
            End With
        End If

        Dim JMSN As String = C_hf.OrtsVorwahlEntfernen(MSN, C_DP.P_TBVorwahl)
        If Not MSN = Nothing Then
            With xPathTeile
                .Add("Telefone")
                .Add("Nummern")
                .Add("*")
                .Add("[. = """ & C_hf.OrtsVorwahlEntfernen(MSN, C_DP.P_TBVorwahl) & """]")
                .Add("@Checked")
            End With

            If C_hf.IsOneOf("1", Split(C_DP.Read(xPathTeile, "0;") & ";", ";", , CompareMethod.Text)) Or AnrMonPhoner Then

                Body = "Tel.-Nr.: " & TelNr & vbCrLf & "Status: " & CStr(IIf(Dauer = 0, "nicht ", C_DP.P_Def_StringEmpty)) & "angenommen" & vbCrLf & vbCrLf

                If Left(KontaktID, 2) = C_DP.P_Def_ErrorMinusOne Then
                    ' kein Kontakt vorhanden
                    AnrName = Mid(KontaktID, 3, InStr(KontaktID, ";") - 3)
                    If AnrName = C_DP.P_Def_StringEmpty Then AnrName = TelNr
                    If InStr(1, AnrName, "Firma", CompareMethod.Text) = 1 Then
                        AnrName = Right(AnrName, Len(AnrName) - 5)
                    End If
                    AnrName = Trim(AnrName)
                    vCard = Mid(KontaktID, InStr(KontaktID, ";") + 1)
                    Firma = ReadFromVCard(vCard, "ORG", "")
                    If Not vCard = C_DP.P_Def_StringEmpty Then Body = Body & "Kontaktdaten (vCard):" & vbCrLf & vCard & vbCrLf
                Else
                    ' Kontakt in den 'Links' eintragen
                    Dim FullName As String = C_DP.P_Def_StringEmpty
                    Dim CompanyName As String = C_DP.P_Def_StringEmpty
                    Dim HomeAddress As String = C_DP.P_Def_StringEmpty
                    Dim BusinessAddress As String = C_DP.P_Def_StringEmpty

                    C_OlI.KontaktInformation(KontaktID, StoreID, FullName:=FullName, CompanyName:=CompanyName, BusinessAddress:=BusinessAddress, HomeAddress:=HomeAddress)

                    If FullName = C_DP.P_Def_StringEmpty Then
                        If CompanyName = C_DP.P_Def_StringEmpty Then
                            AnrName = TelNr
                        Else
                            AnrName = CompanyName
                        End If
                    Else
                        AnrName = FullName
                    End If
                    Firma = CompanyName
                    If Firma = C_DP.P_Def_StringEmpty Then
                        If Not HomeAddress = C_DP.P_Def_StringEmpty Then
                            Body = Body & "Kontaktdaten:" & vbCrLf & AnrName _
                                & vbCrLf & Firma & vbCrLf & HomeAddress & vbCrLf
                        End If
                    Else
                        If Not BusinessAddress = C_DP.P_Def_StringEmpty Then
                            Body = Body & "Kontaktdaten:" & vbCrLf & AnrName _
                                & vbCrLf & Firma & vbCrLf & BusinessAddress & vbCrLf
                        End If
                    End If
                End If

                If C_DP.P_CBJournal Then
                    Select Case NSN
                        Case 0 To 2 ' FON1-3
                            NSN += 1
                        Case 10 To 19 ' DECT
                            NSN += 50
                    End Select
                    Select Case NSN
                        Case 3
                            TelName = "Durchwahl"
                        Case 4
                            TelName = "ISDN Gerät"
                        Case 5
                            TelName = "Fax (intern/PC)"
                        Case 36
                            TelName = "Data S0"
                        Case 37
                            TelName = "Data PC"
                        Case Else
                            With xPathTeile
                                .Clear()
                                .Add("Telefone")
                                .Add("Telefone")
                                .Add("*")
                                .Add("Telefon[@Dialport = """ & NSN & """]")
                                .Add("TelName")
                            End With
                            TelName = C_DP.Read(xPathTeile, "")
                    End Select

                    Select Case Typ
                        Case C_Telefonat.JournalTyp.Eingehend
                            If Dauer = 0 Then
                                C_DP.P_StatVerpasst += 1
                                TypString = "Verpasster Anruf von"
                            Else
                                TypString = "Eingehender Anruf von"
                            End If
                        Case C_Telefonat.JournalTyp.Ausgehend
                            If Dauer = 0 Then
                                C_DP.P_StatNichtErfolgreich += 1
                                TypString = "Nicht erfolgreicher Anruf zu"
                            Else
                                TypString = "Ausgehender Anruf zu"
                            End If
                    End Select
                    ' für Journal
                    With Telefonat
                        .Body = Body
                        .Categories = TelName & "; FritzBox Anrufmonitor; Telefonanrufe"
                        .Companies = Firma
                        .Subject = TypString & " " & AnrName & CStr(IIf(AnrName = TelNr, C_DP.P_Def_StringEmpty, " (" & TelNr & ")")) & CStr(IIf(Split(TelName, ";", , CompareMethod.Text).Length = 1, C_DP.P_Def_StringEmpty, " (" & TelName & ")"))
                    End With
                    ' Journaleintrag schreiben

                    C_OlI.ErstelleJournalEintrag(Telefonat)
                End If

                'Statistik
                If Dauer > 0 Then
                    With xPathTeile
                        .Item(.Count - 1) = C_Telefonat.JournalTyp.Ausgehend.ToString
                    End With
                    With C_DP
                        .Write(xPathTeile, CStr(CInt(.Read(xPathTeile, CStr(0))) + Dauer))
                    End With
                End If
                C_DP.P_StatJournal += 1

                If CDate(Zeit) > SchließZeit Or SchließZeit = System.DateTime.Now Then C_DP.P_StatOLClosedZeit = System.DateTime.Now.AddMinutes(1)

            End If
        Else
            C_hf.LogFile("AnrMonDISCONNECT: Ein unvollständiges Telefonat wurde registriert.")
            With xPathTeile
                .Add("Telefone")
                .Add("Nummern")
                .Add("*")
                .Add("[. = """ & C_hf.OrtsVorwahlEntfernen(JMSN, C_DP.P_TBVorwahl) & """]")
                .Add("@Checked")
            End With
            If C_DP.P_CBJournal And C_hf.IsOneOf("1", Split(C_DP.Read(xPathTeile, "0;") & ";", ";", , CompareMethod.Text)) Then
                ' Wenn Anruf vor dem Outlookstart begonnen wurde, wurde er nicht nachträglich importiert.
                Dim ZeitAnruf As Date = CDate(FBStatus(0))
                ZeitAnruf = ZeitAnruf.AddSeconds(-1 * (ZeitAnruf.Second + Dauer + 70))
                If ZeitAnruf < SchließZeit Then C_DP.P_StatOLClosedZeit = ZeitAnruf
                C_hf.LogFile("AnrMonDISCONNECT: Journalimport wird gestartet")
                Dim formjournalimort As New formJournalimport(Me, C_hf, C_DP, False)
            End If
        End If

        If C_DP.P_CBStoppUhrEinblenden And ShowForms Then STUhrDaten(ID).Abbruch = True
        'Notizeintag
        If C_DP.P_CBNote Then
            With Telefonat
                C_KF.FillNote(AnrMonEvent.AnrMonDISCONNECT, .olContact, CStr(FBStatus.GetValue(0)), .TelNr, Dauer, False)
            End With
        End If
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
