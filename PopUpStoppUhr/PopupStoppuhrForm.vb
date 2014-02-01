Imports System.ComponentModel
Imports System.Drawing.Drawing2D

<System.ComponentModel.DefaultPropertyAttribute("Content"), System.ComponentModel.DesignTimeVisible(False)> _
Public Class PopupStoppuhrForm
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

    Private Declare Function ReleaseCapture Lib "user32" () As Integer
    Private Declare Function SendMessage Lib "user32" Alias "SendMessageA" (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, ByRef lParam As Object) As Integer

    Private Const HTCAPTION As Short = 2
    Private Const WM_NCLBUTTONDOWN As Short = &HA1S
    Private Const WM_SYSCOMMAND As Short = &H112S
    Public Event CloseClick()
    Private bMouseOnClose As Boolean = False
    Private bMouseOnLink As Boolean = False
    Private iHeightOfTitle As Integer
    Private iHeightOfZeit As Integer
    Private iHeightOfTelNr As Integer
    Public Event CloseClickStoppUhr()

    Sub New(ByVal Parent As PopUpStoppUhr)
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
    Private pnParent As PopUpStoppUhr
    Shadows Property Parent() As PopUpStoppUhr
        Get
            Return pnParent
        End Get
        Set(ByVal value As PopUpStoppUhr)
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

    Private Sub PopupStoppuhrForm_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        If Not RectClose.Contains(e.X, e.Y) Then
            ReleaseCapture()
            SendMessage(Me.Handle.ToInt32, WM_NCLBUTTONDOWN, HTCAPTION, 0)
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
            RaiseEvent CloseClickStoppUhr()
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
                .Height = e.Graphics.MeasureString("A", Parent.TitleFont).Height
            End With

            ' <Rechteck Richtung>
            RectRichtung = New Rectangle()
            With RectRichtung
                .X = ErsterEinzug
                .Y = Parent.HeaderHeight + ErsterEinzug
                .Width = ZweiterEinzug - 2 * ErsterEinzug
                .Height = e.Graphics.MeasureString(Parent.Richtung, Parent.TitleFont).Height
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
                .Height = e.Graphics.MeasureString(Parent.Richtung, Parent.TitleFont).Height
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
                .Height = e.Graphics.MeasureString(Parent.Richtung, Parent.TitleFont).Height
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectStart)
            .DrawString("Start: ", Parent.TitleFont, New SolidBrush(Parent.ContentColor), RectStart)
            ' </Rechteck Start>

            ' <Rechteck Ende>
            RectEnde = New Rectangle()
            With RectEnde
                .X = ErsterEinzug
                .Width = ZweiterEinzug - 2 * ErsterEinzug
                .Height = e.Graphics.MeasureString(Parent.Richtung, Parent.TitleFont).Height
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
                .Height = e.Graphics.MeasureString(Parent.Anruf, Parent.TitleFont).Height
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
                .Height = e.Graphics.MeasureString(Parent.Richtung, Parent.TitleFont).Height
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
                .Height = e.Graphics.MeasureString(Parent.Richtung, Parent.TitleFont).Height
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectValueStart)
            .DrawString(Parent.StartZeit, Parent.TitleFont, New SolidBrush(Parent.ContentColor), RectValueStart)
            ' </Rechteck Value Start>

            ' <Rechteck Value Ende>
            RectValueEnde = New Rectangle()
            With RectValueEnde
                .X = ZweiterEinzug
                .Width = Parent.Size.Width - ZweiterEinzug - 1 * ErsterEinzug
                .Height = e.Graphics.MeasureString(Parent.Richtung, Parent.TitleFont).Height
                .Y = Parent.Size.Height - .Height '- ErsterEinzug
            End With
            '.DrawRectangle(New Pen(Brushes.Black), RectValueEnde)
            .DrawString(Parent.EndeZeit, Parent.TitleFont, New SolidBrush(Parent.ContentColor), RectValueEnde)
            ' </Rechteck Value Ende>

            RectZeit = New Rectangle()
            With RectZeit
                .X = 0
                .Y = 2 * (Parent.Size.Height - Parent.ContentFont.Size) / 3 + 2
                .Width = Parent.Size.Width
                .Height = e.Graphics.MeasureString(Parent.Zeit, Parent.ContentFont).Height
            End With
            ' .DrawRectangle(New Pen(Brushes.Black), RectZeit)
            .DrawString(Parent.Zeit, Parent.ContentFont, New SolidBrush(Parent.ContentColor), RectZeit, drawFormatCenter)

        End With
    End Sub

#End Region

End Class
