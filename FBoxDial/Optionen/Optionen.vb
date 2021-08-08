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
    <XmlElement("TBPräfix")> Public Property TBPräfix As String = DfltStringEmpty

    <XmlElement("CBCheckMobil")> Public Property CBCheckMobil As Boolean = True

    <XmlElement("CBCLIR")> Public Property CBCLIR As Boolean = False

    <XmlElement("CBCloseWClient")> Public Property CBCloseWClient As Boolean = True

    <XmlElement("TBWClientEnblDauer")> Public Property TBWClientEnblDauer As Integer = 10
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
    ''' Angabe, ob der Kontakt angezeigt werden soll
    ''' </summary>
    <XmlElement("CBAnrMonZeigeKontakt")> Public Property CBAnrMonZeigeKontakt As Boolean = False
    ''' <summary>
    ''' Angabe, ob ein Kontaktbild angezeigt werden soll.
    ''' </summary>
    <XmlElement("CBAnrMonContactImage")> Public Property CBAnrMonContactImage As Boolean = True
    <XmlElement("CBSetAnrMonBColor")> Public Property CBSetAnrMonBColor As Boolean = False
    <XmlElement("TBAnrMonBColorHex")> Public Property TBAnrMonBColorHex As String
    <XmlElement("TBAnrMonFColorHex")> Public Property TBAnrMonFColorHex As String
#End Region

#Region "Stoppuhr"
    <XmlElement("CBStoppUhrEinblenden")> Public Property CBStoppUhrEinblenden As Boolean = False
    <XmlElement("CBStoppUhrAusblenden")> Public Property CBStoppUhrAusblenden As Boolean = False
    <XmlElement("TBStoppUhrAusblendverzögerung")> Public Property TBStoppUhrAusblendverzögerung As Integer = 10
    <XmlElement("CBSetStoppUhrBColor")> Public Property CBSetStoppUhrBColor As Boolean = False
    <XmlElement("TBStoppUhrBColorHex")> Public Property TBStoppUhrBColorHex As String
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

#End Region

#Region "Einstellungen für die Kontaktsuche - tellows"
    <XmlElement("TBTellowsAPIKey")> Public Property TBTellowsAPIKey As String = DfltStringEmpty
    <XmlElement("CBTellows")> Public Property CBTellows As Boolean = False
    <XmlElement("CBTellowsAnrMonMinScore")> Public Property CBTellowsAnrMonMinScore As Integer = 7
    <XmlElement("CBTellowsAnrMonMinComments")> Public Property CBTellowsAnrMonMinComments As Integer = 3
    <XmlElement("CBTellowsAnrMonColor")> Public Property CBTellowsAnrMonColor As Boolean = False
    <XmlElement("CBTellowsAutoFBBlockList")> Public Property CBTellowsAutoFBBlockList As Boolean = False
    <XmlElement("CBTellowsAutoScoreFBBlockList")> Public Property CBTellowsAutoScoreFBBlockList As Integer = 7
    <XmlElement("CBTellowsAutoUpdateScoreList")> Public Property CBTellowsAutoUpdateScoreList As Boolean = False
    <XmlElement("CBTellowsEntryNumberCount")> Public Property CBTellowsEntryNumberCount As Integer = 10
    <XmlElement("LetzteSperrlistenaktualsierung")> Public Property LetzteSperrlistenaktualisierung As Date = Now
#End Region
#End Region

#Region "Auswertung der Fritz!box Anrufliste"
#Region "Auswertung der Fritz!box Anrufliste - Outlook Journal"
    <XmlElement("CBJournal")> Public Property CBJournal As Boolean = True

#End Region

#Region "Auswertung der Fritz!box Anrufliste - Auswertung der Anrufliste"

    <XmlElement("LetzterJournalEintrag")> Public Property LetzterJournalEintrag As Date = Now

    <XmlElement("LetzterJournalEintragID")> Public Property LetzterJournalEintragID As Integer = 0

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
    <XmlElement("TBPhonerPasswort")> Public Property TBPhonerPasswort As String = DfltStringEmpty
    <XmlElement("CBPhoner")> Public Property CBPhoner As Boolean = False

#End Region

#Region "MicroSIP"
    <XmlElement("TBMicroSIPPath")> Public Property TBMicroSIPPath As String = DfltStringEmpty
    <XmlElement("CBMicroSIP")> Public Property CBMicroSIP As Boolean = False
#End Region
#End Region

    Public Sub New()
        OutlookOrdner = New OutlookOrdnerListe
    End Sub
End Class
