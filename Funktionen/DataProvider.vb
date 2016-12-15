﻿Imports System.Xml
Imports System.Timers
Imports System.ComponentModel
Imports System.Collections.ObjectModel

Public Class DataProvider

    Private P_xMLDoc As XmlDocument
    Private C_XML As XML
    Private WithEvents tSpeichern As Timer

#Region "BackgroundWorker"
    Private WithEvents BWCBox As BackgroundWorker
#End Region

#Region "Windows Const für Office 2003"
#If oVer = 11 Then
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
    ''' <summary>
    ''' Intervall (in Minuten), in dem die XML-Datei gespeichert wird.
    ''' </summary>
    ''' <value>Double</value>
    ''' <returns>5</returns>
    Private Shared ReadOnly Property P_SpeicherIntervall() As Double
        Get
            Return 5.0
        End Get
    End Property

    ''' <summary>
    ''' Name des Wurzelknotens der XML-Datei: "FritzOutlookXML"
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>FritzOutlookXML</returns>
    Private Shared ReadOnly Property P_RootName() As String
        Get
            Return "FritzOutlookXML"
        End Get
    End Property

    ' ''' <summary>
    ' ''' xPath Steuerzeichen: Seperator /
    ' ''' </summary>
    ' ''' <value>String</value>
    ' ''' <returns>/</returns>
    'Private Shared ReadOnly Property P_xPathSeperatorSlash() As String
    '    Get
    '        Return "/"
    '    End Get
    'End Property

    ' ''' <summary>
    ' ''' xPath Steuerzeichen: WildCard *
    ' ''' </summary>
    ' ''' <value>String</value>
    ' ''' <returns>*</returns>
    'Private Shared ReadOnly Property P_xPathWildCard() As String
    '    Get
    '        Return "*"
    '    End Get
    'End Property

    ' ''' <summary>
    ' ''' xPath Steuerzeichen: Öffnende eckige Klammer [
    ' ''' </summary>
    ' ''' <value>String</value>
    ' ''' <returns>[</returns>
    'Private Shared ReadOnly Property P_xPathBracketOpen() As String
    '    Get
    '        Return "["
    '    End Get
    'End Property

    ' ''' <summary>
    ' ''' xPath Steuerzeichen: Schließende eckige Klammer ]
    ' ''' </summary>
    ' ''' <value>String</value>
    ' ''' <returns>]</returns>
    'Private Shared ReadOnly Property P_xPathBracketClose() As String
    '    Get
    '        Return "]"
    '    End Get
    'End Property

    ' ''' <summary>
    ' ''' xPath Steuerzeichen: @
    ' ''' </summary>
    ' ''' <value>String</value>
    ' ''' <returns>@</returns>
    'Private Shared ReadOnly Property P_xPathAttribute() As String
    '    Get
    '        Return "@"
    '    End Get
    'End Property
#End Region

#Region "Value Properties"

    ''' <summary>
    ''' Gibt die im Einstellungsdialog eingegebene Landesvorwahl zurück
    ''' </summary>
    Private _TBLandesVW As String
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
    Private _TBAmt As String
    Public Property P_TBAmt As String
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
    Private _TBVorwahl As String
    Public Property P_TBVorwahl() As String
        Get
            Return _TBVorwahl
        End Get
        Set(ByVal value As String)
            _TBVorwahl = value
        End Set
    End Property

    ''' <summary>
    ''' Enthält den Index im Combobox
    ''' </summary>
    Private _CBoxVorwahl As Integer
    Public Property P_CBoxVorwahl() As Integer
        Get
            Return _CBoxVorwahl
        End Get
        Set(ByVal value As Integer)
            _CBoxVorwahl = value
        End Set
    End Property

    Private _TBNumEntryList As Integer
    Public Property P_TBNumEntryList As Integer
        Get
            Return _TBNumEntryList
        End Get
        Set(ByVal value As Integer)
            _TBNumEntryList = value
        End Set
    End Property

    ' Anrufmonitor

    ''' <summary>
    ''' Gibt an, wie lange der Anrufmonitor angezeigt werden soll, bevor er automatisch ausgeblendet wird
    ''' </summary>
    Private _TBEnblDauer As Integer
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
    Private _CBAnrMonAuto As Boolean
    Public Property P_CBAnrMonAuto() As Boolean
        Get
            Return _CBAnrMonAuto
        End Get
        Set(ByVal value As Boolean)
            _CBAnrMonAuto = value
        End Set
    End Property

    ''' <summary>
    ''' Gibt das Timeout an, ab dem alle Telefonate als verpasst behandelt werden sollen.
    ''' </summary>
    Private _TBAnrBeantworterTimeout As Double
    Public Property P_TBAnrBeantworterTimeout() As Double
        Get
            Return _TBAnrBeantworterTimeout
        End Get
        Set(ByVal value As Double)
            _TBAnrBeantworterTimeout = value
        End Set
    End Property

    ''' <summary>
    ''' Gibt an, um wieviele Punkte der Anrufmonitor in X-Richtung verschoben werden soll.
    ''' </summary>
    Private _TBAnrMonX As Integer
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
    Private _TBAnrMonY As Integer
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
    Private _CBAnrMonMove As Boolean
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
    Private _CBAnrMonTransp As Boolean
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
    ''' <remarks>FritzBoxDial.PopUpAnrMon.eStartPosition</remarks>
    Private _CBoxAnrMonStartPosition As Integer
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
    ''' <remarks>FritzBoxDial.PopUpAnrMon.eMoveDirection</remarks>
    Private _CBoxAnrMonMoveDirection As Integer
    Public Property P_CBoxAnrMonMoveDirection() As Integer
        Get
            Return _CBoxAnrMonMoveDirection
        End Get
        Set(ByVal value As Integer)
            _CBoxAnrMonMoveDirection = value
        End Set
    End Property

    Private _TBAnrMonMoveGeschwindigkeit As Integer
    Public Property P_TBAnrMonMoveGeschwindigkeit() As Integer
        Get
            Return _TBAnrMonMoveGeschwindigkeit
        End Get
        Set(ByVal value As Integer)
            _TBAnrMonMoveGeschwindigkeit = value
        End Set
    End Property

    Private _CBAnrMonZeigeKontakt As Boolean
    Public Property P_CBAnrMonZeigeKontakt() As Boolean
        Get
            Return _CBAnrMonZeigeKontakt
        End Get
        Set(ByVal value As Boolean)
            _CBAnrMonZeigeKontakt = value
        End Set
    End Property

    Private _CBAnrMonContactImage As Boolean
    Public Property P_CBAnrMonContactImage As Boolean
        Get
            Return _CBAnrMonContactImage
        End Get
        Set(ByVal value As Boolean)
            _CBAnrMonContactImage = value
        End Set
    End Property

    Private _CBIndexAus As Boolean
    Public Property P_CBIndexAus As Boolean
        Get
            Return _CBIndexAus
        End Get
        Set(ByVal value As Boolean)
            _CBIndexAus = value
        End Set
    End Property

    Private _CBShowMSN As Boolean
    Public Property P_CBShowMSN As Boolean
        Get
            Return _CBShowMSN
        End Get
        Set(ByVal value As Boolean)
            _CBShowMSN = value
        End Set
    End Property

    Private _CBAutoClose As Boolean
    Public Property P_CBAutoClose As Boolean
        Get
            Return _CBAutoClose
        End Get
        Set(ByVal value As Boolean)
            _CBAutoClose = value
        End Set
    End Property

    Private _CBAnrMonCloseAtDISSCONNECT As Boolean
    Public Property P_CBAnrMonCloseAtDISSCONNECT As Boolean
        Get
            Return _CBAnrMonCloseAtDISSCONNECT
        End Get
        Set(ByVal value As Boolean)
            _CBAnrMonCloseAtDISSCONNECT = value
        End Set
    End Property

    Private _CBVoIPBuster As Boolean
    Public Property P_CBVoIPBuster As Boolean
        Get
            Return _CBVoIPBuster
        End Get
        Set(ByVal value As Boolean)
            _CBVoIPBuster = value
        End Set
    End Property

    Private _CBCbCunterbinden As Boolean
    Public Property P_CBCbCunterbinden As Boolean
        Get
            Return _CBCbCunterbinden
        End Get
        Set(ByVal value As Boolean)
            _CBCbCunterbinden = value
        End Set
    End Property

    Private _CBCallByCall As Boolean
    Public Property P_CBCallByCall As Boolean
        Get
            Return _CBCallByCall
        End Get
        Set(ByVal value As Boolean)
            _CBCallByCall = value
        End Set
    End Property

    Private _CBDialPort As Boolean
    Public Property P_CBDialPort As Boolean
        Get
            Return _CBDialPort
        End Get
        Set(ByVal value As Boolean)
            _CBDialPort = value
        End Set
    End Property

    Private _CBLogFile As Boolean
    Public Property P_CBLogFile As Boolean
        Get
            Return _CBLogFile
        End Get
        Set(ByVal value As Boolean)
            _CBLogFile = value
        End Set
    End Property

    Private _CBSymbWwdh As Boolean
    Public Property P_CBSymbWwdh As Boolean
        Get
            Return _CBSymbWwdh
        End Get
        Set(ByVal value As Boolean)
            _CBSymbWwdh = value
        End Set
    End Property

    Private _CBSymbAnrMon As Boolean
    Public Property P_CBSymbAnrMon As Boolean
        Get
            Return _CBSymbAnrMon
        End Get
        Set(ByVal value As Boolean)
            _CBSymbAnrMon = value
        End Set
    End Property

    Private _CBSymbAnrMonNeuStart As Boolean
    Public Property P_CBSymbAnrMonNeuStart As Boolean
        Get
            Return _CBSymbAnrMonNeuStart
        End Get
        Set(ByVal value As Boolean)
            _CBSymbAnrMonNeuStart = value
        End Set
    End Property


    'Office 2003 und Office 2007

    Private _CBSymbAnrListe As Boolean
    Public Property P_CBSymbAnrListe As Boolean
        Get
            Return _CBSymbAnrListe
        End Get
        Set(ByVal value As Boolean)
            _CBSymbAnrListe = value
        End Set
    End Property

    Private _CBSymbDirekt As Boolean
    Public Property P_CBSymbDirekt As Boolean
        Get
            Return _CBSymbDirekt
        End Get
        Set(ByVal value As Boolean)
            _CBSymbDirekt = value
        End Set
    End Property

    Private _CBSymbRWSuche As Boolean
    Public Property P_CBSymbRWSuche As Boolean
        Get
            Return _CBSymbRWSuche
        End Get
        Set(ByVal value As Boolean)
            _CBSymbRWSuche = value
        End Set
    End Property

    Private _CBSymbVIP As Boolean
    Public Property P_CBSymbVIP As Boolean
        Get
            Return _CBSymbVIP
        End Get
        Set(ByVal value As Boolean)
            _CBSymbVIP = value
        End Set
    End Property

    Private _CBSymbJournalimport As Boolean
    Public Property P_CBSymbJournalimport As Boolean
        Get
            Return _CBSymbJournalimport
        End Get
        Set(ByVal value As Boolean)
            _CBSymbJournalimport = value
        End Set
    End Property

    Private _CBAutoAnrList As Boolean
    Public Property P_CBAutoAnrList As Boolean
        Get
            Return _CBAutoAnrList
        End Get
        Set(ByVal value As Boolean)
            _CBAutoAnrList = value
        End Set
    End Property

    ''' <summary>
    ''' Gibt an ob nur der Hauptkontaktordner durchsucht werden muss oder alle möglichen eingebundenen Kontaktordner
    ''' </summary>
    Private _CBKHO As Boolean
    Public Property P_CBKHO As Boolean
        Get
            Return _CBKHO
        End Get
        Set(ByVal value As Boolean)
            _CBKHO = value
        End Set
    End Property

    Private _CBRWS As Boolean
    Public Property P_CBRWS As Boolean
        Get
            Return _CBRWS
        End Get
        Set(ByVal value As Boolean)
            _CBRWS = value
        End Set
    End Property

    Private _CBKErstellen As Boolean
    Public Property P_CBKErstellen As Boolean
        Get
            Return _CBKErstellen
        End Get
        Set(ByVal value As Boolean)
            _CBKErstellen = value
        End Set
    End Property

    Private _CBRWSIndex As Boolean
    Public Property P_CBRWSIndex As Boolean
        Get
            Return _CBRWSIndex
        End Get
        Set(ByVal value As Boolean)
            _CBRWSIndex = value
        End Set
    End Property

    Private _ComboBoxRWS As Integer
    Public Property P_ComboBoxRWS As Integer
        Get
            Return _ComboBoxRWS
        End Get
        Set(ByVal value As Integer)
            _ComboBoxRWS = value
        End Set
    End Property

    Private _TVKontaktOrdnerEntryID As String
    Public Property P_TVKontaktOrdnerEntryID As String
        Get
            Return _TVKontaktOrdnerEntryID
        End Get
        Set(ByVal value As String)
            _TVKontaktOrdnerEntryID = value
        End Set
    End Property

    Private _TVKontaktOrdnerStoreID As String
    Public Property P_TVKontaktOrdnerStoreID As String
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
    Private _CBIndex As Boolean
    Public Property P_CBIndex As Boolean
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
    Private _CBJournal As Boolean
    Public Property P_CBJournal As Boolean
        Get
            Return _CBJournal
        End Get
        Set(ByVal value As Boolean)
            _CBJournal = value
        End Set
    End Property

    ''' <summary>
    ''' Gibt an, ob bei der Auswertung der Anrufliste die Journaleinträge aktualisiert werden sollen.
    ''' </summary>
    Private _CBAnrListeUpdateJournal As Boolean
    Public Property P_CBAnrListeUpdateJournal As Boolean
        Get
            Return _CBAnrListeUpdateJournal
        End Get
        Set(ByVal value As Boolean)
            _CBAnrListeUpdateJournal = value
        End Set
    End Property

    ''' <summary>
    ''' Gibt an, ob bei der Auswertung der Anrufliste die Wahlwiederholungs- und Rückrufliste aktualisiert werden sollen.
    ''' </summary>
    Private _CBAnrListeUpdateCallLists As Boolean
    Public Property P_CBAnrListeUpdateCallLists As Boolean
        Get
            Return _CBAnrListeUpdateCallLists
        End Get
        Set(ByVal value As Boolean)
            _CBAnrListeUpdateCallLists = value
        End Set
    End Property

    ''' <summary>
    ''' Gibt an, ob verpasste Anrufe mit Hilfe des Anrufmonitors angezeigt werden sollen.
    ''' </summary>
    Private _CBAnrListeShowAnrMon As Boolean
    Public Property P_CBAnrListeShowAnrMon As Boolean
        Get
            Return _CBAnrListeShowAnrMon
        End Get
        Set(ByVal value As Boolean)
            _CBAnrListeShowAnrMon = value
        End Set
    End Property

    Private _CBUseAnrMon As Boolean
    Public Property P_CBUseAnrMon As Boolean
        Get
            Return _CBUseAnrMon
        End Get
        Set(ByVal value As Boolean)
            _CBUseAnrMon = value
        End Set
    End Property

    Private _CBCheckMobil As Boolean
    Public Property P_CBCheckMobil As Boolean
        Get
            Return _CBCheckMobil
        End Get
        Set(ByVal value As Boolean)
            _CBCheckMobil = value
        End Set
    End Property

    Private _CLBTelNr As ReadOnlyCollection(Of String)
    Public ReadOnly Property P_CLBTelNr() As ReadOnlyCollection(Of String)
        Get
            Return _CLBTelNr
        End Get
    End Property

    'StoppUhr

    Private _CBStoppUhrEinblenden As Boolean
    Public Property P_CBStoppUhrEinblenden As Boolean
        Get
            Return _CBStoppUhrEinblenden
        End Get
        Set(ByVal Value As Boolean)
            _CBStoppUhrEinblenden = Value
        End Set
    End Property

    Private _CBStoppUhrAusblenden As Boolean
    Public Property P_CBStoppUhrAusblenden As Boolean
        Get
            Return _CBStoppUhrAusblenden
        End Get
        Set(ByVal Value As Boolean)
            _CBStoppUhrAusblenden = Value
        End Set
    End Property

    Private _TBStoppUhr As Integer
    Public Property P_TBStoppUhr As Integer
        Get
            Return _TBStoppUhr
        End Get
        Set(ByVal value As Integer)
            _TBStoppUhr = value
        End Set
    End Property

    Private _CBStoppUhrX As Integer
    Public Property P_CBStoppUhrX As Integer
        Get
            Return _CBStoppUhrX
        End Get
        Set(ByVal value As Integer)
            _CBStoppUhrX = value
        End Set
    End Property

    Private _CBStoppUhrY As Integer
    Public Property P_CBStoppUhrY() As Integer
        Get
            Return _CBStoppUhrY
        End Get
        Set(ByVal value As Integer)
            _CBStoppUhrY = value
        End Set
    End Property

    Private _P_CBStoppUhrIgnIntFax As Boolean
    Public Property P_CBStoppUhrIgnIntFax As Boolean
        Get
            Return _P_CBStoppUhrIgnIntFax
        End Get
        Set(ByVal Value As Boolean)
            _P_CBStoppUhrIgnIntFax = Value
        End Set
    End Property

    ' Telefonnummernformatierung

    Private _TBTelNrMaske As String
    Public Property P_TBTelNrMaske As String
        Get
            Return _TBTelNrMaske
        End Get
        Set(ByVal value As String)
            _TBTelNrMaske = value
        End Set
    End Property

    Private _CBTelNrGruppieren As Boolean
    Public Property P_CBTelNrGruppieren As Boolean
        Get
            Return _CBTelNrGruppieren
        End Get
        Set(ByVal value As Boolean)
            _CBTelNrGruppieren = value
        End Set
    End Property

    Private _CBintl As Boolean
    Public Property P_CBintl As Boolean
        Get
            Return _CBintl
        End Get
        Set(ByVal value As Boolean)
            _CBintl = value
        End Set
    End Property

    Private _CBIgnoTelNrFormat As Boolean
    Public Property P_CBIgnoTelNrFormat() As Boolean
        Get
            Return _CBIgnoTelNrFormat
        End Get
        Set(ByVal value As Boolean)
            _CBIgnoTelNrFormat = value
        End Set
    End Property

    'Phoner

    Private _CBPhoner As Boolean
    Public Property P_CBPhoner As Boolean
        Get
            Return _CBPhoner
        End Get
        Set(ByVal value As Boolean)
            _CBPhoner = value
        End Set
    End Property

    Private _PhonerVerfügbar As Boolean
    Public Property P_PhonerVerfügbar As Boolean
        Get
            Return _PhonerVerfügbar
        End Get
        Set(ByVal value As Boolean)
            _PhonerVerfügbar = value
        End Set
    End Property

    Private _CBPhonerAnrMon As Boolean
    Public Property P_CBPhonerAnrMon As Boolean
        Get
            Return _CBPhonerAnrMon
        End Get
        Set(ByVal value As Boolean)
            _CBPhonerAnrMon = value
        End Set
    End Property

    Private _ComboBoxPhonerSIP As Integer
    Public Property P_ComboBoxPhonerSIP() As Integer
        Get
            Return _ComboBoxPhonerSIP
        End Get
        Set(ByVal value As Integer)
            _ComboBoxPhonerSIP = value
        End Set
    End Property

    Private _TBPhonerPasswort As String
    Public Property P_TBPhonerPasswort() As String
        Get
            Return _TBPhonerPasswort
        End Get
        Set(ByVal value As String)
            _TBPhonerPasswort = value
        End Set
    End Property

    Private _PhonerTelNameIndex As Integer
    Public Property P_PhonerTelNameIndex As Integer
        Get
            Return _PhonerTelNameIndex
        End Get
        Set(ByVal value As Integer)
            _PhonerTelNameIndex = value
        End Set
    End Property

    ' Statistik

    Private _StatResetZeit As Date
    Public Property P_StatResetZeit As Date
        Get
            Return _StatResetZeit
        End Get
        Set(ByVal value As Date)
            _StatResetZeit = value
        End Set
    End Property

    Private _StatVerpasst As Integer
    Public Property P_StatVerpasst As Integer
        Get
            Return _StatVerpasst
        End Get
        Set(ByVal value As Integer)
            _StatVerpasst = value
        End Set
    End Property

    Private _StatNichtErfolgreich As Integer
    Public Property P_StatNichtErfolgreich As Integer
        Get
            Return _StatNichtErfolgreich
        End Get
        Set(ByVal value As Integer)
            _StatNichtErfolgreich = value
        End Set
    End Property

    Private _StatJournal As Integer
    Public Property P_StatJournal As Integer
        Get
            Return _StatJournal
        End Get
        Set(ByVal value As Integer)
            _StatJournal = value
        End Set
    End Property

    Private _StatKontakt As Integer
    Public Property P_StatKontakt As Integer
        Get
            Return _StatKontakt
        End Get
        Set(ByVal value As Integer)
            _StatKontakt = value
        End Set
    End Property

    Private _StatOLClosedZeit As Date
    Public Property P_StatOLClosedZeit As Date
        Get
            Return _StatOLClosedZeit
        End Get
        Set(ByVal value As Date)
            _StatOLClosedZeit = value
        End Set
    End Property

    ' Wählbox

    Private _TelAnschluss As Integer
    Public Property P_TelAnschluss As Integer
        Get
            Return _TelAnschluss
        End Get
        Set(ByVal value As Integer)
            _TelAnschluss = value
        End Set
    End Property

    Private _TelFestnetz As Boolean
    Public Property P_TelFestnetz As Boolean
        Get
            Return _TelFestnetz
        End Get
        Set(ByVal value As Boolean)
            _TelFestnetz = value
        End Set
    End Property

    Private _TelCLIR As Boolean
    Public Property P_TelCLIR As Boolean
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
    Private _EncodingFritzBox As Text.Encoding
    Public Property P_EncodingFritzBox As Text.Encoding
        Get
            Return _EncodingFritzBox
        End Get
        Set(ByVal value As Text.Encoding)
            _EncodingFritzBox = value
        End Set
    End Property

    ''' <summary>
    ''' Gibt die eingegebene Fritz!Box IP-Adresse an. Dies ist eine Angabe, die der Nutzer in den Einstellungen ändern kann.
    ''' </summary>
    Private _TBFBAdr As String
    Public Property P_TBFBAdr As String
        Get
            Return _TBFBAdr
        End Get
        Set(ByVal value As String)
            _TBFBAdr = value
        End Set
    End Property

    ''' <summary>
    ''' Gibt eine korrekte Fritz!Box IP-Adresse zurück.
    ''' </summary>
    Private _ValidFBAdr As String
    Public Property P_ValidFBAdr As String
        Get
            Return _ValidFBAdr
        End Get
        Set(ByVal value As String)
            _ValidFBAdr = value
        End Set
    End Property

    ''' <summary>
    ''' Gibt an, ob eine Verbindung zur Fritz!Box trotz fehlgeschlagenen Pings aufgebaut werden soll.
    ''' </summary>
    Private _CBForceFBAddr As Boolean
    Public Property P_CBForceFBAddr As Boolean
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
    Private _TBBenutzer As String
    Public Property P_TBBenutzer As String
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
    Private _TBPasswort As String
    Public Property P_TBPasswort As String
        Get
            Return _TBPasswort
        End Get
        Set(ByVal value As String)
            _TBPasswort = value
        End Set
    End Property

    ' Indizierung
    Private _LLetzteIndizierung As Date
    Public Property P_LLetzteIndizierung As Date
        Get
            Return _LLetzteIndizierung
        End Get
        Set(ByVal value As Date)
            _LLetzteIndizierung = value
        End Set
    End Property

    ' Notiz
    Private _CBNote As Boolean
    Public Property P_CBNote As Boolean
        Get
            Return _CBNote
        End Get
        Set(ByVal value As Boolean)
            _CBNote = value
        End Set
    End Property

    ' Einstellungen

    Private _Arbeitsverzeichnis As String
    Public Property P_Arbeitsverzeichnis As String
        Get
            Return _Arbeitsverzeichnis
        End Get
        Set(value As String)
            _Arbeitsverzeichnis = value
        End Set
    End Property

    ' Vorwahllisten
    Private _ListeOrtsVorwahlenD As ReadOnlyCollection(Of String)
    Public ReadOnly Property P_ListeOrtsVorwahlenD() As ReadOnlyCollection(Of String)
        Get
            Return _ListeOrtsVorwahlenD
        End Get
    End Property

    Private _ListeLandesVorwahlen As ReadOnlyCollection(Of String)
    Public ReadOnly Property P_ListeLandesVorwahlen() As ReadOnlyCollection(Of String)
        Get
            Return _ListeLandesVorwahlen
        End Get
    End Property

    Private _ListeOrtsVorwahlenA As ReadOnlyCollection(Of String)
    Public ReadOnly Property P_ListeOrtsVorwahlenA() As ReadOnlyCollection(Of String)
        Get
            Return _ListeOrtsVorwahlenA
        End Get
    End Property

    ' Fritz!Box Kommunikation
    Private _RBFBComUPnP As Boolean
    Public Property P_RBFBComUPnP() As Boolean
        Set(value As Boolean)
            _RBFBComUPnP = value
        End Set
        Get
            Return _RBFBComUPnP
        End Get
    End Property
#End Region

#Region "Global Default Value Properties"
    ''' <summary>
    ''' 00 als String
    ''' </summary>
    ''' <value>00</value>
    ''' <returns>String</returns>
    Public Shared ReadOnly Property P_Def_PreLandesVW() As String
        Get
            Return "00"
        End Get
    End Property
    ''' <summary>
    ''' -1 als String.
    ''' Default Fehler
    ''' </summary>
    ''' <value>-1</value>
    ''' <returns>String</returns>
    Public Shared ReadOnly Property P_Def_ErrorMinusOne_String() As String
        Get
            Return XML.P_Def_ErrorMinusOne_String '"-1"
        End Get
    End Property

    ''' <summary>
    ''' -1 als Integer
    ''' </summary>
    Public Shared ReadOnly Property P_Def_ErrorMinusOne_Integer() As Integer
        Get
            Return -1
        End Get
    End Property

    ''' <summary>
    ''' -2 als String
    ''' </summary>
    Public Shared ReadOnly Property P_Def_ErrorMinusTwo_String() As String
        Get
            Return "-2"
        End Get
    End Property

    ''' <summary>
    ''' Leerstring, String.Empty
    ''' </summary>
    Public Shared ReadOnly Property P_Def_LeerString() As String
        Get
            Return XML.P_Def_StringEmpty 'String.Empty
        End Get
    End Property

    ''' <summary>
    ''' vbCrLf
    ''' </summary>
    ''' <value>vbCrLf</value>
    ''' <returns>vbCrLf</returns>
    Public Shared ReadOnly Property P_Def_EineNeueZeile() As String
        Get
            Return vbCrLf
        End Get
    End Property

    ''' <summary>
    ''' vbCrLf &amp; vbCrLf
    ''' </summary>
    ''' <value>vbCrLf &amp; vbCrLf</value>
    ''' <returns>vbCrLf &amp; vbCrLf</returns>
    Public Shared ReadOnly Property P_Def_ZweiNeueZeilen() As String
        Get
            Return P_Def_EineNeueZeile & P_Def_EineNeueZeile
        End Get
    End Property

    ''' <summary>
    ''' String: unbekannt
    ''' </summary>
    Public Shared ReadOnly Property P_Def_StringUnknown() As String
        Get
            Return "unbekannt"
        End Get
    End Property

    ''' <summary>
    ''' fritz.box
    ''' </summary>
    Public Shared ReadOnly Property P_Def_FritzBoxAdress() As String
        Get
            Return "fritz.box"
        End Get
    End Property

    ''' <summary>
    ''' 192.168.178.1
    ''' </summary>
    Public Shared ReadOnly Property P_Def_FritzBoxIPAdress() As String
        Get
            Return "192.168.178.1"
        End Get
    End Property

    ''' <summary>
    ''' Fritz!Box
    ''' </summary>
    Public Shared ReadOnly Property P_Def_FritzBoxName() As String
        Get
            Return "Fritz!Box"
        End Get
    End Property

    ''' <summary>
    ''' FRITZ!Box_Anrufliste.csv
    ''' </summary>
    Public Shared ReadOnly Property P_Def_AnrListFileName() As String
        Get
            Return "FRITZ!Box_Anrufliste.csv"
        End Get
    End Property

    ''' <summary>
    ''' #96*5*
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TelCodeActivateFritzBoxCallMonitor() As String
        Get
            Return "#96*5*"
        End Get
    End Property

    ''' <summary>
    ''' 1012
    ''' </summary>
    Public Shared ReadOnly Property P_DefaultFBAnrMonPort() As Integer
        Get
            Return 1012
        End Get
    End Property

    ''' <summary>
    ''' Der Zahlenwert NULL <code>"0"</code> als String.
    ''' </summary>
    ''' <value>"0"</value>
    ''' <returns>"0"</returns>
    Public Shared ReadOnly Property P_Def_StringNull() As String
        Get
            Return "0"
        End Get
    End Property

    ''' <summary>
    ''' Das Leerzeichen als <code>Chr(32)</code> als String.
    ''' </summary>
    ''' <value>" "</value>
    ''' <returns>" "</returns>
    Public Shared ReadOnly Property P_Def_Leerzeichen() As String
        Get
            Return Chr(32) '" "
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_StringErrorMinusOne() As String
        Get
            Return CStr(P_Def_ErrorMinusOne_String)
        End Get
    End Property

    ''' <summary>
    ''' 0000000000000000
    ''' </summary>
    Public Shared ReadOnly Property P_Def_SessionID() As String
        Get
            Return "0000000000000000"
        End Get
    End Property

    ''' <summary>
    ''' Mozilla/5.0 (compatible; MSIE 7.0; Windows NT 6.0; WOW64; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.0.04506; Media Center PC 5.0; .NET CLR 3.5.21022; .NET CLR 1.1.4322)
    ''' </summary>
    Public Shared ReadOnly Property P_Def_Header_UserAgent() As String
        Get
            Return "Mozilla/5.0 (compatible; MSIE 7.0; Windows NT 6.0; WOW64; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.0.04506; Media Center PC 5.0; .NET CLR 3.5.21022; .NET CLR 1.1.4322)"
        End Get
    End Property

    ''' <summary>
    ''' application/x-www-form-urlencoded
    ''' </summary>
    Public Shared ReadOnly Property P_Def_Header_ContentType() As String
        Get
            Return "application/x-www-form-urlencoded"
        End Get
    End Property

    ''' <summary>
    ''' text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
    ''' </summary>
    Public Shared ReadOnly Property P_Def_Header_Accept() As String
        Get
            Return "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
        End Get
    End Property

    ''' <summary>
    ''' 3000
    ''' </summary>
    Public Shared ReadOnly Property P_Def_ReStartIntervall() As Integer
        Get
            Return 2000
        End Get
    End Property

    ''' <summary>
    ''' 15
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TryMaxRestart() As Integer
        Get
            Return 15
        End Get
    End Property

    ''' <summary>
    ''' [-&gt;]
    ''' </summary>
    Public Shared ReadOnly Property P_Def_AnrMonDirection_Call() As String
        Get
            Return "[->]"
        End Get
    End Property

    ''' <summary>
    ''' [&lt;-]
    ''' </summary>
    Public Shared ReadOnly Property P_Def_AnrMonDirection_Ring() As String
        Get
            Return "[<-]"
        End Get
    End Property

    ''' <summary>
    ''' [&lt;&gt;]
    ''' </summary>
    Public Shared ReadOnly Property P_Def_AnrMonDirection_Default() As String
        Get
            Return "[<>]"
        End Get
    End Property

    ''' <summary>
    ''' FBDB-AnrMonDirection
    ''' </summary>
    Public Shared ReadOnly Property P_Def_AnrMonDirection_UserProperty_Name() As String
        Get
            Return "FBDB-AnrMonDirection"
        End Get
    End Property

    ''' <summary>
    ''' FBDB-AnrMonZeit
    ''' </summary>
    Public Shared ReadOnly Property P_Def_AnrMonDirection_UserProperty_Zeit() As String
        Get
            Return "FBDB-AnrMonZeit"
        End Get
    End Property

    ''' <summary>
    ''' FBDB_Note_Table
    ''' </summary>
    Public Shared ReadOnly Property P_Def_Note_Table() As String
        Get
            Return "FBDB_Note_Table"
        End Get
    End Property

    ''' <summary>
    ''' BEGIN:VCARD
    ''' </summary>
    Public Shared ReadOnly Property P_Def_Begin_vCard() As String
        Get
            Return "BEGIN:VCARD"
        End Get
    End Property

    ''' <summary>
    ''' END:VCARD
    ''' </summary>
    Public Shared ReadOnly Property P_Def_End_vCard() As String
        Get
            Return "END:VCARD"
        End Get
    End Property

    ''' <summary>
    ''' CallList
    ''' </summary>
    ''' <value>CallList</value>
    ''' <returns>CallList</returns>
    Public Shared ReadOnly Property P_Def_NameListCALL() As String
        Get
            Return "CallList"
        End Get
    End Property

    ''' <summary>
    ''' RingList
    ''' </summary>
    ''' <value>RingList</value>
    ''' <returns>RingList</returns>
    Public Shared ReadOnly Property P_Def_NameListRING() As String
        Get
            Return "RingList"
        End Get
    End Property

    ''' <summary>
    ''' VIPList
    ''' </summary>
    ''' <value>VIPList</value>
    ''' <returns>VIPList</returns>
    Public Shared ReadOnly Property P_Def_NameListVIP() As String
        Get
            Return "VIPList"
        End Get
    End Property

    ''' <summary>
    ''' Fritz!Box Telefon-dingsbums
    ''' </summary>
    ''' <value>Fritz!Box Telefon-dingsbums</value>
    ''' <returns>Fritz!Box Telefon-dingsbums</returns>
    Public Shared ReadOnly Property P_Def_Addin_LangName() As String
        Get
            Return "Fritz!Box Telefon-dingsbums"
        End Get
    End Property

    ''' <summary>
    ''' FritzOutlook
    ''' </summary>
    ''' <value>FritzOutlook</value>
    ''' <returns>FritzOutlook</returns>
    Public Shared ReadOnly Property P_Def_Addin_KurzName() As String
        Get
            Return "FritzOutlook"
        End Get
    End Property

    ''' <summary>
    ''' FritzOutlook.xml
    ''' </summary>
    ''' <value>FritzOutlook.xml</value>
    ''' <returns>FritzOutlook.xml</returns>
    ''' <remarks>Wird mit "P_Def_Addin_KurzName" erstellt.</remarks>
    Public Shared ReadOnly Property P_Def_Config_FileName() As String
        Get
            Return P_Def_Addin_KurzName & ".xml"
        End Get
    End Property

    ''' <summary>
    ''' FritzOutlook.log
    ''' </summary>
    ''' <value>FritzOutlook.log</value>
    ''' <returns>FritzOutlook.log</returns>
    ''' <remarks>Wird mit "P_Def_Addin_KurzName" erstellt.</remarks>
    Public Shared ReadOnly Property P_Def_Log_FileName() As String
        Get
            Return P_Def_Addin_KurzName & ".log"
        End Get
    End Property

    ''' <summary>
    ''' Gibt den Zeitraum in MINUTEN an, nachdem geprüft werden soll, ob der Anrufmonitor noch aktiv ist. 
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Intervall in MINUTEN</returns>
    Public Shared ReadOnly Property P_Def_CheckAnrMonIntervall() As Integer
        Get
            Return 1
        End Get
    End Property

    ''' <summary>
    ''' Gibt den default Dialport für Mobilgeräte an. 
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>99</returns>
    Public Shared ReadOnly Property P_Def_MobilDialPort() As Integer
        Get
            Return 99
        End Get
    End Property

    ''' <summary>
    ''' Der Offset der bei der Auswertung der Anrufliste der AnrufID gegeben wird
    ''' </summary>
    ''' <value>100</value>
    Public Shared ReadOnly Property P_Def_AnrListIDOffset() As Integer
        Get
            Return 100
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_DirectorySeparatorChar() As String
        Get
            Return IO.Path.DirectorySeparatorChar
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_AddInPath() As String
        Get
            Return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & P_Def_DirectorySeparatorChar & P_Def_Addin_LangName & P_Def_DirectorySeparatorChar
        End Get
    End Property

    ''' <summary>
    ''' Ein Array, welches den Namen der UserProperties, die die unformatierte Telefonnummer enthält.
    ''' </summary>
    ''' <value>String-Array</value>
    ''' <returns>String-Array</returns>
    Public Shared ReadOnly Property P_Def_UserProperties() As ReadOnlyCollection(Of String)
        Get
            Dim tmp As New List(Of String)

            tmp.Add("FBDB-AssistantTelephoneNumber")
            tmp.Add("FBDB-BusinessTelephoneNumber")
            tmp.Add("FBDB-Business2TelephoneNumber")
            tmp.Add("FBDB-CallbackTelephoneNumber")
            tmp.Add("FBDB-CarTelephoneNumber")
            tmp.Add("FBDB-CompanyMainTelephoneNumber")
            tmp.Add("FBDB-HomeTelephoneNumber")
            tmp.Add("FBDB-Home2TelephoneNumber")
            tmp.Add("FBDB-ISDNNumber")
            tmp.Add("FBDB-MobileTelephoneNumber")
            tmp.Add("FBDB-OtherTelephoneNumber")
            tmp.Add("FBDB-PagerNumber")
            tmp.Add("FBDB-PrimaryTelephoneNumber")
            tmp.Add("FBDB-RadioTelephoneNumber")
            tmp.Add("FBDB-BusinessFaxNumber")
            tmp.Add("FBDB-HomeFaxNumber")
            tmp.Add("FBDB-OtherFaxNumber")
            tmp.Add("FBDB-Telex")
            tmp.Add("FBDB-TTYTDDTelephoneNumber")

            Return New ReadOnlyCollection(Of String)(tmp)
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_olTelNrTypen() As ReadOnlyCollection(Of String)
        Get
            Dim tmp As New List(Of String)

            tmp.Add("Assistent")
            tmp.Add("Geschäftlich")
            tmp.Add("Geschäftlich 2")
            tmp.Add("Rückmeldung")
            tmp.Add("Auto")
            tmp.Add("Firma")
            tmp.Add("Privat")
            tmp.Add("Privat 2")
            tmp.Add("ISDN")
            tmp.Add("Mobiltelefon")
            tmp.Add("Weitere")
            tmp.Add("Pager")
            tmp.Add("Haupttelefon")
            tmp.Add("Funkruf")
            tmp.Add("Fax geschäftl.")
            tmp.Add("Fax privat")
            tmp.Add("Weiteres Fax")
            tmp.Add("Telex")
            tmp.Add("Texttelefon")

            Return New ReadOnlyCollection(Of String)(tmp)
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_MobilVorwahlItalien() As ReadOnlyCollection(Of String)
        Get
            Dim tmp As New List(Of String)
            tmp.Add("330")
            tmp.Add("331")
            tmp.Add("332")
            tmp.Add("333")
            tmp.Add("334")
            tmp.Add("335")
            tmp.Add("336")
            tmp.Add("337")
            tmp.Add("338")
            tmp.Add("339")
            tmp.Add("360")
            tmp.Add("361")
            tmp.Add("362")
            tmp.Add("363")
            tmp.Add("364")
            tmp.Add("365")
            tmp.Add("366")
            tmp.Add("367")
            tmp.Add("368")
            tmp.Add("390")
            tmp.Add("391")
            tmp.Add("392")
            tmp.Add("393")
            tmp.Add("340")
            tmp.Add("341")
            tmp.Add("342")
            tmp.Add("343")
            tmp.Add("344")
            tmp.Add("345")
            tmp.Add("346")
            tmp.Add("347")
            tmp.Add("348")
            tmp.Add("349")
            tmp.Add("380")
            tmp.Add("381")
            tmp.Add("382")
            tmp.Add("383")
            tmp.Add("384")
            tmp.Add("385")
            tmp.Add("386")
            tmp.Add("387")
            tmp.Add("388")
            tmp.Add("389")
            tmp.Add("320")
            tmp.Add("321")
            tmp.Add("322")
            tmp.Add("323")
            tmp.Add("324")
            tmp.Add("325")
            tmp.Add("326")
            tmp.Add("327")
            tmp.Add("328")
            tmp.Add("329")


            Return New ReadOnlyCollection(Of String)(tmp)
        End Get
    End Property


    Public Shared ReadOnly Property P_Def_UserPropertyIndex() As String
        Get
            Return "FBDB-Save"
        End Get
    End Property
#Region "Journal"
    Public Shared ReadOnly Property P_Def_Journal_Text_Eingehend() As String
        Get
            Return "Eingehender Anruf von "
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_Journal_Text_Ausgehend() As String
        Get
            Return "Ausgehender Anruf zu "
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_Journal_Text_Verpasst() As String
        Get
            Return "Verpasster Anruf von "
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_Journal_Text_NichtErfolgreich() As String
        Get
            Return "Nicht erfolgreicher Anruf zu "
        End Get
    End Property
#End Region

#Region "Phoner"
    Public Shared ReadOnly Property P_Def_Phoner_CONNECT As String
        Get
            Return "CONNECT " 'Das Leerzeichen wird benötigt!
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_Phoner_DISCONNECT As String
        Get
            Return "DISCONNECT"
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_Phoner_Challenge As String
        Get
            Return "Challenge="
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_Phoner_Response As String
        Get
            Return "Response="
        End Get
    End Property


    ''' <summary>
    ''' Welcome To Phoner
    ''' </summary>
    Public Shared ReadOnly Property P_Def_Phoner_Ready As String
        Get
            Return "Welcome To Phoner"
        End Get
    End Property

    Public Shared ReadOnly Property P_DefaultPhonerAnrMonPort() As Integer
        Get
            Return 2012
        End Get
    End Property
#End Region
    ' Passwortverschlüsselung
    Public Shared ReadOnly Property P_Def_PassWordDecryptionKey() As String
        Get
            Return "Fritz!Box Script"
        End Get
    End Property

#Region "CommandBar Office 2003 und Office 2007"
    ''' <summary>
    ''' Der Tag des CommandbarButtons Direktwahl
    ''' </summary>
    ''' <value>eBtnDirektwahl</value>
    Public Shared ReadOnly Property P_CMB_eBtnDirektwahl_Tag() As String
        Get
            Return "eBtnDirektwahl"
        End Get
    End Property

    ''' <summary>
    ''' Der Tag des CommandbarButtons Wählen
    ''' </summary>
    ''' <value>eBtnWaehlen</value>
    Public Shared ReadOnly Property P_CMB_eBtnWaehlen_Tag() As String
        Get
            Return "eBtnWaehlen"
        End Get
    End Property

    ''' <summary>
    ''' Der Tag des CommandbarButtons Einstellungen
    ''' </summary>
    ''' <value>eBtnEinstellungen</value>
    Public Shared ReadOnly Property P_CMB_eBtnEinstellungen_Tag() As String
        Get
            Return "eBtnEinstellungen"
        End Get
    End Property

    ''' <summary>
    ''' Der Tag des CommandbarButtons Anrufmonitor
    ''' </summary>
    ''' <value>eBtnAnrMon</value>
    Public Shared ReadOnly Property P_CMB_eBtnAnrMon_Tag() As String
        Get
            Return "eBtnAnrMon"
        End Get
    End Property

    ''' <summary>
    ''' Der Tag des CommandbarButtons Anrufmonitor Anzeigen
    ''' </summary>
    ''' <value>eBtnAnzeigen</value>
    Public Shared ReadOnly Property P_CMB_eBtnAnzeigen_Tag() As String
        Get
            Return "eBtnAnzeigen"
        End Get
    End Property

    ''' <summary>
    ''' Der Tag des CommandbarButtons Journalimport
    ''' </summary>
    ''' <value>eBtnJournalimport</value>
    Public Shared ReadOnly Property P_CMB_eBtnJournalimport_Tag() As String
        Get
            Return "eBtnJournalimport"
        End Get
    End Property

    ''' <summary>
    ''' Der Tag des CommandbarButtons Anrufmonitor Neustarten
    ''' </summary>
    ''' <value>eBtnAnrMonNeuStart</value>
    Public Shared ReadOnly Property P_CMB_eBtnAnrMonNeuStart_Tag() As String
        Get
            Return "eBtnAnrMonNeuStart"
        End Get
    End Property

    ''' <summary>
    ''' Der Tag des Löschbuttens der Anruf und VIP-Listen
    ''' </summary>
    ''' <value>dynListDel</value>
    Public Shared ReadOnly Property P_CMB_eDynListDel_Tag() As String
        Get
            Return "dynListDel"
        End Get
    End Property

#End Region


#End Region

#Region "Default Value Properties"
    ''' <summary>
    ''' Landesvorwahl für Deutschland mit zwei führenden Nullen: 0049
    ''' </summary>
    ''' <value>0049</value>
    ''' <returns>0049</returns>
    Public Shared ReadOnly Property P_Def_TBLandesVW() As String
        Get
            Return P_Def_PreLandesVW & "49"
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBoxLandesVorwahl() As Integer
        Get
            Return P_Def_ErrorMinusOne_Integer
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_TBAmt() As String
        Get
            Return P_Def_ErrorMinusOne_String
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_TBVorwahl() As String
        Get
            Return P_Def_LeerString
        End Get
    End Property

    ''' <summary>
    ''' 10
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TBNumEntryList() As Integer
        Get
            Return 10
        End Get
    End Property

    ''' <summary>
    ''' 0
    ''' </summary>
    Public Shared ReadOnly Property P_Def_CBoxVorwahl() As Integer
        Get
            Return 0
        End Get
    End Property

    ''' <summary>
    ''' 10
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TBEnblDauer() As Integer
        Get
            Return 10
        End Get
    End Property

    ''' <summary>
    ''' False
    ''' </summary>
    Public Shared ReadOnly Property P_Def_CBAnrMonAuto() As Boolean
        Get
            Return False
        End Get
    End Property

    ''' <summary>
    ''' 30
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TBAnrBeantworterTimeout() As Integer
        Get
            Return 30
        End Get
    End Property

    ''' <summary>
    ''' 0
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TBAnrMonX() As Integer
        Get
            Return 0
        End Get
    End Property

    ''' <summary>
    ''' 0
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TBAnrMonY() As Integer
        Get
            Return 0
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_CBAnrMonMove() As Boolean
        Get
            Return True
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_CBAnrMonTransp() As Boolean
        Get
            Return True
        End Get
    End Property

    ''' <summary>
    ''' 0
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TBAnrMonMoveGeschwindigkeit() As Integer
        Get
            Return 0
        End Get
    End Property

    ''' <summary>
    ''' 0
    ''' </summary>
    Public Shared ReadOnly Property P_Def_CBoxAnrMonStartPosition() As Integer
        Get
            Return 0
        End Get
    End Property

    ''' <summary>
    ''' 0
    ''' </summary>
    Public Shared ReadOnly Property P_Def_CBoxAnrMonMoveDirection() As Integer
        Get
            Return 0
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_CBAnrMonZeigeKontakt() As Boolean
        Get
            Return False
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_CBAnrMonContactImage() As Boolean
        Get
            Return True
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_CBIndexAus() As Boolean
        Get
            Return False
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_CBShowMSN() As Boolean
        Get
            Return False
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_CBAnrMonCloseAtDISSCONNECT() As Boolean
        Get
            Return False
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_CBAutoClose() As Boolean
        Get
            Return True
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_CBVoIPBuster() As Boolean
        Get
            Return False
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_CBCbCunterbinden() As Boolean
        Get
            Return False
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_CBCallByCall() As Boolean
        Get
            Return False
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_CBDialPort() As Boolean
        Get
            Return False
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_CBKErstellen() As Boolean
        Get
            Return False
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_CBLogFile() As Boolean
        Get
            Return True
        End Get
    End Property
    'Einstellung für die Symbolleiste
    Public Shared ReadOnly Property P_Def_CBSymbWwdh() As Boolean
        Get
            Return True
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBSymbAnrMon() As Boolean
        Get
            Return True
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBSymbAnrMonNeuStart() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBSymbAnrListe() As Boolean
        Get
            Return True
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBSymbDirekt() As Boolean
        Get
            Return True
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBSymbRWSuche() As Boolean
        Get
            Return True
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBSymbVIP() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBSymbJournalimport() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBJImport() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBAnrListeUpdateJournal() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBAnrListeUpdateCallLists() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBAnrListeShowAnrMon() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBRWS() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_TVKontaktOrdnerEntryID() As String
        Get
            Return P_Def_StringErrorMinusOne
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_TVKontaktOrdnerStoreID() As String
        Get
            Return P_Def_StringErrorMinusOne
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBKHO() As Boolean
        Get
            Return True
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBRWSIndex() As Boolean
        Get
            Return True
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_ComboBoxRWS() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBIndex() As Boolean
        Get
            Return True
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBJournal() As Boolean
        Get
            Return True
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBUseAnrMon() As Boolean
        Get
            Return True
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBCheckMobil() As Boolean
        Get
            Return True
        End Get
    End Property
    'StoppUhr
    Public Shared ReadOnly Property P_Def_CBStoppUhrEinblenden() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBStoppUhrAusblenden() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_TBStoppUhr() As Integer
        Get
            Return 10
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBStoppUhrX() As Integer
        Get
            Return 10
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBStoppUhrY() As Integer
        Get
            Return 10
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBStoppUhrIgnIntFax() As Boolean
        Get
            Return False
        End Get
    End Property

    ' Telefonnummernformatierung
    ''' <summary>
    ''' Nach der Maske werden Telefonnummern formatiert: %L (%O) %N - %D
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TBTelNrMaske() As String
        Get
            Return "%L (%O) %N - %D"
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBTelNrGruppieren() As Boolean
        Get
            Return True
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBintl() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBIgnoTelNrFormat() As Boolean
        Get
            Return False
        End Get
    End Property
    'Phoner
    Public Shared ReadOnly Property P_Def_CBPhoner As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_PhonerVerfügbar As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBPhonerAnrMon As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_ComboBoxPhonerSIP() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_TBPhonerPasswort() As String
        Get
            Return P_Def_LeerString
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_PhonerTelNameIndex() As Integer
        Get
            Return 0
        End Get
    End Property
    ' Statistik
    Public Shared ReadOnly Property P_Def_StatResetZeit As Date
        Get
            Return System.DateTime.Now
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_StatVerpasst As Integer
        Get
            Return 0
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_StatNichtErfolgreich As Integer
        Get
            Return 0
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_StatJournal() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_StatKontakt() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_StatOLClosedZeit() As Date
        Get
            Return System.DateTime.Now
        End Get
    End Property
    ' Wählbox
    Public Shared ReadOnly Property P_Def_TelAnschluss() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_TelFestnetz() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_TelCLIR() As Boolean
        Get
            Return False
        End Get
    End Property
    ' FritzBox
    ''' <summary>
    ''' Standard-Codierung der Fritz!Box: utf-8
    ''' </summary>
    ''' <returns>Text.Encoding.UTF8</returns>
    Public Shared ReadOnly Property P_Def_EncodingFritzBox() As Text.Encoding
        Get
            Return Text.Encoding.UTF8
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_TBFBAdr() As String
        Get
            Return P_Def_FritzBoxAdress
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_CBForceFBAddr() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_TBBenutzer() As String
        Get
            Return P_Def_LeerString
        End Get
    End Property
    Public Shared ReadOnly Property P_Def_TBPasswort() As String
        Get
            Return P_Def_LeerString
        End Get
    End Property
    ' Indizierung
    Public Shared ReadOnly Property P_Def_LLetzteIndizierung() As Date
        Get
            Return System.DateTime.Now
        End Get
    End Property
    ' Note
    Public Shared ReadOnly Property P_Def_CBNote() As Boolean
        Get
            Return False
        End Get
    End Property
    ' Fritz!Box Kommunikation
    Public Shared ReadOnly Property P_Def_RBFBComUPnP() As Boolean
        Get
            Return False
        End Get
    End Property
#End Region

#Region "Organisation Properties"
    Public Property XMLDoc As XmlDocument
        Get
            Return P_xMLDoc
        End Get
        Set(value As XmlDocument)
            P_xMLDoc = value
        End Set
    End Property

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
    Public Shared ReadOnly Property P_Debug_Use_WebClient() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Debug_AnrufSimulation() As Boolean
        Get
            Return False
        End Get
    End Property
    Public Shared ReadOnly Property P_Debug_ImportTelefone() As Boolean
        Get
            Return False
        End Get
    End Property

    Private _Debug_FBFile As List(Of String)
    Public Property P_Debug_FBFile As List(Of String)
        Get
            Return _Debug_FBFile
        End Get
        Set(value As List(Of String))
            _Debug_FBFile = value
        End Set
    End Property

    Private _P_Debug_PfadKonfig As String
    Public Property P_Debug_PfadKonfig() As String
        Set(value As String)
            _P_Debug_PfadKonfig = value
        End Set
        Get
            Return _P_Debug_PfadKonfig
        End Get
    End Property

    Public Function Debug_getFileContend(ByVal QueryID As String) As String
        Debug_getFileContend = P_Def_LeerString
        Dim oDir As IO.DirectoryInfo
        oDir = New IO.DirectoryInfo(P_Debug_PfadKonfig)

        With oDir.GetFiles(QueryID & ".txt").First
            If .Exists Then
                Using ReadStream As New IO.StreamReader(.FullName)
                    Debug_getFileContend = ReadStream.ReadToEnd()
                    Debug_getFileContend = New String(Debug_getFileContend.Skip(InStr(Debug_getFileContend, "{", CompareMethod.Text) - 1).ToArray)
                End Using
            End If
        End With
    End Function

#End Region

#Region "Literale"
    ' Helfer
    Public Shared ReadOnly Property P_Lit_KeyChange(ByVal Code As String) As String
        Get
            Return "Das Passwort für " & Code & " kann nicht entschlüsselt werden."
        End Get
    End Property

    ' Phoner
    ''' <summary>
    ''' Nr. Code an Phoner übergeben
    ''' </summary>
    ''' <param name="Code"></param>
    Public Shared ReadOnly Property P_Lit_Phoner1(ByVal Code As String) As String
        Get
            Return "Nr. " & Code & " an Phoner übergeben."
        End Get
    End Property

    ''' <summary>
    ''' Fehler!
    ''' Das Phoner-Passwort ist falsch!
    ''' </summary>
    Public Shared ReadOnly Property P_Lit_Phoner2() As String
        Get
            Return "Fehler!" & P_Def_EineNeueZeile & "Das Phoner-Passwort ist falsch!"
        End Get
    End Property

    ''' <summary>
    ''' Fehler!
    ''' Die Phoner-Verson ist zu alt!
    ''' </summary>
    Public Shared ReadOnly Property P_Lit_Phoner3() As String
        Get
            Return "Fehler!" & P_Def_EineNeueZeile & "Die Phoner-Verson ist zu alt!"
        End Get
    End Property

    ''' <summary>
    ''' Fehler!
    ''' TCP Fehler (Stream.CanWrite = False)!
    ''' </summary>
    Public Shared ReadOnly Property P_Lit_Phoner4() As String
        Get
            Return "Fehler!" & P_Def_EineNeueZeile & "TCP Fehler (Stream.CanWrite = False)!"
        End Get
    End Property

    ''' <summary>
    ''' Fehler!
    ''' TCP!
    ''' </summary>
    Public Shared ReadOnly Property P_Lit_Phoner5() As String
        Get
            Return "Fehler!" & P_Def_EineNeueZeile & "TCP!"
        End Get
    End Property

    ''' <summary>
    ''' Fehler!
    ''' Kein Passwort hinterlegt!
    ''' </summary>
    Public Shared ReadOnly Property P_Lit_Phoner6() As String
        Get
            Return "Fehler!" & P_Def_EineNeueZeile & "Kein Passwort hinterlegt!"
        End Get
    End Property

    ''' <summary>
    ''' Fehler!
    ''' Phoner nicht verfügbar!
    ''' </summary>
    Public Shared ReadOnly Property P_Lit_Phoner7() As String
        Get
            Return "Fehler!" & P_Def_EineNeueZeile & "Phoner nicht verfügbar!"
        End Get
    End Property

    ' Anrufmonitor
    ''' <summary>
    ''' Stoppuhr für Telefonat gestartet: AnrName 
    ''' </summary>
    ''' <param name="AnrName"></param>
    Public Shared ReadOnly Property P_AnrMon_Log_StoppUhrStart1(ByVal AnrName As String) As String
        Get
            Return "Stoppuhr für Telefonat gestartet: " & AnrName
        End Get
    End Property

    ''' <summary>
    ''' Der Anrufmonitor kann nicht gestartet werden, da die Fritz!Box die Verbindung verweigert.
    ''' Dies ist meist der Fall, wenn der Fritz!Box Callmonitor deaktiviert ist. Mit dem Telefoncode #96*5* kann dieser aktiviert werden.
    ''' Soll versucht werden, den Fritz!Box Callmonitor über die Direktwahl zu aktivieren? (Danach kann der Anrufmonitor manuell aktiviert werden.)"
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_MsgBox_AnrMonStart1() As String
        Get
            Return "Der Anrufmonitor kann nicht gestartet werden, da die Fritz!Box die Verbindung verweigert." & P_Def_EineNeueZeile & "Dies ist meist der Fall, wenn der Fritz!Box Callmonitor deaktiviert ist. Mit dem Telefoncode """ & P_Def_TelCodeActivateFritzBoxCallMonitor & """ kann dieser aktiviert werden." & P_Def_EineNeueZeile & "Soll versucht werden, den Fritz!Box Callmonitor über die Direktwahl zu aktivieren? (Danach kann der Anrufmonitor manuell aktiviert werden.)"
        End Get
    End Property

    ''' <summary>
    ''' Soll der Fritz!Box Callmonitor aktiviert werden?
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_MsgBox_AnrMonStart2() As String
        Get
            Return "Soll der Fritz!Box Callmonitor aktiviert werden?"
        End Get
    End Property

    ''' <summary>
    ''' Das automatische Aktivieren des Fritz!Box Callmonitor wurde übersprungen.
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonStart1() As String
        Get
            Return "Das automatische Aktivieren des Fritz!Box Callmonitor wurde übersprungen."
        End Get
    End Property

    ''' <summary>
    ''' TCP Verbindung nicht aufgebaut: ErrMsg
    ''' </summary>
    ''' <param name="ErrMsg">Felermeldung</param>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonStart2(ByVal ErrMsg As String) As String
        Get
            Return "TCP Verbindung nicht aufgebaut: " & ErrMsg
        End Get
    End Property

    ''' <summary>
    ''' TCP Verbindung nicht aufgebaut.
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonStart3() As String
        Get
            Return "TCP Verbindung nicht aufgebaut."
        End Get
    End Property

    ''' <summary>
    ''' Anrufmonitor nach StandBy wiederaufgebaut.
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonStart4() As String
        Get
            Return "Anrufmonitor nach StandBy wiederaufgebaut."
        End Get
    End Property

    ''' <summary>
    ''' BWStartTCPReader_RunWorkerCompleted: Es ist ein TCP/IP Fehler aufgetreten.
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonStart5() As String
        Get
            Return "BWStartTCPReader_RunWorkerCompleted: Es ist ein TCP/IP Fehler aufgetreten."
        End Get
    End Property

    ''' <summary>
    ''' Die TCP-Verbindung zum Fritz!Box Anrufmonitor wurde verloren.
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonTimer4() As String
        Get
            Return "Die TCP-Verbindung zum Fritz!Box Anrufmonitor wurde verloren."
        End Get
    End Property

    ''' <summary>
    ''' Fritz!Box nach einem Verbindungsverlust noch nicht verfügbar.
    ''' </summary>
    Public Shared ReadOnly Property P_ReStart_Log_Timer1() As String
        Get
            Return P_Def_FritzBoxName & " nach einem Verbindungsverlust noch nicht verfügbar."
        End Get
    End Property

    ''' <summary>
    ''' Fritz!Box nach Verbindungsverlust wieder verfügbar. Initialisiere Anrufmonitor...
    ''' </summary>
    Public Shared ReadOnly Property P_ReStart_Log_Timer2() As String
        Get
            Return P_Def_FritzBoxName & " nach Verbindungsverlust wieder verfügbar. Initialisiere Anrufmonitor..."
        End Get
    End Property

    ''' <summary>
    ''' Reaktivierung des Anrufmonitors nach einem Verbindungsverlust nicht erfolgreich.
    ''' </summary>
    Public Shared ReadOnly Property P_ReStart_Log_Timer3() As String
        Get
            Return "Reaktivierung des Anrufmonitors nach einem Verbindungsverlust nicht erfolgreich."
        End Get
    End Property

    ''' <summary>
    ''' Anrufmonitor nach einem Verbindungsverlust gestartet
    ''' </summary>
    Public Shared ReadOnly Property P_ReStart_Log_Timer4() As String
        Get
            Return "Anrufmonitor nach einem Verbindungsverlust gestartet."
        End Get
    End Property

    ''' <summary>
    ''' Auswertung der Anrufliste nach dem Aufwachen aus Standby gestartet.
    ''' </summary>
    Public Shared ReadOnly Property P_ReStart_Log_Timer5() As String
        Get
            Return "Auswertung der Anrufliste nach einem Verbindungsverlust gestartet."
        End Get
    End Property

    ''' <summary>
    ''' Welcome to Phoner
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_AnrMonPhonerWelcome() As String
        Get
            Return "Welcome to Phoner"
        End Get
    End Property
    ''' <summary>
    ''' Sorry, too many clients
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_AnrMonPhonerError() As String
        Get
            Return "Sorry, too many clients"
        End Get
    End Property

    ''' <summary>
    ''' AnrMonAktion, Phoner: "Sorry, too many clients"
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonPhoner1() As String
        Get
            Return "AnrMonAktion, Phoner: """ & P_AnrMon_AnrMonPhonerError & """"
        End Get
    End Property

    ''' <summary>
    ''' AnrMonRING/CALL: Kontakt kann nicht angezeigt werden. Grund: %ErrMsg
    ''' </summary>
    ''' <param name="Fkt">Funktionsname</param>
    ''' <param name="ErrMsg">Fehlermeldung</param>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMon1(ByVal Fkt As String, ByVal ErrMsg As String) As String
        Get
            Return Fkt & ": Kontakt kann nicht angezeigt werden. Grund: " & ErrMsg
        End Get
    End Property

    ''' <summary>
    ''' AnrMonRING/CALL: Das Telefonat mit der ID%ID existiert bereits in der Liste.
    ''' </summary>
    ''' <param name="Fkt">Funktionsname</param>
    ''' <param name="ID">ID des Telefonats</param>
    Public Shared ReadOnly Property P_AnrMon_Log_TelList1(ByVal Fkt As String, ByVal ID As String) As String
        Get
            Return Fkt & ": Das Telefonat mit der ID" & ID & " existiert bereits in der Liste."
        End Get
    End Property

    ''' <summary>
    ''' StoppUhr wird eingeblendet.
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonStoppUhr1() As String
        Get
            Return "StoppUhr wird eingeblendet."
        End Get
    End Property

    ''' <summary>
    ''' Integrierte Faxfunktion wurde erkannt: Stoppuhr nicht eingeblendet.
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonStoppUhr2() As String
        Get
            Return "Integrierte Faxfunktion wurde erkannt: Stoppuhr nicht eingeblendet."
        End Get
    End Property

    'Public  Shared ReadOnly Property P_AnrMon_Journal_Def_Categories As String()
    '    Get
    '        Dim tmp() As String = {"FritzBox Anrufmonitor", "Telefonanrufe"}
    '        Return tmp
    '    End Get
    'End Property
    Public Shared ReadOnly Property P_AnrMon_Journal_Def_Categories() As ReadOnlyCollection(Of String)
        Get
            Dim tmp() As String = {"FritzBox Anrufmonitor", "Telefonanrufe"}
            Return New ReadOnlyCollection(Of String)(tmp)
        End Get
    End Property

    ''' <summary>
    ''' Kontaktdaten:
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Journal_Kontaktdaten As String
        Get
            Return "Kontaktdaten:"
        End Get
    End Property

    ''' <summary>
    ''' Kontaktdaten (vCard):
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_AnrMonDISCONNECT_Journal As String
        Get
            Return "Kontaktdaten (vCard):"
        End Get
    End Property

    ''' <summary>
    ''' Ein unvollständiges Telefonat wurde registriert.
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_AnrMonDISCONNECT_Error As String
        Get
            Return "Ein unvollständiges Telefonat wurde registriert."
        End Get
    End Property

    ''' <summary>
    ''' Tel.-Nr.: TelNr Status: (nicht) angenommen    
    ''' </summary>
    ''' <param name="TelNr">Tekefonnummer</param>
    ''' <param name="Angenommen">Boolean, ob das Telefon angenommen wurde oder nicht</param>
    Public Shared ReadOnly Property P_AnrMon_AnrMonDISCONNECT_JournalBody(ByVal TelNr As String, ByVal Angenommen As Boolean) As String
        Get
            Return P_AnrMon_AnrMonDISCONNECT_JournalTelNr & TelNr & P_Def_EineNeueZeile & "Status: " & CStr(IIf(Angenommen, P_Def_LeerString, "nicht ")) & "angenommen" & P_Def_EineNeueZeile & P_Def_EineNeueZeile
        End Get
    End Property

    ''' <summary>
    '''  Tel.-Nr.: 
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_AnrMonDISCONNECT_JournalTelNr() As String
        Get
            Return "Tel.-Nr.: "
        End Get
    End Property

    'Anrufmonitor - PopUp
    ''' <summary>
    ''' Kontakt öffnen
    ''' </summary>
    ''' <value>Kontakt öffnen</value>
    ''' <returns>Kontakt öffnen</returns>
    Public Shared ReadOnly Property P_AnrMon_PopUp_ToolStripMenuItemKontaktöffnen As String
        Get
            Return "Kontakt öffnen"
        End Get
    End Property

    ''' <summary>
    ''' Kontakt erstellen
    ''' </summary>
    ''' <value>Kontakt erstellen</value>
    ''' <returns>Kontakt erstellen</returns>
    Public Shared ReadOnly Property P_AnrMon_PopUp_ToolStripMenuItemKontaktErstellen As String
        Get
            Return "Kontakt erstellen"
        End Get
    End Property
    ''' <summary>
    ''' Rückruf
    ''' </summary>
    ''' <value>Rückruf</value>
    ''' <returns>Rückruf</returns>
    Public Shared ReadOnly Property P_AnrMon_PopUp_ToolStripMenuItemRückruf As String
        Get
            Return "Rückruf"
        End Get
    End Property

    ''' <summary>
    ''' In Zwischenablage kopieren
    ''' </summary>
    ''' <value>In Zwischenablage kopieren</value>
    ''' <returns>In Zwischenablage kopieren</returns>
    Public Shared ReadOnly Property P_AnrMon_PopUp_ToolStripMenuItemKopieren As String
        Get
            Return "In Zwischenablage kopieren"
        End Get
    End Property

    ' Fritz!Box
    ''' <summary>
    ''' Die Fritz!Box lässt keinen weiteren Anmeldeversuch in den nächsten " &amp; Blocktime &amp; " Sekunden zu.  Versuchen Sie es später erneut.
    ''' </summary>
    ''' <param name="Blocktime"></param>
    Public Shared ReadOnly Property P_FritzBox_LoginError_Blocktime(ByVal Blocktime As String) As String
        Get
            Return "Die Fritz!Box lässt keinen weiteren Anmeldeversuch in den nächsten " & Blocktime & " Sekunden zu.  Versuchen Sie es später erneut."
        End Get
    End Property

    ''' <summary>
    ''' Die Fritz!Box benötigt kein Passwort. Das AddIn wird nicht funktionieren.
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_LoginError_MissingPassword As String
        Get
            Return "Die Fritz!Box benötigt kein Passwort. Das AddIn wird nicht funktionieren."
        End Get
    End Property

    ''' <summary>
    ''' Es fehlt die Berechtigung für den Zugriff auf die Fritz!Box. Benutzer: &amp; FBBenutzer
    ''' </summary>
    ''' <param name="FBBenutzer">Nutzername</param>
    Public Shared ReadOnly Property P_FritzBox_LoginError_MissingRights(ByVal FBBenutzer As String) As String
        Get
            Return "Es fehlt die Berechtigung für den Zugriff auf die Fritz!Box. Benutzer: " & FBBenutzer
        End Get
    End Property

    ''' <summary>
    ''' as Passwort zur Fritz!Box kann nicht entschlüsselt werden, da das verschlüsselte Passwort und/oder der Zugangsschlüssel fehlt.
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_LoginError_MissingData() As String
        Get
            Return "Das Passwort zur Fritz!Box kann nicht entschlüsselt werden, da das verschlüsselte Passwort und/oder der Zugangsschlüssel fehlt."
        End Get
    End Property

    ''' <summary>
    ''' Die Anmeldedaten sind falsch.
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_LoginError_LoginIncorrect() As String
        Get
            Return "Die Anmeldedaten sind falsch."
        End Get
    End Property

    ''' <summary>
    ''' Eine gültige SessionID ist bereits vorhanden: &amp; SID
    ''' </summary>
    ''' <param name="SID"></param>


    Public Shared ReadOnly Property P_FritzBox_LoginInfo_SID(ByVal SID As String) As String
        Get
            Return "Eine gültige SessionID ist bereits vorhanden: " & SID
        End Get
    End Property

    ''' <summary>
    ''' Sie haben sich erfolgreich von der FRITZ!Box abgemeldet.
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_LogoutTestString1 As String
        Get
            Return "Sie haben sich erfolgreich von der FRITZ!Box abgemeldet."
        End Get
    End Property

    ''' <summary>
    ''' Sie haben sich erfolgreich von der Benutzeroberfläche Ihrer FRITZ!Box abgemeldet.
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_LogoutTestString2 As String
        Get
            Return "Sie haben sich erfolgreich von der Benutzeroberfläche Ihrer FRITZ!Box abgemeldet."
        End Get
    End Property

    ''' <summary>
    ''' Logout eventuell NICHT erfolgreich!
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_LogoutError As String
        Get
            Return "Logout eventuell NICHT erfolgreich!"
        End Get
    End Property

    ' Telefone
    ''' <summary>
    ''' Fehler bei dem Herunterladen der Telefone: Die Anmeldedaten sind falsch oder es fehlt die Berechtigung für diesen Bereich.
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Tel_Error1 As String
        Get
            Return "Fehler bei dem Herunterladen der Telefone: Die Anmeldedaten sind falsch oder es fehlt die Berechtigung für diesen Bereich."
        End Get
    End Property

    ''' <summary>
    ''' Fehler bei dem Herunterladen der Telefone: Telefonieseite kann nicht gelesen werden.
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Tel_Error2 As String
        Get
            Return "Fehler bei dem Herunterladen der Telefone: Telefonieseite kann nicht gelesen werden."
        End Get
    End Property

    ''' <summary>
    ''' Ausleseroutine für &amp; P_Def_FritzBoxName &amp; bis Firmware 5.25 gestartet...
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Tel_RoutineBis525 As String
        Get
            Return " Ausleseroutine für " & P_Def_FritzBoxName & " bis Firmware 5.25 gestartet..."
        End Get
    End Property

    ''' <summary>
    ''' " Ausleseroutine für " &amp; P_Def_FritzBoxName &amp; " ab Firmware 5.25 bis Firmware 6.05 gestartet..."
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Tel_RoutineAb525 As String
        Get
            Return " Ausleseroutine für " & P_Def_FritzBoxName & " ab Firmware 5.26 bis Firmware 6.04 gestartet..."
        End Get
    End Property

    ''' <summary>
    ''' Ausleseroutine für &amp; P_Def_FritzBoxName &amp; ab Firmware 6.05 gestartet...
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Tel_RoutineAb605 As String
        Get
            Return " Ausleseroutine für " & P_Def_FritzBoxName & " ab Firmware 6.05 gestartet..."
        End Get
    End Property

    ''' <summary>
    ''' Bitte sende diese Datei an folgende E-Mail: &amp; P_AddinKontaktMail
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Tel_DebugMsgAb605 As String
        Get
            Return "Bitte sende diese Datei an folgende E-Mail: " & P_AddinKontaktMail
        End Get
    End Property

    ''' <summary>
    ''' E-Mail kruemelino@gert-michael.de
    ''' </summary>
    Public Shared ReadOnly Property P_AddinKontaktMail As String
        Get
            Return "kruemelino@gert-michael.de"
        End Get
    End Property

    ''' <summary>
    ''' Fritz!Box Telefon Quelldatei: http://" &amp; C_DP.P_ValidFBAdr &amp; "/cgi-bin/webcm?sid=" &amp; SID &amp; "&amp;getpage=../html/de/menus/menu2.html&amp;var:lang=de&amp;var:menu=fon&amp;var:pagename=fondevices
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Tel_AlteRoutine2(ByVal Link As String) As String
        Get
            Return P_Def_FritzBoxName & " Telefon Quelldatei: " & Link
        End Get
    End Property

    ''' <summary>
    ''' Fehler beim Herunterladen der Telefone. Anmeldedaten korrekt?
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Tel_ErrorAlt1 As String
        Get
            Return "Fehler beim Herunterladen der Telefone. Anmeldedaten korrekt?"
        End Get
    End Property

    ''' <summary>
    ''' Telefonnummer gefunden: Typ+i, TelNr
    ''' </summary>
    ''' <param name="Typ">Telefonnummerntyp</param>
    ''' <param name="idx">Nummer der Telefonnummer</param>
    ''' <param name="TelNr">Telefonnummer</param>
    Public Shared ReadOnly Property P_FritzBox_Tel_NrFound(ByVal Typ As String, ByVal idx As Integer, ByVal TelNr As String) As String
        Get
            Return "Telefonnummer gefunden: " & Typ & idx & ", " & TelNr
        End Get
    End Property

    ''' <summary>
    ''' Telefoniegerät gefunden: Typ+Dialport, TelNr, TelName
    ''' </summary>
    ''' <param name="Typ">Telefontyp (DECT, FON, FAX, TAM, S0, ...)</param>
    ''' <param name="Dialport">Dialport</param>
    ''' <param name="TelNr">Telefonnummer</param>
    ''' <param name="TelName">Telefonname</param>
    Public Shared ReadOnly Property P_FritzBox_Tel_DeviceFound(ByVal Typ As String, ByVal Dialport As String, ByVal TelNr As String, ByVal TelName As String) As String
        Get
            Return "Telefoniegerät gefunden: " & Typ & CStr(Dialport) & ", " & TelNr & ", " & TelName
        End Get
    End Property

    ''' <summary>
    ''' "Telefoniegerät: " &amp; TelName &amp; " (" &amp; Dialport &amp; ") ist ein FAX."
    ''' </summary>
    ''' <param name="Dialport">Dialport</param>
    ''' <param name="TelName">Telefonname</param>
    Public Shared ReadOnly Property P_FritzBox_Tel_DeviceisFAX(ByVal Dialport As String, ByVal TelName As String) As String
        Get
            Return "Telefoniegerät: " & TelName & " (" & Dialport & ") ist ein FAX."
        End Get
    End Property

    ''' <summary>
    ''' "Sende " &amp; i &amp; ". von insgesamt " &amp; n &amp; " Hauptanfragen an Fritz!Box..."
    ''' </summary>
    ''' <param name="i">Nummer der aktuellen Anfrage</param>
    ''' <param name="n">Gesamtanzahl an Anfragen</param>
    ''' <returns></returns>
    Public Shared ReadOnly Property P_FritzBox_Tel_SendQuery(ByVal i As Integer, ByVal n As Integer) As String
        Get
            Return "Sende " & i & ". von insgesamt " & n & " Hauptanfragen an die " & P_Def_FritzBoxName & "..."
        End Get
    End Property

    'Wählen (Fritz!Box)
    ''' <summary>
    ''' Fehler! Entwickler kontaktieren.
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Dial_Error1 As String
        Get
            Return "Fehler!" & P_Def_EineNeueZeile & "Entwickler kontaktieren."
        End Get
    End Property

    ''' <summary>
    ''' Fehler! Logfile beachten!
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Dial_Error2 As String
        Get
            Return "Fehler!" & P_Def_EineNeueZeile & "Logfile beachten!"
        End Get
    End Property

    ''' <summary>
    ''' Fehler bei dem Login. SessionID: SID 
    ''' </summary>
    ''' <param name="SID">SessionID</param>
    Public Shared ReadOnly Property P_FritzBox_Dial_Error3(ByVal SID As String) As String
        Get
            Return "Fehler bei dem Login. SessionID: " & SID & "!"
        End Get
    End Property

    ''' <summary>
    ''' Verbindungsaufbau wurde abgebrochen!
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Dial_HangUp As String
        Get
            Return "Verbindungsaufbau" & P_Def_EineNeueZeile & "wurde abgebrochen!"
        End Get
    End Property

    ''' <summary>
    ''' Wähle DialCode Jetzt abheben!
    ''' </summary>
    ''' <param name="DialCode"></param>
    Public Shared ReadOnly Property P_FritzBox_Dial_Start(ByVal DialCode As String) As String
        Get
            Return "Wähle " & DialCode & P_Def_EineNeueZeile & "Jetzt abheben!"
        End Get
    End Property

    'Journalimport (Fritz!Box)
    ''' <summary>
    ''' Der Login in die Fritz!Box ist fehlgeschlagen Die Anmeldedaten sind falsch oder es fehlt die Berechtigung für diesen Bereich.
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_JI_Error1 As String
        Get
            Return "Der Login in die " & P_Def_FritzBoxName & " ist fehlgeschlagen. Die Anmeldedaten sind falsch oder es fehlt die Berechtigung für diesen Bereich."
        End Get
    End Property

    'Information
    Public Shared ReadOnly Property P_FritzBox_Info(ByVal FBTyp As String, ByVal FBFirmware As String) As String
        Get
            Return String.Concat("Ergänze bitte folgende Angaben:", P_Def_ZweiNeueZeilen, "Dein Name:", P_Def_EineNeueZeile, "Problembeschreibung:", P_Def_EineNeueZeile, "Datum & Uhrzeit: ", System.DateTime.Now, P_Def_EineNeueZeile, P_Def_FritzBoxName & "-Typ: ", FBTyp, P_Def_EineNeueZeile, "Firmware: ", FBFirmware, P_Def_EineNeueZeile)
        End Get
    End Property

    'Initialisierung

    ''' <summary>
    ''' "Fritz!Box unter der IP IPAdresse gefunden"
    ''' </summary>
    ''' <param name="IPAdresse"></param>
    Public Shared ReadOnly Property P_Init_FritzBox_Found(ByVal IPAdresse As String) As String
        Get
            Return P_Def_FritzBoxName & " unter der IP " & IPAdresse & " gefunden"
        End Get
    End Property

    ''' <summary>
    '''"Keine Fritz!Box unter der angegebenen IP gefunden.
    ''' </summary>
    Public Shared ReadOnly Property P_Init_FritzBox_NotFound() As String
        Get
            Return "Keine " & P_Def_FritzBoxName & " unter der angegebenen IP gefunden."
        End Get
    End Property

    ''' <summary>
    ''' Keine Gegenstelle unter der angegebenen IP gefunden.
    ''' </summary>
    Public Shared ReadOnly Property P_Init_NotthingFound() As String
        Get
            Return "Keine Gegenstelle unter der angegebenen IP gefunden."
        End Get
    End Property

    ''' <summary>
    ''' Das Anmelden an der Fritz!Box war erfolgreich.
    ''' </summary>
    Public Shared ReadOnly Property P_Init_Login_Korrekt() As String
        Get
            Return "Das Anmelden an der " & P_Def_FritzBoxName & " war erfolgreich."
        End Get
    End Property

    ''' <summary>
    ''' Das Anmelden an der Fritz!Box war erfolgreich.
    ''' </summary>
    Public Shared ReadOnly Property P_Init_Login_Nicht_Korrekt() As String
        Get
            Return "Die Anmeldedaten sind falsch oder es fehlt die Berechtigung."
        End Get
    End Property

    ''' <summary>
    ''' Bitte warten...
    ''' </summary>
    Public Shared ReadOnly Property P_Def_Bitte_Warten() As String
        Get
            Return "Bitte warten..."
        End Get
    End Property

    ''' <summary>
    ''' Zeit: sZeit P_Def_NeueZeile  Telefonnummer: sTelNr
    ''' </summary>
    ''' <param name="sZeit">Zeit</param>
    ''' <param name="sTelNr">Telefonnummer</param>
    Public Shared ReadOnly Property P_CMB_ToolTipp(ByVal sZeit As String, ByVal sTelNr As String) As String
        Get
            Return "Zeit: " & sZeit & P_Def_EineNeueZeile & "Telefonnummer: " & sTelNr
        End Get
    End Property

    ''' <summary>
    ''' Wählen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Dial() As String
        Get
            Return "Wählen"
        End Get
    End Property

    ''' <summary>
    ''' Wahlwiederholung
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_WWDH() As String
        Get
            Return "Wahlwiederholung"
        End Get
    End Property

    ''' <summary>
    ''' Direktwahl
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Direktwahl() As String
        Get
            Return "Direktwahl"
        End Get
    End Property

    ''' <summary>
    ''' Anrufmonitor
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMon() As String
        Get
            Return "Anrufmonitor"
        End Get
    End Property

    ''' <summary>
    ''' Anzeigen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMonAnzeigen() As String
        Get
            Return "Anzeigen"
        End Get
    End Property

    ''' <summary>
    ''' Anrufmonitor neustarten
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMonNeuStart() As String
        Get
            Return "Anrufmonitor neustarten"
        End Get
    End Property

    ''' <summary>
    ''' Rückruf
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_CallBack() As String
        Get
            Return "Rückruf"
        End Get
    End Property

    ''' <summary>
    ''' VIP-Liste
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP() As String
        Get
            Return "VIP-Liste"
        End Get
    End Property

    ''' <summary>
    ''' Journalimport
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Journal() As String
        Get
            Return "Journalimport"
        End Get
    End Property

    ''' <summary>
    ''' Einstellungen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Setup() As String
        Get
            Return "Einstellungen"
        End Get
    End Property

    ''' <summary>
    ''' Liste löschen...
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_ClearList() As String
        Get
            Return "Liste löschen..."
        End Get
    End Property

    ''' <summary>
    ''' Liste löschen...
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_ClearEntry() As String
        Get
            Return "Eintrag löschen..."
        End Get
    End Property

    ''' <summary>
    ''' Öffnet den Wahldialog um das ausgewählte Element anzurufen.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Dial_ToolTipp() As String
        Get
            Return "Öffnet den Wahldialog um das ausgewählte Element anzurufen"
        End Get
    End Property

    ''' <summary>
    ''' Öffnet den Wahldialog für die Wahlwiederholung
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_WWDH_ToolTipp() As String
        Get
            Return "Öffnet den Wahldialog für die Wahlwiederholung"
        End Get
    End Property

    ''' <summary>
    ''' Startet den Anrufmonitor.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMon_ToolTipp() As String
        Get
            Return "Startet den Anrufmonitor"
        End Get
    End Property

    ''' <summary>
    ''' Öffnet den Wahldialog für die Direktwahl
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Direktwahl_ToolTipp() As String
        Get
            Return "Öffnet den Wahldialog für die Direktwahl"
        End Get
    End Property

    ''' <summary>
    ''' Zeigt den letzten Anruf an
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMonAnzeigen_ToolTipp() As String
        Get
            Return "Zeigt den letzten Anruf an"
        End Get
    End Property

    ''' <summary>
    ''' Startet den Anrufmonitor neu
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMonNeuStart_ToolTipp() As String
        Get
            Return "Startet den Anrufmonitor neu"
        End Get
    End Property

    ''' <summary>
    ''' Öffnet den Wahldialog für den Rückruf
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_CallBack_ToolTipp() As String
        Get
            Return "Öffnet den Wahldialog für den Rückruf"
        End Get
    End Property

    ''' <summary>
    ''' Öffnet den Wahldialog um einen VIP anzurufen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP_ToolTipp() As String
        Get
            Return "Öffnet den Wahldialog um einen VIP anzurufen"
        End Get
    End Property

    ''' <summary>
    ''' Die VIP-Liste ist mit 10 Einträgen bereits voll.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP_O11_Voll_ToolTipp() As String
        Get
            Return "Die VIP-Liste ist mit 10 Einträgen bereits voll."
        End Get
    End Property

    ''' <summary>
    ''' Füge diesen Kontakt der VIP-Liste hinzu.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP_Hinzufügen_ToolTipp() As String
        Get
            Return "Füge diesen Kontakt der VIP-Liste hinzu."
        End Get
    End Property

    ''' <summary>
    ''' Entfernt diesen Kontakt von der VIP-Liste.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP_Entfernen_ToolTipp() As String
        Get
            Return "Entfernt diesen Kontakt von der VIP-Liste."
        End Get
    End Property

    ''' <summary>
    ''' Importiert die Anrufliste der Fritz!Box als Journaleinträge
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Journal_ToolTipp() As String
        Get
            Return "Importiert die Anrufliste der " & P_Def_FritzBoxName & " als Journaleinträge"
        End Get
    End Property

    ''' <summary>
    ''' Öffnet die Fritz!Box Telefon-dingsbums Einstellungen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Setup_ToolTipp() As String
        Get
            Return "Öffnet den " & P_Def_Addin_LangName & " Einstellungsdialog"
        End Get
    End Property

    ''' <summary>
    ''' VIP
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_VIP() As String
        Get
            Return "VIP"
        End Get
    End Property

    ''' <summary>
    ''' Upload
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_Upload() As String
        Get
            Return "Upload"
        End Get
    End Property

    ''' <summary>
    ''' Anrufen (Fritz!Box Telefon-Dingsbums)
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_ContextMenueItemCall() As String
        Get
            Return "Anrufen (" & P_Def_Addin_LangName & ")"
        End Get
    End Property

    ''' <summary>
    ''' VIP (Fritz!Box Telefon-Dingsbums)
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_ContextMenueItemVIP() As String
        Get
            Return P_CMB_Insp_VIP & " (" & P_Def_Addin_LangName & ")"
        End Get
    End Property

    ''' <summary>
    ''' Upload (Fritz!Box Telefon-Dingsbums)
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_ContextMenueItemUpload() As String
        Get
            Return P_CMB_Insp_Upload & " (" & P_Def_Addin_LangName & ")"
        End Get
    End Property

    ''' <summary>
    ''' Rückwärtssuche
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_RWS() As String
        Get
            Return "Rückwärtssuche"
        End Get
    End Property

    ''' <summary>
    ''' Suchen Sie zusätzliche Informationen zu diesem Anrufer mit der Rückwärtssuche.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_RWS_ToolTipp() As String
        Get
            Return "Suchen Sie zusätzliche Informationen zu diesem Anrufer mit der Rückwärtssuche"
        End Get
    End Property

    ''' <summary>
    ''' Notiz
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_Note() As String
        Get
            Return "Notiz"
        End Get
    End Property

    ''' <summary>
    ''' Einen Notizeintrag hinzufügen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_Note_ToolTipp() As String
        Get
            Return "Einen Notizeintrag hinzufügen"
        End Get
    End Property

    ''' <summary>
    ''' Fritz!Box Telefonbuch
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Expl_Adrbk() As String
        Get
            Return P_Def_FritzBoxName & " Telefonbuch"
        End Get
    End Property

    ''' <summary>
    ''' Einen Notizeintrag hinzufügen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_UploadKontakt_ToolTipp() As String
        Get
            Return "Lädt diesen Kontakt auf die " & P_Def_FritzBoxName & " hoch."
        End Get
    End Property

    ''' <summary>          
    ''' Kontakt erstellen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Kontakt_Erstellen() As String
        Get
            Return "Kontakt erstellen"
        End Get
    End Property

    ''' <summary>
    ''' Erstellt einen Kontakt aus einem Journaleintrag
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Kontakt_Erstellen_ToolTipp() As String
        Get
            Return "Erstellt einen Kontakt aus einem Journaleintrag"
        End Get
    End Property

    ''' <summary>
    ''' Kontakt anzeigen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Kontakt_Anzeigen() As String
        Get
            Return "Kontakt anzeigen"
        End Get
    End Property

    ''' <summary>
    ''' Zeigt den Kontakt zu diesem Journaleintrag an
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Kontakt_Anzeigen_ToolTipp() As String
        Get
            Return "Zeigt den Kontakt zu diesem Journaleintrag an"
        End Get
    End Property

    ''' <summary>
    ''' Der verknüpfte Kontakt kann nicht gefunden werden! Erstelle einen neuen Kontakt aus diesem Journaleintrag.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Kontakt_Anzeigen_Error_ToolTipp() As String
        Get
            Return "Der verknüpfte Kontakt kann nicht gefunden werden! Erstelle einen neuen Kontakt aus diesem Journaleintrag."
        End Get
    End Property

    ' Rückwärtssuche

    ''' <summary>
    ''' 11880
    ''' </summary>
    Public Shared ReadOnly Property P_RWS11880_Name() As String
        Get
            Return "11880"
        End Get
    End Property

    ''' <summary>
    ''' dasÖrtliche
    ''' </summary>
    Public Shared ReadOnly Property P_RWSDasOertliche_Name() As String
        Get
            Return "dasÖrtliche"
        End Get
    End Property

    ''' <summary>
    ''' dasTelefonbuch
    ''' </summary>
    Public Shared ReadOnly Property P_RWSDasTelefonbuch_Name() As String
        Get
            Return "dasTelefonbuch"
        End Get
    End Property

    ''' <summary>
    ''' tel.search.ch
    ''' </summary>
    Public Shared ReadOnly Property P_RWSTelSearch_Name() As String
        Get
            Return "tel.search.ch"
        End Get
    End Property

    ''' <summary>
    ''' Alle
    ''' </summary>
    Public Shared ReadOnly Property P_RWSAlle_Name() As String
        Get
            Return "Alle"
        End Get
    End Property

    ''' <summary>
    ''' www.11880.com
    ''' </summary>
    Public Shared ReadOnly Property P_RWS11880_Link() As String
        Get
            Return "www.11880.com"
        End Get
    End Property

    ''' <summary>
    ''' www.dasoertliche.de
    ''' </summary>
    Public Shared ReadOnly Property P_RWSDasOertliche_Link() As String
        Get
            Return "www.dasoertliche.de"
        End Get
    End Property

    ''' <summary>
    ''' www.dastelefonbuch.de
    ''' </summary>
    Public Shared ReadOnly Property P_RWSDasTelefonbuch_Link() As String
        Get
            Return "www.dastelefonbuch.de"
        End Get
    End Property

    ''' <summary>
    ''' tel.search.ch
    ''' </summary>
    Public Shared ReadOnly Property P_RWSTelSearch_Link() As String
        Get
            Return "tel.search.ch"
        End Get
    End Property

    ''' <summary>
    ''' Rückwärtssuche mit <c>Link</c> 
    ''' </summary>
    ''' <param name="Link">Link der eingefügt werden soll</param>
    Public Shared ReadOnly Property P_RWS_ToolTipp(ByVal Link As String) As String
        Get
            Return "Rückwärtssuche mit &#34;" & Link & "&#34;"
        End Get
    End Property

    ''' <summary>
    ''' Rückwärtssuche mit allen Anbietern
    ''' </summary>
    Public Shared ReadOnly Property P_RWS_ToolTipp() As String
        Get
            Return "Rückwärtssuche mit allen Anbietern"
        End Get
    End Property

    ' Inspector Button Tag
    ''' <summary>
    ''' Dial_Tag
    ''' </summary>
    Public Shared ReadOnly Property P_Tag_Insp_Dial() As String
        Get
            Return "Dial_Tag"
        End Get
    End Property

    ''' <summary>
    ''' Kontakt_Tag
    ''' </summary>
    Public Shared ReadOnly Property P_Tag_Insp_Kontakt() As String
        Get
            Return "Kontakt_Tag"
        End Get
    End Property

    ''' <summary>
    '''  Der Kontakt kann angezeigt werden: 
    ''' 
    '''  ErrorMessage
    ''' </summary>
    Public Shared ReadOnly Property P_Fehler_Kontakt_Anzeigen(ByVal ErrorMessage As String) As String
        Get
            Return "Der Kontakt kann angezeigt werden: " & P_Def_EineNeueZeile & P_Def_EineNeueZeile & ErrorMessage
        End Get
    End Property

    ''' <summary>
    ''' "Der Kontakt <c>KontaktName</c> wurde erfolgreich auf die Fritz!Box geladen."
    ''' </summary>
    ''' <param name="KontaktName"></param>
    Public Shared ReadOnly Property P_Kontakt_Hochgeladen(ByVal KontaktName As String) As String
        Get
            Return "Der Kontakt " & KontaktName & " wurde erfolgreich auf die " & P_Def_FritzBoxName & " geladen."
        End Get
    End Property

    ''' <summary>
    ''' Der Kontakt <c>KontaktName</c> konnte nicht auf die Fritz!Box geladen werden."
    ''' </summary>
    ''' <param name="KontaktName"></param>
    Public Shared ReadOnly Property P_Fehler_Kontakt_Hochladen(ByVal KontaktName As String) As String
        Get
            Return "Der Kontakt " & KontaktName & " konnte nicht auf die " & P_Def_FritzBoxName & " geladen werden."
        End Get
    End Property

    ''' <summary>
    ''' "Der Addressbuch der Fritz!Box kann nicht geöffnet werden."
    ''' </summary>
    Public Shared ReadOnly Property P_Fehler_Export_Addressbuch() As String
        Get
            Return "Der Addressbuch der " & P_Def_FritzBoxName & " kann nicht geöffnet werden."
        End Get
    End Property

#End Region

    Public Sub New(ByVal XMLKlasse As XML)

        C_XML = XMLKlasse
        ' Pfad zur Einstellungsdatei ermitteln
        Dim ConfigPfad As String
        P_Arbeitsverzeichnis = GetSettingsVBA("Arbeitsverzeichnis", P_Def_AddInPath)
        ConfigPfad = P_Arbeitsverzeichnis & P_Def_Config_FileName

        'Xml Init
        XMLDoc = New XmlDocument()

        With My.Computer.FileSystem
            If Not (.FileExists(ConfigPfad) AndAlso C_XML.XMLValidator(XMLDoc, ConfigPfad)) Then
                XMLDoc.LoadXml("<?xml version=""1.0"" encoding=""UTF-8""?><" & P_RootName & "/>")
                If Not .DirectoryExists(P_Arbeitsverzeichnis) Then .CreateDirectory(P_Arbeitsverzeichnis)
                .WriteAllText(ConfigPfad, XMLDoc.InnerXml, True)
                SaveSettingsVBA("Arbeitsverzeichnis", P_Arbeitsverzeichnis)
            End If
        End With
        CleanUpXML()

        tSpeichern = New Timer
        With tSpeichern
            .Interval = TimeSpan.FromMinutes(P_SpeicherIntervall).TotalMilliseconds
            .Start()
        End With
        LoadOptionData()
    End Sub

    ''' <summary>
    ''' Initiales Laden der Daten aus der XML-Datei
    ''' </summary>
    Private Sub LoadOptionData()
        Dim xPathTeile As New ArrayList

        Me.P_TBLandesVW = C_XML.Read(XMLDoc, P_Def_Options, "TBLandesVW", P_Def_TBLandesVW)
        Me.P_TBAmt = C_XML.Read(XMLDoc, P_Def_Options, "TBAmt", P_Def_TBAmt)
        Me.P_TBFBAdr = C_XML.Read(XMLDoc, P_Def_Options, "TBFBAdr", P_Def_TBFBAdr)
        Me.P_CBForceFBAddr = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBForceFBAddr", CStr(P_Def_CBForceFBAddr)))
        Me.P_TBBenutzer = C_XML.Read(XMLDoc, P_Def_Options, "TBBenutzer", P_Def_TBBenutzer)
        Me.P_TBPasswort = C_XML.Read(XMLDoc, P_Def_Options, "TBPasswort", P_Def_TBPasswort)
        Me.P_TBVorwahl = C_XML.Read(XMLDoc, P_Def_Options, "TBVorwahl", P_Def_TBVorwahl)
        Me.P_TBNumEntryList = CInt(C_XML.Read(XMLDoc, P_Def_Options, "TBNumEntryList", CStr(P_Def_TBNumEntryList)))
        Me.P_CBoxVorwahl = CInt(C_XML.Read(XMLDoc, P_Def_Options, "CBoxVorwahl", CStr(P_Def_CBoxVorwahl)))
        Me.P_TBEnblDauer = CInt(C_XML.Read(XMLDoc, P_Def_Options, "TBEnblDauer", CStr(P_Def_TBEnblDauer)))
        Me.P_CBAnrMonAuto = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrMonAuto", CStr(P_Def_CBAnrMonAuto)))
        Me.P_TBAnrBeantworterTimeout = CInt(C_XML.Read(XMLDoc, P_Def_Options, "TBAnrBeantworterTimeout", CStr(P_Def_TBAnrBeantworterTimeout)))
        Me.P_TBAnrMonX = CInt(C_XML.Read(XMLDoc, P_Def_Options, "TBAnrMonX", CStr(P_Def_TBAnrMonX)))
        Me.P_TBAnrMonY = CInt(C_XML.Read(XMLDoc, P_Def_Options, "TBAnrMonY", CStr(P_Def_TBAnrMonY)))
        Me.P_CBAnrMonMove = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrMonMove", CStr(P_Def_CBAnrMonMove)))
        Me.P_CBAnrMonTransp = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrMonTransp", CStr(P_Def_CBAnrMonTransp)))
        Me.P_TBAnrMonMoveGeschwindigkeit = CInt(C_XML.Read(XMLDoc, P_Def_Options, "TBAnrMonMoveGeschwindigkeit", CStr(P_Def_TBAnrMonMoveGeschwindigkeit)))
        Me.P_CBoxAnrMonStartPosition = CInt(C_XML.Read(XMLDoc, P_Def_Options, "CBoxAnrMonStartPosition", CStr(P_Def_CBoxAnrMonStartPosition)))
        Me.P_CBoxAnrMonMoveDirection = CInt(C_XML.Read(XMLDoc, P_Def_Options, "CBoxAnrMonMoveDirection", CStr(P_Def_CBoxAnrMonMoveDirection)))
        Me.P_CBAnrMonZeigeKontakt = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrMonZeigeKontakt", CStr(P_Def_CBAnrMonZeigeKontakt)))
        Me.P_CBAnrMonContactImage = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrMonContactImage", CStr(P_Def_CBAnrMonContactImage)))
        Me.P_CBIndexAus = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBIndexAus", CStr(P_Def_CBIndexAus)))
        Me.P_CBShowMSN = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBShowMSN", CStr(P_Def_CBShowMSN)))
        Me.P_CBJournal = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBJournal", CStr(P_Def_CBJournal)))
        Me.P_CBAnrListeUpdateJournal = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrListeUpdateJournal", CStr(P_Def_CBAnrListeUpdateJournal)))
        Me.P_CBAnrListeUpdateCallLists = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrListeUpdateCallLists", CStr(P_Def_CBAnrListeUpdateCallLists)))
        Me.P_CBAnrListeShowAnrMon = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrListeShowAnrMon", CStr(P_Def_CBAnrListeShowAnrMon)))
        Me.P_CBUseAnrMon = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBUseAnrMon", CStr(P_Def_CBUseAnrMon)))
        Me.P_CBCheckMobil = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBCheckMobil", CStr(P_Def_CBCheckMobil)))
        Me.P_CBAutoClose = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAutoClose", CStr(P_Def_CBAutoClose)))
        Me.P_CBAnrMonCloseAtDISSCONNECT = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrMonCloseAtDISSCONNECT", CStr(P_Def_CBAnrMonCloseAtDISSCONNECT)))
        Me.P_CBVoIPBuster = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBVoIPBuster", CStr(P_Def_CBVoIPBuster)))
        Me.P_CBCbCunterbinden = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBCbCunterbinden", CStr(P_Def_CBCbCunterbinden)))
        Me.P_CBCallByCall = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBCallByCall", CStr(P_Def_CBCallByCall)))
        Me.P_CBDialPort = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBDialPort", CStr(P_Def_CBDialPort)))
        Me.P_CBKErstellen = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBKErstellen", CStr(P_Def_CBKErstellen)))
        Me.P_CBLogFile = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBLogFile", CStr(P_Def_CBLogFile)))
        ' Einstellungen für die Symbolleiste laden
        Me.P_CBSymbWwdh = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBSymbWwdh", CStr(P_Def_CBSymbWwdh)))
        Me.P_CBSymbAnrMon = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBSymbAnrMon", CStr(P_Def_CBSymbAnrMon)))
        Me.P_CBSymbAnrMonNeuStart = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBSymbAnrMonNeuStart", CStr(P_Def_CBSymbAnrMonNeuStart)))
        Me.P_CBSymbAnrListe = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBSymbAnrListe", CStr(P_Def_CBSymbAnrListe)))
        Me.P_CBSymbDirekt = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBSymbDirekt", CStr(P_Def_CBSymbDirekt)))
        Me.P_CBSymbRWSuche = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBSymbRWSuche", CStr(P_Def_CBSymbRWSuche)))
        Me.P_TVKontaktOrdnerEntryID = C_XML.Read(XMLDoc, P_Def_Options, "TVKontaktOrdnerEntryID", CStr(P_Def_TVKontaktOrdnerEntryID))
        Me.P_TVKontaktOrdnerStoreID = C_XML.Read(XMLDoc, P_Def_Options, "TVKontaktOrdnerStoreID", CStr(P_Def_TVKontaktOrdnerStoreID))
        Me.P_CBSymbVIP = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBSymbVIP", CStr(P_Def_CBSymbVIP)))
        Me.P_CBSymbJournalimport = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBSymbJournalimport", CStr(P_Def_CBSymbJournalimport)))
        Me.P_CBAutoAnrList = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBJImport", CStr(P_Def_CBJImport)))
        ' Einstellungen füer die Rückwärtssuche laden
        Me.P_CBKHO = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBKHO", CStr(P_Def_CBKHO)))
        Me.P_CBRWS = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBRWS", CStr(P_Def_CBRWS)))
        Me.P_CBRWSIndex = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBRWSIndex", CStr(P_Def_CBRWSIndex)))
        Me.P_ComboBoxRWS = CInt(C_XML.Read(XMLDoc, P_Def_Options, "ComboBoxRWS", CStr(P_Def_ComboBoxRWS)))
        Me.P_CBIndex = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBIndex", CStr(P_Def_CBIndex)))
        ' StoppUhr
        Me.P_CBStoppUhrEinblenden = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBStoppUhrEinblenden", CStr(P_Def_CBStoppUhrEinblenden)))
        Me.P_CBStoppUhrAusblenden = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBStoppUhrAusblenden", CStr(P_Def_CBStoppUhrAusblenden)))
        Me.P_TBStoppUhr = CInt(C_XML.Read(XMLDoc, P_Def_Options, "TBStoppUhr", CStr(P_Def_TBStoppUhr)))
        Me.P_CBStoppUhrX = CInt(C_XML.Read(XMLDoc, P_Def_Options, "CBStoppUhrX", CStr(P_Def_CBStoppUhrX)))
        Me.P_CBStoppUhrY = CInt(C_XML.Read(XMLDoc, P_Def_Options, "CBStoppUhrY", CStr(P_Def_CBStoppUhrY)))
        ' Telefonnummernformatierung
        Me.P_TBTelNrMaske = C_XML.Read(XMLDoc, P_Def_Options, "TBTelNrMaske", P_Def_TBTelNrMaske)
        Me.P_CBTelNrGruppieren = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBTelNrGruppieren", CStr(P_Def_CBTelNrGruppieren)))
        Me.P_CBintl = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBintl", CStr(P_Def_CBintl)))
        Me.P_CBIgnoTelNrFormat = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBIgnoTelNrFormat", CStr(P_Def_CBIgnoTelNrFormat)))
        ' Phoner
        Me.P_CBPhoner = CBool(C_XML.Read(XMLDoc, P_Def_Phoner, "CBPhoner", CStr(P_Def_CBPhoner)))
        Me.P_PhonerVerfügbar = CBool(C_XML.Read(XMLDoc, P_Def_Phoner, "PhonerVerfügbar", CStr(P_Def_PhonerVerfügbar)))
        Me.P_ComboBoxPhonerSIP = CInt(C_XML.Read(XMLDoc, P_Def_Phoner, "ComboBoxPhonerSIP", CStr(P_Def_ComboBoxPhonerSIP)))
        Me.P_CBPhonerAnrMon = CBool(C_XML.Read(XMLDoc, P_Def_Phoner, "CBPhonerAnrMon", CStr(P_Def_CBPhonerAnrMon)))
        Me.P_TBPhonerPasswort = C_XML.Read(XMLDoc, P_Def_Phoner, "TBPhonerPasswort", P_Def_TBPhonerPasswort)
        Me.P_PhonerTelNameIndex = CInt(C_XML.Read(XMLDoc, P_Def_Phoner, "PhonerTelNameIndex", CStr(P_Def_PhonerTelNameIndex)))
        ' Statistik
        Me.P_StatResetZeit = CDate(C_XML.Read(XMLDoc, P_Def_Statistics, "ResetZeit", CStr(P_Def_StatResetZeit)))
        Me.P_StatVerpasst = CInt(C_XML.Read(XMLDoc, P_Def_Statistics, "Verpasst", CStr(P_Def_StatVerpasst)))
        Me.P_StatNichtErfolgreich = CInt(C_XML.Read(XMLDoc, P_Def_Statistics, "Nichterfolgreich", CStr(P_Def_StatNichtErfolgreich)))
        Me.P_StatKontakt = CInt(C_XML.Read(XMLDoc, P_Def_Statistics, "Kontakt", CStr(P_Def_StatKontakt)))
        Me.P_StatJournal = CInt(C_XML.Read(XMLDoc, P_Def_Statistics, "Journal", CStr(P_Def_StatJournal)))
        Me.P_StatOLClosedZeit = CDate(C_XML.Read(XMLDoc, P_Def_Journal, "SchließZeit", CStr(P_Def_StatOLClosedZeit)))
        ' Wählbox
        Me.P_TelAnschluss = CInt(C_XML.Read(XMLDoc, P_Def_Options, "Anschluss", CStr(P_Def_TelAnschluss)))
        Me.P_TelFestnetz = CBool(C_XML.Read(XMLDoc, P_Def_Options, "Festnetz", CStr(P_TelFestnetz)))
        Me.P_TelCLIR = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CLIR", CStr(P_Def_TelCLIR)))
        Me.P_EncodingFritzBox = Text.Encoding.GetEncoding(C_XML.Read(XMLDoc, P_Def_Options, "EncodingFritzBox", P_Def_EncodingFritzBox.HeaderName))
        ' Indizierung
        Me.P_LLetzteIndizierung = CDate(C_XML.Read(XMLDoc, P_Def_Options, "LLetzteIndizierung", CStr(P_Def_LLetzteIndizierung)))
        ' Notiz
        Me.P_CBNote = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBNote", CStr(P_Def_CBNote)))
        ' Fritz!Box Kommunikation
        Me.P_RBFBComUPnP = CBool(C_XML.Read(XMLDoc, P_Def_Options, "RBFBComUPnP", CStr(P_Def_RBFBComUPnP)))

        With xPathTeile
            .Clear()
            .Add("Telefone")
            .Add("Nummern")
            .Add("*")
            .Add("[@Checked=""1""]")
        End With
        SetCLBTelNr(New ReadOnlyCollection(Of String)((From x In Split(C_XML.Read(XMLDoc, xPathTeile, DataProvider.P_Def_ErrorMinusOne_String), ";", , CompareMethod.Text) Select x Distinct).ToArray))

        BWCBox = New BackgroundWorker
        With BWCBox
            .WorkerReportsProgress = False
            .RunWorkerAsync(True)
        End With

    End Sub

    Public Sub SetCLBTelNr(ByVal coll As ReadOnlyCollection(Of String))
        _CLBTelNr = coll
    End Sub

    ''' <summary>
    ''' Speicher Daten, die in den Properties stehen in die XML-String.
    ''' </summary>
    Private Sub SaveOptionData()
        C_XML.Write(XMLDoc, P_Def_Options, "TBLandesVW", Me.P_TBLandesVW)
        C_XML.Write(XMLDoc, P_Def_Options, "TBAmt", Me.P_TBAmt)
        C_XML.Write(XMLDoc, P_Def_Options, "TBFBAdr", Me.P_TBFBAdr)
        C_XML.Write(XMLDoc, P_Def_Options, "CBForceFBAddr", CStr(Me.P_CBForceFBAddr))
        C_XML.Write(XMLDoc, P_Def_Options, "TBBenutzer", Me.P_TBBenutzer)
        C_XML.Write(XMLDoc, P_Def_Options, "TBPasswort", Me.P_TBPasswort)
        C_XML.Write(XMLDoc, P_Def_Options, "TBVorwahl", Me.P_TBVorwahl)
        C_XML.Write(XMLDoc, P_Def_Options, "CBoxVorwahl", CStr(Me.P_CBoxVorwahl))
        C_XML.Write(XMLDoc, P_Def_Options, "TBEnblDauer", CStr(Me.P_TBEnblDauer))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrMonAuto", CStr(Me.P_CBAnrMonAuto))
        C_XML.Write(XMLDoc, P_Def_Options, "TBAnrBeantworterTimeout", CStr(Me.P_TBAnrBeantworterTimeout))
        C_XML.Write(XMLDoc, P_Def_Options, "TBAnrMonX", CStr(Me.P_TBAnrMonX))
        C_XML.Write(XMLDoc, P_Def_Options, "TBAnrMonY", CStr(Me.P_TBAnrMonY))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrMonMove", CStr(Me.P_CBAnrMonMove))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrMonTransp", CStr(Me.P_CBAnrMonTransp))
        C_XML.Write(XMLDoc, P_Def_Options, "TBAnrMonMoveGeschwindigkeit", CStr(Me.P_TBAnrMonMoveGeschwindigkeit))
        C_XML.Write(XMLDoc, P_Def_Options, "CBoxAnrMonStartPosition", CStr(Me.P_CBoxAnrMonStartPosition))
        C_XML.Write(XMLDoc, P_Def_Options, "CBoxAnrMonMoveDirection", CStr(Me.P_CBoxAnrMonMoveDirection))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrMonZeigeKontakt", CStr(Me.P_CBAnrMonZeigeKontakt))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrMonContactImage", CStr(Me.P_CBAnrMonContactImage))
        C_XML.Write(XMLDoc, P_Def_Options, "CBIndexAus", CStr(Me.P_CBIndexAus))
        C_XML.Write(XMLDoc, P_Def_Options, "CBShowMSN", CStr(Me.P_CBShowMSN))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAutoClose", CStr(Me.P_CBAutoClose))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrMonCloseAtDISSCONNECT", CStr(Me.P_CBAnrMonCloseAtDISSCONNECT))
        C_XML.Write(XMLDoc, P_Def_Options, "CBVoIPBuster", CStr(Me.P_CBVoIPBuster))
        C_XML.Write(XMLDoc, P_Def_Options, "CBCbCunterbinden", CStr(Me.P_CBVoIPBuster))
        C_XML.Write(XMLDoc, P_Def_Options, "CBCallByCall", CStr(Me.P_CBCallByCall))
        C_XML.Write(XMLDoc, P_Def_Options, "CBDialPort", CStr(Me.P_CBDialPort))
        C_XML.Write(XMLDoc, P_Def_Options, "CBKErstellen", CStr(Me.P_CBKErstellen))
        C_XML.Write(XMLDoc, P_Def_Options, "CBLogFile", CStr(Me.P_CBLogFile))
        ' Einstellungen für die Symbolleiste laden
        C_XML.Write(XMLDoc, P_Def_Options, "CBSymbWwdh", CStr(Me.P_CBSymbWwdh))
        C_XML.Write(XMLDoc, P_Def_Options, "CBSymbAnrMon", CStr(Me.P_CBSymbAnrMon))
        C_XML.Write(XMLDoc, P_Def_Options, "CBSymbAnrMonNeuStart", CStr(Me.P_CBSymbAnrMonNeuStart))
        C_XML.Write(XMLDoc, P_Def_Options, "CBSymbAnrListe", CStr(Me.P_CBSymbAnrListe))
        C_XML.Write(XMLDoc, P_Def_Options, "CBSymbDirekt", CStr(Me.P_CBSymbDirekt))
        C_XML.Write(XMLDoc, P_Def_Options, "CBSymbRWSuche", CStr(Me.P_CBSymbRWSuche))
        C_XML.Write(XMLDoc, P_Def_Options, "CBSymbVIP", CStr(Me.P_CBSymbVIP))
        C_XML.Write(XMLDoc, P_Def_Options, "CBSymbJournalimport", CStr(Me.P_CBSymbJournalimport))
        C_XML.Write(XMLDoc, P_Def_Options, "CBJImport", CStr(Me.P_CBAutoAnrList))
        ' Einstellungen füer die Rückwärtssuche laden
        C_XML.Write(XMLDoc, P_Def_Options, "CBKHO", CStr(Me.P_CBKHO))
        C_XML.Write(XMLDoc, P_Def_Options, "CBRWS", CStr(Me.P_CBRWS))
        C_XML.Write(XMLDoc, P_Def_Options, "CBRWSIndex", CStr(Me.P_CBRWSIndex))
        C_XML.Write(XMLDoc, P_Def_Options, "TVKontaktOrdnerEntryID", CStr(Me.P_TVKontaktOrdnerEntryID))
        C_XML.Write(XMLDoc, P_Def_Options, "TVKontaktOrdnerStoreID", CStr(Me.P_TVKontaktOrdnerStoreID))
        C_XML.Write(XMLDoc, P_Def_Options, "ComboBoxRWS", CStr(Me.P_ComboBoxRWS))
        C_XML.Write(XMLDoc, P_Def_Options, "CBIndex", CStr(Me.P_CBIndex))
        C_XML.Write(XMLDoc, P_Def_Options, "CBJournal", CStr(Me.P_CBJournal))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrListeUpdateJournal", CStr(Me.P_CBAnrListeUpdateJournal))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrListeUpdateCallLists", CStr(Me.P_CBAnrListeUpdateCallLists))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrListeShowAnrMon", CStr(Me.P_CBAnrListeShowAnrMon))
        C_XML.Write(XMLDoc, P_Def_Options, "CBUseAnrMon", CStr(Me.P_CBUseAnrMon))
        C_XML.Write(XMLDoc, P_Def_Options, "CBCheckMobil", CStr(Me.P_CBCheckMobil))
        'StoppUhr
        C_XML.Write(XMLDoc, P_Def_Options, "CBStoppUhrEinblenden", CStr(Me.P_CBStoppUhrEinblenden))
        C_XML.Write(XMLDoc, P_Def_Options, "CBStoppUhrAusblenden", CStr(Me.P_CBStoppUhrAusblenden))
        C_XML.Write(XMLDoc, P_Def_Options, "TBStoppUhr", CStr(Me.P_TBStoppUhr))
        C_XML.Write(XMLDoc, P_Def_Options, "TBTelNrMaske", Me.P_TBTelNrMaske)
        C_XML.Write(XMLDoc, P_Def_Options, "CBTelNrGruppieren", CStr(Me.P_CBTelNrGruppieren))
        C_XML.Write(XMLDoc, P_Def_Options, "CBintl", CStr(Me.P_CBintl))
        C_XML.Write(XMLDoc, P_Def_Options, "CBIgnoTelNrFormat", CStr(Me.P_CBIgnoTelNrFormat))
        C_XML.Write(XMLDoc, P_Def_Options, "CBStoppUhrX", CStr(Me.P_CBStoppUhrX))
        C_XML.Write(XMLDoc, P_Def_Options, "CBStoppUhrY", CStr(Me.P_CBStoppUhrY))
        ' Phoner
        C_XML.Write(XMLDoc, P_Def_Phoner, "CBPhoner", CStr(Me.P_CBPhoner))
        C_XML.Write(XMLDoc, P_Def_Phoner, "PhonerVerfügbar", CStr(Me.P_PhonerVerfügbar))
        C_XML.Write(XMLDoc, P_Def_Phoner, "ComboBoxPhonerSIP", CStr(Me.P_ComboBoxPhonerSIP))
        C_XML.Write(XMLDoc, P_Def_Phoner, "CBPhonerAnrMon", CStr(Me.P_CBPhonerAnrMon))
        C_XML.Write(XMLDoc, P_Def_Phoner, "TBPhonerPasswort", Me.P_TBPhonerPasswort)
        C_XML.Write(XMLDoc, P_Def_Phoner, "PhonerTelNameIndex", CStr(Me.P_PhonerTelNameIndex))
        ' Statistik
        C_XML.Write(XMLDoc, P_Def_Statistics, "ResetZeit", CStr(Me.P_StatResetZeit))
        C_XML.Write(XMLDoc, P_Def_Statistics, "Verpasst", CStr(Me.P_StatVerpasst))
        C_XML.Write(XMLDoc, P_Def_Statistics, "Nichterfolgreich", CStr(Me.P_StatNichtErfolgreich))
        C_XML.Write(XMLDoc, P_Def_Statistics, "Kontakt", CStr(Me.P_StatKontakt))
        C_XML.Write(XMLDoc, P_Def_Statistics, "Journal", CStr(Me.P_StatJournal))
        C_XML.Write(XMLDoc, P_Def_Journal, "SchließZeit", CStr(Me.P_StatOLClosedZeit))
        ' Wählbox
        C_XML.Write(XMLDoc, P_Def_Options, "Anschluss", CStr(Me.P_TelAnschluss))
        C_XML.Write(XMLDoc, P_Def_Options, "Festnetz", CStr(Me.P_TelFestnetz))
        C_XML.Write(XMLDoc, P_Def_Options, "CLIR", CStr(Me.P_TelCLIR))
        'FritzBox
        C_XML.Write(XMLDoc, P_Def_Options, "EncodingFritzBox", Me.P_EncodingFritzBox.HeaderName)
        'Indizierung
        C_XML.Write(XMLDoc, P_Def_Options, "LLetzteIndizierung", CStr(Me.P_LLetzteIndizierung))
        ' Notiz
        C_XML.Write(XMLDoc, P_Def_Options, "CBNote", CStr(Me.P_CBNote))
        ' Fritz!Box Kommunikation
        C_XML.Write(XMLDoc, P_Def_Options, "RBFBComUPnP", CStr(Me.P_RBFBComUPnP))

        ' Do some Stuff
        XMLDoc.Save(P_Arbeitsverzeichnis & P_Def_Config_FileName)
        SaveSettingsVBA("Arbeitsverzeichnis", P_Arbeitsverzeichnis)
    End Sub

    Protected Overrides Sub Finalize()
        SaveOptionData()
        XMLDoc.Save(P_Arbeitsverzeichnis & P_Def_Config_FileName)
        XMLDoc = Nothing
        If tSpeichern IsNot Nothing Then
            tSpeichern.Stop()
            tSpeichern.Dispose()
            tSpeichern = Nothing
        End If

        If P_Debug_FBFile IsNot Nothing Then
            P_Debug_FBFile.Clear()
            P_Debug_FBFile = Nothing
        End If

        MyBase.Finalize()
    End Sub

#Region "XML"
#Region "Speichern"
    Sub SpeichereXMLDatei()
        SaveOptionData()
    End Sub

    Private Sub tSpeichern_Elapsed(ByVal sender As Object, ByVal e As ElapsedEventArgs) Handles tSpeichern.Elapsed
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
        Dim NnSpcMngr As XmlNamespaceManager = Nothing
        With XMLDoc
            ' Diverse Knoten des Journals löschen
            xPathTeile.Add(P_Def_Journal)
            xPathTeile.Add("SchließZeit")
            xPath = C_XML.CreateXPath(XMLDoc, xPathTeile)
            tmpNode = .SelectSingleNode(xPath, NnSpcMngr)
            xPathTeile.Remove("SchließZeit")
            xPath = C_XML.CreateXPath(XMLDoc, xPathTeile)
            If tmpNode IsNot Nothing Then
                .SelectSingleNode(xPath, NnSpcMngr).RemoveAll()
                .SelectSingleNode(xPath, NnSpcMngr).AppendChild(tmpNode)
            End If
        End With
        NnSpcMngr = Nothing
        xPathTeile.Clear()
        xPathTeile = Nothing
    End Sub
#End Region

#Region "Backgroundworker"
    Private Sub BWCBbox_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BWCBox.DoWork
        Dim tmpVorwahl As String = P_TBLandesVW

        If P_ListeLandesVorwahlen Is Nothing Then
            ' Landesvorwahlen
            _ListeLandesVorwahlen = New ReadOnlyCollection(Of String)((Split(My.Resources.Liste_Landesvorwahlen, vbNewLine, , CompareMethod.Text)).ToArray)
        End If

        If P_ListeOrtsVorwahlenD Is Nothing Then
            ' Ortsvorwahlen Deutschland
            _ListeOrtsVorwahlenD = New ReadOnlyCollection(Of String)((Split(My.Resources.Liste_Ortsvorwahlen_Deutschland, vbNewLine, , CompareMethod.Text)).ToArray)
        End If

        If P_ListeOrtsVorwahlenA Is Nothing Then
            ' Ortsvorwahlen Ausland
            _ListeOrtsVorwahlenA = New ReadOnlyCollection(Of String)((Split(My.Resources.Liste_Ortsvorwahlen_Ausland, vbNewLine, , CompareMethod.Text)).ToArray)
        End If
    End Sub

    Private Sub BWCBbox_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BWCBox.RunWorkerCompleted
        BWCBox = Nothing
    End Sub
#End Region
End Class