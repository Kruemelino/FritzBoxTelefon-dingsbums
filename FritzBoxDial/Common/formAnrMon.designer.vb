<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formAnrMon
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItemKontaktöffnen = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemRückruf = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemKopieren = New System.Windows.Forms.ToolStripMenuItem()
        Me.PopUpAnrMon = New FritzBoxDial.PopUpAnrMon()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemKontaktöffnen, Me.ToolStripMenuItemRückruf, Me.ToolStripMenuItemKopieren})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(222, 70)
        '
        'ToolStripMenuItemKontaktöffnen
        '
        Me.ToolStripMenuItemKontaktöffnen.Image = Global.FritzBoxDial.My.Resources.Bild4
        Me.ToolStripMenuItemKontaktöffnen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripMenuItemKontaktöffnen.Name = "ToolStripMenuItemKontaktöffnen"
        Me.ToolStripMenuItemKontaktöffnen.Size = New System.Drawing.Size(221, 22)
        Me.ToolStripMenuItemKontaktöffnen.Text = "Kontakt öffnen"
        '
        'ToolStripMenuItemRückruf
        '
        Me.ToolStripMenuItemRückruf.Image = Global.FritzBoxDial.My.Resources.Bild2
        Me.ToolStripMenuItemRückruf.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripMenuItemRückruf.Name = "ToolStripMenuItemRückruf"
        Me.ToolStripMenuItemRückruf.Size = New System.Drawing.Size(221, 22)
        Me.ToolStripMenuItemRückruf.Text = "Rückruf"
        '
        'ToolStripMenuItemKopieren
        '
        Me.ToolStripMenuItemKopieren.Image = Global.FritzBoxDial.My.Resources.Bild5
        Me.ToolStripMenuItemKopieren.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
        Me.ToolStripMenuItemKopieren.Name = "ToolStripMenuItemKopieren"
        Me.ToolStripMenuItemKopieren.Size = New System.Drawing.Size(221, 22)
        Me.ToolStripMenuItemKopieren.Text = "In Zwischenablage kopieren"
        '
        'PopUpAnrMon
        '
        Me.PopUpAnrMon.AnrName = "Anrufername"
        Me.PopUpAnrMon.AutoAusblenden = False
        Me.PopUpAnrMon.BorderColor = System.Drawing.SystemColors.WindowText
        Me.PopUpAnrMon.ButtonHoverColor = System.Drawing.Color.Orange
        Me.PopUpAnrMon.ContentFont = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PopUpAnrMon.Firma = "Firmenname"
        Me.PopUpAnrMon.HeaderColor = System.Drawing.SystemColors.ControlDarkDark
        Me.PopUpAnrMon.Image = Nothing
        Me.PopUpAnrMon.ImagePosition = New System.Drawing.Point(12, 32)
        Me.PopUpAnrMon.ImageSize = New System.Drawing.Size(48, 48)
        Me.PopUpAnrMon.LinkHoverColor = System.Drawing.SystemColors.Highlight
        Me.PopUpAnrMon.OptionsButton = True
        Me.PopUpAnrMon.OptionsMenu = Me.ContextMenuStrip1
        Me.PopUpAnrMon.PositionsKorrektur = New System.Drawing.Size(0, 0)
        Me.PopUpAnrMon.Size = New System.Drawing.Size(400, 100)
        Me.PopUpAnrMon.TelName = "Telefonname"
        Me.PopUpAnrMon.TelNr = "01156 +49 (0815) 0123456789"
        Me.PopUpAnrMon.TelNrFont = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PopUpAnrMon.TextPadding = New System.Windows.Forms.Padding(5)
        Me.PopUpAnrMon.TitleColor = System.Drawing.SystemColors.ControlText
        Me.PopUpAnrMon.TitleFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PopUpAnrMon.Uhrzeit = "07.09.09 12:00:00"
        '
        'formAnrMon
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(155, 56)
        Me.Name = "formAnrMon"
        Me.Text = "Form1"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PopUpAnrMon As FritzBoxDial.PopUpAnrMon
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ToolStripMenuItemRückruf As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemKopieren As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemKontaktöffnen As System.Windows.Forms.ToolStripMenuItem

End Class
