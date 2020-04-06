<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormAnrList
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
        Me.StartZeit = New System.Windows.Forms.DateTimePicker()
        Me.ButtonStart = New System.Windows.Forms.Button()
        Me.StartDatum = New System.Windows.Forms.DateTimePicker()
        Me.GBoxStartZeit = New System.Windows.Forms.GroupBox()
        Me.GBoxEndZeit = New System.Windows.Forms.GroupBox()
        Me.EndDatum = New System.Windows.Forms.DateTimePicker()
        Me.EndZeit = New System.Windows.Forms.DateTimePicker()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ProgressBarAnrListe = New System.Windows.Forms.ToolStripProgressBar()
        Me.DGVAnrListe = New FBoxDial.FBoxDataGridView()
        Me.GBoxStartZeit.SuspendLayout()
        Me.GBoxEndZeit.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        CType(Me.DGVAnrListe, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StartZeit
        '
        Me.StartZeit.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.StartZeit.Location = New System.Drawing.Point(154, 29)
        Me.StartZeit.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.StartZeit.Name = "StartZeit"
        Me.StartZeit.ShowUpDown = True
        Me.StartZeit.Size = New System.Drawing.Size(97, 26)
        Me.StartZeit.TabIndex = 3
        '
        'ButtonStart
        '
        Me.ButtonStart.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonStart.Enabled = False
        Me.ButtonStart.Location = New System.Drawing.Point(1000, 73)
        Me.ButtonStart.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButtonStart.Name = "ButtonStart"
        Me.ButtonStart.Size = New System.Drawing.Size(180, 40)
        Me.ButtonStart.TabIndex = 0
        Me.ButtonStart.Text = "Starte Journalimport"
        Me.ButtonStart.UseVisualStyleBackColor = True
        '
        'StartDatum
        '
        Me.StartDatum.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.StartDatum.Location = New System.Drawing.Point(9, 29)
        Me.StartDatum.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.StartDatum.Name = "StartDatum"
        Me.StartDatum.Size = New System.Drawing.Size(134, 26)
        Me.StartDatum.TabIndex = 2
        '
        'GBoxStartZeit
        '
        Me.GBoxStartZeit.Controls.Add(Me.StartDatum)
        Me.GBoxStartZeit.Controls.Add(Me.StartZeit)
        Me.GBoxStartZeit.Enabled = False
        Me.GBoxStartZeit.Location = New System.Drawing.Point(8, 14)
        Me.GBoxStartZeit.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBoxStartZeit.Name = "GBoxStartZeit"
        Me.GBoxStartZeit.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBoxStartZeit.Size = New System.Drawing.Size(264, 77)
        Me.GBoxStartZeit.TabIndex = 1
        Me.GBoxStartZeit.TabStop = False
        Me.GBoxStartZeit.Text = "Startzeit"
        '
        'GBoxEndZeit
        '
        Me.GBoxEndZeit.Controls.Add(Me.EndDatum)
        Me.GBoxEndZeit.Controls.Add(Me.EndZeit)
        Me.GBoxEndZeit.Enabled = False
        Me.GBoxEndZeit.Location = New System.Drawing.Point(8, 101)
        Me.GBoxEndZeit.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBoxEndZeit.Name = "GBoxEndZeit"
        Me.GBoxEndZeit.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GBoxEndZeit.Size = New System.Drawing.Size(264, 77)
        Me.GBoxEndZeit.TabIndex = 4
        Me.GBoxEndZeit.TabStop = False
        Me.GBoxEndZeit.Text = "Endzeit"
        '
        'EndDatum
        '
        Me.EndDatum.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.EndDatum.Location = New System.Drawing.Point(9, 29)
        Me.EndDatum.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.EndDatum.Name = "EndDatum"
        Me.EndDatum.Size = New System.Drawing.Size(134, 26)
        Me.EndDatum.TabIndex = 5
        '
        'EndZeit
        '
        Me.EndZeit.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.EndZeit.Location = New System.Drawing.Point(154, 29)
        Me.EndZeit.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.EndZeit.Name = "EndZeit"
        Me.EndZeit.ShowUpDown = True
        Me.EndZeit.Size = New System.Drawing.Size(97, 26)
        Me.EndZeit.TabIndex = 6
        '
        'ButtonCancel
        '
        Me.ButtonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonCancel.Enabled = False
        Me.ButtonCancel.Location = New System.Drawing.Point(1001, 123)
        Me.ButtonCancel.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(180, 40)
        Me.ButtonCancel.TabIndex = 9
        Me.ButtonCancel.Text = "Abbruch"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.ProgressBarAnrListe})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 712)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1190, 32)
        Me.StatusStrip1.TabIndex = 12
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(64, 25)
        Me.ToolStripStatusLabel1.Text = "Status:"
        '
        'ProgressBarAnrListe
        '
        Me.ProgressBarAnrListe.Name = "ProgressBarAnrListe"
        Me.ProgressBarAnrListe.Size = New System.Drawing.Size(400, 24)
        '
        'DGVAnrListe
        '
        Me.DGVAnrListe.AllowUserToAddRows = False
        Me.DGVAnrListe.AllowUserToDeleteRows = False
        Me.DGVAnrListe.AllowUserToResizeColumns = False
        Me.DGVAnrListe.AllowUserToResizeRows = False
        Me.DGVAnrListe.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DGVAnrListe.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DGVAnrListe.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DGVAnrListe.Enabled = False
        Me.DGVAnrListe.Location = New System.Drawing.Point(5, 186)
        Me.DGVAnrListe.Name = "DGVAnrListe"
        Me.DGVAnrListe.RowHeadersVisible = False
        Me.DGVAnrListe.RowHeadersWidth = 62
        Me.DGVAnrListe.RowTemplate.Height = 28
        Me.DGVAnrListe.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DGVAnrListe.ShowCellToolTips = False
        Me.DGVAnrListe.ShowEditingIcon = False
        Me.DGVAnrListe.Size = New System.Drawing.Size(1173, 523)
        Me.DGVAnrListe.TabIndex = 11
        '
        'FormAnrList
        '
        Me.AcceptButton = Me.ButtonStart
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonCancel
        Me.ClientSize = New System.Drawing.Size(1190, 744)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.DGVAnrListe)
        Me.Controls.Add(Me.GBoxEndZeit)
        Me.Controls.Add(Me.GBoxStartZeit)
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.ButtonStart)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "FormAnrList"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Import der Anrufliste"
        Me.GBoxStartZeit.ResumeLayout(False)
        Me.GBoxEndZeit.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        CType(Me.DGVAnrListe, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StartZeit As System.Windows.Forms.DateTimePicker
    Friend WithEvents ButtonStart As System.Windows.Forms.Button
    Friend WithEvents StartDatum As System.Windows.Forms.DateTimePicker
    Friend WithEvents GBoxStartZeit As System.Windows.Forms.GroupBox
    Friend WithEvents GBoxEndZeit As System.Windows.Forms.GroupBox
    Friend WithEvents EndDatum As System.Windows.Forms.DateTimePicker
    Friend WithEvents EndZeit As System.Windows.Forms.DateTimePicker
    Friend WithEvents ButtonCancel As System.Windows.Forms.Button
    Friend WithEvents DGVAnrListe As FBoxDataGridView
    Friend WithEvents StatusStrip1 As Windows.Forms.StatusStrip
    Friend WithEvents ProgressBarAnrListe As Windows.Forms.ToolStripProgressBar
    Friend WithEvents ToolStripStatusLabel1 As Windows.Forms.ToolStripStatusLabel
End Class
