﻿Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.Office.Interop.Outlook
Friend Module KontaktFunktionen
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    ''' <summary>
    ''' Erstellt einen Kontakt aus einer vCard.
    ''' </summary>
    ''' <param name="KontaktID">Rückgabewert: KontaktID des neu erstellten Kontaktes.</param>
    ''' <param name="StoreID">Rückgabewert: StoreID des Ordners, in dem sich der neu erstellte Kontaktes befindet.</param>
    ''' <param name="vCard">Kontaktdaten im vCard-Format.</param>
    ''' <param name="TelNr">Telefonnummer, die zusätzlich eingetragen werden soll.</param>
    ''' <param name="AutoSave">Gibt an ob der Kontakt gespeichert werden soll True, oder nur angezeigt werden soll False.</param>
    ''' <returns>Den erstellten Kontakt als Outlook.ContactItem.</returns>
    Friend Function ErstelleKontakt(ByRef KontaktID As String, ByRef StoreID As String, vCard As String, TelNr As Telefonnummer, AutoSave As Boolean) As ContactItem
        Dim olKontakt As ContactItem


        If Not TelNr.Unbekannt Then

            olKontakt = CType(ThisAddIn.OutookApplication.CreateItem(OlItemType.olContactItem), ContactItem)

            With olKontakt

                If TelNr.IstMobilnummer Then
                    .MobileTelephoneNumber = TelNr.Formatiert
                Else
                    .BusinessTelephoneNumber = TelNr.Formatiert
                End If

                If vCard.IsNotStringEmpty And vCard.IsNotErrorString Then

                    DeserializevCard(vCard, olKontakt)

                    ' Formatiere Telefonnummer
                    If .BusinessTelephoneNumber.IsNotStringEmpty Then
                        Using BTel As New Telefonnummer
                            BTel.SetNummer = .BusinessTelephoneNumber

                            If Not BTel.Equals(TelNr) Then
                                .Business2TelephoneNumber = BTel.Formatiert
                            End If
                        End Using
                        .BusinessTelephoneNumber = TelNr.Formatiert
                    ElseIf .HomeTelephoneNumber.IsNotStringEmpty Then
                        Using HTel As New Telefonnummer
                            HTel.SetNummer = .HomeTelephoneNumber

                            If Not HTel.Equals(TelNr) Then
                                .Home2TelephoneNumber = HTel.Formatiert
                            End If
                        End Using
                        .BusinessTelephoneNumber = TelNr.Formatiert

                        .HomeTelephoneNumber = TelNr.Formatiert
                    ElseIf .HomeTelephoneNumber.IsStringEmpty Then
                        .HomeTelephoneNumber = TelNr.Formatiert
                    End If

                    .Categories = My.Resources.strDefLongName 'Alle Kontakte, die erstellt werden, haben diese Kategorie. Damit sind sie einfach zu erkennen
                    .Body = .Body & vbCrLf & "Erstellt durch das " & My.Resources.strDefLongName & " am " & Now & Dflt2NeueZeile & "vCard:" & Dflt2NeueZeile & vCard
                End If

            End With

            If AutoSave Then SpeichereKontakt(olKontakt)

            ErstelleKontakt = olKontakt
        Else
            Return Nothing
        End If

    End Function
    ''' <summary>
    ''' Erstellt einen Kontakt aus einer Fritz!Box Telefonbucheintrag.
    ''' </summary>
    ''' <param name="KontaktID">Rückgabewert: KontaktID des neu erstellten Kontaktes</param>
    ''' <param name="StoreID">Rückgabewert: StoreID des Ordners, in dem sich der neu erstellte Kontaktes befindet.</param>
    ''' <param name="XMLKontakt">Kontaktdaten als Fritz!Box Telefonbucheintrag</param>
    ''' <param name="TelNr">Telefonnummer, die zusätzlich eingetragen werden soll.</param>
    ''' <param name="AutoSave">Gibt an ob der Kontakt gespeichert werden soll True, oder nur angezeigt werden soll False.</param>
    ''' <returns>Den erstellten Kontakt als Outlook.ContactItem.</returns>
    Friend Function ErstelleKontakt(ByRef KontaktID As String, ByRef StoreID As String, XMLKontakt As FritzBoxXMLKontakt, TelNr As Telefonnummer, AutoSave As Boolean) As ContactItem
        Dim olKontakt As ContactItem

        If Not TelNr.Unbekannt Then

            olKontakt = CType(ThisAddIn.OutookApplication.CreateItem(OlItemType.olContactItem), ContactItem)

            With olKontakt

                If TelNr.IstMobilnummer Then
                    .MobileTelephoneNumber = TelNr.Formatiert
                Else
                    .BusinessTelephoneNumber = TelNr.Formatiert
                End If

                If XMLKontakt IsNot Nothing Then
                    XMLKontakt.XMLKontaktOutlook(olKontakt)

                    .Categories = My.Resources.strDefLongName ' 'Alle Kontakte, die erstellt werden, haben diese Kategorie. Damit sind sie einfach zu erkennen
                    .Body = .Body & vbCrLf & "Erstellt durch das " & My.Resources.strDefLongName & " am " & Now
                End If

            End With

            If AutoSave Then SpeichereKontakt(olKontakt)

            Return olKontakt
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Speichert einen automatisch erstellten Kontakt im dafür vorgesehenen Ordner ab
    ''' </summary>
    ''' <param name="olKontakt">Der zu speichernde Kontakt</param>
    Private Sub SpeichereKontakt(ByRef olKontakt As ContactItem)
        With XMLData.POptionen.OutlookOrdner

            ' Ermittle den Ordner in den der Kontakt gespeichet werden soll
            Dim KontaktOrdner As MAPIFolder = .GetMAPIFolder(OutlookOrdnerVerwendung.KontaktSpeichern)
            ' Speichere den Kontakt... 
            ' Wenn es sich nicht um den Hauptkontaktordner handelt, ist darin der Kontakt bereits (ungespeichert) enthalten. Ein Move würde den Kontakt nur dublizieren.
            If KontaktOrdner.AreNotEqual(.GetDefaultMAPIFolder(OlDefaultFolders.olFolderContacts)) Then
                ' Verschiebe den Kontakt in den gewünschten Ornder
                olKontakt = CType(olKontakt.Move(KontaktOrdner), ContactItem)
                NLogger.Info($"Kontakt {olKontakt.FullName} wurde erstellt und in den Ordner {KontaktOrdner.Name} verschoben.")

            Else
                ' Speichere den Kontakt im Kontakthauptordner
                If olKontakt.Speichern Then NLogger.Info($"Kontakt {olKontakt.FullName} wurde Hauptkontaktordner gespeichert.")

            End If

            ' Indizere den Kontakt, wenn der Ordner, in den er gespeichert werden soll, auch zur Kontaktsuche verwendet werden soll
            IndiziereKontakt(olKontakt, KontaktOrdner)

        End With

    End Sub

    ''' <summary>
    ''' Erstellt einen leeren Kontakt und ergänzt eine Telefonnummer.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, die eingefügt werden soll.</param>
    ''' <param name="Speichern">Gibt an ob der Kontakt gespeichert werden soll True, oder nur angezeigt werden soll False.</param>
    ''' <returns>Den erstellte Kontakt als Outlook.ContactItem.</returns>
    Friend Function ErstelleKontakt(TelNr As Telefonnummer, Speichern As Boolean) As ContactItem
        Return ErstelleKontakt(DfltStringEmpty, DfltStringEmpty, DfltStringEmpty, TelNr, Speichern)
    End Function

    ''' <summary>
    ''' Erstellt einen Kontakt aus einem Inspectorfenster (Journal)
    ''' </summary>
    Friend Sub ZeigeKontaktAusJournal(olJournal As JournalItem)
        Dim vCard As String
        Dim olKontakt As ContactItem = Nothing ' Objekt des Kontakteintrags
        Dim TelNr As Telefonnummer

        With olJournal
            If .Categories.Contains(DfltJournalKategorie) Then

                olKontakt = GetOutlookKontakt(CType(.PropertyAccessor.GetProperties(DASLTagJournal), Object()))

                If olKontakt Is Nothing Then

                    TelNr = New Telefonnummer
                    'Telefonnummer aus dem .Body herausfiltern
                    TelNr.SetNummer = .Body.GetSubString(PfltJournalBodyStart, "Status: ")

                    ' Prüfe ob TelNr unterdrückt
                    If TelNr.Unbekannt Then
                        olKontakt = ErstelleKontakt(TelNr, False)
                    Else
                        ' Entweder erst eingebetteten Kontakt suchen, oder nach vCard suchen.
                        ' vCard aus dem .Body herausfiltern
                        vCard = DfltBegin_vCard & .Body.GetSubString(DfltBegin_vCard, DfltEnd_vCard) & DfltEnd_vCard

                        'Wenn keine vCard im Body gefunden
                        If vCard.AreEqual(DfltBegin_vCard & DfltStrErrorMinusOne & DfltEnd_vCard) Then
                            ' wenn nicht, dann neuen Kontakt mit TelNr öffnen
                            olKontakt = ErstelleKontakt(TelNr, False)
                        Else
                            'vCard gefunden
                            olKontakt = ErstelleKontakt(DfltStringEmpty, DfltStringEmpty, vCard, TelNr, False)
                        End If
                    End If
                End If
            End If
        End With
        If olKontakt IsNot Nothing Then olKontakt.Display()
        ReleaseComObject(olJournal)

    End Sub ' (ZeigeKontaktAusJournal)

    Friend Sub ZeigeKontaktAusInspector(olInsp As Inspector)
        If olInsp IsNot Nothing Then
            If TypeOf olInsp.CurrentItem Is JournalItem Then
                ZeigeKontaktAusJournal(CType(olInsp.CurrentItem, JournalItem))
            End If
        End If
    End Sub ' (ZeigeKontaktAusInspector)

    Friend Sub ZeigeKontaktAusSelection(olSelection As Selection)
        If olSelection IsNot Nothing Then

            If TypeOf olSelection.Item(1) Is JournalItem Then
                ZeigeKontaktAusJournal(CType(olSelection.Item(1), JournalItem))
            End If
        End If
    End Sub ' (ZeigeKontaktAusSelection)

    ''' <summary>
    ''' Speichert das Kontaktbild in den Arbeitsorder. 
    ''' </summary>
    ''' <param name="olContact">Kontakt, aus dem das Kontaktbild extrahiert werden soll.</param>
    ''' <returns>Pfad zum extrahierten Kontaktbild.</returns>
    <Extension> Friend Function KontaktBild(olContact As ContactItem) As String
        KontaktBild = DfltStringEmpty
        If olContact IsNot Nothing Then
            With olContact
                With .Attachments
                    If .Item("ContactPicture.jpg") IsNot Nothing Then
                        KontaktBild = Path.GetTempPath() & Path.GetRandomFileName()
                        KontaktBild = Left(KontaktBild, Len(KontaktBild) - 3) & "jpg"
                        .Item("ContactPicture.jpg").SaveAsFile(KontaktBild)
                    End If
                End With
            End With
        End If
    End Function
    ''' <summary>
    ''' Löscht das Kontaktbild in den Arbeitsorder. 
    ''' </summary>
    ''' <param name="PfadKontaktBild">Pfad zum extrahierten Kontaktbild</param>
    Friend Sub DelKontaktBild(PfadKontaktBild As String)
        If PfadKontaktBild.IsNotStringEmpty Then
            With My.Computer.FileSystem
                If .FileExists(PfadKontaktBild) Then
                    .DeleteFile(PfadKontaktBild, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                End If
            End With
        End If
    End Sub
    ''' <summary>
    ''' Ermittelt aus der KontaktID (EntryID) und der StoreID den zugehörigen Kontakt.
    ''' </summary>
    ''' <param name="KontaktID">EntryID des Kontaktes</param>
    ''' <param name="StoreID">StoreID des beinhaltenden Ordners</param>
    ''' <returns>Erfolg: Kontakt, Misserfolg: Nothing</returns>
    Friend Function GetOutlookKontakt(ByRef KontaktID As String, ByRef StoreID As String) As ContactItem
        GetOutlookKontakt = Nothing
        Try
            GetOutlookKontakt = CType(ThisAddIn.OutookApplication.Session.GetItemFromID(KontaktID, StoreID), ContactItem)
        Catch ex As System.Exception
            NLogger.Error(ex)
        End Try
    End Function

    Friend Function GetOutlookKontakt(ByRef KontaktIDStoreID As Object()) As ContactItem
        GetOutlookKontakt = Nothing

        If Not KontaktIDStoreID.Contains(DfltErrorvalue) Then
            Try
                GetOutlookKontakt = CType(ThisAddIn.OutookApplication.Session.GetItemFromID(KontaktIDStoreID.First.ToString, KontaktIDStoreID.Last.ToString), ContactItem)
            Catch ex As System.Exception
                NLogger.Error(ex)
            End Try
        End If
    End Function
    ''' <summary>
    ''' Ermittelt aus der FolderID (EntryID) und der StoreID den zugehörigen Ordner.
    ''' </summary>
    ''' <param name="FolderID">EntryID des Ordners</param>
    ''' <param name="StoreID">StoreID des Ordners</param>
    ''' <returns>Erfolg: Ordner, Misserfolg: Standard-Kontaktordner</returns>
    ''' <remarks>In Office 2003 ist Outlook.Folder unbekannt, daher Outlook.MAPIFolder</remarks>
    Friend Function GetOutlookFolder(ByRef FolderID As String, ByRef StoreID As String) As MAPIFolder
        GetOutlookFolder = Nothing

        If FolderID.IsNotErrorString And StoreID.IsNotErrorString Then
            Try
                GetOutlookFolder = ThisAddIn.OutookApplication.Session.GetFolderFromID(FolderID, StoreID)
            Catch ex As System.Exception
                NLogger.Error(ex)
            End Try
        End If

    End Function
    <Extension> Friend Function GetKontaktTelNrList(ByRef olContact As ContactItem) As List(Of Telefonnummer)

        Dim alleTelNr(14) As String ' alle im Kontakt enthaltenen Telefonnummern
        Dim alleNrTypen(14) As TelNrType ' die Bezeichnungen der Telefonnummern
        Dim tmpTelNr As Telefonnummer

        With olContact
            alleTelNr(1) = .AssistantTelephoneNumber : alleNrTypen(1).TelNrType = OutlookNrType.AssistantTelephoneNumber
            alleTelNr(2) = .BusinessTelephoneNumber : alleNrTypen(2).TelNrType = OutlookNrType.BusinessTelephoneNumber
            alleTelNr(3) = .Business2TelephoneNumber : alleNrTypen(3).TelNrType = OutlookNrType.Business2TelephoneNumber
            alleTelNr(4) = .CallbackTelephoneNumber : alleNrTypen(4).TelNrType = OutlookNrType.CallbackTelephoneNumber
            alleTelNr(5) = .CarTelephoneNumber : alleNrTypen(5).TelNrType = OutlookNrType.CarTelephoneNumber
            alleTelNr(6) = .CompanyMainTelephoneNumber : alleNrTypen(6).TelNrType = OutlookNrType.CompanyMainTelephoneNumber
            alleTelNr(7) = .HomeTelephoneNumber : alleNrTypen(7).TelNrType = OutlookNrType.HomeTelephoneNumber
            alleTelNr(8) = .Home2TelephoneNumber : alleNrTypen(8).TelNrType = OutlookNrType.Home2TelephoneNumber
            alleTelNr(9) = .ISDNNumber : alleNrTypen(9).TelNrType = OutlookNrType.ISDNNumber
            alleTelNr(10) = .MobileTelephoneNumber : alleNrTypen(10).TelNrType = OutlookNrType.MobileTelephoneNumber
            alleTelNr(11) = .OtherTelephoneNumber : alleNrTypen(11).TelNrType = OutlookNrType.OtherTelephoneNumber
            alleTelNr(12) = .PagerNumber : alleNrTypen(12).TelNrType = OutlookNrType.PagerNumber
            alleTelNr(13) = .PrimaryTelephoneNumber : alleNrTypen(13).TelNrType = OutlookNrType.PrimaryTelephoneNumber
            alleTelNr(14) = .RadioTelephoneNumber : alleNrTypen(14).TelNrType = OutlookNrType.RadioTelephoneNumber
        End With

        GetKontaktTelNrList = New List(Of Telefonnummer)
        For i = LBound(alleTelNr) To UBound(alleTelNr)
            If alleTelNr(i).IsNotStringNothingOrEmpty Then
                tmpTelNr = New Telefonnummer With {.SetNummer = alleTelNr(i), .Typ = alleNrTypen(i)}
                GetKontaktTelNrList.Add(tmpTelNr)
            End If
        Next
    End Function

    <Extension> Friend Function GetKontaktTelNrList(ByRef olExchangeNutzer As ExchangeUser) As List(Of Telefonnummer)

        Dim alleTelNr(2) As String ' alle im Exchangenutzer enthaltenen Telefonnummern
        Dim alleNrTypen(2) As TelNrType ' die Bezeichnungen der Telefonnummern
        Dim tmpTelNr As Telefonnummer

        With olExchangeNutzer
            alleTelNr(1) = .BusinessTelephoneNumber : alleNrTypen(1).TelNrType = OutlookNrType.BusinessTelephoneNumber
            alleTelNr(2) = .MobileTelephoneNumber : alleNrTypen(2).TelNrType = OutlookNrType.MobileTelephoneNumber
        End With

        GetKontaktTelNrList = New List(Of Telefonnummer)
        For i = LBound(alleTelNr) To UBound(alleTelNr)
            If alleTelNr(i).IsNotStringNothingOrEmpty Then
                tmpTelNr = New Telefonnummer With {.SetNummer = alleTelNr(i), .Typ = alleNrTypen(i)}
                GetKontaktTelNrList.Add(tmpTelNr)
            End If
        Next
    End Function

    Friend Function ZähleOutlookKontakte(olFolder As MAPIFolder) As Integer
        Dim retval As Integer = 0

        ' Die Anzahl der Elemente dieses Ordners zählen
        If olFolder.DefaultItemType = OlItemType.olContactItem Then
            retval = olFolder.Items.Count

            ' Unterordner werden rekursiv mitgezählt
            If XMLData.POptionen.CBSucheUnterordner Then
                For Each Unterordner As MAPIFolder In olFolder.Folders
                    retval += ZähleOutlookKontakte(Unterordner)
                    Unterordner.ReleaseComObject
                Next
                'olFolder.ReleaseComObject
            End If
        End If
        Return retval
    End Function

    <Extension> Friend Function StoreID(olKontakt As ContactItem) As String
        Return CType(olKontakt.Parent, MAPIFolder).StoreID
    End Function

    <Extension> Friend Function GetTelNrArray(olContact As ContactItem) As Object()

        Dim tmpTelNr(18) As Object
        With olContact
            tmpTelNr(0) = .AssistantTelephoneNumber
            tmpTelNr(1) = .BusinessTelephoneNumber
            tmpTelNr(2) = .Business2TelephoneNumber
            tmpTelNr(3) = .CallbackTelephoneNumber
            tmpTelNr(4) = .CarTelephoneNumber
            tmpTelNr(5) = .CompanyMainTelephoneNumber
            tmpTelNr(6) = .HomeTelephoneNumber
            tmpTelNr(7) = .Home2TelephoneNumber
            tmpTelNr(8) = .ISDNNumber
            tmpTelNr(9) = .MobileTelephoneNumber
            tmpTelNr(10) = .OtherTelephoneNumber
            tmpTelNr(11) = .PagerNumber
            tmpTelNr(12) = .PrimaryTelephoneNumber
            tmpTelNr(13) = .RadioTelephoneNumber
            tmpTelNr(14) = .BusinessFaxNumber
            tmpTelNr(15) = .HomeFaxNumber
            tmpTelNr(16) = .OtherFaxNumber
            tmpTelNr(17) = .TelexNumber
            tmpTelNr(18) = .TTYTDDTelephoneNumber
        End With
        Return tmpTelNr

    End Function

    <Extension> Friend Function Speichern(ByRef olKontakt As ContactItem) As Boolean
        Try
            olKontakt.Save()
            Return True
        Catch ex As System.Exception
            NLogger.Error(ex, "Kontakt {0} kann nicht gespeichert werden.", olKontakt.FullNameAndCompany)
            Return False
        End Try
    End Function

    <Extension> Friend Function ParentFolder(ByRef olKontakt As ContactItem) As MAPIFolder
        If olKontakt.Parent IsNot Nothing Then
            Return CType(olKontakt.Parent, MAPIFolder)
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Verleicht zwei MAPIFolder anhand der StoreID und der EntryID
    ''' </summary>
    ''' <param name="Ordner1">Erster MAPIFolder</param>
    ''' <param name="Ordner2">Zweiter MAPIFolder</param>
    ''' <returns></returns>
    <Extension> Friend Function AreEqual(Ordner1 As MAPIFolder, Ordner2 As MAPIFolder) As Boolean
        Return Ordner1.StoreID.AreEqual(Ordner2.StoreID) And Ordner1.EntryID.AreEqual(Ordner2.EntryID)
    End Function
    <Extension> Friend Function AreNotEqual(Ordner1 As MAPIFolder, Ordner2 As MAPIFolder) As Boolean
        Return Ordner1.StoreID.AreNotEqual(Ordner2.StoreID) Or Ordner1.EntryID.AreNotEqual(Ordner2.EntryID)
    End Function
#Region "VIP"
    <Extension> Friend Function IsVIP(olKontakt As ContactItem) As Boolean
        ' Prüfe, ob sich der Kontakt in der Liste befindet.
        If XMLData.PTelListen.VIPListe IsNot Nothing Then
            With XMLData.PTelListen.VIPListe
                Return .Exists(Function(VIPEintrag) VIPEintrag.EntryID.AreEqual(olKontakt.EntryID) And VIPEintrag.StoreID.AreEqual(olKontakt.StoreID))
            End With
        End If
        Return False
    End Function

    <Extension> Friend Function ToggleVIP(olKontakt As ContactItem) As Boolean
        If XMLData.PTelListen.VIPListe Is Nothing Then XMLData.PTelListen.VIPListe = New List(Of VIPEntry)

        If olKontakt.IsVIP Then
            ' Entferne den Kontakt von der Liste
            XMLData.PTelListen.VIPListe.RemoveAll(Function(VIPEintrag) VIPEintrag.EntryID.AreEqual(olKontakt.EntryID) And VIPEintrag.StoreID.AreEqual(olKontakt.StoreID))

        Else
            ' Füge einen neuen Eintrag hinzu
            XMLData.PTelListen.VIPListe.Add(New VIPEntry With {.Name = olKontakt.FullNameAndCompany, .EntryID = olKontakt.EntryID, .StoreID = olKontakt.StoreID})

        End If

        ThisAddIn.POutlookRibbons.RefreshRibbon()

        Return olKontakt.IsVIP
    End Function

#End Region

    ''' <summary>
    ''' Gibt die Absender-SMTP-Adresse der E-Mail zurück
    ''' </summary>
    ''' <param name="EMail"></param>
    ''' <remarks>https://docs.microsoft.com/de-de/office/client-developer/outlook/pia/how-to-get-the-smtp-address-of-the-sender-of-a-mail-item</remarks>
    ''' <returns></returns>
    Friend Function GetSenderSMTPAddress(EMail As MailItem) As EMailType

        GetSenderSMTPAddress = New EMailType With {.Addresse = DfltStringEmpty}

        If EMail IsNot Nothing Then

            With GetSenderSMTPAddress

                If EMail.SenderEmailType = "EX" Then

                    ' Exchange User
                    .OutlookTyp = OutlookEMailType.EX

                    Dim Adresseintrag As AddressEntry = EMail.Sender

                    Select Case Adresseintrag?.AddressEntryUserType

                        Case OlAddressEntryUserType.olExchangeUserAddressEntry, OlAddressEntryUserType.olExchangeRemoteUserAddressEntry
                            Dim ExchangeUser As ExchangeUser = Adresseintrag.GetExchangeUser()

                            If ExchangeUser IsNot Nothing Then .Addresse = ExchangeUser.PrimarySmtpAddress

                            ' COM Objekt freigeben
                            ExchangeUser.ReleaseComObject

                        Case Else
                            .Addresse = TryCast(Adresseintrag.PropertyAccessor.GetProperty(DfltDASLSMTPAdress), String)

                    End Select
                    ' COM Objekt freigeben
                    Adresseintrag.ReleaseComObject
                Else

                    ' SMTP Adresse (klassische E-Mail)
                    GetSenderSMTPAddress.OutlookTyp = OutlookEMailType.SMTP

                    .Addresse = EMail.SenderEmailAddress
                End If
            End With

        End If

        Return GetSenderSMTPAddress
    End Function

    Friend Async Sub StartKontaktRWS(olContact As ContactItem, TelNr As Telefonnummer)

        With olContact
            Dim vCard As String

            vCard = Await StartRWS(TelNr, False)

            If vCard.IsStringNothingOrEmpty Then
                .Body += $"{Dflt1NeueZeile}{DfltJournalRWSFehler} {TelNr.Formatiert}"
            Else
                .Body += String.Format($"{Dflt1NeueZeile}{Dflt2NeueZeile}{DfltJournalTextKontaktvCard}{vCard}")

                DeserializevCard(vCard, olContact)
            End If

        End With
    End Sub

    <Extension> Friend Function ErstelleXMLKontakt(olContact As ContactItem) As FritzBoxXMLKontakt

        ' Erstelle ein nen neuen XMLKontakt
        Dim XMLKontakt As New FritzBoxXMLKontakt

        With XMLKontakt
            ' Weise den Namen zu
            .Person.RealName = olContact.FullName

            With .Telefonie
                ' Weise die E-Mails zu
                With .Emails
                    If olContact.Email1Address.IsNotStringNothingOrEmpty Then
                        .Add(New FritzBoxXMLEmail With {.EMail = olContact.Email1Address})
                    End If
                    If olContact.Email2Address.IsNotStringNothingOrEmpty Then
                        .Add(New FritzBoxXMLEmail With {.EMail = olContact.Email2Address})
                    End If
                    If olContact.Email3Address.IsNotStringNothingOrEmpty Then
                        .Add(New FritzBoxXMLEmail With {.EMail = olContact.Email3Address})
                    End If
                End With

                ' Weise die Telefonnummern zu
                With .Nummern
                    For Each TelNr In GetKontaktTelNrList(olContact)
                        .Add(New FritzBoxXMLNummer With {.Nummer = TelNr.Unformatiert, .Typ = TelNr.Typ.XML})
                    Next
                End With

            End With

        End With
        Return XMLKontakt
    End Function

    ''' <summary>
    ''' Überführt eine Auflistung von <see cref="ContactItem"/> zu einer Auflistung von XML_Strings (Fritz!Box Telefonbuch). 
    ''' </summary>
    ''' <param name="olContacts">Die Auflistung von <see cref="ContactItem"/></param>
    ''' <returns>Auflistung von XML_Strings (Fritz!Box Telefonbuch)</returns>
    ''' <remarks>Die Auflistung kann leere Strings enthalten.</remarks>
    <Extension> Friend Function ErstelleXMLKontakte(olContacts As IEnumerable(Of ContactItem)) As IEnumerable(Of String)
        Return olContacts.Select(Function(X)
                                     Dim NeuerKontakt As String = DfltStringEmpty
                                     If XmlSerializeToString(X.ErstelleXMLKontakt, NeuerKontakt) Then
                                         Return NeuerKontakt
                                     Else
                                         NLogger.Warn($"Der Kontakt {X.FullNameAndCompany} kann nicht serialisiert werden.")
                                         Return DfltStringEmpty
                                     End If
                                 End Function)
    End Function
End Module
