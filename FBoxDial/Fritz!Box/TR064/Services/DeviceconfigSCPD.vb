Imports System.Collections
Namespace SOAP
    Public Class DeviceconfigSCPD
        Implements IService
        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger Implements IService.NLogger
        Private Property TR064Start As Func(Of String, String, Hashtable, Hashtable) Implements IService.TR064Start
        Private Property PushStatus As Action(Of LogLevel, String) Implements IService.PushStatus

        Public Sub New(Start As Func(Of String, String, Hashtable, Hashtable), Status As Action(Of LogLevel, String))

            TR064Start = Start

            PushStatus = Status
        End Sub

#Region "deviceconfigSCPD"

        ''' <summary>
        ''' Generate a temporary URL session ID. The session ID is need for accessing URLs like phone book, call list, FAX message, answering machine messages Or phone book images.
        ''' </summary>
        ''' <param name="SessionID">Represents the temporary URL session ID.</param>
        ''' <returns>True when success</returns>
        Friend Function GetSessionID(ByRef SessionID As String) As Boolean

            With TR064Start(Tr064Files.deviceconfigSCPD, "X_AVM-DE_CreateUrlSID", Nothing)

                If .ContainsKey("NewX_AVM-DE_UrlSID") Then

                    SessionID = .Item("NewX_AVM-DE_UrlSID").ToString

                    PushStatus.Invoke(LogLevel.Debug, $"Aktuelle SessionID der Fritz!Box: {SessionID}")

                    Return True
                Else
                    SessionID = FritzBoxDefault.DfltFritzBoxSessionID

                    PushStatus.Invoke(LogLevel.Warn, $"Keine SessionID der Fritz!Box erhalten. Rückgabewert: '{SessionID}' '{ .Item("Error")}'")

                    Return False
                End If
            End With

        End Function
#End Region

    End Class
End Namespace
