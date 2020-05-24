Imports System.Xml.Serialization
Imports Microsoft.Office.Interop

<Serializable()>
Public Class OutlookOrdnerListe

    <XmlElement("Ordner")> Public Property OrdnerListe As List(Of OutlookOrdner)

    Public Sub New()
        OrdnerListe = New List(Of OutlookOrdner)
    End Sub

    Friend Sub AddRange(ByVal ListeOutlookOrdner As List(Of OutlookOrdner))
        OrdnerListe.AddRange(ListeOutlookOrdner)
    End Sub


    Friend Function Exists(ByVal MAPIFolder As Outlook.MAPIFolder, ByVal Verwendung As OutlookOrdnerVerwendung) As Boolean
        Return OrdnerListe.Exists(Function(fldr) fldr.MAPIFolder.AreEqual(MAPIFolder) And fldr.Typ = Verwendung)
    End Function
    Friend Function Find(ByVal Verwendung As OutlookOrdnerVerwendung) As OutlookOrdner
        Return OrdnerListe.Find(Function(fldr) fldr.Typ = Verwendung)
    End Function

    Friend Function Find(ByVal StoreID As String, ByVal FolderID As String, ByVal Verwendung As OutlookOrdnerVerwendung) As OutlookOrdner
        Return OrdnerListe.Find(Function(fldr) fldr.FolderID.AreEqual(FolderID) And fldr.StoreID.AreEqual(StoreID) And fldr.Typ = Verwendung)
    End Function

    Friend Function FindAll(ByVal Verwendung As OutlookOrdnerVerwendung) As List(Of OutlookOrdner)
        Return OrdnerListe.FindAll(Function(fldr) fldr.Typ = Verwendung)
    End Function

    Friend Sub RemoveAll(ByVal Verwendung As OutlookOrdnerVerwendung)
        OrdnerListe.RemoveAll(Function(OlFldr) OlFldr.Typ = Verwendung)
    End Sub



End Class
