Imports Microsoft.Office.Interop

Friend Class KontaktInspector
    Implements IDisposable

    Friend WithEvents OlKontakt As Outlook.ContactItem
    Private disposedValue As Boolean

    Private Sub ContactSaved_Close(ByRef Cancel As Boolean) Handles OlKontakt.Close
        ThisAddIn.KontakInsepektorenListe.Remove(Me)
        Dispose()
    End Sub

    Private Sub ContactSaved_Write(ByRef Cancel As Boolean) Handles OlKontakt.Write
        ' Prüfe ob der Ordner für die Kontaktsuche verwendet wird
        IndiziereKontakt(OlKontakt, OlKontakt.ParentFolder, True)
    End Sub

#Region "IDisposable Support"
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' Verwalteten Zustand (verwaltete Objekte) bereinigen
            End If

            ReleaseComObject(OlKontakt)
            OlKontakt = Nothing
            ' Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
            ' Große Felder auf NULL setzen
            disposedValue = True
        End If
    End Sub

    ' Finalizer nur überschreiben, wenn "Dispose(disposing As Boolean)" Code für die Freigabe nicht verwalteter Ressourcen enthält
    Protected Overrides Sub Finalize()
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=False)
        MyBase.Finalize()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class
