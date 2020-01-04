Imports System.Threading.Tasks
Imports FBoxDial.FritzBoxDefault
Friend Class FritzBoxQuery
    Implements IDisposable

    Public Shared ReadOnly Property PFBLinkQuery(ByVal sSID As String, ByVal sAbfrage As String) As String
        Get
            Return PFBLinkBasis & "/query.lua?" & sSID & "&" & sAbfrage
        End Get
    End Property

    Friend Overloads Async Function FritzBoxQuery(ByVal SessionID As String, ByVal Abfrage As List(Of String)) As Task(Of String)
        Return Await FritzBoxQuery(SessionID, String.Join("&", Abfrage.ToArray))
        'Return Await HTTPGet(PFBLinkQuery(SessionID, String.Join("&", Abfrage.ToArray)), XMLData.POptionen.PEncodingFritzBox)
    End Function
    Friend Overloads Async Function FritzBoxQuery(ByVal SessionID As String, ByVal Abfrage As String) As Task(Of String)
        Return Await HTTPGet(PFBLinkQuery(SessionID, Abfrage), XMLData.POptionen.PEncodingFritzBox)
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' TODO: große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
