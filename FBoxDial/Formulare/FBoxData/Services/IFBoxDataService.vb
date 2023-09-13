Imports System.Threading.Tasks

Public Interface IFBoxDataService
    Sub TR064HttpClient()

    Sub UpdateTheme()

#Region "TAM Anrufbeantworter"
    Function GetTAMItems() As Task(Of IEnumerable(Of FBoxAPI.TAMItem))
    Function GetMessages(TAM As FBoxAPI.TAMItem) As Task(Of IEnumerable(Of FBoxAPI.Message))
    Function ToggleTAM(TAM As FBoxAPI.TAMItem) As Boolean
    Function MarkMessage(Message As FBoxAPI.Message) As Boolean
    Function DeleteMessage(Message As FBoxAPI.Message) As Boolean
    Sub PlayMessage(MessageURL As String)
    Sub StoppMessage(MessageURL As String)
    Sub DownloadMessage(MessageURL As String, FilePath As String)
    Function CompleteURL(PathSegment As String) As String

    Event SoundFinished As EventHandler(Of NotifyEventArgs(Of String))
#End Region

#Region "Anrufliste"
    ''' <summary>
    ''' Ermittle den Zeitpunkt des letzten Journalimportes
    ''' </summary>
    ''' <returns>Date</returns>
    ReadOnly Property GetLastImport() As Date

    ''' <summary>
    ''' Lädt die Anrufliste aus der Fritz!Box asynchron herunter.
    ''' </summary>
    Function GetCallList() As Task(Of FBoxAPI.CallList)

    ''' <summary>
    ''' Erstellt aus dem übegebenen Anruf (<see cref="FBoxAPI.Call"/>) ein Outlook Journaleintrag.
    ''' </summary>
    ''' <param name="Anrufe">Auflistung der zu importierenden Anrufe</param>
    Function ErstelleEinträge(Anrufe As IEnumerable(Of FBoxAPI.Call), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer)

    ''' <summary>
    ''' Lädt die übergebenen Nummern in die Sperrliste der Fritz!Box hoch.
    ''' </summary>
    ''' <param name="Nummern">Nummern, welche gesperrt werden sollen.</param>
    Sub BlockNumbers(Nummern As IEnumerable(Of String))

    ''' <summary>
    ''' Ruft den Kontakt zurück
    ''' </summary>
    ''' <param name="Anruf">Anruf, welcher wiederholt werden soll, oder ein Rückruf erfolgen soll.</param>
    Sub CallXMLContact(Anruf As FBoxAPI.Call)

    Sub ShowXMLContact(Anruf As FBoxAPI.Call)
    Sub SetAppointment(Anruf As FBoxAPI.Call)
    Sub PlayMessage(Anruf As FBoxAPI.Call)
    Sub DownloadFax(Anruf As FBoxAPI.Call)

#End Region

#Region "Deflection - Rufumleitung"
    Function GetDeflectionList() As Task(Of FBoxAPI.DeflectionList)
    Function ToggleRufuml(Deflection As FBoxAPI.Deflection) As Boolean
#End Region

#Region "tellows"
    Function GetTellowsScoreList() As Task(Of List(Of TellowsScoreListEntry))

    Function BlockTellowsNumbers(MinScore As Integer, MaxNrbyEntry As Integer, Einträge As IEnumerable(Of TellowsScoreListEntry), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer)

#End Region

#Region "Telefonbücher"
#Region "Telefonbücher"
    Function GetTelefonbücher() As Task(Of IEnumerable(Of PhonebookEx))
    Function AddTelefonbuch(Name As String) As Task(Of PhonebookEx)
    Function DeleteTelefonbuch(TelefonbuchID As Integer) As Boolean
    Function GetTelefonbuchName(TelefonbuchID As Integer) As String
    Function GetSessionID() As String
#End Region

#Region "Kontakte"
    Function GetKontakt(TelefonbuchID As Integer, UID As Integer) As Task(Of FBoxAPI.Contact)
    Function SetKontakt(TelefonbuchID As Integer, XMLDaten As String) As Integer
    Function DeleteKontakt(TelefonbuchID As Integer, UID As Integer) As Boolean
    Function DeleteKontakte(TelefonbuchID As Integer, Einträge As IEnumerable(Of FBoxAPI.Contact)) As Boolean
    Function LadeKontaktbild(Person As FBoxAPI.Person) As Task(Of Windows.Media.ImageSource)
#End Region

#Region "Rufsperren"
    Function SetRufsperre(XMLDaten As FBoxAPI.Contact) As Integer
    Function DeleteRufsperre(UID As Integer) As Boolean
    Function DeleteRufsperren(Einträge As IEnumerable(Of FBoxAPI.Contact)) As Boolean
#End Region

#Region "Kontakt anrufen"
    Sub Dial(XMLDaten As FBoxAPI.Contact)
#End Region
#End Region

End Interface
