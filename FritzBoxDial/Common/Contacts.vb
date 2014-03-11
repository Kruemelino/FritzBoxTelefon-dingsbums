Imports System.Collections.Generic

Public Class ApiWindow
    Private _hWnd As IntPtr
    Public Property HWnd() As IntPtr
        Get
            Return _hWnd
        End Get
        Set(ByVal value As IntPtr)
            _hWnd = value
        End Set
    End Property
End Class

Public Class Contacts
    Private C_DP As DataProvider
    Private C_hf As Helfer
    Private _C_OLI As OutlookInterface
    Private _listChildren As New List(Of ApiWindow)

    Private ReadOnly UserProperties() As String = Split("FBDB-AssistantTelephoneNumber;FBDB-BusinessTelephoneNumber;FBDB-Business2TelephoneNumber;FBDB-CallbackTelephoneNumber;FBDB-CarTelephoneNumber;FBDB-CompanyMainTelephoneNumber;FBDB-HomeTelephoneNumber;FBDB-Home2TelephoneNumber;FBDB-ISDNNumber;FBDB-MobileTelephoneNumber;FBDB-OtherTelephoneNumber;FBDB-PagerNumber;FBDB-PrimaryTelephoneNumber;FBDB-RadioTelephoneNumber;FBDB-BusinessFaxNumber;FBDB-HomeFaxNumber;FBDB-OtherFaxNumber", ";", , CompareMethod.Text)

    Public Property C_OLI() As OutlookInterface
        Get
            Return _C_OLI
        End Get
        Set(ByVal value As OutlookInterface)
            _C_OLI = value
        End Set
    End Property

    Public Sub New(ByVal DataProviderKlasse As DataProvider, ByVal HelferKlasse As Helfer)

        ' Zuweisen der an die Klasse übergebenen Parameter an die internen Variablen, damit sie in der Klasse global verfügbar sind
        C_DP = DataProviderKlasse
        C_hf = HelferKlasse
    End Sub

    Friend Function KontaktSuche(ByRef KontaktID As String, _
                ByRef StoreID As String, _
                ByVal alleOrdner As Boolean, _
                ByRef TelNr As String, _
                ByVal Absender As String, _
                ByVal LandesVW As String) As Outlook.ContactItem
        KontaktSuche = Nothing
        Dim oApp As Outlook.Application = C_OLI.OutlookApplication()
        If Not oApp Is Nothing Then
            Dim olNamespace As Outlook.NameSpace = oApp.GetNamespace("MAPI")
            If alleOrdner Then
                KontaktSuche = FindeKontakt(TelNr, Absender, LandesVW, olNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts))
            Else
                KontaktSuche = FindeKontakt(TelNr, Absender, LandesVW, olNamespace)
            End If
            If Not KontaktSuche Is Nothing Then
                With KontaktSuche
                    KontaktID = .EntryID
                    StoreID = CType(.Parent, Outlook.MAPIFolder).StoreID
                End With
            End If
            olNamespace = Nothing
        Else
            C_hf.LogFile("Kontaktsuche konnte nicht gestartet werden.")
        End If
        oApp = Nothing
    End Function

    Private Overloads Function FindeKontakt(ByRef TelNr As String, _
                              ByVal Absender As String, _
                              ByVal LandesVW As String, _
                              ByVal NamensRaum As Outlook.NameSpace) _
                              As Outlook.ContactItem

        Dim KontaktGefunden As Outlook.ContactItem = Nothing

        '  Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        Dim j As Integer = 1
        Do While (j <= NamensRaum.Folders.Count) And (KontaktGefunden Is Nothing)
            KontaktGefunden = FindeKontakt(TelNr, Absender, LandesVW, NamensRaum.Folders.Item(j))
            j = j + 1
            Windows.Forms.Application.DoEvents()
        Loop
        Return KontaktGefunden
    End Function

    Private Overloads Function FindeKontakt(ByRef TelNr As String, _
                                 ByVal Absender As String, _
                                 ByVal LandesVW As String, _
                                 ByVal Ordner As Outlook.MAPIFolder) _
                                 As Outlook.ContactItem

        ' sucht in der Kontaktdatenbank nach der TelNr/Email
        ' Parameter:  TelNr (String):           Telefonnummer des zu Suchenden
        '             Absender (String):        AbsenderEmailadresse, des Suchenden
        '             LandesVW (String):        eigene Landesvorwahl
        '             KontaktID (String):       ID der Kontaktdaten falls was gefunden wurde (nur Rückgabewert)
        '             Ordner (Object):          der zu durchsuchende Kontaktordner (für die rekursive Suche)
        '             NamensRaum:               Der Namespace, falls übergeordnet durchsucht werden soll.
        ' Rückgabewert (Outlook.ContactItem):   Der gefundene Kontakt

        Dim gefunden As Outlook.ContactItem = Nothing ' was gefunden?

        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner
        Dim aktKontakt As Outlook.ContactItem  ' aktueller Kontakt
        Dim alleTE(14) As String  ' alle TelNr/Email eines Kontakts
        Dim sFilter As String = C_DP.P_Def_StringEmpty

        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then
            If Not Absender = C_DP.P_Def_StringEmpty Then
                sFilter = String.Concat("[Email1Address] = """, Absender, """ OR [Email2Address] = """, Absender, """ OR [Email3Address] = """, Absender, """")
                gefunden = CType(Ordner.Items.Find(sFilter), Outlook.ContactItem)
            Else
                If C_DP.P_CBIndex Then
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
            gefunden = FindeKontakt(TelNr, Absender, LandesVW, Ordner.Folders.Item(iOrdner))
            iOrdner = iOrdner + 1
            Windows.Forms.Application.DoEvents()
        Loop
        FindeKontakt = gefunden
        aktKontakt = Nothing
    End Function '(FindeKontakt)

    Friend Overloads Function ErstelleKontakt(ByRef KontaktID As String, ByRef StoreID As String, ByVal vCard As String, ByVal TelNr As String, ByVal Speichern As Boolean) As Outlook.ContactItem
        Dim FritzFolderExists As Boolean = False
        Dim olKontakt As Outlook.ContactItem = Nothing        ' Objekt des Kontakteintrags

        If Not vCard = C_DP.P_Def_StringEmpty Then
            Dim olFolder As Outlook.MAPIFolder

            olKontakt = CType(C_OLI.OutlookApplication.CreateItem(Outlook.OlItemType.olContactItem), Outlook.ContactItem)
            olFolder = GetOutlookFolder(C_DP.P_TVKontaktOrdnerEntryID, C_DP.P_TVKontaktOrdnerStoreID)
            If olFolder Is Nothing Then olFolder = C_OLI.OutlookApplication.GetNamespace("MAPI").GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)
            olKontakt = CType(olKontakt.Move(olFolder), Outlook.ContactItem)
            C_hf.NAR(olFolder)
            vCard2Contact(vCard, olKontakt)

            With olKontakt
                If Not C_hf.nurZiffern(.BusinessTelephoneNumber, C_DP.P_TBLandesVW) = C_hf.nurZiffern(TelNr, C_DP.P_TBLandesVW) And Not .BusinessTelephoneNumber = C_DP.P_Def_StringEmpty Then
                    .Business2TelephoneNumber = C_hf.formatTelNr(TelNr)
                ElseIf Not C_hf.nurZiffern(.HomeTelephoneNumber, C_DP.P_TBLandesVW) = C_hf.nurZiffern(TelNr, C_DP.P_TBLandesVW) And Not .HomeTelephoneNumber = C_DP.P_Def_StringEmpty Then
                    .Home2TelephoneNumber = C_hf.formatTelNr(TelNr)
                End If
                .Categories = "Fritz!Box (automatisch erstellt)" 'Alle Kontakte, die erstellt werden, haben diese Kategorie. Damit sind sie einfach zu erkennen
                .Body = .Body & vbCrLf & "Erstellt durch das Fritz!Box Telefon-dingsbums am " & System.DateTime.Now
                If Not C_DP.P_CBIndexAus Then IndiziereKontakt(olKontakt, True)
                If Speichern Then
                    .Save()
                    KontaktID = .EntryID
                    StoreID = CType(.Parent, Outlook.MAPIFolder).StoreID
                End If
                C_hf.LogFile("Kontakt " & .FullName & " wurde erstellt")
            End With
        End If
        ErstelleKontakt = olKontakt
        'C_hf.NAR(olKontakt)
        olKontakt = Nothing
    End Function

    Friend Overloads Function ErstelleKontakt(ByRef KontaktID As String, ByRef StoreID As String, ByVal TelNr As String, ByVal Speichern As Boolean) As Outlook.ContactItem
        ErstelleKontakt = Nothing
        Dim vCard As String
        vCard = Split(KontaktID, ";", 2, CompareMethod.Text)(1)
        If Not vCard = C_DP.P_Def_StringEmpty Then
            Return ErstelleKontakt(KontaktID, StoreID, vCard, TelNr, Speichern)
        End If
    End Function

    ''' <summary>
    ''' Erstellt einen Kontakt aus einem Inspectorfenster (Journal)
    ''' </summary>
    ''' <remarks></remarks>
    Friend Sub KontaktErstellen()
        Dim olAuswahl As Outlook.Inspector ' das aktuelle Inspector-Fenster (Journal)
        Dim vCard As String
        Dim olJournal As Outlook.JournalItem = Nothing
        Dim olKontakt As Outlook.ContactItem = Nothing ' Objekt des Kontakteintrags
        Dim olFolder As Outlook.MAPIFolder
        Dim TelNr As String

        olAuswahl = C_OLI.OutlookApplication.ActiveInspector
        If Not olAuswahl Is Nothing Then
            If TypeOf olAuswahl.CurrentItem Is Outlook.JournalItem Then
                olJournal = CType(olAuswahl.CurrentItem, Outlook.JournalItem)
                With olJournal
                    If Not InStr(1, .Categories, "FritzBox Anrufmonitor", CompareMethod.Text) = 0 Then
                        ' Telefonnummer aus dem .Body herausfiltern
                        TelNr = C_hf.StringEntnehmen(.Body, "Tel.-Nr.: ", "Status: ")
                        ' vCard aus dem .Body herausfiltern
                        vCard = C_hf.StringEntnehmen(.Body, "BEGIN:VCARD", "END:VCARD")
                        ' Wenn keine vCard vorhanden ist, dann Kontakt anzeigen
                        If vCard = C_DP.P_Def_ErrorMinusOne Then
#If Not OVer = 15 Then
                            Dim olLink As Outlook.Link = Nothing
                            For Each olLink In .Links
                                Try
                                    If TypeOf olLink.Item Is Outlook.ContactItem Then
                                        olKontakt = CType(olLink.Item, Outlook.ContactItem)
                                        olKontakt.Display()
                                        C_hf.NAR(olKontakt)
                                        olKontakt = Nothing
                                        Exit For
                                    End If
                                Catch
                                    C_hf.LogFile("KontaktErstellen: Kontakt nicht gefunden")
                                End Try
                            Next
                            C_hf.NAR(olLink) : olLink = Nothing
#Else
                            'Todo
#End If
                        Else
                            olFolder = GetOutlookFolder(C_DP.P_TVKontaktOrdnerEntryID, C_DP.P_TVKontaktOrdnerStoreID)
                            If olFolder Is Nothing Then olFolder = C_OLI.OutlookApplication.GetNamespace("MAPI").GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)
                            olKontakt = CType(C_OLI.OutlookApplication.CreateItem(Outlook.OlItemType.olContactItem), Outlook.ContactItem)
                            olKontakt = CType(olKontakt.Move(olFolder), Outlook.ContactItem)
                            vCard = "BEGIN:VCARD" & vCard & "END:VCARD"
                            vCard2Contact(vCard, olKontakt)
                            If C_hf.Mobilnummer(C_hf.nurZiffern(TelNr, C_DP.P_TBLandesVW)) Then
                                olKontakt.MobileTelephoneNumber = TelNr
                            Else
                                olKontakt.BusinessTelephoneNumber = TelNr
                            End If

                            With olKontakt
                                If Not C_hf.nurZiffern(.BusinessTelephoneNumber, C_DP.P_TBLandesVW) = C_hf.nurZiffern(TelNr, C_DP.P_TBLandesVW) And Not .BusinessTelephoneNumber = C_DP.P_Def_StringEmpty Then
                                    .Business2TelephoneNumber = C_hf.formatTelNr(TelNr)
                                ElseIf Not C_hf.nurZiffern(.HomeTelephoneNumber, C_DP.P_TBLandesVW) = C_hf.nurZiffern(TelNr, C_DP.P_TBLandesVW) And Not .HomeTelephoneNumber = C_DP.P_Def_StringEmpty Then
                                    .Home2TelephoneNumber = C_hf.formatTelNr(TelNr)
                                End If
                                .Categories = "Fritz!Box" 'Alle Kontakte, die erstellt werdn, haben die Kategorie "Fritz!Box". Damit sind sie einfach zu erkennen
                                C_hf.LogFile("Kontakt " & olKontakt.FullName & " wurde aus einem Journaleintrag erzeugt.")
                                .Display()
                            End With
                            C_hf.NAR(olFolder)
                            olFolder = Nothing
                        End If
                    End If
                End With
            End If
        End If
    End Sub ' (KontaktErstellen)

    ''' <summary>
    ''' Zeigt einen Kontakt an. Ist der Kontakt nicht vorhanden wird er aus einer vCard oder ein leerer Kontakt erstellt
    ''' </summary>
    ''' <param name="KontaktID">Eindeutige Identifizierung des Kontaktes. Enthält die vCard, wenn kein Outlookkontakt.</param>
    ''' <param name="StoreID">Eindeutige Identifizierung des Speicherordners des Kontaktes. Enthält die -1, wenn kein Outlookkontakt.</param>
    ''' <param name="TelNr">telefonnummer des Kontaktes</param>
    ''' <param name="Notiz">Notiz, die dem Kontakt hinzugefügt wird.</param>
    ''' <param name="AnrufRichtung">0 Eingehend, 1 Ausgehend</param>
    ''' <remarks></remarks>
    Public Function ZeigeKontakt(ByVal KontaktID As String, ByVal StoreID As String, ByVal TelNr As String) As Outlook.ContactItem

        ZeigeKontakt = Nothing
        ' alle Telefonnummern in der vCard
        If StoreID = C_DP.P_Def_ErrorMinusOne Then
            Dim vCard As String
            Dim alleTelNr As String
            ' kein Kontakteintrag vorhanden, dann anlegen und ausfüllen
            ZeigeKontakt = CType(C_OLI.OutlookApplication.CreateItem(Outlook.OlItemType.olContactItem), Outlook.ContactItem)
            vCard = KontaktID
            With ZeigeKontakt

                If Not vCard = C_DP.P_Def_ErrorMinusOne And Not vCard = C_DP.P_Def_StringEmpty Then
                    vCard2Contact(vCard, ZeigeKontakt)
                    .Body = .Body & vbNewLine & "Kontaktdaten (vCard):" & vbNewLine & vCard
                End If
                If C_hf.Mobilnummer(C_hf.nurZiffern(TelNr, C_DP.P_TBLandesVW)) Then
                    .MobileTelephoneNumber = TelNr
                Else
                    If vCard = C_DP.P_Def_ErrorMinusOne Or vCard = C_DP.P_Def_StringEmpty Then
                        .BusinessTelephoneNumber = TelNr
                    Else
                        ' falls TelNr bei der Rückwärtssuche geändert wurde, diese Nummer als Zweitnummer eintragen
                        alleTelNr = ReadFromVCard(vCard, "TEL", "")
                        If Not C_hf.nurZiffern(.BusinessTelephoneNumber, C_DP.P_TBLandesVW) = C_hf.nurZiffern(TelNr, C_DP.P_TBLandesVW) And Not .BusinessTelephoneNumber = C_DP.P_Def_StringEmpty Then
                            .Business2TelephoneNumber = C_hf.formatTelNr(.BusinessTelephoneNumber)
                            .BusinessTelephoneNumber = C_hf.formatTelNr(TelNr)
                        ElseIf Not C_hf.nurZiffern(.HomeTelephoneNumber, C_DP.P_TBLandesVW) = C_hf.nurZiffern(TelNr, C_DP.P_TBLandesVW) And Not .HomeTelephoneNumber = C_DP.P_Def_StringEmpty Then
                            .Home2TelephoneNumber = C_hf.formatTelNr(.HomeTelephoneNumber)
                            .HomeTelephoneNumber = C_hf.formatTelNr(TelNr)
                        End If
                    End If
                End If
                .Categories = "Fritz!Box"
            End With
        Else
            ' Kontakteintrag anzeigen
            Try
                ZeigeKontakt = GetOutlookKontakt(KontaktID, StoreID)
            Catch ex As Exception
                C_hf.FBDB_MsgBox("Der hinterlegte Kontakt ist nicht mehr verfügbar. Wurde er eventuell gelöscht?", MsgBoxStyle.Information, "")
            End Try
        End If
        If Not ZeigeKontakt Is Nothing Then ZeigeKontakt.Display()
    End Function

    Friend Overloads Sub KontaktInformation(ByRef olContact As Outlook.ContactItem, _
                              Optional ByRef FullName As String = vbNullString, _
                              Optional ByRef CompanyName As String = vbNullString, _
                              Optional ByRef HomeAddress As String = vbNullString, _
                              Optional ByRef BusinessAddress As String = vbNullString)

        If Not olContact Is Nothing Then
            With olContact
                FullName = .FullName
                CompanyName = .CompanyName
                HomeAddress = .HomeAddress
                BusinessAddress = .BusinessAddress
            End With
        End If

    End Sub

    Friend Overloads Sub KontaktInformation(ByRef KontaktID As String, _
                              ByRef StoreID As String, _
                              Optional ByRef FullName As String = vbNullString, _
                              Optional ByRef CompanyName As String = vbNullString, _
                              Optional ByRef HomeAddress As String = vbNullString, _
                              Optional ByRef BusinessAddress As String = vbNullString)

        Dim Kontakt As Outlook.ContactItem = GetOutlookKontakt(KontaktID, StoreID)
        If Not Kontakt Is Nothing Then
            KontaktInformation(Kontakt, FullName:=FullName, CompanyName:=CompanyName, HomeAddress:=HomeAddress, BusinessAddress:=BusinessAddress)
            Kontakt = Nothing
        End If
        C_hf.NAR(Kontakt)
    End Sub

    Friend Function KontaktBild(ByRef KontaktID As String, ByRef StoreID As String) As String
        Dim Kontakt As Outlook.ContactItem = GetOutlookKontakt(KontaktID, StoreID)
        KontaktBild = C_DP.P_Def_StringEmpty
        If Not Kontakt Is Nothing Then
            With Kontakt
                KontaktID = .EntryID
                StoreID = CType(.Parent, Outlook.MAPIFolder).StoreID
                With .Attachments
                    If Not .Item("ContactPicture.jpg") Is Nothing Then
                        KontaktBild = System.IO.Path.GetTempPath() & System.IO.Path.GetRandomFileName()
                        KontaktBild = Left(KontaktBild, Len(KontaktBild) - 3) & "jpg"
                        .Item("ContactPicture.jpg").SaveAsFile(KontaktBild)
                    End If
                End With
            End With
            Kontakt = Nothing
        End If
        C_hf.NAR(Kontakt)
    End Function
    ''' <summary>
    ''' Ermittelt aus der KontaktID und der StoreID den zugehörigen Kontakt.
    ''' </summary>
    ''' <param name="KontaktID">EntryID des Kontaktes</param>
    ''' <param name="StoreID">StoreID des beinhaltenden Ordners</param>
    ''' <returns>Erfolg: Kontakt, Misserfolg: Nothing</returns>
    ''' <remarks></remarks>
    Friend Function GetOutlookKontakt(ByRef KontaktID As String, ByRef StoreID As String) As Outlook.ContactItem
        GetOutlookKontakt = Nothing
        Try
            GetOutlookKontakt = CType(C_OLI.OutlookApplication.GetNamespace("MAPI").GetItemFromID(KontaktID, StoreID), Outlook.ContactItem)
        Catch ex As Exception
            C_hf.LogFile("GetOutlookKontakt: " & ex.Message)
        End Try
    End Function

    Friend Function GetOutlookFolder(ByRef FolderID As String, ByRef StoreID As String) As Outlook.Folder
        GetOutlookFolder = Nothing
        Try
            GetOutlookFolder = CType(C_OLI.OutlookApplication.GetNamespace("MAPI").GetFolderFromID(FolderID, StoreID), Outlook.Folder)
        Catch ex As Exception
            C_hf.LogFile("GetOutlookFolder: " & ex.Message)
        End Try
    End Function
#Region "Kontaktindizierung"
    Friend Sub IndiziereKontakt(ByRef Kontakt As Outlook.ContactItem, WriteLog As Boolean)
        If Not C_DP.P_CBIndexAus Then
            Dim LandesVW As String = C_DP.P_TBLandesVW
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
                    If Not alleTE(i) = C_DP.P_Def_StringEmpty Then ' Fall: Telefonnummer vorhanden
                        If .UserProperties.Find(UserProperties(i)) Is Nothing Then
                            .UserProperties.Add(UserProperties(i), Outlook.OlUserPropertyType.olText, False)
                        End If
                        tempTelNr = C_hf.nurZiffern(alleTE(i), LandesVW)
                        If Not CStr(.UserProperties.Find(UserProperties(i)).Value) = tempTelNr Then
                            .UserProperties.Find(UserProperties(i)).Value = tempTelNr
                        End If
                    ElseIf Not .UserProperties.Find(UserProperties(i)) Is Nothing Then ' Fall:Index vorhanden, Telefonnummer nicht
                        .UserProperties.Find(UserProperties(i)).Delete()
                    End If
                Next
                If WriteLog Then C_hf.LogFile("Kontakt: " & .FullNameAndCompany & " wurde automatisch indiziert.")
                .Save()
            End With
        End If
    End Sub

    Friend Sub DeIndizierungKontakt(ByRef Kontakt As Outlook.ContactItem, WriteLog As Boolean)
        Dim UserEigenschaft As Outlook.UserProperty
        If Not C_DP.P_CBIndexAus Then
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
                    If C_hf.IsOneOf(.Item(1).Name, UserProperties) Then .Remove(1)
                Next
            End With
#End If
        Catch : End Try
    End Sub
#End Region

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
            If TelNr = C_hf.nurZiffern(Telefonnummer, LandesVW) Then Return Telefonnummer
        Next
        Return TelNr
    End Function

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
            If Not ContactName = C_DP.P_Def_StringEmpty Then
                pos = InStr(1, ContactName, "#", CompareMethod.Text)
                If Not pos = 0 Then ContactName = Left(ContactName, pos - 1)
                pos = InStr(1, ContactName, ";", CompareMethod.Text)
                If pos = 0 Then
                    If .LastName = C_DP.P_Def_StringEmpty Then .LastName = ContactName
                Else
                    If .LastName = C_DP.P_Def_StringEmpty Then .LastName = Left(ContactName, pos - 1)
                    ContactName = Mid(ContactName, pos + 1)
                    pos = InStr(1, ContactName, ";", CompareMethod.Text)
                    If pos = 0 Then
                        If .FirstName = C_DP.P_Def_StringEmpty Then .FirstName = ContactName
                    Else
                        If .FirstName = C_DP.P_Def_StringEmpty Then .FirstName = Left(ContactName, pos - 1)
                        ContactName = Mid(ContactName, pos + 1)
                        pos = InStr(1, ContactName, ";", CompareMethod.Text)
                        If pos = 0 Then
                            If .MiddleName = C_DP.P_Def_StringEmpty Then .MiddleName = ContactName
                        Else
                            If .MiddleName = C_DP.P_Def_StringEmpty Then .MiddleName = Left(ContactName, pos - 1)
                            ContactName = Mid(ContactName, pos + 1)
                            pos = InStr(1, ContactName, ";", CompareMethod.Text)
                            If pos = 0 Then
                                If .Title = C_DP.P_Def_StringEmpty Then .Title = ContactName
                            Else
                                If .Title = C_DP.P_Def_StringEmpty Then .Title = Left(ContactName, pos - 1)
                                ContactName = Mid(ContactName, pos + 1)
                                pos = InStr(1, ContactName, ";", CompareMethod.Text)
                                If pos = 0 Then
                                    If .Suffix = C_DP.P_Def_StringEmpty Then .Suffix = ContactName
                                Else
                                    If .Suffix = C_DP.P_Def_StringEmpty Then .Suffix = Left(ContactName, pos - 1)
                                End If
                            End If
                            ' Eingefügt am 9.4.10: Grund 11880 liefert Firmenname mit dem Wort "Firma   " - unschön: entfernt
                            If .Title = "Firma" Then .Title = Nothing
                        End If
                    End If
                End If
            Else
                If .FullName = C_DP.P_Def_StringEmpty Then
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
            If .NickName = C_DP.P_Def_StringEmpty Then
                tmp1 = ReadFromVCard(vCard, "NICKNAME", "")
                pos = InStr(1, tmp1, "#", CompareMethod.Text)
                If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
                .NickName = tmp1
            End If
            'insert Jobtitle and Companny
            If .JobTitle = C_DP.P_Def_StringEmpty Then
                tmp1 = ReadFromVCard(vCard, "TITLE", "")
                pos = InStr(1, tmp1, "#", CompareMethod.Text)
                If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
                .JobTitle = tmp1
            End If
            Company = ReadFromVCard(vCard, "ORG", "")
            If .CompanyName = C_DP.P_Def_StringEmpty Then
                pos = InStr(1, Company, "#", CompareMethod.Text)
                If Not pos = 0 Then Company = Left(Company, pos - 1)
                .CompanyName = Company
            End If
            'insert Telephone Numbers
            BFax = ReadFromVCard(vCard, "TEL", "WORK,FAX")
            If BFax = C_DP.P_Def_StringEmpty Then
                BTel = ReadFromVCard(vCard, "TEL", "WORK")
            Else
                If .BusinessFaxNumber = C_DP.P_Def_StringEmpty Then
                    pos = InStr(1, BFax, "#", CompareMethod.Text)
                    If Not pos = 0 Then BFax = Left(BFax, pos - 1)
                    .BusinessFaxNumber = C_hf.formatTelNr(BFax)
                End If
                BTel = ReadFromVCard(vCard, "TEL", "WORK,VOICE")
            End If
            If .BusinessTelephoneNumber = C_DP.P_Def_StringEmpty Then
                pos = InStr(1, BTel, "#", CompareMethod.Text)
                If Not pos = 0 Then BTel = Left(BTel, pos - 1)
                .BusinessTelephoneNumber = C_hf.formatTelNr(BTel)
            End If
            HFax = ReadFromVCard(vCard, "TEL", "HOME,FAX")
            If HFax = C_DP.P_Def_StringEmpty Then
                HTel = ReadFromVCard(vCard, "TEL", "HOME")
            Else
                If .HomeFaxNumber = C_DP.P_Def_StringEmpty Then
                    pos = InStr(1, HFax, "#", CompareMethod.Text)
                    If Not pos = 0 Then HFax = Left(HFax, pos - 1)
                    .HomeFaxNumber = C_hf.formatTelNr(HFax)
                End If
                HTel = ReadFromVCard(vCard, "TEL", "HOME,VOICE")
            End If
            If .HomeTelephoneNumber = C_DP.P_Def_StringEmpty Then
                pos = InStr(1, HTel, "#", CompareMethod.Text)
                If Not pos = 0 Then HTel = Left(HTel, pos - 1)
                .HomeTelephoneNumber = C_hf.formatTelNr(HTel)
            End If
            Mobile = ReadFromVCard(vCard, "TEL", "CELL")
            If .MobileTelephoneNumber = C_DP.P_Def_StringEmpty Then
                pos = InStr(1, Mobile, "#", CompareMethod.Text)
                If Not pos = 0 Then Mobile = Left(Mobile, pos - 1)
                .MobileTelephoneNumber = C_hf.formatTelNr(Mobile)
            End If
            Pager = ReadFromVCard(vCard, "TEL", "PAGER")
            If .PagerNumber = C_DP.P_Def_StringEmpty Then
                pos = InStr(1, Pager, "#", CompareMethod.Text)
                If Not pos = 0 Then Pager = Left(Pager, pos - 1)
                .PagerNumber = C_hf.formatTelNr(Pager)
            End If
            Car = ReadFromVCard(vCard, "TEL", "CAR")
            If .CarTelephoneNumber = C_DP.P_Def_StringEmpty Then
                pos = InStr(1, Car, "#", CompareMethod.Text)
                If Not pos = 0 Then Car = Left(Car, pos - 1)
                .CarTelephoneNumber = C_hf.formatTelNr(Car)
            End If
            ISDN = ReadFromVCard(vCard, "TEL", "ISDN")
            If .ISDNNumber = C_DP.P_Def_StringEmpty Then
                pos = InStr(1, ISDN, "#", CompareMethod.Text)
                If Not pos = 0 Then ISDN = Left(ISDN, pos - 1)
                .ISDNNumber = C_hf.formatTelNr(ISDN)
            End If
            If BFax = C_DP.P_Def_StringEmpty And _
                BTel = C_DP.P_Def_StringEmpty And _
                HFax = C_DP.P_Def_StringEmpty And _
                HTel = C_DP.P_Def_StringEmpty And _
                Mobile = C_DP.P_Def_StringEmpty And _
                Pager = C_DP.P_Def_StringEmpty And _
                Car = C_DP.P_Def_StringEmpty And _
                ISDN = C_DP.P_Def_StringEmpty Then

                tmp1 = ReadFromVCard(vCard, "TEL", "")
                pos = InStr(1, tmp1, "#", CompareMethod.Text)
                If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
                If Company = C_DP.P_Def_StringEmpty Then
                    If .HomeTelephoneNumber = C_DP.P_Def_StringEmpty Then .HomeTelephoneNumber = C_hf.formatTelNr(tmp1)
                Else
                    If .BusinessTelephoneNumber = C_DP.P_Def_StringEmpty Then .BusinessTelephoneNumber = C_hf.formatTelNr(tmp1)
                End If
            End If
            'insert Birthday
            tmp1 = (ReadFromVCard(vCard, "BDAY", ""))
            pos = InStr(1, tmp1, "#", CompareMethod.Text)
            If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
            If Len(tmp1) = 8 Then tmp1 = Left(tmp1, 4) & "-" & Mid(tmp1, 5, 2) & "-" & Mid(tmp1, 7)
            If Not tmp1 = C_DP.P_Def_StringEmpty And CStr(.Birthday) = "01.01.4501" Then .Birthday = CDate(tmp1)
            'insert addresses
            tmp1 = ReadFromVCard(vCard, "ADR", "HOME,POSTAL")
            If tmp1 = C_DP.P_Def_StringEmpty Then tmp1 = ReadFromVCard(vCard, "ADR", "HOME,PARCEL")
            If tmp1 = C_DP.P_Def_StringEmpty Then tmp1 = ReadFromVCard(vCard, "ADR", "HOME")
            tmp2 = ReadFromVCard(vCard, "ADR", "WORK,POSTAL")
            If tmp2 = C_DP.P_Def_StringEmpty Then tmp2 = ReadFromVCard(vCard, "ADR", "WORK,PARCEL")
            If tmp2 = C_DP.P_Def_StringEmpty Then tmp2 = ReadFromVCard(vCard, "ADR", "WORK")
            If tmp1 = C_DP.P_Def_StringEmpty And tmp2 = C_DP.P_Def_StringEmpty Then
                If Company = C_DP.P_Def_StringEmpty Then
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
                If .HomeAddressPostOfficeBox = C_DP.P_Def_StringEmpty Then .HomeAddressPostOfficeBox = tmp1
            Else
                tmp3 = Left(tmp1, pos - 1)
                tmp1 = Mid(tmp1, pos + 1)
                pos = InStr(1, tmp1, ";", CompareMethod.Text)
                If pos = 0 Then
                    If .HomeAddressPostOfficeBox = C_DP.P_Def_StringEmpty Then .HomeAddressPostOfficeBox = Trim(tmp3 & " " & tmp1)
                Else
                    If .HomeAddressPostOfficeBox = C_DP.P_Def_StringEmpty Then .HomeAddressPostOfficeBox = Trim(tmp3 & " " & Left(tmp1, pos - 1))
                    tmp1 = Mid(tmp1, pos + 1)
                    pos = InStr(1, tmp1, ";", CompareMethod.Text)
                    If pos = 0 Then
                        If .HomeAddressStreet = C_DP.P_Def_StringEmpty Then .HomeAddressStreet = tmp1
                    Else
                        If .HomeAddressStreet = C_DP.P_Def_StringEmpty Then .HomeAddressStreet = Left(tmp1, pos - 1)
                        tmp1 = Mid(tmp1, pos + 1)
                        pos = InStr(1, tmp1, ";", CompareMethod.Text)
                        If pos = 0 Then
                            If .HomeAddressCity = C_DP.P_Def_StringEmpty Then .HomeAddressCity = tmp1
                        Else
                            If .HomeAddressCity = C_DP.P_Def_StringEmpty Then .HomeAddressCity = Left(tmp1, pos - 1)
                            tmp1 = Mid(tmp1, pos + 1)
                            pos = InStr(1, tmp1, ";", CompareMethod.Text)
                            If pos = 0 Then
                                If .HomeAddressState = C_DP.P_Def_StringEmpty Then .HomeAddressState = tmp1
                            Else
                                If .HomeAddressState = C_DP.P_Def_StringEmpty Then .HomeAddressState = Left(tmp1, pos - 1)
                                tmp1 = Mid(tmp1, pos + 1)
                                pos = InStr(1, tmp1, ";", CompareMethod.Text)
                                If pos = 0 Then
                                    If .HomeAddressPostalCode = C_DP.P_Def_StringEmpty Then .HomeAddressPostalCode = tmp1
                                Else
                                    If .HomeAddressPostalCode = C_DP.P_Def_StringEmpty Then .HomeAddressPostalCode = Left(tmp1, pos - 1)
                                    tmp1 = Mid(tmp1, pos + 1)
                                    pos = InStr(1, tmp1, ";", CompareMethod.Text)
                                    If pos = 0 Then
                                        If .HomeAddressCountry = C_DP.P_Def_StringEmpty Then .HomeAddressCountry = tmp1
                                    Else
                                        If .HomeAddressCountry = C_DP.P_Def_StringEmpty Then .HomeAddressCountry = Left(tmp1, pos - 1)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
            pos = InStr(1, tmp2, ";", CompareMethod.Text)
            If pos = 0 Then
                If .BusinessAddressPostOfficeBox = C_DP.P_Def_StringEmpty Then .BusinessAddressPostOfficeBox = tmp2
            Else
                tmp3 = Left(tmp2, pos - 1)
                tmp2 = Mid(tmp2, pos + 1)
                pos = InStr(1, tmp2, ";", CompareMethod.Text)
                If pos = 0 Then
                    If .BusinessAddressPostOfficeBox = C_DP.P_Def_StringEmpty Then .BusinessAddressPostOfficeBox = Trim(tmp3 & " " & tmp2)
                Else
                    If .BusinessAddressPostOfficeBox = C_DP.P_Def_StringEmpty Then .BusinessAddressPostOfficeBox = Trim(tmp3 & " " & Left(tmp2, pos - 1))
                    tmp2 = Mid(tmp2, pos + 1)
                    pos = InStr(1, tmp2, ";", CompareMethod.Text)
                    If pos = 0 Then
                        If .BusinessAddressStreet = C_DP.P_Def_StringEmpty Then .BusinessAddressStreet = tmp2
                    Else
                        If .BusinessAddressStreet = C_DP.P_Def_StringEmpty Then .BusinessAddressStreet = Left(tmp2, pos - 1)
                        tmp2 = Mid(tmp2, pos + 1)
                        pos = InStr(1, tmp2, ";", CompareMethod.Text)
                        If pos = 0 Then
                            If .BusinessAddressCity = C_DP.P_Def_StringEmpty Then .BusinessAddressCity = tmp2
                        Else
                            If .BusinessAddressCity = C_DP.P_Def_StringEmpty Then .BusinessAddressCity = Left(tmp2, pos - 1)
                            tmp2 = Mid(tmp2, pos + 1)
                            pos = InStr(1, tmp2, ";", CompareMethod.Text)
                            If pos = 0 Then
                                If .BusinessAddressState = C_DP.P_Def_StringEmpty Then .BusinessAddressState = tmp2
                            Else
                                If .BusinessAddressState = C_DP.P_Def_StringEmpty Then .BusinessAddressState = Left(tmp2, pos - 1)
                                tmp2 = Mid(tmp2, pos + 1)
                                pos = InStr(1, tmp2, ";", CompareMethod.Text)
                                If pos = 0 Then
                                    If .BusinessAddressPostalCode = C_DP.P_Def_StringEmpty Then .BusinessAddressPostalCode = tmp2
                                Else
                                    If .BusinessAddressPostalCode = C_DP.P_Def_StringEmpty Then .BusinessAddressPostalCode = Left(tmp2, pos - 1)
                                    tmp2 = Mid(tmp2, pos + 1)
                                    pos = InStr(1, tmp2, ";", CompareMethod.Text)
                                    If pos = 0 Then
                                        If .BusinessAddressCountry = C_DP.P_Def_StringEmpty Then .BusinessAddressCountry = tmp2
                                    Else
                                        If .BusinessAddressCountry = C_DP.P_Def_StringEmpty Then .BusinessAddressCountry = Left(tmp2, pos - 1)
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
            If Not tmp2 = C_DP.P_Def_StringEmpty Then
                pos = InStr(1, tmp2, tmp1, CompareMethod.Text)
                If Not tmp1 = C_DP.P_Def_StringEmpty And Not pos = 0 Then
                    tmp2 = tmp1 & ";" & Replace(Left(tmp2, pos - 1) & Mid(tmp2, pos + Len(tmp1)), ";;", ";", , , CompareMethod.Text)
                End If
                Email1 = C_DP.P_Def_StringEmpty
                Email2 = C_DP.P_Def_StringEmpty
                Email3 = C_DP.P_Def_StringEmpty
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
                    If .Email1Address = C_DP.P_Def_StringEmpty Then
                        .Email1Address = Email1
                    ElseIf Not .Email1Address = Email1 Then
                        If Not .Email1Address = Email2 Then Email3 = Email2
                        Email2 = Email1
                    End If
                    If .Email2Address = C_DP.P_Def_StringEmpty Then
                        .Email2Address = Email2
                    ElseIf Not .Email2Address = Email2 Then
                        Email3 = Email2
                    End If
                    If .Email3Address = C_DP.P_Def_StringEmpty Then .Email3Address = Email3
                Catch
                    'LogFile("vCard2Contact: " & Err.Number)
                    If Err.Number = 287 Then
                        'LogFile("Fehler-Beschreibung: " & Err.Description & vbNewLine & "Nutzer hat den Zugriff auf den Kontakt nicht gewährt")
                        C_hf.FBDB_MsgBox("Achtung: Sie haben einen Zugriff auf den Kontakt nicht zugelassen. Email-Addressen oder Notizen konnten nicht in den Kontakt eingetragen werden.", MsgBoxStyle.Exclamation, "vCard2Contact")
                    Else
                        C_hf.FBDB_MsgBox("Es is ein Fehler aufgetreten: " & Err.Description, MsgBoxStyle.Exclamation, "vCard2Contact")
                    End If
                End Try
            End If
            'insert urls
            If .WebPage = C_DP.P_Def_StringEmpty Then .WebPage = Replace(ReadFromVCard(vCard, "URL", ""), ";", " ", , , CompareMethod.Text)
            'insert note
            tmp1 = ReadFromVCard(vCard, "NOTE", "")
            If Not tmp1 = C_DP.P_Def_StringEmpty Then
                Try ' Fehler abfangen
                    .Body = tmp1 & vbNewLine & vbNewLine & .Body
                Catch
                    'LogFile("vCard2Contact: " & Err.Number)
                    If Err.Number = 287 Then
                        'LogFile("Fehler-Beschreibung: " & Err.Description & vbNewLine & "Nutzer hat den Zugriff auf den Kontakt nicht gewährt")
                        C_hf.FBDB_MsgBox("Achtung: Sie haben einen Zugriff auf den Kontakt nicht zugelassen. Email-Addressen oder Notizen konnten nicht in den Kontakt eingetragen werden.", MsgBoxStyle.Exclamation, "vCard2Contact")
                    Else
                        C_hf.FBDB_MsgBox("Es is ein Fehler aufgetreten: " & Err.Description, MsgBoxStyle.Exclamation, "vCard2Contact")
                    End If
                End Try
            End If
        End With

    End Sub

#Region "KontaktNotiz"
    Friend Sub AddNote(ByVal olKontakt As Outlook.ContactItem)

        Dim oInsp As Outlook.Inspector
        Dim Handle As IntPtr

        Dim ReturnValue As Long
        Dim oDoc As Word.Document
        Dim oTable As Word.Table = Nothing
        Dim HeaderRow As Word.Row = Nothing
        Dim CallRow As Word.Row = Nothing
        Dim NoteRow As Word.Row = Nothing
        Dim startLocation As Object

        oInsp = olKontakt.GetInspector
        Handle = GetBodyHandle(oInsp)

        If Not Handle = IntPtr.Zero Then
            oDoc = CType(oInsp.WordEditor, Word.Document)
            CreateTable(oDoc, oTable, HeaderRow, CallRow, NoteRow, True)
            With CallRow
                .Cells(1).Range.Text = C_DP.P_Def_AnrMonDirection_Default
                .Cells(2).Range.Text = C_OLI.BenutzerInitialien
            End With
            If Not NoteRow Is Nothing Then
                startLocation = NoteRow.Range.Start
                oDoc.Range(startLocation, startLocation).Select()
            End If
            oDoc = Nothing

            ' Fokus setzen WICHTIG!
            ReturnValue = OutlookSecurity.SetFocus(Handle)
            ' Aufräumen
            With C_hf
                .NAR(oDoc)
                .NAR(oTable)
                .NAR(HeaderRow)
                .NAR(CallRow)
                .NAR(NoteRow)
            End With
        End If
        'End If
    End Sub

    Private Function GetBodyHandle(ByVal oInsp As Outlook.Inspector) As IntPtr
        Dim HandleNames() As String = Split("AfxWndW;AfxWndW;" & C_DP.P_Def_ErrorMinusOne & ";AfxWndA;_WwB", ";", , CompareMethod.Text)

        GetBodyHandle = OutlookSecurity.FindWindowEX(GetBodyHandle, IntPtr.Zero, "rctrl_renwnd32", oInsp.Caption)

        For Each HandleName As String In HandleNames
            If HandleName = C_DP.P_Def_ErrorMinusOne Then
                GetBodyHandle = GetChildWindows(GetBodyHandle).Item(0).HWnd
            Else
                GetBodyHandle = OutlookSecurity.FindWindowEX(GetBodyHandle, IntPtr.Zero, HandleName, vbNullString)
            End If
            If GetBodyHandle = IntPtr.Zero Then
                Exit For
            End If
        Next
    End Function

    Friend Sub CreateTable(ByRef oDoc As Word.Document, ByRef oTable As Word.Table, ByRef HeaderRow As Word.Row, ByRef CallRow As Word.Row, ByRef NoteRow As Word.Row, ByVal NeueZeile As Boolean)

        Dim nRow As Integer = 1
        Dim nCol As Integer = 6

        Dim oTableLineStyle As Word.WdLineStyle = Word.WdLineStyle.wdLineStyleSingle
        Dim oTableLineWidth_1 As Word.WdLineWidth = Word.WdLineWidth.wdLineWidth025pt
        Dim oTableLineWidth_2 As Word.WdLineWidth = Word.WdLineWidth.wdLineWidth150pt
        Dim oTableLineColor As Word.WdColor = Word.WdColor.wdColorBlack

        Dim Sel4BM As Object

        With oDoc.Bookmarks
            For i = 1 To .Count
                If .Item(i).Name = C_DP.P_Def_Note_Table Then
                    oTable = .Item(i).Range.Tables(1)
                    Exit For
                End If
            Next
        End With
        If oTable Is Nothing Then
            oTable = oDoc.Tables.Add(oDoc.Range(0, 0), nRow, nCol)
            Sel4BM = oTable
            oDoc.Bookmarks.Add(C_DP.P_Def_Note_Table, Sel4BM)
            With oTable
                With .Borders
                    .OutsideLineStyle = oTableLineStyle
                    .OutsideLineWidth = oTableLineWidth_1
                    .OutsideColor = oTableLineColor
                    .InsideLineStyle = oTableLineStyle
                    .InsideLineWidth = oTableLineWidth_1
                    .InsideColor = oTableLineColor
                End With
                HeaderRow = .Rows(1)
                With HeaderRow
                    .Cells(1).Width = 30
                    .Cells(2).Width = 40
                    .Cells(3).Width = 140
                    .Cells(4).Width = 140
                    .Cells(5).Width = 140
                    .Cells(6).Width = 140
                End With

                CallRow = .Rows.Add()
                NoteRow = .Rows.Add()
            End With

            With HeaderRow
                .Range.Font.Bold = vbTrue
                .Cells(1).Range.Text = "Typ"
                .Cells(2).Range.Text = "Initialen"
                .Cells(3).Range.Text = "Telefonnummer"
                .Cells(4).Range.Text = "Begin"
                .Cells(5).Range.Text = "Ende"
                .Cells(6).Range.Text = "Dauer"
                .Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter

                For Each cCell As Word.Cell In .Cells
                    cCell.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter
                Next
            End With

        Else
            HeaderRow = oTable.Rows(1)
            If NeueZeile Then
                CallRow = oTable.Rows.Add(oTable.Rows.Item(2))
                NoteRow = oTable.Rows.Add(oTable.Rows.Item(3))
            Else
                CallRow = oTable.Rows(HeaderRow.Index + 1)
                NoteRow = oTable.Rows(HeaderRow.Index + 2)
            End If
        End If

        With CallRow
            For i = 3 To nCol
                .Cells(i).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight
            Next
        End With
        With NoteRow
            .Cells.Merge()
            .Cells(1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft
            With .Borders(Word.WdBorderType.wdBorderBottom)
                .LineStyle = oTableLineStyle
                .LineWidth = oTableLineWidth_2
                .Color = oTableLineColor
            End With

            With .Range()
                .ParagraphFormat.SpaceBefore = 6
                .ParagraphFormat.SpaceAfter = 6
            End With
        End With
    End Sub
    Friend Function FillNote(ByVal AnrMonTyp As AnrufMonitor.AnrMonEvent, ByVal Telfonat As C_Telefonat, ByVal ContactShown As Boolean) As Long

        'Friend Function FillNote(ByVal AnrMonTyp As AnrufMonitor.AnrMonEvent, ByVal olContact As Outlook.ContactItem, ByVal TelZeit As String, ByVal TelNr As String, ByVal Duration As Double, ByVal ContactShown As Boolean) As Long
        FillNote = vbNull
        With Telfonat

            Dim oInsp As Outlook.Inspector = .olContact.GetInspector
            Dim oPage As Outlook.Pages
            Dim oDoc As Word.Document = CType(oInsp.WordEditor, Word.Document)
            Dim oTable As Word.Table = Nothing

            Dim HeaderRow As Word.Row = Nothing
            Dim CallRow As Word.Row = Nothing
            Dim NoteRow As Word.Row = Nothing

            CreateTable(oDoc, oTable, HeaderRow, CallRow, NoteRow, CBool(IIf((AnrMonTyp = AnrufMonitor.AnrMonEvent.AnrMonRING Or AnrMonTyp = AnrufMonitor.AnrMonEvent.AnrMonCALL) And Not ContactShown, True, False)))
            If Not CallRow Is Nothing Then
                With CallRow
                    Select Case AnrMonTyp
                        Case AnrufMonitor.AnrMonEvent.AnrMonRING, AnrufMonitor.AnrMonEvent.AnrMonCALL
                            .Cells(1).Range.Text = CStr(IIf(AnrMonTyp = AnrufMonitor.AnrMonEvent.AnrMonRING, C_DP.P_Def_AnrMonDirection_Ring, C_DP.P_Def_AnrMonDirection_Call))
                            .Cells(2).Range.Text = C_OLI.BenutzerInitialien
                            .Cells(3).Range.Text = Telfonat.TelNr
                            .Cells(4).Range.Text = CStr(Telfonat.Zeit)
                            .Cells(5).Range.Text = C_DP.P_Def_StringEmpty
                            .Cells(6).Range.Text = C_DP.P_Def_StringEmpty
                        Case AnrufMonitor.AnrMonEvent.AnrMonCONNECT
                            .Cells(4).Range.Text = CStr(Telfonat.Zeit)
                            FillNote = OutlookSecurity.SetFocus(GetBodyHandle(oInsp))
                        Case AnrufMonitor.AnrMonEvent.AnrMonDISCONNECT
                            .Cells(5).Range.Text = Telfonat.Zeit.AddMinutes(Telfonat.Dauer).ToString()
                            .Cells(6).Range.Text = C_hf.GetTimeInterval(Telfonat.Dauer * 60)
                            FillNote = OutlookSecurity.SetFocus(GetBodyHandle(oInsp))
                    End Select
                End With
            End If

            If Not ContactShown Then
                oPage = CType(oInsp.ModifiedFormPages, Outlook.Pages)
                oPage.Add("General")
                oInsp.HideFormPage("General")
                .olContact.Save()
            End If
        End With
    End Function

    Public Function GetChildWindows(ByVal hwnd As IntPtr) As List(Of ApiWindow)
        ' Clear the window list
        Dim ReturnValue As IntPtr
        _listChildren = New List(Of ApiWindow)
        ' Start the enumeration process.
        ReturnValue = OutlookSecurity.EnumChildWindows(hwnd, AddressOf EnumChildWindowProc, &H0)
        ' Return the children list when the process is completed.
        Return _listChildren
    End Function

    Private Sub EnumChildWindowProc(ByVal hwnd As IntPtr, ByVal lParam As Int32)
        ' Attempt to match the child class, if one was specified, otherwise
        ' enumerate all the child windows.
        _listChildren.Add(GetWindowIdentification(hwnd))
    End Sub
    ''' <summary>
    ''' Build the ApiWindow object to hold information about the Window object.
    ''' </summary>
    Private Function GetWindowIdentification(ByVal hwnd As IntPtr) As ApiWindow
        Dim window As New ApiWindow()
        window.HWnd = CType(hwnd, IntPtr)
        Return window
    End Function
#End Region

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
