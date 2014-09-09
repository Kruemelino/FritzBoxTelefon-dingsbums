Public Class F_AnrMon

    Private cmnPrps As New CommonFenster
    Public Event LinkClick(ByVal sender As Object, ByVal e As System.EventArgs)
    Public Event Close(ByVal sender As Object, ByVal e As System.EventArgs)
    Public Event Closed(ByVal sender As Object, ByVal e As System.EventArgs)

    Public Event ToolStripMenuItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs)

    Private WithEvents fPopup As New Common_Form(vAnrMon:=Me, vStoppuhr:=Nothing, vCommon:=cmnPrps)
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

    Private iEffektMoveGeschwindigkeit As Integer = 5
    Public Property EffektMoveGeschwindigkeit() As Integer
        Get
            Return iEffektMoveGeschwindigkeit
        End Get
        Set(ByVal value As Integer)
            iEffektMoveGeschwindigkeit = value
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
    Public Property MoveDirecktion() As eMoveDirection
        Get
            Return _MoveDirection
        End Get
        Set(ByVal value As eMoveDirection)
            _MoveDirection = value
        End Set
    End Property

    Private ptImagePosition As Point = New Point(12, 32) 'New Point(12, 21)
    Public Property ImagePosition() As Point
        Get
            Return ptImagePosition
        End Get
        Set(ByVal value As Point)
            ptImagePosition = value

        End Set
    End Property

    Private szImageSize As Size = New Size(48, 48) 'New Size(0, 0)
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

        'tmAnimation.Interval = 1 'iEffektMoveGeschwindigkeit
        'tmAnimation.Start()
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
        'tmAnimation.Start()
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

    Public Sub tmAnimation_Tick() '(ByVal sender As Object, ByVal e As System.EventArgs) ' Handles tmAnimation.Tick
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
                        'tmAnimation.Stop()
                        bAppearing = False
                        iMaxPosition = .Top
                        dMaxOpacity = .Opacity
                        If bAutoAusblenden Then tmWait.Start()
                    End If

                    Try
                        .Opacity = CDbl(IIf(bEffektTransparenz, GetOpacityBasedOnPosition(), 1))
                    Catch : End Try


                Else 'Ausblenden
                    If Not tmWait.Enabled Then

                        If bMouseIsOn Then
                            .Top = iMaxPosition
                            .Opacity = dMaxOpacity
                            'tmAnimation.Stop()
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
                If bAppearing Then
                    .Opacity += CDbl(IIf(bEffektTransparenz, 0.05, 1))
                    If .Opacity = 1 Then
                        'tmAnimation.Stop()
                        bAppearing = False
                        If bAutoAusblenden Then tmWait.Start()
                    End If
                Else
                    If Not tmWait.Enabled Then
                        If bMouseIsOn Then
                            fPopup.Opacity = 1
                            'tmAnimation.Stop()
                            tmWait.Start()
                        Else
                            .Opacity -= CDbl(IIf(bEffektTransparenz, 0.05, 1))
                            If .Opacity = 0 Then
                                'tmAnimation.Stop()
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
            'tmAnimation.Start()
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
        'tmAnimation.Start()
    End Sub

End Class