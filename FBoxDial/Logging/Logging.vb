Imports System.IO

Friend Module NLogging
    Friend Function DefaultNLogConfig() As Config.LoggingConfiguration

        Dim config = New Config.LoggingConfiguration
        Dim BaseDir As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName)

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
                                                      .KeepFileOpen = True,
                                                      .ConcurrentWrites = True,
                                                      .FileName = Path.Combine(BaseDir, DfltLogFileName),
                                                      .Layout = LayoutText.Join("|"),
                                                      .ArchiveNumbering = Targets.ArchiveNumberingMode.Rolling,
                                                      .ArchiveFileName = Path.Combine(BaseDir, DfltLogArchiveFileName),
                                                      .ArchiveOldFileOnStartupAboveSize = 524288,
                                                      .MaxArchiveFiles = 5})

        ' Standard-Loglevel für das initiale Einlesen der Daten.
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, config.AllTargets.First)

        Return config
    End Function

    Friend Sub SetLogLevel()
        With LogManager.Configuration
            ' Entferne alle vorhandenen Regeln (es sollte nur eine sein)
            .LoggingRules.Clear()
            ' Füge für jedes Target eine Regel hinzu
            For Each Target As Targets.Target In LogManager.Configuration.AllTargets
                .AddRule(LogLevel.FromString(XMLData.POptionen.CBoxMinLogLevel), LogLevel.Fatal, Target)
            Next
            LogManager.ReconfigExistingLoggers()
        End With
    End Sub
End Module
