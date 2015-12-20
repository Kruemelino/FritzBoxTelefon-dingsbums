Imports FritzBoxDial
Imports Newtonsoft.Json

Public Class TAMEntry
    Private sActive As String
    Public Property Active() As String
        Get
            Return sActive
        End Get
        Set(ByVal value As String)
            sActive = value
        End Set
    End Property

    Private sName As String
    Public Property Name() As String
        Get
            Return sName
        End Get
        Set(ByVal value As String)
            sName = value
        End Set
    End Property

    Private sDisplay As String
    Public Property Display() As String
        Get
            Return sDisplay
        End Get
        Set(ByVal value As String)
            sDisplay = value
        End Set
    End Property

    Private sMSNBitmap As String
    Public Property MSNBitmap() As String
        Get
            Return sMSNBitmap
        End Get
        Set(ByVal value As String)
            sMSNBitmap = value
        End Set
    End Property
End Class

Public Class SIPEntry
    Private sactivated As String
    Public Property activated() As String
        Get
            Return sactivated
        End Get
        Set(ByVal value As String)
            sactivated = value
        End Set
    End Property

    Private sdisplayname As String
    Public Property displayname() As String
        Get
            Return sdisplayname
        End Get
        Set(ByVal value As String)
            sdisplayname = value
        End Set
    End Property

    Private sregistrar As String
    Public Property registrar() As String
        Get
            Return sregistrar
        End Get
        Set(ByVal value As String)
            sregistrar = value
        End Set
    End Property

    Private soutboundproxy As String
    Public Property outboundproxy() As String
        Get
            Return soutboundproxy
        End Get
        Set(ByVal value As String)
            soutboundproxy = value
        End Set
    End Property

    Private sprovidername As String
    Public Property providername() As String
        Get
            Return sprovidername
        End Get
        Set(ByVal value As String)
            sprovidername = value
        End Set
    End Property

    Private sID As String
    Public Property ID() As String
        Get
            Return sID
        End Get
        Set(ByVal value As String)
            sID = value
        End Set
    End Property

    Private sgui_readonly As String
    Public Property gui_readonly() As String
        Get
            Return sgui_readonly
        End Get
        Set(ByVal value As String)
            sgui_readonly = value
        End Set
    End Property

    Private swebui_trunk_id As String
    Public Property webui_trunk_id() As String
        Get
            Return swebui_trunk_id
        End Get
        Set(ByVal value As String)
            swebui_trunk_id = value
        End Set
    End Property
End Class

Public Class MSNEntry
    Private sName As String
    Public Property Name() As String
        Get
            Return sName
        End Get
        Set(ByVal value As String)
            sName = value
        End Set
    End Property

    Private sFax As String
    Public Property Fax() As String
        Get
            Return sFax
        End Get
        Set(ByVal value As String)
            sFax = value
        End Set
    End Property

    Private sGroupCall As String
    Public Property GroupCall() As String
        Get
            Return sGroupCall
        End Get
        Set(ByVal value As String)
            sGroupCall = value
        End Set
    End Property

    Private sAllIncomingCalls As String
    Public Property AllIncomingCalls() As String
        Get
            Return sAllIncomingCalls
        End Get
        Set(ByVal value As String)
            sAllIncomingCalls = value
        End Set
    End Property

    Private sOutDialing As String
    Public Property OutDialing() As String
        Get
            Return sOutDialing
        End Get
        Set(ByVal value As String)
            sOutDialing = value
        End Set
    End Property
End Class

Public Class VOIPEntry
    Private senabled As String
    Public Property enabled() As String
        Get
            Return senabled
        End Get
        Set(ByVal value As String)
            senabled = value
        End Set
    End Property

    Private sName As String
    Public Property Name() As String
        Get
            Return sName
        End Get
        Set(ByVal value As String)
            sName = value
        End Set
    End Property

    Private sRingOnAllMSNs As String
    Public Property RingOnAllMSNs() As String
        Get
            Return sRingOnAllMSNs
        End Get
        Set(ByVal value As String)
            sRingOnAllMSNs = value
        End Set
    End Property
End Class

Friend Class FoncontrolUserList
    Private sName As String
    Friend Property Name() As String
        Get
            Return sName
        End Get
        Set(ByVal value As String)
            sName = value
        End Set
    End Property

    Private sType As String
    Friend Property Type() As String
        Get
            Return sType
        End Get
        Set(ByVal value As String)
            sType = value
        End Set
    End Property

    Private sIntern As String
    Friend Property Intern() As String
        Get
            Return sIntern
        End Get
        Set(ByVal value As String)
            sIntern = value
        End Set
    End Property

    Private sId As String
    Friend Property Id() As String
        Get
            Return sId
        End Get
        Set(ByVal value As String)
            sId = value
        End Set
    End Property
End Class

Friend Class FoncontrolUserNList
    Private sNumber As String
    Friend Property Number() As String
        Get
            Return sNumber
        End Get
        Set(ByVal value As String)
            sNumber = value
        End Set
    End Property
End Class

Friend Class VoipExtensionList
    Private senabled As String
    Friend Property enabled() As String
        Get
            Return senabled
        End Get
        Set(ByVal value As String)
            senabled = value
        End Set
    End Property

    Private sName As String
    Friend Property Name() As String
        Get
            Return sName
        End Get
        Set(ByVal value As String)
            sName = value
        End Set
    End Property

    Private sRingOnAllMSNs As String
    Friend Property RingOnAllMSNs() As String
        Get
            Return sRingOnAllMSNs
        End Get
        Set(ByVal value As String)
            sRingOnAllMSNs = value
        End Set
    End Property
End Class

Public Class DECTNr
    Private sNumber As String
    Public Property Number() As String
        Get
            Return sNumber
        End Get
        Set(ByVal value As String)
            sNumber = value
        End Set
    End Property
End Class

Public Class DECTEntry
    Private sName As String
    Public Property Name() As String
        Get
            Return sName
        End Get
        Set(ByVal value As String)
            sName = value
        End Set
    End Property

    Private sType As String
    Public Property Type() As String
        Get
            Return sType
        End Get
        Set(ByVal value As String)
            sType = value
        End Set
    End Property

    Private sIntern As String
    Public Property Intern() As String
        Get
            Return sIntern
        End Get
        Set(ByVal value As String)
            sIntern = value
        End Set
    End Property

    Private sId As String
    Public Property Id() As String
        Get
            Return sId
        End Get
        Set(ByVal value As String)
            sId = value
        End Set
    End Property
End Class

Public Class FritzBoxJSONTelefone1
    Private _tAM() As TAMEntry
    Private _dECT() As DECTEntry
    Private _fON() As MSNEntry
    Private _vOIP() As VOIPEntry

    Private sS0Name1 As String
    Public Property S0Name1() As String
        Get
            Return sS0Name1
        End Get
        Set(ByVal value As String)
            sS0Name1 = value
        End Set
    End Property

    Private sS0Name2 As String
    Public Property S0Name2() As String
        Get
            Return sS0Name2
        End Get
        Set(ByVal value As String)
            sS0Name2 = value
        End Set
    End Property

    Private sS0Name3 As String
    Public Property S0Name3() As String
        Get
            Return sS0Name3
        End Get
        Set(ByVal value As String)
            sS0Name3 = value
        End Set
    End Property

    Private sS0Name4 As String
    Public Property S0Name4() As String
        Get
            Return sS0Name4
        End Get
        Set(ByVal value As String)
            sS0Name4 = value
        End Set
    End Property

    Private sS0Name5 As String
    Public Property S0Name5() As String
        Get
            Return sS0Name5
        End Get
        Set(ByVal value As String)
            sS0Name5 = value
        End Set
    End Property

    Private sS0Name6 As String
    Public Property S0Name6() As String
        Get
            Return sS0Name6
        End Get
        Set(ByVal value As String)
            sS0Name6 = value
        End Set
    End Property

    Private sS0Name7 As String
    Public Property S0Name7() As String
        Get
            Return sS0Name7
        End Get
        Set(ByVal value As String)
            sS0Name7 = value
        End Set
    End Property

    Private sS0Name8 As String
    Public Property S0Name8() As String
        Get
            Return sS0Name8
        End Get
        Set(ByVal value As String)
            sS0Name8 = value
        End Set
    End Property

    Public Property TAM As TAMEntry()
        Get
            Return _tAM
        End Get
        Set(value As TAMEntry())
            _tAM = value
        End Set
    End Property

    Public Property DECT As DECTEntry()
        Get
            Return _dECT
        End Get
        Set(value As DECTEntry())
            _dECT = value
        End Set
    End Property

    Public Property FON As MSNEntry()
        Get
            Return _fON
        End Get
        Set(value As MSNEntry())
            _fON = value
        End Set
    End Property

    Public Property VOIP As VOIPEntry()
        Get
            Return _vOIP
        End Get
        Set(value As VOIPEntry())
            _vOIP = value
        End Set
    End Property
End Class

Public Class FritzBoxJSONTelefone2

    Private sS0TelNr1 As String
    Public Property S0TelNr1() As String
        Get
            Return sS0TelNr1
        End Get
        Set(ByVal value As String)
            sS0TelNr1 = value
        End Set
    End Property

    Private sS0TelNr2 As String
    Public Property S0TelNr2() As String
        Get
            Return sS0TelNr2
        End Get
        Set(ByVal value As String)
            sS0TelNr2 = value
        End Set
    End Property

    Private sS0TelNr3 As String
    Public Property S0TelNr3() As String
        Get
            Return sS0TelNr3
        End Get
        Set(ByVal value As String)
            sS0TelNr3 = value
        End Set
    End Property

    Private sS0TelNr4 As String
    Public Property S0TelNr4() As String
        Get
            Return sS0TelNr4
        End Get
        Set(ByVal value As String)
            sS0TelNr4 = value
        End Set
    End Property

    Private sS0TelNr5 As String
    Public Property S0TelNr5() As String
        Get
            Return sS0TelNr5
        End Get
        Set(ByVal value As String)
            sS0TelNr5 = value
        End Set
    End Property

    Private sS0TelNr6 As String
    Public Property S0TelNr6() As String
        Get
            Return sS0TelNr6
        End Get
        Set(ByVal value As String)
            sS0TelNr6 = value
        End Set
    End Property

    Private sS0TelNr7 As String
    Public Property S0TelNr7() As String
        Get
            Return sS0TelNr7
        End Get
        Set(ByVal value As String)
            sS0TelNr7 = value
        End Set
    End Property

    Private sS0TelNr8 As String
    Public Property S0TelNr8() As String
        Get
            Return sS0TelNr8
        End Get
        Set(ByVal value As String)
            sS0TelNr8 = value
        End Set
    End Property

    Private sS0Type1 As String
    Public Property S0Type1() As String
        Get
            Return sS0Type1
        End Get
        Set(ByVal value As String)
            sS0Type1 = value
        End Set
    End Property

    Private sS0Type2 As String
    Public Property S0Type2() As String
        Get
            Return sS0Type2
        End Get
        Set(ByVal value As String)
            sS0Type2 = value
        End Set
    End Property

    Private sS0Type3 As String
    Public Property S0Type3() As String
        Get
            Return sS0Type3
        End Get
        Set(ByVal value As String)
            sS0Type3 = value
        End Set
    End Property

    Private sS0Type4 As String
    Public Property S0Type4() As String
        Get
            Return sS0Type4
        End Get
        Set(ByVal value As String)
            sS0Type4 = value
        End Set
    End Property

    Private sS0Type5 As String
    Public Property S0Type5() As String
        Get
            Return sS0Type5
        End Get
        Set(ByVal value As String)
            sS0Type5 = value
        End Set
    End Property

    Private sS0Type6 As String
    Public Property S0Type6() As String
        Get
            Return sS0Type6
        End Get
        Set(ByVal value As String)
            sS0Type6 = value
        End Set
    End Property

    Private sS0Type7 As String
    Public Property S0Type7() As String
        Get
            Return sS0Type7
        End Get
        Set(ByVal value As String)
            sS0Type7 = value
        End Set
    End Property

    Private sS0Type8 As String
    Public Property S0Type8() As String
        Get
            Return sS0Type8
        End Get
        Set(ByVal value As String)
            sS0Type8 = value
        End Set
    End Property

    Private eDECT0Nr As DECTNr()
    Public Property DECT0Nr() As DECTNr()
        Get
            Return eDECT0Nr
        End Get
        Set(ByVal value As DECTNr())
            eDECT0Nr = value
        End Set
    End Property

    Private eDECT1Nr As DECTNr()
    Public Property DECT1Nr() As DECTNr()
        Get
            Return eDECT1Nr
        End Get
        Set(ByVal value As DECTNr())
            eDECT1Nr = value
        End Set
    End Property

    Private eDECT2Nr As DECTNr()
    Public Property DECT2Nr() As DECTNr()
        Get
            Return eDECT2Nr
        End Get
        Set(ByVal value As DECTNr())
            eDECT2Nr = value
        End Set
    End Property

    Private eDECT3Nr As DECTNr()
    Public Property DECT3Nr() As DECTNr()
        Get
            Return eDECT3Nr
        End Get
        Set(ByVal value As DECTNr())
            eDECT3Nr = value
        End Set
    End Property

    Private eDECT4Nr As DECTNr()
    Public Property DECT4Nr() As DECTNr()
        Get
            Return eDECT4Nr
        End Get
        Set(ByVal value As DECTNr())
            eDECT4Nr = value
        End Set
    End Property

    Private sFaxMailActive As String
    Public Property FaxMailActive() As String
        Get
            Return sFaxMailActive
        End Get
        Set(ByVal value As String)
            sFaxMailActive = value
        End Set
    End Property

    Private sMobileName As String
    Public Property MobileName() As String
        Get
            Return sMobileName
        End Get
        Set(ByVal value As String)
            sMobileName = value
        End Set
    End Property

    Private sDECT0RingOnAllMSNs As String
    Public Property DECT0RingOnAllMSNs() As String
        Get
            Return sDECT0RingOnAllMSNs
        End Get
        Set(ByVal value As String)
            sDECT0RingOnAllMSNs = value
        End Set
    End Property

    Private sDECT1RingOnAllMSNs As String
    Public Property DECT1RingOnAllMSNs() As String
        Get
            Return sDECT1RingOnAllMSNs
        End Get
        Set(ByVal value As String)
            sDECT1RingOnAllMSNs = value
        End Set
    End Property

    Private sDECT2RingOnAllMSNs As String
    Public Property DECT2RingOnAllMSNs() As String
        Get
            Return sDECT2RingOnAllMSNs
        End Get
        Set(ByVal value As String)
            sDECT2RingOnAllMSNs = value
        End Set
    End Property

    Private sDECT3RingOnAllMSNs As String
    Public Property DECT3RingOnAllMSNs() As String
        Get
            Return sDECT3RingOnAllMSNs
        End Get
        Set(ByVal value As String)
            sDECT3RingOnAllMSNs = value
        End Set
    End Property

    Private sDECT4RingOnAllMSNs As String
    Public Property DECT4RingOnAllMSNs() As String
        Get
            Return sDECT4RingOnAllMSNs
        End Get
        Set(ByVal value As String)
            sDECT4RingOnAllMSNs = value
        End Set
    End Property
End Class

Public Class FritzBoxJSONTelNrT1
    Private sPOTS As String
    Public Property POTS() As String
        Get
            Return sPOTS
        End Get
        Set(ByVal value As String)
            sPOTS = value

        End Set
    End Property

    Private sMobile As String
    Public Property Mobile() As String
        Get
            Return sMobile
        End Get
        Set(ByVal value As String)
            sMobile = value
        End Set
    End Property

    Private sPort0Name As String
    Public Property Port0Name() As String
        Get
            Return sPort0Name
        End Get
        Set(ByVal value As String)
            sPort0Name = value
        End Set
    End Property

    Private sPort1Name As String
    Public Property Port1Name() As String
        Get
            Return sPort1Name
        End Get
        Set(ByVal value As String)
            sPort1Name = value
        End Set
    End Property

    Private sPort2Name As String
    Public Property Port2Name() As String
        Get
            Return sPort2Name
        End Get
        Set(ByVal value As String)
            sPort2Name = value
        End Set
    End Property

    Private sTAM0 As String
    Public Property TAM0() As String
        Get
            Return sTAM0
        End Get
        Set(ByVal value As String)
            sTAM0 = value
        End Set
    End Property

    Private sFAX0 As String
    Public Property FAX0() As String
        Get
            Return sFAX0
        End Get
        Set(ByVal value As String)
            sFAX0 = value
        End Set
    End Property

    Private sMSN0 As String
    Public Property MSN0() As String
        Get
            Return sMSN0
        End Get
        Set(ByVal value As String)
            sMSN0 = value
        End Set
    End Property

    Private sVOIP0Enabled As String
    Public Property VOIP0Enabled() As String
        Get
            Return sVOIP0Enabled
        End Get
        Set(ByVal value As String)
            sVOIP0Enabled = value
        End Set
    End Property

    Private sTAM1 As String
    Public Property TAM1() As String
        Get
            Return sTAM1
        End Get
        Set(ByVal value As String)
            sTAM1 = value
        End Set
    End Property

    Private sFAX1 As String
    Public Property FAX1() As String
        Get
            Return sFAX1
        End Get
        Set(ByVal value As String)
            sFAX1 = value
        End Set
    End Property

    Private sMSN1 As String
    Public Property MSN1() As String
        Get
            Return sMSN1
        End Get
        Set(ByVal value As String)
            sMSN1 = value
        End Set
    End Property

    Private sVOIP1Enabled As String
    Public Property VOIP1Enabled() As String
        Get
            Return sVOIP1Enabled
        End Get
        Set(ByVal value As String)
            sVOIP1Enabled = value
        End Set
    End Property

    Private sTAM2 As String
    Public Property TAM2() As String
        Get
            Return sTAM2
        End Get
        Set(ByVal value As String)
            sTAM2 = value
        End Set
    End Property

    Private sFAX2 As String
    Public Property FAX2() As String
        Get
            Return sFAX2
        End Get
        Set(ByVal value As String)
            sFAX2 = value
        End Set
    End Property

    Private sMSN2 As String
    Public Property MSN2() As String
        Get
            Return sMSN2
        End Get
        Set(ByVal value As String)
            sMSN2 = value
        End Set
    End Property

    Private sVOIP2Enabled As String
    Public Property VOIP2Enabled() As String
        Get
            Return sVOIP2Enabled
        End Get
        Set(ByVal value As String)
            sVOIP2Enabled = value
        End Set
    End Property

    Private sTAM3 As String
    Public Property TAM3() As String
        Get
            Return sTAM3
        End Get
        Set(ByVal value As String)
            sTAM3 = value
        End Set
    End Property

    Private sFAX3 As String
    Public Property FAX3() As String
        Get
            Return sFAX3
        End Get
        Set(ByVal value As String)
            sFAX3 = value
        End Set
    End Property

    Private sMSN3 As String
    Public Property MSN3() As String
        Get
            Return sMSN3
        End Get
        Set(ByVal value As String)
            sMSN3 = value
        End Set
    End Property

    Private sVOIP3Enabled As String
    Public Property VOIP3Enabled() As String
        Get
            Return sVOIP3Enabled
        End Get
        Set(ByVal value As String)
            sVOIP3Enabled = value
        End Set
    End Property

    Private sTAM4 As String
    Public Property TAM4() As String
        Get
            Return sTAM4
        End Get
        Set(ByVal value As String)
            sTAM4 = value
        End Set
    End Property

    Private sFAX4 As String
    Public Property FAX4() As String
        Get
            Return sFAX4
        End Get
        Set(ByVal value As String)
            sFAX4 = value
        End Set
    End Property

    Private sMSN4 As String
    Public Property MSN4() As String
        Get
            Return sMSN4
        End Get
        Set(ByVal value As String)
            sMSN4 = value
        End Set
    End Property

    Private sVOIP4Enabled As String
    Public Property VOIP4Enabled() As String
        Get
            Return sVOIP4Enabled
        End Get
        Set(ByVal value As String)
            sVOIP4Enabled = value
        End Set
    End Property

    Private sTAM5 As String
    Public Property TAM5() As String
        Get
            Return sTAM5
        End Get
        Set(ByVal value As String)
            sTAM5 = value
        End Set
    End Property

    Private sFAX5 As String
    Public Property FAX5() As String
        Get
            Return sFAX5
        End Get
        Set(ByVal value As String)
            sFAX5 = value
        End Set
    End Property

    Private sMSN5 As String
    Public Property MSN5() As String
        Get
            Return sMSN5
        End Get
        Set(ByVal value As String)
            sMSN5 = value
        End Set
    End Property

    Private sVOIP5Enabled As String
    Public Property VOIP5Enabled() As String
        Get
            Return sVOIP5Enabled
        End Get
        Set(ByVal value As String)
            sVOIP5Enabled = value
        End Set
    End Property

    Private sTAM6 As String
    Public Property TAM6() As String
        Get
            Return sTAM6
        End Get
        Set(ByVal value As String)
            sTAM6 = value
        End Set
    End Property

    Private sFAX6 As String
    Public Property FAX6() As String
        Get
            Return sFAX6
        End Get
        Set(ByVal value As String)
            sFAX6 = value
        End Set
    End Property

    Private sMSN6 As String
    Public Property MSN6() As String
        Get
            Return sMSN6
        End Get
        Set(ByVal value As String)
            sMSN6 = value
        End Set
    End Property

    Private sVOIP6Enabled As String
    Public Property VOIP6Enabled() As String
        Get
            Return sVOIP6Enabled
        End Get
        Set(ByVal value As String)
            sVOIP6Enabled = value
        End Set
    End Property

    Private sTAM7 As String
    Public Property TAM7() As String
        Get
            Return sTAM7
        End Get
        Set(ByVal value As String)
            sTAM7 = value
        End Set
    End Property

    Private sFAX7 As String
    Public Property FAX7() As String
        Get
            Return sFAX7
        End Get
        Set(ByVal value As String)
            sFAX7 = value
        End Set
    End Property

    Private sMSN7 As String
    Public Property MSN7() As String
        Get
            Return sMSN7
        End Get
        Set(ByVal value As String)
            sMSN7 = value
        End Set
    End Property

    Private sVOIP7Enabled As String
    Public Property VOIP7Enabled() As String
        Get
            Return sVOIP7Enabled
        End Get
        Set(ByVal value As String)
            sVOIP7Enabled = value
        End Set
    End Property

    Private sTAM8 As String
    Public Property TAM8() As String
        Get
            Return sTAM8
        End Get
        Set(ByVal value As String)
            sTAM8 = value
        End Set
    End Property

    Private sFAX8 As String
    Public Property FAX8() As String
        Get
            Return sFAX8
        End Get
        Set(ByVal value As String)
            sFAX8 = value
        End Set
    End Property

    Private sMSN8 As String
    Public Property MSN8() As String
        Get
            Return sMSN8
        End Get
        Set(ByVal value As String)
            sMSN8 = value
        End Set
    End Property

    Private sVOIP8Enabled As String
    Public Property VOIP8Enabled() As String
        Get
            Return sVOIP8Enabled
        End Get
        Set(ByVal value As String)
            sVOIP8Enabled = value
        End Set
    End Property

    Private sTAM9 As String
    Public Property TAM9() As String
        Get
            Return sTAM9
        End Get
        Set(ByVal value As String)
            sTAM9 = value
        End Set
    End Property

    Private sFAX9 As String
    Public Property FAX9() As String
        Get
            Return sFAX9
        End Get
        Set(ByVal value As String)
            sFAX9 = value
        End Set
    End Property

    Private sMSN9 As String
    Public Property MSN9() As String
        Get
            Return sMSN9
        End Get
        Set(ByVal value As String)
            sMSN9 = value
        End Set
    End Property

    Private sVOIP9Enabled As String
    Public Property VOIP9Enabled() As String
        Get
            Return sVOIP9Enabled
        End Get
        Set(ByVal value As String)
            sVOIP9Enabled = value
        End Set
    End Property

    Private eSIP() As SIPEntry
    Public Property SIP() As SIPEntry()
        Get
            Return eSIP
        End Get
        Set(ByVal value As SIPEntry())
            eSIP = value
        End Set
    End Property
End Class

'Public Class FritzBoxJSONTelNrT2
'#Region "MSN0"
'    Private sMSN0Nr0 As String
'    Public Property MSN0Nr0() As String
'        Get
'            Return sMSN0Nr0
'        End Get
'        Set(ByVal value As String)
'            sMSN0Nr0 = value
'        End Set
'    End Property

'    Private sMSN0Nr1 As String
'    Public Property MSN0Nr1() As String
'        Get
'            Return sMSN0Nr1
'        End Get
'        Set(ByVal value As String)
'            sMSN0Nr1 = value
'        End Set
'    End Property

'    Private sMSN0Nr2 As String
'    Public Property MSN0Nr2() As String
'        Get
'            Return sMSN0Nr2
'        End Get
'        Set(ByVal value As String)
'            sMSN0Nr2 = value
'        End Set
'    End Property

'    Private sMSN0Nr3 As String
'    Public Property MSN0Nr3() As String
'        Get
'            Return sMSN0Nr3
'        End Get
'        Set(ByVal value As String)
'            sMSN0Nr3 = value
'        End Set
'    End Property

'    Private sMSN0Nr4 As String
'    Public Property MSN0Nr4() As String
'        Get
'            Return sMSN0Nr4
'        End Get
'        Set(ByVal value As String)
'            sMSN0Nr4 = value
'        End Set
'    End Property

'    Private sMSN0Nr5 As String
'    Public Property MSN0Nr5() As String
'        Get
'            Return sMSN0Nr5
'        End Get
'        Set(ByVal value As String)
'            sMSN0Nr5 = value
'        End Set
'    End Property

'    Private sMSN0Nr6 As String
'    Public Property MSN0Nr6() As String
'        Get
'            Return sMSN0Nr6
'        End Get
'        Set(ByVal value As String)
'            sMSN0Nr6 = value
'        End Set
'    End Property

'    Private sMSN0Nr7 As String
'    Public Property MSN0Nr7() As String
'        Get
'            Return sMSN0Nr7
'        End Get
'        Set(ByVal value As String)
'            sMSN0Nr7 = value
'        End Set
'    End Property

'    Private sMSN0Nr8 As String
'    Public Property MSN0Nr8() As String
'        Get
'            Return sMSN0Nr8
'        End Get
'        Set(ByVal value As String)
'            sMSN0Nr8 = value
'        End Set
'    End Property

'    Private sMSN0Nr9 As String
'    Public Property MSN0Nr9() As String
'        Get
'            Return sMSN0Nr9
'        End Get
'        Set(ByVal value As String)
'            sMSN0Nr9 = value
'        End Set
'    End Property
'#End Region
'#Region "MSN1"
'    Private sMSN1Nr0 As String
'    Public Property MSN1Nr0() As String
'        Get
'            Return sMSN1Nr0
'        End Get
'        Set(ByVal value As String)
'            sMSN1Nr0 = value
'        End Set
'    End Property

'    Private sMSN1Nr1 As String
'    Public Property MSN1Nr1() As String
'        Get
'            Return sMSN1Nr1
'        End Get
'        Set(ByVal value As String)
'            sMSN1Nr1 = value
'        End Set
'    End Property

'    Private sMSN1Nr2 As String
'    Public Property MSN1Nr2() As String
'        Get
'            Return sMSN1Nr2
'        End Get
'        Set(ByVal value As String)
'            sMSN1Nr2 = value
'        End Set
'    End Property

'    Private sMSN1Nr3 As String
'    Public Property MSN1Nr3() As String
'        Get
'            Return sMSN1Nr3
'        End Get
'        Set(ByVal value As String)
'            sMSN1Nr3 = value
'        End Set
'    End Property

'    Private sMSN1Nr4 As String
'    Public Property MSN1Nr4() As String
'        Get
'            Return sMSN1Nr4
'        End Get
'        Set(ByVal value As String)
'            sMSN1Nr4 = value
'        End Set
'    End Property

'    Private sMSN1Nr5 As String
'    Public Property MSN1Nr5() As String
'        Get
'            Return sMSN1Nr5
'        End Get
'        Set(ByVal value As String)
'            sMSN1Nr5 = value
'        End Set
'    End Property

'    Private sMSN1Nr6 As String
'    Public Property MSN1Nr6() As String
'        Get
'            Return sMSN1Nr6
'        End Get
'        Set(ByVal value As String)
'            sMSN1Nr6 = value
'        End Set
'    End Property

'    Private sMSN1Nr7 As String
'    Public Property MSN1Nr7() As String
'        Get
'            Return sMSN1Nr7
'        End Get
'        Set(ByVal value As String)
'            sMSN1Nr7 = value
'        End Set
'    End Property

'    Private sMSN1Nr8 As String
'    Public Property MSN1Nr8() As String
'        Get
'            Return sMSN1Nr8
'        End Get
'        Set(ByVal value As String)
'            sMSN1Nr8 = value
'        End Set
'    End Property

'    Private sMSN1Nr9 As String
'    Public Property MSN1Nr9() As String
'        Get
'            Return sMSN1Nr9
'        End Get
'        Set(ByVal value As String)
'            sMSN1Nr9 = value
'        End Set
'    End Property
'#End Region
'#Region "MSN2"
'    Private sMSN2Nr0 As String
'    Public Property MSN2Nr0() As String
'        Get
'            Return sMSN2Nr0
'        End Get
'        Set(ByVal value As String)
'            sMSN2Nr0 = value
'        End Set
'    End Property

'    Private sMSN2Nr1 As String
'    Public Property MSN2Nr1() As String
'        Get
'            Return sMSN2Nr1
'        End Get
'        Set(ByVal value As String)
'            sMSN2Nr1 = value
'        End Set
'    End Property

'    Private sMSN2Nr2 As String
'    Public Property MSN2Nr2() As String
'        Get
'            Return sMSN2Nr2
'        End Get
'        Set(ByVal value As String)
'            sMSN2Nr2 = value
'        End Set
'    End Property

'    Private sMSN2Nr3 As String
'    Public Property MSN2Nr3() As String
'        Get
'            Return sMSN2Nr3
'        End Get
'        Set(ByVal value As String)
'            sMSN2Nr3 = value
'        End Set
'    End Property

'    Private sMSN2Nr4 As String
'    Public Property MSN2Nr4() As String
'        Get
'            Return sMSN2Nr4
'        End Get
'        Set(ByVal value As String)
'            sMSN2Nr4 = value
'        End Set
'    End Property

'    Private sMSN2Nr5 As String
'    Public Property MSN2Nr5() As String
'        Get
'            Return sMSN2Nr5
'        End Get
'        Set(ByVal value As String)
'            sMSN2Nr5 = value
'        End Set
'    End Property

'    Private sMSN2Nr6 As String
'    Public Property MSN2Nr6() As String
'        Get
'            Return sMSN2Nr6
'        End Get
'        Set(ByVal value As String)
'            sMSN2Nr6 = value
'        End Set
'    End Property

'    Private sMSN2Nr7 As String
'    Public Property MSN2Nr7() As String
'        Get
'            Return sMSN2Nr7
'        End Get
'        Set(ByVal value As String)
'            sMSN2Nr7 = value
'        End Set
'    End Property

'    Private sMSN2Nr8 As String
'    Public Property MSN2Nr8() As String
'        Get
'            Return sMSN2Nr8
'        End Get
'        Set(ByVal value As String)
'            sMSN2Nr8 = value
'        End Set
'    End Property

'    Private sMSN2Nr9 As String
'    Public Property MSN2Nr9() As String
'        Get
'            Return sMSN2Nr9
'        End Get
'        Set(ByVal value As String)
'            sMSN2Nr9 = value
'        End Set
'    End Property
'#End Region
'#Region "VOIP0"
'    Private sVOIP0Nr0 As String
'    Public Property VOIP0Nr0() As String
'        Get
'            Return sVOIP0Nr0
'        End Get
'        Set(ByVal value As String)
'            sVOIP0Nr0 = value
'        End Set
'    End Property

'    Private sVOIP0Nr1 As String
'    Public Property VOIP0Nr1() As String
'        Get
'            Return sVOIP0Nr1
'        End Get
'        Set(ByVal value As String)
'            sVOIP0Nr1 = value
'        End Set
'    End Property

'    Private sVOIP0Nr2 As String
'    Public Property VOIP0Nr2() As String
'        Get
'            Return sVOIP0Nr2
'        End Get
'        Set(ByVal value As String)
'            sVOIP0Nr2 = value
'        End Set
'    End Property

'    Private sVOIP0Nr3 As String
'    Public Property VOIP0Nr3() As String
'        Get
'            Return sVOIP0Nr3
'        End Get
'        Set(ByVal value As String)
'            sVOIP0Nr3 = value
'        End Set
'    End Property

'    Private sVOIP0Nr4 As String
'    Public Property VOIP0Nr4() As String
'        Get
'            Return sVOIP0Nr4
'        End Get
'        Set(ByVal value As String)
'            sVOIP0Nr4 = value
'        End Set
'    End Property

'    Private sVOIP0Nr5 As String
'    Public Property VOIP0Nr5() As String
'        Get
'            Return sVOIP0Nr5
'        End Get
'        Set(ByVal value As String)
'            sVOIP0Nr5 = value
'        End Set
'    End Property

'    Private sVOIP0Nr6 As String
'    Public Property VOIP0Nr6() As String
'        Get
'            Return sVOIP0Nr6
'        End Get
'        Set(ByVal value As String)
'            sVOIP0Nr6 = value
'        End Set
'    End Property

'    Private sVOIP0Nr7 As String
'    Public Property VOIP0Nr7() As String
'        Get
'            Return sVOIP0Nr7
'        End Get
'        Set(ByVal value As String)
'            sVOIP0Nr7 = value
'        End Set
'    End Property

'    Private sVOIP0Nr8 As String
'    Public Property VOIP0Nr8() As String
'        Get
'            Return sVOIP0Nr8
'        End Get
'        Set(ByVal value As String)
'            sVOIP0Nr8 = value
'        End Set
'    End Property

'    Private sVOIP0Nr9 As String
'    Public Property VOIP0Nr9() As String
'        Get
'            Return sVOIP0Nr9
'        End Get
'        Set(ByVal value As String)
'            sVOIP0Nr9 = value
'        End Set
'    End Property
'#End Region
'#Region "VOIP1"
'    Private sVOIP1Nr0 As String
'    Public Property VOIP1Nr0() As String
'        Get
'            Return sVOIP1Nr0
'        End Get
'        Set(ByVal value As String)
'            sVOIP1Nr0 = value
'        End Set
'    End Property

'    Private sVOIP1Nr1 As String
'    Public Property VOIP1Nr1() As String
'        Get
'            Return sVOIP1Nr1
'        End Get
'        Set(ByVal value As String)
'            sVOIP1Nr1 = value
'        End Set
'    End Property

'    Private sVOIP1Nr2 As String
'    Public Property VOIP1Nr2() As String
'        Get
'            Return sVOIP1Nr2
'        End Get
'        Set(ByVal value As String)
'            sVOIP1Nr2 = value
'        End Set
'    End Property

'    Private sVOIP1Nr3 As String
'    Public Property VOIP1Nr3() As String
'        Get
'            Return sVOIP1Nr3
'        End Get
'        Set(ByVal value As String)
'            sVOIP1Nr3 = value
'        End Set
'    End Property

'    Private sVOIP1Nr4 As String
'    Public Property VOIP1Nr4() As String
'        Get
'            Return sVOIP1Nr4
'        End Get
'        Set(ByVal value As String)
'            sVOIP1Nr4 = value
'        End Set
'    End Property

'    Private sVOIP1Nr5 As String
'    Public Property VOIP1Nr5() As String
'        Get
'            Return sVOIP1Nr5
'        End Get
'        Set(ByVal value As String)
'            sVOIP1Nr5 = value
'        End Set
'    End Property

'    Private sVOIP1Nr6 As String
'    Public Property VOIP1Nr6() As String
'        Get
'            Return sVOIP1Nr6
'        End Get
'        Set(ByVal value As String)
'            sVOIP1Nr6 = value
'        End Set
'    End Property

'    Private sVOIP1Nr7 As String
'    Public Property VOIP1Nr7() As String
'        Get
'            Return sVOIP1Nr7
'        End Get
'        Set(ByVal value As String)
'            sVOIP1Nr7 = value
'        End Set
'    End Property

'    Private sVOIP1Nr8 As String
'    Public Property VOIP1Nr8() As String
'        Get
'            Return sVOIP1Nr8
'        End Get
'        Set(ByVal value As String)
'            sVOIP1Nr8 = value
'        End Set
'    End Property

'    Private sVOIP1Nr9 As String
'    Public Property VOIP1Nr9() As String
'        Get
'            Return sVOIP1Nr9
'        End Get
'        Set(ByVal value As String)
'            sVOIP1Nr9 = value
'        End Set
'    End Property
'#End Region
'#Region "VOIP2"
'    Private sVOIP2Nr0 As String
'    Public Property VOIP2Nr0() As String
'        Get
'            Return sVOIP2Nr0
'        End Get
'        Set(ByVal value As String)
'            sVOIP2Nr0 = value
'        End Set
'    End Property

'    Private sVOIP2Nr1 As String
'    Public Property VOIP2Nr1() As String
'        Get
'            Return sVOIP2Nr1
'        End Get
'        Set(ByVal value As String)
'            sVOIP2Nr1 = value
'        End Set
'    End Property

'    Private sVOIP2Nr2 As String
'    Public Property VOIP2Nr2() As String
'        Get
'            Return sVOIP2Nr2
'        End Get
'        Set(ByVal value As String)
'            sVOIP2Nr2 = value
'        End Set
'    End Property

'    Private sVOIP2Nr3 As String
'    Public Property VOIP2Nr3() As String
'        Get
'            Return sVOIP2Nr3
'        End Get
'        Set(ByVal value As String)
'            sVOIP2Nr3 = value
'        End Set
'    End Property

'    Private sVOIP2Nr4 As String
'    Public Property VOIP2Nr4() As String
'        Get
'            Return sVOIP2Nr4
'        End Get
'        Set(ByVal value As String)
'            sVOIP2Nr4 = value
'        End Set
'    End Property

'    Private sVOIP2Nr5 As String
'    Public Property VOIP2Nr5() As String
'        Get
'            Return sVOIP2Nr5
'        End Get
'        Set(ByVal value As String)
'            sVOIP2Nr5 = value
'        End Set
'    End Property

'    Private sVOIP2Nr6 As String
'    Public Property VOIP2Nr6() As String
'        Get
'            Return sVOIP2Nr6
'        End Get
'        Set(ByVal value As String)
'            sVOIP2Nr6 = value
'        End Set
'    End Property

'    Private sVOIP2Nr7 As String
'    Public Property VOIP2Nr7() As String
'        Get
'            Return sVOIP2Nr7
'        End Get
'        Set(ByVal value As String)
'            sVOIP2Nr7 = value
'        End Set
'    End Property

'    Private sVOIP2Nr8 As String
'    Public Property VOIP2Nr8() As String
'        Get
'            Return sVOIP2Nr8
'        End Get
'        Set(ByVal value As String)
'            sVOIP2Nr8 = value
'        End Set
'    End Property

'    Private sVOIP2Nr9 As String
'    Public Property VOIP2Nr9() As String
'        Get
'            Return sVOIP2Nr9
'        End Get
'        Set(ByVal value As String)
'            sVOIP2Nr9 = value
'        End Set
'    End Property
'#End Region
'#Region "VOIP3"
'    Private sVOIP3Nr0 As String
'    Public Property VOIP3Nr0() As String
'        Get
'            Return sVOIP3Nr0
'        End Get
'        Set(ByVal value As String)
'            sVOIP3Nr0 = value
'        End Set
'    End Property

'    Private sVOIP3Nr1 As String
'    Public Property VOIP3Nr1() As String
'        Get
'            Return sVOIP3Nr1
'        End Get
'        Set(ByVal value As String)
'            sVOIP3Nr1 = value
'        End Set
'    End Property

'    Private sVOIP3Nr2 As String
'    Public Property VOIP3Nr2() As String
'        Get
'            Return sVOIP3Nr2
'        End Get
'        Set(ByVal value As String)
'            sVOIP3Nr2 = value
'        End Set
'    End Property

'    Private sVOIP3Nr3 As String
'    Public Property VOIP3Nr3() As String
'        Get
'            Return sVOIP3Nr3
'        End Get
'        Set(ByVal value As String)
'            sVOIP3Nr3 = value
'        End Set
'    End Property

'    Private sVOIP3Nr4 As String
'    Public Property VOIP3Nr4() As String
'        Get
'            Return sVOIP3Nr4
'        End Get
'        Set(ByVal value As String)
'            sVOIP3Nr4 = value
'        End Set
'    End Property

'    Private sVOIP3Nr5 As String
'    Public Property VOIP3Nr5() As String
'        Get
'            Return sVOIP3Nr5
'        End Get
'        Set(ByVal value As String)
'            sVOIP3Nr5 = value
'        End Set
'    End Property

'    Private sVOIP3Nr6 As String
'    Public Property VOIP3Nr6() As String
'        Get
'            Return sVOIP3Nr6
'        End Get
'        Set(ByVal value As String)
'            sVOIP3Nr6 = value
'        End Set
'    End Property

'    Private sVOIP3Nr7 As String
'    Public Property VOIP3Nr7() As String
'        Get
'            Return sVOIP3Nr7
'        End Get
'        Set(ByVal value As String)
'            sVOIP3Nr7 = value
'        End Set
'    End Property

'    Private sVOIP3Nr8 As String
'    Public Property VOIP3Nr8() As String
'        Get
'            Return sVOIP3Nr8
'        End Get
'        Set(ByVal value As String)
'            sVOIP3Nr8 = value
'        End Set
'    End Property

'    Private sVOIP3Nr9 As String
'    Public Property VOIP3Nr9() As String
'        Get
'            Return sVOIP3Nr9
'        End Get
'        Set(ByVal value As String)
'            sVOIP3Nr9 = value
'        End Set
'    End Property
'#End Region
'#Region "VOIP4"
'    Private sVOIP4Nr0 As String
'    Public Property VOIP4Nr0() As String
'        Get
'            Return sVOIP4Nr0
'        End Get
'        Set(ByVal value As String)
'            sVOIP4Nr0 = value
'        End Set
'    End Property

'    Private sVOIP4Nr1 As String
'    Public Property VOIP4Nr1() As String
'        Get
'            Return sVOIP4Nr1
'        End Get
'        Set(ByVal value As String)
'            sVOIP4Nr1 = value
'        End Set
'    End Property

'    Private sVOIP4Nr2 As String
'    Public Property VOIP4Nr2() As String
'        Get
'            Return sVOIP4Nr2
'        End Get
'        Set(ByVal value As String)
'            sVOIP4Nr2 = value
'        End Set
'    End Property

'    Private sVOIP4Nr3 As String
'    Public Property VOIP4Nr3() As String
'        Get
'            Return sVOIP4Nr3
'        End Get
'        Set(ByVal value As String)
'            sVOIP4Nr3 = value
'        End Set
'    End Property

'    Private sVOIP4Nr4 As String
'    Public Property VOIP4Nr4() As String
'        Get
'            Return sVOIP4Nr4
'        End Get
'        Set(ByVal value As String)
'            sVOIP4Nr4 = value
'        End Set
'    End Property

'    Private sVOIP4Nr5 As String
'    Public Property VOIP4Nr5() As String
'        Get
'            Return sVOIP4Nr5
'        End Get
'        Set(ByVal value As String)
'            sVOIP4Nr5 = value
'        End Set
'    End Property

'    Private sVOIP4Nr6 As String
'    Public Property VOIP4Nr6() As String
'        Get
'            Return sVOIP4Nr6
'        End Get
'        Set(ByVal value As String)
'            sVOIP4Nr6 = value
'        End Set
'    End Property

'    Private sVOIP4Nr7 As String
'    Public Property VOIP4Nr7() As String
'        Get
'            Return sVOIP4Nr7
'        End Get
'        Set(ByVal value As String)
'            sVOIP4Nr7 = value
'        End Set
'    End Property

'    Private sVOIP4Nr8 As String
'    Public Property VOIP4Nr8() As String
'        Get
'            Return sVOIP4Nr8
'        End Get
'        Set(ByVal value As String)
'            sVOIP4Nr8 = value
'        End Set
'    End Property

'    Private sVOIP4Nr9 As String
'    Public Property VOIP4Nr9() As String
'        Get
'            Return sVOIP4Nr9
'        End Get
'        Set(ByVal value As String)
'            sVOIP4Nr9 = value
'        End Set
'    End Property
'#End Region
'#Region "VOIP5"
'    Private sVOIP5Nr0 As String
'    Public Property VOIP5Nr0() As String
'        Get
'            Return sVOIP5Nr0
'        End Get
'        Set(ByVal value As String)
'            sVOIP5Nr0 = value
'        End Set
'    End Property

'    Private sVOIP5Nr1 As String
'    Public Property VOIP5Nr1() As String
'        Get
'            Return sVOIP5Nr1
'        End Get
'        Set(ByVal value As String)
'            sVOIP5Nr1 = value
'        End Set
'    End Property

'    Private sVOIP5Nr2 As String
'    Public Property VOIP5Nr2() As String
'        Get
'            Return sVOIP5Nr2
'        End Get
'        Set(ByVal value As String)
'            sVOIP5Nr2 = value
'        End Set
'    End Property

'    Private sVOIP5Nr3 As String
'    Public Property VOIP5Nr3() As String
'        Get
'            Return sVOIP5Nr3
'        End Get
'        Set(ByVal value As String)
'            sVOIP5Nr3 = value
'        End Set
'    End Property

'    Private sVOIP5Nr4 As String
'    Public Property VOIP5Nr4() As String
'        Get
'            Return sVOIP5Nr4
'        End Get
'        Set(ByVal value As String)
'            sVOIP5Nr4 = value
'        End Set
'    End Property

'    Private sVOIP5Nr5 As String
'    Public Property VOIP5Nr5() As String
'        Get
'            Return sVOIP5Nr5
'        End Get
'        Set(ByVal value As String)
'            sVOIP5Nr5 = value
'        End Set
'    End Property

'    Private sVOIP5Nr6 As String
'    Public Property VOIP5Nr6() As String
'        Get
'            Return sVOIP5Nr6
'        End Get
'        Set(ByVal value As String)
'            sVOIP5Nr6 = value
'        End Set
'    End Property

'    Private sVOIP5Nr7 As String
'    Public Property VOIP5Nr7() As String
'        Get
'            Return sVOIP5Nr7
'        End Get
'        Set(ByVal value As String)
'            sVOIP5Nr7 = value
'        End Set
'    End Property

'    Private sVOIP5Nr8 As String
'    Public Property VOIP5Nr8() As String
'        Get
'            Return sVOIP5Nr8
'        End Get
'        Set(ByVal value As String)
'            sVOIP5Nr8 = value
'        End Set
'    End Property

'    Private sVOIP5Nr9 As String
'    Public Property VOIP5Nr9() As String
'        Get
'            Return sVOIP5Nr9
'        End Get
'        Set(ByVal value As String)
'            sVOIP5Nr9 = value
'        End Set
'    End Property
'#End Region
'#Region "VOIP6"
'    Private sVOIP6Nr0 As String
'    Public Property VOIP6Nr0() As String
'        Get
'            Return sVOIP6Nr0
'        End Get
'        Set(ByVal value As String)
'            sVOIP6Nr0 = value
'        End Set
'    End Property

'    Private sVOIP6Nr1 As String
'    Public Property VOIP6Nr1() As String
'        Get
'            Return sVOIP6Nr1
'        End Get
'        Set(ByVal value As String)
'            sVOIP6Nr1 = value
'        End Set
'    End Property

'    Private sVOIP6Nr2 As String
'    Public Property VOIP6Nr2() As String
'        Get
'            Return sVOIP6Nr2
'        End Get
'        Set(ByVal value As String)
'            sVOIP6Nr2 = value
'        End Set
'    End Property

'    Private sVOIP6Nr3 As String
'    Public Property VOIP6Nr3() As String
'        Get
'            Return sVOIP6Nr3
'        End Get
'        Set(ByVal value As String)
'            sVOIP6Nr3 = value
'        End Set
'    End Property

'    Private sVOIP6Nr4 As String
'    Public Property VOIP6Nr4() As String
'        Get
'            Return sVOIP6Nr4
'        End Get
'        Set(ByVal value As String)
'            sVOIP6Nr4 = value
'        End Set
'    End Property

'    Private sVOIP6Nr5 As String
'    Public Property VOIP6Nr5() As String
'        Get
'            Return sVOIP6Nr5
'        End Get
'        Set(ByVal value As String)
'            sVOIP6Nr5 = value
'        End Set
'    End Property

'    Private sVOIP6Nr6 As String
'    Public Property VOIP6Nr6() As String
'        Get
'            Return sVOIP6Nr6
'        End Get
'        Set(ByVal value As String)
'            sVOIP6Nr6 = value
'        End Set
'    End Property

'    Private sVOIP6Nr7 As String
'    Public Property VOIP6Nr7() As String
'        Get
'            Return sVOIP6Nr7
'        End Get
'        Set(ByVal value As String)
'            sVOIP6Nr7 = value
'        End Set
'    End Property

'    Private sVOIP6Nr8 As String
'    Public Property VOIP6Nr8() As String
'        Get
'            Return sVOIP6Nr8
'        End Get
'        Set(ByVal value As String)
'            sVOIP6Nr8 = value
'        End Set
'    End Property

'    Private sVOIP6Nr9 As String
'    Public Property VOIP6Nr9() As String
'        Get
'            Return sVOIP6Nr9
'        End Get
'        Set(ByVal value As String)
'            sVOIP6Nr9 = value
'        End Set
'    End Property
'#End Region
'#Region "VOIP7"
'    Private sVOIP7Nr0 As String
'    Public Property VOIP7Nr0() As String
'        Get
'            Return sVOIP7Nr0
'        End Get
'        Set(ByVal value As String)
'            sVOIP7Nr0 = value
'        End Set
'    End Property

'    Private sVOIP7Nr1 As String
'    Public Property VOIP7Nr1() As String
'        Get
'            Return sVOIP7Nr1
'        End Get
'        Set(ByVal value As String)
'            sVOIP7Nr1 = value
'        End Set
'    End Property

'    Private sVOIP7Nr2 As String
'    Public Property VOIP7Nr2() As String
'        Get
'            Return sVOIP7Nr2
'        End Get
'        Set(ByVal value As String)
'            sVOIP7Nr2 = value
'        End Set
'    End Property

'    Private sVOIP7Nr3 As String
'    Public Property VOIP7Nr3() As String
'        Get
'            Return sVOIP7Nr3
'        End Get
'        Set(ByVal value As String)
'            sVOIP7Nr3 = value
'        End Set
'    End Property

'    Private sVOIP7Nr4 As String
'    Public Property VOIP7Nr4() As String
'        Get
'            Return sVOIP7Nr4
'        End Get
'        Set(ByVal value As String)
'            sVOIP7Nr4 = value
'        End Set
'    End Property

'    Private sVOIP7Nr5 As String
'    Public Property VOIP7Nr5() As String
'        Get
'            Return sVOIP7Nr5
'        End Get
'        Set(ByVal value As String)
'            sVOIP7Nr5 = value
'        End Set
'    End Property

'    Private sVOIP7Nr6 As String
'    Public Property VOIP7Nr6() As String
'        Get
'            Return sVOIP7Nr6
'        End Get
'        Set(ByVal value As String)
'            sVOIP7Nr6 = value
'        End Set
'    End Property

'    Private sVOIP7Nr7 As String
'    Public Property VOIP7Nr7() As String
'        Get
'            Return sVOIP7Nr7
'        End Get
'        Set(ByVal value As String)
'            sVOIP7Nr7 = value
'        End Set
'    End Property

'    Private sVOIP7Nr8 As String
'    Public Property VOIP7Nr8() As String
'        Get
'            Return sVOIP7Nr8
'        End Get
'        Set(ByVal value As String)
'            sVOIP7Nr8 = value
'        End Set
'    End Property

'    Private sVOIP7Nr9 As String
'    Public Property VOIP7Nr9() As String
'        Get
'            Return sVOIP7Nr9
'        End Get
'        Set(ByVal value As String)
'            sVOIP7Nr9 = value
'        End Set
'    End Property
'#End Region
'#Region "VOIP8"
'    Private sVOIP8Nr0 As String
'    Public Property VOIP8Nr0() As String
'        Get
'            Return sVOIP8Nr0
'        End Get
'        Set(ByVal value As String)
'            sVOIP8Nr0 = value
'        End Set
'    End Property

'    Private sVOIP8Nr1 As String
'    Public Property VOIP8Nr1() As String
'        Get
'            Return sVOIP8Nr1
'        End Get
'        Set(ByVal value As String)
'            sVOIP8Nr1 = value
'        End Set
'    End Property

'    Private sVOIP8Nr2 As String
'    Public Property VOIP8Nr2() As String
'        Get
'            Return sVOIP8Nr2
'        End Get
'        Set(ByVal value As String)
'            sVOIP8Nr2 = value
'        End Set
'    End Property

'    Private sVOIP8Nr3 As String
'    Public Property VOIP8Nr3() As String
'        Get
'            Return sVOIP8Nr3
'        End Get
'        Set(ByVal value As String)
'            sVOIP8Nr3 = value
'        End Set
'    End Property

'    Private sVOIP8Nr4 As String
'    Public Property VOIP8Nr4() As String
'        Get
'            Return sVOIP8Nr4
'        End Get
'        Set(ByVal value As String)
'            sVOIP8Nr4 = value
'        End Set
'    End Property

'    Private sVOIP8Nr5 As String
'    Public Property VOIP8Nr5() As String
'        Get
'            Return sVOIP8Nr5
'        End Get
'        Set(ByVal value As String)
'            sVOIP8Nr5 = value
'        End Set
'    End Property

'    Private sVOIP8Nr6 As String
'    Public Property VOIP8Nr6() As String
'        Get
'            Return sVOIP8Nr6
'        End Get
'        Set(ByVal value As String)
'            sVOIP8Nr6 = value
'        End Set
'    End Property

'    Private sVOIP8Nr7 As String
'    Public Property VOIP8Nr7() As String
'        Get
'            Return sVOIP8Nr7
'        End Get
'        Set(ByVal value As String)
'            sVOIP8Nr7 = value
'        End Set
'    End Property

'    Private sVOIP8Nr8 As String
'    Public Property VOIP8Nr8() As String
'        Get
'            Return sVOIP8Nr8
'        End Get
'        Set(ByVal value As String)
'            sVOIP8Nr8 = value
'        End Set
'    End Property

'    Private sVOIP8Nr9 As String
'    Public Property VOIP8Nr9() As String
'        Get
'            Return sVOIP8Nr9
'        End Get
'        Set(ByVal value As String)
'            sVOIP8Nr9 = value
'        End Set
'    End Property
'#End Region
'#Region "VOIP9"
'    Private sVOIP9Nr0 As String
'    Public Property VOIP9Nr0() As String
'        Get
'            Return sVOIP9Nr0
'        End Get
'        Set(ByVal value As String)
'            sVOIP9Nr0 = value
'        End Set
'    End Property

'    Private sVOIP9Nr1 As String
'    Public Property VOIP9Nr1() As String
'        Get
'            Return sVOIP9Nr1
'        End Get
'        Set(ByVal value As String)
'            sVOIP9Nr1 = value
'        End Set
'    End Property

'    Private sVOIP9Nr2 As String
'    Public Property VOIP9Nr2() As String
'        Get
'            Return sVOIP9Nr2
'        End Get
'        Set(ByVal value As String)
'            sVOIP9Nr2 = value
'        End Set
'    End Property

'    Private sVOIP9Nr3 As String
'    Public Property VOIP9Nr3() As String
'        Get
'            Return sVOIP9Nr3
'        End Get
'        Set(ByVal value As String)
'            sVOIP9Nr3 = value
'        End Set
'    End Property

'    Private sVOIP9Nr4 As String
'    Public Property VOIP9Nr4() As String
'        Get
'            Return sVOIP9Nr4
'        End Get
'        Set(ByVal value As String)
'            sVOIP9Nr4 = value
'        End Set
'    End Property

'    Private sVOIP9Nr5 As String
'    Public Property VOIP9Nr5() As String
'        Get
'            Return sVOIP9Nr5
'        End Get
'        Set(ByVal value As String)
'            sVOIP9Nr5 = value
'        End Set
'    End Property

'    Private sVOIP9Nr6 As String
'    Public Property VOIP9Nr6() As String
'        Get
'            Return sVOIP9Nr6
'        End Get
'        Set(ByVal value As String)
'            sVOIP9Nr6 = value
'        End Set
'    End Property

'    Private sVOIP9Nr7 As String
'    Public Property VOIP9Nr7() As String
'        Get
'            Return sVOIP9Nr7
'        End Get
'        Set(ByVal value As String)
'            sVOIP9Nr7 = value
'        End Set
'    End Property

'    Private sVOIP9Nr8 As String
'    Public Property VOIP9Nr8() As String
'        Get
'            Return sVOIP9Nr8
'        End Get
'        Set(ByVal value As String)
'            sVOIP9Nr8 = value
'        End Set
'    End Property

'    Private sVOIP9Nr9 As String
'    Public Property VOIP9Nr9() As String
'        Get
'            Return sVOIP9Nr9
'        End Get
'        Set(ByVal value As String)
'            sVOIP9Nr9 = value
'        End Set
'    End Property
'#End Region
'End Class

''' <summary>
''' Klasse einer Liste mit 10 Telefonnummernfelder
''' </summary>
Public Class TelNrList

    ''' <summary>
    ''' Gibt die Telefonnummern als Array zurück. Leere Felder und doppelte Werte werden nicht heraus gefiltert.
    ''' </summary>
    ''' <returns>String-Array</returns>
    Public Function ToArray() As String()
        ToArray = {TelNr0, TelNr1, TelNr2, TelNr3, TelNr4, TelNr5, TelNr6, TelNr7, TelNr8, TelNr9}
    End Function

    ''' <summary>
    ''' Gibt die Telefonnummern als Array zurück. Leere Felder und doppelte Werte werden heraus gefiltert.
    ''' </summary>
    ''' <returns>String-Array</returns>
    Public Function ToDistinctArray() As String()
        ' Doppelte entfernen
        ToDistinctArray = (From x In ToArray() Select x Distinct).ToArray
        ' Leere entfernen
        ToDistinctArray = (From x In ToDistinctArray Where Not x Like DataProvider.P_Def_LeerString Select x).ToArray
    End Function

    ''' <summary>
    ''' Gibt den niedrigsten verfügbaren Feldindex für die angegebene Dimension eines Arrays zurück.
    ''' </summary>
    ''' <returns>Integer. Der niedrigste Wert, den der Feldindex für die angegebene Dimension enthalten kann. 
    ''' LBound gibt stets 0 (null) zurück, sofern Array initialisiert wurde, auch wenn das Array keine Elemente enthält, beispielsweise wenn es eine Zeichenfolge mit der Länge 0 (null) ist.
    ''' Wenn Array den Wert Nothing hat, löst LBound eine ArgumentNullException-Ausnahme aus.</returns>
    Public ReadOnly Property LBound As Integer
        Get
            Return ToArray.GetLowerBound(0)
        End Get
    End Property

    ''' <summary>
    ''' ibt den höchsten verfügbaren Feldindex für die angegebene Dimension eines Arrays zurück.
    ''' </summary>
    ''' <returns>Integer. Der höchste Wert, den der Feldindex für die angegebene Dimension enthalten kann. Wenn Array nur ein Element enthält, gibt UBound 0 (null) zurück. Enthält Array keine Elemente, z. B. wenn es sich um eine Zeichenfolge mit der Länge 0 (null) handelt, dann gibt UBound -1 zurück.</returns>
    Public ReadOnly Property UBound As Integer
        Get
            Return ToArray.GetUpperBound(0)
        End Get
    End Property

    Public Property Item(ByVal idx As Integer) As String
        Get
            Select Case idx
                Case 0
                    Item = sTelNr0
                Case 1
                    Item = sTelNr1
                Case 2
                    Item = sTelNr2
                Case 3
                    Item = sTelNr3
                Case 4
                    Item = sTelNr4
                Case 5
                    Item = sTelNr5
                Case 6
                    Item = sTelNr6
                Case 7
                    Item = sTelNr7
                Case 8
                    Item = sTelNr5
                Case 9
                    Item = sTelNr6
                Case Else
                    Item = DataProvider.P_Def_LeerString
            End Select
            Return Item
        End Get
        Set(ByVal value As String)
            Select Case idx
                Case 0
                    sTelNr0 = value
                Case 1
                    sTelNr0 = value
                Case 2
                    sTelNr0 = value
                Case 3
                    sTelNr0 = value
                Case 4
                    sTelNr0 = value
                Case 5
                    sTelNr0 = value
                Case 6
                    sTelNr0 = value
                Case 7
                    sTelNr0 = value
                Case 8
                    sTelNr0 = value
                Case 9
                    sTelNr0 = value
            End Select
        End Set
    End Property

    Private sTelNr0 As String
    Public Property TelNr0() As String
        Get
            Return sTelNr0
        End Get
        Set(ByVal value As String)
            sTelNr0 = value
        End Set
    End Property

    Private sTelNr1 As String
    Public Property TelNr1() As String
        Get
            Return sTelNr1
        End Get
        Set(ByVal value As String)
            sTelNr1 = value
        End Set
    End Property

    Private sTelNr2 As String
    Public Property TelNr2() As String
        Get
            Return sTelNr2
        End Get
        Set(ByVal value As String)
            sTelNr2 = value
        End Set
    End Property

    Private sTelNr3 As String
    Public Property TelNr3() As String
        Get
            Return sTelNr3
        End Get
        Set(ByVal value As String)
            sTelNr3 = value
        End Set
    End Property

    Private sTelNr4 As String
    Public Property TelNr4() As String
        Get
            Return sTelNr4
        End Get
        Set(ByVal value As String)
            sTelNr4 = value
        End Set
    End Property

    Private sTelNr5 As String
    Public Property TelNr5() As String
        Get
            Return sTelNr5
        End Get
        Set(ByVal value As String)
            sTelNr5 = value
        End Set
    End Property

    Private sTelNr6 As String
    Public Property TelNr6() As String
        Get
            Return sTelNr6
        End Get
        Set(ByVal value As String)
            sTelNr6 = value
        End Set
    End Property

    Private sTelNr7 As String
    Public Property TelNr7() As String
        Get
            Return sTelNr7
        End Get
        Set(ByVal value As String)
            sTelNr7 = value
        End Set
    End Property

    Private sTelNr8 As String
    Public Property TelNr8() As String
        Get
            Return sTelNr8
        End Get
        Set(ByVal value As String)
            sTelNr8 = value
        End Set
    End Property

    Private sTelNr9 As String
    Public Property TelNr9() As String
        Get
            Return sTelNr9
        End Get
        Set(ByVal value As String)
            sTelNr9 = value
        End Set
    End Property
End Class

Public Class FONNr
    Private sNumber As String
    Public Property Number() As String
        Get
            Return sNumber
        End Get
        Set(ByVal value As String)
            sNumber = value
        End Set
    End Property
End Class

Public Class FritzBoxJSONTelefoneFONNr
    Private nFONNr() As FONNr
    Public Property FONNr() As FONNr()
        Get
            Return nFONNr
        End Get
        Set(ByVal value As FONNr())
            nFONNr = value
        End Set
    End Property
End Class

''' <summary>
''' Klasse für den Upload von Kontakten
''' </summary>
Public Class Tomark
End Class

''' <summary>
''' Klasse für den Upload von Kontakten
''' </summary>
Public Class FritzBoxJSONUploadResult
    Private ntomark As Tomark
    Public Property tomark() As Tomark
        Get
            Return ntomark
        End Get
        Set(ByVal value As Tomark)
            ntomark = value
        End Set
    End Property

    Private svalidate As String
    Public Property validate() As String
        Get
            Return svalidate
        End Get
        Set(ByVal value As String)
            svalidate = value
        End Set
    End Property

    Private sresult As String
    Public Property result() As String
        Get
            Return sresult
        End Get
        Set(ByVal value As String)
            sresult = value
        End Set
    End Property

    Private bok As Boolean
    Public Property ok() As Boolean
        Get
            Return bok
        End Get
        Set(ByVal value As Boolean)
            bok = value
        End Set
    End Property
End Class

Public Class JSON

    Public Function GetFirstValues(ByVal strJSON As String) As FritzBoxJSONTelNrT1
        Return JsonConvert.DeserializeObject(Of FritzBoxJSONTelNrT1)(strJSON)
    End Function

    Public Function GetThirdValues(ByVal strJSON As String) As FritzBoxJSONTelefone1
        Return JsonConvert.DeserializeObject(Of FritzBoxJSONTelefone1)(strJSON)
    End Function

    Public Function GetForthValues(ByVal strJSON As String) As FritzBoxJSONTelefone2
        Return JsonConvert.DeserializeObject(Of FritzBoxJSONTelefone2)(strJSON)
    End Function

    Public Function GetFifthValues(ByVal strJSON As String) As FritzBoxJSONTelefoneFONNr
        Return JsonConvert.DeserializeObject(Of FritzBoxJSONTelefoneFONNr)(strJSON)
    End Function

    Public Function GetTelNrListJSON(ByVal strJSON As String) As TelNrList
        Return JsonConvert.DeserializeObject(Of TelNrList)(strJSON)
    End Function

    Public Function GetUploadResult(ByVal strJSON As String) As FritzBoxJSONUploadResult
        Return JsonConvert.DeserializeObject(Of FritzBoxJSONUploadResult)(strJSON)
    End Function

End Class