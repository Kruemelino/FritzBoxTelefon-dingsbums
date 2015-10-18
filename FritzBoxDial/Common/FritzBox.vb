Imports System.Text
Imports System.Xml
Imports System.Threading
Imports System.ComponentModel

Public Class FritzBox
    Implements IDisposable

    Private C_XML As XML
    Private C_DP As DataProvider
    Private C_Crypt As Rijndael
    Private C_hf As Helfer
    Private C_FBoxUPnP As FritzBoxServices

    Private FBFehler As Boolean
    Private FBEncoding As System.Text.Encoding = Encoding.UTF8

    Private tb As New System.Windows.Forms.TextBox
    Private EventProvider As IEventProvider

    Private bValSpeichereDaten As Boolean = True
    Private ThisFBFirmware As FritzBoxFirmware
    Private sSID As String
    Private sFirmware As String
    Private WithEvents BWSetDialPort As BackgroundWorker

#Region "Properties"
    Friend Property P_SpeichereDaten() As Boolean
        Get
            Return bValSpeichereDaten
        End Get
        Set(ByVal value As Boolean)
            bValSpeichereDaten = value
        End Set
    End Property

    Private Property P_SID() As String
        Get
            Return sSID
        End Get
        Set(value As String)
            sSID = value
        End Set
    End Property

    Private Property P_Firmware() As String
        Get
            Return sFirmware
        End Get
        Set(value As String)
            sFirmware = value
        End Set
    End Property

    Private ReadOnly Property P_FritzBoxVorhanden(ByVal FritzBoxAdresse As String) As Boolean
        Get
            If C_DP.P_CBForceFBAddr Then
                P_FritzBoxVorhanden = C_hf.Ping(FritzBoxAdresse)
            Else
                C_hf.httpGET("http://" & FritzBoxAdresse, C_hf.GetEncoding(C_DP.P_EncodeingFritzBox), FBFehler)
                Return Not FBFehler
            End If
        End Get
    End Property
#End Region

    Private Structure FritzBoxFirmware
        ''' <summary>
        ''' Erster Teil der Fritz!OS Version. Kann dreistellig sein
        ''' </summary>
        Friend str1 As String

        ''' <summary>
        ''' Zweiter Teil der Fritz!OS Version. Ist zweistellig
        ''' </summary>
        Friend str2 As String

        ''' <summary>
        ''' Dritter Teil der Fritz!OS Version. Ist zweistellig
        ''' </summary>
        Friend str3 As String

        ''' <summary>
        ''' Revision Fritz!OS Version. Ist fünfstelligstellig
        ''' </summary>
        Friend Revision As String

        ''' <summary>
        ''' Setzt die internen Variablen
        ''' </summary>
        ''' <param name="FirmwareMinusRevision">Die Firmware in der Form XX.YY.ZZ-Revision</param>
        ''' <remarks></remarks>
        Friend Sub SetFirmware(ByVal FirmwareMinusRevision As String)
            Dim tmp() As String

            tmp = Split(FirmwareMinusRevision, "-", , CompareMethod.Text)

            If tmp.Count = 2 Then Revision = tmp(1)

            tmp = Split(tmp(0), ".", , CompareMethod.Text)
            If tmp.Count = 3 Then
                str1 = Format(CInt(tmp(tmp.Count - 3)), "000")
            End If
            str2 = Format(CInt(tmp(tmp.Count - 2)), "00")
            str3 = Format(CInt(tmp(tmp.Count - 1)), "00")
        End Sub

        Friend Function ISLargerOREqual(ByVal FirmwareToCheck As String) As Boolean

            Dim tmpFW As New FritzBoxFirmware
            tmpFW.SetFirmware(FirmwareToCheck)
            ISLargerOREqual = (str2 > tmpFW.str2)

            If Not ISLargerOREqual Then
                ISLargerOREqual = (str2 = tmpFW.str2) And str3 >= tmpFW.str3
            End If
        End Function

        Friend Function ISEmpty() As Boolean
            Return str2 = DataProvider.P_Def_LeerString Or str3 = DataProvider.P_Def_LeerString
        End Function

    End Structure

#Region "Properties Fritz!Box Links"
    ' Diese Properties sind hier angeordnet, da sie mit der SessionID gefüttet werden.
    ' Die Session ID soll außerhalb dieser Klasse nicht verfügbar sein.
    ''' <summary>
    ''' http://P_ValidFBAdr
    ''' </summary>
    Private ReadOnly Property P_Link_FB_Basis() As String
        Get
            Return "http://" & C_DP.P_ValidFBAdr
        End Get
    End Property

    ''' <summary>
    ''' http://P_ValidFBAdr/login_sid.lua?
    ''' </summary>
    Private ReadOnly Property P_Link_FB_LoginLua_Basis() As String
        Get
            Return P_Link_FB_Basis & "/login_sid.lua?"
        End Get
    End Property

    ''' <summary>
    ''' Link für den ersten Schritt des neuen SessionIDverfahrens:
    ''' http://P_ValidFBAdr/login_sid.lua?sid=SID
    ''' </summary>
    ''' <param name="sSID">SessionID</param>
    Private ReadOnly Property P_Link_FB_LoginLuaTeil1(ByVal sSID As String) As String
        Get
            Return P_Link_FB_LoginLua_Basis & "sid=" & sSID
        End Get
    End Property

    ''' <summary>
    ''' Link für den zweiten Schritt des neuen SessionIDverfahrens:
    ''' http://P_ValidFBAdr/login_sid.lua?username=" &amp; FBBenutzer &amp; "&amp;response=" &amp; SIDResponse
    ''' </summary>
    ''' <param name="FBBenutzer">Hinterlegter Firtz!Box Benutzer</param>
    ''' <param name="SIDResponse">Erstelltes Response</param>
    Private ReadOnly Property P_Link_FB_LoginLuaTeil2(ByVal FBBenutzer As String, ByVal SIDResponse As String) As String
        Get
            Return P_Link_FB_LoginLua_Basis & "username=" & FBBenutzer & "&response=" & SIDResponse
        End Get
    End Property

    'Login Alte Boxen
    ''' <summary>
    ''' http://P_ValidFBAdr/cgi-bin/webcm
    ''' </summary>
    Private ReadOnly Property P_Link_FB_ExtBasis() As String
        Get
            Return P_Link_FB_Basis & "/cgi-bin/webcm"
        End Get
    End Property

    ''' <summary>
    ''' Link für das alte SessionID verfahren:
    ''' http://fritz.box/cgi-bin/webcm?getpage=../html/login_sid.xml&amp;sid=SID
    ''' </summary>
    ''' <param name="sSID">SessionID</param>
    Private ReadOnly Property P_Link_FB_LoginAltTeil1(ByVal sSID As String) As String
        Get
            Return P_Link_FB_ExtBasis & "?getpage=../html/login_sid.xml&sid=" & sSID
        End Get
    End Property

    ''' <summary>
    ''' getpage=../html/login_sid.xml&amp;login:command/response=" &amp; SIDResponse
    ''' Wird per POST geschickt. Kein "?"
    ''' </summary>
    ''' <param name="SIDResponse"></param>
    Private ReadOnly Property P_Link_FB_LoginAltTeil2(ByVal SIDResponse As String) As String
        Get
            Return "getpage=../html/login_sid.xml&login:command/response=" & SIDResponse
        End Get
    End Property

    ' Logout
    ''' <summary>
    ''' "http://" &amp; C_DP.P_ValidFBAdr &amp; "/home/home.lua?sid=" &amp; sSID &amp; "&amp;logout=1"
    ''' </summary>
    ''' <param name="sSID">SessionID</param>
    Private ReadOnly Property P_Link_FB_LogoutLuaNeu(ByVal sSID As String) As String
        Get
            Return P_Link_FB_Basis & "/home/home.lua?sid=" & sSID & "&logout=1"
        End Get
    End Property

    ''' <summary>
    ''' http:// &amp; P_ValidFBAdr &amp; "/logout.lua?sid=" &amp; sSID
    ''' </summary>
    ''' <param name="sSID">SessionID</param>
    Private ReadOnly Property P_Link_FB_LogoutLuaAlt(ByVal sSID As String) As String
        Get
            Return P_Link_FB_Basis & "/logout.lua?sid=" & sSID
        End Get
    End Property

    'Telefone
    ''' <summary>
    ''' "http://" &amp; C_DP.P_ValidFBAdr &amp; "/fon_num/fon_num_list.lua?sid=" &amp; sSID
    ''' </summary>
    ''' <param name="sSID">SessionID</param>
    Private ReadOnly Property P_Link_FB_Tel1(ByVal sSID As String) As String
        Get
            Return P_Link_FB_Basis & "/fon_num/fon_num_list.lua?sid=" & sSID
        End Get
    End Property

    ''' <summary>
    ''' http:// &amp; C_DP.P_ValidFBAdr &amp; /cgi-bin/webcm?sid= &amp; sSID &amp; &amp;getpage=../html/de/menus/menu2.html&amp;var:lang=de&amp;var:menu=fon&amp;var:pagename=fondevices
    ''' </summary>
    ''' <param name="sSID"></param>
    Private ReadOnly Property P_Link_FB_TelAlt1(ByVal sSID As String) As String
        Get
            Return P_Link_FB_ExtBasis & "?sid=" & sSID & "&getpage=../html/de/menus/menu2.html&var:lang=de&var:menu=fon&var:pagename=fondevices"
        End Get
    End Property

    ' Wählen
    ''' <summary>
    ''' "sid=" &amp; sSID &amp; "&amp;getpage=&amp;telcfg:settings/UseClickToDial=1&amp;telcfg:settings/DialPort=" &amp; DialPort &amp; "&amp;telcfg:command/" &amp; C_hf.IIf(HangUp, "Hangup", "Dial=" &amp; DialCode))
    ''' Wird per POST geschickt. Kein "?"
    ''' </summary>
    ''' <param name="sSID">SessionID</param>
    ''' <param name="DialPort">DialPort</param>
    ''' <param name="DialCode">Gewählte Telefonnummer</param>
    ''' <param name="HangUp">Boolean, ob Abruch erfolgen soll.</param>
    Private ReadOnly Property P_Link_FB_DialV1(ByVal sSID As String, ByVal DialPort As String, ByVal DialCode As String, ByVal HangUp As Boolean) As String
        Get
            Return "sid=" & sSID & "&getpage=&telcfg:settings/UseClickToDial=1&telcfg:settings/DialPort=" & DialPort & "&telcfg:command/" & C_hf.IIf(HangUp, "Hangup", "Dial=" & DialCode)
        End Get
    End Property

    ''' <summary>
    ''' "http://" &amp; C_DP.P_ValidFBAdr &amp; "/fon_num/dial_fonbook.lua"
    ''' </summary>
    Private ReadOnly Property P_Link_FB_TelV2() As String
        Get
            Return P_Link_FB_Basis & "/fon_num/dial_fonbook.lua"
        End Get
    End Property

    ''' <summary>Http POST Data:
    '''  sid=sSID&amp;clicktodial=on&amp;port=DialPort&amp;btn_apply=
    '''  </summary>
    ''' <param name="sSID">SessionID</param>
    ''' <param name="DialPort">Der Dialport, auf den die Wählhilfe geändert werden soll.</param>
    Private ReadOnly Property P_Link_FB_DialV2SetDialPort(ByVal sSID As String, ByVal DialPort As String) As String
        Get
            Return "sid=" & sSID & "&clicktodial=on&port=" & DialPort & "&btn_apply="
        End Get
    End Property

    ''' <summary>
    ''' "http://" &amp; C_DP.P_ValidFBAdr &amp; "/fon_num/fonbook_list.lua?sid=" &amp; sSID &amp; hangup=||dial=DialCode
    ''' </summary>
    ''' <param name="sSID">SessionID</param>
    ''' <param name="DialCode">Gewählte Telefonnummer</param>
    ''' <param name="HangUp">Boolean, ob Abruch erfolgen soll.</param>
    Private ReadOnly Property P_Link_FB_DialV2(ByVal sSID As String, ByVal DialCode As String, ByVal HangUp As Boolean) As String
        Get
            Return P_Link_FB_Basis & "/fon_num/fonbook_list.lua" & "?sid=" & sSID & "" & C_hf.IIf(HangUp, "&hangup=", "&dial=" & DialCode)
        End Get
    End Property

    ' Journalimport
    ''' <summary>
    ''' http://P_ValidFBAdr/fon_num/foncalls_list.lua?sid=sSID
    ''' </summary>
    ''' <param name="sSID"></param>
    Private ReadOnly Property P_Link_JI1(ByVal sSID As String) As String
        Get
            Return P_Link_FB_Basis & "/fon_num/foncalls_list.lua?sid=" & sSID
        End Get
    End Property

    ''' <summary>
    ''' http://P_ValidFBAdr/fon_num/foncalls_list.lua?sid=sSID&amp;csv=
    ''' </summary>
    ''' <param name="sSID"></param>
    Private ReadOnly Property P_Link_JI2(ByVal sSID As String) As String
        Get
            Return P_Link_JI1(sSID) & "&csv="
        End Get
    End Property

    ''' <summary>
    ''' http://P_ValidFBAdr/cgi-bin/webcm?sid=sSID&amp;getpage=../html/de/
    ''' </summary>
    ''' <param name="sSID"></param>
    Private ReadOnly Property P_Link_JIAlt_Basis(ByVal sSID As String) As String
        Get
            Return P_Link_FB_Basis & "/cgi-bin/webcm?sid=" & sSID & "&getpage=../html/de/"
        End Get
    End Property

    ''' <summary>
    ''' http://P_ValidFBAdr/cgi-bin/webcm?sid=sSID&amp;getpage=../html/de/menus/menu2.html&amp;var:lang=de&amp;var:menu=fon&amp;var:pagename=foncalls
    ''' </summary>
    ''' <param name="sSID"></param>
    Private ReadOnly Property P_Link_JIAlt_Child1(ByVal sSID As String) As String
        Get
            Return P_Link_JIAlt_Basis(sSID) & "menus/menu2.html&var:lang=de&var:menu=fon&var:pagename=foncalls"
        End Get
    End Property

    ''' <summary>                                                        
    ''' http://P_ValidFBAdr/cgi-bin/webcm?sid=sSID&amp;getpage=../html/de/FRITZ!Box_Anrufliste.csv
    ''' </summary>
    ''' <param name="sSID"></param>
    Private ReadOnly Property P_Link_JIAlt_Child2(ByVal sSID As String) As String
        Get
            Return P_Link_JIAlt_Basis(sSID) & DataProvider.P_Def_AnrListFileName
        End Get
    End Property

    ''' <summary>
    ''' http://P_ValidFBAdr/jason_boxinfo.xml
    ''' </summary>
    Private ReadOnly Property P_Link_FB_jason_boxinfo() As String
        Get
            Return P_Link_FB_Basis & "/jason_boxinfo.xml"
        End Get
    End Property

    ' Telefonbuch
    ''' <summary>
    ''' http://P_ValidFBAdr/fon_num/fonbook_entry.lua
    ''' </summary>
    Private ReadOnly Property P_Link_FB_FonBook_Entry() As String
        Get
            Return P_Link_FB_Basis & "/fon_num/fonbook_entry.lua"
        End Get
    End Property


    ''' <summary>
    ''' http://P_ValidFBAdr/cgi-bin/firmwarecfg
    ''' </summary>
    Private ReadOnly Property P_Link_FB_ExportAddressbook() As String
        Get
            Return P_Link_FB_Basis & "/cgi-bin/firmwarecfg"
        End Get
    End Property


    ''' <summary>
    ''' http://<c>P_ValidFBAdr</c>/fon_num/fonbook_select.lua?sid=<c>sid</c>
    ''' </summary>
    ''' <param name="sSID">Session ID</param>
    Private ReadOnly Property P_Link_Telefonbuch_List(ByVal sSID As String) As String
        Get
            Return P_Link_FB_Basis & "/fon_num/fonbook_select.lua?sid=" & sSID
        End Get
    End Property

    ' Query
    ''' <summary>
    ''' http://<c>P_ValidFBAdr</c>/query.lua/?sid=?sid=<c>sid</c>&amp;<c>Abfrage</c>
    ''' </summary>
    ''' <param name="sSID">Session ID</param>
    ''' <param name="Abfrage">Zu übersendende Abfrage</param>
    Private ReadOnly Property P_Link_Query(ByVal sSID As String, ByVal Abfrage As String) As String
        Get
            Return P_Link_FB_Basis & "/query.lua?sid=" & sSID & "&" & Abfrage
        End Get
    End Property

    ' Fritz!Box Info
    ''' <summary>
    ''' http://P_ValidFBAdr/jason_boxinfo.xml
    ''' </summary>
    Private ReadOnly Property P_Link_Jason_Boxinfo() As String
        Get
            Return P_Link_FB_Basis & "/jason_boxinfo.xml"
        End Get
    End Property

    ''' <summary>
    ''' http://P_ValidFBAdr/cgi-bin/system_status
    ''' </summary>
    Private ReadOnly Property P_Link_FB_SystemStatus() As String
        Get
            Return P_Link_FB_Basis & "/cgi-bin/system_status"
        End Get
    End Property

#End Region

#Region "Fritz!Box Querys"

    ''' <summary>
    ''' "S0" &amp; <c>idx</c> &amp; Type &amp; "=telcfg:settings/NTHotDialList/" &amp; <c>Type</c> &amp; <c>idx</c>
    ''' </summary>
    ''' <param name="Type">Der Typ des Eintrages: Name oder Number</param>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    ''' <remarks>
    ''' "S0" &amp; i &amp; "Name=telcfg:settings/NTHotDialList/Name" &amp; i
    ''' "S0" &amp; i &amp; "Number=telcfg:settings/NTHotDialList/Number" &amp; i
    ''' </remarks>
    Private ReadOnly Property P_Query_FB_S0(ByVal Type As String, ByVal idx As Integer) As String
        Get
            Return "S0" & idx & Type & "=telcfg:settings/NTHotDialList/" & Type & idx
        End Get
    End Property

    ''' <summary>
    ''' "POTS=telcfg:settings/MSN/POTS"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_POTS() As String
        Get
            Return "POTS=telcfg:settings/MSN/POTS"
        End Get
    End Property

    ''' <summary>
    ''' "Mobile=telcfg:settings/Mobile/MSN"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_Mobile() As String
        Get
            Return "Mobile=telcfg:settings/Mobile/MSN"
        End Get
    End Property

    ''' <summary>
    ''' "Port" &amp; <c>idx</c> &amp; "Name=telcfg:settings/MSN/Port" &amp; <c>idx</c> &amp; "/Name"
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    ''' <remarks>
    ''' "S0" &amp; i &amp; "Name=telcfg:settings/NTHotDialList/Name" &amp; i
    ''' "S0" &amp; i &amp; "Number=telcfg:settings/NTHotDialList/Number" &amp; i
    ''' </remarks>
    Private ReadOnly Property P_Query_FB_FON(ByVal idx As Integer) As String
        Get
            Return "Port" & idx & "Name=telcfg:settings/MSN/Port" & idx & "/Name"
        End Get
    End Property

    ''' <summary>
    ''' "TAM" &amp; <c>idx</c> &amp; "=tam:settings/MSN" &amp; <c>idx</c>
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_TAM(ByVal idx As Integer) As String
        Get
            Return "TAM" & idx & "=tam:settings/MSN" & idx
        End Get
    End Property

    ''' <summary>
    ''' "FAX" &amp; <c>idx</c> &amp; "=telcfg:settings/FaxMSN" &amp; <c>idx</c>
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_FAX(ByVal idx As Integer) As String
        Get
            Return "FAX" & idx & "=telcfg:settings/FaxMSN" & idx
        End Get
    End Property

    ''' <summary>
    ''' "MSN" &amp; <c>idx</c> &amp; "=telcfg:settings/MSN/MSN" &amp; <c>idx</c>
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_MSN(ByVal idx As Integer) As String
        Get
            Return "MSN" & idx & "=telcfg:settings/MSN/MSN" & idx
        End Get
    End Property

    ''' <summary>
    ''' "VOIP" &amp; <c>idx</c> &amp; "Enabled=" &amp; "telcfg:settings/VoipExtension" &amp; <c>idx</c> &amp; "/enabled"
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_VOIP(ByVal idx As Integer) As String
        Get
            Return "VOIP" & idx & "Enabled=" & "telcfg:settings/VoipExtension" & idx & "/enabled"
        End Get
    End Property

    ''' <summary>
    ''' "SIP" &amp; "=" &amp; "sip:settings/sip/list(activated,displayname,registrar,outboundproxy,providername,ID,gui_readonly,webui_trunk_id)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_SIP() As String
        Get
            Return "SIP" & "=" & "sip:settings/sip/list(activated,displayname,registrar,outboundproxy,providername,ID,gui_readonly,webui_trunk_id)"
        End Get
    End Property

    ''' <summary>
    ''' "MSN" &amp; idx &amp; "Nr" &amp; jdx &amp; "=telcfg:settings/MSN/Port" &amp; idx &amp; "/MSN" &amp; jdx
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_MSN_List(ByVal idx As String, ByVal jdx As Integer) As String
        Get
            Return "MSN" & idx & "Nr" & jdx & "=telcfg:settings/MSN/Port" & idx & "/MSN" & jdx
        End Get
    End Property

    ''' <summary>
    ''' "VOIP" &amp; idx &amp; "Nr" &amp; jdx &amp; "=telcfg:settings/VoipExtension" &amp; idx &amp; "/Number" &amp; jdx
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_VOIP_List(ByVal idx As String, ByVal jdx As Integer) As String
        Get
            Return "VOIP" & idx & "Nr" & jdx & "=telcfg:settings/VoipExtension" & idx & "/Number" & jdx
        End Get
    End Property

    ''' <summary>
    ''' "FON=telcfg:settings/MSN/Port/list(Name,Fax,GroupCall,AllIncomingCalls,OutDialing)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_FON_List() As String
        Get
            Return "FON=telcfg:settings/MSN/Port/list(Name,Fax,GroupCall,AllIncomingCalls,OutDialing)"
        End Get
    End Property

    ''' <summary>
    ''' "DECT=telcfg:settings/Foncontrol/User/list(Name,Type,Intern,Id)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_DECT_List() As String
        Get
            Return "DECT=telcfg:settings/Foncontrol/User/list(Name,Type,Intern,Id)"
        End Get
    End Property

    ''' <summary>
    ''' "VOIP=telcfg:settings/VoipExtension/list(enabled,Name,RingOnAllMSNs)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_VOIP_List() As String
        Get
            Return "VOIP=telcfg:settings/VoipExtension/list(enabled,Name,RingOnAllMSNs)"
        End Get
    End Property

    ''' <summary>
    ''' "TAM=tam:settings/TAM/list(Active,Name,Display,MSNBitmap)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_TAM_List() As String
        Get
            Return "TAM=tam:settings/TAM/list(Active,Name,Display,MSNBitmap)"
        End Get
    End Property

    ''' <summary>
    ''' "S0Name" &amp; idx &amp; "=telcfg:settings/NTHotDialList/Name" &amp; idx
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_S0_List(ByVal idx As Integer) As String
        Get
            Return "S0Name" & idx & "=telcfg:settings/NTHotDialList/Name" & idx
        End Get
    End Property

    ''' <summary>
    ''' "S0TelNr" &amp; idx &amp; "=telcfg:settings/NTHotDialList/Number" &amp; idx
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_S0_TelNr(ByVal idx As String) As String
        Get
            Return "S0TelNr" & idx & "=telcfg:settings/NTHotDialList/Number" & idx
        End Get
    End Property

    ''' <summary>
    ''' "S0Type" &amp; idx &amp; "=telcfg:settings/NTHotDialList/Type" &amp; idx
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_S0_Type(ByVal idx As String) As String
        Get
            Return "S0Type" & idx & "=telcfg:settings/NTHotDialList/Type" & idx
        End Get
    End Property

    ''' <summary>
    ''' "DECT" &amp; idx &amp; "RingOnAllMSNs=telcfg:settings/Foncontrol/User" &amp; idx &amp; "/RingOnAllMSNs"
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_DECT_RingOnAllMSNs(ByVal idx As Integer) As String
        Get
            Return "DECT" & idx & "RingOnAllMSNs=telcfg:settings/Foncontrol/User" & idx & "/RingOnAllMSNs"
        End Get
    End Property

    ''' <summary>
    ''' "DECT" &amp; idx &amp; "Nr=telcfg:settings/Foncontrol/User" &amp; idx &amp; "/MSN/list(Number)"
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_DECT_NrList(ByVal idx As Integer) As String
        Get
            Return "DECT" & idx & "Nr=telcfg:settings/Foncontrol/User" & idx & "/MSN/list(Number)"
        End Get
    End Property

    ''' <summary>
    ''' "FaxMailActive=telcfg:settings/FaxMailActive"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_FaxMailActive() As String
        Get
            Return "FaxMailActive=telcfg:settings/FaxMailActive"
        End Get
    End Property

    ''' <summary>
    ''' "MobileName=telcfg:settings/Mobile/Name"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_MobileName() As String
        Get
            Return "MobileName=telcfg:settings/Mobile/Name"
        End Get
    End Property

#End Region

    Public Sub New(ByVal DataProviderKlasse As DataProvider, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal CryptKlasse As Rijndael, _
                   ByVal XMLKlasse As XML, _
                   ByVal UPnpKlasse As FritzBoxServices)


        C_DP = DataProviderKlasse
        C_hf = HelferKlasse
        C_Crypt = CryptKlasse
        C_XML = XMLKlasse
        C_FBoxUPnP = UPnpKlasse

        P_SID = DataProvider.P_Def_SessionID  ' Startwert: Ungültige SID

        C_DP.P_ValidFBAdr = C_hf.ValidIP(C_DP.P_TBFBAdr)

        If P_FritzBoxVorhanden(C_DP.P_ValidFBAdr) Then

            C_FBoxUPnP.SetFritzBoxData(C_DP.P_ValidFBAdr, C_DP.P_TBBenutzer, C_Crypt.DecryptString128Bit(C_DP.P_TBPasswort, C_DP.GetSettingsVBA("Zugang", DataProvider.P_Def_ErrorMinusOne_String)))
            FBFirmware()

            If C_DP.P_EncodeingFritzBox = DataProvider.P_Def_ErrorMinusOne_String Then
                Dim Rückgabe As String
                Rückgabe = C_hf.httpGET(P_Link_FB_Basis, FBEncoding, FBFehler)
                If Not FBFehler Then
                    FBEncoding = C_hf.GetEncoding(C_hf.StringEntnehmen(Rückgabe, "charset=", """>"))
                    C_DP.P_EncodeingFritzBox = FBEncoding.HeaderName
                    C_DP.SpeichereXMLDatei()
                Else
                    C_hf.LogFile("FBError (FritzBox.New): " & Err.Number & " - " & Err.Description & " - " & P_Link_FB_Basis)
                End If
            Else
                FBEncoding = C_hf.GetEncoding(C_DP.P_EncodeingFritzBox)
            End If
        Else
            C_hf.LogFile("FBError (FritzBox.New): Keine Fritz!Box an der Gegenstelle " & C_DP.P_ValidFBAdr)
        End If
    End Sub

#Region "Login & Logout"
    Public Function FBLogin(Optional ByVal InpupBenutzer As String = "", Optional ByVal InpupPasswort As String = "-1") As String
        Dim slogin_xml As String

        ' Mögliche Login-XML:

        ' Alter Login von Firmware xxx.04.76 bis Firmware xxx.05.28
        ' <?xml version="1.0" encoding="utf-8"?>
        ' <SessionInfo>
        '    <iswriteaccess>0</iswriteaccess>
        '    <SID>0000000000000000</SID>
        '    <Challenge>dbef619d</Challenge>
        ' </SessionInfo>

        ' Lua Login ab Firmware xxx.05.29 / xxx.05.5x
        ' <?xml version="1.0" encoding="utf-8"?>
        ' <SessionInfo>
        '    <SID>0000000000000000</SID>
        '    <Challenge>11def856</Challenge>
        '    <BlockTime>0</BlockTime>
        '    <Rights></Rights>
        ' </SessionInfo>

        slogin_xml = C_hf.httpGET(P_Link_FB_LoginLuaTeil1(P_SID), FBEncoding, FBFehler)

        If InStr(slogin_xml, "BlockTime", CompareMethod.Text) = 0 Then
            slogin_xml = C_hf.httpGET(P_Link_FB_LoginAltTeil1(P_SID), FBEncoding, FBFehler)
        End If

        If Not FBFehler Then
            If InStr(slogin_xml, "FRITZ!Box Anmeldung", CompareMethod.Text) = 0 And Not Len(slogin_xml) = 0 Then

                If Not InpupPasswort = DataProvider.P_Def_ErrorMinusOne_String Then
                    C_DP.P_TBPasswort = C_Crypt.EncryptString128Bit(InpupPasswort, DataProvider.P_Def_PassWordDecryptionKey)
                    C_DP.P_TBBenutzer = InpupBenutzer
                    C_DP.SaveSettingsVBA("Zugang", DataProvider.P_Def_PassWordDecryptionKey)
                    C_hf.KeyChange()
                End If

                Dim sBlockTime As String
                Dim sChallenge As String
                Dim sFBBenutzer As String = C_DP.P_TBBenutzer
                Dim sFBPasswort As String = C_DP.P_TBPasswort
                Dim sResponse As String
                Dim sSIDResponse As String
                Dim sZugang As String = C_DP.GetSettingsVBA("Zugang", DataProvider.P_Def_ErrorMinusOne_String)
                Dim XMLDocLogin As New XmlDocument()

                ' Login nur durchführen, wenn überhaupt ein versclüsseltes Passwort und(!) ein Zugangsschlüssel vorhanden ist. Ansonsten bringt das ja nicht viel.
                If sFBPasswort IsNot DataProvider.P_Def_ErrorMinusOne_String And sZugang IsNot DataProvider.P_Def_ErrorMinusOne_String Then
                    With XMLDocLogin
                        .LoadXml(slogin_xml)

                        If .Item("SessionInfo").Item("SID").InnerText() = DataProvider.P_Def_SessionID Then
                            sChallenge = .Item("SessionInfo").Item("Challenge").InnerText()

                            With C_Crypt
                                sSIDResponse = String.Concat(sChallenge, "-", .getMd5Hash(String.Concat(sChallenge, "-", .DecryptString128Bit(sFBPasswort, sZugang)), Encoding.Unicode, True))
                            End With
                            If P_SpeichereDaten Then PushStatus("Challenge: " & sChallenge & vbNewLine & "SIDResponse: " & sSIDResponse)

                            If ThisFBFirmware.ISEmpty Then FBFirmware()

                            If ThisFBFirmware.ISLargerOREqual("5.29") Then
                                'If .InnerXml.Contains("Rights") Then
                                ' Lua Login ab Firmware xxx.05.29 / xxx.05.5x
                                sBlockTime = .Item("SessionInfo").Item("BlockTime").InnerText()
                                If sBlockTime = DataProvider.P_Def_StringNull Then ' "0"
                                    'sLink = "http://" & C_DP.P_ValidFBAdr & "/login_sid.lua?username=" & sFBBenutzer & "&response=" & sSIDResponse

                                    sResponse = C_hf.httpGET(P_Link_FB_LoginLuaTeil2(sFBBenutzer, sSIDResponse), FBEncoding, FBFehler)
                                    If FBFehler Then
                                        C_hf.LogFile("FBError (FBLogin): " & Err.Number & " - " & Err.Description)
                                    End If
                                Else
                                    C_hf.MsgBox(DataProvider.P_FritzBox_LoginError_Blocktime(sBlockTime), MsgBoxStyle.Critical, "FBLogin")
                                    Return DataProvider.P_Def_SessionID
                                End If
                            Else
                                ' Alter Login von Firmware xxx.04.76 bis Firmware xxx.05.28
                                If CBool(.Item("SessionInfo").Item("iswriteaccess").InnerText) Then
                                    C_hf.LogFile(DataProvider.P_FritzBox_LoginError_MissingPassword)
                                    Return .Item("SessionInfo").Item("SID").InnerText()
                                End If

                                'sLink = C_DP.P_Link_FB_Alt_Basis '"http://" & C_DP.P_ValidFBAdr & "/cgi-bin/webcm"
                                'sFormData = C_DP.P_Link_FB_LoginAltTeil2(sSIDResponse) ' "getpage=../html/login_sid.xml&login:command/response=" + sSIDResponse
                                sResponse = C_hf.httpPOST(P_Link_FB_ExtBasis, P_Link_FB_LoginAltTeil2(sSIDResponse), FBEncoding)
                            End If

                            .LoadXml(sResponse)

                            '<SessionInfo>
                            '   <SID>ff88e4d39354992f</SID>
                            '   <Challenge>ab7190d6</Challenge>
                            '   <BlockTime>128</BlockTime>
                            '   <Rights>
                            '       <Name>BoxAdmin</Name>
                            '       <Access>2</Access>
                            '       <Name>Phone</Name>
                            '       </Access>2</Access>
                            '       <Name>NAS></Name>
                            '       <Access>2</Access>
                            '   </Rights>
                            '</SessionInfo>

                            P_SID = .Item("SessionInfo").Item("SID").InnerText()

                            If Not P_SID = DataProvider.P_Def_SessionID Then
                                If ThisFBFirmware.ISLargerOREqual("5.29") Then
                                    If Not C_hf.IsOneOf("BoxAdmin", Split(.SelectSingleNode("//Rights").InnerText, "2")) Then
                                        C_hf.LogFile(DataProvider.P_FritzBox_LoginError_MissingRights(sFBBenutzer))
                                        FBLogout(P_SID)
                                        P_SID = DataProvider.P_Def_SessionID
                                    End If
                                End If
                            Else
                                C_hf.LogFile(DataProvider.P_FritzBox_LoginError_LoginIncorrect)
                            End If

                        ElseIf .Item("SessionInfo").Item("SID").InnerText() = P_SID Then
                            C_hf.LogFile(DataProvider.P_FritzBox_LoginInfo_SID(P_SID))
                        End If
                    End With
                    XMLDocLogin = Nothing
                End If
            Else

            End If
        Else
            C_hf.LogFile(DataProvider.P_FritzBox_LoginError_MissingData)
        End If

        Return P_SID
    End Function

    Public Function FBLogout(ByRef sSID As String) As Boolean
        ' Die Komplementärfunktion zu FBLogin. Beendet die Session, indem ein Logout durchgeführt wird.

        Dim Response As String
        Dim tmpstr As String
        Dim xml As New XmlDocument()

        'sLink = "http://" & C_DP.P_ValidFBAdr & "/login_sid.lua?sid=" & sSID
        Response = C_hf.httpGET(P_Link_FB_LoginLuaTeil1(sSID), FBEncoding, FBFehler)
        If Not FBFehler Then
            With xml
                .LoadXml(Response)
                'If .InnerXml.Contains("Rights") Then
                '    sLink = C_DP.P_Link_FB_LogoutLuaNeu(sSID) '"http://" & C_DP.P_ValidFBAdr & "/home/home.lua?sid=" & sSID & "&logout=1"
                'Else
                '    sLink = C_DP.P_Link_FB_LogoutLuaAlt(sSID) '"http://" & C_DP.P_ValidFBAdr & "/logout.lua?sid=" & sSID
                'End If

                'IIf(.InnerXml.Contains("Rights"), C_DP.P_Link_FB_LogoutLuaNeu(sSID), C_DP.P_Link_FB_LogoutLuaAlt(sSID))
                Response = C_hf.httpGET(C_hf.IIf(.InnerXml.Contains("Rights"), _
                                                 P_Link_FB_LogoutLuaNeu(sSID), _
                                                 P_Link_FB_LogoutLuaAlt(sSID)), FBEncoding, FBFehler)
            End With
            xml = Nothing
            C_hf.KeyChange()
            If Not FBFehler Then
                If Not InStr(Response, DataProvider.P_FritzBox_LogoutTestString1, CompareMethod.Text) = 0 Or _
                    Not InStr(Response, DataProvider.P_FritzBox_LogoutTestString2, CompareMethod.Text) = 0 Then
                    ' C_hf.LogFile("Logout erfolgreich")
                    sSID = DataProvider.P_Def_SessionID
                    Return True
                Else
                    Response = Replace(C_hf.StringEntnehmen(Response, "<pre>", "</pre>"), Chr(34), "'", , , CompareMethod.Text)
                    If Not Response = DataProvider.P_Def_ErrorMinusOne_String Then
                        tmpstr = C_hf.StringEntnehmen(Response, "['logout'] = '", "'")
                        If Not tmpstr = "1" Then C_hf.LogFile(DataProvider.P_FritzBox_LogoutError)
                    End If
                    sSID = DataProvider.P_Def_SessionID
                    Return False
                End If
            Else
                C_hf.LogFile("FBError (FBLogout): " & Err.Number & " - " & Err.Description)
            End If
        Else
            C_hf.LogFile("FBError (FBLogout): Logout übersprungen")
        End If
        Return False
    End Function
#End Region

#Region "Telefonnummern, Telefonnamen"
    Friend Sub FritzBoxDatenDebug(ByVal sLink As String)
        Dim tempstring As String
        Dim tempstring_code As String

        tempstring = C_hf.httpGET(sLink, FBEncoding, FBFehler)
        tempstring = Replace(tempstring, Chr(34), "'", , , CompareMethod.Text)   ' " in ' umwandeln 
        tempstring = Replace(tempstring, Chr(13), "", , , CompareMethod.Text)

        If InStr(tempstring, "Luacgi not readable") = 0 Then
            tempstring_code = C_hf.StringEntnehmen(tempstring, "<code>", "</code>")

            If Not tempstring_code = DataProvider.P_Def_ErrorMinusOne_String Then
                tempstring = tempstring_code
            Else
                tempstring = C_hf.StringEntnehmen(tempstring, "<pre>", "</pre>")
            End If
            If Not tempstring = DataProvider.P_Def_ErrorMinusOne_String Then
                FritzBoxDatenV2(tempstring)
                FBLogout(P_SID)
            Else
                FritzBoxDatenV1(sLink)
            End If
        Else
            FritzBoxDatenV1()
        End If
    End Sub

    Friend Sub FritzBoxDaten()
        'Dim sLink As String
        Dim tempstring As String
        Dim tempstring_code As String

        If P_SpeichereDaten Then PushStatus(DataProvider.P_Def_FritzBoxName & " Adresse: " & C_DP.P_TBFBAdr)

        FBLogin()
        If Not P_SID = DataProvider.P_Def_SessionID Then

            If ThisFBFirmware.ISLargerOREqual("6.05") Then
                PushStatus("Starte AuswertungV3")
                FritzBoxDatenV3()
            ElseIf ThisFBFirmware.ISLargerOREqual("5.25") Then
                tempstring = C_hf.httpGET(P_Link_FB_Tel1(P_SID), FBEncoding, FBFehler)
                If Not FBFehler Then
                    tempstring = Replace(tempstring, Chr(34), "'", , , CompareMethod.Text)   ' " in ' umwandeln 
                    tempstring = Replace(tempstring, Chr(13), "", , , CompareMethod.Text)

                    If InStr(tempstring, "Luacgi not readable") = 0 Then
                        tempstring_code = C_hf.StringEntnehmen(tempstring, "<code>", "</code>")

                        If Not tempstring_code = DataProvider.P_Def_ErrorMinusOne_String Then
                            tempstring = tempstring_code
                        Else
                            tempstring = C_hf.StringEntnehmen(tempstring, "<pre>", "</pre>")
                        End If
                        If Not tempstring = DataProvider.P_Def_ErrorMinusOne_String Then
                            PushStatus("Starte AuswertungV2")
                            FritzBoxDatenV2(tempstring)
                            FBLogout(P_SID)
                        Else
                            C_hf.MsgBox(DataProvider.P_FritzBox_Tel_Error2, MsgBoxStyle.Critical, "FritzBoxDaten #3")
                        End If
                    Else
                        C_hf.LogFile("FBError (FritzBoxDaten): " & Err.Number & " - " & Err.Description)
                    End If
                End If
            Else
                PushStatus("Starte AuswertungV1")
                FritzBoxDatenV1()
            End If
        Else
            C_hf.MsgBox(DataProvider.P_FritzBox_Tel_Error1, MsgBoxStyle.Critical, "FritzBoxDaten #2")
        End If

    End Sub

    Private Sub FritzBoxDatenV1(Optional ByVal Link As String = "-1")
        PushStatus(DataProvider.P_FritzBox_Tel_AlteRoutine)

        'Dim Vorwahl As String = C_DP.P_TBVorwahl  ' In den Einstellungen eingegebene Vorwahl
        Dim TelName As String                 ' Gefundener Telefonname
        Dim TelNr As String                 ' Dazugehörige Telefonnummer
        Dim SIPID As String = DataProvider.P_Def_ErrorMinusOne_String
        Dim pos(6) As Integer                   ' Positionsmarker
        Dim posSTR As Integer = 1
        Dim Anzahl As Integer = 0
        Dim AnzahlISDN As Integer = 0
        Dim ID As Integer
        Dim Section As String
        Dim TelefonString() As String
        Dim j As Integer = 0
        Dim SIP(20) As String
        Dim TAM(10) As String
        Dim MSN(10) As String
        Dim DialPort As String
        Dim POTS As String
        Dim Mobil As String
        Dim AllIn As String
        Dim tempstring As String

        Dim sLink As String

        Dim xPathTeile As New ArrayList
        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList

        Dim PortName() As String = {"readFon123", _
                                    "readNTHotDialList", _
                                    "readDect1", _
                                    "readFonControl", _
                                    "readVoipExt", _
                                    "readTam", _
                                    "readFaxMail"}

        Dim EndPortName() As String = {"return list", _
                                       "return list", _
                                       "return list", _
                                       "return list", _
                                       "return Result", _
                                       "return list", _
                                       "return list"}

        With xPathTeile
            .Clear()
            .Add("Telefone")
            .Add("Nummern")
        End With
        With NodeNames
            .Clear()
            .Add("TelName")
            .Add("TelNr")
        End With
        With AttributeNames
            .Clear()
            .Add("Fax")
            .Add("Dialport")
        End With
        With NodeValues
            .Clear()
            .Add(DataProvider.P_Def_LeerString)
            .Add(DataProvider.P_Def_LeerString)
        End With
        With AttributeValues
            .Clear()
            .Add(DataProvider.P_Def_LeerString)
            .Add(DataProvider.P_Def_LeerString)
        End With

        If Link = DataProvider.P_Def_ErrorMinusOne_String Then
            sLink = P_Link_FB_TelAlt1(P_SID)
        Else
            sLink = Link
        End If

        If P_SpeichereDaten Then PushStatus(DataProvider.P_FritzBox_Tel_AlteRoutine2(sLink))
        tempstring = C_hf.httpGET(sLink, FBEncoding, FBFehler)
        If Not FBFehler Then
            If Not InStr(tempstring, "FRITZ!Box Anmeldung", CompareMethod.Text) = 0 Then
                C_hf.MsgBox(DataProvider.P_FritzBox_Tel_ErrorAlt1, MsgBoxStyle.Critical, "FritzBoxDaten_FWbelow5_50")
                Exit Sub
            End If
            If P_SpeichereDaten Then C_XML.Delete(C_DP.XMLDoc, "Telefone")

            tempstring = Replace(tempstring, Chr(34), "'", , , CompareMethod.Text)   ' " in ' umwandeln

            FBLogout(P_SID)
            xPathTeile.Add("MSN")
            pos(0) = 1
            For i = 0 To 9
                TelNr = C_hf.StringEntnehmen(tempstring, "nrs.msn.push('", "'", posSTR)
                If Not TelNr = DataProvider.P_Def_ErrorMinusOne_String And Not TelNr = DataProvider.P_Def_LeerString Then
                    TelNr = C_hf.EigeneVorwahlenEntfernen(TelNr)
                    MSN(i) = TelNr
                    j = i
                    PushStatus(DataProvider.P_FritzBox_Tel_NrFound("MSN", CStr(i), TelNr))
                    If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, TelNr, "ID", CStr(i))
                End If
            Next
            ReDim Preserve MSN(j)
            posSTR = 1

            'Internetnummern ermitteln
            xPathTeile.Item(xPathTeile.IndexOf("MSN")) = "SIP"
            j = 0
            For i = 0 To 19
                TelNr = C_hf.StringEntnehmen(tempstring, "nrs.sip.push('", "'", posSTR)
                If Not TelNr = DataProvider.P_Def_ErrorMinusOne_String And Not TelNr = DataProvider.P_Def_LeerString Then
                    TelNr = C_hf.EigeneVorwahlenEntfernen(TelNr)
                    SIP(i) = TelNr
                    SIPID = CStr(i)
                    j = i
                    PushStatus(DataProvider.P_FritzBox_Tel_NrFound("SIP", CStr(i), TelNr))
                    If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, TelNr, "ID", SIPID)
                End If
            Next
            ReDim Preserve SIP(j)
            j = 0
            posSTR = 1

            'TAM Nr ermitteln
            xPathTeile.Item(xPathTeile.IndexOf("SIP")) = "TAM"
            For i = 0 To 9
                TelNr = C_hf.StringEntnehmen(tempstring, "nrs.tam.push('", "'", posSTR)
                If Not TelNr = DataProvider.P_Def_ErrorMinusOne_String And Not TelNr = DataProvider.P_Def_LeerString Then
                    TelNr = C_hf.EigeneVorwahlenEntfernen(TelNr)
                    TAM(i) = TelNr
                    PushStatus(DataProvider.P_FritzBox_Tel_NrFound("TAM", CStr(i), TelNr))
                    If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, TelNr, "ID", CStr(i))
                    j = i
                End If
            Next
            ReDim Preserve TAM(j)

            ' Plain old telephone service (POTS)
            xPathTeile.Item(xPathTeile.IndexOf("TAM")) = "POTS"
            POTS = C_hf.StringEntnehmen(tempstring, "telcfg:settings/MSN/POTS' value='", "'")
            If Not POTS = DataProvider.P_Def_ErrorMinusOne_String And Not POTS = DataProvider.P_Def_LeerString Then
                POTS = C_hf.EigeneVorwahlenEntfernen(POTS)
                PushStatus(DataProvider.P_FritzBox_Tel_NrFound("POTS", CStr(0), POTS))
                If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, POTS, "ID", DataProvider.P_Def_StringNull)
            End If

            'Mobilnummer ermitteln
            xPathTeile.Item(xPathTeile.IndexOf("POTS")) = "Mobil"
            Mobil = C_hf.StringEntnehmen(tempstring, "nrs.mobil = '", "'")
            If Not Mobil = DataProvider.P_Def_ErrorMinusOne_String And Not Mobil = DataProvider.P_Def_LeerString Then
                Mobil = C_hf.EigeneVorwahlenEntfernen(Mobil)
                PushStatus(DataProvider.P_FritzBox_Tel_NrFound("Mobil", CStr(0), Mobil))
                If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, Mobil, "ID", DataProvider.P_Def_StringNull)
            End If

            AllIn = AlleNummern(MSN, SIP, TAM, POTS, Mobil)

            'Telefone ermitteln
            pos(0) = 1
            xPathTeile.Item(xPathTeile.IndexOf("Nummern")) = "Telefone"
            xPathTeile.Item(xPathTeile.IndexOf("Mobil")) = "FON"


            For i = 0 To UBound(PortName)
                pos(0) = InStr(pos(0), tempstring, PortName(i), CompareMethod.Text)
                pos(1) = InStr(pos(0), tempstring, EndPortName(i), CompareMethod.Text) + Len(EndPortName(i))
                If pos(1) = Len(EndPortName(i)) Then
                    ' Die JavaFunktion "readVoipExt" für die IPTelefone endet ab der Firmware *80 auf "return Result;". (früher auf "return list;")
                    pos(1) = InStr(pos(0), tempstring, "return list;", CompareMethod.Text) + Len("return list;")
                End If
                Section = Mid(tempstring, pos(0), pos(1) - pos(0))
                TelefonString = Split(Section, "});", , CompareMethod.Text)

                For Each Telefon In TelefonString
                    If InStr(Telefon, "return list") = 0 And InStr(Telefon, "Isdn-Default") = 0 Then
                        pos(0) = InStr(Telefon, "name: ", CompareMethod.Text) + Len("name: ")
                        pos(1) = InStr(pos(0), Telefon, ",", CompareMethod.Text)
                        If Not pos(0) = 6 Or Not pos(1) = 0 Then
                            TelName = Mid(Telefon, pos(0), pos(1) - pos(0))
                            If TelName = "fonName" Then
                                pos(0) = InStr(Telefon, "fonName = '", CompareMethod.Text) + Len("fonName = '")
                                pos(1) = InStr(pos(0), Telefon, "'", CompareMethod.Text)
                                TelName = Mid(Telefon, pos(0), pos(1) - pos(0))
                            Else
                                TelName = Replace(TelName, "'", "", , , CompareMethod.Text)
                            End If
                            pos(2) = InStr(pos(1), Telefon, "number: ", CompareMethod.Text) + Len("number: ")
                            pos(3) = InStr(pos(2), Telefon, Chr(10), CompareMethod.Text)
                            TelNr = Replace(Trim(Mid(Telefon, pos(2), pos(3) - pos(2))), "'", "", , , CompareMethod.Text)
                            TelNr = Replace(TelNr, Chr(10), "", , , CompareMethod.Text)
                            TelNr = Replace(TelNr, Chr(13), "", , , CompareMethod.Text)

                            If TelNr.EndsWith(",") Then TelNr = Left(TelNr, Len(TelNr) - 1) ' Für die Firmware *85
                            If TelNr.EndsWith("#") Then TelNr = Left(TelNr, Len(TelNr) - 1) ' Für die Firmware *85
                            If TelNr.StartsWith("SIP") Then TelNr = SIP(CInt(Mid(TelNr, 4, 1)))
                            If Not Trim(TelName) = DataProvider.P_Def_LeerString And Not Trim(TelNr) = DataProvider.P_Def_LeerString Then
                                Select Case i
                                    Case 0 ' FON 1-3
                                        xPathTeile.Item(xPathTeile.Count - 1) = "FON"
                                        pos(2) = InStr(pos(1), Telefon, "allin: ('", CompareMethod.Text) + Len("allin: ('")
                                        pos(3) = InStr(pos(2), Telefon, "')", CompareMethod.Text)
                                        If Mid(Telefon, pos(2), pos(3) - pos(2)) = "1'=='1" Then
                                            TelNr = AllIn
                                        Else
                                            TelNr = C_hf.EigeneVorwahlenEntfernen(TelNr)
                                        End If
                                        pos(4) = InStr(Telefon, "n = parseInt('", CompareMethod.Text) + Len("n = parseInt('")
                                        pos(5) = InStr(pos(4), Telefon, "'", CompareMethod.Text)
                                        DialPort = CStr(CInt(Mid(Telefon, pos(4), pos(5) - pos(4))) + 1) ' + 1 für FON
                                        pos(2) = InStr(pos(1), Telefon, "outgoing: '", CompareMethod.Text) + Len("outgoing: '")
                                        pos(3) = InStr(pos(2), Telefon, "'", CompareMethod.Text)
                                        PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("FON", DialPort, TelNr, TelName))
                                        If P_SpeichereDaten Then
                                            NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                            NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                            AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                            AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull
                                            C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                                        End If

                                        Anzahl += 1
                                    Case 1 ' S0-Port
                                        xPathTeile.Item(xPathTeile.Count - 1) = "S0"
                                        pos(2) = InStr(Telefon, "partyNo = '", CompareMethod.Text) + Len("partyNo = '")
                                        pos(3) = InStr(pos(2), Telefon, "'", CompareMethod.Text)
                                        If Not pos(2) = pos(3) Then
                                            AnzahlISDN += 1
                                            pos(4) = InStr(pos(1), Telefon, "allin: ('", CompareMethod.Text) + Len("allin: ('")
                                            pos(5) = InStr(pos(2), Telefon, "')", CompareMethod.Text)
                                            If Mid(Telefon, pos(4), pos(5) - pos(4)) = "true" Then
                                                TelNr = AllIn
                                            Else
                                                TelNr = Trim(Mid(Telefon, pos(2), pos(3) - pos(2)))
                                                TelNr = C_hf.EigeneVorwahlenEntfernen(TelNr)
                                            End If
                                            pos(4) = InStr(Telefon, "n = parseInt('", CompareMethod.Text) + Len("n = parseInt('")
                                            pos(5) = InStr(pos(4), Telefon, "'", CompareMethod.Text)
                                            ID = CInt(Mid(Telefon, pos(4), pos(5) - pos(4)))
                                            pos(2) = InStr(pos(1), Telefon, "outgoing: '", CompareMethod.Text) + Len("outgoing: '")
                                            pos(3) = InStr(pos(2), Telefon, "'", CompareMethod.Text)
                                            DialPort = "5" & ID
                                            PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("S0-", DialPort, TelNr, TelName))
                                            If P_SpeichereDaten Then
                                                NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                                NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                                AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                                AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull
                                                C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                                            End If

                                        End If
                                    Case 2 ' DECT Fritz!Fon 7150
                                        xPathTeile.Item(xPathTeile.Count - 1) = "FritzFon"
                                        Anzahl += 1
                                        pos(2) = InStr(Telefon, "n = parseInt('", CompareMethod.Text) + Len("n = parseInt('")
                                        pos(3) = InStr(pos(2), Telefon, "'", CompareMethod.Text)
                                        ID = CInt(Trim(Mid(Telefon, pos(2), pos(3) - pos(2))))
                                        TelNr = C_hf.EigeneVorwahlenEntfernen(TelNr)
                                        DialPort = "6" & ID
                                        TelName = "Fritz!Fon 7150"
                                        PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("DECT Fritz!Fon 7150-", DialPort, TelNr, TelName))
                                        If P_SpeichereDaten Then
                                            NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                            NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                            AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                            AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull
                                            C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                                        End If

                                    Case 3 ' DECT
                                        xPathTeile.Item(xPathTeile.Count - 1) = "DECT"
                                        Dim isUnpersonalizedMini() As String
                                        Dim tempTelNr As String
                                        pos(2) = InStr(Telefon, "isUnpersonalizedMini = '", CompareMethod.Text) + Len("isUnpersonalizedMini = '")
                                        pos(3) = InStr(pos(2), Telefon, "';", CompareMethod.Text)
                                        isUnpersonalizedMini = Split(Mid(Telefon, pos(2), pos(3) - pos(2)), "' == '", , CompareMethod.Text)
                                        If Not isUnpersonalizedMini(0) = isUnpersonalizedMini(1) Then
                                            Anzahl += 1
                                            pos(2) = InStr(Telefon, "intern: isUnpersonalizedMini ? '' : '**", CompareMethod.Text) + Len("intern: isUnpersonalizedMini ? '' : '**") + 2
                                            pos(3) = InStr(pos(2), Telefon, "'", CompareMethod.Text)
                                            DialPort = Trim(Mid(Telefon, pos(2), pos(3) - pos(2)))
                                            pos(2) = InStr(pos(1), Telefon, "allin: ('", CompareMethod.Text) + Len("allin: ('")
                                            pos(3) = InStr(pos(2), Telefon, "')", CompareMethod.Text)
                                            If Mid(Telefon, pos(2), pos(3) - pos(2)) = "1'=='1" Then
                                                TelNr = AllIn
                                            Else
                                                pos(2) = InStr(Telefon, "num = '", CompareMethod.Text) + Len("num = '")
                                                TelNr = DataProvider.P_Def_LeerString
                                                If Not pos(2) = 7 Then
                                                    Do
                                                        pos(3) = InStr(pos(2), Telefon, "'", CompareMethod.Text)
                                                        tempTelNr = Mid(Telefon, pos(2), pos(3) - pos(2))
                                                        TelNr = C_hf.EigeneVorwahlenEntfernen(TelNr)
                                                        TelNr += C_hf.IIf(Right(TelNr, 1) = "#", DataProvider.P_Def_LeerString, tempTelNr & ";")
                                                        pos(2) = InStr(pos(3), Telefon, "num = '", CompareMethod.Text) + Len("num = '")
                                                    Loop Until pos(2) = 7
                                                    TelNr = Left(TelNr, Len(TelNr) - 1)
                                                Else
                                                    pos(2) = InStr(TelNr, ":", CompareMethod.Text) + 2
                                                    TelNr = Trim(Mid(TelNr, pos(2)))
                                                    TelNr = C_hf.EigeneVorwahlenEntfernen(TelNr)
                                                End If
                                            End If
                                            pos(2) = InStr(pos(1), Telefon, "outgoing: isUnpersonalizedMini ? '' : '", CompareMethod.Text) + Len("outgoing: isUnpersonalizedMini ? '' : '")
                                            pos(3) = InStr(pos(2), Telefon, "'", CompareMethod.Text)
                                            PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("DECT ", DialPort, TelNr, TelName))

                                            If P_SpeichereDaten Then
                                                NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                                NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                                AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                                AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull
                                                C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                                            End If

                                        End If
                                    Case 4 ' IP-Telefone
                                        xPathTeile.Item(xPathTeile.Count - 1) = "VOIP"
                                        If Not Trim(TelName) = "TelCfg[Index].Name" Then
                                            pos(4) = InStr(Telefon, "n = parseInt('", CompareMethod.Text) + Len("n = parseInt('")
                                            pos(5) = InStr(pos(4), Telefon, "'", CompareMethod.Text)
                                            ID = CInt(Mid(Telefon, pos(4), pos(5) - pos(4)))
                                            Anzahl += 1
                                            DialPort = "2" & ID
                                            PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("VOIP", DialPort, TelNr, TelName))
                                            If P_SpeichereDaten Then
                                                NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                                NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                                AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                                AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull
                                                C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                                            End If
                                        Else
                                            Dim LANTelefone() As String = Split(Telefon, "in_nums = [];", , CompareMethod.Text)
                                            Dim InNums As String = DataProvider.P_Def_LeerString
                                            Dim NetInfo As String
                                            Dim NetInfoPush As String = DataProvider.P_Def_LeerString
                                            pos(0) = InStr(LANTelefone(LANTelefone.Length - 1), "NetInfo.push(parseInt('", CompareMethod.Text)
                                            If Not pos(0) = 0 Then
                                                NetInfo = Mid(LANTelefone(LANTelefone.Length - 1), pos(0))
                                                pos(0) = 1
                                                Do
                                                    pos(1) = InStr(pos(0), NetInfo, "', 10));", CompareMethod.Text) + Len("', 10));")
                                                    NetInfoPush = Mid(NetInfo, pos(0) + Len("NetInfo.push(parseInt('"), 3) & C_hf.IIf(Not NetInfoPush = DataProvider.P_Def_LeerString, ";" & NetInfoPush, DataProvider.P_Def_LeerString)
                                                    pos(0) = InStr(pos(1), NetInfo, "NetInfo.push(parseInt('", CompareMethod.Text)
                                                Loop Until pos(0) = 0
                                            End If
                                            For Each LANTelefon In LANTelefone
                                                If Not InStr(LANTelefon, "TelCfg.push( { Enabled : '", vbTextCompare) = 0 Then
                                                    Dim tempTelNr As String
                                                    pos(2) = InStr(LANTelefon, "num = '", CompareMethod.Text) + Len("num = '")
                                                    TelNr = DataProvider.P_Def_LeerString
                                                    If Not pos(2) = 7 Then
                                                        InNums = DataProvider.P_Def_LeerString
                                                        Do
                                                            pos(3) = InStr(pos(2), LANTelefon, "'", CompareMethod.Text)
                                                            tempTelNr = Mid(LANTelefon, pos(2), pos(3) - pos(2))
                                                            TelNr = C_hf.EigeneVorwahlenEntfernen(tempTelNr)
                                                            InNums += C_hf.IIf(Strings.Right(TelNr, 1) = "#", DataProvider.P_Def_LeerString, TelNr & ";")
                                                            pos(2) = InStr(pos(3), LANTelefon, "num = '", CompareMethod.Text) + Len("num = '")
                                                        Loop Until pos(2) = 7
                                                        InNums = Left(InNums, Len(InNums) - 1)
                                                    End If

                                                    pos(0) = InStr(LANTelefon, "Name : '", CompareMethod.Text) + Len("Name : '")
                                                    pos(1) = InStr(pos(0), LANTelefon, "'", CompareMethod.Text)
                                                    TelName = Mid(LANTelefon, pos(0), pos(1) - pos(0))
                                                    If Not TelName = DataProvider.P_Def_LeerString Then
                                                        pos(2) = InStr(pos(1), Telefon, "AllIn: ('", CompareMethod.Text) + Len("AllIn: ('")
                                                        pos(3) = InStr(pos(2), Telefon, "')", CompareMethod.Text)
                                                        If Mid(Telefon, pos(2), pos(3) - pos(2)) = "1' == '1" Then
                                                            TelNr = AllIn
                                                        Else
                                                            If Not InStr(LANTelefon, "InNums : in_nums", CompareMethod.Text) = 0 Then
                                                                TelNr = InNums
                                                            Else
                                                                pos(2) = InStr(pos(1), LANTelefon, "Number0 : '", CompareMethod.Text) + Len("Number0 : '")
                                                                pos(3) = InStr(pos(2), LANTelefon, "'", CompareMethod.Text)
                                                                TelNr = C_hf.EigeneVorwahlenEntfernen(Mid(LANTelefon, pos(2), pos(3) - pos(2)))
                                                            End If
                                                        End If
                                                        pos(4) = InStr(LANTelefon, "g_txtIpPhone + ' 62", CompareMethod.Text) + Len("g_txtIpPhone + ' 62")
                                                        ID = CInt(Mid(LANTelefon, pos(4), 1))
                                                        If NetInfoPush = DataProvider.P_Def_LeerString Then
                                                            If Not InStr(LANTelefon, "TelCfg.push( { Enabled : '1',", CompareMethod.Text) = 0 Then
                                                                DialPort = "2" & ID
                                                                Anzahl += 1
                                                                PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("IP-Telefon ", DialPort, TelNr, TelName))
                                                                If P_SpeichereDaten Then
                                                                    NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                                                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                                                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                                                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull
                                                                    C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                                                                End If

                                                            End If
                                                        Else
                                                            If C_hf.IsOneOf("62" & ID, Split(NetInfoPush, ";", , CompareMethod.Text)) Then
                                                                DialPort = "2" & ID
                                                                Anzahl += 1
                                                                PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("IP-Telefon ", DialPort, TelNr, TelName))
                                                                If P_SpeichereDaten Then
                                                                    NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                                                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                                                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                                                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull
                                                                    C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                                                                End If

                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            Next
                                        End If
                                    Case 5 ' Anrufbeantworter
                                        xPathTeile.Item(xPathTeile.Count - 1) = "TAM"
                                        Dim tamMsnBits As Integer
                                        TelNr = DataProvider.P_Def_LeerString
                                        pos(2) = InStr(Telefon, "tamDisplay = '", CompareMethod.Text) + Len("tamDisplay = '")
                                        pos(3) = InStr(pos(2), Telefon, "'", CompareMethod.Text)
                                        If Mid(Telefon, pos(2), pos(3) - pos(2)) = "1" Then
                                            pos(4) = InStr(Telefon, "n = parseInt('", CompareMethod.Text) + Len("n = parseInt('")
                                            pos(5) = InStr(pos(4), Telefon, "'", CompareMethod.Text)
                                            ID = CInt(Mid(Telefon, pos(4), pos(5) - pos(4)))
                                            pos(4) = InStr(Telefon, "var tamMsnBits = parseInt('", CompareMethod.Text) + Len("var tamMsnBits = parseInt('")
                                            pos(5) = InStr(pos(4), Telefon, "'", CompareMethod.Text)
                                            tamMsnBits = CInt(Mid(Telefon, pos(4), pos(5) - pos(4)))
                                            For j = 0 To TAM.Length - 1
                                                If TAM(j) IsNot Nothing Then
                                                    If (tamMsnBits And (1 << j)) > 0 Then ' Aus AVM Quellcode Funktion isBitSet übernommen 
                                                        TelNr += TAM(j) & ";"
                                                    End If
                                                End If
                                            Next
                                            If Not TelNr = DataProvider.P_Def_LeerString Then
                                                TelNr = Left(TelNr, Len(TelNr) - 1)
                                                DialPort = "60" & ID
                                                PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("TAM", DialPort, TelNr, TelName))
                                                If P_SpeichereDaten Then
                                                    NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull
                                                    C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                                                End If

                                                Anzahl += 1
                                            End If
                                        End If
                                    Case 6 ' integrierter Faxempfang
                                        xPathTeile.Item(xPathTeile.Count - 1) = "FAX"
                                        Dim FAXMSN(9) As String
                                        TelNr = DataProvider.P_Def_LeerString
                                        pos(2) = InStr(Telefon, "var isActive = '", CompareMethod.Text) + Len("var isActive = '")
                                        pos(3) = InStr(pos(2), Telefon, "'", CompareMethod.Text)
                                        If Not pos(2) = pos(3) Then
                                            If CInt(Mid(Telefon, pos(2), pos(3) - pos(2))) > 0 Then
                                                TelName = "Faxempfang"
                                                If InStr(Telefon, "allin: true", CompareMethod.Text) = 0 Then
                                                    pos(2) = InStr(Telefon, "var faxMsn = '", CompareMethod.Text) + Len("var faxMsn = '")
                                                    pos(3) = InStr(pos(2), Telefon, "'", CompareMethod.Text)
                                                    If Not pos(2) = Len("var faxMsn = '") Then
                                                        TelNr = Mid(Telefon, pos(2), pos(3) - pos(2))
                                                    Else
                                                        pos(3) = 1
                                                        For j = 0 To 9
                                                            pos(2) = InStr(pos(3), Telefon, "msn = '", CompareMethod.Text) + Len("msn = '")
                                                            pos(3) = InStr(pos(2), Telefon, "'", CompareMethod.Text)
                                                            FAXMSN(j) = Mid(Telefon, pos(2), pos(3) - pos(2))
                                                        Next
                                                        pos(2) = InStr(Telefon, "number: faxMsns[", CompareMethod.Text) + Len("number: faxMsns[")
                                                        pos(3) = InStr(pos(2), Telefon, "]", CompareMethod.Text)
                                                        TelNr = FAXMSN(CInt(Mid(Telefon, pos(2), pos(3) - pos(2))))
                                                    End If
                                                Else
                                                    TelNr = AllIn
                                                End If
                                                DialPort = "5"

                                                PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("Integrierte Faxfunktion", DialPort, TelNr, TelName))
                                                If P_SpeichereDaten Then
                                                    NodeValues.Item(NodeNames.IndexOf("TelName")) = "Faxempfang"
                                                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = "1"
                                                    C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                                                End If
                                                Anzahl += 1
                                            End If
                                        End If
                                End Select
                            End If
                        End If
                    End If
                Next
            Next

            If Not AnzahlISDN = 0 Then
                DialPort = "50"
                PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("ISDN-Basis", DialPort, DataProvider.P_Def_LeerString, "ISDN-Basis"))
                If P_SpeichereDaten Then
                    NodeValues.Item(NodeNames.IndexOf("TelName")) = "ISDN-Basis"
                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = DataProvider.P_Def_LeerString
                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_LeerString
                    C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                End If

            End If
        Else
            C_hf.LogFile("FBError (FritzBoxDatenA): " & Err.Number & " - " & Err.Description & " - " & sLink)
        End If

    End Sub ' (FritzBoxDaten für ältere Firmware)

    Private Sub FritzBoxDatenV2(ByVal Code As String)
        PushStatus(DataProvider.P_FritzBox_Tel_NeueRoutine)

        'Dim Vorwahl As String = C_DP.P_TBVorwahl                 ' In den Einstellungen eingegebene Vorwahl
        Dim Landesvorwahl As String
        Dim TelName As String                 ' Gefundener Telefonname
        Dim TelNr As String                 ' Dazugehörige Telefonnummer
        Dim SIPID As String = DataProvider.P_Def_ErrorMinusOne_String
        Dim pos(1) As Integer
        Dim i As Integer                   ' Laufvariable
        Dim j As Integer
        Dim k As Integer
        Dim SIP(20) As String
        Dim TAM(10) As String
        Dim MSNPort(2, 9) As String
        Dim MSN(9) As String
        Dim FAX(9) As String
        Dim Mobil As String = DataProvider.P_Def_LeerString
        Dim POTS As String = DataProvider.P_Def_LeerString
        Dim allin As String
        Dim DialPort As String = "0"

        Dim tmpTelefone As String
        Dim tmpstrUser() As String
        Dim Node As String
        Dim tmpTelNr As String
        Dim Port As String

        Dim xPathTeile As New ArrayList
        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList

        If P_SpeichereDaten Then C_XML.Delete(C_DP.XMLDoc, "Telefone")

        With xPathTeile
            .Clear()
            .Add("Telefone")
            .Add("Nummern")
        End With
        With NodeNames
            .Clear()
            .Add("TelName")
            .Add("TelNr")
        End With
        With AttributeNames
            .Clear()
            .Add("Fax")
            .Add("Dialport")
        End With
        With NodeValues
            .Clear()
            .Add(DataProvider.P_Def_LeerString)
            .Add(DataProvider.P_Def_LeerString)
        End With
        With AttributeValues
            .Clear()
            .Add(DataProvider.P_Def_LeerString)
            .Add(DataProvider.P_Def_LeerString)
        End With

        With C_hf

            ' SIP Nummern
            xPathTeile.Add("SIP")
            For Each SIPi In Split(.StringEntnehmen(Code, "['sip:settings/sip/list(" & .StringEntnehmen(Code, "['sip:settings/sip/list(", ")'] = {") & ")'] = {", "}" & Chr(10) & "  },"), " },", , CompareMethod.Text)
                If .StringEntnehmen(SIPi, "['activated'] = '", "'") = "1" Then
                    TelNr = .EigeneVorwahlenEntfernen(.StringEntnehmen(SIPi, "['displayname'] = '", "'"))
                    Node = UCase(.StringEntnehmen(SIPi, "['_node'] = '", "'"))
                    SIPID = .StringEntnehmen(SIPi, "['ID'] = '", "'")
                    SIP(CInt(SIPID)) = TelNr
                    PushStatus(DataProvider.P_FritzBox_Tel_NrFound("SIP", Node, TelNr))
                    If P_SpeichereDaten Then
                        C_XML.Write(C_DP.XMLDoc, xPathTeile, TelNr, "ID", SIPID)
                    End If
                End If
            Next

            PushStatus("Letzte SIP: " & SIPID)

            ' MSN Nummern
            xPathTeile.Item(xPathTeile.IndexOf("SIP")) = "MSN"
            For i = 0 To 9
                TelNr = .StringEntnehmen(Code, "['telcfg:settings/MSN/MSN" & i & "'] = '", "'")
                If Not TelNr = DataProvider.P_Def_ErrorMinusOne_String Then
                    If Not Len(TelNr) = 0 Then
                        TelNr = .EigeneVorwahlenEntfernen(TelNr)
                        MSN(i) = TelNr
                        PushStatus(DataProvider.P_FritzBox_Tel_NrFound("MSN", CStr(i), TelNr))
                        If P_SpeichereDaten Then
                            C_XML.Write(C_DP.XMLDoc, xPathTeile, TelNr, "ID", CStr(i))
                        End If
                    End If
                End If
            Next

            For i = 0 To 2
                If Not .StringEntnehmen(Code, "['telcfg:settings/MSN/Port" & i & "/Name'] = '", "'") = DataProvider.P_Def_ErrorMinusOne_String Then
                    For j = 0 To 9
                        TelNr = .StringEntnehmen(Code, "['telcfg:settings/MSN/Port" & i & "/MSN" & j & "'] = '", "'")
                        If Not TelNr = DataProvider.P_Def_ErrorMinusOne_String Then
                            If Not Len(TelNr) = 0 Then
                                If TelNr.StartsWith("SIP") Then
                                    TelNr = SIP(CInt(Mid(TelNr, 4, 1)))
                                Else
                                    TelNr = .EigeneVorwahlenEntfernen(TelNr)
                                End If

                                If Not .IsOneOf(TelNr, MSN) Then
                                    For k = 0 To 9
                                        If MSN(k) = DataProvider.P_Def_LeerString Then
                                            MSN(k) = TelNr
                                            PushStatus(DataProvider.P_FritzBox_Tel_NrFound("MSN", CStr(i), TelNr))
                                            If P_SpeichereDaten Then
                                                C_XML.Write(C_DP.XMLDoc, xPathTeile, TelNr, "ID", CStr(k))
                                            End If
                                            Exit For
                                        End If
                                    Next
                                End If
                                MSNPort(i, j) = TelNr
                            End If
                        End If
                    Next
                End If
            Next

            ' TAM Nummern
            xPathTeile.Item(xPathTeile.IndexOf("MSN")) = "TAM"
            For i = 0 To 9
                TelNr = .StringEntnehmen(Code, "['tam:settings/MSN" & i & "'] = '", "'")
                If Not TelNr = DataProvider.P_Def_ErrorMinusOne_String Then
                    If Not Len(TelNr) = 0 Then
                        If TelNr.StartsWith("SIP") Then
                            TelNr = SIP(CInt(Mid(TelNr, 4, 1)))
                        Else
                            TelNr = .EigeneVorwahlenEntfernen(TelNr)
                        End If
                        PushStatus(DataProvider.P_FritzBox_Tel_NrFound("TAM", CStr(i), TelNr))
                        If P_SpeichereDaten Then
                            C_XML.Write(C_DP.XMLDoc, xPathTeile, TelNr, "ID", CStr(i))
                        End If

                        TAM(i) = TelNr
                    End If
                End If
            Next

            ' FAX Nummern
            xPathTeile.Item(xPathTeile.IndexOf("TAM")) = "FAX"
            For i = 0 To 9
                TelNr = .StringEntnehmen(Code, "['telcfg:settings/FaxMSN" & i & "'] = '", "'")
                If Not TelNr = DataProvider.P_Def_ErrorMinusOne_String Then
                    If Not Len(TelNr) = 0 Then
                        If TelNr.StartsWith("SIP") Then
                            TelNr = SIP(CInt(Mid(TelNr, 4, 1)))
                        Else
                            TelNr = .EigeneVorwahlenEntfernen(TelNr)
                        End If
                        PushStatus(DataProvider.P_FritzBox_Tel_NrFound("FAX", CStr(i), TelNr))
                        If P_SpeichereDaten Then
                            C_XML.Write(C_DP.XMLDoc, xPathTeile, TelNr, "ID", CStr(i))
                        End If

                        FAX(i) = TelNr
                    End If
                End If
            Next

            ' POTSnummer
            xPathTeile.Item(xPathTeile.IndexOf("FAX")) = "POTS"
            POTS = .StringEntnehmen(Code, "['telcfg:settings/MSN/POTS'] = '", "'")
            If Not POTS = DataProvider.P_Def_ErrorMinusOne_String And Not POTS = DataProvider.P_Def_LeerString Then
                If POTS.StartsWith("SIP") Then
                    POTS = SIP(CInt(Mid(POTS, 4, 1)))
                Else
                    POTS = .EigeneVorwahlenEntfernen(POTS)
                End If
                PushStatus(DataProvider.P_FritzBox_Tel_NrFound("POTS", CStr(0), POTS))
                If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, POTS, "ID", DataProvider.P_Def_StringNull)
            End If

            xPathTeile.Item(xPathTeile.IndexOf("POTS")) = "Mobil"

            ' Mobilnummer
            Mobil = .StringEntnehmen(Code, "['telcfg:settings/Mobile/MSN'] = '", "'")
            If Not Mobil = DataProvider.P_Def_ErrorMinusOne_String And Not Mobil = DataProvider.P_Def_LeerString Then
                If Mobil.StartsWith("SIP") Then
                    Mobil = SIP(CInt(Mid(Mobil, 4, 1)))
                Else
                    'Mobil = .EigeneVorwahlenEntfernen(Mobil)
                End If
                PushStatus(DataProvider.P_FritzBox_Tel_NrFound("Mobil", DataProvider.P_Def_MobilDialPort, Mobil))
                If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, Mobil, "ID", DataProvider.P_Def_MobilDialPort)
            End If

            SIP = C_hf.ClearStringArray(SIP)
            MSN = C_hf.ClearStringArray(MSN)
            FAX = C_hf.ClearStringArray(FAX)

            allin = AlleNummern(MSN, SIP, TAM, FAX, POTS, Mobil)

            'Telefone Einlesen

            pos(0) = 1
            xPathTeile.Item(xPathTeile.IndexOf("Nummern")) = "Telefone"
            xPathTeile.Item(xPathTeile.IndexOf("Mobil")) = "FON"
            'FON
            For Each Telefon In Split(.StringEntnehmen(Code, "['telcfg:settings/MSN/Port/list(" & .StringEntnehmen(Code, "['telcfg:settings/MSN/Port/list(", ")'] = {") & ")'] = {", "}" & Chr(10) & "  },"), " },", , CompareMethod.Text)
                TelName = .StringEntnehmen(Telefon, "['Name'] = '", "'")
                If Not (TelName = DataProvider.P_Def_ErrorMinusOne_String Or TelName = DataProvider.P_Def_LeerString) Then
                    TelNr = DataProvider.P_Def_LeerString
                    Port = Right(.StringEntnehmen(Telefon, "['_node'] = '", "'"), 1)

                    Dim tmparray(9) As String
                    For i = 0 To 9
                        tmpTelNr = MSNPort(CInt(Port), i)
                        If Not tmpTelNr = DataProvider.P_Def_LeerString Then
                            tmparray(i) = MSNPort(CInt(Port), i)
                        Else
                            Exit For
                        End If
                    Next
                    tmparray = C_hf.ClearStringArray(tmparray)
                    If tmparray.Length = 0 Then tmparray = MSN

                    TelNr = String.Join(";", tmparray)
                    DialPort = CStr(CInt(Port) + 1) ' + 1 für FON
                    PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("FON", DialPort, TelNr, TelName))
                    If P_SpeichereDaten Then
                        NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                        NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr

                        AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                        AttributeValues.Item(AttributeNames.IndexOf("Fax")) = .StringEntnehmen(Telefon, "['Fax'] = '", "'")
                        C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                    End If
                    If .StringEntnehmen(Telefon, "['Fax'] = '", "'") = "1" Then PushStatus(DataProvider.P_FritzBox_Tel_DeviceisFAX(DialPort, TelName))
                End If
            Next

            ' DECT
            xPathTeile.Item(xPathTeile.IndexOf("FON")) = "DECT"
            tmpTelefone = .StringEntnehmen(Code, "['telcfg:settings/Foncontrol/User/list(" & .StringEntnehmen(Code, "['telcfg:settings/Foncontrol/User/list(", ")'] = {") & ")'] = {", "}" & Chr(10) & "  },")

            For Each DectTelefon In Split(tmpTelefone, "] = {", , CompareMethod.Text)

                DialPort = .StringEntnehmen(DectTelefon, "['Intern'] = '", "'")
                If Not (DialPort = DataProvider.P_Def_ErrorMinusOne_String Or DialPort = DataProvider.P_Def_LeerString) Then
                    TelNr = DataProvider.P_Def_LeerString
                    DialPort = "6" & Strings.Right(DialPort, 1)
                    TelName = .StringEntnehmen(DectTelefon, "['Name'] = '", "'")
                    Node = .StringEntnehmen(DectTelefon, "['_node'] = '", "'")

                    If .StringEntnehmen(Code, "['telcfg:settings/Foncontrol/" & Node & "/RingOnAllMSNs'] = '", "',") = "1" Then
                        TelNr = allin
                    Else
                        tmpstrUser = Split(.StringEntnehmen(Code, "['telcfg:settings/Foncontrol/" & Node & "/MSN/list(Number)'] = {", "}" & Chr(10) & "  },"), "['Number'] = '", , CompareMethod.Text)

                        tmpstrUser(0) = DataProvider.P_Def_LeerString
                        For l As Integer = 1 To tmpstrUser.Length - 1
                            tmpstrUser(l) = Strings.Left(tmpstrUser(l), InStr(tmpstrUser(l), "'", CompareMethod.Text) - 1)
                        Next
                        For Each Nr As String In C_hf.ClearStringArray(tmpstrUser)
                            TelNr = TelNr & ";" & .EigeneVorwahlenEntfernen(Nr)
                        Next
                        TelNr = Mid(TelNr, 2)
                    End If
                    PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("DECT", DialPort, TelNr, TelName))
                    If P_SpeichereDaten Then
                        NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                        NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                        AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                        AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull

                        C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                    End If

                End If
            Next

            xPathTeile.Item(xPathTeile.IndexOf("DECT")) = "VOIP"
            'IP-Telefone
            tmpstrUser = Split(.StringEntnehmen(Code, "['telcfg:settings/VoipExtension/list(" & .StringEntnehmen(Code, "['telcfg:settings/VoipExtension/list(", ")'] = {") & ")'] = {", "}" & Chr(10) & "  },"), " },", , CompareMethod.Text)
            For Each Telefon In tmpstrUser
                If .StringEntnehmen(Telefon, "['enabled'] = '", "'") = "1" Then
                    TelName = .StringEntnehmen(Telefon, "['Name'] = '", "'")
                    TelNr = DataProvider.P_Def_LeerString
                    Port = .StringEntnehmen(Telefon, "['_node'] = '", "'")
                    For j = 0 To 9
                        tmpTelNr = .StringEntnehmen(Code, "['telcfg:settings/" & Port & "/Number" & j & "'] = '", "'")
                        If Not tmpTelNr = DataProvider.P_Def_ErrorMinusOne_String Then
                            If Not Len(tmpTelNr) = 0 Then
                                If Strings.Left(tmpTelNr, 3) = "SIP" Then
                                    tmpTelNr = SIP(CInt(Mid(tmpTelNr, 4, 1)))
                                Else
                                    tmpTelNr = .EigeneVorwahlenEntfernen(tmpTelNr)
                                End If
                                TelNr = tmpTelNr & ";" & TelNr
                            End If
                        End If
                    Next
                    If Not TelNr = DataProvider.P_Def_LeerString Then
                        TelNr = Strings.Left(TelNr, Len(TelNr) - 1)
                    End If

                    DialPort = "2" & Strings.Right(Port, 1)
                    PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("VOIP", DialPort, TelNr, TelName))
                    If P_SpeichereDaten Then
                        NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                        NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                        AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                        AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull

                        C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                    End If

                End If
            Next
            xPathTeile.Item(xPathTeile.IndexOf("VOIP")) = "S0"
            Dim S0Typ As String
            ' S0-Port
            For i = 1 To 8
                TelName = .StringEntnehmen(Code, "['telcfg:settings/NTHotDialList/Name" & i & "'] = '", "'")
                If Not TelName = DataProvider.P_Def_ErrorMinusOne_String Then
                    If Not TelName = DataProvider.P_Def_LeerString Then
                        TelNr = .StringEntnehmen(Code, "['telcfg:settings/NTHotDialList/Number" & i & "'] = '", "'")
                        If Not TelNr = DataProvider.P_Def_ErrorMinusOne_String Then
                            DialPort = "5" & i
                            PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("S0-", DialPort, TelNr, TelName))
                            If P_SpeichereDaten Then
                                NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                AttributeValues.Item(AttributeNames.IndexOf("Fax")) = .IIf(.StringEntnehmen(Code, "['telcfg:settings/NTHotDialList/Type" & i & "'] = '", "'") = "Fax", 1, 0)
                                C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                            End If

                            S0Typ = .StringEntnehmen(Code, "['telcfg:settings/NTHotDialList/Type" & i & "'] = '", "'")
                            If Not TelNr = DataProvider.P_Def_ErrorMinusOne_String Then
                                Select Case S0Typ
                                    Case "Fax"
                                        PushStatus(DataProvider.P_FritzBox_Tel_DeviceisFAX(DialPort, TelName))
                                        'Case "Isdn"
                                        'Case "Fon"
                                        'Case Else
                                End Select
                            End If
                        End If
                    End If
                End If
            Next
            If Not DialPort = DataProvider.P_Def_LeerString Then
                If CDbl(DialPort) > 50 And CDbl(DialPort) < 60 Then
                    DialPort = "50"
                    PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("ISDN-Basis", DialPort, DataProvider.P_Def_LeerString, "ISDN-Basis"))
                    If P_SpeichereDaten Then
                        NodeValues.Item(NodeNames.IndexOf("TelName")) = "ISDN-Basis"
                        NodeValues.Item(NodeNames.IndexOf("TelNr")) = "50"
                        AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                        AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull
                        C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                    End If
                End If
            End If

            xPathTeile.Item(xPathTeile.IndexOf("S0")) = "TAM"
            ' TAM, Anrufbeantworter
            tmpstrUser = Split(.StringEntnehmen(Code, "['tam:settings/TAM/list(" & .StringEntnehmen(Code, "['tam:settings/TAM/list(", ")'] = {") & ")'] = {", "}" & Chr(10) & "  },"), " },", , CompareMethod.Text)
            For Each Anrufbeantworter In tmpstrUser
                If .StringEntnehmen(Anrufbeantworter, "['Active'] = '", "'") = "1" Then
                    TelName = .StringEntnehmen(Anrufbeantworter, "['Name'] = '", "'")
                    Port = .StringEntnehmen(Anrufbeantworter, "['_node'] = '", "'")
                    TelNr = .EigeneVorwahlenEntfernen(TAM(CInt(Strings.Right(Port, 1))))
                    DialPort = "60" & Strings.Right(Port, 1)
                    PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("TAM", DialPort, TelNr, TelName))
                    If P_SpeichereDaten Then
                        NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                        NodeValues.Item(NodeNames.IndexOf("TelNr")) = .IIf(TelNr = DataProvider.P_Def_LeerString, allin, TelNr)
                        AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                        AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull
                        C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                    End If
                End If
            Next

            ' integrierter Faxempfang
            xPathTeile.Item(xPathTeile.IndexOf("TAM")) = "FAX"
            DialPort = .StringEntnehmen(Code, "['telcfg:settings/FaxMailActive'] = '", "'")
            If DialPort IsNot DataProvider.P_Def_StringNull Then
                TelNr = Join(FAX, ";")
                DialPort = "5"
                TelName = "Faxempfang"
                PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("Integrierte Faxfunktion", DialPort, TelNr, TelName))
                If P_SpeichereDaten Then
                    NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = "1"

                    C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                End If
            End If

            ' Mobiltelefon
            xPathTeile.Item(xPathTeile.IndexOf("FAX")) = "Mobil"
            If Mobil IsNot DataProvider.P_Def_LeerString Then
                TelName = .StringEntnehmen(Code, "['telcfg:settings/Mobile/Name'] = '", "'")
                DialPort = DataProvider.P_Def_MobilDialPort
                PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("Mobil", DialPort, Mobil, TelName))
                If P_SpeichereDaten Then
                    NodeValues.Item(NodeNames.IndexOf("TelName")) = .IIf(TelName = DataProvider.P_Def_ErrorMinusOne_String Or TelName = DataProvider.P_Def_LeerString, Mobil, TelName)
                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = Mobil
                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull

                    C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                End If
            End If

            ' Landesvorwahl 
            Landesvorwahl = .StringEntnehmen(Code, "['country'] = '", "'")
            If Len(Landesvorwahl) > 2 Then
                If Len(Landesvorwahl) = 3 And Left(Landesvorwahl, 1) = "0" Then
                    Landesvorwahl = "0" & Landesvorwahl
                End If
                C_DP.P_TBLandesVW = Landesvorwahl
            End If

        End With

    End Sub

    Private Sub FritzBoxDatenV3()
        Dim TelQuery As New ArrayList

        Dim i As Integer
        Dim j As Integer

        Dim FritzBoxJSONTelNr1 As FritzBoxJSONTelNrT1 = Nothing
        Dim FritzBoxJSONTelNr2 As FritzBoxJSONTelNrT2 = Nothing
        Dim FritzBoxJSONTelefone1 As FritzBoxJSONTelefone1 = Nothing
        Dim FritzBoxJSONTelefone2 As FritzBoxJSONTelefone2 = Nothing

        Dim TelName As String                 ' Gefundener Telefonname
        Dim TelNr As String                 ' Dazugehörige Telefonnummer
        Dim SIPID As String = DataProvider.P_Def_ErrorMinusOne_String
        Dim pos(1) As Integer
        Dim SIP(20) As String
        Dim TAM(10) As String
        Dim MSN(36) As String
        Dim FAX(9) As String
        Dim Mobil As String = DataProvider.P_Def_LeerString
        Dim POTS As String = DataProvider.P_Def_LeerString
        Dim allin As String = DataProvider.P_Def_LeerString
        Dim DialPort As String = "0"

        Dim tmpStr As String
        Dim tmpStrArr As String()

        Dim xPathTeile As New ArrayList
        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList

        If P_SpeichereDaten Then C_XML.Delete(C_DP.XMLDoc, "Telefone")

        With xPathTeile
            .Clear()
            .Add("Telefone")
            .Add("Nummern")
        End With
        With NodeNames
            .Clear()
            .Add("TelName")
            .Add("TelNr")
        End With
        With AttributeNames
            .Clear()
            .Add("Fax")
            .Add("Dialport")
        End With
        With NodeValues
            .Clear()
            .Add(DataProvider.P_Def_LeerString)
            .Add(DataProvider.P_Def_LeerString)
        End With
        With AttributeValues
            .Clear()
            .Add(DataProvider.P_Def_LeerString)
            .Add(DataProvider.P_Def_LeerString)
        End With

        ' Telefonnummern ermitteln

        With TelQuery

            ' S0-Port
            For i = 1 To 8
                .Add(P_Query_FB_S0("Name", i))
                .Add(P_Query_FB_S0("Number", i))
            Next

            .Add(P_Query_FB_POTS)
            .Add(P_Query_FB_Mobile)

            ' FON
            For i = 0 To 2
                .Add(P_Query_FB_FON(i))
            Next

            For i = 0 To 9
                .Add(P_Query_FB_TAM(i))
                .Add(P_Query_FB_FAX(i))
                .Add(P_Query_FB_MSN(i))
                .Add(P_Query_FB_VOIP(i))
            Next

            .Add(P_Query_FB_SIP)

            ' Führt das Fritz!Box Query aus und gibt die ersten Daten der Telefonnummern zurück
            FritzBoxJSONTelNr1 = GetFBJSON1(TelQuery)
            .Clear()
            ' MSN Nummern mit zugehöriger Nummer ermitteln.
            ' Es werden nur die Nummern ausgelesen, die auch einen Namen haben

            If Not FritzBoxJSONTelNr1 Is Nothing Then

                tmpStrArr = Split(String.Join(";", _
                    IIf(FritzBoxJSONTelNr1.Port0Name = DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, "0"), _
                    IIf(FritzBoxJSONTelNr1.Port1Name = DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, "1"), _
                    IIf(FritzBoxJSONTelNr1.Port2Name = DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, "2")), ";", , CompareMethod.Text)

                For Each idx In C_hf.ClearStringArray(tmpStrArr, False)
                    For j = 0 To 9
                        .Add(P_Query_FB_MSN_List(idx, j))
                    Next
                Next

                tmpStrArr = Split(String.Join(";", _
                    IIf(FritzBoxJSONTelNr1.VOIP0Enabled = "1", "0", DataProvider.P_Def_LeerString), _
                    IIf(FritzBoxJSONTelNr1.VOIP1Enabled = "1", "1", DataProvider.P_Def_LeerString), _
                    IIf(FritzBoxJSONTelNr1.VOIP2Enabled = "1", "2", DataProvider.P_Def_LeerString), _
                    IIf(FritzBoxJSONTelNr1.VOIP3Enabled = "1", "3", DataProvider.P_Def_LeerString), _
                    IIf(FritzBoxJSONTelNr1.VOIP4Enabled = "1", "4", DataProvider.P_Def_LeerString), _
                    IIf(FritzBoxJSONTelNr1.VOIP5Enabled = "1", "5", DataProvider.P_Def_LeerString), _
                    IIf(FritzBoxJSONTelNr1.VOIP6Enabled = "1", "6", DataProvider.P_Def_LeerString), _
                    IIf(FritzBoxJSONTelNr1.VOIP7Enabled = "1", "7", DataProvider.P_Def_LeerString), _
                    IIf(FritzBoxJSONTelNr1.VOIP8Enabled = "1", "8", DataProvider.P_Def_LeerString), _
                    IIf(FritzBoxJSONTelNr1.VOIP9Enabled = "1", "9", DataProvider.P_Def_LeerString)), ";", , CompareMethod.Text)

                For Each idx In C_hf.ClearStringArray(tmpStrArr, False)
                    For j = 0 To 9
                        .Add(P_Query_FB_VOIP_List(idx, j))
                    Next
                Next

                FritzBoxJSONTelNr2 = GetFBJSON2(TelQuery)

                If Not FritzBoxJSONTelNr2 Is Nothing Then
                    ' SIP Nummern ermitteln
                    xPathTeile.Add("SIP")
                    For Each SIPi As SIPEntry In FritzBoxJSONTelNr1.SIP
                        With SIPi
                            If CBool(.activated) Then
                                TelNr = C_hf.EigeneVorwahlenEntfernen(.displayname)
                                SIPID = .ID
                                SIP(CInt(SIPID)) = TelNr
                                PushStatus(DataProvider.P_FritzBox_Tel_NrFound("SIP", SIPID, TelNr))
                                If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, TelNr, "ID", SIPID)
                            End If
                        End With
                    Next

                    SIP = C_hf.ClearStringArray(SIP)

                    ' MSN Nummern
                    xPathTeile.Item(xPathTeile.Count - 1) = "MSN"
                    j = 0
                    With FritzBoxJSONTelNr1
                        MSN(j) = .MSN0 : j += 1
                        MSN(j) = .MSN1 : j += 1
                        MSN(j) = .MSN2 : j += 1
                        MSN(j) = .MSN3 : j += 1
                        MSN(j) = .MSN4 : j += 1
                        MSN(j) = .MSN5 : j += 1
                        MSN(j) = .MSN6 : j += 1
                        MSN(j) = .MSN7 : j += 1
                        MSN(j) = .MSN8 : j += 1
                        MSN(j) = .MSN9 : j += 1
                    End With

                    With FritzBoxJSONTelNr2
                        If Not FritzBoxJSONTelNr1.Port0Name = DataProvider.P_Def_LeerString Then
                            MSN(j) = .MSN0Nr0 : j += 1
                            MSN(j) = .MSN0Nr1 : j += 1
                            MSN(j) = .MSN0Nr2 : j += 1
                            MSN(j) = .MSN0Nr3 : j += 1
                            MSN(j) = .MSN0Nr4 : j += 1
                            MSN(j) = .MSN0Nr5 : j += 1
                            MSN(j) = .MSN0Nr6 : j += 1
                            MSN(j) = .MSN0Nr7 : j += 1
                            MSN(j) = .MSN0Nr8 : j += 1
                            MSN(j) = .MSN0Nr9 : j += 1
                        End If

                        If Not FritzBoxJSONTelNr1.Port1Name = DataProvider.P_Def_LeerString Then
                            MSN(j) = .MSN1Nr0 : j += 1
                            MSN(j) = .MSN1Nr1 : j += 1
                            MSN(j) = .MSN1Nr2 : j += 1
                            MSN(j) = .MSN1Nr3 : j += 1
                            MSN(j) = .MSN1Nr4 : j += 1
                            MSN(j) = .MSN1Nr5 : j += 1
                            MSN(j) = .MSN1Nr6 : j += 1
                            MSN(j) = .MSN1Nr7 : j += 1
                            MSN(j) = .MSN1Nr8 : j += 1
                            MSN(j) = .MSN1Nr9 : j += 1
                        End If

                        If Not FritzBoxJSONTelNr1.Port2Name = DataProvider.P_Def_LeerString Then
                            MSN(j) = .MSN2Nr0 : j += 1
                            MSN(j) = .MSN2Nr1 : j += 1
                            MSN(j) = .MSN2Nr2 : j += 1
                            MSN(j) = .MSN2Nr3 : j += 1
                            MSN(j) = .MSN2Nr4 : j += 1
                            MSN(j) = .MSN2Nr5 : j += 1
                            MSN(j) = .MSN2Nr6 : j += 1
                            MSN(j) = .MSN2Nr7 : j += 1
                            MSN(j) = .MSN2Nr8 : j += 1
                            MSN(j) = .MSN2Nr9 : j += 1
                        End If
                    End With

                    MSN = C_hf.ClearStringArray(MSN)

                    For i = LBound(MSN) To UBound(MSN)
                        If MSN(i).StartsWith("SIP") Then
                            MSN(i) = SIP(CInt(Mid(MSN(i), 4, 1)))
                        Else
                            MSN(i) = C_hf.EigeneVorwahlenEntfernen(MSN(i))
                        End If
                        PushStatus(DataProvider.P_FritzBox_Tel_NrFound("MSN", CStr(i), MSN(i)))
                        If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, MSN(i), "ID", CStr(i))
                    Next

                    ' TAM Nummern
                    xPathTeile.Item(xPathTeile.Count - 1) = "TAM"
                    j = 0
                    With FritzBoxJSONTelNr1
                        TAM(j) = .TAM0 : j += 1
                        TAM(j) = .TAM1 : j += 1
                        TAM(j) = .TAM2 : j += 1
                        TAM(j) = .TAM3 : j += 1
                        TAM(j) = .TAM4 : j += 1
                        TAM(j) = .TAM5 : j += 1
                        TAM(j) = .TAM6 : j += 1
                        TAM(j) = .TAM7 : j += 1
                        TAM(j) = .TAM8 : j += 1
                        TAM(j) = .TAM9 : j += 1
                    End With

                    TAM = C_hf.ClearStringArray(TAM)

                    For i = LBound(TAM) To UBound(TAM)
                        If TAM(i).StartsWith("SIP") Then
                            TAM(i) = SIP(CInt(Mid(TAM(i), 4, 1)))
                        Else
                            TAM(i) = C_hf.EigeneVorwahlenEntfernen(TAM(i))
                        End If
                        PushStatus(DataProvider.P_FritzBox_Tel_NrFound("TAM", CStr(i), TAM(i)))
                        If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, TAM(i), "ID", CStr(i))
                    Next

                    ' FAX Nummern
                    xPathTeile.Item(xPathTeile.Count - 1) = "FAX"
                    j = 0

                    With FritzBoxJSONTelNr1
                        FAX(j) = .FAX0 : j += 1
                        FAX(j) = .FAX1 : j += 1
                        FAX(j) = .FAX2 : j += 1
                        FAX(j) = .FAX3 : j += 1
                        FAX(j) = .FAX4 : j += 1
                        FAX(j) = .FAX5 : j += 1
                        FAX(j) = .FAX6 : j += 1
                        FAX(j) = .FAX7 : j += 1
                        FAX(j) = .FAX8 : j += 1
                        FAX(j) = .FAX9 : j += 1
                    End With

                    FAX = C_hf.ClearStringArray(FAX)

                    For i = LBound(FAX) To UBound(FAX)
                        If FAX(i).StartsWith("SIP") Then
                            FAX(i) = SIP(CInt(Mid(FAX(i), 4, 1)))
                        Else
                            FAX(i) = C_hf.EigeneVorwahlenEntfernen(FAX(i))
                        End If
                        PushStatus(DataProvider.P_FritzBox_Tel_NrFound("FAX", CStr(i), FAX(i)))
                        If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, FAX(i), "ID", CStr(i))
                    Next

                    ' POTS
                    xPathTeile.Item(xPathTeile.Count - 1) = "POTS"
                    If Not FritzBoxJSONTelNr1.POTS = DataProvider.P_Def_LeerString Then
                        If FritzBoxJSONTelNr1.POTS.StartsWith("SIP") Then
                            FritzBoxJSONTelNr1.POTS = SIP(CInt(Mid(FritzBoxJSONTelNr1.POTS, 4, 1)))
                        Else
                            POTS = C_hf.EigeneVorwahlenEntfernen(FritzBoxJSONTelNr1.POTS)
                        End If
                        PushStatus(DataProvider.P_FritzBox_Tel_NrFound("POTS", DataProvider.P_Def_LeerString, POTS))
                        If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, FritzBoxJSONTelNr1.POTS, "ID", DataProvider.P_Def_StringNull)
                    End If

                    ' Mobilnummer
                    xPathTeile.Item(xPathTeile.Count - 1) = "Mobil"
                    Mobil = FritzBoxJSONTelNr1.Mobile
                    If Not Mobil = DataProvider.P_Def_LeerString Then
                        If Mobil.StartsWith("SIP") Then
                            Mobil = SIP(CInt(Mid(Mobil, 4, 1)))
                        End If
                        PushStatus(DataProvider.P_FritzBox_Tel_NrFound("Mobil", DataProvider.P_Def_MobilDialPort, Mobil))
                        If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, Mobil, "ID", DataProvider.P_Def_MobilDialPort)
                    End If

                    allin = AlleNummern(MSN, SIP, TAM, FAX, POTS, Mobil)

                    .Clear()
                    .Add(P_Query_FB_FON_List)       ' FON
                    .Add(P_Query_FB_DECT_List)      ' DECT (Teil1)
                    .Add(P_Query_FB_VOIP_List)      ' IP-Telefoen
                    .Add(P_Query_FB_TAM_List)       ' TAM

                    For i = 1 To 8
                        .Add(P_Query_FB_S0_List(i)) ' S0
                    Next

                    FritzBoxJSONTelefone1 = GetFBJSON3(TelQuery)
                    If Not FritzBoxJSONTelefone1 Is Nothing Then
                        .Clear()

                        tmpStrArr = Split(String.Join(";", _
                            IIf(FritzBoxJSONTelefone1.S0Name1 = DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, "1"), _
                            IIf(FritzBoxJSONTelefone1.S0Name2 = DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, "2"), _
                            IIf(FritzBoxJSONTelefone1.S0Name3 = DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, "3"), _
                            IIf(FritzBoxJSONTelefone1.S0Name4 = DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, "4"), _
                            IIf(FritzBoxJSONTelefone1.S0Name5 = DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, "5"), _
                            IIf(FritzBoxJSONTelefone1.S0Name6 = DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, "6"), _
                            IIf(FritzBoxJSONTelefone1.S0Name7 = DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, "7"), _
                            IIf(FritzBoxJSONTelefone1.S0Name8 = DataProvider.P_Def_LeerString, DataProvider.P_Def_LeerString, "8")), ";", , CompareMethod.Text)

                        For Each idx In C_hf.ClearStringArray(tmpStrArr)
                            TelQuery.Add(P_Query_FB_S0_TelNr(idx)) ' S0 Nr
                            TelQuery.Add(P_Query_FB_S0_Type(idx)) ' S0 Typ
                        Next


                        For i = LBound(FritzBoxJSONTelefone1.DECT) To UBound(FritzBoxJSONTelefone1.DECT)
                            If Not FritzBoxJSONTelefone1.DECT(i).Intern = DataProvider.P_Def_Leerzeichen Then
                                .Add(P_Query_FB_DECT_RingOnAllMSNs(i))
                                .Add(P_Query_FB_DECT_NrList(i))
                            End If
                        Next

                        .Add(P_Query_FB_FaxMailActive)
                        .Add(P_Query_FB_MobileName)
                        FritzBoxJSONTelefone2 = GetFBJSON4(TelQuery)
                    End If
                End If
            End If
        End With

        ' Telefone 
        If Not FritzBoxJSONTelNr1 Is Nothing And Not _
               FritzBoxJSONTelNr2 Is Nothing And Not _
               FritzBoxJSONTelefone1 Is Nothing And Not _
               FritzBoxJSONTelefone2 Is Nothing Then

            xPathTeile.Item(xPathTeile.IndexOf("Nummern")) = "Telefone"
            xPathTeile.Item(xPathTeile.IndexOf("Mobil")) = "FON"

            For i = LBound(FritzBoxJSONTelefone1.FON) To UBound(FritzBoxJSONTelefone1.FON)
                With CType(FritzBoxJSONTelefone1.FON(i), MSNEntry)
                    If Not .Name = DataProvider.P_Def_LeerString Then
                        TelNr = DataProvider.P_Def_LeerString
                        DialPort = CStr(i + 1)
                        ReDim tmpStrArr(9)
                        With FritzBoxJSONTelNr2

                            Select Case i
                                Case 0
                                    tmpStrArr = Split(String.Join(";", _
                                        .MSN0Nr0,
                                        .MSN0Nr1,
                                        .MSN0Nr2,
                                        .MSN0Nr3,
                                        .MSN0Nr4,
                                        .MSN0Nr5,
                                        .MSN0Nr6,
                                        .MSN0Nr7,
                                        .MSN0Nr8,
                                        .MSN0Nr9) _
                                        , ";", , CompareMethod.Text)
                                Case 1
                                    tmpStrArr = Split(String.Join(";", _
                                        .MSN1Nr0,
                                        .MSN1Nr1,
                                        .MSN1Nr2,
                                        .MSN1Nr3,
                                        .MSN1Nr4,
                                        .MSN1Nr5,
                                        .MSN1Nr6,
                                        .MSN1Nr7,
                                        .MSN1Nr8,
                                        .MSN1Nr9) _
                                        , ";", , CompareMethod.Text)
                                Case 2
                                    tmpStrArr = Split(String.Join(";", _
                                        .MSN2Nr0,
                                        .MSN2Nr1,
                                        .MSN2Nr2,
                                        .MSN2Nr3,
                                        .MSN2Nr4,
                                        .MSN2Nr5,
                                        .MSN2Nr6,
                                        .MSN2Nr7,
                                        .MSN2Nr8,
                                        .MSN2Nr9) _
                                        , ";", , CompareMethod.Text)
                            End Select
                        End With

                        tmpStrArr = C_hf.ClearStringArray(tmpStrArr)

                        If tmpStrArr.Length = 0 Then tmpStrArr = MSN

                        For j = LBound(tmpStrArr) To UBound(tmpStrArr)
                            If tmpStrArr(j).StartsWith("SIP") Then
                                tmpStrArr(j) = SIP(CInt(Mid(tmpStrArr(j), 4, 1)))
                            Else
                                tmpStrArr(j) = C_hf.EigeneVorwahlenEntfernen(tmpStrArr(j))
                            End If
                        Next

                        TelNr = String.Join(";", tmpStrArr)
                        PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("FON", DialPort, TelNr, .Name))
                        If P_SpeichereDaten Then
                            NodeValues.Item(NodeNames.IndexOf("TelName")) = .Name
                            NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                            AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                            AttributeValues.Item(AttributeNames.IndexOf("Fax")) = .Fax
                            C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                        End If
                    End If
                End With
            Next

            ' DECT
            xPathTeile.Item(xPathTeile.IndexOf("FON")) = "DECT"
            For i = LBound(FritzBoxJSONTelefone1.DECT) To UBound(FritzBoxJSONTelefone1.DECT)
                With FritzBoxJSONTelefone1.DECT(i)

                    If Not .Name = DataProvider.P_Def_LeerString Then
                        TelNr = DataProvider.P_Def_LeerString
                        DialPort = "6" & Strings.Right(.Intern, 1)
                        TelName = .Name

                        Dim tmpDectNr() As DECTNr = Nothing

                        Select Case i
                            Case 0
                                If FritzBoxJSONTelefone2.DECT0RingOnAllMSNs = "1" Then
                                    TelNr = allin
                                Else
                                    tmpDectNr = FritzBoxJSONTelefone2.DECT0Nr
                                End If
                            Case 1
                                If FritzBoxJSONTelefone2.DECT1RingOnAllMSNs = "1" Then
                                    TelNr = allin
                                Else
                                    tmpDectNr = FritzBoxJSONTelefone2.DECT1Nr
                                End If
                            Case 2
                                If FritzBoxJSONTelefone2.DECT2RingOnAllMSNs = "1" Then
                                    TelNr = allin
                                Else
                                    tmpDectNr = FritzBoxJSONTelefone2.DECT2Nr
                                End If
                            Case 3
                                If FritzBoxJSONTelefone2.DECT3RingOnAllMSNs = "1" Then
                                    TelNr = allin
                                Else
                                    tmpDectNr = FritzBoxJSONTelefone2.DECT3Nr
                                End If
                            Case 4
                                If FritzBoxJSONTelefone2.DECT4RingOnAllMSNs = "1" Then
                                    TelNr = allin
                                Else
                                    tmpDectNr = FritzBoxJSONTelefone2.DECT4Nr
                                End If
                        End Select

                        If Not TelNr = allin Then
                            For Each DECTNr As DECTNr In tmpDectNr
                                If Not DECTNr.Number = DataProvider.P_Def_LeerString Then
                                    If TelNr = DataProvider.P_Def_LeerString Then
                                        TelNr = C_hf.EigeneVorwahlenEntfernen(DECTNr.Number)
                                    Else
                                        TelNr = String.Join(";", TelNr, C_hf.EigeneVorwahlenEntfernen(DECTNr.Number))
                                    End If
                                End If
                            Next
                        End If

                        PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("DECT", DialPort, TelNr, TelName))
                        If P_SpeichereDaten Then
                            NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                            NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                            AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                            AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull

                            C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                        End If

                    End If
                End With
            Next

            xPathTeile.Item(xPathTeile.IndexOf("DECT")) = "VOIP"
            'IP-Telefone
            For i = LBound(FritzBoxJSONTelefone1.VOIP) To UBound(FritzBoxJSONTelefone1.VOIP)
                With FritzBoxJSONTelefone1.VOIP(i)
                    If .enabled = "1" Then
                        TelName = .Name
                        DialPort = "2" & CStr(i)
                        TelNr = DataProvider.P_Def_LeerString

                        With FritzBoxJSONTelNr2
                            Select Case i
                                Case 0
                                    TelNr += GetFBVoipNr(.VOIP0Nr0, SIP)
                                    TelNr += GetFBVoipNr(.VOIP0Nr1, SIP)
                                    TelNr += GetFBVoipNr(.VOIP0Nr2, SIP)
                                    TelNr += GetFBVoipNr(.VOIP0Nr3, SIP)
                                    TelNr += GetFBVoipNr(.VOIP0Nr4, SIP)
                                    TelNr += GetFBVoipNr(.VOIP0Nr5, SIP)
                                    TelNr += GetFBVoipNr(.VOIP0Nr6, SIP)
                                    TelNr += GetFBVoipNr(.VOIP0Nr7, SIP)
                                    TelNr += GetFBVoipNr(.VOIP0Nr8, SIP)
                                    TelNr += GetFBVoipNr(.VOIP0Nr9, SIP)
                                Case 1
                                    TelNr += GetFBVoipNr(.VOIP1Nr0, SIP)
                                    TelNr += GetFBVoipNr(.VOIP1Nr1, SIP)
                                    TelNr += GetFBVoipNr(.VOIP1Nr2, SIP)
                                    TelNr += GetFBVoipNr(.VOIP1Nr3, SIP)
                                    TelNr += GetFBVoipNr(.VOIP1Nr4, SIP)
                                    TelNr += GetFBVoipNr(.VOIP1Nr5, SIP)
                                    TelNr += GetFBVoipNr(.VOIP1Nr6, SIP)
                                    TelNr += GetFBVoipNr(.VOIP1Nr7, SIP)
                                    TelNr += GetFBVoipNr(.VOIP1Nr8, SIP)
                                    TelNr += GetFBVoipNr(.VOIP1Nr9, SIP)
                                Case 2
                                    TelNr += GetFBVoipNr(.VOIP2Nr0, SIP)
                                    TelNr += GetFBVoipNr(.VOIP2Nr1, SIP)
                                    TelNr += GetFBVoipNr(.VOIP2Nr2, SIP)
                                    TelNr += GetFBVoipNr(.VOIP2Nr3, SIP)
                                    TelNr += GetFBVoipNr(.VOIP2Nr4, SIP)
                                    TelNr += GetFBVoipNr(.VOIP2Nr5, SIP)
                                    TelNr += GetFBVoipNr(.VOIP2Nr6, SIP)
                                    TelNr += GetFBVoipNr(.VOIP2Nr7, SIP)
                                    TelNr += GetFBVoipNr(.VOIP2Nr8, SIP)
                                    TelNr += GetFBVoipNr(.VOIP2Nr9, SIP)
                                Case 3
                                    TelNr += GetFBVoipNr(.VOIP3Nr0, SIP)
                                    TelNr += GetFBVoipNr(.VOIP3Nr1, SIP)
                                    TelNr += GetFBVoipNr(.VOIP3Nr2, SIP)
                                    TelNr += GetFBVoipNr(.VOIP3Nr3, SIP)
                                    TelNr += GetFBVoipNr(.VOIP3Nr4, SIP)
                                    TelNr += GetFBVoipNr(.VOIP3Nr5, SIP)
                                    TelNr += GetFBVoipNr(.VOIP3Nr6, SIP)
                                    TelNr += GetFBVoipNr(.VOIP3Nr7, SIP)
                                    TelNr += GetFBVoipNr(.VOIP3Nr8, SIP)
                                    TelNr += GetFBVoipNr(.VOIP3Nr9, SIP)
                                Case 4
                                    TelNr += GetFBVoipNr(.VOIP4Nr0, SIP)
                                    TelNr += GetFBVoipNr(.VOIP4Nr1, SIP)
                                    TelNr += GetFBVoipNr(.VOIP4Nr2, SIP)
                                    TelNr += GetFBVoipNr(.VOIP4Nr3, SIP)
                                    TelNr += GetFBVoipNr(.VOIP4Nr4, SIP)
                                    TelNr += GetFBVoipNr(.VOIP4Nr5, SIP)
                                    TelNr += GetFBVoipNr(.VOIP4Nr6, SIP)
                                    TelNr += GetFBVoipNr(.VOIP4Nr7, SIP)
                                    TelNr += GetFBVoipNr(.VOIP4Nr8, SIP)
                                    TelNr += GetFBVoipNr(.VOIP4Nr9, SIP)
                                Case 5
                                    TelNr += GetFBVoipNr(.VOIP5Nr0, SIP)
                                    TelNr += GetFBVoipNr(.VOIP5Nr1, SIP)
                                    TelNr += GetFBVoipNr(.VOIP5Nr2, SIP)
                                    TelNr += GetFBVoipNr(.VOIP5Nr3, SIP)
                                    TelNr += GetFBVoipNr(.VOIP5Nr4, SIP)
                                    TelNr += GetFBVoipNr(.VOIP5Nr5, SIP)
                                    TelNr += GetFBVoipNr(.VOIP5Nr6, SIP)
                                    TelNr += GetFBVoipNr(.VOIP5Nr7, SIP)
                                    TelNr += GetFBVoipNr(.VOIP5Nr8, SIP)
                                    TelNr += GetFBVoipNr(.VOIP5Nr9, SIP)
                                Case 6
                                    TelNr += GetFBVoipNr(.VOIP6Nr0, SIP)
                                    TelNr += GetFBVoipNr(.VOIP6Nr1, SIP)
                                    TelNr += GetFBVoipNr(.VOIP6Nr2, SIP)
                                    TelNr += GetFBVoipNr(.VOIP6Nr3, SIP)
                                    TelNr += GetFBVoipNr(.VOIP6Nr4, SIP)
                                    TelNr += GetFBVoipNr(.VOIP6Nr5, SIP)
                                    TelNr += GetFBVoipNr(.VOIP6Nr6, SIP)
                                    TelNr += GetFBVoipNr(.VOIP6Nr7, SIP)
                                    TelNr += GetFBVoipNr(.VOIP6Nr8, SIP)
                                    TelNr += GetFBVoipNr(.VOIP6Nr9, SIP)
                                Case 7
                                    TelNr += GetFBVoipNr(.VOIP7Nr0, SIP)
                                    TelNr += GetFBVoipNr(.VOIP7Nr1, SIP)
                                    TelNr += GetFBVoipNr(.VOIP7Nr2, SIP)
                                    TelNr += GetFBVoipNr(.VOIP7Nr3, SIP)
                                    TelNr += GetFBVoipNr(.VOIP7Nr4, SIP)
                                    TelNr += GetFBVoipNr(.VOIP7Nr5, SIP)
                                    TelNr += GetFBVoipNr(.VOIP7Nr6, SIP)
                                    TelNr += GetFBVoipNr(.VOIP7Nr7, SIP)
                                    TelNr += GetFBVoipNr(.VOIP7Nr8, SIP)
                                    TelNr += GetFBVoipNr(.VOIP7Nr9, SIP)
                                Case 8
                                    TelNr += GetFBVoipNr(.VOIP8Nr0, SIP)
                                    TelNr += GetFBVoipNr(.VOIP8Nr1, SIP)
                                    TelNr += GetFBVoipNr(.VOIP8Nr2, SIP)
                                    TelNr += GetFBVoipNr(.VOIP8Nr3, SIP)
                                    TelNr += GetFBVoipNr(.VOIP8Nr4, SIP)
                                    TelNr += GetFBVoipNr(.VOIP8Nr5, SIP)
                                    TelNr += GetFBVoipNr(.VOIP8Nr6, SIP)
                                    TelNr += GetFBVoipNr(.VOIP8Nr7, SIP)
                                    TelNr += GetFBVoipNr(.VOIP8Nr8, SIP)
                                    TelNr += GetFBVoipNr(.VOIP8Nr9, SIP)
                                Case 9
                                    TelNr += GetFBVoipNr(.VOIP9Nr0, SIP)
                                    TelNr += GetFBVoipNr(.VOIP9Nr1, SIP)
                                    TelNr += GetFBVoipNr(.VOIP9Nr2, SIP)
                                    TelNr += GetFBVoipNr(.VOIP9Nr3, SIP)
                                    TelNr += GetFBVoipNr(.VOIP9Nr4, SIP)
                                    TelNr += GetFBVoipNr(.VOIP9Nr5, SIP)
                                    TelNr += GetFBVoipNr(.VOIP9Nr6, SIP)
                                    TelNr += GetFBVoipNr(.VOIP9Nr7, SIP)
                                    TelNr += GetFBVoipNr(.VOIP9Nr8, SIP)
                                    TelNr += GetFBVoipNr(.VOIP9Nr9, SIP)
                            End Select

                            If Not TelNr = DataProvider.P_Def_LeerString Then TelNr = Strings.Left(TelNr, Len(TelNr) - 1)

                        End With
                        PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("VOIP", DialPort, TelNr, TelName))

                        If P_SpeichereDaten Then
                            NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                            NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                            AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                            AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull
                            DialPort = "2" & i
                            C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                        End If
                    End If
                End With
            Next

            xPathTeile.Item(xPathTeile.IndexOf("VOIP")) = "S0"
            ' S0
            For i = 1 To 8
                Select Case i
                    Case 1
                        TelName = FritzBoxJSONTelefone1.S0Name1
                        TelNr = FritzBoxJSONTelefone2.S0TelNr1
                        tmpStr = FritzBoxJSONTelefone2.S0Type1
                    Case 2
                        TelName = FritzBoxJSONTelefone1.S0Name2
                        TelNr = FritzBoxJSONTelefone2.S0TelNr2
                        tmpStr = FritzBoxJSONTelefone2.S0Type2
                    Case 3
                        TelName = FritzBoxJSONTelefone1.S0Name3
                        TelNr = FritzBoxJSONTelefone2.S0TelNr3
                        tmpStr = FritzBoxJSONTelefone2.S0Type3
                    Case 4
                        TelName = FritzBoxJSONTelefone1.S0Name4
                        TelNr = FritzBoxJSONTelefone2.S0TelNr4
                        tmpStr = FritzBoxJSONTelefone2.S0Type4
                    Case 5
                        TelName = FritzBoxJSONTelefone1.S0Name5
                        TelNr = FritzBoxJSONTelefone2.S0TelNr5
                        tmpStr = FritzBoxJSONTelefone2.S0Type5
                    Case 6
                        TelName = FritzBoxJSONTelefone1.S0Name6
                        TelNr = FritzBoxJSONTelefone2.S0TelNr6
                        tmpStr = FritzBoxJSONTelefone2.S0Type6
                    Case 7
                        TelName = FritzBoxJSONTelefone1.S0Name7
                        TelNr = FritzBoxJSONTelefone2.S0TelNr7
                        tmpStr = FritzBoxJSONTelefone2.S0Type7
                    Case 8
                        TelName = FritzBoxJSONTelefone1.S0Name8
                        TelNr = FritzBoxJSONTelefone2.S0TelNr8
                        tmpStr = FritzBoxJSONTelefone2.S0Type8
                    Case Else
                        TelName = DataProvider.P_Def_LeerString
                        tmpStr = DataProvider.P_Def_LeerString
                        TelNr = DataProvider.P_Def_LeerString
                End Select
                If Not TelName = DataProvider.P_Def_LeerString And Not TelNr = DataProvider.P_Def_LeerString Then
                    DialPort = "5" & i
                    PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("S0-", DialPort, TelNr, TelName))
                    If P_SpeichereDaten Then
                        NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                        NodeValues.Item(NodeNames.IndexOf("TelNr")) = C_hf.EigeneVorwahlenEntfernen(TelNr)
                        AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                        AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_hf.IIf(tmpStr = "Fax", 1, 0)

                        C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                    End If
                End If

            Next
            If Not DialPort = DataProvider.P_Def_LeerString Then
                If CDbl(DialPort) > 50 And CDbl(DialPort) < 60 Then
                    DialPort = "50"
                    PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("ISDN-Basis", DialPort, DataProvider.P_Def_LeerString, "ISDN-Basis"))
                    If P_SpeichereDaten Then
                        NodeValues.Item(NodeNames.IndexOf("TelName")) = "ISDN-Basis"
                        NodeValues.Item(NodeNames.IndexOf("TelNr")) = "50"
                        AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                        AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull
                        C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                    End If
                End If
            End If

            xPathTeile.Item(xPathTeile.IndexOf("S0")) = "TAM"
            ' TAM, Anrufbeantworter
            For i = LBound(FritzBoxJSONTelefone1.TAM) To UBound(FritzBoxJSONTelefone1.TAM)
                With FritzBoxJSONTelefone1.TAM(i)
                    If .Active = "1" Then
                        TelName = .Name

                        If TAM.Count = 0 Then
                            TelNr = allin
                        Else
                            TelNr = C_hf.EigeneVorwahlenEntfernen(TAM(i))
                        End If

                        DialPort = "60" & i
                        PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("TAM", DialPort, TelNr, TelName))
                        If P_SpeichereDaten Then
                            NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                            NodeValues.Item(NodeNames.IndexOf("TelNr")) = C_hf.IIf(TelNr = DataProvider.P_Def_LeerString, allin, TelNr)
                            AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                            AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull
                            C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                        End If
                    End If
                End With
            Next

            xPathTeile.Item(xPathTeile.IndexOf("TAM")) = "FAX"
            ' integrierter Faxempfang
            If FritzBoxJSONTelefone2.FaxMailActive IsNot DataProvider.P_Def_StringNull Then
                TelNr = Join(FAX, ";")
                DialPort = "5"
                TelName = "Faxempfang"
                PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("Integrierte Faxfunktion", DialPort, TelNr, TelName))
                If P_SpeichereDaten Then
                    NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = "1"

                    C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                End If
            End If

            ' Mobiltelefon
            xPathTeile.Item(xPathTeile.IndexOf("FAX")) = "Mobil"
            If Mobil IsNot DataProvider.P_Def_LeerString Then
                TelName = FritzBoxJSONTelefone2.MobileName
                DialPort = DataProvider.P_Def_MobilDialPort
                PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound("Mobil", DialPort, Mobil, TelName))
                If P_SpeichereDaten Then
                    NodeValues.Item(NodeNames.IndexOf("TelName")) = C_hf.IIf(TelName = DataProvider.P_Def_ErrorMinusOne_String Or TelName = DataProvider.P_Def_LeerString, Mobil, TelName)
                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = Mobil
                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = DataProvider.P_Def_StringNull

                    C_XML.AppendNode(C_DP.XMLDoc, xPathTeile, C_XML.CreateXMLNode(C_DP.XMLDoc, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                End If
            End If
        End If
        ' Logout

        FBLogout(P_SID)

        ' Aufräumen
        tmpStrArr = Nothing
        FritzBoxJSONTelNr1 = Nothing
        FritzBoxJSONTelNr2 = Nothing
        FritzBoxJSONTelefone1 = Nothing
        FritzBoxJSONTelefone2 = Nothing
        xPathTeile.Clear()
        NodeNames.Clear()
        NodeValues.Clear()
        AttributeNames.Clear()
        AttributeValues.Clear()
        xPathTeile = Nothing
        NodeNames = Nothing
        NodeValues = Nothing
        AttributeNames = Nothing
        AttributeValues = Nothing
    End Sub

    ''' <summary>
    ''' Wandelt die in Fritz!BoxDatenV3 gefundenen VOIP-Nr in eine gültige Telefonnummer ohne Vorwahlen um.
    ''' </summary>
    ''' <param name="VOIPNr">Die umzuwandelnde Telefonnummer</param>
    ''' <param name="SIPList">Die SIP-Liste</param>
    ''' <returns>EIne gültige Telefonnummer ohne Vorwahlen.</returns>
    Private Function GetFBVoipNr(ByVal VOIPNr As String, ByVal SIPList() As String) As String
        GetFBVoipNr = DataProvider.P_Def_LeerString

        If Not VOIPNr = DataProvider.P_Def_LeerString Then
            If VOIPNr.StartsWith("SIP") Then
                If Not CInt(Mid(VOIPNr, 4, 1)) > UBound(SIPList) Then
                    GetFBVoipNr += SIPList(CInt(Mid(VOIPNr, 4, 1)))
                Else
                    C_hf.LogFile("Die VoIP Nr. " & VOIPNr & " kann keiner SIP zugeordnet werden (UBound(SIPList) = " & UBound(SIPList) & ")")
                End If
            Else
                GetFBVoipNr += C_hf.EigeneVorwahlenEntfernen(VOIPNr)
            End If
            GetFBVoipNr += ";"
        End If

    End Function

    ''' <summary>
    ''' Führt das Fritz!Box Query aus und gibt die Daten verwendbar zurück
    ''' </summary>
    ''' <param name="QueryList">Die Liste der auszuführenden Querys</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetFBJSON1(ByVal QueryList As ArrayList) As FritzBoxJSONTelNrT1
        Dim sQuery() As String
        Dim C_JSON As New JSON

        ReDim sQuery(UBound(QueryList.ToArray))
        For i = LBound(sQuery) To UBound(sQuery)
            sQuery(i) = QueryList.Item(i).ToString
        Next

        GetFBJSON1 = C_JSON.GetFirstValues(FritzBoxQuery(String.Join("&", sQuery)))
    End Function

    Private Function GetFBJSON2(ByVal QueryList As ArrayList) As FritzBoxJSONTelNrT2
        Dim sQuery() As String
        Dim C_JSON As New JSON

        ReDim sQuery(UBound(QueryList.ToArray))
        For i = LBound(sQuery) To UBound(sQuery)
            sQuery(i) = QueryList.Item(i).ToString
        Next

        GetFBJSON2 = C_JSON.GetSecondValues(FritzBoxQuery(String.Join("&", sQuery)))
    End Function

    Private Function GetFBJSON3(ByVal QueryList As ArrayList) As FritzBoxJSONTelefone1
        Dim sQuery() As String
        Dim C_JSON As New JSON

        ReDim sQuery(UBound(QueryList.ToArray))
        For i = LBound(sQuery) To UBound(sQuery)
            sQuery(i) = QueryList.Item(i).ToString
        Next

        GetFBJSON3 = C_JSON.GetThirdValues(FritzBoxQuery(String.Join("&", sQuery)))
    End Function

    Private Function GetFBJSON4(ByVal QueryList As ArrayList) As FritzBoxJSONTelefone2
        Dim sQuery() As String
        Dim C_JSON As New JSON

        ReDim sQuery(UBound(QueryList.ToArray))
        For i = LBound(sQuery) To UBound(sQuery)
            sQuery(i) = QueryList.Item(i).ToString
        Next

        GetFBJSON4 = C_JSON.GetForthValues(FritzBoxQuery(String.Join("&", sQuery)))
    End Function

    Private Function GetFBJSON5(ByVal QueryList As ArrayList) As FritzBoxJSONTelefoneFONNr
        Dim sQuery() As String
        Dim C_JSON As New JSON

        ReDim sQuery(UBound(QueryList.ToArray))
        For i = LBound(sQuery) To UBound(sQuery)
            sQuery(i) = QueryList.Item(i).ToString
        Next

        GetFBJSON5 = C_JSON.GetFifthValues(FritzBoxQuery(String.Join("&", sQuery)))
    End Function

    Private Overloads Function AlleNummern(ByVal MSN() As String, ByVal SIP() As String, ByVal TAM() As String, ByVal FAX() As String, ByVal POTS As String, ByVal Mobil As String) As String
        AlleNummern = DataProvider.P_Def_LeerString
        Dim tmp() As String = Split(Strings.Join(MSN, ";") & ";" & _
                                    Strings.Join(SIP, ";") & ";" & _
                                    Strings.Join(TAM, ";") & ";" & _
                                    Strings.Join(FAX, ";") & ";" & _
                                    POTS & ";" & Mobil, ";", , CompareMethod.Text)

        tmp = C_hf.ClearStringArray(tmp)
        AlleNummern = Join(tmp, ";")
    End Function

    Private Overloads Function AlleNummern(ByVal MSN() As String, ByVal SIP() As String, ByVal TAM() As String, ByVal POTS As String, ByVal Mobil As String) As String
        Dim FAX As String() = Nothing
        Return AlleNummern(MSN, SIP, TAM, FAX, POTS, Mobil)
    End Function
#End Region

#Region "Wählen"
    Friend Function SendDialRequestToBox(ByVal sDialCode As String, ByVal sDialPort As String, bHangUp As Boolean) As String
        If C_DP.P_RBFBComUPnP Then
            Return SendDialRequestToBoxV3(sDialCode, sDialPort, bHangUp)
        Else
            If Not ThisFBFirmware.ISLargerOREqual("6.00") Then
                Return SendDialRequestToBoxV1(sDialCode, sDialPort, bHangUp)
            Else
                Return SendDialRequestToBoxV2(sDialCode, sDialPort, bHangUp)
            End If
        End If
    End Function

    Private Function SendDialRequestToBoxV1(ByVal sDialCode As String, ByVal sDialPort As String, bHangUp As Boolean) As String
        ' überträgt die zum Verbindungsaufbau notwendigen Daten per WinHttp an die FritzBox
        ' Parameter:  dialCode (string):    zu wählende Nummer
        '             fonanschluss (long):  Welcher Anschluss wird verwendet?
        '             HangUp (bool):        Soll Verbindung abgebrochen werden
        ' Rückgabewert (String):            Antworttext (Status)
        '
        Dim Response As String             ' Antwort der FritzBox
        '
        SendDialRequestToBoxV1 = DataProvider.P_FritzBox_Dial_Error1           ' Antwortstring
        If Not P_SID = DataProvider.P_Def_SessionID And Len(P_SID) = Len(DataProvider.P_Def_SessionID) Then
            Response = C_hf.httpPOST(P_Link_FB_ExtBasis, P_Link_FB_DialV1(P_SID, sDialPort, sDialCode, bHangUp), FBEncoding)

            If Response = DataProvider.P_Def_LeerString Then
                SendDialRequestToBoxV1 = C_hf.IIf(bHangUp, DataProvider.P_FritzBox_Dial_HangUp, DataProvider.P_FritzBox_Dial_Start(sDialCode))
            Else
                SendDialRequestToBoxV1 = DataProvider.P_FritzBox_Dial_Error2
                C_hf.LogFile("SendDialRequestToBoxV1: Response: " & Response)
            End If
        Else
            C_hf.MsgBox(DataProvider.P_FritzBox_Dial_Error3(P_SID), MsgBoxStyle.Critical, "sendDialRequestToBox")
        End If
    End Function

    Private Function SendDialRequestToBoxV2(ByVal sDialCode As String, ByVal sDialPort As String, bHangUp As Boolean) As String
        Dim Response As String              ' Antwort der FritzBox
        Dim PortChangeSuccess As Boolean
        Dim DialCodetoBox As String

        SendDialRequestToBoxV2 = DataProvider.P_FritzBox_Dial_Error1
        ' DialPort setzen, wenn erforderlich
        If FritzBoxQuery("DialPort=telcfg:settings/DialPort").Contains(sDialPort) Then
            PortChangeSuccess = True
        Else
            C_hf.LogFile("SendDialRequestToBoxV2: Ändere Dialport auf " & sDialPort)
            ' per HTTP-POST Dialport ändern
            Response = C_hf.httpPOST(P_Link_FB_TelV2, P_Link_FB_DialV2SetDialPort(P_SID, sDialPort), FBEncoding)
            PortChangeSuccess = Response.Contains("[""telcfg:settings/DialPort""] = """ & sDialPort & "")
        End If

        ' Wählen
        If PortChangeSuccess Then
            DialCodetoBox = sDialCode

            ' Tipp von Pikachu: Umwandlung von # und *, da ansonsten die Telefoncodes verschluckt werden. 
            ' Alternativ ein URLEncode (Uri.EscapeDataString(Link).Replace("%20", "+")), 
            ' was aber in der Funktion httpGET zu einem Fehler bei dem Erstellen der neuen URI führt.
            DialCodetoBox = Replace(DialCodetoBox, "#", "%23", , , CompareMethod.Text)
            DialCodetoBox = Replace(DialCodetoBox, "*", "%2A", , , CompareMethod.Text)

            ' Senden des Wählkomandos
            Response = C_hf.httpGET(P_Link_FB_DialV2(P_SID, DialCodetoBox, bHangUp), FBEncoding, FBFehler)
            ' Die Rückgabe ist der JSON - Wert "dialing"
            ' Bei der Wahl von Telefonnummern ist es ein {"dialing": "0123456789#"}
            ' Bei der Wahl von Telefoncodes ist es ein {"dialing": "#96*0*"}
            ' Bei der Wahl Des Hangup ist es ein {"dialing": false} ohne die umschließenden Anführungszeichen" 
            If Response.Contains("""dialing"": " & C_hf.IIf(bHangUp, "false", """" & sDialCode & """")) Then
                SendDialRequestToBoxV2 = C_hf.IIf(bHangUp, DataProvider.P_FritzBox_Dial_HangUp, DataProvider.P_FritzBox_Dial_Start(sDialCode))
            Else
                C_hf.LogFile("SendDialRequestToBoxV2: Response: " & Response.Replace(vbLf, ""))
            End If
        End If
    End Function

    Private Function SendDialRequestToBoxV3(ByVal sDialCode As String, ByVal sDialPort As String, bHangUp As Boolean) As String
        Dim PortChangeSuccess As Boolean
        Dim DialCodetoBox As String
        Dim UPnPDialport As String
        Dim InPutData As New Hashtable
        Dim OutPutData As New Hashtable
        Dim xPathTeile As New ArrayList

        SendDialRequestToBoxV3 = DataProvider.P_FritzBox_Dial_Error1

        With xPathTeile
            .Add("Telefone")
            .Add("Telefone")
            .Add("*")
            .Add("Telefon")
            .Add("[@Dialport = """ & sDialPort & """]")
            .Add("TelName")
            UPnPDialport = C_XML.Read(C_DP.XMLDoc, xPathTeile, DataProvider.P_Def_ErrorMinusOne_String)
        End With

        Select Case CInt(sDialPort)
            'Case 1 To 3
            '    UPnpDialport = "FON: " & UPnpDialport
            Case 50
                UPnPDialport = "ISDN und Schnurlostelefone"
            Case 51 To 58
                UPnPDialport = "ISDN: " & UPnPDialport
            Case 60 To 69
                UPnPDialport = "DECT: " & UPnPDialport
        End Select

        ' DialPort setzen, wenn erforderlich
        OutPutData = C_FBoxUPnP.Start(FritzBoxInformations.KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialGetConfig")

        If OutPutData.Item("NewX_AVM-DE_PhoneName").ToString = UPnPDialport Then
            PortChangeSuccess = True
        Else
            C_hf.LogFile("SendDialRequestToBoxV3: Ändere Dialport auf " & UPnPDialport)
            InPutData.Add("NewX_AVM-DE_PhoneName", UPnPDialport)
            OutPutData = C_FBoxUPnP.Start(FritzBoxInformations.KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialSetConfig", InPutData)
            If OutPutData.Contains("Error") Then
                C_hf.LogFile(OutPutData("Error").ToString.Replace("CHR(60)", "<").Replace("CHR(62)", ">"))
                PortChangeSuccess = False
            End If
        End If

        ' Wählen
        If PortChangeSuccess Then
            DialCodetoBox = sDialCode

            ' Senden des Wählkomandos
            InPutData.Clear()
            If bHangUp Then
                OutPutData = C_FBoxUPnP.Start(FritzBoxInformations.KnownSOAPFile.x_voipSCPD, "X_AVM-DE_Hangup")
            Else
                InPutData.Add("NewX_AVM-DE_PhoneNumber", DialCodetoBox)
                OutPutData = C_FBoxUPnP.Start(FritzBoxInformations.KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialNumber", InPutData)
            End If
            If OutPutData.Contains("Error") Then
                C_hf.LogFile(OutPutData("Error").ToString.Replace("CHR(60)", "<").Replace("CHR(62)", ">"))
            Else
                SendDialRequestToBoxV3 = C_hf.IIf(bHangUp, DataProvider.P_FritzBox_Dial_HangUp, DataProvider.P_FritzBox_Dial_Start(sDialCode))
            End If

        End If

        xPathTeile.Clear()
        InPutData.Clear()
        OutPutData.Clear()
        xPathTeile = Nothing
        InPutData = Nothing
        OutPutData = Nothing
    End Function
#End Region

#Region "Journalimort"

    Public Function DownloadAnrListeV1() As String
        Dim sLink As String
        Dim ReturnString As String = DataProvider.P_Def_LeerString

        If P_SID = DataProvider.P_Def_SessionID Then
            If Not FBLogin() = DataProvider.P_Def_SessionID Then
                ReturnString = DownloadAnrListeV1()
            Else
                C_hf.LogFile("DownloadAnrListe: " & DataProvider.P_FritzBox_JI_Error1)
            End If
        Else
            sLink = P_Link_JI2(P_SID)

            ReturnString = C_hf.httpGET(P_Link_JI1(P_SID), FBEncoding, FBFehler)
            If Not FBFehler Then
                If Not InStr(ReturnString, "Luacgi not readable", CompareMethod.Text) = 0 Then
                    C_hf.httpGET(P_Link_JIAlt_Child1(P_SID), FBEncoding, FBFehler)
                    sLink = P_Link_JIAlt_Child2(P_SID)
                End If
                ReturnString = C_hf.httpGET(sLink, FBEncoding, FBFehler)
            Else
                C_hf.LogFile("FBError (DownloadAnrListe): " & Err.Number & " - " & Err.Description & " - " & sLink)
            End If
        End If
        Return ReturnString
    End Function

    ''' <summary>
    ''' Nutzt den Link http://fritz.box:49000/calllist.lua um die aktuelle Anrufliste zu erhalten.
    ''' Durch den Aufruf per UPnP/SOAP ist automatisch eine SID vorhanden.
    ''' Der Link kann wie folgt erweitert werden (Auszug aus Schnittstellenbeschreibung):
    ''' <list type="table">
    ''' <listheader>
    ''' <term>term</term>
    '''    <description>description</description>
    ''' </listheader>
    ''' <item>
    '''    <term>&amp;days=NNN</term>
    '''    <description>number number of days to look back for calls e.g. 1: calls from today and yesterday, 7: calls from the complete last week, default 999</description>
    ''' </item>
    ''' <item>
    '''    <term>&amp;id=i</term>
    '''    <description>number calls since this unique ID</description>
    ''' </item>
    ''' <item>
    '''    <term>&amp;max=n</term>
    '''    <description>number maximum number of entries in call list, default 999</description>
    ''' </item>
    ''' <item>
    '''    <term>&amp;timestamp=sss</term>
    '''    <description>number value from timestamp tag, to get only entries that are newer (timestamp is resetted by a factory reset)</description>
    ''' </item>
    ''' <item>
    '''    <term>&amp;type=t</term>
    '''    <description>string optional parameter for type of output file: xml (default) or csv</description>
    ''' </item>
    ''' </list>
    ''' </summary>
    ''' <returns>Die Anrufliste im XML-Format.</returns>
    ''' <remarks>http://avm.de/fileadmin/user_upload/Global/Service/Schnittstellen/x_contactSCPD.pdf</remarks>
    Public Function DownloadAnrListeV2() As XmlDocument
        Dim OutPutData As New Hashtable
        OutPutData = C_FBoxUPnP.Start(FritzBoxInformations.KnownSOAPFile.x_contactSCPD, "GetCallList")

        DownloadAnrListeV2 = New XmlDocument
        ' Funktioniert noch nicht
        DownloadAnrListeV2.Load(OutPutData.Item("NewCallListURL").ToString)
    End Function

#End Region

#Region "Information"

    Public Function GetInformationSystemFritzBox() As String

        Dim FBTyp As String = DataProvider.P_Def_StringUnknown
        Dim FBFirmware As String = DataProvider.P_Def_StringUnknown
        Dim FritzBoxInformation() As String

        FritzBoxInformation = Split(C_hf.StringEntnehmen(C_hf.httpGET(P_Link_FB_SystemStatus, System.Text.Encoding.UTF8, Nothing), "<body>", "</body>"), "-", , CompareMethod.Text)
        FBTyp = FritzBoxInformation(0)
        FBFirmware = Replace(Trim(C_hf.GruppiereNummer(FritzBoxInformation(7))), " ", ".", , , CompareMethod.Text)

        Return DataProvider.P_FritzBox_Info(FBTyp, FBFirmware)

    End Function

    ''' <summary>
    ''' Ermittlung der Firmware der Fritz!Box
    ''' </summary>
    ''' <returns>Fritz!Box Firmware-Version</returns>
    ''' <remarks>http://fritz.box/jason_boxinfo.xml</remarks>
    Private Function FBFirmware() As Boolean
        Dim Response As String
        Dim tmp() As String
        Dim InfoXML As XmlDocument
        Dim tmpFBFW As New FritzBoxFirmware

        ' Login Lua 5.29 ab Firmware xxx.05.29 / xxx.05.5x 
        ' Login Xml 5.28 ab Firmware xxx.04.74 - xxx.05.28 
        ' 6.25

        Response = C_hf.httpGET(P_Link_Jason_Boxinfo, FBEncoding, FBFehler)
        ' To Do Fehler Abfangen
        If Not FBFehler Then
            ' Ab der Firmware an 4.82 gibt es die Fritz!BoxInformation am 

            '<j:BoxInfo xmlns:j="http://jason.avm.de/updatecheck/">
            '    <j:Name></j:Name>
            '    <j:HW></j:HW>
            '    <j:Version></j:Version>
            '    <j:Revision></j:Revision>
            '    <j:Serial></j:Serial>
            '    <j:OEM></j:OEM>
            '    <j:Lang></j:Lang>
            '    <j:Annex></j:Annex>
            '    <j:Lab/>
            '    <j:Country></j:Country>
            '</j:BoxInfo>

            '<j:BoxInfo xmlns:j="http://jason.avm.de/updatecheck/">
            '    <j:Name></j:Name>
            '    <j:HW></j:HW>
            '    <j:Version>84.06.25-30630</j:Version>
            '    <j:Revision></j:Revision>
            '    <j:Serial></j:Serial>
            '    <j:OEM></j:OEM>
            '    <j:Lang></j:Lang>
            '    <j:Annex></j:Annex>
            '    <j:Lab></j:Lab>
            '    <j:Country></j:Country>
            '    <j:Flag></j:Flag>
            '    <j:UpdateConfig></j:UpdateConfig>
            '</j:BoxInfo>

            InfoXML = New XmlDocument
            With InfoXML
                .LoadXml(Response)
                Response = .GetElementsByTagName("Version", "http://jason.avm.de/updatecheck/").Item(0).InnerText
                tmp = Split(Response, "-", , CompareMethod.Text)
                If tmp.Count = 1 Then
                    ' Revision anhängen, bei LaborFW hängt es schon dran
                    Response += "-" & .GetElementsByTagName("Revision", "http://jason.avm.de/updatecheck/").Item(0).InnerText
                End If

                tmpFBFW.SetFirmware(Response)
            End With
        Else
            ' ältere Versionen bis 4.82 prüfen
            ' dauert deutlich länger, als die Jason BoxInfo
            Response = C_hf.httpGET(P_Link_FB_SystemStatus, FBEncoding, FBFehler)
            If Not FBFehler Then
                tmp = Split(C_hf.StringEntnehmen(Response, "<body>", "</body>"), "-", , CompareMethod.Text)
                If Not tmp.Count = 1 Then
                    With tmpFBFW
                        Response = Replace(C_hf.GruppiereNummer(tmp(7)), " ", ".", , CompareMethod.Text) & "-" & tmp(8)
                        tmpFBFW.SetFirmware(Response)
                    End With
                Else
                    FBFehler = True
                End If
            End If
        End If
        ThisFBFirmware = tmpFBFW
        Return FBFehler
    End Function
#End Region

#Region "Fritz!Box Telefonbuch"
    ''' <summary>
    ''' Lädt ein einen einzelnen Kontakt in das aktuell geöffnete Telefonbuch der Fritz!Box hoch.
    ''' </summary>
    ''' <param name="Kontakt">Der Kontakt, der hichgeladen werden soll.</param>
    ''' <param name="istVIP">Angabe, ob der Kontakt ein VIP ist. Diese Information wird übernommen.</param>
    Sub UploadKontaktToFritzBox(ByVal Kontakt As Outlook.ContactItem, ByVal istVIP As Boolean)

        Dim EntryName As String
        Dim EmailNew1 As String

        Dim NumberNew(3) As String
        Dim NumberType(3) As String


        Dim cmd As String
        Dim ReturnValue As String

        NumberType(0) = "home"
        NumberType(1) = "mobile"
        NumberType(2) = "work"
        NumberType(3) = "fax_work"

        With Kontakt
            EntryName = .FullName
            NumberNew(0) = C_hf.nurZiffern(.HomeTelephoneNumber)
            NumberNew(1) = C_hf.nurZiffern(.MobileTelephoneNumber)
            NumberNew(2) = C_hf.nurZiffern(.BusinessTelephoneNumber)
            NumberNew(3) = C_hf.nurZiffern(.BusinessFaxNumber)
            EmailNew1 = .Email1Address
        End With

        If P_SID = DataProvider.P_Def_SessionID Then FBLogin()

        If Not P_SID = DataProvider.P_Def_SessionID And Len(P_SID) = Len(DataProvider.P_Def_SessionID) Then
            cmd = "sid=" & P_SID & "&entryname=" & EntryName

            For i = LBound(NumberType) To UBound(NumberType)
                If Not NumberNew(i) = DataProvider.P_Def_LeerString Then
                    cmd += "&numbertypenew1=" & NumberType(i) & "&numbernew1=" & NumberNew(i)
                End If
            Next

            If istVIP Then cmd += "&category=on"

            If Not EmailNew1 = DataProvider.P_Def_LeerString Then cmd += "&emailnew1=" & EmailNew1

            cmd += "&apply=" 'Wichtig!

            With C_hf
                ReturnValue = .httpPOST(P_Link_FB_FonBook_Entry, cmd, FBEncoding)
                If ReturnValue.Contains(EntryName) Then
                    .LogFile(DataProvider.P_Kontakt_Hochgeladen(EntryName))
                    .MsgBox(DataProvider.P_Kontakt_Hochgeladen(EntryName), MsgBoxStyle.Information, "UploadKontaktToFritzBox")
                Else
                    .MsgBox(DataProvider.P_Fehler_Kontakt_Hochladen(EntryName), MsgBoxStyle.Exclamation, "UploadKontaktToFritzBox")
                End If

            End With
        Else
            C_hf.MsgBox(DataProvider.P_FritzBox_Dial_Error3(P_SID), MsgBoxStyle.Critical, "UploadKontaktToFritzBox")
        End If
    End Sub

    ''' <summary>
    ''' Lädt das gewünschte Telefonbuch von der Fritz!Box herunter.
    ''' </summary>
    ''' <param name="sPhonebookId">
    ''' ID des Telefonbuches: 
    ''' 0 = Haupttelefonbuch
    ''' 255 = Intern
    ''' 256 = Clip Info</param>
    ''' <param name="sPhonebookExportName">Der Name des Telefonbuches, welcher mindestens ein Zeichen enthalten muss, wenn die ID größer als ID 1 ist.</param>
    ''' <returns>XMl Telefonbuch</returns>
    Friend Function DownloadAddressbook(ByVal sPhonebookId As String, ByVal sPhonebookExportName As String) As XmlDocument
        DownloadAddressbook = Nothing
        Dim row As String
        Dim cmd As String
        Dim ReturnValue As String
        Dim XMLFBAddressbuch As XmlDocument

        If P_SID = DataProvider.P_Def_SessionID Then FBLogin()
        If Not P_SID = DataProvider.P_Def_SessionID And Len(P_SID) = Len(DataProvider.P_Def_SessionID) Then

            row = "---" & 12345 + Rnd() * 16777216
            cmd = row & vbCrLf & "Content-Disposition: form-data; name=""sid""" & vbCrLf & vbCrLf & P_SID & vbCrLf _
             & row & vbCrLf & "Content-Disposition: form-data; name=""PhonebookId""" & vbCrLf & vbCrLf & sPhonebookId & vbCrLf _
             & row & vbCrLf & "Content-Disposition: form-data; name=""PhonebookExportName""" & vbCrLf & vbCrLf & sPhonebookExportName & vbCrLf _
             & row & vbCrLf & "Content-Disposition: form-data; name=""PhonebookExport""" & vbCrLf & vbCrLf & vbCrLf & row & "--" & vbCrLf

            With C_hf
                ReturnValue = .httpPOST(P_Link_FB_ExportAddressbook, cmd, FBEncoding)
                If ReturnValue.StartsWith("<?xml") Then
                    XMLFBAddressbuch = New XmlDocument()
                    Try
                        XMLFBAddressbuch.LoadXml(ReturnValue)
                    Catch ex As Exception
                        .LogFile(DataProvider.P_Fehler_Export_Addressbuch)
                    End Try
                    DownloadAddressbook = XMLFBAddressbuch
                End If
            End With
        Else
            C_hf.MsgBox(DataProvider.P_FritzBox_Dial_Error3(P_SID), MsgBoxStyle.Critical, "DownloadAddressbook")
        End If
    End Function

    ''' <summary>
    ''' Lädt ein Fritz!Box Telefonbuch im XML Format auf die Fritz!Box hoch. 
    ''' </summary>
    ''' <param name="sPhonebookId">
    ''' ID des Telefonbuches: 
    ''' 0 = Haupttelefonbuch
    ''' 255 = Intern
    ''' 256 = Clip Info</param>
    ''' <param name="XMLTelefonbuch">Das Telefonbuch im XML Format</param>
    ''' <returns>Bollean, ob Upload erfolgreich war oder halt nicht.</returns>
    Friend Function UploadAddressbook(ByVal sPhonebookId As String, ByVal XMLTelefonbuch As String) As Boolean
        Dim cmd As String
        UploadAddressbook = False

        If P_SID = DataProvider.P_Def_SessionID Then FBLogin()
        If Not P_SID = DataProvider.P_Def_SessionID And Len(P_SID) = Len(DataProvider.P_Def_SessionID) Then

            cmd = "---" & 12345 + Rnd() * 16777216
            cmd = cmd & vbCrLf & "Content-Disposition: form-data; name=""sid""" & vbCrLf & vbCrLf & P_SID & vbCrLf _
            & cmd & vbCrLf & "Content-Disposition: form-data; name=""PhonebookId""" & vbCrLf & vbCrLf & sPhonebookId & vbCrLf _
            & cmd & vbCrLf & "Content-Disposition: form-data; name=""PhonebookImportFile""" & vbCrLf & vbCrLf & "@" + XMLTelefonbuch + ";type=text/xml" & vbCrLf _
            & cmd & "--" & vbCrLf

            UploadAddressbook = C_hf.httpPOST(P_Link_FB_ExportAddressbook, cmd, FBEncoding).Contains("Das Telefonbuch der FRITZ!Box wurde wiederhergestellt.")

        Else
            C_hf.MsgBox(DataProvider.P_FritzBox_Dial_Error3(P_SID), MsgBoxStyle.Critical, "UploadAddressbook")
        End If
    End Function

    ' Link Telefonbuch hinzufügen
    ' http://192.168.180.1/fon_num/fonbook_edit.lua?sid=9f4d23c5f4dcefd2&uid=new&back_to_page=%2Ffon_num%2Ffonbook_list.lua

    ''' <summary>
    ''' Gibt eine Liste der verfügbaren Fritz!Box Telefonbücher zurück.
    ''' </summary>
    ''' <returns>List</returns>
    ''' <remarks>http://fritz.box/fon_num/fonbook_select.lua</remarks>
    Friend Function GetTelefonbuchListe() As String()
        Dim ReturnTelefonbuchListe As String() = {"0'>Telefonbuch"}

        Dim sPage As String
        Dim tmp As String
        Dim Liste As String = DataProvider.P_Def_LeerString
        Dim pos As Integer = 1

        If P_SID = DataProvider.P_Def_SessionID Then FBLogin()
        If Not P_SID = DataProvider.P_Def_SessionID And Len(P_SID) = Len(DataProvider.P_Def_SessionID) Then
            sPage = Replace(C_hf.httpGET(P_Link_Telefonbuch_List(P_SID), FBEncoding, FBFehler), Chr(34), "'", , , CompareMethod.Text)
            sPage = sPage.Replace(Chr(13), "")
            If sPage.Contains("label for='uiBookid:") Then
                Do
                    tmp = C_hf.StringEntnehmen(sPage, "label for='uiBookid:", "</label>", pos)
                    If tmp IsNot DataProvider.P_Def_ErrorMinusOne_String Then
                        tmp = tmp.Replace("'>", ": ")
                        Liste += tmp & ";"
                    End If
                Loop Until tmp Is DataProvider.P_Def_ErrorMinusOne_String
                Liste = Liste.Remove(Liste.Length - 1, 1)
            End If
            ReturnTelefonbuchListe = Split(Liste, ";", , CompareMethod.Text)
        End If
        Return ReturnTelefonbuchListe
    End Function

#End Region

    Private Sub PushStatus(ByVal Status As String)
        tb.Text = Status
    End Sub

    Friend Sub SetEventProvider(ByVal ep As IEventProvider)
        If EventProvider Is Nothing Then
            EventProvider = ep
            AddHandler tb.TextChanged, AddressOf ep.GenericHandler
        End If
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                tb.Dispose()
                ' TODO: Verwalteten Zustand löschen (verwaltete Objekte).
            End If
            BWSetDialPort.Dispose()
            ' TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() unten überschreiben.
            ' TODO: Große Felder auf NULL festlegen.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(ByVal disposing As Boolean) oben über Code zum Freigeben von nicht verwalteten Ressourcen verfügt.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(ByVal disposing As Boolean) Bereinigungscode ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(disposing As Boolean) Bereinigungscode ein.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

#Region "FritzBoxQuery"
    Private Function FritzBoxQuery(ByVal Abfrage As String) As String
        FritzBoxQuery = DataProvider.P_Def_ErrorMinusOne_String

        If P_SID = DataProvider.P_Def_SessionID Then FBLogin()
        If Not P_SID = DataProvider.P_Def_SessionID And Len(P_SID) = Len(DataProvider.P_Def_SessionID) Then
            FritzBoxQuery = C_hf.httpGET(P_Link_Query(P_SID, Abfrage), FBEncoding, FBFehler)
        End If
    End Function

#End Region
End Class