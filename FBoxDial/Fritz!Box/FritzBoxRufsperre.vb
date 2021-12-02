Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Office.Interop.Outlook

Friend Module FritzBoxRufsperre
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    ''' <summary>
    ''' Fügt einen Sperrlisteneintrag (<see cref="FBoxAPI.Contact"/>) zu der Sperrliste hinzu.
    ''' </summary>
    ''' <param name="Nummern">Auflistung an Telefonnummern, die gesperrt werden sollen.</param>
    ''' <param name="Name">Name des Sperrlisteneintrages</param>
    ''' <param name="UID">Rückgabewert: UID des neuen Sperreintrages</param>
    ''' <returns></returns>
    Friend Function AddToCallBarring(fboxTR064 As FBoxAPI.FritzBoxTR64, Nummern As IEnumerable(Of String), Name As String, Optional ByRef UID As Integer = 0) As Boolean
        ' Prüfe, ob Fritz!Box verfügbar
        If Ping(XMLData.POptionen.ValidFBAdr) Then

            ' Sperreintrag generieren
            Dim Sperreintrag As FBoxAPI.Contact = CreateContact(Name)

            With Sperreintrag

                ' Prüfe, ob die übergebenen Nummern bereits auf der Rufsperre der Fritz!Box vorhanden sind.
                ' Ein Eintrag auf der Fritz!Box kann mehrere Telefonnummern enthalten
                For Each TelNr In Nummern
                    Dim Eintrag As ContactViewModel = Nothing
                    If GetCallBarringEntryByNum(fboxTR064, TelNr, Eintrag) Then
                        ' Ein Eintrag mit der Nummer bereits vorhanden
                        NLogger.Info($"Ein Eintrag mit der '{TelNr}' ist in der Sperrliste bereits vorhanden (ID {Eintrag.Uniqueid}.")
                    Else
                        ' füge die Telefonnummer dem hinzuzufügenden Sperreintrag hinzu
                        .Telephony.Numbers.Add(New FBoxAPI.Number With {.Number = TelNr})
                    End If
                Next

                ' Prüfe, ob der Sperreintrag überhaupt Nummern hat.
                If .Telephony.Numbers.Any Then
                    ' lade den Sperreintrag hoch
                    Return fboxTR064.X_contact.SetCallBarringEntry(Sperreintrag.GetXMLKontakt, UID)
                Else
                    Return False
                End If

            End With

        Else
            NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
            Return False
        End If
    End Function

    Friend Function AddToCallBarring(Nummern As IEnumerable(Of String), Name As String, Optional ByRef UID As Integer = 0) As Boolean
        Using FBoxTR064 = New FBoxAPI.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)
            Return AddToCallBarring(FBoxTR064, Nummern, Name, UID)
        End Using
    End Function

    ''' <summary>
    ''' Fügt einen Sperrlisteneintrag (<see cref="ContactViewModel"/>) zu der Sperrliste hinzu.
    ''' </summary>
    ''' <param name="Sperreintrag">Sperrlisteneintrag</param>
    ''' <param name="UID">Rückgabewert: UID des neuen Sperreintrages</param>
    Friend Function AddToCallBarring(fboxTR064 As FBoxAPI.FritzBoxTR64, Sperreintrag As FBoxAPI.Contact, Optional ByRef UID As Integer = 0) As Boolean
        ' Prüfe, ob Fritz!Box verfügbar
        If Ping(XMLData.POptionen.ValidFBAdr) Then
            Return fboxTR064.X_contact.SetCallBarringEntry(Sperreintrag.GetXMLKontakt, UID)
        Else
            NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
            Return False
        End If
    End Function

    ''' <summary>
    ''' Fügt eine Auflistung von Outlook Kontakten (<see cref="ContactItem"/>) zu der Sperrliste hinzu.
    ''' </summary>
    ''' <param name="OutlookKontakte">Auflistung von Sperrlisteneinträgen</param>
    Friend Async Sub AddToCallBarring(fboxTR064 As FBoxAPI.FritzBoxTR64, OutlookKontakte As IEnumerable(Of ContactItem))
        Const SperrlistenID As Integer = 258
        ' Prüfe, ob Fritz!Box verfügbar
        If Ping(XMLData.POptionen.ValidFBAdr) Then

            ' Erzeuge für jeden Kontakt einen Eintrag
            For Each Kontakt In OutlookKontakte
                Await Task.Run(Sub()

                                   With Kontakt
                                       ' Überprüfe, ob es in diesem Telefonbuch bereits einen verknüpften Kontakt gibt
                                       Dim UID As Integer = Kontakt.GetUniqueID(SperrlistenID)

                                       If UID.AreEqual(-1) Then
                                           NLogger.Debug($"Sperreintrag { .FullName} wird neu angelegt.")
                                       Else
                                           NLogger.Debug($"Sperreintrag { .FullName} wird überschrieben ({UID}).")
                                       End If

                                       ' Erstelle ein entsprechendes XML-Datenobjekt und lade es hoch
                                       If fboxTR064.X_contact.SetCallBarringEntry(.ErstelleXMLKontakt(UID), UID) Then
                                           ' Stelle die Verknüpfung her
                                           .SetUniqueID(SperrlistenID.ToString, UID.ToString)

                                           NLogger.Info($"Kontakt { .FullName} mit der ID '{UID}' in der Sperrliste der Fritz!Box angelegt.")

                                       End If
                                   End With
                               End Sub)
            Next

        Else
            NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
        End If
    End Sub

    Friend Sub AddToCallBarring(OutlookKontakte As IEnumerable(Of ContactItem))
        Using FBoxTR064 = New FBoxAPI.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)
            AddToCallBarring(FBoxTR064, OutlookKontakte)
        End Using
    End Sub

    ''' <summary>
    ''' Löscht den Sperrlisteneintrag mit der entsprechenden <paramref name="UID"/>.
    ''' </summary>
    ''' <param name="UID">UID des zu entfernenden Sperrlisteneintrages</param>
    ''' <returns>True, wenn erfolgreich</returns>
    Friend Function DeleteCallBarring(fboxTR064 As FBoxAPI.FritzBoxTR64, UID As Integer) As Boolean
        ' Prüfe, ob Fritz!Box verfügbar
        If Ping(XMLData.POptionen.ValidFBAdr) Then
            Return fboxTR064.X_contact.DeleteCallBarringEntryUID(UID)
        Else
            NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
            Return False
        End If
    End Function

    ''' <summary>
    ''' Löscht die Sperrlisteneinträge von der Fritz!Box.
    ''' </summary>
    ''' <param name="Einträge">Zu entferndende Sperrlisteneinträge.</param>
    ''' <returns>True, wenn erfolgreich</returns>
    Friend Function DeleteCallBarrings(fboxTR064 As FBoxAPI.FritzBoxTR64, Einträge As IEnumerable(Of FBoxAPI.Contact)) As Boolean
        ' Prüfe, ob Fritz!Box verfügbar
        If Ping(XMLData.POptionen.ValidFBAdr) Then
            With fboxTR064.X_contact
                For Each Kontakt In Einträge
                    If .DeleteCallBarringEntryUID(Kontakt.Uniqueid) Then
                        NLogger.Info($"Eintrag mit der ID '{Kontakt.Uniqueid}' in den Rufsperren der Fritz!Box gelöscht.")

                    Else
                        NLogger.Warn($"Kontakt mit der ID '{Kontakt.Uniqueid}' in den Rufsperren der Fritz!Box nicht gelöscht.")

                    End If
                Next
            End With
            Return True
        Else
            NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
            Return False
        End If
    End Function

    ''' <summary>
    ''' Ermittelt einen Sperrlisteneintrag anhand der übergebenen Telefonnummer
    ''' </summary>
    ''' <param name="Nummer">Telefonnummer</param>
    ''' <param name="Eintrag">Rückgabewert: Sperrlisteintrag als <see cref="ContactViewModel"/></param>
    ''' <returns>True, wenn Suche erfolgreich</returns>
    Private Function GetCallBarringEntryByNum(fboxTR064 As FBoxAPI.FritzBoxTR64, Nummer As String, ByRef Eintrag As ContactViewModel) As Boolean
        ' Prüfe, ob Fritz!Box verfügbar
        If Ping(XMLData.POptionen.ValidFBAdr) Then

            With fboxTR064.X_contact
                Dim EintragsDaten As String = DfltStringEmpty
                If .GetCallBarringEntryByNum(Nummer, EintragsDaten) AndAlso EintragsDaten.IsNotStringNothingOrEmpty Then
                    ' Deserialisiere das Ergebnis
                    If DeserializeXML(EintragsDaten, False, Eintrag) Then
                        NLogger.Info($"Eintrag für die Nummer '{Nummer}' wurde in den Rufsperren der Fritz!Box gefunden (ID '{Eintrag.Uniqueid}').")
                        Return True

                    Else
                        Return False
                        NLogger.Warn($"Deserialisierungsfehler für Sperrlisteneintrag für {Nummer}.")

                    End If
                Else
                    NLogger.Debug($"Eintrag für die Nummer '{Nummer}' wurde in den Rufsperren der Fritz!Box nicht gefunden.")
                    Return False
                End If
            End With
        Else
            NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
            Return False
        End If
    End Function

#Region "Rufsperre"

    ''' <summary>
    ''' Erzeugt einen Sperrlisteneintrag aus einer <see cref="Telefonnummer"/> und lädt diesen auf die Fritz!Box hoch.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer, die der Sperrliste hinzugefügt werden soll.</param>
    Friend Sub AddNrToBlockList(fboxTR064 As FBoxAPI.FritzBoxTR64, TelNr As Telefonnummer)
        AddToCallBarring(fboxTR064, New List(Of String) From {TelNr.Unformatiert}, Localize.resCommon.strCallBarring)
    End Sub
    Friend Sub AddNrToBlockList(TelNr As Telefonnummer)
        Using FBoxTR064 = New FBoxAPI.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)
            AddToCallBarring(FBoxTR064, New List(Of String) From {TelNr.Unformatiert}, Localize.resCommon.strCallBarring)
        End Using
    End Sub

    ''' <summary>
    ''' Erzeugt einen Sperrlisteneintrag aus einer <see cref="IEnumerable(Of String)"/> und lädt diesen auf die Fritz!Box hoch.
    ''' </summary>
    ''' <param name="TelNrListe">Liste an Telefonnummern, die der Sperrliste hinzugefügt werden sollen.</param>
    Friend Sub AddNrToBlockList(fboxTR064 As FBoxAPI.FritzBoxTR64, TelNrListe As IEnumerable(Of String))
        AddToCallBarring(fboxTR064, TelNrListe, Localize.resCommon.strCallBarring)
    End Sub

    ''' <summary>
    ''' Erzeugt einen Sperrlisteneintrag aus einer <see cref="TellowsResponse"/> und lädt diesen auf die Fritz!Box hoch.
    ''' </summary>
    ''' <param name="tellowsResponse">Ergebnis von tellows welches der Sperrliste hinzugefügt werden soll.</param>
    Friend Sub AddNrToBlockList(fboxTR064 As FBoxAPI.FritzBoxTR64, tellowsResponse As TellowsResponse)
        With tellowsResponse
            AddToCallBarring(fboxTR064, New List(Of String) From { .Number}, String.Join(", ", .CallerNames))
        End With
    End Sub

    ''' <summary>
    ''' Erzeugt einen neuen Eintrag als <see cref="FBoxAPI.Contact"/>für die Sperrliste, oder fügt die Nummer einem bestehenden Eintrag hinzu. 
    ''' </summary>
    ''' <param name="FBoxRufSperre">aktuelles Rufsperrentelefonbuch als <see cref="FBoxAPI.Phonebook"/></param>
    ''' <param name="MaxNrbyEntry">Maximale Anzahl an Telefonnummern, die pro Eintrag in der Fritz!Box Rufsperre gespeichert werden sollen.</param>
    ''' <returns>Sperrlisteintrag, der die Nummer enthält. Oder Nothing, falls Nummer bereits in der Sperrliste enthalten ist.</returns>
    Private Function AddTellowsEntry(Eintrag As TellowsScoreListEntry, FBoxRufSperre As PhonebookEx, MaxNrbyEntry As Integer) As FBoxAPI.Contact

        Dim DfltName As String = $"{Eintrag.CallerType} (tellows Score {Eintrag.Score})"

        ' Finde einen Eintrag, der die Nummer bereits enthält
        If FBoxRufSperre.FindbyNumber(Eintrag.Number).Any Then
            NLogger.Trace($"Die Nummer {Eintrag.Number} ist bereits in der Sperrliste enthalten.")
            ' gib Nothing zurück
            Return Nothing
        Else
            ' Finde einen passenden Sperreintrag, der A die richtige Bezeichnung hat und B noch Platz hat
            Dim TellowsSperrEinträge As List(Of FBoxAPI.Contact) = FBoxRufSperre.Phonebook.Contacts.Where(Function(K) K.Person.RealName.AreEqual(DfltName) AndAlso K.Telephony.Numbers.Count.IsLess(MaxNrbyEntry)).ToList

            If TellowsSperrEinträge IsNot Nothing AndAlso TellowsSperrEinträge.Any Then
                NLogger.Debug($"Ein Eintrag für die Nummer {Eintrag.Number} (Score: {Eintrag.Score}) wurde gefunden")
                ' Füge die Nummer dem ersten möglichen Eintrag hinzu
                TellowsSperrEinträge.First.Telephony.Numbers.Add(New FBoxAPI.Number With {.Number = Eintrag.Number})

                Return TellowsSperrEinträge.First
            Else

                NLogger.Debug($"Ein neuer Eintrag für die Nummer {Eintrag.Number} (Score: {Eintrag.Score}) wurde erstellt")
                ' Lege einen neuen Sperrlisteintrag an
                Dim NeuerSperrEintrag As New FBoxAPI.Contact
                With NeuerSperrEintrag
                    .Person.RealName = DfltName
                    .Telephony.Numbers.Add(New FBoxAPI.Number With {.Number = Eintrag.Number})
                End With

                ' Füge den neuen Eintrag dem Telefonbuch hinzu
                FBoxRufSperre.AddContact(NeuerSperrEintrag)

                Return NeuerSperrEintrag

            End If
        End If

    End Function

    ''' <summary>
    ''' Lädt die tellows Scorelist auf die Fritz!Box hoch.
    ''' </summary>
    ''' <param name="MinScore">Mindestscore</param>
    ''' <param name="MaxNrbyEntry">Maximale Anzahl an Telefonnummern, die pro Eintrag in der Fritz!Box Rufsperre gespeichert werden sollen.</param>
    ''' <param name="Einträge">Tellows Sperrliste</param>
    ''' <param name="ct">Abbruchtoken</param>
    ''' <param name="progress">Prozessinformationen</param>
    ''' <returns>Anzahl neu angelegter Telefonnummern</returns>
    Friend Async Function BlockTellowsNumbers(fboxTR064 As FBoxAPI.FritzBoxTR64, MinScore As Integer, MaxNrbyEntry As Integer, Einträge As IEnumerable(Of TellowsScoreListEntry), ct As CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer)
        Return Await Task.Run(Async Function()
                                  Using tellows As New Tellows ', fboxTR064 As New SOAP.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)
                                      ' Lade die Rufsperre herunter
                                      Dim RufsperreFritzBox = Await Telefonbücher.LadeSperrliste(fboxTR064)
                                      ' hochzuladende Einträge
                                      Dim NeueSperrEinträge As New List(Of FBoxAPI.Contact)
                                      ' Anzahl hinzugefügter Nummern
                                      Dim NeueNummern As Integer = 0

                                      NLogger.Debug("Ermittle neue Sperrlisteinträge aus tellows.")

                                      ' Schleife durch alle Einträge, die den Mindesscore erfüllen
                                      For Each Eintrag In Einträge.Where(Function(E) E.Score.IsLargerOrEqual(MinScore))

                                          ' Sucht einen passenden Eintrag in der Sperrliste und fügt die Nummer hinzu
                                          Dim NeuerSperrEintrag = AddTellowsEntry(Eintrag, RufsperreFritzBox.First, MaxNrbyEntry)

                                          ' Falls Nothing, dann ist die Nummer bereits in der Liste
                                          If NeuerSperrEintrag IsNot Nothing Then
                                              ' Lade den Eintrag hoch, wenn die maximale Anzahl erreicht wurde
                                              If NeuerSperrEintrag.Telephony.Numbers.Count.AreDifferentTo(MaxNrbyEntry) AndAlso Not NeueSperrEinträge.Contains(NeuerSperrEintrag) Then
                                                  NeueSperrEinträge.Add(NeuerSperrEintrag)
                                              End If

                                              NeueNummern += 1
                                          End If

                                          If ct.IsCancellationRequested Then Exit For
                                      Next

                                      NLogger.Debug($"Es wurden {NeueSperrEinträge.Count} neue Einträge für {NeueNummern} Nummern erzeugt.")

                                      If NeueSperrEinträge.Any Then
                                          If Windows.MessageBox.Show(String.Format(Localize.LocFBoxData.strQuestionUpdatetellows, NeueSperrEinträge.Count, NeueNummern, MinScore), My.Resources.strDefLongName, Windows.MessageBoxButton.YesNo) = vbYes Then
                                              ' Lade die Sperrlisteinträge hoch
                                              For Each Eintrag In NeueSperrEinträge
                                                  If AddToCallBarring(fboxTR064, Eintrag) Then progress.Report(Eintrag.Telephony.Numbers.Count)

                                                  If ct.IsCancellationRequested Then Exit For
                                              Next

                                              NLogger.Info($"{NeueNummern} neue Nummern der tellows Scorelist (ab Score {MinScore}) in die Fritz!Box Sperrliste ({MaxNrbyEntry} Nummern je Eintrag) übernommen.")
                                          Else
                                              NLogger.Debug("Hochladen auf die Fritz!Box nicht ausgeführt.")
                                          End If
                                      End If

                                      Return NeueNummern
                                  End Using
                              End Function, ct)

    End Function
#End Region

End Module
