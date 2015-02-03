Friend Class formTBControl
    Private F_TelefonBuch As formTelefonbuch

    Public Sub New(ByVal mdiParent As formTelefonbuch)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        F_TelefonBuch = mdiParent
    End Sub

    Private Sub BAdd_Click(sender As Object, e As EventArgs) Handles BAdd.Click
        F_TelefonBuch.Eintrag_Add_Click()
    End Sub

    Private Sub BDel_Click(sender As Object, e As EventArgs) Handles BDel.Click
        F_TelefonBuch.Eintrag_Delete_Click()
    End Sub
End Class