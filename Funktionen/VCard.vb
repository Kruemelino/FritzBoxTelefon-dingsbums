Public Module VCard
    Function ReadFromVCard(ByVal vCard As String, ByVal VCProp As String, ByVal Parameter As String) As String
        ' liest von einer vCard bestimmte Daten aus
        ' Parameter  vCard (String):      vCard
        '            VCProp (String):   auszulesende Eigenschaft (z.B. "N", "FN", "ORG")
        '            Parameter (String):  Parameter der Eigenschaft (z.B. "WORK", "HOME")
        '                                 wenn "" dann werden alle Daten mit 'VCProp' ausgegeben
        ' Rückgabewert (String)           die gewünschten Daten (falls vorhanden)

        Dim pos1 As Integer, pos2 As Integer    ' Positionen in Strings
        Dim Line As String  ' aktuell bearbeitete Zeile in vCard
        Dim Prop As String = ""  ' Eigenschaft in akueller Zeile
        Dim Value As String  ' Wert der Eigenschaft
        Dim PropName As String  ' Name der Eigenschaft
        Dim Params As String  ' Parameter in aktueller Zeile
        Dim curParam As String  ' Hilfsstring für 'Parameter'
        Dim Parameter2 As String  ' Hilfsstring für 'Parameter'
        Dim found As Boolean ' Hilfsvariable

        VCProp = UCase(VCProp)
        Parameter = UCase(Parameter)
        ' Störende Sonderzeichen entfernen
        vCard = Replace(vCard, Chr(9), " ", , , CompareMethod.Text)
        vCard = Replace(vCard, "=0D", "", , , CompareMethod.Text)
        vCard = Replace(vCard, "=0A", Chr(10), , , CompareMethod.Text)
        pos1 = 1
        ReadFromVCard = ""
        ' Zeilenweises Abarbeiten der vCard
        Do
            pos1 = InStr(1, vCard, Chr(10), CompareMethod.Text)
            If pos1 = 0 Then Line = vCard Else Line = Left(vCard, pos1 - 1)
            vCard = Mid(vCard, pos1 + 1)
            ' Eigenschaft und Wert separieren
            pos2 = InStr(1, Line, ":", CompareMethod.Text)
            If Not pos2 = 0 Then
                Prop = Trim(Left(Line, pos2 - 1))
                Value = Trim(Mid(Line, pos2 + 1))
                pos2 = InStr(1, Prop, ";", CompareMethod.Text)
                ' Eigenschaftsname und Parameter separieren
                If pos2 = 0 Then
                    PropName = Prop
                    Params = ""
                Else
                    PropName = Trim(Left(Prop, pos2 - 1))
                    Params = Trim(Mid(Prop, pos2 + 1))
                End If
                ' Stimmt der gefundene Eigenschaftsname mit 'VCProp' überein?
                If PropName = VCProp Then
                    If Parameter = "" Then
                        ' kein Parameter angegeben => alle Werte werden zurückgegeben
                        ReadFromVCard = ReadFromVCard & "#" & Trim(Value)
                    Else
                        ' Vergleich der Parameter in der Zeile mit 'Parameter' durchführen
                        found = True
                        Parameter2 = Parameter
                        Do
                            pos2 = InStr(1, Parameter2, ",", CompareMethod.Text)
                            If pos2 = 0 Then curParam = Parameter2 Else curParam = Left(Parameter2, pos2 - 1)
                            Parameter2 = Mid(Parameter2, pos2 + 1)
                            found = found And Not InStr(1, Params, curParam) = 0
                        Loop Until pos2 = 0
                        ' Wenn der Parameter passt, dann Wert zurückgeben
                        If found Then ReadFromVCard = ReadFromVCard & "#" & Trim(Value)
                    End If
                End If
            End If
        Loop Until pos1 = 0 Or Prop = "END"
        If Not Len(ReadFromVCard) = 0 Then ReadFromVCard = Mid(ReadFromVCard, 2)
        ' Zeilenbrüche decodieren (Achtung: nicht konform zu vCard-Regelwerk!!)
        ReadFromVCard = Replace(ReadFromVCard, "=0D", Chr(13), , , CompareMethod.Text)
        ReadFromVCard = Replace(ReadFromVCard, "=0A", Chr(10), , , CompareMethod.Text)
    End Function

    Function ReadFNfromVCard(ByVal vCard As String) As String
        ' liest von einer vCard den vollen Namen (FN bzw. N) aus
        ' Priorität hat dabei die Eigenschaft "N" (Name). Ist sie "" dann wird "FN" (Fullname) verwendet.
        ' Parameter  vCard (String):  vCard
        ' Rückgabewert (String)       der Name in der vCard (falls vorhanden)

        Dim ContactNames As String
        Dim ContactName As String ' kompletter Name ("N") aus vCard
        Dim LastName As String ' Nachname
        Dim FirstName As String = String.Empty ' Vorname
        Dim MiddleName As String = String.Empty ' weiterer Vorname
        Dim Title As String = String.Empty ' akademischer Titel
        Dim Suffix As String = String.Empty ' Namenserweiterung (z.B. sen. oder jun.)
        Dim pos1 As Integer   ' pos1ition innerhalb eines Strings
        Dim pos2 As Integer

        ' Eigenschaft "N" aus vCard lesen
        ContactNames = ReadFromVCard(vCard, "N", "")
        If Not ContactNames = "" Then
            ' wenn nicht "", dann umsortieren, so dass: "Titel Vornamen Nachname Suffix"
            Do
                pos2 = InStr(1, ContactNames, "#", CompareMethod.Text)
                If pos2 = 0 Then
                    ContactName = ContactNames
                Else
                    ContactName = Left(ContactNames, pos2 - 1)
                    ContactNames = Mid(ContactNames, pos2 + 1)
                End If
                pos1 = InStr(1, ContactName, ";", CompareMethod.Text)
                If pos1 = 0 Then
                    LastName = ContactName
                Else
                    LastName = Left(ContactName, pos1 - 1)
                    ContactName = Mid(ContactName, pos1 + 1)
                    pos1 = InStr(1, ContactName, ";", CompareMethod.Text)
                    If pos1 = 0 Then
                        FirstName = ContactName
                    Else
                        FirstName = Left(ContactName, pos1 - 1)
                        ContactName = Mid(ContactName, pos1 + 1)
                        pos1 = InStr(1, ContactName, ";", CompareMethod.Text)
                        If pos1 = 0 Then
                            MiddleName = ContactName
                        Else
                            MiddleName = Left(ContactName, pos1 - 1)
                            ContactName = Mid(ContactName, pos1 + 1)
                            pos1 = InStr(1, ContactName, ";", CompareMethod.Text)
                            If pos1 = 0 Then
                                Title = ContactName
                            Else
                                Title = Left(ContactName, pos1 - 1)
                                ContactName = Mid(ContactName, pos1 + 1)
                                pos1 = InStr(1, ContactName, ";", CompareMethod.Text)
                                If pos1 = 0 Then
                                    Suffix = ContactName
                                Else
                                    Suffix = Left(ContactName, pos1 - 1)
                                End If
                            End If
                        End If
                    End If
                End If
                ReadFNfromVCard = "#" & Trim(Title & " " & Trim(FirstName & " " & MiddleName) & " " & LastName & " " & Suffix)
            Loop Until pos2 = 0
            If Not Len(ReadFNfromVCard) = 0 Then ReadFNfromVCard = Mid(ReadFNfromVCard, 2)
        Else
            ' wenn kein "N" vorhanden ist, dann "FN" verwenden
            ReadFNfromVCard = ReadFromVCard(vCard, "FN", "")
        End If
    End Function

End Module
