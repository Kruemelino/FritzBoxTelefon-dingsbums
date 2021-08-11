Imports System.Threading.Tasks
Imports Microsoft.Office.Interop
Friend Module Journal
    Private Property Anrufliste As FritzBoxXMLCallList
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Async Sub AutoAnrListe()

        ' Lade die Anruflise aus der Fritz!Box herunter
        Anrufliste = Await LadeFritzBoxAnrufliste()

        If Anrufliste IsNot Nothing Then
            NLogger.Debug($"Anrufliste mit {Anrufliste.Calls.Count} Einträgen geladen.")

            ' Starte die Auswertung der Anrufliste
            Await ImportCalls(XMLData.POptionen.LetzteAuswertungAnrList, Now)

            ' Merke die Zeit
            If XMLData.POptionen.LetzteAuswertungAnrList < Now Then XMLData.POptionen.LetzteAuswertungAnrList = Now
        End If
    End Sub

    Friend Async Sub AutoBlockListe()

        With XMLData.POptionen
            If Now.Subtract(.LetzteSperrlistenaktualisierung).TotalHours.IsLargerOrEqual(24) Then
                NLogger.Debug("Rufsperre der Fritz!Box wird aktualisiert.")

                Dim CTS = New Threading.CancellationTokenSource
                Dim progressIndicator = New Progress(Of Integer)(Sub(status)
                                                                 End Sub)

                Await BlockTellowsNumbers(.CBTellowsAutoScoreFBBlockList, .CBTellowsEntryNumberCount, ThisAddIn.TellowsScoreList, CTS.Token, progressIndicator)

                .LetzteSperrlistenaktualisierung = Now

                CTS.Dispose()
            Else
                NLogger.Debug("Rufsperre der Fritz!Box wird nicht aktualisiert.")
            End If

        End With
    End Sub

    Private Function ImportCalls(DatumZeitAnfang As Date, DatumZeitEnde As Date) As Task
        Return Task.Run(Sub()
                            Dim Abfrage As ParallelQuery(Of FritzBoxXMLCall)

                            Abfrage = From Anruf In Anrufliste.Calls.AsParallel() Where Anruf.Type.IsLessOrEqual(3) And DatumZeitAnfang <= Anruf.Datum And DatumZeitEnde >= Anruf.Datum Select Anruf
                            Abfrage.ForAll(Sub(r)
                                               ' in ErstelleTelefonat wird auch die Wahlwiederholungs- und Rückrufliste ausgewertet.
                                               Using t As Telefonat = r.ErstelleTelefonat
                                                   ' Erstelle einen Journaleintrag
                                                   t.ErstelleJournalEintrag()
                                               End Using
                                           End Sub)

                        End Sub)
    End Function

    Friend Async Sub StartJournalRWS(olJournal As Outlook.JournalItem)
        With olJournal

            If Not .Body.Contains(Localize.LocAnrMon.strNrUnterdrückt) And .Categories.Contains(Localize.LocAnrMon.strJournalCatDefault) Then

                Dim vCard As String
                Dim TelNr As Telefonnummer

                ' Telefonnummer aus dem Body ermitteln
                TelNr = New Telefonnummer With {.SetNummer = olJournal.Body.GetSubString(Localize.LocAnrMon.strJournalBodyStart, Dflt1NeueZeile)}
                vCard = Await StartRWS(TelNr, False)

                If vCard.IsStringNothingOrEmpty Then
                    .Body += String.Format($"{Dflt1NeueZeile}{Localize.LocAnrMon.strJournalFehler}")
                Else
                    .Body += String.Format($"{Dflt1NeueZeile}{Localize.LocAnrMon.strJournalTextvCard}{Dflt2NeueZeile}{vCard}")
                End If

            End If
        End With
    End Sub
End Module
