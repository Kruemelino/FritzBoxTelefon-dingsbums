Imports System.Net.Sockets
Public Class NotifyEventArgs(Of T) : Inherits EventArgs
    Public ReadOnly Value As T
    Public Sub New(Value As T)
        MyBase.New()
        Me.Value = Value
    End Sub
End Class 'NotifyEventArgs(Of T)

''' <summary>
''' Eventhandler, der den Sender ordentlich typisiert �bermittelt
''' </summary>
Public Delegate Sub EventHandlerEx(Of T0)(Sender As T0)

''' <summary>
''' Abwandlung VersuchsChat mit leistungsf�higem Server von ErfinderDesRades
''' https://www.vb-paradise.de/index.php/Thread/61948-VersuchsChat-mit-leistungsf%C3%A4higem-Server
''' </summary>
Public Class AnrMonClient
    Implements IDisposable

    Private Property AnrMonTcpClient As TcpClient
    Private Property AnrMonStream As NetworkStream
    Private Property IsDisposed As Boolean = False
    Friend Property Verbunden As Boolean

    Public Event Disposed As EventHandlerEx(Of AnrMonClient)
    Public Event Message As EventHandler(Of NotifyEventArgs(Of String))
    Private ReadOnly Buf(&H400 - 1) As Byte

    Friend Sub New(TC As TcpClient)
        AnrMonTcpClient = TC
        Verbunden = False
    End Sub

    Friend Sub Connect()
        AnrMonStream = AnrMonTcpClient.GetStream
        AnrMonStream.BeginRead(Buf, 0, Buf.Length, AddressOf EndRead, Nothing)

        Verbunden = True
    End Sub

    Friend Sub Disconnect()

        Verbunden = False
        ' NetworkStream schlie0en
        AnrMonStream.Close()
        ' TCP Client schlie�en
        AnrMonTcpClient.Close()

        Dispose()
    End Sub


    Private Sub EndRead(ByVal ar As IAsyncResult)
        If Verbunden And Not IsDisposed Then
            Dim read As Integer = AnrMonStream.EndRead(ar)
            If read = 0 Then 'leere Daten�bermittlung signalisiert Verbindungsabbruch
                Dispose()
                Return
            End If

            Dim SB As New StringBuilder(Encoding.UTF8.GetString(Buf, 0, read))
            Do While AnrMonStream.DataAvailable
                read = AnrMonStream.Read(Buf, 0, Buf.Length)
                SB.Append(Encoding.UTF8.GetString(Buf, 0, read))
            Loop
            RaiseEvent Message(Me, New NotifyEventArgs(Of String)(String.Concat(SB.ToString)))

            AnrMonStream.BeginRead(Buf, 0, Buf.Length, AddressOf EndRead, Nothing)
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If IsDisposed Then Return
        IsDisposed = True
        ' Die erzwungenen �berschreibungen von Sub Dispose(Boolean) aufrufen

        AnrMonStream.Dispose()
        AnrMonTcpClient.Dispose()

        RaiseEvent Disposed(Me)
        GC.SuppressFinalize(Me)
    End Sub
End Class
