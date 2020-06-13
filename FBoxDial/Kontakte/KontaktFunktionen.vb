Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.Office.Interop
Friend Module KontaktFunktionen
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend ReadOnly Property P_DefContactFolder() As Outlook.MAPIFolder = ThisAddIn.POutookApplication.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)

    ''' <summary>
    ''' Erstellt einen Kontakt aus einer vCard.
    ''' </summary>
    ''' <param name="KontaktID">Rückgabewert: KontaktID des neu erstellten Kontaktes.</param>
    ''' <param name="StoreID">Rückgabewert: StoreID des Ordners, in dem sich der neu erstellte Kontaktes befindet.</param>
    ''' <param name="vCard">Kontaktdaten im vCard-Format.</param>
    ''' <param name="TelNr">Telefonnummer, die zusätzlich eingetragen werden soll.</param>
    ''' <param name="AutoSave">Gibt an ob der Kontakt gespeichert werden soll True, oder nur angezeigt werden soll False.</param>
    ''' <returns>Den erstellte Kontakt als Outlook.ContactItem.</returns>
    Friend Function ErstelleKontakt(ByRef KontaktID As String, ByRef StoreID As String, ByVal vCard As String, ByVal TelNr As Telefonnummer, ByVal AutoSave As Boolean) As Outlook.ContactItem
        Dim olKontakt As Outlook.ContactItem


        If Not TelNr.Unbekannt Then

            olKontakt = CType(ThisAddIn.POutookApplication.CreateItem(Outlook.OlItemType.olContactItem), Outlook.ContactItem)

            With olKontakt

                If TelNr.IstMobilnummer Then
                    .MobileTelephoneNumber = TelNr.Formatiert
                Else
                    .BusinessTelephoneNumber = TelNr.Formatiert
                End If

                If vCard.IsNotStringEmpty And vCard.IsNotErrorString Then

                    Using vCrd As New VCard
                        vCrd.DeserializevCard(vCard, olKontakt)
                    End Using

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

                    .Categories = PDfltAddin_LangName '"Fritz!Box Telefon-dingsbums" 'Alle Kontakte, die erstellt werden, haben diese Kategorie. Damit sind sie einfach zu erkennen
                    .Body = .Body & vbCrLf & "Erstellt durch das " & PDfltAddin_LangName & " am " & Now & PDflt2NeueZeile & "vCard:" & PDflt2NeueZeile & vCard
                End If

            End With

            If AutoSave Then SpeichereKontakt(olKontakt)

            ErstelleKontakt = olKontakt
        Else
            Return Nothing
        End If

    End Function
    Friend Function ErstelleKontakt(ByRef KontaktID As String, ByRef StoreID As String, ByVal XMLKontakt As FritzBoxXMLKontakt, ByVal TelNr As Telefonnummer, ByVal AutoSave As Boolean) As Outlook.ContactItem
        Dim olKontakt As Outlook.ContactItem


        If Not TelNr.Unbekannt Then

            olKontakt = CType(ThisAddIn.POutookApplication.CreateItem(Outlook.OlItemType.olContactItem), Outlook.ContactItem)

            With olKontakt

                If TelNr.IstMobilnummer Then
                    .MobileTelephoneNumber = TelNr.Formatiert
                Else
                    .BusinessTelephoneNumber = TelNr.Formatiert
                End If

                If XMLKontakt IsNot Nothing Then
                    XMLKontaktOutlook(XMLKontakt, olKontakt)

                    .Categories = PDfltAddin_LangName '"Fritz!Box Telefon-dingsbums" 'Alle Kontakte, die erstellt werden, haben diese Kategorie. Damit sind sie einfach zu erkennen
                    .Body = .Body & vbCrLf & "Erstellt durch das " & PDfltAddin_LangName & " am " & Now
                End If

            End With

            If AutoSave Then SpeichereKontakt(olKontakt)

            Return olKontakt
        Else
            Return Nothing
        End If
    End Function

    Private Sub SpeichereKontakt(ByRef olKontakt As Outlook.ContactItem)

        ' Ermittle den Ordner in den der Kontakt gespeichet werden soll
        Dim KontaktOrdner As OutlookOrdner = XMLData.POptionen.OutlookOrdner.Find(OutlookOrdnerVerwendung.KontaktSpeichern)

        ' Speichere den Kontakt... (Wenn es sich nicht um den Hauptkontaktordner handelt, der Kontakt ist da breits (ungespeichert) enthalten. Ein Move würde den Kontakt dublizieren.)
        If KontaktOrdner IsNot Nothing AndAlso KontaktOrdner.MAPIFolder IsNot Nothing And ThisAddIn.POutookApplication.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts).AreNotEqual(KontaktOrdner.MAPIFolder) Then
            ' ... im festgelegten Ordner
            olKontakt = CType(olKontakt.Move(KontaktOrdner.MAPIFolder), Outlook.ContactItem)
            NLogger.Info("Kontakt {0} wurde erstellt und in den Ordner {1} verschoben.", olKontakt.FullName, KontaktOrdner.MAPIFolder.Name)
        Else
            ' ... im Kontakthauptordner
            If olKontakt.Speichern Then NLogger.Info("Kontakt {0} wurde Hauptkontaktordner gespeichert.", olKontakt.FullName)
        End If

        ' Indiziere den Kontakt, falls dieser nicht eingeblendet ist
        'If olKontakt.GetInspector Is Nothing Then
        ' Indizere den Kontakt, wenn der Ordner, in den er gespeichert werden soll, auch zur Kontaktsuche verwendet werden soll
        If XMLData.POptionen.OutlookOrdner.Exists(KontaktOrdner.MAPIFolder, OutlookOrdnerVerwendung.KontaktSuche) Then IndiziereKontakt(olKontakt)

        'End If

    End Sub

    ''' <summary>
    ''' Erstellt einen leeren Kontakt und ergänzt eine Telefonnummer.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, die eingefügt werden soll.</param>
    ''' <param name="Speichern">Gibt an ob der Kontakt gespeichert werden soll True, oder nur angezeigt werden soll False.</param>
    ''' <returns>Den erstellte Kontakt als Outlook.ContactItem.</returns>
    Friend Function ErstelleKontakt(ByVal TelNr As Telefonnummer, ByVal Speichern As Boolean) As Outlook.ContactItem
        Return ErstelleKontakt(PDfltStringEmpty, PDfltStringEmpty, PDfltStringEmpty, TelNr, Speichern)
    End Function

    ''' <summary>
    ''' Erstellt einen Kontakt aus einem Inspectorfenster (Journal)
    ''' </summary>
    Friend Sub ZeigeKontaktAusJournal(ByVal olJournal As Outlook.JournalItem)
        Dim vCard As String
        Dim olKontakt As Outlook.ContactItem = Nothing ' Objekt des Kontakteintrags
        Dim TelNr As Telefonnummer

        With olJournal
            If .Categories.Contains(PDfltJournalKategorie) Then

                olKontakt = GetOutlookKontakt(CType(.PropertyAccessor.GetProperties(DASLTagJournal), Object()))

                If olKontakt Is Nothing Then

                    TelNr = New Telefonnummer
                    'Telefonnummer aus dem .Body herausfiltern
                    TelNr.SetNummer = .Body.GetSubString(PDfltJournalBodyStart, "Status: ")

                    ' Prüfe ob TelNr unterdrückt
                    If TelNr.Unbekannt Then
                        olKontakt = ErstelleKontakt(TelNr, False)
                    Else
                        ' Entweder erst eingebetteten Kontakt suchen, oder nach vCard suchen.
                        ' vCard aus dem .Body herausfiltern
                        vCard = PDfltBegin_vCard & .Body.GetSubString(PDfltBegin_vCard, PDfltEnd_vCard) & PDfltEnd_vCard

                        'Wenn keine vCard im Body gefunden
                        If vCard.AreEqual(PDfltBegin_vCard & PDfltStrErrorMinusOne & PDfltEnd_vCard) Then
                            ' wenn nicht, dann neuen Kontakt mit TelNr öffnen
                            olKontakt = ErstelleKontakt(TelNr, False)
                        Else
                            'vCard gefunden
                            olKontakt = ErstelleKontakt(PDfltStringEmpty, PDfltStringEmpty, vCard, TelNr, False)
                        End If
                    End If
                End If
            End If
        End With
        If olKontakt IsNot Nothing Then olKontakt.Display()
        ReleaseComObject(olJournal)

    End Sub ' (ZeigeKontaktAusJournal)

    Friend Sub ZeigeKontaktAusInspector(ByVal olInsp As Outlook.Inspector)
        If olInsp IsNot Nothing Then
            If TypeOf olInsp.CurrentItem Is Outlook.JournalItem Then
                ZeigeKontaktAusJournal(CType(olInsp.CurrentItem, Outlook.JournalItem))
            End If
        End If
    End Sub ' (ZeigeKontaktAusInspector)

    Friend Sub ZeigeKontaktAusSelection(ByVal olSelection As Outlook.Selection)
        If olSelection IsNot Nothing Then

            If TypeOf olSelection.Item(1) Is Outlook.JournalItem Then
                ZeigeKontaktAusJournal(CType(olSelection.Item(1), Outlook.JournalItem))
            End If
        End If
    End Sub ' (ZeigeKontaktAusSelection)

    ''' <summary>
    ''' Speichert das Kontaktbild in den Arbeitsorder. 
    ''' </summary>
    ''' <param name="olContact">Kontakt, aus dem das Kontaktbild extrahiert werden soll.</param>
    ''' <returns>Pfad zum extrahierten Kontaktbild.</returns>
    Friend Function KontaktBild(ByRef olContact As Outlook.ContactItem) As String
        KontaktBild = PDfltStringEmpty
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
    Friend Sub DelKontaktBild(ByVal PfadKontaktBild As String)
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
    Friend Function GetOutlookKontakt(ByRef KontaktID As String, ByRef StoreID As String) As Outlook.ContactItem
        GetOutlookKontakt = Nothing
        Try
            GetOutlookKontakt = CType(ThisAddIn.POutookApplication.Session.GetItemFromID(KontaktID, StoreID), Outlook.ContactItem)
        Catch ex As Exception
            NLogger.Error(ex)
        End Try
    End Function

    Friend Function GetOutlookKontakt(ByRef KontaktIDStoreID As Object()) As Outlook.ContactItem
        GetOutlookKontakt = Nothing

        If Not KontaktIDStoreID.Contains(DfltErrorvalue) Then
            Try
                GetOutlookKontakt = CType(ThisAddIn.POutookApplication.Session.GetItemFromID(KontaktIDStoreID.First.ToString, KontaktIDStoreID.Last.ToString), Outlook.ContactItem)
            Catch ex As Exception
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
    Friend Function GetOutlookFolder(ByRef FolderID As String, ByRef StoreID As String) As Outlook.MAPIFolder
        GetOutlookFolder = Nothing

        If FolderID.IsNotErrorString And StoreID.IsNotErrorString Then
            Try
                GetOutlookFolder = ThisAddIn.POutookApplication.Session.GetFolderFromID(FolderID, StoreID)
            Catch ex As Exception
                NLogger.Error(ex)
            End Try
        End If

        'If GetOutlookFolder Is Nothing Then
        '    GetOutlookFolder = P_DefContactFolder
        '    FolderID = GetOutlookFolder.EntryID
        '    StoreID = CType(GetOutlookFolder.Parent, Outlook.MAPIFolder).StoreID
        '    XMLData.POptionen.PTVKontaktOrdnerEntryID = FolderID
        '    XMLData.POptionen.PTVKontaktOrdnerStoreID = StoreID
        'End If
    End Function
    Friend Function GetKontaktTelNrList(ByRef olContact As Outlook.ContactItem) As List(Of Telefonnummer)

        Dim alleTelNr(14) As String ' alle im Kontakt enthaltenen Telefonnummern
        Dim alleNrTypen(14) As String ' die Bezeichnungen der Telefonnummern
        Dim tmpTelNr As Telefonnummer

        With olContact
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
        End With

        GetKontaktTelNrList = New List(Of Telefonnummer)
        For i = LBound(alleTelNr) To UBound(alleTelNr)
            If alleTelNr(i).IsNotStringNothingOrEmpty Then
                tmpTelNr = New Telefonnummer With {.SetNummer = alleTelNr(i), .OutlookTyp = alleNrTypen(i)}
                GetKontaktTelNrList.Add(tmpTelNr)
            End If
        Next
    End Function

    Friend Function GetKontaktTelNrList(ByRef olExchangeNutzer As Outlook.ExchangeUser) As List(Of Telefonnummer)

        Dim alleTelNr(2) As String ' alle im Exchangenutzer enthaltenen Telefonnummern
        Dim alleNrTypen(2) As String ' die Bezeichnungen der Telefonnummern
        Dim tmpTelNr As Telefonnummer

        With olExchangeNutzer
            alleTelNr(1) = .BusinessTelephoneNumber : alleNrTypen(1) = "Geschäftlich"
            alleTelNr(2) = .MobileTelephoneNumber : alleNrTypen(2) = "Mobiltelefon"
        End With

        GetKontaktTelNrList = New List(Of Telefonnummer)
        For i = LBound(alleTelNr) To UBound(alleTelNr)
            If alleTelNr(i).IsNotStringNothingOrEmpty Then
                tmpTelNr = New Telefonnummer With {.SetNummer = alleTelNr(i), .OutlookTyp = alleNrTypen(i)}
                GetKontaktTelNrList.Add(tmpTelNr)
            End If
        Next
    End Function

    Friend Function ZähleOutlookKontakte(ByVal olFolder As Outlook.MAPIFolder) As Integer
        Dim retval As Integer = 0

        ' Die Anzahl der Elemente dieses Ordners zählen
        If olFolder.DefaultItemType = Outlook.OlItemType.olContactItem Then
            retval = olFolder.Items.Count

            ' Unterordner werden rekursiv mitgezählt
            If XMLData.POptionen.PCBSucheUnterordner Then
                For Each Unterordner As Outlook.MAPIFolder In olFolder.Folders
                    retval += ZähleOutlookKontakte(Unterordner)
                    Unterordner.ReleaseComObject
                Next
                'olFolder.ReleaseComObject
            End If
        End If
        Return retval
    End Function

    <Extension> Friend Function StoreID(ByVal olKontakt As Outlook.ContactItem) As String
        Return CType(olKontakt.Parent, Outlook.MAPIFolder).StoreID
    End Function

    <Extension> Friend Function GetTelNrArray(ByVal olContact As Outlook.ContactItem) As Object()

        Dim tmpTelNr(18) As Object
        With olContact
            tmpTelNr(0) = .AssistantTelephoneNumber     ' "urn:schemas:contacts:secretaryphone" 
            tmpTelNr(1) = .BusinessTelephoneNumber      ' "urn:schemas:contacts:officetelephonenumber" 
            tmpTelNr(2) = .Business2TelephoneNumber     ' "urn:schemas:contacts:office2telephonenumber" 
            tmpTelNr(3) = .CallbackTelephoneNumber      ' "urn:schemas:contacts:callbackphone" 
            tmpTelNr(4) = .CarTelephoneNumber           ' "urn:schemas:contacts:othermobile" 
            tmpTelNr(5) = .CompanyMainTelephoneNumber   ' "urn:schemas:contacts:organizationmainphone" 
            tmpTelNr(6) = .HomeTelephoneNumber          ' "urn:schemas:contacts:homePhone" 
            tmpTelNr(7) = .Home2TelephoneNumber         ' "urn:schemas:contacts:homePhone2" 
            tmpTelNr(8) = .ISDNNumber                   ' "urn:schemas:contacts:internationalisdnnumber" 
            tmpTelNr(9) = .MobileTelephoneNumber        ' "http://schemas.microsoft.com/mapi/proptag/0x3a1c001f" 
            tmpTelNr(10) = .OtherTelephoneNumber        ' "urn:schemas:contacts:otherTelephone" ' 
            tmpTelNr(11) = .PagerNumber                 ' "urn:schemas:contacts:pager" ' 
            tmpTelNr(12) = .PrimaryTelephoneNumber      ' "http://schemas.microsoft.com/mapi/proptag/0x3a1a001f" 
            tmpTelNr(13) = .RadioTelephoneNumber        ' "http://schemas.microsoft.com/mapi/proptag/0x3a1d001f" 
            tmpTelNr(14) = .BusinessFaxNumber           ' "urn:schemas:contacts:facsimiletelephonenumber" 
            tmpTelNr(15) = .HomeFaxNumber               ' "urn:schemas:contacts:homefax" ' 
            tmpTelNr(16) = .OtherFaxNumber              ' "urn:schemas:contacts:otherfax" ' 
            tmpTelNr(17) = .TelexNumber                 ' "urn:schemas:contacts:telexnumber" ' 
            tmpTelNr(18) = .TTYTDDTelephoneNumber       ' "urn:schemas:contacts:ttytddphone" ' 
        End With
        Return tmpTelNr

    End Function

    <Extension> Friend Function Speichern(ByRef olKontakt As Outlook.ContactItem) As Boolean
        Try
            olKontakt.Save()
            Return True
        Catch ex As Exception
            NLogger.Error(ex, "Kontakt {0} kann nicht gespeichert werden.", olKontakt.FullNameAndCompany)
            Return False
        End Try
    End Function

    <Extension> Friend Function ParentFolder(ByRef olKontakt As Outlook.ContactItem) As Outlook.MAPIFolder
        If olKontakt.Parent IsNot Nothing Then
            Return CType(olKontakt.Parent, Outlook.MAPIFolder)
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
    <Extension> Friend Function AreEqual(ByVal Ordner1 As Outlook.MAPIFolder, ByVal Ordner2 As Outlook.MAPIFolder) As Boolean
        Return Ordner1.StoreID.AreEqual(Ordner2.StoreID) And Ordner1.EntryID.AreEqual(Ordner2.EntryID)
    End Function
    <Extension> Friend Function AreNotEqual(ByVal Ordner1 As Outlook.MAPIFolder, ByVal Ordner2 As Outlook.MAPIFolder) As Boolean
        Return Ordner1.StoreID.AreNotEqual(Ordner2.StoreID) Or Ordner1.EntryID.AreNotEqual(Ordner2.EntryID)
    End Function
#Region "VIP"
    <Extension> Friend Function IsVIP(ByVal olKontakt As Outlook.ContactItem) As Boolean

        IsVIP = False
        ' Prüfe, ob sich der Kontakt in der Liste befindet.
        If XMLData.PTelefonie.VIPListe IsNot Nothing Then
            With XMLData.PTelefonie.VIPListe
                IsVIP = .Exists(Function(VIPEintrag) VIPEintrag.EntryID.AreEqual(olKontakt.EntryID) And VIPEintrag.StoreID.AreEqual(olKontakt.StoreID))
            End With
        End If
    End Function

    <Extension> Friend Sub AddVIP(ByVal olKontakt As Outlook.ContactItem)
        If XMLData.PTelefonie.VIPListe Is Nothing Then XMLData.PTelefonie.VIPListe = New List(Of VIPEntry)

        With XMLData.PTelefonie.VIPListe
            .Add(New VIPEntry With {.Name = olKontakt.FullNameAndCompany, .EntryID = olKontakt.EntryID, .StoreID = olKontakt.StoreID})
        End With
    End Sub

    <Extension> Friend Sub RemoveVIP(ByVal olKontakt As Outlook.ContactItem)
        Dim tmpVIPEntry As VIPEntry

        If XMLData.PTelefonie.VIPListe Is Nothing Then XMLData.PTelefonie.VIPListe = New List(Of VIPEntry)

        With XMLData.PTelefonie.VIPListe
            tmpVIPEntry = .Find(Function(VIPEintrag) VIPEintrag.EntryID.AreEqual(olKontakt.EntryID) And VIPEintrag.StoreID.AreEqual(olKontakt.StoreID))

            If tmpVIPEntry IsNot Nothing Then .Remove(tmpVIPEntry)
        End With
    End Sub

#End Region
    Private Sub XMLKontaktOutlook(ByVal XMLKontakt As FritzBoxXMLKontakt, ByRef Kontakt As Outlook.ContactItem)
        ' Werte übeführen
        With Kontakt
            ' Name
            .FullName = XMLKontakt.Person.RealName
            ' E-Mail Adressen (Es gibt in Outlook maximal 3 E-Mail Adressen)
            For i = 1 To Math.Min(XMLKontakt.Telefonie.Dienste.Emails.Count, 3)
                Select Case i
                    Case 1
                        .Email1Address = XMLKontakt.Telefonie.Dienste.Emails.Item(i - 1).EMail
                    Case 2
                        .Email2Address = XMLKontakt.Telefonie.Dienste.Emails.Item(i - 1).EMail
                    Case 3
                        .Email3Address = XMLKontakt.Telefonie.Dienste.Emails.Item(i - 1).EMail
                End Select
            Next
            ' Telefonnummern
            For Each TelNr As FritzBoxXMLNummer In XMLKontakt.Telefonie.Nummern
                Using tmpTelNr As New Telefonnummer With {.SetNummer = TelNr.Nummer}
                    ' Zuordnung zu den Kategorien                    
                    ' Type = "home":    .CarTelephoneNumber, .HomeTelephoneNumber, .Home2TelephoneNumber, .ISDNNumber, .TTYTDDTelephoneNumber, .OtherTelephoneNumber                           
                    ' Type = "mobile":  .MobileTelephoneNumber, .PagerNumber, .RadioTelephoneNumber
                    ' Type = "work":    .AssistantTelephoneNumber, .BusinessTelephoneNumber, .Business2TelephoneNumber, .CallbackTelephoneNumber, .CompanyMainTelephoneNumber, .PrimaryTelephoneNumber
                    ' Type = "fax_work: .BusinessFaxNumber, .HomeFaxNumber, .OtherFaxNumber, .TelexNumber
                    Select Case TelNr.Typ
                        Case "home"
                            If .HomeTelephoneNumber.IsStringNothingOrEmpty Then
                                .HomeTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .Home2TelephoneNumber.IsStringNothingOrEmpty Then
                                .Home2TelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .CarTelephoneNumber.IsStringNothingOrEmpty Then
                                .CarTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .OtherTelephoneNumber.IsStringNothingOrEmpty Then
                                .OtherTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .ISDNNumber.IsStringNothingOrEmpty Then
                                .ISDNNumber = tmpTelNr.Formatiert
                            ElseIf .TTYTDDTelephoneNumber.IsStringNothingOrEmpty Then
                                .TTYTDDTelephoneNumber = tmpTelNr.Formatiert
                            End If
                        Case "mobile"
                            If .MobileTelephoneNumber.IsStringNothingOrEmpty Then
                                .MobileTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .PagerNumber.IsStringNothingOrEmpty Then
                                .PagerNumber = tmpTelNr.Formatiert
                            ElseIf .RadioTelephoneNumber.IsStringNothingOrEmpty Then
                                .RadioTelephoneNumber = tmpTelNr.Formatiert
                            End If
                        Case "work"
                            If .BusinessTelephoneNumber.IsStringNothingOrEmpty Then
                                .BusinessTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .Business2TelephoneNumber.IsStringNothingOrEmpty Then
                                .Business2TelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .AssistantTelephoneNumber.IsStringNothingOrEmpty Then
                                .AssistantTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .CallbackTelephoneNumber.IsStringNothingOrEmpty Then
                                .CallbackTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .CompanyMainTelephoneNumber.IsStringNothingOrEmpty Then
                                .CompanyMainTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .PrimaryTelephoneNumber.IsStringNothingOrEmpty Then
                                .PrimaryTelephoneNumber = tmpTelNr.Formatiert
                            End If
                        Case "fax_work"
                            If .BusinessFaxNumber.IsStringNothingOrEmpty Then
                                .BusinessFaxNumber = tmpTelNr.Formatiert
                            ElseIf .HomeFaxNumber.IsStringNothingOrEmpty Then
                                .HomeFaxNumber = tmpTelNr.Formatiert
                            ElseIf .OtherFaxNumber.IsStringNothingOrEmpty Then
                                .OtherFaxNumber = tmpTelNr.Formatiert
                            ElseIf .TelexNumber.IsStringNothingOrEmpty Then
                                .TelexNumber = tmpTelNr.Formatiert
                            End If
                    End Select
                End Using
            Next
            ' Body
            Dim mySerializer As New XmlSerializer(GetType(FritzBoxXMLKontakt))
            Dim settings As New XmlWriterSettings With {.Indent = True, .OmitXmlDeclaration = False}
            Dim XmlSerializerNamespace As New XmlSerializerNamespaces()

            XmlSerializerNamespace.Add(PDfltStringEmpty, PDfltStringEmpty)

            Using TextSchreiber As New StringWriter
                mySerializer.Serialize(TextSchreiber, XMLKontakt, XmlSerializerNamespace)
                .Body = TextSchreiber.ToString()
            End Using
        End With
    End Sub

    ''' <summary>
    ''' Gibt die Absender-SMTP-Adresse der E-Mail zurück
    ''' </summary>
    ''' <param name="EMail"></param>
    ''' <remarks>https://docs.microsoft.com/de-de/office/client-developer/outlook/pia/how-to-get-the-smtp-address-of-the-sender-of-a-mail-item</remarks>
    ''' <returns></returns>
    Friend Function GetSenderSMTPAddress(ByVal EMail As Outlook.MailItem) As String

        If EMail IsNot Nothing Then
            If EMail.SenderEmailType = "EX" Then
                Dim Adresseintrag As Outlook.AddressEntry = EMail.Sender

                Select Case Adresseintrag?.AddressEntryUserType
                    Case Outlook.OlAddressEntryUserType.olExchangeUserAddressEntry, Outlook.OlAddressEntryUserType.olExchangeRemoteUserAddressEntry
                        Dim ExchangeUser As Outlook.ExchangeUser = Adresseintrag.GetExchangeUser()

                        If ExchangeUser IsNot Nothing Then
                            Return ExchangeUser.PrimarySmtpAddress
                        Else
                            Return PDfltStringEmpty
                        End If
                        ExchangeUser.ReleaseComObject

                    Case Else
                        Return TryCast(Adresseintrag.PropertyAccessor.GetProperty(PDfltDASLSMTPAdress), String)

                End Select

                Adresseintrag.ReleaseComObject
            Else
                Return EMail.SenderEmailAddress
            End If
        Else
            Return PDfltStringEmpty
        End If
    End Function

End Module
