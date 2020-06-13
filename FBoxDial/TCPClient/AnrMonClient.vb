Imports System.Net.Sockets

''' <summary>
''' Abwandlung VersuchsChat mit leistungsfähigem Server von ErfinderDesRades
''' https://www.vb-paradise.de/index.php/Thread/61948-VersuchsChat-mit-leistungsf%C3%A4higem-Server
''' </summary>
Friend Class AnrMonClient
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
        ' TCP Client schließen
        AnrMonTcpClient.Close()

        Dispose()
    End Sub

    Private Sub EndRead(ByVal ar As IAsyncResult)
        If Verbunden And Not IsDisposed Then
            Dim read As Integer = AnrMonStream.EndRead(ar)
            If read.IsZero Then 'leere Datenübermittlung signalisiert Verbindungsabbruch
                Dispose()
            Else
                With New StringBuilder(Encoding.UTF8.GetString(Buf, 0, read))
                    Do While AnrMonStream.DataAvailable
                        read = AnrMonStream.Read(Buf, 0, Buf.Length)
                        .Append(Encoding.UTF8.GetString(Buf, 0, read))
                    Loop
                    RaiseEvent Message(Me, New NotifyEventArgs(Of String)(String.Concat(.ToString)))
                End With
                AnrMonStream.BeginRead(Buf, 0, Buf.Length, AddressOf EndRead, Nothing)
            End If

        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If IsDisposed Then Return
        IsDisposed = True
        ' Die erzwungenen Überschreibungen von Sub Dispose(Boolean) aufrufen

        AnrMonStream.Dispose()
        AnrMonTcpClient.Dispose()

        RaiseEvent Disposed(Me)
        GC.SuppressFinalize(Me)
    End Sub
End Class
