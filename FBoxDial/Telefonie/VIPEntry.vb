Imports System.Xml.Serialization
Imports Microsoft.Office.Interop

<Serializable()> Public Class VIPEntry
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

End Class