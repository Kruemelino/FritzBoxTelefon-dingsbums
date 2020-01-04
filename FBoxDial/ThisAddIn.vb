Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop.Outlook

Public NotInheritable Class ThisAddIn
    Friend Shared Property POutlookRibbons() As OutlookRibbons
    Friend Shared Property POutookApplication As Application
    Friend Shared Property PAnrufmonitor As Anrufmonitor
    Friend Shared Property PPhoneBookXML As FritzBoxXMLTelefonbücher
    Friend Shared Property PCallListXML As FritzBoxXMLCallList

    Private WithEvents OutlookInspectors As Inspectors
    Friend Shared Property OffeneKontakInsepektoren As List(Of ContactSaved)
    Friend Shared ReadOnly Property Version() As String
        Get
            With Reflection.Assembly.GetExecutingAssembly.GetName.Version
                Return .Major & "." & .Minor & "." & .Build
            End With
        End Get
    End Property

    Protected Overrides Function CreateRibbonExtensibilityObject() As IRibbonExtensibility
        If POutlookRibbons Is Nothing Then POutlookRibbons = New OutlookRibbons
        Return POutlookRibbons
    End Function
    Private Sub ThisAddIn_Startup() Handles Me.Startup
        Dim UserData As NutzerDaten = New NutzerDaten

        ' Outlook.Application initialisieren
        If POutookApplication Is Nothing Then POutookApplication = CType(Application, Application)

        If POutookApplication.ActiveExplorer IsNot Nothing Then
            'StandBy Handler
            AddHandler Microsoft.Win32.SystemEvents.PowerModeChanged, AddressOf AnrMonRestartNachStandBy
            ' Starte die Funktionen des Addins
            StarteAddinFunktionen()
        Else
            LogFile("Addin nicht gestartet, da kein Explorer vorhanden")
        End If

    End Sub

    Private Async Sub StarteAddinFunktionen()
        ' Anrufmonitor starten
        If XMLData.POptionen.PCBAnrMonAuto Then
            PAnrufmonitor = New Anrufmonitor
            PAnrufmonitor.StartStopAnrMon()
        End If

        ' Lade alle Telefonbücher aus der Fritz!Box herunter
        If XMLData.POptionen.PCBKontaktSucheFritzBox Then
            Await LadeFritzBoxTelefonbücher()
        End If

        ' Inspektoren erfassen
        OutlookInspectors = Application.Inspectors

        ' Anrufliste auswerten
        If XMLData.POptionen.PCBAutoAnrList Then AutoAnrListe()

    End Sub

    Private Sub Application_Quit() Handles Application.Quit, Me.Shutdown
        ' Eintrag ins Log
        LogFile(String.Format("{0} V{1} beendet.", PDfltAddin_LangName, Version))

        ' XML-Datei Speichern
        XMLData.Speichern()
    End Sub

#Region "Standby Wakeup"
    ''' <summary>
    ''' Startet den Anrufmonitor nach dem Aufwachen nach dem Standby neu, bzw. Beendet ihn, falls ein Standyby erkannt wird.
    ''' </summary>
    Sub AnrMonRestartNachStandBy(ByVal sender As Object, ByVal e As Microsoft.Win32.PowerModeChangedEventArgs)
        LogFile("PowerMode: " & e.Mode.ToString & " (" & e.Mode & ")")
        Select Case e.Mode
            Case Microsoft.Win32.PowerModes.Resume
                ' Wiederherstelung nach dem Standby
                StarteAddinFunktionen()
            Case Microsoft.Win32.PowerModes.Suspend
                ' XML-Datei speichern
                XMLData.Speichern()
        End Select
    End Sub
#End Region

#Region "Inspector"
    Private Sub OutlookInspectors_NewInspector(Inspector As Inspector) Handles OutlookInspectors.NewInspector
        If TypeOf Inspector.CurrentItem Is ContactItem Then
            If OffeneKontakInsepektoren Is Nothing Then OffeneKontakInsepektoren = New List(Of ContactSaved)
            OffeneKontakInsepektoren.Add(New ContactSaved() With {.Kontakt = CType(Inspector.CurrentItem, ContactItem)})
        End If
    End Sub
#End Region
End Class
