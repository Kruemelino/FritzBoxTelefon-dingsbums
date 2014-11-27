<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formAdressbuch
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(formAdressbuch))
        Me.StatStAdressbuch = New System.Windows.Forms.StatusStrip()
        Me.TSAdressbuch = New System.Windows.Forms.ToolStrip()
        Me.NeuToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ÖffnenToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.SpeichernToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ImportToolStrip = New System.Windows.Forms.ToolStripButton()
        Me.ExportToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.OFDAdressdbuch = New System.Windows.Forms.OpenFileDialog()
        Me.SFDAdressbuch = New System.Windows.Forms.SaveFileDialog()
        Me.SCAdressbuch = New System.Windows.Forms.SplitContainer()
        Me.DGVAdressbuch = New System.Windows.Forms.DataGridView()
        Me.CMSAdressbuch = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.TSMI_Add = New System.Windows.Forms.ToolStripMenuItem()
        Me.TSMI_Delete = New System.Windows.Forms.ToolStripMenuItem()
        Me.BTest = New System.Windows.Forms.Button()
        Me.Adrbk_ID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_uniqueid = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_Mod_Time = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_VIP = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.Adrbk_Name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_TelNrHome_prio = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.AdrBk_TelNrHome = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_TelNrMobil_prio = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.AdrBk_TelNrMobil = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_TelNrWork_prio = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.AdrBk_TelNrWork = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_TelNrFax_prio = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.AdrBk_TelNrFaxWork = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_EMail = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TSAdressbuch.SuspendLayout()
        CType(Me.SCAdressbuch, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SCAdressbuch.Panel1.SuspendLayout()
        Me.SCAdressbuch.Panel2.SuspendLayout()
        Me.SCAdressbuch.SuspendLayout()
        CType(Me.DGVAdressbuch, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.CMSAdressbuch.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatStAdressbuch
        '
        Me.StatStAdressbuch.Location = New System.Drawing.Point(0, 540)
        Me.StatStAdressbuch.Name = "StatStAdressbuch"
        Me.StatStAdressbuch.Size = New System.Drawing.Size(784, 22)
        Me.StatStAdressbuch.TabIndex = 0
        Me.StatStAdressbuch.Text = "StatusStrip1"
        '
        'TSAdressbuch
        '
        Me.TSAdressbuch.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NeuToolStripButton, Me.ÖffnenToolStripButton, Me.SpeichernToolStripButton, Me.ImportToolStrip, Me.ExportToolStripButton})
        Me.TSAdressbuch.Location = New System.Drawing.Point(0, 0)
        Me.TSAdressbuch.Name = "TSAdressbuch"
        Me.TSAdressbuch.Size = New System.Drawing.Size(784, 25)
        Me.TSAdressbuch.TabIndex = 1
        Me.TSAdressbuch.Text = "TSAdressbuch"
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
        'SCAdressbuch
        '
        Me.SCAdressbuch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SCAdressbuch.Location = New System.Drawing.Point(0, 25)
        Me.SCAdressbuch.Name = "SCAdressbuch"
        '
        'SCAdressbuch.Panel1
        '
        Me.SCAdressbuch.Panel1.Controls.Add(Me.DGVAdressbuch)
        '
        'SCAdressbuch.Panel2
        '
        Me.SCAdressbuch.Panel2.Controls.Add(Me.BTest)
        Me.SCAdressbuch.Size = New System.Drawing.Size(784, 515)
        Me.SCAdressbuch.SplitterDistance = 699
        Me.SCAdressbuch.TabIndex = 3
        '
        'DGVAdressbuch
        '
        Me.DGVAdressbuch.AllowDrop = True
        Me.DGVAdressbuch.AllowUserToResizeRows = False
        Me.DGVAdressbuch.ColumnHeadersHeight = 25
        Me.DGVAdressbuch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.DGVAdressbuch.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Adrbk_ID, Me.AdrBk_uniqueid, Me.AdrBk_Mod_Time, Me.AdrBk_VIP, Me.Adrbk_Name, Me.AdrBk_TelNrHome_prio, Me.AdrBk_TelNrHome, Me.AdrBk_TelNrMobil_prio, Me.AdrBk_TelNrMobil, Me.AdrBk_TelNrWork_prio, Me.AdrBk_TelNrWork, Me.AdrBk_TelNrFax_prio, Me.AdrBk_TelNrFaxWork, Me.AdrBk_EMail})
        Me.DGVAdressbuch.ContextMenuStrip = Me.CMSAdressbuch
        Me.DGVAdressbuch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVAdressbuch.Enabled = False
        Me.DGVAdressbuch.Location = New System.Drawing.Point(0, 0)
        Me.DGVAdressbuch.Name = "DGVAdressbuch"
        Me.DGVAdressbuch.RowHeadersWidth = 25
        Me.DGVAdressbuch.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DGVAdressbuch.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGVAdressbuch.Size = New System.Drawing.Size(699, 515)
        Me.DGVAdressbuch.TabIndex = 3
        '
        'CMSAdressbuch
        '
        Me.CMSAdressbuch.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TSMI_Add, Me.TSMI_Delete})
        Me.CMSAdressbuch.Name = "CMSAdressbuch"
        Me.CMSAdressbuch.Size = New System.Drawing.Size(177, 48)
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
        'BTest
        '
        Me.BTest.Location = New System.Drawing.Point(3, 3)
        Me.BTest.Name = "BTest"
        Me.BTest.Size = New System.Drawing.Size(75, 23)
        Me.BTest.TabIndex = 0
        Me.BTest.Text = "Testen"
        Me.BTest.UseVisualStyleBackColor = True
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
        'AdrBk_TelNrHome_prio
        '
        Me.AdrBk_TelNrHome_prio.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.AdrBk_TelNrHome_prio.DataPropertyName = "TelNr_home_Prio"
        Me.AdrBk_TelNrHome_prio.FillWeight = 30.0!
        Me.AdrBk_TelNrHome_prio.HeaderText = "prio"
        Me.AdrBk_TelNrHome_prio.Name = "AdrBk_TelNrHome_prio"
        Me.AdrBk_TelNrHome_prio.ToolTipText = "Hauptnummer"
        Me.AdrBk_TelNrHome_prio.Width = 30
        '
        'AdrBk_TelNrHome
        '
        Me.AdrBk_TelNrHome.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.AdrBk_TelNrHome.DataPropertyName = "TelNr_home_TelNr"
        Me.AdrBk_TelNrHome.HeaderText = "Nummer (Home)"
        Me.AdrBk_TelNrHome.MinimumWidth = 120
        Me.AdrBk_TelNrHome.Name = "AdrBk_TelNrHome"
        '
        'AdrBk_TelNrMobil_prio
        '
        Me.AdrBk_TelNrMobil_prio.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.AdrBk_TelNrMobil_prio.DataPropertyName = "TelNr_mobile_Prio"
        Me.AdrBk_TelNrMobil_prio.FillWeight = 30.0!
        Me.AdrBk_TelNrMobil_prio.HeaderText = "prio"
        Me.AdrBk_TelNrMobil_prio.Name = "AdrBk_TelNrMobil_prio"
        Me.AdrBk_TelNrMobil_prio.Width = 30
        '
        'AdrBk_TelNrMobil
        '
        Me.AdrBk_TelNrMobil.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.AdrBk_TelNrMobil.DataPropertyName = "TelNr_mobile_TelNr"
        Me.AdrBk_TelNrMobil.HeaderText = "Nummer (Mobil)"
        Me.AdrBk_TelNrMobil.MinimumWidth = 120
        Me.AdrBk_TelNrMobil.Name = "AdrBk_TelNrMobil"
        '
        'AdrBk_TelNrWork_prio
        '
        Me.AdrBk_TelNrWork_prio.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.AdrBk_TelNrWork_prio.DataPropertyName = "TelNr_work_Prio"
        Me.AdrBk_TelNrWork_prio.FillWeight = 30.0!
        Me.AdrBk_TelNrWork_prio.HeaderText = "prio"
        Me.AdrBk_TelNrWork_prio.Name = "AdrBk_TelNrWork_prio"
        Me.AdrBk_TelNrWork_prio.Width = 30
        '
        'AdrBk_TelNrWork
        '
        Me.AdrBk_TelNrWork.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.AdrBk_TelNrWork.DataPropertyName = "TelNr_work_TelNr"
        Me.AdrBk_TelNrWork.HeaderText = "Nummer (Work)"
        Me.AdrBk_TelNrWork.MinimumWidth = 120
        Me.AdrBk_TelNrWork.Name = "AdrBk_TelNrWork"
        '
        'AdrBk_TelNrFax_prio
        '
        Me.AdrBk_TelNrFax_prio.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.AdrBk_TelNrFax_prio.DataPropertyName = "TelNr_fax_work_Prio"
        Me.AdrBk_TelNrFax_prio.FillWeight = 30.0!
        Me.AdrBk_TelNrFax_prio.HeaderText = "prio"
        Me.AdrBk_TelNrFax_prio.Name = "AdrBk_TelNrFax_prio"
        Me.AdrBk_TelNrFax_prio.Width = 30
        '
        'AdrBk_TelNrFaxWork
        '
        Me.AdrBk_TelNrFaxWork.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.AdrBk_TelNrFaxWork.DataPropertyName = "TelNr_fax_work_TelNr"
        Me.AdrBk_TelNrFaxWork.HeaderText = "Nummer (Fax)"
        Me.AdrBk_TelNrFaxWork.MinimumWidth = 120
        Me.AdrBk_TelNrFaxWork.Name = "AdrBk_TelNrFaxWork"
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
        'formAdressbuch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.SCAdressbuch)
        Me.Controls.Add(Me.TSAdressbuch)
        Me.Controls.Add(Me.StatStAdressbuch)
        Me.Name = "formAdressbuch"
        Me.ShowIcon = false
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.Text = "Adressbuch"
        Me.TSAdressbuch.ResumeLayout(false)
        Me.TSAdressbuch.PerformLayout
        Me.SCAdressbuch.Panel1.ResumeLayout(false)
        Me.SCAdressbuch.Panel2.ResumeLayout(false)
        CType(Me.SCAdressbuch,System.ComponentModel.ISupportInitialize).EndInit
        Me.SCAdressbuch.ResumeLayout(false)
        CType(Me.DGVAdressbuch,System.ComponentModel.ISupportInitialize).EndInit
        Me.CMSAdressbuch.ResumeLayout(false)
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents StatStAdressbuch As System.Windows.Forms.StatusStrip
    Friend WithEvents TSAdressbuch As System.Windows.Forms.ToolStrip
    Friend WithEvents ÖffnenToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents SpeichernToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents OFDAdressdbuch As System.Windows.Forms.OpenFileDialog
    Friend WithEvents SFDAdressbuch As System.Windows.Forms.SaveFileDialog
    Friend WithEvents NeuToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents ImportToolStrip As System.Windows.Forms.ToolStripButton
    Friend WithEvents ExportToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents SCAdressbuch As System.Windows.Forms.SplitContainer
    Friend WithEvents DGVAdressbuch As System.Windows.Forms.DataGridView
    Friend WithEvents BTest As System.Windows.Forms.Button
    Friend WithEvents CMSAdressbuch As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents TSMI_Delete As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TSMI_Add As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Adrbk_ID As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_uniqueid As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_Mod_Time As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_VIP As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents Adrbk_Name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_TelNrHome_prio As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents AdrBk_TelNrHome As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_TelNrMobil_prio As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents AdrBk_TelNrMobil As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_TelNrWork_prio As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents AdrBk_TelNrWork As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_TelNrFax_prio As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents AdrBk_TelNrFaxWork As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_EMail As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
