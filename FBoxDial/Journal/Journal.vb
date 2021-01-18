Imports System.Threading.Tasks
Imports Microsoft.Office.Interop
Module Journal
    Private Property Anrufliste As FritzBoxXMLCallList
    'Private Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

    Friend Async Sub AutoAnrListe()
        ' XMLData.POptionen.PLetzterJournalEintragID
        Anrufliste = Await LadeFritzBoxAnrufliste()
        If Anrufliste IsNot Nothing Then
            Await ImportCalls(XMLData.POptionen.LetzterJournalEintrag, Now)
        End If
    End Sub

    Private Function ImportCalls(DatumZeitAnfang As Date, DatumZeitEnde As Date) As Task
        Return Task.Run(Sub()
                            Dim Abfrage As ParallelQuery(Of FritzBoxXMLCall)

                            Abfrage = From Anruf In Anrufliste.Calls.AsParallel() Where (Anruf.Type.IsLessOrEqual(3) And DatumZeitAnfang <= Anruf.Datum And DatumZeitEnde >= Anruf.Datum) Select Anruf
                            Abfrage.ForAll(Sub(r)
                                               Using t As Telefonat = r.ErstelleTelefonat
                                                   t.ErstelleJournalEintrag()
                                               End Using
                                           End Sub)

                        End Sub)
    End Function

    Friend Async Sub StartJournalRWS(olJournal As Outlook.JournalItem)
        With olJournal

            If Not .Body.Contains(DfltStringUnbekannt) And .Categories.Contains(DfltJournalKategorie) Then

                Dim vCard As String
                Dim TelNr As Telefonnummer

                ' Telefonnummer aus dem Body ermitteln
                TelNr = New Telefonnummer With {.SetNummer = olJournal.Body.GetSubString(PfltJournalBodyStart, Dflt1NeueZeile)}
                vCard = Await StartRWS(TelNr, False)

                If vCard.IsStringNothingOrEmpty Then
                    .Body += String.Format("{0}{1}", Dflt1NeueZeile, DfltJournalRWSFehler)
                Else
                    .Body += String.Format("{0}{2}{1}{3}", Dflt1NeueZeile, Dflt2NeueZeile, DfltJournalTextKontaktvCard, vCard)
                End If

            End If
        End With
    End Sub
End Module
