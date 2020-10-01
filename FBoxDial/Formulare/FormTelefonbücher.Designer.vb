<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormTelefonbücher
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.MainSplitContainerV = New System.Windows.Forms.SplitContainer()
        Me.TLPBücher = New System.Windows.Forms.TableLayoutPanel()
        Me.BSpeichern = New System.Windows.Forms.Button()
        Me.BAdd = New System.Windows.Forms.Button()
        Me.BRemove = New System.Windows.Forms.Button()
        Me.SubSplitContainer = New System.Windows.Forms.SplitContainer()
        Me.DetailSplitContainer = New System.Windows.Forms.SplitContainer()
        Me.TBName = New System.Windows.Forms.TextBox()
        Me.LCTelefonbücher = New FBoxDial.TelBuchListControl()
        Me.DGVTelBuchEinträge = New FBoxDial.FBoxDataGridView()
        Me.DGVTelefonnummern = New FBoxDial.FBoxDataGridView()
        Me.DGVEMail = New FBoxDial.FBoxDataGridView()
        Me.Detail2SplitContainer = New System.Windows.Forms.SplitContainer()
        CType(Me.MainSplitContainerV, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MainSplitContainerV.Panel1.SuspendLayout()
        Me.MainSplitContainerV.Panel2.SuspendLayout()
        Me.MainSplitContainerV.SuspendLayout()
        Me.TLPBücher.SuspendLayout()
        CType(Me.SubSplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SubSplitContainer.Panel1.SuspendLayout()
        Me.SubSplitContainer.Panel2.SuspendLayout()
        Me.SubSplitContainer.SuspendLayout()
        CType(Me.DetailSplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.DetailSplitContainer.Panel1.SuspendLayout()
        Me.DetailSplitContainer.Panel2.SuspendLayout()
        Me.DetailSplitContainer.SuspendLayout()
        CType(Me.DGVTelBuchEinträge, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DGVTelefonnummern, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DGVEMail, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Detail2SplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Detail2SplitContainer.Panel1.SuspendLayout()
        Me.Detail2SplitContainer.Panel2.SuspendLayout()
        Me.Detail2SplitContainer.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 1044)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1555, 22)
        Me.StatusStrip1.TabIndex = 0
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.GripMargin = New System.Windows.Forms.Padding(2, 2, 0, 2)
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(6, 1, 0, 1)
        Me.MenuStrip1.Size = New System.Drawing.Size(1555, 36)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'MainSplitContainerV
        '
        Me.MainSplitContainerV.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MainSplitContainerV.Location = New System.Drawing.Point(0, 36)
        Me.MainSplitContainerV.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.MainSplitContainerV.Name = "MainSplitContainerV"
        '
        'MainSplitContainerV.Panel1
        '
        Me.MainSplitContainerV.Panel1.Controls.Add(Me.TLPBücher)
        Me.MainSplitContainerV.Panel1.Margin = New System.Windows.Forms.Padding(0, 6, 0, 6)
        Me.MainSplitContainerV.Panel1.Padding = New System.Windows.Forms.Padding(0, 0, 0, 6)
        Me.MainSplitContainerV.Panel1MinSize = 200
        '
        'MainSplitContainerV.Panel2
        '
        Me.MainSplitContainerV.Panel2.Controls.Add(Me.SubSplitContainer)
        Me.MainSplitContainerV.Size = New System.Drawing.Size(1555, 1008)
        Me.MainSplitContainerV.SplitterDistance = 445
        Me.MainSplitContainerV.TabIndex = 2
        '
        'TLPBücher
        '
        Me.TLPBücher.ColumnCount = 3
        Me.TLPBücher.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TLPBücher.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TLPBücher.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TLPBücher.Controls.Add(Me.BSpeichern, 2, 1)
        Me.TLPBücher.Controls.Add(Me.BAdd, 0, 1)
        Me.TLPBücher.Controls.Add(Me.LCTelefonbücher, 0, 0)
        Me.TLPBücher.Controls.Add(Me.BRemove, 1, 1)
        Me.TLPBücher.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TLPBücher.Location = New System.Drawing.Point(0, 0)
        Me.TLPBücher.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.TLPBücher.Name = "TLPBücher"
        Me.TLPBücher.RowCount = 2
        Me.TLPBücher.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TLPBücher.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TLPBücher.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50.0!))
        Me.TLPBücher.Size = New System.Drawing.Size(445, 1002)
        Me.TLPBücher.TabIndex = 3
        '
        'BSpeichern
        '
        Me.BSpeichern.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BSpeichern.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.BSpeichern.Image = Global.FBoxDial.My.Resources.Resources.Save
        Me.BSpeichern.Location = New System.Drawing.Point(299, 946)
        Me.BSpeichern.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.BSpeichern.MinimumSize = New System.Drawing.Size(47, 52)
        Me.BSpeichern.Name = "BSpeichern"
        Me.BSpeichern.Size = New System.Drawing.Size(143, 52)
        Me.BSpeichern.TabIndex = 3
        Me.BSpeichern.UseVisualStyleBackColor = True
        '
        'BAdd
        '
        Me.BAdd.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BAdd.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.BAdd.Image = Global.FBoxDial.My.Resources.Resources.Add
        Me.BAdd.Location = New System.Drawing.Point(3, 946)
        Me.BAdd.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.BAdd.MinimumSize = New System.Drawing.Size(47, 52)
        Me.BAdd.Name = "BAdd"
        Me.BAdd.Size = New System.Drawing.Size(142, 52)
        Me.BAdd.TabIndex = 1
        Me.BAdd.UseVisualStyleBackColor = True
        '
        'BRemove
        '
        Me.BRemove.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BRemove.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.BRemove.Image = Global.FBoxDial.My.Resources.Resources.Remove
        Me.BRemove.Location = New System.Drawing.Point(151, 946)
        Me.BRemove.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.BRemove.MinimumSize = New System.Drawing.Size(47, 52)
        Me.BRemove.Name = "BRemove"
        Me.BRemove.Size = New System.Drawing.Size(142, 52)
        Me.BRemove.TabIndex = 2
        Me.BRemove.UseVisualStyleBackColor = True
        '
        'SubSplitContainer
        '
        Me.SubSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SubSplitContainer.Location = New System.Drawing.Point(0, 0)
        Me.SubSplitContainer.Margin = New System.Windows.Forms.Padding(11, 12, 11, 12)
        Me.SubSplitContainer.Name = "SubSplitContainer"
        Me.SubSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SubSplitContainer.Panel1
        '
        Me.SubSplitContainer.Panel1.Controls.Add(Me.DGVTelBuchEinträge)
        Me.SubSplitContainer.Panel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.SubSplitContainer.Panel1.Padding = New System.Windows.Forms.Padding(0, 0, 11, 6)
        '
        'SubSplitContainer.Panel2
        '
        Me.SubSplitContainer.Panel2.Controls.Add(Me.DetailSplitContainer)
        Me.SubSplitContainer.Panel2.Padding = New System.Windows.Forms.Padding(0, 6, 11, 6)
        Me.SubSplitContainer.Size = New System.Drawing.Size(1106, 1008)
        Me.SubSplitContainer.SplitterDistance = 388
        Me.SubSplitContainer.SplitterWidth = 5
        Me.SubSplitContainer.TabIndex = 0
        '
        'DetailSplitContainer
        '
        Me.DetailSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DetailSplitContainer.Location = New System.Drawing.Point(0, 6)
        Me.DetailSplitContainer.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.DetailSplitContainer.Name = "DetailSplitContainer"
        Me.DetailSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'DetailSplitContainer.Panel1
        '
        Me.DetailSplitContainer.Panel1.Controls.Add(Me.Detail2SplitContainer)
        '
        'DetailSplitContainer.Panel2
        '
        Me.DetailSplitContainer.Panel2.Controls.Add(Me.DGVEMail)
        Me.DetailSplitContainer.Size = New System.Drawing.Size(1095, 603)
        Me.DetailSplitContainer.SplitterDistance = 262
        Me.DetailSplitContainer.SplitterWidth = 5
        Me.DetailSplitContainer.TabIndex = 3
        '
        'TBName
        '
        Me.TBName.Dock = System.Windows.Forms.DockStyle.Top
        Me.TBName.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TBName.Location = New System.Drawing.Point(0, 0)
        Me.TBName.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.TBName.Name = "TBName"
        Me.TBName.Size = New System.Drawing.Size(1095, 35)
        Me.TBName.TabIndex = 1
        '
        'LCTelefonbücher
        '
        Me.LCTelefonbücher.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TLPBücher.SetColumnSpan(Me.LCTelefonbücher, 3)
        Me.LCTelefonbücher.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LCTelefonbücher.Location = New System.Drawing.Point(1, 1)
        Me.LCTelefonbücher.Margin = New System.Windows.Forms.Padding(1)
        Me.LCTelefonbücher.Name = "LCTelefonbücher"
        Me.LCTelefonbücher.Size = New System.Drawing.Size(443, 940)
        Me.LCTelefonbücher.TabIndex = 0
        '
        'DGVTelBuchEinträge
        '
        Me.DGVTelBuchEinträge.AllowUserToAddRows = False
        Me.DGVTelBuchEinträge.AllowUserToDeleteRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.DGVTelBuchEinträge.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.DGVTelBuchEinträge.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DGVTelBuchEinträge.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVTelBuchEinträge.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVTelBuchEinträge.Location = New System.Drawing.Point(0, 0)
        Me.DGVTelBuchEinträge.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.DGVTelBuchEinträge.Name = "DGVTelBuchEinträge"
        Me.DGVTelBuchEinträge.RowHeadersVisible = False
        Me.DGVTelBuchEinträge.RowHeadersWidth = 62
        Me.DGVTelBuchEinträge.RowTemplate.DefaultCellStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.DGVTelBuchEinträge.RowTemplate.Height = 28
        Me.DGVTelBuchEinträge.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DGVTelBuchEinträge.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGVTelBuchEinträge.ShowEditingIcon = False
        Me.DGVTelBuchEinträge.Size = New System.Drawing.Size(1095, 382)
        Me.DGVTelBuchEinträge.TabIndex = 0
        '
        'DGVTelefonnummern
        '
        Me.DGVTelefonnummern.AllowUserToDeleteRows = False
        Me.DGVTelefonnummern.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DGVTelefonnummern.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVTelefonnummern.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVTelefonnummern.Location = New System.Drawing.Point(0, 0)
        Me.DGVTelefonnummern.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.DGVTelefonnummern.Name = "DGVTelefonnummern"
        Me.DGVTelefonnummern.RowHeadersVisible = False
        Me.DGVTelefonnummern.RowHeadersWidth = 62
        Me.DGVTelefonnummern.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DGVTelefonnummern.RowTemplate.DefaultCellStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.DGVTelefonnummern.RowTemplate.Height = 28
        Me.DGVTelefonnummern.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DGVTelefonnummern.ShowEditingIcon = False
        Me.DGVTelefonnummern.Size = New System.Drawing.Size(1095, 131)
        Me.DGVTelefonnummern.TabIndex = 0
        '
        'DGVEMail
        '
        Me.DGVEMail.AllowUserToDeleteRows = False
        Me.DGVEMail.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DGVEMail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVEMail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVEMail.Location = New System.Drawing.Point(0, 0)
        Me.DGVEMail.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.DGVEMail.Name = "DGVEMail"
        Me.DGVEMail.RowHeadersVisible = False
        Me.DGVEMail.RowHeadersWidth = 62
        Me.DGVEMail.RowTemplate.DefaultCellStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.DGVEMail.RowTemplate.Height = 28
        Me.DGVEMail.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DGVEMail.ShowEditingIcon = False
        Me.DGVEMail.Size = New System.Drawing.Size(1095, 336)
        Me.DGVEMail.TabIndex = 2
        '
        'Detail2SplitContainer
        '
        Me.Detail2SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Detail2SplitContainer.Location = New System.Drawing.Point(0, 0)
        Me.Detail2SplitContainer.Margin = New System.Windows.Forms.Padding(0)
        Me.Detail2SplitContainer.Name = "Detail2SplitContainer"
        Me.Detail2SplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'Detail2SplitContainer.Panel1
        '
        Me.Detail2SplitContainer.Panel1.Controls.Add(Me.TBName)
        '
        'Detail2SplitContainer.Panel2
        '
        Me.Detail2SplitContainer.Panel2.Controls.Add(Me.DGVTelefonnummern)
        Me.Detail2SplitContainer.Size = New System.Drawing.Size(1095, 262)
        Me.Detail2SplitContainer.SplitterDistance = 127
        Me.Detail2SplitContainer.TabIndex = 2
        '
        'FormTelefonbücher
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1555, 1066)
        Me.Controls.Add(Me.MainSplitContainerV)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.MinimumSize = New System.Drawing.Size(672, 486)
        Me.Name = "FormTelefonbücher"
        Me.ShowIcon = False
        Me.Text = "Fritz!Box Telefonbücher"
        Me.MainSplitContainerV.Panel1.ResumeLayout(False)
        Me.MainSplitContainerV.Panel2.ResumeLayout(False)
        CType(Me.MainSplitContainerV, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MainSplitContainerV.ResumeLayout(False)
        Me.TLPBücher.ResumeLayout(False)
        Me.SubSplitContainer.Panel1.ResumeLayout(False)
        Me.SubSplitContainer.Panel2.ResumeLayout(False)
        CType(Me.SubSplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SubSplitContainer.ResumeLayout(False)
        Me.DetailSplitContainer.Panel1.ResumeLayout(False)
        Me.DetailSplitContainer.Panel2.ResumeLayout(False)
        CType(Me.DetailSplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.DetailSplitContainer.ResumeLayout(False)
        CType(Me.DGVTelBuchEinträge, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DGVTelefonnummern, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DGVEMail, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Detail2SplitContainer.Panel1.ResumeLayout(False)
        Me.Detail2SplitContainer.Panel1.PerformLayout()
        Me.Detail2SplitContainer.Panel2.ResumeLayout(False)
        CType(Me.Detail2SplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Detail2SplitContainer.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents StatusStrip1 As Windows.Forms.StatusStrip
    Friend WithEvents MenuStrip1 As Windows.Forms.MenuStrip
    Friend WithEvents MainSplitContainerV As Windows.Forms.SplitContainer
    Friend WithEvents LCTelefonbücher As TelBuchListControl
    Friend WithEvents SubSplitContainer As Windows.Forms.SplitContainer
    Friend WithEvents DGVTelBuchEinträge As FBoxDataGridView
    Friend WithEvents DGVTelefonnummern As FBoxDataGridView
    Friend WithEvents TBName As Windows.Forms.TextBox
    Friend WithEvents DGVEMail As FBoxDataGridView
    Friend WithEvents DetailSplitContainer As Windows.Forms.SplitContainer
    Friend WithEvents BRemove As Windows.Forms.Button
    Friend WithEvents BAdd As Windows.Forms.Button
    Friend WithEvents TLPBücher As Windows.Forms.TableLayoutPanel
    Friend WithEvents BSpeichern As Windows.Forms.Button
    Friend WithEvents Detail2SplitContainer As Windows.Forms.SplitContainer
End Class
