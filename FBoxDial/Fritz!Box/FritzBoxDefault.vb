Imports System.Xml.Schema

Friend NotInheritable Class FritzBoxDefault

    Friend Shared ReadOnly Property DfltFritzBoxAdress() As String = "fritz.box"
    Friend Shared ReadOnly Property DfltFritzBoxUser As String = "admin"
    Friend Shared ReadOnly Property DfltFritzBoxIPAdress As String = "192.168.178.1"
    Friend Shared ReadOnly Property DfltFritzBoxName As String = "Fritz!Box"
    Friend Shared ReadOnly Property DfltTelCodeActivateFritzBoxCallMonitor() As String = "#96*5*"
    Friend Shared ReadOnly Property DfltFBAnrMonPort() As Integer = 1012
    Friend Shared ReadOnly Property DfltCodePageFritzBox() As Integer = 65001

#Region "Properties Fritz!Box Links"
    Friend Shared ReadOnly Property FBLinkBasis() As String
        Get
            If XMLData.POptionen.ValidFBAdr.IsNotStringEmpty Then
                XMLData.POptionen.ValidFBAdr = ValidIP(XMLData.POptionen.TBFBAdr)
            End If
            Return "http://" & XMLData.POptionen.ValidFBAdr
        End Get
    End Property
#End Region

#Region "Fritz!Box Querys"
    ''' <summary>
    ''' "POTS=telcfg:settings/MSN/POTS"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryPOTS() As String = "POTS=telcfg:settings/MSN/POTS"

    ''' <summary>
    ''' "Mobile=telcfg:settings/Mobile/MSN"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryMobile() As String = "Mobile=telcfg:settings/Mobile/MSN"

    ''' <summary>
    ''' Port<paramref name="idx"/>Name=telcfg:settings/MSN/Port<paramref name="idx"/>/Name
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    ''' <remarks>
    ''' "S0" &amp; i &amp; "Name=telcfg:settings/NTHotDialList/Name" &amp; i
    ''' "S0" &amp; i &amp; "Number=telcfg:settings/NTHotDialList/Number" &amp; i
    ''' </remarks>
    Friend Shared ReadOnly Property FBoxQueryFON(idx As Integer) As String
        Get
            Return $"Port{idx}Name=telcfg:settings/MSN/Port{idx}/Name"
        End Get
    End Property

    ''' <summary>
    ''' TAM<paramref name="idx"/>=tam:settings/MSN<paramref name="idx"/>
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryTAM(idx As Integer) As String
        Get
            Return $"TAM{idx}=tam:settings/MSN{idx}"
        End Get
    End Property

    ''' <summary>
    ''' FAX<paramref name="idx"/>=telcfg:settings/FaxMSN<paramref name="idx"/>
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryFAX(idx As Integer) As String
        Get
            Return $"FAX{idx}=telcfg:settings/FaxMSN{idx}"
        End Get
    End Property

    ''' <summary>
    ''' MSN<paramref name="idx"/>=telcfg:settings/MSN/MSN<paramref name="idx"/>
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryMSN(idx As Integer) As String
        Get
            Return $"MSN{idx}=telcfg:settings/MSN/MSN{idx}"
        End Get
    End Property

    ''' <summary>
    ''' VOIP<paramref name="idx"/>Enabled=telcfg:settings/VoipExtension<paramref name="idx"/>/enabled
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryVOIP(idx As Integer) As String
        Get
            Return $"VOIP{idx}Enabled=telcfg:settings/VoipExtension{idx}/enabled"
        End Get
    End Property

    ''' <summary>
    ''' SIP=sip:settings/sip/list(activated,displayname,ID)
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQuerySIP() As String = "SIP=sip:settings/sip/list(activated,displayname,ID)"

    ''' <summary>
    ''' TelNr<paramref name="jdx"/>=telcfg:settings/MSN/Port<paramref name="idx"/>/MSN<paramref name="jdx"/>
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <param name="jdx">Der Index des Eintrage</param>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryMSNTelNrList(idx As Integer, jdx As Integer) As String
        Get
            Return $"TelNr{jdx}=telcfg:settings/MSN/Port{idx}/MSN{jdx}"
        End Get
    End Property

    ''' <summary>
    ''' TelNr<paramref name="jdx"/>=telcfg:settings/VoipExtension<paramref name="idx"/>/Number<paramref name="jdx"/>
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <param name="jdx">Der Index des Eintrage</param>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryVOIPTelNrList(idx As Integer, jdx As Integer) As String
        Get
            Return $"TelNr{jdx}=telcfg:settings/VoipExtension{idx}/Number{jdx}"
        End Get
    End Property

    ''' <summary>
    ''' "FON=telcfg:settings/MSN/Port/list(Name,Fax,GroupCall,AllIncomingCalls,OutDialing)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryFONList() As String = "FON=telcfg:settings/MSN/Port/list(Name,Fax)"

    ''' <summary>
    ''' "DECT=telcfg:settings/Foncontrol/User/list(Name,Type,Intern,Id)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryDECTList() As String = "DECT=telcfg:settings/Foncontrol/User/list(Name,Type,Intern,Id)"

    ''' <summary>
    ''' "VOIP=telcfg:settings/VoipExtension/list(enabled,Name,RingOnAllMSNs)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryVOIP() As String = "VOIP=telcfg:settings/VoipExtension/list(enabled,Name)"

    ''' <summary>
    ''' "TAM=tam:settings/TAM/list(Name,Display,Active,MSNBitmap,NumNewMessages,NumOldMessages)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryTAMList() As String = "TAM=tam:settings/TAM/list(Active,Name)"

    ''' <summary>
    ''' S0<paramref name="Type"/><paramref name="idx"/>=telcfg:settings/NTHotDialList/<paramref name="Type"/><paramref name="idx"/>
    ''' </summary>
    ''' <param name="Type">Der Typ des Eintrages: Name oder Number</param>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    ''' <remarks>
    ''' "S0" &amp; i &amp; "Name=telcfg:settings/NTHotDialList/Name" &amp; i
    ''' "S0" &amp; i &amp; "Number=telcfg:settings/NTHotDialList/Number" &amp; i
    ''' </remarks>
    Friend Shared ReadOnly Property FBoxQueryS0(Type As String, idx As Integer) As String
        Get
            Return $"S0{Type}{idx}=telcfg:settings/NTHotDialList/{Type}{idx}"
        End Get
    End Property

    ''' <summary>
    ''' DECT<paramref name="idx"/>RingOnAllMSNs=telcfg:settings/Foncontrol/User<paramref name="idx"/>/RingOnAllMSNs
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryDECTRingOnAllMSNs(ByVal idx As Integer) As String
        Get
            Return $"DECT{idx}RingOnAllMSNs=telcfg:settings/Foncontrol/User{idx}/RingOnAllMSNs"
        End Get
    End Property

    ''' <summary>
    ''' DECT<paramref name="idx"/>Nr=telcfg:settings/Foncontrol/User<paramref name="idx"/>/MSN/list(Number)"
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryDECTNrList(ByVal idx As Integer) As String
        Get
            Return $"DECT{idx}Nr=telcfg:settings/Foncontrol/User{idx}/MSN/list(Number)"
        End Get
    End Property

    ''' <summary>
    ''' "FaxMailActive=telcfg:settings/FaxMailActive"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryFaxMailActive() As String = "FaxMailActive=telcfg:settings/FaxMailActive"

    ''' <summary>
    ''' "MobileName=telcfg:settings/Mobile/Name"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryMobileName() As String = "MobileName=telcfg:settings/Mobile/Name"

    ''' <summary>
    ''' LKZPrefix=telcfg:settings/Location/LKZPrefix
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryLKZPrefix() As String = "LKZPrefix=telcfg:settings/Location/LKZPrefix"

    ''' <summary>
    ''' LKZ=telcfg:settings/Location/LKZ
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryLKZ() As String = "LKZ=telcfg:settings/Location/LKZ"

    ''' <summary>
    ''' OKZPrefix=telcfg:settings/Location/OKZPrefix
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryKZPrefix() As String = "OKZPrefix=telcfg:settings/Location/OKZPrefix"

    ''' <summary>
    ''' OKZ=telcfg:settings/Location/OKZ
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Friend Shared ReadOnly Property FBoxQueryOKZ() As String = "OKZ=telcfg:settings/Location/OKZ"

#End Region

#Region "Fritz!Box SOAP/TR64"
    Friend Shared ReadOnly Property DfltSOAPPort() As Integer = 49000
    Friend Shared ReadOnly Property DfltSOAPPortSSL() As Integer = 49443
    ''' <summary>
    ''' nameSpace URL: http://schemas.xmlsoap.org/soap/envelope/
    ''' </summary>
    Friend Shared ReadOnly Property DfltSOAPRequestNameSpaceEnvelope() As String = "http://schemas.xmlsoap.org/soap/envelope/"
    ''' <summary>
    ''' nameSpace URL: http://schemas.xmlsoap.org/soap/encoding/
    ''' </summary>
    Friend Shared ReadOnly Property DfltSOAPRequestNameSpaceEncoding() As String = "http://schemas.xmlsoap.org/soap/encoding/"
    Friend Shared ReadOnly Property DfltSOAPRequestSchema() As XmlSchema
        Get
            Dim XMLSOAPSchema As New XmlSchema

            With XMLSOAPSchema.Namespaces
                .Add("s", DfltSOAPRequestNameSpaceEnvelope)
                .Add("u", DfltSOAPRequestNameSpaceEncoding)
            End With

            Return XMLSOAPSchema
        End Get
    End Property
#End Region

#Region "Fritz!Box Telefonbuch"

    Friend Shared ReadOnly Property DfltTelBuchTelTyp() As List(Of KeyValuePair(Of String, String))
        Get
            Dim values As New List(Of KeyValuePair(Of String, String)) From {
                New KeyValuePair(Of String, String)("home", "Privat"),
                New KeyValuePair(Of String, String)("work", "Arbeit"),
                New KeyValuePair(Of String, String)("intern", "Intern"),
                New KeyValuePair(Of String, String)("fax", "Fax Privat"),
                New KeyValuePair(Of String, String)("fax_work", "Fax Arbeit"),
                New KeyValuePair(Of String, String)("mobile", "Mobil"),
                New KeyValuePair(Of String, String)("memo", "Memo")
            }
            Return values
        End Get
    End Property


#End Region
End Class
