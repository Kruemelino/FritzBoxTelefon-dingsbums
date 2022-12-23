Imports FBoxAPI

''' <summary>
''' Klasse zum Schreiben der Log-Messages, welche aus der FBoxAPI Schnittstelle kommen, und dem Handling der 2FA.
''' </summary>
Friend Class FBoxAPIConnector
    Implements IFBoxAPIConnector

#Region "Logging"
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property AuthBox As ZweiFaktorAuthentifizierung

    ''' <summary>
    ''' Schreibt die Log-Message in das Log des Addins.
    ''' </summary>
    ''' <param name="MessageContainer">Container, welcher alle übermittelten Log-Informationen aus der FBoxAPI Schnittstelle enthält.</param>
    Public Sub LogMessage(MessageContainer As LogMessage) Implements IFBoxAPIConnector.LogMessage
        With MessageContainer

            Dim LogEvent As New LogEventInfo() With {.Level = NLog.LogLevel.FromOrdinal(MessageContainer.Level),
                                                     .LoggerName = MessageContainer.CallerClassName,
                                                     .Exception = MessageContainer.Ex,
                                                     .Message = MessageContainer.Message}

            LogEvent.SetCallerInfo(.CallerClassName, .CallerMemberName, .CallerFilePath, .CallerLineNumber)

            NLogger.Log(LogEvent)
        End With
    End Sub
#End Region

#Region "Zwei-Faktor Authentication"
    Private _AbortAuthentication As Boolean
    Public Property AbortAuthentication As Boolean Implements IFBoxAPIConnector.AbortAuthentication
        Get
            Return _AbortAuthentication
        End Get
        Set
            _AbortAuthentication = Value

            If AuthBox IsNot Nothing AndAlso Value Then AuthBox.Hide()
        End Set
    End Property

    Private _AuthenticationSuccesful As Boolean
    Public Property AuthenticationSuccesful As Boolean Implements IFBoxAPIConnector.AuthenticationSuccesful
        Get
            Return _AuthenticationSuccesful
        End Get
        Set
            _AuthenticationSuccesful = Value

            If AuthBox IsNot Nothing AndAlso Value Then AuthBox.Hide()
        End Set
    End Property

    Public Sub Signal2FAuthentication(Methods As String) Implements IFBoxAPIConnector.Signal2FAuthentication

        AuthBox = New ZweiFaktorAuthentifizierung(Me)
        AuthBox.ZweiFaktorBoxStart(Methods)

    End Sub
#End Region

End Class
