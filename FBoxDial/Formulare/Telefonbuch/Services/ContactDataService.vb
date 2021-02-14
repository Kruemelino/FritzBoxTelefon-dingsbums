''' <summary>
''' Das Telefonbuch ist von Tosker erstellt worden und auf Youtube und Github bereitgestellt:
''' https://github.com/Tosker/ContactBook-Tutorial
''' Es wurde duch Kruemelino zu Zwecken der Anzeige von Fritz!Box Kontakten angepasst und erweitert.
''' </summary>
Public Class ContactDataService
    Implements IContactDataService

    Public Async Function GetFBContacts() As Threading.Tasks.Task(Of FritzBoxXMLTelefonbücher) Implements IContactDataService.GetTelefonbücher
        'If ThisAddIn.PhoneBookXML Is Nothing OrElse ThisAddIn.PhoneBookXML.Telefonbücher Is Nothing Then
        ' Telefonbücher asynchron herunterladen
        ThisAddIn.PhoneBookXML = Await Telefonbücher.LadeFritzBoxTelefonbücher()
        'End If

        Return ThisAddIn.PhoneBookXML
    End Function

    Public Async Function AddPhonebook(Name As String) As Threading.Tasks.Task(Of FritzBoxXMLTelefonbuch) Implements IContactDataService.AddTelefonbuch

        Return Await Telefonbücher.ErstelleTelefonbuch(Name)

    End Function

    Public Function DeleteTelefonbuch(TelefonbuchID As Integer) As Boolean Implements IContactDataService.DeleteTelefonbuch
        Return Telefonbücher.LöscheTelefonbuch(TelefonbuchID)
    End Function

    Public Function SetKontakt(TelefonbuchID As Integer, XMLDaten As String) As Integer Implements IContactDataService.SetKontakt
        Return Telefonbücher.SetTelefonbuchEintrag(TelefonbuchID, XMLDaten)
    End Function
    Public Function DeleteKontakt(TelefonbuchID As Integer, UID As Integer) As Boolean Implements IContactDataService.DeleteKontakt
        Return Telefonbücher.DeleteTelefonbuchEintrag(TelefonbuchID, UID)
    End Function

End Class

