Imports System.Drawing
Imports System.Windows.Forms

Public Class FormAnrMon


#Region "Event"
    Public Event LinkClick(ByVal sender As Object, ByVal e As EventArgs)
    Public Event Schließen(ByVal sender As Object, ByVal e As EventArgs)
    Public Event Geschlossen(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ToolStripMenuItemClicked(ByVal sender As Object, ByVal e As ToolStripItemClickedEventArgs)
#End Region
#Region "Properties"
    Private Property CmnPrps As New CommonFenster
    Private WithEvents FPopup As New AnrMonCommon(Me, CmnPrps)

    Private Property CompContainer As New System.ComponentModel.Container()

    Private WithEvents AnrMonContextMenuStrip As New ContextMenuStrip(CompContainer)
    Private Property ToolStripMenuItemKontaktöffnen As New ToolStripMenuItem()
    Private Property ToolStripMenuItemRückruf As New ToolStripMenuItem()
    Private Property ToolStripMenuItemKopieren As New ToolStripMenuItem()

    Private WithEvents CtContextMenu As ContextMenuStrip = Nothing
    Friend Property OptionsMenu() As ContextMenuStrip
        Get
            Return CtContextMenu
        End Get
        Set(ByVal value As ContextMenuStrip)
            CtContextMenu = value
        End Set
    End Property

    Friend Property Size() As Size = New Size(400, 100)
    Friend Property AutoAusblenden() As Boolean = True
    Friend Property AnzAnrMon() As Integer = 1
    Friend Property AbstandAnrMon() As Integer = 10
    ' folgende Properties ggf. wieder auf Public setzen:
    Friend Property Image() As Image = Nothing
    Friend Property AnrName() As String
    Friend Property Uhrzeit() As Date
    Friend Property TelNr() As String
    Friend Property TelName() As String
    Friend Property Firma() As String

#Region "Timer"
    ''' <summary>
    ''' Timer für das automatische Ausblenden des Anrufmonitors.
    ''' So bald die gewählte Zeit erreicht ist, wird der Anrtufmonitor ausgeblendet.
    ''' Wenn die Maus sich auf dem Fenster befindet, wird der Timer unterbrochen.
    ''' Sobald sich die Maus vom dem Fenster entfernt, wird der Timer fortgesetzt.
    ''' </summary>
    Private WithEvents AnrMonTimer As Timer
    Private Property StartTime As Date
    Private Property PauseTime As Date
    Private Property TotalTimePaused As TimeSpan
    Friend Property Anzeigedauer() As Integer = 10000
#End Region

#End Region

    Public Sub New()
        With FPopup
            .FormBorderStyle = FormBorderStyle.None
            .StartPosition = FormStartPosition.Manual
            .ShowInTaskbar = True
        End With
        InitializeComponentContextMenuStrip()
    End Sub

    Public Sub Popup()
        Dim X As Integer
        Dim Y As Integer
        Dim retVal As Boolean

        ' Timer starten
        If AutoAusblenden Then
            If AnrMonTimer Is Nothing Then AnrMonTimer = New Timer
            With AnrMonTimer
                StartTime = Date.Now()
                .Start()
            End With
        End If

        ' Popup einblenden
        With FPopup
            .TopMost = True
            .Size = Size
            .ScaleFaktor = GetScaling()
            ' X-Koordinate
            X = Screen.PrimaryScreen.WorkingArea.Right - AbstandAnrMon - CInt(.ScaleFaktor.Width * .Size.Width)

            ' Y-Koordinate
            Y = Screen.PrimaryScreen.WorkingArea.Bottom - AnzAnrMon * (AbstandAnrMon + (CInt(.ScaleFaktor.Height * .Size.Height)))

            .Location = New Point(X, Y)

            retVal = UnsafeNativeMethods.SetWindowPos(.Handle, HWndInsertAfterFlags.HWND_TOPMOST, 0, 0, 0, 0, CType(SetWindowPosFlags.DoNotActivate + SetWindowPosFlags.IgnoreMove + SetWindowPosFlags.IgnoreResize + SetWindowPosFlags.DoNotChangeOwnerZOrder, SetWindowPosFlags))

            .Show()
        End With
    End Sub

    ''' <summary>
    ''' Initialisierungsroutine des ehemaligen AnrMonForm. Es wird das ContextMenuStrip an sich initialisiert 
    ''' </summary>
    Private Sub InitializeComponentContextMenuStrip()
        '
        'ContextMenuStrip
        '
        With AnrMonContextMenuStrip
            .Items.AddRange(New ToolStripItem() {ToolStripMenuItemKontaktöffnen, ToolStripMenuItemRückruf, ToolStripMenuItemKopieren})
            .Name = "AnrMonContextMenuStrip"
            .RenderMode = ToolStripRenderMode.System
            '.Size = New Size(222, 70)
        End With

        With ToolStripMenuItemKontaktöffnen
            .ImageScaling = ToolStripItemImageScaling.None
            .Name = "ToolStripMenuItemKontaktöffnen"
            .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
        End With

        With ToolStripMenuItemRückruf
            .ImageScaling = ToolStripItemImageScaling.None
            .Name = "ToolStripMenuItemRückruf"
            .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
        End With

        With ToolStripMenuItemKopieren
            .ImageScaling = ToolStripItemImageScaling.None
            .Name = "ToolStripMenuItemKopieren"
            .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
        End With

        OptionsMenu = AnrMonContextMenuStrip
    End Sub

    Friend Sub Invalidate()
        FPopup.Invalidate()
    End Sub

    Friend Sub Close()

        If AnrMonTimer IsNot Nothing Then
            AnrMonTimer.Stop()
            AnrMonTimer.Dispose()
        End If

        FPopup.Dispose()

        RaiseEvent Geschlossen(Me, EventArgs.Empty)
    End Sub

#Region "Eigene Events"
    Private Sub FPopup_CloseClick() Handles FPopup.CloseClick
        RaiseEvent Schließen(Me, EventArgs.Empty)
    End Sub
    Private Sub FPopup_LinkClick() Handles FPopup.LinkClick
        RaiseEvent LinkClick(Me, EventArgs.Empty)
    End Sub
    Private Sub FPopup_ToolStripMenuItemRückrufClickClick(ByVal sender As Object, ByVal e As ToolStripItemClickedEventArgs) Handles CtContextMenu.ItemClicked
        RaiseEvent ToolStripMenuItemClicked(Me, e)
    End Sub
#End Region

    Private Sub AnrMonTimer_Tick(sender As Object, e As EventArgs) Handles AnrMonTimer.Tick
        If Now.Subtract(StartTime).Subtract(TotalTimePaused).TotalMilliseconds.IsLargerOrEqual(Anzeigedauer) Then Close()
    End Sub

    Private Sub FPopup_MouseEnter(ByVal sender As Object, ByVal e As EventArgs) Handles FPopup.MouseEnter
        PauseTime = Now
        AnrMonTimer.Enabled = False
    End Sub

    Private Sub FPopup_MouseLeave(ByVal sender As Object, ByVal e As EventArgs) Handles FPopup.MouseLeave
        TotalTimePaused = TotalTimePaused.Add(Now.Subtract(PauseTime))
        AnrMonTimer.Enabled = True
    End Sub

    Private Sub CtContextMenu_Closed(ByVal sender As Object, ByVal e As ToolStripDropDownClosedEventArgs) Handles CtContextMenu.Closed

    End Sub

End Class
