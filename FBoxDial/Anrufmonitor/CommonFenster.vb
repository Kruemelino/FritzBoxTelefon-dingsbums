Imports System.Drawing
Imports System.Windows.Forms

Friend Class CommonFenster
    Implements IDisposable

#Region "DefaultFont"
    Friend Property DefFontName() As String = "Microsoft Sans Serif"
    Friend Property DefFontStyle() As FontStyle = FontStyle.Regular
    Friend Property FontStyleBold() As FontStyle = FontStyle.Bold
    Friend Property DefGraphicsUnit() As GraphicsUnit = GraphicsUnit.Point
    Friend Property DefgdiCharSet() As Byte = CType(0, Byte)
#End Region

#Region "Properties Allgemein"
    Friend Property HeaderColor() As Color = SystemColors.ControlDarkDark
    Friend Property BodyColor() As Color = SystemColors.Control
    Friend Property TitleColor() As Color = SystemColors.ControlText
    Friend Property ContentColor() As Color = SystemColors.ControlText
    Friend Property BorderColor() As Color = SystemColors.WindowFrame
    Friend Property ButtonBorderColor() As Color = SystemColors.WindowFrame
    Friend Property ButtonHoverColor() As Color = SystemColors.Highlight
    Friend Property GradientPower() As Integer = 50
    Friend Property TextPadding() As Padding = New Padding(5)
    Friend Property HeaderHeight() As Integer = 9
#End Region

#Region "Properties Anrufmonitor"
    Friend Property LinkHoverColor() As Color = SystemColors.Highlight
    Friend Property OptionsButton() As Boolean = True
    Friend Property TitleFont() As Font = New Font(DefFontName, 8, DefFontStyle, DefGraphicsUnit, DefgdiCharSet)
    Friend Property TelNrFont() As Font = New Font(DefFontName, 11, DefFontStyle, DefGraphicsUnit, DefgdiCharSet)
    Friend Property AnrNameFont() As Font = New Font(DefFontName, 16, FontStyleBold, DefGraphicsUnit, DefgdiCharSet)
    Friend Property Image() As Image = Nothing
    Friend Property AnrName() As String
    Friend Property Uhrzeit() As String
    Friend Property TelNr() As String
    Friend Property TelName() As String
    Friend Property Firma() As String
#End Region

    Private Function AddValueMax255(ByVal Input As Integer, ByVal Add As Integer) As Integer
        If Input + Add < 256 Then
            Return Input + Add
        Else
            Return 255
        End If
    End Function

    Private Function DedValueMin0(ByVal Input As Integer, ByVal Ded As Integer) As Integer
        Return GetLarger(Input - Ded, 0)
        'If Input - Ded > 0 Then
        '    Return Input - Ded
        'Else
        '    Return 0
        'End If
    End Function

    Friend Function GetDarkerColor(ByVal Color As Color) As Color
        Dim clNew As Color
        clNew = Drawing.Color.FromArgb(255, DedValueMin0(Color.R.ToInt, GradientPower), DedValueMin0(Color.G.ToInt, GradientPower), DedValueMin0(Color.B.ToInt, GradientPower))
        Return clNew
    End Function

    Friend Function GetLighterColor(ByVal Color As Color) As Color
        Dim clNew As Color
        clNew = Drawing.Color.FromArgb(255, AddValueMax255(CInt(Color.R), GradientPower), AddValueMax255(CInt(Color.G), GradientPower), AddValueMax255(CInt(Color.B), GradientPower))
        Return clNew
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' Managed Code?
            End If
            TitleFont.Dispose()
            TelNrFont.Dispose()
            AnrNameFont.Dispose()
        End If
        Me.disposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
        Dispose(False)
        MyBase.Finalize()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class