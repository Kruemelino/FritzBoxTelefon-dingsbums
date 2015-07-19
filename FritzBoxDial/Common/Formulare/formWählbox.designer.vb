<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formWählbox
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    '<System.Diagnostics.DebuggerNonUserCode()> _
    'Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    '    If disposing AndAlso components IsNot Nothing Then
    '        components.Dispose()
    '    End If
    '    MyBase.Dispose(disposing)
    'End Sub

    'Wird vom Windows Form-Designer benötigt.
    'Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.CloseButton = New System.Windows.Forms.Button()
        Me.ButtonZeigeKontakt = New System.Windows.Forms.Button()
        Me.ListTel = New System.Windows.Forms.DataGridView()
        Me.Nr = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Typ = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TelNr = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.checkNetz = New System.Windows.Forms.CheckBox()
        Me.checkCLIR = New System.Windows.Forms.CheckBox()
        Me.checkCBC = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.ComboBoxFon = New System.Windows.Forms.ComboBox()
        Me.Frame1 = New System.Windows.Forms.GroupBox()
        Me.LabelStatus = New System.Windows.Forms.Label()
        Me.cancelCallButton = New System.Windows.Forms.Button()
        Me.Gespraechsart = New System.Windows.Forms.Label()
        Me.GBoxDirektWahl = New System.Windows.Forms.GroupBox()
        Me.LabelCheckTest = New System.Windows.Forms.Label()
        Me.TelNrBox = New System.Windows.Forms.TextBox()
        Me.Titel = New System.Windows.Forms.Label()
        Me.ButtonWeiter = New System.Windows.Forms.Button()
        Me.ContactImage = New System.Windows.Forms.PictureBox()
        Me.BVIP = New System.Windows.Forms.CheckBox()
        Me.PDialDirekt = New System.Windows.Forms.Panel()
        Me.PDialNormal = New System.Windows.Forms.Panel()
        Me.PDialCbC = New System.Windows.Forms.Panel()
        Me.GBoxCbC = New System.Windows.Forms.GroupBox()
        Me.listCbCAnbieter = New System.Windows.Forms.DataGridView()
        Me.Nummer = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CentProMin = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Vorwahl = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Takt = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Tarif = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Bemerkung = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.LLBiligertelefonieren = New System.Windows.Forms.LinkLabel()
        Me.LCbCWarnung = New System.Windows.Forms.Label()
        CType(Me.ListTel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.Frame1.SuspendLayout()
        Me.GBoxDirektWahl.SuspendLayout()
        CType(Me.ContactImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PDialDirekt.SuspendLayout()
        Me.PDialNormal.SuspendLayout()
        Me.PDialCbC.SuspendLayout()
        Me.GBoxCbC.SuspendLayout()
        CType(Me.listCbCAnbieter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CloseButton
        '
        Me.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CloseButton.Location = New System.Drawing.Point(464, 189)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New System.Drawing.Size(75, 50)
        Me.CloseButton.TabIndex = 0
        Me.CloseButton.Text = "Schließen"
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'ButtonZeigeKontakt
        '
        Me.ButtonZeigeKontakt.Location = New System.Drawing.Point(464, 3)
        Me.ButtonZeigeKontakt.Name = "ButtonZeigeKontakt"
        Me.ButtonZeigeKontakt.Size = New System.Drawing.Size(75, 48)
        Me.ButtonZeigeKontakt.TabIndex = 2
        Me.ButtonZeigeKontakt.Text = "Zeige Kontakt"
        Me.ButtonZeigeKontakt.UseVisualStyleBackColor = True
        '
        'ListTel
        '
        Me.ListTel.AllowUserToAddRows = False
        Me.ListTel.AllowUserToDeleteRows = False
        Me.ListTel.AllowUserToResizeColumns = False
        Me.ListTel.AllowUserToResizeRows = False
        Me.ListTel.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable
        Me.ListTel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.ListTel.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Nr, Me.Typ, Me.TelNr})
        Me.ListTel.Enabled = False
        Me.ListTel.Location = New System.Drawing.Point(3, 3)
        Me.ListTel.MultiSelect = False
        Me.ListTel.Name = "ListTel"
        Me.ListTel.ReadOnly = True
        Me.ListTel.RowHeadersVisible = False
        Me.ListTel.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
        Me.ListTel.ShowEditingIcon = False
        Me.ListTel.Size = New System.Drawing.Size(454, 150)
        Me.ListTel.TabIndex = 3
        '
        'Nr
        '
        Me.Nr.HeaderText = "Nr."
        Me.Nr.MinimumWidth = 25
        Me.Nr.Name = "Nr"
        Me.Nr.ReadOnly = True
        Me.Nr.Width = 25
        '
        'Typ
        '
        Me.Typ.HeaderText = "Typ"
        Me.Typ.MinimumWidth = 180
        Me.Typ.Name = "Typ"
        Me.Typ.ReadOnly = True
        Me.Typ.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Typ.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Typ.ToolTipText = "Eintragstyp"
        Me.Typ.Width = 201
        '
        'TelNr
        '
        Me.TelNr.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.TelNr.HeaderText = "Telefonnummer"
        Me.TelNr.MinimumWidth = 180
        Me.TelNr.Name = "TelNr"
        Me.TelNr.ReadOnly = True
        Me.TelNr.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.TelNr.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.TelNr.ToolTipText = "Telefonnummer des Anzurufenden"
        '
        'checkNetz
        '
        Me.checkNetz.AutoSize = True
        Me.checkNetz.Location = New System.Drawing.Point(4, 160)
        Me.checkNetz.Name = "checkNetz"
        Me.checkNetz.Size = New System.Drawing.Size(134, 17)
        Me.checkNetz.TabIndex = 4
        Me.checkNetz.Text = "über Festnetz anrufen"
        Me.checkNetz.UseVisualStyleBackColor = True
        '
        'checkCLIR
        '
        Me.checkCLIR.AutoSize = True
        Me.checkCLIR.Location = New System.Drawing.Point(144, 159)
        Me.checkCLIR.Name = "checkCLIR"
        Me.checkCLIR.Size = New System.Drawing.Size(157, 17)
        Me.checkCLIR.TabIndex = 5
        Me.checkCLIR.Text = "Rufnummernunterdrückung"
        Me.checkCLIR.UseVisualStyleBackColor = True
        '
        'checkCBC
        '
        Me.checkCBC.AutoSize = True
        Me.checkCBC.Location = New System.Drawing.Point(307, 160)
        Me.checkCBC.Name = "checkCBC"
        Me.checkCBC.Size = New System.Drawing.Size(129, 17)
        Me.checkCBC.TabIndex = 6
        Me.checkCBC.Text = "Call-by-Call (einmalig)"
        Me.checkCBC.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.ComboBoxFon)
        Me.GroupBox1.ForeColor = System.Drawing.SystemColors.MenuText
        Me.GroupBox1.Location = New System.Drawing.Point(4, 183)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(190, 55)
        Me.GroupBox1.TabIndex = 7
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Verbinden über"
        '
        'ComboBoxFon
        '
        Me.ComboBoxFon.FormattingEnabled = True
        Me.ComboBoxFon.Location = New System.Drawing.Point(7, 21)
        Me.ComboBoxFon.Name = "ComboBoxFon"
        Me.ComboBoxFon.Size = New System.Drawing.Size(177, 21)
        Me.ComboBoxFon.TabIndex = 0
        '
        'Frame1
        '
        Me.Frame1.Controls.Add(Me.LabelStatus)
        Me.Frame1.Controls.Add(Me.cancelCallButton)
        Me.Frame1.Location = New System.Drawing.Point(200, 183)
        Me.Frame1.Name = "Frame1"
        Me.Frame1.Size = New System.Drawing.Size(257, 55)
        Me.Frame1.TabIndex = 8
        Me.Frame1.TabStop = False
        Me.Frame1.Text = "Status"
        '
        'LabelStatus
        '
        Me.LabelStatus.AutoSize = True
        Me.LabelStatus.ForeColor = System.Drawing.Color.Red
        Me.LabelStatus.Location = New System.Drawing.Point(88, 14)
        Me.LabelStatus.MaximumSize = New System.Drawing.Size(160, 0)
        Me.LabelStatus.MinimumSize = New System.Drawing.Size(160, 33)
        Me.LabelStatus.Name = "LabelStatus"
        Me.LabelStatus.Size = New System.Drawing.Size(160, 33)
        Me.LabelStatus.TabIndex = 1
        Me.LabelStatus.Text = "Bitte warten..."
        Me.LabelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cancelCallButton
        '
        Me.cancelCallButton.Location = New System.Drawing.Point(7, 21)
        Me.cancelCallButton.Name = "cancelCallButton"
        Me.cancelCallButton.Size = New System.Drawing.Size(75, 23)
        Me.cancelCallButton.TabIndex = 0
        Me.cancelCallButton.Text = "Abbruch"
        Me.cancelCallButton.UseVisualStyleBackColor = True
        Me.cancelCallButton.Visible = False
        '
        'Gespraechsart
        '
        Me.Gespraechsart.AutoSize = True
        Me.Gespraechsart.Cursor = System.Windows.Forms.Cursors.Default
        Me.Gespraechsart.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Gespraechsart.ForeColor = System.Drawing.Color.Green
        Me.Gespraechsart.ImageAlign = System.Drawing.ContentAlignment.TopLeft
        Me.Gespraechsart.Location = New System.Drawing.Point(356, 150)
        Me.Gespraechsart.MinimumSize = New System.Drawing.Size(10, 13)
        Me.Gespraechsart.Name = "Gespraechsart"
        Me.Gespraechsart.Size = New System.Drawing.Size(38, 13)
        Me.Gespraechsart.TabIndex = 15
        Me.Gespraechsart.Text = "Zone:"
        '
        'GBoxDirektWahl
        '
        Me.GBoxDirektWahl.Controls.Add(Me.LabelCheckTest)
        Me.GBoxDirektWahl.Controls.Add(Me.TelNrBox)
        Me.GBoxDirektWahl.Controls.Add(Me.Titel)
        Me.GBoxDirektWahl.Location = New System.Drawing.Point(3, 3)
        Me.GBoxDirektWahl.Name = "GBoxDirektWahl"
        Me.GBoxDirektWahl.Size = New System.Drawing.Size(535, 104)
        Me.GBoxDirektWahl.TabIndex = 1
        Me.GBoxDirektWahl.TabStop = False
        Me.GBoxDirektWahl.Text = "Direktwahl"
        '
        'LabelCheckTest
        '
        Me.LabelCheckTest.AutoSize = True
        Me.LabelCheckTest.Location = New System.Drawing.Point(3, 82)
        Me.LabelCheckTest.Name = "LabelCheckTest"
        Me.LabelCheckTest.Size = New System.Drawing.Size(170, 13)
        Me.LabelCheckTest.TabIndex = 3
        Me.LabelCheckTest.Text = "Diese Telefonnumer wird gewählt:"
        '
        'TelNrBox
        '
        Me.TelNrBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TelNrBox.Font = New System.Drawing.Font("Tahoma", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TelNrBox.Location = New System.Drawing.Point(3, 43)
        Me.TelNrBox.Name = "TelNrBox"
        Me.TelNrBox.Size = New System.Drawing.Size(528, 33)
        Me.TelNrBox.TabIndex = 1
        Me.TelNrBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Titel
        '
        Me.Titel.AutoSize = True
        Me.Titel.Location = New System.Drawing.Point(3, 18)
        Me.Titel.Name = "Titel"
        Me.Titel.Size = New System.Drawing.Size(472, 13)
        Me.Titel.TabIndex = 0
        Me.Titel.Text = "Geben Sie die zu wählende Telefonnummer ein. Beim Klick auf ""Weiter"" wird der Anr" & _
    "uf aufgebaut."
        '
        'ButtonWeiter
        '
        Me.ButtonWeiter.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.ButtonWeiter.Enabled = False
        Me.ButtonWeiter.Location = New System.Drawing.Point(464, 113)
        Me.ButtonWeiter.Name = "ButtonWeiter"
        Me.ButtonWeiter.Size = New System.Drawing.Size(75, 50)
        Me.ButtonWeiter.TabIndex = 2
        Me.ButtonWeiter.Text = "Wählen"
        Me.ButtonWeiter.UseVisualStyleBackColor = True
        '
        'ContactImage
        '
        Me.ContactImage.Location = New System.Drawing.Point(465, 57)
        Me.ContactImage.Name = "ContactImage"
        Me.ContactImage.Size = New System.Drawing.Size(73, 96)
        Me.ContactImage.TabIndex = 17
        Me.ContactImage.TabStop = False
        '
        'BVIP
        '
        Me.BVIP.Appearance = System.Windows.Forms.Appearance.Button
        Me.BVIP.Enabled = False
        Me.BVIP.Location = New System.Drawing.Point(464, 156)
        Me.BVIP.Name = "BVIP"
        Me.BVIP.Size = New System.Drawing.Size(75, 23)
        Me.BVIP.TabIndex = 20
        Me.BVIP.Text = "VIP"
        Me.BVIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.BVIP.UseVisualStyleBackColor = True
        '
        'PDialDirekt
        '
        Me.PDialDirekt.Controls.Add(Me.GBoxDirektWahl)
        Me.PDialDirekt.Controls.Add(Me.ButtonWeiter)
        Me.PDialDirekt.Location = New System.Drawing.Point(5, 532)
        Me.PDialDirekt.Name = "PDialDirekt"
        Me.PDialDirekt.Size = New System.Drawing.Size(542, 166)
        Me.PDialDirekt.TabIndex = 21
        '
        'PDialNormal
        '
        Me.PDialNormal.Controls.Add(Me.ListTel)
        Me.PDialNormal.Controls.Add(Me.CloseButton)
        Me.PDialNormal.Controls.Add(Me.BVIP)
        Me.PDialNormal.Controls.Add(Me.ButtonZeigeKontakt)
        Me.PDialNormal.Controls.Add(Me.ContactImage)
        Me.PDialNormal.Controls.Add(Me.checkNetz)
        Me.PDialNormal.Controls.Add(Me.checkCLIR)
        Me.PDialNormal.Controls.Add(Me.checkCBC)
        Me.PDialNormal.Controls.Add(Me.GroupBox1)
        Me.PDialNormal.Controls.Add(Me.Frame1)
        Me.PDialNormal.Location = New System.Drawing.Point(5, 5)
        Me.PDialNormal.Name = "PDialNormal"
        Me.PDialNormal.Size = New System.Drawing.Size(542, 243)
        Me.PDialNormal.TabIndex = 22
        '
        'PDialCbC
        '
        Me.PDialCbC.Controls.Add(Me.GBoxCbC)
        Me.PDialCbC.Location = New System.Drawing.Point(5, 268)
        Me.PDialCbC.Name = "PDialCbC"
        Me.PDialCbC.Size = New System.Drawing.Size(542, 236)
        Me.PDialCbC.TabIndex = 23
        '
        'GBoxCbC
        '
        Me.GBoxCbC.Controls.Add(Me.Gespraechsart)
        Me.GBoxCbC.Controls.Add(Me.listCbCAnbieter)
        Me.GBoxCbC.Controls.Add(Me.Label2)
        Me.GBoxCbC.Controls.Add(Me.LLBiligertelefonieren)
        Me.GBoxCbC.Controls.Add(Me.LCbCWarnung)
        Me.GBoxCbC.Location = New System.Drawing.Point(3, 3)
        Me.GBoxCbC.Name = "GBoxCbC"
        Me.GBoxCbC.Size = New System.Drawing.Size(535, 219)
        Me.GBoxCbC.TabIndex = 16
        Me.GBoxCbC.TabStop = False
        Me.GBoxCbC.Text = "Call-by-Call mit Billiger-Telefonieren.de"
        '
        'listCbCAnbieter
        '
        Me.listCbCAnbieter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.listCbCAnbieter.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Nummer, Me.CentProMin, Me.Vorwahl, Me.Takt, Me.Tarif, Me.Bemerkung})
        Me.listCbCAnbieter.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.listCbCAnbieter.Location = New System.Drawing.Point(3, 26)
        Me.listCbCAnbieter.Name = "listCbCAnbieter"
        Me.listCbCAnbieter.RowHeadersVisible = False
        Me.listCbCAnbieter.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.listCbCAnbieter.ShowEditingIcon = False
        Me.listCbCAnbieter.Size = New System.Drawing.Size(524, 110)
        Me.listCbCAnbieter.TabIndex = 18
        '
        'Nummer
        '
        Me.Nummer.HeaderText = "Nr."
        Me.Nummer.Name = "Nummer"
        Me.Nummer.Width = 30
        '
        'CentProMin
        '
        Me.CentProMin.HeaderText = "ct./min"
        Me.CentProMin.Name = "CentProMin"
        Me.CentProMin.ReadOnly = True
        '
        'Vorwahl
        '
        Me.Vorwahl.HeaderText = "Vorwahl"
        Me.Vorwahl.Name = "Vorwahl"
        Me.Vorwahl.ReadOnly = True
        Me.Vorwahl.Width = 50
        '
        'Takt
        '
        Me.Takt.HeaderText = "Takt"
        Me.Takt.Name = "Takt"
        Me.Takt.ReadOnly = True
        Me.Takt.Width = 40
        '
        'Tarif
        '
        Me.Tarif.HeaderText = "Tarif"
        Me.Tarif.Name = "Tarif"
        '
        'Bemerkung
        '
        Me.Bemerkung.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.Bemerkung.HeaderText = "Bemerkung"
        Me.Bemerkung.Name = "Bemerkung"
        Me.Bemerkung.ReadOnly = True
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(3, 150)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(347, 18)
        Me.Label2.TabIndex = 19
        Me.Label2.Text = "Überprüfe Auswertungsergebnisse bei billiger-telefonieren.de:"
        '
        'LLBiligertelefonieren
        '
        Me.LLBiligertelefonieren.AutoSize = True
        Me.LLBiligertelefonieren.Location = New System.Drawing.Point(3, 168)
        Me.LLBiligertelefonieren.Name = "LLBiligertelefonieren"
        Me.LLBiligertelefonieren.Size = New System.Drawing.Size(145, 13)
        Me.LLBiligertelefonieren.TabIndex = 21
        Me.LLBiligertelefonieren.TabStop = True
        Me.LLBiligertelefonieren.Text = "www.Billiger-Telefonieren.de"
        '
        'LCbCWarnung
        '
        Me.LCbCWarnung.ForeColor = System.Drawing.Color.Red
        Me.LCbCWarnung.Location = New System.Drawing.Point(3, 188)
        Me.LCbCWarnung.Name = "LCbCWarnung"
        Me.LCbCWarnung.Size = New System.Drawing.Size(524, 30)
        Me.LCbCWarnung.TabIndex = 20
        Me.LCbCWarnung.Text = "Es wird keinerlei Haftung für die angezeigten Preise übernommen. Eine Richtigkeit" & _
    " der angezeigten Daten kann nicht gewährleistet werden. Die Benutzung erfolgt au" & _
    "f eigene Gefahr!"
        '
        'formWählbox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(560, 709)
        Me.Controls.Add(Me.PDialCbC)
        Me.Controls.Add(Me.PDialNormal)
        Me.Controls.Add(Me.PDialDirekt)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "formWählbox"
        Me.ShowIcon = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Anruf: [Name]"
        Me.TopMost = True
        CType(Me.ListTel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.Frame1.ResumeLayout(False)
        Me.Frame1.PerformLayout()
        Me.GBoxDirektWahl.ResumeLayout(False)
        Me.GBoxDirektWahl.PerformLayout()
        CType(Me.ContactImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PDialDirekt.ResumeLayout(False)
        Me.PDialNormal.ResumeLayout(False)
        Me.PDialNormal.PerformLayout()
        Me.PDialCbC.ResumeLayout(False)
        Me.GBoxCbC.ResumeLayout(False)
        Me.GBoxCbC.PerformLayout()
        CType(Me.listCbCAnbieter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents CloseButton As System.Windows.Forms.Button
    Friend WithEvents ButtonZeigeKontakt As System.Windows.Forms.Button
    Friend WithEvents ListTel As System.Windows.Forms.DataGridView
    Friend WithEvents checkNetz As System.Windows.Forms.CheckBox
    Friend WithEvents checkCLIR As System.Windows.Forms.CheckBox
    Friend WithEvents checkCBC As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents ComboBoxFon As System.Windows.Forms.ComboBox
    Friend WithEvents Frame1 As System.Windows.Forms.GroupBox
    Friend WithEvents cancelCallButton As System.Windows.Forms.Button
    Friend WithEvents LabelStatus As System.Windows.Forms.Label
    Friend WithEvents Gespraechsart As System.Windows.Forms.Label
    Friend WithEvents GBoxDirektWahl As System.Windows.Forms.GroupBox
    Friend WithEvents ButtonWeiter As System.Windows.Forms.Button
    Friend WithEvents TelNrBox As System.Windows.Forms.TextBox
    Friend WithEvents Titel As System.Windows.Forms.Label
    Friend WithEvents LabelCheckTest As System.Windows.Forms.Label
    Friend WithEvents ContactImage As System.Windows.Forms.PictureBox
    Friend WithEvents BVIP As System.Windows.Forms.CheckBox
    Friend WithEvents PDialDirekt As System.Windows.Forms.Panel
    Friend WithEvents PDialNormal As System.Windows.Forms.Panel
    Friend WithEvents PDialCbC As System.Windows.Forms.Panel
    Friend WithEvents GBoxCbC As System.Windows.Forms.GroupBox
    Friend WithEvents listCbCAnbieter As System.Windows.Forms.DataGridView
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents LLBiligertelefonieren As System.Windows.Forms.LinkLabel
    Friend WithEvents LCbCWarnung As System.Windows.Forms.Label
    Friend WithEvents Nr As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Typ As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TelNr As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Nummer As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents CentProMin As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Vorwahl As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Takt As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Tarif As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Bemerkung As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
