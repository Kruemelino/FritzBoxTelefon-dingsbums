Imports System.Xml.Serialization
Imports Microsoft.Office.Interop

<Serializable()>
Public Class IndizieOrdnerListe

    <XmlElement("Ordner")> Public Property OrdnerListe As List(Of IndizerterOrdner)

    Public Sub New()
        OrdnerListe = New List(Of IndizerterOrdner)
    End Sub

    Friend Function Contains(ByVal StoreID As String, ByVal FolderID As String) As Boolean

        Return GetFolder(StoreID, FolderID) IsNot Nothing
    End Function

    Friend Function GetFolder(ByVal StoreID As String, ByVal FolderID As String) As IndizerterOrdner
        Return OrdnerListe.Find(Function(eintrag) eintrag.FolderID.AreEqual(FolderID) And eintrag.StoreID.AreEqual(StoreID))
    End Function

End Class
