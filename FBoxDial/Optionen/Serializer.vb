Imports System.IO
Imports System.Threading.Tasks
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Xml.Xsl

Friend Module Serializer
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Sub Laden(ByRef objectData As OutlookXML)

        Dim DateiInfo As FileInfo
        Dim Pfad As String

        Pfad = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, DfltConfigFileName)

        DateiInfo = New FileInfo(Pfad)
        DateiInfo.Directory.Create() ' If the directory already exists, this method does nothing.

        If Not (File.Exists(Pfad) AndAlso DeserializeObject(Pfad, objectData)) Then
            objectData = New OutlookXML
        End If

        ' Setze einige Felder
        If objectData IsNot Nothing Then
            With objectData.POptionen
                ' .Arbeitsverzeichnis = DateiInfo.Directory.ToString
                .ValidFBAdr = ValidIP(.TBFBAdr)

            End With
        End If

    End Sub

    Friend Sub Speichern(Of T)(objectData As T, Pfad As String)
        If objectData IsNot Nothing Then
            Dim XmlSerializerNamespace As New XmlSerializerNamespaces()
            XmlSerializerNamespace.Add(DfltStringEmpty, DfltStringEmpty)

            Using XmlSchreiber As XmlWriter = XmlWriter.Create(Pfad, New XmlWriterSettings With {.Indent = True, .OmitXmlDeclaration = False})
                With New XmlSerializer(GetType(T))
                    .Serialize(XmlSchreiber, objectData, XmlSerializerNamespace)
                End With
            End Using
        End If
    End Sub

    ''' <summary>
    ''' Überprüft, ob die einzulesenden Daten überhaupt eine XML sind.
    ''' </summary>
    ''' <param name="InputData">Die einzulesenden Daten</param>
    ''' <param name="IsPfad">Angabe, ob ein Dateipfad oder XML-Daten geprüft werden sollen.</param>
    ''' <returns>Boolean</returns>
    Private Function CheckXMLData(InputData As String, IsPfad As Boolean) As Boolean
        Dim xDoc As New XmlDocument
        Try
            ' Versuche die Datei zu laden, wenn es keine Exception gibt, ist alles ok
            If IsPfad Then
                xDoc.Load(InputData)
            Else
                xDoc.LoadXml(InputData)
            End If

            Return True

        Catch ex As XmlException
            NLogger.Fatal(ex, $"Die XML-Datan weist einen Lade- oder Analysefehler auf: '{InputData}'")

            Return False

        Catch ex As FileNotFoundException
            NLogger.Fatal(ex, $"Die XML-Datan kann nicht gefunden werden: '{InputData}'")

            Return False

        End Try
    End Function
    ''' <summary>
    ''' Deserialisiert die XML-Datei, die unter <paramref name="UniformResourceIdentifier"/> gespeichert ist.
    ''' </summary>
    ''' <typeparam name="T">Zieltdatentyp</typeparam>
    ''' <param name="UniformResourceIdentifier">URI der XML-Datei.</param>
    ''' <param name="ReturnObj"></param>
    ''' <returns>True oder False, je nach Ergebnis der Deserialisierung</returns>
    Friend Function DeserializeObject(Of T)(UniformResourceIdentifier As Uri, ByRef ReturnObj As T) As Boolean
        Return DeserializeObject(UniformResourceIdentifier.AbsoluteUri, ReturnObj)
    End Function

    ''' <summary>
    ''' Deserialisiert die XML-Datei mittels <see cref="Task"/>, die unter <paramref name="Pfad"/> gespeichert ist.
    ''' </summary>
    ''' <typeparam name="T">Zieltdatentyp</typeparam>
    ''' <param name="Pfad">Speicherort</param>
    ''' <param name="xslt">XSLT-Transformation</param>
    ''' <returns>True oder False, je nach Ergebnis der Deserialisierung</returns>
    Friend Function DeserializeObjectAsyc(Of T)(Pfad As String, xslt As XslCompiledTransform) As Task(Of T)
        Return Task.Run(Function()
                            Dim ReturnObj As T
                            If DeserializeObject(Pfad, xslt, ReturnObj) Then
                                Return ReturnObj
                            Else
                                Return Nothing
                            End If
                        End Function)
    End Function
    ''' <summary>
    ''' Deserialisiert die XML-Datei mittels <see cref="Task"/>, die unter <paramref name="Pfad"/> gespeichert ist.
    ''' </summary>
    ''' <typeparam name="T">Zieltdatentyp</typeparam>
    ''' <param name="Pfad">Speicherort</param>
    ''' <returns>True oder False, je nach Ergebnis der Deserialisierung</returns>
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
    ''' Deserialisiert die XML-Datei, die unter <paramref name="Pfad"/> gespeichert ist.
    ''' </summary>
    ''' <typeparam name="T">Zieltdatentyp</typeparam>
    ''' <param name="Pfad">Speicherort</param>
    ''' <param name="ReturnObj">Deserialisiertes Datenobjekt vom Type <typeparamref name="T"/>.</param>
    ''' <returns>True oder False, je nach Ergebnis der Deserialisierung</returns>
    Private Function DeserializeObject(Of T)(Pfad As String, ByRef ReturnObj As T) As Boolean

        If CheckXMLData(Pfad, True) Then
            Dim Serializer As New XmlSerializer(GetType(T))

            ' Erstelle einen XMLReader zum einlesen der XML-Datei
            Using Reader As XmlReader = XmlReader.Create(Pfad)

                ' Deserialisiere das transformierte XML-Objekt
                Return DeserializeObject(Reader, ReturnObj)

            End Using
        Else
            NLogger.Fatal($"Fehler beim Deserialisieren: {Pfad} kann nicht deserialisert werden.")
            Return False
        End If

    End Function

    ''' <summary>
    ''' Deserialisiert die XML-Datei, die unter <paramref name="Pfad"/> gespeichert ist. Führt zusätzlich eine XSLT-Transformation durch.
    ''' </summary>
    ''' <typeparam name="T">Zieltdatentyp</typeparam>
    ''' <param name="Pfad">Speicherort</param>
    ''' <param name="xslt">XSLT-Transformation</param>
    ''' <param name="ReturnObj">Deserialisiertes Datenobjekt vom Type <typeparamref name="T"/>.</param>
    ''' <returns>True oder False, je nach Ergebnis der Deserialisierung</returns>
    Private Function DeserializeObject(Of T)(Pfad As String, xslt As XslCompiledTransform, ByRef ReturnObj As T) As Boolean

        If xslt IsNot Nothing AndAlso CheckXMLData(Pfad, True) Then
            Dim Serializer As New XmlSerializer(GetType(T))

            Dim TransformationOutput As StringBuilder = New StringBuilder()
            Dim writerSettings As XmlWriterSettings = New XmlWriterSettings With {.OmitXmlDeclaration = True}

            ' Erstelle einen XMLReader
            Using Reader As XmlReader = XmlReader.Create(Pfad)

                ' Erstelle einen XMLWriter
                Using transformedData As XmlWriter = XmlWriter.Create(TransformationOutput, writerSettings)
                    ' Transformiere das XML-Objekt
                    xslt.Transform(Reader, transformedData)

                    ' Lies das transformierte XML-Objekt ein
                    Using ReaderTransformed As XmlReader = XmlReader.Create(New StringReader(TransformationOutput.ToString()))

                        ' Deserialisiere das transformierte XML-Objekt
                        Return DeserializeObject(ReaderTransformed, ReturnObj)

                    End Using

                End Using

            End Using
        Else
            NLogger.Fatal($"Fehler beim Deserialisieren: {Pfad} kann nicht deserialisert werden.")
            Return False
        End If
    End Function

    ''' <summary>
    ''' Deserialisiert den übergebenen <paramref name="Reader"/> (<see cref="XmlReader"/>).
    ''' </summary>
    ''' <typeparam name="T">Typ des deserialsierten Objektes.</typeparam>
    ''' <param name="Reader">Der <see cref="XmlReader"/>.</param>
    ''' <param name="ReturnObj">Deserialisiertes Datenobjekt vom Type <typeparamref name="T"/>.</param>
    ''' <returns>True oder False, je nach Ergebnis der Deserialisierung</returns>
    Private Function DeserializeObject(Of T)(Reader As XmlReader, ByRef ReturnObj As T) As Boolean

        Dim Serializer As New XmlSerializer(GetType(T))

        If Serializer.CanDeserialize(Reader) Then
            Try
                ReturnObj = CType(Serializer.Deserialize(Reader, New XmlDeserializationEvents With {.OnUnknownAttribute = AddressOf On_UnknownAttribute,
                                                                                                    .OnUnknownElement = AddressOf On_UnknownElement,
                                                                                                    .OnUnknownNode = AddressOf On_UnknownNode,
                                                                                                    .OnUnreferencedObject = AddressOf On_UnreferencedObject}), T)

                Return True

            Catch ex As InvalidOperationException

                NLogger.Fatal(ex, $"Bei der Deserialisierung ist ein Fehler aufgetreten.")
                Return False
            End Try
        Else
            NLogger.Fatal($"Fehler beim Deserialisieren.")
            Return False
        End If

    End Function

    Private Sub On_UnknownAttribute(sender As Object, e As XmlAttributeEventArgs)
        NLogger.Warn($"Unknown Attribute: {e.Attr.Name} in {e.ObjectBeingDeserialized}")
    End Sub

    Private Sub On_UnknownElement(sender As Object, e As XmlElementEventArgs)
        NLogger.Warn($"Unknown Element: {e.Element.Name} in {e.ObjectBeingDeserialized}")
    End Sub

    Private Sub On_UnknownNode(sender As Object, e As XmlNodeEventArgs)
        NLogger.Warn($"Unknown Node: {e.Name} in {e.ObjectBeingDeserialized}")
    End Sub

    Private Sub On_UnreferencedObject(sender As Object, e As UnreferencedObjectEventArgs)
        NLogger.Warn($"Unreferenced Object: {e.UnreferencedId}")
    End Sub

    Friend Function XmlDeserializeFromString(Of T)(objectData As String, ByRef result As T) As Boolean

        If CheckXMLData(objectData, False) Then

            Dim Serializer = New XmlSerializer(GetType(T))
            Using Reader As New StringReader(objectData)
                Try
                    result = CType(Serializer.Deserialize(Reader), T)

                    Return True
                Catch ex As InvalidOperationException
                    NLogger.Fatal(ex, $"Fehler beim Deserialisieren von {GetType(T).FullName}: {objectData}")

                    ' Gib Nothing zurück
                    result = Nothing
                    Return False
                End Try

            End Using
        Else
            Return False
        End If

    End Function

    Friend Function XmlSerializeToString(Of T)(objectData As T, ByRef result As String) As Boolean

        If objectData IsNot Nothing Then
            Dim XmlSerializerNamespace As New XmlSerializerNamespaces()
            XmlSerializerNamespace.Add(DfltStringEmpty, DfltStringEmpty)

            Using XmlSchreiber As New Utf8StringWriter

                With New XmlSerializer(GetType(T))
                    Try
                        .Serialize(XmlSchreiber, objectData, XmlSerializerNamespace)
                        result = XmlSchreiber.ToString

                        Return True
                    Catch ex As InvalidOperationException
                        NLogger.Fatal(ex, $"Fehler beim Serialisieren von {GetType(T).FullName}: {objectData}")

                        Return False
                    End Try

                End With
            End Using
        End If

        Return False
    End Function

    ''' <summary>
    ''' Erzeugt einen Klone des übergebenen Objektes mittels XML Serialisierung und anschließender Deserialisierung.
    ''' </summary>
    ''' <typeparam name="T">Typ des Objektes.</typeparam>
    ''' <param name="Objekt">Das zu klonende Objekt</param>
    ''' <returns>Den Klon.</returns>
    Friend Function XMLClone(Of T)(Objekt As T) As T
        Dim tmp As String = DfltStringEmpty

        If Objekt IsNot Nothing Then
            If Not XmlSerializeToString(Objekt, tmp) OrElse Not XmlDeserializeFromString(tmp, XMLClone) Then
                NLogger.Warn($"Fehler beim Klonen eines Objektes ({Objekt.GetType.Name}):  '{tmp}'")
            End If
        End If
    End Function

End Module
