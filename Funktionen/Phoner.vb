﻿Imports Microsoft.Win32
Imports System.Net.Sockets
Imports System.IO
Public Class PhonerInterface
    Private C_DP As DataProvider
    Private C_hf As Helfer
    Private C_Crypt As Rijndael

    Private PhonerAddresse As String = "127.0.0.1"
    Private PhonerAnrMonPort As Integer = 2012

    Public Sub New(ByVal HelferKlasse As Helfer,
                   ByVal DataProviderKlasse As DataProvider,
                   ByVal CryptKlasse As Rijndael)

        C_Crypt = CryptKlasse
        C_DP = DataProviderKlasse
        C_hf = HelferKlasse
    End Sub

    <DebuggerStepThrough>
    Public Function PhonerReady() As Boolean
        Return Not Diagnostics.Process.GetProcessesByName("phoner").Length = 0
    End Function

    Public Function DialPhoner(ByVal dialCode As String) As String
        If PhonerReady() Then
            Dim PhonerPasswort As String = C_DP.P_TBPhonerPasswort
            Dim ZugangPasswortPhoner As String = C_DP.GetSettingsVBA("ZugangPasswortPhoner", DataProvider.P_Def_ErrorMinusOne_String)

            Dim Stream As NetworkStream
            Dim remoteEP As System.Net.IPEndPoint
            Dim PhonertcpClient As TcpClient
            Dim StreamWriter As StreamWriter
            Dim StreamReader As StreamReader
            Dim Zeile As String
            Dim Challenge As String
            Dim Response As String
            If Not PhonerPasswort = DataProvider.P_Def_ErrorMinusOne_String Or Not ZugangPasswortPhoner = DataProvider.P_Def_ErrorMinusOne_String Then

                remoteEP = New System.Net.IPEndPoint(Net.IPAddress.Parse(PhonerAddresse), PhonerAnrMonPort)
                PhonertcpClient = New TcpClient()

                PhonertcpClient.Connect(remoteEP)
                Stream = PhonertcpClient.GetStream()

                If Stream IsNot Nothing Then
                    StreamWriter = New StreamWriter(Stream)
                    StreamReader = New StreamReader(Stream)

                    If Stream.CanWrite Then
                        With StreamWriter
                            .WriteLine("Login")
                            .AutoFlush = True
                            Zeile = StreamReader.ReadLine()
                            If LCase(Zeile) = LCase(DataProvider.P_Def_Phoner_Ready) Then ' "Welcome to Phoner" 
                                Challenge = Mid(StreamReader.ReadLine(), Strings.Len(DataProvider.P_Def_Phoner_Challenge) + 1)
                                ' Anmerkung: Hat bis jetzt funktioniert. Aber es kann sein, dass eine Umwandlung der Zeichen, dessen Codepoint > 255 ist, nicht notig ist.
                                Response = UCase(C_Crypt.getMd5Hash(Challenge & C_Crypt.DecryptString128Bit(PhonerPasswort, ZugangPasswortPhoner), Text.Encoding.ASCII, True))
                                .WriteLine(DataProvider.P_Def_Phoner_Response & Response)
                                C_hf.ThreadSleep(100)
                                If Stream.DataAvailable Then
                                    If dialCode = DataProvider.P_Def_Phoner_DISCONNECT Then  '"DISCONNECT"
                                        .WriteLine(dialCode)
                                    Else
                                        .WriteLine(DataProvider.P_Def_Phoner_CONNECT & dialCode)
                                    End If
                                    DialPhoner = DataProvider.P_Lit_Phoner1(dialCode) '"Nr. " & dialCode & " an Phoner übergeben"
                                Else
                                    DialPhoner = DataProvider.P_Lit_Phoner2 '"Fehler!" & vbCrLf & "Das Phoner-Passwort ist falsch!"
                                End If
                            Else
                                DialPhoner = DataProvider.P_Lit_Phoner3 '"Fehler!" & vbCrLf & "Die Phoner-Verson ist zu alt!"
                            End If
                        End With
                    Else
                        DialPhoner = DataProvider.P_Lit_Phoner4 '"Fehler!" & vbCrLf & "TCP Fehler (Stream.CanWrite = False)!"
                    End If

                Else
                    DialPhoner = DataProvider.P_Lit_Phoner5 '"Fehler!" & vbCrLf & "TCP!"
                End If
                C_hf.ThreadSleep(500)
                C_hf.KeyChange()
                PhonertcpClient.Close()
            Else
                DialPhoner = DataProvider.P_Lit_Phoner6 '"Fehler!" & vbCrLf & "Kein Passwort hinterlegt!"
            End If

            Stream = Nothing
            PhonertcpClient = Nothing
            StreamWriter = Nothing
            StreamReader = Nothing
            remoteEP = Nothing
        Else
            DialPhoner = DataProvider.P_Lit_Phoner7 '"Fehler!" & vbCrLf & "Phoner nicht verfügbar!"
        End If
    End Function

End Class