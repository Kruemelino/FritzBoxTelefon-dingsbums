Imports System.Windows.Media.Imaging


Public Interface IAnrMonService
#Region "Styling"
    Sub GetColors(ByRef BackgroundColor As String, ByRef ForeColor As String, TelNr As Telefonnummer, IsStoppUhr As Boolean)
#End Region

#Region "MissedCallPane"
    Sub RemoveMissedCall(MissedCall As MissedCallViewModel)
#End Region

    Function LadeBild(AnrMonTelefonat As Telefonat) As Threading.Tasks.Task(Of BitmapImage)
    Sub BlockNumber(TelNr As Telefonnummer)

    Function GetEigeneTelNr(TelNr As String) As Telefonnummer

#Region "TAM Messages"
    Sub PlayMessage(MessageURL As String)
    Sub StoppMessage(MessageURL As String)
    Function CompleteURL(PathSegment As String) As String

    Event SoundFinished As EventHandler(Of NotifyEventArgs(Of String))
#End Region

End Interface
