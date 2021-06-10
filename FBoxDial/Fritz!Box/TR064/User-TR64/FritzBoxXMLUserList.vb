Imports System.Xml.Serialization
<XmlRoot("List"), XmlType("List")> Public Class FritzBoxXMLUserList
    <XmlElement("Username")> Public Property UserListe As List(Of FritzBoxXMLUser)

    <XmlIgnore> Friend ReadOnly Property GetLastUsedUser As FritzBoxXMLUser
        Get
            Return UserListe.Find(Function(User) User.LastUser.IsNotZero)
        End Get
    End Property
End Class
