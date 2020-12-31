Imports System.Collections
Imports System.Xml

Friend Class FritzBoxSOAPService

    Private Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger
    'Private Property ServiceDefinition As ServiceBaseInformation
    Private Property ActionList As List(Of Action_ALT)
    'Private Property StateVariableList As List(Of StateVariable)

    Public Sub New(ByVal XMLServiceDefinition As ServiceBaseInformation_ALT)
        'ServiceDefinition = XMLServiceDefinition
        If XMLServiceDefinition IsNot Nothing Then
            ActionList = SetupActions(XMLServiceDefinition)
        Else
            NLogger.Error("XMLServiceDefinition nicht verfügbar")
        End If

        'StateVariableList = SetupStateVariables()
    End Sub

    Friend Function GetActionByName(ByVal ActionName As String) As Action_ALT
        Return ActionList.Find(Function(GetbyActionName) GetbyActionName.ActionName = ActionName)
    End Function

    Friend Function HasAction(ByVal ActionName As String) As Boolean
        Return Not IsNothing(ActionList.Find(Function(GetbyActionName) GetbyActionName.ActionName = ActionName))
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
        'ActionInputData = Nothing
    End Function

    Private Function SetupActions(ByVal XMLServiceDefinition As ServiceBaseInformation_ALT) As List(Of Action_ALT)
        Const BaseTagName As String = "action"
        Const ActionNameTag As String = "name"
        Const ActionArgumentListTag As String = "argumentList"

        Const ArgumentNameTag As String = "name"
        Const ArgumentDirectionTag As String = "direction"
        Const ArgumentRelatedStateVariableTag As String = "relatedStateVariable"

        Dim XMLDefinitionFile As XmlDocument = GetSOAPXMLFile("http://" & XMLData.POptionen.TBFBAdr & ":" & FritzBoxDefault.DfltSOAPPort & XMLServiceDefinition.SCPDURL)

        Dim ActionList As New List(Of Action_ALT)
        Dim tmpAction As Action_ALT

        For Each ActionXMLNode As XmlNode In XMLDefinitionFile.GetElementsByTagName(BaseTagName)
            tmpAction = New Action_ALT
            With tmpAction
                .BaseService = XMLServiceDefinition
                .ActionName = ActionXMLNode.Item(ActionNameTag).InnerText
                'Argumente finden
                .ArgumentList = New List(Of Argument_ALT)
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

    Private Function SetupArgument(ByVal ArgumentName As String, ArgumentDirection As String, ArgumentRelatedStateVariable As String) As Argument_ALT
        SetupArgument = New Argument_ALT With {.Direction = ArgumentDirection, .Name = ArgumentName, .RelatedStateVariable = ArgumentRelatedStateVariable}
    End Function

    'Private Function SetupStateVariables() As List(Of StateVariable)
    '    Dim StateVariableList As New List(Of StateVariable)

    '    Using tmpStateVariable As StateVariable = New StateVariable
    '        With tmpStateVariable
    '            .Name = "PersistentData"
    '            .sendEvents = StateVariableSendEvent.SendEventNO
    '            .dataType = DataType.dataTypeString
    '        End With
    '        StateVariableList.Add(tmpStateVariable)
    '    End Using

    '    Using tmpStateVariable As StateVariable = New StateVariable
    '        With tmpStateVariable
    '            .Name = "A_ARG_TYPE_UUID"
    '            .sendEvents = StateVariableSendEvent.SendEventNO
    '            .dataType = DataType.dataTypeString
    '        End With
    '        StateVariableList.Add(tmpStateVariable)
    '    End Using

    '    Using tmpStateVariable As StateVariable = New StateVariable
    '        With tmpStateVariable
    '            .Name = "A_ARG_TYPE_Status"
    '            .sendEvents = StateVariableSendEvent.SendEventNO
    '            .dataType = DataType.dataTypeString
    '        End With
    '        StateVariableList.Add(tmpStateVariable)
    '    End Using

    '    Using tmpStateVariable As StateVariable = New StateVariable
    '        With tmpStateVariable
    '            .Name = "UUID"
    '            .sendEvents = StateVariableSendEvent.SendEventNO
    '            .dataType = DataType.dataTypeuuid
    '        End With
    '        StateVariableList.Add(tmpStateVariable)
    '    End Using

    '    Using tmpStateVariable As StateVariable = New StateVariable
    '        With tmpStateVariable
    '            .Name = "X_AVM-DE_Password"
    '            .sendEvents = StateVariableSendEvent.SendEventNO
    '            .dataType = DataType.dataTypeuuid
    '        End With
    '        StateVariableList.Add(tmpStateVariable)
    '    End Using

    '    Using tmpStateVariable As StateVariable = New StateVariable
    '        With tmpStateVariable
    '            .Name = "X_AVM-DE_ConfigFileUrl"
    '            .sendEvents = StateVariableSendEvent.SendEventNO
    '            .dataType = DataType.dataTypeuuid
    '        End With
    '        StateVariableList.Add(tmpStateVariable)
    '    End Using

    '    Using tmpStateVariable As StateVariable = New StateVariable
    '        With tmpStateVariable
    '            .Name = "X_AVM-DE_UrlSID"
    '            .sendEvents = StateVariableSendEvent.SendEventNO
    '            .dataType = DataType.dataTypeString
    '        End With
    '        StateVariableList.Add(tmpStateVariable)
    '    End Using

    '    Return StateVariableList
    'End Function
End Class

