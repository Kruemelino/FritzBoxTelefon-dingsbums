Imports System.Reflection
Imports System.Threading.Tasks
Imports Microsoft.Office.Interop.Outlook
Imports System.Xml

Namespace Telefonbücher
    Friend Module FritzBoxTelefonbuch
        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

        Friend Async Function LadeTelefonbücher(FBoxTR064 As FBoxAPI.FritzBoxTR64) As Task(Of IEnumerable(Of PhonebookEx))

            ' Prüfe, ob Fritz!Box verfügbar
            If FBoxTR064.Ready Then
                With FBoxTR064.X_contact
                    ' Ermittle alle verfügbaren Telefonbücher
                    Dim PhonebookIDs As Integer() = {}
                    If .GetPhonebookList(PhonebookIDs) Then

                        ' Initialiesiere die Gesamtliste der Telefonbücher
                        Dim AlleTelefonbücher As New List(Of PhonebookEx)
                        Dim PhonebookURL As String = String.Empty

                        ' Lade die xslt Transformationsdatei
                        Dim xslt As New Xsl.XslCompiledTransform
                        xslt.Load(XmlReader.Create(Assembly.GetExecutingAssembly.GetManifestResourceStream("FBoxDial.ToLower.xslt")))

                        Dim AktuellePhoneBookXML As FBoxAPI.PhonebooksType

                        ' Schleife durch alle ermittelten IDs
                        For Each PhonebookID In PhonebookIDs

                            ' Ermittle die URL zum Telefonbuch
                            If .GetPhonebook(PhonebookID, PhonebookURL) Then

                                NLogger.Debug($"Telefonbuch {PhonebookID} heruntergeladen: {PhonebookURL} ")

                                ' Lade das Telefonbuch herunter
                                AktuellePhoneBookXML = Await DeserializeAsyncXML(Of FBoxAPI.PhonebooksType)(PhonebookURL, True, xslt)

                                If AktuellePhoneBookXML IsNot Nothing Then
                                    ' Verarbeite die Telefonbücher
                                    For Each Telefonbuch In AktuellePhoneBookXML.Phonebooks.ConvertAll(Function(P) New PhonebookEx(P))

                                        ' Setze die ID
                                        Telefonbuch.ID = PhonebookID
                                        ' Füge die Telefonbücher zusammen
                                        AlleTelefonbücher.Add(Telefonbuch)
                                    Next

                                End If
                            End If

                        Next

                        If AlleTelefonbücher.Count.AreDifferentTo(PhonebookIDs.Count) Then
                            NLogger.Warn($"Es konnten nur {AlleTelefonbücher.Count} von {PhonebookIDs.Count} Telefonbüchern heruntergeladen werden.")
                        End If

                        ' Füge das Telefonbuch der Rufsperre hinzu.
                        AlleTelefonbücher.AddRange(Await LadeSperrliste(FBoxTR064))

                        Return AlleTelefonbücher
                    End If

                    Return Nothing
                End With
            Else
                NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
                Return Nothing
            End If
        End Function

        Friend Function LadeTelefonbücherNamen(FBoxTR064 As FBoxAPI.FritzBoxTR64) As IEnumerable(Of PhonebookEx)
            '' Prüfe, ob Fritz!Box verfügbar
            With FBoxTR064.X_contact
                ' Ermittle alle verfügbaren Telefonbücher
                Dim PhonebookIDs As Integer() = {}
                If .GetPhonebookList(PhonebookIDs) Then

                    ' Initialiesiere die Gesamtliste der Telefonbücher
                    Dim AlleTelefonbücher As New List(Of PhonebookEx)

                    ' Schleife durch alle ermittelten IDs
                    For Each PhonebookID In PhonebookIDs
                        Dim PhonebookURL As String = String.Empty
                        Dim PhonebookName As String = String.Empty
                        ' Ermittle die URL und Namen zum Telefonbuch
                        If .GetPhonebook(PhonebookID, PhonebookURL, PhonebookName) Then

                            NLogger.Debug($"Name des Telefonbuches {PhonebookID} ermittelt: '{PhonebookName}'")

                            Dim AktuellePhoneBookXML As New PhonebookEx(Nothing) With {.ID = PhonebookID, .Rufsperren = False, .Name = PhonebookName}

                            AlleTelefonbücher.Add(AktuellePhoneBookXML)

                        End If
                    Next
                    ' Setze diese unvollständige Liste global.
                    If Globals.ThisAddIn.PhoneBookXML Is Nothing Then Globals.ThisAddIn.PhoneBookXML = AlleTelefonbücher

                    Return AlleTelefonbücher
                End If

                Return Nothing
            End With

        End Function

        Friend Async Function LadeSperrliste(FBoxTR064 As FBoxAPI.FritzBoxTR64) As Task(Of IEnumerable(Of PhonebookEx))

            With FBoxTR064.X_contact
                ' Initialiesiere die Gesamtliste der Telefonbücher
                Dim PhonebookURL As String = String.Empty
                Dim Rufsperren As New List(Of PhonebookEx)
                ' Lade die xslt Transformationsdatei
                Dim xslt As New Xsl.XslCompiledTransform
                xslt.Load(XmlReader.Create(Assembly.GetExecutingAssembly.GetManifestResourceStream("FBoxDial.ToLower.xslt")))

                Dim CallBarringXML As New FBoxAPI.PhonebooksType

                If .GetCallBarringList(PhonebookURL) Then
                    NLogger.Debug($"Rufsperren heruntergeladen: {PhonebookURL} ")

                    ' Lade das Telefonbuch herunter
                    CallBarringXML = Await DeserializeAsyncXML(Of FBoxAPI.PhonebooksType)(PhonebookURL, True, xslt)

                    If CallBarringXML IsNot Nothing Then
                        ' Verarbeite die Telefonbücher
                        For Each Telefonbuch As PhonebookEx In CallBarringXML.Phonebooks.ConvertAll(Function(P) New PhonebookEx(P))

                            '' Angabe, dass es sich um die Rufsperren handelt
                            Telefonbuch.Rufsperren = True

                            ' ID Setzen 258
                            Telefonbuch.ID = Telefonbuch.Phonebook.Owner.ToInt

                            ' Ändere Namen
                            Telefonbuch.Phonebook.Name = Localize.LocFBoxData.strCallBarringList

                            Rufsperren.Add(Telefonbuch)
                        Next
                    End If
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
                Return Bücher.First.FindbyNumber(TelNr).First
            Else
                Return Nothing
            End If

        End Function
#End Region

#Region "Aktionen für Telefonbuch"
        ''' <summary>
        ''' Erstellt ein neues Telefonbuch.
        ''' </summary>
        ''' <param name="FBoxTR064">TR064 Schnittstelle zur Fritz!Box</param>
        ''' <param name="TelefonbuchName">Übergabe des neuen Namens des Telefonbuches.</param>
        ''' <returns>XML-Telefonbuch</returns>
        Friend Async Function ErstelleTelefonbuch(FBoxTR064 As FBoxAPI.FritzBoxTR64, TelefonbuchName As String) As Task(Of PhonebookEx)
            ' Prüfe, ob Fritz!Box verfügbar
            With FBoxTR064.X_contact
                ' Hole die momentan verfügbaren Ids der Telefonbücher
                Dim IdsA As Integer() = {}
                Dim PhonebookURL As String = String.Empty
                Dim NameOK As Boolean = True
                If .GetPhonebookList(IdsA) Then

                    ' Prüfe, ob bereits ein Telefonbuch mit dem Namen vorhanden ist.
                    For Each ID In IdsA
                        Dim Name As String = String.Empty

                        If .GetPhonebook(ID, PhonebookURL, Name) Then
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

                            ' Lade Ermittle URL des neuen Telefonbuches 
                            If .GetPhonebook(PhonebookID, PhonebookURL, TelefonbuchName) Then

                                NLogger.Debug($"Telefonbuch {PhonebookID} heruntergeladen: {PhonebookURL}")

                                ' Lade die xslt Transformationsdatei
                                Dim xslt As New Xsl.XslCompiledTransform
                                xslt.Load(XmlReader.Create(Assembly.GetExecutingAssembly.GetManifestResourceStream("FBoxDial.ToLower.xslt")))

                                With Await DeserializeAsyncXML(Of FBoxAPI.PhonebooksType)(PhonebookURL, True, xslt)
                                    ' Setze die ID und gib das Telefonbuch zurück
                                    Return New PhonebookEx(.Phonebooks.First) With {.ID = PhonebookID}
                                End With
                            End If
                        End If
                    End If
                End If

                Return Nothing
            End With

        End Function

        ''' <summary>
        ''' Löscht das Telefonbuch mit der <paramref name="TelefonbuchID"/>.
        ''' </summary>
        ''' <param name="fbtr064">TR064 Schnittstelle zur Fritz!Box</param>
        ''' <param name="TelefonbuchID">Die ID des zu löschenden Telefonbuches</param>
        ''' <returns>Boolean, ob erfolgreich.</returns>
        ''' <remarks>Wenn die ID nicht vorhanden ist, wird trotzdem <c>True</c> zurückgegeben.</remarks>
        Friend Function LöscheTelefonbuch(fbtr064 As FBoxAPI.FritzBoxTR64, TelefonbuchID As Integer) As Boolean
            '' Prüfe, ob Fritz!Box verfügbar
            With fbtr064.X_contact
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

        ''' <summary>
        ''' Startet das Hochladen eines Kontaktes.
        ''' </summary>
        ''' <param name="FBoxTR064">TR064 Schnittstelle zur Fritz!Box</param>
        ''' <param name="TelefonbuchID">ID des Telefonbuches</param>
        ''' <param name="XMLDaten">Passender XML String des Kontaktes</param>
        ''' <returns>Die einzigartige ID des Kontaktes im Fritz!Box Telefonbuch mit der <paramref name="TelefonbuchID"/>.</returns>
        ''' <remarks>Wird durch das Formular Telefonbuch des Addins aufgerufen.</remarks>
        Friend Function SetTelefonbuchEintrag(FBoxTR064 As FBoxAPI.FritzBoxTR64, TelefonbuchID As Integer, XMLDaten As String) As Integer
            If XMLDaten.IsNotStringNothingOrEmpty Then

                With FBoxTR064.X_contact
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
        ''' Startet das Hochladen mehrerer Outlook-Kontakte mittels asynchroner Aufgaben (<see cref="Task"/>).
        ''' </summary>
        ''' <param name="FBoxTR064">TR064 Schnittstelle zur Fritz!Box</param>
        ''' <param name="TelefonbuchID">ID des Telefonbuches</param>
        ''' <param name="OutlookKontakte">Auflistung der hochzuladenden Outlook Kontakte</param>
        ''' <remarks>Wird durch die Ribbons des Addins aufgerufen.</remarks>
        Friend Async Sub SetTelefonbuchEintrag(FBoxTR064 As FBoxAPI.FritzBoxTR64, TelefonbuchID As Integer, OutlookKontakte As IEnumerable(Of ContactItem))

            ' Generiere eine Liste an Task
            Dim TaskList As New List(Of Task(Of String))

            ' Schleife durch alle Kontakte
            For Each Kontakt In OutlookKontakte
                TaskList.Add(Task.Run(Function() As String
                                          With Kontakt
                                              ' Überprüfe, ob es in diesem Telefonbuch bereits einen verknüpften Kontakt gibt
                                              Dim UID As Integer = Kontakt.GetUniqueID(TelefonbuchID)
                                              Dim retVal As String = If(UID.AreEqual(-1), Localize.resRibbon.UploadCreateContact, Localize.resRibbon.UploadOverwriteContact)

                                              ' Erstelle ein entsprechendes XML-Datenobjekt und lade es hoch
                                              If FBoxTR064.X_contact.SetPhonebookEntryUID(TelefonbuchID, .ErstelleXMLKontakt(UID), UID) Then
                                                  ' Stelle die Verknüpfung her
                                                  .SetUniqueID(TelefonbuchID.ToString, UID.ToString)

                                                  ' Statusmeldung
                                                  retVal = String.Format(Localize.resRibbon.UploadSuccess, .FullName, TelefonbuchID, retVal, UID)

                                              Else
                                                  ' Statusmeldung
                                                  retVal = String.Format(Localize.resRibbon.UploadError, .FullName, TelefonbuchID, retVal)
                                              End If
                                              NLogger.Info(retVal)
                                              Return retVal
                                          End With
                                      End Function))

                ' Die einzelnen Vorgänge müssen nacheinander erfolgen, da es sonst zu einer WebException kommt: Die zugrunde liegende Verbindung wurde geschlossen: Für den geschützten SSL/TLS-Kanal konnte keine Vertrauensstellung hergestellt werden.
                Await TaskList.Last
            Next

            Await Task.WhenAll(TaskList)
            ' Gib eine finale Statusmeldung heraus
            Windows.MessageBox.Show(String.Format(Localize.resRibbon.UploadResultMessageHeader, OutlookKontakte.Count, vbCrLf & vbCrLf, String.Join(vbCrLf, TaskList.Select(Function(R) R.Result))), My.Resources.strDefLongName, Windows.MessageBoxButton.OK)
        End Sub

        ''' <summary>
        ''' Startet das Hochladen mehrerer Outlook-Kontakte mittels asynchroner Aufgaben (<see cref="Task"/>). Die TR-064 Schnittstelle wird eigenshierfür neu initialisiert.
        ''' </summary>
        ''' <param name="TelefonbuchID">ID des Telefonbuches</param>
        ''' <param name="OutlookKontakte">Auflistung der hochzuladenden Outlook Kontakte</param>
        ''' <remarks>Wird durch die Ribbons des Addins aufgerufen.</remarks>
        Friend Sub SetTelefonbuchEintrag(TelefonbuchID As Integer, OutlookKontakte As IEnumerable(Of ContactItem))
            Using FBoxTR064 = New FBoxAPI.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, XMLData.POptionen.TBNetworkTimeout, FritzBoxDefault.Anmeldeinformationen)
                SetTelefonbuchEintrag(FBoxTR064, TelefonbuchID, OutlookKontakte)
            End Using
        End Sub
        ''' <summary>
        ''' Startet das Löschen eines Kontaktes.
        ''' </summary>
        ''' <param name="FBoxTR064">TR064 Schnittstelle zur Fritz!Box</param>
        ''' <param name="TelefonbuchID">ID des Telefonbuches</param>
        ''' <param name="UID">Einzigartige ID des zu löschenden Kontaktes</param>
        ''' <returns>Boolean, ob erfolgreich, oder nicht.</returns>
        ''' <remarks>Wird durch das Formular Telefonbuch des Addins aufgerufen.</remarks>
        Friend Function DeleteTelefonbuchEintrag(FBoxTR064 As FBoxAPI.FritzBoxTR64, TelefonbuchID As Integer, UID As Integer) As Boolean
            With FBoxTR064.X_contact
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
        ''' <param name="fbtr064">TR064 Schnittstelle zur Fritz!Box</param>
        ''' <param name="TelefonbuchID">ID des Telefonbuches</param>
        ''' <param name="Einträge">Auflistung der zu löschenden Kontakte</param>
        ''' <returns>Boolean, ob erfolgreich, oder nicht.</returns>
        ''' <remarks>Wird durch das Formular Telefonbuch des Addins aufgerufen.</remarks>
        Friend Function DeleteTelefonbuchEinträge(fbtr064 As FBoxAPI.FritzBoxTR64, TelefonbuchID As Integer, Einträge As IEnumerable(Of FBoxAPI.Contact)) As Boolean

            With fbtr064.X_contact
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
