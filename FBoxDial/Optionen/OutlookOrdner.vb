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

    <XmlElement> Public Property FolderID As String
    <XmlElement> Public Property StoreID As String
    <XmlAttribute> Public Property Name As String
    <XmlAttribute> Public Property Typ As OutlookOrdnerVerwendung

    <XmlIgnore> Friend Property MAPIFolder As Outlook.MAPIFolder
        Get
            Return GetOutlookFolder(FolderID, StoreID)
        End Get
        Set(value As Outlook.MAPIFolder)
            FolderID = value.EntryID
            StoreID = value.StoreID
        End Set
    End Property

    'Public Function CompareTo(other As IndizerterOrdner) As Integer Implements IComparable(Of IndizerterOrdner).CompareTo
    '    Return other.StoreID.CompareTo(StoreID) And other.FolderID.CompareTo(FolderID)
    'End Function

    Public Overloads Function Equals(ByVal other As OutlookOrdner) As Boolean Implements IEquatable(Of OutlookOrdner).Equals
        If other Is Nothing Then Return False
        Return FolderID = other.FolderID AndAlso StoreID = other.StoreID
    End Function

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Return Equals(TryCast(obj, OutlookOrdner))
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return (FolderID, StoreID).GetHashCode()
    End Function

End Class
