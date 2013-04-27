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

    'Public Enum WindowStyles As Long

    '    WS_OVERLAPPED = 0
    '    WS_POPUP = 2147483648
    '    WS_CHILD = 1073741824
    '    WS_MINIMIZE = 536870912
    '    WS_VISIBLE = 268435456
    '    WS_DISABLED = 134217728
    '    WS_CLIPSIBLINGS = 67108864
    '    WS_CLIPCHILDREN = 33554432
    '    WS_MAXIMIZE = 16777216
    '    WS_BORDER = 8388608
    '    WS_DLGFRAME = 4194304
    '    WS_VSCROLL = 2097152
    '    WS_HSCROLL = 1048576
    '    WS_SYSMENU = 524288
    '    WS_THICKFRAME = 262144
    '    WS_GROUP = 131072
    '    WS_TABSTOP = 65536

    '    WS_MINIMIZEBOX = 131072
    '    WS_MAXIMIZEBOX = 65536

    '    WS_CAPTION = WS_BORDER Or WS_DLGFRAME
    '    WS_TILED = WS_OVERLAPPED
    '    WS_ICONIC = WS_MINIMIZE
    '    WS_SIZEBOX = WS_THICKFRAME
    '    WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW

    '    WS_OVERLAPPEDWINDOW = WS_OVERLAPPED Or WS_CAPTION Or WS_SYSMENU Or _
    '              WS_THICKFRAME Or WS_MINIMIZEBOX Or WS_MAXIMIZEBOX
    '    WS_POPUPWINDOW = WS_POPUP Or WS_BORDER Or WS_SYSMENU
    '    WS_CHILDWINDOW = WS_CHILD

    '    WS_EX_DLGMODALFRAME = 1
    '    WS_EX_NOPARENTNOTIFY = 4
    '    WS_EX_TOPMOST = 8
    '    WS_EX_ACCEPTFILES = 16
    '    WS_EX_TRANSPARENT = 32

    '    '#If (WINVER >= 400) Then
    '    WS_EX_MDICHILD = 64
    '    WS_EX_TOOLWINDOW = 128
    '    WS_EX_WINDOWEDGE = 256
    '    WS_EX_CLIENTEDGE = 512
    '    WS_EX_CONTEXTHELP = 1024

    '    WS_EX_RIGHT = 4096
    '    WS_EX_LEFT = 0
    '    WS_EX_RTLREADING = 8192
    '    WS_EX_LTRREADING = 0
    '    WS_EX_LEFTSCROLLBAR = 16384
    '    WS_EX_RIGHTSCROLLBAR = 0

    '    WS_EX_CONTROLPARENT = 65536
    '    WS_EX_STATICEDGE = 131072
    '    WS_EX_APPWINDOW = 262144

    '    WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE Or WS_EX_CLIENTEDGE
    '    WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE Or WS_EX_TOOLWINDOW Or WS_EX_TOPMOST
    '    '#End If

    '    '#If (WIN32WINNT >= 500) Then
    '    WS_EX_LAYERED = 524288
    '    '#End If

    '    '#If (WINVER >= 500) Then
    '    WS_EX_NOINHERITLAYOUT = 1048576 ' Disable inheritence of mirroring by children
    '    WS_EX_LAYOUTRTL = 4194304 ' Right to left mirroring
    '    '#End If

    '    '#If (WIN32WINNT >= 500) Then
    '    WS_EX_COMPOSITED = 33554432
    '    WS_EX_NOACTIVATE = 67108864
    '    '#End If

    'End Enum

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
        With e.Graphics
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
                        ' Ab jetzt TelefonNamen verkleinern
                        If iTelNameLänge + iUhrzeitLänge > Länge Then
                            Dim sTest As String() = Split(sTelName, ", ", , CompareMethod.Text)
                            sTelName = vbNullString
                            For i = 0 To UBound(sTest) - 1
                                sTelName = sTelName & sTest(i) & ", "
                                If i = sTest.Length / 2 Then
                                    sTelName = sTelName & vbNewLine
                                End If
                            Next
                            sTelName = sTelName & sTest(UBound(sTest))
                        End If
                    End If
                End If
            End If

            .DrawString(sUhrzeit, Parent.TitleFont, New SolidBrush(Parent.TitleColor), iTitleOrigin + Parent.TextPadding.Left, Parent.TextPadding.Top + Parent.HeaderHeight)
            .DrawString(sTelName, Parent.TitleFont, New SolidBrush(Parent.TitleColor), Parent.Size.Width - Parent.TextPadding.Right - 21, Parent.TextPadding.Top + Parent.HeaderHeight, drawFormatRight)
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




