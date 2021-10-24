Imports System.Collections
Namespace SOAP
    Public Class X_contactSCPD
        Implements IService
        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger Implements IService.NLogger
        Private Property TR064Start As Func(Of String, String, Hashtable, Hashtable) Implements IService.TR064Start
        Private Property PushStatus As Action(Of LogLevel, String) Implements IService.PushStatus

        Public Sub New(Start As Func(Of String, String, Hashtable, Hashtable), Status As Action(Of LogLevel, String))

            TR064Start = Start

            PushStatus = Status
        End Sub

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

            With TR064Start(Tr064Files.x_contactSCPD, "GetCallList", Nothing)

                If .ContainsKey("NewCallListURL") Then

                    CallListURL = .Item("NewCallListURL").ToString

                    PushStatus.Invoke(LogLevel.Debug, $"Pfad zur Anrufliste der Fritz!Box: {CallListURL} ")

                    Return True
                Else
                    CallListURL = DfltStringEmpty

                    PushStatus.Invoke(LogLevel.Warn, $"Pfad zur Anrufliste der Fritz!Box konnte nicht ermittelt. '{ .Item("Error")}'")

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

            With TR064Start(Tr064Files.x_contactSCPD, "GetPhonebookList", Nothing)

                If .ContainsKey("NewPhonebookList") Then
                    ' Comma separated list of PhonebookID 
                    PhonebookList = Array.ConvertAll(.Item("NewPhonebookList").ToString.Split(","),
                                                     New Converter(Of String, Integer)(AddressOf Integer.Parse))

                    PushStatus.Invoke(LogLevel.Debug, $"Telefonbuchliste der Fritz!Box: '{String.Join(", ", PhonebookList)}'")

                    Return True
                Else
                    PhonebookList = {}

                    PushStatus.Invoke(LogLevel.Warn, $"Telefonbuchliste der Fritz!Box konnte nicht ermittelt. '{ .Item("Error")}'")

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

                    PushStatus.Invoke(LogLevel.Debug, $"Pfad zum Telefonbuch '{PhonebookName}' der Fritz!Box: {PhonebookURL} ")

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetPhonebook konnte für das Telefonbuch {PhonebookID} nicht aufgelößt werden. '{ .Item("Error")}'")
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

                    PushStatus.Invoke(LogLevel.Debug, $"Telefonbucheintrag '{PhonebookEntryID}' aus Telefonbuch {PhonebookID} der Fritz!Box ausgelesen: '{PhonebookEntryData}'")

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetPhonebookEntry für konnte für den Telefonbucheintrag '{PhonebookEntryID}' aus Telefonbuch {PhonebookID} nicht aufgelößt werden. '{ .Item("Error")}'")
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

                    PushStatus.Invoke(LogLevel.Debug, $"Telefonbucheintrag '{PhonebookEntryUniqueID}' aus Telefonbuch {PhonebookID} der Fritz!Box ausgelesen: '{PhonebookEntryData}'")

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetPhonebookEntry für konnte für den Telefonbucheintrag '{PhonebookEntryUniqueID}' aus Telefonbuch '{PhonebookID}' nicht aufgelößt werden. '{ .Item("Error")}'")
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
                    PushStatus.Invoke(LogLevel.Warn, $"SetPhonebookEntryUID konnte nicht aufgelöst werden. '{ .Item("Error")}'")
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

                    PushStatus.Invoke(LogLevel.Debug, $"Rufsperre aus Telefonbuch {PhonebookEntryID} der Fritz!Box ausgelesen: '{PhonebookEntryData}'")

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetCallBarringEntry konnte für die ID {PhonebookEntryID} nicht aufgelöst werden. '{ .Item("Error")}'")

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

                    PushStatus.Invoke(LogLevel.Debug, $"Rufsperre für die Nummer {Number} der Fritz!Box ausgelesen: '{PhonebookEntryData}'")

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetCallBarringEntryByNum konnte für die Nummer {Number} nicht aufgelöst werden. '{ .Item("Error")}'")

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

            With TR064Start(Tr064Files.x_contactSCPD, "GetCallBarringList", Nothing)

                If .ContainsKey("NewPhonebookURL") Then
                    ' Phonebook URL auslesen
                    PhonebookURL = .Item("NewPhonebookURL").ToString

                    PushStatus.Invoke(LogLevel.Debug, $"Pfad zur Rufsperre der Fritz!Box: {PhonebookURL} ")

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetCallBarringList konnte für die Rufsperre nicht aufgelöst werden. '{ .Item("Error")}'")
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

                    PushStatus.Invoke(LogLevel.Debug, $"Rufsperre in der Fritz!Box angelegt: '{PhonebookEntryUniqueID}'")

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"SetCallBarringEntry konnte keinen Eintrag anlegen: '{PhonebookEntryData}' '{ .Item("Error")}'")

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

            With TR064Start(Tr064Files.x_contactSCPD, "GetNumberOfDeflections", Nothing)

                If .ContainsKey("NewNumberOfDeflections") Then
                    ' Phonebook URL auslesen
                    NumberOfDeflections = .Item("NewNumberOfDeflections").ToString

                    PushStatus.Invoke(LogLevel.Debug, $"Anzahl der Rufweiterleitungen aus der Fritz!Box ausgelesen: '{NumberOfDeflections}'")

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetNumberOfDeflections konnte nicht aufgelöst werden. '{ .Item("Error")}'")

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
                    DeflectionInfo.PhonebookID = .Item("NewPhonebookID").ToString

                    PushStatus.Invoke(LogLevel.Debug, $"GetDeflection ({DeflectionId}): {DeflectionInfo.Mode}; {DeflectionInfo.Enable}")

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetDeflection konnte für nicht aufgelößt werden. '{ .Item("Error")}'")

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

            With TR064Start(Tr064Files.x_contactSCPD, "GetDeflections", Nothing)

                If .ContainsKey("NewDeflectionList") Then
                    ' Phonebook URL auslesen
                    DeflectionList = .Item("NewDeflectionList").ToString

                    PushStatus.Invoke(LogLevel.Debug, $"Liste der Rufweiterleitungen: '{DeflectionList}'")

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetDeflections konnte nicht aufgelöst werden. '{ .Item("Error")}'")

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


    End Class
End Namespace