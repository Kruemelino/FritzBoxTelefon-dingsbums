Friend Class MicroSIP
    Implements IDisposable

    Private Const MicroSIPProgressName As String = "MicroSIP"

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend ReadOnly Property MicroSIPReady As Boolean = Process.GetProcessesByName(MicroSIPProgressName).Length.IsNotZero

#Region "MicroSIP Commandline"
    ''' <summary>
    ''' Hang up all calls: microsip.exe /hangupall
    ''' </summary>
    Private Const CommandHangUpAll As String = "/hangupall"
    Private disposedValue As Boolean

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

#Region "Event"
    ''' <summary>
    ''' Event zum setzen des Status
    ''' </summary>
    ''' <param name="Status">Text, welcher Angezeigt werden soll</param>
    Friend Event SetStatus(Status As String)
#End Region

    Public Sub New()

        Dim ProcressMicroSIP As Process()
        ProcressMicroSIP = Process.GetProcessesByName(MicroSIPProgressName)

        If ProcressMicroSIP.Length.IsNotZero Then

            NLogger.Debug(MicroSIPBereit)

            ' Ermittle Pfad zur ausgeführten MicroSIP.exe
            XMLData.POptionen.TBMicroSIPPath = ProcressMicroSIP.First.MainModule.FileName

            NLogger.Debug(MicroSIPgestartet(XMLData.POptionen.TBMicroSIPPath))

        Else
            NLogger.Debug(MicroSIPNichtBereit)

            If XMLData.POptionen.TBMicroSIPPath.IsNotStringNothingOrEmpty Then
                ' Starte MicroSIP
                Process.Start(XMLData.POptionen.TBMicroSIPPath)
                NLogger.Info(MicroSIPgestartet)
            End If
        End If

    End Sub

    Friend Function Dial(DialCode As String, Hangup As Boolean) As Boolean

        Dial = False

        If MicroSIPReady Then
            ' Wählkommando senden
            If Hangup Then
                ' Abbruch des Rufaufbaues mittels Parameter
                Process.Start(XMLData.POptionen.TBMicroSIPPath, CommandHangUpAll)

                NLogger.Debug(SoftPhoneAbbruch)
            Else
                ' Aufbau des Telefonates mittels Parameter 
                Process.Start(XMLData.POptionen.TBMicroSIPPath, DialCode)

                NLogger.Debug(SoftPhoneErfolgreich(DialCode, MicroSIPProgressName))
            End If

        Else
            ' Phoner nicht verfügbar
            RaiseEvent SetStatus(MicroSIPNichtBereit)
            NLogger.Warn(MicroSIPNichtBereit)
        End If
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
            End If

            ' TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
            ' TODO: Große Felder auf NULL setzen
            disposedValue = True
        End If
    End Sub

    ' ' TODO: Finalizer nur überschreiben, wenn "Dispose(disposing As Boolean)" Code für die Freigabe nicht verwalteter Ressourcen enthält
    ' Protected Overrides Sub Finalize()
    '     ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
