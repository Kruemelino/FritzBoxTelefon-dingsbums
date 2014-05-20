Imports System.Xml
Imports System.Timers
Imports System.ComponentModel

Public Class DataProvider
    Private XMLDoc As XmlDocument
    Private WithEvents tSpeichern As Timer
#Region "BackgroundWorker"
    Private WithEvents BWCBox As BackgroundWorker
#End Region

#Region "Windows Const für Office 2003"
#If over = 11 Then
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

    Public Const HTCAPTION As Short = 2

    Public Const WM_CONTEXTMENU As Long = &H7B ' unique to rich edit control
    Public Const WM_NCLBUTTONDOWN As Short = &HA1S
    Public Const WM_USER As Long = &H400
    Public Const WM_SYSCOMMAND As Short = &H112S
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

    Private ValueCBForceFBAddr As Boolean
    Private ValueCBAnrMonContactImage As Boolean
    Private ValueCBIndexAus As Boolean
    Private ValueCBShowMSN As Boolean
    Private ValueCBAnrMonMove As Boolean
    Private ValueCBAnrMonTransp As Boolean
    Private ValueCBAnrMonAuto As Boolean
    Private ValueCBAutoClose As Boolean
    Private ValueCBVoIPBuster As Boolean
    Private ValueCBCbCunterbinden As Boolean
    Private ValueCBCallByCall As Boolean
    Private ValueCBDialPort As Boolean
    Private ValueCBKErstellen As Boolean
    Private ValueCBLogFile As Boolean
    Private ValueCBSymbWwdh As Boolean
    Private ValueCBSymbAnrMon As Boolean
    Private ValueCBSymbAnrMonNeuStart As Boolean
    Private ValueCBSymbAnrListe As Boolean
    Private ValueCBSymbDirekt As Boolean
    Private ValueCBSymbRWSuche As Boolean
    Private ValueCBSymbVIP As Boolean
    Private ValueCBSymbJournalimport As Boolean
    Private ValueCBJImport As Boolean
    Private ValueCBRWS As Boolean
    Private ValueCBKHO As Boolean
    Private ValueCBRWSIndex As Boolean
    Private ValueComboBoxRWS As Integer
    Private ValueTVKontaktOrdnerEntryID As String
    Private ValueTVKontaktOrdnerStoreID As String
    Private ValueCBIndex As Boolean
    Private ValueTBLandesVW As String
    Private ValueTBAmt As String
    Private ValueTBFBAdr As String
    Private ValueTBBenutzer As String
    Private ValueTBPasswort As String
    Private ValueTBVorwahl As String
    Private ValueCBoxVorwahl As Integer
    'Anrufmonitor
    Private ValueTBEnblDauer As Integer
    Private ValueTBAnrMonX As Integer
    Private ValueTBAnrMonY As Integer
    Private ValueCBoxAnrMonStartPosition As Integer
    Private ValueCBoxAnrMonMoveDirection As Integer
    Private ValueTBAnrMonMoveGeschwindigkeit As Integer
    Private ValueCBAnrMonZeigeKontakt As Boolean
    Private ValueCBJournal As Boolean
    Private ValueCBUseAnrMon As Boolean
    Private ValueCBCheckMobil As Boolean
    Private ValueCLBTelNr() As String
    'StoppUhr
    Private ValueCBStoppUhrEinblenden As Boolean
    Private ValueCBStoppUhrAusblenden As Boolean
    Private ValueTBStoppUhr As Integer
    Private ValueCBStoppUhrX As Integer
    Private ValueCBStoppUhrY As Integer
    ' Telefonnummernformatierung
    Private ValueTBTelNrMaske As String
    Private ValueCBTelNrGruppieren As Boolean
    Private ValueCBintl As Boolean
    Private ValueCBIgnoTelNrFormat As Boolean
    ' Phoner
    Private ValueCBPhoner As Boolean
    Private ValuePhonerVerfügbar As Boolean
    Private ValueCBPhonerAnrMon As Boolean
    Private ValueComboBoxPhonerSIP As Integer
    Private ValueTBPhonerPasswort As String
    Private ValuePhonerTelNameIndex As Integer
    ' Statistik
    Private ValueStatResetZeit As Date
    Private ValueStatVerpasst As Integer
    Private ValueStatNichtErfolgreich As Integer
    Private ValueStatKontakt As Integer
    Private ValueStatJournal As Integer
    Private ValueStatOLClosedZeit As Date
    ' Wählbox
    Private ValueTelAnschluss As Integer
    Private ValueTelFestnetz As Boolean
    Private ValueTelCLIR As Boolean
    ' FritzBox
    Private ValueEncodeingFritzBox As String
    ' Indizierung
    Private ValueLLetzteIndizierung As Date
    ' Notiz
    Private ValueCBNote As Boolean
    'Einstellungen
    Private ValueArbeitsverzeichnis As String
    ' Vorwahllisten
    Private ValueListeOrtsVorwahlen As String()
    Private ValueListeLandesVorwahlen As String()
#End Region

#Region "Value Properties"
    ''' <summary>
    ''' Gibt die im Einstellungsdialog eingegebene Landesvorwahl zurück
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>Landesvorwahl</returns>
    ''' <remarks></remarks>
    Public Property ProperyTBLandesVW() As String
        Get
            Return ValueTBLandesVW
        End Get
        Set(ByVal value As String)
            ValueTBLandesVW = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, ob eine Amtsholung stets mitgewählt werden soll. Die Amtsholung wird in den Einstellungen festgelegt.
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>Zahl für die Amtsholung</returns>
    ''' <remarks></remarks>
    Public Property ProperyTBAmt() As String
        Get
            Return ValueTBAmt
        End Get
        Set(ByVal value As String)
            ValueTBAmt = value
        End Set
    End Property
    ''' <summary>
    ''' Eigenschaft für die hinterlege Ortsvorwahl
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>Ortsvorwahl</returns>
    ''' <remarks></remarks>
    Public Property ProperyTBVorwahl() As String
        Get
            Return ValueTBVorwahl
        End Get
        Set(ByVal value As String)
            ValueTBVorwahl = value
        End Set
    End Property
    ''' <summary>
    ''' Enthält den Index im Combobox
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ProperyCBoxVorwahl() As Integer
        Get
            Return ValueCBoxVorwahl
        End Get
        Set(ByVal value As Integer)
            ValueCBoxVorwahl = value
        End Set
    End Property
    ' Anrufmonitor
    ''' <summary>
    ''' Gibt an, wie lange der Anrufmonitor angezeigt werden soll, bevor er automatisch ausgeblendet wird
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Intervall</returns>
    ''' <remarks></remarks>
    Public Property ProperyTBEnblDauer() As Integer
        Get
            Return ValueTBEnblDauer
        End Get
        Set(ByVal value As Integer)
            ValueTBEnblDauer = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, ob der Anrufmonitor automatisch gestartét werden soll.
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns>Autostart</returns>
    ''' <remarks></remarks>
    Public Property ProperyCBAnrMonAuto() As Boolean
        Get
            Return ValueCBAnrMonAuto
        End Get
        Set(ByVal value As Boolean)
            ValueCBAnrMonAuto = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, um wieviele Punkte der Anrufmonitor in X-Richtung verschoben werden soll.
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Positionskorrektur X</returns>
    ''' <remarks></remarks>
    Public Property ProperyTBAnrMonX() As Integer
        Get
            Return ValueTBAnrMonX
        End Get
        Set(ByVal value As Integer)
            ValueTBAnrMonX = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, um wieviele Punkte der Anrufmonitor in Y-Richtung verschoben werden soll.
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Positionskorrektur Y</returns>
    ''' <remarks></remarks>
    Public Property ProperyTBAnrMonY() As Integer
        Get
            Return ValueTBAnrMonY
        End Get
        Set(ByVal value As Integer)
            ValueTBAnrMonY = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an ob der Anrufmonitor in den Bildschirm hereingescrollt werden soll.
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns>Anrufmonitorbewegung</returns>
    ''' <remarks></remarks>
    Public Property ProperyCBAnrMonMove() As Boolean
        Get
            Return ValueCBAnrMonMove
        End Get
        Set(ByVal value As Boolean)
            ValueCBAnrMonMove = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, ob der Anrufmonitor eingeblendet werden soll.
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ProperyCBAnrMonTransp() As Boolean
        Get
            Return ValueCBAnrMonTransp
        End Get
        Set(ByVal value As Boolean)
            ValueCBAnrMonTransp = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt die Endposition des Anrufmonitors an.
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Wert für die Position</returns>
    ''' <remarks>FritzBoxDial.PopUpAnrMon.eStartPosition</remarks>
    Public Property ProperyCBoxAnrMonStartPosition() As Integer
        Get
            Return ValueCBoxAnrMonStartPosition
        End Get
        Set(ByVal value As Integer)
            ValueCBoxAnrMonStartPosition = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt die Bewegungsrichtung des Anrufmonitors an.
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Wert für Richtung, abhängig von der Endosition.</returns>
    ''' <remarks>FritzBoxDial.PopUpAnrMon.eMoveDirection</remarks>
    Public Property ProperyCBoxAnrMonMoveDirection() As Integer
        Get
            Return ValueCBoxAnrMonMoveDirection
        End Get
        Set(ByVal value As Integer)
            ValueCBoxAnrMonMoveDirection = value
        End Set
    End Property
    Public Property ProperyTBAnrMonMoveGeschwindigkeit() As Integer
        Get
            Return ValueTBAnrMonMoveGeschwindigkeit
        End Get
        Set(ByVal value As Integer)
            ValueTBAnrMonMoveGeschwindigkeit = value
        End Set
    End Property

    Public Property ProperyCBAnrMonZeigeKontakt() As Boolean
        Get
            Return ValueCBAnrMonZeigeKontakt
        End Get
        Set(ByVal value As Boolean)
            ValueCBAnrMonZeigeKontakt = value
        End Set
    End Property
    Public Property ProperyCBAnrMonContactImage() As Boolean
        Get
            Return ValueCBAnrMonContactImage
        End Get
        Set(ByVal value As Boolean)
            ValueCBAnrMonContactImage = value
        End Set
    End Property
    Public Property ProperyCBIndexAus() As Boolean
        Get
            Return ValueCBIndexAus
        End Get
        Set(ByVal value As Boolean)
            ValueCBIndexAus = value
        End Set
    End Property
    Public Property ProperyCBShowMSN() As Boolean
        Get
            Return ValueCBShowMSN
        End Get
        Set(ByVal value As Boolean)
            ValueCBShowMSN = value
        End Set
    End Property

    Public Property ProperyCBAutoClose() As Boolean
        Get
            Return ValueCBAutoClose
        End Get
        Set(ByVal value As Boolean)
            ValueCBAutoClose = value
        End Set
    End Property
    Public Property ProperyCBVoIPBuster() As Boolean
        Get
            Return ValueCBVoIPBuster
        End Get
        Set(ByVal value As Boolean)
            ValueCBVoIPBuster = value
        End Set
    End Property
    Public Property ProperyCBCbCunterbinden() As Boolean
        Get
            Return ValueCBCbCunterbinden
        End Get
        Set(ByVal value As Boolean)
            ValueCBCbCunterbinden = value
        End Set
    End Property
    Public Property ProperyCBCallByCall() As Boolean
        Get
            Return ValueCBCallByCall
        End Get
        Set(ByVal value As Boolean)
            ValueCBCallByCall = value
        End Set
    End Property
    Public Property ProperyCBDialPort() As Boolean
        Get
            Return ValueCBDialPort
        End Get
        Set(ByVal value As Boolean)
            ValueCBDialPort = value
        End Set
    End Property
    Public Property ProperyCBLogFile() As Boolean
        Get
            Return ValueCBLogFile
        End Get
        Set(ByVal value As Boolean)
            ValueCBLogFile = value
        End Set
    End Property
    Public Property ProperyCBSymbWwdh() As Boolean
        Get
            Return ValueCBSymbWwdh
        End Get
        Set(ByVal value As Boolean)
            ValueCBSymbWwdh = value
        End Set
    End Property
    Public Property ProperyCBSymbAnrMon() As Boolean
        Get
            Return ValueCBSymbAnrMon
        End Get
        Set(ByVal value As Boolean)
            ValueCBSymbAnrMon = value
        End Set
    End Property
    Public Property ProperyCBSymbAnrMonNeuStart() As Boolean
        Get
            Return ValueCBSymbAnrMonNeuStart
        End Get
        Set(ByVal value As Boolean)
            ValueCBSymbAnrMonNeuStart = value
        End Set
    End Property
    'Pffice 2003 und Office 2007
    Public Property ProperyCBSymbAnrListe() As Boolean
        Get
            Return ValueCBSymbAnrListe
        End Get
        Set(ByVal value As Boolean)
            ValueCBSymbAnrListe = value
        End Set
    End Property
    Public Property ProperyCBSymbDirekt() As Boolean
        Get
            Return ValueCBSymbDirekt
        End Get
        Set(ByVal value As Boolean)
            ValueCBSymbDirekt = value
        End Set
    End Property
    Public Property ProperyCBSymbRWSuche() As Boolean
        Get
            Return ValueCBSymbRWSuche
        End Get
        Set(ByVal value As Boolean)
            ValueCBSymbRWSuche = value
        End Set
    End Property
    Public Property ProperyCBSymbVIP() As Boolean
        Get
            Return ValueCBSymbVIP
        End Get
        Set(ByVal value As Boolean)
            ValueCBSymbVIP = value
        End Set
    End Property
    Public Property ProperyCBSymbJournalimport() As Boolean
        Get
            Return ValueCBSymbJournalimport
        End Get
        Set(ByVal value As Boolean)
            ValueCBSymbJournalimport = value
        End Set
    End Property
    Public Property ProperyCBJImport() As Boolean
        Get
            Return ValueCBJImport
        End Get
        Set(ByVal value As Boolean)
            ValueCBJImport = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an ob nur der Hauptkontaktordner durchsucht werden muss oder alle möglichen eingebundenen Kontaktordner
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns>True, wenn nur der Hauptkontaktordner durchsucht werden muss</returns>
    ''' <remarks></remarks>
    Public Property ProperyCBKHO() As Boolean
        Get
            Return ValueCBKHO
        End Get
        Set(ByVal value As Boolean)
            ValueCBKHO = value
        End Set
    End Property
    Public Property ProperyCBRWS() As Boolean
        Get
            Return ValueCBRWS
        End Get
        Set(ByVal value As Boolean)
            ValueCBRWS = value
        End Set
    End Property
    Public Property ProperyCBKErstellen() As Boolean
        Get
            Return ValueCBKErstellen
        End Get
        Set(ByVal value As Boolean)
            ValueCBKErstellen = value
        End Set
    End Property
    Public Property ProperyCBRWSIndex() As Boolean
        Get
            Return ValueCBRWSIndex
        End Get
        Set(ByVal value As Boolean)
            ValueCBRWSIndex = value
        End Set
    End Property
    Public Property ProperyComboBoxRWS() As Integer
        Get
            Return ValueComboBoxRWS
        End Get
        Set(ByVal value As Integer)
            ValueComboBoxRWS = value
        End Set
    End Property
    Public Property ProperyTVKontaktOrdnerEntryID() As String
        Get
            Return ValueTVKontaktOrdnerEntryID
        End Get
        Set(ByVal value As String)
            ValueTVKontaktOrdnerEntryID = value
        End Set
    End Property
    Public Property ProperyTVKontaktOrdnerStoreID() As String
        Get
            Return ValueTVKontaktOrdnerStoreID
        End Get
        Set(ByVal value As String)
            ValueTVKontaktOrdnerStoreID = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, ob die Indizierung durchgeführt werden soll. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns>True/False</returns>
    ''' <remarks></remarks>
    Public Property ProperyCBIndex() As Boolean
        Get
            Return ValueCBIndex
        End Get
        Set(ByVal value As Boolean)
            ValueCBIndex = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, ob Journaleinträge erstellt werden soll. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns>True/False</returns>
    ''' <remarks></remarks>
    Public Property ProperyCBJournal() As Boolean
        Get
            Return ValueCBJournal
        End Get
        Set(ByVal value As Boolean)
            ValueCBJournal = value
        End Set
    End Property
    Public Property ProperyCBUseAnrMon() As Boolean
        Get
            Return ValueCBUseAnrMon
        End Get
        Set(ByVal value As Boolean)
            ValueCBUseAnrMon = value
        End Set
    End Property
    Public Property ProperyCBCheckMobil() As Boolean
        Get
            Return ValueCBCheckMobil
        End Get
        Set(ByVal value As Boolean)
            ValueCBCheckMobil = value
        End Set
    End Property

    Public Property ProperyCLBTelNr As String()
        Get
            Return ValueCLBTelNr
        End Get
        Set(ByVal value As String())
            ValueCLBTelNr = value
        End Set
    End Property
    'StoppUhr
    Public Property ProperyCBStoppUhrEinblenden() As Boolean
        Get
            Return ValueCBStoppUhrEinblenden
        End Get
        Set(ByVal Value As Boolean)
            ValueCBStoppUhrEinblenden = Value
        End Set
    End Property
    Public Property ProperyCBStoppUhrAusblenden() As Boolean
        Get
            Return ValueCBStoppUhrAusblenden
        End Get
        Set(ByVal Value As Boolean)
            ValueCBStoppUhrAusblenden = Value
        End Set
    End Property
    Public Property ProperyTBStoppUhr() As Integer
        Get
            Return ValueTBStoppUhr
        End Get
        Set(ByVal value As Integer)
            ValueTBStoppUhr = value
        End Set
    End Property
    Public Property ProperyCBStoppUhrX() As Integer
        Get
            Return ValueCBStoppUhrX
        End Get
        Set(ByVal value As Integer)
            ValueCBStoppUhrX = value
        End Set
    End Property
    Public Property ProperyCBStoppUhrY() As Integer
        Get
            Return ValueCBStoppUhrY
        End Get
        Set(ByVal value As Integer)
            ValueCBStoppUhrY = value
        End Set
    End Property
    ' Telefonnummernformatierung
    Public Property ProperyTBTelNrMaske() As String
        Get
            Return ValueTBTelNrMaske
        End Get
        Set(ByVal value As String)
            ValueTBTelNrMaske = value
        End Set
    End Property
    Public Property ProperyCBTelNrGruppieren() As Boolean
        Get
            Return ValueCBTelNrGruppieren
        End Get
        Set(ByVal value As Boolean)
            ValueCBTelNrGruppieren = value
        End Set
    End Property
    Public Property ProperyCBintl() As Boolean
        Get
            Return ValueCBintl
        End Get
        Set(ByVal value As Boolean)
            ValueCBintl = value
        End Set
    End Property
    Public Property ProperyCBIgnoTelNrFormat() As Boolean
        Get
            Return ValueCBIgnoTelNrFormat
        End Get
        Set(ByVal value As Boolean)
            ValueCBIgnoTelNrFormat = value
        End Set
    End Property
    'Phoner
    Public Property ProperyCBPhoner As Boolean
        Get
            Return ValueCBPhoner
        End Get
        Set(ByVal value As Boolean)
            ValueCBPhoner = value
        End Set
    End Property
    Public Property ProperyPhonerVerfügbar As Boolean
        Get
            Return ValuePhonerVerfügbar
        End Get
        Set(ByVal value As Boolean)
            ValuePhonerVerfügbar = value
        End Set
    End Property
    Public Property ProperyCBPhonerAnrMon As Boolean
        Get
            Return ValueCBPhonerAnrMon
        End Get
        Set(ByVal value As Boolean)
            ValueCBPhonerAnrMon = value
        End Set
    End Property
    Public Property ProperyComboBoxPhonerSIP() As Integer
        Get
            Return ValueComboBoxPhonerSIP
        End Get
        Set(ByVal value As Integer)
            ValueComboBoxPhonerSIP = value
        End Set
    End Property
    Public Property ProperyTBPhonerPasswort() As String
        Get
            Return ValueTBPhonerPasswort
        End Get
        Set(ByVal value As String)
            ValueTBPhonerPasswort = value
        End Set
    End Property
    Public Property ProperyPhonerTelNameIndex() As Integer
        Get
            Return ValuePhonerTelNameIndex
        End Get
        Set(ByVal value As Integer)
            ValuePhonerTelNameIndex = value
        End Set
    End Property
    ' Statistik
    Public Property ProperyStatResetZeit As Date
        Get
            Return ValueStatResetZeit
        End Get
        Set(ByVal value As Date)
            ValueStatResetZeit = value
        End Set
    End Property
    Public Property ProperyStatVerpasst As Integer
        Get
            Return ValueStatVerpasst
        End Get
        Set(ByVal value As Integer)
            ValueStatVerpasst = value
        End Set
    End Property
    Public Property ProperyStatNichtErfolgreich As Integer
        Get
            Return ValueStatNichtErfolgreich
        End Get
        Set(ByVal value As Integer)
            ValueStatNichtErfolgreich = value
        End Set
    End Property
    Public Property ProperyStatJournal() As Integer
        Get
            Return ValueStatJournal
        End Get
        Set(ByVal value As Integer)
            ValueStatJournal = value
        End Set
    End Property
    Public Property ProperyStatKontakt() As Integer
        Get
            Return ValueStatKontakt
        End Get
        Set(ByVal value As Integer)
            ValueStatKontakt = value
        End Set
    End Property
    Public Property ProperyStatOLClosedZeit() As Date
        Get
            Return ValueStatOLClosedZeit
        End Get
        Set(ByVal value As Date)
            ValueStatOLClosedZeit = value
        End Set
    End Property
    ' Wählbox
    Public Property ProperyTelAnschluss() As Integer
        Get
            Return ValueTelAnschluss
        End Get
        Set(ByVal value As Integer)
            ValueTelAnschluss = value
        End Set
    End Property
    Public Property ProperyTelFestnetz() As Boolean
        Get
            Return ValueTelFestnetz
        End Get
        Set(ByVal value As Boolean)
            ValueTelFestnetz = value
        End Set
    End Property
    Public Property ProperyTelCLIR() As Boolean
        Get
            Return ValueTelCLIR
        End Get
        Set(ByVal value As Boolean)
            ValueTelCLIR = value
        End Set
    End Property
    ' FritzBox
    ''' <summary>
    ''' Gibt die ermittelte Zeichencodierung der Fritzbox wieder. Der Wert wird automatisch ermittelt. 
    ''' </summary>
    ''' <value>String</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ProperyEncodeingFritzBox() As String
        Get
            Return ValueEncodeingFritzBox
        End Get
        Set(ByVal value As String)
            ValueEncodeingFritzBox = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt die eingegebene Fritz!Box IP-Adresse an. Dies ist eine Angabe, die der Nutzer in den Einstellungen ändern kann.
    ''' </summary>
    ''' <value>String</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ProperyTBFBAdr() As String
        Get
            Return ValueTBFBAdr
        End Get
        Set(ByVal value As String)
            ValueTBFBAdr = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt an, ob eine Verbindung zur Fritz!Box trotz fehlgeschlagenen Pings aufgebaut werden soll.
    ''' </summary>
    ''' <value>Boolean</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ProperyCBForceFBAddr() As Boolean
        Get
            Return ValueCBForceFBAddr
        End Get
        Set(ByVal value As Boolean)
            ValueCBForceFBAddr = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt den einegegebenen Benutzernamen für das Login der Fritz!Box an. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    ''' <value>String</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ProperyTBBenutzer() As String
        Get
            Return ValueTBBenutzer
        End Get
        Set(ByVal value As String)
            ValueTBBenutzer = value
        End Set
    End Property
    ''' <summary>
    ''' Gibt das eingegebene Passwort für das Login der Fritz!Box an. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>Das verschlüsselte Passwort</returns>
    ''' <remarks></remarks>
    Public Property ProperyTBPasswort() As String
        Get
            Return ValueTBPasswort
        End Get
        Set(ByVal value As String)
            ValueTBPasswort = value
        End Set
    End Property
    ' Indizierung
    Public Property ProperyLLetzteIndizierung() As Date
        Get
            Return ValueLLetzteIndizierung
        End Get
        Set(ByVal value As Date)
            ValueLLetzteIndizierung = value
        End Set
    End Property
    ' Note
    Public Property ProperyCBNote() As Boolean
        Get
            Return ValueCBNote
        End Get
        Set(ByVal value As Boolean)
            ValueCBNote = value
        End Set
    End Property
    ' Einstellungen
    Public Property ProperyArbeitsverzeichnis() As String
        Get
            Return ValueArbeitsverzeichnis
        End Get
        Set(value As String)
            ValueArbeitsverzeichnis = value
        End Set
    End Property
    ' Vorwahllisten
    Public Property ProperyListeOrtsVorwahlen() As String()
        Get
            Return ValueListeOrtsVorwahlen
        End Get
        Set(value As String())
            ValueListeOrtsVorwahlen = value
        End Set
    End Property
    Public Property ProperyListeLandesVorwahlen() As String()
        Get
            Return ValueListeLandesVorwahlen
        End Get
        Set(value As String())
            ValueListeLandesVorwahlen = value
        End Set
    End Property
#End Region

#Region "Global Default Value Properties"
    ''' <summary>
    ''' Default Fehlerwert
    ''' </summary>
    ''' <value>-1</value>
    ''' <returns>String</returns>
    Public ReadOnly Property Propery_Def_ErrorMinusOne_String() As String
        Get
            Return "-1"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_ErrorMinusOne_Integer() As Integer
        Get
            Return -1
        End Get
    End Property

    Public ReadOnly Property Propery_Def_ErrorMinusTwo_String() As String
        Get
            Return "-2"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_StringEmpty() As String
        Get
            Return String.Empty
        End Get
    End Property

    Public ReadOnly Property Propery_Def_NeueZeile() As String
        Get
            Return vbCrLf
        End Get
    End Property

    Public ReadOnly Property Propery_Def_StringUnknown() As String
        Get
            Return "unbekannt"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_FritzBoxAdress() As String
        Get
            Return "fritz.box"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_TelCodeActivateFritzBoxCallMonitor() As String
        Get
            Return "#96*5*"
        End Get
    End Property

    Public ReadOnly Property Propery_DefaultFBAnrMonPort() As Integer
        Get
            Return 1012
        End Get
    End Property

    Public ReadOnly Property Propery_Def_StringNull() As String
        Get
            Return "0"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_StringErrorMinusOne() As String
        Get
            Return CStr(Propery_Def_ErrorMinusOne_String)
        End Get
    End Property

    Public ReadOnly Property Propery_Def_SessionID() As String
        Get
            Return "0000000000000000"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Header_UserAgent() As String
        Get
            Return "Mozilla/5.0 (compatible; MSIE 7.0; Windows NT 6.0; WOW64; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.0.04506; Media Center PC 5.0; .NET CLR 3.5.21022; .NET CLR 1.1.4322)"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Header_ContentType() As String
        Get
            Return "application/x-www-form-urlencoded"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Header_Accept() As String
        Get
            Return "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_ReStartIntervall() As Integer
        Get
            Return 3000
        End Get
    End Property

    Public ReadOnly Property Propery_Def_TryMaxRestart() As Integer
        Get
            Return 15
        End Get
    End Property

    Public ReadOnly Property Propery_Def_AnrMonDirection_Call() As String
        Get
            Return "[->]"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_AnrMonDirection_Ring() As String
        Get
            Return "[<-]"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_AnrMonDirection_Default() As String
        Get
            Return "[<>]"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_AnrMonDirection_UserProperty_Name() As String
        Get
            Return "FBDB-AnrMonDirection"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_AnrMonDirection_UserProperty_Zeit() As String
        Get
            Return "FBDB-AnrMonZeit"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Note_Table() As String
        Get
            Return "FBDB_Note_Table"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Begin_vCard() As String
        Get
            Return "BEGIN:VCARD"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_End_vCard() As String
        Get
            Return "END:VCARD"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_NameListCALL() As String
        Get
            Return "CallList"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_NameListRING() As String
        Get
            Return "RingList"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_NameListVIP() As String
        Get
            Return "VIPList"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Addin_LangName() As String
        Get
            Return "Fritz!Box Telefon-dingsbums"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Addin_KurzName() As String
        Get
            Return "FritzOutlook"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Config_FileName() As String
        Get
            Return Propery_Def_Addin_KurzName & ".xml"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Log_FileName() As String
        Get
            Return Propery_Def_Addin_KurzName & ".log"
        End Get
    End Property

    ''' <summary>
    ''' Gibt den Zeitraum in MINUTEN an, nachdem geprüft werden soll, ob der Anrufmonitor noch aktiv ist. 
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Intervall in MINUTEN</returns>
    Public ReadOnly Property Propery_Def_CheckAnrMonIntervall() As Integer
        Get
            Return 1
        End Get
    End Property

    Public ReadOnly Property Propery_Def_DirectorySeparatorChar() As String
        Get
            Return IO.Path.DirectorySeparatorChar
        End Get
    End Property
    Public ReadOnly Property Propery_Def_AddInPath() As String
        Get
            Return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & Propery_Def_DirectorySeparatorChar & Propery_Def_Addin_LangName & Propery_Def_DirectorySeparatorChar
        End Get
    End Property

    ''' <summary>
    ''' Ein Array, welches den Namen der UserProperties, die die unformatierte Telefonnummer enthält.
    ''' </summary>
    ''' <value>String-Array</value>
    ''' <returns>String-Array</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Propery_Def_UserProperties As String()
        Get
            Dim tmp() As String = {"FBDB-AssistantTelephoneNumber", _
                                   "FBDB-BusinessTelephoneNumber", _
                                   "FBDB-Business2TelephoneNumber", _
                                   "FBDB-CallbackTelephoneNumber", _
                                   "FBDB-CarTelephoneNumber", _
                                   "FBDB-CompanyMainTelephoneNumber", _
                                   "FBDB-HomeTelephoneNumber", _
                                   "FBDB-Home2TelephoneNumber", _
                                   "FBDB-ISDNNumber", _
                                   "FBDB-MobileTelephoneNumber", _
                                   "FBDB-OtherTelephoneNumber", _
                                   "FBDB-PagerNumber", _
                                   "FBDB-PrimaryTelephoneNumber", _
                                   "FBDB-RadioTelephoneNumber", _
                                   "FBDB-BusinessFaxNumber", _
                                   "FBDB-HomeFaxNumber", _
                                   "FBDB-OtherFaxNumber", _
                                   "FBDB-Telex", _
                                   "FBDB-TTYTDDTelephoneNumber"}
            Return tmp
        End Get
    End Property

    Public ReadOnly Property Propery_Def_olTelNrTypen As String()
        Get
            Dim tmp() As String = {"Assistent", _
                                   "Geschäftlich", _
                                   "Geschäftlich 2", _
                                   "Rückmeldung", _
                                   "Auto", _
                                   "Firma", _
                                   "Privat", _
                                   "Privat 2", _
                                   "ISDN", _
                                   "Mobiltelefon", _
                                   "Weitere", _
                                   "Pager", _
                                   "Haupttelefon", _
                                   "Funkruf", _
                                   "Fax geschäftl.", _
                                   "Fax privat", _
                                   "Weiteres Fax", _
                                   "Telex", _
                                   "Texttelefon"}
            Return tmp
        End Get
    End Property

    Public ReadOnly Property Propery_Def_UserPropertyIndex() As String
        Get
            Return "FBDB-Save"
        End Get
    End Property
#Region "Journal"
    Public ReadOnly Property Propery_Def_Journal_Text_Eingehend() As String
        Get
            Return "Eingehender Anruf von"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Journal_Text_Ausgehend() As String
        Get
            Return "Ausgehender Anruf zu"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Journal_Text_Verpasst() As String
        Get
            Return "Verpasster Anruf von"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Journal_Text_NichtErfolgreich() As String
        Get
            Return "Nicht erfolgreicher Anruf zu"
        End Get
    End Property
#End Region

#Region "Phoner"
    Public ReadOnly Property Propery_Def_Phoner_CONNECT As String
        Get
            Return "CONNECT " 'Das Leerzeichen wird benötigt!
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Phoner_DISCONNECT As String
        Get
            Return "DISCONNECT"
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Phoner_Challenge As String
        Get
            Return "Challenge="
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Phoner_Response As String
        Get
            Return "Response="
        End Get
    End Property

    Public ReadOnly Property Propery_Def_Phoner_Ready As String
        Get
            Return "Welcome to Phoner"
        End Get
    End Property

    Public ReadOnly Property Propery_DefaultPhonerAnrMonPort() As Integer
        Get
            Return 2012
        End Get
    End Property
#End Region
    ' Passwortverschlüsselung
    Public ReadOnly Property Propery_Def_PassWordDecryptionKey As String
        Get
            Return "Fritz!Box Script"
        End Get
    End Property

#End Region

#Region "Default Value Properties"

    Public ReadOnly Property Propery_Def_TBLandesVW() As String
        Get
            Return "0049"
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBoxLandesVorwahl() As Integer
        Get
            Return Propery_Def_ErrorMinusOne_Integer
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TBAmt() As String
        Get
            Return Propery_Def_ErrorMinusOne_String
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TBVorwahl() As String
        Get
            Return Propery_Def_StringEmpty
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBoxVorwahl() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TBEnblDauer() As Integer
        Get
            Return 10
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBAnrMonAuto() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TBAnrMonX() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TBAnrMonY() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBAnrMonMove() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBAnrMonTransp() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TBAnrMonMoveGeschwindigkeit() As Integer
        Get
            Return 5
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBoxAnrMonStartPosition() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBoxAnrMonMoveDirection() As Integer
        Get
            Return 0
        End Get
    End Property

    Public ReadOnly Property Propery_Def_CBAnrMonZeigeKontakt() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBAnrMonContactImage() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBIndexAus() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBShowMSN() As Boolean
        Get
            Return False
        End Get
    End Property

    Public ReadOnly Property Propery_Def_CBAutoClose() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBVoIPBuster() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBCbCunterbinden() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBCallByCall() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBDialPort() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBKErstellen() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBLogFile() As Boolean
        Get
            Return True
        End Get
    End Property
    'Einstellung für die Symbolleiste
    Public ReadOnly Property Propery_Def_CBSymbWwdh() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBSymbAnrMon() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBSymbAnrMonNeuStart() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBSymbAnrListe() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBSymbDirekt() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBSymbRWSuche() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBSymbVIP() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBSymbJournalimport() As Boolean
        Get
            Return False
        End Get
    End Property

    Public ReadOnly Property Propery_Def_CBJImport() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBRWS() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TVKontaktOrdnerEntryID() As String
        Get
            Return Propery_Def_StringErrorMinusOne
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TVKontaktOrdnerStoreID() As String
        Get
            Return Propery_Def_StringErrorMinusOne
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBKHO() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBRWSIndex() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_ComboBoxRWS() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBIndex() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBJournal() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBUseAnrMon() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBCheckMobil() As Boolean
        Get
            Return True
        End Get
    End Property
    'StoppUhr
    Public ReadOnly Property Propery_Def_CBStoppUhrEinblenden() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBStoppUhrAusblenden() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TBStoppUhr() As Integer
        Get
            Return 10
        End Get

    End Property
    Public ReadOnly Property Propery_Def_CBStoppUhrX() As Integer
        Get
            Return 10
        End Get

    End Property
    Public ReadOnly Property Propery_Def_CBStoppUhrY() As Integer
        Get
            Return 10
        End Get

    End Property
    ' Telefonnummernformatierung
    Public ReadOnly Property Propery_Def_TBTelNrMaske() As String
        Get
            Return "%L (%O) %N - %D"
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBTelNrGruppieren() As Boolean
        Get
            Return True
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBintl() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBIgnoTelNrFormat() As Boolean
        Get
            Return False
        End Get
    End Property
    'Phoner
    Public ReadOnly Property Propery_Def_CBPhoner As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_PhonerVerfügbar As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBPhonerAnrMon As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_ComboBoxPhonerSIP() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TBPhonerPasswort() As String
        Get
            Return Propery_Def_StringEmpty
        End Get
    End Property
    Public ReadOnly Property Propery_Def_PhonerTelNameIndex() As Integer
        Get
            Return 0
        End Get
    End Property
    ' Statistik
    Public ReadOnly Property Propery_Def_StatResetZeit As Date
        Get
            Return System.DateTime.Now
        End Get
    End Property
    Public ReadOnly Property Propery_Def_StatVerpasst As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property Propery_Def_StatNichtErfolgreich As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property Propery_Def_StatJournal() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property Propery_Def_StatKontakt() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property Propery_Def_StatOLClosedZeit() As Date
        Get
            Return System.DateTime.Now
        End Get
    End Property
    ' Wählbox
    Public ReadOnly Property Propery_Def_TelAnschluss() As Integer
        Get
            Return 0
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TelFestnetz() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TelCLIR() As Boolean
        Get
            Return False
        End Get
    End Property
    ' FritzBox
    Public ReadOnly Property Propery_Def_EncodeingFritzBox() As String
        Get
            Return Propery_Def_ErrorMinusOne_String
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TBFBAdr() As String
        Get
            Return Propery_Def_FritzBoxAdress
        End Get
    End Property
    Public ReadOnly Property Propery_Def_CBForceFBAddr() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TBBenutzer() As String
        Get
            Return Propery_Def_StringEmpty
        End Get
    End Property
    Public ReadOnly Property Propery_Def_TBPasswort() As String
        Get
            Return Propery_Def_StringEmpty
        End Get
    End Property
    ' Indizierung
    Public ReadOnly Property Propery_Def_LLetzteIndizierung() As Date
        Get
            Return System.DateTime.Now
        End Get
    End Property
    ' Note
    Public ReadOnly Property Propery_Def_CBNote() As Boolean
        Get
            Return False
        End Get
    End Property

#End Region

#Region "Organisation Properties"
    Private ReadOnly Property Propery_Def_Options() As String
        Get
            Return "Optionen"
        End Get
    End Property
    Private ReadOnly Property Propery_Def_Statistics() As String
        Get
            Return "Statistik"
        End Get
    End Property
    Private ReadOnly Property Propery_Def_Journal() As String
        Get
            Return "Journal"
        End Get
    End Property
    Private ReadOnly Property Propery_Def_Phoner() As String
        Get
            Return "Phoner"
        End Get
    End Property

#End Region

#Region "Debug Properties"
    Public ReadOnly Property ProperyDebug_Use_WebClient() As Boolean
        Get
            Return False
        End Get
    End Property
    Public ReadOnly Property ProperyDebug_AnrufSimulation() As Boolean
        Get
            Return False
        End Get
    End Property
#End Region

    Public Sub New()
        ' Pfad zur Einstellungsdatei ermitteln
        Dim ConfigPfad As String
        ProperyArbeitsverzeichnis = GetSettingsVBA("Arbeitsverzeichnis", Propery_Def_AddInPath)
        ConfigPfad = ProperyArbeitsverzeichnis & Propery_Def_Config_FileName

        XMLDoc = New XmlDocument()
        With My.Computer.FileSystem
            If Not (.FileExists(ConfigPfad) AndAlso XMLValidator(ConfigPfad)) Then
                XMLDoc.LoadXml("<?xml version=""1.0"" encoding=""UTF-8""?><" & RootName & "/>")
                If Not .DirectoryExists(ProperyArbeitsverzeichnis) Then .CreateDirectory(ProperyArbeitsverzeichnis)
                .WriteAllText(ConfigPfad, XMLDoc.InnerXml, True)
                SaveSettingsVBA("Arbeitsverzeichnis", ProperyArbeitsverzeichnis)
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
        Dim xPathTeile As New ArrayList

        Me.ProperyTBLandesVW = Read(Propery_Def_Options, "TBLandesVW", Propery_Def_TBLandesVW)
        Me.ProperyTBAmt = Read(Propery_Def_Options, "TBAmt", Propery_Def_TBAmt)
        Me.ProperyTBFBAdr = Read(Propery_Def_Options, "TBFBAdr", Propery_Def_TBFBAdr)
        Me.ProperyCBForceFBAddr = CBool(Read(Propery_Def_Options, "CBForceFBAddr", CStr(Propery_Def_CBForceFBAddr)))
        Me.ProperyTBBenutzer = Read(Propery_Def_Options, "TBBenutzer", Propery_Def_TBBenutzer)
        Me.ProperyTBPasswort = Read(Propery_Def_Options, "TBPasswort", Propery_Def_TBPasswort)
        Me.ProperyTBVorwahl = Read(Propery_Def_Options, "TBVorwahl", Propery_Def_TBVorwahl)
        Me.ProperyCBoxVorwahl = CInt(Read(Propery_Def_Options, "CBoxVorwahl", CStr(Propery_Def_CBoxVorwahl)))
        Me.ProperyTBEnblDauer = CInt(Read(Propery_Def_Options, "TBEnblDauer", CStr(Propery_Def_TBEnblDauer)))
        Me.ProperyCBAnrMonAuto = CBool(Read(Propery_Def_Options, "CBAnrMonAuto", CStr(Propery_Def_CBAnrMonAuto)))
        Me.ProperyTBAnrMonX = CInt(Read(Propery_Def_Options, "TBAnrMonX", CStr(Propery_Def_TBAnrMonX)))
        Me.ProperyTBAnrMonY = CInt(Read(Propery_Def_Options, "TBAnrMonY", CStr(Propery_Def_TBAnrMonY)))
        Me.ProperyCBAnrMonMove = CBool(Read(Propery_Def_Options, "CBAnrMonMove", CStr(Propery_Def_CBAnrMonMove)))
        Me.ProperyCBAnrMonTransp = CBool(Read(Propery_Def_Options, "CBAnrMonTransp", CStr(Propery_Def_CBAnrMonTransp)))
        Me.ProperyTBAnrMonMoveGeschwindigkeit = CInt(Read(Propery_Def_Options, "TBAnrMonMoveGeschwindigkeit", CStr(Propery_Def_TBAnrMonMoveGeschwindigkeit)))
        Me.ProperyCBoxAnrMonStartPosition = CInt(Read(Propery_Def_Options, "CBoxAnrMonStartPosition", CStr(Propery_Def_CBoxAnrMonStartPosition)))
        Me.ProperyCBoxAnrMonMoveDirection = CInt(Read(Propery_Def_Options, "CBoxAnrMonMoveDirection", CStr(Propery_Def_CBoxAnrMonMoveDirection)))
        Me.ProperyCBAnrMonZeigeKontakt = CBool(Read(Propery_Def_Options, "CBAnrMonZeigeKontakt", CStr(Propery_Def_CBAnrMonZeigeKontakt)))
        Me.ProperyCBAnrMonContactImage = CBool(Read(Propery_Def_Options, "CBAnrMonContactImage", CStr(Propery_Def_CBAnrMonContactImage)))
        Me.ProperyCBIndexAus = CBool(Read(Propery_Def_Options, "CBIndexAus", CStr(Propery_Def_CBIndexAus)))
        Me.ProperyCBShowMSN = CBool(Read(Propery_Def_Options, "CBShowMSN", CStr(Propery_Def_CBShowMSN)))
        Me.ProperyCBJournal = CBool(Read(Propery_Def_Options, "CBJournal", CStr(Propery_Def_CBJournal)))
        Me.ProperyCBUseAnrMon = CBool(Read(Propery_Def_Options, "CBUseAnrMon", CStr(Propery_Def_CBUseAnrMon)))
        Me.ProperyCBCheckMobil = CBool(Read(Propery_Def_Options, "CBCheckMobil", CStr(Propery_Def_CBCheckMobil)))
        Me.ProperyCBAutoClose = CBool(Read(Propery_Def_Options, "CBAutoClose", CStr(Propery_Def_CBAutoClose)))
        Me.ProperyCBVoIPBuster = CBool(Read(Propery_Def_Options, "CBVoIPBuster", CStr(Propery_Def_CBVoIPBuster)))
        Me.ProperyCBCbCunterbinden = CBool(Read(Propery_Def_Options, "CBCbCunterbinden", CStr(Propery_Def_CBCbCunterbinden)))
        Me.ProperyCBCallByCall = CBool(Read(Propery_Def_Options, "CBCallByCall", CStr(Propery_Def_CBCallByCall)))
        Me.ProperyCBDialPort = CBool(Read(Propery_Def_Options, "CBDialPort", CStr(Propery_Def_CBDialPort)))
        Me.ProperyCBKErstellen = CBool(Read(Propery_Def_Options, "CBKErstellen", CStr(Propery_Def_CBKErstellen)))
        Me.ProperyCBLogFile = CBool(Read(Propery_Def_Options, "CBLogFile", CStr(Propery_Def_CBLogFile)))
        ' Einstellungen für die Symbolleiste laden
        Me.ProperyCBSymbWwdh = CBool(Read(Propery_Def_Options, "CBSymbWwdh", CStr(Propery_Def_CBSymbWwdh)))
        Me.ProperyCBSymbAnrMon = CBool(Read(Propery_Def_Options, "CBSymbAnrMon", CStr(Propery_Def_CBSymbAnrMon)))
        Me.ProperyCBSymbAnrMonNeuStart = CBool(Read(Propery_Def_Options, "CBSymbAnrMonNeuStart", CStr(Propery_Def_CBSymbAnrMonNeuStart)))
        Me.ProperyCBSymbAnrListe = CBool(Read(Propery_Def_Options, "CBSymbAnrListe", CStr(Propery_Def_CBSymbAnrListe)))
        Me.ProperyCBSymbDirekt = CBool(Read(Propery_Def_Options, "CBSymbDirekt", CStr(Propery_Def_CBSymbDirekt)))
        Me.ProperyCBSymbRWSuche = CBool(Read(Propery_Def_Options, "CBSymbRWSuche", CStr(Propery_Def_CBSymbRWSuche)))
        Me.ProperyTVKontaktOrdnerEntryID = Read(Propery_Def_Options, "TVKontaktOrdnerEntryID", CStr(Propery_Def_TVKontaktOrdnerEntryID))
        Me.ProperyTVKontaktOrdnerStoreID = Read(Propery_Def_Options, "TVKontaktOrdnerStoreID", CStr(Propery_Def_TVKontaktOrdnerStoreID))
        Me.ProperyCBSymbVIP = CBool(Read(Propery_Def_Options, "CBSymbVIP", CStr(Propery_Def_CBSymbVIP)))
        Me.ProperyCBSymbJournalimport = CBool(Read(Propery_Def_Options, "CBSymbJournalimport", CStr(Propery_Def_CBSymbJournalimport)))
        Me.ProperyCBJImport = CBool(Read(Propery_Def_Options, "CBJImport", CStr(Propery_Def_CBJImport)))
        ' Einstellungen füer die Rückwärtssuche laden
        Me.ProperyCBKHO = CBool(Read(Propery_Def_Options, "CBKHO", CStr(Propery_Def_CBKHO)))
        Me.ProperyCBRWS = CBool(Read(Propery_Def_Options, "CBRWS", CStr(Propery_Def_CBRWS)))
        Me.ProperyCBRWSIndex = CBool(Read(Propery_Def_Options, "CBRWSIndex", CStr(Propery_Def_CBRWSIndex)))
        Me.ProperyComboBoxRWS = CInt(Read(Propery_Def_Options, "ComboBoxRWS", CStr(Propery_Def_ComboBoxRWS)))
        Me.ProperyCBIndex = CBool(Read(Propery_Def_Options, "CBIndex", CStr(Propery_Def_CBIndex)))
        ' StoppUhr
        Me.ProperyCBStoppUhrEinblenden = CBool(Read(Propery_Def_Options, "CBStoppUhrEinblenden", CStr(Propery_Def_CBStoppUhrEinblenden)))
        Me.ProperyCBStoppUhrAusblenden = CBool(Read(Propery_Def_Options, "CBStoppUhrAusblenden", CStr(Propery_Def_CBStoppUhrAusblenden)))
        Me.ProperyTBStoppUhr = CInt(Read(Propery_Def_Options, "TBStoppUhr", CStr(Propery_Def_TBStoppUhr)))
        Me.ProperyCBStoppUhrX = CInt(Read(Propery_Def_Options, "CBStoppUhrX", CStr(Propery_Def_CBStoppUhrX)))
        Me.ProperyCBStoppUhrY = CInt(Read(Propery_Def_Options, "CBStoppUhrY", CStr(Propery_Def_CBStoppUhrY)))
        ' Telefonnummernformatierung
        Me.ProperyTBTelNrMaske = Read(Propery_Def_Options, "TBTelNrMaske", Propery_Def_TBTelNrMaske)
        Me.ProperyCBTelNrGruppieren = CBool(Read(Propery_Def_Options, "CBTelNrGruppieren", CStr(Propery_Def_CBTelNrGruppieren)))
        Me.ProperyCBintl = CBool(Read(Propery_Def_Options, "CBintl", CStr(Propery_Def_CBintl)))
        Me.ProperyCBIgnoTelNrFormat = CBool(Read(Propery_Def_Options, "CBIgnoTelNrFormat", CStr(Propery_Def_CBIgnoTelNrFormat)))
        ' Phoner
        Me.ProperyCBPhoner = CBool(Read(Propery_Def_Phoner, "CBPhoner", CStr(Propery_Def_CBPhoner)))
        Me.ProperyPhonerVerfügbar = CBool(Read(Propery_Def_Phoner, "PhonerVerfügbar", CStr(Propery_Def_PhonerVerfügbar)))
        Me.ProperyComboBoxPhonerSIP = CInt(Read(Propery_Def_Phoner, "ComboBoxPhonerSIP", CStr(Propery_Def_ComboBoxPhonerSIP)))
        Me.ProperyCBPhonerAnrMon = CBool(Read(Propery_Def_Phoner, "CBPhonerAnrMon", CStr(Propery_Def_CBPhonerAnrMon)))
        Me.ProperyTBPhonerPasswort = Read(Propery_Def_Phoner, "TBPhonerPasswort", Propery_Def_TBPhonerPasswort)
        Me.ProperyPhonerTelNameIndex = CInt(Read(Propery_Def_Phoner, "PhonerTelNameIndex", CStr(Propery_Def_PhonerTelNameIndex)))
        ' Statistik
        Me.ProperyStatResetZeit = CDate(Read(Propery_Def_Statistics, "ResetZeit", CStr(Propery_Def_StatResetZeit)))
        Me.ProperyStatVerpasst = CInt(Read(Propery_Def_Statistics, "Verpasst", CStr(Propery_Def_StatVerpasst)))
        Me.ProperyStatNichtErfolgreich = CInt(Read(Propery_Def_Statistics, "Nichterfolgreich", CStr(Propery_Def_StatNichtErfolgreich)))
        Me.ProperyStatKontakt = CInt(Read(Propery_Def_Statistics, "Kontakt", CStr(Propery_Def_StatKontakt)))
        Me.ProperyStatJournal = CInt(Read(Propery_Def_Statistics, "Journal", CStr(Propery_Def_StatJournal)))
        Me.ProperyStatOLClosedZeit = CDate(Read(Propery_Def_Journal, "SchließZeit", CStr(Propery_Def_StatOLClosedZeit)))
        ' Wählbox
        Me.ProperyTelAnschluss = CInt(Read(Propery_Def_Options, "Anschluss", CStr(Propery_Def_TelAnschluss)))
        Me.ProperyTelFestnetz = CBool(Read(Propery_Def_Options, "Festnetz", CStr(ProperyTelFestnetz)))
        Me.ProperyTelCLIR = CBool(Read(Propery_Def_Options, "CLIR", CStr(Propery_Def_TelCLIR)))
        Me.ProperyEncodeingFritzBox = Read(Propery_Def_Options, "EncodeingFritzBox", Propery_Def_EncodeingFritzBox)
        ' Indizierung
        Me.ProperyLLetzteIndizierung = CDate(Read(Propery_Def_Options, "LLetzteIndizierung", CStr(Propery_Def_LLetzteIndizierung)))
        ' Notiz
        Me.ProperyCBNote = CBool(Read(Propery_Def_Options, "CBNote", CStr(Propery_Def_CBNote)))

        With xPathTeile
            .Clear()
            .Add("Telefone")
            .Add("Nummern")
            .Add("*")
            .Add("[@Checked=""1""]")
        End With
        Me.ProperyCLBTelNr = (From x In Split(Read(xPathTeile, Me.Propery_Def_ErrorMinusOne_String), ";", , CompareMethod.Text) Select x Distinct).ToArray
    End Sub

    Private Sub SaveOptionData()
        Write(Propery_Def_Options, "TBLandesVW", Me.ProperyTBLandesVW)
        Write(Propery_Def_Options, "TBAmt", Me.ProperyTBAmt)
        Write(Propery_Def_Options, "TBFBAdr", Me.ProperyTBFBAdr)
        Write(Propery_Def_Options, "CBForceFBAddr", CStr(Me.ProperyCBForceFBAddr))
        Write(Propery_Def_Options, "TBBenutzer", Me.ProperyTBBenutzer)
        Write(Propery_Def_Options, "TBPasswort", Me.ProperyTBPasswort)
        Write(Propery_Def_Options, "TBVorwahl", Me.ProperyTBVorwahl)
        Write(Propery_Def_Options, "CBoxVorwahl", CStr(Me.ProperyCBoxVorwahl))
        Write(Propery_Def_Options, "TBEnblDauer", CStr(Me.ProperyTBEnblDauer))
        Write(Propery_Def_Options, "CBAnrMonAuto", CStr(Me.ProperyCBAnrMonAuto))
        Write(Propery_Def_Options, "TBAnrMonX", CStr(Me.ProperyTBAnrMonX))
        Write(Propery_Def_Options, "TBAnrMonY", CStr(Me.ProperyTBAnrMonY))
        Write(Propery_Def_Options, "CBAnrMonMove", CStr(Me.ProperyCBAnrMonMove))
        Write(Propery_Def_Options, "CBAnrMonTransp", CStr(Me.ProperyCBAnrMonTransp))
        Write(Propery_Def_Options, "TBAnrMonMoveGeschwindigkeit", CStr(Me.ProperyTBAnrMonMoveGeschwindigkeit))
        Write(Propery_Def_Options, "CBoxAnrMonStartPosition", CStr(Me.ProperyCBoxAnrMonStartPosition))
        Write(Propery_Def_Options, "CBoxAnrMonMoveDirection", CStr(Me.ProperyCBoxAnrMonMoveDirection))
        Write(Propery_Def_Options, "CBAnrMonZeigeKontakt", CStr(Me.ProperyCBAnrMonZeigeKontakt))
        Write(Propery_Def_Options, "CBAnrMonContactImage", CStr(Me.ProperyCBAnrMonContactImage))
        Write(Propery_Def_Options, "CBIndexAus", CStr(Me.ProperyCBIndexAus))
        Write(Propery_Def_Options, "CBShowMSN", CStr(Me.ProperyCBShowMSN))
        Write(Propery_Def_Options, "CBAutoClose", CStr(Me.ProperyCBAutoClose))
        Write(Propery_Def_Options, "CBVoIPBuster", CStr(Me.ProperyCBVoIPBuster))
        Write(Propery_Def_Options, "CBCbCunterbinden", CStr(Me.ProperyCBVoIPBuster))
        Write(Propery_Def_Options, "CBCallByCall", CStr(Me.ProperyCBCallByCall))
        Write(Propery_Def_Options, "CBDialPort", CStr(Me.ProperyCBDialPort))
        Write(Propery_Def_Options, "CBKErstellen", CStr(Me.ProperyCBKErstellen))
        Write(Propery_Def_Options, "CBLogFile", CStr(Me.ProperyCBLogFile))
        ' Einstellungen für die Symbolleiste laden
        Write(Propery_Def_Options, "CBSymbWwdh", CStr(Me.ProperyCBSymbWwdh))
        Write(Propery_Def_Options, "CBSymbAnrMon", CStr(Me.ProperyCBSymbAnrMon))
        Write(Propery_Def_Options, "CBSymbAnrMonNeuStart", CStr(Me.ProperyCBSymbAnrMonNeuStart))
        Write(Propery_Def_Options, "CBSymbAnrListe", CStr(Me.ProperyCBSymbAnrListe))
        Write(Propery_Def_Options, "CBSymbDirekt", CStr(Me.ProperyCBSymbDirekt))
        Write(Propery_Def_Options, "CBSymbRWSuche", CStr(Me.ProperyCBSymbRWSuche))
        Write(Propery_Def_Options, "CBSymbVIP", CStr(Me.ProperyCBSymbVIP))
        Write(Propery_Def_Options, "CBSymbJournalimport", CStr(Me.ProperyCBSymbJournalimport))
        Write(Propery_Def_Options, "CBJImport", CStr(Me.ProperyCBJImport))
        ' Einstellungen füer die Rückwärtssuche laden
        Write(Propery_Def_Options, "CBKHO", CStr(Me.ProperyCBKHO))
        Write(Propery_Def_Options, "CBRWS", CStr(Me.ProperyCBRWS))
        Write(Propery_Def_Options, "CBRWSIndex", CStr(Me.ProperyCBRWSIndex))
        Write(Propery_Def_Options, "TVKontaktOrdnerEntryID", CStr(Me.ProperyTVKontaktOrdnerEntryID))
        Write(Propery_Def_Options, "TVKontaktOrdnerStoreID", CStr(Me.ProperyTVKontaktOrdnerStoreID))
        Write(Propery_Def_Options, "ComboBoxRWS", CStr(Me.ProperyComboBoxRWS))
        Write(Propery_Def_Options, "CBIndex", CStr(Me.ProperyCBIndex))
        Write(Propery_Def_Options, "CBJournal", CStr(Me.ProperyCBJournal))
        Write(Propery_Def_Options, "CBUseAnrMon", CStr(Me.ProperyCBUseAnrMon))
        Write(Propery_Def_Options, "CBCheckMobil", CStr(Me.ProperyCBCheckMobil))
        'StoppUhr
        Write(Propery_Def_Options, "CBStoppUhrEinblenden", CStr(Me.ProperyCBStoppUhrEinblenden))
        Write(Propery_Def_Options, "CBStoppUhrAusblenden", CStr(Me.ProperyCBStoppUhrAusblenden))
        Write(Propery_Def_Options, "TBStoppUhr", CStr(Me.ProperyTBStoppUhr))
        Write(Propery_Def_Options, "TBTelNrMaske", Me.ProperyTBTelNrMaske)
        Write(Propery_Def_Options, "CBTelNrGruppieren", CStr(Me.ProperyCBTelNrGruppieren))
        Write(Propery_Def_Options, "CBintl", CStr(Me.ProperyCBintl))
        Write(Propery_Def_Options, "CBIgnoTelNrFormat", CStr(Me.ProperyCBIgnoTelNrFormat))
        Write(Propery_Def_Options, "CBStoppUhrX", CStr(Me.ProperyCBStoppUhrX))
        Write(Propery_Def_Options, "CBStoppUhrY", CStr(Me.ProperyCBStoppUhrY))
        ' Phoner
        Write(Propery_Def_Phoner, "CBPhoner", CStr(Me.ProperyCBPhoner))
        Write(Propery_Def_Phoner, "PhonerVerfügbar", CStr(Me.ProperyPhonerVerfügbar))
        Write(Propery_Def_Phoner, "ComboBoxPhonerSIP", CStr(Me.ProperyComboBoxPhonerSIP))
        Write(Propery_Def_Phoner, "CBPhonerAnrMon", CStr(Me.ProperyCBPhonerAnrMon))
        Write(Propery_Def_Phoner, "TBPhonerPasswort", Me.ProperyTBPhonerPasswort)
        Write(Propery_Def_Phoner, "PhonerTelNameIndex", CStr(Me.ProperyPhonerTelNameIndex))
        ' Statistik
        Write(Propery_Def_Statistics, "ResetZeit", CStr(Me.ProperyStatResetZeit))
        Write(Propery_Def_Statistics, "Verpasst", CStr(Me.ProperyStatVerpasst))
        Write(Propery_Def_Statistics, "Nichterfolgreich", CStr(Me.ProperyStatNichtErfolgreich))
        Write(Propery_Def_Statistics, "Kontakt", CStr(Me.ProperyStatKontakt))
        Write(Propery_Def_Statistics, "Journal", CStr(Me.ProperyStatJournal))
        Write(Propery_Def_Journal, "SchließZeit", CStr(Me.ProperyStatOLClosedZeit))
        ' Wählbox
        Write(Propery_Def_Options, "Anschluss", CStr(Me.ProperyTelAnschluss))
        Write(Propery_Def_Options, "Festnetz", CStr(Me.ProperyTelFestnetz))
        Write(Propery_Def_Options, "CLIR", CStr(Me.ProperyTelCLIR))
        'FritzBox
        Write(Propery_Def_Options, "EncodeingFritzBox", Me.ProperyEncodeingFritzBox)
        'Indizierung
        Write(Propery_Def_Options, "LLetzteIndizierung", CStr(Me.ProperyLLetzteIndizierung))
        ' Notiz
        Write(Propery_Def_Options, "CBNote", CStr(Me.ProperyCBNote))

        XMLDoc.Save(ProperyArbeitsverzeichnis & Propery_Def_Config_FileName)
        SaveSettingsVBA("Arbeitsverzeichnis", ProperyArbeitsverzeichnis)

        BWCBox = New BackgroundWorker
        With BWCBox
            .WorkerReportsProgress = False
            .RunWorkerAsync(True)
        End With

    End Sub

    Protected Overrides Sub Finalize()
        SaveOptionData()
        XMLDoc.Save(ProperyArbeitsverzeichnis & Propery_Def_Config_FileName)
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
                Read = Propery_Def_StringEmpty
                For Each tmpXMLNode As XmlNode In tmpXMLNodeList
                    Read += tmpXMLNode.InnerText & ";"
                Next
                Read = Left(Read, Len(Read) - 1)
            End If
        End If
        xPathTeile = Nothing
    End Function

    Public Sub GetProperXPath(ByRef xPathTeile As ArrayList)
        Dim i As Integer = 1

        Dim xPath As String
        Dim tmpXMLNode As XmlNode
        Dim tmpParentXMLNode As XmlNode

        xPath = CreateXPath(xPathTeile)

        tmpXMLNode = XMLDoc.SelectSingleNode(xPath)
        If Not tmpXMLNode Is Nothing Then
            tmpParentXMLNode = tmpXMLNode.ParentNode
            Do Until tmpParentXMLNode.Name = xPathTeile.Item(1).ToString
                If Not Left(xPathTeile.Item(xPathTeile.Count - i).ToString, 1) = xPathBracketOpen Then
                    xPathTeile.Item(xPathTeile.Count - 1 - i) = tmpParentXMLNode.Name
                    tmpParentXMLNode = tmpParentXMLNode.ParentNode
                End If
                i += 1
            Loop
        End If
    End Sub

    Function ReadElementName(ByVal xPathTeile As ArrayList, ByVal sDefault As String) As String
        ReadElementName = sDefault
        Dim xPath As String
        Dim tmpXMLNode As XmlNode
        xPath = CreateXPath(xPathTeile)
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
        Return Write(ZielKnoten, Value, Propery_Def_StringEmpty, Propery_Def_StringEmpty)
    End Function

    Public Overloads Function Write(ByVal ZielKnoten As ArrayList, ByVal Value As String, ByVal AttributeName As String, ByVal AttributeValue As String) As Boolean
        Dim xPathTeile As New ArrayList
        Dim sTmpXPath As String = Propery_Def_StringEmpty
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
                        If Not AttributeName = Propery_Def_StringEmpty Then
                            If Not (tmpXMLNode.ChildNodes.Count = 0 And tmpXMLNode.Value = Nothing) Then
                                tmpXMLNode = .SelectSingleNode(xPath & CStr(IIf(Not AttributeName = Propery_Def_StringEmpty, "[@" & AttributeName & "=""" & AttributeValue & """]", Propery_Def_StringEmpty)))
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
        Dim tmpXMLNodeList As XmlNodeList

        Dim xPath As String = CreateXPath(alxPathTeile)
        With XMLDoc
            tmpXMLNodeList = .SelectNodes(xPath)
            For Each tmpXMLNode As XmlNode In tmpXMLNodeList
                If Not tmpXMLNode Is Nothing Then
                    tmpXMLNode = .SelectSingleNode(xPath).ParentNode
                    tmpXMLNode.RemoveChild(.SelectSingleNode(xPath))
                    If tmpXMLNode.ChildNodes.Count = 0 Then
                        tmpXMLNode.ParentNode.RemoveChild(tmpXMLNode)
                    End If
                End If
            Next
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
            tmpXMLNode = XMLDoc.CreateNode(XmlNodeType.Element, NodeName, Propery_Def_StringEmpty)
            With tmpXMLNode
                For i As Integer = 0 To SubNodeName.Count - 1
                    If Not SubNodeValue.Item(i).ToString = Propery_Def_ErrorMinusOne_String Then
                        tmpXMLChildNode = XMLDoc.CreateNode(XmlNodeType.Element, SubNodeName.Item(i).ToString, Propery_Def_StringEmpty)
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
                If Not AttributeValue = Propery_Def_StringEmpty And Not AttributeName = Propery_Def_StringEmpty Then alxPathTeile.Add("[@" & AttributeName & "=""" & AttributeValue & """]")
                xPath = CreateXPath(alxPathTeile)
                If Not AttributeValue = Propery_Def_StringEmpty Then alxPathTeile.RemoveAt(alxPathTeile.Count - 1)
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
        Dim tmpxPath As String = Propery_Def_StringEmpty
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
#Region "Validator"
    ''' <summary>
    ''' Prüft ob die XML-Datei geöffnet werden kann.
    ''' </summary>
    ''' <param name="XMLpath"></param>
    ''' <returns><c>True</c>, wenn Datei geöffnet werden kann, ansonsten <c>False</c>.</returns>
    ''' <remarks></remarks>
    Private Function XMLValidator(ByVal XMLpath As String) As Boolean
        XMLValidator = True
        Try
            XMLDoc.Load(XMLpath)
        Catch
            XMLValidator = False
        End Try
    End Function
#End Region
#End Region

#Region "Registry VBA GetSettings SetSettings"
    Public Function GetSettingsVBA(ByVal Key As String, ByVal DefaultValue As String) As String
        Return GetSetting(Propery_Def_Addin_KurzName, Propery_Def_Options, Key, DefaultValue)
    End Function
    Public Sub SaveSettingsVBA(ByVal Key As String, ByVal DefaultValue As String)
        SaveSetting(Propery_Def_Addin_KurzName, Propery_Def_Options, Key, DefaultValue)
    End Sub
#End Region

#Region "Stuff"
    Private Sub CleanUpXML()
        Dim tmpNode As XmlNode
        Dim xPathTeile As New ArrayList
        Dim xPath As String

        With XMLDoc
            ' Diverse Knoten des Journals löschen
            xPathTeile.Add(Propery_Def_Journal)
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

#Region "Backgroundworker"
    Private Sub BWCBbox_DoWork(sender As Object, e As DoWorkEventArgs) Handles BWCBox.DoWork
        Dim Vorwahliste As String
        Dim i As Integer
        Dim tmpVorwahl As String = ProperyTBLandesVW

        If ProperyListeLandesVorwahlen Is Nothing Then
            ' Landesvorwahlen
            Vorwahliste = Replace(My.Resources.Liste_Landesvorwahlen, ";" & vbNewLine, ")" & vbNewLine, , , CompareMethod.Text)
            Vorwahliste = Replace(Vorwahliste, ";", " (", , , CompareMethod.Text)

            ProperyListeLandesVorwahlen = (From s In Split(Vorwahliste, vbNewLine, , CompareMethod.Text) Where s.ToLower Like "00*" Select s).ToArray
        End If

        tmpVorwahl = CStr(IIf(tmpVorwahl = Propery_Def_StringEmpty, ProperyTBLandesVW, tmpVorwahl))

        If ProperyTBLandesVW = Propery_Def_TBLandesVW Then
            ' Ortsvorwahlen Deutschland
            Vorwahliste = Replace(My.Resources.Liste_Ortsvorwahlen_Deutschland, ";" & vbNewLine, ")" & vbNewLine, , , CompareMethod.Text)
            Vorwahliste = Replace(Vorwahliste, ";", " (", , , CompareMethod.Text)

            ProperyListeOrtsVorwahlen = (From s In Split(Vorwahliste, vbNewLine, , CompareMethod.Text) Where s.ToLower Like "0*" Select s).ToArray
        Else
            tmpVorwahl = Strings.Replace(tmpVorwahl, "00", "", , 1, CompareMethod.Text)

            Vorwahliste = Replace(My.Resources.Liste_Ortsvorwahlen_Ausland, ";" & vbNewLine, ")" & vbNewLine, , , CompareMethod.Text)
            Dim tmpvw() As String
            ProperyListeOrtsVorwahlen = (From s In Split(Vorwahliste, vbNewLine, , CompareMethod.Text) Where s.ToLower Like tmpVorwahl & ";*" Select s).ToArray
            For i = LBound(ProperyListeOrtsVorwahlen) To UBound(ProperyListeOrtsVorwahlen)
                tmpvw = Split(ProperyListeOrtsVorwahlen(i), ";", , CompareMethod.Text)
                ProperyListeOrtsVorwahlen(i) = tmpvw(1) & " (" & tmpvw(2)
            Next
        End If
    End Sub


    Private Sub BWCBbox_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BWCBox.RunWorkerCompleted
        BWCBox = Nothing
    End Sub
#End Region
End Class

