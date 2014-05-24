Public Class formStoppUhr
    Implements IDisposable


    Friend Property StUhrClosed() As Boolean
        Get
            Return V_StUhrClosed
        End Get
        Set(ByVal value As Boolean)
            V_StUhrClosed = value
        End Set
    End Property

    Friend Property Position() As System.Drawing.Point
        Get
            Return V_Position
        End Get
        Set(ByVal value As System.Drawing.Point)
            V_Position = value
        End Set
    End Property

    Private V_StUhrClosed As Boolean
    Private V_Position As System.Drawing.Point

    Public Sub New(ByVal Anrufer As String, ByVal sz As String, ByVal sRichtung As String, ByVal WarteZeit As Integer, ByVal PositionStart As System.Drawing.Point, ByVal sMSN As String)
        InitializeComponent()

        With PopUpStoppUhr
            .Anruf = Anrufer
            .StartZeit = sz
            .WarteZeit = WarteZeit
            .StartPosition = PositionStart
            .StoppuhrStart()
            .Richtung = sRichtung
            .Popup()
            .MSN = sMSN
        End With
    End Sub

    Public Sub Stopp()
        PopUpStoppUhr.StoppuhrStopp()
    End Sub

    Private Sub Stoppuhr_Close() Handles PopUpStoppUhr.Close
        Position = PopUpStoppUhr.StartPosition
        StUhrClosed = True
        Me.Finalize()
    End Sub
End Class