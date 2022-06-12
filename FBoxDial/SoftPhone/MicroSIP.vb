Friend Module MicroSIP

    Private Const MicroSIPProgressName As String = "MicroSIP"

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend ReadOnly Property MicroSIPReady As Boolean
        Get
            Return Process.GetProcessesByName(MicroSIPProgressName).Length.IsNotZero
        End Get
    End Property

#Region "MicroSIP Commandline"
    ''' <summary>
    ''' Hang up all calls: microsip.exe /hangupall
    ''' </summary>
    Private Const CommandHangUpAll As String = "/hangupall"

    '    ''' <summary>
    '    ''' Answer a Call: microsip.exe /answer
    '    ''' </summary>
    '    Private Const CommandAnswer As String = "/answer"

    '    ''' <summary>
    '    ''' Start minimized: microsip.exe /minimized
    '    ''' </summary>
    '    Private Const CommandMinimized As String = "/minimized"

    '    ''' <summary>
    '    ''' Exit: microsip.exe /exit
    '    ''' </summary>
    '    Private Const CommandExit As String = "/exit"
#End Region

    Friend Function MicroSIPGetExecutablePath() As String
        Dim ProcressMicroSIP As Process() = Process.GetProcessesByName(MicroSIPProgressName)

        If ProcressMicroSIP.Length.IsNotZero Then

            NLogger.Debug(Localize.LocWählclient.strMicroSIPBereit)

            ' Ermittle Pfad zur ausgeführten MicroSIP.exe
            Return ProcressMicroSIP.First.MainModule.FileName
        Else
            Return String.Empty
        End If
    End Function

    Private Sub MicroSIPStart(Connector As IIPPhoneConnector)
        NLogger.Debug(Localize.LocWählclient.strMicroSIPNichtBereit)

        If Connector.ConnectionUriCall.IsNotStringNothingOrEmpty Then
            ' Starte MicroSIP
            Try
                Process.Start(Connector.ConnectionUriCall)

                NLogger.Info(Localize.LocWählclient.strMicroSIPgestartet)
            Catch ex As ComponentModel.Win32Exception
                NLogger.Warn(ex)
            Catch ex As ObjectDisposedException
                NLogger.Warn(ex)
            Catch ex As IO.FileNotFoundException
                NLogger.Warn(ex)
            End Try

        End If
    End Sub

    Friend Function Dial(Connector As IIPPhoneConnector, DialCode As String, Hangup As Boolean) As Boolean
        Dial = False

        If Connector.Type = IPPhoneConnectorType.MicroSIP Then
            If Not MicroSIPReady Then MicroSIPStart(Connector)

            If MicroSIPReady Then
                ' Wählkommando senden
                If Hangup Then
                    ' Abbruch des Rufaufbaues mittels Parameter
                    Process.Start(Connector.ConnectionUriCancel, CommandHangUpAll)

                    NLogger.Debug(Localize.LocWählclient.strSoftPhoneAbbruch)
                Else
                    If Connector.AppendSuffix Then DialCode += "#"

                    ' Aufbau des Telefonates mittels Parameter 
                    Process.Start(Connector.ConnectionUriCall, DialCode)

                    NLogger.Debug(String.Format(Localize.LocWählclient.strSoftPhoneErfolgreich, DialCode, MicroSIPProgressName))
                End If
                ' Gib Rückmeldung, damit Wählclient kein Fehler ausgibt
                Return True
            Else
                ' MicroSIP nicht verfügbar
                NLogger.Warn(Localize.LocWählclient.strMicroSIPNichtBereit)
                ' Gib Rückmeldung, damit Wählclient einen Fehler ausgibt
                Return False
            End If
        End If

    End Function

End Module
