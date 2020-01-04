Imports System.Xml.Serialization

<Serializable()> Public Class XRingListe
    <XmlElement("Eintrag")> Public Property Einträge As List(Of Telefonat)
End Class

<Serializable()> Public Class XCallListe
    <XmlElement("Eintrag")> Public Property Einträge As List(Of Telefonat)
End Class

<Serializable()> Public Class XRWSIndex
    <XmlElement("Eintrag")> Public Property Einträge As List(Of RWSIndexEntry)
End Class

<Serializable()> Public Class XVIP
    <XmlElement("Eintrag")> Public Property Einträge As List(Of VIPEntry)
End Class