Imports System.Xml.Serialization
Imports Microsoft.Office.Interop

Public Enum OutlookOrdnerVerwendung As Integer
    KontaktSuche = 1
    KontaktSpeichern = 2
    JournalSpeichern = 4
End Enum

<Serializable()>
Public Class OutlookOrdner
    'Implements IComparable(Of IndizerterOrdner)
    Implements IEquatable(Of OutlookOrdner)

    Public Sub New()

    End Sub

    Public Sub New(OlFolder As Outlook.MAPIFolder, Verwendung As OutlookOrdnerVerwendung)

        FolderID = OlFolder.EntryID
        StoreID = OlFolder.StoreID
        Name = OlFolder.Name
        Typ = Verwendung

    End Sub

    <XmlElement> Public Property FolderID As String
    <XmlElement> Public Property StoreID As String
    <XmlAttribute> Public Property Name As String
    <XmlAttribute> Public Property Typ As OutlookOrdnerVerwendung

    <XmlIgnore> Friend Property MAPIFolder As Outlook.MAPIFolder
        Get
            Return GetOutlookFolder(FolderID, StoreID)
        End Get
        Set
            FolderID = Value.EntryID
            StoreID = Value.StoreID
        End Set
    End Property

    Public Overloads Function Equals(other As OutlookOrdner) As Boolean Implements IEquatable(Of OutlookOrdner).Equals
        If other Is Nothing Then Return False
        Return FolderID.AreEqual(other.FolderID) And StoreID.AreEqual(other.StoreID) And Typ.Equals(other.Typ)
    End Function

    Public Overloads Function Equals(other As Outlook.MAPIFolder, Verwendung As OutlookOrdnerVerwendung) As Boolean
        If other Is Nothing Then Return False
        Return FolderID.AreEqual(other.EntryID) And StoreID.AreEqual(other.StoreID) And Typ.Equals(Verwendung)
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return (FolderID, StoreID).GetHashCode()
    End Function


End Class
