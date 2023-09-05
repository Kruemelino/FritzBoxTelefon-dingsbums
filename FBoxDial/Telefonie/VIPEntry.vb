Imports System.Xml.Serialization
Imports Microsoft.Office.Interop

<Serializable()> Public Class VIPEntry
    Implements IEqualityComparer(Of VIPEntry)
    Implements IEquatable(Of VIPEntry)

    '<XmlElement> Public Property VCard As String
    <XmlAttribute> Public Property Name As String
    <XmlElement> Public Property EntryID As String
    <XmlElement> Public Property StoreID As String
    <XmlIgnore> Public Property OlContact() As Outlook.ContactItem

#Region "IEquatable Support"
    Public Overloads Function Equals(other As VIPEntry) As Boolean Implements IEquatable(Of VIPEntry).Equals
        Return EntryID.IsEqual(other.EntryID) AndAlso StoreID.IsEqual(other.StoreID)
    End Function
#End Region

#Region "IEqualityComparer Support"
    Public Overloads Function Equals(x As VIPEntry, y As VIPEntry) As Boolean Implements IEqualityComparer(Of VIPEntry).Equals
        Return x.Equals(y)
    End Function

    Public Overloads Function GetHashCode(obj As VIPEntry) As Integer Implements IEqualityComparer(Of VIPEntry).GetHashCode

        ' Check whether the object is null.
        If obj Is Nothing Then Return 0

        Return If(obj.Name Is Nothing, 0, obj.Name.GetHashCode())
    End Function
#End Region

End Class

