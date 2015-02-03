Imports System.Windows.Forms

Friend Class formTBDTV
    Private F_TelefonBuch As formTelefonbuch

    Public Sub New(ByVal mdiParent As formTelefonbuch)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        F_TelefonBuch = mdiParent
    End Sub


    Private Sub DGVTelefonbuch_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        F_TelefonBuch.DGVTelefonbuch_CellValueChanged(sender, e)
    End Sub

End Class