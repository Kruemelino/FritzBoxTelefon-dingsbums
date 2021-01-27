Imports System.Xml.Serialization
<Serializable()>
<XmlRoot("List")> Public Class SIPTelNrList

    <XmlElement("Item")> Public Property TelNrList As List(Of SIPTelNr)

    Public Sub New()
        TelNrList = New List(Of SIPTelNr)
    End Sub

End Class
