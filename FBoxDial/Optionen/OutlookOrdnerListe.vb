Imports System.Xml.Serialization

<Serializable()>
Public Class OutlookOrdnerListe

    <XmlElement("Ordner")> Public Property OrdnerListe As List(Of OutlookOrdner)

    Public Sub New()
        OrdnerListe = New List(Of OutlookOrdner)
    End Sub

    Friend Function Contains(ByVal StoreID As String, ByVal FolderID As String) As Boolean
        Return GetFolder(StoreID, FolderID) IsNot Nothing
    End Function

    Friend Function GetFolder(ByVal StoreID As String, ByVal FolderID As String) As OutlookOrdner
        Return OrdnerListe.Find(Function(Eintrag) Eintrag.FolderID.AreEqual(FolderID) And Eintrag.StoreID.AreEqual(StoreID))
    End Function

End Class
