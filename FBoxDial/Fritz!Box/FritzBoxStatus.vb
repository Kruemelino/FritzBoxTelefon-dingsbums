<DebuggerStepThrough>
Friend Module FritzBoxStatus
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Sub FBoxAPIMessage(sender As Object, e As FBoxAPI.NotifyEventArgs(Of FBoxAPI.LogMessage))

        With e.Value
            Dim LogEvent As New LogEventInfo(LogLevel.FromOrdinal(.Level),
                                             .CallerClassName,
                                             .Message)

            LogEvent.SetCallerInfo(.CallerClassName, .CallerMemberName, .CallerFilePath, .CallerLineNumber)

            NLogger.Log(LogEvent)
        End With

    End Sub
End Module
