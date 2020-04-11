<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormCfg
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormCfg))
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.BReset = New System.Windows.Forms.Button()
        Me.BAbbruch = New System.Windows.Forms.Button()
        Me.BApply = New System.Windows.Forms.Button()
        Me.BOK = New System.Windows.Forms.Button()
        Me.ToolTipFBDBConfig = New System.Windows.Forms.ToolTip(Me.components)
        Me.CBShowMSN = New System.Windows.Forms.CheckBox()
        Me.LTelNrMaske = New System.Windows.Forms.Label()
        Me.TBTelNrMaske = New System.Windows.Forms.TextBox()
        Me.CBTelNrGruppieren = New System.Windows.Forms.CheckBox()
        Me.CBCheckMobil = New System.Windows.Forms.CheckBox()
        Me.CBForceDialLKZ = New System.Windows.Forms.CheckBox()
        Me.CBCbCunterbinden = New System.Windows.Forms.CheckBox()
        Me.LAmtsholung = New System.Windows.Forms.Label()
        Me.TBAmt = New System.Windows.Forms.TextBox()
        Me.BTestLogin = New System.Windows.Forms.Button()
        Me.LBenutzer = New System.Windows.Forms.Label()
        Me.CBForceFBAdr = New System.Windows.Forms.CheckBox()
        Me.TBNumEntryList = New System.Windows.Forms.TextBox()
        Me.LNumEntryList = New System.Windows.Forms.Label()
        Me.CBAutoAnrList = New System.Windows.Forms.CheckBox()
        Me.TBRWSTest = New System.Windows.Forms.TextBox()
        Me.CBRWSIndex = New System.Windows.Forms.CheckBox()
        Me.CBKErstellen = New System.Windows.Forms.CheckBox()
        Me.PGrundeinstellungen = New System.Windows.Forms.TabPage()
        Me.GBFormatierungTelefonnummern = New System.Windows.Forms.GroupBox()
        Me.CBIgnoTelNrFormat = New System.Windows.Forms.CheckBox()
        Me.CBintl = New System.Windows.Forms.CheckBox()
        Me.GBEinstellungWählhilfe = New System.Windows.Forms.GroupBox()
        Me.TBWClientEnblDauer = New System.Windows.Forms.TextBox()
        Me.LWClientEnblDauer = New System.Windows.Forms.Label()
        Me.CBCloseWClient = New System.Windows.Forms.CheckBox()
        Me.CBDialPort = New System.Windows.Forms.CheckBox()
        Me.GBErforderlicheAngaben = New System.Windows.Forms.GroupBox()
        Me.TBPasswort = New System.Windows.Forms.MaskedTextBox()
        Me.TBLandesKZ = New System.Windows.Forms.TextBox()
        Me.TBOrtsKZ = New System.Windows.Forms.TextBox()
        Me.TBBenutzer = New System.Windows.Forms.TextBox()
        Me.LLandeskennzahl = New System.Windows.Forms.Label()
        Me.LOrtskennzahl = New System.Windows.Forms.Label()
        Me.LPasswort = New System.Windows.Forms.Label()
        Me.TBFBAdr = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.PInfo = New System.Windows.Forms.TabPage()
        Me.BArbeitsverzeichnis = New System.Windows.Forms.Button()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.LVersion = New System.Windows.Forms.Label()
        Me.RichTextBox1 = New System.Windows.Forms.RichTextBox()
        Me.LinkHomepage = New System.Windows.Forms.LinkLabel()
        Me.LinkEmail = New System.Windows.Forms.LinkLabel()
        Me.LinkForum = New System.Windows.Forms.LinkLabel()
        Me.PTelefone = New System.Windows.Forms.TabPage()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.DGVTelList = New FBoxDial.FBoxDataGridView()
        Me.BTelefonliste = New System.Windows.Forms.Button()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.PAnrufmonitor = New System.Windows.Forms.TabPage()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.GBJournal = New System.Windows.Forms.GroupBox()
        Me.CBJournal = New System.Windows.Forms.CheckBox()
        Me.GBAnrListeMain = New System.Windows.Forms.GroupBox()
        Me.CBAnrListeShowAnrMon = New System.Windows.Forms.CheckBox()
        Me.CBAnrListeUpdateCallLists = New System.Windows.Forms.CheckBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.GBAnrMonMain = New System.Windows.Forms.GroupBox()
        Me.PanelAnrMon = New System.Windows.Forms.Panel()
        Me.CBAnrMonContactImage = New System.Windows.Forms.CheckBox()
        Me.CBAnrMonZeigeKontakt = New System.Windows.Forms.CheckBox()
        Me.CBAnrMonAuto = New System.Windows.Forms.CheckBox()
        Me.CBAnrMonCloseAtDISSCONNECT = New System.Windows.Forms.CheckBox()
        Me.CBAutoClose = New System.Windows.Forms.CheckBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TBEnblDauer = New System.Windows.Forms.TextBox()
        Me.CLBTelNr = New System.Windows.Forms.CheckedListBox()
        Me.LEnblDauer = New System.Windows.Forms.Label()
        Me.CBUseAnrMon = New System.Windows.Forms.CheckBox()
        Me.FBDB_MP = New System.Windows.Forms.TabControl()
        Me.PAnrMonSim = New System.Windows.Forms.TabPage()
        Me.GBoxAnrMonDISCONNECT = New System.Windows.Forms.GroupBox()
        Me.TBAnrMonSimDISCONNECTDauer = New System.Windows.Forms.TextBox()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.LAnrMonSimDISCONNECT = New System.Windows.Forms.Label()
        Me.TBAnrMonSimDISCONNECTID = New System.Windows.Forms.TextBox()
        Me.LAnrMonSimLabelDISCONNECT = New System.Windows.Forms.Label()
        Me.DTPAnrMonSimDISCONNECT = New System.Windows.Forms.DateTimePicker()
        Me.BAnrMonSimDISCONNECT = New System.Windows.Forms.Button()
        Me.GBoxAnrMonCONNECT = New System.Windows.Forms.GroupBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.CBoxAnrMonSimCONNECTNSTID = New System.Windows.Forms.ComboBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.TBAnrMonSimCONNECTAugTelNr = New System.Windows.Forms.TextBox()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.LAnrMonSimCONNECT = New System.Windows.Forms.Label()
        Me.TBAnrMonSimCONNECTID = New System.Windows.Forms.TextBox()
        Me.LAnrMonSimLabelCONNECT = New System.Windows.Forms.Label()
        Me.DTPAnrMonSimCONNECT = New System.Windows.Forms.DateTimePicker()
        Me.BAnrMonSimCONNECT = New System.Windows.Forms.Button()
        Me.GBoxAnrMonCALL = New System.Windows.Forms.GroupBox()
        Me.CBoxAnrMonSimCALLNSTID = New System.Windows.Forms.ComboBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.CBoxAnrMonSimCALLSIPID = New System.Windows.Forms.ComboBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.CBoxAnrMonSimCALLEigTelNr = New System.Windows.Forms.ComboBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TBAnrMonSimCALLAugTelNr = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.LAnrMonSimCALL = New System.Windows.Forms.Label()
        Me.TBAnrMonSimCALLID = New System.Windows.Forms.TextBox()
        Me.LAnrMonSimLabelCALL = New System.Windows.Forms.Label()
        Me.DTPAnrMonSimCALL = New System.Windows.Forms.DateTimePicker()
        Me.BAnrMonSimCALL = New System.Windows.Forms.Button()
        Me.GBoxAnrMonRING = New System.Windows.Forms.GroupBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.CBoxAnrMonSimRINGSIPID = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.CBoxAnrMonSimRINGEigTelNr = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TBAnrMonSimRINGAugTelNr = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.LAnrMonSimRING = New System.Windows.Forms.Label()
        Me.TBAnrMonSimRINGID = New System.Windows.Forms.TextBox()
        Me.LAnrMonSimLabelRING = New System.Windows.Forms.Label()
        Me.DTPAnrMonSimRING = New System.Windows.Forms.DateTimePicker()
        Me.BAnrMonSimRING = New System.Windows.Forms.Button()
        Me.PKontaktsuche2 = New System.Windows.Forms.TabPage()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GBRWS = New System.Windows.Forms.GroupBox()
        Me.BRWSTest = New System.Windows.Forms.Button()
        Me.CBRWS = New System.Windows.Forms.CheckBox()
        Me.GBIndizierung = New System.Windows.Forms.GroupBox()
        Me.LabelAnzahl = New System.Windows.Forms.Label()
        Me.RadioButtonEntfernen = New System.Windows.Forms.RadioButton()
        Me.RadioButtonErstelle = New System.Windows.Forms.RadioButton()
        Me.BIndizierungAbbrechen = New System.Windows.Forms.Button()
        Me.BIndizierungStart = New System.Windows.Forms.Button()
        Me.ProgressBarIndex = New System.Windows.Forms.ProgressBar()
        Me.CBKontaktSucheFritzBox = New System.Windows.Forms.CheckBox()
        Me.BKontOrdLaden = New System.Windows.Forms.Button()
        Me.TreeViewKontakte = New System.Windows.Forms.TreeView()
        Me.PLogging = New System.Windows.Forms.TabPage()
        Me.LMinLogLevel = New System.Windows.Forms.Label()
        Me.CBoxMinLogLevel = New System.Windows.Forms.ComboBox()
        Me.GBLogging = New System.Windows.Forms.GroupBox()
        Me.LinkLogFile = New System.Windows.Forms.LinkLabel()
        Me.TBLogging = New System.Windows.Forms.TextBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.BXML = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.PGrundeinstellungen.SuspendLayout()
        Me.GBFormatierungTelefonnummern.SuspendLayout()
        Me.GBEinstellungWählhilfe.SuspendLayout()
        Me.GBErforderlicheAngaben.SuspendLayout()
        Me.PInfo.SuspendLayout()
        Me.PTelefone.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.DGVTelList, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PAnrufmonitor.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.GBJournal.SuspendLayout()
        Me.GBAnrListeMain.SuspendLayout()
        Me.GBAnrMonMain.SuspendLayout()
        Me.PanelAnrMon.SuspendLayout()
        Me.FBDB_MP.SuspendLayout()
        Me.PAnrMonSim.SuspendLayout()
        Me.GBoxAnrMonDISCONNECT.SuspendLayout()
        Me.GBoxAnrMonCONNECT.SuspendLayout()
        Me.GBoxAnrMonCALL.SuspendLayout()
        Me.GBoxAnrMonRING.SuspendLayout()
        Me.PKontaktsuche2.SuspendLayout()
        Me.GBRWS.SuspendLayout()
        Me.GBIndizierung.SuspendLayout()
        Me.PLogging.SuspendLayout()
        Me.GBLogging.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'BReset
        '
        Me.BReset.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BReset.Location = New System.Drawing.Point(523, 5)
        Me.BReset.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BReset.Name = "BReset"
        Me.BReset.Size = New System.Drawing.Size(165, 43)
        Me.BReset.TabIndex = 4
        Me.BReset.Text = "Zurücksetzen"
        Me.BReset.UseVisualStyleBackColor = True
        '
        'BAbbruch
        '
        Me.BAbbruch.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BAbbruch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BAbbruch.Location = New System.Drawing.Point(350, 5)
        Me.BAbbruch.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BAbbruch.Name = "BAbbruch"
        Me.BAbbruch.Size = New System.Drawing.Size(165, 43)
        Me.BAbbruch.TabIndex = 3
        Me.BAbbruch.Text = "Abbruch"
        Me.BAbbruch.UseVisualStyleBackColor = True
        '
        'BApply
        '
        Me.BApply.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BApply.Location = New System.Drawing.Point(177, 5)
        Me.BApply.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BApply.Name = "BApply"
        Me.BApply.Size = New System.Drawing.Size(165, 43)
        Me.BApply.TabIndex = 2
        Me.BApply.Text = "Übernehmen"
        Me.BApply.UseVisualStyleBackColor = True
        '
        'BOK
        '
        Me.BOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BOK.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BOK.Location = New System.Drawing.Point(4, 5)
        Me.BOK.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BOK.Name = "BOK"
        Me.BOK.Size = New System.Drawing.Size(165, 43)
        Me.BOK.TabIndex = 1
        Me.BOK.Text = "OK"
        Me.BOK.UseVisualStyleBackColor = True
        '
        'ToolTipFBDBConfig
        '
        Me.ToolTipFBDBConfig.AutoPopDelay = 10000
        Me.ToolTipFBDBConfig.InitialDelay = 500
        Me.ToolTipFBDBConfig.ReshowDelay = 100
        Me.ToolTipFBDBConfig.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info
        Me.ToolTipFBDBConfig.ToolTipTitle = "Erläuterung:"
        '
        'CBShowMSN
        '
        Me.CBShowMSN.AutoSize = True
        Me.CBShowMSN.Enabled = False
        Me.CBShowMSN.Location = New System.Drawing.Point(4, 303)
        Me.CBShowMSN.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBShowMSN.MinimumSize = New System.Drawing.Size(400, 0)
        Me.CBShowMSN.Name = "CBShowMSN"
        Me.CBShowMSN.Size = New System.Drawing.Size(400, 24)
        Me.CBShowMSN.TabIndex = 9
        Me.CBShowMSN.Text = "Zeige MSN im Anrufmonitor an"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBShowMSN, "Wenn diese Einstellung gesetzt ist, wird die jeweilige MSN im Anrufmonitor angeze" &
        "igt.")
        Me.CBShowMSN.UseVisualStyleBackColor = True
        '
        'LTelNrMaske
        '
        Me.LTelNrMaske.AutoSize = True
        Me.LTelNrMaske.Location = New System.Drawing.Point(135, 31)
        Me.LTelNrMaske.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LTelNrMaske.MinimumSize = New System.Drawing.Size(65, 0)
        Me.LTelNrMaske.Name = "LTelNrMaske"
        Me.LTelNrMaske.Size = New System.Drawing.Size(65, 20)
        Me.LTelNrMaske.TabIndex = 3
        Me.LTelNrMaske.Text = "Maske:"
        Me.ToolTipFBDBConfig.SetToolTip(Me.LTelNrMaske, resources.GetString("LTelNrMaske.ToolTip"))
        '
        'TBTelNrMaske
        '
        Me.TBTelNrMaske.Location = New System.Drawing.Point(207, 26)
        Me.TBTelNrMaske.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBTelNrMaske.Name = "TBTelNrMaske"
        Me.TBTelNrMaske.Size = New System.Drawing.Size(146, 26)
        Me.TBTelNrMaske.TabIndex = 13
        Me.ToolTipFBDBConfig.SetToolTip(Me.TBTelNrMaske, resources.GetString("TBTelNrMaske.ToolTip"))
        '
        'CBTelNrGruppieren
        '
        Me.CBTelNrGruppieren.AutoSize = True
        Me.CBTelNrGruppieren.Location = New System.Drawing.Point(9, 29)
        Me.CBTelNrGruppieren.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBTelNrGruppieren.MinimumSize = New System.Drawing.Size(120, 0)
        Me.CBTelNrGruppieren.Name = "CBTelNrGruppieren"
        Me.CBTelNrGruppieren.Size = New System.Drawing.Size(120, 24)
        Me.CBTelNrGruppieren.TabIndex = 12
        Me.CBTelNrGruppieren.Text = "Gruppieren"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBTelNrGruppieren, "Gruppiert Rufnummernteile in Zweierblöcke für bessere Lessbarkeit." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Beispiel:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "oh" &
        "ne Gruppierung: +49 (123) 4567890 " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "mit Gruppierung: +49 (1 23) 4 56 78 90 ")
        Me.CBTelNrGruppieren.UseVisualStyleBackColor = True
        '
        'CBCheckMobil
        '
        Me.CBCheckMobil.AutoSize = True
        Me.CBCheckMobil.Location = New System.Drawing.Point(9, 187)
        Me.CBCheckMobil.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBCheckMobil.MinimumSize = New System.Drawing.Size(400, 0)
        Me.CBCheckMobil.Name = "CBCheckMobil"
        Me.CBCheckMobil.Size = New System.Drawing.Size(400, 24)
        Me.CBCheckMobil.TabIndex = 11
        Me.CBCheckMobil.Text = "Nachfrage beim Wählen von Mobilnummern"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBCheckMobil, "Um unnötige Verbindungskosten zu Mobilfunkgeräten zu vermeiden, wird vor dem Wähl" &
        "en eine zusätzliche Benutzereingabe erforderlich.")
        Me.CBCheckMobil.UseVisualStyleBackColor = True
        '
        'CBForceDialLKZ
        '
        Me.CBForceDialLKZ.AutoSize = True
        Me.CBForceDialLKZ.Location = New System.Drawing.Point(9, 34)
        Me.CBForceDialLKZ.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBForceDialLKZ.MinimumSize = New System.Drawing.Size(400, 0)
        Me.CBForceDialLKZ.Name = "CBForceDialLKZ"
        Me.CBForceDialLKZ.Size = New System.Drawing.Size(400, 24)
        Me.CBForceDialLKZ.TabIndex = 6
        Me.CBForceDialLKZ.Text = "Landeskennzahl immer mitwählen"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBForceDialLKZ, "Mit dieser Einstellung wird die definierte Landesvorwahl immer mitgewählt.")
        Me.CBForceDialLKZ.UseVisualStyleBackColor = True
        '
        'CBCbCunterbinden
        '
        Me.CBCbCunterbinden.AutoSize = True
        Me.CBCbCunterbinden.Enabled = False
        Me.CBCbCunterbinden.Location = New System.Drawing.Point(9, 151)
        Me.CBCbCunterbinden.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBCbCunterbinden.MinimumSize = New System.Drawing.Size(205, 0)
        Me.CBCbCunterbinden.Name = "CBCbCunterbinden"
        Me.CBCbCunterbinden.Size = New System.Drawing.Size(205, 24)
        Me.CBCbCunterbinden.TabIndex = 9
        Me.CBCbCunterbinden.Text = "Call-by-Call unterbinden"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBCbCunterbinden, "Mitunter ist es sinnvoll Call-by-Call Vorwahlen zu unterbinden, z.B. wenn Sie " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "k" &
        "einen Festnetzanschluss haben und nur über Ihren Internetanbieter telefonieren.")
        Me.CBCbCunterbinden.UseVisualStyleBackColor = True
        '
        'LAmtsholung
        '
        Me.LAmtsholung.AutoSize = True
        Me.LAmtsholung.Location = New System.Drawing.Point(61, 75)
        Me.LAmtsholung.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LAmtsholung.MinimumSize = New System.Drawing.Size(100, 0)
        Me.LAmtsholung.Name = "LAmtsholung"
        Me.LAmtsholung.Size = New System.Drawing.Size(100, 20)
        Me.LAmtsholung.TabIndex = 31
        Me.LAmtsholung.Text = "Amtsholung"
        Me.ToolTipFBDBConfig.SetToolTip(Me.LAmtsholung, "Geben Sie hier eine 0 ein falls eine Amtsholung benötigt wird.")
        '
        'TBAmt
        '
        Me.TBAmt.Location = New System.Drawing.Point(9, 72)
        Me.TBAmt.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBAmt.Name = "TBAmt"
        Me.TBAmt.Size = New System.Drawing.Size(43, 26)
        Me.TBAmt.TabIndex = 7
        Me.ToolTipFBDBConfig.SetToolTip(Me.TBAmt, "Geben Sie hier eine 0 ein falls eine Amtsholung benötigt wird.")
        '
        'BTestLogin
        '
        Me.BTestLogin.Location = New System.Drawing.Point(344, 109)
        Me.BTestLogin.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BTestLogin.Name = "BTestLogin"
        Me.BTestLogin.Size = New System.Drawing.Size(70, 35)
        Me.BTestLogin.TabIndex = 32
        Me.BTestLogin.Text = "Test"
        Me.ToolTipFBDBConfig.SetToolTip(Me.BTestLogin, "Teste den eingegebenen Benutzername und Passwort.")
        Me.BTestLogin.UseVisualStyleBackColor = True
        Me.BTestLogin.Visible = False
        '
        'LBenutzer
        '
        Me.LBenutzer.AutoSize = True
        Me.LBenutzer.Location = New System.Drawing.Point(170, 75)
        Me.LBenutzer.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LBenutzer.MinimumSize = New System.Drawing.Size(230, 0)
        Me.LBenutzer.Name = "LBenutzer"
        Me.LBenutzer.Size = New System.Drawing.Size(230, 20)
        Me.LBenutzer.TabIndex = 16
        Me.LBenutzer.Text = "Fritz!Box Benutzername"
        Me.ToolTipFBDBConfig.SetToolTip(Me.LBenutzer, resources.GetString("LBenutzer.ToolTip"))
        '
        'CBForceFBAdr
        '
        Me.CBForceFBAdr.AutoSize = True
        Me.CBForceFBAdr.Enabled = False
        Me.CBForceFBAdr.Location = New System.Drawing.Point(174, 34)
        Me.CBForceFBAdr.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBForceFBAdr.MinimumSize = New System.Drawing.Size(230, 0)
        Me.CBForceFBAdr.Name = "CBForceFBAdr"
        Me.CBForceFBAdr.Size = New System.Drawing.Size(230, 24)
        Me.CBForceFBAdr.TabIndex = 1
        Me.CBForceFBAdr.Text = "Fritz!Box Adresse"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBForceFBAdr, "Wenn der Haken gesetzt wird, wird trotz fehlgeschlagener Ping-Check eine Verbindu" &
        "ng zur eingegebenen Addresse aufgebaut." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Das ist z.B. bei einigen dyndns-Anbiete" &
        "rn nötig, da diese Pings blockieren.")
        Me.CBForceFBAdr.UseVisualStyleBackColor = True
        '
        'TBNumEntryList
        '
        Me.TBNumEntryList.Location = New System.Drawing.Point(9, 28)
        Me.TBNumEntryList.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBNumEntryList.Name = "TBNumEntryList"
        Me.TBNumEntryList.Size = New System.Drawing.Size(42, 26)
        Me.TBNumEntryList.TabIndex = 39
        Me.ToolTipFBDBConfig.SetToolTip(Me.TBNumEntryList, "Gibt die Anzahl der Listenelemente an, die in der Wahlwiederholungs und Rückrufli" &
        "ste aufgeführt werden." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Eine Reduktion des Wertes bedeutet automatisch, dass die" &
        " Listen geleert werden.")
        '
        'LNumEntryList
        '
        Me.LNumEntryList.AutoSize = True
        Me.LNumEntryList.Location = New System.Drawing.Point(62, 32)
        Me.LNumEntryList.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LNumEntryList.MinimumSize = New System.Drawing.Size(300, 0)
        Me.LNumEntryList.Name = "LNumEntryList"
        Me.LNumEntryList.Size = New System.Drawing.Size(300, 20)
        Me.LNumEntryList.TabIndex = 40
        Me.LNumEntryList.Text = "Anzahl der Listenelemente je Liste"
        Me.ToolTipFBDBConfig.SetToolTip(Me.LNumEntryList, "Telefonate, die nach der definierten Zeitspanne verbunden werden, wenden als ""Ver" &
        "passt"" behandelt." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Nach der Zeitspanne erfolgt z. B. die Verbindung zum Anrufbea" &
        "ntworter.")
        '
        'CBAutoAnrList
        '
        Me.CBAutoAnrList.AutoSize = True
        Me.CBAutoAnrList.Location = New System.Drawing.Point(4, 29)
        Me.CBAutoAnrList.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAutoAnrList.MinimumSize = New System.Drawing.Size(400, 0)
        Me.CBAutoAnrList.Name = "CBAutoAnrList"
        Me.CBAutoAnrList.Size = New System.Drawing.Size(400, 24)
        Me.CBAutoAnrList.TabIndex = 8
        Me.CBAutoAnrList.Text = "Anrufliste beim Start auswerten"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBAutoAnrList, resources.GetString("CBAutoAnrList.ToolTip"))
        Me.CBAutoAnrList.UseVisualStyleBackColor = True
        '
        'TBRWSTest
        '
        Me.TBRWSTest.Location = New System.Drawing.Point(285, 27)
        Me.TBRWSTest.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBRWSTest.Name = "TBRWSTest"
        Me.TBRWSTest.Size = New System.Drawing.Size(168, 26)
        Me.TBRWSTest.TabIndex = 6
        Me.ToolTipFBDBConfig.SetToolTip(Me.TBRWSTest, "Geben Sie hier eine gültige Telefonnummer ein, nach der eine Rückwärtssuche durch" &
        "geführt werden soll.")
        '
        'CBRWSIndex
        '
        Me.CBRWSIndex.AutoSize = True
        Me.CBRWSIndex.Enabled = False
        Me.CBRWSIndex.Location = New System.Drawing.Point(7, 97)
        Me.CBRWSIndex.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBRWSIndex.Name = "CBRWSIndex"
        Me.CBRWSIndex.Size = New System.Drawing.Size(155, 24)
        Me.CBRWSIndex.TabIndex = 3
        Me.CBRWSIndex.Text = "Ergebnis merken"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBRWSIndex, resources.GetString("CBRWSIndex.ToolTip"))
        Me.CBRWSIndex.UseVisualStyleBackColor = True
        '
        'CBKErstellen
        '
        Me.CBKErstellen.AutoSize = True
        Me.CBKErstellen.Enabled = False
        Me.CBKErstellen.Location = New System.Drawing.Point(7, 63)
        Me.CBKErstellen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBKErstellen.Name = "CBKErstellen"
        Me.CBKErstellen.Size = New System.Drawing.Size(154, 24)
        Me.CBKErstellen.TabIndex = 2
        Me.CBKErstellen.Text = "Kontakt erstellen"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBKErstellen, "Nach erfolgreicher Rückwärtssuche, wird bei dieser Einstellung ein neuer Kontakt " &
        "erstellt.")
        Me.CBKErstellen.UseVisualStyleBackColor = True
        '
        'PGrundeinstellungen
        '
        Me.PGrundeinstellungen.Controls.Add(Me.GBFormatierungTelefonnummern)
        Me.PGrundeinstellungen.Controls.Add(Me.GBEinstellungWählhilfe)
        Me.PGrundeinstellungen.Controls.Add(Me.GBErforderlicheAngaben)
        Me.PGrundeinstellungen.Controls.Add(Me.Label13)
        Me.PGrundeinstellungen.Location = New System.Drawing.Point(4, 29)
        Me.PGrundeinstellungen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PGrundeinstellungen.Name = "PGrundeinstellungen"
        Me.PGrundeinstellungen.Size = New System.Drawing.Size(860, 458)
        Me.PGrundeinstellungen.TabIndex = 7
        Me.PGrundeinstellungen.Text = "Grundeinstellungen"
        Me.PGrundeinstellungen.UseVisualStyleBackColor = True
        '
        'GBFormatierungTelefonnummern
        '
        Me.GBFormatierungTelefonnummern.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GBFormatierungTelefonnummern.Controls.Add(Me.CBIgnoTelNrFormat)
        Me.GBFormatierungTelefonnummern.Controls.Add(Me.LTelNrMaske)
        Me.GBFormatierungTelefonnummern.Controls.Add(Me.TBTelNrMaske)
        Me.GBFormatierungTelefonnummern.Controls.Add(Me.CBintl)
        Me.GBFormatierungTelefonnummern.Controls.Add(Me.CBTelNrGruppieren)
        Me.GBFormatierungTelefonnummern.Location = New System.Drawing.Point(0, 299)
        Me.GBFormatierungTelefonnummern.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBFormatierungTelefonnummern.Name = "GBFormatierungTelefonnummern"
        Me.GBFormatierungTelefonnummern.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBFormatierungTelefonnummern.Size = New System.Drawing.Size(423, 152)
        Me.GBFormatierungTelefonnummern.TabIndex = 17
        Me.GBFormatierungTelefonnummern.TabStop = False
        Me.GBFormatierungTelefonnummern.Text = "Formatierung von Telefonnummern"
        '
        'CBIgnoTelNrFormat
        '
        Me.CBIgnoTelNrFormat.AutoSize = True
        Me.CBIgnoTelNrFormat.Enabled = False
        Me.CBIgnoTelNrFormat.Location = New System.Drawing.Point(9, 100)
        Me.CBIgnoTelNrFormat.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBIgnoTelNrFormat.MinimumSize = New System.Drawing.Size(400, 0)
        Me.CBIgnoTelNrFormat.Name = "CBIgnoTelNrFormat"
        Me.CBIgnoTelNrFormat.Size = New System.Drawing.Size(400, 24)
        Me.CBIgnoTelNrFormat.TabIndex = 15
        Me.CBIgnoTelNrFormat.Text = "Ignoriere Formatierung der Kontakte"
        Me.CBIgnoTelNrFormat.UseVisualStyleBackColor = True
        '
        'CBintl
        '
        Me.CBintl.AutoSize = True
        Me.CBintl.Location = New System.Drawing.Point(9, 65)
        Me.CBintl.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBintl.MinimumSize = New System.Drawing.Size(400, 0)
        Me.CBintl.Name = "CBintl"
        Me.CBintl.Size = New System.Drawing.Size(400, 24)
        Me.CBintl.TabIndex = 14
        Me.CBintl.Text = "Internationale Vorwahl immer anfügen"
        Me.CBintl.UseVisualStyleBackColor = True
        '
        'GBEinstellungWählhilfe
        '
        Me.GBEinstellungWählhilfe.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GBEinstellungWählhilfe.Controls.Add(Me.TBWClientEnblDauer)
        Me.GBEinstellungWählhilfe.Controls.Add(Me.LWClientEnblDauer)
        Me.GBEinstellungWählhilfe.Controls.Add(Me.CBCloseWClient)
        Me.GBEinstellungWählhilfe.Controls.Add(Me.CBDialPort)
        Me.GBEinstellungWählhilfe.Controls.Add(Me.CBCheckMobil)
        Me.GBEinstellungWählhilfe.Controls.Add(Me.CBForceDialLKZ)
        Me.GBEinstellungWählhilfe.Controls.Add(Me.CBCbCunterbinden)
        Me.GBEinstellungWählhilfe.Controls.Add(Me.LAmtsholung)
        Me.GBEinstellungWählhilfe.Controls.Add(Me.TBAmt)
        Me.GBEinstellungWählhilfe.Location = New System.Drawing.Point(433, 58)
        Me.GBEinstellungWählhilfe.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBEinstellungWählhilfe.Name = "GBEinstellungWählhilfe"
        Me.GBEinstellungWählhilfe.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBEinstellungWählhilfe.Size = New System.Drawing.Size(423, 316)
        Me.GBEinstellungWählhilfe.TabIndex = 18
        Me.GBEinstellungWählhilfe.TabStop = False
        Me.GBEinstellungWählhilfe.Text = "Einstellungen für die Wählhilfe"
        '
        'TBWClientEnblDauer
        '
        Me.TBWClientEnblDauer.Location = New System.Drawing.Point(8, 262)
        Me.TBWClientEnblDauer.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBWClientEnblDauer.Name = "TBWClientEnblDauer"
        Me.TBWClientEnblDauer.Size = New System.Drawing.Size(42, 26)
        Me.TBWClientEnblDauer.TabIndex = 33
        '
        'LWClientEnblDauer
        '
        Me.LWClientEnblDauer.AutoSize = True
        Me.LWClientEnblDauer.Location = New System.Drawing.Point(61, 265)
        Me.LWClientEnblDauer.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LWClientEnblDauer.MinimumSize = New System.Drawing.Size(350, 0)
        Me.LWClientEnblDauer.Name = "LWClientEnblDauer"
        Me.LWClientEnblDauer.Size = New System.Drawing.Size(350, 20)
        Me.LWClientEnblDauer.TabIndex = 34
        Me.LWClientEnblDauer.Text = "Anzeigedauer nach dem Wählen [s]"
        '
        'CBCloseWClient
        '
        Me.CBCloseWClient.AutoSize = True
        Me.CBCloseWClient.Location = New System.Drawing.Point(9, 223)
        Me.CBCloseWClient.Name = "CBCloseWClient"
        Me.CBCloseWClient.Size = New System.Drawing.Size(291, 24)
        Me.CBCloseWClient.TabIndex = 32
        Me.CBCloseWClient.Text = "Wähldialog automatisch ausblenden"
        Me.CBCloseWClient.UseVisualStyleBackColor = True
        '
        'CBDialPort
        '
        Me.CBDialPort.AutoSize = True
        Me.CBDialPort.Enabled = False
        Me.CBDialPort.Location = New System.Drawing.Point(9, 115)
        Me.CBDialPort.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBDialPort.MinimumSize = New System.Drawing.Size(200, 0)
        Me.CBDialPort.Name = "CBDialPort"
        Me.CBDialPort.Size = New System.Drawing.Size(200, 24)
        Me.CBDialPort.TabIndex = 8
        Me.CBDialPort.Text = "Dialport anzeigen"
        Me.CBDialPort.UseVisualStyleBackColor = True
        '
        'GBErforderlicheAngaben
        '
        Me.GBErforderlicheAngaben.Controls.Add(Me.BTestLogin)
        Me.GBErforderlicheAngaben.Controls.Add(Me.TBPasswort)
        Me.GBErforderlicheAngaben.Controls.Add(Me.LBenutzer)
        Me.GBErforderlicheAngaben.Controls.Add(Me.TBLandesKZ)
        Me.GBErforderlicheAngaben.Controls.Add(Me.TBOrtsKZ)
        Me.GBErforderlicheAngaben.Controls.Add(Me.TBBenutzer)
        Me.GBErforderlicheAngaben.Controls.Add(Me.CBForceFBAdr)
        Me.GBErforderlicheAngaben.Controls.Add(Me.LLandeskennzahl)
        Me.GBErforderlicheAngaben.Controls.Add(Me.LOrtskennzahl)
        Me.GBErforderlicheAngaben.Controls.Add(Me.LPasswort)
        Me.GBErforderlicheAngaben.Controls.Add(Me.TBFBAdr)
        Me.GBErforderlicheAngaben.Location = New System.Drawing.Point(0, 58)
        Me.GBErforderlicheAngaben.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBErforderlicheAngaben.Name = "GBErforderlicheAngaben"
        Me.GBErforderlicheAngaben.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBErforderlicheAngaben.Size = New System.Drawing.Size(423, 232)
        Me.GBErforderlicheAngaben.TabIndex = 16
        Me.GBErforderlicheAngaben.TabStop = False
        Me.GBErforderlicheAngaben.Text = "Erforderliche Angaben"
        '
        'TBPasswort
        '
        Me.TBPasswort.Location = New System.Drawing.Point(9, 109)
        Me.TBPasswort.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBPasswort.Name = "TBPasswort"
        Me.TBPasswort.Size = New System.Drawing.Size(148, 26)
        Me.TBPasswort.TabIndex = 17
        Me.TBPasswort.UseSystemPasswordChar = True
        '
        'TBLandesKZ
        '
        Me.TBLandesKZ.Location = New System.Drawing.Point(9, 189)
        Me.TBLandesKZ.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBLandesKZ.Name = "TBLandesKZ"
        Me.TBLandesKZ.Size = New System.Drawing.Size(148, 26)
        Me.TBLandesKZ.TabIndex = 2
        '
        'TBOrtsKZ
        '
        Me.TBOrtsKZ.Location = New System.Drawing.Point(9, 149)
        Me.TBOrtsKZ.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBOrtsKZ.Name = "TBOrtsKZ"
        Me.TBOrtsKZ.Size = New System.Drawing.Size(148, 26)
        Me.TBOrtsKZ.TabIndex = 2
        '
        'TBBenutzer
        '
        Me.TBBenutzer.Location = New System.Drawing.Point(9, 71)
        Me.TBBenutzer.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBBenutzer.Name = "TBBenutzer"
        Me.TBBenutzer.Size = New System.Drawing.Size(148, 26)
        Me.TBBenutzer.TabIndex = 2
        '
        'LLandeskennzahl
        '
        Me.LLandeskennzahl.AutoSize = True
        Me.LLandeskennzahl.Location = New System.Drawing.Point(170, 192)
        Me.LLandeskennzahl.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LLandeskennzahl.MinimumSize = New System.Drawing.Size(230, 0)
        Me.LLandeskennzahl.Name = "LLandeskennzahl"
        Me.LLandeskennzahl.Size = New System.Drawing.Size(230, 20)
        Me.LLandeskennzahl.TabIndex = 13
        Me.LLandeskennzahl.Text = "Landeskennzahl"
        '
        'LOrtskennzahl
        '
        Me.LOrtskennzahl.AutoSize = True
        Me.LOrtskennzahl.Location = New System.Drawing.Point(170, 154)
        Me.LOrtskennzahl.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LOrtskennzahl.MinimumSize = New System.Drawing.Size(230, 0)
        Me.LOrtskennzahl.Name = "LOrtskennzahl"
        Me.LOrtskennzahl.Size = New System.Drawing.Size(230, 20)
        Me.LOrtskennzahl.TabIndex = 11
        Me.LOrtskennzahl.Text = "Ortskennzahl"
        '
        'LPasswort
        '
        Me.LPasswort.AutoSize = True
        Me.LPasswort.Location = New System.Drawing.Point(170, 115)
        Me.LPasswort.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LPasswort.MinimumSize = New System.Drawing.Size(160, 0)
        Me.LPasswort.Name = "LPasswort"
        Me.LPasswort.Size = New System.Drawing.Size(160, 20)
        Me.LPasswort.TabIndex = 3
        Me.LPasswort.Text = "Fritz!Box Passwort"
        '
        'TBFBAdr
        '
        Me.TBFBAdr.Location = New System.Drawing.Point(9, 31)
        Me.TBFBAdr.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBFBAdr.Name = "TBFBAdr"
        Me.TBFBAdr.Size = New System.Drawing.Size(148, 26)
        Me.TBFBAdr.TabIndex = 0
        '
        'Label13
        '
        Me.Label13.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(0, 0)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(860, 54)
        Me.Label13.TabIndex = 23
        Me.Label13.Text = "Grundeinstellungen"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PInfo
        '
        Me.PInfo.Controls.Add(Me.BArbeitsverzeichnis)
        Me.PInfo.Controls.Add(Me.Label17)
        Me.PInfo.Controls.Add(Me.Label16)
        Me.PInfo.Controls.Add(Me.Label10)
        Me.PInfo.Controls.Add(Me.LVersion)
        Me.PInfo.Controls.Add(Me.RichTextBox1)
        Me.PInfo.Controls.Add(Me.LinkHomepage)
        Me.PInfo.Controls.Add(Me.LinkEmail)
        Me.PInfo.Controls.Add(Me.LinkForum)
        Me.PInfo.Location = New System.Drawing.Point(4, 29)
        Me.PInfo.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PInfo.Name = "PInfo"
        Me.PInfo.Size = New System.Drawing.Size(860, 458)
        Me.PInfo.TabIndex = 4
        Me.PInfo.Text = "Info..."
        Me.PInfo.UseVisualStyleBackColor = True
        '
        'BArbeitsverzeichnis
        '
        Me.BArbeitsverzeichnis.Location = New System.Drawing.Point(615, 89)
        Me.BArbeitsverzeichnis.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BArbeitsverzeichnis.Name = "BArbeitsverzeichnis"
        Me.BArbeitsverzeichnis.Size = New System.Drawing.Size(232, 43)
        Me.BArbeitsverzeichnis.TabIndex = 6
        Me.BArbeitsverzeichnis.Text = "Arbeitsverzeichnis ändern"
        Me.BArbeitsverzeichnis.UseVisualStyleBackColor = True
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(4, 102)
        Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(482, 20)
        Me.Label17.TabIndex = 0
        Me.Label17.Text = "Der SourceCode zu diesem AddIn steht auf GitHub zur Verfügung:"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(4, 77)
        Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(225, 20)
        Me.Label16.TabIndex = 2
        Me.Label16.Text = "Forum und aktuelle Versionen:"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(4, 52)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(68, 20)
        Me.Label10.TabIndex = 2
        Me.Label10.Text = "Kontakt:"
        '
        'LVersion
        '
        Me.LVersion.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LVersion.Location = New System.Drawing.Point(8, 14)
        Me.LVersion.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LVersion.Name = "LVersion"
        Me.LVersion.Size = New System.Drawing.Size(441, 25)
        Me.LVersion.TabIndex = 1
        Me.LVersion.Text = "Fritz!Box Telefon-Dingsbums "
        '
        'RichTextBox1
        '
        Me.RichTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.RichTextBox1.Location = New System.Drawing.Point(4, 146)
        Me.RichTextBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RichTextBox1.Name = "RichTextBox1"
        Me.RichTextBox1.Size = New System.Drawing.Size(841, 289)
        Me.RichTextBox1.TabIndex = 0
        Me.RichTextBox1.Text = resources.GetString("RichTextBox1.Text")
        '
        'LinkHomepage
        '
        Me.LinkHomepage.Location = New System.Drawing.Point(360, 102)
        Me.LinkHomepage.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LinkHomepage.Name = "LinkHomepage"
        Me.LinkHomepage.Size = New System.Drawing.Size(225, 20)
        Me.LinkHomepage.TabIndex = 5
        Me.LinkHomepage.TabStop = True
        Me.LinkHomepage.Text = "GitHub"
        Me.LinkHomepage.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'LinkEmail
        '
        Me.LinkEmail.Location = New System.Drawing.Point(360, 52)
        Me.LinkEmail.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LinkEmail.Name = "LinkEmail"
        Me.LinkEmail.Size = New System.Drawing.Size(225, 20)
        Me.LinkEmail.TabIndex = 1
        Me.LinkEmail.TabStop = True
        Me.LinkEmail.Text = "kruemelino@gert-michael.de"
        Me.LinkEmail.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'LinkForum
        '
        Me.LinkForum.Location = New System.Drawing.Point(360, 77)
        Me.LinkForum.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LinkForum.Name = "LinkForum"
        Me.LinkForum.Size = New System.Drawing.Size(225, 20)
        Me.LinkForum.TabIndex = 2
        Me.LinkForum.TabStop = True
        Me.LinkForum.Text = "www.ip-phone-forum.de"
        Me.LinkForum.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'PTelefone
        '
        Me.PTelefone.Controls.Add(Me.GroupBox1)
        Me.PTelefone.Controls.Add(Me.Label15)
        Me.PTelefone.Location = New System.Drawing.Point(4, 29)
        Me.PTelefone.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PTelefone.Name = "PTelefone"
        Me.PTelefone.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PTelefone.Size = New System.Drawing.Size(860, 458)
        Me.PTelefone.TabIndex = 5
        Me.PTelefone.Text = "Telefone"
        Me.PTelefone.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.DGVTelList)
        Me.GroupBox1.Controls.Add(Me.BTelefonliste)
        Me.GroupBox1.Location = New System.Drawing.Point(0, 63)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Size = New System.Drawing.Size(855, 389)
        Me.GroupBox1.TabIndex = 27
        Me.GroupBox1.TabStop = False
        '
        'DGVTelList
        '
        Me.DGVTelList.AllowUserToAddRows = False
        Me.DGVTelList.AllowUserToDeleteRows = False
        Me.DGVTelList.AllowUserToResizeColumns = False
        Me.DGVTelList.AllowUserToResizeRows = False
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DGVTelList.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.DGVTelList.ColumnHeadersHeight = 34
        Me.DGVTelList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.DGVTelList.Dock = System.Windows.Forms.DockStyle.Top
        Me.DGVTelList.Location = New System.Drawing.Point(4, 24)
        Me.DGVTelList.MultiSelect = False
        Me.DGVTelList.Name = "DGVTelList"
        Me.DGVTelList.RowHeadersVisible = False
        Me.DGVTelList.RowHeadersWidth = 62
        Me.DGVTelList.RowTemplate.Height = 28
        Me.DGVTelList.ShowEditingIcon = False
        Me.DGVTelList.Size = New System.Drawing.Size(847, 293)
        Me.DGVTelList.TabIndex = 36
        '
        'BTelefonliste
        '
        Me.BTelefonliste.Location = New System.Drawing.Point(591, 337)
        Me.BTelefonliste.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BTelefonliste.Name = "BTelefonliste"
        Me.BTelefonliste.Size = New System.Drawing.Size(255, 43)
        Me.BTelefonliste.TabIndex = 35
        Me.BTelefonliste.Text = "Telefone erneut einlesen"
        Me.BTelefonliste.UseVisualStyleBackColor = True
        '
        'Label15
        '
        Me.Label15.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(4, 5)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(852, 54)
        Me.Label15.TabIndex = 26
        Me.Label15.Text = "Nummern"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PAnrufmonitor
        '
        Me.PAnrufmonitor.Controls.Add(Me.GroupBox6)
        Me.PAnrufmonitor.Controls.Add(Me.GBJournal)
        Me.PAnrufmonitor.Controls.Add(Me.GBAnrListeMain)
        Me.PAnrufmonitor.Controls.Add(Me.Label22)
        Me.PAnrufmonitor.Controls.Add(Me.GBAnrMonMain)
        Me.PAnrufmonitor.Location = New System.Drawing.Point(4, 29)
        Me.PAnrufmonitor.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PAnrufmonitor.Name = "PAnrufmonitor"
        Me.PAnrufmonitor.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PAnrufmonitor.Size = New System.Drawing.Size(860, 458)
        Me.PAnrufmonitor.TabIndex = 0
        Me.PAnrufmonitor.Text = "Anrufmonitor"
        Me.PAnrufmonitor.UseVisualStyleBackColor = True
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.TBNumEntryList)
        Me.GroupBox6.Controls.Add(Me.LNumEntryList)
        Me.GroupBox6.Location = New System.Drawing.Point(432, 297)
        Me.GroupBox6.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox6.Size = New System.Drawing.Size(424, 85)
        Me.GroupBox6.TabIndex = 38
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Anruflisten"
        '
        'GBJournal
        '
        Me.GBJournal.Controls.Add(Me.CBJournal)
        Me.GBJournal.Location = New System.Drawing.Point(432, 58)
        Me.GBJournal.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBJournal.Name = "GBJournal"
        Me.GBJournal.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBJournal.Size = New System.Drawing.Size(423, 85)
        Me.GBJournal.TabIndex = 37
        Me.GBJournal.TabStop = False
        Me.GBJournal.Text = "Outlook Journal"
        '
        'CBJournal
        '
        Me.CBJournal.AutoSize = True
        Me.CBJournal.Location = New System.Drawing.Point(4, 35)
        Me.CBJournal.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBJournal.MinimumSize = New System.Drawing.Size(400, 0)
        Me.CBJournal.Name = "CBJournal"
        Me.CBJournal.Size = New System.Drawing.Size(400, 24)
        Me.CBJournal.TabIndex = 7
        Me.CBJournal.Text = "Journaleinträge erstellen"
        Me.CBJournal.UseVisualStyleBackColor = True
        '
        'GBAnrListeMain
        '
        Me.GBAnrListeMain.Controls.Add(Me.CBAnrListeShowAnrMon)
        Me.GBAnrListeMain.Controls.Add(Me.CBAutoAnrList)
        Me.GBAnrListeMain.Controls.Add(Me.CBAnrListeUpdateCallLists)
        Me.GBAnrListeMain.Location = New System.Drawing.Point(432, 153)
        Me.GBAnrListeMain.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBAnrListeMain.Name = "GBAnrListeMain"
        Me.GBAnrListeMain.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBAnrListeMain.Size = New System.Drawing.Size(423, 134)
        Me.GBAnrListeMain.TabIndex = 36
        Me.GBAnrListeMain.TabStop = False
        Me.GBAnrListeMain.Text = "Auswertung der Anrufliste"
        '
        'CBAnrListeShowAnrMon
        '
        Me.CBAnrListeShowAnrMon.AutoSize = True
        Me.CBAnrListeShowAnrMon.Enabled = False
        Me.CBAnrListeShowAnrMon.Location = New System.Drawing.Point(4, 97)
        Me.CBAnrListeShowAnrMon.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAnrListeShowAnrMon.MinimumSize = New System.Drawing.Size(400, 0)
        Me.CBAnrListeShowAnrMon.Name = "CBAnrListeShowAnrMon"
        Me.CBAnrListeShowAnrMon.Size = New System.Drawing.Size(400, 24)
        Me.CBAnrListeShowAnrMon.TabIndex = 9
        Me.CBAnrListeShowAnrMon.Text = "Verpasste Telefonate mit Anrufmonitor anzeigen"
        Me.CBAnrListeShowAnrMon.UseVisualStyleBackColor = True
        '
        'CBAnrListeUpdateCallLists
        '
        Me.CBAnrListeUpdateCallLists.AutoSize = True
        Me.CBAnrListeUpdateCallLists.Location = New System.Drawing.Point(4, 63)
        Me.CBAnrListeUpdateCallLists.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAnrListeUpdateCallLists.MinimumSize = New System.Drawing.Size(400, 0)
        Me.CBAnrListeUpdateCallLists.Name = "CBAnrListeUpdateCallLists"
        Me.CBAnrListeUpdateCallLists.Size = New System.Drawing.Size(400, 24)
        Me.CBAnrListeUpdateCallLists.TabIndex = 7
        Me.CBAnrListeUpdateCallLists.Text = "Rückruf- und Wahlwiederholungsliste aktualisieren"
        Me.CBAnrListeUpdateCallLists.UseVisualStyleBackColor = True
        '
        'Label22
        '
        Me.Label22.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label22.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.Location = New System.Drawing.Point(4, 5)
        Me.Label22.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(852, 54)
        Me.Label22.TabIndex = 29
        Me.Label22.Text = "Einstellungen für den Anrufmonitor"
        Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'GBAnrMonMain
        '
        Me.GBAnrMonMain.Controls.Add(Me.PanelAnrMon)
        Me.GBAnrMonMain.Controls.Add(Me.CBUseAnrMon)
        Me.GBAnrMonMain.Location = New System.Drawing.Point(0, 58)
        Me.GBAnrMonMain.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBAnrMonMain.Name = "GBAnrMonMain"
        Me.GBAnrMonMain.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBAnrMonMain.Size = New System.Drawing.Size(423, 394)
        Me.GBAnrMonMain.TabIndex = 14
        Me.GBAnrMonMain.TabStop = False
        Me.GBAnrMonMain.Text = "Einstellungen für den Anrufmonitor"
        '
        'PanelAnrMon
        '
        Me.PanelAnrMon.Controls.Add(Me.CBAnrMonContactImage)
        Me.PanelAnrMon.Controls.Add(Me.CBShowMSN)
        Me.PanelAnrMon.Controls.Add(Me.CBAnrMonZeigeKontakt)
        Me.PanelAnrMon.Controls.Add(Me.CBAnrMonAuto)
        Me.PanelAnrMon.Controls.Add(Me.CBAnrMonCloseAtDISSCONNECT)
        Me.PanelAnrMon.Controls.Add(Me.CBAutoClose)
        Me.PanelAnrMon.Controls.Add(Me.Label2)
        Me.PanelAnrMon.Controls.Add(Me.TBEnblDauer)
        Me.PanelAnrMon.Controls.Add(Me.CLBTelNr)
        Me.PanelAnrMon.Controls.Add(Me.LEnblDauer)
        Me.PanelAnrMon.Location = New System.Drawing.Point(0, 55)
        Me.PanelAnrMon.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PanelAnrMon.Name = "PanelAnrMon"
        Me.PanelAnrMon.Size = New System.Drawing.Size(423, 338)
        Me.PanelAnrMon.TabIndex = 35
        '
        'CBAnrMonContactImage
        '
        Me.CBAnrMonContactImage.AutoSize = True
        Me.CBAnrMonContactImage.Location = New System.Drawing.Point(235, 269)
        Me.CBAnrMonContactImage.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAnrMonContactImage.MinimumSize = New System.Drawing.Size(150, 0)
        Me.CBAnrMonContactImage.Name = "CBAnrMonContactImage"
        Me.CBAnrMonContactImage.Size = New System.Drawing.Size(158, 24)
        Me.CBAnrMonContactImage.TabIndex = 910
        Me.CBAnrMonContactImage.Text = "Zeige Kontaktbild"
        Me.CBAnrMonContactImage.UseVisualStyleBackColor = True
        '
        'CBAnrMonZeigeKontakt
        '
        Me.CBAnrMonZeigeKontakt.AutoSize = True
        Me.CBAnrMonZeigeKontakt.Enabled = False
        Me.CBAnrMonZeigeKontakt.Location = New System.Drawing.Point(4, 269)
        Me.CBAnrMonZeigeKontakt.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAnrMonZeigeKontakt.MinimumSize = New System.Drawing.Size(200, 0)
        Me.CBAnrMonZeigeKontakt.Name = "CBAnrMonZeigeKontakt"
        Me.CBAnrMonZeigeKontakt.Size = New System.Drawing.Size(208, 24)
        Me.CBAnrMonZeigeKontakt.TabIndex = 34
        Me.CBAnrMonZeigeKontakt.Text = "Kontakt bei Anruf öffnen"
        Me.CBAnrMonZeigeKontakt.UseVisualStyleBackColor = True
        '
        'CBAnrMonAuto
        '
        Me.CBAnrMonAuto.AutoSize = True
        Me.CBAnrMonAuto.Location = New System.Drawing.Point(4, 9)
        Me.CBAnrMonAuto.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAnrMonAuto.MinimumSize = New System.Drawing.Size(400, 0)
        Me.CBAnrMonAuto.Name = "CBAnrMonAuto"
        Me.CBAnrMonAuto.Size = New System.Drawing.Size(400, 24)
        Me.CBAnrMonAuto.TabIndex = 1
        Me.CBAnrMonAuto.Text = "Anrufmonitor mit Outlook starten"
        Me.CBAnrMonAuto.UseVisualStyleBackColor = True
        '
        'CBAnrMonCloseAtDISSCONNECT
        '
        Me.CBAnrMonCloseAtDISSCONNECT.AutoSize = True
        Me.CBAnrMonCloseAtDISSCONNECT.Enabled = False
        Me.CBAnrMonCloseAtDISSCONNECT.Location = New System.Drawing.Point(4, 115)
        Me.CBAnrMonCloseAtDISSCONNECT.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAnrMonCloseAtDISSCONNECT.MinimumSize = New System.Drawing.Size(400, 0)
        Me.CBAnrMonCloseAtDISSCONNECT.Name = "CBAnrMonCloseAtDISSCONNECT"
        Me.CBAnrMonCloseAtDISSCONNECT.Size = New System.Drawing.Size(400, 24)
        Me.CBAnrMonCloseAtDISSCONNECT.TabIndex = 2
        Me.CBAnrMonCloseAtDISSCONNECT.Text = "Anruffenster beim Auflegen ausblenden"
        Me.CBAnrMonCloseAtDISSCONNECT.UseVisualStyleBackColor = True
        '
        'CBAutoClose
        '
        Me.CBAutoClose.AutoSize = True
        Me.CBAutoClose.Location = New System.Drawing.Point(4, 45)
        Me.CBAutoClose.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAutoClose.MinimumSize = New System.Drawing.Size(400, 0)
        Me.CBAutoClose.Name = "CBAutoClose"
        Me.CBAutoClose.Size = New System.Drawing.Size(400, 24)
        Me.CBAutoClose.TabIndex = 2
        Me.CBAutoClose.Text = "Anruffenster automatisch ausblenden"
        Me.CBAutoClose.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(231, 152)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(171, 74)
        Me.Label2.TabIndex = 33
        Me.Label2.Text = "Stellen Sie die zu überwachenden Telefonnummern ein."
        '
        'TBEnblDauer
        '
        Me.TBEnblDauer.Location = New System.Drawing.Point(3, 77)
        Me.TBEnblDauer.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBEnblDauer.Name = "TBEnblDauer"
        Me.TBEnblDauer.Size = New System.Drawing.Size(42, 26)
        Me.TBEnblDauer.TabIndex = 3
        '
        'CLBTelNr
        '
        Me.CLBTelNr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CLBTelNr.CheckOnClick = True
        Me.CLBTelNr.HorizontalScrollbar = True
        Me.CLBTelNr.IntegralHeight = False
        Me.CLBTelNr.Location = New System.Drawing.Point(0, 151)
        Me.CLBTelNr.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CLBTelNr.Name = "CLBTelNr"
        Me.CLBTelNr.Size = New System.Drawing.Size(221, 108)
        Me.CLBTelNr.TabIndex = 4
        '
        'LEnblDauer
        '
        Me.LEnblDauer.AutoSize = True
        Me.LEnblDauer.Location = New System.Drawing.Point(56, 82)
        Me.LEnblDauer.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LEnblDauer.MinimumSize = New System.Drawing.Size(350, 0)
        Me.LEnblDauer.Name = "LEnblDauer"
        Me.LEnblDauer.Size = New System.Drawing.Size(350, 20)
        Me.LEnblDauer.TabIndex = 31
        Me.LEnblDauer.Text = "Anzeigedauer bei Anruf (minimal: 4s) [s]"
        '
        'CBUseAnrMon
        '
        Me.CBUseAnrMon.AutoSize = True
        Me.CBUseAnrMon.Enabled = False
        Me.CBUseAnrMon.Location = New System.Drawing.Point(4, 29)
        Me.CBUseAnrMon.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBUseAnrMon.MinimumSize = New System.Drawing.Size(400, 0)
        Me.CBUseAnrMon.Name = "CBUseAnrMon"
        Me.CBUseAnrMon.Size = New System.Drawing.Size(400, 24)
        Me.CBUseAnrMon.TabIndex = 0
        Me.CBUseAnrMon.Text = "Anrufmonitor verwenden"
        Me.CBUseAnrMon.UseVisualStyleBackColor = True
        '
        'FBDB_MP
        '
        Me.FBDB_MP.Controls.Add(Me.PGrundeinstellungen)
        Me.FBDB_MP.Controls.Add(Me.PAnrufmonitor)
        Me.FBDB_MP.Controls.Add(Me.PAnrMonSim)
        Me.FBDB_MP.Controls.Add(Me.PKontaktsuche2)
        Me.FBDB_MP.Controls.Add(Me.PTelefone)
        Me.FBDB_MP.Controls.Add(Me.PLogging)
        Me.FBDB_MP.Controls.Add(Me.PInfo)
        Me.FBDB_MP.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FBDB_MP.Location = New System.Drawing.Point(4, 5)
        Me.FBDB_MP.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.FBDB_MP.Name = "FBDB_MP"
        Me.FBDB_MP.SelectedIndex = 0
        Me.FBDB_MP.Size = New System.Drawing.Size(868, 491)
        Me.FBDB_MP.TabIndex = 1
        '
        'PAnrMonSim
        '
        Me.PAnrMonSim.Controls.Add(Me.GBoxAnrMonDISCONNECT)
        Me.PAnrMonSim.Controls.Add(Me.GBoxAnrMonCONNECT)
        Me.PAnrMonSim.Controls.Add(Me.GBoxAnrMonCALL)
        Me.PAnrMonSim.Controls.Add(Me.GBoxAnrMonRING)
        Me.PAnrMonSim.Location = New System.Drawing.Point(4, 29)
        Me.PAnrMonSim.Name = "PAnrMonSim"
        Me.PAnrMonSim.Padding = New System.Windows.Forms.Padding(3)
        Me.PAnrMonSim.Size = New System.Drawing.Size(860, 458)
        Me.PAnrMonSim.TabIndex = 14
        Me.PAnrMonSim.Text = "Simulation"
        Me.PAnrMonSim.UseVisualStyleBackColor = True
        '
        'GBoxAnrMonDISCONNECT
        '
        Me.GBoxAnrMonDISCONNECT.Controls.Add(Me.TBAnrMonSimDISCONNECTDauer)
        Me.GBoxAnrMonDISCONNECT.Controls.Add(Me.Label26)
        Me.GBoxAnrMonDISCONNECT.Controls.Add(Me.Label27)
        Me.GBoxAnrMonDISCONNECT.Controls.Add(Me.LAnrMonSimDISCONNECT)
        Me.GBoxAnrMonDISCONNECT.Controls.Add(Me.TBAnrMonSimDISCONNECTID)
        Me.GBoxAnrMonDISCONNECT.Controls.Add(Me.LAnrMonSimLabelDISCONNECT)
        Me.GBoxAnrMonDISCONNECT.Controls.Add(Me.DTPAnrMonSimDISCONNECT)
        Me.GBoxAnrMonDISCONNECT.Controls.Add(Me.BAnrMonSimDISCONNECT)
        Me.GBoxAnrMonDISCONNECT.Location = New System.Drawing.Point(6, 324)
        Me.GBoxAnrMonDISCONNECT.Name = "GBoxAnrMonDISCONNECT"
        Me.GBoxAnrMonDISCONNECT.Size = New System.Drawing.Size(848, 100)
        Me.GBoxAnrMonDISCONNECT.TabIndex = 3
        Me.GBoxAnrMonDISCONNECT.TabStop = False
        Me.GBoxAnrMonDISCONNECT.Text = "Anrufmonitor DISCONNECT"
        '
        'TBAnrMonSimDISCONNECTDauer
        '
        Me.TBAnrMonSimDISCONNECTDauer.Location = New System.Drawing.Point(363, 27)
        Me.TBAnrMonSimDISCONNECTDauer.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.TBAnrMonSimDISCONNECTDauer.Name = "TBAnrMonSimDISCONNECTDauer"
        Me.TBAnrMonSimDISCONNECTDauer.Size = New System.Drawing.Size(32, 26)
        Me.TBAnrMonSimDISCONNECTDauer.TabIndex = 44
        Me.TBAnrMonSimDISCONNECTDauer.Text = "0"
        Me.TBAnrMonSimDISCONNECTDauer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Location = New System.Drawing.Point(398, 27)
        Me.Label26.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(13, 20)
        Me.Label26.TabIndex = 43
        Me.Label26.Text = ";"
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Location = New System.Drawing.Point(348, 30)
        Me.Label27.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(13, 20)
        Me.Label27.TabIndex = 41
        Me.Label27.Text = ";"
        '
        'LAnrMonSimDISCONNECT
        '
        Me.LAnrMonSimDISCONNECT.AutoSize = True
        Me.LAnrMonSimDISCONNECT.Location = New System.Drawing.Point(191, 30)
        Me.LAnrMonSimDISCONNECT.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.LAnrMonSimDISCONNECT.Name = "LAnrMonSimDISCONNECT"
        Me.LAnrMonSimDISCONNECT.Size = New System.Drawing.Size(121, 20)
        Me.LAnrMonSimDISCONNECT.TabIndex = 40
        Me.LAnrMonSimDISCONNECT.Text = ";DISCONNECT;"
        '
        'TBAnrMonSimDISCONNECTID
        '
        Me.TBAnrMonSimDISCONNECTID.Location = New System.Drawing.Point(314, 27)
        Me.TBAnrMonSimDISCONNECTID.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.TBAnrMonSimDISCONNECTID.Name = "TBAnrMonSimDISCONNECTID"
        Me.TBAnrMonSimDISCONNECTID.Size = New System.Drawing.Size(32, 26)
        Me.TBAnrMonSimDISCONNECTID.TabIndex = 39
        Me.TBAnrMonSimDISCONNECTID.Text = "0"
        Me.TBAnrMonSimDISCONNECTID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'LAnrMonSimLabelDISCONNECT
        '
        Me.LAnrMonSimLabelDISCONNECT.AutoSize = True
        Me.LAnrMonSimLabelDISCONNECT.Location = New System.Drawing.Point(6, 66)
        Me.LAnrMonSimLabelDISCONNECT.Name = "LAnrMonSimLabelDISCONNECT"
        Me.LAnrMonSimLabelDISCONNECT.Size = New System.Drawing.Size(275, 20)
        Me.LAnrMonSimLabelDISCONNECT.TabIndex = 38
        Me.LAnrMonSimLabelDISCONNECT.Text = "23.06.18 13:20:52;DISCONNECT;1;9;"
        '
        'DTPAnrMonSimDISCONNECT
        '
        Me.DTPAnrMonSimDISCONNECT.CustomFormat = "dd.MM.yy HH:mm:ss"
        Me.DTPAnrMonSimDISCONNECT.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPAnrMonSimDISCONNECT.Location = New System.Drawing.Point(6, 25)
        Me.DTPAnrMonSimDISCONNECT.Margin = New System.Windows.Forms.Padding(3, 3, 1, 3)
        Me.DTPAnrMonSimDISCONNECT.Name = "DTPAnrMonSimDISCONNECT"
        Me.DTPAnrMonSimDISCONNECT.Size = New System.Drawing.Size(183, 26)
        Me.DTPAnrMonSimDISCONNECT.TabIndex = 37
        Me.DTPAnrMonSimDISCONNECT.Value = New Date(2019, 6, 29, 15, 7, 37, 0)
        '
        'BAnrMonSimDISCONNECT
        '
        Me.BAnrMonSimDISCONNECT.Location = New System.Drawing.Point(720, 25)
        Me.BAnrMonSimDISCONNECT.Name = "BAnrMonSimDISCONNECT"
        Me.BAnrMonSimDISCONNECT.Size = New System.Drawing.Size(122, 61)
        Me.BAnrMonSimDISCONNECT.TabIndex = 3
        Me.BAnrMonSimDISCONNECT.Text = "DISCONNECT"
        Me.BAnrMonSimDISCONNECT.UseVisualStyleBackColor = True
        '
        'GBoxAnrMonCONNECT
        '
        Me.GBoxAnrMonCONNECT.Controls.Add(Me.Label19)
        Me.GBoxAnrMonCONNECT.Controls.Add(Me.CBoxAnrMonSimCONNECTNSTID)
        Me.GBoxAnrMonCONNECT.Controls.Add(Me.Label18)
        Me.GBoxAnrMonCONNECT.Controls.Add(Me.TBAnrMonSimCONNECTAugTelNr)
        Me.GBoxAnrMonCONNECT.Controls.Add(Me.Label24)
        Me.GBoxAnrMonCONNECT.Controls.Add(Me.LAnrMonSimCONNECT)
        Me.GBoxAnrMonCONNECT.Controls.Add(Me.TBAnrMonSimCONNECTID)
        Me.GBoxAnrMonCONNECT.Controls.Add(Me.LAnrMonSimLabelCONNECT)
        Me.GBoxAnrMonCONNECT.Controls.Add(Me.DTPAnrMonSimCONNECT)
        Me.GBoxAnrMonCONNECT.Controls.Add(Me.BAnrMonSimCONNECT)
        Me.GBoxAnrMonCONNECT.Location = New System.Drawing.Point(6, 218)
        Me.GBoxAnrMonCONNECT.Name = "GBoxAnrMonCONNECT"
        Me.GBoxAnrMonCONNECT.Size = New System.Drawing.Size(848, 100)
        Me.GBoxAnrMonCONNECT.TabIndex = 2
        Me.GBoxAnrMonCONNECT.TabStop = False
        Me.GBoxAnrMonCONNECT.Text = "Anrufmonitor CONNECT"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(545, 30)
        Me.Label19.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(13, 20)
        Me.Label19.TabIndex = 37
        Me.Label19.Text = ";"
        '
        'CBoxAnrMonSimCONNECTNSTID
        '
        Me.CBoxAnrMonSimCONNECTNSTID.FormattingEnabled = True
        Me.CBoxAnrMonSimCONNECTNSTID.Location = New System.Drawing.Point(335, 27)
        Me.CBoxAnrMonSimCONNECTNSTID.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.CBoxAnrMonSimCONNECTNSTID.Name = "CBoxAnrMonSimCONNECTNSTID"
        Me.CBoxAnrMonSimCONNECTNSTID.Size = New System.Drawing.Size(71, 28)
        Me.CBoxAnrMonSimCONNECTNSTID.TabIndex = 36
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(408, 30)
        Me.Label18.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(13, 20)
        Me.Label18.TabIndex = 35
        Me.Label18.Text = ";"
        '
        'TBAnrMonSimCONNECTAugTelNr
        '
        Me.TBAnrMonSimCONNECTAugTelNr.Location = New System.Drawing.Point(423, 27)
        Me.TBAnrMonSimCONNECTAugTelNr.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.TBAnrMonSimCONNECTAugTelNr.Name = "TBAnrMonSimCONNECTAugTelNr"
        Me.TBAnrMonSimCONNECTAugTelNr.Size = New System.Drawing.Size(122, 26)
        Me.TBAnrMonSimCONNECTAugTelNr.TabIndex = 32
        Me.TBAnrMonSimCONNECTAugTelNr.Text = "0123456789"
        Me.TBAnrMonSimCONNECTAugTelNr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(320, 30)
        Me.Label24.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(13, 20)
        Me.Label24.TabIndex = 31
        Me.Label24.Text = ";"
        '
        'LAnrMonSimCONNECT
        '
        Me.LAnrMonSimCONNECT.AutoSize = True
        Me.LAnrMonSimCONNECT.Location = New System.Drawing.Point(191, 30)
        Me.LAnrMonSimCONNECT.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.LAnrMonSimCONNECT.Name = "LAnrMonSimCONNECT"
        Me.LAnrMonSimCONNECT.Size = New System.Drawing.Size(93, 20)
        Me.LAnrMonSimCONNECT.TabIndex = 30
        Me.LAnrMonSimCONNECT.Text = ";CONNECT;"
        '
        'TBAnrMonSimCONNECTID
        '
        Me.TBAnrMonSimCONNECTID.Location = New System.Drawing.Point(286, 27)
        Me.TBAnrMonSimCONNECTID.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.TBAnrMonSimCONNECTID.Name = "TBAnrMonSimCONNECTID"
        Me.TBAnrMonSimCONNECTID.Size = New System.Drawing.Size(32, 26)
        Me.TBAnrMonSimCONNECTID.TabIndex = 29
        Me.TBAnrMonSimCONNECTID.Text = "0"
        Me.TBAnrMonSimCONNECTID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'LAnrMonSimLabelCONNECT
        '
        Me.LAnrMonSimLabelCONNECT.AutoSize = True
        Me.LAnrMonSimLabelCONNECT.Location = New System.Drawing.Point(6, 66)
        Me.LAnrMonSimLabelCONNECT.Name = "LAnrMonSimLabelCONNECT"
        Me.LAnrMonSimLabelCONNECT.Size = New System.Drawing.Size(350, 20)
        Me.LAnrMonSimLabelCONNECT.TabIndex = 28
        Me.LAnrMonSimLabelCONNECT.Text = "23.06.18 13:20:44;CONNECT;1;40;0123456789;"
        '
        'DTPAnrMonSimCONNECT
        '
        Me.DTPAnrMonSimCONNECT.CustomFormat = "dd.MM.yy HH:mm:ss"
        Me.DTPAnrMonSimCONNECT.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPAnrMonSimCONNECT.Location = New System.Drawing.Point(6, 25)
        Me.DTPAnrMonSimCONNECT.Margin = New System.Windows.Forms.Padding(3, 3, 1, 3)
        Me.DTPAnrMonSimCONNECT.Name = "DTPAnrMonSimCONNECT"
        Me.DTPAnrMonSimCONNECT.Size = New System.Drawing.Size(183, 26)
        Me.DTPAnrMonSimCONNECT.TabIndex = 27
        Me.DTPAnrMonSimCONNECT.Value = New Date(2019, 6, 29, 15, 7, 37, 0)
        '
        'BAnrMonSimCONNECT
        '
        Me.BAnrMonSimCONNECT.Location = New System.Drawing.Point(720, 25)
        Me.BAnrMonSimCONNECT.Name = "BAnrMonSimCONNECT"
        Me.BAnrMonSimCONNECT.Size = New System.Drawing.Size(122, 61)
        Me.BAnrMonSimCONNECT.TabIndex = 2
        Me.BAnrMonSimCONNECT.Text = "CONNECT"
        Me.BAnrMonSimCONNECT.UseVisualStyleBackColor = True
        '
        'GBoxAnrMonCALL
        '
        Me.GBoxAnrMonCALL.Controls.Add(Me.CBoxAnrMonSimCALLNSTID)
        Me.GBoxAnrMonCALL.Controls.Add(Me.Label14)
        Me.GBoxAnrMonCALL.Controls.Add(Me.Label3)
        Me.GBoxAnrMonCALL.Controls.Add(Me.CBoxAnrMonSimCALLSIPID)
        Me.GBoxAnrMonCALL.Controls.Add(Me.Label9)
        Me.GBoxAnrMonCALL.Controls.Add(Me.CBoxAnrMonSimCALLEigTelNr)
        Me.GBoxAnrMonCALL.Controls.Add(Me.Label11)
        Me.GBoxAnrMonCALL.Controls.Add(Me.TBAnrMonSimCALLAugTelNr)
        Me.GBoxAnrMonCALL.Controls.Add(Me.Label12)
        Me.GBoxAnrMonCALL.Controls.Add(Me.LAnrMonSimCALL)
        Me.GBoxAnrMonCALL.Controls.Add(Me.TBAnrMonSimCALLID)
        Me.GBoxAnrMonCALL.Controls.Add(Me.LAnrMonSimLabelCALL)
        Me.GBoxAnrMonCALL.Controls.Add(Me.DTPAnrMonSimCALL)
        Me.GBoxAnrMonCALL.Controls.Add(Me.BAnrMonSimCALL)
        Me.GBoxAnrMonCALL.Location = New System.Drawing.Point(6, 112)
        Me.GBoxAnrMonCALL.Name = "GBoxAnrMonCALL"
        Me.GBoxAnrMonCALL.Size = New System.Drawing.Size(848, 100)
        Me.GBoxAnrMonCALL.TabIndex = 1
        Me.GBoxAnrMonCALL.TabStop = False
        Me.GBoxAnrMonCALL.Text = "Anrufmonitor CALL"
        '
        'CBoxAnrMonSimCALLNSTID
        '
        Me.CBoxAnrMonSimCALLNSTID.FormattingEnabled = True
        Me.CBoxAnrMonSimCALLNSTID.Location = New System.Drawing.Point(299, 27)
        Me.CBoxAnrMonSimCALLNSTID.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.CBoxAnrMonSimCALLNSTID.Name = "CBoxAnrMonSimCALLNSTID"
        Me.CBoxAnrMonSimCALLNSTID.Size = New System.Drawing.Size(71, 28)
        Me.CBoxAnrMonSimCALLNSTID.TabIndex = 26
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(480, 30)
        Me.Label14.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(13, 20)
        Me.Label14.TabIndex = 25
        Me.Label14.Text = ";"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(694, 30)
        Me.Label3.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(13, 20)
        Me.Label3.TabIndex = 23
        Me.Label3.Text = ";"
        '
        'CBoxAnrMonSimCALLSIPID
        '
        Me.CBoxAnrMonSimCALLSIPID.FormattingEnabled = True
        Me.CBoxAnrMonSimCALLSIPID.Location = New System.Drawing.Point(634, 27)
        Me.CBoxAnrMonSimCALLSIPID.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.CBoxAnrMonSimCALLSIPID.Name = "CBoxAnrMonSimCALLSIPID"
        Me.CBoxAnrMonSimCALLSIPID.Size = New System.Drawing.Size(58, 28)
        Me.CBoxAnrMonSimCALLSIPID.TabIndex = 22
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(619, 30)
        Me.Label9.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(13, 20)
        Me.Label9.TabIndex = 21
        Me.Label9.Text = ";"
        '
        'CBoxAnrMonSimCALLEigTelNr
        '
        Me.CBoxAnrMonSimCALLEigTelNr.FormattingEnabled = True
        Me.CBoxAnrMonSimCALLEigTelNr.Location = New System.Drawing.Point(387, 27)
        Me.CBoxAnrMonSimCALLEigTelNr.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.CBoxAnrMonSimCALLEigTelNr.Name = "CBoxAnrMonSimCALLEigTelNr"
        Me.CBoxAnrMonSimCALLEigTelNr.Size = New System.Drawing.Size(91, 28)
        Me.CBoxAnrMonSimCALLEigTelNr.TabIndex = 20
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(372, 30)
        Me.Label11.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(13, 20)
        Me.Label11.TabIndex = 19
        Me.Label11.Text = ";"
        '
        'TBAnrMonSimCALLAugTelNr
        '
        Me.TBAnrMonSimCALLAugTelNr.Location = New System.Drawing.Point(495, 27)
        Me.TBAnrMonSimCALLAugTelNr.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.TBAnrMonSimCALLAugTelNr.Name = "TBAnrMonSimCALLAugTelNr"
        Me.TBAnrMonSimCALLAugTelNr.Size = New System.Drawing.Size(122, 26)
        Me.TBAnrMonSimCALLAugTelNr.TabIndex = 18
        Me.TBAnrMonSimCALLAugTelNr.Text = "0123456789"
        Me.TBAnrMonSimCALLAugTelNr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(284, 30)
        Me.Label12.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(13, 20)
        Me.Label12.TabIndex = 17
        Me.Label12.Text = ";"
        '
        'LAnrMonSimCALL
        '
        Me.LAnrMonSimCALL.AutoSize = True
        Me.LAnrMonSimCALL.Location = New System.Drawing.Point(191, 30)
        Me.LAnrMonSimCALL.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.LAnrMonSimCALL.Name = "LAnrMonSimCALL"
        Me.LAnrMonSimCALL.Size = New System.Drawing.Size(57, 20)
        Me.LAnrMonSimCALL.TabIndex = 16
        Me.LAnrMonSimCALL.Text = ";CALL;"
        '
        'TBAnrMonSimCALLID
        '
        Me.TBAnrMonSimCALLID.Location = New System.Drawing.Point(250, 27)
        Me.TBAnrMonSimCALLID.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.TBAnrMonSimCALLID.Name = "TBAnrMonSimCALLID"
        Me.TBAnrMonSimCALLID.Size = New System.Drawing.Size(32, 26)
        Me.TBAnrMonSimCALLID.TabIndex = 15
        Me.TBAnrMonSimCALLID.Text = "0"
        Me.TBAnrMonSimCALLID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'LAnrMonSimLabelCALL
        '
        Me.LAnrMonSimLabelCALL.AutoSize = True
        Me.LAnrMonSimLabelCALL.Location = New System.Drawing.Point(6, 66)
        Me.LAnrMonSimLabelCALL.Name = "LAnrMonSimLabelCALL"
        Me.LAnrMonSimLabelCALL.Size = New System.Drawing.Size(402, 20)
        Me.LAnrMonSimLabelCALL.TabIndex = 14
        Me.LAnrMonSimLabelCALL.Text = "23.06.18 13:20:24;CALL;3;4;987654;0123456789;SIP0;"
        '
        'DTPAnrMonSimCALL
        '
        Me.DTPAnrMonSimCALL.CustomFormat = "dd.MM.yy HH:mm:ss"
        Me.DTPAnrMonSimCALL.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPAnrMonSimCALL.Location = New System.Drawing.Point(6, 25)
        Me.DTPAnrMonSimCALL.Margin = New System.Windows.Forms.Padding(3, 3, 1, 3)
        Me.DTPAnrMonSimCALL.Name = "DTPAnrMonSimCALL"
        Me.DTPAnrMonSimCALL.Size = New System.Drawing.Size(183, 26)
        Me.DTPAnrMonSimCALL.TabIndex = 13
        Me.DTPAnrMonSimCALL.Value = New Date(2019, 6, 29, 15, 7, 37, 0)
        '
        'BAnrMonSimCALL
        '
        Me.BAnrMonSimCALL.Location = New System.Drawing.Point(720, 25)
        Me.BAnrMonSimCALL.Name = "BAnrMonSimCALL"
        Me.BAnrMonSimCALL.Size = New System.Drawing.Size(122, 61)
        Me.BAnrMonSimCALL.TabIndex = 1
        Me.BAnrMonSimCALL.Text = "CALL"
        Me.BAnrMonSimCALL.UseVisualStyleBackColor = True
        '
        'GBoxAnrMonRING
        '
        Me.GBoxAnrMonRING.Controls.Add(Me.Label8)
        Me.GBoxAnrMonRING.Controls.Add(Me.CBoxAnrMonSimRINGSIPID)
        Me.GBoxAnrMonRING.Controls.Add(Me.Label6)
        Me.GBoxAnrMonRING.Controls.Add(Me.CBoxAnrMonSimRINGEigTelNr)
        Me.GBoxAnrMonRING.Controls.Add(Me.Label5)
        Me.GBoxAnrMonRING.Controls.Add(Me.TBAnrMonSimRINGAugTelNr)
        Me.GBoxAnrMonRING.Controls.Add(Me.Label4)
        Me.GBoxAnrMonRING.Controls.Add(Me.LAnrMonSimRING)
        Me.GBoxAnrMonRING.Controls.Add(Me.TBAnrMonSimRINGID)
        Me.GBoxAnrMonRING.Controls.Add(Me.LAnrMonSimLabelRING)
        Me.GBoxAnrMonRING.Controls.Add(Me.DTPAnrMonSimRING)
        Me.GBoxAnrMonRING.Controls.Add(Me.BAnrMonSimRING)
        Me.GBoxAnrMonRING.Location = New System.Drawing.Point(6, 6)
        Me.GBoxAnrMonRING.Name = "GBoxAnrMonRING"
        Me.GBoxAnrMonRING.Size = New System.Drawing.Size(848, 100)
        Me.GBoxAnrMonRING.TabIndex = 0
        Me.GBoxAnrMonRING.TabStop = False
        Me.GBoxAnrMonRING.Text = "Anrufmonitor RING"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(609, 30)
        Me.Label8.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(13, 20)
        Me.Label8.TabIndex = 12
        Me.Label8.Text = ";"
        '
        'CBoxAnrMonSimRINGSIPID
        '
        Me.CBoxAnrMonSimRINGSIPID.FormattingEnabled = True
        Me.CBoxAnrMonSimRINGSIPID.Location = New System.Drawing.Point(549, 27)
        Me.CBoxAnrMonSimRINGSIPID.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.CBoxAnrMonSimRINGSIPID.Name = "CBoxAnrMonSimRINGSIPID"
        Me.CBoxAnrMonSimRINGSIPID.Size = New System.Drawing.Size(58, 28)
        Me.CBoxAnrMonSimRINGSIPID.TabIndex = 11
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(534, 30)
        Me.Label6.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(13, 20)
        Me.Label6.TabIndex = 10
        Me.Label6.Text = ";"
        '
        'CBoxAnrMonSimRINGEigTelNr
        '
        Me.CBoxAnrMonSimRINGEigTelNr.FormattingEnabled = True
        Me.CBoxAnrMonSimRINGEigTelNr.Location = New System.Drawing.Point(441, 27)
        Me.CBoxAnrMonSimRINGEigTelNr.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.CBoxAnrMonSimRINGEigTelNr.Name = "CBoxAnrMonSimRINGEigTelNr"
        Me.CBoxAnrMonSimRINGEigTelNr.Size = New System.Drawing.Size(91, 28)
        Me.CBoxAnrMonSimRINGEigTelNr.TabIndex = 8
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(426, 30)
        Me.Label5.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(13, 20)
        Me.Label5.TabIndex = 7
        Me.Label5.Text = ";"
        '
        'TBAnrMonSimRINGAugTelNr
        '
        Me.TBAnrMonSimRINGAugTelNr.Location = New System.Drawing.Point(299, 27)
        Me.TBAnrMonSimRINGAugTelNr.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.TBAnrMonSimRINGAugTelNr.Name = "TBAnrMonSimRINGAugTelNr"
        Me.TBAnrMonSimRINGAugTelNr.Size = New System.Drawing.Size(122, 26)
        Me.TBAnrMonSimRINGAugTelNr.TabIndex = 6
        Me.TBAnrMonSimRINGAugTelNr.Text = "0123456789"
        Me.TBAnrMonSimRINGAugTelNr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(284, 30)
        Me.Label4.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(13, 20)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = ";"
        '
        'LAnrMonSimRING
        '
        Me.LAnrMonSimRING.AutoSize = True
        Me.LAnrMonSimRING.Location = New System.Drawing.Point(190, 30)
        Me.LAnrMonSimRING.Margin = New System.Windows.Forms.Padding(1, 0, 1, 0)
        Me.LAnrMonSimRING.Name = "LAnrMonSimRING"
        Me.LAnrMonSimRING.Size = New System.Drawing.Size(58, 20)
        Me.LAnrMonSimRING.TabIndex = 4
        Me.LAnrMonSimRING.Text = ";RING;"
        '
        'TBAnrMonSimRINGID
        '
        Me.TBAnrMonSimRINGID.Location = New System.Drawing.Point(250, 27)
        Me.TBAnrMonSimRINGID.Margin = New System.Windows.Forms.Padding(1, 3, 1, 3)
        Me.TBAnrMonSimRINGID.Name = "TBAnrMonSimRINGID"
        Me.TBAnrMonSimRINGID.Size = New System.Drawing.Size(32, 26)
        Me.TBAnrMonSimRINGID.TabIndex = 3
        Me.TBAnrMonSimRINGID.Text = "0"
        Me.TBAnrMonSimRINGID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'LAnrMonSimLabelRING
        '
        Me.LAnrMonSimLabelRING.AutoSize = True
        Me.LAnrMonSimLabelRING.Location = New System.Drawing.Point(6, 66)
        Me.LAnrMonSimLabelRING.Name = "LAnrMonSimLabelRING"
        Me.LAnrMonSimLabelRING.Size = New System.Drawing.Size(390, 20)
        Me.LAnrMonSimLabelRING.TabIndex = 2
        Me.LAnrMonSimLabelRING.Text = "23.06.18 13:20:24;RING;1;0123456789;987654;SIP4;"
        '
        'DTPAnrMonSimRING
        '
        Me.DTPAnrMonSimRING.CustomFormat = "dd.MM.yy HH:mm:ss"
        Me.DTPAnrMonSimRING.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DTPAnrMonSimRING.Location = New System.Drawing.Point(6, 25)
        Me.DTPAnrMonSimRING.Margin = New System.Windows.Forms.Padding(3, 3, 1, 3)
        Me.DTPAnrMonSimRING.Name = "DTPAnrMonSimRING"
        Me.DTPAnrMonSimRING.Size = New System.Drawing.Size(183, 26)
        Me.DTPAnrMonSimRING.TabIndex = 1
        Me.DTPAnrMonSimRING.Value = New Date(2019, 6, 29, 15, 7, 37, 0)
        '
        'BAnrMonSimRING
        '
        Me.BAnrMonSimRING.Location = New System.Drawing.Point(720, 25)
        Me.BAnrMonSimRING.Name = "BAnrMonSimRING"
        Me.BAnrMonSimRING.Size = New System.Drawing.Size(122, 61)
        Me.BAnrMonSimRING.TabIndex = 0
        Me.BAnrMonSimRING.Text = "RING"
        Me.BAnrMonSimRING.UseVisualStyleBackColor = True
        '
        'PKontaktsuche2
        '
        Me.PKontaktsuche2.Controls.Add(Me.Label1)
        Me.PKontaktsuche2.Controls.Add(Me.GBRWS)
        Me.PKontaktsuche2.Controls.Add(Me.GBIndizierung)
        Me.PKontaktsuche2.Controls.Add(Me.CBKontaktSucheFritzBox)
        Me.PKontaktsuche2.Controls.Add(Me.BKontOrdLaden)
        Me.PKontaktsuche2.Controls.Add(Me.TreeViewKontakte)
        Me.PKontaktsuche2.Location = New System.Drawing.Point(4, 29)
        Me.PKontaktsuche2.Name = "PKontaktsuche2"
        Me.PKontaktsuche2.Padding = New System.Windows.Forms.Padding(3)
        Me.PKontaktsuche2.Size = New System.Drawing.Size(860, 458)
        Me.PKontaktsuche2.TabIndex = 15
        Me.PKontaktsuche2.Text = "Kontaktsuche"
        Me.PKontaktsuche2.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.Label1.Location = New System.Drawing.Point(3, 3)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(854, 54)
        Me.Label1.TabIndex = 38
        Me.Label1.Text = "Einstellungen für die Kontaktsuche"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'GBRWS
        '
        Me.GBRWS.Controls.Add(Me.BRWSTest)
        Me.GBRWS.Controls.Add(Me.TBRWSTest)
        Me.GBRWS.Controls.Add(Me.CBRWSIndex)
        Me.GBRWS.Controls.Add(Me.CBKErstellen)
        Me.GBRWS.Controls.Add(Me.CBRWS)
        Me.GBRWS.Location = New System.Drawing.Point(0, 330)
        Me.GBRWS.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBRWS.Name = "GBRWS"
        Me.GBRWS.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBRWS.Size = New System.Drawing.Size(461, 128)
        Me.GBRWS.TabIndex = 37
        Me.GBRWS.TabStop = False
        Me.GBRWS.Text = "Rückwärtssuche (RWS)"
        '
        'BRWSTest
        '
        Me.BRWSTest.Location = New System.Drawing.Point(285, 61)
        Me.BRWSTest.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BRWSTest.Name = "BRWSTest"
        Me.BRWSTest.Size = New System.Drawing.Size(168, 43)
        Me.BRWSTest.TabIndex = 8
        Me.BRWSTest.Text = "Starte Test"
        Me.BRWSTest.UseVisualStyleBackColor = True
        '
        'CBRWS
        '
        Me.CBRWS.AutoSize = True
        Me.CBRWS.Location = New System.Drawing.Point(7, 29)
        Me.CBRWS.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBRWS.MinimumSize = New System.Drawing.Size(165, 0)
        Me.CBRWS.Name = "CBRWS"
        Me.CBRWS.Size = New System.Drawing.Size(165, 24)
        Me.CBRWS.TabIndex = 0
        Me.CBRWS.Text = "DasÖrtliche"
        Me.CBRWS.UseVisualStyleBackColor = True
        '
        'GBIndizierung
        '
        Me.GBIndizierung.Controls.Add(Me.LabelAnzahl)
        Me.GBIndizierung.Controls.Add(Me.RadioButtonEntfernen)
        Me.GBIndizierung.Controls.Add(Me.RadioButtonErstelle)
        Me.GBIndizierung.Controls.Add(Me.BIndizierungAbbrechen)
        Me.GBIndizierung.Controls.Add(Me.BIndizierungStart)
        Me.GBIndizierung.Controls.Add(Me.ProgressBarIndex)
        Me.GBIndizierung.Location = New System.Drawing.Point(0, 156)
        Me.GBIndizierung.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBIndizierung.Name = "GBIndizierung"
        Me.GBIndizierung.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBIndizierung.Size = New System.Drawing.Size(461, 164)
        Me.GBIndizierung.TabIndex = 36
        Me.GBIndizierung.TabStop = False
        Me.GBIndizierung.Text = "Kontaktindizierung"
        '
        'LabelAnzahl
        '
        Me.LabelAnzahl.AutoSize = True
        Me.LabelAnzahl.Location = New System.Drawing.Point(6, 130)
        Me.LabelAnzahl.Name = "LabelAnzahl"
        Me.LabelAnzahl.Size = New System.Drawing.Size(165, 20)
        Me.LabelAnzahl.TabIndex = 11
        Me.LabelAnzahl.Text = "Status der Indizierung"
        '
        'RadioButtonEntfernen
        '
        Me.RadioButtonEntfernen.AutoSize = True
        Me.RadioButtonEntfernen.Location = New System.Drawing.Point(132, 38)
        Me.RadioButtonEntfernen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RadioButtonEntfernen.MinimumSize = New System.Drawing.Size(110, 0)
        Me.RadioButtonEntfernen.Name = "RadioButtonEntfernen"
        Me.RadioButtonEntfernen.Size = New System.Drawing.Size(110, 24)
        Me.RadioButtonEntfernen.TabIndex = 8
        Me.RadioButtonEntfernen.TabStop = True
        Me.RadioButtonEntfernen.Text = "entfernen"
        Me.RadioButtonEntfernen.UseVisualStyleBackColor = True
        '
        'RadioButtonErstelle
        '
        Me.RadioButtonErstelle.AutoSize = True
        Me.RadioButtonErstelle.Checked = True
        Me.RadioButtonErstelle.Location = New System.Drawing.Point(14, 38)
        Me.RadioButtonErstelle.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RadioButtonErstelle.MinimumSize = New System.Drawing.Size(110, 0)
        Me.RadioButtonErstelle.Name = "RadioButtonErstelle"
        Me.RadioButtonErstelle.Size = New System.Drawing.Size(110, 24)
        Me.RadioButtonErstelle.TabIndex = 7
        Me.RadioButtonErstelle.TabStop = True
        Me.RadioButtonErstelle.Text = "erstellen"
        Me.RadioButtonErstelle.UseVisualStyleBackColor = True
        '
        'BIndizierungAbbrechen
        '
        Me.BIndizierungAbbrechen.Enabled = False
        Me.BIndizierungAbbrechen.Location = New System.Drawing.Point(285, 29)
        Me.BIndizierungAbbrechen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BIndizierungAbbrechen.Name = "BIndizierungAbbrechen"
        Me.BIndizierungAbbrechen.Size = New System.Drawing.Size(168, 43)
        Me.BIndizierungAbbrechen.TabIndex = 10
        Me.BIndizierungAbbrechen.Text = "Abbrechen"
        Me.BIndizierungAbbrechen.UseVisualStyleBackColor = True
        '
        'BIndizierungStart
        '
        Me.BIndizierungStart.Location = New System.Drawing.Point(285, 82)
        Me.BIndizierungStart.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BIndizierungStart.Name = "BIndizierungStart"
        Me.BIndizierungStart.Size = New System.Drawing.Size(168, 43)
        Me.BIndizierungStart.TabIndex = 9
        Me.BIndizierungStart.Text = "Start"
        Me.BIndizierungStart.UseVisualStyleBackColor = True
        '
        'ProgressBarIndex
        '
        Me.ProgressBarIndex.Location = New System.Drawing.Point(8, 82)
        Me.ProgressBarIndex.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ProgressBarIndex.Name = "ProgressBarIndex"
        Me.ProgressBarIndex.Size = New System.Drawing.Size(260, 43)
        Me.ProgressBarIndex.TabIndex = 10
        '
        'CBKontaktSucheFritzBox
        '
        Me.CBKontaktSucheFritzBox.AutoSize = True
        Me.CBKontaktSucheFritzBox.Location = New System.Drawing.Point(7, 104)
        Me.CBKontaktSucheFritzBox.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBKontaktSucheFritzBox.MinimumSize = New System.Drawing.Size(350, 0)
        Me.CBKontaktSucheFritzBox.Name = "CBKontaktSucheFritzBox"
        Me.CBKontaktSucheFritzBox.Size = New System.Drawing.Size(350, 24)
        Me.CBKontaktSucheFritzBox.TabIndex = 35
        Me.CBKontaktSucheFritzBox.Text = "Die Fritz!Box Telefonbüchern durchsuchen"
        Me.CBKontaktSucheFritzBox.UseVisualStyleBackColor = True
        '
        'BKontOrdLaden
        '
        Me.BKontOrdLaden.Location = New System.Drawing.Point(468, 60)
        Me.BKontOrdLaden.Name = "BKontOrdLaden"
        Me.BKontOrdLaden.Size = New System.Drawing.Size(392, 43)
        Me.BKontOrdLaden.TabIndex = 1
        Me.BKontOrdLaden.Text = "Outlook-Kontaktordner laden..."
        Me.BKontOrdLaden.UseVisualStyleBackColor = True
        '
        'TreeViewKontakte
        '
        Me.TreeViewKontakte.Location = New System.Drawing.Point(468, 111)
        Me.TreeViewKontakte.Name = "TreeViewKontakte"
        Me.TreeViewKontakte.Size = New System.Drawing.Size(392, 347)
        Me.TreeViewKontakte.TabIndex = 0
        '
        'PLogging
        '
        Me.PLogging.Controls.Add(Me.LMinLogLevel)
        Me.PLogging.Controls.Add(Me.CBoxMinLogLevel)
        Me.PLogging.Controls.Add(Me.GBLogging)
        Me.PLogging.Controls.Add(Me.Label23)
        Me.PLogging.Location = New System.Drawing.Point(4, 29)
        Me.PLogging.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PLogging.Name = "PLogging"
        Me.PLogging.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PLogging.Size = New System.Drawing.Size(860, 458)
        Me.PLogging.TabIndex = 11
        Me.PLogging.Text = "Logging"
        Me.PLogging.UseVisualStyleBackColor = True
        '
        'LMinLogLevel
        '
        Me.LMinLogLevel.AutoSize = True
        Me.LMinLogLevel.Location = New System.Drawing.Point(7, 84)
        Me.LMinLogLevel.Name = "LMinLogLevel"
        Me.LMinLogLevel.Size = New System.Drawing.Size(141, 20)
        Me.LMinLogLevel.TabIndex = 28
        Me.LMinLogLevel.Text = "Minimales Loglevel"
        '
        'CBoxMinLogLevel
        '
        Me.CBoxMinLogLevel.FormattingEnabled = True
        Me.CBoxMinLogLevel.Items.AddRange(New Object() {"Fatal", "Error", "Warn", "Info", "Debug", "Trace"})
        Me.CBoxMinLogLevel.Location = New System.Drawing.Point(173, 81)
        Me.CBoxMinLogLevel.Name = "CBoxMinLogLevel"
        Me.CBoxMinLogLevel.Size = New System.Drawing.Size(121, 28)
        Me.CBoxMinLogLevel.TabIndex = 27
        '
        'GBLogging
        '
        Me.GBLogging.Controls.Add(Me.LinkLogFile)
        Me.GBLogging.Controls.Add(Me.TBLogging)
        Me.GBLogging.Location = New System.Drawing.Point(0, 117)
        Me.GBLogging.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBLogging.Name = "GBLogging"
        Me.GBLogging.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBLogging.Size = New System.Drawing.Size(855, 335)
        Me.GBLogging.TabIndex = 26
        Me.GBLogging.TabStop = False
        '
        'LinkLogFile
        '
        Me.LinkLogFile.Location = New System.Drawing.Point(3, 307)
        Me.LinkLogFile.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LinkLogFile.MaximumSize = New System.Drawing.Size(848, 23)
        Me.LinkLogFile.Name = "LinkLogFile"
        Me.LinkLogFile.Size = New System.Drawing.Size(848, 23)
        Me.LinkLogFile.TabIndex = 26
        Me.LinkLogFile.TabStop = True
        Me.LinkLogFile.Text = "Link zur Logfile"
        '
        'TBLogging
        '
        Me.TBLogging.Dock = System.Windows.Forms.DockStyle.Top
        Me.TBLogging.Font = New System.Drawing.Font("Consolas", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TBLogging.Location = New System.Drawing.Point(4, 24)
        Me.TBLogging.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBLogging.Multiline = True
        Me.TBLogging.Name = "TBLogging"
        Me.TBLogging.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TBLogging.Size = New System.Drawing.Size(847, 278)
        Me.TBLogging.TabIndex = 25
        '
        'Label23
        '
        Me.Label23.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label23.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.Location = New System.Drawing.Point(4, 5)
        Me.Label23.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(852, 54)
        Me.Label23.TabIndex = 24
        Me.Label23.Text = "Logging"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BXML
        '
        Me.BXML.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BXML.Location = New System.Drawing.Point(696, 5)
        Me.BXML.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BXML.Name = "BXML"
        Me.BXML.Size = New System.Drawing.Size(168, 43)
        Me.BXML.TabIndex = 27
        Me.BXML.Text = "Einstellungsdatei"
        Me.BXML.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 5
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.BOK, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.BXML, 4, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.BApply, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.BAbbruch, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.BReset, 3, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(4, 506)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(868, 52)
        Me.TableLayoutPanel1.TabIndex = 28
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 1
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.TableLayoutPanel1, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.FBDB_MP, 0, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 2
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(876, 563)
        Me.TableLayoutPanel2.TabIndex = 29
        '
        'FormCfg
        '
        Me.AcceptButton = Me.BOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BAbbruch
        Me.ClientSize = New System.Drawing.Size(876, 563)
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FormCfg"
        Me.Text = "Einstellungen für das Fritz!Box Telefon-Dingsbums"
        Me.PGrundeinstellungen.ResumeLayout(False)
        Me.GBFormatierungTelefonnummern.ResumeLayout(False)
        Me.GBFormatierungTelefonnummern.PerformLayout()
        Me.GBEinstellungWählhilfe.ResumeLayout(False)
        Me.GBEinstellungWählhilfe.PerformLayout()
        Me.GBErforderlicheAngaben.ResumeLayout(False)
        Me.GBErforderlicheAngaben.PerformLayout()
        Me.PInfo.ResumeLayout(False)
        Me.PInfo.PerformLayout()
        Me.PTelefone.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        CType(Me.DGVTelList, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PAnrufmonitor.ResumeLayout(False)
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.GBJournal.ResumeLayout(False)
        Me.GBJournal.PerformLayout()
        Me.GBAnrListeMain.ResumeLayout(False)
        Me.GBAnrListeMain.PerformLayout()
        Me.GBAnrMonMain.ResumeLayout(False)
        Me.GBAnrMonMain.PerformLayout()
        Me.PanelAnrMon.ResumeLayout(False)
        Me.PanelAnrMon.PerformLayout()
        Me.FBDB_MP.ResumeLayout(False)
        Me.PAnrMonSim.ResumeLayout(False)
        Me.GBoxAnrMonDISCONNECT.ResumeLayout(False)
        Me.GBoxAnrMonDISCONNECT.PerformLayout()
        Me.GBoxAnrMonCONNECT.ResumeLayout(False)
        Me.GBoxAnrMonCONNECT.PerformLayout()
        Me.GBoxAnrMonCALL.ResumeLayout(False)
        Me.GBoxAnrMonCALL.PerformLayout()
        Me.GBoxAnrMonRING.ResumeLayout(False)
        Me.GBoxAnrMonRING.PerformLayout()
        Me.PKontaktsuche2.ResumeLayout(False)
        Me.PKontaktsuche2.PerformLayout()
        Me.GBRWS.ResumeLayout(False)
        Me.GBRWS.PerformLayout()
        Me.GBIndizierung.ResumeLayout(False)
        Me.GBIndizierung.PerformLayout()
        Me.PLogging.ResumeLayout(False)
        Me.PLogging.PerformLayout()
        Me.GBLogging.ResumeLayout(False)
        Me.GBLogging.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BReset As System.Windows.Forms.Button
    Friend WithEvents BAbbruch As System.Windows.Forms.Button
    Friend WithEvents BApply As System.Windows.Forms.Button
    Friend WithEvents BOK As System.Windows.Forms.Button
    Friend WithEvents ToolTipFBDBConfig As System.Windows.Forms.ToolTip
    Friend WithEvents PInfo As System.Windows.Forms.TabPage
    Friend WithEvents LinkEmail As System.Windows.Forms.LinkLabel
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents LVersion As System.Windows.Forms.Label
    Friend WithEvents RichTextBox1 As System.Windows.Forms.RichTextBox
    Friend WithEvents PTelefone As System.Windows.Forms.TabPage
    Friend WithEvents PAnrufmonitor As System.Windows.Forms.TabPage
    Friend WithEvents CBAnrMonContactImage As System.Windows.Forms.CheckBox
    Friend WithEvents CBShowMSN As System.Windows.Forms.CheckBox
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents GBAnrMonMain As System.Windows.Forms.GroupBox
    Friend WithEvents PanelAnrMon As System.Windows.Forms.Panel
    Friend WithEvents CBAnrMonAuto As System.Windows.Forms.CheckBox
    Friend WithEvents CBAutoClose As System.Windows.Forms.CheckBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TBEnblDauer As System.Windows.Forms.TextBox
    Friend WithEvents CLBTelNr As System.Windows.Forms.CheckedListBox
    Friend WithEvents LEnblDauer As System.Windows.Forms.Label
    Friend WithEvents CBUseAnrMon As System.Windows.Forms.CheckBox
    Friend WithEvents PGrundeinstellungen As System.Windows.Forms.TabPage
    Friend WithEvents GBFormatierungTelefonnummern As System.Windows.Forms.GroupBox
    Friend WithEvents CBIgnoTelNrFormat As System.Windows.Forms.CheckBox
    Friend WithEvents LTelNrMaske As System.Windows.Forms.Label
    Friend WithEvents TBTelNrMaske As System.Windows.Forms.TextBox
    Friend WithEvents CBintl As System.Windows.Forms.CheckBox
    Friend WithEvents CBTelNrGruppieren As System.Windows.Forms.CheckBox
    Friend WithEvents GBEinstellungWählhilfe As System.Windows.Forms.GroupBox
    Friend WithEvents CBCheckMobil As System.Windows.Forms.CheckBox
    Friend WithEvents CBForceDialLKZ As System.Windows.Forms.CheckBox
    Friend WithEvents CBCbCunterbinden As System.Windows.Forms.CheckBox
    Friend WithEvents LAmtsholung As System.Windows.Forms.Label
    Friend WithEvents TBAmt As System.Windows.Forms.TextBox
    Friend WithEvents GBErforderlicheAngaben As System.Windows.Forms.GroupBox
    Friend WithEvents LLandeskennzahl As System.Windows.Forms.Label
    Friend WithEvents LOrtskennzahl As System.Windows.Forms.Label
    Friend WithEvents LPasswort As System.Windows.Forms.Label
    Friend WithEvents TBFBAdr As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents FBDB_MP As System.Windows.Forms.TabControl
    Friend WithEvents LinkForum As System.Windows.Forms.LinkLabel
    Friend WithEvents CBDialPort As System.Windows.Forms.CheckBox
    Friend WithEvents CBForceFBAdr As System.Windows.Forms.CheckBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents LBenutzer As System.Windows.Forms.Label
    Friend WithEvents TBBenutzer As System.Windows.Forms.TextBox
    Friend WithEvents LinkHomepage As System.Windows.Forms.LinkLabel
    Friend WithEvents PLogging As System.Windows.Forms.TabPage
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents TBLogging As System.Windows.Forms.TextBox
    Friend WithEvents GBLogging As System.Windows.Forms.GroupBox
    Friend WithEvents LinkLogFile As System.Windows.Forms.LinkLabel
    Friend WithEvents TBPasswort As System.Windows.Forms.MaskedTextBox
    Friend WithEvents BXML As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents BTelefonliste As System.Windows.Forms.Button
    Friend WithEvents CBAnrMonZeigeKontakt As System.Windows.Forms.CheckBox
    Friend WithEvents BArbeitsverzeichnis As System.Windows.Forms.Button
    Friend WithEvents TBOrtsKZ As System.Windows.Forms.TextBox
    Friend WithEvents TBLandesKZ As System.Windows.Forms.TextBox
    Friend WithEvents BTestLogin As System.Windows.Forms.Button
    Friend WithEvents CBAnrMonCloseAtDISSCONNECT As System.Windows.Forms.CheckBox
    Friend WithEvents PAnrMonSim As Windows.Forms.TabPage
    Friend WithEvents GBoxAnrMonDISCONNECT As Windows.Forms.GroupBox
    Friend WithEvents BAnrMonSimDISCONNECT As Windows.Forms.Button
    Friend WithEvents GBoxAnrMonCONNECT As Windows.Forms.GroupBox
    Friend WithEvents BAnrMonSimCONNECT As Windows.Forms.Button
    Friend WithEvents GBoxAnrMonCALL As Windows.Forms.GroupBox
    Friend WithEvents BAnrMonSimCALL As Windows.Forms.Button
    Friend WithEvents GBoxAnrMonRING As Windows.Forms.GroupBox
    Friend WithEvents BAnrMonSimRING As Windows.Forms.Button
    Friend WithEvents LAnrMonSimLabelRING As Windows.Forms.Label
    Friend WithEvents DTPAnrMonSimRING As Windows.Forms.DateTimePicker
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents LAnrMonSimRING As Windows.Forms.Label
    Friend WithEvents TBAnrMonSimRINGID As Windows.Forms.TextBox
    Friend WithEvents Label8 As Windows.Forms.Label
    Friend WithEvents CBoxAnrMonSimRINGSIPID As Windows.Forms.ComboBox
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents CBoxAnrMonSimRINGEigTelNr As Windows.Forms.ComboBox
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents TBAnrMonSimRINGAugTelNr As Windows.Forms.TextBox
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents CBoxAnrMonSimCALLSIPID As Windows.Forms.ComboBox
    Friend WithEvents Label9 As Windows.Forms.Label
    Friend WithEvents CBoxAnrMonSimCALLEigTelNr As Windows.Forms.ComboBox
    Friend WithEvents Label11 As Windows.Forms.Label
    Friend WithEvents TBAnrMonSimCALLAugTelNr As Windows.Forms.TextBox
    Friend WithEvents Label12 As Windows.Forms.Label
    Friend WithEvents LAnrMonSimCALL As Windows.Forms.Label
    Friend WithEvents TBAnrMonSimCALLID As Windows.Forms.TextBox
    Friend WithEvents LAnrMonSimLabelCALL As Windows.Forms.Label
    Friend WithEvents DTPAnrMonSimCALL As Windows.Forms.DateTimePicker
    Friend WithEvents Label14 As Windows.Forms.Label
    Friend WithEvents CBoxAnrMonSimCALLNSTID As Windows.Forms.ComboBox
    Friend WithEvents CBoxAnrMonSimCONNECTNSTID As Windows.Forms.ComboBox
    Friend WithEvents Label18 As Windows.Forms.Label
    Friend WithEvents TBAnrMonSimCONNECTAugTelNr As Windows.Forms.TextBox
    Friend WithEvents Label24 As Windows.Forms.Label
    Friend WithEvents LAnrMonSimCONNECT As Windows.Forms.Label
    Friend WithEvents TBAnrMonSimCONNECTID As Windows.Forms.TextBox
    Friend WithEvents LAnrMonSimLabelCONNECT As Windows.Forms.Label
    Friend WithEvents DTPAnrMonSimCONNECT As Windows.Forms.DateTimePicker
    Friend WithEvents Label19 As Windows.Forms.Label
    Friend WithEvents Label26 As Windows.Forms.Label
    Friend WithEvents Label27 As Windows.Forms.Label
    Friend WithEvents LAnrMonSimDISCONNECT As Windows.Forms.Label
    Friend WithEvents TBAnrMonSimDISCONNECTID As Windows.Forms.TextBox
    Friend WithEvents LAnrMonSimLabelDISCONNECT As Windows.Forms.Label
    Friend WithEvents DTPAnrMonSimDISCONNECT As Windows.Forms.DateTimePicker
    Friend WithEvents TBAnrMonSimDISCONNECTDauer As Windows.Forms.TextBox
    Friend WithEvents GroupBox6 As Windows.Forms.GroupBox
    Friend WithEvents TBNumEntryList As Windows.Forms.TextBox
    Friend WithEvents LNumEntryList As Windows.Forms.Label
    Friend WithEvents GBJournal As Windows.Forms.GroupBox
    Friend WithEvents CBJournal As Windows.Forms.CheckBox
    Friend WithEvents GBAnrListeMain As Windows.Forms.GroupBox
    Friend WithEvents CBAnrListeShowAnrMon As Windows.Forms.CheckBox
    Friend WithEvents CBAutoAnrList As Windows.Forms.CheckBox
    Friend WithEvents CBAnrListeUpdateCallLists As Windows.Forms.CheckBox
    Friend WithEvents LMinLogLevel As Windows.Forms.Label
    Friend WithEvents CBoxMinLogLevel As Windows.Forms.ComboBox
    Friend WithEvents CBCloseWClient As Windows.Forms.CheckBox
    Friend WithEvents TBWClientEnblDauer As Windows.Forms.TextBox
    Friend WithEvents LWClientEnblDauer As Windows.Forms.Label
    Friend WithEvents DGVTelList As FBoxDataGridView
    Friend WithEvents PKontaktsuche2 As Windows.Forms.TabPage
    Friend WithEvents TreeViewKontakte As Windows.Forms.TreeView
    Friend WithEvents BKontOrdLaden As Windows.Forms.Button
    Friend WithEvents CBKontaktSucheFritzBox As Windows.Forms.CheckBox
    Friend WithEvents GBIndizierung As Windows.Forms.GroupBox
    Friend WithEvents RadioButtonEntfernen As Windows.Forms.RadioButton
    Friend WithEvents RadioButtonErstelle As Windows.Forms.RadioButton
    Friend WithEvents BIndizierungAbbrechen As Windows.Forms.Button
    Friend WithEvents BIndizierungStart As Windows.Forms.Button
    Friend WithEvents ProgressBarIndex As Windows.Forms.ProgressBar
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents GBRWS As Windows.Forms.GroupBox
    Friend WithEvents BRWSTest As Windows.Forms.Button
    Friend WithEvents TBRWSTest As Windows.Forms.TextBox
    Friend WithEvents CBRWSIndex As Windows.Forms.CheckBox
    Friend WithEvents CBKErstellen As Windows.Forms.CheckBox
    Friend WithEvents CBRWS As Windows.Forms.CheckBox
    Friend WithEvents LabelAnzahl As Windows.Forms.Label
End Class
