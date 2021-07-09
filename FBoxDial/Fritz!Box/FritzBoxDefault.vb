Friend NotInheritable Class FritzBoxDefault
    Friend Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Const DfltFritzBoxAdress As String = "192.168.178.1"
    Friend Const DfltFritzBoxHostName As String = "fritz.box"
    Friend Const DfltFritzBoxSessionID As String = "0000000000000000"
    Friend Const DfltCodePageFritzBox As Integer = 65001

    ''' <summary>
    ''' Anmeldeinformationen für die Fritz!Box
    ''' </summary>
    ''' <returns><see cref="Net.NetworkCredential"/></returns>
    Friend Shared ReadOnly Property Anmeldeinformationen As Net.NetworkCredential
        Get
            ' Falls noch kein Benutzer gesetzt wurde, dann muss der Standard gesetzt sein
            If XMLData.POptionen.TBBenutzer.IsStringNothingOrEmpty Then XMLData.POptionen.TBBenutzer = GetDefaultUserName

            Using Crypter As New Rijndael
                Return New Net.NetworkCredential(XMLData.POptionen.TBBenutzer, Crypter.DecryptString(XMLData.POptionen.TBPasswort, DfltDeCryptKey))
            End Using
        End Get
    End Property

    Friend Shared ReadOnly Property GetDefaultUserName As String
        Get
            ' Prüfe, ob Fritz!Box verfügbar
            If Ping(XMLData.POptionen.ValidFBAdr) Then
                ' Eine Unterscheidung nach Firmware ist erforderlich.
                Using FBTR064 As New SOAP.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, Nothing)
                    With FBTR064
                        If .Major.IsLargerOrEqual(7) And .Minor.IsLargerOrEqual(24) Then
                            ' ermittle den zuletzt angemeldeten User
                            Dim XMLString As String = DfltStringEmpty
                            Dim FritzBoxUsers As New FritzBoxXMLUserList

                            If .GetUserList(XMLString) AndAlso DeserializeXML(XMLString, False, FritzBoxUsers) Then
                                NLogger.Info($"Benutzername zum Login auf zuletzt genutzten User gesetzt: '{FritzBoxUsers.GetLastUsedUser.UserName}'")
                                Return FritzBoxUsers.GetLastUsedUser.UserName
                            Else
                                NLogger.Warn($"Benutzername zum Login konnte nucht ermittelt werden: '{ .DisplayVersion}'")
                                Return DfltStringEmpty
                            End If
                        Else
                            ' Default Username der älteren Versionen vor 7.24
                            NLogger.Info("Benutzername zum Login auf alten Standardwert gesetzt: 'admin'")
                            Return "admin"
                        End If
                    End With
                End Using
            Else
                NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
                Return DfltStringEmpty
            End If
        End Get
    End Property
End Class
