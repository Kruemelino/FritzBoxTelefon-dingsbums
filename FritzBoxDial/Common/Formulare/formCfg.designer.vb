<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formCfg
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(formCfg))
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.BReset = New System.Windows.Forms.Button()
        Me.BAbbruch = New System.Windows.Forms.Button()
        Me.BApply = New System.Windows.Forms.Button()
        Me.BOK = New System.Windows.Forms.Button()
        Me.ToolTipFBDBConfig = New System.Windows.Forms.ToolTip(Me.components)
        Me.CBKErstellen = New System.Windows.Forms.CheckBox()
        Me.CBRWSIndex = New System.Windows.Forms.CheckBox()
        Me.CBKHO = New System.Windows.Forms.CheckBox()
        Me.CBIndexAus = New System.Windows.Forms.CheckBox()
        Me.CBAnrMonTransp = New System.Windows.Forms.CheckBox()
        Me.CBAnrMonMove = New System.Windows.Forms.CheckBox()
        Me.CBShowMSN = New System.Windows.Forms.CheckBox()
        Me.PGrundeinstellungen = New System.Windows.Forms.TabPage()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.CBIgnoTelNrFormat = New System.Windows.Forms.CheckBox()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.TBTelNrMaske = New System.Windows.Forms.TextBox()
        Me.CBintl = New System.Windows.Forms.CheckBox()
        Me.CBTelNrGruppieren = New System.Windows.Forms.CheckBox()
        Me.GroupBoxStoppUhr = New System.Windows.Forms.GroupBox()
        Me.CBStoppUhrIgnIntFax = New System.Windows.Forms.CheckBox()
        Me.BStoppUhrAnzeigen = New System.Windows.Forms.Button()
        Me.LabelStoppUhr = New System.Windows.Forms.Label()
        Me.TBStoppUhr = New System.Windows.Forms.TextBox()
        Me.CBStoppUhrAusblenden = New System.Windows.Forms.CheckBox()
        Me.CBStoppUhrEinblenden = New System.Windows.Forms.CheckBox()
        Me.Frame3 = New System.Windows.Forms.GroupBox()
        Me.CBCallByCall = New System.Windows.Forms.CheckBox()
        Me.CBDialPort = New System.Windows.Forms.CheckBox()
        Me.CBCheckMobil = New System.Windows.Forms.CheckBox()
        Me.CBVoIPBuster = New System.Windows.Forms.CheckBox()
        Me.CBCbCunterbinden = New System.Windows.Forms.CheckBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TBAmt = New System.Windows.Forms.TextBox()
        Me.FrameErforderlich = New System.Windows.Forms.GroupBox()
        Me.BTestLogin = New System.Windows.Forms.Button()
        Me.TBPasswort = New System.Windows.Forms.MaskedTextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TBLandesVW = New System.Windows.Forms.TextBox()
        Me.TBVorwahl = New System.Windows.Forms.TextBox()
        Me.TBBenutzer = New System.Windows.Forms.TextBox()
        Me.CBForceFBAddr = New System.Windows.Forms.CheckBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblTBPasswort = New System.Windows.Forms.Label()
        Me.TBFBAdr = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.BAnrMonTest = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TBRWSTest = New System.Windows.Forms.TextBox()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.CBAutoAnrList = New System.Windows.Forms.CheckBox()
        Me.TVOutlookContact = New System.Windows.Forms.TreeView()
        Me.TBAnrBeantworterTimeout = New System.Windows.Forms.TextBox()
        Me.LAnrBeantworterTimeout = New System.Windows.Forms.Label()
        Me.BProbleme = New System.Windows.Forms.Button()
        Me.BZwischenablage = New System.Windows.Forms.Button()
        Me.BStartDebug = New System.Windows.Forms.Button()
        Me.TBNumEntryList = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.RBFBComUPnP = New System.Windows.Forms.RadioButton()
        Me.RBFBComWeb = New System.Windows.Forms.RadioButton()
        Me.BManLoad = New System.Windows.Forms.Button()
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
        Me.LTelStatus = New System.Windows.Forms.Label()
        Me.BResetStat = New System.Windows.Forms.Button()
        Me.BTelefonliste = New System.Windows.Forms.Button()
        Me.TBAnderes = New System.Windows.Forms.Label()
        Me.TBSchließZeit = New System.Windows.Forms.Label()
        Me.TBReset = New System.Windows.Forms.Label()
        Me.TelList = New System.Windows.Forms.DataGridView()
        Me.ColumnStandardTelefon = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.Nr = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.dialCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Typ = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Telefonname = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.OutNr = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Eingehend = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Ausgehend = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Gesamt = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.PKontaktsuche = New System.Windows.Forms.TabPage()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBoxIndex = New System.Windows.Forms.GroupBox()
        Me.CBTelFormKorr = New System.Windows.Forms.CheckBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.RadioButtonEntfernen = New System.Windows.Forms.RadioButton()
        Me.RadioButtonErstelle = New System.Windows.Forms.RadioButton()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.LabelAnzahl = New System.Windows.Forms.Label()
        Me.BIndizierungAbbrechen = New System.Windows.Forms.Button()
        Me.BIndizierungStart = New System.Windows.Forms.Button()
        Me.ProgressBarIndex = New System.Windows.Forms.ProgressBar()
        Me.GroupBoxRWS = New System.Windows.Forms.GroupBox()
        Me.BRWSTest = New System.Windows.Forms.Button()
        Me.LRWSTest = New System.Windows.Forms.Label()
        Me.ComboBoxRWS = New System.Windows.Forms.ComboBox()
        Me.CBRWS = New System.Windows.Forms.CheckBox()
        Me.PAnrufmonitor = New System.Windows.Forms.TabPage()
        Me.GBoxAnrMonLayout = New System.Windows.Forms.GroupBox()
        Me.LAnrMonMoveDirection = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.CBoxAnrMonMoveDirection = New System.Windows.Forms.ComboBox()
        Me.CBoxAnrMonStartPosition = New System.Windows.Forms.ComboBox()
        Me.CBAnrMonContactImage = New System.Windows.Forms.CheckBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.TBAnrMonY = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.TBAnrMonX = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.TBAnrMonMoveGeschwindigkeit = New System.Windows.Forms.TrackBar()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.GBoxAnrMonMain = New System.Windows.Forms.GroupBox()
        Me.PanelAnrMon = New System.Windows.Forms.Panel()
        Me.CBNote = New System.Windows.Forms.CheckBox()
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
        Me.PDiverses = New System.Windows.Forms.TabPage()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.GBoxJournal = New System.Windows.Forms.GroupBox()
        Me.CBJournal = New System.Windows.Forms.CheckBox()
        Me.GboxAnrListeMain = New System.Windows.Forms.GroupBox()
        Me.CBAnrListeShowAnrMon = New System.Windows.Forms.CheckBox()
        Me.CBAnrListeUpdateJournal = New System.Windows.Forms.CheckBox()
        Me.CBAnrListeUpdateCallLists = New System.Windows.Forms.CheckBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.PPhoner = New System.Windows.Forms.TabPage()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.PanelPhonerAktiv = New System.Windows.Forms.Panel()
        Me.LabelPhoner = New System.Windows.Forms.Label()
        Me.PanelPhoner = New System.Windows.Forms.Panel()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.CBPhonerAnrMon = New System.Windows.Forms.CheckBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.CBPhoner = New System.Windows.Forms.CheckBox()
        Me.ComboBoxPhonerSIP = New System.Windows.Forms.ComboBox()
        Me.TBPhonerPasswort = New System.Windows.Forms.MaskedTextBox()
        Me.LPassworPhoner = New System.Windows.Forms.Label()
        Me.BPhoner = New System.Windows.Forms.Button()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.LinkPhoner = New System.Windows.Forms.LinkLabel()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.PLogging = New System.Windows.Forms.TabPage()
        Me.GBLogging = New System.Windows.Forms.GroupBox()
        Me.BLogging = New System.Windows.Forms.Button()
        Me.LinkLogFile = New System.Windows.Forms.LinkLabel()
        Me.TBLogging = New System.Windows.Forms.TextBox()
        Me.CBLogFile = New System.Windows.Forms.CheckBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.PDebug = New System.Windows.Forms.TabPage()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.TBDiagnose = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.BXML = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.PGrundeinstellungen.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBoxStoppUhr.SuspendLayout()
        Me.Frame3.SuspendLayout()
        Me.FrameErforderlich.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.PInfo.SuspendLayout()
        Me.PTelefone.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.TelList, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PKontaktsuche.SuspendLayout()
        Me.GroupBoxIndex.SuspendLayout()
        Me.GroupBoxRWS.SuspendLayout()
        Me.PAnrufmonitor.SuspendLayout()
        Me.GBoxAnrMonLayout.SuspendLayout()
        CType(Me.TBAnrMonMoveGeschwindigkeit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GBoxAnrMonMain.SuspendLayout()
        Me.PanelAnrMon.SuspendLayout()
        Me.FBDB_MP.SuspendLayout()
        Me.PDiverses.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.GBoxJournal.SuspendLayout()
        Me.GboxAnrListeMain.SuspendLayout()
        Me.PPhoner.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.PanelPhoner.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.PLogging.SuspendLayout()
        Me.GBLogging.SuspendLayout()
        Me.PDebug.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
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
        'CBKErstellen
        '
        Me.CBKErstellen.AutoSize = True
        Me.CBKErstellen.Enabled = False
        Me.CBKErstellen.Location = New System.Drawing.Point(9, 106)
        Me.CBKErstellen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBKErstellen.Name = "CBKErstellen"
        Me.CBKErstellen.Size = New System.Drawing.Size(321, 24)
        Me.CBKErstellen.TabIndex = 2
        Me.CBKErstellen.Text = "Kontakt bei erfolgreicher Suche erstellen"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBKErstellen, "Nach erfolgreicher Rückwärtssuche, wird bei dieser Einstellung ein neuer Kontakt " &
        "erstellt.")
        Me.CBKErstellen.UseVisualStyleBackColor = True
        '
        'CBRWSIndex
        '
        Me.CBRWSIndex.AutoSize = True
        Me.CBRWSIndex.Enabled = False
        Me.CBRWSIndex.Location = New System.Drawing.Point(9, 142)
        Me.CBRWSIndex.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBRWSIndex.Name = "CBRWSIndex"
        Me.CBRWSIndex.Size = New System.Drawing.Size(362, 24)
        Me.CBRWSIndex.TabIndex = 3
        Me.CBRWSIndex.Text = "Speichere Ergebnisse der Rückwärtssuche ab"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBRWSIndex, resources.GetString("CBRWSIndex.ToolTip"))
        Me.CBRWSIndex.UseVisualStyleBackColor = True
        '
        'CBKHO
        '
        Me.CBKHO.AutoSize = True
        Me.CBKHO.Location = New System.Drawing.Point(9, 29)
        Me.CBKHO.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBKHO.Name = "CBKHO"
        Me.CBKHO.Size = New System.Drawing.Size(334, 24)
        Me.CBKHO.TabIndex = 5
        Me.CBKHO.Text = "Nur den Hauptkontaktordner durchsuchen"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBKHO, resources.GetString("CBKHO.ToolTip"))
        Me.CBKHO.UseVisualStyleBackColor = True
        '
        'CBIndexAus
        '
        Me.CBIndexAus.AutoSize = True
        Me.CBIndexAus.Enabled = False
        Me.CBIndexAus.Location = New System.Drawing.Point(372, 29)
        Me.CBIndexAus.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBIndexAus.Name = "CBIndexAus"
        Me.CBIndexAus.Size = New System.Drawing.Size(446, 24)
        Me.CBIndexAus.TabIndex = 6
        Me.CBIndexAus.Text = "Indizierung auschalten (nur wenn Anrufmonitor deaktiviert)"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBIndexAus, "Wenn Sie den Anrufmonitor nicht verwenden, können Sie die Indizierung auch aussch" &
        "alten.")
        Me.CBIndexAus.UseVisualStyleBackColor = True
        '
        'CBAnrMonTransp
        '
        Me.CBAnrMonTransp.AutoSize = True
        Me.CBAnrMonTransp.Location = New System.Drawing.Point(6, 29)
        Me.CBAnrMonTransp.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAnrMonTransp.Name = "CBAnrMonTransp"
        Me.CBAnrMonTransp.Size = New System.Drawing.Size(200, 24)
        Me.CBAnrMonTransp.TabIndex = 7
        Me.CBAnrMonTransp.Text = "Verwende Transparenz"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBAnrMonTransp, "Wenn diese Einstellung gesetzt ist, wird der Anrufmonitor ein und ausgeblendet." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) &
        "Dazu wird die Transparenz des Anrufmonitors erhöht, bzw. verringert.")
        Me.CBAnrMonTransp.UseVisualStyleBackColor = True
        '
        'CBAnrMonMove
        '
        Me.CBAnrMonMove.AutoSize = True
        Me.CBAnrMonMove.Location = New System.Drawing.Point(6, 65)
        Me.CBAnrMonMove.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAnrMonMove.Name = "CBAnrMonMove"
        Me.CBAnrMonMove.Size = New System.Drawing.Size(277, 24)
        Me.CBAnrMonMove.TabIndex = 8
        Me.CBAnrMonMove.Text = "Verwende Anrufmonitorbewegung"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBAnrMonMove, "Wenn diese Einstellung gesetzt ist, wird der Anrufmonitor von unten in den Deskto" &
        "p hinein geschoben.")
        Me.CBAnrMonMove.UseVisualStyleBackColor = True
        '
        'CBShowMSN
        '
        Me.CBShowMSN.AutoSize = True
        Me.CBShowMSN.Location = New System.Drawing.Point(6, 100)
        Me.CBShowMSN.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBShowMSN.Name = "CBShowMSN"
        Me.CBShowMSN.Size = New System.Drawing.Size(252, 24)
        Me.CBShowMSN.TabIndex = 9
        Me.CBShowMSN.Text = "Zeige MSN im Anrufmonitor an"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBShowMSN, "Wenn diese Einstellung gesetzt ist, wird die jeweilige MSN im Anrufmonitor angeze" &
        "igt.")
        Me.CBShowMSN.UseVisualStyleBackColor = True
        '
        'PGrundeinstellungen
        '
        Me.PGrundeinstellungen.Controls.Add(Me.GroupBox5)
        Me.PGrundeinstellungen.Controls.Add(Me.GroupBoxStoppUhr)
        Me.PGrundeinstellungen.Controls.Add(Me.Frame3)
        Me.PGrundeinstellungen.Controls.Add(Me.FrameErforderlich)
        Me.PGrundeinstellungen.Controls.Add(Me.Label13)
        Me.PGrundeinstellungen.Location = New System.Drawing.Point(4, 29)
        Me.PGrundeinstellungen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PGrundeinstellungen.Name = "PGrundeinstellungen"
        Me.PGrundeinstellungen.Size = New System.Drawing.Size(860, 458)
        Me.PGrundeinstellungen.TabIndex = 7
        Me.PGrundeinstellungen.Text = "Grundeinstellungen"
        Me.ToolTipFBDBConfig.SetToolTip(Me.PGrundeinstellungen, "Bevor eine Handynummer gewählt wird")
        Me.PGrundeinstellungen.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox5.Controls.Add(Me.CBIgnoTelNrFormat)
        Me.GroupBox5.Controls.Add(Me.Label24)
        Me.GroupBox5.Controls.Add(Me.TBTelNrMaske)
        Me.GroupBox5.Controls.Add(Me.CBintl)
        Me.GroupBox5.Controls.Add(Me.CBTelNrGruppieren)
        Me.GroupBox5.Location = New System.Drawing.Point(0, 299)
        Me.GroupBox5.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox5.Size = New System.Drawing.Size(423, 152)
        Me.GroupBox5.TabIndex = 17
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Formatierung von Telefonnummern"
        '
        'CBIgnoTelNrFormat
        '
        Me.CBIgnoTelNrFormat.AutoSize = True
        Me.CBIgnoTelNrFormat.Location = New System.Drawing.Point(9, 100)
        Me.CBIgnoTelNrFormat.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBIgnoTelNrFormat.Name = "CBIgnoTelNrFormat"
        Me.CBIgnoTelNrFormat.Size = New System.Drawing.Size(292, 24)
        Me.CBIgnoTelNrFormat.TabIndex = 15
        Me.CBIgnoTelNrFormat.Text = "Ignoriere Formatierung der Kontakte"
        Me.CBIgnoTelNrFormat.UseVisualStyleBackColor = True
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(135, 31)
        Me.Label24.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(60, 20)
        Me.Label24.TabIndex = 3
        Me.Label24.Text = "Maske:"
        Me.ToolTipFBDBConfig.SetToolTip(Me.Label24, resources.GetString("Label24.ToolTip"))
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
        'CBintl
        '
        Me.CBintl.AutoSize = True
        Me.CBintl.Location = New System.Drawing.Point(9, 65)
        Me.CBintl.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBintl.Name = "CBintl"
        Me.CBintl.Size = New System.Drawing.Size(304, 24)
        Me.CBintl.TabIndex = 14
        Me.CBintl.Text = "Internationale Vorwahl immer anfügen"
        Me.CBintl.UseVisualStyleBackColor = True
        '
        'CBTelNrGruppieren
        '
        Me.CBTelNrGruppieren.AutoSize = True
        Me.CBTelNrGruppieren.Location = New System.Drawing.Point(9, 29)
        Me.CBTelNrGruppieren.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBTelNrGruppieren.Name = "CBTelNrGruppieren"
        Me.CBTelNrGruppieren.Size = New System.Drawing.Size(115, 24)
        Me.CBTelNrGruppieren.TabIndex = 12
        Me.CBTelNrGruppieren.Text = "Gruppieren"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBTelNrGruppieren, "Gruppiert Rufnummernteile in Zweierblöcke für bessere Lessbarkeit." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Beispiel:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "oh" &
        "ne Gruppierung: +49 (123) 4567890 " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "mit Gruppierung: +49 (1 23) 4 56 78 90 ")
        Me.CBTelNrGruppieren.UseVisualStyleBackColor = True
        '
        'GroupBoxStoppUhr
        '
        Me.GroupBoxStoppUhr.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBoxStoppUhr.Controls.Add(Me.CBStoppUhrIgnIntFax)
        Me.GroupBoxStoppUhr.Controls.Add(Me.BStoppUhrAnzeigen)
        Me.GroupBoxStoppUhr.Controls.Add(Me.LabelStoppUhr)
        Me.GroupBoxStoppUhr.Controls.Add(Me.TBStoppUhr)
        Me.GroupBoxStoppUhr.Controls.Add(Me.CBStoppUhrAusblenden)
        Me.GroupBoxStoppUhr.Controls.Add(Me.CBStoppUhrEinblenden)
        Me.GroupBoxStoppUhr.Location = New System.Drawing.Point(433, 299)
        Me.GroupBoxStoppUhr.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBoxStoppUhr.Name = "GroupBoxStoppUhr"
        Me.GroupBoxStoppUhr.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBoxStoppUhr.Size = New System.Drawing.Size(423, 152)
        Me.GroupBoxStoppUhr.TabIndex = 19
        Me.GroupBoxStoppUhr.TabStop = False
        Me.GroupBoxStoppUhr.Text = "Stoppuhr"
        '
        'CBStoppUhrIgnIntFax
        '
        Me.CBStoppUhrIgnIntFax.AutoSize = True
        Me.CBStoppUhrIgnIntFax.Location = New System.Drawing.Point(9, 100)
        Me.CBStoppUhrIgnIntFax.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBStoppUhrIgnIntFax.Name = "CBStoppUhrIgnIntFax"
        Me.CBStoppUhrIgnIntFax.Size = New System.Drawing.Size(216, 24)
        Me.CBStoppUhrIgnIntFax.TabIndex = 20
        Me.CBStoppUhrIgnIntFax.Text = "Ignoriere intern. Faxempf."
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBStoppUhrIgnIntFax, "Die Stoppuhr wird nicht angezeigt wenn ein interner Faxempfang erkannt wurde.")
        Me.CBStoppUhrIgnIntFax.UseVisualStyleBackColor = True
        '
        'BStoppUhrAnzeigen
        '
        Me.BStoppUhrAnzeigen.Location = New System.Drawing.Point(250, 100)
        Me.BStoppUhrAnzeigen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BStoppUhrAnzeigen.Name = "BStoppUhrAnzeigen"
        Me.BStoppUhrAnzeigen.Size = New System.Drawing.Size(168, 43)
        Me.BStoppUhrAnzeigen.TabIndex = 19
        Me.BStoppUhrAnzeigen.Text = "Anzeigen"
        Me.BStoppUhrAnzeigen.UseVisualStyleBackColor = True
        '
        'LabelStoppUhr
        '
        Me.LabelStoppUhr.AutoSize = True
        Me.LabelStoppUhr.Location = New System.Drawing.Point(252, 66)
        Me.LabelStoppUhr.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelStoppUhr.Name = "LabelStoppUhr"
        Me.LabelStoppUhr.Size = New System.Drawing.Size(112, 20)
        Me.LabelStoppUhr.TabIndex = 3
        Me.LabelStoppUhr.Text = "Sekunden aus"
        '
        'TBStoppUhr
        '
        Me.TBStoppUhr.Location = New System.Drawing.Point(201, 62)
        Me.TBStoppUhr.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBStoppUhr.Name = "TBStoppUhr"
        Me.TBStoppUhr.Size = New System.Drawing.Size(43, 26)
        Me.TBStoppUhr.TabIndex = 18
        '
        'CBStoppUhrAusblenden
        '
        Me.CBStoppUhrAusblenden.AutoSize = True
        Me.CBStoppUhrAusblenden.Location = New System.Drawing.Point(9, 65)
        Me.CBStoppUhrAusblenden.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBStoppUhrAusblenden.Name = "CBStoppUhrAusblenden"
        Me.CBStoppUhrAusblenden.Size = New System.Drawing.Size(194, 24)
        Me.CBStoppUhrAusblenden.TabIndex = 17
        Me.CBStoppUhrAusblenden.Text = "Blende Stoppuhr nach"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBStoppUhrAusblenden, "Blendet die Stoppuhr nach Beendiging eines Telefonats aus." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10))
        Me.CBStoppUhrAusblenden.UseVisualStyleBackColor = True
        '
        'CBStoppUhrEinblenden
        '
        Me.CBStoppUhrEinblenden.AutoSize = True
        Me.CBStoppUhrEinblenden.Location = New System.Drawing.Point(9, 29)
        Me.CBStoppUhrEinblenden.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBStoppUhrEinblenden.Name = "CBStoppUhrEinblenden"
        Me.CBStoppUhrEinblenden.Size = New System.Drawing.Size(180, 24)
        Me.CBStoppUhrEinblenden.TabIndex = 16
        Me.CBStoppUhrEinblenden.Text = "Blende Stoppuhr ein"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBStoppUhrEinblenden, "Blendet eine Stoppuhr beim Zustandekommen einer Verbindung ein." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10))
        Me.CBStoppUhrEinblenden.UseVisualStyleBackColor = True
        '
        'Frame3
        '
        Me.Frame3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Frame3.Controls.Add(Me.CBCallByCall)
        Me.Frame3.Controls.Add(Me.CBDialPort)
        Me.Frame3.Controls.Add(Me.CBCheckMobil)
        Me.Frame3.Controls.Add(Me.CBVoIPBuster)
        Me.Frame3.Controls.Add(Me.CBCbCunterbinden)
        Me.Frame3.Controls.Add(Me.Label6)
        Me.Frame3.Controls.Add(Me.TBAmt)
        Me.Frame3.Location = New System.Drawing.Point(433, 58)
        Me.Frame3.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Frame3.Name = "Frame3"
        Me.Frame3.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Frame3.Size = New System.Drawing.Size(423, 232)
        Me.Frame3.TabIndex = 18
        Me.Frame3.TabStop = False
        Me.Frame3.Text = "Einstellungen für die Wählhilfe"
        '
        'CBCallByCall
        '
        Me.CBCallByCall.AutoSize = True
        Me.CBCallByCall.Location = New System.Drawing.Point(222, 152)
        Me.CBCallByCall.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBCallByCall.Name = "CBCallByCall"
        Me.CBCallByCall.Size = New System.Drawing.Size(185, 24)
        Me.CBCallByCall.TabIndex = 10
        Me.CBCallByCall.Text = "Jedesmal Call-by-Call"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBCallByCall, "Call-by-Call ist eine Funktion, die es erlaubt günstig mit Vorvorwahlen zu telefo" &
        "nieren. " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Um die aktuell günstigsten Vorvorwahlen zu ermitteln, wird Billiger-Te" &
        "lefonieren.de verwendet.")
        Me.CBCallByCall.UseVisualStyleBackColor = True
        '
        'CBDialPort
        '
        Me.CBDialPort.AutoSize = True
        Me.CBDialPort.Location = New System.Drawing.Point(9, 112)
        Me.CBDialPort.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBDialPort.Name = "CBDialPort"
        Me.CBDialPort.Size = New System.Drawing.Size(159, 24)
        Me.CBDialPort.TabIndex = 8
        Me.CBDialPort.Text = "Dialport anzeigen"
        Me.CBDialPort.UseVisualStyleBackColor = True
        '
        'CBCheckMobil
        '
        Me.CBCheckMobil.AutoSize = True
        Me.CBCheckMobil.Location = New System.Drawing.Point(9, 192)
        Me.CBCheckMobil.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBCheckMobil.Name = "CBCheckMobil"
        Me.CBCheckMobil.Size = New System.Drawing.Size(342, 24)
        Me.CBCheckMobil.TabIndex = 11
        Me.CBCheckMobil.Text = "Nachfrage beim Wählen von Mobilnummern"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBCheckMobil, "Um unnötige Verbindungskosten zu Mobilfunkgeräten zu vermeiden, wird vor dem Wähl" &
        "en eine zusätzliche Benutzereingabe erforderlich.")
        Me.CBCheckMobil.UseVisualStyleBackColor = True
        '
        'CBVoIPBuster
        '
        Me.CBVoIPBuster.AutoSize = True
        Me.CBVoIPBuster.Location = New System.Drawing.Point(9, 34)
        Me.CBVoIPBuster.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBVoIPBuster.Name = "CBVoIPBuster"
        Me.CBVoIPBuster.Size = New System.Drawing.Size(263, 24)
        Me.CBVoIPBuster.TabIndex = 6
        Me.CBVoIPBuster.Text = "Landesvorwahl immer mitwählen"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBVoIPBuster, "Mit dieser Einstellung wird die definierte Landesvorwahl immer mitgewählt.")
        Me.CBVoIPBuster.UseVisualStyleBackColor = True
        '
        'CBCbCunterbinden
        '
        Me.CBCbCunterbinden.AutoSize = True
        Me.CBCbCunterbinden.Location = New System.Drawing.Point(9, 152)
        Me.CBCbCunterbinden.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBCbCunterbinden.Name = "CBCbCunterbinden"
        Me.CBCbCunterbinden.Size = New System.Drawing.Size(202, 24)
        Me.CBCbCunterbinden.TabIndex = 9
        Me.CBCbCunterbinden.Text = "Call-by-Call unterbinden"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBCbCunterbinden, "Mitunter ist es sinnvoll Call-by-Call Vorwahlen zu unterbinden, z.B. wenn Sie " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "k" &
        "einen Festnetzanschluss haben und nur über Ihren Internetanbieter telefonieren.")
        Me.CBCbCunterbinden.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(63, 75)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(94, 20)
        Me.Label6.TabIndex = 31
        Me.Label6.Text = "Amtsholung"
        Me.ToolTipFBDBConfig.SetToolTip(Me.Label6, "Geben Sie hier eine 0 ein falls eine Amtsholung benötigt wird.")
        '
        'TBAmt
        '
        Me.TBAmt.Location = New System.Drawing.Point(9, 71)
        Me.TBAmt.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBAmt.Name = "TBAmt"
        Me.TBAmt.Size = New System.Drawing.Size(43, 26)
        Me.TBAmt.TabIndex = 7
        Me.ToolTipFBDBConfig.SetToolTip(Me.TBAmt, "Geben Sie hier eine 0 ein falls eine Amtsholung benötigt wird.")
        '
        'FrameErforderlich
        '
        Me.FrameErforderlich.Controls.Add(Me.BTestLogin)
        Me.FrameErforderlich.Controls.Add(Me.TBPasswort)
        Me.FrameErforderlich.Controls.Add(Me.Label3)
        Me.FrameErforderlich.Controls.Add(Me.TBLandesVW)
        Me.FrameErforderlich.Controls.Add(Me.TBVorwahl)
        Me.FrameErforderlich.Controls.Add(Me.TBBenutzer)
        Me.FrameErforderlich.Controls.Add(Me.CBForceFBAddr)
        Me.FrameErforderlich.Controls.Add(Me.Label5)
        Me.FrameErforderlich.Controls.Add(Me.Label4)
        Me.FrameErforderlich.Controls.Add(Me.lblTBPasswort)
        Me.FrameErforderlich.Controls.Add(Me.TBFBAdr)
        Me.FrameErforderlich.Location = New System.Drawing.Point(0, 58)
        Me.FrameErforderlich.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.FrameErforderlich.Name = "FrameErforderlich"
        Me.FrameErforderlich.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.FrameErforderlich.Size = New System.Drawing.Size(423, 232)
        Me.FrameErforderlich.TabIndex = 16
        Me.FrameErforderlich.TabStop = False
        Me.FrameErforderlich.Text = "Erforderliche Angaben"
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
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(170, 75)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(180, 20)
        Me.Label3.TabIndex = 16
        Me.Label3.Text = "Fritz!Box Benutzername"
        Me.ToolTipFBDBConfig.SetToolTip(Me.Label3, resources.GetString("Label3.ToolTip"))
        '
        'TBLandesVW
        '
        Me.TBLandesVW.Location = New System.Drawing.Point(9, 189)
        Me.TBLandesVW.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBLandesVW.Name = "TBLandesVW"
        Me.TBLandesVW.Size = New System.Drawing.Size(148, 26)
        Me.TBLandesVW.TabIndex = 2
        '
        'TBVorwahl
        '
        Me.TBVorwahl.Location = New System.Drawing.Point(9, 149)
        Me.TBVorwahl.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBVorwahl.Name = "TBVorwahl"
        Me.TBVorwahl.Size = New System.Drawing.Size(148, 26)
        Me.TBVorwahl.TabIndex = 2
        '
        'TBBenutzer
        '
        Me.TBBenutzer.Location = New System.Drawing.Point(9, 71)
        Me.TBBenutzer.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBBenutzer.Name = "TBBenutzer"
        Me.TBBenutzer.Size = New System.Drawing.Size(148, 26)
        Me.TBBenutzer.TabIndex = 2
        '
        'CBForceFBAddr
        '
        Me.CBForceFBAddr.AutoSize = True
        Me.CBForceFBAddr.Location = New System.Drawing.Point(174, 34)
        Me.CBForceFBAddr.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBForceFBAddr.Name = "CBForceFBAddr"
        Me.CBForceFBAddr.Size = New System.Drawing.Size(160, 24)
        Me.CBForceFBAddr.TabIndex = 1
        Me.CBForceFBAddr.Text = "Fritz!Box Adresse"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBForceFBAddr, "Wenn der Haken gesetzt wird, wird trotz fehlgeschlagener Ping-Check eine Verbindu" &
        "ng zur eingegebenen Addresse aufgebaut." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Das ist z.B. bei einigen dyndns-Anbiete" &
        "rn nötig, da diese Pings blockieren.")
        Me.CBForceFBAddr.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(170, 194)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(115, 20)
        Me.Label5.TabIndex = 13
        Me.Label5.Text = "Landesvorwahl" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(170, 154)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(92, 20)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "Ortsvorwahl" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'lblTBPasswort
        '
        Me.lblTBPasswort.AutoSize = True
        Me.lblTBPasswort.Location = New System.Drawing.Point(170, 115)
        Me.lblTBPasswort.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTBPasswort.Name = "lblTBPasswort"
        Me.lblTBPasswort.Size = New System.Drawing.Size(140, 20)
        Me.lblTBPasswort.TabIndex = 3
        Me.lblTBPasswort.Text = "Fritz!Box Passwort"
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
        'Label33
        '
        Me.Label33.AutoSize = True
        Me.Label33.Location = New System.Drawing.Point(170, 112)
        Me.Label33.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(93, 20)
        Me.Label33.TabIndex = 13
        Me.Label33.Text = "SIP-Telefon"
        Me.ToolTipFBDBConfig.SetToolTip(Me.Label33, "Geben Sie hier das SIP-Telefon, an welches mit Phoner verknüpft ist.")
        '
        'BAnrMonTest
        '
        Me.BAnrMonTest.Location = New System.Drawing.Point(249, 346)
        Me.BAnrMonTest.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BAnrMonTest.Name = "BAnrMonTest"
        Me.BAnrMonTest.Size = New System.Drawing.Size(168, 43)
        Me.BAnrMonTest.TabIndex = 14
        Me.BAnrMonTest.Text = "Anzeigen"
        Me.ToolTipFBDBConfig.SetToolTip(Me.BAnrMonTest, "Zeigt den Anrufmonitor testweise an.")
        Me.BAnrMonTest.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(381, 31)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(246, 20)
        Me.Label7.TabIndex = 5
        Me.Label7.Text = "Speicherort für erstellte Kontakte:"
        Me.ToolTipFBDBConfig.SetToolTip(Me.Label7, resources.GetString("Label7.ToolTip"))
        '
        'TBRWSTest
        '
        Me.TBRWSTest.Location = New System.Drawing.Point(130, 68)
        Me.TBRWSTest.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBRWSTest.Name = "TBRWSTest"
        Me.TBRWSTest.Size = New System.Drawing.Size(148, 26)
        Me.TBRWSTest.TabIndex = 6
        Me.ToolTipFBDBConfig.SetToolTip(Me.TBRWSTest, "Geben Sie hier eine gültige Telefonnummer ein, nach der eine Rückwärtssuche durch" &
        "geführt werden soll.")
        '
        'Label32
        '
        Me.Label32.AutoSize = True
        Me.Label32.Location = New System.Drawing.Point(310, 200)
        Me.Label32.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(72, 20)
        Me.Label32.TabIndex = 27
        Me.Label32.Text = "schneller"
        Me.Label32.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.ToolTipFBDBConfig.SetToolTip(Me.Label32, "Derzeit abgeschaltet!")
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Location = New System.Drawing.Point(202, 200)
        Me.Label28.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(83, 20)
        Me.Label28.TabIndex = 27
        Me.Label28.Text = "langsamer"
        Me.ToolTipFBDBConfig.SetToolTip(Me.Label28, "Derzeit abgeschaltet!")
        '
        'CBAutoAnrList
        '
        Me.CBAutoAnrList.AutoSize = True
        Me.CBAutoAnrList.Location = New System.Drawing.Point(4, 29)
        Me.CBAutoAnrList.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAutoAnrList.Name = "CBAutoAnrList"
        Me.CBAutoAnrList.Size = New System.Drawing.Size(257, 24)
        Me.CBAutoAnrList.TabIndex = 8
        Me.CBAutoAnrList.Text = "Anrufliste beim Start auswerten"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBAutoAnrList, resources.GetString("CBAutoAnrList.ToolTip"))
        Me.CBAutoAnrList.UseVisualStyleBackColor = True
        '
        'TVOutlookContact
        '
        Me.TVOutlookContact.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TVOutlookContact.CheckBoxes = True
        Me.TVOutlookContact.FullRowSelect = True
        Me.TVOutlookContact.Location = New System.Drawing.Point(386, 60)
        Me.TVOutlookContact.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TVOutlookContact.Name = "TVOutlookContact"
        Me.TVOutlookContact.Size = New System.Drawing.Size(460, 107)
        Me.TVOutlookContact.TabIndex = 4
        Me.ToolTipFBDBConfig.SetToolTip(Me.TVOutlookContact, resources.GetString("TVOutlookContact.ToolTip"))
        '
        'TBAnrBeantworterTimeout
        '
        Me.TBAnrBeantworterTimeout.Enabled = False
        Me.TBAnrBeantworterTimeout.Location = New System.Drawing.Point(3, 174)
        Me.TBAnrBeantworterTimeout.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBAnrBeantworterTimeout.Name = "TBAnrBeantworterTimeout"
        Me.TBAnrBeantworterTimeout.Size = New System.Drawing.Size(42, 26)
        Me.TBAnrBeantworterTimeout.TabIndex = 37
        Me.ToolTipFBDBConfig.SetToolTip(Me.TBAnrBeantworterTimeout, "Telefonate, die nach der definierten Zeitspanne verbunden werden, wenden als ""Ver" &
        "passt"" behandelt." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Nach der Zeitspanne erfolgt z. B. die Verbindung zum Anrufbea" &
        "ntworter.")
        '
        'LAnrBeantworterTimeout
        '
        Me.LAnrBeantworterTimeout.AutoSize = True
        Me.LAnrBeantworterTimeout.Enabled = False
        Me.LAnrBeantworterTimeout.Location = New System.Drawing.Point(56, 178)
        Me.LAnrBeantworterTimeout.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LAnrBeantworterTimeout.Name = "LAnrBeantworterTimeout"
        Me.LAnrBeantworterTimeout.Size = New System.Drawing.Size(206, 20)
        Me.LAnrBeantworterTimeout.TabIndex = 38
        Me.LAnrBeantworterTimeout.Text = "Anrufbentworter-Timeout [s]"
        Me.ToolTipFBDBConfig.SetToolTip(Me.LAnrBeantworterTimeout, "Telefonate, die nach der definierten Zeitspanne verbunden werden, wenden als ""Ver" &
        "passt"" behandelt." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Nach der Zeitspanne erfolgt z. B. die Verbindung zum Anrufbea" &
        "ntworter.")
        '
        'BProbleme
        '
        Me.BProbleme.Location = New System.Drawing.Point(582, 158)
        Me.BProbleme.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BProbleme.Name = "BProbleme"
        Me.BProbleme.Size = New System.Drawing.Size(268, 43)
        Me.BProbleme.TabIndex = 33
        Me.BProbleme.Text = "Probleme?"
        Me.ToolTipFBDBConfig.SetToolTip(Me.BProbleme, "Werden nicht alle Telefonnummern oder Telefone erkannt?" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Werden sie falsch zugeor" &
        "dnet?" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Das Addin sammelt ein paar Informationen und schickt sie an den Entwick" &
        "ler." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10))
        Me.BProbleme.UseVisualStyleBackColor = True
        '
        'BZwischenablage
        '
        Me.BZwischenablage.Location = New System.Drawing.Point(582, 106)
        Me.BZwischenablage.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BZwischenablage.Name = "BZwischenablage"
        Me.BZwischenablage.Size = New System.Drawing.Size(268, 43)
        Me.BZwischenablage.TabIndex = 32
        Me.BZwischenablage.Text = "Kopieren"
        Me.ToolTipFBDBConfig.SetToolTip(Me.BZwischenablage, "Kopiert den Statustext in die Zwischenablage")
        Me.BZwischenablage.UseVisualStyleBackColor = True
        '
        'BStartDebug
        '
        Me.BStartDebug.Location = New System.Drawing.Point(582, 54)
        Me.BStartDebug.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BStartDebug.Name = "BStartDebug"
        Me.BStartDebug.Size = New System.Drawing.Size(268, 43)
        Me.BStartDebug.TabIndex = 30
        Me.BStartDebug.Text = "Start"
        Me.ToolTipFBDBConfig.SetToolTip(Me.BStartDebug, "Kopiert den Statustext in die Zwischenablage")
        Me.BStartDebug.UseVisualStyleBackColor = True
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
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(62, 32)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(252, 20)
        Me.Label9.TabIndex = 40
        Me.Label9.Text = "Anzahl der Listenelemente je Liste"
        Me.ToolTipFBDBConfig.SetToolTip(Me.Label9, "Telefonate, die nach der definierten Zeitspanne verbunden werden, wenden als ""Ver" &
        "passt"" behandelt." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Nach der Zeitspanne erfolgt z. B. die Verbindung zum Anrufbea" &
        "ntworter.")
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.RBFBComUPnP)
        Me.GroupBox3.Controls.Add(Me.RBFBComWeb)
        Me.GroupBox3.Location = New System.Drawing.Point(0, 380)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox3.Size = New System.Drawing.Size(424, 68)
        Me.GroupBox3.TabIndex = 34
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Fritz!Box Kommunikation"
        Me.ToolTipFBDBConfig.SetToolTip(Me.GroupBox3, "Bei Änderung der Fritz!Box Kommunikation ist ein Neustart des Addins erforderlich" &
        ".")
        '
        'RBFBComUPnP
        '
        Me.RBFBComUPnP.AutoSize = True
        Me.RBFBComUPnP.Location = New System.Drawing.Point(172, 29)
        Me.RBFBComUPnP.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RBFBComUPnP.Name = "RBFBComUPnP"
        Me.RBFBComUPnP.Size = New System.Drawing.Size(234, 24)
        Me.RBFBComUPnP.TabIndex = 0
        Me.RBFBComUPnP.Text = "UPnP (SOAP, experimentell)"
        Me.ToolTipFBDBConfig.SetToolTip(Me.RBFBComUPnP, "Bei Änderung der Fritz!Box Kommunikation ist ein Neustart des Addins erforderlich" &
        ".")
        Me.RBFBComUPnP.UseVisualStyleBackColor = True
        '
        'RBFBComWeb
        '
        Me.RBFBComWeb.AutoSize = True
        Me.RBFBComWeb.Checked = True
        Me.RBFBComWeb.Location = New System.Drawing.Point(9, 29)
        Me.RBFBComWeb.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RBFBComWeb.Name = "RBFBComWeb"
        Me.RBFBComWeb.Size = New System.Drawing.Size(142, 24)
        Me.RBFBComWeb.TabIndex = 0
        Me.RBFBComWeb.TabStop = True
        Me.RBFBComWeb.Text = "Weboberfläche"
        Me.ToolTipFBDBConfig.SetToolTip(Me.RBFBComWeb, "Bei Änderung der Fritz!Box Kommunikation ist ein Neustart des Addins erforderlich" &
        ".")
        Me.RBFBComWeb.UseVisualStyleBackColor = True
        '
        'BManLoad
        '
        Me.BManLoad.Location = New System.Drawing.Point(582, 211)
        Me.BManLoad.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BManLoad.Name = "BManLoad"
        Me.BManLoad.Size = New System.Drawing.Size(268, 43)
        Me.BManLoad.TabIndex = 34
        Me.BManLoad.Text = "Manuelles Einlesen"
        Me.ToolTipFBDBConfig.SetToolTip(Me.BManLoad, "Startet das manuelle Einlesen der Telefonkonfiguration aus den Debug-Dateien.")
        Me.BManLoad.UseVisualStyleBackColor = True
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
        Me.PInfo.Size = New System.Drawing.Size(859, 459)
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
        Me.PTelefone.Size = New System.Drawing.Size(859, 459)
        Me.PTelefone.TabIndex = 5
        Me.PTelefone.Text = "Telefone"
        Me.PTelefone.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.LTelStatus)
        Me.GroupBox1.Controls.Add(Me.BResetStat)
        Me.GroupBox1.Controls.Add(Me.BTelefonliste)
        Me.GroupBox1.Controls.Add(Me.TBAnderes)
        Me.GroupBox1.Controls.Add(Me.TBSchließZeit)
        Me.GroupBox1.Controls.Add(Me.TBReset)
        Me.GroupBox1.Controls.Add(Me.TelList)
        Me.GroupBox1.Location = New System.Drawing.Point(0, 63)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Size = New System.Drawing.Size(855, 389)
        Me.GroupBox1.TabIndex = 27
        Me.GroupBox1.TabStop = False
        '
        'LTelStatus
        '
        Me.LTelStatus.Location = New System.Drawing.Point(318, 312)
        Me.LTelStatus.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LTelStatus.Name = "LTelStatus"
        Me.LTelStatus.Size = New System.Drawing.Size(519, 20)
        Me.LTelStatus.TabIndex = 39
        Me.LTelStatus.Text = "Status: "
        '
        'BResetStat
        '
        Me.BResetStat.Location = New System.Drawing.Point(327, 337)
        Me.BResetStat.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BResetStat.Name = "BResetStat"
        Me.BResetStat.Size = New System.Drawing.Size(255, 43)
        Me.BResetStat.TabIndex = 34
        Me.BResetStat.Text = "Statistik zurücksetzen"
        Me.BResetStat.UseVisualStyleBackColor = True
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
        'TBAnderes
        '
        Me.TBAnderes.AutoSize = True
        Me.TBAnderes.Location = New System.Drawing.Point(4, 300)
        Me.TBAnderes.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.TBAnderes.Name = "TBAnderes"
        Me.TBAnderes.Size = New System.Drawing.Size(80, 20)
        Me.TBAnderes.TabIndex = 38
        Me.TBAnderes.Text = "Sonstiges"
        '
        'TBSchließZeit
        '
        Me.TBSchließZeit.AutoSize = True
        Me.TBSchließZeit.Location = New System.Drawing.Point(4, 280)
        Me.TBSchließZeit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.TBSchließZeit.Name = "TBSchließZeit"
        Me.TBSchließZeit.Size = New System.Drawing.Size(109, 20)
        Me.TBSchließZeit.TabIndex = 36
        Me.TBSchließZeit.Text = "TBSchließZeit"
        '
        'TBReset
        '
        Me.TBReset.AutoSize = True
        Me.TBReset.Location = New System.Drawing.Point(4, 260)
        Me.TBReset.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.TBReset.Name = "TBReset"
        Me.TBReset.Size = New System.Drawing.Size(72, 20)
        Me.TBReset.TabIndex = 37
        Me.TBReset.Text = "TBReset"
        '
        'TelList
        '
        Me.TelList.AllowUserToAddRows = False
        Me.TelList.AllowUserToDeleteRows = False
        Me.TelList.AllowUserToResizeRows = False
        Me.TelList.BackgroundColor = System.Drawing.SystemColors.Window
        Me.TelList.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TelList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.TelList.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ColumnStandardTelefon, Me.Nr, Me.dialCode, Me.Typ, Me.Telefonname, Me.OutNr, Me.Eingehend, Me.Ausgehend, Me.Gesamt})
        Me.TelList.Dock = System.Windows.Forms.DockStyle.Top
        Me.TelList.Location = New System.Drawing.Point(4, 24)
        Me.TelList.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TelList.MultiSelect = False
        Me.TelList.Name = "TelList"
        Me.TelList.RowHeadersVisible = False
        Me.TelList.RowTemplate.Height = 18
        Me.TelList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.TelList.Size = New System.Drawing.Size(847, 231)
        Me.TelList.TabIndex = 33
        '
        'ColumnStandardTelefon
        '
        Me.ColumnStandardTelefon.HeaderText = "*"
        Me.ColumnStandardTelefon.MinimumWidth = 25
        Me.ColumnStandardTelefon.Name = "ColumnStandardTelefon"
        Me.ColumnStandardTelefon.ToolTipText = "Standardtelefon"
        Me.ColumnStandardTelefon.Width = 25
        '
        'Nr
        '
        Me.Nr.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.Nr.HeaderText = "Nr."
        Me.Nr.MinimumWidth = 25
        Me.Nr.Name = "Nr"
        Me.Nr.ReadOnly = True
        Me.Nr.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Nr.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Nr.Width = 25
        '
        'dialCode
        '
        Me.dialCode.HeaderText = "ID"
        Me.dialCode.MinimumWidth = 25
        Me.dialCode.Name = "dialCode"
        Me.dialCode.ReadOnly = True
        Me.dialCode.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dialCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.dialCode.ToolTipText = "Entspricht dem Dialport der Fritz!Box und nicht der internen Nummer."
        Me.dialCode.Width = 30
        '
        'Typ
        '
        Me.Typ.HeaderText = "Typ"
        Me.Typ.MinimumWidth = 50
        Me.Typ.Name = "Typ"
        Me.Typ.ReadOnly = True
        Me.Typ.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Typ.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Typ.Width = 50
        '
        'Telefonname
        '
        Me.Telefonname.HeaderText = "Telefonname"
        Me.Telefonname.MinimumWidth = 75
        Me.Telefonname.Name = "Telefonname"
        Me.Telefonname.ReadOnly = True
        Me.Telefonname.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Telefonname.Width = 115
        '
        'OutNr
        '
        Me.OutNr.HeaderText = "Telefonnummer"
        Me.OutNr.MinimumWidth = 100
        Me.OutNr.Name = "OutNr"
        Me.OutNr.ReadOnly = True
        Me.OutNr.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.OutNr.Width = 115
        '
        'Eingehend
        '
        Me.Eingehend.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        DataGridViewCellStyle1.Format = "T"
        DataGridViewCellStyle1.NullValue = Nothing
        Me.Eingehend.DefaultCellStyle = DataGridViewCellStyle1
        Me.Eingehend.HeaderText = "Eingehend"
        Me.Eingehend.MinimumWidth = 65
        Me.Eingehend.Name = "Eingehend"
        Me.Eingehend.ReadOnly = True
        Me.Eingehend.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Eingehend.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Eingehend.Width = 65
        '
        'Ausgehend
        '
        Me.Ausgehend.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        DataGridViewCellStyle2.Format = "T"
        DataGridViewCellStyle2.NullValue = Nothing
        Me.Ausgehend.DefaultCellStyle = DataGridViewCellStyle2
        Me.Ausgehend.HeaderText = "Ausgehend"
        Me.Ausgehend.MinimumWidth = 65
        Me.Ausgehend.Name = "Ausgehend"
        Me.Ausgehend.ReadOnly = True
        Me.Ausgehend.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Ausgehend.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Ausgehend.Width = 65
        '
        'Gesamt
        '
        Me.Gesamt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        DataGridViewCellStyle3.Format = "T"
        DataGridViewCellStyle3.NullValue = Nothing
        Me.Gesamt.DefaultCellStyle = DataGridViewCellStyle3
        Me.Gesamt.HeaderText = "Gesamt"
        Me.Gesamt.MinimumWidth = 65
        Me.Gesamt.Name = "Gesamt"
        Me.Gesamt.ReadOnly = True
        Me.Gesamt.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Gesamt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Gesamt.Width = 65
        '
        'Label15
        '
        Me.Label15.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(4, 5)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(851, 54)
        Me.Label15.TabIndex = 26
        Me.Label15.Text = "Telefone und Statistik"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PKontaktsuche
        '
        Me.PKontaktsuche.Controls.Add(Me.Label1)
        Me.PKontaktsuche.Controls.Add(Me.GroupBoxIndex)
        Me.PKontaktsuche.Controls.Add(Me.GroupBoxRWS)
        Me.PKontaktsuche.Location = New System.Drawing.Point(4, 29)
        Me.PKontaktsuche.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PKontaktsuche.Name = "PKontaktsuche"
        Me.PKontaktsuche.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PKontaktsuche.Size = New System.Drawing.Size(859, 459)
        Me.PKontaktsuche.TabIndex = 6
        Me.PKontaktsuche.Text = "Kontaktsuche"
        Me.PKontaktsuche.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.Label1.Location = New System.Drawing.Point(4, 5)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(851, 54)
        Me.Label1.TabIndex = 33
        Me.Label1.Text = "Einstellungen für die Kontaktsuche"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'GroupBoxIndex
        '
        Me.GroupBoxIndex.Controls.Add(Me.CBTelFormKorr)
        Me.GroupBoxIndex.Controls.Add(Me.Label21)
        Me.GroupBoxIndex.Controls.Add(Me.RadioButtonEntfernen)
        Me.GroupBoxIndex.Controls.Add(Me.RadioButtonErstelle)
        Me.GroupBoxIndex.Controls.Add(Me.Label20)
        Me.GroupBoxIndex.Controls.Add(Me.LabelAnzahl)
        Me.GroupBoxIndex.Controls.Add(Me.BIndizierungAbbrechen)
        Me.GroupBoxIndex.Controls.Add(Me.BIndizierungStart)
        Me.GroupBoxIndex.Controls.Add(Me.ProgressBarIndex)
        Me.GroupBoxIndex.Controls.Add(Me.CBIndexAus)
        Me.GroupBoxIndex.Controls.Add(Me.CBKHO)
        Me.GroupBoxIndex.Location = New System.Drawing.Point(0, 63)
        Me.GroupBoxIndex.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBoxIndex.Name = "GroupBoxIndex"
        Me.GroupBoxIndex.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBoxIndex.Size = New System.Drawing.Size(855, 203)
        Me.GroupBoxIndex.TabIndex = 10
        Me.GroupBoxIndex.TabStop = False
        Me.GroupBoxIndex.Text = "Kontaktsuche in Outlook (Indizierung)"
        '
        'CBTelFormKorr
        '
        Me.CBTelFormKorr.AutoSize = True
        Me.CBTelFormKorr.Location = New System.Drawing.Point(516, 95)
        Me.CBTelFormKorr.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBTelFormKorr.Name = "CBTelFormKorr"
        Me.CBTelFormKorr.Size = New System.Drawing.Size(323, 24)
        Me.CBTelFormKorr.TabIndex = 19
        Me.CBTelFormKorr.Text = "Telefonnummernformatierung korrigieren"
        Me.CBTelFormKorr.UseVisualStyleBackColor = True
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(4, 97)
        Me.Label21.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(189, 20)
        Me.Label21.TabIndex = 18
        Me.Label21.Text = "Benutzerdefinierte Felder"
        '
        'RadioButtonEntfernen
        '
        Me.RadioButtonEntfernen.AutoSize = True
        Me.RadioButtonEntfernen.Location = New System.Drawing.Point(372, 94)
        Me.RadioButtonEntfernen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RadioButtonEntfernen.Name = "RadioButtonEntfernen"
        Me.RadioButtonEntfernen.Size = New System.Drawing.Size(103, 24)
        Me.RadioButtonEntfernen.TabIndex = 8
        Me.RadioButtonEntfernen.TabStop = True
        Me.RadioButtonEntfernen.Text = "entfernen"
        Me.RadioButtonEntfernen.UseVisualStyleBackColor = True
        '
        'RadioButtonErstelle
        '
        Me.RadioButtonErstelle.AutoSize = True
        Me.RadioButtonErstelle.Checked = True
        Me.RadioButtonErstelle.Location = New System.Drawing.Point(224, 94)
        Me.RadioButtonErstelle.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RadioButtonErstelle.Name = "RadioButtonErstelle"
        Me.RadioButtonErstelle.Size = New System.Drawing.Size(94, 24)
        Me.RadioButtonErstelle.TabIndex = 7
        Me.RadioButtonErstelle.TabStop = True
        Me.RadioButtonErstelle.Text = "erstellen"
        Me.RadioButtonErstelle.UseVisualStyleBackColor = True
        '
        'Label20
        '
        Me.Label20.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label20.Location = New System.Drawing.Point(9, 52)
        Me.Label20.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(850, 37)
        Me.Label20.TabIndex = 14
        Me.Label20.Text = "Falls keine Indizierung durgeführt wird, werden keine Kontakte im Anrufmonitor an" &
    "gezeigt."
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LabelAnzahl
        '
        Me.LabelAnzahl.BackColor = System.Drawing.Color.Transparent
        Me.LabelAnzahl.Location = New System.Drawing.Point(4, 125)
        Me.LabelAnzahl.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelAnzahl.Name = "LabelAnzahl"
        Me.LabelAnzahl.Size = New System.Drawing.Size(846, 20)
        Me.LabelAnzahl.TabIndex = 13
        Me.LabelAnzahl.Text = "Status: "
        '
        'BIndizierungAbbrechen
        '
        Me.BIndizierungAbbrechen.Enabled = False
        Me.BIndizierungAbbrechen.Location = New System.Drawing.Point(682, 149)
        Me.BIndizierungAbbrechen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BIndizierungAbbrechen.Name = "BIndizierungAbbrechen"
        Me.BIndizierungAbbrechen.Size = New System.Drawing.Size(168, 43)
        Me.BIndizierungAbbrechen.TabIndex = 10
        Me.BIndizierungAbbrechen.Text = "Abbrechen"
        Me.BIndizierungAbbrechen.UseVisualStyleBackColor = True
        '
        'BIndizierungStart
        '
        Me.BIndizierungStart.Location = New System.Drawing.Point(506, 149)
        Me.BIndizierungStart.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BIndizierungStart.Name = "BIndizierungStart"
        Me.BIndizierungStart.Size = New System.Drawing.Size(168, 43)
        Me.BIndizierungStart.TabIndex = 9
        Me.BIndizierungStart.Text = "Start"
        Me.BIndizierungStart.UseVisualStyleBackColor = True
        '
        'ProgressBarIndex
        '
        Me.ProgressBarIndex.Location = New System.Drawing.Point(9, 149)
        Me.ProgressBarIndex.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ProgressBarIndex.Name = "ProgressBarIndex"
        Me.ProgressBarIndex.Size = New System.Drawing.Size(488, 43)
        Me.ProgressBarIndex.TabIndex = 10
        '
        'GroupBoxRWS
        '
        Me.GroupBoxRWS.Controls.Add(Me.BRWSTest)
        Me.GroupBoxRWS.Controls.Add(Me.LRWSTest)
        Me.GroupBoxRWS.Controls.Add(Me.TBRWSTest)
        Me.GroupBoxRWS.Controls.Add(Me.Label7)
        Me.GroupBoxRWS.Controls.Add(Me.TVOutlookContact)
        Me.GroupBoxRWS.Controls.Add(Me.CBRWSIndex)
        Me.GroupBoxRWS.Controls.Add(Me.ComboBoxRWS)
        Me.GroupBoxRWS.Controls.Add(Me.CBKErstellen)
        Me.GroupBoxRWS.Controls.Add(Me.CBRWS)
        Me.GroupBoxRWS.Location = New System.Drawing.Point(0, 275)
        Me.GroupBoxRWS.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBoxRWS.Name = "GroupBoxRWS"
        Me.GroupBoxRWS.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBoxRWS.Size = New System.Drawing.Size(856, 177)
        Me.GroupBoxRWS.TabIndex = 9
        Me.GroupBoxRWS.TabStop = False
        Me.GroupBoxRWS.Text = "Rückwärtssuche (RWS)"
        '
        'BRWSTest
        '
        Me.BRWSTest.Enabled = False
        Me.BRWSTest.Location = New System.Drawing.Point(290, 68)
        Me.BRWSTest.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BRWSTest.Name = "BRWSTest"
        Me.BRWSTest.Size = New System.Drawing.Size(87, 31)
        Me.BRWSTest.TabIndex = 8
        Me.BRWSTest.Text = "Teste..."
        Me.BRWSTest.UseVisualStyleBackColor = True
        '
        'LRWSTest
        '
        Me.LRWSTest.AutoSize = True
        Me.LRWSTest.Location = New System.Drawing.Point(4, 72)
        Me.LRWSTest.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LRWSTest.Name = "LRWSTest"
        Me.LRWSTest.Size = New System.Drawing.Size(113, 20)
        Me.LRWSTest.TabIndex = 7
        Me.LRWSTest.Text = "Test der RWS:"
        '
        'ComboBoxRWS
        '
        Me.ComboBoxRWS.FormattingEnabled = True
        Me.ComboBoxRWS.Location = New System.Drawing.Point(182, 26)
        Me.ComboBoxRWS.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ComboBoxRWS.Name = "ComboBoxRWS"
        Me.ComboBoxRWS.Size = New System.Drawing.Size(180, 28)
        Me.ComboBoxRWS.TabIndex = 1
        '
        'CBRWS
        '
        Me.CBRWS.AutoSize = True
        Me.CBRWS.Location = New System.Drawing.Point(9, 29)
        Me.CBRWS.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBRWS.Name = "CBRWS"
        Me.CBRWS.Size = New System.Drawing.Size(157, 24)
        Me.CBRWS.TabIndex = 0
        Me.CBRWS.Text = "Rückwärtssuche:"
        Me.CBRWS.UseVisualStyleBackColor = True
        '
        'PAnrufmonitor
        '
        Me.PAnrufmonitor.Controls.Add(Me.GBoxAnrMonLayout)
        Me.PAnrufmonitor.Controls.Add(Me.Label22)
        Me.PAnrufmonitor.Controls.Add(Me.GBoxAnrMonMain)
        Me.PAnrufmonitor.Location = New System.Drawing.Point(4, 29)
        Me.PAnrufmonitor.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PAnrufmonitor.Name = "PAnrufmonitor"
        Me.PAnrufmonitor.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PAnrufmonitor.Size = New System.Drawing.Size(859, 459)
        Me.PAnrufmonitor.TabIndex = 0
        Me.PAnrufmonitor.Text = "Anrufmonitor"
        Me.PAnrufmonitor.UseVisualStyleBackColor = True
        '
        'GBoxAnrMonLayout
        '
        Me.GBoxAnrMonLayout.Controls.Add(Me.LAnrMonMoveDirection)
        Me.GBoxAnrMonLayout.Controls.Add(Me.Label19)
        Me.GBoxAnrMonLayout.Controls.Add(Me.CBoxAnrMonMoveDirection)
        Me.GBoxAnrMonLayout.Controls.Add(Me.CBoxAnrMonStartPosition)
        Me.GBoxAnrMonLayout.Controls.Add(Me.CBAnrMonContactImage)
        Me.GBoxAnrMonLayout.Controls.Add(Me.CBShowMSN)
        Me.GBoxAnrMonLayout.Controls.Add(Me.Label18)
        Me.GBoxAnrMonLayout.Controls.Add(Me.Label32)
        Me.GBoxAnrMonLayout.Controls.Add(Me.Label28)
        Me.GBoxAnrMonLayout.Controls.Add(Me.BAnrMonTest)
        Me.GBoxAnrMonLayout.Controls.Add(Me.CBAnrMonMove)
        Me.GBoxAnrMonLayout.Controls.Add(Me.CBAnrMonTransp)
        Me.GBoxAnrMonLayout.Controls.Add(Me.TBAnrMonY)
        Me.GBoxAnrMonLayout.Controls.Add(Me.Label11)
        Me.GBoxAnrMonLayout.Controls.Add(Me.Label14)
        Me.GBoxAnrMonLayout.Controls.Add(Me.TBAnrMonX)
        Me.GBoxAnrMonLayout.Controls.Add(Me.Label12)
        Me.GBoxAnrMonLayout.Controls.Add(Me.TBAnrMonMoveGeschwindigkeit)
        Me.GBoxAnrMonLayout.Location = New System.Drawing.Point(432, 58)
        Me.GBoxAnrMonLayout.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBoxAnrMonLayout.Name = "GBoxAnrMonLayout"
        Me.GBoxAnrMonLayout.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBoxAnrMonLayout.Size = New System.Drawing.Size(423, 394)
        Me.GBoxAnrMonLayout.TabIndex = 15
        Me.GBoxAnrMonLayout.TabStop = False
        Me.GBoxAnrMonLayout.Text = "Aussehen anpassen"
        '
        'LAnrMonMoveDirection
        '
        Me.LAnrMonMoveDirection.AutoSize = True
        Me.LAnrMonMoveDirection.Location = New System.Drawing.Point(200, 226)
        Me.LAnrMonMoveDirection.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LAnrMonMoveDirection.Name = "LAnrMonMoveDirection"
        Me.LAnrMonMoveDirection.Size = New System.Drawing.Size(122, 20)
        Me.LAnrMonMoveDirection.TabIndex = 913
        Me.LAnrMonMoveDirection.Text = "Einblenden von:"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(4, 228)
        Me.Label19.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(69, 20)
        Me.Label19.TabIndex = 912
        Me.Label19.Text = "Position:"
        '
        'CBoxAnrMonMoveDirection
        '
        Me.CBoxAnrMonMoveDirection.FormattingEnabled = True
        Me.CBoxAnrMonMoveDirection.Items.AddRange(New Object() {"Oben/Unten", "Links/Rechts"})
        Me.CBoxAnrMonMoveDirection.Location = New System.Drawing.Point(200, 254)
        Me.CBoxAnrMonMoveDirection.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBoxAnrMonMoveDirection.Name = "CBoxAnrMonMoveDirection"
        Me.CBoxAnrMonMoveDirection.Size = New System.Drawing.Size(180, 28)
        Me.CBoxAnrMonMoveDirection.TabIndex = 911
        '
        'CBoxAnrMonStartPosition
        '
        Me.CBoxAnrMonStartPosition.FormattingEnabled = True
        Me.CBoxAnrMonStartPosition.Items.AddRange(New Object() {"Unten Rechts", "Unten Links", "Oben Links", "Oben Rechts"})
        Me.CBoxAnrMonStartPosition.Location = New System.Drawing.Point(9, 254)
        Me.CBoxAnrMonStartPosition.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBoxAnrMonStartPosition.MaxDropDownItems = 4
        Me.CBoxAnrMonStartPosition.Name = "CBoxAnrMonStartPosition"
        Me.CBoxAnrMonStartPosition.Size = New System.Drawing.Size(180, 28)
        Me.CBoxAnrMonStartPosition.TabIndex = 911
        '
        'CBAnrMonContactImage
        '
        Me.CBAnrMonContactImage.AutoSize = True
        Me.CBAnrMonContactImage.Location = New System.Drawing.Point(6, 135)
        Me.CBAnrMonContactImage.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAnrMonContactImage.Name = "CBAnrMonContactImage"
        Me.CBAnrMonContactImage.Size = New System.Drawing.Size(158, 24)
        Me.CBAnrMonContactImage.TabIndex = 910
        Me.CBAnrMonContactImage.Text = "Zeige Kontaktbild"
        Me.CBAnrMonContactImage.UseVisualStyleBackColor = True
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(4, 175)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(187, 20)
        Me.Label18.TabIndex = 26
        Me.Label18.Text = "Einblendgeschwindigkeit:"
        '
        'TBAnrMonY
        '
        Me.TBAnrMonY.AcceptsReturn = True
        Me.TBAnrMonY.Location = New System.Drawing.Point(9, 354)
        Me.TBAnrMonY.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBAnrMonY.Name = "TBAnrMonY"
        Me.TBAnrMonY.Size = New System.Drawing.Size(43, 26)
        Me.TBAnrMonY.TabIndex = 13
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(63, 358)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(122, 20)
        Me.Label11.TabIndex = 13
        Me.Label11.Text = "Punkte (vertikal)"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(4, 291)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(274, 20)
        Me.Label14.TabIndex = 23
        Me.Label14.Text = "Positionskorrektur des Anrufmonitors:"
        '
        'TBAnrMonX
        '
        Me.TBAnrMonX.AcceptsReturn = True
        Me.TBAnrMonX.Location = New System.Drawing.Point(9, 317)
        Me.TBAnrMonX.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBAnrMonX.Name = "TBAnrMonX"
        Me.TBAnrMonX.Size = New System.Drawing.Size(43, 26)
        Me.TBAnrMonX.TabIndex = 12
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(63, 322)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(142, 20)
        Me.Label12.TabIndex = 11
        Me.Label12.Text = "Punkte (horizontal)"
        '
        'TBAnrMonMoveGeschwindigkeit
        '
        Me.TBAnrMonMoveGeschwindigkeit.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.TBAnrMonMoveGeschwindigkeit.LargeChange = 1
        Me.TBAnrMonMoveGeschwindigkeit.Location = New System.Drawing.Point(196, 152)
        Me.TBAnrMonMoveGeschwindigkeit.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBAnrMonMoveGeschwindigkeit.Name = "TBAnrMonMoveGeschwindigkeit"
        Me.TBAnrMonMoveGeschwindigkeit.Size = New System.Drawing.Size(184, 69)
        Me.TBAnrMonMoveGeschwindigkeit.TabIndex = 11
        Me.TBAnrMonMoveGeschwindigkeit.TickStyle = System.Windows.Forms.TickStyle.TopLeft
        '
        'Label22
        '
        Me.Label22.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label22.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.Location = New System.Drawing.Point(4, 5)
        Me.Label22.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(851, 54)
        Me.Label22.TabIndex = 29
        Me.Label22.Text = "Einstellungen für den Anrufmonitor"
        Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'GBoxAnrMonMain
        '
        Me.GBoxAnrMonMain.Controls.Add(Me.PanelAnrMon)
        Me.GBoxAnrMonMain.Controls.Add(Me.CBUseAnrMon)
        Me.GBoxAnrMonMain.Location = New System.Drawing.Point(0, 58)
        Me.GBoxAnrMonMain.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBoxAnrMonMain.Name = "GBoxAnrMonMain"
        Me.GBoxAnrMonMain.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBoxAnrMonMain.Size = New System.Drawing.Size(423, 394)
        Me.GBoxAnrMonMain.TabIndex = 14
        Me.GBoxAnrMonMain.TabStop = False
        Me.GBoxAnrMonMain.Text = "Einstellungen für den Anrufmonitor"
        '
        'PanelAnrMon
        '
        Me.PanelAnrMon.Controls.Add(Me.CBNote)
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
        'CBNote
        '
        Me.CBNote.AutoSize = True
        Me.CBNote.Location = New System.Drawing.Point(2, 263)
        Me.CBNote.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBNote.Name = "CBNote"
        Me.CBNote.Size = New System.Drawing.Size(184, 24)
        Me.CBNote.TabIndex = 34
        Me.CBNote.Text = "Notizeintrag erstellen"
        Me.CBNote.UseVisualStyleBackColor = True
        '
        'CBAnrMonZeigeKontakt
        '
        Me.CBAnrMonZeigeKontakt.AutoSize = True
        Me.CBAnrMonZeigeKontakt.Location = New System.Drawing.Point(2, 231)
        Me.CBAnrMonZeigeKontakt.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
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
        Me.CBAnrMonAuto.Name = "CBAnrMonAuto"
        Me.CBAnrMonAuto.Size = New System.Drawing.Size(265, 24)
        Me.CBAnrMonAuto.TabIndex = 1
        Me.CBAnrMonAuto.Text = "Anrufmonitor mit Outlook starten"
        Me.CBAnrMonAuto.UseVisualStyleBackColor = True
        '
        'CBAnrMonCloseAtDISSCONNECT
        '
        Me.CBAnrMonCloseAtDISSCONNECT.AutoSize = True
        Me.CBAnrMonCloseAtDISSCONNECT.Location = New System.Drawing.Point(4, 115)
        Me.CBAnrMonCloseAtDISSCONNECT.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAnrMonCloseAtDISSCONNECT.Name = "CBAnrMonCloseAtDISSCONNECT"
        Me.CBAnrMonCloseAtDISSCONNECT.Size = New System.Drawing.Size(317, 24)
        Me.CBAnrMonCloseAtDISSCONNECT.TabIndex = 2
        Me.CBAnrMonCloseAtDISSCONNECT.Text = "Anruffenster beim Auflegen ausblenden"
        Me.CBAnrMonCloseAtDISSCONNECT.UseVisualStyleBackColor = True
        '
        'CBAutoClose
        '
        Me.CBAutoClose.AutoSize = True
        Me.CBAutoClose.Location = New System.Drawing.Point(4, 45)
        Me.CBAutoClose.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAutoClose.Name = "CBAutoClose"
        Me.CBAutoClose.Size = New System.Drawing.Size(302, 24)
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
        Me.CLBTelNr.Size = New System.Drawing.Size(221, 74)
        Me.CLBTelNr.TabIndex = 4
        '
        'LEnblDauer
        '
        Me.LEnblDauer.AutoSize = True
        Me.LEnblDauer.Location = New System.Drawing.Point(56, 82)
        Me.LEnblDauer.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LEnblDauer.Name = "LEnblDauer"
        Me.LEnblDauer.Size = New System.Drawing.Size(288, 20)
        Me.LEnblDauer.TabIndex = 31
        Me.LEnblDauer.Text = "Anzeigedauer bei Anruf (minimal: 4s) [s]"
        '
        'CBUseAnrMon
        '
        Me.CBUseAnrMon.AutoSize = True
        Me.CBUseAnrMon.Location = New System.Drawing.Point(4, 29)
        Me.CBUseAnrMon.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBUseAnrMon.Name = "CBUseAnrMon"
        Me.CBUseAnrMon.Size = New System.Drawing.Size(208, 24)
        Me.CBUseAnrMon.TabIndex = 0
        Me.CBUseAnrMon.Text = "Anrufmonitor verwenden"
        Me.CBUseAnrMon.UseVisualStyleBackColor = True
        '
        'FBDB_MP
        '
        Me.FBDB_MP.Controls.Add(Me.PGrundeinstellungen)
        Me.FBDB_MP.Controls.Add(Me.PAnrufmonitor)
        Me.FBDB_MP.Controls.Add(Me.PKontaktsuche)
        Me.FBDB_MP.Controls.Add(Me.PDiverses)
        Me.FBDB_MP.Controls.Add(Me.PTelefone)
        Me.FBDB_MP.Controls.Add(Me.PPhoner)
        Me.FBDB_MP.Controls.Add(Me.PLogging)
        Me.FBDB_MP.Controls.Add(Me.PDebug)
        Me.FBDB_MP.Controls.Add(Me.PInfo)
        Me.FBDB_MP.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FBDB_MP.Location = New System.Drawing.Point(4, 5)
        Me.FBDB_MP.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.FBDB_MP.Name = "FBDB_MP"
        Me.FBDB_MP.SelectedIndex = 0
        Me.FBDB_MP.Size = New System.Drawing.Size(868, 491)
        Me.FBDB_MP.TabIndex = 1
        '
        'PDiverses
        '
        Me.PDiverses.Controls.Add(Me.GroupBox6)
        Me.PDiverses.Controls.Add(Me.GroupBox3)
        Me.PDiverses.Controls.Add(Me.GBoxJournal)
        Me.PDiverses.Controls.Add(Me.GboxAnrListeMain)
        Me.PDiverses.Controls.Add(Me.Label25)
        Me.PDiverses.Location = New System.Drawing.Point(4, 29)
        Me.PDiverses.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PDiverses.Name = "PDiverses"
        Me.PDiverses.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PDiverses.Size = New System.Drawing.Size(860, 458)
        Me.PDiverses.TabIndex = 13
        Me.PDiverses.Text = "Weitere Funktionen"
        Me.PDiverses.UseVisualStyleBackColor = True
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.TBNumEntryList)
        Me.GroupBox6.Controls.Add(Me.Label9)
        Me.GroupBox6.Location = New System.Drawing.Point(432, 380)
        Me.GroupBox6.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox6.Size = New System.Drawing.Size(424, 68)
        Me.GroupBox6.TabIndex = 35
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Anruflisten"
        '
        'GBoxJournal
        '
        Me.GBoxJournal.Controls.Add(Me.CBJournal)
        Me.GBoxJournal.Location = New System.Drawing.Point(0, 58)
        Me.GBoxJournal.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBoxJournal.Name = "GBoxJournal"
        Me.GBoxJournal.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBoxJournal.Size = New System.Drawing.Size(423, 91)
        Me.GBoxJournal.TabIndex = 33
        Me.GBoxJournal.TabStop = False
        Me.GBoxJournal.Text = "Outlook Journal"
        '
        'CBJournal
        '
        Me.CBJournal.AutoSize = True
        Me.CBJournal.Location = New System.Drawing.Point(4, 35)
        Me.CBJournal.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBJournal.Name = "CBJournal"
        Me.CBJournal.Size = New System.Drawing.Size(209, 24)
        Me.CBJournal.TabIndex = 7
        Me.CBJournal.Text = "Journaleinträge erstellen"
        Me.CBJournal.UseVisualStyleBackColor = True
        '
        'GboxAnrListeMain
        '
        Me.GboxAnrListeMain.Controls.Add(Me.TBAnrBeantworterTimeout)
        Me.GboxAnrListeMain.Controls.Add(Me.LAnrBeantworterTimeout)
        Me.GboxAnrListeMain.Controls.Add(Me.CBAnrListeShowAnrMon)
        Me.GboxAnrListeMain.Controls.Add(Me.CBAnrListeUpdateJournal)
        Me.GboxAnrListeMain.Controls.Add(Me.CBAutoAnrList)
        Me.GboxAnrListeMain.Controls.Add(Me.CBAnrListeUpdateCallLists)
        Me.GboxAnrListeMain.Location = New System.Drawing.Point(0, 160)
        Me.GboxAnrListeMain.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GboxAnrListeMain.Name = "GboxAnrListeMain"
        Me.GboxAnrListeMain.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GboxAnrListeMain.Size = New System.Drawing.Size(423, 217)
        Me.GboxAnrListeMain.TabIndex = 31
        Me.GboxAnrListeMain.TabStop = False
        Me.GboxAnrListeMain.Text = "Auswertung der Anrufliste"
        '
        'CBAnrListeShowAnrMon
        '
        Me.CBAnrListeShowAnrMon.AutoSize = True
        Me.CBAnrListeShowAnrMon.Location = New System.Drawing.Point(4, 135)
        Me.CBAnrListeShowAnrMon.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAnrListeShowAnrMon.Name = "CBAnrListeShowAnrMon"
        Me.CBAnrListeShowAnrMon.Size = New System.Drawing.Size(378, 24)
        Me.CBAnrListeShowAnrMon.TabIndex = 9
        Me.CBAnrListeShowAnrMon.Text = "Verpasste Telefonate mit Anrufmonitor anzeigen"
        Me.CBAnrListeShowAnrMon.UseVisualStyleBackColor = True
        '
        'CBAnrListeUpdateJournal
        '
        Me.CBAnrListeUpdateJournal.AutoSize = True
        Me.CBAnrListeUpdateJournal.Enabled = False
        Me.CBAnrListeUpdateJournal.Location = New System.Drawing.Point(4, 65)
        Me.CBAnrListeUpdateJournal.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAnrListeUpdateJournal.Name = "CBAnrListeUpdateJournal"
        Me.CBAnrListeUpdateJournal.Size = New System.Drawing.Size(262, 24)
        Me.CBAnrListeUpdateJournal.TabIndex = 8
        Me.CBAnrListeUpdateJournal.Text = "Journaleinträge vervollständigen"
        Me.CBAnrListeUpdateJournal.UseVisualStyleBackColor = True
        '
        'CBAnrListeUpdateCallLists
        '
        Me.CBAnrListeUpdateCallLists.AutoSize = True
        Me.CBAnrListeUpdateCallLists.Location = New System.Drawing.Point(4, 100)
        Me.CBAnrListeUpdateCallLists.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBAnrListeUpdateCallLists.Name = "CBAnrListeUpdateCallLists"
        Me.CBAnrListeUpdateCallLists.Size = New System.Drawing.Size(390, 24)
        Me.CBAnrListeUpdateCallLists.TabIndex = 7
        Me.CBAnrListeUpdateCallLists.Text = "Rückruf- und Wahlwiederholungsliste aktualisieren"
        Me.CBAnrListeUpdateCallLists.UseVisualStyleBackColor = True
        '
        'Label25
        '
        Me.Label25.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label25.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label25.Location = New System.Drawing.Point(4, 5)
        Me.Label25.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(852, 54)
        Me.Label25.TabIndex = 30
        Me.Label25.Text = "Auswertung der Fritz!box Anrufliste"
        Me.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PPhoner
        '
        Me.PPhoner.Controls.Add(Me.GroupBox4)
        Me.PPhoner.Controls.Add(Me.Label31)
        Me.PPhoner.Controls.Add(Me.Label30)
        Me.PPhoner.Controls.Add(Me.LinkPhoner)
        Me.PPhoner.Controls.Add(Me.Label27)
        Me.PPhoner.Controls.Add(Me.Label26)
        Me.PPhoner.Location = New System.Drawing.Point(4, 29)
        Me.PPhoner.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PPhoner.Name = "PPhoner"
        Me.PPhoner.Size = New System.Drawing.Size(859, 459)
        Me.PPhoner.TabIndex = 12
        Me.PPhoner.Text = "Phoner"
        Me.PPhoner.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.PanelPhonerAktiv)
        Me.GroupBox4.Controls.Add(Me.LabelPhoner)
        Me.GroupBox4.Controls.Add(Me.PanelPhoner)
        Me.GroupBox4.Controls.Add(Me.BPhoner)
        Me.GroupBox4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.GroupBox4.Location = New System.Drawing.Point(0, 224)
        Me.GroupBox4.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox4.Size = New System.Drawing.Size(859, 235)
        Me.GroupBox4.TabIndex = 27
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Phoner Einstellungen"
        '
        'PanelPhonerAktiv
        '
        Me.PanelPhonerAktiv.Location = New System.Drawing.Point(171, 29)
        Me.PanelPhonerAktiv.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PanelPhonerAktiv.Name = "PanelPhonerAktiv"
        Me.PanelPhonerAktiv.Size = New System.Drawing.Size(20, 20)
        Me.PanelPhonerAktiv.TabIndex = 10
        '
        'LabelPhoner
        '
        Me.LabelPhoner.AutoSize = True
        Me.LabelPhoner.Location = New System.Drawing.Point(200, 29)
        Me.LabelPhoner.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LabelPhoner.Name = "LabelPhoner"
        Me.LabelPhoner.Size = New System.Drawing.Size(120, 20)
        Me.LabelPhoner.TabIndex = 5
        Me.LabelPhoner.Text = "Phoner ist aktiv."
        '
        'PanelPhoner
        '
        Me.PanelPhoner.Controls.Add(Me.GroupBox7)
        Me.PanelPhoner.Controls.Add(Me.Label33)
        Me.PanelPhoner.Controls.Add(Me.CBPhoner)
        Me.PanelPhoner.Controls.Add(Me.ComboBoxPhonerSIP)
        Me.PanelPhoner.Controls.Add(Me.TBPhonerPasswort)
        Me.PanelPhoner.Controls.Add(Me.LPassworPhoner)
        Me.PanelPhoner.Location = New System.Drawing.Point(166, 54)
        Me.PanelPhoner.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PanelPhoner.Name = "PanelPhoner"
        Me.PanelPhoner.Size = New System.Drawing.Size(675, 165)
        Me.PanelPhoner.TabIndex = 8
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.CBPhonerAnrMon)
        Me.GroupBox7.Controls.Add(Me.Label29)
        Me.GroupBox7.Location = New System.Drawing.Point(316, 5)
        Me.GroupBox7.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox7.Size = New System.Drawing.Size(354, 140)
        Me.GroupBox7.TabIndex = 14
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "Phoner Anrufmonitor"
        '
        'CBPhonerAnrMon
        '
        Me.CBPhonerAnrMon.AutoSize = True
        Me.CBPhonerAnrMon.Location = New System.Drawing.Point(14, 106)
        Me.CBPhonerAnrMon.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBPhonerAnrMon.Name = "CBPhonerAnrMon"
        Me.CBPhonerAnrMon.Size = New System.Drawing.Size(263, 24)
        Me.CBPhonerAnrMon.TabIndex = 1
        Me.CBPhonerAnrMon.Text = "Phoner Anrufmonitor verwenden"
        Me.CBPhonerAnrMon.UseVisualStyleBackColor = True
        '
        'Label29
        '
        Me.Label29.Location = New System.Drawing.Point(9, 25)
        Me.Label29.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(298, 78)
        Me.Label29.TabIndex = 12
        Me.Label29.Text = "Bei der Verwerndung des Phoner Anrufmonitors wird der Anrufmonitor der Fritz!Box " &
    "nicht mehr überwacht!"
        '
        'CBPhoner
        '
        Me.CBPhoner.AutoSize = True
        Me.CBPhoner.Location = New System.Drawing.Point(10, 28)
        Me.CBPhoner.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBPhoner.Name = "CBPhoner"
        Me.CBPhoner.Size = New System.Drawing.Size(246, 24)
        Me.CBPhoner.TabIndex = 3
        Me.CBPhoner.Text = "Softphone Phoner verwenden"
        Me.CBPhoner.UseVisualStyleBackColor = True
        '
        'ComboBoxPhonerSIP
        '
        Me.ComboBoxPhonerSIP.FormattingEnabled = True
        Me.ComboBoxPhonerSIP.Location = New System.Drawing.Point(10, 108)
        Me.ComboBoxPhonerSIP.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ComboBoxPhonerSIP.Name = "ComboBoxPhonerSIP"
        Me.ComboBoxPhonerSIP.Size = New System.Drawing.Size(148, 28)
        Me.ComboBoxPhonerSIP.TabIndex = 2
        '
        'TBPhonerPasswort
        '
        Me.TBPhonerPasswort.Location = New System.Drawing.Point(10, 63)
        Me.TBPhonerPasswort.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBPhonerPasswort.Name = "TBPhonerPasswort"
        Me.TBPhonerPasswort.Size = New System.Drawing.Size(148, 26)
        Me.TBPhonerPasswort.TabIndex = 7
        Me.TBPhonerPasswort.UseSystemPasswordChar = True
        '
        'LPassworPhoner
        '
        Me.LPassworPhoner.AutoSize = True
        Me.LPassworPhoner.Location = New System.Drawing.Point(170, 68)
        Me.LPassworPhoner.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LPassworPhoner.Name = "LPassworPhoner"
        Me.LPassworPhoner.Size = New System.Drawing.Size(129, 20)
        Me.LPassworPhoner.TabIndex = 6
        Me.LPassworPhoner.Text = "Phoner Passwort"
        '
        'BPhoner
        '
        Me.BPhoner.Location = New System.Drawing.Point(9, 49)
        Me.BPhoner.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BPhoner.Name = "BPhoner"
        Me.BPhoner.Size = New System.Drawing.Size(148, 169)
        Me.BPhoner.TabIndex = 9
        Me.BPhoner.Text = "Teste die Verfügbarkeit von Phoner"
        Me.BPhoner.UseVisualStyleBackColor = True
        '
        'Label31
        '
        Me.Label31.Location = New System.Drawing.Point(6, 78)
        Me.Label31.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(842, 85)
        Me.Label31.TabIndex = 26
        Me.Label31.Text = resources.GetString("Label31.Text")
        '
        'Label30
        '
        Me.Label30.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label30.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label30.Location = New System.Drawing.Point(0, 0)
        Me.Label30.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(859, 54)
        Me.Label30.TabIndex = 25
        Me.Label30.Text = "Einstellungen für Phoner"
        Me.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LinkPhoner
        '
        Me.LinkPhoner.AutoSize = True
        Me.LinkPhoner.Location = New System.Drawing.Point(423, 163)
        Me.LinkPhoner.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LinkPhoner.Name = "LinkPhoner"
        Me.LinkPhoner.Size = New System.Drawing.Size(60, 20)
        Me.LinkPhoner.TabIndex = 4
        Me.LinkPhoner.TabStop = True
        Me.LinkPhoner.Text = "Phoner"
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Location = New System.Drawing.Point(6, 188)
        Me.Label27.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(492, 20)
        Me.Label27.TabIndex = 11
        Me.Label27.Text = "Phoner Copyright 2015 Heiko Sommerfeldt. Alle Rechte vorbehalten."
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Location = New System.Drawing.Point(6, 163)
        Me.Label26.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(369, 20)
        Me.Label26.TabIndex = 10
        Me.Label26.Text = "Phoner kann über folgenden Link bezogen werden:"
        '
        'PLogging
        '
        Me.PLogging.Controls.Add(Me.GBLogging)
        Me.PLogging.Controls.Add(Me.CBLogFile)
        Me.PLogging.Controls.Add(Me.Label23)
        Me.PLogging.Location = New System.Drawing.Point(4, 29)
        Me.PLogging.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PLogging.Name = "PLogging"
        Me.PLogging.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PLogging.Size = New System.Drawing.Size(859, 459)
        Me.PLogging.TabIndex = 11
        Me.PLogging.Text = "Logging"
        Me.PLogging.UseVisualStyleBackColor = True
        '
        'GBLogging
        '
        Me.GBLogging.Controls.Add(Me.BLogging)
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
        'BLogging
        '
        Me.BLogging.Location = New System.Drawing.Point(516, 283)
        Me.BLogging.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BLogging.Name = "BLogging"
        Me.BLogging.Size = New System.Drawing.Size(334, 43)
        Me.BLogging.TabIndex = 27
        Me.BLogging.Text = "Log in die Zwischenablage kopieren"
        Me.BLogging.UseVisualStyleBackColor = True
        '
        'LinkLogFile
        '
        Me.LinkLogFile.Location = New System.Drawing.Point(3, 258)
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
        Me.TBLogging.Location = New System.Drawing.Point(4, 24)
        Me.TBLogging.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBLogging.Multiline = True
        Me.TBLogging.Name = "TBLogging"
        Me.TBLogging.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TBLogging.Size = New System.Drawing.Size(847, 224)
        Me.TBLogging.TabIndex = 25
        '
        'CBLogFile
        '
        Me.CBLogFile.AutoSize = True
        Me.CBLogFile.Location = New System.Drawing.Point(9, 77)
        Me.CBLogFile.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBLogFile.Name = "CBLogFile"
        Me.CBLogFile.Size = New System.Drawing.Size(163, 24)
        Me.CBLogFile.TabIndex = 12
        Me.CBLogFile.Text = "Logging aktivieren"
        Me.CBLogFile.UseVisualStyleBackColor = True
        '
        'Label23
        '
        Me.Label23.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label23.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.Location = New System.Drawing.Point(4, 5)
        Me.Label23.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(851, 54)
        Me.Label23.TabIndex = 24
        Me.Label23.Text = "Logging"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PDebug
        '
        Me.PDebug.Controls.Add(Me.GroupBox2)
        Me.PDebug.Controls.Add(Me.Label8)
        Me.PDebug.Location = New System.Drawing.Point(4, 29)
        Me.PDebug.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PDebug.Name = "PDebug"
        Me.PDebug.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PDebug.Size = New System.Drawing.Size(859, 459)
        Me.PDebug.TabIndex = 10
        Me.PDebug.Text = "Debug"
        Me.PDebug.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.BManLoad)
        Me.GroupBox2.Controls.Add(Me.BStartDebug)
        Me.GroupBox2.Controls.Add(Me.BZwischenablage)
        Me.GroupBox2.Controls.Add(Me.BProbleme)
        Me.GroupBox2.Controls.Add(Me.TBDiagnose)
        Me.GroupBox2.Location = New System.Drawing.Point(0, 63)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox2.Size = New System.Drawing.Size(855, 389)
        Me.GroupBox2.TabIndex = 30
        Me.GroupBox2.TabStop = False
        '
        'TBDiagnose
        '
        Me.TBDiagnose.BackColor = System.Drawing.SystemColors.Window
        Me.TBDiagnose.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TBDiagnose.Location = New System.Drawing.Point(3, 20)
        Me.TBDiagnose.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBDiagnose.Multiline = True
        Me.TBDiagnose.Name = "TBDiagnose"
        Me.TBDiagnose.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TBDiagnose.Size = New System.Drawing.Size(563, 337)
        Me.TBDiagnose.TabIndex = 31
        Me.TBDiagnose.TabStop = False
        '
        'Label8
        '
        Me.Label8.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(4, 5)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(851, 54)
        Me.Label8.TabIndex = 24
        Me.Label8.Text = "Einlesen der eingerichteten Telefone"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        'formCfg
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
        Me.Name = "formCfg"
        Me.Text = "Einstellungen für das Fritz!Box Telefon-Dingsbums"
        Me.PGrundeinstellungen.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.GroupBoxStoppUhr.ResumeLayout(False)
        Me.GroupBoxStoppUhr.PerformLayout()
        Me.Frame3.ResumeLayout(False)
        Me.Frame3.PerformLayout()
        Me.FrameErforderlich.ResumeLayout(False)
        Me.FrameErforderlich.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.PInfo.ResumeLayout(False)
        Me.PInfo.PerformLayout()
        Me.PTelefone.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.TelList, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PKontaktsuche.ResumeLayout(False)
        Me.GroupBoxIndex.ResumeLayout(False)
        Me.GroupBoxIndex.PerformLayout()
        Me.GroupBoxRWS.ResumeLayout(False)
        Me.GroupBoxRWS.PerformLayout()
        Me.PAnrufmonitor.ResumeLayout(False)
        Me.GBoxAnrMonLayout.ResumeLayout(False)
        Me.GBoxAnrMonLayout.PerformLayout()
        CType(Me.TBAnrMonMoveGeschwindigkeit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GBoxAnrMonMain.ResumeLayout(False)
        Me.GBoxAnrMonMain.PerformLayout()
        Me.PanelAnrMon.ResumeLayout(False)
        Me.PanelAnrMon.PerformLayout()
        Me.FBDB_MP.ResumeLayout(False)
        Me.PDiverses.ResumeLayout(False)
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.GBoxJournal.ResumeLayout(False)
        Me.GBoxJournal.PerformLayout()
        Me.GboxAnrListeMain.ResumeLayout(False)
        Me.GboxAnrListeMain.PerformLayout()
        Me.PPhoner.ResumeLayout(False)
        Me.PPhoner.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.PanelPhoner.ResumeLayout(False)
        Me.PanelPhoner.PerformLayout()
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox7.PerformLayout()
        Me.PLogging.ResumeLayout(False)
        Me.PLogging.PerformLayout()
        Me.GBLogging.ResumeLayout(False)
        Me.GBLogging.PerformLayout()
        Me.PDebug.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
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
    Friend WithEvents PKontaktsuche As System.Windows.Forms.TabPage
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents GroupBoxIndex As System.Windows.Forms.GroupBox
    Friend WithEvents CBIndexAus As System.Windows.Forms.CheckBox
    Friend WithEvents CBKHO As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBoxRWS As System.Windows.Forms.GroupBox
    Friend WithEvents CBRWSIndex As System.Windows.Forms.CheckBox
    Friend WithEvents ComboBoxRWS As System.Windows.Forms.ComboBox
    Friend WithEvents CBKErstellen As System.Windows.Forms.CheckBox
    Friend WithEvents CBRWS As System.Windows.Forms.CheckBox
    Friend WithEvents PAnrufmonitor As System.Windows.Forms.TabPage
    Friend WithEvents GBoxAnrMonLayout As System.Windows.Forms.GroupBox
    Friend WithEvents CBAnrMonContactImage As System.Windows.Forms.CheckBox
    Friend WithEvents CBShowMSN As System.Windows.Forms.CheckBox
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents BAnrMonTest As System.Windows.Forms.Button
    Friend WithEvents CBAnrMonMove As System.Windows.Forms.CheckBox
    Friend WithEvents CBAnrMonTransp As System.Windows.Forms.CheckBox
    Friend WithEvents TBAnrMonY As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents TBAnrMonX As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents TBAnrMonMoveGeschwindigkeit As System.Windows.Forms.TrackBar
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents GBoxAnrMonMain As System.Windows.Forms.GroupBox
    Friend WithEvents PanelAnrMon As System.Windows.Forms.Panel
    Friend WithEvents CBAnrMonAuto As System.Windows.Forms.CheckBox
    Friend WithEvents CBAutoClose As System.Windows.Forms.CheckBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TBEnblDauer As System.Windows.Forms.TextBox
    Friend WithEvents CLBTelNr As System.Windows.Forms.CheckedListBox
    Friend WithEvents LEnblDauer As System.Windows.Forms.Label
    Friend WithEvents CBUseAnrMon As System.Windows.Forms.CheckBox
    Friend WithEvents PGrundeinstellungen As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents CBIgnoTelNrFormat As System.Windows.Forms.CheckBox
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents TBTelNrMaske As System.Windows.Forms.TextBox
    Friend WithEvents CBintl As System.Windows.Forms.CheckBox
    Friend WithEvents CBTelNrGruppieren As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBoxStoppUhr As System.Windows.Forms.GroupBox
    Friend WithEvents LabelStoppUhr As System.Windows.Forms.Label
    Friend WithEvents TBStoppUhr As System.Windows.Forms.TextBox
    Friend WithEvents CBStoppUhrAusblenden As System.Windows.Forms.CheckBox
    Friend WithEvents CBStoppUhrEinblenden As System.Windows.Forms.CheckBox
    Friend WithEvents Frame3 As System.Windows.Forms.GroupBox
    Friend WithEvents CBCheckMobil As System.Windows.Forms.CheckBox
    Friend WithEvents CBVoIPBuster As System.Windows.Forms.CheckBox
    Friend WithEvents CBCbCunterbinden As System.Windows.Forms.CheckBox
    Friend WithEvents CBCallByCall As System.Windows.Forms.CheckBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents TBAmt As System.Windows.Forms.TextBox
    Friend WithEvents FrameErforderlich As System.Windows.Forms.GroupBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblTBPasswort As System.Windows.Forms.Label
    Friend WithEvents TBFBAdr As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents FBDB_MP As System.Windows.Forms.TabControl
    Friend WithEvents LinkForum As System.Windows.Forms.LinkLabel
    Friend WithEvents CBDialPort As System.Windows.Forms.CheckBox
    Friend WithEvents LabelAnzahl As System.Windows.Forms.Label
    Friend WithEvents BIndizierungAbbrechen As System.Windows.Forms.Button
    Friend WithEvents BIndizierungStart As System.Windows.Forms.Button
    Friend WithEvents ProgressBarIndex As System.Windows.Forms.ProgressBar
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents RadioButtonEntfernen As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButtonErstelle As System.Windows.Forms.RadioButton
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents CBForceFBAddr As System.Windows.Forms.CheckBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TBBenutzer As System.Windows.Forms.TextBox
    Friend WithEvents LinkHomepage As System.Windows.Forms.LinkLabel
    Friend WithEvents PLogging As System.Windows.Forms.TabPage
    Friend WithEvents CBLogFile As System.Windows.Forms.CheckBox
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents TBLogging As System.Windows.Forms.TextBox
    Friend WithEvents GBLogging As System.Windows.Forms.GroupBox
    Friend WithEvents LinkLogFile As System.Windows.Forms.LinkLabel
    Friend WithEvents BLogging As System.Windows.Forms.Button
    Friend WithEvents PPhoner As System.Windows.Forms.TabPage
    Friend WithEvents CBPhonerAnrMon As System.Windows.Forms.CheckBox
    Friend WithEvents CBPhoner As System.Windows.Forms.CheckBox
    Friend WithEvents ComboBoxPhonerSIP As System.Windows.Forms.ComboBox
    Friend WithEvents LinkPhoner As System.Windows.Forms.LinkLabel
    Friend WithEvents LabelPhoner As System.Windows.Forms.Label
    Friend WithEvents LPassworPhoner As System.Windows.Forms.Label
    Friend WithEvents TBPhonerPasswort As System.Windows.Forms.MaskedTextBox
    Friend WithEvents TBPasswort As System.Windows.Forms.MaskedTextBox
    Friend WithEvents PanelPhoner As System.Windows.Forms.Panel
    Friend WithEvents BPhoner As System.Windows.Forms.Button
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents GroupBox7 As System.Windows.Forms.GroupBox
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents PanelPhonerAktiv As System.Windows.Forms.Panel
    Friend WithEvents BXML As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents LTelStatus As System.Windows.Forms.Label
    Friend WithEvents BResetStat As System.Windows.Forms.Button
    Friend WithEvents BTelefonliste As System.Windows.Forms.Button
    Friend WithEvents TBAnderes As System.Windows.Forms.Label
    Friend WithEvents TBSchließZeit As System.Windows.Forms.Label
    Friend WithEvents TBReset As System.Windows.Forms.Label
    Friend WithEvents TelList As System.Windows.Forms.DataGridView
    Friend WithEvents ColumnStandardTelefon As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents Nr As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents dialCode As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Typ As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Telefonname As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents OutNr As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Eingehend As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Ausgehend As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Gesamt As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PDebug As System.Windows.Forms.TabPage
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents BStoppUhrAnzeigen As System.Windows.Forms.Button
    Friend WithEvents CBAnrMonZeigeKontakt As System.Windows.Forms.CheckBox
    Friend WithEvents CBoxAnrMonMoveDirection As System.Windows.Forms.ComboBox
    Friend WithEvents CBoxAnrMonStartPosition As System.Windows.Forms.ComboBox
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents LAnrMonMoveDirection As System.Windows.Forms.Label
    Friend WithEvents CBNote As System.Windows.Forms.CheckBox
    Friend WithEvents TVOutlookContact As System.Windows.Forms.TreeView
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents BArbeitsverzeichnis As System.Windows.Forms.Button
    Friend WithEvents TBVorwahl As System.Windows.Forms.TextBox
    Friend WithEvents TBLandesVW As System.Windows.Forms.TextBox
    Friend WithEvents BRWSTest As System.Windows.Forms.Button
    Friend WithEvents LRWSTest As System.Windows.Forms.Label
    Friend WithEvents TBRWSTest As System.Windows.Forms.TextBox
    Friend WithEvents BTestLogin As System.Windows.Forms.Button
    Friend WithEvents CBAnrMonCloseAtDISSCONNECT As System.Windows.Forms.CheckBox
    Friend WithEvents CBStoppUhrIgnIntFax As System.Windows.Forms.CheckBox
    Friend WithEvents PDiverses As System.Windows.Forms.TabPage
    Friend WithEvents GboxAnrListeMain As System.Windows.Forms.GroupBox
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents CBAutoAnrList As System.Windows.Forms.CheckBox
    Friend WithEvents CBJournal As System.Windows.Forms.CheckBox
    Friend WithEvents CBAnrListeUpdateCallLists As System.Windows.Forms.CheckBox
    Friend WithEvents GBoxJournal As System.Windows.Forms.GroupBox
    Friend WithEvents CBAnrListeUpdateJournal As System.Windows.Forms.CheckBox
    Friend WithEvents CBAnrListeShowAnrMon As System.Windows.Forms.CheckBox
    Friend WithEvents TBAnrBeantworterTimeout As System.Windows.Forms.TextBox
    Friend WithEvents LAnrBeantworterTimeout As System.Windows.Forms.Label
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents RBFBComUPnP As System.Windows.Forms.RadioButton
    Friend WithEvents RBFBComWeb As System.Windows.Forms.RadioButton
    Friend WithEvents GroupBox2 As Windows.Forms.GroupBox
    Friend WithEvents BStartDebug As Windows.Forms.Button
    Friend WithEvents BZwischenablage As Windows.Forms.Button
    Friend WithEvents BProbleme As Windows.Forms.Button
    Friend WithEvents TBDiagnose As Windows.Forms.TextBox
    Friend WithEvents CBTelFormKorr As Windows.Forms.CheckBox
    Friend WithEvents GroupBox6 As Windows.Forms.GroupBox
    Friend WithEvents TBNumEntryList As Windows.Forms.TextBox
    Friend WithEvents Label9 As Windows.Forms.Label
    Friend WithEvents BManLoad As Windows.Forms.Button

End Class
