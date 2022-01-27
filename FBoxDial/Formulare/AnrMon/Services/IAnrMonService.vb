Imports System.Windows.Media.Imaging

Public Interface IAnrMonService

    Sub RemoveMissedCall(MissedCall As MissedCallViewModel)
    Function LadeBild(AnrMonTelefonat As Telefonat) As Threading.Tasks.Task(Of BitmapImage)
    Sub BlockNumber(TelNr As Telefonnummer)

End Interface
