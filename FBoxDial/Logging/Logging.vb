Imports System.IO

Friend Module NLogging
    Friend ReadOnly Property LevelMap As Dictionary(Of TraceLevel, LogLevel) = New Dictionary(Of TraceLevel, LogLevel) From {{TraceLevel.Verbose, LogLevel.Debug},
                                                                                                                             {TraceLevel.Info, LogLevel.Info},
                                                                                                                             {TraceLevel.Warning, LogLevel.Warn},
                                                                                                                             {TraceLevel.Error, LogLevel.Error},
                                                                                                                             {TraceLevel.Off, LogLevel.Fatal}}

    Friend Function DefaultNLogConfig() As Config.LoggingConfiguration

        Dim config = New Config.LoggingConfiguration

        Dim BaseDir As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName)

        Dim LayoutText As String() = {"${date:format=dd.MM.yyyy HH\:mm\:ss.fff}",
                                      "${level}",
                                      "${logger}",
                                      "${callsite:includeNamespace=false:className=false:methodName=true:cleanNamesOfAnonymousDelegates=true:cleanNamesOfAsyncContinuations=true}",
                                      "${threadid}",
                                      "${callsite-linenumber}",
                                      "${message}",
                                      "${onexception:${newline}Exception\: ${exception:format=type,message,method,properties,stackTrace :maxInnerExceptionLevel=50 :innerFormat=shortType,message,method,stackTrace :separator=\r\n}}"}

        ' Level  Typical Use
        ' Fatal  Something bad happened; application is going down
        ' Error  Something failed; application may or may not Continue
        ' Warn   Something unexpected; application will continue
        ' Info   Normal behavior like mail sent, user updated profile etc.
        ' Debug  For debugging; executed query, user authenticated, session expired
        ' Trace  For trace debugging; begin method X, end method X

        config.AddTarget(New Targets.FileTarget With {.Name = "f",
                                                      .Encoding = Encoding.UTF8,
                                                      .KeepFileOpen = True,
                                                      .ConcurrentWrites = True,
                                                      .FileName = Path.Combine(BaseDir, $"{My.Resources.strDefShortName}.log"),
                                                      .Layout = String.Join("|", LayoutText),
                                                      .DeleteOldFileOnStartup = True,
                                                      .ArchiveOldFileOnStartup = True,
                                                      .ArchiveNumbering = Targets.ArchiveNumberingMode.Date,
                                                      .ArchiveDateFormat = "yyMMdd-HHmm",
                                                      .ArchiveFileName = Path.Combine(BaseDir, $"{My.Resources.strDefShortName}.{{#}}.log"),
                                                      .MaxArchiveFiles = 24,
                                                      .MaxArchiveDays = 7})

        ' Standard-Loglevel für das initiale Einlesen der Daten.
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, config.AllTargets.First)

        Return config
    End Function

    Friend Sub SetLogLevel(minLogLevel As String)
        With LogManager.Configuration
            ' Entferne alle vorhandenen Regeln (es sollte nur eine sein)
            .LoggingRules.Clear()
            ' Füge für jedes Target eine Regel hinzu
            For Each Target As Targets.Target In LogManager.Configuration.AllTargets
                .AddRule(LogLevel.FromString(minLogLevel), LogLevel.Fatal, Target)
            Next
            LogManager.ReconfigExistingLoggers()
        End With
    End Sub
End Module
