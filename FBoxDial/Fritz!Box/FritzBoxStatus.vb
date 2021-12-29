
Friend Module FritzBoxStatus
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend Sub FBoxAPIMessage(sender As Object, e As FBoxAPI.NotifyEventArgs(Of FBoxAPI.LogMessage))
        NLogger.Log(LogLevel.FromOrdinal(e.Value.Level), e.Value.Message)
    End Sub
End Module
