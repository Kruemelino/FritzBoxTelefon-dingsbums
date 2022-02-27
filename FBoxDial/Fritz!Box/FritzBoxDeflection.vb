Imports System.Threading.Tasks

Friend Module FritzBoxDeflection

#Region "Rufbehandlung Laden"
    Friend Function LadeDeflections() As Task(Of FBoxAPI.DeflectionList)
        Return Globals.ThisAddIn.FBoxTR064?.X_contact.GetDeflections
    End Function
#End Region
End Module
