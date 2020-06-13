Imports System.Collections
Imports System.Xml
Imports System.Xml.Serialization

<Serializable()>
Public Class Service
    <XmlElement("serviceType")> Public Property ServiceType As String
    <XmlElement("serviceId")> Public Property ServiceId As String
    <XmlElement("controlURL")> Public Property ControlURL As String
    <XmlElement("eventSubURL")> Public Property EventSubURL As String
    <XmlElement("SCPDURL")> Public Property SCPDURL As String

    <XmlIgnore> Friend Property SCPD As ServiceControlProtocolDefinition

    Friend Function GetActionByName(ByVal ActionName As String) As Action
        Return SCPD?.ActionList.Find(Function(Action) Action.Name = ActionName)
    End Function

    Friend Function ActionExists(ByVal ActionName As String) As Boolean
        If SCPD Is Nothing Then
            SCPD = DeserializeObject(Of ServiceControlProtocolDefinition)($"http://{XMLData.POptionen.PTBFBAdr}:{FritzBoxDefault.PDfltSOAPPort}{SCPDURL}")
        End If

        Return SCPD.ActionList.Exists(Function(Action) Action.Name = ActionName)

    End Function

    Friend Function CheckInput(ByVal ActionName As String, ByVal InputData As Hashtable) As Boolean
        CheckInput = False
        Dim ActionInputData As Hashtable = GetActionByName(ActionName).GetInputArguments

        If InputData Is Nothing Then
            If ActionInputData.Count.IsZero Then
                CheckInput = True
            End If
        Else
            ' Prüfe Anzahl der zu übergebenden Daten
            If ActionInputData.Count.AreEqual(InputData.Count) Then
                CheckInput = True
                For Each submitItem As DictionaryEntry In ActionInputData
                    If Not InputData.ContainsKey(submitItem.Key) Then
                        CheckInput = False
                        Exit For
                    End If
                Next
            End If

        End If
        ActionInputData.Clear()
    End Function

    Friend Function Start(ByVal [Action] As Action, ByVal InputArguments As Hashtable) As Hashtable
        Dim ReturnXMLDox As New XmlDocument
        Dim OutputHashTable As New Hashtable

        ReturnXMLDox.LoadXml(FritzBoxPOST(Action.Name, $"https://{XMLData.POptionen.PTBFBAdr }:{FritzBoxDefault.PDfltSOAPPortSSL}{ControlURL}", ServiceType, GetRequest(Action, InputArguments)))

        If ReturnXMLDox.DocumentElement.Name.AreEqual("FEHLER") Then
            With ErrorHashTable
                .Clear()
                .Add("Error", ReturnXMLDox.DocumentElement.InnerText)
            End With
            OutputHashTable = ErrorHashTable
        Else
            If ReturnXMLDox.InnerXml.IsNotStringEmpty Then
                For Each OUTArguments As Argument In Action.ArgumentList.FindAll(Function(GetbyDirection) GetbyDirection.Direction = ArgumentDirection.OUT)
                    OutputHashTable.Add(OUTArguments.Name, ReturnXMLDox.GetElementsByTagName(OUTArguments.Name).Item(0).InnerText)
                Next
            End If
        End If

        Return OutputHashTable
    End Function

    ''' <summary>
    ''' Erstellt den XML-Request für die jeweilige Action 
    ''' </summary>
    ''' <param name="Action">Die <paramref name="Action"/>, die ausgeführt werden soll.</param>
    ''' <param name="InputValues">Die Daten, welche müt übergeben werden sollen.</param>
    Private Function GetRequest(ByVal Action As Action, ByVal InputValues As Hashtable) As XmlDocument

        '	<?xml version="1.0" encoding="utf-8"?>
        '	<s:Envelope s:encodingStyle="http://schemas.xmlsoap.org/soap/encoding/" xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
        '		<s:Body>
        '			<u:GetPhonebook xmlns:u="urn:dslforum-org:service:X_AVM-DE_OnTel:1">
        '				<u:NewPhonebookID>0</u:NewPhonebookID>
        '			</u:GetPhonebook>
        '		</s:Body>
        '	</s:Envelope> 

        GetRequest = New XmlDocument

        With GetRequest
            ' XML-Schemata hinzufügen
            .Schemas.Add(FritzBoxDefault.PDfltSOAPRequestSchema)

            ' XML Deklaration hinzufügen
            .AppendChild(.CreateXmlDeclaration("1.0", "utf-8", ""))

            ' XML-RootElement "Envelope" generieren
            With .AppendChild(.CreateElement("s", "Envelope", FritzBoxDefault.PDfltSOAPRequestNameSpaceEnvelope))
                ' Das Attribut "encodingStyle" dem XML-Root-Element hinzufügen
                With .Attributes.Append(GetRequest.CreateAttribute("s", "encodingStyle", FritzBoxDefault.PDfltSOAPRequestNameSpaceEnvelope))
                    ' Den Wert des Attributes "encodingStyle" setzen
                    .Value = FritzBoxDefault.PDfltSOAPRequestNameSpaceEncoding
                End With

                ' XML-BodyElement "Body" generieren und dem XML-RootElement anhängen
                With .AppendChild(GetRequest.CreateElement("s", "Body", FritzBoxDefault.PDfltSOAPRequestNameSpaceEnvelope))

                    ' XML-Element mit dem namen der Action generieren und dem XML-BodyElement anhängen
                    With .AppendChild(GetRequest.CreateElement("u", Action.Name, ServiceType))

                        ' Die zu übergebenden Daten generieren, falls welche übergeben werden sollen
                        If InputValues IsNot Nothing Then
                            ' Schleife durch jedes Wertepaar
                            For Each submitItem As DictionaryEntry In InputValues

                                ' XML-Element mit dem namen des Inputwertes generieren und dem XML-Action Element anhängen
                                With .AppendChild(GetRequest.CreateElement("u", CStr(submitItem.Key), ServiceType))
                                    .InnerText = submitItem.Value.ToString
                                End With ' InputValue XML Element
                            Next
                        End If

                    End With ' XML-ActionElement 

                End With ' XML-BodyElement 

            End With ' XML-RootElement 

        End With ' XML Document GetRequest

    End Function
End Class
