Imports System.Xml

Public Class XML

    Public Sub New()
        P_NameStartChar = GetNameStartChar()
        P_NameChar = GetNameChar()
    End Sub

#Region "PrivateData"
    Private _NameStartChar As String
    Private _NameChar As String
#End Region

#Region "Konstanten"
    ''' <summary>
    ''' xPath Steuerzeichen: Seperator /
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>/</returns>
    Private ReadOnly Property P_xPathSeperatorSlash() As String
        Get
            Return "/"
        End Get
    End Property

    ''' <summary>
    ''' xPath Steuerzeichen: WildCard *
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>*</returns>
    Private ReadOnly Property P_xPathWildCard() As String
        Get
            Return "*"
        End Get
    End Property

    ''' <summary>
    ''' xPath Steuerzeichen: Öffnende eckige Klammer [
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>[</returns>
    Private ReadOnly Property P_xPathBracketOpen() As String
        Get
            Return "["
        End Get
    End Property

    ''' <summary>
    ''' xPath Steuerzeichen: Schließende eckige Klammer ]
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>]</returns>
    Private ReadOnly Property P_xPathBracketClose() As String
        Get
            Return "]"
        End Get
    End Property

    ''' <summary>
    ''' xPath Steuerzeichen: @
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>@</returns>
    Private ReadOnly Property P_xPathAttribute() As String
        Get
            Return "@"
        End Get
    End Property

    ''' <summary>
    ''' Ein String, der alle nach den 2.3 Common Syntactic Constructs für NameStartChar enthält
    ''' </summary>
    Private Property P_NameStartChar As String
        Set(value As String)
            _NameStartChar = value
        End Set
        Get
            Return _NameStartChar
        End Get
    End Property

    ''' <summary>
    ''' Ein String, der alle nach den 2.3 Common Syntactic Constructs für NameChar enthält
    ''' </summary>
    Private Property P_NameChar As String
        Set(value As String)
            _NameChar = value
        End Set
        Get
            Return _NameChar
        End Get
    End Property

    ''' <summary>
    ''' Leerstring, String.Empty
    ''' </summary>
    Public Shared ReadOnly Property P_Def_StringEmpty() As String
        Get
            Return String.Empty
        End Get
    End Property

    ''' <summary>
    ''' -1 als String.
    ''' Default Fehler
    ''' </summary>
    ''' <value>-1</value>
    ''' <returns>String</returns>
    Public Shared ReadOnly Property P_Def_ErrorMinusOne_String() As String
        Get
            Return "-1"
        End Get
    End Property
#End Region

    ''' <summary>
    ''' Erstellt ein String, der alle gültigen Zeichen nach den 2.3 Common Syntactic Constructs für NameStartChar enthält:
    ''' ":" | [A-Z] | "_" | [a-z] | [#xC0-#xD6] | [#xD8-#xF6] | [#xF8-#x2FF] | [#x370-#x37D] | [#x37F-#x1FFF] | [#x200C-#x200D] | [#x2070-#x218F] | [#x2C00-#x2FEF] | [#x3001-#xD7FF] | [#xF900-#xFDCF] | [#xFDF0-#xFFFD] | [#x10000-#xEFFFF]
    ''' </summary>
    ''' <returns>String, der alle erlaubten Startchars enthält</returns>
    ''' <remarks>http://www.w3.org/TR/REC-xml/#NT-Name</remarks>
    Private Function GetNameStartChar() As String
        ' Doppelpunkt :
        Dim tmp As String = Chr(58)

        ' [A-Z]
        For c = 65 To 90
            tmp += Chr(c)
        Next

        ' Unterstrich _
        tmp += Chr(95)

        ' [a-z]
        For c = 97 To 122
            tmp += Chr(c)
        Next

        '[#xC0-#xD6]
        For c = &HC0 To &HD6
            tmp += Convert.ToChar(c)
        Next

        '[#xD8-#xF6]
        For c = &HD8 To &HF6
            tmp += Convert.ToChar(c)
        Next

        '[#xF8-#x2FF] 
        For c = &HF8 To &H2FF
            tmp += Convert.ToChar(c)
        Next

        '[#x370-#x37D]
        For c = &H370 To &H37D
            tmp += Convert.ToChar(c)
        Next

        '[#x37F-#x1FFF] 
        For c = &H37F To &H1FFF
            tmp += Convert.ToChar(c)
        Next

        '[#x200C-#x200D] 
        For c = &H200C To &H200D
            tmp += Convert.ToChar(c)
        Next

        '[#x2070-#x218F]
        For c = &H2070 To &H218F
            tmp += Convert.ToChar(c)
        Next

        '[#x2C00-#x2FEF]
        For c = &H2C00 To &H2FEF
            tmp += Convert.ToChar(c)
        Next

        '[#x3001-#xD7FF]
        For c = &H3001 To &HD7FF
            tmp += Convert.ToChar(c)
        Next

        '[#xF900-#xFDCF]
        For c = &HF900 To &HFDCF
            tmp += Convert.ToChar(c)
        Next

        '[#xFDF0-#xFFFD]
        For c = &HFDF0 To &HFFFD
            tmp += Convert.ToChar(c)
        Next

        ''[#x10000-#xEFFFF]
        'For c = &H10000 To &HEFFFF
        '    tmp += Convert.ToChar(c)
        'Next

        Return tmp

    End Function

    ''' <summary>
    ''' Erstellt ein String, der alle gültigen Zeichen nach den 2.3 Common Syntactic Constructs für NameChar enthält:
    ''' NameStartChar | "-" | "." | [0-9] | #xB7 | [#x0300-#x036F] | [#x203F-#x2040]
    ''' </summary>
    ''' <returns>String, der alle erlaubten Startchars enthält</returns>
    ''' <remarks>http://www.w3.org/TR/REC-xml/#NT-Name</remarks>
    Private Function GetNameChar() As String
        ' NameStartChar
        Dim tmp As String = P_NameStartChar

        ' Bindestrich -
        tmp += Chr(45)

        ' Punkt .
        tmp += Chr(46)

        ' [0-9]
        For c = 48 To 57
            tmp += Chr(c)
        Next

        ' Middle dot
        tmp += Convert.ToChar(&HB7)

        '[#x0300-#x036F]
        For c = &H300 To &H36F
            tmp += Convert.ToChar(c)
        Next

        ''[#x203F-#x2040]
        For c = &H203F To &H2040
            tmp += Convert.ToChar(c)
        Next

        Return tmp

    End Function

#Region "XML"
#Region "Read"
    Public Overloads Function Read(ByVal XMLDoc As XmlDocument, ByVal DieSektion As String, ByVal DerEintrag As String, ByVal sDefault As String) As String
        Dim xPathTeile As New ArrayList
        With xPathTeile
            .Add(IIf(IsNumeric(Left(DieSektion, 1)), "ID" & DieSektion, DieSektion))
            .Add(IIf(IsNumeric(Left(DerEintrag, 1)), "ID" & DerEintrag, DerEintrag))
        End With
        Return Read(XMLDoc, xPathTeile, sDefault)
    End Function

    Public Overloads Function Read(ByVal XMLDoc As XmlDocument, ByVal xPathTeile As ArrayList, ByVal sDefault As String) As String
        Read = sDefault

        Dim tmpXMLNodeList As XmlNodeList
        Dim xPath As String = CreateXPath(XMLDoc, xPathTeile)

        'If CheckXPathRead(xPath) Then
        tmpXMLNodeList = XMLDoc.SelectNodes(xPath)
        If Not tmpXMLNodeList.Count = 0 Then
            Read = P_Def_StringEmpty
            For Each tmpXMLNode As XmlNode In tmpXMLNodeList
                Read += tmpXMLNode.InnerText & ";"
            Next
            Read = Left(Read, Len(Read) - 1)
        End If
        'End If
        xPathTeile = Nothing
    End Function

    ''' <summary>
    ''' Ersetzt Wildcard durch einen vorhandenen Knoten.
    ''' </summary>
    ''' <param name="xPathTeile"></param>
    ''' <remarks>Ich weiß nicht mehr was der hier macht.</remarks>
    Public Function GetProperXPath(ByVal XMLDoc As XmlDocument, ByRef xPathTeile As ArrayList) As Boolean
        GetProperXPath = False
        Dim i As Integer = 1
        Dim xPath As String
        Dim tmpXMLNode As XmlNode
        Dim tmpParentXMLNode As XmlNode

        xPath = CreateXPath(XMLDoc, xPathTeile)

        tmpXMLNode = XMLDoc.SelectSingleNode(xPath)
        If tmpXMLNode IsNot Nothing Then
            tmpParentXMLNode = tmpXMLNode.ParentNode
            Do Until tmpParentXMLNode.Name = xPathTeile.Item(1).ToString

                If Not (xPathTeile.Item(xPathTeile.Count - i - 1).ToString.StartsWith(P_xPathBracketOpen) Or _
                        xPathTeile.Item(xPathTeile.Count - i - 1).ToString.StartsWith(P_xPathAttribute)) Then

                    xPathTeile.Item(xPathTeile.Count - i - 1) = tmpParentXMLNode.Name
                    tmpParentXMLNode = tmpParentXMLNode.ParentNode

                End If
                i += 1
            Loop
            GetProperXPath = True
        End If

    End Function

    Function ReadElementName(ByVal XMLDoc As XmlDocument, ByVal xPathTeile As ArrayList, ByVal sDefault As String) As String
        ReadElementName = sDefault
        Dim xPath As String
        Dim tmpXMLNode As XmlNode
        xPath = CreateXPath(XMLDoc, xPathTeile)
        With XMLDoc
            tmpXMLNode = .SelectSingleNode(xPath)
            If tmpXMLNode IsNot Nothing Then
                ReadElementName = tmpXMLNode.ParentNode.Name
            End If
        End With
        tmpXMLNode = Nothing
    End Function
#End Region

#Region "Write"
    Public Overloads Function Write(ByRef XMLDoc As XmlDocument, ByVal DieSektion As String, ByVal DerEintrag As String, ByVal Value As String) As Boolean
        Dim xPathTeile As New ArrayList
        With xPathTeile
            .Add(IIf(IsNumeric(Left(DieSektion, 1)), "ID" & DieSektion, DieSektion))
            .Add(IIf(IsNumeric(Left(DerEintrag, 1)), "ID" & DerEintrag, DerEintrag))
        End With
        Return Write(XMLDoc, xPathTeile, Value)
    End Function

    Public Overloads Function Write(ByRef XMLDoc As XmlDocument, ByVal ZielKnoten As ArrayList, ByVal Value As String) As Boolean
        Return Write(XMLDoc, ZielKnoten, Value, P_Def_StringEmpty, P_Def_StringEmpty)
    End Function

    Public Overloads Function Write(ByRef XMLDoc As XmlDocument, ByVal ZielKnoten As ArrayList, ByVal Value As String, ByVal AttributeName As String, ByVal AttributeValue As String) As Boolean
        Dim xPathTeile As New ArrayList
        Dim sParentXPath As String = P_Def_StringEmpty
        Dim xPath As String
        Dim tmpXMLNode As XmlNode
        Dim tmpXMLNodeList As XmlNodeList
        Dim tmpXMLAttribute As XmlAttribute

        xPath = CreateXPath(XMLDoc, ZielKnoten)
        With XMLDoc
            tmpXMLNodeList = .SelectNodes(xPath)
            If Not tmpXMLNodeList.Count = 0 Then
                For Each tmpXMLNode In tmpXMLNodeList
                    If Not AttributeName = P_Def_StringEmpty Then
                        If Not (tmpXMLNode.ChildNodes.Count = 0 And tmpXMLNode.Value = Nothing) Then
                            tmpXMLNode = .SelectSingleNode(xPath & CStr(IIf(Not AttributeName = P_Def_StringEmpty, "[@" & AttributeName & "=""" & AttributeValue & """]", P_Def_StringEmpty)))
                        End If
                        If tmpXMLNode Is Nothing Then
                            tmpXMLNode = .SelectSingleNode(xPath).ParentNode.AppendChild(.CreateElement(.SelectSingleNode(xPath).Name))
                        End If
                        tmpXMLAttribute = XMLDoc.CreateAttribute(AttributeName)
                        tmpXMLAttribute.Value = AttributeValue
                        tmpXMLNode.Attributes.Append(tmpXMLAttribute)
                    End If
                    tmpXMLNode.InnerText() = Value
                Next
            Else
                ' Eintrag noch nicht vorhanden
                'If xPath.Contains(P_xPathWildCard) Then
                '    GetProperXPath(ZielKnoten)
                '    xPath = CreateXPath(ZielKnoten)
                'End If
                sParentXPath = .DocumentElement.Name
                For Each sNodeName As String In ZielKnoten
                    ' Rüfe ob NodeName den XML-Namenskonvention entspricht
                    If IsNumeric(Left(sNodeName, 1)) Then sNodeName = "ID" & sNodeName
                    xPathTeile.Add(sNodeName)
                    xPath = CreateXPath(XMLDoc, xPathTeile)
                    If .SelectSingleNode(xPath) Is Nothing Then
                        .SelectSingleNode(sParentXPath).AppendChild(.CreateElement(sNodeName))
                        'If Not (sNodeName.Contains(P_xPathBracketOpen) And sNodeName.Contains(P_xPathBracketClose)) Then
                        '    If .SelectSingleNode(sParentXPath) IsNot Nothing Then
                        '        .SelectSingleNode(sParentXPath).AppendChild(.CreateElement(sNodeName))
                        '    Else
                        '        ' Wenn der Knoten, in den Geschrieben werden soll, nicht erstellt werden kann, dann wird Write auf False gesetzt.
                        '        Write = False
                        '    End If
                        'End If
                    End If
                    sParentXPath = xPath
                Next
                ' Prüfen, ob es Probleme gab.
                Write(XMLDoc, ZielKnoten, Value, AttributeName, AttributeValue)
            End If
        End With
        Write = True

        xPathTeile = Nothing
        tmpXMLAttribute = Nothing
        tmpXMLNode = Nothing
    End Function

    ''' <summary>
    ''' Prüft den NodeName auf nicht erlaubte Zeichen.
    ''' Wenn das erste Zeichen nicht korrekt ist, wird ein _ davorgesetzt. Dies ist erlaubt.
    ''' Wenn weiter Zeichen nicht korrekt sind, werden diese durch den Charcode ersetzt. (Prüfen)
    ''' </summary>
    ''' <param name="sNodeName">Korrekter String</param>
    ''' <remarks>http://www.w3.org/TR/REC-xml/#NT-Name</remarks>
    Public Function CheckNodeName(ByVal sNodeName As String) As String

        ' Ist erstes Zeichen prüfen
        If Not P_NameStartChar.Contains(Left(sNodeName, 1)) Then
            sNodeName = "_" & sNodeName
        End If

        ' Prüfe ob nichterlaubte Zeichen im Namen sind
        For Each C As Char In sNodeName
            If Not P_NameChar.Contains(C) Then
                'Prüfen ob das Sinnvoll ist
                sNodeName = Replace(sNodeName, C, CStr(Asc(C)), , , CompareMethod.Text)
            End If
        Next

        ' Nodename darf nicht mit XML beginnen
        If LCase(Left(sNodeName, 3)) = "xml" Then sNodeName = "_" & sNodeName
        'Rückgabe
        Return sNodeName
    End Function

    Public Overloads Function WriteAttribute(ByRef XMLDoc As XmlDocument, ByVal ZielKnoten As ArrayList, ByVal AttributeName As String, ByVal AttributeValue As String) As Boolean
        WriteAttribute = False
        Dim xPath As String
        xPath = CreateXPath(XMLDoc, ZielKnoten)
        WriteAttribute(XMLDoc, XMLDoc.SelectNodes(xPath), AttributeName, AttributeValue)
    End Function

    Public Overloads Function WriteAttribute(ByRef XMLDoc As XmlDocument, ByRef tmpXMLNodeList As XmlNodeList, ByVal AttributeName As String, ByVal AttributeValue As String) As Boolean
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

    Public Overloads Function Delete(ByRef XMLDoc As XmlDocument, ByVal DieSektion As String) As Boolean
        Dim xPathTeile As New ArrayList
        xPathTeile.Add(DieSektion)
        Return Delete(XMLDoc, xPathTeile)
    End Function

    Public Overloads Function Delete(ByRef XMLDoc As XmlDocument, ByVal alxPathTeile As ArrayList) As Boolean
        Dim tmpXMLNodeList As XmlNodeList

        Dim xPath As String = CreateXPath(XMLDoc, alxPathTeile)
        With XMLDoc
            tmpXMLNodeList = .SelectNodes(xPath)
            For Each tmpXMLNode As XmlNode In tmpXMLNodeList
                If tmpXMLNode IsNot Nothing Then
                    tmpXMLNode = .SelectSingleNode(xPath).ParentNode
                    tmpXMLNode.RemoveChild(.SelectSingleNode(xPath))
                    If tmpXMLNode.ChildNodes.Count = 0 Then
                        tmpXMLNode.ParentNode.RemoveChild(tmpXMLNode)
                    End If
                End If
            Next
        End With
        alxPathTeile = Nothing
        Return True
    End Function

#End Region

#Region "Knoten"
    Function CreateXMLNode(ByRef XMLDoc As XmlDocument, ByVal NodeName As String, ByVal SubNodeName As ArrayList, ByVal SubNodeValue As ArrayList, ByVal AttributeName As ArrayList, ByVal AttributeValue As ArrayList) As XmlNode
        CreateXMLNode = Nothing
        If SubNodeName.Count = SubNodeValue.Count Then

            Dim tmpXMLNode As XmlNode
            Dim tmpXMLChildNode As XmlNode
            Dim tmpXMLAttribute As XmlAttribute
            tmpXMLNode = XMLDoc.CreateNode(XmlNodeType.Element, NodeName, P_Def_StringEmpty)
            With tmpXMLNode
                For i As Integer = 0 To SubNodeName.Count - 1
                    If Not SubNodeValue.Item(i).ToString = P_Def_ErrorMinusOne_String Then
                        tmpXMLChildNode = XMLDoc.CreateNode(XmlNodeType.Element, SubNodeName.Item(i).ToString, P_Def_StringEmpty)
                        tmpXMLChildNode.InnerText = SubNodeValue.Item(i).ToString
                        .AppendChild(tmpXMLChildNode)
                    End If
                Next
            End With
            For i As Integer = 0 To AttributeName.Count - 1
                If AttributeValue.Item(i) IsNot Nothing Then
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

    Sub ReadXMLNode(ByVal XMLDoc As XmlDocument, ByVal alxPathTeile As ArrayList, ByVal SubNodeName As ArrayList, ByRef SubNodeValue As ArrayList, ByVal AttributeName As String, ByVal AttributeValue As String)

        If SubNodeName.Count = SubNodeValue.Count Then
            Dim xPath As String
            Dim tmpXMLNode As XmlNode
            With XMLDoc
                ' BUG: 
                If Not AttributeValue = P_Def_StringEmpty And Not AttributeName = P_Def_StringEmpty Then alxPathTeile.Add("[@" & AttributeName & "=""" & AttributeValue & """]")
                xPath = CreateXPath(XMLDoc, alxPathTeile)
                If Not AttributeValue = P_Def_StringEmpty Then alxPathTeile.RemoveAt(alxPathTeile.Count - 1)
                tmpXMLNode = .SelectSingleNode(xPath)
                If tmpXMLNode IsNot Nothing Then
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

    Public Sub AppendNode(ByRef XMLDoc As XmlDocument, ByVal alxPathTeile As ArrayList, ByVal Knoten As XmlNode)
        Dim xPathTeileEC As Long = alxPathTeile.Count
        Dim DestxPath As String
        Dim tmpxPath As String = P_Def_StringEmpty
        Dim tmpXMLNode As XmlNode
        DestxPath = CreateXPath(XMLDoc, alxPathTeile)
        With XMLDoc
            tmpXMLNode = .SelectSingleNode(DestxPath)
            If tmpXMLNode Is Nothing Then
                Write(XMLDoc, alxPathTeile, "")
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
            tmpxPath = CreateXPath(XMLDoc, alxPathTeile)

            If .SelectSingleNode(tmpxPath) IsNot Nothing Then
                tmpXMLNode.RemoveChild(.SelectSingleNode(tmpxPath))
            End If
            tmpXMLNode.AppendChild(Knoten)
        End With
        Do Until alxPathTeile.Count = xPathTeileEC
            alxPathTeile.RemoveAt(alxPathTeile.Count - 1)
        Loop

    End Sub

    Public Function SubNoteCount(ByVal XMLDoc As XmlDocument, ByVal alxPathTeile As ArrayList) As Integer
        SubNoteCount = 0
        Dim tmpxPath As String
        Dim tmpXMLNode As XmlNode
        tmpxPath = CreateXPath(XMLDoc, alxPathTeile)
        With XMLDoc
            tmpXMLNode = .SelectSingleNode(tmpxPath)
            If tmpXMLNode IsNot Nothing Then
                SubNoteCount = tmpXMLNode.ChildNodes.Count
            End If
        End With
        tmpXMLNode = Nothing
    End Function
#End Region

#Region "Validator"
    ''' <summary>
    ''' Prüft ob die XML-Datei geöffnet werden kann.
    ''' </summary>
    ''' <param name="XMLpath"></param>
    ''' <returns><c>True</c>, wenn Datei geöffnet werden kann, ansonsten <c>False</c>.</returns>
    Public Function XMLValidator(ByRef XMLDoc As XmlDocument, ByVal XMLpath As String) As Boolean
        XMLValidator = True
        Try
            XMLDoc.Load(XMLpath)
        Catch
            XMLValidator = False
        End Try
    End Function
#End Region

    ''' <summary>
    ''' Erstellt einen korrekten xPath aus einer Liste einzelnen xPath-Elementen zusammen
    ''' </summary>
    ''' <param name="xPathElements">Lista an xPath-Elementen</param>
    ''' <returns>gültiger xPath</returns>
    Function CreateXPath(ByVal XMLDoc As XmlDocument, ByVal xPathElements As ArrayList) As String
        ' fügt den Root-knoten an, falls nicht vorhanden

        Dim newxPath As New ArrayList

        If Not xPathElements.Item(0).ToString = XMLDoc.DocumentElement.Name Then xPathElements.Insert(0, XMLDoc.DocumentElement.Name)

        For Each xPathElement As String In xPathElements
            If xPathElement.Contains(P_xPathBracketOpen) And xPathElement.Contains(P_xPathBracketClose) Or xPathElement.StartsWith(P_xPathAttribute) Or xPathElement.StartsWith(P_xPathWildCard) Then
                ' Hier eventuell eingreifen Attributnamen prüfen
                newxPath.Add(xPathElement)
            Else
                newxPath.Add(CheckNodeName(xPathElement))
            End If
        Next

        xPathElements = newxPath

        CreateXPath = Replace(P_xPathSeperatorSlash & Join(xPathElements.ToArray(), P_xPathSeperatorSlash), P_xPathSeperatorSlash & P_xPathBracketOpen, P_xPathBracketOpen, , , CompareMethod.Text)
        CreateXPath = Replace(CreateXPath, P_xPathBracketClose & P_xPathBracketOpen, " and ", , , CompareMethod.Text) ' ][ -> and
        newxPath = Nothing
    End Function
#End Region


End Class
