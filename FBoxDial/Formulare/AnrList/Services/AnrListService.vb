Public Class AnrListService
    Implements IAnrListService
    ''' <summary>
    ''' Ermittle den Zeitpunkt des letzten Journalimportes
    ''' </summary>
    ''' <returns>Date</returns>
    Friend ReadOnly Property GetLastImport() As Date Implements IAnrListService.GetLastImport
        Get
            Return XMLData.POptionen.LetzterJournalEintrag
        End Get
    End Property

    ''' <summary>
    ''' Lädt die Anrufliste aus der Fritz!Box herunter
    ''' </summary>
    ''' <returns>FritzBoxXMLCallList</returns>
    Friend Async Function GetAnrufListe() As Threading.Tasks.Task(Of FritzBoxXMLCallList) Implements IAnrListService.GetAnrufListe
        Return Await LadeFritzBoxAnrufliste()
    End Function

    ''' <summary>
    ''' Erstellt aus dem übegebenen Anruf (<see cref="FritzBoxXMLCall"/>) ein Outlook Journaleintrag.
    ''' </summary>
    ''' <param name="Anruf">Der zu verarbeitende Anruf.</param>
    Public Sub ErstelleEintrag(Anruf As FritzBoxXMLCall) Implements IAnrListService.ErstelleEintrag
        Anruf.ErstelleTelefonat.ErstelleJournalEintrag()
    End Sub
End Class
