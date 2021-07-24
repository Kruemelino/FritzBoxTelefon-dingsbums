Imports System.Threading.Tasks
Imports Microsoft.Office.Interop
Friend Module Journal
    Private Property Anrufliste As FritzBoxXMLCallList
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Async Sub AutoAnrListe()

        ' Lade die Anruflise aus der Fritz!Box herunter
        Dim TaskAnrufListe As Task(Of FritzBoxXMLCallList) = LadeFritzBoxAnrufliste()
        Dim TaskScoreListe As Task(Of List(Of TellowsScoreListEntry)) = Nothing

        ' Lade die tellows Liste
        If XMLData.POptionen.CBTellows Then
            Using tellows As New Tellows
                TaskScoreListe = tellows.LadeScoreList
            End Using
        End If

        Await TaskAnrufListe
        Anrufliste = TaskAnrufListe.Result

        If TaskScoreListe IsNot Nothing Then
            Await TaskScoreListe
            ThisAddIn.TellowsScoreList = TaskScoreListe.Result
            NLogger.Debug($"tellows Scorelist mit {ThisAddIn.TellowsScoreList.Count} Einträgen geladen.")
        End If

        If Anrufliste IsNot Nothing Then
            NLogger.Debug($"Anrufliste mit {Anrufliste.Calls.Count} Einträgen geladen.")

            ' Starte die Auswertung der Anrufliste
            Await ImportCalls(XMLData.POptionen.LetzterJournalEintrag, Now)
        End If
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
                    .Body += String.Format("{0}{1}", Dflt1NeueZeile, Localize.LocAnrMon.strJournalFehler)
                Else
                    .Body += String.Format("{0}{2}{1}{3}", Dflt1NeueZeile, Dflt2NeueZeile, Localize.LocAnrMon.strJournalTextvCard, vCard)
                End If

            End If
        End With
    End Sub
End Module
