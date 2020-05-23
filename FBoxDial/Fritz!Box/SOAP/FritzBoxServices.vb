Imports System.Collections
Imports System.Net
Imports System.Xml

Public Class FritzBoxServices
    Implements IDisposable

    Private Shared Property NLogger As NLog.Logger = LogManager.GetCurrentClassLogger
    Private Property ServiceList As List(Of ServiceBaseInformation)

    Public Sub New()

        ErrorHashTable = New Hashtable
        ServicePointManager.ServerCertificateValidationCallback = New Security.RemoteCertificateValidationCallback(AddressOf AcceptCert)

        ServiceList = SetupServices(GetSOAPXMLFile("http://" & XMLData.POptionen.PTBFBAdr & ":" & FritzBoxDefault.PDfltFBSOAP & KnownSOAPFile.tr64desc))

    End Sub

    Private Function AcceptCert(ByVal sender As Object, ByVal cert As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, ByVal errors As System.Net.Security.SslPolicyErrors) As Boolean
        Return True
    End Function

    Private Function GetServiceBaseInformationbySCPDURL(ByVal SSCPDURL As String) As ServiceBaseInformation

        If ServiceList IsNot Nothing Then
            Return ServiceList.Find(Function(Service) Service.SCPDURL = SSCPDURL)
        Else
            NLogger.Error("SOAP zur Fritz!Box ist nicht bereit: {0}", XMLData.POptionen.PTBFBAdr)
            Return Nothing
        End If

    End Function

    Private Function SetupServices(ByVal XMLDefinition As XmlDocument) As List(Of ServiceBaseInformation)
        Const BaseTagName As String = "service"

        Const ElementNameControlURL As String = "controlURL"
        Const ElementNameEventSubURL As String = "eventSubURL"
        Const ElementNameSCPDURL As String = "SCPDURL"
        Const ElementNameServiceId As String = "serviceId"
        Const ElementNameServiceType As String = "serviceType"

        Dim ServiceList As New List(Of ServiceBaseInformation)

        If XMLDefinition IsNot Nothing Then
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
        Else
            NLogger.Error("SOAP zur Fritz!Box ist nicht bereit: {0}", XMLData.POptionen.PTBFBAdr)
        End If

        Return ServiceList
    End Function

    Friend Overloads Function Start(ByVal SCPDURL As String, ByVal FritzBoxActionName As String, ByVal FritzBoxInputHashTable As Hashtable) As Hashtable
        If Ping(XMLData.POptionen.PTBFBAdr) Then
            Dim tmpSOAPService As New FritzBoxSOAPService(GetServiceBaseInformationbySCPDURL(SCPDURL))
            Dim SOAPError As String

            Start = ErrorHashTable

            If tmpSOAPService.HasAction(FritzBoxActionName) Then
                If tmpSOAPService.CheckInput(FritzBoxActionName, FritzBoxInputHashTable) Then
                    Return tmpSOAPService.GetActionByName(FritzBoxActionName).Start(FritzBoxInputHashTable)
                Else
                    SOAPError = "InputData for Action """ & FritzBoxActionName & """ not valid!"
                End If
            Else
                SOAPError = "Action """ & FritzBoxActionName & """ does not excist!"
            End If

            If SOAPError.IsNotStringEmpty Then
                NLogger.Error(SOAPError)
                With ErrorHashTable
                    .Clear()
                    .Add("Error", SOAPError)
                End With
                Return ErrorHashTable
            End If
        Else
            With ErrorHashTable
                .Clear()
                .Add("Error", "Gegenstelle nicht erreichbar!")
            End With
            Return ErrorHashTable
        End If
    End Function

    Friend Overloads Function Start(ByVal SCPDURL As String, ByVal FritzBoxActionName As String) As Hashtable
        Return Start(SCPDURL, FritzBoxActionName, Nothing)
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
            End If

            ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
            ' TODO: große Felder auf Null setzen.
            ServiceList.Clear()
        End If
        disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(disposing As Boolean) weiter oben Code zur Bereinigung nicht verwalteter Ressourcen enthält.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
        ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
