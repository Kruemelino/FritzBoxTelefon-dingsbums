Public Class AnrListService
    Implements IAnrListService

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend ReadOnly Property GetLastImport() As Date Implements IAnrListService.GetLastImport
        Get
            Return XMLData.POptionen.LetzterJournalEintrag
        End Get
    End Property

    Friend Async Function GetAnrufListe() As Threading.Tasks.Task(Of FritzBoxXMLCallList) Implements IAnrListService.GetAnrufListe
        Return Await LadeFritzBoxAnrufliste()
    End Function

    Friend Sub ErstelleEintrag(Anruf As FritzBoxXMLCall) Implements IAnrListService.ErstelleEintrag
        Anruf.ErstelleTelefonat.ErstelleJournalEintrag()
    End Sub

    Friend Sub BlockNumbers(TelNrListe As IEnumerable(Of String)) Implements IAnrListService.BlockNumbers

        If TelNrListe.Any Then

            Dim Sperreintrag As New FritzBoxXMLKontakt
            Sperreintrag.Person.RealName = My.Resources.strDefLongName

            With Sperreintrag

                For Each TelNr In TelNrListe
                    .Telefonie.Nummern.Add(New FritzBoxXMLNummer With {.Nummer = TelNr})
                Next

            End With
            Threading.Tasks.Task.Run(Sub()
                                         If AddToCallBarring(Sperreintrag) Then
                                             NLogger.Info($"Die Nummer {Sperreintrag.Telefonie.Nummern} wurde(n) der Sperrliste hinzugefügt.")
                                         Else
                                             NLogger.Warn($"Die Nummer {Sperreintrag.Telefonie.Nummern} wurde(n) der Sperrliste nicht hinzugefügt.")
                                         End If
                                     End Sub)

        End If

    End Sub
End Class
