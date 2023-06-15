Imports Microsoft.Office.Interop.Outlook

Friend Class InspectorWrapper
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property Inspektor As Inspector
    Private Property OlItem As OutlookItemWrapper

    Public Sub New(i As Inspector)
        ' ThisAddin!: Nur für ContactItem

        _Inspektor = i
        NLogger.Debug("Ein neues Inspector-Fenster für einen Kontakt wird geöffnet.")

        AddHandler Inspektor.Close, AddressOf Inspektor_Close

        If Inspektor IsNot Nothing Then OlItem = New OutlookItemWrapper(Inspektor.CurrentItem)

        ' Entferne das aktuelle Item des Inspectors aus der Liste der selektierten Elemente des Explorers. 
        ' Ansonsten werden die Events für Write doppelt ausgelöst.
        Globals.ThisAddIn.ExplorerWrappers.Values.ToList.ForEach(Sub(E) E.RemoveSelectedItem(OlItem))
    End Sub

    Private Sub Inspektor_Close()
        OlItem.Auflösen()

        Globals.ThisAddIn.InspectorWrappers.Remove(Inspektor)

        RemoveHandler Inspektor.Close, AddressOf Inspektor_Close
        Inspektor = Nothing
    End Sub

End Class
