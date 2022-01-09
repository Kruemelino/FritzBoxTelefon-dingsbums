﻿Imports System.Threading.Tasks
Imports Microsoft.Office.Interop.Outlook
Public Interface IOptionenService

    Sub UpdateTheme()

#Region "Grunddaten"
    Function LadeFBoxUser(IPAdresse As String) As ObservableCollectionEx(Of FBoxAPI.User)
    Function GetOutlookStoreRootFolder() As IEnumerable(Of MAPIFolder)
#End Region

#Region "Design"
    Sub ShowDesignTest()
    Sub ToogleDesign()
#End Region


#Region "Import Telefoniedaten"
    Event Status As EventHandler(Of String)
    Event Beendet As EventHandler(Of NotifyEventArgs(Of Telefonie))
    Sub StartImport()
#End Region

#Region "Indizierung Ordner"
    ''' <summary>
    ''' Startet die Indizierung des Ordners
    ''' </summary>
    ''' <param name="Ordner">Outlook-Ordner, der indiziert werden soll</param>
    ''' <param name="IndexModus">Modus: true indizieren, false deindizieren</param>
    Function Indexer(Ordner As List(Of MAPIFolder), IndexModus As Boolean, ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer)
    Function ZähleOutlookKontakte(olFolders As List(Of MAPIFolder)) As Integer
#End Region

#Region "MicroSIP"
    Function GetMicroSIPExecutablePath() As String
#End Region

#Region "Test Rückwärtssuche"
    Event BeendetRWS As EventHandler(Of NotifyEventArgs(Of Boolean))
    Sub StartRWSTest(TelNr As String)
#End Region

#Region "Tellows"
    Function GetTellowsAccountData(XAuthToken As String) As Task(Of TellowsPartnerInfo)
    Function GetTellowsLiveAPIData(TelNr As String, XAuthToken As String) As Task(Of TellowsResponse)
#End Region

#Region "Test Login"
    Event BeendetLogin As EventHandler(Of NotifyEventArgs(Of Boolean))
    Sub StartLoginTest(FbAdr As String, User As String, Password As Security.SecureString)
#End Region

#Region "Test Kontaktsuche"
    Event BeendetKontaktsuche As EventHandler(Of NotifyEventArgs(Of Boolean))
    Sub StartKontaktsucheTest(TelNr As String)
#End Region

#Region "Test Anrufmonitor"
    Sub StartAnrMonTest(TelNr As String, CONNECT As Boolean, rnd As Boolean, rndOutlook As Boolean, rndFBox As Boolean, rndTellows As Boolean, clir As Boolean)
#End Region
End Interface
