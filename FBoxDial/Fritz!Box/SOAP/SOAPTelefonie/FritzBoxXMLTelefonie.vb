Imports System.Xml.Serialization
<Serializable()> Public Class FritzBoxXMLTelefonie

    <XmlElement("services")> Public Property Dienste As FritzBoxXMLServices
    <XmlElement("number")> Public Property Nummern As List(Of FritzBoxXMLNummer)

    Friend ReadOnly Property GetFirstNumber As FritzBoxXMLNummer
        Get
            Return If(Nummern.Any, Nummern.First, Nothing)
        End Get
    End Property
End Class
