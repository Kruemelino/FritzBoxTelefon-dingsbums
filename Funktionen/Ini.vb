Public Class InI
    Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
    Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lplFileName As String) As Integer
    Private Declare Function GetPrivateProfileSection Lib "kernel32" Alias "GetPrivateProfileSectionA" (ByVal lpAppName As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
    Private Declare Function WritePrivateProfileSection Lib "kernel32" Alias "WritePrivateProfileSectionA" (ByVal lpAppName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer

    Function Write(ByVal DateiName As String, ByVal DieSektion As String, ByVal DerEintrag As String, ByVal Value As String) As Integer

        Dim PfadName As String = IO.Path.GetDirectoryName(DateiName)
        Try
            If Not IO.Directory.Exists(PfadName) Then
                IO.Directory.CreateDirectory(PfadName) ' Directory erstellen
            End If
        Catch
            'LogFile("Es ist ein Fehler beim Erstellen des Ordners """ & PfadName & """ aufgetreten.")
        End Try

        Write = WritePrivateProfileString(DieSektion, DerEintrag, Value, DateiName)
    End Function

    Function Read(ByVal DateiName As String, ByVal DieSektion As String, ByVal DerEintrag As String, Optional ByVal Def As String = "False") As String
        Dim temp As String = Strings.Space(2048)
        Dim X As Int32
        X = GetPrivateProfileString(DieSektion, DerEintrag, Def, temp, Len(temp), DateiName) ' Make API Call
        If X > 0 Then
            Return temp.Substring(0, X)
        Else
            Return String.Empty
        End If
    End Function


    Function ReadSection(ByVal DateiName As String, ByVal DieSektion As String) As String()
        Dim temp As String = Strings.Space(8192)
        Dim X As Int32
        ReDim ReadSection(2)
        X = GetPrivateProfileSection(DieSektion, temp, Len(temp), DateiName) ' Make API Call
        If X > 0 And Not X = &H100000 - 2 Then
            Return Split(temp.Substring(0, X), ChrW(&H0), , CompareMethod.Text)
        End If
    End Function

    Function WriteSection(ByVal DateiName As String, ByVal DieSektion As String, ByVal DerEintrag As String) As Long
        WriteSection = WritePrivateProfileSection(DieSektion, DerEintrag, DateiName)
    End Function

End Class
