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
    ''' <param name="ToBeEncrypted">Verschlüsselte Zeichenfolge</param>
    ''' <param name="vstrDeCryptKey">Generierter Schlüsselzeichenfolge</param>
    ''' <returns>Die verschlüsselte Zeichenfolge</returns>
    Friend Function EncryptString(ToBeEncrypted As SecureString, vstrDeCryptKey As String) As String
        ' Standardwert
        EncryptString = DfltStrErrorMinusOne

        ' Test ob gültige Eingangsdaten vorhanden
        If ToBeEncrypted IsNot Nothing Then

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
                        Dim Buffer As Byte() = ToByteArray(ToBeEncrypted, Encoding.Unicode)
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
    ''' Entschlüsselungsroutine
    ''' </summary>
    ''' <param name="vstrStringToBeDecrypted">Verschlüsselte Zeichenfolge</param>
    ''' <param name="vstrDeCryptKey">Schlüsselzeichenfolge</param>
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
                            Return GetSecureString(decryptor.TransformFinalBlock(buffer, 0, buffer.Length), Encoding.Unicode)
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

    ''' <summary>
    ''' Wandelt einen <see cref="SecureString"/> in ein Array von <see cref="Byte"/> um.
    ''' </summary>
    ''' <param name="secureString">Die Zeichenfolge als <see cref="SecureString"/>, welche umgewandelt werden soll.</param>
    ''' <param name="encoding">Zeichencodierung</param>
    ''' <returns>ByteArray</returns>
    Private Function ToByteArray(secureString As SecureString, encoding As Encoding) As Byte()
        Dim unmanagedString As IntPtr = IntPtr.Zero

        Try
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString)
            Return encoding.GetBytes(Marshal.PtrToStringUni(unmanagedString))
        Catch ex As Exception
            NLogger.Error(ex)
            Return {}
        Finally
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString)
        End Try

    End Function

    ''' <summary>
    ''' Wandelt ein Array von <see cref="Byte"/> in ein <see cref="SecureString"/> um.
    ''' </summary>
    ''' <param name="decryptedBuffer"><see cref="Byte"/>-Array, welches umgewandelt werden soll.</param>
    ''' <param name="encoding">Zeichencodierung</param>
    ''' <returns>SecureString</returns>
    Private Function GetSecureString(ByRef decryptedBuffer As Byte(), encoding As Encoding) As SecureString
        Dim output As New SecureString

        Dim outputBuffer As Char() = encoding.GetChars(decryptedBuffer)

        For i As Integer = 0 To outputBuffer.Length - 1
            output.AppendChar(outputBuffer(i))
        Next

        output.MakeReadOnly()

        If outputBuffer IsNot Nothing Then Array.Clear(outputBuffer, 0, outputBuffer.Length)

        If decryptedBuffer IsNot Nothing Then Array.Clear(decryptedBuffer, 0, decryptedBuffer.Length)

        Return output
    End Function

    ''' <summary>
    ''' Generiert ein <see cref="Byte"/>-Array mit zufälligen Werten.
    ''' </summary>
    ''' <param name="maximumLength">Länge des <see cref="Byte"/>-Array</param>
    ''' <returns>ByteArray</returns>
    Private Function GetRndByteArray(maximumLength As Integer) As Byte()
        Dim RndByte(maximumLength - 1) As Byte
        Using rng As RandomNumberGenerator = New RNGCryptoServiceProvider
            rng.GetNonZeroBytes(RndByte)
        End Using
        Return RndByte
    End Function

    ''' <summary>
    ''' Erstellt einen MD5-Hash eines <see cref="SecureString"/> durch.
    ''' </summary>
    ''' <param name="secureString">Verschlüsselte Zeichenfolge</param>
    ''' <param name="Zeichencodierung">Zeichencodierung</param>
    ''' <param name="Präfix">Optionaler Präfix, welcher vor dem erstellen des Hashes dem <see cref="SecureString"/> vorangestellt wird.</param>
    ''' <returns></returns>
    Friend Function SecureStringToMD5(secureString As SecureString, Zeichencodierung As Encoding, Optional Präfix As String = "") As String

        Dim BufferSecuredString As Byte() = ToByteArray(secureString, Zeichencodierung)
        Dim BufferPräfix As Byte() = Zeichencodierung.GetBytes(Präfix)
        Dim Buffer(BufferSecuredString.Length + BufferPräfix.Length - 1) As Byte
        Try

            BufferPräfix.CopyTo(Buffer, 0)
            BufferSecuredString.CopyTo(Buffer, BufferPräfix.Length)

            Using md5 As MD5 = New MD5CryptoServiceProvider
                Dim sBuilder As New StringBuilder()
                For Each b As Byte In md5.ComputeHash(Buffer)
                    sBuilder.Append(b.ToString("x2"))
                Next
                Return sBuilder.ToString()
            End Using

        Finally
            If BufferSecuredString IsNot Nothing Then Array.Clear(BufferSecuredString, 0, BufferSecuredString.Length)
            If Buffer IsNot Nothing Then Array.Clear(Buffer, 0, Buffer.Length)
        End Try

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
