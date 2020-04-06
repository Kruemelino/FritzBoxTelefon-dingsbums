Imports System.Windows.Forms

Public Class FormTelefonbücher
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        LadeTelefonbücher()


    End Sub

    Private Async Sub LadeTelefonbücher()
        If ThisAddIn.PPhoneBookXML Is Nothing Then ThisAddIn.PPhoneBookXML = Await LadeFritzBoxTelefonbücher()

        If ThisAddIn.PPhoneBookXML IsNot Nothing AndAlso ThisAddIn.PPhoneBookXML.Telefonbuch IsNot Nothing AndAlso ThisAddIn.PPhoneBookXML.Telefonbuch.Any Then
            For Each TelBuch In ThisAddIn.PPhoneBookXML.Telefonbuch
                LCTelefonbücher.AddTelefonbuch(TelBuch)
            Next
        End If
    End Sub

    Private Sub LCTelefonbücher_ItemClick(sender As Object, Index As Integer) Handles LCTelefonbücher.ItemClick

    End Sub
End Class