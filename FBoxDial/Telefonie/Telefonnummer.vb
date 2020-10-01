Imports System.Xml.Serialization
Imports FBoxDial.DfltWerteTelefonie

<Serializable()>
Public Class Telefonnummer
    Inherits BindableBase

    Implements IEquatable(Of Telefonnummer)
    Implements IDisposable
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Eigenschaften"

    <XmlIgnore> Private _Nummer As String
    <XmlAttribute> Public Property Nummer As String
        Get
            Return _Nummer
        End Get
        Set(value As String)
            SetProperty(_Nummer, value)
        End Set

    End Property
    <XmlIgnore> Private _EigeneNummer As Boolean
    <XmlAttribute> Public Property EigeneNummer As Boolean
        Get
            Return _EigeneNummer
        End Get
        Set(value As Boolean)
            SetProperty(_EigeneNummer, value)
        End Set
    End Property
    <XmlIgnore> Private _Überwacht As Boolean
    <XmlAttribute> Public Property Überwacht As Boolean
        Get
            Return _Überwacht
        End Get
        Set(value As Boolean)
            SetProperty(_Überwacht, value)
        End Set
    End Property
    <XmlIgnore> Private _SIPNode As String
    <XmlAttribute> Public Property SIPNode As String
        Get
            Return _SIPNode
        End Get
        Set(value As String)
            SetProperty(_SIPNode, value)
        End Set
    End Property
    <XmlIgnore> Private _ID0 As Integer
    <XmlIgnore> Public Property ID0 As Integer
        Get
            Return _ID0
        End Get
        Set(value As Integer)
            SetProperty(_ID0, value)
        End Set
    End Property
    <XmlIgnore> Private _ID1 As Integer
    <XmlIgnore> Public Property ID1 As Integer
        Get
            Return _ID1
        End Get
        Set(value As Integer)
            SetProperty(_ID1, value)
        End Set
    End Property
    <XmlElement> Public Property Typ As List(Of TelTypen)
    <XmlIgnore> Private _Landeskennzahl As String
    <XmlElement> Public Property Landeskennzahl As String
        Get
            Return _Landeskennzahl
        End Get
        Set(value As String)
            SetProperty(_Landeskennzahl, value)
        End Set
    End Property
    <XmlIgnore> Private _Ortskennzahl As String
    <XmlElement> Public Property Ortskennzahl As String
        Get
            Return _Ortskennzahl
        End Get
        Set(value As String)
            SetProperty(_Ortskennzahl, value)
        End Set
    End Property
    <XmlIgnore> Private _Einwahl As String
    <XmlElement> Public Property Einwahl As String
        Get
            Return _Einwahl
        End Get
        Set(value As String)
            SetProperty(_Einwahl, value)
        End Set
    End Property
    <XmlIgnore> Private _Durchwahl As String
    <XmlElement> Public Property Durchwahl As String
        Get
            Return _Durchwahl
        End Get
        Set(value As String)
            SetProperty(_Durchwahl, value)
        End Set
    End Property
    <XmlIgnore> Private _Formatiert As String
    <XmlElement> Public Property Formatiert As String
        Get
            Return _Formatiert
        End Get
        Set(value As String)
            SetProperty(_Formatiert, value)
        End Set
    End Property
    <XmlIgnore> Private _Unformatiert As String
    <XmlElement> Public Property Unformatiert As String
        Get
            Return _Unformatiert
        End Get
        Set(value As String)
            SetProperty(_Unformatiert, value)
        End Set
    End Property
    <XmlIgnore> Private _Unbekannt As Boolean
    <XmlElement> Public Property Unbekannt As Boolean
        Get
            Return _Unbekannt
        End Get
        Set(value As Boolean)
            SetProperty(_Unbekannt, value)
        End Set
    End Property
    <XmlIgnore> Private _OutlookTyp As String
    <XmlIgnore> Public Property OutlookTyp As String
        Get
            Return _OutlookTyp
        End Get
        Set(value As String)
            SetProperty(_OutlookTyp, value)
        End Set
    End Property

    <XmlIgnore> Public WriteOnly Property SetNummer As String
        Set(value As String)

            Unbekannt = value.AreEqual(PDfltStringEmpty)

            If Not Unbekannt Then
                If Typ Is Nothing Then Typ = New List(Of TelTypen)

                Nummer = value

                ' Ermittle die unformatierte Telefonnummer
                Unformatiert = NurZiffern(Nummer)

                ' Ermittle die Kennzahlen LKZ und ONKZ aus der Datei
                SetTelNrTeile()

                ' Formatiere die Telefonnummer
                Formatiert = FormatTelNr()

                ' Ermittle die unformatierte Telefonnummer
                Unformatiert = NurZiffern(Formatiert)
            End If
        End Set
    End Property

    <XmlIgnore> ReadOnly Property IstMobilnummer As Boolean
        Get
            If Not Ortskennzahl = PDfltStringEmpty Then
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
#End Region

    Public Sub New()

    End Sub
#Region "Funktionen"
    ''' <summary>
    ''' Bereinigt die Telefunnummer von Sonderzeichen wie Klammern und Striche.
    ''' Buchstaben werden wie auf der Telefontastatur in Zahlen übertragen.
    ''' </summary>
    ''' <remarks>Achtung! "*", "#" bleiben bestehen!</remarks>
    Private Function NurZiffern(ByVal Nr As String) As String
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

            ' Landesvorwahl entfernen bei Inlandsgesprächen (einschließlich nachfolgender 0)
            NurZiffern = NurZiffern.RegExReplace($"^0{XMLData.POptionen.PTBLandesKZ}{{1}}[0]?", "0")

            ' Bei diversen VoIP-Anbietern werden 2 führende Nullen zusätzlich gewählt: Entfernen "000" -> "0"
            NurZiffern = NurZiffern.RegExReplace("^[0]{3}", "0")
        End If
    End Function

    ''' <summary>
    ''' Zerlegt die Telefonnummer in ihre Bestandteile.
    ''' </summary>
    Private Sub SetTelNrTeile()
        Dim i, j As Integer
        Dim tmpLKZ As CLandeskennzahl
        Dim tmpONKZ As List(Of COrtsnetzkennzahl)
        Dim TelNr As String

        If Unformatiert.IsNotStringEmpty AndAlso Unformatiert.Length.IsLarger(2) Then
            ' Entferne den Stern
            TelNr = Replace(Unformatiert, "*", PDfltStringEmpty, , , CompareMethod.Text)

            ' Prüfen: Beginnt die Vorwahl mit der 00, dann ist eine Landesvorwahl enthalten. Wenn nicht, dann nimm die Standard-Landesvorwahl
            If TelNr.StartsWith(PDfltVAZ) Then
                ' Die maximale Länge an LKZ ist 3
                i = 3
                Do
                    tmpLKZ = ThisAddIn.PCVorwahlen.Kennzahlen.Landeskennzahlen.Find(Function(laKZ) laKZ.Landeskennzahl = TelNr.Substring(2, i))
                    i -= 1
                Loop Until tmpLKZ IsNot Nothing Or i.IsZero
                ' Eine Landeskennzahl wurde gefunden
                If tmpLKZ IsNot Nothing Then
                    Landeskennzahl = tmpLKZ.Landeskennzahl
                Else
                    ' Es wurde keine gültige Landeskennzahl gefunden. Die Nummer ist ggf. falsch zusammengesetzt, oder die LKZ ist nicht in der Liste 
                    NLogger.Warn("Landeskennzahl der Telefonnummer {0} kann nicht ermittelt werden.", Unformatiert)
                    Landeskennzahl = XMLData.POptionen.PTBLandesKZ
                    ' Wähle die LKZ für das Default-Land aus, damit die Routine die Ortskennzahl ermitteln kann
                    tmpLKZ = ThisAddIn.PCVorwahlen.Kennzahlen.Landeskennzahlen.Find(Function(laKZ) laKZ.Landeskennzahl = Landeskennzahl)
                End If

            Else
                Landeskennzahl = XMLData.POptionen.PTBLandesKZ
                ' Wähle die LKZ für das Default-Land aus
                tmpLKZ = ThisAddIn.PCVorwahlen.Kennzahlen.Landeskennzahlen.Find(Function(laKZ) laKZ.Landeskennzahl = Landeskennzahl)
            End If

            ' Einwahl: Landesvorwahl am Anfang entfernen
            Einwahl = TelNr.RegExRemove($"^{PDfltVAZ}{Landeskennzahl}?")

            ' Extrahiere die Ortsvorwahl, wenn die Telefonnummer mit einer Landesvorwahl beginnt, oder einer führenden Null (Amt)
            If TelNr.StartsWith(PDfltVAZ) Or TelNr.StartsWith(PDfltAmt) Then

                ' Es muss eine Landeskennzahl ermittelt sein.
                ' Hier ist irgendwo ein Bug, dass die ThisAddIn.PCVorwahlen.Kennzahlen.Landeskennzahlen leer ist. Vielleicht war das Addin zu schnell beim Automatischen Journalimport.
                If tmpLKZ Is Nothing Then
                    NLogger.Error("Es konnte keine Landeskennzahl für {0} ermittet werden. Das Laden der Vorwahlen ist{1} abgeschlossen.", TelNr, If(ThisAddIn.PCVorwahlen.Kennzahlen.Landeskennzahlen.Any, PDfltStringEmpty, " nicht"))
                    Ortskennzahl = PDfltStringEmpty
                Else
                    i = 0
                    If TelNr.StartsWith("0") Then i = 1
                    If TelNr.StartsWith($"{PDfltVAZ}{Landeskennzahl}") Then i = (PDfltVAZ & Landeskennzahl).Length

                    j = TelNr.Length - i
                    Do
                        tmpONKZ = tmpLKZ.Ortsnetzkennzahlen.FindAll(Function(OrNKZ) OrNKZ.Ortskennzahl = TelNr.Substring(i, j))
                        j -= 1
                    Loop Until tmpONKZ.Count.AreEqual(1) Or j.IsZero

                    If tmpONKZ.Count.AreEqual(1) Then
                        Ortskennzahl = tmpONKZ.First.Ortskennzahl
                        ' Einwahl: Ortsvorwahl am Anfang entfernen
                    Else
                        Ortskennzahl = PDfltStringEmpty
                    End If
                    tmpONKZ.Clear()
                End If
            Else
                ' es handelt sich vermutlich um eine Nummer im eigenen Ortsnetz
                Ortskennzahl = XMLData.POptionen.PTBOrtsKZ
            End If

            Einwahl = Einwahl.RegExRemove($"0?{Ortskennzahl}")

            ' Suche eine Durchwahl
            If Nummer.Contains("-") Then
                Durchwahl = Nummer.RegExRemove("^.+\-+ *").Trim()
                ' Einwahl: Druchwahl am Ende entfernen
                Einwahl = Einwahl.RegExRemove($"{Durchwahl}$")
            Else
                Durchwahl = PDfltStringEmpty
            End If

            tmpLKZ = Nothing
            tmpONKZ = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Gruppiert den Telefonnummernteil in Blöcke von 2 Ziffern
    ''' </summary>
    ''' <param name="TelNrTeil">Nummernteil, der gruppiert werden soll</param>
    ''' <param name="Gruppieren">Boolean-Wert, der angibt, ob das Gruppieren durchgeführt werden soll.</param>
    ''' <returns></returns>
    Private Function Gruppiere(ByVal TelNrTeil As String, ByVal Gruppieren As Boolean) As String
        Gruppiere = TelNrTeil
        If Gruppieren Then
            Dim imax As Integer
            imax = Math.Round(Len(TelNrTeil) / 2 + 0.1).ToInt
            Gruppiere = PDfltStringEmpty
            For i = 1 To imax
                Gruppiere = String.Concat(Right(TelNrTeil, 2), PDfltStringLeerzeichen, Gruppiere)
                If Not Len(TelNrTeil) = 1 Then TelNrTeil = Left(TelNrTeil, Len(TelNrTeil) - 2)
            Next
        End If
        Return Gruppiere.Trim
    End Function

    ''' <summary>
    ''' Formatiert die Telefonnummern nach gängigen Regeln
    ''' </summary>
    Private Function FormatTelNr() As String

        Dim tmpOrtsvorwahl As String
        Dim tmpLandesvorwahl As String
        Dim tmpGruppieren As Boolean = XMLData.POptionen.PCBTelNrGruppieren

        If Unbekannt Then
            Return PDfltStringEmpty
        Else
            FormatTelNr = XMLData.POptionen.PTBTelNrMaske

            ' Wenn die Maske keine Durchwahl vorgesehen hat, dann darf die  Druchwahl nicht vergessen werden. Sie muss an die Einwahl angehangen werden.
            If Not FormatTelNr.Contains("%D") Then FormatTelNr = Replace(FormatTelNr, "%N", "%N%D")

            ' Wenn Keine Durchwahl der Telefonnummer vorhanden ist dann entferne in der Maske alles, was hinter der Einwahl befindet
            If Durchwahl.IsStringEmpty Then FormatTelNr = FormatTelNr.RegExReplace("%N.*", "%N")

            ' Setze die Ortsvorwahl, wenn immer eine internale Nummer erzeugt werden soll UND
            '                        wenn die Landesvorwahl der Nummer leer ist ODER gleich der eigestellten Landesvorwahl ist UND
            '                        die Ortsvorwahl nicht vorhanden ist

            If (Landeskennzahl.AreEqual(XMLData.POptionen.PTBLandesKZ) Or Landeskennzahl.AreEqual(PDfltStringEmpty)) And XMLData.POptionen.PCBintl And Ortskennzahl.IsStringEmpty Then
                Ortskennzahl = XMLData.POptionen.PTBOrtsKZ
            End If

            If Landeskennzahl.AreEqual(XMLData.POptionen.PTBLandesKZ) Then
                tmpOrtsvorwahl = Ortskennzahl
                ' Wenn die Landeskennzahl gleich der hinterlegten Kennzahl entspricht: Inland
                If XMLData.POptionen.PCBintl Then
                    ' Eine Ortsvorwahl muss vorhanden sein
                    If Ortskennzahl.IsStringEmpty Then tmpOrtsvorwahl = XMLData.POptionen.PTBOrtsKZ
                    ' Entferne die führende Null
                    tmpOrtsvorwahl = tmpOrtsvorwahl.RegExRemove("^(0)+")
                    ' Die Landesvorwahl muss gesetzt sein
                    tmpLandesvorwahl = Landeskennzahl
                Else
                    ' Keine Landesvorwahl ausgeben
                    tmpLandesvorwahl = PDfltStringEmpty
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
                FormatTelNr = FormatTelNr.RegExReplace("[^%L]*%O[^%N]*", If(FormatTelNr.Contains("%L "), PDfltStringLeerzeichen, PDfltStringEmpty))
            End If

            ' Füge das + bei Landvoran
            If tmpLandesvorwahl.IsNotStringEmpty Then tmpLandesvorwahl = $"+{tmpLandesvorwahl}"

            'Finales Zusammenstellen
            Return FormatTelNr.Replace("%L", tmpLandesvorwahl).Replace("%O", Gruppiere(tmpOrtsvorwahl, tmpGruppieren)).Replace("%N", Gruppiere(Einwahl, tmpGruppieren)).Replace("%D", Gruppiere(Durchwahl, tmpGruppieren)).Trim

        End If
    End Function
#End Region

#Region "IEquatable"
    Public Overloads Function Equals(ByVal other As Telefonnummer) As Boolean Implements IEquatable(Of Telefonnummer).Equals
        Return other IsNot Nothing AndAlso Unformatiert.AreEqual(other.Unformatiert)
    End Function
    Public Overloads Function Equals(ByVal other As String) As Boolean
        ' Erstelle aus other eine Telefonnummer

        Using tmpTelNr As New Telefonnummer With {.SetNummer = other}
            Return other IsNot Nothing AndAlso Unformatiert.AreEqual(tmpTelNr.Unformatiert)
        End Using
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
            If Typ IsNot Nothing Then Typ.Clear()
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