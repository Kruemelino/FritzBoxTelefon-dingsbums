Imports System.Threading.Tasks

Public Interface IFBoxDataService
    Sub Finalize()

#Region "TAM Anrufbeantworter"
    Function GetTAMList() As Task(Of TAMList)
    Sub ToggleTAM(TAM As TAMItem)
    Function MarkMessage(Message As FritzBoxXMLMessage) As Boolean
    Function DeleteMessage(Message As FritzBoxXMLMessage) As Boolean
    Sub PlayMessage(Message As FritzBoxXMLMessage)
#End Region

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
#End Region

#Region "tellows"
    Function GetTellowsScoreList() As Task(Of List(Of TellowsScoreListEntry))

    Function BlockTellowsNumbers(MinScore As Integer, MaxNrbyEntry As Integer, Einträge As IEnumerable(Of TellowsScoreListEntry), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer)

#End Region

#Region "Telefonbücher"
#Region "Fritz!Box Telefonbücher"
    Function GetTelefonbücher() As Task(Of FritzBoxXMLTelefonbücher)
    Function AddTelefonbuch(Name As String) As Task(Of FritzBoxXMLTelefonbuch)
    Function DeleteTelefonbuch(TelefonbuchID As Integer) As Boolean
    Function GetSessionID() As String
#End Region

#Region "Fritz!Box Kontakte"
    Function SetKontakt(TelefonbuchID As Integer, XMLDaten As String) As Integer
    Function DeleteKontakt(TelefonbuchID As Integer, UID As Integer) As Boolean
    Function DeleteKontakte(TelefonbuchID As Integer, Einträge As IEnumerable(Of FritzBoxXMLKontakt)) As Boolean
#End Region

#Region "Fritz!Box Rufsperren"
    Function SetRufsperre(XMLDaten As FritzBoxXMLKontakt) As Integer
    Function DeleteRufsperre(UID As Integer) As Boolean
    Function DeleteRufsperren(Einträge As IEnumerable(Of FritzBoxXMLKontakt)) As Boolean
#End Region

#Region "Kontakt anrufen"
    Sub Dial(XMLDaten As FritzBoxXMLKontakt)
#End Region
#End Region

#Region "Deflection - Rufumleitung"
    Function GestDeflectionList() As Task(Of DeflectionList)
    Sub ToggleRufuml(Deflection As DeflectionInfo)
#End Region
End Interface
