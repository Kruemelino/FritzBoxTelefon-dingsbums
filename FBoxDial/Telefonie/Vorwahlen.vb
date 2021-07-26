Imports System.Xml.Serialization
Imports FBoxDial.DfltWerteTelefonie
<Serializable(), XmlRoot("KZ")> Public Class Kennzahlen
    <XmlElement("LKZ")> Public Property Landeskennzahlen As List(Of Landeskennzahl)
End Class

<Serializable(), XmlType("LKZ")> Public Class Landeskennzahl
    <XmlAttribute("n")> Public Property Landeskennzahl As String
    <XmlAttribute("Code")> Public Property Code As String
    <XmlElement("ONKZ")> Public Property Ortsnetzkennzahlen As List(Of Ortsnetzkennzahlen)
End Class

<Serializable(), XmlType("ONKZ")> Public Class Ortsnetzkennzahlen
    <XmlAttribute("n")> Public Property Ortsnetzkennzahl As String
    <XmlAttribute("Name")> Public Property Name As String
End Class

Friend Class Vorwahlen
    Public Property Kennzahlen As Kennzahlen
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private ReadOnly Property GetDefaultLKZ() As Landeskennzahl
        Get
            Return GetDefaultLKZ(XMLData.PTelefonie.LKZ)
        End Get
    End Property
    Private ReadOnly Property GetDefaultLKZ(LKZString As String) As Landeskennzahl
        Get
            If LKZString.IsStringNothingOrEmpty Then
                NLogger.Warn("Übergebener String ist Null oder Nothing.")
                Return New Landeskennzahl With {.Landeskennzahl = DfltStringEmpty, .Ortsnetzkennzahlen = New List(Of Ortsnetzkennzahlen)}
            Else
                Return Kennzahlen.Landeskennzahlen.Find(Function(laKZ) laKZ.Landeskennzahl = LKZString)
            End If
        End Get
    End Property

    Private ReadOnly Property GetDefaultONKZ() As Ortsnetzkennzahlen
        Get
            Return GetDefaultLKZ?.Ortsnetzkennzahlen.Find(Function(OKZ) OKZ.Ortsnetzkennzahl = XMLData.PTelefonie.OKZ)
        End Get
    End Property

    Friend Async Function LadeVorwahlen() As Threading.Tasks.Task(Of Kennzahlen)
        Return Await DeserializeAsyncXML(Of Kennzahlen)(My.Resources.Vorwahlen, False)
    End Function

    Friend Sub TelNrKennzahlen(TelNr As Telefonnummer, ByRef _LKZ As Landeskennzahl, ByRef _ONKZ As Ortsnetzkennzahlen)
        Dim LKZListe As List(Of Landeskennzahl)
        ' Landeskennzahl ermitteln
        LKZListe = GetTelNrLKZListe(TelNr)

        ' Ortsnetzkennzahl ermitteln
        If LKZListe.Any Then
            _ONKZ = TelNrONKZ(TelNr, LKZListe)
            _LKZ = LKZListe.First
        End If

    End Sub

    ''' <summary>
    ''' Ermittelt die Landeskennzahlen aus einer unformatierten Telefonnummer. 
    ''' Es kann sein, dass mehrere Landeskennzeahlen ermittelt werden. Dies ist vor allem für die 1 der Fall. Weitere Beispiele: 7, 44
    ''' </summary>
    Private Function GetTelNrLKZListe(TelNr As Telefonnummer) As List(Of Landeskennzahl)
        Dim i As Integer
        Dim LKZListe As New List(Of Landeskennzahl)

        With TelNr
            ' Prüfe, ob die Telefonnummer eine Landeskennzahl enthält
            If .Landeskennzahl.IsNotStringNothingOrEmpty Then
                LKZListe.Add(GetDefaultLKZ(.Landeskennzahl))
            Else
                ' Beginnt die Nummer mit der Verkehrsausscheidungsziffer (VAZ)
                If .Unformatiert.StartsWith(PDfltVAZ) Then
                    ' Die maximale Länge einer LKZ ist 3
                    i = 3

                    ' Stelle sicher, dass die Telefonnummer ausreichend lang ist.
                    If .Unformatiert.Length.IsLargerOrEqual(2 + i) Then
                        ' Es kann mehrere Treffer geben
                        Do
                            LKZListe = Kennzahlen.Landeskennzahlen.FindAll(Function(laKZ) laKZ.Landeskennzahl = .Unformatiert.Substring(2, i))
                            i -= 1
                        Loop Until LKZListe.Any Or i.IsZero
                    End If

                    If LKZListe.Any Then
                        ' Es wurden Einträge gefunden
                        If LKZListe.Count.AreEqual(1) Then
                            NLogger.Trace($"Eine Landeskennzahl der Telefonnummer { .Unformatiert} wurde ermittelt: '{LKZListe.First.Landeskennzahl}' ({LKZListe.First.Code})")
                        Else
                            NLogger.Trace($"{LKZListe.Count} Landeskennzahlen der Telefonnummer { .Unformatiert} wurde ermittelt: '{LKZListe.First.Landeskennzahl}'")
                        End If

                    Else
                        ' Es wurde keine gültige Landeskennzahl gefunden. Die Nummer ist ggf. falsch zusammengesetzt, oder die LKZ ist nicht in der Liste 
                        NLogger.Warn($"Landeskennzahl der Telefonnummer '{ .Unformatiert}' kann nicht ermittelt werden.")
                        'If Not TelNr.EigeneNummer Then TelNr.Landeskennzahl = XMLData.PTelefonie.LKZ
                    End If
                End If
            End If

            If Not LKZListe.Any Then
                LKZListe.Add(GetDefaultLKZ)
                NLogger.Trace($"Standard-Landeskennzahl der Telefonnummer '{ .Unformatiert}' wurde gesetzt: {LKZListe.First.Landeskennzahl} ({LKZListe.First.Code})")
            End If
        End With
        Return LKZListe
    End Function

    Private Function TelNrONKZ(TelNr As Telefonnummer, _LKZ As List(Of Landeskennzahl)) As Ortsnetzkennzahlen
        Dim i, j As Integer
        Dim _ONKZ As Ortsnetzkennzahlen = Nothing
        Dim ONKZListe As List(Of Ortsnetzkennzahlen)

        With TelNr.Unformatiert

            If .StartsWith(PDfltVAZ) Or .StartsWith(PDfltAmt) Then
                ' Es können mehrere Landeskennzahlen passen: z.B. 1, 7, 44
                ' Schleife durch alle Landeskennzahlen
                For Each LKZ In _LKZ
                    i = 0

                    If .StartsWith(PDfltAmt) Then i = 1
                    If .StartsWith($"{PDfltVAZ}{LKZ.Landeskennzahl}") Then i = $"{PDfltVAZ}{LKZ.Landeskennzahl}".Length
                    If .StartsWith($"{PDfltVAZ}{LKZ.Landeskennzahl}{PDfltAmt}") Then i = $"{PDfltVAZ}{LKZ.Landeskennzahl}{PDfltAmt}".Length

                    j = .Length - i
                    Do
                        ONKZListe = LKZ.Ortsnetzkennzahlen.FindAll(Function(OrNKZ) OrNKZ.Ortsnetzkennzahl = .Substring(i, j))
                        j -= 1
                    Loop Until ONKZListe.Count.AreEqual(1) Or j.IsZero

                    If ONKZListe.Count.AreEqual(1) Then

                        _ONKZ = ONKZListe.First
                        NLogger.Trace($"Ortsnetzkennzahl der Telefonnummer '{TelNr.Unformatiert}' wurde ermittelt: {_ONKZ.Ortsnetzkennzahl} ({_ONKZ.Name})")

                        _LKZ.Clear()
                        _LKZ.Add(LKZ)

                        Exit For
                    End If

                    ONKZListe.Clear()
                Next

            Else
                ' Es handelt sich vermutlich um eine Nummer im eigenen Ortsnetz
                ' Setze, die Ortskennzahl, falls diese noch nicht gesetzt ist, mit der in den Einstellungen hinterlegten OKZ
                _ONKZ = GetDefaultONKZ
                If _ONKZ IsNot Nothing Then
                    NLogger.Trace($"Standard-Ortsnetzkennzahl der Telefonnummer '{TelNr.Unformatiert}' wurde gesetzt: '{_ONKZ.Ortsnetzkennzahl}' ({_ONKZ.Name})")
                Else
                    NLogger.Warn($"Die Standard-Ortsvorwahl ({XMLData.PTelefonie.OKZ}) konnte nicht aus der LKZ {_LKZ.First.Landeskennzahl} ermittelt werden.")
                End If
            End If

        End With

        Return _ONKZ
    End Function


End Class

