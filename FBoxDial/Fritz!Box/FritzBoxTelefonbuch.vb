Imports System.Reflection
Imports System.Threading.Tasks
Imports System.Xml

Namespace Telefonbücher
    Friend Module FritzBoxTelefonbuch
        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

        Friend Async Function LadeFritzBoxTelefonbücher() As Task(Of FritzBoxXMLTelefonbücher)

            Using fbtr064 As New FritzBoxTR64
                With fbtr064
                    ' Ermittle alle verfügbaren Telefonbücher
                    Dim PhonebookIDs As Integer() = {}
                    If .GetPhonebookList(PhonebookIDs) Then

                        ' Initialiesiere die Gesamtliste der Telefonbücher
                        Dim AlleTelefonbücher As New FritzBoxXMLTelefonbücher With {.NurHeaderDaten = False}
                        Dim PhonebookURL As String = DfltStringEmpty

                        ' Lade die xslt Transformationsdatei
                        Dim xslt As New Xsl.XslCompiledTransform
                        xslt.Load(XmlReader.Create(Assembly.GetExecutingAssembly.GetManifestResourceStream("FBoxDial.ToLower.xslt")))

                        Dim AktuellePhoneBookXML As FritzBoxXMLTelefonbücher

                        ' Schleife durch alle ermittelten IDs
                        For Each PhonebookID In PhonebookIDs

                            ' Ermittle die URL zum Telefonbuch
                            If .GetPhonebook(PhonebookID, PhonebookURL) Then

                                NLogger.Debug($"Telefonbuch {PhonebookID} heruntergeladen: '{PhonebookURL}'")

                                ' Lade das Telefonbuch herunter
                                AktuellePhoneBookXML = Await DeserializeObjectAsyc(Of FritzBoxXMLTelefonbücher)(PhonebookURL, xslt)

                                If AktuellePhoneBookXML IsNot Nothing Then
                                    ' Verarbeite die Telefonbücher
                                    For Each Telefonbuch In AktuellePhoneBookXML.Telefonbücher

                                        ' Setze die ID
                                        Telefonbuch.ID = PhonebookID

                                    Next

                                    ' Füge die Telefonbücher zusammen
                                    AlleTelefonbücher.Telefonbücher.AddRange(AktuellePhoneBookXML.Telefonbücher)
                                End If
                            End If

                        Next

                        If AlleTelefonbücher.Telefonbücher.Count.AreDifferentTo(PhonebookIDs.Count) Then
                            NLogger.Warn($"Es konnten nur {AlleTelefonbücher.Telefonbücher.Count} von {PhonebookIDs.Count} Telefonbüchern heruntergeladen werden.")
                        End If

                        ' TODO: Ergänze Rufsperren
                        If .GetCallBarringList(PhonebookURL) Then
                            NLogger.Debug($"Rufsperren heruntergeladen: '{PhonebookURL}'")

                            ' Lade das Telefonbuch herunter
                            AktuellePhoneBookXML = Await DeserializeObjectAsyc(Of FritzBoxXMLTelefonbücher)(PhonebookURL, xslt)


                            If AktuellePhoneBookXML IsNot Nothing Then
                                ' Verarbeite die Telefonbücher
                                For Each Telefonbuch In AktuellePhoneBookXML.Telefonbücher

                                    ' Angabe, dass es sich um die Rufsperren handelt
                                    Telefonbuch.Rufsperren = True

                                    ' ID Setzen 258
                                    Telefonbuch.ID = Telefonbuch.Owner.ToInt

                                    ' Ändere Namen
                                    Telefonbuch.Name = Localize.resTelefonbuch.strCallBarringList
                                Next

                                ' Füge die Telefonbücher zusammen
                                AlleTelefonbücher.Telefonbücher.AddRange(AktuellePhoneBookXML.Telefonbücher)
                            End If
                        End If


                        Return AlleTelefonbücher
                    End If
                    Return Nothing
                End With
            End Using
        End Function

        Friend Function LadeHeaderFritzBoxTelefonbücher() As FritzBoxXMLTelefonbücher
            Using fbtr064 As New FritzBoxTR64
                With fbtr064
                    ' Ermittle alle verfügbaren Telefonbücher
                    Dim PhonebookIDs As Integer() = {}
                    If .GetPhonebookList(PhonebookIDs) Then

                        ' Initialiesiere die Gesamtliste der Telefonbücher
                        Dim AlleTelefonbücher As New FritzBoxXMLTelefonbücher With {.NurHeaderDaten = True}

                        ' Schleife durch alle ermittelten IDs
                        For Each PhonebookID In PhonebookIDs
                            Dim PhonebookURL As String = DfltStringEmpty
                            Dim PhonebookName As String = DfltStringEmpty
                            ' Ermittle die URL und Namen zum Telefonbuch
                            If .GetPhonebook(PhonebookID, PhonebookURL, PhonebookName) Then

                                NLogger.Debug($"Name des Telefonbuches {PhonebookID} ermittelt: '{PhonebookName}'")

                                Dim AktuellePhoneBookXML As New FritzBoxXMLTelefonbuch With {.ID = PhonebookID, .Name = PhonebookName}

                                AlleTelefonbücher.Telefonbücher.Add(AktuellePhoneBookXML)

                            End If
                        Next
                        ' Setze diese unvollständige Liste global.
                        If ThisAddIn.PhoneBookXML Is Nothing Then ThisAddIn.PhoneBookXML = AlleTelefonbücher

                        Return AlleTelefonbücher
                    End If
                    Return Nothing
                End With
            End Using
        End Function

#Region "Aktionen für Telefonbuch"
        ''' <summary>
        ''' Erstellt ein neues Telefonbuch.
        ''' </summary>
        ''' <param name="TelefonbuchName">Übergabe des neuen Namens des Telefonbuches.</param>
        ''' <returns>XML-Telefonbuch</returns>
        Friend Async Function ErstelleTelefonbuch(TelefonbuchName As String) As Task(Of FritzBoxXMLTelefonbuch)
            Using fboxTR064 As New FritzBoxTR64
                With fboxTR064
                    ' Hole die momentan verfügbaren Ids der Telefonbücher
                    Dim IdsA As Integer() = {}
                    Dim PhonebookURL As String = DfltStringEmpty
                    Dim NameOK As Boolean = True
                    If .GetPhonebookList(IdsA) Then

                        ' Prüfe, ob bereits ein Telefonbuch mit dem Namen vorhanden ist.
                        For Each ID In IdsA
                            Dim Name As String = DfltStringEmpty

                            If .GetPhonebook(ID, PhonebookURL, Name) Then
                                If Name.AreEqual(TelefonbuchName) Then
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

                                    With Await DeserializeObjectAsyc(Of FritzBoxXMLTelefonbücher)(PhonebookURL, xslt)
                                        ' Setze die ID
                                        .Telefonbücher.First.ID = PhonebookID

                                        ' Gib das Telefonbuch zurück
                                        Return .Telefonbücher.First

                                    End With


                                End If

                            End If

                        End If

                    End If

                    Return Nothing
                End With
            End Using
        End Function

        ''' <summary>
        ''' Löscht das Telefonbuch mit der <paramref name="TelefonbuchID"/>.
        ''' </summary>
        ''' <param name="TelefonbuchID">Die ID des zu löschenden Telefonbuches</param>
        ''' <returns>Boolean, ob erfolgreich.</returns>
        ''' <remarks>Wenn die ID nicht vorhanden ist, wird trotzdem <c>True</c> zurückgegeben.</remarks>
        Friend Function LöscheTelefonbuch(TelefonbuchID As Integer) As Boolean
            Using fbtr064 As New FritzBoxTR64
                With fbtr064
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
            End Using
        End Function

#End Region

#Region "Aktionen für Telefonbucheinträge"

        Friend Function SetTelefonbuchEintrag(TelefonbuchID As Integer, XMLDaten As String) As Integer

            If XMLDaten.IsNotStringEmpty Then
                Using fbtr064 As New FritzBoxTR64
                    With fbtr064
                        Dim UID As Integer = -1
                        If .SetPhonebookEntryUID(TelefonbuchID, XMLDaten, UID) Then
                            NLogger.Info($"Kontakt mit der ID '{UID}' im Telefonbuch {TelefonbuchID} auf der Fritz!Box angelegt.")
                            Return UID
                        End If
                    End With
                End Using
            End If
            Return -1
        End Function

        Friend Function DeleteTelefonbuchEintrag(TelefonbuchID As Integer, UID As Integer) As Boolean
            Using fbtr064 As New FritzBoxTR64
                With fbtr064
                    If .DeletePhonebookEntryUID(TelefonbuchID, UID) Then
                        NLogger.Info($"Kontakt mit der ID '{UID}' im Telefonbuch {TelefonbuchID} auf der Fritz!Box gelöscht.")
                        Return True

                    Else
                        NLogger.Warn($"Kontakt mit der ID '{UID}' im Telefonbuch {TelefonbuchID} auf der Fritz!Box nicht gelöscht.")
                        Return False

                    End If
                End With
            End Using
            Return True
        End Function
#End Region
    End Module
End Namespace
