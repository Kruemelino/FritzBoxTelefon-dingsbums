Imports System.Threading.Tasks

Friend Module FritzBoxTAM
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Anrufbeantworter Grundfunktionen"
    Friend Async Function LadeFritzBoxTAM(FBoxTR064 As FBoxAPI.FritzBoxTR64) As Task(Of FBoxAPI.TAMList)
        Dim XMLListe As String = String.Empty

        ' Ermittle Pfad zu den Anrufbeantwortern
        If FBoxTR064.Ready AndAlso FBoxTR064.X_tam.GetList(XMLListe) Then
            Return Await DeserializeAsyncXML(Of FBoxAPI.TAMList)(XMLListe, False)
        Else
            NLogger.Warn("Pfad zur XML-TAM konnte nicht ermittelt werden.")
            Return New FBoxAPI.TAMList
        End If

    End Function

    Friend Function ToggleTAMItem(FBoxTR064 As FBoxAPI.FritzBoxTR64, TAM As FBoxAPI.TAMItem) As Boolean
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
#End Region

#Region "Nachrichten"
    Friend Function GetTAMMessagges(FBoxTR064 As FBoxAPI.FritzBoxTR64, TAM As FBoxAPI.TAMItem) As IEnumerable(Of FBoxAPI.Message)

        ' Wenn der TAM angezeigt wird, dann ermittle die URL via TR064 zur MessageList
        If TAM.Display Then Return GetTAMMessagges(FBoxTR064, TAM.Index)

        ' Gib eine leere Liste Zurück
        Return New List(Of FBoxAPI.Message)
    End Function

    Friend Function GetTAMMessagges(FBoxTR064 As FBoxAPI.FritzBoxTR64, ID As Integer) As IEnumerable(Of FBoxAPI.Message)

        Dim MessageListURL As String = String.Empty
        If FBoxTR064.X_tam.GetMessageList(MessageListURL, ID) Then
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

    Friend Function MarkTAMMessage(FBoxTR064 As FBoxAPI.FritzBoxTR64, Message As FBoxAPI.Message) As Boolean
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

    Friend Function DeleteTAMMessage(FBoxTR064 As FBoxAPI.FritzBoxTR64, Message As FBoxAPI.Message) As Boolean
        With Message
            Return FBoxTR064.X_tam.DeleteMessage(.Tam, .Index)
        End With
    End Function


#End Region


End Module
