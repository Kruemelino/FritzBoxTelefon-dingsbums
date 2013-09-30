Imports Microsoft.Office.Core
Imports Microsoft.Win32

Public Class ThisAddIn
#Region "Office 2003 & 2007 Eventhandler"
#If OVer < 14 Then
    Public WithEvents eBtnWaehlen As Office.CommandBarButton
    Public WithEvents eBtnDirektwahl As Office.CommandBarButton
    Public WithEvents eBtnAnrMonitor As Office.CommandBarButton
    Public WithEvents eBtnAnzeigen As Office.CommandBarButton
    Public WithEvents eBtnJournalimport As Office.CommandBarButton
    Public WithEvents eBtnLeitungsbelegung As Office.CommandBarButton
    Public WithEvents eBtnEinstellungen As Office.CommandBarButton
    Public WithEvents eBtnAnrMonNeuStart As Office.CommandBarButton
    Public WithEvents ePopWwdh As Office.CommandBarPopup
    Public WithEvents ePopWwdh1, ePopWwdh2, ePopWwdh3, ePopWwdh4, ePopWwdh5 As Office.CommandBarButton
    Public WithEvents ePopWwdh6, ePopWwdh7, ePopWwdh8, ePopWwdh9, ePopWwdh10 As Office.CommandBarButton
    Public WithEvents ePopAnr As Office.CommandBarPopup
    Public WithEvents ePopAnr1, ePopAnr2, ePopAnr3, ePopAnr4, ePopAnr5 As Office.CommandBarButton
    Public WithEvents ePopAnr6, ePopAnr7, ePopAnr8, ePopAnr9, ePopAnr10 As Office.CommandBarButton
    Public Shared WithEvents ePopVIP As Office.CommandBarPopup
    Public WithEvents ePopVIP1, ePopVIP2, ePopVIP3, ePopVIP4, ePopVIP5 As Office.CommandBarButton
    Public WithEvents ePopVIP6, ePopVIP7, ePopVIP8, ePopVIP9, ePopVIP10 As Office.CommandBarButton
#End If
#If OVer = 11 Then
    Public WithEvents iPopRWS As Office.CommandBarPopup
    Public WithEvents iBtnWwh As Office.CommandBarButton
    'Public WithEvents iBtnRwsGoYellow As Office.CommandBarButton
    Public WithEvents iBtnRws11880 As Office.CommandBarButton
    Public WithEvents iBtnRWSDasTelefonbuch As Office.CommandBarButton
    Public WithEvents iBtnRWStelSearch As Office.CommandBarButton
    Public WithEvents iBtnRWSAlle As Office.CommandBarButton
    Public WithEvents iBtnKontakterstellen As Office.CommandBarButton
    Public WithEvents iBtnVIP As Office.CommandBarButton
#End If
#End Region
    Public Shared oApp As Outlook.Application

    Public WithEvents ContactSaved As Outlook.ContactItem
    Public WithEvents oInsps As Outlook.Inspectors
    Public Shared XML As MyXML ' Reader/Writer initialisieren
    Public Shared fBox As FritzBox  'Deklarieren der Klasse
    Public Shared AnrMon As AnrufMonitor
    Public Shared RWSSuche As formRWSuche
    Public Shared Journalimport As formJournalimport
    Public Shared WClient As Wählclient
    Public Shared Crypt As New Rijndael
    Public Shared hf As Helfer
    Public Shared KontaktFunktionen As Contacts
    Public Shared Phoner As PhonerInterface
    Public Shared GUI As GraphicalUserInterface
    Public Shared OlI As OutlookInterface

    Public Shared Dateipfad As String
#If OVer < 14 Then
    Private FritzCmdBar As Office.CommandBar
#End If

    Private FbAddr As String

    Private Initialisierung As formInit

    Public Const Version As String = "3.5.3"

    Public Shared UseAnrMon As Boolean
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
                hf.LogFile("Aufwachen aus StandBy: " & e.Mode)
                AnrMon.AnrMonStartNachStandby()
            Case PowerModes.Suspend
                AnrMon.AnrMonQuit()
                hf.LogFile("Anrufmonitor für StandBy beendet")
            Case Else
                hf.LogFile("Empfangener Powermode: " & e.Mode)
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
            If Not CBool(XML.Read("Optionen", "CBIndexAus", "False")) Then oInsps = Application.Inspectors
        Else
            hf.LogFile("Addin nicht gestartet, da kein Explorer vorhanden war")
        End If
    End Sub

    Private Sub ContactSaved_Write(ByRef Cancel As Boolean) Handles ContactSaved.Write
        If Not CBool(XML.Read("Optionen", "CBIndexAus", "False")) Then
            KontaktFunktionen.IndiziereKontakt(ContactSaved, True)
        End If
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
            If XML.Read("Optionen", "CBKHO", "True") = "True" Then
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
    Private Sub eBtnDirektwahl_click(ByVal control As Office.CommandBarButton, ByRef cancel As Boolean) Handles eBtnDirektwahl.Click
        GUI.ÖffneDirektwahl()
    End Sub

    Private Sub ebtnWaehlen_click(ByVal control As Office.CommandBarButton, ByRef cancel As Boolean) Handles eBtnWaehlen.Click
        GUI.WählenExplorer()
    End Sub

    Private Sub ePopAnr1_click(ByVal control As Office.CommandBarButton, ByRef cancel As Boolean) Handles _
    ePopAnr1.Click, ePopAnr2.Click, ePopAnr3.Click, ePopAnr4.Click, ePopAnr5.Click, ePopAnr6.Click, ePopAnr7.Click, ePopAnr8.Click, ePopAnr9.Click, ePopAnr10.Click, _
    ePopWwdh1.Click, ePopWwdh2.Click, ePopWwdh3.Click, ePopWwdh4.Click, ePopWwdh5.Click, ePopWwdh6.Click, ePopWwdh7.Click, ePopWwdh8.Click, ePopWwdh9.Click, ePopWwdh10.Click, _
    ePopVIP1.Click, ePopVIP2.Click, ePopVIP3.Click, ePopVIP4.Click, ePopVIP5.Click, ePopVIP6.Click, ePopVIP7.Click, ePopVIP8.Click, ePopVIP9.Click, ePopVIP10.Click
        GUI.KlickListen(control.Tag)
    End Sub

    Private Sub eBtnEinstellungen_click(ByVal control As Office.CommandBarButton, ByRef cancel As Boolean) Handles eBtnEinstellungen.Click
        GUI.ÖffneEinstellungen()
    End Sub

    Private Sub eBtnAnrMonitor_Click(ByVal Ctrl As Office.CommandBarButton, ByRef CancelDefault As Boolean) Handles eBtnAnrMonitor.Click
        AnrMon.AnrMonAnAus()
    End Sub

    Private Sub eBtnAnzeigen_Click(ByVal Ctrl As Office.CommandBarButton, ByRef CancelDefault As Boolean) Handles eBtnAnzeigen.Click
        GUI.ÖffneAnrMonAnzeigen()
    End Sub

    Private Sub eBtnJournalimport_Click(ByVal Ctrl As Microsoft.Office.Core.CommandBarButton, ByRef CancelDefault As Boolean) Handles eBtnJournalimport.Click
        GUI.ÖffneJournalImport()
    End Sub

    Private Sub eBtnAnrMonNeuStart_Click(ByVal Ctrl As Microsoft.Office.Core.CommandBarButton, ByRef CancelDefault As Boolean) Handles eBtnAnrMonNeuStart.Click
        GUI.AnrMonNeustarten()
    End Sub
#End Region

#End If
#End Region
#Region " Office 2003 Inspectorfenster"
#If OVer = 11 Then
    Private Sub iBtnKontakterstellen_Click(ByVal Ctrl As Microsoft.Office.Core.CommandBarButton, ByRef CancelDefault As Boolean) Handles iBtnKontakterstellen.Click
        GUI.KontaktErstellen()
    End Sub

    'Private Sub iBtnRwsGoYellow_Click1(ByVal Ctrl As Microsoft.Office.Core.CommandBarButton, ByRef CancelDefault As Boolean) Handles iBtnRwsGoYellow.Click
    '    GUI.RWSGoYellow(oApp.ActiveInspector)
    'End Sub

    Private Sub iBtnRws11880_Click1(ByVal Ctrl As Microsoft.Office.Core.CommandBarButton, ByRef CancelDefault As Boolean) Handles iBtnRws11880.Click
        GUI.RWS11880(oApp.ActiveInspector)
    End Sub

    Private Sub iBtnRWSDasTelefonbuch_Click1(ByVal Ctrl As Microsoft.Office.Core.CommandBarButton, ByRef CancelDefault As Boolean) Handles iBtnRWSDasTelefonbuch.Click
        GUI.RWSDasTelefonbuch(oApp.ActiveInspector)
    End Sub

    Private Sub iBtnRWStelSearch_Click1(ByVal Ctrl As Microsoft.Office.Core.CommandBarButton, ByRef CancelDefault As Boolean) Handles iBtnRWStelSearch.Click
        GUI.RWSTelSearch(oApp.ActiveInspector)
    End Sub

    Private Sub iBtnRWSAlle_Click1(ByVal Ctrl As Microsoft.Office.Core.CommandBarButton, ByRef CancelDefault As Boolean) Handles iBtnRWSAlle.Click
        GUI.RWSAlle(oApp.ActiveInspector)
    End Sub

    Private Sub iBtnWwh_Click(ByVal Ctrl As Microsoft.Office.Core.CommandBarButton, ByRef CancelDefault As Boolean) Handles iBtnWwh.Click
        WClient.WählenAusInspector()
    End Sub

    Private Sub iBtnVIP_Click(ByVal Ctrl As Microsoft.Office.Core.CommandBarButton, ByRef CancelDefault As Boolean) Handles iBtnVIP.Click
        Dim aktKontakt As Outlook.ContactItem = CType(oApp.ActiveInspector.CurrentItem, Outlook.ContactItem)
        If GUI.IsVIP(aktKontakt) Then
            GUI.RemoveVIP(aktKontakt.EntryID, CType(aktKontakt.Parent, Outlook.MAPIFolder).StoreID)
            Ctrl.State = MsoButtonState.msoButtonUp
        Else
            GUI.AddVIP(aktKontakt)
            Ctrl.State = MsoButtonState.msoButtonDown
        End If
    End Sub
#End If
#End Region

End Class
