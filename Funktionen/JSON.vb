Imports Newtonsoft.Json

Public Class TAMEntry
    Public Property Active As String
    Public Property Name As String
    Public Property Display As String
    Public Property MSNBitmap As String
End Class

Public Class SIPEntry
    Public Property activated As String
    Public Property displayname As String
    Public Property registrar As String
    Public Property outboundproxy As String
    Public Property providername As String
    Public Property ID As String
    Public Property gui_readonly As String
    Public Property webui_trunk_id As String
End Class

Public Class MSNEntry
    Public Property Name As String
    Public Property Fax As String
    Public Property GroupCall As String
    Public Property AllIncomingCalls As String
    Public Property OutDialing As String
End Class

Public Class VOIPEntry
    Public Property enabled As String
    Public Property Name As String
    Public Property RingOnAllMSNs As String
End Class

Friend Class FoncontrolUserList
    Friend Property Name As String
    Friend Property Type As String
    Friend Property Intern As String
    Friend Property Id As String
End Class

Friend Class FoncontrolUserNList
    Friend Property Number As String
End Class

Friend Class VoipExtensionList
    Friend Property enabled As String
    Friend Property Name As String
    Friend Property RingOnAllMSNs As String
End Class

Public Class DECTNr
    Public Property Number As String
End Class

Public Class DECTEntry
    Public Property Name As String
    Public Property Type As String
    Public Property Intern As String
    Public Property Id As String
End Class

Public Class FritzBoxJSONTelefone1
    Public TAM() As TAMEntry
    Public DECT() As DECTEntry
    Public FON() As MSNEntry
    Public VOIP() As VOIPEntry
    Public Property S0Name1 As String
    Public Property S0Name2 As String
    Public Property S0Name3 As String
    Public Property S0Name4 As String
    Public Property S0Name5 As String
    Public Property S0Name6 As String
    Public Property S0Name7 As String
    Public Property S0Name8 As String
End Class

Public Class FritzBoxJSONTelefone2
    Public Property S0TelNr1 As String
    Public Property S0TelNr2 As String
    Public Property S0TelNr3 As String
    Public Property S0TelNr4 As String
    Public Property S0TelNr5 As String
    Public Property S0TelNr6 As String
    Public Property S0TelNr7 As String
    Public Property S0TelNr8 As String
    Public Property S0Type1 As String
    Public Property S0Type2 As String
    Public Property S0Type3 As String
    Public Property S0Type4 As String
    Public Property S0Type5 As String
    Public Property S0Type6 As String
    Public Property S0Type7 As String
    Public Property S0Type8 As String
    Public Property DECT0Nr As DECTNr()
    Public Property DECT1Nr As DECTNr()
    Public Property DECT2Nr As DECTNr()
    Public Property DECT3Nr As DECTNr()
    Public Property DECT4Nr As DECTNr()
    Public Property FaxMailActive As String
    Public Property MobileName As String
    Public Property DECT0RingOnAllMSNs As String
    Public Property DECT1RingOnAllMSNs As String
    Public Property DECT2RingOnAllMSNs As String
    Public Property DECT3RingOnAllMSNs As String
    Public Property DECT4RingOnAllMSNs As String
End Class

Public Class FritzBoxJSONTelNrT1
    Public Property POTS As String
    Public Property Mobile As String
    Public Property Port0Name As String
    Public Property Port1Name As String
    Public Property Port2Name As String
    Public Property TAM0 As String
    Public Property FAX0 As String
    Public Property MSN0 As String
    Public Property VOIP0Enabled As String
    Public Property TAM1 As String
    Public Property FAX1 As String
    Public Property MSN1 As String
    Public Property S01Name As String
    Public Property S01Number As String
    Public Property VOIP1Enabled As String
    Public Property TAM2 As String
    Public Property FAX2 As String
    Public Property MSN2 As String
    Public Property S02Name As String
    Public Property S02Number As String
    Public Property VOIP2Enabled As String
    Public Property TAM3 As String
    Public Property FAX3 As String
    Public Property MSN3 As String
    Public Property S03Name As String
    Public Property S03Number As String
    Public Property VOIP3Enabled As String
    Public Property TAM4 As String
    Public Property FAX4 As String
    Public Property MSN4 As String
    Public Property S04Name As String
    Public Property S04Number As String
    Public Property VOIP4Enabled As String
    Public Property TAM5 As String
    Public Property FAX5 As String
    Public Property MSN5 As String
    Public Property S05Name As String
    Public Property S05Number As String
    Public Property VOIP5Enabled As String
    Public Property TAM6 As String
    Public Property FAX6 As String
    Public Property MSN6 As String
    Public Property S06Name As String
    Public Property S06Number As String
    Public Property VOIP6Enabled As String
    Public Property TAM7 As String
    Public Property FAX7 As String
    Public Property MSN7 As String
    Public Property S07Name As String
    Public Property S07Number As String
    Public Property VOIP7Enabled As String
    Public Property TAM8 As String
    Public Property FAX8 As String
    Public Property MSN8 As String
    Public Property S08Name As String
    Public Property S08Number As String
    Public Property VOIP8Enabled As String
    Public Property TAM9 As String
    Public Property FAX9 As String
    Public Property MSN9 As String
    Public Property VOIP9Enabled As String
    Public Property SIP As SIPEntry()
End Class

Public Class FritzBoxJSONTelNrT2
    Public Property MSN0Nr0 As String
    Public Property MSN0Nr1 As String
    Public Property MSN0Nr2 As String
    Public Property MSN0Nr3 As String
    Public Property MSN0Nr4 As String
    Public Property MSN0Nr5 As String
    Public Property MSN0Nr6 As String
    Public Property MSN0Nr7 As String
    Public Property MSN0Nr8 As String
    Public Property MSN0Nr9 As String

    Public Property MSN1Nr0 As String
    Public Property MSN1Nr1 As String
    Public Property MSN1Nr2 As String
    Public Property MSN1Nr3 As String
    Public Property MSN1Nr4 As String
    Public Property MSN1Nr5 As String
    Public Property MSN1Nr6 As String
    Public Property MSN1Nr7 As String
    Public Property MSN1Nr8 As String
    Public Property MSN1Nr9 As String

    Public Property MSN2Nr0 As String
    Public Property MSN2Nr1 As String
    Public Property MSN2Nr2 As String
    Public Property MSN2Nr3 As String
    Public Property MSN2Nr4 As String
    Public Property MSN2Nr5 As String
    Public Property MSN2Nr6 As String
    Public Property MSN2Nr7 As String
    Public Property MSN2Nr8 As String
    Public Property MSN2Nr9 As String

    Public Property VOIP0Nr0 As String
    Public Property VOIP0Nr1 As String
    Public Property VOIP0Nr2 As String
    Public Property VOIP0Nr3 As String
    Public Property VOIP0Nr4 As String
    Public Property VOIP0Nr5 As String
    Public Property VOIP0Nr6 As String
    Public Property VOIP0Nr7 As String
    Public Property VOIP0Nr8 As String
    Public Property VOIP0Nr9 As String

    Public Property VOIP1Nr0 As String
    Public Property VOIP1Nr1 As String
    Public Property VOIP1Nr2 As String
    Public Property VOIP1Nr3 As String
    Public Property VOIP1Nr4 As String
    Public Property VOIP1Nr5 As String
    Public Property VOIP1Nr6 As String
    Public Property VOIP1Nr7 As String
    Public Property VOIP1Nr8 As String
    Public Property VOIP1Nr9 As String

    Public Property VOIP2Nr0 As String
    Public Property VOIP2Nr1 As String
    Public Property VOIP2Nr2 As String
    Public Property VOIP2Nr3 As String
    Public Property VOIP2Nr4 As String
    Public Property VOIP2Nr5 As String
    Public Property VOIP2Nr6 As String
    Public Property VOIP2Nr7 As String
    Public Property VOIP2Nr8 As String
    Public Property VOIP2Nr9 As String

    Public Property VOIP3Nr0 As String
    Public Property VOIP3Nr1 As String
    Public Property VOIP3Nr2 As String
    Public Property VOIP3Nr3 As String
    Public Property VOIP3Nr4 As String
    Public Property VOIP3Nr5 As String
    Public Property VOIP3Nr6 As String
    Public Property VOIP3Nr7 As String
    Public Property VOIP3Nr8 As String
    Public Property VOIP3Nr9 As String

    Public Property VOIP4Nr0 As String
    Public Property VOIP4Nr1 As String
    Public Property VOIP4Nr2 As String
    Public Property VOIP4Nr3 As String
    Public Property VOIP4Nr4 As String
    Public Property VOIP4Nr5 As String
    Public Property VOIP4Nr6 As String
    Public Property VOIP4Nr7 As String
    Public Property VOIP4Nr8 As String
    Public Property VOIP4Nr9 As String

    Public Property VOIP5Nr0 As String
    Public Property VOIP5Nr1 As String
    Public Property VOIP5Nr2 As String
    Public Property VOIP5Nr3 As String
    Public Property VOIP5Nr4 As String
    Public Property VOIP5Nr5 As String
    Public Property VOIP5Nr6 As String
    Public Property VOIP5Nr7 As String
    Public Property VOIP5Nr8 As String
    Public Property VOIP5Nr9 As String

    Public Property VOIP6Nr0 As String
    Public Property VOIP6Nr1 As String
    Public Property VOIP6Nr2 As String
    Public Property VOIP6Nr3 As String
    Public Property VOIP6Nr4 As String
    Public Property VOIP6Nr5 As String
    Public Property VOIP6Nr6 As String
    Public Property VOIP6Nr7 As String
    Public Property VOIP6Nr8 As String
    Public Property VOIP6Nr9 As String

    Public Property VOIP7Nr0 As String
    Public Property VOIP7Nr1 As String
    Public Property VOIP7Nr2 As String
    Public Property VOIP7Nr3 As String
    Public Property VOIP7Nr4 As String
    Public Property VOIP7Nr5 As String
    Public Property VOIP7Nr6 As String
    Public Property VOIP7Nr7 As String
    Public Property VOIP7Nr8 As String
    Public Property VOIP7Nr9 As String

    Public Property VOIP8Nr0 As String
    Public Property VOIP8Nr1 As String
    Public Property VOIP8Nr2 As String
    Public Property VOIP8Nr3 As String
    Public Property VOIP8Nr4 As String
    Public Property VOIP8Nr5 As String
    Public Property VOIP8Nr6 As String
    Public Property VOIP8Nr7 As String
    Public Property VOIP8Nr8 As String
    Public Property VOIP8Nr9 As String

    Public Property VOIP9Nr0 As String
    Public Property VOIP9Nr1 As String
    Public Property VOIP9Nr2 As String
    Public Property VOIP9Nr3 As String
    Public Property VOIP9Nr4 As String
    Public Property VOIP9Nr5 As String
    Public Property VOIP9Nr6 As String
    Public Property VOIP9Nr7 As String
    Public Property VOIP9Nr8 As String
    Public Property VOIP9Nr9 As String
End Class

Public Class FONNr
    Public Property Number As String
End Class

Public Class FritzBoxJSONTelefoneFONNr
    Public Property FONNr As FONNr()
End Class

Public Class JSON

    Public Function GetFirstValues(ByVal strJSON As String) As FritzBoxJSONTelNrT1
        Return JsonConvert.DeserializeObject(Of FritzBoxJSONTelNrT1)(strJSON)
    End Function

    Public Function GetSecondValues(ByVal strJSON As String) As FritzBoxJSONTelNrT2
        Return JsonConvert.DeserializeObject(Of FritzBoxJSONTelNrT2)(strJSON)
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

End Class
