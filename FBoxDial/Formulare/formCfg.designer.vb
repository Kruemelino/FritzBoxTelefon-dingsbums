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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.BReset = New System.Windows.Forms.Button()
        Me.BAbbruch = New System.Windows.Forms.Button()
        Me.BApply = New System.Windows.Forms.Button()
        Me.BOK = New System.Windows.Forms.Button()
        Me.ToolTipFBDBConfig = New System.Windows.Forms.ToolTip(Me.components)
        Me.LPhonerSIPTelefon = New System.Windows.Forms.Label()
        Me.CBKErstellen = New System.Windows.Forms.CheckBox()
        Me.CBRWSIndex = New System.Windows.Forms.CheckBox()
        Me.TBRWSTest = New System.Windows.Forms.TextBox()
        Me.CBShowMSN = New System.Windows.Forms.CheckBox()
        Me.CBAutoAnrList = New System.Windows.Forms.CheckBox()
        Me.LNumEntryList = New System.Windows.Forms.Label()
        Me.TBNumEntryList = New System.Windows.Forms.TextBox()
        Me.CBForceFBAdr = New System.Windows.Forms.CheckBox()
        Me.LBenutzer = New System.Windows.Forms.Label()
        Me.BTestLogin = New System.Windows.Forms.Button()
        Me.TBAmt = New System.Windows.Forms.TextBox()
        Me.LAmtsholung = New System.Windows.Forms.Label()
        Me.CBCbCunterbinden = New System.Windows.Forms.CheckBox()
        Me.CBForceDialLKZ = New System.Windows.Forms.CheckBox()
        Me.CBCheckMobil = New System.Windows.Forms.CheckBox()
        Me.CBTelNrGruppieren = New System.Windows.Forms.CheckBox()
        Me.TBTelNrMaske = New System.Windows.Forms.TextBox()
        Me.LTelNrMaske = New System.Windows.Forms.Label()
        Me.BXML = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.FBDB_MP = New System.Windows.Forms.TabControl()
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
        Me.PAnrufmonitor = New System.Windows.Forms.TabPage()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
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
        Me.PKontaktsuche = New System.Windows.Forms.TabPage()
        Me.TBHinweisKontaktsuche = New System.Windows.Forms.TextBox()
        Me.GBKontaktsuche = New System.Windows.Forms.GroupBox()
        Me.CBSucheUnterordner = New System.Windows.Forms.CheckBox()
        Me.CBKontaktSucheFritzBox = New System.Windows.Forms.CheckBox()
        Me.GBRWS = New System.Windows.Forms.GroupBox()
        Me.BRWSTest = New System.Windows.Forms.Button()
        Me.CBRWS = New System.Windows.Forms.CheckBox()
        Me.BKontaktOrdnerSuche = New System.Windows.Forms.Button()
        Me.GBIndizierung = New System.Windows.Forms.GroupBox()
        Me.LabelAnzahl = New System.Windows.Forms.Label()
        Me.RadioButtonEntfernen = New System.Windows.Forms.RadioButton()
        Me.RadioButtonErstelle = New System.Windows.Forms.RadioButton()
        Me.BIndizierungAbbrechen = New System.Windows.Forms.Button()
        Me.BIndizierungStart = New System.Windows.Forms.Button()
        Me.ProgressBarIndex = New System.Windows.Forms.ProgressBar()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TabJournal = New System.Windows.Forms.TabPage()
        Me.TBHinweisJournal = New System.Windows.Forms.TextBox()
        Me.BJournalOrdnerErstellen = New System.Windows.Forms.Button()
        Me.LHeaderTabJournal = New System.Windows.Forms.Label()
        Me.GBJournal = New System.Windows.Forms.GroupBox()
        Me.CBJournal = New System.Windows.Forms.CheckBox()
        Me.PKontakterstellung = New System.Windows.Forms.TabPage()
        Me.GBKontakterstellung = New System.Windows.Forms.GroupBox()
        Me.BKontaktOrdnerErstellen = New System.Windows.Forms.Button()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.TBHinweisKontakterstellung = New System.Windows.Forms.TextBox()
        Me.PTelefone = New System.Windows.Forms.TabPage()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.StatusStripTelefone = New System.Windows.Forms.StatusStrip()
        Me.TSSL_Telefone = New System.Windows.Forms.ToolStripStatusLabel()
        Me.BTelefonliste = New System.Windows.Forms.Button()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.PPhoner = New System.Windows.Forms.TabPage()
        Me.TBPhonerHinweise = New System.Windows.Forms.TextBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.BPhonerTest = New System.Windows.Forms.Button()
        Me.CBPhoner = New System.Windows.Forms.CheckBox()
        Me.CBoxPhonerSIP = New System.Windows.Forms.ComboBox()
        Me.TBPhonerPasswort = New System.Windows.Forms.MaskedTextBox()
        Me.LPassworPhoner = New System.Windows.Forms.Label()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.LinkPhoner = New System.Windows.Forms.LinkLabel()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.PLogging = New System.Windows.Forms.TabPage()
        Me.LMinLogLevel = New System.Windows.Forms.Label()
        Me.CBoxMinLogLevel = New System.Windows.Forms.ComboBox()
        Me.GBLogging = New System.Windows.Forms.GroupBox()
        Me.LinkLogFile = New System.Windows.Forms.LinkLabel()
        Me.TBLogging = New System.Windows.Forms.TextBox()
        Me.Label23 = New System.Windows.Forms.Label()
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
        Me.TreeViewKontakteSuche = New FBoxDial.OlOrdnerTreeView()
        Me.TreeViewJournal = New FBoxDial.OlOrdnerTreeView()
        Me.TreeViewKontakteErstellen = New FBoxDial.OlOrdnerTreeView()
        Me.DGVTelList = New FBoxDial.FBoxDataGridView()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.FBDB_MP.SuspendLayout()
        Me.PGrundeinstellungen.SuspendLayout()
        Me.GBFormatierungTelefonnummern.SuspendLayout()
        Me.GBEinstellungWählhilfe.SuspendLayout()
        Me.GBErforderlicheAngaben.SuspendLayout()
        Me.PAnrufmonitor.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.GBAnrListeMain.SuspendLayout()
        Me.GBAnrMonMain.SuspendLayout()
        Me.PanelAnrMon.SuspendLayout()
        Me.PKontaktsuche.SuspendLayout()
        Me.GBKontaktsuche.SuspendLayout()
        Me.GBRWS.SuspendLayout()
        Me.GBIndizierung.SuspendLayout()
        Me.TabJournal.SuspendLayout()
        Me.GBJournal.SuspendLayout()
        Me.PKontakterstellung.SuspendLayout()
        Me.GBKontakterstellung.SuspendLayout()
        Me.PTelefone.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.StatusStripTelefone.SuspendLayout()
        Me.PPhoner.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.PLogging.SuspendLayout()
        Me.GBLogging.SuspendLayout()
        Me.PInfo.SuspendLayout()
        CType(Me.DGVTelList, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BReset
        '
        Me.BReset.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BReset.Location = New System.Drawing.Point(523, 5)
        Me.BReset.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BReset.Name = "BReset"
        Me.BReset.Size = New System.Drawing.Size(165, 42)
        Me.BReset.TabIndex = 3
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
        Me.BAbbruch.Size = New System.Drawing.Size(165, 42)
        Me.BAbbruch.TabIndex = 2
        Me.BAbbruch.Text = "Abbruch"
        Me.BAbbruch.UseVisualStyleBackColor = True
        '
        'BApply
        '
        Me.BApply.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BApply.Location = New System.Drawing.Point(177, 5)
        Me.BApply.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BApply.Name = "BApply"
        Me.BApply.Size = New System.Drawing.Size(165, 42)
        Me.BApply.TabIndex = 1
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
        Me.BOK.Size = New System.Drawing.Size(165, 42)
        Me.BOK.TabIndex = 0
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
        'LPhonerSIPTelefon
        '
        Me.LPhonerSIPTelefon.AutoSize = True
        Me.LPhonerSIPTelefon.Enabled = False
        Me.LPhonerSIPTelefon.Location = New System.Drawing.Point(166, 108)
        Me.LPhonerSIPTelefon.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LPhonerSIPTelefon.Name = "LPhonerSIPTelefon"
        Me.LPhonerSIPTelefon.Size = New System.Drawing.Size(93, 20)
        Me.LPhonerSIPTelefon.TabIndex = 13
        Me.LPhonerSIPTelefon.Text = "SIP-Telefon"
        Me.ToolTipFBDBConfig.SetToolTip(Me.LPhonerSIPTelefon, "Geben Sie hier das SIP-Telefon, an welches mit Phoner verknüpft ist.")
        '
        'CBKErstellen
        '
        Me.CBKErstellen.AutoSize = True
        Me.CBKErstellen.Location = New System.Drawing.Point(9, 29)
        Me.CBKErstellen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBKErstellen.Name = "CBKErstellen"
        Me.CBKErstellen.Size = New System.Drawing.Size(154, 24)
        Me.CBKErstellen.TabIndex = 1
        Me.CBKErstellen.Text = "Kontakt erstellen"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBKErstellen, "Nach erfolgreicher Rückwärtssuche, wird bei dieser Einstellung ein neuer Kontakt " &
        "erstellt.")
        Me.CBKErstellen.UseVisualStyleBackColor = True
        '
        'CBRWSIndex
        '
        Me.CBRWSIndex.AutoSize = True
        Me.CBRWSIndex.Enabled = False
        Me.CBRWSIndex.Location = New System.Drawing.Point(7, 62)
        Me.CBRWSIndex.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBRWSIndex.Name = "CBRWSIndex"
        Me.CBRWSIndex.Size = New System.Drawing.Size(155, 24)
        Me.CBRWSIndex.TabIndex = 2
        Me.CBRWSIndex.Text = "Ergebnis merken"
        Me.ToolTipFBDBConfig.SetToolTip(Me.CBRWSIndex, resources.GetString("CBRWSIndex.ToolTip"))
        Me.CBRWSIndex.UseVisualStyleBackColor = True
        '
        'TBRWSTest
        '
        Me.TBRWSTest.Location = New System.Drawing.Point(246, 26)
        Me.TBRWSTest.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBRWSTest.Name = "TBRWSTest"
        Me.TBRWSTest.Size = New System.Drawing.Size(167, 26)
        Me.TBRWSTest.TabIndex = 1
        Me.ToolTipFBDBConfig.SetToolTip(Me.TBRWSTest, "Geben Sie hier eine gültige Telefonnummer ein, nach der eine Rückwärtssuche durch" &
        "geführt werden soll.")
        '
        'CBShowMSN
        '
        Me.CBShowMSN.AutoSize = True
        Me.CBShowMSN.Enabled = False
        Me.CBShowMSN.Location = New System.Drawing.Point(4, 302)
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
        'TBAmt
        '
        Me.TBAmt.Location = New System.Drawing.Point(9, 72)
        Me.TBAmt.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBAmt.Name = "TBAmt"
        Me.TBAmt.Size = New System.Drawing.Size(43, 26)
        Me.TBAmt.TabIndex = 7
        Me.ToolTipFBDBConfig.SetToolTip(Me.TBAmt, "Geben Sie hier eine 0 ein falls eine Amtsholung benötigt wird.")
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
        'CBCheckMobil
        '
        Me.CBCheckMobil.AutoSize = True
        Me.CBCheckMobil.Location = New System.Drawing.Point(9, 188)
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
        'TBTelNrMaske
        '
        Me.TBTelNrMaske.Location = New System.Drawing.Point(207, 26)
        Me.TBTelNrMaske.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBTelNrMaske.Name = "TBTelNrMaske"
        Me.TBTelNrMaske.Size = New System.Drawing.Size(146, 26)
        Me.TBTelNrMaske.TabIndex = 13
        Me.ToolTipFBDBConfig.SetToolTip(Me.TBTelNrMaske, resources.GetString("TBTelNrMaske.ToolTip"))
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
        'BXML
        '
        Me.BXML.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BXML.Location = New System.Drawing.Point(696, 5)
        Me.BXML.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BXML.Name = "BXML"
        Me.BXML.Size = New System.Drawing.Size(168, 42)
        Me.BXML.TabIndex = 4
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
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(4, 505)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(868, 52)
        Me.TableLayoutPanel1.TabIndex = 0
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
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(876, 562)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'FBDB_MP
        '
        Me.FBDB_MP.Controls.Add(Me.PGrundeinstellungen)
        Me.FBDB_MP.Controls.Add(Me.PAnrufmonitor)
        Me.FBDB_MP.Controls.Add(Me.PKontaktsuche)
        Me.FBDB_MP.Controls.Add(Me.TabJournal)
        Me.FBDB_MP.Controls.Add(Me.PKontakterstellung)
        Me.FBDB_MP.Controls.Add(Me.PTelefone)
        Me.FBDB_MP.Controls.Add(Me.PPhoner)
        Me.FBDB_MP.Controls.Add(Me.PLogging)
        Me.FBDB_MP.Controls.Add(Me.PInfo)
        Me.FBDB_MP.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FBDB_MP.Location = New System.Drawing.Point(4, 5)
        Me.FBDB_MP.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.FBDB_MP.Name = "FBDB_MP"
        Me.FBDB_MP.SelectedIndex = 0
        Me.FBDB_MP.Size = New System.Drawing.Size(868, 490)
        Me.FBDB_MP.TabIndex = 1
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
        Me.PGrundeinstellungen.Size = New System.Drawing.Size(860, 457)
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
        Me.GBFormatierungTelefonnummern.Location = New System.Drawing.Point(0, 298)
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
        Me.CBCloseWClient.Location = New System.Drawing.Point(9, 222)
        Me.CBCloseWClient.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
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
        'PAnrufmonitor
        '
        Me.PAnrufmonitor.Controls.Add(Me.GroupBox6)
        Me.PAnrufmonitor.Controls.Add(Me.GBAnrListeMain)
        Me.PAnrufmonitor.Controls.Add(Me.Label22)
        Me.PAnrufmonitor.Controls.Add(Me.GBAnrMonMain)
        Me.PAnrufmonitor.Location = New System.Drawing.Point(4, 29)
        Me.PAnrufmonitor.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PAnrufmonitor.Name = "PAnrufmonitor"
        Me.PAnrufmonitor.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PAnrufmonitor.Size = New System.Drawing.Size(860, 457)
        Me.PAnrufmonitor.TabIndex = 0
        Me.PAnrufmonitor.Text = "Anrufmonitor"
        Me.PAnrufmonitor.UseVisualStyleBackColor = True
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.TBNumEntryList)
        Me.GroupBox6.Controls.Add(Me.LNumEntryList)
        Me.GroupBox6.Location = New System.Drawing.Point(432, 298)
        Me.GroupBox6.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox6.Size = New System.Drawing.Size(424, 85)
        Me.GroupBox6.TabIndex = 38
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Anruflisten"
        '
        'GBAnrListeMain
        '
        Me.GBAnrListeMain.Controls.Add(Me.CBAnrListeShowAnrMon)
        Me.GBAnrListeMain.Controls.Add(Me.CBAutoAnrList)
        Me.GBAnrListeMain.Controls.Add(Me.CBAnrListeUpdateCallLists)
        Me.GBAnrListeMain.Location = New System.Drawing.Point(432, 152)
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
        Me.CBAnrListeShowAnrMon.Location = New System.Drawing.Point(4, 98)
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
        Me.CBAnrListeUpdateCallLists.Location = New System.Drawing.Point(4, 62)
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
        Me.TBEnblDauer.Location = New System.Drawing.Point(3, 78)
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
        'PKontaktsuche
        '
        Me.PKontaktsuche.Controls.Add(Me.TBHinweisKontaktsuche)
        Me.PKontaktsuche.Controls.Add(Me.GBKontaktsuche)
        Me.PKontaktsuche.Controls.Add(Me.GBRWS)
        Me.PKontaktsuche.Controls.Add(Me.BKontaktOrdnerSuche)
        Me.PKontaktsuche.Controls.Add(Me.GBIndizierung)
        Me.PKontaktsuche.Controls.Add(Me.Label1)
        Me.PKontaktsuche.Controls.Add(Me.TreeViewKontakteSuche)
        Me.PKontaktsuche.Location = New System.Drawing.Point(4, 29)
        Me.PKontaktsuche.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PKontaktsuche.Name = "PKontaktsuche"
        Me.PKontaktsuche.Padding = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.PKontaktsuche.Size = New System.Drawing.Size(860, 457)
        Me.PKontaktsuche.TabIndex = 15
        Me.PKontaktsuche.Text = "Kontaktsuche"
        Me.PKontaktsuche.UseVisualStyleBackColor = True
        '
        'TBHinweisKontaktsuche
        '
        Me.TBHinweisKontaktsuche.Location = New System.Drawing.Point(430, 66)
        Me.TBHinweisKontaktsuche.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.TBHinweisKontaktsuche.Multiline = True
        Me.TBHinweisKontaktsuche.Name = "TBHinweisKontaktsuche"
        Me.TBHinweisKontaktsuche.ReadOnly = True
        Me.TBHinweisKontaktsuche.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TBHinweisKontaktsuche.Size = New System.Drawing.Size(424, 89)
        Me.TBHinweisKontaktsuche.TabIndex = 4
        Me.TBHinweisKontaktsuche.TabStop = False
        Me.TBHinweisKontaktsuche.Text = resources.GetString("TBHinweisKontaktsuche.Text")
        '
        'GBKontaktsuche
        '
        Me.GBKontaktsuche.Controls.Add(Me.CBSucheUnterordner)
        Me.GBKontaktsuche.Controls.Add(Me.CBKontaktSucheFritzBox)
        Me.GBKontaktsuche.Location = New System.Drawing.Point(0, 58)
        Me.GBKontaktsuche.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.GBKontaktsuche.Name = "GBKontaktsuche"
        Me.GBKontaktsuche.Padding = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.GBKontaktsuche.Size = New System.Drawing.Size(423, 99)
        Me.GBKontaktsuche.TabIndex = 1
        Me.GBKontaktsuche.TabStop = False
        Me.GBKontaktsuche.Text = "Suche von Kontakten"
        '
        'CBSucheUnterordner
        '
        Me.CBSucheUnterordner.AutoSize = True
        Me.CBSucheUnterordner.Location = New System.Drawing.Point(9, 29)
        Me.CBSucheUnterordner.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.CBSucheUnterordner.Name = "CBSucheUnterordner"
        Me.CBSucheUnterordner.Size = New System.Drawing.Size(211, 24)
        Me.CBSucheUnterordner.TabIndex = 0
        Me.CBSucheUnterordner.Text = "Unterordner einbeziehen"
        Me.CBSucheUnterordner.TextAlign = System.Drawing.ContentAlignment.TopLeft
        Me.CBSucheUnterordner.UseVisualStyleBackColor = True
        '
        'CBKontaktSucheFritzBox
        '
        Me.CBKontaktSucheFritzBox.AutoSize = True
        Me.CBKontaktSucheFritzBox.Location = New System.Drawing.Point(9, 62)
        Me.CBKontaktSucheFritzBox.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBKontaktSucheFritzBox.MinimumSize = New System.Drawing.Size(350, 0)
        Me.CBKontaktSucheFritzBox.Name = "CBKontaktSucheFritzBox"
        Me.CBKontaktSucheFritzBox.Size = New System.Drawing.Size(350, 24)
        Me.CBKontaktSucheFritzBox.TabIndex = 1
        Me.CBKontaktSucheFritzBox.Text = "Die Fritz!Box Telefonbüchern durchsuchen"
        Me.CBKontaktSucheFritzBox.UseVisualStyleBackColor = True
        '
        'GBRWS
        '
        Me.GBRWS.Controls.Add(Me.BRWSTest)
        Me.GBRWS.Controls.Add(Me.TBRWSTest)
        Me.GBRWS.Controls.Add(Me.CBRWSIndex)
        Me.GBRWS.Controls.Add(Me.CBRWS)
        Me.GBRWS.Location = New System.Drawing.Point(0, 339)
        Me.GBRWS.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBRWS.Name = "GBRWS"
        Me.GBRWS.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBRWS.Size = New System.Drawing.Size(423, 116)
        Me.GBRWS.TabIndex = 3
        Me.GBRWS.TabStop = False
        Me.GBRWS.Text = "Rückwärtssuche (RWS)"
        '
        'BRWSTest
        '
        Me.BRWSTest.Location = New System.Drawing.Point(246, 62)
        Me.BRWSTest.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BRWSTest.Name = "BRWSTest"
        Me.BRWSTest.Size = New System.Drawing.Size(168, 42)
        Me.BRWSTest.TabIndex = 3
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
        'BKontaktOrdnerSuche
        '
        Me.BKontaktOrdnerSuche.Location = New System.Drawing.Point(430, 174)
        Me.BKontaktOrdnerSuche.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.BKontaktOrdnerSuche.Name = "BKontaktOrdnerSuche"
        Me.BKontaktOrdnerSuche.Size = New System.Drawing.Size(426, 42)
        Me.BKontaktOrdnerSuche.TabIndex = 1
        Me.BKontaktOrdnerSuche.Text = "Outlook-Kontaktordner laden..."
        Me.BKontaktOrdnerSuche.UseVisualStyleBackColor = True
        '
        'GBIndizierung
        '
        Me.GBIndizierung.Controls.Add(Me.LabelAnzahl)
        Me.GBIndizierung.Controls.Add(Me.RadioButtonEntfernen)
        Me.GBIndizierung.Controls.Add(Me.RadioButtonErstelle)
        Me.GBIndizierung.Controls.Add(Me.BIndizierungAbbrechen)
        Me.GBIndizierung.Controls.Add(Me.BIndizierungStart)
        Me.GBIndizierung.Controls.Add(Me.ProgressBarIndex)
        Me.GBIndizierung.Location = New System.Drawing.Point(0, 165)
        Me.GBIndizierung.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBIndizierung.Name = "GBIndizierung"
        Me.GBIndizierung.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBIndizierung.Size = New System.Drawing.Size(423, 164)
        Me.GBIndizierung.TabIndex = 2
        Me.GBIndizierung.TabStop = False
        Me.GBIndizierung.Text = "Kontaktindizierung"
        '
        'LabelAnzahl
        '
        Me.LabelAnzahl.AutoSize = True
        Me.LabelAnzahl.Location = New System.Drawing.Point(6, 130)
        Me.LabelAnzahl.Name = "LabelAnzahl"
        Me.LabelAnzahl.Size = New System.Drawing.Size(165, 20)
        Me.LabelAnzahl.TabIndex = 5
        Me.LabelAnzahl.Text = "Status der Indizierung"
        '
        'RadioButtonEntfernen
        '
        Me.RadioButtonEntfernen.AutoSize = True
        Me.RadioButtonEntfernen.Location = New System.Drawing.Point(133, 38)
        Me.RadioButtonEntfernen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.RadioButtonEntfernen.MinimumSize = New System.Drawing.Size(110, 0)
        Me.RadioButtonEntfernen.Name = "RadioButtonEntfernen"
        Me.RadioButtonEntfernen.Size = New System.Drawing.Size(110, 24)
        Me.RadioButtonEntfernen.TabIndex = 1
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
        Me.RadioButtonErstelle.TabIndex = 0
        Me.RadioButtonErstelle.TabStop = True
        Me.RadioButtonErstelle.Text = "erstellen"
        Me.RadioButtonErstelle.UseVisualStyleBackColor = True
        '
        'BIndizierungAbbrechen
        '
        Me.BIndizierungAbbrechen.Enabled = False
        Me.BIndizierungAbbrechen.Location = New System.Drawing.Point(246, 29)
        Me.BIndizierungAbbrechen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BIndizierungAbbrechen.Name = "BIndizierungAbbrechen"
        Me.BIndizierungAbbrechen.Size = New System.Drawing.Size(168, 42)
        Me.BIndizierungAbbrechen.TabIndex = 2
        Me.BIndizierungAbbrechen.Text = "Abbrechen"
        Me.BIndizierungAbbrechen.UseVisualStyleBackColor = True
        '
        'BIndizierungStart
        '
        Me.BIndizierungStart.Location = New System.Drawing.Point(246, 82)
        Me.BIndizierungStart.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BIndizierungStart.Name = "BIndizierungStart"
        Me.BIndizierungStart.Size = New System.Drawing.Size(168, 42)
        Me.BIndizierungStart.TabIndex = 4
        Me.BIndizierungStart.Text = "Start"
        Me.BIndizierungStart.UseVisualStyleBackColor = True
        '
        'ProgressBarIndex
        '
        Me.ProgressBarIndex.Location = New System.Drawing.Point(8, 82)
        Me.ProgressBarIndex.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ProgressBarIndex.Name = "ProgressBarIndex"
        Me.ProgressBarIndex.Size = New System.Drawing.Size(234, 42)
        Me.ProgressBarIndex.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.Label1.Location = New System.Drawing.Point(3, 2)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(854, 54)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Einstellungen für die Kontaktsuche"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TabJournal
        '
        Me.TabJournal.Controls.Add(Me.TBHinweisJournal)
        Me.TabJournal.Controls.Add(Me.BJournalOrdnerErstellen)
        Me.TabJournal.Controls.Add(Me.LHeaderTabJournal)
        Me.TabJournal.Controls.Add(Me.GBJournal)
        Me.TabJournal.Controls.Add(Me.TreeViewJournal)
        Me.TabJournal.Location = New System.Drawing.Point(4, 29)
        Me.TabJournal.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.TabJournal.Name = "TabJournal"
        Me.TabJournal.Padding = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.TabJournal.Size = New System.Drawing.Size(860, 457)
        Me.TabJournal.TabIndex = 17
        Me.TabJournal.Text = "Journal"
        Me.TabJournal.UseVisualStyleBackColor = True
        '
        'TBHinweisJournal
        '
        Me.TBHinweisJournal.Location = New System.Drawing.Point(430, 66)
        Me.TBHinweisJournal.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.TBHinweisJournal.Multiline = True
        Me.TBHinweisJournal.Name = "TBHinweisJournal"
        Me.TBHinweisJournal.ReadOnly = True
        Me.TBHinweisJournal.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TBHinweisJournal.Size = New System.Drawing.Size(424, 89)
        Me.TBHinweisJournal.TabIndex = 2
        Me.TBHinweisJournal.TabStop = False
        Me.TBHinweisJournal.Text = resources.GetString("TBHinweisJournal.Text")
        '
        'BJournalOrdnerErstellen
        '
        Me.BJournalOrdnerErstellen.Location = New System.Drawing.Point(430, 174)
        Me.BJournalOrdnerErstellen.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.BJournalOrdnerErstellen.Name = "BJournalOrdnerErstellen"
        Me.BJournalOrdnerErstellen.Size = New System.Drawing.Size(426, 42)
        Me.BJournalOrdnerErstellen.TabIndex = 3
        Me.BJournalOrdnerErstellen.Text = "Outlook-Journalordner laden..."
        Me.BJournalOrdnerErstellen.UseVisualStyleBackColor = True
        '
        'LHeaderTabJournal
        '
        Me.LHeaderTabJournal.Dock = System.Windows.Forms.DockStyle.Top
        Me.LHeaderTabJournal.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.LHeaderTabJournal.Location = New System.Drawing.Point(3, 4)
        Me.LHeaderTabJournal.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LHeaderTabJournal.Name = "LHeaderTabJournal"
        Me.LHeaderTabJournal.Size = New System.Drawing.Size(854, 54)
        Me.LHeaderTabJournal.TabIndex = 0
        Me.LHeaderTabJournal.Text = "Einstellungen für das Journal"
        Me.LHeaderTabJournal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'GBJournal
        '
        Me.GBJournal.Controls.Add(Me.CBJournal)
        Me.GBJournal.Location = New System.Drawing.Point(0, 58)
        Me.GBJournal.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBJournal.Name = "GBJournal"
        Me.GBJournal.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBJournal.Size = New System.Drawing.Size(423, 75)
        Me.GBJournal.TabIndex = 1
        Me.GBJournal.TabStop = False
        Me.GBJournal.Text = "Outlook Journal"
        '
        'CBJournal
        '
        Me.CBJournal.AutoSize = True
        Me.CBJournal.Location = New System.Drawing.Point(9, 29)
        Me.CBJournal.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBJournal.MinimumSize = New System.Drawing.Size(400, 0)
        Me.CBJournal.Name = "CBJournal"
        Me.CBJournal.Size = New System.Drawing.Size(400, 24)
        Me.CBJournal.TabIndex = 0
        Me.CBJournal.Text = "Journaleinträge erstellen"
        Me.CBJournal.UseVisualStyleBackColor = True
        '
        'PKontakterstellung
        '
        Me.PKontakterstellung.Controls.Add(Me.GBKontakterstellung)
        Me.PKontakterstellung.Controls.Add(Me.BKontaktOrdnerErstellen)
        Me.PKontakterstellung.Controls.Add(Me.Label21)
        Me.PKontakterstellung.Controls.Add(Me.TBHinweisKontakterstellung)
        Me.PKontakterstellung.Controls.Add(Me.TreeViewKontakteErstellen)
        Me.PKontakterstellung.Location = New System.Drawing.Point(4, 29)
        Me.PKontakterstellung.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.PKontakterstellung.Name = "PKontakterstellung"
        Me.PKontakterstellung.Padding = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.PKontakterstellung.Size = New System.Drawing.Size(860, 457)
        Me.PKontakterstellung.TabIndex = 18
        Me.PKontakterstellung.Text = "Kontakterstellung"
        Me.PKontakterstellung.UseVisualStyleBackColor = True
        '
        'GBKontakterstellung
        '
        Me.GBKontakterstellung.Controls.Add(Me.CBKErstellen)
        Me.GBKontakterstellung.Location = New System.Drawing.Point(0, 58)
        Me.GBKontakterstellung.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.GBKontakterstellung.Name = "GBKontakterstellung"
        Me.GBKontakterstellung.Padding = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.GBKontakterstellung.Size = New System.Drawing.Size(423, 75)
        Me.GBKontakterstellung.TabIndex = 5
        Me.GBKontakterstellung.TabStop = False
        Me.GBKontakterstellung.Text = "Erstellung von Kontakten"
        '
        'BKontaktOrdnerErstellen
        '
        Me.BKontaktOrdnerErstellen.Location = New System.Drawing.Point(430, 174)
        Me.BKontaktOrdnerErstellen.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.BKontaktOrdnerErstellen.Name = "BKontaktOrdnerErstellen"
        Me.BKontaktOrdnerErstellen.Size = New System.Drawing.Size(426, 42)
        Me.BKontaktOrdnerErstellen.TabIndex = 3
        Me.BKontaktOrdnerErstellen.Text = "Outlook-Journalordner laden..."
        Me.BKontaktOrdnerErstellen.UseVisualStyleBackColor = True
        '
        'Label21
        '
        Me.Label21.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label21.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.Label21.Location = New System.Drawing.Point(3, 4)
        Me.Label21.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(854, 54)
        Me.Label21.TabIndex = 0
        Me.Label21.Text = "Einstellungen für die Kontakterstellung"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TBHinweisKontakterstellung
        '
        Me.TBHinweisKontakterstellung.Location = New System.Drawing.Point(430, 66)
        Me.TBHinweisKontakterstellung.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.TBHinweisKontakterstellung.Multiline = True
        Me.TBHinweisKontakterstellung.Name = "TBHinweisKontakterstellung"
        Me.TBHinweisKontakterstellung.ReadOnly = True
        Me.TBHinweisKontakterstellung.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TBHinweisKontakterstellung.Size = New System.Drawing.Size(424, 89)
        Me.TBHinweisKontakterstellung.TabIndex = 2
        Me.TBHinweisKontakterstellung.TabStop = False
        Me.TBHinweisKontakterstellung.Text = resources.GetString("TBHinweisKontakterstellung.Text")
        '
        'PTelefone
        '
        Me.PTelefone.Controls.Add(Me.GroupBox1)
        Me.PTelefone.Controls.Add(Me.Label15)
        Me.PTelefone.Location = New System.Drawing.Point(4, 29)
        Me.PTelefone.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PTelefone.Name = "PTelefone"
        Me.PTelefone.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PTelefone.Size = New System.Drawing.Size(860, 457)
        Me.PTelefone.TabIndex = 5
        Me.PTelefone.Text = "Telefone"
        Me.PTelefone.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.StatusStripTelefone)
        Me.GroupBox1.Controls.Add(Me.DGVTelList)
        Me.GroupBox1.Controls.Add(Me.BTelefonliste)
        Me.GroupBox1.Location = New System.Drawing.Point(0, 62)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Size = New System.Drawing.Size(855, 389)
        Me.GroupBox1.TabIndex = 27
        Me.GroupBox1.TabStop = False
        '
        'StatusStripTelefone
        '
        Me.StatusStripTelefone.Font = New System.Drawing.Font("Segoe UI", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StatusStripTelefone.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.StatusStripTelefone.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TSSL_Telefone})
        Me.StatusStripTelefone.Location = New System.Drawing.Point(4, 362)
        Me.StatusStripTelefone.Name = "StatusStripTelefone"
        Me.StatusStripTelefone.Padding = New System.Windows.Forms.Padding(1, 0, 16, 0)
        Me.StatusStripTelefone.Size = New System.Drawing.Size(847, 22)
        Me.StatusStripTelefone.SizingGrip = False
        Me.StatusStripTelefone.TabIndex = 37
        Me.StatusStripTelefone.Text = "StatusStrip1"
        '
        'TSSL_Telefone
        '
        Me.TSSL_Telefone.Margin = New System.Windows.Forms.Padding(0)
        Me.TSSL_Telefone.Name = "TSSL_Telefone"
        Me.TSSL_Telefone.Size = New System.Drawing.Size(59, 22)
        Me.TSSL_Telefone.Text = "Bereit..."
        '
        'BTelefonliste
        '
        Me.BTelefonliste.Location = New System.Drawing.Point(591, 304)
        Me.BTelefonliste.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BTelefonliste.Name = "BTelefonliste"
        Me.BTelefonliste.Size = New System.Drawing.Size(255, 42)
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
        'PPhoner
        '
        Me.PPhoner.Controls.Add(Me.TBPhonerHinweise)
        Me.PPhoner.Controls.Add(Me.GroupBox4)
        Me.PPhoner.Controls.Add(Me.Label31)
        Me.PPhoner.Controls.Add(Me.LinkPhoner)
        Me.PPhoner.Controls.Add(Me.Label7)
        Me.PPhoner.Controls.Add(Me.Label20)
        Me.PPhoner.Controls.Add(Me.Label30)
        Me.PPhoner.Location = New System.Drawing.Point(4, 29)
        Me.PPhoner.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.PPhoner.Name = "PPhoner"
        Me.PPhoner.Size = New System.Drawing.Size(860, 457)
        Me.PPhoner.TabIndex = 16
        Me.PPhoner.Text = "Phoner"
        Me.PPhoner.UseVisualStyleBackColor = True
        '
        'TBPhonerHinweise
        '
        Me.TBPhonerHinweise.Location = New System.Drawing.Point(330, 228)
        Me.TBPhonerHinweise.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.TBPhonerHinweise.Multiline = True
        Me.TBPhonerHinweise.Name = "TBPhonerHinweise"
        Me.TBPhonerHinweise.ReadOnly = True
        Me.TBPhonerHinweise.Size = New System.Drawing.Size(517, 204)
        Me.TBPhonerHinweise.TabIndex = 32
        Me.TBPhonerHinweise.Text = resources.GetString("TBPhonerHinweise.Text")
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.BPhonerTest)
        Me.GroupBox4.Controls.Add(Me.CBPhoner)
        Me.GroupBox4.Controls.Add(Me.LPhonerSIPTelefon)
        Me.GroupBox4.Controls.Add(Me.CBoxPhonerSIP)
        Me.GroupBox4.Controls.Add(Me.TBPhonerPasswort)
        Me.GroupBox4.Controls.Add(Me.LPassworPhoner)
        Me.GroupBox4.Location = New System.Drawing.Point(9, 228)
        Me.GroupBox4.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox4.Size = New System.Drawing.Size(313, 205)
        Me.GroupBox4.TabIndex = 31
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Phoner Einstellungen"
        '
        'BPhonerTest
        '
        Me.BPhonerTest.Enabled = False
        Me.BPhonerTest.Location = New System.Drawing.Point(9, 146)
        Me.BPhonerTest.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.BPhonerTest.Name = "BPhonerTest"
        Me.BPhonerTest.Size = New System.Drawing.Size(287, 50)
        Me.BPhonerTest.TabIndex = 32
        Me.BPhonerTest.Text = "Teste Authentifizierung"
        Me.BPhonerTest.UseVisualStyleBackColor = True
        '
        'CBPhoner
        '
        Me.CBPhoner.AutoSize = True
        Me.CBPhoner.Location = New System.Drawing.Point(9, 30)
        Me.CBPhoner.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBPhoner.Name = "CBPhoner"
        Me.CBPhoner.Size = New System.Drawing.Size(246, 24)
        Me.CBPhoner.TabIndex = 14
        Me.CBPhoner.Text = "Softphone Phoner verwenden"
        Me.CBPhoner.UseVisualStyleBackColor = True
        '
        'CBoxPhonerSIP
        '
        Me.CBoxPhonerSIP.Enabled = False
        Me.CBoxPhonerSIP.FormattingEnabled = True
        Me.CBoxPhonerSIP.Location = New System.Drawing.Point(9, 104)
        Me.CBoxPhonerSIP.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CBoxPhonerSIP.Name = "CBoxPhonerSIP"
        Me.CBoxPhonerSIP.Size = New System.Drawing.Size(148, 28)
        Me.CBoxPhonerSIP.TabIndex = 2
        '
        'TBPhonerPasswort
        '
        Me.TBPhonerPasswort.Enabled = False
        Me.TBPhonerPasswort.Location = New System.Drawing.Point(9, 66)
        Me.TBPhonerPasswort.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBPhonerPasswort.Name = "TBPhonerPasswort"
        Me.TBPhonerPasswort.Size = New System.Drawing.Size(148, 26)
        Me.TBPhonerPasswort.TabIndex = 7
        Me.TBPhonerPasswort.UseSystemPasswordChar = True
        '
        'LPassworPhoner
        '
        Me.LPassworPhoner.AutoSize = True
        Me.LPassworPhoner.Enabled = False
        Me.LPassworPhoner.Location = New System.Drawing.Point(166, 70)
        Me.LPassworPhoner.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LPassworPhoner.Name = "LPassworPhoner"
        Me.LPassworPhoner.Size = New System.Drawing.Size(129, 20)
        Me.LPassworPhoner.TabIndex = 6
        Me.LPassworPhoner.Text = "Phoner Passwort"
        '
        'Label31
        '
        Me.Label31.Location = New System.Drawing.Point(6, 78)
        Me.Label31.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(842, 85)
        Me.Label31.TabIndex = 30
        Me.Label31.Text = resources.GetString("Label31.Text")
        '
        'LinkPhoner
        '
        Me.LinkPhoner.AutoSize = True
        Me.LinkPhoner.Location = New System.Drawing.Point(440, 162)
        Me.LinkPhoner.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LinkPhoner.Name = "LinkPhoner"
        Me.LinkPhoner.Size = New System.Drawing.Size(122, 20)
        Me.LinkPhoner.TabIndex = 27
        Me.LinkPhoner.TabStop = True
        Me.LinkPhoner.Text = "www.phoner.de/"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(6, 184)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(492, 20)
        Me.Label7.TabIndex = 29
        Me.Label7.Text = "Phoner Copyright 2020 Heiko Sommerfeldt. Alle Rechte vorbehalten."
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(6, 162)
        Me.Label20.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(369, 20)
        Me.Label20.TabIndex = 28
        Me.Label20.Text = "Phoner kann über folgenden Link bezogen werden:"
        '
        'Label30
        '
        Me.Label30.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label30.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label30.Location = New System.Drawing.Point(0, 0)
        Me.Label30.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(860, 54)
        Me.Label30.TabIndex = 26
        Me.Label30.Text = "Einstellungen für Phoner"
        Me.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        Me.PLogging.Size = New System.Drawing.Size(860, 457)
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
        Me.CBoxMinLogLevel.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.CBoxMinLogLevel.Name = "CBoxMinLogLevel"
        Me.CBoxMinLogLevel.Size = New System.Drawing.Size(121, 28)
        Me.CBoxMinLogLevel.TabIndex = 27
        '
        'GBLogging
        '
        Me.GBLogging.Controls.Add(Me.LinkLogFile)
        Me.GBLogging.Controls.Add(Me.TBLogging)
        Me.GBLogging.Location = New System.Drawing.Point(0, 118)
        Me.GBLogging.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBLogging.Name = "GBLogging"
        Me.GBLogging.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBLogging.Size = New System.Drawing.Size(855, 335)
        Me.GBLogging.TabIndex = 26
        Me.GBLogging.TabStop = False
        '
        'LinkLogFile
        '
        Me.LinkLogFile.Location = New System.Drawing.Point(3, 308)
        Me.LinkLogFile.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LinkLogFile.MaximumSize = New System.Drawing.Size(848, 22)
        Me.LinkLogFile.Name = "LinkLogFile"
        Me.LinkLogFile.Size = New System.Drawing.Size(848, 22)
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
        Me.PInfo.Size = New System.Drawing.Size(860, 457)
        Me.PInfo.TabIndex = 4
        Me.PInfo.Text = "Info..."
        Me.PInfo.UseVisualStyleBackColor = True
        '
        'BArbeitsverzeichnis
        '
        Me.BArbeitsverzeichnis.Location = New System.Drawing.Point(615, 89)
        Me.BArbeitsverzeichnis.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BArbeitsverzeichnis.Name = "BArbeitsverzeichnis"
        Me.BArbeitsverzeichnis.Size = New System.Drawing.Size(232, 42)
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
        Me.Label16.Location = New System.Drawing.Point(4, 78)
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
        Me.RichTextBox1.Location = New System.Drawing.Point(4, 141)
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
        Me.LinkForum.Location = New System.Drawing.Point(360, 78)
        Me.LinkForum.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LinkForum.Name = "LinkForum"
        Me.LinkForum.Size = New System.Drawing.Size(225, 20)
        Me.LinkForum.TabIndex = 2
        Me.LinkForum.TabStop = True
        Me.LinkForum.Text = "www.ip-phone-forum.de"
        Me.LinkForum.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TreeViewKontakteSuche
        '
        Me.TreeViewKontakteSuche.ImageIndex = 0
        Me.TreeViewKontakteSuche.Location = New System.Drawing.Point(431, 222)
        Me.TreeViewKontakteSuche.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.TreeViewKontakteSuche.Name = "TreeViewKontakteSuche"
        Me.TreeViewKontakteSuche.SelectedImageIndex = 0
        Me.TreeViewKontakteSuche.ShowRootLines = False
        Me.TreeViewKontakteSuche.Size = New System.Drawing.Size(424, 230)
        Me.TreeViewKontakteSuche.TabIndex = 6
        '
        'TreeViewJournal
        '
        Me.TreeViewJournal.ImageIndex = 0
        Me.TreeViewJournal.Location = New System.Drawing.Point(431, 222)
        Me.TreeViewJournal.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.TreeViewJournal.Name = "TreeViewJournal"
        Me.TreeViewJournal.SelectedImageIndex = 0
        Me.TreeViewJournal.ShowRootLines = False
        Me.TreeViewJournal.Size = New System.Drawing.Size(424, 230)
        Me.TreeViewJournal.TabIndex = 4
        '
        'TreeViewKontakteErstellen
        '
        Me.TreeViewKontakteErstellen.ImageIndex = 0
        Me.TreeViewKontakteErstellen.Location = New System.Drawing.Point(431, 222)
        Me.TreeViewKontakteErstellen.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.TreeViewKontakteErstellen.Name = "TreeViewKontakteErstellen"
        Me.TreeViewKontakteErstellen.SelectedImageIndex = 0
        Me.TreeViewKontakteErstellen.ShowRootLines = False
        Me.TreeViewKontakteErstellen.Size = New System.Drawing.Size(424, 230)
        Me.TreeViewKontakteErstellen.TabIndex = 4
        '
        'DGVTelList
        '
        Me.DGVTelList.AllowUserToAddRows = False
        Me.DGVTelList.AllowUserToDeleteRows = False
        Me.DGVTelList.AllowUserToResizeColumns = False
        Me.DGVTelList.AllowUserToResizeRows = False
        Me.DGVTelList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DGVTelList.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DGVTelList.ColumnHeadersHeight = 34
        Me.DGVTelList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.DGVTelList.Dock = System.Windows.Forms.DockStyle.Top
        Me.DGVTelList.Location = New System.Drawing.Point(4, 24)
        Me.DGVTelList.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.DGVTelList.MultiSelect = False
        Me.DGVTelList.Name = "DGVTelList"
        Me.DGVTelList.RowHeadersVisible = False
        Me.DGVTelList.RowHeadersWidth = 62
        Me.DGVTelList.RowTemplate.Height = 28
        Me.DGVTelList.ShowEditingIcon = False
        Me.DGVTelList.Size = New System.Drawing.Size(847, 272)
        Me.DGVTelList.TabIndex = 36
        '
        'FormCfg
        '
        Me.AcceptButton = Me.BOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BAbbruch
        Me.ClientSize = New System.Drawing.Size(876, 562)
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FormCfg"
        Me.Text = "Einstellungen für das Fritz!Box Telefon-Dingsbums"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.FBDB_MP.ResumeLayout(False)
        Me.PGrundeinstellungen.ResumeLayout(False)
        Me.GBFormatierungTelefonnummern.ResumeLayout(False)
        Me.GBFormatierungTelefonnummern.PerformLayout()
        Me.GBEinstellungWählhilfe.ResumeLayout(False)
        Me.GBEinstellungWählhilfe.PerformLayout()
        Me.GBErforderlicheAngaben.ResumeLayout(False)
        Me.GBErforderlicheAngaben.PerformLayout()
        Me.PAnrufmonitor.ResumeLayout(False)
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.GBAnrListeMain.ResumeLayout(False)
        Me.GBAnrListeMain.PerformLayout()
        Me.GBAnrMonMain.ResumeLayout(False)
        Me.GBAnrMonMain.PerformLayout()
        Me.PanelAnrMon.ResumeLayout(False)
        Me.PanelAnrMon.PerformLayout()
        Me.PKontaktsuche.ResumeLayout(False)
        Me.PKontaktsuche.PerformLayout()
        Me.GBKontaktsuche.ResumeLayout(False)
        Me.GBKontaktsuche.PerformLayout()
        Me.GBRWS.ResumeLayout(False)
        Me.GBRWS.PerformLayout()
        Me.GBIndizierung.ResumeLayout(False)
        Me.GBIndizierung.PerformLayout()
        Me.TabJournal.ResumeLayout(False)
        Me.TabJournal.PerformLayout()
        Me.GBJournal.ResumeLayout(False)
        Me.GBJournal.PerformLayout()
        Me.PKontakterstellung.ResumeLayout(False)
        Me.PKontakterstellung.PerformLayout()
        Me.GBKontakterstellung.ResumeLayout(False)
        Me.GBKontakterstellung.PerformLayout()
        Me.PTelefone.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.StatusStripTelefone.ResumeLayout(False)
        Me.StatusStripTelefone.PerformLayout()
        Me.PPhoner.ResumeLayout(False)
        Me.PPhoner.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.PLogging.ResumeLayout(False)
        Me.PLogging.PerformLayout()
        Me.GBLogging.ResumeLayout(False)
        Me.GBLogging.PerformLayout()
        Me.PInfo.ResumeLayout(False)
        Me.PInfo.PerformLayout()
        CType(Me.DGVTelList, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BReset As System.Windows.Forms.Button
    Friend WithEvents BAbbruch As System.Windows.Forms.Button
    Friend WithEvents BApply As System.Windows.Forms.Button
    Friend WithEvents BOK As System.Windows.Forms.Button
    Friend WithEvents ToolTipFBDBConfig As System.Windows.Forms.ToolTip
    Friend WithEvents BXML As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents FBDB_MP As Windows.Forms.TabControl
    Friend WithEvents PGrundeinstellungen As Windows.Forms.TabPage
    Friend WithEvents GBFormatierungTelefonnummern As Windows.Forms.GroupBox
    Friend WithEvents CBIgnoTelNrFormat As Windows.Forms.CheckBox
    Friend WithEvents LTelNrMaske As Windows.Forms.Label
    Friend WithEvents TBTelNrMaske As Windows.Forms.TextBox
    Friend WithEvents CBintl As Windows.Forms.CheckBox
    Friend WithEvents CBTelNrGruppieren As Windows.Forms.CheckBox
    Friend WithEvents GBEinstellungWählhilfe As Windows.Forms.GroupBox
    Friend WithEvents TBWClientEnblDauer As Windows.Forms.TextBox
    Friend WithEvents LWClientEnblDauer As Windows.Forms.Label
    Friend WithEvents CBCloseWClient As Windows.Forms.CheckBox
    Friend WithEvents CBDialPort As Windows.Forms.CheckBox
    Friend WithEvents CBCheckMobil As Windows.Forms.CheckBox
    Friend WithEvents CBForceDialLKZ As Windows.Forms.CheckBox
    Friend WithEvents CBCbCunterbinden As Windows.Forms.CheckBox
    Friend WithEvents LAmtsholung As Windows.Forms.Label
    Friend WithEvents TBAmt As Windows.Forms.TextBox
    Friend WithEvents GBErforderlicheAngaben As Windows.Forms.GroupBox
    Friend WithEvents BTestLogin As Windows.Forms.Button
    Friend WithEvents TBPasswort As Windows.Forms.MaskedTextBox
    Friend WithEvents LBenutzer As Windows.Forms.Label
    Friend WithEvents TBLandesKZ As Windows.Forms.TextBox
    Friend WithEvents TBOrtsKZ As Windows.Forms.TextBox
    Friend WithEvents TBBenutzer As Windows.Forms.TextBox
    Friend WithEvents CBForceFBAdr As Windows.Forms.CheckBox
    Friend WithEvents LLandeskennzahl As Windows.Forms.Label
    Friend WithEvents LOrtskennzahl As Windows.Forms.Label
    Friend WithEvents LPasswort As Windows.Forms.Label
    Friend WithEvents TBFBAdr As Windows.Forms.TextBox
    Friend WithEvents Label13 As Windows.Forms.Label
    Friend WithEvents PAnrufmonitor As Windows.Forms.TabPage
    Friend WithEvents GroupBox6 As Windows.Forms.GroupBox
    Friend WithEvents TBNumEntryList As Windows.Forms.TextBox
    Friend WithEvents LNumEntryList As Windows.Forms.Label
    Friend WithEvents GBAnrListeMain As Windows.Forms.GroupBox
    Friend WithEvents CBAnrListeShowAnrMon As Windows.Forms.CheckBox
    Friend WithEvents CBAutoAnrList As Windows.Forms.CheckBox
    Friend WithEvents CBAnrListeUpdateCallLists As Windows.Forms.CheckBox
    Friend WithEvents Label22 As Windows.Forms.Label
    Friend WithEvents GBAnrMonMain As Windows.Forms.GroupBox
    Friend WithEvents PanelAnrMon As Windows.Forms.Panel
    Friend WithEvents CBAnrMonContactImage As Windows.Forms.CheckBox
    Friend WithEvents CBShowMSN As Windows.Forms.CheckBox
    Friend WithEvents CBAnrMonZeigeKontakt As Windows.Forms.CheckBox
    Friend WithEvents CBAnrMonAuto As Windows.Forms.CheckBox
    Friend WithEvents CBAnrMonCloseAtDISSCONNECT As Windows.Forms.CheckBox
    Friend WithEvents CBAutoClose As Windows.Forms.CheckBox
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents TBEnblDauer As Windows.Forms.TextBox
    Friend WithEvents CLBTelNr As Windows.Forms.CheckedListBox
    Friend WithEvents LEnblDauer As Windows.Forms.Label
    Friend WithEvents CBUseAnrMon As Windows.Forms.CheckBox
    Friend WithEvents PKontaktsuche As Windows.Forms.TabPage
    Friend WithEvents TBHinweisKontaktsuche As Windows.Forms.TextBox
    Friend WithEvents GBKontaktsuche As Windows.Forms.GroupBox
    Friend WithEvents CBSucheUnterordner As Windows.Forms.CheckBox
    Friend WithEvents CBKontaktSucheFritzBox As Windows.Forms.CheckBox
    Friend WithEvents GBRWS As Windows.Forms.GroupBox
    Friend WithEvents BRWSTest As Windows.Forms.Button
    Friend WithEvents TBRWSTest As Windows.Forms.TextBox
    Friend WithEvents CBRWSIndex As Windows.Forms.CheckBox
    Friend WithEvents CBRWS As Windows.Forms.CheckBox
    Friend WithEvents BKontaktOrdnerSuche As Windows.Forms.Button
    Friend WithEvents GBIndizierung As Windows.Forms.GroupBox
    Friend WithEvents LabelAnzahl As Windows.Forms.Label
    Friend WithEvents RadioButtonEntfernen As Windows.Forms.RadioButton
    Friend WithEvents RadioButtonErstelle As Windows.Forms.RadioButton
    Friend WithEvents BIndizierungAbbrechen As Windows.Forms.Button
    Friend WithEvents BIndizierungStart As Windows.Forms.Button
    Friend WithEvents ProgressBarIndex As Windows.Forms.ProgressBar
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents TreeViewKontakteSuche As OlOrdnerTreeView
    Friend WithEvents TabJournal As Windows.Forms.TabPage
    Friend WithEvents TBHinweisJournal As Windows.Forms.TextBox
    Friend WithEvents BJournalOrdnerErstellen As Windows.Forms.Button
    Friend WithEvents LHeaderTabJournal As Windows.Forms.Label
    Friend WithEvents GBJournal As Windows.Forms.GroupBox
    Friend WithEvents CBJournal As Windows.Forms.CheckBox
    Friend WithEvents TreeViewJournal As OlOrdnerTreeView
    Friend WithEvents PKontakterstellung As Windows.Forms.TabPage
    Friend WithEvents GBKontakterstellung As Windows.Forms.GroupBox
    Friend WithEvents CBKErstellen As Windows.Forms.CheckBox
    Friend WithEvents BKontaktOrdnerErstellen As Windows.Forms.Button
    Friend WithEvents Label21 As Windows.Forms.Label
    Friend WithEvents TBHinweisKontakterstellung As Windows.Forms.TextBox
    Friend WithEvents TreeViewKontakteErstellen As OlOrdnerTreeView
    Friend WithEvents PTelefone As Windows.Forms.TabPage
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents StatusStripTelefone As Windows.Forms.StatusStrip
    Friend WithEvents TSSL_Telefone As Windows.Forms.ToolStripStatusLabel
    Friend WithEvents DGVTelList As FBoxDataGridView
    Friend WithEvents BTelefonliste As Windows.Forms.Button
    Friend WithEvents Label15 As Windows.Forms.Label
    Friend WithEvents PPhoner As Windows.Forms.TabPage
    Friend WithEvents TBPhonerHinweise As Windows.Forms.TextBox
    Friend WithEvents GroupBox4 As Windows.Forms.GroupBox
    Friend WithEvents BPhonerTest As Windows.Forms.Button
    Friend WithEvents CBPhoner As Windows.Forms.CheckBox
    Friend WithEvents LPhonerSIPTelefon As Windows.Forms.Label
    Friend WithEvents CBoxPhonerSIP As Windows.Forms.ComboBox
    Friend WithEvents TBPhonerPasswort As Windows.Forms.MaskedTextBox
    Friend WithEvents LPassworPhoner As Windows.Forms.Label
    Friend WithEvents Label31 As Windows.Forms.Label
    Friend WithEvents LinkPhoner As Windows.Forms.LinkLabel
    Friend WithEvents Label7 As Windows.Forms.Label
    Friend WithEvents Label20 As Windows.Forms.Label
    Friend WithEvents Label30 As Windows.Forms.Label
    Friend WithEvents PLogging As Windows.Forms.TabPage
    Friend WithEvents LMinLogLevel As Windows.Forms.Label
    Friend WithEvents CBoxMinLogLevel As Windows.Forms.ComboBox
    Friend WithEvents GBLogging As Windows.Forms.GroupBox
    Friend WithEvents LinkLogFile As Windows.Forms.LinkLabel
    Friend WithEvents TBLogging As Windows.Forms.TextBox
    Friend WithEvents Label23 As Windows.Forms.Label
    Friend WithEvents PInfo As Windows.Forms.TabPage
    Friend WithEvents BArbeitsverzeichnis As Windows.Forms.Button
    Friend WithEvents Label17 As Windows.Forms.Label
    Friend WithEvents Label16 As Windows.Forms.Label
    Friend WithEvents Label10 As Windows.Forms.Label
    Friend WithEvents LVersion As Windows.Forms.Label
    Friend WithEvents RichTextBox1 As Windows.Forms.RichTextBox
    Friend WithEvents LinkHomepage As Windows.Forms.LinkLabel
    Friend WithEvents LinkEmail As Windows.Forms.LinkLabel
    Friend WithEvents LinkForum As Windows.Forms.LinkLabel
End Class
