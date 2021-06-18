Imports System.IO

Friend Module NLogging
    Friend Function DefaultNLogConfig() As Config.LoggingConfiguration

        Dim config = New Config.LoggingConfiguration

        Dim LayoutText As String() = {"${date:format=dd.MM.yyyy HH\:mm\:ss.fff}",
                                      "${level}",
                                      "${logger}",
                                      "${callsite:includeNamespace=false:className=false:methodName=true:cleanNamesOfAnonymousDelegates=true:cleanNamesOfAsyncContinuations=true}",
                                      "${callsite-linenumber}",
                                      "${message}",
                                      "${onexception:${newline}Exception\: ${exception:format=type,message,method,properties,stackTrace :maxInnerExceptionLevel=50 :innerFormat=shortType,message,method,stackTrace :separator=\r\n}}"}

        ' Level  Typical Use
        ' Fatal  Something bad happened; application Is going down
        ' Error  Something failed; application may Or may Not Continue
        ' Warn   Something unexpected; application will continue
        ' Info   Normal behavior Like mail sent, user updated profile etc.
        ' Debug  For debugging; executed query, user authenticated, session expired
        ' Trace  For trace debugging; begin method X, end method X

        config.AddTarget(New Targets.FileTarget With {.Name = "f",
                                                      .Encoding = Encoding.UTF8,
                                                      .KeepFileOpen = False,
                                                      .FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, DfltLogFileName),
                                                      .Layout = LayoutText.Join("|")})

        ' Standard-Loglevel für das initiale Einlesen der Daten.
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, config.AllTargets.First)
        Return config
    End Function

    Friend Sub SetLogLevel()
        With LogManager.Configuration
            .LoggingRules.Clear()
            For Each Target As Targets.Target In LogManager.Configuration.AllTargets
                .AddRule(LogLevel.FromString(XMLData.POptionen.CBoxMinLogLevel), LogLevel.Fatal, Target)
            Next
            LogManager.ReconfigExistingLoggers()
        End With
    End Sub
End Module
