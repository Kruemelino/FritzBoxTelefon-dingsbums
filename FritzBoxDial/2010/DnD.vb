' --------------------------------------------------------------------------
' Licensed under MIT License.
'
' Outlook DnD Data Reader
' 
' File     : OleDataReader.cs
' Author   : Tobias Viehweger <tobias.viehweger@yasoon.com / @mnkypete>
'
' --------------------------------------------------------------------------
Imports System.IO

Namespace OutlookDndWpf
    Public Class OleDataReader
        Private stream As MemoryStream

        Public Sub SetStream(ByVal inStream As MemoryStream)
            stream = inStream
        End Sub

        ''' <summary>
        ''' Funktion zur Ermittlung der Outlook daten, die per Drag and Dropp an eine Form übergeben werden. 
        ''' 2015 Nach VB.NET portiert</summary>
        ''' <remarks>https://github.com/yasoonOfficial/outlook-dndprotocol</remarks>
        ''' <permission>Licensed under MIT License.
        ''' Outlook DnD Data Reader 
        ''' File     : MainWindow.xaml.cs
        ''' Author   : Tobias Viehweger tobias.viehweger@yasoon.com / @mnkypete</permission>
        Friend Function ReadOutlookDnDData() As MyOleOutlookData

            Dim reader As New BinaryReader(Me.stream)

            Dim sr As New StreamReader(Me.stream)
            Dim t As String = sr.ReadToEnd()

            '1. First 4 bytes are the length of the FolderId (In bytes)
            'Note: These are possibly uint? We don't expect it to be that long nevertheless..
            Dim folderIdLength As Int32 = reader.ReadInt32()

            '2. Read FolderId    
            Dim folderId As Byte() = reader.ReadBytes(folderIdLength)
            Dim folderIdHex As String = ByteArrayToString(folderId)

            '3. Next 4 bytes are the StoreId length (In bytes)
            Dim storeIdLength As Int32 = reader.ReadInt32()

            '4. Read StoreId
            Dim storeId As Byte() = reader.ReadBytes(storeIdLength)
            Dim storeIdHex As String = ByteArrayToString(storeId)

            '5. There are now some bytes which are not identified yet..
            reader.ReadBytes(4)
            reader.ReadBytes(4)
            reader.ReadBytes(4) ' <== These appear to be folder dependent somehow..

            '6. Read items count, again, we assume int instead of uint because that much items
            '   => Other problems =)
            Dim itemCount As Int32 = reader.ReadInt32()

            Dim items(itemCount) As MyOleOutlookItemData

            For i = 0 To itemCount - 1
                'First 4 bytes, represent the MAPI property 0x8014 ("SideEffects" in OlSpy)
                Dim sideEffects As Int32 = reader.ReadInt32()

                'Next byte tells us the length of the message class string (i.e. IPM.Note)
                Dim classLength As Byte = reader.ReadByte()

                'Now, read type
                Dim messageClass As String = Encoding.ASCII.GetString(reader.ReadBytes(classLength))

                'Next, read the unicode char (!) count of the subject 
                'Note: It seems that Outlook limits this to 255, cross reference mail spec sometime..
                Dim subjectLength As Byte = reader.ReadByte()

                'Read the subject, note that this is unicode, so we need to read 2 bytes per char!
                Dim subject As String = Encoding.Unicode.GetString(reader.ReadBytes(subjectLength * 2))

                'Next up: EntryID including it's length (same as for store + folder)
                Dim entryIdLength As Int32 = reader.ReadInt32()
                Dim entryId As Byte() = reader.ReadBytes(entryIdLength)
                Dim entryIdHex As String = ByteArrayToString(entryId)

                'Now the SearchKey MAPI property of the item
                Dim searchKeyLength As Int32 = reader.ReadInt32()
                Dim searchKey As Byte() = reader.ReadBytes(searchKeyLength)
                Dim searchKeyHex As String = ByteArrayToString(searchKey)


                'Some more stuff which is not quite clear, the next 4 bytes seem to be always => E0 80 E9 5A
                reader.ReadBytes(4)

                'The next 24 byte are some more flags which are not worked out yet, afterwards the next item begins
                reader.ReadBytes(24)

                items(i) = New MyOleOutlookItemData
                items(i).EntryId = entryIdHex
                items(i).MessageClass = messageClass
                items(i).SearchKey = searchKeyHex
                items(i).Subject = subject

            Next
            Dim Data As MyOleOutlookData = New MyOleOutlookData()
            Data.StoreId = storeIdHex
            Data.FolderId = folderIdHex
            Data.Items = items

            Return Data
        End Function

        Public Overloads Function ByteArrayToString(ByVal ByteArray As Byte()) As String
            Dim hex As StringBuilder = New StringBuilder(ByteArray.Length * 2)
            For Each b As Byte In ByteArray
                hex.AppendFormat("{0:x2}", b)
            Next
            Return hex.ToString()
            'Return System.Text.Encoding.UTF8.GetString(ByteArray)
        End Function
    End Class

    ' --------------------------------------------------------------------------
    ' Licensed under MIT License.
    '
    ' Outlook DnD Data Reader
    ' 
    ' File     : OleOutlookData.cs
    ' Author   : Tobias Viehweger <tobias.viehweger@yasoon.com / @mnkypete>
    '
    ' -------------------------------------------------------------------------- 

    Class MyOleOutlookItemData

        Private _Subject As String
        Friend Property Subject As String
            Get
                Return _Subject
            End Get
            Set(value As String)
                _Subject = value
            End Set
        End Property

        Private _EntryId As String
        Friend Property EntryId As String
            Get
                Return _EntryId
            End Get
            Set(value As String)
                _EntryId = value
            End Set
        End Property

        Private _SearchKey As String
        Friend Property SearchKey As String
            Get
                Return _SearchKey
            End Get
            Set(value As String)
                _SearchKey = value
            End Set
        End Property

        Private _MessageClass As String
        Friend Property MessageClass As String
            Get
                Return _MessageClass
            End Get
            Set(value As String)
                _MessageClass = value
            End Set
        End Property


    End Class

    Class MyOleOutlookData

        Private _StoreId As String
        Friend Property StoreId As String
            Get
                Return _StoreId
            End Get
            Set(value As String)
                _StoreId = value
            End Set
        End Property

        Private _FolderId As String
        Friend Property FolderId As String
            Get
                Return _FolderId
            End Get
            Set(value As String)
                _FolderId = value
            End Set
        End Property

        Private _SearchKey As MyOleOutlookItemData()
        Friend Property Items As MyOleOutlookItemData()
            Get
                Return _SearchKey
            End Get
            Set(value As MyOleOutlookItemData())
                _SearchKey = value
            End Set
        End Property
    End Class

End Namespace

