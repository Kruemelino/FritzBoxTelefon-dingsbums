Imports Microsoft.Office.Interop.Outlook

Friend Class InspectorWrapper
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property Inspektor As Inspector
    Private Property OlKontakt As ContactItem

    Public Sub New(i As Inspector)
        ' ThisAddin!: Nur für ContactItem

        Inspektor = i
        NLogger.Debug("Ein neues Inspector-Fenster für einen Kontakt wird geöffnet.")

        AddHandler Inspektor.Close, AddressOf Inspektor_Close

        If Inspektor IsNot Nothing Then 'AndAlso TypeOf Inspektor.CurrentItem Is ContactItem Then
            OlKontakt = CType(Inspektor.CurrentItem, ContactItem)

            ' Füge Ereignishandler hinzu
            AddHandler OlKontakt.Write, AddressOf OlKontakt_Write

        End If

    End Sub

    Private Sub OlKontakt_Write(ByRef Cancel As Boolean)
        NLogger.Debug($"Speichern des Kontaktes '{OlKontakt.FullName}' wurde registriert.")
        IndiziereKontakt(OlKontakt, OlKontakt.ParentFolder, True)
    End Sub

    Private Sub Inspektor_Close()

        If OlKontakt IsNot Nothing Then
            ' Entferne Ereignishandler 
            RemoveHandler OlKontakt.Write, AddressOf OlKontakt_Write

            ReleaseComObject(OlKontakt)
            OlKontakt = Nothing
        End If

        Globals.ThisAddIn.InspectorWrappers.Remove(Inspektor)

        RemoveHandler Inspektor.Close, AddressOf Inspektor_Close
        Inspektor = Nothing
    End Sub

End Class
