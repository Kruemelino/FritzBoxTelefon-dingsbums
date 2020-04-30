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
        Me.CBoxDirektwahl = New System.Windows.Forms.ComboBox()
        Me.LDirektwahl = New System.Windows.Forms.Label()
        Me.BWählenDirektwahl = New System.Windows.Forms.Button()
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
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 117.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.Panel1, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.PanelWählclient, 0, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel2.Margin = New System.Windows.Forms.Padding(2)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(519, 256)
        Me.TableLayoutPanel2.TabIndex = 3
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.ButtonZeigeKontakt)
        Me.Panel1.Controls.Add(Me.BSchließen)
        Me.Panel1.Controls.Add(Me.BVIP)
        Me.Panel1.Controls.Add(Me.PicBoxKontaktBild)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(404, 2)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(2)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(113, 252)
        Me.Panel1.TabIndex = 0
        '
        'ButtonZeigeKontakt
        '
        Me.ButtonZeigeKontakt.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonZeigeKontakt.Enabled = False
        Me.ButtonZeigeKontakt.Location = New System.Drawing.Point(0, 162)
        Me.ButtonZeigeKontakt.Margin = New System.Windows.Forms.Padding(2)
        Me.ButtonZeigeKontakt.Name = "ButtonZeigeKontakt"
        Me.ButtonZeigeKontakt.Size = New System.Drawing.Size(113, 25)
        Me.ButtonZeigeKontakt.TabIndex = 3
        Me.ButtonZeigeKontakt.Text = "Zeige Kontakt"
        Me.ButtonZeigeKontakt.UseVisualStyleBackColor = True
        '
        'BSchließen
        '
        Me.BSchließen.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BSchließen.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BSchließen.Location = New System.Drawing.Point(0, 223)
        Me.BSchließen.Margin = New System.Windows.Forms.Padding(2)
        Me.BSchließen.Name = "BSchließen"
        Me.BSchließen.Size = New System.Drawing.Size(113, 25)
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
        Me.BVIP.Location = New System.Drawing.Point(0, 192)
        Me.BVIP.Margin = New System.Windows.Forms.Padding(2)
        Me.BVIP.Name = "BVIP"
        Me.BVIP.Size = New System.Drawing.Size(113, 25)
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
        Me.PicBoxKontaktBild.Margin = New System.Windows.Forms.Padding(2)
        Me.PicBoxKontaktBild.Name = "PicBoxKontaktBild"
        Me.PicBoxKontaktBild.Size = New System.Drawing.Size(113, 139)
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
        Me.PanelWählclient.Location = New System.Drawing.Point(2, 2)
        Me.PanelWählclient.Margin = New System.Windows.Forms.Padding(2)
        Me.PanelWählclient.Name = "PanelWählclient"
        Me.PanelWählclient.Size = New System.Drawing.Size(398, 252)
        Me.PanelWählclient.TabIndex = 1
        '
        'CBCLIR
        '
        Me.CBCLIR.AutoSize = True
        Me.CBCLIR.Location = New System.Drawing.Point(6, 187)
        Me.CBCLIR.Margin = New System.Windows.Forms.Padding(2)
        Me.CBCLIR.Name = "CBCLIR"
        Me.CBCLIR.Size = New System.Drawing.Size(153, 21)
        Me.CBCLIR.TabIndex = 5
        Me.CBCLIR.Text = "Rufnummer unterdrücken"
        Me.CBCLIR.TextAlign = System.Drawing.ContentAlignment.TopLeft
        Me.CBCLIR.UseVisualStyleBackColor = True
        '
        'PanelDirektwahl
        '
        Me.PanelDirektwahl.Controls.Add(Me.CBoxDirektwahl)
        Me.PanelDirektwahl.Controls.Add(Me.LDirektwahl)
        Me.PanelDirektwahl.Controls.Add(Me.BWählenDirektwahl)
        Me.PanelDirektwahl.Location = New System.Drawing.Point(0, 96)
        Me.PanelDirektwahl.Margin = New System.Windows.Forms.Padding(2)
        Me.PanelDirektwahl.Name = "PanelDirektwahl"
        Me.PanelDirektwahl.Size = New System.Drawing.Size(397, 86)
        Me.PanelDirektwahl.TabIndex = 13
        '
        'CBoxDirektwahl
        '
        Me.CBoxDirektwahl.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CBoxDirektwahl.FormattingEnabled = True
        Me.CBoxDirektwahl.Location = New System.Drawing.Point(8, 36)
        Me.CBoxDirektwahl.Name = "CBoxDirektwahl"
        Me.CBoxDirektwahl.Size = New System.Drawing.Size(307, 40)
        Me.CBoxDirektwahl.TabIndex = 23
        '
        'LDirektwahl
        '
        Me.LDirektwahl.AutoSize = True
        Me.LDirektwahl.Location = New System.Drawing.Point(5, 11)
        Me.LDirektwahl.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LDirektwahl.Name = "LDirektwahl"
        Me.LDirektwahl.Size = New System.Drawing.Size(318, 13)
        Me.LDirektwahl.TabIndex = 8
        Me.LDirektwahl.Text = "Direktwahl - Geben Sie die zu wählende Telefonnummer direkt ein"
        '
        'BWählenDirektwahl
        '
        Me.BWählenDirektwahl.Location = New System.Drawing.Point(320, 36)
        Me.BWählenDirektwahl.Margin = New System.Windows.Forms.Padding(2)
        Me.BWählenDirektwahl.Name = "BWählenDirektwahl"
        Me.BWählenDirektwahl.Size = New System.Drawing.Size(72, 25)
        Me.BWählenDirektwahl.TabIndex = 7
        Me.BWählenDirektwahl.Text = "Wählen"
        Me.BWählenDirektwahl.UseVisualStyleBackColor = True
        '
        'PanelKontaktwahl
        '
        Me.PanelKontaktwahl.Controls.Add(Me.dgvKontaktNr)
        Me.PanelKontaktwahl.Location = New System.Drawing.Point(0, 0)
        Me.PanelKontaktwahl.Margin = New System.Windows.Forms.Padding(2)
        Me.PanelKontaktwahl.Name = "PanelKontaktwahl"
        Me.PanelKontaktwahl.Size = New System.Drawing.Size(397, 89)
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
        Me.dgvKontaktNr.Margin = New System.Windows.Forms.Padding(2)
        Me.dgvKontaktNr.Name = "dgvKontaktNr"
        Me.dgvKontaktNr.RowHeadersVisible = False
        Me.dgvKontaktNr.RowHeadersWidth = 62
        Me.dgvKontaktNr.RowTemplate.Height = 28
        Me.dgvKontaktNr.Size = New System.Drawing.Size(397, 89)
        Me.dgvKontaktNr.TabIndex = 10
        '
        'GBoxStatus
        '
        Me.GBoxStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GBoxStatus.Controls.Add(Me.LStatus)
        Me.GBoxStatus.Controls.Add(Me.TBStatus)
        Me.GBoxStatus.Controls.Add(Me.BCancelCall)
        Me.GBoxStatus.Location = New System.Drawing.Point(186, 187)
        Me.GBoxStatus.Margin = New System.Windows.Forms.Padding(2)
        Me.GBoxStatus.Name = "GBoxStatus"
        Me.GBoxStatus.Padding = New System.Windows.Forms.Padding(2)
        Me.GBoxStatus.Size = New System.Drawing.Size(212, 66)
        Me.GBoxStatus.TabIndex = 9
        Me.GBoxStatus.TabStop = False
        Me.GBoxStatus.Text = "Status"
        '
        'LStatus
        '
        Me.LStatus.ForeColor = System.Drawing.Color.Red
        Me.LStatus.Location = New System.Drawing.Point(4, 15)
        Me.LStatus.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.LStatus.Name = "LStatus"
        Me.LStatus.Size = New System.Drawing.Size(81, 19)
        Me.LStatus.TabIndex = 6
        Me.LStatus.Text = "Jetzt abheben"
        Me.LStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TBStatus
        '
        Me.TBStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TBStatus.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.TBStatus.Location = New System.Drawing.Point(89, 15)
        Me.TBStatus.Margin = New System.Windows.Forms.Padding(2)
        Me.TBStatus.Multiline = True
        Me.TBStatus.Name = "TBStatus"
        Me.TBStatus.ReadOnly = True
        Me.TBStatus.Size = New System.Drawing.Size(119, 48)
        Me.TBStatus.TabIndex = 5
        Me.TBStatus.WordWrap = False
        '
        'BCancelCall
        '
        Me.BCancelCall.Location = New System.Drawing.Point(4, 36)
        Me.BCancelCall.Margin = New System.Windows.Forms.Padding(2)
        Me.BCancelCall.Name = "BCancelCall"
        Me.BCancelCall.Size = New System.Drawing.Size(81, 25)
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
        Me.GBoxVerbinden.Location = New System.Drawing.Point(2, 206)
        Me.GBoxVerbinden.Margin = New System.Windows.Forms.Padding(2)
        Me.GBoxVerbinden.Name = "GBoxVerbinden"
        Me.GBoxVerbinden.Padding = New System.Windows.Forms.Padding(2)
        Me.GBoxVerbinden.Size = New System.Drawing.Size(181, 47)
        Me.GBoxVerbinden.TabIndex = 0
        Me.GBoxVerbinden.TabStop = False
        Me.GBoxVerbinden.Text = "Verbinden über ..."
        '
        'ComboBoxFon
        '
        Me.ComboBoxFon.FormattingEnabled = True
        Me.ComboBoxFon.Location = New System.Drawing.Point(4, 21)
        Me.ComboBoxFon.Margin = New System.Windows.Forms.Padding(2)
        Me.ComboBoxFon.Name = "ComboBoxFon"
        Me.ComboBoxFon.Size = New System.Drawing.Size(173, 21)
        Me.ComboBoxFon.TabIndex = 1
        '
        'FormWählclient
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(519, 256)
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Margin = New System.Windows.Forms.Padding(2)
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
    Friend WithEvents TBStatus As Windows.Forms.TextBox
    Friend WithEvents LStatus As Windows.Forms.Label
    Friend WithEvents CBoxDirektwahl As Windows.Forms.ComboBox
End Class
