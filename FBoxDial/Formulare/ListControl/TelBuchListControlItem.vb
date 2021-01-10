Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D

<Obsolete> Friend Class TelBuchListControlItem
#Region "Events"
    Public Event SelectionChanged(sender As Object)
    Friend WithEvents TmrMouseLeave As New Timer With {.Interval = 10}

    Public Event ContextMenuClicked(sender As Object, e As ToolStripItemClickedEventArgs, TB As FritzBoxXMLTelefonbuch)

#End Region

#Region "Properties"
    Private Property Roundness As Integer = 5
    'Public Property TelBuchName As String
    'Public Property Anzahl As Integer
    'Public Property Besitzer As String
    Public Property Selected As Boolean
    Public Property Telefonbuch As FritzBoxXMLTelefonbuch
    Private Property ShowBorders As Boolean = False
#End Region

#Region "DefaultFont"
    Private Property DefFontName() As String = "Microsoft Sans Serif"
    Private Property DefFontStyle() As FontStyle = FontStyle.Regular
    Private Property DefFontStyleBold() As FontStyle = FontStyle.Bold
    Private Property DefGraphicsUnit() As GraphicsUnit = GraphicsUnit.Point
    Private Property DefgdiCharSet() As Byte = CType(0, Byte)
#End Region

#Region "Properties RectangleF"
    Private Property RectName() As RectangleF
    Private Property RectAnzahl() As RectangleF
    Friend Property ScaleFaktor As SizeF

#End Region

#Region "Mouse coding"
    Private Enum MouseCapture
        Outside
        Inside
    End Enum
    Private Enum ButtonState
        ButtonUp
        ButtonDown
        Disabled
    End Enum
    Private Property BState As ButtonState
    Private Property BMouse As MouseCapture

    Private Sub ListControlItem_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick
        If Selected = False Then
            Selected = True
            RaiseEvent SelectionChanged(Me)
        End If

        If e.Button = MouseButtons.Right Then
            CMSTelefonbücher.Show(MousePosition)
        End If
    End Sub

    Private Sub ListControlItem_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        BState = ButtonState.ButtonDown
        Refresh()
    End Sub

    Private Sub ListControlItem_MouseEnter(sender As Object, e As EventArgs) Handles Me.MouseEnter
        BMouse = MouseCapture.Inside
        TmrMouseLeave.Start()
        Refresh()
    End Sub

    Private Sub ListControlItem_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp ', rdButton.MouseUp
        BState = ButtonState.ButtonUp
        Refresh()
    End Sub

    Private Sub TmrMouseLeave_Tick(sender As Object, e As EventArgs) Handles TmrMouseLeave.Tick
        Dim scrPT = MousePosition
        Dim ctlPT As Point = Me.PointToClient(scrPT)
        '
        If ctlPT.X.IsNegative Or ctlPT.Y.IsNegative Or ctlPT.X.IsLarger(Me.Width) Or ctlPT.Y.IsLarger(Me.Height) Then
            ' Stop timer
            TmrMouseLeave.Stop()
            BMouse = MouseCapture.Outside
            Refresh()
        Else
            BMouse = MouseCapture.Inside
        End If
    End Sub
#End Region

#Region "Painting"

    Private Sub Paint_DrawBackground(gfx As Graphics)
        Dim rect As New Rectangle(0, 0, Width - 1, Height - 1)

        'Build a rounded rectangle
        Dim p As New GraphicsPath
        With p
            .StartFigure()
            .AddArc(New Rectangle(rect.Left, rect.Top, Roundness, Roundness), 180, 90)
            .AddLine(rect.Left + Roundness, 0, rect.Right - Roundness, 0)
            .AddArc(New Rectangle(rect.Right - Roundness, 0, Roundness, Roundness), -90, 90)
            .AddLine(rect.Right, Roundness, rect.Right, rect.Bottom - Roundness)
            .AddArc(New Rectangle(rect.Right - Roundness, rect.Bottom - Roundness, Roundness, Roundness), 0, 90)
            .AddLine(rect.Right - Roundness, rect.Bottom, rect.Left + Roundness, rect.Bottom)
            .AddArc(New Rectangle(rect.Left, rect.Height - Roundness, Roundness, Roundness), 90, 90)
            .CloseFigure()
        End With

        ' Draw the background
        Dim ColorScheme As Color() = Nothing
        Dim brdr As SolidBrush

        If bState = ButtonState.Disabled Then
            ' normal
            brdr = ColorSchemes.DisabledBorder
            ColorScheme = ColorSchemes.DisabledAllColor
        Else
            If Selected Then
                ' Selected
                brdr = ColorSchemes.SelectedBorder

                If bState = ButtonState.ButtonUp And bMouse = MouseCapture.Outside Then
                    ' normal
                    ColorScheme = ColorSchemes.SelectedNormal

                ElseIf bState = ButtonState.ButtonUp And bMouse = MouseCapture.Inside Then
                    '  hover 
                    ColorScheme = ColorSchemes.SelectedHover

                ElseIf bState = ButtonState.ButtonDown And bMouse = MouseCapture.Outside Then
                    ' no one cares!
                    Exit Sub
                ElseIf bState = ButtonState.ButtonDown And bMouse = MouseCapture.Inside Then
                    ' pressed
                    ColorScheme = ColorSchemes.SelectedPressed
                End If

            Else
                ' Not selected
                brdr = ColorSchemes.UnSelectedBorder

                If bState = ButtonState.ButtonUp And bMouse = MouseCapture.Outside Then
                    ' normal
                    brdr = ColorSchemes.DisabledBorder
                    ColorScheme = ColorSchemes.UnSelectedNormal

                ElseIf bState = ButtonState.ButtonUp And bMouse = MouseCapture.Inside Then
                    '  hover 
                    ColorScheme = ColorSchemes.UnSelectedHover

                ElseIf bState = ButtonState.ButtonDown And bMouse = MouseCapture.Outside Then
                    ' no one cares!
                    Exit Sub
                ElseIf bState = ButtonState.ButtonDown And bMouse = MouseCapture.Inside Then
                    ' pressed
                    ColorScheme = ColorSchemes.UnSelectedPressed
                End If

            End If
        End If

        ' Draw
        Dim b As LinearGradientBrush = New LinearGradientBrush(rect, Color.White, Color.Black, LinearGradientMode.Vertical)
        Dim blend As ColorBlend = New ColorBlend With {
            .Colors = ColorScheme,
            .Positions = New Single() {0.0F, 0.1, 0.9F, 0.95F, 1.0F}
        }
        b.InterpolationColors = blend
        gfx.FillPath(b, p)

        ' Draw border
        gfx.DrawPath(New Pen(brdr), p)

        ' Draw bottom border if Normal state (not hovered)
        If bMouse = MouseCapture.Outside Then
            rect = New Rectangle(rect.Left, Me.Height - 1, rect.Width, 1)
            b = New LinearGradientBrush(rect, Color.Blue, Color.Yellow, LinearGradientMode.Horizontal)
            blend = New ColorBlend With {
                .Colors = New Color() {Color.White, Color.LightGray, Color.White},
                .Positions = New Single() {0.0F, 0.5F, 1.0F}
            }
            b.InterpolationColors = blend
            '
            gfx.FillRectangle(b, rect)
        End If
    End Sub

    Private Sub Paint_Telefonbuch(gfx As Graphics)

        Dim fnt As Font
        Dim SF As New StringFormat With {.Trimming = StringTrimming.EllipsisCharacter, .LineAlignment = StringAlignment.Center}
        Dim workingRect As New RectangleF(30, 0, Me.Width - 40 - 6, Me.Height - 1)

        ' Telefonbuchname
        fnt = New Font(DefFontName, 10, DefFontStyleBold, DefGraphicsUnit, DefgdiCharSet)
        RectName = New RectangleF(30, 0, gfx.MeasureString(Telefonbuch.Name, fnt).Width, workingRect.Height)
        gfx.DrawString(Telefonbuch.Name, fnt, Brushes.Black, RectName, SF)

        ' Anzahl an Elementen
        fnt = New Font(DefFontName, 10, DefFontStyle, DefGraphicsUnit, DefgdiCharSet)
        RectAnzahl = New RectangleF(workingRect.Right - gfx.MeasureString(Telefonbuch.Kontakte.Count.ToString, fnt).Width, 0, gfx.MeasureString(Telefonbuch.Kontakte.Count.ToString, fnt).Width, workingRect.Height)
        gfx.DrawString(Telefonbuch.Kontakte.Count.ToString, fnt, Brushes.Black, RectAnzahl, SF)

        If ShowBorders Then
            DrawRectangleF(gfx, workingRect, Color.Red)
            DrawRectangleF(gfx, RectName, Color.Blue)
            DrawRectangleF(gfx, RectAnzahl, Color.Green)
        End If
    End Sub

    Private Sub PaintEvent(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Dim gfx = e.Graphics

        Paint_DrawBackground(gfx)
        Paint_Telefonbuch(gfx)
    End Sub

    Private Sub DrawRectangleF(ByVal g As Graphics, ByVal rect As RectangleF, ByVal color As Color)
        Using pen As Pen = New Pen(color, 1)
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height)
        End Using
    End Sub

    Private Sub TelBuchListControlItem_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        Refresh()
    End Sub

#End Region

    Private Sub CMSTelefonbücher_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles CMSTelefonbücher.ItemClicked
        RaiseEvent ContextMenuClicked(sender, e, Telefonbuch)
    End Sub


End Class
