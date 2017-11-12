Imports System.Net
Imports System.IO

Friend Structure ArgumentDirection
#If OVer = 11 Then
    Private Dummy As String
#End If
    Friend Shared directionIN As String = "in"
    Friend Shared directionOUT As String = "out"
End Structure

Friend Structure dataType
#If OVer = 11 Then
    Private Dummy As String
#End If
    Friend Shared dataTypeString As String = "string"
    Friend Shared dataTypeuuid As String = "uuid"
End Structure

Friend Structure StateVariableSendEvent
#If OVer = 11 Then
    Private Dummy As String
#End If
    Friend Shared SendEventYES As String = "yes"
    Friend Shared SendEventNO As String = "no"
End Structure

Friend Class Action
    Implements IDisposable
    Friend BaseService As ServiceBaseInformation
    Friend ActionName As String
    Friend ArgumentList As New List(Of Argument)

    Friend Function GetInputArguments() As Hashtable
        Dim InputHashTable As New Hashtable
        For Each INArguments As Argument In ArgumentList.FindAll(Function(GetbyDirection) GetbyDirection.Direction = ArgumentDirection.directionIN)
            InputHashTable.Add(INArguments.Name, "")
        Next
        Return InputHashTable
    End Function

    Friend Function Start(ByVal InputArguments As Hashtable) As Hashtable
        Dim ReturnXMLDox As XmlDocument
        Dim OutputHashTable As New Hashtable

        ReturnXMLDox = FritzBoxPOST(ActionName, "https://" & C_DP.P_TBFBAdr & ":" & DataProvider.P_Port_FB_SOAP_SSL & BaseService.controlURL, BaseService.serviceType, GetSOAPRequest(InputArguments))
        If ReturnXMLDox.DocumentElement.Name = "FEHLER" Then
            With ErrorHashTable
                .Clear()
                .Add("Error", ReturnXMLDox.DocumentElement.InnerText)
            End With
            OutputHashTable = ErrorHashTable
        Else
            If Not ReturnXMLDox.InnerXml = "" Then
                For Each OUTArguments As Argument In ArgumentList.FindAll(Function(GetbyDirection) GetbyDirection.Direction = ArgumentDirection.directionOUT)
                    OutputHashTable.Add(OUTArguments.Name, ReturnXMLDox.GetElementsByTagName(OUTArguments.Name).Item(0).InnerText)
                Next
            End If
        End If

        Return OutputHashTable
    End Function

    ''' <summary>
    ''' Stellt den SOAP Request bereit
    ''' </summary>
    Private Function GetSOAPRequest(ByVal submitValues As Hashtable) As String
        ' Von hinten durch die Brust ins Auge

        Dim BaseNSs As String = "http://schemas.xmlsoap.org/soap/envelope/"
        Dim BaseEnc As String = "http://schemas.xmlsoap.org/soap/encoding/"

        Dim XMLSOAPRequest As New XmlDocument
        Dim XMLSOAPSchema As New Schema.XmlSchema

        Dim rootXMLElement As XmlElement
        Dim tmpXMLElement As XmlElement
        Dim tmpXMLNode As XmlNode

        With XMLSOAPSchema.Namespaces
            .Add("s", BaseNSs)
            .Add("u", BaseService.serviceType)
        End With

        With XMLSOAPRequest
            .Schemas.Add(XMLSOAPSchema)
            .AppendChild(.CreateXmlDeclaration("1.0", "utf-8", ""))

            rootXMLElement = .CreateElement("s", "Envelope", BaseNSs)
            rootXMLElement.SetAttribute("encodingStyle", BaseNSs, BaseEnc)

            tmpXMLElement = .CreateElement("s", "Body", BaseNSs)
            rootXMLElement.AppendChild(tmpXMLElement)

            tmpXMLNode = tmpXMLElement.AppendChild(.CreateElement("u", ActionName, BaseService.serviceType))

            If Not submitValues Is Nothing Then
                For Each submitItem As DictionaryEntry In submitValues
                    tmpXMLElement = .CreateElement("u", CStr(submitItem.Key), BaseService.serviceType)
                    tmpXMLElement.InnerText = submitItem.Value.ToString
                    tmpXMLNode.AppendChild(tmpXMLElement)
                Next
            End If

            .AppendChild(rootXMLElement)
        End With

        Return XMLSOAPRequest.InnerXml
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: Verwalteten Zustand löschen (verwaltete Objekte).
            End If
            ArgumentList.Clear()
        End If
        Me.disposedValue = True
    End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class

Friend Class Argument
    Friend Name As String
    Friend Direction As String
    Friend RelatedStateVariable As String
End Class

Friend Class ServiceBaseInformation
    Implements IDisposable

    Friend serviceType As String
    Friend serviceId As String
    Friend controlURL As String
    Friend eventSubURL As String
    Friend SCPDURL As String

#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: Verwalteten Zustand löschen (verwaltete Objekte).
            End If

            ' TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() unten überschreiben.
            ' TODO: Große Felder auf NULL festlegen.
        End If
        Me.disposedValue = True
    End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class

Friend Class StateVariable
    Implements IDisposable

    Friend Name As String
    Friend dataType As String
    Friend sendEvents As String

#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: Verwalteten Zustand löschen (verwaltete Objekte).
            End If

            ' TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() unten überschreiben.
            ' TODO: Große Felder auf NULL festlegen.
        End If
        Me.disposedValue = True
    End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class

Friend Module BaseFuctions

#Region "HTTP"
    Friend Function FritzBoxGET(ByVal Link As String, ByRef FBError As Boolean) As String
        Dim fbURI As New Uri(Link)

        FritzBoxGET = ""

        With CType(HttpWebRequest.Create(fbURI), HttpWebRequest)
            .Method = WebRequestMethods.Http.Get
            .Proxy = Nothing
            .KeepAlive = False
            .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)
            Try
                With New IO.StreamReader(.GetResponse().GetResponseStream(), Encoding.UTF8)
                    FBError = False
                    FritzBoxGET = .ReadToEnd()
                    .Close()
                End With
            Catch exANE As ArgumentNullException
                FBError = True
            Catch exWE As WebException
                FBError = True
            End Try
        End With

    End Function

    Friend Function FritzBoxPOST(ByVal SOAPAction As String, ByVal urlFull As String, ByVal ServiceType As String, ByVal SOAPXML As String) As XmlDocument

        Dim RetVal As New XmlDocument

        Dim ErrorText As String = DataProvider.P_Def_LeerString
        Dim fbPostBytes As Byte()

        Dim fbURI As New Uri(urlFull)

        Dim tmpUsername As String

        fbPostBytes = Encoding.UTF8.GetBytes(SOAPXML)

        ' Wenn der UserName leer ist muss der Default-Wert ermittelt werden.
        If C_DP.P_TBBenutzer = DataProvider.P_Def_LeerString Then
            tmpUsername = C_DP.P_Def_FritzBoxUser
        Else
            tmpUsername = C_DP.P_TBBenutzer
        End If

        With CType(WebRequest.Create(fbURI), HttpWebRequest)
            .Proxy = Nothing
            .KeepAlive = False
            .CachePolicy = New Cache.HttpRequestCachePolicy(Cache.HttpRequestCacheLevel.BypassCache)
            .Credentials = New NetworkCredential(tmpUsername, C_Crypt.DecryptString128Bit(C_DP.P_TBPasswort, C_DP.GetSettingsVBA("Zugang", DataProvider.P_Def_ErrorMinusOne_String)))
            .Method = WebRequestMethods.Http.Post
            .Headers.Add("SOAPACTION", """" + ServiceType + "#" + SOAPAction + """")
            .ContentType = P_SOAPContentType
            .UserAgent = P_SOAPUserAgent
            .ContentLength = fbPostBytes.Length

            With .GetRequestStream
                .Write(fbPostBytes, 0, fbPostBytes.Length)
                .Close()
            End With

            Try
                With New StreamReader(.GetResponse.GetResponseStream())
                    RetVal.LoadXml(.ReadToEnd())
                End With
            Catch ex As WebException When ex.Message.Contains("606")
                ErrorText = "SOAP Interner-Fehler 606: " & SOAPAction & """ Action not authorized"""
                'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
            Catch ex As WebException When ex.Message.Contains("500")
                ErrorText = "SOAP Interner-Fehler 500: " & SOAPAction & vbNewLine & vbNewLine & "Method: " & .Method.ToString & vbNewLine & "SOAPACTION: " & """" + ServiceType + "#" + SOAPAction + """" & vbNewLine & "ContentType: " & .ContentType.ToString & vbNewLine & "UserAgent: " & .UserAgent.ToString & vbNewLine & "ContentLength: " & .ContentLength.ToString & vbNewLine & vbNewLine & SOAPXML
                'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
            Catch ex As WebException When ex.Message.Contains("713")
                ErrorText = "SOAP Interner-Fehler 713: " & SOAPAction & """ Invalid array index"""
                'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
            Catch ex As WebException When ex.Message.Contains("820")
                ErrorText = "SOAP Interner-Fehler 820: " & SOAPAction & """ Internal error """
                'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
            Catch ex As WebException When ex.Message.Contains("401")
                ErrorText = "SOAP Login-Fehler 401: " & SOAPAction & """ Unauthorized"""
                'MsgBox(ErrorText, MsgBoxStyle.Exclamation)
            End Try
        End With

        If Not ErrorText = "" Then RetVal.LoadXml("<FEHLER>" & ErrorText.Replace("<", "CHR(60)").Replace(">", "CHR(62)") & "</FEHLER>")
        fbURI = Nothing
        Return RetVal
    End Function
#End Region

    Friend Function GetSOAPXMLFile(ByVal Pfad As String) As XmlDocument
        Dim Fehler As Boolean = True
        Dim retVal As String
        Dim XMLFile As New XmlDocument

        retVal = FritzBoxGET(Pfad, Fehler)

        If Not Fehler Then
            XMLFile.LoadXml(retVal)
            GetSOAPXMLFile = XMLFile
        Else
            XMLFile.LoadXml("<FEHLER/>")
            GetSOAPXMLFile = XMLFile
        End If
        XMLFile = Nothing
    End Function

    Friend Function SetupActions(ByVal XMLServiceDefinition As ServiceBaseInformation) As List(Of Action)
        Const BaseTagName As String = "action"
        Const ActionNameTag As String = "name"
        Const ActionArgumentListTag As String = "argumentList"

        Const ArgumentNameTag As String = "name"
        Const ArgumentDirectionTag As String = "direction"
        Const ArgumentRelatedStateVariableTag As String = "relatedStateVariable"

        Dim XMLDefinitionFile As XmlDocument = GetSOAPXMLFile("http://" & C_DP.P_TBFBAdr & ":" & DataProvider.P_Port_FB_SOAP & XMLServiceDefinition.SCPDURL)

        Dim ActionList As New List(Of Action)
        Dim tmpAction As Action

        For Each ActionXMLNode As XmlNode In XMLDefinitionFile.GetElementsByTagName(BaseTagName)
            tmpAction = New Action
            With tmpAction
                .BaseService = XMLServiceDefinition
                .ActionName = ActionXMLNode.Item(ActionNameTag).InnerText
                'Argumente finden
                .ArgumentList = New List(Of Argument)
                If ActionXMLNode.Item(ActionArgumentListTag) IsNot Nothing Then
                    For Each ArgumentXMLNode As XmlNode In ActionXMLNode.Item(ActionArgumentListTag).ChildNodes
                        With ArgumentXMLNode
                            tmpAction.ArgumentList.Add(SetupArgument(.Item(ArgumentNameTag).InnerText, .Item(ArgumentDirectionTag).InnerText, .Item(ArgumentRelatedStateVariableTag).InnerText))
                        End With
                    Next
                End If
            End With
            ActionList.Add(tmpAction)
        Next
        Return ActionList
        tmpAction = Nothing
    End Function

    Friend Function SetupArgument(ByVal ArgumentName As String, ArgumentDirection As String, ArgumentRelatedStateVariable As String) As Argument
        SetupArgument = New Argument
        With SetupArgument
            .Name = ArgumentName
            .Direction = ArgumentDirection
            .RelatedStateVariable = ArgumentRelatedStateVariable
        End With
    End Function

    Friend Function SetupServices(ByVal XMLDefinition As XmlDocument) As List(Of ServiceBaseInformation)
        Const BaseTagName As String = "service"

        Const ElementNameControlURL As String = "controlURL"
        Const ElementNameEventSubURL As String = "eventSubURL"
        Const ElementNameSCPDURL As String = "SCPDURL"
        Const ElementNameServiceId As String = "serviceId"
        Const ElementNameServiceType As String = "serviceType"

        Dim ServiceList As New List(Of ServiceBaseInformation)

        For Each ServiceXMLNode As XmlNode In XMLDefinition.GetElementsByTagName(BaseTagName)
            Using tmpService As ServiceBaseInformation = New ServiceBaseInformation
                With tmpService
                    .controlURL = ServiceXMLNode.Item(ElementNameControlURL).InnerText
                    .eventSubURL = ServiceXMLNode.Item(ElementNameEventSubURL).InnerText
                    .SCPDURL = ServiceXMLNode.Item(ElementNameSCPDURL).InnerText
                    .serviceId = ServiceXMLNode.Item(ElementNameServiceId).InnerText
                    .serviceType = ServiceXMLNode.Item(ElementNameServiceType).InnerText
                End With
                ServiceList.Add(tmpService)
            End Using
        Next

        Return ServiceList
    End Function

    Friend Function SetupStateVariables() As List(Of StateVariable)
        Dim StateVariableList As New List(Of StateVariable)

        Using tmpStateVariable As StateVariable = New StateVariable
            With tmpStateVariable
                .Name = "PersistentData"
                .sendEvents = StateVariableSendEvent.SendEventNO
                .dataType = dataType.dataTypeString
            End With
            StateVariableList.Add(tmpStateVariable)
        End Using

        Using tmpStateVariable As StateVariable = New StateVariable
            With tmpStateVariable
                .Name = "A_ARG_TYPE_UUID"
                .sendEvents = StateVariableSendEvent.SendEventNO
                .dataType = dataType.dataTypeString
            End With
            StateVariableList.Add(tmpStateVariable)
        End Using

        Using tmpStateVariable As StateVariable = New StateVariable
            With tmpStateVariable
                .Name = "A_ARG_TYPE_Status"
                .sendEvents = StateVariableSendEvent.SendEventNO
                .dataType = dataType.dataTypeString
            End With
            StateVariableList.Add(tmpStateVariable)
        End Using

        Using tmpStateVariable As StateVariable = New StateVariable
            With tmpStateVariable
                .Name = "UUID"
                .sendEvents = StateVariableSendEvent.SendEventNO
                .dataType = dataType.dataTypeuuid
            End With
            StateVariableList.Add(tmpStateVariable)
        End Using

        Using tmpStateVariable As StateVariable = New StateVariable
            With tmpStateVariable
                .Name = "X_AVM-DE_Password"
                .sendEvents = StateVariableSendEvent.SendEventNO
                .dataType = dataType.dataTypeuuid
            End With
            StateVariableList.Add(tmpStateVariable)
        End Using

        Using tmpStateVariable As StateVariable = New StateVariable
            With tmpStateVariable
                .Name = "X_AVM-DE_ConfigFileUrl"
                .sendEvents = StateVariableSendEvent.SendEventNO
                .dataType = dataType.dataTypeuuid
            End With
            StateVariableList.Add(tmpStateVariable)
        End Using

        Using tmpStateVariable As StateVariable = New StateVariable
            With tmpStateVariable
                .Name = "X_AVM-DE_UrlSID"
                .sendEvents = StateVariableSendEvent.SendEventNO
                .dataType = dataType.dataTypeString
            End With
            StateVariableList.Add(tmpStateVariable)
        End Using

        Return StateVariableList
    End Function
End Module