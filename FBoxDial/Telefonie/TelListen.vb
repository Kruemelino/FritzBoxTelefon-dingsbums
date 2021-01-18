Imports System.Xml.Serialization

<Serializable()> Public Class TelListen

    <XmlArray("RINGListe"), XmlArrayItem("Eintrag")> Public Property RINGListe As List(Of Telefonat)
    <XmlArray("CALLListe"), XmlArrayItem("Eintrag")> Public Property CALLListe As List(Of Telefonat)
    <XmlArray("RWSIndex"), XmlArrayItem("Eintrag")> Public Property RWSIndex As List(Of RWSIndexEntry)
    <XmlArray("VIPListe"), XmlArrayItem("Eintrag")> Public Property VIPListe As List(Of VIPEntry)

    ''' <summary>
    ''' Gibt die zuletzt gewählten Telefonnummern der Wahlwiederholungsliste zurück
    ''' </summary>
    ''' <param name="Telefonate">Wahlwiederhohlungsliste</param>
    ''' <returns>Liste der Telefonnummern</returns>
    Friend Function GetTelNrList(Telefonate As List(Of Telefonat)) As List(Of Telefonnummer)
        GetTelNrList = New List(Of Telefonnummer)
        For Each Tel As Telefonat In Telefonate
            GetTelNrList.Add(Tel.GegenstelleTelNr)
        Next
    End Function
End Class
