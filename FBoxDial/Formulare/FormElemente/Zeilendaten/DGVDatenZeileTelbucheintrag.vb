Friend Class DGVDatenZeileTelbucheintrag
    Implements IComparable(Of DGVDatenZeileTelbucheintrag)

    Public Property Uniqueid As String
    Public Property RealName As String
    Public Property Nummer As String
    Public Property Typ As String

    Friend Property Telefonbucheintrag As FritzBoxXMLKontakt

    Public Function CompareTo(other As DGVDatenZeileTelbucheintrag) As Integer Implements IComparable(Of DGVDatenZeileTelbucheintrag).CompareTo
        Return Uniqueid.CompareTo(other.Uniqueid)
    End Function
End Class
