Imports System.Xml.Serialization
Imports System.ComponentModel
<Serializable()> Public Class FritzBoxXMLKontakt
    Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    <XmlElement("category")> Public Property Kategorie As String
    <XmlElement("person")> Public Property Person As FritzBoxXMLPerson
    <XmlElement("uniqueid")> Public Property Uniqueid As String
    <XmlElement("telephony")> Public Property Telefonie As FritzBoxXMLTelefonie
End Class
