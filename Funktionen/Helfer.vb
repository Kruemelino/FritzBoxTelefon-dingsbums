Imports System.Net
Imports System.Text

Public Class Helfer

    Private C_DP As DataProvider
    Private C_Crypt As Rijndael

    Private sDateiPfad As String

    Public Sub New(ByVal ArbeitsPfad As String, ByVal DataProviderKlasse As DataProvider, ByVal CryptKlasse As Rijndael)
        sDateiPfad = ArbeitsPfad
        C_DP = DataProviderKlasse
        C_Crypt = CryptKlasse
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

#Region " String Behandlung"

    Public Function StringEntnehmen(Text As String, StringDavor As String, StringDanach As String, Optional Reverse As Boolean = False) As String
        Dim pos(1) As Integer
        If Not Reverse Then
            pos(0) = InStr(1, Text, StringDavor, CompareMethod.Text) + Len(StringDavor)
            pos(1) = InStr(pos(0), Text, StringDanach, CompareMethod.Text)
        Else
            pos(1) = InStrRev(Text, StringDanach, , CompareMethod.Text)
            pos(0) = InStrRev(Text, StringDavor, pos(1), CompareMethod.Text) + Len(StringDavor)
        End If
        If Not pos(0) = Len(StringDavor) Then
            StringEntnehmen = Mid(Text, pos(0), pos(1) - pos(0))
        Else
            StringEntnehmen = "-1"
        End If
    End Function

    Public Function IsOneOf(ByVal A As String, ByVal B() As String) As Boolean
        Return CBool(IIf((From Strng In B Where Strng = A).ToArray.Count = 0, False, True))
    End Function

#End Region

    Public Sub NAR(ByVal o As Object)
        If Not o Is Nothing Then
            Try
                System.Runtime.InteropServices.Marshal.ReleaseComObject(o)
            Catch ex As Exception
                FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "NAR")
            Finally
                o = Nothing
            End Try
        End If
    End Sub

    Public Function Ping(ByRef IPAdresse As String) As Boolean
        Ping = False

        Dim PingSender As New NetworkInformation.Ping()
        Dim Options As New NetworkInformation.PingOptions()
        Dim PingReply As NetworkInformation.PingReply = Nothing
        Dim data As String = C_DP.P_Def_StringEmpty

        Dim buffer As Byte() = Encoding.ASCII.GetBytes(data)
        Dim timeout As Integer = 120

        Options.DontFragment = True
        Try
            PingReply = PingSender.Send(IPAdresse, timeout, buffer, Options)
        Catch ex As Exception
            LogFile("Ping zu """ & IPAdresse & """ nicht erfolgreich: " & ex.InnerException.Message)
            Ping = False
        End Try

        If Not PingReply Is Nothing Then
            With PingReply
                If .Status = NetworkInformation.IPStatus.Success Then
                    IPAdresse = .Address.ToString
                    Ping = True
                Else
                    Ping = False
                End If
            End With
        End If
        PingSender.Dispose()
        Options = Nothing
        PingSender = Nothing
    End Function

    ''' <summary>
    ''' Wandelt die eingegebene IP-Adresse in eine für dieses Addin gültige IPAdresse.
    ''' IPv4 und IPv6 müssen differenziert behandelt werden.
    ''' Für Anrufmonitor ist es egal ob IPv4 oder IPv6 da der RemoteEndPoint ein IPAddress-Objekt verwendet.
    ''' Die HTML/URL müssen gesondert beachtet werden. Dafün muss die IPv6 in eckige Klammern gesetzt werden.
    ''' </summary>
    ''' <param name="InputIP">IP-Adresse</param>
    ''' <param name="ValidIpv6URL">Gibt an, ob die IPv6 Adresse in ein URL-Konformen String gewandelt werden soll.</param>
    ''' <returns>Korrekte IP-Adresse</returns>
    Public Function ValidIP(ByVal InputIP As String) As String
        Dim _IPAddress As IPAddress = Nothing
        Dim IPHostInfo As IPHostEntry
        If IPAddress.TryParse(InputIP, _IPAddress) Then
            Select Case _IPAddress.AddressFamily
                Case Sockets.AddressFamily.InterNetworkV6
                    ValidIP = "[" & _IPAddress.ToString & "]"
                Case Sockets.AddressFamily.InterNetwork
                    ValidIP = _IPAddress.ToString
                Case Else
                    LogFile("Die IP """ & InputIP & """ kann nicht zugeordnet werden.")
                    ValidIP = InputIP
            End Select
        Else
            IPHostInfo = Dns.GetHostEntry(C_DP.P_TBFBAdr)
            _IPAddress = IPAddress.Parse(IPHostInfo.AddressList(0).ToString)
            ' Keine gültige IP-Adresse

            ValidIP = ValidIP(_IPAddress.ToString)
        End If
    End Function

    Public Function LogFile(ByVal Meldung As String) As Boolean
        Dim LogDatei As String = Dateipfade("LogDatei")
        If C_DP.P_CBLogFile Then
            With My.Computer.FileSystem
                If .FileExists(LogDatei) Then
                    If .GetFileInfo(LogDatei).Length > 1048576 Then .DeleteFile(LogDatei)
                End If
                Try
                    .WriteAllText(LogDatei, Date.Now & " - " & Meldung & vbNewLine, True)
                Catch : End Try
            End With
        End If
        Return True
    End Function

    Public Function GetEncoding(ByVal Encoding As String) As System.Text.Encoding
        Select Case LCase(Encoding)
            Case "utf-8"
                Return System.Text.Encoding.UTF8
            Case Else
                Return System.Text.Encoding.Default
        End Select
    End Function

    Public Function FBDB_MsgBox(ByVal Meldung As String, ByVal Style As MsgBoxStyle, ByVal Aufruf As String) As MsgBoxResult
        If Style = MsgBoxStyle.Critical Or Style = MsgBoxStyle.Exclamation Then
            Meldung = "Die Funktion " & Aufruf & " meldet folgenden Fehler:" & vbCrLf & vbCrLf & Meldung
            LogFile(Meldung)
        End If
        Return MsgBox(Meldung, Style, "Fritz!Box Telefon-Dingsbums")
    End Function

    Public Function Dateipfade(ByVal Datei As String) As String
        Dim XMLDateiPfad As String = C_DP.GetXMLDateiPfad

        Select Case Datei
            Case "LogDatei"
                Datei = "FBDB.log"
            Case "AnrListe"
                Datei = "AnrListe.csv"
            Case "JournalXML"
                Datei = "Journal.xml"
            Case "FritzOutlookXML"
                Datei = "FritzOutlook.xml"
        End Select

        Return Left(XMLDateiPfad, InStrRev(XMLDateiPfad, "\", , CompareMethod.Text)) & Datei
    End Function
    ''' <summary>
    ''' Diese Routine ändert den Zugang zu den verschlüsselten Passwort.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub KeyChange()
        Dim tempZugang As String
        Dim i As Long

        If Not C_DP.P_TBPasswort = C_DP.P_Def_StringEmpty Then
            tempZugang = C_DP.P_Def_StringEmpty
            For i = 0 To 2
                tempZugang = tempZugang & Hex(Rnd() * 255)
            Next
            tempZugang = C_Crypt.getMd5Hash(tempZugang, Encoding.Unicode)
            C_DP.P_TBPasswort = C_Crypt.EncryptString128Bit(C_Crypt.DecryptString128Bit(C_DP.P_TBPasswort, GetSetting("FritzBox", "Optionen", "Zugang", "-1")), tempZugang)
            SaveSetting("Fritzbox", "Optionen", "Zugang", tempZugang)
        End If

        If Not C_DP.P_TBPhonerPasswort = C_DP.P_Def_StringEmpty Then
            tempZugang = C_DP.P_Def_StringEmpty
            For i = 0 To 2
                tempZugang = tempZugang & Hex(Rnd() * 255)
            Next
            tempZugang = C_Crypt.getMd5Hash(tempZugang, Encoding.Unicode)
            C_DP.P_TBPhonerPasswort = C_Crypt.EncryptString128Bit(C_Crypt.DecryptString128Bit(C_DP.P_TBPhonerPasswort, GetSetting("FritzBox", "Optionen", "ZugangPasswortPhoner", "-1")), tempZugang)
            SaveSetting("Fritzbox", "Optionen", "ZugangPasswortPhoner", tempZugang)
        End If

        C_DP.SpeichereXMLDatei()

    End Sub ' (Keyänderung) 

    Public Function GetInformationSystemFritzBox(ByVal FBAdr As String) As String
        Dim sLink As String
        Dim FBTyp As String = C_DP.P_Def_StringUnknown
        Dim FBFW As String = C_DP.P_Def_StringUnknown

        If LCase(FBAdr) = C_DP.P_Def_FritzBoxAdress Then Ping(FBAdr)

        sLink = "http://" & FBAdr & "/cgi-bin/system_status"
        Dim FritzBoxInformation() As String = Split(StringEntnehmen(httpGET(sLink, System.Text.Encoding.UTF8, Nothing), "<body>", "</body>"), "-", , CompareMethod.Text)
        FBTyp = FritzBoxInformation(0)
        FBFW = Replace(Trim(GruppiereNummer(FritzBoxInformation(7))), " ", ".", , , CompareMethod.Text)

        Return String.Concat("Ergänze bitte folgende Angaben:", vbNewLine, vbNewLine, _
                             "Dein Name:", vbNewLine, _
                             "Problembeschreibung:", vbNewLine, _
                             "Datum & Uhrzeit: ", System.DateTime.Now, vbNewLine, _
                             "Fritz!Box-Typ: ", FBTyp, vbNewLine, _
                             "Firmware: ", FBFW, vbNewLine)

    End Function

#Region " Telefonnummern formatieren"
    ''' <summary>
    ''' Formatiert die Telefonnummern nach gängigen Regelm
    ''' </summary>
    ''' <param name="TelNr">Die zu formatierende Telefonnummer</param>
    ''' <returns>Die formatierte Telefonnummer</returns>
    ''' <remarks></remarks>
    Function formatTelNr(ByVal TelNr As String) As String
        ' formatiert die Telefonnummer in der Form "(0# ##) # ## ## ##" bzw. "+## (# ##) # ## ## ##"
        ' Parameter:  TelNr (String):     zu formatierende Telefonnummer
        '             OrtsVW (String):    Ortsvorwahl der Telefonnummer
        '             LandesVW (String):  Landesvorwahl der Telefonnummer
        '             intl (Boolean):     wenn 'true' dann wird die internationale Form verwendet.
        ' Rückgabewert (String):          formatierte Telefonnummer

        Dim RufNr As String ' Telefonnummer ohne Vorwahl

        Dim LandesVW As String
        Dim OrtsVW As String
        Dim Durchwahl As String
        Dim posOrtsVW As Integer   ' Position der Vorwahl in TelNr
        Dim posDurchwahl As Integer   ' Position der Durchwahl in TelNr
        Dim tempOrtsVW As String = String.Empty ' Hilfsstring für OrtsVW
        Dim tempRufNr As String = String.Empty ' Hilfsstring für RufNr
        Dim tempDurchwahl As String = String.Empty ' Hilfsstring für LandesVW
        Dim TelTeile() As String = TelNrTeile(TelNr)
        Dim Maske As String = C_DP.P_TBTelNrMaske

        LandesVW = TelTeile(0)
        OrtsVW = TelTeile(1)
        Durchwahl = TelTeile(2)

        TelNr = nurZiffern(TelNr, LandesVW)
        If Not OrtsVW = C_DP.P_Def_StringEmpty Then
            posOrtsVW = InStr(TelNr, OrtsVW, CompareMethod.Text)
            RufNr = Mid(TelNr, posOrtsVW + Len(OrtsVW))
            If Not LandesVW = "0039" Then RufNr = CStr(IIf(Left(RufNr, 1) = "0", Mid(RufNr, 2), RufNr))
        Else
            RufNr = TelNr
            If LandesVW = "0039" Then Durchwahl = CStr(IIf(Left(Durchwahl, 1) = "0", Durchwahl, "0" & Durchwahl))
        End If

        ' nur ausführen, wenn die Ortsvorwahl in der Telefonnummer enthalten ist
        ' LandesVW und RufNr aus TelNr separieren

        posDurchwahl = InStr(1, RufNr, Durchwahl, CompareMethod.Text)
        If posDurchwahl = 1 And Not Durchwahl = C_DP.P_Def_StringEmpty Then
            tempDurchwahl = Mid(RufNr, Len(Durchwahl) + 1)
            RufNr = Durchwahl
        Else
            Durchwahl = C_DP.P_Def_StringEmpty
        End If
        If LandesVW = "0" Then
            OrtsVW = "0" & OrtsVW
            LandesVW = C_DP.P_Def_StringEmpty
        End If
        ' Maske Prüfen
        If InStr(Maske, "%D", CompareMethod.Text) = 0 Then Maske = Replace(Maske, "%N", "%N%D")
        If Not InStr(Maske, "%N%D", CompareMethod.Text) = 0 Then
            RufNr = RufNr & tempDurchwahl
            tempDurchwahl = C_DP.P_Def_StringEmpty
        End If

        If OrtsVW = C_DP.P_Def_StringEmpty Then
            ' Keine Ortsvorwahl: Alles zwischen %L und %N entfernen
            Dim pos1 As Integer
            Dim pos2 As Integer
            Dim CutOut As String
            pos1 = InStr(Maske, "%L", CompareMethod.Text) + 2
            pos2 = InStr(Maske, "%N", CompareMethod.Text)
            CutOut = Mid(Maske, pos1, pos2 - pos1)
            Maske = Replace(Maske, CutOut, CStr(IIf(Left(CutOut, 1) = " ", " ", C_DP.P_Def_StringEmpty)), , 1, CompareMethod.Text)
        End If
        If LandesVW = C_DP.P_Def_StringEmpty Then LandesVW = C_DP.P_TBLandesVW
        If C_DP.P_CBintl Or Not LandesVW = C_DP.P_TBLandesVW Then
            If Not OrtsVW = C_DP.P_Def_StringEmpty Then
                If Left(OrtsVW, 1) = "0" Then OrtsVW = Mid(OrtsVW, 2)
                OrtsVW = CStr(IIf(LandesVW = "0039", "0", C_DP.P_Def_StringEmpty)) & OrtsVW
            Else
                If Left(RufNr, 1) = "0" Then RufNr = Mid(RufNr, 2)
                RufNr = CStr(IIf(LandesVW = "0039", "0", C_DP.P_Def_StringEmpty)) & RufNr
            End If
            If Left(LandesVW, 2) = "00" Then LandesVW = Replace(LandesVW, "00", "+", 1, 1, CompareMethod.Text)
        Else
            OrtsVW = CStr(IIf(Left(OrtsVW, 1) = "0", OrtsVW, "0" & OrtsVW))
            LandesVW = C_DP.P_Def_StringEmpty
        End If

        ' NANP
        If LandesVW = "+1" Then
            Maske = "%L (%O) %N-%D"
            C_DP.P_CBTelNrGruppieren = False
            If tempDurchwahl = C_DP.P_Def_StringEmpty Then
                tempDurchwahl = Mid(RufNr, 4)
                RufNr = Left(RufNr, 3)
            End If
        End If

        If C_DP.P_CBTelNrGruppieren Then
            tempOrtsVW = GruppiereNummer(OrtsVW)
            tempRufNr = GruppiereNummer(RufNr)
            tempDurchwahl = GruppiereNummer(tempDurchwahl)
        Else
            tempOrtsVW = OrtsVW
            tempRufNr = RufNr
        End If
        ' formatTelNr zusammenstellen
        tempRufNr = Trim(Replace(tempRufNr, "  ", " ", , , CompareMethod.Text))
        ' Maske %L (%O) % - %D
        Maske = Replace(Maske, "%L", Trim(LandesVW))
        Maske = Replace(Maske, "%O", Trim(tempOrtsVW))
        Maske = Replace(Maske, "%N", tempRufNr)
        If Not Trim(tempDurchwahl) = C_DP.P_Def_StringEmpty Then
            Maske = Replace(Maske, "%D", Trim(tempDurchwahl))
        Else
            posDurchwahl = InStr(Maske, tempRufNr, CompareMethod.Text) + Len(tempRufNr) - 1
            Maske = Left(Maske, posDurchwahl)
        End If
        Maske = Trim(Replace(Maske, "  ", " ", , , CompareMethod.Text))

        Return Maske
    End Function

    Function GruppiereNummer(ByVal Nr As String) As String
        Dim imax As Integer
        imax = CInt(Math.Round(Len(Nr) / 2 + 0.1))
        GruppiereNummer = C_DP.P_Def_StringEmpty
        For i = 1 To imax
            GruppiereNummer = Right(Nr, 2) & " " & GruppiereNummer
            If Not Len(Nr) = 1 Then Nr = Left(Nr, Len(Nr) - 2)
        Next
    End Function

    Function OrtsVorwahlEntfernen(ByVal TelNr As String, ByVal Vorwahl As String) As String
        If Left(Vorwahl, 1) = "0" Then Vorwahl = Mid(Vorwahl, 2)
        If Left(TelNr, 1) = "0" Then TelNr = Mid(TelNr, 2)
        If Strings.Left(TelNr, Len(Vorwahl)) = Vorwahl Then TelNr = Mid(TelNr, Len(Vorwahl) + 1)
        Return TelNr
    End Function

    Function TelNrTeile(ByVal TelNr As String) As String()
        ' Findet die Ortsvorwahl in einem formatierten Telefonstring
        ' Kriterien: die Ortsvorwahl befindet sich in Klammern
        '            die OrtsVorwahl wird duch ein "-", "/" oder " " von der Rufnummer separiert
        ' Eine eventuell vorhandene Landesvorwahl wird berücksichtigt (vorher entfernt)
        ' Parameter:  TelNr (String):  Telefonnummer, die die Ortsvorwahl enthält
        ' Rückgabewert (String):       Ortsvorwahl

        Dim pos1 As Integer   ' Positionen innerhalb der TelNr
        Dim pos2 As Integer   ' Positionen innerhalb der TelNr
        Dim c As String ' einzelnes Zeichen des TelNr-Strings
        Dim OrtsVW As String = C_DP.P_Def_StringEmpty
        Dim LandesVW As String
        Dim Durchwahl As String
        Dim ErsteZiffer As String

        If Not TelNr = C_DP.P_Def_StringEmpty Then
            TelNr = Replace(TelNr, "(0)", " ", , , CompareMethod.Text)
            TelNr = Replace(TelNr, "++", "00", , , CompareMethod.Text)
            TelNr = Replace(TelNr, "+ ", "+", , , CompareMethod.Text)
            TelNr = Replace(TelNr, "+", "00", , , CompareMethod.Text)
            TelNr = Replace(TelNr, "[", "(", , , CompareMethod.Text)
            TelNr = Replace(TelNr, "]", ")", , , CompareMethod.Text)
            TelNr = Replace(TelNr, "{", "(", , , CompareMethod.Text)
            TelNr = Replace(TelNr, "[", ")", , , CompareMethod.Text)
            TelNr = Replace(TelNr, "#", "", , , CompareMethod.Text)
            TelNr = Replace(TelNr, " ", "", , , CompareMethod.Text)
            If Left(TelNr, 2) = "00" Then
                'Landesvorwahl vorhanden
                LandesVW = VorwahlausDatei(TelNr, My.Resources.LandesVorwahlen)
                If Not LandesVW = C_DP.P_Def_StringEmpty Then
                    LandesVW = "00" & LandesVW
                    TelNr = Mid(TelNr, Len(LandesVW) + 1)
                End If
            Else
                LandesVW = C_DP.P_Def_StringEmpty
            End If
            LandesVW = Replace(LandesVW, " ", "", , , CompareMethod.Text) 'Leerzeichen entfernen'

            pos1 = InStr(1, TelNr, "(", CompareMethod.Text) + 1
            pos2 = InStr(1, TelNr, ")", CompareMethod.Text)
            If pos1 = 1 Or pos2 = 0 Then
                If LandesVW = "0049" Or LandesVW = C_DP.P_Def_StringEmpty Then
                    ' Ortsvorwahl nicht in Klammern
                    If Left(TelNr, 1) = "0" Then TelNr = Mid(TelNr, 2)
                    OrtsVW = VorwahlausDatei(TelNr, My.Resources.Vorwahlen)

                    ' Vierstellige Mobilfunkvorwahlen ermitteln
                    ErsteZiffer = Mid(TelNr, Len(OrtsVW) + 1, 1)
                    Select Case OrtsVW
                        Case "150" ' Group3G UMTS Holding GmbH
                            If ErsteZiffer = "5" Then OrtsVW += ErsteZiffer
                        Case "151" ' Telekom Deutschland GmbH
                            If IsOneOf(ErsteZiffer, New String() {"1", "2", "4", "5"}) Then OrtsVW += ErsteZiffer
                        Case "152" ' Vodafone D2 GmbH
                            If IsOneOf(ErsteZiffer, New String() {"0", "1", "2", "3", "5"}) Then OrtsVW += ErsteZiffer
                        Case "157" ' E-Plus Mobilfunk GmbH & Co. KG 
                            If IsOneOf(ErsteZiffer, New String() {"0", "3", "5", "7", "8"}) Then OrtsVW += ErsteZiffer
                    End Select
                Else
                    OrtsVW = AuslandsVorwahlausDatei(TelNr, LandesVW)
                    Select Case LandesVW
                        Case "007"
                            ErsteZiffer = Mid(TelNr, Len(OrtsVW) + 1, 1)
                            ' Kasachstan
                            If IsOneOf(OrtsVW, New String() {"3292", "3152", "3252", "3232", "3262"}) And ErsteZiffer = "2" Then OrtsVW += ErsteZiffer
                            'case Polen
                    End Select
                End If
                TelNr = Mid(TelNr, Len(OrtsVW) + CInt(IIf(Left(TelNr, 1) = "0", 2, 1)))
            Else
                ' Ortsvorwahl in Klammern
                OrtsVW = nurZiffern(Mid(TelNr, pos1, pos2 - pos1), LandesVW)
                TelNr = Trim(Mid(TelNr, pos2 + 1))
            End If
            pos1 = 0
            Do
                pos1 = pos1 + 1
                c = Mid(TelNr, pos1, 1)
                Windows.Forms.Application.DoEvents()
            Loop While (c >= "0" And c <= "9") And pos1 <= Len(TelNr)
            If Not pos1 = 0 Then
                Durchwahl = Left(TelNr, pos1 - 1)
            Else
                Durchwahl = C_DP.P_Def_StringEmpty
            End If
            Durchwahl = Replace(Durchwahl, " ", "", , , CompareMethod.Text) 'Leerzeichen entfernen'
        Else
            LandesVW = C_DP.P_Def_StringEmpty
            OrtsVW = C_DP.P_Def_StringEmpty
            Durchwahl = C_DP.P_Def_StringEmpty
        End If
        TelNrTeile = New String() {LandesVW, OrtsVW, Durchwahl}

    End Function

    Function VorwahlausDatei(ByVal TelNr As String, ByVal Liste As String) As String
        VorwahlausDatei = C_DP.P_Def_StringEmpty
        Dim Suchmuster As String
        Dim Vorwahlen() As String = Split(Liste, vbNewLine, , CompareMethod.Text)
        Dim i As Integer = 1
        If Left(TelNr, 2) = "00" Then TelNr = Mid(TelNr, 3)
        If Left(TelNr, 1) = "0" Then TelNr = Mid(TelNr, 2)
        Do
            i += 1
            Suchmuster = Strings.Left(TelNr, i) & ";*"
            Dim Trefferliste = From s In Vorwahlen Where s.ToLower Like Suchmuster.ToLower Select s
            VorwahlausDatei = Split(Trefferliste(0), ";", , CompareMethod.Text)(0)
            Windows.Forms.Application.DoEvents()
        Loop Until Not VorwahlausDatei = C_DP.P_Def_StringEmpty Or i = 5
    End Function

    Function AuslandsVorwahlausDatei(ByVal TelNr As String, ByVal LandesVW As String) As String
        TelNr = Replace(TelNr, "*", "", , , CompareMethod.Text)
        AuslandsVorwahlausDatei = C_DP.P_Def_StringEmpty
        Dim Suchmuster As String
        Dim Vorwahlen() As String = Split(My.Resources.VorwahlenAusland, vbNewLine, , CompareMethod.Text)
        Dim i As Integer = 1
        Dim tmpvorwahl() As String
        If Left(LandesVW, 2) = "00" Then LandesVW = Mid(LandesVW, 3)
        If Left(LandesVW, 1) = "0" Then LandesVW = Mid(LandesVW, 2)
        If Left(TelNr, 2) = "00" Then TelNr = Mid(TelNr, 3)
        If Left(TelNr, 1) = "0" Then TelNr = Mid(TelNr, 2)
        Do
            i += 1
            Suchmuster = LandesVW & ";" & Strings.Left(TelNr, i) & ";*"
            Dim Trefferliste = From s In Vorwahlen Where s.ToLower Like Suchmuster.ToLower Select s
            Windows.Forms.Application.DoEvents()
            tmpvorwahl = Split(Trefferliste(0), ";", , CompareMethod.Text)
            If Not tmpvorwahl.Length = 1 Then AuslandsVorwahlausDatei = tmpvorwahl(1)
        Loop Until Not AuslandsVorwahlausDatei = C_DP.P_Def_StringEmpty Or i = 5
    End Function

    Public Function nurZiffern(ByVal TelNr As String, ByVal LandesVW As String) As String
        ' aus FritzBoxDial übernommen
        ' ist jetzt eine eigenständige Funktion, da sie häufig gebraucht wird
        ' bereinigt die Telefunnummer von Sonderzeichen wie Klammern und Striche
        ' Buchstaben werden wie auf der Telefontastatur in Zahlen übertragen
        ' Parameter:  TelNr (String):     Telefonnummer mit Sonderzeichen
        '             LandesVW (String):  eigene Landesvorwahl (wird entfernt)
        ' Rückgabewert (String):       saubere Telefonnummer (nur aus Ziffern bestehend)

        Dim i As Integer   ' Zählvariable
        Dim c As String ' einzelnes Zeichen
        ' Dim Vorwahl As String
        'Dim pos As Integer

        nurZiffern = C_DP.P_Def_StringEmpty
        TelNr = UCase(TelNr)
        'Vorwahl = TelNrTeile(TelNr)(1)
        'pos = InStr(1, Vorwahl, ";", vbTextCompare) + 1
        'Vorwahl = Mid(Vorwahl, pos, InStr(pos, Vorwahl, ";", vbTextCompare) - pos)
        ' Nur gültige Zeichen in der Nummer erlauben!
        For i = 1 To Len(TelNr)
            c = Mid(TelNr, i, 1)
            Select Case c                ' Einzelnes Char auswerten
                ' Zahlen und Steuerzeichen direkt übertragen.
                Case "0" To "9", "*", "#"
                    nurZiffern = nurZiffern + c
                    ' Restliche Buchstaben umwandeln.
                Case "A" To "C"
                    nurZiffern = nurZiffern + "2"
                Case "D" To "F"
                    nurZiffern = nurZiffern + "3"
                Case "G" To "I"
                    nurZiffern = nurZiffern + "4"
                Case "J" To "C"
                    nurZiffern = nurZiffern + "5"
                Case "M" To "O"
                    nurZiffern = nurZiffern + "6"
                Case "P" To "S"
                    nurZiffern = nurZiffern + "7"
                Case "T" To "V"
                    nurZiffern = nurZiffern + "8"
                Case "W" To "Z"
                    nurZiffern = nurZiffern + "9"
                Case "+"
                    nurZiffern = nurZiffern + "00"
            End Select
        Next
        ' Landesvorwahl entfernen bei Inlandsgesprächen (einschließlich nachfolgender 0)
        If Left(nurZiffern, Len(LandesVW)) = LandesVW Then
            nurZiffern = Replace(nurZiffern, LandesVW & "0", "0", , 1)
            nurZiffern = Replace(nurZiffern, LandesVW, "0", , 1)
        End If

        ' Bei diversen VoIP-Anbietern werden 2 führende Nullen zusätzlich gewählt: Entfernen "000" -> "0"
        If Left(nurZiffern, 3) = "000" Then nurZiffern = Right(nurZiffern, Len(nurZiffern) - 2)
    End Function '(nurZiffern)

    Function Mobilnummer(ByVal TelNr As String) As Boolean
        Dim TempTelNr As String() = TelNrTeile(TelNr)
        Dim Vorwahl As String = Left(TempTelNr(1), 2)
        If TempTelNr(0) = "0049" Or TempTelNr(0) = C_DP.P_Def_StringEmpty Then
            If Vorwahl = "15" Or Vorwahl = "16" Or Vorwahl = "17" Then Return True
        End If
        Return False
    End Function
#End Region

#Region " HTTPTransfer"
    ', ByRef Fehler As ErrObject
    'Public Function httpRead(ByVal Link As String, ByVal Encoding As System.Text.Encoding, ByRef FBError As ErrObject) As String
    '    FBError = Nothing
    '    Dim uri As New Uri(Link)
    '    httpRead = C_DP.P_Def_StringEmpty

    '    Try

    '        Select Case uri.Scheme
    '            Case uri.UriSchemeHttp

    '                With CType(HttpWebRequest.Create(uri), HttpWebRequest)
    '                    .Method = WebRequestMethods.Http.Get
    '                    .Proxy = Nothing
    '                    .KeepAlive = False
    '                    .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)
    '                    With New IO.StreamReader(.GetResponse().GetResponseStream(), Encoding)
    '                        httpRead = .ReadToEnd()
    '                        .Close()
    '                        .Dispose()
    '                    End With
    '                End With

    '            Case uri.UriSchemeFile

    '                With CType(FileWebRequest.Create(uri), FileWebRequest)
    '                    .Method = WebRequestMethods.Http.Get
    '                    .Proxy = Nothing
    '                    .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)
    '                    With New IO.StreamReader(.GetResponse().GetResponseStream(), Encoding)
    '                        httpRead = .ReadToEnd()
    '                        .Close()
    '                        .Dispose()
    '                    End With
    '                End With

    '        End Select




    '    Catch
    '        FBError = Err()
    '        LogFile("Es ist ein Fehler in der Funktion HTTPTransfer.Read aufgetreten: " & Err.Description)
    '    End Try
    '    uri = Nothing
    '    Return httpRead
    'End Function

    'Public Function httpWrite(ByVal Link As String, ByVal data As String, ByVal Encoding As System.Text.Encoding) As String
    '    httpWrite = C_DP.P_Def_StringEmpty
    '    Dim uri As New Uri(Link)
    '    Try
    '        If (uri.Scheme = uri.UriSchemeHttp) Then

    '            With CType(HttpWebRequest.Create(uri), HttpWebRequest)
    '                .Method = WebRequestMethods.Http.Post
    '                .Proxy = Nothing
    '                .KeepAlive = False
    '                .ContentLength = data.Length
    '                .ContentType = "application/x-www-form-urlencoded"
    '                .Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
    '                .UserAgent = C_DP.P_Def_UserAgent

    '                With New IO.StreamWriter(.GetRequestStream)
    '                    .Write(data)
    '                    System.Threading.Thread.Sleep(100)
    '                    .Close()
    '                End With

    '                With New IO.StreamReader(CType(.GetResponse, HttpWebResponse).GetResponseStream(), Encoding)
    '                    httpWrite = .ReadToEnd()
    '                    System.Threading.Thread.Sleep(100)
    '                    .Close()
    '                End With
    '            End With
    '        End If
    '    Catch
    '        LogFile("Es ist ein Fehler in der Funktion HTTPTransfer.Write aufgetreten: " & Err.Description)
    '    End Try

    'End Function

#End Region


#Region "HTTP-GET / POST"
    ''' <summary>
    ''' Läd einen Inhalt über einen HTTP-GET von einem Server herunter 
    ''' </summary>
    ''' <param name="Link">Link zum Inhalt</param>
    ''' <param name="Encoding">Zeichencodierung</param>
    ''' <param name="FBError">Rückgabewert. Ist Wahr, wenn Ein Fehler aufgetreten ist.</param>
    ''' <returns>String: Inhalt</returns>
    ''' <remarks></remarks>
    Public Function httpGET(ByVal Link As String, ByVal Encoding As System.Text.Encoding, ByRef FBError As Boolean) As String
        Dim webClient As New WebClient
        Dim uri As New Uri(Link)
        Dim PfadTMPfile As String

        httpGET = C_DP.P_Def_StringEmpty
        With webClient
            .Encoding = Encoding
            .Proxy = Nothing
            .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)

            Try
                If uri.Scheme = uri.UriSchemeHttp Then
                    .Headers.Add(HttpRequestHeader.KeepAlive, "False")
                End If
                If C_DP.P_Debug_SaveToFile Then
                    PfadTMPfile = My.Computer.FileSystem.GetTempFileName()
                    .DownloadFile(uri, PfadTMPfile)
                    LogFile("SaveToFile: " & Link & " gespeichert: " & PfadTMPfile)
                    httpGET = My.Computer.FileSystem.ReadAllText(PfadTMPfile)
                    My.Computer.FileSystem.DeleteFile(PfadTMPfile)
                    LogFile("SaveToFile: " & PfadTMPfile & " gelöscht.")
                Else
                    httpGET = .DownloadString(uri)
                End If
            Catch exANE As ArgumentNullException
                FBError = True
                LogFile("httpGET: " & exANE.Message)
            Catch exWE As WebException
                FBError = True
                LogFile("httpGET: " & exWE.Message & " - Link: " & Link)
            End Try

        End With


    End Function

    Public Function httpPOST(ByVal Link As String, ByVal UploadData As String, ByVal Encoding As System.Text.Encoding) As String
        Dim webClient As New WebClient
        Dim uri As New Uri(Link)

        httpPOST = C_DP.P_Def_StringEmpty

        If uri.Scheme = uri.UriSchemeHttp Then
            With webClient
                .Encoding = Encoding
                .Proxy = Nothing
                .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)

                With .Headers
                    .Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded")
                    .Add(HttpRequestHeader.ContentLength, UploadData.Length.ToString)
                    .Add(HttpRequestHeader.UserAgent, C_DP.P_Def_UserAgent)
                    .Add(HttpRequestHeader.KeepAlive, "False")
                    .Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
                End With

                Try
                    httpPOST = .UploadString(uri, UploadData)
                Catch exANE As ArgumentNullException
                    LogFile("httpPOST: " & exANE.Message)
                Catch exWE As WebException
                    LogFile("httpPOST: " & exWE.Message & " - Link: " & Link)
                End Try
            End With
        End If
    End Function
#End Region

#Region " Timer"
    Public Function SetTimer(ByRef Interval As Double) As System.Timers.Timer
        Try
            'Debug.Print("Entering System Timers") 'dann werden die Knöpfe sowieso immer eingeblendet.
            Dim aTimer As New System.Timers.Timer
            With aTimer
                .Interval = Interval
                .AutoReset = True
                .Enabled = True
            End With
            Return aTimer
        Catch ex As Exception
            FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "SetTimer")
            Return Nothing
        End Try
    End Function

    Public Function KillTimer(ByVal Timer As System.Timers.Timer) As Boolean
        Try
            With Timer
                .AutoReset = False
                .Enabled = False
                .Dispose()
                Return True
            End With
        Catch ex As Exception
            FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "KillTimer")
            Return False
        End Try
    End Function
#End Region

    Public Function GetTimeInterval(ByVal nSeks As Double) As String
        'http://www.vbarchiv.net/faq/date_sectotime.php
        Dim h As Double, m As Double
        h = nSeks / 3600
        nSeks = nSeks Mod 3600
        m = nSeks / 60
        nSeks = nSeks Mod 60
        Return Format(h, "00") & ":" & Format(m, "00") & ":" & Format(nSeks, "00")
    End Function

    Public Function AcceptOnlyNumeric(ByVal sTxt As String) As String
        If sTxt = String.Empty Then Return String.Empty
        If Mid(sTxt, Len(sTxt), 1) Like "[0-9]" = False Then
            Return Mid(sTxt, 1, Len(sTxt) - 1)
        End If
        Return sTxt
    End Function
End Class
