Public Class AnrListService
    Implements IAnrListService

    Friend ReadOnly Property GetLastImport() As Date Implements IAnrListService.GetLastImport
        Get
            Return XMLData.POptionen.LetzterJournalEintrag
        End Get
    End Property

    Friend Async Function GetAnrufListe() As Threading.Tasks.Task(Of FritzBoxXMLCallList) Implements IAnrListService.GetAnrufListe
        Return Await LadeFritzBoxAnrufliste()
    End Function

    Public Sub ErstelleEintrag(Anruf As FritzBoxXMLCall) Implements IAnrListService.ErstelleEintrag
        Anruf.ErstelleTelefonat.ErstelleJournalEintrag()
    End Sub
End Class
