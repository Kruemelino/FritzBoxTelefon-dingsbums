Imports System.Timers

Public Class F_StoppUhr
    Implements IDisposable

    Private cmnPrps As New CommonFenster
    Private WithEvents fStoppUhr As New Common_Form(vAnrMon:=Nothing, vStoppuhr:=Me, vCommon:=cmnPrps)
    Private WithEvents TimerZeit As New Timer
    Private WithEvents TimerSchließen As New Timer
    Private Stoppwatch As New Stopwatch
    Private i As Integer = 0
    Public Event Close(ByVal sender As Object, ByVal e As System.EventArgs)
    Delegate Sub SchließeStoppUhr()

#Region "Properties"

    Private sZeit As String
    Property Zeit() As String
        Get
            Return sZeit
        End Get
        Set(ByVal value As String)
            sZeit = value
        End Set
    End Property

    Private sAnruf As String
    Property Anruf() As String
        Get
            Return sAnruf
        End Get
        Set(ByVal value As String)
            sAnruf = value
        End Set
    End Property

    Private sRichtung As String
    Property Richtung() As String
        Get
            Return sRichtung
        End Get
        Set(ByVal value As String)
            sRichtung = value
        End Set
    End Property

    Private sWarteZeit As Integer
    Property WarteZeit() As Integer
        Get
            Return sWarteZeit
        End Get
        Set(ByVal value As Integer)
            sWarteZeit = value
        End Set
    End Property

    Private sStartZeit As String
    Property StartZeit() As String
        Get
            Return sStartZeit
        End Get
        Set(ByVal value As String)
            sStartZeit = value
        End Set
    End Property

    Private sEndeZeit As String
    Property EndeZeit() As String
        Get
            Return sEndeZeit
        End Get
        Set(ByVal value As String)
            sEndeZeit = value
        End Set
    End Property

    Private sMSN As String
    Property MSN() As String
        Get
            Return sMSN
        End Get
        Set(ByVal value As String)
            sMSN = value
        End Set
    End Property

    Private szSize As Size = New Size(250, 100)
    Property Size() As Size
        Get
            Return szSize
        End Get
        Set(ByVal value As Size)
            szSize = value
        End Set
    End Property

    Private szStartPosition As Point = New Point(0, 0)
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
        With fStoppUhr
            .FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            .StartPosition = System.Windows.Forms.FormStartPosition.Manual
            .ShowInTaskbar = True
        End With
    End Sub

    Sub Popup()
        Dim retVal As Boolean
        With fStoppUhr
            .TopMost = True
            .Size = Size
            .Location = StartPosition
            .Show()

            retVal = OutlookSecurity.SetWindowPos(.Handle, hWndInsertAfterFlags.HWND_TOPMOST, 0, 0, 0, 0, _
                                      CType(SetWindowPosFlags.DoNotActivate + _
                                      SetWindowPosFlags.IgnoreMove + _
                                      SetWindowPosFlags.IgnoreResize + _
                                      SetWindowPosFlags.DoNotChangeOwnerZOrder, SetWindowPosFlags))
        End With
    End Sub

    Public Sub StoppuhrStart()

        With TimerZeit
            .Interval = 50
            .Start()
        End With
        Stoppwatch.Start()
    End Sub

    Public Sub StoppuhrStopp()
        Dim Zeit As String
        With System.DateTime.Now
            Zeit = String.Format("{0:00}:{1:00}:{2:00}", .Hour, .Minute, .Second)
        End With
        EndeZeit = Zeit
        fStoppUhr.Invalidate()
        TimerZeit.Stop()
        Stoppwatch.Stop()
        If Not sWarteZeit = -1 Then
            TimerSchließen = New Timer
            With TimerSchließen
                .Interval = sWarteZeit * 1000
                .AutoReset = True
                .Start()
            End With
        End If
    End Sub

    Private Sub timerZeit_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TimerZeit.Elapsed
        With Stoppwatch.Elapsed
            Zeit = String.Format("{0:00}:{1:00}:{2:00}", .Hours, .Minutes, .Seconds)
        End With
        fStoppUhr.Invalidate()
    End Sub

    Private Sub TimerSchließen_Elapsed(ByVal sender As Object, ByVal e As System.EventArgs) Handles TimerSchließen.Elapsed, fStoppUhr.CloseClick 'Ehemals: System.Timers.ElapsedEventArgs

        TimerSchließen.Stop()
        TimerSchließen = Nothing
        Stoppwatch.Stop()
        TimerZeit.Close()
        Stoppwatch = Nothing
        TimerZeit = Nothing
        StartPosition = fStoppUhr.Location
        AutoSchließen()
        RaiseEvent Close(Me, EventArgs.Empty)
        Me.Finalize()
    End Sub

    Sub AutoSchließen()
        If fStoppUhr.InvokeRequired Then
            Dim D As New SchließeStoppUhr(AddressOf AutoSchließen)
            fStoppUhr.Invoke(D)
        Else
            fStoppUhr.Close()
        End If
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: Verwalteten Zustand löschen (verwaltete Objekte).
            End If
            fStoppUhr.Dispose()
            'cmnPrps.Dispose()
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