Imports FBoxAPI

''' <summary>
''' Klasse zun Schreiben der Log-Messages, welche aus der FBoxAPI Schnittstelle kommen.
''' </summary>
Friend Class FBoxAPILog
    Implements ILogWriter

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    ''' <summary>
    ''' Schreibt die Log-Message in das Log des Addins.
    ''' </summary>
    ''' <param name="MessageContainer">Container, welcher alle übermittelten Log-Informationen aus der FBoxAPI Schnittstelle enthält-</param>
    Public Sub LogMessage(MessageContainer As LogMessage) Implements ILogWriter.LogMessage
        With MessageContainer
            Dim LogEvent As New LogEventInfo(NLog.LogLevel.FromOrdinal(.Level),
                                             .CallerClassName,
                                             .Message)
            LogEvent.Exception = .Ex

            LogEvent.SetCallerInfo(.CallerClassName, .CallerMemberName, .CallerFilePath, .CallerLineNumber)

            NLogger.Log(LogEvent)
        End With
    End Sub
End Class
