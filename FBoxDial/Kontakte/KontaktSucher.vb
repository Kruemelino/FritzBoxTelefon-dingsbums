Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Office.Interop

Friend Module KontaktSucher

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend Event Status As EventHandler(Of String)
    Friend Event Beendet As EventHandler(Of NotifyEventArgs(Of Boolean))

#Region "Kontaktsuche DASL in Ordnerauswahl"

    ''' <summary>
    ''' Startet eine Kontaktsuche in allen ausgewählten Kontaktordnern (<see cref="Outlook.OlDefaultFolders.olFolderContacts"/>) durch.
    ''' <para>Es wird nach einer Telefonnummer gesucht.</para>
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, nach der gesucht werden soll.</param>
    ''' <returns>Kontakt, der zu der Telefonnummer gefunden wurde.</returns>
    Friend Async Function KontaktSucheTelNr(TelNr As Telefonnummer) As Task(Of Outlook.ContactItem)
        Dim olKontakt As Outlook.ContactItem = Nothing

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
    ''' <param name="olOrdner">Der zu durchsuchende Ornder als <see cref="Outlook.MAPIFolder"/></param>
    ''' <param name="sFilter">Ein Filter in der Syntax für Microsoft Jet oder DAV Searching and Locating (DASL), die die Kriterien für Elemente im übergeordneten Ordner gibt.</param>
    ''' <returns>Auflistung aller zur <paramref name="sFilter"/> passenden Kontakte aus diesem <paramref name="olOrdner"/> als Liste von <seealso cref="Outlook.ContactItem"/></returns>
    Private Function FindeKontaktInOrdner(olOrdner As Outlook.MAPIFolder, sFilter As String, ct As CancellationToken) As List(Of Outlook.ContactItem)

        Dim olKontaktListe As New List(Of Outlook.ContactItem)

        If olOrdner.DefaultItemType = Outlook.OlItemType.olContactItem Then

            Dim oTable As Outlook.Table
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
    ''' Führt die Kontaktsuche in allen ausgewählten Kontaktordnern (<see cref="Outlook.OlDefaultFolders.olFolderContacts"/>) durch.
    ''' </summary>
    ''' <param name="sFilter">Ein Filter in der Syntax für Microsoft Jet oder DAV Searching and Locating (DASL), die die Kriterien für Elemente im übergeordneten Ordner gibt.</param>
    ''' <returns>Auflistung aller zur <paramref name="sFilter"/> passenden Kontakte aus allen gewählten Kontaktorndern als Liste von <seealso cref="Outlook.ContactItem"/></returns>
    Private Async Function KontaktSucheFilter(sFilter As String, ct As CancellationToken) As Task(Of List(Of Outlook.ContactItem))
        Dim olKontaktListe As New List(Of Outlook.ContactItem)

        If Globals.ThisAddIn.Application IsNot Nothing AndAlso sFilter.IsNotStringNothingOrEmpty Then

            NLogger.Trace($"Kontaktsuche mit Filter gestartet: {sFilter}")

            ' Ermittle Ordner
            Dim OrdnerListe As List(Of OutlookOrdner) = XMLData.POptionen.OutlookOrdner.FindAll(OutlookOrdnerVerwendung.KontaktSuche)

            ' Füge den Standardkontaktordner hinzu, falls keine anderen Ordner definiert wurden.
            If Not OrdnerListe.Any Then
                OrdnerListe.Add(New OutlookOrdner(GetDefaultMAPIFolder(Outlook.OlDefaultFolders.olFolderContacts), OutlookOrdnerVerwendung.KontaktSuche))
            End If

            ' Erzeuge eine Liste aller existierenden Ordner, die der Nutzer ausgewählt hat
            Dim MAPIFolderList As List(Of Outlook.MAPIFolder) = OrdnerListe.Where(Function(F) F.Exists).Select(Function(S) S.MAPIFolder).ToList

            ' Füge die Unterordner hinzu
            If XMLData.POptionen.CBSucheUnterordner Then
                PushStatus(LogLevel.Debug, "Ermittle Unterordner...")
                AddChildFolders(MAPIFolderList, Outlook.OlItemType.olContactItem)
            End If

            PushStatus(LogLevel.Debug, $"Starte die Kontaktsuche in {MAPIFolderList.Count} Ordnern.")

            ' Erzeuge eine neue Liste von Taskobjekten, die eine Liste von Kontaktelementen zurückgeben.
            Dim TaskList As New List(Of Task(Of List(Of Outlook.ContactItem)))
            ' Führe die Kontaktsuche aus.
            For Each MapiFolder In MAPIFolderList
                ' Füge einen eigenen Task je Ordner hinzu.
                TaskList.Add(Task.Run(Function() As List(Of Outlook.ContactItem)
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
    Friend Function KontaktSuche(SMTPAdresse As EMailType) As Outlook.ContactItem

        If SMTPAdresse.Addresse.IsNotStringNothingOrEmpty Then
            ' Empfänger generieren
            Dim Empfänger As Outlook.Recipient = Globals.ThisAddIn.Application.Session.CreateRecipient(SMTPAdresse.Addresse)

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

    Friend Function KontaktSucheExchangeUser(SMTPAdresse As EMailType) As Outlook.ExchangeUser

        If SMTPAdresse.Addresse.IsNotStringNothingOrEmpty Then
            ' Empfänger generieren
            Dim Empfänger As Outlook.Recipient = Globals.ThisAddIn.Application.Session.CreateRecipient(SMTPAdresse.Addresse)

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
    Friend Function KontaktSuche(Kontaktkarte As Microsoft.Office.Core.IMsoContactCard) As Outlook.ContactItem

        If Kontaktkarte IsNot Nothing Then

            Select Case Kontaktkarte.AddressType
                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeSMTP
                    ' über Kontaktkarte.Address wird die SMTP-Adresse zurückgegeben
                    Return KontaktSuche(New EMailType With {.Addresse = Kontaktkarte.Address, .OutlookTyp = OutlookEMailType.SMTP})

                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeOutlook
                    Dim Adresseintrag As Outlook.AddressEntry = Globals.ThisAddIn.Application.Session.GetAddressEntryFromID(Kontaktkarte.Address)

                    If Adresseintrag?.AddressEntryUserType = Outlook.OlAddressEntryUserType.olOutlookContactAddressEntry Then
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
    Friend Function KontaktSucheExchangeUser(Kontaktkarte As Microsoft.Office.Core.IMsoContactCard) As Outlook.ExchangeUser

        If Kontaktkarte IsNot Nothing Then

            Select Case Kontaktkarte.AddressType
                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeSMTP
                    ' über Kontaktkarte.Address wird die SMTP-Adresse zurückgegeben
                    Return KontaktSucheExchangeUser(New EMailType With {.Addresse = Kontaktkarte.Address, .OutlookTyp = OutlookEMailType.EX})

                Case Microsoft.Office.Core.MsoContactCardAddressType.msoContactCardAddressTypeOutlook
                    Dim Adresseintrag As Outlook.AddressEntry = Globals.ThisAddIn.Application.Session.GetAddressEntryFromID(Kontaktkarte.Address)

                    Select Case Adresseintrag?.AddressEntryUserType
                        Case Outlook.OlAddressEntryUserType.olExchangeUserAddressEntry, Outlook.OlAddressEntryUserType.olExchangeRemoteUserAddressEntry
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
    ''' <returns>Liste von gefundenen <see cref="Outlook.ContactItem"/></returns>
    Friend Async Function KontaktSucheNameField(FilterWert As String, Exakt As Boolean, ct As CancellationToken) As Task(Of List(Of Outlook.ContactItem))
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
    ''' <returns>Liste von gefundenen <see cref="Outlook.ContactItem"/></returns>
    Friend Async Function KontaktSucheNumberField(FilterWert As String, Exakt As Boolean) As Task(Of List(Of Outlook.ContactItem))
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
    ''' <returns>Liste von gefundenen <see cref="Outlook.ContactItem"/></returns>
    Friend Async Function KontaktSucheEMailField(FilterWert As String, Exakt As Boolean) As Task(Of List(Of Outlook.ContactItem))
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
End Module
