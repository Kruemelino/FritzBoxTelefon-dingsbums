Imports System.Data

Friend Class WählClientDataRow
    Inherits DataRow

    Friend Property TelNr As Telefonnummer

    Public Sub New(ByVal rb As DataRowBuilder)
        MyBase.New(rb)
    End Sub
End Class

