<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formTelefonbuch
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(formTelefonbuch))
        Dim XmlViewerSettings1 As FritzBoxDial.XMLViewerSettings = New FritzBoxDial.XMLViewerSettings()
        Me.StatStTelefonbuch = New System.Windows.Forms.StatusStrip()
        Me.TSSLTelefonbuch = New System.Windows.Forms.ToolStripStatusLabel()
        Me.TSTelefonbuch = New System.Windows.Forms.ToolStrip()
        Me.NeuToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ÖffnenToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.SpeichernToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ImportToolStrip = New System.Windows.Forms.ToolStripButton()
        Me.ExportToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.OFDAdressdbuch = New System.Windows.Forms.OpenFileDialog()
        Me.SFDTelefonbuch = New System.Windows.Forms.SaveFileDialog()
        Me.CMSTelefonbuch = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.TSMI_Add = New System.Windows.Forms.ToolStripMenuItem()
        Me.TSMI_Delete = New System.Windows.Forms.ToolStripMenuItem()
        Me.SCTelefonbuch = New System.Windows.Forms.SplitContainer()
        Me.TCTelefonbuch = New System.Windows.Forms.TabControl()
        Me.TPTelefonbuchDTV = New System.Windows.Forms.TabPage()
        Me.DGVTelefonbuch = New System.Windows.Forms.DataGridView()
        Me.Adrbk_ID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_uniqueid = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_Mod_Time = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_VIP = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.Adrbk_Name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_EMail = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Adrbk_Prio = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.AdrBk_TelNrHome = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_TelNrMobil = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_TelNrWork = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_TelNrFaxWork = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_KwV = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.AdrBk_Kurzwahl = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_Vanity = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TPTelefonbuchXML = New System.Windows.Forms.TabPage()
        Me.myXMLViewer = New FritzBoxDial.XMLViewer()
        Me.TBFBTelefonbuecher = New System.Windows.Forms.TabPage()
        Me.CBoxFBTelbuch = New System.Windows.Forms.ComboBox()
        Me.TBAdrbuchName = New System.Windows.Forms.TextBox()
        Me.LTelefonbuchName = New System.Windows.Forms.Label()
        Me.LHinweis = New System.Windows.Forms.Label()
        Me.BDel = New System.Windows.Forms.Button()
        Me.BAdd = New System.Windows.Forms.Button()
        Me.BTest = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.StatStTelefonbuch.SuspendLayout()
        Me.TSTelefonbuch.SuspendLayout()
        Me.CMSTelefonbuch.SuspendLayout()
        CType(Me.SCTelefonbuch, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SCTelefonbuch.Panel1.SuspendLayout()
        Me.SCTelefonbuch.Panel2.SuspendLayout()
        Me.SCTelefonbuch.SuspendLayout()
        Me.TCTelefonbuch.SuspendLayout()
        Me.TPTelefonbuchDTV.SuspendLayout()
        CType(Me.DGVTelefonbuch, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TPTelefonbuchXML.SuspendLayout()
        Me.TBFBTelefonbuecher.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatStTelefonbuch
        '
        Me.StatStTelefonbuch.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TSSLTelefonbuch})
        Me.StatStTelefonbuch.Location = New System.Drawing.Point(0, 540)
        Me.StatStTelefonbuch.Name = "StatStTelefonbuch"
        Me.StatStTelefonbuch.Size = New System.Drawing.Size(784, 22)
        Me.StatStTelefonbuch.TabIndex = 0
        Me.StatStTelefonbuch.Text = "StatusStrip1"
        '
        'TSSLTelefonbuch
        '
        Me.TSSLTelefonbuch.Name = "TSSLTelefonbuch"
        Me.TSSLTelefonbuch.Size = New System.Drawing.Size(39, 17)
        Me.TSSLTelefonbuch.Text = "Status"
        '
        'TSTelefonbuch
        '
        Me.TSTelefonbuch.AllowDrop = True
        Me.TSTelefonbuch.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NeuToolStripButton, Me.ÖffnenToolStripButton, Me.SpeichernToolStripButton, Me.ImportToolStrip, Me.ExportToolStripButton})
        Me.TSTelefonbuch.Location = New System.Drawing.Point(0, 0)
        Me.TSTelefonbuch.Name = "TSTelefonbuch"
        Me.TSTelefonbuch.Size = New System.Drawing.Size(784, 25)
        Me.TSTelefonbuch.TabIndex = 1
        Me.TSTelefonbuch.Text = "TSTelefonbuch"
        '
        'NeuToolStripButton
        '
        Me.NeuToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.NeuToolStripButton.Image = CType(resources.GetObject("NeuToolStripButton.Image"), System.Drawing.Image)
        Me.NeuToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.NeuToolStripButton.Name = "NeuToolStripButton"
        Me.NeuToolStripButton.Size = New System.Drawing.Size(33, 22)
        Me.NeuToolStripButton.Text = "&Neu"
        '
        'ÖffnenToolStripButton
        '
        Me.ÖffnenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ÖffnenToolStripButton.Image = CType(resources.GetObject("ÖffnenToolStripButton.Image"), System.Drawing.Image)
        Me.ÖffnenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ÖffnenToolStripButton.Name = "ÖffnenToolStripButton"
        Me.ÖffnenToolStripButton.Size = New System.Drawing.Size(48, 22)
        Me.ÖffnenToolStripButton.Text = "Ö&ffnen"
        '
        'SpeichernToolStripButton
        '
        Me.SpeichernToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.SpeichernToolStripButton.Image = CType(resources.GetObject("SpeichernToolStripButton.Image"), System.Drawing.Image)
        Me.SpeichernToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.SpeichernToolStripButton.Name = "SpeichernToolStripButton"
        Me.SpeichernToolStripButton.Size = New System.Drawing.Size(63, 22)
        Me.SpeichernToolStripButton.Text = "&Speichern"
        '
        'ImportToolStrip
        '
        Me.ImportToolStrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ImportToolStrip.Image = CType(resources.GetObject("ImportToolStrip.Image"), System.Drawing.Image)
        Me.ImportToolStrip.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ImportToolStrip.Name = "ImportToolStrip"
        Me.ImportToolStrip.Size = New System.Drawing.Size(47, 22)
        Me.ImportToolStrip.Text = "&Import"
        '
        'ExportToolStripButton
        '
        Me.ExportToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ExportToolStripButton.Image = CType(resources.GetObject("ExportToolStripButton.Image"), System.Drawing.Image)
        Me.ExportToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ExportToolStripButton.Name = "ExportToolStripButton"
        Me.ExportToolStripButton.Size = New System.Drawing.Size(44, 22)
        Me.ExportToolStripButton.Text = "Export"
        '
        'OFDAdressdbuch
        '
        Me.OFDAdressdbuch.FileName = "OpenFileDialog1"
        '
        'CMSTelefonbuch
        '
        Me.CMSTelefonbuch.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TSMI_Add, Me.TSMI_Delete})
        Me.CMSTelefonbuch.Name = "CMSTelefonbuch"
        Me.CMSTelefonbuch.Size = New System.Drawing.Size(177, 48)
        '
        'TSMI_Add
        '
        Me.TSMI_Add.Name = "TSMI_Add"
        Me.TSMI_Add.Size = New System.Drawing.Size(176, 22)
        Me.TSMI_Add.Text = "Eintrag Hinzufügen"
        '
        'TSMI_Delete
        '
        Me.TSMI_Delete.Name = "TSMI_Delete"
        Me.TSMI_Delete.Size = New System.Drawing.Size(176, 22)
        Me.TSMI_Delete.Text = "Eintrag Löschen"
        '
        'SCTelefonbuch
        '
        Me.SCTelefonbuch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SCTelefonbuch.Location = New System.Drawing.Point(0, 25)
        Me.SCTelefonbuch.Name = "SCTelefonbuch"
        '
        'SCTelefonbuch.Panel1
        '
        Me.SCTelefonbuch.Panel1.Controls.Add(Me.TCTelefonbuch)
        '
        'SCTelefonbuch.Panel2
        '
        Me.SCTelefonbuch.Panel2.Controls.Add(Me.CBoxFBTelbuch)
        Me.SCTelefonbuch.Panel2.Controls.Add(Me.TBAdrbuchName)
        Me.SCTelefonbuch.Panel2.Controls.Add(Me.LTelefonbuchName)
        Me.SCTelefonbuch.Panel2.Controls.Add(Me.LHinweis)
        Me.SCTelefonbuch.Panel2.Controls.Add(Me.BDel)
        Me.SCTelefonbuch.Panel2.Controls.Add(Me.BAdd)
        Me.SCTelefonbuch.Panel2.Controls.Add(Me.BTest)
        Me.SCTelefonbuch.Size = New System.Drawing.Size(784, 515)
        Me.SCTelefonbuch.SplitterDistance = 600
        Me.SCTelefonbuch.TabIndex = 3
        '
        'TCTelefonbuch
        '
        Me.TCTelefonbuch.Alignment = System.Windows.Forms.TabAlignment.Bottom
        Me.TCTelefonbuch.AllowDrop = True
        Me.TCTelefonbuch.Controls.Add(Me.TPTelefonbuchDTV)
        Me.TCTelefonbuch.Controls.Add(Me.TPTelefonbuchXML)
        Me.TCTelefonbuch.Controls.Add(Me.TBFBTelefonbuecher)
        Me.TCTelefonbuch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TCTelefonbuch.Location = New System.Drawing.Point(0, 0)
        Me.TCTelefonbuch.Multiline = True
        Me.TCTelefonbuch.Name = "TCTelefonbuch"
        Me.TCTelefonbuch.SelectedIndex = 0
        Me.TCTelefonbuch.Size = New System.Drawing.Size(600, 515)
        Me.TCTelefonbuch.TabIndex = 4
        '
        'TPTelefonbuchDTV
        '
        Me.TPTelefonbuchDTV.AllowDrop = True
        Me.TPTelefonbuchDTV.Controls.Add(Me.DGVTelefonbuch)
        Me.TPTelefonbuchDTV.Location = New System.Drawing.Point(4, 4)
        Me.TPTelefonbuchDTV.Name = "TPTelefonbuchDTV"
        Me.TPTelefonbuchDTV.Padding = New System.Windows.Forms.Padding(3)
        Me.TPTelefonbuchDTV.Size = New System.Drawing.Size(592, 489)
        Me.TPTelefonbuchDTV.TabIndex = 0
        Me.TPTelefonbuchDTV.Text = "Telefonbuch"
        Me.TPTelefonbuchDTV.UseVisualStyleBackColor = True
        '
        'DGVTelefonbuch
        '
        Me.DGVTelefonbuch.AllowDrop = True
        Me.DGVTelefonbuch.AllowUserToAddRows = False
        Me.DGVTelefonbuch.AllowUserToResizeRows = False
        Me.DGVTelefonbuch.ColumnHeadersHeight = 25
        Me.DGVTelefonbuch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.DGVTelefonbuch.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Adrbk_ID, Me.AdrBk_uniqueid, Me.AdrBk_Mod_Time, Me.AdrBk_VIP, Me.Adrbk_Name, Me.AdrBk_EMail, Me.Adrbk_Prio, Me.AdrBk_TelNrHome, Me.AdrBk_TelNrMobil, Me.AdrBk_TelNrWork, Me.AdrBk_TelNrFaxWork, Me.AdrBk_KwV, Me.AdrBk_Kurzwahl, Me.AdrBk_Vanity})
        Me.DGVTelefonbuch.ContextMenuStrip = Me.CMSTelefonbuch
        Me.DGVTelefonbuch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVTelefonbuch.Enabled = False
        Me.DGVTelefonbuch.Location = New System.Drawing.Point(3, 3)
        Me.DGVTelefonbuch.Name = "DGVTelefonbuch"
        Me.DGVTelefonbuch.RowHeadersWidth = 25
        Me.DGVTelefonbuch.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DGVTelefonbuch.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGVTelefonbuch.Size = New System.Drawing.Size(586, 483)
        Me.DGVTelefonbuch.TabIndex = 3
        '
        'Adrbk_ID
        '
        Me.Adrbk_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.Adrbk_ID.DataPropertyName = "ID"
        Me.Adrbk_ID.FillWeight = 30.0!
        Me.Adrbk_ID.HeaderText = "ID"
        Me.Adrbk_ID.Name = "Adrbk_ID"
        Me.Adrbk_ID.ReadOnly = True
        Me.Adrbk_ID.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Adrbk_ID.Width = 30
        '
        'AdrBk_uniqueid
        '
        Me.AdrBk_uniqueid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.AdrBk_uniqueid.DataPropertyName = "uniqueid"
        Me.AdrBk_uniqueid.FillWeight = 30.0!
        Me.AdrBk_uniqueid.HeaderText = "uID"
        Me.AdrBk_uniqueid.Name = "AdrBk_uniqueid"
        Me.AdrBk_uniqueid.ReadOnly = True
        Me.AdrBk_uniqueid.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.AdrBk_uniqueid.ToolTipText = "uniqueid"
        Me.AdrBk_uniqueid.Width = 30
        '
        'AdrBk_Mod_Time
        '
        Me.AdrBk_Mod_Time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.AdrBk_Mod_Time.DataPropertyName = "Mod_Time"
        Me.AdrBk_Mod_Time.FillWeight = 80.0!
        Me.AdrBk_Mod_Time.HeaderText = "Mod Time"
        Me.AdrBk_Mod_Time.Name = "AdrBk_Mod_Time"
        Me.AdrBk_Mod_Time.ReadOnly = True
        Me.AdrBk_Mod_Time.Width = 80
        '
        'AdrBk_VIP
        '
        Me.AdrBk_VIP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.AdrBk_VIP.DataPropertyName = "Category"
        Me.AdrBk_VIP.FillWeight = 30.0!
        Me.AdrBk_VIP.HeaderText = "VIP"
        Me.AdrBk_VIP.Name = "AdrBk_VIP"
        Me.AdrBk_VIP.Width = 30
        '
        'Adrbk_Name
        '
        Me.Adrbk_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.Adrbk_Name.DataPropertyName = "RealName"
        Me.Adrbk_Name.HeaderText = "Name"
        Me.Adrbk_Name.MinimumWidth = 150
        Me.Adrbk_Name.Name = "Adrbk_Name"
        '
        'AdrBk_EMail
        '
        Me.AdrBk_EMail.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.AdrBk_EMail.DataPropertyName = "EMail"
        Me.AdrBk_EMail.FillWeight = 200.0!
        Me.AdrBk_EMail.HeaderText = "E-Mail"
        Me.AdrBk_EMail.MinimumWidth = 200
        Me.AdrBk_EMail.Name = "AdrBk_EMail"
        '
        'Adrbk_Prio
        '
        Me.Adrbk_Prio.DataPropertyName = "TelNr_Prio"
        Me.Adrbk_Prio.HeaderText = "Hauptnummer"
        Me.Adrbk_Prio.Items.AddRange(New Object() {"Privat", "Mobil", "Geschäftlich", "Fax"})
        Me.Adrbk_Prio.Name = "Adrbk_Prio"
        '
        'AdrBk_TelNrHome
        '
        Me.AdrBk_TelNrHome.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.AdrBk_TelNrHome.DataPropertyName = "TelNr_home_TelNr"
        Me.AdrBk_TelNrHome.HeaderText = "Privat"
        Me.AdrBk_TelNrHome.MinimumWidth = 120
        Me.AdrBk_TelNrHome.Name = "AdrBk_TelNrHome"
        '
        'AdrBk_TelNrMobil
        '
        Me.AdrBk_TelNrMobil.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.AdrBk_TelNrMobil.DataPropertyName = "TelNr_mobile_TelNr"
        Me.AdrBk_TelNrMobil.HeaderText = "Mobil"
        Me.AdrBk_TelNrMobil.MinimumWidth = 120
        Me.AdrBk_TelNrMobil.Name = "AdrBk_TelNrMobil"
        '
        'AdrBk_TelNrWork
        '
        Me.AdrBk_TelNrWork.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.AdrBk_TelNrWork.DataPropertyName = "TelNr_work_TelNr"
        Me.AdrBk_TelNrWork.HeaderText = "Geschäftlich"
        Me.AdrBk_TelNrWork.MinimumWidth = 120
        Me.AdrBk_TelNrWork.Name = "AdrBk_TelNrWork"
        '
        'AdrBk_TelNrFaxWork
        '
        Me.AdrBk_TelNrFaxWork.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.AdrBk_TelNrFaxWork.DataPropertyName = "TelNr_fax_work_TelNr"
        Me.AdrBk_TelNrFaxWork.HeaderText = "Fax"
        Me.AdrBk_TelNrFaxWork.MinimumWidth = 120
        Me.AdrBk_TelNrFaxWork.Name = "AdrBk_TelNrFaxWork"
        '
        'AdrBk_KwV
        '
        Me.AdrBk_KwV.DataPropertyName = "TelNr_kwV"
        Me.AdrBk_KwV.HeaderText = "Kurzwahl-/Vanity"
        Me.AdrBk_KwV.Items.AddRange(New Object() {"Privat", "Mobil", "Geschäftlich", "Fax"})
        Me.AdrBk_KwV.Name = "AdrBk_KwV"
        '
        'AdrBk_Kurzwahl
        '
        Me.AdrBk_Kurzwahl.DataPropertyName = "TelNr_Kurzwahl"
        Me.AdrBk_Kurzwahl.HeaderText = "Kurzwahl"
        Me.AdrBk_Kurzwahl.Name = "AdrBk_Kurzwahl"
        '
        'AdrBk_Vanity
        '
        Me.AdrBk_Vanity.DataPropertyName = "TelNr_Vanity"
        Me.AdrBk_Vanity.HeaderText = "Vanity"
        Me.AdrBk_Vanity.Name = "AdrBk_Vanity"
        '
        'TPTelefonbuchXML
        '
        Me.TPTelefonbuchXML.Controls.Add(Me.myXMLViewer)
        Me.TPTelefonbuchXML.Location = New System.Drawing.Point(4, 4)
        Me.TPTelefonbuchXML.Name = "TPTelefonbuchXML"
        Me.TPTelefonbuchXML.Padding = New System.Windows.Forms.Padding(3)
        Me.TPTelefonbuchXML.Size = New System.Drawing.Size(592, 489)
        Me.TPTelefonbuchXML.TabIndex = 1
        Me.TPTelefonbuchXML.Text = "XML"
        Me.TPTelefonbuchXML.UseVisualStyleBackColor = True
        '
        'myXMLViewer
        '
        Me.myXMLViewer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.myXMLViewer.Location = New System.Drawing.Point(3, 3)
        Me.myXMLViewer.Name = "myXMLViewer"
        Me.myXMLViewer.ReadOnly = True
        XmlViewerSettings1.AttributeKey = System.Drawing.Color.Red
        XmlViewerSettings1.AttributeValue = System.Drawing.Color.Blue
        XmlViewerSettings1.Element = System.Drawing.Color.DarkRed
        XmlViewerSettings1.Tag = System.Drawing.Color.Blue
        XmlViewerSettings1.Value = System.Drawing.Color.Black
        Me.myXMLViewer.Settings = XmlViewerSettings1
        Me.myXMLViewer.Size = New System.Drawing.Size(586, 483)
        Me.myXMLViewer.TabIndex = 0
        Me.myXMLViewer.Text = ""
        '
        'TBFBTelefonbuecher
        '
        Me.TBFBTelefonbuecher.Controls.Add(Me.TextBox1)
        Me.TBFBTelefonbuecher.Location = New System.Drawing.Point(4, 4)
        Me.TBFBTelefonbuecher.Name = "TBFBTelefonbuecher"
        Me.TBFBTelefonbuecher.Padding = New System.Windows.Forms.Padding(3)
        Me.TBFBTelefonbuecher.Size = New System.Drawing.Size(592, 489)
        Me.TBFBTelefonbuecher.TabIndex = 2
        Me.TBFBTelefonbuecher.Text = "Fritz!Box Telefonbücher"
        Me.TBFBTelefonbuecher.UseVisualStyleBackColor = True
        '
        'CBoxFBTelbuch
        '
        Me.CBoxFBTelbuch.FormattingEnabled = True
        Me.CBoxFBTelbuch.Location = New System.Drawing.Point(4, 50)
        Me.CBoxFBTelbuch.Name = "CBoxFBTelbuch"
        Me.CBoxFBTelbuch.Size = New System.Drawing.Size(172, 21)
        Me.CBoxFBTelbuch.TabIndex = 5
        '
        'TBAdrbuchName
        '
        Me.TBAdrbuchName.Location = New System.Drawing.Point(4, 23)
        Me.TBAdrbuchName.Name = "TBAdrbuchName"
        Me.TBAdrbuchName.Size = New System.Drawing.Size(172, 20)
        Me.TBAdrbuchName.TabIndex = 4
        '
        'LTelefonbuchName
        '
        Me.LTelefonbuchName.AutoSize = True
        Me.LTelefonbuchName.Location = New System.Drawing.Point(6, 7)
        Me.LTelefonbuchName.Name = "LTelefonbuchName"
        Me.LTelefonbuchName.Size = New System.Drawing.Size(96, 13)
        Me.LTelefonbuchName.TabIndex = 3
        Me.LTelefonbuchName.Text = "Telefonbuchname:"
        '
        'LHinweis
        '
        Me.LHinweis.Location = New System.Drawing.Point(2, 132)
        Me.LHinweis.Name = "LHinweis"
        Me.LHinweis.Size = New System.Drawing.Size(174, 274)
        Me.LHinweis.TabIndex = 2
        Me.LHinweis.Text = resources.GetString("LHinweis.Text")
        '
        'BDel
        '
        Me.BDel.Location = New System.Drawing.Point(3, 106)
        Me.BDel.Name = "BDel"
        Me.BDel.Size = New System.Drawing.Size(174, 23)
        Me.BDel.TabIndex = 1
        Me.BDel.Text = "Löschen"
        Me.BDel.UseVisualStyleBackColor = True
        '
        'BAdd
        '
        Me.BAdd.Location = New System.Drawing.Point(3, 77)
        Me.BAdd.Name = "BAdd"
        Me.BAdd.Size = New System.Drawing.Size(174, 23)
        Me.BAdd.TabIndex = 1
        Me.BAdd.Text = "Hinzufügen"
        Me.BAdd.UseVisualStyleBackColor = True
        '
        'BTest
        '
        Me.BTest.Location = New System.Drawing.Point(3, 489)
        Me.BTest.Name = "BTest"
        Me.BTest.Size = New System.Drawing.Size(174, 23)
        Me.BTest.TabIndex = 0
        Me.BTest.Text = "Testen"
        Me.BTest.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(29, 29)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(515, 398)
        Me.TextBox1.TabIndex = 0
        '
        'formTelefonbuch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.SCTelefonbuch)
        Me.Controls.Add(Me.TSTelefonbuch)
        Me.Controls.Add(Me.StatStTelefonbuch)
        Me.Name = "formTelefonbuch"
        Me.ShowIcon = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.Text = "Fritz!Box Telefonbücher"
        Me.StatStTelefonbuch.ResumeLayout(False)
        Me.StatStTelefonbuch.PerformLayout()
        Me.TSTelefonbuch.ResumeLayout(False)
        Me.TSTelefonbuch.PerformLayout()
        Me.CMSTelefonbuch.ResumeLayout(False)
        Me.SCTelefonbuch.Panel1.ResumeLayout(False)
        Me.SCTelefonbuch.Panel2.ResumeLayout(False)
        Me.SCTelefonbuch.Panel2.PerformLayout()
        CType(Me.SCTelefonbuch, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SCTelefonbuch.ResumeLayout(False)
        Me.TCTelefonbuch.ResumeLayout(False)
        Me.TPTelefonbuchDTV.ResumeLayout(False)
        CType(Me.DGVTelefonbuch, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TPTelefonbuchXML.ResumeLayout(False)
        Me.TBFBTelefonbuecher.ResumeLayout(False)
        Me.TBFBTelefonbuecher.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatStTelefonbuch As System.Windows.Forms.StatusStrip
    Friend WithEvents TSTelefonbuch As System.Windows.Forms.ToolStrip
    Friend WithEvents ÖffnenToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents SpeichernToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents OFDAdressdbuch As System.Windows.Forms.OpenFileDialog
    Friend WithEvents SFDTelefonbuch As System.Windows.Forms.SaveFileDialog
    Friend WithEvents NeuToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents ImportToolStrip As System.Windows.Forms.ToolStripButton
    Friend WithEvents ExportToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents SCTelefonbuch As System.Windows.Forms.SplitContainer
    Friend WithEvents DGVTelefonbuch As System.Windows.Forms.DataGridView
    Friend WithEvents BTest As System.Windows.Forms.Button
    Friend WithEvents CMSTelefonbuch As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents TSMI_Delete As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TSMI_Add As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BDel As System.Windows.Forms.Button
    Friend WithEvents BAdd As System.Windows.Forms.Button
    Friend WithEvents LHinweis As System.Windows.Forms.Label
    Friend WithEvents Adrbk_ID As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_uniqueid As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_Mod_Time As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_VIP As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents Adrbk_Name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_EMail As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Adrbk_Prio As System.Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents AdrBk_TelNrHome As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_TelNrMobil As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_TelNrWork As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_TelNrFaxWork As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_KwV As System.Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents AdrBk_Kurzwahl As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_Vanity As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TCTelefonbuch As System.Windows.Forms.TabControl
    Friend WithEvents TPTelefonbuchDTV As System.Windows.Forms.TabPage
    Friend WithEvents TPTelefonbuchXML As System.Windows.Forms.TabPage
    Friend WithEvents myXMLViewer As New XMLViewer()
    Friend WithEvents TBAdrbuchName As System.Windows.Forms.TextBox
    Friend WithEvents LTelefonbuchName As System.Windows.Forms.Label
    Friend WithEvents TSSLTelefonbuch As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents TBFBTelefonbuecher As System.Windows.Forms.TabPage
    Friend WithEvents CBoxFBTelbuch As System.Windows.Forms.ComboBox
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
End Class
