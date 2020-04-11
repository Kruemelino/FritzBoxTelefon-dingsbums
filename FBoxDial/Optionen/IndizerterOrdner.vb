Imports System.Xml.Serialization
<Serializable()>
Public Class IndizerterOrdner
    Implements IComparable(Of IndizerterOrdner)
    Implements IEquatable(Of IndizerterOrdner)

    <XmlElement> Public Property FolderID As String
    <XmlElement> Public Property StoreID As String
    <XmlAttribute> Public Property Name As String

    Public Function CompareTo(other As IndizerterOrdner) As Integer Implements IComparable(Of IndizerterOrdner).CompareTo
        Return other.StoreID.CompareTo(StoreID) And other.FolderID.CompareTo(FolderID)
    End Function

    Public Overloads Function Equals(ByVal other As IndizerterOrdner) As Boolean Implements IEquatable(Of IndizerterOrdner).Equals
        If other Is Nothing Then Return False
        Return FolderID = other.FolderID AndAlso StoreID = other.StoreID
    End Function

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Return Equals(TryCast(obj, IndizerterOrdner))
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return (FolderID, StoreID).GetHashCode()
    End Function

End Class
