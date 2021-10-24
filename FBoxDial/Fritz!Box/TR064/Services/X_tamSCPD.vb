Imports System.Collections
Namespace SOAP
    Public Class X_tamSCPD
        Implements IService
        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger Implements IService.NLogger
        Private Property TR064Start As Func(Of String, String, Hashtable, Hashtable) Implements IService.TR064Start
        Private Property PushStatus As Action(Of LogLevel, String) Implements IService.PushStatus

        Public Sub New(Start As Func(Of String, String, Hashtable, Hashtable), Status As Action(Of LogLevel, String))

            TR064Start = Start

            PushStatus = Status
        End Sub

#Region "x_tamSCPD"
        ''' <summary>
        ''' Return a informations of tam index <paramref name="i"/>. 
        ''' </summary>
        ''' <param name="TAMInfo">Structure, which holds all data of the TAM</param>
        ''' <param name="i">Represents the index of all tam.</param>
        ''' <returns>True when success</returns>
        Friend Function GetTAMInfo(ByRef TAMInfo As TAMInfo, i As Integer) As Boolean

            If TAMInfo Is Nothing Then TAMInfo = New TAMInfo

            With TR064Start(Tr064Files.x_tamSCPD, "GetInfo", New Hashtable From {{"NewIndex", i}})

                If .ContainsKey("NewEnable") And .ContainsKey("NewPhoneNumbers") Then

                    TAMInfo.Enable = CBool(.Item("NewEnable"))
                    TAMInfo.Name = .Item("NewName").ToString
                    TAMInfo.TAMRunning = CBool(.Item("NewTAMRunning"))
                    TAMInfo.Stick = CUShort(.Item("NewStick"))
                    TAMInfo.Status = CUShort(.Item("NewStatus"))
                    TAMInfo.Capacity = CULng(.Item("NewCapacity"))
                    TAMInfo.Mode = .Item("NewMode").ToString
                    TAMInfo.RingSeconds = CUShort(.Item("NewRingSeconds"))
                    TAMInfo.PhoneNumbers = .Item("NewPhoneNumbers").ToString.Split(",")

                    PushStatus.Invoke(LogLevel.Debug, $"GetTAMInfoEx ({i}): {TAMInfo.Name}; {TAMInfo.Enable}")

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetTAMInfoEx konnte für nicht aufgelößt werden. '{ .Item("Error")}'")

                    Return False
                End If
            End With

        End Function

        ''' <summary>
        ''' Create an URL to download the list of message for a specified TAM. 
        ''' </summary>
        ''' <remarks>If the HTTP request for the resulting URL fails, it is recommended to make a New SOAP request For GetMessageList or call the SOAP action DeviceConfig:X_AVM-DE_CreateUrlSID for a New session ID.<br/>
        ''' The following URL parameters are supported.
        ''' <list type="bullet">
        ''' <item>max: maximum number of entries in message list, default 999</item>
        ''' <item>sid: Session ID for authentication</item>
        ''' </list>
        ''' </remarks>
        ''' <param name="GetMessageListURL">URL to download the list of message for a specified TAM</param>
        ''' <param name="i">ID of the specified TAM</param>
        ''' <returns>True when success</returns>
        Friend Function GetMessageList(ByRef GetMessageListURL As String, i As Integer) As Boolean
            With TR064Start(Tr064Files.x_tamSCPD, "GetMessageList", New Hashtable From {{"NewIndex", i}})
                If .ContainsKey("NewURL") Then

                    GetMessageListURL = .Item("NewURL").ToString

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetMessageList konnte für nicht aufgelößt werden. '{ .Item("Error")}'")

                    Return False
                End If
            End With

        End Function
        ''' <summary>
        ''' Returns the global information and the specific answering machine information as xml list.
        ''' </summary>
        ''' <param name="TAMListe">Represents the list of all tam.</param>
        ''' <returns>True when success</returns>
        Friend Function GetTAMList(ByRef TAMListe As TAMList) As Boolean

            With TR064Start(Tr064Files.x_tamSCPD, "GetList", Nothing)

                If .ContainsKey("NewTAMList") Then

                    NLogger.Trace(.Item("NewTAMList"))

                    If Not DeserializeXML(.Item("NewTAMList").ToString(), False, TAMListe) Then
                        PushStatus.Invoke(LogLevel.Warn, $"GetList (TAM) konnte für nicht deserialisiert werden.")
                    End If

                    ' Wenn keine TAM angeschlossen wurden, gib eine leere Klasse zurück
                    If TAMListe Is Nothing Then TAMListe = New TAMList

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetList (TAM) konnte für nicht aufgelößt werden. '{ .Item("Error")}'")
                    TAMListe = Nothing

                    Return False
                End If
            End With

        End Function

        ''' <summary>
        ''' If Enable is set to true, the TAM will be visible in WebGUI. 
        ''' </summary>
        ''' <param name="Index">Index of TAM</param>
        ''' <param name="Enable">Enable state</param>
        ''' <returns>True when success</returns>
        Friend Function SetEnable(Index As Integer, Enable As Boolean) As Boolean

            With TR064Start(Tr064Files.x_tamSCPD, "SetEnable", New Hashtable From {{"NewIndex", Index},
                                                                                   {"NewEnable", Enable.ToInt}})
                Return Not .ContainsKey("Error")
            End With

        End Function

        ''' <summary>
        ''' Mark a specified message as read. A specific TAM is selected by Index.
        ''' The Index field from a message in the MessageList should be taken for the MessageIndex
        ''' to select a specific message. If the MarkedAsRead state variable is set to 1, the message
        ''' is marked as read, when it is 0, the message is marked as unread. The default value is 1
        ''' to guarantee downward compatibility to older clients.
        ''' </summary>
        ''' <param name="Index">Index of the MessageList</param>
        ''' <param name="MessageIndex">Index of the Message</param>
        ''' <param name="MarkedAsRead">Optional, to stay compatible with older clients, default value is 1</param>
        ''' <returns>True when success</returns>
        Friend Function MarkMessage(Index As Integer, MessageIndex As Integer, MarkedAsRead As Boolean) As Boolean

            With TR064Start(Tr064Files.x_tamSCPD, "MarkMessage", New Hashtable From {{"NewIndex", Index},
                                                                                     {"NewMessageIndex", MessageIndex},
                                                                                     {"NewMarkedAsRead", MarkedAsRead.ToInt}})
                Return Not .ContainsKey("Error")
            End With

        End Function

        ''' <summary>
        ''' Delete a specified message. A specific TAM is selected by Index.
        ''' The Index field from a message in the MessageList should be taken for the MessageIndex
        ''' to select a specific message. 
        ''' </summary>
        ''' <param name="Index">Index of the MessageList</param>
        ''' <param name="MessageIndex">Index of the Message</param>
        ''' <returns>True when success</returns>
        Friend Function DeleteMessage(Index As Integer, MessageIndex As Integer) As Boolean

            With TR064Start(Tr064Files.x_tamSCPD, "DeleteMessage", New Hashtable From {{"NewIndex", Index},
                                                                                       {"NewMessageIndex", MessageIndex}})


                If Not .ContainsKey("Error") Then

                    PushStatus.Invoke(LogLevel.Info, $"Nachricht auf Anrufbeantworter {Index} mit ID {MessageIndex} gelöscht, '{ .Item("Error")}'")
                    Return True
                Else

                    PushStatus.Invoke(LogLevel.Warn, $"Nachricht auf Anrufbeantworter {Index} mit ID {MessageIndex} nicht gelöscht, '{ .Item("Error")}'")
                    Return False
                End If
            End With

        End Function
#End Region


    End Class
End Namespace