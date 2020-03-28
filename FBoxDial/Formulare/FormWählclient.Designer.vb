<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormWählclient
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ButtonZeigeKontakt = New System.Windows.Forms.Button()
        Me.BSchließen = New System.Windows.Forms.Button()
        Me.BVIP = New System.Windows.Forms.CheckBox()
        Me.PicBoxKontaktBild = New System.Windows.Forms.PictureBox()
        Me.PanelWählclient = New System.Windows.Forms.Panel()
        Me.CBCLIR = New System.Windows.Forms.CheckBox()
        Me.PanelDirektwahl = New System.Windows.Forms.Panel()
        Me.LDirektwahl = New System.Windows.Forms.Label()
        Me.BWählenDirektwahl = New System.Windows.Forms.Button()
        Me.TBDirektwahl = New System.Windows.Forms.TextBox()
        Me.PanelKontaktwahl = New System.Windows.Forms.Panel()
        Me.dgvKontaktNr = New FBoxDial.FBoxDataGridView()
        Me.GBoxStatus = New System.Windows.Forms.GroupBox()
        Me.LStatus = New System.Windows.Forms.Label()
        Me.TBStatus = New System.Windows.Forms.TextBox()
        Me.BCancelCall = New System.Windows.Forms.Button()
        Me.GBoxVerbinden = New System.Windows.Forms.GroupBox()
        Me.ComboBoxFon = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.PicBoxKontaktBild, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelWählclient.SuspendLayout()
        Me.PanelDirektwahl.SuspendLayout()
        Me.PanelKontaktwahl.SuspendLayout()
        CType(Me.dgvKontaktNr, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GBoxStatus.SuspendLayout()
        Me.GBoxVerbinden.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 176.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.Panel1, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.PanelWählclient, 0, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(778, 394)
        Me.TableLayoutPanel2.TabIndex = 3
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.ButtonZeigeKontakt)
        Me.Panel1.Controls.Add(Me.BSchließen)
        Me.Panel1.Controls.Add(Me.BVIP)
        Me.Panel1.Controls.Add(Me.PicBoxKontaktBild)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(605, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(170, 388)
        Me.Panel1.TabIndex = 0
        '
        'ButtonZeigeKontakt
        '
        Me.ButtonZeigeKontakt.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonZeigeKontakt.Enabled = False
        Me.ButtonZeigeKontakt.Location = New System.Drawing.Point(0, 250)
        Me.ButtonZeigeKontakt.Name = "ButtonZeigeKontakt"
        Me.ButtonZeigeKontakt.Size = New System.Drawing.Size(170, 39)
        Me.ButtonZeigeKontakt.TabIndex = 3
        Me.ButtonZeigeKontakt.Text = "Zeige Kontakt"
        Me.ButtonZeigeKontakt.UseVisualStyleBackColor = True
        '
        'BSchließen
        '
        Me.BSchließen.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BSchließen.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BSchließen.Location = New System.Drawing.Point(0, 343)
        Me.BSchließen.Name = "BSchließen"
        Me.BSchließen.Size = New System.Drawing.Size(170, 39)
        Me.BSchließen.TabIndex = 21
        Me.BSchließen.Text = "Schließen"
        Me.BSchließen.UseVisualStyleBackColor = True
        '
        'BVIP
        '
        Me.BVIP.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BVIP.Appearance = System.Windows.Forms.Appearance.Button
        Me.BVIP.Enabled = False
        Me.BVIP.Location = New System.Drawing.Point(0, 295)
        Me.BVIP.Name = "BVIP"
        Me.BVIP.Size = New System.Drawing.Size(170, 39)
        Me.BVIP.TabIndex = 22
        Me.BVIP.Text = "VIP"
        Me.BVIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.BVIP.UseVisualStyleBackColor = True
        '
        'PicBoxKontaktBild
        '
        Me.PicBoxKontaktBild.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PicBoxKontaktBild.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicBoxKontaktBild.Location = New System.Drawing.Point(0, 0)
        Me.PicBoxKontaktBild.Name = "PicBoxKontaktBild"
        Me.PicBoxKontaktBild.Size = New System.Drawing.Size(170, 213)
        Me.PicBoxKontaktBild.TabIndex = 17
        Me.PicBoxKontaktBild.TabStop = False
        '
        'PanelWählclient
        '
        Me.PanelWählclient.Controls.Add(Me.CBCLIR)
        Me.PanelWählclient.Controls.Add(Me.PanelDirektwahl)
        Me.PanelWählclient.Controls.Add(Me.PanelKontaktwahl)
        Me.PanelWählclient.Controls.Add(Me.GBoxStatus)
        Me.PanelWählclient.Controls.Add(Me.GBoxVerbinden)
        Me.PanelWählclient.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelWählclient.Location = New System.Drawing.Point(3, 3)
        Me.PanelWählclient.Name = "PanelWählclient"
        Me.PanelWählclient.Size = New System.Drawing.Size(596, 388)
        Me.PanelWählclient.TabIndex = 1
        '
        'CBCLIR
        '
        Me.CBCLIR.AutoSize = True
        Me.CBCLIR.Location = New System.Drawing.Point(9, 287)
        Me.CBCLIR.Name = "CBCLIR"
        Me.CBCLIR.Size = New System.Drawing.Size(217, 24)
        Me.CBCLIR.TabIndex = 5
        Me.CBCLIR.Text = "Rufnummer unterdrücken"
        Me.CBCLIR.TextAlign = System.Drawing.ContentAlignment.TopLeft
        Me.CBCLIR.UseVisualStyleBackColor = True
        '
        'PanelDirektwahl
        '
        Me.PanelDirektwahl.Controls.Add(Me.LDirektwahl)
        Me.PanelDirektwahl.Controls.Add(Me.BWählenDirektwahl)
        Me.PanelDirektwahl.Controls.Add(Me.TBDirektwahl)
        Me.PanelDirektwahl.Location = New System.Drawing.Point(0, 148)
        Me.PanelDirektwahl.Name = "PanelDirektwahl"
        Me.PanelDirektwahl.Size = New System.Drawing.Size(596, 133)
        Me.PanelDirektwahl.TabIndex = 13
        '
        'LDirektwahl
        '
        Me.LDirektwahl.AutoSize = True
        Me.LDirektwahl.Location = New System.Drawing.Point(8, 17)
        Me.LDirektwahl.Name = "LDirektwahl"
        Me.LDirektwahl.Size = New System.Drawing.Size(473, 20)
        Me.LDirektwahl.TabIndex = 8
        Me.LDirektwahl.Text = "Direktwahl - Geben Sie die zu wählende Telefonnummer direkt ein"
        '
        'BWählenDirektwahl
        '
        Me.BWählenDirektwahl.Location = New System.Drawing.Point(480, 56)
        Me.BWählenDirektwahl.Name = "BWählenDirektwahl"
        Me.BWählenDirektwahl.Size = New System.Drawing.Size(108, 39)
        Me.BWählenDirektwahl.TabIndex = 7
        Me.BWählenDirektwahl.Text = "Wählen"
        Me.BWählenDirektwahl.UseVisualStyleBackColor = True
        '
        'TBDirektwahl
        '
        Me.TBDirektwahl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TBDirektwahl.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TBDirektwahl.Location = New System.Drawing.Point(8, 56)
        Me.TBDirektwahl.Name = "TBDirektwahl"
        Me.TBDirektwahl.Size = New System.Drawing.Size(467, 39)
        Me.TBDirektwahl.TabIndex = 6
        Me.TBDirektwahl.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'PanelKontaktwahl
        '
        Me.PanelKontaktwahl.Controls.Add(Me.dgvKontaktNr)
        Me.PanelKontaktwahl.Location = New System.Drawing.Point(0, 0)
        Me.PanelKontaktwahl.Name = "PanelKontaktwahl"
        Me.PanelKontaktwahl.Size = New System.Drawing.Size(596, 137)
        Me.PanelKontaktwahl.TabIndex = 12
        '
        'dgvKontaktNr
        '
        Me.dgvKontaktNr.AllowUserToAddRows = False
        Me.dgvKontaktNr.AllowUserToDeleteRows = False
        Me.dgvKontaktNr.AllowUserToResizeColumns = False
        Me.dgvKontaktNr.AllowUserToResizeRows = False
        Me.dgvKontaktNr.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.dgvKontaktNr.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable
        Me.dgvKontaktNr.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvKontaktNr.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvKontaktNr.Location = New System.Drawing.Point(0, 0)
        Me.dgvKontaktNr.Name = "dgvKontaktNr"
        Me.dgvKontaktNr.RowHeadersVisible = False
        Me.dgvKontaktNr.RowHeadersWidth = 62
        Me.dgvKontaktNr.RowTemplate.Height = 28
        Me.dgvKontaktNr.Size = New System.Drawing.Size(596, 137)
        Me.dgvKontaktNr.TabIndex = 10
        '
        'GBoxStatus
        '
        Me.GBoxStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GBoxStatus.Controls.Add(Me.LStatus)
        Me.GBoxStatus.Controls.Add(Me.TBStatus)
        Me.GBoxStatus.Controls.Add(Me.BCancelCall)
        Me.GBoxStatus.Location = New System.Drawing.Point(279, 287)
        Me.GBoxStatus.Name = "GBoxStatus"
        Me.GBoxStatus.Size = New System.Drawing.Size(317, 102)
        Me.GBoxStatus.TabIndex = 9
        Me.GBoxStatus.TabStop = False
        Me.GBoxStatus.Text = "Status"
        '
        'LStatus
        '
        Me.LStatus.ForeColor = System.Drawing.Color.Red
        Me.LStatus.Location = New System.Drawing.Point(6, 23)
        Me.LStatus.Name = "LStatus"
        Me.LStatus.Size = New System.Drawing.Size(121, 30)
        Me.LStatus.TabIndex = 6
        Me.LStatus.Text = "Jetzt abheben"
        Me.LStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TBStatus
        '
        Me.TBStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TBStatus.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.TBStatus.Location = New System.Drawing.Point(133, 23)
        Me.TBStatus.Multiline = True
        Me.TBStatus.Name = "TBStatus"
        Me.TBStatus.ReadOnly = True
        Me.TBStatus.Size = New System.Drawing.Size(177, 72)
        Me.TBStatus.TabIndex = 5
        Me.TBStatus.WordWrap = False
        '
        'BCancelCall
        '
        Me.BCancelCall.Location = New System.Drawing.Point(6, 56)
        Me.BCancelCall.Name = "BCancelCall"
        Me.BCancelCall.Size = New System.Drawing.Size(121, 39)
        Me.BCancelCall.TabIndex = 4
        Me.BCancelCall.Text = "Abbruch"
        Me.BCancelCall.UseVisualStyleBackColor = True
        Me.BCancelCall.Visible = False
        '
        'GBoxVerbinden
        '
        Me.GBoxVerbinden.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GBoxVerbinden.Controls.Add(Me.ComboBoxFon)
        Me.GBoxVerbinden.Location = New System.Drawing.Point(3, 317)
        Me.GBoxVerbinden.Name = "GBoxVerbinden"
        Me.GBoxVerbinden.Size = New System.Drawing.Size(270, 72)
        Me.GBoxVerbinden.TabIndex = 0
        Me.GBoxVerbinden.TabStop = False
        Me.GBoxVerbinden.Text = "Verbinden über ..."
        '
        'ComboBoxFon
        '
        Me.ComboBoxFon.FormattingEnabled = True
        Me.ComboBoxFon.Location = New System.Drawing.Point(6, 32)
        Me.ComboBoxFon.Name = "ComboBoxFon"
        Me.ComboBoxFon.Size = New System.Drawing.Size(258, 28)
        Me.ComboBoxFon.TabIndex = 1
        '
        'FormWählclient
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(778, 394)
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "FormWählclient"
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        CType(Me.PicBoxKontaktBild, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelWählclient.ResumeLayout(False)
        Me.PanelWählclient.PerformLayout()
        Me.PanelDirektwahl.ResumeLayout(False)
        Me.PanelDirektwahl.PerformLayout()
        Me.PanelKontaktwahl.ResumeLayout(False)
        CType(Me.dgvKontaktNr, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GBoxStatus.ResumeLayout(False)
        Me.GBoxStatus.PerformLayout()
        Me.GBoxVerbinden.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel2 As Windows.Forms.TableLayoutPanel
    Friend WithEvents Panel1 As Windows.Forms.Panel
    Friend WithEvents PanelWählclient As Windows.Forms.Panel
    Friend WithEvents ButtonZeigeKontakt As Windows.Forms.Button
    Friend WithEvents PicBoxKontaktBild As Windows.Forms.PictureBox
    Friend WithEvents BSchließen As Windows.Forms.Button
    Friend WithEvents BVIP As Windows.Forms.CheckBox
    Friend WithEvents GBoxVerbinden As Windows.Forms.GroupBox
    Friend WithEvents ComboBoxFon As Windows.Forms.ComboBox
    Friend WithEvents GBoxStatus As Windows.Forms.GroupBox
    Friend WithEvents BCancelCall As Windows.Forms.Button
    Friend WithEvents dgvKontaktNr As FBoxDataGridView
    Friend WithEvents CBCLIR As Windows.Forms.CheckBox
    Friend WithEvents PanelKontaktwahl As Windows.Forms.Panel
    Friend WithEvents PanelDirektwahl As Windows.Forms.Panel
    Friend WithEvents LDirektwahl As Windows.Forms.Label
    Friend WithEvents BWählenDirektwahl As Windows.Forms.Button
    Friend WithEvents TBDirektwahl As Windows.Forms.TextBox
    Friend WithEvents TBStatus As Windows.Forms.TextBox
    Friend WithEvents LStatus As Windows.Forms.Label
End Class
