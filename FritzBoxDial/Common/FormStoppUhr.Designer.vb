<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formStoppUhr
    Inherits System.Windows.Forms.Form
#If OVer = 11 Then
    Friend WithEvents Stoppuhr As FritzBoxDial.Stoppuhr
#Else
    Friend WithEvents Stoppuhr As Stoppuhr
#End If

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
#If OVer = 11 Then
        Me.Stoppuhr = New FritzBoxDial.Stoppuhr()
#Else
        Me.Stoppuhr = New Stoppuhr()
#End If

        Me.SuspendLayout()
        '
        'Stoppuhr
        '
        Me.Stoppuhr.Anruf = Nothing
        Me.Stoppuhr.ButtonHoverColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.Stoppuhr.ContentFont = New System.Drawing.Font("Tahoma", 20.0!)
        Me.Stoppuhr.StartPosition = New System.Drawing.Point(10, 10)
        Me.Stoppuhr.Size = New System.Drawing.Size(200, 100)
        Me.Stoppuhr.TextPadding = New System.Windows.Forms.Padding(0)
        Me.Stoppuhr.TitleFont = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Stoppuhr.Zeit = Nothing
        '
        'formStoppUhr
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 262)
        Me.Name = "formStoppUhr"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub
End Class
