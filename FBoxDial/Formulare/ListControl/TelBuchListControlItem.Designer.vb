<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TelBuchListControlItem
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
        Me.components = New System.ComponentModel.Container()
        Me.CMSTelefonbücher = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.TSMAddTelBook = New System.Windows.Forms.ToolStripMenuItem()
        Me.TSMRemoveTelBook = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.TSMDownloadTelBook = New System.Windows.Forms.ToolStripMenuItem()
        Me.TSMUploadTelBook = New System.Windows.Forms.ToolStripMenuItem()
        Me.CMSTelefonbücher.SuspendLayout()
        Me.SuspendLayout()
        '
        'CMSTelefonbücher
        '
        Me.CMSTelefonbücher.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.CMSTelefonbücher.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TSMAddTelBook, Me.TSMRemoveTelBook, Me.ToolStripSeparator1, Me.TSMDownloadTelBook, Me.TSMUploadTelBook})
        Me.CMSTelefonbücher.Name = "CMSTelefonbücher"
        Me.CMSTelefonbücher.Size = New System.Drawing.Size(495, 171)
        '
        'TSMAddTelBook
        '
        Me.TSMAddTelBook.Image = Global.FBoxDial.My.Resources.Resources.Add
        Me.TSMAddTelBook.Name = "TSMAddTelBook"
        Me.TSMAddTelBook.Size = New System.Drawing.Size(494, 32)
        Me.TSMAddTelBook.Text = "Neues Telefonbuch auf der FritzBox Erstellen"
        '
        'TSMRemoveTelBook
        '
        Me.TSMRemoveTelBook.Image = Global.FBoxDial.My.Resources.Resources.Remove
        Me.TSMRemoveTelBook.Name = "TSMRemoveTelBook"
        Me.TSMRemoveTelBook.Size = New System.Drawing.Size(494, 32)
        Me.TSMRemoveTelBook.Text = "Telefonbuch von der Fritz!Box entfernen"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(491, 6)
        '
        'TSMDownloadTelBook
        '
        Me.TSMDownloadTelBook.Image = Global.FBoxDial.My.Resources.Resources.Download
        Me.TSMDownloadTelBook.Name = "TSMDownloadTelBook"
        Me.TSMDownloadTelBook.Size = New System.Drawing.Size(494, 32)
        Me.TSMDownloadTelBook.Text = "Telefonbuch erneut von der Fritz!Box herunterladen"
        '
        'TSMUploadTelBook
        '
        Me.TSMUploadTelBook.Image = Global.FBoxDial.My.Resources.Resources.Upload
        Me.TSMUploadTelBook.Name = "TSMUploadTelBook"
        Me.TSMUploadTelBook.Size = New System.Drawing.Size(494, 32)
        Me.TSMUploadTelBook.Text = "Telefonbuch zur Fritz!Box hochladen"
        '
        'TelBuchListControlItem
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Name = "TelBuchListControlItem"
        Me.Size = New System.Drawing.Size(150, 40)
        Me.CMSTelefonbücher.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents CMSTelefonbücher As Windows.Forms.ContextMenuStrip
    Friend WithEvents TSMAddTelBook As Windows.Forms.ToolStripMenuItem
    Friend WithEvents TSMRemoveTelBook As Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As Windows.Forms.ToolStripSeparator
    Friend WithEvents TSMDownloadTelBook As Windows.Forms.ToolStripMenuItem
    Friend WithEvents TSMUploadTelBook As Windows.Forms.ToolStripMenuItem
End Class
