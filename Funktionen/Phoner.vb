Imports Microsoft.Win32
Imports System.Net
Imports System.IO
Public Class Phoner

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

        'System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(500))
        Dim Client As New Sockets.TcpClient()
        Dim remoteEP As New IPEndPoint(IPAddress.Parse("127.0.0.1"), 2012)
        Try
            Client.Connect(remoteEP)
            CheckIsPhonerRunning = True
            Client.Close()
        Catch Err As Exception
            CheckIsPhonerRunning = False
        End Try
        remoteEP = Nothing
        Client = Nothing
    End Function
End Class