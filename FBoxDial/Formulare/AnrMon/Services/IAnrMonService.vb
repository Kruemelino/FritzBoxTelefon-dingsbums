Imports System.Windows.Media.Imaging

Public Interface IAnrMonService

    Function LadeBild(AnrMonTelefonat As Telefonat) As Threading.Tasks.Task(Of BitmapImage)

    Sub BlockNumber(TelNr As Telefonnummer)

End Interface
