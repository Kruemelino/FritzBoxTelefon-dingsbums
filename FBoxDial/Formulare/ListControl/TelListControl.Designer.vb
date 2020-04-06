<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TelBuchListControl
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
        Me.flpListBox = New System.Windows.Forms.FlowLayoutPanel()
        Me.SuspendLayout()
        '
        'flpListBox
        '
        Me.flpListBox.AutoScroll = True
        Me.flpListBox.AutoSize = True
        Me.flpListBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flpListBox.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.flpListBox.Location = New System.Drawing.Point(0, 0)
        Me.flpListBox.Margin = New System.Windows.Forms.Padding(0)
        Me.flpListBox.Name = "flpListBox"
        Me.flpListBox.Size = New System.Drawing.Size(548, 250)
        Me.flpListBox.TabIndex = 0
        Me.flpListBox.WrapContents = False
        '
        'ListControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.flpListBox)
        Me.Name = "ListControl"
        Me.Size = New System.Drawing.Size(548, 250)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents flpListBox As Windows.Forms.FlowLayoutPanel
End Class
