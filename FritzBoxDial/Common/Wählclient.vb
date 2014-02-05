Imports System.Drawing

Public Class Wählclient
    Private C_DP As DataProvider
    Private frm_Wählbox As formWählbox
    Private C_hf As Helfer
    Private C_KF As Contacts
    Private C_GUI As GraphicalUserInterface
    Private C_OlI As OutlookInterface
    Private C_FBox As FritzBox
    Private C_Phoner As PhonerInterface

    Public Sub New(ByVal DataProviderKlasse As DataProvider, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal KontaktKlasse As Contacts, _
                   ByVal InterfaceKlasse As GraphicalUserInterface, _
                   ByVal OutlInter As OutlookInterface, _
                   ByVal cFBox As FritzBox, _
                   ByVal PhonerKlasse As PhonerInterface)
        C_hf = HelferKlasse
        C_KF = KontaktKlasse
        C_GUI = InterfaceKlasse
        C_DP = DataProviderKlasse
        C_OlI = OutlInter
        C_FBox = cFBox
        C_Phoner = PhonerKlasse

    End Sub
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
#Region "Alles was mit dem Wählen zu tun hat"
    Friend Sub WählboxStart(ByVal olAuswahl As Outlook.Selection)
        ' wird durch das Symbol 'Wählen' in der 'FritzBox'-Symbolleiste ausgeführt
        Dim olNamespace As Outlook.NameSpace
        Dim aktKontakt As Outlook.ContactItem       ' aktuell ausgewählter Kontakt
        Dim i As Long              ' Zählvariable
        Dim pos1 As Integer
        Dim pos2 As Integer
        Dim vCard As String
        Dim name As String
        Dim res As Outlook.ContactItem

        olNamespace = C_OlI.OutlookApplication.GetNamespace("MAPI")
        ' Ist überhaupt etwas ausgewählt?
        If (olAuswahl.Count = 1) Then
            If TypeOf olAuswahl.Item(1) Is Outlook.MailItem Then
                ' Es wurde eine Mail ausgewählt
                ' Den zur Email-Adresse gehörigen Kontakt suchen
                Dim aktMail As Outlook.MailItem = CType(olAuswahl.Item(1), Outlook.MailItem)
                Dim Absender As String

                Absender = aktMail.SenderEmailAddress
                If C_DP.P_CBKHO Then
                    res = C_KF.FindeKontakt("", Absender, "", olNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts))
                Else
                    res = C_KF.FindeKontakt("", Absender, "", olNamespace)
                End If

                i = 0
                If Not Absender = C_DP.P_Def_StringEmpty Then
                    If Not res Is Nothing Then
                        Wählbox(res, C_DP.P_Def_StringEmpty, False, C_DP.P_Def_StringEmpty)
                    Else
                        C_hf.FBDB_MsgBox("Es ist kein Kontakt mit der E-Mail-Adresse " & Absender & " vorhanden!", MsgBoxStyle.Information, "WählboxStart")
                    End If
                End If
                With C_hf
                    .NAR(olNamespace)
                    .NAR(aktMail)
                End With
                olNamespace = Nothing
                aktMail = Nothing
            ElseIf TypeOf olAuswahl.Item(1) Is Outlook.ContactItem Then
                ' Es wurde gleich ein Kontakt gewählt!
                ' Nun direkt den Wähldialog für den Kontakt anzeigen.
                aktKontakt = CType(olAuswahl.Item(1), Outlook.ContactItem)
                Wählbox(aktKontakt, C_DP.P_Def_StringEmpty, False, C_DP.P_Def_StringEmpty)
                C_hf.NAR(aktKontakt)
                aktKontakt = Nothing
            ElseIf TypeOf olAuswahl.Item(1) Is Outlook.AppointmentItem Then
#If Not OVer = 15 Then
                Dim oAppItem As Outlook.AppointmentItem = CType(olAuswahl.Item(1), Outlook.AppointmentItem)
                Dim oAppLink As Outlook.Link
                Dim oAppThing As Object
                For Each oAppLink In oAppItem.Links
                    oAppThing = oAppLink.Item
                    If TypeOf oAppThing Is Outlook.ContactItem Then 'Nur, wenn der Link auf einen Kontakt zeigt....
                        Wählbox(CType(oAppThing, Outlook.ContactItem), C_DP.P_Def_StringEmpty, False, C_DP.P_Def_StringEmpty)
                        Exit For
                    End If
                Next 'oAppLink
                C_hf.NAR(oAppItem)
                oAppItem = Nothing
#End If
            ElseIf TypeOf olAuswahl.Item(1) Is Outlook.JournalItem Then
                ' Es wurde ein Journaleintrag gewählt!
                Dim aktJournal As Outlook.JournalItem = CType(olAuswahl.Item(1), Outlook.JournalItem)
                If InStr(aktJournal.Body, C_DP.P_Def_StringUnknown) = 0 _
                    And Not InStr(aktJournal.Categories, "FritzBox Anrufmonitor") = 0 Then
#If Not OVer = 15 Then
                    If Not aktJournal.Links.Count = 0 Then
                        Dim oAppLink As Outlook.Link
                        Dim oAppThing As Object
                        For Each oAppLink In aktJournal.Links
                            oAppThing = oAppLink.Item
                            If TypeOf oAppThing Is Outlook.ContactItem Then
                                Wählbox(CType(oAppThing, Outlook.ContactItem), Mid(aktJournal.Body, 11, InStr(1, aktJournal.Body, vbNewLine) - 11), False, C_DP.P_Def_StringEmpty)
                                Exit For
                            End If
                            C_hf.NAR(oAppThing)
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
                            vName = C_DP.P_Def_ErrorMinusOne & name & ";" & vCard
                        Else
                            vName = String.Empty
                        End If

                        Wählbox(Nothing, Mid(aktJournal.Body, 11, InStr(1, aktJournal.Body, vbNewLine) - 11), False, vName)
#If Not OVer = 15 Then
                    End If
#End If

                End If
            Else
                C_hf.FBDB_MsgBox("Es muss entweder ein Kontakt, eine E-Mail-Adresse oder ein Journal ausgewählt sein!", MsgBoxStyle.Exclamation, "WählboxStart")
            End If
        Else
            C_hf.FBDB_MsgBox("Es muss genau ein Element ausgewählt sein!", MsgBoxStyle.Exclamation, "WählboxStart")
        End If
        olAuswahl = Nothing
        Exit Sub
        'Catch
        '    C_hf.FBDB_MsgBox("Es muss entweder ein Kontakt, eine Email oder ein Journal ausgewählt sein!", MsgBoxStyle.Exclamation, "WählboxStart")
        'End Try
    End Sub ' (WählboxStart)

    Sub Wählbox(ByVal oContact As Outlook.ContactItem, ByVal TelNr As String, ByVal Direktwahl As Boolean, ByVal vName As String)
        ' macht alle Eintragungen in 'formWählbox'
        ' aus FritzBoxDial übernommen und überarbeitet
        ' Parameter:  oContact (ContactItem): Kontaktdaten des Anzurufenden
        '             TelNr (String):         Telefonnummer des Anzurufenden
        Dim alleTelNr(14) As String ' alle im Kontakt enthaltenen Telefonnummern
        Dim alleNrTypen(14) As String ' die Bezeichnungen der Telefonnummern
        Dim i, iTelNr As Integer    ' Zählvariablen
        Dim ImgPath As String = C_DP.P_Def_StringEmpty   ' Position innerhalb eines Strings
        Dim row(2) As String

        frm_Wählbox = New formWählbox(Direktwahl, C_DP, C_hf, C_GUI, C_FBox, C_Phoner, C_KF)

        If oContact Is Nothing Then
            frm_Wählbox.Tag = C_DP.P_Def_ErrorMinusOne
        Else
            frm_Wählbox.Tag = oContact.EntryID & ";" & CType(oContact.Parent, Outlook.MAPIFolder).StoreID
        End If

        iTelNr = 1 'Index Zeile im DataGrid des Formulars
        ' Ist der Kontakt nicht vorhanden (z.B. Rückruf)?
        If oContact Is Nothing Then
            If Not Direktwahl Then
                ' Ortsvorwahl vor die Nummer setzen, falls eine Rufnummer nicht mit "0" beginnt und nicht mit "11"
                ' (Rufnummern die mit "11" beginnen sind Notrufnummern oder andere Sondernummern)
                If Not Left(C_hf.nurZiffern(TelNr, C_DP.P_TBLandesVW), 1) = "0" And Not Left(C_hf.nurZiffern(TelNr, C_DP.P_TBLandesVW), 2) = "11" Then _
                    TelNr = C_DP.P_TBVorwahl & TelNr

                If vName = String.Empty Then
                    frm_Wählbox.Text = "Anruf: " & TelNr
                Else
                    Dim st As Integer = InStr(vName, ";", CompareMethod.Text)
                    If st = 0 Then
                        frm_Wählbox.Text = "Anruf: " & vName
                    Else
                        frm_Wählbox.Text = "Anruf: " & Mid(vName, 3, st - 3)
                    End If
                End If
                ' Liste füllen
                row(0) = CStr(iTelNr) 'Index Zeile im DataGrid
                row(2) = TelNr
                frm_Wählbox.ListTel.Rows.Add(row)
            Else
                frm_Wählbox.Text = "Anruf: Direktwahl"
                frm_Wählbox.ContactImage.Visible = False
            End If
        Else
            ' Welche Telefonnummerntypen sollen angezeigt werden?
            With oContact
                ' Fenstertitel setzen
                frm_Wählbox.Text = Replace("Anruf: " & .FullName & " (" & .CompanyName & ")", " ()", "", , , CompareMethod.Text)
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
                ImgPath = C_OlI.KontaktBild(.EntryID, CType(.Parent, Outlook.MAPIFolder).StoreID)
                If Not ImgPath = C_DP.P_Def_StringEmpty Then
                    Dim orgIm As Image = Image.FromFile(ImgPath)
                    With frm_Wählbox.ContactImage
                        Dim Bildgröße As New Size(.Width, CInt((.Width * orgIm.Size.Height) / orgIm.Size.Width))
                        Dim showim As Image = New Bitmap(Bildgröße.Width, Bildgröße.Height)
                        Dim g As Graphics = Graphics.FromImage(showim)
                        g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                        g.DrawImage(orgIm, 0, 0, Bildgröße.Width, Bildgröße.Height)
                        g.Dispose()
                        .Image = showim
                    End With
                Else
                    frm_Wählbox.ContactImage.Visible = False
                End If
            End With
            ' Liste füllen
            For i = LBound(alleTelNr) + 1 To UBound(alleTelNr)
                If Not alleTelNr(i) = C_DP.P_Def_StringEmpty Then
                    ' Wenn die Telefonnummer nicht leer ist, dann in die Liste hinzufügen
                    row(0) = CStr(iTelNr) 'Index wird eins hochgezählt
                    'Ortsvorwahl vor die Nummer setzen, falls eine Rufnummer nicht mit "0" beginnt und nicht mit "11"
                    '(Rufnummern die mit "11" beginnen sind Notrufnummern oder andere Sondernummern)
                    If Not Left(C_hf.nurZiffern(alleTelNr(i), C_DP.P_TBLandesVW), 1) = "0" And Not Left(C_hf.nurZiffern(alleTelNr(i), C_DP.P_TBLandesVW), 2) = "11" Then _
                        alleTelNr(i) = C_DP.P_TBVorwahl & alleTelNr(i)
                    If C_hf.nurZiffern(alleTelNr(i), C_DP.P_TBLandesVW) = C_hf.nurZiffern(TelNr, C_DP.P_TBLandesVW) Then
                        row(1) = alleNrTypen(i) & " *"
                    Else
                        row(1) = alleNrTypen(i)
                    End If
                    row(2) = alleTelNr(i)
                    iTelNr += 1
                    frm_Wählbox.ListTel.Rows.Add(row)
                End If
            Next
            'VIP
            frm_Wählbox.BVIP.Enabled = True
            frm_Wählbox.BVIP.Checked = C_GUI.IsVIP(oContact)
        End If
        With frm_Wählbox
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
                C_hf.FBDB_MsgBox("Der Kontakt hat keine Telefonnummern", MsgBoxStyle.Exclamation, "Wählbox")
            End If
        End With
        ' Wähldialog anzeigen

    End Sub '(Wählbox)

    Sub OnActionListen(ByVal index As String)
        Dim oNS As Outlook.NameSpace = ThisAddIn.P_oApp.GetNamespace("MAPI")
        Dim oContact As Outlook.ContactItem
        Dim Telefonat As String() = Split(index, ";", , CompareMethod.Text) ' ####List;ID
        ' KontaktID, StoreID, TelNr ermitteln
        Dim KontaktID As String
        Dim StoreID As String
        Dim TelNr As String
        Dim Anrufer As String
        Dim ListNodeNames As New ArrayList
        Dim ListNodeValues As New ArrayList
        Dim xPathTeile As New ArrayList

        ' TelNr
        ListNodeNames.Add("TelNr")
        ListNodeValues.Add(C_DP.P_Def_ErrorMinusOne)

        ' Anrufer
        ListNodeNames.Add("Anrufer")
        ListNodeValues.Add(C_DP.P_Def_ErrorMinusOne)

        ' StoreID
        ListNodeNames.Add("StoreID")
        ListNodeValues.Add(C_DP.P_Def_ErrorMinusOne)

        ' KontaktID
        ListNodeNames.Add("KontaktID")
        ListNodeValues.Add(C_DP.P_Def_ErrorMinusOne & ";")

        With xPathTeile
            .Add(Telefonat(0))
            .Add("Eintrag")
        End With
        C_DP.ReadXMLNode(xPathTeile, ListNodeNames, ListNodeValues, Telefonat(1))

        Anrufer = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("Anrufer")))
        TelNr = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("TelNr")))
        KontaktID = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("KontaktID")))
        StoreID = CStr(ListNodeValues.Item(ListNodeNames.IndexOf("StoreID")))

        If Not KontaktID = C_DP.P_Def_ErrorMinusOne And Not StoreID = C_DP.P_Def_ErrorMinusOne Then
            Try
                oContact = CType(oNS.GetItemFromID(KontaktID, StoreID), Outlook.ContactItem) ' wird durch den Symbolbereich 'Rückruf' in der 'FritzBox'-Symbolleiste ausgeführt
            Catch ex As Exception
                Select Case Telefonat(0)
                    Case "VIPListe"
                        If C_hf.FBDB_MsgBox("Der zuwählende Kontakt wurde nicht gefunden. Er wurde möglicherweise gelöscht oder verschoben. Soll der zugehörige VIP-Eintrag entfernt werden?", MsgBoxStyle.YesNo, "OnActionAnrListen") = MsgBoxResult.Yes Then
                            C_GUI.RemoveVIP(KontaktID, StoreID)
                        End If
                    Case Else
                        C_hf.FBDB_MsgBox("Der zuwählende Kontakt wurde nicht gefunden. Er wurde möglicherweise gelöscht oder verschoben.", MsgBoxStyle.Critical, "OnActionAnrListen")
                End Select
                Exit Sub
            End Try
        Else
            oContact = Nothing
        End If
        Wählbox(oContact, TelNr, False, Anrufer) '.TooltipText = TelNr. - .Caption = evtl. vorh. Name.
    End Sub

    ' (ZeigeKontakt)
    ''' <summary>
    ''' Wird durch formAnrMon Button Rückruf (für das direkte Rückrufen des letzten Anrufers) ausgelöst.
    ''' </summary>
    ''' <param name="ID">Die ID des letzten Anrufers.</param>
    ''' <remarks>131211 erfolgreich</remarks>
    Public Sub Rueckruf(ByVal ID As Integer)
        Dim StoreID As String
        Dim KontaktID As String
        Dim TelNr As String
        Dim oNS As Outlook.NameSpace = C_OlI.OutlookApplication.GetNamespace("MAPI")
        Dim oContact As Outlook.ContactItem

        Dim xPathTeile As New ArrayList
        With xPathTeile
            .Add("LetzterAnrufer")
            .Add("Eintrag[@ID = """ & ID & """]")

            .Add("TelNr")
            TelNr = C_DP.Read(xPathTeile, C_DP.P_Def_StringUnknown)

            .Item(.Count - 1) = "StoreID"
            StoreID = C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne)

            .Item(.Count - 1) = "KontaktID"
            KontaktID = C_DP.Read(xPathTeile, C_DP.P_Def_ErrorMinusOne)
        End With

        If Not Left(KontaktID, 2) = C_DP.P_Def_ErrorMinusOne And Not Left(StoreID, 3) = C_DP.P_Def_ErrorMinusOne & ";" Then
            oContact = CType(oNS.GetItemFromID(KontaktID, StoreID), Outlook.ContactItem)
        Else
            oContact = Nothing
        End If
        Wählbox(oContact, TelNr, False, C_DP.P_Def_StringEmpty)
    End Sub

    Public Sub WählenAusInspector()
        'Mit diesem Makro ist es möglich direkt aus einem geöffneten Kontakt oder Journaleintrag zu wählen. ähnlich wählboxstart

        Dim olAuswahl As Outlook.Inspector ' das aktuelle Inspector-Fenster (Kontakt oder Journal)
        Dim TelNr As String    ' Telefonnummer des zu Suchenden
        Dim vCard As String
        Dim name As String
        Dim pos1 As Integer
        Dim pos2 As Integer
        Dim Absender As String
        Dim olNamespace As Outlook.NameSpace


        olAuswahl = ThisAddIn.P_oApp.ActiveInspector

        If TypeOf olAuswahl.CurrentItem Is Outlook.ContactItem Then ' ist aktuelles Fenster ein Kontakt?
            Dim olContact As Outlook.ContactItem = CType(olAuswahl.CurrentItem, Outlook.ContactItem)
            Wählbox(olContact, C_DP.P_Def_StringEmpty, False, C_DP.P_Def_StringEmpty)
            C_hf.NAR(olContact) : olContact = Nothing
        ElseIf TypeOf olAuswahl.CurrentItem Is Outlook.JournalItem Then ' ist aktuelles Fenster ein Journal?
            Dim olJournal As Outlook.JournalItem = CType(olAuswahl.CurrentItem, Outlook.JournalItem)
            If Not InStr(olJournal.Categories, "FritzBox Anrufmonitor") = 0 Then
                ' wurde der Eintrag vom Anrufmonitor angelegt?
                ' TelNr aus dem .Body entnehmen
                TelNr = Mid(olJournal.Body, 11, InStr(1, olJournal.Body, vbNewLine) - 11)
                If Not TelNr = C_DP.P_Def_StringUnknown Then
#If Not OVer = 15 Then
                    If Not olJournal.Links.Count = 0 Then 'KontaktID des darangehangenen Kontaktes ermitteln
                        Dim olLink As Outlook.Link = Nothing
                        Dim olContact As Outlook.ContactItem
                        For Each olLink In olJournal.Links
                            If TypeOf olLink.Item Is Outlook.ContactItem Then
                                olContact = CType(olLink.Item, Outlook.ContactItem)
                                Wählbox(olContact, C_DP.P_Def_StringEmpty, False, C_DP.P_Def_StringEmpty)
                                C_hf.NAR(olContact) : olContact = Nothing
                                Exit Sub
                            End If
                        Next
                        C_hf.NAR(olLink) : olLink = Nothing
                    Else ' Wenn in dem Journal kein Link hinterlegt ist, suche nach einer vCard im Body des Journaleintrags.
#End If
                        Dim vName As String
                        pos1 = InStr(1, olJournal.Body, "BEGIN:VCARD", CompareMethod.Text)
                        pos2 = InStr(1, olJournal.Body, "END:VCARD", CompareMethod.Text)
                        If Not pos1 = 0 And Not pos2 = 0 Then
                            pos2 = pos2 + 9
                            vCard = Mid(olJournal.Body, pos1, pos2 - pos1)
                            name = Replace(ReadFromVCard(vCard, "N", ""), ";", "", , , CompareMethod.Text)
                            vName = C_DP.P_Def_ErrorMinusOne & name & ";" & vCard
                        Else
                            vName = C_DP.P_Def_ErrorMinusOne & ";"
                        End If
                        If Not TelNr Is String.Empty And Not vName Is String.Empty Then Wählbox(Nothing, TelNr, False, vName)
#If Not OVer = 15 Then
                    End If
#End If
                End If
            End If
        ElseIf TypeOf olAuswahl.CurrentItem Is Outlook.MailItem Then ' ist aktuelles Fenster ein Mail?
            Dim res As Outlook.ContactItem
            olNamespace = ThisAddIn.P_oApp.GetNamespace("MAPI")
            Dim olMail As Outlook.MailItem = CType(olAuswahl.CurrentItem, Outlook.MailItem)
            Absender = olMail.SenderEmailAddress
            If C_DP.P_CBKHO Then
                res = C_KF.FindeKontakt(C_DP.P_Def_StringEmpty, Absender, C_DP.P_Def_StringEmpty, olNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts))
            Else
                res = C_KF.FindeKontakt(C_DP.P_Def_StringEmpty, Absender, C_DP.P_Def_StringEmpty, olNamespace)
            End If
            ' Nun den zur Email-Adresse gehörigen Kontakt suchen
            If Not Absender = C_DP.P_Def_StringEmpty Then
                If Not res Is Nothing Then
                    Wählbox(res, C_DP.P_Def_StringEmpty, False, C_DP.P_Def_StringEmpty)
                Else
                    C_hf.FBDB_MsgBox("Es ist kein Kontakt mit der E-Mail-Adresse " & Absender & " vorhanden!", MsgBoxStyle.Exclamation, "WählenAusKontakt")
                End If
            End If
        End If

    End Sub '(WählenAusKontakt)
#End Region


End Class
