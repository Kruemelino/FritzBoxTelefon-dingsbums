<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formRWSuche
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(formRWSuche))
        Me.LabelName = New System.Windows.Forms.Label()
        Me.DirektTel = New System.Windows.Forms.TextBox()
        Me.LabelDirekteingabe = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ListTel = New System.Windows.Forms.DataGridView()
        Me.Nr = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Typ = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TelNr = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ButtonSuchen = New System.Windows.Forms.Button()
        CType(Me.ListTel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LabelName
        '
        Me.LabelName.AutoSize = True
        Me.LabelName.Location = New System.Drawing.Point(9, 9)
        Me.LabelName.Name = "LabelName"
        Me.LabelName.Size = New System.Drawing.Size(104, 13)
        Me.LabelName.TabIndex = 0
        Me.LabelName.Text = "Telefonnummernliste"
        '
        'DirektTel
        '
        Me.DirektTel.Location = New System.Drawing.Point(12, 127)
        Me.DirektTel.Name = "DirektTel"
        Me.DirektTel.Size = New System.Drawing.Size(302, 20)
        Me.DirektTel.TabIndex = 1
        '
        'LabelDirekteingabe
        '
        Me.LabelDirekteingabe.AutoSize = True
        Me.LabelDirekteingabe.Location = New System.Drawing.Point(12, 111)
        Me.LabelDirekteingabe.Name = "LabelDirekteingabe"
        Me.LabelDirekteingabe.Size = New System.Drawing.Size(170, 13)
        Me.LabelDirekteingabe.TabIndex = 2
        Me.LabelDirekteingabe.Text = "Direkteingabe der Telefonnummer:"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Red
        Me.Label1.Location = New System.Drawing.Point(12, 160)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(390, 42)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = resources.GetString("Label1.Text")
        '
        'ListTel
        '
        Me.ListTel.AllowUserToAddRows = False
        Me.ListTel.AllowUserToDeleteRows = False
        Me.ListTel.AllowUserToResizeColumns = False
        Me.ListTel.AllowUserToResizeRows = False
        Me.ListTel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.ListTel.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Nr, Me.Typ, Me.TelNr})
        Me.ListTel.Location = New System.Drawing.Point(15, 25)
        Me.ListTel.Name = "ListTel"
        Me.ListTel.ReadOnly = True
        Me.ListTel.RowHeadersVisible = False
        Me.ListTel.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.ListTel.Size = New System.Drawing.Size(384, 83)
        Me.ListTel.TabIndex = 6
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
        Me.Typ.MinimumWidth = 130
        Me.Typ.Name = "Typ"
        Me.Typ.ReadOnly = True
        Me.Typ.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.Typ.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Typ.ToolTipText = "Eintragstyp"
        Me.Typ.Width = 130
        '
        'TelNr
        '
        Me.TelNr.HeaderText = "Telefonnummer"
        Me.TelNr.MinimumWidth = 180
        Me.TelNr.Name = "TelNr"
        Me.TelNr.ReadOnly = True
        Me.TelNr.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.TelNr.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.TelNr.ToolTipText = "Telefonnummer des Anzurufenden"
        Me.TelNr.Width = 225
        '
        'ButtonSuchen
        '
        Me.ButtonSuchen.Location = New System.Drawing.Point(320, 128)
        Me.ButtonSuchen.Name = "ButtonSuchen"
        Me.ButtonSuchen.Size = New System.Drawing.Size(79, 19)
        Me.ButtonSuchen.TabIndex = 7
        Me.ButtonSuchen.Text = "Suchen"
        Me.ButtonSuchen.UseVisualStyleBackColor = True
        '
        'formRWSuche
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(411, 208)
        Me.Controls.Add(Me.ButtonSuchen)
        Me.Controls.Add(Me.ListTel)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.DirektTel)
        Me.Controls.Add(Me.LabelName)
        Me.Controls.Add(Me.LabelDirekteingabe)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "formRWSuche"
        Me.Text = "fromRWSuche"
        CType(Me.ListTel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents LabelName As System.Windows.Forms.Label
    Friend WithEvents DirektTel As System.Windows.Forms.TextBox
    Friend WithEvents LabelDirekteingabe As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ListTel As System.Windows.Forms.DataGridView
    Friend WithEvents Nr As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Typ As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TelNr As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ButtonSuchen As System.Windows.Forms.Button
End Class
