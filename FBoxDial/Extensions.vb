Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions


<DebuggerStepThrough()>
Public Module Extensions
    Private Property NLogger As NLog.Logger = LogManager.GetCurrentClassLogger

#Region "Extensions für Verarbeitung von Zahlen: Double, Integer, Long"
    Private Const Epsilon As Single = Single.Epsilon
    ''' <summary>
    ''' Gibt den Absolutwert der Zahlengröße zurück
    ''' </summary>
    <Extension()> Public Function Absolute(ByVal Val1 As Double) As Double
        Return Math.Abs(Val1)
    End Function
    <Extension()> Public Function Absolute(ByVal Val1 As Integer) As Integer
        Absolute = Math.Abs(Val1)
    End Function
    <Extension()> Public Function Absolute(ByVal Val1 As Long) As Long
        Absolute = Math.Abs(Val1)
    End Function

    ''' <summary>
    ''' Prüft, ob die übergebende Größe Null ist.
    ''' </summary>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function IsZero(ByVal Val1 As Double) As Boolean
        Return Val1.Absolute < Epsilon
    End Function
    <Extension()> Public Function IsZero(ByVal Val1 As Integer) As Boolean
        Return Val1.Absolute < Epsilon
    End Function

    ''' <summary>
    ''' Prüft, ob die übergebende Größe ungleich Null ist.
    ''' </summary>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function IsNotZero(ByVal Val1 As Double) As Boolean
        Return Not Val1.IsZero
    End Function
    <Extension()> Public Function IsNotZero(ByVal Val1 As Integer) As Boolean
        Return Not Val1.IsZero
    End Function

    ''' <summary>
    ''' Prüft, ob die beiden übergebenen Größen gleich sind: <paramref name="Val1"/> == <paramref name="Val2"/>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function AreEqual(ByVal Val1 As Double, ByVal Val2 As Double) As Boolean
        Return (Val1 - Val2).Absolute < Epsilon
    End Function
    <Extension()> Public Function AreEqual(ByVal Val1 As Integer, ByVal Val2 As Integer) As Boolean
        Return (Val1 - Val2).Absolute < Epsilon
    End Function
    <Extension()> Public Function AreEqual(ByVal Val1 As Long, ByVal Val2 As Long) As Boolean
        Return (Val1 - Val2).Absolute < Epsilon
    End Function

    ''' <summary>
    ''' Prüft, ob die beiden übergebenen Größen gleich sind: <paramref name="Val1"/> == <paramref name="Val2"/>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function AreDifferent(ByVal Val1 As Double, ByVal Val2 As Double) As Boolean
        Return Not Val1.AreEqual(Val2)
    End Function
    <Extension()> Public Function AreDifferent(ByVal Val1 As Integer, ByVal Val2 As Integer) As Boolean
        Return Not Val1.AreEqual(Val2)
    End Function

    ''' <summary>
    ''' Prüft, ob die erste übergebene Größe <paramref name="Val1"/> kleiner als die zweite übergebene Größe <paramref name="Val2"/> ist: <paramref name="Val1"/> &lt; <paramref name="Val2"/>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function IsLess(ByVal Val1 As Double, ByVal Val2 As Double) As Boolean
        Return Val2 - Val1 > Epsilon
    End Function
    <Extension()> Public Function IsLess(ByVal Val1 As Integer, ByVal Val2 As Integer) As Boolean
        Return Val2 - Val1 > Epsilon
    End Function

    ''' <summary>
    ''' Prüft, ob die erste übergebene Größe <paramref name="Val1"/> kleiner oder gleich als die zweite übergebene Größe <paramref name="Val2"/> ist: <paramref name="Val1"/> &lt;= <paramref name="Val2"/>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>

    <Extension()> Public Function IsLessOrEqual(ByVal Val1 As Integer, ByVal Val2 As Integer) As Boolean
        Return Val1 - Val2 <= Epsilon
    End Function
    '<Extension()> Public Function IsLessOrEqual(ByVal Val1 As Double, ByVal Val2 As Double) As Boolean
    '    Return Val1 - Val2 <= Epsilon
    'End Function

    ''' <summary>
    ''' Prüft, ob die erste übergebene Größe <paramref name="Val1"/> größer als die zweite übergebene Größe <paramref name="Val2"/> ist: <paramref name="Val1"/> &gt; <paramref name="Val2"/>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function IsLarger(ByVal Val1 As Double, ByVal Val2 As Double) As Boolean
        Return Val1 - Val2 > Epsilon
    End Function
    <Extension()> Public Function IsLarger(ByVal Val1 As Integer, ByVal Val2 As Integer) As Boolean
        Return Val1 - Val2 > Epsilon
    End Function
    <Extension()> Public Function IsLarger(ByVal Val1 As Long, ByVal Val2 As Long) As Boolean
        Return Val1 - Val2 > Epsilon
    End Function

    ''' <summary>
    ''' Prüft, ob die erste übergebene Größe <paramref name="Val1"/> größer oder gleich als die zweite übergebene Größe <paramref name="Val2"/> ist: <paramref name="Val1"/> &gt;= <paramref name="Val2"/>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function IsLargerOrEqual(ByVal Val1 As Double, ByVal Val2 As Double) As Boolean
        Return Val2 - Val1 <= Epsilon
    End Function
    <Extension()> Public Function IsLargerOrEqual(ByVal Val1 As Integer, ByVal Val2 As Integer) As Boolean
        Return Val2 - Val1 <= Epsilon
    End Function
    <Extension()> Public Function IsLargerOrEqual(ByVal Val1 As Single, ByVal Val2 As Single) As Boolean
        Return Val2 - Val1 <= Epsilon
    End Function

    ''' <summary>
    ''' Gibt den größeren von zwei Vergleichswerten zurück
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function GetLarger(ByVal Val1 As Integer, ByVal Val2 As Integer) As Integer
        Return If(Val1.IsLargerOrEqual(Val2), Val1, Val2)
    End Function

    ''' <summary>
    ''' Prüft, ob die übergebene Größe <paramref name="Val1"/> sich innerhalb eines Bereiches befindet: <paramref name="LVal"/> &lt; <paramref name="Val1"/> &lt; <paramref name="UVal"/>.
    ''' </summary>
    ''' <param name="Val1">Zu prüfende Größe</param>
    ''' <param name="LVal">Untere Schwelle</param>
    ''' <param name="UVal">Obere schwelle</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function IsInRange(ByVal Val1 As Double, ByVal LVal As Double, ByVal UVal As Double) As Boolean
        Return Val1.IsLarger(LVal) And Val1.IsLess(UVal)
    End Function
    <Extension()> Public Function IsInRange(ByVal Val1 As Integer, ByVal LVal As Integer, ByVal UVal As Integer) As Boolean
        Return Val1.IsLarger(LVal) And Val1.IsLess(UVal)
    End Function


    ''' <summary>
    ''' Prüft, ob der übergebende Wert negativ ist
    ''' </summary>
    ''' <param name="Value">Der zu überprüfende Wert.</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Friend Function IsNegative(ByVal Value As Double) As Boolean
    Return IsLess(Value, 0)
    End Function

    <Extension()> Friend Function IsNegative(ByVal Value As Integer) As Boolean
        Return IsLess(Value, 0)
    End Function
    <Extension()> Friend Function IsPositive(ByVal Value As Double) As Boolean
        Return IsLarger(Value, 0)
    End Function

    <Extension()> Friend Function IsPositive(ByVal Value As Integer) As Boolean
        Return IsLarger(Value, 0)
    End Function
#End Region

#Region "Extensions für Verarbeitung von Zeichenfolgen: String"
    <Extension> Public Function AreEqual(ByVal Str1 As String, ByVal Str2 As String) As Boolean
        Return String.Compare(Str1, Str2).IsZero
    End Function
    <Extension> Public Function AreNotEqual(ByVal Str1 As String, ByVal Str2 As String) As Boolean
        Return String.Compare(Str1, Str2).IsNotZero
    End Function
    <Extension> Public Function IsStringEmpty(ByVal Str1 As String) As Boolean
        Return Str1.AreEqual(String.Empty)
    End Function
    <Extension> Public Function IsNotStringEmpty(ByVal Str1 As String) As Boolean
        Return Str1 IsNot Nothing AndAlso Not Str1.IsStringEmpty
    End Function
    <Extension> Public Function IsStringNothing(ByVal Str1 As String) As Boolean
        Return Str1 Is Nothing
    End Function
    <Extension> Public Function IsNotStringNothing(ByVal Str1 As String) As Boolean
        Return Not Str1.IsStringNothing
    End Function
    <Extension> Public Function IsStringNothingOrEmpty(ByVal Str1 As String) As Boolean
        Return Str1.IsStringNothing OrElse Str1.IsStringEmpty
    End Function
    <Extension> Public Function IsNotStringNothingOrEmpty(ByVal Str1 As String) As Boolean
        Return Not Str1.IsStringNothingOrEmpty
    End Function
    <Extension> Public Function IsErrorString(ByVal Str1 As String) As Boolean
        Return Str1.AreEqual(PDfltStrErrorMinusOne)
    End Function
    <Extension> Public Function IsNotErrorString(ByVal Str1 As String) As Boolean
        Return Not Str1.IsErrorString
    End Function
    <Extension> Public Function RegExReplace(ByVal str1 As String, ByVal pattern As String, ByVal replacement As String, Optional ByVal RegOpt As RegexOptions = RegexOptions.None) As String
        Return Regex.Replace(str1, pattern, replacement, RegOpt)
    End Function
    <Extension> Public Function IsRegExMatch(ByVal str1 As String, ByVal pattern As String, Optional ByVal RegOpt As RegexOptions = RegexOptions.None) As Boolean
        Return Regex.Match(str1, pattern, RegOpt).Success
    End Function
    <Extension> Public Function Left(ByVal str1 As String, ByVal iLength As Integer) As String
        Return Strings.Left(str1, iLength)
    End Function
    ''' <summary>
    ''' Gibt nur die Numerischen Ziffen eines String zurück
    ''' </summary>
    ''' <param name="sTxt">String der umgewandelt werden soll</param>
    <Extension> Public Function AcceptOnlyNumeric(ByVal sTxt As String) As String
        Return Regex.Replace(sTxt, "\D", PDfltStringEmpty)
    End Function

    ''' <summary>
    ''' Entnimmt aus dem String <paramref name="Text"/> einen enthaltenen Sub-String ausgehend von einer Zeichenfolge davor <paramref name="StringDavor"/> 
    ''' und deiner Zeichenfolge danach <paramref name="StringDanach"/>.
    ''' </summary>
    ''' <param name="Text">String aus dem der Sub-String entnommen werden soll.</param>
    ''' <param name="StringDavor">Zeichenfolge vor dem zu entnehmenden Sub-String.</param>
    ''' <param name="StringDanach">Zeichenfolge nach dem zu entnehmenden Sub-String.</param>
    ''' <param name="Reverse">Flag, Ob die Suche nach den Zeichenfolgen vor und nach dem Sub-String vom Ende des <paramref name="Text"/> aus begonnen werden soll.</param>
    ''' <returns>Wenn <paramref name="StringDavor"/> und <paramref name="StringDanach"/> enthalten sind, dann wird der Teilstring zurückgegeben. Ansonsten "-1".</returns>
    <Extension> Public Function GetSubString(ByVal Text As String, ByVal StringDavor As String, ByVal StringDanach As String, ByVal Reverse As Boolean) As String
        Dim pos(1) As Integer

        If Not Reverse Then
            pos(0) = InStr(1, Text, StringDavor, CompareMethod.Text) + Len(StringDavor)
            pos(1) = InStr(pos(0), Text, StringDanach, CompareMethod.Text)
        Else
            pos(1) = InStrRev(Text, StringDanach, , CompareMethod.Text)
            pos(0) = InStrRev(Text, StringDavor, pos(1), CompareMethod.Text) + Len(StringDavor)
        End If

        If Not pos(0).AreEqual(Len(StringDavor)) Then
            GetSubString = Mid(Text, pos(0), pos(1) - pos(0))
        Else
            GetSubString = PDfltStrErrorMinusOne
        End If
    End Function

    ''' <summary>
    ''' Entnimmt aus dem String <paramref name="Text"/> einen enthaltenen Sub-String ausgehend von einer Zeichenfolge davor <paramref name="StringDavor"/>
    ''' und deiner Zeichenfolge danach <paramref name="StringDanach"/>.
    ''' </summary>
    ''' <param name="Text">String aus dem der Sub-String entnommen werden soll.</param>
    ''' <param name="StringDavor">Zeichenfolge vor dem zu entnehmenden Sub-String.</param>
    ''' <param name="StringDanach">Zeichenfolge nach dem zu entnehmenden Sub-String.</param>
    ''' <returns>Wenn <paramref name="StringDavor"/> und <paramref name="StringDanach"/> enthalten sind, dann wird der Teilstring zurückgegeben. Ansonsten "-1".</returns>
    <Extension> Public Function GetSubString(ByVal Text As String, ByVal StringDavor As String, ByVal StringDanach As String) As String
        Return GetSubString(Text, StringDavor, StringDanach, False)
    End Function

    ''' <summary>
    ''' Entnimmt aus dem String <paramref name="Text"/> einen enthaltenen Sub-String ausgehend von einer Zeichenfolge davor <paramref name="StringDavor"/> 
    ''' und deiner Zeichenfolge danach <paramref name="StringDanach"/>.
    ''' Beginnt Suche nach TeilString an einem Startpunkt <paramref name="StartPosition"/>.
    ''' </summary>
    ''' <param name="Text">String aus dem der Sub-String entnommen werden soll.</param>
    ''' <param name="StringDavor">Zeichenfolge vor dem zu entnehmenden Sub-String.</param>
    ''' <param name="StringDanach">Zeichenfolge nach dem zu entnehmenden Sub-String.</param>
    ''' <param name="StartPosition">Startposition, bei der mit der Suche nach den Zeichenfolgen vor und nach dem Sub-String begonnen werden soll.</param>
    ''' <returns>Wenn <paramref name="StringDavor"/> und <paramref name="StringDanach"/> enthalten sind, dann wird der Teilstring zurückgegeben. Ansonsten "-1".</returns>
    <Extension> Public Function GetSubString(ByVal Text As String, ByVal StringDavor As String, ByVal StringDanach As String, ByRef StartPosition As Integer) As String
        Dim pos(1) As Integer

        pos(0) = InStr(StartPosition, Text, StringDavor, CompareMethod.Text) + Len(StringDavor)
        pos(1) = InStr(pos(0), Text, StringDanach, CompareMethod.Text)

        If Not Not pos(0).AreEqual(Len(StringDavor)) Then
            GetSubString = Mid(Text, pos(0), pos(1) - pos(0))
            StartPosition = pos(1)
        Else
            GetSubString = PDfltStrErrorMinusOne
        End If

    End Function

    <Extension> Public Function Split(ByVal Text As String, ByVal Delimiter As String) As String()
        Return Strings.Split(Text, Delimiter,, CompareMethod.Text)
    End Function

    <Extension> Public Function XMLMaskiereZeichen(ByVal Text As String) As String
        ' Nicht zugelassene Zeichen der XML-Notifikation ersetzen.
        ' Zeichen	Notation in XML
        ' <	        &lt;    &#60;
        ' >	        &gt;    &#62;
        ' &	        &amp;   &#38; Zweimal anfügen, da es ansonsten ignoriert wird
        ' "	        &quot;  &#34;
        ' '	        &apos;  &#38;
        Return Text.Replace("&", "&amp;&amp;").Replace("&amp;&amp;#", "&#").Replace("<", "&lt;").Replace(">", "&gt;").Replace(Chr(34), "&quot;").Replace("'", "&apos;")
    End Function
#End Region

#Region "Extensions für Verarbeitung von Zeichenfolgen: List(Of Telefonat), List(Of VIPEntry)"
    <Extension> Public Sub Insert(ByRef Liste As List(Of Telefonat), ByVal item As Telefonat)

        ' Liste initialisieren, falls erforderlich
        If Liste Is Nothing Then Liste = New List(Of Telefonat)

        'Liste.Add(item)
        Liste.Insert(0, item)
        ' Liste sortieren
        Liste = Liste.OrderByDescending(Function(TF) TF.ZeitBeginn).ToList

        ' Entferne alle überflüssigen Elemente
        With Liste
            ' PTBNumEntryList = 10
            ' .Count = 11
            ' Start = PTBNumEntryList (Nullbasiert), Anzahl an zu löschenden Elementen = .Count - PTBNumEntryList
            ' Start = 10, Anzahl = 11 - 10 = 1
            If .Count.IsLarger(XMLData.POptionen.PTBNumEntryList) Then
                .RemoveRange(XMLData.POptionen.PTBNumEntryList, .Count - XMLData.POptionen.PTBNumEntryList)
            End If

        End With
        ThisAddIn.POutlookRibbons.RefreshRibbon()
    End Sub

    <Extension> Public Sub Insert(ByRef Liste As List(Of VIPEntry), ByVal item As VIPEntry)

        ' Liste initialisieren, falls erforderlich
        If Liste Is Nothing Then Liste = New List(Of VIPEntry)

        Liste.Insert(0, item)

        ThisAddIn.POutlookRibbons.RefreshRibbon()
    End Sub
#End Region


#Region "Zahlenkonvertierungen"
    <Extension> Public Function ToInt(ByVal Text As String) As Integer
        Dim retVal As Integer = PDfltIntErrorMinusOne
        If Integer.TryParse(Text, retVal) Then
            Return retVal
        Else
            Return PDfltIntErrorMinusOne
        End If
    End Function
    <Extension> Public Function ToInt(ByVal sWert As Single) As Integer
        Return CInt(sWert)
    End Function
    <Extension> Public Function ToInt(ByVal dWert As Double) As Integer
        Return CInt(dWert)
    End Function
    <Extension> Public Function ToInt(ByVal dWert As Byte) As Integer
        Return dWert
    End Function

    <Extension> Public Function ToSng(ByVal dWert As Double) As Single
        Return CSng(dWert)
    End Function
#End Region

#Region "Bytes"
    <Extension> Public Function Append(Of T)(ByVal arr1() As T, ByVal arr2 As ICollection(Of T)) As T()
        Dim retVal(arr1.Length + arr2.Count - 1) As T
        Array.Copy(arr1, retVal, arr1.Length)
        arr2.CopyTo(retVal, arr1.Length)
        Return retVal
    End Function
    <Extension> Public Function SplitByte(Of T)(ByVal arr As T(), ByVal index As Integer) As T()()
        Dim retVal = {New T(index - 1) {}, New T(arr.Length - (index + 1)) {}}
        Array.Copy(arr, 0, retVal(0), 0, retVal(0).Length)
        Array.Copy(arr, index, retVal(1), 0, retVal(1).Length)
        Return retVal
    End Function
    <Extension> Public Function ToBase64String(ByVal arr1() As Byte) As String
        Return Convert.ToBase64String(arr1)
    End Function
    <Extension> Public Function FromBase64String(ByVal str1 As String) As Byte()
        Return Convert.FromBase64String(str1)
    End Function
#End Region

#Region "Netzwerkfunktionen"
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
        Dim data As String = PDfltStringEmpty

        Dim buffer As Byte() = Encoding.ASCII.GetBytes(data)
        Dim timeout As Integer = 120

        Options.DontFragment = True

        Try
            PingReply = PingSender.Send(IPAdresse, timeout, buffer, Options)
        Catch ex As Exception
            NLogger.Warn(ex, "Ping zu {0} nicht erfolgreich", IPAdresse)
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
                                NLogger.Info("IPv6: {0}, IPv4: {1}", .Address.ToString, IPAdresse)
                                Exit For
                            End If
                        Next
                    Else
                        IPAdresse = .Address.ToString
                    End If
                    Ping = True
                Else
                    NLogger.Warn("Ping zu '{0}' nicht erfolgreich: {1}" & .Status, IPAdresse, .Status)
                    Ping = False
                End If
            End With
        End If
        PingSender.Dispose()
        'Options = Nothing
        'PingSender = Nothing
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

        ValidIP = FritzBoxDefault.PDfltFritzBoxAdress

        If IPAddress.TryParse(InputIP, IPAddresse) Then
            Select Case IPAddresse.AddressFamily
                Case Sockets.AddressFamily.InterNetworkV6
                    ValidIP = "[" & IPAddresse.ToString & "]"
                Case Sockets.AddressFamily.InterNetwork
                    ValidIP = IPAddresse.ToString
                Case Else
                    NLogger.Warn("Die IP '{0}' kann nicht zugeordnet werden.", InputIP)
                    ValidIP = InputIP
            End Select
        Else
            Try
                IPHostInfo = Dns.GetHostEntry(XMLData.POptionen.PTBFBAdr)
                For Each IPAddresse In IPHostInfo.AddressList
                    If IPAddresse.AddressFamily = Sockets.AddressFamily.InterNetwork Then
                        ValidIP = IPAddresse.ToString
                    End If
                Next
            Catch ex As Exception
                NLogger.Warn(ex, "Die Adresse '{0}' kann nicht zugeordnet werden.", XMLData.POptionen.PTBFBAdr)
                ValidIP = XMLData.POptionen.PTBFBAdr
            End Try
        End If

    End Function

    Public Async Function HTTPGet(ByVal Link As String, ByVal FBEncoding As Encoding) As Threading.Tasks.Task(Of String)

        Dim retVal As String = PDfltStringEmpty
        Dim UniformResourceIdentifier As New Uri(Link)

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Select Case UniformResourceIdentifier.Scheme
            Case Uri.UriSchemeHttp, Uri.UriSchemeHttps

                Using webClient As New WebClient
                    With webClient
                        .Encoding = FBEncoding
                        .Proxy = Nothing
                        .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)
                        .Headers.Add(HttpRequestHeader.KeepAlive, "False")
                        Try
                            retVal = Await .DownloadStringTaskAsync(UniformResourceIdentifier)
                            NLogger.Debug("HTTPGet: {0} - {1}", Link, retVal)
                        Catch exANE As ArgumentNullException
                            NLogger.Error(exANE)
                        Catch exWE As WebException
                            NLogger.Error(exWE, "Link: {0}", Link)
                        End Try
                    End With
                End Using
            Case Else
                NLogger.Warn("Uri.Scheme: {0}", UniformResourceIdentifier.Scheme)
        End Select
        Return retVal
    End Function

    Public Async Function HTTPPost(ByVal Link As String, ByVal Daten As String, ByVal ZeichenCodierung As Encoding) As Threading.Tasks.Task(Of String)

        Dim retVal As String = PDfltStringEmpty
        Dim UniformResourceIdentifier As New Uri(Link)

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        If UniformResourceIdentifier.Scheme = Uri.UriSchemeHttp Then
            Using webClient As New WebClient
                With webClient
                    .Encoding = ZeichenCodierung
                    .Proxy = Nothing
                    .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)
                    With .Headers
                        .Add(HttpRequestHeader.ContentLength, Daten.Length.ToString)
                        .Add(HttpRequestHeader.UserAgent, PDfltHeader_UserAgent)
                        .Add(HttpRequestHeader.KeepAlive, "True")
                        .Add(HttpRequestHeader.Accept, PDfltHeader_Accept)
                    End With

                    Try
                        retVal = Await .UploadStringTaskAsync(UniformResourceIdentifier, Daten)
                        NLogger.Debug("HTTPPost: {0} - {1} - {2}", Link, Daten, retVal)
                    Catch exANE As ArgumentNullException
                        NLogger.Error(exANE)
                    Catch exWE As WebException
                        NLogger.Error(exWE, "Link: {0}", Link)
                    End Try
                End With
            End Using
        End If
        Return retVal
    End Function

#End Region

#Region "Hilfsfunktionen"
    ''' <summary>
    ''' Dekrementiert den Verweiszähler des dem angegebenen COM-Objekt zugeordneten angegebenen Runtime Callable Wrapper (RCW)
    ''' </summary>
    ''' <param name="COMObject">Das freizugebende COM-Objekt.</param>
    <Extension> Public Sub ReleaseComObject(Of T)(ByVal COMObject As T)
        If COMObject IsNot Nothing Then
            Try
                Runtime.InteropServices.Marshal.ReleaseComObject(COMObject)
            Catch ex As ArgumentException
                NLogger.Error(ex, "COM-Object ist kein gültiges COM-Objekt: {0}", COMObject.ToString)
            End Try
        End If
    End Sub

    Public Function MsgBox(ByVal Meldung As String, ByVal Style As MsgBoxStyle, ByVal Aufruf As String) As MsgBoxResult
        If Style = MsgBoxStyle.Critical Or Style = MsgBoxStyle.Exclamation Then
            Meldung = String.Format("Die Funktion {0} meldet folgenden Fehler: {1}{2}", Aufruf, PDflt2NeueZeile, Meldung)
            NLogger.Warn(Meldung)
        End If
        Return Microsoft.VisualBasic.MsgBox(Meldung, Style, PDfltAddin_LangName)
    End Function

    '''' <summary>
    '''' Wandelt eine Zeitspanne in Sekunden in ein Format in Stunden:Minuten:Sekunden um
    '''' </summary>
    '''' <param name="nSeks">Sekunden der Zeitspanne</param>
    'Public Function GetTimeInterval(ByVal nSeks As Double) As String
    '    'http://www.vbarchiv.net/faq/date_sectotime.php
    '    Dim h As Double, m As Double
    '    h = nSeks / 3600
    '    nSeks = nSeks Mod 3600
    '    m = nSeks / 60
    '    nSeks = nSeks Mod 60
    '    Return Format(h, "00") & ":" & Format(m, "00") & ":" & Format(nSeks, "00")
    'End Function

    '''' <summary>
    '''' Entfernt doppelte und leere Einträge aus einem String-Array.
    '''' </summary>
    '''' <param name="ArraytoClear">Das zu bereinigende Array</param>
    '''' <param name="ClearDouble">Angabe, ob doppelte Einträge entfernt werden sollen.</param>
    '''' <param name="ClearEmpty">Angabe, ob leere Einträge entfernt werden sollen.</param>
    '''' <param name="ClearMinusOne">Angabe, ob Einträge mit dem Wert -1 entfernt werden sollen.</param>
    '''' <returns>Das bereinigte String-Array</returns>
    '''' <remarks></remarks>
    '<Extension> Public Function ClearStringArray(ByVal ArraytoClear As String(), ByVal ClearDouble As Boolean, ByVal ClearEmpty As Boolean, ByVal ClearMinusOne As Boolean) As String()
    '    ' Doppelte entfernen
    '    If ClearDouble Then ArraytoClear = (From x In ArraytoClear Select x Distinct).ToArray
    '    ' Leere entfernen
    '    If ClearEmpty Then ArraytoClear = (From x In ArraytoClear Where Not x Like PDfltStringEmpty Select x).ToArray
    '    ' -1 entfernen
    '    If ClearMinusOne Then ArraytoClear = (From x In ArraytoClear Where Not x Like PDfltStrErrorMinusOne Select x).ToArray

    '    Return ArraytoClear
    'End Function

    ''' <summary>
    ''' Erstellt einen Timer mit dem übergeben Intervall.
    ''' </summary>
    ''' <param name="Interval">Das Intervall des Timers.</param>
    ''' <returns>Den gerade erstellten Timer.</returns>
    Public Function SetTimer(ByVal Interval As Double) As Timers.Timer
        Dim aTimer As New Timers.Timer

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
    Public Function KillTimer(ByVal Timer As Timers.Timer) As Timers.Timer
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

    Public Function GetScaling() As Drawing.SizeF
        Dim regAppliedDPI As Integer
        Try
            regAppliedDPI = CInt(My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Control Panel\Desktop\WindowMetrics", "AppliedDPI", Nothing))
        Catch ex As Exception
            regAppliedDPI = 96
            NLogger.Warn(ex)
        End Try
        Return New Drawing.SizeF((regAppliedDPI / 96).ToSng, (regAppliedDPI / 96).ToSng)
    End Function
    Public Function BlankImage() As Drawing.Image
        Static oBM As New Drawing.Bitmap(1, 1)
        Try

            If oBM Is Nothing Then
                oBM.SetPixel(0, 0, Drawing.Color.Transparent)
            End If
            Return oBM
        Catch ex As Exception
            Return Nothing
        End Try

    End Function
    <Extension> Public Sub Sort(NodesCollection As Windows.Forms.TreeNodeCollection, Ascending As Boolean, SortChildNodes As Boolean)

        Dim node1 As Windows.Forms.TreeNode, node2 As Windows.Forms.TreeNode
        Dim iTn1 As Integer = Nothing
        Dim iTn2 As Integer = Nothing
        Dim k As Integer
        Dim iCompareResult As Integer
        Dim Comparer As New StringLogicalComparer()

        ' SortChildNodes: Ggf. Rekursion einleiten
        If SortChildNodes Then
            For i As Integer = 0 To NodesCollection.Count - 1
                Sort(NodesCollection(i).Nodes, Ascending, SortChildNodes)
            Next i
        End If

        ' Alle Knoten durchwandern
        For i As Integer = 0 To NodesCollection.Count - 1

            k = NodesCollection.Count

            Do While k.IsLarger(i)

                k -= 1
                ' Je zwei Nodes ermitteln und Texte vergleichen
                node1 = NodesCollection(i)
                node2 = NodesCollection(k)
                iCompareResult = Comparer.Compare(node1.Text, node2.Text)

                ' Falls die Sortierung nicht dem übergebenen Sortierkriterium entspricht:
                If (Ascending = True And iCompareResult.IsPositive) OrElse (Ascending = False And iCompareResult.IsNegative) Then

                    ' Nodes austauschen
                    With NodesCollection
                        .Remove(node1)
                        .Remove(node2)
                        .Insert(i, node2)
                        .Insert(k, node1)
                    End With
                End If
            Loop
        Next

    End Sub
#End Region
End Module