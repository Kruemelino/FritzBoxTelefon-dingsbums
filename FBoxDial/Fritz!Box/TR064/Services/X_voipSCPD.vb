Imports System.Collections
Namespace SOAP

    Friend Class X_voipSCPD
        Implements IService
        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger Implements IService.NLogger
        Private Property TR064Start As Func(Of String, String, Hashtable, Hashtable) Implements IService.TR064Start
        Private Property PushStatus As Action(Of LogLevel, String) Implements IService.PushStatus

        Public Sub New(Start As Func(Of String, String, Hashtable, Hashtable), Status As Action(Of LogLevel, String))

            TR064Start = Start

            PushStatus = Status
        End Sub

#Region "x_voipSCPD"
        Friend Function GetExistingVoIPNumbers(ByRef ExistingVoIPNumbers As Integer) As Boolean
            With TR064Start(Tr064Files.x_voipSCPD, "GetExistingVoIPNumbers", Nothing)

                If .ContainsKey("NewExistingVoIPNumbers") Then
                    ExistingVoIPNumbers = CInt(.Item("NewExistingVoIPNumbers"))

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetExistingVoIPNumbers konnte nicht aufgelößt werden. '{ .Item("Error")}'")
                    ExistingVoIPNumbers = 0

                    Return False
                End If
            End With
        End Function

        Friend Function GetMaxVoIPNumbers(ByRef MaxVoIPNumbers As Integer) As Boolean
            With TR064Start(Tr064Files.x_voipSCPD, "GetMaxVoIPNumbers", Nothing)

                If .ContainsKey("NewMaxVoIPNumbers") Then
                    MaxVoIPNumbers = CInt(.Item("NewMaxVoIPNumbers"))

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetMaxVoIPNumbers konnte nicht aufgelößt werden. '{ .Item("Error")}'")
                    MaxVoIPNumbers = 0

                    Return False
                End If
            End With
        End Function

        Friend Function GetVoIPEnableAreaCode(ByRef VoIPEnableAreaCode As Integer, VoIPAccountIndex As Integer) As Boolean
            With TR064Start(Tr064Files.x_voipSCPD, "GetVoIPEnableAreaCode", New Hashtable From {{"NewVoIPAccountIndex", VoIPAccountIndex}})

                If .ContainsKey("NewVoIPEnableAreaCode") Then
                    VoIPEnableAreaCode = CInt(.Item("NewVoIPEnableAreaCode"))

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetVoIPEnableAreaCode konnte nicht aufgelößt werden. '{ .Item("Error")}'")
                    VoIPEnableAreaCode = 0

                    Return False
                End If
            End With
        End Function

        Friend Function GetVoIPEnableCountryCode(ByRef VoIPEnableCountryCode As Integer, VoIPAccountIndex As Integer) As Boolean
            With TR064Start(Tr064Files.x_voipSCPD, "GetVoIPEnableCountryCode", New Hashtable From {{"NewVoIPAccountIndex", VoIPAccountIndex}})

                If .ContainsKey("NewVoIPEnableCountryCode") Then
                    VoIPEnableCountryCode = CInt(.Item("NewVoIPEnableCountryCode"))

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetVoIPEnableCountryCode konnte nicht aufgelößt werden. '{ .Item("Error")}'")
                    VoIPEnableCountryCode = 0

                    Return False
                End If
            End With
        End Function

        Friend Function GetNumberOfClients(ByRef NumberOfClients As Integer) As Boolean
            With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_GetNumberOfClients", Nothing)

                If .ContainsKey("NewX_AVM-DE_NumberOfClients") Then
                    NumberOfClients = CInt(.Item("NewX_AVM-DE_NumberOfClients"))

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"GetNumberOfClients konnte nicht aufgelößt werden. '{ .Item("Error")}'")
                    NumberOfClients = 0

                    Return False
                End If
            End With
        End Function

        Friend Function GetClient2(ByRef Client As SIPClient, ClientIndex As Integer) As Boolean
            If Client Is Nothing Then Client = New SIPClient

            With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_GetClient2", New Hashtable From {{"NewVoIPAccountIndex", ClientIndex}})

                If .ContainsKey("NewX_AVM-DE_ClientUsername ") Then
                    Client.ClientUsername = .Item("NewX_AVM-DE_ClientUsername").ToString
                    Client.ClientRegistrar = .Item("NewX_AVM-DE_ClientRegistrar").ToString
                    Client.ClientRegistrarPort = CInt(.Item("NewX_AVM-DE_ClientRegistrarPort"))
                    Client.PhoneName = .Item("NewX_AVM-DE_PhoneName").ToString
                    Client.ClientId = .Item("NewX_AVM-DE_ClientId").ToString
                    Client.OutGoingNumber = .Item("NewX_AVM-DE_OutGoingNumber").ToString
                    Client.InternalNumber = CInt("NewX_AVM-DE_InternalNumber")

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"X_AVM-DE_GetClient2 konnte nicht aufgelößt werden. '{ .Item("Error")}'")

                    Return False
                End If
            End With
        End Function

        ''' <summary>
        ''' Return SIP Client account with incoming numbers and allow registration from outside flag.
        ''' </summary>
        ''' <remarks>The format of the state variable X_AVM-DE_IncomingNumbers is similar to the state variable X_AVMDE_Numbers described in the paragraph X_AVM-DE_GetNumbers (below).
        ''' If the SIP client shall react on all possible numbers the Type is set to eAllCalls.</remarks>
        Friend Function GetClient3(ByRef Client As SIPClient, ClientIndex As Integer) As Boolean
            If Client Is Nothing Then Client = New SIPClient

            With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_GetClient3", New Hashtable From {{"NewVoIPAccountIndex", ClientIndex}})

                If .ContainsKey("NewX_AVM-DE_ClientUsername ") Then
                    Client.ClientUsername = .Item("NewX_AVM-DE_ClientUsername").ToString
                    Client.ClientRegistrar = .Item("NewX_AVM-DE_ClientRegistrar").ToString
                    Client.ClientRegistrarPort = CInt(.Item("NewX_AVM-DE_ClientRegistrarPort"))
                    Client.PhoneName = .Item("NewX_AVM-DE_PhoneName").ToString
                    Client.ClientId = .Item("NewX_AVM-DE_ClientId").ToString
                    Client.OutGoingNumber = .Item("NewX_AVM-DE_OutGoingNumber").ToString
                    If Not DeserializeXML(.Item("NewX_AVM-DE_InComingNumbers").ToString(), False, Client.InComingNumbers) Then
                        PushStatus.Invoke(LogLevel.Warn, $"NewX_AVM-DE_InComingNumbers konnte für nicht deserialisiert werden. '{ .Item("Error")}'")
                    End If
                    Client.ExternalRegistration = CBool(.Item("NewX_AVM-DE_ExternalRegistration"))
                    Client.InternalNumber = CInt(.Item("NewX_AVM-DE_InternalNumber"))
                    Client.DelayedCallNotification = CBool(.Item("NewX_AVM-DE_DelayedCallNotification"))

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"X_AVM-DE_GetClient3 konnte nicht aufgelößt werden. '{ .Item("Error")}'")

                    Return False
                End If
            End With
        End Function

        ''' <summary>
        ''' Get the configured common country code where the <paramref name="LKZ"/> represents the actual country code and the <paramref name="LKZPrefix"/> is the international call prefix.
        ''' </summary>
        ''' <param name="LKZ">Represents the actual country code.</param>
        ''' <param name="LKZPrefix">Represents the international call prefix.</param>
        ''' <returns>True when success</returns>
        Friend Function GetVoIPCommonCountryCode(ByRef LKZ As String, Optional ByRef LKZPrefix As String = "") As Boolean

            With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_GetVoIPCommonCountryCode", Nothing)

                If .ContainsKey("NewX_AVM-DE_LKZ") And .ContainsKey("NewX_AVM-DE_LKZPrefix") Then
                    LKZ = .Item("NewX_AVM-DE_LKZ").ToString
                    LKZPrefix = .Item("NewX_AVM-DE_LKZPrefix").ToString

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"LKZ und LKZPrefix konnten nicht ermittelt werden. '{ .Item("Error")}'")
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

            With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_GetVoIPCommonAreaCode", Nothing)

                If .ContainsKey("NewX_AVM-DE_OKZ") And .ContainsKey("NewX_AVM-DE_OKZPrefix") Then
                    OKZ = .Item("NewX_AVM-DE_OKZ").ToString
                    OKZPrefix = .Item("NewX_AVM-DE_OKZPrefix").ToString

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"OKZ und OKZPrefix konnten nicht ermittelt werden. '{ .Item("Error")}'")
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
            With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_DialGetConfig", Nothing)

                If .ContainsKey("NewX_AVM-DE_PhoneName") Then
                    PhoneName = .Item("NewX_AVM-DE_PhoneName").ToString

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"X_AVM-DE_DialGetConfig konnte nicht aufgelößt werden. '{ .Item("Error")}'")
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
            With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_DialHangup", Nothing)
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

        Friend Function GetNumberOfNumbers(ByRef NumberOfNumbers As Integer) As Boolean
            With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_GetNumberOfNumbers", Nothing)

                If .ContainsKey("NewNumberOfNumbers") Then
                    NumberOfNumbers = CInt(.Item("NewNumberOfNumbers"))

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"X_AVM-DE_GetNumberOfNumbers konnte nicht aufgelößt werden. '{ .Item("Error")}'")
                    NumberOfNumbers = 0

                    Return False
                End If
            End With
        End Function

        ''' <summary>
        ''' Return a list of all telephone numbers. 
        ''' </summary>
        ''' <param name="NumberList">Represents the list of all telephone numbers.</param>
        ''' <returns>True when success</returns>
        ''' <remarks>The list contains all configured numbers for all number types. The index can be used to see how many numbers are configured For one type. </remarks>
        Friend Function GetNumbers(ByRef NumberList As SIPTelNrList) As Boolean

            With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_GetNumbers", Nothing)

                If .ContainsKey("NewNumberList") Then

                    NLogger.Trace(.Item("NewNumberList"))

                    If Not DeserializeXML(.Item("NewNumberList").ToString(), False, NumberList) Then
                        PushStatus.Invoke(LogLevel.Warn, $"X_AVM-DE_GetNumbers konnte für nicht deserialisiert werden. '{ .Item("Error")}'")
                    End If

                    ' Wenn keine Nummern angeschlossen wurden, gib eine leere Klasse zurück
                    If NumberList Is Nothing Then NumberList = New SIPTelNrList

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"X_AVM-DE_GetNumbers konnte für nicht aufgelößt werden.")
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
                    PushStatus.Invoke(LogLevel.Warn, $"X_AVM-DE_GetPhonePort konnte für id {i} nicht aufgelößt werden. '{ .Item("Error")}'")
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

            With TR064Start(Tr064Files.x_voipSCPD, "X_AVM-DE_GetClients", Nothing)

                If .ContainsKey("NewX_AVM-DE_ClientList") Then

                    NLogger.Trace(.Item("NewX_AVM-DE_ClientList"))

                    If Not DeserializeXML(.Item("NewX_AVM-DE_ClientList").ToString(), False, ClientList) Then
                        PushStatus.Invoke(LogLevel.Warn, $"X_AVM-DE_GetNumbers konnte für nicht deserialisiert werden.")
                    End If

                    ' Wenn keine SIP-Clients angeschlossen wurden, gib eine leere Klasse zurück
                    If ClientList Is Nothing Then ClientList = New SIPClientList

                    Return True

                Else
                    PushStatus.Invoke(LogLevel.Warn, $"X_AVM-DE_GetClients konnte für nicht aufgelößt werden. '{ .Item("Error")}'")
                    ClientList = Nothing

                    Return False
                End If
            End With

        End Function

#End Region

    End Class

End Namespace