Imports System.Threading.Tasks
Imports Microsoft.Office.Interop
Friend Module Journal
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Async Sub AutoAnrListe(Anrufliste As FBoxAPI.CallList)

        If Anrufliste IsNot Nothing Then
            NLogger.Debug($"Anrufliste mit {Anrufliste.Calls.Count} Einträgen geladen.")

            ' Starte die Auswertung der Anrufliste
            Await ImportCalls(Anrufliste.Calls, XMLData.POptionen.LetzteAuswertungAnrList, Now)

        End If
    End Sub

    Friend Async Sub AutoBlockListe(fboxTR064 As FBoxAPI.FritzBoxTR64)

        With XMLData.POptionen
            If Now.Subtract(.LetzteSperrlistenAktualisierung).TotalHours.IsLargerOrEqual(24) Then
                NLogger.Debug("Rufsperre der Fritz!Box wird aktualisiert.")

                Dim CTS = New Threading.CancellationTokenSource
                Dim progressIndicator = New Progress(Of Integer)(Sub(status)
                                                                 End Sub)

                Await BlockTellowsNumbers(fboxTR064, .CBTellowsAutoScoreFBBlockList, .CBTellowsEntryNumberCount, ThisAddIn.TellowsScoreList, CTS.Token, progressIndicator)

                .LetzteSperrlistenAktualisierung = Now

                CTS.Dispose()
            Else
                NLogger.Debug("Rufsperre der Fritz!Box wird nicht aktualisiert.")
            End If

        End With
    End Sub

    Private Function ImportCalls(Anrufliste As IEnumerable(Of FBoxAPI.Call), DatumZeitAnfang As Date, DatumZeitEnde As Date) As Task
        Return Task.Run(Sub()
                            Dim Abfrage As ParallelQuery(Of FBoxAPI.Call)

                            Abfrage = From Anruf In Anrufliste.AsParallel() Where Anruf.Type.IsLessOrEqual(3) And DatumZeitAnfang <= CDate(Anruf.Date) And DatumZeitEnde >= CDate(Anruf.Date) Select Anruf

                            Abfrage.ForAll(Async Sub(Anruf)
                                               ' in ErstelleTelefonat wird auch die Wahlwiederholungs- und Rückrufliste ausgewertet.
                                               Using t As Telefonat = Await ErstelleTelefonat(Anruf)
                                                   If t IsNot Nothing Then t.SetUpOlLists()
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

    Friend Sub UpdateTimeAnrList()
        ' Merke die Zeit
        If XMLData.POptionen.LetzteAuswertungAnrList < Now Then XMLData.POptionen.LetzteAuswertungAnrList = Now
    End Sub
End Module
