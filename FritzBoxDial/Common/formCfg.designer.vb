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
        Me.ButtonZuruecksetzen = New System.Windows.Forms.Button()
        Me.ButtonAbbruch = New System.Windows.Forms.Button()
        Me.ButtonUebernehmen = New System.Windows.Forms.Button()
        Me.ButtonOK = New System.Windows.Forms.Button()
        Me.ToolTipFBDBConfig = New System.Windows.Forms.ToolTip(Me.components)
        Me.CBKErstellen = New System.Windows.Forms.CheckBox()
        Me.CBRWSIndex = New System.Windows.Forms.CheckBox()
        Me.CBKHO = New System.Windows.Forms.CheckBox()
        Me.CBIndexAus = New System.Windows.Forms.CheckBox()
        Me.CBJImport = New System.Windows.Forms.CheckBox()
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
        Me.TBPasswort = New System.Windows.Forms.MaskedTextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TBBenutzer = New System.Windows.Forms.TextBox()
        Me.CBForceFBAddr = New System.Windows.Forms.CheckBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TBLandesVW = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TBVorwahl = New System.Windows.Forms.TextBox()
        Me.lblTBPasswort = New System.Windows.Forms.Label()
        Me.TBFBAdr = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.BAnrMonTest = New System.Windows.Forms.Button()
        Me.BStartDebug = New System.Windows.Forms.Button()
        Me.BZwischenablage = New System.Windows.Forms.Button()
        Me.BProbleme = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.PInfo = New System.Windows.Forms.TabPage()
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
        Me.PIndex = New System.Windows.Forms.TabPage()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GroupBoxIndex = New System.Windows.Forms.GroupBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.RadioButtonEntfernen = New System.Windows.Forms.RadioButton()
        Me.RadioButtonErstelle = New System.Windows.Forms.RadioButton()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.LabelAnzahl = New System.Windows.Forms.Label()
        Me.BIndizierungAbbrechen = New System.Windows.Forms.Button()
        Me.BIndizierungStart = New System.Windows.Forms.Button()
        Me.ProgressBarIndex = New System.Windows.Forms.ProgressBar()
        Me.GroupBoxRWS = New System.Windows.Forms.GroupBox()
        Me.TVOutlookContact = New System.Windows.Forms.TreeView()
        Me.ComboBoxRWS = New System.Windows.Forms.ComboBox()
        Me.CBRWS = New System.Windows.Forms.CheckBox()
        Me.PAnrufmonitor = New System.Windows.Forms.TabPage()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.LAnrMonMoveDirection = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.CBoxAnrMonMoveDirection = New System.Windows.Forms.ComboBox()
        Me.CBoxAnrMonStartPosition = New System.Windows.Forms.ComboBox()
        Me.CBAnrMonContactImage = New System.Windows.Forms.CheckBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.TBAnrMonY = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.TBAnrMonX = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.TBAnrMonMoveGeschwindigkeit = New System.Windows.Forms.TrackBar()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Frame1 = New System.Windows.Forms.GroupBox()
        Me.PanelAnrMon = New System.Windows.Forms.Panel()
        Me.CBNote = New System.Windows.Forms.CheckBox()
        Me.CBAnrMonZeigeKontakt = New System.Windows.Forms.CheckBox()
        Me.CBAnrMonAuto = New System.Windows.Forms.CheckBox()
        Me.CBJournal = New System.Windows.Forms.CheckBox()
        Me.CBAutoClose = New System.Windows.Forms.CheckBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TBEnblDauer = New System.Windows.Forms.TextBox()
        Me.CLBTelNr = New System.Windows.Forms.CheckedListBox()
        Me.LEnblDauer = New System.Windows.Forms.Label()
        Me.CBUseAnrMon = New System.Windows.Forms.CheckBox()
        Me.FBDB_MP = New System.Windows.Forms.TabControl()
        Me.PSymbolleiste = New System.Windows.Forms.TabPage()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.CBSymbJournalimport = New System.Windows.Forms.CheckBox()
        Me.CBSymbVIP = New System.Windows.Forms.CheckBox()
        Me.CBSymbRWSuche = New System.Windows.Forms.CheckBox()
        Me.CBSymbDirekt = New System.Windows.Forms.CheckBox()
        Me.CBSymbAnrMonNeuStart = New System.Windows.Forms.CheckBox()
        Me.CBSymbWwdh = New System.Windows.Forms.CheckBox()
        Me.CBSymbAnrMon = New System.Windows.Forms.CheckBox()
        Me.CBSymbAnrListe = New System.Windows.Forms.CheckBox()
        Me.LabelSymb = New System.Windows.Forms.Label()
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
        Me.ButtonPhoner = New System.Windows.Forms.Button()
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
        Me.PTelefonDatei = New System.Windows.Forms.Panel()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.TBTelefonDatei = New System.Windows.Forms.TextBox()
        Me.BTelefonDatei = New System.Windows.Forms.Button()
        Me.CBTelefonDatei = New System.Windows.Forms.CheckBox()
        Me.TBDiagnose = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.ButtonXML = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.ButtonArbeitsverzeichnis = New System.Windows.Forms.Button()
        Me.PGrundeinstellungen.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBoxStoppUhr.SuspendLayout()
        Me.Frame3.SuspendLayout()
        Me.FrameErforderlich.SuspendLayout()
        Me.PInfo.SuspendLayout()
        Me.PTelefone.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.TelList, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PIndex.SuspendLayout()
        Me.GroupBoxIndex.SuspendLayout()
        Me.GroupBoxRWS.SuspendLayout()
        Me.PAnrufmonitor.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        CType(Me.TBAnrMonMoveGeschwindigkeit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Frame1.SuspendLayout()
        Me.PanelAnrMon.SuspendLayout()
        Me.FBDB_MP.SuspendLayout()
        Me.PSymbolleiste.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.PPhoner.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.PanelPhoner.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.PLogging.SuspendLayout()
        Me.GBLogging.SuspendLayout()
        Me.PDebug.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.PTelefonDatei.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'ButtonZuruecksetzen
        '
        Me.ButtonZuruecksetzen.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonZuruecksetzen.Location = New System.Drawing.Point(348, 3)
        Me.ButtonZuruecksetzen.Name = "ButtonZuruecksetzen"
        Me.ButtonZuruecksetzen.Size = New System.Drawing.Size(109, 28)
        Me.ButtonZuruecksetzen.TabIndex = 4
        Me.ButtonZuruecksetzen.Text = "Zurücksetzen"
        Me.ButtonZuruecksetzen.UseVisualStyleBackColor = True
        '
        'ButtonAbbruch
        '
        Me.ButtonAbbruch.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonAbbruch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonAbbruch.Location = New System.Drawing.Point(233, 3)
        Me.ButtonAbbruch.Name = "ButtonAbbruch"
        Me.ButtonAbbruch.Size = New System.Drawing.Size(109, 28)
        Me.ButtonAbbruch.TabIndex = 3
        Me.ButtonAbbruch.Text = "Abbruch"
        Me.ButtonAbbruch.UseVisualStyleBackColor = True
        '
        'ButtonUebernehmen
        '
        Me.ButtonUebernehmen.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonUebernehmen.Location = New System.Drawing.Point(118, 3)
        Me.ButtonUebernehmen.Name = "ButtonUebernehmen"
        Me.ButtonUebernehmen.Size = New System.Drawing.Size(109, 28)
        Me.ButtonUebernehmen.TabIndex = 2
        Me.ButtonUebernehmen.Text = "Übernehmen"
        Me.ButtonUebernehmen.UseVisualStyleBackColor = True
        '
        'ButtonOK
        '
        Me.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.ButtonOK.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonOK.Location = New System.Drawing.Point(3, 3)
        Me.ButtonOK.Name = "ButtonOK"
        Me.ButtonOK.Size = New System.Drawing.Size(109, 28)
        Me.ButtonOK.TabIndex = 1
        Me.ButtonOK.Text = "OK"
        Me.ButtonOK.UseVisualStyleBackColor = True
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
        Me.CBKErstellen.Location = New System.Drawing.Point(6, 42)
        Me.CBKErstellen.Name = "CBKErstellen"
        Me.CBKErstellen.Size = New System.Drawing.Size(217, 17)
        Me.CBKErstellen.TabIndex = 2
        Me.CBKErstellen.Text = "Kontakt bei erfolgreicher Suche erstellen"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBKErstellen, "Nach erfolgreicher Rückwärtssuche, wird bei dieser Einstellung ein neuer Kontakt " & _
        "erstellt.")
        Me.CBKErstellen.UseVisualStyleBackColor = True
        '
        'CBRWSIndex
        '
        Me.CBRWSIndex.AutoSize = True
        Me.CBRWSIndex.Enabled = False
        Me.CBRWSIndex.Location = New System.Drawing.Point(6, 65)
        Me.CBRWSIndex.Name = "CBRWSIndex"
        Me.CBRWSIndex.Size = New System.Drawing.Size(245, 17)
        Me.CBRWSIndex.TabIndex = 3
        Me.CBRWSIndex.Text = "Speichere Ergebnisse der Rückwärtssuche ab"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBRWSIndex, resources.GetString("CBRWSIndex.ToolTip"))
        Me.CBRWSIndex.UseVisualStyleBackColor = True
        '
        'CBKHO
        '
        Me.CBKHO.AutoSize = True
        Me.CBKHO.Location = New System.Drawing.Point(6, 19)
        Me.CBKHO.Name = "CBKHO"
        Me.CBKHO.Size = New System.Drawing.Size(227, 17)
        Me.CBKHO.TabIndex = 5
        Me.CBKHO.Text = "Nur den Hauptkontaktordner durchsuchen"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBKHO, resources.GetString("CBKHO.ToolTip"))
        Me.CBKHO.UseVisualStyleBackColor = True
        '
        'CBIndexAus
        '
        Me.CBIndexAus.AutoSize = True
        Me.CBIndexAus.Enabled = False
        Me.CBIndexAus.Location = New System.Drawing.Point(248, 19)
        Me.CBIndexAus.Name = "CBIndexAus"
        Me.CBIndexAus.Size = New System.Drawing.Size(142, 17)
        Me.CBIndexAus.TabIndex = 6
        Me.CBIndexAus.Text = """Indizierung"" auschalten"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBIndexAus, "Wenn Sie den Anrufmonitor nicht verwenden, können sie die Indizierung auch aussch" & _
        "alten.")
        Me.CBIndexAus.UseVisualStyleBackColor = True
        '
        'CBJImport
        '
        Me.CBJImport.AutoSize = True
        Me.CBJImport.Enabled = False
        Me.CBJImport.Location = New System.Drawing.Point(8, 156)
        Me.CBJImport.Name = "CBJImport"
        Me.CBJImport.Size = New System.Drawing.Size(202, 17)
        Me.CBJImport.TabIndex = 6
        Me.CBJImport.Text = "Journaleinträge beim Start importieren"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBJImport, resources.GetString("CBJImport.ToolTip"))
        Me.CBJImport.UseVisualStyleBackColor = True
        '
        'CBAnrMonTransp
        '
        Me.CBAnrMonTransp.AutoSize = True
        Me.CBAnrMonTransp.Location = New System.Drawing.Point(4, 19)
        Me.CBAnrMonTransp.Name = "CBAnrMonTransp"
        Me.CBAnrMonTransp.Size = New System.Drawing.Size(136, 17)
        Me.CBAnrMonTransp.TabIndex = 7
        Me.CBAnrMonTransp.Text = "Verwende Transparenz"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBAnrMonTransp, "Wenn diese Einstellung gesetzt ist, wird der Anrufmonitor ein und ausgeblendet." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & _
        "Dazu wird die Transparenz des Anrufmonitors erhöht, bzw. verringert.")
        Me.CBAnrMonTransp.UseVisualStyleBackColor = True
        '
        'CBAnrMonMove
        '
        Me.CBAnrMonMove.AutoSize = True
        Me.CBAnrMonMove.Location = New System.Drawing.Point(4, 42)
        Me.CBAnrMonMove.Name = "CBAnrMonMove"
        Me.CBAnrMonMove.Size = New System.Drawing.Size(186, 17)
        Me.CBAnrMonMove.TabIndex = 8
        Me.CBAnrMonMove.Text = "Verwende Anrufmonitorbewegung"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBAnrMonMove, "Wenn diese Einstellung gesetzt ist, wird der Anrufmonitor von unten in den Deskto" & _
        "p hinein geschoben.")
        Me.CBAnrMonMove.UseVisualStyleBackColor = True
        '
        'CBShowMSN
        '
        Me.CBShowMSN.AutoSize = True
        Me.CBShowMSN.Location = New System.Drawing.Point(4, 65)
        Me.CBShowMSN.Name = "CBShowMSN"
        Me.CBShowMSN.Size = New System.Drawing.Size(170, 17)
        Me.CBShowMSN.TabIndex = 9
        Me.CBShowMSN.Text = "Zeige MSN im Anrufmonitor an"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBShowMSN, "Wenn diese Einstellung gesetzt ist, wird die jeweilige MSN im Anrufmonitor angeze" & _
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
        Me.PGrundeinstellungen.Location = New System.Drawing.Point(4, 22)
        Me.PGrundeinstellungen.Name = "PGrundeinstellungen"
        Me.PGrundeinstellungen.Size = New System.Drawing.Size(570, 294)
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
        Me.GroupBox5.Location = New System.Drawing.Point(0, 195)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(282, 99)
        Me.GroupBox5.TabIndex = 17
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Telefonnummernformatierung"
        '
        'CBIgnoTelNrFormat
        '
        Me.CBIgnoTelNrFormat.AutoSize = True
        Me.CBIgnoTelNrFormat.Location = New System.Drawing.Point(6, 62)
        Me.CBIgnoTelNrFormat.Name = "CBIgnoTelNrFormat"
        Me.CBIgnoTelNrFormat.Size = New System.Drawing.Size(195, 17)
        Me.CBIgnoTelNrFormat.TabIndex = 15
        Me.CBIgnoTelNrFormat.Text = "Ignoriere Formatierung der Kontakte"
        Me.CBIgnoTelNrFormat.UseVisualStyleBackColor = True
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(90, 17)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(42, 13)
        Me.Label24.TabIndex = 3
        Me.Label24.Text = "Maske:"
        Me.ToolTipFBDBConfig.SetToolTip(Me.Label24, resources.GetString("Label24.ToolTip"))
        '
        'TBTelNrMaske
        '
        Me.TBTelNrMaske.Location = New System.Drawing.Point(138, 14)
        Me.TBTelNrMaske.Name = "TBTelNrMaske"
        Me.TBTelNrMaske.Size = New System.Drawing.Size(99, 20)
        Me.TBTelNrMaske.TabIndex = 13
        Me.ToolTipFBDBConfig.SetToolTip(Me.TBTelNrMaske, resources.GetString("TBTelNrMaske.ToolTip"))
        '
        'CBintl
        '
        Me.CBintl.AutoSize = True
        Me.CBintl.Location = New System.Drawing.Point(6, 39)
        Me.CBintl.Name = "CBintl"
        Me.CBintl.Size = New System.Drawing.Size(205, 17)
        Me.CBintl.TabIndex = 14
        Me.CBintl.Text = "Internatlionale Vorwahl immer anfügen"
        Me.CBintl.UseVisualStyleBackColor = True
        '
        'CBTelNrGruppieren
        '
        Me.CBTelNrGruppieren.AutoSize = True
        Me.CBTelNrGruppieren.Location = New System.Drawing.Point(6, 16)
        Me.CBTelNrGruppieren.Name = "CBTelNrGruppieren"
        Me.CBTelNrGruppieren.Size = New System.Drawing.Size(78, 17)
        Me.CBTelNrGruppieren.TabIndex = 12
        Me.CBTelNrGruppieren.Text = "Gruppieren"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBTelNrGruppieren, "Gruppiert Rufnummernteile in Zweierblöcke für bessere Lessbarkeit." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Beispiel:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "oh" & _
        "ne Gruppierung: +49 (123) 4567890 " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "mit Gruppierung: +49 (1 23) 4 56 78 90 ")
        Me.CBTelNrGruppieren.UseVisualStyleBackColor = True
        '
        'GroupBoxStoppUhr
        '
        Me.GroupBoxStoppUhr.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBoxStoppUhr.Controls.Add(Me.BStoppUhrAnzeigen)
        Me.GroupBoxStoppUhr.Controls.Add(Me.LabelStoppUhr)
        Me.GroupBoxStoppUhr.Controls.Add(Me.TBStoppUhr)
        Me.GroupBoxStoppUhr.Controls.Add(Me.CBStoppUhrAusblenden)
        Me.GroupBoxStoppUhr.Controls.Add(Me.CBStoppUhrEinblenden)
        Me.GroupBoxStoppUhr.Location = New System.Drawing.Point(288, 195)
        Me.GroupBoxStoppUhr.Name = "GroupBoxStoppUhr"
        Me.GroupBoxStoppUhr.Size = New System.Drawing.Size(282, 99)
        Me.GroupBoxStoppUhr.TabIndex = 19
        Me.GroupBoxStoppUhr.TabStop = False
        Me.GroupBoxStoppUhr.Text = "Stoppuhr"
        '
        'BStoppUhrAnzeigen
        '
        Me.BStoppUhrAnzeigen.Location = New System.Drawing.Point(167, 65)
        Me.BStoppUhrAnzeigen.Name = "BStoppUhrAnzeigen"
        Me.BStoppUhrAnzeigen.Size = New System.Drawing.Size(112, 28)
        Me.BStoppUhrAnzeigen.TabIndex = 19
        Me.BStoppUhrAnzeigen.Text = "Anzeigen"
        Me.BStoppUhrAnzeigen.UseVisualStyleBackColor = True
        '
        'LabelStoppUhr
        '
        Me.LabelStoppUhr.AutoSize = True
        Me.LabelStoppUhr.Location = New System.Drawing.Point(168, 40)
        Me.LabelStoppUhr.Name = "LabelStoppUhr"
        Me.LabelStoppUhr.Size = New System.Drawing.Size(76, 13)
        Me.LabelStoppUhr.TabIndex = 3
        Me.LabelStoppUhr.Text = "Sekunden aus"
        '
        'TBStoppUhr
        '
        Me.TBStoppUhr.Location = New System.Drawing.Point(134, 37)
        Me.TBStoppUhr.Name = "TBStoppUhr"
        Me.TBStoppUhr.Size = New System.Drawing.Size(30, 20)
        Me.TBStoppUhr.TabIndex = 18
        '
        'CBStoppUhrAusblenden
        '
        Me.CBStoppUhrAusblenden.AutoSize = True
        Me.CBStoppUhrAusblenden.Location = New System.Drawing.Point(6, 39)
        Me.CBStoppUhrAusblenden.Name = "CBStoppUhrAusblenden"
        Me.CBStoppUhrAusblenden.Size = New System.Drawing.Size(132, 17)
        Me.CBStoppUhrAusblenden.TabIndex = 17
        Me.CBStoppUhrAusblenden.Text = "Blende Stoppuhr nach"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBStoppUhrAusblenden, "Blendet die Stoppuhr nach Beendiging eines Telefonats aus." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10))
        Me.CBStoppUhrAusblenden.UseVisualStyleBackColor = True
        '
        'CBStoppUhrEinblenden
        '
        Me.CBStoppUhrEinblenden.AutoSize = True
        Me.CBStoppUhrEinblenden.Location = New System.Drawing.Point(6, 16)
        Me.CBStoppUhrEinblenden.Name = "CBStoppUhrEinblenden"
        Me.CBStoppUhrEinblenden.Size = New System.Drawing.Size(122, 17)
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
        Me.Frame3.Location = New System.Drawing.Point(288, 38)
        Me.Frame3.Name = "Frame3"
        Me.Frame3.Size = New System.Drawing.Size(282, 151)
        Me.Frame3.TabIndex = 18
        Me.Frame3.TabStop = False
        Me.Frame3.Text = "Einstellungen für die Wählhilfe"
        '
        'CBCallByCall
        '
        Me.CBCallByCall.AutoSize = True
        Me.CBCallByCall.Location = New System.Drawing.Point(148, 100)
        Me.CBCallByCall.Name = "CBCallByCall"
        Me.CBCallByCall.Size = New System.Drawing.Size(124, 17)
        Me.CBCallByCall.TabIndex = 10
        Me.CBCallByCall.Text = "Jedesmal Call-by-Call"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBCallByCall, "Call-by-Call ist eine Funktion, die es erlaubt günstig mit Vorvorwahlen zu telefo" & _
        "nieren. " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Um die aktuell günstigsten Vorvorwahlen zu ermitteln, wird Billiger-Te" & _
        "lefonieren.de verwendet.")
        Me.CBCallByCall.UseVisualStyleBackColor = True
        '
        'CBDialPort
        '
        Me.CBDialPort.AutoSize = True
        Me.CBDialPort.Location = New System.Drawing.Point(6, 73)
        Me.CBDialPort.Name = "CBDialPort"
        Me.CBDialPort.Size = New System.Drawing.Size(108, 17)
        Me.CBDialPort.TabIndex = 8
        Me.CBDialPort.Text = "Dialport anzeigen"
        Me.CBDialPort.UseVisualStyleBackColor = True
        '
        'CBCheckMobil
        '
        Me.CBCheckMobil.AutoSize = True
        Me.CBCheckMobil.Location = New System.Drawing.Point(6, 125)
        Me.CBCheckMobil.Name = "CBCheckMobil"
        Me.CBCheckMobil.Size = New System.Drawing.Size(233, 17)
        Me.CBCheckMobil.TabIndex = 11
        Me.CBCheckMobil.Text = "Nachfrage beim Wählen von Mobilnummern"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBCheckMobil, "Um unnötige Verbindungskosten zu Mobilfunkgeräten zu vermeiden, wird vor dem Wähl" & _
        "en eine zusätzliche Benutzereingabe erforderlich.")
        Me.CBCheckMobil.UseVisualStyleBackColor = True
        '
        'CBVoIPBuster
        '
        Me.CBVoIPBuster.AutoSize = True
        Me.CBVoIPBuster.Location = New System.Drawing.Point(6, 22)
        Me.CBVoIPBuster.Name = "CBVoIPBuster"
        Me.CBVoIPBuster.Size = New System.Drawing.Size(178, 17)
        Me.CBVoIPBuster.TabIndex = 6
        Me.CBVoIPBuster.Text = "Landesvorwahl immer mitwählen"
        Me.CBVoIPBuster.UseVisualStyleBackColor = True
        '
        'CBCbCunterbinden
        '
        Me.CBCbCunterbinden.AutoSize = True
        Me.CBCbCunterbinden.Location = New System.Drawing.Point(6, 99)
        Me.CBCbCunterbinden.Name = "CBCbCunterbinden"
        Me.CBCbCunterbinden.Size = New System.Drawing.Size(136, 17)
        Me.CBCbCunterbinden.TabIndex = 9
        Me.CBCbCunterbinden.Text = "Call-by-Call unterbinden"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBCbCunterbinden, "Mitunter ist es sinnvoll Call-by-Call Vorwahlen zu unterbinden, z.B. wenn Sie " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "k" & _
        "einen Festnetzanschluss haben und nur über Ihren Internetanbieter telefonieren.")
        Me.CBCbCunterbinden.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(42, 49)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(62, 13)
        Me.Label6.TabIndex = 31
        Me.Label6.Text = "Amtsholung"
        Me.ToolTipFBDBConfig.SetToolTip(Me.Label6, "Geben Sie hier eine 0 ein falls eine Amtsholung benötigt wird.")
        '
        'TBAmt
        '
        Me.TBAmt.Location = New System.Drawing.Point(6, 46)
        Me.TBAmt.Name = "TBAmt"
        Me.TBAmt.Size = New System.Drawing.Size(30, 20)
        Me.TBAmt.TabIndex = 7
        Me.ToolTipFBDBConfig.SetToolTip(Me.TBAmt, "Geben Sie hier eine 0 ein falls eine Amtsholung benötigt wird.")
        '
        'FrameErforderlich
        '
        Me.FrameErforderlich.Controls.Add(Me.TBPasswort)
        Me.FrameErforderlich.Controls.Add(Me.Label3)
        Me.FrameErforderlich.Controls.Add(Me.TBBenutzer)
        Me.FrameErforderlich.Controls.Add(Me.CBForceFBAddr)
        Me.FrameErforderlich.Controls.Add(Me.Label5)
        Me.FrameErforderlich.Controls.Add(Me.TBLandesVW)
        Me.FrameErforderlich.Controls.Add(Me.Label4)
        Me.FrameErforderlich.Controls.Add(Me.TBVorwahl)
        Me.FrameErforderlich.Controls.Add(Me.lblTBPasswort)
        Me.FrameErforderlich.Controls.Add(Me.TBFBAdr)
        Me.FrameErforderlich.Location = New System.Drawing.Point(0, 38)
        Me.FrameErforderlich.Name = "FrameErforderlich"
        Me.FrameErforderlich.Size = New System.Drawing.Size(282, 151)
        Me.FrameErforderlich.TabIndex = 16
        Me.FrameErforderlich.TabStop = False
        Me.FrameErforderlich.Text = "Erforderliche Angaben"
        '
        'TBPasswort
        '
        Me.TBPasswort.Location = New System.Drawing.Point(6, 71)
        Me.TBPasswort.Name = "TBPasswort"
        Me.TBPasswort.Size = New System.Drawing.Size(100, 20)
        Me.TBPasswort.TabIndex = 17
        Me.TBPasswort.UseSystemPasswordChar = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(113, 49)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(118, 13)
        Me.Label3.TabIndex = 16
        Me.Label3.Text = "Fritz!Box Benutzername"
        Me.ToolTipFBDBConfig.SetToolTip(Me.Label3, resources.GetString("Label3.ToolTip"))
        '
        'TBBenutzer
        '
        Me.TBBenutzer.Location = New System.Drawing.Point(6, 46)
        Me.TBBenutzer.Name = "TBBenutzer"
        Me.TBBenutzer.Size = New System.Drawing.Size(100, 20)
        Me.TBBenutzer.TabIndex = 2
        '
        'CBForceFBAddr
        '
        Me.CBForceFBAddr.AutoSize = True
        Me.CBForceFBAddr.Location = New System.Drawing.Point(116, 22)
        Me.CBForceFBAddr.Name = "CBForceFBAddr"
        Me.CBForceFBAddr.Size = New System.Drawing.Size(107, 17)
        Me.CBForceFBAddr.TabIndex = 1
        Me.CBForceFBAddr.Text = "Fritz!Box Adresse"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBForceFBAddr, "Wenn der Haken gesetzt wird, wird trotz fehlgeschlagener Ping-Check eine Verbindu" & _
        "ng zur eingegebenen Addresse aufgebaut." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Das ist z.B. bei einigen dyndns-Anbiete" & _
        "rn nötig, da diese Pings blockieren.")
        Me.CBForceFBAddr.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(113, 126)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(79, 13)
        Me.Label5.TabIndex = 13
        Me.Label5.Text = "Landesvorwahl" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'TBLandesVW
        '
        Me.TBLandesVW.Location = New System.Drawing.Point(6, 123)
        Me.TBLandesVW.Name = "TBLandesVW"
        Me.TBLandesVW.Size = New System.Drawing.Size(100, 20)
        Me.TBLandesVW.TabIndex = 5
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(113, 100)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(81, 13)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "Eigene Vorwahl" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'TBVorwahl
        '
        Me.TBVorwahl.Location = New System.Drawing.Point(6, 97)
        Me.TBVorwahl.Name = "TBVorwahl"
        Me.TBVorwahl.Size = New System.Drawing.Size(100, 20)
        Me.TBVorwahl.TabIndex = 4
        '
        'lblTBPasswort
        '
        Me.lblTBPasswort.AutoSize = True
        Me.lblTBPasswort.Location = New System.Drawing.Point(113, 75)
        Me.lblTBPasswort.Name = "lblTBPasswort"
        Me.lblTBPasswort.Size = New System.Drawing.Size(93, 13)
        Me.lblTBPasswort.TabIndex = 3
        Me.lblTBPasswort.Text = "Fritz!Box Passwort"
        '
        'TBFBAdr
        '
        Me.TBFBAdr.Location = New System.Drawing.Point(6, 20)
        Me.TBFBAdr.Name = "TBFBAdr"
        Me.TBFBAdr.Size = New System.Drawing.Size(100, 20)
        Me.TBFBAdr.TabIndex = 0
        '
        'Label13
        '
        Me.Label13.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(0, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(570, 35)
        Me.Label13.TabIndex = 23
        Me.Label13.Text = "Grundeinstellungen"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label33
        '
        Me.Label33.AutoSize = True
        Me.Label33.Location = New System.Drawing.Point(113, 73)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(63, 13)
        Me.Label33.TabIndex = 13
        Me.Label33.Text = "SIP-Telefon"
        Me.ToolTipFBDBConfig.SetToolTip(Me.Label33, "Geben Sie hier das SIP-Telefon, an welches mit Phoner verknüpft ist.")
        '
        'BAnrMonTest
        '
        Me.BAnrMonTest.Location = New System.Drawing.Point(166, 225)
        Me.BAnrMonTest.Name = "BAnrMonTest"
        Me.BAnrMonTest.Size = New System.Drawing.Size(112, 28)
        Me.BAnrMonTest.TabIndex = 14
        Me.BAnrMonTest.Text = "Anzeigen"
        Me.ToolTipFBDBConfig.SetToolTip(Me.BAnrMonTest, "Zeigt den Anrufmonitor testweise an.")
        Me.BAnrMonTest.UseVisualStyleBackColor = True
        '
        'BStartDebug
        '
        Me.BStartDebug.Location = New System.Drawing.Point(388, 35)
        Me.BStartDebug.Name = "BStartDebug"
        Me.BStartDebug.Size = New System.Drawing.Size(179, 28)
        Me.BStartDebug.TabIndex = 30
        Me.BStartDebug.Text = "Start"
        Me.ToolTipFBDBConfig.SetToolTip(Me.BStartDebug, "Kopiert den Statustext in die Zwischenablage")
        Me.BStartDebug.UseVisualStyleBackColor = True
        '
        'BZwischenablage
        '
        Me.BZwischenablage.Location = New System.Drawing.Point(388, 69)
        Me.BZwischenablage.Name = "BZwischenablage"
        Me.BZwischenablage.Size = New System.Drawing.Size(179, 28)
        Me.BZwischenablage.TabIndex = 32
        Me.BZwischenablage.Text = "Kopieren"
        Me.ToolTipFBDBConfig.SetToolTip(Me.BZwischenablage, "Kopiert den Statustext in die Zwischenablage")
        Me.BZwischenablage.UseVisualStyleBackColor = True
        '
        'BProbleme
        '
        Me.BProbleme.Location = New System.Drawing.Point(388, 103)
        Me.BProbleme.Name = "BProbleme"
        Me.BProbleme.Size = New System.Drawing.Size(179, 28)
        Me.BProbleme.TabIndex = 33
        Me.BProbleme.Text = "Probleme?"
        Me.ToolTipFBDBConfig.SetToolTip(Me.BProbleme, "Werden nicht alle Telefonnummern oder Telefone erkannt?" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Werden sie falsch zugeor" & _
        "dnet?" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Das Addin sammelt ein paar Informationen und schickt sie an den Entwick" & _
        "ler." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10))
        Me.BProbleme.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(254, 20)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(77, 13)
        Me.Label7.TabIndex = 5
        Me.Label7.Text = "Kontaktordner:"
        Me.ToolTipFBDBConfig.SetToolTip(Me.Label7, "Hier werden alle Kontakte gespeichert.")
        '
        'PInfo
        '
        Me.PInfo.Controls.Add(Me.ButtonArbeitsverzeichnis)
        Me.PInfo.Controls.Add(Me.Label17)
        Me.PInfo.Controls.Add(Me.Label16)
        Me.PInfo.Controls.Add(Me.Label10)
        Me.PInfo.Controls.Add(Me.LVersion)
        Me.PInfo.Controls.Add(Me.RichTextBox1)
        Me.PInfo.Controls.Add(Me.LinkHomepage)
        Me.PInfo.Controls.Add(Me.LinkEmail)
        Me.PInfo.Controls.Add(Me.LinkForum)
        Me.PInfo.Location = New System.Drawing.Point(4, 22)
        Me.PInfo.Name = "PInfo"
        Me.PInfo.Size = New System.Drawing.Size(570, 294)
        Me.PInfo.TabIndex = 4
        Me.PInfo.Text = "Info..."
        Me.PInfo.UseVisualStyleBackColor = True
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(3, 66)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(319, 13)
        Me.Label17.TabIndex = 0
        Me.Label17.Text = "Der SourceCode zu diesem AddIn steht auf GitHub zur Verfügung:"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(3, 50)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(150, 13)
        Me.Label16.TabIndex = 2
        Me.Label16.Text = "Forum und aktuelle Versionen:"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(3, 34)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(47, 13)
        Me.Label10.TabIndex = 2
        Me.Label10.Text = "Kontakt:"
        '
        'LVersion
        '
        Me.LVersion.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LVersion.Location = New System.Drawing.Point(5, 9)
        Me.LVersion.Name = "LVersion"
        Me.LVersion.Size = New System.Drawing.Size(294, 16)
        Me.LVersion.TabIndex = 1
        Me.LVersion.Text = "Fritz!Box Telefon-Dingsbums "
        '
        'RichTextBox1
        '
        Me.RichTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.RichTextBox1.Location = New System.Drawing.Point(3, 95)
        Me.RichTextBox1.Name = "RichTextBox1"
        Me.RichTextBox1.Size = New System.Drawing.Size(562, 189)
        Me.RichTextBox1.TabIndex = 0
        Me.RichTextBox1.Text = resources.GetString("RichTextBox1.Text")
        '
        'LinkHomepage
        '
        Me.LinkHomepage.Location = New System.Drawing.Point(240, 66)
        Me.LinkHomepage.Name = "LinkHomepage"
        Me.LinkHomepage.Size = New System.Drawing.Size(150, 13)
        Me.LinkHomepage.TabIndex = 5
        Me.LinkHomepage.TabStop = True
        Me.LinkHomepage.Text = "GitHub"
        Me.LinkHomepage.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'LinkEmail
        '
        Me.LinkEmail.Location = New System.Drawing.Point(240, 34)
        Me.LinkEmail.Name = "LinkEmail"
        Me.LinkEmail.Size = New System.Drawing.Size(150, 13)
        Me.LinkEmail.TabIndex = 1
        Me.LinkEmail.TabStop = True
        Me.LinkEmail.Text = "kruemelino@gert-michael.de"
        Me.LinkEmail.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'LinkForum
        '
        Me.LinkForum.Location = New System.Drawing.Point(240, 50)
        Me.LinkForum.Name = "LinkForum"
        Me.LinkForum.Size = New System.Drawing.Size(150, 13)
        Me.LinkForum.TabIndex = 2
        Me.LinkForum.TabStop = True
        Me.LinkForum.Text = "www.ip-phone-forum.de"
        Me.LinkForum.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'PTelefone
        '
        Me.PTelefone.Controls.Add(Me.GroupBox1)
        Me.PTelefone.Controls.Add(Me.Label15)
        Me.PTelefone.Location = New System.Drawing.Point(4, 22)
        Me.PTelefone.Name = "PTelefone"
        Me.PTelefone.Padding = New System.Windows.Forms.Padding(3)
        Me.PTelefone.Size = New System.Drawing.Size(570, 294)
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
        Me.GroupBox1.Location = New System.Drawing.Point(0, 41)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(570, 253)
        Me.GroupBox1.TabIndex = 27
        Me.GroupBox1.TabStop = False
        '
        'LTelStatus
        '
        Me.LTelStatus.Location = New System.Drawing.Point(212, 203)
        Me.LTelStatus.Name = "LTelStatus"
        Me.LTelStatus.Size = New System.Drawing.Size(346, 13)
        Me.LTelStatus.TabIndex = 39
        Me.LTelStatus.Text = "Status: "
        '
        'BResetStat
        '
        Me.BResetStat.Location = New System.Drawing.Point(218, 219)
        Me.BResetStat.Name = "BResetStat"
        Me.BResetStat.Size = New System.Drawing.Size(170, 28)
        Me.BResetStat.TabIndex = 34
        Me.BResetStat.Text = "Statistik zurücksetzen"
        Me.BResetStat.UseVisualStyleBackColor = True
        '
        'BTelefonliste
        '
        Me.BTelefonliste.Location = New System.Drawing.Point(394, 219)
        Me.BTelefonliste.Name = "BTelefonliste"
        Me.BTelefonliste.Size = New System.Drawing.Size(170, 28)
        Me.BTelefonliste.TabIndex = 35
        Me.BTelefonliste.Text = "Telefone erneut einlesen"
        Me.BTelefonliste.UseVisualStyleBackColor = True
        '
        'TBAnderes
        '
        Me.TBAnderes.AutoSize = True
        Me.TBAnderes.Location = New System.Drawing.Point(3, 195)
        Me.TBAnderes.Name = "TBAnderes"
        Me.TBAnderes.Size = New System.Drawing.Size(53, 13)
        Me.TBAnderes.TabIndex = 38
        Me.TBAnderes.Text = "Sonstiges"
        '
        'TBSchließZeit
        '
        Me.TBSchließZeit.AutoSize = True
        Me.TBSchließZeit.Location = New System.Drawing.Point(3, 182)
        Me.TBSchließZeit.Name = "TBSchließZeit"
        Me.TBSchließZeit.Size = New System.Drawing.Size(74, 13)
        Me.TBSchließZeit.TabIndex = 36
        Me.TBSchließZeit.Text = "TBSchließZeit"
        '
        'TBReset
        '
        Me.TBReset.AutoSize = True
        Me.TBReset.Location = New System.Drawing.Point(3, 169)
        Me.TBReset.Name = "TBReset"
        Me.TBReset.Size = New System.Drawing.Size(49, 13)
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
        Me.TelList.Location = New System.Drawing.Point(3, 16)
        Me.TelList.MultiSelect = False
        Me.TelList.Name = "TelList"
        Me.TelList.RowHeadersVisible = False
        Me.TelList.RowTemplate.Height = 18
        Me.TelList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.TelList.Size = New System.Drawing.Size(564, 150)
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
        Me.Label15.Location = New System.Drawing.Point(3, 3)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(564, 35)
        Me.Label15.TabIndex = 26
        Me.Label15.Text = "Telefone und Statistik"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PIndex
        '
        Me.PIndex.Controls.Add(Me.Label1)
        Me.PIndex.Controls.Add(Me.GroupBoxIndex)
        Me.PIndex.Controls.Add(Me.GroupBoxRWS)
        Me.PIndex.Location = New System.Drawing.Point(4, 22)
        Me.PIndex.Name = "PIndex"
        Me.PIndex.Padding = New System.Windows.Forms.Padding(3)
        Me.PIndex.Size = New System.Drawing.Size(570, 294)
        Me.PIndex.TabIndex = 6
        Me.PIndex.Text = "Kontaktsuche"
        Me.PIndex.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.Label1.Location = New System.Drawing.Point(3, 3)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(564, 35)
        Me.Label1.TabIndex = 33
        Me.Label1.Text = "Einstellungen für die Kontaktsuche"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'GroupBoxIndex
        '
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
        Me.GroupBoxIndex.Location = New System.Drawing.Point(0, 41)
        Me.GroupBoxIndex.Name = "GroupBoxIndex"
        Me.GroupBoxIndex.Size = New System.Drawing.Size(570, 145)
        Me.GroupBoxIndex.TabIndex = 10
        Me.GroupBoxIndex.TabStop = False
        Me.GroupBoxIndex.Text = "Kontaktsuche in Outlook (Indizierung)"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(3, 63)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(124, 13)
        Me.Label21.TabIndex = 18
        Me.Label21.Text = "Benutzerdefinierte Felder"
        '
        'RadioButtonEntfernen
        '
        Me.RadioButtonEntfernen.AutoSize = True
        Me.RadioButtonEntfernen.Location = New System.Drawing.Point(248, 61)
        Me.RadioButtonEntfernen.Name = "RadioButtonEntfernen"
        Me.RadioButtonEntfernen.Size = New System.Drawing.Size(70, 17)
        Me.RadioButtonEntfernen.TabIndex = 8
        Me.RadioButtonEntfernen.TabStop = True
        Me.RadioButtonEntfernen.Text = "entfernen"
        Me.RadioButtonEntfernen.UseVisualStyleBackColor = True
        '
        'RadioButtonErstelle
        '
        Me.RadioButtonErstelle.AutoSize = True
        Me.RadioButtonErstelle.Checked = True
        Me.RadioButtonErstelle.Location = New System.Drawing.Point(149, 61)
        Me.RadioButtonErstelle.Name = "RadioButtonErstelle"
        Me.RadioButtonErstelle.Size = New System.Drawing.Size(64, 17)
        Me.RadioButtonErstelle.TabIndex = 7
        Me.RadioButtonErstelle.TabStop = True
        Me.RadioButtonErstelle.Text = "erstellen"
        Me.RadioButtonErstelle.UseVisualStyleBackColor = True
        '
        'Label20
        '
        Me.Label20.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label20.Location = New System.Drawing.Point(6, 34)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(567, 24)
        Me.Label20.TabIndex = 14
        Me.Label20.Text = "Falls keine Indizierung durgeführt wird, werden keine Kontakte im Anrufmonitor an" & _
    "gezeigt."
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LabelAnzahl
        '
        Me.LabelAnzahl.BackColor = System.Drawing.Color.Transparent
        Me.LabelAnzahl.Location = New System.Drawing.Point(3, 95)
        Me.LabelAnzahl.Name = "LabelAnzahl"
        Me.LabelAnzahl.Size = New System.Drawing.Size(564, 13)
        Me.LabelAnzahl.TabIndex = 13
        Me.LabelAnzahl.Text = "Status: "
        '
        'BIndizierungAbbrechen
        '
        Me.BIndizierungAbbrechen.Enabled = False
        Me.BIndizierungAbbrechen.Location = New System.Drawing.Point(455, 111)
        Me.BIndizierungAbbrechen.Name = "BIndizierungAbbrechen"
        Me.BIndizierungAbbrechen.Size = New System.Drawing.Size(112, 28)
        Me.BIndizierungAbbrechen.TabIndex = 10
        Me.BIndizierungAbbrechen.Text = "Abbrechen"
        Me.BIndizierungAbbrechen.UseVisualStyleBackColor = True
        '
        'BIndizierungStart
        '
        Me.BIndizierungStart.Location = New System.Drawing.Point(337, 111)
        Me.BIndizierungStart.Name = "BIndizierungStart"
        Me.BIndizierungStart.Size = New System.Drawing.Size(112, 28)
        Me.BIndizierungStart.TabIndex = 9
        Me.BIndizierungStart.Text = "Start"
        Me.BIndizierungStart.UseVisualStyleBackColor = True
        '
        'ProgressBarIndex
        '
        Me.ProgressBarIndex.Location = New System.Drawing.Point(6, 111)
        Me.ProgressBarIndex.Name = "ProgressBarIndex"
        Me.ProgressBarIndex.Size = New System.Drawing.Size(325, 28)
        Me.ProgressBarIndex.TabIndex = 10
        '
        'GroupBoxRWS
        '
        Me.GroupBoxRWS.Controls.Add(Me.Label7)
        Me.GroupBoxRWS.Controls.Add(Me.TVOutlookContact)
        Me.GroupBoxRWS.Controls.Add(Me.CBRWSIndex)
        Me.GroupBoxRWS.Controls.Add(Me.ComboBoxRWS)
        Me.GroupBoxRWS.Controls.Add(Me.CBKErstellen)
        Me.GroupBoxRWS.Controls.Add(Me.CBRWS)
        Me.GroupBoxRWS.Location = New System.Drawing.Point(0, 205)
        Me.GroupBoxRWS.Name = "GroupBoxRWS"
        Me.GroupBoxRWS.Size = New System.Drawing.Size(571, 89)
        Me.GroupBoxRWS.TabIndex = 9
        Me.GroupBoxRWS.TabStop = False
        Me.GroupBoxRWS.Text = "Rückwärtssuche"
        '
        'TVOutlookContact
        '
        Me.TVOutlookContact.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TVOutlookContact.Location = New System.Drawing.Point(257, 39)
        Me.TVOutlookContact.Name = "TVOutlookContact"
        Me.TVOutlookContact.Size = New System.Drawing.Size(307, 47)
        Me.TVOutlookContact.TabIndex = 4
        '
        'ComboBoxRWS
        '
        Me.ComboBoxRWS.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.ComboBoxRWS.FormattingEnabled = True
        Me.ComboBoxRWS.Location = New System.Drawing.Point(121, 17)
        Me.ComboBoxRWS.Name = "ComboBoxRWS"
        Me.ComboBoxRWS.Size = New System.Drawing.Size(121, 21)
        Me.ComboBoxRWS.TabIndex = 1
        '
        'CBRWS
        '
        Me.CBRWS.AutoSize = True
        Me.CBRWS.Location = New System.Drawing.Point(6, 19)
        Me.CBRWS.Name = "CBRWS"
        Me.CBRWS.Size = New System.Drawing.Size(109, 17)
        Me.CBRWS.TabIndex = 0
        Me.CBRWS.Text = "Rückwärtssuche:"
        Me.CBRWS.UseVisualStyleBackColor = True
        '
        'PAnrufmonitor
        '
        Me.PAnrufmonitor.Controls.Add(Me.GroupBox6)
        Me.PAnrufmonitor.Controls.Add(Me.Label22)
        Me.PAnrufmonitor.Controls.Add(Me.Frame1)
        Me.PAnrufmonitor.Location = New System.Drawing.Point(4, 22)
        Me.PAnrufmonitor.Name = "PAnrufmonitor"
        Me.PAnrufmonitor.Padding = New System.Windows.Forms.Padding(3)
        Me.PAnrufmonitor.Size = New System.Drawing.Size(570, 294)
        Me.PAnrufmonitor.TabIndex = 0
        Me.PAnrufmonitor.Text = "Anrufmonitor"
        Me.PAnrufmonitor.UseVisualStyleBackColor = True
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.LAnrMonMoveDirection)
        Me.GroupBox6.Controls.Add(Me.Label19)
        Me.GroupBox6.Controls.Add(Me.CBoxAnrMonMoveDirection)
        Me.GroupBox6.Controls.Add(Me.CBoxAnrMonStartPosition)
        Me.GroupBox6.Controls.Add(Me.CBAnrMonContactImage)
        Me.GroupBox6.Controls.Add(Me.CBShowMSN)
        Me.GroupBox6.Controls.Add(Me.Label18)
        Me.GroupBox6.Controls.Add(Me.Label32)
        Me.GroupBox6.Controls.Add(Me.Label28)
        Me.GroupBox6.Controls.Add(Me.BAnrMonTest)
        Me.GroupBox6.Controls.Add(Me.CBAnrMonMove)
        Me.GroupBox6.Controls.Add(Me.CBAnrMonTransp)
        Me.GroupBox6.Controls.Add(Me.TBAnrMonY)
        Me.GroupBox6.Controls.Add(Me.Label11)
        Me.GroupBox6.Controls.Add(Me.Label14)
        Me.GroupBox6.Controls.Add(Me.TBAnrMonX)
        Me.GroupBox6.Controls.Add(Me.Label12)
        Me.GroupBox6.Controls.Add(Me.TBAnrMonMoveGeschwindigkeit)
        Me.GroupBox6.Location = New System.Drawing.Point(288, 38)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(282, 256)
        Me.GroupBox6.TabIndex = 15
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Aussehen anpassen"
        '
        'LAnrMonMoveDirection
        '
        Me.LAnrMonMoveDirection.AutoSize = True
        Me.LAnrMonMoveDirection.Location = New System.Drawing.Point(133, 147)
        Me.LAnrMonMoveDirection.Name = "LAnrMonMoveDirection"
        Me.LAnrMonMoveDirection.Size = New System.Drawing.Size(84, 13)
        Me.LAnrMonMoveDirection.TabIndex = 913
        Me.LAnrMonMoveDirection.Text = "Einblenden von:"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(3, 148)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(47, 13)
        Me.Label19.TabIndex = 912
        Me.Label19.Text = "Position:"
        '
        'CBoxAnrMonMoveDirection
        '
        Me.CBoxAnrMonMoveDirection.FormattingEnabled = True
        Me.CBoxAnrMonMoveDirection.Items.AddRange(New Object() {"Oben/Unten", "Links/Rechts"})
        Me.CBoxAnrMonMoveDirection.Location = New System.Drawing.Point(133, 165)
        Me.CBoxAnrMonMoveDirection.Name = "CBoxAnrMonMoveDirection"
        Me.CBoxAnrMonMoveDirection.Size = New System.Drawing.Size(121, 21)
        Me.CBoxAnrMonMoveDirection.TabIndex = 911
        '
        'CBoxAnrMonStartPosition
        '
        Me.CBoxAnrMonStartPosition.FormattingEnabled = True
        Me.CBoxAnrMonStartPosition.Items.AddRange(New Object() {"Unten Rechts", "Unten Links", "Oben Links", "Oben Rechts"})
        Me.CBoxAnrMonStartPosition.Location = New System.Drawing.Point(6, 165)
        Me.CBoxAnrMonStartPosition.MaxDropDownItems = 4
        Me.CBoxAnrMonStartPosition.Name = "CBoxAnrMonStartPosition"
        Me.CBoxAnrMonStartPosition.Size = New System.Drawing.Size(121, 21)
        Me.CBoxAnrMonStartPosition.TabIndex = 911
        '
        'CBAnrMonContactImage
        '
        Me.CBAnrMonContactImage.AutoSize = True
        Me.CBAnrMonContactImage.Location = New System.Drawing.Point(4, 88)
        Me.CBAnrMonContactImage.Name = "CBAnrMonContactImage"
        Me.CBAnrMonContactImage.Size = New System.Drawing.Size(109, 17)
        Me.CBAnrMonContactImage.TabIndex = 910
        Me.CBAnrMonContactImage.Text = "Zeige Kontaktbild"
        Me.CBAnrMonContactImage.UseVisualStyleBackColor = True
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(3, 114)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(127, 13)
        Me.Label18.TabIndex = 26
        Me.Label18.Text = "Einblendgeschwindigkeit:"
        '
        'Label32
        '
        Me.Label32.AutoSize = True
        Me.Label32.Location = New System.Drawing.Point(207, 130)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(49, 13)
        Me.Label32.TabIndex = 27
        Me.Label32.Text = "schneller"
        Me.Label32.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Location = New System.Drawing.Point(135, 130)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(55, 13)
        Me.Label28.TabIndex = 27
        Me.Label28.Text = "langsamer"
        '
        'TBAnrMonY
        '
        Me.TBAnrMonY.AcceptsReturn = True
        Me.TBAnrMonY.Location = New System.Drawing.Point(6, 229)
        Me.TBAnrMonY.Name = "TBAnrMonY"
        Me.TBAnrMonY.Size = New System.Drawing.Size(30, 20)
        Me.TBAnrMonY.TabIndex = 13
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(42, 232)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(84, 13)
        Me.Label11.TabIndex = 13
        Me.Label11.Text = "Punkte (vertikal)"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(3, 189)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(181, 13)
        Me.Label14.TabIndex = 23
        Me.Label14.Text = "Positionskorrektur des Anrufmonitors:"
        '
        'TBAnrMonX
        '
        Me.TBAnrMonX.AcceptsReturn = True
        Me.TBAnrMonX.Location = New System.Drawing.Point(6, 205)
        Me.TBAnrMonX.Name = "TBAnrMonX"
        Me.TBAnrMonX.Size = New System.Drawing.Size(30, 20)
        Me.TBAnrMonX.TabIndex = 12
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(42, 208)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(95, 13)
        Me.Label12.TabIndex = 11
        Me.Label12.Text = "Punkte (horizontal)"
        '
        'TBAnrMonMoveGeschwindigkeit
        '
        Me.TBAnrMonMoveGeschwindigkeit.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.TBAnrMonMoveGeschwindigkeit.LargeChange = 2
        Me.TBAnrMonMoveGeschwindigkeit.Location = New System.Drawing.Point(131, 99)
        Me.TBAnrMonMoveGeschwindigkeit.Maximum = 9
        Me.TBAnrMonMoveGeschwindigkeit.Minimum = 1
        Me.TBAnrMonMoveGeschwindigkeit.Name = "TBAnrMonMoveGeschwindigkeit"
        Me.TBAnrMonMoveGeschwindigkeit.Size = New System.Drawing.Size(123, 45)
        Me.TBAnrMonMoveGeschwindigkeit.TabIndex = 11
        Me.TBAnrMonMoveGeschwindigkeit.TickStyle = System.Windows.Forms.TickStyle.TopLeft
        Me.TBAnrMonMoveGeschwindigkeit.Value = 5
        '
        'Label22
        '
        Me.Label22.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label22.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.Location = New System.Drawing.Point(3, 3)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(564, 35)
        Me.Label22.TabIndex = 29
        Me.Label22.Text = "Einstellungen für den Anrufmonitor"
        Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Frame1
        '
        Me.Frame1.Controls.Add(Me.PanelAnrMon)
        Me.Frame1.Controls.Add(Me.CBUseAnrMon)
        Me.Frame1.Location = New System.Drawing.Point(0, 38)
        Me.Frame1.Name = "Frame1"
        Me.Frame1.Size = New System.Drawing.Size(282, 256)
        Me.Frame1.TabIndex = 14
        Me.Frame1.TabStop = False
        Me.Frame1.Text = "Einstellungen für den Anrufmonitor"
        '
        'PanelAnrMon
        '
        Me.PanelAnrMon.Controls.Add(Me.CBNote)
        Me.PanelAnrMon.Controls.Add(Me.CBAnrMonZeigeKontakt)
        Me.PanelAnrMon.Controls.Add(Me.CBAnrMonAuto)
        Me.PanelAnrMon.Controls.Add(Me.CBJImport)
        Me.PanelAnrMon.Controls.Add(Me.CBJournal)
        Me.PanelAnrMon.Controls.Add(Me.CBAutoClose)
        Me.PanelAnrMon.Controls.Add(Me.Label2)
        Me.PanelAnrMon.Controls.Add(Me.TBEnblDauer)
        Me.PanelAnrMon.Controls.Add(Me.CLBTelNr)
        Me.PanelAnrMon.Controls.Add(Me.LEnblDauer)
        Me.PanelAnrMon.Location = New System.Drawing.Point(0, 36)
        Me.PanelAnrMon.Name = "PanelAnrMon"
        Me.PanelAnrMon.Size = New System.Drawing.Size(282, 220)
        Me.PanelAnrMon.TabIndex = 35
        '
        'CBNote
        '
        Me.CBNote.AutoSize = True
        Me.CBNote.Location = New System.Drawing.Point(7, 202)
        Me.CBNote.Name = "CBNote"
        Me.CBNote.Size = New System.Drawing.Size(184, 17)
        Me.CBNote.TabIndex = 34
        Me.CBNote.Text = "Notizeintrag automatisch erstellen"
        Me.CBNote.UseVisualStyleBackColor = True
        '
        'CBAnrMonZeigeKontakt
        '
        Me.CBAnrMonZeigeKontakt.AutoSize = True
        Me.CBAnrMonZeigeKontakt.Location = New System.Drawing.Point(7, 179)
        Me.CBAnrMonZeigeKontakt.Name = "CBAnrMonZeigeKontakt"
        Me.CBAnrMonZeigeKontakt.Size = New System.Drawing.Size(141, 17)
        Me.CBAnrMonZeigeKontakt.TabIndex = 34
        Me.CBAnrMonZeigeKontakt.Text = "Kontakt bei Anruf öffnen"
        Me.CBAnrMonZeigeKontakt.UseVisualStyleBackColor = True
        '
        'CBAnrMonAuto
        '
        Me.CBAnrMonAuto.AutoSize = True
        Me.CBAnrMonAuto.Location = New System.Drawing.Point(8, 6)
        Me.CBAnrMonAuto.Name = "CBAnrMonAuto"
        Me.CBAnrMonAuto.Size = New System.Drawing.Size(176, 17)
        Me.CBAnrMonAuto.TabIndex = 1
        Me.CBAnrMonAuto.Text = "Anrufmonitor mit Outlook starten"
        Me.CBAnrMonAuto.UseVisualStyleBackColor = True
        '
        'CBJournal
        '
        Me.CBJournal.AutoSize = True
        Me.CBJournal.Location = New System.Drawing.Point(8, 133)
        Me.CBJournal.Name = "CBJournal"
        Me.CBJournal.Size = New System.Drawing.Size(153, 17)
        Me.CBJournal.TabIndex = 5
        Me.CBJournal.Text = "Journaleinträge hinzufügen"
        Me.CBJournal.UseVisualStyleBackColor = True
        '
        'CBAutoClose
        '
        Me.CBAutoClose.AutoSize = True
        Me.CBAutoClose.Location = New System.Drawing.Point(8, 29)
        Me.CBAutoClose.Name = "CBAutoClose"
        Me.CBAutoClose.Size = New System.Drawing.Size(191, 17)
        Me.CBAutoClose.TabIndex = 2
        Me.CBAutoClose.Text = "Anruffenster automatisch schließen"
        Me.CBAutoClose.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(162, 78)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(114, 52)
        Me.Label2.TabIndex = 33
        Me.Label2.Text = "Stellen Sie die zu überwachenden Telefonnummern ein."
        '
        'TBEnblDauer
        '
        Me.TBEnblDauer.Location = New System.Drawing.Point(8, 52)
        Me.TBEnblDauer.Name = "TBEnblDauer"
        Me.TBEnblDauer.Size = New System.Drawing.Size(29, 20)
        Me.TBEnblDauer.TabIndex = 3
        '
        'CLBTelNr
        '
        Me.CLBTelNr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CLBTelNr.CheckOnClick = True
        Me.CLBTelNr.FormattingEnabled = True
        Me.CLBTelNr.HorizontalScrollbar = True
        Me.CLBTelNr.Location = New System.Drawing.Point(8, 78)
        Me.CLBTelNr.Name = "CLBTelNr"
        Me.CLBTelNr.Size = New System.Drawing.Size(148, 47)
        Me.CLBTelNr.TabIndex = 4
        '
        'LEnblDauer
        '
        Me.LEnblDauer.AutoSize = True
        Me.LEnblDauer.Location = New System.Drawing.Point(43, 55)
        Me.LEnblDauer.Name = "LEnblDauer"
        Me.LEnblDauer.Size = New System.Drawing.Size(191, 13)
        Me.LEnblDauer.TabIndex = 31
        Me.LEnblDauer.Text = "Anzeigedauer bei Anruf (minimal: 4s) [s]"
        '
        'CBUseAnrMon
        '
        Me.CBUseAnrMon.AutoSize = True
        Me.CBUseAnrMon.Location = New System.Drawing.Point(8, 19)
        Me.CBUseAnrMon.Name = "CBUseAnrMon"
        Me.CBUseAnrMon.Size = New System.Drawing.Size(141, 17)
        Me.CBUseAnrMon.TabIndex = 0
        Me.CBUseAnrMon.Text = "Anrufmonitor verwenden"
        Me.CBUseAnrMon.UseVisualStyleBackColor = True
        '
        'FBDB_MP
        '
        Me.FBDB_MP.Controls.Add(Me.PGrundeinstellungen)
        Me.FBDB_MP.Controls.Add(Me.PAnrufmonitor)
        Me.FBDB_MP.Controls.Add(Me.PIndex)
        Me.FBDB_MP.Controls.Add(Me.PSymbolleiste)
        Me.FBDB_MP.Controls.Add(Me.PTelefone)
        Me.FBDB_MP.Controls.Add(Me.PPhoner)
        Me.FBDB_MP.Controls.Add(Me.PLogging)
        Me.FBDB_MP.Controls.Add(Me.PDebug)
        Me.FBDB_MP.Controls.Add(Me.PInfo)
        Me.FBDB_MP.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FBDB_MP.Location = New System.Drawing.Point(3, 3)
        Me.FBDB_MP.Name = "FBDB_MP"
        Me.FBDB_MP.SelectedIndex = 0
        Me.FBDB_MP.Size = New System.Drawing.Size(578, 320)
        Me.FBDB_MP.TabIndex = 1
        '
        'PSymbolleiste
        '
        Me.PSymbolleiste.Controls.Add(Me.GroupBox3)
        Me.PSymbolleiste.Controls.Add(Me.LabelSymb)
        Me.PSymbolleiste.Location = New System.Drawing.Point(4, 22)
        Me.PSymbolleiste.Name = "PSymbolleiste"
        Me.PSymbolleiste.Size = New System.Drawing.Size(570, 294)
        Me.PSymbolleiste.TabIndex = 9
        Me.PSymbolleiste.Text = "Symbolleiste"
        Me.PSymbolleiste.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.CBSymbJournalimport)
        Me.GroupBox3.Controls.Add(Me.CBSymbVIP)
        Me.GroupBox3.Controls.Add(Me.CBSymbRWSuche)
        Me.GroupBox3.Controls.Add(Me.CBSymbDirekt)
        Me.GroupBox3.Controls.Add(Me.CBSymbAnrMonNeuStart)
        Me.GroupBox3.Controls.Add(Me.CBSymbWwdh)
        Me.GroupBox3.Controls.Add(Me.CBSymbAnrMon)
        Me.GroupBox3.Controls.Add(Me.CBSymbAnrListe)
        Me.GroupBox3.Location = New System.Drawing.Point(3, 64)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(199, 205)
        Me.GroupBox3.TabIndex = 25
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Einstellungen für Symbolleisten"
        '
        'CBSymbJournalimport
        '
        Me.CBSymbJournalimport.AutoSize = True
        Me.CBSymbJournalimport.Location = New System.Drawing.Point(6, 180)
        Me.CBSymbJournalimport.Name = "CBSymbJournalimport"
        Me.CBSymbJournalimport.Size = New System.Drawing.Size(88, 17)
        Me.CBSymbJournalimport.TabIndex = 8
        Me.CBSymbJournalimport.Text = "Journalimport"
        Me.CBSymbJournalimport.UseVisualStyleBackColor = True
        '
        'CBSymbVIP
        '
        Me.CBSymbVIP.AutoSize = True
        Me.CBSymbVIP.Location = New System.Drawing.Point(6, 157)
        Me.CBSymbVIP.Name = "CBSymbVIP"
        Me.CBSymbVIP.Size = New System.Drawing.Size(43, 17)
        Me.CBSymbVIP.TabIndex = 7
        Me.CBSymbVIP.Text = "VIP"
        Me.CBSymbVIP.UseVisualStyleBackColor = True
        '
        'CBSymbRWSuche
        '
        Me.CBSymbRWSuche.AutoSize = True
        Me.CBSymbRWSuche.Location = New System.Drawing.Point(6, 134)
        Me.CBSymbRWSuche.Name = "CBSymbRWSuche"
        Me.CBSymbRWSuche.Size = New System.Drawing.Size(106, 17)
        Me.CBSymbRWSuche.TabIndex = 6
        Me.CBSymbRWSuche.Text = "Rückwärtssuche"
        Me.CBSymbRWSuche.UseVisualStyleBackColor = True
        '
        'CBSymbDirekt
        '
        Me.CBSymbDirekt.AutoSize = True
        Me.CBSymbDirekt.Location = New System.Drawing.Point(6, 111)
        Me.CBSymbDirekt.Name = "CBSymbDirekt"
        Me.CBSymbDirekt.Size = New System.Drawing.Size(76, 17)
        Me.CBSymbDirekt.TabIndex = 5
        Me.CBSymbDirekt.Text = "Direktwahl"
        Me.CBSymbDirekt.UseVisualStyleBackColor = True
        '
        'CBSymbAnrMonNeuStart
        '
        Me.CBSymbAnrMonNeuStart.AutoSize = True
        Me.CBSymbAnrMonNeuStart.Location = New System.Drawing.Point(6, 42)
        Me.CBSymbAnrMonNeuStart.Name = "CBSymbAnrMonNeuStart"
        Me.CBSymbAnrMonNeuStart.Size = New System.Drawing.Size(138, 17)
        Me.CBSymbAnrMonNeuStart.TabIndex = 2
        Me.CBSymbAnrMonNeuStart.Text = "Anrufmonitor neustarten"
        Me.CBSymbAnrMonNeuStart.UseVisualStyleBackColor = True
        '
        'CBSymbWwdh
        '
        Me.CBSymbWwdh.AutoSize = True
        Me.CBSymbWwdh.Location = New System.Drawing.Point(6, 65)
        Me.CBSymbWwdh.Name = "CBSymbWwdh"
        Me.CBSymbWwdh.Size = New System.Drawing.Size(118, 17)
        Me.CBSymbWwdh.TabIndex = 3
        Me.CBSymbWwdh.Text = "Wahlwiederhohung"
        Me.CBSymbWwdh.UseVisualStyleBackColor = True
        '
        'CBSymbAnrMon
        '
        Me.CBSymbAnrMon.AutoSize = True
        Me.CBSymbAnrMon.Location = New System.Drawing.Point(6, 19)
        Me.CBSymbAnrMon.Name = "CBSymbAnrMon"
        Me.CBSymbAnrMon.Size = New System.Drawing.Size(85, 17)
        Me.CBSymbAnrMon.TabIndex = 1
        Me.CBSymbAnrMon.Text = "Anrufmonitor"
        Me.CBSymbAnrMon.UseVisualStyleBackColor = True
        '
        'CBSymbAnrListe
        '
        Me.CBSymbAnrListe.AutoSize = True
        Me.CBSymbAnrListe.Location = New System.Drawing.Point(6, 88)
        Me.CBSymbAnrListe.Name = "CBSymbAnrListe"
        Me.CBSymbAnrListe.Size = New System.Drawing.Size(72, 17)
        Me.CBSymbAnrListe.TabIndex = 4
        Me.CBSymbAnrListe.Text = "Anrufliste "
        Me.CBSymbAnrListe.UseVisualStyleBackColor = True
        '
        'LabelSymb
        '
        Me.LabelSymb.Dock = System.Windows.Forms.DockStyle.Top
        Me.LabelSymb.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelSymb.Location = New System.Drawing.Point(0, 0)
        Me.LabelSymb.Name = "LabelSymb"
        Me.LabelSymb.Size = New System.Drawing.Size(570, 35)
        Me.LabelSymb.TabIndex = 24
        Me.LabelSymb.Text = "Einstellung für die Symbolleiste"
        Me.LabelSymb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PPhoner
        '
        Me.PPhoner.Controls.Add(Me.GroupBox4)
        Me.PPhoner.Controls.Add(Me.Label31)
        Me.PPhoner.Controls.Add(Me.Label30)
        Me.PPhoner.Controls.Add(Me.LinkPhoner)
        Me.PPhoner.Controls.Add(Me.Label27)
        Me.PPhoner.Controls.Add(Me.Label26)
        Me.PPhoner.Location = New System.Drawing.Point(4, 22)
        Me.PPhoner.Name = "PPhoner"
        Me.PPhoner.Size = New System.Drawing.Size(570, 294)
        Me.PPhoner.TabIndex = 12
        Me.PPhoner.Text = "Phoner"
        Me.PPhoner.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.PanelPhonerAktiv)
        Me.GroupBox4.Controls.Add(Me.LabelPhoner)
        Me.GroupBox4.Controls.Add(Me.PanelPhoner)
        Me.GroupBox4.Controls.Add(Me.ButtonPhoner)
        Me.GroupBox4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.GroupBox4.Location = New System.Drawing.Point(0, 141)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(570, 153)
        Me.GroupBox4.TabIndex = 27
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Phoner Einstellungen"
        '
        'PanelPhonerAktiv
        '
        Me.PanelPhonerAktiv.Location = New System.Drawing.Point(114, 19)
        Me.PanelPhonerAktiv.Name = "PanelPhonerAktiv"
        Me.PanelPhonerAktiv.Size = New System.Drawing.Size(13, 13)
        Me.PanelPhonerAktiv.TabIndex = 10
        '
        'LabelPhoner
        '
        Me.LabelPhoner.AutoSize = True
        Me.LabelPhoner.Location = New System.Drawing.Point(133, 19)
        Me.LabelPhoner.Name = "LabelPhoner"
        Me.LabelPhoner.Size = New System.Drawing.Size(83, 13)
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
        Me.PanelPhoner.Location = New System.Drawing.Point(111, 35)
        Me.PanelPhoner.Name = "PanelPhoner"
        Me.PanelPhoner.Size = New System.Drawing.Size(450, 107)
        Me.PanelPhoner.TabIndex = 8
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.CBPhonerAnrMon)
        Me.GroupBox7.Controls.Add(Me.Label29)
        Me.GroupBox7.Location = New System.Drawing.Point(211, 3)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(236, 91)
        Me.GroupBox7.TabIndex = 14
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "Phoner Anrufmonitor"
        '
        'CBPhonerAnrMon
        '
        Me.CBPhonerAnrMon.AutoSize = True
        Me.CBPhonerAnrMon.Location = New System.Drawing.Point(9, 69)
        Me.CBPhonerAnrMon.Name = "CBPhonerAnrMon"
        Me.CBPhonerAnrMon.Size = New System.Drawing.Size(178, 17)
        Me.CBPhonerAnrMon.TabIndex = 1
        Me.CBPhonerAnrMon.Text = "Phoner Anrufmonitor verwenden"
        Me.CBPhonerAnrMon.UseVisualStyleBackColor = True
        '
        'Label29
        '
        Me.Label29.Location = New System.Drawing.Point(6, 16)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(199, 51)
        Me.Label29.TabIndex = 12
        Me.Label29.Text = "Bei der Verwerndung des Phoner Anrufmonitors wird der Anrufmonitor der Fritz!Box " & _
    "nicht mehr überwacht!"
        '
        'CBPhoner
        '
        Me.CBPhoner.AutoSize = True
        Me.CBPhoner.Location = New System.Drawing.Point(7, 18)
        Me.CBPhoner.Name = "CBPhoner"
        Me.CBPhoner.Size = New System.Drawing.Size(168, 17)
        Me.CBPhoner.TabIndex = 3
        Me.CBPhoner.Text = "Softphone Phoner verwenden"
        Me.CBPhoner.UseVisualStyleBackColor = True
        '
        'ComboBoxPhonerSIP
        '
        Me.ComboBoxPhonerSIP.FormattingEnabled = True
        Me.ComboBoxPhonerSIP.Location = New System.Drawing.Point(7, 70)
        Me.ComboBoxPhonerSIP.Name = "ComboBoxPhonerSIP"
        Me.ComboBoxPhonerSIP.Size = New System.Drawing.Size(100, 21)
        Me.ComboBoxPhonerSIP.TabIndex = 2
        '
        'TBPhonerPasswort
        '
        Me.TBPhonerPasswort.Location = New System.Drawing.Point(7, 41)
        Me.TBPhonerPasswort.Name = "TBPhonerPasswort"
        Me.TBPhonerPasswort.Size = New System.Drawing.Size(100, 20)
        Me.TBPhonerPasswort.TabIndex = 7
        Me.TBPhonerPasswort.UseSystemPasswordChar = True
        '
        'LPassworPhoner
        '
        Me.LPassworPhoner.AutoSize = True
        Me.LPassworPhoner.Location = New System.Drawing.Point(113, 44)
        Me.LPassworPhoner.Name = "LPassworPhoner"
        Me.LPassworPhoner.Size = New System.Drawing.Size(87, 13)
        Me.LPassworPhoner.TabIndex = 6
        Me.LPassworPhoner.Text = "Phoner Passwort"
        '
        'ButtonPhoner
        '
        Me.ButtonPhoner.Location = New System.Drawing.Point(6, 32)
        Me.ButtonPhoner.Name = "ButtonPhoner"
        Me.ButtonPhoner.Size = New System.Drawing.Size(99, 110)
        Me.ButtonPhoner.TabIndex = 9
        Me.ButtonPhoner.Text = "Teste die Verfügbarkeit von Phoner"
        Me.ButtonPhoner.UseVisualStyleBackColor = True
        '
        'Label31
        '
        Me.Label31.Location = New System.Drawing.Point(4, 51)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(561, 55)
        Me.Label31.TabIndex = 26
        Me.Label31.Text = resources.GetString("Label31.Text")
        '
        'Label30
        '
        Me.Label30.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label30.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label30.Location = New System.Drawing.Point(0, 0)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(570, 35)
        Me.Label30.TabIndex = 25
        Me.Label30.Text = "Einstellungen für Phoner"
        Me.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LinkPhoner
        '
        Me.LinkPhoner.AutoSize = True
        Me.LinkPhoner.Location = New System.Drawing.Point(282, 106)
        Me.LinkPhoner.Name = "LinkPhoner"
        Me.LinkPhoner.Size = New System.Drawing.Size(41, 13)
        Me.LinkPhoner.TabIndex = 4
        Me.LinkPhoner.TabStop = True
        Me.LinkPhoner.Text = "Phoner"
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Location = New System.Drawing.Point(4, 122)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(330, 13)
        Me.Label27.TabIndex = 11
        Me.Label27.Text = "Phoner Copyright 2013 Heiko Sommerfeldt. Alle Rechte vorbehalten."
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Location = New System.Drawing.Point(4, 106)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(250, 13)
        Me.Label26.TabIndex = 10
        Me.Label26.Text = "Phoner kann über folgenden Link bezogen werden:"
        '
        'PLogging
        '
        Me.PLogging.Controls.Add(Me.GBLogging)
        Me.PLogging.Controls.Add(Me.CBLogFile)
        Me.PLogging.Controls.Add(Me.Label23)
        Me.PLogging.Location = New System.Drawing.Point(4, 22)
        Me.PLogging.Name = "PLogging"
        Me.PLogging.Padding = New System.Windows.Forms.Padding(3)
        Me.PLogging.Size = New System.Drawing.Size(570, 294)
        Me.PLogging.TabIndex = 11
        Me.PLogging.Text = "Logging"
        Me.PLogging.UseVisualStyleBackColor = True
        '
        'GBLogging
        '
        Me.GBLogging.Controls.Add(Me.BLogging)
        Me.GBLogging.Controls.Add(Me.LinkLogFile)
        Me.GBLogging.Controls.Add(Me.TBLogging)
        Me.GBLogging.Location = New System.Drawing.Point(0, 76)
        Me.GBLogging.Name = "GBLogging"
        Me.GBLogging.Size = New System.Drawing.Size(570, 218)
        Me.GBLogging.TabIndex = 26
        Me.GBLogging.TabStop = False
        '
        'BLogging
        '
        Me.BLogging.Location = New System.Drawing.Point(383, 184)
        Me.BLogging.Name = "BLogging"
        Me.BLogging.Size = New System.Drawing.Size(184, 28)
        Me.BLogging.TabIndex = 27
        Me.BLogging.Text = "In Zischenablage zu kopieren"
        Me.BLogging.UseVisualStyleBackColor = True
        '
        'LinkLogFile
        '
        Me.LinkLogFile.Location = New System.Drawing.Point(2, 168)
        Me.LinkLogFile.MaximumSize = New System.Drawing.Size(565, 15)
        Me.LinkLogFile.Name = "LinkLogFile"
        Me.LinkLogFile.Size = New System.Drawing.Size(565, 15)
        Me.LinkLogFile.TabIndex = 26
        Me.LinkLogFile.TabStop = True
        Me.LinkLogFile.Text = "Link zur Logfile"
        '
        'TBLogging
        '
        Me.TBLogging.Dock = System.Windows.Forms.DockStyle.Top
        Me.TBLogging.Location = New System.Drawing.Point(3, 16)
        Me.TBLogging.Multiline = True
        Me.TBLogging.Name = "TBLogging"
        Me.TBLogging.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TBLogging.Size = New System.Drawing.Size(564, 147)
        Me.TBLogging.TabIndex = 25
        '
        'CBLogFile
        '
        Me.CBLogFile.AutoSize = True
        Me.CBLogFile.Location = New System.Drawing.Point(6, 50)
        Me.CBLogFile.Name = "CBLogFile"
        Me.CBLogFile.Size = New System.Drawing.Size(113, 17)
        Me.CBLogFile.TabIndex = 12
        Me.CBLogFile.Text = "Logging aktivieren"
        Me.CBLogFile.UseVisualStyleBackColor = True
        '
        'Label23
        '
        Me.Label23.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label23.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.Location = New System.Drawing.Point(3, 3)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(564, 35)
        Me.Label23.TabIndex = 24
        Me.Label23.Text = "Logging"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'PDebug
        '
        Me.PDebug.Controls.Add(Me.GroupBox2)
        Me.PDebug.Controls.Add(Me.Label8)
        Me.PDebug.Location = New System.Drawing.Point(4, 22)
        Me.PDebug.Name = "PDebug"
        Me.PDebug.Padding = New System.Windows.Forms.Padding(3)
        Me.PDebug.Size = New System.Drawing.Size(570, 294)
        Me.PDebug.TabIndex = 10
        Me.PDebug.Text = "Debug"
        Me.PDebug.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.PTelefonDatei)
        Me.GroupBox2.Controls.Add(Me.CBTelefonDatei)
        Me.GroupBox2.Controls.Add(Me.BStartDebug)
        Me.GroupBox2.Controls.Add(Me.BZwischenablage)
        Me.GroupBox2.Controls.Add(Me.BProbleme)
        Me.GroupBox2.Controls.Add(Me.TBDiagnose)
        Me.GroupBox2.Location = New System.Drawing.Point(0, 41)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(570, 253)
        Me.GroupBox2.TabIndex = 30
        Me.GroupBox2.TabStop = False
        '
        'PTelefonDatei
        '
        Me.PTelefonDatei.Controls.Add(Me.Label9)
        Me.PTelefonDatei.Controls.Add(Me.TBTelefonDatei)
        Me.PTelefonDatei.Controls.Add(Me.BTelefonDatei)
        Me.PTelefonDatei.Enabled = False
        Me.PTelefonDatei.Location = New System.Drawing.Point(384, 160)
        Me.PTelefonDatei.Name = "PTelefonDatei"
        Me.PTelefonDatei.Size = New System.Drawing.Size(186, 87)
        Me.PTelefonDatei.TabIndex = 35
        '
        'Label9
        '
        Me.Label9.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(3, 7)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(164, 13)
        Me.Label9.TabIndex = 26
        Me.Label9.Text = "Wählen Sie die Telefondatei aus:"
        '
        'TBTelefonDatei
        '
        Me.TBTelefonDatei.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TBTelefonDatei.Location = New System.Drawing.Point(4, 30)
        Me.TBTelefonDatei.Name = "TBTelefonDatei"
        Me.TBTelefonDatei.Size = New System.Drawing.Size(179, 20)
        Me.TBTelefonDatei.TabIndex = 5
        '
        'BTelefonDatei
        '
        Me.BTelefonDatei.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BTelefonDatei.Location = New System.Drawing.Point(4, 56)
        Me.BTelefonDatei.Name = "BTelefonDatei"
        Me.BTelefonDatei.Size = New System.Drawing.Size(179, 28)
        Me.BTelefonDatei.TabIndex = 6
        Me.BTelefonDatei.Text = "Laden"
        Me.BTelefonDatei.UseVisualStyleBackColor = True
        '
        'CBTelefonDatei
        '
        Me.CBTelefonDatei.AutoSize = True
        Me.CBTelefonDatei.Location = New System.Drawing.Point(385, 137)
        Me.CBTelefonDatei.Name = "CBTelefonDatei"
        Me.CBTelefonDatei.Size = New System.Drawing.Size(127, 17)
        Me.CBTelefonDatei.TabIndex = 34
        Me.CBTelefonDatei.Text = "Telefondatei einlesen"
        Me.CBTelefonDatei.UseVisualStyleBackColor = True
        '
        'TBDiagnose
        '
        Me.TBDiagnose.BackColor = System.Drawing.SystemColors.Window
        Me.TBDiagnose.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TBDiagnose.Location = New System.Drawing.Point(2, 13)
        Me.TBDiagnose.Multiline = True
        Me.TBDiagnose.Name = "TBDiagnose"
        Me.TBDiagnose.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TBDiagnose.Size = New System.Drawing.Size(376, 220)
        Me.TBDiagnose.TabIndex = 31
        Me.TBDiagnose.TabStop = False
        '
        'Label8
        '
        Me.Label8.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(3, 3)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(564, 35)
        Me.Label8.TabIndex = 24
        Me.Label8.Text = "Einlesen der eingerichteten Telefone"
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ButtonXML
        '
        Me.ButtonXML.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonXML.Location = New System.Drawing.Point(463, 3)
        Me.ButtonXML.Name = "ButtonXML"
        Me.ButtonXML.Size = New System.Drawing.Size(112, 28)
        Me.ButtonXML.TabIndex = 27
        Me.ButtonXML.Text = "Einstellungsdatei"
        Me.ButtonXML.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 5
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonOK, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonXML, 4, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonUebernehmen, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonAbbruch, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonZuruecksetzen, 3, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 329)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(578, 34)
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
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 2
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(584, 366)
        Me.TableLayoutPanel2.TabIndex = 29
        '
        'ButtonArbeitsverzeichnis
        '
        Me.ButtonArbeitsverzeichnis.Location = New System.Drawing.Point(410, 58)
        Me.ButtonArbeitsverzeichnis.Name = "ButtonArbeitsverzeichnis"
        Me.ButtonArbeitsverzeichnis.Size = New System.Drawing.Size(155, 28)
        Me.ButtonArbeitsverzeichnis.TabIndex = 6
        Me.ButtonArbeitsverzeichnis.Text = "Arbeitsverzeichnis ändern"
        Me.ButtonArbeitsverzeichnis.UseVisualStyleBackColor = True
        '
        'formCfg
        '
        Me.AcceptButton = Me.ButtonOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonAbbruch
        Me.ClientSize = New System.Drawing.Size(584, 366)
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
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
        Me.PInfo.ResumeLayout(False)
        Me.PInfo.PerformLayout()
        Me.PTelefone.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.TelList, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PIndex.ResumeLayout(False)
        Me.GroupBoxIndex.ResumeLayout(False)
        Me.GroupBoxIndex.PerformLayout()
        Me.GroupBoxRWS.ResumeLayout(False)
        Me.GroupBoxRWS.PerformLayout()
        Me.PAnrufmonitor.ResumeLayout(False)
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        CType(Me.TBAnrMonMoveGeschwindigkeit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Frame1.ResumeLayout(False)
        Me.Frame1.PerformLayout()
        Me.PanelAnrMon.ResumeLayout(False)
        Me.PanelAnrMon.PerformLayout()
        Me.FBDB_MP.ResumeLayout(False)
        Me.PSymbolleiste.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
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
        Me.PTelefonDatei.ResumeLayout(False)
        Me.PTelefonDatei.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ButtonZuruecksetzen As System.Windows.Forms.Button
    Friend WithEvents ButtonAbbruch As System.Windows.Forms.Button
    Friend WithEvents ButtonUebernehmen As System.Windows.Forms.Button
    Friend WithEvents ButtonOK As System.Windows.Forms.Button
    Friend WithEvents ToolTipFBDBConfig As System.Windows.Forms.ToolTip
    Friend WithEvents PInfo As System.Windows.Forms.TabPage
    Friend WithEvents LinkEmail As System.Windows.Forms.LinkLabel
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents LVersion As System.Windows.Forms.Label
    Friend WithEvents RichTextBox1 As System.Windows.Forms.RichTextBox
    Friend WithEvents PTelefone As System.Windows.Forms.TabPage
    Friend WithEvents PIndex As System.Windows.Forms.TabPage
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
    Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
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
    Friend WithEvents Frame1 As System.Windows.Forms.GroupBox
    Friend WithEvents PanelAnrMon As System.Windows.Forms.Panel
    Friend WithEvents CBAnrMonAuto As System.Windows.Forms.CheckBox
    Friend WithEvents CBJImport As System.Windows.Forms.CheckBox
    Friend WithEvents CBJournal As System.Windows.Forms.CheckBox
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
    Friend WithEvents TBLandesVW As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TBVorwahl As System.Windows.Forms.TextBox
    Friend WithEvents lblTBPasswort As System.Windows.Forms.Label
    Friend WithEvents TBFBAdr As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents FBDB_MP As System.Windows.Forms.TabControl
    Friend WithEvents PSymbolleiste As System.Windows.Forms.TabPage
    Friend WithEvents LabelSymb As System.Windows.Forms.Label
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents CBSymbRWSuche As System.Windows.Forms.CheckBox
    Friend WithEvents CBSymbDirekt As System.Windows.Forms.CheckBox
    Friend WithEvents CBSymbAnrMonNeuStart As System.Windows.Forms.CheckBox
    Friend WithEvents CBSymbWwdh As System.Windows.Forms.CheckBox
    Friend WithEvents CBSymbAnrMon As System.Windows.Forms.CheckBox
    Friend WithEvents CBSymbAnrListe As System.Windows.Forms.CheckBox
    Friend WithEvents CBSymbVIP As System.Windows.Forms.CheckBox
    Friend WithEvents CBSymbJournalimport As System.Windows.Forms.CheckBox
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
    Friend WithEvents ButtonPhoner As System.Windows.Forms.Button
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents GroupBox7 As System.Windows.Forms.GroupBox
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents PanelPhonerAktiv As System.Windows.Forms.Panel
    Friend WithEvents ButtonXML As System.Windows.Forms.Button
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
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents PTelefonDatei As System.Windows.Forms.Panel
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents TBTelefonDatei As System.Windows.Forms.TextBox
    Friend WithEvents BTelefonDatei As System.Windows.Forms.Button
    Friend WithEvents CBTelefonDatei As System.Windows.Forms.CheckBox
    Friend WithEvents BStartDebug As System.Windows.Forms.Button
    Friend WithEvents BZwischenablage As System.Windows.Forms.Button
    Friend WithEvents BProbleme As System.Windows.Forms.Button
    Friend WithEvents TBDiagnose As System.Windows.Forms.TextBox
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
    Friend WithEvents ButtonArbeitsverzeichnis As System.Windows.Forms.Button
#If OVer < 14 Then
#End If
End Class
