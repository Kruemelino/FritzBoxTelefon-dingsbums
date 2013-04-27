<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formIndizierung
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(formIndizierung))
        Me.ProgressBarIndex = New System.Windows.Forms.ProgressBar()
        Me.ButtonStart = New System.Windows.Forms.Button()
        Me.LabelAnzahl = New System.Windows.Forms.Label()
        Me.ButtonAbbrechen = New System.Windows.Forms.Button()
        Me.ButtonSchließen = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.CheckBoxIndexAutoStart = New System.Windows.Forms.CheckBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SuspendLayout()
        '
        'ProgressBarIndex
        '
        Me.ProgressBarIndex.Location = New System.Drawing.Point(12, 127)
        Me.ProgressBarIndex.Name = "ProgressBarIndex"
        Me.ProgressBarIndex.Size = New System.Drawing.Size(405, 23)
        Me.ProgressBarIndex.TabIndex = 0
        '
        'ButtonStart
        '
        Me.ButtonStart.Location = New System.Drawing.Point(12, 156)
        Me.ButtonStart.Name = "ButtonStart"
        Me.ButtonStart.Size = New System.Drawing.Size(131, 41)
        Me.ButtonStart.TabIndex = 1
        Me.ButtonStart.Text = "Indizierung Starten"
        Me.ButtonStart.UseVisualStyleBackColor = True
        '
        'LabelAnzahl
        '
        Me.LabelAnzahl.BackColor = System.Drawing.SystemColors.Control
        Me.LabelAnzahl.Location = New System.Drawing.Point(12, 109)
        Me.LabelAnzahl.Name = "LabelAnzahl"
        Me.LabelAnzahl.Size = New System.Drawing.Size(227, 13)
        Me.LabelAnzahl.TabIndex = 2
        Me.LabelAnzahl.Text = "Status: "
        '
        'ButtonAbbrechen
        '
        Me.ButtonAbbrechen.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonAbbrechen.Enabled = False
        Me.ButtonAbbrechen.Location = New System.Drawing.Point(149, 156)
        Me.ButtonAbbrechen.Name = "ButtonAbbrechen"
        Me.ButtonAbbrechen.Size = New System.Drawing.Size(131, 41)
        Me.ButtonAbbrechen.TabIndex = 1
        Me.ButtonAbbrechen.Text = "Indizierung Abbrechen"
        Me.ButtonAbbrechen.UseVisualStyleBackColor = True
        '
        'ButtonSchließen
        '
        Me.ButtonSchließen.Location = New System.Drawing.Point(286, 156)
        Me.ButtonSchließen.Name = "ButtonSchließen"
        Me.ButtonSchließen.Size = New System.Drawing.Size(131, 41)
        Me.ButtonSchließen.TabIndex = 1
        Me.ButtonSchließen.Text = "Indizierung Schließen"
        Me.ButtonSchließen.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(9, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(408, 96)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = resources.GetString("Label1.Text")
        '
        'CheckBoxIndexAutoStart
        '
        Me.CheckBoxIndexAutoStart.AutoSize = True
        Me.CheckBoxIndexAutoStart.Location = New System.Drawing.Point(245, 108)
        Me.CheckBoxIndexAutoStart.Name = "CheckBoxIndexAutoStart"
        Me.CheckBoxIndexAutoStart.Size = New System.Drawing.Size(172, 17)
        Me.CheckBoxIndexAutoStart.TabIndex = 4
        Me.CheckBoxIndexAutoStart.Text = "Indizierung automatisch starten"
        Me.ToolTip1.SetToolTip(Me.CheckBoxIndexAutoStart, "Startet die Kontaktindizierung beim nächsten Öffnen dieses Formulars automatisch." & _
                "")
        Me.CheckBoxIndexAutoStart.UseVisualStyleBackColor = True
        '
        'formIndizierung
        '
        Me.AcceptButton = Me.ButtonStart
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonAbbrechen
        Me.ClientSize = New System.Drawing.Size(427, 209)
        Me.ControlBox = False
        Me.Controls.Add(Me.CheckBoxIndexAutoStart)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.LabelAnzahl)
        Me.Controls.Add(Me.ButtonSchließen)
        Me.Controls.Add(Me.ButtonAbbrechen)
        Me.Controls.Add(Me.ButtonStart)
        Me.Controls.Add(Me.ProgressBarIndex)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "formIndizierung"
        Me.ShowIcon = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Kontaktindizierung"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ProgressBarIndex As System.Windows.Forms.ProgressBar
    Friend WithEvents ButtonStart As System.Windows.Forms.Button
    Friend WithEvents LabelAnzahl As System.Windows.Forms.Label
    Friend WithEvents ButtonAbbrechen As System.Windows.Forms.Button
    Friend WithEvents ButtonSchließen As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents CheckBoxIndexAutoStart As System.Windows.Forms.CheckBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
End Class
