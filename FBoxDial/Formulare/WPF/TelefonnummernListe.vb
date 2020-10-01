Public Class TelefonnummernListe
    Inherits BindableBase

    Public Property Name As String

    Public Property Einträge As New ObservableCollectionEx(Of Telefonnummer)

    Sub FillDummyData()

        Name = "Testname Testname Testname Testname Testname Testname Testname Testname"

        Einträge.Add(New Telefonnummer With {.SetNummer = "03520880303"})
    End Sub
End Class
