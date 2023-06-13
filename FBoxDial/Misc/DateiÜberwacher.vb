Imports System.IO
Imports System.Web

Friend Class DateiÜberwacher
    Implements IDisposable

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Private ReadOnly Property FSW As FileSystemSafeWatcher

    Public Sub New(Ordner As String, Filter As String)

        FSW = New FileSystemSafeWatcher() With {.Path = Ordner,
                                                .Filter = Filter,
                                                .NotifyFilter = NotifyFilters.LastWrite}

        AddHandler FSW.Changed, AddressOf OnChanged
        FSW.EnableRaisingEvents = True

    End Sub

    Private Sub OnChanged(source As Object, e As FileSystemEventArgs)

        Dim TelNr As New Telefonnummer With {.SetNummer = HttpUtility.UrlDecode(File.ReadLines(e.FullPath).First, Encoding.UTF8)}

        NLogger.Info($"Telefonnummer aus tel:// bzw. callto:// erfasst: {TelNr.Unformatiert}")

        ' Neuen Wählclient generieren
        Dim WählClient As New FritzBoxWählClient(True)
        WählClient.WählboxStart(TelNr)
    End Sub


#Region "Disposable"
    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' Verwalteten Zustand (verwaltete Objekte) bereinigen
            End If

            RemoveHandler FSW.Changed, AddressOf OnChanged
            FSW.EnableRaisingEvents = False
            FSW.Dispose()
            ' Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
            ' Große Felder auf NULL setzen
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class
