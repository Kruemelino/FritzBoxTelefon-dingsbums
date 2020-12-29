Imports System.Xml.Serialization
<Serializable()> Public Class Optionen
    Inherits NotifyBase

    <XmlIgnore> Public Property Arbeitsverzeichnis As String

#Region "Fritz!Box"
    ''' <summary>
    ''' Gibt die ermittelte Zeichencodierung der Fritzbox wieder. Der Wert wird automatisch ermittelt. 
    ''' </summary>
    <XmlIgnore> Public Property EncodingFritzBox As Encoding
        Get
            If PCodePageFritzBox.IsZero Then PCodePageFritzBox = FritzBoxDefault.PDfltCodePageFritzBox
            Return Encoding.GetEncoding(PCodePageFritzBox)
        End Get
        Set(value As Encoding)
            PCodePageFritzBox = value.CodePage
        End Set
    End Property
    <XmlElement("EncodingFritzBox")> Public Property PCodePageFritzBox As Integer
#End Region

#Region "Grundeinstellungen"
#Region "Grundeinstellungen - Erforderliche Angaben"
    ''' <summary>
    ''' Gibt die eingegebene Fritz!Box IP-Adresse an. Dies ist eine Angabe, die der Nutzer in den Einstellungen ändern kann.
    ''' </summary>
    <XmlElement("TBFBAdr")> Public Property TBFBAdr As String

    ''' <summary>
    ''' Gibt eine korrekte Fritz!Box IP-Adresse zurück.
    ''' </summary>
    <XmlElement("ValidFBAdr")> Public Property ValidFBAdr As String

    '''' <summary>
    '''' Gibt an, ob eine Verbindung zur Fritz!Box trotz fehlgeschlagenen Pings aufgebaut werden soll.
    '''' </summary>
    '<XmlElement("CBForceFBAdr")> Public Property PCBForceFBAdr As Boolean

    ''' <summary>
    ''' Gibt den einegegebenen Benutzernamen für das Login der Fritz!Box an. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    <XmlElement("TBBenutzer")> Public Property TBBenutzer As String

    ''' <summary>
    ''' Gibt das eingegebene Passwort für das Login der Fritz!Box an. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    <XmlElement("TBPasswort")> Public Property TBPasswort As String

    ''' <summary>
    ''' Eigenschaft für die hinterlege Ortsvorwahl
    ''' </summary>
    <XmlElement("TBOrtsKZ")> Public Property TBOrtsKZ() As String

    ''' <summary>
    ''' Gibt die im Einstellungsdialog eingegebene Landesvorwahl zurück
    ''' </summary>
    <XmlElement("TBLandesKZ")> Public Property TBLandesKZ() As String
#End Region

#Region "Grundeinstellungen - Formatierung von Telefonnummern"
    ''' <summary>
    ''' Gibt die Maske zurück, nach der Telefonnummern formatiert werden.
    ''' </summary>
    <XmlElement("TBTelNrMaske")> Public Property TBTelNrMaske As String
    ''' <summary>
    ''' Gibt an, ob Telefonnummern zur besseren lesbarkeit gruppiert werden sollen.
    ''' </summary>
    <XmlElement("CBTelNrGruppieren")> Public Property CBTelNrGruppieren As Boolean
    ''' <summary>
    ''' Gibt an, ob immer eine internationale Vorwahl gewählt werden soll.
    ''' </summary>
    <XmlElement("CBintl")> Public Property CBintl As Boolean
#End Region

#Region "Grundeinstellungen - Einstellung für die Wählhilfe"
    <XmlElement("CBForceDialLKZ")> Public Property CBForceDialLKZ As Boolean
    ''' <summary>
    ''' Gibt an, ob eine Amtsholung stets mitgewählt werden soll. Die Amtsholung wird in den Einstellungen festgelegt.
    ''' </summary>
    <XmlElement("TBAmt")> Public Property TBAmt As String
    <XmlElement("CBCheckMobil")> Public Property CBCheckMobil As Boolean
    <XmlElement("PCBCLIR")> Public Property CBCLIR As Boolean
    <XmlElement("PCBCloseWClient")> Public Property CBCloseWClient As Boolean
    <XmlElement("TBWClientEnblDauer")> Public Property TBWClientEnblDauer As Integer
#End Region
#End Region

#Region "Anrufmonitor"
#Region "Anrufmonitor - Einstellungen für den Anrufmonitor"
    <XmlElement("CBAnrMonAuto")> Public Property CBAnrMonAuto As Boolean

    ''' <summary>
    ''' Angabe, ob der Anrufmonitor automatisch geschlossen werden soll.
    ''' </summary>
    <XmlElement("CBAutoClose")> Public Property CBAutoClose As Boolean
    ''' <summary>
    ''' Einblenddauer des Anrufmonitors in Sekunden.
    ''' </summary>
    <XmlElement("TBEnblDauer")> Public Property TBEnblDauer As Integer
    ''' <summary>
    ''' Angabe, ob der Kontakt Angezeigt werden soll
    ''' </summary>
    <XmlElement("CBAnrMonZeigeKontakt")> Public Property CBAnrMonZeigeKontakt As Boolean
    ''' <summary>
    ''' Angabe, ob ein Kontaktbild angezeigt werden soll.
    ''' </summary>
    <XmlElement("CBAnrMonContactImage")> Public Property CBAnrMonContactImage As Boolean
#End Region
#End Region

#Region "Einstellungen für die Kontaktsuche"

    <XmlElement("OutlookOrdner")> Public Property OutlookOrdner As OutlookOrdnerListe
#Region "Einstellungen für die Kontaktsuche - Kontaktsuche in Outlook (Indizierung)"
    <XmlElement("CBSucheUnterordner")> Public Property CBSucheUnterordner As Boolean
    <XmlElement("CBKontaktSucheFritzBox")> Public Property CBKontaktSucheFritzBox As Boolean
#End Region
#Region "Einstellungen für die Kontaktsuche - Rückwärtssuche (RWS)"
    <XmlElement("CBRWS")> Public Property CBRWS As Boolean
    <XmlElement("CBKErstellen")> Public Property CBKErstellen As Boolean
    <XmlElement("CBRWSIndex")> Public Property CBRWSIndex As Boolean

    <XmlElement("CBUseLegacySearch")> Public Property PCBUseLegacySearch As Boolean
    <XmlElement("CBUseLegacyUserProp")> Public Property PCBUseLegacyUserProp As Boolean
#End Region

#End Region

#Region "Auswertung der Fritz!box Anrufliste"
#Region "Auswertung der Fritz!box Anrufliste - Outlook Journal"
    <XmlElement("CBJournal")> Public Property CBJournal As Boolean
#End Region

#Region "Auswertung der Fritz!box Anrufliste - Auswertung der Anrufliste"
    <XmlElement("LetzterJournalEintrag")> Public Property LetzterJournalEintrag As Date
    <XmlElement("LetzterJournalEintragID")> Public Property LetzterJournalEintragID As Integer
    <XmlElement("CBAutoAnrList")> Public Property CBAutoAnrList As Boolean
    <XmlElement("CBAnrListeUpdateCallLists")> Public Property CBAnrListeUpdateCallLists As Boolean
    '<XmlElement("CBAnrListeShowAnrMon")> Public Property PCBAnrListeShowAnrMon As Boolean
    '<XmlElement("TBAnrBeantworterTimeout")> Public Property PTBAnrBeantworterTimeout As Integer
#End Region
#Region "Auswertung der Fritz!box Anrufliste - Anruflisten"
    <XmlElement("TBNumEntryList")> Public Property TBNumEntryList As Integer
#End Region
#End Region

#Region "Logging"
    <XmlElement("CBoxMinLogLevel")> Public Property CBoxMinLogLevel As String
#End Region

#Region "Phoner"
    <XmlElement("TBPhonerPasswort")> Public Property TBPhonerPasswort As String
    <XmlElement("CBPhoner")> Public Property CBPhoner As Boolean
#End Region

    Public Sub New()
        OutlookOrdner = New OutlookOrdnerListe
    End Sub
End Class
