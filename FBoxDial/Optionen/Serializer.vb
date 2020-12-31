Imports System.IO
Imports System.Threading.Tasks
Imports System.Xml
Imports System.Xml.Serialization

Friend Module Serializer
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Sub Laden(ByRef XMLData As OutlookXML)

        Dim DateiInfo As FileInfo
        Dim Pfad As String

        Pfad = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, DfltConfigFileName)

        DateiInfo = New FileInfo(Pfad)
        DateiInfo.Directory.Create() ' If the directory already exists, this method does nothing.

        If File.Exists(Pfad) Then
            XMLData = DeserializeObject(Of OutlookXML)(Pfad)
        Else
            XMLData = New OutlookXML
        End If

        ' Setze einige Felder
        If XMLData IsNot Nothing Then
            With XMLData
                With .POptionen
                    .Arbeitsverzeichnis = DateiInfo.Directory.ToString
                    .ValidFBAdr = ValidIP(.TBFBAdr)
                End With
            End With
        End If

    End Sub

    Friend Sub Speichern(Of T)(ByVal XMLData As T, ByVal Pfad As String)
        If XMLData IsNot Nothing Then
            Dim XmlSerializerNamespace As New XmlSerializerNamespaces()
            XmlSerializerNamespace.Add(DfltStringEmpty, DfltStringEmpty)

            Using XmlSchreiber As XmlWriter = XmlWriter.Create(Pfad, New XmlWriterSettings With {.Indent = True, .OmitXmlDeclaration = False})
                With New XmlSerializer(GetType(T))
                    .Serialize(XmlSchreiber, XMLData, XmlSerializerNamespace)
                End With
            End Using
        End If
    End Sub

    Friend Function DeserializeObjectAsyc(Of T)(ByVal Pfad As String) As Task(Of T)
        Return Task.Run(Function()
                            Return DeserializeObject(Of T)(Pfad)
                        End Function)
    End Function

    Friend Function DeserializeObject(Of T)(ByVal Pfad As String) As T

        Dim mySerializer As New XmlSerializer(GetType(T))
        Using XmlLeser As XmlReader = XmlReader.Create(Pfad)
            If mySerializer.CanDeserialize(XmlLeser) Then
                Try
                    Return CType(mySerializer.Deserialize(XmlLeser), T)
                Catch ex As InvalidOperationException
                    NLogger.Fatal(ex)
                End Try
            End If
        End Using

    End Function

    Friend Function XmlDeserializeFromString(Of T)(ByVal objectData As String) As T
        Return CType(XmlDeserializeFromString(objectData, GetType(T)), T)
    End Function

    Private Function XmlDeserializeFromString(ByVal objectData As String, ByVal type As Type) As Object
        Dim serializer = New XmlSerializer(type)
        Dim result As Object

        Using reader As TextReader = New StringReader(objectData)
            result = serializer.Deserialize(reader)
        End Using

        Return result
    End Function
End Module
