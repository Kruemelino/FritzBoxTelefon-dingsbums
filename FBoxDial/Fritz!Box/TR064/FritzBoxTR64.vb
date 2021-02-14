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
            If Not XmlDeserializeFromString(Response, FBTR64Desc) Then
                NLogger.Error($"FritzBoxTR64 kann nicht initialisiert werden: Fehler beim Deserialisieren der FBTR64Desc.")
            End If
        Else
            NLogger.Error($"FritzBoxTR64 kann nicht initialisiert werden: Fehler beim Herunterladen der FBTR64Desc.")
        End If

    End Sub
    <DebuggerStepThrough>
    Private Function GetService(SCPDURL As String) As Service

        If FBTR64Desc IsNot Nothing AndAlso FBTR64Desc.Device.ServiceList.Any Then
            Return FBTR64Desc.Device.ServiceList.Find(Function(Service) Service.SCPDURL.AreEqual(SCPDURL))
        Else
            NLogger.Error($"SOAP zur Fritz!Box ist nicht bereit: {XMLData.POptionen.ValidFBAdr}")
            Return Nothing
        End If

    End Function

    Private Function TR064Start(SCPDURL As String, ActionName As String, Optional InputHashTable As Hashtable = Nothing) As Hashtable

        If Ping(XMLData.POptionen.ValidFBAdr) Then
            Dim TR064Error As String

            With GetService(SCPDURL)
                If?.ActionExists(ActionName) Then
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
    ''' Ermittelt die URL zum Herunterladen des Anrufliste.
    ''' </summary>
    ''' <param name="CallListURL">Represents the URL to the CallList.
    ''' The URL can be extended to limit the number of entries in the XML call list file.
    ''' E.g. max=42 would limit to 42 calls in the list.
    ''' If the parameter Is Not Set Or the value Is 0 all calls will be inserted into the Call list file.
    ''' The URL can be extended To fetch a limited number Of entries Using the parameter days.
    ''' E.g. days=7 would fetch the calls from now until 7 days in the past.
    ''' If the parameter Is Not Set Or the value Is 0 all calls will be inserted into the Call list file.
    ''' The parameter NewCallListURL Is empty, If the feature (CallList) Is disabled. If the feature
    ''' Is Not supported an internal error (820) Is returned. In the other case the URL Is returned.    
    '''     <list type="bullet">
    '''         <listheader>The following URL parameters are supported.</listheader>
    '''         <item><term>name</term> (number): number of days to look back for calls e.g. 1: calls from today and yesterday, 7: calls from the complete last week, Default 999</item>
    '''         <item><term>id</term> (number): calls since this unique ID</item>
    '''         <item><term>maxv</term> (number): maximum number of entries in call list, default 999</item>
    '''         <item><term>sid</term> (hex-string): Session ID for authentication </item>
    '''         <item><term>timestamp</term> (number): value from timestamp tag, to get only entries that are newer (timestamp Is resetted by a factory reset) </item>
    '''         <item><term>tr064sid</term>  (string): Session ID for authentication (obsolete)</item>
    '''         <item><term>type</term>  (string): optional parameter for type of output file: xml (default) or csv </item>
    '''     </list>
    '''     The parameters timestamp and id have to be used in combination. If only one of both is used, the feature Is Not supported. 
    ''' </param>
    ''' <returns>True when success</returns>
    ''' <remarks>
    ''' 
    ''' </remarks>
    Friend Function GetCallList(ByRef CallListURL As String) As Boolean

        With TR064Start(Tr064Files.x_contactSCPD, "GetCallList")

            If .ContainsKey("NewCallListURL") Then

                CallListURL = .Item("NewCallListURL").ToString

                NLogger.Debug($"Pfad zur Anrufliste der Fritz!Box: '{CallListURL}'")

                GetCallList = True
            Else
                CallListURL = DfltStringEmpty

                NLogger.Warn($"Pfad zur Anrufliste der Fritz!Box konnte nicht ermittelt.")

                GetCallList = False
            End If
        End With

    End Function

    ''' <summary>
    ''' Ermittelt die Liste der Telefonbocher. 
    ''' </summary>
    ''' <param name="PhonebookList">Liste der Telefonbuch IDs</param>
    ''' <returns>True when success</returns>
    Friend Function GetPhonebookList(ByRef PhonebookList As Integer()) As Boolean

        With TR064Start(Tr064Files.x_contactSCPD, "GetPhonebookList")

            If .ContainsKey("NewPhonebookList") Then
                ' Comma separated list of PhonebookID 
                PhonebookList = Array.ConvertAll(.Item("NewPhonebookList").ToString.Split(","),
                                                 New Converter(Of String, Integer)(AddressOf Integer.Parse))

                NLogger.Debug($"Telefonbuchliste der Fritz!Box: '{String.Join(", ", PhonebookList)}'")

                GetPhonebookList = True
            Else
                PhonebookList = {}

                NLogger.Warn($"Telefonbuchliste der Fritz!Box konnte nicht ermittelt.")

                GetPhonebookList = False
            End If
        End With

    End Function

    ''' <summary>
    ''' Ermittelt die URL zum Herunterladen des Telefonbuches mit der <paramref name="PhonebookID"/>.
    ''' </summary>
    ''' <param name="PhonebookURL"> Represents the URL to the phone book with <paramref name="PhonebookID"/>.
    '''     The following URL parameters are supported.
    '''     <list type="bullet">
    '''     <listheader>The following URL parameters are supported.</listheader>
    '''     <item><term>pbid</term> (number): number of days to look back for calls e.g. 1: calls from today and yesterday, 7: calls from the complete last week, Default 999</item>
    '''     <item><term>max</term> (number): maximum number of entries in call list, default 999</item>
    '''     <item><term>sid</term> (hex-string): Session ID for authentication </item>
    '''     <item><term>timestamp</term> (number): value from timestamp tag, to get the phonebook content only if last modification was made after this timestamp</item>
    '''     <item><term>tr064sid</term> (string): Session ID for authentication (obsolete)</item>
    ''' </list></param>
    ''' <param name="PhonebookID">ID of the phonebook.</param>
    ''' <param name="PhonebookName">Name of the phonebook.</param>
    ''' <param name="PhonebookExtraID">The value of <paramref name="PhonebookExtraID"/> may be an empty string. </param>
    ''' <returns>True when success</returns>
    Friend Function GetPhonebook(PhonebookID As Integer, ByRef PhonebookURL As String,
                                 Optional ByRef PhonebookName As String = "",
                                 Optional ByRef PhonebookExtraID As String = "") As Boolean

        With TR064Start(Tr064Files.x_contactSCPD, "GetPhonebook", New Hashtable From {{"NewPhonebookID", PhonebookID}})

            If .ContainsKey("NewPhonebookURL") Then
                ' Phonebook URL auslesen
                PhonebookURL = .Item("NewPhonebookURL").ToString
                ' Phonebook Name auslesen
                If .ContainsKey("NewPhonebookName") Then PhonebookName = .Item("NewPhonebookName").ToString
                ' Phonebook ExtraID auslesen
                If .ContainsKey("NewPhonebookExtraID") Then PhonebookExtraID = .Item("NewPhonebookExtraID").ToString

                NLogger.Debug($"Pfad zum Telefonbuch '{PhonebookName}' der Fritz!Box: '{PhonebookURL}'")

                GetPhonebook = True

            Else
                NLogger.Warn($"GetPhonebook konnte für das Telefonbuch {PhonebookID} nicht aufgelößt werden.")
                PhonebookURL = DfltStringEmpty

                GetPhonebook = False
            End If
        End With

    End Function

    ''' <summary>
    ''' Fügt ein neues Telefonbuch hinzu.
    ''' </summary>
    ''' <param name="PhonebookName">Name des neuen Telefonbuches.</param>
    ''' <param name="PhonebookExtraID">ExtraID des neuen Telefonbuches. (Optional)</param>
    ''' <returns>True when success</returns>
    Friend Function AddPhonebook(PhonebookName As String, Optional PhonebookExtraID As String = "") As Boolean

        With TR064Start(Tr064Files.x_contactSCPD, "AddPhonebook", New Hashtable From {{"NewPhonebookName", PhonebookName},
                                                                                      {"NewPhonebookExtraID", PhonebookExtraID}})

            Return Not .ContainsKey("Error")

        End With

    End Function

    ''' <summary>
    ''' Löscht das Telefonbuch mit der <paramref name="NewPhonebookID"/>.
    ''' </summary>
    ''' <remarks>The default phonebook (PhonebookID = 0) is not deletable, but therefore, each entry will be deleted And the phonebook will be empty afterwards.</remarks>
    ''' <param name="NewPhonebookID">ID of the phonebook.</param>
    ''' <param name="PhonebookExtraID">Optional parameter to make a phonebook unique.</param>
    ''' <returns>True when success</returns>
    Friend Function DeletePhonebook(NewPhonebookID As Integer, Optional PhonebookExtraID As String = "") As Boolean

        With TR064Start(Tr064Files.x_contactSCPD, "DeletePhonebook", New Hashtable From {{"NewPhonebookID", NewPhonebookID},
                                                                                         {"NewPhonebookExtraID", PhonebookExtraID}})

            Return Not .ContainsKey("Error")

        End With

    End Function

    ''' <summary>
    ''' Get a single telephone book entry from the specified book.
    ''' </summary>
    ''' <param name="PhonebookID">Number for a single phonebook.</param>
    ''' <param name="PhonebookEntryID">Unique identifier (number) for a single entry in a phonebook.</param>
    ''' <param name="PhonebookEntryData">XML document with a single entry. </param>
    ''' <returns>True when success</returns>
    Friend Function GetPhonebookEntry(PhonebookID As Integer, PhonebookEntryID As Integer, ByRef PhonebookEntryData As String) As Boolean

        With TR064Start(Tr064Files.x_contactSCPD, "GetPhonebookEntry", New Hashtable From {{"NewPhonebookID", PhonebookID},
                                                                                           {"NewPhonebookEntryID", PhonebookEntryID},
                                                                                           {"NewPhonebookEntryData", PhonebookEntryData}})

            If .ContainsKey("NewPhonebookEntryData") Then
                ' Phonebook URL auslesen
                PhonebookEntryData = .Item("NewPhonebookEntryData").ToString

                NLogger.Debug($"Telefonbucheintrag '{PhonebookEntryID}' aus Telefonbuch {PhonebookID} der Fritz!Box ausgelesen: '{PhonebookEntryData}'")

                GetPhonebookEntry = True

            Else
                NLogger.Warn($"GetPhonebookEntry für konnte für den Telefonbucheintrag '{PhonebookEntryID}' aus Telefonbuch {PhonebookID} nicht aufgelößt werden.")
                PhonebookEntryData = DfltStringEmpty

                GetPhonebookEntry = False
            End If

        End With

    End Function

    ''' <summary>
    ''' Get a single telephone book entry from the specified book using the unique ID from the entry.
    ''' </summary>
    ''' <param name="PhonebookID">Number for a single phonebook.</param>
    ''' <param name="PhonebookEntryUniqueID">Unique identifier (number) for a single entry in a phonebook.</param>
    ''' <param name="PhonebookEntryData">XML document with a single entry. </param>
    ''' <returns>True when success</returns>
    Friend Function GetPhonebookEntryUID(PhonebookID As Integer, PhonebookEntryUniqueID As Integer, ByRef PhonebookEntryData As String) As Boolean

        With TR064Start(Tr064Files.x_contactSCPD, "GetPhonebookEntryUID", New Hashtable From {{"NewPhonebookID", PhonebookID},
                                                                                              {"NewPhonebookEntryUniqueID", PhonebookEntryUniqueID},
                                                                                              {"NewPhonebookEntryData", PhonebookEntryData}})

            If .ContainsKey("NewPhonebookEntryData") Then
                ' Phonebook URL auslesen
                PhonebookEntryData = .Item("NewPhonebookEntryData").ToString

                NLogger.Debug($"Telefonbucheintrag '{PhonebookEntryUniqueID}' aus Telefonbuch {PhonebookID} der Fritz!Box ausgelesen: '{PhonebookEntryData}'")

                GetPhonebookEntryUID = True

            Else
                NLogger.Warn($"GetPhonebookEntry für konnte für den Telefonbucheintrag '{PhonebookEntryUniqueID}' aus Telefonbuch {PhonebookID} nicht aufgelößt werden.")
                PhonebookEntryData = DfltStringEmpty

                GetPhonebookEntryUID = False
            End If

        End With

    End Function

    ''' <summary>
    ''' Add a new or change an existing entry in a telephone book using the unique ID of the entry
    ''' <list type="bullet">
    '''     <listheader>
    '''         <term>Add new entry:</term>    
    '''     </listheader>
    '''     <item>set phonebook ID and XML entry data structure (without the unique ID tag)</item>
    ''' </list>
    ''' <list type="bullet">
    '''     <listheader>
    '''         <term>Change existing entry:</term>    
    '''     </listheader>
    '''     <item>set phonebook ID and XML entry data structure with the unique ID tag (e.g. <uniqueid>28</uniqueid>)</item>
    ''' </list>
    ''' The action returns the unique ID of the new or changed entry
    ''' </summary>
    ''' <param name="PhonebookID">ID of the phonebook.</param>
    ''' <param name="PhonebookEntryData">XML document with a single entry</param>
    ''' <param name="PhonebookEntryUniqueID">The action returns the unique ID of the new or changed entry.</param>
    ''' <returns>True when success</returns>
    Friend Function SetPhonebookEntryUID(PhonebookID As Integer, PhonebookEntryData As String, ByRef PhonebookEntryUniqueID As Integer) As Boolean

        With TR064Start(Tr064Files.x_contactSCPD, "SetPhonebookEntryUID", New Hashtable From {{"NewPhonebookID", PhonebookID},
                                                                                              {"NewPhonebookEntryData", PhonebookEntryData}})

            If .ContainsKey("NewPhonebookEntryUniqueID") Then
                ' Phonebook URL auslesen
                PhonebookEntryUniqueID = CInt(.Item("NewPhonebookEntryUniqueID"))

                SetPhonebookEntryUID = True

            Else
                NLogger.Warn($"SetPhonebookEntryUID konnte für nicht aufgelößt werden.")
                PhonebookEntryUniqueID = -1

                SetPhonebookEntryUID = False
            End If
        End With

    End Function

    ''' <summary>
    ''' Delete an existing telephone book entry.
    ''' Changes to online phonebooks are not allowed.
    ''' </summary>
    ''' <param name="PhonebookID">ID of the phonebook.</param>
    ''' <param name="PhonebookEntryID">Number for a single entry in a phonebook.</param>
    ''' <returns>True when success</returns>
    Friend Function DeletePhonebookEntry(PhonebookID As Integer, PhonebookEntryID As Integer) As Boolean

        With TR064Start(Tr064Files.x_contactSCPD, "DeletePhonebookEntry", New Hashtable From {{"NewPhonebookID", PhonebookID},
                                                                                              {"NewPhonebookEntryID", PhonebookEntryID}})
            Return Not .ContainsKey("Error")

        End With
    End Function

    ''' <summary>
    ''' Delete an existing telephone book entry using the unique ID from the entry.
    ''' Changes to online phonebooks are not allowed.
    ''' </summary>
    ''' <param name="PhonebookID">ID of the phonebook.</param>
    ''' <param name="NewPhonebookEntryUniqueID">Unique identifier (number) for a single entry in a phonebook.</param>
    ''' <returns>True when success</returns>
    Friend Function DeletePhonebookEntryUID(PhonebookID As Integer, NewPhonebookEntryUniqueID As Integer) As Boolean

        With TR064Start(Tr064Files.x_contactSCPD, "DeletePhonebookEntryUID", New Hashtable From {{"NewPhonebookID", PhonebookID},
                                                                                                 {"NewPhonebookEntryUniqueID", NewPhonebookEntryUniqueID}})
            Return Not .ContainsKey("Error")

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

                If Not XmlDeserializeFromString(.Item("NewTAMList").ToString(), TAMListe) Then
                    NLogger.Warn($"GetList (TAM) konnte für nicht deserialisiert werden.")
                End If

                ' Wenn keine TAM angeschlossen wurden, gib eine leere Klasse zurück
                If TAMListe Is Nothing Then TAMListe = New TAMList

                GetTAMList = True

            Else
                NLogger.Warn($"GetList (TAM) konnte für nicht aufgelößt werden.")
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

                If Not XmlDeserializeFromString(.Item("NewNumberList").ToString(), NumberList) Then
                    NLogger.Warn($"X_AVM-DE_GetNumbers konnte für nicht deserialisiert werden.")
                End If

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

                If Not XmlDeserializeFromString(.Item("NewX_AVM-DE_ClientList").ToString(), ClientList) Then
                    NLogger.Warn($"X_AVM-DE_GetNumbers konnte für nicht deserialisiert werden.")
                End If

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
