Public Class AnrMonService
    Implements IAnrMonService
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Friend Sub BlockNumber(TelNr As Telefonnummer) Implements IAnrMonService.BlockNumber
        AddNrToBlockList(TelNr)
    End Sub
End Class
