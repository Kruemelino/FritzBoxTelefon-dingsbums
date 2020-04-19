Imports System.Xml.Serialization
Imports System.ComponentModel
<Serializable()> Public Class FritzBoxXMLPerson
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    <XmlElement("realName")> Public Property RealName As String
    <XmlElement("imageURL")> Public Property ImageURL As String
End Class
