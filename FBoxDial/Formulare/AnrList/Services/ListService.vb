Imports System.Threading.Tasks
Public Class ListService
    Implements IListService

    'Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
#Region "Anrufliste"
    Friend ReadOnly Property GetLastImport() As Date Implements IListService.GetLastImport
        Get
            Return XMLData.POptionen.LetzterJournalEintrag
        End Get
    End Property

    Friend Async Function GetAnrufListe() As Task(Of FritzBoxXMLCallList) Implements IListService.GetAnrufListe
        Return Await LadeFritzBoxAnrufliste()
    End Function

    Friend Async Function ErstelleEinträge(Anrufe As IEnumerable(Of FritzBoxXMLCall), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer) Implements IListService.ErstelleEinträge
        Return Await ErstelleJournal(Anrufe, ct, progress)
    End Function
#End Region

#Region "tellows"

    ''' <summary>
    ''' Lädt die tellows ScoreList herunter
    ''' </summary>
    Friend Async Function GetTellowsScoreList() As Task(Of List(Of TellowsScoreListEntry)) Implements IListService.GetTellowsScoreList
        Using tellows As New Tellows
            Return Await tellows.LadeScoreList()
        End Using
    End Function

    Friend Async Function BlockTellowsNumbers(MinScore As Integer, MaxNrbyEntry As Integer, Einträge As IEnumerable(Of TellowsScoreListEntry), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer) Implements IListService.BlockTellowsNumbers
        Return Await FritzBoxRufsperre.BlockTellowsNumbers(MinScore, MaxNrbyEntry, Einträge, ct, progress)
    End Function

#End Region

    Friend Sub BlockNumbers(TelNrListe As IEnumerable(Of String)) Implements IListService.BlockNumbers
        AddNrToBlockList(TelNrListe)
    End Sub

End Class
