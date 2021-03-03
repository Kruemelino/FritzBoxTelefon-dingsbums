Imports System.Security.Cryptography

''' <remarks>http://www.freevbcode.com/ShowCode.asp?ID=4520</remarks>
Friend Class Rijndael
    Implements IDisposable
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    ''' <summary>
    ''' Verschlüsselungsroutine
    ''' </summary>
    ''' <param name="vstrTextToBeEncrypted">Verschlüsselte Zeichenfolge</param>
    ''' <returns>Die verschlüsselte Zeichenfolge</returns>
    Friend Function EncryptString128Bit(vstrTextToBeEncrypted As String, vstrDeCryptKey As String) As String
        ' Standardwert
        EncryptString128Bit = DfltStrErrorMinusOne

        ' Test ob gültige Eingangsdaten vorhanden
        If vstrTextToBeEncrypted.IsNotErrorString And vstrTextToBeEncrypted.IsNotStringEmpty Then

            ' Erstelle einen Zufälligen Zeichenfolge als Salt
            Dim Salt() As Byte = GetSalt(16)
            ' Erstelle einen Zufälligen Schlüssel
            Dim EncryptionKey() As Byte = GetRndKey(32)

            ' Speichere den Salt und Key in der Registry ab
            SaveSetting(My.Resources.strDefShortName, DfltOptions, vstrDeCryptKey, Salt.Append(EncryptionKey).ToBase64String)

            ' Create the encryptor and write value to it after it is converted into a byte array
            Using rijAlg As New RijndaelManaged()
                With rijAlg
                    .KeySize = 256
                    .BlockSize = 256
                    .Mode = CipherMode.CBC
                    Using rfc = New Rfc2898DeriveBytes(EncryptionKey, Salt, 1000)
                        .IV = rfc.GetBytes(CInt(.BlockSize \ 8))
                        .Key = rfc.GetBytes(CInt(.KeySize \ 8))
                    End Using

                    ' Create a encrytor to perform the stream transform. 
                    Using encryptor As ICryptoTransform = rijAlg.CreateEncryptor(.Key, .IV)
                        Dim buffer() As Byte = Encoding.Unicode.GetBytes(vstrTextToBeEncrypted)
                        Return encryptor.TransformFinalBlock(buffer, 0, buffer.Length).ToBase64String
                    End Using
                End With
            End Using
        End If
    End Function

    ''' <summary>
    ''' Entschlüsselungsroutine
    ''' </summary>
    ''' <param name="vstrStringToBeDecrypted">Verschlüsselte Zeichenfolge</param>
    ''' <returns>Die entschlüsselte Zeichenfolge</returns>
    Friend Function DecryptString128Bit(vstrStringToBeDecrypted As String, vstrDeCryptKey As String) As String
        ' Lese den Key aus der Registry aus
        Dim DecryptionSaltKey As String = GetSetting(My.Resources.strDefShortName, DfltOptions, vstrDeCryptKey, DfltStrErrorMinusOne)
        ' Standardwert
        DecryptString128Bit = DfltStrErrorMinusOne
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
                            .IV = rfc.GetBytes(CInt(.BlockSize \ 8))
                            .Key = rfc.GetBytes(CInt(.KeySize \ 8))
                        End Using

                        ' Create a decrytor to perform the stream transform. 
                        Using decryptor = rijAlg.CreateDecryptor(.Key, .IV)
                            Dim buffer() As Byte = vstrStringToBeDecrypted.FromBase64String
                            Return Encoding.Unicode.GetString(decryptor.TransformFinalBlock(buffer, 0, buffer.Length))
                        End Using
                    End With
                End Using
            Catch ex As Exception
                ' Die Ausnahme tritt ein, wenn die Entschlüsselung nicht möglich ist.
                NLogger.Error(ex)
            End Try
        End If
    End Function

    <DebuggerStepThrough>
    Private Function GetRndKey(maximumSaltLength As Integer) As Byte()
        Dim RndKey(maximumSaltLength - 1) As Byte
        Using rng As RandomNumberGenerator = New RNGCryptoServiceProvider
            rng.GetNonZeroBytes(RndKey)
        End Using
        Return RndKey
    End Function
    <DebuggerStepThrough>
    Private Function GetSalt(maximumSaltLength As Integer) As Byte()
        Dim Salt(maximumSaltLength - 1) As Byte
        Using rng As RandomNumberGenerator = New RNGCryptoServiceProvider
            rng.GetNonZeroBytes(Salt)
        End Using
        Return Salt
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
                ' TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' TODO: große Felder auf Null setzen.
        End If
        disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
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
        ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class

