﻿Imports System.Collections
Imports System.Net

Friend Class FritzBoxTR64
    Implements IDisposable

    Private Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property FBTR64Desc As TR64Desc

    Public Sub New()
        Dim Response As String = DfltStringEmpty

        ErrorHashTable = New Hashtable

        ' ByPass SSL Certificate Validation Checking
        ServicePointManager.ServerCertificateValidationCallback = Function(se As Object, cert As System.Security.Cryptography.X509Certificates.X509Certificate, chain As System.Security.Cryptography.X509Certificates.X509Chain, sslerror As Security.SslPolicyErrors) True

        ' Funktioniert nicht: ByPass SSL Certificate Validation Checking wird ignoriert. Es kommt zu unerklärlichen System.Net.WebException in FritzBoxPOST
        ' FBTR64Desc = DeserializeObject(Of TR64Desc)($"http://{XMLData.POptionen.PTBFBAdr}:{FritzBoxDefault.PDfltFBSOAP}{Tr064Files.tr64desc}")

        ' Workaround: XML-Datei als String herunterladen und separat Deserialisieren

        ' Herunterladen
        If FritzBoxGet(New UriBuilder(Uri.UriSchemeHttps, XMLData.POptionen.ValidFBAdr, FritzBoxDefault.DfltTR064PortSSL, Tr064Files.tr64desc).Uri, Response) Then
            ' Deserialisieren
            FBTR64Desc = XmlDeserializeFromString(Of TR64Desc)(Response)

        End If

    End Sub
    <DebuggerStepThrough>
    Private Function GetService(SCPDURL As String) As Service

        If FBTR64Desc IsNot Nothing AndAlso FBTR64Desc.Device.ServiceList.Any Then
            Return FBTR64Desc.Device.ServiceList.Find(Function(Service) Service.SCPDURL.AreEqual(SCPDURL))
        Else
            NLogger.Error("SOAP zur Fritz!Box ist nicht bereit: {0}", XMLData.POptionen.TBFBAdr)
            Return Nothing
        End If

    End Function

    Friend Overloads Function TR064Start(SCPDURL As String, ActionName As String, Optional InputHashTable As Hashtable = Nothing) As Hashtable

        If Ping(XMLData.POptionen.ValidFBAdr) Then
            Dim TR064Error As String

            With GetService(SCPDURL)
                If .ActionExists(ActionName) Then
                    If .CheckInput(ActionName, InputHashTable) Then
                        Return .Start(.GetActionByName(ActionName), InputHashTable)
                    Else
                        TR064Error = $"InputData for Action ""{ActionName}"" not valid!"
                    End If
                Else
                    TR064Error = $"Action ""{ActionName}"" does not exist!"
                End If
            End With

            If TR064Error.IsNotStringEmpty Then
                NLogger.Error(TR064Error)
                With ErrorHashTable
                    .Clear()
                    .Add("Error", TR064Error)
                End With
            End If
        Else
            With ErrorHashTable
                .Clear()
                .Add("Error", $"Gegenstelle ({XMLData.POptionen.ValidFBAdr}) nicht erreichbar!")
            End With
        End If
        Return ErrorHashTable
    End Function

#Region "Abfragen"

#Region "deviceconfigSCPD"
    ''' <summary>
    ''' Generate a temporary URL session ID. The session ID is need for accessing URLs like phone book, call list, FAX message, answering machine messages Or phone book images.
    ''' </summary>
    ''' <param name="SessionID">Represents the temporary URL session ID.</param>
    ''' <returns>True when success</returns>
    Friend Function GetSessionID(ByRef SessionID As String) As Boolean

        With TR064Start(Tr064Files.deviceconfigSCPD, "X_AVM-DE_CreateUrlSID")

            If .ContainsKey("NewX_AVM-DE_UrlSID") Then

                SessionID = .Item("NewX_AVM-DE_UrlSID").ToString

                NLogger.Debug($"Aktuelle SessionID der Fritz!Box: {SessionID}")

                GetSessionID = True
            Else
                SessionID = FritzBoxDefault.DfltFritzBoxSessionID

                NLogger.Warn($"Keine SessionID der Fritz!Box erhalten. Rückgabewert: {SessionID}")

                GetSessionID = False
            End If
        End With

    End Function
#End Region

#Region "x_contactSCPD"

    ''' <summary>
    ''' The URL can be extended to limit the number of entries in the XML call list file.
    ''' E.g. max=42 would limit to 42 calls in the list.
    ''' If the parameter Is Not Set Or the value Is 0 all calls will be inserted into the Call list file.
    ''' The URL can be extended To fetch a limited number Of entries Using the parameter days.
    ''' E.g. days=7 would fetch the calls from now until 7 days in the past.
    ''' If the parameter Is Not Set Or the value Is 0 all calls will be inserted into the Call list file.
    ''' The parameter NewCallListURL Is empty, If the feature (CallList) Is disabled. If the feature
    ''' Is Not supported an internal error (820) Is returned. In the other case the URL Is returned.
    ''' </summary>
    ''' <param name="CallListURL">Represents the URL to the CallList.</param>
    ''' <returns>True when success</returns>
    ''' <remarks>
    ''' 
    ''' <list type="bullet">
    ''' <listheader>The following URL parameters are supported.</listheader>
    ''' <item>Parameter name (number): number of days to look back for calls e.g. 1: calls from today and yesterday, 7: calls from the complete last week, Default 999</item>
    ''' <item>Parameter id (number): calls since this unique ID</item>
    ''' <item>Parameter max (number): maximum number of entries in call list, default 999</item>
    ''' <item>Parameter sid (hex-string): Session ID for authentication </item>
    ''' <item>Parameter timestamp (number): value from timestamp tag, to get only entries that are newer (timestamp Is resetted by a factory reset) </item>
    ''' <item>Parameter tr064sid  (string): Session ID for authentication (obsolete)</item>
    ''' <item>Parameter type  (string): optional parameter for type of output file: xml (default) or csv </item>
    ''' </list>
    ''' The parameters timestamp and id have to be used in combination. If only one of both is used, the feature Is Not supported. 
    ''' </remarks>
    Friend Function GetCallList(ByRef CallListURL As String) As Boolean

        With TR064Start(Tr064Files.x_contactSCPD, "GetCallList")

            If .ContainsKey("NewCallListURL") Then

                CallListURL = .Item("NewCallListURL").ToString

                NLogger.Debug($"Pfad zur Anrufliste der Fritz!Box: '{CallListURL}'")

                GetCallList = True
            Else
                CallListURL = DfltStringEmpty

                NLogger.Warn($"Pfad zur Anrufliste der Fritz!Box nicht ermittelt.")

                GetCallList = False
            End If
        End With

    End Function
#End Region

#Region "x_tamSCPD"
    ''' <summary>
    ''' Return a informations of tam index <paramref name="i"/>. 
    ''' </summary>
    ''' <param name="PhoneNumbers">Empty string represents all numbers. Comma (,) separated list represents specific phone numbers.</param>
    ''' <param name="i">Represents the index of all tam.</param>
    ''' <returns>True when success</returns>
    ''' <remarks>Weitere felder verfügbar: NewEnable, NewName, NewTAMRunning, NewStick, NewStatus, NewCapacity, NewMode, NewRingSeconds </remarks>
    Friend Function GetTAMInfo(ByRef PhoneNumbers As String(), i As Integer) As Boolean

        With TR064Start(Tr064Files.x_tamSCPD, "GetInfo", New Hashtable From {{"NewIndex", i}})

            If .ContainsKey("NewPhoneNumbers") Then

                NLogger.Trace(.Item("NewPhoneNumbers"))

                PhoneNumbers = .Item("NewPhoneNumbers").ToString.Split(",")

                GetTAMInfo = True

            Else
                NLogger.Warn($"GetInfo konnte für nicht aufgelößt werden.")
                PhoneNumbers = {}

                GetTAMInfo = False
            End If
        End With

    End Function

    ''' <summary>
    ''' Returns the global information and the specific answering machine information as xml list.
    ''' </summary>
    ''' <param name="TAMListe">Represents the list of all tam.</param>
    ''' <returns>True when success</returns>
    Friend Function GetTAMList(ByRef TAMListe As TAMList) As Boolean

        With TR064Start(Tr064Files.x_tamSCPD, "GetList")

            If .ContainsKey("NewTAMList") Then

                NLogger.Trace(.Item("NewTAMList"))

                TAMListe = XmlDeserializeFromString(Of TAMList)(.Item("NewTAMList").ToString())

                ' Wenn keine TAM angeschlossen wurden, gib eine leere Klasse zurück
                If TAMListe Is Nothing Then TAMListe = New TAMList

                GetTAMList = True

            Else
                NLogger.Warn($"GetList konnte für nicht aufgelößt werden.")
                TAMListe = Nothing

                GetTAMList = False
            End If
        End With

    End Function
#End Region

#Region "x_voipSCPD"
    ''' <summary>
    ''' Get the configured common country code where the <paramref name="LKZ"/> represents the actual country code and the <paramref name="LKZPrefix"/> is the international call prefix.
    ''' </summary>
    ''' <param name="LKZ">Represents the actual country code.</param>
    ''' <param name="LKZPrefix">Represents the international call prefix.</param>
    ''' <returns>True when success</returns>
    Friend Function GetVoIPCommonCountryCode(ByRef LKZ As String, Optional ByRef LKZPrefix As String = "") As Boolean

        With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_GetVoIPCommonCountryCode")

            If .ContainsKey("NewX_AVM-DE_LKZ") And .ContainsKey("NewX_AVM-DE_LKZPrefix") Then
                LKZ = .Item("NewX_AVM-DE_LKZ").ToString
                LKZPrefix = .Item("NewX_AVM-DE_LKZPrefix").ToString

                GetVoIPCommonCountryCode = True

            Else
                NLogger.Warn($"LKZ und LKZPrefix konnten nicht ermittelt werden.")
                LKZ = If(LKZ.IsStringNothing, DfltStringEmpty, LKZ)
                LKZPrefix = If(LKZPrefix.IsStringNothing, DfltStringEmpty, LKZPrefix)

                GetVoIPCommonCountryCode = False
            End If
        End With

    End Function

    ''' <summary>
    ''' Get the configured common area code where the <paramref name="OKZ"/> represents the actual area code and the <paramref name="OKZPrefix"/> is the national Call prefix.
    ''' </summary>
    ''' <param name="OKZ">Represents the actual area code.</param>
    ''' <param name="OKZPrefix">Represents the national Call prefix.</param>
    ''' <returns>True when success</returns>
    Friend Function GetVoIPCommonAreaCode(ByRef OKZ As String, Optional ByRef OKZPrefix As String = "") As Boolean

        With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_GetVoIPCommonAreaCode")

            If .ContainsKey("NewX_AVM-DE_OKZ") And .ContainsKey("NewX_AVM-DE_OKZPrefix") Then
                OKZ = .Item("NewX_AVM-DE_OKZ").ToString
                OKZPrefix = .Item("NewX_AVM-DE_OKZPrefix").ToString

                GetVoIPCommonAreaCode = True

            Else
                NLogger.Warn($"OKZ und OKZPrefix konnten nicht ermittelt werden.")
                OKZ = If(OKZ.IsStringNothing, DfltStringEmpty, OKZ)
                OKZPrefix = If(OKZPrefix.IsStringNothing, DfltStringEmpty, OKZPrefix)

                GetVoIPCommonAreaCode = False
            End If
        End With

    End Function

    ''' <summary>
    ''' Ermittelt das aktuell ausgewählte Telefon der Fritz!Box Wählhilfe
    ''' </summary>
    ''' <param name="PhoneName">Phoneport des ausgewählten Telefones.</param>
    ''' <returns>True when success</returns>
    Friend Function DialGetConfig(ByRef PhoneName As String) As Boolean
        With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_DialGetConfig")

            If .ContainsKey("NewX_AVM-DE_PhoneName") Then
                PhoneName = .Item("NewX_AVM-DE_PhoneName").ToString

                DialGetConfig = True

            Else
                NLogger.Warn($"X_AVM-DE_DialGetConfig konnte nicht aufgelößt werden.")
                PhoneName = DfltStringEmpty

                DialGetConfig = False
            End If
        End With
    End Function

    ''' <summary>
    ''' Disconnect the dialling process. 
    ''' </summary>
    ''' <returns>True</returns>
    Friend Function DialHangup() As Boolean
        With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_DialHangup")
            Return Not .ContainsKey("Error")
        End With
    End Function

    ''' <summary>
    ''' Startet den Wählvorgang mit der übergebenen Telefonnummer.
    ''' </summary>
    ''' <param name="PhoneNumber">Die zu wählende Telefonnummer.</param>
    Friend Function DialNumber(PhoneNumber As String) As Boolean
        With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_DialNumber", New Hashtable From {{"NewX_AVM-DE_PhoneNumber", PhoneNumber}})
            Return Not .ContainsKey("Error")
        End With
    End Function

    ''' <summary>
    ''' Stellt die Wählhilfe der Fritz!Box auf das gewünschte Telefon um.
    ''' </summary>
    ''' <param name="PhoneName">Phoneport des Telefones.</param>
    Friend Function DialSetConfig(PhoneName As String) As Boolean
        With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_DialSetConfig", New Hashtable From {{"NewX_AVM-DE_PhoneName", PhoneName}})
            Return Not .ContainsKey("Error")
        End With
    End Function

    ''' <summary>
    ''' Return a list of all telephone numbers. 
    ''' </summary>
    ''' <param name="NumberList">Represents the list of all telephone numbers.</param>
    ''' <returns>True when success</returns>
    ''' <remarks>The list contains all configured numbers for all number types. The index can be used to see how many numbers are configured For one type. </remarks>
    Friend Function GetNumbers(ByRef NumberList As SIPTelNrList) As Boolean

        With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_GetNumbers")

            If .ContainsKey("NewNumberList") Then

                NLogger.Trace(.Item("NewNumberList"))

                NumberList = XmlDeserializeFromString(Of SIPTelNrList)(.Item("NewNumberList").ToString())

                ' Wenn keine Nummern angeschlossen wurden, gib eine leere Klasse zurück
                If NumberList Is Nothing Then NumberList = New SIPTelNrList

                GetNumbers = True

            Else
                NLogger.Warn($"X_AVM-DE_GetNumbers konnte für nicht aufgelößt werden.")
                NumberList = Nothing

                GetNumbers = False
            End If
        End With

    End Function

    ''' <summary>
    ''' Return phone name by <paramref name="i"/> (1 … n) usable for X_AVM-DE_SetDialConfig.
    ''' <list type="bullet">
    ''' <item>FON1: Telefon</item>
    ''' <item>FON2: Telefon</item>
    ''' <item>ISDN: ISDN/DECT Rundruf</item>
    ''' <item>DECT: Mobilteil 1</item>
    ''' </list>
    ''' </summary>
    ''' <param name="PhoneName">Represents the PhoneName of index <paramref name="i"/>.</param>
    ''' <param name="i">Represents the index of all dialable phones.</param>
    ''' <remarks>SIP IP phones are not usable for X_AVM-DE_SetDialConfig.</remarks>
    ''' <returns>True when success</returns>
    Friend Function GetPhonePort(ByRef PhoneName As String, i As Integer) As Boolean

        With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_GetPhonePort", New Hashtable From {{"NewIndex", i}})

            If .ContainsKey("NewX_AVM-DE_PhoneName") Then
                PhoneName = .Item("NewX_AVM-DE_PhoneName").ToString

                GetPhonePort = True

            Else
                NLogger.Warn($"X_AVM-DE_GetPhonePort konnte für id {i} nicht aufgelößt werden.")
                PhoneName = DfltStringEmpty

                GetPhonePort = False
            End If
        End With

    End Function

    ''' <summary>
    ''' Return a list of all SIP client accounts. 
    ''' </summary>
    ''' <param name="ClientList">Represents the list of all SIP client accounts.</param>
    ''' <returns>True when success</returns>
    ''' <remarks>The list contains all configured SIP client accounts a XML list.</remarks>
    Friend Function GetSIPClients(ByRef ClientList As SIPClientList) As Boolean

        With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_GetClients")

            If .ContainsKey("NewX_AVM-DE_ClientList") Then

                NLogger.Trace(.Item("NewX_AVM-DE_ClientList"))

                ClientList = XmlDeserializeFromString(Of SIPClientList)(.Item("NewX_AVM-DE_ClientList").ToString())

                ' Wenn keine SIP-Clients angeschlossen wurden, gib eine leere Klasse zurück
                If ClientList Is Nothing Then ClientList = New SIPClientList

                GetSIPClients = True

            Else
                NLogger.Warn($"X_AVM-DE_GetClients konnte für nicht aufgelößt werden.")
                ClientList = Nothing

                GetSIPClients = False
            End If
        End With

    End Function



#End Region

#End Region
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
