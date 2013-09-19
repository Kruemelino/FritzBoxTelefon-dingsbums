Imports System.Drawing
Public Class Wählclient
    Private C_XML As MyXML
    Private frmWählbox As formWählbox
    Private hf As Helfer
    Private KontaktFunktionen As Contacts
    Private GUI As GraphicalUserInterface
    Private OlI As OutlookInterface
    Private FBox As FritzBox

    Private PhonerFunktionen As PhonerInterface

    Public Sub New(ByVal XMlKlasse As MyXML, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal KontaktKlasse As Contacts, _
                   ByVal InterfaceKlasse As GraphicalUserInterface, _
                   ByVal OutlInter As OutlookInterface, _
                   ByVal cFBox As FritzBox, _
                   ByVal PhonerKlasse As PhonerInterface)
        hf = HelferKlasse
        KontaktFunktionen = KontaktKlasse
        GUI = InterfaceKlasse
        C_XML = XMlKlasse
        OlI = OutlInter
        FBox = cFBox
        PhonerFunktionen = PhonerKlasse

    End Sub
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
#Region "Alles was mit dem Wählen zu tun hat"
    Friend Sub WählboxStart(ByVal olAuswahl As Outlook.Selection)
        ' wird durch das Symbol 'Wählen' in der 'FritzBox'-Symbolleiste ausgeführt
        Dim olNamespace As Outlook.NameSpace
        Dim olfolder As Outlook.MAPIFolder
        Dim aktKontakt As Outlook.ContactItem       ' aktuell ausgewählter Kontakt
        Dim i As Long              ' Zählvariable
        Dim pos1 As Integer
        Dim pos2 As Integer
        Dim vCard As String
        Dim name As String
        Dim res As Outlook.ContactItem

        Try
            olNamespace = ThisAddIn.oApp.GetNamespace("MAPI")
            ' Ist überhaupt etwas ausgewählt?
            If (olAuswahl.Count = 1) Then
                If TypeOf olAuswahl.Item(1) Is Outlook.MailItem Then
                    ' Es wurde eine Mail ausgewählt
                    Dim aktMail As Outlook.MailItem = CType(olAuswahl.Item(1), Outlook.MailItem)
                    Dim Absender As String

                    Absender = aktMail.SenderEmailAddress
                    If C_XML.Read("Optionen", "CBKHO", "True") = "TRUE" Then
                        olfolder = olNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)
                        res = KontaktFunktionen.FindeKontakt("", Absender, "", olfolder, Nothing)
                    Else
                        res = KontaktFunktionen.FindeKontakt("", Absender, "", Nothing, olNamespace)
                    End If
                    ' Nun den zur Email-Adresse gehörigen Kontakt suchen
                    i = 0
                    If Not Absender = "" Then
                        If Not res Is Nothing Then
                            Wählbox(res, "", False)
                        Else
                            hf.FBDB_MsgBox("Es ist kein Kontakt mit der E-Mail-Adresse " & Absender & " vorhanden!", MsgBoxStyle.Exclamation, "WählboxStart")
                        End If
                    End If
                    With hf
                        .NAR(olNamespace)
                        .NAR(aktMail)
                    End With
                    olNamespace = Nothing
                    aktMail = Nothing
                ElseIf TypeOf olAuswahl.Item(1) Is Outlook.ContactItem Then
                    ' Es wurde gleich ein Kontakt gewählt!
                    ' Nun direkt den Wähldialog für den Kontakt anzeigen.
                    aktKontakt = CType(olAuswahl.Item(1), Outlook.ContactItem)
                    Wählbox(aktKontakt, String.Empty, False)
                    hf.NAR(aktKontakt)
                    aktKontakt = Nothing
                ElseIf TypeOf olAuswahl.Item(1) Is Outlook.AppointmentItem Then
#If Not OVer = 15 Then
                    Dim oAppItem As Outlook.AppointmentItem = CType(olAuswahl.Item(1), Outlook.AppointmentItem)
                    Dim oAppLink As Outlook.Link
                    Dim oAppThing As Object
                    For Each oAppLink In oAppItem.Links
                        oAppThing = oAppLink.Item
                        If TypeOf oAppThing Is Outlook.ContactItem Then 'Nur, wenn der Link auf einen Kontakt zeigt....
                            Wählbox(CType(oAppThing, Outlook.ContactItem), String.Empty, False)
                            Exit For
                        End If
                    Next 'oAppLink
                    hf.NAR(oAppItem)
                    oAppItem = Nothing
#End If
                ElseIf TypeOf olAuswahl.Item(1) Is Outlook.JournalItem Then
                    ' Es wurde ein Journaleintrag gewählt!
                    Dim aktJournal As Outlook.JournalItem = CType(olAuswahl.Item(1), Outlook.JournalItem)
                    If InStr(aktJournal.Body, "unbekannt") = 0 _
                        And Not InStr(aktJournal.Categories, "FritzBox Anrufmonitor") = 0 Then
#If Not OVer = 15 Then
                        If Not aktJournal.Links.Count = 0 Then
                            Dim oAppLink As Outlook.Link
                            Dim oAppThing As Object
                            For Each oAppLink In aktJournal.Links
                                oAppThing = oAppLink.Item
                                If TypeOf oAppThing Is Outlook.ContactItem Then
                                    Wählbox(CType(oAppThing, Outlook.ContactItem), Mid(aktJournal.Body, 11, InStr(1, aktJournal.Body, vbNewLine) - 11), False)
                                    Exit For
                                End If
                                hf.NAR(oAppThing)
                            Next 'oAppLink
                        Else
#End If
                            pos1 = InStr(1, aktJournal.Body, "BEGIN:VCARD", CompareMethod.Text)
                            pos2 = InStr(1, aktJournal.Body, "END:VCARD", CompareMethod.Text)
                            Dim vName As String
                            If Not pos1 = 0 And Not pos2 = 0 Then
                                pos2 = pos2 + 9
                                vCard = Mid(aktJournal.Body, pos1, pos2 - pos1)
                                name = Replace(ReadFromVCard(vCard, "N", ""), ";", "", , , CompareMethod.Text)
                                vName = "-1" & name & ";" & vCard
                            Else
                                vName = String.Empty
                            End If

                            Wählbox(Nothing, Mid(aktJournal.Body, 11, InStr(1, aktJournal.Body, vbNewLine) - 11), False, vName)
#If Not OVer = 15 Then
                        End If
#End If

                    End If
                Else
                    hf.FBDB_MsgBox("Es muss entweder ein Kontakt, eine E-Mail-Adresse oder ein Journal ausgewählt sein!", MsgBoxStyle.Exclamation, "WählboxStart")
                End If
            Else
                hf.FBDB_MsgBox("Es muss genau ein Element ausgewählt sein!", MsgBoxStyle.Exclamation, "WählboxStart")
            End If
            olAuswahl = Nothing
            Exit Sub
        Catch
            hf.FBDB_MsgBox("Es muss entweder ein Kontakt, eine Email oder ein Journal ausgewählt sein!", MsgBoxStyle.Exclamation, "WählboxStart")
        End Try
    End Sub ' (WählboxStart)

    Sub Wählbox(ByVal oContact As Outlook.ContactItem, ByVal TelNr As String, ByVal Direktwahl As Boolean, Optional ByVal vName As String = "")
        ' macht alle Eintragungen in 'formWählbox'
        ' aus FritzBoxDial übernommen und überarbeitet
        ' Parameter:  oContact (ContactItem): Kontaktdaten des Anzurufenden
        '             TelNr (String):         Telefonnummer des Anzurufenden
        Dim alleTelNr(14) As String ' alle im Kontakt enthaltenen Telefonnummern
        Dim alleNrTypen(14) As String ' die Bezeichnungen der Telefonnummern
        Dim i, iTelNr As Integer    ' Zählvariablen
        Dim ImgPath As String = vbNullString   ' Position innerhalb eines Strings
        Dim LandesVW As String  ' eigene Landesvorwahl
        Dim row(2) As String

        frmWählbox = New formWählbox(Direktwahl, C_XML, hf, GUI, FBox, PhonerFunktionen)

        LandesVW = C_XML.Read("Optionen", "TBLandesVW", "0049")
        If oContact Is Nothing Then
            frmWählbox.Tag = "-1"
        Else
            frmWählbox.Tag = oContact.EntryID & ";" & CType(oContact.Parent, Outlook.MAPIFolder).StoreID
        End If

        iTelNr = 1 'Index Zeile im DataGrid des Formulars
        ' Ist der Kontakt nicht vorhanden (z.B. Rückruf)?
        If oContact Is Nothing Then
            If Not Direktwahl Then
                ' Ortsvorwahl vor die Nummer setzen, falls eine Rufnummer nicht mit "0" beginnt und nicht mit "11"
                ' (Rufnummern die mit "11" beginnen sind Notrufnummern oder andere Sondernummern)
                If Not Left(hf.nurZiffern(TelNr, LandesVW), 1) = "0" And Not Left(hf.nurZiffern(TelNr, LandesVW), 2) = "11" Then _
                    TelNr = C_XML.Read("Optionen", "TBVorwahl", "") & TelNr

                If vName = String.Empty Then
                    frmWählbox.Text = "Anruf: " & TelNr
                Else
                    Dim st As Integer = InStr(vName, ";", CompareMethod.Text)
                    If st = 0 Then
                        frmWählbox.Text = "Anruf: " & vName
                    Else
                        frmWählbox.Text = "Anruf: " & Mid(vName, 3, st - 3)
                    End If
                End If
                ' Liste füllen
                row(0) = CStr(iTelNr) 'Index Zeile im DataGrid
                row(2) = TelNr
                frmWählbox.ListTel.Rows.Add(row)
            Else
                frmWählbox.Text = "Anruf: Direktwahl"
                frmWählbox.ContactImage.Visible = False
            End If
        Else
            ' Welche Telefonnummerntypen sollen angezeigt werden?
            With oContact
                ' Fenstertitel setzen
                frmWählbox.Text = Replace("Anruf: " & .FullName & " (" & .CompanyName & ")", " ()", "", , , CompareMethod.Text)
                ' Die einzelnen Telefonnummern in ein Array packen
                ' Die deutsche Bezeichnung der Nummerntypen
                ' für Anzeigezwecke auch in ein Array packen.
                alleTelNr(1) = .AssistantTelephoneNumber : alleNrTypen(1) = "Assistent"
                alleTelNr(2) = .BusinessTelephoneNumber : alleNrTypen(2) = "Geschäftlich"
                alleTelNr(3) = .Business2TelephoneNumber : alleNrTypen(3) = "Geschäftlich2"
                alleTelNr(4) = .CallbackTelephoneNumber : alleNrTypen(4) = "Rückmeldung"
                alleTelNr(5) = .CarTelephoneNumber : alleNrTypen(5) = "Auto"
                alleTelNr(6) = .CompanyMainTelephoneNumber : alleNrTypen(6) = "Firma"
                alleTelNr(7) = .HomeTelephoneNumber : alleNrTypen(7) = "Privat"
                alleTelNr(8) = .Home2TelephoneNumber : alleNrTypen(8) = "Privat2"
                alleTelNr(9) = .ISDNNumber : alleNrTypen(9) = "ISDN"
                alleTelNr(10) = .MobileTelephoneNumber : alleNrTypen(10) = "Mobiltelefon"
                alleTelNr(11) = .OtherTelephoneNumber : alleNrTypen(11) = "Weitere"
                alleTelNr(12) = .PagerNumber : alleNrTypen(12) = "Pager"
                alleTelNr(13) = .PrimaryTelephoneNumber : alleNrTypen(13) = "Haupttelefon"
                alleTelNr(14) = .RadioTelephoneNumber : alleNrTypen(14) = "Funkruf"
                ImgPath = OlI.KontaktBild(.EntryID, CType(.Parent, Outlook.MAPIFolder).StoreID)
                If Not ImgPath = vbNullString Then
                    Dim orgIm As Image = Image.FromFile(ImgPath)
                    With frmWählbox.ContactImage
                        Dim Bildgröße As New Size(.Width, CInt((.Width * orgIm.Size.Height) / orgIm.Size.Width))
                        Dim showim As Image = New Bitmap(Bildgröße.Width, Bildgröße.Height)
                        Dim g As Graphics = Graphics.FromImage(showim)
                        g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                        g.DrawImage(orgIm, 0, 0, Bildgröße.Width, Bildgröße.Height)
                        g.Dispose()
                        .Image = showim
                    End With
                Else
                    frmWählbox.ContactImage.Visible = False
                End If
            End With
            ' Liste füllen
            For i = LBound(alleTelNr) + 1 To UBound(alleTelNr)
                If Not alleTelNr(i) = "" Then
                    ' Wenn die Telefonnummer nicht leer ist, dann in die Liste hinzufügen
                    row(0) = CStr(iTelNr) 'Index wird eins hochgezählt
                    'Ortsvorwahl vor die Nummer setzen, falls eine Rufnummer nicht mit "0" beginnt und nicht mit "11"
                    '(Rufnummern die mit "11" beginnen sind Notrufnummern oder andere Sondernummern)
                    If Not Left(hf.nurZiffern(alleTelNr(i), LandesVW), 1) = "0" And Not Left(hf.nurZiffern(alleTelNr(i), LandesVW), 2) = "11" Then _
                        alleTelNr(i) = C_XML.Read("Optionen", "TBVorwahl", "") & alleTelNr(i)
                    If hf.nurZiffern(alleTelNr(i), LandesVW) = hf.nurZiffern(TelNr, LandesVW) Then
                        row(1) = alleNrTypen(i) & " *"
                    Else
                        row(1) = alleNrTypen(i)
                    End If
                    row(2) = alleTelNr(i)
                    iTelNr += 1
                    frmWählbox.ListTel.Rows.Add(row)
                End If
            Next
        End If
        With frmWählbox
            If Not .ListTel.RowCount = 0 Or Direktwahl Then
                .Show()
                .BringToFront()
                If Direktwahl Then
                    .TelNrBox.Focus()
                    .AcceptButton = .ButtonWeiter
                Else
                    .Focus()
                End If
            Else
                hf.FBDB_MsgBox("Der Kontakt hat keine Telefonnummern", MsgBoxStyle.Exclamation, "Wählbox")
            End If
        End With
        ' Wähldialog anzeigen

    End Sub '(Wählbox)

    Sub OnActionAnrListen(ByVal index As String)
        Dim oNS As Outlook.NameSpace = ThisAddIn.oApp.GetNamespace("MAPI")
        Dim oContact As Outlook.ContactItem
        Dim Eintrag As String()
        Dim Telefonat As String() = Split(index, ";", , CompareMethod.Text)
        Select Case Telefonat(0)
            Case "Wwdh"
                Eintrag = Split(C_XML.Read("Wwdh", "WwdhEintrag " & Telefonat(1), "-1;"), ";", 6, CompareMethod.Text)
            Case "AnrListe"
                Eintrag = Split(C_XML.Read("AnrListe", "AnrListeEintrag " & Telefonat(1), "-1;"), ";", 6, CompareMethod.Text)
            Case "VIPListe"
                Eintrag = Split(C_XML.Read("VIPListe", "VIPListeEintrag " & Telefonat(1), "-1;"), ";", 6, CompareMethod.Text)
            Case Else
                Exit Sub
        End Select

        If Not Left(Eintrag(5), 2) = "-1" And Not Left(Eintrag(4), 2) = "-1" Then
            Try
                oContact = CType(oNS.GetItemFromID(Eintrag(5), Eintrag(4)), Outlook.ContactItem) ' wird durch den Symbolbereich 'Rückruf' in der 'FritzBox'-Symbolleiste ausgeführt
            Catch ex As Exception
                Select Case Telefonat(0)
                    Case "VIPListe"
                        If hf.FBDB_MsgBox("Der zuwählende Kontakt wurde nicht gefunden. Er wurde möglicherweise gelöscht oder verschoben. Soll der zugehörige VIP-Eintrag entfernt werden?", MsgBoxStyle.YesNo, "OnActionAnrListen") = MsgBoxResult.Yes Then
                            GUI.RemoveVIP(Eintrag(4), Eintrag(3))
                        End If
                    Case Else
                        hf.FBDB_MsgBox("Der zuwählende Kontakt wurde nicht gefunden. Er wurde möglicherweise gelöscht oder verschoben.", MsgBoxStyle.Critical, "OnActionAnrListen")
                End Select
                Exit Sub
            End Try
        Else
            oContact = Nothing
        End If
        Wählbox(oContact, Eintrag(1), False, Eintrag(0)) '.TooltipText = TelNr. - .Caption = evtl. vorh. Name.
    End Sub

    Public Sub ZeigeKontakt(ByVal KontaktDaten() As String)
        Dim Vorwahl As String      ' eigene Ortsvorwahl
        Dim Kontakt As Outlook.ContactItem ' Objekt des Kontakteintrags
        Dim vCard As String      ' vCard
        Dim alleTelNr As String      ' alle Telefonnummern in der vCard
        Dim LandesVW As String      ' eigene Landesvorwahl

        LandesVW = C_XML.Read("Optionen", "TBLandesVW", "0049")

        If Left(KontaktDaten(0), 2) = "-1" Then
            ' kein Kontakteintrag vorhanden, dann anlegen und ausfüllen
            Kontakt = CType(ThisAddIn.oApp.CreateItem(Outlook.OlItemType.olContactItem), Outlook.ContactItem)
            vCard = Split(KontaktDaten(0), ";", 2, CompareMethod.Text)(1)
            vCard = Replace(vCard, "=0D", Chr(13), , , CompareMethod.Text)
            vCard = Replace(vCard, "=0A", Chr(10), , , CompareMethod.Text)
            With Kontakt
                If Not vCard = "-1" And Not vCard = "" Then
                    KontaktFunktionen.vCard2Contact(vCard, Kontakt)
                    .Body = .Body & vbNewLine & "Kontaktdaten (vCard):" & vbNewLine & vCard
                End If
                Vorwahl = Left(KontaktDaten(2), 3)
                If Vorwahl = "015" Or Vorwahl = "016" Or Vorwahl = "017" Then
                    .MobileTelephoneNumber = KontaktDaten(2)
                Else
                    If vCard = "-1" Or vCard = "" Then
                        .BusinessTelephoneNumber = KontaktDaten(2)
                    Else
                        ' falls TelNr bei der Rückwärtssuche geändert wurde, diese Nummer als Zweitnummer eintragen
                        alleTelNr = ReadFromVCard(vCard, "TEL", "")
                        If Not hf.nurZiffern(.BusinessTelephoneNumber, LandesVW) = hf.nurZiffern(KontaktDaten(2), LandesVW) And Not .BusinessTelephoneNumber = "" Then
                            .Business2TelephoneNumber = hf.formatTelNr(.BusinessTelephoneNumber)
                            .BusinessTelephoneNumber = hf.formatTelNr(KontaktDaten(2))
                        ElseIf Not hf.nurZiffern(.HomeTelephoneNumber, LandesVW) = hf.nurZiffern(KontaktDaten(2), LandesVW) And Not .HomeTelephoneNumber = "" Then
                            .Home2TelephoneNumber = hf.formatTelNr(.HomeTelephoneNumber)
                            .HomeTelephoneNumber = hf.formatTelNr(KontaktDaten(2))
                        End If
                    End If
                End If

                .Categories = "Fritz!Box"
                .Display()
            End With
        Else
            ' Kontakteintrag anzeigen
            Try
                CType(CType(ThisAddIn.oApp.GetNamespace("MAPI"), Outlook.NameSpace).GetItemFromID(KontaktDaten(0), KontaktDaten(1)), Outlook.ContactItem).Display()
            Catch ex As Exception
                hf.FBDB_MsgBox("Der hinterlegte Kontakt ist nicht mehr verfügbar. Wurde er eventuell gelöscht?", MsgBoxStyle.Information, "")
            End Try
        End If
    End Sub ' (ZeigeKontakt)

    Public Sub Rueckruf(ByVal ID As Integer) 'wird durch formAnrMon Button Rückruf (für das direkte Rückrufen des letzten Anrufers) ausgelöst.
        Dim oNS As Outlook.NameSpace = ThisAddIn.oApp.GetNamespace("MAPI")
        Dim letzterAnrufer() As String = Split(C_XML.Read("letzterAnrufer", "letzterAnrufer " & ID, CStr(DateTime.Now) & ";;unbekannt;;-1;-1;"), ";", 6, CompareMethod.Text)
        Dim KontaktID As String = letzterAnrufer(5)
        Dim StoreID As String = letzterAnrufer(4)
        Dim oContact As Outlook.ContactItem
        If Not Left(KontaktID, 2) = "-1" And Not Left(StoreID, 3) = "-1;" Then
            oContact = CType(oNS.GetItemFromID(KontaktID, StoreID), Outlook.ContactItem)
        Else
            oContact = Nothing
        End If
        Wählbox(oContact, letzterAnrufer(2), False)
    End Sub

    Public Sub WählenAusInspector()
        'Mit diesem Makro ist es möglich direkt aus einem geöffneten Kontakt oder Journaleintrag zu wählen. ähnlich wählboxstart

        Dim olAuswahl As Outlook.Inspector ' das aktuelle Inspector-Fenster (Kontakt oder Journal)
        Dim TelNr As String    ' Telefonnummer des zu Suchenden
        'Dim KontaktID As String = String.Empty   ' KontaktID wird für Wählbox benötigt
        'Dim StoreID As String = String.Empty
        Dim vCard As String
        Dim name As String
        Dim pos1 As Integer
        Dim pos2 As Integer
        Dim Absender As String
        Dim olNamespace As Outlook.NameSpace
        Dim olfolder As Outlook.MAPIFolder


        olAuswahl = ThisAddIn.oApp.ActiveInspector

        If TypeOf olAuswahl.CurrentItem Is Outlook.ContactItem Then ' ist aktuelles Fenster ein Kontakt?
            Dim olContact As Outlook.ContactItem = CType(olAuswahl.CurrentItem, Outlook.ContactItem)
            Wählbox(olContact, "", False)
            hf.NAR(olContact) : olContact = Nothing
        ElseIf TypeOf olAuswahl.CurrentItem Is Outlook.JournalItem Then ' ist aktuelles Fenster ein Journal?
            Dim olJournal As Outlook.JournalItem = CType(olAuswahl.CurrentItem, Outlook.JournalItem)
            If Not InStr(olJournal.Categories, "FritzBox Anrufmonitor") = 0 Then
                ' wurde der Eintrag vom Anrufmonitor angelegt?
                ' TelNr aus dem .Body entnehmen
                TelNr = Mid(olJournal.Body, 11, InStr(1, olJournal.Body, vbNewLine) - 11)
                If Not TelNr = "unbekannt" Then
#If Not OVer = 15 Then
                    If Not olJournal.Links.Count = 0 Then 'KontaktID des darangehangenen Kontaktes ermitteln
                        Dim olLink As Outlook.Link = Nothing
                        Dim olContact As Outlook.ContactItem
                        For Each olLink In olJournal.Links
                            If TypeOf olLink.Item Is Outlook.ContactItem Then
                                olContact = CType(olLink.Item, Outlook.ContactItem)
                                Wählbox(olContact, "", False)
                                hf.NAR(olContact) : olContact = Nothing
                                Exit Sub
                            End If
                        Next
                        hf.NAR(olLink) : olLink = Nothing
                    Else ' Wenn in dem Journal kein Link hinterlegt ist, suche nach einer vCard im Body des Journaleintrags.
#End If
                        Dim vName As String
                        pos1 = InStr(1, olJournal.Body, "BEGIN:VCARD", CompareMethod.Text)
                        pos2 = InStr(1, olJournal.Body, "END:VCARD", CompareMethod.Text)
                        If Not pos1 = 0 And Not pos2 = 0 Then
                            pos2 = pos2 + 9
                            vCard = Mid(olJournal.Body, pos1, pos2 - pos1)
                            name = Replace(ReadFromVCard(vCard, "N", ""), ";", "", , , CompareMethod.Text)
                            vName = "-1" & name & ";" & vCard
                        Else
                            vName = "-1;"
                        End If
                        If Not TelNr Is String.Empty And Not vName Is String.Empty Then Wählbox(Nothing, TelNr, False, vName)
#If Not OVer = 15 Then
                    End If
#End If
                End If
            End If
        ElseIf TypeOf olAuswahl.CurrentItem Is Outlook.MailItem Then ' ist aktuelles Fenster ein Mail?
            Dim res As Outlook.ContactItem
            olNamespace = ThisAddIn.oApp.GetNamespace("MAPI")
            Dim olMail As Outlook.MailItem = CType(olAuswahl.CurrentItem, Outlook.MailItem)
            Absender = olMail.SenderEmailAddress
            If C_XML.Read("Optionen", "CBKHO", "True") = "True" Then
                olfolder = olNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)
                res = KontaktFunktionen.FindeKontakt("", Absender, "", olfolder, Nothing)
            Else
                res = KontaktFunktionen.FindeKontakt("", Absender, "", Nothing, olNamespace)
            End If
            ' Nun den zur Email-Adresse gehörigen Kontakt suchen
            If Not Absender = "" Then
                If Not res Is Nothing Then
                    Wählbox(res, "", False)
                Else
                    hf.FBDB_MsgBox("Es ist kein Kontakt mit der E-Mail-Adresse " & Absender & " vorhanden!", MsgBoxStyle.Exclamation, "WählenAusKontakt")
                End If
            End If
        End If

    End Sub '(WählenAusKontakt)
#End Region


End Class
