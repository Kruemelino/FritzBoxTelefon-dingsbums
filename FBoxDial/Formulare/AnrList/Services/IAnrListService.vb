Public Interface IAnrListService
    ''' <summary>
    ''' Ermittle den Zeitpunkt des letzten Journalimportes
    ''' </summary>
    ''' <returns>Date</returns>
    ReadOnly Property GetLastImport() As Date

    ''' <summary>
    ''' Lädt die Anrufliste aus der Fritz!Box herunter
    ''' </summary>
    ''' <returns>FritzBoxXMLCallList</returns>
    Function GetAnrufListe() As Threading.Tasks.Task(Of FritzBoxXMLCallList)

    ''' <summary>
    ''' Erstellt aus dem übegebenen Anruf (<see cref="FritzBoxXMLCall"/>) ein Outlook Journaleintrag.
    ''' </summary>
    ''' <param name="Anruf">Der zu verarbeitende Anruf.</param>
    Sub ErstelleEintrag(Anruf As FritzBoxXMLCall)

End Interface
