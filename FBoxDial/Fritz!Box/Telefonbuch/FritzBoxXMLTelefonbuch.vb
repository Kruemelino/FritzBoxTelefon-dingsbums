Imports System.Xml.Serialization
Imports System.ComponentModel
<Serializable()> Public Class FritzBoxXMLTelefonbuch
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    <XmlAttribute("owner")> Public Property Owner As String
    <XmlAttribute("name")> Public Property Name As String
    <XmlElement("timestamp")> Public Property Zeitstempel As String
    <XmlElement("contact")> Public Property Kontakte As List(Of FritzBoxXMLKontakt)
    <XmlIgnore> Friend Property ID As String
End Class
