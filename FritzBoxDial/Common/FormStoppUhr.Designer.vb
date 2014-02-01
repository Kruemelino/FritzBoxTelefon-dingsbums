<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formStoppUhr
    Inherits System.Windows.Forms.Form
#If OVer = 11 Then
    Friend WithEvents Stoppuhr As FritzBoxDial.Stoppuhr
#Else
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
        Me.PopUpStoppUhr = New FritzBoxDial.PopUpStoppUhr()
        Me.SuspendLayout()
        '
        'PopUpStoppUhr
        '
        Me.PopUpStoppUhr.Anruf = Nothing
        Me.PopUpStoppUhr.ContentFont = New System.Drawing.Font("Tahoma", 18.0!)
        Me.PopUpStoppUhr.EndeZeit = Nothing
        Me.PopUpStoppUhr.MSN = Nothing
        Me.PopUpStoppUhr.Richtung = Nothing
        Me.PopUpStoppUhr.Size = New System.Drawing.Size(250, 100)
        Me.PopUpStoppUhr.StartPosition = New System.Drawing.Point(0, 0)
        Me.PopUpStoppUhr.StartZeit = Nothing
        Me.PopUpStoppUhr.TextPadding = New System.Windows.Forms.Padding(0)
        Me.PopUpStoppUhr.TitleFont = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.PopUpStoppUhr.WarteZeit = 0
        Me.PopUpStoppUhr.Zeit = Nothing
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
    Friend WithEvents PopUpStoppUhr As FritzBoxDial.PopUpStoppUhr
End Class
