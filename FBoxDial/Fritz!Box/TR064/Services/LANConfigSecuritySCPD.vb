Imports System.Collections
Namespace SOAP
    Public Class LANConfigSecuritySCPD
        Implements IService
        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger Implements IService.NLogger
        Private Property TR064Start As Func(Of String, String, Hashtable, Hashtable) Implements IService.TR064Start
        Private Property PushStatus As Action(Of LogLevel, String) Implements IService.PushStatus

        Public Sub New(Start As Func(Of String, String, Hashtable, Hashtable), Status As Action(Of LogLevel, String))

            TR064Start = Start

            PushStatus = Status
        End Sub

#Region "LANConfigSecurity"
        ''' <summary>
        ''' Get the usernames of all users in a xml-list. Each item has an attribute “last_user”, which is set to '1' for only that username, which was used since last login.
        ''' </summary>
        ''' <param name="UserList">Get the usernames of all users in a xml-list.</param>
        ''' <returns>True when success</returns>
        Friend Function GetUserList(ByRef UserList As String) As Boolean

            With TR064Start(Tr064Files.lanconfigsecuritySCPD, "X_AVM-DE_GetUserList", Nothing)

                If .ContainsKey("NewX_AVM-DE_UserList") Then

                    UserList = .Item("NewX_AVM-DE_UserList").ToString

                    PushStatus.Invoke(LogLevel.Debug, $"Userliste der Fritz!Box: '{UserList}'")

                    Return True
                Else
                    UserList = DfltStringEmpty

                    PushStatus.Invoke(LogLevel.Warn, $"Userliste der Fritz!Box konnte nicht ermittelt. '{ .Item("Error")}'")

                    Return False
                End If
            End With

        End Function
#End Region


    End Class
End Namespace
