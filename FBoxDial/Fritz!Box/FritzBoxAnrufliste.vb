Imports System.Threading.Tasks

Module FritzBoxAnrufliste
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
#Region "Anrufliste Laden"
    Friend Async Function LadeFritzBoxAnrufliste() As Task(Of FritzBoxXMLCallList)
        Dim OutPutData As Collections.Hashtable

        Using fboxSOAP As New FritzBoxTR64
            ' Lade die Anrufliste herunter
            OutPutData = fboxSOAP.Start(Tr064Files.x_contactSCPD, "GetCallList")

            If OutPutData.ContainsKey("Error") Then
                NLogger.Error("XML-Anrufliste konnte nicht heruntergeladen werden.")
                Return Nothing
            Else
                If OutPutData.ContainsKey("NewCallListURL") Then
                    ' Deserialisiere die Anrufliste
                    Return Await DeserializeObjectAsyc(Of FritzBoxXMLCallList)(OutPutData.Item("NewCallListURL").ToString())
                Else
                    NLogger.Warn("XML-Anrufliste konnte nicht heruntergeladen werden.")
                    Return Nothing
                End If
            End If

        End Using
    End Function
#End Region
End Module
