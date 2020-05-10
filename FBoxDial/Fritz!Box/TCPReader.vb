Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Threading.Tasks
Public Class TCPReader
    Private Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
    Private Property Endpoint As String
    Private Property EndpointPort As Integer

    Private ReadOnly pts As New ParameterizedThreadStart(AddressOf SteamMonitor)

    Friend Event DataAvailable(ByVal Data As String, ByVal Simuliert As Boolean)
    Friend Event Connected()
    Friend Event Disconnected()
    Friend Property Disconnect As Boolean
    Friend Property Verbunden As Boolean

    Friend Sub New(ByVal IPEndpoint As String, ByVal PortEndpoint As Integer)
        Verbunden = False
        Disconnect = False

        Endpoint = IPEndpoint
        EndpointPort = PortEndpoint
        'Connect()
    End Sub

    Friend Async Sub Connect()
        ' Baue den TCPSocket auf
        Dim TCPSocket As Socket
        Dim DataStream As NetworkStream
        Dim ReceiveThread As Thread
        Dim param_obj(2) As Object
        ' Asynchoner Vorgang des Aufbaues
        TCPSocket = Await ConnectSocket()
        ' Wenn die Verbindung besteht, etabliere den Networkstream
        If TCPSocket IsNot Nothing AndAlso TCPSocket.Connected Then
            DataStream = New NetworkStream(TCPSocket)
            If DataStream.CanRead Then
                Verbunden = True
                RaiseEvent Connected()

                param_obj(0) = DataStream
                param_obj(1) = TCPSocket
                ReceiveThread = New Thread(pts)
                With ReceiveThread
                    .IsBackground = True
                    .Start(param_obj)
                End With
            End If
        End If
    End Sub

    Private Async Function ConnectSocket() As Task(Of Socket)
        Dim TCPSocket As Socket
        Dim IPAddresse As IPAddress = IPAddress.Loopback
        Dim RemoteEP As IPEndPoint

        If IPAddress.TryParse(Endpoint, IPAddresse) Then

            ' Gegenstelle festlegen
            RemoteEP = New IPEndPoint(IPAddresse, EndpointPort)
            ' Socket definieren
            TCPSocket = New Socket(IPAddresse.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            ' Versuche eine Verbindung aufzubauen
            Try
                Await TCPSocket.ConnectAsync(RemoteEP)
                Return TCPSocket
            Catch SocketError As SocketException
                Verbunden = False
                NLogger.Error(SocketError)
                'Select Case SocketError.SocketErrorCode
                '    Case Sockets.SocketError.AccessDenied
                '    Case Sockets.SocketError.ConnectionRefused
                'End Select
                Return Nothing
            End Try
        Else
            Return Nothing
        End If
    End Function
    Private Sub SteamMonitor(ByVal obj As Object)
        Dim param_obj() As Object = DirectCast(obj, Object())

        Dim DataStream As NetworkStream = DirectCast(param_obj(0), NetworkStream)
        Dim TCPSocket As Socket = DirectCast(param_obj(1), Socket)

        If TCPSocket IsNot Nothing AndAlso TCPSocket.Connected Then
            Using sR As New StreamReader(DataStream)
                Do While TCPSocket.Connected And Not Disconnect
                    If DataStream.DataAvailable Then
                        RaiseEvent DataAvailable(sR.ReadLine, False)
                    End If
                Loop
                If Disconnect Then
                    Verbunden = False
                    RaiseEvent Disconnected()
                    TCPSocket.Disconnect(False)
                End If
            End Using
        End If
    End Sub

End Class
