<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formTBDTV
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
        Me.DGVTelefonbuch = New System.Windows.Forms.DataGridView()
        Me.Adrbk_ID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_uniqueid = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_Mod_Time = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_VIP = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.Adrbk_Name = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Adrbk_Prio = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.AdrBk_TelNrHome = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_TelNrMobil = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_TelNrWork = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_TelNrFaxWork = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_KwV = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.AdrBk_Kurzwahl = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_Vanity = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.AdrBk_EMail = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.DGVTelefonbuch, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DGVTelefonbuch
        '
        Me.DGVTelefonbuch.AllowUserToAddRows = False
        Me.DGVTelefonbuch.AllowUserToResizeRows = False
        Me.DGVTelefonbuch.ColumnHeadersHeight = 25
        Me.DGVTelefonbuch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.DGVTelefonbuch.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Adrbk_ID, Me.AdrBk_uniqueid, Me.AdrBk_Mod_Time, Me.AdrBk_VIP, Me.Adrbk_Name, Me.Adrbk_Prio, Me.AdrBk_TelNrHome, Me.AdrBk_TelNrMobil, Me.AdrBk_TelNrWork, Me.AdrBk_TelNrFaxWork, Me.AdrBk_KwV, Me.AdrBk_Kurzwahl, Me.AdrBk_Vanity, Me.AdrBk_EMail})
        Me.DGVTelefonbuch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVTelefonbuch.Enabled = False
        Me.DGVTelefonbuch.Location = New System.Drawing.Point(0, 0)
        Me.DGVTelefonbuch.Name = "DGVTelefonbuch"
        Me.DGVTelefonbuch.RowHeadersWidth = 25
        Me.DGVTelefonbuch.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DGVTelefonbuch.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGVTelefonbuch.Size = New System.Drawing.Size(1000, 500)
        Me.DGVTelefonbuch.TabIndex = 4
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
        Me.AdrBk_VIP.TrueValue = ""
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
        'AdrBk_EMail
        '
        Me.AdrBk_EMail.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.AdrBk_EMail.DataPropertyName = "EMail"
        Me.AdrBk_EMail.FillWeight = 200.0!
        Me.AdrBk_EMail.HeaderText = "E-Mail"
        Me.AdrBk_EMail.MinimumWidth = 200
        Me.AdrBk_EMail.Name = "AdrBk_EMail"
        '
        'formTBTabView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1000, 500)
        Me.ControlBox = False
        Me.Controls.Add(Me.DGVTelefonbuch)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "formTBTabView"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "formTBTabView"
        CType(Me.DGVTelefonbuch, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DGVTelefonbuch As System.Windows.Forms.DataGridView
    Friend WithEvents Adrbk_ID As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_uniqueid As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_Mod_Time As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_VIP As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents Adrbk_Name As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Adrbk_Prio As System.Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents AdrBk_TelNrHome As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_TelNrMobil As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_TelNrWork As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_TelNrFaxWork As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_KwV As System.Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents AdrBk_Kurzwahl As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_Vanity As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents AdrBk_EMail As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
