Imports Microsoft.Win32
Imports System.Net.Sockets
Imports System.IO
Public Class PhonerInterface
    Private C_XML As MyXML
    Private hf As Helfer
    Private Crypt As Rijndael

    Private PhonerAddresse As String = "127.0.0.1"
    Private PhonerAnrMonPort As Integer = 2012

    Public Sub New(ByVal HelferKlasse As Helfer, _
                   ByVal XMLKlasse As MyXML, _
                   ByVal cryptKlasse As Rijndael)

        Crypt = cryptKlasse
        C_XML = XMLKlasse
        hf = HelferKlasse
    End Sub

    Public Function PhonerReady() As Boolean
        PhonerReady = False
        Return CheckIsPhonerInstalled() And CheckIsPhonerRunning()
    End Function

    Private Function CheckIsPhonerInstalled() As Boolean
        CheckIsPhonerInstalled = False
        ' Funktion von Klaus Raykowski (info@sbv-computer.de) übernommen und an 64bit-Systeme angepasst

        'Püft ob Phoner installiert ist.
        'Die Prüfung auf vorhandene CAPI kann entfallen
        'da kein Fehler auftritt wenn diese nicht vorhanden ist außer
        'das das Programm nicht wählt. 

        Try
            Dim strClassID As String = "-"
            Dim RegKey As RegistryKey
            RegKey = Registry.ClassesRoot.OpenSubKey("Phoner.CPhoner\CLSID", False)
            strClassID = CType(RegKey.GetValue("", "-"), String)
            If Not strClassID = "-" Then
                RegKey = Registry.ClassesRoot.OpenSubKey("CLSID\" & strClassID & "\LocalServer32", False)
                If RegKey Is Nothing Then
                    RegKey = Registry.ClassesRoot.OpenSubKey("Wow6432Node\CLSID\" & strClassID & "\LocalServer32", False)
                End If
                If Not CType(RegKey.GetValue("", "-"), String) = "-" Then CheckIsPhonerInstalled = True
            End If
            RegKey.Close()
        Catch : End Try
    End Function

    Private Function CheckIsPhonerRunning() As Boolean
        CheckIsPhonerRunning = False
        'Püft ob Phoner läuft.

        Dim Client As New TcpClient()
        Dim RemoteEP As New Net.IPEndPoint(Net.IPAddress.Parse(PhonerAddresse), PhonerAnrMonPort)
        Try
            Client.Connect(RemoteEP)
            CheckIsPhonerRunning = True
            Client.Close()
        Catch
            CheckIsPhonerRunning = False
        End Try
        RemoteEP = Nothing
        Client = Nothing
    End Function

    Public Function DialPhoner(ByVal dialCode As String) As String
        If PhonerReady() Then
            Dim PhonerPasswort As String = C_XML.Read("Phoner", "PhonerPasswort", "-1")
            Dim ZugangPasswortPhoner As String = GetSetting("FritzBox", "Optionen", "ZugangPasswortPhoner", "-1")
            If Not PhonerPasswort = "-1" Or Not ZugangPasswortPhoner = "-1" Then
                Dim Stream As NetworkStream
                Dim remoteEP As New System.Net.IPEndPoint(Net.IPAddress.Parse(PhonerAddresse), PhonerAnrMonPort)
                Dim tcpClient As New TcpClient()

                tcpClient.Connect(remoteEP)
                Stream = tcpClient.GetStream()

                If Not Stream Is Nothing Then
                    Dim StreamWriter As New StreamWriter(Stream)
                    Dim StreamReader As New StreamReader(Stream)
                    If Stream.CanWrite Then
                        With StreamWriter
                            .WriteLine("Login")
                            .AutoFlush = True
                            If StreamReader.ReadLine() = "Welcome to Phoner" Then
                                Dim Challenge As String = Mid(StreamReader.ReadLine(), Strings.Len("Challenge=") + 1)
                                Dim Response As String = UCase(Crypt.getMd5Hash(Challenge & Crypt.DecryptString128Bit(PhonerPasswort, ZugangPasswortPhoner), System.Text.Encoding.ASCII))
                                .WriteLine("Response=" & Response)
                                System.Threading.Thread.Sleep(100)
                                If Stream.DataAvailable Then
                                    .WriteLine("CONNECT " & dialCode)
                                    DialPhoner = "Nr. " & dialCode & " an Phoner übergeben"
                                Else
                                    DialPhoner = "Fehler!" & vbCrLf & "Das Phoner-Passwort ist falsch!"
                                End If
                            Else
                                DialPhoner = "Fehler!" & vbCrLf & "Die Phoner-Verson ist zu alt!"
                            End If
                        End With
                    Else
                        DialPhoner = "Fehler!" & vbCrLf & "TCP Fehler (Stream.CanWrite = False)!"
                    End If
                    StreamWriter = Nothing
                    StreamReader = Nothing
                Else
                    DialPhoner = "Fehler!" & vbCrLf & "TCP!"
                End If
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(500))
                tcpClient.Close()
                tcpClient = Nothing
                Stream = Nothing
                hf.KeyChange()
            Else
                DialPhoner = "Fehler!" & vbCrLf & "Kein Passwort hinterlegt!"
            End If
        Else
            DialPhoner = "Fehler!" & vbCrLf & "Phoner nicht verfügbar!"
        End If
    End Function

End Class