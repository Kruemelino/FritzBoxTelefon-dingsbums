<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormMain
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
        Me.TBDiagnose = New System.Windows.Forms.TextBox()
        Me.BStart = New System.Windows.Forms.Button()
        Me.TBBenutzer = New System.Windows.Forms.TextBox()
        Me.TBPasswort = New System.Windows.Forms.TextBox()
        Me.TBVorwahl = New System.Windows.Forms.TextBox()
        Me.TBLandesVW = New System.Windows.Forms.TextBox()
        Me.LFBUser = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TBTelefonie = New System.Windows.Forms.RichTextBox()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TBDiagnose
        '
        Me.TBDiagnose.Location = New System.Drawing.Point(12, 116)
        Me.TBDiagnose.Multiline = True
        Me.TBDiagnose.Name = "TBDiagnose"
        Me.TBDiagnose.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TBDiagnose.Size = New System.Drawing.Size(235, 94)
        Me.TBDiagnose.TabIndex = 0
        '
        'BStart
        '
        Me.BStart.Location = New System.Drawing.Point(172, 216)
        Me.BStart.Name = "BStart"
        Me.BStart.Size = New System.Drawing.Size(75, 23)
        Me.BStart.TabIndex = 1
        Me.BStart.Text = "Start"
        Me.BStart.UseVisualStyleBackColor = True
        '
        'TBBenutzer
        '
        Me.TBBenutzer.Location = New System.Drawing.Point(147, 12)
        Me.TBBenutzer.Name = "TBBenutzer"
        Me.TBBenutzer.Size = New System.Drawing.Size(100, 20)
        Me.TBBenutzer.TabIndex = 2
        '
        'TBPasswort
        '
        Me.TBPasswort.Location = New System.Drawing.Point(147, 38)
        Me.TBPasswort.Name = "TBPasswort"
        Me.TBPasswort.Size = New System.Drawing.Size(100, 20)
        Me.TBPasswort.TabIndex = 3
        Me.TBPasswort.UseSystemPasswordChar = True
        '
        'TBVorwahl
        '
        Me.TBVorwahl.Location = New System.Drawing.Point(147, 64)
        Me.TBVorwahl.Name = "TBVorwahl"
        Me.TBVorwahl.Size = New System.Drawing.Size(100, 20)
        Me.TBVorwahl.TabIndex = 4
        '
        'TBLandesVW
        '
        Me.TBLandesVW.Location = New System.Drawing.Point(147, 90)
        Me.TBLandesVW.Name = "TBLandesVW"
        Me.TBLandesVW.Size = New System.Drawing.Size(100, 20)
        Me.TBLandesVW.TabIndex = 5
        '
        'LFBUser
        '
        Me.LFBUser.AutoSize = True
        Me.LFBUser.Location = New System.Drawing.Point(12, 15)
        Me.LFBUser.Name = "LFBUser"
        Me.LFBUser.Size = New System.Drawing.Size(121, 13)
        Me.LFBUser.TabIndex = 7
        Me.LFBUser.Text = "Fritz!Box Benutzername:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 41)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(93, 13)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Fritz!Box Passwort"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 67)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(63, 13)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Ortsvorwahl"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 93)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(79, 13)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "Landesvorwahl"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.TBTelefonie)
        Me.GroupBox1.Location = New System.Drawing.Point(253, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(280, 198)
        Me.GroupBox1.TabIndex = 11
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Telefeniegeräte"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(253, 216)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(97, 23)
        Me.Button1.TabIndex = 12
        Me.Button1.Text = "Herunterladen"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'TBTelefonie
        '
        Me.TBTelefonie.Location = New System.Drawing.Point(6, 19)
        Me.TBTelefonie.Name = "TBTelefonie"
        Me.TBTelefonie.Size = New System.Drawing.Size(268, 173)
        Me.TBTelefonie.TabIndex = 13
        Me.TBTelefonie.Text = ""
        '
        'FormMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(545, 252)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.LFBUser)
        Me.Controls.Add(Me.TBLandesVW)
        Me.Controls.Add(Me.TBVorwahl)
        Me.Controls.Add(Me.TBPasswort)
        Me.Controls.Add(Me.TBBenutzer)
        Me.Controls.Add(Me.BStart)
        Me.Controls.Add(Me.TBDiagnose)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "FormMain"
        Me.ShowIcon = False
        Me.Text = "Fritz!Box Telefon-dingsbums Diagnose"
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TBDiagnose As System.Windows.Forms.TextBox
    Friend WithEvents BStart As System.Windows.Forms.Button
    Friend WithEvents TBBenutzer As System.Windows.Forms.TextBox
    Friend WithEvents TBVorwahl As System.Windows.Forms.TextBox
    Friend WithEvents TBLandesVW As System.Windows.Forms.TextBox
    Friend WithEvents LFBUser As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TBPasswort As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents TBTelefonie As System.Windows.Forms.RichTextBox

End Class
