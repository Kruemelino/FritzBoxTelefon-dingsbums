Friend NotInheritable Class DefaultWerte
#Region "Default Value Properties"
    Public Shared ReadOnly Property PDfltOptions As String = "Optionen"
    Public Shared ReadOnly Property PDfltDeCryptKey As String = "ZugangN"
    Public Shared ReadOnly Property PDfltCBoxLandesVorwahl() As Integer = PDfltIntErrorMinusOne
    Public Shared ReadOnly Property PDfltTBAmt() As String = PDfltStringEmpty
    Public Shared ReadOnly Property PDfltTBOrtsKZ() As String = PDfltStringEmpty
    Public Shared ReadOnly Property PDfltTBLandesKZ() As String = DfltWerteTelefonie.PDfltLandesKZ
    Public Shared ReadOnly Property PDfltTBNumEntryList() As Integer = 10
    Public Shared ReadOnly Property PDfltCBoxVorwahl() As Integer = 0
    Public Shared ReadOnly Property PDfltTBEnblDauer() As Integer = 10
    Public Shared ReadOnly Property PDfltCBAnrMonAuto() As Boolean = False
    Public Shared ReadOnly Property PDfltTBAnrBeantworterTimeout() As Integer = 30
    Public Shared ReadOnly Property PDfltCBAnrMonZeigeKontakt() As Boolean = False
    Public Shared ReadOnly Property PDfltCBAnrMonContactImage() As Boolean = True
    Public Shared ReadOnly Property PDfltCBIndexAus() As Boolean = False
    Public Shared ReadOnly Property PDfltCBShowMSN() As Boolean = False
    Public Shared ReadOnly Property PDfltCBAnrMonCloseAtDISSCONNECT() As Boolean = False
    Public Shared ReadOnly Property PDfltCBAutoClose() As Boolean = True
    Public Shared ReadOnly Property PDfltCBVoIPBuster() As Boolean = False
    Public Shared ReadOnly Property PDfltCBCbCunterbinden() As Boolean = False
    Public Shared ReadOnly Property PDfltCBCallByCall() As Boolean = False
    Public Shared ReadOnly Property PDfltCBDialPort() As Boolean = False
    Public Shared ReadOnly Property PDfltCBKErstellen() As Boolean = False
    Public Shared ReadOnly Property PDfltCBoxMinLogLevel() As String = "Info"
    Public Shared ReadOnly Property PDfltCBJImport() As Boolean = False
    Public Shared ReadOnly Property PDfltCBAnrListeUpdateCallLists() As Boolean = False
    Public Shared ReadOnly Property PDfltCBAnrListeShowAnrMon() As Boolean = False
    Public Shared ReadOnly Property PDfltCBRWS() As Boolean = False
    Public Shared ReadOnly Property PDfltTVKontaktOrdnerEntryID() As String = PDfltStrErrorMinusOne
    Public Shared ReadOnly Property PDfltTVKontaktOrdnerStoreID() As String = PDfltStrErrorMinusOne
    Public Shared ReadOnly Property PDfltCBRWSIndex() As Boolean = True
    Public Shared ReadOnly Property PDfltComboBoxRWS() As Integer = 0
    Public Shared ReadOnly Property PDfltCBIndex() As Boolean = True
    Public Shared ReadOnly Property PDfltCBJournal() As Boolean = True
    Public Shared ReadOnly Property PDfltCBUseAnrMon() As Boolean = True
    Public Shared ReadOnly Property PDfltCBCheckMobil() As Boolean = True
    Public Shared ReadOnly Property PDfltTBTelNrMaske() As String = "%L (%O) %N - %D"
    Public Shared ReadOnly Property PDfltCBTelNrGruppieren() As Boolean = True
    Public Shared ReadOnly Property PDfltCBintl() As Boolean = False
    Public Shared ReadOnly Property PDfltCBIgnoTelNrFormat() As Boolean = False
    Public Shared ReadOnly Property PDfltTelAnschluss() As String = PDfltStringEmpty
    Public Shared ReadOnly Property PDfltTelFestnetz() As Boolean = False
    Public Shared ReadOnly Property PDfltTelCLIR() As Boolean = False
    Public Shared ReadOnly Property PDfltCBForceFBAdr() As Boolean = False
    Public Shared ReadOnly Property PDfltTBBenutzer() As String = PDfltStringEmpty
    Public Shared ReadOnly Property PDfltTBPasswort() As String = PDfltStringEmpty
    Public Shared ReadOnly Property PDfltTBFBAdr() As String = FritzBoxDefault.PDfltFritzBoxIPAdress
    Public Shared ReadOnly Property PDfltLetzterJournalEintragID As Integer = 0
    Public Shared ReadOnly Property PDfltLetzterJournalEintrag As Date = Now
    Public Shared ReadOnly Property PDfltCBCloseWClient As Boolean = True
    Public Shared ReadOnly Property PDfltTBWClientEnblDauer() As Integer = 10
    Public Shared Property PDfltCBUseLegacySearch As Boolean = False
    Public Shared Property PDfltCBUseLegacyUserProp As Boolean = False

#End Region

End Class
