Public Class AnrMonService
    Implements IAnrMonService
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend Sub BlockNumbers(TelNr As Telefonnummer) Implements IAnrMonService.BlockNumbers
        BlockNumbers(TelNr)
    End Sub
End Class
