Imports System.Threading
Imports System.Windows

Friend Class ZweiFaktorAuthentifizierung
    Implements IDisposable

#Region "Properties"
    Private Property ZweiFAWPF As ZweiFaktorBoxWPF
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Private Property DatenService As IZweiFAService
    Private Property FBoxAPIConnector As FBoxAPILog

#End Region

    Public Sub New(APIConnector As FBoxAPILog)
        ' Neuer Datenservice
        DatenService = New ZweiFAService(Me)
        FBoxAPIConnector = APIConnector
    End Sub

    Friend Async Sub ZweiFaktorBoxStart(Methods As String)
        Await StartSTATask(Function() As Boolean
                               NLogger.Debug("Blende einen neuen Wählclient als STA Task ein")

                               ' Neuen Wählclient generieren
                               ' Finde das existierende Fenster, oder generiere ein neues
                               ZweiFAWPF = AddWindow(Of ZweiFaktorBoxWPF)()

                               ' Übergib die Methoden
                               ZweiFaktorBox(Methods)

                               ' Halte den Thread offen so lange das Formular offen ist.
                               While ZweiFAWPF.IsVisible
                                   Forms.Application.DoEvents()
                                   Thread.Sleep(100)
                               End While

                               Return False
                           End Function)
    End Sub

    Private Sub ZweiFaktorBox(Methods As String)

        If Methods.IsNotStringNothingOrEmpty Then

            With ZweiFAWPF
                .DataContext = New ZweiFaktorBoxViewModel(DatenService) With {.Instance = ZweiFAWPF.Dispatcher,
                                                                              .SetMethods = Methods}

                .Show()
            End With
        Else
            NLogger.Error("Die Methods sind nicht vorhanden.")
        End If
    End Sub

    Friend Sub Hide()
        ZweiFAWPF.CloseBox()
        Dispose()
    End Sub

    Friend Sub Cancel()
        FBoxAPIConnector.AbortAuthentication = True
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then

                ' Verwalteten Zustand (verwaltete Objekte) bereinigen
                DatenService = Nothing
            End If
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
