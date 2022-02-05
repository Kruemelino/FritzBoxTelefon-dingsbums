Imports System.Windows.Media.Imaging

Public Interface IAnrMonService

    Sub RemoveMissedCall(MissedCall As MissedCallViewModel)
    Function LadeBild(AnrMonTelefonat As Telefonat) As Threading.Tasks.Task(Of BitmapImage)
    Sub BlockNumber(TelNr As Telefonnummer)


    Sub PlayMessage(MessageURL As String)
    Sub StoppMessage(MessageURL As String)
    Function CompleteURL(PathSegment As String) As String

    Event SoundFinished As EventHandler(Of NotifyEventArgs(Of String))
End Interface
