Public Class AnrMonService
    Implements IAnrMonService
    Friend Sub BlockNumber(TelNr As Telefonnummer) Implements IAnrMonService.BlockNumber
        AddNrToBlockList(TelNr)
    End Sub
End Class
