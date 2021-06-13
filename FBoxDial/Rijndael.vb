Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Security.Cryptography

''' <remarks>http://www.freevbcode.com/ShowCode.asp?ID=4520</remarks>
<DebuggerStepThrough>
Friend Class Rijndael
    Implements IDisposable
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    ''' <summary>
    ''' Verschlüsselungsroutine
    ''' </summary>
    ''' <param name="vstrTextToBeEncrypted">Verschlüsselte Zeichenfolge</param>
    ''' <returns>Die verschlüsselte Zeichenfolge</returns>
    Friend Function EncryptString(vstrTextToBeEncrypted As SecureString, vstrDeCryptKey As String) As String
        ' Standardwert
        EncryptString = DfltStrErrorMinusOne

        ' Test ob gültige Eingangsdaten vorhanden
        If vstrTextToBeEncrypted IsNot Nothing Then

            ' Erstelle einen Zufälligen Zeichenfolge als Salt
            Dim Salt() As Byte = GetRndByteArray(16)
            ' Erstelle einen Zufälligen Schlüssel
            Dim EncryptionKey() As Byte = GetRndByteArray(32)

            ' Speichere den Salt und Key in der Registry ab
            SaveSetting(My.Resources.strDefShortName, DfltOptions, vstrDeCryptKey, Salt.Append(EncryptionKey).ToBase64String)

            ' Create the encryptor and write value to it after it is converted into a byte array
            Using rijAlg As New RijndaelManaged()
                With rijAlg
                    .KeySize = 256
                    .BlockSize = 256
                    .Mode = CipherMode.CBC
                    Using rfc = New Rfc2898DeriveBytes(EncryptionKey, Salt, 1000)
                        .IV = rfc.GetBytes(.BlockSize \ 8)
                        .Key = rfc.GetBytes(.KeySize \ 8)
                    End Using

                    ' Create a encrytor to perform the stream transform. 
                    Using encryptor As ICryptoTransform = rijAlg.CreateEncryptor(.Key, .IV)
                        Dim Buffer As Byte() = ToByteArray(vstrTextToBeEncrypted, Encoding.Unicode)
                        Try
                            Return encryptor.TransformFinalBlock(Buffer, 0, Buffer.Length).ToBase64String
                        Finally
                            If Buffer IsNot Nothing Then Array.Clear(Buffer, 0, Buffer.Length)
                        End Try
                    End Using
                End With
            End Using

        End If
    End Function

    ''' <summary>
    ''' Wandelt einen <see cref="SecureString"/> in ein Array von <see cref="Byte"/> um.
    ''' </summary>
    ''' <param name="secureString">Die Zeichenfolge als <see cref="SecureString"/>, welche umgewandelt werden soll. </param>
    ''' <param name="encoding">Zeichencodierung</param>
    ''' <returns>ByteArray</returns>
    Private Function ToByteArray(secureString As SecureString, encoding As Encoding) As Byte()

        Dim unmanagedString As IntPtr = IntPtr.Zero

        Try
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString)
            Return encoding.GetBytes(Marshal.PtrToStringUni(unmanagedString))
        Finally
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString)
        End Try

    End Function

    Private Function GetSecureString(ByRef decryptedBuffer As Byte()) As SecureString
        Dim output As New SecureString

        Dim outputBuffer As Char() = Encoding.Unicode.GetChars(decryptedBuffer)

        For i As Integer = 0 To outputBuffer.Length - 1
            output.AppendChar(outputBuffer(i))
        Next

        output.MakeReadOnly()

        If outputBuffer IsNot Nothing Then Array.Clear(outputBuffer, 0, outputBuffer.Length)

        If decryptedBuffer IsNot Nothing Then Array.Clear(decryptedBuffer, 0, decryptedBuffer.Length)

        Return output
    End Function

    Friend Function GetUnSecureString(secstrPassword As SecureString) As String
        Dim unmanagedString As IntPtr = IntPtr.Zero

        Try
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secstrPassword)
            Return Marshal.PtrToStringUni(unmanagedString)
        Finally
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString)
        End Try
    End Function

    ''' <summary>
    ''' Entschlüsselungsroutine
    ''' </summary>
    ''' <param name="vstrStringToBeDecrypted">Verschlüsselte Zeichenfolge</param>
    ''' <returns>Die entschlüsselte Zeichenfolge als <see cref="SecureString"/></returns>
    Friend Function DecryptString(vstrStringToBeDecrypted As String, vstrDeCryptKey As String) As SecureString
        ' Lese den Key aus der Registry aus
        Dim DecryptionSaltKey As String = GetSetting(My.Resources.strDefShortName, DfltOptions, vstrDeCryptKey, DfltStrErrorMinusOne)
        Dim buffer() As Byte = Nothing

        ' Test ob gültige Eingangsdaten vorhanden
        If vstrStringToBeDecrypted.IsNotErrorString And vstrStringToBeDecrypted.IsNotStringEmpty And DecryptionSaltKey.IsNotErrorString Then
            ' Extrahiere aus dem DecryptionSaltKey den Salt und den Key
            Dim SaltKey As Byte()() = DecryptionSaltKey.FromBase64String.SplitByte(16)

            Try
                Using rijAlg As New RijndaelManaged
                    With rijAlg
                        .KeySize = 256
                        .BlockSize = 256
                        .Mode = CipherMode.CBC
                        Using rfc = New Rfc2898DeriveBytes(SaltKey(1), SaltKey(0), 1000)
                            .IV = rfc.GetBytes(.BlockSize \ 8)
                            .Key = rfc.GetBytes(.KeySize \ 8)
                        End Using

                        ' Create a decrytor to perform the stream transform. 
                        Using decryptor = rijAlg.CreateDecryptor(.Key, .IV)
                            buffer = vstrStringToBeDecrypted.FromBase64String
                            Return GetSecureString(decryptor.TransformFinalBlock(buffer, 0, buffer.Length))
                        End Using
                    End With
                End Using
            Catch ex As Exception
                ' Die Ausnahme tritt ein, wenn die Entschlüsselung nicht möglich ist.
                NLogger.Error(ex)
            Finally
                ' Bereinige den Buffer
                If buffer IsNot Nothing Then Array.Clear(buffer, 0, buffer.Length)
            End Try
        End If
        Return Nothing
    End Function

    Private Function GetRndByteArray(maximumLength As Integer) As Byte()
        Dim RndByte(maximumLength - 1) As Byte
        Using rng As RandomNumberGenerator = New RNGCryptoServiceProvider
            rng.GetNonZeroBytes(RndByte)
        End Using
        Return RndByte
    End Function

    Friend Function GetMd5Hash(input As String, Enkodierung As Encoding) As String

        Using md5 As MD5 = New MD5CryptoServiceProvider
            Dim sBuilder As New StringBuilder()
            For Each b As Byte In md5.ComputeHash(Enkodierung.GetBytes(input.ToCharArray))
                sBuilder.Append(b.ToString("x2"))
            Next
            Return sBuilder.ToString()
        End Using

    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    ' IDisposable
    <DebuggerStepThrough>
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ' nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub

    ' Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    <DebuggerStepThrough>
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class

