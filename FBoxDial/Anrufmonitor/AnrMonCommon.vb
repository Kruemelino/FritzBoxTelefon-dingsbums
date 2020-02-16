Imports System.Windows.Forms
Imports System.Drawing
Imports System.ComponentModel

Friend Class AnrMonCommon
    Inherits Form
    Implements IDisposable

    Friend Event LinkClick(ByVal sender As Object, ByVal e As EventArgs)
    Friend Event CloseClick(ByVal sender As Object, ByVal e As EventArgs)

    Sub New(ByVal vAnrMon As FormAnrMon, ByRef vCommon As CommonFenster)
        PAnrMon = vAnrMon
        PCommon = vCommon

        SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        SetStyle(ControlStyles.ResizeRedraw, True)
        SetStyle(ControlStyles.UserPaint, True)
        SetStyle(ControlStyles.AllPaintingInWmPaint, True)
    End Sub


#Region "Properties"
    Private Property BMouseOnClose As Boolean = False
    Private Property BMouseOnLink As Boolean = False
    Private Property BMouseOnOptions As Boolean = False
    Protected Overrides ReadOnly Property ShowWithoutActivation() As Boolean = True
    Private Property PKontaktbild As Bitmap
    Friend Property ShowBorders As Boolean = False
    Shadows Property PAnrMon() As FormAnrMon
    Shadows Property PCommon() As CommonFenster
#End Region

#Region "Properties RectangleF"
    Private Property RectTelNr() As RectangleF
    Private Property RectAnrName() As RectangleF
    Private Property RectFirma() As RectangleF
    Private Property RectZeit() As RectangleF
    Private Property RectClose() As RectangleF
    Private Property RectOptions() As RectangleF
    Private Property RectImage() As RectangleF
    Private Property RectTelName As RectangleF
    Friend Property ScaleFaktor As SizeF

#End Region

#Region "Events"
    Private Sub Me_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Me.MouseMove

        BMouseOnClose = RectClose.Contains(e.X, e.Y)
        BMouseOnOptions = RectOptions.Contains(e.X, e.Y)
        BMouseOnLink = RectAnrName.Contains(e.X, e.Y)
        Invalidate()
    End Sub
    Private Sub Me_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Me.MouseUp
        If RectClose.Contains(e.X, e.Y) Then
            RaiseEvent CloseClick(Me, EventArgs.Empty)
        End If

        If RectAnrName.Contains(e.X, e.Y) Then
            RaiseEvent LinkClick(Me, EventArgs.Empty)
        End If

        If RectOptions.Contains(e.X, e.Y) Then
            If PAnrMon.OptionsMenu IsNot Nothing Then
                PAnrMon.OptionsMenu.Show(Me, New Point((RectOptions.Right - PAnrMon.OptionsMenu.Width).ToInt, RectOptions.Bottom.ToInt))
            End If
        End If

    End Sub
    Private Sub Me_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles Me.Paint
        If PAnrMon IsNot Nothing Then AnrMon_Paint(sender, e)
    End Sub
    Private Sub Me_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Scale(ScaleFaktor)
    End Sub
    Private Sub AnrMon_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)

        Dim rcBody As New RectangleF(0, 0, Width, Height)
        Dim rcHeader As New RectangleF(0, 0, Width, PCommon.HeaderHeight)
        Dim rcForm As New RectangleF(0, 0, Width - 1, Height - 1)
        Dim brBody As New Drawing2D.LinearGradientBrush(rcBody, PCommon.BodyColor, PCommon.GetLighterColor(PCommon.BodyColor), Drawing2D.LinearGradientMode.Vertical)
        Dim drawFormatCenter As New StringFormat()
        Dim drawFormatRight As New StringFormat()
        Dim drawFormatLeft As New StringFormat()
        Dim brHeader As New Drawing2D.LinearGradientBrush(rcHeader, PCommon.HeaderColor, PCommon.GetDarkerColor(PCommon.HeaderColor), Drawing2D.LinearGradientMode.Vertical)

        Dim IHeightOfTitle As Integer
        Dim IHeightOfTelNr As Integer
        Dim ITitleOrigin As Integer

        With drawFormatCenter
            .Alignment = StringAlignment.Center
            .LineAlignment = StringAlignment.Center
        End With

        With drawFormatRight
            .Alignment = StringAlignment.Far
            .LineAlignment = StringAlignment.Center
        End With

        With drawFormatLeft
            .Alignment = StringAlignment.Near
            .LineAlignment = StringAlignment.Center
        End With

        With e.Graphics

            .Clip = New Region(rcBody)
            .FillRectangle(brBody, rcBody)
            .FillRectangle(brHeader, rcHeader)
            DrawRectangleF(e.Graphics, rcForm)
            ' Buttton: Schließen

            RectClose = New RectangleF(Width - 5 - 16 * ScaleFaktor.Width, 12, 16 * ScaleFaktor.Width, 16 * ScaleFaktor.Height)

            If BMouseOnClose Then
                .FillRectangle(New SolidBrush(PCommon.ButtonHoverColor), RectClose)
                DrawRectangleF(e.Graphics, RectClose)
            End If
            ' Zeichne das X
            .DrawLine(New Pen(PCommon.ContentColor, 2), RectClose.Left + 4, RectClose.Top + 4, RectClose.Right - 4, RectClose.Bottom - 4)
            .DrawLine(New Pen(PCommon.ContentColor, 2), RectClose.Left + 4, RectClose.Bottom - 4, RectClose.Right - 4, RectClose.Top + 4)

            ' Buttton: Schließen
            If PCommon.OptionsButton Then
                RectOptions = New RectangleF(RectClose.Left - 5 - 16 * ScaleFaktor.Width, 12, 16 * ScaleFaktor.Width, 16 * ScaleFaktor.Height)
                If BMouseOnOptions Then
                    .FillRectangle(New SolidBrush(PCommon.ButtonHoverColor), RectOptions)
                    DrawRectangleF(e.Graphics, RectOptions)
                End If
                ' Zeichne Optionsstriche
                .DrawLine(New Pen(PCommon.ContentColor, 2), RectOptions.Left + 4, RectOptions.Top + 6, RectOptions.Right - 4, RectOptions.Top + 6)
                .DrawLine(New Pen(PCommon.ContentColor, 2), RectOptions.Left + 4, RectOptions.Top + RectOptions.Height / 2, RectOptions.Right - 4, RectOptions.Top + RectOptions.Height / 2)
                .DrawLine(New Pen(PCommon.ContentColor, 2), RectOptions.Left + 4, RectOptions.Bottom - 6, RectOptions.Right - 4, RectOptions.Bottom - 6)
            End If

            IHeightOfTitle = .MeasureString(PAnrMon.TelName, PCommon.TitleFont).Height.ToInt
            IHeightOfTelNr = .MeasureString(PAnrMon.TelNr, PCommon.TelNrFont).Height.ToInt
            ITitleOrigin = PCommon.TextPadding.Left

            ' Zeichne Rechteck für das Kontaktbild
            If PAnrMon.Image IsNot Nothing Then
                ' Um so viel Informationen wie möglich zur Verfügung zu stellen wird der Breich für das Bild anhand der Höhe des Anrufmonitors und dem Seitenverhältnis des Bildes bestimmt.

                RectImage = New RectangleF(ITitleOrigin,
                                           PCommon.TextPadding.Top + PCommon.HeaderHeight,
                                           (Height - PCommon.TextPadding.Top - PCommon.TextPadding.Bottom - PCommon.HeaderHeight) * PAnrMon.Image.Size.Width \ PAnrMon.Image.Size.Height,
                                           Height - PCommon.TextPadding.Top - PCommon.TextPadding.Bottom - PCommon.HeaderHeight)

                PKontaktbild = New Bitmap(RectImage.Width.ToInt, RectImage.Height.ToInt)

                Using g As Graphics = Graphics.FromImage(PKontaktbild)
                    g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                    g.DrawImage(PAnrMon.Image, 0, 0, RectImage.Width, RectImage.Height)
                End Using
                .DrawImage(PKontaktbild, RectImage.Location)
                DrawRectangleF(e.Graphics, RectImage)
            End If

            ' Zeichne Rechteck für die Zeit
            RectZeit = New RectangleF(RectImage.Right + PCommon.TextPadding.Left,
                                      PCommon.TextPadding.Top + PCommon.HeaderHeight,
                                      .MeasureString(PAnrMon.Uhrzeit.ToString("F"), PCommon.TitleFont).Width,
                                      IHeightOfTitle)

            ' Zeichne Rechteck für den Telefonname 
            RectTelName = New RectangleF(RectZeit.Right + PCommon.TextPadding.Left,
                                         RectZeit.Top,
                                         RectOptions.Left - RectZeit.Right - 2 * PCommon.TextPadding.Left,
                                         IHeightOfTitle)

            ' Zeichne Rechteck für das Telefonnummer
            RectTelNr = New RectangleF(PCommon.TextPadding.Left + RectImage.Right,
                                       RectZeit.Bottom + PCommon.TextPadding.Top,
                                       RectClose.Right - RectImage.Right - PCommon.TextPadding.Right,
                                       IHeightOfTelNr)

            RectFirma = New RectangleF(PCommon.TextPadding.Left + RectImage.Right,
                                       Height - IHeightOfTitle - PCommon.TextPadding.Bottom,
                                       RectClose.Right - RectImage.Right - PCommon.TextPadding.Right,
                                       IHeightOfTitle)

            RectAnrName = New RectangleF(PCommon.TextPadding.Left + RectImage.Right,
                                         RectTelNr.Bottom + PCommon.TextPadding.Top,
                                         RectClose.Right - RectImage.Right - PCommon.TextPadding.Right,
                                         RectFirma.Top - RectTelNr.Bottom - 2 * PCommon.TextPadding.Top)

            ' Inhalt in die Rechtecke schreiben
            .DrawString(PAnrMon.Uhrzeit.ToString("F"), PCommon.TitleFont, New SolidBrush(PCommon.TitleColor), RectZeit, drawFormatLeft)
            .DrawString(PAnrMon.TelName, PCommon.TitleFont, New SolidBrush(PCommon.TitleColor), RectTelName, drawFormatRight)
            .DrawString(PAnrMon.TelNr, PCommon.TelNrFont, New SolidBrush(PCommon.TitleColor), RectTelNr, drawFormatCenter)
            .DrawString(PAnrMon.Firma, PCommon.TitleFont, New SolidBrush(PCommon.TitleColor), RectFirma, drawFormatCenter)

            If .MeasureString(PAnrMon.AnrName, PCommon.AnrNameFont).Width.IsLargerOrEqual(RectAnrName.Width) Then
                ' Verkleinere Die Schriftgröße
                PCommon.AnrNameFont = New Font(PCommon.DefFontName, CSng(7.5 * ScaleFaktor.Height), PCommon.FontStyleBold, PCommon.DefGraphicsUnit, PCommon.DefgdiCharSet)
            End If

            If BMouseOnLink Then
                Cursor = Cursors.Hand
                .DrawString(PAnrMon.AnrName, PCommon.AnrNameFont, New SolidBrush(PCommon.LinkHoverColor), RectAnrName, drawFormatCenter)
            Else
                Cursor = Cursors.Default
                .DrawString(PAnrMon.AnrName, PCommon.AnrNameFont, New SolidBrush(PCommon.ContentColor), RectAnrName, drawFormatCenter)
            End If

            ' Zeichne die Umrandungen für die Felder zum Debuggen
            If ShowBorders Then
                DrawRectangleF(e.Graphics, RectZeit)
                DrawRectangleF(e.Graphics, RectTelName)
                DrawRectangleF(e.Graphics, RectAnrName)
                DrawRectangleF(e.Graphics, RectTelNr)
                DrawRectangleF(e.Graphics, RectFirma)
                'DrawRectangleF(e.Graphics, RectImage)
            End If

        End With
    End Sub
    Private Sub DrawRectangleF(ByVal g As Graphics, ByVal rect As RectangleF)
        Using pen As Pen = New Pen(PCommon.ButtonBorderColor, 1)
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height)
        End Using
    End Sub

    Private Sub AnrMonCommon_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If PKontaktbild IsNot Nothing Then PKontaktbild.Dispose()
        Me.Dispose(True)
    End Sub
#End Region


End Class
