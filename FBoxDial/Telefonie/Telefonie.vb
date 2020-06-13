Imports System.Xml.Serialization

<Serializable()> Public Class Telefonie

    <XmlElement("Telefonnummer")> Public Property Telefonnummern As List(Of Telefonnummer)
    <XmlElement("Telefoniegerät")> Public Property Telefoniegeräte As List(Of Telefoniegerät)
    '<XmlElement("LetzterAnrufer")> Public Property LetzterAnrufer As Telefonat

    <XmlArray("RINGListe"), XmlArrayItem("Eintrag")> Public Property RINGListe As List(Of Telefonat)
    <XmlArray("CALLListe"), XmlArrayItem("Eintrag")> Public Property CALLListe As List(Of Telefonat)
    <XmlArray("RWSIndex"), XmlArrayItem("Eintrag")> Public Property RWSIndex As List(Of RWSIndexEntry)
    <XmlArray("VIPListe"), XmlArrayItem("Eintrag")> Public Property VIPListe As List(Of VIPEntry)

    Public Sub New()
        Telefonnummern = New List(Of Telefonnummer)
        Telefoniegeräte = New List(Of Telefoniegerät)
    End Sub

    Friend Function AddNewTelNrStr(ByVal TelNrStr As String) As Telefonnummer

        AddNewTelNrStr = Telefonnummern.Find(Function(Nummer) Nummer.Equals(TelNrStr))

        If AddNewTelNrStr Is Nothing Then
            AddNewTelNrStr = New Telefonnummer With {.SetNummer = TelNrStr}
            Telefonnummern.Add(AddNewTelNrStr)
        End If
    End Function

    Friend Function GetNummer(ByVal TelNrStr As String) As Telefonnummer
        Return Telefonnummern.Find(Function(Tel) Tel.Equals(TelNrStr))
    End Function

End Class
