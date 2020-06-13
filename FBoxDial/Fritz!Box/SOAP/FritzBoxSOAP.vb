Imports System.Collections
Imports System.Net

Public Class FritzBoxSOAP
    Implements IDisposable

    Private Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property FBTR64Desc As TR64Desc
    Private Property HttpFehler As Boolean
    Public Sub New()
        Dim HttpResponse As String

        HttpFehler = False
        ErrorHashTable = New Hashtable

        ' ByPass SSL Certificate Validation Checking
        ServicePointManager.ServerCertificateValidationCallback = Function(se As Object, cert As System.Security.Cryptography.X509Certificates.X509Certificate, chain As System.Security.Cryptography.X509Certificates.X509Chain, sslerror As Security.SslPolicyErrors) True

        ' Funktioniert nicht: ByPass SSL Certificate Validation Checking wird ignoriert. Es kommt zu unerklärlichen System.Net.WebException in FritzBoxPOST
        ' FBTR64Desc = DeserializeObject(Of TR64Desc)($"http://{XMLData.POptionen.PTBFBAdr}:{FritzBoxDefault.PDfltFBSOAP}{KnownSOAPFile.tr64desc}")

        ' Workaround: XML-Datei als String herunterladen und separat Deserialisieren
        ' Herunterladen
        HttpResponse = FritzBoxGet($"http://{XMLData.POptionen.PTBFBAdr}:{FritzBoxDefault.PDfltSOAPPort}{KnownSOAPFile.tr64desc}", HttpFehler)
        ' Deserialisieren
        If Not HttpFehler Then FBTR64Desc = XmlDeserializeFromString(Of TR64Desc)(HttpResponse)
    End Sub

    Private Function GetService(ByVal SCPDURL As String) As Service

        If FBTR64Desc IsNot Nothing AndAlso FBTR64Desc.Device.ServiceList.Any Then
            Return FBTR64Desc.Device.ServiceList.Find(Function(Service) Service.SCPDURL.AreEqual(SCPDURL))
        Else
            NLogger.Error("SOAP zur Fritz!Box ist nicht bereit: {0}", XMLData.POptionen.PTBFBAdr)
            Return Nothing
        End If

    End Function

    Friend Overloads Function Start(ByVal SCPDURL As String, ByVal ActionName As String, ByVal InputHashTable As Hashtable) As Hashtable
        If Ping(XMLData.POptionen.PTBFBAdr) Then

            Dim SOAPError As String

            With GetService(SCPDURL)
                If .ActionExists(ActionName) Then
                    If .CheckInput(ActionName, InputHashTable) Then
                        Return .Start(.GetActionByName(ActionName), InputHashTable)
                    Else
                        SOAPError = $"InputData for Action ""{ActionName}"" not valid!"
                    End If
                Else
                    SOAPError = $"Action ""{ActionName}"" does not exist!"
                End If
            End With

            If SOAPError.IsNotStringEmpty Then
                NLogger.Error(SOAPError)
                With ErrorHashTable
                    .Clear()
                    .Add("Error", SOAPError)
                End With
            End If
        Else
            With ErrorHashTable
                .Clear()
                .Add("Error", "Gegenstelle nicht erreichbar!")
            End With
        End If
        Return ErrorHashTable
    End Function

    Friend Overloads Function Start(ByVal SCPDURL As String, ByVal FritzBoxActionName As String) As Hashtable
        Return Start(SCPDURL, FritzBoxActionName, Nothing)
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            'Restore SSL Certificate Validation Checking
            ServicePointManager.ServerCertificateValidationCallback = Nothing
        End If
        disposedValue = True
    End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
        Dispose(True)
    End Sub
#End Region

End Class
