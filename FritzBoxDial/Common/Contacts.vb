Imports System.Collections.Generic
Imports System.Collections.ObjectModel

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

Public Class KontaktFunktionen
    Private C_DP As DataProvider
    Private C_hf As Helfer
    Private ListChildren As New List(Of ApiWindow)

    Private V_C_OLI As OutlookInterface
    Friend Property C_OLI() As OutlookInterface
        Get
            Return V_C_OLI
        End Get
        Set(ByVal value As OutlookInterface)
            V_C_OLI = value
        End Set
    End Property

    Friend ReadOnly Property P_DefContactFolder() As Outlook.MAPIFolder
        Get
            Return CType(C_OLI.OutlookApplication.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts), Outlook.MAPIFolder)
        End Get
    End Property

    Friend Sub New(ByVal DataProviderKlasse As DataProvider, ByVal HelferKlasse As Helfer)

        ' Zuweisen der an die Klasse übergebenen Parameter an die internen Variablen, damit sie in der Klasse global verfügbar sind
        C_DP = DataProviderKlasse
        C_hf = HelferKlasse

    End Sub

    ''' <summary>
    ''' Startet die Kontaktsuche mit einer E-Mail oder einer Telefonnummer.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, die als Suchkriterium verwendet werden soll.</param>
    ''' <param name="EMailAdresse">E-Mail, die als Suchkriterium verwendet werden soll.</param>
    ''' <param name="KontaktID">Rückgabewert: KontaktID des gefundenen Kontaktes.</param>
    ''' <param name="StoreID">Rückgabewert: StoreID des Ordners, in dem sich der gefundene Kontaktes befindet.</param>
    ''' <param name="alleOrdner">Flag, welches Bestimmt, ob alle Ordner durchsucht werden soll, oder nur der Hauptkontaktordner.</param>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem</c>.</returns>
    Friend Function KontaktSuche(ByRef TelNr As String, _
                                 ByVal EMailAdresse As String, _
                                 ByRef KontaktID As String, _
                                 ByRef StoreID As String, _
                                 ByVal alleOrdner As Boolean) As Outlook.ContactItem

        KontaktSuche = Nothing

        Dim oApp As Outlook.Application = C_OLI.OutlookApplication()
        Dim olSession As Outlook.NameSpace = oApp.Session
        Dim sFilter As String = DataProvider.P_Def_LeerString
        Dim JoinFilter(DataProvider.P_Def_UserProperties.Count - 1) As String

        If oApp IsNot Nothing Then

            If EMailAdresse = DataProvider.P_Def_ErrorMinusOne_String Then
                If C_DP.P_CBIndex Then
                    ' Filter zusammenstellen
#If Not OVer = 11 Then
                    For i = 0 To DataProvider.P_Def_UserProperties.Count - 1
                        JoinFilter(i) = String.Concat("""http://schemas.microsoft.com/mapi/string/{00020329-0000-0000-C000-000000000046}/", _
                                                      DataProvider.P_Def_UserProperties(i), "/0x0000001f"" = '", TelNr, "'")
                    Next
                    sFilter = "@SQL=" & String.Join(" OR ", JoinFilter)
#End If

                    If alleOrdner Then
                        KontaktSuche = FindeAnruferKontakt(TelNr, P_DefContactFolder, sFilter)
                    Else
                        KontaktSuche = FindeAnruferKontakt(TelNr, olSession, sFilter)
                    End If
                End If
            ElseIf Not EMailAdresse = DataProvider.P_Def_LeerString Then
                sFilter = String.Concat("[Email1Address] = """, EMailAdresse, _
                                        """ OR [Email2Address] = """, EMailAdresse, _
                                        """ OR [Email3Address] = """, EMailAdresse, """")

                If alleOrdner Then
                    KontaktSuche = FindeAbsenderKontakt(EMailAdresse, P_DefContactFolder, sFilter)
                Else
                    KontaktSuche = FindeAbsenderKontakt(EMailAdresse, olSession, sFilter)
                End If
            End If

            If KontaktSuche IsNot Nothing Then
                With KontaktSuche
                    KontaktID = .EntryID
                    StoreID = CType(.Parent, Outlook.MAPIFolder).StoreID
                End With
            End If
        End If
        olSession = Nothing
        oApp = Nothing

    End Function

    ''' <summary>
    ''' Überladene Funktion die die Suche mit einer Telefonnummer durchführt. Start ist hier der <c>Outlook.NameSpace.</c>
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, die als Suchkriterium verwendet wird.</param>
    ''' <param name="NamensRaum">Startpunkt der Rekursiven Suche als <c>Outlook.NameSpace</c>.</param>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem</c>.</returns>
    Private Overloads Function FindeAnruferKontakt(ByRef TelNr As String, ByVal NamensRaum As Outlook.NameSpace, ByVal sFilter As String) As Outlook.ContactItem

        Dim KontaktGefunden As Outlook.ContactItem = Nothing

        '  Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        Dim j As Integer = 1
        Do While (j <= NamensRaum.Folders.Count) And (KontaktGefunden Is Nothing)
            KontaktGefunden = FindeAnruferKontakt(TelNr, NamensRaum.Folders.Item(j), sFilter)
            j = j + 1
            Windows.Forms.Application.DoEvents()
        Loop
        Return KontaktGefunden
    End Function

    ''' <summary>
    ''' Überladene Funktion die die Suche mit einer Telefonnummer in einem Outlookordner durchführt. 
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, die als Suchkriterium verwendet wird.</param>
    ''' <param name="Ordner">Outlookordner in dem die Suche durchgeführt wird.</param>
    ''' <param name="sFilter">Der Filter, mit dem die Suche nach dem Kontakt durchgeführt werden soll. (Für Office 2003 irrelevant).</param>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem.</c></returns>
    ''' <remarks>Die Suche wird mittels der outlookinternen Suchroutine (<c>Ordner.Items.Find(sFilter)</c> durchgeführt.
    ''' Der Abgleich erfolgt über die benutzerdefinierten Eigenschaften (<c>UserProperties</c>, die bei der Indizierung festgelegt werden. 
    ''' Der Filter, der für die Suche verwendet wird, wird mittels Stringverkettung aus der Telefonnummer und der benutzerdefinierten Eigenschaften verknüpft.
    ''' Der Filter behandelt dabei alle vorhandenen benutzerdefinierten Eigenschaften des Addins, die mit einem <c> OR </c> verknüpft sind.
    ''' Die Suche über verkettete benutzerdefinierten Eigenschaften erfordert entweder, dass benutzerdefinierten Eigenschaften 
    ''' auch dem Kontaktordner bekannt sind (nicht erwünscht), oder, dass die Suche über eine SQL-Abfrage mit
    ''' <c>Verweisen auf Eigenschaften mithilfe von Namespaces</c> ("http://msdn.microsoft.com/en-us/library/office/ff868915.aspx") durchgeführt wird.
    ''' 
    ''' Es wird pro Kontaktordner ein Suchvorgang durchgeführt. Dieses Suchverfahren kann ab Officeversion 12 verwendet werden.
    ''' 
    ''' In Office 11 muss der Filter klassisch zusammengesetzt werden. Dabei sind pro Kontaktordner mehrere Suchvorgänge erforderlich, da die Verkettung
    ''' mit <c> OR </c> nicht funktioniert.</remarks>
    Private Overloads Function FindeAnruferKontakt(ByRef TelNr As String, ByVal Ordner As Outlook.MAPIFolder, ByVal sFilter As String) As Outlook.ContactItem

        Dim olKontakt As Outlook.ContactItem = Nothing
        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner

        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then

#If OVer = 11 Then
            Dim Personen As Outlook.Items = Ordner.Items
            For Each UserProperty In DataProvider.P_Def_UserProperties
                sFilter = "[" & UserProperty & "] = """ & TelNr & """"
                Try
                    olKontakt = CType(Personen.Find(sFilter), Outlook.ContactItem)
                Catch : End Try
                If olKontakt IsNot Nothing Then Exit For
            Next
#Else
            olKontakt = CType(Ordner.Items.Find(sFilter), Outlook.ContactItem)
#End If
            If olKontakt IsNot Nothing Then
                With olKontakt
                    Dim alleTE() As String = {.AssistantTelephoneNumber, _
                                              .BusinessTelephoneNumber, _
                                              .Business2TelephoneNumber, _
                                              .CallbackTelephoneNumber, _
                                              .CarTelephoneNumber, _
                                              .CompanyMainTelephoneNumber, _
                                              .HomeTelephoneNumber, _
                                              .Home2TelephoneNumber, _
                                              .ISDNNumber, _
                                              .MobileTelephoneNumber, _
                                              .OtherTelephoneNumber, _
                                              .PagerNumber, _
                                              .PrimaryTelephoneNumber, _
                                              .RadioTelephoneNumber, _
                                              .BusinessFaxNumber, _
                                              .HomeFaxNumber, _
                                              .OtherFaxNumber, _
                                              .TelexNumber, _
                                              .TTYTDDTelephoneNumber}

                    For Each fTelNr As String In alleTE
                        If C_hf.TelNrVergleich(TelNr, fTelNr) Then
                            TelNr = fTelNr
                            Exit For
                        End If
                    Next
                End With
            End If
        End If

        ' Unterordner werden rekursiv durchsucht
        iOrdner = 1
        Do While (iOrdner <= Ordner.Folders.Count) And (olKontakt Is Nothing)
            olKontakt = FindeAnruferKontakt(TelNr, Ordner.Folders.Item(iOrdner), sFilter)
            iOrdner = iOrdner + 1
            Windows.Forms.Application.DoEvents()
        Loop
        FindeAnruferKontakt = olKontakt
    End Function '(FindeKontakt)

    ''' <summary>
    ''' Überladene Funktion die die Suche mit einer E-Mail-Adresse durchführt. Start ist hier der <c>Outlook.NameSpace.</c>
    ''' </summary>
    ''' <param name="EMailAdresse">E-Mail-Adresse, die als Suchkriterium verwendet wird.</param>
    ''' <param name="NamensRaum">Startpunkt der Rekursiven Suche als <c>Outlook.NameSpace</c>.</param>
    ''' <param name="sFilter">Der Filter, mit dem die Suche nach dem Kontakt durchgeführt werden soll.</param>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem</c>.</returns>
    Private Overloads Function FindeAbsenderKontakt(ByVal EMailAdresse As String, ByVal NamensRaum As Outlook.NameSpace, ByVal sFilter As String) As Outlook.ContactItem

        Dim KontaktGefunden As Outlook.ContactItem = Nothing

        '  Wenn statt einem Ordner der NameSpace übergeben wurde braucht man zuerst mal die oberste Ordnerliste.
        Dim j As Integer = 1
        Do While (j <= NamensRaum.Folders.Count) And (KontaktGefunden Is Nothing)
            KontaktGefunden = FindeAbsenderKontakt(EMailAdresse, NamensRaum.Folders.Item(j), sFilter)
            j = j + 1
            Windows.Forms.Application.DoEvents()
        Loop
        Return KontaktGefunden
    End Function

    ''' <summary>
    ''' Überladene Funktion die die Suche mit einer Telefonnummer in einem Outlookordner durchführt. 
    ''' </summary>
    ''' <param name="EMailAdresse">E-Mail-Adresse, die als Suchkriterium verwendet wird.</param>
    ''' <param name="Ordner">Outlookordner in dem die Suche durchgeführt wird.</param>
    ''' <param name="sFilter">Der Filter, mit dem die Suche nach dem Kontakt durchgeführt werden soll.</param>
    ''' <returns>Den gefundenen Kontakt als <c>Outlook.ContactItem.</c></returns>
    ''' <remarks>Die Suche wird mittels der outlookinternen Suchroutine (<c>Ordner.Items.Find(sFilter)</c> durchgeführt.</remarks>
    Private Overloads Function FindeAbsenderKontakt(ByVal EMailAdresse As String, ByVal Ordner As Outlook.MAPIFolder, ByVal sFilter As String) As Outlook.ContactItem

        Dim olKontakt As Outlook.ContactItem = Nothing

        Dim iOrdner As Long    ' Zählvariable für den aktuellen Ordner

        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then
            olKontakt = CType(Ordner.Items.Find(sFilter), Outlook.ContactItem)
        End If

        ' Unterordner werden rekursiv durchsucht
        iOrdner = 1
        Do While (iOrdner <= Ordner.Folders.Count) And (olKontakt Is Nothing)
            olKontakt = FindeAbsenderKontakt(EMailAdresse, Ordner.Folders.Item(iOrdner), sFilter)
            iOrdner = iOrdner + 1
            Windows.Forms.Application.DoEvents()
        Loop
        FindeAbsenderKontakt = olKontakt
    End Function '(FindeKontakt)

    ''' <summary>
    ''' Erstellt einen Kontakt aus einer vCard.
    ''' </summary>
    ''' <param name="KontaktID">Rückgabewert: KontaktID des neu erstellten Kontaktes.</param>
    ''' <param name="StoreID">Rückgabewert: StoreID des Ordners, in dem sich der neu erstellte Kontaktes befindet.</param>
    ''' <param name="vCard">Kontaktdaten im vCard-Format.</param>
    ''' <param name="TelNr">Telefonnummer, die zusätzlich eingetragen werden soll.</param>
    ''' <param name="AutoSave">Gibt an ob der Kontakt gespeichert werden soll <c>True</c>, oder nur angezeigt werden soll <c>False</c>.</param>
    ''' <returns>Den erstellte Kontakt als <c>Outlook.ContactItem.</c></returns>
    Friend Overloads Function ErstelleKontakt(ByRef KontaktID As String, ByRef StoreID As String, ByVal vCard As String, ByVal TelNr As String, ByVal AutoSave As Boolean) As Outlook.ContactItem
        Dim olKontakt As Outlook.ContactItem = Nothing        ' Objekt des Kontakteintrags
        Dim olFolder As Outlook.MAPIFolder

        olKontakt = CType(C_OLI.OutlookApplication.CreateItem(Outlook.OlItemType.olContactItem), Outlook.ContactItem)

        With olKontakt
            If C_hf.Mobilnummer(C_hf.nurZiffern(TelNr)) Then
                .MobileTelephoneNumber = TelNr
            Else
                .BusinessTelephoneNumber = TelNr
            End If

            If Not (vCard = DataProvider.P_Def_LeerString Or vCard = DataProvider.P_Def_ErrorMinusOne_String Or vCard = DataProvider.P_Def_ErrorMinusTwo_String) Then
                vCard2Contact(vCard, olKontakt)

                If Not TelNr = DataProvider.P_Def_LeerString Then
                    ' Formatiere Telefonnummer
                    TelNr = C_hf.FormatTelNr(TelNr)
                    If Not .BusinessTelephoneNumber = DataProvider.P_Def_LeerString Then
                        If Not C_hf.TelNrVergleich(.BusinessTelephoneNumber, TelNr) Then
                            .Business2TelephoneNumber = C_hf.FormatTelNr(.BusinessTelephoneNumber)
                        End If
                        .BusinessTelephoneNumber = TelNr
                    ElseIf Not .HomeTelephoneNumber = DataProvider.P_Def_LeerString Then
                        If Not C_hf.TelNrVergleich(.HomeTelephoneNumber, TelNr) Then
                            .Home2TelephoneNumber = C_hf.FormatTelNr(.HomeTelephoneNumber)
                        End If
                        .HomeTelephoneNumber = TelNr
                    ElseIf .HomeTelephoneNumber Is DataProvider.P_Def_LeerString Then
                        .HomeTelephoneNumber = TelNr
                    End If
                End If
                .Categories = DataProvider.P_Def_Addin_LangName '"Fritz!Box Telefon-dingsbums" 'Alle Kontakte, die erstellt werden, haben diese Kategorie. Damit sind sie einfach zu erkennen
                .Body = .Body & vbCrLf & "Erstellt durch das " & DataProvider.P_Def_Addin_LangName & " am " & System.DateTime.Now & DataProvider.P_Def_ZweiNeueZeilen & "vCard:" & DataProvider.P_Def_ZweiNeueZeilen & vCard
            End If
        End With

        If AutoSave Then
            If olKontakt.GetInspector Is Nothing Then IndiziereKontakt(olKontakt)
            ' Todo 1: Prüfe, ob ein Ordner ausgewählt wurde (Properties sind nicht -1)
            ' Todo 2: Prüfe, ob Ordner aus 1 nicht der default Ordner ist.

            'Handlung 1:
            If Not (C_DP.P_TVKontaktOrdnerEntryID = DataProvider.P_Def_ErrorMinusOne_String Or C_DP.P_TVKontaktOrdnerStoreID = DataProvider.P_Def_ErrorMinusOne_String) Then
                olFolder = GetOutlookFolder(C_DP.P_TVKontaktOrdnerEntryID, C_DP.P_TVKontaktOrdnerStoreID)
                ' Handlung 2:
                If olFolder.EntryID = P_DefContactFolder.EntryID And olFolder.StoreID = P_DefContactFolder.StoreID Then
                    olKontakt.Save()
                    C_hf.LogFile("Kontakt " & olKontakt.FullName & " wurde Hauptkontaktordner gespeichert.")
                Else
                    olKontakt = CType(olKontakt.Move(olFolder), Outlook.ContactItem)
                    C_hf.LogFile("Kontakt " & olKontakt.FullName & " wurde erstellt und in den Ordner " & olFolder.Name & " verschoben.")
                End If

                KontaktID = olKontakt.EntryID
                StoreID = olFolder.StoreID
                C_hf.NAR(olFolder)
            End If

        Else
            olKontakt.UserProperties.Add(DataProvider.P_Def_UserPropertyIndex, Outlook.OlUserPropertyType.olText, False).Value = "False"
        End If
        ErstelleKontakt = olKontakt

    End Function

    ''' <summary>
    ''' Erstellt einen leeren Kontakt und ergänzt eine Telefonnummer.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, die eingefügt werden soll.</param>
    ''' <param name="Speichern">Gibt an ob der Kontakt gespeichert werden soll <c>True</c>, oder nur angezeigt werden soll <c>False</c>.</param>
    ''' <returns>Den erstellte Kontakt als <c>Outlook.ContactItem.</c></returns>
    Friend Overloads Function ErstelleKontakt(ByVal TelNr As String, ByVal Speichern As Boolean) As Outlook.ContactItem
        Return ErstelleKontakt(DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, TelNr, Speichern)
    End Function

    ''' <summary>
    ''' Erstellt einen Kontakt aus einem Inspectorfenster (Journal)
    ''' </summary>
    Friend Sub ZeigeKontaktAusJournal()
        Dim olAuswahl As Outlook.Inspector ' das aktuelle Inspector-Fenster (Journal)
        Dim vCard As String
        Dim olJournal As Outlook.JournalItem = Nothing
        Dim olKontakt As Outlook.ContactItem = Nothing ' Objekt des Kontakteintrags
        Dim TelNr As String
        olAuswahl = C_OLI.OutlookApplication.ActiveInspector
        If olAuswahl IsNot Nothing Then
            If TypeOf olAuswahl.CurrentItem Is Outlook.JournalItem Then
                olJournal = CType(olAuswahl.CurrentItem, Outlook.JournalItem)
                With olJournal
                    If Not InStr(1, .Categories, "FritzBox Anrufmonitor", CompareMethod.Text) = 0 Then
                        'Telefonnummer aus dem .Body herausfiltern
                        TelNr = C_hf.StringEntnehmen(.Body, "Tel.-Nr.: ", "Status: ")

                        ' Prüfe ob TelNr unterdrückt
                        If TelNr = DataProvider.P_Def_StringUnknown Then
                            olKontakt = ErstelleKontakt(DataProvider.P_Def_LeerString, False)
                        Else
                            ' Entweder erst eingebetteten Kontakt suchen, oder nach vCard suchen.
                            ' vCard aus dem .Body herausfiltern
                            vCard = DataProvider.P_Def_Begin_vCard & C_hf.StringEntnehmen(.Body, DataProvider.P_Def_Begin_vCard, DataProvider.P_Def_End_vCard) & DataProvider.P_Def_End_vCard

                            'Wenn keine vCard im Body gefunden
                            If vCard = DataProvider.P_Def_Begin_vCard & DataProvider.P_Def_ErrorMinusOne_String & DataProvider.P_Def_End_vCard Then
                                'dann prüfe ob eingebetteter Kontakt vorhanden ist.
                                ' wenn ja olContact damit belegen
#If Not OVer = 15 Then
                                For Each olLink As Outlook.Link In .Links
                                    Try
                                        If TypeOf olLink.Item Is Outlook.ContactItem Then
                                            olKontakt = CType(olLink.Item, Outlook.ContactItem)
                                            Exit For
                                        End If
                                    Catch : End Try
                                Next
#End If
                                If olKontakt Is Nothing Then
                                    ' wenn nicht, dann neuen Kontakt mit TelNr öffnen
                                    olKontakt = ErstelleKontakt(TelNr, False)
                                End If
                            Else
                                'vCard gefunden
                                olKontakt = ErstelleKontakt(DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, vCard, TelNr, False)
                            End If
                        End If
                    End If
                End With
                If olKontakt IsNot Nothing Then olKontakt.Display()
                C_hf.NAR(olJournal)
            End If
        End If
    End Sub ' (KontaktErstellen)

    ''' <summary>
    ''' Speichert das Kontaktbild in den Arbeitsorder. 
    ''' </summary>
    ''' <param name="olContact">Kontakt, aus dem das Kontaktbild extrahiert werden soll.</param>
    ''' <returns>Pfad zum extrahierten Kontaktbild.</returns>
    Friend Function KontaktBild(ByRef olContact As Outlook.ContactItem) As String
        KontaktBild = DataProvider.P_Def_LeerString
        If olContact IsNot Nothing Then
            With olContact
                With .Attachments
                    If .Item("ContactPicture.jpg") IsNot Nothing Then
                        KontaktBild = System.IO.Path.GetTempPath() & System.IO.Path.GetRandomFileName()
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
        If Not PfadKontaktBild = DataProvider.P_Def_LeerString Then
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
            GetOutlookKontakt = CType(C_OLI.OutlookApplication.Session.GetItemFromID(KontaktID, StoreID), Outlook.ContactItem)
        Catch ex As Exception
            C_hf.LogFile("GetOutlookKontakt: " & ex.Message)
        End Try
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

        If Not (FolderID = DataProvider.P_Def_ErrorMinusOne_String Or StoreID = DataProvider.P_Def_ErrorMinusOne_String) Then
            Try
                GetOutlookFolder = CType(C_OLI.OutlookApplication.Session.GetFolderFromID(FolderID, StoreID), Outlook.MAPIFolder)
            Catch ex As Exception
                C_hf.LogFile("GetOutlookFolder: " & ex.Message)
            End Try
        End If

        If GetOutlookFolder Is Nothing Then
            GetOutlookFolder = P_DefContactFolder
            FolderID = GetOutlookFolder.EntryID
            StoreID = CType(GetOutlookFolder.Parent, Outlook.MAPIFolder).StoreID
            C_DP.P_TVKontaktOrdnerEntryID = FolderID
            C_DP.P_TVKontaktOrdnerStoreID = StoreID
        End If
    End Function

#Region "Kontaktindizierung"

    Friend Function IndizierungErforderlich(ByVal olKontaKt As Outlook.ContactItem) As Boolean
        IndizierungErforderlich = True
        With olKontaKt
            ' Nicht Indizieren, wenn Kontakt, der eventuell schon Daten enthält, nur angezeigt wird, aber noch nicht gespeichert wurde.
            ' Indizierung betrifft Telefonnummer, daher Prüfe ob Telefonnummern eingetragen vorhanden.
            Dim alleTE() As String = {.AssistantTelephoneNumber, _
                                      .BusinessTelephoneNumber, _
                                      .Business2TelephoneNumber, _
                                      .CallbackTelephoneNumber, _
                                      .CarTelephoneNumber, _
                                      .CompanyMainTelephoneNumber, _
                                      .HomeTelephoneNumber, _
                                      .Home2TelephoneNumber, _
                                      .ISDNNumber, _
                                      .MobileTelephoneNumber, _
                                      .OtherTelephoneNumber, _
                                      .PagerNumber, _
                                      .PrimaryTelephoneNumber, _
                                      .RadioTelephoneNumber, _
                                      .BusinessFaxNumber, _
                                      .HomeFaxNumber, _
                                      .OtherFaxNumber, _
                                      .TelexNumber, _
                                      .TTYTDDTelephoneNumber}

            If Not C_hf.ClearStringArray(alleTE, False, True, False).Count = 0 Then
                ' Reicht nicht aus! Weiterer Gehirnschmalz erforderlich
                If .UserProperties.Find(DataProvider.P_Def_UserPropertyIndex) IsNot Nothing Then
                    If CBool(.UserProperties.Find(DataProvider.P_Def_UserPropertyIndex).Value) = False Then
                        IndizierungErforderlich = False
                    End If
                    .UserProperties.Find(DataProvider.P_Def_UserPropertyIndex).Delete()
                    Return IndizierungErforderlich
                End If
                Return True
            End If

        End With
        Return False
    End Function

    ''' <summary>
    ''' Indiziert einen Kontaktelement.
    ''' </summary>
    ''' <param name="olKontakt">Der Kontakt der indiziert werden soll.</param>
    Friend Sub IndiziereKontakt(ByRef olKontakt As Outlook.ContactItem)
        If Not C_DP.P_CBIndexAus Then
            Dim tempTelNr As String
            Dim alleTE As New List(Of String)
            Try
                With olKontakt
                    alleTE.Add(.AssistantTelephoneNumber)
                    alleTE.Add(.BusinessTelephoneNumber)
                    alleTE.Add(.Business2TelephoneNumber)
                    alleTE.Add(.CallbackTelephoneNumber)
                    alleTE.Add(.CarTelephoneNumber)
                    alleTE.Add(.CompanyMainTelephoneNumber)
                    alleTE.Add(.HomeTelephoneNumber)
                    alleTE.Add(.Home2TelephoneNumber)
                    alleTE.Add(.ISDNNumber)
                    alleTE.Add(.MobileTelephoneNumber)
                    alleTE.Add(.OtherTelephoneNumber)
                    alleTE.Add(.PagerNumber)
                    alleTE.Add(.PrimaryTelephoneNumber)
                    alleTE.Add(.RadioTelephoneNumber)
                    alleTE.Add(.BusinessFaxNumber)
                    alleTE.Add(.HomeFaxNumber)
                    alleTE.Add(.OtherFaxNumber)
                    alleTE.Add(.TelexNumber)
                    alleTE.Add(.TTYTDDTelephoneNumber)

                    For i = 0 To alleTE.Count
                        If Not alleTE(i) = DataProvider.P_Def_LeerString Then ' Fall: Telefonnummer vorhanden
                            If .UserProperties.Find(DataProvider.P_Def_UserProperties(i)) Is Nothing Then ' Fall Index nicht vorhanden
#If OVer = 11 Then
                                .UserProperties.Add(DataProvider.P_Def_UserProperties(i), Outlook.OlUserPropertyType.olText, True)
#Else
                                .UserProperties.Add(DataProvider.P_Def_UserProperties(i), Outlook.OlUserPropertyType.olText, False)
#End If
                            End If

                            tempTelNr = C_hf.nurZiffern(alleTE(i))
                            If Not CStr(.UserProperties.Find(DataProvider.P_Def_UserProperties(i)).Value) = tempTelNr Then
                                .UserProperties.Find(DataProvider.P_Def_UserProperties(i)).Value = tempTelNr
                            End If
                        ElseIf .UserProperties.Find(DataProvider.P_Def_UserProperties(i)) IsNot Nothing Then ' Fall:Index vorhanden, Telefonnummer nicht
                            .UserProperties.Find(DataProvider.P_Def_UserProperties(i)).Delete()
                        End If
                    Next

                    If Not .Saved Then
                        .Save()
                        C_hf.LogFile("Kontakt " & olKontakt.FullNameAndCompany & " wurde durch die Indizierung gespeichert.")
                    End If

                End With
            Catch ex As Exception
                C_hf.LogFile("IndiziereKontakt: Auf den Kontakt " & olKontakt.FullNameAndCompany & " kann nicht zugegriffen werden." & DataProvider.P_Def_EineNeueZeile & ex.Message)
            End Try
            alleTE = Nothing
        End If
    End Sub

    Friend Sub KontaktFormatTelNr(ByRef olKontakt As Outlook.ContactItem)
        If Not C_DP.P_CBIndexAus Then
            Try
                With olKontakt
                    If Not .AssistantTelephoneNumber = DataProvider.P_Def_LeerString Then .AssistantTelephoneNumber = C_hf.FormatTelNr(.AssistantTelephoneNumber)
                    If Not .BusinessTelephoneNumber = DataProvider.P_Def_LeerString Then .BusinessTelephoneNumber = C_hf.FormatTelNr(.BusinessTelephoneNumber)
                    If Not .Business2TelephoneNumber = DataProvider.P_Def_LeerString Then .Business2TelephoneNumber = C_hf.FormatTelNr(.Business2TelephoneNumber)
                    If Not .CallbackTelephoneNumber = DataProvider.P_Def_LeerString Then .CallbackTelephoneNumber = C_hf.FormatTelNr(.CallbackTelephoneNumber)
                    If Not .CarTelephoneNumber = DataProvider.P_Def_LeerString Then .CarTelephoneNumber = C_hf.FormatTelNr(.CarTelephoneNumber)
                    If Not .CompanyMainTelephoneNumber = DataProvider.P_Def_LeerString Then .CompanyMainTelephoneNumber = C_hf.FormatTelNr(.CompanyMainTelephoneNumber)
                    If Not .HomeTelephoneNumber = DataProvider.P_Def_LeerString Then .HomeTelephoneNumber = C_hf.FormatTelNr(.HomeTelephoneNumber)
                    If Not .Home2TelephoneNumber = DataProvider.P_Def_LeerString Then .Home2TelephoneNumber = C_hf.FormatTelNr(.Home2TelephoneNumber)
                    If Not .ISDNNumber = DataProvider.P_Def_LeerString Then .ISDNNumber = C_hf.FormatTelNr(.ISDNNumber)
                    If Not .MobileTelephoneNumber = DataProvider.P_Def_LeerString Then .MobileTelephoneNumber = C_hf.FormatTelNr(.MobileTelephoneNumber)
                    If Not .OtherTelephoneNumber = DataProvider.P_Def_LeerString Then .OtherTelephoneNumber = C_hf.FormatTelNr(.OtherTelephoneNumber)
                    If Not .PagerNumber = DataProvider.P_Def_LeerString Then .PagerNumber = C_hf.FormatTelNr(.PagerNumber)
                    If Not .PrimaryTelephoneNumber = DataProvider.P_Def_LeerString Then .PrimaryTelephoneNumber = C_hf.FormatTelNr(.PrimaryTelephoneNumber)
                    If Not .RadioTelephoneNumber = DataProvider.P_Def_LeerString Then .RadioTelephoneNumber = C_hf.FormatTelNr(.RadioTelephoneNumber)
                    If Not .BusinessFaxNumber = DataProvider.P_Def_LeerString Then .BusinessFaxNumber = C_hf.FormatTelNr(.BusinessFaxNumber)
                    If Not .HomeFaxNumber = DataProvider.P_Def_LeerString Then .HomeFaxNumber = C_hf.FormatTelNr(.HomeFaxNumber)
                    If Not .OtherFaxNumber = DataProvider.P_Def_LeerString Then .OtherFaxNumber = C_hf.FormatTelNr(.OtherFaxNumber)
                    If Not .TelexNumber = DataProvider.P_Def_LeerString Then .TelexNumber = C_hf.FormatTelNr(.TelexNumber)
                    If Not .TTYTDDTelephoneNumber = DataProvider.P_Def_LeerString Then .TTYTDDTelephoneNumber = C_hf.FormatTelNr(.TTYTDDTelephoneNumber)

                    If Not .Saved Then
                        .Save()
                        C_hf.LogFile("Kontakt " & olKontakt.FullNameAndCompany & " wurde durch das Formatieren der Nummern gespeichert.")
                    End If

                End With
            Catch ex As Exception
                C_hf.LogFile("KontaktFormatTelNr: Auf den Kontakt " & olKontakt.FullNameAndCompany & " kann nicht zugegriffen werden." & DataProvider.P_Def_EineNeueZeile & ex.Message)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Entfernt alle Indizierungseinträge aus einem Kontaktelement.
    ''' </summary>
    ''' <param name="olKontakt">Der Kontakt der deindiziert werden soll.</param>
    Friend Sub DeIndizierungKontakt(ByRef olKontakt As Outlook.ContactItem)
        Dim UserEigenschaft As Outlook.UserProperty
        If Not C_DP.P_CBIndexAus Then
            With olKontakt.UserProperties
                For Each UserProperty In DataProvider.P_Def_UserProperties
                    Try
                        UserEigenschaft = .Find(UserProperty)
                    Catch
                        UserEigenschaft = Nothing
                    End Try
                    If UserEigenschaft IsNot Nothing Then UserEigenschaft.Delete()
                    UserEigenschaft = Nothing
                Next
            End With
            olKontakt.Save()
        End If
    End Sub

    ''' <summary>
    ''' Entfernt alle Indizierungseinträge aus den Ordnern aus einem Kontaktelement.
    ''' </summary>
    ''' <param name="Ordner">Der Ordner der deindiziert werden soll.</param>
    ''' <remarks>Funktion wird eigentlich nicht benötigt, da mit aktuellen Programmversionen keine benutzerdefinierten Kontaktfelder in Ordnern erstellt werden.
    ''' Die Funktion dient zum bereinigen von Ordner, die mit älteren Programmversionen indiziert wurden.</remarks>
    Friend Sub DeIndizierungOrdner(ByVal Ordner As Outlook.MAPIFolder)
#If Not OVer = 11 Then
        Try
            With Ordner.UserDefinedProperties
                For i = 1 To .Count
                    If DataProvider.P_Def_UserProperties.Contains(.Item(1).Name) Then .Remove(1)
                Next
            End With
        Catch : End Try
#End If
    End Sub


#End Region

#Region "vCard"
    ''' <summary>
    ''' Fürgt die Informationen einer vCard in ein Kontaktelement ein.
    ''' </summary>
    ''' <param name="vCard">Quelle: Die vCard, die eingelesen werden soll.</param>
    ''' <param name="Contact">Ziel: (Rückgabe) Der Kontakt in den die Informationen der vCard geschrieben werden als<c>Outlook.ContactItem</c></param>
    Friend Sub vCard2Contact(ByVal vCard As String, ByRef Contact As Outlook.ContactItem)

        Dim ContactName As String  ' kompletter Name ("N") aus vCard
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
            If Not ContactName = DataProvider.P_Def_LeerString Then
                pos = InStr(1, ContactName, "#", CompareMethod.Text)
                If Not pos = 0 Then ContactName = Left(ContactName, pos - 1)
                pos = InStr(1, ContactName, ";", CompareMethod.Text)
                If pos = 0 Then
                    If .LastName = DataProvider.P_Def_LeerString Then .LastName = ContactName
                Else
                    If .LastName = DataProvider.P_Def_LeerString Then .LastName = Left(ContactName, pos - 1)
                    ContactName = Mid(ContactName, pos + 1)
                    pos = InStr(1, ContactName, ";", CompareMethod.Text)
                    If pos = 0 Then
                        If .FirstName = DataProvider.P_Def_LeerString Then .FirstName = ContactName
                    Else
                        If .FirstName = DataProvider.P_Def_LeerString Then .FirstName = Left(ContactName, pos - 1)
                        ContactName = Mid(ContactName, pos + 1)
                        pos = InStr(1, ContactName, ";", CompareMethod.Text)
                        If pos = 0 Then
                            If .MiddleName = DataProvider.P_Def_LeerString Then .MiddleName = ContactName
                        Else
                            If .MiddleName = DataProvider.P_Def_LeerString Then .MiddleName = Left(ContactName, pos - 1)
                            ContactName = Mid(ContactName, pos + 1)
                            pos = InStr(1, ContactName, ";", CompareMethod.Text)
                            If pos = 0 Then
                                If .Title = DataProvider.P_Def_LeerString Then .Title = ContactName
                            Else
                                If .Title = DataProvider.P_Def_LeerString Then .Title = Left(ContactName, pos - 1)
                                ContactName = Mid(ContactName, pos + 1)
                                pos = InStr(1, ContactName, ";", CompareMethod.Text)
                                If pos = 0 Then
                                    If .Suffix = DataProvider.P_Def_LeerString Then .Suffix = ContactName
                                Else
                                    If .Suffix = DataProvider.P_Def_LeerString Then .Suffix = Left(ContactName, pos - 1)
                                End If
                            End If
                            ' Eingefügt am 9.4.10: Grund 11880 liefert Firmenname mit dem Wort "Firma   " - unschön: entfernt
                            If .Title = "Firma" Then .Title = Nothing
                        End If
                    End If
                End If
            Else
                If .FullName = DataProvider.P_Def_LeerString Then
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
            If .NickName = DataProvider.P_Def_LeerString Then
                tmp1 = ReadFromVCard(vCard, "NICKNAME", "")
                pos = InStr(1, tmp1, "#", CompareMethod.Text)
                If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
                .NickName = tmp1
            End If
            'insert Jobtitle and Companny
            If .JobTitle = DataProvider.P_Def_LeerString Then
                tmp1 = ReadFromVCard(vCard, "TITLE", "")
                pos = InStr(1, tmp1, "#", CompareMethod.Text)
                If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
                .JobTitle = tmp1
            End If
            Company = ReadFromVCard(vCard, "ORG", "")
            If .CompanyName = DataProvider.P_Def_LeerString Then
                pos = InStr(1, Company, "#", CompareMethod.Text)
                If Not pos = 0 Then Company = Left(Company, pos - 1)
                .CompanyName = Company
            End If
            'insert Telephone Numbers
            BFax = ReadFromVCard(vCard, "TEL", "WORK,FAX")
            If BFax = DataProvider.P_Def_LeerString Then
                BTel = ReadFromVCard(vCard, "TEL", "WORK")
            Else
                If .BusinessFaxNumber = DataProvider.P_Def_LeerString Then
                    pos = InStr(1, BFax, "#", CompareMethod.Text)
                    If Not pos = 0 Then BFax = Left(BFax, pos - 1)
                    .BusinessFaxNumber = C_hf.FormatTelNr(BFax)
                End If
                BTel = ReadFromVCard(vCard, "TEL", "WORK,VOICE")
            End If
            If .BusinessTelephoneNumber = DataProvider.P_Def_LeerString Then
                pos = InStr(1, BTel, "#", CompareMethod.Text)
                If Not pos = 0 Then BTel = Left(BTel, pos - 1)
                .BusinessTelephoneNumber = C_hf.FormatTelNr(BTel)
            End If
            HFax = ReadFromVCard(vCard, "TEL", "HOME,FAX")
            If HFax = DataProvider.P_Def_LeerString Then
                HTel = ReadFromVCard(vCard, "TEL", "HOME")
            Else
                If .HomeFaxNumber = DataProvider.P_Def_LeerString Then
                    pos = InStr(1, HFax, "#", CompareMethod.Text)
                    If Not pos = 0 Then HFax = Left(HFax, pos - 1)
                    .HomeFaxNumber = C_hf.FormatTelNr(HFax)
                End If
                HTel = ReadFromVCard(vCard, "TEL", "HOME,VOICE")
            End If
            If .HomeTelephoneNumber = DataProvider.P_Def_LeerString Then
                pos = InStr(1, HTel, "#", CompareMethod.Text)
                If Not pos = 0 Then HTel = Left(HTel, pos - 1)
                .HomeTelephoneNumber = C_hf.FormatTelNr(HTel)
            End If
            Mobile = ReadFromVCard(vCard, "TEL", "CELL")
            If .MobileTelephoneNumber = DataProvider.P_Def_LeerString Then
                pos = InStr(1, Mobile, "#", CompareMethod.Text)
                If Not pos = 0 Then Mobile = Left(Mobile, pos - 1)
                .MobileTelephoneNumber = C_hf.FormatTelNr(Mobile)
            End If
            Pager = ReadFromVCard(vCard, "TEL", "PAGER")
            If .PagerNumber = DataProvider.P_Def_LeerString Then
                pos = InStr(1, Pager, "#", CompareMethod.Text)
                If Not pos = 0 Then Pager = Left(Pager, pos - 1)
                .PagerNumber = C_hf.FormatTelNr(Pager)
            End If
            Car = ReadFromVCard(vCard, "TEL", "CAR")
            If .CarTelephoneNumber = DataProvider.P_Def_LeerString Then
                pos = InStr(1, Car, "#", CompareMethod.Text)
                If Not pos = 0 Then Car = Left(Car, pos - 1)
                .CarTelephoneNumber = C_hf.FormatTelNr(Car)
            End If
            ISDN = ReadFromVCard(vCard, "TEL", "ISDN")
            If .ISDNNumber = DataProvider.P_Def_LeerString Then
                pos = InStr(1, ISDN, "#", CompareMethod.Text)
                If Not pos = 0 Then ISDN = Left(ISDN, pos - 1)
                .ISDNNumber = C_hf.FormatTelNr(ISDN)
            End If
            If BFax = DataProvider.P_Def_LeerString And _
                BTel = DataProvider.P_Def_LeerString And _
                HFax = DataProvider.P_Def_LeerString And _
                HTel = DataProvider.P_Def_LeerString And _
                Mobile = DataProvider.P_Def_LeerString And _
                Pager = DataProvider.P_Def_LeerString And _
                Car = DataProvider.P_Def_LeerString And _
                ISDN = DataProvider.P_Def_LeerString Then

                tmp1 = ReadFromVCard(vCard, "TEL", "")
                pos = InStr(1, tmp1, "#", CompareMethod.Text)
                If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
                If Company = DataProvider.P_Def_LeerString Then
                    If .HomeTelephoneNumber = DataProvider.P_Def_LeerString Then .HomeTelephoneNumber = C_hf.FormatTelNr(tmp1)
                Else
                    If .BusinessTelephoneNumber = DataProvider.P_Def_LeerString Then .BusinessTelephoneNumber = C_hf.FormatTelNr(tmp1)
                End If
            End If
            'insert Birthday
            tmp1 = (ReadFromVCard(vCard, "BDAY", ""))
            pos = InStr(1, tmp1, "#", CompareMethod.Text)
            If Not pos = 0 Then tmp1 = Left(tmp1, pos - 1)
            If Len(tmp1) = 8 Then tmp1 = Left(tmp1, 4) & "-" & Mid(tmp1, 5, 2) & "-" & Mid(tmp1, 7)
            If Not tmp1 = DataProvider.P_Def_LeerString And CStr(.Birthday) = "01.01.4501" Then .Birthday = CDate(tmp1)
            'insert addresses
            tmp1 = ReadFromVCard(vCard, "ADR", "HOME,POSTAL")
            If tmp1 = DataProvider.P_Def_LeerString Then tmp1 = ReadFromVCard(vCard, "ADR", "HOME,PARCEL")
            If tmp1 = DataProvider.P_Def_LeerString Then tmp1 = ReadFromVCard(vCard, "ADR", "HOME")
            tmp2 = ReadFromVCard(vCard, "ADR", "WORK,POSTAL")
            If tmp2 = DataProvider.P_Def_LeerString Then tmp2 = ReadFromVCard(vCard, "ADR", "WORK,PARCEL")
            If tmp2 = DataProvider.P_Def_LeerString Then tmp2 = ReadFromVCard(vCard, "ADR", "WORK")
            If tmp1 = DataProvider.P_Def_LeerString And tmp2 = DataProvider.P_Def_LeerString Then
                If Company = DataProvider.P_Def_LeerString Then
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
                If .HomeAddressPostOfficeBox = DataProvider.P_Def_LeerString Then .HomeAddressPostOfficeBox = tmp1
            Else
                tmp3 = Left(tmp1, pos - 1)
                tmp1 = Mid(tmp1, pos + 1)
                pos = InStr(1, tmp1, ";", CompareMethod.Text)
                If pos = 0 Then
                    If .HomeAddressPostOfficeBox = DataProvider.P_Def_LeerString Then .HomeAddressPostOfficeBox = Trim(tmp3 & " " & tmp1)
                Else
                    If .HomeAddressPostOfficeBox = DataProvider.P_Def_LeerString Then .HomeAddressPostOfficeBox = Trim(tmp3 & " " & Left(tmp1, pos - 1))
                    tmp1 = Mid(tmp1, pos + 1)
                    pos = InStr(1, tmp1, ";", CompareMethod.Text)
                    If pos = 0 Then
                        If .HomeAddressStreet = DataProvider.P_Def_LeerString Then .HomeAddressStreet = tmp1
                    Else
                        If .HomeAddressStreet = DataProvider.P_Def_LeerString Then .HomeAddressStreet = Left(tmp1, pos - 1)
                        tmp1 = Mid(tmp1, pos + 1)
                        pos = InStr(1, tmp1, ";", CompareMethod.Text)
                        If pos = 0 Then
                            If .HomeAddressCity = DataProvider.P_Def_LeerString Then .HomeAddressCity = tmp1
                        Else
                            If .HomeAddressCity = DataProvider.P_Def_LeerString Then .HomeAddressCity = Left(tmp1, pos - 1)
                            tmp1 = Mid(tmp1, pos + 1)
                            pos = InStr(1, tmp1, ";", CompareMethod.Text)
                            If pos = 0 Then
                                If .HomeAddressState = DataProvider.P_Def_LeerString Then .HomeAddressState = tmp1
                            Else
                                If .HomeAddressState = DataProvider.P_Def_LeerString Then .HomeAddressState = Left(tmp1, pos - 1)
                                tmp1 = Mid(tmp1, pos + 1)
                                pos = InStr(1, tmp1, ";", CompareMethod.Text)
                                If pos = 0 Then
                                    If .HomeAddressPostalCode = DataProvider.P_Def_LeerString Then .HomeAddressPostalCode = tmp1
                                Else
                                    If .HomeAddressPostalCode = DataProvider.P_Def_LeerString Then .HomeAddressPostalCode = Left(tmp1, pos - 1)
                                    tmp1 = Mid(tmp1, pos + 1)
                                    pos = InStr(1, tmp1, ";", CompareMethod.Text)
                                    If pos = 0 Then
                                        If .HomeAddressCountry = DataProvider.P_Def_LeerString Then .HomeAddressCountry = tmp1
                                    Else
                                        If .HomeAddressCountry = DataProvider.P_Def_LeerString Then .HomeAddressCountry = Left(tmp1, pos - 1)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
            pos = InStr(1, tmp2, ";", CompareMethod.Text)
            If pos = 0 Then
                If .BusinessAddressPostOfficeBox = DataProvider.P_Def_LeerString Then .BusinessAddressPostOfficeBox = tmp2
            Else
                tmp3 = Left(tmp2, pos - 1)
                tmp2 = Mid(tmp2, pos + 1)
                pos = InStr(1, tmp2, ";", CompareMethod.Text)
                If pos = 0 Then
                    If .BusinessAddressPostOfficeBox = DataProvider.P_Def_LeerString Then .BusinessAddressPostOfficeBox = Trim(tmp3 & " " & tmp2)
                Else
                    If .BusinessAddressPostOfficeBox = DataProvider.P_Def_LeerString Then .BusinessAddressPostOfficeBox = Trim(tmp3 & " " & Left(tmp2, pos - 1))
                    tmp2 = Mid(tmp2, pos + 1)
                    pos = InStr(1, tmp2, ";", CompareMethod.Text)
                    If pos = 0 Then
                        If .BusinessAddressStreet = DataProvider.P_Def_LeerString Then .BusinessAddressStreet = tmp2
                    Else
                        If .BusinessAddressStreet = DataProvider.P_Def_LeerString Then .BusinessAddressStreet = Left(tmp2, pos - 1)
                        tmp2 = Mid(tmp2, pos + 1)
                        pos = InStr(1, tmp2, ";", CompareMethod.Text)
                        If pos = 0 Then
                            If .BusinessAddressCity = DataProvider.P_Def_LeerString Then .BusinessAddressCity = tmp2
                        Else
                            If .BusinessAddressCity = DataProvider.P_Def_LeerString Then .BusinessAddressCity = Left(tmp2, pos - 1)
                            tmp2 = Mid(tmp2, pos + 1)
                            pos = InStr(1, tmp2, ";", CompareMethod.Text)
                            If pos = 0 Then
                                If .BusinessAddressState = DataProvider.P_Def_LeerString Then .BusinessAddressState = tmp2
                            Else
                                If .BusinessAddressState = DataProvider.P_Def_LeerString Then .BusinessAddressState = Left(tmp2, pos - 1)
                                tmp2 = Mid(tmp2, pos + 1)
                                pos = InStr(1, tmp2, ";", CompareMethod.Text)
                                If pos = 0 Then
                                    If .BusinessAddressPostalCode = DataProvider.P_Def_LeerString Then .BusinessAddressPostalCode = tmp2
                                Else
                                    If .BusinessAddressPostalCode = DataProvider.P_Def_LeerString Then .BusinessAddressPostalCode = Left(tmp2, pos - 1)
                                    tmp2 = Mid(tmp2, pos + 1)
                                    pos = InStr(1, tmp2, ";", CompareMethod.Text)
                                    If pos = 0 Then
                                        If .BusinessAddressCountry = DataProvider.P_Def_LeerString Then .BusinessAddressCountry = tmp2
                                    Else
                                        If .BusinessAddressCountry = DataProvider.P_Def_LeerString Then .BusinessAddressCountry = Left(tmp2, pos - 1)
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
            If Not tmp2 = DataProvider.P_Def_LeerString Then
                pos = InStr(1, tmp2, tmp1, CompareMethod.Text)
                If Not tmp1 = DataProvider.P_Def_LeerString And Not pos = 0 Then
                    tmp2 = tmp1 & ";" & Replace(Left(tmp2, pos - 1) & Mid(tmp2, pos + Len(tmp1)), ";;", ";", , , CompareMethod.Text)
                End If
                Email1 = DataProvider.P_Def_LeerString
                Email2 = DataProvider.P_Def_LeerString
                Email3 = DataProvider.P_Def_LeerString
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
                    If .Email1Address = DataProvider.P_Def_LeerString Then
                        .Email1Address = Email1
                    ElseIf Not .Email1Address = Email1 Then
                        If Not .Email1Address = Email2 Then Email3 = Email2
                        Email2 = Email1
                    End If
                    If .Email2Address = DataProvider.P_Def_LeerString Then
                        .Email2Address = Email2
                    ElseIf Not .Email2Address = Email2 Then
                        Email3 = Email2
                    End If
                    If .Email3Address = DataProvider.P_Def_LeerString Then .Email3Address = Email3
                Catch
                    'LogFile("vCard2Contact: " & Err.Number)
                    If Err.Number = 287 Then
                        'LogFile("Fehler-Beschreibung: " & Err.Description & vbNewLine & "Nutzer hat den Zugriff auf den Kontakt nicht gewährt")
                        C_hf.MsgBox("Achtung: Sie haben einen Zugriff auf den Kontakt nicht zugelassen. Email-Addressen oder Notizen konnten nicht in den Kontakt eingetragen werden.", MsgBoxStyle.Exclamation, "vCard2Contact")
                    Else
                        C_hf.MsgBox("Es is ein Fehler aufgetreten: " & Err.Description, MsgBoxStyle.Exclamation, "vCard2Contact")
                    End If
                End Try
            End If
            'insert urls
            If .WebPage = DataProvider.P_Def_LeerString Then .WebPage = Replace(ReadFromVCard(vCard, "URL", ""), ";", " ", , , CompareMethod.Text)
            'insert note
            tmp1 = ReadFromVCard(vCard, "NOTE", "")
            If Not tmp1 = DataProvider.P_Def_LeerString Then
                Try ' Fehler abfangen
                    .Body = tmp1 & vbNewLine & vbNewLine & .Body
                Catch
                    'LogFile("vCard2Contact: " & Err.Number)
                    If Err.Number = 287 Then
                        'LogFile("Fehler-Beschreibung: " & Err.Description & vbNewLine & "Nutzer hat den Zugriff auf den Kontakt nicht gewährt")
                        C_hf.MsgBox("Achtung: Sie haben einen Zugriff auf den Kontakt nicht zugelassen. Email-Addressen oder Notizen konnten nicht in den Kontakt eingetragen werden.", MsgBoxStyle.Exclamation, "vCard2Contact")
                    Else
                        C_hf.MsgBox("Es is ein Fehler aufgetreten: " & Err.Description, MsgBoxStyle.Exclamation, "vCard2Contact")
                    End If
                End Try
            End If
        End With

    End Sub
#End Region

#Region "KontaktNotiz"
#If Not OVer = 11 Then

    ''' <summary>
    ''' Fügt einen Notizzeile in den Body eines Kontaktes
    ''' </summary>
    ''' <param name="olKontakt">Kontakt, in den die Notizzeile geschrieben werden soll.</param>
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
                .Cells(1).Range.Text = DataProvider.P_Def_AnrMonDirection_Default
                .Cells(2).Range.Text = C_OLI.BenutzerInitialien
            End With

            If NoteRow IsNot Nothing Then
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
    End Sub


    ''' <summary>
    ''' Ermittelt den Handle des Body-Elementes eines Kontaktinspectors
    ''' </summary>
    ''' <param name="oInsp">Inspector eines Kontaktes.</param>
    ''' <returns>Pointer auf das Body-Element.</returns>
    Private Function GetBodyHandle(ByVal oInsp As Outlook.Inspector) As IntPtr
        Dim HandleNames() As String = {"AfxWndW", _
                                       "AfxWndW", _
                                       DataProvider.P_Def_ErrorMinusOne_String, _
                                       "AfxWndA", _
                                       "_WwB"}

        GetBodyHandle = OutlookSecurity.FindWindowEX(GetBodyHandle, IntPtr.Zero, "rctrl_renwnd32", oInsp.Caption)

        For Each HandleName As String In HandleNames
            If HandleName = DataProvider.P_Def_ErrorMinusOne_String Then
                GetBodyHandle = GetChildWindows(GetBodyHandle).Item(0).HWnd
            Else
                GetBodyHandle = OutlookSecurity.FindWindowEX(GetBodyHandle, IntPtr.Zero, HandleName, vbNullString)
            End If
            If GetBodyHandle = IntPtr.Zero Then
                Exit For
            End If
        Next
    End Function

    ''' <summary>
    ''' Erstellt die Notiztabelle, bzw. fügt Notizzeilen an.
    ''' </summary>
    ''' <param name="oDoc">Das Worddokument, in den die Notiztabelle, bzw. Notizzeile eingefügt werden soll.</param>
    ''' <param name="oTable">Die Notiztabelle an sich.</param>
    ''' <param name="HeaderRow">Die Kopfzeile der Notiztabelle.</param>
    ''' <param name="CallRow">Die Kopfzeile des einzelnen Anrufes.</param>
    ''' <param name="NoteRow">BEreich in den die Notizen eingetragen werden.</param>
    ''' <param name="NeueZeile">Flag, die angibt ob eine neue Zeile hinzugefügt werden soll.</param>
    Friend Sub CreateTable(ByRef oDoc As Word.Document, ByRef oTable As Word.Table, ByRef HeaderRow As Word.Row, ByRef CallRow As Word.Row, ByRef NoteRow As Word.Row, ByVal NeueZeile As Boolean)

        Dim nRow As Integer = 1
        Dim nCol As Integer = 6

        Dim oTableLineStyle As Word.WdLineStyle = Word.WdLineStyle.wdLineStyleSingle
        Dim oTableLineWidth_1 As Word.WdLineWidth = Word.WdLineWidth.wdLineWidth025pt
        Dim oTableLineWidth_2 As Word.WdLineWidth = Word.WdLineWidth.wdLineWidth150pt
        Dim oTableLineColor As Word.WdColor = Word.WdColor.wdColorBlack
        Dim oTableFontColorIndex As Word.WdColorIndex = Word.WdColorIndex.wdBlack
        Dim Sel4BM As Object

        With oDoc.Bookmarks
            For i = 1 To .Count
                If .Item(i).Name = DataProvider.P_Def_Note_Table Then
                    oTable = .Item(i).Range.Tables(1)
                    Exit For
                End If
            Next
        End With
        If oTable Is Nothing Then
            oTable = oDoc.Tables.Add(oDoc.Range(0, 0), nRow, nCol)
            Sel4BM = oTable
            oDoc.Bookmarks.Add(DataProvider.P_Def_Note_Table, Sel4BM)
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

            With .Cells(1).Range
                .ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft
                .Font.ColorIndex = oTableFontColorIndex
            End With

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

    ''' <summary>
    ''' Füllt die Notizzeile mit Informationen
    ''' </summary>
    ''' <param name="AnrMonTyp">Gibt, an ob es sich um einen RING, CALL, CONNECT oder DISCONNECT handelt.</param>
    ''' <param name="Telfonat">Alle Informationen zu dem Telefonat.</param>
    ''' <param name="ContactShown">Gibt an ob der Kontakt angezeigt wird.</param>
    Friend Sub FillNote(ByVal AnrMonTyp As AnrufMonitor.AnrMonEvent, ByVal Telfonat As C_Telefonat, ByVal ContactShown As Boolean)

        'FillNote = vbNull
        With Telfonat

            Dim oInsp As Outlook.Inspector = .olContact.GetInspector
            Dim oPage As Outlook.Pages
            Dim oDoc As Word.Document = CType(oInsp.WordEditor, Word.Document)
            Dim oTable As Word.Table = Nothing

            Dim HeaderRow As Word.Row = Nothing
            Dim CallRow As Word.Row = Nothing
            Dim NoteRow As Word.Row = Nothing

            CreateTable(oDoc, oTable, HeaderRow, CallRow, NoteRow, C_hf.IIf((AnrMonTyp = AnrufMonitor.AnrMonEvent.AnrMonRING Or AnrMonTyp = AnrufMonitor.AnrMonEvent.AnrMonCALL) And Not ContactShown, True, False))
            If CallRow IsNot Nothing Then
                With CallRow
                    Select Case AnrMonTyp
                        Case AnrufMonitor.AnrMonEvent.AnrMonRING, AnrufMonitor.AnrMonEvent.AnrMonCALL
                            .Cells(1).Range.Text = C_hf.IIf(AnrMonTyp = AnrufMonitor.AnrMonEvent.AnrMonRING, DataProvider.P_Def_AnrMonDirection_Ring, DataProvider.P_Def_AnrMonDirection_Call)
                            .Cells(2).Range.Text = C_OLI.BenutzerInitialien
                            .Cells(3).Range.Text = Telfonat.TelNr
                            .Cells(4).Range.Text = CStr(Telfonat.Zeit)
                            .Cells(5).Range.Text = DataProvider.P_Def_LeerString
                            .Cells(6).Range.Text = DataProvider.P_Def_LeerString
                        Case AnrufMonitor.AnrMonEvent.AnrMonCONNECT
                            .Cells(4).Range.Text = CStr(Telfonat.Zeit)
                            OutlookSecurity.SetFocus(GetBodyHandle(oInsp)).ToString()
                        Case AnrufMonitor.AnrMonEvent.AnrMonDISCONNECT
                            .Cells(5).Range.Text = Telfonat.Zeit.AddMinutes(Telfonat.Dauer).ToString()
                            .Cells(6).Range.Text = C_hf.GetTimeInterval(Telfonat.Dauer * 60)
                            OutlookSecurity.SetFocus(GetBodyHandle(oInsp)).ToString()
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
    End Sub

    ''' <summary>
    ''' Gibt alle Handles der Childwindows zurück.
    ''' </summary>
    ''' <param name="hwnd">Ausgangshandle</param>
    ''' <returns>Liste der Handles.</returns>
    Private Function GetChildWindows(ByVal hwnd As IntPtr) As List(Of ApiWindow)
        ' Clear the window list
        Dim ReturnValue As Int32
        ListChildren = New List(Of ApiWindow)
        ' Start the enumeration process.
        ReturnValue = OutlookSecurity.EnumChildWindows(hwnd, AddressOf EnumChildWindowProc, IntPtr.Zero)
        ' Return the children list when the process is completed.
        Return ListChildren
    End Function

    ''' <summary>
    ''' Attempt to match the child class, if one was specified, otherwiseenumerate all the child windows.
    ''' </summary>
    ''' <param name="hwnd"></param>
    ''' <param name="lParam"></param>
    Private Sub EnumChildWindowProc(ByVal hwnd As IntPtr, ByVal lParam As Int32)
        ListChildren.Add(GetWindowIdentification(hwnd))
    End Sub

    ''' <summary>
    ''' Build the ApiWindow object to hold information about the Window object.
    ''' Gibt hier das Handle zurück.
    ''' </summary>
    ''' <param name="hwnd"></param>
    Private Function GetWindowIdentification(ByVal hwnd As IntPtr) As ApiWindow
        Dim window As New ApiWindow()
        window.HWnd = CType(hwnd, IntPtr)
        Return window
    End Function

#End If
#End Region

End Class

Friend Class ContactSaved
    Implements IDisposable

    Friend WithEvents ContactSaved As Outlook.ContactItem
    Private C_KF As KontaktFunktionen

    Public Sub New(ByVal KontaktKlasse As KontaktFunktionen)
        C_KF = KontaktKlasse
    End Sub

    Private Sub ContactSaved_Close(ByRef Cancel As Boolean) Handles ContactSaved.Close
        ThisAddIn.ListofOpenContacts.Remove(Me)
        Me.Dispose()
    End Sub

    Private Sub ContactSaved_Write(ByRef Cancel As Boolean) Handles ContactSaved.Write
        If C_KF.IndizierungErforderlich(ContactSaved) Then
            C_KF.IndiziereKontakt(ContactSaved)
        End If
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
