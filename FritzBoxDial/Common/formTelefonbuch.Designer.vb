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
        Me.OFDAdressdbuch = New System.Windows.Forms.OpenFileDialog()
        Me.SFDTelefonbuch = New System.Windows.Forms.SaveFileDialog()
        Me.CMSTelefonbuch = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.TSMI_Add = New System.Windows.Forms.ToolStripMenuItem()
        Me.TSMI_Delete = New System.Windows.Forms.ToolStripMenuItem()
        Me.NeuToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ÖffnenToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.SpeichernToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ImportToolStrip = New System.Windows.Forms.ToolStripButton()
        Me.ExportToolStripButton = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.TSTelefonbuch = New System.Windows.Forms.ToolStrip()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.TSSLTelefonbuch = New System.Windows.Forms.ToolStripStatusLabel()
        Me.CMSTelefonbuch.SuspendLayout
        Me.TSTelefonbuch.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        Me.SuspendLayout
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
        'NeuToolStripButton
        '
        Me.NeuToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.NeuToolStripButton.Image = CType(resources.GetObject("NeuToolStripButton.Image"),System.Drawing.Image)
        Me.NeuToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.NeuToolStripButton.Name = "NeuToolStripButton"
        Me.NeuToolStripButton.Size = New System.Drawing.Size(33, 22)
        Me.NeuToolStripButton.Text = "&Neu"
        '
        'ÖffnenToolStripButton
        '
        Me.ÖffnenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ÖffnenToolStripButton.Image = CType(resources.GetObject("ÖffnenToolStripButton.Image"),System.Drawing.Image)
        Me.ÖffnenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ÖffnenToolStripButton.Name = "ÖffnenToolStripButton"
        Me.ÖffnenToolStripButton.Size = New System.Drawing.Size(48, 22)
        Me.ÖffnenToolStripButton.Text = "Ö&ffnen"
        '
        'SpeichernToolStripButton
        '
        Me.SpeichernToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.SpeichernToolStripButton.Image = CType(resources.GetObject("SpeichernToolStripButton.Image"),System.Drawing.Image)
        Me.SpeichernToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.SpeichernToolStripButton.Name = "SpeichernToolStripButton"
        Me.SpeichernToolStripButton.Size = New System.Drawing.Size(63, 22)
        Me.SpeichernToolStripButton.Text = "&Speichern"
        '
        'ImportToolStrip
        '
        Me.ImportToolStrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ImportToolStrip.Image = CType(resources.GetObject("ImportToolStrip.Image"),System.Drawing.Image)
        Me.ImportToolStrip.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ImportToolStrip.Name = "ImportToolStrip"
        Me.ImportToolStrip.Size = New System.Drawing.Size(47, 22)
        Me.ImportToolStrip.Text = "&Import"
        '
        'ExportToolStripButton
        '
        Me.ExportToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ExportToolStripButton.Image = CType(resources.GetObject("ExportToolStripButton.Image"),System.Drawing.Image)
        Me.ExportToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ExportToolStripButton.Name = "ExportToolStripButton"
        Me.ExportToolStripButton.Size = New System.Drawing.Size(44, 22)
        Me.ExportToolStripButton.Text = "&Export"
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"),System.Drawing.Image)
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton1.Text = "ToolStripButton1"
        '
        'TSTelefonbuch
        '
        Me.TSTelefonbuch.AllowDrop = true
        Me.TSTelefonbuch.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NeuToolStripButton, Me.ÖffnenToolStripButton, Me.SpeichernToolStripButton, Me.ImportToolStrip, Me.ExportToolStripButton, Me.ToolStripButton1})
        Me.TSTelefonbuch.Location = New System.Drawing.Point(0, 0)
        Me.TSTelefonbuch.Name = "TSTelefonbuch"
        Me.TSTelefonbuch.Size = New System.Drawing.Size(784, 25)
        Me.TSTelefonbuch.TabIndex = 1
        Me.TSTelefonbuch.Text = "TSTelefonbuch"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TSSLTelefonbuch})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 539)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(784, 22)
        Me.StatusStrip1.TabIndex = 7
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'TSSLTelefonbuch
        '
        Me.TSSLTelefonbuch.Name = "TSSLTelefonbuch"
        Me.TSSLTelefonbuch.Size = New System.Drawing.Size(121, 17)
        Me.TSSLTelefonbuch.Text = "ToolStripStatusLabel1"
        '
        'formTelefonbuch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 561)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.TSTelefonbuch)
        Me.IsMdiContainer = True
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "formTelefonbuch"
        Me.ShowIcon = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.Text = "Fritz!Box Telefonbücher"
        Me.TopMost = True
        Me.CMSTelefonbuch.ResumeLayout(False)
        Me.TSTelefonbuch.ResumeLayout(false)
        Me.TSTelefonbuch.PerformLayout
        Me.StatusStrip1.ResumeLayout(false)
        Me.StatusStrip1.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents OFDAdressdbuch As System.Windows.Forms.OpenFileDialog
    Friend WithEvents SFDTelefonbuch As System.Windows.Forms.SaveFileDialog
    Friend WithEvents CMSTelefonbuch As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents TSMI_Delete As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TSMI_Add As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NeuToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents ÖffnenToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents SpeichernToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents ImportToolStrip As System.Windows.Forms.ToolStripButton
    Friend WithEvents ExportToolStripButton As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripButton1 As System.Windows.Forms.ToolStripButton
    Friend WithEvents TSTelefonbuch As System.Windows.Forms.ToolStrip
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents TSSLTelefonbuch As System.Windows.Forms.ToolStripStatusLabel
End Class
