Imports System.Security.Cryptography
Imports System.IO
Imports System.Text
Imports System.Management

''' <remarks>http://icodesnippet.com/snippet/vbnet/computing-hash-values-vbnet-code-snippets</remarks>
Public Class Rijndael

    Private C_DP As DataProvider

    Public Sub New(ByVal DataProviderKlasse As DataProvider)
        C_DP = DataProviderKlasse
    End Sub

    Public Enum HashType
        MD5
        SHA1
        SHA256
        SHA384
        SHA512
    End Enum

    Private HWID As String = GetHWID()
    Private bytIV() As Byte = CreateIV(HWID)

    Public Function EncryptString128Bit(ByVal vstrTextToBeEncrypted As String, ByVal vstrEncryptionKey As String) As String
        ' Standardwert
        EncryptString128Bit = DataProvider.P_Def_ErrorMinusOne_String

        ' Test ob gültige Eingangsdaten vorhanden
        If vstrTextToBeEncrypted IsNot DataProvider.P_Def_ErrorMinusOne_String And vstrEncryptionKey IsNot DataProvider.P_Def_ErrorMinusOne_String Then

            Dim bytValue() As Byte
            Dim bytKey() As Byte
            Dim bytEncoded() As Byte = {0}
            Dim intLength As Integer
            Dim intRemaining As Integer

            vstrEncryptionKey = getMd5Hash(String.Concat(vstrEncryptionKey, HWID), Encoding.Unicode, False)

            '   **********************************************************************
            '   ******  Strip any null character from string to be encrypted    ******
            '   **********************************************************************

            vstrTextToBeEncrypted = StripNullCharacters(vstrTextToBeEncrypted)

            '   **********************************************************************
            '   ******  Value must be within ASCII range (i.e., no DBCS chars)  ******
            '   **********************************************************************

            bytValue = Encoding.ASCII.GetBytes(vstrTextToBeEncrypted.ToCharArray)

            intLength = Len(vstrEncryptionKey)

            '   ********************************************************************
            '   ******   Encryption Key must be 256 bits long (32 bytes)      ******
            '   ******   If it is longer than 32 bytes it will be truncated.  ******
            '   ******   If it is shorter than 32 bytes it will be padded     ******
            '   ******   with upper-case Xs.                                  ****** 
            '   ********************************************************************

            If intLength >= 32 Then
                vstrEncryptionKey = Strings.Left(vstrEncryptionKey, 32)
            Else
                intLength = Len(vstrEncryptionKey)
                intRemaining = 32 - intLength
                vstrEncryptionKey = vstrEncryptionKey & Strings.StrDup(intRemaining, "X")
            End If

            bytKey = Encoding.ASCII.GetBytes(vstrEncryptionKey.ToCharArray)

            '   ***********************************************************************
            '   ******  Create the encryptor and write value to it after it is   ******
            '   ******  converted into a byte array                              ******
            '   ***********************************************************************

            Using rijAlg As New RijndaelManaged()
                With rijAlg
                    .Key = bytKey
                    .IV = bytIV

                    ' Create a decrytor to perform the stream transform. 
                    Using encryptor As ICryptoTransform = rijAlg.CreateEncryptor(.Key, .IV)
                        ' Create the streams used for encryption. 
                        Using msEncrypt As New MemoryStream()
                            Using csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
                                Using swEncrypt As New StreamWriter(csEncrypt)
                                    'Write all data to the stream.
                                    swEncrypt.Write(vstrTextToBeEncrypted)
                                End Using
                                bytEncoded = msEncrypt.ToArray()
                            End Using
                        End Using
                    End Using
                End With
            End Using

            '   ***********************************************************************
            '   ******   Return encryptes value (converted from  byte Array to   ******
            '   ******   a base64 string).  Base64 is MIME encoding)             ******
            '   ***********************************************************************

            Return Convert.ToBase64String(bytEncoded)

        End If
    End Function

    Public Function DecryptString128Bit(ByVal vstrStringToBeDecrypted As String, ByVal vstrDecryptionKey As String) As String
        ' Standardwert
        DecryptString128Bit = DataProvider.P_Def_ErrorMinusOne_String

        ' Test ob gültige Eingangsdaten vorhanden
        If vstrStringToBeDecrypted IsNot DataProvider.P_Def_ErrorMinusOne_String And vstrDecryptionKey IsNot DataProvider.P_Def_ErrorMinusOne_String Then

            Dim bytDataToBeDecrypted() As Byte
            Dim bytDecryptionKey() As Byte

            Dim intLength As Integer
            Dim intRemaining As Integer

            Dim strReturnString As String = String.Empty
            Dim plaintext As String = vbNullString

            vstrDecryptionKey = getMd5Hash(String.Concat(vstrDecryptionKey, HWID), Encoding.Unicode, False)

            '   *****************************************************************
            '   ******   Convert base64 encrypted value to byte array      ******
            '   *****************************************************************

            bytDataToBeDecrypted = Convert.FromBase64String(vstrStringToBeDecrypted)

            '   ********************************************************************
            '   ******   Encryption Key must be 256 bits long (32 bytes)      ******
            '   ******   If it is longer than 32 bytes it will be truncated.  ******
            '   ******   If it is shorter than 32 bytes it will be padded     ******
            '   ******   with upper-case Xs.                                  ****** 
            '   ********************************************************************

            intLength = Len(vstrDecryptionKey)

            If intLength >= 32 Then
                vstrDecryptionKey = Strings.Left(vstrDecryptionKey, 32)
            Else
                intLength = Len(vstrDecryptionKey)
                intRemaining = 32 - intLength
                vstrDecryptionKey = vstrDecryptionKey & Strings.StrDup(intRemaining, "X")
            End If

            bytDecryptionKey = Encoding.ASCII.GetBytes(vstrDecryptionKey.ToCharArray)

            '   ***********************************************************************
            '   ******  Create the decryptor and write value to it after it is   ******
            '   ******  converted into a byte array                              ******
            '   ***********************************************************************
            Try
                Using rijAlg As New RijndaelManaged
                    With rijAlg
                        .Key = bytDecryptionKey
                        .IV = bytIV
                        ' Create a decrytor to perform the stream transform. 
                        Using decryptor = rijAlg.CreateDecryptor(.Key, .IV)
                            ' Create the streams used for decryption.
                            Using msDecrypt As New MemoryStream(bytDataToBeDecrypted)
                                Using csDecrypt As New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)
                                    Using srDecrypt As New StreamReader(csDecrypt)
                                        ' Read the decrypted bytes from the decrypting stream 
                                        ' and place them in a string.
                                        plaintext = srDecrypt.ReadToEnd()
                                    End Using
                                End Using
                            End Using
                        End Using
                    End With
                End Using
            Catch : End Try ' Die Ausnahme tritt ein, wenn die Entschlüsselung nicht möglich ist.

            '   *****************************************
            '   ******   Return decypted value     ******
            '   *****************************************

            Return StripNullCharacters(plaintext)
        End If
    End Function

    Public Function StripNullCharacters(ByVal vstrStringWithNulls As String) As String

        Dim intPosition As Integer
        Dim strStringWithOutNulls As String

        intPosition = 1
        strStringWithOutNulls = vstrStringWithNulls

        Do While intPosition > 0 And vstrStringWithNulls IsNot vbNullString
            intPosition = InStr(intPosition, vstrStringWithNulls, vbNullChar)

            If intPosition > 0 Then
                strStringWithOutNulls = Left$(strStringWithOutNulls, intPosition - 1) & _
                                  Right$(strStringWithOutNulls, Len(strStringWithOutNulls) - intPosition)
            End If

            If intPosition > strStringWithOutNulls.Length Then
                Exit Do
            End If
        Loop

        Return strStringWithOutNulls

    End Function

    Public Function GetSalt() As String
        Dim rng As RandomNumberGenerator = New RNGCryptoServiceProvider()
        Dim tokenData(16 - 1) As Byte

        rng.GetNonZeroBytes(tokenData)
        'Fehler in Office 2003
        'rng.Dispose()

        Return Convert.ToBase64String(tokenData)
    End Function

    Public Function getMd5Hash(ByVal input As String, ByVal Enkodierung As Encoding, ByVal CodePointFB As Boolean) As String 'Unicode für Fritz!Box

        Dim ZeichenArray() As Char
        Dim j As Integer
        ZeichenArray = input.ToCharArray

        If CodePointFB Then
            ' Aus Kompatibilitätsgründen muss für jedes Zeichen, dessen Unicode Codepoint > 255 ist, die Codierung des "."-Zeichens benutzt werden (0x2e 0x00 in UTF-16LE). 
            ' Dies betrit also alle Zeichen, die nicht in ISO-8859-1 dargestellt werden können, z. B. das Euro-Zeichen.

            For j = LBound(ZeichenArray) To UBound(ZeichenArray)
                If Strings.AscW(ZeichenArray(j)) > 255 Then ZeichenArray(j) = CChar(".")
            Next
        End If

        Dim md5 As MD5 = New MD5CryptoServiceProvider
        Dim data As Byte() = md5.ComputeHash(Enkodierung.GetBytes(ZeichenArray))

        Dim sBuilder As New StringBuilder()
        For Each b As Byte In data
            sBuilder.Append(b.ToString("x2"))
        Next
        Return sBuilder.ToString()

    End Function

    ''' <summary>
    ''' Erstellt einen Fingerabdruck des Rechners anhand der "ProcessorID", der "VolumeSerialnumber" der ersten Festplatte, der "Win32_BaseBoard_SerialNumber" und der Sicherheits-ID für den Benutzer
    ''' </summary>
    ''' <returns>Fingerabdruck bzw. HardwareID</returns>
    Private Function GetHWID() As String
        Dim list As New List(Of String)

        Dim Query As SelectQuery
        Dim Search As ManagementObjectSearcher

        Query = New SelectQuery("Win32_Processor")
        Search = New ManagementObjectSearcher(Query)

        For Each info As ManagementObject In Search.Get()
            list.Add(info("ProcessorId").ToString)
        Next

        Dim disk As ManagementObject

        For Each d As DriveInfo In DriveInfo.GetDrives()
            If d.DriveType = DriveType.Fixed AndAlso d.IsReady Then
                disk = New ManagementObject(String.Format("Win32_Logicaldisk='{0}'", d.Name.Substring(0, 2)))
                list.Add(disk.Properties("VolumeSerialnumber").Value.ToString)
                Exit For
            End If
        Next

        Query = New SelectQuery("Win32_BaseBoard")
        Search = New ManagementObjectSearcher(Query)

        For Each info As ManagementObject In Search.Get()
            list.Add(info("SerialNumber").ToString)
        Next

        list.Add(System.Security.Principal.WindowsIdentity.GetCurrent.User.Value)
        Query = Nothing
        Search = Nothing
        Return String.Join("-", list.ToArray())
    End Function

    ''' <summary>
    ''' Erstellt einen Initialisierungsvektor (IV) für den symmetrischen Algorithmus.
    ''' </summary>
    ''' <param name="strData">Text, aus dem der Initialisierungsvektor generiert werden soll.</param>
    ''' <returns>Initialisierungsvektor</returns>
    Private Function CreateIV(ByVal strData As String) As Byte()
        'Convert strPassword to an array and store in chrData.
        Dim chrData() As Char = strData.ToCharArray
        'Use intLength to get strPassword size.
        Dim intLength As Integer = chrData.GetUpperBound(0)
        'Declare bytDataToHash and make it the same size as chrData.
        Dim bytDataToHash(intLength) As Byte

        'Use For Next to convert and store chrData into bytDataToHash.
        For i As Integer = 0 To chrData.GetUpperBound(0)
            bytDataToHash(i) = CByte(Asc(chrData(i)))
        Next

        'Declare what hash to use.
        Dim SHA512 As New System.Security.Cryptography.SHA512Managed
        'Declare bytResult, Hash bytDataToHash and store it in bytResult.
        Dim bytResult As Byte() = SHA512.ComputeHash(bytDataToHash)
        'Declare bytIV(15).  It will hold 128 bits.
        Dim bytIV(15) As Byte

        'Use For Next to put a specific size (128 bits) of 
        'bytResult into bytIV. The 0 To 30 for bytKey used the first 256 bits.
        'of the hashed password. The 32 To 47 will put the next 128 bits into bytIV.
        For i As Integer = 32 To 47
            bytIV(i - 32) = bytResult(i)
        Next

        Return bytIV 'return the IV
    End Function

    'Public Function GetHash(ByVal original As String, ByVal hashType As HashType, ByVal UE As Encoding) As String
    '    Dim hash As String
    '    Select Case hashType
    '        Case hashType.MD5
    '            hash = getMd5Hash(original, UE, False)
    '        Case hashType.SHA1
    '            hash = GetSHA1Hash(original, UE)
    '        Case hashType.SHA256
    '            hash = GetSHA256Hash(original, UE)
    '        Case hashType.SHA384
    '            hash = GetSHA384Hash(original, UE)
    '        Case hashType.SHA512
    '            hash = GetSHA512Hash(original, UE)
    '        Case Else
    '            Throw New ArgumentOutOfRangeException("hashType", hashType, "Unsupported HashType.")
    '    End Select
    '    Return hash
    'End Function

    'Private Overloads Function GetMD5Hash(ByVal original As String, ByVal UE As Encoding) As String
    '    Dim HashValue As Byte()
    '    Dim MessageBytes As Byte() = UE.GetBytes(original)
    '    Dim md5 As MD5 = New MD5CryptoServiceProvider
    '    Dim strHex As String = ""
    '    HashValue = md5.ComputeHash(MessageBytes)
    '    For Each b As Byte In HashValue
    '        strHex &= String.Format("{0:x2}", b)
    '    Next
    '    Return strHex
    'End Function

    'Private Function GetSHA1Hash(ByVal original As String, ByVal UE As Encoding) As String
    '    Dim HashValue As Byte()
    '    Dim MessageBytes As Byte() = UE.GetBytes(original)
    '    Dim SHhash As SHA1Managed = New SHA1Managed
    '    Dim strHex As String = ""
    '    HashValue = SHhash.ComputeHash(MessageBytes)
    '    For Each b As Byte In HashValue
    '        strHex &= String.Format("{0:x2}", b)
    '    Next
    '    Return strHex
    'End Function

    'Private Function GetSHA256Hash(ByVal original As String, ByVal UE As Encoding) As String
    '    Dim HashValue As Byte()
    '    Dim MessageBytes As Byte() = UE.GetBytes(original)
    '    Dim SHhash As SHA256Managed = New SHA256Managed
    '    Dim strHex As String = ""
    '    HashValue = SHhash.ComputeHash(MessageBytes)
    '    For Each b As Byte In HashValue
    '        strHex &= String.Format("{0:x2}", b)
    '    Next
    '    Return strHex
    'End Function

    'Private Function GetSHA384Hash(ByVal original As String, ByVal UE As Encoding) As String
    '    Dim HashValue As Byte()
    '    Dim MessageBytes As Byte() = UE.GetBytes(original)
    '    Dim SHhash As SHA384Managed = New SHA384Managed
    '    Dim strHex As String = ""
    '    HashValue = SHhash.ComputeHash(MessageBytes)
    '    For Each b As Byte In HashValue
    '        strHex &= String.Format("{0:x2}", b)
    '    Next
    '    Return strHex
    'End Function

    'Private Function GetSHA512Hash(ByVal original As String, ByVal UE As Encoding) As String
    '    Dim HashValue As Byte()
    '    Dim MessageBytes As Byte() = UE.GetBytes(original)
    '    Dim SHhash As SHA512Managed = New SHA512Managed
    '    Dim strHex As String = ""
    '    HashValue = SHhash.ComputeHash(MessageBytes)
    '    For Each b As Byte In HashValue
    '        strHex &= String.Format("{0:x2}", b)
    '    Next
    '    Return strHex
    'End Function

End Class

