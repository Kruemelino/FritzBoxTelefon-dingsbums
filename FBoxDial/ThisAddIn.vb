Imports System.Threading.Tasks
Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop.Outlook

Public NotInheritable Class ThisAddIn

    Friend Shared Property POutlookRibbons() As OutlookRibbons
    Friend Shared Property OutookApplication As Application
    Friend Shared Property PAnrufmonitor As Anrufmonitor
    Friend Shared Property PhoneBookXML As FritzBoxXMLTelefonbücher
    Friend Shared Property PVorwahlen As Vorwahlen
    Friend Shared Property TellowsScoreList As List(Of TellowsScoreListEntry)
    Friend Shared Property OffeneAnrMonWPF As List(Of AnrMonWPF)
    Friend Shared Property OffeneStoppUhrWPF As List(Of StoppUhrWPF)
    Friend Shared Property AddinWindows As New List(Of Windows.Window)
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property AnrMonWarAktiv As Boolean

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
            AddHandler Microsoft.Win32.SystemEvents.PowerModeChanged, AddressOf AnrMonRestartNachStandBy
            ' Starte die Funktionen des Addins
            StarteAddinFunktionen()
        Else
            NLogger.Warn("Addin nicht gestartet, da kein Explorer vorhanden")
        End If
        ' enable keyboard intercepts
        KeyboardHooking.SetHook()
    End Sub

    Private Async Sub StarteAddinFunktionen()
        NLogger.Info($"Starte {My.Resources.strDefLongName} {Reflection.Assembly.GetExecutingAssembly.GetName.Version}...")

        Dim TaskScoreListe As Task(Of List(Of TellowsScoreListEntry)) = Nothing
        Dim TaskTelefonbücher As Task(Of FritzBoxXMLTelefonbücher) = Nothing
        Dim TaskVorwahlen As Task(Of Kennzahlen) = Nothing
        Dim TaskAnrList As Task(Of FritzBoxXMLCallList) = Nothing

        ' Initialisiere die Landes- und Ortskennzahlen
        PVorwahlen = New Vorwahlen
        NLogger.Debug("Starte Einlesen der Landes- und Ortskennzahlen")
        TaskVorwahlen = PVorwahlen.LadeVorwahlen

        Dim UserData As New NutzerDaten
        NLogger.Debug("Nutzererinstellungen geladen...")

        ' Initiiere die TR064 Schnittstelle für die Abfragen der Daten der Fritz!Box
        Using FBoxTR064 = New SOAP.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)

            If FBoxTR064.Bereit Then
                ' Schreibe in das Log noch Informationen zur Fritz!Box
                NLogger.Info($"{FBoxTR064.FriendlyName} {FBoxTR064.DisplayVersion}")

                ' Lade die Anrufliste herunter
                If XMLData.POptionen.CBAutoAnrList Then
                    TaskAnrList = LadeFritzBoxAnrufliste(FBoxTR064)
                End If

                ' Lade alle Telefonbücher aus der Fritz!Box via Task herunter
                If XMLData.POptionen.CBKontaktSucheFritzBox Then
                    TaskTelefonbücher = Telefonbücher.LadeFritzBoxTelefonbücher(FBoxTR064)
                End If

                ' Tellows ScoreList laden
                If XMLData.POptionen.CBTellowsAutoUpdateScoreList Then
                    Using tellows As New Tellows
                        TaskScoreListe = tellows.LadeScoreList
                    End Using
                End If
            Else
                NLogger.Warn("TR064 Schnittstelle der Dritz!Box nicht verfügbar.")
            End If

            ' Anrufmonitor starten
            If XMLData.POptionen.CBAnrMonAuto Then
                PAnrufmonitor = New Anrufmonitor
                PAnrufmonitor.StartAnrMon()
                NLogger.Debug("Anrufmonitor gestartet...")
            End If

            ' Explorer Ereignishandler festlegen
            SetExplorer()
            NLogger.Debug("Outlook-Explorer Ereignishandler erfasst...")

            ' Outlook Inspektoren erfassen
            OutlookInspectors = Application.Inspectors
            NLogger.Debug("Outlook-Inspektor Ereignishandler erfasst...")

            ' Beendigung des Task für das Einlesen der Kennzahlen abwarten
            PVorwahlen.Kennzahlen = Await TaskVorwahlen
            NLogger.Debug($"Landes- und Ortskennzahlen {If(PVorwahlen.Kennzahlen Is Nothing, "nicht ", "")}geladen...")

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
        End Using

    End Sub


    Private Sub Application_Quit() Handles Application.Quit, Me.Shutdown

        ' Listen leeren
        If PVorwahlen IsNot Nothing Then PVorwahlen.Kennzahlen.Landeskennzahlen.Clear()
        ' Anrufmonitor beenden
        If PAnrufmonitor IsNot Nothing Then PAnrufmonitor.StoppAnrMon()
        ' Eintrag ins Log
        NLogger.Info($"{My.Resources.strDefLongName} {Reflection.Assembly.GetExecutingAssembly.GetName.Version} beendet.")
        ' XML-Datei Speichern
        XmlSerializeToFile(XMLData, IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, DfltConfigFileName))
        ' disable keyboard intercepts
        KeyboardHooking.ReleaseHook()

        OutookApplication = Nothing
    End Sub

#Region "Standby Wakeup"
    ''' <summary>
    ''' Startet den Anrufmonitor nach dem Aufwachen nach dem Standby neu, bzw. Beendet ihn, falls ein Standyby erkannt wird.
    ''' </summary>
    Private Sub AnrMonRestartNachStandBy(sender As Object, e As Microsoft.Win32.PowerModeChangedEventArgs)

        NLogger.Info($"PowerMode: {e.Mode} ({e.Mode})")

        Select Case e.Mode
            Case Microsoft.Win32.PowerModes.Resume
                ' Wiederherstelung nach dem Standby

                ' Starte den Anrufmonitor wenn er zuvor aktiv war, und er automatisch gestartet werden soll.
                If AnrMonWarAktiv And XMLData.POptionen.CBAnrMonAuto Then
                    ' Eintrag ins Log
                    NLogger.Info("Anrufmonitor nach Standby gestartet.")
                    ' Anrufmonitor erneut starten
                    PAnrufmonitor.Reaktivieren()
                End If

            Case Microsoft.Win32.PowerModes.Suspend
                ' Anrufmonitor beenden, falls dieser aktiv ist
                If PAnrufmonitor IsNot Nothing Then
                    ' Eintrag ins Log
                    NLogger.Info("Anrufmonitor für Standby angehalten.")
                    ' Merken, dass er aktiv war
                    AnrMonWarAktiv = PAnrufmonitor.Aktiv
                    ' Anrufmonitor anhalten
                    PAnrufmonitor.StoppAnrMon()
                End If

                ' XML-Datei speichern
                XmlSerializeToFile(XMLData, IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.AssemblyName, DfltConfigFileName))

        End Select
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

                    IndiziereKontakt(CType(ClipboardObject, ContactItem), Target)

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
            KontakInsepektorenListe.Add(New KontaktInspector() With {.Kontakt = CType(Inspector.CurrentItem, ContactItem)})
        End If
    End Sub

#End Region
End Class
