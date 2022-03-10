Imports FBoxAPI
Friend Class FBoxAPILog
    Implements ILogWriter

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Public Sub LogMessage(MessageContainer As LogMessage) Implements ILogWriter.LogMessage
        With MessageContainer
            Dim LogEvent As New LogEventInfo(NLog.LogLevel.FromOrdinal(.Level),
                                             .CallerClassName,
                                             .Message)

            LogEvent.SetCallerInfo(.CallerClassName, .CallerMemberName, .CallerFilePath, .CallerLineNumber)

            NLogger.Log(LogEvent)
        End With
    End Sub
End Class
