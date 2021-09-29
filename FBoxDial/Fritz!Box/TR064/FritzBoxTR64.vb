Imports System.Collections
Imports System.Net

Namespace SOAP
    Friend Class FritzBoxTR64
        Implements IDisposable

        Friend Event Status As EventHandler(Of NotifyEventArgs(Of String))

        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
        Private Property FBTR64Desc As TR64Desc
        Private Property Credential As NetworkCredential
        Private Property FBoxIPAdresse As String

        ''' <summary>
        ''' Initiiert eine neue TR064 Schnittstelle zur Fritz!Box. Die <see cref="NetworkCredential"/> werden hier übergeben.<br/>
        ''' Falls die auzuführende Funktion keine Anmeldung erfordert, kann <paramref name="Anmeldeinformationen"/> Nothing sein.
        ''' </summary>
        ''' <param name="FritzBoxAdresse">Die IP Adresse der Fritz!Box.</param>
        ''' <param name="Anmeldeinformationen">Die Anmeldeinformationen (Benutzername und Passwort) als <see cref="NetworkCredential"/>.</param>
        Public Sub New(FritzBoxAdresse As String, Anmeldeinformationen As NetworkCredential)
            Dim Response As String = DfltStringEmpty

            ' IP Adresse der Fritz!Box setzen
            FBoxIPAdresse = FritzBoxAdresse

            ' Netzwerkanmeldeinformationen zuweisen
            Credential = Anmeldeinformationen

            ' ByPass SSL Certificate Validation Checking
            ServicePointManager.ServerCertificateValidationCallback = Function(se As Object, cert As System.Security.Cryptography.X509Certificates.X509Certificate, chain As System.Security.Cryptography.X509Certificates.X509Chain, sslerror As Security.SslPolicyErrors) True

            ' Funktioniert nicht: ByPass SSL Certificate Validation Checking wird ignoriert. Es kommt zu unerklärlichen System.Net.WebException in FritzBoxPOST
            ' FBTR64Desc = DeserializeObject(Of TR64Desc)($"http://{FBoxIPAdresse}:{FritzBoxDefault.PDfltFBSOAP}{Tr064Files.tr64desc}")

            ' Workaround: XML-Datei als String herunterladen und separat deserialisieren

            ' Herunterladen
            If DownloadString(New UriBuilder(Uri.UriSchemeHttps, FBoxIPAdresse, DfltTR064PortSSL, Tr064Files.tr64desc).Uri, Response) Then
                ' Deserialisieren
                If Not DeserializeXML(Response, False, FBTR64Desc) Then
                    PushStatus(LogLevel.Error, "FritzBoxTR64 kann nicht initialisiert werden: Fehler beim Deserialisieren der FBTR64Desc.")
                End If
            Else
                PushStatus(LogLevel.Error, "FritzBoxTR64 kann nicht initialisiert werden: Fehler beim Herunterladen der FBTR64Desc.")
            End If

        End Sub

        Private Sub PushStatus(Level As LogLevel, StatusMessage As String)
            NLogger.Log(Level, StatusMessage)
            RaiseEvent Status(Me, New NotifyEventArgs(Of String)(StatusMessage))
        End Sub

        Private Function GetService(SCPDURL As String) As Service

            If FBTR64Desc IsNot Nothing AndAlso FBTR64Desc.Device.ServiceList.Any Then
                ' Suche den angeforderten Service
                Dim FBoxService As Service = FBTR64Desc.Device.ServiceList.Find(Function(Service) Service.SCPDURL.AreEqual(SCPDURL))

                ' Weise die Fritz!Box IP-Adresse zu
                If FBoxService IsNot Nothing Then FBoxService.FBoxIPAdresse = FBoxIPAdresse

                Return FBoxService
            Else

                PushStatus(LogLevel.Error, $"SOAP zur Fritz!Box ist nicht bereit: {FBoxIPAdresse}")
                Return Nothing
            End If

        End Function

        Private Function TR064Start(SCPDURL As String, ActionName As String, Optional InputHashTable As Hashtable = Nothing) As Hashtable

            'If Ping(FBoxIPAdresse) Then

            With GetService(SCPDURL)
                If?.ActionExists(ActionName) Then
                    If .CheckInput(ActionName, InputHashTable) Then
                        Return .Start(.GetActionByName(ActionName), InputHashTable, Credential)
                    Else
                        PushStatus(LogLevel.Error, $"InputData for Action '{ActionName}' not valid!")
                    End If
                Else
                    PushStatus(LogLevel.Error, $"Action '{ActionName}'does not exist!")
                End If
            End With

            'End If

            Return New Hashtable From {{"Error", DfltStringEmpty}}
        End Function

#Region "Abfragen"

#Region "TR64Desc"
        ''' <summary>
        ''' Gibt die Firmware der Fritz!Box aus der TR-064 Description zurück.
        ''' </summary>
        ''' <returns>Fritz!Box Firmware Version</returns>
        Friend ReadOnly Property DisplayVersion As String
            Get
                Return If(FBTR64Desc IsNot Nothing AndAlso FBTR64Desc.SystemVersion IsNot Nothing, FBTR64Desc.SystemVersion.Display, DfltStringEmpty)
            End Get
        End Property

        Friend ReadOnly Property HardwareVersion As Integer
            Get
                Return FBTR64Desc.SystemVersion.HW
            End Get
        End Property

        Friend ReadOnly Property Major As Integer
            Get
                Return FBTR64Desc.SystemVersion.Major
            End Get
        End Property

        Friend ReadOnly Property Minor As Integer
            Get
                Return FBTR64Desc.SystemVersion.Minor
            End Get
        End Property

        Friend ReadOnly Property FriendlyName As String
            Get
                Return If(FBTR64Desc IsNot Nothing AndAlso FBTR64Desc.Device IsNot Nothing, FBTR64Desc.Device.FriendlyName, "Keine Verbindung zu einer Fritz!Box hergestellt.")
            End Get
        End Property

#End Region

#Region "deviceinfoSCPD"
        Friend Function GetInfo(Optional ByRef ManufacturerName As String = "",
                                Optional ByRef ManufacturerOUI As String = "",
                                Optional ByRef ModelName As String = "",
                                Optional ByRef Description As String = "",
                                Optional ByRef ProductClass As String = "",
                                Optional ByRef SerialNumber As String = "",
                                Optional ByRef SoftwareVersion As String = "",
                                Optional ByRef HardwareVersion As String = "",
                                Optional ByRef SpecVersion As String = "",
                                Optional ByRef ProvisioningCode As String = "",
                                Optional ByRef UpTime As String = "",
                                Optional ByRef DeviceLog As String = "") As Boolean

            With TR064Start(Tr064Files.deviceinfoSCPD, "GetInfo")

                If .ContainsKey("NewSoftwareVersion") Then

                    ManufacturerName = .Item("NewManufacturerName").ToString
                    ManufacturerOUI = .Item("NewManufacturerOUI").ToString
                    ModelName = .Item("NewModelName").ToString
                    Description = .Item("NewDescription").ToString
                    ProductClass = .Item("NewProductClass").ToString
                    SerialNumber = .Item("NewSerialNumber").ToString
                    SoftwareVersion = .Item("NewSoftwareVersion").ToString
                    HardwareVersion = .Item("NewHardwareVersion").ToString
                    SpecVersion = .Item("NewSpecVersion").ToString
                    ProvisioningCode = .Item("NewProvisioningCode").ToString
                    UpTime = .Item("NewUpTime").ToString
                    DeviceLog = .Item("NewDeviceLog").ToString

                    PushStatus(LogLevel.Debug, $"Geräteinformationen der Fritz!Box: {Description}")

                    Return True
                Else
                    PushStatus(LogLevel.Warn, $"Keine Geräteinformationen der Fritz!Box erhalten. '{ .Item("Error")}'")

                    Return False
                End If
            End With

        End Function
#End Region

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

                    PushStatus(LogLevel.Debug, $"Aktuelle SessionID der Fritz!Box: {SessionID}")

                    Return True
                Else
                    SessionID = FritzBoxDefault.DfltFritzBoxSessionID

                    PushStatus(LogLevel.Warn, $"Keine SessionID der Fritz!Box erhalten. Rückgabewert: '{SessionID}' '{ .Item("Error")}'")

                    Return False
                End If
            End With

        End Function
#End Region

#Region "LANConfigSecurity"
        ''' <summary>
        ''' Get the usernames of all users in a xml-list. Each item has an attribute “last_user”, which is set to '1' for only that username, which was used since last login.
        ''' </summary>
        ''' <param name="UserList">Get the usernames of all users in a xml-list.</param>
        ''' <returns>True when success</returns>
        Friend Function GetUserList(ByRef UserList As String) As Boolean

            With TR064Start(Tr064Files.lanconfigsecuritySCPD, "X_AVM-DE_GetUserList")

                If .ContainsKey("NewX_AVM-DE_UserList") Then

                    UserList = .Item("NewX_AVM-DE_UserList").ToString

                    PushStatus(LogLevel.Debug, $"Userliste der Fritz!Box: '{UserList}'")

                    Return True
                Else
                    UserList = DfltStringEmpty

                    PushStatus(LogLevel.Warn, $"Userliste der Fritz!Box konnte nicht ermittelt. '{ .Item("Error")}'")

                    Return False
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
        ''' </remarks>
        Friend Function GetCallList(ByRef CallListURL As String) As Boolean

            With TR064Start(Tr064Files.x_contactSCPD, "GetCallList")

                If .ContainsKey("NewCallListURL") Then

                    CallListURL = .Item("NewCallListURL").ToString

                    PushStatus(LogLevel.Debug, $"Pfad zur Anrufliste der Fritz!Box: '{CallListURL}'")

                    Return True
                Else
                    CallListURL = DfltStringEmpty

                    PushStatus(LogLevel.Warn, $"Pfad zur Anrufliste der Fritz!Box konnte nicht ermittelt. '{ .Item("Error")}'")

                    Return False
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

                    PushStatus(LogLevel.Debug, $"Telefonbuchliste der Fritz!Box: '{String.Join(", ", PhonebookList)}'")

                    Return True
                Else
                    PhonebookList = {}

                    PushStatus(LogLevel.Warn, $"Telefonbuchliste der Fritz!Box konnte nicht ermittelt. '{ .Item("Error")}'")

                    Return False
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

                    PushStatus(LogLevel.Debug, $"Pfad zum Telefonbuch '{PhonebookName}' der Fritz!Box: {PhonebookURL} ")

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"GetPhonebook konnte für das Telefonbuch {PhonebookID} nicht aufgelößt werden. '{ .Item("Error")}'")
                    PhonebookURL = DfltStringEmpty

                    Return False
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
                                                                                               {"NewPhonebookEntryID", PhonebookEntryID}})

                If .ContainsKey("NewPhonebookEntryData") Then
                    ' Phonebook URL auslesen
                    PhonebookEntryData = .Item("NewPhonebookEntryData").ToString

                    PushStatus(LogLevel.Debug, $"Telefonbucheintrag '{PhonebookEntryID}' aus Telefonbuch {PhonebookID} der Fritz!Box ausgelesen: '{PhonebookEntryData}'")

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"GetPhonebookEntry für konnte für den Telefonbucheintrag '{PhonebookEntryID}' aus Telefonbuch {PhonebookID} nicht aufgelößt werden. '{ .Item("Error")}'")
                    PhonebookEntryData = DfltStringEmpty

                    Return False
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
                                                                                                  {"NewPhonebookEntryUniqueID", PhonebookEntryUniqueID}})

                If .ContainsKey("NewPhonebookEntryData") Then
                    ' Phonebook URL auslesen
                    PhonebookEntryData = .Item("NewPhonebookEntryData").ToString

                    PushStatus(LogLevel.Debug, $"Telefonbucheintrag '{PhonebookEntryUniqueID}' aus Telefonbuch {PhonebookID} der Fritz!Box ausgelesen: '{PhonebookEntryData}'")

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"GetPhonebookEntry für konnte für den Telefonbucheintrag '{PhonebookEntryUniqueID}' aus Telefonbuch '{PhonebookID}' nicht aufgelößt werden. '{ .Item("Error")}'")
                    PhonebookEntryData = DfltStringEmpty

                    Return False
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

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"SetPhonebookEntryUID konnte nicht aufgelöst werden. '{ .Item("Error")}'")
                    PhonebookEntryUniqueID = -1

                    Return False
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

        ''' <summary>
        ''' Returns a call barring entry by its PhonebookEntryID of the specific call barring phonebook. 
        ''' </summary>
        ''' <param name="PhonebookEntryID">ID of the specific call barring phonebook.</param>
        ''' <param name="PhonebookEntryData">A call barring entry</param>
        ''' <returns>True when success</returns>
        Friend Function GetCallBarringEntry(PhonebookEntryID As Integer, ByRef PhonebookEntryData As String) As Boolean

            With TR064Start(Tr064Files.x_contactSCPD, "GetCallBarringEntry", New Hashtable From {{"NewPhonebookEntryID", PhonebookEntryID}})

                If .ContainsKey("NewPhonebookEntryData") Then
                    ' Phonebook URL auslesen
                    PhonebookEntryData = .Item("NewPhonebookEntryData").ToString

                    PushStatus(LogLevel.Debug, $"Rufsperre aus Telefonbuch {PhonebookEntryID} der Fritz!Box ausgelesen: '{PhonebookEntryData}'")

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"GetCallBarringEntry konnte für die ID {PhonebookEntryID} nicht aufgelöst werden. '{ .Item("Error")}'")

                    PhonebookEntryData = DfltStringEmpty

                    Return False
                End If
            End With

        End Function

        ''' <summary>
        ''' Returns a call barring entry by its number. If the number exists in the internal phonebook 
        ''' but not in the specific call barring phonebook, error code 714 Is returned.
        ''' </summary>
        ''' <param name="Number">phone number</param>
        ''' <param name="PhonebookEntryData">XML document with a single call barring entry.</param>
        ''' <returns>True when success</returns>
        Friend Function GetCallBarringEntryByNum(Number As String, ByRef PhonebookEntryData As String) As Boolean

            With TR064Start(Tr064Files.x_contactSCPD, "GetCallBarringEntryByNum", New Hashtable From {{"NewNumber", Number}})

                If .ContainsKey("NewPhonebookEntryData") Then
                    ' Phonebook URL auslesen
                    PhonebookEntryData = .Item("NewPhonebookEntryData").ToString

                    PushStatus(LogLevel.Debug, $"Rufsperre für die Nummer {Number} der Fritz!Box ausgelesen: '{PhonebookEntryData}'")

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"GetCallBarringEntryByNum konnte für die Nummer {Number} nicht aufgelöst werden. '{ .Item("Error")}'")

                    PhonebookEntryData = DfltStringEmpty

                    Return False
                End If
            End With

        End Function

        ''' <summary>
        ''' Returns a url which leads to an xml formatted file which contains all entries of the call barring phonebook.
        ''' </summary>
        ''' <param name="PhonebookURL">Url of the call barring phonebook</param>
        ''' <returns>True when success</returns>
        Friend Function GetCallBarringList(ByRef PhonebookURL As String) As Boolean

            With TR064Start(Tr064Files.x_contactSCPD, "GetCallBarringList")

                If .ContainsKey("NewPhonebookURL") Then
                    ' Phonebook URL auslesen
                    PhonebookURL = .Item("NewPhonebookURL").ToString

                    PushStatus(LogLevel.Debug, $"Pfad zur Rufsperre der Fritz!Box: {PhonebookURL} ")

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"GetCallBarringList konnte für die Rufsperre nicht aufgelöst werden. '{ .Item("Error")}'")
                    PhonebookURL = DfltStringEmpty

                    Return False
                End If
            End With

        End Function

        ''' <summary>
        ''' Add a phonebook entry to the specific call barring phonebook. When no uniqueid is given 
        ''' a new entry is created. Even when an entry with the given number is already existing.
        ''' When a uniqueid is set which already exist, this entry will be overwritten. When a uniqueid
        ''' is given which does not exist, a new entry is created and the new uniqueid is returned in argument NewPhonebookEntryUniqueID.
        ''' </summary>
        ''' <param name="PhonebookEntryData">XML document with a single call barring entry.</param>
        ''' <param name="PhonebookEntryUniqueID">Unique identifier (number) for a single entry in the specific call barring phonebook.</param>
        ''' <returns>True when success</returns>
        Friend Function SetCallBarringEntry(PhonebookEntryData As String, Optional ByRef PhonebookEntryUniqueID As Integer = 0) As Boolean

            With TR064Start(Tr064Files.x_contactSCPD, "SetCallBarringEntry", New Hashtable From {{"NewPhonebookEntryData", PhonebookEntryData}})

                If .ContainsKey("NewPhonebookEntryUniqueID") Then
                    ' Phonebook URL auslesen
                    PhonebookEntryUniqueID = CInt(.Item("NewPhonebookEntryUniqueID"))

                    PushStatus(LogLevel.Debug, $"Rufsperre in der Fritz!Box angelegt: '{PhonebookEntryUniqueID}'")

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"SetCallBarringEntry konnte keinen Eintrag anlegen: '{PhonebookEntryData}' '{ .Item("Error")}'")

                    PhonebookEntryUniqueID = -1

                    Return False
                End If
            End With

        End Function

        ''' <summary>
        ''' Delete an entry of the call barring phonebook by its uniqueid.
        ''' </summary>
        ''' <param name="NewPhonebookEntryUniqueID">uniqueid of an entry</param>
        ''' <returns>True when success</returns>
        Friend Function DeleteCallBarringEntryUID(NewPhonebookEntryUniqueID As Integer) As Boolean

            With TR064Start(Tr064Files.x_contactSCPD, "DeleteCallBarringEntryUID", New Hashtable From {{"NewPhonebookEntryUniqueID", NewPhonebookEntryUniqueID}})
                Return Not .ContainsKey("Error")

            End With

        End Function

        ''' <summary>
        ''' Get the number of deflection entrys.
        ''' </summary>
        ''' <param name="NumberOfDeflections">Returns the number of deflection entrys</param>
        ''' <returns>True when success</returns>
        Friend Function GetNumberOfDeflections(ByRef NumberOfDeflections As String) As Boolean

            With TR064Start(Tr064Files.x_contactSCPD, "GetNumberOfDeflections")

                If .ContainsKey("NewNumberOfDeflections") Then
                    ' Phonebook URL auslesen
                    NumberOfDeflections = .Item("NewNumberOfDeflections").ToString

                    PushStatus(LogLevel.Debug, $"Anzahl der Rufweiterleitungen aus der Fritz!Box ausgelesen: '{NumberOfDeflections}'")

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"GetNumberOfDeflections konnte nicht aufgelöst werden. '{ .Item("Error")}'")

                    NumberOfDeflections = DfltStringEmpty

                    Return False
                End If
            End With

        End Function

        ''' <summary>
        ''' Get the parameter for a deflection entry.
        ''' DeflectionID is in the range of 0 .. NumberOfDeflections-1.
        ''' </summary>
        ''' <param name="DeflectionInfo">Komplexes Datenelement, was alle Informationen zu der Rufumleitung enthält.</param>
        ''' <param name="DeflectionId">Die ID der Rufumleitung</param>
        ''' <returns>True when success</returns>
        Friend Function GetDeflection(ByRef DeflectionInfo As DeflectionInfo, DeflectionId As Integer) As Boolean

            If DeflectionInfo Is Nothing Then DeflectionInfo = New DeflectionInfo

            With TR064Start(Tr064Files.x_tamSCPD, "GetInfo", New Hashtable From {{"NewDeflectionId", DeflectionId}})

                If .ContainsKey("NewEnable") Then

                    DeflectionInfo.Enable = CBool(.Item("NewEnable"))
                    DeflectionInfo.Type = CType(.Item("NewType"), TypeEnum)
                    DeflectionInfo.Number = .Item("NewNumber").ToString
                    DeflectionInfo.DeflectionToNumber = .Item("NewDeflectionToNumber").ToString
                    DeflectionInfo.Mode = CType(.Item("NewMode"), ModeEnum)
                    DeflectionInfo.Outgoing = .Item("NewOutgoing").ToString
                    DeflectionInfo.PhonebookID = CInt(.Item("NewPhonebookID"))

                    PushStatus(LogLevel.Debug, $"GetDeflection ({DeflectionId}): {DeflectionInfo.Mode}; {DeflectionInfo.Enable}")

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"GetDeflection konnte für nicht aufgelößt werden. '{ .Item("Error")}'")

                    Return False
                End If
            End With

        End Function

        ''' <summary>
        ''' Returns a list of deflections.
        ''' </summary>
        ''' <param name="DeflectionList">List of deflections</param>
        ''' <returns>True when success</returns>
        Friend Function GetDeflections(ByRef DeflectionList As String) As Boolean

            With TR064Start(Tr064Files.x_contactSCPD, "GetDeflections")

                If .ContainsKey("NewDeflectionList") Then
                    ' Phonebook URL auslesen
                    DeflectionList = .Item("NewDeflectionList").ToString

                    PushStatus(LogLevel.Debug, $"Liste der Rufweiterleitungen: '{DeflectionList}'")

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"GetDeflections konnte nicht aufgelöst werden. '{ .Item("Error")}'")

                    DeflectionList = DfltStringEmpty

                    Return False
                End If
            End With

        End Function

        ''' <summary>
        ''' Enable or disable a deflection.
        ''' DeflectionID is in the range of 0 .. NumberOfDeflections-1
        ''' </summary>
        ''' <param name="DeflectionId">Die ID der Rufumleitung</param>
        ''' <param name="Enable">Neuer Aktivierungszustand</param>
        ''' <returns>True when success</returns>
        Friend Function SetDeflectionEnable(DeflectionId As Integer, Enable As Boolean) As Boolean

            With TR064Start(Tr064Files.x_contactSCPD, "SetPhonebookEntryUID", New Hashtable From {{"NewDeflectionId", DeflectionId},
                                                                                                  {"NewEnable", Enable.ToString}})

                Return Not .ContainsKey("Error")

            End With

        End Function
#End Region

#Region "x_tamSCPD"
        ''' <summary>
        ''' Return a informations of tam index <paramref name="i"/>. 
        ''' </summary>
        ''' <param name="TAMInfo">Structure, which holds all data of the TAM</param>
        ''' <param name="i">Represents the index of all tam.</param>
        ''' <returns>True when success</returns>
        Friend Function GetTAMInfo(ByRef TAMInfo As TAMInfo, i As Integer) As Boolean

            If TAMInfo Is Nothing Then TAMInfo = New TAMInfo

            With TR064Start(Tr064Files.x_tamSCPD, "GetInfo", New Hashtable From {{"NewIndex", i}})

                If .ContainsKey("NewEnable") And .ContainsKey("NewPhoneNumbers") Then

                    TAMInfo.Enable = CBool(.Item("NewEnable"))
                    TAMInfo.Name = .Item("NewName").ToString
                    TAMInfo.TAMRunning = CBool(.Item("NewTAMRunning"))
                    TAMInfo.Stick = CUShort(.Item("NewStick"))
                    TAMInfo.Status = CUShort(.Item("NewStatus"))
                    TAMInfo.Capacity = CULng(.Item("NewCapacity"))
                    TAMInfo.Mode = .Item("NewMode").ToString
                    TAMInfo.RingSeconds = CUShort(.Item("NewRingSeconds"))
                    TAMInfo.PhoneNumbers = .Item("NewPhoneNumbers").ToString.Split(",")

                    PushStatus(LogLevel.Debug, $"GetTAMInfoEx ({i}): {TAMInfo.Name}; {TAMInfo.Enable}")

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"GetTAMInfoEx konnte für nicht aufgelößt werden. '{ .Item("Error")}'")

                    Return False
                End If
            End With

        End Function

        ''' <summary>
        ''' Create an URL to download the list of message for a specified TAM. 
        ''' </summary>
        ''' <remarks>If the HTTP request for the resulting URL fails, it is recommended to make a New SOAP request For GetMessageList or call the SOAP action DeviceConfig:X_AVM-DE_CreateUrlSID for a New session ID.<br/>
        ''' The following URL parameters are supported.
        ''' <list type="bullet">
        ''' <item>max: maximum number of entries in message list, default 999</item>
        ''' <item>sid: Session ID for authentication</item>
        ''' </list>
        ''' </remarks>
        ''' <param name="GetMessageListURL">URL to download the list of message for a specified TAM</param>
        ''' <param name="i">ID of the specified TAM</param>
        ''' <returns>True when success</returns>
        Friend Function GetMessageList(ByRef GetMessageListURL As String, i As Integer) As Boolean
            With TR064Start(Tr064Files.x_tamSCPD, "GetMessageList", New Hashtable From {{"NewIndex", i}})
                If .ContainsKey("NewURL") Then

                    GetMessageListURL = .Item("NewURL").ToString

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"GetMessageList konnte für nicht aufgelößt werden. '{ .Item("Error")}'")

                    Return False
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

                    If Not DeserializeXML(.Item("NewTAMList").ToString(), False, TAMListe) Then
                        PushStatus(LogLevel.Warn, $"GetList (TAM) konnte für nicht deserialisiert werden.")
                    End If

                    ' Wenn keine TAM angeschlossen wurden, gib eine leere Klasse zurück
                    If TAMListe Is Nothing Then TAMListe = New TAMList

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"GetList (TAM) konnte für nicht aufgelößt werden. '{ .Item("Error")}'")
                    TAMListe = Nothing

                    Return False
                End If
            End With

        End Function

        ''' <summary>
        ''' If Enable is set to true, the TAM will be visible in WebGUI. 
        ''' </summary>
        ''' <param name="Index">Index of TAM</param>
        ''' <param name="Enable">Enable state</param>
        ''' <returns>True when success</returns>
        Friend Function SetEnable(Index As Integer, Enable As Boolean) As Boolean

            With TR064Start(Tr064Files.x_tamSCPD, "SetEnable", New Hashtable From {{"NewIndex", Index},
                                                                                   {"NewEnable", Enable.ToInt}})
                Return Not .ContainsKey("Error")
            End With

        End Function

        ''' <summary>
        ''' Mark a specified message as read. A specific TAM is selected by Index.
        ''' The Index field from a message in the MessageList should be taken for the MessageIndex
        ''' to select a specific message. If the MarkedAsRead state variable is set to 1, the message
        ''' is marked as read, when it is 0, the message is marked as unread. The default value is 1
        ''' to guarantee downward compatibility to older clients.
        ''' </summary>
        ''' <param name="Index">Index of the MessageList</param>
        ''' <param name="MessageIndex">Index of the Message</param>
        ''' <param name="MarkedAsRead">Optional, to stay compatible with older clients, default value is 1</param>
        ''' <returns>True when success</returns>
        Friend Function MarkMessage(Index As Integer, MessageIndex As Integer, MarkedAsRead As Boolean) As Boolean

            With TR064Start(Tr064Files.x_tamSCPD, "MarkMessage", New Hashtable From {{"NewIndex", Index},
                                                                                     {"NewMessageIndex", MessageIndex},
                                                                                     {"NewMarkedAsRead", MarkedAsRead.ToInt}})
                Return Not .ContainsKey("Error")
            End With

        End Function

        ''' <summary>
        ''' Delete a specified message. A specific TAM is selected by Index.
        ''' The Index field from a message in the MessageList should be taken for the MessageIndex
        ''' to select a specific message. 
        ''' </summary>
        ''' <param name="Index">Index of the MessageList</param>
        ''' <param name="MessageIndex">Index of the Message</param>
        ''' <returns>True when success</returns>
        Friend Function DeleteMessage(Index As Integer, MessageIndex As Integer) As Boolean

            With TR064Start(Tr064Files.x_tamSCPD, "DeleteMessage", New Hashtable From {{"NewIndex", Index},
                                                                                       {"NewMessageIndex", MessageIndex}})


                If Not .ContainsKey("Error") Then

                    PushStatus(LogLevel.Info, $"Nachricht auf Anrufbeantworter {Index} mit ID {MessageIndex} gelöscht, '{ .Item("Error")}'")
                    Return True
                Else

                    PushStatus(LogLevel.Warn, $"Nachricht auf Anrufbeantworter {Index} mit ID {MessageIndex} nicht gelöscht, '{ .Item("Error")}'")
                    Return False
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

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"LKZ und LKZPrefix konnten nicht ermittelt werden. '{ .Item("Error")}'")
                    LKZ = If(LKZ.IsStringNothingOrEmpty, DfltStringEmpty, LKZ)
                    LKZPrefix = If(LKZPrefix.IsStringNothingOrEmpty, DfltStringEmpty, LKZPrefix)

                    Return False
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

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"OKZ und OKZPrefix konnten nicht ermittelt werden. '{ .Item("Error")}'")
                    OKZ = If(OKZ.IsStringNothingOrEmpty, DfltStringEmpty, OKZ)
                    OKZPrefix = If(OKZPrefix.IsStringNothingOrEmpty, DfltStringEmpty, OKZPrefix)

                    Return False
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

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"X_AVM-DE_DialGetConfig konnte nicht aufgelößt werden. '{ .Item("Error")}'")
                    PhoneName = DfltStringEmpty

                    Return False
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

                    If Not DeserializeXML(.Item("NewNumberList").ToString(), False, NumberList) Then
                        PushStatus(LogLevel.Warn, $"X_AVM-DE_GetNumbers konnte für nicht deserialisiert werden. '{ .Item("Error")}'")
                    End If

                    ' Wenn keine Nummern angeschlossen wurden, gib eine leere Klasse zurück
                    If NumberList Is Nothing Then NumberList = New SIPTelNrList

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"X_AVM-DE_GetNumbers konnte für nicht aufgelößt werden.")
                    NumberList = Nothing

                    Return False
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

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"X_AVM-DE_GetPhonePort konnte für id {i} nicht aufgelößt werden. '{ .Item("Error")}'")
                    PhoneName = DfltStringEmpty

                    Return False
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

                    If Not DeserializeXML(.Item("NewX_AVM-DE_ClientList").ToString(), False, ClientList) Then
                        PushStatus(LogLevel.Warn, $"X_AVM-DE_GetNumbers konnte für nicht deserialisiert werden.")
                    End If

                    ' Wenn keine SIP-Clients angeschlossen wurden, gib eine leere Klasse zurück
                    If ClientList Is Nothing Then ClientList = New SIPClientList

                    Return True

                Else
                    PushStatus(LogLevel.Warn, $"X_AVM-DE_GetClients konnte für nicht aufgelößt werden. '{ .Item("Error")}'")
                    ClientList = Nothing

                    Return False
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

End Namespace