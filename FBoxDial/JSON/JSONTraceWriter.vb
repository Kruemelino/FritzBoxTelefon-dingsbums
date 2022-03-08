Imports Newtonsoft.Json.Serialization

Friend Class JSONTraceWriter
    Implements ITraceWriter

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Private ReadOnly Property LevelFilter As TraceLevel = TraceLevel.Verbose Implements ITraceWriter.LevelFilter

    Friend Property JSONLoggerOff As Boolean = False

    Private Sub Trace(level As TraceLevel, message As String, ex As Exception) Implements ITraceWriter.Trace
        ' Schreibe das Log, falls dies nicht explizit ausgeschaltet wurde (u. a. bei Tellows)
        If Not JSONLoggerOff Then NLogger.Log(LevelMap(level), ex, message)
    End Sub
End Class
