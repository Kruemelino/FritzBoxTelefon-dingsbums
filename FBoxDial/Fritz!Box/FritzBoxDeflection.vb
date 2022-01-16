Imports System.Threading.Tasks

Friend Module FritzBoxDeflection
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Rufbehandlung Laden"
    Friend Async Function LadeDeflections(FBoxTR064 As FBoxAPI.FritzBoxTR64) As Task(Of FBoxAPI.DeflectionList)
        Dim XMLListe As String = String.Empty

        ' Ermittle Pfad zur Rufbehandlung
        If FBoxTR064.Ready AndAlso FBoxTR064.X_contact.GetDeflections(XMLListe) Then
            Return Await DeserializeAsyncXML(Of FBoxAPI.DeflectionList)(XMLListe, False)
        Else
            NLogger.Warn("Pfad zur Liste der Deflections konnte nicht ermittelt werden.")
            Return New FBoxAPI.DeflectionList
        End If

    End Function
#End Region
End Module
