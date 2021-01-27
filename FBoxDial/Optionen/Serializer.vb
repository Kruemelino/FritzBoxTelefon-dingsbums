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
            With XMLData.POptionen
                .Arbeitsverzeichnis = DateiInfo.Directory.ToString
                .ValidFBAdr = ValidIP(.TBFBAdr)
            End With
        End If

    End Sub

    Friend Sub Speichern(Of T)(XMLData As T, Pfad As String)
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

    Friend Function DeserializeObjectAsyc(Of T)(Pfad As String) As Task(Of T)
        Return Task.Run(Function()
                            Return DeserializeObject(Of T)(Pfad)
                        End Function)
    End Function

    Friend Function DeserializeObjectAsyc(Of T)(UniformResourceIdentifier As Uri) As Task(Of T)
        Return Task.Run(Function()
                            Return DeserializeObject(Of T)(UniformResourceIdentifier.AbsoluteUri)
                        End Function)
    End Function

    Private Function DeserializeObject(Of T)(Pfad As String) As T

        Dim mySerializer As New XmlSerializer(GetType(T))
        Using XmlLeser As XmlReader = XmlReader.Create(Pfad)
            If mySerializer.CanDeserialize(XmlLeser) Then
                Try
                    Return CType(mySerializer.Deserialize(XmlLeser), T)
                Catch ex As InvalidOperationException
                    NLogger.Fatal($"Fehler beim Deserialisieren: {Pfad}", ex)
                End Try
            End If
        End Using

    End Function

    Friend Function DeserializeObject(Of T)(UniformResourceIdentifier As Uri) As T
        Return DeserializeObject(Of T)(UniformResourceIdentifier.AbsoluteUri)
    End Function

    Friend Function XmlDeserializeFromString(Of T)(objectData As String) As T
        Dim O As Object = Nothing
        If XmlDeserializeFromString(objectData, GetType(T), O) Then
            Return CType(O, T)
        Else
            Return Nothing
        End If
    End Function

    Private Function XmlDeserializeFromString(objectData As String, T As Type, ByRef result As Object) As Boolean
        Dim serializer = New XmlSerializer(T)
        'Dim result As Object

        Using reader As TextReader = New StringReader(objectData)
            Try
                result = serializer.Deserialize(reader)

                Return True
            Catch ex As InvalidOperationException
                NLogger.Fatal($"Fehler beim Deserialisieren von {T.FullName}: {objectData}", ex)

                ' Gib Nothing zurück
                result = Nothing
                Return False
            End Try

        End Using

    End Function
End Module
