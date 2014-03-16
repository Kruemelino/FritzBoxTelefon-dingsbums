Imports System.ComponentModel

<DefaultEvent("LinkClick")> Public Class PopUpAnrMon
    Inherits Component

    Public Event LinkClick(ByVal sender As Object, ByVal e As System.EventArgs)
    Public Event Close(ByVal sender As Object, ByVal e As System.EventArgs)
    Public Event Closed(ByVal sender As Object, ByVal e As System.EventArgs)

    Private WithEvents fPopup As New PopUpAnrMonForm(Me)
    Private WithEvents tmAnimation As New Timer
    Private WithEvents tmWait As New Timer

    Private bAppearing As Boolean = True
    Public bShouldRemainVisible As Boolean = False
    Private i As Integer = 0

    Private bMouseIsOn As Boolean = False
    Private iMaxPosition As Integer
    Private dMaxOpacity As Double
    Private dummybool As Boolean

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
    Private clLinkHover As Color = SystemColors.HotTrack
    <Category("Appearance"), _
    DefaultValue(GetType(Color), "HotTrack")> _
    Property LinkHoverColor() As Color
        Get
            Return clLinkHover
        End Get
        Set(ByVal value As Color)
            clLinkHover = value

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
    Private ftBase As Font = SystemFonts.DialogFont
    <Category("Anrufername")> _
    Property ContentFont() As Font
        Get
            Return ftBase
        End Get
        Set(ByVal value As Font)
            ftBase = value
        End Set
    End Property
    Private ftTitle As Font = SystemFonts.CaptionFont
    <Category("Title")> _
    Property TitleFont() As Font
        Get
            Return ftTitle
        End Get
        Set(ByVal value As Font)
            ftTitle = value

        End Set
    End Property
    Private ftTelNr As Font = SystemFonts.CaptionFont
    <Category("TelNr")> _
    Property TelNrFont() As Font
        Get
            Return ftTelNr
        End Get
        Set(ByVal value As Font)
            ftTelNr = value

        End Set
    End Property
    Private ptImagePosition As Point = New Point(12, 21)
    <Category("Image")> _
    Property ImagePosition() As Point
        Get
            Return ptImagePosition
        End Get
        Set(ByVal value As Point)
            ptImagePosition = value

        End Set
    End Property
    Private szImageSize As Size = New Size(0, 0)
    <Category("Image")> _
    Property ImageSize() As Size
        Get
            If szImageSize.Width = 0 Then
                If Not Image Is Nothing Then
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
    <Category("Image")> _
    Property Image() As Image
        Get
            Return imImage
        End Get
        Set(ByVal value As Image)
            imImage = value

        End Set
    End Property
    Private sAnrName As String
    <Category("Anrufername")> _
    Property AnrName() As String
        Get
            Return sAnrName
        End Get
        Set(ByVal value As String)
            sAnrName = value

        End Set
    End Property
    Private sUhrzeit As String
    <Category("Uhrzeit")> _
    Property Uhrzeit() As String
        Get
            Return sUhrzeit
        End Get
        Set(ByVal value As String)
            sUhrzeit = value

        End Set
    End Property
    Private sTelNr As String
    <Category("TelNr")> _
    Property TelNr() As String
        Get
            Return sTelNr
        End Get
        Set(ByVal value As String)
            sTelNr = value

        End Set
    End Property
    Private sTelName As String
    <Category("TelName")> _
    Property TelName() As String
        Get
            Return sTelName
        End Get
        Set(ByVal value As String)
            sTelName = value

        End Set
    End Property
    Private sFirma As String
    <Category("Firma")> _
    Property Firma() As String
        Get
            Return sFirma
        End Get
        Set(ByVal value As String)
            sFirma = value

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
    Private bCloseButtonVisible As Boolean = True
    <Category("Buttons"), _
    DefaultValue(True)> _
    Property CloseButton() As Boolean
        Get
            Return bCloseButtonVisible
        End Get
        Set(ByVal value As Boolean)
            bCloseButtonVisible = value
        End Set
    End Property
    Private bOptionsButtonVisible As Boolean = False
    <Category("Buttons"), _
    DefaultValue(False)> _
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
    <Category("Behavior"), _
    DefaultValue(3000)> _
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
    <Category("Appearance"), _
    DefaultValue(True)> _
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
    <Category("Appearance"), _
    DefaultValue(5)> _
    Property EffektMoveGeschwindigkeit() As Integer
        Get
            Return iEffektMoveGeschwindigkeit
        End Get
        Set(ByVal value As Integer)
            iEffektMoveGeschwindigkeit = value
        End Set
    End Property

    Private pStartpunkt As eStartPosition
    <Category("Appearance")> _
    Property Startpunkt() As eStartPosition
        Get
            Return pStartpunkt
        End Get
        Set(ByVal value As eStartPosition)
            pStartpunkt = value
        End Set
    End Property

    Private _MoveDirecktion As eMoveDirection
    <Category("Appearance")> _
    Property MoveDirecktion() As eMoveDirection
        Get
            Return _MoveDirecktion
        End Get
        Set(ByVal value As eMoveDirection)
            _MoveDirecktion = value
        End Set
    End Property

#End Region

    Public Sub New()
        With fPopup
            .FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            .StartPosition = System.Windows.Forms.FormStartPosition.Manual
            .ShowInTaskbar = True
        End With
    End Sub

    Public Sub Popup()
        Dim X As Integer
        Dim Y As Integer
        Dim retVal As Boolean

        tmWait.Interval = 200
        With fPopup
            .TopMost = True
            .Size = Size
            .Opacity = IIf(bEffektTransparenz, 0, 1)

            Select Case Startpunkt
                Case eStartPosition.BottomLeft
                    X = Screen.PrimaryScreen.WorkingArea.Left + 10 - PositionsKorrektur.Width
                    Y = Screen.PrimaryScreen.WorkingArea.Bottom - 10 - PositionsKorrektur.Height - fPopup.Height
                Case eStartPosition.TopLeft
                    X = Screen.PrimaryScreen.WorkingArea.Left + 10 - PositionsKorrektur.Width
                    Y = Screen.PrimaryScreen.WorkingArea.Top + 10 - PositionsKorrektur.Height
                Case eStartPosition.BottomRight
                    X = Screen.PrimaryScreen.WorkingArea.Right - fPopup.Size.Width - 10 - PositionsKorrektur.Width
                    Y = Screen.PrimaryScreen.WorkingArea.Bottom - 10 - PositionsKorrektur.Height - fPopup.Height
                Case eStartPosition.TopRight
                    X = Screen.PrimaryScreen.WorkingArea.Right - fPopup.Size.Width - 10 - PositionsKorrektur.Width
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
            .Text = AnrName & IIf(TelNr = "", "", " (" & TelNr & ")")

            retVal = OutlookSecurity.SetWindowPos(.Handle, hWndInsertAfterFlags.HWND_TOPMOST, 0, 0, 0, 0, _
                                                  SetWindowPosFlags.DoNotActivate + _
                                                  SetWindowPosFlags.IgnoreMove + _
                                                  SetWindowPosFlags.IgnoreResize + _
                                                  SetWindowPosFlags.DoNotChangeOwnerZOrder)

            .Show()
        End With

        tmAnimation.Interval = iEffektMoveGeschwindigkeit
        tmAnimation.Start()
    End Sub

    Public Sub Hide()
        bMouseIsOn = False
        tmWait.Stop()
        tmAnimation.Start()
    End Sub

    Private Sub fPopup_CloseClick() Handles fPopup.CloseClick
        RaiseEvent Close(Me, EventArgs.Empty)
        Me.Finalize()
    End Sub

    Private Sub fPopup_LinkClick() Handles fPopup.LinkClick
        RaiseEvent LinkClick(Me, EventArgs.Empty)
    End Sub

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

                    .Opacity = IIf(bEffektTransparenz, GetOpacityBasedOnPosition(), 1)

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

                        .Opacity = IIf(bEffektTransparenz, GetOpacityBasedOnPosition(), 1)
                    End If
                End If
            Else
                If bAppearing Then
                    .Opacity += IIf(bEffektTransparenz, 0.05, 1)
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
                        .Opacity -= IIf(bEffektTransparenz, 0.05, 1)
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

    'Protected Overrides Sub Finalize()
    '    fPopup.TopMost = False
    '    fPopup.Dispose(True)
    'End Sub

End Class