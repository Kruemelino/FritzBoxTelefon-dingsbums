Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop
Friend Module KontaktFunktionen
    Private Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
    Friend ReadOnly Property P_DefContactFolder() As Outlook.MAPIFolder = ThisAddIn.POutookApplication.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)

    ''' <summary>
    ''' Erstellt einen Kontakt aus einer vCard.
    ''' </summary>
    ''' <param name="KontaktID">Rückgabewert: KontaktID des neu erstellten Kontaktes.</param>
    ''' <param name="StoreID">Rückgabewert: StoreID des Ordners, in dem sich der neu erstellte Kontaktes befindet.</param>
    ''' <param name="vCard">Kontaktdaten im vCard-Format.</param>
    ''' <param name="TelNr">Telefonnummer, die zusätzlich eingetragen werden soll.</param>
    ''' <param name="AutoSave">Gibt an ob der Kontakt gespeichert werden soll <c>True</c>, oder nur angezeigt werden soll <c>False</c>.</param>
    ''' <returns>Den erstellte Kontakt als <c>Outlook.ContactItem.</c></returns>
    Friend Function ErstelleKontakt(ByRef KontaktID As String, ByRef StoreID As String, ByVal vCard As String, ByVal TelNr As Telefonnummer, ByVal AutoSave As Boolean) As Outlook.ContactItem
        Dim olKontakt As Outlook.ContactItem
        Dim olFolder As Outlook.MAPIFolder

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

            If AutoSave Then
                If olKontakt.GetInspector Is Nothing Then
                    Using KontakIndexer As New KontaktIndizierer
                        KontakIndexer.IndiziereKontakt(olKontakt)
                    End Using
                End If
                ' Todo 1: Prüfe, ob ein Ordner ausgewählt wurde (Properties sind nicht -1)
                ' Todo 2: Prüfe, ob Ordner aus 1 nicht der default Ordner ist.

                'Handlung 1:

                If XMLData.POptionen.PTVKontaktOrdnerEntryID.IsNotStringEmpty And XMLData.POptionen.PTVKontaktOrdnerStoreID.IsNotStringEmpty Then
                    olFolder = GetOutlookFolder(XMLData.POptionen.PTVKontaktOrdnerEntryID, XMLData.POptionen.PTVKontaktOrdnerStoreID)
                    ' Handlung 2:
                    If olFolder.EntryID = P_DefContactFolder.EntryID And olFolder.StoreID = P_DefContactFolder.StoreID Then
                        olKontakt.Save()
                        NLogger.Info("Kontakt {0} wurde Hauptkontaktordner gespeichert.", olKontakt.FullName)
                    Else
                        olKontakt = CType(olKontakt.Move(olFolder), Outlook.ContactItem)
                        NLogger.Info("Kontakt {0} wurde erstellt und in den Ordner {1} verschoben.", olKontakt.FullName, olFolder.Name)
                    End If

                    KontaktID = olKontakt.EntryID
                    StoreID = olFolder.StoreID
                    olFolder.ReleaseComObject
                End If
            Else
                olKontakt.UserProperties.Add(PDfltUserPropertyIndex, Outlook.OlUserPropertyType.olText, False).Value = "False"
            End If
            ErstelleKontakt = olKontakt
        Else
            Return Nothing
        End If

    End Function
    Friend Function ErstelleKontakt(ByRef KontaktID As String, ByRef StoreID As String, ByVal XMLKontakt As FritzBoxXMLKontakt, ByVal TelNr As Telefonnummer, ByVal AutoSave As Boolean) As Outlook.ContactItem
        Dim olKontakt As Outlook.ContactItem
        Dim olFolder As Outlook.MAPIFolder

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

            If AutoSave Then
                If olKontakt.GetInspector Is Nothing Then
                    Using KontakIndexer As New KontaktIndizierer
                        KontakIndexer.IndiziereKontakt(olKontakt)
                    End Using
                End If
                ' Todo 1: Prüfe, ob ein Ordner ausgewählt wurde (Properties sind nicht -1)
                ' Todo 2: Prüfe, ob Ordner aus 1 nicht der default Ordner ist.

                'Handlung 1:

                If XMLData.POptionen.PTVKontaktOrdnerEntryID.IsNotStringEmpty And XMLData.POptionen.PTVKontaktOrdnerStoreID.IsNotStringEmpty Then
                    olFolder = GetOutlookFolder(XMLData.POptionen.PTVKontaktOrdnerEntryID, XMLData.POptionen.PTVKontaktOrdnerStoreID)
                    ' Handlung 2:
                    If olFolder.EntryID = P_DefContactFolder.EntryID And olFolder.StoreID = P_DefContactFolder.StoreID Then
                        olKontakt.Save()
                        NLogger.Info("Kontakt {0} wurde Hauptkontaktordner gespeichert.", olKontakt.FullName)
                    Else
                        olKontakt = CType(olKontakt.Move(olFolder), Outlook.ContactItem)
                        NLogger.Info("Kontakt {0} wurde erstellt und in den Ordner {1} verschoben.", olKontakt.FullName, olFolder.Name)
                    End If

                    KontaktID = olKontakt.EntryID
                    StoreID = olFolder.StoreID
                    ReleaseComObject(olFolder)
                End If
            Else
                olKontakt.UserProperties.Add(PDfltUserPropertyIndex, Outlook.OlUserPropertyType.olText, False).Value = "False"
            End If
            ErstelleKontakt = olKontakt
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Erstellt einen leeren Kontakt und ergänzt eine Telefonnummer.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, die eingefügt werden soll.</param>
    ''' <param name="Speichern">Gibt an ob der Kontakt gespeichert werden soll <c>True</c>, oder nur angezeigt werden soll <c>False</c>.</param>
    ''' <returns>Den erstellte Kontakt als <c>Outlook.ContactItem.</c></returns>
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
                        KontaktBild = IO.Path.GetTempPath() & IO.Path.GetRandomFileName()
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
                GetOutlookFolder = CType(ThisAddIn.POutookApplication.Session.GetFolderFromID(FolderID, StoreID), Outlook.MAPIFolder)
            Catch ex As Exception
                NLogger.Error(ex)
            End Try
        End If

        If GetOutlookFolder Is Nothing Then
            GetOutlookFolder = P_DefContactFolder
            FolderID = GetOutlookFolder.EntryID
            StoreID = CType(GetOutlookFolder.Parent, Outlook.MAPIFolder).StoreID
            XMLData.POptionen.PTVKontaktOrdnerEntryID = FolderID
            XMLData.POptionen.PTVKontaktOrdnerStoreID = StoreID
        End If
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
    Friend Async Function ZähleOutlookKontakte() As Threading.Tasks.Task(Of Integer)
        Dim olNamespace As Outlook.NameSpace ' MAPI-Namespace
        Dim olfolder As Outlook.MAPIFolder
        Dim retval As Integer
        olNamespace = ThisAddIn.POutookApplication.GetNamespace("MAPI")

        If XMLData.POptionen.PCBKontaktSucheHauptOrdner Then
            olfolder = P_DefContactFolder
            retval = Await ZähleOutlookKontakte(olfolder, Nothing)
        Else
            retval = Await ZähleOutlookKontakte(Nothing, olNamespace)
        End If
        Return retval
    End Function
    Private Async Function ZähleOutlookKontakte(ByVal Ordner As Outlook.MAPIFolder, ByVal NamensRaum As Outlook.NameSpace) As Threading.Tasks.Task(Of Integer)
        Dim tmpAnzahl As Integer = 0
        ' Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        If NamensRaum IsNot Nothing Then
            For Each olFolder As Outlook.MAPIFolder In NamensRaum.Folders
                tmpAnzahl += Await ZähleOutlookKontakte(olFolder, Nothing)
            Next
        Else
            If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then
                tmpAnzahl += Ordner.Items.Count
            End If

            ' Unterordner werden rekursiv durchsucht
            For Each olFolder As Outlook.MAPIFolder In Ordner.Folders
                tmpAnzahl += Await ZähleOutlookKontakte(olFolder, Nothing)
            Next
        End If

        Return tmpAnzahl
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

#Region "VIP"
    <Extension> Friend Function IsVIP(ByVal olKontakt As Outlook.ContactItem) As Boolean

        IsVIP = False
        ' Prüfe, ob sich der Kontakt in der Liste befindet.
        If XMLData.PTelefonie.VIPListe IsNot Nothing Then
            With XMLData.PTelefonie.VIPListe
                If .Einträge IsNot Nothing AndAlso .Einträge.Any Then
                    IsVIP = .Einträge.Exists(Function(VIPEintrag) VIPEintrag.EntryID.AreEqual(olKontakt.EntryID) And VIPEintrag.StoreID.AreEqual(olKontakt.StoreID))
                End If
            End With
        End If
    End Function

    <Extension> Friend Sub AddVIP(ByVal olKontakt As Outlook.ContactItem)
        If XMLData.PTelefonie.VIPListe Is Nothing Then XMLData.PTelefonie.VIPListe = New XVIP
        With XMLData.PTelefonie.VIPListe
            If .Einträge Is Nothing Then .Einträge = New List(Of VIPEntry)

            .Einträge.Add(New VIPEntry With {.Name = olKontakt.FullNameAndCompany, .EntryID = olKontakt.EntryID, .StoreID = olKontakt.StoreID})
        End With
    End Sub

    <Extension> Friend Sub RemoveVIP(ByVal olKontakt As Outlook.ContactItem)
        Dim tmpVIPEntry As VIPEntry

        If XMLData.PTelefonie.VIPListe Is Nothing Then XMLData.PTelefonie.VIPListe = New XVIP
        With XMLData.PTelefonie.VIPListe
            If .Einträge Is Nothing Then .Einträge = New List(Of VIPEntry)
            tmpVIPEntry = .Einträge.Find(Function(VIPEintrag) VIPEintrag.EntryID.AreEqual(olKontakt.EntryID) And VIPEintrag.StoreID.AreEqual(olKontakt.StoreID))

            If tmpVIPEntry IsNot Nothing Then .Einträge.Remove(tmpVIPEntry)
        End With
    End Sub

#End Region

#Region "TreeView"
    ' 03.04.2020 vorläufig deaktiviert
    'Friend Sub GetKontaktOrdnerInTreeView(ByVal TreeView As Windows.Forms.TreeView)
    '    Dim olNamespace As Outlook.NameSpace = ThisAddIn.POutookApplication.GetNamespace("MAPI")
    '    Dim iOrdner As Integer = 1

    '    With TreeView
    '        .Nodes.Add("Kontaktordner")
    '    End With

    '    Do While iOrdner.IsLessOrEqual(olNamespace.Folders.Count)
    '        KontaktOrdnerInTreeView(olNamespace.Folders.Item(iOrdner), TreeView, TreeView.Nodes(0))
    '        iOrdner += 1
    '    Loop
    'End Sub

    Private Sub KontaktOrdnerInTreeView(ByVal Ordner As Outlook.MAPIFolder, ByVal TreeView As Windows.Forms.TreeView, ByVal BaseNode As Windows.Forms.TreeNode)
        Dim ChildNode As Windows.Forms.TreeNode
        'Dim iOrdner As Integer = 1
        'Dim SubFolder As Outlook.MAPIFolder

        'Do While iOrdner.IsLessOrEqual(Ordner.Folders.Count)
        '    SubFolder = Ordner.Folders.Item(iOrdner)
        '    ChildNode = BaseNode
        '    If SubFolder.DefaultItemType = Outlook.OlItemType.olContactItem Then
        '        Try
        '            ChildNode = BaseNode.Nodes.Add(SubFolder.EntryID & ";" & SubFolder.StoreID, SubFolder.Name, "Kontakt")
        '            ChildNode.Tag = ChildNode.Name
        '            If ChildNode.Level.AreEqual(1) Then ChildNode.Text += String.Format(" ({0})", Ordner.Name)
        '        Catch ex As Exception
        '            LogFile(String.Format("Auf den Ordner {0} kann nicht zugegriffen werden.", SubFolder.Name))
        '            ChildNode = BaseNode
        '        End Try
        '    End If
        '    KontaktOrdnerInTreeView(SubFolder, TreeView, ChildNode)
        '    iOrdner += 1
        'Loop

        For Each SubFolder As Outlook.MAPIFolder In Ordner.Folders

            If SubFolder.DefaultItemType = Outlook.OlItemType.olContactItem Then

                Try
                    ChildNode = BaseNode.Nodes.Add(SubFolder.EntryID & ";" & SubFolder.StoreID, SubFolder.Name, "Kontakt")
                    ChildNode.Tag = ChildNode.Name
                    If ChildNode.Level.AreEqual(1) Then ChildNode.Text += String.Format(" ({0})", Ordner.Name)
                Catch ex As Exception
                    NLogger.Error(ex, "Auf den Ordner {0} kann nicht zugegriffen werden.", SubFolder.Name)
                    ChildNode = BaseNode
                End Try
            Else
                ChildNode = BaseNode
            End If
            Windows.Forms.Application.DoEvents()
            KontaktOrdnerInTreeView(SubFolder, TreeView, ChildNode)
        Next

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
                    Select Case TelNr.Type
                        Case "home"
                            If .HomeTelephoneNumber.IsStringEmpty Then
                                .HomeTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .Home2TelephoneNumber.IsStringEmpty Then
                                .Home2TelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .CarTelephoneNumber.IsStringEmpty Then
                                .CarTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .OtherTelephoneNumber.IsStringEmpty Then
                                .OtherTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .ISDNNumber.IsStringEmpty Then
                                .ISDNNumber = tmpTelNr.Formatiert
                            ElseIf .TTYTDDTelephoneNumber.IsStringEmpty Then
                                .TTYTDDTelephoneNumber = tmpTelNr.Formatiert
                            End If
                        Case "mobile"
                            If .MobileTelephoneNumber.IsStringEmpty Then
                                .MobileTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .PagerNumber.IsStringEmpty Then
                                .PagerNumber = tmpTelNr.Formatiert
                            ElseIf .RadioTelephoneNumber.IsStringEmpty Then
                                .RadioTelephoneNumber = tmpTelNr.Formatiert
                            End If
                        Case "work"
                            If .BusinessTelephoneNumber.IsStringEmpty Then
                                .BusinessTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .Business2TelephoneNumber.IsStringEmpty Then
                                .Business2TelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .AssistantTelephoneNumber.IsStringEmpty Then
                                .AssistantTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .CallbackTelephoneNumber.IsStringEmpty Then
                                .CallbackTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .CompanyMainTelephoneNumber.IsStringEmpty Then
                                .CompanyMainTelephoneNumber = tmpTelNr.Formatiert
                            ElseIf .PrimaryTelephoneNumber.IsStringEmpty Then
                                .PrimaryTelephoneNumber = tmpTelNr.Formatiert
                            End If
                        Case "fax_work"
                            If .BusinessFaxNumber.IsStringEmpty Then
                                .BusinessFaxNumber = tmpTelNr.Formatiert
                            ElseIf .HomeFaxNumber.IsStringEmpty Then
                                .HomeFaxNumber = tmpTelNr.Formatiert
                            ElseIf .OtherFaxNumber.IsStringEmpty Then
                                .OtherFaxNumber = tmpTelNr.Formatiert
                            ElseIf .TelexNumber.IsStringEmpty Then
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
                mySerializer.Serialize(TextSchreiber, XMLData, XmlSerializerNamespace)
                .Body = TextSchreiber.ToString()
            End Using
        End With
    End Sub

    Friend Function GetSmtpAddress(ByVal card As IMsoContactCard) As String
        If card.AddressType = MsoContactCardAddressType.msoContactCardAddressTypeOutlook Then

            Dim ae As Outlook.AddressEntry = ThisAddIn.POutookApplication.Session.GetAddressEntryFromID(card.Address)

            Select Case ae.AddressEntryUserType
                Case Outlook.OlAddressEntryUserType.olExchangeUserAddressEntry, Outlook.OlAddressEntryUserType.olExchangeRemoteUserAddressEntry
                    Dim ex As Outlook.ExchangeUser = ae.GetExchangeUser()
                    Return ex.PrimarySmtpAddress
                Case Outlook.OlAddressEntryUserType.olOutlookContactAddressEntry
                    Return ae.Address
                Case Else
                    Throw New Exception("Valid address entry not found.")
            End Select
        Else
            Return card.Address
        End If
    End Function

End Module
Friend Class ContactSaved
    Implements IDisposable

    Friend WithEvents Kontakt As Outlook.ContactItem

    Private Sub ContactSaved_Close(ByRef Cancel As Boolean) Handles Kontakt.Close
        ThisAddIn.OffeneKontakInsepektoren.Remove(Me)
        Me.Dispose()
    End Sub

    Private Sub ContactSaved_Write(ByRef Cancel As Boolean) Handles Kontakt.Write
        Using KontaktIndexer As New KontaktIndizierer
            KontaktIndexer.IndiziereKontakt(Kontakt)
        End Using
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                'C_KF = Nothing
            End If
        End If
        Me.disposedValue = True
    End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
