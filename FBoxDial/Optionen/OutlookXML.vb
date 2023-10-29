Imports System.Xml.Serialization
<Serializable()>
<XmlRoot("FritzOutlookXML")> Public Class OutlookXML

    <XmlElement("Optionen")> Public Property POptionen As Optionen
    <XmlElement("Telefonie")> Public Property PTelefonie As Telefonie
    '<XmlElement("TelListen")> Public Property PTelListen As TelListen

    Friend Sub New()
        POptionen = New Optionen
        PTelefonie = New Telefonie
        'PTelListen = New TelListen
        With PTelefonie
            .Telefonnummern = New List(Of Telefonnummer)
            .Telefoniegeräte = New List(Of Telefoniegerät)
            .IPTelefone = New List(Of IPPhoneConnector)
        End With
    End Sub

End Class
