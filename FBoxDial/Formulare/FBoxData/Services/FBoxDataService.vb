Imports System.Threading.Tasks
Public Class FBoxDataService
    Implements IFBoxDataService
    Friend Sub UpdateTheme() Implements IFBoxDataService.UpdateTheme
        OfficeColors.UpdateTheme()
    End Sub

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property FBoxTR064 As FBoxAPI.FritzBoxTR64
    Private Property SoundPlayer As Media.SoundPlayer
    Friend Property SessionID As String
    Public Sub New()
        FBoxTR064 = New FBoxAPI.FritzBoxTR64()

        AddHandler FBoxTR064.Status, AddressOf FBoxAPIMessage

        FBoxTR064.Init(XMLData.POptionen.ValidFBAdr, XMLData.POptionen.TBNetworkTimeout, FritzBoxDefault.Anmeldeinformationen)

        SessionID = GetSesssionID()

    End Sub

    Protected Overrides Sub Finalize() Implements IFBoxDataService.Finalize

        RemoveHandler FBoxTR064.Status, AddressOf FBoxAPIMessage

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

    Friend Async Function GetCallList() As Task(Of FBoxAPI.CallList) Implements IFBoxDataService.GetCallList
        Return Await LadeFritzBoxAnrufliste(FBoxTR064)
    End Function

    Friend Async Function ErstelleEinträge(Anrufe As IEnumerable(Of FBoxAPI.Call), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer) Implements IFBoxDataService.ErstelleEinträge
        Return Await SetUpOutlookListen(Anrufe, ct, progress)
    End Function

    Friend Sub BlockNumbers(TelNrListe As IEnumerable(Of String)) Implements IFBoxDataService.BlockNumbers
        AddNrToBlockList(FBoxTR064, TelNrListe)
    End Sub

    Friend Async Sub CallXMLContact(Anruf As FBoxAPI.Call) Implements IFBoxDataService.CallXMLContact
        Using t = Await ErstelleTelefonat(Anruf)
            t.Rückruf()
        End Using
    End Sub

    Friend Async Sub ShowXMLContact(Anruf As FBoxAPI.Call) Implements IFBoxDataService.ShowXMLContact
        Using t = Await ErstelleTelefonat(Anruf)
            t.ZeigeKontakt()
        End Using
    End Sub

    Friend Sub PlayMessage(CallItem As FBoxAPI.Call) Implements IFBoxDataService.PlayCallMessage

        Dim Pfad As String = CompleteURL(CallItem)

        NLogger.Debug($"Anrufbeantworternachricht via Callist für Anruf {CallItem.ID}: {Pfad}")

        PlayRecord(Pfad)
    End Sub

    Friend Async Sub DownloadFax(CallItem As FBoxAPI.Call) Implements IFBoxDataService.DownloadFax

        Dim URI As New Uri(CompleteURL(CallItem))
        Dim DateiPfad As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Templates), IO.Path.GetRandomFileName.RegExReplace("\.\w*$", ".pdf"))

        NLogger.Debug($"Faxdokument via Callist für Anruf {CallItem.ID}: {URI} - {DateiPfad}")

        If Await DownloadToFileTaskAsync(URI, DateiPfad) Then Process.Start(New ProcessStartInfo(DateiPfad))
    End Sub

    Private Function CompleteURL(CallItem As FBoxAPI.Call) As String
        Dim SessionID As String = FritzBoxDefault.DfltFritzBoxSessionID
        ' Ermittle die SessionID. Sollte das schief gehen, kommt es zu einer Fehlermeldung im Log.
        FBoxTR064.Deviceconfig.GetSessionID(SessionID)
        Return If(SessionID.IsNotEqual(FritzBoxDefault.DfltFritzBoxSessionID), $"https://{XMLData.POptionen.ValidFBAdr}:{FritzBoxDefault.DfltTR064PortSSL}{CallItem.Path}&{SessionID}", String.Empty)
    End Function
#End Region

#Region "TAM Anrufbeantworter"
    Friend Function GetTAMItems() As IEnumerable(Of FBoxAPI.TAMItem) Implements IFBoxDataService.GetTAMItems
        Dim ABListe As FBoxAPI.TAMList = Nothing

        ' Lade Anrufbeantworter, TAM (telephone answering machine) via TR-064 
        If FBoxTR064.X_tam.GetList(ABListe) Then
            Return ABListe.Items
        Else
            Return New List(Of FBoxAPI.TAMItem)
        End If
    End Function

    Public Function GetMessagges(TAM As FBoxAPI.TAMItem) As IEnumerable(Of FBoxAPI.Message) Implements IFBoxDataService.GetMessagges

        Dim MessageListURL As String = String.Empty
        ' Wenn der TAM angezeigt wird, dann ermittle die URL via TR064 zur MessageList
        If TAM.Display AndAlso FBoxTR064.X_tam.GetMessageList(MessageListURL, TAM.Index) Then
            Dim MessageList As New FBoxAPI.MessageList
            ' Deserialisiere die MessageList
            If DeserializeXML(MessageListURL, True, MessageList) Then
                NLogger.Debug($"{MessageList.Messages.Count} TAM Einträge von {MessageListURL} eingelesen.")
                Return MessageList.Messages
            Else
                NLogger.Warn($"TAM Einträge von {MessageListURL} nicht eingelesen.")
            End If
        End If
        ' Gib eine leere Liste Zurück
        Return New List(Of FBoxAPI.Message)
    End Function

    Friend Function ToggleTAM(TAM As FBoxAPI.TAMItem) As Boolean Implements IFBoxDataService.ToggleTAM
        ' Ermittle den aktuellen Status des Anrufbeantworters von der Fritz!Box
        With FBoxTR064.X_tam

            Dim TAMInfo As New FBoxAPI.TAMInfo
            ' Lade die erweiterten TAM Infosätze herunter
            If FBoxTR064.X_tam.GetTAMInfo(TAMInfo, TAM.Index) Then
                Dim NewEnableState As Boolean = Not TAMInfo.Enable

                If .SetEnable(TAM.Index, NewEnableState) Then TAM.Enable = NewEnableState

                NLogger.Info($"Anrufbeantworter {TAM.Name} ({TAM.Index}) {If(NewEnableState, "aktiviert", "deaktiviert")}.")

            End If
        End With
        Return TAM.Enable
    End Function

    Friend Function MarkMessage(Message As FBoxAPI.Message) As Boolean Implements IFBoxDataService.MarkMessage
        With Message
            ' Andersrum: If the MarkedAsRead state variable is set to 1, the message is marked as read, when it is 0, the message is marked as unread.
            Dim NewMarkState As Boolean = .[New]
            If FBoxTR064.X_tam.MarkMessage(.Tam, .Index, NewMarkState) Then
                .[New] = Not NewMarkState

                NLogger.Info($"Anrufbeantworter Message {Message.Index} auf {If(Message.[New], "neu", "abgehört")} gesetzt.")

            Else

                NLogger.Warn($"Anrufbeantworter Message {Message.Index} nicht auf {If(Message.[New], "neu", "abgehört")} gesetzt.")

            End If
            Return .[New]
        End With
    End Function

    Friend Function DeleteMessage(Message As FBoxAPI.Message) As Boolean Implements IFBoxDataService.DeleteMessage
        With Message
            Return FBoxTR064.X_tam.DeleteMessage(.Tam, .Index)
        End With
    End Function

    Friend Sub PlayMessage(Message As FBoxAPI.Message) Implements IFBoxDataService.PlayMessage

        Dim Pfad As String = CompleteURL(Message)

        NLogger.Debug($"Anrufbeantworternachricht via TAM für Eintrag {Message.ID}: {Pfad}")

        PlayRecord(Pfad)

    End Sub

    Private Function CompleteURL(Message As FBoxAPI.Message) As String
        Dim SessionID As String = FritzBoxDefault.DfltFritzBoxSessionID
        ' Ermittle die SessionID. Sollte das schief gehen, kommt es zu einer Fehlermeldung im Log.
        FBoxTR064.Deviceconfig.GetSessionID(SessionID)
        Return If(SessionID.IsNotEqual(FritzBoxDefault.DfltFritzBoxSessionID), $"https://{XMLData.POptionen.ValidFBAdr}:{FritzBoxDefault.DfltTR064PortSSL}{Message.Path}&{SessionID}", String.Empty)
    End Function

#End Region

#Region "Deflection - Rufbehandlung"
    Friend Function GetDeflectionList() As FBoxAPI.DeflectionList Implements IFBoxDataService.GetDeflectionList
        'Dim DeflectionListVM As IEnumerable(Of FBoxDeflectionItemViewModel) = Nothing
        Dim DeflectionList As New FBoxAPI.DeflectionList

        If FBoxTR064.Bereit Then FBoxTR064.X_contact.GetDeflections(DeflectionList)

        Return DeflectionList
    End Function

    Friend Function ToggleRufuml(Deflection As FBoxAPI.Deflection) As Boolean Implements IFBoxDataService.ToggleRufuml
        With Deflection
            Dim NewEnableState As Boolean = Not .Enable

            If FBoxTR064.X_contact.SetDeflectionEnable(Deflection.DeflectionId, NewEnableState) Then

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
    Public Async Function GetFBContacts() As Task(Of IEnumerable(Of PhonebookEx)) Implements IFBoxDataService.GetTelefonbücher
        ' Telefonbücher asynchron herunterladen
        Globals.ThisAddIn.PhoneBookXML = Await Telefonbücher.LadeTelefonbücher(FBoxTR064)
        Return Globals.ThisAddIn.PhoneBookXML
    End Function

    Public Async Function AddPhonebook(Name As String) As Task(Of PhonebookEx) Implements IFBoxDataService.AddTelefonbuch
        Return Await Telefonbücher.ErstelleTelefonbuch(FBoxTR064, Name)
    End Function

    Public Function DeletePhonebook(TelefonbuchID As Integer) As Boolean Implements IFBoxDataService.DeleteTelefonbuch
        Return Telefonbücher.LöscheTelefonbuch(FBoxTR064, TelefonbuchID)
    End Function

    Public Function GetSesssionID() As String Implements IFBoxDataService.GetSessionID

        Dim SessionID As String = FritzBoxDefault.DfltFritzBoxSessionID

        ' Prüfe, ob Fritz!Box verfügbar
        'If Ping(XMLData.POptionen.ValidFBAdr) Then
        FBoxTR064.Deviceconfig.GetSessionID(SessionID)
        'Else
        '    NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
        'End If
        Return SessionID
    End Function

#End Region

#Region "Kontakte"
    Public Function SetKontakt(TelefonbuchID As Integer, XMLDaten As String) As Integer Implements IFBoxDataService.SetKontakt
        Return Telefonbücher.SetTelefonbuchEintrag(FBoxTR064, TelefonbuchID, XMLDaten)
    End Function

    Public Function DeleteKontakt(TelefonbuchID As Integer, UID As Integer) As Boolean Implements IFBoxDataService.DeleteKontakt
        Return Telefonbücher.DeleteTelefonbuchEintrag(FBoxTR064, TelefonbuchID, UID)
    End Function

    Public Function DeleteKontakte(TelefonbuchID As Integer, Einträge As IEnumerable(Of FBoxAPI.Contact)) As Boolean Implements IFBoxDataService.DeleteKontakte
        Return Telefonbücher.DeleteTelefonbuchEinträge(FBoxTR064, TelefonbuchID, Einträge)
    End Function

    Public Async Function LadeKontaktbild(Person As FBoxAPI.Person) As Task(Of Windows.Media.ImageSource) Implements IFBoxDataService.LadeKontaktbild
        If Person IsNot Nothing AndAlso Person.ImageURL.IsNotStringNothingOrEmpty Then
            Return Await GetPersonImage(Person.CompleteImageURL(SessionID))
        Else
            Return Nothing
        End If
    End Function
#End Region

#Region "Rufsperre"
    Public Function SetRufsperre(XMLDaten As FBoxAPI.Contact) As Integer Implements IFBoxDataService.SetRufsperre
        Dim UID As Integer = 0
        Return If(AddToCallBarring(FBoxTR064, XMLDaten, UID), UID, -1)
    End Function

    Public Function DeleteRufsperre(UID As Integer) As Boolean Implements IFBoxDataService.DeleteRufsperre
        Return DeleteCallBarring(FBoxTR064, UID)
    End Function

    Public Function DeleteRufsperren(Einträge As IEnumerable(Of FBoxAPI.Contact)) As Boolean Implements IFBoxDataService.DeleteRufsperren
        Return DeleteCallBarrings(FBoxTR064, Einträge)
    End Function

#End Region

#Region "Kontakt anrufen"
    Public Sub Dial(XMLDaten As FBoxAPI.Contact) Implements IFBoxDataService.Dial
        Dim WählClient As New FritzBoxWählClient
        WählClient.WählboxStart(XMLDaten)
    End Sub
#End Region
#End Region

#Region "SoundPlayer"
    Private Async Sub PlayRecord(Pfad As String)
        ' TODO: Fehlermeldung als Messagebox rausgeben

        If Not Pfad.Contains(FritzBoxDefault.DfltFritzBoxSessionID) Then
            If SoundPlayer Is Nothing Then SoundPlayer = New Media.SoundPlayer
            With SoundPlayer
                ' halte die aktuelle Wiedergabe an
                .Stop()
                ' Lade die neue Wiedergabedatei
                Using wc As New Net.WebClient()
                    Try
                        .Stream = Await GetStreamTaskAsync(New Uri(Pfad))
                        .Play()
                    Catch ex As Net.WebException
                        ' Der durch Kombinieren von BaseAddress und address gebildete URI ist ungültig.
                        ' - oder -
                        ' Fehler beim Herunterladen der Ressource.
                        NLogger.Error(ex, $"Link: {Pfad} ")

                    Catch ex As ArgumentNullException
                        ' Der address-Parameter ist null.
                        NLogger.Error(ex, "Der address-Parameter ist null.")

                    End Try

                End Using
            End With
        Else

        End If
    End Sub
#End Region

End Class
