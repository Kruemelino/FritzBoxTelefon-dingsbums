Imports System.Windows.Media
Imports System.Xml.Serialization
<Serializable(), XmlType("person")> Public Class FritzBoxXMLPerson
    Inherits NotifyBase

    Private _RealName As String
    Private _ImageURL As String

    ''' <summary>
    ''' Name of Contact 
    ''' </summary>
    <XmlElement("realName")> Public Property RealName As String
        Get
            Return _RealName
        End Get
        Set
            SetProperty(_RealName, Value)
        End Set
    End Property

    ''' <summary>
    ''' A telephone book may contain URLs with an image for the contact. 
    ''' The content can be downloaded using the protocol, hostname and port with the image URL.
    ''' An example is described here:<br/>
    ''' Protocol: https<br/>
    ''' Hostname: fritz.box<br/>
    ''' Port: 49443<br/>
    ''' image URL : /download.lua?path=/var/media/ftp/JetFlash-Transcend4GB-01/FRITZ/fonpix/1316705057-0.jpg<br/>
    ''' The combination of Protocoll + :// + Hostname + : + Port + image URL will be the complete URL<br/>
    ''' https://fritz.box:49443/download.lua?path=/var/media/ftp/JetFlash-Transcend4GB01/FRITZ/fonpix/1316705057-0.jpg<br/>
    ''' Please note, that this URL might require authentication. 
    ''' </summary>
    ''' <returns>HTTP URL to image for this contact</returns>
    <XmlElement("imageURL")> Public Property ImageURL As String
        Get
            Return _ImageURL
        End Get
        Set
            SetProperty(_ImageURL, Value)
        End Set
    End Property

    Private _ImageData As ImageSource
    <XmlIgnore> Public Property ImageData As ImageSource
        Get
            Return _ImageData
        End Get
        Set
            SetProperty(_ImageData, Value)
        End Set
    End Property

    <XmlIgnore> Friend ReadOnly Property CompleteImageURL(Optional SessionID As String = FritzBoxDefault.DfltFritzBoxSessionID) As String
        Get
            If SessionID.AreEqual(FritzBoxDefault.DfltFritzBoxSessionID) Then
                If Ping(XMLData.POptionen.ValidFBAdr) Then
                    Using fbtr064 As New SOAP.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)
                        fbtr064.GetSessionID(SessionID)
                        ' Session ID erhalten, ansonnsten DfltFritzBoxSessionID
                    End Using
                End If
            End If

            If SessionID.AreNotEqual(FritzBoxDefault.DfltFritzBoxSessionID) Then
                Return $"https://{XMLData.POptionen.ValidFBAdr}:{SOAP.DfltTR064PortSSL}{ImageURL}&{SessionID}"
            Else
                Return DfltStringEmpty
            End If
        End Get

    End Property

End Class
