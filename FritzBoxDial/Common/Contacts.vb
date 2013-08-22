Public Class Contacts
    Private ini As InI
    Private hf As Helfer
    Private DateiPfad As String
    ReadOnly UserProperties() As String = Split("FBDB-AssistantTelephoneNumber;FBDB-BusinessTelephoneNumber;FBDB-Business2TelephoneNumber;FBDB-CallbackTelephoneNumber;FBDB-CarTelephoneNumber;FBDB-CompanyMainTelephoneNumber;FBDB-HomeTelephoneNumber;FBDB-Home2TelephoneNumber;FBDB-ISDNNumber;FBDB-MobileTelephoneNumber;FBDB-OtherTelephoneNumber;FBDB-PagerNumber;FBDB-PrimaryTelephoneNumber;FBDB-RadioTelephoneNumber;FBDB-BusinessFaxNumber;FBDB-HomeFaxNumber;FBDB-OtherFaxNumber", ";", , CompareMethod.Text)

    Public Sub New(ByVal IniPath As String, ByVal iniKlasse As InI, ByVal HelferKlasse As Helfer)

        ' Zuweisen der an die Klasse übergebenen Parameter an die internen Variablen, damit sie in der Klasse global verfügbar sind
        ini = iniKlasse
        DateiPfad = IniPath
        hf = HelferKlasse
    End Sub

    Friend Function FindeKontakt(ByRef TelNr As String, _
                                 ByVal Absender As String, _
                                 ByVal LandesVW As String, _
                                 ByVal Ordner As Outlook.MAPIFolder, _
                                 ByVal NamensRaum As Outlook.NameSpace) _
                             As Outlook.ContactItem

        ' sucht in der Kontaktdatenbank nach der TelNr/Email
        ' Parameter:  TelNr (String):           Telefonnummer des zu Suchenden
        '             Absender (String):        AbsenderEmailadresse, des Suchenden
        '             LandesVW (String):        eigene Landesvorwahl
        '             KontaktID (String):       ID der Kontaktdaten falls was gefunden wurde (nur Rückgabewert)
        '             Ordner (Object):          der zu durchsuchende Kontaktordner (für die rekursive Suche)
        '             NamensRaum:               Der Namespace, falls übergeordnet durchsucht werden soll.
        ' Rückgabewert (Outlook.ContactItem):   Der gefundene Kontakt

        ' !!!!!!!!!!!!!!!!!!!! ACHTUNG WICHTIG !!!!!!!!!!!!!!!!!!!!
        ' Es muss entweder Ordner ODER Namensraum verwendet werden.
        ' fehlt beides, kann die Funktion nichts zurückbringen !!!!

        Dim gefunden As Outlook.ContactItem = Nothing ' was gefunden?

        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner
        Dim aktKontakt As Outlook.ContactItem  ' aktueller Kontakt
        Dim alleTE(14) As String  ' alle TelNr/Email eines Kontakts
        Dim sFilter As String = vbNullString

        ' Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        If Not NamensRaum Is Nothing Then
            Dim j As Integer = 1
            Do While (j <= NamensRaum.Folders.Count) And (gefunden Is Nothing)
                gefunden = FindeKontakt(TelNr, Absender, LandesVW, NamensRaum.Folders.Item(j), Nothing)
                j = j + 1
                Windows.Forms.Application.DoEvents()
            Loop
            aktKontakt = Nothing
            Return gefunden
        End If

        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then
            If Not Absender = vbNullString Then
                sFilter = String.Concat("[Email1Address] = """, Absender, """ OR [Email2Address] = """, Absender, """ OR [Email3Address] = """, Absender, """")
                gefunden = CType(Ordner.Items.Find(sFilter), Outlook.ContactItem)
            Else
                If CBool(ini.Read(DateiPfad, "Optionen", "CBIndex", "True")) Then
                    Dim Personen As Outlook.Items = Ordner.Items
                    ' In Outlook 2003 funktioniert die Verkettung mit OR nicht.
#If OVer = 11 Then
                    For Each UserProperty In UserProperties
                        sFilter = "[" & UserProperty & "] = """ & TelNr & """"
                        Try
                            gefunden = CType(Personen.Find(sFilter), Outlook.ContactItem)
                        Catch ex As Exception
                        End Try
                        If Not gefunden Is Nothing Then Exit For
                    Next

                    'For Each UserProperty In UserProperties
                    '    sFilter = String.Concat("@SQL=""http://schemas.microsoft.com/mapi/string/{00020329-0000-0000-C000-000000000046}/", UserProperty, "/0x0000001f"" = '", TelNr, "'")
                    '    gefunden = CType(Personen.Find(sFilter), Outlook.ContactItem)
                    '    If Not gefunden Is Nothing Then Exit For
                    'Next
#Else
                    Dim JoinFilter(UserProperties.Length - 1) As String
                    For i = 0 To UserProperties.Length - 1
                        JoinFilter(i) = String.Concat("""http://schemas.microsoft.com/mapi/string/{00020329-0000-0000-C000-000000000046}/", UserProperties(i), "/0x0000001f"" = '", TelNr, "'")
                    Next
                    sFilter = "@SQL=" & String.Join(" OR ", JoinFilter)
                    gefunden = CType(Ordner.Items.Find(sFilter), Outlook.ContactItem)
#End If
                    If Not gefunden Is Nothing Then TelNr = NrFormat(gefunden, TelNr, LandesVW)
                End If
            End If
        End If
        ' Unterordner werden rekursiv durchsucht
        iOrdner = 1
        Do While (iOrdner <= Ordner.Folders.Count) And (gefunden Is Nothing)
            gefunden = FindeKontakt(TelNr, Absender, LandesVW, Ordner.Folders.Item(iOrdner), Nothing)
            iOrdner = iOrdner + 1
            Windows.Forms.Application.DoEvents()
        Loop
        FindeKontakt = gefunden
        aktKontakt = Nothing
    End Function '(FindeKontakt)

    Friend Sub ErstelleKontakt(ByRef KontaktID As String, ByRef StoreID As String, ByVal vCard As String, ByVal TelNr As String)
        Dim FritzFolderExists As Boolean = False
        Dim Kontakt As Outlook.ContactItem = Nothing        ' Objekt des Kontakteintrags
        If Not vCard = "" Then
            Dim olContactsFolder As Outlook.MAPIFolder = ThisAddIn.oApp.GetNamespace("MAPI").GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)
            Dim olFolder As Outlook.MAPIFolder = olContactsFolder.Folders.GetFirst

            For Each olFolder In olContactsFolder.Folders
                If olFolder.Name = "Fritz!Box" Then
                    FritzFolderExists = True
                    Exit For
                End If
            Next 'olFolder
            If Not FritzFolderExists Then olFolder = olContactsFolder.Folders.Add("Fritz!Box")
            olContactsFolder = Nothing
            Kontakt = CType(ThisAddIn.oApp.CreateItem(Outlook.OlItemType.olContactItem), Outlook.ContactItem)
            Kontakt = CType(Kontakt.Move(olFolder), Outlook.ContactItem)

            olFolder = Nothing

            vCard2Contact(vCard, Kontakt)

            With Kontakt
                If Not hf.nurZiffern(.BusinessTelephoneNumber, "0049") = hf.nurZiffern(TelNr, "0049") And Not .BusinessTelephoneNumber = "" Then
                    .Business2TelephoneNumber = hf.formatTelNr(TelNr)
                ElseIf Not hf.nurZiffern(.HomeTelephoneNumber, "0049") = hf.nurZiffern(TelNr, "0049") And Not .HomeTelephoneNumber = "" Then
                    .Home2TelephoneNumber = hf.formatTelNr(TelNr)
                End If
                .Categories = "Fritz!Box (automatisch erstellt)" 'Alle Kontakte, die erstellt werden, haben diese Kategorie. Damit sind sie einfach zu erkennen
                .Body = .Body & vbCrLf & "Erstellt durch das Fritz!Box Telefon-dingsbums am " & System.DateTime.Now
                If Not CBool(ini.Read(DateiPfad, "Optionen", "CBIndexAus", "False")) Then
                    IndiziereKontakt(Kontakt, True)
                End If
                .Save()
                KontaktID = .EntryID
                StoreID = CType(.Parent, Outlook.MAPIFolder).StoreID
                hf.LogFile("Kontakt " & Kontakt.FullName & " wurde erstellt")
            End With

        End If
        Kontakt = Nothing
    End Sub

    Sub KontaktErstellen()
        Dim olAuswahl As Outlook.Inspector ' das aktuelle Inspector-Fenster (Kontakt oder Journal)
        Dim pos1 As Integer
        Dim pos2 As Integer
        Dim vCard As String
        Dim Journal As Outlook.JournalItem
        Dim Kontakt As Outlook.ContactItem ' Objekt des Kontakteintrags
        Dim TelNr As String

        olAuswahl = ThisAddIn.oApp.ActiveInspector
        If Not olAuswahl Is Nothing Then
            If TypeOf olAuswahl.CurrentItem Is Outlook.JournalItem Then
                Journal = CType(olAuswahl.CurrentItem, Outlook.JournalItem)
                With Journal
                    If Not InStr(1, Journal.Categories, "FritzBox Anrufmonitor", CompareMethod.Text) = 0 Then
                        ' Telefonnummer aus dem .Body herausfiltern
                        TelNr = hf.StringEntnehmen(.Body, "Tel.-Nr.: ", "Status: ")
                        ' vCard aus dem .Body herausfiltern
                        pos1 = InStr(1, .Body, "BEGIN:VCARD", CompareMethod.Text)
                        pos2 = InStr(1, .Body, "END:VCARD", CompareMethod.Text) + 9
                        ' Wenn vCard vorhanden ist, dann Kontakt erstellen
#If Not OVer = 15 Then
                        If pos1 = 0 Or pos2 = 9 Then
                            Dim olLink As Outlook.Link = Nothing
                            Dim olContact As Outlook.ContactItem
                            For Each olLink In .Links
                                Try
                                    If TypeOf olLink.Item Is Outlook.ContactItem Then
                                        olContact = CType(olLink.Item, Outlook.ContactItem)
                                        olContact.Display()
                                        hf.NAR(olContact) : olContact = Nothing
                                        Exit Sub
                                    End If
                                Catch
                                    hf.LogFile("KontaktErstellen: Kontakt nicht gefunden")
                                End Try
                            Next
                            hf.NAR(olLink) : olLink = Nothing
                        End If
#End If
                        Kontakt = CType(ThisAddIn.oApp.CreateItem(Outlook.OlItemType.olContactItem), Outlook.ContactItem)
                        If Not pos1 = 0 And Not pos2 = 0 Then
                            vCard = Mid(.Body, pos1, pos2 - pos1)
                            vCard2Contact(vCard, Kontakt)
                        Else
                            If hf.Mobilnummer(hf.nurZiffern(TelNr, "0049")) Then
                                Kontakt.MobileTelephoneNumber = TelNr
                            Else
                                Kontakt.BusinessTelephoneNumber = TelNr
                            End If

                        End If
                        ini.Write(DateiPfad, "Journal", "JournalID", .EntryID)
                        ini.Write(DateiPfad, "Journal", "JournalStoreID", CType(.Parent, Outlook.MAPIFolder).StoreID)
                        With Kontakt
                            If Not hf.nurZiffern(.BusinessTelephoneNumber, "0049") = hf.nurZiffern(TelNr, "0049") And Not .BusinessTelephoneNumber = "" Then
                                .Business2TelephoneNumber = hf.formatTelNr(TelNr)
                            ElseIf Not hf.nurZiffern(.HomeTelephoneNumber, "0049") = hf.nurZiffern(TelNr, "0049") And Not .HomeTelephoneNumber = "" Then
                                .Home2TelephoneNumber = hf.formatTelNr(TelNr)
                            End If
                            .Categories = "Fritz!Box" 'Alle Kontakte, die erstellt werdn, haben die Kategorie "Fritz!Box". Damit sind sie einfach zu erkennen
                            hf.LogFile("Kontakt " & Kontakt.FullName & " wurde aus einem Journaleintrag erzeugt.")
                            .Display()
                        End With
                    End If

                End With
            End If
        End If
    End Sub ' (KontaktErstellen)

    Private Function NrFormat(ByVal gefKontakt As Outlook.ContactItem, ByVal TelNr As String, ByVal LandesVW As String) As String
        Dim alleTE(16) As String
        With gefKontakt
            alleTE(0) = .AssistantTelephoneNumber
            alleTE(1) = .BusinessTelephoneNumber
            alleTE(2) = .Business2TelephoneNumber
            alleTE(3) = .CallbackTelephoneNumber
            alleTE(4) = .CarTelephoneNumber
            alleTE(5) = .CompanyMainTelephoneNumber
            alleTE(6) = .HomeTelephoneNumber
            alleTE(7) = .Home2TelephoneNumber
            alleTE(8) = .ISDNNumber
            alleTE(9) = .MobileTelephoneNumber
            alleTE(10) = .OtherTelephoneNumber
            alleTE(11) = .PagerNumber
            alleTE(12) = .PrimaryTelephoneNumber
            alleTE(13) = .RadioTelephoneNumber
            alleTE(14) = .BusinessFaxNumber
            alleTE(15) = .HomeFaxNumber
            alleTE(16) = .OtherFaxNumber
        End With
        For Each Telefonnummer In alleTE
            If TelNr = hf.nurZiffern(Telefonnummer, LandesVW) Then Return Telefonnummer
        Next
        Return TelNr
    End Function

    Friend Sub IndiziereKontakt(ByRef Kontakt As Outlook.ContactItem, WriteLog As Boolean)
        If Not CBool(ini.Read(DateiPfad, "Optionen", "CBIndexAus", "False")) Then
            Dim LandesVW As String = ini.Read(DateiPfad, "Optionen", "TBLandesVW", "0049")
            Dim alleTE(16) As String  ' alle TelNr/Email eines Kontakts
            Dim speichern As Boolean = False
            Dim tempTelNr As String

            With Kontakt
                alleTE(0) = .AssistantTelephoneNumber
                alleTE(1) = .BusinessTelephoneNumber
                alleTE(2) = .Business2TelephoneNumber
                alleTE(3) = .CallbackTelephoneNumber
                alleTE(4) = .CarTelephoneNumber
                alleTE(5) = .CompanyMainTelephoneNumber
                alleTE(6) = .HomeTelephoneNumber
                alleTE(7) = .Home2TelephoneNumber
                alleTE(8) = .ISDNNumber
                alleTE(9) = .MobileTelephoneNumber
                alleTE(10) = .OtherTelephoneNumber
                alleTE(11) = .PagerNumber
                alleTE(12) = .PrimaryTelephoneNumber
                alleTE(13) = .RadioTelephoneNumber
                alleTE(14) = .BusinessFaxNumber
                alleTE(15) = .HomeFaxNumber
                alleTE(16) = .OtherFaxNumber

                For i = LBound(alleTE) To UBound(alleTE)
                    If Not alleTE(i) = vbNullString Then ' Fall: Telefonnummer vorhanden
                        If .UserProperties.Find(UserProperties(i)) Is Nothing Then
                            .UserProperties.Add(UserProperties(i), Outlook.OlUserPropertyType.olText, False)
                        End If
                        tempTelNr = hf.nurZiffern(alleTE(i), LandesVW)
                        If Not CStr(.UserProperties.Find(UserProperties(i)).Value) = tempTelNr Then
                            .UserProperties.Find(UserProperties(i)).Value = tempTelNr
                        End If
                    ElseIf Not .UserProperties.Find(UserProperties(i)) Is Nothing Then ' Fall:Index vorhanden, Telefonnummer nicht
                        .UserProperties.Find(UserProperties(i)).Delete()
                    End If
                Next
                If WriteLog Then hf.LogFile("Kontakt: " & .FullNameAndCompany & " wurde automatisch indiziert.")
                .Save()
            End With
        End If
    End Sub

    Friend Sub DeIndizierungKontakt(ByRef Kontakt As Outlook.ContactItem, WriteLog As Boolean)
        Dim UserEigenschaft As Outlook.UserProperty
        If Not CBool(ini.Read(DateiPfad, "Optionen", "CBIndexAus", "False")) Then
            With Kontakt.UserProperties
                For Each UserProperty In UserProperties
                    Try
                        UserEigenschaft = .Find(UserProperty)
                    Catch
                        UserEigenschaft = Nothing
                    End Try
                    If Not UserEigenschaft Is Nothing Then UserEigenschaft.Delete()
                    UserEigenschaft = Nothing
                Next
            End With
            Kontakt.Save()
        End If
    End Sub

    Friend Sub DeIndizierungOrdner(ByVal Ordner As Outlook.MAPIFolder)
        Try
#If Not OVer = 11 Then
            With Ordner.UserDefinedProperties
                For i = 1 To .Count
                    If hf.IsOneOf(.Item(1).Name, UserProperties) Then .Remove(1)
                Next
            End With
#End If
        Catch : End Try
    End Sub

    Sub vCard2Contact(ByVal vCard As String, ByRef Contact As Outlook.ContactItem)
        ' überträgt den Inhalt einer vCard in einen Kontakt
        ' Parameter:  vCard (String):         Quelle (zu übertragende vCard)
        '             Contact (ContactItem):  Ziel (Kontakt, in den die Daten eingetragen werden sollen)

        Dim ContactName As String  ' kompletter Name ("N") aus vCard
        'Dim NameParts As Object ' Bestandteile von ContactName
        Dim pos As Integer    ' Position innerhalb eines Strings
        Dim tmp1 As String, tmp2 As String, tmp3 As String  ' Hilfsstrings
        Dim Company As String  ' Firmenname
        Dim BFax As String, BTel As String  ' dienstl. Tel. und Fax
        Dim HFax As String, HTel As String  ' privates Tel. und Fax
        Dim Mobile As String, Car As String  ' Mobil- und Autotelefon
        Dim Pager As String, ISDN As String  ' sonstige Tel.-Nr.
        Dim Email1 As String, Email2 As String, Email3 As String  ' Emailadressen

        With Contact
            'insert Name
            ContactName = ReadFromVCard(vCard, "N", "")
            If Not ContactName = "" Then
                pos = InStr(1, ContactName, "#", CompareMethod.Text)
                If Not pos = 0 Then ContactName = Left(ContactName, pos - 1)
                pos = InStr(1, ContactName, ";", CompareMethod.Text)
                If pos = 0 Then
                    If .LastName = "" Then .LastName = ContactName
                Else
                    If .LastName = "" Then .LastName = Left(ContactName, pos - 1)
                    ContactName = Mid(ContactName, pos + 1)
                    pos = InStr(1, ContactName, ";", CompareMethod.Text)
                    If pos = 0 Then
                        If .FirstName = "" Then .FirstName = ContactName
                    Else
                        If .FirstName = "" Then .FirstName = Left(ContactName, pos - 1)
                        ContactName = Mid(ContactName, pos + 1)
                        pos = InStr(1, ContactName, ";", CompareMethod.Text)
                        If pos = 0 Then
                            If .MiddleName = "" Then .MiddleName = ContactName
                        Else
                            If .MiddleName = "" Then .MiddleName = Left(ContactName, pos - 1)
                            ContactName = Mid(ContactName, pos + 1)
                            pos = InStr(1, ContactName, ";", CompareMethod.Text)
                            If pos = 0 Then
                                If .Title = "" Then .Title = ContactName
                            Else
                                If .Title = "" Then .Title = Left(ContactName, pos - 1)
                                ContactName = Mid(ContactName, pos + 1)
                                pos = InStr(1, ContactName, ";", CompareMethod.Text)
                                If pos = 0 Then
                                    If .Suffix = "" Then .Suffix = ContactName
                                Else
                                    If .Suffix = "" Then .Suffix = Left(ContactName, pos - 1)
                                End If
                            End If
                            ' Eingefügt am 9.4.10: Grund 11880 liefert Firmenname mit dem Wort "Firma   " - unschön: entfernt
                            If .Title = "Firma" Then .Title = Nothing
                        End If
                    End If
                End If
            Else
                If .FullName = "" Then
                    tmp1 = ReadFromVCard(vCard, "FN", "")
                    pos = InStr(1, tmp1, "#", CompareMethod.Text)
                    ' Eingefügt am 9.4.10: Grund 11880 liefert Firmenname mit dem Wort "Firma   " - unschön: entfernt
                    If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
                    If InStr(1, tmp1, "Firma", CompareMethod.Text) = 1 Then
                        tmp1 = Right(tmp1, Len(tmp1) - 5)
                    End If
                    tmp1 = Trim(tmp1)
                    ' Ende 9.4.10
                    .FullName = tmp1
                End If
            End If
            If .NickName = "" Then
                tmp1 = ReadFromVCard(vCard, "NICKNAME", "")
                pos = InStr(1, tmp1, "#", CompareMethod.Text)
                If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
                .NickName = tmp1
            End If
            'insert Jobtitle and Companny
            If .JobTitle = "" Then
                tmp1 = ReadFromVCard(vCard, "TITLE", "")
                pos = InStr(1, tmp1, "#", CompareMethod.Text)
                If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
                .JobTitle = tmp1
            End If
            Company = ReadFromVCard(vCard, "ORG", "")
            If .CompanyName = "" Then
                pos = InStr(1, Company, "#", CompareMethod.Text)
                If Not pos = 0 Then Company = Left(Company, pos - 1)
                .CompanyName = Company
            End If
            'insert Telephone Numbers
            BFax = ReadFromVCard(vCard, "TEL", "WORK,FAX")
            If BFax = "" Then
                BTel = ReadFromVCard(vCard, "TEL", "WORK")
            Else
                If .BusinessFaxNumber = "" Then
                    pos = InStr(1, BFax, "#", CompareMethod.Text)
                    If Not pos = 0 Then BFax = Left(BFax, pos - 1)
                    .BusinessFaxNumber = hf.formatTelNr(BFax)
                End If
                BTel = ReadFromVCard(vCard, "TEL", "WORK,VOICE")
            End If
            If .BusinessTelephoneNumber = "" Then
                pos = InStr(1, BTel, "#", CompareMethod.Text)
                If Not pos = 0 Then BTel = Left(BTel, pos - 1)
                .BusinessTelephoneNumber = hf.formatTelNr(BTel)
            End If
            HFax = ReadFromVCard(vCard, "TEL", "HOME,FAX")
            If HFax = "" Then
                HTel = ReadFromVCard(vCard, "TEL", "HOME")
            Else
                If .HomeFaxNumber = "" Then
                    pos = InStr(1, HFax, "#", CompareMethod.Text)
                    If Not pos = 0 Then HFax = Left(HFax, pos - 1)
                    .HomeFaxNumber = hf.formatTelNr(HFax)
                End If
                HTel = ReadFromVCard(vCard, "TEL", "HOME,VOICE")
            End If
            If .HomeTelephoneNumber = "" Then
                pos = InStr(1, HTel, "#", CompareMethod.Text)
                If Not pos = 0 Then HTel = Left(HTel, pos - 1)
                .HomeTelephoneNumber = hf.formatTelNr(HTel)
            End If
            Mobile = ReadFromVCard(vCard, "TEL", "CELL")
            If .MobileTelephoneNumber = "" Then
                pos = InStr(1, Mobile, "#", CompareMethod.Text)
                If Not pos = 0 Then Mobile = Left(Mobile, pos - 1)
                .MobileTelephoneNumber = hf.formatTelNr(Mobile)
            End If
            Pager = ReadFromVCard(vCard, "TEL", "PAGER")
            If .PagerNumber = "" Then
                pos = InStr(1, Pager, "#", CompareMethod.Text)
                If Not pos = 0 Then Pager = Left(Pager, pos - 1)
                .PagerNumber = hf.formatTelNr(Pager)
            End If
            Car = ReadFromVCard(vCard, "TEL", "CAR")
            If .CarTelephoneNumber = "" Then
                pos = InStr(1, Car, "#", CompareMethod.Text)
                If Not pos = 0 Then Car = Left(Car, pos - 1)
                .CarTelephoneNumber = hf.formatTelNr(Car)
            End If
            ISDN = ReadFromVCard(vCard, "TEL", "ISDN")
            If .ISDNNumber = "" Then
                pos = InStr(1, ISDN, "#", CompareMethod.Text)
                If Not pos = 0 Then ISDN = Left(ISDN, pos - 1)
                .ISDNNumber = hf.formatTelNr(ISDN)
            End If
            If BFax = "" And BTel = "" And HFax = "" And HTel = "" And Mobile = "" And Pager = "" And Car = "" And ISDN = "" Then
                tmp1 = ReadFromVCard(vCard, "TEL", "")
                pos = InStr(1, tmp1, "#", CompareMethod.Text)
                If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
                If Company = "" Then
                    If .HomeTelephoneNumber = "" Then .HomeTelephoneNumber = hf.formatTelNr(tmp1)
                Else
                    If .BusinessTelephoneNumber = "" Then .BusinessTelephoneNumber = hf.formatTelNr(tmp1)
                End If
            End If
            'insert Birthday
            tmp1 = (ReadFromVCard(vCard, "BDAY", ""))
            pos = InStr(1, tmp1, "#", CompareMethod.Text)
            If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
            If Len(tmp1) = 8 Then tmp1 = Left(tmp1, 4) & "-" & Mid(tmp1, 5, 2) & "-" & Mid(tmp1, 7)
            If Not tmp1 = "" And CStr(.Birthday) = "01.01.4501" Then .Birthday = CDate(tmp1)
            'insert addresses
            tmp1 = ReadFromVCard(vCard, "ADR", "HOME,POSTAL")
            If tmp1 = "" Then tmp1 = ReadFromVCard(vCard, "ADR", "HOME,PARCEL")
            If tmp1 = "" Then tmp1 = ReadFromVCard(vCard, "ADR", "HOME")
            tmp2 = ReadFromVCard(vCard, "ADR", "WORK,POSTAL")
            If tmp2 = "" Then tmp2 = ReadFromVCard(vCard, "ADR", "WORK,PARCEL")
            If tmp2 = "" Then tmp2 = ReadFromVCard(vCard, "ADR", "WORK")
            If tmp1 = "" And tmp2 = "" Then
                If Company = "" Then
                    tmp1 = ReadFromVCard(vCard, "ADR", "")
                Else
                    tmp2 = ReadFromVCard(vCard, "ADR", "")
                End If
            End If
            pos = InStr(1, tmp1, "#", CompareMethod.Text)
            If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
            pos = InStr(1, tmp2, "#", CompareMethod.Text)
            If Not pos = 0 Then tmp2 = Left(tmp2, pos - 1)
            pos = InStr(1, tmp1, ";", CompareMethod.Text)
            If pos = 0 Then
                If .HomeAddressPostOfficeBox = "" Then .HomeAddressPostOfficeBox = tmp1
            Else
                tmp3 = Left(tmp1, pos - 1)
                tmp1 = Mid(tmp1, pos + 1)
                pos = InStr(1, tmp1, ";", CompareMethod.Text)
                If pos = 0 Then
                    If .HomeAddressPostOfficeBox = "" Then .HomeAddressPostOfficeBox = Trim(tmp3 & " " & tmp1)
                Else
                    If .HomeAddressPostOfficeBox = "" Then .HomeAddressPostOfficeBox = Trim(tmp3 & " " & Left(tmp1, pos - 1))
                    tmp1 = Mid(tmp1, pos + 1)
                    pos = InStr(1, tmp1, ";", CompareMethod.Text)
                    If pos = 0 Then
                        If .HomeAddressStreet = "" Then .HomeAddressStreet = tmp1
                    Else
                        If .HomeAddressStreet = "" Then .HomeAddressStreet = Left(tmp1, pos - 1)
                        tmp1 = Mid(tmp1, pos + 1)
                        pos = InStr(1, tmp1, ";", CompareMethod.Text)
                        If pos = 0 Then
                            If .HomeAddressCity = "" Then .HomeAddressCity = tmp1
                        Else
                            If .HomeAddressCity = "" Then .HomeAddressCity = Left(tmp1, pos - 1)
                            tmp1 = Mid(tmp1, pos + 1)
                            pos = InStr(1, tmp1, ";", CompareMethod.Text)
                            If pos = 0 Then
                                If .HomeAddressState = "" Then .HomeAddressState = tmp1
                            Else
                                If .HomeAddressState = "" Then .HomeAddressState = Left(tmp1, pos - 1)
                                tmp1 = Mid(tmp1, pos + 1)
                                pos = InStr(1, tmp1, ";", CompareMethod.Text)
                                If pos = 0 Then
                                    If .HomeAddressPostalCode = "" Then .HomeAddressPostalCode = tmp1
                                Else
                                    If .HomeAddressPostalCode = "" Then .HomeAddressPostalCode = Left(tmp1, pos - 1)
                                    tmp1 = Mid(tmp1, pos + 1)
                                    pos = InStr(1, tmp1, ";", CompareMethod.Text)
                                    If pos = 0 Then
                                        If .HomeAddressCountry = "" Then .HomeAddressCountry = tmp1
                                    Else
                                        If .HomeAddressCountry = "" Then .HomeAddressCountry = Left(tmp1, pos - 1)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
            pos = InStr(1, tmp2, ";", CompareMethod.Text)
            If pos = 0 Then
                If .BusinessAddressPostOfficeBox = "" Then .BusinessAddressPostOfficeBox = tmp2
            Else
                tmp3 = Left(tmp2, pos - 1)
                tmp2 = Mid(tmp2, pos + 1)
                pos = InStr(1, tmp2, ";", CompareMethod.Text)
                If pos = 0 Then
                    If .BusinessAddressPostOfficeBox = "" Then .BusinessAddressPostOfficeBox = Trim(tmp3 & " " & tmp2)
                Else
                    If .BusinessAddressPostOfficeBox = "" Then .BusinessAddressPostOfficeBox = Trim(tmp3 & " " & Left(tmp2, pos - 1))
                    tmp2 = Mid(tmp2, pos + 1)
                    pos = InStr(1, tmp2, ";", CompareMethod.Text)
                    If pos = 0 Then
                        If .BusinessAddressStreet = "" Then .BusinessAddressStreet = tmp2
                    Else
                        If .BusinessAddressStreet = "" Then .BusinessAddressStreet = Left(tmp2, pos - 1)
                        tmp2 = Mid(tmp2, pos + 1)
                        pos = InStr(1, tmp2, ";", CompareMethod.Text)
                        If pos = 0 Then
                            If .BusinessAddressCity = "" Then .BusinessAddressCity = tmp2
                        Else
                            If .BusinessAddressCity = "" Then .BusinessAddressCity = Left(tmp2, pos - 1)
                            tmp2 = Mid(tmp2, pos + 1)
                            pos = InStr(1, tmp2, ";", CompareMethod.Text)
                            If pos = 0 Then
                                If .BusinessAddressState = "" Then .BusinessAddressState = tmp2
                            Else
                                If .BusinessAddressState = "" Then .BusinessAddressState = Left(tmp2, pos - 1)
                                tmp2 = Mid(tmp2, pos + 1)
                                pos = InStr(1, tmp2, ";", CompareMethod.Text)
                                If pos = 0 Then
                                    If .BusinessAddressPostalCode = "" Then .BusinessAddressPostalCode = tmp2
                                Else
                                    If .BusinessAddressPostalCode = "" Then .BusinessAddressPostalCode = Left(tmp2, pos - 1)
                                    tmp2 = Mid(tmp2, pos + 1)
                                    pos = InStr(1, tmp2, ";", CompareMethod.Text)
                                    If pos = 0 Then
                                        If .BusinessAddressCountry = "" Then .BusinessAddressCountry = tmp2
                                    Else
                                        If .BusinessAddressCountry = "" Then .BusinessAddressCountry = Left(tmp2, pos - 1)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
            'insert email-addresses
            tmp1 = ReadFromVCard(vCard, "EMAIL", "PREF")
            pos = InStr(1, tmp1, "#", CompareMethod.Text)
            If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
            tmp2 = ReadFromVCard(vCard, "EMAIL", "")
            pos = InStr(1, tmp2, "#", CompareMethod.Text)
            If Not pos = 0 Then tmp2 = Left(tmp2, pos - 1)
            If Not tmp2 = "" Then
                pos = InStr(1, tmp2, tmp1, CompareMethod.Text)
                If Not tmp1 = "" And Not pos = 0 Then
                    tmp2 = tmp1 & ";" & Replace(Left(tmp2, pos - 1) & Mid(tmp2, pos + Len(tmp1)), ";;", ";", , , CompareMethod.Text)
                End If
                Email1 = ""
                Email2 = ""
                Email3 = ""
                pos = InStr(1, tmp2, ";", CompareMethod.Text)
                If pos = 0 Then
                    Email1 = tmp2
                Else
                    Email1 = Left(tmp2, pos - 1)
                    tmp2 = Mid(tmp2, pos + 1)
                    pos = InStr(1, tmp2, ";", CompareMethod.Text)
                    If pos = 0 Then
                        Email2 = tmp2
                    Else
                        Email2 = Left(tmp2, pos - 1)
                        tmp2 = Mid(tmp2, pos + 1)
                        pos = InStr(1, tmp2, ";", CompareMethod.Text)
                        If pos = 0 Then
                            Email3 = tmp2
                        Else
                            Email3 = Left(tmp2, pos - 1)
                        End If
                    End If
                End If
                Try ' Fehler abfangen
                    If .Email1Address = "" Then
                        .Email1Address = Email1
                    ElseIf Not .Email1Address = Email1 Then
                        If Not .Email1Address = Email2 Then Email3 = Email2
                        Email2 = Email1
                    End If
                    If .Email2Address = "" Then
                        .Email2Address = Email2
                    ElseIf Not .Email2Address = Email2 Then
                        Email3 = Email2
                    End If
                    If .Email3Address = "" Then .Email3Address = Email3
                Catch
                    'LogFile("vCard2Contact: " & Err.Number)
                    If Err.Number = 287 Then
                        'LogFile("Fehler-Beschreibung: " & Err.Description & vbNewLine & "Nutzer hat den Zugriff auf den Kontakt nicht gewährt")
                        hf.FBDB_MsgBox("Achtung: Sie haben einen Zugriff auf den Kontakt nicht zugelassen. Email-Addressen oder Notizen konnten nicht in den Kontakt eingetragen werden.", MsgBoxStyle.Exclamation, "vCard2Contact")
                    Else
                        hf.FBDB_MsgBox("Es is ein Fehler aufgetreten: " & Err.Description, MsgBoxStyle.Exclamation, "vCard2Contact")
                    End If
                End Try
            End If
            'insert urls
            If .WebPage = "" Then .WebPage = Replace(ReadFromVCard(vCard, "URL", ""), ";", " ", , , CompareMethod.Text)
            'insert note
            tmp1 = ReadFromVCard(vCard, "NOTE", "")
            If Not tmp1 = "" Then
                Try ' Fehler abfangen
                    .Body = tmp1 & vbNewLine & vbNewLine & .Body
                Catch
                    'LogFile("vCard2Contact: " & Err.Number)
                    If Err.Number = 287 Then
                        'LogFile("Fehler-Beschreibung: " & Err.Description & vbNewLine & "Nutzer hat den Zugriff auf den Kontakt nicht gewährt")
                        hf.FBDB_MsgBox("Achtung: Sie haben einen Zugriff auf den Kontakt nicht zugelassen. Email-Addressen oder Notizen konnten nicht in den Kontakt eingetragen werden.", MsgBoxStyle.Exclamation, "vCard2Contact")
                    Else
                        hf.FBDB_MsgBox("Es is ein Fehler aufgetreten: " & Err.Description, MsgBoxStyle.Exclamation, "vCard2Contact")
                    End If
                End Try
            End If
        End With

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
