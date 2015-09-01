Imports System.Collections.Generic
Imports System.Xml
Imports System.Net

Public Module FritzBoxInformations
#Region "Lokale Variablen"
    Private SSLPort As Integer = 49443
    Private FBoxIP As String = "fritz.box"
    Private FBoxUser As String
    Private FBoxPasswort As String
#End Region
    Friend ErrorHashTable As Hashtable
#Region "Properties"
    ''' <summary>
    ''' Enthält eine gültige Fritz!Box Adresse.
    ''' </summary>
    Friend Property P_FritzBox_IP() As String
        Set(value As String)
            FBoxIP = value
        End Set
        Get
            Return FBoxIP
        End Get
    End Property

    Friend Property P_FritzBox_UserName As String
        Set(value As String)
            FBoxUser = value
        End Set
        Get
            Return FBoxUser
        End Get
    End Property

    Friend Property P_FritzBox_Passwort As String
        Set(value As String)
            FBoxPasswort = value
        End Set
        Get
            Return FBoxPasswort
        End Get
    End Property

    Friend ReadOnly Property P_Port_FB_SOAP() As Integer
        Get
            Return 49000
        End Get
    End Property

    Friend Property P_Port_FB_SOAP_SSL() As Integer
        Get
            Return SSLPort
        End Get
        Set(value As Integer)
            SSLPort = value
        End Set
    End Property

    Friend ReadOnly Property P_SOAPContentType As String
        Get
            Return "text/xml; charset=""utf-8"""
        End Get
    End Property

    Friend ReadOnly Property P_SOAPUserAgent As String
        Get
            Return "AVM UPnP/1.0 Client 1.0"
        End Get
    End Property
#End Region

#Region "Fritz!Box UPnP/TR-064 Files"

    ''' <summary>
    ''' Gibt die SCPDURL der bekannten Services zurück.
    ''' </summary>
    Public Structure KnownSOAPFile
#If OVer = 11 Then
        Private Dummy As String
#End If
        ''' <summary>
        ''' deviceconfigSCPD
        ''' </summary>
        Shared deviceconfigSCPD As String = "/deviceconfigSCPD.xml"

        ''' <summary>
        ''' deviceinfoSCPD
        ''' </summary>
        Shared deviceinfoSCPD As String = "/deviceinfoSCPD.xml"

        ''' <summary>
        ''' lanconfigsecuritySCPD
        ''' </summary>
        Shared lanconfigsecuritySCPD As String = "/lanconfigsecuritySCPD.xml"

        ''' <summary>
        ''' lanhostconfigmgmSCPD
        ''' </summary>
        Shared lanhostconfigmgmSCPD As String = "/lanhostconfigmgmSCPD.xml"

        ''' <summary>
        ''' layer3forwardingSCPD
        ''' </summary>
        Shared layer3forwardingSCPD As String = "/layer3forwardingSCPD.xml"

        ''' <summary>
        ''' mgmsrvSCPD
        ''' </summary>
        Shared mgmsrvSCPD As String = "/mgmsrvSCPD.xml"

        ''' <summary>
        ''' timeSCPD
        ''' </summary>
        Shared timeSCPD As String = "/timeSCPD.xml"

        ''' <summary>
        ''' timeSCPDuserifSCPD
        ''' </summary>
        Shared userifSCPD As String = "/userifSCPD.xml"

        ''' <summary>
        ''' wancommonifconfigSCPD
        ''' </summary>
        Shared wancommonifconfigSCPD As String = "/wancommonifconfigSCPD.xml"

        ''' <summary>
        ''' wandslifconfigSCPD
        ''' </summary>
        Shared wandslifconfigSCPD As String = "/wandslifconfigSCPD.xml"

        ''' <summary>
        ''' wandsllinkconfigSCPD
        ''' </summary>
        Shared wandsllinkconfigSCPD As String = "/wandsllinkconfigSCPD.xml"

        ''' <summary>
        ''' wanpppconnSCPD
        ''' </summary>
        Shared wanpppconnSCPD As String = "/wanpppconnSCPD.xml"

        ''' <summary>
        ''' wanipconnSCPD
        ''' </summary>
        Shared wanipconnSCPD As String = "/wanipconnSCPD.xml"

        ''' <summary>
        ''' wlanconfigSCPD
        ''' </summary>
        Shared wlanconfigSCPD As String = "/wlanconfigSCPD.xml"

        ''' <summary>
        ''' hostsSCPD
        ''' </summary>
        Shared hostsSCPD As String = "/hostsSCPD.xml"

        ''' <summary>
        ''' lanifconfigSCPD
        ''' </summary>
        Shared lanifconfigSCPD As String = "/lanifconfigSCPD.xml"

        ''' <summary>
        ''' wanethlinkconfigSCPD
        ''' </summary>
        Shared wanethlinkconfigSCPD As String = "/wanethlinkconfigSCPD.xml"

        ''' <summary>
        ''' x_upnpSCPD
        ''' </summary>
        Shared x_upnpSCPD As String = "/x_upnpSCPD.xml"

        ''' <summary>
        ''' x_webdavSCPD
        ''' </summary>
        Shared x_contactSCPD As String = "/x_contactSCPD.xml"

        ''' <summary>
        ''' x_myfritzSCPD
        ''' </summary>
        Shared x_myfritzSCPD As String = "/x_myfritzSCPD.xml"

        ''' <summary>
        ''' x_storageSCPD
        ''' </summary>
        Shared x_storageSCPD As String = "/x_storageSCPD.xml"

        ''' <summary>
        ''' x_remoteSCPD
        ''' </summary>
        Shared x_remoteSCPD As String = "/x_remoteSCPD.xml"

        ''' <summary>
        ''' x_tamSCPD
        ''' </summary>
        Shared x_tamSCPD As String = "/x_tamSCPD.xml"

        ''' <summary>
        ''' x_voipSCPD
        ''' </summary>
        Shared x_voipSCPD As String = "/x_voipSCPD.xml"

        ''' <summary>
        ''' x_webdavSCPD
        ''' </summary>
        Shared x_webdavSCPD As String = "/x_webdavSCPD.xml"

        ''' <summary>
        ''' igddesc
        ''' </summary>
        Shared igddesc As String = "/igddesc.xml"

        ''' <summary>
        ''' any
        ''' </summary>
        Shared any As String = "/any.xml"

        ''' <summary>
        ''' x_webdavSCPD
        ''' </summary>
        Shared igdicfgSCPD As String = "/igdicfgSCPD.xml"

        ''' <summary>
        ''' x_webdavSCPD
        ''' </summary>
        Shared igddslSCPD As String = "/igddslSCPD.xml"

        ''' <summary>
        ''' igdconnSCPD
        ''' </summary>
        Shared igdconnSCPD As String = "/igdconnSCPD.xml"

        ''' <summary>
        ''' tr64desc
        ''' </summary>
        Shared tr64desc As String = "/tr64desc.xml"

    End Structure

#End Region
End Module

Public Class FritzBoxServices

    Private ServiceList As List(Of ServiceBaseInformation)

    Public Sub New()
        ErrorHashTable = New Hashtable
        ServicePointManager.ServerCertificateValidationCallback = New System.Net.Security.RemoteCertificateValidationCallback(AddressOf AcceptCert)
    End Sub

    Public Sub SetFritzBoxData(ByVal FritzBoxIP As String, ByVal FritzBoxUserName As String, ByVal FritzBoxPasswort As String)
        P_FritzBox_IP = FritzBoxIP
        P_FritzBox_UserName = FritzBoxUserName
        P_FritzBox_Passwort = FritzBoxPasswort

        ServiceList = SetupServices(GetSOAPXMLFile("http://" & P_FritzBox_IP & ":" & P_Port_FB_SOAP & KnownSOAPFile.tr64desc))
    End Sub
    Private Function AcceptCert(ByVal sender As Object, ByVal cert As System.Security.Cryptography.X509Certificates.X509Certificate, _
                ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, _
                ByVal errors As System.Net.Security.SslPolicyErrors) As Boolean
        Return True
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Function GetServiceBaseInformationbySCPDURL(ByVal SSCPDURL As String) As ServiceBaseInformation
        Return ServiceList.Find(Function(Service) Service.SCPDURL = SSCPDURL)
    End Function

    Public Overloads Function Start(ByVal SCPDURL As String, ByVal FritzBoxActionName As String, ByVal FritzBoxInputHashTable As Hashtable) As Hashtable

        Dim tmpSOAPService As New FritzBoxSOAPService(GetServiceBaseInformationbySCPDURL(SCPDURL))
        Dim SOAPError As String = ""

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

        If Not SOAPError = "" Then
            With ErrorHashTable
                .Clear()
                .Add("Error", SOAPError)
            End With
            Return ErrorHashTable
        End If
    End Function

    Public Overloads Function Start(ByVal SCPDURL As String, ByVal FritzBoxActionName As String) As Hashtable
        Return Start(SCPDURL, FritzBoxActionName, Nothing)
    End Function
End Class

Friend Class FritzBoxSOAPService
    Private ServiceDefinition As ServiceBaseInformation

    Private ActionList As List(Of Action)
    Private StateVariableList As List(Of StateVariable)

    Public Sub New(ByVal XMLServiceDefinition As ServiceBaseInformation)
        ' Lokal Sichern
        ServiceDefinition = XMLServiceDefinition

        ActionList = SetupActions(XMLServiceDefinition)
        'StateVariableList = SetupStateVariables()
    End Sub

    Friend Function GetActionByName(ByVal ActionName As String) As Action
        Return ActionList.Find(Function(GetbyActionName) GetbyActionName.ActionName = ActionName)
    End Function

    Friend Function HasAction(ByVal ActionName As String) As Boolean
        Return Not IsNothing(ActionList.Find(Function(GetbyActionName) GetbyActionName.ActionName = ActionName))
    End Function

    Friend Function CheckInput(ByVal ActionName As String, ByVal InputData As Hashtable) As Boolean
        CheckInput = False
        Dim ActionInputData As Hashtable = GetActionByName(ActionName).GetInputArguments
        Dim idx As Integer = 0
        If InputData Is Nothing Then
            If ActionInputData.Count = 0 Then
                CheckInput = True
            End If
        Else
            ' Prüfe Anzahl der zu übergebenden Daten
            If ActionInputData.Count = InputData.Count Then
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
        ActionInputData = Nothing
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
