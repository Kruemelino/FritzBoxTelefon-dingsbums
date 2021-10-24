Imports System.Xml.Serialization

Namespace TR064
    <Serializable()>
    <XmlRoot("List")> Public Class TAMList

        <XmlElement("TAMRunning")> Public Property TAMRunning As Boolean
        <XmlElement("Stick")> Public Property Stick As UShort
        <XmlElement("Status")> Public Property Status As UShort
        <XmlElement("Capacity")> Public Property Capacity As Integer
        <XmlElement("Item")> Public Property TAMListe As List(Of TAMItem)

        Public Sub New()
            TAMListe = New List(Of TAMItem)
        End Sub
    End Class
End Namespace