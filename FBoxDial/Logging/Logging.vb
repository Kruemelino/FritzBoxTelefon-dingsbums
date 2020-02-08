Friend Module NLogging

    Friend Function DefaultNLogConfig() As NLog.Config.LoggingConfiguration
        Dim config = New NLog.Config.LoggingConfiguration

        Dim DfltLogLayout As New NLog.Layouts.SimpleLayout With {.Text = PDfltNLog_LayoutText}


        Dim Ziel As New NLog.Targets.FileTarget With {.Name = "f",
                                                      .Encoding = Encoding.UTF8,
                                                      .KeepFileOpen = False,
                                                      .FileName = IO.Path.Combine(XMLData.POptionen.PArbeitsverzeichnis, PDfltLog_FileName),
                                                      .Layout = DfltLogLayout}

        ' Level  Typical Use
        ' Fatal  Something bad happened; application Is going down
        ' Error  Something failed; application may Or may Not Continue
        ' Warn   Something unexpected; application will continue
        ' Info   Normal behavior Like mail sent, user updated profile etc.
        ' Debug  For debugging; executed query, user authenticated, session expired
        ' Trace  For trace debugging; begin method X, end method X

        Dim minLogLevel As NLog.LogLevel = NLog.LogLevel.FromString(XMLData.POptionen.PCBoxMinLogLevel)
        Dim maxLogLevel As NLog.LogLevel = NLog.LogLevel.Fatal

        config.AddRule(minLogLevel, maxLogLevel, Ziel)

        Return config
    End Function
End Module
