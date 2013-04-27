Imports System.ComponentModel
Imports System.Drawing.Drawing2D

<System.ComponentModel.DefaultPropertyAttribute("Content"), _
System.ComponentModel.DesignTimeVisible(False)> _
Public Class PopupStoppuhrForm
    Inherits System.Windows.Forms.Form
    Private Declare Function ReleaseCapture Lib "user32" () As Integer
    Private Declare Function SendMessage Lib "user32" Alias "SendMessageA" ( _
        ByVal hwnd As Integer, _
        ByVal wMsg As Integer, _
        ByVal wParam As Integer, _
        ByRef lParam As Object) As Integer

    Private Const HTCAPTION As Short = 2
    Private Const WM_NCLBUTTONDOWN As Short = &HA1S
    Private Const WM_SYSCOMMAND As Short = &H112S
    Public Event CloseClick()
    Private bMouseOnClose As Boolean = False
    Private bMouseOnLink As Boolean = False
    Private iHeightOfTitle As Integer
    Private iHeightOfZeit As Integer
    Private iHeightOfTelNr As Integer
    Public Event LinkClick()
    Public Event CloseClickStoppUhr()

    Sub New(ByVal Parent As Stoppuhr)
        pnParent = Parent
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
    End Sub

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'PopupStoppuhrForm
        '
        Me.ClientSize = New System.Drawing.Size(300, 66)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "PopupStoppuhrForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.ResumeLayout(False)

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
    Private pnParent As Stoppuhr
    Shadows Property Parent() As Stoppuhr
        Get
            Return pnParent
        End Get
        Set(ByVal value As Stoppuhr)
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

    Private Sub PopupNotifierForm_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        If Not RectClose.Contains(e.X, e.Y) Then
            ReleaseCapture()
            SendMessage(Me.Handle.ToInt32, WM_NCLBUTTONDOWN, HTCAPTION, 0)
        End If
    End Sub

    Private Sub PopupNotifierForm_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        If RectClose.Contains(e.X, e.Y) Then
            bMouseOnClose = True
        Else
            bMouseOnClose = False
        End If
        Invalidate()
    End Sub

    Private Sub PopupNotifierForm_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        If RectClose.Contains(e.X, e.Y) Then
            Me.Close()
            RaiseEvent CloseClickStoppUhr()
        End If
    End Sub

    Private Sub PopupNotifierForm_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
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
            If bMouseOnClose Then
                .FillRectangle(New SolidBrush(Parent.ButtonHoverColor), RectClose)
                .DrawRectangle(New Pen(Parent.ButtonBorderColor), RectClose)
            End If
            .DrawLine(New Pen(Parent.ContentColor, 2), RectClose.Left + 4, RectClose.Top + 4, RectClose.Right - 4, RectClose.Bottom - 4)
            .DrawLine(New Pen(Parent.ContentColor, 2), RectClose.Left + 4, RectClose.Bottom - 4, RectClose.Right - 4, RectClose.Top + 4)
            Dim rect As New Rectangle
            With rect
                .X = 64
                .Y = Parent.HeaderHeight + 5
                .Width = RectClose.X - .X
                .Height = e.Graphics.MeasureString("A", Parent.TitleFont).Height
            End With
            .DrawString(Parent.Richtung, Parent.TitleFont, New SolidBrush(Parent.ContentColor), 5, Parent.HeaderHeight + 5)
            .DrawString(Parent.Anruf, Parent.TitleFont, New SolidBrush(Parent.ContentColor), rect)

            .DrawString("MSN: ", Parent.TitleFont, New SolidBrush(Parent.ContentColor), 5, 2 * Parent.HeaderHeight + 10)
            .DrawString(Parent.MSN, Parent.TitleFont, New SolidBrush(Parent.ContentColor), 64, 2 * Parent.HeaderHeight + 10)

            .DrawString("Start:", Parent.TitleFont, New SolidBrush(Parent.ContentColor), 5, 3 * Parent.HeaderHeight + 15)
            .DrawString(Parent.StartZeit, Parent.TitleFont, New SolidBrush(Parent.ContentColor), 64, 3 * Parent.HeaderHeight + 15)

            .DrawString("Ende: ", Parent.TitleFont, New SolidBrush(Parent.ContentColor), 5, Me.Height - Parent.HeaderHeight - 10)
            .DrawString(Parent.EndeZeit, Parent.TitleFont, New SolidBrush(Parent.ContentColor), 64, Me.Height - Parent.HeaderHeight - 10)

            .DrawString(Parent.Zeit, Parent.ContentFont, New SolidBrush(Parent.ContentColor), Parent.Size.Width / 2, 2 * (Parent.Size.Height - Parent.ContentFont.Size) / 3, drawFormatCenter)

        End With
    End Sub
#End Region


End Class
