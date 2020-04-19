Imports System.Xml.Serialization
Imports System.ComponentModel
<Serializable()> Public Class FritzBoxXMLEmail
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    <XmlAttribute("classifier")> Public Property Klassifizierer As String
    <XmlText()> Public Property EMail As String


End Class
