Public Class InI
    Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
    Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lplFileName As String) As Integer
    Private Declare Function GetPrivateProfileSection Lib "kernel32" Alias "GetPrivateProfileSectionA" (ByVal lpAppName As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
    Private Declare Function WritePrivateProfileSection Lib "kernel32" Alias "WritePrivateProfileSectionA" (ByVal lpAppName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer

    Private SecOptionen As String()
    Private SecTelefone As String()
    Private SecJournal As String()
    Private SecStatistik As String()
    Private SecPhoner As String()

    Private sDateiName As String

    Public Sub New(ByVal Filename As String)
        sDateiName = Filename
        INIreload()
    End Sub


    Function Write(ByVal DateiName As String, ByVal DieSektion As String, ByVal DerEintrag As String, ByVal Value As String) As Integer

        Dim PfadName As String = IO.Path.GetDirectoryName(DateiName)
        Try
            If Not IO.Directory.Exists(PfadName) Then
                IO.Directory.CreateDirectory(PfadName) ' Directory erstellen
            End If
        Catch : End Try

        Write = WritePrivateProfileString(DieSektion, DerEintrag, Value, DateiName)
    End Function

    Overloads Function Read(ByVal DateiName As String, ByVal DieSektion As String, ByVal DerEintrag As String, ByVal Def As String) As String
        Dim temp As String = Strings.Space(2048)
        Dim X As Int32
        X = GetPrivateProfileString(DieSektion, DerEintrag, Def, temp, Len(temp), DateiName) ' Make API Call
        If X > 0 Then
            Return temp.Substring(0, X)
        Else
            Return String.Empty
        End If
    End Function

    Overloads Function Read(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal Def As String) As String
        Dim sSection As String()
        Dim tmpEintrag As String()

        Select Case DieSektion
            Case "Optionen"
                sSection = SecOptionen
            Case "Telefone"
                sSection = SecTelefone
            Case "Journal"
                sSection = SecJournal
            Case "Statistik"
                sSection = SecStatistik
            Case "Phoner"
                sSection = SecPhoner
            Case Else
                sSection = Nothing
        End Select

        If Not sSection Is Nothing Then
            tmpEintrag = (From x In sSection Where x Like DerEintrag & "=*" Select x).ToArray
            If tmpEintrag.Length > 0 Then
                Return Split(tmpEintrag(0), "=", , CompareMethod.Text)(1)
            End If
        End If
        Return Def
    End Function

    Function ReadSection(ByVal DieSektion As String) As String()
        Dim temp As String = Strings.Space(8192)
        Dim X As Int32
        ReDim ReadSection(2)
        X = GetPrivateProfileSection(DieSektion, temp, Len(temp), sDateiName) ' Make API Call
        If X > 0 And Not X = &H100000 - 2 Then
            Return Split(temp.Substring(0, X), ChrW(&H0), , CompareMethod.Text)
        End If
    End Function

    Function WriteSection(ByVal DieSektion As String, ByVal DerEintrag As String) As Long
        WriteSection = WritePrivateProfileSection(DieSektion, DerEintrag, sDateiName)
    End Function

    Public Function WriteIniSection(ByVal sSection As String, ByVal sValues() As String) As Boolean
        Dim s As String = ""
        For Each a As String In sValues
            If Not a = vbNullString Then s &= a & ControlChars.NullChar
        Next
        s &= ControlChars.NullChar
        Return WritePrivateProfileSection(sSection, s, sDateiName) = 0
    End Function

    Sub INIreload()
        SecOptionen = ReadSection("Optionen")
        SecTelefone = ReadSection("Telefone")
        SecJournal = ReadSection("Journal")
        SecStatistik = ReadSection("Statistik")
        SecPhoner = ReadSection("Phoner")
    End Sub

    Protected Overrides Sub Finalize()
        SecOptionen = Nothing
        SecTelefone = Nothing
        SecJournal = Nothing
        SecStatistik = Nothing
        SecPhoner = Nothing
        MyBase.Finalize()
    End Sub
End Class
