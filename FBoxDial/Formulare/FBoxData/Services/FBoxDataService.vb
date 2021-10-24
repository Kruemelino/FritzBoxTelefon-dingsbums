Imports System.Threading.Tasks
Public Class FBoxDataService
    Implements IFBoxDataService

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property FBoxTR064 As TR064.FritzBoxTR64
    Private Property SoundPlayer As Media.SoundPlayer

    Public Sub New()
        FBoxTR064 = New TR064.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)
    End Sub
    Protected Overrides Sub Finalize() Implements IFBoxDataService.Finalize
        FBoxTR064.Dispose()
        SoundPlayer?.Dispose()

        MyBase.Finalize()
    End Sub

#Region "Anrufliste"
    Friend ReadOnly Property GetLastImport() As Date Implements IFBoxDataService.GetLastImport
        Get
            Return XMLData.POptionen.LetzteAuswertungAnrList
        End Get
    End Property

    Friend Async Function GetAnrufListe() As Task(Of TR064.FritzBoxXMLCallList) Implements IFBoxDataService.GetAnrufListe
        Return Await LadeFritzBoxAnrufliste(FBoxTR064)
    End Function

    Friend Async Function ErstelleEinträge(Anrufe As IEnumerable(Of TR064.FritzBoxXMLCall), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer) Implements IFBoxDataService.ErstelleEinträge
        Return Await ErstelleJournal(Anrufe, ct, progress)
    End Function

    Friend Sub BlockNumbers(TelNrListe As IEnumerable(Of String)) Implements IFBoxDataService.BlockNumbers
        AddNrToBlockList(FBoxTR064, TelNrListe)
    End Sub

    Friend Async Sub CallXMLContact(Kontakt As TR064.FritzBoxXMLCall) Implements IFBoxDataService.CallXMLContact
        Using t = Await Kontakt.ErstelleTelefonat
            t.Rückruf()
        End Using
    End Sub

    Friend Async Sub ShowXMLContact(Kontakt As TR064.FritzBoxXMLCall) Implements IFBoxDataService.ShowXMLContact
        Using t = Await Kontakt.ErstelleTelefonat
            t.ZeigeKontakt()
        End Using
    End Sub
#End Region

#Region "TAM Anrufbeantworter"
    Friend Async Function GetTAMList() As Task(Of TR064.TAMList) Implements IFBoxDataService.GetTAMList
        Dim ABListe As TR064.TAMList = Nothing
        ' Lade Anrufbeantworter, TAM (telephone answering machine) via TR-064 

        If FBoxTR064.X_tam.GetTAMList(ABListe) Then
            ' Werte alle TAMs aus.
            Await Task.Run(Sub()
                               For Each AB In ABListe.TAMListe
                                   AB.GetTAMInformation(FBoxTR064)
                               Next
                           End Sub)
        End If

        Return ABListe
    End Function

    Friend Sub ToggleTAM(TAM As TR064.TAMItem) Implements IFBoxDataService.ToggleTAM
        TAM.ToggleTAMEnableState()
    End Sub

    Friend Function MarkMessage(Message As TR064.FritzBoxXMLMessage) As Boolean Implements IFBoxDataService.MarkMessage
        With Message
            ' Andersrum: If the MarkedAsRead state variable is set to 1, the message is marked as read, when it is 0, the message is marked as unread.
            Return FBoxTR064.X_tam.MarkMessage(.Tam, .Index, Not .[New])
        End With
    End Function

    Friend Function DeleteMessage(Message As TR064.FritzBoxXMLMessage) As Boolean Implements IFBoxDataService.DeleteMessage
        With Message
            Return FBoxTR064.X_tam.DeleteMessage(.Tam, .Index)
        End With
    End Function

    Friend Async Sub PlayMessage(Message As TR064.FritzBoxXMLMessage) Implements IFBoxDataService.PlayMessage

        NLogger.Debug($"SoundSoundPlayer.Play Anrufbeantworter {Message.CompleteURL}")

        If SoundPlayer Is Nothing Then SoundPlayer = New Media.SoundPlayer
        With SoundPlayer
            ' halte die aktuelle Wiedergabe an
            .Stop()
            ' Lade die neue Wiedergabedatei
            Using wc As New Net.WebClient()
                .Stream = Await GetStreamTaskAsync(New Uri(Message.CompleteURL))
            End Using

            .Play()
        End With

    End Sub
#End Region

#Region "tellows"

    ''' <summary>
    ''' Lädt die tellows ScoreList herunter
    ''' </summary>
    Friend Async Function GetTellowsScoreList() As Task(Of List(Of TellowsScoreListEntry)) Implements IFBoxDataService.GetTellowsScoreList
        Using tellows As New Tellows
            Return Await tellows.LadeScoreList()
        End Using
    End Function

    Friend Async Function BlockTellowsNumbers(MinScore As Integer, MaxNrbyEntry As Integer, Einträge As IEnumerable(Of TellowsScoreListEntry), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer) Implements IFBoxDataService.BlockTellowsNumbers
        Return Await FritzBoxRufsperre.BlockTellowsNumbers(FBoxTR064, MinScore, MaxNrbyEntry, Einträge, ct, progress)
    End Function

#End Region

#Region "Telefonbücher"

#Region "Fritz!Box Telefonbücher"
    Public Async Function GetFBContacts() As Task(Of TR064.FritzBoxXMLTelefonbücher) Implements IFBoxDataService.GetTelefonbücher
        ' Telefonbücher asynchron herunterladen
        ThisAddIn.PhoneBookXML = Await Telefonbücher.LadeFritzBoxTelefonbücher(FBoxTR064)
        Return ThisAddIn.PhoneBookXML
    End Function

    Public Async Function AddPhonebook(Name As String) As Task(Of TR064.FritzBoxXMLTelefonbuch) Implements IFBoxDataService.AddTelefonbuch
        Return Await Telefonbücher.ErstelleTelefonbuch(FBoxTR064, Name)
    End Function

    Public Function DeletePhonebook(TelefonbuchID As Integer) As Boolean Implements IFBoxDataService.DeleteTelefonbuch
        Return Telefonbücher.LöscheTelefonbuch(FBoxTR064, TelefonbuchID)
    End Function

    Public Function GetSesssionID() As String Implements IFBoxDataService.GetSessionID
        Return Telefonbücher.GetSessionID(FBoxTR064)
    End Function

#End Region

#Region "Fritz!Box Kontakte"
    Public Function SetKontakt(TelefonbuchID As Integer, XMLDaten As String) As Integer Implements IFBoxDataService.SetKontakt
        Return Telefonbücher.SetTelefonbuchEintrag(FBoxTR064, TelefonbuchID, XMLDaten)
    End Function

    Public Function DeleteKontakt(TelefonbuchID As Integer, UID As Integer) As Boolean Implements IFBoxDataService.DeleteKontakt
        Return Telefonbücher.DeleteTelefonbuchEintrag(FBoxTR064, TelefonbuchID, UID)
    End Function

    Public Function DeleteKontakte(TelefonbuchID As Integer, Einträge As IEnumerable(Of TR064.FritzBoxXMLKontakt)) As Boolean Implements IFBoxDataService.DeleteKontakte
        Return Telefonbücher.DeleteTelefonbuchEinträge(FBoxTR064, TelefonbuchID, Einträge)
    End Function

#End Region

#Region "Fritz!Box Rufsperre"
    Public Function SetRufsperre(XMLDaten As TR064.FritzBoxXMLKontakt) As Integer Implements IFBoxDataService.SetRufsperre
        Dim UID As Integer = 0
        Return If(AddToCallBarring(FBoxTR064, XMLDaten, UID), UID, -1)
    End Function

    Public Function DeleteRufsperre(UID As Integer) As Boolean Implements IFBoxDataService.DeleteRufsperre
        Return DeleteCallBarring(FBoxTR064, UID)
    End Function

    Public Function DeleteRufsperren(Einträge As IEnumerable(Of TR064.FritzBoxXMLKontakt)) As Boolean Implements IFBoxDataService.DeleteRufsperren
        Return DeleteCallBarrings(FBoxTR064, Einträge)
    End Function

#End Region

#Region "Kontakt anrufen"
    Public Sub Dial(XMLDaten As TR064.FritzBoxXMLKontakt) Implements IFBoxDataService.Dial
        Dim WählClient As New FritzBoxWählClient
        WählClient.WählboxStart(XMLDaten)
    End Sub
#End Region
#End Region

#Region "Deflection - Rufumleitung"
    Friend Function GertDeflectionList() As Task(Of TR064.DeflectionList) Implements IFBoxDataService.GestDeflectionList
        Dim Deflections As String = DfltStringEmpty
        Dim DeflectionList As New TR064.DeflectionList

        If FBoxTR064.X_contact.GetDeflections(Deflections) Then
            Return DeserializeAsyncXML(Of TR064.DeflectionList)(Deflections, False)
        Else
            Return Nothing
        End If
    End Function

    Public Sub ToggleRufuml(Deflection As TR064.DeflectionInfo) Implements IFBoxDataService.ToggleRufuml
        FBoxTR064.X_contact.SetDeflectionEnable(Deflection.DeflectionId, Not Deflection.Enable)
    End Sub
#End Region
End Class
