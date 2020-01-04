Imports System.Xml.Serialization
<Serializable()> Public Class Optionen
    <XmlIgnore> Public Property PArbeitsverzeichnis As String

#Region "Fritz!Box"
    ''' <summary>
    ''' Gibt die ermittelte Zeichencodierung der Fritzbox wieder. Der Wert wird automatisch ermittelt. 
    ''' </summary>
    <XmlIgnore> Public Property PEncodingFritzBox As Encoding
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
    <XmlElement("TBFBAdr")> Public Property PTBFBAdr As String

    ''' <summary>
    ''' Gibt eine korrekte Fritz!Box IP-Adresse zurück.
    ''' </summary>
    <XmlElement("ValidFBAdr")> Public Property PValidFBAdr As String

    '''' <summary>
    '''' Gibt an, ob eine Verbindung zur Fritz!Box trotz fehlgeschlagenen Pings aufgebaut werden soll.
    '''' </summary>
    <XmlElement("CBForceFBAdr")> Public Property PCBForceFBAdr As Boolean

    ''' <summary>
    ''' Gibt den einegegebenen Benutzernamen für das Login der Fritz!Box an. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    <XmlElement("TBBenutzer")> Public Property PTBBenutzer As String

    ''' <summary>
    ''' Gibt das eingegebene Passwort für das Login der Fritz!Box an. Dies ist eine Angabe, die der Nutzer in den Einstellungen wählen kann.
    ''' </summary>
    <XmlElement("TBPasswort")> Public Property PTBPasswort As String

    ''' <summary>
    ''' Eigenschaft für die hinterlege Ortsvorwahl
    ''' </summary>
    <XmlElement("TBOrtsKZ")> Public Property PTBOrtsKZ() As String

    ''' <summary>
    ''' Gibt die im Einstellungsdialog eingegebene Landesvorwahl zurück
    ''' </summary>
    <XmlElement("TBLandesKZ")> Public Property PTBLandesKZ() As String
#End Region

#Region "Grundeinstellungen - Formatierung von Telefonnummern"
    <XmlElement("TBTelNrMaske")> Public Property PTBTelNrMaske As String
    <XmlElement("CBTelNrGruppieren")> Public Property PCBTelNrGruppieren As Boolean
    <XmlElement("CBintl")> Public Property PCBintl As Boolean
    <XmlElement("CBIgnoTelNrFormat")> Public Property PCBIgnoTelNrFormat() As Boolean
#End Region

#Region "Grundeinstellungen - Einstellung für die Wählhilfe"
    <XmlElement("CBForceDialLKZ")> Public Property PCBForceDialLKZ As Boolean
    ''' <summary>
    ''' Gibt an, ob eine Amtsholung stets mitgewählt werden soll. Die Amtsholung wird in den Einstellungen festgelegt.
    ''' </summary>
    <XmlElement("TBAmt")> Public Property PTBAmt As String
    <XmlElement("CBDialPort")> Public Property PCBDialPort As Boolean
    <XmlElement("CBCbCunterbinden")> Public Property PCBCbCunterbinden As Boolean
    <XmlElement("CBCheckMobil")> Public Property PCBCheckMobil As Boolean
    <XmlElement("TelAnschluss")> Public Property PTelAnschluss As String
    <XmlElement("PCBCLIR")> Public Property PCBCLIR As Boolean
#End Region
#End Region

#Region "Anrufmonitor"
#Region "Anrufmonitor - Einstellungen für den Anrufmonitor"
    <XmlElement("CBUseAnrMon")> Public Property PCBUseAnrMon As Boolean
    <XmlElement("CBAnrMonAuto")> Public Property PCBAnrMonAuto As Boolean
    <XmlElement("CBAutoClose")> Public Property PCBAutoClose As Boolean
    <XmlElement("TBEnblDauer")> Public Property PTBEnblDauer As Integer
    <XmlElement("CBAnrMonCloseAtDISSCONNECT")> Public Property PCBAnrMonCloseAtDISSCONNECT As Boolean
    <XmlElement("CBAnrMonZeigeKontakt")> Public Property PCBAnrMonZeigeKontakt As Boolean
    <XmlElement("CBAnrMonContactImage")> Public Property PCBAnrMonContactImage As Boolean
    <XmlElement("CBShowMSN")> Public Property PCBShowMSN As Boolean
#End Region
#End Region

#Region "Einstellungen für die Kontaktsuche"
#Region "Einstellungen für die Kontaktsuche - Kontaktsuche in Outlook (Indizierung)"
    <XmlElement("CBKontaktSucheHauptOrdner")> Public Property PCBKontaktSucheHauptOrdner As Boolean
    <XmlElement("CBKontaktSucheFritzBox")> Public Property PCBKontaktSucheFritzBox As Boolean
#End Region
#Region "Einstellungen für die Kontaktsuche - Rückwärtssuche (RWS)"
    <XmlElement("CBRWS")> Public Property PCBRWS As Boolean
    <XmlElement("CBKErstellen")> Public Property PCBKErstellen As Boolean
    <XmlElement("CBRWSIndex")> Public Property PCBRWSIndex As Boolean
    <XmlElement("TVKontaktOrdnerEntryID")> Public Property PTVKontaktOrdnerEntryID As String
    <XmlElement("TVKontaktOrdnerStoreID")> Public Property PTVKontaktOrdnerStoreID As String
#End Region

#End Region

#Region "Auswertung der Fritz!box Anrufliste"
#Region "Auswertung der Fritz!box Anrufliste - Outlook Journal"
    <XmlElement("CBJournal")> Public Property PCBJournal As Boolean
#End Region

#Region "Auswertung der Fritz!box Anrufliste - Auswertung der Anrufliste"
    <XmlElement("LetzterJournalEintrag")> Public Property PLetzterJournalEintrag As Date
    <XmlElement("LetzterJournalEintragID")> Public Property PLetzterJournalEintragID As Integer
    <XmlElement("CBAutoAnrList")> Public Property PCBAutoAnrList As Boolean
    <XmlElement("CBAnrListeUpdateCallLists")> Public Property PCBAnrListeUpdateCallLists As Boolean
    <XmlElement("CBAnrListeShowAnrMon")> Public Property PCBAnrListeShowAnrMon As Boolean
    <XmlElement("TBAnrBeantworterTimeout")> Public Property PTBAnrBeantworterTimeout As Integer
#End Region
#Region "Auswertung der Fritz!box Anrufliste - Anruflisten"
    <XmlElement("TBNumEntryList")> Public Property PTBNumEntryList As Integer
#End Region
#End Region

#Region "Logging"
    <XmlElement("CBLogFile")> Public Property PCBLogFile As Boolean
#End Region
End Class
