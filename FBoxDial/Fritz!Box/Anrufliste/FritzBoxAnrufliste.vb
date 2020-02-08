Imports System.Threading.Tasks

Module FritzBoxAnrufliste
    Private Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
#Region "Anrufliste Laden"
    Friend Async Function LadeFritzBoxAnrufliste() As Task(Of FritzBoxXMLCallList)
        Dim OutPutData As Collections.Hashtable

        Using fboxSOAP As New FritzBoxServices
            ' Lade die Anrufliste herunter
            OutPutData = fboxSOAP.Start(KnownSOAPFile.x_contactSCPD, "GetCallList")

            If OutPutData.ContainsKey("Error") Then
                NLogger.Error(OutPutData.Item("NewCallListURL"))
                Return Nothing
            Else
                If OutPutData.ContainsKey("NewCallListURL") Then
                    ' Deserialisiere die Anrufliste
                    Return Await DeserializeObject(Of FritzBoxXMLCallList)(OutPutData.Item("NewCallListURL").ToString())
                Else
                    NLogger.Warn("XML-Anrufliste konnte nicht heruntergeladen werden.")
                    Return Nothing
                End If
            End If

        End Using
    End Function
#End Region
End Module
