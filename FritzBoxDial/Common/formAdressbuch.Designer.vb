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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(formAdressbuch))
        Me.StatStAdressbuch = New System.Windows.Forms.StatusStrip()
        Me.TSAdressbuch = New System.Windows.Forms.ToolStrip()
        Me.NeuToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ÖffnenToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.SpeichernToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ImportToolStrip = New System.Windows.Forms.ToolStripButton()
        Me.ExportToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.DGVAdressbuch = New System.Windows.Forms.DataGridView()
        Me.OFDAdressdbuch = New System.Windows.Forms.OpenFileDialog()
        Me.SFDAdressbuch = New System.Windows.Forms.SaveFileDialog()
        Me.TSAdressbuch.SuspendLayout()
        CType(Me.DGVAdressbuch, System.ComponentModel.ISupportInitialize).BeginInit()
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
        'DGVAdressbuch
        '
        Me.DGVAdressbuch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVAdressbuch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DGVAdressbuch.Location = New System.Drawing.Point(0, 25)
        Me.DGVAdressbuch.Name = "DGVAdressbuch"
        Me.DGVAdressbuch.Size = New System.Drawing.Size(784, 515)
        Me.DGVAdressbuch.TabIndex = 2
        '
        'OFDAdressdbuch
        '
        Me.OFDAdressdbuch.FileName = "OpenFileDialog1"
        '
        'formAdressbuch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.DGVAdressbuch)
        Me.Controls.Add(Me.TSAdressbuch)
        Me.Controls.Add(Me.StatStAdressbuch)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Name = "formAdressbuch"
        Me.ShowIcon = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.Text = "Adressbuch"
        Me.TSAdressbuch.ResumeLayout(False)
        Me.TSAdressbuch.PerformLayout()
        CType(Me.DGVAdressbuch, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatStAdressbuch As System.Windows.Forms.StatusStrip
    Friend WithEvents TSAdressbuch As System.Windows.Forms.ToolStrip
    Friend WithEvents ÖffnenToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents SpeichernToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents DGVAdressbuch As System.Windows.Forms.DataGridView
    Friend WithEvents OFDAdressdbuch As System.Windows.Forms.OpenFileDialog
    Friend WithEvents SFDAdressbuch As System.Windows.Forms.SaveFileDialog
    Friend WithEvents NeuToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents ImportToolStrip As System.Windows.Forms.ToolStripButton
    Friend WithEvents ExportToolStripButton As System.Windows.Forms.ToolStripButton
End Class
