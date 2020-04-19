Friend Class DGVDatenZeileEMailEintrag
    Implements IComparable(Of DGVDatenZeileEMailEintrag)

    Public Property EMail As String

    Friend Property EMailEintrag As FritzBoxXMLEmail

    Public Function CompareTo(other As DGVDatenZeileEMailEintrag) As Integer Implements IComparable(Of DGVDatenZeileEMailEintrag).CompareTo
        Return EMail.CompareTo(other.EMail)
    End Function
End Class
