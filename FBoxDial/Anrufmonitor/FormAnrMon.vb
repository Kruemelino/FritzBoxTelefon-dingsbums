Imports System.Drawing
Imports System.Windows.Forms

Public Class FormAnrMon
    Implements IDisposable

#Region "Event"
    Public Event LinkClick(ByVal sender As Object, ByVal e As EventArgs)
    Public Event Close(ByVal sender As Object, ByVal e As EventArgs)
    Public Event Closed(ByVal sender As Object, ByVal e As EventArgs)
    Public Event ToolStripMenuItemClicked(ByVal sender As Object, ByVal e As ToolStripItemClickedEventArgs)
#End Region

    Private Property CmnPrps As New CommonFenster
    Private WithEvents FPopup As New Common_Form(Me, CmnPrps)
    Private WithEvents TmWait As New Timer

    Private Property BMouseIsOn As Boolean = False
    Private Property BAppearing As Boolean = True

    Private Property I As Integer = 0
    Private Property CompContainer As New System.ComponentModel.Container()
    Private WithEvents AnrMonContextMenuStrip As New ContextMenuStrip(CompContainer)
    Private Property ToolStripMenuItemKontaktöffnen As New ToolStripMenuItem()
    Private Property ToolStripMenuItemRückruf As New ToolStripMenuItem()
    Private Property ToolStripMenuItemKopieren As New ToolStripMenuItem()

#Region "Properties"
    Friend Property ShouldRemainVisible() As Boolean = False
    Friend Property FromCloseed As Boolean = False
    Private WithEvents CtContextMenu As ContextMenuStrip = Nothing
    Friend Property OptionsMenu() As ContextMenuStrip
        Get
            Return CtContextMenu
        End Get
        Set(ByVal value As ContextMenuStrip)
            CtContextMenu = value
        End Set
    End Property
    Friend Property ShowDelay() As Integer = 3000
    Friend Property Size() As Size = New Size(400, 100)
    Friend Property AutoAusblenden() As Boolean = True
    Friend Property AnzAnrMon() As Integer = 1
    Friend Property AbstandAnrMon() As Integer = 10
    Public Property Image() As Image = Nothing
    Public Property AnrName() As String
    Public Property Uhrzeit() As Date
    Public Property TelNr() As String
    Public Property TelName() As String
    Public Property Firma() As String

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

        TmWait.Interval = 200
        With FPopup
            .TopMost = True
            .Size = Size
            .ScaleFaktor = GetScaling()
            X = Screen.PrimaryScreen.WorkingArea.Right - CInt(.ScaleFaktor.Width * (.Size.Width)) - AbstandAnrMon
            Y = Screen.PrimaryScreen.WorkingArea.Bottom - CInt(.ScaleFaktor.Height * (.Size.Height)) - AbstandAnrMon

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
        With Me.AnrMonContextMenuStrip
            .Items.AddRange(New ToolStripItem() {Me.ToolStripMenuItemKontaktöffnen, Me.ToolStripMenuItemRückruf, Me.ToolStripMenuItemKopieren})
            .Name = "AnrMonContextMenuStrip"
            .RenderMode = ToolStripRenderMode.System
            '.Size = New Size(222, 70)
        End With

        With Me.ToolStripMenuItemKontaktöffnen
            .ImageScaling = ToolStripItemImageScaling.None
            .Name = "ToolStripMenuItemKontaktöffnen"
            .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
        End With

        With Me.ToolStripMenuItemRückruf
            .ImageScaling = ToolStripItemImageScaling.None
            .Name = "ToolStripMenuItemRückruf"
            .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
        End With

        With Me.ToolStripMenuItemKopieren
            .ImageScaling = ToolStripItemImageScaling.None
            .Name = "ToolStripMenuItemKopieren"
            .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
        End With
        Me.OptionsMenu = Me.AnrMonContextMenuStrip
    End Sub

    Public Sub Hide()
        bMouseIsOn = False
        AutoAusblenden = True
        tmWait.Stop()
    End Sub

#Region "Eigene Events"
    Private Sub FPopup_CloseClick() Handles FPopup.CloseClick
        RaiseEvent Close(Me, EventArgs.Empty)
    End Sub

    Private Sub FPopup_LinkClick() Handles FPopup.LinkClick
        RaiseEvent LinkClick(Me, EventArgs.Empty)
    End Sub
    Private Sub FPopup_ToolStripMenuItemRückrufClickClick(ByVal sender As Object, ByVal e As ToolStripItemClickedEventArgs) Handles CtContextMenu.ItemClicked
        RaiseEvent ToolStripMenuItemClicked(Me, e)
    End Sub
#End Region

    Public Sub TmAnimation_Tick()
        With FPopup
            .Invalidate()
            If bAppearing Then
                bAppearing = False
                If AutoAusblenden Then TmWait.Start()
            Else
                If Not TmWait.Enabled And AutoAusblenden Then
                    If bMouseIsOn Then
                        TmWait.Start()
                    Else
                        .TopMost = False
                        .Close()
                        FromCloseed = True
                        bAppearing = True
                        RaiseEvent Closed(Me, EventArgs.Empty)
                    End If
                End If
            End If
        End With
    End Sub

    Private Sub TmWait_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles TmWait.Tick
        i += TmWait.Interval
        If i.IsLarger(ShowDelay) Then TmWait.Stop()
        FPopup.Invalidate()
    End Sub

    Private Sub FPopup_MouseEnter(ByVal sender As Object, ByVal e As EventArgs) Handles FPopup.MouseEnter
        bMouseIsOn = True
    End Sub

    Private Sub FPopup_MouseLeave(ByVal sender As Object, ByVal e As EventArgs) Handles FPopup.MouseLeave
        If Not ShouldRemainVisible Then bMouseIsOn = False
    End Sub

    Private Sub CtContextMenu_Closed(ByVal sender As Object, ByVal e As ToolStripDropDownClosedEventArgs) Handles CtContextMenu.Closed
        ShouldRemainVisible = False
        bMouseIsOn = False
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                fPopup.Close()
                With tmWait
                    .Stop()
                    .Dispose()
                End With
            End If

            cmnPrps.Dispose()
            CompContainer.Dispose()
            AnrMonContextMenuStrip.Dispose()
            ToolStripMenuItemKontaktöffnen.Dispose()
            ToolStripMenuItemRückruf.Dispose()
            ToolStripMenuItemKopieren.Dispose()
        End If
        Me.disposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
        Dispose(False)
        MyBase.Finalize()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
