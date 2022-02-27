Friend NotInheritable Class FritzBoxDefault
    Friend Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Friend Const DfltFritzBoxAdress As String = "192.168.178.1"
    Friend Const DfltFritzBoxHostName As String = "fritz.box"
    Friend Const DfltFritzBoxSessionID As String = "0000000000000000"
    Friend Const DfltCodePageFritzBox As Integer = 65001
    Friend Const DfltTR064PortSSL As Integer = 49443
    ''' <summary>
    ''' Anmeldeinformationen für die Fritz!Box
    ''' </summary>
    ''' <returns><see cref="Net.NetworkCredential"/></returns>
    Friend Shared ReadOnly Property Anmeldeinformationen As Net.NetworkCredential
        Get
            ' Falls noch kein Benutzer gesetzt wurde, dann muss der Standard gesetzt sein
            If XMLData.POptionen.TBBenutzer.IsStringNothingOrEmpty Then XMLData.POptionen.TBBenutzer = GetDefaultUserName

            Using Crypter As New Rijndael
                Return New Net.NetworkCredential(XMLData.POptionen.TBBenutzer, Crypter.DecryptString(XMLData.POptionen.TBPasswort, My.Resources.strDfltDeCryptKey))
            End Using
        End Get
    End Property

    Friend Shared ReadOnly Property GetDefaultUserName As String
        Get
            ' Eine Unterscheidung nach Firmware ist erforderlich.
            If Globals.ThisAddIn.FBoxTR064 IsNot Nothing Then
                With Globals.ThisAddIn.FBoxTR064
                    If .Major.IsLargerOrEqual(7) And .Minor.IsLargerOrEqual(24) Then
                        ' ermittle den zuletzt angemeldeten User
                        Dim XMLString As String = String.Empty
                        Dim FritzBoxUsers As New FBoxAPI.UserList

                        If .LANConfigSecurity.GetUserList(XMLString) AndAlso DeserializeXML(XMLString, False, FritzBoxUsers) Then
                            NLogger.Info($"Benutzername zum Login auf zuletzt genutzten User gesetzt: '{FritzBoxUsers.GetLastUsedUser.UserName}'")
                            Return FritzBoxUsers.GetLastUsedUser.UserName
                        Else
                            NLogger.Warn($"Benutzername zum Login konnte nucht ermittelt werden: '{ .DisplayVersion}'")
                            Return String.Empty
                        End If
                    Else
                        ' Default Username der älteren Versionen vor 7.24
                        NLogger.Info("Benutzername zum Login auf alten Standardwert gesetzt: 'admin'")
                        Return "admin"
                    End If
                End With
            Else
                NLogger.Info("Benutzername zum Login konnte nucht ermittelt werden, da TR-064 nicht bereit.")
                Return String.Empty
            End If
        End Get
    End Property

    Friend Shared Function CompleteURL(PathSegment As String) As String
        Dim SessionID As String = DfltFritzBoxSessionID
        ' Ermittle die SessionID. Sollte das schief gehen, kommt es zu einer Fehlermeldung im Log.
        If Globals.ThisAddIn.FBoxTR064.Deviceconfig.GetSessionID(SessionID) Then
            Return $"https://{XMLData.POptionen.ValidFBAdr}:{DfltTR064PortSSL}{PathSegment}&{SessionID}"
        Else
            Return String.Empty
        End If
    End Function
End Class
