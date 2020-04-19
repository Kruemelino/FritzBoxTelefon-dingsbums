Imports System.ComponentModel
Imports System.Xml.Serialization

<Serializable()> Public Class FritzBoxXMLNummer
    Implements INotifyPropertyChanged

    <XmlAttribute("type")> Public Property Typ As String
    <XmlAttribute("vanity")> Public Property Vanity As String
    <XmlAttribute("prio")> Public Property Prio As String
    <XmlAttribute("quickdial")> Public Property Schnellwahl As String
    <XmlText()> Public Property Nummer As String

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
End Class
