Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Windows.Media
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Office.Interop.Outlook
Imports MixERP.Net.VCards
Friend Module KontaktFunktionen
    Private ReadOnly Property DfltErrorvalue As Integer = -2147221233
    Private ReadOnly Property DfltDASLSMTPAdress As String = "http://schemas.microsoft.com/mapi/proptag/0x39FE001E"
    Private ReadOnly Property DASLTagFBTelBuch As Object() = {$"{DfltDASLSchema}FBDB-PhonebookID", $"{DfltDASLSchema}FBDB-PhonebookEntryID"}.ToArray
    Private ReadOnly Property DASLTagTelNrIndex As Object() = GetType(OutlookContactNumberFields).GetProperties.Select(Function(P) $"{DfltDASLSchema}FBDB-{P.Name}").ToArray

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    ''' <summary>
    ''' Gibt den Kontakt als Objekt zurück. 
    ''' </summary>
    ''' <param name="olKontakt"></param>
    ''' <returns><paramref name="olKontakt"/></returns>
    <Extension> Friend Function Self(olKontakt As ContactItem) As ContactItem
        Return olKontakt
    End Function

#Region "Generieung neuer Kontakte"
    ''' <summary>
    ''' Erstellt einen Kontakt aus einer vCard.
    ''' </summary>
    ''' <param name="vCard">Kontaktdaten im vCard-Format.</param>
    ''' <param name="TelNr">Telefonnummer, die zusätzlich eingetragen werden soll.</param>
    ''' <param name="AutoSave">Gibt an ob der Kontakt gespeichert werden soll True, oder nur angezeigt werden soll False.</param>
    ''' <returns>Den erstellten Kontakt als <see cref="ContactItem"/>.</returns>
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
    ''' <returns>Den erstellten Kontakt als <see cref="ContactItem"/>.</returns>
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

                End If

            End With

            If AutoSave Then SpeichereKontakt(olKontakt)

            Return olKontakt

        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' Erstellt einen Kontakt aus einer Fritz!Box Telefonbucheintrag.
    ''' </summary>
    ''' <param name="XMLKontakt">Kontaktdaten als Fritz!Box Telefonbucheintrag</param>
    ''' <param name="olFolder">Outlook Ordner in dem der Kontakt gespeichert werden soll.</param>
    ''' <param name="FBoxBuchID">Eindeutige ID des Telefonbuches, aus dem der Telefonbucheintrag entnommen wurde.</param>
    ''' <returns>Den erstellten Kontakt als <see cref="ContactItem"/>.</returns>
    Friend Function ErstelleKontakt(XMLKontakt As FBoxAPI.Contact, olFolder As MAPIFolder, FBoxBuchID As Integer) As ContactItem

        Dim olKontakt As ContactItem = CType(Globals.ThisAddIn.Application.CreateItem(OlItemType.olContactItem), ContactItem)

        With olKontakt

            If XMLKontakt IsNot Nothing Then
                XMLKontakt.XMLKontaktOutlook(olKontakt)
                .SetUniqueID(FBoxBuchID.ToString, XMLKontakt.Uniqueid.ToString, False)
                If Not olFolder.AreEqual(GetDefaultMAPIFolder(OlDefaultFolders.olFolderContacts)) Then
                    ' Verschiebe den Kontakt in den gewünschten Ornder
                    olKontakt = CType(olKontakt.Move(olFolder), ContactItem)
                    NLogger.Info($"Kontakt {olKontakt.FullName} wurde erstellt und in den Ordner {olFolder.Name} verschoben.")

                Else
                    ' Speichere den Kontakt im Kontakthauptordner
                    ' Speichern ist überflüssig, da der Kontakt bein nachfolgenden Indizieren/deindizieren ohnehin stets gespeichert wird.

                    'If olKontakt.Speichern Then NLogger.Info($"Kontakt {olKontakt.FullName} wurde Hauptkontaktordner gespeichert.")
                End If

                ' Indizere den Kontakt, wenn der Ordner, in den er gespeichert werden soll, auch zur Kontaktsuche verwendet werden soll
                IndiziereKontakt(olKontakt, olFolder, False)
            End If

        End With

        Return olKontakt

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
#End Region

#Region "Einblenden Kontakte"
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
#End Region

#Region "Speichern"
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

    <Extension> Friend Function Speichern(olKontakt As ContactItem) As Boolean
        Try
            olKontakt.Save()
            Return True
        Catch ex As System.Exception
            NLogger.Error(ex, $"Kontakt {olKontakt.FullNameAndCompany} kann nicht gespeichert werden.")
            Return False
        End Try
    End Function
#End Region

#Region "Ermittlung von Kontakten"
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
            NLogger.Debug($"Outlook Kontakt {GetOutlookKontakt?.FullNameAndCompany.RemoveLineBreaks} aus EntryID und KontaktID ermittelt.")
        Catch ex As System.Exception
            NLogger.Error(ex, $"der Kontakt kann mit der KontaktID '{KontaktID}' und der StoreID '{StoreID}' nicht ermittelt werden.")
        End Try
    End Function

    ''' <summary>
    ''' Ermittelt aus der KontaktID (EntryID) und der StoreID den zugehörigen Kontakt.
    ''' </summary>
    ''' <param name="KontaktIDStoreID">EntryID und StoreID des Kontaktes</param>
    ''' <returns>Erfolg: Kontakt, Misserfolg: Nothing</returns>
    Friend Function GetOutlookKontakt(KontaktIDStoreID As Object()) As ContactItem
        Return If(Not KontaktIDStoreID.Contains(DfltErrorvalue), GetOutlookKontakt(KontaktIDStoreID.First.ToString, KontaktIDStoreID.Last.ToString), Nothing)
    End Function
#End Region

#Region "Kontaktsuche"
    Friend Event Status As EventHandler(Of String)
    Friend Event Beendet As EventHandler(Of NotifyEventArgs(Of Boolean))

#Region "Kontaktsuche DASL in Ordnerauswahl"

    ''' <summary>
    ''' Startet eine Kontaktsuche in allen ausgewählten Kontaktordnern (<see cref="OlDefaultFolders.olFolderContacts"/>) durch.
    ''' <para>Es wird nach einer Telefonnummer gesucht.</para>
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, nach der gesucht werden soll.</param>
    ''' <returns>Kontakt, der zu der Telefonnummer gefunden wurde.</returns>
    Friend Async Function KontaktSucheTelNr(TelNr As Telefonnummer) As Task(Of ContactItem)
        Dim olKontakt As ContactItem = Nothing

        If TelNr IsNot Nothing Then
            PushStatus(LogLevel.Debug, $"Kontaktsuche für {TelNr.Unformatiert} gestartet")

            With Await KontaktSucheNumberField(TelNr.Unformatiert, True)
                If .Any Then
                    olKontakt = .First
                    ' Gib alle anderen wieder Frei
                    .Skip(1).ToList.ForEach(Sub(O) ReleaseComObject(O))
                End If
            End With
        End If

        PushStatus(LogLevel.Debug, $"Kontaktsuche für {TelNr.Unformatiert} beendet.")
        RaiseEvent Beendet(Nothing, New NotifyEventArgs(Of Boolean)(olKontakt IsNot Nothing))

        Return olKontakt
    End Function

    ''' <summary>
    ''' Durchsucht den <paramref name="olOrdner"/>.
    ''' <para>Es wird vorab geprüft, ob es sich um einen Kontaktornder handelt.</para>
    ''' </summary>
    ''' <param name="olOrdner">Der zu durchsuchende Ornder als <see cref="MAPIFolder"/></param>
    ''' <param name="sFilter">Ein Filter in der Syntax für Microsoft Jet oder DAV Searching and Locating (DASL), die die Kriterien für Elemente im übergeordneten Ordner gibt.</param>
    ''' <returns>Auflistung aller zur <paramref name="sFilter"/> passenden Kontakte aus diesem <paramref name="olOrdner"/> als Liste von <seealso cref="ContactItem"/></returns>
    Private Function FindeKontaktInOrdner(olOrdner As MAPIFolder, sFilter As String, ct As CancellationToken) As List(Of ContactItem)

        Dim olKontaktListe As New List(Of ContactItem)

        If olOrdner.DefaultItemType = OlItemType.olContactItem Then

            Dim oTable As Table
            ' Die Suche erfolgt mittels einer gefilterten Outlook-Datentabelle, welche nur passende Kontakte enthalten.
            ' Erstellung der Datentabelle
            oTable = olOrdner.GetTable(sFilter)
            ' Festlegung der Spalten. 
            With oTable.Columns
                ' Zunächst werden alle Spalten entfernt
                .RemoveAll()
                .Add("EntryID")
            End With

            ' Schleife durch alle Tabellenzeilen
            Do Until oTable.EndOfTable Or ct.IsCancellationRequested
                ' Ermittle das zugehörige Kontaktelement und füge sie in die Rückgabeliste
                With oTable.GetNextRow()
                    olKontaktListe.Add(GetOutlookKontakt(.Item("EntryID").ToString, olOrdner.StoreID))
                End With
            Loop

            ' Gibt die Elemente frei
            ReleaseComObject(oTable)
            oTable = Nothing
        End If

        Return olKontaktListe
    End Function

    ''' <summary>
    ''' Führt die Kontaktsuche in allen ausgewählten Kontaktordnern (<see cref="OlDefaultFolders.olFolderContacts"/>) durch.
    ''' </summary>
    ''' <param name="sFilter">Ein Filter in der Syntax für Microsoft Jet oder DAV Searching and Locating (DASL), die die Kriterien für Elemente im übergeordneten Ordner gibt.</param>
    ''' <returns>Auflistung aller zur <paramref name="sFilter"/> passenden Kontakte aus allen gewählten Kontaktorndern als Liste von <seealso cref="ContactItem"/></returns>
    Private Async Function KontaktSucheFilter(sFilter As String, ct As CancellationToken) As Task(Of List(Of ContactItem))
        Dim olKontaktListe As New List(Of ContactItem)

        If Globals.ThisAddIn.Application IsNot Nothing AndAlso sFilter.IsNotStringNothingOrEmpty Then

            NLogger.Trace($"Kontaktsuche mit Filter gestartet: {sFilter}")

            ' Ermittle Ordner
            Dim OrdnerListe As List(Of OutlookOrdner) = XMLData.POptionen.OutlookOrdner.FindAll(OutlookOrdnerVerwendung.KontaktSuche)

            ' Füge den Standardkontaktordner hinzu, falls keine anderen Ordner definiert wurden.
            If Not OrdnerListe.Any Then
                OrdnerListe.Add(New OutlookOrdner(GetDefaultMAPIFolder(OlDefaultFolders.olFolderContacts), OutlookOrdnerVerwendung.KontaktSuche))
            End If

            ' Erzeuge eine Liste aller existierenden Ordner, die der Nutzer ausgewählt hat
            Dim MAPIFolderList As List(Of MAPIFolder) = OrdnerListe.Where(Function(F) F.Exists).Select(Function(S) S.MAPIFolder).ToList

            ' Füge die Unterordner hinzu
            If XMLData.POptionen.CBSucheUnterordner Then
                PushStatus(LogLevel.Debug, "Ermittle Unterordner...")
                AddOutlookChildFolders(MAPIFolderList, OlItemType.olContactItem)
            End If

            PushStatus(LogLevel.Debug, $"Starte die Kontaktsuche in {MAPIFolderList.Count} Ordnern.")

            ' Erzeuge eine neue Liste von Taskobjekten, die eine Liste von Kontaktelementen zurückgeben.
            Dim TaskList As New List(Of Task(Of List(Of ContactItem)))
            ' Führe die Kontaktsuche aus.
            For Each MapiFolder In MAPIFolderList
                ' Füge einen eigenen Task je Ordner hinzu.
                TaskList.Add(Task.Run(Function() As List(Of ContactItem)
                                          PushStatus(LogLevel.Debug, $"Kontaktsuche in MAPIFolder '{MapiFolder.Name}' gestartet")
                                          Return FindeKontaktInOrdner(MapiFolder, sFilter, ct)
                                      End Function, ct))
            Next

            ' Schleife, so lange Tasks in der Liste enthalten sind
            While TaskList.Any And Not ct.IsCancellationRequested

                ' Warte den Abschluss eines Tasks ab.
                Dim t = Await Task.WhenAny(TaskList)

                ' Das Ergebnis ist eine Liste. Wenn Kontakte gefunden wurden, die auf den Filter passen, sind sie darin enthalten
                If t.Result.Any Then
                    ' Füge die gefundenen Kontakte in die Ergebnisliste hinzu.
                    olKontaktListe.AddRange(t.Result)
                    PushStatus(LogLevel.Debug, $"Kontaktsuche erfolgreich: {t.Result.Count} Kontakte in '{t.Result.First.ParentFolder.Name}' gefunden: {String.Join(", ", t.Result.Select(Function(K) K.FullName))}")
                End If

                ' Entferne den Task aus der Liste, da er abgeschlossen ist.
                TaskList.Remove(t)
            End While

            ' Aufräumen
            With OrdnerListe
                .ForEach(Sub(O) O.Dispose())
                .Clear()
            End With
            OrdnerListe = Nothing

            With MAPIFolderList
                .ForEach(Sub(O) ReleaseComObject(O))
                .Clear()
            End With
            MAPIFolderList = Nothing
        End If

        PushStatus(LogLevel.Debug, $"Kontaktsuche beendet.")
        RaiseEvent Beendet(Nothing, New NotifyEventArgs(Of Boolean)(olKontaktListe IsNot Nothing))

        Return olKontaktListe
    End Function

    ''' <summary>
    ''' Gibt eine Statusmeldung (<paramref name="StatusMessage"/>) als Event aus. Gleichzeitig wird in das Log mit vorgegebenem <paramref name="Level"/> geschrieben.
    ''' </summary>
    ''' <param name="Level">NLog LogLevel</param>
    ''' <param name="StatusMessage">Die auszugebende Statusmeldung.</param>
    Private Sub PushStatus(Level As LogLevel, StatusMessage As String)
        NLogger.Log(Level, StatusMessage)
        RaiseEvent Status(Nothing, StatusMessage)
    End Sub
#End Region

#Region "Absendersuche E-Mail"

    ''' <summary>
    ''' Funktion die die Suche mit einer E-Mail durchführt.
    ''' </summary>
    ''' <param name="SMTPAdresse">Mail-Addresse, die als Suchkriterium verwendet wird.</param>
    ''' <returns>Den gefundenen Kontakt als Outlook.ContactItem.</returns>
    Friend Function KontaktSuche(SMTPAdresse As EMailType) As ContactItem

        If SMTPAdresse.Addresse.IsNotStringNothingOrEmpty Then
            ' Empfänger generieren
            Dim Empfänger As Recipient = Globals.ThisAddIn.Application.Session.CreateRecipient(SMTPAdresse.Addresse)

            With Empfänger
                .Resolve()
                Return .AddressEntry.GetContact
            End With

            ' Wichtig: Auflösen, da sonst verzögert versendete E-Mails nicht versendet werden.
            ReleaseComObject(Empfänger)
        Else
            Return Nothing
        End If
    End Function

    Friend Function KontaktSucheExchangeUser(SMTPAdresse As EMailType) As ExchangeUser

        If SMTPAdresse.Addresse.IsNotStringNothingOrEmpty Then
            ' Empfänger generieren
            Dim Empfänger As Recipient = Globals.ThisAddIn.Application.Session.CreateRecipient(SMTPAdresse.Addresse)

            With Empfänger
                .Resolve()
                Return .AddressEntry.GetExchangeUser
            End With

            ' Wichtig: Auflösen, da sonst verzögert versendete E-Mails nicht versendet werden.
            ReleaseComObject(Empfänger)
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Funktion die die Suche mit einer Kontaktkarte durchführt.
    ''' </summary>
    ''' <param name="Kontaktkarte">Kontaktkarte (ContactCard), die als Suchkriterium verwendet wird.</param>
    ''' <returns>Den gefundenen Kontakt als Outlook.ContactItem</returns>
    Friend Function KontaktSuche(Kontaktkarte As Microsoft.Office.Core.IMsoContactCard) As ContactItem

        If Kontaktkarte IsNot Nothing Then

            Select Case Kontaktkarte.AddressType
                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeSMTP
                    ' über Kontaktkarte.Address wird die SMTP-Adresse zurückgegeben
                    Return KontaktSuche(New EMailType With {.Addresse = Kontaktkarte.Address, .OutlookTyp = OutlookEMailType.SMTP})

                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeOutlook
                    Dim Adresseintrag As AddressEntry = Globals.ThisAddIn.Application.Session.GetAddressEntryFromID(Kontaktkarte.Address)

                    If Adresseintrag?.AddressEntryUserType = OlAddressEntryUserType.olOutlookContactAddressEntry Then
                        Return Adresseintrag.GetContact
                    Else
                        Return Nothing
                    End If

                    ReleaseComObject(Adresseintrag)

                Case Else
                    Return Nothing
            End Select
        Else
            Return Nothing
        End If
        ReleaseComObject(Kontaktkarte)

    End Function

    ''' <summary>
    ''' Funktion die die Suche mit einer Kontaktkarte durchführt.
    ''' </summary>
    ''' <param name="Kontaktkarte">Kontaktkarte (ContactCard), die als Suchkriterium verwendet wird.</param>
    ''' <returns>Den gefundenen Kontakt als Outlook.ExchangeUser</returns>
    Friend Function KontaktSucheExchangeUser(Kontaktkarte As Microsoft.Office.Core.IMsoContactCard) As ExchangeUser

        If Kontaktkarte IsNot Nothing Then

            Select Case Kontaktkarte.AddressType
                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeSMTP
                    ' über Kontaktkarte.Address wird die SMTP-Adresse zurückgegeben
                    Return KontaktSucheExchangeUser(New EMailType With {.Addresse = Kontaktkarte.Address, .OutlookTyp = OutlookEMailType.EX})

                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeOutlook
                    Dim Adresseintrag As AddressEntry = Globals.ThisAddIn.Application.Session.GetAddressEntryFromID(Kontaktkarte.Address)

                    Select Case Adresseintrag?.AddressEntryUserType
                        Case OlAddressEntryUserType.olExchangeUserAddressEntry, OlAddressEntryUserType.olExchangeRemoteUserAddressEntry
                            Return Adresseintrag.GetExchangeUser()
                        Case Else
                            Return Nothing
                    End Select

                    ReleaseComObject(Adresseintrag)
                Case Else
                    Return Nothing
            End Select
        Else
            Return Nothing
        End If
        ReleaseComObject(Kontaktkarte)
    End Function

#End Region

#Region "Kontaktsuche Aufrufe"
    ''' <summary>
    ''' Stellt einen Filter für eine Kontaktsuche in den Namensfeldern zusammen und startet die Suche 
    ''' </summary>
    ''' <param name="FilterWert">Zeichenfolge nach der die Kontakte gesucht werden sollen.</param>
    ''' <param name="Exakt">Angabe ob die die Ergebnisse exakt übereinstimmen müssen, oder ob der <paramref name="FilterWert"/> enthalten sein kann. </param>
    ''' <returns>Liste von gefundenen <see cref="ContactItem"/></returns>
    Friend Async Function KontaktSucheNameField(FilterWert As String, Exakt As Boolean, ct As CancellationToken) As Task(Of List(Of ContactItem))
        Dim Filter As New List(Of String)

        ' Standard Outlook Namens Felder 
        If Exakt Then
            ' Exakte Suche 
            Filter.AddRange(GetType(OutlookContactNameFields).GetProperties.Select(Function(P) $"{P.GetValue(Nothing)} = '{FilterWert}'"))
        Else
            ' Zeichenfolge kann enthalten sein
            Filter.AddRange(GetType(OutlookContactNameFields).GetProperties.Select(Function(P) $"{P.GetValue(Nothing)} LIKE '%{FilterWert}%'"))
        End If

        ' Führe die Suche aus
        Return Await KontaktSucheFilter($"@SQL={String.Join(" OR ", Filter)}", ct)
    End Function

    ''' <summary>
    ''' Stellt einen Filter für eine Kontaktsuche in den Telefonnummern zusammen und startet die Suche 
    ''' </summary>
    ''' <param name="FilterWert">Zeichenfolge nach der die Kontakte gesucht werden sollen.</param>
    ''' <param name="Exakt">Angabe ob die die Ergebnisse exakt übereinstimmen müssen, oder ob der <paramref name="FilterWert"/> enthalten sein kann. </param>
    ''' <returns>Liste von gefundenen <see cref="ContactItem"/></returns>
    Friend Async Function KontaktSucheNumberField(FilterWert As String, Exakt As Boolean) As Task(Of List(Of ContactItem))
        Dim Filter As New List(Of String)

        If Exakt Then
            ' Exakte Suche 
            ' Standard Outlook Nummern Felder (wird nicht benötigt, da die indizierten Felder durchsucht werden sollen)
            ' Filter.AddRange(GetType(OutlookContactNumberFields).GetProperties.Select(Function(P) $"{P.GetValue(Nothing)} = '{FilterWert}'"))

            ' Indizierte Telefonnummernfelder hinzufügen
            Filter.AddRange(GetType(OutlookContactNumberFields).GetProperties.Select(Function(P) $"""{DfltDASLSchema}FBDB-{P.Name}/0x0000001f"" = '{FilterWert}'"))

        Else
            ' Zeichenfolge kann enthalten sein

            ' Standard Outlook Nummern Felder 
            Filter.AddRange(GetType(OutlookContactNumberFields).GetProperties.Select(Function(P) $"{P.GetValue(Nothing)} LIKE '%{FilterWert}%'"))

            ' Indizierte Telefonnummernfelder hinzufügen
            Filter.AddRange(GetType(OutlookContactNumberFields).GetProperties.Select(Function(P) $"""{DfltDASLSchema}FBDB-{P.Name}/0x0000001f"" LIKE '%{FilterWert}%'"))
        End If

        ' Führe die Suche aus
        Return Await KontaktSucheFilter($"@SQL={String.Join(" OR ", Filter)}", Nothing)
    End Function

    ''' <summary>
    ''' Stellt einen Filter für eine Kontaktsuche in den E-Mailfeldern zusammen und startet die Suche 
    ''' </summary>
    ''' <param name="FilterWert">Zeichenfolge nach der die Kontakte gesucht werden sollen.</param>
    ''' <param name="Exakt">Angabe ob die die Ergebnisse exakt übereinstimmen müssen, oder ob der <paramref name="FilterWert"/> enthalten sein kann. </param>
    ''' <returns>Liste von gefundenen <see cref="ContactItem"/></returns>
    Friend Async Function KontaktSucheEMailField(FilterWert As String, Exakt As Boolean) As Task(Of List(Of ContactItem))
        Dim Filter As New List(Of String)

        ' Standard Outlook E-Mail Felder 

        If Exakt Then
            ' Exakte Suche 
            Filter.AddRange(GetType(OutlookContactEMailFields).GetProperties.Select(Function(P) $"{P.GetValue(Nothing)} = '{FilterWert}'"))
        Else
            ' Zeichenfolge kann enthalten sein
            Filter.AddRange(GetType(OutlookContactEMailFields).GetProperties.Select(Function(P) $"{P.GetValue(Nothing)} LIKE '%{FilterWert}%'"))
        End If

        ' Führe die Suche aus
        Return Await KontaktSucheFilter($"@SQL={String.Join(" OR ", Filter)}", Nothing)
    End Function
#End Region
#End Region

#Region "Indizierung"

    ''' <summary>
    ''' Indiziert oder deindiziert ein Kontaktelement, ne nach dem, ob der Ordner für die Kontaktsuche ausgewählt wurde
    ''' </summary>
    ''' <param name="olKontakt">Der Kontakt der indiziert werden soll.</param>
    ''' <param name="olOrdner">Der Ordner in dem Der Kontakt gespeichert werden soll.</param>
    ''' <param name="RCO">Angabe, ob der indizierte Kontakte freigegeben werden soll. <see cref="ReleaseComObject"/></param>
    Friend Sub IndiziereKontakt(olKontakt As ContactItem, olOrdner As MAPIFolder, RCO As Boolean)

        ' Wird der Zielordner für, die Kontaktsuche verwendet?
        If olOrdner.OrdnerAusgewählt(OutlookOrdnerVerwendung.KontaktSuche) Then
            ' Indiziere den Kontakt
            IndiziereKontakt(olKontakt)

        Else
            ' Deindiziere den Kontakt
            DeIndiziereKontakt(olKontakt)

        End If

        If RCO Then
            ReleaseComObject(olKontakt)
            ReleaseComObject(olOrdner)
        End If

    End Sub

    ''' <summary>
    ''' Indiziert ein Kontaktelement.
    ''' </summary>
    ''' <param name="olKontakt">Der Kontakt der indiziert werden soll.</param>
    Friend Sub IndiziereKontakt(olKontakt As ContactItem)

        With olKontakt

            NLogger.Trace($"Indizierung des Kontaktes { .FullNameAndCompany} gestartet.")

            Dim colArgs As Object()
            ' Lade alle Telefonnummern des Kontaktes
            ' Das Laden der Telefonnummern mittels PropertyAccessor ist nicht sinnvoll.
            ' Die Daten liegen darin erst nach dem Speichern des Kontaktes vor.
            ' colArgs = CType(.PropertyAccessor.GetProperties(DASLTagTelNrIndex), Object())
            ' Die Telefonnummern werden stattdessen aus den Eigenschaften des Kontaktes direkt ausgelesen.

            ' Entferne alle Formatierungen der Telefonnummern
            colArgs = .GetTelNrArray.Select(Of Object)(Function(N) If(N IsNot Nothing, New Telefonnummer() With {.SetNummer = N.ToString}.Unformatiert, String.Empty)).ToArray

            ' Lösche alle Indizierungsfelder
            .PropertyAccessor.DeleteProperties(DASLTagTelNrIndex)

            ' Speichere die Nummern und nicht sichtbare Felder
            Try
                .PropertyAccessor.SetProperties(DASLTagTelNrIndex, colArgs)
            Catch ex As System.Exception
                NLogger.Error(ex, $"Kontakt: { .FullNameAndCompany}")
            End Try

            ' colArgs = CType(.PropertyAccessor.GetProperties(DASLTagTelNrIndex), Object())

            If .Speichern Then NLogger.Debug($"Indizierung des Kontaktes { .FullNameAndCompany.RemoveLineBreaks} abgeschlossen.")

        End With
    End Sub

    ''' <summary>
    ''' Entfernt alle Indizierungseinträge aus einem Kontaktelement.
    ''' </summary>
    ''' <param name="olKontakt">Der Kontakt der deindiziert werden soll.</param>
    Friend Sub DeIndiziereKontakt(olKontakt As ContactItem)

        With olKontakt
            ' Lösche alle Indizierungsfelder
            .PropertyAccessor.DeleteProperties(DASLTagTelNrIndex)

            If .Speichern Then NLogger.Debug($"Deindizierung des Kontaktes { .FullNameAndCompany.RemoveLineBreaks} abgeschlossen.")
        End With
    End Sub

    ''' <summary>
    ''' Erstellt ein Dictionary aller indizierten Telefonnummern. Key ist die englisch-sprachige Bezeichnung des Eintrages.
    ''' </summary>
    ''' <param name="olKontakt">Aktueller Kontakt</param>
    ''' <returns>Dictionary aller indizierten Telefonnummern</returns>
    Friend Function GetIndexList(olKontakt As ContactItem) As Dictionary(Of String, String)
        With olKontakt
            Dim colArgs As Object() = CType(.PropertyAccessor.GetProperties(DASLTagTelNrIndex), Object())
            Dim Text As List(Of String) = GetType(OutlookContactNumberFields).GetProperties.Select(Function(P) P.Name).ToList

            ' Stellt eine Zuordnung zwichen der Nummernbezeichnung und dem Key sowie der Nummer und des Values her.
            ' Im zweiten schritt werden alle elemente rausgefiltert, die leer sind.
            Return Text.ToDictionary(Function(i) Text(Text.IndexOf(i)), Function(i) colArgs(Text.IndexOf(i)).ToString) _
                       .Where(Function(i) i.Value.IsNotStringNothingOrEmpty AndAlso i.Value.IsNotEqual(DfltErrorvalue.ToString)) _
                       .ToDictionary(Function(i) i.Key, Function(i) i.Value)
        End With
    End Function

#End Region

#Region "Synchronisation"
    ''' <summary>
    ''' Synchronisiert einen Kontaktordner <paramref name="OutlookOrdner"/> mit einem Fritz!Box Telefonbuch (<paramref name="FBoxTBuch"/>)
    ''' </summary>
    ''' <param name="OutlookOrdner">Der zu synchrinisierende Outlook Ordner</param>
    ''' <param name="FBoxTBuch">Das zu synchrinisierende Fritz!Box Telefonbuch</param>
    ''' <param name="Modus">Der Synchronisationsmodus. Hier wird festgelegt, in welche Richtung die Daten bei Änderungen verschoben werden.</param>
    ''' <param name="ct">CancellationToken zum Abbruch der Routine</param>
    ''' <param name="Progress">Anbieter für Statusupdates</param>
    Friend Async Function Synchronisierer(OutlookOrdner As MAPIFolder, FBoxTBuch As PhonebookEx, Modus As SyncMode, ct As CancellationToken, Progress As IProgress(Of String)) As Task(Of Integer)

        Dim VerarbeiteteKontakte As Integer = 0

        'Dim TaskList As New List(Of Task(Of String))
        Dim TaskList As New List(Of Task)

        Dim FBKontakte As New List(Of FBoxAPI.Contact)
        FBKontakte.AddRange(FBoxTBuch.GetContacts)

        ' Schleife durch jedes Element dieses Ordners. 
        For Each Item In OutlookOrdner.Items

            Select Case True
                ' Unterscheidung je nach Datentyp
                Case TypeOf Item Is ContactItem

                    'Dim aktKontakt As ContactItem = CType(Item, ContactItem)

                    ' Synchronisiere Kontakt
                    With CType(Item, ContactItem)
                        Dim uID As Integer = .GetUniqueID(FBoxTBuch.ID)
                        If uID.AreEqual(-1) Then
                            Progress?.Report($"Kontakt '{ .FullName}' auf der Fritz!Box erzeugt ...")
                            ' Es gibt keinen Kontakt auf der Fritz!Box
                            TaskList.Add(Task.Run(Sub() Telefonbücher.SetTelefonbuchEintrag(FBoxTBuch.ID, .Self)))
                        Else
                            ' Es gibt einen Kontakt im Fritz!Box Telefonbuch
                            Dim FBoxKontakt As FBoxAPI.Contact = FBoxTBuch.GetContact(uID)
                            ' Gibt es in dem Telefonbuch einen Kontakt mit der ID
                            If FBoxKontakt IsNot Nothing Then
                                ' Ja ... Abgleich
                                If Not .IsEqual(FBoxKontakt) Then
                                    Select Case Modus
                                        Case SyncMode.OutlookToFritzBox
                                            Progress?.Report($"Kontakt '{ .FullName}' auf der Fritz!Box überschrieben (uID {FBoxKontakt.Uniqueid}) ...")

                                            ' Kontakt auf der Fritz!Box ersetzen
                                            TaskList.Add(Task.Run(Sub() Telefonbücher.SetTelefonbuchEintrag(FBoxTBuch.ID, .Self)))

                                        Case SyncMode.FritzBoxToOutlook
                                            Progress?.Report($"Kontakt '{ .FullName}' in Outlook überschrieben (uID {FBoxKontakt.Uniqueid}) ...")

                                            TaskList.Add(Task.Run(Sub() ÜberschreibeKontakt(.Self, FBoxKontakt)))

                                    End Select

                                End If
                            Else
                                ' Nein ... Kontakt wurde auf der Fritz!Box gelöscht?
                                Progress?.Report($"Kontakt '{ .FullName}' im Outlook gelöscht ...")
                                .Delete()

                            End If

                            ' Entferne den Kontakt aus dem heruntergeladenen Telefonbuch
                            FBKontakte.Remove(FBoxKontakt)
                        End If

                        ' Erhöhe Wert für Progressbar und schreibe einen Status
                        Progress?.Report($"Kontakt '{ .FullName}' abgeschlossen ...")
                    End With

                    'aktKontakt = Nothing

                Case TypeOf Item Is AddressList ' Adressliste
                    With CType(Item, AddressList)
                        Progress?.Report($"Adressliste '{ .Name}' übergangen ...")
                    End With

                Case TypeOf Item Is DistListItem ' Verteilerliste
                    With CType(Item, DistListItem)
                        Progress?.Report($"Verteilerliste '{ .DLName}' übergangen ...")
                    End With

                Case Else
                    Progress?.Report($"Unbekanntes Objekt übergangen ...")

            End Select

            ' Die einzelnen Vorgänge müssen nacheinander erfolgen, da es sonst zu einer WebException kommt: Die zugrunde liegende Verbindung wurde geschlossen: Für den geschützten SSL/TLS-Kanal konnte keine Vertrauensstellung hergestellt werden.
            If TaskList.Any Then Await TaskList.Last

            ReleaseComObject(Item)
            ' Frage Cancelation ab
            If ct.IsCancellationRequested Then Exit For

            VerarbeiteteKontakte += 1
        Next

        ' Alle Kontakte, welche jetzt noch im Telefonbuch sind, müssen im Outlook angelegt werden
        For Each FBoxKontakt In FBKontakte
            Select Case Modus
                Case SyncMode.OutlookToFritzBox
                    With FBoxKontakt
                        Telefonbücher.DeleteTelefonbuchEintrag(FBoxTBuch.ID, .Uniqueid)
                        Progress?.Report($"Kontakt '{ .Person.RealName}' auf der Fritz!Box gelöscht (uID { .Uniqueid}) ...")
                    End With

                Case SyncMode.FritzBoxToOutlook

                    With ErstelleKontakt(FBoxKontakt, OutlookOrdner, FBoxTBuch.ID)
                        Progress?.Report($"Kontakt '{ .FullName}' in Outlook erzeugt (uID {FBoxKontakt.Uniqueid}) ...")
                    End With

            End Select
            ' Frage Cancelation ab
            If ct.IsCancellationRequested Then Exit For

            VerarbeiteteKontakte += 1
        Next

        Return VerarbeiteteKontakte
    End Function

    ''' <summary>
    ''' Synchronisiert einen einzelnen Outlook Kontakt <paramref name="olContact"/> mit einem Fritz!Box Telefonbuch.
    ''' </summary>
    ''' <param name="olContact">Der zu synchrinisierende Outlook Kontakt</param>
    ''' <param name="olFolder">Der Ordner in dem sich der zu synchrinisierende Outlook Kontakt befindet</param>
    <Extension> Friend Async Sub Synchronisierer(olContact As ContactItem, olFolder As MAPIFolder)

        Dim olOrdner As OutlookOrdner = XMLData.POptionen.OutlookOrdner.Find(olFolder, OutlookOrdnerVerwendung.FBoxSync)

        If olOrdner?.FBoxSyncOptions?.ValidData IsNot Nothing Then

            ' Synchronisiere Kontakt

            With olContact
                Dim uID As Integer = .GetUniqueID(olOrdner.FBoxSyncOptions.FBoxSyncID)
                If uID.AreEqual(-1) Then
                    NLogger.Info($"Kontakt '{ .FullName}' auf der Fritz!Box erzeugt ...")
                    ' Es gibt keinen Kontakt auf der Fritz!Box
                    Telefonbücher.SetTelefonbuchEintrag(olOrdner.FBoxSyncOptions.FBoxSyncID, olContact)
                Else
                    ' Es gibt einen Kontakt im Fritz!Box Telefonbuch
                    Dim FBoxKontakt As FBoxAPI.Contact = Await Telefonbücher.GetTelefonbuchEintrag(olOrdner.FBoxSyncOptions.FBoxSyncID, uID)
                    ' Gibt es in dem Telefonbuch einen Kontakt mit der ID
                    If FBoxKontakt IsNot Nothing Then
                        ' Ja ... Abgleich
                        If Not .IsEqual(FBoxKontakt) Then

                            NLogger.Info($"Kontakt '{ .FullName}' auf der Fritz!Box überschrieben (uID {FBoxKontakt.Uniqueid}) ...")

                            ' Kontakt auf der Fritz!Box ersetzen
                            Telefonbücher.SetTelefonbuchEintrag(olOrdner.FBoxSyncOptions.FBoxSyncID, olContact)

                        End If
                    Else
                        ' Nein ... Kontakt wurde auf der Fritz!Box gelöscht?
                        NLogger.Info($"Kontakt '{ .FullName}' im Outlook gelöscht ...")
                        .Delete()

                    End If

                End If
            End With
        End If
    End Sub

    ''' <summary>
    ''' Löscht einen einzelnen olContact auf dem Fritz!Box Telefonbuch, wenn der Kontakt im Outlook gelöscht wurde.
    ''' </summary>
    ''' <param name="olContact">Der zu löschende Kontakt.</param>
    <Extension> Friend Sub SyncDelete(olContact As ContactItem)
        Dim olOrdner As OutlookOrdner = XMLData.POptionen.OutlookOrdner.Find(olContact.ParentFolder, OutlookOrdnerVerwendung.FBoxSync)

        If olOrdner?.FBoxSyncOptions?.ValidData IsNot Nothing Then

            ' Lösche Kontakt auf der Fritz!Box (wenn vorhanden)

            With olContact
                Dim uID As Integer = .GetUniqueID(olOrdner.FBoxSyncOptions.FBoxSyncID)
                If Not uID.AreEqual(-1) Then
                    ' Es gibt einen Kontakt im Fritz!Box Telefonbuch
                    Telefonbücher.DeleteTelefonbuchEintrag(olOrdner.FBoxSyncOptions.FBoxSyncID, uID)
                    NLogger.Info($"Kontakt '{olContact.FullName}' auf der Fritz!Box gelöscht (uID { uID}) ...")
                End If
            End With
        End If
    End Sub

    ''' <summary>
    ''' Startet die automatische Synchronisation bei Outlook-Start.
    ''' </summary>
    Friend Async Sub StartAutoSync()
        Dim progressIndicator = New Progress(Of String)(Sub(status)
                                                            NLogger.Info(status)
                                                        End Sub)

        Dim TaskList As New List(Of Task(Of Integer))

        For Each Ordner In XMLData.POptionen.OutlookOrdner.FindAll(OutlookOrdnerVerwendung.FBoxSync).Where(Function(O) (O.FBoxSyncOptions.FBoxCBSyncStartUp))

            Dim FBoxTelefonbuch As PhonebookEx = Globals.ThisAddIn.PhoneBookXML.Where(Function(TB) TB.ID.AreEqual(Ordner.FBoxSyncOptions.FBoxSyncID)).First
            If FBoxTelefonbuch IsNot Nothing Then

                If Not FBoxTelefonbuch.NurName OrElse Not Await FBoxTelefonbuch.UpdatePhonebook() Then
                    NLogger.Info($"Starte die automatische Syncronisation des Outlook-Ordners {Ordner.Name} mit {FBoxTelefonbuch.Name}")

                    TaskList.Add(Task.Run(Function() Synchronisierer(Ordner.MAPIFolder, FBoxTelefonbuch, SyncMode.FritzBoxToOutlook, Nothing, progressIndicator)))
                End If
            End If

            ' Die einzelnen Vorgänge müssen nacheinander erfolgen, da es sonst zu einer WebException kommt: Die zugrunde liegende Verbindung wurde geschlossen: Für den geschützten SSL/TLS-Kanal konnte keine Vertrauensstellung hergestellt werden.
            If TaskList.Any Then Await TaskList.Last
        Next
        NLogger.Info($"Automatische Syncronisation abgeschlossen: {(Await Task.WhenAll(TaskList)).Sum}")

    End Sub
#End Region

#Region "MAPIFolder"
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

    ''' <summary>
    ''' Rekursive Funktion zur Ermittlung aller Unterordner eines <see cref="MAPIFolder"/>, welche einen definierten <see cref="OlItemType"/> entsprechen.
    ''' </summary>
    ''' <param name="BaseFolder">Basisorder, aus dem die Ordner ermittelt werden.</param>
    ''' <param name="ItemType">Der Typ der enthaltenen Items</param>
    ''' <returns>Auflistung aller Unterordner</returns>
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

    ''' <summary>
    ''' Fügt alle Unterordner eines <see cref="MAPIFolder"/>, welche einen definierten <see cref="OlItemType"/> entsprechen,  in eine Liste ein.
    ''' </summary>
    ''' <param name="MAPIFolderList">Auflistung der Outlook Ordner.</param>
    ''' <param name="ItemType">Der Typ der enthaltenen Items</param>
    Friend Sub AddOutlookChildFolders(MAPIFolderList As List(Of MAPIFolder), ItemType As OlItemType)
        Dim MAPIFolderChildList As New List(Of MAPIFolder)

        For Each MapiFolder In MAPIFolderList
            MAPIFolderChildList.AddRange(GetOutlookChildFolders(MapiFolder, ItemType))
        Next
        MAPIFolderList.AddRange(MAPIFolderChildList)
    End Sub

    ''' <summary>
    ''' Gibt den <see cref="MAPIFolder"/>, in dem sich der Kontakt befindet, zurück.
    ''' </summary>
    ''' <param name="olKontakt">Outlook Kontakt</param>
    <Extension> Friend Function ParentFolder(olKontakt As ContactItem) As MAPIFolder
        If olKontakt.Parent IsNot Nothing Then
            Return CType(olKontakt.Parent, MAPIFolder)
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Gibt die StoreID des <see cref="MAPIFolder"/>, in dem sich der Kontakt befindet, zurück.
    ''' </summary>
    ''' <param name="olKontakt">Outlook Kontakt</param>
    <Extension> Friend Function StoreID(olKontakt As ContactItem) As String
        Return CType(olKontakt.Parent, MAPIFolder).StoreID
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

#End Region

#Region "E-Mail / Absender"
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
#End Region

#Region "Auflistung von Telefonnummern"
#Region "ContactItem"
    ''' <summary>
    ''' Angabe, ob ein Kontakt Telefonnummern hat.
    ''' </summary>
    ''' <param name="olKontakt">Outlook Kontakt</param>
    ''' <param name="MitFax">Angabe, ob Fax-Nummern berücksichtigt werden sollen.</param>
    <Extension> Friend Function HatKontaktTelefonnummern(olKontakt As ContactItem, MitFax As Boolean) As Boolean
        With olKontakt.GetTelNrArray.ToList
            Return .Where(Function(N) N IsNot Nothing AndAlso .IndexOf(N).IsLessOrEqual(If(MitFax, 13, 18))).Any
        End With
    End Function

    ''' <summary>
    ''' Erstellt eine Liste aller Telefonnummern. 
    ''' </summary>
    ''' <param name="olKontakt">Aktueller Kontakt</param>
    ''' <param name="MitFax">Angabe, ob Fax-Nummern berücksichtigt werden sollen.</param>
    ''' <returns>Liste aller Telefonnummern</returns>
    <Extension> Friend Function GetTelNrList(olKontakt As ContactItem, MitFax As Boolean) As List(Of Telefonnummer)

        Dim TelNrArray As List(Of Object) = olKontakt.GetTelNrArray.ToList
        Return TelNrArray.Where(Function(N) N IsNot Nothing AndAlso TelNrArray.IndexOf(N).IsLessOrEqual(If(MitFax, 13, 18))) _
                         .Select(Function(S) New Telefonnummer With {.SetNummer = S.ToString,
                                                                     .Typ = New TelNrType With {.TelNrType = CType(TelNrArray.IndexOf(S), OutlookNrType)}}).ToList

    End Function

    ''' <summary>
    ''' Erstellt ein Dictionary aller Telefonnummern. Key ist die englisch-sprachige Bezeichnung des Eintrages.
    ''' </summary>
    ''' <param name="olKontakt">Aktueller Kontakt</param>
    ''' <param name="MitFax">Angabe, ob Fax-Nummern berücksichtigt werden sollen.</param>
    ''' <returns>Dictionary aller Telefonnummern</returns>
    Friend Function GetTelNrDictionary(olKontakt As ContactItem, MitFax As Boolean) As Dictionary(Of String, String)
        Dim Text As List(Of String) = GetType(OutlookContactNumberFields).GetProperties.Select(Function(P) P.Name).ToList

        ' Stellt eine Zuordnung zwichen der Nummernbezeichnung und dem Key sowie der Nummer und des Values her.
        ' Im zweiten schritt werden alle Elemente rausgefiltert, die leer sind.
        Return Text.ToDictionary(Function(i) Text(Text.IndexOf(i)), Function(i) olKontakt.GetTelNrArray(Text.IndexOf(i)).ToString) _
                   .Where(Function(i) i.Value.IsNotStringNothingOrEmpty AndAlso Text.IndexOf(i.Key).IsLessOrEqual(If(MitFax, 13, 18))) _
                   .ToDictionary(Function(i) i.Key, Function(i) i.Value)

    End Function

    ''' <summary>
    ''' Erstellt ein <see cref="Object"/>-Array aller Telefonnummern.
    ''' </summary>
    ''' <param name="olKontakt">Aktueller Kontakt</param>
    ''' <returns>Array aller Telefonnummern</returns>
    <Extension> Friend Function GetTelNrArray(olKontakt As ContactItem) As Object()

        Dim tmpTelNr(18) As Object
        With olKontakt
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

#End Region

#Region "ExchangeUser"
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

    <Extension> Friend Function HatKontaktTelefonnummern(olExchangeNutzer As ExchangeUser) As Boolean
        With olExchangeNutzer
            Return .BusinessTelephoneNumber.IsNotStringNothingOrEmpty Or .MobileTelephoneNumber.IsNotStringNothingOrEmpty
        End With
    End Function
#End Region
#End Region

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

#Region "Rückwärtssuche"
    ''' <summary>
    ''' Startet die Rückwärtssuche über die übergebene Telefonnumer und aktualisiere den übergebenen Kontakt.
    ''' </summary>
    ''' <param name="olKontakt">Zu aktualisierender Kontakt</param>
    ''' <param name="TelNr">Telefonnummer</param>
    Friend Async Sub StartKontaktRWS(olKontakt As ContactItem, TelNr As Telefonnummer)

        Dim vCard As String

        vCard = Await StartRWS(TelNr, False)

        If vCard.IsStringNothingOrEmpty Then
            AddinMsgBox($"{Localize.LocAnrMon.strJournalRWSFehler} {TelNr.Formatiert}", MsgBoxStyle.Information, Localize.LocOptionen.strSearchContactHeadRWS)
        Else
            If Not XMLData.POptionen.CBNoContactNotes Then
                olKontakt.Body += String.Format($"{vbCrLf & vbCrLf}{Localize.LocAnrMon.strJournalTextvCard}{vCard}")
            End If

            DeserializevCard(vCard, olKontakt)
        End If

    End Sub

    ''' <summary>
    ''' Fügt die Informationen einer vCard in ein Kontaktelement ein.<br/>
    ''' <see href="https://github.com/mixerp/MixERP.Net.VCards"/><br/>
    ''' <see href="https://www.ietf.org/rfc/rfc2426.txt"/>
    ''' </summary>
    ''' <param name="vCard">Quelle: Die vCard, die eingelesen werden soll.</param>
    ''' <param name="Kontakt">Ziel: (Rückgabe) Der Kontakt in den die Informationen der vCard geschrieben werden als <see cref="ContactItem"/></param>
    Friend Sub DeserializevCard(vCard As String, ByRef Kontakt As ContactItem)

        Dim vc As VCard = Nothing

        Try
            vc = Deserializer.GetVCard(vCard)
        Catch ex As System.Exception
            NLogger.Warn(ex)
        End Try

        If vc IsNot Nothing Then
            With vc
                ' insert Name
                Kontakt.FirstName = .FirstName
                Kontakt.LastName = .LastName
                Kontakt.Title = .Title
                Kontakt.Suffix = .Suffix
                Kontakt.NickName = .NickName
                ' insert Jobtitle and Companny
                Kontakt.JobTitle = .Title
                Kontakt.CompanyName = .Organization
                ' insert Telephone Numbers
                For Each vCardTelephone As Models.Telephone In .Telephones
                    Using tmpTelNr As New Telefonnummer
                        tmpTelNr.SetNummer = vCardTelephone.Number

                        Select Case vCardTelephone.Type
                            Case Types.TelephoneType.Bbs
                            ' bulletin board system telephone number
                            Case Types.TelephoneType.Car
                                Kontakt.CarTelephoneNumber = tmpTelNr.Formatiert
                            Case Types.TelephoneType.Cell
                                ' cellular telephone number
                                Kontakt.MobileTelephoneNumber = tmpTelNr.Formatiert
                            Case Types.TelephoneType.Fax
                                ' facsimile telephone number
                                Kontakt.BusinessFaxNumber = tmpTelNr.Formatiert
                            Case Types.TelephoneType.Home
                                ' telephone number associated with a residence
                                Kontakt.HomeTelephoneNumber = tmpTelNr.Formatiert
                            Case Types.TelephoneType.Isdn
                                ' ISDN service telephone number
                                Kontakt.ISDNNumber = tmpTelNr.Formatiert
                            Case Types.TelephoneType.Message
                                ' telephone number has voice messaging support
                                Kontakt.CallbackTelephoneNumber = tmpTelNr.Formatiert
                            Case Types.TelephoneType.Modem
                                ' MODEM connected telephone number
                                Kontakt.Home2TelephoneNumber = tmpTelNr.Formatiert
                            Case Types.TelephoneType.Pager
                                ' paging device telephone number
                                Kontakt.PagerNumber = tmpTelNr.Formatiert
                            Case Types.TelephoneType.Personal
                                ' personal communication services telephone number
                                Kontakt.OtherTelephoneNumber = tmpTelNr.Formatiert
                            Case Types.TelephoneType.Preferred
                                ' preferred-use telephone number
                                Kontakt.PrimaryTelephoneNumber = tmpTelNr.Formatiert
                            Case Types.TelephoneType.Video
                                ' video conferencing telephone number
                                Kontakt.Business2TelephoneNumber = tmpTelNr.Formatiert
                            Case Types.TelephoneType.Voice
                                ' voice telephone number
                                Kontakt.CompanyMainTelephoneNumber = tmpTelNr.Formatiert
                            Case Types.TelephoneType.Work
                                ' telephone number associated with a place of work
                                Kontakt.BusinessTelephoneNumber = tmpTelNr.Formatiert
                        End Select
                    End Using
                Next
                ' insert Birthday
                If .BirthDay IsNot Nothing Then
                    Kontakt.Birthday = CDate(.BirthDay)
                End If

                ' insert addresses
                If .Addresses IsNot Nothing Then
                    For Each vCardAddress As Models.Address In .Addresses
                        Select Case vCardAddress.Type

                            Case Types.AddressType.Home
                                Kontakt.HomeAddressCity = vCardAddress.Locality
                                Kontakt.HomeAddressCountry = vCardAddress.Country
                                Kontakt.HomeAddressStreet = vCardAddress.Street
                                Kontakt.HomeAddressState = vCardAddress.Region
                                Kontakt.HomeAddressPostalCode = vCardAddress.PostalCode
                                Kontakt.HomeAddressPostOfficeBox = vCardAddress.PoBox

                            Case Else
                                Kontakt.BusinessAddressCity = vCardAddress.Locality
                                Kontakt.BusinessAddressCountry = vCardAddress.Country
                                Kontakt.BusinessAddressStreet = vCardAddress.Street
                                Kontakt.BusinessAddressState = vCardAddress.Region
                                Kontakt.BusinessAddressPostalCode = vCardAddress.PostalCode
                                Kontakt.BusinessAddressPostOfficeBox = vCardAddress.PoBox

                        End Select
                    Next
                End If

                ' insert email-addresses
                If .Emails IsNot Nothing Then
                    For Each vCardEMail As Models.Email In .Emails
                        If Kontakt.Email1Address.IsStringNothingOrEmpty Then Kontakt.Email1Address = vCardEMail.EmailAddress
                        If Kontakt.Email2Address.IsStringNothingOrEmpty Then Kontakt.Email2Address = vCardEMail.EmailAddress
                        If Kontakt.Email3Address.IsStringNothingOrEmpty Then Kontakt.Email3Address = vCardEMail.EmailAddress
                    Next
                End If
                ' insert URL
                If .Url IsNot Nothing Then
                    Kontakt.WebPage = .Url.OriginalString
                End If

            End With

        End If

    End Sub
#End Region

#Region "Interaktionen für Fritz!Box Telefonbücher"
    ''' <summary>
    ''' Überführt einen Outlook-Kontakt in einen Kontakt für das Fritz!Box Telefonbuch
    ''' </summary>
    ''' <param name="olKontakt">Der Outlook Kontakt, der überführt werden soll.</param>
    ''' <param name="UID">Falls bekannt, die Uniqueid des Kontaktes im Fritz!Box Telefonbuch.</param>
    ''' <returns>Ein XML-Objekt für das Fritz!Box Telefonbuch.</returns>
    <Extension> Friend Function ErstelleFBoxKontakt(olKontakt As ContactItem, Optional UID As Integer = -1) As FBoxAPI.Contact

        ' Erstelle ein nen neuen XMLKontakt
        Dim XMLKontakt As FBoxAPI.Contact = CreateContact(Localize.resCommon.strOhneName)

        With XMLKontakt
            ' Weise den Namen zu
            If olKontakt.FullName.IsStringNothingOrEmpty Then
                .Person.RealName = If(olKontakt.CompanyName.IsStringNothingOrEmpty, Localize.resCommon.strOhneName, olKontakt.CompanyName)
            Else
                .Person.RealName = olKontakt.FullName
            End If

            If UID.AreDifferentTo(-1) Then .Uniqueid = UID

            With .Telephony
                ' Weise die E-Mails zu
                With .Emails
                    If olKontakt.Email1Address.IsNotStringNothingOrEmpty Then
                        .Add(New FBoxAPI.Email With {.EMail = olKontakt.Email1Address})
                    End If
                    If olKontakt.Email2Address.IsNotStringNothingOrEmpty Then
                        .Add(New FBoxAPI.Email With {.EMail = olKontakt.Email2Address})
                    End If
                    If olKontakt.Email3Address.IsNotStringNothingOrEmpty Then
                        .Add(New FBoxAPI.Email With {.EMail = olKontakt.Email3Address})
                    End If
                End With

                ' Weise die Telefonnummern zu
                With .Numbers
                    For Each TelNr In GetTelNrList(olKontakt, True)
                        .Add(New FBoxAPI.NumberType With {.Number = TelNr.Unformatiert, .Type = TelNr.Typ.XML})
                    Next
                End With

            End With

        End With
        Return XMLKontakt
    End Function

    ''' <summary>
    ''' Überführt einen Outlook-Kontakt in einen Kontakt für das Fritz!Box Telefonbuch als Strings. 
    ''' </summary>
    ''' <param name="olKontakt">Der Outlook-Kontakt als <see cref="ContactItem"/></param>
    ''' <param name="UID">Falls bekannt, die Uniqueid des Kontaktes im Fritz!Box Telefonbuch.</param>
    ''' <returns>Eine Zeichenfolge die ein XML-Eintrag für das Fritz!Box Telefonbuch enthält.</returns>
    ''' <remarks>Die Auflistung kann leere Strings enthalten.</remarks>
    <Extension> Friend Function ErstelleXMLKontakt(olKontakt As ContactItem, Optional UID As Integer = -1) As String

        Dim NeuerKontakt As String = String.Empty
        If XmlSerializeToString(olKontakt.ErstelleFBoxKontakt(UID), NeuerKontakt) Then
            Return NeuerKontakt
        Else
            NLogger.Warn($"Der Kontakt {olKontakt.FullNameAndCompany} kann nicht serialisiert werden.")
        End If
        Return String.Empty
    End Function

    ''' <summary>
    ''' Vergleichsfunktion um einen Outlook-Kontakt mit einem Fritz!Box Telefonbucheintrag abzugleichen.
    ''' </summary>
    <Extension> Friend Function IsEqual(olKontakt As ContactItem, FBoxContact As FBoxAPI.Contact) As Boolean

        ' Erzeuge aus dem Outlook-Kontakt einen weiteren Fritz!Box Kontakt, der als Vergleichsobjekt dient
        With olKontakt.ErstelleFBoxKontakt(FBoxContact.Uniqueid)
            Return .Equals(FBoxContact)
        End With

    End Function

    ''' <summary>
    ''' Ermittelt die UniqueID des verknüpften Fritz!Box Telefonbucheintrages.
    ''' </summary>
    ''' <param name="olKontakte">Der Outlook-Kontakt, aus dem die UniqueID ausgelesen werden soll.</param>
    ''' <param name="TelefonbuchID">Die ID des zugehörigen Telefonbuches.</param>
    <Extension> Friend Function GetUniqueID(olKontakte As ContactItem, TelefonbuchID As Integer) As Integer
        With olKontakte

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

    ''' <summary>
    ''' Ermittelt alle UniqueIDs der verknüpften Fritz!Box Telefonbucheinträge.
    ''' </summary>
    ''' <param name="olKontakte">Der Outlook-Kontakt, aus dem die UniqueIDs ausgelesen werden soll.</param>
    <Extension> Friend Function GetUniqueID(olKontakte As ContactItem) As Dictionary(Of String, String)
        With olKontakte

            ' Überprüfe, ob es in diesem Kontakt Daten zu einem Eintrag in einem Telefonbuch gibt
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

            Return BookIDs.Zip(UniqueIDs, Function(k, v) New With {k, v}).ToDictionary(Function(x) x.k, Function(x) x.v)

        End With
    End Function

    ''' <summary>
    ''' Speichert die UniqueID des verknüpften Fritz!Box Telefonbucheintrages.
    ''' </summary>
    ''' <param name="olKontakte">Der Outlook-Kontakt, in dem die UniqueID gespeichert werden soll.</param>
    ''' <param name="TelefonbuchID">Die ID des zugehörigen Telefonbuches.</param>
    ''' <param name="UniqueID">Die UniqueID des verknüpften Fritz!Box Telefonbucheintrages, welche in den Kontakt gespeichert werden soll.</param>
    ''' <param name="Speichern">Angabe, ob der Kontakt im Anschluss gespeichert werden soll.</param>
    <Extension> Friend Sub SetUniqueID(olKontakte As ContactItem, TelefonbuchID As String, UniqueID As String, Speichern As Boolean)
        With olKontakte

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
            If Speichern Then .Save()
        End With
    End Sub

    ''' <summary>
    ''' Überschreibt einen vorhandenen Outlook-Kontakt mit Werten eines Fritz!Box Telefonbucheintrages.
    ''' </summary>
    ''' <param name="olKontakt"></param>
    ''' <param name="XMLKontakt"></param>
    Friend Sub ÜberschreibeKontakt(olKontakt As ContactItem, XMLKontakt As FBoxAPI.Contact)
        If olKontakt IsNot Nothing AndAlso XMLKontakt IsNot Nothing Then
            XMLKontakt.XMLKontaktOutlook(olKontakt)

            ' Indizere den Kontakt, wenn der Ordner, in den er gespeichert werden soll, auch zur Kontaktsuche verwendet werden soll
            IndiziereKontakt(olKontakt, olKontakt.ParentFolder, False)
        End If
    End Sub
#End Region

#Region "Bilder"

    ''' <summary>
    ''' Speichert das Kontaktbild in den Arbeitsorder. 
    ''' </summary>
    ''' <param name="olKontakt">Kontakt, aus dem das Kontaktbild extrahiert werden soll.</param>
    ''' <returns>Pfad zum extrahierten Kontaktbild.</returns>
    <Extension> Friend Function KontaktBild(olKontakt As ContactItem) As String
        KontaktBild = String.Empty
        If olKontakt IsNot Nothing Then
            With olKontakt
                With .Attachments
                    If .Item("ContactPicture.jpg") IsNot Nothing Then
                        KontaktBild = $"{Path.GetTempPath}{Path.GetRandomFileName}" '.RegExReplace(".{3}$", "jpg")
                        .Item("ContactPicture.jpg").SaveAsFile(KontaktBild)

                        NLogger.Debug($"Bild des Kontaktes {olKontakt.FullName} unter Pfad {KontaktBild} gespeichert.")
                    End If
                End With
            End With
        End If
    End Function

    <Extension> Friend Function KontaktBildEx(olKontakt As ContactItem) As Imaging.BitmapImage
        If olKontakt IsNot Nothing Then
            With olKontakt
                With .Attachments
                    If .Item("ContactPicture.jpg") IsNot Nothing Then
                        ' Bild Speichern
                        Dim BildPfad As String = $"{Path.GetTempPath}{Path.GetRandomFileName}"
                        .Item("ContactPicture.jpg").SaveAsFile(BildPfad)
                        NLogger.Debug($"Bild des Kontaktes {olKontakt.FullName} unter Pfad {BildPfad} gespeichert.")

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