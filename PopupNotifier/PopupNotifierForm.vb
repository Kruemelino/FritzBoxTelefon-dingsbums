Imports System.ComponentModel
Imports System.Drawing.Drawing2D

<System.ComponentModel.DefaultPropertyAttribute("Content"), _
System.ComponentModel.DesignTimeVisible(False)> _
Public Class PopupNotifierForm
    Inherits System.Windows.Forms.Form

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' Copyright ©1996-2011 VBnet/Randy Birch, All Rights Reserved.
    ' Some pages may also contain other copyrights by the author.
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' Distribution: You can freely use this code in your own
    '               applications, but you may not reproduce 
    '               or publish this code on any web site,
    '               online service, or distribute as source 
    '               on any media without express permission.
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Sub New(ByVal Parent As PopupNotifier)
        pnParent = Parent
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
    End Sub


    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'PopupNotifierForm
        '
        Me.ClientSize = New System.Drawing.Size(392, 66)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "PopupNotifierForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.ResumeLayout(True)
    End Sub

    Private bMouseOnClose As Boolean = False
    Private bMouseOnLink As Boolean = False
    Private bMouseOnOptions As Boolean = False
    Private iHeightOfTitle As Integer
    Private iHeightOfAnrName As Integer
    Private iHeightOfTelNr As Integer
    Private iTitleOrigin As Integer
    Public Event LinkClick()
    Public Event CloseClick()

#Region "Properties"
    Protected Overrides ReadOnly Property ShowWithoutActivation() As Boolean
        Get
            Return True
        End Get
    End Property
    Private pnParent As PopupNotifier
    Shadows Property Parent() As PopupNotifier
        Get
            Return pnParent
        End Get
        Set(ByVal value As PopupNotifier)
            pnParent = value
        End Set
    End Property
    'Protected Overrides ReadOnly Property CreateParams As CreateParams

    '    Get
    '        Dim baseParams As System.Windows.Forms.CreateParams = MyBase.CreateParams
    '        ' WS_EX_NOACTIVATE = 0x08000000,
    '        ' WS_EX_TOOLWINDOW = 0x00000080,
    '        ' baseParams.ExStyle |= ( int )( 
    '        '  Win32.ExtendedWindowStyles.WS_EX_NOACTIVATE | 
    '        '  Win32.ExtendedWindowStyles.WS_EX_TOOLWINDOW );
    '        'baseParams.ExStyle = baseParams.ExStyle Or CInt((WindowStyles.WS_EX_NOACTIVATE Or WindowStyles.WS_EX_TOOLWINDOW))

    '        Return baseParams
    '    End Get
    'End Property
#End Region

#Region "Functions & Private properties"
    Private Function AddValueMax255(ByVal Input As Integer, ByVal Add As Integer) As Integer
        If Input + Add < 256 Then
            Return Input + Add
        Else
            Return 255
        End If
    End Function

    Private Function DedValueMin0(ByVal Input As Integer, ByVal Ded As Integer) As Integer
        If Input - Ded > 0 Then
            Return Input - Ded
        Else
            Return 0
        End If
    End Function

    Private Function GetDarkerColor(ByVal Color As Color) As Color
        Dim clNew As Color
        clNew = Drawing.Color.FromArgb(255, DedValueMin0(CInt(Color.R), Parent.GradientPower), DedValueMin0(CInt(Color.G), Parent.GradientPower), DedValueMin0(CInt(Color.B), Parent.GradientPower))
        Return clNew
    End Function

    Private Function GetLighterColor(ByVal Color As Color) As Color
        Dim clNew As Color
        clNew = Drawing.Color.FromArgb(255, AddValueMax255(CInt(Color.R), Parent.GradientPower), AddValueMax255(CInt(Color.G), Parent.GradientPower), AddValueMax255(CInt(Color.B), Parent.GradientPower))
        Return clNew
    End Function

    Private Function GetLighterTransparentColor(ByVal Color As Color) As Color
        Dim clNew As Color
        clNew = Drawing.Color.FromArgb(0, AddValueMax255(CInt(Color.R), Parent.GradientPower), AddValueMax255(CInt(Color.G), Parent.GradientPower), AddValueMax255(CInt(Color.B), Parent.GradientPower))
        Return clNew
    End Function

    Private ReadOnly Property RectTelNr() As RectangleF

        Get
            If Not Parent.Image Is Nothing Then
                Return New RectangleF(Parent.ImagePosition.X + Parent.ImageSize.Width + Parent.TextPadding.Left, Parent.TextPadding.Top + iHeightOfTitle + 1.5 * Parent.HeaderHeight, Me.Width - Parent.ImageSize.Width - Parent.ImagePosition.X - Parent.TextPadding.Left - Parent.TextPadding.Right, iHeightOfTelNr)
            Else
                Return New RectangleF(Parent.TextPadding.Left, Parent.TextPadding.Top + iHeightOfTitle + 1.5 * Parent.HeaderHeight, Me.Width - Parent.TextPadding.Left - Parent.TextPadding.Right, iHeightOfTelNr)
            End If
        End Get
    End Property

    Private ReadOnly Property RectAnrName() As RectangleF

        Get
            If Not Parent.Image Is Nothing Then
                Return New RectangleF(Parent.ImagePosition.X + Parent.ImageSize.Width + Parent.TextPadding.Left, Parent.TextPadding.Top + iHeightOfTitle + 1.5 * Parent.HeaderHeight + iHeightOfTelNr, Me.Width - Parent.ImageSize.Width - Parent.ImagePosition.X - Parent.TextPadding.Left - Parent.TextPadding.Right, iHeightOfAnrName)
            Else
                Return New RectangleF(Parent.TextPadding.Left, Parent.TextPadding.Top + iHeightOfTitle + 1.5 * Parent.HeaderHeight + iHeightOfTelNr, Me.Width - Parent.TextPadding.Left - Parent.TextPadding.Right, iHeightOfAnrName)
            End If
        End Get
    End Property

    Private ReadOnly Property RectFirma() As RectangleF

        Get
            If Not Parent.Image Is Nothing Then
                Return New RectangleF(Parent.ImagePosition.X + Parent.ImageSize.Width + Parent.TextPadding.Left, Me.Height - Parent.TextPadding.Bottom - iHeightOfTitle, Me.Width - Parent.ImageSize.Width - Parent.ImagePosition.X - Parent.TextPadding.Left - Parent.TextPadding.Right, iHeightOfTitle)
            Else
                Return New RectangleF(Parent.TextPadding.Left, Me.Height - iHeightOfTitle - Parent.TextPadding.Bottom, Me.Width - Parent.TextPadding.Left - Parent.TextPadding.Right, iHeightOfTitle)
            End If
        End Get
    End Property

    Private ReadOnly Property RectClose() As Rectangle
        Get
            Return New Rectangle(Me.Width - 5 - 16, 12, 16, 16)
        End Get
    End Property

    Private ReadOnly Property RectOptions() As Rectangle
        Get
            Return New Rectangle(Me.Width - 5 - 16, 12 + 16 + 5, 16, 16)
        End Get
    End Property

    Private ReadOnly Property RectImage() As Rectangle
        Get
            If Not Parent.Image Is Nothing Then
                Return New Rectangle(Parent.ImagePosition, Parent.ImageSize)
            End If
        End Get
    End Property

#End Region

#Region "Events"

    Private Sub Form1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        If Parent.CloseButton Then
            If RectClose.Contains(e.X, e.Y) Then
                bMouseOnClose = True
            Else
                bMouseOnClose = False
            End If
        End If
        If Parent.OptionsButton Then
            If RectOptions.Contains(e.X, e.Y) Then
                bMouseOnOptions = True
            Else
                bMouseOnOptions = False
            End If
        End If
        If RectAnrName.Contains(e.X, e.Y) Then
            bMouseOnLink = True
        Else
            bMouseOnLink = False
        End If
        Invalidate()
    End Sub

    Private Sub Form1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        If RectClose.Contains(e.X, e.Y) Then
            RaiseEvent CloseClick()
        End If
        If RectAnrName.Contains(e.X, e.Y) Then
            RaiseEvent LinkClick()
        End If
        If RectOptions.Contains(e.X, e.Y) Then
            If Not Parent.OptionsMenu Is Nothing Then
                Parent.OptionsMenu.Show(Me, New Point(RectOptions.Right - Parent.OptionsMenu.Width, RectOptions.Bottom))
                Parent.bShouldRemainVisible = True
            End If
        End If
    End Sub

    Private Sub Form1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        Dim iTelNameLänge As Integer
        Dim iUhrzeitLänge As Integer
        Dim iAnrNameLänge As Integer
        Dim sUhrzeit As String
        Dim sTelName As String
        Dim Länge As Integer

        Dim rcBody As New Rectangle(0, 0, Me.Width, Me.Height)

        Dim rcHeader As New Rectangle(0, 0, Me.Width, Parent.HeaderHeight)
        Dim rcForm As New Rectangle(0, 0, Me.Width - 1, Me.Height - 1)
        Dim brBody As New LinearGradientBrush(rcBody, Parent.BodyColor, GetLighterColor(Parent.BodyColor), LinearGradientMode.Vertical)
        Dim drawFormatCenter As New StringFormat()
        drawFormatCenter.Alignment = StringAlignment.Center
        Dim drawFormatRight As New StringFormat()
        drawFormatRight.Alignment = StringAlignment.Far
        Dim brHeader As New LinearGradientBrush(rcHeader, Parent.HeaderColor, GetDarkerColor(Parent.HeaderColor), LinearGradientMode.Vertical)
        Dim RectZeit As RectangleF
        Dim RectTelName As RectangleF
        With e.Graphics
            .Clip = New Region(rcBody)
            .FillRectangle(brBody, rcBody)
            .FillRectangle(brHeader, rcHeader)
            .DrawRectangle(New Pen(Parent.BorderColor), rcForm)
            If Parent.CloseButton Then
                If bMouseOnClose Then
                    .FillRectangle(New SolidBrush(Parent.ButtonHoverColor), RectClose)
                    .DrawRectangle(New Pen(Parent.ButtonBorderColor), RectClose)
                End If
                .DrawLine(New Pen(Parent.ContentColor, 2), RectClose.Left + 4, RectClose.Top + 4, RectClose.Right - 4, RectClose.Bottom - 4)
                .DrawLine(New Pen(Parent.ContentColor, 2), RectClose.Left + 4, RectClose.Bottom - 4, RectClose.Right - 4, RectClose.Top + 4)
            End If
            If Parent.OptionsButton Then
                If bMouseOnOptions Then
                    .FillRectangle(New SolidBrush(Parent.ButtonHoverColor), RectOptions)
                    .DrawRectangle(New Pen(Parent.ButtonBorderColor), RectOptions)
                End If
                .FillPolygon(New SolidBrush(ForeColor), New Point() {New Point(RectOptions.Left + 4, RectOptions.Top + 6), New Point(RectOptions.Left + 12, RectOptions.Top + 6), New Point(RectOptions.Left + 8, RectOptions.Top + 4 + 6)})
            End If
            '.Clip = New Region(rcClip)
            iHeightOfTitle = .MeasureString("A", Parent.TitleFont).Height
            iHeightOfAnrName = .MeasureString("A", Parent.ContentFont).Height
            iHeightOfTelNr = .MeasureString("A", Parent.TelNrFont).Height
            iTitleOrigin = Parent.TextPadding.Left
            If Not Parent.Image Is Nothing Then
                Dim showim As Image = New Bitmap(Parent.ImageSize.Width, Parent.ImageSize.Height)
                Dim g1 As Graphics = Graphics.FromImage(showim)
                g1.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                g1.DrawImage(Parent.Image, 0, 0, Parent.ImageSize.Width, Parent.ImageSize.Height)
                g1.Dispose()
                .DrawImage(showim, Parent.ImagePosition)
                .DrawRectangle(New Pen(Parent.ButtonBorderColor), RectImage)
            End If
            Länge = Parent.Size.Width - Parent.TextPadding.Right - 21 - iTitleOrigin + Parent.TextPadding.Left
            sUhrzeit = CDate(Parent.Uhrzeit).ToString("dddd, dd. MMMM yyyy HH:mm:ss")
            sTelName = Parent.TelName
            iTelNameLänge = .MeasureString(sTelName, Parent.TitleFont).Width
            iUhrzeitLänge = .MeasureString(sUhrzeit, Parent.TitleFont).Width
            If iTelNameLänge + iUhrzeitLänge > Länge Then
                sUhrzeit = CDate(Parent.Uhrzeit).ToString("dddd, dd. MMM. yy HH:mm:ss")
                iUhrzeitLänge = .MeasureString(sUhrzeit, Parent.TitleFont).Width
                If iTelNameLänge + iUhrzeitLänge > Länge Then
                    sUhrzeit = CDate(Parent.Uhrzeit).ToString("ddd, dd.MM.yy HH:mm:ss")
                    iUhrzeitLänge = .MeasureString(sUhrzeit, Parent.TitleFont).Width
                    If iTelNameLänge + iUhrzeitLänge > Länge Then
                        sUhrzeit = CDate(Parent.Uhrzeit).ToString("dd.MM.yy HH:mm:ss")
                        iUhrzeitLänge = .MeasureString(sUhrzeit, Parent.TitleFont).Width
                    End If
                End If
            End If
            RectZeit = New RectangleF(iTitleOrigin + Parent.TextPadding.Left, Parent.TextPadding.Top + Parent.HeaderHeight, .MeasureString(sUhrzeit, Parent.TitleFont).Width, iHeightOfTitle)
            RectTelName = New RectangleF(RectZeit.Right, RectZeit.Top, RectClose.Left - RectZeit.Right, iHeightOfTitle)

            .DrawString(sUhrzeit, Parent.TitleFont, New SolidBrush(Parent.TitleColor), RectZeit)
            If iTelNameLänge > RectTelName.Width Then
                RectTelName.Y = Parent.HeaderHeight
                RectTelName.Size = New Size(RectTelName.Width, RectTelName.Height * 2 - 3)
            End If
            .DrawString(sTelName, Parent.TitleFont, New SolidBrush(Parent.TitleColor), RectTelName, drawFormatRight)
            .DrawString(Parent.TelNr, Parent.TelNrFont, New SolidBrush(Parent.TitleColor), RectTelNr, drawFormatCenter)
            .DrawString(Parent.Firma, Parent.TitleFont, New SolidBrush(Parent.TitleColor), RectFirma, drawFormatCenter)

            Dim tempfont As New Font("Microsoft Sans Serif", 16, FontStyle.Regular)
            Dim sAnrName As String
            sAnrName = Parent.AnrName
            iAnrNameLänge = .MeasureString(sAnrName, tempfont, 0, StringFormat.GenericTypographic).Width

            If iAnrNameLänge > RectAnrName.Width Then
                Dim iFontSize As Integer
                iFontSize = ((RectAnrName.Width - Parent.TextPadding.Right - Parent.TextPadding.Left) * (tempfont.Size / 72 * .DpiX - 1.5 * Parent.TextPadding.Top)) / (iAnrNameLänge - 2 * Parent.TextPadding.Left)
                iFontSize = IIf(iFontSize < 8, 8, iFontSize)
                tempfont = New Font("Microsoft Sans Serif", iFontSize, FontStyle.Regular)
            End If

            If bMouseOnLink Then
                Me.Cursor = Cursors.Hand
                .DrawString(Parent.AnrName, tempfont, New SolidBrush(Parent.LinkHoverColor), RectAnrName, drawFormatCenter)
            Else
                Me.Cursor = Cursors.Default
                .DrawString(Parent.AnrName, tempfont, New SolidBrush(Parent.ContentColor), RectAnrName, drawFormatCenter)
            End If
        End With
    End Sub

#End Region

End Class




