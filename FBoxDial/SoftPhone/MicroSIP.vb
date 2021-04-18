﻿Friend Class MicroSIP
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

    Public Sub New()

        Dim ProcressMicroSIP As Process()
        ProcressMicroSIP = Process.GetProcessesByName(MicroSIPProgressName)

        If ProcressMicroSIP.Length.IsNotZero Then

            NLogger.Debug(Localize.LocWählclient.strMicroSIPBereit)

            ' Ermittle Pfad zur ausgeführten MicroSIP.exe
            XMLData.POptionen.TBMicroSIPPath = ProcressMicroSIP.First.MainModule.FileName

            NLogger.Debug(String.Format(Localize.LocWählclient.strMicroSIPgestartetPfad, XMLData.POptionen.TBMicroSIPPath))

        Else
            NLogger.Debug(Localize.LocWählclient.strMicroSIPNichtBereit)

            If XMLData.POptionen.TBMicroSIPPath.IsNotStringNothingOrEmpty Then
                ' Starte MicroSIP
                Process.Start(XMLData.POptionen.TBMicroSIPPath)
                NLogger.Info(Localize.LocWählclient.strMicroSIPgestartet)
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

                NLogger.Debug(Localize.LocWählclient.strSoftPhoneAbbruch)
            Else
                ' Aufbau des Telefonates mittels Parameter 
                Process.Start(XMLData.POptionen.TBMicroSIPPath, DialCode)

                NLogger.Debug(String.Format(Localize.LocWählclient.strSoftPhoneAbbruch, DialCode, MicroSIPProgressName))
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

    ' Finalizer nur überschreiben, wenn "Dispose(disposing As Boolean)" Code für die Freigabe nicht verwalteter Ressourcen enthält
    ' Protected Overrides Sub Finalize()
    '     ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        Dispose(disposing:=True)
        ' Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
End Class
