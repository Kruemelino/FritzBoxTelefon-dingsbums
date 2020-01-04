Imports System.Threading.Tasks

Module FritzBoxAnrufliste

#Region "Anrufliste Laden"
    Friend Async Function LadeFritzBoxAnrufliste() As Task(Of FritzBoxXMLCallList)
        Dim OutPutData As Collections.Hashtable

        Using fboxSOAP As New FritzBoxServices
            ' Lade die Anrufliste herunter
            OutPutData = fboxSOAP.Start(KnownSOAPFile.x_contactSCPD, "GetCallList")

            If OutPutData.ContainsKey("NewCallListURL") Then
                ' Deserialisiere die Anrufliste
                Return Await DeserializeObject(Of FritzBoxXMLCallList)(OutPutData.Item("NewCallListURL").ToString())
            Else
                Return Nothing
            End If

        End Using
    End Function


    'Friend Async Function LadeFritzBoxAnrufliste(ByVal ID As Integer) As Task(Of FritzBoxXMLCallList)
    '    Dim OutPutData As Collections.Hashtable
    '    Dim exurl As String

    '    Using fboxSOAP As New FritzBoxServices
    '        ' Lade die Anrufliste herunter
    '        OutPutData = fboxSOAP.Start(KnownSOAPFile.x_contactSCPD, "GetCallList")

    '        If OutPutData.ContainsKey("NewCallListURL") Then
    '            ' Deserialisiere die Anrufliste

    '            exurl = String.Format("{0}&id={1}", OutPutData.Item("NewCallListURL").ToString(), ID)
    '            Return Await DeserializeObject(Of FritzBoxXMLCallList)(exurl)
    '        Else
    '            Return Nothing
    '        End If

    '    End Using
    'End Function
#End Region
End Module
