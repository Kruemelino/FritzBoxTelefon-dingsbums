Imports System.Threading.Tasks
Imports Microsoft.Office.Interop
Friend Module Journal
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Async Sub AutoAnrListe(Anrufliste As FBoxAPI.CallList)

        If Anrufliste IsNot Nothing Then
            NLogger.Debug($"Werte {Anrufliste.Calls.Count} neue Telefonate der Anrufliste aus.")

            ' Starte die Auswertung der Anrufliste
            Await Task.Run(Sub()
                               Dim Abfrage As ParallelQuery(Of FBoxAPI.Call)

                               Abfrage = From Anruf In Anrufliste.Calls.AsParallel() Where Anruf.Type.IsLessOrEqual(3) And
                                                                                           XMLData.POptionen.LetzteAuswertungAnrList <= CDate(Anruf.Date) And
                                                                                           Now >= CDate(Anruf.Date) Select Anruf

                               Abfrage.ForAll(Async Sub(Anruf)
                                                  Using t As Telefonat = Await ErstelleTelefonat(Anruf)
                                                      ' Aktualisiere die Wahlwiederholungs- und Rückrufliste
                                                      ' Erstelle Journaleinträge,
                                                      ' Ergänze das Callpane
                                                      If t IsNot Nothing Then t.SetUpOlLists(True)
                                                  End Using
                                              End Sub)
                           End Sub)

        End If
    End Sub

    Friend Async Sub AutoBlockListe()

        With XMLData.POptionen
            If Now.Subtract(.LetzteSperrlistenAktualisierung).TotalHours.IsLargerOrEqual(24) Then
                NLogger.Debug("Rufsperre der Fritz!Box wird aktualisiert.")

                Dim CTS = New Threading.CancellationTokenSource
                Dim progressIndicator = New Progress(Of Integer)(Sub(status)
                                                                 End Sub)

                Await BlockTellowsNumbers(.CBTellowsAutoScoreFBBlockList, .CBTellowsEntryNumberCount, Globals.ThisAddIn.TellowsScoreList, CTS.Token, progressIndicator)

                .LetzteSperrlistenAktualisierung = Now

                CTS.Dispose()
            Else
                NLogger.Debug("Rufsperre der Fritz!Box wird nicht aktualisiert.")
            End If

        End With
    End Sub

    Friend Async Sub StartOlItemRWS(Of T)(olItem As T)
        Select Case True
            Case TypeOf olItem Is Outlook.AppointmentItem
                With CType(olItem, Outlook.AppointmentItem)
                    .Body = Await GetRWSResponse(.Body, .Categories)
                End With

            Case TypeOf olItem Is Outlook.JournalItem
                With CType(olItem, Outlook.JournalItem)
                    .Body = Await GetRWSResponse(.Body, .Categories)
                End With

        End Select

    End Sub

    Private Async Function GetRWSResponse(Body As String, Categories As String) As Task(Of String)
        If Not Body.Contains(Localize.LocAnrMon.strNrUnterdrückt) And Categories.Contains(Localize.LocAnrMon.strJournalCatDefault) Then

            Dim vCard As String
            Dim TelNr As Telefonnummer

            ' Telefonnummer aus dem Body ermitteln
            TelNr = New Telefonnummer With {.SetNummer = Body.GetSubString(Localize.LocAnrMon.strJournalBodyStart, vbCrLf)}
            vCard = Await StartRWS(TelNr, False)

            If vCard.IsStringNothingOrEmpty Then
                Body += String.Format($"{vbCrLf}{Localize.LocAnrMon.strJournalFehler}")
            Else
                Body += String.Format($"{vbCrLf}{Localize.LocAnrMon.strJournalTextvCard}{vbCrLf & vbCrLf}{vCard}")
            End If

        End If

        Return Body
    End Function

    Friend Sub UpdateTimeAnrList()
        ' Merke die Zeit
        If XMLData.POptionen.LetzteAuswertungAnrList < Now Then XMLData.POptionen.LetzteAuswertungAnrList = Now
    End Sub
End Module
