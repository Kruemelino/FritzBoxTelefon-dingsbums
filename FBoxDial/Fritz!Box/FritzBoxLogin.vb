Friend Module FritzBoxLogin

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    ''' <summary>
    ''' Gibt eine gültige SessionID zurück
    ''' </summary>
    ''' <returns></returns>    
    Friend Function GetSessionID() As String
        Dim OutPutData As Collections.Hashtable

        Using fboxSOAP As New FritzBoxSOAP
            OutPutData = fboxSOAP.Start(KnownSOAPFile.deviceconfigSCPD, "X_AVM-DE_CreateUrlSID")
            If OutPutData.ContainsKey("NewX_AVM-DE_UrlSID") Then
                NLogger.Debug(OutPutData.Item("NewX_AVM-DE_UrlSID").ToString)
                Return OutPutData.Item("NewX_AVM-DE_UrlSID").ToString
            Else
                Return OutPutData.Item("Error").ToString
            End If
        End Using

    End Function

End Module
