Friend Class DGVDatenZeileNummernEintrag
    Implements IComparable(Of DGVDatenZeileNummernEintrag)

    Public Property Typ As String
    'Public Property Vanity As String
    Public Property Prio As Boolean
    'Public Property Schnellwahl As String
    Public Property Nummer As String

    Friend Property FBoxXMLNummer As FritzBoxXMLNummer
    Friend Property FBoxXMLKontakt As FritzBoxXMLKontakt

    Public Sub New()
        Nummer = PDfltStringEmpty
        FBoxXMLNummer = New FritzBoxXMLNummer
    End Sub

    Public Function CompareTo(other As DGVDatenZeileNummernEintrag) As Integer Implements IComparable(Of DGVDatenZeileNummernEintrag).CompareTo
        Return Nummer.CompareTo(other.Nummer)
    End Function
End Class
