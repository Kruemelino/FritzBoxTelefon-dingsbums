Imports System.IO
Imports System.Threading.Tasks
Imports System.Xml
Imports System.Xml.Serialization

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
            NLogger.Fatal(ex, $"Die XML-Datab weist einen Lade- oder Analysefehler auf: '{InputData}'")

            Return False

        Catch ex As FileNotFoundException
            NLogger.Fatal(ex, $"Die XML-Datab kann nicht gefunden werden: '{InputData}'")

            Return False

        End Try
    End Function

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

        If CheckXMLData(Pfad, True) Then
            Dim Serializer As New XmlSerializer(GetType(T))
            Using Reader As XmlReader = XmlReader.Create(Pfad)

                If Serializer.CanDeserialize(Reader) Then

                    Try
                        ReturnObj = CType(Serializer.Deserialize(Reader), T)

                        Return True

                    Catch ex As InvalidOperationException

                        NLogger.Fatal(ex, $"Bei der Deserialisierung ist ein Fehler aufgetreten.: '{Pfad}'")
                        Return False
                    End Try
                Else
                    NLogger.Fatal($"Fehler beim Deserialisieren: {Pfad} kann nicht deserialisert werden.")
                    Return False
                End If
            End Using
        Else
            Return False
        End If

    End Function

    Friend Function DeserializeObject(Of T)(UniformResourceIdentifier As Uri, ByRef ReturnObj As T) As Boolean
        Return DeserializeObject(UniformResourceIdentifier.AbsoluteUri, ReturnObj)
    End Function

    Friend Function XmlDeserializeFromString(Of T)(objectData As String, ByRef result As T) As Boolean

        If CheckXMLData(objectData, False) Then

            Dim Serializer = New XmlSerializer(GetType(T))
            Using Reader As TextReader = New StringReader(objectData)
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

            Using XmlSchreiber As StringWriter = New StringWriter
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

        If Not XmlSerializeToString(Objekt, tmp) OrElse Not XmlDeserializeFromString(tmp, XMLClone) Then
            NLogger.Warn($"Fehler beim Klonen eines Objektes ({Objekt.GetType.Name}):  '{tmp}'")
        End If

    End Function

End Module
