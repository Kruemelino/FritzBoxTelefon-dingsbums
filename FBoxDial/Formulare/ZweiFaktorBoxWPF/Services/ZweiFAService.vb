Public Class ZweiFAService
    Implements IZweiFAService

    Private Property Client As ZweiFaktorAuthentifizierung

    Friend Sub New(C As ZweiFaktorAuthentifizierung)
        _Client = C
    End Sub

    Private Sub UpdateTheme() Implements IZweiFAService.UpdateTheme
        OfficeColors.UpdateTheme()
    End Sub

    Public Sub CancelAuth() Implements IZweiFAService.CancelAuth
        Client.Cancel()
    End Sub
End Class
