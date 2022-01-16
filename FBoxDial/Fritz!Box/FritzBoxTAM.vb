Imports System.Threading.Tasks

Friend Module FritzBoxTAM
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Anrufbeantworter Laden"
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
#End Region
End Module
