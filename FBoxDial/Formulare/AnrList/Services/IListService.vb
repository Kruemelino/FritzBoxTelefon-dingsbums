Imports System.Threading.Tasks
Public Interface IListService

#Region "Anrufliste"
    ''' <summary>
    ''' Ermittle den Zeitpunkt des letzten Journalimportes
    ''' </summary>
    ''' <returns>Date</returns>
    ReadOnly Property GetLastImport() As Date

    ''' <summary>
    ''' Lädt die Anrufliste aus der Fritz!Box herunter
    ''' </summary>
    ''' <returns>FritzBoxXMLCallList</returns>
    Function GetAnrufListe() As Task(Of FritzBoxXMLCallList)

    ''' <summary>
    ''' Erstellt aus dem übegebenen Anruf (<see cref="FritzBoxXMLCall"/>) ein Outlook Journaleintrag.
    ''' </summary>
    ''' <param name="Anrufe">Auflistung der zu importierenden Anrufe</param>
    Function ErstelleEinträge(Anrufe As IEnumerable(Of FritzBoxXMLCall), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer)
#End Region

#Region "tellows"
    Function GetTellowsScoreList() As Task(Of List(Of TellowsScoreListEntry))

    Function BlockTellowsNumbers(MinScore As Integer, MaxNrbyEntry As Integer, Einträge As IEnumerable(Of TellowsScoreListEntry), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer)

#End Region

    ''' <summary>
    ''' Lädt die übergebenen Nummern in die Sperrliste der Fritz!Box hoch.
    ''' </summary>
    ''' <param name="Nummern">Nummern, welche gesperrt werden sollen.</param>
    Sub BlockNumbers(Nummern As IEnumerable(Of String))

    ''' <summary>
    ''' Ruft den Kontakt zurück
    ''' </summary>
    ''' <param name="Kontakt">Anruf, welcher wiederholt werden soll, oder ein Rückruf erfolgen soll.</param>
    Sub CallXMLContact(Kontakt As FritzBoxXMLCall)

    Sub ShowXMLContact(Kontakt As FritzBoxXMLCall)
End Interface
