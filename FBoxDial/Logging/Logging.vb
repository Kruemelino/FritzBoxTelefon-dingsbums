Friend Module NLogging

    Friend Function DefaultNLogConfig() As NLog.Config.LoggingConfiguration
        Dim config = New NLog.Config.LoggingConfiguration

        Dim DfltLogLayout As New NLog.Layouts.SimpleLayout With {.Text = PDfltNLog_LayoutText}


        Dim Ziel As New NLog.Targets.FileTarget With {.Name = "f",
                                                      .Encoding = Encoding.UTF8,
                                                      .KeepFileOpen = False,
                                                      .FileName = IO.Path.Combine(XMLData.POptionen.PArbeitsverzeichnis, PDfltLog_FileName),
                                                      .Layout = DfltLogLayout}

        Dim minLogLevel As NLog.LogLevel = NLog.LogLevel.Info
        Dim maxLogLevel As NLog.LogLevel = NLog.LogLevel.Fatal

        config.AddRule(minLogLevel, maxLogLevel, Ziel)

        Return config
    End Function
End Module
