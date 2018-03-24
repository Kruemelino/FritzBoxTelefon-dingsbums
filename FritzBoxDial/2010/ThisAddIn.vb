Imports Microsoft.Office.Core
Public Class ThisAddIn
    Private WithEvents oInsps As Outlook.Inspectors
    Friend Shared ListofOpenContacts As New Generic.List(Of ContactSaved)
    Public Shared Event PowerModeChanged As Microsoft.Win32.PowerModeChangedEventHandler

#Region "Eigene Formulare"
    Private F_AnrListImport As formImportAnrList
    Private F_Cfg As formCfg
    Private F_Init As formInit
#End Region

#Region "Properties"
    ''' <summary>
    ''' Gibt die Versionsnummer des Addins zurück.
    ''' </summary>
    ''' <value>System.Reflection.Assembly.GetExecutingAssembly.GetName.Version</value>
    ''' <returns>.Major.Minor.Build</returns>
    Friend Shared ReadOnly Property Version() As String
        Get
            With Reflection.Assembly.GetExecutingAssembly.GetName.Version
                Return .Major & "." & .Minor & "." & .Build
            End With
        End Get
    End Property

    ''' <summary>
    ''' Gibt die aktuelle Outlook-Application zurück.
    ''' </summary>
    Friend Shared Property P_oApp() As Outlook.Application

    ''' <summary>
    ''' Rückgabewert für die Klasse DataProvider 
    ''' </summary>
    Friend Property P_DP() As DataProvider

    ''' <summary>
    ''' Rückgabewert für die Klasse Helfer 
    ''' </summary>
    Friend Property P_HF() As Helfer

    ''' <summary>
    ''' Rückgabewert für die Klasse KontaktFunktionen 
    ''' </summary>
    Friend Property P_KF() As KontaktFunktionen

    ''' <summary>
    ''' Rückgabewert für die Klasse GraphicalUserInterface 
    ''' </summary>
    Friend Property P_GUI() As GraphicalUserInterface

    ''' <summary>
    ''' Rückgabewert für die Klasse AnrufMonitor 
    ''' </summary>
    Friend Property P_AnrMon() As AnrufMonitor

    ''' <summary>
    ''' Rückgabewert für die Klasse XML 
    ''' </summary>
    Friend Property P_XML() As XML

    Friend Property P_FBox() As FritzBox

#End Region

    Protected Overrides Function CreateRibbonExtensibilityObject() As IRibbonExtensibility
        F_Init = New formInit(P_GUI, P_KF, P_HF, P_DP, P_AnrMon, P_XML, P_FBox)
        Return P_GUI
    End Function

    ''' <summary>
    ''' Startet den Anrufmonitor nach dem Aufwachen nach dem Standby neu, bzw. Beendet ihn, falls ein Standyby erkannt wird.
    ''' </summary>
    Sub AnrMonRestartNachStandBy(ByVal sender As Object, ByVal e As Microsoft.Win32.PowerModeChangedEventArgs)
        P_HF.LogFile("PowerMode: " & e.Mode.ToString & " (" & e.Mode & ")")
        Select Case e.Mode
            Case Microsoft.Win32.PowerModes.Resume
                ThisAddIn_Startup(True)
            Case Microsoft.Win32.PowerModes.Suspend
                P_AnrMon.AnrMonStartStopp()
                P_DP.SpeichereXMLDatei()
        End Select
    End Sub

    ''' <summary>
    ''' Startet das Fritz!Box Telefon-dingsbums
    ''' </summary>
    Private Overloads Sub ThisAddIn_Startup(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Startup
        'StandBy Handler
        AddHandler Microsoft.Win32.SystemEvents.PowerModeChanged, AddressOf AnrMonRestartNachStandBy

        ' Starte das Addin normal
        ThisAddIn_Startup(False)
    End Sub

    ''' <summary>
    ''' Startet das Fritz!Box Telefon-dingsbums manuell
    ''' </summary>
    ''' <param name="Standby">Angabe, obb das Addin aus dem Standby automatisch gestartet wird.</param>
    Private Overloads Sub ThisAddin_Startup(ByVal Standby As Boolean)

        If P_oApp Is Nothing Then
            P_oApp = CType(Application, Outlook.Application)
        End If

        If Standby Then
            P_GUI.RefreshRibbon()

            F_Init.StandByReStart()
        Else
            If P_oApp.ActiveExplorer IsNot Nothing Then
                ' Letzten Anrufer laden. Dazu wird P_oApp benötigt (Kontaktbild)
                P_AnrMon.LetzterAnrufer = P_AnrMon.LadeLetzterAnrufer()

                If Not P_DP.P_CBIndexAus Then oInsps = Application.Inspectors
            Else
                P_HF.LogFile("Addin nicht gestartet, da kein Explorer vorhanden")
            End If
        End If

    End Sub

    Private Sub Application_Quit() Handles Application.Quit, Me.Shutdown
        P_AnrMon.AnrMonStartStopp()
        P_HF.LogFile(DataProvider.P_Def_Addin_LangName & " V" & Version & " beendet.")
        P_DP.SpeichereXMLDatei()
        P_HF.NAR(P_oApp)
    End Sub

    Private Sub myOlInspectors(ByVal Inspector As Outlook.Inspector) Handles oInsps.NewInspector
        If TypeOf Inspector.CurrentItem Is Outlook.ContactItem Then
            If Not (P_DP.P_CBKHO AndAlso Not _
                    CType(CType(Inspector.CurrentItem, Outlook.ContactItem).Parent, Outlook.MAPIFolder).StoreID =
                    P_KF.P_DefContactFolder.StoreID) Then

                Dim KS As New ContactSaved(P_KF)
                KS.ContactSaved = CType(Inspector.CurrentItem, Outlook.ContactItem)
                ListofOpenContacts.Add(KS)
            End If
        End If
    End Sub

End Class