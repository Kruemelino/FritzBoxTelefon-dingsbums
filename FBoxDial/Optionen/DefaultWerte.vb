Friend NotInheritable Class DefaultWerte
#Region "Default Value Properties"
    Public Shared ReadOnly Property DfltOptions As String = "Optionen"
    Public Shared ReadOnly Property DfltMinLogLevel As LogLevel = LogLevel.Debug
    Public Shared ReadOnly Property DfltDeCryptKey As String = "ZugangV5"
    Public Shared ReadOnly Property DfltTBAmt() As String = DfltStringEmpty
    Public Shared ReadOnly Property DfltTBOrtsKZ() As String = DfltStringEmpty
    Public Shared ReadOnly Property DfltTBNumEntryList() As Integer = 10
    Public Shared ReadOnly Property DfltTBEnblDauer() As Integer = 10
    Public Shared ReadOnly Property DfltCBAnrMonAuto() As Boolean = False
    Public Shared ReadOnly Property DfltCBAnrMonZeigeKontakt() As Boolean = False
    Public Shared ReadOnly Property DfltCBAnrMonContactImage() As Boolean = True
    Public Shared ReadOnly Property DfltCBAutoClose() As Boolean = True
    Public Shared ReadOnly Property DfltCBForceDialLKZ() As Boolean = False
    Public Shared ReadOnly Property DfltCBKErstellen() As Boolean = False
    Public Shared ReadOnly Property DfltCBAnrListeUpdateCallLists() As Boolean = False
    Public Shared ReadOnly Property DfltCBRWS() As Boolean = False
    Public Shared ReadOnly Property DfltCBRWSIndex() As Boolean = True
    Public Shared ReadOnly Property DfltCBJournal() As Boolean = True
    Public Shared ReadOnly Property DfltCBCheckMobil() As Boolean = True
    Public Shared ReadOnly Property DfltTBTelNrMaske() As String = "%L (%O) %N - %D"
    Public Shared ReadOnly Property DfltCBTelNrGruppieren() As Boolean = True
    Public Shared ReadOnly Property DfltCBintl() As Boolean = False
    Public Shared ReadOnly Property DfltCLIR() As Boolean = False
    Public Shared ReadOnly Property DfltLetzterJournalEintragID As Integer = 0
    Public Shared ReadOnly Property DfltLetzterJournalEintrag As Date = Now
    Public Shared ReadOnly Property DfltCBAutoAnrList As Boolean = False
    Public Shared ReadOnly Property DfltCBCloseWClient As Boolean = True
    Public Shared ReadOnly Property DfltTBWClientEnblDauer() As Integer = 10
    Public Shared ReadOnly Property DfltCBUseLegacySearch As Boolean = False
    Public Shared ReadOnly Property DfltCBUseLegacyUserProp As Boolean = False
    Public Shared ReadOnly Property DfltCBSucheUnterordner As Boolean = False
    Public Shared ReadOnly Property DfltCBKontaktSucheFritzBox As Boolean = False
    Public Shared ReadOnly Property DfltTBPhonerPasswort() As String = DfltStringEmpty
    Public Shared ReadOnly Property DfltCBPhoner As Boolean = False
    Public Shared ReadOnly Property DfltTBPfadMicroSIP() As String = DfltStringEmpty
    Public Shared ReadOnly Property DfltCBMicroSIP As Boolean = False
#End Region

End Class
