﻿Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.Office.Interop.Outlook

Namespace Telefonbücher
    Friend Module FritzBoxRufsperre
        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Funktionen für das Hinzufügen von Sperreinträgen"
        ''' <summary>
        ''' Fügt einen Sperrlisteneintrag (<see cref="FBoxAPI.Contact"/>) zu der Sperrliste hinzu.
        ''' </summary>
        ''' <param name="Nummern">Auflistung an Telefonnummern, die gesperrt werden sollen.</param>
        ''' <param name="Name">Name des Sperrlisteneintrages</param>
        ''' <param name="UID">Rückgabewert: UID des neuen Sperreintrages</param>
        ''' <returns></returns>
        Friend Function AddToCallBarring(Nummern As IEnumerable(Of String), Name As String, Optional ByRef UID As Integer = 0) As Boolean

            ' Sperreintrag generieren
            Dim Sperreintrag As FBoxAPI.Contact = CreateContact(If(Nummern.Count.AreEqual(1), Nummern.First, $"{Name} ({Nummern.Count})"))

            With Sperreintrag

                ' Prüfe, ob die übergebenen Nummern bereits auf der Rufsperre der Fritz!Box vorhanden sind.
                ' Ein Eintrag auf der Fritz!Box kann mehrere Telefonnummern enthalten
                For Each TelNr In Nummern

                    If IsNumOnCallBarringList(TelNr) Then
                        ' Ein Eintrag mit der Nummer bereits vorhanden
                        NLogger.Info($"Ein Eintrag mit der '{TelNr}' ist in der Sperrliste bereits vorhanden.")
                    Else
                        ' füge die Telefonnummer dem hinzuzufügenden Sperreintrag hinzu
                        .Telephony.Numbers.Add(New FBoxAPI.NumberType With {.Number = TelNr})
                    End If
                Next

                ' Prüfe, ob der Sperreintrag überhaupt Nummern hat.
                If .Telephony.Numbers.Any Then
                    ' Fügen den neuen Sperreintrag dem heruntergeladenen Telefonbuch hinzu
                    'With Globals.ThisAddIn.PhoneBookXML.Where(Function(Buch) Buch.CallBarringBook)
                    '    If .Any Then
                    '        .First.AddContact(Sperreintrag)
                    '    End If
                    'End With

                    ' lade den Sperreintrag hoch
                    Return Globals.ThisAddIn.FBoxTR064.X_contact.SetCallBarringEntry(Sperreintrag.GetXMLKontakt, UID)
                Else
                    Return False
                End If

            End With

        End Function

        ''' <summary>
        ''' Fügt einen Sperrlisteneintrag (<see cref="ContactViewModel"/>) zu der Sperrliste hinzu.
        ''' </summary>
        ''' <param name="Sperreintrag">Sperrlisteneintrag</param>
        ''' <param name="UID">Rückgabewert: UID des neuen Sperreintrages</param>
        Friend Function AddToCallBarring(Sperreintrag As FBoxAPI.Contact, Optional ByRef UID As Integer = 0) As Boolean
            Return Globals.ThisAddIn.FBoxTR064.X_contact.SetCallBarringEntry(Sperreintrag.GetXMLKontakt, UID)
        End Function

        ''' <summary>
        ''' Fügt eine Auflistung von Outlook Kontakten (<see cref="ContactItem"/>) zu der Sperrliste hinzu.
        ''' </summary>
        ''' <param name="OutlookKontakte">Auflistung von Sperrlisteneinträgen</param>
        Friend Async Sub AddToCallBarring(OutlookKontakte As IEnumerable(Of ContactItem))

            ' Erzeuge für jeden Kontakt einen Eintrag
            For Each Kontakt In OutlookKontakte
                Await Task.Run(Sub()

                                   With Kontakt
                                       ' Überprüfe, ob es in diesem Telefonbuch bereits einen verknüpften Kontakt gibt
                                       Dim UID As Integer = Kontakt.GetUniqueID(FritzBoxDefault.DfltCallBarringID)

                                       If UID.AreEqual(-1) Then
                                           NLogger.Debug($"Sperreintrag { .FullName} wird neu angelegt.")
                                       Else
                                           NLogger.Debug($"Sperreintrag { .FullName} wird überschrieben ({UID}).")
                                       End If

                                       ' Erstelle ein entsprechendes XML-Datenobjekt und lade es hoch
                                       If Globals.ThisAddIn.FBoxTR064.X_contact.SetCallBarringEntry(.ErstelleXMLKontakt(UID), UID) Then
                                           ' Stelle die Verknüpfung her
                                           .SetUniqueID(FritzBoxDefault.DfltCallBarringID.ToString, UID.ToString, True)

                                           NLogger.Info($"Kontakt { .FullName} mit der ID '{UID}' in der Sperrliste der Fritz!Box angelegt.")

                                       End If
                                   End With
                               End Sub)
            Next

        End Sub

        ''' <summary>
        ''' Erzeugt einen Sperrlisteneintrag aus einer <see cref="Telefonnummer"/> und lädt diesen auf die Fritz!Box hoch.
        ''' </summary>
        ''' <param name="TelNr">Telefonnummer, die der Sperrliste hinzugefügt werden soll.</param>
        Friend Sub AddNrToBlockList(TelNr As Telefonnummer)
            AddToCallBarring(New List(Of String) From {TelNr.Unformatiert}, TelNr.Unformatiert)
        End Sub

        ''' <summary>
        ''' Erzeugt einen Sperrlisteneintrag aus einer <see cref="IEnumerable(Of String)"/> und lädt diesen auf die Fritz!Box hoch.
        ''' </summary>
        ''' <param name="TelNrListe">Liste an Telefonnummern, die der Sperrliste hinzugefügt werden sollen.</param>
        Friend Sub AddNrToBlockList(TelNrListe As IEnumerable(Of String))
            AddToCallBarring(TelNrListe, Localize.resCommon.strCallBarring)
        End Sub
#End Region

#Region "Funktionen für das Entfernen von Sperreinträgen"
        ''' <summary>
        ''' Löscht den Sperrlisteneintrag mit der entsprechenden <paramref name="UID"/>.
        ''' </summary>
        ''' <param name="UID">UID des zu entfernenden Sperrlisteneintrages</param>
        ''' <returns>True, wenn erfolgreich</returns>
        Friend Function DeleteCallBarring(UID As Integer) As Boolean
            Return Globals.ThisAddIn.FBoxTR064.X_contact.DeleteCallBarringEntryUID(UID)
        End Function

        ''' <summary>
        ''' Löscht die Sperrlisteneinträge von der Fritz!Box.
        ''' </summary>
        ''' <param name="Einträge">Zu entferndende Sperrlisteneinträge.</param>
        ''' <returns>True, wenn erfolgreich</returns>
        Friend Function DeleteCallBarrings(Einträge As IEnumerable(Of FBoxAPI.Contact)) As Boolean
            With Globals.ThisAddIn.FBoxTR064.X_contact
                For Each Kontakt In Einträge
                    If .DeleteCallBarringEntryUID(Kontakt.Uniqueid) Then
                        NLogger.Info($"Eintrag mit der ID '{Kontakt.Uniqueid}' in den Rufsperren der Fritz!Box gelöscht.")

                    Else
                        NLogger.Warn($"Kontakt mit der ID '{Kontakt.Uniqueid}' in den Rufsperren der Fritz!Box nicht gelöscht.")

                    End If
                Next
            End With
            Return True
        End Function
#End Region

#Region "Allgemeine Funktionen und Abfragen"
        ''' <summary>
        ''' Ermittelt einen Sperrlisteneintrag anhand der übergebenen Telefonnummer
        ''' </summary>
        ''' <param name="Nummer">Telefonnummer</param>
        ''' <param name="Eintrag">Rückgabewert: Sperrlisteintrag als <see cref="FBoxAPI.Contact"/></param>
        ''' <returns>True, wenn Suche erfolgreich</returns>
        Private Function GetCallBarringEntryByNum(Nummer As String, ByRef Eintrag As FBoxAPI.Contact) As Boolean
            With Globals.ThisAddIn.FBoxTR064.X_contact

                Dim EintragsDaten As String = String.Empty
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
                    Return True
                End If
            End With
        End Function

        Private Function IsNumOnCallBarringList(Nummer As String) As Boolean
            With Globals.ThisAddIn.FBoxTR064.X_contact

                Dim EintragsDaten As String = String.Empty
                If .GetCallBarringEntryByNum(Nummer, EintragsDaten) AndAlso EintragsDaten.IsNotStringNothingOrEmpty Then

                    NLogger.Info($"Eintrag für die Nummer '{Nummer}' wurde in den Rufsperren der Fritz!Box gefunden.")
                    Return True
                Else
                    NLogger.Debug($"Eintrag für die Nummer '{Nummer}' wurde in den Rufsperren der Fritz!Box nicht gefunden.")
                    Return False
                End If
            End With
        End Function

        ''' <summary>
        ''' Überprüft, ob die Telefonnummer durch die Fitz!Box gesperrt ist. Hierzu wird die Sperrliste abgefragt und alle Rufbehandlungen geprüft, die zur Rufsperre dienen.
        ''' </summary>
        ''' <param name="TelNr">Zu prüfende Nummer</param>
        Friend Function IsFBoxBlocked(TelNr As Telefonnummer) As Boolean

            If Globals.ThisAddIn.FBoxTR064?.Ready Then

                ' Ist die Nummer auf der Rufsperre der Fritz!Box
                If IsNumOnCallBarringList(TelNr.Unformatiert) Then Return True

                Dim DeflectionList As New FBoxAPI.DeflectionList

                ' Lade alle Rufbehandlungen herunter
                If Globals.ThisAddIn.FBoxTR064.X_contact.GetDeflections(DeflectionList) Then

                    ' Temporäres Ausgeben der aktuellen Rufbehandlungen 
                    'Dim r As String = String.Empty
                    'If XmlSerializeToString(DeflectionList, r) Then NLogger.Debug($"Aktuelle Rufbehandlungen: {r}")

                    ' Filtere alle aktiven Rufbehandlungen, die zur Rufsperre dienen
                    With DeflectionList.Deflections.Where(Function(D) D.Enable AndAlso D.Mode = FBoxAPI.DeflectionModeEnum.eNoSignal)

                        If .Any Then ' Es gibt aktive Rufbehandlungen, die zur Rufsperre dienen

                            ' Unterscheidung: Rufnummer unterdrückt
                            If TelNr.Unterdrückt Then
                                ' 2. Abfrage, ob unterdrückte Nummern generell nicht signalisiert werden.
                                ' Finde eine Rufbehandlung, nach der unterdrückte Nummern (DeflectionType.fromAnonymous) nicht signalisiert (DeflectionMode.eNoSignal) werden.
                                Return .Where(Function(D) D.Type = FBoxAPI.DeflectionTypeEnum.fromAnonymous).Any
                            Else

                                ' Prüfe, ob Rufsperren für gesamte Telefonbücher vorhanden sind.
                                With .Where(Function(D) D.Type = FBoxAPI.DeflectionTypeEnum.fromPB)
                                    ' Ermittle alle Telefonbücher, die zur Rufsperre dienen und prüfe, ob die Telefonnummer enthalten ist.
                                    If .Any AndAlso .Select(Function(DEF) Globals.ThisAddIn.PhoneBookXML.Where(Function(PB) PB.ID.AreEqual(DEF.PhonebookID.ToInt)).First).Contains(TelNr) Then
                                        NLogger.Debug($"Die Nummer '{TelNr.Unformatiert}' ist in einem Telefonbuch enthalten, welches als Rufsperre dient.")
                                        Return True
                                    End If
                                End With

                                ' Rufsperre für Nummern, die mit Zeichenfolge beginnen
                                With .Where(Function(D) D.Type = FBoxAPI.DeflectionTypeEnum.fromNumber AndAlso TelNr.Unformatiert.StartsWith(D.Number))
                                    If .Any Then
                                        NLogger.Debug($"Für die Nummer '{TelNr.Unformatiert}' existiert eine passende Rufbehandlung { .First.DeflectionId}: { .First.Number}")
                                        Return True
                                    End If
                                End With

                                ' Rufsperre für alle eingehenden Telefonate, die nicht auf dem Telefonbuch sind
                                With .Where(Function(D) D.Type = FBoxAPI.DeflectionTypeEnum.fromNotInPhonebook AndAlso Not Globals.ThisAddIn.PhoneBookXML.Contains(TelNr))
                                    If .Any Then
                                        NLogger.Debug($"Die Nummer '{TelNr.Unformatiert}' ist in keinem Telefonbuch enthalten (Rufbehandlung: { .First.DeflectionId} { .First.Type})")
                                        Return True
                                    End If
                                End With
                            End If
                        End If
                    End With
                End If

            End If

                        Return False
        End Function
#End Region

#Region "Funktionen im Zusammenhang mit tellows"
        ''' <summary>
        ''' Erzeugt einen neuen Eintrag als <see cref="FBoxAPI.Contact"/>für die Sperrliste, oder fügt die Nummer einem bestehenden Eintrag hinzu. 
        ''' </summary>
        ''' <param name="FBoxRufSperre">aktuelles Rufsperrentelefonbuch als <see cref="FBoxAPI.Phonebook"/></param>
        ''' <param name="MaxNrbyEntry">Maximale Anzahl an Telefonnummern, die pro Eintrag in der Fritz!Box Rufsperre gespeichert werden sollen.</param>
        ''' <returns>Sperrlisteintrag, der die Nummer enthält. Oder Nothing, falls Nummer bereits in der Sperrliste enthalten ist.</returns>
        Private Function AddTellowsEntry(Eintrag As TellowsScoreListEntry, FBoxRufSperre As PhonebookEx, MaxNrbyEntry As Integer) As FBoxAPI.Contact

            Dim DfltName As String = $"{Eintrag.CallerType} (tellows Score {Eintrag.Score})"

            ' Finde einen Eintrag, der die Nummer bereits enthält
            If FBoxRufSperre.GetContact(Eintrag.Number).Any Then
                NLogger.Trace($"Die Nummer {Eintrag.Number} ist bereits in der Sperrliste enthalten.")
                ' gib Nothing zurück
                Return Nothing
            Else
                ' Finde einen passenden Sperreintrag, der A die richtige Bezeichnung hat und B noch Platz hat
                Dim TellowsSperrEinträge As List(Of FBoxAPI.Contact) = FBoxRufSperre.Phonebook.Contacts.Where(Function(K) K.Person.RealName.IsEqual(DfltName) AndAlso K.Telephony.Numbers.Count.IsLess(MaxNrbyEntry)).ToList

                If TellowsSperrEinträge IsNot Nothing AndAlso TellowsSperrEinträge.Any Then
                    NLogger.Debug($"Ein Eintrag für die Nummer {Eintrag.Number} (Score: {Eintrag.Score}) wurde gefunden")
                    ' Füge die Nummer dem ersten möglichen Eintrag hinzu
                    TellowsSperrEinträge.First.Telephony.Numbers.Add(New FBoxAPI.NumberType With {.Number = Eintrag.Number})

                    Return TellowsSperrEinträge.First
                Else

                    NLogger.Debug($"Ein neuer Eintrag für die Nummer {Eintrag.Number} (Score: {Eintrag.Score}) wurde erstellt")
                    ' Lege einen neuen Sperrlisteintrag an
                    Dim NeuerSperrEintrag As FBoxAPI.Contact = CreateContact(DfltName)

                    With NeuerSperrEintrag
                        .Telephony.Numbers.Add(New FBoxAPI.NumberType With {.Number = Eintrag.Number})
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
        Friend Async Function BlockTellowsNumbers(MinScore As Integer, MaxNrbyEntry As Integer, Einträge As IEnumerable(Of TellowsScoreListEntry), ct As CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer)
            Return Await Task.Run(Async Function()
                                      Using tellows As New Tellows
                                          ' Lade die Rufsperre herunter
                                          Dim RufsperreFritzBox = Await Telefonbücher.LadeSperrliste()
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
                                                      If AddToCallBarring(Eintrag) Then progress.Report(Eintrag.Telephony.Numbers.Count)

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
End Namespace