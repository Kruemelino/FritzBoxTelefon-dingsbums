Imports System.Threading.Tasks
Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop.Outlook

Public NotInheritable Class ThisAddIn

    Friend Shared Property POutlookRibbons() As OutlookRibbons
    Friend Shared Property OutookApplication As Application
    Friend Shared Property PAnrufmonitor As Anrufmonitor
    Friend Shared Property PhoneBookXML As IEnumerable(Of PhonebookEx)
    Friend Shared Property PVorwahlen As Vorwahlen
    Friend Shared Property TellowsScoreList As List(Of TellowsScoreListEntry)
    Friend Shared Property OffeneAnrMonWPF As List(Of AnrMonWPF)
    Friend Shared Property OffeneStoppUhrWPF As List(Of StoppUhrWPF)
    Friend Shared Property AddinWindows As New List(Of Windows.Window)
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
        If OutookApplication Is Nothing Then OutookApplication = Application

        If OutookApplication.ActiveExplorer IsNot Nothing Then
            ' Ereignishandler für StandBy / Resume
            NLogger.Debug("Füge Ereignishandler für PowerModeChanged hinzu.")
            AddHandler Microsoft.Win32.SystemEvents.PowerModeChanged, AddressOf PowerModeChanged

            ' Starte die Funktionen des Addins asynchron
            NLogger.Info($"Starte {My.Resources.strDefLongName} {Reflection.Assembly.GetExecutingAssembly.GetName.Version}...")

            StarteAddinFunktionen()
        Else
            NLogger.Warn("Addin nicht gestartet, da kein Explorer vorhanden")
        End If
    End Sub

    Private Sub StarteAddinFunktionen()
        ' Initialisiere die Landes- und Ortskennzahlen und beginne das Einlesen
        PVorwahlen = New Vorwahlen
        NLogger.Debug("Landes- und Ortskennzahlen geladen...")

        ' Lade die Nutzerdaten
        Dim UserData As New NutzerDaten
        NLogger.Debug("Nutzererinstellungen geladen...")

        ' Initiiere die TR064 Schnittstelle für die Abfragen der Daten der Fritz!Box
        TimerStart()

        ' enable keyboard intercepts
        If XMLData.POptionen.CBKeyboard Then KeyboardHooking.SetHook()
    End Sub

    ''' <summary>
    ''' Routine wird mittels Timer gestartet. Auch nach dem Aufwachen aus dem Standby.
    ''' </summary>
    Private Async Sub InitFBoxConnection()

        Dim TaskScoreListe As Task(Of List(Of TellowsScoreListEntry)) = Nothing
        Dim TaskTelefonbücher As Task(Of IEnumerable(Of PhonebookEx)) = Nothing
        Dim TaskAnrList As Task(Of FBoxAPI.CallList) = Nothing

        ' Explorer Ereignishandler festlegen
        SetExplorer()
        NLogger.Debug("Outlook-Explorer Ereignishandler erfasst...")

        ' Outlook Inspektoren erfassen
        OutlookInspectors = Application.Inspectors
        NLogger.Debug("Outlook-Inspektor Ereignishandler erfasst...")

        ' Anrufmonitor starten
        If XMLData.POptionen.CBAnrMonAuto Then
            If PAnrufmonitor Is Nothing Then PAnrufmonitor = New Anrufmonitor

            PAnrufmonitor.Start()
            NLogger.Debug("Anrufmonitor gestartet...")
        End If

        Using FBoxTR064 = New FBoxAPI.FritzBoxTR64()

            ' Ereignishandler hinzufügen
            AddHandler FBoxTR064.Status, AddressOf FBoxAPIMessage
            ' TR064 Schnittstelle initiieren
            FBoxTR064.Init(XMLData.POptionen.ValidFBAdr, XMLData.POptionen.TBNetworkTimeout, FritzBoxDefault.Anmeldeinformationen)

            If FBoxTR064.Bereit Then
                ' Schreibe in das Log noch Informationen zur Fritz!Box
                NLogger.Info($"{FBoxTR064.FriendlyName} {FBoxTR064.DisplayVersion}")

                ' Lade die Anrufliste herunter
                If XMLData.POptionen.CBAutoAnrList Then TaskAnrList = LadeFritzBoxAnrufliste(FBoxTR064)

                ' Lade alle Telefonbücher aus der Fritz!Box via Task herunter
                If XMLData.POptionen.CBKontaktSucheFritzBox Then TaskTelefonbücher = Telefonbücher.LadeTelefonbücher(FBoxTR064)

                ' Tellows ScoreList laden
                If XMLData.POptionen.CBTellowsAutoUpdateScoreList Then
                    Using tellows As New Tellows
                        TaskScoreListe = tellows.LadeScoreList
                    End Using
                End If
            Else
                NLogger.Warn("TR064 Schnittstelle der Fritz!Box nicht verfügbar.")
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
                AutoBlockListe(FBoxTR064)
            End If

            ' Ereignishandler entfernen
            RemoveHandler FBoxTR064.Status, AddressOf FBoxAPIMessage

        End Using

    End Sub

    Private Sub BeendeAddinFunktionen()

        ' Listen leeren
        If PVorwahlen IsNot Nothing Then PVorwahlen.Kennzahlen.Landeskennzahlen.Clear()
        ' Anrufmonitor beenden
        If PAnrufmonitor IsNot Nothing Then PAnrufmonitor.Stopp()
        ' Eintrag ins Log
        NLogger.Info($"{My.Resources.strDefLongName} {Reflection.Assembly.GetExecutingAssembly.GetName.Version} beendet.")
        ' XML-Datei Speichern
        XmlSerializeToFile(XMLData, IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, $"{My.Resources.strDefShortName}.xml"))

        ' disable keyboard intercepts
        If XMLData.POptionen.CBKeyboard Then KeyboardHooking.ReleaseHook()
    End Sub

    Private Sub Application_Quit() Handles Application.Quit, Me.Shutdown

        BeendeAddinFunktionen()

        ReleaseComObject(OutookApplication)
        OutookApplication = Nothing
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
    Private WithEvents OutlookExplorers As Explorers
    Private WithEvents OutlookMainExplorer As Explorer

    Private Sub SetExplorer()

        ' Outlook Haupt Explorer festlegen
        OutlookMainExplorer = Application.ActiveExplorer

        ' Outlook Explorer erfassen
        OutlookExplorers = Application.Explorers

    End Sub

    ''' <summary>
    ''' Tritt ein, wenn ein neues Explorer-Fenster geöffnet wird, entweder als Ergebnis einer Benutzeraktion oder durch Programmcode.
    ''' </summary>
    Private Sub OutlookExplorers_NewExplorer(Explorer As Explorer) Handles OutlookExplorers.NewExplorer
        NLogger.Debug("Ein neues Explorer-Fenster wird geöffnet")
        AddHandler Explorer.BeforeItemPaste, AddressOf OutlookExplorer_BeforeItemPaste

        AddHandler Explorer.SelectionChange, AddressOf OutlookExplorer_SelectionChange
    End Sub

    Private Sub OutlookExplorer_SelectionChange() Handles OutlookMainExplorer.SelectionChange
        POutlookRibbons.RefreshRibbon()
    End Sub

    ''' <summary>
    ''' Tritt ein, wenn ein Outlook-Element eingefügt wird.
    ''' </summary>
    Private Sub OutlookExplorer_BeforeItemPaste(ByRef ClipboardContent As Object, Target As MAPIFolder, ByRef Cancel As Boolean) Handles OutlookMainExplorer.BeforeItemPaste

        ' Ist der Inhalt eine Selection? (Im Besten Fall eine Anzahl an Kontakten)
        If TypeOf ClipboardContent Is Selection Then
            ' Schleife durch alle Elemente der selektierten Objekte
            For Each ClipboardObject As Object In CType(ClipboardContent, Selection)

                ' Wenn es sich um Kontakte handelt, dann (de-)indiziere den Kontakt
                If TypeOf ClipboardObject Is ContactItem Then

                    IndiziereKontakt(CType(ClipboardObject, ContactItem), Target, True)

                End If
            Next
        End If
    End Sub

#End Region

#Region "Outlook Inspector"
    Private WithEvents OutlookInspectors As Inspectors
    Friend Shared Property KontakInsepektorenListe As List(Of KontaktInspector)

    ''' <summary>
    ''' Tritt ein, wenn als Ergebnis einer Benutzeraktion oder durch Programmcode ein neues Inspektor-Fenster geöffnet wird.
    ''' </summary>
    Private Sub OutlookInspectors_NewInspector(Inspector As Inspector) Handles OutlookInspectors.NewInspector

        ' Handelt es sich um einen Kontakt-ispektor?
        If TypeOf Inspector.CurrentItem Is ContactItem Then

            ' Initiiere die Liste der Offenen Kontaktinspektoren, falls noch nicht geschehen
            If KontakInsepektorenListe Is Nothing Then KontakInsepektorenListe = New List(Of KontaktInspector)

            ' Füge diesen Kontaktinspektor hinzu
            KontakInsepektorenListe.Add(New KontaktInspector() With {.OlKontakt = CType(Inspector.CurrentItem, ContactItem)})
        End If
    End Sub

#End Region
End Class
