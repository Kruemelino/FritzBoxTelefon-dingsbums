Imports System.Threading
Imports System.Net
Imports System.IO
Imports System.ComponentModel
Imports System.Collections

Public Class AnrufMonitor
    Private WithEvents BWAnrMonEinblenden As BackgroundWorker
    Private WithEvents BWStoppuhrEinblenden As BackgroundWorker
    Private WithEvents BWStartTCPReader As BackgroundWorker
    Private WithEvents TimerReStartStandBy As System.Timers.Timer

    Private ReceiveThread As Thread
    Private AnrMonList As New Collections.ArrayList
    Private Shared Stream As Sockets.NetworkStream
    Private STUhrDaten(5) As StructStoppUhr
    Private GUI As GraphicalUserInterface
    Private OlI As OutlookInterface
    Private KontaktFunktionen As Contacts
    Private C_XML As MyXML
    Private hf As Helfer
    Private frmRWS As formRWSuche
    Private frmStopp As formStoppUhr

    Private StandbyCounter As Integer
    Friend AnrMonAktiv As Boolean                    ' damit 'AnrMonAktion' nur einmal aktiv ist
    Friend AnrMonError As Boolean
    Private TelAnzahl As Integer
    Private UseAnrMon As Boolean
    Private Eingeblendet As Integer = 0

    Private IPAddresse As String = "fritz.box"
    Private FBAnrMonPort As Integer = 1012

#Region "Phoner"
    Private AnrMonPhoner As Boolean = False
#End Region

    Public Sub New(ByVal RWS As formRWSuche, _
                   ByVal NutzeAnrMon As Boolean, _
                   ByVal iniKlasse As MyXML, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal KontaktKlasse As Contacts, _
                   ByVal InterfacesKlasse As GraphicalUserInterface, _
                   ByVal OutlInter As OutlookInterface, _
                   ByVal FBAdr As String)

        hf = HelferKlasse
        KontaktFunktionen = KontaktKlasse
        GUI = InterfacesKlasse
        C_XML = iniKlasse
        frmRWS = RWS
        UseAnrMon = NutzeAnrMon
        OlI = OutlInter
        IPAddresse = FBAdr
        ' STARTE Anrmon
        AnrMonStart(False)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
#Region "Strukturen"
    Structure StructStoppUhr
        Dim Anruf As String
        Dim Abbruch As Boolean
        Dim StartZeit As String
        Dim Richtung As String
        Dim MSN As String
    End Structure
#End Region

#Region "Anrufmonitor Grundlagen"
    Private Sub AnrMonAktion()
        ' schaut in der FritzBox im Port 1012 nach und startet entsprechende Unterprogramme
        Dim r As New StreamReader(Stream)
        Dim FBStatus As String  ' Status-String der FritzBox
        Dim aktZeile() As String  ' aktuelle Zeile im Status-String
        Dim CBStoppUhrEinblenden As Boolean = CBool(C_XML.Read("Optionen", "CBStoppUhrEinblenden", "False"))
        Do
            If Stream.DataAvailable And AnrMonAktiv Then
                FBStatus = r.ReadLine
                Select Case FBStatus
                    Case "Welcome to Phoner"
                        AnrMonPhoner = True
                    Case "Sorry, too many clients"
                        hf.LogFile("AnrMonAktion, Phoner: ""Sorry, too many clients""")
                    Case Else
                        hf.LogFile("AnrMonAktion: " & FBStatus)
                        aktZeile = Split(FBStatus, ";", , CompareMethod.Text)
                        If Not aktZeile.Length = 1 Then
                            'Schauen ob "RING", "CALL", "CONNECT" oder "DISCONNECT" übermittelt wurde
                            Select Case CStr(aktZeile.GetValue(1))
                                Case "RING"
                                    AnrMonRING(aktZeile, True, CBStoppUhrEinblenden)
                                Case "CALL"
                                    AnrMonCALL(aktZeile, CBStoppUhrEinblenden)
                                Case "CONNECT"
                                    AnrMonCONNECT(aktZeile, CBStoppUhrEinblenden)
                                Case "DISCONNECT"
                                    AnrMonDISCONNECT(aktZeile, CBStoppUhrEinblenden)
                            End Select
                        End If
                End Select
            End If
            Thread.Sleep(50)
            Windows.Forms.Application.DoEvents()
        Loop Until Not AnrMonAktiv
        r.Close()
        r = Nothing
    End Sub '(AnrMonAktion)

    Friend Function AnrMonAnAus() As Boolean 'ByRef oExp As Outlook.Explorer)
        ' wird durch das Symbol 'Anrufmonitor' in der 'FritzBox'-Symbolleiste ausgeführt
        ' schaltet den Anrufmonitor an bzw. aus

        If AnrMonAktiv Then
            ' Timer stoppen, TCP/IP-Verbindung(schließen)
            If AnrMonQuit() Then
#If OVer < 14 Then
                GUI.SetAnrMonButton(False)
#End If
            End If
            Return False
        Else
            ' Timer starten, TCP/IP-Verbindung öffnen
            If AnrMonStart(True) Then
#If OVer < 14 Then
                GUI.SetAnrMonButton(True)
#End If
            End If
            Return True
        End If

    End Function '(AnrMonAnAus)

    Function AnrMonStart(ByVal Manuell As Boolean) As Boolean
        If (C_XML.Read("Optionen", "CBAnrMonAuto", "False") = "True" Or Manuell) And UseAnrMon Then

            If CBool(C_XML.Read("Phoner", "CBPhonerAnrMon", "False")) Then
                FBAnrMonPort = 2012
                IPAddresse = "127.0.0.1"
            End If

            If hf.Ping(IPAddresse) Or CBool(C_XML.Read("Optionen", "CBForceFBAddr", "False")) Then
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
        Return True
    End Function '(AnrMonStart)

    Function AnrMonStartNachStandby() As Boolean
        AnrMonStartNachStandby = False
        If C_XML.Read("Optionen", "CBAnrMonAuto", "False") = "True" And UseAnrMon Then
            Dim FbIP As String = C_XML.Read("Optionen", "TBFBAdr", "192.168.178.1")
            If Not hf.Ping(FbIP) Then
                hf.LogFile("Standby Timer  1. Ping nicht erfolgreich")
                TimerReStartStandBy = hf.SetTimer(2000)
                StandbyCounter = 2
            Else
                hf.LogFile("Standby 1. Ping erfolgreich")
                AnrMonStart(False)
                If C_XML.Read("Optionen", "CBJournal", "False") = "True" Then
                    Dim formjournalimort As New formJournalimport(Me, hf, C_XML, False)
                End If
            End If
            Return True
        End If
    End Function

    Private Sub TimerReStartStandBy_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerReStartStandBy.Elapsed
        Dim FBAdr As String = C_XML.Read("Optionen", "TBFBAdr", "fritz.box")
        If hf.Ping(FBAdr) Then
            hf.LogFile("Standby Timer " & StandbyCounter & ". Ping erfolgreich")
            StandbyCounter = 15
            AnrMonStart(False)
            If C_XML.Read("Optionen", "CBJournal", "False") = "True" Then
                Dim formjournalimort As New formJournalimport(Me, hf, C_XML, False)
            End If
            hf.LogFile("Anrufmonitor nach StandBy neugestartet")
        End If

        If StandbyCounter >= 14 Then
            If StandbyCounter = 14 Then hf.LogFile("TimerReStartStandBy: Reaktivierung des Anrufmonitors nicht erfolgreich.")
            hf.KillTimer(TimerReStartStandBy)
        Else
            hf.LogFile("Standby Timer " & StandbyCounter & ". nicht Ping erfolgreich")
            StandbyCounter += 1
        End If

    End Sub

    Function AnrMonQuit() As Boolean
        ' wird beim Beenden von Outlook ausgeführt und beendet den Anrufmonitor
        AnrMonAktiv = False

        hf.LogFile("AnrMonQuit: Anrufmonitor beendet")
        Return True
    End Function '(AnrMonQuit)

    Friend Sub AnrMonReStart()
        Dim Erfolgreich As Boolean = AnrMonQuit()
        If Erfolgreich Then Erfolgreich = AnrMonStart(False)
    End Sub

    Friend Function TelefonName(ByVal MSN As String) As String

        Dim tempTelName() As String
        Dim Nebenstellen() As String
        Nebenstellen = (From x In Split(C_XML.Read("Telefone", "EingerichteteTelefone", "1;2;3;51;52;53;54;55;56;57;58;50;60;61;62;63;64;65;66;67;68;69;20;21;22;23;24;25;26;27;28;29"), ";", , CompareMethod.Text) Where Not x Like "60#" Select x).ToArray ' TAM entfernen
        TelefonName = vbNullString
        If Not AnrMonPhoner Then
            For Each Nebenstelle In Nebenstellen
                tempTelName = Split(C_XML.Read("Telefone", Nebenstelle, "-1;"), ";", , CompareMethod.Text)
                If Not tempTelName(0) = "-1" Or tempTelName(0) = "" Then
                    If hf.IsOneOf(MSN, Split(tempTelName(1), "_", , CompareMethod.Text)) Then
                        TelefonName += tempTelName(2) & ", "
                    End If
                End If
            Next
        Else
            TelefonName = "Phoner, " ' ,  werden danach entfernt.
        End If
        If Not TelefonName = vbNullString Then TelefonName = Left(TelefonName, Len(TelefonName) - 2)

    End Function

    Private Sub BWAnrMonEinblenden_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWAnrMonEinblenden.DoWork
        Dim ID As Integer = CInt(e.Argument)
        'Dim letzterAnrufer() As String = Split(C_XML.Read("letzterAnrufer", "letzterAnrufer" & ID, CStr(System.DateTime.Now) & ";;unbekannt;;-1;-1;"), ";", 6, CompareMethod.Text)
        AnrMonList.Add(New formAnrMon(CInt(ID), True, C_XML, hf, Me, OlI))
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
                        Thread.Sleep(2)
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
        If CBool(C_XML.Read("Optionen", "CBStoppUhrAusblenden", "False")) Then
            WarteZeit = CInt(C_XML.Read("Optionen", "TBStoppUhr", "0"))
        Else
            WarteZeit = -1
        End If

        StartPosition = New System.Drawing.Point(CInt(C_XML.Read("Optionen", "CBStoppUhrX", "10")), CInt(C_XML.Read("Optionen", "CBStoppUhrY", "10")))
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
            BWStoppuhrEinblenden.WorkerSupportsCancellation = True
            Do Until frmStUhr.StUhrClosed
                If Not Beendet And .Abbruch Then
                    frmStUhr.Stopp()
                    Beendet = True
                End If
                Thread.Sleep(20)
                Windows.Forms.Application.DoEvents()
            Loop
            C_XML.Write("Optionen", "CBStoppUhrX", CStr(frmStUhr.Position.X), False)
            C_XML.Write("Optionen", "CBStoppUhrY", CStr(frmStUhr.Position.Y), True)
            frmStUhr = Nothing
        End With
    End Sub

    Private Sub BWStartTCPReader_DoWork(sender As Object, e As DoWorkEventArgs) Handles BWStartTCPReader.DoWork
        System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(500))
        Dim IPAddress As IPAddress
        If LCase(IPAddresse) = "fritz.box" Then
            Dim IPHostInfo As IPHostEntry = Dns.GetHostEntry(IPAddresse)
            IPAddress = IPAddress.Parse(IPHostInfo.AddressList(0).ToString)
        Else
            IPAddress = IPAddress.Parse(IPAddresse)
        End If
        Dim Client As New Sockets.TcpClient()
        Dim remoteEP As New IPEndPoint(IPAddress, FBAnrMonPort)
        Try
            Client.Connect(remoteEP)
            Stream = Client.GetStream()
            Dim ReceiveThread As New Thread(AddressOf AnrMonAktion)
            ReceiveThread.IsBackground = True
            ReceiveThread.Start()
            AnrMonAktiv = ReceiveThread.IsAlive
            e.Result = AnrMonAktiv
        Catch Err As Exception
            hf.LogFile("TCP Verbindung nicht aufgebaut: " & Err.Message)
            e.Result = False
        End Try
    End Sub

    Private Sub BWStartTCPReader_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BWStartTCPReader.RunWorkerCompleted

        If CBool(e.Result) Then
#If OVer < 14 Then
            GUI.SetAnrMonButton(True)
#End If
#If OVer >= 14 Then
            GUI.InvalidateControlAnrMon()
#End If
            'hf.LogFile("BWStartTCPReader_RunWorkerCompleted: Anrufmonitor gestartet")
            AnrMonAktiv = CBool(e.Result)
            AnrMonError = False
        Else
            hf.LogFile("BWStartTCPReader_RunWorkerCompleted: Es ist ein TCP/IP Fehler aufgetreten.")
            AnrMonAktiv = False
            AnrMonError = True
        End If
        BWStartTCPReader.Dispose()
    End Sub
#End Region

#Region "Anrufmonitor Ereignisse"
    Friend Sub AnrMonRING(ByVal FBStatus As String(), ByVal AnrMonAnzeigen As Boolean, ByVal StoppUhrAnzeigen As Boolean)
        ' wertet einen eingehenden Anruf aus
        ' Parameter: FBStatus (String ()):   Status-String der FritzBox
        '            anzeigen (Boolean):  nur bei 'true' wird 'AnrMonEinblenden' ausgeführt
        ' FBStatus(0): Uhrzeit
        ' FBStatus(1): RING, wird nicht verwendet
        ' FBStatus(2): Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
        ' FBStatus(3): Eingehende Telefonnummer, TelNr
        ' FBStatus(4): Angerufene eigene Telefonnummer, MSN
        ' FBStatus(5): ???

        Dim Vorwahl As String = C_XML.Read("Optionen", "TBVorwahl", "")
        Dim checkstring As String = C_XML.Read("Telefone", "CLBTelNr", ";") ' Enthällt alle MSN, auf die reagiert werden soll

        Dim MSN As String = CStr(FBStatus.GetValue(4))
        ' Anruf nur anzeigen, wenn die MSN stimmt
        If hf.IsOneOf(hf.OrtsVorwahlEntfernen(MSN, Vorwahl), Split(checkstring, ";", , CompareMethod.Text)) Or AnrMonPhoner Then

            Dim TelNr As String            ' ermittelte TelNr
            Dim Anrufer As String = vbNullString           ' ermittelter Anrufer
            Dim vCard As String = vbNullString           ' vCard des Anrufers
            Dim KontaktID As String = vbNullString             ' ID der Kontaktdaten des Anrufers
            Dim StoreID As String = vbNullString           ' ID des Ordners, in dem sich der Kontakt befindet
            Dim ID As Integer            ' ID des Telefonats
            Dim index As Long              ' Zählvariable
            Dim rws As Boolean = False    ' 'true' wenn die Rückwärtssuche erfolgreich war
            Dim LandesVW As String = C_XML.Read("Optionen", "TBLandesVW", "0049")           ' eigene Landesvorwahl
            Dim LetzterAnrufer(5) As String

            ID = CInt(FBStatus.GetValue(2))
            TelNr = CStr(FBStatus.GetValue(3))
            'MSN = CStr(FBStatus.GetValue(4))  'Ist doch schon belegt
            ' Phoner
            If AnrMonPhoner Then
                Dim PhonerTelNr() As String
                Dim pos As Integer = InStr(TelNr, "@", CompareMethod.Text)
                If Not pos = 0 Then
                    TelNr = Left(TelNr, pos - 1)
                Else
                    PhonerTelNr = hf.TelNrTeile(TelNr)
                    If Not PhonerTelNr(1) = "" Then TelNr = PhonerTelNr(1) & Mid(TelNr, InStr(TelNr, ")", CompareMethod.Text) + 2)
                    If Not PhonerTelNr(0) = "" Then TelNr = PhonerTelNr(0) & Mid(TelNr, 2)
                End If
                TelNr = hf.nurZiffern(TelNr, LandesVW)
            End If
            ' Ende Phoner

            If Len(TelNr) = 0 Then TelNr = "unbekannt"
            LetzterAnrufer(0) = CStr(FBStatus.GetValue(0)) 'Zeit
            LetzterAnrufer(1) = Anrufer
            LetzterAnrufer(2) = TelNr
            LetzterAnrufer(3) = MSN
            'LetzterAnrufer(4) = StoreID
            'LetzterAnrufer(5) = KontaktID
            SpeichereLetzerAnrufer(CStr(ID), LetzterAnrufer)
            ' Daten für Anzeige im Anrurfmonitor speichern
            ' Der letzterAnrufer enthält in dieser Reihenfolge Uhrzeit, Anrufername, Telefonnummer, MSN, StoreID, KontaktID
            'C_XML.Write("letzterAnrufer", "letzterAnrufer" & ID, Join(letzterAnrufer, ";"), False)
            'C_XML.Write("letzterAnrufer", "Letzter", CStr(ID), False)
            If AnrMonAnzeigen Then
                If Not OlI.VollBildAnwendungAktiv Then
                    BWAnrMonEinblenden = New BackgroundWorker
                    BWAnrMonEinblenden.RunWorkerAsync(ID)
                End If
            End If

            ' Daten in den Kontakten suchen und per Rückwärtssuche ermitteln
            If Not TelNr = "unbekannt" Then
                Dim FullName As String = vbNullString
                Dim CompanyName As String = vbNullString
                ' Anrufer in den Outlook-Kontakten suchen
                If OlI.StarteKontaktSuche(KontaktID, StoreID, CBool(C_XML.Read("Optionen", "CBKHO", "True") = "True"), TelNr, "", LandesVW) Then
                    OlI.KontaktInformation(KontaktID, StoreID, FullName:=FullName, CompanyName:=CompanyName)
                    Anrufer = Replace(FullName & " (" & CompanyName & ")", " ()", "")
                    If CBool(C_XML.Read("Optionen", "CBIgnoTelNrFormat", "False")) Then TelNr = hf.formatTelNr(TelNr)
                Else
                    ' Anrufer per Rückwärtssuche ermitteln
                    If C_XML.Read("Optionen", "CBRueckwaertssuche", "False") = "True" Then
                        Dim RWSIndex As Boolean = CBool(C_XML.Read("Optionen", "CBRWSIndex", "True"))
                        If RWSIndex Then vCard = C_XML.Read("CBRWSIndex", TelNr, "")
                        If vCard = vbNullString Then
                            Select Case C_XML.Read("Optionen", "CBoxRWSuche", "0")
                                Case "0"
                                    rws = frmRWS.RWS11880(TelNr, vCard)
                                Case "1"
                                    rws = frmRWS.RWSDasTelefonbuch(TelNr, vCard)
                                Case "2"
                                    rws = frmRWS.RWStelsearch(TelNr, vCard)
                                Case "3"
                                    rws = frmRWS.RWSAlle(TelNr, vCard)
                            End Select
                            'Im folgenden wird automatisch ein Kontakt erstellt, der durch die Rückwärtssuche ermittlt wurde. Dies geschieht nur, wenn es gewünscht ist.
                            If rws And C_XML.Read("Optionen", "CBKErstellen", "False") = "True" Then
                                KontaktFunktionen.ErstelleKontakt(KontaktID, StoreID, vCard, TelNr)
                                OlI.KontaktInformation(KontaktID, StoreID, FullName:=FullName, CompanyName:=CompanyName)
                                Anrufer = Replace(FullName & " (" & CompanyName & ")", " ()", "")
                            End If
                        Else
                            rws = True
                            vCard = Replace(vCard, "=0D", Chr(13), , , CompareMethod.Text)
                            vCard = Replace(vCard, "=0A", Chr(10), , , CompareMethod.Text)
                        End If

                        If rws And KontaktID = "-1;" Then
                            Anrufer = ReadFNfromVCard(vCard)
                            Anrufer = Replace(Anrufer, Chr(13), "", , , CompareMethod.Text)
                            If InStr(1, Anrufer, "Firma", CompareMethod.Text) = 1 Then Anrufer = Right(Anrufer, Len(Anrufer) - 5)
                            Anrufer = Trim(Anrufer)
                            vCard = Replace(vCard, Chr(13), "=0D", , , CompareMethod.Text)
                            vCard = Replace(vCard, Chr(10), "=0A", , , CompareMethod.Text)
                            If RWSIndex Then C_XML.Write("CBRWSIndex", hf.nurZiffern(TelNr, LandesVW), vCard, False)
                            KontaktID = "-1" & Anrufer & ";" & vCard
                        End If
                    End If
                    TelNr = hf.formatTelNr(TelNr)
                End If

                LetzterAnrufer(1) = Anrufer
                LetzterAnrufer(2) = TelNr
                LetzterAnrufer(4) = StoreID
                LetzterAnrufer(5) = KontaktID
                'C_XML.Write("letzterAnrufer", "letzterAnrufer" & ID, Join(letzterAnrufer, ";"), True)
                SpeichereLetzerAnrufer(CStr(ID), LetzterAnrufer)
                ' Daten im Menü für Rückruf speichern
                index = CLng(C_XML.Read("AnrListe", "Index", "0"))

                If Not Split(C_XML.Read("AnrListe", "AnrListeEintrag" & Trim(Str((index + 9) Mod 10)), ";"), ";", 5, CompareMethod.Text)(1) = TelNr Then
                    Dim StrArr() As String = {Anrufer, TelNr, FBStatus(0), CStr((index + 1) Mod 10), StoreID, KontaktID}
                    C_XML.Write("AnrListe", "AnrListeEintrag" & index, Join(StrArr, ";"), False)
                    C_XML.Write("AnrListe", "Index", CStr((index + 1) Mod 10), True)
#If OVer < 14 Then
                    If C_XML.Read( "Optionen", "CBSymbAnrListe", "False") = "True" Then GUI.FillPopupItems("AnrListe")
#End If
                End If
            End If
            'StoppUhr
            If StoppUhrAnzeigen Then
                With STUhrDaten(ID)
                    .Richtung = "Anruf von:"
                    If Anrufer = "" Then
                        .Anruf = TelNr
                    Else
                        .Anruf = Anrufer
                    End If
                End With
            End If
            ' Daten für den Journaleintrag sichern
            If C_XML.Read("Optionen", "CBJournal", "False") = "True" Or StoppUhrAnzeigen Then
                NeuerJournalEintrag(ID, "Eingehender Anruf von", CStr(FBStatus.GetValue(0)), MSN, TelNr, KontaktID, StoreID)
            End If
        End If

    End Sub '(AnrMonRING)

    Friend Sub AnrMonCALL(ByVal FBStatus As String(), ByVal StoppUhrAnzeigen As Boolean)
        ' wertet einen ausgehenden Anruf aus
        ' Parameter: FBStatus (String()):  Status-String der FritzBox

        ' FBStatus(0): Uhrzeit
        ' FBStatus(1): CALL, wird nicht verwendet
        ' FBStatus(2): Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
        ' FBStatus(3): Nebenstellennummer, eindeutige Zuordnung des Telefons
        ' FBStatus(4): Ausgehende eigene Telefonnummer, MSN
        ' FBStatus(5): die gewählte Rufnummer

        Dim Vorwahl As String = C_XML.Read("Optionen", "TBVorwahl", "")
        Dim checkstring As String = C_XML.Read("Telefone", "CLBTelNr", "-1")
        Dim MSN As String = CStr(FBStatus.GetValue(4))
        ' Problem DECT/IP-Telefone: keine MSN im über Anrufmonitor eingegangen. Aus Datei ermitteln.
        If MSN = vbNullString Then
            Select Case CInt(FBStatus.GetValue(3))
                Case 10 To 19 'DECT
                    MSN = Split(C_XML.Read("Telefone", CStr(CInt(FBStatus.GetValue(3)) + 50), ";"), ";", , CompareMethod.Text)(0)
                Case 20 To 29 'IP-Telefone (beobachtet bei Wahlregel HandyNr nur per Festnetz, daher erste MSN)
                    MSN = C_XML.Read("Telefone", "MSN0", "")
            End Select
        End If
        ' Anruf nur bearbeiten, wenn die MSN oder VoIP-Nr stimmt
        If hf.IsOneOf(hf.OrtsVorwahlEntfernen(MSN, Vorwahl), Split(checkstring, ";", , CompareMethod.Text)) Or AnrMonPhoner Then
            Dim LandesVW As String = C_XML.Read("Optionen", "TBLandesVW", "0049")           ' eigene Landesvorwahl
            Dim TelNr As String            ' ermittelte TelNr
            Dim Anrufer As String            ' ermittelter Anrufer
            Dim vCard As String = ""          ' vCard des Anrufers
            Dim KontaktID As String = "-1;"         ' ID der Kontaktdaten des Anrufers
            Dim StoreID As String = "-1"          ' ID des Ordners, in dem sich der Kontakt befindet
            Dim ID As Integer = CInt(FBStatus.GetValue(2))     ' ID des Telefonats
            Dim index As Long              ' Zählvariable
            Dim rws As Boolean           ' 'true' wenn die Rückwärtssuche erfolgreich war

            TelNr = hf.nurZiffern(CStr(FBStatus.GetValue(5)), LandesVW)
            If TelNr = "" Then TelNr = "unbekannt"
            ' CbC-Vorwahl entfernen
            If Left(TelNr, 4) = "0100" Then TelNr = Right(TelNr, Len(TelNr) - 6)
            If Left(TelNr, 3) = "010" Then TelNr = Right(TelNr, Len(TelNr) - 5)
            If Not Left(TelNr, 1) = "0" And Not Left(TelNr, 2) = "11" And Not Left(TelNr, 1) = "+" Then _
                TelNr = C_XML.Read("Optionen", "TBVorwahl", "") & TelNr
            ' Raute entfernen
            If Right(TelNr, 1) = "#" Then TelNr = Left(TelNr, Len(TelNr) - 1)
            ' Daten zurücksetzen
            Anrufer = TelNr
            If Not TelNr = "unbekannt" Then
                Dim FullName As String = vbNullString
                Dim CompanyName As String = vbNullString
                ' Anrufer in den Outlook-Kontakten suchen
                If OlI.StarteKontaktSuche(KontaktID, StoreID, CBool(C_XML.Read("Optionen", "CBKHO", "True") = "True"), TelNr, "", LandesVW) Then
                    OlI.KontaktInformation(KontaktID, StoreID, FullName:=FullName, CompanyName:=CompanyName)
                    Anrufer = Replace(FullName & " (" & CompanyName & ")", " ()", "")
                    If CBool(C_XML.Read("Optionen", "CBIgnoTelNrFormat", "False")) Then TelNr = hf.formatTelNr(TelNr)
                Else
                    ' Anrufer per Rückwärtssuche ermitteln
                    If C_XML.Read("Optionen", "CBRueckwaertssuche", "False") = "True" Then
                        Dim RWSIndex As Boolean = CBool(C_XML.Read("Optionen", "CBRWSIndex", "True"))
                        If RWSIndex Then vCard = C_XML.Read("CBRWSIndex", TelNr, "")
                        If vCard = vbNullString Then
                            Select Case C_XML.Read("Optionen", "CBoxRWSuche", "0")
                                'Case "0"
                                '    rws = frmRWS.RWSGoYellow(TelNr, vCard)
                                Case "0"
                                    rws = frmRWS.RWS11880(TelNr, vCard)
                                Case "1"
                                    rws = frmRWS.RWSDasTelefonbuch(TelNr, vCard)
                                Case "2"
                                    rws = frmRWS.RWStelsearch(TelNr, vCard)
                                Case "3"
                                    rws = frmRWS.RWSAlle(TelNr, vCard)
                            End Select
                            'Im folgenden wird automatisch ein Kontakt erstellt, der durch die Rückwärtssuche ermittlt wurde. Dies geschieht nur, wenn es gewünscht ist.
                            If rws And C_XML.Read("Optionen", "CBKErstellen", "False") = "True" Then
                                KontaktFunktionen.ErstelleKontakt(KontaktID, StoreID, vCard, TelNr)
                                OlI.KontaktInformation(KontaktID, StoreID, FullName:=FullName, CompanyName:=CompanyName)
                                Anrufer = Replace(FullName & " (" & CompanyName & ")", " ()", "")
                            End If
                        Else
                            rws = True
                            vCard = Replace(vCard, "=0D", Chr(13), , , CompareMethod.Text)
                            vCard = Replace(vCard, "=0A", Chr(10), , , CompareMethod.Text)
                        End If
                        If rws And KontaktID = "-1;" Then
                            Anrufer = ReadFNfromVCard(vCard)
                            Anrufer = Replace(Anrufer, Chr(13), "", , , CompareMethod.Text)
                            If InStr(1, Anrufer, "Firma", CompareMethod.Text) = 1 Then
                                Anrufer = Right(Anrufer, Len(Anrufer) - 5)
                            End If
                            Anrufer = Trim(Anrufer)
                            vCard = Replace(vCard, Chr(13), "=0D", , , CompareMethod.Text)
                            vCard = Replace(vCard, Chr(10), "=0A", , , CompareMethod.Text)
                            If RWSIndex Then C_XML.Write("CBRWSIndex", hf.nurZiffern(TelNr, LandesVW), vCard, False)
                            KontaktID = "-1" & Anrufer & ";" & vCard
                        End If
                    End If
                    TelNr = hf.formatTelNr(TelNr)
                End If
            End If
            ' Daten im Menü für Wahlwiederholung speichern
            index = CLng(C_XML.Read("Wwdh", "Index", "0"))
            ' Debug.Print(C_XML.Read( "Wwdh", "WwdhEintrag" & Str((index + 9) Mod 10), ";"))
            If Not hf.nurZiffern(Split(C_XML.Read("Wwdh", "WwdhEintrag" & Trim(Str((index + 9) Mod 10)), ";"), ";", 5, CompareMethod.Text)(1), LandesVW) = hf.nurZiffern(TelNr, LandesVW) Then
                Dim StrArr() As String = {Anrufer, TelNr, FBStatus(0), CStr((index + 1) Mod 10), StoreID, KontaktID}
                C_XML.Write("Wwdh", "WwdhEintrag" & index, Join(StrArr, ";"), False)
                C_XML.Write("Wwdh", "Index", CStr((index + 1) Mod 10), True)
#If OVer < 14 Then
                If C_XML.Read( "Optionen", "CBSymbWwdh", "False") = "True" Then GUI.FillPopupItems("Wwdh")
#End If
            End If
            ' AnrMonReStart()
            'StoppUhr
            If StoppUhrAnzeigen Then
                With STUhrDaten(ID)
                    .Richtung = "Anruf zu:"
                    If Anrufer = "" Then
                        .Anruf = TelNr
                    Else
                        .Anruf = Anrufer
                    End If
                End With
            End If
            ' Daten für den Journaleintrag sichern
            If C_XML.Read("Optionen", "CBJournal", "False") = "True" Or StoppUhrAnzeigen Then
                NeuerJournalEintrag(ID, "Ausgehender Anruf zu", CStr(FBStatus.GetValue(0)), MSN, TelNr, KontaktID, StoreID)
                JIÄndern(ID, "NSN", CStr(FBStatus.GetValue(3)))
            End If
        End If
    End Sub '(AnrMonCALL)

    Friend Sub AnrMonCONNECT(ByVal FBStatus As String(), ByVal StoppUhrAnzeigen As Boolean)
        ' wertet eine Zustande gekommene Verbindung aus
        ' Parameter: FBStatus (String()):  Status-String der FritzBox
        If C_XML.Read("Optionen", "CBJournal", "False") = "True" Then
            Dim checkstring As String = C_XML.Read("Telefone", "CLBTelNr", "-1")
            Dim ID As Integer = CInt(FBStatus.GetValue(2))
            Dim MSN As String = JEWertAuslesen(ID, "MSN")
            ' FBStatus(0): Uhrzeit
            ' FBStatus(1): CONNECT, wird nicht verwendet
            ' FBStatus(2): Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
            ' FBStatus(3): Nebenstellennummer, eindeutige Zuordnung des Telefons
            If Not MSN = Nothing Then
                If hf.IsOneOf(hf.OrtsVorwahlEntfernen(MSN, C_XML.Read("Optionen", "TBVorwahl", "")), Split(checkstring, ";", , CompareMethod.Text)) Or AnrMonPhoner Then
                    ' Daten für den Journaleintrag sichern (Beginn des Telefonats)
                    JIÄndern(ID, "NSN", CStr(FBStatus.GetValue(3)))
                    JIÄndern(ID, "Zeit", CStr(FBStatus.GetValue(0)))
                    'StoppUhr
                    If StoppUhrAnzeigen Then
                        BWStoppuhrEinblenden = New BackgroundWorker
                        With BWStoppuhrEinblenden
                            .WorkerSupportsCancellation = True
                            .RunWorkerAsync(ID)
                        End With
                        With STUhrDaten(ID)
                            .MSN = MSN
                            .StartZeit = CStr(System.DateTime.Now)
                            .Abbruch = False
                        End With
                    End If
                Else
                    hf.LogFile("Ein unvollständiges Telefonat wurde registriert.")
                End If
            End If
        End If
    End Sub '(AnrMonCONNECT)

    Friend Sub AnrMonDISCONNECT(ByVal FBStatus As String(), ByVal StoppUhrAnzeigen As Boolean)
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
        ' die zum Anruf gehörende MSN oder VoIP-Nr
        Dim TelName As String
        Dim tmpTelName As String = vbNullString
        Dim TempStat As Integer
        Dim Vorwahl As String = C_XML.Read("Optionen", "TBVorwahl", "")

        Dim NSN As String = vbNullString
        Dim Zeit As String = vbNullString
        Dim Typ As String = vbNullString
        Dim MSN As String = vbNullString
        Dim TelNr As String = vbNullString
        Dim StoreID As String = vbNullString
        Dim KontaktID As String = vbNullString

        Dim FritzFolderExists As Boolean = False
        Dim checkstring As String = C_XML.Read("Telefone", "CLBTelNr", "-1")
        Dim SchließZeit As Date = CDate(C_XML.Read("Journal", "SchließZeit", CStr(System.DateTime.Now)))

        If C_XML.Read("Optionen", "CBJournal", "False") = "True" Then
            JIauslesen(ID, NSN, Zeit, Typ, MSN, TelNr, StoreID, KontaktID)
            Dim JMSN As String = hf.OrtsVorwahlEntfernen(MSN, Vorwahl)
            If Not MSN = Nothing Then
                If hf.IsOneOf(JMSN, Split(checkstring, ";", , CompareMethod.Text)) Or AnrMonPhoner Then

                    'Ist eingespeicherte MSN in der MSN aus FBStatus vorhanden
                    ' Telefonnamen ermitteln
                    If NSN Is Nothing Then NSN = "-1"
                    Select Case CInt(NSN)
                        Case 0 To 2 ' FON1-3
                            TelName = Split(C_XML.Read("Telefone", CStr(CInt(NSN) + 1), ";"), ";", , CompareMethod.Text)(2)
                            'TelName = Mid(TelName, InStr(1, TelName, ";", CompareMethod.Text) + 1)
                        Case 20 To 29 ' LAN/WLAN 
                            TelName = Split(C_XML.Read("Telefone", NSN, ";"), ";", , CompareMethod.Text)(2)
                            'TelName = Mid(TelName, InStr(1, TelName, ";", CompareMethod.Text) + 1)
                        Case 5
                            TelName = "PC-Fax"
                        Case 10 To 19 ' DECT
                            TelName = Split(C_XML.Read("Telefone", CStr(CInt(NSN) + 50), ";"), ";", , CompareMethod.Text)(2)
                            'TelName = Mid(TelName, InStr(1, TelName, ";", CompareMethod.Text) + 1)
                        Case Else  ' S0-Bus
                            TelName = TelefonName(JMSN)
                    End Select

                    ' Journaleintrag schreiben

                    If Dauer = 0 Then
                        Body = "Tel.-Nr.: " & TelNr & vbCrLf & "Status: nicht angenommen" & vbCrLf & vbCrLf
                        If Left(Typ, 3) = "Ein" Then
                            Typ = "Verpasster Anruf von"
                            TempStat = CInt(C_XML.Read("Statistik", "Verpasst", "0"))
                            C_XML.Write("Statistik", "Verpasst", CStr(TempStat + 1), False)
                        Else
                            Typ = "Nicht erfolgreicher Anruf zu"
                            TempStat = CInt(C_XML.Read("Statistik", "Nichterfolgreich", "0"))
                            C_XML.Write("Statistik", "Nichterfolgreich", CStr(TempStat + 1), False)
                        End If
                    Else
                        Body = "Tel.-Nr.: " & TelNr & vbCrLf & "Status: angenommen" & vbCrLf & vbCrLf
                    End If
                    If Dauer > 0 Then
                        If Mid(Typ, 1, 3) = "Ein" Then
                            TempStat = CInt(C_XML.Read("Statistik", "eingehend", "0"))
                            C_XML.Write("Statistik", "eingehend", CStr(TempStat + Dauer), False)
                            TempStat = CInt(C_XML.Read("Statistik", JMSN & "ein", "0"))
                            C_XML.Write("Statistik", JMSN & "ein", CStr(TempStat + Dauer), False)
                        Else
                            TempStat = CInt(C_XML.Read("Statistik", "ausgehend", "0"))
                            C_XML.Write("Statistik", "ausgehend", CStr(TempStat + Dauer), False)
                            TempStat = CInt(C_XML.Read("Statistik", JMSN & "aus", "0"))
                            C_XML.Write("Statistik", JMSN & "aus", CStr(TempStat + Dauer), False)
                        End If
                    End If

                    If Dauer > 0 And Dauer <= 30 Then Dauer = 31

                    If Left(KontaktID, 2) = "-1" Then
                        ' kein Kontakt vorhanden
                        AnrName = Mid(KontaktID, 3, InStr(KontaktID, ";") - 3)
                        If AnrName = "" Then AnrName = TelNr
                        If InStr(1, AnrName, "Firma", CompareMethod.Text) = 1 Then
                            AnrName = Right(AnrName, Len(AnrName) - 5)
                        End If
                        AnrName = Trim(AnrName)
                        vCard = Mid(KontaktID, InStr(KontaktID, ";") + 1)
                        vCard = Replace(vCard, "=0D", Chr(13), , , CompareMethod.Text)
                        vCard = Replace(vCard, "=0A", Chr(10), , , CompareMethod.Text)
                        Firma = ReadFromVCard(vCard, "ORG", "")
                        If Not vCard = "" Then Body = Body & "Kontaktdaten (vCard):" & vbCrLf & vCard & vbCrLf
                    Else
                        ' Kontakt in den 'Links' eintragen
                        Dim FullName As String = vbNullString
                        Dim CompanyName As String = vbNullString
                        Dim HomeAddress As String = vbNullString
                        Dim BusinessAddress As String = vbNullString

                        OlI.KontaktInformation(KontaktID, StoreID, FullName:=FullName, CompanyName:=CompanyName, BusinessAddress:=BusinessAddress, HomeAddress:=HomeAddress)

                        If FullName = "" Then
                            If CompanyName = "" Then
                                AnrName = TelNr
                            Else
                                AnrName = CompanyName
                            End If
                        Else
                            AnrName = FullName
                        End If
                        Firma = CompanyName
                        If Firma = "" Then
                            If Not HomeAddress = "" Then
                                Body = Body & "Kontaktdaten:" & vbCrLf & AnrName _
                                    & vbCrLf & Firma & vbCrLf & HomeAddress & vbCrLf
                            End If
                        Else
                            If Not BusinessAddress = "" Then
                                Body = Body & "Kontaktdaten:" & vbCrLf & AnrName _
                                    & vbCrLf & Firma & vbCrLf & BusinessAddress & vbCrLf
                            End If
                        End If

                    End If

                    ' Prüfe ob TelName angehängt werden soll
                    If Not Split(checkstring, ";", , CompareMethod.Text).Length = 1 Or CInt(C_XML.Read("Telefone", "Anzahl", "1")) > 1 Then
                        tmpTelName = CStr(IIf(Len(TelName) = 0, "", " (" & TelName & ")"))
                    End If

                    'Dim JEintrag As Outlook.JournalItem =
                    OlI.ErstelleJournalItem(Typ & " " & AnrName & CStr(IIf(AnrName = TelNr, vbNullString, " (" & TelNr & ")")) & tmpTelName, _
                                    CInt(Dauer / 60), Body, CDate(Zeit), Firma, TelName & "; FritzBox Anrufmonitor; Telefonanrufe", KontaktID, StoreID)


                    TempStat = CInt(C_XML.Read("Statistik", "Journal", "0"))
                    C_XML.Write("Statistik", "Journal", CStr(TempStat + 1), True)

                    If CDate(Zeit) > SchließZeit Or SchließZeit = System.DateTime.Now Then
                        C_XML.Write("Journal", "SchließZeit", CStr(System.DateTime.Now.AddMinutes(1)), True)
                    End If
                    JEentfernen(ID)
                End If
            Else
                hf.LogFile("AnrMonDISCONNECT: Ein unvollständiges Telefonat wurde registriert.")
                'If Not UsePhonerOhneFritzBox Then
                '    If C_XML.Read( "Optionen", "CBJournal", "False") = "True" And HelferFunktionen.IsOneOf(JMSN, Split(checkstring, ";", , CompareMethod.Text)) Then
                '        ' Wenn Anruf vor dem Outlookstart begonnen wurde, wurde er nicht nachträglich importiert.
                '        Dim ZeitAnruf As Date = CDate(FBStatus(0))
                '        ZeitAnruf = ZeitAnruf.AddSeconds(-1 * (ZeitAnruf.Second + Dauer + 70))
                '        If ZeitAnruf < SchließZeit Then C_XML.Write( "Journal", "SchließZeit", CStr(ZeitAnruf))
                '        HelferFunktionen.LogFile("AnrMonDISCONNECT: Journalimport wird gestartet")
                '        Dim formjournalimort As New formJournalimport( httpTrans, Me, False, C_XML, HelferFunktionen)
                '    End If
                'End If
            End If
        End If

        If StoppUhrAnzeigen Then
            STUhrDaten(ID).Abbruch = True
        End If
    End Sub '(AnrMonDISCONNECT)
#End Region

#Region "Journaleinträge"
    Sub NeuerJournalEintrag(ByVal ID As Integer, _
                            ByVal Typ As String, _
                            ByVal Zeit As String, _
                            ByVal MSN As String, _
                            ByVal TelNr As String, _
                            ByVal KontaktID As String, _
                            ByVal StoreID As String)

        Dim StrArr As New ArrayList
        With StrArr
            .Add("Journal")
            .Add("ID" & ID)
            .Add("Typ")
            C_XML.Write(StrArr, Typ, False)

            .Item(.Count - 1) = "Zeit"
            C_XML.Write(StrArr, Zeit, False)

            .Item(.Count - 1) = "MSN"
            C_XML.Write(StrArr, MSN, False)

            .Item(.Count - 1) = "TelNr"
            C_XML.Write(StrArr, TelNr, False)

            .Item(.Count - 1) = "KontaktID"
            C_XML.Write(StrArr, KontaktID, False)

            .Item(.Count - 1) = "StoreID"
            C_XML.Write(StrArr, StoreID, True)

        End With
        StrArr = Nothing
    End Sub

    Sub JIauslesen(ByVal ID As Integer, _
               ByRef NSN As String, _
               ByRef Zeit As String, _
               ByRef Typ As String, _
               ByRef MSN As String, _
               ByRef TelNr As String, _
               ByRef StoreID As String, _
               ByRef KontaktID As String)

        Dim StrArr As New ArrayList
        With StrArr
            .Add("Journal")
            .Add("ID" & ID)
            .Add("Typ")
            Typ = C_XML.Read(StrArr, "-1")

            .RemoveAt(.Count - 1)
            .Add("Zeit")
            Zeit = C_XML.Read(StrArr, "-1")

            .RemoveAt(.Count - 1)
            .Add("MSN")
            MSN = C_XML.Read(StrArr, "-1")

            .RemoveAt(.Count - 1)
            .Add("NSN")
            NSN = C_XML.Read(StrArr, "-1")

            .RemoveAt(.Count - 1)
            .Add("TelNr")
            TelNr = C_XML.Read(StrArr, "-1")

            .RemoveAt(.Count - 1)
            .Add("KontaktID")
            KontaktID = C_XML.Read(StrArr, "-1")

            .RemoveAt(.Count - 1)
            .Add("StoreID")
            StoreID = C_XML.Read(StrArr, "-1")
        End With

        StrArr = Nothing
    End Sub

    Sub JIÄndern(ByVal ID As Integer, _
                   ByVal Name As String, _
                   ByVal Value As String)

        Dim StrArr As New ArrayList
        With StrArr
            .Add("Journal")
            .Add("ID" & ID)
            .Add(Name)
            C_XML.Write(StrArr, Value, False)
        End With
        StrArr = Nothing
    End Sub

    Function JEWertAuslesen(ByVal ID As Integer, ByVal Name As String) As String

        Dim StrArr As New ArrayList
        With StrArr
            .Add("Journal")
            .Add("ID" & ID)
            .Add(Name)
            Return C_XML.Read(StrArr, "-1")
        End With
        StrArr = Nothing
    End Function

    Sub JEentfernen(ID As Integer)
        Dim StrArr As New ArrayList
        With StrArr
            .Add("Journal")
            .Add("ID" & ID)
            C_XML.Delete(StrArr)
        End With
    End Sub
#End Region

#Region "LetzterAnrufer"
    Sub SpeichereLetzerAnrufer(ByVal ID As String, ByVal LA As String())
        'LA(0) = Zeit
        'LA(1) = Anrufer
        'LA(2) = TelNr
        'LA(3) = MSN
        'LA(4) = StoreID
        'LA(5) = KontaktID
        Dim StrArr As New ArrayList
        With StrArr
            .Add("LetzterAnrufer")
            .Add("Letzter")
            C_XML.Write(StrArr, ID, False)

            .Item(.Count - 1) = "ID" & ID
            .Add("Zeit")
            C_XML.Write(StrArr, LA(0), False)

            .Item(.Count - 1) = "Anrufer"
            If Not LA(1) Is vbNullString Then
                C_XML.Write(StrArr, LA(1), False)
            Else
                C_XML.Delete(StrArr)
            End If

            .Item(.Count - 1) = "TelNr"
            C_XML.Write(StrArr, LA(2), False)

            .Item(.Count - 1) = "MSN"
            C_XML.Write(StrArr, LA(3), False)

            .Item(.Count - 1) = "StoreID"
            If Not LA(4) Is vbNullString Then
                C_XML.Write(StrArr, LA(4), False)
            Else
                C_XML.Delete(StrArr)
            End If

            .Item(.Count - 1) = "KontaktID"
            If Not LA(5) Is vbNullString Then
                C_XML.Write(StrArr, LA(5), True)
            Else
                C_XML.Delete(StrArr)
            End If
        End With
        StrArr = Nothing
    End Sub
#End Region
End Class
