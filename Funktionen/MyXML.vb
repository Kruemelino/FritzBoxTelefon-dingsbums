Imports System.Xml
Imports System.Timers

Public Class MyXML
    Private XMLDoc As XmlDocument
    Private sDateiPfad As String

    Private Const Speicherintervall As Double = 15 'in Minuten
    Private Const RootName As String = "FritzOutlookXML"
    Private WithEvents tSpeichern As Timer

    Public Sub New(ByVal DateiPfad As String)
        sDateiPfad = DateiPfad
        XMLDoc = New XmlDocument()
        With My.Computer.FileSystem
            If .FileExists(sDateiPfad) And .GetFileInfo(sDateiPfad).Extension.ToString = ".xml" Then
                XMLDoc.Load(sDateiPfad)
            Else
                XMLDoc.LoadXml("<?xml version=""1.0"" encoding=""UTF-8""?><" & RootName & "/>")
                .CreateDirectory(.GetParentPath(sDateiPfad))
                .WriteAllText(sDateiPfad, XMLDoc.InnerXml, True)
            End If
        End With
        CleanUpXML()
        tSpeichern = New Timer
        With tSpeichern
            .Interval = TimeSpan.FromMinutes(Speicherintervall).TotalMilliseconds
            .Start()
        End With
    End Sub

    Protected Overrides Sub Finalize()
        XMLDoc.Save(sDateiPfad)
        XMLDoc = Nothing
        If Not tSpeichern Is Nothing Then
            tSpeichern.Stop()
            tSpeichern.Dispose()
            tSpeichern = Nothing
        End If

        MyBase.Finalize()
    End Sub

#Region "Read"
    Public Overloads Function Read(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal sDefault As String) As String
        Dim xPathTeile As New ArrayList
        With xPathTeile
            .Add(IIf(IsNumeric(Left(DieSektion, 1)), "ID" & DieSektion, DieSektion))
            .Add(IIf(IsNumeric(Left(DerEintrag, 1)), "ID" & DerEintrag, DerEintrag))
        End With
        Return Read(xPathTeile, sDefault)
    End Function

    Public Overloads Function Read(ByVal xPathTeile As ArrayList, ByVal sDefault As String) As String
        Read = vbNullString
        Dim tmpXMLNodeList As XmlNodeList
        Dim xPath As String = CreateXPath(xPathTeile)

        tmpXMLNodeList = XMLDoc.SelectNodes(xPath)
        If Not tmpXMLNodeList.Count = 0 Then
            ' If tmpXMLNodeList.Count = 1 Then
            'Read = tmpXMLNodeList.Item(0).InnerText
            'Else
            For Each tmpXMLNode As XmlNode In tmpXMLNodeList
                Read += tmpXMLNode.InnerText & ";"
            Next
            Read = Left(Read, Len(Read) - 1)
            'End If
        Else
            Read = sDefault
        End If
        xPathTeile = Nothing
    End Function

    Function ReadElementName(ByVal ZielKnoten As ArrayList, ByVal sDefault As String) As String
        ReadElementName = vbNullString
        Dim xPath As String
        Dim tmpXMLNode As XmlNode
        xPath = CreateXPath(ZielKnoten)
        With XMLDoc
            tmpXMLNode = .SelectSingleNode(xPath)
            If Not tmpXMLNode Is Nothing Then
                ReadElementName = tmpXMLNode.ParentNode.Name
            End If
        End With
        tmpXMLNode = Nothing
    End Function
#End Region

#Region "Write"
    Public Overloads Function Write(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal Value As String, ByVal SpeichereDatei As Boolean) As Boolean
        Dim xPathTeile As New ArrayList
        With xPathTeile
            .Add(IIf(IsNumeric(Left(DieSektion, 1)), "ID" & DieSektion, DieSektion))
            .Add(IIf(IsNumeric(Left(DerEintrag, 1)), "ID" & DerEintrag, DerEintrag))
        End With
        Return Write(xPathTeile, Value, SpeichereDatei)
    End Function

    Public Overloads Function Write(ByVal ZielKnoten As ArrayList, ByVal Value As String, ByVal SpeichereDatei As Boolean) As Boolean
        Return Write(ZielKnoten, Value, vbNullString, vbNullString, SpeichereDatei)
    End Function

    Public Overloads Function Write(ByVal ZielKnoten As ArrayList, ByVal Value As String, ByVal AttributeName As String, ByVal AttributeValue As String, ByVal SpeichereDatei As Boolean) As Boolean
        Dim xPathTeile As New ArrayList
        Dim sTmpXPath As String = vbNullString
        Dim xPath As String
        Dim tmpXMLNode As XmlNode
        Dim tmpXMLAttribute As XmlAttribute
        xPath = CreateXPath(ZielKnoten)
        With XMLDoc
            tmpXMLNode = .SelectSingleNode(xPath)
            If Not tmpXMLNode Is Nothing Then
                If Not AttributeName = vbNullString Then
                    If Not (tmpXMLNode.ChildNodes.Count = 0 And tmpXMLNode.Value = Nothing) Then
                        tmpXMLNode = .SelectSingleNode(xPath & CStr(IIf(Not AttributeName = vbNullString, "[@" & AttributeName & "=""" & AttributeValue & """]", vbNullString)))
                    End If
                    If tmpXMLNode Is Nothing Then
                        tmpXMLNode = .SelectSingleNode(xPath).ParentNode.AppendChild(.CreateElement(.SelectSingleNode(xPath).Name))
                    End If
                    tmpXMLAttribute = XMLDoc.CreateAttribute(AttributeName)
                    tmpXMLAttribute.Value = AttributeValue
                    tmpXMLNode.Attributes.Append(tmpXMLAttribute)
                End If
                tmpXMLNode.InnerText() = Value
            Else
                For Each sNodeName As String In ZielKnoten
                    If IsNumeric(Left(sNodeName, 1)) Then sNodeName = "ID" & sNodeName
                    xPathTeile.Add(sNodeName)
                    xPath = CreateXPath(xPathTeile)
                    If .SelectSingleNode(xPath) Is Nothing Then
                        .SelectSingleNode(sTmpXPath).AppendChild(.CreateElement(sNodeName))
                    End If
                    sTmpXPath = xPath
                Next
                Write(ZielKnoten, Value, AttributeName, AttributeValue, SpeichereDatei)
            End If
            If SpeichereDatei Then SpeichereXMLDatei()
        End With
        xPathTeile = Nothing
        tmpXMLAttribute = Nothing
        tmpXMLNode = Nothing
        Return True
    End Function

    Public Overloads Function WriteAttribute(ByVal ZielKnoten As ArrayList, ByVal AttributeName As String, ByVal AttributeValue As String) As Boolean
        WriteAttribute = False
        Dim xPath As String
        xPath = CreateXPath(ZielKnoten)
        WriteAttribute(XMLDoc.SelectNodes(xPath), AttributeName, AttributeValue)
    End Function

    Public Overloads Function WriteAttribute(ByRef tmpXMLNodeList As XmlNodeList, ByVal AttributeName As String, ByVal AttributeValue As String) As Boolean
        WriteAttribute = True

        Dim tmpXMLAttribute As XmlAttribute

        With XMLDoc
            If Not tmpXMLNodeList.Count = 0 Then
                For Each tmpXMLNode As XmlNode In tmpXMLNodeList
                    tmpXMLAttribute = tmpXMLNode.Attributes.ItemOf(AttributeName)
                    If tmpXMLAttribute Is Nothing Then
                        tmpXMLAttribute = .CreateAttribute(AttributeName)
                        tmpXMLNode.Attributes.Append(tmpXMLAttribute)
                    End If
                    tmpXMLAttribute.Value = AttributeValue
                Next
            End If
        End With
    End Function
#End Region

#Region "Löschen"

    Public Overloads Function Delete(ByVal DieSektion As String) As Boolean
        Dim xPathTeile As New ArrayList
        xPathTeile.Add(DieSektion)
        Return Delete(xPathTeile)
    End Function

    Public Overloads Function Delete(ByVal alxPathTeile As ArrayList) As Boolean
        Dim tmpXMLNode As XmlNode
        Dim xPath As String = CreateXPath(alxPathTeile)
        With XMLDoc
            If Not .SelectSingleNode(xPath) Is Nothing Then
                tmpXMLNode = .SelectSingleNode(xPath).ParentNode
                tmpXMLNode.RemoveChild(.SelectSingleNode(xPath))
                If tmpXMLNode.ChildNodes.Count = 0 Then
                    tmpXMLNode.ParentNode.RemoveChild(tmpXMLNode)
                End If
            End If
        End With
        alxPathTeile = Nothing
        Return True
    End Function

#End Region

#Region "Knoten"
    Function CreateXMLNode(ByVal NodeName As String, ByVal SubNodeName As ArrayList, ByVal SubNodeValue As ArrayList, ByVal AttributeName As ArrayList, ByVal AttributeValue As ArrayList) As XmlNode
        CreateXMLNode = Nothing
        If SubNodeName.Count = SubNodeValue.Count Then

            Dim tmpXMLNode As XmlNode
            Dim tmpXMLChildNode As XmlNode
            Dim tmpXMLAttribute As XmlAttribute
            tmpXMLNode = XMLDoc.CreateNode(XmlNodeType.Element, NodeName, vbNullString)
            With tmpXMLNode
                For i As Integer = 0 To SubNodeName.Count - 1
                    If Not SubNodeValue.Item(i).ToString = "-1" Then
                        tmpXMLChildNode = XMLDoc.CreateNode(XmlNodeType.Element, SubNodeName.Item(i).ToString, vbNullString)
                        tmpXMLChildNode.InnerText = SubNodeValue.Item(i).ToString
                        .AppendChild(tmpXMLChildNode)
                    End If
                Next
            End With
            For i As Integer = 0 To AttributeName.Count - 1
                If Not AttributeValue.Item(i) Is Nothing Then
                    tmpXMLAttribute = XMLDoc.CreateAttribute(AttributeName.Item(i).ToString)
                    tmpXMLAttribute.Value = AttributeValue.Item(i).ToString
                    tmpXMLNode.Attributes.Append(tmpXMLAttribute)
                End If
            Next

            CreateXMLNode = tmpXMLNode

            tmpXMLAttribute = Nothing
            tmpXMLNode = Nothing
            tmpXMLChildNode = Nothing
        End If
    End Function

    Sub ReadXMLNode(ByVal alxPathTeile As ArrayList, ByVal SubNodeName As ArrayList, ByRef SubNodeValue As ArrayList, ByVal AttributeValue As String)

        If SubNodeName.Count = SubNodeValue.Count Then
            Dim xPath As String
            Dim tmpXMLNode As XmlNode
            With XMLDoc
                If Not AttributeValue = vbNullString Then
                    alxPathTeile.Add("[@ID=""" & AttributeValue & """]")
                End If
                xPath = CreateXPath(alxPathTeile)
                tmpXMLNode = .SelectSingleNode(xPath)
                If Not tmpXMLNode Is Nothing Then
                    With tmpXMLNode
                        For Each XmlChildNode As XmlNode In tmpXMLNode.ChildNodes
                            If Not SubNodeName.IndexOf(XmlChildNode.Name) = -1 Then
                                SubNodeValue.Item(SubNodeName.IndexOf(XmlChildNode.Name)) = XmlChildNode.InnerText
                            End If

                        Next
                    End With
                End If
            End With
            tmpXMLNode = Nothing
        End If
    End Sub

    Sub AppendNode(ByVal alxPathTeile As ArrayList, ByVal Knoten As XmlNode)
        Dim xPathTeileEC As Long = alxPathTeile.Count
        Dim DestxPath As String
        Dim tmpxPath As String = vbNullString
        Dim tmpXMLNode As XmlNode
        DestxPath = CreateXPath(alxPathTeile)
        With XMLDoc
            tmpXMLNode = .SelectSingleNode(DestxPath)
            If tmpXMLNode Is Nothing Then
                Write(alxPathTeile, "", False)
                tmpXMLNode = .SelectSingleNode(DestxPath)
            End If
            'Attribute
            alxPathTeile.Add(Knoten.Name)
            With Knoten
                If Not .Attributes.Count = 0 Then
                    For i = 0 To .Attributes.Count - 1
                        ' String "tmpxPath" wird hier missbraucht, damit keine unnötige Variable deklariert werden muss.
                        tmpxPath += "[@" & .Attributes.Item(i).Name & "=""" & .Attributes.Item(i).Value & """]"
                    Next
                    alxPathTeile.Add(Replace(tmpxPath, "][@", " and @", , , CompareMethod.Text))
                End If
            End With
            tmpxPath = CreateXPath(alxPathTeile)

            If Not .SelectSingleNode(tmpxPath) Is Nothing Then
                tmpXMLNode.RemoveChild(.SelectSingleNode(tmpxPath))
            End If
            tmpXMLNode.AppendChild(Knoten)
        End With
        Do Until alxPathTeile.Count = xPathTeileEC
            alxPathTeile.RemoveAt(alxPathTeile.Count - 1)
        Loop

    End Sub

    Function SubNoteCount(ByVal alxPathTeile As ArrayList) As Integer
        SubNoteCount = 0
        Dim tmpxPath As String
        Dim tmpXMLNode As XmlNode
        tmpxPath = CreateXPath(alxPathTeile)
        With XMLDoc
            tmpXMLNode = .SelectSingleNode(tmpxPath)
            If Not tmpXMLNode Is Nothing Then
                SubNoteCount = tmpXMLNode.ChildNodes.Count
            End If
        End With
        tmpXMLNode = Nothing
    End Function

#End Region

#Region "Speichern"
    Sub SpeichereXMLDatei()
        XMLDoc.Save(sDateiPfad)
    End Sub

    Private Sub tSpeichern_Elapsed(sender As Object, e As ElapsedEventArgs) Handles tSpeichern.Elapsed
        SpeichereXMLDatei()
    End Sub
#End Region

#Region "Stuff"
    Private Sub CleanUpXML()
        Dim tmpNode As XmlNode
        Dim xPathTeile As New ArrayList
        Dim xPath As String

        With XMLDoc
            ' Diverse Knoten des Journals löschen
            xPathTeile.Add("Journal")
            xPathTeile.Add("SchließZeit")
            xPath = CreateXPath(xPathTeile)
            tmpNode = .SelectSingleNode(xPath)
            xPathTeile.Remove("SchließZeit")
            xPath = CreateXPath(xPathTeile)
            If Not tmpNode Is Nothing Then
                .SelectSingleNode(xPath).RemoveAll()
                .SelectSingleNode(xPath).AppendChild(tmpNode)
            End If
            ' Alle Knoten LetzterAnrufer löschen
            xPathTeile.RemoveRange(0, xPathTeile.Count)
            xPathTeile.Add("LetzterAnrufer")
            xPath = CreateXPath(xPathTeile)
            tmpNode = .SelectSingleNode(xPath)
            If Not tmpNode Is Nothing Then
                .DocumentElement.RemoveChild(.SelectSingleNode(xPath))
            End If
            xPathTeile = Nothing
        End With
    End Sub

    Function CreateXPath(ByVal xPathElements As ArrayList) As String
        If Not xPathElements.Item(0).ToString = XMLDoc.DocumentElement.Name Then xPathElements.Insert(0, XMLDoc.DocumentElement.Name)
        CreateXPath = Replace("/" & Join(xPathElements.ToArray(), "/"), "/[", "[", , , CompareMethod.Text)
        CreateXPath = Replace(CreateXPath, "][", " and ", , , CompareMethod.Text)
    End Function

    Function GetXMLDateiPfad() As String
        Return sDateiPfad
    End Function
#End Region

End Class
