<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formTBControl
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(formTBControl))
        Me.CBoxFBTelbuch = New System.Windows.Forms.ComboBox()
        Me.TBAdrbuchName = New System.Windows.Forms.TextBox()
        Me.LTelefonbuchName = New System.Windows.Forms.Label()
        Me.LHinweis = New System.Windows.Forms.Label()
        Me.BDel = New System.Windows.Forms.Button()
        Me.BAdd = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'CBoxFBTelbuch
        '
        Me.CBoxFBTelbuch.FormattingEnabled = True
        Me.CBoxFBTelbuch.Location = New System.Drawing.Point(10, 52)
        Me.CBoxFBTelbuch.Name = "CBoxFBTelbuch"
        Me.CBoxFBTelbuch.Size = New System.Drawing.Size(172, 21)
        Me.CBoxFBTelbuch.TabIndex = 11
        '
        'TBAdrbuchName
        '
        Me.TBAdrbuchName.Location = New System.Drawing.Point(10, 25)
        Me.TBAdrbuchName.Name = "TBAdrbuchName"
        Me.TBAdrbuchName.Size = New System.Drawing.Size(172, 20)
        Me.TBAdrbuchName.TabIndex = 10
        '
        'LTelefonbuchName
        '
        Me.LTelefonbuchName.AutoSize = True
        Me.LTelefonbuchName.Location = New System.Drawing.Point(12, 9)
        Me.LTelefonbuchName.Name = "LTelefonbuchName"
        Me.LTelefonbuchName.Size = New System.Drawing.Size(96, 13)
        Me.LTelefonbuchName.TabIndex = 9
        Me.LTelefonbuchName.Text = "Telefonbuchname:"
        '
        'LHinweis
        '
        Me.LHinweis.Location = New System.Drawing.Point(8, 134)
        Me.LHinweis.Name = "LHinweis"
        Me.LHinweis.Size = New System.Drawing.Size(174, 247)
        Me.LHinweis.TabIndex = 8
        Me.LHinweis.Text = resources.GetString("LHinweis.Text")
        '
        'BDel
        '
        Me.BDel.Location = New System.Drawing.Point(9, 108)
        Me.BDel.Name = "BDel"
        Me.BDel.Size = New System.Drawing.Size(174, 23)
        Me.BDel.TabIndex = 6
        Me.BDel.Text = "Löschen"
        Me.BDel.UseVisualStyleBackColor = True
        '
        'BAdd
        '
        Me.BAdd.Location = New System.Drawing.Point(9, 79)
        Me.BAdd.Name = "BAdd"
        Me.BAdd.Size = New System.Drawing.Size(174, 23)
        Me.BAdd.TabIndex = 7
        Me.BAdd.Text = "Hinzufügen"
        Me.BAdd.UseVisualStyleBackColor = True
        '
        'formTBControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(200, 390)
        Me.Controls.Add(Me.CBoxFBTelbuch)
        Me.Controls.Add(Me.TBAdrbuchName)
        Me.Controls.Add(Me.LTelefonbuchName)
        Me.Controls.Add(Me.LHinweis)
        Me.Controls.Add(Me.BDel)
        Me.Controls.Add(Me.BAdd)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "formTBControl"
        Me.Text = "Form2"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents CBoxFBTelbuch As System.Windows.Forms.ComboBox
    Friend WithEvents TBAdrbuchName As System.Windows.Forms.TextBox
    Friend WithEvents LTelefonbuchName As System.Windows.Forms.Label
    Friend WithEvents LHinweis As System.Windows.Forms.Label
    Friend WithEvents BDel As System.Windows.Forms.Button
    Friend WithEvents BAdd As System.Windows.Forms.Button
End Class
