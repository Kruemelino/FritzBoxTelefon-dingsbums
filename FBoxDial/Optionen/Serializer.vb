Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading.Tasks
Imports System.Xml
Imports System.Xml.Serialization

Friend Module Serializer
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    <Extension> Friend Function Laden(ByRef XMLData As OutlookXML) As Boolean
        Dim mySerializer As New XmlSerializer(GetType(OutlookXML))
        Dim DateiInfo As FileInfo
        Dim Pfad As String

        Pfad = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, PDfltConfig_FileName)

        DateiInfo = New FileInfo(Pfad)
        DateiInfo.Directory.Create() ' If the directory already exists, this method does nothing.

        If File.Exists(Pfad) Then
            XMLData = DeserializeObject(Of OutlookXML)(Pfad)
        Else
            XMLData = ErstelleXMLDatei(Pfad)
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

        Return XMLData IsNot Nothing
    End Function

    <Extension> Friend Sub Speichern(Of T)(ByVal XMLData As T, ByVal Pfad As String)
        If XMLData IsNot Nothing Then
            Dim XmlSerializerNamespace As New XmlSerializerNamespaces()
            XmlSerializerNamespace.Add(PDfltStringEmpty, PDfltStringEmpty)

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

    'Friend Function XmlSerializeToString(ByVal objectInstance As Object) As String
    '    Dim serializer = New XmlSerializer(objectInstance.[GetType]())
    '    Dim sb = New StringBuilder()

    '    Using writer As TextWriter = New StringWriter(sb)
    '        serializer.Serialize(writer, objectInstance)
    '    End Using

    '    Return sb.ToString()
    'End Function

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

    Private Function ErstelleXMLDatei(ByVal sPfad As String) As OutlookXML
        Dim XMLDefault As DefaultWerte = New DefaultWerte

        XMLData = New OutlookXML

        Dim tmpPropertyInfo As Reflection.PropertyInfo
        For Each PropertyInfo As Reflection.PropertyInfo In XMLData.POptionen.GetType.GetProperties

            tmpPropertyInfo = Array.Find(XMLDefault.GetType.GetProperties,
                                         Function(DefPropertyInfo As Reflection.PropertyInfo) DefPropertyInfo.Name.AreEqual(PropertyInfo.Name.RegExReplace("^P", "PDflt")))

            If tmpPropertyInfo IsNot Nothing Then
                PropertyInfo.SetValue(XMLData.POptionen, tmpPropertyInfo.GetValue(XMLDefault))
            End If
        Next

        ' Setze einige Felder
        With XMLData
            With .POptionen
                .Arbeitsverzeichnis = Path.GetDirectoryName(sPfad)
                .ValidFBAdr = ValidIP(.TBFBAdr)
            End With
        End With

        XMLData.Speichern(Path.Combine(XMLData.POptionen.Arbeitsverzeichnis, PDfltConfig_FileName))
        Return XMLData
    End Function
End Module
