Imports System.Xml.Serialization
<Serializable()>
<XmlRoot("List")> Public Class SIPClientList
    <XmlElement("Item")> Public Property SIPClientList As List(Of SIPClient)
End Class
