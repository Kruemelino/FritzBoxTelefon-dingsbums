Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading.Tasks
Imports System.Xml
Imports System.Xml.Serialization

Friend Module Serializer
    Private Property NLogger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

    <Extension> Friend Function Laden(ByRef XMLData As OutlookXML) As Boolean
        Dim mySerializer As New XmlSerializer(GetType(OutlookXML))
        Dim DateiInfo As FileInfo
        Dim Pfad As String

        Pfad = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, PDfltAddin_KurzName & ".xml")

        DateiInfo = New FileInfo(Pfad)
        DateiInfo.Directory.Create() ' If the directory already exists, this method does nothing.

        If File.Exists(Pfad.ToString) Then
            Using XmlLeser As XmlReader = XmlReader.Create(Pfad)
                If mySerializer.CanDeserialize(XmlLeser) Then
                    Try
                        XMLData = CType(mySerializer.Deserialize(XmlLeser), OutlookXML)
                    Catch ex As InvalidOperationException
                        NLogger.Fatal(ex)
                    End Try
                End If
            End Using
        Else
            XMLData = ErstelleXMLDatei(Pfad)
        End If

        ' Setze einige Felder
        If XMLData IsNot Nothing Then
            With XMLData
                With .POptionen
                    .PArbeitsverzeichnis = DateiInfo.Directory.ToString
                    .PValidFBAdr = ValidIP(.PTBFBAdr)
                End With
            End With
        End If

        Return XMLData IsNot Nothing
    End Function

    Friend Function DeserializeObject(Of T)(ByVal Pfad As String) As Task(Of T)
        Return Task.Run(Function()
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
                        End Function)
    End Function

    <Extension> Friend Sub Speichern(ByVal XMLData As OutlookXML)
        If XMLData IsNot Nothing Then
            Dim mySerializer As New XmlSerializer(GetType(OutlookXML))
            Dim settings As New XmlWriterSettings With {.Indent = True, .OmitXmlDeclaration = False}
            Dim XmlSerializerNamespace As New XmlSerializerNamespaces()

            XmlSerializerNamespace.Add(PDfltStringEmpty, PDfltStringEmpty)

            Using XmlSchreiber As XmlWriter = XmlWriter.Create(Path.Combine(XMLData.POptionen.PArbeitsverzeichnis, PDfltAddin_KurzName & ".xml"), settings)
                mySerializer.Serialize(XmlSchreiber, XMLData, XmlSerializerNamespace)
            End Using
        End If
    End Sub

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
                .PArbeitsverzeichnis = Path.GetDirectoryName(sPfad)
                .PValidFBAdr = ValidIP(.PTBFBAdr)
            End With
        End With

        XMLData.Speichern
        Return XMLData
    End Function
End Module
