Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop.Outlook

Public NotInheritable Class ThisAddIn
    Friend Shared Property POutlookRibbons() As OutlookRibbons
    Friend Shared Property POutookApplication As Application

    Private WithEvents OutlookInspectors As Inspectors
    Friend Shared Property PAnrufmonitor As Anrufmonitor
    Friend Shared Property PPhoneBookXML As FritzBoxXMLTelefonbücher
    'Friend Shared Property PCallListXML As FritzBoxXMLCallList

    Friend Shared Property PCVorwahlen As CVorwahlen
    Friend Shared Property OffeneKontakInsepektoren As List(Of KontaktGespeichert)
    Friend Shared Property OffenePopUps As List(Of Popup)


    Private Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend Shared ReadOnly Property Version() As String
        Get
            With Reflection.Assembly.GetExecutingAssembly.GetName.Version
                Return String.Format("V{0}.{1}.{2}", .Major, .Minor, .Build)
            End With
        End Get
    End Property

    Protected Overrides Function CreateRibbonExtensibilityObject() As IRibbonExtensibility
        If POutlookRibbons Is Nothing Then POutlookRibbons = New OutlookRibbons
        Return POutlookRibbons
    End Function

    Private Sub ThisAddIn_Startup() Handles Me.Startup
        Dim UserData As NutzerDaten = New NutzerDaten
        ' Logging konfigurieren
        LogManager.Configuration = DefaultNLogConfig()

        ' Outlook.Application initialisieren
        If POutookApplication Is Nothing Then POutookApplication = Application

        If POutookApplication.ActiveExplorer IsNot Nothing Then
            'StandBy Handler
            AddHandler Microsoft.Win32.SystemEvents.PowerModeChanged, AddressOf AnrMonRestartNachStandBy
            ' Starte die Funktionen des Addins
            StarteAddinFunktionen()

        Else
            NLogger.Warn("Addin nicht gestartet, da kein Explorer vorhanden")
        End If
    End Sub

    Private Async Sub StarteAddinFunktionen()

        ' Initialisiere die Landes- und Ortskennzahlen
        PCVorwahlen = New CVorwahlen

        ' Anrufmonitor starten
        If XMLData.POptionen.PCBAnrMonAuto Then
            PAnrufmonitor = New Anrufmonitor
            PAnrufmonitor.StartStopAnrMon()
        End If

        ' Lade alle Telefonbücher aus der Fritz!Box herunter
        If XMLData.POptionen.PCBKontaktSucheFritzBox Then PPhoneBookXML = Await LadeFritzBoxTelefonbücher()

        ' Inspektoren erfassen
        OutlookInspectors = Application.Inspectors

        ' Anrufliste auswerten
        If XMLData.POptionen.PCBAutoAnrList Then AutoAnrListe()

        NLogger.Info("{0} {1} gestartet.", PDfltAddin_LangName, Version)

    End Sub

    Private Sub Application_Quit() Handles Application.Quit, Me.Shutdown
        ' Listen leeren
        If Not PCVorwahlen Is Nothing Then
            PCVorwahlen.Kennzahlen.Landeskennzahlen.Clear()
        End If
        ' Anrufmonitor beenden
        If PAnrufmonitor IsNot Nothing Then PAnrufmonitor.Stopp()
        ' Eintrag ins Log
        NLogger.Info("{0} {1} beendet.", PDfltAddin_LangName, Version)
        ' XML-Datei Speichern
        XMLData.Speichern(IO.Path.Combine(XMLData.POptionen.PArbeitsverzeichnis, $"{PDfltAddin_KurzName}.xml"))
    End Sub

#Region "Standby Wakeup"
    ''' <summary>
    ''' Startet den Anrufmonitor nach dem Aufwachen nach dem Standby neu, bzw. Beendet ihn, falls ein Standyby erkannt wird.
    ''' </summary>
    Sub AnrMonRestartNachStandBy(ByVal sender As Object, ByVal e As Microsoft.Win32.PowerModeChangedEventArgs)
        NLogger.Info("PowerMode: {0} ({1})", e.Mode.ToString, e.Mode)
        Select Case e.Mode
            Case Microsoft.Win32.PowerModes.Resume
                ' Wiederherstelung nach dem Standby
                ' StarteAddinFunktionen()

                ' Anrufmonitor starten
                If XMLData.POptionen.PCBAnrMonAuto Then
                    PAnrufmonitor = New Anrufmonitor
                    PAnrufmonitor.StartStopAnrMon()
                End If
            Case Microsoft.Win32.PowerModes.Suspend
                ' Anrufmonitor beenden
                If PAnrufmonitor IsNot Nothing Then PAnrufmonitor.Stopp()
                ' XML-Datei speichern
                XMLData.Speichern(IO.Path.Combine(XMLData.POptionen.PArbeitsverzeichnis, $"{PDfltAddin_KurzName}.xml"))
        End Select
    End Sub
#End Region

#Region "Inspector"
    Private Sub OutlookInspectors_NewInspector(Inspector As Inspector) Handles OutlookInspectors.NewInspector
        If TypeOf Inspector.CurrentItem Is ContactItem Then
            If OffeneKontakInsepektoren Is Nothing Then OffeneKontakInsepektoren = New List(Of KontaktGespeichert)
            OffeneKontakInsepektoren.Add(New KontaktGespeichert() With {.Kontakt = CType(Inspector.CurrentItem, ContactItem)})
        End If
    End Sub
#End Region
End Class
