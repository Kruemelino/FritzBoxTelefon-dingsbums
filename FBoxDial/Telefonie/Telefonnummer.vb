Imports System.Xml.Serialization
Imports FBoxDial.DfltWerteTelefonie

<Serializable()>
Public Class Telefonnummer
    Implements IEquatable(Of Telefonnummer)
    Implements IDisposable
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Eigenschaften"
    <XmlElement> Public Property Nummer As String
    <XmlAttribute> Public Property EigeneNummer As Boolean
    <XmlAttribute> Public Property Überwacht As Boolean
    <XmlAttribute> Public Property IstGültig As Boolean = True
    <XmlElement> Public Property Landeskennzahl As String
    <XmlElement> Public Property Ortskennzahl As String
    <XmlElement> Public Property Einwahl As String
    <XmlElement> Public Property Durchwahl As String
    <XmlElement> Public Property Formatiert As String
    <XmlElement> Public Property Unformatiert As String
    <XmlElement> Public Property Unterdrückt As Boolean
    <XmlIgnore> Public Property Typ As TelNrType
    <XmlElement> Public Property SIP As Integer
    <XmlElement> Public Property Location As String
    <XmlElement> Public Property AreaCode As String
    <XmlIgnore> Public WriteOnly Property SetNummer As String
        Set
            NLogger.Trace($"SetNummer Start: '{Value}'; '{EigeneNummer}'; '{Ortskennzahl}'; '{Landeskennzahl}'")
            ' Prüfe, ob eine leere Zeichenfolge übergeben wurde
            If Value.IsStringNothingOrEmpty Then
                Unterdrückt = True

            Else
                Nummer = Value

                ' Ermittle die unformatierte Telefonnummer
                Unformatiert = NurZiffern(Nummer)

                ' Ermittle die Kennzahlen LKZ und ONKZ aus der Datei
                SetTelNrTeile()

                ' Formatiere die Telefonnummer
                Formatiert = FormatTelNr()

                ' Ermittle die unformatierte Telefonnummer
                Unformatiert = NurZiffern(Formatiert)
            End If
            NLogger.Trace($"Nummer erfasst: '{Value}'; '{EigeneNummer}'; '{Unformatiert}'; '{Formatiert}'; '{Ortskennzahl}'; '{Landeskennzahl}'")
        End Set
    End Property
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

#End Region
    Public Sub New()

    End Sub

#Region "Funktionen"
    ''' <summary>
    ''' Bereinigt die Telefunnummer von Sonderzeichen wie Klammern und Striche.
    ''' Buchstaben werden wie auf der Telefontastatur in Zahlen übertragen.
    ''' </summary>
    Private Function NurZiffern(Nr As String) As String
        NurZiffern = Nr

        If NurZiffern.IsNotStringNothingOrEmpty Then

            ' Entferne jeden String, der vor einem Doppelpunkt steht (einschließlich :)
            NurZiffern = NurZiffern.ToLower.RegExRemove("^.+:+")

            ' Buchstaben in Ziffen analog zu Telefontasten umwandeln.
            NurZiffern = NurZiffern.RegExReplace("[abc]", "2").
                                    RegExReplace("[def]", "3").
                                    RegExReplace("[ghi]", "4").
                                    RegExReplace("[jkl]", "5").
                                    RegExReplace("[mno]", "6").
                                    RegExReplace("[pqrs]", "7").
                                    RegExReplace("[tuv]", "8").
                                    RegExReplace("[wxyz]", "9").
                                    RegExReplace("^[+]", PDfltVAZ)

            ' Entferne alles, was keine Ziffer ist
            NurZiffern = NurZiffern.RegExRemove("[^0-9]")

            ' Landesvorwahl entfernen bei Inlandsgesprächen (einschließlich ggf. vorhandener nachfolgender 0)
            If Landeskennzahl.IsEqual(XMLData.PTelefonie.LKZ) Then
                NurZiffern = NurZiffern.RegExReplace($"^{PDfltVAZ}{Landeskennzahl}{{1}}[0]?", "0")
            End If

            ' Bei diversen VoIP-Anbietern werden 2 führende Nullen zusätzlich gewählt: Entfernen "000" -> "0"
            NurZiffern = NurZiffern.RegExReplace("^[0]{3}", "0")
        End If
    End Function

    ''' <summary>
    ''' Zerlegt die Telefonnummer in ihre Bestandteile.
    ''' </summary>
    Private Sub SetTelNrTeile()
        Dim _LKZ As Landeskennzahl = Nothing
        Dim _ONKZ As Ortsnetzkennzahlen = Nothing
        Dim TelNr As String

        If Unformatiert.IsNotStringNothingOrEmpty AndAlso Unformatiert.Length.IsLarger(2) Then
            ' Beginne mit der unformatierten Nummer
            TelNr = Unformatiert

            ' Ermittle die Vorwahlen
            ThisAddIn.PVorwahlen.TelNrKennzahlen(Me, _LKZ, _ONKZ)

            ' Weise die Eigenschaften der Landeskennzahl zu
            If _LKZ IsNot Nothing Then
                With _LKZ
                    ' Landeskennzahl ohne VAZ
                    Landeskennzahl = .Landeskennzahl
                    ' Areacode des Landes
                    AreaCode = .Code
                End With
            Else
                IstGültig = False
                NLogger.Warn($"Landeskennzahl für {Unformatiert} konnte nicht ermittelt werden.")
            End If

            ' Weise die Eigenschaften der Ortsnetzkennzahl zu
            If _ONKZ IsNot Nothing Then
                With _ONKZ
                    ' Landeskennzahl ohne VAZ
                    Ortskennzahl = .Ortsnetzkennzahl
                    ' Name des Ortes/zugehörigen Netzes
                    Location = .Name
                End With
            Else
                IstGültig = False
                NLogger.Warn($"Ortsnetzkennzahl für {Unformatiert} konnte nicht ermittelt werden.")
            End If

            ' Einwahl: Landesvorwahl am Anfang entfernen, Ortsvorwahl am Ende Entfernen
            Einwahl = TelNr.RegExRemove($"^{PDfltVAZ}{Landeskennzahl}?").RegExRemove($"^0?{Ortskennzahl}")

            ' Suche eine Durchwahl
            If Nummer.Contains("-") Then
                Durchwahl = Nummer.RegExRemove("^.+\-+ *").Trim()
                ' Einwahl: Druchwahl am Ende entfernen
                Einwahl = Einwahl.RegExRemove($"{Durchwahl}$")
            Else
                Durchwahl = String.Empty
            End If

        End If
    End Sub

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

        If Unterdrückt Then ' Rückgabe Leerstring, falls die Nummer unterdrückt ist.
            Return String.Empty

        ElseIf IstGültig Then ' Führe eine Rufnummernformatierung durch, wenn die Nummer gültig ist.
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
                        FormatTelNr = PDfltMaskeNANP
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
        Else
            ' Die Nummer ist ungültig: Gib die unformatierte Nummer zurück
            NLogger.Info($"Formatierung der ungültigen Telefonnummer '{Nummer}' nicht durchgeführt.")
            Return Unformatiert
        End If
    End Function
#End Region

#Region "IEquatable"
    Public Overloads Function Equals(other As Telefonnummer) As Boolean Implements IEquatable(Of Telefonnummer).Equals
        Return other IsNot Nothing AndAlso Unformatiert.IsEqual(other.Unformatiert)
    End Function
    Public Overloads Function Equals(other As String) As Boolean

        ' Keine internen Nummenr der Box vergleichen
        If other.StartsWith("*") Then Return False

        ' Entferne erstmal alle unnötigen Zeichen:
        Dim AndereNummer As String = NurZiffern(other)

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
                    If EigeneNummer Then
                        Return Equals(New Telefonnummer With {.EigeneNummer = EigeneNummer, .Landeskennzahl = Landeskennzahl, .Ortskennzahl = Ortskennzahl, .SetNummer = AndereNummer})
                    Else
                        Return Equals(New Telefonnummer With {.SetNummer = AndereNummer})
                    End If
                Else
                    NLogger.Trace($"Telefonnummernvergleich false ({other}): '{AndereNummer}'; {Unformatiert}")
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