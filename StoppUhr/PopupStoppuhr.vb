Imports System.ComponentModel
Imports System.Timers

Public Class Stoppuhr
    Inherits Component
    Private WithEvents fStopUhr As New PopupStoppuhrForm(Me)
    Private WithEvents TimerZeit As New Timer
    Private WithEvents TimerSchlieﬂen As New Timer
    Private Stoppwatch As New Stopwatch
    Private i As Integer = 0
    Event Close()
    Delegate Sub SchlieﬂeStoppUhr()

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
    Private ftBase As Font = SystemFonts.DialogFont
    <Category("Zeit")> _
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
    Private szSize As Size = New Size(200, 100)
    <Category("Appearance")> _
    Property Size() As Size
        Get
            Return szSize
        End Get
        Set(ByVal value As Size)
            szSize = value
        End Set
    End Property
    Private szStartPosition As Size = New Point(0, 0)
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
        With fStopUhr
            .TopMost = True
            .Size = Size
            .Location = StartPosition
            .Show()
        End With
    End Sub

    Sub StoppuhrStart()

        With TimerZeit
            .Interval = 250
            .Start()
        End With
        Stoppwatch.Start()
    End Sub

    Sub StoppuhrStopp()
        EndeZeit = CStr(System.DateTime.Now)
        fStopUhr.Invalidate()
        TimerZeit.Stop()
        Stoppwatch.Stop()
        If Not sWarteZeit = -1 Then
            TimerSchlieﬂen = New Timer
            With TimerSchlieﬂen
                .Interval = sWarteZeit * 1000
                .AutoReset = True
                .Start()
            End With
        End If
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub timerZeit_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerZeit.Elapsed
        With Stoppwatch.Elapsed
            Zeit = String.Format("{0:00}:{1:00}:{2:00}", .Hours, .Minutes, .Seconds)
        End With
        fStopUhr.Invalidate()
    End Sub

    Private Sub fPopup_CloseClick() Handles fStopUhr.CloseClickStoppUhr
        Stoppwatch.Stop()
        TimerZeit.Close()
        TimerSchlieﬂen.Close()
        Stoppwatch = Nothing
        TimerZeit = Nothing
        TimerSchlieﬂen = Nothing
        StartPosition = fStopUhr.Location
        AutoSchlieﬂen()
        RaiseEvent Close()
        Me.Finalize()
    End Sub

    Private Sub TimerSchlieﬂen_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerSchlieﬂen.Elapsed
        TimerSchlieﬂen.Stop()
        TimerSchlieﬂen = Nothing
        Stoppwatch.Stop()
        TimerZeit.Close()
        Stoppwatch = Nothing
        TimerZeit = Nothing
        StartPosition = fStopUhr.Location
        AutoSchlieﬂen()
        RaiseEvent Close()
        Me.Finalize()
    End Sub

    Sub AutoSchlieﬂen()
        If fStopUhr.InvokeRequired Then
            Dim D As New SchlieﬂeStoppUhr(AddressOf AutoSchlieﬂen)
            fStopUhr.Invoke(D)
        Else
            fStopUhr.Close()
        End If
    End Sub
End Class
