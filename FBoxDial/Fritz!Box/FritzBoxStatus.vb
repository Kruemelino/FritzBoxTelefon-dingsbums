
Friend Module FritzBoxStatus
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend Sub FBoxAPIMessage(sender As Object, e As FBoxAPI.NotifyEventArgs(Of FBoxAPI.LogMessage))
        If e.Value.Ex Is Nothing Then
            NLogger.Log(LogLevel.FromOrdinal(e.Value.Level), e.Value.Message)
        Else
            NLogger.Log(LogLevel.FromOrdinal(e.Value.Level), e.Value.Ex, e.Value.Message)
        End If
    End Sub
End Module
