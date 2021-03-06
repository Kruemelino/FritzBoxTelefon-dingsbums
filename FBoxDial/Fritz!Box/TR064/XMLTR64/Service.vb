﻿Imports System.Collections
Imports System.Net
Imports System.Xml
Imports System.Xml.Serialization

Namespace SOAP
    '<DebuggerStepThrough>
    <Serializable()>
    Public Class Service

        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

        <XmlElement("serviceType")> Public Property ServiceType As String
        <XmlElement("serviceId")> Public Property ServiceId As String
        <XmlElement("controlURL")> Public Property ControlURL As String
        <XmlElement("eventSubURL")> Public Property EventSubURL As String
        <XmlElement("SCPDURL")> Public Property SCPDURL As String

        <XmlIgnore> Friend Property SCPD As ServiceControlProtocolDefinition
        <XmlIgnore> Friend Property FBoxIPAdresse As String

        Friend Function GetActionByName(ActionName As String) As Action
            Return SCPD?.ActionList.Find(Function(Action) Action.Name = ActionName)
        End Function

        ''' <summary>
        ''' Proft, ob die geforderte Action mit dem <paramref name="ActionName"/> existiert.
        ''' </summary>
        ''' <param name="ActionName">Name der auszuführenden Action.</param>
        ''' <returns>Boolean</returns>
        Friend Function ActionExists(ActionName As String) As Boolean

            ' Wenn ServiceControlProtocolDefinition noch nicht geladen wurde, dann lade sie von der Fritz!Box
            If SCPD Is Nothing Then
                ' Wenn keine IPAddresse vorhanden ist, was eigentlich nicht möglich ist, dann wirf einen Fehler aus.
                If FBoxIPAdresse.IsStringNothingOrEmpty Then
                    NLogger.Error($"Action '{ActionName}': IP Adresse nicht vorhanden.")
                Else
                    If Not DeserializeXML($"{Uri.UriSchemeHttp}://{FBoxIPAdresse}:{DfltTR064Port}{SCPDURL}", True, SCPD) Then
                        ' Fehlerfall
                        NLogger.Error($"Action '{ActionName}': ServiceControlProtocolDefinition nicht geladen.")
                    End If
                End If
            End If

            Return SCPD IsNot Nothing AndAlso SCPD.ActionList.Exists(Function(Action) Action.Name = ActionName)

        End Function

        Friend Function CheckInput(ActionName As String, InputData As Hashtable) As Boolean
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

        Friend Function Start([Action] As Action, InputArguments As Hashtable, NetworkCredential As NetworkCredential) As Hashtable
            Dim ReturnXMLDoc As New XmlDocument
            Dim OutputHashTable As New Hashtable
            Dim Response As String = DfltStringEmpty

            With OutputHashTable

                If FBoxIPAdresse.IsStringNothingOrEmpty Then
                    ' Wenn keine IPAddresse vorhanden ist, was eigentlich nicht möglich ist, dann wirf einen Fehler aus.
                    NLogger.Error($"Action '{[Action]}': IP Adresse nicht vorhanden.")
                    .Add("Error", DfltStringEmpty)
                Else
                    ' Header festlegen
                    Dim TR064Header As New WebHeaderCollection From {{HttpRequestHeader.ContentType, TR064ContentType},
                                                                     {HttpRequestHeader.UserAgent, TR064UserAgent},
                                                                     {"SOAPACTION", $"""{ServiceType}#{Action.Name}"""}}

                    If UploadData(New UriBuilder(Uri.UriSchemeHttps, FBoxIPAdresse, DfltTR064PortSSL, ControlURL).Uri,
                                  GetRequest(Action, InputArguments).InnerXml,
                                  NetworkCredential,
                                  Response,
                                  TR064Header) Then

                        NLogger.Trace($"Action {Action.Name}: {Response}")

                        Try
                            ReturnXMLDoc.LoadXml(Response)
                        Catch ex As XmlException
                            ' Fehlerfall
                            .Add("Error", Response)
                            NLogger.Error(ex, Response)
                        End Try

                        ' TODO: Serialisieren
                        If ReturnXMLDoc.InnerXml.IsNotStringEmpty Then
                            For Each OUTArguments As Argument In Action.ArgumentList.FindAll(Function(GetbyDirection) GetbyDirection.Direction = ArgumentDirection.OUT)
                                .Add(OUTArguments.Name, ReturnXMLDoc.GetElementsByTagName(OUTArguments.Name).Item(0).InnerText)
                            Next
                        End If

                    Else
                        ' Fehlerfall
                        .Add("Error", Response)
                        NLogger.Error(Response)
                    End If

                End If
            End With

            Return OutputHashTable
        End Function

        ''' <summary>
        ''' Erstellt den XML-Request für die jeweilige Action 
        ''' </summary>
        ''' <param name="Action">Die <paramref name="Action"/>, die ausgeführt werden soll.</param>
        ''' <param name="InputValues">Die Daten, welche müt übergeben werden sollen.</param>
        Private Function GetRequest(Action As Action, InputValues As Hashtable) As XmlDocument

            GetRequest = New XmlDocument

            With GetRequest
                ' XML-Schemata hinzufügen
                .Schemas.Add(DfltSOAPRequestSchema)

                ' XML Deklaration hinzufügen
                .AppendChild(.CreateXmlDeclaration("1.0", "utf-8", ""))

                ' XML-RootElement "Envelope" generieren
                With .AppendChild(.CreateElement("s", "Envelope", DfltTR064RequestNameSpaceEnvelope))
                    ' Das Attribut "encodingStyle" dem XML-Root-Element hinzufügen
                    With .Attributes.Append(GetRequest.CreateAttribute("s", "encodingStyle", DfltTR064RequestNameSpaceEnvelope))
                        ' Den Wert des Attributes "encodingStyle" setzen
                        .Value = DfltTR064RequestNameSpaceEncoding
                    End With

                    ' XML-BodyElement "Body" generieren und dem XML-RootElement anhängen
                    With .AppendChild(GetRequest.CreateElement("s", "Body", DfltTR064RequestNameSpaceEnvelope))

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

End Namespace
