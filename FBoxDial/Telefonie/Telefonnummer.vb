Imports System.Xml.Serialization
Imports FBoxDial.DfltWerteTelefonie

<Serializable()>
Public Class Telefonnummer
    Implements IEquatable(Of Telefonnummer)
    Implements IDisposable
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Eigenschaften"
    <XmlElement> Public Property EigeneNummerInfo As EigeneNrInfo = Nothing
    <XmlElement> Public Property Original As String
    <XmlElement> Public Property Nummer As String
    <XmlElement> Public Property Landeskennzahl As String
    <XmlElement> Public Property Ortskennzahl As String
    <XmlElement> Public Property Einwahl As String
    <XmlElement> Public Property Durchwahl As String
    <XmlElement> Public Property Formatiert As String
    <XmlElement> Public Property Unformatiert As String
    <XmlElement> Public Property Unterdrückt As Boolean
    ''' <summary>
    ''' Intern: Telefonnummerntyp im Outlook bzw. Fritz!Box Telefonbuch
    ''' </summary>
    <XmlIgnore> Public Property Typ As TelNrType
    <XmlElement> Public Property Location As String
    <XmlElement> Public Property AreaCode As String

    <XmlIgnore> Public ReadOnly Property TellowsNummer As String
        Get
            ' 1. Entferne jeden String, der vor einem Doppelpunkt steht (einschließlich :)
            ' 2. Ersetze das + durch die Verkehrsausscheidungsziffer (VAZ) 00
            ' 3. Entferne alles, was keine Ziffer ist
            ' 4. Wenn die Nummer nicht mit einer Null beginnt, füge die Ortskennzahl (einschließlich führender Null) hinzu

            '      1                          .2                             .3                    .4
            Return Nummer.RegExRemove("^.+:+").RegExReplace("^[+]", PDfltVAZ).RegExRemove("[^0-9]").RegExReplace("^(?!0)", $"0{Ortskennzahl}")
        End Get
    End Property

    <XmlIgnore> ReadOnly Property IstMobilnummer As Boolean
        Get
            If Not Ortskennzahl = String.Empty Then
                Select Case Landeskennzahl
                    Case "49"
                        Return Ortskennzahl.IsRegExMatch("^(15|16|17)")
                    Case "39"
                        Return PDfltMobilIt.Contains(Ortskennzahl)
                    Case Else
                        Return False
                End Select
            Else
                Return False
            End If
        End Get
    End Property

    ''' <summary>
    ''' Gibt an, ob es sich um eine inländische Nummer handelt.
    ''' </summary>
    <XmlIgnore> ReadOnly Property IstInland As Boolean
        Get
            Return Landeskennzahl.IsEqual(XMLData.PTelefonie.LKZ) Or Landeskennzahl.IsNotStringNothingOrEmpty
        End Get
    End Property

    ''' <summary>
    ''' Gibt an, ob es sich um eine Nummer aus dem eigenen Ortsnetz handelt
    ''' </summary>
    <XmlIgnore> ReadOnly Property IstOrtsnetz As Boolean
        Get
            Return IstInland AndAlso Ortskennzahl.IsEqual(XMLData.PTelefonie.OKZ)
        End Get
    End Property

    <XmlIgnore> ReadOnly Property IstNANP As Boolean
        Get
            Return Unformatiert.IsRegExMatch("^(001){1}[2-9]\d{2}[2-9]\d{6}$")
        End Get
    End Property

#End Region

#Region "SetNummer"
    <XmlIgnore> Public WriteOnly Property SetNummer As String
        Set
            NLogger.Trace($"SetNummer Start: '{Value}'; '{Ortskennzahl}'; '{Landeskennzahl}'")
            ' Prüfe, ob eine leere Zeichenfolge übergeben wurde
            If Value.IsStringNothingOrEmpty Then
                _Unterdrückt = True

            Else
                Original = Value

                ' Ermittle die unformatierte Telefonnummer
                Unformatiert = NurZiffern(Original)

                ' Ermittle die Kennzahlen LKZ und ONKZ aus der Datei
                ' Gibt True zurück, wenn die LKZ und ONKZ ermittelt werden konnten.
                If SetTelNrTeile() Then
                    ' Formatiere die Telefonnummer
                    Formatiert = FormatTelNr()

                    ' Ermittle die unformatierte Telefonnummer erneut
                    Unformatiert = NurZiffern(Formatiert, True)
                Else
                    ' Die Nummer ist ungültig
                    NLogger.Info($"Formatierung der ungültigen Telefonnummer '{Original}' nicht durchgeführt.")
                    Formatiert = Unformatiert
                End If

            End If
            NLogger.Trace($"Nummer erfasst: '{Original}'; '{Unformatiert}'; '{Formatiert}'; '{Ortskennzahl}'; '{Landeskennzahl}'")
        End Set
    End Property
#End Region

#Region "Konstruktor"
    Public Sub New()

    End Sub

    Friend Sub New(LKZ As String, OKZ As String)
        Landeskennzahl = LKZ
        Ortskennzahl = OKZ
    End Sub
#End Region

#Region "Funktionen"
    ''' <summary>
    ''' Bereinigt die Telefunnummer von Sonderzeichen wie Klammern und Striche.
    ''' Buchstaben werden wie auf der Telefontastatur in Zahlen übertragen.
    ''' </summary>
    ''' <param name="FürVergleich">Angabe, ob die Telefonnummer für Vergleichszwecke bereinigt werden soll.</param>
    Private Function NurZiffern(Nr As String, Optional FürVergleich As Boolean = False) As String

        ' Initioalen Rückgabewert
        NurZiffern = Nr

        If Nr.IsNotStringNothingOrEmpty Then

            ' Entferne alle Klammerausdrücke, die sich am Ende der Telefonnummer befinden können.
            ' In der Klammer muss ein Text gefolgt von einem Doppelpunkt enthalten sein.
            Nr = Nr.ToLower.RegExRemove("[\(|\{|\[].+:.+[\)|\}|\]]$")

            ' Entferne jeden String, der vor einem Doppelpunkt steht (einschließlich :)
            Nr = Nr.RegExRemove("^.+:+")

            ' Schreibe die relevate noch formatierte Nummer in die Eigenschaft, wenn es sich um die eigene Nummer handelt
            If Not FürVergleich Then _Nummer = Nr.Trim

            ' Buchstaben in Ziffen analog zu Telefontasten umwandeln.
            Nr = Nr.RegExReplace("[abc]", "2").
                    RegExReplace("[def]", "3").
                    RegExReplace("[ghi]", "4").
                    RegExReplace("[jkl]", "5").
                    RegExReplace("[mno]", "6").
                    RegExReplace("[pqrs]", "7").
                    RegExReplace("[tuv]", "8").
                    RegExReplace("[wxyz]", "9").
                    RegExReplace("^[+]", PDfltVAZ)

            ' Entferne alles, was keine Ziffer ist
            Nr = Nr.RegExRemove("[^0-9]")

            ' Landesvorwahl entfernen bei Inlandsgesprächen (einschließlich ggf. vorhandener nachfolgender 0)
            If Landeskennzahl.IsEqual(XMLData.PTelefonie.LKZ) Then
                Nr = Nr.RegExReplace($"^{PDfltVAZ}{Landeskennzahl}{{1}}[0]?", "0")
            End If

            ' Bei diversen VoIP-Anbietern werden 2 führende Nullen zusätzlich gewählt: Entfernen "000" -> "0"
            Return Nr.RegExReplace("^[0]{3}", "0")
        End If
    End Function

    ''' <summary>
    ''' Zerlegt die Telefonnummer in ihre Bestandteile.
    ''' </summary>
    Private Function SetTelNrTeile() As Boolean
        Dim LKZ As Landeskennzahl = Nothing
        Dim ONKZ As Ortsnetzkennzahlen = Nothing

        SetTelNrTeile = True

        If Unformatiert.IsNotStringNothingOrEmpty AndAlso Unformatiert.Length.IsLarger(2) Then

            ' Ermittle die Vorwahlen
            Globals.ThisAddIn.PVorwahlen.TelNrKennzahlen(Me, LKZ, ONKZ)

            ' Weise die Eigenschaften der Landeskennzahl zu
            If LKZ IsNot Nothing Then
                With LKZ
                    ' Landeskennzahl ohne VAZ
                    _Landeskennzahl = .Landeskennzahl
                    ' Areacode des Landes
                    _AreaCode = .Code
                End With
            Else
                SetTelNrTeile = False
                NLogger.Warn($"Landeskennzahl für {Unformatiert} konnte nicht ermittelt werden.")
            End If

            ' Weise die Eigenschaften der Ortsnetzkennzahl zu
            If ONKZ IsNot Nothing Then
                With ONKZ
                    ' Landeskennzahl ohne VAZ
                    Ortskennzahl = .Ortsnetzkennzahl
                    ' Name des Ortes/zugehörigen Netzes
                    Location = .Name
                End With
            Else
                SetTelNrTeile = False
                NLogger.Warn($"Ortsnetzkennzahl für {Unformatiert} konnte nicht ermittelt werden.")
            End If

            If IstNANP Then
                ' NANP Central Office (CO) 3 STellig
                Einwahl = Right(Unformatiert, 7).Left(3)

                ' NANP Suffix 4 Stellig
                Durchwahl = Right(Unformatiert, 4)
            Else
                ' Einwahl: Landesvorwahl am Anfang entfernen, Ortsvorwahl am Anfang Entfernen
                Einwahl = Unformatiert.RegExRemove($"^{PDfltVAZ}{Landeskennzahl}?").RegExRemove($"^0?{Ortskennzahl}")

                ' Suche eine Durchwahl
                If Nummer.Contains("-") Then
                    Durchwahl = Nummer.RegExRemove("^.+\-+ *").Trim()
                    ' Einwahl: Druchwahl am Ende entfernen
                    Einwahl = Einwahl.RegExRemove($"{Durchwahl}$")
                Else
                    Durchwahl = String.Empty
                End If
            End If

        End If
    End Function

    ''' <summary>
    ''' Gruppiert den Telefonnummernteil in Blöcke von 2 Ziffern
    ''' </summary>
    ''' <param name="TelNrTeil">Nummernteil, der gruppiert werden soll</param>
    ''' <param name="Gruppieren">Boolean-Wert, der angibt, ob das Gruppieren durchgeführt werden soll.</param>
    ''' <returns></returns>
    Private Function Gruppiere(TelNrTeil As String, Gruppieren As Boolean) As String
        Gruppiere = TelNrTeil
        If Gruppieren Then
            Dim imax As Integer
            imax = Math.Round(Len(TelNrTeil) / 2 + 0.1).ToInt
            Gruppiere = String.Empty
            For i = 1 To imax
                Gruppiere = String.Concat(Right(TelNrTeil, 2), Chr(32), Gruppiere)
                If Not Len(TelNrTeil) = 1 Then TelNrTeil = Left(TelNrTeil, Len(TelNrTeil) - 2)
            Next
        End If
        Return Gruppiere?.Trim
    End Function

    ''' <summary>
    ''' Formatiert die Telefonnummern nach gängigen Regeln
    ''' </summary>
    Private Function FormatTelNr() As String

        Dim tmpOrtsvorwahl As String
        Dim tmpLandesvorwahl As String
        Dim tmpGruppieren As Boolean = XMLData.POptionen.CBTelNrGruppieren

        FormatTelNr = XMLData.POptionen.TBTelNrMaske

        ' Wenn die Maske keine Durchwahl vorgesehen hat, dann darf die  Druchwahl nicht vergessen werden. Sie muss an die Einwahl angehangen werden.
        If Not FormatTelNr.Contains("%D") Then FormatTelNr = Replace(FormatTelNr, "%N", "%N%D")

        ' Wenn Keine Durchwahl der Telefonnummer vorhanden ist dann entferne in der Maske alles, was hinter der Einwahl befindet
        If Durchwahl.IsStringNothingOrEmpty Then FormatTelNr = FormatTelNr.RegExReplace("%N.*", "%N")

        ' Setze die Ortsvorwahl, wenn immer eine internale Nummer erzeugt werden soll UND
        '                        wenn die Landesvorwahl der Nummer leer ist ODER gleich der eigestellten Landesvorwahl ist UND
        '                        die Ortsvorwahl nicht vorhanden ist

        If XMLData.POptionen.CBintl And IstInland And Ortskennzahl.IsStringNothingOrEmpty Then
            Ortskennzahl = XMLData.PTelefonie.OKZ
        End If

        If Landeskennzahl.IsEqual(XMLData.PTelefonie.LKZ) Then
            tmpOrtsvorwahl = Ortskennzahl
            ' Wenn die Landeskennzahl gleich der hinterlegten Kennzahl entspricht: Inland
            If XMLData.POptionen.CBintl Then
                ' Eine Ortsvorwahl muss vorhanden sein
                If Ortskennzahl.IsStringNothingOrEmpty Then tmpOrtsvorwahl = XMLData.PTelefonie.OKZ
                ' Entferne die führende Null OKZ Prefix
                tmpOrtsvorwahl = tmpOrtsvorwahl.RegExRemove("^(0)+")
                ' Die Landesvorwahl muss gesetzt sein
                tmpLandesvorwahl = Landeskennzahl
            Else
                ' Keine Landesvorwahl ausgeben
                tmpLandesvorwahl = String.Empty
                ' Ortsvorwahl mit führender Null ausgeben
                tmpOrtsvorwahl = $"0{tmpOrtsvorwahl}"
            End If
        Else
            ' Wenn die Landeskennzahl nicht der hinterlegten Kennzahl entspricht: Ausland
            tmpLandesvorwahl = Landeskennzahl
            tmpOrtsvorwahl = Ortskennzahl

            ' Sonderbehandlungen für internationale Nummern
            Select Case Landeskennzahl
                Case "1" ' Nordamerikanischer Nummerierungsplan NANP
                    FormatTelNr = "%L-%O-%N-%D"
                    ' NANP - Nummern werden nicht gruppiert
                    tmpGruppieren = False

                    ' Wenn keine Ortskennzahl gefunden wurde, dann gehe nach dem Schema vor 
                    If Ortskennzahl.IsStringNothingOrEmpty Then
                        ' In der Regel sind die NPA immer 3 Ziffern lang
                        Ortskennzahl = Left(Einwahl, 3)
                        tmpOrtsvorwahl = Ortskennzahl
                        Einwahl = Mid(Einwahl, 4)
                    End If

                    ' Wenn es keine Durchwahl gibt, dann teile die Einwahl nach der dritten Ziffer
                    If Durchwahl.IsStringNothingOrEmpty Then
                        Durchwahl = Mid(Einwahl, 4)
                        Einwahl = Left(Einwahl, 3)
                    End If

                Case "39" ' Italen: Ortsvorwahl ist immer mitzuwählen
                    tmpOrtsvorwahl = If(Ortskennzahl.StartsWith("0"), Ortskennzahl, $"0{Ortskennzahl}")
                Case Else
            End Select
        End If

        If Ortskennzahl.IsStringNothingOrEmpty Then
            ' Maske %L (%O) %N - %D
            ' Wenn keine Ortskennzahl vorhanden ist, dann muss diese bei der Formatierung nicht berücksichtigt werden.
            ' Die Ortskennzahl ist dann in der Einwahl enthalten.
            ' Keine Ortskennzahl: Alles zwischen %L und %N entfernen
            FormatTelNr = FormatTelNr.RegExReplace("[^%L]*%O[^%N]*", If(FormatTelNr.Contains("%L "), Chr(32), String.Empty))
        End If

        ' Füge das + bei Landvoran
        If tmpLandesvorwahl.IsNotStringNothingOrEmpty Then tmpLandesvorwahl = $"+{tmpLandesvorwahl}"

        'Finales Zusammenstellen
        Return FormatTelNr.Replace("%L", tmpLandesvorwahl).Replace("%O", Gruppiere(tmpOrtsvorwahl, tmpGruppieren)).Replace("%N", Gruppiere(Einwahl, tmpGruppieren)).Replace("%D", Gruppiere(Durchwahl, tmpGruppieren)).Trim

    End Function

#End Region

#Region "IEquatable"
    ''' <summary>
    ''' Führt einen Vergleich von <see cref="Telefonnummer"/>-Objekten mit dem übergebenen <see cref="Telefonnummer"/> <paramref name="AndereTelefonnummer"/> durch.
    ''' </summary>
    ''' <param name="AndereTelefonnummer"><see cref="Telefonnummer"/>-Objekt mit der dieses <see cref="Telefonnummer"/>-Objekt verglichen werden soll.</param>
    Public Overloads Function Equals(AndereTelefonnummer As Telefonnummer) As Boolean Implements IEquatable(Of Telefonnummer).Equals
        Return AndereTelefonnummer IsNot Nothing AndAlso Unformatiert.IsEqual(AndereTelefonnummer.Unformatiert)
    End Function

    ''' <summary>
    ''' Führt einen Vergleich von <see cref="Telefonnummer"/>-Objekten mit dem übergebenen <see cref="String"/> <paramref name="AndereTelefonnummer"/> durch.
    ''' </summary>
    ''' <param name="AndereTelefonnummer">Zeichenfolge der Telefonnummer mit der dieses <see cref="Telefonnummer"/>-Objekt verglichen werden soll.</param>
    Public Overloads Function Equals(AndereTelefonnummer As String) As Boolean

        ' Wenn beide Nummern nicht bekannt bzw. unterdrückt sind, dann sind sie gleich.
        If AndereTelefonnummer.IsStringNothingOrEmpty AndAlso Unterdrückt Then Return True

        ' Wenn eine der beiden Nummern nicht bekannt bzw. unterdrückt ist, dann braucht auch kein weiterer Vergleich durchgeführt werden.
        If AndereTelefonnummer.IsStringNothingOrEmpty Xor Unterdrückt Then Return False

        ' Keine internen Nummenr der Box vergleichen
        If AndereTelefonnummer.StartsWith("*") Then Return False

        ' Entferne erstmal alle unnötigen Zeichen:
        Dim AndereNummer As String = NurZiffern(AndereTelefonnummer, True)

        ' Führe einen schnellen Vergleich durch, ob die unformatierte Nummer oder die Einwahl identisch sind.
        Select Case True
            Case Unformatiert.IsEqual(AndereNummer)
                NLogger.Trace($"Telefonnummernvergleich Unformatiert true: '{AndereNummer}'; {Unformatiert}")
                Return True

            Case Einwahl.IsEqual(AndereNummer)
                NLogger.Trace($"Telefonnummernvergleich Einwahl true: '{AndereNummer}'; {Einwahl}")
                Return True

            Case Else
                ' Prüfe, ob die Nummern überhaupt gleich sein können:
                If Unformatiert.Length.IsLargerOrEqual(3) And
                    (Unformatiert.Contains(AndereNummer) Or AndereNummer.Contains(Unformatiert)) Then
                    ' Führe den direkten Vergleich durch, in dem eine neue Telefonnummer angelegt wird
                    ' Bei Vergleich eigener Nummern, übergib die OKZ und LKZ
                    If EigeneNummerInfo IsNot Nothing Then
                        Return Equals(New Telefonnummer(Landeskennzahl, Ortskennzahl) With {.SetNummer = AndereNummer})
                    Else
                        Return Equals(New Telefonnummer With {.SetNummer = AndereNummer})
                    End If
                Else
                    NLogger.Trace($"Telefonnummernvergleich false ({AndereTelefonnummer}): '{AndereNummer}'; {Unformatiert}")
                    Return False
                End If
        End Select

    End Function
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.


    ' IDisposable
    <DebuggerStepThrough>
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ' nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub

    ' Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    <DebuggerStepThrough>
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub

#End Region
End Class