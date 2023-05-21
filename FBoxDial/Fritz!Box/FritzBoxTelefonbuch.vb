Imports System.Security.Cryptography
Imports System.Threading.Tasks
Imports Microsoft.Office.Interop.Outlook

Namespace Telefonbücher
    Friend Module FritzBoxTelefonbuch
        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

        ''' <summary>
        ''' Erstellt eine Liste aller auf der Fritz!Box verfügbaren Telefonbücher.
        ''' Die einzelnen Einträge werden heruntergeladen.
        ''' </summary>
        Friend Async Function LadeTelefonbücher() As Task(Of IEnumerable(Of PhonebookEx))

            ' Prüfe, ob Fritz!Box verfügbar
            If Globals.ThisAddIn.FBoxTR064.Ready Then
                With Globals.ThisAddIn.FBoxTR064.X_contact
                    ' Ermittle alle verfügbaren Telefonbücher
                    Dim PhonebookIDs As Integer() = {}
                    If .GetPhonebookList(PhonebookIDs) Then

                        ' Initialiesiere die Gesamtliste der Telefonbücher
                        Dim AlleTelefonbücher As New List(Of PhonebookEx)
                        Dim PhonebookURL As String = String.Empty

                        ' Schleife durch alle ermittelten IDs
                        For Each PhonebookID In PhonebookIDs
                            AlleTelefonbücher.AddRange(Await LadeTelefonbuch(PhonebookID))
                        Next

                        If AlleTelefonbücher.Count.AreDifferentTo(PhonebookIDs.Count) Then
                            NLogger.Warn($"Es konnten nur {AlleTelefonbücher.Count} von {PhonebookIDs.Count} Telefonbüchern heruntergeladen werden.")
                        End If

                        ' Füge das Telefonbuch der Rufsperre hinzu.
                        AlleTelefonbücher.AddRange(Await LadeSperrliste())

                        Return AlleTelefonbücher

                    End If

                    Return Nothing
                End With
            Else
                NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
                Return Nothing
            End If
        End Function

        Friend Async Function LadeTelefonbuch(PhonebookID As Integer) As Task(Of IEnumerable(Of PhonebookEx))

            ' Prüfe, ob Fritz!Box verfügbar
            If Globals.ThisAddIn.FBoxTR064.Ready Then
                With Globals.ThisAddIn.FBoxTR064.X_contact

                    ' Initialiesiere die Gesamtliste der Telefonbücher
                    Dim AlleTelefonbücher As New List(Of PhonebookEx)
                    Dim PhonebookURL As String = String.Empty

                    Dim AktuellePhoneBookXML As FBoxAPI.PhonebooksType

                    ' Lade das Telefonbuch herunter und deserialisiere es im Anschluss
                    AktuellePhoneBookXML = Await .GetPhonebook(PhonebookID)

                    If AktuellePhoneBookXML IsNot Nothing Then
                        ' Verarbeite die Telefonbücher
                        For Each Telefonbuch In AktuellePhoneBookXML.Phonebooks.ConvertAll(Function(P) New PhonebookEx With {.Phonebook = P,
                                                                                                                             .ID = PhonebookID})
                            ' Füge die Telefonbücher zusammen
                            AlleTelefonbücher.Add(Telefonbuch)
                        Next
                    End If

                    Return AlleTelefonbücher

                End With
            Else
                NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' Erstellt eine Liste aller auf der Fritz!Box verfügbaren Telefonbücher.
        ''' Die einzelnen Einträge werden jedoch nicht heruntergeladen.
        ''' </summary>
        Friend Function LadeTelefonbuchNamen() As IEnumerable(Of PhonebookEx)
            '' Prüfe, ob Fritz!Box verfügbar
            With Globals.ThisAddIn.FBoxTR064.X_contact
                ' Ermittle alle verfügbaren Telefonbücher
                Dim PhonebookIDs As Integer() = {}
                If .GetPhonebookList(PhonebookIDs) Then

                    ' Initialiesiere die Gesamtliste der Telefonbücher
                    Dim AlleTelefonbücher As New List(Of PhonebookEx)

                    ' Schleife durch alle ermittelten IDs
                    For Each PhonebookID In PhonebookIDs
                        Dim PhonebookName As String = String.Empty
                        ' Ermittle die Namen zum Telefonbuch
                        If .GetPhonebook(PhonebookID, String.Empty, PhonebookName, String.Empty) Then

                            NLogger.Debug($"Name des Telefonbuches {PhonebookID} ermittelt: '{PhonebookName}'")

                            Dim AktuellePhoneBookXML As New PhonebookEx() With {.ID = PhonebookID,
                                                                                .Rufsperren = False,
                                                                                .NurName = True,
                                                                                .Phonebook = New FBoxAPI.Phonebook With {.Name = PhonebookName}}

                            AlleTelefonbücher.Add(AktuellePhoneBookXML)

                        End If
                    Next

                    ' Füge das Telefonbuch der Rufsperre hinzu.
                    AlleTelefonbücher.Add(New PhonebookEx() With {.ID = FritzBoxDefault.DfltCallBarringID,
                                                                  .Rufsperren = True,
                                                                  .NurName = True,
                                                                  .Phonebook = New FBoxAPI.Phonebook With {.Name = Localize.LocFBoxData.strCallBarringList}})

                    ' Setze diese unvollständige Liste global.
                    If Globals.ThisAddIn.PhoneBookXML Is Nothing Then Globals.ThisAddIn.PhoneBookXML = AlleTelefonbücher

                    Return AlleTelefonbücher
                End If

                Return Nothing
            End With

        End Function

        Friend Async Function LadeSperrliste() As Task(Of IEnumerable(Of PhonebookEx))

            With Globals.ThisAddIn.FBoxTR064.X_contact
                ' Initialiesiere die Gesamtliste der Telefonbücher
                Dim Rufsperren As New List(Of PhonebookEx)

                Dim CallBarringXML As FBoxAPI.PhonebooksType = Await .GetCallBarringList

                If CallBarringXML IsNot Nothing Then
                    ' Verarbeite die Sperrliste
                    For Each Telefonbuch As PhonebookEx In CallBarringXML.Phonebooks.ConvertAll(Function(P) New PhonebookEx() With {.Phonebook = P})

                        ' Angabe, dass es sich um die Rufsperren handelt
                        Telefonbuch.Rufsperren = True

                        ' ID Setzen 258
                        Telefonbuch.ID = Telefonbuch.Phonebook.Owner.ToInt

                        ' Ändere Namen
                        Telefonbuch.Phonebook.Name = Localize.LocFBoxData.strCallBarringList

                        Rufsperren.Add(Telefonbuch)
                    Next
                End If

                Return Rufsperren
            End With
        End Function

#Region "Aktionen für Telefonbücher"
        Friend Function Find(Phonebooks As IEnumerable(Of PhonebookEx), TelNr As Telefonnummer) As FBoxAPI.Contact
            NLogger.Debug($"Starte Kontaktsuche in den Fritz!Box Telefonbüchern für Telefonnummer '{TelNr.Unformatiert}'.")

            ' Suche alle Telefonbücher mit einem entsprechenden Kontakt
            Dim Bücher As IEnumerable(Of PhonebookEx) = Phonebooks.Where(Function(B) B.ContainsNumber(TelNr))

            If Bücher.Any Then
                NLogger.Debug($"Telefonnummer {TelNr.Unformatiert} in {Bücher.Count} Buch/Büchern gefunden.")
                ' Extrahiere einen Kontakt mit dieser Nummer
                Return Bücher.First.GetContact(TelNr).First
            Else
                Return Nothing
            End If

        End Function

        Friend Function Contains(Phonebooks As IEnumerable(Of PhonebookEx), TelNr As Telefonnummer) As Boolean
            NLogger.Debug($"Starte Kontaktsuche in den Fritz!Box Telefonbüchern für Telefonnummer '{TelNr.Unformatiert}'.")

            ' Suche alle Telefonbücher mit einem entsprechenden Kontakt
            Return Phonebooks.Where(Function(B) B.ContainsNumber(TelNr)).Any

        End Function

        Friend Function GetPhonebook(PhonebookID As Integer) As PhonebookEx
            Return Globals.ThisAddIn.PhoneBookXML.Where(Function(B) B.ID.AreEqual(PhonebookID)).First
        End Function

        'Friend Sub ExtendContacts(Phonebooks As IEnumerable(Of PhonebookEx))
        '    For Each Phonebook As PhonebookEx In Phonebooks.Where(Function(PB) Not PB.Rufsperren And Not PB.IsDAV)
        '        Phonebook.ExtendContacts()
        '    Next
        'End Sub
#End Region

#Region "Aktionen für Telefonbuch"
        ''' <summary>
        ''' Erstellt ein neues Telefonbuch.
        ''' </summary>
        ''' <param name="TelefonbuchName">Übergabe des neuen Namens des Telefonbuches.</param>
        ''' <returns>XML-Telefonbuch</returns>
        Friend Async Function ErstelleTelefonbuch(TelefonbuchName As String) As Task(Of PhonebookEx)
            ' Prüfe, ob Fritz!Box verfügbar
            With Globals.ThisAddIn.FBoxTR064.X_contact
                ' Hole die momentan verfügbaren Ids der Telefonbücher
                Dim IdsA As Integer() = {}
                Dim PhonebookURL As String = String.Empty
                Dim NameOK As Boolean = True

                If .GetPhonebookList(IdsA) Then

                    ' Prüfe, ob bereits ein Telefonbuch mit dem Namen vorhanden ist.
                    For Each ID In IdsA
                        Dim Name As String = String.Empty

                        If .GetPhonebook(ID, PhonebookURL, Name, String.Empty) Then
                            If Name.IsEqual(TelefonbuchName) Then
                                NLogger.Warn($"Ein Telefonbuch mit dem Namen '{TelefonbuchName}' kann nicht angelegt werden, da bereits eins mit diesem Namen exisiert.")
                                NameOK = False
                            End If
                        End If
                    Next

                    ' Erzeuge ein neues Telefonbuch mit dem übergebenen Namen.
                    If NameOK AndAlso .AddPhonebook(TelefonbuchName) Then
                        ' Das neue Telefonbuch hat von der Fritz!Box eine ID zugewiesen bekommen.
                        NLogger.Info($"Telefonbuch mit dem Namen '{TelefonbuchName}' auf der Fritz!Box erstellt.")
                        ' Ermittle das neu angelegte Telefonbuch zur Rückgabe
                        Dim IdsB As Integer() = {}
                        If .GetPhonebookList(IdsB) Then
                            Dim PhonebookID As Integer = IdsB.Except(IdsA).First

                            With Await .GetPhonebook(PhonebookID)
                                ' Setze die ID und gib das Telefonbuch zurück
                                Return New PhonebookEx(.Phonebooks.First) With {.ID = PhonebookID}
                            End With
                        End If
                    End If
                End If

                Return Nothing
            End With

        End Function

        ''' <summary>
        ''' Löscht das Telefonbuch mit der <paramref name="TelefonbuchID"/>.
        ''' </summary>
        ''' <param name="TelefonbuchID">Die ID des zu löschenden Telefonbuches</param>
        ''' <returns>Boolean, ob erfolgreich.</returns>
        ''' <remarks>Wenn die ID nicht vorhanden ist, wird trotzdem <c>True</c> zurückgegeben.</remarks>
        Friend Function LöscheTelefonbuch(TelefonbuchID As Integer) As Boolean
            '' Prüfe, ob Fritz!Box verfügbar
            With Globals.ThisAddIn.FBoxTR064.X_contact
                ' Hole die momentan verfügbaren Ids der Telefonbücher
                Dim IdsA As Integer() = {}

                If .GetPhonebookList(IdsA) Then
                    ' Prüfe, ob ein Telefonbuch mit der ID vorhanden ist, wenn nicht, muss auch nichts gelöscht werden.
                    If IdsA.Contains(TelefonbuchID) Then
                        ' Lösche das Telefonbuch
                        If .DeletePhonebook(TelefonbuchID) Then
                            NLogger.Info($"Telefonbuch mit der ID '{TelefonbuchID}' auf der Fritz!Box gelöscht.")
                            Return True

                        Else
                            NLogger.Warn($"Telefonbuch mit der ID '{TelefonbuchID}' auf der Fritz!Box nicht gelöscht.")
                            Return False

                        End If
                    End If
                End If

                Return True
            End With

        End Function

#End Region

#Region "Aktionen für Telefonbucheinträge"
        Friend Async Function GetTelefonbuchEintrag(TelefonbuchID As Integer, ID As Integer) As Task(Of FBoxAPI.Contact)
            With Globals.ThisAddIn.FBoxTR064.X_contact
                Return Await .GetPhonebookEntryUID(TelefonbuchID, ID)
            End With

        End Function

        ''' <summary>
        ''' Startet das Hochladen eines Kontaktes.
        ''' </summary>
        ''' <param name="TelefonbuchID">ID des Telefonbuches</param>
        ''' <param name="XMLDaten">Passender XML String des Kontaktes</param>
        ''' <returns>Die einzigartige ID des Kontaktes im Fritz!Box Telefonbuch mit der <paramref name="TelefonbuchID"/>.</returns>
        ''' <remarks>Wird durch das Formular Telefonbuch des Addins aufgerufen.</remarks>
        Friend Function SetTelefonbuchEintrag(TelefonbuchID As Integer, XMLDaten As String) As Integer
            If XMLDaten.IsNotStringNothingOrEmpty Then

                With Globals.ThisAddIn.FBoxTR064.X_contact
                    Dim UID As Integer = -1
                    If .SetPhonebookEntryUID(TelefonbuchID, XMLDaten, UID) Then
                        NLogger.Info($"Kontakt mit der ID '{UID}' im Telefonbuch {TelefonbuchID} der Fritz!Box angelegt.")
                        Return UID
                    End If
                End With
            End If
            Return -1
        End Function

        ''' <summary>
        ''' Startet das Hochladen eines Kontaktes ohne Rückmeldung.
        ''' </summary>
        ''' <param name="TelefonbuchID">ID des Telefonbuches</param>
        ''' <param name="OutlookKontakt">Outlook-Kontakt, der konvertiert und hochgeladen werden soll.</param>
        ''' <remarks>Der Outlook-Kontakt wird mit der UniqueID und der TelefonbuchID ergänzt (via PropertyAccessor).</remarks>
        Friend Sub SetTelefonbuchEintrag(TelefonbuchID As Integer, OutlookKontakt As ContactItem)
            SetPhonebookEntryUID(TelefonbuchID, OutlookKontakt)
        End Sub

        ''' <summary>
        ''' Startet das Hochladen eines Kontaktes mit Rückmeldung.
        ''' </summary>
        ''' <param name="TelefonbuchID">ID des Telefonbuches</param>
        ''' <param name="OutlookKontakt">Outlook-Kontakt, der konvertiert und hochgeladen werden soll.</param>
        ''' <remarks>Der Outlook-Kontakt wird mit der UniqueID und der TelefonbuchID ergänzt (via PropertyAccessor).</remarks>
        ''' <returns>Ergebniszeichenfolge</returns>
        Private Function SetPhonebookEntryUID(TelefonbuchID As Integer, OutlookKontakt As ContactItem) As String
            With OutlookKontakt
                ' Überprüfe, ob es in diesem Telefonbuch bereits einen verknüpften Kontakt gibt
                Dim UID As Integer = .GetUniqueID(TelefonbuchID)
                Dim retVal As String = If(UID.AreEqual(-1), Localize.resRibbon.UploadCreateContact, Localize.resRibbon.UploadOverwriteContact)

                ' Erstelle ein entsprechendes XML-Datenobjekt und lade es hoch
                If Globals.ThisAddIn.FBoxTR064.X_contact.SetPhonebookEntryUID(TelefonbuchID, .ErstelleXMLKontakt(UID), UID) Then

                    ' Merke dir die aktuelle Zeit in dem Kontakt
                    .SetFBoxModTime(TelefonbuchID, UID, Now)

                    ' Stelle die Verknüpfung her
                    .SetUniqueID(TelefonbuchID.ToString, UID.ToString, True)

                    ' Statusmeldung
                    retVal = String.Format(Localize.resRibbon.UploadSuccess, .FullName, TelefonbuchID, retVal, UID)

                Else
                    ' Statusmeldung
                    retVal = String.Format(Localize.resRibbon.UploadError, .FullName, TelefonbuchID, retVal)
                End If
                NLogger.Info(retVal)
                Return retVal
            End With
        End Function

        ''' <summary>
        ''' Startet das Hochladen mehrerer Outlook-Kontakte mittels asynchroner Aufgaben (<see cref="Task"/>).
        ''' </summary>
        ''' <param name="TelefonbuchID">ID des Telefonbuches</param>
        ''' <param name="OutlookKontakte">Auflistung der hochzuladenden Outlook Kontakte</param>
        ''' <remarks>Wird durch die Ribbons des Addins aufgerufen.</remarks>
        Friend Async Sub SetTelefonbuchEintrag(TelefonbuchID As Integer, OutlookKontakte As IEnumerable(Of ContactItem))

            ' Generiere eine Liste an Task
            Dim TaskList As New List(Of Task(Of String))

            ' Schleife durch alle Kontakte
            For Each Kontakt In OutlookKontakte
                TaskList.Add(Task.Run(Function() SetPhonebookEntryUID(TelefonbuchID, Kontakt)))

                ' Die einzelnen Vorgänge müssen nacheinander erfolgen, da es sonst zu einer WebException kommt: Die zugrunde liegende Verbindung wurde geschlossen: Für den geschützten SSL/TLS-Kanal konnte keine Vertrauensstellung hergestellt werden.
                Await TaskList.Last
            Next

            Await Task.WhenAll(TaskList)
            ' Gib eine finale Statusmeldung heraus
            Windows.MessageBox.Show(String.Format(Localize.resRibbon.UploadResultMessageHeader, OutlookKontakte.Count, vbCrLf & vbCrLf, String.Join(vbCrLf, TaskList.Select(Function(R) R.Result))), My.Resources.strDefLongName, Windows.MessageBoxButton.OK)
        End Sub

        ''' <summary>
        ''' Startet das Löschen eines Kontaktes.
        ''' </summary>
        ''' <param name="TelefonbuchID">ID des Telefonbuches</param>
        ''' <param name="UID">Einzigartige ID des zu löschenden Kontaktes</param>
        ''' <returns>Boolean, ob erfolgreich, oder nicht.</returns>
        ''' <remarks>Wird durch das Formular Telefonbuch des Addins aufgerufen.</remarks>
        Friend Function DeleteTelefonbuchEintrag(TelefonbuchID As Integer, UID As Integer) As Boolean
            With Globals.ThisAddIn.FBoxTR064.X_contact
                If .DeletePhonebookEntryUID(TelefonbuchID, UID) Then
                    NLogger.Info($"Kontakt mit der ID '{UID}' im Telefonbuch {TelefonbuchID} auf der Fritz!Box gelöscht.")
                    Return True

                Else
                    NLogger.Warn($"Kontakt mit der ID '{UID}' im Telefonbuch {TelefonbuchID} auf der Fritz!Box nicht gelöscht.")
                    Return False

                End If
            End With
            Return True

        End Function

        ''' <summary>
        ''' Startet das Löschen eines Kontaktes.
        ''' </summary>
        ''' <param name="TelefonbuchID">ID des Telefonbuches</param>
        ''' <param name="Einträge">Auflistung der zu löschenden Kontakte</param>
        ''' <returns>Boolean, ob erfolgreich, oder nicht.</returns>
        ''' <remarks>Wird durch das Formular Telefonbuch des Addins aufgerufen.</remarks>
        Friend Function DeleteTelefonbuchEinträge(TelefonbuchID As Integer, Einträge As IEnumerable(Of FBoxAPI.Contact)) As Boolean

            With Globals.ThisAddIn.FBoxTR064.X_contact
                For Each Kontakt In Einträge
                    If .DeletePhonebookEntryUID(TelefonbuchID, Kontakt.Uniqueid) Then
                        NLogger.Info($"Kontakt mit der ID '{Kontakt.Uniqueid}' im Telefonbuch {TelefonbuchID} auf der Fritz!Box gelöscht.")
                        Return True

                    Else
                        NLogger.Warn($"Kontakt mit der ID '{Kontakt.Uniqueid}' im Telefonbuch {TelefonbuchID} auf der Fritz!Box nicht gelöscht.")
                        Return False

                    End If
                Next
            End With
            Return True

        End Function

#End Region

    End Module
End Namespace
