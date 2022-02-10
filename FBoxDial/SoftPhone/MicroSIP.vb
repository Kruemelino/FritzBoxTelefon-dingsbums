Friend Class MicroSIP
    Implements IDisposable
    Implements IIPPhone

    Private Const MicroSIPProgressName As String = "MicroSIP"

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Public Property AppendSuffix As Boolean = True Implements IIPPhone.AppendSuffix

    Friend ReadOnly Property MicroSIPReady As Boolean Implements IIPPhone.IPPhoneReady
        Get
            Return Process.GetProcessesByName(MicroSIPProgressName).Length.IsNotZero
        End Get
    End Property
    Friend ReadOnly Property MicroSIPPath As String

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

    Public Sub New()
        If Not MicroSIPReady Then StartMicroSIP()

        MicroSIPPath = GetExecutablePath()
    End Sub

    Private Function GetExecutablePath() As String
        Dim ProcressMicroSIP As Process() = Process.GetProcessesByName(MicroSIPProgressName)

        If ProcressMicroSIP.Length.IsNotZero Then

            NLogger.Debug(Localize.LocWählclient.strMicroSIPBereit)

            ' Ermittle Pfad zur ausgeführten MicroSIP.exe
            Return ProcressMicroSIP.First.MainModule.FileName

            NLogger.Debug(String.Format(Localize.LocWählclient.strMicroSIPgestartetPfad, MicroSIPPath))

        Else
            Return String.Empty
        End If
    End Function

    Private Sub StartMicroSIP()
        NLogger.Debug(Localize.LocWählclient.strMicroSIPNichtBereit)

        If XMLData.POptionen.TBMicroSIPPath.IsNotStringNothingOrEmpty Then
            ' Starte MicroSIP
            Try
                Process.Start(XMLData.POptionen.TBMicroSIPPath)

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

    Friend Function Dial(DialCode As String, Hangup As Boolean) As Boolean Implements IIPPhone.Dial

        Dial = False

        If MicroSIPReady Then
            ' Wählkommando senden
            If Hangup Then
                ' Abbruch des Rufaufbaues mittels Parameter
                Process.Start(MicroSIPPath, CommandHangUpAll)

                NLogger.Debug(Localize.LocWählclient.strSoftPhoneAbbruch)
            Else
                If AppendSuffix Then DialCode += "#"

                ' Aufbau des Telefonates mittels Parameter 
                Process.Start(MicroSIPPath, DialCode)

                NLogger.Debug(String.Format(Localize.LocWählclient.strSoftPhoneErfolgreich, DialCode, MicroSIPProgressName))
            End If

        Else
            ' Phoner nicht verfügbar
            NLogger.Warn(Localize.LocWählclient.strMicroSIPNichtBereit)
        End If
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' Verwalteten Zustand (verwaltete Objekte) bereinigen
            End If

            ' Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
            ' Große Felder auf NULL setzen
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        ' Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub

End Class
