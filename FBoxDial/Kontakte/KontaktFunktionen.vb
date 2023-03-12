﻿Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.Office.Interop.Outlook
Imports System.Windows.Media
Friend Module KontaktFunktionen
    Private ReadOnly Property DfltErrorvalue As Integer = -2147221233
    Private ReadOnly Property DfltDASLSMTPAdress As String = "http://schemas.microsoft.com/mapi/proptag/0x39FE001E"
    Private ReadOnly Property DASLTagFBTelBuch As Object() = {$"{DfltDASLSchema}FBDB-PhonebookID", $"{DfltDASLSchema}FBDB-PhonebookEntryID"}.ToArray

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    ''' <summary>
    ''' Erstellt einen Kontakt aus einer vCard.
    ''' </summary>
    ''' <param name="vCard">Kontaktdaten im vCard-Format.</param>
    ''' <param name="TelNr">Telefonnummer, die zusätzlich eingetragen werden soll.</param>
    ''' <param name="AutoSave">Gibt an ob der Kontakt gespeichert werden soll True, oder nur angezeigt werden soll False.</param>
    ''' <returns>Den erstellten Kontakt als Outlook.ContactItem.</returns>
    Friend Function ErstelleKontakt(vCard As String, TelNr As Telefonnummer, AutoSave As Boolean) As ContactItem
        Dim olKontakt As ContactItem

        If Not TelNr.Unterdrückt Then

            olKontakt = CType(Globals.ThisAddIn.Application.CreateItem(OlItemType.olContactItem), ContactItem)

            With olKontakt

                If TelNr.IstMobilnummer Then
                    .MobileTelephoneNumber = TelNr.Formatiert
                Else
                    .BusinessTelephoneNumber = TelNr.Formatiert
                End If

                If vCard.IsNotStringNothingOrEmpty And vCard.IsNotEqual("-1") Then

                    DeserializevCard(vCard, olKontakt)

                    ' Formatiere Telefonnummer
                    If .BusinessTelephoneNumber.IsNotStringNothingOrEmpty Then
                        Using BTel As New Telefonnummer
                            BTel.SetNummer = .BusinessTelephoneNumber

                            If Not BTel.Equals(TelNr) Then
                                .Business2TelephoneNumber = BTel.Formatiert
                            End If
                        End Using
                        .BusinessTelephoneNumber = TelNr.Formatiert
                    ElseIf .HomeTelephoneNumber.IsNotStringNothingOrEmpty Then
                        Using HTel As New Telefonnummer
                            HTel.SetNummer = .HomeTelephoneNumber

                            If Not HTel.Equals(TelNr) Then
                                .Home2TelephoneNumber = HTel.Formatiert
                            End If
                        End Using
                        .BusinessTelephoneNumber = TelNr.Formatiert

                        .HomeTelephoneNumber = TelNr.Formatiert
                    ElseIf .HomeTelephoneNumber.IsStringNothingOrEmpty Then
                        .HomeTelephoneNumber = TelNr.Formatiert
                    End If

                    .Categories = My.Resources.strDefLongName 'Alle Kontakte, die erstellt werden, haben diese Kategorie. Damit sind sie einfach zu erkennen

                    If Not XMLData.POptionen.CBNoContactNotes Then
                        .Body = $"{String.Format(Localize.resCommon.strCreateContact, My.Resources.strDefLongName, Now)}{vbCrLf & vbCrLf}vCard:{vbCrLf & vbCrLf}{vCard}"
                    End If
                End If

            End With

            If AutoSave Then SpeichereKontakt(olKontakt)

            Return olKontakt
        Else
            Return Nothing
        End If

    End Function
    ''' <summary>
    ''' Erstellt einen Kontakt aus einer Fritz!Box Telefonbucheintrag.
    ''' </summary>
    ''' <param name="XMLKontakt">Kontaktdaten als Fritz!Box Telefonbucheintrag</param>
    ''' <param name="TelNr">Telefonnummer, die zusätzlich eingetragen werden soll.</param>
    ''' <param name="AutoSave">Gibt an ob der Kontakt gespeichert werden soll True, oder nur angezeigt werden soll False.</param>
    ''' <returns>Den erstellten Kontakt als Outlook.ContactItem.</returns>
    Friend Function ErstelleKontakt(XMLKontakt As FBoxAPI.Contact, TelNr As Telefonnummer, AutoSave As Boolean) As ContactItem

        If Not TelNr.Unterdrückt Then

            Dim olKontakt As ContactItem = CType(Globals.ThisAddIn.Application.CreateItem(OlItemType.olContactItem), ContactItem)

            With olKontakt

                If TelNr.IstMobilnummer Then
                    .MobileTelephoneNumber = TelNr.Formatiert
                Else
                    .BusinessTelephoneNumber = TelNr.Formatiert
                End If

                If XMLKontakt IsNot Nothing Then
                    XMLKontakt.XMLKontaktOutlook(olKontakt)

                    .Categories = My.Resources.strDefLongName ' 'Alle Kontakte, die erstellt werden, haben diese Kategorie. Damit sind sie einfach zu erkennen

                    If Not XMLData.POptionen.CBNoContactNotes Then
                        .Body = String.Format(Localize.resCommon.strCreateContact, My.Resources.strDefLongName, Now)
                    End If
                End If

            End With

            If AutoSave Then SpeichereKontakt(olKontakt)

            Return olKontakt

        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' Erstellt einen leeren Kontakt und ergänzt eine Telefonnummer.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, die eingefügt werden soll.</param>
    ''' <param name="Speichern">Gibt an ob der Kontakt gespeichert werden soll True, oder nur angezeigt werden soll False.</param>
    ''' <returns>Den erstellte Kontakt als Outlook.ContactItem.</returns>
    Friend Function ErstelleKontakt(TelNr As Telefonnummer, Speichern As Boolean) As ContactItem
        Return ErstelleKontakt(String.Empty, TelNr, Speichern)
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
            If Not KontaktOrdner.AreEqual(GetDefaultMAPIFolder(OlDefaultFolders.olFolderContacts)) Then
                ' Verschiebe den Kontakt in den gewünschten Ornder
                olKontakt = CType(olKontakt.Move(KontaktOrdner), ContactItem)
                NLogger.Info($"Kontakt {olKontakt.FullName} wurde erstellt und in den Ordner {KontaktOrdner.Name} verschoben.")

            Else
                ' Speichere den Kontakt im Kontakthauptordner
                ' Speichern ist überflüssig, da der Kontakt bein nachfolgenden Indizieren/deindizieren ohnehin stets gespeichert wird.

                'If olKontakt.Speichern Then NLogger.Info($"Kontakt {olKontakt.FullName} wurde Hauptkontaktordner gespeichert.")
            End If

            ' Indizere den Kontakt, wenn der Ordner, in den er gespeichert werden soll, auch zur Kontaktsuche verwendet werden soll
            IndiziereKontakt(olKontakt, KontaktOrdner, False)

        End With

    End Sub

    ''' <summary>
    ''' Erstellt einen Kontakt aus einem Outlook-Item (Journal oder Termin) oder blendet den verknüpften Kontakt ein.
    ''' </summary>
    Friend Sub ZeigeKontaktAusOutlookItem(Of T)(olItem As T)
        Dim vCard As String
        Dim olKontakt As ContactItem ' Objekt des Kontakteintrags
        Dim TelNr As Telefonnummer
        Dim Body As String
        Dim Categories As String

        Select Case True
            Case TypeOf olItem Is AppointmentItem
                With CType(olItem, AppointmentItem)
                    ' Outlook-Kontakt ermitteln
                    olKontakt = GetOutlookKontakt(CType(.PropertyAccessor.GetProperties(DASLTagOlItem), Object()))
                    ' Body entnehmen
                    Body = .Body
                    ' Categories entnehmen
                    Categories = .Categories
                End With

            Case TypeOf olItem Is JournalItem
                With CType(olItem, JournalItem)
                    ' Outlook-Kontakt ermitteln
                    olKontakt = GetOutlookKontakt(CType(.PropertyAccessor.GetProperties(DASLTagOlItem), Object()))
                    ' Body entnehmen
                    Body = .Body
                    ' Categories entnehmen
                    Categories = .Categories
                End With

            Case Else
                olKontakt = Nothing
                Body = String.Empty
                Categories = String.Empty
        End Select

        ReleaseComObject(olItem)

        If Categories.Contains(Localize.LocAnrMon.strJournalCatDefault) Then

            If olKontakt Is Nothing Then

                ' Telefonnummer aus dem .Body herausfiltern
                TelNr = New Telefonnummer With {.SetNummer = Body.GetSubString(Localize.LocAnrMon.strJournalBodyStart, "Status: ")}

                ' Prüfe ob TelNr unterdrückt
                If TelNr.Unterdrückt Then
                    olKontakt = ErstelleKontakt(TelNr, False)
                Else
                    ' Entweder erst eingebetteten Kontakt suchen, oder nach vCard suchen.
                    ' vCard aus dem .Body herausfiltern
                    vCard = $"BEGIN:VCARD{Body.GetSubString("BEGIN:VCARD", "END:VCARD")}END:VCARD"

                    'Wenn keine vCard im Body gefunden
                    If vCard.IsEqual($"BEGIN:VCARD-1END:VCARD") Then
                        ' wenn nicht, dann neuen Kontakt mit TelNr öffnen
                        olKontakt = ErstelleKontakt(TelNr, False)
                    Else
                        'vCard gefunden
                        olKontakt = ErstelleKontakt(vCard, TelNr, False)
                    End If
                End If
            End If
        End If

        If olKontakt IsNot Nothing Then olKontakt.Display()

    End Sub ' (ZeigeKontaktAusOutlookItem)

    ''' <summary>
    ''' Erstellt einen Kontakt aus einem Inspectorfenster (Journal oder Termin) oder blendet den verknüpften Kontakt ein.
    ''' </summary>
    Friend Sub ZeigeKontaktAusInspector(olInsp As Inspector)
        If olInsp IsNot Nothing Then
            Select Case True
                Case TypeOf olInsp.CurrentItem Is JournalItem Or
                     TypeOf olInsp.CurrentItem Is AppointmentItem

                    ZeigeKontaktAusOutlookItem(olInsp.CurrentItem)

            End Select
        End If
    End Sub ' (ZeigeKontaktAusInspector)

    ''' <summary>
    ''' Ermittelt aus der KontaktID (EntryID) und der StoreID den zugehörigen Kontakt.
    ''' </summary>
    ''' <param name="KontaktID">EntryID des Kontaktes</param>
    ''' <param name="StoreID">StoreID des beinhaltenden Ordners</param>
    ''' <returns>Erfolg: Kontakt, Misserfolg: Nothing</returns>
    Friend Function GetOutlookKontakt(ByRef KontaktID As String, ByRef StoreID As String) As ContactItem
        GetOutlookKontakt = Nothing
        Try
            GetOutlookKontakt = CType(Globals.ThisAddIn.Application.Session.GetItemFromID(KontaktID, StoreID), ContactItem)
        Catch ex As System.Exception
            NLogger.Error(ex)
        End Try
    End Function

    Friend Function GetOutlookKontakt(KontaktIDStoreID As Object()) As ContactItem
        GetOutlookKontakt = Nothing

        If Not KontaktIDStoreID.Contains(DfltErrorvalue) Then
            Try
                GetOutlookKontakt = CType(Globals.ThisAddIn.Application.Session.GetItemFromID(KontaktIDStoreID.First.ToString, KontaktIDStoreID.Last.ToString), ContactItem)
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
    ''' <returns>Erfolg: Ordner, Misserfolg: Nothing</returns>
    ''' <remarks>In Office 2003 ist Outlook.Folder unbekannt, daher Outlook.MAPIFolder</remarks>
    Friend Function GetOutlookFolder(FolderID As String, StoreID As String) As MAPIFolder
        GetOutlookFolder = Nothing

        If Globals.ThisAddIn.Application IsNot Nothing AndAlso Globals.ThisAddIn.Application.Session IsNot Nothing Then
            If FolderID.IsNotEqual("-1") And StoreID.IsNotEqual("-1") Then
                Try
                    ' Überprüfe, ob der Store vorhanden ist
                    Dim store = Globals.ThisAddIn.Application.Session.GetStoreFromID(StoreID)
                    ' Ermittle den Folder
                    GetOutlookFolder = Globals.ThisAddIn.Application.Session.GetFolderFromID(FolderID, StoreID)
                Catch ex As System.Exception
                    NLogger.Error(ex)
                End Try
            End If
        Else
            NLogger.Warn("Die Outlook Application bzw. die Session ist Nothing.")
        End If

    End Function

    ''' <summary>
    ''' Gibt ein Objekt vom Typ <see cref="MAPIFolder"/>, das den Standardordner des angeforderten <see cref="OlDefaultFolders"/> für das aktuelle Profil darstellt.
    ''' </summary>
    ''' <param name="FolderType">Der Typ des zurückzugebenden standardmäßigen Ordners.</param>
    ''' <returns>Ein Objekt vom Typ <see cref="MAPIFolder"/>, das den standardmäßigen Ordner des angeforderten Typs für das aktuelle Profil darstellt.</returns>
    Friend Function GetDefaultMAPIFolder(FolderType As OlDefaultFolders) As MAPIFolder
        Return Globals.ThisAddIn.Application.Session.GetDefaultFolder(FolderType)
    End Function

    Private Function GetOutlookChildFolders(BaseFolder As MAPIFolder, ItemType As OlItemType) As List(Of MAPIFolder)
        Dim ChildFolders As New List(Of MAPIFolder)
        ' Füge die direkten Childfolder hinzu
        For Each Folder As MAPIFolder In BaseFolder.Folders
            If Folder.DefaultItemType = ItemType Then ChildFolders.Add(Folder)

            ' Rekursiver Aufruf
            ChildFolders.AddRange(GetOutlookChildFolders(Folder, ItemType))
        Next

        Return ChildFolders
    End Function

    Friend Sub AddChildFolders(MAPIFolderList As List(Of MAPIFolder), ItemType As OlItemType)
        Dim MAPIFolderChildList As New List(Of MAPIFolder)

        For Each MapiFolder In MAPIFolderList
            MAPIFolderChildList.AddRange(GetOutlookChildFolders(MapiFolder, ItemType))
        Next
        MAPIFolderList.AddRange(MAPIFolderChildList)
    End Sub

    <Extension> Friend Function GetKontaktTelNrList(olContact As ContactItem, MitFax As Boolean) As List(Of Telefonnummer)

        Dim TelNrArray As List(Of Object) = olContact.GetTelNrArray.ToList
        Return TelNrArray.Where(Function(N) N IsNot Nothing AndAlso TelNrArray.IndexOf(N).IsLessOrEqual(If(MitFax, 13, 18))) _
                         .Select(Function(S) New Telefonnummer With {.SetNummer = S.ToString,
                                                                     .Typ = New TelNrType With {.TelNrType = CType(TelNrArray.IndexOf(S), OutlookNrType)}}).ToList

    End Function

    <Extension> Friend Function GetKontaktTelNrList(olExchangeNutzer As ExchangeUser) As List(Of Telefonnummer)

        Dim tmpTelNr(18) As Object
        With olExchangeNutzer
            tmpTelNr(1) = .BusinessTelephoneNumber
            tmpTelNr(9) = .MobileTelephoneNumber
        End With

        Dim TelNrArray As List(Of Object) = tmpTelNr.ToList
        Return TelNrArray.Where(Function(N) N IsNot Nothing) _
                         .Select(Function(S) New Telefonnummer With {.SetNummer = S.ToString,
                                                                     .Typ = New TelNrType With {.TelNrType = CType(TelNrArray.IndexOf(S), OutlookNrType)}}).ToList

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

    <Extension> Friend Function Speichern(olKontakt As ContactItem) As Boolean
        Try
            olKontakt.Save()
            Return True
        Catch ex As System.Exception
            NLogger.Error(ex, $"Kontakt {olKontakt.FullNameAndCompany} kann nicht gespeichert werden.")
            Return False
        End Try
    End Function

    <Extension> Friend Function ParentFolder(olKontakt As ContactItem) As MAPIFolder
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
        Return Ordner1 IsNot Nothing AndAlso Ordner2 IsNot Nothing AndAlso Ordner1.StoreID.IsEqual(Ordner2.StoreID) AndAlso Ordner1.EntryID.IsEqual(Ordner2.EntryID)
    End Function

#Region "VIP"
    <Extension> Friend Function IsVIP(olKontakt As ContactItem) As Boolean
        ' Prüfe, ob sich der Kontakt in der Liste befindet.
        If XMLData.PTelListen.VIPListe IsNot Nothing Then
            With XMLData.PTelListen.VIPListe
                Return .Exists(Function(VIPEintrag) VIPEintrag.EntryID.IsEqual(olKontakt.EntryID) And VIPEintrag.StoreID.IsEqual(olKontakt.StoreID))
            End With
        End If
        Return False
    End Function

    <Extension> Friend Function ToggleVIP(olKontakt As ContactItem) As Boolean
        If XMLData.PTelListen.VIPListe Is Nothing Then XMLData.PTelListen.VIPListe = New List(Of VIPEntry)

        If olKontakt.IsVIP Then
            ' Entferne den Kontakt von der Liste
            XMLData.PTelListen.VIPListe.RemoveAll(Function(VIPEintrag) VIPEintrag.EntryID.IsEqual(olKontakt.EntryID) And VIPEintrag.StoreID.IsEqual(olKontakt.StoreID))
        Else
            ' Füge einen neuen Eintrag hinzu
            XMLData.PTelListen.VIPListe.Add(New VIPEntry With {.Name = olKontakt.FullNameAndCompany, .EntryID = olKontakt.EntryID, .StoreID = olKontakt.StoreID})
        End If

        Globals.ThisAddIn.POutlookRibbons.RefreshRibbon()

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

        GetSenderSMTPAddress = New EMailType With {.Addresse = String.Empty}

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
                            ReleaseComObject(ExchangeUser)

                        Case Else
                            .Addresse = TryCast(Adresseintrag.PropertyAccessor.GetProperty(DfltDASLSMTPAdress), String)

                    End Select
                    ' COM Objekt freigeben
                    ReleaseComObject(Adresseintrag)
                Else

                    ' SMTP Adresse (klassische E-Mail)
                    .OutlookTyp = OutlookEMailType.SMTP

                    .Addresse = EMail.SenderEmailAddress
                End If
            End With

        End If

        Return GetSenderSMTPAddress
    End Function

    Friend Async Sub StartKontaktRWS(olContact As ContactItem, TelNr As Telefonnummer)

        Dim vCard As String

        vCard = Await StartRWS(TelNr, False)

        If vCard.IsStringNothingOrEmpty Then
            AddinMsgBox($"{Localize.LocAnrMon.strJournalRWSFehler} {TelNr.Formatiert}", MsgBoxStyle.Information, Localize.LocOptionen.strSearchContactHeadRWS)
        Else
            If Not XMLData.POptionen.CBNoContactNotes Then
                olContact.Body += String.Format($"{vbCrLf & vbCrLf}{Localize.LocAnrMon.strJournalTextvCard}{vCard}")
            End If

            DeserializevCard(vCard, olContact)
        End If

    End Sub

    ''' <summary>
    ''' Überführt einen Outlook-Kontakt in einen Kontakt für das Fritz!Box Telefondingsbums
    ''' </summary>
    ''' <param name="olContact">Der Outlook Kontakt, der überführt werden soll.</param>
    ''' <param name="UID">Falls bekannt, die Uniqueid des Kontaktes im Fritz!Box Telefonbuch.</param>
    ''' <returns></returns>
    <Extension> Friend Function ErstelleFBoxKontakt(olContact As ContactItem, Optional UID As Integer = -1) As FBoxAPI.Contact

        ' Erstelle ein nen neuen XMLKontakt
        Dim XMLKontakt As FBoxAPI.Contact = CreateContact(Localize.resCommon.strOhneName)

        With XMLKontakt
            ' Weise den Namen zu
            If olContact.FullName.IsStringNothingOrEmpty Then
                .Person.RealName = If(olContact.CompanyName.IsStringNothingOrEmpty, Localize.resCommon.strOhneName, olContact.CompanyName)
            Else
                .Person.RealName = olContact.FullName
            End If

            If UID.AreDifferentTo(-1) Then .Uniqueid = UID

            With .Telephony
                ' Weise die E-Mails zu
                With .Emails
                    If olContact.Email1Address.IsNotStringNothingOrEmpty Then
                        .Add(New FBoxAPI.Email With {.EMail = olContact.Email1Address})
                    End If
                    If olContact.Email2Address.IsNotStringNothingOrEmpty Then
                        .Add(New FBoxAPI.Email With {.EMail = olContact.Email2Address})
                    End If
                    If olContact.Email3Address.IsNotStringNothingOrEmpty Then
                        .Add(New FBoxAPI.Email With {.EMail = olContact.Email3Address})
                    End If
                End With

                ' Weise die Telefonnummern zu
                With .Numbers
                    For Each TelNr In GetKontaktTelNrList(olContact, True)
                        .Add(New FBoxAPI.NumberType With {.Number = TelNr.Unformatiert, .Type = TelNr.Typ.XML})
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
    <Extension> Friend Function ErstelleXMLKontakt(olContacts As ContactItem, Optional UID As Integer = -1) As String

        Dim NeuerKontakt As String = String.Empty
        If XmlSerializeToString(olContacts.ErstelleFBoxKontakt(UID), NeuerKontakt) Then
            Return NeuerKontakt
        Else
            NLogger.Warn($"Der Kontakt {olContacts.FullNameAndCompany} kann nicht serialisiert werden.")
            Return String.Empty
        End If

    End Function

    <Extension> Friend Function GetUniqueID(olContacts As ContactItem, TelefonbuchID As Integer) As Integer
        With olContacts

            ' Überprüfe, ob es in diesem Kontakt Daten zu einem Eintrag in einem Telefonbuch gibt
            Dim colArgs() As Object = CType(.PropertyAccessor.GetProperties(DASLTagFBTelBuch), Object())

            ' Wenn es keine Fehler gab (Einträge sind vorhanden) und die TelefonbuchID übereinstimmt. dann gib die ID zurück
            If Not colArgs.Contains(DfltErrorvalue) Then

                Dim i As Integer = Array.IndexOf(Split(colArgs(0).ToString, ","), TelefonbuchID.ToString)
                If i.AreEqual(-1) Then
                    Return -1
                Else
                    Return Split(colArgs(1).ToString, ",").ToList(i).ToInt
                End If

            Else
                Return -1
            End If
        End With
    End Function

    <Extension> Friend Sub SetUniqueID(olContacts As ContactItem, TelefonbuchID As String, UniqueID As String)
        With olContacts

            ' Ermittle alle bisherigen Verknüpfungen
            Dim colArgs() As Object = CType(.PropertyAccessor.GetProperties(DASLTagFBTelBuch), Object())
            Dim BookIDs As List(Of String)
            Dim UniqueIDs As List(Of String)

            If colArgs.Contains(DfltErrorvalue) Then
                BookIDs = New List(Of String)
                UniqueIDs = New List(Of String)
            Else
                BookIDs = Split(colArgs(0).ToString, ",").ToList
                UniqueIDs = Split(colArgs(1).ToString, ",").ToList
            End If

            Dim i As Integer = Array.IndexOf(Split(colArgs(0).ToString, ","), TelefonbuchID)

            If i.AreEqual(-1) Then
                ' Es gibt noch keinen Eintrag
                BookIDs.Add(TelefonbuchID)
                UniqueIDs.Add(UniqueID)
            Else
                ' Es gibt bereits einen Eintrag: Überschreibe den Wert
                UniqueIDs(i) = UniqueID
            End If

            ' Entferne etwaige vorhandene 
            .PropertyAccessor.DeleteProperties(DASLTagFBTelBuch)

            colArgs(0) = String.Join(",", BookIDs)
            colArgs(1) = String.Join(",", UniqueIDs)

            ' Verknüpfe Outlook-Kontakt mit dem Fritz!Box Telefonbucheintrag
            .PropertyAccessor.SetProperties(DASLTagFBTelBuch, colArgs)

            ' Speichere den Kontakt
            .Save()
        End With
    End Sub

#Region "Bilder"

    ''' <summary>
    ''' Speichert das Kontaktbild in den Arbeitsorder. 
    ''' </summary>
    ''' <param name="olContact">Kontakt, aus dem das Kontaktbild extrahiert werden soll.</param>
    ''' <returns>Pfad zum extrahierten Kontaktbild.</returns>
    <Extension> Friend Function KontaktBild(olContact As ContactItem) As String
        KontaktBild = String.Empty
        If olContact IsNot Nothing Then
            With olContact
                With .Attachments
                    If .Item("ContactPicture.jpg") IsNot Nothing Then
                        KontaktBild = $"{Path.GetTempPath}{Path.GetRandomFileName}" '.RegExReplace(".{3}$", "jpg")
                        .Item("ContactPicture.jpg").SaveAsFile(KontaktBild)

                        NLogger.Debug($"Bild des Kontaktes {olContact.FullName} unter Pfad {KontaktBild} gespeichert.")
                    End If
                End With
            End With
        End If
    End Function

    <Extension> Friend Function KontaktBildEx(olContact As ContactItem) As Imaging.BitmapImage
        If olContact IsNot Nothing Then
            With olContact
                With .Attachments
                    If .Item("ContactPicture.jpg") IsNot Nothing Then
                        ' Bild Speichern
                        Dim BildPfad As String = $"{Path.GetTempPath}{Path.GetRandomFileName}"
                        .Item("ContactPicture.jpg").SaveAsFile(BildPfad)
                        NLogger.Debug($"Bild des Kontaktes {olContact.FullName} unter Pfad {BildPfad} gespeichert.")

                        ' Bild in das Datenobjekt laden und abschließend löschen
                        Return KontaktBildEx(BildPfad)

                    End If
                End With
            End With
        End If
        Return Nothing
    End Function

    Friend Function KontaktBildEx(BildPfad As String) As Imaging.BitmapImage
        If BildPfad.IsNotStringNothingOrEmpty Then
            ' Bild in das Datenobjekt laden
            Dim biImg As New Imaging.BitmapImage
            With biImg
                .BeginInit()
                .CacheOption = Imaging.BitmapCacheOption.OnLoad
                .UriSource = New Uri(BildPfad)
                .EndInit()
            End With

            ' Bild wieder löschen
            LöscheDatei(BildPfad)

            Return biImg
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' Löscht das Kontaktbild in den Arbeitsorder. 
    ''' </summary>
    ''' <param name="DateiPfad">Pfad zum extrahierten Kontaktbild</param>
    Friend Sub LöscheDatei(DateiPfad As String)
        If DateiPfad.IsNotStringNothingOrEmpty Then
            With My.Computer.FileSystem
                If .FileExists(DateiPfad) Then
                    .DeleteFile(DateiPfad, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                    NLogger.Debug($"Datei {DateiPfad} gelöscht.")
                End If
            End With
        End If
    End Sub

#End Region
End Module
