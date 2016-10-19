Imports System.Net
Imports System.Text
Imports System.Threading
Imports System.IO
Imports System.Collections.ObjectModel

Public Class Helfer
    Private Const Epsilon As Single = Single.Epsilon

    Private C_XML As XML
    Private C_DP As DataProvider
    Private C_Crypt As Rijndael

    Public Structure Telefonnummer
#Region "Datenfelder"
        Private sTelNr As String
        ''' <summary>
        ''' Die komplette unformatierte Telefonnummer 
        ''' </summary>
        Public Property TelNr() As String
            Get
                Return sTelNr
            End Get
            Set(ByVal value As String)
                sTelNr = value
            End Set
        End Property

        Private sOrtsvorwahl As String
        ''' <summary>
        ''' Die Ortsvorwahl der Telefonnummer
        ''' </summary>
        Public Property Ortsvorwahl As String
            Get
                Return sOrtsvorwahl
            End Get
            Set(value As String)
                sOrtsvorwahl = value
            End Set
        End Property

        Private sLandesvorwahl As String
        ''' <summary>
        ''' Die Landesvorwahl der Telefonnummer
        ''' </summary>
        Public Property Landesvorwahl As String
            Get
                Return sLandesvorwahl
            End Get
            Set(value As String)
                sLandesvorwahl = value
            End Set
        End Property

        Private sDurchwahl As String
        ''' <summary>
        ''' Die Durchwahl der Telefonnummer
        ''' </summary>
        Public Property Durchwahl As String
            Get
                Return sDurchwahl
            End Get
            Set(value As String)
                sDurchwahl = value
            End Set
        End Property

        Private sNummer As String
        ''' <summary>
        ''' Der Rest der Nummer, wenn alle bekannten Teile abgeschnitten wurden
        ''' </summary>
        Public Property Nummer As String
            Get
                Return sNummer
            End Get
            Set(value As String)
                sNummer = value
            End Set
        End Property
#End Region
    End Structure

#Region "Vergleichsmodi"
    Friend Enum Vergleichsmodus
        KleinerGleich = -2
        Kleiner = -1
        Gleich = 0
        Größer = 1
        GrößerGleich = 2
    End Enum
#End Region

    Public Sub New(ByVal DataProviderKlasse As DataProvider, ByVal CryptKlasse As Rijndael, XMLKlasse As XML)
        C_XML = XMLKlasse
        C_DP = DataProviderKlasse
        C_Crypt = CryptKlasse
    End Sub

#Region "String Behandlung"
    ''' <summary>
    ''' Entnimmt aus dem String <c>Text</c> einen enthaltenen Sub-String ausgehend von einer Zeichenfolge davor <c>StringDavor</c> 
    ''' und deiner Zeichenfolge danach <c>StringDanach</c>.
    ''' </summary>
    ''' <param name="Text">String aus dem der Sub-String entnommen werden soll.</param>
    ''' <param name="StringDavor">Zeichenfolge vor dem zu entnehmenden Sub-String.</param>
    ''' <param name="StringDanach">Zeichenfolge nach dem zu entnehmenden Sub-String.</param>
    ''' <param name="Reverse">Flag, Ob die Suche nach den Zeichenfolgen vor und nach dem Sub-String vom Ende des <c>Textes</c> aus begonnen werden soll.</param>
    ''' <returns>Wenn <c>StringDavor</c> und <c>StringDanach</c> enthalten sind, dann wird der Teilstring zurückgegeben. Ansonsten "-1".</returns>
    Public Overloads Function StringEntnehmen(ByVal Text As String, ByVal StringDavor As String, ByVal StringDanach As String, ByVal Reverse As Boolean) As String
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
    ''' </summary>
    ''' <param name="Text">String aus dem der Sub-String entnommen werden soll.</param>
    ''' <param name="StringDavor">Zeichenfolge vor dem zu entnehmenden Sub-String.</param>
    ''' <param name="StringDanach">Zeichenfolge nach dem zu entnehmenden Sub-String.</param>
    ''' <returns>Wenn <c>StringDavor</c> und <c>StringDanach</c> enthalten sind, dann wird der Teilstring zurückgegeben. Ansonsten "-1".</returns>
    Public Overloads Function StringEntnehmen(ByVal Text As String, ByVal StringDavor As String, ByVal StringDanach As String) As String
        Return StringEntnehmen(Text, StringDavor, StringDanach, False)
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

    '''' <summary>
    '''' Prüft ob, ein String <c>A</c> in einem Sting-Array <c>B</c> enthalten ist. 
    '''' </summary>
    '''' <param name="A">Zu prüfender String.</param>
    '''' <param name="B">Array in dem zu prüfen ist.</param>
    '''' <returns><c>True</c>, wenn enthalten, <c>False</c>, wenn nicht.</returns>
    '<DebuggerStepThrough>
    'Public Function IsOneOf(ByVal A As String, ByVal B() As String) As Boolean
    '    Return B.Contains(A)
    '    'Return IIf(CheckIsZero((From Strng In B Where Strng = A).ToArray.Count), False, True)
    'End Function
#End Region

    ''' <summary>
    ''' Dekrementiert den Verweiszähler des dem angegebenen COM-Objekt zugeordneten angegebenen Runtime Callable Wrapper (RCW)
    ''' </summary>
    ''' <param name="COMObject">Das freizugebende COM-Objekt.</param>
    <DebuggerStepThrough>
    Public Sub NAR(ByVal COMObject As Object)

        If COMObject IsNot Nothing Then
            Try
                Runtime.InteropServices.Marshal.ReleaseComObject(COMObject)
            Catch ex As ArgumentException
                MsgBox("COM-Object ist kein gültiges COM-Objekt: " & ex.Message, MsgBoxStyle.Critical, "NAR")
            Finally
                COMObject = Nothing
            End Try
        End If
    End Sub

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

    <DebuggerStepThrough>
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

    ''' <summary>
    ''' Gibt die Zeichenkodierung der Fritz!Box-Oberfläche Zurück. Dies sollte im Standardfall UTF-8 sein. 
    ''' </summary>
    ''' <param name="Quelltext">Der Quelltext einer Seite der Fritz!Box-Oberfläche</param>
    ''' <returns>Die Zeichencodierung als <c>System.Text.Encoding</c></returns>
    Public Function GetEncoding(ByVal Quelltext As String) As Encoding
        If Quelltext.Contains("charset=utf-8") Then
            ' Schnelle Weg für den Standardfall
            GetEncoding = DataProvider.P_Def_EncodingFritzBox ' Standard: UTF-8
        Else
            ' Ermittle die Zeichencodierung
            Dim CodePageName As String
            ' Extrahiere den Codepage-Namen der Codierung (Entspricht dem Header-Name). 
            CodePageName = StringEntnehmen(Quelltext.Replace("""", "'"), "charset=", "'")
            Try
                GetEncoding = Encoding.GetEncoding(CodePageName)
            Catch ex As ArgumentException
                GetEncoding = DataProvider.P_Def_EncodingFritzBox
                LogFile("Die Codierung " & CodePageName & " kann nicht verarbeitet werden.")
            End Try
        End If

    End Function

    Public Function MsgBox(ByVal Meldung As String, ByVal Style As MsgBoxStyle, ByVal Aufruf As String) As MsgBoxResult
        If Style = MsgBoxStyle.Critical Or Style = MsgBoxStyle.Exclamation Then
            Meldung = "Die Funktion " & Aufruf & " meldet folgenden Fehler:" & vbCrLf & vbCrLf & Meldung
            LogFile(Meldung)
        End If
        Return Microsoft.VisualBasic.MsgBox(Meldung, Style, DataProvider.P_Def_Addin_LangName) '"Fritz!Box Telefon-Dingsbums"
    End Function

    ''' <summary>
    ''' Diese Routine ändert den Zugang zu den verschlüsselten Passwort.
    ''' </summary>
    Public Sub KeyChange()
        Dim AlterZugang As String
        Dim NeuerZugang As String
        If Not C_DP.P_TBPasswort = DataProvider.P_Def_LeerString And Not C_DP.P_TBPasswort = DataProvider.P_Def_ErrorMinusOne_String Then
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

        If Not C_DP.P_TBPhonerPasswort = DataProvider.P_Def_LeerString And Not C_DP.P_TBPhonerPasswort = DataProvider.P_Def_ErrorMinusOne_String Then
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

    ''' <summary>
    ''' Wandelt eine Zeitspanne in Sekunden in ein Format in Stunden:Minuten:Sekunden um
    ''' </summary>
    ''' <param name="nSeks">Sekunden der Zeitspanne</param>
    Public Function GetTimeInterval(ByVal nSeks As Double) As String
        'http://www.vbarchiv.net/faq/date_sectotime.php
        Dim h As Double, m As Double
        h = nSeks / 3600
        nSeks = nSeks Mod 3600
        m = nSeks / 60
        nSeks = nSeks Mod 60
        Return Format(h, "00") & ":" & Format(m, "00") & ":" & Format(nSeks, "00")
    End Function

    ''' <summary>
    ''' Gibt nur die Numerischen Ziffen eines String zurück
    ''' </summary>
    ''' <param name="sTxt">String der umgewandelt werden soll</param>
    Public Function AcceptOnlyNumeric(ByVal sTxt As String) As String

        Dim regex As RegularExpressions.Regex = New RegularExpressions.Regex("^[+-]?\d+$")
        Dim match As RegularExpressions.Match = regex.Match(sTxt)
        If match.Success Then
            AcceptOnlyNumeric = match.Value
        Else
            AcceptOnlyNumeric = DataProvider.P_Def_LeerString
        End If

        match = Nothing
        regex = Nothing
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

    ''' <summary>
    ''' Entfernt doppelte und leere Einträge aus einem String-Array.
    ''' </summary>
    ''' <param name="ArraytoClear">Das zu bereinigende Array</param>
    ''' <param name="ClearDouble">Angabe, ob doppelte Einträge entfernt werden sollen.</param>
    ''' <param name="ClearEmpty">Angabe, ob leere Einträge entfernt werden sollen.</param>
    ''' <param name="ClearMinusOne">Angabe, ob Einträge mit dem Wert -1 entfernt werden sollen.</param>
    ''' <returns>Das bereinigte String-Array</returns>
    ''' <remarks></remarks>
    Public Function ClearStringArray(ByVal ArraytoClear As String(), ByVal ClearDouble As Boolean, ByVal ClearEmpty As Boolean, ByVal ClearMinusOne As Boolean) As String()
        ' Doppelte entfernen
        If ClearDouble Then ArraytoClear = (From x In ArraytoClear Select x Distinct).ToArray
        ' Leere entfernen
        If ClearEmpty Then ArraytoClear = (From x In ArraytoClear Where Not x Like DataProvider.P_Def_LeerString Select x).ToArray
        ' -1 entfernen
        If ClearMinusOne Then ArraytoClear = (From x In ArraytoClear Where Not x Like DataProvider.P_Def_ErrorMinusOne_String Select x).ToArray

        Return ArraytoClear
    End Function

#Region "Überladene .NET Funktionen"
#Region "IIF"
    ''' <summary>
    ''' Überladene .NET-Funktion: Gibt abhängig von der Auswertung eines Ausdrucks eines von zwei Boolean zurück.
    ''' </summary>
    ''' <param name="Expression">Erforderlich. Boolean-Datentyp. Der Ausdruck, der ausgewertet werden soll.</param>
    ''' <param name="TruePart">Erforderlich. Boolean. Wird zurückgegeben, wenn Expression <c>True</c> ergibt.</param>
    ''' <param name="FalsePart">Erforderlich. Boolean. Wird zurückgegeben, wenn Expression <c>False</c> ergibt.</param>
    ''' <returns>Gibt abhängig von der Auswertung eines Ausdrucks eines von zwei Objekten zurück.</returns>
    ''' <remarks>https://msdn.microsoft.com/de-de/library/27ydhh0d(v=vs.90).aspx</remarks>
    <DebuggerStepThrough>
    Public Overloads Function IIf(ByVal Expression As Boolean, ByVal TruePart As Boolean, ByVal FalsePart As Boolean) As Boolean
        If Expression Then
            Return TruePart
        Else
            Return FalsePart
        End If
    End Function

    ''' <summary>
    ''' Überladene .NET-Funktion: Gibt abhängig von der Auswertung eines Ausdrucks eines von zwei Integer zurück. 
    ''' </summary>
    ''' <param name="Expression">Erforderlich. Boolean-Datentyp. Der Ausdruck, der ausgewertet werden soll.</param>
    ''' <param name="TruePart">Erforderlich. Integer. Wird zurückgegeben, wenn Expression <c>True</c> ergibt.</param>
    ''' <param name="FalsePart">Erforderlich. Integer. Wird zurückgegeben, wenn Expression <c>False</c> ergibt.</param>
    ''' <returns>Gibt abhängig von der Auswertung eines Ausdrucks eines von zwei Objekten zurück.</returns>
    ''' <remarks>https://msdn.microsoft.com/de-de/library/27ydhh0d(v=vs.90).aspx</remarks>
    <DebuggerStepThrough>
    Public Overloads Function IIf(ByVal Expression As Boolean, ByVal TruePart As Integer, ByVal FalsePart As Integer) As Integer
        If Expression Then
            Return TruePart
        Else
            Return FalsePart
        End If
    End Function

    ''' <summary>
    ''' Überladene .NET-Funktion: Gibt abhängig von der Auswertung eines Ausdrucks eines von zwei Strings zurück. 
    ''' </summary>
    ''' <param name="Expression">Erforderlich. Boolean-Datentyp. Der Ausdruck, der ausgewertet werden soll.</param>
    ''' <param name="TruePart">Erforderlich. String. Wird zurückgegeben, wenn Expression <c>True</c> ergibt.</param>
    ''' <param name="FalsePart">Erforderlich. String. Wird zurückgegeben, wenn Expression <c>False</c> ergibt.</param>
    ''' <returns>Gibt abhängig von der Auswertung eines Ausdrucks eines von zwei Objekten zurück.</returns>
    ''' <remarks>https://msdn.microsoft.com/de-de/library/27ydhh0d(v=vs.90).aspx</remarks>
    <DebuggerStepThrough>
    Public Overloads Function IIf(ByVal Expression As Boolean, ByVal TruePart As String, ByVal FalsePart As String) As String
        If Expression Then
            Return TruePart
        Else
            Return FalsePart
        End If
    End Function

    ''' <summary>
    ''' Überladene .NET-Funktion: Gibt abhängig von der Auswertung eines Ausdrucks eines von zwei System.Drawing.Color zurück.  
    ''' </summary>
    ''' <param name="Expression">Erforderlich. Boolean-Datentyp. Der Ausdruck, der ausgewertet werden soll.</param>
    ''' <param name="TruePart">Erforderlich. System.Drawing.Color. Wird zurückgegeben, wenn Expression <c>True</c> ergibt.</param>
    ''' <param name="FalsePart">Erforderlich. System.Drawing.Color. Wird zurückgegeben, wenn Expression <c>False</c> ergibt.</param>
    ''' <returns>Gibt abhängig von der Auswertung eines Ausdrucks eines von zwei Objekten zurück.</returns>
    ''' <remarks>https://msdn.microsoft.com/de-de/library/27ydhh0d(v=vs.90).aspx</remarks>
    <DebuggerStepThrough>
    Public Overloads Function IIf(ByVal Expression As Boolean, ByVal TruePart As Drawing.Color, ByVal FalsePart As Drawing.Color) As Drawing.Color
        If Expression Then
            Return TruePart
        Else
            Return FalsePart
        End If
    End Function

    ''' <summary>
    ''' Überladene .NET-Funktion: Gibt abhängig von der Auswertung eines Ausdrucks eines von zwei Double zurück.
    ''' </summary>
    ''' <param name="Expression">Erforderlich. Boolean-Datentyp. Der Ausdruck, der ausgewertet werden soll.</param>
    ''' <param name="TruePart">Erforderlich. Double. Wird zurückgegeben, wenn Expression <c>True</c> ergibt.</param>
    ''' <param name="FalsePart">Erforderlich. Double. Wird zurückgegeben, wenn Expression <c>False</c> ergibt.</param>
    ''' <returns>Gibt abhängig von der Auswertung eines Ausdrucks eines von zwei Objekten zurück.</returns>
    ''' <remarks>https://msdn.microsoft.com/de-de/library/27ydhh0d(v=vs.90).aspx</remarks>
    <DebuggerStepThrough>
    Public Overloads Function IIf(ByVal Expression As Boolean, ByVal TruePart As Double, ByVal FalsePart As Double) As Double
        If Expression Then
            Return TruePart
        Else
            Return FalsePart
        End If
    End Function
#End Region
#End Region

#Region "Telefonnummern formatieren"
    ''' <summary>
    ''' Formatiert die Telefonnummern nach gängigen Regeln
    ''' </summary>
    ''' <param name="TelNr">Die zu formatierende Telefonnummer</param>
    ''' <returns>Die formatierte Telefonnummer</returns>
    Function FormatTelNr(ByVal TelNr As String) As String

        Dim TelTeile As Telefonnummer = TelNrTeile(TelNr)
        Dim TelNrGruppieren As Boolean

        FormatTelNr = C_DP.P_TBTelNrMaske
        With TelTeile

            ' Maske Prüfen
            'If InStr(FormatTelNr, "%D", CompareMethod.Text) = 0 Then FormatTelNr = Replace(FormatTelNr, "%N", "%N%D")
            If Not FormatTelNr.Contains("%D") Then FormatTelNr = Replace(FormatTelNr, "%N", "%N%D")

            'If Not InStr(FormatTelNr, "%N%D", CompareMethod.Text) = 0 Then
            If FormatTelNr.Contains("%N%D") Then
                .Nummer = .Nummer & .Durchwahl
                .Durchwahl = DataProvider.P_Def_LeerString
            End If

            ' Setze die Ortsvorwahl, wenn immer eine internale Nummer erzeugt werden soll UND
            '                        wenn die Landesvorwahl der Nummer leer ist ODER gleich der eigestellten Landesvorwahl ist UND
            '                        die Ortsvorwahl nicht vorhanden ist
            If (.Landesvorwahl = C_DP.P_TBLandesVW Or .Landesvorwahl = DataProvider.P_Def_LeerString) And C_DP.P_CBintl And .Ortsvorwahl = DataProvider.P_Def_LeerString Then
                .Ortsvorwahl = C_DP.P_TBVorwahl
            End If

            If .Ortsvorwahl = DataProvider.P_Def_LeerString Then
                ' Maske %L (%O) %N - %D
                ' Wenn keine Ortsvorwahl vorhanden ist, dann muss diese bei der Formatierung nicht berücksichtigt werden.
                ' Keine Ortsvorwahl: Alles zwischen %L und %N entfernen
                Dim pos1 As Integer
                Dim pos2 As Integer
                Dim CutOut As String
                pos1 = InStr(FormatTelNr, "%L", CompareMethod.Text) + 2
                pos2 = InStr(FormatTelNr, "%N", CompareMethod.Text)
                CutOut = Mid(FormatTelNr, pos1, pos2 - pos1)
                FormatTelNr = Replace(FormatTelNr, CutOut, CStr(IIf(CutOut.StartsWith(" "), " ", DataProvider.P_Def_LeerString)), , 1, CompareMethod.Text)
            End If

            If C_DP.P_CBintl Then
                ' Eine Ortsvorwahl muss vorhanden sein
                If .Ortsvorwahl = DataProvider.P_Def_LeerString Then .Ortsvorwahl = C_DP.P_TBVorwahl
                ' Die Landesvorwahl muss gesetzt sein
                If .Landesvorwahl = DataProvider.P_Def_LeerString Then .Landesvorwahl = C_DP.P_TBLandesVW
            Else
                If .Landesvorwahl = C_DP.P_TBLandesVW Or .Landesvorwahl Is Nothing Then
                    ' Wenn die internationale Vorwahl nicht vorangestellt werden soll, dann füge eine Null an die Ortsvorwahl
                    .Ortsvorwahl = IIf(.Ortsvorwahl.StartsWith("0"), .Ortsvorwahl, "0" & .Ortsvorwahl)
                    ' Die Landesvorwahl kann entfernt werden, wenn sie mit der Vorwahl in den Einstellungen übereinstimmt
                    .Landesvorwahl = DataProvider.P_Def_LeerString
                End If
            End If

            ' Ersetze 00 mit einem +
            If .Landesvorwahl.StartsWith(DataProvider.P_Def_PreLandesVW) Then .Landesvorwahl = Replace(.Landesvorwahl, DataProvider.P_Def_PreLandesVW, "+", 1, 1, CompareMethod.Text)

            TelNrGruppieren = C_DP.P_CBTelNrGruppieren
            ' NANP
            If .Landesvorwahl = "+1" Then
                FormatTelNr = "%L (%O) %N-%D"
                TelNrGruppieren = False
                If .Durchwahl = DataProvider.P_Def_LeerString Then
                    .Durchwahl = Mid(.Nummer, 4)
                    .Nummer = Left(.Nummer, 3)
                End If
            End If

            If TelNrGruppieren Then
                .Ortsvorwahl = GruppiereNummer(.Ortsvorwahl)
                .Nummer = GruppiereNummer(.Nummer)
                .Durchwahl = GruppiereNummer(.Durchwahl)
            End If

            ' formatTelNr zusammenstellen
            .Nummer = Replace(.Nummer, "  ", " ", , , CompareMethod.Text)
            ' Maske %L (%O) %N - %D
            FormatTelNr = Replace(FormatTelNr, "%L", .Landesvorwahl)
            FormatTelNr = Replace(FormatTelNr, "%O", .Ortsvorwahl)
            FormatTelNr = Replace(FormatTelNr, "%N", .Nummer)
            If Not .Durchwahl = DataProvider.P_Def_LeerString Then
                FormatTelNr = Replace(FormatTelNr, "%D", .Durchwahl)
            Else
                FormatTelNr = Left(FormatTelNr, InStr(FormatTelNr, .Nummer, CompareMethod.Text) + Len(.Nummer) - 1)
            End If
            FormatTelNr = Trim(Replace(FormatTelNr, "  ", " ", , , CompareMethod.Text))

        End With
        Return FormatTelNr
    End Function

    Function GruppiereNummer(ByVal Nr As String) As String
        Dim imax As Integer
        imax = CInt(Math.Round(Len(Nr) / 2 + 0.1))
        GruppiereNummer = DataProvider.P_Def_LeerString
        For i = 1 To imax
            GruppiereNummer = Right(Nr, 2) & DataProvider.P_Def_Leerzeichen & GruppiereNummer
            If Not Len(Nr) = 1 Then Nr = Left(Nr, Len(Nr) - 2)
        Next
        Return Trim(GruppiereNummer)
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

        Dim tmpTelNrTeile As Telefonnummer
        'Dim tmpLandesVorwahl As String
        'Dim tmpOrtsVorwahl As String


        If Not TelNr = DataProvider.P_Def_LeerString Then
            tmpTelNrTeile = TelNrTeile(TelNr)

            With tmpTelNrTeile
                If .Ortsvorwahl = IIf(C_DP.P_TBVorwahl.StartsWith("0"), C_DP.P_TBVorwahl.Remove(0, 1), C_DP.P_TBVorwahl) Then
                    TelNr = .Nummer & .Durchwahl
                End If
            End With
            '' Nummer korrigieren, falls diese mit der Landes- und Ortsvorwahl ohne führende "00" beginnt.
            'tmpLandesVorwahl = C_DP.P_TBLandesVW
            'tmpOrtsVorwahl = C_DP.P_TBVorwahl
            'If tmpLandesVorwahl.StartsWith(DataProvider.P_Def_PreLandesVW) Then tmpLandesVorwahl = tmpLandesVorwahl.Remove(0, 2)
            'If tmpOrtsVorwahl.StartsWith("0") Then tmpOrtsVorwahl = tmpOrtsVorwahl.Remove(0, 1)

            'If TelNr.StartsWith(tmpLandesVorwahl & tmpOrtsVorwahl) Then TelNr = TelNr.Insert(0, "+")

            'tmpTelNrTeile = TelNrTeile(TelNr)

            '' Landesvorwahl entfernen
            'If tmpTelNrTeile.Landesvorwahl = tmpLandesVorwahl.Insert(0, DataProvider.P_Def_PreLandesVW) Then TelNr = TelNr.Remove(0, Len(tmpTelNrTeile.Landesvorwahl))

            '' Ortsvorwahl entfernen
            'If tmpTelNrTeile.Ortsvorwahl = tmpOrtsVorwahl And Not Mobilnummer(TelNr) Then
            '    TelNr = TelNr.Remove(0, Len(tmpTelNrTeile.Ortsvorwahl) + CInt(IIf(TelNr.StartsWith("0"), 1, 0)))
            'End If

        End If

        Return TelNr
    End Function

    '''' <summary>
    '''' TelNr bereinigen
    '''' </summary>
    '''' <param name="TelNr"></param>
    'Private Sub TelNrBereinigen(ByRef TelNr As String)

    '    'TelNr = Replace(TelNr, "(0)", " ", , , CompareMethod.Text)
    '    'TelNr = Replace(TelNr, "++", "00", , , CompareMethod.Text)
    '    'TelNr = Replace(TelNr, "+ ", "+", , , CompareMethod.Text)
    '    'TelNr = Replace(TelNr, "+", "00", , , CompareMethod.Text)
    '    'TelNr = Replace(TelNr, "[", "(", , , CompareMethod.Text)
    '    'TelNr = Replace(TelNr, "]", ")", , , CompareMethod.Text)
    '    'TelNr = Replace(TelNr, "{", "(", , , CompareMethod.Text)
    '    'TelNr = Replace(TelNr, "[", ")", , , CompareMethod.Text)
    '    'TelNr = Replace(TelNr, "#", "", , , CompareMethod.Text)
    '    'TelNr = Replace(TelNr, " ", "", , , CompareMethod.Text)

    '    TelNr = TelNr.Replace("(0)", " ")
    '    TelNr = TelNr.Replace("++", DataProvider.P_Def_PreLandesVW)
    '    TelNr = TelNr.Replace("+ ", "+")
    '    TelNr = TelNr.Replace("+", DataProvider.P_Def_PreLandesVW)
    '    TelNr = TelNr.Replace("[", "(")
    '    TelNr = TelNr.Replace("]", ")")
    '    TelNr = TelNr.Replace("{", "(")
    '    TelNr = TelNr.Replace("[", ")")
    '    TelNr = TelNr.Replace("#", "")
    '    TelNr = TelNr.Replace(" ", "")
    'End Sub

    ''' <summary>
    ''' Zerlegt die Telefonnummer in ihre Bestandteile.
    ''' </summary>
    ''' <param name="TelNr">Zu zerlegende Telefonnummer.</param>
    ''' <returns>Telefonnummer als eigener Datentyp</returns>
    Function TelNrTeile(ByVal TelNr As String) As Telefonnummer

        Dim retVal As New Telefonnummer
        Dim ErsteZiffer As String

        With retVal
            .TelNr = TelNr
            .Landesvorwahl = DataProvider.P_Def_LeerString
            .Nummer = DataProvider.P_Def_LeerString
            .Ortsvorwahl = DataProvider.P_Def_LeerString
            .Durchwahl = DataProvider.P_Def_LeerString

            If Not TelNr = DataProvider.P_Def_LeerString Then
                TelNr = nurZiffern(TelNr)

                If TelNr.StartsWith(DataProvider.P_Def_PreLandesVW) Then
                    'Landesvorwahl vorhanden
                    If TelNr.StartsWith(C_DP.P_TBLandesVW) Then
                        .Landesvorwahl = C_DP.P_TBLandesVW
                    Else
                        .Landesvorwahl = VorwahlausDatei(TelNr, DataProvider.P_Def_LeerString, C_DP.P_ListeLandesVorwahlen)
                        If Not .Landesvorwahl = DataProvider.P_Def_LeerString Then
                            .Landesvorwahl = DataProvider.P_Def_PreLandesVW & .Landesvorwahl
                        End If
                    End If
                    TelNr = Mid(TelNr, Len(.Landesvorwahl) + 1)
                Else
                    .Landesvorwahl = DataProvider.P_Def_LeerString
                End If
                .Landesvorwahl = Replace(.Landesvorwahl, " ", "", , , CompareMethod.Text) 'Leerzeichen entfernen'

                If TelNr.StartsWith("0") Then TelNr = TelNr.Remove(0, 1)

                ' Ortsvorwahl
                If .Landesvorwahl = DataProvider.P_Def_TBLandesVW Or .Landesvorwahl = DataProvider.P_Def_LeerString Then 'nur Deutschland
                    .Ortsvorwahl = VorwahlausDatei(TelNr, .Landesvorwahl, C_DP.P_ListeOrtsVorwahlenD)
                Else
                    .Ortsvorwahl = VorwahlausDatei(TelNr, .Landesvorwahl, C_DP.P_ListeOrtsVorwahlenA)
                    Select Case .Landesvorwahl
                        Case DataProvider.P_Def_PreLandesVW & "7" ' Kasachstan
                            ErsteZiffer = Mid(TelNr, Len(.Ortsvorwahl) + 1, 1)
                            If New String() {"3292", "3152", "3252", "3232", "3262"}.Contains(.Ortsvorwahl) And ErsteZiffer = "2" Then .Ortsvorwahl += ErsteZiffer
                            'If IsOneOf(.Ortsvorwahl, New String() {"3292", "3152", "3252", "3232", "3262"}) And ErsteZiffer = "2" Then .Ortsvorwahl += ErsteZiffer
                        Case DataProvider.P_Def_PreLandesVW & "39" ' Italien
                            ' Dies betrifft nur das Festnetz
                            If Not DataProvider.P_Def_MobilVorwahlItalien.Contains(.Ortsvorwahl) Then
                                .Ortsvorwahl = "0" & .Ortsvorwahl
                            End If
                    End Select
                End If

                TelNr = Mid(TelNr, Len(.Ortsvorwahl) + 1)

                If .TelNr.Contains("-") Then
                    .Durchwahl = Trim(Mid(.TelNr, InStrRev(.TelNr, "-",, CompareMethod.Text) + 1))
                End If
                .Nummer = Left(TelNr, Len(TelNr) - Len(.Durchwahl))
            End If
        End With

        Return retVal

    End Function

    Function VorwahlausDatei(ByVal TelNr As String, ByVal LandesVW As String, ByVal Vorwahlliste As ReadOnlyCollection(Of String)) As String
        VorwahlausDatei = DataProvider.P_Def_LeerString
        Dim i As Integer = 0
        Dim Prefix As String = DataProvider.P_Def_LeerString
        Dim Trefferliste As IEnumerable(Of String) = Nothing

        TelNr = Replace(TelNr, "*", "", , , CompareMethod.Text)
        If TelNr.StartsWith(DataProvider.P_Def_PreLandesVW) Then TelNr = TelNr.Remove(0, 2)
        If TelNr.StartsWith("0") Then TelNr = TelNr.Remove(0, 1)

        If Vorwahlliste Is C_DP.P_ListeOrtsVorwahlenA Then
            If LandesVW.StartsWith(DataProvider.P_Def_PreLandesVW) Then LandesVW = LandesVW.Remove(0, 2)
            If LandesVW.StartsWith("0") Then LandesVW = LandesVW.Remove(0, 1)
            Prefix = LandesVW & ":"
        End If

        Do
            i += 1
            Trefferliste = From s In Vorwahlliste Where s.ToLower Like Prefix & Left(TelNr, i).ToLower & "*" Select s
        Loop Until Trefferliste.Count = 1 Or i = 6

        If Trefferliste.Count = 1 Then VorwahlausDatei = Trefferliste(0).Substring(Prefix.Length)

        Trefferliste = Nothing
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
        If nurZiffern.StartsWith(C_DP.P_TBLandesVW) Then
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
        Dim TempTelNr As Telefonnummer = TelNrTeile(TelNr)
        Dim Vorwahl As String = Left(TempTelNr.Ortsvorwahl, 2)

        Return (TempTelNr.Landesvorwahl = C_DP.P_TBLandesVW Or TempTelNr.Landesvorwahl = DataProvider.P_Def_LeerString) And (Vorwahl.StartsWith("15") Or Vorwahl.StartsWith("16") Or Vorwahl.StartsWith("17"))
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
#End Region

#Region "HTTPTransfer"
    Public Function httpGET(ByVal Link As String, ByVal Encoding As Encoding, ByRef FBError As Boolean) As String
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
                        .Timeout = IIf(C_DP.P_CBForceFBAddr, 5000, 100000)
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
                    .Timeout = IIf(C_DP.P_CBForceFBAddr, 5000, 100000)
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

#Region "Timer"
    ''' <summary>
    ''' Erstellt einen Timer mit dem übergeben Intervall.
    ''' </summary>
    ''' <param name="Interval">Das Intervall des Timers.</param>
    ''' <returns>Den gerade erstellten Timer.</returns>
    Public Function SetTimer(ByVal Interval As Double) As System.Timers.Timer
        Dim aTimer As New System.Timers.Timer

        With aTimer
            .Interval = Interval
            .AutoReset = True
            .Enabled = True
        End With
        Return aTimer

    End Function

    ''' <summary>
    ''' Löscht den Timer und gibt dessen Ressoucen frei.
    ''' </summary>
    ''' <param name="Timer">Der zu löschende Timer.</param>
    ''' <returns>Einen Timer, welcher <c>Nothing</c> ist.</returns>
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
    Sub ThreadSleep(ByVal Dauer As Integer)
        Thread.Sleep(Dauer)
    End Sub
#End Region

#Region "Vergleichsfunktionen"
    ''' <summary>
    ''' Prüft, ob die übergebende Größe Null ist.
    ''' </summary>
    ''' <param name="Val1">Zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <DebuggerStepThrough>
    Friend Function CheckIsZero(ByVal Val1 As Double) As Boolean
        CheckIsZero = Val1 < Epsilon
    End Function

    ''' <summary>
    ''' Prüft, ob die beiden übergebenen Größen gleich sind: <c>Val1</c> = <c>Val2</c>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <DebuggerStepThrough>
    Friend Function CheckIsEqual(ByVal Val1 As Double, ByVal Val2 As Double) As Boolean
        CheckIsEqual = Math.Abs((Val1 - Val2)) < Epsilon
    End Function

    ''' <summary>
    ''' Prüft, ob die erste übergebene Größe <c>Val1</c> kleiner als die zweite übergebene Größe <c>Val2</c> ist: <c>Val1</c> &lt; <c>Val2</c>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <DebuggerStepThrough>
    Friend Function CheckIsLess(ByVal Val1 As Double, ByVal Val2 As Double) As Boolean
        CheckIsLess = Val2 - Val1 > Epsilon
    End Function

    ''' <summary>
    ''' Prüft, ob die erste übergebene Größe <c>Val1</c> kleiner oder gleich als die zweite übergebene Größe <c>Val2</c> ist: <c>Val1</c> &lt;= <c>Val2</c>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <DebuggerStepThrough>
    Friend Function CheckIsLessOrEqual(ByVal Val1 As Double, ByVal Val2 As Double) As Boolean
        CheckIsLessOrEqual = Val1 - Val2 <= Epsilon
    End Function

    ''' <summary>
    ''' Prüft, ob die erste übergebene Größe <c>Val1</c> größer als die zweite übergebene Größe <c>Val2</c> ist: <c>Val1</c> &gt; <c>Val2</c>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <DebuggerStepThrough>
    Friend Function CheckIsLarger(ByVal Val1 As Double, ByVal Val2 As Double) As Boolean
        CheckIsLarger = Val1 - Val2 > Epsilon
    End Function

    ''' <summary>
    ''' Prüft, ob die erste übergebene Größe <c>Val1</c> größer oder gleich als die zweite übergebene Größe <c>Val2</c> ist: <c>Val1</c> &gt;= <c>Val2</c>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <DebuggerStepThrough>
    Friend Function CheckIsLargerOrEqual(ByVal Val1 As Double, ByVal Val2 As Double) As Boolean
        CheckIsLargerOrEqual = Val2 - Val1 <= Epsilon
    End Function

    ''' <summary>
    ''' Prüft, ob die übergebene Größe <c>Val1</c> sich innerhalb eines Bereiches befindet: <c>LVal</c> &lt; <c>Val1</c> &lt; <c>UVal</c>.
    ''' </summary>
    ''' <param name="Val1">Zu prüfende Größe</param>
    ''' <param name="LVal">Untere Schwelle</param>
    ''' <param name="UVal">Obere schwelle</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <DebuggerStepThrough>
    Friend Function CheckIsInRange(ByVal Val1 As Double, ByVal LVal As Double, ByVal UVal As Double) As Boolean
        CheckIsInRange = CheckIsLarger(Val1, LVal) And CheckIsLess(Val1, UVal)
    End Function

    ''' <summary>
    ''' Vergleicht beide Größen und gibt die kleinere der beiden übergebenen Größen zurück.
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Kleinere der beiden Größen</returns>
    <DebuggerStepThrough>
    Friend Function GetLower(ByVal Val1 As Double, ByVal Val2 As Double) As Double
        GetLower = IIf(CheckIsLess(Val1, Val2), Val1, Val2)
    End Function

    ''' <summary>
    ''' Vergleicht beide Größen und gibt die größere der beiden übergebenen Größen zurück.
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Größere der beiden Größen</returns>
    <DebuggerStepThrough>
    Friend Function GetLarger(ByVal Val1 As Double, ByVal Val2 As Double) As Double
        GetLarger = IIf(CheckIsLarger(Val1, Val2), Val1, Val2)
    End Function

    ''' <summary>
    ''' Prüft über den Vergleichsmodus <c>Modus</c>, ob die erste übergebene Größe <c>Val1</c> kleiner, größer etc. als die zweite übergebene Größe <c>Val2</c> ist.
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <param name="Modus">Vergleichsmodus</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <DebuggerStepThrough>
    Friend Function CheckValues(ByVal Val1 As Double, ByVal Val2 As Double, ByVal Modus As Vergleichsmodus) As Boolean

        Select Case Modus
            Case Vergleichsmodus.KleinerGleich
                Return CheckIsLessOrEqual(Val1, Val2)
            Case Vergleichsmodus.Kleiner
                Return CheckIsLess(Val1, Val2)
            Case Vergleichsmodus.Gleich
                Return CheckIsEqual(Val1, Val2)
            Case Vergleichsmodus.Größer
                Return CheckIsLarger(Val1, Val2)
            Case Vergleichsmodus.GrößerGleich
                Return CheckIsLargerOrEqual(Val1, Val2)
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Gibt das Verhältnis der ersten übergebene Größe <c>Val1</c> zur zweiten übergebene Größe <c>Val2</c> an.
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns></returns>
    <DebuggerStepThrough>
    Friend Function CheckValues(ByVal Val1 As Double, ByVal Val2 As Double) As Vergleichsmodus
        If CheckIsEqual(Val1, Val2) Then
            CheckValues = Vergleichsmodus.Gleich
        Else
            If CheckIsLess(Val1, Val2) Then
                CheckValues = Vergleichsmodus.Kleiner
            Else
                CheckValues = Vergleichsmodus.Größer
            End If
        End If
    End Function

#End Region

#Region "Momentan nicht verwendeter Code"
    'Public Function GetUNIXTimeStamp(ByVal dteDate As Date) As Long
    '    'If dteDate.IsDaylightSavingTime Then dteDate = DateAdd(DateInterval.Hour, -1, dteDate)
    '    Return DateDiff(DateInterval.Second, #1/1/1970#, dteDate)
    'End Function  
#Region "GZip"
    '    Public Function GZipCompressString(ByVal text As String) As String
    '        Dim buffer As Byte() = Encoding.Unicode.GetBytes(text)
    '        Dim compressed As Byte()
    '        Dim gzBuffer As Byte()

    '        Using ms As New MemoryStream
    '            Using zipStream As New Compression.GZipStream(ms, Compression.CompressionMode.Compress, True)
    '                zipStream.Write(buffer, 0, buffer.Length)
    '            End Using
    '            ms.Position = 0

    '            compressed = New Byte(CInt(ms.Length - 1)) {}

    '            ms.Read(compressed, 0, compressed.Length)
    '            gzBuffer = New Byte(compressed.Length + 3) {}

    '            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length)
    '            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4)
    '            Return Convert.ToBase64String(gzBuffer)
    '        End Using
    '    End Function

    '    Public Function GZipDecompressString(ByVal compressedText As String) As String
    '        Dim gzBuffer As Byte() = Convert.FromBase64String(compressedText)
    '        Dim msgLength As Integer
    '        Dim buffer As Byte()

    '        Using ms As New MemoryStream()
    '            msgLength = BitConverter.ToInt32(gzBuffer, 0)
    '            ms.Write(gzBuffer, 4, gzBuffer.Length - 4)

    '            buffer = New Byte(msgLength - 1) {}

    '            ms.Position = 0
    '            Using zipStream As New Compression.GZipStream(ms, Compression.CompressionMode.Decompress)
    '                zipStream.Read(buffer, 0, buffer.Length)
    '            End Using

    '            Return Encoding.Unicode.GetString(buffer, 0, buffer.Length)
    '        End Using
    '    End Function
#End Region
#End Region
End Class
