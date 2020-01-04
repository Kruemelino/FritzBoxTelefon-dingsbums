
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization

<Serializable(), XmlRoot("KZ")> Public Class CKennzahlen
    <XmlElement("LKZ")> Public Property Landeskennzahlen As List(Of CLandeskennzahl)
End Class

<Serializable(), XmlType("LKZ")> Public Class CLandeskennzahl
    <XmlAttribute("n")> Public Property Landeskennzahl As String
    <XmlElement("ONKZ")> Public Property Ortsnetzkennzahlen As List(Of COrtsnetzkennzahl)
End Class

<Serializable(), XmlType("O")> Public Class COrtsnetzkennzahl
    <XmlAttribute("n")> Public Property Ortskennzahl As String
End Class

Friend NotInheritable Class CVorwahlen

    Public Shared Property Kennzahlen As CKennzahlen

    Public Sub New()

        Dim XMLDoc As New XmlDocument
        Dim mySerializer As New XmlSerializer(GetType(CKennzahlen))

        If XMLDoc IsNot Nothing Then
            Using Reader As TextReader = New StringReader(My.Resources.Vorwahlen)
                Kennzahlen = CType(mySerializer.Deserialize(Reader), CKennzahlen)
            End Using
        Else
            Kennzahlen = Nothing
        End If

    End Sub
End Class
