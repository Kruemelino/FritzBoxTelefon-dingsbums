Imports System.Net

Public Module FritzBoxInformations
#Region "Lokale Variablen"
    Private DP As DataProvider
    Private Crypt As Rijndael
#End Region
    Friend ErrorHashTable As Hashtable
#Region "Properties"
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

    Friend Property C_DP As DataProvider
        Set(value As DataProvider)
            DP = value
        End Set
        Get
            Return DP
        End Get
    End Property

    Friend Property C_Crypt As Rijndael
        Set(value As Rijndael)
            Crypt = value
        End Set
        Get
            Return Crypt
        End Get
    End Property

#End Region

#Region "Fritz!Box UPnP/TR-064 Files"

    ''' <summary>
    ''' Gibt die SCPDURL der bekannten Services zurück.
    ''' </summary>
    Public Structure KnownSOAPFile
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
        ''' X_appsetup
        ''' </summary>
        Shared X_appsetup As String = "/x_appsetupSCPD.xml"

        ''' <summary>
        ''' X_homeautoSCPD
        ''' </summary>
        Shared X_homeautoSCPD As String = "/x_homeautoSCPD.xml"

        ''' <summary>
        ''' X_homeplugSCPD
        ''' </summary>
        Shared X_homeplugSCPD As String = "/x_homeplugSCPD.xml"

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

        ''' <summary>
        ''' tr64desc
        ''' </summary>
        Shared x_speedtestSCPD As String = "/x_speedtestSCPD.xml"

        ''' <summary>
        ''' tr64desc
        ''' </summary>
        Shared x_dectSCPD As String = "/x_dectSCPD.xml"

        ''' <summary>
        ''' tr64desc
        ''' </summary>
        Shared x_filelinksSCPD As String = "/x_filelinksSCPD.xml"

        ''' <summary>
        ''' tr64desc
        ''' </summary>
        Shared x_authSCPD As String = "/x_authSCPD.xml"
    End Structure

#End Region
End Module

Public Class FritzBoxServices
    Private ServiceList As List(Of ServiceBaseInformation)

    Public Sub New(ByVal Datenhalter As DataProvider, ByVal CryptKlasse As Rijndael)

        C_DP = Datenhalter
        C_Crypt = CryptKlasse

        ErrorHashTable = New Hashtable
        ServicePointManager.ServerCertificateValidationCallback = New Security.RemoteCertificateValidationCallback(AddressOf AcceptCert)

        ' Lade Services
        ServiceList = SetupServices(GetSOAPXMLFile("http://" & C_DP.P_TBFBAdr & ":" & DataProvider.P_Port_FB_SOAP & KnownSOAPFile.tr64desc))
    End Sub

    Private Function AcceptCert(ByVal sender As Object, ByVal cert As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, ByVal errors As System.Net.Security.SslPolicyErrors) As Boolean
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
