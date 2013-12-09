Imports Microsoft.Office.Core
Imports Microsoft.Win32

Public Class ThisAddIn
#Region "Office 2003 & 2007 Eventhandler"
#If OVer < 14 Then
    Private WithEvents eBtnWaehlen As Office.CommandBarButton
    Private WithEvents eBtnDirektwahl As Office.CommandBarButton
    Private WithEvents eBtnAnrMonitor As Office.CommandBarButton
    Private WithEvents eBtnAnzeigen As Office.CommandBarButton
    Private WithEvents eBtnJournalimport As Office.CommandBarButton
    Private WithEvents eBtnLeitungsbelegung As Office.CommandBarButton
    Private WithEvents eBtnEinstellungen As Office.CommandBarButton
    Private WithEvents eBtnAnrMonNeuStart As Office.CommandBarButton
    Private WithEvents ePopWwdh As Office.CommandBarPopup
    Private WithEvents ePopWwdh1, ePopWwdh2, ePopWwdh3, ePopWwdh4, ePopWwdh5 As Office.CommandBarButton
    Private WithEvents ePopWwdh6, ePopWwdh7, ePopWwdh8, ePopWwdh9, ePopWwdh10 As Office.CommandBarButton
    Private WithEvents ePopAnr As Office.CommandBarPopup
    Private WithEvents ePopAnr1, ePopAnr2, ePopAnr3, ePopAnr4, ePopAnr5 As Office.CommandBarButton
    Private WithEvents ePopAnr6, ePopAnr7, ePopAnr8, ePopAnr9, ePopAnr10 As Office.CommandBarButton
    Private WithEvents ePopVIP As Office.CommandBarPopup
    Private WithEvents ePopVIP1, ePopVIP2, ePopVIP3, ePopVIP4, ePopVIP5 As Office.CommandBarButton
    Private WithEvents ePopVIP6, ePopVIP7, ePopVIP8, ePopVIP9, ePopVIP10 As Office.CommandBarButton
#End If
#If OVer = 11 Then
    Private WithEvents iPopRWS As Office.CommandBarPopup
    Private WithEvents iBtnWwh As Office.CommandBarButton
    Private WithEvents iBtnRws11880 As Office.CommandBarButton
    Private WithEvents iBtnRWSDasTelefonbuch As Office.CommandBarButton
    Private WithEvents iBtnRWStelSearch As Office.CommandBarButton
    Private WithEvents iBtnRWSAlle As Office.CommandBarButton
    Private WithEvents iBtnKontakterstellen As Office.CommandBarButton
    Private WithEvents iBtnVIP As Office.CommandBarButton
#End If
#End Region
    Private Shared oApp As Outlook.Application
    Private WithEvents ContactSaved As Outlook.ContactItem
    Private WithEvents oInsps As Outlook.Inspectors
    Private Shared XML As DataProvider ' Reader/Writer initialisieren
    Private Shared fBox As FritzBox  'Deklarieren der Klasse
    Private Shared AnrMon As AnrufMonitor
    Private Shared WClient As Wählclient
    Private Shared hf As Helfer
    Private Shared KontaktFunktionen As Contacts
    Private Shared GUI As GraphicalUserInterface
    Private Shared Cfg As formCfg
    Private Shared Dateipfad As String

#Region "Properties"
    Friend Shared Property P_oApp() As Outlook.Application
        Get
            Return oApp
        End Get
        Set(ByVal value As Outlook.Application)
            oApp = value
        End Set
    End Property

    Friend Shared Property P_XML() As DataProvider
        Get
            Return XML
        End Get
        Set(ByVal value As DataProvider)
            XML = value
        End Set
    End Property

    Friend Shared Property P_hf() As Helfer
        Get
            Return hf
        End Get
        Set(ByVal value As Helfer)
            hf = value
        End Set
    End Property

    Friend Shared Property P_KontaktFunktionen() As Contacts
        Get
            Return KontaktFunktionen
        End Get
        Set(ByVal value As Contacts)
            KontaktFunktionen = value
        End Set
    End Property

    Friend Shared Property P_GUI() As GraphicalUserInterface
        Get
            Return GUI
        End Get
        Set(ByVal value As GraphicalUserInterface)
            GUI = value
        End Set
    End Property

    Friend Shared Property P_WClient() As Wählclient
        Get
            Return WClient
        End Get
        Set(ByVal value As Wählclient)
            WClient = value
        End Set
    End Property

    Friend Shared Property P_FritzBox() As FritzBox
        Get
            Return fBox
        End Get
        Set(ByVal value As FritzBox)
            fBox = value
        End Set
    End Property

    Friend Shared Property P_AnrMon() As AnrufMonitor
        Get
            Return AnrMon
        End Get
        Set(ByVal value As AnrufMonitor)
            AnrMon = value
        End Set
    End Property

    Friend Shared Property P_Dateipfad() As String
        Get
            Return Dateipfad
        End Get
        Set(ByVal value As String)
            Dateipfad = value
        End Set
    End Property

    Friend Shared Property P_Config() As formCfg
        Get
            Return Cfg
        End Get
        Set(ByVal value As formCfg)
            Cfg = value
        End Set
    End Property
#End Region

#If OVer < 14 Then
    Private FritzCmdBar As Office.CommandBar
#End If

    Private Initialisierung As formInit
    Public Const Version As String = "3.6.13"
    Public Shared Event PowerModeChanged As PowerModeChangedEventHandler

#If Not OVer = 11 Then
    Protected Overrides Function CreateRibbonExtensibilityObject() As IRibbonExtensibility
        Initialisierung = New formInit
        Return GUI
    End Function
#End If

    Sub AnrMonRestartNachStandBy(ByVal sender As Object, ByVal e As PowerModeChangedEventArgs)
        Select Case e.Mode
            Case PowerModes.Resume
                hf.LogFile("StandBy: PowerModes." & PowerModes.Resume.ToString)
                AnrMon.AnrMonStartNachStandby()
            Case PowerModes.Suspend
                AnrMon.AnrMonQuit()
                hf.LogFile("StandBy: PowerModes." & PowerModes.Suspend.ToString)
            Case Else
                hf.LogFile("PowerMode: " & e.Mode)
        End Select
    End Sub

    Private Sub ThisAddIn_Startup(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Startup

        AddHandler SystemEvents.PowerModeChanged, AddressOf AnrMonRestartNachStandBy
        Dim i As Integer = 2

        oApp = CType(Application, Outlook.Application)

        If Not oApp.ActiveExplorer Is Nothing Then
#If OVer = 11 Then
            Initialisierung = New formInit
#End If

#If OVer < 14 Then
            GUI.SymbolleisteErzeugen(ePopWwdh, ePopAnr, ePopVIP, eBtnWaehlen, eBtnDirektwahl, eBtnAnrMonitor, eBtnAnzeigen, eBtnAnrMonNeuStart, eBtnJournalimport, eBtnEinstellungen, _
                                     ePopWwdh1, ePopWwdh2, ePopWwdh3, ePopWwdh4, ePopWwdh5, ePopWwdh6, ePopWwdh7, ePopWwdh8, ePopWwdh9, ePopWwdh10, _
                                     ePopAnr1, ePopAnr2, ePopAnr3, ePopAnr4, ePopAnr5, ePopAnr6, ePopAnr7, ePopAnr8, ePopAnr9, ePopAnr10, _
                                     ePopVIP1, ePopVIP2, ePopVIP3, ePopVIP4, ePopVIP5, ePopVIP6, ePopVIP7, ePopVIP8, ePopVIP9, ePopVIP10)
#End If
            If Not XML.P_CBIndexAus Then oInsps = Application.Inspectors
        Else
            hf.LogFile("Addin nicht gestartet, da kein Explorer vorhanden war")
        End If
    End Sub

    Private Sub ContactSaved_Write(ByRef Cancel As Boolean) Handles ContactSaved.Write
        If Not XML.P_CBIndexAus Then KontaktFunktionen.IndiziereKontakt(ContactSaved, True)
    End Sub

    Private Sub ThisAddIn_Shutdown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shutdown
        AnrMon.AnrMonQuit()
        XML.SpeichereXMLDatei()
        With hf
            .NAR(oApp)
#If OVer < 14 Then
            .NAR(FritzCmdBar)
#End If
        End With
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub myOlInspectors(ByVal Inspector As Outlook.Inspector) Handles oInsps.NewInspector
#If OVer = 11 Then
        GUI.InspectorSybolleisteErzeugen(Inspector, iPopRWS, iBtnWwh, iBtnRws11880, iBtnRWSDasTelefonbuch, iBtnRWStelSearch, iBtnRWSAlle, iBtnKontakterstellen, iBtnVIP)
#End If
        If TypeOf Inspector.CurrentItem Is Outlook.ContactItem Then
            If XML.P_CBKHO Then
                Dim Ordner As Outlook.MAPIFolder
                Dim StandardOrdner As Outlook.MAPIFolder
                Dim olNamespace As Outlook.NameSpace
                Ordner = CType(CType(Inspector.CurrentItem, Outlook.ContactItem).Parent, Outlook.MAPIFolder)
                olNamespace = oApp.GetNamespace("MAPI")
                StandardOrdner = olNamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts)
                If Not StandardOrdner.StoreID = Ordner.StoreID Then Exit Sub
            End If
            ContactSaved = CType(Inspector.CurrentItem, Outlook.ContactItem)
        End If
    End Sub

#Region " Office 2003 & 2007"
#If OVer < 14 Then
#Region " Button"
    Private Sub eBtn_Click(ByVal Ctrl As Microsoft.Office.Core.CommandBarButton, ByRef CancelDefault As Boolean) Handles eBtnDirektwahl.Click, _
                                                                                                                         eBtnWaehlen.Click, _
                                                                                                                         eBtnEinstellungen.Click, _
                                                                                                                         eBtnAnrMonitor.Click, _
                                                                                                                         eBtnAnzeigen.Click, _
                                                                                                                         eBtnJournalimport.Click, _
                                                                                                                         eBtnAnrMonNeuStart.Click

        With (GUI)
            Select Case CType(Ctrl, CommandBarButton).Caption
                Case "Direktwahl"
                    .WähleDirektwahl()
                Case "Wählen"
                    .WählenExplorer()
                Case "Einstellungen"
                    .ÖffneEinstellungen()
                Case "Anrufmonitor"
                    AnrMon.AnrMonAnAus()
                Case "Anzeigen"
                    .ÖffneAnrMonAnzeigen()
                Case "Journalimport"
                    .ÖffneJournalImport()
                Case "Anrufmonitor neustarten"
                    .AnrMonNeustarten()
            End Select
        End With
    End Sub

    Private Sub ePopAnr1_click(ByVal control As Office.CommandBarButton, ByRef cancel As Boolean) Handles ePopAnr1.Click, _
                                                                                                          ePopAnr2.Click, _
                                                                                                          ePopAnr3.Click, _
                                                                                                          ePopAnr4.Click, _
                                                                                                          ePopAnr5.Click, _
                                                                                                          ePopAnr6.Click, _
                                                                                                          ePopAnr7.Click, _
                                                                                                          ePopAnr8.Click, _
                                                                                                          ePopAnr9.Click, _
                                                                                                          ePopAnr10.Click, _
                                                                                                          ePopWwdh1.Click, _
                                                                                                          ePopWwdh2.Click, _
                                                                                                          ePopWwdh3.Click, _
                                                                                                          ePopWwdh4.Click, _
                                                                                                          ePopWwdh5.Click, _
                                                                                                          ePopWwdh6.Click, _
                                                                                                          ePopWwdh7.Click, _
                                                                                                          ePopWwdh8.Click, _
                                                                                                          ePopWwdh9.Click, _
                                                                                                          ePopWwdh10.Click, _
                                                                                                          ePopVIP1.Click, _
                                                                                                          ePopVIP2.Click, _
                                                                                                          ePopVIP3.Click, _
                                                                                                          ePopVIP4.Click, _
                                                                                                          ePopVIP5.Click, _
                                                                                                          ePopVIP6.Click, _
                                                                                                          ePopVIP7.Click, _
                                                                                                          ePopVIP8.Click, _
                                                                                                          ePopVIP9.Click, _
                                                                                                          ePopVIP10.Click
        GUI.KlickListen(control.Tag)
    End Sub
#End Region
#End If
#End Region

#Region " Office 2003 Inspectorfenster"
#If OVer = 11 Then
    Private Sub iBtn_Click(ByVal Ctrl As Microsoft.Office.Core.CommandBarButton, ByRef CancelDefault As Boolean) Handles iBtnKontakterstellen.Click, _
                                                                                                                         iBtnRws11880.Click, _
                                                                                                                         iBtnRWSDasTelefonbuch.Click, _
                                                                                                                         iBtnRWStelSearch.Click, _
                                                                                                                         iBtnRWSAlle.Click, _
                                                                                                                         iBtnWwh.Click, _
                                                                                                                         iBtnVIP.Click

        With (GUI)
            Select Case CType(Ctrl, CommandBarButton).Caption
                Case "Kontakt erstellen"
                    .KontaktErstellen()
                Case "11880"
                    .RWS11880(oApp.ActiveInspector)
                Case "DasTelefonbuch"
                    .RWSDasTelefonbuch(oApp.ActiveInspector)
                Case "tel.search.ch"
                    .RWSTelSearch(oApp.ActiveInspector)
                Case "Alle"
                    .RWSAlle(oApp.ActiveInspector)
                Case "Wählen"
                    WClient.WählenAusInspector()
                Case "VIP"
                    Dim aktKontakt As Outlook.ContactItem = CType(oApp.ActiveInspector.CurrentItem, Outlook.ContactItem)
                    If .IsVIP(aktKontakt) Then
                        .RemoveVIP(aktKontakt.EntryID, CType(aktKontakt.Parent, Outlook.MAPIFolder).StoreID)
                        Ctrl.State = MsoButtonState.msoButtonUp
                    Else
                        .AddVIP(aktKontakt)
                        Ctrl.State = MsoButtonState.msoButtonDown
                    End If
            End Select
        End With
    End Sub
#End If
#End Region

End Class
