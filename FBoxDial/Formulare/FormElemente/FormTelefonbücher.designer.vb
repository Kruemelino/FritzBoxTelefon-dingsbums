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
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.MainSplitContainerV = New System.Windows.Forms.SplitContainer()
        Me.LCTelefonbücher = New FBoxDial.TelBuchListControl()
        CType(Me.MainSplitContainerV, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MainSplitContainerV.Panel1.SuspendLayout()
        Me.MainSplitContainerV.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 522)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(878, 22)
        Me.StatusStrip1.TabIndex = 0
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.GripMargin = New System.Windows.Forms.Padding(2, 2, 0, 2)
        Me.MenuStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(878, 36)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'MainSplitContainerV
        '
        Me.MainSplitContainerV.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MainSplitContainerV.Location = New System.Drawing.Point(0, 36)
        Me.MainSplitContainerV.Name = "MainSplitContainerV"
        '
        'MainSplitContainerV.Panel1
        '
        Me.MainSplitContainerV.Panel1.Controls.Add(Me.LCTelefonbücher)
        Me.MainSplitContainerV.Size = New System.Drawing.Size(878, 486)
        Me.MainSplitContainerV.SplitterDistance = 254
        Me.MainSplitContainerV.TabIndex = 2
        '
        'LCTelefonbücher
        '
        Me.LCTelefonbücher.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LCTelefonbücher.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LCTelefonbücher.Location = New System.Drawing.Point(0, 0)
        Me.LCTelefonbücher.Name = "LCTelefonbücher"
        Me.LCTelefonbücher.Size = New System.Drawing.Size(254, 486)
        Me.LCTelefonbücher.TabIndex = 0
        '
        'FormTelefonbücher
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(878, 544)
        Me.Controls.Add(Me.MainSplitContainerV)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "FormTelefonbücher"
        Me.Text = "Fritz!Box Telefonbücher"
        Me.MainSplitContainerV.Panel1.ResumeLayout(False)
        CType(Me.MainSplitContainerV, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MainSplitContainerV.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents StatusStrip1 As Windows.Forms.StatusStrip
    Friend WithEvents MenuStrip1 As Windows.Forms.MenuStrip
    Friend WithEvents MainSplitContainerV As Windows.Forms.SplitContainer
    Friend WithEvents LCTelefonbücher As TelBuchListControl
End Class
