Imports System.Xml
Imports System.Timers

Public Class DataProvider
    Private XMLDoc As XmlDocument
    Private WithEvents tSpeichern As Timer
#Region "Windows Const für Office 2003"
#If OVer = 11 Then
    Public Const ECM_FIRST As Long = &H1500
    Public Const EM_AUTOURLDETECT As Long = (WM_USER + 91)
    Public Const EM_CANPASTE As Long = (WM_USER + 50)  ' unique to rich edit control
    Public Const EM_CANREDO As Long = (WM_USER + 85)
    Public Const EM_CANUNDO As Long = &HC6
    Public Const EM_CHARFROMPOS As Long = &HD7
    Public Const EM_CONVPOSITION As Long = (WM_USER + 108)
    Public Const EM_DISPLAYBAND As Long = (WM_USER + 51)  ' unique to rich edit control
    Public Const EM_EXGETSEL As Long = (WM_USER + 52)  ' unique to rich edit control
    Public Const EM_EXLIMITTEXT As Long = (WM_USER + 53)   ' unique to rich edit control
    Public Const EM_EXLINEFROMCHAR As Long = (WM_USER + 54)  ' unique to rich edit control
    Public Const EM_EXSETSEL As Long = (WM_USER + 55)   ' unique to rich edit control
    Public Const EM_FINDTEXT As Long = (WM_USER + 56)   ' unique to rich edit control
    Public Const EM_FINDTEXTEX As Long = (WM_USER + 79)  ' unique to rich edit control
    Public Const EM_FINDTEXTEXW As Long = (WM_USER + 124)
    Public Const EM_FINDTEXTW As Long = (WM_USER + 123)
    Public Const EM_FINDWORDBREAK As Long = (WM_USER + 76)  ' unique to rich edit control
    Public Const EM_FMTLINES As Long = &HC8
    Public Const EM_FORMATRANGE As Long = (WM_USER + 57)  ' unique to rich edit control
    Public Const EM_GETAUTOURLDETECT As Long = (WM_USER + 92)
    Public Const EM_GETBIDIOPTIONS As Long = (WM_USER + 201)
    Public Const EM_GETCHARFORMAT As Long = (WM_USER + 58) ' unique to rich edit control
    Public Const EM_GETEDITSTYLE As Long = (WM_USER + 205)
    Public Const EM_GETEVENTMASK As Long = (WM_USER + 59) ' unique to rich edit control
    Public Const EM_GETFIRSTVISIBLELINE As Long = &HCE
    Public Const EM_GETHANDLE As Long = &HBD
    Public Const EM_GETIMECOLOR As Long = (WM_USER + 105)  ' unique to rich edit control
    Public Const EM_GETIMECOMPMODE As Long = (WM_USER + 122)
    Public Const EM_GETIMEMODEBIAS As Long = (WM_USER + 127)
    Public Const EM_GETIMEOPTIONS As Long = (WM_USER + 107)  ' unique to rich edit control
    Public Const EM_GETIMESTATUS As Long = &HD9
    Public Const EM_GETLANGOPTIONS As Long = (WM_USER + 121)
    Public Const EM_GETLINE As Long = &HC4
    Public Const EM_GETLINECOUNT As Long = &HBA
    Public Const EM_GETMARGINS As Long = &HD4
    Public Const EM_GETMODIFY As Long = &HB8
    Public Const EM_GETOLEINTERFACE As Long = (WM_USER + 60) ' unique to rich edit control
    Public Const EM_GETOPTIONS As Long = (WM_USER + 78)  ' unique to rich edit control
    Public Const EM_GETPARAFORMAT As Long = (WM_USER + 61)  ' unique to rich edit control
    Public Const EM_GETPASSWORDCHAR As Long = &HD2
    Public Const EM_GETPUNCTUATION As Long = (WM_USER + 101) ' unique to rich edit control
    Public Const EM_GETRECT As Long = &HB2
    Public Const EM_GETREDONAME As Long = (WM_USER + 87)
    Public Const EM_GETSCROLLPOS As Long = (WM_USER + 221)
    Public Const EM_GETSEL As Long = &HB0
    Public Const EM_GETSELTEXT As Long = (WM_USER + 62) ' unique to rich edit control
    Public Const EM_GETTEXTEX As Long = (WM_USER + 94)
    Public Const EM_GETTEXTLENGTHEX As Long = (WM_USER + 95)
    Public Const EM_GETTEXTMODE As Long = (WM_USER + 90)
    Public Const EM_GETTEXTRANGE As Long = (WM_USER + 75)  ' unique to rich edit control
    Public Const EM_GETTHUMB As Long = &HBE
    Public Const EM_GETTYPOGRAPHYOPTIONS As Long = (WM_USER + 203)
    Public Const EM_GETUNDONAME As Long = (WM_USER + 86)
    Public Const EM_GETWORDBREAKPROC As Long = &HD1
    Public Const EM_GETWORDBREAKPROCEX As Long = (WM_USER + 80) ' unique to rich edit control
    Public Const EM_GETWORDWRAPMODE As Long = (WM_USER + 103) ' unique to rich edit control
    Public Const EM_GETZOOM As Long = (WM_USER + 224)
    Public Const EM_HIDESELECTION As Long = (WM_USER + 63) ' unique to rich edit control
    Public Const EM_LIMITTEXT As Long = &HC5
    Public Const EM_LINEFROMCHAR As Long = &HC9
    Public Const EM_LINEINDEX As Long = &HBB
    Public Const EM_LINELENGTH As Long = &HC1
    Public Const EM_LINESCROLL As Long = &HB6
    Public Const EM_OUTLINE As Long = (WM_USER + 220)
    Public Const EM_PASTESPECIAL As Long = (WM_USER + 64)  ' unique to rich edit control
    Public Const EM_POSFROMCHAR As Long = (WM_USER + 38)
    Public Const EM_RECONVERSION As Long = (WM_USER + 125)
    Public Const EM_REDO As Long = (WM_USER + 84)
    Public Const EM_REPLACESEL As Long = &HC2
    Public Const EM_REQUESTRESIZE As Long = (WM_USER + 65)  ' unique to rich edit control
    Public Const EM_SCROLL As Long = &HB5
    Public Const EM_SCROLLCARET As Long = &HB7
    Public Const EM_SELECTIONTYPE As Long = (WM_USER + 66) ' unique to rich edit control
    Public Const EM_SETBIDIOPTIONS As Long = (WM_USER + 200)
    Public Const EM_SETBKGNDCOLOR As Long = (WM_USER + 67) ' unique to rich edit control
    Public Const EM_SETCHARFORMAT As Long = (WM_USER + 68) ' unique to rich edit control
    Public Const EM_SETCUEBANNER As Long = (ECM_FIRST + 1)
    Public Const EM_SETEDITSTYLE As Long = (WM_USER + 204)
    Public Const EM_SETEVENTMASK As Long = (WM_USER + 69) ' unique to rich edit control
    Public Const EM_SETFONTSIZE As Long = (WM_USER + 223)
    Public Const EM_SETHANDLE As Long = &HBC  ' unique to rich edit control
    Public Const EM_SETIMECOLOR As Long = (WM_USER + 104)
    Public Const EM_SETIMEMODEBIAS As Long = (WM_USER + 126)
    Public Const EM_SETIMEOPTIONS As Long = (WM_USER + 106) ' unique to rich edit control
    Public Const EM_SETIMESTATUS As Long = &HD8
    Public Const EM_SETLANGOPTIONS As Long = (WM_USER + 120)
    Public Const EM_SETLIMITTEXT As Long = EM_LIMITTEXT
    Public Const EM_SETMARGINS As Long = &HD3
    Public Const EM_SETMODIFY As Long = &HB9
    Public Const EM_SETOLECALLBACK As Long = (WM_USER + 70)
    Public Const EM_SETOLEINTERFACE As Long = (WM_USER - 70)  ' unique to rich edit control
    Public Const EM_SETOPTIONS As Long = (WM_USER + 77) ' unique to rich edit control
    Public Const EM_SETPALETTE As Long = (WM_USER + 93)
    Public Const EM_SETPARAFORMAT As Long = (WM_USER + 71)  ' unique to rich edit control
    Public Const EM_SETPASSWORDCHAR As Long = &HCC
    Public Const EM_SETPUNCTUATION As Long = (WM_USER + 100) ' unique to rich edit control
    Public Const EM_SETREADONLY As Long = &HCF
    Public Const EM_SETRECT As Long = &HB3
    Public Const EM_SETRECTNP As Long = &HB4
    Public Const EM_SETSCROLLPOS As Long = (WM_USER + 222)
    Public Const EM_SETSEL As Long = &HB1
    Public Const EM_SETTABSTOPS As Long = &HCB
    Public Const EM_SETTARGETDEVICE As Long = (WM_USER + 72) ' unique to rich edit control
    Public Const EM_SETTEXT As Long = &HC
    Public Const EM_SETTEXTEX As Long = (WM_USER + 97)
    Public Const EM_SETTEXTMODE As Long = (WM_USER + 89)
    Public Const EM_SETTYPOGRAPHYOPTIONS As Long = (WM_USER + 202)
    Public Const EM_SETUNDOLIMIT As Long = (WM_USER + 82)
    Public Const EM_SETWORDBREAKPROC As Long = &HD0
    Public Const EM_SETWORDBREAKPROCEX As Long = (WM_USER + 81) ' unique to rich edit control
    Public Const EM_SETWORDWRAPMODE As Long = (WM_USER + 102)  ' unique to rich edit control
    Public Const EM_SETZOOM As Long = (WM_USER + 225)
    Public Const EM_SHOWSCROLLBAR As Long = (WM_USER + 96)
    Public Const EM_STOPGROUPTYPING As Long = (WM_USER + 88)
    Public Const EM_STREAMIN As Long = (WM_USER + 73)  ' unique to rich edit control
    Public Const EM_STREAMOUT As Long = (WM_USER + 74) ' unique to rich edit control
    Public Const EM_UNDO As Long = &HC7
    Public Const WM_CONTEXTMENU As Long = &H7B ' unique to rich edit control
    Public Const WM_USER As Long = &H400
#End If
#End Region
#Region "Konstanten"
    Private Const Speicherintervall As Double = 5 'in Minuten
    Private Const RootName As String = "FritzOutlookXML"
    Private Const xPathSeperatorSlash As String = "/"
    Private Const xPathWildCard As String = "*"
    Private Const xPathBracketOpen As String = "["
    Private Const xPathBracketClose As String = "]"
#End Region
#Region "PrivateData"

    Private _CBForceFBAddr As Boolean
    Private _CBAnrMonContactImage As Boolean
    Private _CBIndexAus As Boolean
    Private _CBShowMSN As Boolean
    Private _CBAnrMonMove As Boolean
    Private _CBAnrMonTransp As Boolean
    Private _CBAnrMonAuto As Boolean
    Private _CBAutoClose As Boolean
    Private _CBVoIPBuster As Boolean
    Private _CBCbCunterbinden As Boolean
    Private _CBCallByCall As Boolean
    Private _CBDialPort As Boolean
    Private _CBKErstellen As Boolean
    Private _CBLogFile As Boolean
#If OVer < 14 Then
    Private _CBSymbWwdh As Boolean
    Private _CBSymbAnrMon As Boolean
    Private _CBSymbAnrMonNeuStart As Boolean
    Private _CBSymbAnrListe As Boolean
    Private _CBSymbDirekt As Boolean
    Private _CBSymbRWSuche As Boolean
    Private _CBSymbVIP As Boolean
    Private _CBSymbJournalimport As Boolean
#End If
    Private _CBJImport As Boolean
    Private _CBRWS As Boolean
    Private _CBKHO As Boolean
    Private _CBRWSIndex As Boolean
    Private _ComboBoxRWS As Integer
    Private _TVKontaktOrdnerEntryID As String
    Private _TVKontaktOrdnerStoreID As String
    Private _CBIndex As Boolean
    Private _TBLandesVW As String
    Private _TBAmt As String
    Private _TBFBAdr As String
    Private _TBBenutzer As String
    Private _TBPasswort As String
    Private _TBVorwahl As String
    'Anrufmonitor
    Private _TBEnblDauer As Integer
    Private _TBAnrMonX As Integer
    Private _TBAnrMonY As Integer
    Private _CBoxAnrMonStartPosition As Integer
    Private _CBoxAnrMonMoveDirection As Integer
    Private _TBAnrMonMoveGeschwindigkeit As Integer
    Private _CBAnrMonZeigeKontakt As Boolean
    Private _CBJournal As Boolean
    Private _CBUseAnrMon As Boolean
    Private _CBCheckMobil As Boolean
    'StoppUhr
    Private _CBStoppUhrEinblenden As Boolean
    Private _CBStoppUhrAusblenden As Boolean
    Private _TBStoppUhr As Integer
    Private _CBStoppUhrX As Integer
    Private _CBStoppUhrY As Integer
    ' Telefonnummernformatierung
    Private _TBTelNrMaske As String
    Private _CBTelNrGruppieren As Boolean
    Private _CBintl As Boolean
    Private _CBIgnoTelNrFormat As Boolean
    ' Phoner
    Private _CBPhoner As Boolean
    Private _PhonerVerfügbar As Boolean
    Private _CBPhonerAnrMon As Boolean
    Private _ComboBoxPhonerSIP As Integer
    Private _TBPhonerPasswort As String
    Private _PhonerTelNameIndex As Integer
    ' Statistik
    Private _StatResetZeit As Date
    Private _StatVerpasst As Integer
    Private _StatNichtErfolgreich As Integer
    Private _StatKontakt As Integer
    Private _StatJournal As Integer
    Private _StatOLClosedZeit As Date
    ' Wählbox
    Private _TelAnschluss As Integer
    Private _TelFestnetz As Boolean
    Private _TelCLIR As Boolean
    ' FritzBox
    Private _EncodeingFritzBox As String
    ' Indizierung
    Private _LLetzteIndizierung As Date
    ' Notiz
    Private _CBNote As Boolean
    'Einstellungen
    Private _Arbeitsverzeichnis As String
#End Region
#Region "Value Properties"
    ''' <summary>
    ''' Gibt die im Einstellungsdialog eingegebene Landesvorwahl zurück
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>Landesvorwahl</returns>
    ''' <remarks></remarks>
    Public Property P_TBLandesVW() As String
        Get
            Return _TBLandesVW
        End Get
        Set(ByVal value As String)
            _TBLandesVW = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, ob eine Amtsholung stets mitgewählt werden soll. Die Amtsholung wird in den Einstellungen festgelegt.
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>Zahl für die Amtsholung</returns>
    ''' <remarks></remarks>
    Public Property P_TBAmt() As String
        Get
            Return _TBAmt
        End Get
        Set(ByVal value As String)
            _TBAmt = value
        End Set
    End Property
    ''' <summary>
    ''' Eigenschaft für die hinterlege Ortsvorwahl
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>Ortsvorwahl</returns>
    ''' <remarks></remarks>
    Public Property P_TBVorwahl() As String
        Get
            Return _TBVorwahl
        End Get
        Set(ByVal value As String)
            _TBVorwahl = value
        End Set
    End Property
    ' Anrufmonitor
    ''' <summary>
    ''' Gibt an, wie lange der Anrufmonitor angezeigt werden soll, bevor er automatisch ausgeblendet wird
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Intervall</returns>
    ''' <remarks></remarks>
    Public Property P_TBEnblDauer() As Integer
        Get
            Return _TBEnblDauer
        End Get
        Set(ByVal value As Integer)
            _TBEnblDauer = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, ob der Anrufmonitor automatisch gestartét werden soll.
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns>Autostart</returns>
    ''' <remarks></remarks>
    Public Property P_CBAnrMonAuto() As Boolean
        Get
            Return _CBAnrMonAuto
        End Get
        Set(ByVal value As Boolean)
            _CBAnrMonAuto = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, um wieviele Punkte der Anrufmonitor in X-Richtung verschoben werden soll.
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Positionskorrektur X</returns>
    ''' <remarks></remarks>
    Public Property P_TBAnrMonX() As Integer
        Get
            Return _TBAnrMonX
        End Get
        Set(ByVal value As Integer)
            _TBAnrMonX = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, um wieviele Punkte der Anrufmonitor in Y-Richtung verschoben werden soll.
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Positionskorrektur Y</returns>
    ''' <remarks></remarks>
    Public Property P_TBAnrMonY() As Integer
        Get
            Return _TBAnrMonY
        End Get
        Set(ByVal value As Integer)
            _TBAnrMonY = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an ob der Anrufmonitor in den Bildschirm hereingescrollt werden soll.
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns>Anrufmonitorbewegung</returns>
    ''' <remarks></remarks>
    Public Property P_CBAnrMonMove() As Boolean
        Get
            Return _CBAnrMonMove
        End Get
        Set(ByVal value As Boolean)
            _CBAnrMonMove = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, ob der Anrufmonitor eingeblendet werden soll.
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property P_CBAnrMonTransp() As Boolean
        Get
            Return _CBAnrMonTransp
        End Get
        Set(ByVal value As Boolean)
            _CBAnrMonTransp = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt die Endposition des Anrufmonitors an.
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Wert für die Position</returns>
    ''' <remarks>FritzBoxDial.PopUpAnrMon.eStartPosition</remarks>
    Public Property P_CBoxAnrMonStartPosition() As Integer
        Get
            Return _CBoxAnrMonStartPosition
        End Get
        Set(ByVal value As Integer)
            _CBoxAnrMonStartPosition = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt die Bewegungsrichtung des Anrufmonitors an.
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Wert für Richtung, abhängig von der Endosition.</returns>
    ''' <remarks>FritzBoxDial.PopUpAnrMon.eMoveDirection</remarks>
    Public Property P_CBoxAnrMonMoveDirection() As Integer
        Get
            Return _CBoxAnrMonMoveDirection
        End Get
        Set(ByVal value As Integer)
            _CBoxAnrMonMoveDirection = value
        End Set
    End Property
    Public Property P_TBAnrMonMoveGeschwindigkeit() As Integer
        Get
            Return _TBAnrMonMoveGeschwindigkeit
        End Get
        Set(ByVal value As Integer)
            _TBAnrMonMoveGeschwindigkeit = value
        End Set
    End Property

    Public Property P_CBAnrMonZeigeKontakt() As Boolean
        Get
            Return _CBAnrMonZeigeKontakt
        End Get
        Set(ByVal value As Boolean)
            _CBAnrMonZeigeKontakt = value
        End Set
    End Property
    Public Property P_CBAnrMonContactImage() As Boolean
        Get
            Return _CBAnrMonContactImage
        End Get
        Set(ByVal value As Boolean)
            _CBAnrMonContactImage = value
        End Set
    End Property
    Public Property P_CBIndexAus() As Boolean
        Get
            Return _CBIndexAus
        End Get
        Set(ByVal value As Boolean)
            _CBIndexAus = value
        End Set
    End Property
    Public Property P_CBShowMSN() As Boolean
        Get
            Return _CBShowMSN
        End Get
        Set(ByVal value As Boolean)
            _CBShowMSN = value
        End Set
    End Property

    Public Property P_CBAutoClose() As Boolean
        Get
            Return _CBAutoClose
        End Get
        Set(ByVal value As Boolean)
            _CBAutoClose = value
        End Set
    End Property
    Public Property P_CBVoIPBuster() As Boolean
        Get
            Return _CBVoIPBuster
        End Get
        Set(ByVal value As Boolean)
            _CBVoIPBuster = value
        End Set
    End Property
    Public Property P_CBCbCunterbinden() As Boolean
        Get
            Return _CBCbCunterbinden
        End Get
        Set(ByVal value As Boolean)
            _CBCbCunterbinden = value
        End Set
    End Property
    Public Property P_CBCallByCall() As Boolean
        Get
            Return _CBCallByCall
        End Get
        Set(ByVal value As Boolean)
            _CBCallByCall = value
        End Set
    End Property
    Public Property P_CBDialPort() As Boolean
        Get
            Return _CBDialPort
        End Get
        Set(ByVal value As Boolean)
            _CBDialPort = value
        End Set
    End Property
    Public Property P_CBLogFile() As Boolean
        Get
            Return _CBLogFile
        End Get
        Set(ByVal value As Boolean)
            _CBLogFile = value
        End Set
    End Property
    Public Property P_CBSymbWwdh() As Boolean
        Get
            Return _CBSymbWwdh
        End Get
        Set(ByVal value As Boolean)
            _CBSymbWwdh = value
        End Set
    End Property
    Public Property P_CBSymbAnrMon() As Boolean
        Get
            Return _CBLogFile
        End Get
        Set(ByVal value As Boolean)
            _CBLogFile = value
        End Set
    End Property
    Public Property P_CBSymbAnrMonNeuStart() As Boolean
        Get
            Return _CBSymbAnrMonNeuStart
        End Get
        Set(ByVal value As Boolean)
            _CBSymbAnrMonNeuStart = value
        End Set
    End Property
    'Pffice 2003 und Office 2007
    Public Property P_CBSymbAnrListe() As Boolean
        Get
            Return _CBSymbAnrListe
        End Get
        Set(ByVal value As Boolean)
            _CBSymbAnrListe = value
        End Set
    End Property
    Public Property P_CBSymbDirekt() As Boolean
        Get
            Return _CBSymbDirekt
        End Get
        Set(ByVal value As Boolean)
            _CBSymbDirekt = value
        End Set
    End Property
    Public Property P_CBSymbRWSuche() As Boolean
        Get
            Return _CBSymbRWSuche
        End Get
        Set(ByVal value As Boolean)
            _CBSymbRWSuche = value
        End Set
    End Property
    Public Property P_CBSymbVIP() As Boolean
        Get
            Return _CBSymbVIP
        End Get
        Set(ByVal value As Boolean)
            _CBSymbVIP = value
        End Set
    End Property
    Public Property P_CBSymbJournalimport() As Boolean
        Get
            Return _CBSymbJournalimport
        End Get
        Set(ByVal value As Boolean)
            _CBSymbJournalimport = value
        End Set
    End Property
    Public Property P_CBJImport() As Boolean
        Get
            Return _CBJImport
        End Get
        Set(ByVal value As Boolean)
            _CBJImport = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an ob nur der Hauptkontaktordner durchsucht werden muss oder alle möglichen eingebundenen Kontaktordner
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns>True, wenn nur der Hauptkontaktordner durchsucht werden muss</returns>
    ''' <remarks></remarks>
    Public Property P_CBKHO() As Boolean
        Get
            Return _CBKHO
        End Get
        Set(ByVal value As Boolean)
            _CBKHO = value
        End Set
    End Property
    Public Property P_CBRWS() As Boolean
        Get
            Return _CBRWS
        End Get
        Set(ByVal value As Boolean)
            _CBRWS = value
        End Set
    End Property
    Public Property P_CBKErstellen() As Boolean
        Get
            Return _CBKErstellen
        End Get
        Set(ByVal value As Boolean)
            _CBKErstellen = value
        End Set
    End Property
    Public Property P_CBRWSIndex() As Boolean
        Get
            Return _CBRWSIndex
        End Get
        Set(ByVal value As Boolean)
            _CBRWSIndex = value
        End Set
    End Property
    Public Property P_ComboBoxRWS() As Integer
        Get
            Return _ComboBoxRWS
        End Get
        Set(ByVal value As Integer)
            _ComboBoxRWS = value
        End Set
    End Property
    Public Property P_TVKontaktOrdnerEntryID() As String
        Get
            Return _TVKontaktOrdnerEntryID
        End Get
        Set(ByVal value As String)
            _TVKontaktOrdnerEntryID = value
        End Set
    End Property
    Public Property P_TVKontaktOrdnerStoreID() As String
        Get
            Return _TVKontaktOrdnerStoreID
        End Get
        Set(ByVal value As String)
            _TVKontaktOrdnerStoreID = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, ob die Indizierung durchgeführt werden soll. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns>True/False</returns>
    ''' <remarks></remarks>
    Public Property P_CBIndex() As Boolean
        Get
            Return _CBIndex
        End Get
        Set(ByVal value As Boolean)
            _CBIndex = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, ob Journaleinträge erstellt werden soll. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns>True/False</returns>
    ''' <remarks></remarks>
    Public Property P_CBJournal() As Boolean
        Get
            Return _CBJournal
        End Get
        Set(ByVal value As Boolean)
            _CBJournal = value
        End Set
    End Property
    Public Property P_CBUseAnrMon() As Boolean
        Get
            Return _CBUseAnrMon
        End Get
        Set(ByVal value As Boolean)
            _CBUseAnrMon = value
        End Set
    End Property
    Public Property P_CBCheckMobil() As Boolean
        Get
            Return _CBCheckMobil
        End Get
        Set(ByVal value As Boolean)
            _CBCheckMobil = value
        End Set
    End Property
    'StoppUhr
    Public Property P_CBStoppUhrEinblenden() As Boolean
        Get
            Return _CBStoppUhrEinblenden
        End Get
        Set(ByVal Value As Boolean)
            _CBStoppUhrEinblenden = Value
        End Set
    End Property
    Public Property P_CBStoppUhrAusblenden() As Boolean
        Get
            Return _CBStoppUhrAusblenden
        End Get
        Set(ByVal Value As Boolean)
            _CBStoppUhrAusblenden = Value
        End Set
    End Property
    Public Property P_TBStoppUhr() As Integer
        Get
            Return _TBStoppUhr
        End Get
        Set(ByVal value As Integer)
            _TBStoppUhr = value
        End Set
    End Property
    Public Property P_CBStoppUhrX() As Integer
        Get
            Return _CBStoppUhrX
        End Get
        Set(ByVal value As Integer)
            _CBStoppUhrX = value
        End Set
    End Property
    Public Property P_CBStoppUhrY() As Integer
        Get
            Return _CBStoppUhrY
        End Get
        Set(ByVal value As Integer)
            _CBStoppUhrY = value
        End Set
    End Property
    ' Telefonnummernformatierung
    Public Property P_TBTelNrMaske() As String
        Get
            Return _TBTelNrMaske
        End Get
        Set(ByVal value As String)
            _TBTelNrMaske = value
        End Set
    End Property
    Public Property P_CBTelNrGruppieren() As Boolean
        Get
            Return _CBTelNrGruppieren
        End Get
        Set(ByVal value As Boolean)
            _CBTelNrGruppieren = value
        End Set
    End Property
    Public Property P_CBintl() As Boolean
        Get
            Return _CBintl
        End Get
        Set(ByVal value As Boolean)
            _CBintl = value
        End Set
    End Property
    Public Property P_CBIgnoTelNrFormat() As Boolean
        Get
            Return _CBIgnoTelNrFormat
        End Get
        Set(ByVal value As Boolean)
            _CBIgnoTelNrFormat = value
        End Set
    End Property
    'Phoner
    Public Property P_CBPhoner As Boolean
        Get
            Return _CBPhoner
        End Get
        Set(ByVal value As Boolean)
            _CBPhoner = value
        End Set
    End Property
    Public Property P_PhonerVerfügbar As Boolean
        Get
            Return _PhonerVerfügbar
        End Get
        Set(ByVal value As Boolean)
            _PhonerVerfügbar = value
        End Set
    End Property
    Public Property P_CBPhonerAnrMon As Boolean
        Get
            Return _CBPhonerAnrMon
        End Get
        Set(ByVal value As Boolean)
            _CBPhonerAnrMon = value
        End Set
    End Property
    Public Property P_ComboBoxPhonerSIP() As Integer
        Get
            Return _ComboBoxPhonerSIP
        End Get
        Set(ByVal value As Integer)
            _ComboBoxPhonerSIP = value
        End Set
    End Property
    Public Property P_TBPhonerPasswort() As String
        Get
            Return _TBPhonerPasswort
        End Get
        Set(ByVal value As String)
            _TBPhonerPasswort = value
        End Set
    End Property
    Public Property P_PhonerTelNameIndex() As Integer
        Get
            Return _PhonerTelNameIndex
        End Get
        Set(ByVal value As Integer)
            _PhonerTelNameIndex = value
        End Set
    End Property
    ' Statistik
    Public Property P_StatResetZeit As Date
        Get
            Return _StatResetZeit
        End Get
        Set(ByVal value As Date)
            _StatResetZeit = value
        End Set
    End Property
    Public Property P_StatVerpasst As Integer
        Get
            Return _StatVerpasst
        End Get
        Set(ByVal value As Integer)
            _StatVerpasst = value
        End Set
    End Property
    Public Property P_StatNichtErfolgreich As Integer
        Get
            Return _StatNichtErfolgreich
        End Get
        Set(ByVal value As Integer)
            _StatNichtErfolgreich = value
        End Set
    End Property
    Public Property P_StatJournal() As Integer
        Get
            Return _StatJournal
        End Get
        Set(ByVal value As Integer)
            _StatJournal = value
        End Set
    End Property
    Public Property P_StatKontakt() As Integer
        Get
            Return _StatKontakt
        End Get
        Set(ByVal value As Integer)
            _StatKontakt = value
        End Set
    End Property
    Public Property P_StatOLClosedZeit() As Date
        Get
            Return _StatOLClosedZeit
        End Get
        Set(ByVal value As Date)
            _StatOLClosedZeit = value
        End Set
    End Property
    ' Wählbox
    Public Property P_TelAnschluss() As Integer
        Get
            Return _TelAnschluss
        End Get
        Set(ByVal value As Integer)
            _TelAnschluss = value
        End Set
    End Property
    Public Property P_TelFestnetz() As Boolean
        Get
            Return _TelFestnetz
        End Get
        Set(ByVal value As Boolean)
            _TelFestnetz = value
        End Set
    End Property
    Public Property P_TelCLIR() As Boolean
        Get
            Return _TelCLIR
        End Get
        Set(ByVal value As Boolean)
            _TelCLIR = value
        End Set
    End Property
    ' FritzBox
    ''' <summary>
    ''' Gibt die ermittelte Zeichencodierung der Fritzbox wieder. Der Wert wird automatisch ermittelt. 
    ''' </summary>
    ''' <value>String</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property P_EncodeingFritzBox() As String
        Get
            Return _EncodeingFritzBox
        End Get
        Set(ByVal value As String)
            _EncodeingFritzBox = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt die eingegebene Fritz!Box IP-Adresse an. Dies ist eine Angabe, die der Nutzer in den Einstellungen ändern kann.
    ''' </summary>
    ''' <value>String</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property P_TBFBAdr() As String
        Get
            Return _TBFBAdr
        End Get
        Set(ByVal value As String)
            _TBFBAdr = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, ob eine Verbindung zur Fritz!Box trotz fehlgeschlagenen Pings aufgebaut werden soll.
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property P_CBForceFBAddr() As Boolean
        Get
            Return _CBForceFBAddr
        End Get
        Set(ByVal value As Boolean)
            _CBForceFBAddr = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt den einegegebenen Benutzernamen für das Login der Fritz!Box an. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    ''' <value>String</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property P_TBBenutzer() As String
        Get
            Return _TBBenutzer
        End Get
        Set(ByVal value As String)
            _TBBenutzer = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt das eingegebene Passwort für das Login der Fritz!Box an. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>Das verschlüsselte Passwort</returns>
    ''' <remarks></remarks>
    Public Property P_TBPasswort() As String
        Get
            Return _TBPasswort
        End Get
        Set(ByVal value As String)
            _TBPasswort = value
        End Set
    End Property
    ' Indizierung
    Public Property P_LLetzteIndizierung() As Date
        Get
            Return _LLetzteIndizierung
        End Get
        Set(ByVal value As Date)
            _LLetzteIndizierung = value
        End Set
    End Property
    ' Note
    Public Property P_CBNote() As Boolean
        Get
            Return _CBNote
        End Get
        Set(ByVal value As Boolean)
            _CBNote = value
        End Set
    End Property
    ' Einstellungen
    Public Property P_Arbeitsverzeichnis() As String
        Get
            Return _Arbeitsverzeichnis
        End Get
        Set(value As String)
            _Arbeitsverzeichnis = value
        End Set
    End Property
#End Region
#Region "Global Default Value Properties"
    ''' <summary>
    ''' Default Fehlerwert
    ''' </summary>
    ''' <value>-1</value>
    ''' <returns>String</returns>
    Public ReadOnly Property P_Def_ErrorMinusOne() As String
        Get
            Return "-1"
        End Get
    End Property

    Public ReadOnly Property P_Def_ErrorMinusTwo() As String
        Get
            Return "-2"
        End Get
    End Property

    Public ReadOnly Property P_Def_StringEmpty() As String
        Get
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property P_Def_NeueZeile() As String
        Get
            Return vbCrLf
        End Get
    End Property

    Public ReadOnly Property P_Def_StringUnknown() As String
        Get
            Return "unbekannt"
        End Get
    End Property

    Public ReadOnly Property P_Def_FritzBoxAdress() As String
        Get
            Return "fritz.box"
        End Get
    End Property

    Public ReadOnly Property P_DefaultFBAnrMonPort() As Integer
        Get
            Return 1012
        End Get
    End Property

    Public ReadOnly Property P_DefaultPhonerAnrMonPort() As Integer
        Get
            Return 2012
        End Get
    End Property

    Public ReadOnly Property P_Def_StringNull() As String
        Get
            Return "0"
        End Get
    End Property

    Public ReadOnly Property P_Def_StringErrorMinusOne() As String
        Get
            Return CStr(P_Def_ErrorMinusOne)
        End Get
    End Property

    Public ReadOnly Property P_Def_SessionID() As String
        Get
            Return "0000000000000000"
        End Get
    End Property

    Public ReadOnly Property P_Def_Header_UserAgent() As String
        Get
            Return "Mozilla/5.0 (compatible; MSIE 7.0; Windows NT 6.0; WOW64; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.0.04506; Media Center PC 5.0; .NET CLR 3.5.21022; .NET CLR 1.1.4322)"
        End Get
    End Property

    Public ReadOnly Property P_Def_Header_ContentType() As String
        Get
            Return "application/x-www-form-urlencoded"
        End Get
    End Property

    Public ReadOnly Property P_Def_Header_Accept() As String
        Get
            Return "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
        End Get
    End Property

    Public ReadOnly Property P_Def_ReStartIntervall() As Integer
        Get
            Return 3000
        End Get
    End Property

    Public ReadOnly Property P_Def_TryMaxRestart() As Integer
        Get
            Return 15
        End Get
    End Property

    Public ReadOnly Property P_Def_AnrMonDirection_Call() As String
        Get
            Return "[->]"
        End Get
    End Property

    Public ReadOnly Property P_Def_AnrMonDirection_Ring() As String
        Get
            Return "[<-]"
        End Get
    End Property

    Public ReadOnly Property P_Def_AnrMonDirection_Default() As String
        Get
            Return "[<>]"
        End Get
    End Property

    Public ReadOnly Property P_Def_AnrMonDirection_UserProperty_Name() As String
        Get
            Return "FBDB-AnrMonDirection"
        End Get
    End Property

    Public ReadOnly Property P_Def_AnrMonDirection_UserProperty_Zeit() As String
        Get
            Return "FBDB-AnrMonZeit"
        End Get
    End Property

    Public ReadOnly Property P_Def_Note_Table() As String
        Get
            Return "FBDB_Note_Table"
        End Get
    End Property

    Public ReadOnly Property P_Def_Begin_vCard() As String
        Get
            Return "BEGIN:VCARD"
        End Get
    End Property

    Public ReadOnly Property P_Def_End_vCard() As String
        Get
            Return "END:VCARD"
        End Get
    End Property

    Public ReadOnly Property P_Def_NameListCALL() As String
        Get
            Return "CallList"
        End Get
    End Property

    Public ReadOnly Property P_Def_NameListRING() As String
        Get
            Return "RingList"
        End Get
    End Property

    Public ReadOnly Property P_Def_NameListVIP() As String
        Get
            Return "VIPList"
        End Get
    End Property

    Public ReadOnly Property P_Def_Addin_LangName() As String
        Get
            Return "Fritz!Box Telefon-dingsbums"
        End Get
    End Property

    Public ReadOnly Property P_Def_Addin_KurzName() As String
        Get
            Return "FritzOutlook"
        End Get
    End Property

    Public ReadOnly Property P_Def_Config_FileName() As String
        Get
            Return P_Def_Addin_KurzName & ".xml"
        End Get
    End Property

    Public ReadOnly Property P_Def_Log_FileName() As String
        Get
            Return P_Def_Addin_KurzName & ".log"
        End Get
    End Property
    ''' <summary>
    ''' Gibt den Zeitraum in MINUTEN an, nachdem geprüft werden soll, ob der Anrufmonitor noch aktiv ist. 
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Intervall in MINUTEN</returns>
    Public ReadOnly Property P_Def_CheckAnrMonIntervall() As Integer
        Get
            Return 1
        End Get
    End Property

    'Private ReadOnly Property P_Def_Arbeitsverzeichnis As String
    '    Get
    '        Return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & P_Def_Addin_LangName & "\"
    '    End Get
    'End Property
#End Region
#Region "Default Value Properties"

    Public ReadOnly Property P_Def_TBLandesVW() As String
        Get
            Return "0049"
        End Get
    End Property
    Public ReadOnly Property P_Def_TBAmt() As String
        Get
            Return P_Def_ErrorMinusOne
        End Get
    End Property
    Public ReadOnly Property P_Def_TBVorwahl() As String
        Get
            Return P_Def_StringEmpty
        End Get
    End Property
    Public ReadOnly Property P_Def_TBEnblDauer() As Integer
        Get
            Return 10
        End Get
    End Property
    Public ReadOnly Property P_Def_CBAnrMonAuto() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_TBAnrMonX() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property P_Def_TBAnrMonY() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property P_Def_CBAnrMonMove() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_CBAnrMonTransp() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_TBAnrMonMoveGeschwindigkeit() As Integer
        Get
            Return 5
        End Get
    End Property
    Public ReadOnly Property P_Def_CBoxAnrMonStartPosition() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property P_Def_CBoxAnrMonMoveDirection() As Integer
        Get
            Return 0
        End Get
    End Property

    Public ReadOnly Property P_Def_CBAnrMonZeigeKontakt() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_CBAnrMonContactImage() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_CBIndexAus() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_CBShowMSN() As Boolean
        Get
            Return False
        End Get
    End Property

    Public ReadOnly Property P_Def_CBAutoClose() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_CBVoIPBuster() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_CBCbCunterbinden() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_CBCallByCall() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_CBDialPort() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_CBKErstellen() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_CBLogFile() As Boolean
        Get
            Return True
        End Get
    End Property
    'Einstellung für die Symbolleiste
    Public ReadOnly Property P_Def_CBSymbWwdh() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_CBSymbAnrMon() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_CBSymbAnrMonNeuStart() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_CBSymbAnrListe() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_CBSymbDirekt() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_CBSymbRWSuche() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_CBSymbVIP() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_CBSymbJournalimport() As Boolean
        Get
            Return False
        End Get
    End Property

    Public ReadOnly Property P_Def_CBJImport() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_CBRWS() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_TVKontaktOrdnerEntryID() As String
        Get
            Return P_Def_StringErrorMinusOne
        End Get
    End Property
    Public ReadOnly Property P_Def_TVKontaktOrdnerStoreID() As String
        Get
            Return P_Def_StringErrorMinusOne
        End Get
    End Property
    Public ReadOnly Property P_Def_CBKHO() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_CBRWSIndex() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_ComboBoxRWS() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property P_Def_CBIndex() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_CBJournal() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_CBUseAnrMon() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_CBCheckMobil() As Boolean
        Get
            Return True
        End Get
    End Property
    'StoppUhr
    Public ReadOnly Property P_Def_CBStoppUhrEinblenden() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_CBStoppUhrAusblenden() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_TBStoppUhr() As Integer
        Get
            Return 10
        End Get

    End Property
    Public ReadOnly Property P_Def_CBStoppUhrX() As Integer
        Get
            Return 10
        End Get

    End Property
    Public ReadOnly Property P_Def_CBStoppUhrY() As Integer
        Get
            Return 10
        End Get

    End Property
    ' Telefonnummernformatierung
    Public ReadOnly Property P_Def_TBTelNrMaske() As String
        Get
            Return "%L (%O) %N - %D"
        End Get
    End Property
    Public ReadOnly Property P_Def_CBTelNrGruppieren() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property P_Def_CBintl() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_CBIgnoTelNrFormat() As Boolean
        Get
            Return False
        End Get
    End Property
    'Phoner
    Public ReadOnly Property P_Def_CBPhoner As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_PhonerVerfügbar As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_CBPhonerAnrMon As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_ComboBoxPhonerSIP() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property P_Def_TBPhonerPasswort() As String
        Get
            Return P_Def_StringEmpty
        End Get
    End Property
    Public ReadOnly Property P_Def_PhonerTelNameIndex() As Integer
        Get
            Return 0
        End Get
    End Property
    ' Statistik
    Public ReadOnly Property P_Def_StatResetZeit As Date
        Get
            Return System.DateTime.Now
        End Get
    End Property
    Public ReadOnly Property P_Def_StatVerpasst As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property P_Def_StatNichtErfolgreich As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property P_Def_StatJournal() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property P_Def_StatKontakt() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property P_Def_StatOLClosedZeit() As Date
        Get
            Return System.DateTime.Now
        End Get
    End Property
    ' Wählbox
    Public ReadOnly Property P_Def_TelAnschluss() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property P_Def_TelFestnetz() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_TelCLIR() As Boolean
        Get
            Return False
        End Get
    End Property
    ' FritzBox
    Public ReadOnly Property P_Def_EncodeingFritzBox() As String
        Get
            Return P_Def_ErrorMinusOne
        End Get
    End Property
    Public ReadOnly Property P_Def_TBFBAdr() As String
        Get
            Return P_Def_FritzBoxAdress
        End Get
    End Property
    Public ReadOnly Property P_Def_CBForceFBAddr() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property P_Def_TBBenutzer() As String
        Get
            Return P_Def_StringEmpty
        End Get
    End Property
    Public ReadOnly Property P_Def_TBPasswort() As String
        Get
            Return P_Def_StringEmpty
        End Get
    End Property
    ' Indizierung
    Public ReadOnly Property P_Def_LLetzteIndizierung() As Date
        Get
            Return System.DateTime.Now
        End Get
    End Property
    ' Note
    Public ReadOnly Property P_Def_CBNote() As Boolean
        Get
            Return False
        End Get
    End Property

#End Region
#Region "Organisation Properties"
    Private ReadOnly Property P_Def_Options() As String
        Get
            Return "Optionen"
        End Get
    End Property
    Private ReadOnly Property P_Def_Statistics() As String
        Get
            Return "Statistik"
        End Get
    End Property
    Private ReadOnly Property P_Def_Journal() As String
        Get
            Return "Journal"
        End Get
    End Property
    Private ReadOnly Property P_Def_Phoner() As String
        Get
            Return "Phoner"
        End Get
    End Property
#End Region

#Region "Debug Properties"
    Public ReadOnly Property P_Debug_Use_WebClient() As Boolean
        Get
            Return False
        End Get
    End Property
#End Region

    Public Sub New()
        ' Pfad zur Einstellungsdatei ermitteln
        Dim ConfigPfad As String
        P_Arbeitsverzeichnis = GetSettingsVBA("Arbeitsverzeichnis", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\" & P_Def_Addin_LangName & "\")
        configpfad = P_Arbeitsverzeichnis & P_Def_Config_FileName

        XMLDoc = New XmlDocument()
        With My.Computer.FileSystem
            If .FileExists(ConfigPfad) Then
                XMLDoc.Load(ConfigPfad)
            Else
                XMLDoc.LoadXml("<?xml version=""1.0"" encoding=""UTF-8""?><" & RootName & "/>")
                If Not .DirectoryExists(P_Arbeitsverzeichnis) Then .CreateDirectory(P_Arbeitsverzeichnis)
                .WriteAllText(ConfigPfad, XMLDoc.InnerXml, True)
                SaveSettingsVBA("Arbeitsverzeichnis", P_Arbeitsverzeichnis)
            End If
        End With
        CleanUpXML()
        tSpeichern = New Timer
        With tSpeichern
            .Interval = TimeSpan.FromMinutes(Speicherintervall).TotalMilliseconds
            .Start()
        End With
        LoadOptionData()
    End Sub

    Private Sub LoadOptionData()

        Me.P_TBLandesVW = Read(P_Def_Options, "TBLandesVW", P_Def_TBLandesVW)
        Me.P_TBAmt = Read(P_Def_Options, "TBAmt", P_Def_TBAmt)
        Me.P_TBFBAdr = Read(P_Def_Options, "TBFBAdr", P_Def_TBFBAdr)
        Me.P_CBForceFBAddr = CBool(Read(P_Def_Options, "CBForceFBAddr", CStr(P_Def_CBForceFBAddr)))
        Me.P_TBBenutzer = Read(P_Def_Options, "TBBenutzer", P_Def_TBBenutzer)
        Me.P_TBPasswort = Read(P_Def_Options, "TBPasswort", P_Def_TBPasswort)

        Me.P_TBVorwahl = Read(P_Def_Options, "TBVorwahl", P_Def_TBVorwahl)
        Me.P_TBEnblDauer = CInt(Read(P_Def_Options, "TBEnblDauer", CStr(P_Def_TBEnblDauer)))
        Me.P_CBAnrMonAuto = CBool(Read(P_Def_Options, "CBAnrMonAuto", CStr(P_Def_CBAnrMonAuto)))
        Me.P_TBAnrMonX = CInt(Read(P_Def_Options, "TBAnrMonX", CStr(P_Def_TBAnrMonX)))
        Me.P_TBAnrMonY = CInt(Read(P_Def_Options, "TBAnrMonY", CStr(P_Def_TBAnrMonY)))
        Me.P_CBAnrMonMove = CBool(Read(P_Def_Options, "CBAnrMonMove", CStr(P_Def_CBAnrMonMove)))
        Me.P_CBAnrMonTransp = CBool(Read(P_Def_Options, "CBAnrMonTransp", CStr(P_Def_CBAnrMonTransp)))
        Me.P_TBAnrMonMoveGeschwindigkeit = CInt(Read(P_Def_Options, "TBAnrMonMoveGeschwindigkeit", CStr(P_Def_TBAnrMonMoveGeschwindigkeit)))
        Me.P_CBoxAnrMonStartPosition = CInt(Read(P_Def_Options, "CBoxAnrMonStartPosition", CStr(P_Def_CBoxAnrMonStartPosition)))
        Me.P_CBoxAnrMonMoveDirection = CInt(Read(P_Def_Options, "CBoxAnrMonMoveDirection", CStr(P_Def_CBoxAnrMonMoveDirection)))
        Me.P_CBAnrMonZeigeKontakt = CBool(Read(P_Def_Options, "CBAnrMonZeigeKontakt", CStr(P_Def_CBAnrMonZeigeKontakt)))
        Me.P_CBAnrMonContactImage = CBool(Read(P_Def_Options, "CBAnrMonContactImage", CStr(P_Def_CBAnrMonContactImage)))
        Me.P_CBIndexAus = CBool(Read(P_Def_Options, "CBIndexAus", CStr(P_Def_CBIndexAus)))
        Me.P_CBShowMSN = CBool(Read(P_Def_Options, "CBShowMSN", CStr(P_Def_CBShowMSN)))
        Me.P_CBJournal = CBool(Read(P_Def_Options, "CBJournal", CStr(P_Def_CBJournal)))
        Me.P_CBUseAnrMon = CBool(Read(P_Def_Options, "CBUseAnrMon", CStr(P_Def_CBUseAnrMon)))
        Me.P_CBCheckMobil = CBool(Read(P_Def_Options, "CBCheckMobil", CStr(P_Def_CBCheckMobil)))
        Me.P_CBAutoClose = CBool(Read(P_Def_Options, "CBAutoClose", CStr(P_Def_CBAutoClose)))
        Me.P_CBVoIPBuster = CBool(Read(P_Def_Options, "CBVoIPBuster", CStr(P_Def_CBVoIPBuster)))
        Me.P_CBCbCunterbinden = CBool(Read(P_Def_Options, "CBCbCunterbinden", CStr(P_Def_CBCbCunterbinden)))
        Me.P_CBCallByCall = CBool(Read(P_Def_Options, "CBCallByCall", CStr(P_Def_CBCallByCall)))
        Me.P_CBDialPort = CBool(Read(P_Def_Options, "CBDialPort", CStr(P_Def_CBDialPort)))
        Me.P_CBKErstellen = CBool(Read(P_Def_Options, "CBKErstellen", CStr(P_Def_CBKErstellen)))
        Me.P_CBLogFile = CBool(Read(P_Def_Options, "CBLogFile", CStr(P_Def_CBLogFile)))
        ' Einstellungen für die Symbolleiste laden
        Me.P_CBSymbWwdh = CBool(Read(P_Def_Options, "CBSymbWwdh", CStr(P_Def_CBSymbWwdh)))
        Me.P_CBSymbAnrMon = CBool(Read(P_Def_Options, "CBSymbAnrMon", CStr(P_Def_CBSymbAnrMon)))
        Me.P_CBSymbAnrMonNeuStart = CBool(Read(P_Def_Options, "CBSymbAnrMonNeuStart", CStr(P_Def_CBSymbAnrMonNeuStart)))
        Me.P_CBSymbAnrListe = CBool(Read(P_Def_Options, "CBSymbAnrListe", CStr(P_Def_CBSymbAnrListe)))
        Me.P_CBSymbDirekt = CBool(Read(P_Def_Options, "CBSymbDirekt", CStr(P_Def_CBSymbDirekt)))
        Me.P_CBSymbRWSuche = CBool(Read(P_Def_Options, "CBSymbRWSuche", CStr(P_Def_CBSymbRWSuche)))
        Me.P_TVKontaktOrdnerEntryID = Read(P_Def_Options, "TVKontaktOrdnerEntryID", CStr(P_Def_TVKontaktOrdnerEntryID))
        Me.P_TVKontaktOrdnerStoreID = Read(P_Def_Options, "TVKontaktOrdnerStoreID", CStr(P_Def_TVKontaktOrdnerStoreID))
        Me.P_CBSymbVIP = CBool(Read(P_Def_Options, "CBSymbVIP", CStr(P_Def_CBSymbVIP)))
        Me.P_CBSymbJournalimport = CBool(Read(P_Def_Options, "CBSymbJournalimport", CStr(P_Def_CBSymbJournalimport)))
        Me.P_CBJImport = CBool(Read(P_Def_Options, "CBJImport", CStr(P_Def_CBJImport)))
        ' Einstellungen füer die Rückwärtssuche laden
        Me.P_CBKHO = CBool(Read(P_Def_Options, "CBKHO", CStr(P_Def_CBKHO)))
        Me.P_CBRWS = CBool(Read(P_Def_Options, "CBRWS", CStr(P_Def_CBRWS)))
        Me.P_CBRWSIndex = CBool(Read(P_Def_Options, "CBRWSIndex", CStr(P_Def_CBRWSIndex)))
        Me.P_ComboBoxRWS = CInt(Read(P_Def_Options, "ComboBoxRWS", CStr(P_Def_ComboBoxRWS)))
        Me.P_CBIndex = CBool(Read(P_Def_Options, "CBIndex", CStr(P_Def_CBIndex)))
        ' StoppUhr
        Me.P_CBStoppUhrEinblenden = CBool(Read(P_Def_Options, "CBStoppUhrEinblenden", CStr(P_Def_CBStoppUhrEinblenden)))
        Me.P_CBStoppUhrAusblenden = CBool(Read(P_Def_Options, "CBStoppUhrAusblenden", CStr(P_Def_CBStoppUhrAusblenden)))
        Me.P_TBStoppUhr = CInt(Read(P_Def_Options, "TBStoppUhr", CStr(P_Def_TBStoppUhr)))
        Me.P_CBStoppUhrX = CInt(Read(P_Def_Options, "CBStoppUhrX", CStr(P_Def_CBStoppUhrX)))
        Me.P_CBStoppUhrY = CInt(Read(P_Def_Options, "CBStoppUhrY", CStr(P_Def_CBStoppUhrY)))
        ' Telefonnummernformatierung
        Me.P_TBTelNrMaske = Read(P_Def_Options, "TBTelNrMaske", P_Def_TBTelNrMaske)
        Me.P_CBTelNrGruppieren = CBool(Read(P_Def_Options, "CBTelNrGruppieren", CStr(P_Def_CBTelNrGruppieren)))
        Me.P_CBintl = CBool(Read(P_Def_Options, "CBintl", CStr(P_Def_CBintl)))
        Me.P_CBIgnoTelNrFormat = CBool(Read(P_Def_Options, "CBIgnoTelNrFormat", CStr(P_Def_CBIgnoTelNrFormat)))
        ' Phoner
        Me.P_CBPhoner = CBool(Read(P_Def_Phoner, "CBPhoner", CStr(P_Def_CBPhoner)))
        Me.P_PhonerVerfügbar = CBool(Read(P_Def_Phoner, "PhonerVerfügbar", CStr(P_Def_PhonerVerfügbar)))
        Me.P_ComboBoxPhonerSIP = CInt(Read(P_Def_Phoner, "ComboBoxPhonerSIP", CStr(P_Def_ComboBoxPhonerSIP)))
        Me.P_CBPhonerAnrMon = CBool(Read(P_Def_Phoner, "CBPhonerAnrMon", CStr(P_Def_CBPhonerAnrMon)))
        Me.P_TBPhonerPasswort = Read(P_Def_Phoner, "TBPhonerPasswort", P_Def_TBPhonerPasswort)
        Me.P_PhonerTelNameIndex = CInt(Read(P_Def_Phoner, "PhonerTelNameIndex", CStr(P_Def_PhonerTelNameIndex)))
        ' Statistik
        Me.P_StatResetZeit = CDate(Read(P_Def_Statistics, "ResetZeit", CStr(P_Def_StatResetZeit)))
        Me.P_StatVerpasst = CInt(Read(P_Def_Statistics, "Verpasst", CStr(P_Def_StatVerpasst)))
        Me.P_StatNichtErfolgreich = CInt(Read(P_Def_Statistics, "Nichterfolgreich", CStr(P_Def_StatNichtErfolgreich)))
        Me.P_StatKontakt = CInt(Read(P_Def_Statistics, "Kontakt", CStr(P_Def_StatKontakt)))
        Me.P_StatJournal = CInt(Read(P_Def_Statistics, "Journal", CStr(P_Def_StatJournal)))
        Me.P_StatOLClosedZeit = CDate(Read(P_Def_Journal, "SchließZeit", CStr(P_Def_StatOLClosedZeit)))
        ' Wählbox
        Me.P_TelAnschluss = CInt(Read(P_Def_Options, "Anschluss", CStr(P_Def_TelAnschluss)))
        Me.P_TelFestnetz = CBool(Read(P_Def_Options, "Festnetz", CStr(P_TelFestnetz)))
        Me.P_TelCLIR = CBool(Read(P_Def_Options, "CLIR", CStr(P_Def_TelCLIR)))
        Me.P_EncodeingFritzBox = Read(P_Def_Options, "EncodeingFritzBox", P_Def_EncodeingFritzBox)
        ' Indizierung
        Me.P_LLetzteIndizierung = CDate(Read(P_Def_Options, "LLetzteIndizierung", CStr(P_Def_LLetzteIndizierung)))
        ' Notiz
        Me.P_CBNote = CBool(Read(P_Def_Options, "CBNote", CStr(P_Def_CBNote)))
    End Sub

    Private Sub SaveOptionData()
        Write(P_Def_Options, "TBLandesVW", Me.P_TBLandesVW)
        Write(P_Def_Options, "TBAmt", Me.P_TBAmt)
        Write(P_Def_Options, "TBFBAdr", Me.P_TBFBAdr)
        Write(P_Def_Options, "CBForceFBAddr", CStr(Me.P_CBForceFBAddr))
        Write(P_Def_Options, "TBBenutzer", Me.P_TBBenutzer)
        Write(P_Def_Options, "TBPasswort", Me.P_TBPasswort)
        Write(P_Def_Options, "TBVorwahl", Me.P_TBVorwahl)
        Write(P_Def_Options, "TBEnblDauer", CStr(Me.P_TBEnblDauer))
        Write(P_Def_Options, "CBAnrMonAuto", CStr(Me.P_CBAnrMonAuto))
        Write(P_Def_Options, "TBAnrMonX", CStr(Me.P_TBAnrMonX))
        Write(P_Def_Options, "TBAnrMonY", CStr(Me.P_TBAnrMonY))
        Write(P_Def_Options, "CBAnrMonMove", CStr(Me.P_CBAnrMonMove))
        Write(P_Def_Options, "CBAnrMonTransp", CStr(Me.P_CBAnrMonTransp))
        Write(P_Def_Options, "TBAnrMonMoveGeschwindigkeit", CStr(Me.P_TBAnrMonMoveGeschwindigkeit))
        Write(P_Def_Options, "CBoxAnrMonStartPosition", CStr(Me.P_CBoxAnrMonStartPosition))
        Write(P_Def_Options, "CBoxAnrMonMoveDirection", CStr(Me.P_CBoxAnrMonMoveDirection))
        Write(P_Def_Options, "CBAnrMonZeigeKontakt", CStr(Me.P_CBAnrMonZeigeKontakt))
        Write(P_Def_Options, "CBAnrMonContactImage", CStr(Me.P_CBAnrMonContactImage))
        Write(P_Def_Options, "CBIndexAus", CStr(Me.P_CBIndexAus))
        Write(P_Def_Options, "CBShowMSN", CStr(Me.P_CBShowMSN))
        Write(P_Def_Options, "CBAutoClose", CStr(Me.P_CBAutoClose))
        Write(P_Def_Options, "CBVoIPBuster", CStr(Me.P_CBVoIPBuster))
        Write(P_Def_Options, "CBCbCunterbinden", CStr(Me.P_CBVoIPBuster))
        Write(P_Def_Options, "CBCallByCall", CStr(Me.P_CBCallByCall))
        Write(P_Def_Options, "CBDialPort", CStr(Me.P_CBDialPort))
        Write(P_Def_Options, "CBKErstellen", CStr(Me.P_CBKErstellen))
        Write(P_Def_Options, "CBLogFile", CStr(Me.P_CBLogFile))
        ' Einstellungen für die Symbolleiste laden
        Write(P_Def_Options, "CBSymbWwdh", CStr(Me.P_CBSymbWwdh))
        Write(P_Def_Options, "CBSymbAnrMon", CStr(Me.P_CBSymbAnrMon))
        Write(P_Def_Options, "CBSymbAnrMonNeuStart", CStr(Me.P_CBSymbAnrMonNeuStart))
        Write(P_Def_Options, "CBSymbAnrListe", CStr(Me.P_CBSymbAnrListe))
        Write(P_Def_Options, "CBSymbDirekt", CStr(Me.P_CBSymbDirekt))
        Write(P_Def_Options, "CBSymbRWSuche", CStr(Me.P_CBSymbRWSuche))
        Write(P_Def_Options, "CBSymbVIP", CStr(Me.P_CBSymbVIP))
        Write(P_Def_Options, "CBSymbJournalimport", CStr(Me.P_CBSymbJournalimport))
        Write(P_Def_Options, "CBJImport", CStr(Me.P_CBJImport))
        ' Einstellungen füer die Rückwärtssuche laden
        Write(P_Def_Options, "CBKHO", CStr(Me.P_CBKHO))
        Write(P_Def_Options, "CBRWS", CStr(Me.P_CBRWS))
        Write(P_Def_Options, "CBRWSIndex", CStr(Me.P_CBRWSIndex))
        Write(P_Def_Options, "TVKontaktOrdnerEntryID", CStr(Me.P_TVKontaktOrdnerEntryID))
        Write(P_Def_Options, "TVKontaktOrdnerStoreID", CStr(Me.P_TVKontaktOrdnerStoreID))
        Write(P_Def_Options, "", CStr(Me.P_ComboBoxRWS))
        Write(P_Def_Options, "CBIndex", CStr(Me.P_CBIndex))
        Write(P_Def_Options, "CBJournal", CStr(Me.P_CBJournal))
        Write(P_Def_Options, "CBUseAnrMon", CStr(Me.P_CBUseAnrMon))
        Write(P_Def_Options, "CBCheckMobil", CStr(Me.P_CBCheckMobil))
        'StoppUhr
        Write(P_Def_Options, "CBStoppUhrEinblenden", CStr(Me.P_CBStoppUhrEinblenden))
        Write(P_Def_Options, "CBStoppUhrAusblenden", CStr(Me.P_CBStoppUhrAusblenden))
        Write(P_Def_Options, "TBStoppUhr", CStr(Me.P_TBStoppUhr))
        Write(P_Def_Options, "TBTelNrMaske", Me.P_TBTelNrMaske)
        Write(P_Def_Options, "CBTelNrGruppieren", CStr(Me.P_CBTelNrGruppieren))
        Write(P_Def_Options, "CBintl", CStr(Me.P_CBintl))
        Write(P_Def_Options, "CBIgnoTelNrFormat", CStr(Me.P_CBIgnoTelNrFormat))
        Write(P_Def_Options, "CBStoppUhrX", CStr(Me.P_CBStoppUhrX))
        Write(P_Def_Options, "CBStoppUhrY", CStr(Me.P_CBStoppUhrY))
        ' Phoner
        Write(P_Def_Phoner, "CBPhoner", CStr(Me.P_CBPhoner))
        Write(P_Def_Phoner, "PhonerVerfügbar", CStr(Me.P_PhonerVerfügbar))
        Write(P_Def_Phoner, "ComboBoxPhonerSIP", CStr(Me.P_ComboBoxPhonerSIP))
        Write(P_Def_Phoner, "CBPhonerAnrMon", CStr(Me.P_CBPhonerAnrMon))
        Write(P_Def_Phoner, "TBPhonerPasswort", Me.P_TBPhonerPasswort)
        Write(P_Def_Phoner, "PhonerTelNameIndex", CStr(Me.P_PhonerTelNameIndex))
        ' Statistik
        Write(P_Def_Statistics, "CBPhoner", CStr(Me.P_StatResetZeit))
        Write(P_Def_Statistics, "PhonerVerfügbar", CStr(Me.P_StatVerpasst))
        Write(P_Def_Statistics, "ComboBoxPhonerSIP", CStr(Me.P_StatNichtErfolgreich))
        Write(P_Def_Statistics, "CBPhonerAnrMon", CStr(Me.P_StatKontakt))
        Write(P_Def_Statistics, "TBPhonerPasswort", CStr(Me.P_StatJournal))
        Write(P_Def_Journal, "SchließZeit", CStr(Me.P_StatOLClosedZeit))
        ' Wählbox
        Write(P_Def_Options, "Anschluss", CStr(Me.P_TelAnschluss))
        Write(P_Def_Options, "Festnetz", CStr(Me.P_TelFestnetz))
        Write(P_Def_Options, "CLIR", CStr(Me.P_TelCLIR))
        'FritzBox
        Write(P_Def_Options, "EncodeingFritzBox", Me.P_EncodeingFritzBox)
        'indizierung
        Write(P_Def_Options, "LLetzteIndizierung", CStr(Me.P_LLetzteIndizierung))
        XMLDoc.Save(P_Arbeitsverzeichnis & P_Def_Config_FileName)
        SaveSettingsVBA("Arbeitsverzeichnis", P_Arbeitsverzeichnis)
    End Sub

    Protected Overrides Sub Finalize()
        SaveOptionData()
        XMLDoc.Save(P_Arbeitsverzeichnis & P_Def_Config_FileName)
        XMLDoc = Nothing
        If Not tSpeichern Is Nothing Then
            tSpeichern.Stop()
            tSpeichern.Dispose()
            tSpeichern = Nothing
        End If

        MyBase.Finalize()
    End Sub

#Region "XML"
#Region "Read"
    Public Overloads Function Read(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal sDefault As String) As String
        Dim xPathTeile As New ArrayList
        With xPathTeile
            .Add(IIf(IsNumeric(Left(DieSektion, 1)), "ID" & DieSektion, DieSektion))
            .Add(IIf(IsNumeric(Left(DerEintrag, 1)), "ID" & DerEintrag, DerEintrag))
        End With
        Return Read(xPathTeile, sDefault)
    End Function

    Public Overloads Function Read(ByVal xPathTeile As ArrayList, ByVal sDefault As String) As String
        Read = sDefault

        Dim tmpXMLNodeList As XmlNodeList
        Dim xPath As String = CreateXPath(xPathTeile)

        If CheckXPathRead(xPath) Then
            tmpXMLNodeList = XMLDoc.SelectNodes(xPath)
            If Not tmpXMLNodeList.Count = 0 Then
                Read = P_Def_StringEmpty
                For Each tmpXMLNode As XmlNode In tmpXMLNodeList
                    Read += tmpXMLNode.InnerText & ";"
                Next
                Read = Left(Read, Len(Read) - 1)
            End If
        End If
        xPathTeile = Nothing
    End Function

    Function ReadElementName(ByVal ZielKnoten As ArrayList, ByVal sDefault As String) As String
        ReadElementName = sDefault
        Dim xPath As String
        Dim tmpXMLNode As XmlNode
        xPath = CreateXPath(ZielKnoten)
        With XMLDoc
            tmpXMLNode = .SelectSingleNode(xPath)
            If Not tmpXMLNode Is Nothing Then
                ReadElementName = tmpXMLNode.ParentNode.Name
            End If
        End With
        tmpXMLNode = Nothing
    End Function
#End Region
#Region "Write"
    Public Overloads Function Write(ByVal DieSektion As String, ByVal DerEintrag As String, ByVal Value As String) As Boolean
        Dim xPathTeile As New ArrayList
        With xPathTeile
            .Add(IIf(IsNumeric(Left(DieSektion, 1)), "ID" & DieSektion, DieSektion))
            .Add(IIf(IsNumeric(Left(DerEintrag, 1)), "ID" & DerEintrag, DerEintrag))
        End With
        Return Write(xPathTeile, Value)
    End Function

    Public Overloads Function Write(ByVal ZielKnoten As ArrayList, ByVal Value As String) As Boolean
        Return Write(ZielKnoten, Value, P_Def_StringEmpty, P_Def_StringEmpty)
    End Function

    Public Overloads Function Write(ByVal ZielKnoten As ArrayList, ByVal Value As String, ByVal AttributeName As String, ByVal AttributeValue As String) As Boolean
        Dim xPathTeile As New ArrayList
        Dim sTmpXPath As String = P_Def_StringEmpty
        Dim xPath As String
        Dim tmpXMLNode As XmlNode
        Dim tmpXMLNodeList As XmlNodeList
        Dim tmpXMLAttribute As XmlAttribute
        xPath = CreateXPath(ZielKnoten)
        If CheckXPathWrite(xPath) Then
            With XMLDoc
                tmpXMLNodeList = .SelectNodes(xPath)
                If Not tmpXMLNodeList.Count = 0 Then
                    For Each tmpXMLNode In tmpXMLNodeList
                        If Not AttributeName = P_Def_StringEmpty Then
                            If Not (tmpXMLNode.ChildNodes.Count = 0 And tmpXMLNode.Value = Nothing) Then
                                tmpXMLNode = .SelectSingleNode(xPath & CStr(IIf(Not AttributeName = P_Def_StringEmpty, "[@" & AttributeName & "=""" & AttributeValue & """]", P_Def_StringEmpty)))
                            End If
                            If tmpXMLNode Is Nothing Then
                                tmpXMLNode = .SelectSingleNode(xPath).ParentNode.AppendChild(.CreateElement(.SelectSingleNode(xPath).Name))
                            End If
                            tmpXMLAttribute = XMLDoc.CreateAttribute(AttributeName)
                            tmpXMLAttribute.Value = AttributeValue
                            tmpXMLNode.Attributes.Append(tmpXMLAttribute)
                        End If
                        tmpXMLNode.InnerText() = Value
                    Next
                Else
                    For Each sNodeName As String In ZielKnoten
                        If IsNumeric(Left(sNodeName, 1)) Then sNodeName = "ID" & sNodeName
                        xPathTeile.Add(sNodeName)
                        xPath = CreateXPath(xPathTeile)
                        If .SelectSingleNode(xPath) Is Nothing Then
                            .SelectSingleNode(sTmpXPath).AppendChild(.CreateElement(sNodeName))
                        End If
                        sTmpXPath = xPath
                    Next
                    Write(ZielKnoten, Value, AttributeName, AttributeValue)
                End If
            End With
            Write = True
        Else
            Write = False
        End If
        xPathTeile = Nothing
        tmpXMLAttribute = Nothing
        tmpXMLNode = Nothing
    End Function

    Public Overloads Function WriteAttribute(ByVal ZielKnoten As ArrayList, ByVal AttributeName As String, ByVal AttributeValue As String) As Boolean
        WriteAttribute = False
        Dim xPath As String
        xPath = CreateXPath(ZielKnoten)
        WriteAttribute(XMLDoc.SelectNodes(xPath), AttributeName, AttributeValue)
    End Function

    Public Overloads Function WriteAttribute(ByRef tmpXMLNodeList As XmlNodeList, ByVal AttributeName As String, ByVal AttributeValue As String) As Boolean
        WriteAttribute = True

        Dim tmpXMLAttribute As XmlAttribute

        With XMLDoc
            If Not tmpXMLNodeList.Count = 0 Then
                For Each tmpXMLNode As XmlNode In tmpXMLNodeList
                    tmpXMLAttribute = tmpXMLNode.Attributes.ItemOf(AttributeName)
                    If tmpXMLAttribute Is Nothing Then
                        tmpXMLAttribute = .CreateAttribute(AttributeName)
                        tmpXMLNode.Attributes.Append(tmpXMLAttribute)
                    End If
                    tmpXMLAttribute.Value = AttributeValue
                Next
            End If
        End With
    End Function
#End Region
#Region "Löschen"

    Public Overloads Function Delete(ByVal DieSektion As String) As Boolean
        Dim xPathTeile As New ArrayList
        xPathTeile.Add(DieSektion)
        Return Delete(xPathTeile)
    End Function

    Public Overloads Function Delete(ByVal alxPathTeile As ArrayList) As Boolean
        Dim tmpXMLNode As XmlNode
        Dim xPath As String = CreateXPath(alxPathTeile)
        With XMLDoc
            If Not .SelectSingleNode(xPath) Is Nothing Then
                tmpXMLNode = .SelectSingleNode(xPath).ParentNode
                tmpXMLNode.RemoveChild(.SelectSingleNode(xPath))
                If tmpXMLNode.ChildNodes.Count = 0 Then
                    tmpXMLNode.ParentNode.RemoveChild(tmpXMLNode)
                End If
            End If
        End With
        alxPathTeile = Nothing
        Return True
    End Function

#End Region
#Region "Knoten"
    Function CreateXMLNode(ByVal NodeName As String, ByVal SubNodeName As ArrayList, ByVal SubNodeValue As ArrayList, ByVal AttributeName As ArrayList, ByVal AttributeValue As ArrayList) As XmlNode
        CreateXMLNode = Nothing
        If SubNodeName.Count = SubNodeValue.Count Then

            Dim tmpXMLNode As XmlNode
            Dim tmpXMLChildNode As XmlNode
            Dim tmpXMLAttribute As XmlAttribute
            tmpXMLNode = XMLDoc.CreateNode(XmlNodeType.Element, NodeName, P_Def_StringEmpty)
            With tmpXMLNode
                For i As Integer = 0 To SubNodeName.Count - 1
                    If Not SubNodeValue.Item(i).ToString = P_Def_ErrorMinusOne Then
                        tmpXMLChildNode = XMLDoc.CreateNode(XmlNodeType.Element, SubNodeName.Item(i).ToString, P_Def_StringEmpty)
                        tmpXMLChildNode.InnerText = SubNodeValue.Item(i).ToString
                        .AppendChild(tmpXMLChildNode)
                    End If
                Next
            End With
            For i As Integer = 0 To AttributeName.Count - 1
                If Not AttributeValue.Item(i) Is Nothing Then
                    tmpXMLAttribute = XMLDoc.CreateAttribute(AttributeName.Item(i).ToString)
                    tmpXMLAttribute.Value = AttributeValue.Item(i).ToString
                    tmpXMLNode.Attributes.Append(tmpXMLAttribute)
                End If
            Next

            CreateXMLNode = tmpXMLNode

            tmpXMLAttribute = Nothing
            tmpXMLNode = Nothing
            tmpXMLChildNode = Nothing
        End If
    End Function

    Sub ReadXMLNode(ByVal alxPathTeile As ArrayList, ByVal SubNodeName As ArrayList, ByRef SubNodeValue As ArrayList, ByVal AttributeName As String, ByVal AttributeValue As String)

        If SubNodeName.Count = SubNodeValue.Count Then
            Dim xPath As String
            Dim tmpXMLNode As XmlNode
            With XMLDoc
                ' BUG: 
                If Not AttributeValue = P_Def_StringEmpty And Not AttributeName = P_Def_StringEmpty Then alxPathTeile.Add("[@" & AttributeName & "=""" & AttributeValue & """]")
                xPath = CreateXPath(alxPathTeile)
                If Not AttributeValue = P_Def_StringEmpty Then alxPathTeile.RemoveAt(alxPathTeile.Count - 1)
                tmpXMLNode = .SelectSingleNode(xPath)
                If Not tmpXMLNode Is Nothing Then
                    With tmpXMLNode
                        For Each XmlChildNode As XmlNode In tmpXMLNode.ChildNodes
                            If Not SubNodeName.IndexOf(XmlChildNode.Name) = -1 Then
                                SubNodeValue.Item(SubNodeName.IndexOf(XmlChildNode.Name)) = XmlChildNode.InnerText
                            End If

                        Next
                    End With
                End If
            End With
            tmpXMLNode = Nothing
        End If
    End Sub

    Sub AppendNode(ByVal alxPathTeile As ArrayList, ByVal Knoten As XmlNode)
        Dim xPathTeileEC As Long = alxPathTeile.Count
        Dim DestxPath As String
        Dim tmpxPath As String = P_Def_StringEmpty
        Dim tmpXMLNode As XmlNode
        DestxPath = CreateXPath(alxPathTeile)
        With XMLDoc
            tmpXMLNode = .SelectSingleNode(DestxPath)
            If tmpXMLNode Is Nothing Then
                Write(alxPathTeile, "")
                tmpXMLNode = .SelectSingleNode(DestxPath)
            End If
            'Attribute
            alxPathTeile.Add(Knoten.Name)
            With Knoten
                If Not .Attributes.Count = 0 Then
                    For i = 0 To .Attributes.Count - 1
                        ' String "tmpxPath" wird hier missbraucht, damit keine unnötige Variable deklariert werden muss.
                        tmpxPath += "[@" & .Attributes.Item(i).Name & "=""" & .Attributes.Item(i).Value & """]"
                    Next
                    alxPathTeile.Add(Replace(tmpxPath, "][@", " and @", , , CompareMethod.Text))
                End If
            End With
            tmpxPath = CreateXPath(alxPathTeile)

            If Not .SelectSingleNode(tmpxPath) Is Nothing Then
                tmpXMLNode.RemoveChild(.SelectSingleNode(tmpxPath))
            End If
            tmpXMLNode.AppendChild(Knoten)
        End With
        Do Until alxPathTeile.Count = xPathTeileEC
            alxPathTeile.RemoveAt(alxPathTeile.Count - 1)
        Loop

    End Sub

    Function SubNoteCount(ByVal alxPathTeile As ArrayList) As Integer
        SubNoteCount = 0
        Dim tmpxPath As String
        Dim tmpXMLNode As XmlNode
        tmpxPath = CreateXPath(alxPathTeile)
        With XMLDoc
            tmpXMLNode = .SelectSingleNode(tmpxPath)
            If Not tmpXMLNode Is Nothing Then
                SubNoteCount = tmpXMLNode.ChildNodes.Count
            End If
        End With
        tmpXMLNode = Nothing
    End Function

#End Region
#Region "Speichern"
    Sub SpeichereXMLDatei()
        SaveOptionData()
    End Sub

    Private Sub tSpeichern_Elapsed(sender As Object, e As ElapsedEventArgs) Handles tSpeichern.Elapsed
        SaveOptionData()
    End Sub
#End Region
#End Region

#Region "Registry VBA GetSettings SetSettings"
    Public Function GetSettingsVBA(ByVal Key As String, ByVal DefaultValue As String) As String
        Return GetSetting(P_Def_Addin_KurzName, P_Def_Options, Key, DefaultValue)
    End Function
    Public Sub SaveSettingsVBA(ByVal Key As String, ByVal DefaultValue As String)
        SaveSetting(P_Def_Addin_KurzName, P_Def_Options, Key, DefaultValue)
    End Sub
#End Region


#Region "Stuff"
    Private Sub CleanUpXML()
        Dim tmpNode As XmlNode
        Dim xPathTeile As New ArrayList
        Dim xPath As String

        With XMLDoc
            ' Diverse Knoten des Journals löschen
            xPathTeile.Add(P_Def_Journal)
            xPathTeile.Add("SchließZeit")
            xPath = CreateXPath(xPathTeile)
            tmpNode = .SelectSingleNode(xPath)
            xPathTeile.Remove("SchließZeit")
            xPath = CreateXPath(xPathTeile)
            If Not tmpNode Is Nothing Then
                .SelectSingleNode(xPath).RemoveAll()
                .SelectSingleNode(xPath).AppendChild(tmpNode)
            End If
            ' Alle Knoten LetzterAnrufer löschen
            'xPathTeile.Clear()
            'xPathTeile.Add("LetzterAnrufer")
            'xPath = CreateXPath(xPathTeile)
            'tmpNode = .SelectSingleNode(xPath)
            'If Not tmpNode Is Nothing Then
            '    .DocumentElement.RemoveChild(.SelectSingleNode(xPath))
            'End If
            xPathTeile = Nothing
        End With
    End Sub

    Function CreateXPath(ByVal xPathElements As ArrayList) As String
        If Not xPathElements.Item(0).ToString = XMLDoc.DocumentElement.Name Then xPathElements.Insert(0, XMLDoc.DocumentElement.Name)
        CreateXPath = Replace(xPathSeperatorSlash & Join(xPathElements.ToArray(), xPathSeperatorSlash), xPathSeperatorSlash & xPathBracketOpen, xPathBracketOpen, , , CompareMethod.Text)
        CreateXPath = Replace(CreateXPath, xPathBracketClose & xPathBracketOpen, " and ", , , CompareMethod.Text)
    End Function

    Private Function CheckXPathWrite(ByVal xPath As String) As Boolean
        CheckXPathWrite = True

        If Not InStr(xPath, xPathSeperatorSlash & xPathWildCard, CompareMethod.Text) = 0 Then Return False '/*
        If Right(xPath, 1) = xPathSeperatorSlash Then Return False
    End Function
    Private Function CheckXPathRead(ByVal xPath As String) As Boolean
        CheckXPathRead = True

        If Not InStr(xPath, "!", CompareMethod.Text) = 0 Then Return False
        If Right(xPath, 1) = xPathSeperatorSlash Then Return False
    End Function
#End Region
End Class

