Friend Module FritzBoxLogin

    Private Property NLogger As NLog.Logger = LogManager.GetCurrentClassLogger

    ''' <summary>
    ''' Gibt eine gültige SessionID zurück
    ''' </summary>
    ''' <returns></returns>    
    Friend ReadOnly Property GetSessionID As String
        Get
            Dim OutPutData As Collections.Hashtable

            Using fboxSOAP As New FritzBoxServices
                OutPutData = fboxSOAP.Start(KnownSOAPFile.deviceconfigSCPD, "X_AVM-DE_CreateUrlSID")
                If OutPutData.ContainsKey("NewX_AVM-DE_UrlSID") Then
                    NLogger.Debug(OutPutData.Item("NewX_AVM-DE_UrlSID").ToString)
                    Return OutPutData.Item("NewX_AVM-DE_UrlSID").ToString
                Else
                    Return OutPutData.Item("Error").ToString
                End If
            End Using
        End Get
    End Property

End Module
