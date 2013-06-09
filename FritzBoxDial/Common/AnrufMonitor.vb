Imports System.Threading
Imports System.Net
Imports System.IO
Imports System.ComponentModel

Public Class AnrufMonitor
    Private WithEvents BWAnrMonEinblenden As BackgroundWorker
    Private WithEvents BWStoppuhrEinblenden As BackgroundWorker
    Private WithEvents TimerReStartStandBy As System.Timers.Timer

    Private ReceiveThread As Thread
    Private AnrMonList As New Collections.ArrayList
    Private Shared Stream As Sockets.NetworkStream
    Private STUhrDaten(5) As StructStoppUhr
    Private GUI As GraphicalUserInterface
    Private OlI As OutlookInterface
    Private KontaktFunktionen As Contacts
    Private ini As InI
    Private hf As Helfer
    Private frmRWS As formRWSuche
    Private frmStopp As formStoppUhr
    Private JExml As JournalXML

    Private StandbyCounter As Integer
    Public AnrMonAktiv As Boolean                    ' damit 'AnrMonAktion' nur einmal aktiv ist
    Public AnrMonError As Boolean
    Private InIPfad As String
    Private TelAnzahl As Integer
    Private UseAnrMon As Boolean
    Private Eingeblendet As Integer = 0

    Public Sub New(ByVal FilePfad As String, ByVal RWS As formRWSuche, ByVal NutzeAnrMon As Boolean, ByVal iniKlasse As InI, ByVal HelferKlasse As Helfer, _
           ByVal KontaktKlasse As Contacts, ByVal InterfacesKlasse As GraphicalUserInterface, ByVal OutlInter As OutlookInterface)
        hf = HelferKlasse
        KontaktFunktionen = KontaktKlasse
        GUI = InterfacesKlasse
        ini = iniKlasse
        frmRWS = RWS
        InIPfad = FilePfad ' InIPfad wird übergeben
        UseAnrMon = NutzeAnrMon
        OlI = OutlInter
        JExml = New JournalXML(hf, InIPfad)
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

    Function StarteTCPReader(ByVal IPAdresse As String, ByVal IPPort As Integer) As Boolean
        System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(500))
        Dim IPAdress As IPAddress
        If LCase(IPAdresse) = LCase("fritz.box") Then
            Dim IPHostInfo As IPHostEntry = Dns.GetHostEntry(IPAdresse)
            IPAdress = IPAddress.Parse(IPHostInfo.AddressList(0).ToString)
        Else
            IPAdress = IPAddress.Parse(IPAdresse)
        End If
        Dim Client As New Sockets.TcpClient()
        Dim remoteEP As New IPEndPoint(IPAdress, IPPort)
        Try
            Client.Connect(remoteEP)
            Stream = Client.GetStream()
            Dim ReceiveThread As New Thread(AddressOf AnrMonAktion)
            ReceiveThread.IsBackground = True
            ReceiveThread.Start()
            AnrMonAktiv = ReceiveThread.IsAlive
            Return AnrMonAktiv
        Catch Err As Exception
            hf.LogFile("TCP Verbindung nicht aufgebaut: " & Err.Message)
            Return False
        End Try
    End Function

    Private Sub AnrMonAktion()
        ' schaut in der FritzBox im Port 1012 nach und startet entsprechende Unterprogramme
        Dim r As New StreamReader(Stream)
        Dim FBStatus As String  ' Status-String der FritzBox
        Dim aktZeile() As String  ' aktuelle Zeile im Status-String
        Dim CBStoppUhrEinblenden As Boolean = CBool(ini.Read(InIPfad, "Optionen", "CBStoppUhrEinblenden", "False"))
        Do
            If Stream.DataAvailable And AnrMonAktiv Then
                FBStatus = r.ReadLine
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
        Dim Erfolgreich As Boolean = False

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
        ' wird beim Start von Outlook ausgeführt und startet den Anrufmonitor
        If (ini.Read(InIPfad, "Optionen", "CBAnrMonAuto", "False") = "True" Or Manuell) And UseAnrMon Then
            Dim Port As Integer = 1012
            Dim IPAddresse As String = ini.Read(InIPfad, "Optionen", "TBFBAdr", "fritz.box")
            If hf.Ping(IPAddresse) Or CBool(ini.Read(InIPfad, "Optionen", "CBForceFBAddr", "False")) Then
                Dim Erfolgreich As Boolean = StarteTCPReader(IPAddresse, Port)
                If Erfolgreich Then
#If OVer < 14 Then
                GUI.SetAnrMonButton(True)
#End If
#If OVer >= 14 Then
                    GUI.InvalidateControlAnrMon()
#End If
                    hf.LogFile("AnrMonStart: Anrufmonitor gestartet")
                    AnrMonAktiv = Erfolgreich
                    AnrMonError = False
                Else
                    hf.LogFile("AnrMonStart: AnrMonAnAus: Es ist ein TCP/IP Fehler aufgetreten.")
                    AnrMonAktiv = False
                    AnrMonError = True
                End If
                Return Erfolgreich
            Else
                AnrMonAktiv = False
                AnrMonError = True
            End If
        End If
        Return True
    End Function '(AnrMonStart)

    Function AnrMonStartNachStandby() As Boolean
        AnrMonStartNachStandby = False
        If ini.Read(InIPfad, "Optionen", "CBAnrMonAuto", "False") = "True" And UseAnrMon Then
            Dim FbIP As String = ini.Read(InIPfad, "Optionen", "TBFBAdr", "192.168.178.1")
            If Not hf.Ping(FbIP) Then
                hf.LogFile("Standby Timer  1. Ping nicht erfolgreich")
                TimerReStartStandBy = hf.SetTimer(2000)
                StandbyCounter = 2
            Else
                hf.LogFile("Standby 1. Ping erfolgreich")
                AnrMonStart(False)
                If ini.Read(InIPfad, "Optionen", "CBJournal", "False") = "True" Then
                    Dim formjournalimort As New formJournalimport(InIPfad, Me, hf, ini, False)
                End If
            End If
            Return True
        End If
    End Function

    Private Sub TimerReStartStandBy_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerReStartStandBy.Elapsed
        Dim FBAdr As String = ini.Read(InIPfad, "Optionen", "TBFBAdr", "fritz.box")
        If hf.Ping(FBAdr) Then
            hf.LogFile("Standby Timer " & StandbyCounter & ". Ping erfolgreich")
            StandbyCounter = 15
            AnrMonStart(False)
            If ini.Read(InIPfad, "Optionen", "CBJournal", "False") = "True" Then
                Dim formjournalimort As New formJournalimport(InIPfad, Me, hf, ini, False)
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

    Public Function TelefonName(ByVal MSN As String) As String

        Dim tempTelName() As String
        Dim Nebenstellen() As String
        Nebenstellen = (From x In Split(ini.Read(InIPfad, "Telefone", "EingerichteteTelefone", "1;2;3;51;52;53;54;55;56;57;58;50;60;61;62;63;64;65;66;67;68;69;20;21;22;23;24;25;26;27;28;29"), ";", , CompareMethod.Text) Where Not x Like "60#" Select x).ToArray ' TAM entfernen
        TelefonName = vbNullString
        For Each Nebenstelle In Nebenstellen
            tempTelName = Split(ini.Read(InIPfad, "Telefone", Nebenstelle, "-1;"), ";", , CompareMethod.Text)
            If Not tempTelName(0) = "-1" Or tempTelName(0) = "" Then
                If hf.IsOneOf(MSN, Split(tempTelName(1), "_", , CompareMethod.Text)) Then
                    TelefonName += tempTelName(2) & ", "
                End If
            End If
        Next
        If Not TelefonName = vbNullString Then TelefonName = Left(TelefonName, Len(TelefonName) - 2)
    End Function

    Private Sub BWAnrMonEinblenden_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BWAnrMonEinblenden.DoWork
        Dim ID As Integer = CInt(e.Argument)
        Dim letzterAnrufer() As String = Split(ini.Read(hf.Dateipfade(InIPfad, "Listen"), "letzterAnrufer", "letzterAnrufer " & ID, CStr(System.DateTime.Now) & ";;unbekannt;;-1;-1;"), ";", 6, CompareMethod.Text)
        AnrMonList.Add(New formAnrMon(InIPfad, CInt(ID), True, ini, hf, Me, OlI))
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
        If CBool(ini.Read(InIPfad, "Optionen", "CBStoppUhrAusblenden", "False")) Then
            WarteZeit = CInt(ini.Read(InIPfad, "Optionen", "TBStoppUhr", "0"))
        Else
            WarteZeit = -1
        End If

        StartPosition = New System.Drawing.Point(CInt(ini.Read(InIPfad, "Optionen", "CBStoppUhrX", "10")), CInt(ini.Read(InIPfad, "Optionen", "CBStoppUhrY", "10")))
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
            ini.Write(InIPfad, "Optionen", "CBStoppUhrX", CStr(frmStUhr.Position.X))
            ini.Write(InIPfad, "Optionen", "CBStoppUhrY", CStr(frmStUhr.Position.Y))
            frmStUhr = Nothing
        End With
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

        Dim Vorwahl As String = ini.Read(InIPfad, "Optionen", "TBVorwahl", "")
        Dim checkstring As String = ini.Read(InIPfad, "Telefone", "CLBTelNr", "-1") ' Enthällt alle MSN, auf die reakiert werden soll
        Dim MSN As String = CStr(FBStatus.GetValue(4))
        If hf.IsOneOf(hf.OrtsVorwahlEntfernen(MSN, Vorwahl), Split(checkstring, ";", , CompareMethod.Text)) Then
            'Dimensionierung in die Abfrage geschoben, um eine unnötige Dimensionierung zu verhindern.
            'Dim olNamespace As Outlook.NameSpace ' MAPI-Namespace
            'Dim olfolder As Outlook.MAPIFolder
            Dim TelNr As String            ' ermittelte TelNr
            Dim Anrufer As String = vbNullString           ' ermittelter Anrufer
            Dim vCard As String = vbNullString           ' vCard des Anrufers
            Dim KontaktID As String = "-1;"           ' ID der Kontaktdaten des Anrufers
            Dim StoreID As String = "-1"           ' ID des Ordners, in dem sich der Kontakt befindet
            Dim ID As Integer            ' ID des Telefonats
            Dim index As Long              ' Zählvariable
            'Dim GefundenerKontakt As Outlook.ContactItem
            Dim rws As Boolean = False    ' 'true' wenn die Rückwärtssuche erfolgreich war
            Dim LandesVW As String = ini.Read(InIPfad, "Optionen", "TBLandesVW", "0049")           ' eigene Landesvorwahl

            Dim IndexDatei As String = hf.Dateipfade(InIPfad, "KontaktIndex")
            Dim Listen As String = hf.Dateipfade(InIPfad, "Listen")

            ' Anruf nur anzeigen, wenn die MSN oder VoIP-Nr stimmt

            ID = CInt(FBStatus.GetValue(2))
            TelNr = CStr(FBStatus.GetValue(3))

            If Len(TelNr) = 0 Then TelNr = "unbekannt"
            MSN = CStr(FBStatus.GetValue(4))
            Dim letzterAnrufer() As String = {CStr(FBStatus.GetValue(0)), Anrufer, TelNr, MSN, StoreID, KontaktID}
            ' Der letzterAnrufer enthält in dieser Reihenfolge Uhrzeit, Anrufername, Telefonnummer, MSN, StoreID, KontaktID
            ini.Write(Listen, "letzterAnrufer", "letzterAnrufer " & ID, Join(letzterAnrufer, ";"))
            ' Daten für Anzeige im Anrurfmonitor speichern
            ini.Write(Listen, "letzterAnrufer", "Letzter", CStr(ID))
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
                If OlI.StarteKontaktSuche(KontaktID, StoreID, CBool(ini.Read(InIPfad, "Optionen", "CBKHO", "True") = "True"), TelNr, "", LandesVW) Then
                    OlI.KontaktInformation(KontaktID, StoreID, FullName:=FullName, CompanyName:=CompanyName)
                    Anrufer = Replace(FullName & " (" & CompanyName & ")", " ()", "")
                    If CBool(ini.Read(InIPfad, "Optionen", "CBIgnoTelNrFormat", "False")) Then TelNr = hf.formatTelNr(TelNr)
                Else
                    ' Anrufer per Rückwärtssuche ermitteln
                    If ini.Read(InIPfad, "Optionen", "CBRückwärtssuche", "False") = "True" Then
                        Dim RWSIndex As Boolean = CBool(ini.Read(InIPfad, "Optionen", "CBRWSIndex", "True"))
                        If RWSIndex Then vCard = ini.Read(IndexDatei, "CBRWSIndex", TelNr, "")
                        If vCard = vbNullString Then
                            Select Case ini.Read(InIPfad, "Optionen", "CBoxRWSuche", "0")
                                Case "0"
                                    rws = frmRWS.RWSGoYellow(TelNr, vCard)
                                Case "1"
                                    rws = frmRWS.RWS11880(TelNr, vCard)
                                Case "2"
                                    rws = frmRWS.RWSDasTelefonbuch(TelNr, vCard)
                                Case "3"
                                    rws = frmRWS.RWStelsearch(TelNr, vCard)
                                Case "4"
                                    rws = frmRWS.RWSAlle(TelNr, vCard)
                            End Select
                            'Im folgenden wird automatisch ein Kontakt erstellt, der durch die Rückwärtssuche ermittlt wurde. Dies geschieht nur, wenn es gewünscht ist.
                            If rws And ini.Read(InIPfad, "Optionen", "CBKErstellen", "False") = "True" Then
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
                            If RWSIndex Then ini.Write(IndexDatei, "CBRWSIndex", hf.nurZiffern(TelNr, LandesVW), vCard)
                            KontaktID = "-1" & Anrufer & ";" & vCard
                        End If
                    End If
                    TelNr = hf.formatTelNr(TelNr)
                End If

                letzterAnrufer(1) = Anrufer
                letzterAnrufer(2) = TelNr
                letzterAnrufer(5) = KontaktID
                letzterAnrufer(4) = StoreID
                ini.Write(Listen, "letzterAnrufer", "letzterAnrufer " & ID, Join(letzterAnrufer, ";"))

                ' Daten im Menü für Rückruf speichern
                index = CLng(ini.Read(Listen, "AnrListe", "Index", "0"))

                If Not Split(ini.Read(Listen, "AnrListe", "AnrListeEintrag" & Str((index + 9) Mod 10), ";"), ";", 5, CompareMethod.Text)(1) = TelNr Then
                    Dim StrArr() As String = {Anrufer, TelNr, FBStatus(0), CStr((index + 1) Mod 10), StoreID, KontaktID}
                    ini.Write(Listen, "AnrListe", "AnrListeEintrag " & index, Join(StrArr, ";"))
                    ini.Write(Listen, "AnrListe", "Index", CStr((index + 1) Mod 10))
#If OVer < 14 Then
                    If ini.Read(InIPfad, "Optionen", "CBSymbAnrListe", "False") = "True" Then GUI.FillPopupItems("AnrListe")
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
            If ini.Read(InIPfad, "Optionen", "CBJournal", "False") = "True" Or StoppUhrAnzeigen Then
                JExml.NeuerJI(ID, "Eingehender Anruf von", CStr(FBStatus.GetValue(0)), MSN, TelNr, KontaktID, StoreID)
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

        Dim Vorwahl As String = ini.Read(InIPfad, "Optionen", "TBVorwahl", "")
        Dim checkstring As String = ini.Read(InIPfad, "Telefone", "CLBTelNr", "-1")
        Dim MSN As String = CStr(FBStatus.GetValue(4))
        ' Problem DECT/IP-Telefone: keine MSN im über Anrufmonitor eingegangen. Aus Datei ermitteln.
        If MSN = vbNullString Then
            Select Case CInt(FBStatus.GetValue(3))
                Case 10 To 19 'DECT
                    MSN = Split(ini.Read(InIPfad, "Telefone", CStr(CInt(FBStatus.GetValue(3)) + 50), ";"), ";", , CompareMethod.Text)(0)
                Case 20 To 29 'IP-Telefone (beobachtet bei Wahlregel HandyNr nur per Festnetz, daher erste MSN)
                    MSN = ini.Read(InIPfad, "Telefone", "MSN0", "")
            End Select
        End If
        ' Anruf nur bearbeiten, wenn die MSN oder VoIP-Nr stimmt
        If hf.IsOneOf(hf.OrtsVorwahlEntfernen(MSN, Vorwahl), Split(checkstring, ";", , CompareMethod.Text)) Then
            Dim LandesVW As String = ini.Read(InIPfad, "Optionen", "TBLandesVW", "0049")           ' eigene Landesvorwahl
            Dim TelNr As String            ' ermittelte TelNr
            Dim Anrufer As String            ' ermittelter Anrufer
            Dim vCard As String = ""          ' vCard des Anrufers
            Dim KontaktID As String = "-1;"         ' ID der Kontaktdaten des Anrufers
            Dim StoreID As String = "-1"          ' ID des Ordners, in dem sich der Kontakt befindet
            Dim ID As Integer = CInt(FBStatus.GetValue(2))     ' ID des Telefonats
            Dim index As Long              ' Zählvariable
            Dim rws As Boolean           ' 'true' wenn die Rückwärtssuche erfolgreich war

            Dim IndexDatei = hf.Dateipfade(InIPfad, "KontaktIndex")
            Dim Listen As String = hf.Dateipfade(InIPfad, "Listen")

            TelNr = hf.nurZiffern(CStr(FBStatus.GetValue(5)), LandesVW)
            If TelNr = "" Then TelNr = "unbekannt"
            ' CbC-Vorwahl entfernen
            If Left(TelNr, 4) = "0100" Then TelNr = Right(TelNr, Len(TelNr) - 6)
            If Left(TelNr, 3) = "010" Then TelNr = Right(TelNr, Len(TelNr) - 5)
            If Not Left(TelNr, 1) = "0" And Not Left(TelNr, 2) = "11" And Not Left(TelNr, 1) = "+" Then _
                TelNr = ini.Read(InIPfad, "Optionen", "TBVorwahl", "") & TelNr
            ' Raute entfernen
            If Right(TelNr, 1) = "#" Then TelNr = Left(TelNr, Len(TelNr) - 1)
            ' Daten zurücksetzen
            Anrufer = TelNr
            If Not TelNr = "unbekannt" Then
                Dim FullName As String = vbNullString
                Dim CompanyName As String = vbNullString
                ' Anrufer in den Outlook-Kontakten suchen
                If OlI.StarteKontaktSuche(KontaktID, StoreID, CBool(ini.Read(InIPfad, "Optionen", "CBKHO", "True") = "True"), TelNr, "", LandesVW) Then
                    OlI.KontaktInformation(KontaktID, StoreID, FullName:=FullName, CompanyName:=CompanyName)
                    Anrufer = Replace(FullName & " (" & CompanyName & ")", " ()", "")
                    If CBool(ini.Read(InIPfad, "Optionen", "CBIgnoTelNrFormat", "False")) Then TelNr = hf.formatTelNr(TelNr)
                Else
                    ' Anrufer per Rückwärtssuche ermitteln
                    If ini.Read(InIPfad, "Optionen", "CBRückwärtssuche", "False") = "True" Then
                        Dim RWSIndex As Boolean = CBool(ini.Read(InIPfad, "Optionen", "CBRWSIndex", "True"))
                        If RWSIndex Then vCard = ini.Read(IndexDatei, "CBRWSIndex", TelNr, "")
                        If vCard = vbNullString Then
                            Select Case ini.Read(InIPfad, "Optionen", "CBoxRWSuche", "0")
                                Case "0"
                                    rws = frmRWS.RWSGoYellow(TelNr, vCard)
                                Case "1"
                                    rws = frmRWS.RWS11880(TelNr, vCard)
                                Case "2"
                                    rws = frmRWS.RWSDasTelefonbuch(TelNr, vCard)
                                Case "3"
                                    rws = frmRWS.RWStelsearch(TelNr, vCard)
                                Case "4"
                                    rws = frmRWS.RWSAlle(TelNr, vCard)
                            End Select
                            'Im folgenden wird automatisch ein Kontakt erstellt, der durch die Rückwärtssuche ermittlt wurde. Dies geschieht nur, wenn es gewünscht ist.
                            If rws And ini.Read(InIPfad, "Optionen", "CBKErstellen", "False") = "True" Then
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
                            If RWSIndex Then
                                ini.Write(IndexDatei, "CBRWSIndex", hf.nurZiffern(TelNr, LandesVW), vCard)
                            End If
                            KontaktID = "-1" & Anrufer & ";" & vCard
                        End If
                    End If
                    TelNr = hf.formatTelNr(TelNr)
                End If
            End If
            ' Daten im Menü für Wahlwiederholung speichern
            index = CLng(ini.Read(Listen, "Wwdh", "Index", "0"))
            ' Debug.Print(ini.Read(Listen, "Wwdh", "WwdhEintrag" & Str((index + 9) Mod 10), ";"))
            If Not hf.nurZiffern(Split(ini.Read(Listen, "Wwdh", "WwdhEintrag" & Str((index + 9) Mod 10), ";"), ";", 5, CompareMethod.Text)(1), LandesVW) = hf.nurZiffern(TelNr, LandesVW) Then
                Dim StrArr() As String = {Anrufer, TelNr, FBStatus(0), CStr((index + 1) Mod 10), StoreID, KontaktID}
                ini.Write(Listen, "Wwdh", "WwdhEintrag " & index, Join(StrArr, ";"))
                ini.Write(Listen, "Wwdh", "Index", CStr((index + 1) Mod 10))
#If OVer < 14 Then
                If ini.Read(InIPfad, "Optionen", "CBSymbWwdh", "False") = "True" Then GUI.FillPopupItems("Wwdh")
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
            If ini.Read(InIPfad, "Optionen", "CBJournal", "False") = "True" Or StoppUhrAnzeigen Then
                JExml.NeuerJI(ID, "Ausgehender Anruf zu", CStr(FBStatus.GetValue(0)), MSN, TelNr, KontaktID, StoreID)
                JExml.ZuJEhinzufügen(ID, "NSN", CStr(FBStatus.GetValue(3)))
            End If
        End If
    End Sub '(AnrMonCALL)

    Friend Sub AnrMonCONNECT(ByVal FBStatus As String(), ByVal StoppUhrAnzeigen As Boolean)
        ' wertet eine Zustande gekommene Verbindung aus
        ' Parameter: FBStatus (String()):  Status-String der FritzBox
        If ini.Read(InIPfad, "Optionen", "CBJournal", "False") = "True" Then
            Dim checkstring As String = ini.Read(InIPfad, "Telefone", "CLBTelNr", "-1")
            Dim ID As Integer = CInt(FBStatus.GetValue(2))
            Dim MSN As String = JExml.JEWertAuslesen(ID, "MSN")
            ' FBStatus(0): Uhrzeit
            ' FBStatus(1): CONNECT, wird nicht verwendet
            ' FBStatus(2): Die Nummer der aktuell aufgebauten Verbindungen (0 ... n), dient zur Zuordnung der Telefonate, ID
            ' FBStatus(3): Nebenstellennummer, eindeutige Zuordnung des Telefons
            If Not MSN = Nothing Then
                If hf.IsOneOf(hf.OrtsVorwahlEntfernen(MSN, ini.Read(InIPfad, "Optionen", "TBVorwahl", "")), Split(checkstring, ";", , CompareMethod.Text)) Then
                    ' Daten für den Journaleintrag sichern (Beginn des Telefonats)
                    With JExml
                        If .JEWertAuslesen(ID, "NSN") = vbNullString Then
                            .ZuJEhinzufügen(ID, "NSN", CStr(FBStatus.GetValue(3)))
                        Else
                            .JIÄndern(ID, "NSN", CStr(FBStatus.GetValue(3)))
                        End If
                        .ZuJEhinzufügen(ID, "Zeit", CStr(FBStatus.GetValue(0)))
                    End With
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
        Dim Vorwahl As String = ini.Read(InIPfad, "Optionen", "TBVorwahl", "")

        Dim NSN As String = vbNullString
        Dim Zeit As String = vbNullString
        Dim Typ As String = vbNullString
        Dim MSN As String = vbNullString
        Dim TelNr As String = vbNullString
        Dim StoreID As String = vbNullString
        Dim KontaktID As String = vbNullString

        Dim FritzFolderExists As Boolean = False
        Dim checkstring As String = ini.Read(InIPfad, "Telefone", "CLBTelNr", "-1")
        Dim SchließZeit As Date = CDate(ini.Read(InIPfad, "Journal", "SchließZeit", CStr(System.DateTime.Now)))

        If ini.Read(InIPfad, "Optionen", "CBJournal", "False") = "True" Then
            JExml.JIauslesen(ID, NSN, Zeit, Typ, MSN, TelNr, StoreID, KontaktID)
            Dim JMSN As String = hf.OrtsVorwahlEntfernen(MSN, Vorwahl)
            If Not MSN = Nothing Then
                If hf.IsOneOf(JMSN, Split(checkstring, ";", , CompareMethod.Text)) Then

                    'Ist eingespeicherte MSN in der MSN aus FBStatus vorhanden
                    ' Telefonnamen ermitteln
                    If NSN Is Nothing Then NSN = "-1"
                    Select Case CInt(NSN)
                        Case 0 To 2 ' FON1-3
                            TelName = Split(ini.Read(InIPfad, "Telefone", CStr(CInt(NSN) + 1), ";"), ";", , CompareMethod.Text)(2)
                            'TelName = Mid(TelName, InStr(1, TelName, ";", CompareMethod.Text) + 1)
                        Case 20 To 29 ' LAN/WLAN 
                            TelName = Split(ini.Read(InIPfad, "Telefone", NSN, ";"), ";", , CompareMethod.Text)(2)
                            'TelName = Mid(TelName, InStr(1, TelName, ";", CompareMethod.Text) + 1)
                        Case 5
                            TelName = "PC-Fax"
                        Case 10 To 19 ' DECT
                            TelName = Split(ini.Read(InIPfad, "Telefone", CStr(CInt(NSN) + 50), ";"), ";", , CompareMethod.Text)(2)
                            'TelName = Mid(TelName, InStr(1, TelName, ";", CompareMethod.Text) + 1)
                        Case Else  ' S0-Bus
                            TelName = TelefonName(JMSN)
                    End Select

                    ' Journaleintrag schreiben

                    If Dauer = 0 Then
                        Body = "Tel.-Nr.: " & TelNr & vbCrLf & "Status: nicht angenommen" & vbCrLf & vbCrLf
                        If Left(Typ, 3) = "Ein" Then
                            Typ = "Verpasster Anruf von"
                            TempStat = CInt(ini.Read(InIPfad, "Statistik", "Verpasst", "0"))
                            ini.Write(InIPfad, "Statistik", "Verpasst", CStr(TempStat + 1))
                        Else
                            Typ = "Nicht erfolgreicher Anruf zu"
                            TempStat = CInt(ini.Read(InIPfad, "Statistik", "Nichterfolgreich", "0"))
                            ini.Write(InIPfad, "Statistik", "Nichterfolgreich", CStr(TempStat + 1))
                        End If
                    Else
                        Body = "Tel.-Nr.: " & TelNr & vbCrLf & "Status: angenommen" & vbCrLf & vbCrLf
                    End If
                    If Dauer > 0 Then
                        If Mid(Typ, 1, 3) = "Ein" Then
                            TempStat = CInt(ini.Read(InIPfad, "Statistik", "eingehend", "0"))
                            ini.Write(InIPfad, "Statistik", "eingehend", CStr(TempStat + Dauer))
                            TempStat = CInt(ini.Read(InIPfad, "Statistik", JMSN & "ein", "0"))
                            ini.Write(InIPfad, "Statistik", JMSN & "ein", CStr(TempStat + Dauer))
                        Else
                            TempStat = CInt(ini.Read(InIPfad, "Statistik", "ausgehend", "0"))
                            ini.Write(InIPfad, "Statistik", "ausgehend", CStr(TempStat + Dauer))
                            TempStat = CInt(ini.Read(InIPfad, "Statistik", JMSN & "aus", "0"))
                            ini.Write(InIPfad, "Statistik", JMSN & "aus", CStr(TempStat + Dauer))
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
                    If Not Split(checkstring, ";", , CompareMethod.Text).Length = 1 Or CInt(ini.Read(InIPfad, "Telefone", "Anzahl", "1")) > 1 Then
                        tmpTelName = CStr(IIf(Len(TelName) = 0, "", " (" & TelName & ")"))
                    End If

                    'Dim JEintrag As Outlook.JournalItem =
                    OlI.ErstelleJournalItem(Typ & " " & AnrName & CStr(IIf(AnrName = TelNr, vbNullString, " (" & TelNr & ")")) & tmpTelName, _
                                    CInt(Dauer / 60), Body, CDate(Zeit), Firma, TelName & "; FritzBox Anrufmonitor; Telefonanrufe", KontaktID, StoreID)

                    'JEintrag.Close(Microsoft.Office.Interop.Outlook.OlInspectorClose.olSave)

                    TempStat = CInt(ini.Read(InIPfad, "Statistik", "Journal", "0"))
                    ini.Write(InIPfad, "Statistik", "Journal", CStr(TempStat + 1))

                    If CDate(Zeit) > SchließZeit Or SchließZeit = System.DateTime.Now Then
                        ini.Write(InIPfad, "Journal", "SchließZeit", CStr(System.DateTime.Now.AddMinutes(1)))
                    End If
                    ' AnrMonReStart()
                    JExml.JEentfernen(ID)
                End If
            Else
                hf.LogFile("AnrMonDISCONNECT: Ein unvollständiges Telefonat wurde registriert.")
            End If
        End If

        If StoppUhrAnzeigen Then
            STUhrDaten(ID).Abbruch = True
        End If
    End Sub '(AnrMonDISCONNECT)
#End Region

    
End Class
