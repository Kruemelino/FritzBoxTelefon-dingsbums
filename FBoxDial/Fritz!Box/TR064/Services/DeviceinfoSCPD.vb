Imports System.Collections

Namespace TR064
    Public Class DeviceinfoSCPD
        Implements IService
        Private Property NLogger As Logger = LogManager.GetCurrentClassLogger Implements IService.NLogger
        Private Property TR064Start As Func(Of String, String, Hashtable, Hashtable) Implements IService.TR064Start
        Private Property PushStatus As Action(Of LogLevel, String) Implements IService.PushStatus

        Public Sub New(Start As Func(Of String, String, Hashtable, Hashtable), Status As Action(Of LogLevel, String))

            TR064Start = Start

            PushStatus = Status
        End Sub

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

            With TR064Start(Tr064Files.deviceinfoSCPD, "GetInfo", Nothing)

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

                    PushStatus.Invoke(LogLevel.Debug, $"Geräteinformationen der Fritz!Box: {Description}")

                    Return True
                Else
                    PushStatus.Invoke(LogLevel.Warn, $"Keine Geräteinformationen der Fritz!Box erhalten. '{ .Item("Error")}'")

                    Return False
                End If
            End With

        End Function
#End Region

    End Class
End Namespace
