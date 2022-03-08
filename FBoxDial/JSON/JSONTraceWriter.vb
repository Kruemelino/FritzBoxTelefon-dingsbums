Imports Newtonsoft.Json.Serialization

Friend Class JSONTraceWriter
    Implements ITraceWriter

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Public ReadOnly Property LevelFilter As TraceLevel = TraceLevel.Verbose Implements ITraceWriter.LevelFilter

    Public Sub Trace(level As TraceLevel, message As String, ex As Exception) Implements ITraceWriter.Trace
        NLogger.Log(LevelMap(level), ex, message)
    End Sub
End Class
