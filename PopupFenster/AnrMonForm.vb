Imports System.ComponentModel
Imports System.Drawing.Drawing2D

<System.ComponentModel.DefaultPropertyAttribute("Content"), System.ComponentModel.DesignTimeVisible(False)> _
Friend Class AnrMonForm
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
    Sub New(ByVal Parent As F_AnrMon)
        pnParent = Parent
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
    End Sub


    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'PopUpAnrMonForm
        '
        Me.ClientSize = New System.Drawing.Size(392, 66)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "PopUpAnrMonForm"
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

    Public Event LinkClick(ByVal sender As Object, ByVal e As System.EventArgs)
    Public Event CloseClick(ByVal sender As Object, ByVal e As System.EventArgs)

#Region "Properties"
    Protected Overrides ReadOnly Property ShowWithoutActivation() As Boolean
        Get
            Return True
        End Get
    End Property
    Private pnParent As F_AnrMon
    Shadows Property Parent() As F_AnrMon
        Get
            Return pnParent
        End Get
        Set(ByVal value As F_AnrMon)
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
            If Parent.Image IsNot Nothing Then
                Return New RectangleF(Parent.ImagePosition.X + Parent.ImageSize.Width + Parent.TextPadding.Left, CSng(Parent.TextPadding.Top + iHeightOfTitle + 1.5 * Parent.HeaderHeight), Me.Width - Parent.ImageSize.Width - Parent.ImagePosition.X - Parent.TextPadding.Left - Parent.TextPadding.Right, iHeightOfTelNr)
            Else
                Return New RectangleF(Parent.TextPadding.Left, CSng(Parent.TextPadding.Top + iHeightOfTitle + 1.5 * Parent.HeaderHeight), Me.Width - Parent.TextPadding.Left - Parent.TextPadding.Right, iHeightOfTelNr)
            End If
        End Get
    End Property

    Private ReadOnly Property RectAnrName() As RectangleF

        Get
            If Parent.Image IsNot Nothing Then
                Return New RectangleF(Parent.ImagePosition.X + Parent.ImageSize.Width + Parent.TextPadding.Left, _
                                      CSng(Parent.TextPadding.Top + iHeightOfTitle + 1.5 * Parent.HeaderHeight + iHeightOfTelNr), _
                                      Me.Width - Parent.ImageSize.Width - Parent.ImagePosition.X - Parent.TextPadding.Left - Parent.TextPadding.Right, iHeightOfAnrName)
            Else
                Return New RectangleF(Parent.TextPadding.Left, CSng(Parent.TextPadding.Top + iHeightOfTitle + 1.5 * Parent.HeaderHeight + iHeightOfTelNr), _
                                      Me.Width - Parent.TextPadding.Left - Parent.TextPadding.Right, iHeightOfAnrName)
            End If
        End Get
    End Property

    Private ReadOnly Property RectFirma() As RectangleF

        Get
            If Parent.Image IsNot Nothing Then
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
            If Parent.Image IsNot Nothing Then
                Return New Rectangle(Parent.ImagePosition, Parent.ImageSize)
            End If
        End Get
    End Property

#End Region

#Region "Events"

    Private Sub AnrMonForm_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        Me.Finalize()
    End Sub

    Private Sub Me_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
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

    Private Sub Me_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        If RectClose.Contains(e.X, e.Y) Then
            RaiseEvent CloseClick(Me, EventArgs.Empty)
        End If
        If RectAnrName.Contains(e.X, e.Y) Then
            RaiseEvent LinkClick(Me, EventArgs.Empty)
        End If
        If RectOptions.Contains(e.X, e.Y) Then
            If Parent.OptionsMenu IsNot Nothing Then
                Parent.OptionsMenu.Show(Me, New Point(RectOptions.Right - Parent.OptionsMenu.Width, RectOptions.Bottom))
                Parent.bShouldRemainVisible = True
            End If
        End If
    End Sub

    Private Sub Me_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
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
            iHeightOfTitle = CInt(.MeasureString("A", Parent.TitleFont).Height)
            iHeightOfAnrName = CInt(.MeasureString("A", Parent.ContentFont).Height)
            iHeightOfTelNr = CInt(.MeasureString("A", Parent.TelNrFont).Height)
            iTitleOrigin = Parent.TextPadding.Left
            If Parent.Image IsNot Nothing Then
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
            iTelNameLänge = CInt(.MeasureString(sTelName, Parent.TitleFont).Width)
            iUhrzeitLänge = CInt(.MeasureString(sUhrzeit, Parent.TitleFont).Width)
            If iTelNameLänge + iUhrzeitLänge > Länge Then
                sUhrzeit = CDate(Parent.Uhrzeit).ToString("dddd, dd. MMM. yy HH:mm:ss")
                iUhrzeitLänge = CInt(.MeasureString(sUhrzeit, Parent.TitleFont).Width)
                If iTelNameLänge + iUhrzeitLänge > Länge Then
                    sUhrzeit = CDate(Parent.Uhrzeit).ToString("ddd, dd.MM.yy HH:mm:ss")
                    iUhrzeitLänge = CInt(.MeasureString(sUhrzeit, Parent.TitleFont).Width)
                    If iTelNameLänge + iUhrzeitLänge > Länge Then
                        sUhrzeit = CDate(Parent.Uhrzeit).ToString("dd.MM.yy HH:mm:ss")
                        iUhrzeitLänge = CInt(.MeasureString(sUhrzeit, Parent.TitleFont).Width)
                    End If
                End If
            End If
            RectZeit = New RectangleF(iTitleOrigin + Parent.TextPadding.Left, Parent.TextPadding.Top + Parent.HeaderHeight, .MeasureString(sUhrzeit, Parent.TitleFont).Width, iHeightOfTitle)
            RectTelName = New RectangleF(RectZeit.Right, RectZeit.Top, RectClose.Left - RectZeit.Right, iHeightOfTitle)

            .DrawString(sUhrzeit, Parent.TitleFont, New SolidBrush(Parent.TitleColor), RectZeit)
            If iTelNameLänge > RectTelName.Width Then
                RectTelName.Y = Parent.HeaderHeight
                RectTelName.Size = New Size(CInt(RectTelName.Width), CInt(RectTelName.Height * 2 - 3))
            End If
            .DrawString(sTelName, Parent.TitleFont, New SolidBrush(Parent.TitleColor), RectTelName, drawFormatRight)
            .DrawString(Parent.TelNr, Parent.TelNrFont, New SolidBrush(Parent.TitleColor), RectTelNr, drawFormatCenter)
            .DrawString(Parent.Firma, Parent.TitleFont, New SolidBrush(Parent.TitleColor), RectFirma, drawFormatCenter)

            Dim tempfont As New Font("Microsoft Sans Serif", 16, FontStyle.Regular)
            Dim sAnrName As String
            sAnrName = Parent.AnrName
            iAnrNameLänge = CInt(.MeasureString(sAnrName, tempfont, 0, StringFormat.GenericTypographic).Width)

            If iAnrNameLänge > RectAnrName.Width Then
                Dim iFontSize As Integer
                iFontSize = CInt(((RectAnrName.Width - Parent.TextPadding.Right - Parent.TextPadding.Left) * (tempfont.Size / 72 * .DpiX - 1.5 * Parent.TextPadding.Top)) / (iAnrNameLänge - 2 * Parent.TextPadding.Left))
                iFontSize = CInt(IIf(iFontSize < 8, 8, iFontSize))
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

    Protected Overrides Sub Finalize()
        Me.Hide()
        MyBase.Finalize()
    End Sub
End Class

<DefaultEvent("LinkClick")> Public Class F_AnrMon
    Inherits Component

    Public Event LinkClick(ByVal sender As Object, ByVal e As System.EventArgs)
    Public Event Close(ByVal sender As Object, ByVal e As System.EventArgs)
    Public Event Closed(ByVal sender As Object, ByVal e As System.EventArgs)

    Public Event ToolStripMenuItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs)

    Private WithEvents fPopup As New AnrMonForm(Me)
    Private WithEvents tmAnimation As New Timer
    Private WithEvents tmWait As New Timer

    Private bAppearing As Boolean = True
    Public bShouldRemainVisible As Boolean = False
    Private i As Integer = 0

    Private bMouseIsOn As Boolean = False
    Private iMaxPosition As Integer
    Private dMaxOpacity As Double
    Private dummybool As Boolean

    Private CompContainer As New System.ComponentModel.Container()
    Private WithEvents AnrMonContextMenuStrip As New ContextMenuStrip(CompContainer)
    Private ToolStripMenuItemKontaktöffnen As New ToolStripMenuItem()
    Private ToolStripMenuItemRückruf As New ToolStripMenuItem()
    Private ToolStripMenuItemKopieren As New ToolStripMenuItem()

    Enum eStartPosition
        BottomRight
        BottomLeft
        TopLeft
        TopRight
    End Enum

    Enum eMoveDirection
        Y
        X
    End Enum

#Region "Properties"

    Private clHeader As Color = SystemColors.ControlDarkDark 'SystemColors.ControlDark
    <Category("Header"), DefaultValue(GetType(Color), "ControlDark")> _
    Property HeaderColor() As Color
        Get
            Return clHeader
        End Get
        Set(ByVal value As Color)
            clHeader = value

        End Set
    End Property

    Private clBody As Color = SystemColors.Control
    <Category("Appearance"), DefaultValue(GetType(Color), "Control")> _
    Property BodyColor() As Color
        Get
            Return clBody
        End Get
        Set(ByVal value As Color)
            clBody = value

        End Set
    End Property

    Private clTitle As Color = System.Drawing.SystemColors.ControlText 'Color.Gray
    <Category("Title"), DefaultValue(GetType(Color), "Gray")> _
    Property TitleColor() As Color
        Get
            Return clTitle
        End Get
        Set(ByVal value As Color)
            clTitle = value

        End Set
    End Property

    Private clBase As Color = SystemColors.ControlText
    <Category("Content"), DefaultValue(GetType(Color), "ControlText")> _
    Property ContentColor() As Color
        Get
            Return clBase
        End Get
        Set(ByVal value As Color)
            clBase = value

        End Set
    End Property

    Private clBorder As Color = SystemColors.WindowText 'SystemColors.WindowFrame
    <Category("Appearance"), DefaultValue(GetType(Color), "WindowText")> _
    Property BorderColor() As Color
        Get
            Return clBorder
        End Get
        Set(ByVal value As Color)
            clBorder = value
        End Set
    End Property

    Private clCloseBorder As Color = SystemColors.WindowFrame
    <Category("Buttons"), DefaultValue(GetType(Color), "WindowFrame")> _
    Property ButtonBorderColor() As Color
        Get
            Return clCloseBorder
        End Get
        Set(ByVal value As Color)
            clCloseBorder = value
        End Set
    End Property

    Private clCloseHover As Color = Color.Orange 'SystemColors.Highlight
    <Category("Buttons"), DefaultValue(GetType(Color), "Highlight")> _
    Property ButtonHoverColor() As Color
        Get
            Return clCloseHover
        End Get
        Set(ByVal value As Color)
            clCloseHover = value
        End Set
    End Property

    Private clLinkHover As Color = SystemColors.Highlight 'SystemColors.HotTrack
    <Category("Appearance"), DefaultValue(GetType(Color), "HotTrack")> _
    Property LinkHoverColor() As Color
        Get
            Return clLinkHover
        End Get
        Set(ByVal value As Color)
            clLinkHover = value

        End Set
    End Property

    Private iDiffGradient As Integer = 50
    <Category("Appearance"), DefaultValue(50)> _
    Property GradientPower() As Integer
        Get
            Return iDiffGradient
        End Get
        Set(ByVal value As Integer)
            iDiffGradient = value

        End Set
    End Property

    Private ftBase As Font = New Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)) 'SystemFonts.DialogFont
    <Category("Anrufername")> Property ContentFont() As Font
        Get
            Return ftBase
        End Get
        Set(ByVal value As Font)
            ftBase = value
        End Set
    End Property

    Private ftTitle As Font = New Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)) 'SystemFonts.CaptionFont
    <Category("Title")> _
    Property TitleFont() As Font
        Get
            Return ftTitle
        End Get
        Set(ByVal value As Font)
            ftTitle = value

        End Set
    End Property

    Private pdTextPadding As Padding = New Padding(5) ' New Padding(0)
    <Category("Appearance")> _
    Property TextPadding() As Padding
        Get
            Return pdTextPadding
        End Get
        Set(ByVal value As Padding)
            pdTextPadding = value

        End Set
    End Property

    Private iHeaderHeight As Integer = 9
    <Category("Header"), DefaultValue(9)> _
    Property HeaderHeight() As Integer
        Get
            Return iHeaderHeight
        End Get
        Set(ByVal value As Integer)
            iHeaderHeight = value

        End Set
    End Property

    Private bCloseButtonVisible As Boolean = True
    <Category("Buttons"), DefaultValue(True)> _
    Property CloseButton() As Boolean
        Get
            Return bCloseButtonVisible
        End Get
        Set(ByVal value As Boolean)
            bCloseButtonVisible = value
        End Set
    End Property

    Private bOptionsButtonVisible As Boolean = False
    <Category("Buttons"), DefaultValue(False)> _
    Property OptionsButton() As Boolean
        Get
            Return bOptionsButtonVisible
        End Get
        Set(ByVal value As Boolean)
            bOptionsButtonVisible = value

        End Set
    End Property

    Private WithEvents ctContextMenu As ContextMenuStrip = Nothing
    <Category("Behavior")> _
    Property OptionsMenu() As ContextMenuStrip
        Get
            Return ctContextMenu
        End Get
        Set(ByVal value As ContextMenuStrip)
            ctContextMenu = value
        End Set
    End Property

    Private iShowDelay As Integer = 3000
    <Category("Behavior"), DefaultValue(3000)> _
    Property ShowDelay() As Integer
        Get
            Return iShowDelay
        End Get
        Set(ByVal value As Integer)
            iShowDelay = value
        End Set
    End Property

    Private szSize As Size = New Size(400, 100)
    <Category("Appearance")> _
    Property Size() As Size
        Get
            Return szSize
        End Get
        Set(ByVal value As Size)
            szSize = value
        End Set
    End Property

    Private bAutoAusblenden As Boolean = True
    <Category("Appearance"), DefaultValue(True)> _
    Property AutoAusblenden() As Boolean
        Get
            Return bAutoAusblenden
        End Get
        Set(ByVal value As Boolean)
            bAutoAusblenden = value
        End Set
    End Property

    Private szPosition As Size = New Size(0, 0)
    <Category("Appearance")> _
    Property PositionsKorrektur() As Size
        Get
            Return szPosition
        End Get
        Set(ByVal value As Size)
            szPosition = value
        End Set
    End Property

    Private bEffektTransparenz As Boolean = True
    <Category("Appearance"), _
    DefaultValue(True)> _
    Property EffektTransparenz() As Boolean
        Get
            Return bEffektTransparenz
        End Get
        Set(ByVal value As Boolean)
            bEffektTransparenz = value
        End Set
    End Property

    Private bEffektMove As Boolean = True
    <Category("Appearance"), _
    DefaultValue(True)> _
    Property EffektMove() As Boolean
        Get
            Return bEffektMove
        End Get
        Set(ByVal value As Boolean)
            bEffektMove = value
        End Set
    End Property

    Private iEffektMoveGeschwindigkeit As Integer = 5
    <Category("Anrufmonitor"), DefaultValue(5)> _
    Property EffektMoveGeschwindigkeit() As Integer
        Get
            Return iEffektMoveGeschwindigkeit
        End Get
        Set(ByVal value As Integer)
            iEffektMoveGeschwindigkeit = value
        End Set
    End Property

    Private pStartpunkt As eStartPosition
    <Category("Anrufmonitor")> _
    Property Startpunkt() As eStartPosition
        Get
            Return pStartpunkt
        End Get
        Set(ByVal value As eStartPosition)
            pStartpunkt = value
        End Set
    End Property

    Private _MoveDirection As eMoveDirection
    <Category("Anrufmonitor")> _
    Property MoveDirecktion() As eMoveDirection
        Get
            Return _MoveDirection
        End Get
        Set(ByVal value As eMoveDirection)
            _MoveDirection = value
        End Set
    End Property

    Private ftTelNr As Font = New Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte)) 'SystemFonts.CaptionFont
    <Category("Anrufmonitor")> Property TelNrFont() As Font
        Get
            Return ftTelNr
        End Get
        Set(ByVal value As Font)
            ftTelNr = value

        End Set
    End Property

    Private ptImagePosition As Point = New Point(12, 32) 'New Point(12, 21)
    <Category("Anrufmonitor")> _
    Property ImagePosition() As Point
        Get
            Return ptImagePosition
        End Get
        Set(ByVal value As Point)
            ptImagePosition = value

        End Set
    End Property

    Private szImageSize As Size = New Size(48, 48) 'New Size(0, 0)
    <Category("Anrufmonitor")> _
    Property ImageSize() As Size
        Get
            If szImageSize.Width = 0 Then
                If Image IsNot Nothing Then
                    Return Image.Size
                Else
                    Return New Size(32, 32)
                End If
            Else
                Return szImageSize
            End If
        End Get
        Set(ByVal value As Size)
            szImageSize = value
        End Set
    End Property

    Private imImage As Image = Nothing
    <Category("Anrufmonitor")> _
    Property Image() As Image
        Get
            Return imImage
        End Get
        Set(ByVal value As Image)
            imImage = value
        End Set
    End Property

    Private sAnrName As String
    <Category("Anrufmonitor")> _
    Property AnrName() As String
        Get
            Return sAnrName
        End Get
        Set(ByVal value As String)
            sAnrName = value
        End Set
    End Property

    Private sUhrzeit As String
    <Category("Anrufmonitor")> _
    Property Uhrzeit() As String
        Get
            Return sUhrzeit
        End Get
        Set(ByVal value As String)
            sUhrzeit = value
        End Set
    End Property

    Private sTelNr As String
    <Category("Anrufmonitor")> _
    Property TelNr() As String
        Get
            Return sTelNr
        End Get
        Set(ByVal value As String)
            sTelNr = value
        End Set
    End Property

    Private sTelName As String
    <Category("Anrufmonitor")> _
    Property TelName() As String
        Get
            Return sTelName
        End Get
        Set(ByVal value As String)
            sTelName = value
        End Set
    End Property

    Private sFirma As String
    <Category("Anrufmonitor")> _
    Property Firma() As String
        Get
            Return sFirma
        End Get
        Set(ByVal value As String)
            sFirma = value
        End Set
    End Property

#End Region

    Public Sub New()
        With fPopup
            .FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            .StartPosition = System.Windows.Forms.FormStartPosition.Manual
            .ShowInTaskbar = True
        End With
        InitializeComponentContextMenuStrip()
    End Sub

    Public Sub Popup()
        Dim X As Integer
        Dim Y As Integer
        Dim retVal As Boolean

        tmWait.Interval = 200
        With fPopup
            .TopMost = True
            .Size = Size
            .Opacity = CDbl(IIf(bEffektTransparenz, 0, 1))


            Select Case Startpunkt
                Case eStartPosition.BottomLeft
                    X = Screen.PrimaryScreen.WorkingArea.Left + 10 - PositionsKorrektur.Width
                    Y = Screen.PrimaryScreen.WorkingArea.Bottom - 10 - .Height - PositionsKorrektur.Height
                Case eStartPosition.TopLeft
                    X = Screen.PrimaryScreen.WorkingArea.Left + 10 - PositionsKorrektur.Width
                    Y = Screen.PrimaryScreen.WorkingArea.Top + 10 - PositionsKorrektur.Height
                Case eStartPosition.BottomRight
                    X = Screen.PrimaryScreen.WorkingArea.Right - 10 - .Size.Width - PositionsKorrektur.Width
                    Y = Screen.PrimaryScreen.WorkingArea.Bottom - 10 - .Height - PositionsKorrektur.Height
                Case eStartPosition.TopRight
                    X = Screen.PrimaryScreen.WorkingArea.Right - 10 - .Size.Width - PositionsKorrektur.Width
                    Y = Screen.PrimaryScreen.WorkingArea.Top + 10 - PositionsKorrektur.Height
            End Select

            If bEffektMove Then
                Select Case MoveDirecktion
                    Case eMoveDirection.X
                        Select Case Startpunkt
                            Case eStartPosition.BottomLeft, eStartPosition.TopLeft
                                X = Screen.PrimaryScreen.WorkingArea.Left - fPopup.Size.Width + 2
                            Case eStartPosition.BottomRight, eStartPosition.TopRight
                                X = Screen.PrimaryScreen.WorkingArea.Right + 2
                        End Select
                    Case eMoveDirection.Y
                        Select Case Startpunkt
                            Case eStartPosition.TopLeft, eStartPosition.TopRight
                                Y = Screen.PrimaryScreen.WorkingArea.Top - fPopup.Height + 1
                            Case eStartPosition.BottomRight, eStartPosition.BottomLeft
                                Y = Screen.PrimaryScreen.WorkingArea.Bottom - 1
                        End Select
                End Select

            End If

            .Location = New Point(X, Y)
            .Text = AnrName & CStr(IIf(TelNr = "", "", " (" & TelNr & ")"))

            retVal = OutlookSecurity.SetWindowPos(.Handle, hWndInsertAfterFlags.HWND_TOPMOST, 0, 0, 0, 0, _
                                                  CType(SetWindowPosFlags.DoNotActivate + _
                                                  SetWindowPosFlags.IgnoreMove + _
                                                  SetWindowPosFlags.IgnoreResize + _
                                                  SetWindowPosFlags.DoNotChangeOwnerZOrder, SetWindowPosFlags))

            .Show()
        End With

        tmAnimation.Interval = iEffektMoveGeschwindigkeit
        tmAnimation.Start()
    End Sub

    ''' <summary>
    ''' Initialisierungsroutine des ehemaligen AnrMonForm. Es wird das ContextMenuStrip an Sich initialisiert 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeComponentContextMenuStrip()
        '
        'ContextMenuStrip
        '
        With Me.AnrMonContextMenuStrip
            .Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemKontaktöffnen, Me.ToolStripMenuItemRückruf, Me.ToolStripMenuItemKopieren})
            .Name = "AnrMonContextMenuStrip"
            .RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            .Size = New System.Drawing.Size(222, 70)
        End With
        '
        'ToolStripMenuItemKontaktöffnen
        '
        With Me.ToolStripMenuItemKontaktöffnen
            '.Image = ToolStripMenuItemKontaktöffnenImage
            .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
            .Name = "ToolStripMenuItemKontaktöffnen"
            .Size = New System.Drawing.Size(221, 22)
            '.Text = ToolStripMenuItemKontaktöffnenText '"Kontakt öffnen"
        End With
        '
        'ToolStripMenuItemRückruf
        '
        With Me.ToolStripMenuItemRückruf
            '.Image = ToolStripMenuItemKontaktöffnenImage
            .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
            .Name = "ToolStripMenuItemRückruf"
            .Size = New System.Drawing.Size(221, 22)
            '.Text = ToolStripMenuItemRückrufText '"Rückruf"
        End With
        '
        'ToolStripMenuItemKopieren
        '
        With Me.ToolStripMenuItemKopieren
            '.Image = ToolStripMenuItemKopierenImage
            .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
            .Name = "ToolStripMenuItemKopieren"
            .Size = New System.Drawing.Size(221, 22)
            '.Text = ToolStripMenuItemKopierenText '"In Zwischenablage kopieren"
        End With
        Me.OptionsMenu = Me.AnrMonContextMenuStrip
    End Sub

    Public Sub Hide()
        bMouseIsOn = False
        tmWait.Stop()
        tmAnimation.Start()
    End Sub

#Region "Eigene Events"
    Private Sub fPopup_CloseClick() Handles fPopup.CloseClick
        RaiseEvent Close(Me, EventArgs.Empty)
        Me.Finalize()
    End Sub

    Private Sub fPopup_LinkClick() Handles fPopup.LinkClick
        RaiseEvent LinkClick(Me, EventArgs.Empty)
    End Sub
    Private Sub fPopup_ToolStripMenuItemRückrufClickClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ctContextMenu.ItemClicked
        RaiseEvent ToolStripMenuItemClicked(Me, e)
    End Sub
#End Region

    Private Function GetOpacityBasedOnPosition() As Double

        Dim iCentPurcent As Integer
        Dim iCurrentlyShown As Integer
        Dim dPourcentOpacity As Double

        Select Case MoveDirecktion
            Case eMoveDirection.X
                iCentPurcent = fPopup.Width
                Select Case Startpunkt
                    Case eStartPosition.BottomLeft, eStartPosition.TopLeft
                        iCurrentlyShown = fPopup.Right
                    Case eStartPosition.BottomRight, eStartPosition.TopRight
                        iCurrentlyShown = Screen.PrimaryScreen.WorkingArea.Width - fPopup.Left
                End Select
                dPourcentOpacity = iCurrentlyShown * 100 / iCentPurcent
            Case eMoveDirection.Y
                iCentPurcent = fPopup.Height
                Select Case Startpunkt
                    Case eStartPosition.BottomLeft, eStartPosition.BottomRight
                        iCurrentlyShown = Screen.PrimaryScreen.WorkingArea.Height - fPopup.Top
                    Case eStartPosition.TopLeft, eStartPosition.TopRight
                        iCurrentlyShown = fPopup.Bottom
                End Select
                dPourcentOpacity = iCentPurcent / 100 * iCurrentlyShown
        End Select

        Return dPourcentOpacity / 100
    End Function

    Private Sub tmAnimation_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmAnimation.Tick
        Dim StoppAnimation As Boolean = False
        With fPopup
            .Invalidate()
            If bEffektMove Then
                If bAppearing Then 'Einblenden
                    Select Case MoveDirecktion
                        Case eMoveDirection.X
                            Select Case Startpunkt
                                Case eStartPosition.BottomLeft, eStartPosition.TopLeft
                                    .Left += 2
                                    StoppAnimation = .Left = Screen.PrimaryScreen.WorkingArea.Left + 10 - PositionsKorrektur.Width
                                Case eStartPosition.BottomRight, eStartPosition.TopRight
                                    .Left -= 2
                                    StoppAnimation = .Left = Screen.PrimaryScreen.WorkingArea.Right - fPopup.Size.Width - 10 - PositionsKorrektur.Width
                            End Select
                        Case eMoveDirection.Y
                            Select Case Startpunkt
                                Case eStartPosition.BottomLeft, eStartPosition.BottomRight
                                    .Top -= 1
                                    StoppAnimation = .Top + .Height = Screen.PrimaryScreen.WorkingArea.Bottom - 10 - PositionsKorrektur.Height
                                Case eStartPosition.TopLeft, eStartPosition.TopRight
                                    .Top += 1
                                    StoppAnimation = .Top = Screen.PrimaryScreen.WorkingArea.Top + 10 - PositionsKorrektur.Height
                            End Select
                    End Select

                    If StoppAnimation Then
                        tmAnimation.Stop()
                        bAppearing = False
                        iMaxPosition = .Top
                        dMaxOpacity = .Opacity
                        If bAutoAusblenden Then tmWait.Start()
                    End If

                    Try
                        .Opacity = CDbl(IIf(bEffektTransparenz, GetOpacityBasedOnPosition(), 1))
                    Catch : End Try


                Else 'Ausblenden
                    If bMouseIsOn Then
                        .Top = iMaxPosition
                        .Opacity = dMaxOpacity
                        tmAnimation.Stop()
                        tmWait.Start()
                    Else
                        Select Case MoveDirecktion
                            Case eMoveDirection.X
                                Select Case Startpunkt
                                    Case eStartPosition.BottomLeft, eStartPosition.TopLeft
                                        .Left -= 2
                                        StoppAnimation = .Right < Screen.PrimaryScreen.WorkingArea.Left
                                    Case eStartPosition.BottomRight, eStartPosition.TopRight
                                        .Left += 2
                                        StoppAnimation = .Left > Screen.PrimaryScreen.WorkingArea.Right
                                End Select
                            Case eMoveDirection.Y
                                Select Case Startpunkt
                                    Case eStartPosition.BottomLeft, eStartPosition.BottomRight
                                        .Top += 1
                                        StoppAnimation = .Top > Screen.PrimaryScreen.WorkingArea.Bottom - PositionsKorrektur.Width
                                    Case eStartPosition.TopLeft, eStartPosition.TopRight
                                        .Top -= 1
                                        StoppAnimation = .Bottom < Screen.PrimaryScreen.WorkingArea.Top - PositionsKorrektur.Width
                                End Select
                        End Select

                        If StoppAnimation Then
                            tmAnimation.Stop()
                            .TopMost = False
                            .Close()
                            bAppearing = True
                            RaiseEvent Closed(Me, EventArgs.Empty)
                        End If

                        .Opacity = CDbl(IIf(bEffektTransparenz, GetOpacityBasedOnPosition(), 1))
                    End If
                End If
            Else
                If bAppearing Then
                    .Opacity += CDbl(IIf(bEffektTransparenz, 0.05, 1))
                    If .Opacity = 1 Then
                        tmAnimation.Stop()
                        bAppearing = False
                        If bAutoAusblenden Then tmWait.Start()
                    End If
                Else
                    If bMouseIsOn Then
                        fPopup.Opacity = 1
                        tmAnimation.Stop()
                        tmWait.Start()
                    Else
                        .Opacity -= CDbl(IIf(bEffektTransparenz, 0.05, 1))
                        If .Opacity = 0 Then
                            tmAnimation.Stop()
                            .TopMost = False
                            .Close()
                            bAppearing = True
                            RaiseEvent Closed(Me, EventArgs.Empty)
                        End If
                    End If
                End If
            End If
        End With
    End Sub

    Private Sub tmWait_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmWait.Tick
        i += tmWait.Interval
        If i = ShowDelay Then
            tmWait.Stop()
            tmAnimation.Start()
        End If
        fPopup.Invalidate()

    End Sub

    Private Sub fPopup_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles fPopup.MouseEnter
        bMouseIsOn = True
    End Sub

    Private Sub fPopup_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles fPopup.MouseLeave
        If Not bShouldRemainVisible Then bMouseIsOn = False
    End Sub

    Private Sub ctContextMenu_Closed(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripDropDownClosedEventArgs) Handles ctContextMenu.Closed
        bShouldRemainVisible = False
        bMouseIsOn = False
        tmAnimation.Start()
    End Sub

End Class