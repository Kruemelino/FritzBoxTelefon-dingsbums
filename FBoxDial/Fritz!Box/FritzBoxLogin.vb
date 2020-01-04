Friend Class FritzBoxLogin
    Implements IDisposable

    ''' <summary>
    ''' Gibt eine gültige SessionID zurüvk
    ''' </summary>
    ''' <returns></returns>    
    Friend ReadOnly Property GetSessionID As String
        Get
            Dim OutPutData As Collections.Hashtable

            Using fboxSOAP As New FritzBoxServices
                OutPutData = fboxSOAP.Start(KnownSOAPFile.deviceconfigSCPD, "X_AVM-DE_CreateUrlSID")
                If OutPutData.ContainsKey("NewX_AVM-DE_UrlSID") Then
                    Return OutPutData.Item("NewX_AVM-DE_UrlSID").ToString
                Else
                    Return OutPutData.Item("Error").ToString
                End If
            End Using
        End Get
    End Property

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
