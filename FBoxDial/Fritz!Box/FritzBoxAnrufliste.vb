Imports System.Threading.Tasks

Module FritzBoxAnrufliste
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
#Region "Anrufliste Laden"
    Friend Async Function LadeFritzBoxAnrufliste() As Task(Of FritzBoxXMLCallList)
        Using fboxTR064 As New SOAP.FritzBoxTR64
            Dim Pfad As String = DfltStringEmpty

            ' Ermittle Pfad zur Anrufliste
            If fboxTR064.GetCallList(Pfad) Then
                Return Await DeserializeObjectAsyc(Of FritzBoxXMLCallList)(Pfad)

            Else
                NLogger.Warn("Pfad zur XML-Anrufliste konnte nicht ermittelt werden.")
                Return Nothing
            End If
        End Using
    End Function
#End Region
End Module
