Imports System.Xml.Serialization
<Serializable()>
<XmlRoot("FritzOutlookXML")> Public Class OutlookXML

    <XmlElement("Optionen")> Public Property POptionen As Optionen
    <XmlElement("Telefonie")> Public Property PTelefonie As Telefonie


    Friend Sub New()
        POptionen = New Optionen
        PTelefonie = New Telefonie

        With PTelefonie
            .Telefonnummern = New List(Of Telefonnummer)
            .Telefoniegeräte = New List(Of Telefoniegerät)
            .IPTelefone = New List(Of IPPhoneConnector)
        End With
    End Sub

End Class
