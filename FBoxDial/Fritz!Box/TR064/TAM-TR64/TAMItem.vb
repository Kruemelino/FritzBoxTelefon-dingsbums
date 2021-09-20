﻿Imports System.Xml.Serialization
<Serializable()> Public Class TAMItem
    Inherits NotifyBase

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Private _Index As Integer
    <XmlElement("Index")> Public Property Index As Integer
        Get
            Return _Index
        End Get
        Set
            SetProperty(_Index, Value)
        End Set
    End Property

    Private _Display As Boolean
    <XmlElement("Display")> Public Property Display As Boolean
        Get
            Return _Display
        End Get
        Set
            SetProperty(_Display, Value)
        End Set
    End Property

    Private _Enable As Boolean
    <XmlElement("Enable")> Public Property Enable As Boolean
        Get
            Return _Enable
        End Get
        Set
            SetProperty(_Enable, Value)
        End Set
    End Property

    Private _Name As String
    <XmlElement("Name")> Public Property Name As String
        Get
            Return _Name
        End Get
        Set
            SetProperty(_Name, Value)
        End Set
    End Property

    Private _TAMInfo As TAMInfo
    <XmlIgnore> Public Property TAMInfo As TAMInfo
        Get
            Return _TAMInfo
        End Get
        Set
            SetProperty(_TAMInfo, Value)
        End Set
    End Property

    Friend Function GetTAMInformation(fboxTR064 As SOAP.FritzBoxTR64) As TAMInfo

        fboxTR064.GetTAMInfoEx(TAMInfo, Index)

        Return TAMInfo
    End Function
    Friend Sub ToggleTAMEnableState(fboxTR064 As SOAP.FritzBoxTR64)

        'Using fboxTR064 As New SOAP.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)

        ' Ermittle den aktuellen Status des Anrufbeantworters
        With GetTAMInformation(fboxTR064) ' TAMInfo
            Dim NewEnableState As Boolean = Not .Enable

            If fboxTR064.SetEnable(Index, NewEnableState) Then Enable = NewEnableState

            NLogger.Info($"Anrufbeantworter {Name} ({Index}) {If(NewEnableState, "aktiviert", "deaktiviert")}.")
        End With

        'End Using

    End Sub

End Class

