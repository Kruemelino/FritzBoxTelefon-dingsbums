Imports System.ComponentModel
<DefaultEvent("LinkClick")> _
Public Class PopupNotifier
    Inherits Component

    Event LinkClick()
    Event Close()
    Event Closed()
    Private WithEvents fPopup As New PopupNotifierForm(Me)
    Private WithEvents tmAnimation As New Timer
    Private WithEvents tmWait As New Timer

    Private bAppearing As Boolean = True
    Public bShouldRemainVisible As Boolean = False
    Private i As Integer = 0

    Private Declare Auto Function SetWindowPos Lib "user32" (ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As UInteger) As Boolean
    Private Declare Function ShowWindow Lib "user32" (ByVal hwnd As Long, ByVal nCmdShow As Long) As Long

    ReadOnly HWND_BOTTOM As New IntPtr(1)
    ReadOnly HWND_NOTOPMOST As New IntPtr(-2)
    ReadOnly HWND_TOP As New IntPtr(0)
    ReadOnly HWND_TOPMOST As New IntPtr(-1)
    ReadOnly SWP_NOSIZE As UInteger = 1
    ReadOnly SWP_NOMOVE As UInteger = 2
    ReadOnly SWP_NOACTIVATE As UInteger = 16 '0x0010; 
    ReadOnly SW_SHOWNOACTIVATE = 4 ' Zeigt das Fenster an ohne es zu aktivieren
    ReadOnly DS_SETFOREGROUND As UInteger = &H200 'Danke an Pikachu für den Tipp :)
    Private bMouseIsOn As Boolean = False
    Private iMaxPosition As Integer
    Private dMaxOpacity As Double
    Private dummybool As Boolean



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
    Private iEffektMoveGeschwindigkeit As Integer = 50
    <Category("Appearance"), _
    DefaultValue(50)> _
    Property EffektMoveGeschwindigkeit() As Integer
        Get
            Return iEffektMoveGeschwindigkeit
        End Get
        Set(ByVal value As Integer)
            iEffektMoveGeschwindigkeit = value
        End Set
    End Property
#End Region

    Sub New()
        With fPopup
            .FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            .StartPosition = System.Windows.Forms.FormStartPosition.Manual
            .ShowInTaskbar = True
        End With
    End Sub

    Sub Popup()
        tmWait.Interval = 200
        With fPopup
            .TopMost = True
            .Size = Size
            .Opacity = IIf(bEffektTransparenz, 0, 1)
            .Location = New Point(Screen.PrimaryScreen.WorkingArea.Right - fPopup.Size.Width - 10 - szPosition.Width, IIf(bEffektMove, Screen.PrimaryScreen.WorkingArea.Bottom - 1, Screen.PrimaryScreen.WorkingArea.Bottom - 10 - szPosition.Height - fPopup.Height))
            .Text = AnrName & IIf(TelNr = "", "", " (" & TelNr & ")")
            .Show()
            SetWindowPos(fPopup.Handle.ToInt32, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOACTIVATE + SWP_NOMOVE + SWP_NOSIZE + DS_SETFOREGROUND)
            SetWindowPos(fPopup.Handle.ToInt32, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOACTIVATE + SWP_NOMOVE + SWP_NOSIZE + DS_SETFOREGROUND)

        End With

        tmAnimation.Interval = iEffektMoveGeschwindigkeit
        tmAnimation.Start()
    End Sub

    Sub Hide()
        bMouseIsOn = False
        tmWait.Stop()
        tmAnimation.Start()
    End Sub

    Private Sub fPopup_CloseClick() Handles fPopup.CloseClick
        RaiseEvent Close()
        Me.Finalize()
    End Sub

    Private Sub fPopup_LinkClick() Handles fPopup.LinkClick
        RaiseEvent LinkClick()
    End Sub

    Private Function GetOpacityBasedOnPosition() As Double
        Dim iCentPourcent As Integer = fPopup.Height
        Dim iCurrentlyShown As Integer = Screen.PrimaryScreen.WorkingArea.Height - fPopup.Top
        Dim dPourcentOpacity As Double = iCentPourcent / 100 * iCurrentlyShown
        Return (dPourcentOpacity / 100) ' - 0.05
    End Function

    Private Sub tmAnimation_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmAnimation.Tick
        With fPopup
            .Invalidate()
            If bEffektMove Then
                If bAppearing Then
                    .Top -= 4
                    .Opacity = IIf(bEffektTransparenz, GetOpacityBasedOnPosition(), 1)
                    If .Top + .Height < Screen.PrimaryScreen.WorkingArea.Bottom - 10 - szPosition.Height Then
                        tmAnimation.Stop()
                        bAppearing = False
                        iMaxPosition = .Top
                        dMaxOpacity = .Opacity
                        If bAutoAusblenden Then tmWait.Start()
                    End If
                Else
                    If bMouseIsOn Then
                        .Top = iMaxPosition
                        .Opacity = dMaxOpacity
                        tmAnimation.Stop()
                        tmWait.Start()
                    Else
                        .Top += 3
                        .Opacity = IIf(bEffektTransparenz, GetOpacityBasedOnPosition(), 1)
                        If .Top > Screen.PrimaryScreen.WorkingArea.Bottom Then
                            tmAnimation.Stop()
                            .TopMost = False
                            .Close()
                            bAppearing = True
                            RaiseEvent Closed()
                        End If
                    End If
                End If
            Else
                .Top = Screen.PrimaryScreen.WorkingArea.Bottom - 10 - szPosition.Height - .Height
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
                            RaiseEvent Closed()
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

    Protected Overrides Sub Finalize()
        fPopup.TopMost = False
        MyBase.Finalize()
    End Sub
End Class