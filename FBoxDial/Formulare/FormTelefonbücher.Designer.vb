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
        Me.LCTelefonbücher = New FBoxDial.TelBuchListControl()
        Me.SubSplitContainer = New System.Windows.Forms.SplitContainer()
        Me.DGVTelBuchEinträge = New FBoxDial.FBoxDataGridView()
        Me.DGVEMail = New FBoxDial.FBoxDataGridView()
        Me.DGVTelefonnummern = New FBoxDial.FBoxDataGridView()
        Me.TBName = New System.Windows.Forms.TextBox()
        CType(Me.MainSplitContainerV, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MainSplitContainerV.Panel1.SuspendLayout()
        Me.MainSplitContainerV.Panel2.SuspendLayout()
        Me.MainSplitContainerV.SuspendLayout()
        CType(Me.SubSplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SubSplitContainer.Panel1.SuspendLayout()
        Me.SubSplitContainer.Panel2.SuspendLayout()
        Me.SubSplitContainer.SuspendLayout()
        CType(Me.DGVTelBuchEinträge, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DGVEMail, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DGVTelefonnummern, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 574)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(954, 22)
        Me.StatusStrip1.TabIndex = 0
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.GripMargin = New System.Windows.Forms.Padding(2, 2, 0, 2)
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(954, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'MainSplitContainerV
        '
        Me.MainSplitContainerV.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MainSplitContainerV.Location = New System.Drawing.Point(0, 24)
        Me.MainSplitContainerV.Name = "MainSplitContainerV"
        '
        'MainSplitContainerV.Panel1
        '
        Me.MainSplitContainerV.Panel1.Controls.Add(Me.LCTelefonbücher)
        '
        'MainSplitContainerV.Panel2
        '
        Me.MainSplitContainerV.Panel2.Controls.Add(Me.SubSplitContainer)
        Me.MainSplitContainerV.Size = New System.Drawing.Size(954, 550)
        Me.MainSplitContainerV.SplitterDistance = 275
        Me.MainSplitContainerV.TabIndex = 2
        '
        'LCTelefonbücher
        '
        Me.LCTelefonbücher.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LCTelefonbücher.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LCTelefonbücher.Location = New System.Drawing.Point(0, 0)
        Me.LCTelefonbücher.Name = "LCTelefonbücher"
        Me.LCTelefonbücher.Size = New System.Drawing.Size(275, 550)
        Me.LCTelefonbücher.TabIndex = 0
        '
        'SubSplitContainer
        '
        Me.SubSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SubSplitContainer.Location = New System.Drawing.Point(0, 0)
        Me.SubSplitContainer.Name = "SubSplitContainer"
        Me.SubSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SubSplitContainer.Panel1
        '
        Me.SubSplitContainer.Panel1.Controls.Add(Me.DGVTelBuchEinträge)
        Me.SubSplitContainer.Panel1.Margin = New System.Windows.Forms.Padding(5)
        '
        'SubSplitContainer.Panel2
        '
        Me.SubSplitContainer.Panel2.Controls.Add(Me.DGVEMail)
        Me.SubSplitContainer.Panel2.Controls.Add(Me.DGVTelefonnummern)
        Me.SubSplitContainer.Panel2.Controls.Add(Me.TBName)
        Me.SubSplitContainer.Size = New System.Drawing.Size(675, 550)
        Me.SubSplitContainer.SplitterDistance = 215
        Me.SubSplitContainer.TabIndex = 0
        '
        'DGVTelBuchEinträge
        '
        Me.DGVTelBuchEinträge.AllowUserToAddRows = False
        Me.DGVTelBuchEinträge.AllowUserToDeleteRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.Gainsboro
        Me.DGVTelBuchEinträge.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.DGVTelBuchEinträge.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DGVTelBuchEinträge.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DGVTelBuchEinträge.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVTelBuchEinträge.Location = New System.Drawing.Point(0, 0)
        Me.DGVTelBuchEinträge.Name = "DGVTelBuchEinträge"
        Me.DGVTelBuchEinträge.RowHeadersVisible = False
        Me.DGVTelBuchEinträge.RowHeadersWidth = 62
        Me.DGVTelBuchEinträge.RowTemplate.DefaultCellStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.DGVTelBuchEinträge.RowTemplate.Height = 28
        Me.DGVTelBuchEinträge.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DGVTelBuchEinträge.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGVTelBuchEinträge.ShowEditingIcon = False
        Me.DGVTelBuchEinträge.Size = New System.Drawing.Size(663, 219)
        Me.DGVTelBuchEinträge.TabIndex = 0
        '
        'DGVEMail
        '
        Me.DGVEMail.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DGVEMail.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DGVEMail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVEMail.Location = New System.Drawing.Point(0, 181)
        Me.DGVEMail.Name = "DGVEMail"
        Me.DGVEMail.RowHeadersVisible = False
        Me.DGVEMail.RowHeadersWidth = 62
        Me.DGVEMail.RowTemplate.DefaultCellStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.DGVEMail.RowTemplate.Height = 28
        Me.DGVEMail.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DGVEMail.ShowEditingIcon = False
        Me.DGVEMail.Size = New System.Drawing.Size(663, 131)
        Me.DGVEMail.TabIndex = 2
        '
        'DGVTelefonnummern
        '
        Me.DGVTelefonnummern.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DGVTelefonnummern.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.DGVTelefonnummern.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVTelefonnummern.Location = New System.Drawing.Point(0, 44)
        Me.DGVTelefonnummern.Name = "DGVTelefonnummern"
        Me.DGVTelefonnummern.RowHeadersWidth = 62
        Me.DGVTelefonnummern.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DGVTelefonnummern.RowTemplate.DefaultCellStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.DGVTelefonnummern.RowTemplate.Height = 28
        Me.DGVTelefonnummern.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DGVTelefonnummern.ShowEditingIcon = False
        Me.DGVTelefonnummern.Size = New System.Drawing.Size(663, 131)
        Me.DGVTelefonnummern.TabIndex = 0
        '
        'TBName
        '
        Me.TBName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TBName.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TBName.Location = New System.Drawing.Point(0, 3)
        Me.TBName.Name = "TBName"
        Me.TBName.Size = New System.Drawing.Size(663, 35)
        Me.TBName.TabIndex = 1
        '
        'FormTelefonbücher
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(954, 596)
        Me.Controls.Add(Me.MainSplitContainerV)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "FormTelefonbücher"
        Me.Text = "Fritz!Box Telefonbücher"
        Me.MainSplitContainerV.Panel1.ResumeLayout(False)
        Me.MainSplitContainerV.Panel2.ResumeLayout(False)
        CType(Me.MainSplitContainerV, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MainSplitContainerV.ResumeLayout(False)
        Me.SubSplitContainer.Panel1.ResumeLayout(False)
        Me.SubSplitContainer.Panel2.ResumeLayout(False)
        Me.SubSplitContainer.Panel2.PerformLayout()
        CType(Me.SubSplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SubSplitContainer.ResumeLayout(False)
        CType(Me.DGVTelBuchEinträge, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DGVEMail, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DGVTelefonnummern, System.ComponentModel.ISupportInitialize).EndInit()
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
End Class
