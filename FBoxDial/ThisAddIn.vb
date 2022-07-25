Imports System.Net
Imports System.Threading.Tasks
Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop.Outlook

Public NotInheritable Class ThisAddIn

    Friend Property POutlookRibbons() As OutlookRibbons
    Friend Property PAnrufmonitor As Anrufmonitor
    Friend Property PhoneBookXML As IEnumerable(Of PhonebookEx)
    Friend Property PVorwahlen As Vorwahlen
    Friend Property TellowsScoreList As List(Of TellowsScoreListEntry)
    Friend Property OffeneAnrMonWPF As List(Of AnrMonWPF)
    Friend Property OffeneStoppUhrWPF As List(Of StoppUhrWPF)
    Friend Property AddinWindows As New List(Of Windows.Window)
    Friend Property WPFApplication As App
    Friend Property FBoxTR064 As FBoxAPI.FritzBoxTR64
    Friend Property FBoxhttpClient As AddinHTTPClient
    Private Property LinkProtokoll As DateiÜberwacher
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Timer für Raktivierung nach StandBy"
    Private Property NeustartTimer As Timers.Timer
    Private Property NeustartTimerIterations As Integer
#End Region

    Protected Overrides Function CreateRibbonExtensibilityObject() As IRibbonExtensibility
        If POutlookRibbons Is Nothing Then POutlookRibbons = New OutlookRibbons
        Return POutlookRibbons
    End Function

    Private Sub ThisAddIn_Startup() Handles Me.Startup
        ' Logging konfigurieren
        LogManager.Configuration = DefaultNLogConfig()

        ' Outlook.Application initialisieren
        If Application.ActiveExplorer IsNot Nothing Then

            ' Application laden https://github.com/didzispetkus/vsto-external-resource-library
            If Windows.Application.Current Is Nothing Then WPFApplication = New App
            Windows.Application.Current.ShutdownMode = Windows.ShutdownMode.OnExplicitShutdown

            ' Ereignishandler für StandBy / Resume
            NLogger.Debug("Füge Ereignishandler für PowerModeChanged hinzu.")
            AddHandler Microsoft.Win32.SystemEvents.PowerModeChanged, AddressOf PowerModeChanged

            ' Starte die Funktionen des Addins asynchron
            NLogger.Info($"Starte {My.Resources.strDefLongName} {Reflection.Assembly.GetExecutingAssembly.GetName.Version} ({Globals.ThisAddIn.Application.Name} {Globals.ThisAddIn.Application.Version})...")

            StarteAddinFunktionen()
        Else
            NLogger.Warn("Addin nicht gestartet, da kein Explorer vorhanden")
        End If
    End Sub

    Private Sub StarteAddinFunktionen()

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        ' Initialisiere die Landes- und Ortskennzahlen und beginne das Einlesen
        PVorwahlen = New Vorwahlen
        NLogger.Debug("Landes- und Ortskennzahlen geladen...")

        ' Lade die Nutzerdaten
        Dim UserData As New NutzerDaten
        NLogger.Debug("Nutzererinstellungen geladen...")

        ' Theme gemäß den aktuellen Einstellungen von Office setzen
        UpdateTheme()

        ' Explorer Ereignishandler festlegen
        SetExplorer()
        NLogger.Debug("Outlook-Explorer Ereignishandler erfasst...")

        ' Outlook Inspektoren erfassen
        SetInspector()
        NLogger.Debug("Outlook-Inspektor Ereignishandler erfasst...")

        ' Globaler httpClient für Rückwärtssuche und Tellows
        FBoxhttpClient = New AddinHTTPClient

        ' Initiiere den timergesteuerten Start der einzelnen Funktionen
        TimerStart()

        ' enable keyboard intercepts
        SetupKeyboardHooking()
    End Sub

    ''' <summary>
    ''' Routine wird mittels Timer gestartet. Auch nach dem Aufwachen aus dem Standby.
    ''' </summary>
    Private Async Sub InitFBoxConnection()
        Dim TaskScoreListe As Task(Of List(Of TellowsScoreListEntry)) = Nothing
        Dim TaskTelefonbücher As Task(Of IEnumerable(Of PhonebookEx)) = Nothing
        Dim TaskAnrList As Task(Of FBoxAPI.CallList) = Nothing

        ' TR064 Schnittstelle definieren. 
        FBoxTR064 = New FBoxAPI.FritzBoxTR64(New FBoxAPI.Settings With {.Anmeldeinformationen = FritzBoxDefault.Anmeldeinformationen,
                                                                        .FritzBoxAdresse = XMLData.POptionen.ValidFBAdr,
                                                                        .LogWriter = New FBoxAPILog})

        ' Anrufmonitor starten
        If XMLData.POptionen.CBAnrMonAuto Then
            If PAnrufmonitor Is Nothing Then PAnrufmonitor = New Anrufmonitor

            PAnrufmonitor.Start()
            NLogger.Debug("Anrufmonitor gestartet...")
        End If

        ' Schreibe in das Log noch Informationen zur Fritz!Box
        NLogger.Info($"{FBoxTR064.FriendlyName} {FBoxTR064.DisplayVersion}")

        ' Lade die Anrufliste herunter
        If XMLData.POptionen.CBAutoAnrList Then TaskAnrList = LadeFritzBoxAnrufliste(XMLData.POptionen.FBoxCallListLastImportedID,
                                                                                     XMLData.POptionen.FBoxCallListTimeStamp)

        ' Lade alle Telefonbücher aus der Fritz!Box via Task herunter
        If XMLData.POptionen.CBKontaktSucheFritzBox Then
            TaskTelefonbücher = Telefonbücher.LadeTelefonbücher()
        Else
            ' Falls die Kontaktsuche nicht über die Fritz!Box Telefonbücher laufen soll, dann lade die Telefonbuchnamen herunter
            TaskTelefonbücher = Task.Run(Function() Telefonbücher.LadeTelefonbücherNamen())
        End If

        ' Tellows ScoreList laden
        If XMLData.POptionen.CBTellowsAutoUpdateScoreList Then
            Using tellows As New Tellows
                TaskScoreListe = tellows.LadeScoreList
            End Using
        End If

        ' Beendigung des Task für das Herunterladen der Fritz!Box Telefonbücher abwarten
        If TaskTelefonbücher IsNot Nothing Then
            PhoneBookXML = Await TaskTelefonbücher
            NLogger.Debug($"Fritz!Box Telefonbücher geladen...")
        End If

        ' Beendigung des Task für das Herunterladen der tellows ScoreList abwarten
        If TaskScoreListe IsNot Nothing Then
            TellowsScoreList = Await TaskScoreListe
            If TellowsScoreList IsNot Nothing Then
                NLogger.Debug($"Die tellows Scorelist mit {TellowsScoreList.Count} Einträgen geladen.")
            Else
                NLogger.Warn($"Die tellows Scorelist konnte nicht geladen werden.")
            End If
        End If

        ' Anrufliste auswerten
        If TaskAnrList IsNot Nothing Then
            NLogger.Debug("Auswertung Anrufliste gestartet...")
            AutoAnrListe(Await TaskAnrList)
        End If

        ' Aktualisierung der tellows Sperrliste
        If XMLData.POptionen.CBTellowsAutoUpdateScoreList Then
            NLogger.Debug("Update Rufsperre durch tellows gestartet...")
            AutoBlockListe()
        End If

        ' Dateisystemüberwachung für tel:// und callto:// Links
        If XMLData.POptionen.CBLinkProtokoll Then
            NLogger.Debug("Dateiüberwachung für tel:// und callto:// Links gestartet...")
            LinkProtokoll = New DateiÜberwacher(IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName), My.Resources.strLinkProtFileName)
        End If

    End Sub

    Private Sub BeendeAddinFunktionen()
        ' Inspector
        RemoveHandler InspectorListe.NewInspector, AddressOf Inspectoren_NewInspector
        InspectorListe = Nothing
        InspectorWrappers = Nothing

        ' Explorer
        RemoveHandler ExplorerListe.NewExplorer, AddressOf Explorer_NewExplorer
        ExplorerListe = Nothing
        ExplorerWrappers = Nothing

        ' Listen leeren
        If PVorwahlen IsNot Nothing Then PVorwahlen.Kennzahlen.Landeskennzahlen.Clear()

        ' Anrufmonitor beenden
        If PAnrufmonitor IsNot Nothing Then PAnrufmonitor.Stopp()

        ' Dateisystemüberwachung für tel:// und callto:// Links
        LinkProtokoll?.Dispose()

        ' TR-064-Schnittstelle auflösen
        FBoxTR064?.Dispose()

        ' HttpClient auflösen
        FBoxhttpClient?.Dispose()

        ' Eintrag ins Log
        NLogger.Info($"{My.Resources.strDefLongName} {Reflection.Assembly.GetExecutingAssembly.GetName.Version} beendet.")

        ' XML-Datei Speichern
        XmlSerializeToFile(XMLData, IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, $"{My.Resources.strDefShortName}.xml"))

        ' disable keyboard intercepts
        If XMLData.POptionen.CBKeyboard Then KeyboardHooking.ReleaseHook()
    End Sub

    Private Sub Application_Quit() Handles Application.Quit, Me.Shutdown
        BeendeAddinFunktionen()

        ' //github.com/didzispetkus/vsto-external-resource-library
        If Windows.Application.Current IsNot Nothing Then Windows.Application.Current.Shutdown()

        ReleaseComObject(Application)
    End Sub

#Region "Standby Wakeup TimerStart"
    ''' <summary>
    ''' Startet das Addin nach dem Aufwachen nach dem Standby neu, bzw. Beendet es, falls ein Standyby erkannt wird.
    ''' </summary>
    Private Sub PowerModeChanged(sender As Object, e As Microsoft.Win32.PowerModeChangedEventArgs)

        NLogger.Info($"PowerMode: {e.Mode} ({e.Mode})")

        Select Case e.Mode
            Case Microsoft.Win32.PowerModes.Resume
                ' Wiederherstelung nach dem Standby
                StarteAddinFunktionen()

            Case Microsoft.Win32.PowerModes.Suspend
                ' Beende alle Funktionen des Addins
                BeendeAddinFunktionen()
        End Select
    End Sub

    ''' <summary>
    ''' Nach dem Aufwachen aus dem Standby besteht meist noch keine Verbindung zur Fritz!Box. Es wird mehrfach, mittels <see cref="Ping(ByRef String)"/>, geprüft, ob die Fritz!Box wieder erreichbar ist. 
    ''' </summary>
    Private Sub TimerStart()

        If NeustartTimer IsNot Nothing Then
            NLogger.Debug("Timer für den Start wird neu gestartet.")

            ' Ereignishandler entfernen
            RemoveHandler NeustartTimer.Elapsed, AddressOf NeustartTimer_Elapsed

            ' Timer stoppen und auflösen
            With NeustartTimer
                .Stop()
                .Dispose()
            End With
        End If

        ' Initiiere einen neuen Timer
        NLogger.Debug("Timer für Start des Addins wird gestartet.")

        ' Setze die Zählvariable auf 0
        NeustartTimerIterations = 0

        ' Initiiere den Timer mit Intervall von 2 Sekunden
        NeustartTimer = New Timers.Timer
        With NeustartTimer
            .Interval = 2000
            .AutoReset = True
            ' Starte den Timer
            .Start()
        End With

        ' Ereignishandler hinzufügen
        AddHandler NeustartTimer.Elapsed, AddressOf NeustartTimer_Elapsed
    End Sub

    Private Sub NeustartTimer_Elapsed(sender As Object, e As Timers.ElapsedEventArgs)
        ' Prüfe, ob die maximale Anzahl an Durchläufen (15) noch nicht erreicht wurde
        If NeustartTimerIterations.IsLess(15) Then

            ' Wenn ein Ping zur Fritz!Box erfolgreich war, dann hat das Wiederverbinden geklappt.
            If Ping(XMLData.POptionen.ValidFBAdr) Then
                ' Halte den TImer an und löse ihn auf
                With NeustartTimer
                    .Stop()
                    .Dispose()
                End With

                ' Starte alle weiteren Addinfunktionen
                InitFBoxConnection()
                ' Statusmeldung
                NLogger.Info($"Addin konnte nach {NeustartTimerIterations} Versuchen erfolgreich gestartet werden.")
            Else
                ' Erhöhe den Wert der durchgeführten Iterationen
                NeustartTimerIterations += 1
                ' Statusmeldung
                NLogger.Debug($"Timer: Starte {NeustartTimerIterations}. Versuch den Anrufmonitor zu starten.")
            End If

        Else
            ' Es konnte keine Verbindung zur Fritz!Box aufgebaut werden.
            NLogger.Warn($"Addin konnte nach {NeustartTimerIterations} Versuchen nicht gestartet werden.")

            ' Ereignishandler entfernen
            RemoveHandler NeustartTimer.Elapsed, AddressOf NeustartTimer_Elapsed

            ' Timer stoppen und auflösen
            With NeustartTimer
                .Stop()
                .Dispose()
            End With
        End If
        ' Ribbon aktualisieren
        POutlookRibbons.RefreshRibbon()
    End Sub
#End Region

#Region "Outlook Explorer"
    Friend ExplorerWrappers As Dictionary(Of Explorer, ExplorerWrapper)
    Private Property ExplorerListe As Explorers

    Private Sub SetExplorer()
        ' Liste aller Outlook Explorer erfassen
        ExplorerListe = Application.Explorers
        ' Eventhandler hinzufügen
        AddHandler ExplorerListe.NewExplorer, AddressOf Explorer_NewExplorer
        ' ExplorerWrappers initiieren
        ExplorerWrappers = New Dictionary(Of Explorer, ExplorerWrapper)

        For Each E As Explorer In ExplorerListe
            Explorer_NewExplorer(E)
        Next

    End Sub

    Private Sub Explorer_NewExplorer(e As Explorer)
        ExplorerWrappers.Add(e, New ExplorerWrapper(e))
    End Sub

#End Region

#Region "Outlook Inspector"

    Friend InspectorWrappers As Dictionary(Of Inspector, InspectorWrapper)
    Private Property InspectorListe As Inspectors

    Private Sub SetInspector()
        ' Liste aller Outlook Inspectoren erfassen
        InspectorListe = Application.Inspectors
        ' Eventhandler hinzufügen
        AddHandler InspectorListe.NewInspector, AddressOf Inspectoren_NewInspector
        ' InspectorWrappers initiieren
        InspectorWrappers = New Dictionary(Of Inspector, InspectorWrapper)
        For Each I As Inspector In InspectorListe
            Inspectoren_NewInspector(I)
        Next
    End Sub
    Private Sub Inspectoren_NewInspector(Inspector As Inspector)
        If TypeOf Inspector.CurrentItem Is ContactItem Then
            InspectorWrappers.Add(Inspector, New InspectorWrapper(Inspector))
        End If
    End Sub
#End Region

#Region "KeyboardHooking"
    Friend Sub SetupKeyboardHooking()
        If XMLData.POptionen.CBKeyboard Then
            KeyboardHooking.SetHook(XMLData.POptionen.CBKeyboardModifierShift, XMLData.POptionen.CBKeyboardModifierControl)
        Else
            KeyboardHooking.ReleaseHook()
        End If
    End Sub
#End Region
End Class
