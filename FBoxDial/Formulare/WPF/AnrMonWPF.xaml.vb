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
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)

        ' Skalinierung
        'Height *= ScaleFaktor.Height
        'Width *= ScaleFaktor.Width

    End Sub

#Region "Eigenschaften"
    'Private Property ScaleFaktor As SizeF = GetScaling()
    Private ReadOnly Property AbstandAnrMon As Integer = 10
    Private Property IsClosing As Boolean = False
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
        If Now.Subtract(StartTime).Subtract(TotalTimePaused).TotalMilliseconds.IsLargerOrEqual(XMLData.POptionen.PTBEnblDauer * 1000) Then
            NLogger.Debug("Anrufmonitor automatisch nach {0} + {1} Sekunden geschlossen", XMLData.POptionen.PTBEnblDauer, TotalTimePaused.TotalSeconds)
            ' Timer anhalten
            AnrMonTimer.Stop()
            AnrMonTimer.Dispose()
            ' Das Fenster im korrekten synchronen Thread schließen
            Dispatcher.Invoke(Sub()
                                  Close()
                              End Sub)
        End If
    End Sub
    Private Sub AnrMonTest_MouseEnter(sender As Object, e As MouseEventArgs) Handles Me.MouseEnter
        If AnrMonTimer IsNot Nothing Then
            PauseTime = Now
            AnrMonTimer.Enabled = False
            NLogger.Debug("Anrufmonitor: Timer angehalten")
        End If
    End Sub

    Private Sub AnrMonTest_MouseLeave(sender As Object, e As MouseEventArgs) Handles Me.MouseLeave
        If AnrMonTimer IsNot Nothing Then
            TotalTimePaused = TotalTimePaused.Add(Now.Subtract(PauseTime))
            AnrMonTimer.Enabled = Not IsClosing ' Wenn das Fenster geschlossen wird, darf der Timer nicht wieder gestartet werden.
            If AnrMonTimer.Enabled Then NLogger.Debug("Anrufmonitor: Timer nach {0} Sekunden fortgesetzt", Now.Subtract(PauseTime).TotalSeconds)
        End If
    End Sub
#End Region

#Region "Event"
    Public Event Geschlossen(ByVal sender As Object, ByVal e As EventArgs)
#End Region

    ''' <summary>
    ''' Blendet den Anrfmonitor für das übergebene Telefonat (<paramref name="Tlfnt"/>) ein.
    ''' </summary>
    ''' <param name="Tlfnt"></param>
    Friend Sub ShowAnrMon(ByVal Tlfnt As Telefonat)

        ' Fülle das Viewmodel
        SetFormViewModel(Tlfnt)

        ' Timer starten
        If XMLData.POptionen.PCBAutoClose Then
            If AnrMonTimer Is Nothing Then AnrMonTimer = New Timers.Timer
            With AnrMonTimer
                StartTime = Date.Now()
                .Interval = 100
                .Start()
            End With
        End If

        Topmost = True
        ' X-Koordinate
        Left = SystemParameters.WorkArea.Right - Width - AbstandAnrMon

        ' Y-Koordinate
        Top = SystemParameters.WorkArea.Bottom - Height - AbstandAnrMon - ThisAddIn.OffeneAnrMonWPF.Count * (AbstandAnrMon + Height)

        ' Notwendigkeit unklar. Funktioniert aber gut.
        UnsafeNativeMethods.SetWindowPos(New WindowInteropHelper(Me).Handle, HWndInsertAfterFlags.HWND_TOPMOST, 0, 0, 0, 0, CType(SetWindowPosFlags.DoNotActivate + SetWindowPosFlags.IgnoreMove + SetWindowPosFlags.IgnoreResize + SetWindowPosFlags.DoNotChangeOwnerZOrder, SetWindowPosFlags))

        IsClosing = False
        ' Popup einblenden
        Me.Show()
    End Sub

    Private Sub SetFormViewModel(ByVal Tlfnt As Telefonat)
        With CType(DataContext, AnrMonViewModel)
            ' Anruferzeit festlegen: Beginn des Telefonates
            .Zeit = Tlfnt.ZeitBeginn

            'Anrufende Telefonnummer setzen
            If Anrufer Is Nothing OrElse Tlfnt.NrUnterdrückt Then
                ' Kontaktinformationen wurden nicht gefunden oder die Nummer wurde unterdrückt
                .AnrMonTelNr = PDfltStringEmpty
            Else
                ' Kontaktinformationen wurden gefunden
                .AnrMonTelNr = Tlfnt.GegenstelleTelNr?.Formatiert
            End If

            ' Anrufer Name setzen
            If Tlfnt.NrUnterdrückt Then
                ' Die Nummer wurde unterdrückt
                .AnrMonAnrufer = PDfltStringUnbekannt
            Else
                If Anrufer IsNot Nothing Then
                    ' Kontaktinformationen wurden gefunden
                    .AnrMonAnrufer = Tlfnt.Anrufer
                Else
                    ' Kontaktinformationen wurden nicht gefunden
                    .AnrMonAnrufer = Tlfnt.GegenstelleTelNr?.Formatiert
                End If
            End If

            ' Firmeninformationen setzen
            .AnrMonFirma = Tlfnt.Firma

            ' Geräteinformationen setzen
            If Tlfnt.RINGGeräte Is Nothing Then Tlfnt.RINGGeräte = XMLData.PTelefonie.Telefoniegeräte.FindAll(Function(Tel) Tel.StrEinTelNr.Contains(Tlfnt.OutEigeneTelNr))

            .AnrMonTelName = String.Join(", ", Tlfnt.RINGGeräte.Select(Function(Gerät) Gerät.Name).ToList())

            ' Outlook Kontaktelement setzen
            .OKontakt = Tlfnt.OlKontakt

            ' Text für Zwischenablage setzen
            If Tlfnt.NrUnterdrückt Then
                ' Die Nummer wurde unterdrückt
                .AnrMonClipboard = PDfltStringUnbekannt
            Else
                If Tlfnt.Anrufer IsNot Nothing Then
                    ' Kontaktinformationen wurden gefunden
                    .AnrMonClipboard = String.Format("{0} ({1})", Tlfnt.Anrufer, Tlfnt.GegenstelleTelNr?.Formatiert)
                Else
                    ' Kontaktinformationen wurden nicht gefunden
                    .AnrMonClipboard = Tlfnt.GegenstelleTelNr?.Formatiert
                End If
            End If

            ' Kontaktbild setzen
            If Tlfnt.OlKontakt Is Nothing AndAlso (Tlfnt.OutlookKontaktID.IsNotStringEmpty And Tlfnt.OutlookStoreID.IsNotStringEmpty) Then Tlfnt.OlKontakt = GetOutlookKontakt(Tlfnt.OutlookKontaktID, Tlfnt.OutlookStoreID)

            ' Speichere das Kontaktbild in einem temporären Ordner
            Dim BildPfad As String = KontaktBild(Tlfnt.OlKontakt)

            If Not XMLData.POptionen.PCBAnrMonContactImage Or BildPfad.IsStringNothingOrEmpty Then
                ' Bild ausblenden
                AnrBild.Visibility = Visibility.Collapsed
                ' Margin der Textfelder anpassen
                ColBild.Width = New GridLength(4)
            Else
                ' Bild einblenden
                AnrBild.Visibility = Visibility.Visible
                ' Kontaktbild laden
                .Kontaktbild = New BitmapImage
                With .Kontaktbild
                    .BeginInit()
                    .CacheOption = BitmapCacheOption.OnLoad
                    .UriSource = New Uri(BildPfad)
                    .EndInit()
                End With
                'Lösche das Kontaktbild 
                DelKontaktBild(BildPfad)
                ' Breite der Spalte für das Bild anpassen
                ColBild.Width = New GridLength(80)
            End If

        End With
    End Sub

    Friend Sub Update(ByVal Tlfnt As Telefonat)
        Dispatcher.Invoke(Sub()
                              SetFormViewModel(Tlfnt)
                          End Sub)
    End Sub

    '#Region "Skalierung"
    '    ' https://inchoatethoughts.com/scaling-your-user-interface-in-a-wpf-application

    '    Public Shared ReadOnly ScaleValueProperty As DependencyProperty = DependencyProperty.Register("ScaleValue", GetType(Double), GetType(AnrMonWPF), New UIPropertyMetadata(1.0, New PropertyChangedCallback(AddressOf OnScaleValueChanged), New CoerceValueCallback(AddressOf OnCoerceScaleValue)))

    '    Private Shared Function OnCoerceScaleValue(ByVal o As DependencyObject, ByVal value As Object) As Object
    '        Dim mainWindow As AnrMonWPF = TryCast(o, AnrMonWPF)

    '        If mainWindow IsNot Nothing Then
    '            Return mainWindow.OnCoerceScaleValue(CDbl(value))
    '        Else
    '            Return value
    '        End If
    '    End Function

    '    Private Shared Sub OnScaleValueChanged(ByVal o As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
    '        Dim mainWindow As AnrMonWPF = TryCast(o, AnrMonWPF)
    '        If mainWindow IsNot Nothing Then mainWindow.OnScaleValueChanged(CDbl(e.OldValue), CDbl(e.NewValue))
    '    End Sub

    '    Protected Overridable Function OnCoerceScaleValue(ByVal value As Double) As Double
    '        If Double.IsNaN(value) Then Return 1.0F
    '        value = Math.Max(0.1, value)
    '        Return value
    '    End Function

    '    Protected Overridable Sub OnScaleValueChanged(ByVal oldValue As Double, ByVal newValue As Double)
    '    End Sub

    '    Public Property ScaleValue As Double
    '        Get
    '            Return CDbl(GetValue(ScaleValueProperty))
    '        End Get
    '        Set(ByVal value As Double)
    '            SetValue(ScaleValueProperty, value)
    '        End Set
    '    End Property

    '    Private Sub MainGrid_SizeChanged(ByVal sender As Object, ByVal e As EventArgs)
    '        ScaleValue = CDbl(OnCoerceScaleValue(AnrMon, Math.Min(ScaleFaktor.Width, ScaleFaktor.Height)))
    '    End Sub

    '#End Region

    Private Sub BClose_Click(sender As Object, e As RoutedEventArgs) Handles bClose.Click
        NLogger.Debug("Anrufmonitor manuell geschlossen")
        If AnrMonTimer IsNot Nothing Then
            AnrMonTimer.Stop()
            NLogger.Debug("Timer für Anrufmonitor manuell angehalten")
        End If

        IsClosing = True
        ' Schließt das Fenster
        Close()
    End Sub

    Private Sub AnrMonWPF_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        RaiseEvent Geschlossen(Me, EventArgs.Empty)
        'If AnrMonTimer IsNot Nothing Then AnrMonTimer.Dispose()
    End Sub

    Private Sub BOptionen_MouseEnter(sender As Object, e As MouseEventArgs)
        OptionPopup.StaysOpen = True
    End Sub

    Private Sub BOptionen_MouseLeave(sender As Object, e As MouseEventArgs)
        OptionPopup.StaysOpen = False
    End Sub

    Private Sub BReCall_Click(sender As Object, e As RoutedEventArgs)
        CType(DataContext, Telefonat).Rückruf()
    End Sub

    Private Sub BContact_Click(sender As Object, e As RoutedEventArgs)
        CType(DataContext, Telefonat).ZeigeKontakt()
    End Sub

    Private Sub BCopy_Click(sender As Object, e As RoutedEventArgs)
        With CType(DataContext, AnrMonViewModel)
            Clipboard.SetText(.AnrMonClipboard)
        End With
    End Sub

    Private Sub Anrufer_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        With CType(DataContext, AnrMonViewModel)
            Clipboard.SetText(.AnrMonClipboard)
        End With
    End Sub
End Class
