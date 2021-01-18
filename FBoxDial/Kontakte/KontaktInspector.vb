Imports Microsoft.Office.Interop

Friend Class KontaktInspector
    Implements IDisposable

    Friend WithEvents Kontakt As Outlook.ContactItem

    Private Sub ContactSaved_Close(ByRef Cancel As Boolean) Handles Kontakt.Close
        ThisAddIn.KontakInsepektorenListe.Remove(Me)
        Me.Dispose()
    End Sub

    Private Sub ContactSaved_Write(ByRef Cancel As Boolean) Handles Kontakt.Write
        ' Prüfe ob der Ordner für die Kontaktsuche verwendet wird
        IndiziereKontakt(Kontakt, Kontakt.ParentFolder)

    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                'C_KF = Nothing
            End If
        End If
        Me.disposedValue = True
    End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
