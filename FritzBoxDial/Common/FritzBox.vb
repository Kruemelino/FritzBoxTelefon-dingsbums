Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Xml

Public Class FritzBox
    Implements IDisposable

    Private C_XML As XML
    Private C_DP As DataProvider
    Private C_Crypt As Rijndael
    Private C_hf As Helfer
    Private C_FBoxUPnP As FritzBoxServices

    Private FBFehler As Boolean

    Private tb As New Windows.Forms.TextBox
    Private EventProvider As IEventProvider

    Private bValSpeichereDaten As Boolean = True
    Private ThisFBFirmware As FritzBoxFirmware
    Private sSID As String
    Private sFirmware As String
    Private WithEvents BWSetDialPort As BackgroundWorker

#Region "Objektorientiertes Einlesen der Telefone"
#Region "Auflistungen"
    ''' <summary>
    ''' Auflistung der möglichen Telefontypen der an der Fritz!Box angeschlossenen Telefone
    ''' </summary>
    Private Enum TelTyp As Integer
        FON = 1
        DECT = 2
        IP = 4
        S0 = 8
        FAX = 16
        Mobil = 32
        POTS = 64
        MSN = 128
        TAM = 256
        SIP = 512
    End Enum

    ''' <summary>
    ''' Auflistung der Basiswerte für den Dialport abhängig von Telefontyp.
    ''' </summary>
    Private Enum DialPortBase As Integer
        FON = 1
        Fax = 5
        IP = 20
        S0 = 50
        DECT = 60
        TAM = 600
    End Enum
#End Region

    Private Class FritzBoxTelefonnummernListe
        Private C_hf As Helfer

        Friend Sub New(ByVal HelferKlasse As Helfer)
            Nummernliste = New List(Of FritzBoxTelefonnummer)
            C_hf = HelferKlasse
        End Sub

        Private lNummernListe As List(Of FritzBoxTelefonnummer)
        ''' <summary>
        ''' Liste von Telefonnummern
        ''' </summary>
        ''' <returns></returns>
        Friend Property Nummernliste() As List(Of FritzBoxTelefonnummer)
            Get
                Return lNummernListe
            End Get
            Set(ByVal value As List(Of FritzBoxTelefonnummer))
                lNummernListe = value
            End Set
        End Property

        ''' <summary>
        ''' Fügt eine Telefonnummer <c>NeueTelNr</c> in die Liste ein.
        ''' </summary>
        ''' <param name="NeueTelNr">Die Telefonnummer, die hinzugefügt werden soll</param>
        Friend Sub Add(ByVal NeueTelNr As FritzBoxTelefonnummer)
            Dim tmpTelNr As FritzBoxTelefonnummer
            If NeueTelNr.TelNr.StartsWith("SIP") Then
                tmpTelNr = Nummernliste.Find(Function(SIP) SIP.ID0 = CInt(NeueTelNr.TelNr.Replace("SIP", "")))
                If Not tmpTelNr.TelNr = DataProvider.P_Def_LeerString Then
                    NeueTelNr.TelNr = tmpTelNr.TelNr
                End If
            Else
                NeueTelNr.TelNr = C_hf.EigeneVorwahlenEntfernen(NeueTelNr.TelNr)
            End If

            Nummernliste.Add(NeueTelNr)
            tmpTelNr = Nothing
        End Sub

        ''' <summary>
        ''' Gibt alle einmaligen Nummern in einem String-Array zurück.
        ''' </summary>
        Friend Function EinmaligeNummernString() As String()
            Dim retVal(-1) As String

            For Each Telefonnummer As FritzBoxTelefonnummer In Nummernliste
                If Not retVal.Contains(Telefonnummer.TelNr) Then
                    ReDim Preserve retVal(UBound(retVal) + 1)
                    retVal(UBound(retVal)) = Telefonnummer.TelNr
                End If
            Next
            Return retVal
        End Function

        ''' <summary>
        ''' Gibt alle einmaligen Nummern in einem String-Array zurück.
        ''' </summary>
        Friend Function EinmaligeNummern() As List(Of FritzBoxTelefonnummer)
            Dim retVal As New List(Of FritzBoxTelefonnummer)

            For Each Telefonnummer As FritzBoxTelefonnummer In Nummernliste
                Dim tmpTelNr As String = Telefonnummer.TelNr
                If retVal.Find(Function(TelNr) TelNr.TelNr = tmpTelNr).TelNr = DataProvider.P_Def_LeerString Then
                    retVal.Add(Telefonnummer)
                End If
            Next
            Return retVal
        End Function

        ''' <summary>
        ''' Speichert die Telefonnummern in die Liste
        ''' </summary>
        ''' <param name="C_XML">Klasse für das Handling der XML-Datei</param>
        ''' <param name="XMLDatei">Die XML-Datei, in welche die Daten geschrieben werden.</param>
        Friend Sub SpeicherNummer(ByVal C_XML As XML, ByVal XMLDatei As XmlDocument)
            Dim xPathTeile As New ArrayList

            With xPathTeile
                .Clear()
                .Add("Telefone")
                .Add("Nummern")
                .Add(DataProvider.P_Def_ErrorMinusOne_String)
            End With

            For Each tmpTelNr As FritzBoxTelefonnummer In Nummernliste
                With tmpTelNr
                    xPathTeile.Item(xPathTeile.Count - 1) = [Enum].GetName(GetType(TelTyp), .TelTyp)
                    C_XML.Write(XMLDatei, xPathTeile, .TelNr, "ID", CStr(.ID0))
                End With
            Next

            ' Aufräumen
            xPathTeile = Nothing
        End Sub
    End Class

    Private Class FritzBoxTelefonListe
        Private C_hf As Helfer

        Friend Sub New(ByVal HelferKlasse As Helfer)
            Telefonliste = New List(Of FritzBoxTelefon)
            C_hf = HelferKlasse
        End Sub

        Private lTelefonListe As List(Of FritzBoxTelefon)
        ''' <summary>
        ''' Liste von Telefonen
        ''' </summary>
        ''' <returns></returns>
        Friend Property Telefonliste() As List(Of FritzBoxTelefon)
            Get
                Return lTelefonListe
            End Get
            Set(ByVal value As List(Of FritzBoxTelefon))
                lTelefonListe = value
            End Set
        End Property

        ''' <summary>
        ''' Fügt eine Telefonnummer <c>NeueTelNr</c> in die Liste ein.
        ''' </summary>
        ''' <param name="NeuesTelefon">Das Telefon, welches hinzugefügt werden soll</param>
        Friend Sub Add(ByVal NeuesTelefon As FritzBoxTelefon)
            Telefonliste.Add(NeuesTelefon)
        End Sub

        Friend Sub SpeicherTelefone(ByVal C_XML As XML, ByVal XMLDatei As XmlDocument)
            Dim xPathTeile As New ArrayList
            Dim NodeNames As New ArrayList
            Dim NodeValues As New ArrayList
            Dim AttributeNames As New ArrayList
            Dim AttributeValues As New ArrayList

            With xPathTeile
                .Clear()
                .Add(XMLDatei.DocumentElement.Name)
                .Add("Telefone")
                .Add("Telefone")
                .Add(DataProvider.P_Def_ErrorMinusOne_String)
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

            For Each tmpTelefon As FritzBoxTelefon In Telefonliste
                With tmpTelefon
                    xPathTeile.Item(xPathTeile.Count - 1) = [Enum].GetName(GetType(TelTyp), .TelTyp)

                    NodeValues.Item(NodeNames.IndexOf("TelName")) = .TelName
                    NodeValues.Item(NodeNames.IndexOf("TelNr")) = Join(.EingehendeNummern.EinmaligeNummernString, ";")
                    AttributeValues.Item(AttributeNames.IndexOf("Dialport")) = .Dialport
                    AttributeValues.Item(AttributeNames.IndexOf("Fax")) = C_hf.IIf(.IsFax, "1", "0")
                    C_XML.AppendNode(XMLDatei, xPathTeile, C_XML.CreateXMLNode(XMLDatei, "Telefon", NodeNames, NodeValues, AttributeNames, AttributeValues))
                End With
            Next

            ' Aufräumen
            xPathTeile = Nothing
            NodeNames = Nothing
            NodeValues = Nothing
            AttributeNames = Nothing
            AttributeValues = Nothing
        End Sub

    End Class

    Private Structure FritzBoxTelefonnummer
#Region "Datenfelder"
        Private sTelNr As String
        ''' <summary>
        ''' Die komplette unformatierte Telefonnummer 
        ''' </summary>
        Friend Property TelNr() As String
            Get
                Return sTelNr
            End Get
            Set(ByVal value As String)
                sTelNr = value
            End Set
        End Property

        Private sTelTyp As TelTyp
        ''' <summary>
        ''' Der Telefontyp der Telefonnummer: FON, DECT, VOIP, S0
        ''' </summary>
        Friend Property TelTyp() As TelTyp
            Get
                Return sTelTyp
            End Get
            Set(ByVal value As TelTyp)
                sTelTyp = value
            End Set
        End Property

        Private iID0 As Integer
        ''' <summary>
        ''' Eine eindeutige Identifikation der Telefonnummer
        ''' </summary>
        ''' <returns></returns>
        Public Property ID0() As Integer
            Get
                Return iID0
            End Get
            Set(ByVal value As Integer)
                iID0 = value
            End Set
        End Property

        Private iID1 As Integer
        ''' <summary>
        ''' Eine eindeutige Identifikation der Telefonnummer
        ''' </summary>
        ''' <returns></returns>
        Public Property ID1() As Integer
            Get
                Return iID1
            End Get
            Set(ByVal value As Integer)
                iID1 = value
            End Set
        End Property
#End Region
    End Structure

    Private Class FritzBoxTelefon
        Private C_hf As Helfer

        Friend Sub New(ByVal HelferKlasse As Helfer)
            C_hf = HelferKlasse
            EingehendeNummern = New FritzBoxTelefonnummernListe(C_hf)
        End Sub
#Region "Datenfelder"
        Private sTelName As String
        ''' <summary>
        ''' Der Telefonname des Telefons
        ''' </summary>
        Friend Property TelName() As String
            Get
                Return sTelName
            End Get
            Set(ByVal value As String)
                sTelName = value
            End Set
        End Property

        Private sTelTyp As TelTyp
        ''' <summary>
        ''' Der Telefontyp des Telefons: FON, DECT, VOIP, S0
        ''' </summary>
        Friend Property TelTyp() As TelTyp
            Get
                Return sTelTyp
            End Get
            Set(ByVal value As TelTyp)
                sTelTyp = value
            End Set
        End Property

        Private sAusgehendeNummer As FritzBoxTelefonnummer
        ''' <summary>
        ''' Ausgehende Nummer des Telefons
        ''' </summary>
        Friend Property AusgehendeNummer() As FritzBoxTelefonnummer
            Get
                Return sAusgehendeNummer
            End Get
            Set(ByVal value As FritzBoxTelefonnummer)
                sAusgehendeNummer = value
            End Set
        End Property

        Private sEingehendeNummern As FritzBoxTelefonnummernListe
        ''' <summary>
        ''' Liste der eingehenden Nummern, auf die das Telefon reagiert
        ''' </summary>
        ''' <returns></returns>
        Friend Property EingehendeNummern() As FritzBoxTelefonnummernListe
            Get
                Return sEingehendeNummern
            End Get
            Set(ByVal value As FritzBoxTelefonnummernListe)
                sEingehendeNummern = value
            End Set
        End Property

        Private bIsFax As Boolean
        ''' <summary>
        ''' Gibt an oder legt fest, ob es sich bei dem Telfon um ein Fax handelt
        ''' </summary>
        Public Property IsFax() As Boolean
            Get
                Return bIsFax
            End Get
            Set(ByVal value As Boolean)
                bIsFax = value
            End Set
        End Property

        Private iDialport As Integer
        ''' <summary>
        ''' Der Dialport des Telefons
        ''' </summary>
        ''' <returns></returns>
        Public Property Dialport() As Integer
            Get
                Return iDialport
            End Get
            Set(ByVal value As Integer)
                iDialport = value
            End Set
        End Property
#End Region
    End Class

#End Region

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
        Set(ByVal value As String)
            sSID = value
        End Set
    End Property

    Private Property P_Firmware() As String
        Get
            Return sFirmware
        End Get
        Set(ByVal value As String)
            sFirmware = value
        End Set
    End Property

    Private ReadOnly Property P_FritzBoxVorhanden(ByVal FritzBoxAdresse As String) As Boolean
        Get
            If C_DP.P_CBForceFBAddr Then
                C_hf.httpGET("http://" & FritzBoxAdresse, C_DP.P_EncodingFritzBox, FBFehler)
                Return Not FBFehler
            Else
                Return C_hf.Ping(FritzBoxAdresse)
            End If
        End Get
    End Property

    Friend ReadOnly Property P_FritzBoxTyp As String
        Get
            Return ThisFBFirmware.FritzBoxTyp
        End Get
    End Property

    Friend ReadOnly Property P_FritzBoxFirmware As String
        Get
            Return ThisFBFirmware.str1 & "." & ThisFBFirmware.str2 & "." & ThisFBFirmware.str3 & "-" & ThisFBFirmware.Revision
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
        ''' Der Fritz!Box Typ
        ''' </summary>
        Friend FritzBoxTyp As String

        ''' <summary>
        ''' Setzt die internen Variablen
        ''' </summary>
        ''' <param name="FirmwareMinusRevision">Die Firmware in der Form XX.YY.ZZ-Revision</param>
        Friend Overloads Sub SetFirmware(ByVal FirmwareMinusRevision As String)
            Dim tmp() As String

            tmp = Split(FirmwareMinusRevision, "-", , CompareMethod.Text)

            If tmp.Count = 2 Then Revision = tmp(1)

            tmp = Split(tmp(0), ".", , CompareMethod.Text)
            If tmp.Count = 3 Then
                If Len(tmp(tmp.Count - 3)) = 3 Then
                    str1 = Format(CInt(tmp(tmp.Count - 3)), "000")
                Else
                    str1 = Format(CInt(tmp(tmp.Count - 3)), "00")
                End If
            End If
            str2 = Format(CInt(tmp(tmp.Count - 2)), "00")
            str3 = Format(CInt(tmp(tmp.Count - 1)), "00")
        End Sub


        ''' <summary>
        ''' Liest die Firmware und den Fritz!Box Typ aus dem ServiceCode der Fritz!Box aus 
        ''' </summary>
        ''' <param name="ServiceCode">ServiceCode der Fritz!Box (http://fritz.box/cgi-bin/system_status)</param>
        Friend Overloads Sub SetFirmware(ByVal ServiceCode() As String)
            Dim idx As Integer

            ' FRITZ!Box 6360 Cable (kdg)-Kabel-132103-010101-320574-607226-787902-850606-30492-kdg
            ' Es muss darauf geachtet werden, dass die entscheidenden Datenfelder dynamisch ermittelt werden. 
            ' Das letzte Datenfeld (n) ist das Brandung
            ' Das vorletzte Datenfeld (n-1) ist die Revision
            ' Das vor-vor-letze Datenfeld (n-2) ist die Firmware

            ' Lese den Fritz!Box Typ
            idx = LBound(ServiceCode)
            FritzBoxTyp = ServiceCode(idx)

            ' Lese Firmwareversion aus
            idx = UBound(ServiceCode) - 2
            If IsNumeric(ServiceCode(idx)) AndAlso Len(ServiceCode(idx)) = 6 Then
                str1 = Mid(ServiceCode(idx), 1, 2)
                str2 = Mid(ServiceCode(idx), 3, 2)
                str3 = Mid(ServiceCode(idx), 5, 2)
            End If

            ' Lese Revision aus
            idx = UBound(ServiceCode) - 1
            If IsNumeric(ServiceCode(idx)) AndAlso Len(ServiceCode(idx)) = 5 Then
                Revision = ServiceCode(idx)
            End If
            ' Lese Firmwareversion und Sub-Version aus
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
    ''' http://P_ValidFBAdr/data.lua
    ''' </summary>
    Private ReadOnly Property P_Link_FB_Data() As String
        Get
            Return P_Link_FB_Basis & "/data.lua"
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
    ''' http://<c>P_ValidFBAdr</c>/query.lua/?sid=<c>sSID</c>&amp;<c>sAbfrage</c>
    ''' </summary>
    ''' <param name="sSID">Session ID</param>
    ''' <param name="sAbfrage">Zu übersendende Abfrage</param>
    Private ReadOnly Property P_Link_Query(ByVal sSID As String, ByVal sAbfrage As String) As String
        Get
            Return P_Link_FB_Basis & "/query.lua?sid=" & sSID & "&" & sAbfrage
        End Get
    End Property

    'Private ReadOnly Property P_Link_TwoFactor(ByVal sSID As String) As String
    '    Get
    '        Return P_Link_FB_Basis & "/twofactor.lua?sid=" & sSID
    '    End Get
    'End Property

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
    ''' "SIP" &amp; "=" &amp; "sip:settings/sip/list(activated,displayname,ID)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_SIP() As String
        Get
            Return "SIP" & "=" & "sip:settings/sip/list(activated,displayname,ID)"
        End Get
    End Property

    Private ReadOnly Property P_Query_FB_TelList_Header(ByVal jdx As Integer) As String
        Get
            Return "TelNr" & jdx
        End Get
    End Property

    ''' <summary>
    ''' "MSN" &amp; idx &amp; "Nr" &amp; jdx &amp; "=telcfg:settings/MSN/Port" &amp; idx &amp; "/MSN" &amp; jdx
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_MSN_TelNrList(ByVal idx As Integer, ByVal jdx As Integer) As String
        Get
            Return P_Query_FB_TelList_Header(jdx) & "=telcfg:settings/MSN/Port" & idx & "/MSN" & jdx
        End Get
    End Property

    ''' <summary>
    ''' "VOIP" &amp; idx &amp; "Nr" &amp; jdx &amp; "=telcfg:settings/VoipExtension" &amp; idx &amp; "/Number" &amp; jdx
    ''' </summary>
    ''' <param name="idx">Der Index des Eintrages</param>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_VOIP_TelNrList(ByVal idx As Integer, ByVal jdx As Integer) As String
        Get
            Return P_Query_FB_TelList_Header(jdx) & "=telcfg:settings/VoipExtension" & idx & "/Number" & jdx
        End Get
    End Property


    ''' <summary>
    ''' "FON=telcfg:settings/MSN/Port/list(Name,Fax,GroupCall,AllIncomingCalls,OutDialing)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_FON_List() As String
        Get
            Return "FON=telcfg:settings/MSN/Port/list(Name,Fax)"
            'Return "FON=telcfg:settings/MSN/Port/list(Name,Fax,GroupCall,AllIncomingCalls,OutDialing)"
        End Get
    End Property

    ''' <summary>
    ''' "DECT=telcfg:settings/Foncontrol/User/list(Name,Type,Intern,Id)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_DECT_List() As String
        Get
            Return "DECT=telcfg:settings/Foncontrol/User/list(Name,Intern)"
        End Get
    End Property

    ''' <summary>
    ''' "VOIP=telcfg:settings/VoipExtension/list(enabled,Name,RingOnAllMSNs)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_VOIP_List() As String
        Get
            Return "VOIP=telcfg:settings/VoipExtension/list(enabled,Name)"
        End Get
    End Property

    ''' <summary>
    ''' "TAM=tam:settings/TAM/list(Active,Name,Display,MSNBitmap)"
    ''' </summary>
    ''' <returns>Der zusammengefügte String</returns>
    Private ReadOnly Property P_Query_FB_TAM_List() As String
        Get
            Return "TAM=tam:settings/TAM/list(Active,Name)"
        End Get
    End Property

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
    Private ReadOnly Property P_Query_FB_S0(ByVal Type As String, ByVal idx As Integer) As String
        Get
            Return "S0" & Type & idx & "=telcfg:settings/NTHotDialList/" & Type & idx
        End Get
    End Property


    '''' <summary>
    '''' "S0Name" &amp; idx &amp; "=telcfg:settings/NTHotDialList/Name" &amp; idx
    '''' <param name="idx">Der Index des Eintrages</param>
    '''' </summary>
    '''' <returns>Der zusammengefügte String</returns>
    'Private ReadOnly Property P_Query_FB_S0_List(ByVal idx As Integer) As String
    '    Get
    '        Return "S0Name" & idx & "=telcfg:settings/NTHotDialList/Name" & idx
    '    End Get
    'End Property

    '''' <summary>
    '''' "S0TelNr" &amp; idx &amp; "=telcfg:settings/NTHotDialList/Number" &amp; idx
    '''' <param name="idx">Der Index des Eintrages</param>
    '''' </summary>
    '''' <returns>Der zusammengefügte String</returns>
    'Private ReadOnly Property P_Query_FB_S0_TelNr(ByVal idx As Integer) As String
    '    Get
    '        Return "S0TelNr" & idx & "=telcfg:settings/NTHotDialList/Number" & idx
    '    End Get
    'End Property

    '''' <summary>
    '''' "S0Type" &amp; idx &amp; "=telcfg:settings/NTHotDialList/Type" &amp; idx
    '''' <param name="idx">Der Index des Eintrages</param>
    '''' </summary>
    '''' <returns>Der zusammengefügte String</returns>
    'Private ReadOnly Property P_Query_FB_S0_Type(ByVal idx As Integer) As String
    '    Get
    '        Return "S0Type" & idx & "=telcfg:settings/NTHotDialList/Type" & idx
    '    End Get
    'End Property

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

    Public Sub New(ByVal DataProviderKlasse As DataProvider, ByVal HelferKlasse As Helfer, ByVal CryptKlasse As Rijndael, ByVal XMLKlasse As XML, ByVal UPnpKlasse As FritzBoxServices)

        C_DP = DataProviderKlasse
        C_hf = HelferKlasse
        C_Crypt = CryptKlasse
        C_XML = XMLKlasse
        C_FBoxUPnP = UPnpKlasse

        P_SID = DataProvider.P_Def_SessionID  ' Startwert: Ungültige SID

        C_DP.P_ValidFBAdr = C_hf.ValidIP(C_DP.P_TBFBAdr)

        If P_FritzBoxVorhanden(C_DP.P_ValidFBAdr) Then
            ' Übergebe an die UPnP-Klasse die Daten der Fritz!Box
            'If C_DP.P_RBFBComUPnP Then
            '    C_FBoxUPnP.SetFritzBoxData(C_DP.P_ValidFBAdr, C_DP.P_TBBenutzer, C_Crypt.DecryptString128Bit(C_DP.P_TBPasswort, C_DP.GetSettingsVBA("Zugang", DataProvider.P_Def_ErrorMinusOne_String)))
            'End If
            ' Setze Firmware der Fritz!Box
            FBFirmware()

            C_DP.P_EncodingFritzBox = C_hf.GetEncoding(C_hf.httpGET(P_Link_FB_Basis, C_DP.P_EncodingFritzBox, FBFehler))
            C_DP.SpeichereXMLDatei()
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

        slogin_xml = C_hf.httpGET(P_Link_FB_LoginLuaTeil1(P_SID), C_DP.P_EncodingFritzBox, FBFehler)

        If Not FBFehler Then

            If InStr(slogin_xml, "BlockTime", CompareMethod.Text) = 0 Then
                slogin_xml = C_hf.httpGET(P_Link_FB_LoginAltTeil1(P_SID), C_DP.P_EncodingFritzBox, FBFehler)
            End If

            If Not (slogin_xml = DataProvider.P_Def_LeerString Or slogin_xml.Contains("FRITZ!Box Anmeldung")) Then

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

                            If Not P_SpeichereDaten Then PushStatus("Challenge: " & sChallenge & vbNewLine & "SIDResponse: " & sSIDResponse)

                            If ThisFBFirmware.ISEmpty Then FBFirmware()

                            If ThisFBFirmware.ISLargerOREqual("5.29") Then
                                'If .InnerXml.Contains("Rights") Then
                                ' Lua Login ab Firmware xxx.05.29 / xxx.05.5x
                                sBlockTime = .Item("SessionInfo").Item("BlockTime").InnerText
                                If sBlockTime = DataProvider.P_Def_StringNull Then ' "0"
                                    'sLink = "http://" & C_DP.P_ValidFBAdr & "/login_sid.lua?username=" & sFBBenutzer & "&response=" & sSIDResponse
                                    sResponse = C_hf.httpGET(P_Link_FB_LoginLuaTeil2(sFBBenutzer, sSIDResponse), C_DP.P_EncodingFritzBox, FBFehler)
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
                                sResponse = C_hf.httpPOST(P_Link_FB_ExtBasis, P_Link_FB_LoginAltTeil2(sSIDResponse), C_DP.P_EncodingFritzBox)
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
                                    If Not Split(.SelectSingleNode("//Rights").InnerText, "2").Contains("BoxAdmin") Then
                                        'If Not C_hf.IsOneOf("BoxAdmin", Split(.SelectSingleNode("//Rights").InnerText, "2")) Then
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
        Response = C_hf.httpGET(P_Link_FB_LoginLuaTeil1(sSID), C_DP.P_EncodingFritzBox, FBFehler)
        If Not FBFehler Then
            With xml
                .LoadXml(Response)
                'If .InnerXml.Contains("Rights") Then
                '    sLink = C_DP.P_Link_FB_LogoutLuaNeu(sSID) '"http://" & C_DP.P_ValidFBAdr & "/home/home.lua?sid=" & sSID & "&logout=1"
                'Else
                '    sLink = C_DP.P_Link_FB_LogoutLuaAlt(sSID) '"http://" & C_DP.P_ValidFBAdr & "/logout.lua?sid=" & sSID
                'End If

                'IIf(.InnerXml.Contains("Rights"), C_DP.P_Link_FB_LogoutLuaNeu(sSID), C_DP.P_Link_FB_LogoutLuaAlt(sSID))
                Response = C_hf.httpGET(C_hf.IIf(.InnerXml.Contains("Rights"), P_Link_FB_LogoutLuaNeu(sSID), P_Link_FB_LogoutLuaAlt(sSID)), C_DP.P_EncodingFritzBox, FBFehler)
            End With
            xml = Nothing
            C_hf.KeyChange()
            If Not FBFehler Then
                If Not InStr(Response, DataProvider.P_FritzBox_LogoutTestString1, CompareMethod.Text) = 0 Or Not InStr(Response, DataProvider.P_FritzBox_LogoutTestString2, CompareMethod.Text) = 0 Then
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
    Friend Sub FritzBoxDaten(ByVal Debug As Boolean, ByVal VonFritzBox As Boolean)
        'Dim sLink As String
        Dim tempstring As String
        Dim tempstring_code As String

        If P_SpeichereDaten Then PushStatus(DataProvider.P_Def_FritzBoxName & " Adresse: " & C_DP.P_TBFBAdr)

        FBLogin()
        If Not P_SID = DataProvider.P_Def_SessionID Then

            If ThisFBFirmware.ISLargerOREqual("6.05") Then
                PushStatus("Starte AuswertungV3")

                FritzBoxDatenV3(Debug, VonFritzBox)
            ElseIf ThisFBFirmware.ISLargerOREqual("5.25") Then

                tempstring = C_hf.httpGET(P_Link_FB_Tel1(P_SID), C_DP.P_EncodingFritzBox, FBFehler)
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

    Private Sub FritzBoxDatenV1()
        PushStatus(DataProvider.P_FritzBox_Tel_RoutineBis525)

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

        Dim PortName() As String = {"readFon123", "readNTHotDialList", "readDect1", "readFonControl", "readVoipExt", "readTam", "readFaxMail"}

        Dim EndPortName() As String = {"return list", "return list", "return list", "return list", "return Result", "return list", "return list"}

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

        sLink = P_Link_FB_TelAlt1(P_SID)

        If P_SpeichereDaten Then PushStatus(DataProvider.P_FritzBox_Tel_AlteRoutine2(sLink))
        tempstring = C_hf.httpGET(sLink, C_DP.P_EncodingFritzBox, FBFehler)
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
                    PushStatus(DataProvider.P_FritzBox_Tel_NrFound("MSN", i, TelNr))
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
                    PushStatus(DataProvider.P_FritzBox_Tel_NrFound("SIP", i, TelNr))
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
                    PushStatus(DataProvider.P_FritzBox_Tel_NrFound("TAM", i, TelNr))
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
                PushStatus(DataProvider.P_FritzBox_Tel_NrFound("POTS", 0, POTS))
                If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, POTS, "ID", DataProvider.P_Def_StringNull)
            End If

            'Mobilnummer ermitteln
            xPathTeile.Item(xPathTeile.IndexOf("POTS")) = "Mobil"
            Mobil = C_hf.StringEntnehmen(tempstring, "nrs.mobil = '", "'")
            If Not Mobil = DataProvider.P_Def_ErrorMinusOne_String And Not Mobil = DataProvider.P_Def_LeerString Then
                Mobil = C_hf.EigeneVorwahlenEntfernen(Mobil)
                PushStatus(DataProvider.P_FritzBox_Tel_NrFound("Mobil", 0, Mobil))
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
                                                            If Split(NetInfoPush, ";", , CompareMethod.Text).Contains("62" & ID) Then
                                                                'If C_hf.IsOneOf("62" & ID, Split(NetInfoPush, ";", , CompareMethod.Text)) Then
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
        PushStatus(DataProvider.P_FritzBox_Tel_RoutineAb525)

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
                    PushStatus(DataProvider.P_FritzBox_Tel_NrFound("SIP", CInt(Node), TelNr))
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
                        PushStatus(DataProvider.P_FritzBox_Tel_NrFound("MSN", i, TelNr))
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

                                If Not MSN.Contains(TelNr) Then
                                    'If Not .IsOneOf(TelNr, MSN) Then
                                    For k = 0 To 9
                                        If MSN(k) = DataProvider.P_Def_LeerString Then
                                            MSN(k) = TelNr
                                            PushStatus(DataProvider.P_FritzBox_Tel_NrFound("MSN", i, TelNr))
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
                        PushStatus(DataProvider.P_FritzBox_Tel_NrFound("TAM", i, TelNr))
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
                        PushStatus(DataProvider.P_FritzBox_Tel_NrFound("FAX", i, TelNr))
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
                PushStatus(DataProvider.P_FritzBox_Tel_NrFound("POTS", 0, POTS))
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
                If P_SpeichereDaten Then C_XML.Write(C_DP.XMLDoc, xPathTeile, Mobil, "ID", CStr(DataProvider.P_Def_MobilDialPort))
            End If

            SIP = C_hf.ClearStringArray(SIP, True, True, True)
            MSN = C_hf.ClearStringArray(MSN, True, True, True)
            FAX = C_hf.ClearStringArray(FAX, True, True, True)

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
                    tmparray = C_hf.ClearStringArray(tmparray, True, True, True)
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
                        For Each Nr As String In C_hf.ClearStringArray(tmpstrUser, True, True, True)
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
                DialPort = CStr(DataProvider.P_Def_MobilDialPort)
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

    Private Sub FritzBoxDatenV3(ByVal Debug As Boolean, ByVal LadeVonFritzBox As Boolean)
        PushStatus(DataProvider.P_FritzBox_Tel_RoutineAb605)

        Dim C_JSON As New JSON
        Dim TelQuery As New List(Of String)
        Dim FritzBoxJSONTelNr1 As FritzBoxJSONTelNrT1
        Dim FritzBoxJSONTelefone1 As FritzBoxJSONTelefone1
        Dim FritzBoxJSONTelefone2 As FritzBoxJSONTelefone2

        Dim TelefonNummern As New FritzBoxTelefonnummernListe(C_hf)
        Dim Telefone As New FritzBoxTelefonListe(C_hf)

        Dim tmpTelNr As FritzBoxTelefonnummer
        Dim tmpTelNrList As TelNrList
        Dim tmpTelefon As FritzBoxTelefon
        Dim tmpStrArr As String()
        Dim idx As Integer
        Dim jdx As Integer
        Dim kdx As Integer

        Dim QueryID() As String = {"MainQuery01", "MainQuery02", "MainQuery03", "ListMSN", "ListVOIP"}

        With TelQuery

            ' POTS Nummer
            .Add(P_Query_FB_POTS)
            ' Mobilnummer
            .Add(P_Query_FB_Mobile)

            ' FON-Name
            For i = 0 To 2
                .Add(P_Query_FB_FON(i))
            Next

            For i = 0 To 9
                ' Anrufbeantworter-Nummern
                .Add(P_Query_FB_TAM(i))
                ' Fax-Nummern
                .Add(P_Query_FB_FAX(i))
                ' Klassische analoge MSN
                .Add(P_Query_FB_MSN(i))
                ' VoIP-Nummern
                .Add(P_Query_FB_VOIP(i))
            Next

            ' SIP-Nummern
            .Add(P_Query_FB_SIP)
            ' Führt das Fritz!Box Query aus und gibt die ersten Daten der Telefonnummern zurück
            If LadeVonFritzBox Then
                PushStatus(DataProvider.P_FritzBox_Tel_SendQuery(1, 3))
                FritzBoxJSONTelNr1 = C_JSON.GetFirstValues(FritzBoxQuery(String.Join("&", TelQuery.ToArray), QueryID(0), Debug))
            Else
                PushStatus("Lade Datei: " & C_DP.P_Debug_PfadKonfig & IO.Path.DirectorySeparatorChar & QueryID(0) & ".txt")
                FritzBoxJSONTelNr1 = C_JSON.GetFirstValues(C_DP.Debug_getFileContend(QueryID(0)))
            End If

            .Clear()
        End With

        ReDim tmpStrArr(-1)

        If Not Debug Then

            With FritzBoxJSONTelNr1
                PushStatus("Ermittle vorhandene Telefonnummern...")
                ' Verarbeite Telefonnummern: MSN, TAM, FAX

                For jdx = 1 To 3
                    Select Case jdx
                        Case 1
                            ' Verarbeite MSN-Nummern
                            tmpStrArr = .MSNList
                        Case 2
                            ' Verarbeite TAM-Nummern (Anrufbeantworter)
                            tmpStrArr = .TAMList
                        Case 3
                            ' Verarbeite FAX-Nummern
                            tmpStrArr = .FAXList
                    End Select

                    For idx = LBound(tmpStrArr) To UBound(tmpStrArr)
                        If Not tmpStrArr(idx) = DataProvider.P_Def_LeerString Then
                            tmpTelNr = New FritzBoxTelefonnummer
                            tmpTelNr.TelNr = tmpStrArr(idx)
                            tmpTelNr.ID0 = idx
                            Select Case jdx
                                Case 1
                                    tmpTelNr.TelTyp = TelTyp.MSN
                                Case 2
                                    tmpTelNr.TelTyp = TelTyp.TAM
                                Case 3
                                    tmpTelNr.TelTyp = TelTyp.FAX
                            End Select
                            TelefonNummern.Add(tmpTelNr)
                            PushStatus(DataProvider.P_FritzBox_Tel_NrFound([Enum].GetName(GetType(TelTyp), tmpTelNr.TelTyp), tmpTelNr.ID0, tmpTelNr.TelNr))
                        End If
                    Next
                Next

                ' Verarbeite Telefonnummern: SIP
                For Each SIPi As SIPEntry In FritzBoxJSONTelNr1.SIP
                    With SIPi
                        If CBool(.activated) Then
                            tmpTelNr = New FritzBoxTelefonnummer
                            tmpTelNr.TelNr = .displayname
                            tmpTelNr.ID0 = CInt(.ID)
                            tmpTelNr.TelTyp = TelTyp.SIP
                            TelefonNummern.Add(tmpTelNr)
                            PushStatus(DataProvider.P_FritzBox_Tel_NrFound([Enum].GetName(GetType(TelTyp), tmpTelNr.TelTyp), tmpTelNr.ID0, tmpTelNr.TelNr))
                        End If
                    End With
                Next

                ' Verarbeite Telefonnummern: POTS
                If Not .POTS = DataProvider.P_Def_LeerString Then
                    tmpTelNr = New FritzBoxTelefonnummer
                    tmpTelNr.TelNr = .POTS
                    tmpTelNr.TelTyp = TelTyp.POTS
                    TelefonNummern.Add(tmpTelNr)
                    PushStatus(DataProvider.P_FritzBox_Tel_NrFound([Enum].GetName(GetType(TelTyp), tmpTelNr.TelTyp), tmpTelNr.ID0, tmpTelNr.TelNr))
                End If

                ' Verarbeite Telefonnummern: Mobil
                If Not .Mobile = DataProvider.P_Def_LeerString Then
                    tmpTelNr = New FritzBoxTelefonnummer
                    tmpTelNr.TelNr = .Mobile
                    tmpTelNr.TelTyp = TelTyp.Mobil
                    TelefonNummern.Add(tmpTelNr)
                    PushStatus(DataProvider.P_FritzBox_Tel_NrFound([Enum].GetName(GetType(TelTyp), tmpTelNr.TelTyp), tmpTelNr.ID0, tmpTelNr.TelNr))
                End If
            End With 'FritzBoxJSONTelNr1
        End If

        ' Verarbeite Telefonnummern über die angeschlossenen Geräte
        For kdx = 0 To 1
            Select Case kdx
                Case 0
                    tmpStrArr = FritzBoxJSONTelNr1.MSNPortEnabled
                Case 1
                    tmpStrArr = FritzBoxJSONTelNr1.VOIPPortEnabled
            End Select

            For idx = LBound(tmpStrArr) To UBound(tmpStrArr)
                If (kdx = 0 And Not tmpStrArr(idx) = DataProvider.P_Def_LeerString) OrElse (kdx = 1 And tmpStrArr(idx) = "1") Then
                    ' Füge alle 10 möglichen zugeordneten Nummern hinzu
                    TelQuery.Clear()
                    For jdx = 0 To 9
                        Select Case kdx
                            Case 0
                                TelQuery.Add(P_Query_FB_MSN_TelNrList(idx, jdx))
                            Case 1
                                TelQuery.Add(P_Query_FB_VOIP_TelNrList(idx, jdx))
                        End Select
                    Next
                    ' Pro Gerät erfolgt eine Abfrage an die Fritz!Box
                    If LadeVonFritzBox Then
                        tmpTelNrList = C_JSON.GetTelNrListJSON(FritzBoxQuery(String.Join("&", TelQuery.ToArray), QueryID(kdx + 3) & idx, Debug))
                    Else
                        PushStatus("Lade Datei: " & C_DP.P_Debug_PfadKonfig & IO.Path.DirectorySeparatorChar & QueryID(kdx + 3) & idx & ".txt")
                        tmpTelNrList = C_JSON.GetTelNrListJSON(C_DP.Debug_getFileContend(QueryID(kdx + 3) & idx))
                    End If
                    With tmpTelNrList
                        For jdx = .LBound To .UBound
                            If Not Debug Then
                                If Not .Item(jdx) = DataProvider.P_Def_LeerString Then
                                    tmpTelNr = New FritzBoxTelefonnummer
                                    tmpTelNr.TelNr = .Item(jdx)
                                    tmpTelNr.ID0 = jdx
                                    tmpTelNr.ID1 = idx
                                    Select Case kdx
                                        Case 0
                                            tmpTelNr.TelTyp = TelTyp.MSN
                                        Case 1
                                            tmpTelNr.TelTyp = TelTyp.IP
                                    End Select
                                    TelefonNummern.Add(tmpTelNr)
                                    PushStatus(DataProvider.P_FritzBox_Tel_NrFound([Enum].GetName(GetType(TelTyp), tmpTelNr.TelTyp), tmpTelNr.ID0, tmpTelNr.TelNr))
                                End If
                            End If
                        Next
                    End With
                End If
            Next
        Next

        With TelQuery
            .Clear()
            .Add(P_Query_FB_FON_List)       ' FON
            .Add(P_Query_FB_DECT_List)      ' DECT (Teil1)
            .Add(P_Query_FB_VOIP_List)      ' IP-Telefoen
            .Add(P_Query_FB_TAM_List)       ' TAM

            For idx = 1 To 8
                .Add(P_Query_FB_S0("Name", idx))
            Next
        End With 'TelQuery

        If LadeVonFritzBox Then
            PushStatus(DataProvider.P_FritzBox_Tel_SendQuery(2, 3))
            FritzBoxJSONTelefone1 = C_JSON.GetSecondValues(FritzBoxQuery(String.Join("&", TelQuery.ToArray), QueryID(1), Debug))
        Else
            PushStatus("Lade Datei: " & C_DP.P_Debug_PfadKonfig & IO.Path.DirectorySeparatorChar & QueryID(1) & ".txt")
            FritzBoxJSONTelefone1 = C_JSON.GetSecondValues(C_DP.Debug_getFileContend(QueryID(1)))
        End If


        With FritzBoxJSONTelefone1
            TelQuery.Clear()
            For idx = 0 To 7 'LBound(.S0NameList) + 1 To LBound(.S0NameList) + 1
                If Not .S0NameList(idx) = DataProvider.P_Def_LeerString Then
                    TelQuery.Add(P_Query_FB_S0("Number", idx + 1))
                    TelQuery.Add(P_Query_FB_S0("Type", idx + 1))
                End If
            Next

            For idx = LBound(FritzBoxJSONTelefone1.DECT) To UBound(FritzBoxJSONTelefone1.DECT)
                If Not FritzBoxJSONTelefone1.DECT(idx).Intern = DataProvider.P_Def_Leerzeichen Then
                    TelQuery.Add(P_Query_FB_DECT_RingOnAllMSNs(idx))
                    TelQuery.Add(P_Query_FB_DECT_NrList(idx))
                End If
            Next

            TelQuery.Add(P_Query_FB_FaxMailActive)
            TelQuery.Add(P_Query_FB_MobileName)

        End With ' FritzBoxJSONTelefone1

        If LadeVonFritzBox Then
            PushStatus(DataProvider.P_FritzBox_Tel_SendQuery(3, 3))
            FritzBoxJSONTelefone2 = C_JSON.GetThirdValues(FritzBoxQuery(String.Join("&", TelQuery.ToArray), QueryID(2), Debug))
        Else
            PushStatus("Lade Datei: " & C_DP.P_Debug_PfadKonfig & IO.Path.DirectorySeparatorChar & QueryID(2) & ".txt")
            FritzBoxJSONTelefone2 = C_JSON.GetThirdValues(C_DP.Debug_getFileContend(QueryID(2)))
        End If


        ' Wenn der User auf Probleme geklicckt hat, sollen nur die Dateien ermittelt werden, und nicht noch eine Auswertung gestartet werden, die eventuell zum Absturz führt.
        If Not Debug Then

            PushStatus("Alle relevanten Daten von der Fritz!Box erhalten. Ermittle vorhandene Telefone...")
            'FON
            PushStatus("Verarbeite Geräte: FON")
            For idx = LBound(FritzBoxJSONTelefone1.FON) To UBound(FritzBoxJSONTelefone1.FON)
                With FritzBoxJSONTelefone1.FON(idx)
                    If Not .Name = DataProvider.P_Def_LeerString Then
                        tmpTelefon = New FritzBoxTelefon(C_hf)
                        tmpTelefon.TelTyp = TelTyp.FON
                        tmpTelefon.Dialport = DialPortBase.FON + idx
                        tmpTelefon.IsFax = CBool(.Fax)
                        tmpTelefon.TelName = .Name
                        tmpTelefon.EingehendeNummern.Nummernliste = TelefonNummern.Nummernliste.FindAll(Function(Nummern) Nummern.ID1 = idx And Nummern.TelTyp = TelTyp.MSN)
                        Telefone.Add(tmpTelefon)
                        PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound([Enum].GetName(GetType(TelTyp), tmpTelefon.TelTyp), CStr(tmpTelefon.Dialport), Join(tmpTelefon.EingehendeNummern.EinmaligeNummernString, ","), tmpTelefon.TelName))
                    End If
                End With
            Next

            'DECT
            PushStatus("Verarbeite Geräte: DECT")
            For idx = LBound(FritzBoxJSONTelefone1.DECT) To UBound(FritzBoxJSONTelefone1.DECT)
                With FritzBoxJSONTelefone1.DECT(idx)

                    If Not .Name = DataProvider.P_Def_LeerString Then
                        tmpTelefon = New FritzBoxTelefon(C_hf)
                        tmpTelefon.TelTyp = TelTyp.DECT
                        tmpTelefon.Dialport = DialPortBase.DECT + CInt(Right(.Intern, 1))
                        tmpTelefon.IsFax = False
                        tmpTelefon.TelName = .Name

                        If FritzBoxJSONTelefone2.DECTRingOnAllMSNs(idx) = "1" Then
                            tmpTelefon.EingehendeNummern.Nummernliste = TelefonNummern.EinmaligeNummern
                        Else
                            For Each aktDECTNr As DECTNr In FritzBoxJSONTelefone2.DECTTelNr(idx)
                                If Not aktDECTNr.Number = DataProvider.P_Def_LeerString Then
                                    Dim tmpDectNr As String = aktDECTNr.Number
                                    tmpTelefon.EingehendeNummern.Nummernliste.Add(TelefonNummern.Nummernliste.Find(Function(Nummer) Nummer.TelNr = tmpDectNr))
                                End If
                            Next
                        End If
                        Telefone.Add(tmpTelefon)
                        PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound([Enum].GetName(GetType(TelTyp), tmpTelefon.TelTyp), CStr(tmpTelefon.Dialport), Join(tmpTelefon.EingehendeNummern.EinmaligeNummernString, ","), tmpTelefon.TelName))
                    End If
                End With

            Next

            'IP-Telefone
            PushStatus("Verarbeite Geräte: IP-Telefone")
            For idx = LBound(FritzBoxJSONTelefone1.VOIP) To UBound(FritzBoxJSONTelefone1.VOIP)
                With FritzBoxJSONTelefone1.VOIP(idx)
                    If .enabled = "1" Then
                        tmpTelefon = New FritzBoxTelefon(C_hf)
                        tmpTelefon.TelTyp = TelTyp.IP
                        tmpTelefon.Dialport = DialPortBase.IP + idx
                        tmpTelefon.TelName = .Name
                        tmpTelefon.EingehendeNummern.Nummernliste = TelefonNummern.Nummernliste.FindAll(Function(Nummern) Nummern.ID1 = idx And Nummern.TelTyp = TelTyp.IP)
                        Telefone.Add(tmpTelefon)
                        PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound([Enum].GetName(GetType(TelTyp), tmpTelefon.TelTyp), CStr(tmpTelefon.Dialport), Join(tmpTelefon.EingehendeNummern.EinmaligeNummernString, ","), tmpTelefon.TelName))
                    End If
                End With
            Next

            'S0
            PushStatus("Verarbeite Geräte: S0")
            For idx = 0 To 7
                If Not FritzBoxJSONTelefone1.S0NameList(idx) = DataProvider.P_Def_LeerString And Not FritzBoxJSONTelefone2.S0NumberList(idx) = DataProvider.P_Def_LeerString Then
                    tmpTelefon = New FritzBoxTelefon(C_hf)
                    tmpTelefon.TelTyp = TelTyp.S0
                    tmpTelefon.Dialport = DialPortBase.S0 + idx + 1
                    tmpTelefon.TelName = FritzBoxJSONTelefone1.S0NameList(idx)
                    tmpTelefon.EingehendeNummern.Nummernliste.Add(TelefonNummern.Nummernliste.Find(Function(Nummern) Nummern.TelNr = C_hf.EigeneVorwahlenEntfernen(FritzBoxJSONTelefone2.S0NumberList(idx))))
                    Telefone.Add(tmpTelefon)
                    PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound([Enum].GetName(GetType(TelTyp), tmpTelefon.TelTyp), CStr(tmpTelefon.Dialport), Join(tmpTelefon.EingehendeNummern.EinmaligeNummernString, ","), tmpTelefon.TelName))
                End If
            Next
            If Not Telefone.Telefonliste.Find(Function(Telefon) Telefon.TelTyp = TelTyp.S0) Is Nothing Then
                tmpTelefon = New FritzBoxTelefon(C_hf)
                tmpTelefon.TelTyp = TelTyp.S0
                tmpTelefon.Dialport = DialPortBase.S0
                tmpTelefon.TelName = "ISDN-Basis"
                Telefone.Add(tmpTelefon)
                PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound([Enum].GetName(GetType(TelTyp), tmpTelefon.TelTyp), CStr(tmpTelefon.Dialport), Join(tmpTelefon.EingehendeNummern.EinmaligeNummernString, ","), tmpTelefon.TelName))
            End If

            ' TAM, Anrufbeantworter
            PushStatus("Verarbeite Geräte: Anrufbeantworter")
            For idx = LBound(FritzBoxJSONTelefone1.TAM) To UBound(FritzBoxJSONTelefone1.TAM)
                With FritzBoxJSONTelefone1.TAM(idx)
                    If .Active = "1" Then
                        tmpTelefon = New FritzBoxTelefon(C_hf)
                        tmpTelefon.TelTyp = TelTyp.TAM
                        tmpTelefon.Dialport = DialPortBase.TAM + idx
                        tmpTelefon.TelName = .Name
                        If TelefonNummern.Nummernliste.FindAll(Function(Nummern) Nummern.TelTyp = TelTyp.TAM).Count = 0 Then
                            tmpTelefon.EingehendeNummern.Nummernliste = TelefonNummern.EinmaligeNummern
                        Else
                            tmpTelefon.EingehendeNummern.Nummernliste.Add(TelefonNummern.Nummernliste.Find(Function(Nummer) Nummer.TelTyp = TelTyp.TAM And Nummer.ID0 = idx))
                        End If
                        Telefone.Add(tmpTelefon)
                        PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound([Enum].GetName(GetType(TelTyp), tmpTelefon.TelTyp), CStr(tmpTelefon.Dialport), Join(tmpTelefon.EingehendeNummern.EinmaligeNummernString, ","), tmpTelefon.TelName))
                    End If
                End With
            Next

            ' integrierter Faxempfang
            PushStatus("Verarbeite Gerät: integrierter Faxempfang")
            If FritzBoxJSONTelefone2.FaxMailActive IsNot DataProvider.P_Def_StringNull Then
                tmpTelefon = New FritzBoxTelefon(C_hf)
                tmpTelefon.TelTyp = TelTyp.FAX
                tmpTelefon.Dialport = DialPortBase.Fax
                tmpTelefon.TelName = "Faxempfang"
                tmpTelefon.IsFax = True
                tmpTelefon.EingehendeNummern.Nummernliste = TelefonNummern.Nummernliste.FindAll(Function(Nummern) Nummern.TelTyp = TelTyp.FAX)
                Telefone.Add(tmpTelefon)
                PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound([Enum].GetName(GetType(TelTyp), tmpTelefon.TelTyp), CStr(tmpTelefon.Dialport), Join(tmpTelefon.EingehendeNummern.EinmaligeNummernString, ","), tmpTelefon.TelName))
            End If

            'Mobil
            PushStatus("Verarbeite Gerät: Mobil")
            If TelefonNummern.Nummernliste.Find(Function(Nummer) Nummer.TelTyp = TelTyp.Mobil).TelNr IsNot Nothing Then
                tmpTelefon = New FritzBoxTelefon(C_hf)
                tmpTelefon.TelTyp = TelTyp.Mobil
                tmpTelefon.Dialport = DataProvider.P_Def_MobilDialPort
                tmpTelefon.TelName = FritzBoxJSONTelefone2.MobileName
                tmpTelefon.EingehendeNummern.Nummernliste.Add(TelefonNummern.Nummernliste.Find(Function(Nummer) Nummer.TelTyp = TelTyp.Mobil))
                Telefone.Add(tmpTelefon)
                PushStatus(DataProvider.P_FritzBox_Tel_DeviceFound([Enum].GetName(GetType(TelTyp), tmpTelefon.TelTyp), CStr(tmpTelefon.Dialport), Join(tmpTelefon.EingehendeNummern.EinmaligeNummernString, ","), tmpTelefon.TelName))
            End If
            PushStatus("Verarbeitung der Telefoniegeräte abgeschlossen.")

            If P_SpeichereDaten Then
                PushStatus(" Speicher Telefonnummern und Telefoniegeräte. Lösche alte Daten...")
                C_XML.Delete(C_DP.XMLDoc, "Telefone")
                PushStatus("Speichere Telefonnummern...")
                TelefonNummern.SpeicherNummer(C_XML, C_DP.XMLDoc)
                PushStatus("Speichere Telefone...")
                Telefone.SpeicherTelefone(C_XML, C_DP.XMLDoc)
            End If

            PushStatus("Das Einlesen der Telefone und Telefonnummern aus der Fritz!Box ist abgeschlossen.")
        End If
        ' Aufräumen
        tmpTelNr = Nothing
        C_JSON = Nothing
        TelQuery = Nothing
        FritzBoxJSONTelNr1 = Nothing
        FritzBoxJSONTelefone1 = Nothing
        FritzBoxJSONTelefone2 = Nothing
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

    Private Overloads Function AlleNummern(ByVal MSN() As String, ByVal SIP() As String, ByVal TAM() As String, ByVal FAX() As String, ByVal POTS As String, ByVal Mobil As String) As String
        AlleNummern = DataProvider.P_Def_LeerString
        Dim tmp() As String = Split(Join(MSN, ";") & ";" & Join(SIP, ";") & ";" & Join(TAM, ";") & ";" & Join(FAX, ";") & ";" & POTS & ";" & Mobil, ";", , CompareMethod.Text)

        tmp = C_hf.ClearStringArray(tmp, True, True, True)
        AlleNummern = Join(tmp, ";")
    End Function

    Private Overloads Function AlleNummern(ByVal MSN() As String, ByVal SIP() As String, ByVal TAM() As String, ByVal POTS As String, ByVal Mobil As String) As String
        Dim FAX As String() = Nothing
        Return AlleNummern(MSN, SIP, TAM, FAX, POTS, Mobil)
    End Function
#End Region

#Region "Wählen"
    Friend Function SendDialRequestToBox(ByVal sDialCode As String, ByVal sDialPort As String, ByVal bHangUp As Boolean) As String
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

    Private Function SendDialRequestToBoxV1(ByVal sDialCode As String, ByVal sDialPort As String, ByVal bHangUp As Boolean) As String
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
            Response = C_hf.httpPOST(P_Link_FB_ExtBasis, P_Link_FB_DialV1(P_SID, sDialPort, sDialCode, bHangUp), C_DP.P_EncodingFritzBox)

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

    Private Function SendDialRequestToBoxV2(ByVal sDialCode As String, ByVal sDialPort As String, ByVal bHangUp As Boolean) As String
        Dim Response As String = ""             ' Antwort der FritzBox
        Dim PortChangeSuccess As Boolean
        Dim DialCodetoBox As String
        ' Dim tempstring As String
        SendDialRequestToBoxV2 = DataProvider.P_FritzBox_Dial_Error1

        ' DialPort setzen, wenn erforderlich
        If FritzBoxQuery("DialPort=telcfg:settings/DialPort", "", False).Contains(sDialPort) Then
            PortChangeSuccess = True
        Else
            C_hf.LogFile("SendDialRequestToBoxV2: Ändere Dialport auf " & sDialPort)
            ' per HTTP-POST Dialport ändern
            Response = C_hf.httpPOST(P_Link_FB_TelV2, P_Link_FB_DialV2SetDialPort(P_SID, sDialPort), C_DP.P_EncodingFritzBox)
            ' {"data":{"btn_apply":"twofactor","twofactor":"button,dtmf;3170"}}
            If Response.Contains("twofactor") Then
                C_hf.MsgBox("Die Zweifaktor-Authentifizierung der Fritz!Box ist aktiv. Diese Sicherheitsfunktion muss deaktiviert werden, damit das Wählen mit dem ausgewählten Telefon möglich ist." & DataProvider.P_Def_ZweiNeueZeilen & "In der Fritz!Box:" & DataProvider.P_Def_EineNeueZeile & "System / FRITZ!Box - Benutzer / Anmeldung im Heimnetz" & DataProvider.P_Def_EineNeueZeile & "Entfernen Sie den Haken 'Ausführung bestimmter Einstellungen und Funktionen zusätzlich bestätigen.'", MsgBoxStyle.Critical, "SendDialRequestToBoxV2")
                PortChangeSuccess = False
            Else
                ' Prüfen, ob es erfolgreich war
                PortChangeSuccess = FritzBoxQuery("DialPort=telcfg:settings/DialPort", "", False).Contains(sDialPort)
            End If
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
            Response = C_hf.httpGET(P_Link_FB_DialV2(P_SID, DialCodetoBox, bHangUp), C_DP.P_EncodingFritzBox, FBFehler)
            ' Die Rückgabe ist der JSON - Wert "dialing"
            ' Bei der Wahl von Telefonnummern ist es ein {"dialing": "0123456789#"}
            ' Bei der Wahl von Telefoncodes ist es ein {"dialing": "#96*0*"}
            ' Bei der Wahl Des Hangup ist es ein {"dialing": false} ohne die umschließenden Anführungszeichen" 
            If Response.Contains("""dialing""") And Response.Contains(C_hf.IIf(bHangUp, "false", sDialCode)) Then
                SendDialRequestToBoxV2 = C_hf.IIf(bHangUp, DataProvider.P_FritzBox_Dial_HangUp, DataProvider.P_FritzBox_Dial_Start(sDialCode))
            Else
                C_hf.LogFile("SendDialRequestToBoxV2: Response: " & Response.Replace(vbLf, ""))
            End If
        Else
            C_hf.LogFile("SendDialRequestToBoxV2: Response: " & Response.Replace(vbLf, ""))
        End If
    End Function

    Private Function SendDialRequestToBoxV3(ByVal sDialCode As String, ByVal sDialPort As String, ByVal bHangUp As Boolean) As String
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
        End With

        UPnPDialport = C_XML.Read(C_DP.XMLDoc, xPathTeile, DataProvider.P_Def_ErrorMinusOne_String)

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
        OutPutData = C_FBoxUPnP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialGetConfig")

        If OutPutData.Item("NewX_AVM-DE_PhoneName").ToString = UPnPDialport Then
            PortChangeSuccess = True
        Else
            C_hf.LogFile("SendDialRequestToBoxV3: Ändere Dialport auf " & UPnPDialport)
            InPutData.Add("NewX_AVM-DE_PhoneName", UPnPDialport)
            OutPutData = C_FBoxUPnP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialSetConfig", InPutData)
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
                OutPutData = C_FBoxUPnP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialHangup")  ' Alt X_AVM-DE_Hangup
            Else
                InPutData.Add("NewX_AVM-DE_PhoneNumber", DialCodetoBox)
                OutPutData = C_FBoxUPnP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_DialNumber", InPutData)
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

            ReturnString = C_hf.httpGET(P_Link_JI1(P_SID), C_DP.P_EncodingFritzBox, FBFehler)
            If Not FBFehler Then
                If Not InStr(ReturnString, "Luacgi not readable", CompareMethod.Text) = 0 Then
                    C_hf.httpGET(P_Link_JIAlt_Child1(P_SID), C_DP.P_EncodingFritzBox, FBFehler)
                    sLink = P_Link_JIAlt_Child2(P_SID)
                End If
                ReturnString = C_hf.httpGET(sLink, C_DP.P_EncodingFritzBox, FBFehler)
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

        Response = C_hf.httpGET(P_Link_Jason_Boxinfo, C_DP.P_EncodingFritzBox, FBFehler)
        ' To Do Fehler Abfangen
        If Not FBFehler Then
            ' Ab der Firmware an 4.82 gibt es die Fritz!BoxInformation an 

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
                tmpFBFW.FritzBoxTyp = .GetElementsByTagName("Name", "http://jason.avm.de/updatecheck/").Item(0).InnerText
                tmpFBFW.SetFirmware(Response)
            End With
        Else
            ' ältere Versionen bis 4.82 prüfen
            ' dauert deutlich länger, als die Jason BoxInfo
            Response = C_hf.httpGET(P_Link_FB_SystemStatus, C_DP.P_EncodingFritzBox, FBFehler)
            If Not FBFehler Then
                tmpFBFW.SetFirmware(Split(C_hf.StringEntnehmen(Response, "<body>", "</body>"), "-", , CompareMethod.Text))
            End If
        End If
        ThisFBFirmware = tmpFBFW
        Return FBFehler
    End Function
#End Region

#Region "Fritz!Box Telefonbuch"

    Sub UploadKontaktToFritzBox(ByVal Kontakt As Outlook.ContactItem, ByVal istVIP As Boolean, ByVal BookID As String)
        If C_DP.P_RBFBComUPnP Then
            UploadKontaktToFritzBoxV3(Kontakt, istVIP, BookID)
        Else

            If ThisFBFirmware.ISLargerOREqual("6.30") Then
                UploadKontaktToFritzBoxV2(Kontakt, istVIP)
            Else
                UploadKontaktToFritzBoxV1(Kontakt, istVIP)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Lädt ein einen einzelnen Kontakt in das aktuell geöffnete Telefonbuch der Fritz!Box hoch.
    ''' </summary>
    ''' <param name="Kontakt">Der Kontakt, der hichgeladen werden soll.</param>
    ''' <param name="istVIP">Angabe, ob der Kontakt ein VIP ist. Diese Information wird übernommen.</param>
    Sub UploadKontaktToFritzBoxV1(ByVal Kontakt As Outlook.ContactItem, ByVal istVIP As Boolean)

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
                ReturnValue = .httpPOST(P_Link_FB_FonBook_Entry, cmd, C_DP.P_EncodingFritzBox)
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
    ''' Lädt ein einen einzelnen Kontakt in das aktuell geöffnete Telefonbuch der Fritz!Box hoch.
    ''' </summary>
    ''' <param name="Kontakt">Der Kontakt, der hichgeladen werden soll.</param>
    ''' <param name="istVIP">Angabe, ob der Kontakt ein VIP ist. Diese Information wird übernommen.</param>
    Sub UploadKontaktToFritzBoxV2(ByVal Kontakt As Outlook.ContactItem, ByVal istVIP As Boolean)

        Dim EntryName As String
        Dim EmailNew1 As String

        Dim NumberNew(3) As String
        Dim NumberType(3) As String

        Dim FritzBoxJSONUploadKontakt As FritzBoxJSONUploadResult = Nothing
        Dim C_JSON As New JSON
        Dim cmd As String
        Dim ReturnValue As String

        NumberType(0) = "home"
        NumberType(1) = "mobile"
        NumberType(2) = "work"
        NumberType(3) = "fax_work"

        With Kontakt
            EntryName = Replace(.FullName, DataProvider.P_Def_Leerzeichen, "+")
            NumberNew(0) = C_hf.nurZiffern(.HomeTelephoneNumber)
            NumberNew(1) = C_hf.nurZiffern(.MobileTelephoneNumber)
            NumberNew(2) = C_hf.nurZiffern(.BusinessTelephoneNumber)
            NumberNew(3) = C_hf.nurZiffern(.BusinessFaxNumber)
            EmailNew1 = .Email1Address
        End With

        If P_SID = DataProvider.P_Def_SessionID Then FBLogin()

        If Not P_SID = DataProvider.P_Def_SessionID And Len(P_SID) = Len(DataProvider.P_Def_SessionID) Then
            cmd = "sid=" & P_SID & "&entryname=" & EntryName

            For i = LBound(NumberType) To UBound(NumberType) - 1
                If Not NumberNew(i) = DataProvider.P_Def_LeerString Then
                    cmd += "&numbertypenew" & i + 1 & "=" & NumberType(i) & "&numbernew" & i + 1 & "=" & NumberNew(i)
                End If
            Next

            If istVIP Then cmd += "&category=on"

            If Not EmailNew1 = DataProvider.P_Def_LeerString Then cmd += "&emailnew1=" & EmailNew1
            cmd += "&prionumber=none"
            cmd += "&bookid=0"
            cmd += "&validate=apply"

            'cmd += "&back_to_page=/fon_num/fonbook_list.lua"
            'cmd += "&vanity="
            'cmd += "&code="
            'cmd += "&idx="
            'cmd += "&uid="
            cmd += "&xhr=1"

            With C_hf
                ReturnValue = .httpPOST(P_Link_FB_FonBook_Entry, cmd, C_DP.P_EncodingFritzBox)
                FritzBoxJSONUploadKontakt = C_JSON.GetUploadResult(ReturnValue)

                If FritzBoxJSONUploadKontakt.ok And FritzBoxJSONUploadKontakt.result.ToLower = "ok" Then

                    ' xhr=1&
                    ' sid=b14a6a7b97c4ad41&
                    ' lang=de&
                    ' no_sidrenew=&
                    ' idx=&
                    ' uid=&
                    ' entryname=Tim+Roch&
                    ' numbertypenew1=home&
                    ' numbernew1=&
                    ' numbertypenew2=mobile&
                    ' numbernew2=&
                    ' numbertypenew3=work&
                    ' numbernew3=0123456789&
                    ' prionumber=none&
                    ' code=&
                    ' vanity=&
                    ' emailnew1=&
                    ' bookid=0&
                    ' back_to_page=/fon_num/fonbook_list.lua&
                    ' apply=&
                    ' oldpage=/fon_num/fonbook_entry.lua'

                    cmd = "xhr=1"
                    cmd += "&sid=" & P_SID
                    cmd += "&lang=de"
                    cmd += "&no_sidrenew="
                    cmd += "&idx="
                    cmd += "&uid="
                    cmd += "&entryname=" & EntryName
                    For i = LBound(NumberType) To UBound(NumberType) - 1
                        If Not NumberNew(i) = DataProvider.P_Def_LeerString Then
                            cmd += "&numbertypenew" & i + 1 & "=" & NumberType(i) & "&numbernew" & i + 1 & "=" & NumberNew(i)
                        End If
                    Next
                    cmd += "&prionumber=none"
                    cmd += "&code="
                    cmd += "&vanity="
                    If Not EmailNew1 = DataProvider.P_Def_LeerString Then cmd += "&emailnew1=" & EmailNew1
                    cmd += "&bookid=0"
                    cmd += "&back_to_page=/fon_num/fonbook_list.lua"
                    cmd += "&apply="
                    cmd += "&oldpage=/fon_num/fonbook_entry.lua"

                    ReturnValue = .httpPOST(P_Link_FB_Data, cmd, C_DP.P_EncodingFritzBox)

                    .LogFile(DataProvider.P_Kontakt_Hochgeladen(EntryName))
                    .MsgBox(DataProvider.P_Kontakt_Hochgeladen(EntryName), MsgBoxStyle.Information, "UploadKontaktToFritzBox")
                Else
                    .MsgBox(DataProvider.P_Fehler_Kontakt_Hochladen(EntryName), MsgBoxStyle.Exclamation, "UploadKontaktToFritzBox")
                End If
            End With

        Else
            C_hf.MsgBox(DataProvider.P_FritzBox_Dial_Error3(P_SID), MsgBoxStyle.Critical, "UploadKontaktToFritzBox")
        End If

        C_JSON = Nothing
    End Sub

    Sub UploadKontaktToFritzBoxV3(ByVal Kontakt As Outlook.ContactItem, ByVal istVIP As Boolean, ByVal PhoneBookID As String)
        ' SetPhonebookEntry
        ' Add new entries with “” as value for PhonebookEntryID.
        ' Change existing entries with a value used for PhonebookEntryID with GetPhonebookEntry.
        ' The variable PhonebookEntryData may contain a unique ID.

        ' in NewPhonebookID as PhonebookID
        ' in NewPhonebookEntryID as PhonebookEntryID
        ' in NewPhonebookEntryData as PhonebookEntryData
        Dim xPathTeile As New ArrayList

        Dim InPutData As New Hashtable
        Dim OutPutData As New Hashtable
        Dim tmpname As String
        Dim Telefonbuch As XmlDocument
        Dim tmpxmlNode As XmlNode
        ''Dim PhoneBookID As String
        Dim NewPhonebookEntryID As String = ""
        'Dim Liste() As String
        Dim NameVohanden As Boolean = False

        OutPutData = C_FBoxUPnP.Start(KnownSOAPFile.x_contactSCPD, "GetPhonebookList")
        If OutPutData.Contains("Error") Then
            C_hf.MsgBox(OutPutData("Error").ToString.Replace("CHR(60)", "<").Replace("CHR(62)", ">"), MsgBoxStyle.Exclamation, "UploadKontaktToFritzBox")
        Else
            ' Herunterladen des Telefonbuches
            InPutData.Add("NewPhonebookID", 0)
            OutPutData = C_FBoxUPnP.Start(KnownSOAPFile.x_contactSCPD, "GetPhonebook")

            Telefonbuch = DownloadAddressbook(PhoneBookID)
            If Telefonbuch IsNot Nothing Then

                tmpname = Kontakt.FullNameAndCompany.Replace("&", "&amp;").Replace(DataProvider.P_Def_EineNeueZeile, " / ")

                With Kontakt

                    ' Liste = { .AssistantTelephoneNumber, .BusinessTelephoneNumber, .Business2TelephoneNumber, .CallbackTelephoneNumber, .CarTelephoneNumber, .CompanyMainTelephoneNumber, .HomeTelephoneNumber, .Home2TelephoneNumber, .ISDNNumber, .MobileTelephoneNumber, .OtherTelephoneNumber, .PagerNumber, .PrimaryTelephoneNumber, .RadioTelephoneNumber, .BusinessFaxNumber, .HomeFaxNumber, .OtherFaxNumber, .TelexNumber, .TTYTDDTelephoneNumber}
                    ' Liste = C_hf.ClearStringArray(Liste, True, True, False)
                    ' For i = LBound(Liste) To UBound(Liste)
                    '     Liste(i) = C_hf.nurZiffern(Liste(i))
                    ' Next
                    With xPathTeile
                        .Add("phonebook")
                        .Add("contact")
                        .Add("[contains(//realName,""" & tmpname & """)]")
                        ' .Add("contact[contains(//realName,""" & tmpname & """) and (contains(//number, """ & String.Join(""") or contains(//number, """, Liste) & """))]")
                        .Add("uniqueid")
                    End With
                    tmpxmlNode = Telefonbuch.SelectSingleNode(C_XML.CreateXPath(Telefonbuch, xPathTeile))
                    If tmpxmlNode IsNot Nothing Then
                        Select Case C_hf.MsgBox("Der Kontakt """ & tmpname & """ ist bereits im Telefonbuch vorhanden." & DataProvider.P_Def_ZweiNeueZeilen & "Soll der Kontakt ersetzt werden?", MsgBoxStyle.YesNoCancel, "SOAP-KontaktUpload")
                            Case vbYes
                                NewPhonebookEntryID = tmpxmlNode.InnerText
                            Case vbNo
                                NewPhonebookEntryID = ""
                            Case vbCancel
                                NewPhonebookEntryID = "ABBRUCH"
                        End Select
                    End If
                End With
            End If

            If Not NewPhonebookEntryID = "ABBRUCH" Then
                ' Hochladen des Kontaktes
                InPutData.Clear()
                InPutData.Add("NewPhonebookID", PhoneBookID)
                InPutData.Add("NewPhonebookEntryID", NewPhonebookEntryID)
                InPutData.Add("NewPhonebookEntryData", GetXMLContactEntry(Kontakt, istVIP, NewPhonebookEntryID))

                OutPutData = C_FBoxUPnP.Start(KnownSOAPFile.x_contactSCPD, "SetPhonebookEntry", InPutData)
                With C_hf

                    If OutPutData.Contains("Error") Then
                        .MsgBox(DataProvider.P_Fehler_Kontakt_Hochladen(Kontakt.FullNameAndCompany) & DataProvider.P_Def_ZweiNeueZeilen & OutPutData("Error").ToString.Replace("CHR(60)", "<").Replace("CHR(62)", ">"), MsgBoxStyle.Exclamation, "UploadKontaktToFritzBox")
                    Else
                        .LogFile(DataProvider.P_Kontakt_Hochgeladen(Kontakt.FullNameAndCompany))
                        .MsgBox(DataProvider.P_Kontakt_Hochgeladen(Kontakt.FullNameAndCompany), MsgBoxStyle.Information, "UploadKontaktToFritzBoxV3")
                    End If
                End With
            End If
        End If
        Telefonbuch = Nothing
        tmpxmlNode = Nothing
        InPutData = Nothing
        OutPutData = Nothing
    End Sub

    ''' <summary>
    ''' Erstellt den XML Kontakt für das Fritz!Box Telefonbuch
    ''' </summary>
    ''' <param name="Kontakt"></param>
    ''' <param name="istVIP"></param>
    ''' <returns></returns>
    Private Function GetXMLContactEntry(ByVal Kontakt As Outlook.ContactItem, ByVal istVIP As Boolean, ByVal NewPhonebookEntryID As String) As String

        Dim Liste As String()
        Dim NumberType As String = "home"
        ' Dim TelNr As String

        Dim NodeNames As New ArrayList
        Dim NodeValues As New ArrayList
        Dim AttributeNames As New ArrayList
        Dim AttributeValues As New ArrayList

        Dim Eintrag As New XmlDocument()
        'Dim KnotenName As XmlNode
        Dim XMLKnoten As XmlNode


        Eintrag.AppendChild(Eintrag.CreateXmlDeclaration("1.0", "UTF-8", Nothing))

        With Kontakt
            ' Wurzelelement generieren mit erster Ebene generieren
            '#Region "Wurzelelement mit erster Ebene"

            NodeNames.Add("category")
            NodeValues.Add(C_hf.IIf(istVIP, "1", ""))

            NodeNames.Add("telephony")
            NodeValues.Add("")

            NodeNames.Add("services")
            NodeValues.Add("")

            'NodeNames.Add("setup")
            'NodeValues.Add("")

            NodeNames.Add("features")
            NodeValues.Add("")

            'NodeNames.Add("mod_time")
            'NodeValues.Add("")

            NodeNames.Add("uniqueid")
            NodeValues.Add(NewPhonebookEntryID)

            Eintrag.AppendChild(C_XML.CreateXMLNode(Eintrag, "contact", NodeNames, NodeValues, AttributeNames, AttributeValues))

            NodeNames.Clear()
            NodeValues.Clear()
            '#End Region

            '#Region "Name"
            ' Name des Kontaktes
            NodeNames.Add("realName")
            NodeValues.Add(.FullNameAndCompany.Replace("&", "&amp;").Replace(DataProvider.P_Def_EineNeueZeile, " / "))

            Eintrag.DocumentElement.InsertAfter(C_XML.CreateXMLNode(Eintrag, "person", NodeNames, NodeValues, AttributeNames, AttributeValues), Eintrag.DocumentElement.GetElementsByTagName("category")(0))

            NodeNames.Clear()
            NodeValues.Clear()
            '#End Region

            '#Region "Telefonnummern"
            ' Telefonnummern des Kontaktes
            XMLKnoten = Eintrag.DocumentElement.GetElementsByTagName("telephony")(0)
            Liste = {.AssistantTelephoneNumber, .BusinessTelephoneNumber, .Business2TelephoneNumber, .CallbackTelephoneNumber, .CarTelephoneNumber, .CompanyMainTelephoneNumber, .HomeTelephoneNumber, .Home2TelephoneNumber, .ISDNNumber, .MobileTelephoneNumber, .OtherTelephoneNumber, .PagerNumber, .PrimaryTelephoneNumber, .RadioTelephoneNumber, .BusinessFaxNumber, .HomeFaxNumber, .OtherFaxNumber, .TelexNumber, .TTYTDDTelephoneNumber}

            For Each TelNr As String In Liste
                If Not TelNr = DataProvider.P_Def_LeerString Then
                    Select Case TelNr
                        Case .CarTelephoneNumber, .HomeTelephoneNumber, .Home2TelephoneNumber, .ISDNNumber, .TTYTDDTelephoneNumber, .OtherTelephoneNumber
                            NumberType = "home"
                        Case .MobileTelephoneNumber, .PagerNumber, .RadioTelephoneNumber
                            NumberType = "mobile"
                        Case .AssistantTelephoneNumber, .BusinessTelephoneNumber, .Business2TelephoneNumber, .CallbackTelephoneNumber, .CompanyMainTelephoneNumber, .PrimaryTelephoneNumber
                            NumberType = "work"
                        Case .BusinessFaxNumber, .HomeFaxNumber, .OtherFaxNumber, .TelexNumber
                            NumberType = "fax_work"
                    End Select

                    AttributeNames.Add("type")
                    AttributeValues.Add(NumberType)

                    XMLKnoten.AppendChild(C_XML.CreateXMLNode(Eintrag, "number", NodeNames, NodeValues, AttributeNames, AttributeValues)).InnerText = C_hf.nurZiffern(TelNr)

                    AttributeNames.Clear()
                    AttributeValues.Clear()
                End If
            Next
            XMLKnoten.Attributes.Append(Eintrag.CreateAttribute("nid"))
            XMLKnoten.Attributes(0).Value = CStr(XMLKnoten.ChildNodes.Count - 1) ' Warum Minus 1 weiß ich nicht

            NodeNames.Clear()
            NodeValues.Clear()
            XMLKnoten = Nothing
            '#End Region

            '#Region "E-Mail"
            ' E-Mail des Kontaktes
            XMLKnoten = Eintrag.DocumentElement.GetElementsByTagName("services")(0)
            Liste = {.Email1Address, .Email2Address, .Email3Address}

            For Each EmailAddress As String In Liste
                If Not EmailAddress = DataProvider.P_Def_LeerString Then

                    AttributeNames.Add("classifier")

                    Select Case EmailAddress
                        Case .Email1Address
                            AttributeValues.Add("work")
                        Case .Email2Address
                            AttributeValues.Add("Private")
                        Case .Email3Address
                            AttributeValues.Add("other")
                    End Select

                    AttributeNames.Add("id")
                    AttributeValues.Add(XMLKnoten.ChildNodes.Count - 1)

                    XMLKnoten.AppendChild(C_XML.CreateXMLNode(Eintrag, "email", NodeNames, NodeValues, AttributeNames, AttributeValues)).InnerText = EmailAddress

                    AttributeNames.Clear()
                    AttributeValues.Clear()
                End If
            Next
            XMLKnoten.Attributes.Append(Eintrag.CreateAttribute("nid"))
            XMLKnoten.Attributes(0).Value = CStr(XMLKnoten.ChildNodes.Count - 1) ' Warum Minus 1 weiß ich nicht

            NodeNames.Clear()
            NodeValues.Clear()
            '#End Region

        End With

        GetXMLContactEntry = Eintrag.InnerXml

        XMLKnoten = Nothing
        NodeNames = Nothing
        NodeValues = Nothing
        AttributeNames = Nothing
        AttributeValues = Nothing
        Eintrag = Nothing
    End Function

    ''' <summary>
    ''' Lädt das gewünschte Telefonbuch von der Fritz!Box herunter.
    ''' </summary>
    ''' <param name="sPhonebookId">
    ''' ID des Telefonbuches: 
    ''' 0 = Haupttelefonbuch
    ''' 255 = Intern
    ''' 256 = Clip Info</param>
    ''' <returns>XML Telefonbuch</returns>
    Private Function DownloadAddressbook(ByVal sPhonebookId As String) As XmlDocument

        Dim InPutData As New Hashtable
        Dim OutPutData As New Hashtable

        Dim ReturnValue As String
        Dim XMLFBAddressbuch As XmlDocument
        DownloadAddressbook = Nothing
        InPutData.Add("NewPhonebookID", sPhonebookId)
        OutPutData = C_FBoxUPnP.Start(KnownSOAPFile.x_contactSCPD, "GetPhonebook", InPutData)

        If OutPutData.ContainsKey("NewPhonebookURL") Then
            With C_hf
                ReturnValue = .httpGET(OutPutData.Item("NewPhonebookURL").ToString, C_DP.P_EncodingFritzBox, FBFehler)

                If ReturnValue.StartsWith("<?xml ") Then
                    XMLFBAddressbuch = New XmlDocument()
                    Try
                        XMLFBAddressbuch.LoadXml(ReturnValue)
                    Catch ex As Exception
                        .LogFile(DataProvider.P_Fehler_Export_Addressbuch)
                    End Try
                    DownloadAddressbook = XMLFBAddressbuch
                End If
            End With
        End If

        InPutData = Nothing
        OutPutData = Nothing
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

            UploadAddressbook = C_hf.httpPOST(P_Link_FB_ExportAddressbook, cmd, C_DP.P_EncodingFritzBox).Contains("Das Telefonbuch der FRITZ!Box wurde wiederhergestellt.")

        Else
            C_hf.MsgBox(DataProvider.P_FritzBox_Dial_Error3(P_SID), MsgBoxStyle.Critical, "UploadAddressbook")
        End If
    End Function

    ' Link Telefonbuch hinzufügen
    ' http://192.168.180.1/fon_num/fonbook_edit.lua?sid=9f4d23c5f4dcefd2&uid=new&back_to_page=/fon_num/fonbook_list.lua

    ''' <summary>
    ''' Gibt eine Liste der verfügbaren Fritz!Box Telefonbücher zurück.
    ''' </summary>
    ''' <returns>List</returns>
    ''' <remarks>http://fritz.box/fon_num/fonbook_select.lua</remarks>
    Friend Function GetTelefonbuchListe() As String()


        If C_DP.P_RBFBComUPnP Then
            Dim InPutData As New Hashtable
            Dim OutPutData As New Hashtable
            OutPutData = C_FBoxUPnP.Start(KnownSOAPFile.x_contactSCPD, "GetPhonebookList")
            Dim PhonebookListe As String()
            PhonebookListe = Split(OutPutData("NewPhonebookList").ToString, ",", , CompareMethod.Text)

            For idx As Integer = LBound(PhonebookListe) To UBound(PhonebookListe)
                InPutData.Clear()
                InPutData.Add("NewPhonebookID", PhonebookListe(idx))
                OutPutData = C_FBoxUPnP.Start(KnownSOAPFile.x_contactSCPD, "GetPhonebook", InPutData)

                PhonebookListe(idx) += ";" & OutPutData("NewPhonebookName").ToString()
            Next
            GetTelefonbuchListe = PhonebookListe

        Else
            Dim ReturnTelefonbuchListe As String() = {"0'>Telefonbuch"}

            Dim sPage As String
            Dim tmp As String
            Dim Liste As String = DataProvider.P_Def_LeerString
            Dim pos As Integer = 1

            If P_SID = DataProvider.P_Def_SessionID Then FBLogin()
            If Not P_SID = DataProvider.P_Def_SessionID And Len(P_SID) = Len(DataProvider.P_Def_SessionID) Then
                sPage = Replace(C_hf.httpGET(P_Link_Telefonbuch_List(P_SID), C_DP.P_EncodingFritzBox, FBFehler), Chr(34), "'", , , CompareMethod.Text)
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
        End If


    End Function

#End Region

#Region "Fritz!Box Query"
    ''' <summary>
    ''' Führt eine Query-Abfrage an die Fritz!Box durch
    ''' </summary>
    ''' <param name="Abfrage">Die durchzuführende Abfrage</param>
    ''' <param name="InDateiSpeichern">Angabe, ob die Abfrage zu Debugzwecken gespeichert werden soll</param>
    ''' <returns></returns>
    Private Function FritzBoxQuery(ByVal Abfrage As String, ByVal QueryID As String, ByVal InDateiSpeichern As Boolean) As String
        FritzBoxQuery = DataProvider.P_Def_ErrorMinusOne_String

        If P_SID = DataProvider.P_Def_SessionID Then FBLogin()
        If Not P_SID = DataProvider.P_Def_SessionID And Len(P_SID) = Len(DataProvider.P_Def_SessionID) Then
            C_hf.LogFile(P_Link_Query(P_SID, Abfrage))
            FritzBoxQuery = C_hf.httpGET(P_Link_Query(P_SID, Abfrage), C_DP.P_EncodingFritzBox, FBFehler)
        End If

        If InDateiSpeichern Then
            Dim PfadTMPfile As String
            Dim tmpFilePath As String
            'Dim tmpFileBase As String
            With My.Computer.FileSystem
                PfadTMPfile = .GetTempFileName()
                tmpFilePath = .GetFileInfo(PfadTMPfile).DirectoryName
                'tmpFileBase = Split(.GetFileInfo(PfadTMPfile).Name, ".", , CompareMethod.Text)(0)

                '.RenameFile(PfadTMPfile, tmpFileBase & ".txt")
                .RenameFile(PfadTMPfile, QueryID & ".txt")
                PfadTMPfile = .GetFiles(tmpFilePath, FileIO.SearchOption.SearchTopLevelOnly, QueryID & ".txt")(0).ToString
                .WriteAllText(PfadTMPfile, DataProvider.P_FritzBox_Tel_DebugMsgAb605 & DataProvider.P_Def_EineNeueZeile & "Pfad zur Datei: " & PfadTMPfile & DataProvider.P_Def_ZweiNeueZeilen & P_Link_Query(P_SID, Abfrage) & DataProvider.P_Def_ZweiNeueZeilen & FritzBoxQuery, False)
                'Rückgabe des Dateipfades
                If C_DP.P_Debug_FBFile Is Nothing Then C_DP.P_Debug_FBFile = New List(Of String)

                C_DP.P_Debug_FBFile.Add(PfadTMPfile)
            End With
        End If
    End Function

#End Region

    Friend Sub PushStatus(ByVal Status As String)
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
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                tb.Dispose()
                ' TODO: Verwalteten Zustand löschen (verwaltete Objekte).
            End If
            BWSetDialPort.Dispose()
            ' TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() unten überschreiben.
            ' TODO: Große Felder auf NULL festlegen.
        End If
        disposedValue = True
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