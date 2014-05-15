Imports System.Security.Cryptography
Imports System.IO
Imports System.Text
Imports System.Management

Public Class MyRijndael

    Private HWID As String = GetHWID()
    Private bytIV() As Byte = CreateIV(HWID)

    'Public Function EncryptString128Bit(ByVal vstrTextToBeEncrypted As String, ByVal vstrEncryptionKey As String) As String

    '    vstrEncryptionKey = getMd5Hash(String.Concat(vstrEncryptionKey, HWID), Encoding.Unicode)

    '    Dim bytValue() As Byte
    '    Dim bytKey() As Byte
    '    Dim bytEncoded() As Byte = {0}
    '    ' Dim bytIV() As Byte = {121, 241, 10, 1, 132, 74, 11, 39, 255, 91, 45, 78, 14, 211, 22, 62}
    '    Dim intLength As Integer
    '    Dim intRemaining As Integer
    '    Dim objMemoryStream As New MemoryStream()
    '    Dim objCryptoStream As CryptoStream
    '    Dim objRijndaelManaged As RijndaelManaged


    '    '   **********************************************************************
    '    '   ******  Strip any null character from string to be encrypted    ******
    '    '   **********************************************************************

    '    vstrTextToBeEncrypted = StripNullCharacters(vstrTextToBeEncrypted)

    '    '   **********************************************************************
    '    '   ******  Value must be within ASCII range (i.e., no DBCS chars)  ******
    '    '   **********************************************************************

    '    bytValue = Encoding.ASCII.GetBytes(vstrTextToBeEncrypted.ToCharArray)

    '    intLength = Len(vstrEncryptionKey)

    '    '   ********************************************************************
    '    '   ******   Encryption Key must be 256 bits long (32 bytes)      ******
    '    '   ******   If it is longer than 32 bytes it will be truncated.  ******
    '    '   ******   If it is shorter than 32 bytes it will be padded     ******
    '    '   ******   with upper-case Xs.                                  ****** 
    '    '   ********************************************************************

    '    If intLength >= 32 Then
    '        vstrEncryptionKey = Strings.Left(vstrEncryptionKey, 32)
    '    Else
    '        intLength = Len(vstrEncryptionKey)
    '        intRemaining = 32 - intLength
    '        vstrEncryptionKey = vstrEncryptionKey & Strings.StrDup(intRemaining, "X")
    '    End If

    '    bytKey = Encoding.ASCII.GetBytes(vstrEncryptionKey.ToCharArray)

    '    objRijndaelManaged = New RijndaelManaged()

    '    '   ***********************************************************************
    '    '   ******  Create the encryptor and write value to it after it is   ******
    '    '   ******  converted into a byte array                              ******
    '    '   ***********************************************************************

    '    Try
    '        objCryptoStream = New CryptoStream(objMemoryStream, objRijndaelManaged.CreateEncryptor(bytKey, bytIV), CryptoStreamMode.Write)
    '        objCryptoStream.Write(bytValue, 0, bytValue.Length)

    '        objCryptoStream.FlushFinalBlock()

    '        bytEncoded = objMemoryStream.ToArray

    '        objMemoryStream.Close()
    '        objCryptoStream.Close()

    '    Catch : End Try

    '    '   ***********************************************************************
    '    '   ******   Return encryptes value (converted from  byte Array to   ******
    '    '   ******   a base64 string).  Base64 is MIME encoding)             ******
    '    '   ***********************************************************************

    '    Return Convert.ToBase64String(bytEncoded)

    'End Function

    'Public Function DecryptString128Bit(ByVal vstrStringToBeDecrypted As String, ByVal vstrDecryptionKey As String) As String
    '    vstrDecryptionKey = getMd5Hash(String.Concat(vstrDecryptionKey, HWID), Encoding.Unicode)

    '    Dim bytDataToBeDecrypted() As Byte
    '    Dim bytTemp() As Byte
    '    ' Dim bytIV() As Byte = {121, 241, 10, 1, 132, 74, 11, 39, 255, 91, 45, 78, 14, 211, 22, 62}
    '    Dim objRijndaelManaged As New RijndaelManaged()
    '    Dim objMemoryStream As MemoryStream
    '    Dim objCryptoStream As CryptoStream
    '    Dim bytDecryptionKey() As Byte

    '    Dim intLength As Integer
    '    Dim intRemaining As Integer
    '    Dim strReturnString As String = String.Empty


    '    '   *****************************************************************
    '    '   ******   Convert base64 encrypted value to byte array      ******
    '    '   *****************************************************************

    '    bytDataToBeDecrypted = Convert.FromBase64String(vstrStringToBeDecrypted)

    '    '   ********************************************************************
    '    '   ******   Encryption Key must be 256 bits long (32 bytes)      ******
    '    '   ******   If it is longer than 32 bytes it will be truncated.  ******
    '    '   ******   If it is shorter than 32 bytes it will be padded     ******
    '    '   ******   with upper-case Xs.                                  ****** 
    '    '   ********************************************************************

    '    intLength = Len(vstrDecryptionKey)

    '    If intLength >= 32 Then
    '        vstrDecryptionKey = Strings.Left(vstrDecryptionKey, 32)
    '    Else
    '        intLength = Len(vstrDecryptionKey)
    '        intRemaining = 32 - intLength
    '        vstrDecryptionKey = vstrDecryptionKey & Strings.StrDup(intRemaining, "X")
    '    End If

    '    bytDecryptionKey = Encoding.ASCII.GetBytes(vstrDecryptionKey.ToCharArray)

    '    ReDim bytTemp(bytDataToBeDecrypted.Length)

    '    objMemoryStream = New MemoryStream(bytDataToBeDecrypted)

    '    '   ***********************************************************************
    '    '   ******  Create the decryptor and write value to it after it is   ******
    '    '   ******  converted into a byte array                              ******
    '    '   ***********************************************************************

    '    Try

    '        objCryptoStream = New CryptoStream(objMemoryStream, objRijndaelManaged.CreateDecryptor(bytDecryptionKey, bytIV), CryptoStreamMode.Read)

    '        objCryptoStream.Read(bytTemp, 0, bytTemp.Length)

    '        objCryptoStream.FlushFinalBlock()

    '        objMemoryStream.Close()

    '        objCryptoStream.Close()

    '    Catch : End Try

    '    '   *****************************************
    '    '   ******   Return decypted value     ******
    '    '   *****************************************

    '    Return StripNullCharacters(Encoding.ASCII.GetString(bytTemp))

    'End Function


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="plainText"></param>
    ''' <param name="Key"></param>
    ''' <returns></returns>
    ''' <remarks>http://msdn.microsoft.com/de-de/library/system.security.cryptography.rijndael(v=vs.100).aspx</remarks>
    Public Function EncryptString(ByVal plainText As String, ByVal Key As String) As String
        ' Check arguments.
        If plainText Is Nothing OrElse plainText.Length <= 0 Then
            Throw New ArgumentNullException("plainText")
        End If
        If Key Is Nothing OrElse Key.Length <= 0 Then
            Throw New ArgumentNullException("Key")
        End If

        Dim encrypted() As Byte

        ' Code from old Function EncryptString128Bit
        Dim intLength As Integer
        Dim bytKey() As Byte

        intLength = Len(Key)

        '   Encryption Key must be 256 bits long (32 bytes) If it is longer than 32 bytes it will be truncated.
        '   If it is shorter than 32 bytes it will be padded with upper-case Xs.

        If intLength >= 32 Then
            Key = Strings.Left(Key, 32)
        Else
            intLength = Len(Key)
            Key = Key & Strings.StrDup(32 - intLength, "X")
        End If

        bytKey = Encoding.ASCII.GetBytes(Key.ToCharArray)

        ' Create an Rijndael object with the specified key and IV.
        Using rijAlg = Rijndael.Create()
            rijAlg.Key = bytKey
            rijAlg.IV = bytIV

            ' Create a decrytor to perform the stream transform.
            Dim encryptor As ICryptoTransform = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV)
            ' Create the streams used for encryption.
            Using msEncrypt As New MemoryStream()
                Using csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
                    Using swEncrypt As New StreamWriter(csEncrypt)

                        'Write all data to the stream.
                        swEncrypt.Write(plainText)
                    End Using
                    encrypted = msEncrypt.ToArray()
                End Using
            End Using
        End Using

        ' Return the encrypted string from the memory stream.
        Return Convert.ToBase64String(encrypted)

    End Function 'EncryptStringToBytes

    Public Function DecryptString(ByVal cipherText As String, ByVal Key As String) As String
        ' Check arguments.
        If cipherText Is Nothing OrElse cipherText.Length <= 0 Then
            Throw New ArgumentNullException("cipherText")
        End If
        If Key Is Nothing OrElse Key.Length <= 0 Then
            Throw New ArgumentNullException("Key")
        End If
        ' Declare the string used to hold
        ' the decrypted text.
        Dim plaintext As String = Nothing

        ' Code from old Function DecryptString128Bit
        Dim bytDataToBeDecrypted() As Byte
        Dim intLength As Integer
        Dim bytKey() As Byte

        intLength = Len(Key)

        '   Encryption Key must be 256 bits long (32 bytes) If it is longer than 32 bytes it will be truncated.
        '   If it is shorter than 32 bytes it will be padded with upper-case Xs.

        If intLength >= 32 Then
            Key = Strings.Left(Key, 32)
        Else
            intLength = Len(Key)
            Key = Key & Strings.StrDup(32 - intLength, "X")
        End If

        bytKey = Encoding.ASCII.GetBytes(Key.ToCharArray)
        bytDataToBeDecrypted = Convert.FromBase64String(cipherText)

        ' Create an Rijndael object
        ' with the specified key and IV.
        Using rijAlg = Rijndael.Create()
            rijAlg.Key = bytKey
            rijAlg.IV = bytIV

            ' Create a decrytor to perform the stream transform.
            Dim decryptor As ICryptoTransform = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV)

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

        Return plaintext

    End Function 'DecryptStringFromBytes 


    Public Function StripNullCharacters(ByVal vstrStringWithNulls As String) As String

        Dim intPosition As Integer
        Dim strStringWithOutNulls As String

        intPosition = 1
        strStringWithOutNulls = vstrStringWithNulls

        Do While intPosition > 0
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

    Function getMd5Hash(ByVal input As String, ByVal Enkodierung As Encoding) As String 'Unicode für Fritz!Box
        Dim ZeichenArray() As Char
        Dim j As Integer
        ZeichenArray = input.ToCharArray
        For j = LBound(ZeichenArray) To UBound(ZeichenArray)
            If Strings.AscW(ZeichenArray(j)) > 255 Then ZeichenArray(j) = CChar(".")
        Next

        Dim md5Hasher As MD5 = MD5.Create()
        Dim data As Byte() = md5Hasher.ComputeHash(Enkodierung.GetBytes(input))
        Dim sBuilder As New StringBuilder()

        Dim i As Integer
        For i = 0 To data.Length - 1
            sBuilder.Append(data(i).ToString("x2"))
        Next i
        Return sBuilder.ToString()

    End Function

    ''' <summary>
    ''' Erstellt einen Fingerabdruck des Rechners anhand der "ProcessorID", der "VolumeSerialnumber" der ersten Festplatte, der "Win32_BaseBoard_SerialNumber" und der Sicherheits-ID für den Benutzer
    ''' </summary>
    ''' <returns>Fingerabdruck bzw. HardwareID</returns>
    ''' <remarks></remarks>
    Private Function GetHWID() As String
        Dim list As New List(Of String)


        Dim Query As SelectQuery
        Dim Search As ManagementObjectSearcher

        Query = New SelectQuery("Win32_processor")
        Search = New ManagementObjectSearcher(Query)

        For Each info As ManagementObject In Search.Get()
            list.Add(info("processorId").ToString)
        Next

        Dim disk As ManagementObject

        For Each d As DriveInfo In DriveInfo.GetDrives()
            If d.DriveType = DriveType.Fixed AndAlso d.IsReady Then
                disk = New ManagementObject(String.Format("Win32_Logicaldisk='{0}'", d.Name.Substring(0, 2)))
                list.Add(disk.Properties("VolumeSerialnumber").Value.ToString)
                Exit For
            End If
        Next

        'disk = New ManagementObject(String.Format("Win32_Logicaldisk='{0}'", DriveInfo.GetDrives.First.Name.Substring(0, 2)))
        'list.Add(disk.Properties("VolumeSerialnumber").Value.ToString)

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
    ''' <remarks></remarks>
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
End Class

