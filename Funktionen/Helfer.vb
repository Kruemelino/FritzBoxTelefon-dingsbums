Imports System.Net
Imports System.Text
Imports System.Threading
Imports System.IO

Public Class Helfer
    Public Enum Vorwahllisten
        Liste_Landesvorwahlen = 0
        Liste_Ortsvorwahlen_Ausland = 1
        Liste_Ortsvorwahlen_Deutschland = 2
    End Enum

    Private C_XML As XML
    Private C_DP As DataProvider
    Private C_Crypt As Rijndael

    Public Sub New(ByVal DataProviderKlasse As DataProvider, ByVal CryptKlasse As Rijndael, XMLKlasse As XML)
        C_XML = XMLKlasse
        C_DP = DataProviderKlasse
        C_Crypt = CryptKlasse
    End Sub

#Region " String Behandlung"
    ''' <summary>
    ''' Entnimmt aus dem String <c>Text</c> einen enthaltenen Sub-String ausgehend von einer Zeichenfolge davor <c>StringDavor</c> 
    ''' und deiner Zeichenfolge danach <c>StringDanach</c>.
    ''' </summary>
    ''' <param name="Text">String aus dem der Sub-String entnommen werden soll.</param>
    ''' <param name="StringDavor">Zeichenfolge vor dem zu entnehmenden Sub-String.</param>
    ''' <param name="StringDanach">Zeichenfolge nach dem zu entnehmenden Sub-String.</param>
    ''' <param name="Reverse">Flag, Ob die Suche nach den Zeichenfolgen vor und nach dem Sub-String vom Ende des <c>Textes</c> aus begonnen werden soll.</param>
    ''' <returns>Wenn <c>StringDavor</c> und <c>StringDanach</c> enthalten sind, dann wird der Teilstring zurückgegeben. Ansonsten "-1".</returns>

    Public Overloads Function StringEntnehmen(ByVal Text As String, ByVal StringDavor As String, ByVal StringDanach As String, Optional ByVal Reverse As Boolean = False) As String
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
            StringEntnehmen = DataProvider.P_Def_ErrorMinusOne_String
        End If
    End Function

    ''' <summary>
    ''' Entnimmt aus dem String <c>Text</c> einen enthaltenen Sub-String ausgehend von einer Zeichenfolge davor <c>StringDavor</c> 
    ''' und deiner Zeichenfolge danach <c>StringDanach</c>.
    ''' Beginnt Suche nach TeilString an einem Startpunkt <c>StartPosition</c>.
    ''' </summary>
    ''' <param name="Text">String aus dem der Sub-String entnommen werden soll.</param>
    ''' <param name="StringDavor">Zeichenfolge vor dem zu entnehmenden Sub-String.</param>
    ''' <param name="StringDanach">Zeichenfolge nach dem zu entnehmenden Sub-String.</param>
    ''' <param name="StartPosition">Startposition, bei der mit der Suche nach den Zeichenfolgen vor und nach dem Sub-String begonnen werden soll.</param>
    ''' <returns>Wenn <c>StringDavor</c> und <c>StringDanach</c> enthalten sind, dann wird der Teilstring zurückgegeben. Ansonsten "-1".</returns>

    Public Overloads Function StringEntnehmen(ByVal Text As String, ByVal StringDavor As String, ByVal StringDanach As String, ByRef StartPosition As Integer) As String
        Dim pos(1) As Integer

        pos(0) = InStr(StartPosition, Text, StringDavor, CompareMethod.Text) + Len(StringDavor)
        pos(1) = InStr(pos(0), Text, StringDanach, CompareMethod.Text)

        If Not pos(0) = Len(StringDavor) Then
            StringEntnehmen = Mid(Text, pos(0), pos(1) - pos(0))
            StartPosition = pos(1)
        Else
            StringEntnehmen = DataProvider.P_Def_ErrorMinusOne_String
        End If

    End Function

    ''' <summary>
    ''' Prüft ob, ein String <c>A</c> in einem Sting-Array <c>B</c> enthalten ist. 
    ''' </summary>
    ''' <param name="A">Zu prüfender String.</param>
    ''' <param name="B">Array in dem zu prüfen ist.</param>
    ''' <returns><c>True</c>, wenn enthalten, <c>False</c>, wenn nicht.</returns>

    Public Function IsOneOf(ByVal A As String, ByVal B() As String) As Boolean
        Return CBool(IIf((From Strng In B Where Strng = A).ToArray.Count = 0, False, True))
    End Function
#End Region

    Public Sub NAR(ByVal o As Object)
        If o IsNot Nothing Then
            Try
                System.Runtime.InteropServices.Marshal.ReleaseComObject(o)
            Catch ex As Exception
                FBDB_MsgBox(ex.Message, MsgBoxStyle.Critical, "NAR")
            Finally
                o = Nothing
            End Try
        End If
    End Sub

    Public Function GetUnixTime() As Integer
        Return CInt((DateTime.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds)
    End Function

    ''' <summary>
    ''' Führt einen Ping zur Gegenstelle aus.
    ''' </summary>
    ''' <param name="IPAdresse">IP-Adresse Netzwerkname der Gegenstelle. Rückgabe der IP-Adresse</param>
    ''' <returns>Boolean</returns>
    Public Function Ping(ByRef IPAdresse As String) As Boolean
        Ping = False

        Dim IPHostInfo As IPHostEntry
        Dim PingSender As New NetworkInformation.Ping()
        Dim Options As New NetworkInformation.PingOptions()
        Dim PingReply As NetworkInformation.PingReply = Nothing
        Dim data As String = DataProvider.P_Def_LeerString

        Dim buffer As Byte() = Encoding.ASCII.GetBytes(data)
        Dim timeout As Integer = 120

        Options.DontFragment = True

        Try
            PingReply = PingSender.Send(IPAdresse, timeout, buffer, Options)
        Catch ex As Exception
            LogFile("Ping zu """ & IPAdresse & """ nicht erfolgreich: " & ex.InnerException.Message)
            Ping = False
        End Try

        If PingReply IsNot Nothing Then
            With PingReply
                If .Status = NetworkInformation.IPStatus.Success Then
                    If .Address.AddressFamily = Sockets.AddressFamily.InterNetworkV6 Then
                        'Zugehörige IPv4 ermitteln
                        IPHostInfo = Dns.GetHostEntry(.Address)
                        For Each _IPAddress As IPAddress In IPHostInfo.AddressList
                            If _IPAddress.AddressFamily = Sockets.AddressFamily.InterNetwork Then
                                IPAdresse = _IPAddress.ToString
                                ' Prüfen ob es eine generel gültige lokale IPv6 Adresse gibt: fd00::2665:11ff:fed8:6086
                                ' und wie die zu ermitteln ist
                                LogFile("IPv6: " & .Address.ToString & ", IPv4: " & IPAdresse)
                                Exit For
                            End If
                        Next
                    Else
                        IPAdresse = .Address.ToString
                    End If
                    Ping = True
                Else
                    LogFile("Ping zu """ & IPAdresse & """ nicht erfolgreich: " & .Status)
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
    ''' 
    ''' Möglicher Input:
    ''' IPv4: Nichts unternehmen
    ''' IPv6: 
    ''' String, der aufgelöst werden kann z.B. "fritz.box"
    ''' String, der nicht aufgelöst werden kann
    ''' </summary>
    ''' <param name="InputIP">IP-Adresse</param>
    ''' <returns>Korrekte IP-Adresse</returns>
    Public Function ValidIP(ByVal InputIP As String) As String
        Dim IPAddresse As IPAddress = Nothing
        Dim IPHostInfo As IPHostEntry

        ValidIP = DataProvider.P_Def_FritzBoxAdress

        If IPAddress.TryParse(InputIP, IPAddresse) Then
            Select Case IPAddresse.AddressFamily
                Case Sockets.AddressFamily.InterNetworkV6
                    ValidIP = "[" & IPAddresse.ToString & "]"
                Case Sockets.AddressFamily.InterNetwork
                    ValidIP = IPAddresse.ToString
                Case Else
                    LogFile("Die IP """ & InputIP & """ kann nicht zugeordnet werden.")
                    ValidIP = InputIP
            End Select
        Else
            Try
                IPHostInfo = Dns.GetHostEntry(C_DP.P_TBFBAdr)
                For Each IPAddresse In IPHostInfo.AddressList
                    If IPAddresse.AddressFamily = Sockets.AddressFamily.InterNetwork Then
                        ValidIP = IPAddresse.ToString
                    End If
                Next
            Catch ' ex As Exception
                LogFile("Die Adresse """ & C_DP.P_TBFBAdr & """ kann nicht zugeordnet werden.")
                ValidIP = C_DP.P_TBFBAdr
            End Try
        End If

    End Function

    Public Sub LogFile(ByVal Meldung As String)
        Dim LogDatei As String = C_DP.P_Arbeitsverzeichnis & DataProvider.P_Def_Log_FileName
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
    End Sub

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
        Return MsgBox(Meldung, Style, DataProvider.P_Def_Addin_LangName) '"Fritz!Box Telefon-Dingsbums"
    End Function

    ''' <summary>
    ''' Diese Routine ändert den Zugang zu den verschlüsselten Passwort.
    ''' </summary>
    Public Sub KeyChange()
        Dim AlterZugang As String
        Dim NeuerZugang As String
        If Not C_DP.P_TBPasswort = DataProvider.P_Def_LeerString Then
            With C_DP
                AlterZugang = .GetSettingsVBA("Zugang", DataProvider.P_Def_ErrorMinusOne_String)
                If Not AlterZugang = DataProvider.P_Def_ErrorMinusOne_String Then
                    NeuerZugang = C_Crypt.GetSalt
                    .P_TBPasswort = C_Crypt.EncryptString128Bit(C_Crypt.DecryptString128Bit(.P_TBPasswort, AlterZugang), NeuerZugang)
                    .SaveSettingsVBA("Zugang", NeuerZugang)
                Else
                    LogFile(DataProvider.P_Lit_KeyChange("die Fritz!Box"))
                    .P_TBPasswort = DataProvider.P_Def_LeerString
                End If
            End With
        End If

        If Not C_DP.P_TBPhonerPasswort = DataProvider.P_Def_LeerString Then
            With C_DP
                AlterZugang = .GetSettingsVBA("ZugangPasswortPhoner", DataProvider.P_Def_ErrorMinusOne_String)
                If Not AlterZugang = DataProvider.P_Def_ErrorMinusOne_String Then
                    NeuerZugang = C_Crypt.GetSalt
                    .P_TBPhonerPasswort = C_Crypt.EncryptString128Bit(C_Crypt.DecryptString128Bit(.P_TBPhonerPasswort, AlterZugang), NeuerZugang)
                    .SaveSettingsVBA("ZugangPasswortPhoner", NeuerZugang)
                Else
                    LogFile(DataProvider.P_Lit_KeyChange("Phoner"))
                    .P_TBPhonerPasswort = DataProvider.P_Def_LeerString
                End If
            End With
        End If

        C_DP.SpeichereXMLDatei()

    End Sub ' (Keyänderung) 

#Region " Telefonnummern formatieren"
    ''' <summary>
    ''' Formatiert die Telefonnummern nach gängigen Regeln
    ''' </summary>
    ''' <param name="TelNr">Die zu formatierende Telefonnummer</param>
    ''' <returns>Die formatierte Telefonnummer</returns>

    Function formatTelNr(ByVal TelNr As String) As String
        Dim RufNr As String ' Telefonnummer ohne Vorwahl
        Dim LandesVW As String
        Dim OrtsVW As String
        Dim Durchwahl As String
        Dim posOrtsVW As Integer   ' Position der Vorwahl in TelNr
        Dim posDurchwahl As Integer   ' Position der Durchwahl in TelNr
        Dim tempOrtsVW As String = DataProvider.P_Def_LeerString ' Hilfsstring für OrtsVW
        Dim tempRufNr As String = DataProvider.P_Def_LeerString ' Hilfsstring für RufNr
        Dim tempDurchwahl As String = DataProvider.P_Def_LeerString ' Hilfsstring für LandesVW
        Dim TelTeile() As String = TelNrTeile(TelNr)
        Dim Maske As String = C_DP.P_TBTelNrMaske
        Dim TelNrGruppieren As Boolean

        LandesVW = TelTeile(0)
        OrtsVW = TelTeile(1)
        Durchwahl = TelTeile(2)

        TelNr = nurZiffern(TelNr) ' Nur ziffern erntfernt Landesvorwahl, wenn diese mir der in den Einstellungen übereinstimmt.

        ' 1. Landesvorwahl abtrennen
        ' Landesvorwahl ist immer an erster Stelle (wenn vorhanden)
        ' [0043]123456789
        ' Italien ist eine Ausnahme:
        ' Die führende Null der Ortskennung ist fester, unveränderlicher und unverzichtbarer Bestandteil und muss bestehen bleiben. 
        ' Handynummern in Italien haben dagegen keine führende Null.
        If Not LandesVW = DataProvider.P_Def_LeerString And Not LandesVW = C_DP.P_TBLandesVW Then
            RufNr = Mid(TelNr, Len(LandesVW) + 1)
            'If LandesVW = "0039" AndAlso Left(RufNr, 1) = "0" Then ' Italien
            ''Mach irgendwas. Oder auch nicht.
            'End If
        Else
            RufNr = TelNr
        End If

        ' 2. Ortsvorwahl entfernen
        ' [0123]456789
        If Not OrtsVW = DataProvider.P_Def_LeerString Then
            posOrtsVW = InStr(RufNr, OrtsVW, CompareMethod.Text)
            RufNr = Mid(RufNr, posOrtsVW + Len(OrtsVW))
        Else
            If RufNr = DataProvider.P_Def_LeerString Then RufNr = TelNr
        End If

        ' nur ausführen, wenn die Ortsvorwahl in der Telefonnummer enthalten ist
        ' LandesVW und RufNr aus TelNr separieren

        posDurchwahl = InStr(1, RufNr, Durchwahl, CompareMethod.Text)
        If posDurchwahl = 1 And Not Durchwahl = DataProvider.P_Def_LeerString Then
            tempDurchwahl = Mid(RufNr, Len(Durchwahl) + 1)
            RufNr = Durchwahl
        Else
            Durchwahl = DataProvider.P_Def_LeerString
        End If

        If LandesVW = "0" Then
            OrtsVW = "0" & OrtsVW
            LandesVW = DataProvider.P_Def_LeerString
        End If

        ' Maske Prüfen
        If InStr(Maske, "%D", CompareMethod.Text) = 0 Then Maske = Replace(Maske, "%N", "%N%D")
        If Not InStr(Maske, "%N%D", CompareMethod.Text) = 0 Then
            RufNr = RufNr & tempDurchwahl
            tempDurchwahl = DataProvider.P_Def_LeerString
        End If

        ' Setze die Ortsvorwahl, wenn immer eine internale Nummer erzeugt werden soll UND
        '                        wenn die Landesvorwahl der Nummer leer ist ODER gleich der eigestellten Landesvorwahl ist UND
        '                        die Ortsvorwahl nicht vorhanden ist
        If (LandesVW = C_DP.P_TBLandesVW Or LandesVW = DataProvider.P_Def_LeerString) And C_DP.P_CBintl And OrtsVW = DataProvider.P_Def_LeerString Then
            OrtsVW = C_DP.P_TBVorwahl
        End If

        If OrtsVW = DataProvider.P_Def_LeerString Then
            ' Maske %L (%O) %N - %D
            ' Wenn keine Ortsvorwahl vorhanden ist, dann muss diese bei der Formatierung nicht berücksichtigt werden.
            ' Keine Ortsvorwahl: Alles zwischen %L und %N entfernen
            Dim pos1 As Integer
            Dim pos2 As Integer
            Dim CutOut As String
            pos1 = InStr(Maske, "%L", CompareMethod.Text) + 2
            pos2 = InStr(Maske, "%N", CompareMethod.Text)
            CutOut = Mid(Maske, pos1, pos2 - pos1)
            Maske = Replace(Maske, CutOut, CStr(IIf(Left(CutOut, 1) = " ", " ", DataProvider.P_Def_LeerString)), , 1, CompareMethod.Text)
        End If

        If C_DP.P_CBintl Then
            If LandesVW = DataProvider.P_Def_LeerString Then LandesVW = C_DP.P_TBLandesVW
            If OrtsVW = DataProvider.P_Def_LeerString Then
                If Not LandesVW = DataProvider.P_Def_PreLandesVW & "39" Then
                    'Else
                    If OrtsVW.StartsWith("0") Then
                        OrtsVW = Mid(OrtsVW, 2)
                    End If
                End If
            End If
        Else
            If Not OrtsVW = DataProvider.P_Def_LeerString Then
                OrtsVW = CStr(IIf(Left(OrtsVW, 1) = "0", OrtsVW, "0" & OrtsVW))
            End If
            If LandesVW = C_DP.P_TBLandesVW Then
                LandesVW = DataProvider.P_Def_LeerString
            End If
        End If

        If Not LandesVW = DataProvider.P_Def_LeerString Then
            If LandesVW.StartsWith(DataProvider.P_Def_PreLandesVW) Then LandesVW = Replace(LandesVW, DataProvider.P_Def_PreLandesVW, "+", 1, 1, CompareMethod.Text)
        End If

        TelNrGruppieren = C_DP.P_CBTelNrGruppieren
        ' NANP
        If LandesVW = "+1" Then
            Maske = "%L (%O) %N-%D"
            TelNrGruppieren = False
            If tempDurchwahl = DataProvider.P_Def_LeerString Then
                tempDurchwahl = Mid(RufNr, 4)
                RufNr = Left(RufNr, 3)
            End If
        End If

        If TelNrGruppieren Then
            tempOrtsVW = GruppiereNummer(OrtsVW)
            tempRufNr = GruppiereNummer(RufNr)
            tempDurchwahl = GruppiereNummer(tempDurchwahl)
        Else
            tempOrtsVW = OrtsVW
            tempRufNr = RufNr
        End If
        ' formatTelNr zusammenstellen
        tempRufNr = Trim(Replace(tempRufNr, "  ", " ", , , CompareMethod.Text))
        ' Maske %L (%O) %N - %D
        Maske = Replace(Maske, "%L", Trim(LandesVW))
        Maske = Replace(Maske, "%O", Trim(tempOrtsVW))
        Maske = Replace(Maske, "%N", tempRufNr)
        If Not Trim(tempDurchwahl) = DataProvider.P_Def_LeerString Then
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
        GruppiereNummer = DataProvider.P_Def_LeerString
        For i = 1 To imax
            GruppiereNummer = Right(Nr, 2) & " " & GruppiereNummer
            If Not Len(Nr) = 1 Then Nr = Left(Nr, Len(Nr) - 2)
        Next
    End Function


    ''' <summary>
    ''' Entfernt alle Vorahlen aus den eigenen Nummern. D.h. diese Funktion ist nur gedacht um konfigurierten Nummern aus der Fritz!Box zu verarbeiten.
    ''' </summary>
    ''' <param name="TelNr">Eigene Nummer aus der Fritz!Box</param>
    ''' <returns>Eigene Nummer ohne Landes- und Ortsvorwahl</returns>
    ''' <remarks>In einigen Boxen sind die Nummern mit Landes- und Ortsvorwahl integriert. 
    ''' Problematisch ist dies unter Umständen, da die Nummern bei den Telefonen systematisch fehlerhaft eingetragen sind. Dies wirkt sich auch auf den Anrufmonitor aus.
    ''' Es kann sein, dass die eigene Nummer mit der Landesvorwahl OHNE "+" oder "00" beginnt. In dem Fall wird geprüft, ob die eigene Nummer mit der 
    ''' Landesvorwahl OHNE "+" oder "00" und der Ortsvorwahl beginnt. Hier ist weitere Optimierung nötig: 
    ''' Es ist denkbar, dass es eine komplette Nummer gibt: +49304930NNN. Wenn diese Nummer in der Fritz!Box als 4930NNN (also ohne eigentliche Landes und Ortsvorwahl) hinterlegt ist,
    ''' dann wird die Nummer fälschlicherweise nur zu NNN und nicht korrekt zu 4930NNN verarbeitet.</remarks>
    Function EigeneVorwahlenEntfernen(ByVal TelNr As String) As String

        Dim tmpTelNrTeile() As String
        Dim tmpLandesVorwahl As String
        Dim tmpOrtsVorwahl As String


        If Not TelNr = DataProvider.P_Def_LeerString Then

            ' Nummer korrigieren, falls diese mit der Landes- und Ortsvorwahl ohne führende "00" beginnt.
            tmpLandesVorwahl = C_DP.P_TBLandesVW
            tmpOrtsVorwahl = C_DP.P_TBVorwahl
            If tmpLandesVorwahl.StartsWith(DataProvider.P_Def_PreLandesVW) Then tmpLandesVorwahl = tmpLandesVorwahl.Remove(0, 2)
            If tmpOrtsVorwahl.StartsWith("0") Then tmpOrtsVorwahl = tmpOrtsVorwahl.Remove(0, 1)

            If TelNr.StartsWith(tmpLandesVorwahl & tmpOrtsVorwahl) Then TelNr = TelNr.Insert(0, "+")

            tmpTelNrTeile = TelNrTeile(TelNr)
            TelNrBereinigen(TelNr)

            ' Landesvorwahl entfernen
            If tmpTelNrTeile(0) = tmpLandesVorwahl.Insert(0, DataProvider.P_Def_PreLandesVW) Then TelNr = TelNr.Remove(0, Len(tmpTelNrTeile(0)))

            ' Ortsvorwahl entfernen
            If tmpTelNrTeile(1) = tmpOrtsVorwahl And Not Mobilnummer(TelNr) Then
                TelNr = TelNr.Remove(0, Len(tmpTelNrTeile(1)) + CInt(IIf(TelNr.StartsWith("0"), 1, 0)))
            End If

        End If

        Return TelNr
    End Function

    ''' <summary>
    ''' TelNr bereinigen
    ''' </summary>
    ''' <param name="TelNr"></param>

    Private Sub TelNrBereinigen(ByRef TelNr As String)

        'TelNr = Replace(TelNr, "(0)", " ", , , CompareMethod.Text)
        'TelNr = Replace(TelNr, "++", "00", , , CompareMethod.Text)
        'TelNr = Replace(TelNr, "+ ", "+", , , CompareMethod.Text)
        'TelNr = Replace(TelNr, "+", "00", , , CompareMethod.Text)
        'TelNr = Replace(TelNr, "[", "(", , , CompareMethod.Text)
        'TelNr = Replace(TelNr, "]", ")", , , CompareMethod.Text)
        'TelNr = Replace(TelNr, "{", "(", , , CompareMethod.Text)
        'TelNr = Replace(TelNr, "[", ")", , , CompareMethod.Text)
        'TelNr = Replace(TelNr, "#", "", , , CompareMethod.Text)
        'TelNr = Replace(TelNr, " ", "", , , CompareMethod.Text)

        TelNr = TelNr.Replace("(0)", " ")
        TelNr = TelNr.Replace("++", DataProvider.P_Def_PreLandesVW)
        TelNr = TelNr.Replace("+ ", "+")
        TelNr = TelNr.Replace("+", DataProvider.P_Def_PreLandesVW)
        TelNr = TelNr.Replace("[", "(")
        TelNr = TelNr.Replace("]", ")")
        TelNr = TelNr.Replace("{", "(")
        TelNr = TelNr.Replace("[", ")")
        TelNr = TelNr.Replace("#", "")
        TelNr = TelNr.Replace(" ", "")
    End Sub

    ''' <summary>
    ''' Zerlegt die Telefonnummer in ihre Bestandteile (0: Landesvorwahl,1: Ortsvorwahl,2: Durchwahl).
    ''' </summary>
    ''' <param name="TelNr">Zu zerlegende Telefonnummer.</param>
    ''' <returns>Stringarray:
    ''' 0: Landesvorwahl mit führenden "00"
    ''' 1: Ortsvorwahl ohne führende 0
    ''' 2: Durchwahl</returns>
    ''' <remarks>Findet die Ortsvorwahl in einem formatierten Telefonstring
    '''  Kriterien: die Ortsvorwahl befindet sich in Klammern
    '''  die OrtsVorwahl wird duch ein "-", "/" oder " " von der Rufnummer separiert
    '''  Eine eventuell vorhandene Landesvorwahl wird berücksichtigt (vorher entfernt)</remarks>
    Function TelNrTeile(ByVal TelNr As String) As String()

        Dim pos1 As Integer   ' Positionen innerhalb der TelNr
        Dim pos2 As Integer   ' Positionen innerhalb der TelNr
        Dim c As String ' einzelnes Zeichen des TelNr-Strings
        Dim OrtsVW As String = DataProvider.P_Def_LeerString
        Dim LandesVW As String
        Dim Durchwahl As String
        Dim ErsteZiffer As String

        If Not TelNr = DataProvider.P_Def_LeerString Then

            TelNrBereinigen(TelNr)

            If TelNr.StartsWith(DataProvider.P_Def_PreLandesVW) Then
                'Landesvorwahl vorhanden
                LandesVW = VorwahlausDatei(TelNr, My.Resources.Liste_Landesvorwahlen)
                If Not LandesVW = DataProvider.P_Def_LeerString Then
                    LandesVW = DataProvider.P_Def_PreLandesVW & LandesVW
                    TelNr = Mid(TelNr, Len(LandesVW) + 1)
                End If
            Else
                LandesVW = DataProvider.P_Def_LeerString
            End If
            LandesVW = Replace(LandesVW, " ", "", , , CompareMethod.Text) 'Leerzeichen entfernen'

            pos1 = InStr(TelNr, "(", CompareMethod.Text) + 1
            pos2 = InStr(TelNr, ")", CompareMethod.Text)
            If pos1 = 1 Or pos2 = 0 Then
                If LandesVW = DataProvider.P_Def_TBLandesVW Or LandesVW = DataProvider.P_Def_LeerString Then 'nur Deutschland
                    ' Ortsvorwahl nicht in Klammern

                    If TelNr.StartsWith("0") Then TelNr = TelNr.Remove(0, 1) ' Null entfernen
                    OrtsVW = VorwahlausDatei(TelNr, My.Resources.Liste_Ortsvorwahlen_Deutschland)

                    ' Vierstellige Mobilfunkvorwahlen ermitteln
                    'ErsteZiffer = Mid(TelNr, Len(OrtsVW) + 1, 1)
                    'Select Case OrtsVW
                    '    Case "150" ' Group3G UMTS Holding GmbH
                    '        If ErsteZiffer = "5" Then OrtsVW += ErsteZiffer
                    '    Case "151" ' Telekom Deutschland GmbH
                    '        If IsOneOf(ErsteZiffer, New String() {"1", "2", "4", "5", "6", "7"}) Then OrtsVW += ErsteZiffer
                    '    Case "152" ' Vodafone D2 GmbH
                    '        If IsOneOf(ErsteZiffer, New String() {"0", "1", "2", "3", "5", "6"}) Then OrtsVW += ErsteZiffer
                    '    Case "157" ' E-Plus Mobilfunk GmbH & Co. KG 
                    '        If IsOneOf(ErsteZiffer, New String() {"0", "3", "5", "7", "8", "9"}) Then OrtsVW += ErsteZiffer
                    '    Case "159" ' Telefónica Germany GmbH & Co. OHG (O2) 
                    '        If IsOneOf(ErsteZiffer, New String() {"0"}) Then OrtsVW += ErsteZiffer
                    'End Select

                    ''Die Vorwahlen sind von der Bundesnetzagentur wie folgt vergeben, Wikipedia abgerufen 24.05.2014

                    ''Telekom: 01511, 01512, 01514, 01515, 01516, 01517, 0160, 0170, 0171, 0175
                    ''Vodafone: 01520, 01522, 01523, 01525, 01526 (ab März 2014), 0162, 0172, 0173, 0174, 01529 (Tru)
                    ''Virtuelle Netzbetreiber (nutzt Netz von Vodafone, im Hintergrund eigene Infrastruktur): 01521 Lycamobile
                    ''E-Plus: 01573, 01575, 01577, 01578, 0163, 0177, 0178
                    ''Virtuelle Netzbetreiber (nutzen Netz von E-Plus, im Hintergrund eigene Infrastruktur): 01570 Telogic (Betrieb eingestellt), 01579 Sipgate Wireless
                    ''O2: 01590, 0176, 0179
                Else
                    OrtsVW = AuslandsVorwahlausDatei(TelNr, LandesVW)
                    Select Case LandesVW
                        Case DataProvider.P_Def_PreLandesVW & "7" ' Kasachstan
                            ErsteZiffer = Mid(TelNr, Len(OrtsVW) + 1, 1)
                            If IsOneOf(OrtsVW, New String() {"3292", "3152", "3252", "3232", "3262"}) And ErsteZiffer = "2" Then OrtsVW += ErsteZiffer
                        Case DataProvider.P_Def_PreLandesVW & "39" ' Italien
                            If TelNr.StartsWith("0") Then OrtsVW = "0" & OrtsVW
                    End Select
                End If
                TelNr = Mid(TelNr, Len(OrtsVW) + 1) 'CInt(IIf(Left(TelNr, 1) = "0", 2, 1))
            Else
                ' Ortsvorwahl in Klammern
                OrtsVW = nurZiffern(Mid(TelNr, pos1, pos2 - pos1))
                TelNr = Trim(Mid(TelNr, pos2 + 1))
            End If
            'Durchwahl ermitteln
            pos1 = 0
            Do
                pos1 = pos1 + 1
                c = Mid(TelNr, pos1, 1)
                Windows.Forms.Application.DoEvents()
            Loop While (c >= "0" And c <= "9") And pos1 <= Len(TelNr)
            If Not pos1 = 0 And Not pos1 = Len(TelNr) + 1 Then
                Durchwahl = Left(TelNr, pos1 - 1)
            Else
                Durchwahl = DataProvider.P_Def_LeerString
            End If
            Durchwahl = Replace(Durchwahl, " ", "", , , CompareMethod.Text) 'Leerzeichen entfernen'
        Else
            LandesVW = DataProvider.P_Def_LeerString
            OrtsVW = DataProvider.P_Def_LeerString
            Durchwahl = DataProvider.P_Def_LeerString
        End If
        TelNrTeile = New String() {LandesVW, OrtsVW, Durchwahl}

    End Function

    Function VorwahlausDatei(ByVal TelNr As String, ByVal Liste As String) As String
        VorwahlausDatei = DataProvider.P_Def_LeerString
        Dim Suchmuster As String
        Dim Vorwahlen() As String = Split(Liste, vbNewLine, , CompareMethod.Text)
        Dim i As Integer = 1
        Dim tmpErgebnis As String
        Dim Treffer As String = DataProvider.P_Def_LeerString

        If TelNr.StartsWith(DataProvider.P_Def_PreLandesVW) Then TelNr = TelNr.Remove(0, 2)
        If TelNr.StartsWith("0") Then TelNr = TelNr.Remove(0, 1)
        Do
            i += 1
            Suchmuster = Strings.Left(TelNr, i) & ";*"
            Dim Trefferliste = From s In Vorwahlen Where s.ToLower Like Suchmuster.ToLower Select s
            tmpErgebnis = Split(Trefferliste(0), ";", , CompareMethod.Text)(0)
            If Not tmpErgebnis = DataProvider.P_Def_LeerString Then
                Treffer = tmpErgebnis
            End If
            Windows.Forms.Application.DoEvents()
        Loop Until i = 5
        Return Treffer
    End Function

    Function AuslandsVorwahlausDatei(ByVal TelNr As String, ByVal LandesVW As String) As String

        Dim Suchmuster As String
        Dim Vorwahlen() As String = Split(My.Resources.Liste_Ortsvorwahlen_Ausland, vbNewLine, , CompareMethod.Text)
        Dim i As Integer = 0
        Dim tmpvorwahl() As String

        TelNr = Replace(TelNr, "*", "", , , CompareMethod.Text)
        AuslandsVorwahlausDatei = DataProvider.P_Def_LeerString

        If LandesVW.StartsWith(DataProvider.P_Def_PreLandesVW) Then LandesVW = LandesVW.Remove(0, 2)
        If LandesVW.StartsWith("0") Then LandesVW = LandesVW.Remove(0, 1)
        If TelNr.StartsWith(DataProvider.P_Def_PreLandesVW) Then TelNr = TelNr.Remove(0, 2)
        If TelNr.StartsWith("0") Then TelNr = TelNr.Remove(0, 1)
        Do
            i += 1
            Suchmuster = LandesVW & ";" & Strings.Left(TelNr, i) & ";*"
            Dim Trefferliste = From s In Vorwahlen Where s.ToLower Like Suchmuster.ToLower Select s
            Windows.Forms.Application.DoEvents()
            tmpvorwahl = Split(Trefferliste(0), ";", , CompareMethod.Text)
            If Not tmpvorwahl.Length = 1 Then AuslandsVorwahlausDatei = tmpvorwahl(1)
        Loop Until i = 5
        'Loop Until Not AuslandsVorwahlausDatei = DataProvider.P_Def_StringEmpty Or i = 5
    End Function

    ''' <summary>
    ''' Bereinigt die Telefunnummer von Sonderzeichen wie Klammern und Striche.
    ''' Buchstaben werden wie auf der Telefontastatur in Zahlen übertragen.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer mit Sonderzeichen</param>
    ''' <returns>saubere Telefonnummer (nur aus Ziffern bestehend)</returns>
    ''' <remarks>Achtung! "*", "#" bleiben Bestehen!!!</remarks>
    Public Function nurZiffern(ByVal TelNr As String) As String
        Dim i As Integer   ' Zählvariable
        Dim c As String ' einzelnes Zeichen

        nurZiffern = DataProvider.P_Def_LeerString
        TelNr = UCase(TelNr)

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
                    nurZiffern = nurZiffern + DataProvider.P_Def_PreLandesVW
            End Select
        Next
        ' Landesvorwahl entfernen bei Inlandsgesprächen (einschließlich nachfolgender 0)
        If Left(nurZiffern, Len(C_DP.P_TBLandesVW)) = C_DP.P_TBLandesVW Then
            nurZiffern = Replace(nurZiffern, C_DP.P_TBLandesVW & "0", "0", , 1)
            nurZiffern = Replace(nurZiffern, C_DP.P_TBLandesVW, "0", , 1)
        End If

        ' Bei diversen VoIP-Anbietern werden 2 führende Nullen zusätzlich gewählt: Entfernen "000" -> "0"
        If nurZiffern.StartsWith("000") Then nurZiffern = Right(nurZiffern, Len(nurZiffern) - 2)
    End Function '(nurZiffern)

    ''' <summary>
    ''' Gibt zurück ob eine Telefonnummer eine Mobilnummer ist.
    ''' </summary>
    ''' <param name="TelNr">Die zu prüfende Telefonnummer.</param>
    ''' <returns>True, wenn es sich um eine Mobilnummer handelt.</returns>
    Public Function Mobilnummer(ByVal TelNr As String) As Boolean
        Dim TempTelNr As String() = TelNrTeile(TelNr)
        Dim Vorwahl As String = Left(TempTelNr(1), 2)

        Return (TempTelNr(0) = C_DP.P_TBLandesVW Or TempTelNr(0) = DataProvider.P_Def_LeerString) And (Vorwahl.StartsWith("15") Or Vorwahl.StartsWith("16") Or Vorwahl.StartsWith("17"))
    End Function

    ''' <summary>
    ''' Vergleicht zwei Telefonnummern auf Gleichheit. die Formatierung wird ignoriert. 
    ''' </summary>
    ''' <param name="TelNr1">Erste zu vergleichende Telefonnummer.</param>
    ''' <param name="TelNr2">Zweite zu vergleichende Telefonnummer.</param>
    ''' <returns>True, wenn gleich, False wenn nicht gleich.</returns>
    Public Function TelNrVergleich(ByVal TelNr1 As String, ByVal TelNr2 As String) As Boolean
        Return nurZiffern(TelNr1).Equals(nurZiffern(TelNr2))
    End Function

    Public Function VorwahlListe(ByVal VList As Vorwahllisten) As String
        Select Case VList
            Case Vorwahllisten.Liste_Landesvorwahlen
                VorwahlListe = My.Resources.Liste_Landesvorwahlen
            Case Vorwahllisten.Liste_Ortsvorwahlen_Ausland
                VorwahlListe = My.Resources.Liste_Ortsvorwahlen_Ausland
            Case Vorwahllisten.Liste_Ortsvorwahlen_Deutschland
                VorwahlListe = My.Resources.Liste_Ortsvorwahlen_Deutschland
            Case Else
                VorwahlListe = DataProvider.P_Def_ErrorMinusOne_String
        End Select
    End Function


#End Region

#Region " HTTPTransfer"
    Public Function httpGET(ByVal Link As String, ByVal Encoding As System.Text.Encoding, ByRef FBError As Boolean) As String
        Dim UniformResourceIdentifier As New Uri(Link)

        httpGET = DataProvider.P_Def_LeerString

        Select Case UniformResourceIdentifier.Scheme
            Case Uri.UriSchemeHttp
                If DataProvider.P_Debug_Use_WebClient Then
                    Dim webClient As New WebClient
                    With webClient
                        .Encoding = Encoding
                        .Proxy = Nothing
                        .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)
                        .Headers.Add(HttpRequestHeader.KeepAlive, "False")
                        Try
                            httpGET = .DownloadString(UniformResourceIdentifier)
                            FBError = False
                        Catch exANE As ArgumentNullException
                            FBError = True
                            LogFile("httpGET_WebClient: " & exANE.Message)
                        Catch exWE As WebException
                            FBError = True
                            LogFile("httpGET_WebClient: " & exWE.Message & " - Link: " & Link)
                        End Try
                    End With
                Else
                    With CType(HttpWebRequest.Create(UniformResourceIdentifier), HttpWebRequest)
                        .Method = WebRequestMethods.Http.Get
                        .Proxy = Nothing
                        .KeepAlive = False
                        .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)
                        Try
                            With New IO.StreamReader(.GetResponse().GetResponseStream(), Encoding)
                                FBError = False
                                httpGET = .ReadToEnd()
                                .Close()
                            End With
                        Catch exANE As ArgumentNullException
                            FBError = True
                            LogFile("httpGET_Stream (ArgumentNullException): " & exANE.Message)
                        Catch exWE As WebException
                            FBError = True
                            LogFile("httpGET_Stream (WebException): " & exWE.Message & " - Link: " & Link)
                        End Try
                    End With
                End If
            Case Uri.UriSchemeFile
                With My.Computer.FileSystem
                    If .FileExists(Link) Then
                        httpGET = .ReadAllText(Link, Encoding)
                    Else
                        LogFile("Datei kann nicht gefunden werden: " & Link)
                        FBError = True
                    End If
                End With
            Case Else
                LogFile("Uri.Scheme: " & UniformResourceIdentifier.Scheme)
                FBError = True
        End Select

    End Function

    Public Function httpPOST(ByVal Link As String, ByVal Daten As String, ByVal ZeichenCodierung As System.Text.Encoding) As String
        httpPOST = DataProvider.P_Def_LeerString
        Dim UniformResourceIdentifier As New Uri(Link)
        If UniformResourceIdentifier.Scheme = Uri.UriSchemeHttp Then
            If DataProvider.P_Debug_Use_WebClient Then
                Dim webClient As New WebClient
                With webClient
                    .Encoding = ZeichenCodierung
                    .Proxy = Nothing
                    .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)

                    With .Headers
                        .Add(HttpRequestHeader.ContentLength, Daten.Length.ToString)
                        .Add(HttpRequestHeader.UserAgent, DataProvider.P_Def_Header_UserAgent)
                        .Add(HttpRequestHeader.KeepAlive, "True")
                        .Add(HttpRequestHeader.Accept, DataProvider.P_Def_Header_Accept)
                    End With

                    Try
                        httpPOST = .UploadString(UniformResourceIdentifier, Daten)
                    Catch exANE As ArgumentNullException
                        LogFile("httpPOST_WebClient: " & exANE.Message)
                    Catch exWE As WebException
                        LogFile("httpPOST_WebClient: " & exWE.Message & " - Link: " & Link)
                    End Try
                End With
            Else
                With CType(HttpWebRequest.Create(UniformResourceIdentifier), HttpWebRequest)
                    .Method = WebRequestMethods.Http.Post
                    .Proxy = Nothing
                    .KeepAlive = True
                    .ContentLength = Encoding.UTF8.GetBytes(Daten).Length
                    .ContentType = DataProvider.P_Def_Header_ContentType
                    .Accept = DataProvider.P_Def_Header_Accept
                    .UserAgent = DataProvider.P_Def_Header_UserAgent
                    .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)

                    Try

                        With New IO.StreamWriter(.GetRequestStream)
                            .Write(Daten)
                            ThreadSleep(100)
                            .Close()
                        End With

                        With New IO.StreamReader(CType(.GetResponse, HttpWebResponse).GetResponseStream(), ZeichenCodierung)
                            httpPOST = .ReadToEnd()
                            'ThreadSleep(1000)
                            .Close()
                        End With
                    Catch exPVE As ProtocolViolationException
                        LogFile("httpPOST_Stream: " & exPVE.Message & " - Link: " & .ContentLength)
                    Catch exANE As ArgumentNullException
                        LogFile("httpPOST_Stream: " & exANE.Message)
                    Catch exWE As WebException
                        LogFile("httpPOST_Stream: " & exWE.Message & " - Link: " & Link)
                    End Try
                End With
            End If
        End If
    End Function
#End Region

#Region " Timer"
    Public Function SetTimer(ByRef Interval As Double) As System.Timers.Timer
        Dim aTimer As New System.Timers.Timer

        With aTimer
            .Interval = Interval
            .AutoReset = True
            .Enabled = True
        End With
        Return aTimer

    End Function

    Public Function KillTimer(ByVal Timer As System.Timers.Timer) As System.Timers.Timer
        If Timer IsNot Nothing Then
            With Timer
                .Stop()
                .AutoReset = False
                .Enabled = False
                .Dispose()
            End With
        End If
        Return Nothing
    End Function
#End Region

#Region "Threads"
    Sub ThreadSleep(ByRef Dauer As Integer)
        Thread.Sleep(Dauer)
    End Sub
#End Region

    Public Overloads Function ByteArrayToString(ByVal ByteArray As Byte()) As String
        'Dim hex As StringBuilder = New StringBuilder(ByteArray.Length * 2)
        'For Each b As Byte In ByteArray
        '    hex.AppendFormat("{0:x2}", b)
        'Next
        'Return hex.ToString()
        Return System.Text.Encoding.UTF8.GetString(ByteArray)
    End Function

    Public Function GetTimeInterval(ByVal nSeks As Double) As String
        'http://www.vbarchiv.net/faq/date_sectotime.php
        Dim h As Double, m As Double
        h = nSeks / 3600
        nSeks = nSeks Mod 3600
        m = nSeks / 60
        nSeks = nSeks Mod 60
        Return Format(h, DataProvider.P_Def_PreLandesVW) & ":" & Format(m, DataProvider.P_Def_PreLandesVW) & ":" & Format(nSeks, DataProvider.P_Def_PreLandesVW)
    End Function

    Public Function AcceptOnlyNumeric(ByVal sTxt As String) As String
        If sTxt = DataProvider.P_Def_LeerString Then Return DataProvider.P_Def_LeerString
        If Mid(sTxt, Len(sTxt), 1) Like "[0-9]" = False Then
            Return Mid(sTxt, 1, Len(sTxt) - 1)
        End If
        Return sTxt
    End Function

    Public Function TelefonName(ByVal MSN As String) As String
        TelefonName = DataProvider.P_Def_LeerString
        If Not MSN = DataProvider.P_Def_LeerString Then
            Dim xPathTeile As New ArrayList
            With xPathTeile
                .Add("Telefone")
                .Add("Telefone")
                .Add("*")
                .Add("Telefon")
                .Add("[contains(TelNr, """ & MSN & """) and not(@Dialport > 599)]") ' Keine Anrufbeantworter
                .Add("TelName")
            End With
            TelefonName = Replace(C_XML.Read(C_DP.XMLDoc, xPathTeile, ""), ";", ", ")
            xPathTeile = Nothing
        End If
    End Function
End Class
