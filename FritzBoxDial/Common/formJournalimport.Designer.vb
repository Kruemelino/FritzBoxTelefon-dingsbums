<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formJournalimport
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(formJournalimport))
        Me.StartZeit = New System.Windows.Forms.DateTimePicker()
        Me.ButtonStart = New System.Windows.Forms.Button()
        Me.StartDatum = New System.Windows.Forms.DateTimePicker()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.EndDatum = New System.Windows.Forms.DateTimePicker()
        Me.EndZeit = New System.Windows.Forms.DateTimePicker()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblBG1Percent = New System.Windows.Forms.Label()
        Me.BereichAuswertung = New System.Windows.Forms.GroupBox()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.ButtonSchließen = New System.Windows.Forms.Button()
        Me.ButtonHerunterladen = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.BereichAuswertung.SuspendLayout()
        Me.SuspendLayout()
        '
        'StartZeit
        '
        Me.StartZeit.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.StartZeit.Location = New System.Drawing.Point(103, 19)
        Me.StartZeit.Name = "StartZeit"
        Me.StartZeit.ShowUpDown = True
        Me.StartZeit.Size = New System.Drawing.Size(66, 20)
        Me.StartZeit.TabIndex = 3
        '
        'ButtonStart
        '
        Me.ButtonStart.Location = New System.Drawing.Point(5, 251)
        Me.ButtonStart.Name = "ButtonStart"
        Me.ButtonStart.Size = New System.Drawing.Size(120, 26)
        Me.ButtonStart.TabIndex = 0
        Me.ButtonStart.Text = "Starte Journalimport"
        Me.ButtonStart.UseVisualStyleBackColor = True
        '
        'StartDatum
        '
        Me.StartDatum.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.StartDatum.Location = New System.Drawing.Point(6, 19)
        Me.StartDatum.Name = "StartDatum"
        Me.StartDatum.Size = New System.Drawing.Size(91, 20)
        Me.StartDatum.TabIndex = 2
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.StartDatum)
        Me.GroupBox1.Controls.Add(Me.StartZeit)
        Me.GroupBox1.Location = New System.Drawing.Point(5, 75)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(176, 50)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Startzeit"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.EndDatum)
        Me.GroupBox2.Controls.Add(Me.EndZeit)
        Me.GroupBox2.Location = New System.Drawing.Point(213, 75)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(176, 50)
        Me.GroupBox2.TabIndex = 4
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Endzeit"
        '
        'EndDatum
        '
        Me.EndDatum.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.EndDatum.Location = New System.Drawing.Point(6, 19)
        Me.EndDatum.Name = "EndDatum"
        Me.EndDatum.Size = New System.Drawing.Size(91, 20)
        Me.EndDatum.TabIndex = 5
        '
        'EndZeit
        '
        Me.EndZeit.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.EndZeit.Location = New System.Drawing.Point(103, 19)
        Me.EndZeit.Name = "EndZeit"
        Me.EndZeit.ShowUpDown = True
        Me.EndZeit.Size = New System.Drawing.Size(66, 20)
        Me.EndZeit.TabIndex = 6
        '
        'ButtonCancel
        '
        Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonCancel.Location = New System.Drawing.Point(137, 251)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(120, 26)
        Me.ButtonCancel.TabIndex = 9
        Me.ButtonCancel.Text = "Abbruch"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(5, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(384, 63)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = resources.GetString("Label1.Text")
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(8, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(369, 31)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Die Anrufliste wurde bereits von der Fritz!Box heruntergeladen und wird nun ausge" & _
            "wertet.  Bitte Warten Sie bis der Vorgang abgeschlossen ist."
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(83, 47)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(121, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Bereits abgeschlossen:"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblBG1Percent
        '
        Me.lblBG1Percent.Location = New System.Drawing.Point(210, 47)
        Me.lblBG1Percent.Name = "lblBG1Percent"
        Me.lblBG1Percent.Size = New System.Drawing.Size(74, 13)
        Me.lblBG1Percent.TabIndex = 7
        Me.lblBG1Percent.Text = "-"
        '
        'BereichAuswertung
        '
        Me.BereichAuswertung.Controls.Add(Me.Label2)
        Me.BereichAuswertung.Controls.Add(Me.Label3)
        Me.BereichAuswertung.Controls.Add(Me.lblBG1Percent)
        Me.BereichAuswertung.Controls.Add(Me.ProgressBar1)
        Me.BereichAuswertung.Enabled = False
        Me.BereichAuswertung.Location = New System.Drawing.Point(5, 156)
        Me.BereichAuswertung.Name = "BereichAuswertung"
        Me.BereichAuswertung.Size = New System.Drawing.Size(384, 89)
        Me.BereichAuswertung.TabIndex = 5
        Me.BereichAuswertung.TabStop = False
        Me.BereichAuswertung.Text = "Auswertung der Anrufliste"
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(6, 63)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(371, 19)
        Me.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ProgressBar1.TabIndex = 6
        '
        'ButtonSchließen
        '
        Me.ButtonSchließen.Location = New System.Drawing.Point(269, 251)
        Me.ButtonSchließen.Name = "ButtonSchließen"
        Me.ButtonSchließen.Size = New System.Drawing.Size(120, 26)
        Me.ButtonSchließen.TabIndex = 10
        Me.ButtonSchließen.Text = "Schließen"
        Me.ButtonSchließen.UseVisualStyleBackColor = True
        '
        'ButtonHerunterladen
        '
        Me.ButtonHerunterladen.Enabled = False
        Me.ButtonHerunterladen.Location = New System.Drawing.Point(5, 131)
        Me.ButtonHerunterladen.Name = "ButtonHerunterladen"
        Me.ButtonHerunterladen.Size = New System.Drawing.Size(384, 19)
        Me.ButtonHerunterladen.TabIndex = 7
        Me.ButtonHerunterladen.Text = "Anrufliste erneut herunterladen"
        Me.ButtonHerunterladen.UseVisualStyleBackColor = True
        '
        'formJournalimport
        '
        Me.AcceptButton = Me.ButtonStart
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonCancel
        Me.ClientSize = New System.Drawing.Size(394, 286)
        Me.Controls.Add(Me.ButtonHerunterladen)
        Me.Controls.Add(Me.BereichAuswertung)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.ButtonSchließen)
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.ButtonStart)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "formJournalimport"
        Me.ShowIcon = False
        Me.Text = "Journalimport"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.BereichAuswertung.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents StartZeit As System.Windows.Forms.DateTimePicker
    Friend WithEvents ButtonStart As System.Windows.Forms.Button
    Friend WithEvents StartDatum As System.Windows.Forms.DateTimePicker
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents EndDatum As System.Windows.Forms.DateTimePicker
    Friend WithEvents EndZeit As System.Windows.Forms.DateTimePicker
    Friend WithEvents ButtonCancel As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblBG1Percent As System.Windows.Forms.Label
    Friend WithEvents BereichAuswertung As System.Windows.Forms.GroupBox
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents ButtonSchließen As System.Windows.Forms.Button
    Friend WithEvents ButtonHerunterladen As System.Windows.Forms.Button
End Class
