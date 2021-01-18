Imports System.Data

<Obsolete> Public Class TelGeräteListDataRow
    Inherits DataRow

    Friend Property Gerät As Telefoniegerät

    Public Sub New(rb As DataRowBuilder)
        MyBase.New(rb)
    End Sub
End Class

