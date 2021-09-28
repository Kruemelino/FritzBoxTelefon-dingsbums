Public Interface ITAMService

#Region "TAM"
    Function GetTAMList() As Threading.Tasks.Task(Of TAMList)
    Sub ToggleTAM(TAM As TAMItem)
    Function MarkMessage(Message As FritzBoxXMLMessage) As Boolean
    Function DeleteMessage(Message As FritzBoxXMLMessage) As Boolean
    Sub PlayMessage(Message As FritzBoxXMLMessage)

#End Region

End Interface
