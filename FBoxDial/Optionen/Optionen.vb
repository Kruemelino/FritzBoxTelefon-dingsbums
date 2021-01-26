Imports System.Xml.Serialization
<Serializable()> Public Class Optionen
    Inherits NotifyBase

    <XmlIgnore> Public Property Arbeitsverzeichnis As String

#Region "Grundeinstellungen"
#Region "Grundeinstellungen - Erforderliche Angaben"
    Private _TBFBAdr As String
    ''' <summary>
    ''' Gibt die eingegebene Fritz!Box IP-Adresse an. Dies ist eine Angabe, die der Nutzer in den Einstellungen ändern kann.
    ''' </summary>
    <XmlElement("TBFBAdr")> Public Property TBFBAdr As String
        Get
            Return GetProperty(_TBFBAdr, FritzBoxDefault.DfltFritzBoxIPAdress)
        End Get
        Set
            _TBFBAdr = Value
        End Set
    End Property

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
    Private _TBTelNrMaske As String
    Private _CBTelNrGruppieren As Boolean
    Private _CBintl As Boolean

    ''' <summary>
    ''' Gibt die Maske zurück, nach der Telefonnummern formatiert werden.
    ''' </summary>
    <XmlElement("TBTelNrMaske")> Public Property TBTelNrMaske As String
        Get
            Return GetProperty(_TBTelNrMaske, DefaultWerte.DfltTBTelNrMaske)
        End Get
        Set
            _TBTelNrMaske = Value
        End Set
    End Property

    ''' <summary>
    ''' Gibt an, ob Telefonnummern zur besseren lesbarkeit gruppiert werden sollen.
    ''' </summary>
    <XmlElement("CBTelNrGruppieren")> Public Property CBTelNrGruppieren As Boolean
        Get
            Return GetProperty(_CBTelNrGruppieren, DefaultWerte.DfltCBTelNrGruppieren)
        End Get
        Set
            _CBTelNrGruppieren = Value
        End Set
    End Property

    ''' <summary>
    ''' Gibt an, ob immer eine internationale Vorwahl gewählt werden soll.
    ''' </summary>
    <XmlElement("CBintl")> Public Property CBintl As Boolean
        Get
            Return GetProperty(_CBintl, DefaultWerte.DfltCBintl)
        End Get
        Set
            _CBintl = Value
        End Set
    End Property
#End Region

#Region "Grundeinstellungen - Einstellung für die Wählhilfe"
    Private _CBForceDialLKZ As Boolean
    Private _TBAmt As String
    Private _CBCheckMobil As Boolean
    Private _CBCLIR As Boolean
    Private _CBCloseWClient As Boolean
    Private _TBWClientEnblDauer As Integer

    <XmlElement("CBForceDialLKZ")> Public Property CBForceDialLKZ As Boolean
        Get
            Return GetProperty(_CBForceDialLKZ, DefaultWerte.DfltCBForceDialLKZ)
        End Get
        Set
            _CBForceDialLKZ = Value
        End Set
    End Property

    ''' <summary>
    ''' Gibt an, ob eine Amtsholung stets mitgewählt werden soll. Die Amtsholung wird in den Einstellungen festgelegt.
    ''' </summary>
    <XmlElement("TBAmt")> Public Property TBAmt As String
        Get
            Return GetProperty(_TBAmt, DefaultWerte.DfltTBAmt)
        End Get
        Set
            _TBAmt = Value
        End Set
    End Property

    <XmlElement("CBCheckMobil")> Public Property CBCheckMobil As Boolean
        Get
            Return GetProperty(_CBCheckMobil, DefaultWerte.DfltCBCheckMobil)
        End Get
        Set
            _CBCheckMobil = Value
        End Set
    End Property

    <XmlElement("CBCLIR")> Public Property CBCLIR As Boolean
        Get
            Return GetProperty(_CBCLIR, DefaultWerte.DfltCLIR)
        End Get
        Set
            _CBCLIR = Value
        End Set
    End Property

    <XmlElement("CBCloseWClient")> Public Property CBCloseWClient As Boolean
        Get
            Return GetProperty(_CBCloseWClient, DefaultWerte.DfltCBCloseWClient)
        End Get
        Set
            _CBCloseWClient = Value
        End Set
    End Property

    <XmlElement("TBWClientEnblDauer")> Public Property TBWClientEnblDauer As Integer
        Get
            Return GetProperty(_TBWClientEnblDauer, DefaultWerte.DfltTBWClientEnblDauer)
        End Get
        Set
            _TBWClientEnblDauer = Value
        End Set
    End Property
#End Region
#End Region

#Region "Anrufmonitor"
#Region "Anrufmonitor - Einstellungen für den Anrufmonitor"
    Private _CBAnrMonAuto As Boolean
    Private _CBAutoClose As Boolean
    Private _TBEnblDauer As Integer
    Private _CBAnrMonZeigeKontakt As Boolean
    Private _CBAnrMonContactImage As Boolean
    Private _CBAnrMonVollbildAnzeigen As Boolean

    <XmlElement("CBAnrMonAuto")> Public Property CBAnrMonAuto As Boolean
        Get
            Return GetProperty(_CBAnrMonAuto, DefaultWerte.DfltCBAnrMonAuto)
        End Get
        Set
            _CBAnrMonAuto = Value
        End Set
    End Property

    ''' <summary>
    ''' Angabe, ob der Anrufmonitor automatisch geschlossen werden soll.
    ''' </summary>
    <XmlElement("CBAutoClose")> Public Property CBAutoClose As Boolean
        Get
            Return GetProperty(_CBAutoClose, DefaultWerte.DfltCBAutoClose)
        End Get
        Set
            _CBAutoClose = Value
        End Set
    End Property

    ''' <summary>
    ''' Einblenddauer des Anrufmonitors in Sekunden.
    ''' </summary>
    <XmlElement("TBEnblDauer")> Public Property TBEnblDauer As Integer
        Get
            Return GetProperty(_TBEnblDauer, DefaultWerte.DfltTBEnblDauer)
        End Get
        Set
            _TBEnblDauer = Value
        End Set
    End Property

    ''' <summary>
    ''' Angabe, ob der Kontakt angezeigt werden soll
    ''' </summary>
    <XmlElement("CBAnrMonZeigeKontakt")> Public Property CBAnrMonZeigeKontakt As Boolean
        Get
            Return GetProperty(_CBAnrMonZeigeKontakt, DefaultWerte.DfltCBAnrMonZeigeKontakt)
        End Get
        Set
            _CBAnrMonZeigeKontakt = Value
        End Set
    End Property

    ''' <summary>
    ''' Angabe, ob ein Kontaktbild angezeigt werden soll.
    ''' </summary>
    <XmlElement("CBAnrMonContactImage")> Public Property CBAnrMonContactImage As Boolean
        Get
            Return GetProperty(_CBAnrMonContactImage, DefaultWerte.DfltCBAnrMonContactImage)
        End Get
        Set
            _CBAnrMonContactImage = Value
        End Set
    End Property

    ''' <summary>
    ''' Angabe, ob der Anrufmonitor bei Vollbildanwendungen eingeblendet werden soll.
    ''' </summary>
    <XmlElement("CBAnrMonVollbildAnzeigen")> Public Property CBAnrMonVollbildAnzeigen As Boolean
        Get
            Return GetProperty(_CBAnrMonVollbildAnzeigen, DefaultWerte.DfltCBAnrMonVollBildAnzeigen)
        End Get
        Set
            _CBAnrMonVollbildAnzeigen = Value
        End Set
    End Property
#End Region

#Region "Stoppuhr"
    Private _CBStoppUhrEinblenden As Boolean
    Private _CBStoppUhrAusblenden As Boolean
    Private _TBStoppUhrAusblendverzögerung As Integer
    Public Property CBStoppUhrEinblenden As Boolean
        Get
            Return GetProperty(_CBStoppUhrEinblenden, DefaultWerte.DfltCBStoppUhrEinblenden)
        End Get
        Set
            _CBStoppUhrEinblenden = Value
        End Set
    End Property

    Public Property CBStoppUhrAusblenden As Boolean
        Get
            Return GetProperty(_CBStoppUhrAusblenden, DefaultWerte.DfltCBStoppUhrAusblenden)
        End Get
        Set
            _CBStoppUhrAusblenden = Value
        End Set
    End Property

    Public Property TBStoppUhrAusblendverzögerung As Integer
        Get
            Return GetProperty(_TBStoppUhrAusblendverzögerung, DefaultWerte.DfltTBStoppUhrAusblendverzögerung)
        End Get
        Set
            _TBStoppUhrAusblendverzögerung = Value
        End Set
    End Property
#End Region
#End Region

#Region "Einstellungen für die Kontaktsuche"
    Private _CBSucheUnterordner As Boolean
    Private _CBKontaktSucheFritzBox As Boolean
    Private _CBRWS As Boolean
    Private _CBKErstellen As Boolean
    Private _CBRWSIndex As Boolean
    Private _PCBUseLegacySearch As Boolean
    Private _PCBUseLegacyUserProp As Boolean
    <XmlElement("OutlookOrdner")> Public Property OutlookOrdner As OutlookOrdnerListe

    <XmlElement("CBUseLegacySearch")> Public Property CBUseLegacySearch As Boolean
        Get
            Return GetProperty(_PCBUseLegacySearch, DefaultWerte.DfltCBUseLegacySearch)
        End Get
        Set
            _PCBUseLegacySearch = Value
        End Set
    End Property

    <XmlElement("CBUseLegacyUserProp")> Public Property CBUseLegacyUserProp As Boolean
        Get
            Return GetProperty(_PCBUseLegacyUserProp, DefaultWerte.DfltCBUseLegacyUserProp)
        End Get
        Set
            _PCBUseLegacyUserProp = Value
        End Set
    End Property
#Region "Einstellungen für die Kontaktsuche - Kontaktsuche in Outlook (Indizierung)"

    <XmlElement("CBSucheUnterordner")> Public Property CBSucheUnterordner As Boolean
        Get
            Return GetProperty(_CBSucheUnterordner, DefaultWerte.DfltCBSucheUnterordner)
        End Get
        Set
            _CBSucheUnterordner = Value
        End Set
    End Property

    <XmlElement("CBKontaktSucheFritzBox")> Public Property CBKontaktSucheFritzBox As Boolean
        Get
            Return GetProperty(_CBKontaktSucheFritzBox, DefaultWerte.DfltCBKontaktSucheFritzBox)
        End Get
        Set
            _CBKontaktSucheFritzBox = Value
        End Set
    End Property
#End Region
#Region "Einstellungen für die Kontaktsuche - Rückwärtssuche (RWS)"
    <XmlElement("CBRWS")> Public Property CBRWS As Boolean
        Get
            Return GetProperty(_CBRWS, DefaultWerte.DfltCBRWS)
        End Get
        Set
            _CBRWS = Value
        End Set
    End Property

    <XmlElement("CBKErstellen")> Public Property CBKErstellen As Boolean
        Get
            Return GetProperty(_CBKErstellen, DefaultWerte.DfltCBKErstellen)
        End Get
        Set
            _CBKErstellen = Value
        End Set
    End Property

    <XmlElement("CBRWSIndex")> Public Property CBRWSIndex As Boolean
        Get
            Return GetProperty(_CBRWSIndex, DefaultWerte.DfltCBRWSIndex)
        End Get
        Set
            _CBRWSIndex = Value
        End Set
    End Property

#End Region

#End Region

#Region "Auswertung der Fritz!box Anrufliste"
#Region "Auswertung der Fritz!box Anrufliste - Outlook Journal"
    Private _CBJournal As Boolean

    <XmlElement("CBJournal")> Public Property CBJournal As Boolean
        Get
            Return GetProperty(_CBJournal, DefaultWerte.DfltCBJournal)
        End Get
        Set
            _CBJournal = Value
        End Set
    End Property
#End Region

#Region "Auswertung der Fritz!box Anrufliste - Auswertung der Anrufliste"
    Private _LetzterJournalEintrag As Date
    Private _LetzterJournalEintragID As Integer
    Private _CBAutoAnrList As Boolean
    Private _CBAnrListeUpdateCallLists As Boolean

    <XmlElement("LetzterJournalEintrag")> Public Property LetzterJournalEintrag As Date
        Get
            Return GetProperty(_LetzterJournalEintrag, DefaultWerte.DfltLetzterJournalEintrag)
        End Get
        Set
            _LetzterJournalEintrag = Value
        End Set
    End Property

    <XmlElement("LetzterJournalEintragID")> Public Property LetzterJournalEintragID As Integer
        Get
            Return GetProperty(_LetzterJournalEintragID, DefaultWerte.DfltLetzterJournalEintragID)
        End Get
        Set
            _LetzterJournalEintragID = Value
        End Set
    End Property

    <XmlElement("CBAutoAnrList")> Public Property CBAutoAnrList As Boolean
        Get
            Return GetProperty(_CBAutoAnrList, DefaultWerte.DfltCBAutoAnrList)
        End Get
        Set
            _CBAutoAnrList = Value
        End Set
    End Property

    <XmlElement("CBAnrListeUpdateCallLists")> Public Property CBAnrListeUpdateCallLists As Boolean
        Get
            Return GetProperty(_CBAnrListeUpdateCallLists, DefaultWerte.DfltCBAnrListeUpdateCallLists)
        End Get
        Set
            _CBAnrListeUpdateCallLists = Value
        End Set
    End Property

#End Region
#Region "Auswertung der Fritz!box Anrufliste - Anruflisten"
    Private _TBNumEntryList As Integer
    <XmlElement("TBNumEntryList")> Public Property TBNumEntryList As Integer
        Get
            Return GetProperty(_TBNumEntryList, DefaultWerte.DfltTBNumEntryList)
        End Get
        Set
            _TBNumEntryList = Value
        End Set
    End Property
#End Region
#End Region

#Region "Logging"
    Private _CBoxMinLogLevel As String
    <XmlElement("CBoxMinLogLevel")> Public Property CBoxMinLogLevel As String
        Get
            Return GetProperty(_CBoxMinLogLevel, DefaultWerte.DfltMinLogLevel.Name)
        End Get
        Set
            _CBoxMinLogLevel = Value
        End Set
    End Property
#End Region

#Region "SoftPhones"
#Region "Phoner"
    Private _TBPhonerPasswort As String
    Private _CBPhoner As Boolean

    <XmlElement("TBPhonerPasswort")> Public Property TBPhonerPasswort As String
        Get
            Return GetProperty(_TBPhonerPasswort, DefaultWerte.DfltTBPhonerPasswort)
        End Get
        Set
            _TBPhonerPasswort = Value
        End Set
    End Property

    <XmlElement("CBPhoner")> Public Property CBPhoner As Boolean
        Get
            Return GetProperty(_CBPhoner, DefaultWerte.DfltCBPhoner)
        End Get
        Set
            _CBPhoner = Value
        End Set
    End Property
#End Region

#Region "MicroSIP"
    Private _TBMicroSIPPath As String
    Private _CBMicroSIP As Boolean

    <XmlElement("TBMicroSIPPath")> Public Property TBMicroSIPPath As String
        Get
            Return _TBMicroSIPPath
        End Get
        Set
            _TBMicroSIPPath = Value
        End Set
    End Property

    <XmlElement("CBMicroSIP")> Public Property CBMicroSIP As Boolean
        Get
            Return GetProperty(_CBMicroSIP, DefaultWerte.DfltCBMicroSIP)
        End Get
        Set
            _CBMicroSIP = Value
        End Set
    End Property
#End Region
#End Region

    Public Sub New()
        OutlookOrdner = New OutlookOrdnerListe
    End Sub
End Class
