Public Class formStoppUhr
    Public StUhrClosed As Boolean
    Public Position As System.Drawing.Point

    Public Sub New(ByVal Anrufer As String, ByVal sz As String, ByVal sRichtung As String, ByVal WarteZeit As Integer, ByVal PositionStart As System.Drawing.Point, ByVal sMSN As String)
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        With Stoppuhr
            .Anruf = Anrufer
            .StartZeit = sz
            .WarteZeit = WarteZeit
            .StartPosition = PositionStart
            .StoppuhrStart()
            .Richtung = sRichtung
            .Popup()
            .MSN = sMSN
        End With

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
    End Sub

    Public Sub Stopp()
        Stoppuhr.StoppuhrStopp()
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub Stoppuhr_Close() Handles Stoppuhr.Close
        Position = Stoppuhr.StartPosition
        StUhrClosed = True
        Me.Finalize()
    End Sub
End Class