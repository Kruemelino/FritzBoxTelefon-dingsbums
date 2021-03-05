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
    <XmlIgnore> Public Property ID0 As Integer
    <XmlElement> Public Property Landeskennzahl As String
    <XmlElement> Public Property Ortskennzahl As String
    <XmlElement> Public Property Einwahl As String
    <XmlElement> Public Property Durchwahl As String
    <XmlElement> Public Property Formatiert As String
    <XmlElement> Public Property Unformatiert As String
    <XmlElement> Public Property Unbekannt As Boolean
    <XmlIgnore> Public Property Typ As TelNrType
    <XmlElement> Public Property SIP As Integer

    Public Sub New()

    End Sub

    <XmlIgnore> Public WriteOnly Property SetNummer As String
        Set
            NLogger.Trace($"SetNummer Start: '{Value}'; '{EigeneNummer}'; '{Ortskennzahl}'; '{Landeskennzahl}'")
            ' Prüfe, ob eine leere Zeichenfolge übergeben wurde
            If Value.IsStringNothingOrEmpty Then
                Unbekannt = True

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

#End Region
    <XmlIgnore> ReadOnly Property IstMobilnummer As Boolean
        Get
            If Not Ortskennzahl = DfltStringEmpty Then
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

#Region "Funktionen"
    ''' <summary>
    ''' Bereinigt die Telefunnummer von Sonderzeichen wie Klammern und Striche.
    ''' Buchstaben werden wie auf der Telefontastatur in Zahlen übertragen.
    ''' </summary>
    ''' <remarks>Achtung! "*", "#" bleiben bestehen!</remarks>
    Private Function NurZiffern(Nr As String) As String
        NurZiffern = Nr

        If NurZiffern IsNot Nothing And NurZiffern.IsNotStringEmpty Then
            NurZiffern = NurZiffern.ToLower

            ' Entferne jeden String, der vor einem Doppelpunkt steht (einschließlich :)
            NurZiffern = NurZiffern.ToLower.RegExRemove("^.+:+")

            '' Buchstaben in Ziffen analog zu Telefontasten umwandeln.
            NurZiffern = NurZiffern.RegExReplace("[abc]", "2").
                                    RegExReplace("[def]", "3").
                                    RegExReplace("[ghi]", "4").
                                    RegExReplace("[jkl]", "5").
                                    RegExReplace("[mno]", "6").
                                    RegExReplace("[pqrs]", "7").
                                    RegExReplace("[tuv]", "8").
                                    RegExReplace("[wxyz]", "9").
                                    RegExReplace("^[+]", PDfltVAZ)

            ' Alles was jetzt keine Zahlen oder Steuerzeichen direkt entfernen
            NurZiffern = NurZiffern.RegExRemove("[^0-9\#\*]")

            ' Landesvorwahl entfernen bei Inlandsgesprächen (einschließlich ggf. vorhandener nachfolgender 0)

            NurZiffern = NurZiffern.RegExReplace($"^{PDfltVAZ}{If(Landeskennzahl.IsStringNothingOrEmpty, XMLData.PTelefonie.LKZ, Landeskennzahl)}{{1}}[0]?", "0")

            ' Bei diversen VoIP-Anbietern werden 2 führende Nullen zusätzlich gewählt: Entfernen "000" -> "0"
            NurZiffern = NurZiffern.RegExReplace("^[0]{3}", "0")
            End If
    End Function

    ''' <summary>
    ''' Zerlegt die Telefonnummer in ihre Bestandteile.
    ''' </summary>
    Private Sub SetTelNrTeile()
        Dim i, j As Integer
        Dim LKZObj As CLandeskennzahl
        Dim ListeOKZObj As List(Of COrtsnetzkennzahl)
        Dim TelNr As String

        If Unformatiert.IsNotStringEmpty AndAlso Unformatiert.Length.IsLarger(2) Then
            ' Entferne den Stern
            TelNr = Replace(Unformatiert, "*", DfltStringEmpty, , , CompareMethod.Text)

            ' Prüfen: Beginnt die Vorwahl mit der 00, dann ist eine Landesvorwahl enthalten. Wenn nicht, dann nimm die Standard-Landeskennzahl
            If TelNr.StartsWith(PDfltVAZ) Then
                ' Die maximale Länge an LKZ ist 3
                i = 3
                Do
                    LKZObj = ThisAddIn.PCVorwahlen.Kennzahlen.Landeskennzahlen.Find(Function(laKZ) laKZ.Landeskennzahl = TelNr.Substring(2, i))
                    i -= 1
                Loop Until LKZObj IsNot Nothing Or i.IsZero
                ' Eine Landeskennzahl wurde gefunden
                If LKZObj IsNot Nothing Then
                    Landeskennzahl = LKZObj.Landeskennzahl
                Else
                    ' Es wurde keine gültige Landeskennzahl gefunden. Die Nummer ist ggf. falsch zusammengesetzt, oder die LKZ ist nicht in der Liste 
                    NLogger.Warn("Landeskennzahl der Telefonnummer {0} kann nicht ermittelt werden.", Unformatiert)
                    If Not EigeneNummer Then Landeskennzahl = XMLData.PTelefonie.LKZ
                    ' Wähle die LKZ für das Default-Land aus, damit die Routine die Ortskennzahl ermitteln kann
                    LKZObj = ThisAddIn.PCVorwahlen.Kennzahlen.Landeskennzahlen.Find(Function(laKZ) laKZ.Landeskennzahl = Landeskennzahl)
                End If

            Else
                ' Eine Landeskennzahl und eine Ortskennzahl müssen vorhanden sein.
                ' Setze, die Landeskennzahl, falls diese noch nicht gesetzt ist, mit der in den Einstellungen hinterlegten LKZ
                If Landeskennzahl.IsStringNothingOrEmpty Then Landeskennzahl = XMLData.PTelefonie.LKZ

                ' Wähle die LKZ für das Default-Land aus
                LKZObj = ThisAddIn.PCVorwahlen.Kennzahlen.Landeskennzahlen.Find(Function(LObj) LObj.Landeskennzahl = Landeskennzahl)
            End If

            ' Einwahl: Landesvorwahl am Anfang entfernen
            Einwahl = TelNr.RegExRemove($"^{PDfltVAZ}{Landeskennzahl}?")

            ' Extrahiere die Ortsvorwahl, wenn die Telefonnummer mit einer Landesvorwahl beginnt, oder einer führenden Null (Amt)
            If TelNr.StartsWith(PDfltVAZ) Or TelNr.StartsWith(PDfltAmt) Then

                ' Es muss eine Landeskennzahl ermittelt sein.
                ' Hier ist irgendwo ein Bug, dass die ThisAddIn.PCVorwahlen.Kennzahlen.Landeskennzahlen leer ist. Vielleicht war das Addin zu schnell beim automatischen Journalimport.
                If LKZObj Is Nothing Then
                    NLogger.Error("Es konnte keine Landeskennzahl für {0} ermittet werden. Das Laden der Vorwahlen ist{1} abgeschlossen.", TelNr, If(ThisAddIn.PCVorwahlen.Kennzahlen.Landeskennzahlen.Any, DfltStringEmpty, " nicht"))
                    Ortskennzahl = DfltStringEmpty
                Else
                    i = 0
                    If TelNr.StartsWith("0") Then i = 1
                    If TelNr.StartsWith($"{PDfltVAZ}{Landeskennzahl}") Then i = (PDfltVAZ & Landeskennzahl).Length

                    j = TelNr.Length - i
                    Do
                        ListeOKZObj = LKZObj.Ortsnetzkennzahlen.FindAll(Function(OrNKZ) OrNKZ.Ortskennzahl = TelNr.Substring(i, j))
                        j -= 1
                    Loop Until ListeOKZObj.Count.AreEqual(1) Or j.IsZero

                    If ListeOKZObj.Count.AreEqual(1) Then
                        Ortskennzahl = ListeOKZObj.First.Ortskennzahl
                        ' Einwahl: Ortsvorwahl am Anfang entfernen
                    Else
                        Ortskennzahl = DfltStringEmpty
                    End If
                    ListeOKZObj.Clear()
                End If
            Else
                ' Es handelt sich vermutlich um eine Nummer im eigenen Ortsnetz
                ' Setze, die Ortskennzahl, falls diese noch nicht gesetzt ist, mit der in den Einstellungen hinterlegten OKZ
                If Ortskennzahl.IsStringNothingOrEmpty Then Ortskennzahl = XMLData.PTelefonie.OKZ
            End If

            Einwahl = Einwahl.RegExRemove($"^0?{Ortskennzahl}")

            ' Suche eine Durchwahl
            If Nummer.Contains("-") Then
                Durchwahl = Nummer.RegExRemove("^.+\-+ *").Trim()
                ' Einwahl: Druchwahl am Ende entfernen
                Einwahl = Einwahl.RegExRemove($"{Durchwahl}$")
            Else
                Durchwahl = DfltStringEmpty
            End If

            LKZObj = Nothing
            ListeOKZObj = Nothing
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
            Gruppiere = DfltStringEmpty
            For i = 1 To imax
                Gruppiere = String.Concat(Right(TelNrTeil, 2), DfltStringLeerzeichen, Gruppiere)
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

        If Unbekannt Then
            Return DfltStringEmpty
        Else
            FormatTelNr = XMLData.POptionen.TBTelNrMaske

            ' Wenn die Maske keine Durchwahl vorgesehen hat, dann darf die  Druchwahl nicht vergessen werden. Sie muss an die Einwahl angehangen werden.
            If Not FormatTelNr.Contains("%D") Then FormatTelNr = Replace(FormatTelNr, "%N", "%N%D")

            ' Wenn Keine Durchwahl der Telefonnummer vorhanden ist dann entferne in der Maske alles, was hinter der Einwahl befindet
            If Durchwahl.IsStringEmpty Then FormatTelNr = FormatTelNr.RegExReplace("%N.*", "%N")

            ' Setze die Ortsvorwahl, wenn immer eine internale Nummer erzeugt werden soll UND
            '                        wenn die Landesvorwahl der Nummer leer ist ODER gleich der eigestellten Landesvorwahl ist UND
            '                        die Ortsvorwahl nicht vorhanden ist

            If (Landeskennzahl.AreEqual(XMLData.PTelefonie.LKZ) Or Landeskennzahl.AreEqual(DfltStringEmpty)) And XMLData.POptionen.CBintl And Ortskennzahl.IsStringEmpty Then
                Ortskennzahl = XMLData.PTelefonie.OKZ
            End If

            If Landeskennzahl.AreEqual(XMLData.PTelefonie.LKZ) Then
                tmpOrtsvorwahl = Ortskennzahl
                ' Wenn die Landeskennzahl gleich der hinterlegten Kennzahl entspricht: Inland
                If XMLData.POptionen.CBintl Then
                    ' Eine Ortsvorwahl muss vorhanden sein
                    If Ortskennzahl.IsStringEmpty Then tmpOrtsvorwahl = XMLData.PTelefonie.OKZ
                    ' Entferne die führende Null OKZ Prefix
                    tmpOrtsvorwahl = tmpOrtsvorwahl.RegExRemove("^(0)+")
                    ' Die Landesvorwahl muss gesetzt sein
                    tmpLandesvorwahl = Landeskennzahl
                Else
                    ' Keine Landesvorwahl ausgeben
                    tmpLandesvorwahl = DfltStringEmpty
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
                        If Ortskennzahl.IsStringEmpty Then
                            ' In der Regel sind die NPA immer 3 Ziffern lang
                            Ortskennzahl = Left(Einwahl, 3)
                            tmpOrtsvorwahl = Ortskennzahl
                            Einwahl = Mid(Einwahl, 4)
                        End If

                        ' Wenn es keine Durchwahl gibt, dann teile die Einwahl nach der dritten Ziffer
                        If Durchwahl.IsStringEmpty Then
                            Durchwahl = Mid(Einwahl, 4)
                            Einwahl = Left(Einwahl, 3)
                        End If

                    Case "39" ' Italen: Ortsvorwahl ist immer mitzuwählen
                        tmpOrtsvorwahl = If(Ortskennzahl.StartsWith("0"), Ortskennzahl, $"0{Ortskennzahl}")
                    Case Else
                End Select
            End If

            If Ortskennzahl.IsStringEmpty Then
                ' Maske %L (%O) %N - %D
                ' Wenn keine Ortskennzahl vorhanden ist, dann muss diese bei der Formatierung nicht berücksichtigt werden.
                ' Die Ortskennzahl ist dann in der Einwahl enthalten.
                ' Keine Ortskennzahl: Alles zwischen %L und %N entfernen
                FormatTelNr = FormatTelNr.RegExReplace("[^%L]*%O[^%N]*", If(FormatTelNr.Contains("%L "), DfltStringLeerzeichen, DfltStringEmpty))
            End If

            ' Füge das + bei Landvoran
            If tmpLandesvorwahl.IsNotStringEmpty Then tmpLandesvorwahl = $"+{tmpLandesvorwahl}"

            'Finales Zusammenstellen
            Return FormatTelNr.Replace("%L", tmpLandesvorwahl).Replace("%O", Gruppiere(tmpOrtsvorwahl, tmpGruppieren)).Replace("%N", Gruppiere(Einwahl, tmpGruppieren)).Replace("%D", Gruppiere(Durchwahl, tmpGruppieren)).Trim

        End If
    End Function
#End Region

#Region "IEquatable"
    Public Overloads Function Equals(other As Telefonnummer) As Boolean Implements IEquatable(Of Telefonnummer).Equals
        Return other IsNot Nothing AndAlso Unformatiert.AreEqual(other.Unformatiert)
    End Function
    Public Overloads Function Equals(other As String) As Boolean

        ' Keine internen Nummenr der Box vergleichen
        If other.StartsWith("*") Then Return False

        ' Entferne erstmal alle unnötigen Zeichen:
        Dim AndereNummer As String = NurZiffern(other)

        ' Führe einen schnellen Vergleich durch, ob die unformatierte Nummer oder die Einwahl identisch sind.
        Select Case True
            Case Unformatiert.AreEqual(AndereNummer)
                'NLogger.Trace($"Telefonnummernvergleich Unformatiert true: '{AndereNummer}'; {Unformatiert}")
                Return True

            Case Einwahl.AreEqual(AndereNummer)
                'NLogger.Trace($"Telefonnummernvergleich Einwahl true : '{AndereNummer}'; {Einwahl}")
                Return True

            Case Else
                ' Prüfe, ob die Nummern überhaupt gleich sein können:
                If Unformatiert.Length.IsLargerOrEqual(3) And
                    (Unformatiert.Contains(AndereNummer) Or AndereNummer.Contains(Unformatiert)) Then
                    ' Führe den direkten Vergleich durch, in dem eine neue Telefonnummer angelegt wird
                    ' Bei Vergleich eigenener Nummern, übergib die OKZ und LKZ
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
                ' TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' TODO: große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
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
        ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub

#End Region
End Class