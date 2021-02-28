Imports System.Xml.Serialization
Imports Microsoft.Office.Interop

<Serializable()> Public Class VIPEntry
    '<XmlElement> Public Property VCard As String
    <XmlAttribute> Public Property Name As String
    <XmlElement> Public Property EntryID As String
    <XmlElement> Public Property StoreID As String
    <XmlIgnore> Public Property OlContact() As Outlook.ContactItem

End Class

