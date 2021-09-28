Imports System.Threading.Tasks

Friend Class TAMService
    Implements ITAMService

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
#Region "TAM"
    Friend Async Function GetTAMList() As Task(Of TAMList) Implements ITAMService.GetTAMList
        Dim ABListe As TAMList = Nothing
        ' Lade Anrufbeantworter, TAM (telephone answering machine) via TR-064 
        Using fbtr064 As New SOAP.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)

            If fbtr064.GetTAMList(ABListe) Then
                ' Werte alle TAMs aus.
                Await Task.Run(Sub()
                                   For Each AB In ABListe.TAMListe
                                       AB.GetTAMInformation(fbtr064)
                                   Next
                               End Sub)
            End If
        End Using

        Return ABListe
    End Function

    Friend Sub ToggleTAM(TAM As TAMItem) Implements ITAMService.ToggleTAM
        TAM.ToggleTAMEnableState()
    End Sub

    Friend Function MarkMessage(Message As FritzBoxXMLMessage) As Boolean Implements ITAMService.MarkMessage
        Using fbtr064 As New SOAP.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)
            With Message
                ' Andersrum: If the MarkedAsRead state variable is set to 1, the message is marked as read, when it is 0, the message is marked as unread.
                Return fbtr064.MarkMessage(.Tam, .Index, Not .[New])
            End With
        End Using
    End Function

    Friend Function DeleteMessage(Message As FritzBoxXMLMessage) As Boolean Implements ITAMService.DeleteMessage
        Using fbtr064 As New SOAP.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)
            With Message
                Return fbtr064.DeleteMessage(.Tam, .Index)
            End With
        End Using
    End Function

    Friend Async Sub PlayMessage(Message As FritzBoxXMLMessage) Implements ITAMService.PlayMessage

        NLogger.Debug($"SoundSoundPlayer.Play Anrufbeantworter {Message.CompleteURL}")

        Using SP As New Media.SoundPlayer
            Using wc As New Net.WebClient()
                SP.Stream = Await GetStreamTaskAsync(New Uri(Message.CompleteURL))
            End Using

            SP.Play()
        End Using

    End Sub
#End Region
End Class
