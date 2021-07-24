Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

<DebuggerStepThrough()>
Public Module Extensions
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Extensions für Verarbeitung von Zahlen: Double, Integer, Long"
    Private Const Epsilon As Single = Single.Epsilon
    ''' <summary>
    ''' Gibt den Absolutwert der Zahlengröße zurück
    ''' </summary>
    <Extension()> Public Function Absolute(Val1 As Double) As Double
        Return Math.Abs(Val1)
    End Function
    <Extension()> Public Function Absolute(Val1 As Integer) As Integer
        Absolute = Math.Abs(Val1)
    End Function
    <Extension()> Public Function Absolute(Val1 As Long) As Long
        Absolute = Math.Abs(Val1)
    End Function

    ''' <summary>
    ''' Prüft, ob die übergebende Größe Null ist.
    ''' </summary>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function IsZero(Val1 As Double) As Boolean
        Return Val1.Absolute < Epsilon
    End Function
    <Extension()> Public Function IsZero(Val1 As Integer) As Boolean
        Return Val1.Absolute < Epsilon
    End Function

    ''' <summary>
    ''' Prüft, ob die übergebende Größe ungleich Null ist.
    ''' </summary>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function IsNotZero(Val1 As Double) As Boolean
        Return Not Val1.IsZero
    End Function
    <Extension()> Public Function IsNotZero(Val1 As Integer) As Boolean
        Return Not Val1.IsZero
    End Function

    ''' <summary>
    ''' Prüft, ob die beiden übergebenen Größen gleich sind: <paramref name="Val1"/> == <paramref name="Val2"/>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function AreEqual(Val1 As Double, Val2 As Double) As Boolean
        Return (Val1 - Val2).Absolute < Epsilon
    End Function
    <Extension()> Public Function AreEqual(Val1 As Integer, Val2 As Integer) As Boolean
        Return (Val1 - Val2).Absolute < Epsilon
    End Function
    <Extension()> Public Function AreEqual(Val1 As Long, Val2 As Long) As Boolean
        Return (Val1 - Val2).Absolute < Epsilon
    End Function

    ''' <summary>
    ''' Prüft, ob die beiden übergebenen Größen gleich sind: <paramref name="Val1"/> == <paramref name="Val2"/>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function AreDifferentTo(Val1 As Double, Val2 As Double) As Boolean
        Return Not Val1.AreEqual(Val2)
    End Function
    <Extension()> Public Function AreDifferentTo(Val1 As Integer, Val2 As Integer) As Boolean
        Return Not Val1.AreEqual(Val2)
    End Function

    ''' <summary>
    ''' Prüft, ob die erste übergebene Größe <paramref name="Val1"/> kleiner als die zweite übergebene Größe <paramref name="Val2"/> ist: <paramref name="Val1"/> &lt; <paramref name="Val2"/>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function IsLess(Val1 As Double, Val2 As Double) As Boolean
        Return Val2 - Val1 > Epsilon
    End Function
    <Extension()> Public Function IsLess(Val1 As Integer, Val2 As Integer) As Boolean
        Return Val2 - Val1 > Epsilon
    End Function

    ''' <summary>
    ''' Prüft, ob die erste übergebene Größe <paramref name="Val1"/> kleiner oder gleich als die zweite übergebene Größe <paramref name="Val2"/> ist: <paramref name="Val1"/> &lt;= <paramref name="Val2"/>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>

    <Extension()> Public Function IsLessOrEqual(Val1 As Integer, Val2 As Integer) As Boolean
        Return Val1 - Val2 <= Epsilon
    End Function
    '<Extension()> Public Function IsLessOrEqual(Val1 As Double,  Val2 As Double) As Boolean
    '    Return Val1 - Val2 <= Epsilon
    'End Function

    ''' <summary>
    ''' Prüft, ob die erste übergebene Größe <paramref name="Val1"/> größer als die zweite übergebene Größe <paramref name="Val2"/> ist: <paramref name="Val1"/> &gt; <paramref name="Val2"/>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function IsLarger(Val1 As Double, Val2 As Double) As Boolean
        Return Val1 - Val2 > Epsilon
    End Function
    <Extension()> Public Function IsLarger(Val1 As Integer, Val2 As Integer) As Boolean
        Return Val1 - Val2 > Epsilon
    End Function
    <Extension()> Public Function IsLarger(Val1 As Long, Val2 As Long) As Boolean
        Return Val1 - Val2 > Epsilon
    End Function

    ''' <summary>
    ''' Prüft, ob die erste übergebene Größe <paramref name="Val1"/> größer oder gleich als die zweite übergebene Größe <paramref name="Val2"/> ist: <paramref name="Val1"/> &gt;= <paramref name="Val2"/>
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function IsLargerOrEqual(Val1 As Double, Val2 As Double) As Boolean
        Return Val2 - Val1 <= Epsilon
    End Function
    <Extension()> Public Function IsLargerOrEqual(Val1 As Integer, Val2 As Integer) As Boolean
        Return Val2 - Val1 <= Epsilon
    End Function
    <Extension()> Public Function IsLargerOrEqual(Val1 As Single, Val2 As Single) As Boolean
        Return Val2 - Val1 <= Epsilon
    End Function

    ''' <summary>
    ''' Gibt den größeren von zwei Vergleichswerten zurück
    ''' </summary>
    ''' <param name="Val1">Erste zu prüfende Größe</param>
    ''' <param name="Val2">Zweite zu prüfende Größe</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function GetLarger(Val1 As Integer, Val2 As Integer) As Integer
        Return If(Val1.IsLargerOrEqual(Val2), Val1, Val2)
    End Function

    ''' <summary>
    ''' Prüft, ob die übergebene Größe <paramref name="Val1"/> sich innerhalb eines Bereiches befindet: <paramref name="LVal"/> &lt; <paramref name="Val1"/> &lt; <paramref name="UVal"/>.
    ''' </summary>
    ''' <param name="Val1">Zu prüfende Größe</param>
    ''' <param name="LVal">Untere Schwelle</param>
    ''' <param name="UVal">Obere schwelle</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Public Function IsInRange(Val1 As Integer, LVal As Integer, UVal As Integer) As Boolean
        Return Val1.IsLargerOrEqual(LVal) And Val1.IsLessOrEqual(UVal)
    End Function


    ''' <summary>
    ''' Prüft, ob der übergebende Wert negativ ist
    ''' </summary>
    ''' <param name="Value">Der zu überprüfende Wert.</param>
    ''' <returns>Es erfolgt ein Vergleich gegen die festgelegte Epsilonschwelle.</returns>
    <Extension()> Friend Function IsNegative(Value As Double) As Boolean
        Return IsLess(Value, 0)
    End Function

    <Extension()> Friend Function IsNegative(Value As Integer) As Boolean
        Return IsLess(Value, 0)
    End Function
    <Extension()> Friend Function IsPositive(Value As Double) As Boolean
        Return IsLarger(Value, 0)
    End Function

    <Extension()> Friend Function IsPositive(Value As Integer) As Boolean
        Return IsLarger(Value, 0)
    End Function
#End Region

#Region "Extensions für Verarbeitung von Zeichenfolgen: String"
    <Extension> Public Function AreEqual(Str1 As String, Str2 As String) As Boolean
        Return String.Compare(Str1, Str2).IsZero
    End Function
    <Extension> Public Function AreNotEqual(Str1 As String, Str2 As String) As Boolean
        Return String.Compare(Str1, Str2).IsNotZero
    End Function
    <Extension> Public Function IsStringEmpty(Str1 As String) As Boolean
        Return Str1.AreEqual(String.Empty)
    End Function
    <Extension> Public Function IsNotStringEmpty(Str1 As String) As Boolean
        Return Str1 IsNot Nothing AndAlso Not Str1.IsStringEmpty
    End Function
    <Extension> Public Function IsStringNothingOrEmpty(Str1 As String) As Boolean
        Return Str1 Is Nothing OrElse Str1.IsStringEmpty
    End Function
    <Extension> Public Function IsNotStringNothingOrEmpty(Str1 As String) As Boolean
        Return Not Str1.IsStringNothingOrEmpty
    End Function
    <Extension> Public Function IsErrorString(Str1 As String) As Boolean
        Return Str1.AreEqual(DfltStrErrorMinusOne)
    End Function
    <Extension> Public Function IsNotErrorString(Str1 As String) As Boolean
        Return Not Str1.IsErrorString
    End Function
    <Extension> Public Function RegExReplace(str1 As String, pattern As String, replacement As String, Optional RegOpt As RegexOptions = RegexOptions.None) As String
        Return Regex.Replace(str1, pattern, replacement, RegOpt)
    End Function
    <Extension> Public Function RegExRemove(str1 As String, pattern As String, Optional RegOpt As RegexOptions = RegexOptions.None) As String
        Return str1.RegExReplace(pattern, DfltStringEmpty, RegOpt)
    End Function
    <Extension> Public Function IsRegExMatch(str1 As String, pattern As String, Optional RegOpt As RegexOptions = RegexOptions.None) As Boolean
        Return Regex.Match(str1, pattern, RegOpt).Success
    End Function
    <Extension> Public Function Left(str1 As String, iLength As Integer) As String
        Return Strings.Left(str1, iLength)
    End Function
    ''' <summary>
    ''' Gibt nur die Numerischen Ziffen eines String zurück
    ''' </summary>
    ''' <param name="sTxt">String der umgewandelt werden soll</param>
    <Extension> Public Function AcceptOnlyNumeric(sTxt As String) As String
        Return sTxt.RegExRemove("\D")
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
    <Extension> Public Function GetSubString(Text As String, StringDavor As String, StringDanach As String, Reverse As Boolean) As String
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
            GetSubString = DfltStrErrorMinusOne
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
    <Extension> Public Function GetSubString(Text As String, StringDavor As String, StringDanach As String) As String
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
    <Extension> Public Function GetSubString(Text As String, StringDavor As String, StringDanach As String, ByRef StartPosition As Integer) As String
        Dim pos(1) As Integer

        pos(0) = InStr(StartPosition, Text, StringDavor, CompareMethod.Text) + Len(StringDavor)
        pos(1) = InStr(pos(0), Text, StringDanach, CompareMethod.Text)

        If Not Not pos(0).AreEqual(Len(StringDavor)) Then
            GetSubString = Mid(Text, pos(0), pos(1) - pos(0))
            StartPosition = pos(1)
        Else
            GetSubString = DfltStrErrorMinusOne
        End If

    End Function

    <Extension> Public Function Split(Text As String, Delimiter As String) As String()
        Return Strings.Split(Text, Delimiter,, CompareMethod.Text)
    End Function

    <Extension> Public Function XMLMaskiereZeichen(Text As String) As String
        ' Nicht zugelassene Zeichen der XML-Notifikation ersetzen.
        ' Zeichen	Notation in XML
        ' <	        &lt;    &#60;
        ' >	        &gt;    &#62;
        ' &	        &amp;   &#38; Zweimal anfügen, da es ansonsten ignoriert wird
        ' "	        &quot;  &#34;
        ' '	        &apos;  &#38;
        Return Text.Replace("&", "&amp;&amp;").Replace("&amp;&amp;#", "&#").Replace("<", "&lt;").Replace(">", "&gt;").Replace(Chr(34), "&quot;").Replace("'", "&apos;")
    End Function

    <Extension> Public Function Join(Text As String(), Separator As String) As String
        Return String.Join(Separator, Text)
    End Function

#End Region

#Region "Extensions für Verarbeitung von Zeichenfolgen: List(Of Telefonat), List(Of VIPEntry)"
    <Extension> Public Sub Insert(ByRef Liste As List(Of Telefonat), item As Telefonat)

        ' Liste initialisieren, falls erforderlich
        If Liste Is Nothing Then Liste = New List(Of Telefonat)

        ' Überprüfe, ob eigene Nummer überhaupt überwacht wird
        If item.EigeneTelNr.Überwacht Then

            ' Eintrag hinzufügen
            Liste.Insert(0, item)
            ' Liste sortieren
            Liste = Liste.OrderByDescending(Function(TF) TF?.ZeitBeginn).ToList

            ' Entferne alle überflüssigen Elemente
            With Liste
                ' PTBNumEntryList = 10
                ' .Count = 11
                ' Start = PTBNumEntryList (Nullbasiert), Anzahl an zu löschenden Elementen = .Count - PTBNumEntryList
                ' Start = 10, Anzahl = 11 - 10 = 1
                If .Count.IsLarger(XMLData.POptionen.TBNumEntryList) Then
                    .RemoveRange(XMLData.POptionen.TBNumEntryList, .Count - XMLData.POptionen.TBNumEntryList)
                End If

            End With
        End If

        ThisAddIn.POutlookRibbons.RefreshRibbon()
    End Sub

    <Extension> Public Sub Insert(ByRef Liste As List(Of VIPEntry), item As VIPEntry)

        ' Liste initialisieren, falls erforderlich
        If Liste Is Nothing Then Liste = New List(Of VIPEntry)

        Liste.Insert(0, item)

        ThisAddIn.POutlookRibbons.RefreshRibbon()
    End Sub
#End Region

#Region "Zahlenkonvertierungen"
    ''' <summary>
    ''' Konvertiert einen String zu Integer.
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns>Den konvertierten String. falls das nicht möglich ist, wird -1 zurückgegeben.</returns>
    <Extension> Public Function ToInt(Text As String) As Integer
        Dim retVal As Integer = -1
        If Integer.TryParse(Text, retVal) Then
            Return retVal
        Else
            Return -1
        End If
    End Function
    <Extension> Public Function ToInt(sWert As Single) As Integer
        Return CInt(sWert)
    End Function
    <Extension> Public Function ToInt(dWert As Double) As Integer
        Return CInt(dWert)
    End Function
    <Extension> Public Function ToInt(bWert As Byte) As Integer
        Return bWert
    End Function

    <Extension> Public Function ToSng(dWert As Double) As Single
        Return CSng(dWert)
    End Function
#End Region

#Region "Bytes"
    <Extension> Public Function Append(Of T)(arr1() As T, arr2 As ICollection(Of T)) As T()
        Dim retVal(arr1.Length + arr2.Count - 1) As T
        Array.Copy(arr1, retVal, arr1.Length)
        arr2.CopyTo(retVal, arr1.Length)
        Return retVal
    End Function
    <Extension> Public Function SplitByte(Of T)(arr As T(), index As Integer) As T()()
        Dim retVal = {New T(index - 1) {}, New T(arr.Length - (index + 1)) {}}
        Array.Copy(arr, 0, retVal(0), 0, retVal(0).Length)
        Array.Copy(arr, index, retVal(1), 0, retVal(1).Length)
        Return retVal
    End Function
    <Extension> Public Function ToBase64String(arr1() As Byte) As String
        Return Convert.ToBase64String(arr1)
    End Function
    <Extension> Public Function FromBase64String(str1 As String) As Byte()
        Return Convert.FromBase64String(str1)
    End Function
#End Region

#Region "Hilfsfunktionen"
    ''' <summary>
    ''' Dekrementiert den Verweiszähler des dem angegebenen COM-Objekt zugeordneten angegebenen Runtime Callable Wrapper (RCW)
    ''' </summary>
    ''' <param name="COMObject">Das freizugebende COM-Objekt.</param>
    <Extension> Public Sub ReleaseComObject(Of T)(COMObject As T)
        If COMObject IsNot Nothing Then
            Try
                Runtime.InteropServices.Marshal.ReleaseComObject(COMObject)
            Catch ex As ArgumentException
                NLogger.Error(ex, "COM-Object ist kein gültiges COM-Objekt: {0}", COMObject.ToString)
            End Try
        End If
    End Sub

    Public Function MsgBox(Meldung As String, Style As MsgBoxStyle, Aufruf As String) As MsgBoxResult
        If Style = MsgBoxStyle.Critical Or Style = MsgBoxStyle.Exclamation Then
            Meldung = String.Format("Die Funktion {0} meldet folgenden Fehler: {1}{2}", Aufruf, Dflt2NeueZeile, Meldung)
            NLogger.Warn(Meldung)
        End If
        Return Microsoft.VisualBasic.MsgBox(Meldung, Style, My.Resources.strDefLongName)
    End Function

    ''' <summary>
    ''' Erstellt einen Timer mit dem übergeben Intervall.
    ''' </summary>
    ''' <param name="Interval">Das Intervall des Timers.</param>
    ''' <returns>Den gerade erstellten Timer.</returns>
    <Obsolete> Public Function SetTimer(Interval As Double) As Timers.Timer
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
    <Obsolete> Public Function KillTimer(Timer As Timers.Timer) As Timers.Timer
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

End Module