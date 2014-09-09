Friend Class CommonFenster

#Region "Enum"
    'Enum eStartPosition
    '    BottomRight
    '    BottomLeft
    '    TopLeft
    '    TopRight
    'End Enum

    'Enum eMoveDirection
    '    Y
    '    X
    'End Enum
#End Region

#Region "DefaultFont"
    Private sDefFontName As String = "Microsoft Sans Serif"
    Property DefFontName() As String
        Get
            Return sDefFontName
        End Get
        Set(ByVal value As String)
            sDefFontName = value
        End Set
    End Property

    Private sDefFontStyle As FontStyle = FontStyle.Regular
    Property DefFontStyle() As FontStyle
        Get
            Return sDefFontStyle
        End Get
        Set(ByVal value As FontStyle)
            sDefFontStyle = value
        End Set
    End Property

    Private sDefGraphicsUnit As GraphicsUnit = GraphicsUnit.Point
    Property DefGraphicsUnit() As GraphicsUnit
        Get
            Return sDefGraphicsUnit
        End Get
        Set(ByVal value As GraphicsUnit)
            sDefGraphicsUnit = value
        End Set
    End Property

    Private sDefgdiCharSet As Byte = CType(0, Byte)
    Property DefgdiCharSet() As Byte
        Get
            Return sDefgdiCharSet
        End Get
        Set(ByVal value As Byte)
            sDefgdiCharSet = value
        End Set
    End Property
#End Region

#Region "Properties Stoppuhr"

    Private clHeader As Color = SystemColors.ControlDarkDark
    Property HeaderColor() As Color
        Get
            Return clHeader
        End Get
        Set(ByVal value As Color)
            clHeader = value
        End Set
    End Property

    Private clBody As Color = SystemColors.Control
    Property BodyColor() As Color
        Get
            Return clBody
        End Get
        Set(ByVal value As Color)
            clBody = value
        End Set
    End Property

    Private clTitle As Color = SystemColors.ControlText
    Property TitleColor() As Color
        Get
            Return clTitle
        End Get
        Set(ByVal value As Color)
            clTitle = value
        End Set
    End Property

    Private clBase As Color = SystemColors.ControlText
    Property ContentColor() As Color
        Get
            Return clBase
        End Get
        Set(ByVal value As Color)
            clBase = value
        End Set
    End Property

    Private clBorder As Color = SystemColors.WindowFrame
    Property BorderColor() As Color
        Get
            Return clBorder
        End Get
        Set(ByVal value As Color)
            clBorder = value
        End Set
    End Property

    Private clCloseBorder As Color = SystemColors.WindowFrame
    Property ButtonBorderColor() As Color
        Get
            Return clCloseBorder
        End Get
        Set(ByVal value As Color)
            clCloseBorder = value
        End Set
    End Property

    Private clCloseHover As Color = SystemColors.Highlight
    Property ButtonHoverColor() As Color
        Get
            Return clCloseHover
        End Get
        Set(ByVal value As Color)
            clCloseHover = value
        End Set
    End Property

    Private iDiffGradient As Integer = 50
    Property GradientPower() As Integer
        Get
            Return iDiffGradient
        End Get
        Set(ByVal value As Integer)
            iDiffGradient = value
        End Set
    End Property

    Private ftSUBase As Font = New Font(DefFontName, 18.0!, DefFontStyle, sDefGraphicsUnit, DefgdiCharSet)
    Property fSUContentFont() As Font
        Get

            Return ftSUBase
        End Get
        Set(ByVal value As Font)
            ftSUBase = value
        End Set
    End Property

    Private pdTextPadding As Padding = New Padding(5)
    Property TextPadding() As Padding
        Get
            Return pdTextPadding
        End Get
        Set(ByVal value As Padding)
            pdTextPadding = value
        End Set
    End Property

    Private iHeaderHeight As Integer = 9
    Property HeaderHeight() As Integer
        Get
            Return iHeaderHeight
        End Get
        Set(ByVal value As Integer)
            iHeaderHeight = value
        End Set
    End Property

#End Region

#Region "Properties Anrufmonitor"

    Private clLinkHover As Color = SystemColors.Highlight 'SystemColors.HotTrack
    Property LinkHoverColor() As Color
        Get
            Return clLinkHover
        End Get
        Set(ByVal value As Color)
            clLinkHover = value

        End Set
    End Property

    Private bCloseButtonVisible As Boolean = True
    Property CloseButton() As Boolean
        Get
            Return bCloseButtonVisible
        End Get
        Set(ByVal value As Boolean)
            bCloseButtonVisible = value
        End Set
    End Property

    Private bOptionsButtonVisible As Boolean = True
    Property OptionsButton() As Boolean
        Get
            Return bOptionsButtonVisible
        End Get
        Set(ByVal value As Boolean)
            bOptionsButtonVisible = value
        End Set
    End Property

    Private ftBase As Font = New Font(DefFontName, 15.75!, DefFontStyle, sDefGraphicsUnit, DefgdiCharSet) 'SystemFonts.DialogFont
    Property ContentFont() As Font
        Get
            Return ftBase
        End Get
        Set(ByVal value As Font)
            ftBase = value
        End Set
    End Property

    Private ftTitle As Font = New Font(DefFontName, 8.25!, DefFontStyle, sDefGraphicsUnit, DefgdiCharSet) 'SystemFonts.CaptionFont
    Property TitleFont() As Font
        Get
            Return ftTitle
        End Get
        Set(ByVal value As Font)
            ftTitle = value
        End Set
    End Property

    Private iEffektMoveGeschwindigkeit As Integer = 5
    Property EffektMoveGeschwindigkeit() As Integer
        Get
            Return iEffektMoveGeschwindigkeit
        End Get
        Set(ByVal value As Integer)
            iEffektMoveGeschwindigkeit = value
        End Set
    End Property

    'Private pStartpunkt As eStartPosition
    'Property Startpunkt() As eStartPosition
    '    Get
    '        Return pStartpunkt
    '    End Get
    '    Set(ByVal value As eStartPosition)
    '        pStartpunkt = value
    '    End Set
    'End Property

    'Private _MoveDirection As eMoveDirection
    'Property MoveDirecktion() As eMoveDirection
    '    Get
    '        Return _MoveDirection
    '    End Get
    '    Set(ByVal value As eMoveDirection)
    '        _MoveDirection = value
    '    End Set
    'End Property

    Private ftTelNr As Font = New Font(DefFontName, 11.25!, DefFontStyle, sDefGraphicsUnit, DefgdiCharSet) 'SystemFonts.CaptionFont
    Property TelNrFont() As Font
        Get
            Return ftTelNr
        End Get
        Set(ByVal value As Font)
            ftTelNr = value

        End Set
    End Property

    Private ptImagePosition As Point = New Point(12, 32) 'New Point(12, 21)
    Property ImagePosition() As Point
        Get
            Return ptImagePosition
        End Get
        Set(ByVal value As Point)
            ptImagePosition = value

        End Set
    End Property

    Private szImageSize As Size = New Size(48, 48) 'New Size(0, 0)
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
    Property Image() As Image
        Get
            Return imImage
        End Get
        Set(ByVal value As Image)
            imImage = value
        End Set
    End Property

    Private sAnrName As String
    Property AnrName() As String
        Get
            Return sAnrName
        End Get
        Set(ByVal value As String)
            sAnrName = value
        End Set
    End Property

    Private sUhrzeit As String
    Property Uhrzeit() As String
        Get
            Return sUhrzeit
        End Get
        Set(ByVal value As String)
            sUhrzeit = value
        End Set
    End Property

    Private sTelNr As String
    Property TelNr() As String
        Get
            Return sTelNr
        End Get
        Set(ByVal value As String)
            sTelNr = value
        End Set
    End Property

    Private sTelName As String
    Property TelName() As String
        Get
            Return sTelName
        End Get
        Set(ByVal value As String)
            sTelName = value
        End Set
    End Property

    Private sFirma As String
    Property Firma() As String
        Get
            Return sFirma
        End Get
        Set(ByVal value As String)
            sFirma = value
        End Set
    End Property

#End Region

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

    Friend Function GetDarkerColor(ByVal Color As Color) As Color
        Dim clNew As Color
        clNew = Drawing.Color.FromArgb(255, DedValueMin0(CInt(Color.R), GradientPower), DedValueMin0(CInt(Color.G), GradientPower), DedValueMin0(CInt(Color.B), GradientPower))
        Return clNew
    End Function

    Friend Function GetLighterColor(ByVal Color As Color) As Color
        Dim clNew As Color
        clNew = Drawing.Color.FromArgb(255, AddValueMax255(CInt(Color.R), GradientPower), AddValueMax255(CInt(Color.G), GradientPower), AddValueMax255(CInt(Color.B), GradientPower))
        Return clNew
    End Function

    'Private Function GetLighterTransparentColor(ByVal Color As Color) As Color
    '    Dim clNew As Color
    '    clNew = Drawing.Color.FromArgb(0, AddValueMax255(CInt(Color.R), GradientPower), AddValueMax255(CInt(Color.G), GradientPower), AddValueMax255(CInt(Color.B), GradientPower))
    '    Return clNew
    'End Function

End Class

Friend Class Common_Form
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
    Private bMouseOnClose As Boolean = False
    Private bMouseOnLink As Boolean = False
    Private bMouseOnOptions As Boolean = False
    Private iHeightOfTitle As Integer
    Private iHeightOfAnrName As Integer
    Private iHeightOfTelNr As Integer
    Private iTitleOrigin As Integer

    Friend Event LinkClick(ByVal sender As Object, ByVal e As System.EventArgs)
    Friend Event CloseClick(ByVal sender As Object, ByVal e As System.EventArgs)

    Sub New(ByVal vAnrMon As F_AnrMon, ByVal vStoppuhr As F_StoppUhr, ByRef vCommon As CommonFenster)
        P_pnAnrMon = vAnrMon
        P_pnStoppuhr = vStoppuhr
        P_Common = vCommon

        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
    End Sub

    Private Sub InitializeComponent()
        Me.SuspendLayout()

        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual

        Me.Name = "FBDBForm"
        Me.ResumeLayout(True)
    End Sub

#Region "Properties"
    Protected Overrides ReadOnly Property ShowWithoutActivation() As Boolean
        Get
            Return True
        End Get
    End Property

    Private pnAnrMon As F_AnrMon
    Shadows Property P_pnAnrMon() As F_AnrMon
        Get
            Return pnAnrMon
        End Get
        Set(ByVal value As F_AnrMon)
            pnAnrMon = value
        End Set
    End Property

    Private pnStoppuhr As F_StoppUhr
    Shadows Property P_pnStoppuhr() As F_StoppUhr
        Get
            Return pnStoppuhr
        End Get
        Set(ByVal value As F_StoppUhr)
            pnStoppuhr = value
        End Set
    End Property

    Private pnCmn As CommonFenster
    Shadows Property P_Common() As CommonFenster
        Get
            Return pnCmn
        End Get
        Set(ByVal value As CommonFenster)
            pnCmn = value
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
    Private ReadOnly Property RectTelNr() As RectangleF
        Get
            If P_pnAnrMon.Image IsNot Nothing Then
                Return New RectangleF(P_pnAnrMon.ImagePosition.X + P_pnAnrMon.ImageSize.Width + P_Common.TextPadding.Left, CSng(P_Common.TextPadding.Top + iHeightOfTitle + 1.5 * P_Common.HeaderHeight), Me.Width - P_pnAnrMon.ImageSize.Width - P_pnAnrMon.ImagePosition.X - P_Common.TextPadding.Left - P_Common.TextPadding.Right, iHeightOfTelNr)
            Else
                Return New RectangleF(P_Common.TextPadding.Left, CSng(P_Common.TextPadding.Top + iHeightOfTitle + 1.5 * P_Common.HeaderHeight), Me.Width - P_Common.TextPadding.Left - P_Common.TextPadding.Right, iHeightOfTelNr)
            End If
        End Get
    End Property

    Private ReadOnly Property RectAnrName() As RectangleF
        Get
            If P_pnAnrMon.Image IsNot Nothing Then
                Return New RectangleF(P_pnAnrMon.ImagePosition.X + P_pnAnrMon.ImageSize.Width + P_Common.TextPadding.Left, _
                                      CSng(P_Common.TextPadding.Top + iHeightOfTitle + 1.5 * P_Common.HeaderHeight + iHeightOfTelNr), _
                                      Me.Width - P_pnAnrMon.ImageSize.Width - P_pnAnrMon.ImagePosition.X - P_Common.TextPadding.Left - P_Common.TextPadding.Right, iHeightOfAnrName)
            Else
                Return New RectangleF(P_Common.TextPadding.Left, CSng(P_Common.TextPadding.Top + iHeightOfTitle + 1.5 * P_Common.HeaderHeight + iHeightOfTelNr), _
                                      Me.Width - P_Common.TextPadding.Left - P_Common.TextPadding.Right, iHeightOfAnrName)
            End If
        End Get
    End Property

    Private ReadOnly Property RectFirma() As RectangleF
        Get
            If P_pnAnrMon.Image IsNot Nothing Then
                Return New RectangleF(P_pnAnrMon.ImagePosition.X + P_pnAnrMon.ImageSize.Width + P_Common.TextPadding.Left, Me.Height - P_Common.TextPadding.Bottom - iHeightOfTitle, Me.Width - P_pnAnrMon.ImageSize.Width - P_pnAnrMon.ImagePosition.X - P_Common.TextPadding.Left - P_Common.TextPadding.Right, iHeightOfTitle)
            Else
                Return New RectangleF(P_Common.TextPadding.Left, Me.Height - iHeightOfTitle - P_Common.TextPadding.Bottom, Me.Width - P_Common.TextPadding.Left - P_Common.TextPadding.Right, iHeightOfTitle)
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
            If P_pnAnrMon.Image IsNot Nothing Then
                Return New Rectangle(P_pnAnrMon.ImagePosition, P_pnAnrMon.ImageSize)
            End If
        End Get
    End Property
#End Region

#Region "Events"

    Private Sub Me_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        Me.Finalize()
    End Sub

    Private Sub Me_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        If P_Common.CloseButton Then
            If RectClose.Contains(e.X, e.Y) Then
                bMouseOnClose = True
            Else
                bMouseOnClose = False
            End If
        End If

        If P_pnAnrMon IsNot Nothing Then

            If P_Common.OptionsButton Then
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

        End If
        Invalidate()
    End Sub

    Private Sub Me_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        If RectClose.Contains(e.X, e.Y) Then
            RaiseEvent CloseClick(Me, EventArgs.Empty)
        End If

        If P_pnAnrMon IsNot Nothing Then
            If RectAnrName.Contains(e.X, e.Y) Then
                RaiseEvent LinkClick(Me, EventArgs.Empty)
            End If
            If RectOptions.Contains(e.X, e.Y) Then
                If P_pnAnrMon.OptionsMenu IsNot Nothing Then
                    P_pnAnrMon.OptionsMenu.Show(Me, New Point(RectOptions.Right - P_pnAnrMon.OptionsMenu.Width, RectOptions.Bottom))
                    P_pnAnrMon.bShouldRemainVisible = True
                End If
            End If
        End If
    End Sub

    Private Sub Me_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        If P_pnAnrMon IsNot Nothing Then AnrMon_Paint(sender, e)

        If P_pnStoppuhr IsNot Nothing Then StoppuhrPaint(sender, e)
    End Sub

    Private Sub AnrMon_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)
        Dim iTelNameLänge As Integer
        Dim iUhrzeitLänge As Integer
        Dim iAnrNameLänge As Integer
        Dim sUhrzeit As String
        Dim sTelName As String
        Dim Länge As Integer

        Dim rcBody As New Rectangle(0, 0, Me.Width, Me.Height)

        Dim rcHeader As New Rectangle(0, 0, Me.Width, P_Common.HeaderHeight)
        Dim rcForm As New Rectangle(0, 0, Me.Width - 1, Me.Height - 1)
        Dim brBody As New Drawing2D.LinearGradientBrush(rcBody, P_Common.BodyColor, P_Common.GetLighterColor(P_Common.BodyColor), Drawing2D.LinearGradientMode.Vertical)
        Dim drawFormatCenter As New StringFormat()
        Dim drawFormatRight As New StringFormat()

        Dim brHeader As New Drawing2D.LinearGradientBrush(rcHeader, P_Common.HeaderColor, P_Common.GetDarkerColor(P_Common.HeaderColor), Drawing2D.LinearGradientMode.Vertical)
        Dim RectZeit As RectangleF
        Dim RectTelName As RectangleF

        drawFormatCenter.Alignment = StringAlignment.Center
        drawFormatRight.Alignment = StringAlignment.Far

        With e.Graphics
            .Clip = New Region(rcBody)
            .FillRectangle(brBody, rcBody)
            .FillRectangle(brHeader, rcHeader)
            .DrawRectangle(New Pen(P_Common.BorderColor), rcForm)
            If P_Common.CloseButton Then
                If bMouseOnClose Then
                    .FillRectangle(New SolidBrush(P_Common.ButtonHoverColor), RectClose)
                    .DrawRectangle(New Pen(P_Common.ButtonBorderColor), RectClose)
                End If
                .DrawLine(New Pen(P_Common.ContentColor, 2), RectClose.Left + 4, RectClose.Top + 4, RectClose.Right - 4, RectClose.Bottom - 4)
                .DrawLine(New Pen(P_Common.ContentColor, 2), RectClose.Left + 4, RectClose.Bottom - 4, RectClose.Right - 4, RectClose.Top + 4)
            End If
            If P_Common.OptionsButton Then
                If bMouseOnOptions Then
                    .FillRectangle(New SolidBrush(P_Common.ButtonHoverColor), RectOptions)
                    .DrawRectangle(New Pen(P_Common.ButtonBorderColor), RectOptions)
                End If
                .FillPolygon(New SolidBrush(ForeColor), New Point() {New Point(RectOptions.Left + 4, RectOptions.Top + 6), New Point(RectOptions.Left + 12, RectOptions.Top + 6), New Point(RectOptions.Left + 8, RectOptions.Top + 4 + 6)})
            End If
            iHeightOfTitle = CInt(.MeasureString("A", P_Common.TitleFont).Height)
            iHeightOfAnrName = CInt(.MeasureString("A", P_Common.ContentFont).Height)
            iHeightOfTelNr = CInt(.MeasureString("A", P_Common.TelNrFont).Height)
            iTitleOrigin = P_Common.TextPadding.Left
            If P_pnAnrMon.Image IsNot Nothing Then
                Dim showim As Image = New Bitmap(P_pnAnrMon.ImageSize.Width, P_pnAnrMon.ImageSize.Height)
                Dim g1 As Graphics = Graphics.FromImage(showim)
                g1.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                g1.DrawImage(P_pnAnrMon.Image, 0, 0, P_pnAnrMon.ImageSize.Width, P_pnAnrMon.ImageSize.Height)
                g1.Dispose()
                .DrawImage(showim, P_pnAnrMon.ImagePosition)
                .DrawRectangle(New Pen(P_Common.ButtonBorderColor), RectImage)
            End If
            Länge = P_pnAnrMon.Size.Width - P_Common.TextPadding.Right - 21 - iTitleOrigin + P_Common.TextPadding.Left
            sUhrzeit = CDate(P_pnAnrMon.Uhrzeit).ToString("dddd, dd. MMMM yyyy HH:mm:ss")
            sTelName = P_pnAnrMon.TelName
            iTelNameLänge = CInt(.MeasureString(sTelName, P_Common.TitleFont).Width)
            iUhrzeitLänge = CInt(.MeasureString(sUhrzeit, P_Common.TitleFont).Width)
            If iTelNameLänge + iUhrzeitLänge > Länge Then
                sUhrzeit = CDate(P_pnAnrMon.Uhrzeit).ToString("dddd, dd. MMM. yy HH:mm:ss")
                iUhrzeitLänge = CInt(.MeasureString(sUhrzeit, P_Common.TitleFont).Width)
                If iTelNameLänge + iUhrzeitLänge > Länge Then
                    sUhrzeit = CDate(P_pnAnrMon.Uhrzeit).ToString("ddd, dd.MM.yy HH:mm:ss")
                    iUhrzeitLänge = CInt(.MeasureString(sUhrzeit, P_Common.TitleFont).Width)
                    If iTelNameLänge + iUhrzeitLänge > Länge Then
                        sUhrzeit = CDate(P_pnAnrMon.Uhrzeit).ToString("dd.MM.yy HH:mm:ss")
                        iUhrzeitLänge = CInt(.MeasureString(sUhrzeit, P_Common.TitleFont).Width)
                    End If
                End If
            End If
            RectZeit = New RectangleF(iTitleOrigin + P_Common.TextPadding.Left, P_Common.TextPadding.Top + P_Common.HeaderHeight, .MeasureString(sUhrzeit, P_Common.TitleFont).Width, iHeightOfTitle)
            RectTelName = New RectangleF(RectZeit.Right, RectZeit.Top, RectClose.Left - RectZeit.Right, iHeightOfTitle)

            .DrawString(sUhrzeit, P_Common.TitleFont, New SolidBrush(P_Common.TitleColor), RectZeit)
            If iTelNameLänge > RectTelName.Width Then
                RectTelName.Y = P_Common.HeaderHeight
                RectTelName.Size = New Size(CInt(RectTelName.Width), CInt(RectTelName.Height * 2 - 3))
            End If
            .DrawString(sTelName, P_Common.TitleFont, New SolidBrush(P_Common.TitleColor), RectTelName, drawFormatRight)
            .DrawString(P_pnAnrMon.TelNr, P_Common.TelNrFont, New SolidBrush(P_Common.TitleColor), RectTelNr, drawFormatCenter)
            .DrawString(P_pnAnrMon.Firma, P_Common.TitleFont, New SolidBrush(P_Common.TitleColor), RectFirma, drawFormatCenter)

            Dim tempfont As New Font(P_Common.DefFontName, 16, P_Common.DefFontStyle, P_Common.DefGraphicsUnit, P_Common.DefgdiCharSet)
            Dim sAnrName As String
            sAnrName = P_pnAnrMon.AnrName
            iAnrNameLänge = CInt(.MeasureString(sAnrName, tempfont, 0, StringFormat.GenericTypographic).Width)

            If iAnrNameLänge > RectAnrName.Width Then
                Dim iFontSize As Integer
                iFontSize = CInt(((RectAnrName.Width - P_Common.TextPadding.Right - P_Common.TextPadding.Left) * (tempfont.Size / 72 * .DpiX - 1.5 * P_Common.TextPadding.Top)) / (iAnrNameLänge - 2 * P_Common.TextPadding.Left))
                iFontSize = CInt(IIf(iFontSize < 8, 8, iFontSize))
                tempfont = New Font(P_Common.DefFontName, 16, P_Common.DefFontStyle, P_Common.DefGraphicsUnit, P_Common.DefgdiCharSet)
            End If

            If bMouseOnLink Then
                Me.Cursor = Cursors.Hand
                .DrawString(P_pnAnrMon.AnrName, tempfont, New SolidBrush(P_Common.LinkHoverColor), RectAnrName, drawFormatCenter)
            Else
                Me.Cursor = Cursors.Default
                .DrawString(P_pnAnrMon.AnrName, tempfont, New SolidBrush(P_Common.ContentColor), RectAnrName, drawFormatCenter)
            End If
        End With
    End Sub

    Private Sub StoppuhrPaint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)
        Dim rcBody As New Rectangle(0, 0, Me.Width, Me.Height)
        Dim rcHeader As New Rectangle(0, 0, Me.Width, P_Common.HeaderHeight)
        Dim rcForm As New Rectangle(0, 0, Me.Width - 1, Me.Height - 1)
        Dim brBody As New Drawing2D.LinearGradientBrush(rcBody, P_Common.BodyColor, P_Common.GetLighterColor(P_Common.BodyColor), Drawing2D.LinearGradientMode.Vertical)
        Dim drawFormatCenter As New StringFormat()
        Dim drawFormatRight As New StringFormat()
        Dim brHeader As New Drawing2D.LinearGradientBrush(rcHeader, P_Common.HeaderColor, P_Common.GetDarkerColor(P_Common.HeaderColor), Drawing2D.LinearGradientMode.Vertical)

        Dim RectZeit As Rectangle
        Dim RectRichtung As Rectangle
        Dim RectAnruf As Rectangle
        Dim RectMSN As Rectangle
        Dim RectStart As Rectangle
        Dim RectEnde As Rectangle
        Dim RectValueStart As Rectangle
        Dim RectValueEnde As Rectangle
        Dim RectValueMSN As Rectangle
        Dim rect As Rectangle

        Dim ErsterEinzug As Integer = 5
        Dim ZweiterEinzug As Integer = 64

        drawFormatCenter.Alignment = StringAlignment.Center
        drawFormatRight.Alignment = StringAlignment.Far


        With e.Graphics
            .FillRectangle(brBody, rcBody)
            .FillRectangle(brHeader, rcHeader)
            .DrawRectangle(New Pen(P_Common.BorderColor), rcForm)
            If bMouseOnClose Then
                .FillRectangle(New SolidBrush(P_Common.ButtonHoverColor), RectClose)
                .DrawRectangle(New Pen(P_Common.ButtonBorderColor), RectClose)
            End If
            .DrawLine(New Pen(P_Common.ContentColor, 2), RectClose.Left + 4, RectClose.Top + 4, RectClose.Right - 4, RectClose.Bottom - 4)
            .DrawLine(New Pen(P_Common.ContentColor, 2), RectClose.Left + 4, RectClose.Bottom - 4, RectClose.Right - 4, RectClose.Top + 4)
            rect = New Rectangle
            With rect
                .X = 64
                .Y = P_Common.HeaderHeight + 5
                .Width = RectClose.X - .X
                .Height = CInt(e.Graphics.MeasureString("A", P_Common.TitleFont).Height)
            End With

            ' <Rechteck Richtung>
            RectRichtung = New Rectangle()
            With RectRichtung
                .X = ErsterEinzug
                .Y = P_Common.HeaderHeight + ErsterEinzug
                .Width = ZweiterEinzug - 2 * ErsterEinzug
                .Height = CInt(e.Graphics.MeasureString(P_pnStoppuhr.Richtung, P_Common.TitleFont).Height)
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectRichtung)
            .DrawString(P_pnStoppuhr.Richtung, P_Common.TitleFont, New SolidBrush(P_Common.ContentColor), RectRichtung)
            ' </Rechteck Richtung>

            ' <Rechteck MSN>
            RectMSN = New Rectangle()
            With RectMSN
                .X = ErsterEinzug
                .Y = 2 * (P_Common.HeaderHeight + ErsterEinzug)
                .Width = ZweiterEinzug - 2 * ErsterEinzug
                .Height = CInt(e.Graphics.MeasureString(P_pnStoppuhr.Richtung, P_Common.TitleFont).Height)
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectMSN)
            .DrawString("MSN: ", P_Common.TitleFont, New SolidBrush(P_Common.ContentColor), RectMSN)
            ' </Rechteck MSN>

            ' <Rechteck Start>
            RectStart = New Rectangle()
            With RectStart
                .X = ErsterEinzug
                .Y = 3 * (P_Common.HeaderHeight + ErsterEinzug)
                .Width = ZweiterEinzug - 2 * ErsterEinzug
                .Height = CInt(e.Graphics.MeasureString(P_pnStoppuhr.Richtung, P_Common.TitleFont).Height)
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectStart)
            .DrawString("Start: ", P_Common.TitleFont, New SolidBrush(P_Common.ContentColor), RectStart)
            ' </Rechteck Start>

            ' <Rechteck Ende>
            RectEnde = New Rectangle()
            With RectEnde
                .X = ErsterEinzug
                .Width = ZweiterEinzug - 2 * ErsterEinzug
                .Height = CInt(e.Graphics.MeasureString(P_pnStoppuhr.Richtung, P_Common.TitleFont).Height)
                .Y = P_pnStoppuhr.Size.Height - .Height - 1 ' - ErsterEinzug
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectEnde)
            .DrawString("Ende: ", P_Common.TitleFont, New SolidBrush(P_Common.ContentColor), RectEnde)
            ' </Rechteck Ende>

            ' <Rechteck Value Anruf>
            RectAnruf = New Rectangle()
            With RectAnruf
                .X = ZweiterEinzug
                .Y = 1 * (P_Common.HeaderHeight + ErsterEinzug)
                .Width = RectClose.X - ZweiterEinzug - ErsterEinzug
                '.Width = P_Common.Size.Width - ZweiterEinzug - ErsterEinzug - RectClose.X
                .Height = CInt(e.Graphics.MeasureString(P_pnStoppuhr.Anruf, P_Common.TitleFont).Height)
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectAnruf)
            .DrawString(P_pnStoppuhr.Anruf, P_Common.TitleFont, New SolidBrush(P_Common.ContentColor), RectAnruf)
            ' </Rechteck Value Anruf>

            ' <Rechteck Value MSN>
            RectValueMSN = New Rectangle()
            With RectValueMSN
                .X = ZweiterEinzug
                .Y = 2 * (P_Common.HeaderHeight + ErsterEinzug)
                .Width = P_pnStoppuhr.Size.Width - ZweiterEinzug - ErsterEinzug
                .Height = CInt(e.Graphics.MeasureString(P_pnStoppuhr.Richtung, P_Common.TitleFont).Height)
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectValueMSN)
            .DrawString(P_pnStoppuhr.MSN, P_Common.TitleFont, New SolidBrush(P_Common.ContentColor), RectValueMSN)
            ' </Rechteck Value MSN>

            ' <Rechteck Value Start>
            RectValueStart = New Rectangle()
            With RectValueStart
                .X = ZweiterEinzug
                .Y = 3 * (P_Common.HeaderHeight + ErsterEinzug)
                .Width = P_pnStoppuhr.Size.Width - ZweiterEinzug - ErsterEinzug
                .Height = CInt(e.Graphics.MeasureString(P_pnStoppuhr.Richtung, P_Common.TitleFont).Height)
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectValueStart)
            .DrawString(P_pnStoppuhr.StartZeit, P_Common.TitleFont, New SolidBrush(P_Common.ContentColor), RectValueStart)
            ' </Rechteck Value Start>

            ' <Rechteck Value Ende>
            RectValueEnde = New Rectangle()
            With RectValueEnde
                .X = ZweiterEinzug
                .Width = P_pnStoppuhr.Size.Width - ZweiterEinzug - 1 * ErsterEinzug
                .Height = CInt(e.Graphics.MeasureString(P_pnStoppuhr.Richtung, P_Common.TitleFont).Height)
                .Y = P_pnStoppuhr.Size.Height - .Height - 1 '- ErsterEinzug
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectValueEnde)
            .DrawString(P_pnStoppuhr.EndeZeit, P_Common.TitleFont, New SolidBrush(P_Common.ContentColor), RectValueEnde)
            ' </Rechteck Value Ende>

            RectZeit = New Rectangle()
            With RectZeit
                .X = 0
                .Y = CInt(2 * (P_pnStoppuhr.Size.Height - P_Common.ContentFont.Size) / 3 + 2)
                .Width = P_pnStoppuhr.Size.Width
                .Height = CInt(e.Graphics.MeasureString(P_pnStoppuhr.Zeit, P_Common.fSUContentFont).Height)
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectZeit)
            .DrawString(P_pnStoppuhr.Zeit, P_Common.fSUContentFont, New SolidBrush(P_Common.ContentColor), RectZeit, drawFormatCenter)

        End With
    End Sub

#End Region

    Protected Overrides Sub Finalize()
        Me.Hide()
        MyBase.Finalize()
    End Sub
End Class