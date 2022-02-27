Imports System.IO
Imports System.Threading.Tasks
Imports System.Xml
Imports System.Xml.Serialization
Imports Newtonsoft.Json

Friend Module Serializer
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "XML"
#Region "CheckXMLData"
    ''' <summary>
    ''' Überprüft, ob die einzulesenden Daten überhaupt eine XML sind. Dazu wird versucht die XML Daten einzulesen. 
    ''' Wenn die Daten eingelesen werden können, werden sie als <see cref="XmlDocument"/> zur weiteren Verarbeitung in <paramref name="xDoc"/> bereitgestellt.
    ''' </summary>
    ''' <param name="InputData">Die einzulesenden Daten</param>
    ''' <param name="IsPfad">Angabe, ob ein Dateipfad oder XML-Daten geprüft werden sollen.</param>
    ''' <param name="xDoc">XML-Daten zur weiteren Verwendung</param>
    ''' <returns>Boolean</returns>
    Private Function CheckXMLData(InputData As String, IsPfad As Boolean, ByRef xDoc As XmlDocument) As Boolean

        If InputData.IsNotStringNothingOrEmpty Then
            Try
                ' Versuche die Datei zu laden, wenn es keine Exception gibt, ist alles ok

                With xDoc
                    ' Verhindere, dass etwaige HTML-Seiten validiert werden. Hier friert der Prozess ein.
                    .XmlResolver = Nothing

                    If IsPfad Then
                        .Load(InputData)
                    Else
                        .LoadXml(InputData)
                    End If
                End With

                Return True

            Catch ex As XmlException
                NLogger.Fatal(ex, $"Die XML-Datan weist einen Lade- oder Analysefehler auf: '{InputData}'")

                Return False

            Catch ex As FileNotFoundException
                NLogger.Fatal(ex, $"Die XML-Datan kann nicht gefunden werden: '{InputData}'")

                Return False

            End Try
        Else
            NLogger.Fatal("Die übergebenen XML-Datan sind leer.")

            Return False
        End If
    End Function

#End Region

#Region "XML Deserialisieren"
#Region "Synchron"
    ''' <summary>
    ''' Deserialisiert die XML-Datei, die unter <paramref name="Data"/> gespeichert ist.
    ''' </summary>
    ''' <typeparam name="T">Zieltdatentyp</typeparam>
    ''' <param name="Data">Speicherort</param>
    ''' <param name="IsPath">Angabe, ob es sich um einen Pfad handelt.</param>
    ''' <param name="ReturnObj">Deserialisiertes Datenobjekt vom Type <typeparamref name="T"/>.</param>
    ''' <returns>True oder False, je nach Ergebnis der Deserialisierung</returns>
    Friend Function DeserializeXML(Of T)(Data As String, IsPath As Boolean, ByRef ReturnObj As T) As Boolean

        Dim xDoc As New XmlDocument
        If CheckXMLData(Data, IsPath, xDoc) Then

            ' Erstelle einen XMLReader zum Deserialisieren des XML-Documentes
            Using Reader As New XmlNodeReader(xDoc)

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

            End Using

        Else
            NLogger.Fatal($"Fehler beim Deserialisieren: {Data} kann nicht deserialisert werden.")
            Return False

        End If
        xDoc = Nothing
    End Function

#End Region

#Region "Asynchron"
    ''' <summary>
    ''' Deserialisiert die XML-Datei mittels <see cref="Task"/>, die unter <paramref name="Data"/> gespeichert ist.
    ''' </summary>
    ''' <typeparam name="T">Zieltdatentyp</typeparam>
    ''' <param name="Data">Speicherort</param>
    ''' <param name="IsPath">Angabe, ob es sich um einen Pfad handelt.</param>
    ''' <returns>Das Ergebnis des Deserialisierungsvorganges.</returns>
    Friend Function DeserializeAsyncXML(Of T)(Data As String, IsPath As Boolean) As Task(Of T)
        Return Task.Run(Function()
                            Dim ReturnObj As T
                            Return If(DeserializeXML(Data, IsPath, ReturnObj), ReturnObj, Nothing)
                        End Function)
    End Function
#End Region

#Region "XmlDeserializationEvents"
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
#End Region
#End Region

#Region "XML Serialisieren"

    Friend Function XmlSerializeToString(Of T)(objectData As T, ByRef result As String) As Boolean

        If objectData IsNot Nothing Then
            Dim XmlSerializerNamespace As New XmlSerializerNamespaces()
            XmlSerializerNamespace.Add(String.Empty, String.Empty)

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

    Friend Function XmlSerializeToFile(Of T)(objectData As T, Pfad As String) As Boolean
        If objectData IsNot Nothing Then
            Dim XmlSerializerNamespace As New XmlSerializerNamespaces()
            XmlSerializerNamespace.Add(String.Empty, String.Empty)

            Using XmlSchreiber As XmlWriter = XmlWriter.Create(Pfad, New XmlWriterSettings With {.Indent = True, .OmitXmlDeclaration = False})
                With New XmlSerializer(GetType(T))
                    Try
                        .Serialize(XmlSchreiber, objectData, XmlSerializerNamespace)
                        NLogger.Debug($"Einstellungsdatei gespeichert: {Pfad}")

                        Return True
                    Catch ex As InvalidOperationException
                        NLogger.Fatal(ex, $"Fehler beim Serialisieren/Speichern von {GetType(T).FullName}: {Pfad}")

                        Return False
                    End Try

                End With
            End Using
        End If

        Return False
    End Function
#End Region

    ''' <summary>
    ''' Erzeugt einen Klone des übergebenen Objektes mittels XML Serialisierung und anschließender Deserialisierung.
    ''' </summary>
    ''' <typeparam name="T">Typ des Objektes.</typeparam>
    ''' <param name="Objekt">Das zu klonende Objekt</param>
    ''' <returns>Den Klon.</returns>
    Friend Function XMLClone(Of T)(Objekt As T) As T
        Dim tmp As String = String.Empty

        If Objekt IsNot Nothing Then
            If Not XmlSerializeToString(Objekt, tmp) OrElse Not DeserializeXML(tmp, False, XMLClone) Then
                NLogger.Warn($"Fehler beim Klonen eines Objektes ({Objekt.GetType.Name}):  '{tmp}'")
            End If
        End If
    End Function
#End Region

#Region "JSON"
    Friend Function JSONDeserializeObjectFromString(Of T)(objectData As String, ByRef result As T) As Boolean
        Try
            result = JsonConvert.DeserializeObject(Of T)(objectData, New CustomBooleanJsonConverter)
            Return True
        Catch ex As Exception
            NLogger.Fatal(ex, $"Fehler beim Deserialisieren von {GetType(T).Name}: {objectData}")

            Return False
        End Try
    End Function

    Friend Async Function JSONDeserializeFromStringAsync(Of T)(objectData As String) As Task(Of T)
        Return Await Task.Run(Function()
                                  Dim Result As T

                                  Return If(JSONDeserializeObjectFromString(objectData, Result), Result, Nothing)

                              End Function)
    End Function
#End Region
End Module
