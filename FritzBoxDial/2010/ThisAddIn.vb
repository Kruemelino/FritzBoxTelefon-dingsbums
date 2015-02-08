Imports Microsoft.Office.Core

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
    Private WithEvents iBtnRWSDasOertliche As Office.CommandBarButton
    Private WithEvents iBtnRws11880 As Office.CommandBarButton
    Private WithEvents iBtnRWSDasTelefonbuch As Office.CommandBarButton
    Private WithEvents iBtnRWStelSearch As Office.CommandBarButton
    Private WithEvents iBtnRWSAlle As Office.CommandBarButton
    Private WithEvents iBtnKontakterstellen As Office.CommandBarButton
    Private WithEvents iBtnVIP As Office.CommandBarButton
    Private WithEvents iBtnUpload As Office.CommandBarButton
#End If
#End Region
    Private Shared oApp As Outlook.Application

    Private WithEvents oInsps As Outlook.Inspectors
    Friend Shared ListofOpenContacts As New Generic.List(Of ContactSaved)

    Private Shared C_DP As DataProvider ' Reader/Writer initialisieren
    Private Shared C_Fbox As FritzBox  'Deklarieren der Klasse
    Private Shared C_AnrMon As AnrufMonitor
    Private Shared C_WClient As Wählclient
    Private Shared C_HF As Helfer
    Private Shared C_KF As Contacts
    Private Shared C_GUI As GraphicalUserInterface
    Private Shared F_Cfg As formCfg

    Private Initialisierung As formInit
    Public Shared Event PowerModeChanged As Microsoft.Win32.PowerModeChangedEventHandler

#Region "Properties"
    ''' <summary>
    ''' Gibt die Versionsnummer des Addins zurück.
    ''' </summary>
    ''' <value>System.Reflection.Assembly.GetExecutingAssembly.GetName.Version</value>
    ''' <returns>.Major.Minor.Build</returns>
    ''' <remarks></remarks>
    Friend Shared ReadOnly Property Version() As String
        Get
            With System.Reflection.Assembly.GetExecutingAssembly.GetName.Version
                Return .Major & "." & .Minor & "." & .Build
            End With
        End Get
    End Property

    ''' <summary>
    ''' Gibt die aktuelle Outlook-Application zurück.
    ''' </summary>
    Friend Shared Property P_oApp() As Outlook.Application
        Get
            Return oApp
        End Get
        Set(ByVal value As Outlook.Application)
            oApp = value
        End Set
    End Property

    Friend Shared Property P_DP() As DataProvider
        Get
            Return C_DP
        End Get
        Set(ByVal value As DataProvider)
            C_DP = value
        End Set
    End Property

    Friend Shared Property P_HF() As Helfer
        Get
            Return C_HF
        End Get
        Set(ByVal value As Helfer)
            C_HF = value
        End Set
    End Property

    Friend Shared Property P_KF() As Contacts
        Get
            Return C_KF
        End Get
        Set(ByVal value As Contacts)
            C_KF = value
        End Set
    End Property

    Friend Shared Property P_GUI() As GraphicalUserInterface
        Get
            Return C_GUI
        End Get
        Set(ByVal value As GraphicalUserInterface)
            C_GUI = value
        End Set
    End Property

    Friend Shared Property P_WClient() As Wählclient
        Get
            Return C_WClient
        End Get
        Set(ByVal value As Wählclient)
            C_WClient = value
        End Set
    End Property

    Friend Shared Property P_FritzBox() As FritzBox
        Get
            Return C_Fbox
        End Get
        Set(ByVal value As FritzBox)
            C_Fbox = value
        End Set
    End Property

    Friend Shared Property P_AnrMon() As AnrufMonitor
        Get
            Return C_AnrMon
        End Get
        Set(ByVal value As AnrufMonitor)
            C_AnrMon = value
        End Set
    End Property

    Friend Shared Property P_Config() As formCfg
        Get
            Return F_Cfg
        End Get
        Set(ByVal value As formCfg)
            F_Cfg = value
        End Set
    End Property

#End Region

#If Not OVer = 11 Then
    Protected Overrides Function CreateRibbonExtensibilityObject() As IRibbonExtensibility
        Initialisierung = New formInit
        Return C_GUI
    End Function
#End If

    Sub AnrMonRestartNachStandBy(ByVal sender As Object, ByVal e As Microsoft.Win32.PowerModeChangedEventArgs)
        C_HF.LogFile("PowerMode: " & e.Mode.ToString & " (" & e.Mode & ")")
        Select Case e.Mode
            Case Microsoft.Win32.PowerModes.Resume
                C_AnrMon.AnrMonStartNachStandby()
            Case Microsoft.Win32.PowerModes.Suspend
                C_AnrMon.AnrMonStartStopp()
        End Select
    End Sub

    ''' <summary>
    ''' Startet das Fritz!Box Telefon-dingsbums
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ThisAddIn_Startup(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Startup

        AddHandler Microsoft.Win32.SystemEvents.PowerModeChanged, AddressOf AnrMonRestartNachStandBy

        P_oApp = CType(Application, Outlook.Application)

        If P_oApp.ActiveExplorer IsNot Nothing Then
#If OVer = 11 Then
            Initialisierung = New formInit
#End If
            ' Letzten Anrufer laden. Dazu wird P_oApp benötigt (Kontaktbild)
            P_AnrMon.LetzterAnrufer = P_AnrMon.LadeLetzterAnrufer()
#If OVer < 14 Then
            C_GUI.SymbolleisteErzeugen(ePopWwdh, ePopAnr, ePopVIP, eBtnWaehlen, eBtnDirektwahl, eBtnAnrMonitor, eBtnAnzeigen, eBtnAnrMonNeuStart, eBtnJournalimport, eBtnEinstellungen, _
                                     ePopWwdh1, ePopWwdh2, ePopWwdh3, ePopWwdh4, ePopWwdh5, ePopWwdh6, ePopWwdh7, ePopWwdh8, ePopWwdh9, ePopWwdh10, _
                                     ePopAnr1, ePopAnr2, ePopAnr3, ePopAnr4, ePopAnr5, ePopAnr6, ePopAnr7, ePopAnr8, ePopAnr9, ePopAnr10, _
                                     ePopVIP1, ePopVIP2, ePopVIP3, ePopVIP4, ePopVIP5, ePopVIP6, ePopVIP7, ePopVIP8, ePopVIP9, ePopVIP10)
#End If
            If Not C_DP.P_CBIndexAus Then oInsps = Application.Inspectors
        Else
            C_HF.LogFile("Addin nicht gestartet, da kein Explorer vorhanden")
        End If
    End Sub

    Private Shared Sub Application_Quit() Handles Application.Quit, Me.Shutdown
        C_AnrMon.AnrMonStartStopp()
        C_HF.LogFile(C_DP.P_Def_Addin_LangName & " V" & Version & " beendet.")
        C_DP.SpeichereXMLDatei()
        With C_HF
            .NAR(P_oApp)
        End With
    End Sub

    Private Sub myOlInspectors(ByVal Inspector As Outlook.Inspector) Handles oInsps.NewInspector
#If OVer = 11 Then
        C_GUI.InspectorSybolleisteErzeugen(Inspector, iPopRWS, iBtnWwh, iBtnRWSDasOertliche, iBtnRws11880, iBtnRWSDasTelefonbuch, iBtnRWStelSearch, iBtnRWSAlle, iBtnKontakterstellen, iBtnVIP, iBtnUpload)
#End If
        If TypeOf Inspector.CurrentItem Is Outlook.ContactItem Then
            If C_DP.P_CBKHO AndAlso Not _
                    CType(CType(Inspector.CurrentItem, Outlook.ContactItem).Parent, Outlook.MAPIFolder).StoreID = _
                    C_KF.P_DefContactFolder.StoreID Then Exit Sub
            Dim KS As New ContactSaved
            KS.ContactSaved = CType(Inspector.CurrentItem, Outlook.ContactItem)
            ListofOpenContacts.Add(KS)
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

        With (C_GUI)
            Select Case CType(Ctrl, CommandBarButton).Caption
                Case "Direktwahl"
                    .WähleDirektwahl()
                Case "Wählen"
                    .WählenExplorer()
                Case "Einstellungen"
                    .ÖffneEinstellungen()
                Case "Anrufmonitor"
                    C_AnrMon.AnrMonStartStopp()
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
        C_WClient.OnActionListen(control.Tag)
    End Sub
#End Region
#End If
#End Region

#Region " Office 2003 Inspectorfenster"
#If OVer = 11 Then
    Private Sub iBtn_Click(ByVal Ctrl As Microsoft.Office.Core.CommandBarButton, ByRef CancelDefault As Boolean) Handles iBtnKontakterstellen.Click, _
                                                                                                                         iBtnRWSDasOertliche.Click, _
                                                                                                                         iBtnRws11880.Click, _
                                                                                                                         iBtnRWSDasTelefonbuch.Click, _
                                                                                                                         iBtnRWStelSearch.Click, _
                                                                                                                         iBtnRWSAlle.Click, _
                                                                                                                         iBtnWwh.Click, _
                                                                                                                         iBtnVIP.Click, _
                                                                                                                         iBtnUpload.click

        With (C_GUI)
            Select Case CType(Ctrl, CommandBarButton).Tag
                Case C_DP.P_Tag_Insp_Kontakt
                    .KontaktErstellen()
                Case C_DP.P_RWSDasOertliche_Name
                    .OnActionRWS(oApp.ActiveInspector, RückwärtsSuchmaschine.RWSDasOertliche)
                Case C_DP.P_RWS11880_Name
                    .OnActionRWS(oApp.ActiveInspector, RückwärtsSuchmaschine.RWS11880)
                Case C_DP.P_RWSDasTelefonbuch_Name
                    .OnActionRWS(oApp.ActiveInspector, RückwärtsSuchmaschine.RWSDasTelefonbuch)
                Case C_DP.P_RWSTelSearch_Name
                    .OnActionRWS(oApp.ActiveInspector, RückwärtsSuchmaschine.RWStelSearch)
                Case C_DP.P_RWSAlle_Name
                    .OnActionRWS(oApp.ActiveInspector, RückwärtsSuchmaschine.RWSAlle)
                Case C_DP.P_Tag_Insp_Dial
                    C_WClient.WählenAusInspector()
                Case C_DP.P_CMB_Insp_VIP
                    Dim aktKontakt As Outlook.ContactItem = CType(oApp.ActiveInspector.CurrentItem, Outlook.ContactItem)
                    If .IsVIP(aktKontakt) Then
                        .RemoveVIP(aktKontakt.EntryID, CType(aktKontakt.Parent, Outlook.MAPIFolder).StoreID)
                        Ctrl.State = MsoButtonState.msoButtonUp
                    Else
                        .AddVIP(aktKontakt)
                        Ctrl.State = MsoButtonState.msoButtonDown
                    End If
                Case C_DP.P_CMB_Insp_Upload
                    Dim aktKontakt As Outlook.ContactItem = CType(oApp.ActiveInspector.CurrentItem, Outlook.ContactItem)
                    C_Fbox.UploadKontaktToFritzBox(aktKontakt, .IsVIP(aktKontakt))
            End Select
        End With
    End Sub
#End If
#End Region
End Class