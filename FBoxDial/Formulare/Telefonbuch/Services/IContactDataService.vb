''' <summary>
''' Das Telefonbuch ist von Tosker erstellt worden und auf Youtube und Github bereitgestellt:
''' https://github.com/Tosker/ContactBook-Tutorial
''' Es wurde duch Kruemelino zu Zwecken der Anzeige von Fritz!Box Kontakten angepasst und erweitert.
''' </summary>
Public Interface IContactDataService

#Region "Fritz!Box Telefonbücher"
    Function GetTelefonbücher() As Threading.Tasks.Task(Of FritzBoxXMLTelefonbücher)
    Function AddTelefonbuch(Name As String) As Threading.Tasks.Task(Of FritzBoxXMLTelefonbuch)
    Function DeleteTelefonbuch(TelefonbuchID As Integer) As Boolean
    Function GetSessionID() As String
#End Region

#Region "Fritz!Box Kontakte"
    Function SetKontakt(TelefonbuchID As Integer, XMLDaten As String) As Integer
    Function DeleteKontakt(TelefonbuchID As Integer, UID As Integer) As Boolean
    Function DeleteKontakte(TelefonbuchID As Integer, Einträge As IEnumerable(Of FritzBoxXMLKontakt)) As Boolean
#End Region

#Region "Fritz!Box Rufsperren"
    Function SetRufsperre(XMLDaten As FritzBoxXMLKontakt) As Integer
    Function DeleteRufsperre(UID As Integer) As Boolean
    Function DeleteRufsperren(Einträge As IEnumerable(Of FritzBoxXMLKontakt)) As Boolean
#End Region

#Region "Kontakt anrufen"
    Sub Dial(XMLDaten As FritzBoxXMLKontakt)
#End Region
End Interface
