Imports System.Threading
Imports System.Windows

Friend Class ZweiFaktorAuthentifizierung
    Implements IDisposable

#Region "Properties"
    Private Property ZweiFAWPF As ZweiFaktorBoxWPF
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Private Property DatenService As IZweiFAService
    Private Property FBoxAPIConnector As FBoxAPIConnector

#End Region

    Public Sub New(APIConnector As FBoxAPIConnector)
        ' Neuer Datenservice
        DatenService = New ZweiFAService(Me)
        FBoxAPIConnector = APIConnector
    End Sub

    Friend Async Sub ZweiFaktorBoxStart(Methods As String)
        Await StartSTATask(Function() As Boolean
                               NLogger.Debug("Blende einen neuen Wählclient als STA Task ein")

                               ' Neuen Wählclient generieren
                               ' Finde das existierende Fenster, oder generiere ein neues
                               ZweiFAWPF = AddWindow(Of ZweiFaktorBoxWPF)(False)

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

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Methods">button,dtmf;*10637</param>
    Private Sub ZweiFaktorBox(Methods As String)

        If Methods.IsNotStringNothingOrEmpty Then

            With ZweiFAWPF
                .DataContext = New ZweiFaktorBoxViewModel(DatenService) With {.Instance = ZweiFAWPF.Dispatcher,
                                                                              .SetMethods = GetMethodText(Methods)}

                .Show()
            End With
        Else
            NLogger.Error("Die Methoden sind nicht gesetzt.")
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Methods">button,dtmf;*10637</param>
    ''' <returns></returns>
    Private Function GetMethodText(Methods As String) As String
        Dim MethodsArray As String() = Split(Methods, ";")

        GetMethodText = Localize.LocZweiFaktorBox.strMethod01

        ' Schleife duch die Methoden
        For Each Method In Split(MethodsArray.First, ",")

            ' Wenn die Authentifizierung mittels Button möglich ist:
            If Method.IsEqual("button") Then GetMethodText += vbCrLf & vbCrLf & Localize.LocZweiFaktorBox.strMethodButton

            ' Wenn die Authentifizierung mittels Tastenkombination möglich ist:
            If Method.IsEqual("dtmf") Then GetMethodText += vbCrLf & vbCrLf & String.Format(Localize.LocZweiFaktorBox.strMethodDTMF, MethodsArray.Last)

        Next

    End Function

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
