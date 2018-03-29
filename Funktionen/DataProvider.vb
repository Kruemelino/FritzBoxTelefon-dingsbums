Imports System.Xml
Imports System.Timers
Imports System.ComponentModel
Imports System.Collections.ObjectModel

Public Class DataProvider
    Private C_XML As XML
    Private WithEvents tSpeichern As Timer

#Region "BackgroundWorker"
    Private WithEvents BWCBox As BackgroundWorker
#End Region

#Region "Konstanten"
    ''' <summary>
    ''' Intervall (in Minuten), in dem die XML-Datei gespeichert wird.
    ''' </summary>
    ''' <value>Double</value>
    ''' <returns>5</returns>
    Private Shared ReadOnly Property P_SpeicherIntervall() As Double = 5.0

    ''' <summary>
    ''' Name des Wurzelknotens der XML-Datei: "FritzOutlookXML"
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>FritzOutlookXML</returns>
    Private Shared ReadOnly Property P_RootName() As String = "FritzOutlookXML"

#End Region

#Region "Value Properties"

    ''' <summary>
    ''' Gibt die im Einstellungsdialog eingegebene Landesvorwahl zurück
    ''' </summary>
    Public Property P_TBLandesVW() As String

    ''' <summary>
    ''' Gibt an, ob eine Amtsholung stets mitgewählt werden soll. Die Amtsholung wird in den Einstellungen festgelegt.
    ''' </summary>
    Public Property P_TBAmt As String

    ''' <summary>
    ''' Eigenschaft für die hinterlege Ortsvorwahl
    ''' </summary>
    Public Property P_TBVorwahl() As String

    ''' <summary>
    ''' Enthält den Index im Combobox
    ''' </summary>
    Public Property P_CBoxVorwahl() As Integer
    Public Property P_TBNumEntryList As Integer

    ' Anrufmonitor
    ''' <summary>
    ''' Gibt an, wie lange der Anrufmonitor angezeigt werden soll, bevor er automatisch ausgeblendet wird
    ''' </summary>
    Public Property P_TBEnblDauer() As Integer

    ''' <summary>
    ''' Gibt an, ob der Anrufmonitor automatisch gestartét werden soll.
    ''' </summary>
    Public Property P_CBAnrMonAuto() As Boolean

    ''' <summary>
    ''' Gibt das Timeout an, ab dem alle Telefonate als verpasst behandelt werden sollen.
    ''' </summary>
    Public Property P_TBAnrBeantworterTimeout() As Double

    ''' <summary>
    ''' Gibt an, um wieviele Punkte der Anrufmonitor in X-Richtung verschoben werden soll.
    ''' </summary>
    Public Property P_TBAnrMonX() As Integer

    ''' <summary>
    ''' Gibt an, um wieviele Punkte der Anrufmonitor in Y-Richtung verschoben werden soll.
    ''' </summary>
    Public Property P_TBAnrMonY() As Integer

    ''' <summary>
    ''' Gibt an ob der Anrufmonitor in den Bildschirm hereingescrollt werden soll.
    ''' </summary>
    Public Property P_CBAnrMonMove() As Boolean

    ''' <summary>
    ''' Gibt an, ob der Anrufmonitor eingeblendet werden soll.
    ''' </summary>
    Public Property P_CBAnrMonTransp() As Boolean

    ''' <summary>
    ''' Gibt die Endposition des Anrufmonitors an.
    ''' </summary>
    ''' <remarks>FritzBoxDial.PopUpAnrMon.eStartPosition</remarks>
    Public Property P_CBoxAnrMonStartPosition() As Integer

    ''' <summary>
    ''' Gibt die Bewegungsrichtung des Anrufmonitors an.
    ''' </summary>
    ''' <remarks>FritzBoxDial.PopUpAnrMon.eMoveDirection</remarks>
    Public Property P_CBoxAnrMonMoveDirection() As Integer

    Public Property P_TBAnrMonMoveGeschwindigkeit() As Integer
    Public Property P_CBAnrMonZeigeKontakt() As Boolean
    Public Property P_CBAnrMonContactImage As Boolean
    Public Property P_CBIndexAus As Boolean
    Public Property P_CBShowMSN As Boolean
    Public Property P_CBAutoClose As Boolean
    Public Property P_CBAnrMonCloseAtDISSCONNECT As Boolean
    Public Property P_CBVoIPBuster As Boolean
    Public Property P_CBCbCunterbinden As Boolean
    Public Property P_CBCallByCall As Boolean
    Public Property P_CBDialPort As Boolean
    Public Property P_CBLogFile As Boolean
    Public Property P_CBSymbWwdh As Boolean
    Public Property P_CBSymbAnrMon As Boolean
    Public Property P_CBSymbAnrMonNeuStart As Boolean
    Public Property P_CBAutoAnrList As Boolean
    ''' <summary>
    ''' Gibt an ob nur der Hauptkontaktordner durchsucht werden muss oder alle möglichen eingebundenen Kontaktordner
    ''' </summary>
    Public Property P_CBKHO As Boolean

    Public Property P_CBRWS As Boolean
    Public Property P_CBKErstellen As Boolean

    Public Property P_CBRWSIndex As Boolean
    Public Property P_ComboBoxRWS As Integer
    Public Property P_TVKontaktOrdnerEntryID As String
    Public Property P_TVKontaktOrdnerStoreID As String

    ''' <summary>
    ''' Gibt an, ob die Indizierung durchgeführt werden soll. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    Public Property P_CBIndex As Boolean

    ''' <summary>
    ''' Gibt an, ob Journaleinträge erstellt werden soll. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    Public Property P_CBJournal As Boolean

    ''' <summary>
    ''' Gibt an, ob bei der Auswertung der Anrufliste die Journaleinträge aktualisiert werden sollen.
    ''' </summary>
    Public Property P_CBAnrListeUpdateJournal As Boolean

    ''' <summary>
    ''' Gibt an, ob bei der Auswertung der Anrufliste die Wahlwiederholungs- und Rückrufliste aktualisiert werden sollen.
    ''' </summary>
    Public Property P_CBAnrListeUpdateCallLists As Boolean

    ''' <summary>
    ''' Gibt an, ob verpasste Anrufe mit Hilfe des Anrufmonitors angezeigt werden sollen.
    ''' </summary>
    Public Property P_CBAnrListeShowAnrMon As Boolean

    Public Property P_CBUseAnrMon As Boolean
    Public Property P_CBCheckMobil As Boolean
    Public Property P_CLBTelNr() As Collection(Of String)

    'StoppUhr
    Public Property P_CBStoppUhrEinblenden As Boolean
    Public Property P_CBStoppUhrAusblenden As Boolean
    Public Property P_TBStoppUhr As Integer
    Public Property P_CBStoppUhrX As Integer
    Public Property P_CBStoppUhrY() As Integer
    Public Property P_CBStoppUhrIgnIntFax As Boolean

    ' Telefonnummernformatierung

    Public Property P_TBTelNrMaske As String
    Public Property P_CBTelNrGruppieren As Boolean
    Public Property P_CBintl As Boolean
    Public Property P_CBIgnoTelNrFormat() As Boolean

    'Phoner

    Public Property P_CBPhoner As Boolean
    Public Property P_PhonerVerfügbar As Boolean
    Public Property P_CBPhonerAnrMon As Boolean
    Public Property P_ComboBoxPhonerSIP() As Integer
    Public Property P_TBPhonerPasswort() As String
    Public Property P_PhonerTelNameIndex As Integer

    ' Statistik

    Public Property P_StatResetZeit As Date
    Public Property P_StatVerpasst As Integer
    Public Property P_StatNichtErfolgreich As Integer
    Public Property P_StatJournal As Integer
    Public Property P_StatKontakt As Integer
    Public Property P_StatOLClosedZeit As Date

    ' Wählbox
    Public Property P_TelAnschluss As Integer
    Public Property P_TelFestnetz As Boolean
    Public Property P_TelCLIR As Boolean

    ' FritzBox

    ''' <summary>
    ''' Gibt die ermittelte Zeichencodierung der Fritzbox wieder. Der Wert wird automatisch ermittelt. 
    ''' </summary>
    Public Property P_EncodingFritzBox As Text.Encoding

    ''' <summary>
    ''' Gibt die eingegebene Fritz!Box IP-Adresse an. Dies ist eine Angabe, die der Nutzer in den Einstellungen ändern kann.
    ''' </summary>
    Public Property P_TBFBAdr As String

    ''' <summary>
    ''' Gibt eine korrekte Fritz!Box IP-Adresse zurück.
    ''' </summary>
    Public Property P_ValidFBAdr As String

    ''' <summary>
    ''' Gibt an, ob eine Verbindung zur Fritz!Box trotz fehlgeschlagenen Pings aufgebaut werden soll.
    ''' </summary>
    Public Property P_CBForceFBAddr As Boolean

    ''' <summary>
    ''' Gibt den einegegebenen Benutzernamen für das Login der Fritz!Box an. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    Public Property P_TBBenutzer As String

    ''' <summary>
    ''' Gibt das eingegebene Passwort für das Login der Fritz!Box an. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    Public Property P_TBPasswort As String

    ' Indizierung
    Public Property P_LLetzteIndizierung As Date

    ' Notiz
    Public Property P_CBNote As Boolean

    ' Einstellungen
    Public Property P_Arbeitsverzeichnis As String

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
    Public Property P_RBFBComUPnP() As Boolean
#End Region

#Region "Global Default Value Properties"
    ''' <summary>
    ''' 00 als String
    ''' </summary>
    ''' <value>00</value>
    ''' <returns>String</returns>
    Public Shared ReadOnly Property P_Def_PreLandesVW() As String = "00"

    ''' <summary>
    ''' -1 als String.
    ''' Default Fehler
    ''' </summary>
    ''' <value>-1</value>
    ''' <returns>String</returns>
    Public Shared ReadOnly Property P_Def_ErrorMinusOne_String() As String = XML.P_Def_ErrorMinusOne_String

    ''' <summary>
    ''' -1 als Integer
    ''' </summary>
    Public Shared ReadOnly Property P_Def_ErrorMinusOne_Integer() As Integer = -1

    ''' <summary>
    ''' -2 als String
    ''' </summary>
    Public Shared ReadOnly Property P_Def_ErrorMinusTwo_String() As String = "-2"

    ''' <summary>
    ''' Leerstring, String.Empty
    ''' </summary>
    Public Shared ReadOnly Property P_Def_LeerString() As String = XML.P_Def_StringEmpty

    ''' <summary>
    ''' vbCrLf
    ''' </summary>
    ''' <value>vbCrLf</value>
    ''' <returns>vbCrLf</returns>
    Public Shared ReadOnly Property P_Def_EineNeueZeile() As String = vbCrLf

    ''' <summary>
    ''' vbCrLf &amp; vbCrLf
    ''' </summary>
    ''' <value>vbCrLf &amp; vbCrLf</value>
    ''' <returns>vbCrLf &amp; vbCrLf</returns>
    Public Shared ReadOnly Property P_Def_ZweiNeueZeilen() As String = P_Def_EineNeueZeile & P_Def_EineNeueZeile

    ''' <summary>
    ''' String: unbekannt
    ''' </summary>
    Public Shared ReadOnly Property P_Def_StringUnknown() As String = "unbekannt"

    ''' <summary>
    ''' fritz.box
    ''' </summary>
    Public Shared ReadOnly Property P_Def_FritzBoxAdress() As String = "fritz.box"

    ''' <summary>
    ''' Der Standarduser bei Anmeldung mit Passwort ohne Benutzername: admin
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property P_Def_FritzBoxUser() As String = "admin"

    ''' <summary>
    ''' 192.168.178.1
    ''' </summary>
    Public Shared ReadOnly Property P_Def_FritzBoxIPAdress() As String = "192.168.178.1"

    ''' <summary>
    ''' Fritz!Box
    ''' </summary>
    Public Shared ReadOnly Property P_Def_FritzBoxName() As String = "Fritz!Box"

    ''' <summary>
    ''' FRITZ!Box_Anrufliste.csv
    ''' </summary>
    Public Shared ReadOnly Property P_Def_AnrListFileName() As String = "FRITZ!Box_Anrufliste.csv"

    ''' <summary>
    ''' #96*5*
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TelCodeActivateFritzBoxCallMonitor() As String = "#96*5*"

    ''' <summary>
    ''' 1012
    ''' </summary>
    Public Shared ReadOnly Property P_DefaultFBAnrMonPort() As Integer = 1012

    ''' <summary>
    ''' 49000
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property P_Port_FB_SOAP() As Integer = 49000

    ''' <summary>
    ''' 49443
    ''' </summary>
    Public Shared ReadOnly Property P_Port_FB_SOAP_SSL() As Integer = 49443

    ''' <summary>
    ''' Der Zahlenwert NULL <code>"0"</code> als String.
    ''' </summary>
    ''' <value>"0"</value>
    ''' <returns>"0"</returns>
    Public Shared ReadOnly Property P_Def_StringNull() As String = "0"

    ''' <summary>
    ''' Das Leerzeichen als <code>Chr(32)</code> als String.
    ''' </summary>
    ''' <value>" "</value>
    ''' <returns>" "</returns>
    Public Shared ReadOnly Property P_Def_Leerzeichen() As String = Chr(32)

    ''' <summary>
    ''' 0000000000000000
    ''' </summary>
    Public Shared ReadOnly Property P_Def_SessionID() As String = "0000000000000000"

    ''' <summary>
    ''' Mozilla/5.0 (compatible; MSIE 7.0; Windows NT 6.0; WOW64; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.0.04506; Media Center PC 5.0; .NET CLR 3.5.21022; .NET CLR 1.1.4322)
    ''' </summary>
    Public Shared ReadOnly Property P_Def_Header_UserAgent() As String = "Mozilla/5.0 (compatible; MSIE 7.0; Windows NT 6.0; WOW64; SLCC1; .NET CLR 2.0.50727; .NET CLR 3.0.04506; Media Center PC 5.0; .NET CLR 3.5.21022; .NET CLR 1.1.4322)"

    ''' <summary>
    ''' application/x-www-form-urlencoded
    ''' </summary>
    Public Shared ReadOnly Property P_Def_Header_ContentType() As String = "application/x-www-form-urlencoded"

    ''' <summary>
    ''' text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
    ''' </summary>
    Public Shared ReadOnly Property P_Def_Header_Accept() As String = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"

    ''' <summary>
    ''' 3000
    ''' </summary>
    Public Shared ReadOnly Property P_Def_ReStartIntervall() As Integer = 2000

    ''' <summary>
    ''' 15
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TryMaxRestart() As Integer = 15

    ''' <summary>
    ''' [-&gt;]
    ''' </summary>
    Public Shared ReadOnly Property P_Def_AnrMonDirection_Call() As String = "[->]"

    ''' <summary>
    ''' [&lt;-]
    ''' </summary>
    Public Shared ReadOnly Property P_Def_AnrMonDirection_Ring() As String = "[<-]"

    ''' <summary>
    ''' [&lt;&gt;]
    ''' </summary>
    Public Shared ReadOnly Property P_Def_AnrMonDirection_Default() As String = "[<>]"

    ''' <summary>
    ''' FBDB-AnrMonDirection
    ''' </summary>
    Public Shared ReadOnly Property P_Def_AnrMonDirection_UserProperty_Name() As String = "FBDB-AnrMonDirection"

    ''' <summary>
    ''' FBDB-AnrMonZeit
    ''' </summary>
    Public Shared ReadOnly Property P_Def_AnrMonDirection_UserProperty_Zeit() As String = "FBDB-AnrMonZeit"

    ''' <summary>
    ''' FBDB_Note_Table
    ''' </summary>
    Public Shared ReadOnly Property P_Def_Note_Table() As String = "FBDB_Note_Table"

    ''' <summary>
    ''' BEGIN:VCARD
    ''' </summary>
    Public Shared ReadOnly Property P_Def_Begin_vCard() As String = "BEGIN:VCARD"

    ''' <summary>
    ''' END:VCARD
    ''' </summary>
    Public Shared ReadOnly Property P_Def_End_vCard() As String = "END:VCARD"

    ''' <summary>
    ''' CallList
    ''' </summary>
    ''' <value>CallList</value>
    ''' <returns>CallList</returns>
    Public Shared ReadOnly Property P_Def_NameListCALL() As String = "CallList"

    ''' <summary>
    ''' RingList
    ''' </summary>
    ''' <value>RingList</value>
    ''' <returns>RingList</returns>
    Public Shared ReadOnly Property P_Def_NameListRING() As String = "RingList"

    ''' <summary>
    ''' VIPList
    ''' </summary>
    ''' <value>VIPList</value>
    ''' <returns>VIPList</returns>
    Public Shared ReadOnly Property P_Def_NameListVIP() As String = "VIPList"

    ''' <summary>
    ''' Fritz!Box Telefon-dingsbums
    ''' </summary>
    ''' <value>Fritz!Box Telefon-dingsbums</value>
    ''' <returns>Fritz!Box Telefon-dingsbums</returns>
    Public Shared ReadOnly Property P_Def_Addin_LangName() As String = "Fritz!Box Telefon-dingsbums"

    ''' <summary>
    ''' FritzOutlook
    ''' </summary>
    ''' <value>FritzOutlook</value>
    ''' <returns>FritzOutlook</returns>
    Public Shared ReadOnly Property P_Def_Addin_KurzName() As String = "FritzOutlook"

    ''' <summary>
    ''' FritzOutlook.xml
    ''' </summary>
    ''' <value>FritzOutlook.xml</value>
    ''' <returns>FritzOutlook.xml</returns>
    ''' <remarks>Wird mit "P_Def_Addin_KurzName" erstellt.</remarks>
    Public Shared ReadOnly Property P_Def_Config_FileName() As String = P_Def_Addin_KurzName & ".xml"

    ''' <summary>
    ''' FritzOutlook.log
    ''' </summary>
    ''' <value>FritzOutlook.log</value>
    ''' <returns>FritzOutlook.log</returns>
    ''' <remarks>Wird mit "P_Def_Addin_KurzName" erstellt.</remarks>
    Public Shared ReadOnly Property P_Def_Log_FileName() As String = P_Def_Addin_KurzName & ".log"

    ''' <summary>
    ''' Gibt den Zeitraum in MINUTEN an, nachdem geprüft werden soll, ob der Anrufmonitor noch aktiv ist. 
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Intervall in MINUTEN</returns>
    Public Shared ReadOnly Property P_Def_CheckAnrMonIntervall() As Integer = 1

    ''' <summary>
    ''' Gibt den default Dialport für Mobilgeräte an. 
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>99</returns>
    Public Shared ReadOnly Property P_Def_MobilDialPort() As Integer = 99

    ''' <summary>
    ''' Der Offset der bei der Auswertung der Anrufliste der AnrufID gegeben wird
    ''' </summary>
    ''' <value>100</value>
    Public Shared ReadOnly Property P_Def_AnrListIDOffset() As Integer = 100

    Public Shared ReadOnly Property P_Def_DirectorySeparatorChar() As String = IO.Path.DirectorySeparatorChar

    Public Shared ReadOnly Property P_Def_AddInPath() As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & P_Def_DirectorySeparatorChar & P_Def_Addin_LangName & P_Def_DirectorySeparatorChar


    ''' <summary>
    ''' Ein Array, welches den Namen der UserProperties, die die unformatierte Telefonnummer enthält.
    ''' </summary>
    ''' <value>String-Array</value>
    ''' <returns>String-Array</returns>
    Public Shared ReadOnly Property P_Def_UserProperties() As ReadOnlyCollection(Of String)
        Get
            Dim tmp As New List(Of String) From {
                "FBDB-AssistantTelephoneNumber",
                "FBDB-BusinessTelephoneNumber",
                "FBDB-Business2TelephoneNumber",
                "FBDB-CallbackTelephoneNumber",
                "FBDB-CarTelephoneNumber",
                "FBDB-CompanyMainTelephoneNumber",
                "FBDB-HomeTelephoneNumber",
                "FBDB-Home2TelephoneNumber",
                "FBDB-ISDNNumber",
                "FBDB-MobileTelephoneNumber",
                "FBDB-OtherTelephoneNumber",
                "FBDB-PagerNumber",
                "FBDB-PrimaryTelephoneNumber",
                "FBDB-RadioTelephoneNumber",
                "FBDB-BusinessFaxNumber",
                "FBDB-HomeFaxNumber",
                "FBDB-OtherFaxNumber",
                "FBDB-Telex",
                "FBDB-TTYTDDTelephoneNumber"
            }

            Return New ReadOnlyCollection(Of String)(tmp)
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_olTelNrTypen() As ReadOnlyCollection(Of String)
        Get
            Dim tmp As New List(Of String) From {
                "Assistent",
                "Geschäftlich",
                "Geschäftlich 2",
                "Rückmeldung",
                "Auto",
                "Firma",
                "Privat",
                "Privat 2",
                "ISDN",
                "Mobiltelefon",
                "Weitere",
                "Pager",
                "Haupttelefon",
                "Funkruf",
                "Fax geschäftl.",
                "Fax privat",
                "Weiteres Fax",
                "Telex",
                "Texttelefon"
            }

            Return New ReadOnlyCollection(Of String)(tmp)
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_MobilVorwahlItalien() As ReadOnlyCollection(Of String)
        Get
            Dim tmp As New List(Of String) From {
                "330", "331", "332", "333", "334", "335", "336", "337", "338", "339", "360", "361", "362", "363", "364", "365", "366", "367", "368", "390", "391", "392", "393", "340", "341", "342", "343", "344", "345", "346", "347", "348",
                "349", "380", "381", "382", "383", "384", "385", "386", "387", "388", "389", "320", "321", "322", "323", "324", "325", "326", "327", "328", "329"}

            Return New ReadOnlyCollection(Of String)(tmp)
        End Get
    End Property

    Public Shared ReadOnly Property P_Def_UserPropertyIndex() As String = "FBDB-Save"

#Region "Journal"
    Public Shared ReadOnly Property P_Def_Journal_Text_Eingehend() As String = "Eingehender Anruf von "
    Public Shared ReadOnly Property P_Def_Journal_Text_Ausgehend() As String = "Ausgehender Anruf zu "
    Public Shared ReadOnly Property P_Def_Journal_Text_Verpasst() As String = "Verpasster Anruf von "
    Public Shared ReadOnly Property P_Def_Journal_Text_NichtErfolgreich() As String = "Nicht erfolgreicher Anruf zu "
#End Region

#Region "Phoner"
    Public Shared ReadOnly Property P_Def_Phoner_CONNECT As String = "CONNECT " 'Das Leerzeichen wird benötigt!
    Public Shared ReadOnly Property P_Def_Phoner_DISCONNECT As String = "DISCONNECT"
    Public Shared ReadOnly Property P_Def_Phoner_Challenge As String = "Challenge="
    Public Shared ReadOnly Property P_Def_Phoner_Response As String = "Response="

    ''' <summary>
    ''' Welcome To Phoner
    ''' </summary>
    Public Shared ReadOnly Property P_Def_Phoner_Ready As String = "Welcome to Phoner"
    Public Shared ReadOnly Property P_DefaultPhonerAnrMonPort() As Integer = 2012
#End Region
    ' Passwortverschlüsselung
    Public Shared ReadOnly Property P_Def_PassWordDecryptionKey() As String = "Fritz!Box Script"

#End Region

#Region "Default Value Properties"
    ''' <summary>
    ''' Landesvorwahl für Deutschland mit zwei führenden Nullen: 0049
    ''' </summary>
    ''' <value>0049</value>
    ''' <returns>0049</returns>
    Public Shared ReadOnly Property P_Def_TBLandesVW() As String = P_Def_PreLandesVW & "49"
    Public Shared ReadOnly Property P_Def_CBoxLandesVorwahl() As Integer = P_Def_ErrorMinusOne_Integer
    Public Shared ReadOnly Property P_Def_TBAmt() As String = P_Def_ErrorMinusOne_String
    Public Shared ReadOnly Property P_Def_TBVorwahl() As String = P_Def_LeerString

    ''' <summary>
    ''' 10
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TBNumEntryList() As Integer = 10

    ''' <summary>
    ''' 0
    ''' </summary>
    Public Shared ReadOnly Property P_Def_CBoxVorwahl() As Integer = 0


    ''' <summary>
    ''' 10
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TBEnblDauer() As Integer = 10

    ''' <summary>
    ''' False
    ''' </summary>
    Public Shared ReadOnly Property P_Def_CBAnrMonAuto() As Boolean = False

    ''' <summary>
    ''' 30
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TBAnrBeantworterTimeout() As Integer = 30

    ''' <summary>
    ''' 0
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TBAnrMonX() As Integer = 0

    ''' <summary>
    ''' 0
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TBAnrMonY() As Integer = 0
    Public Shared ReadOnly Property P_Def_CBAnrMonMove() As Boolean = True
    Public Shared ReadOnly Property P_Def_CBAnrMonTransp() As Boolean = True

    ''' <summary>
    ''' 0
    ''' </summary>
    Public Shared ReadOnly Property P_Def_TBAnrMonMoveGeschwindigkeit() As Integer = 0

    ''' <summary>
    ''' 0
    ''' </summary>
    Public Shared ReadOnly Property P_Def_CBoxAnrMonStartPosition() As Integer = 0

    ''' <summary>
    ''' 0
    ''' </summary>
    Public Shared ReadOnly Property P_Def_CBoxAnrMonMoveDirection() As Integer = 0
    Public Shared ReadOnly Property P_Def_CBAnrMonZeigeKontakt() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBAnrMonContactImage() As Boolean = True
    Public Shared ReadOnly Property P_Def_CBIndexAus() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBShowMSN() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBAnrMonCloseAtDISSCONNECT() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBAutoClose() As Boolean = True
    Public Shared ReadOnly Property P_Def_CBVoIPBuster() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBCbCunterbinden() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBCallByCall() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBDialPort() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBKErstellen() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBLogFile() As Boolean = True
    'Einstellung für die Symbolleiste
    Public Shared ReadOnly Property P_Def_CBSymbWwdh() As Boolean = True
    Public Shared ReadOnly Property P_Def_CBSymbAnrMon() As Boolean = True
    Public Shared ReadOnly Property P_Def_CBSymbAnrMonNeuStart() As Boolean = True
    Public Shared ReadOnly Property P_Def_CBSymbAnrListe() As Boolean = True
    Public Shared ReadOnly Property P_Def_CBSymbDirekt() As Boolean = True
    Public Shared ReadOnly Property P_Def_CBSymbRWSuche() As Boolean = True
    Public Shared ReadOnly Property P_Def_CBSymbVIP() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBSymbJournalimport() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBJImport() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBAnrListeUpdateJournal() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBAnrListeUpdateCallLists() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBAnrListeShowAnrMon() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBRWS() As Boolean = False
    Public Shared ReadOnly Property P_Def_TVKontaktOrdnerEntryID() As String = P_Def_ErrorMinusOne_String
    Public Shared ReadOnly Property P_Def_TVKontaktOrdnerStoreID() As String = P_Def_ErrorMinusOne_String
    Public Shared ReadOnly Property P_Def_CBKHO() As Boolean = True
    Public Shared ReadOnly Property P_Def_CBRWSIndex() As Boolean = True
    Public Shared ReadOnly Property P_Def_ComboBoxRWS() As Integer = 0
    Public Shared ReadOnly Property P_Def_CBIndex() As Boolean = True
    Public Shared ReadOnly Property P_Def_CBJournal() As Boolean = True
    Public Shared ReadOnly Property P_Def_CBUseAnrMon() As Boolean = True
    Public Shared ReadOnly Property P_Def_CBCheckMobil() As Boolean = True
    'StoppUhr
    Public Shared ReadOnly Property P_Def_CBStoppUhrEinblenden() As Boolean = False
    Public Shared ReadOnly Property P_Def_CBStoppUhrAusblenden() As Boolean = False
    Public Shared ReadOnly Property P_Def_TBStoppUhr() As Integer = 10
    Public Shared ReadOnly Property P_Def_CBStoppUhrX() As Integer = 10
    Public Shared ReadOnly Property P_Def_CBStoppUhrY() As Integer = 10
    Public Shared ReadOnly Property P_Def_CBStoppUhrIgnIntFax() As Boolean = False


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
    Private ReadOnly Property P_Def_Options() As String = "Optionen"
    Private ReadOnly Property P_Def_Statistics() As String = "Statistik"
    Private ReadOnly Property P_Def_Journal() As String = "Journal"
    Private ReadOnly Property P_Def_Phoner() As String = "Phoner"

#End Region

#Region "Debug Properties"
    Public Shared ReadOnly Property P_Debug_Use_WebClient() As Boolean = False
    Public Shared ReadOnly Property P_Debug_AnrufSimulation() As Boolean = False
    Public Shared ReadOnly Property P_Debug_ImportTelefone() As Boolean = False
    Public Property P_Debug_FBFile As List(Of String)
    Public Property P_Debug_PfadKonfig() As String

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
    Public Shared ReadOnly Property P_Lit_Phoner2() As String = "Fehler!" & P_Def_EineNeueZeile & "Das Phoner-Passwort ist falsch!"


    ''' <summary>
    ''' Fehler!
    ''' Die Phoner-Verson ist zu alt!
    ''' </summary>
    Public Shared ReadOnly Property P_Lit_Phoner3() As String = "Fehler!" & P_Def_EineNeueZeile & "Die Phoner-Verson ist zu alt!"

    ''' <summary>
    ''' Fehler!
    ''' TCP Fehler (Stream.CanWrite = False)!
    ''' </summary>
    Public Shared ReadOnly Property P_Lit_Phoner4() As String = "Fehler!" & P_Def_EineNeueZeile & "TCP Fehler (Stream.CanWrite = False)!"

    ''' <summary>
    ''' Fehler!
    ''' TCP!
    ''' </summary>
    Public Shared ReadOnly Property P_Lit_Phoner5() As String = "Fehler!" & P_Def_EineNeueZeile & "TCP!"

    ''' <summary>
    ''' Fehler!
    ''' Kein Passwort hinterlegt!
    ''' </summary>
    Public Shared ReadOnly Property P_Lit_Phoner6() As String = "Fehler!" & P_Def_EineNeueZeile & "Kein Passwort hinterlegt!"

    ''' <summary>
    ''' Fehler!
    ''' Phoner nicht verfügbar!
    ''' </summary>
    Public Shared ReadOnly Property P_Lit_Phoner7() As String = "Fehler!" & P_Def_EineNeueZeile & "Phoner nicht verfügbar!"

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
    Public Shared ReadOnly Property P_AnrMon_MsgBox_AnrMonStart1() As String = "Der Anrufmonitor kann nicht gestartet werden, da die Fritz!Box die Verbindung verweigert." & P_Def_EineNeueZeile & "Dies ist meist der Fall, wenn der Fritz!Box Callmonitor deaktiviert ist. Mit dem Telefoncode """ & P_Def_TelCodeActivateFritzBoxCallMonitor & """ kann dieser aktiviert werden." & P_Def_EineNeueZeile & "Soll versucht werden, den Fritz!Box Callmonitor über die Direktwahl zu aktivieren? (Danach kann der Anrufmonitor manuell aktiviert werden.)"

    ''' <summary>
    ''' Soll der Fritz!Box Callmonitor aktiviert werden?
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_MsgBox_AnrMonStart2() As String = "Soll der Fritz!Box Callmonitor aktiviert werden?"

    ''' <summary>
    ''' Das automatische Aktivieren des Fritz!Box Callmonitor wurde übersprungen.
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonStart1() As String = "Das automatische Aktivieren des Fritz!Box Callmonitor wurde übersprungen."

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
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonStart3() As String = "TCP Verbindung nicht aufgebaut."

    ''' <summary>
    ''' Anrufmonitor nach StandBy wiederaufgebaut.
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonStart4() As String = "Anrufmonitor nach StandBy wiederaufgebaut."

    ''' <summary>
    ''' BWStartTCPReader_RunWorkerCompleted: Es ist ein TCP/IP Fehler aufgetreten.
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonStart5() As String = "BWStartTCPReader_RunWorkerCompleted: Es ist ein TCP/IP Fehler aufgetreten."

    ''' <summary>
    ''' Die TCP-Verbindung zum Fritz!Box Anrufmonitor wurde verloren.
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonTimer4() As String = "Die TCP-Verbindung zum Fritz!Box Anrufmonitor wurde verloren."


    ''' <summary>
    ''' Fritz!Box nach einem Verbindungsverlust noch nicht verfügbar.
    ''' </summary>
    Public Shared ReadOnly Property P_ReStart_Log_Timer1() As String = P_Def_FritzBoxName & " nach einem Verbindungsverlust noch nicht verfügbar."

    ''' <summary>
    ''' Fritz!Box nach Verbindungsverlust wieder verfügbar. Initialisiere Anrufmonitor...
    ''' </summary>
    Public Shared ReadOnly Property P_ReStart_Log_Timer2() As String = P_Def_FritzBoxName & " nach Verbindungsverlust wieder verfügbar. Initialisiere Anrufmonitor..."

    ''' <summary>
    ''' Reaktivierung des Anrufmonitors nach einem Verbindungsverlust nicht erfolgreich.
    ''' </summary>
    Public Shared ReadOnly Property P_ReStart_Log_Timer3() As String = "Reaktivierung des Anrufmonitors nach einem Verbindungsverlust nicht erfolgreich."

    ''' <summary>
    ''' Anrufmonitor nach einem Verbindungsverlust gestartet
    ''' </summary>
    Public Shared ReadOnly Property P_ReStart_Log_Timer4() As String = "Anrufmonitor nach einem Verbindungsverlust gestartet."

    ''' <summary>
    ''' Auswertung der Anrufliste nach dem Aufwachen aus Standby gestartet.
    ''' </summary>
    Public Shared ReadOnly Property P_ReStart_Log_Timer5() As String = "Auswertung der Anrufliste nach einem Verbindungsverlust gestartet."

    ''' <summary>
    ''' Welcome to Phoner
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_AnrMonPhonerWelcome() As String = "Welcome to Phoner"

    ''' <summary>
    ''' Sorry, too many clients
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_AnrMonPhonerError() As String = "Sorry, too many clients"

    ''' <summary>
    ''' AnrMonAktion, Phoner: "Sorry, too many clients"
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonPhoner1() As String = "AnrMonAktion, Phoner: """ & P_AnrMon_AnrMonPhonerError & """"

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
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonStoppUhr1() As String = "StoppUhr wird eingeblendet."

    ''' <summary>
    ''' Integrierte Faxfunktion wurde erkannt: Stoppuhr nicht eingeblendet.
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Log_AnrMonStoppUhr2() As String = "Integrierte Faxfunktion wurde erkannt: Stoppuhr nicht eingeblendet."

    Public Shared ReadOnly Property P_AnrMon_Journal_Def_Categories() As ReadOnlyCollection(Of String)
        Get
            Return New ReadOnlyCollection(Of String)({"FritzBox Anrufmonitor", "Telefonanrufe"})
        End Get
    End Property

    ''' <summary>
    ''' Kontaktdaten:
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_Journal_Kontaktdaten As String = "Kontaktdaten:"

    ''' <summary>
    ''' Kontaktdaten (vCard):
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_AnrMonDISCONNECT_Journal As String = "Kontaktdaten (vCard):"

    ''' <summary>
    ''' Ein unvollständiges Telefonat wurde registriert.
    ''' </summary>
    Public Shared ReadOnly Property P_AnrMon_AnrMonDISCONNECT_Error As String = "Ein unvollständiges Telefonat wurde registriert."

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
    Public Shared ReadOnly Property P_AnrMon_AnrMonDISCONNECT_JournalTelNr() As String = "Tel.-Nr.: "

    'Anrufmonitor - PopUp
    ''' <summary>
    ''' Kontakt öffnen
    ''' </summary>
    ''' <value>Kontakt öffnen</value>
    ''' <returns>Kontakt öffnen</returns>
    Public Shared ReadOnly Property P_AnrMon_PopUp_ToolStripMenuItemKontaktöffnen As String = "Kontakt öffnen"

    ''' <summary>
    ''' Kontakt erstellen
    ''' </summary>
    ''' <value>Kontakt erstellen</value>
    ''' <returns>Kontakt erstellen</returns>
    Public Shared ReadOnly Property P_AnrMon_PopUp_ToolStripMenuItemKontaktErstellen As String = "Kontakt erstellen"

    ''' <summary>
    ''' Rückruf
    ''' </summary>
    ''' <value>Rückruf</value>
    ''' <returns>Rückruf</returns>
    Public Shared ReadOnly Property P_AnrMon_PopUp_ToolStripMenuItemRückruf As String = "Rückruf"

    ''' <summary>
    ''' In Zwischenablage kopieren
    ''' </summary>
    ''' <value>In Zwischenablage kopieren</value>
    ''' <returns>In Zwischenablage kopieren</returns>
    Public Shared ReadOnly Property P_AnrMon_PopUp_ToolStripMenuItemKopieren As String = "In Zwischenablage kopieren"

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
    Public Shared ReadOnly Property P_FritzBox_LoginError_MissingPassword As String = "Die Fritz!Box benötigt kein Passwort. Das AddIn wird nicht funktionieren."

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
    Public Shared ReadOnly Property P_FritzBox_LoginError_MissingData() As String = "Das Passwort zur Fritz!Box kann nicht entschlüsselt werden, da das verschlüsselte Passwort und/oder der Zugangsschlüssel fehlt."

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
    Public Shared ReadOnly Property P_FritzBox_LogoutTestString1 As String= "Sie haben sich erfolgreich von der FRITZ!Box abgemeldet."

    ''' <summary>
    ''' Sie haben sich erfolgreich von der Benutzeroberfläche Ihrer FRITZ!Box abgemeldet.
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_LogoutTestString2 As String = "Sie haben sich erfolgreich von der Benutzeroberfläche Ihrer FRITZ!Box abgemeldet."

    ''' <summary>
    ''' Logout eventuell NICHT erfolgreich!
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_LogoutError As String = "Logout eventuell NICHT erfolgreich!"

    ' Telefone
    ''' <summary>
    ''' Fehler bei dem Herunterladen der Telefone: Die Anmeldedaten sind falsch oder es fehlt die Berechtigung für diesen Bereich.
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Tel_Error1 As String = "Fehler bei dem Herunterladen der Telefone: Die Anmeldedaten sind falsch oder es fehlt die Berechtigung für diesen Bereich."

    ''' <summary>
    ''' Fehler bei dem Herunterladen der Telefone: Telefonieseite kann nicht gelesen werden.
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Tel_Error2 As String = "Fehler bei dem Herunterladen der Telefone: Telefonieseite kann nicht gelesen werden."

    ''' <summary>
    ''' Ausleseroutine für &amp; P_Def_FritzBoxName &amp; bis Firmware 5.25 gestartet...
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Tel_RoutineBis525 As String = " Ausleseroutine für " & P_Def_FritzBoxName & " bis Firmware 5.25 gestartet..."

    ''' <summary>
    ''' " Ausleseroutine für " &amp; P_Def_FritzBoxName &amp; " ab Firmware 5.25 bis Firmware 6.05 gestartet..."
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Tel_RoutineAb525 As String = " Ausleseroutine für " & P_Def_FritzBoxName & " ab Firmware 5.26 bis Firmware 6.04 gestartet..."

    ''' <summary>
    ''' Ausleseroutine für &amp; P_Def_FritzBoxName &amp; ab Firmware 6.05 gestartet...
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Tel_RoutineAb605 As String = " Ausleseroutine für " & P_Def_FritzBoxName & " ab Firmware 6.05 gestartet..."

    ''' <summary>
    ''' Bitte sende diese Datei an folgende E-Mail: &amp; P_AddinKontaktMail
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Tel_DebugMsgAb605 As String = "Bitte sende diese Datei an folgende E-Mail: " & P_AddinKontaktMail

    ''' <summary>
    ''' E-Mail kruemelino@gert-michael.de
    ''' </summary>
    Public Shared ReadOnly Property P_AddinKontaktMail As String = "kruemelino@gert-michael.de"

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
    Public Shared ReadOnly Property P_FritzBox_Tel_ErrorAlt1 As String = "Fehler beim Herunterladen der Telefone. Anmeldedaten korrekt?"

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
    Public Shared ReadOnly Property P_FritzBox_Dial_Error1 As String = "Fehler!" & P_Def_EineNeueZeile & "Entwickler kontaktieren."

    ''' <summary>
    ''' Fehler! Logfile beachten!
    ''' </summary>
    Public Shared ReadOnly Property P_FritzBox_Dial_Error2 As String = "Fehler!" & P_Def_EineNeueZeile & "Logfile beachten!"

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
    Public Shared ReadOnly Property P_FritzBox_Dial_HangUp As String = "Verbindungsaufbau" & P_Def_EineNeueZeile & "wurde abgebrochen!"

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
    Public Shared ReadOnly Property P_FritzBox_JI_Error1 As String = "Der Login in die " & P_Def_FritzBoxName & " ist fehlgeschlagen. Die Anmeldedaten sind falsch oder es fehlt die Berechtigung für diesen Bereich."

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
    Public Shared ReadOnly Property P_Init_FritzBox_NotFound() As String = "Keine " & P_Def_FritzBoxName & " unter der angegebenen IP gefunden."

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
    Public Shared ReadOnly Property P_Init_Login_Korrekt() As String = "Das Anmelden an der " & P_Def_FritzBoxName & " war erfolgreich."

    ''' <summary>
    ''' Das Anmelden an der Fritz!Box war erfolgreich.
    ''' </summary>
    Public Shared ReadOnly Property P_Init_Login_Nicht_Korrekt() As String = "Die Anmeldedaten sind falsch oder es fehlt die Berechtigung."

    ''' <summary>
    ''' Bitte warten...
    ''' </summary>
    Public Shared ReadOnly Property P_Def_Bitte_Warten() As String = "Bitte warten..."

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
    Public Shared ReadOnly Property P_CMB_Dial() As String = "Wählen"

    ''' <summary>
    ''' Wahlwiederholung
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_WWDH() As String = "Wahlwiederholung"

    ''' <summary>
    ''' Direktwahl
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Direktwahl() As String = "Direktwahl"

    ''' <summary>
    ''' Anrufmonitor
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMon() As String = "Anrufmonitor"

    ''' <summary>
    ''' Anzeigen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMonAnzeigen() As String = "Anzeigen"

    ''' <summary>
    ''' Anrufmonitor neustarten
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMonNeuStart() As String = "Anrufmonitor neustarten"

    ''' <summary>
    ''' Rückruf
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_CallBack() As String = "Rückruf"

    ''' <summary>
    ''' VIP-Liste
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP() As String = "VIP-Liste"

    ''' <summary>
    ''' Journalimport
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Journal() As String = "Journalimport"

    ''' <summary>
    ''' Einstellungen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Setup() As String = "Einstellungen"

    ''' <summary>
    ''' Liste löschen...
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_ClearList() As String = "Liste löschen..."

    ''' <summary>
    ''' Liste löschen...
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_ClearEntry() As String = "Eintrag löschen..."

    ''' <summary>
    ''' Öffnet den Wahldialog um das ausgewählte Element anzurufen.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Dial_ToolTipp() As String = "Öffnet den Wahldialog um das ausgewählte Element anzurufen"

    ''' <summary>
    ''' Öffnet den Wahldialog für die Wahlwiederholung
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_WWDH_ToolTipp() As String = "Öffnet den Wahldialog für die Wahlwiederholung"

    ''' <summary>
    ''' Startet den Anrufmonitor.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMon_ToolTipp() As String = "Startet den Anrufmonitor"

    ''' <summary>
    ''' Öffnet den Wahldialog für die Direktwahl
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Direktwahl_ToolTipp() As String = "Öffnet den Wahldialog für die Direktwahl"

    ''' <summary>
    ''' Zeigt den letzten Anruf an
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMonAnzeigen_ToolTipp() As String = "Zeigt den letzten Anruf an"

    ''' <summary>
    ''' Startet den Anrufmonitor neu
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_AnrMonNeuStart_ToolTipp() As String = "Startet den Anrufmonitor neu"

    ''' <summary>
    ''' Öffnet den Wahldialog für den Rückruf
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_CallBack_ToolTipp() As String = "Öffnet den Wahldialog für den Rückruf"

    ''' <summary>
    ''' Öffnet den Wahldialog um einen VIP anzurufen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP_ToolTipp() As String = "Öffnet den Wahldialog um einen VIP anzurufen"

    ''' <summary>
    ''' Die VIP-Liste ist mit 10 Einträgen bereits voll.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP_O11_Voll_ToolTipp() As String = "Die VIP-Liste ist mit 10 Einträgen bereits voll."

    ''' <summary>
    ''' Füge diesen Kontakt der VIP-Liste hinzu.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP_Hinzufügen_ToolTipp() As String = "Füge diesen Kontakt der VIP-Liste hinzu."

    ''' <summary>
    ''' Entfernt diesen Kontakt von der VIP-Liste.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_VIP_Entfernen_ToolTipp() As String = "Entfernt diesen Kontakt von der VIP-Liste."

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
    Public Shared ReadOnly Property P_CMB_Insp_VIP() As String = "VIP"

    ''' <summary>
    ''' Upload
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_Upload() As String = "Upload"

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
    Public Shared ReadOnly Property P_CMB_Insp_RWS() As String = "Rückwärtssuche"

    ''' <summary>
    ''' Suchen Sie zusätzliche Informationen zu diesem Anrufer mit der Rückwärtssuche.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_RWS_ToolTipp() As String = "Suchen Sie zusätzliche Informationen zu diesem Anrufer mit der Rückwärtssuche"

    ''' <summary>
    ''' Notiz
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_Note() As String = "Notiz"

    ''' <summary>
    ''' Einen Notizeintrag hinzufügen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Insp_Note_ToolTipp() As String = "Einen Notizeintrag hinzufügen"

    ''' <summary>
    ''' Fritz!Box Telefonbuch
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Expl_Adrbk() As String = P_Def_FritzBoxName & " Telefonbuch"

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
    Public Shared ReadOnly Property P_CMB_Kontakt_Erstellen() As String = "Kontakt erstellen"

    ''' <summary>
    ''' Erstellt einen Kontakt aus einem Journaleintrag
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Kontakt_Erstellen_ToolTipp() As String = "Erstellt einen Kontakt aus einem Journaleintrag"

    ''' <summary>
    ''' Kontakt anzeigen
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Kontakt_Anzeigen() As String = "Kontakt anzeigen"

    ''' <summary>
    ''' Zeigt den Kontakt zu diesem Journaleintrag an
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Kontakt_Anzeigen_ToolTipp() As String = "Zeigt den Kontakt zu diesem Journaleintrag an"

    ''' <summary>
    ''' Der verknüpfte Kontakt kann nicht gefunden werden! Erstelle einen neuen Kontakt aus diesem Journaleintrag.
    ''' </summary>
    Public Shared ReadOnly Property P_CMB_Kontakt_Anzeigen_Error_ToolTipp() As String = "Der verknüpfte Kontakt kann nicht gefunden werden! Erstelle einen neuen Kontakt aus diesem Journaleintrag."

    ' Rückwärtssuche

    ''' <summary>
    ''' 11880
    ''' </summary>
    Public Shared ReadOnly Property P_RWS11880_Name() As String = "11880"

    ''' <summary>
    ''' dasÖrtliche
    ''' </summary>
    Public Shared ReadOnly Property P_RWSDasOertliche_Name() As String = "dasÖrtliche"

    ''' <summary>
    ''' dasTelefonbuch
    ''' </summary>
    Public Shared ReadOnly Property P_RWSDasTelefonbuch_Name() As String = "dasTelefonbuch"

    ''' <summary>
    ''' tel.search.ch
    ''' </summary>
    Public Shared ReadOnly Property P_RWSTelSearch_Name() As String = "tel.search.ch"

    ''' <summary>
    ''' Alle
    ''' </summary>
    Public Shared ReadOnly Property P_RWSAlle_Name() As String = "Alle"

    ''' <summary>
    ''' www.11880.com
    ''' </summary>
    Public Shared ReadOnly Property P_RWS11880_Link() As String = "www.11880.com"

    ''' <summary>
    ''' www.dasoertliche.de
    ''' </summary>
    Public Shared ReadOnly Property P_RWSDasOertliche_Link() As String = "www.dasoertliche.de"

    ''' <summary>
    ''' www.dastelefonbuch.de
    ''' </summary>
    Public Shared ReadOnly Property P_RWSDasTelefonbuch_Link() As String = "www.dastelefonbuch.de"

    ''' <summary>
    ''' tel.search.ch
    ''' </summary>
    Public Shared ReadOnly Property P_RWSTelSearch_Link() As String = "tel.search.ch"

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
    Public Shared ReadOnly Property P_RWS_ToolTipp() As String = "Rückwärtssuche mit allen Anbietern"

    ' Inspector Button Tag
    ''' <summary>
    ''' Dial_Tag
    ''' </summary>
    Public Shared ReadOnly Property P_Tag_Insp_Dial() As String = "Dial_Tag"

    ''' <summary>
    ''' Kontakt_Tag
    ''' </summary>
    Public Shared ReadOnly Property P_Tag_Insp_Kontakt() As String = "Kontakt_Tag"

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
    Public Shared ReadOnly Property P_Fehler_Export_Addressbuch() As String = "Der Addressbuch der " & P_Def_FritzBoxName & " kann nicht geöffnet werden."

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

        P_TBLandesVW = C_XML.Read(XMLDoc, P_Def_Options, "TBLandesVW", P_Def_TBLandesVW)
        P_TBAmt = C_XML.Read(XMLDoc, P_Def_Options, "TBAmt", P_Def_TBAmt)
        P_TBFBAdr = C_XML.Read(XMLDoc, P_Def_Options, "TBFBAdr", P_Def_TBFBAdr)
        P_CBForceFBAddr = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBForceFBAddr", CStr(P_Def_CBForceFBAddr)))
        P_TBBenutzer = C_XML.Read(XMLDoc, P_Def_Options, "TBBenutzer", P_Def_TBBenutzer)
        P_TBPasswort = C_XML.Read(XMLDoc, P_Def_Options, "TBPasswort", P_Def_TBPasswort)
        P_TBVorwahl = C_XML.Read(XMLDoc, P_Def_Options, "TBVorwahl", P_Def_TBVorwahl)
        P_TBNumEntryList = CInt(C_XML.Read(XMLDoc, P_Def_Options, "TBNumEntryList", CStr(P_Def_TBNumEntryList)))
        P_CBoxVorwahl = CInt(C_XML.Read(XMLDoc, P_Def_Options, "CBoxVorwahl", CStr(P_Def_CBoxVorwahl)))
        P_TBEnblDauer = CInt(C_XML.Read(XMLDoc, P_Def_Options, "TBEnblDauer", CStr(P_Def_TBEnblDauer)))
        P_CBAnrMonAuto = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrMonAuto", CStr(P_Def_CBAnrMonAuto)))
        P_TBAnrBeantworterTimeout = CInt(C_XML.Read(XMLDoc, P_Def_Options, "TBAnrBeantworterTimeout", CStr(P_Def_TBAnrBeantworterTimeout)))
        P_TBAnrMonX = CInt(C_XML.Read(XMLDoc, P_Def_Options, "TBAnrMonX", CStr(P_Def_TBAnrMonX)))
        P_TBAnrMonY = CInt(C_XML.Read(XMLDoc, P_Def_Options, "TBAnrMonY", CStr(P_Def_TBAnrMonY)))
        P_CBAnrMonMove = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrMonMove", CStr(P_Def_CBAnrMonMove)))
        P_CBAnrMonTransp = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrMonTransp", CStr(P_Def_CBAnrMonTransp)))
        P_TBAnrMonMoveGeschwindigkeit = CInt(C_XML.Read(XMLDoc, P_Def_Options, "TBAnrMonMoveGeschwindigkeit", CStr(P_Def_TBAnrMonMoveGeschwindigkeit)))
        P_CBoxAnrMonStartPosition = CInt(C_XML.Read(XMLDoc, P_Def_Options, "CBoxAnrMonStartPosition", CStr(P_Def_CBoxAnrMonStartPosition)))
        P_CBoxAnrMonMoveDirection = CInt(C_XML.Read(XMLDoc, P_Def_Options, "CBoxAnrMonMoveDirection", CStr(P_Def_CBoxAnrMonMoveDirection)))
        P_CBAnrMonZeigeKontakt = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrMonZeigeKontakt", CStr(P_Def_CBAnrMonZeigeKontakt)))
        P_CBAnrMonContactImage = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrMonContactImage", CStr(P_Def_CBAnrMonContactImage)))
        P_CBIndexAus = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBIndexAus", CStr(P_Def_CBIndexAus)))
        P_CBShowMSN = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBShowMSN", CStr(P_Def_CBShowMSN)))
        P_CBJournal = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBJournal", CStr(P_Def_CBJournal)))
        P_CBAnrListeUpdateJournal = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrListeUpdateJournal", CStr(P_Def_CBAnrListeUpdateJournal)))
        P_CBAnrListeUpdateCallLists = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrListeUpdateCallLists", CStr(P_Def_CBAnrListeUpdateCallLists)))
        P_CBAnrListeShowAnrMon = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrListeShowAnrMon", CStr(P_Def_CBAnrListeShowAnrMon)))
        P_CBUseAnrMon = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBUseAnrMon", CStr(P_Def_CBUseAnrMon)))
        P_CBCheckMobil = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBCheckMobil", CStr(P_Def_CBCheckMobil)))
        P_CBAutoClose = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAutoClose", CStr(P_Def_CBAutoClose)))
        P_CBAnrMonCloseAtDISSCONNECT = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBAnrMonCloseAtDISSCONNECT", CStr(P_Def_CBAnrMonCloseAtDISSCONNECT)))
        P_CBVoIPBuster = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBVoIPBuster", CStr(P_Def_CBVoIPBuster)))
        P_CBCbCunterbinden = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBCbCunterbinden", CStr(P_Def_CBCbCunterbinden)))
        P_CBCallByCall = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBCallByCall", CStr(P_Def_CBCallByCall)))
        P_CBDialPort = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBDialPort", CStr(P_Def_CBDialPort)))
        P_CBKErstellen = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBKErstellen", CStr(P_Def_CBKErstellen)))
        P_CBLogFile = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBLogFile", CStr(P_Def_CBLogFile)))
        ' Einstellungen für die Symbolleiste laden
        P_CBSymbWwdh = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBSymbWwdh", CStr(P_Def_CBSymbWwdh)))
        P_CBSymbAnrMon = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBSymbAnrMon", CStr(P_Def_CBSymbAnrMon)))
        P_CBSymbAnrMonNeuStart = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBSymbAnrMonNeuStart", CStr(P_Def_CBSymbAnrMonNeuStart)))
        P_TVKontaktOrdnerEntryID = C_XML.Read(XMLDoc, P_Def_Options, "TVKontaktOrdnerEntryID", CStr(P_Def_TVKontaktOrdnerEntryID))
        P_TVKontaktOrdnerStoreID = C_XML.Read(XMLDoc, P_Def_Options, "TVKontaktOrdnerStoreID", CStr(P_Def_TVKontaktOrdnerStoreID))
        P_CBAutoAnrList = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBJImport", CStr(P_Def_CBJImport)))
        ' Einstellungen füer die Rückwärtssuche laden
        P_CBKHO = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBKHO", CStr(P_Def_CBKHO)))
        P_CBRWS = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBRWS", CStr(P_Def_CBRWS)))
        P_CBRWSIndex = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBRWSIndex", CStr(P_Def_CBRWSIndex)))
        P_ComboBoxRWS = CInt(C_XML.Read(XMLDoc, P_Def_Options, "ComboBoxRWS", CStr(P_Def_ComboBoxRWS)))
        P_CBIndex = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBIndex", CStr(P_Def_CBIndex)))
        ' StoppUhr
        P_CBStoppUhrEinblenden = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBStoppUhrEinblenden", CStr(P_Def_CBStoppUhrEinblenden)))
        P_CBStoppUhrAusblenden = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBStoppUhrAusblenden", CStr(P_Def_CBStoppUhrAusblenden)))
        P_TBStoppUhr = CInt(C_XML.Read(XMLDoc, P_Def_Options, "TBStoppUhr", CStr(P_Def_TBStoppUhr)))
        P_CBStoppUhrX = CInt(C_XML.Read(XMLDoc, P_Def_Options, "CBStoppUhrX", CStr(P_Def_CBStoppUhrX)))
        P_CBStoppUhrY = CInt(C_XML.Read(XMLDoc, P_Def_Options, "CBStoppUhrY", CStr(P_Def_CBStoppUhrY)))
        ' Telefonnummernformatierung
        P_TBTelNrMaske = C_XML.Read(XMLDoc, P_Def_Options, "TBTelNrMaske", P_Def_TBTelNrMaske)
        P_CBTelNrGruppieren = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBTelNrGruppieren", CStr(P_Def_CBTelNrGruppieren)))
        P_CBintl = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBintl", CStr(P_Def_CBintl)))
        P_CBIgnoTelNrFormat = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBIgnoTelNrFormat", CStr(P_Def_CBIgnoTelNrFormat)))
        ' Phoner
        P_CBPhoner = CBool(C_XML.Read(XMLDoc, P_Def_Phoner, "CBPhoner", CStr(P_Def_CBPhoner)))
        P_PhonerVerfügbar = CBool(C_XML.Read(XMLDoc, P_Def_Phoner, "PhonerVerfügbar", CStr(P_Def_PhonerVerfügbar)))
        P_ComboBoxPhonerSIP = CInt(C_XML.Read(XMLDoc, P_Def_Phoner, "ComboBoxPhonerSIP", CStr(P_Def_ComboBoxPhonerSIP)))
        P_CBPhonerAnrMon = CBool(C_XML.Read(XMLDoc, P_Def_Phoner, "CBPhonerAnrMon", CStr(P_Def_CBPhonerAnrMon)))
        P_TBPhonerPasswort = C_XML.Read(XMLDoc, P_Def_Phoner, "TBPhonerPasswort", P_Def_TBPhonerPasswort)
        P_PhonerTelNameIndex = CInt(C_XML.Read(XMLDoc, P_Def_Phoner, "PhonerTelNameIndex", CStr(P_Def_PhonerTelNameIndex)))
        ' Statistik
        P_StatResetZeit = CDate(C_XML.Read(XMLDoc, P_Def_Statistics, "ResetZeit", CStr(P_Def_StatResetZeit)))
        P_StatVerpasst = CInt(C_XML.Read(XMLDoc, P_Def_Statistics, "Verpasst", CStr(P_Def_StatVerpasst)))
        P_StatNichtErfolgreich = CInt(C_XML.Read(XMLDoc, P_Def_Statistics, "Nichterfolgreich", CStr(P_Def_StatNichtErfolgreich)))
        P_StatKontakt = CInt(C_XML.Read(XMLDoc, P_Def_Statistics, "Kontakt", CStr(P_Def_StatKontakt)))
        P_StatJournal = CInt(C_XML.Read(XMLDoc, P_Def_Statistics, "Journal", CStr(P_Def_StatJournal)))
        P_StatOLClosedZeit = CDate(C_XML.Read(XMLDoc, P_Def_Journal, "SchließZeit", CStr(P_Def_StatOLClosedZeit)))
        ' Wählbox
        P_TelAnschluss = CInt(C_XML.Read(XMLDoc, P_Def_Options, "Anschluss", CStr(P_Def_TelAnschluss)))
        P_TelFestnetz = CBool(C_XML.Read(XMLDoc, P_Def_Options, "Festnetz", CStr(P_TelFestnetz)))
        P_TelCLIR = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CLIR", CStr(P_Def_TelCLIR)))
        P_EncodingFritzBox = Text.Encoding.GetEncoding(C_XML.Read(XMLDoc, P_Def_Options, "EncodingFritzBox", P_Def_EncodingFritzBox.HeaderName))
        ' Indizierung
        P_LLetzteIndizierung = CDate(C_XML.Read(XMLDoc, P_Def_Options, "LLetzteIndizierung", CStr(P_Def_LLetzteIndizierung)))
        ' Notiz
        P_CBNote = CBool(C_XML.Read(XMLDoc, P_Def_Options, "CBNote", CStr(P_Def_CBNote)))
        ' Fritz!Box Kommunikation
        P_RBFBComUPnP = CBool(C_XML.Read(XMLDoc, P_Def_Options, "RBFBComUPnP", CStr(P_Def_RBFBComUPnP)))

        With xPathTeile
            .Clear()
            .Add("Telefone")
            .Add("Nummern")
            '.Add("*")
            .Add("[@Checked=""1""]")
        End With

        If P_CLBTelNr IsNot Nothing Then P_CLBTelNr.Clear()
        P_CLBTelNr = New Collection(Of String)((From x In Split(C_XML.Read(XMLDoc, xPathTeile, P_Def_ErrorMinusOne_String), ";", , CompareMethod.Text) Select x Distinct).ToArray)

        BWCBox = New BackgroundWorker
        With BWCBox
            .WorkerReportsProgress = False
            .RunWorkerAsync(True)
        End With

    End Sub

    ''' <summary>
    ''' Speicher Daten, die in den Properties stehen in die XML-String.
    ''' </summary>
    Private Sub SaveOptionData()
        C_XML.Write(XMLDoc, P_Def_Options, "TBLandesVW", P_TBLandesVW)
        C_XML.Write(XMLDoc, P_Def_Options, "TBAmt", P_TBAmt)
        C_XML.Write(XMLDoc, P_Def_Options, "TBFBAdr", P_TBFBAdr)
        C_XML.Write(XMLDoc, P_Def_Options, "CBForceFBAddr", CStr(P_CBForceFBAddr))
        C_XML.Write(XMLDoc, P_Def_Options, "TBBenutzer", P_TBBenutzer)
        C_XML.Write(XMLDoc, P_Def_Options, "TBPasswort", P_TBPasswort)
        C_XML.Write(XMLDoc, P_Def_Options, "TBVorwahl", P_TBVorwahl)
        C_XML.Write(XMLDoc, P_Def_Options, "CBoxVorwahl", CStr(P_CBoxVorwahl))
        C_XML.Write(XMLDoc, P_Def_Options, "TBEnblDauer", CStr(P_TBEnblDauer))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrMonAuto", CStr(P_CBAnrMonAuto))
        C_XML.Write(XMLDoc, P_Def_Options, "TBAnrBeantworterTimeout", CStr(P_TBAnrBeantworterTimeout))
        C_XML.Write(XMLDoc, P_Def_Options, "TBAnrMonX", CStr(P_TBAnrMonX))
        C_XML.Write(XMLDoc, P_Def_Options, "TBAnrMonY", CStr(P_TBAnrMonY))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrMonMove", CStr(P_CBAnrMonMove))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrMonTransp", CStr(P_CBAnrMonTransp))
        C_XML.Write(XMLDoc, P_Def_Options, "TBAnrMonMoveGeschwindigkeit", CStr(P_TBAnrMonMoveGeschwindigkeit))
        C_XML.Write(XMLDoc, P_Def_Options, "CBoxAnrMonStartPosition", CStr(P_CBoxAnrMonStartPosition))
        C_XML.Write(XMLDoc, P_Def_Options, "CBoxAnrMonMoveDirection", CStr(P_CBoxAnrMonMoveDirection))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrMonZeigeKontakt", CStr(P_CBAnrMonZeigeKontakt))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrMonContactImage", CStr(P_CBAnrMonContactImage))
        C_XML.Write(XMLDoc, P_Def_Options, "CBIndexAus", CStr(P_CBIndexAus))
        C_XML.Write(XMLDoc, P_Def_Options, "CBShowMSN", CStr(P_CBShowMSN))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAutoClose", CStr(P_CBAutoClose))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrMonCloseAtDISSCONNECT", CStr(P_CBAnrMonCloseAtDISSCONNECT))
        C_XML.Write(XMLDoc, P_Def_Options, "CBVoIPBuster", CStr(P_CBVoIPBuster))
        C_XML.Write(XMLDoc, P_Def_Options, "CBCbCunterbinden", CStr(P_CBVoIPBuster))
        C_XML.Write(XMLDoc, P_Def_Options, "CBCallByCall", CStr(P_CBCallByCall))
        C_XML.Write(XMLDoc, P_Def_Options, "CBDialPort", CStr(P_CBDialPort))
        C_XML.Write(XMLDoc, P_Def_Options, "CBKErstellen", CStr(P_CBKErstellen))
        C_XML.Write(XMLDoc, P_Def_Options, "CBLogFile", CStr(P_CBLogFile))
        ' Einstellungen für die Symbolleiste laden
        C_XML.Write(XMLDoc, P_Def_Options, "CBSymbWwdh", CStr(P_CBSymbWwdh))
        C_XML.Write(XMLDoc, P_Def_Options, "CBSymbAnrMon", CStr(P_CBSymbAnrMon))
        C_XML.Write(XMLDoc, P_Def_Options, "CBSymbAnrMonNeuStart", CStr(P_CBSymbAnrMonNeuStart))
        C_XML.Write(XMLDoc, P_Def_Options, "CBJImport", CStr(P_CBAutoAnrList))
        ' Einstellungen füer die Rückwärtssuche laden
        C_XML.Write(XMLDoc, P_Def_Options, "CBKHO", CStr(P_CBKHO))
        C_XML.Write(XMLDoc, P_Def_Options, "CBRWS", CStr(P_CBRWS))
        C_XML.Write(XMLDoc, P_Def_Options, "CBRWSIndex", CStr(P_CBRWSIndex))
        C_XML.Write(XMLDoc, P_Def_Options, "TVKontaktOrdnerEntryID", CStr(P_TVKontaktOrdnerEntryID))
        C_XML.Write(XMLDoc, P_Def_Options, "TVKontaktOrdnerStoreID", CStr(P_TVKontaktOrdnerStoreID))
        C_XML.Write(XMLDoc, P_Def_Options, "ComboBoxRWS", CStr(P_ComboBoxRWS))
        C_XML.Write(XMLDoc, P_Def_Options, "CBIndex", CStr(P_CBIndex))
        C_XML.Write(XMLDoc, P_Def_Options, "CBJournal", CStr(P_CBJournal))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrListeUpdateJournal", CStr(P_CBAnrListeUpdateJournal))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrListeUpdateCallLists", CStr(P_CBAnrListeUpdateCallLists))
        C_XML.Write(XMLDoc, P_Def_Options, "CBAnrListeShowAnrMon", CStr(P_CBAnrListeShowAnrMon))
        C_XML.Write(XMLDoc, P_Def_Options, "CBUseAnrMon", CStr(P_CBUseAnrMon))
        C_XML.Write(XMLDoc, P_Def_Options, "CBCheckMobil", CStr(P_CBCheckMobil))
        'StoppUhr
        C_XML.Write(XMLDoc, P_Def_Options, "CBStoppUhrEinblenden", CStr(P_CBStoppUhrEinblenden))
        C_XML.Write(XMLDoc, P_Def_Options, "CBStoppUhrAusblenden", CStr(P_CBStoppUhrAusblenden))
        C_XML.Write(XMLDoc, P_Def_Options, "TBStoppUhr", CStr(P_TBStoppUhr))
        C_XML.Write(XMLDoc, P_Def_Options, "TBTelNrMaske", P_TBTelNrMaske)
        C_XML.Write(XMLDoc, P_Def_Options, "CBTelNrGruppieren", CStr(P_CBTelNrGruppieren))
        C_XML.Write(XMLDoc, P_Def_Options, "CBintl", CStr(P_CBintl))
        C_XML.Write(XMLDoc, P_Def_Options, "CBIgnoTelNrFormat", CStr(P_CBIgnoTelNrFormat))
        C_XML.Write(XMLDoc, P_Def_Options, "CBStoppUhrX", CStr(P_CBStoppUhrX))
        C_XML.Write(XMLDoc, P_Def_Options, "CBStoppUhrY", CStr(P_CBStoppUhrY))
        ' Phoner
        C_XML.Write(XMLDoc, P_Def_Phoner, "CBPhoner", CStr(P_CBPhoner))
        C_XML.Write(XMLDoc, P_Def_Phoner, "PhonerVerfügbar", CStr(P_PhonerVerfügbar))
        C_XML.Write(XMLDoc, P_Def_Phoner, "ComboBoxPhonerSIP", CStr(P_ComboBoxPhonerSIP))
        C_XML.Write(XMLDoc, P_Def_Phoner, "CBPhonerAnrMon", CStr(P_CBPhonerAnrMon))
        C_XML.Write(XMLDoc, P_Def_Phoner, "TBPhonerPasswort", P_TBPhonerPasswort)
        C_XML.Write(XMLDoc, P_Def_Phoner, "PhonerTelNameIndex", CStr(P_PhonerTelNameIndex))
        ' Statistik
        C_XML.Write(XMLDoc, P_Def_Statistics, "ResetZeit", CStr(P_StatResetZeit))
        C_XML.Write(XMLDoc, P_Def_Statistics, "Verpasst", CStr(P_StatVerpasst))
        C_XML.Write(XMLDoc, P_Def_Statistics, "Nichterfolgreich", CStr(P_StatNichtErfolgreich))
        C_XML.Write(XMLDoc, P_Def_Statistics, "Kontakt", CStr(P_StatKontakt))
        C_XML.Write(XMLDoc, P_Def_Statistics, "Journal", CStr(P_StatJournal))
        C_XML.Write(XMLDoc, P_Def_Journal, "SchließZeit", CStr(P_StatOLClosedZeit))
        ' Wählbox
        C_XML.Write(XMLDoc, P_Def_Options, "Anschluss", CStr(P_TelAnschluss))
        C_XML.Write(XMLDoc, P_Def_Options, "Festnetz", CStr(P_TelFestnetz))
        C_XML.Write(XMLDoc, P_Def_Options, "CLIR", CStr(P_TelCLIR))
        'FritzBox
        C_XML.Write(XMLDoc, P_Def_Options, "EncodingFritzBox", P_EncodingFritzBox.HeaderName)
        'Indizierung
        C_XML.Write(XMLDoc, P_Def_Options, "LLetzteIndizierung", CStr(P_LLetzteIndizierung))
        ' Notiz
        C_XML.Write(XMLDoc, P_Def_Options, "CBNote", CStr(P_CBNote))
        ' Fritz!Box Kommunikation
        C_XML.Write(XMLDoc, P_Def_Options, "RBFBComUPnP", CStr(P_RBFBComUPnP))

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