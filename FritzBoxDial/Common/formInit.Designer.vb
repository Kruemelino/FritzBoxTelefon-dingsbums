<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formInit
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
        Me.PanelUserEingabe = New System.Windows.Forms.Panel()
        Me.LabelFBUser = New System.Windows.Forms.Label()
        Me.TBFBUser = New System.Windows.Forms.TextBox()
        Me.BSchließen = New System.Windows.Forms.Button()
        Me.BFertigstellen = New System.Windows.Forms.Button()
        Me.LTelListe = New System.Windows.Forms.Label()
        Me.CLBTelNr = New System.Windows.Forms.CheckedListBox()
        Me.BtELeINLESEN = New System.Windows.Forms.Button()
        Me.LMessage = New System.Windows.Forms.Label()
        Me.BFBPW = New System.Windows.Forms.Button()
        Me.BFBAdr = New System.Windows.Forms.Button()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.LLandesvorwahl = New System.Windows.Forms.Label()
        Me.TBLandesvorwahl = New System.Windows.Forms.TextBox()
        Me.LVorwahl = New System.Windows.Forms.Label()
        Me.TBVorwahl = New System.Windows.Forms.TextBox()
        Me.LFBPW = New System.Windows.Forms.Label()
        Me.TBFBPW = New System.Windows.Forms.TextBox()
        Me.LFBAdr = New System.Windows.Forms.Label()
        Me.TBFritzBoxAdr = New System.Windows.Forms.TextBox()
        Me.PanelUserEingabe.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelUserEingabe
        '
        Me.PanelUserEingabe.BackColor = System.Drawing.SystemColors.Control
        Me.PanelUserEingabe.Controls.Add(Me.LabelFBUser)
        Me.PanelUserEingabe.Controls.Add(Me.TBFBUser)
        Me.PanelUserEingabe.Controls.Add(Me.BSchließen)
        Me.PanelUserEingabe.Controls.Add(Me.BFertigstellen)
        Me.PanelUserEingabe.Controls.Add(Me.LTelListe)
        Me.PanelUserEingabe.Controls.Add(Me.CLBTelNr)
        Me.PanelUserEingabe.Controls.Add(Me.BtELeINLESEN)
        Me.PanelUserEingabe.Controls.Add(Me.LMessage)
        Me.PanelUserEingabe.Controls.Add(Me.BFBPW)
        Me.PanelUserEingabe.Controls.Add(Me.BFBAdr)
        Me.PanelUserEingabe.Controls.Add(Me.Label13)
        Me.PanelUserEingabe.Controls.Add(Me.LLandesvorwahl)
        Me.PanelUserEingabe.Controls.Add(Me.TBLandesvorwahl)
        Me.PanelUserEingabe.Controls.Add(Me.LVorwahl)
        Me.PanelUserEingabe.Controls.Add(Me.TBVorwahl)
        Me.PanelUserEingabe.Controls.Add(Me.LFBPW)
        Me.PanelUserEingabe.Controls.Add(Me.TBFBPW)
        Me.PanelUserEingabe.Controls.Add(Me.LFBAdr)
        Me.PanelUserEingabe.Controls.Add(Me.TBFritzBoxAdr)
        Me.PanelUserEingabe.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelUserEingabe.Location = New System.Drawing.Point(0, 0)
        Me.PanelUserEingabe.Name = "PanelUserEingabe"
        Me.PanelUserEingabe.Size = New System.Drawing.Size(366, 395)
        Me.PanelUserEingabe.TabIndex = 0
        '
        'LabelFBUser
        '
        Me.LabelFBUser.AutoSize = True
        Me.LabelFBUser.Enabled = False
        Me.LabelFBUser.Location = New System.Drawing.Point(123, 90)
        Me.LabelFBUser.Name = "LabelFBUser"
        Me.LabelFBUser.Size = New System.Drawing.Size(118, 13)
        Me.LabelFBUser.TabIndex = 36
        Me.LabelFBUser.Text = "Fritz!Box Benutzername"
        '
        'TBFBUser
        '
        Me.TBFBUser.Enabled = False
        Me.TBFBUser.Location = New System.Drawing.Point(16, 87)
        Me.TBFBUser.Name = "TBFBUser"
        Me.TBFBUser.Size = New System.Drawing.Size(100, 20)
        Me.TBFBUser.TabIndex = 35
        '
        'BSchließen
        '
        Me.BSchließen.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BSchließen.Enabled = False
        Me.BSchließen.Location = New System.Drawing.Point(279, 305)
        Me.BSchließen.Name = "BSchließen"
        Me.BSchließen.Size = New System.Drawing.Size(75, 20)
        Me.BSchließen.TabIndex = 34
        Me.BSchließen.Text = "Schließen"
        Me.BSchließen.UseVisualStyleBackColor = True
        '
        'BFertigstellen
        '
        Me.BFertigstellen.Enabled = False
        Me.BFertigstellen.Location = New System.Drawing.Point(279, 279)
        Me.BFertigstellen.Name = "BFertigstellen"
        Me.BFertigstellen.Size = New System.Drawing.Size(75, 20)
        Me.BFertigstellen.TabIndex = 33
        Me.BFertigstellen.Text = "Fertigstellen"
        Me.BFertigstellen.UseVisualStyleBackColor = True
        '
        'LTelListe
        '
        Me.LTelListe.Enabled = False
        Me.LTelListe.Location = New System.Drawing.Point(122, 216)
        Me.LTelListe.Name = "LTelListe"
        Me.LTelListe.Size = New System.Drawing.Size(151, 86)
        Me.LTelListe.TabIndex = 32
        Me.LTelListe.Text = "Wählen Sie mindestens eine Telefonnummer aus."
        '
        'CLBTelNr
        '
        Me.CLBTelNr.CheckOnClick = True
        Me.CLBTelNr.Enabled = False
        Me.CLBTelNr.FormattingEnabled = True
        Me.CLBTelNr.HorizontalScrollbar = True
        Me.CLBTelNr.Location = New System.Drawing.Point(16, 216)
        Me.CLBTelNr.Name = "CLBTelNr"
        Me.CLBTelNr.Size = New System.Drawing.Size(100, 109)
        Me.CLBTelNr.TabIndex = 31
        '
        'BtELeINLESEN
        '
        Me.BtELeINLESEN.Enabled = False
        Me.BtELeINLESEN.Location = New System.Drawing.Point(16, 190)
        Me.BtELeINLESEN.Name = "BtELeINLESEN"
        Me.BtELeINLESEN.Size = New System.Drawing.Size(343, 20)
        Me.BtELeINLESEN.TabIndex = 30
        Me.BtELeINLESEN.Text = "Telefone einlesen"
        Me.BtELeINLESEN.UseVisualStyleBackColor = True
        '
        'LMessage
        '
        Me.LMessage.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LMessage.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LMessage.Location = New System.Drawing.Point(12, 328)
        Me.LMessage.Name = "LMessage"
        Me.LMessage.Size = New System.Drawing.Size(347, 58)
        Me.LMessage.TabIndex = 29
        Me.LMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BFBPW
        '
        Me.BFBPW.Enabled = False
        Me.BFBPW.Location = New System.Drawing.Point(284, 112)
        Me.BFBPW.Name = "BFBPW"
        Me.BFBPW.Size = New System.Drawing.Size(75, 20)
        Me.BFBPW.TabIndex = 28
        Me.BFBPW.Text = "Prüfe"
        Me.BFBPW.UseVisualStyleBackColor = True
        '
        'BFBAdr
        '
        Me.BFBAdr.Location = New System.Drawing.Point(284, 60)
        Me.BFBAdr.Name = "BFBAdr"
        Me.BFBAdr.Size = New System.Drawing.Size(75, 20)
        Me.BFBAdr.TabIndex = 27
        Me.BFBAdr.Text = "Prüfe"
        Me.BFBAdr.UseVisualStyleBackColor = True
        '
        'Label13
        '
        Me.Label13.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(12, 9)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(347, 49)
        Me.Label13.TabIndex = 26
        Me.Label13.Text = "Geben Sie die benötigten Daten ein:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LLandesvorwahl
        '
        Me.LLandesvorwahl.AutoSize = True
        Me.LLandesvorwahl.Enabled = False
        Me.LLandesvorwahl.Location = New System.Drawing.Point(123, 167)
        Me.LLandesvorwahl.Name = "LLandesvorwahl"
        Me.LLandesvorwahl.Size = New System.Drawing.Size(79, 13)
        Me.LLandesvorwahl.TabIndex = 25
        Me.LLandesvorwahl.Text = "Landesvorwahl" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'TBLandesvorwahl
        '
        Me.TBLandesvorwahl.Enabled = False
        Me.TBLandesvorwahl.Location = New System.Drawing.Point(16, 164)
        Me.TBLandesvorwahl.Name = "TBLandesvorwahl"
        Me.TBLandesvorwahl.Size = New System.Drawing.Size(100, 20)
        Me.TBLandesvorwahl.TabIndex = 22
        '
        'LVorwahl
        '
        Me.LVorwahl.AutoSize = True
        Me.LVorwahl.Enabled = False
        Me.LVorwahl.Location = New System.Drawing.Point(123, 141)
        Me.LVorwahl.Name = "LVorwahl"
        Me.LVorwahl.Size = New System.Drawing.Size(81, 13)
        Me.LVorwahl.TabIndex = 24
        Me.LVorwahl.Text = "Eigene Vorwahl" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'TBVorwahl
        '
        Me.TBVorwahl.Enabled = False
        Me.TBVorwahl.Location = New System.Drawing.Point(16, 138)
        Me.TBVorwahl.Name = "TBVorwahl"
        Me.TBVorwahl.Size = New System.Drawing.Size(100, 20)
        Me.TBVorwahl.TabIndex = 21
        '
        'LFBPW
        '
        Me.LFBPW.AutoSize = True
        Me.LFBPW.Enabled = False
        Me.LFBPW.Location = New System.Drawing.Point(123, 116)
        Me.LFBPW.Name = "LFBPW"
        Me.LFBPW.Size = New System.Drawing.Size(93, 13)
        Me.LFBPW.TabIndex = 23
        Me.LFBPW.Text = "Fritz!Box Passwort"
        '
        'TBFBPW
        '
        Me.TBFBPW.Enabled = False
        Me.TBFBPW.Location = New System.Drawing.Point(16, 113)
        Me.TBFBPW.Name = "TBFBPW"
        Me.TBFBPW.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TBFBPW.Size = New System.Drawing.Size(100, 20)
        Me.TBFBPW.TabIndex = 19
        Me.TBFBPW.UseSystemPasswordChar = True
        '
        'LFBAdr
        '
        Me.LFBAdr.AutoSize = True
        Me.LFBAdr.Location = New System.Drawing.Point(123, 64)
        Me.LFBAdr.Name = "LFBAdr"
        Me.LFBAdr.Size = New System.Drawing.Size(88, 13)
        Me.LFBAdr.TabIndex = 20
        Me.LFBAdr.Text = "Fritz!Box Adresse"
        '
        'TBFritzBoxAdr
        '
        Me.TBFritzBoxAdr.Location = New System.Drawing.Point(16, 61)
        Me.TBFritzBoxAdr.Name = "TBFritzBoxAdr"
        Me.TBFritzBoxAdr.Size = New System.Drawing.Size(100, 20)
        Me.TBFritzBoxAdr.TabIndex = 18
        Me.TBFritzBoxAdr.Text = "fritz.box"
        '
        'formInit
        '
        Me.AcceptButton = Me.BFertigstellen
        Me.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BSchließen
        Me.ClientSize = New System.Drawing.Size(366, 395)
        Me.Controls.Add(Me.PanelUserEingabe)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(382, 429)
        Me.MinimumSize = New System.Drawing.Size(382, 429)
        Me.Name = "formInit"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "Initialisierung des Fritz!Box Telefon-Dingsbums"
        Me.PanelUserEingabe.ResumeLayout(False)
        Me.PanelUserEingabe.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelUserEingabe As System.Windows.Forms.Panel
    Friend WithEvents LLandesvorwahl As System.Windows.Forms.Label
    Friend WithEvents TBLandesvorwahl As System.Windows.Forms.TextBox
    Friend WithEvents LVorwahl As System.Windows.Forms.Label
    Friend WithEvents TBVorwahl As System.Windows.Forms.TextBox
    Friend WithEvents LFBPW As System.Windows.Forms.Label
    Friend WithEvents TBFBPW As System.Windows.Forms.TextBox
    Friend WithEvents LFBAdr As System.Windows.Forms.Label
    Friend WithEvents TBFritzBoxAdr As System.Windows.Forms.TextBox
    Friend WithEvents BFBPW As System.Windows.Forms.Button
    Friend WithEvents BFBAdr As System.Windows.Forms.Button
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents LMessage As System.Windows.Forms.Label
    Friend WithEvents BtELeINLESEN As System.Windows.Forms.Button
    Friend WithEvents CLBTelNr As System.Windows.Forms.CheckedListBox
    Friend WithEvents LTelListe As System.Windows.Forms.Label
    Friend WithEvents BFertigstellen As System.Windows.Forms.Button
    Friend WithEvents BSchließen As System.Windows.Forms.Button
    Friend WithEvents LabelFBUser As System.Windows.Forms.Label
    Friend WithEvents TBFBUser As System.Windows.Forms.TextBox
End Class
