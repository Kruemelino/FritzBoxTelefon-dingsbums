Imports System.Collections
Imports System.Xml

Friend Structure ArgumentDirection_ALT
    Friend Shared directionIN As String = "in"
    Friend Shared directionOUT As String = "out"
End Structure

Friend Structure DataType
    Friend Shared dataTypeString As String = "string"
    Friend Shared dataTypeuuid As String = "uuid"
End Structure

Friend Structure StateVariableSendEvent_ALT
    Friend Shared SendEventYES As String = "yes"
    Friend Shared SendEventNO As String = "no"
End Structure

Friend Class Action_ALT
    Implements IDisposable
    Friend BaseService As ServiceBaseInformation_ALT
    Friend ActionName As String
    Friend ArgumentList As New List(Of Argument_ALT)

    Friend Function GetInputArguments() As Hashtable
        Dim InputHashTable As New Hashtable
        For Each INArguments As Argument_ALT In ArgumentList.FindAll(Function(GetbyDirection) GetbyDirection.Direction = ArgumentDirection.IN)
            InputHashTable.Add(INArguments.Name, "")
        Next
        Return InputHashTable
    End Function

    Friend Function Start(ByVal InputArguments As Hashtable) As Hashtable
        Dim ReturnXMLDox As New XmlDocument
        Dim OutputHashTable As New Hashtable

        ReturnXMLDox.LoadXml(FritzBoxPOST(ActionName, $"https://{XMLData.POptionen.TBFBAdr }:{FritzBoxDefault.DfltSOAPPortSSL}{BaseService.controlURL}", BaseService.serviceType, GetSOAPRequest(InputArguments)))

        If ReturnXMLDox.DocumentElement.Name.AreEqual("FEHLER") Then
            With ErrorHashTable
                .Clear()
                .Add("Error", ReturnXMLDox.DocumentElement.InnerText)
            End With
            OutputHashTable = ErrorHashTable
        Else
            If ReturnXMLDox.InnerXml.IsNotStringEmpty Then
                For Each OUTArguments As Argument_ALT In ArgumentList.FindAll(Function(GetbyDirection) GetbyDirection.Direction = ArgumentDirection.OUT)
                    OutputHashTable.Add(OUTArguments.Name, ReturnXMLDox.GetElementsByTagName(OUTArguments.Name).Item(0).InnerText)
                Next
            End If
        End If

        Return OutputHashTable
    End Function

    ''' <summary>
    ''' Stellt den SOAP Request bereit
    ''' </summary>
    Private Function GetSOAPRequest(ByVal submitValues As Hashtable) As XmlDocument

        Dim BaseNSs As String = "http://schemas.xmlsoap.org/soap/envelope/"
        Dim BaseEnc As String = "http://schemas.xmlsoap.org/soap/encoding/"

        Dim XMLSOAPRequest As New XmlDocument
        Dim XMLSOAPSchema As New Schema.XmlSchema

        Dim rootXMLElement As XmlElement
        Dim XMLNodeAction As XmlNode

        Dim tmpXMLElement As XmlElement

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

            XMLNodeAction = tmpXMLElement.AppendChild(.CreateElement("u", ActionName, BaseService.serviceType))

            If Not submitValues Is Nothing Then
                For Each submitItem As DictionaryEntry In submitValues
                    tmpXMLElement = .CreateElement("u", CStr(submitItem.Key), BaseService.serviceType)
                    tmpXMLElement.InnerText = submitItem.Value.ToString
                    XMLNodeAction.AppendChild(tmpXMLElement)
                Next
            End If

            .AppendChild(rootXMLElement)
        End With

        Return XMLSOAPRequest
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

Friend Class Argument_ALT
    Friend Name As String
    Friend Direction As String
    Friend RelatedStateVariable As String
End Class

Friend Class ServiceBaseInformation_ALT
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

Friend Class StateVariable_ALT
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
