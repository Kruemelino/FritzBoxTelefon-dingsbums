Imports System.Xml.Serialization
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

    <XmlIgnore> Friend Property MessageList As FritzBoxXMLMessageList

    Friend Function GetTAMInformation(fboxTR064 As SOAP.FritzBoxTR64) As TAMInfo
        With fboxTR064.X_tam
            ' Lade die erweiterten TAM Infosätze herunter
            If .GetTAMInfo(TAMInfo, Index) Then
                ' Wenn der TAM aktiv und angezeigt wird, dann ermittle die URL zur MessageList
                Dim MessageListURL As String = DfltStringEmpty
                If Enable And Display AndAlso .GetMessageList(MessageListURL, Index) Then
                    ' Deserialisiere die MessageList
                    If DeserializeXML(MessageListURL, True, MessageList) Then
                        NLogger.Debug($"{MessageList.Messages.Count} TAM Einträge von {MessageListURL} eingelesen.")
                    Else
                        NLogger.Warn($"TAM Einträge von {MessageListURL} nicht eingelesen.")
                    End If
                End If

            End If

        End With

        Return TAMInfo
    End Function

    Friend Sub ToggleTAMEnableState()

        Using fboxTR064 As New SOAP.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)

            ' Ermittle den aktuellen Status des Anrufbeantworters
            With GetTAMInformation(fboxTR064) ' TAMInfo
                Dim NewEnableState As Boolean = Not .Enable

                If fboxTR064.X_tam.SetEnable(Index, NewEnableState) Then Enable = NewEnableState

                NLogger.Info($"Anrufbeantworter {Name} ({Index}) {If(NewEnableState, "aktiviert", "deaktiviert")}.")
            End With

        End Using

    End Sub

End Class

