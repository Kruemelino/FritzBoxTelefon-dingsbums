Imports System.Threading.Tasks

Friend Module FritzBoxTAM
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Anrufbeantworter Grundfunktionen"
    Friend Async Function LadeFritzBoxTAM() As Task(Of FBoxAPI.TAMList)
        ' Ermittle Pfad zu den Anrufbeantwortern
        If Globals.ThisAddIn.FBoxTR064?.Ready Then
            Return If(Await Globals.ThisAddIn.FBoxTR064.X_tam.GetList(), New FBoxAPI.TAMList)
        Else
            NLogger.Warn("Pfad zur XML-TAM konnte nicht ermittelt werden.")
            Return New FBoxAPI.TAMList
        End If
    End Function

    Friend Function ToggleTAMItem(TAM As FBoxAPI.TAMItem) As Boolean
        ' Ermittle den aktuellen Status des Anrufbeantworters von der Fritz!Box
        With Globals.ThisAddIn.FBoxTR064.X_tam

            Dim TAMInfo As New FBoxAPI.TAMInfo
            ' Lade die erweiterten TAM Infosätze herunter
            If Globals.ThisAddIn.FBoxTR064.X_tam.GetTAMInfo(TAMInfo, TAM.Index) Then
                Dim NewEnableState As Boolean = Not TAMInfo.Enable

                If .SetEnable(TAM.Index, NewEnableState) Then TAM.Enable = NewEnableState

                NLogger.Info($"Anrufbeantworter {TAM.Name} ({TAM.Index}) {If(NewEnableState, "aktiviert", "deaktiviert")}.")

            End If
        End With
        Return TAM.Enable
    End Function
#End Region

#Region "Nachrichten"
    Friend Async Function GetTAMMessages(TAM As FBoxAPI.TAMItem) As Task(Of IEnumerable(Of FBoxAPI.Message))

        ' Wenn der TAM angezeigt wird, dann ermittle die URL via TR064 zur MessageList
        If TAM.Display Then Return Await GetTAMMessages(TAM.Index)

        ' Gib eine leere Liste Zurück
        Return New List(Of FBoxAPI.Message)
    End Function

    Friend Async Function GetTAMMessages(ID As Integer) As Task(Of IEnumerable(Of FBoxAPI.Message))
        If Globals.ThisAddIn.FBoxTR064.Ready Then
            Return (Await Globals.ThisAddIn.FBoxTR064.X_tam.GetMessageList(ID)).Messages
        End If
        ' Gib eine leere Liste Zurück
        Return New List(Of FBoxAPI.Message)
    End Function

    Friend Function MarkTAMMessage(Message As FBoxAPI.Message) As Boolean
        With Message
            ' Andersrum: If the MarkedAsRead state variable is set to 1, the message is marked as read, when it is 0, the message is marked as unread.
            Dim NewMarkState As Boolean = .[New]
            If Globals.ThisAddIn.FBoxTR064.X_tam.MarkMessage(.Tam, .Index, NewMarkState) Then
                .[New] = Not NewMarkState
                NLogger.Info($"Anrufbeantworter Message {Message.Index} auf {If(Message.[New], "neu", "abgehört")} gesetzt.")
            Else
                NLogger.Warn($"Anrufbeantworter Message {Message.Index} nicht auf {If(Message.[New], "neu", "abgehört")} gesetzt.")
            End If
            Return .[New]
        End With
    End Function

    Friend Function DeleteTAMMessage(Message As FBoxAPI.Message) As Boolean
        With Message
            Return Globals.ThisAddIn.FBoxTR064.X_tam.DeleteMessage(.Tam, .Index)
        End With
    End Function


#End Region


End Module
