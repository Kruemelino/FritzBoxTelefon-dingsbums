Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Timers

<System.ComponentModel.DefaultPropertyAttribute("Content"), System.ComponentModel.DesignTimeVisible(False)> _
Friend Class StoppuhrForm
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
    Private iHeightOfTitle As Integer
    Private iHeightOfZeit As Integer
    Private iHeightOfTelNr As Integer

    Friend Event CloseClickStoppUhr(ByVal sender As Object, ByVal e As System.EventArgs)
    Friend Event CloseClick(ByVal sender As Object, ByVal e As System.EventArgs)

    Sub New(ByVal Parent As F_StoppUhr)
        pnParent = Parent
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
    End Sub

    Protected Overrides Sub Finalize()
        Me.Hide()
        MyBase.Finalize()
    End Sub

#Region "Properties"
    Protected Overrides ReadOnly Property ShowWithoutActivation() As Boolean
        Get
            Return True
        End Get
    End Property
    Private pnParent As F_StoppUhr
    Shadows Property Parent() As F_StoppUhr
        Get
            Return pnParent
        End Get
        Set(ByVal value As F_StoppUhr)
            pnParent = value
        End Set
    End Property

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

    Private ReadOnly Property RectClose() As Rectangle
        Get
            Return New Rectangle(Me.Width - 5 - 16, 12, 16, 16)
        End Get
    End Property

#End Region

#Region "Events"
    Private Sub StoppuhrForm_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        Me.Finalize()
    End Sub

    Private Sub PopupStoppuhrForm_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        Dim retIPVal As IntPtr
        Dim HTCAPTION As IntPtr = CType(2, IntPtr)
        Dim WM_NCLBUTTONDOWN As Int32 = &HA1S
        Dim retbVal As Boolean
        If Not RectClose.Contains(e.X, e.Y) Then
            retbVal = OutlookSecurity.ReleaseCapture()
            retIPVal = OutlookSecurity.SendMessage(Me.Handle, WM_NCLBUTTONDOWN, HTCAPTION, IntPtr.Zero)
        End If
    End Sub

    Private Sub PopupStoppuhrForm_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        If RectClose.Contains(e.X, e.Y) Then
            bMouseOnClose = True
        Else
            bMouseOnClose = False
        End If
        Invalidate()
    End Sub

    Private Sub PopupStoppuhrForm_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        If RectClose.Contains(e.X, e.Y) Then
            Me.Close()
            RaiseEvent CloseClickStoppUhr(sender, e)
        End If
    End Sub

    Private Sub PopupStopUhrPaint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        Dim rcBody As New Rectangle(0, 0, Me.Width, Me.Height)
        Dim rcHeader As New Rectangle(0, 0, Me.Width, Parent.HeaderHeight)
        Dim rcForm As New Rectangle(0, 0, Me.Width - 1, Me.Height - 1)
        Dim brBody As New LinearGradientBrush(rcBody, Parent.BodyColor, GetLighterColor(Parent.BodyColor), LinearGradientMode.Vertical)
        Dim drawFormatCenter As New StringFormat()
        Dim drawFormatRight As New StringFormat()
        Dim brHeader As New LinearGradientBrush(rcHeader, Parent.HeaderColor, GetDarkerColor(Parent.HeaderColor), LinearGradientMode.Vertical)

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
            .DrawRectangle(New Pen(Parent.BorderColor), rcForm)
            If bMouseOnClose Then
                .FillRectangle(New SolidBrush(Parent.ButtonHoverColor), RectClose)
                .DrawRectangle(New Pen(Parent.ButtonBorderColor), RectClose)
            End If
            .DrawLine(New Pen(Parent.ContentColor, 2), RectClose.Left + 4, RectClose.Top + 4, RectClose.Right - 4, RectClose.Bottom - 4)
            .DrawLine(New Pen(Parent.ContentColor, 2), RectClose.Left + 4, RectClose.Bottom - 4, RectClose.Right - 4, RectClose.Top + 4)
            rect = New Rectangle
            With rect
                .X = 64
                .Y = Parent.HeaderHeight + 5
                .Width = RectClose.X - .X
                .Height = CInt(e.Graphics.MeasureString("A", Parent.TitleFont).Height)
            End With

            ' <Rechteck Richtung>
            RectRichtung = New Rectangle()
            With RectRichtung
                .X = ErsterEinzug
                .Y = Parent.HeaderHeight + ErsterEinzug
                .Width = ZweiterEinzug - 2 * ErsterEinzug
                .Height = CInt(e.Graphics.MeasureString(Parent.Richtung, Parent.TitleFont).Height)
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectRichtung)
            .DrawString(Parent.Richtung, Parent.TitleFont, New SolidBrush(Parent.ContentColor), RectRichtung)
            ' </Rechteck Richtung>

            ' <Rechteck MSN>
            RectMSN = New Rectangle()
            With RectMSN
                .X = ErsterEinzug
                .Y = 2 * (Parent.HeaderHeight + ErsterEinzug)
                .Width = ZweiterEinzug - 2 * ErsterEinzug
                .Height = CInt(e.Graphics.MeasureString(Parent.Richtung, Parent.TitleFont).Height)
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectMSN)
            .DrawString("MSN: ", Parent.TitleFont, New SolidBrush(Parent.ContentColor), RectMSN)
            ' </Rechteck MSN>

            ' <Rechteck Start>
            RectStart = New Rectangle()
            With RectStart
                .X = ErsterEinzug
                .Y = 3 * (Parent.HeaderHeight + ErsterEinzug)
                .Width = ZweiterEinzug - 2 * ErsterEinzug
                .Height = CInt(e.Graphics.MeasureString(Parent.Richtung, Parent.TitleFont).Height)
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectStart)
            .DrawString("Start: ", Parent.TitleFont, New SolidBrush(Parent.ContentColor), RectStart)
            ' </Rechteck Start>

            ' <Rechteck Ende>
            RectEnde = New Rectangle()
            With RectEnde
                .X = ErsterEinzug
                .Width = ZweiterEinzug - 2 * ErsterEinzug
                .Height = CInt(e.Graphics.MeasureString(Parent.Richtung, Parent.TitleFont).Height)
                .Y = Parent.Size.Height - .Height ' - ErsterEinzug
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectEnde)
            .DrawString("Ende: ", Parent.TitleFont, New SolidBrush(Parent.ContentColor), RectEnde)
            ' </Rechteck Ende>

            ' <Rechteck Value Anruf>
            RectAnruf = New Rectangle()
            With RectAnruf
                .X = ZweiterEinzug
                .Y = 1 * (Parent.HeaderHeight + ErsterEinzug)
                .Width = RectClose.X - ZweiterEinzug - ErsterEinzug
                '.Width = Parent.Size.Width - ZweiterEinzug - ErsterEinzug - RectClose.X
                .Height = CInt(e.Graphics.MeasureString(Parent.Anruf, Parent.TitleFont).Height)
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectAnruf)
            .DrawString(Parent.Anruf, Parent.TitleFont, New SolidBrush(Parent.ContentColor), RectAnruf)
            ' </Rechteck Value Anruf>

            ' <Rechteck Value MSN>
            RectValueMSN = New Rectangle()
            With RectValueMSN
                .X = ZweiterEinzug
                .Y = 2 * (Parent.HeaderHeight + ErsterEinzug)
                .Width = Parent.Size.Width - ZweiterEinzug - ErsterEinzug
                .Height = CInt(e.Graphics.MeasureString(Parent.Richtung, Parent.TitleFont).Height)
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectValueMSN)
            .DrawString(Parent.MSN, Parent.TitleFont, New SolidBrush(Parent.ContentColor), RectValueMSN)
            ' </Rechteck Value MSN>

            ' <Rechteck Value Start>
            RectValueStart = New Rectangle()
            With RectValueStart
                .X = ZweiterEinzug
                .Y = 3 * (Parent.HeaderHeight + ErsterEinzug)
                .Width = Parent.Size.Width - ZweiterEinzug - ErsterEinzug
                .Height = CInt(e.Graphics.MeasureString(Parent.Richtung, Parent.TitleFont).Height)
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectValueStart)
            .DrawString(Parent.StartZeit, Parent.TitleFont, New SolidBrush(Parent.ContentColor), RectValueStart)
            ' </Rechteck Value Start>

            ' <Rechteck Value Ende>
            RectValueEnde = New Rectangle()
            With RectValueEnde
                .X = ZweiterEinzug
                .Width = Parent.Size.Width - ZweiterEinzug - 1 * ErsterEinzug
                .Height = CInt(e.Graphics.MeasureString(Parent.Richtung, Parent.TitleFont).Height)
                .Y = Parent.Size.Height - .Height '- ErsterEinzug
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectValueEnde)
            .DrawString(Parent.EndeZeit, Parent.TitleFont, New SolidBrush(Parent.ContentColor), RectValueEnde)
            ' </Rechteck Value Ende>

            RectZeit = New Rectangle()
            With RectZeit
                .X = 0
                .Y = CInt(2 * (Parent.Size.Height - Parent.ContentFont.Size) / 3 + 2)
                .Width = Parent.Size.Width
                .Height = CInt(e.Graphics.MeasureString(Parent.Zeit, Parent.ContentFont).Height)
            End With
            ' .DrawRectangle(New Pen(Brushes.Black), RectZeit)
            .DrawString(Parent.Zeit, Parent.ContentFont, New SolidBrush(Parent.ContentColor), RectZeit, drawFormatCenter)

        End With
    End Sub

#End Region

End Class

Public Class F_StoppUhr
    Inherits Component
    Private WithEvents fStopUhr As New StoppuhrForm(Me)
    Private WithEvents TimerZeit As New Timer
    Private WithEvents TimerSchließen As New Timer
    Private Stoppwatch As New Stopwatch
    Private i As Integer = 0
    Public Event Close(ByVal sender As Object, ByVal e As System.EventArgs)
    Delegate Sub SchließeStoppUhr()

#Region "Properties"
    Private clHeader As Color = SystemColors.ControlDark
    <Category("Header"), _
    DefaultValue(GetType(Color), "ControlDark")> _
    Property HeaderColor() As Color
        Get
            Return clHeader
        End Get
        Set(ByVal value As Color)
            clHeader = value
        End Set
    End Property
    Private clBody As Color = SystemColors.Control
    <Category("Appearance"), _
    DefaultValue(GetType(Color), "Control")> _
    Property BodyColor() As Color
        Get
            Return clBody
        End Get
        Set(ByVal value As Color)
            clBody = value

        End Set
    End Property
    Private clTitle As Color = Color.Gray
    <Category("Title"), _
    DefaultValue(GetType(Color), "Gray")> _
    Property TitleColor() As Color
        Get
            Return clTitle
        End Get
        Set(ByVal value As Color)
            clTitle = value

        End Set
    End Property
    Private clBase As Color = SystemColors.ControlText
    <Category("Content"), _
    DefaultValue(GetType(Color), "ControlText")> _
    Property ContentColor() As Color
        Get
            Return clBase
        End Get
        Set(ByVal value As Color)
            clBase = value
        End Set
    End Property
    Private clBorder As Color = SystemColors.WindowFrame
    <Category("Appearance"), _
    DefaultValue(GetType(Color), "WindowFrame")> _
    Property BorderColor() As Color
        Get
            Return clBorder
        End Get
        Set(ByVal value As Color)
            clBorder = value
        End Set
    End Property
    Private clCloseBorder As Color = SystemColors.WindowFrame
    <Category("Buttons"), _
    DefaultValue(GetType(Color), "WindowFrame")> _
    Property ButtonBorderColor() As Color
        Get
            Return clCloseBorder
        End Get
        Set(ByVal value As Color)
            clCloseBorder = value
        End Set
    End Property
    Private clCloseHover As Color = SystemColors.Highlight
    <Category("Buttons"), _
    DefaultValue(GetType(Color), "Highlight")> _
    Property ButtonHoverColor() As Color
        Get
            Return clCloseHover
        End Get
        Set(ByVal value As Color)
            clCloseHover = value

        End Set
    End Property
    Private iDiffGradient As Integer = 50
    <Category("Appearance"), _
    DefaultValue(50)> _
    Property GradientPower() As Integer
        Get
            Return iDiffGradient
        End Get
        Set(ByVal value As Integer)
            iDiffGradient = value

        End Set
    End Property
    Private ftBase As Font = New Font("Segoe UI", 18) 'SystemFonts.DialogFont
    <Category("Zeit")> _
    Property ContentFont() As Font
        Get
            Return ftBase
        End Get
        Set(ByVal value As Font)
            ftBase = value
        End Set
    End Property
    Private ftTitle As Font = New Font("Segoe UI", 9) 'SystemFonts.CaptionFont
    <Category("Title")> _
    Property TitleFont() As Font
        Get
            Return ftTitle
        End Get
        Set(ByVal value As Font)
            ftTitle = value

        End Set
    End Property
    Private sZeit As String
    <Category("Zeit")> _
    Property Zeit() As String
        Get
            Return sZeit
        End Get
        Set(ByVal value As String)
            sZeit = value
        End Set
    End Property
    Private sAnruf As String
    <Category("Zeit")> _
    Property Anruf() As String
        Get
            Return sAnruf
        End Get
        Set(ByVal value As String)
            sAnruf = value
        End Set
    End Property
    Private sRichtung As String
    <Category("Zeit")> _
    Property Richtung() As String
        Get
            Return sRichtung
        End Get
        Set(ByVal value As String)
            sRichtung = value
        End Set
    End Property
    Private sWarteZeit As Integer
    <Category("Zeit")> _
    Property WarteZeit() As Integer
        Get
            Return sWarteZeit
        End Get
        Set(ByVal value As Integer)
            sWarteZeit = value
        End Set
    End Property
    Private sStartZeit As String
    <Category("Zeit")> _
    Property StartZeit() As String
        Get
            Return sStartZeit
        End Get
        Set(ByVal value As String)
            sStartZeit = value
        End Set
    End Property
    Private sEndeZeit As String
    <Category("Zeit")> _
    Property EndeZeit() As String
        Get
            Return sEndeZeit
        End Get
        Set(ByVal value As String)
            sEndeZeit = value
        End Set
    End Property
    Private sMSN As String
    <Category("MSN")> _
    Property MSN() As String
        Get
            Return sMSN
        End Get
        Set(ByVal value As String)
            sMSN = value
        End Set
    End Property
    Private pdTextPadding As Padding = New Padding(0)
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
    <Category("Header"), _
    DefaultValue(9)> _
    Property HeaderHeight() As Integer
        Get
            Return iHeaderHeight
        End Get
        Set(ByVal value As Integer)
            iHeaderHeight = value

        End Set
    End Property
    Private szSize As Size = New Size(250, 100)
    <Category("Appearance")> _
    Property Size() As Size
        Get
            Return szSize
        End Get
        Set(ByVal value As Size)
            szSize = value
        End Set
    End Property
    Private szStartPosition As Point = New Point(0, 0)
    <Category("Appearance")> _
    Property StartPosition() As Point
        Get
            Return szStartPosition
        End Get
        Set(ByVal value As Point)
            szStartPosition = value
        End Set
    End Property
#End Region

    Sub New()
        With fStopUhr
            .FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            .StartPosition = System.Windows.Forms.FormStartPosition.Manual
            .ShowInTaskbar = True
        End With
    End Sub

    Sub Popup()
        Dim retVal As Boolean
        With fStopUhr
            .TopMost = True
            .Size = Size
            .Location = StartPosition
            .Show()

            retVal = OutlookSecurity.SetWindowPos(.Handle, hWndInsertAfterFlags.HWND_TOPMOST, 0, 0, 0, 0, _
                                      CType(SetWindowPosFlags.DoNotActivate + _
                                      SetWindowPosFlags.IgnoreMove + _
                                      SetWindowPosFlags.IgnoreResize + _
                                      SetWindowPosFlags.DoNotChangeOwnerZOrder, SetWindowPosFlags))
        End With
    End Sub

    Public Sub StoppuhrStart()

        With TimerZeit
            .Interval = 250
            .Start()
        End With
        Stoppwatch.Start()
    End Sub

    Public Sub StoppuhrStopp()
        Dim Zeit As String
        With System.DateTime.Now
            Zeit = String.Format("{0:00}:{1:00}:{2:00}", .Hour, .Minute, .Second)
        End With
        EndeZeit = Zeit
        fStopUhr.Invalidate()
        TimerZeit.Stop()
        Stoppwatch.Stop()
        If Not sWarteZeit = -1 Then
            TimerSchließen = New Timer
            With TimerSchließen
                .Interval = sWarteZeit * 1000
                .AutoReset = True
                .Start()
            End With
        End If
    End Sub

    Private Sub timerZeit_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerZeit.Elapsed
        With Stoppwatch.Elapsed
            Zeit = String.Format("{0:00}:{1:00}:{2:00}", .Hours, .Minutes, .Seconds)
        End With
        fStopUhr.Invalidate()
    End Sub

    Private Sub TimerSchließen_Elapsed(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles TimerSchließen.Elapsed, fStopUhr.CloseClickStoppUhr 'Ehemals: System.Timers.ElapsedEventArgs

        TimerSchließen.Stop()
        TimerSchließen = Nothing
        Stoppwatch.Stop()
        TimerZeit.Close()
        Stoppwatch = Nothing
        TimerZeit = Nothing
        StartPosition = fStopUhr.Location
        AutoSchließen()
        RaiseEvent Close(Me, EventArgs.Empty)
        Me.Finalize()
    End Sub

    Sub AutoSchließen()
        If fStopUhr.InvokeRequired Then
            Dim D As New SchließeStoppUhr(AddressOf AutoSchließen)
            fStopUhr.Invoke(D)
        Else
            fStopUhr.Close()
        End If
    End Sub
End Class