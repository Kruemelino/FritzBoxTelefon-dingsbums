Imports System.Data

Public Class TelGeräteListDataRow
    Inherits DataRow

    Friend Property Gerät As Telefoniegerät

    Public Sub New(ByVal rb As DataRowBuilder)
        MyBase.New(rb)
    End Sub
End Class

