Imports System.ComponentModel
Imports System.Drawing
Imports System.Threading
Imports System.Timers
Imports System.Windows
Imports System.Windows.Input
Imports System.Windows.Interop
Imports System.Windows.Markup
Imports System.Windows.Media.Imaging

Public Class AnrMonWPF
    Inherits Window

    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)

        ' Skalinierung
        Height *= ScaleFaktor.Height
        Width *= ScaleFaktor.Width

    End Sub

#Region "EigenSchaften"
    Private Property ScaleFaktor As SizeF = GetScaling()
    Private ReadOnly Property AbstandAnrMon As Integer = 10

#End Region

#Region "Timer"
    ''' <summary>
    ''' Timer für das automatische Ausblenden des Anrufmonitors.
    ''' So bald die gewählte Zeit erreicht ist, wird der Anrtufmonitor ausgeblendet.
    ''' Wenn die Maus sich auf dem Fenster befindet, wird der Timer unterbrochen.
    ''' Sobald sich die Maus vom dem Fenster entfernt, wird der Timer fortgesetzt.
    ''' </summary>
    Private WithEvents AnrMonTimer As Timers.Timer
    Private Property StartTime As Date
    Private Property PauseTime As Date
    Private Property TotalTimePaused As TimeSpan

    Private Sub AnrMonTimer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles AnrMonTimer.Elapsed
        If Now.Subtract(StartTime).Subtract(TotalTimePaused).TotalMilliseconds.IsLargerOrEqual(XMLData.POptionen.PTBEnblDauer * 1000) Then Dispatcher.Invoke(Sub()
                                                                                                                                                                 AnrMonClose()
                                                                                                                                                             End Sub)
    End Sub
    Private Sub AnrMonTest_MouseEnter(sender As Object, e As MouseEventArgs) Handles Me.MouseEnter
        PauseTime = Now
        AnrMonTimer.Enabled = False
    End Sub

    Private Sub AnrMonTest_MouseLeave(sender As Object, e As MouseEventArgs) Handles Me.MouseLeave
        TotalTimePaused = TotalTimePaused.Add(Now.Subtract(PauseTime))
        AnrMonTimer.Enabled = True
    End Sub
#End Region

#Region "Event"
    Public Event Geschlossen(ByVal sender As Object, ByVal e As EventArgs)
#End Region

    Friend Sub Popup(ByVal Tlfnt As Telefonat)
        Dim AnruferBildPfad As String = Tlfnt.AnrMonImagePfad

        DataContext = Tlfnt

        If Not XMLData.POptionen.PCBAnrMonContactImage Or AnruferBildPfad.IsStringNothingOrEmpty Then
            ' Bild ausblenden
            AnrBild.Visibility = Visibility.Collapsed
            ' Margin der Textfelder anpassen
            ColBild.Width = New GridLength(4)
        Else
            ' Bild einblenden
            AnrBild.Visibility = Visibility.Visible
            ' Kontaktbild laden
            Dim bI As New BitmapImage
            With bI
                .BeginInit()
                .CacheOption = BitmapCacheOption.OnLoad
                .UriSource = New Uri(AnruferBildPfad)
                .EndInit()
            End With
            AnrBild.Source = bI
            'Lösche das Kontaktbild 
            DelKontaktBild(AnruferBildPfad)
            ' Breite der Spalte für das Bild anpassen
            ColBild.Width = New GridLength(80)
        End If

        ' Timer starten
        If XMLData.POptionen.PCBAutoClose Then
            If AnrMonTimer Is Nothing Then AnrMonTimer = New Timers.Timer
            With AnrMonTimer
                StartTime = Date.Now()
                .Start()
            End With
        End If

        Topmost = True
        ' X-Koordinate
        Left = SystemParameters.WorkArea.Right - Width - AbstandAnrMon

        ' Y-Koordinate
        Top = SystemParameters.WorkArea.Bottom - Height - AbstandAnrMon - ThisAddIn.OffenePopUps.Count * (AbstandAnrMon + Height)

        ' Popup einblenden
        Dim retVal As Boolean = UnsafeNativeMethods.SetWindowPos(New WindowInteropHelper(Me).Handle, HWndInsertAfterFlags.HWND_TOPMOST, 0, 0, 0, 0, CType(SetWindowPosFlags.DoNotActivate + SetWindowPosFlags.IgnoreMove + SetWindowPosFlags.IgnoreResize + SetWindowPosFlags.DoNotChangeOwnerZOrder, SetWindowPosFlags))

        Me.Show()
    End Sub

    Friend Sub Update(ByVal Tlfnt As Telefonat)
        DataContext = Tlfnt
    End Sub

    Friend Sub AnrMonClose()

        AnrMonTimer?.Stop()
        'AnrMonTimer?.Dispose()
        Close()

    End Sub

#Region "Skalierung"
    ' https://inchoatethoughts.com/scaling-your-user-interface-in-a-wpf-application

    Public Shared ReadOnly ScaleValueProperty As DependencyProperty = DependencyProperty.Register("ScaleValue", GetType(Double), GetType(AnrMonWPF), New UIPropertyMetadata(1.0, New PropertyChangedCallback(AddressOf OnScaleValueChanged), New CoerceValueCallback(AddressOf OnCoerceScaleValue)))

    Private Shared Function OnCoerceScaleValue(ByVal o As DependencyObject, ByVal value As Object) As Object
        Dim mainWindow As AnrMonWPF = TryCast(o, AnrMonWPF)

        If mainWindow IsNot Nothing Then
            Return mainWindow.OnCoerceScaleValue(CDbl(value))
        Else
            Return value
        End If
    End Function

    Private Shared Sub OnScaleValueChanged(ByVal o As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        Dim mainWindow As AnrMonWPF = TryCast(o, AnrMonWPF)
        If mainWindow IsNot Nothing Then mainWindow.OnScaleValueChanged(CDbl(e.OldValue), CDbl(e.NewValue))
    End Sub

    Protected Overridable Function OnCoerceScaleValue(ByVal value As Double) As Double
        If Double.IsNaN(value) Then Return 1.0F
        value = Math.Max(0.1, value)
        Return value
    End Function

    Protected Overridable Sub OnScaleValueChanged(ByVal oldValue As Double, ByVal newValue As Double)
    End Sub

    Public Property ScaleValue As Double
        Get
            Return CDbl(GetValue(ScaleValueProperty))
        End Get
        Set(ByVal value As Double)
            SetValue(ScaleValueProperty, value)
        End Set
    End Property

    Private Sub MainGrid_SizeChanged(ByVal sender As Object, ByVal e As EventArgs)
        ScaleValue = CDbl(OnCoerceScaleValue(AnrMon, Math.Min(ScaleFaktor.Width, ScaleFaktor.Height)))
    End Sub

#End Region

    Private Sub BClose_Click(sender As Object, e As RoutedEventArgs) Handles bClose.Click
        AnrMonClose()
    End Sub

    Private Sub AnrMonWPF_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        RaiseEvent Geschlossen(Me, EventArgs.Empty)

    End Sub

End Class
