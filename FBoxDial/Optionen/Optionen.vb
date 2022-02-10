Imports System.Xml.Serialization
<Serializable()> Public Class Optionen

#Region "Grundeinstellungen"
#Region "Grundeinstellungen - Erforderliche Angaben"
    ''' <summary>
    ''' Gibt die eingegebene Fritz!Box IP-Adresse an. Dies ist eine Angabe, die der Nutzer in den Einstellungen ändern kann.
    ''' </summary>
    <XmlElement("TBFBAdr")> Public Property TBFBAdr As String = FritzBoxDefault.DfltFritzBoxHostName

    ''' <summary>
    ''' Gibt eine korrekte Fritz!Box IP-Adresse zurück.
    ''' </summary>
    <XmlElement("ValidFBAdr")> Public Property ValidFBAdr As String

    ''' <summary>
    ''' Gibt den einegegebenen Benutzernamen für das Login der Fritz!Box an. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    <XmlElement("TBBenutzer")> Public Property TBBenutzer As String

    ''' <summary>
    ''' Gibt das eingegebene Passwort für das Login der Fritz!Box an. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    <XmlElement("TBPasswort")> Public Property TBPasswort As String

#End Region

#Region "Grundeinstellungen - Formatierung von Telefonnummern"
    ''' <summary>
    ''' Gibt die Maske zurück, nach der Telefonnummern formatiert werden.
    ''' </summary>
    <XmlElement("TBTelNrMaske")> Public Property TBTelNrMaske As String = "%L (%O) %N - %D"

    ''' <summary>
    ''' Gibt an, ob Telefonnummern zur besseren lesbarkeit gruppiert werden sollen.
    ''' </summary>
    <XmlElement("CBTelNrGruppieren")> Public Property CBTelNrGruppieren As Boolean = True

    ''' <summary>
    ''' Gibt an, ob immer eine internationale Vorwahl gewählt werden soll.
    ''' </summary>
    <XmlElement("CBintl")> Public Property CBintl As Boolean = False
#End Region

#Region "Grundeinstellungen - Einstellung für die Wählhilfe"
    <XmlElement("CBForceDialLKZ")> Public Property CBForceDialLKZ As Boolean = False

    ''' <summary>
    ''' Gibt an, ob eine Amtsholung stets mitgewählt werden soll. Die Amtsholung wird in den Einstellungen festgelegt.
    ''' </summary>
    <XmlElement("TBPräfix")> Public Property TBPräfix As String = String.Empty

    <XmlElement("CBCheckMobil")> Public Property CBCheckMobil As Boolean = True

    <XmlElement("CBCLIR")> Public Property CBCLIR As Boolean = False

    <XmlElement("CBCloseWClient")> Public Property CBCloseWClient As Boolean = True

    <XmlElement("TBWClientEnblDauer")> Public Property TBWClientEnblDauer As Integer = 10
#End Region

#Region "Grundeinstlleungen - Tweaks"
    ''' <summary>
    ''' Gibt an, ob die Tastatursteuerung aktiviert werden soll.
    ''' </summary>
    <XmlElement("CBKeyboard")> Public Property CBKeyboard As Boolean = True

    ''' <summary>
    ''' Gibt an, ob bei der Prüfung des Enabled-State des Wählenbuttons im Ribbon deaktiviert wird.
    ''' Dies ist bei verzögertem E-Mail versand notwendig.
    ''' </summary>
    <XmlElement("CBDisableMailCheck")> Public Property CBDisableMailCheck As Boolean = False

    ''' <summary>
    ''' Timeout für die Netzwerkschnittstelle. Normalfall 120 ms. 
    ''' </summary>
    <XmlElement("TBNetworkTimeout")> Public Property TBNetworkTimeout As Integer = 120
#End Region

#Region "Design"
    <XmlElement("CBoxDesignMode")> Public Property CBoxDesignMode As DesignModes = DesignModes.Light

#End Region
#End Region

#Region "Anrufmonitor"
#Region "Anrufmonitor - Einstellungen für den Anrufmonitor"
    <XmlElement("CBAnrMonAuto")> Public Property CBAnrMonAuto As Boolean = False
    ''' <summary>
    ''' Angabe, ob der Anrufmonitor automatisch geschlossen werden soll.
    ''' </summary>
    <XmlElement("CBAutoClose")> Public Property CBAutoClose As Boolean = True
    ''' <summary>
    ''' Einblenddauer des Anrufmonitors in Sekunden.
    ''' </summary>
    <XmlElement("TBEnblDauer")> Public Property TBEnblDauer As Integer = 10
    ''' <summary>
    ''' Angabe, ob der Anrufmonitor bei Rufannahme ausgeblendet werden soll.
    ''' </summary>
    <XmlElement("CBAnrMonHideCONNECT")> Public Property CBAnrMonHideCONNECT As Boolean = False
    ''' <summary>
    ''' Angabe, ob der Anrufmonitor eingeblendet werden soll, falls sich der Anrufer auf der Sperrliste befindet
    ''' </summary>
    <XmlElement("CBAnrMonBlockNr")> Public Property CBAnrMonBlockNr As Boolean = False
    ''' <summary>
    ''' Angabe, ob der Kontakt angezeigt werden soll
    ''' </summary>
    <XmlElement("CBAnrMonZeigeKontakt")> Public Property CBAnrMonZeigeKontakt As Boolean = False
    ''' <summary>
    ''' Angabe, ob Anrufe, die an einen Anrufbeantworter gegangen sind, als verpasst behandelt werden sollen.
    ''' </summary>
    <XmlElement("CBIsTAMMissed")> Public Property CBIsTAMMissed As Boolean = True
    ''' <summary>
    ''' Angabe, ob ein Kontaktbild angezeigt werden soll.
    ''' </summary>
    <XmlElement("CBAnrMonContactImage")> Public Property CBAnrMonContactImage As Boolean = True
    ''' <summary>
    ''' Angabe, ob die Farben des Anrufmonitors geändert werden soll
    ''' </summary>
    <XmlElement("CBSetAnrMonBColor")> Public Property CBSetAnrMonBColor As Boolean = False
    ''' <summary>
    ''' Hintergrundfarbe als HEX-String #00000000
    ''' </summary>
    <XmlElement("TBAnrMonBColorHex")> Public Property TBAnrMonBColorHex As String
    ''' <summary>
    ''' Schriftfarbe als HEX-String #00000000
    ''' </summary>
    <XmlElement("TBAnrMonFColorHex")> Public Property TBAnrMonFColorHex As String
    ''' <summary>
    ''' Positionskorrektur des Anrufmonitors in X-Richtung
    ''' </summary>
    <XmlElement("TBAnrMonModPosX")> Public Property TBAnrMonModPosX As Double = 0.0R
    ''' <summary>
    ''' Positionskorrektur des Anrufmonitors in Y-Richtung
    ''' </summary>
    <XmlElement("TBAnrMonModPosY")> Public Property TBAnrMonModPosY As Double = 0.0R
    ''' <summary>
    ''' Grundabstand des Anrufmonitors
    ''' </summary>
    <XmlElement("TBAnrMonAbstand")> Public Property TBAnrMonAbstand As Double = 10.0R
#Region "CallPane"
    ''' <summary>
    ''' Angabe, ob verpasste Anrufe im CallPane angezeigt werden sollen.
    ''' </summary>
    <XmlElement("CBShowMissedCallPane")> Public Property CBShowMissedCallPane As Boolean = False
    ''' <summary>
    ''' Angabe, ob das CallPane automatisch geschlossen werden soll, wenn Anrufliste leer ist.
    ''' </summary>
    <XmlElement("CBCloseEmptyCallPane")> Public Property CBCloseEmptyCallPane As Boolean = True
    ''' <summary>
    ''' Angabe, ob beim Schließen des CallPane alle enthaltenen Anrufe entfernt werden sollen.
    ''' </summary>
    <XmlElement("CBClearCallPaneAtClose")> Public Property CBClearCallPaneAtClose As Boolean = True
    ''' <summary>
    ''' Angabe, ob Seiten Fenster bei Outlookstart bereits eingeblendet werden soll.
    ''' </summary>
    <XmlElement("CBShowCallPaneAtStart")> Public Property CBShowCallPaneAtStart As Boolean = False
    ''' <summary>
    ''' Gibt die Standardbreite des Pane bei Start an.
    ''' </summary>
    <XmlElement("TBCallPaneStartWidth")> Public Property TBCallPaneStartWidth As Integer = 400
#End Region
#End Region
#Region "Stoppuhr"
    ''' <summary>
    ''' Angabe, ob die Stoppuhr angezeigt werden soll
    ''' </summary>
    <XmlElement("CBStoppUhrEinblenden")> Public Property CBStoppUhrEinblenden As Boolean = False
    ''' <summary>
    ''' Angabe, ob die Stoppuhr nach dem Telefonat automatisch ausgeblendet werden soll
    ''' </summary>
    <XmlElement("CBStoppUhrAusblenden")> Public Property CBStoppUhrAusblenden As Boolean = False
    ''' <summary>
    ''' Zeitangabe, nachdem die Stoppuhr ausgeblendet werden soll. (Korresbondiert zu <see cref="CBStoppUhrAusblenden"/>)
    ''' </summary>
    <XmlElement("TBStoppUhrAusblendverzögerung")> Public Property TBStoppUhrAusblendverzögerung As Integer = 10
    ''' <summary>
    ''' Angabe, ob die Farben der Stoppuhr geändert werden soll
    ''' </summary>
    <XmlElement("CBSetStoppUhrBColor")> Public Property CBSetStoppUhrBColor As Boolean = False
    ''' <summary>
    ''' Hintergrundfarbe als HEX-String #00000000
    ''' </summary>
    <XmlElement("TBStoppUhrBColorHex")> Public Property TBStoppUhrBColorHex As String
    ''' <summary>
    ''' Schriftfarbe als HEX-String #00000000
    ''' </summary>
    <XmlElement("TBStoppUhrFColorHex")> Public Property TBStoppUhrFColorHex As String
#End Region
#End Region

#Region "Einstellungen für die Kontaktsuche"

    <XmlElement("OutlookOrdner")> Public Property OutlookOrdner As OutlookOrdnerListe

#Region "Einstellungen für die Kontaktsuche - Kontaktsuche in Outlook (Indizierung)"

    <XmlElement("CBSucheUnterordner")> Public Property CBSucheUnterordner As Boolean = False

    <XmlElement("CBKontaktSucheFritzBox")> Public Property CBKontaktSucheFritzBox As Boolean = False

#End Region

#Region "Einstellungen für die Kontaktsuche - Rückwärtssuche (RWS)"
    <XmlElement("CBRWS")> Public Property CBRWS As Boolean = False

    <XmlElement("CBKErstellen")> Public Property CBKErstellen As Boolean = False

    <XmlElement("CBRWSIndex")> Public Property CBRWSIndex As Boolean = True

    <XmlElement("CBNoContactNotes")> Public Property CBNoContactNotes As Boolean = False
#End Region

#Region "Einstellungen für die Kontaktsuche - tellows"
    <XmlElement("TBTellowsAPIKey")> Public Property TBTellowsAPIKey As String = String.Empty
    <XmlElement("CBTellows")> Public Property CBTellows As Boolean = False
    <XmlElement("CBTellowsAnrMonMinScore")> Public Property CBTellowsAnrMonMinScore As Integer = 7
    <XmlElement("CBTellowsAnrMonMinComments")> Public Property CBTellowsAnrMonMinComments As Integer = 3
    <XmlElement("CBTellowsAnrMonColor")> Public Property CBTellowsAnrMonColor As Boolean = False
    <XmlElement("CBTellowsAutoFBBlockList")> Public Property CBTellowsAutoFBBlockList As Boolean = False
    <XmlElement("CBTellowsAutoScoreFBBlockList")> Public Property CBTellowsAutoScoreFBBlockList As Integer = 7
    <XmlElement("CBTellowsAutoUpdateScoreList")> Public Property CBTellowsAutoUpdateScoreList As Boolean = False
    <XmlElement("CBTellowsEntryNumberCount")> Public Property CBTellowsEntryNumberCount As Integer = 10
    <XmlElement("LetzteSperrlistenaktualsierung")> Public Property LetzteSperrlistenAktualisierung As Date = Now
#End Region
#End Region

#Region "Auswertung der Fritz!box Anrufliste"
#Region "Auswertung der Fritz!box Anrufliste - Outlook Journal"
    <XmlElement("CBJournal")> Public Property CBJournal As Boolean = True
    <XmlElement("CBJournalBlockNr")> Public Property CBJournalBlockNr As Boolean = False
#End Region

#Region "Auswertung der Fritz!box Anrufliste - Auswertung der Anrufliste"

    <XmlElement("LetzteAuswertungAnrList")> Public Property LetzteAuswertungAnrList As Date = Now

    <XmlElement("CBAutoAnrList")> Public Property CBAutoAnrList As Boolean = False

    <XmlElement("CBAnrListeUpdateCallLists")> Public Property CBAnrListeUpdateCallLists As Boolean = False

#End Region
#Region "Auswertung der Fritz!box Anrufliste - Anruflisten"
    <XmlElement("TBNumEntryList")> Public Property TBNumEntryList As Integer = 10
#End Region
#End Region

#Region "Logging"
#If DEBUG Then
    <XmlElement("CBoxMinLogLevel")> Public Property CBoxMinLogLevel As String = LogLevel.Debug.Name
#Else
    <XmlElement("CBoxMinLogLevel")> Public Property CBoxMinLogLevel As String = LogLevel.Info.Name
#End If
#End Region

#Region "SoftPhones"
#Region "Phoner"
    <XmlElement("TBPhonerPasswort")> Public Property TBPhonerPasswort As String = String.Empty
    <XmlElement("CBPhoner")> Public Property CBPhoner As Boolean = False
    <XmlElement("CBPhonerSuffix")> Public Property CBPhonerSuffix As Boolean = True
#End Region

#Region "MicroSIP"
    <XmlElement("TBMicroSIPPath")> Public Property TBMicroSIPPath As String = String.Empty
    <XmlElement("CBMicroSIP")> Public Property CBMicroSIP As Boolean = False
    <XmlElement("CBMicroSIPSuffix")> Public Property CBMicroSIPSuffix As Boolean = True

#End Region
#End Region

    Public Sub New()
        OutlookOrdner = New OutlookOrdnerListe
    End Sub
End Class
