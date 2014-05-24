Imports System.Text
Imports System.Xml
Imports System.Threading
Imports System.ComponentModel

Public Class FritzBox
    Implements IDisposable

    Private C_DP As DataProvider
    Private C_Crypt As MyRijndael
    Private C_hf As Helfer

    Private FBFehler As Boolean
    Private FBEncoding As System.Text.Encoding = Encoding.UTF8

    Private tb As New System.Windows.Forms.TextBox
    Private EventProvider As IEventProvider

    Private bValSpeichereDaten As Boolean = True

    Private sSID As String

#Region "Properties"
    Friend Property P_SpeichereDaten() As Boolean
        Get
            Return bValSpeichereDaten
        End Get
        Set(ByVal value As Boolean)
            bValSpeichereDaten = value
        End Set
    End Property
#End Region

    Public Sub New(ByVal xmlKlasse As DataProvider, _
                   ByVal HelferKlasse As Helfer, _
                   ByVal CryptKlasse As MyRijndael)


        C_DP = xmlKlasse
        C_hf = HelferKlasse
        C_hf.KeyChange()
        C_Crypt = CryptKlasse

        sSID = C_DP.P_Def_SessionID  ' Startwert: Ungültige SID

        If C_DP.P_EncodeingFritzBox = C_DP.P_Def_ErrorMinusOne_String Then
            Dim Rückgabe As String
            Rückgabe = C_hf.httpGET("http://" & C_hf.ValidIP(C_DP.P_TBFBAdr), FBEncoding, FBFehler)
            If Not FBFehler Then
                FBEncoding = C_hf.GetEncoding(C_hf.StringEntnehmen(Rückgabe, "charset=", """>"))
                C_DP.P_EncodeingFritzBox = FBEncoding.HeaderName
                C_DP.SpeichereXMLDatei()
            Else
                C_hf.LogFile("FBError (FritzBox.New): " & Err.Number & " - " & Err.Description & " - " & "http://" & C_hf.ValidIP(C_DP.P_TBFBAdr))
            End If
        Else
            FBEncoding = C_hf.GetEncoding(C_DP.P_EncodeingFritzBox)
        End If
    End Sub

#Region "Login & Logout"
    Public Function FBLogIn(ByRef LuaLogin As Boolean, Optional ByVal InpupBenutzer As String = "", Optional ByVal InpupPasswort As String = "-1") As String
        Dim sLink As String
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

        sLink = "http://" & C_hf.ValidIP(C_DP.P_TBFBAdr) & "/login_sid.lua?sid=" & sSID
        slogin_xml = C_hf.httpGET(sLink, FBEncoding, FBFehler)

        If InStr(slogin_xml, "BlockTime", CompareMethod.Text) = 0 Then
            sLink = "http://" & C_hf.ValidIP(C_DP.P_TBFBAdr) & "/cgi-bin/webcm?getpage=../html/login_sid.xml&sid=" & sSID
            slogin_xml = C_hf.httpGET(sLink, FBEncoding, FBFehler)
        End If

        If Not FBFehler Then
            If InStr(slogin_xml, "FRITZ!Box Anmeldung", CompareMethod.Text) = 0 And Not Len(slogin_xml) = 0 Then

                If Not InpupPasswort = C_DP.P_Def_ErrorMinusOne_String Then
                    C_DP.P_TBPasswort = C_Crypt.EncryptString128Bit(InpupPasswort, C_DP.P_Def_PassWordDecryptionKey)
                    C_DP.P_TBBenutzer = InpupBenutzer
                    C_DP.SaveSettingsVBA("Zugang", C_DP.P_Def_PassWordDecryptionKey)
                    C_hf.KeyChange()
                End If

                Dim sBlockTime As String
                Dim sChallenge As String
                Dim sFBBenutzer As String = C_DP.P_TBBenutzer
                Dim sFBPasswort As String = C_DP.P_TBPasswort
                Dim sFormData As String
                Dim sResponse As String
                Dim sSIDResponse As String
                Dim sZugang As String = C_DP.GetSettingsVBA("Zugang", C_DP.P_Def_ErrorMinusOne_String)
                Dim XMLDocLogin As New XmlDocument()

                With XMLDocLogin
                    .LoadXml(slogin_xml)

                    If .Item("SessionInfo").Item("SID").InnerText() = C_DP.P_Def_SessionID Then
                        sChallenge = .Item("SessionInfo").Item("Challenge").InnerText()

                        With C_Crypt
                            sSIDResponse = String.Concat(sChallenge, "-", .getMd5Hash(String.Concat(sChallenge, "-", .DecryptString128Bit(sFBPasswort, sZugang)), Encoding.Unicode, True))
                        End With
                        If P_SpeichereDaten Then PushStatus("Challenge: " & sChallenge & vbNewLine & "SIDResponse: " & sSIDResponse)

                        If .InnerXml.Contains("Rights") Then
                            ' Lua Login ab Firmware xxx.05.29 / xxx.05.5x
                            sBlockTime = .Item("SessionInfo").Item("BlockTime").InnerText()
                            If sBlockTime = C_DP.P_Def_StringNull Then ' "0"
                                sLink = "http://" & C_hf.ValidIP(C_DP.P_TBFBAdr) & "/login_sid.lua?username=" & sFBBenutzer & "&response=" & sSIDResponse
                                sResponse = C_hf.httpGET(sLink, FBEncoding, FBFehler)
                                If Not FBFehler Then
                                    LuaLogin = True
                                Else
                                    C_hf.LogFile("FBError (FBLogin): " & Err.Number & " - " & Err.Description & " - " & sLink)
                                End If
                            Else
                                C_hf.FBDB_MsgBox("Die Fritz!Box lässt keinen weiteren Anmeldeversuch in den nächsten " & sBlockTime & " Sekunden zu.  Versuchen Sie es später erneut.", MsgBoxStyle.Critical, "FBLogin")
                                Return C_DP.P_Def_SessionID
                            End If
                        Else
                            ' Alter Login von Firmware xxx.04.76 bis Firmware xxx.05.28
                            If CBool(.Item("SessionInfo").Item("iswriteaccess").InnerText) Then
                                C_hf.LogFile("Die Fritz!Box benötigt kein Passwort. Das AddIn wird nicht funktionieren.")
                                Return .Item("SessionInfo").Item("SID").InnerText()
                            End If

                            sLink = "http://" & C_hf.ValidIP(C_DP.P_TBFBAdr) & "/cgi-bin/webcm"
                            sFormData = "getpage=../html/login_sid.xml&login:command/response=" + sSIDResponse
                            sResponse = C_hf.httpPOST(sLink, sFormData, FBEncoding)

                            LuaLogin = False
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

                        sSID = .Item("SessionInfo").Item("SID").InnerText()

                        If Not sSID = C_DP.P_Def_SessionID Then
                            If LuaLogin Then
                                If Not C_hf.IsOneOf("BoxAdmin", Split(.SelectSingleNode("//Rights").InnerText, "2")) Then
                                    C_hf.LogFile("Es fehlt die Berechtigung für den Zugriff auf die Fritz!Box. Benutzer: " & sFBBenutzer)
                                    FBLogOut(sSID)
                                    sSID = C_DP.P_Def_SessionID
                                End If
                                C_DP.Write("Optionen", sFBBenutzer, CStr(IIf(sSID = C_DP.P_Def_SessionID, 0, 2)))
                            End If
                        Else
                            C_hf.LogFile("Die Anmeldedaten sind falsch." & sSID)
                        End If

                    ElseIf .Item("SessionInfo").Item("SID").InnerText() = sSID Then
                        C_hf.LogFile("Eine gültige SessionID ist bereits vorhanden: " & sSID)
                    End If
                End With
                XMLDocLogin = Nothing
            End If
        Else
            C_hf.LogFile("FBError (FBLogin): " & Err.Number & " - " & Err.Description & " - " & sLink)
        End If
        Return sSID
    End Function

    Public Function FBLogOut(ByRef sSID As String) As Boolean
        ' Die Komplementärfunktion zu FBLogin. Beendet die Session, indem ein Logout durchgeführt wird.

        Dim sLink As String
        Dim Response As String
        Dim tmpstr As String
        Dim xml As New XmlDocument()

        sLink = "http://" & C_hf.ValidIP(C_DP.P_TBFBAdr) & "/login_sid.lua?sid=" & sSID
        Response = C_hf.httpGET(sLink, FBEncoding, FBFehler)
        If Not FBFehler Then
            With xml
                .LoadXml(Response)
                If .InnerXml.Contains("Rights") Then
                    sLink = "http://" & C_hf.ValidIP(C_DP.P_TBFBAdr) & "/home/home.lua?sid=" & sSID & "&logout=1"
                Else
                    sLink = "http://" & C_hf.ValidIP(C_DP.P_TBFBAdr) & "/logout.lua?sid=" & sSID
                End If
            End With
            xml = Nothing
            Response = C_hf.httpGET(sLink, FBEncoding, FBFehler)
            C_hf.KeyChange()
            If Not FBFehler Then
                If Not InStr(Response, "Sie haben sich erfolgreich von der FRITZ!Box abgemeldet.", CompareMethod.Text) = 0 Or _
                    Not InStr(Response, "Sie haben sich erfolgreich von der Benutzeroberfläche Ihrer FRITZ!Box abgemeldet.", CompareMethod.Text) = 0 Then
                    ' C_hf.LogFile("Logout erfolgreich")
                    sSID = C_DP.P_Def_SessionID
                    Return True
                Else
                    Response = Replace(C_hf.StringEntnehmen(Response, "<pre>", "</pre>"), Chr(34), "'", , , CompareMethod.Text)
                    If Not Response = C_DP.P_Def_ErrorMinusOne_String Then
                        tmpstr = C_hf.StringEntnehmen(Response, "['logout'] = '", "'")
                        If Not tmpstr = "1" Then
                            C_hf.LogFile("Logout eventuell NICHT erfolgreich!")
                        End If
                    End If
                    sSID = C_DP.P_Def_SessionID
                    Return False
                End If
            Else
                C_hf.LogFile("FBError (FBLogout): " & Err.Number & " - " & Err.Description & " - " & sLink)
            End If
        Else
            C_hf.LogFile("FBError (FBLogout): " & Err.Number & " - " & Err.Description & " - " & sLink)
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

            If Not tempstring_code = C_DP.P_Def_ErrorMinusOne_String Then
                tempstring = tempstring_code
            Else
                tempstring = C_hf.StringEntnehmen(tempstring, "<pre>", "</pre>")
            End If
            If Not tempstring = C_DP.P_Def_ErrorMinusOne_String Then
                FritzBoxDatenN(tempstring)
                FBLogOut(sSID)
            Else
                FritzBoxDatenA(sLink)
            End If
        Else
            FritzBoxDatenA()
        End If
    End Sub

    Friend Sub FritzBoxDaten()
        Dim FW550 As Boolean = True
        Dim sLink As String
        Dim tempstring As String
        Dim tempstring_code As String

        If P_SpeichereDaten Then PushStatus("Fritz!Box Adresse: " & C_DP.P_TBFBAdr)

        FBLogIn(FW550)
        If Not sSID = C_DP.P_Def_SessionID Then
            sLink = "http://" & C_hf.ValidIP(C_DP.P_TBFBAdr) & "/fon_num/fon_num_list.lua?sid=" & sSID

            PushStatus("Fritz!Box SessionID: " & sSID)
            PushStatus("Fritz!Box Firmware  5.50: " & FW550.ToString)
            tempstring = C_hf.httpGET(sLink, FBEncoding, FBFehler)
            If Not FBFehler Then
                If InStr(tempstring, "FRITZ!Box Anmeldung", CompareMethod.Text) = 0 Then
                    tempstring = Replace(tempstring, Chr(34), "'", , , CompareMethod.Text)   ' " in ' umwandeln 
                    tempstring = Replace(tempstring, Chr(13), "", , , CompareMethod.Text)

                    If InStr(tempstring, "Luacgi not readable") = 0 Then
                        tempstring_code = C_hf.StringEntnehmen(tempstring, "<code>", "</code>")

                        If Not tempstring_code = C_DP.P_Def_ErrorMinusOne_String Then
                            tempstring = tempstring_code
                        Else
                            tempstring = C_hf.StringEntnehmen(tempstring, "<pre>", "</pre>")
                        End If
                        If Not tempstring = C_DP.P_Def_ErrorMinusOne_String Then
                            FritzBoxDatenN(tempstring)
                            FBLogOut(sSID)
                        Else
                            C_hf.FBDB_MsgBox("Fehler bei dem Herunterladen der Telefone: Telefonieseite kann nicht gelesen werden.", MsgBoxStyle.Critical, "FritzBoxDaten #3")
                        End If
                    Else
                        FritzBoxDatenA()
                    End If
                Else
                    C_hf.FBDB_MsgBox("Fehler bei dem Herunterladen der Telefone: Die Anmeldedaten sind falsch oder es fehlt die Berechtigung für diesen Bereich.", MsgBoxStyle.Critical, "FritzBoxDaten #1")
                End If
            Else
                C_hf.LogFile("FBError (FritzBoxDaten): " & Err.Number & " - " & Err.Description & " - " & sLink)
            End If
        Else
            C_hf.FBDB_MsgBox("Fehler bei dem Herunterladen der Telefone: Die Anmeldedaten sind falsch oder es fehlt die Berechtigung für diesen Bereich.", MsgBoxStyle.Critical, "FritzBoxDaten #2")
        End If

    End Sub

    Private Sub FritzBoxDatenA(Optional ByVal Link As String = "-1")
        PushStatus("ALte Ausleseroutine für Fritz!Box Telefone gestartet.")

        'Dim Vorwahl As String = C_DP.P_TBVorwahl  ' In den Einstellungen eingegebene Vorwahl
        Dim TelName As String                 ' Gefundener Telefonname
        Dim TelNr As String                 ' Dazugehörige Telefonnummer
        Dim SIPID As String = C_DP.P_Def_ErrorMinusOne_String
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
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
        End With
        With AttributeValues
            .Clear()
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
        End With

        If Link = C_DP.P_Def_ErrorMinusOne_String Then
            sLink = "http://" & C_hf.ValidIP(C_DP.P_TBFBAdr) & "/cgi-bin/webcm?sid=" & sSID & "&getpage=../html/de/menus/menu2.html&var:lang=de&var:menu=fon&var:pagename=fondevices"
        Else
            sLink = Link
        End If

        If P_SpeichereDaten Then PushStatus("Fritz!Box Telefon Quelldatei: " & sLink)
        tempstring = C_hf.httpGET(sLink, FBEncoding, FBFehler)
        If Not FBFehler Then
            If Not InStr(tempstring, "FRITZ!Box Anmeldung", CompareMethod.Text) = 0 Then
                C_hf.FBDB_MsgBox("Fehler bei dem Herunterladen der Telefone. Anmeldung fehlerhaft o.A.!", MsgBoxStyle.Critical, "FritzBoxDaten_FWbelow5_50")
                Exit Sub
            End If
            If P_SpeichereDaten Then C_DP.Delete("Telefone")

            tempstring = Replace(tempstring, Chr(34), "'", , , CompareMethod.Text)   ' " in ' umwandeln

            FBLogOut(sSID)
            xPathTeile.Add("MSN")
            pos(0) = 1
            For i = 0 To 9
                TelNr = C_hf.StringEntnehmen(tempstring, "nrs.msn.push('", "'", posSTR)
                If Not TelNr = C_DP.P_Def_ErrorMinusOne_String And Not TelNr = C_DP.P_Def_StringEmpty Then
                    TelNr = C_hf.EigeneVorwahlenEntfernen(TelNr)
                    MSN(i) = TelNr
                    j = i
                    PushStatus("MSN-telefonnummer gefunden: MSN" & CStr(i) & ", " & TelNr)
                    If P_SpeichereDaten Then C_DP.Write(xPathTeile, TelNr, "ID", CStr(i))
                End If
            Next
            ReDim Preserve MSN(j)
            posSTR = 1

            'Internetnummern ermitteln
            xPathTeile.Item(xPathTeile.IndexOf("MSN")) = "SIP"
            j = 0
            For i = 0 To 19
                TelNr = C_hf.StringEntnehmen(tempstring, "nrs.sip.push('", "'", posSTR)
                If Not TelNr = C_DP.P_Def_ErrorMinusOne_String And Not TelNr = C_DP.P_Def_StringEmpty Then
                    TelNr = C_hf.EigeneVorwahlenEntfernen(TelNr)
                    SIP(i) = TelNr
                    SIPID = CStr(i)
                    j = i
                    PushStatus("Internettelefonnummer gefunden: SIP" & CStr(i) & ", " & TelNr)
                    If P_SpeichereDaten Then C_DP.Write(xPathTeile, TelNr, "ID", SIPID)
                End If
            Next
            ReDim Preserve SIP(j)
            j = 0
            posSTR = 1

            'TAM Nr ermitteln
            xPathTeile.Item(xPathTeile.IndexOf("SIP")) = "TAM"
            For i = 0 To 9
                TelNr = C_hf.StringEntnehmen(tempstring, "nrs.tam.push('", "'", posSTR)
                If Not TelNr = C_DP.P_Def_ErrorMinusOne_String And Not TelNr = C_DP.P_Def_StringEmpty Then
                    TelNr = C_hf.EigeneVorwahlenEntfernen(TelNr)
                    TAM(i) = TelNr
                    PushStatus("Anrufbeantworternummer gefunden: TAM" & CStr(i) & ", " & TelNr)
                    If P_SpeichereDaten Then C_DP.Write(xPathTeile, TelNr, "ID", CStr(i))
                    j = i
                End If
            Next
            ReDim Preserve TAM(j)

            ' Plain old telephone service (POTS)
            xPathTeile.Item(xPathTeile.IndexOf("TAM")) = "POTS"
            POTS = C_hf.StringEntnehmen(tempstring, "telcfg:settings/MSN/POTS' value='", "'")
            If Not POTS = C_DP.P_Def_ErrorMinusOne_String And Not POTS = C_DP.P_Def_StringEmpty Then
                POTS = C_hf.EigeneVorwahlenEntfernen(POTS)
                PushStatus("Plain old telephone service gefunden: POTS, " & POTS)
                If P_SpeichereDaten Then C_DP.Write(xPathTeile, POTS, "ID", C_DP.P_Def_StringNull)
            End If

            'Mobilnummer ermitteln
            xPathTeile.Item(xPathTeile.IndexOf("POTS")) = "Mobil"
            Mobil = C_hf.StringEntnehmen(tempstring, "nrs.mobil = '", "'")
            If Not Mobil = C_DP.P_Def_ErrorMinusOne_String And Not Mobil = C_DP.P_Def_StringEmpty Then
                Mobil = C_hf.EigeneVorwahlenEntfernen(Mobil)
                PushStatus("Mobilnummer gefunden: Mobil, " & Mobil)
                If P_SpeichereDaten Then C_DP.Write(xPathTeile, Mobil, "ID", C_DP.P_Def_StringNull)
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
                            If Right(TelNr, 1) = "," Then TelNr = Left(TelNr, Len(TelNr) - 1) ' Für die Firmware *85
                            If Right(TelNr, 1) = "#" Then TelNr = Left(TelNr, Len(TelNr) - 1) ' Für die Firmware *85
                            If Left(TelNr, 3) = "SIP" Then TelNr = SIP(CInt(Mid(TelNr, 4, 1)))
                            If Not Trim(TelName) = C_DP.P_Def_StringEmpty And Not Trim(TelNr) = C_DP.P_Def_StringEmpty Then
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
                                        PushStatus("Analogtelefon gefunden: FON" & CStr(DialPort) & ", " & TelNr & ", " & TelName)
                                        If P_SpeichereDaten Then
                                            NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                            NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                            AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                            AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_DP.P_Def_StringNull
                                            C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
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
                                            PushStatus("S0-Telefon gefunden: " & DialPort & ", " & ", " & TelNr & ", " & TelName)
                                            If P_SpeichereDaten Then
                                                NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                                NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                                AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                                AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_DP.P_Def_StringNull
                                                C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
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
                                        PushStatus("DECT Fritz!Fon 7150 gefunden: " & DialPort & ", " & ", " & TelNr & ", " & TelName)
                                        If P_SpeichereDaten Then
                                            NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                            NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                            AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                            AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_DP.P_Def_StringNull
                                            C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
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
                                                TelNr = C_DP.P_Def_StringEmpty
                                                If Not pos(2) = 7 Then
                                                    Do
                                                        pos(3) = InStr(pos(2), Telefon, "'", CompareMethod.Text)
                                                        tempTelNr = Mid(Telefon, pos(2), pos(3) - pos(2))
                                                        TelNr = C_hf.EigeneVorwahlenEntfernen(TelNr)
                                                        TelNr += CStr(IIf(Right(TelNr, 1) = "#", C_DP.P_Def_StringEmpty, tempTelNr & ";"))
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
                                            PushStatus("DECT-Telefon gefunden: " & DialPort & ", " & TelNr & ", " & TelName)
                                            If P_SpeichereDaten Then
                                                NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                                NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                                AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                                AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_DP.P_Def_StringNull
                                                C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
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
                                            PushStatus("IP-Telefon gefunden: " & DialPort & ", " & TelNr & ", " & TelName)
                                            If P_SpeichereDaten Then
                                                NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                                NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                                AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                                AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_DP.P_Def_StringNull
                                                C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                                            End If
                                        Else
                                            Dim LANTelefone() As String = Split(Telefon, "in_nums = [];", , CompareMethod.Text)
                                            Dim InNums As String = C_DP.P_Def_StringEmpty
                                            Dim NetInfo As String
                                            Dim NetInfoPush As String = C_DP.P_Def_StringEmpty
                                            pos(0) = InStr(LANTelefone(LANTelefone.Length - 1), "NetInfo.push(parseInt('", CompareMethod.Text)
                                            If Not pos(0) = 0 Then
                                                NetInfo = Mid(LANTelefone(LANTelefone.Length - 1), pos(0))
                                                pos(0) = 1
                                                Do
                                                    pos(1) = InStr(pos(0), NetInfo, "', 10));", CompareMethod.Text) + Len("', 10));")
                                                    NetInfoPush = Mid(NetInfo, pos(0) + Len("NetInfo.push(parseInt('"), 3) & CStr(IIf(Not NetInfoPush = C_DP.P_Def_StringEmpty, ";" & NetInfoPush, C_DP.P_Def_StringEmpty))
                                                    pos(0) = InStr(pos(1), NetInfo, "NetInfo.push(parseInt('", CompareMethod.Text)
                                                Loop Until pos(0) = 0
                                            End If
                                            For Each LANTelefon In LANTelefone
                                                If Not InStr(LANTelefon, "TelCfg.push( { Enabled : '", vbTextCompare) = 0 Then
                                                    Dim tempTelNr As String
                                                    pos(2) = InStr(LANTelefon, "num = '", CompareMethod.Text) + Len("num = '")
                                                    TelNr = C_DP.P_Def_StringEmpty
                                                    If Not pos(2) = 7 Then
                                                        InNums = C_DP.P_Def_StringEmpty
                                                        Do
                                                            pos(3) = InStr(pos(2), LANTelefon, "'", CompareMethod.Text)
                                                            tempTelNr = Mid(LANTelefon, pos(2), pos(3) - pos(2))
                                                            TelNr = C_hf.EigeneVorwahlenEntfernen(tempTelNr)
                                                            InNums += CStr(IIf(Strings.Right(TelNr, 1) = "#", C_DP.P_Def_StringEmpty, TelNr & ";"))
                                                            pos(2) = InStr(pos(3), LANTelefon, "num = '", CompareMethod.Text) + Len("num = '")
                                                        Loop Until pos(2) = 7
                                                        InNums = Left(InNums, Len(InNums) - 1)
                                                    End If

                                                    pos(0) = InStr(LANTelefon, "Name : '", CompareMethod.Text) + Len("Name : '")
                                                    pos(1) = InStr(pos(0), LANTelefon, "'", CompareMethod.Text)
                                                    TelName = Mid(LANTelefon, pos(0), pos(1) - pos(0))
                                                    If Not TelName = C_DP.P_Def_StringEmpty Then
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
                                                        If NetInfoPush = C_DP.P_Def_StringEmpty Then
                                                            If Not InStr(LANTelefon, "TelCfg.push( { Enabled : '1',", CompareMethod.Text) = 0 Then
                                                                DialPort = "2" & ID
                                                                Anzahl += 1
                                                                PushStatus("IP-Telefon gefunden: " & DialPort & ", " & TelNr & ", " & TelName)
                                                                If P_SpeichereDaten Then
                                                                    NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                                                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                                                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                                                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_DP.P_Def_StringNull
                                                                    C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                                                                End If

                                                            End If
                                                        Else
                                                            If C_hf.IsOneOf("62" & ID, Split(NetInfoPush, ";", , CompareMethod.Text)) Then
                                                                DialPort = "2" & ID
                                                                Anzahl += 1
                                                                PushStatus("IP-Telefon gefunden: " & DialPort & ", " & TelNr & ", " & TelName)
                                                                If P_SpeichereDaten Then
                                                                    NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                                                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                                                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                                                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_DP.P_Def_StringNull
                                                                    C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
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
                                        TelNr = C_DP.P_Def_StringEmpty
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
                                                If Not TAM(j) Is Nothing Then
                                                    If (tamMsnBits And (1 << j)) > 0 Then ' Aus AVM Quellcode Funktion isBitSet übernommen 
                                                        TelNr += TAM(j) & ";"
                                                    End If
                                                End If
                                            Next
                                            If Not TelNr = C_DP.P_Def_StringEmpty Then
                                                TelNr = Left(TelNr, Len(TelNr) - 1)
                                                DialPort = "60" & ID

                                                PushStatus("Anrufbeantworter gefunden: " & DialPort & ", " & ", " & TelNr & ", " & TelName)
                                                If P_SpeichereDaten Then
                                                    NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_DP.P_Def_StringNull
                                                    C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                                                End If

                                                Anzahl += 1
                                            End If
                                        End If
                                    Case 6 ' integrierter Faxempfang
                                        xPathTeile.Item(xPathTeile.Count - 1) = "FAX"
                                        Dim FAXMSN(9) As String
                                        TelNr = C_DP.P_Def_StringEmpty
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

                                                PushStatus("Die integrierte Faxfunktion ist eingeschaltet: " & DialPort & ", " & TelNr & "," & TelName)
                                                If P_SpeichereDaten Then
                                                    NodeValues.Item(NodeNames.IndexOf("TelName")) = "Faxempfang"
                                                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = "1"
                                                    C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
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
                PushStatus("S0-Basis hinzugefügt.")
                If P_SpeichereDaten Then
                    NodeValues.Item(NodeNames.IndexOf("TelName")) = "ISDN-Basis"
                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = C_DP.P_Def_StringEmpty
                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_DP.P_Def_StringEmpty
                    C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                End If

            End If
        Else
            C_hf.LogFile("FBError (FritzBoxDatenA): " & Err.Number & " - " & Err.Description & " - " & sLink)
        End If

    End Sub ' (FritzBoxDaten für ältere Firmware)

    Private Sub FritzBoxDatenN(ByVal Code As String)
        PushStatus("Neue Ausleseroutine für Fritz!Box Telefone gestartet.")

        'Dim Vorwahl As String = C_DP.P_TBVorwahl                 ' In den Einstellungen eingegebene Vorwahl
        Dim Landesvorwahl As String
        Dim TelName As String                 ' Gefundener Telefonname
        Dim TelNr As String                 ' Dazugehörige Telefonnummer
        Dim SIPID As String = C_DP.P_Def_ErrorMinusOne_String
        Dim pos(1) As Integer
        Dim i As Integer                   ' Laufvariable
        Dim j As Integer
        Dim k As Integer
        Dim SIP(20) As String
        Dim TAM(10) As String
        Dim MSNPort(2, 9) As String
        Dim MSN(9) As String
        Dim FAX(9) As String
        Dim Mobil As String
        Dim POTS As String
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

        'NodeNames.Add("TelName")
        'NodeNames.Add("TelNr")
        'AttributeNames.Add("Fax")
        'AttributeNames.Add("Dialport")

        If P_SpeichereDaten Then C_DP.Delete("Telefone")

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
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
        End With
        With AttributeValues
            .Clear()
            .Add(C_DP.P_Def_StringEmpty)
            .Add(C_DP.P_Def_StringEmpty)
        End With
        'SIP Nummern
        With C_hf
            xPathTeile.Add("SIP")
            For Each SIPi In Split(.StringEntnehmen(Code, "['sip:settings/sip/list(" & .StringEntnehmen(Code, "['sip:settings/sip/list(", ")'] = {") & ")'] = {", "}" & Chr(10) & "  },"), " },", , CompareMethod.Text)
                If .StringEntnehmen(SIPi, "['activated'] = '", "'") = "1" Then
                    TelNr = .EigeneVorwahlenEntfernen(.StringEntnehmen(SIPi, "['displayname'] = '", "'"))
                    Node = UCase(.StringEntnehmen(SIPi, "['_node'] = '", "'"))
                    SIPID = .StringEntnehmen(SIPi, "['ID'] = '", "'")
                    SIP(CInt(SIPID)) = TelNr
                    PushStatus("Internettelefonnummer gefunden: " & Node & ", " & TelNr)
                    If P_SpeichereDaten Then
                        C_DP.Write(xPathTeile, TelNr, "ID", SIPID)
                    End If
                End If
            Next

            'SIP = (From x In SIP Where Not x Like C_DP.P_Def_StringEmpty Select x).ToArray
            PushStatus("Letzte SIP: " & SIPID)

            xPathTeile.Item(xPathTeile.IndexOf("SIP")) = "MSN"
            For i = 0 To 9
                TelNr = .StringEntnehmen(Code, "['telcfg:settings/MSN/MSN" & i & "'] = '", "'")
                If Not TelNr = C_DP.P_Def_ErrorMinusOne_String Then
                    If Not Len(TelNr) = 0 Then
                        TelNr = .EigeneVorwahlenEntfernen(TelNr)
                        MSN(i) = TelNr
                        PushStatus("MSN-Telefonnummer gefunden: MSN" & CStr(i) & ", " & TelNr)
                        If P_SpeichereDaten Then
                            C_DP.Write(xPathTeile, TelNr, "ID", CStr(i))
                        End If
                    End If
                End If
            Next

            For i = 0 To 2
                If Not .StringEntnehmen(Code, "['telcfg:settings/MSN/Port" & i & "/Name'] = '", "'") = C_DP.P_Def_ErrorMinusOne_String Then
                    For j = 0 To 9
                        TelNr = .StringEntnehmen(Code, "['telcfg:settings/MSN/Port" & i & "/MSN" & j & "'] = '", "'")
                        If Not TelNr = C_DP.P_Def_ErrorMinusOne_String Then
                            If Not Len(TelNr) = 0 Then
                                If Strings.Left(TelNr, 3) = "SIP" Then
                                    TelNr = SIP(CInt(Mid(TelNr, 4, 1)))
                                Else
                                    TelNr = .EigeneVorwahlenEntfernen(TelNr)
                                End If

                                If Not .IsOneOf(TelNr, MSN) Then
                                    For k = 0 To 9
                                        If MSN(k) = C_DP.P_Def_StringEmpty Then
                                            MSN(k) = TelNr
                                            PushStatus("MSN-Telefonnummer gefunden: MSN" & CStr(k) & ", " & TelNr)
                                            If P_SpeichereDaten Then
                                                C_DP.Write(xPathTeile, TelNr, "ID", CStr(k))
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
            'MSN = (From x In MSN Select x Distinct).ToArray 'Doppelte entfernen
            'MSN = (From x In MSN Where Not x Like C_DP.P_Def_StringEmpty Select x).ToArray

            xPathTeile.Item(xPathTeile.IndexOf("MSN")) = "TAM"
            For i = 0 To 9
                TelNr = .StringEntnehmen(Code, "['tam:settings/MSN" & i & "'] = '", "'")
                If Not TelNr = C_DP.P_Def_ErrorMinusOne_String Then
                    If Not Len(TelNr) = 0 Then
                        If Strings.Left(TelNr, 3) = "SIP" Then
                            TelNr = SIP(CInt(Mid(TelNr, 4, 1)))
                        Else
                            TelNr = .EigeneVorwahlenEntfernen(TelNr)
                        End If
                        PushStatus("Anrufbeantworternummer gefunden: TAM" & CStr(i) & ", " & TelNr)
                        If P_SpeichereDaten Then
                            C_DP.Write(xPathTeile, TelNr, "ID", CStr(i))
                        End If

                        TAM(i) = TelNr
                    End If
                End If
            Next

            xPathTeile.Item(xPathTeile.IndexOf("TAM")) = "FAX"
            For i = 0 To 9
                TelNr = .StringEntnehmen(Code, "['telcfg:settings/FaxMSN" & i & "'] = '", "'")
                If Not TelNr = C_DP.P_Def_ErrorMinusOne_String Then
                    If Not Len(TelNr) = 0 Then
                        If Strings.Left(TelNr, 3) = "SIP" Then
                            TelNr = SIP(CInt(Mid(TelNr, 4, 1)))
                        Else
                            TelNr = .EigeneVorwahlenEntfernen(TelNr)
                        End If
                        PushStatus("Faxnummer (FAX) gefunden: FAX" & CStr(i) & ", " & TelNr)
                        If P_SpeichereDaten Then
                            C_DP.Write(xPathTeile, TelNr, "ID", CStr(i))
                        End If

                        FAX(i) = TelNr
                    End If
                End If
            Next
            ' FAX = (From x In FAX Where Not x Like C_DP.P_Def_StringEmpty Select x).ToArray

            xPathTeile.Item(xPathTeile.IndexOf("FAX")) = "POTS"
            POTS = .StringEntnehmen(Code, "['telcfg:settings/MSN/POTS'] = '", "'")
            If Not POTS = C_DP.P_Def_ErrorMinusOne_String And Not POTS = C_DP.P_Def_StringEmpty Then
                If Strings.Left(POTS, 3) = "SIP" Then
                    POTS = SIP(CInt(Mid(POTS, 4, 1)))
                Else
                    POTS = .EigeneVorwahlenEntfernen(POTS)
                End If
                PushStatus("Plain old telephone service (POTS) gefunden: " & POTS)
                If P_SpeichereDaten Then C_DP.Write(xPathTeile, POTS, "ID", C_DP.P_Def_StringNull)
            End If

            xPathTeile.Item(xPathTeile.IndexOf("POTS")) = "Mobil"

            Mobil = .StringEntnehmen(Code, "['telcfg:settings/Mobile/MSN'] = '", "'")
            If Not Mobil = C_DP.P_Def_ErrorMinusOne_String And Not Mobil = C_DP.P_Def_StringEmpty Then
                If Strings.Left(Mobil, 3) = "SIP" Then
                    Mobil = SIP(CInt(Mid(Mobil, 4, 1)))
                Else
                    Mobil = .EigeneVorwahlenEntfernen(Mobil)
                End If
                PushStatus("Mobilnummer (Mobil) gefunden: " & Mobil)
                If P_SpeichereDaten Then C_DP.Write(xPathTeile, Mobil, "ID", C_DP.P_Def_StringNull)
            End If

            SIP = (From x In SIP Where Not x Like C_DP.P_Def_StringEmpty Select x).ToArray
            MSN = (From x In MSN Select x Distinct).ToArray 'Doppelte entfernen
            MSN = (From x In MSN Where Not x Like C_DP.P_Def_StringEmpty Select x).ToArray
            FAX = (From x In FAX Where Not x Like C_DP.P_Def_StringEmpty Select x).ToArray



            allin = AlleNummern(MSN, SIP, TAM, FAX, POTS, Mobil)

            'Telefone Einlesen

            pos(0) = 1
            xPathTeile.Item(xPathTeile.IndexOf("Nummern")) = "Telefone"
            xPathTeile.Item(xPathTeile.IndexOf("Mobil")) = "FON"
            'FON
            For Each Telefon In Split(.StringEntnehmen(Code, "['telcfg:settings/MSN/Port/list(" & .StringEntnehmen(Code, "['telcfg:settings/MSN/Port/list(", ")'] = {") & ")'] = {", "}" & Chr(10) & "  },"), " },", , CompareMethod.Text)
                TelName = .StringEntnehmen(Telefon, "['Name'] = '", "'")
                If Not (TelName = C_DP.P_Def_ErrorMinusOne_String Or TelName = C_DP.P_Def_StringEmpty) Then
                    TelNr = C_DP.P_Def_StringEmpty
                    Port = Right(.StringEntnehmen(Telefon, "['_node'] = '", "'"), 1)

                    Dim tmparray(9) As String
                    For i = 0 To 9
                        tmpTelNr = MSNPort(CInt(Port), i)
                        If Not tmpTelNr = C_DP.P_Def_StringEmpty Then
                            tmparray(i) = MSNPort(CInt(Port), i)
                        Else
                            Exit For
                        End If
                    Next
                    tmparray = (From x In tmparray Where Not x Like C_DP.P_Def_StringEmpty Select x).ToArray
                    If tmparray.Length = 0 Then tmparray = MSN

                    TelNr = String.Join(";", tmparray)
                    DialPort = CStr(CInt(Port) + 1) ' + 1 für FON

                    PushStatus("Analogtelefon gefunden: FON" & DialPort & ", " & TelNr & ", " & TelName)
                    If P_SpeichereDaten Then
                        NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                        NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr

                        AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                        AttributeValues.Item(AttributeNames.IndexOf("Fax")) = .StringEntnehmen(Telefon, "['Fax'] = '", "'")
                        C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                    End If
                    If .StringEntnehmen(Telefon, "['Fax'] = '", "'") = "1" Then
                        PushStatus("Analogtelefon FON" & DialPort & " ist ein FAX.")
                    End If

                End If
            Next

            ' DECT
            xPathTeile.Item(xPathTeile.IndexOf("FON")) = "DECT"
            tmpTelefone = .StringEntnehmen(Code, "['telcfg:settings/Foncontrol/User/list(" & .StringEntnehmen(Code, "['telcfg:settings/Foncontrol/User/list(", ")'] = {") & ")'] = {", "}" & Chr(10) & "  },")

            For Each DectTelefon In Split(tmpTelefone, "] = {", , CompareMethod.Text)

                DialPort = .StringEntnehmen(DectTelefon, "['Intern'] = '", "'")
                If Not (DialPort = C_DP.P_Def_ErrorMinusOne_String Or DialPort = C_DP.P_Def_StringEmpty) Then
                    TelNr = C_DP.P_Def_StringEmpty
                    DialPort = "6" & Strings.Right(DialPort, 1)
                    TelName = .StringEntnehmen(DectTelefon, "['Name'] = '", "'")
                    Node = .StringEntnehmen(DectTelefon, "['_node'] = '", "'")

                    If .StringEntnehmen(Code, "['telcfg:settings/Foncontrol/" & Node & "/RingOnAllMSNs'] = '", "',") = "1" Then
                        TelNr = allin
                    Else
                        tmpstrUser = Split(.StringEntnehmen(Code, "['telcfg:settings/Foncontrol/" & Node & "/MSN/list(Number)'] = {", "}" & Chr(10) & "  },"), "['Number'] = '", , CompareMethod.Text)

                        tmpstrUser(0) = C_DP.P_Def_StringEmpty
                        For l As Integer = 1 To tmpstrUser.Length - 1
                            tmpstrUser(l) = Strings.Left(tmpstrUser(l), InStr(tmpstrUser(l), "'", CompareMethod.Text) - 1)
                        Next
                        For Each Nr In (From x In tmpstrUser Where Not x Like C_DP.P_Def_StringEmpty Select x).ToArray ' Leere entfernen
                            TelNr = TelNr & ";" & .EigeneVorwahlenEntfernen(Nr)
                        Next
                        TelNr = Mid(TelNr, 2)
                    End If
                    PushStatus("DECT-Telefon gefunden: " & DialPort & ", " & TelNr & ", " & TelName)
                    If P_SpeichereDaten Then
                        NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                        NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                        AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                        AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_DP.P_Def_StringNull

                        C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                    End If

                End If
            Next

            xPathTeile.Item(xPathTeile.IndexOf("DECT")) = "VOIP"
            'IP-Telefone
            tmpstrUser = Split(.StringEntnehmen(Code, "['telcfg:settings/VoipExtension/list(" & .StringEntnehmen(Code, "['telcfg:settings/VoipExtension/list(", ")'] = {") & ")'] = {", "}" & Chr(10) & "  },"), " },", , CompareMethod.Text)
            For Each Telefon In tmpstrUser
                If .StringEntnehmen(Telefon, "['enabled'] = '", "'") = "1" Then
                    TelName = .StringEntnehmen(Telefon, "['Name'] = '", "'")
                    TelNr = C_DP.P_Def_StringEmpty
                    Port = .StringEntnehmen(Telefon, "['_node'] = '", "'")
                    For j = 0 To 9
                        tmpTelNr = .StringEntnehmen(Code, "['telcfg:settings/" & Port & "/Number" & j & "'] = '", "'")
                        If Not tmpTelNr = C_DP.P_Def_ErrorMinusOne_String Then
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
                    If Not TelNr = C_DP.P_Def_StringEmpty Then
                        TelNr = Strings.Left(TelNr, Len(TelNr) - 1)
                    End If

                    DialPort = "2" & Strings.Right(Port, 1)
                    PushStatus("IP-Telefon gefunden: " & DialPort & ", " & TelNr & ", " & TelName)
                    If P_SpeichereDaten Then
                        NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                        NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                        AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                        AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_DP.P_Def_StringNull

                        C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                    End If

                End If
            Next
            xPathTeile.Item(xPathTeile.IndexOf("VOIP")) = "S0"
            Dim S0Typ As String
            ' S0-Port
            For i = 1 To 8
                TelName = .StringEntnehmen(Code, "['telcfg:settings/NTHotDialList/Name" & i & "'] = '", "'")
                If Not TelName = C_DP.P_Def_ErrorMinusOne_String Then
                    If Not TelName = C_DP.P_Def_StringEmpty Then
                        TelNr = .StringEntnehmen(Code, "['telcfg:settings/NTHotDialList/Number" & i & "'] = '", "'")
                        If Not TelNr = C_DP.P_Def_ErrorMinusOne_String Then
                            DialPort = "5" & i
                            PushStatus("S0-Telefon gefunden: " & DialPort & ", " & ", " & TelNr & ", " & TelName)
                            If P_SpeichereDaten Then
                                NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                                NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                                AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                                AttributeValues.Item(AttributeNames.IndexOf("Fax")) = IIf(.StringEntnehmen(Code, "['telcfg:settings/NTHotDialList/Type" & i & "'] = '", "'") = "Fax", 1, 0)

                                C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                            End If

                            S0Typ = .StringEntnehmen(Code, "['telcfg:settings/NTHotDialList/Type" & i & "'] = '", "'")
                            If Not TelNr = C_DP.P_Def_ErrorMinusOne_String Then
                                Select Case S0Typ
                                    Case "Fax"
                                        PushStatus("S0-telefon " & DialPort & " ist ein FAX.")
                                        'Case "Isdn"
                                        'Case "Fon"
                                        'Case Else
                                End Select
                            End If
                        End If
                    End If
                End If
            Next
            If Not DialPort = C_DP.P_Def_StringEmpty Then
                If CDbl(DialPort) > 50 And CDbl(DialPort) < 60 Then
                    DialPort = "50"
                    PushStatus("S0-Basis hinzugefügt.")
                    If P_SpeichereDaten Then
                        NodeValues.Item(NodeNames.IndexOf("TelName")) = "ISDN-Basis"
                        NodeValues.Item(NodeNames.IndexOf("TelNr")) = "50"
                        AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                        AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_DP.P_Def_StringNull
                        C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                    End If
                End If
            End If
            xPathTeile.Item(xPathTeile.IndexOf("S0")) = "TAM"
            ' TAM
            tmpstrUser = Split(.StringEntnehmen(Code, "['tam:settings/TAM/list(" & .StringEntnehmen(Code, "['tam:settings/TAM/list(", ")'] = {") & ")'] = {", "}" & Chr(10) & "  },"), " },", , CompareMethod.Text)
            For Each Anrufbeantworter In tmpstrUser
                If .StringEntnehmen(Anrufbeantworter, "['Active'] = '", "'") = "1" Then
                    TelName = .StringEntnehmen(Anrufbeantworter, "['Name'] = '", "'")
                    Port = .StringEntnehmen(Anrufbeantworter, "['_node'] = '", "'")
                    TelNr = .EigeneVorwahlenEntfernen(TAM(CInt(Strings.Right(Port, 1))))
                    DialPort = "60" & Strings.Right(Port, 1)
                    PushStatus("Anrufbeantworter gefunden: " & DialPort & ", " & ", " & TelNr & ", " & TelName)
                    If P_SpeichereDaten Then
                        NodeValues.Item(NodeNames.IndexOf("TelName")) = TelName
                        NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                        AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                        AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_DP.P_Def_StringNull
                        C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                    End If

                End If
            Next


            ' integrierter Faxempfang
            xPathTeile.Item(xPathTeile.IndexOf("TAM")) = "FAX"
            DialPort = .StringEntnehmen(Code, "['telcfg:settings/FaxMailActive'] = '", "'")
            If Not DialPort = "0" Then
                TelNr = C_DP.P_Def_ErrorMinusOne_String
                DialPort = "5"
                PushStatus("Die integrierte Faxfunktion ist eingeschaltet: " & DialPort & ", " & TelNr & "," & "Faxempfang")
                If P_SpeichereDaten Then
                    NodeValues.Item(NodeNames.IndexOf("TelName")) = "Faxempfang"
                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = TelNr
                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = DialPort
                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = "1"

                    C_DP.AppendNode(xPathTeile, C_DP.CreateXMLNode("Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                End If

            End If

            Landesvorwahl = .StringEntnehmen(Code, "['country'] = '", "'")
            If Len(Landesvorwahl) > 2 Then
                If Len(Landesvorwahl) = 3 And Left(Landesvorwahl, 1) = "0" Then
                    Landesvorwahl = "0" & Landesvorwahl
                End If
                C_DP.P_TBLandesVW = Landesvorwahl
            End If

        End With

    End Sub

    Private Overloads Function AlleNummern(ByVal MSN() As String, ByVal SIP() As String, ByVal TAM() As String, ByVal FAX() As String, ByVal POTS As String, ByVal Mobil As String) As String
        AlleNummern = C_DP.P_Def_StringEmpty
        Dim tmp() As String = Split(Strings.Join(MSN, ";") & ";" & Strings.Join(SIP, ";") & ";" & Strings.Join(TAM, ";") & ";" & Strings.Join(FAX, ";") & ";" & POTS & ";" & Mobil, ";", , CompareMethod.Text)
        tmp = (From x In tmp Select x Distinct).ToArray 'Doppelte entfernen
        tmp = (From x In tmp Where Not x Like C_DP.P_Def_StringEmpty Select x).ToArray ' Leere entfernen
        For Each Nr In tmp
            AlleNummern = Nr & ";" & AlleNummern
        Next
        AlleNummern = Strings.Left(AlleNummern, Len(AlleNummern) - 1)
    End Function

    Private Overloads Function AlleNummern(ByVal MSN() As String, ByVal SIP() As String, ByVal TAM() As String, ByVal POTS As String, ByVal Mobil As String) As String
        AlleNummern = C_DP.P_Def_StringEmpty
        Dim tmp() As String = Split(Strings.Join(MSN, ";") & ";" & Strings.Join(SIP, ";") & ";" & Strings.Join(TAM, ";") & ";" & POTS & ";" & Mobil, ";", , CompareMethod.Text)
        tmp = (From x In tmp Select x Distinct).ToArray 'Doppelte entfernen
        tmp = (From x In tmp Where Not x Like C_DP.P_Def_StringEmpty Select x).ToArray ' Leere entfernen
        tmp = (From x In tmp Where Not x Like C_DP.P_Def_ErrorMinusOne_String Select x).ToArray ' -1 entfernen
        For Each Nr In tmp
            AlleNummern = Nr & ";" & AlleNummern
        Next
        AlleNummern = Strings.Left(AlleNummern, Len(AlleNummern) - 1)
    End Function
#End Region

#Region "Wählen"
    Friend Function SendDialRequestToBox(ByVal DialCode As String, ByVal DialPort As String, HangUp As Boolean) As String
        ' überträgt die zum Verbindungsaufbau notwendigen Daten per WinHttp an die FritzBox
        ' Parameter:  dialCode (string):    zu wählende Nummer
        '             fonanschluss (long):  Welcher Anschluss wird verwendet?
        '             HangUp (bool):        Soll Verbindung abgebrochen werden
        ' Rückgabewert (String):            Antworttext (Status)
        '
        Dim formdata As String             ' an die FritzBox zu sendende Daten
        Dim Response As String             ' Antwort der FritzBox
        Dim Link As String
        '
        SendDialRequestToBox = "Fehler!" & vbCrLf & "Entwickler kontaktieren."            ' Antwortstring
        If Not sSID = C_DP.P_Def_SessionID And Len(sSID) = Len(C_DP.P_Def_SessionID) Then
            Link = "http://" & C_hf.ValidIP(C_DP.P_TBFBAdr) & "/cgi-bin/webcm"
            formdata = "sid=" & sSID & "&getpage=&telcfg:settings/UseClickToDial=1&telcfg:settings/DialPort=" & DialPort & "&telcfg:command/" & CStr(IIf(HangUp, "Hangup", "Dial=" & DialCode))
            Response = C_hf.httpPOST(Link, formdata, FBEncoding)

            If Response = C_DP.P_Def_StringEmpty Then
                SendDialRequestToBox = CStr(IIf(HangUp, "Verbindungsaufbau" & vbCrLf & "wurde abgebrochen!", "Wähle " & DialCode & vbCrLf & "Jetzt abheben!"))
            Else
                SendDialRequestToBox = "Fehler!" & vbCrLf & "Logfile beachten!"
                C_hf.LogFile("SendDialRequestToBox: Response: " & Response)
            End If
        Else
            C_hf.FBDB_MsgBox("Fehler bei dem Login. SessionID: " & sSID & "!", MsgBoxStyle.Critical, "sendDialRequestToBox")
        End If
    End Function
#End Region

#Region "Journalimort"

    Public Function DownloadAnrListe() As String
        Dim sLink(1) As String
        Dim ReturnString As String = C_DP.P_Def_StringEmpty
        Dim LinkBase As String

        sSID = FBLogIn(True)
        If Not sSID = C_DP.P_Def_SessionID Then

            sLink(0) = "http://" & C_hf.ValidIP(C_DP.P_TBFBAdr) & "/fon_num/foncalls_list.lua?sid=" & sSID
            sLink(1) = sLink(0) & "&csv="

            ReturnString = C_hf.httpGET(sLink(0), FBEncoding, FBFehler)
            If Not FBFehler Then
                If Not InStr(ReturnString, "Luacgi not readable", CompareMethod.Text) = 0 Then
                    LinkBase = "http://" & C_hf.ValidIP(C_DP.P_TBFBAdr) & "/cgi-bin/webcm?sid=" & sSID & "&getpage=../html/de/"
                    sLink(0) = LinkBase & "menus/menu2.html&var:lang=de&var:menu=fon&var:pagename=foncalls"
                    C_hf.httpGET(sLink(0), FBEncoding, FBFehler)
                    sLink(1) = LinkBase & "FRITZ!Box_Anrufliste.csv"
                End If
                ReturnString = C_hf.httpGET(sLink(1), FBEncoding, FBFehler)
            Else
                C_hf.LogFile("FBError (DownloadAnrListe): " & Err.Number & " - " & Err.Description & " - " & sLink(0))
            End If
        Else
            C_hf.FBDB_MsgBox("Der Login in die Fritz!Box ist fehlgeschlagen" & vbCrLf & vbCrLf & "Die Anmeldedaten sind falsch oder es fehlt die Berechtigung für diesen Bereich.", MsgBoxStyle.Critical, "DownloadAnrListe_DoWork")
            C_hf.LogFile("Die Anmeldedaten sind falsch oder es fehlt die Berechtigung für diesen Bereich. (DownloadAnrListe_DoWork)")
        End If
        Return ReturnString
    End Function

#End Region

#Region "Information"
    Public Function GetInformationSystemFritzBox(ByVal FBAdr As String) As String
        Dim sLink As String
        Dim FBTyp As String = C_DP.P_Def_StringUnknown
        Dim FBFW As String = C_DP.P_Def_StringUnknown
        Dim FritzBoxInformation() As String

        If LCase(FBAdr) = C_DP.P_Def_FritzBoxAdress Then C_hf.Ping(FBAdr)

        sLink = "http://" & FBAdr & "/cgi-bin/system_status"
        FritzBoxInformation = Split(C_hf.StringEntnehmen(C_hf.httpGET(sLink, System.Text.Encoding.UTF8, Nothing), "<body>", "</body>"), "-", , CompareMethod.Text)
        FBTyp = FritzBoxInformation(0)
        FBFW = Replace(Trim(C_hf.GruppiereNummer(FritzBoxInformation(7))), " ", ".", , , CompareMethod.Text)

        Return String.Concat("Ergänze bitte folgende Angaben:", vbNewLine, vbNewLine, _
                             "Dein Name:", vbNewLine, _
                             "Problembeschreibung:", vbNewLine, _
                             "Datum & Uhrzeit: ", System.DateTime.Now, vbNewLine, _
                             "Fritz!Box-Typ: ", FBTyp, vbNewLine, _
                             "Firmware: ", FBFW, vbNewLine)

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

End Class
