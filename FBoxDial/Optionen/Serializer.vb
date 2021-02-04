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

        If Not (File.Exists(Pfad) AndAlso DeserializeObject(Pfad, XMLData)) Then
            XMLData = New OutlookXML
        End If

        ' Setze einige Felder
        If XMLData IsNot Nothing Then
            With XMLData.POptionen
                ' .Arbeitsverzeichnis = DateiInfo.Directory.ToString
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
                            Dim ReturnObj As T
                            If DeserializeObject(Pfad, ReturnObj) Then
                                Return ReturnObj
                            Else
                                Return Nothing
                            End If
                        End Function)
    End Function

    ''' <summary>
    ''' Deserialisiert die XML-Datei, die unter <paramref name="Pfad"/> gesperichert ist.
    ''' </summary>
    ''' <typeparam name="T">Zieltdatentyp</typeparam>
    ''' <param name="Pfad">Dateispeicherort</param>
    ''' <param name="ReturnObj">Deserialisiertes Datenobjekt vom Type <typeparamref name="T"/>.</param>
    ''' <returns>True oder False, je nach Ergebnis</returns>
    Private Function DeserializeObject(Of T)(Pfad As String, ByRef ReturnObj As T) As Boolean

        Dim Serializer As New XmlSerializer(GetType(T))
        Using Reader As XmlReader = XmlReader.Create(Pfad)
            If Serializer.CanDeserialize(Reader) Then

                Try
                    ReturnObj = CType(Serializer.Deserialize(Reader), T)

                    Return True

                Catch ex As InvalidOperationException

                    NLogger.Fatal($"Fehler beim Deserialisieren: {Pfad}", ex)

                    Return False
                End Try
            Else
                NLogger.Fatal($"Fehler beim Deserialisieren: {Pfad} kann nicht deserialisert werden.")
                Return False
            End If
        End Using

    End Function

    Friend Function DeserializeObject(Of T)(UniformResourceIdentifier As Uri, ByRef ReturnObj As T) As Boolean
        Return DeserializeObject(UniformResourceIdentifier.AbsoluteUri, ReturnObj)
    End Function

    Friend Function XmlDeserializeFromString(Of T)(objectData As String, ByRef result As T) As Boolean

        Dim Serializer = New XmlSerializer(GetType(T))
        Using Reader As TextReader = New StringReader(objectData)
            Try
                result = CType(Serializer.Deserialize(Reader), T)

                Return True
            Catch ex As InvalidOperationException
                NLogger.Fatal($"Fehler beim Deserialisieren von {GetType(T).FullName}: {objectData}", ex)

                ' Gib Nothing zurück
                result = Nothing
                Return False
            End Try

        End Using

    End Function

End Module
