Public NotInheritable Class FritzBoxDefault

    Public Shared ReadOnly Property PDfltFritzBoxAdress() As String = "fritz.box"
    Public Shared ReadOnly Property PDfltFritzBoxUser As String = "admin"
    Public Shared ReadOnly Property PDfltFritzBoxIPAdress As String = "192.168.178.1"
    Public Shared ReadOnly Property PDfltFritzBoxName As String = "Fritz!Box"
    Public Shared ReadOnly Property PDfltAnrListFileName As String = "FRITZ!Box_Anrufliste.csv"
    Public Shared ReadOnly Property PDfltTelCodeActivateFritzBoxCallMonitor() As String = "#96*5*"
    Public Shared ReadOnly Property PDfltFBAnrMonPort() As Integer = 1012
    Public Shared ReadOnly Property PDfltFBSOAP() As Integer = 49000
    Public Shared ReadOnly Property PDfltFBSOAPSSL() As Integer = 49443
    Public Shared ReadOnly Property PDfltCodePageFritzBox() As Integer = 65001

#Region "Properties Fritz!Box Links"
    Public Shared ReadOnly Property PFBLinkBasis() As String
        Get
            If XMLData.POptionen.PValidFBAdr.IsNotStringEmpty Then
                XMLData.POptionen.PValidFBAdr = ValidIP(XMLData.POptionen.PTBFBAdr)
            End If
            Return "http://" & XMLData.POptionen.PValidFBAdr
        End Get
    End Property
    'Private ReadOnly Property PFBLinkLoginLua_Basis() As String = PFBLinkBasis & "/login_sid.lua?"
    'Private ReadOnly Property PFBLinkLoginLuaTeil1(ByVal sSID As String) As String
    '    Get
    '        Return PFBLinkLoginLua_Basis & sSID
    '    End Get
    'End Property
    'Private ReadOnly Property PFBLinkLoginLuaTeil2(ByVal FBBenutzer As String, ByVal SIDResponse As String) As String
    '    Get
    '        Return PFBLinkLoginLua_Basis & "username=" & FBBenutzer & "&response=" & SIDResponse
    '    End Get
    'End Property
    'Private ReadOnly Property PFBLinkExtBasis() As String = PFBLinkBasis & "/cgi-bin/webcm"
    'Private ReadOnly Property PFBLinkLogoutLuaNeu(ByVal sSID As String) As String
    '    Get
    '        Return PFBLinkBasis & "/home/home.lua?" & sSID & "&logout=1"
    '    End Get
    'End Property
    'Telefone
    'Private ReadOnly Property PFBLinkTel1(ByVal sSID As String) As String
    '    Get
    '        Return PFBLinkBasis & "/fon_num/fon_num_list.lua?" & sSID
    '    End Get
    'End Property
    'Private ReadOnly Property PFBLinkTelAlt1(ByVal sSID As String) As String
    '    Get
    '        Return PFBLinkExtBasis & "?" & sSID & "&getpage=../html/de/menus/menu2.html&var:lang=de&var:menu=fon&var:pagename=fondevices"
    '    End Get
    'End Property

    ' Wählen
    'Private ReadOnly Property PFBLinkJI1(ByVal sSID As String) As String
    '    Get
    '        Return PFBLinkBasis & "/fon_num/foncalls_list.lua?" & sSID
    '    End Get
    'End Property
    'Private ReadOnly Property PFBLinkJI2(ByVal sSID As String) As String
    '    Get
    '        Return PFBLinkJI1(sSID) & "&csv="
    '    End Get
    'End Property
    'Private ReadOnly Property PFBLinkJIAlt_Basis(ByVal sSID As String) As String
    '    Get
    '        Return PFBLinkBasis & "/cgi-bin/webcm?" & sSID & "&getpage=../html/de/"
    '    End Get
    'End Property
    'Private ReadOnly Property PFBLinkJIAlt_Child1(ByVal sSID As String) As String
    '    Get
    '        Return PFBLinkJIAlt_Basis(sSID) & "menus/menu2.html&var:lang=de&var:menu=fon&var:pagename=foncalls"
    '    End Get
    'End Property
    'Private ReadOnly Property PFBLinkJIAlt_Child2(ByVal sSID As String) As String
    '    Get
    '        Return PFBLinkJIAlt_Basis(sSID) & PDfltAnrListFileName
    '    End Get
    'End Property
    ' Telefonbuch
    'Private ReadOnly Property PFBLinkFonBook_Entry() As String = PFBLinkBasis & "/fon_num/fonbook_entry.lua"
    'Private ReadOnly Property PFBLinkData() As String = PFBLinkBasis & "/data.lua"
    'Private ReadOnly Property PFBLinkExportAdressbook() As String = PFBLinkBasis & "/cgi-bin/firmwarecfg"
    'Private ReadOnly Property PFBLinkTelefonbuch_List(ByVal sSID As String) As String
    '    Get
    '        Return PFBLinkBasis & "/fon_num/fonbook_select.lua?" & sSID
    '    End Get
    'End Property


    'Private ReadOnly Property PFBLinkLED_Display() As String = PFBLinkBasis & "/system/led_display.lua"
    'Private ReadOnly Property PFBLinkJason_Boxinfo() As String = PFBLinkBasis & "/jason_boxinfo.xml"
    'Private ReadOnly Property PFBLinkSystemStatus() As String = PFBLinkBasis & "/cgi-bin/system_status"
#End Region

#Region "Fritz!Box Querys"
    ''' <summary>
    ''' "POTS=telcfg:settings/MSN/POTS"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_POTS() As String = "POTS=telcfg:settings/MSN/POTS"

    ''' <summary>
    ''' "Mobile=telcfg:settings/Mobile/MSN"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_Mobile() As String = "Mobile=telcfg:settings/Mobile/MSN"

    ''' <summary>
    ''' "Port" &amp; <c>idx</c> &amp; "Name=telcfg:settings/MSN/Port" &amp; <c>idx</c> &amp; "/Name"
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    ''' <remarks>
    ''' "S0" &amp; i &amp; "Name=telcfg:settings/NTHotDialList/Name" &amp; i
    ''' "S0" &amp; i &amp; "Number=telcfg:settings/NTHotDialList/Number" &amp; i
    ''' </remarks>
    Public Shared ReadOnly Property P_Query_FB_FON(ByVal idx As Integer) As String
        Get
            Return "Port" & idx & "Name=telcfg:settings/MSN/Port" & idx & "/Name"
        End Get
    End Property

    ''' <summary>
    ''' "TAM" &amp; <c>idx</c> &amp; "=tam:settings/MSN" &amp; <c>idx</c>
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_TAM(ByVal idx As Integer) As String
        Get
            Return "TAM" & idx & "=tam:settings/MSN" & idx
        End Get
    End Property

    ''' <summary>
    ''' "FAX" &amp; <c>idx</c> &amp; "=telcfg:settings/FaxMSN" &amp; <c>idx</c>
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_FAX(ByVal idx As Integer) As String
        Get
            Return "FAX" & idx & "=telcfg:settings/FaxMSN" & idx
        End Get
    End Property

    ''' <summary>
    ''' "MSN" &amp; <c>idx</c> &amp; "=telcfg:settings/MSN/MSN" &amp; <c>idx</c>
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_MSN(ByVal idx As Integer) As String
        Get
            Return "MSN" & idx & "=telcfg:settings/MSN/MSN" & idx
        End Get
    End Property

    ''' <summary>
    ''' "VOIP" &amp; <c>idx</c> &amp; "Enabled=" &amp; "telcfg:settings/VoipExtension" &amp; <c>idx</c> &amp; "/enabled"
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_VOIP(ByVal idx As Integer) As String
        Get
            Return "VOIP" & idx & "Enabled=" & "telcfg:settings/VoipExtension" & idx & "/enabled"
        End Get
    End Property
    Public Shared ReadOnly Property P_Query_FB_VOIPa(ByVal VoipExtensionidx As String) As String
        Get
            Return "VOIPEnabled=" & "telcfg:settings/" & VoipExtensionidx & "/enabled"
        End Get
    End Property
    ''' <summary>
    ''' "SIP" &amp; "=" &amp; "sip:settings/sip/list(activated,displayname,ID)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_SIP() As String
        Get
            Return "SIP" & "=" & "sip:settings/sip/list(activated,displayname,ID)"
        End Get
    End Property

    Public Shared ReadOnly Property P_Query_FB_TelList_Header(ByVal jdx As Integer) As String
        Get
            Return "TelNr" & jdx
        End Get
    End Property

    ''' <summary>
    ''' "MSN" &amp; idx &amp; "Nr" &amp; jdx &amp; "=telcfg:settings/MSN/Port" &amp; idx &amp; "/MSN" &amp; jdx
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_MSN_TelNrList(ByVal idx As Integer, ByVal jdx As Integer) As String
        Get
            Return P_Query_FB_TelList_Header(jdx) & "=telcfg:settings/MSN/Port" & idx & "/MSN" & jdx
        End Get
    End Property

    ''' <summary>
    ''' "VOIP" &amp; idx &amp; "Nr" &amp; jdx &amp; "=telcfg:settings/VoipExtension" &amp; idx &amp; "/Number" &amp; jdx
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_VOIP_TelNrList(ByVal idx As Integer, ByVal jdx As Integer) As String
        Get
            Return P_Query_FB_TelList_Header(jdx) & "=telcfg:settings/VoipExtension" & idx & "/Number" & jdx
        End Get
    End Property

    ''' <summary>
    ''' "FON=telcfg:settings/MSN/Port/list(Name,Fax,GroupCall,AllIncomingCalls,OutDialing)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_FON_List() As String = "FON=telcfg:settings/MSN/Port/list(Name,Fax)"

    ''' <summary>
    ''' "DECT=telcfg:settings/Foncontrol/User/list(Name,Type,Intern,Id)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_DECT_List() As String = "DECT=telcfg:settings/Foncontrol/User/list(Name,Type,Intern,Id)"

    ''' <summary>
    ''' "VOIP=telcfg:settings/VoipExtension/list(enabled,Name,RingOnAllMSNs)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_VOIP_List() As String = "VOIP=telcfg:settings/VoipExtension/list(enabled,Name)"

    ''' <summary>
    ''' "TAM=tam:settings/TAM/list(Name,Display,Active,MSNBitmap,NumNewMessages,NumOldMessages)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_TAM_List() As String = "TAM=tam:settings/TAM/list(Active,Name)"

    ''' <summary>
    ''' "S0" &amp; <c>Type</c> &amp; <c>idx</c> &amp; "=telcfg:settings/NTHotDialList/" &amp; <c>Type</c> &amp; <c>idx</c>
    ''' </summary>
    ''' <param name="Type">Der Typ des Eintrages: Name oder Number</param>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    ''' <remarks>
    ''' "S0" &amp; i &amp; "Name=telcfg:settings/NTHotDialList/Name" &amp; i
    ''' "S0" &amp; i &amp; "Number=telcfg:settings/NTHotDialList/Number" &amp; i
    ''' </remarks>
    Public Shared ReadOnly Property P_Query_FB_S0(ByVal Type As String, ByVal idx As Integer) As String
        Get
            Return "S0" & Type & idx & "=telcfg:settings/NTHotDialList/" & Type & idx
        End Get
    End Property

    ''' <summary>
    ''' "DECT" &amp; idx &amp; "RingOnAllMSNs=telcfg:settings/Foncontrol/User" &amp; idx &amp; "/RingOnAllMSNs"
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_DECT_RingOnAllMSNs(ByVal idx As Integer) As String
        Get
            Return "DECT" & idx & "RingOnAllMSNs=telcfg:settings/Foncontrol/User" & idx & "/RingOnAllMSNs"
        End Get
    End Property

    ''' <summary>
    ''' "DECT" &amp; idx &amp; "Nr=telcfg:settings/Foncontrol/User" &amp; idx &amp; "/MSN/list(Number)"
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_DECT_NrList(ByVal idx As Integer) As String
        Get
            Return "DECT" & idx & "Nr=telcfg:settings/Foncontrol/User" & idx & "/MSN/list(Number)"
        End Get
    End Property

    ''' <summary>
    ''' "FaxMailActive=telcfg:settings/FaxMailActive"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_FaxMailActive() As String = "FaxMailActive=telcfg:settings/FaxMailActive"

    ''' <summary>
    ''' "MobileName=telcfg:settings/Mobile/Name"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_MobileName() As String = "MobileName=telcfg:settings/Mobile/Name"

    ''' <summary>
    ''' LKZPrefix=telcfg:settings/Location/LKZPrefix
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>

    Public Shared ReadOnly Property P_Query_FB_LKZPrefix() As String = "LKZPrefix=telcfg:settings/Location/LKZPrefix"
    ''' <summary>
    ''' LKZ=telcfg:settings/Location/LKZ
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>

    Public Shared ReadOnly Property P_Query_FB_LKZ() As String = "LKZ=telcfg:settings/Location/LKZ"
    ''' <summary>
    ''' OKZPrefix=telcfg:settings/Location/OKZPrefix
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_OKZPrefix() As String = "OKZPrefix=telcfg:settings/Location/OKZPrefix"

    ''' <summary>
    ''' OKZ=telcfg:settings/Location/OKZ
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Public Shared ReadOnly Property P_Query_FB_OKZ() As String = "OKZ=telcfg:settings/Location/OKZ"

#End Region

End Class
