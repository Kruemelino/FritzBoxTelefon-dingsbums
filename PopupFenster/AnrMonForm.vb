Public Class F_AnrMon
    Implements IDisposable

#Region "Event"
    Public Event LinkClick(ByVal sender As Object, ByVal e As System.EventArgs)
    Public Event Close(ByVal sender As Object, ByVal e As System.EventArgs)
    Public Event Closed(ByVal sender As Object, ByVal e As System.EventArgs)
    Public Event ToolStripMenuItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs)
#End Region

    Private cmnPrps As New CommonFenster
    Private WithEvents fPopup As New Common_Form(vAnrMon:=Me, vStoppuhr:=Nothing, vCommon:=cmnPrps)
    Private WithEvents tmWait As New Timer

    Private bMouseIsOn As Boolean = False
    Private bAppearing As Boolean = True

    Private i As Integer = 0
    Private iMaxPosition As Integer
    Private dMaxOpacity As Double

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
    Private bShouldRemainVisible As Boolean = False
    Friend Property ShouldRemainVisible() As Boolean
        Get
            Return bShouldRemainVisible
        End Get
        Set(ByVal value As Boolean)
            bShouldRemainVisible = value
        End Set
    End Property

    Private WithEvents ctContextMenu As ContextMenuStrip = Nothing
    Public Property OptionsMenu() As ContextMenuStrip
        Get
            Return ctContextMenu
        End Get
        Set(ByVal value As ContextMenuStrip)
            ctContextMenu = value
        End Set
    End Property

    Private iShowDelay As Integer = 3000
    Public Property ShowDelay() As Integer
        Get
            Return iShowDelay
        End Get
        Set(ByVal value As Integer)
            iShowDelay = value
        End Set
    End Property

    Private szSize As Size = New Size(400, 100)
    Public Property Size() As Size
        Get
            Return szSize
        End Get
        Set(ByVal value As Size)
            szSize = value
        End Set
    End Property

    Private bAutoAusblenden As Boolean = True
    Public Property AutoAusblenden() As Boolean
        Get
            Return bAutoAusblenden
        End Get
        Set(ByVal value As Boolean)
            bAutoAusblenden = value
        End Set
    End Property

    Private szPositionsKorrektur As Size = New Size(0, 0)
    Public Property PositionsKorrektur() As Size
        Get
            Return szPositionsKorrektur
        End Get
        Set(ByVal value As Size)
            szPositionsKorrektur = value
        End Set
    End Property

    Private iAnzAnrMon As Integer = 1
    Public Property AnzAnrMon() As Integer
        Get
            Return iAnzAnrMon
        End Get
        Set(ByVal value As Integer)
            iAnzAnrMon = value
        End Set
    End Property

    Private iAbstandAnrMon As Integer = 10
    Public Property AbstandAnrMon() As Integer
        Get
            Return iAbstandAnrMon
        End Get
        Set(ByVal value As Integer)
            iAbstandAnrMon = value
        End Set
    End Property

    Private bEffektTransparenz As Boolean = True
    Public Property EffektTransparenz() As Boolean
        Get
            Return bEffektTransparenz
        End Get
        Set(ByVal value As Boolean)
            bEffektTransparenz = value
        End Set
    End Property

    Private bEffektMove As Boolean = True
    Public Property EffektMove() As Boolean
        Get
            Return bEffektMove
        End Get
        Set(ByVal value As Boolean)
            bEffektMove = value
        End Set
    End Property

    Private pStartpunkt As eStartPosition
    Public Property Startpunkt() As eStartPosition
        Get
            Return pStartpunkt
        End Get
        Set(ByVal value As eStartPosition)
            pStartpunkt = value
        End Set
    End Property

    Private _MoveDirection As eMoveDirection
    Public Property MoveDirection() As eMoveDirection
        Get
            Return _MoveDirection
        End Get
        Set(ByVal value As eMoveDirection)
            _MoveDirection = value
        End Set
    End Property

    Private ptImagePosition As Point = New Point(12, 32)
    Public Property ImagePosition() As Point
        Get
            Return ptImagePosition
        End Get
        Set(ByVal value As Point)
            ptImagePosition = value
        End Set
    End Property

    Private szImageSize As Size = New Size(48, 48)
    Public Property ImageSize() As Size
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
    Public Property Image() As Image
        Get
            Return imImage
        End Get
        Set(ByVal value As Image)
            imImage = value
        End Set
    End Property

    Private sAnrName As String
    Public Property AnrName() As String
        Get
            Return sAnrName
        End Get
        Set(ByVal value As String)
            sAnrName = value
        End Set
    End Property

    Private sUhrzeit As String
    Public Property Uhrzeit() As String
        Get
            Return sUhrzeit
        End Get
        Set(ByVal value As String)
            sUhrzeit = value
        End Set
    End Property

    Private sTelNr As String
    Public Property TelNr() As String
        Get
            Return sTelNr
        End Get
        Set(ByVal value As String)
            sTelNr = value
        End Set
    End Property

    Private sTelName As String
    Public Property TelName() As String
        Get
            Return sTelName
        End Get
        Set(ByVal value As String)
            sTelName = value
        End Set
    End Property

    Private sFirma As String
    Public Property Firma() As String
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
                    X = Screen.PrimaryScreen.WorkingArea.Left - PositionsKorrektur.Width + AbstandAnrMon
                    Y = Screen.PrimaryScreen.WorkingArea.Bottom - .Height - PositionsKorrektur.Height - AnzAnrMon * .Height - (AnzAnrMon + 1) * AbstandAnrMon
                Case eStartPosition.TopLeft
                    X = Screen.PrimaryScreen.WorkingArea.Left - PositionsKorrektur.Width + AbstandAnrMon
                    Y = Screen.PrimaryScreen.WorkingArea.Top - PositionsKorrektur.Height + AnzAnrMon * .Height + (AnzAnrMon + 1) * AbstandAnrMon
                Case eStartPosition.BottomRight
                    X = Screen.PrimaryScreen.WorkingArea.Right - .Size.Width - PositionsKorrektur.Width - AbstandAnrMon
                    Y = Screen.PrimaryScreen.WorkingArea.Bottom - .Height - PositionsKorrektur.Height - AnzAnrMon * .Height - (AnzAnrMon + 1) * AbstandAnrMon
                Case eStartPosition.TopRight
                    X = Screen.PrimaryScreen.WorkingArea.Right - .Size.Width - PositionsKorrektur.Width - AbstandAnrMon
                    Y = Screen.PrimaryScreen.WorkingArea.Top - PositionsKorrektur.Height + AnzAnrMon * .Height + (AnzAnrMon + 1) * AbstandAnrMon
            End Select

            If bEffektMove Then
                Select Case MoveDirection
                    Case eMoveDirection.X
                        Select Case Startpunkt
                            Case eStartPosition.BottomLeft, eStartPosition.TopLeft ' -->
                                X = Screen.PrimaryScreen.WorkingArea.Left - fPopup.Size.Width + 2
                            Case eStartPosition.BottomRight, eStartPosition.TopRight ' <---
                                X = Screen.PrimaryScreen.WorkingArea.Right + 2
                        End Select
                    Case eMoveDirection.Y
                        Select Case Startpunkt
                            Case eStartPosition.TopLeft, eStartPosition.TopRight
                                Y = Screen.PrimaryScreen.WorkingArea.Top - fPopup.Height + 1 + AnzAnrMon * .Height
                            Case eStartPosition.BottomRight, eStartPosition.BottomLeft
                                Y = Screen.PrimaryScreen.WorkingArea.Bottom - 1 - AnzAnrMon * .Height
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
    End Sub

    ''' <summary>
    ''' Initialisierungsroutine des ehemaligen AnrMonForm. Es wird das ContextMenuStrip an sich initialisiert 
    ''' </summary>
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
        AutoAusblenden = True
        tmWait.Stop()
    End Sub

#Region "Eigene Events"
    Private Sub fPopup_CloseClick() Handles fPopup.CloseClick
        RaiseEvent Close(Me, EventArgs.Empty)
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

        Select Case MoveDirection
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
                        iCurrentlyShown = Screen.PrimaryScreen.WorkingArea.Height - fPopup.Top - AnzAnrMon * fPopup.Height
                    Case eStartPosition.TopLeft, eStartPosition.TopRight
                        iCurrentlyShown = fPopup.Bottom - AnzAnrMon * fPopup.Height
                End Select
                dPourcentOpacity = iCentPurcent / 100 * iCurrentlyShown
        End Select

        Return dPourcentOpacity / 100
    End Function

    Public Sub tmAnimation_Tick()
        Dim StoppPunkt As Boolean = False
        With fPopup
            .Invalidate()
            If bEffektMove Then
                If bAppearing Then 'Einblenden
                    Select Case MoveDirection
                        Case eMoveDirection.X
                            Select Case Startpunkt
                                Case eStartPosition.BottomLeft, eStartPosition.TopLeft
                                    .Left += 2
                                    StoppPunkt = .Left = Screen.PrimaryScreen.WorkingArea.Left - PositionsKorrektur.Width + AbstandAnrMon
                                Case eStartPosition.BottomRight, eStartPosition.TopRight
                                    .Left -= 2
                                    StoppPunkt = .Left = Screen.PrimaryScreen.WorkingArea.Right - fPopup.Size.Width - PositionsKorrektur.Width - AbstandAnrMon
                            End Select
                        Case eMoveDirection.Y
                            Select Case Startpunkt
                                Case eStartPosition.BottomLeft, eStartPosition.BottomRight
                                    .Top -= 1
                                    StoppPunkt = .Top + .Height = Screen.PrimaryScreen.WorkingArea.Bottom - PositionsKorrektur.Height - AnzAnrMon * .Height - (AnzAnrMon + 1) * AbstandAnrMon
                                Case eStartPosition.TopLeft, eStartPosition.TopRight
                                    .Top += 1
                                    StoppPunkt = .Top = Screen.PrimaryScreen.WorkingArea.Top - PositionsKorrektur.Height + AnzAnrMon * .Height + (AnzAnrMon + 1) * AbstandAnrMon
                            End Select
                    End Select

                    If StoppPunkt Then
                        bAppearing = False
                        iMaxPosition = .Top
                        dMaxOpacity = .Opacity
                        If AutoAusblenden Then tmWait.Start()
                    End If

                    Try
                        .Opacity = CDbl(IIf(bEffektTransparenz, GetOpacityBasedOnPosition(), 1))
                    Catch : End Try


                Else 'Ausblenden
                    If Not tmWait.Enabled And AutoAusblenden Then

                        If bMouseIsOn Then
                            .Top = iMaxPosition
                            .Opacity = dMaxOpacity
                            'tmAnimation.Stop()
                            tmWait.Start()
                        Else
                            Select Case MoveDirection
                                Case eMoveDirection.X
                                    Select Case Startpunkt
                                        Case eStartPosition.BottomLeft, eStartPosition.TopLeft
                                            .Left -= 2
                                            StoppPunkt = .Right < Screen.PrimaryScreen.WorkingArea.Left
                                        Case eStartPosition.BottomRight, eStartPosition.TopRight
                                            .Left += 2
                                            StoppPunkt = .Left > Screen.PrimaryScreen.WorkingArea.Right
                                    End Select
                                Case eMoveDirection.Y
                                    Select Case Startpunkt
                                        Case eStartPosition.BottomLeft, eStartPosition.BottomRight
                                            .Top += 1
                                            StoppPunkt = .Top > Screen.PrimaryScreen.WorkingArea.Bottom - PositionsKorrektur.Width - AnzAnrMon * .Height
                                        Case eStartPosition.TopLeft, eStartPosition.TopRight
                                            .Top -= 1
                                            StoppPunkt = .Bottom < Screen.PrimaryScreen.WorkingArea.Top - PositionsKorrektur.Width + AnzAnrMon * .Height
                                    End Select
                            End Select

                            If StoppPunkt Then
                                'tmAnimation.Stop()
                                .TopMost = False
                                .Close()
                                bAppearing = True
                                RaiseEvent Closed(Me, EventArgs.Empty)
                            End If

                            .Opacity = CDbl(IIf(bEffektTransparenz, GetOpacityBasedOnPosition(), 1))
                        End If
                    End If
                End If
            Else
                'Einblenden ohne Bewegung
                If bAppearing Then
                    .Opacity += CDbl(IIf(bEffektTransparenz, 0.05, 1))
                    If .Opacity = 1 Then
                        bAppearing = False
                        If AutoAusblenden Then tmWait.Start()
                    End If
                Else
                    If Not tmWait.Enabled And AutoAusblenden Then
                        If bMouseIsOn Then
                            fPopup.Opacity = 1
                            tmWait.Start()
                        Else
                            .Opacity -= CDbl(IIf(bEffektTransparenz, 0.05, 1))
                            If .Opacity = 0 Then
                                .TopMost = False
                                .Close()
                                bAppearing = True
                                RaiseEvent Closed(Me, EventArgs.Empty)
                            End If
                        End If
                    End If
                End If
            End If
        End With
    End Sub

    Private Sub tmWait_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmWait.Tick
        i += tmWait.Interval
        If i > ShowDelay Then
            tmWait.Stop()
        End If
        fPopup.Invalidate()
    End Sub

    Private Sub fPopup_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles fPopup.MouseEnter
        bMouseIsOn = True
    End Sub

    Private Sub fPopup_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles fPopup.MouseLeave
        If Not ShouldRemainVisible Then bMouseIsOn = False
    End Sub

    Private Sub ctContextMenu_Closed(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripDropDownClosedEventArgs) Handles ctContextMenu.Closed
        ShouldRemainVisible = False
        bMouseIsOn = False
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

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