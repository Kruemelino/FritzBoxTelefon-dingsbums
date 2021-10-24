Imports System.Xml.Serialization

Namespace TR064
    <Serializable()>
    <XmlRoot("List")> Public Class SIPClientList

        <XmlElement("Item")> Public Property SIPClients As List(Of SIPClient)

        Public Sub New()
            SIPClients = New List(Of SIPClient)
        End Sub

    End Class
End Namespace
