Imports System.Threading.Tasks
Imports Microsoft.Office.Core

Public Class FBoxDataService
    Implements IFBoxDataService

    Friend Sub UpdateTheme() Implements IFBoxDataService.UpdateTheme
        OfficeColors.UpdateTheme()
    End Sub

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property SoundPlayer As SoundPlayerEx
    Friend Property SessionID As String

    Public Sub New()
        Globals.ThisAddIn.FBoxTR064?.Deviceconfig.GetSessionID(SessionID)
    End Sub

    Protected Overrides Sub Finalize() Implements IFBoxDataService.TR064HttpClient

        ' SoundPlayer
        If SoundPlayer IsNot Nothing Then
            With SoundPlayer
                ' Beende das Abspielen, falls aktiv
                If .PlayingAsync Then .Stop()

                RemoveHandler .SoundFinished, AddressOf SoundPlayer_SoundFinished
                ' Gib Resourcen des Soundplayer frei
                .Dispose()
            End With
        End If

        MyBase.Finalize()
    End Sub

#Region "Anrufliste"
    Private ReadOnly Property GetLastImport() As Date Implements IFBoxDataService.GetLastImport
        Get
            Return XMLData.POptionen.LetzteAuswertungAnrList
        End Get
    End Property

    Private Async Function GetCallList() As Task(Of FBoxAPI.CallList) Implements IFBoxDataService.GetCallList
        Return Await LadeFritzBoxAnrufliste()
    End Function

    Private Async Function ErstelleEinträge(Anrufe As IEnumerable(Of FBoxAPI.Call), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer) Implements IFBoxDataService.ErstelleEinträge
        Return Await SetUpOutlookListen(Anrufe, ct, progress)
    End Function

    Private Sub BlockNumbers(TelNrListe As IEnumerable(Of String)) Implements IFBoxDataService.BlockNumbers
        AddNrToBlockList(TelNrListe)
    End Sub

    Private Async Sub CallXMLContact(Anruf As FBoxAPI.Call) Implements IFBoxDataService.CallXMLContact
        Dim T As Telefonat = Await ErstelleTelefonat(Anruf)
        T.Rückruf()
    End Sub

    Private Async Sub ShowXMLContact(Anruf As FBoxAPI.Call) Implements IFBoxDataService.ShowXMLContact
        Dim T As Telefonat = Await ErstelleTelefonat(Anruf)
        T.ZeigeKontakt()
    End Sub

    Private Sub PlayMessage(CallItem As FBoxAPI.Call) Implements IFBoxDataService.PlayMessage

        Dim Pfad As String = CompleteURL(CallItem)

        NLogger.Debug($"Anrufbeantworternachricht via Callist für Anruf {CallItem.ID}: {Pfad}")

        PlayRecord(Pfad)
    End Sub

    Private Async Sub DownloadFax(CallItem As FBoxAPI.Call) Implements IFBoxDataService.DownloadFax

        Dim URI As New Uri(CompleteURL(CallItem))
        Dim DateiPfad As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Templates), IO.Path.GetRandomFileName.RegExReplace("\.\w*$", ".pdf"))

        NLogger.Debug($"Faxdokument via Callist für Anruf {CallItem.ID}: {URI} - {DateiPfad}")

        If Await Globals.ThisAddIn.FBoxTR064.HttpService.DownloadToFileSystem(URI, DateiPfad) Then Process.Start(New ProcessStartInfo(DateiPfad))
    End Sub

    Private Function CompleteURL(CallItem As FBoxAPI.Call) As String
        Return If(Not SessionID.Contains(FritzBoxDefault.DfltFritzBoxSessionID), $"https://{XMLData.POptionen.ValidFBAdr}:{FritzBoxDefault.DfltTR064PortSSL}{CallItem.Path}&{SessionID}", String.Empty)
    End Function
#End Region

#Region "TAM Anrufbeantworter"
    Private Async Function GetTAMItems() As Task(Of IEnumerable(Of FBoxAPI.TAMItem)) Implements IFBoxDataService.GetTAMItems
        ' Lade Anrufbeantworter, TAM (telephone answering machine) via TR-064 
        Dim ABListe As FBoxAPI.TAMList = Await LadeFritzBoxTAM()
        Return ABListe.Items
    End Function

    Private Async Function GetMessages(TAM As FBoxAPI.TAMItem) As Task(Of IEnumerable(Of FBoxAPI.Message)) Implements IFBoxDataService.GetMessages
        Return Await GetTAMMessages(TAM)
    End Function

    Private Function ToggleTAM(TAM As FBoxAPI.TAMItem) As Boolean Implements IFBoxDataService.ToggleTAM
        Return ToggleTAMItem(TAM)
    End Function

    Private Function MarkMessage(Message As FBoxAPI.Message) As Boolean Implements IFBoxDataService.MarkMessage
        Return MarkTAMMessage(Message)
    End Function

    Private Function DeleteMessage(Message As FBoxAPI.Message) As Boolean Implements IFBoxDataService.DeleteMessage
        Return DeleteTAMMessage(Message)
    End Function

    Private Sub PlayMessage(MessageURL As String) Implements IFBoxDataService.PlayMessage

        NLogger.Debug($"Anrufbeantworternachricht via TAM für Eintrag: {MessageURL}")

        PlayRecord(MessageURL)

    End Sub

    Private Sub StoppMessage(MessageURL As String) Implements IFBoxDataService.StoppMessage
        StoppRecord(MessageURL)
    End Sub

    Public Async Sub DownloadMessage(MessageURL As String, FilePath As String) Implements IFBoxDataService.DownloadMessage
        ' Herunterladen
        Await Globals.ThisAddIn.FBoxTR064.HttpService.DownloadToFileSystem(New Uri(MessageURL), FilePath)
    End Sub

    Private Function CompleteURL(PathSegment As String) As String Implements IFBoxDataService.CompleteURL
        Return FritzBoxDefault.CompleteURL(PathSegment)
    End Function

#End Region

#Region "Deflection - Rufbehandlung"
    Private Async Function GetDeflectionList() As Task(Of FBoxAPI.DeflectionList) Implements IFBoxDataService.GetDeflectionList
        ' Lade Deflections via TR-064 
        Return Await LadeDeflections()
    End Function

    Private Function ToggleRufuml(Deflection As FBoxAPI.Deflection) As Boolean Implements IFBoxDataService.ToggleRufuml
        With Deflection
            Dim NewEnableState As Boolean = Not .Enable

            If Globals.ThisAddIn.FBoxTR064.X_contact.SetDeflectionEnable(Deflection.DeflectionId, NewEnableState) Then

                .Enable = NewEnableState

                NLogger.Info($"Rufbehandlung {Deflection.DeflectionId} {If(Deflection.Enable, "", "de")}aktiviert.")

            Else

                NLogger.Warn($"Rufbehandlung Message {Deflection.DeflectionId} nicht {If(Deflection.Enable, "", "de")}aktiviert.")

            End If
            Return .Enable
        End With
    End Function
#End Region

#Region "tellows"
    ''' <summary>
    ''' Lädt die tellows ScoreList herunter
    ''' </summary>
    Private Async Function GetTellowsScoreList() As Task(Of List(Of TellowsScoreListEntry)) Implements IFBoxDataService.GetTellowsScoreList
        Using tellows As New Tellows
            Return Await tellows.LadeScoreList()
        End Using
    End Function

    Private Async Function BlockTellowsNumbers(MinScore As Integer, MaxNrbyEntry As Integer, Einträge As IEnumerable(Of TellowsScoreListEntry), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer) Implements IFBoxDataService.BlockTellowsNumbers
        Return Await FritzBoxRufsperre.BlockTellowsNumbers(MinScore, MaxNrbyEntry, Einträge, ct, progress)
    End Function

#End Region

#Region "Telefonbücher"

#Region "Fritz!Box Telefonbücher"
    Private Async Function GetFBContacts() As Task(Of IEnumerable(Of PhonebookEx)) Implements IFBoxDataService.GetTelefonbücher
        ' Telefonbücher asynchron herunterladen
        Globals.ThisAddIn.PhoneBookXML = Await Telefonbücher.LadeTelefonbücher()
        Return Globals.ThisAddIn.PhoneBookXML
    End Function

    Private Async Function AddPhonebook(Name As String) As Task(Of PhonebookEx) Implements IFBoxDataService.AddTelefonbuch
        Return Await Telefonbücher.ErstelleTelefonbuch(Name)
    End Function

    Private Function DeletePhonebook(TelefonbuchID As Integer) As Boolean Implements IFBoxDataService.DeleteTelefonbuch
        Return Telefonbücher.LöscheTelefonbuch(TelefonbuchID)
    End Function

    Private Function GetTelefonbuchName(TelefonbuchID As Integer) As String Implements IFBoxDataService.GetTelefonbuchName
        If Globals.ThisAddIn.PhoneBookXML Is Nothing Then
            Return $"ID {TelefonbuchID}"
        Else
            Return $"{Globals.ThisAddIn.PhoneBookXML.Where(Function(PB) PB.ID.AreEqual(TelefonbuchID)).First.Name} ({TelefonbuchID})"
        End If
    End Function

    Private Function GetSesssionID() As String Implements IFBoxDataService.GetSessionID

        Dim SessionID As String = FritzBoxDefault.DfltFritzBoxSessionID

        ' Prüfe, ob Fritz!Box verfügbar
        Globals.ThisAddIn.FBoxTR064.Deviceconfig.GetSessionID(SessionID)

        Return SessionID
    End Function

#End Region

#Region "Kontakte"
    Private Function SetKontakt(TelefonbuchID As Integer, XMLDaten As String) As Integer Implements IFBoxDataService.SetKontakt
        Return Telefonbücher.SetTelefonbuchEintrag(TelefonbuchID, XMLDaten)
    End Function

    Private Function DeleteKontakt(TelefonbuchID As Integer, UID As Integer) As Boolean Implements IFBoxDataService.DeleteKontakt
        Return Telefonbücher.DeleteTelefonbuchEintrag(TelefonbuchID, UID)
    End Function

    Private Function DeleteKontakte(TelefonbuchID As Integer, Einträge As IEnumerable(Of FBoxAPI.Contact)) As Boolean Implements IFBoxDataService.DeleteKontakte
        Return Telefonbücher.DeleteTelefonbuchEinträge(TelefonbuchID, Einträge)
    End Function

    Private Async Function LadeKontaktbild(Person As FBoxAPI.Person) As Task(Of Windows.Media.ImageSource) Implements IFBoxDataService.LadeKontaktbild
        If Person IsNot Nothing AndAlso Person.ImageURL.IsNotStringNothingOrEmpty Then
            Return Await GetPersonImage(Person.CompleteImageURL(SessionID))
        Else
            Return Nothing
        End If
    End Function

    Public Async Function GetKontakt(TelefonbuchID As Integer, UID As Integer) As Task(Of FBoxAPI.Contact) Implements IFBoxDataService.GetKontakt
        Return Await Telefonbücher.GetTelefonbuchEintrag(TelefonbuchID, UID)
    End Function
#End Region

#Region "Rufsperre"
    Private Function SetRufsperre(XMLDaten As FBoxAPI.Contact) As Integer Implements IFBoxDataService.SetRufsperre
        Dim UID As Integer = 0
        Return If(AddToCallBarring(XMLDaten, UID), UID, -1)
    End Function

    Private Function DeleteRufsperre(UID As Integer) As Boolean Implements IFBoxDataService.DeleteRufsperre
        Return DeleteCallBarring(UID)
    End Function

    Private Function DeleteRufsperren(Einträge As IEnumerable(Of FBoxAPI.Contact)) As Boolean Implements IFBoxDataService.DeleteRufsperren
        Return DeleteCallBarrings(Einträge)
    End Function

#End Region

#Region "Kontakt anrufen"
    Private Sub Dial(XMLDaten As FBoxAPI.Contact) Implements IFBoxDataService.Dial
        ' Neuen Wählclient generieren
        ' Finde das existierende Fenster, oder generiere ein neues
        With New FritzBoxWählClient
            .WählboxStart(XMLDaten)
        End With
    End Sub
#End Region
#End Region

#Region "SoundPlayer"
    Private Event SoundFinished As EventHandler(Of NotifyEventArgs(Of String)) Implements IFBoxDataService.SoundFinished

    Private Sub PlayRecord(Pfad As String)

        If Not Pfad.Contains(FritzBoxDefault.DfltFritzBoxSessionID) Then

            If SoundPlayer Is Nothing Then
                SoundPlayer = New SoundPlayerEx()
                AddHandler SoundPlayer.SoundFinished, AddressOf SoundPlayer_SoundFinished

            End If

            With SoundPlayer
                If .PlayingAsync Then .Stop()

                .LocationURL = Pfad
                .PlayAsync()

            End With
        Else
            NLogger.Warn($"TAM Message kann nicht heruntergeladen werden: {Pfad} ")
        End If
    End Sub

    Private Sub StoppRecord(Pfad As String)
        If SoundPlayer IsNot Nothing Then
            With SoundPlayer
                If .PlayingAsync Then .Stop()
            End With
        End If
    End Sub

    Private Sub SoundPlayer_SoundFinished(sender As Object, e As NotifyEventArgs(Of String))

        RaiseEvent SoundFinished(Me, e)

        SoundPlayer.LocationURL = String.Empty
    End Sub

#End Region

End Class
