Imports System.Xml.Serialization

<Serializable()> Public Class Telefonie

    <XmlElement("Telefonnummer")> Public Property Telefonnummern As List(Of Telefonnummer)
    <XmlElement("Telefoniegerät")> Public Property Telefoniegeräte As List(Of Telefoniegerät)

    ''' <summary>
    ''' Ortskennzahl des Telefonanschlusses. Wird automatisch ermittelt. Kann in den Einstellungen überschrieben werden.
    ''' </summary>
    <XmlElement("TBOrtsKZ")> Public Property OKZ As String

    ''' <summary>
    ''' Landeskennzahl der Telefonanschlusses. Wird automatisch ermittelt. Kann in den Einstellungen überschrieben werden.
    ''' </summary>
    <XmlElement("TBLandesKZ")> Public Property LKZ As String

    Public Sub New()
        Telefonnummern = New List(Of Telefonnummer)
        Telefoniegeräte = New List(Of Telefoniegerät)

    End Sub

    ''' <summary>
    ''' Fügt eine neue eigene Telefonnummer hinzu, falls sie noch nicht exisiert, und gieb sie zurück.
    ''' Falls die Nummer schon in der Liste enthalten ist, gib diese zurück.
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer als Zeichenfolge</param>
    ''' <returns>Telefonnummer</returns>
    Friend Function AddEigeneTelNr(TelNr As String) As Telefonnummer

        AddEigeneTelNr = Telefonnummern.Find(Function(Nummer) Nummer.Equals(TelNr))

        If AddEigeneTelNr Is Nothing Then
            AddEigeneTelNr = New Telefonnummer With {.EigeneNummer = True, .Ortskennzahl = OKZ, .Landeskennzahl = LKZ, .SetNummer = TelNr}
            Telefonnummern.Add(AddEigeneTelNr)
        End If
    End Function

    ''' <summary>
    ''' Gibt die Telefonnummer zurück, die der übergebenen Zeichenfolge entspricht
    ''' </summary>
    ''' <param name="TelNr">Telefonnummer als Zeichenfolge</param>
    ''' <returns>Telefonnummer</returns>
    Friend Function GetNummer(TelNr As String) As Telefonnummer
        Return Telefonnummern.Find(Function(Tel) Tel.Equals(TelNr))
    End Function

    Friend Sub GetKennzahlen()
        Dim OutPutData As Collections.Hashtable
        Using fbSOAP As New FritzBoxSOAP

            ' Landeskennzahl ermitteln: X_AVM-DE_GetVoIPCommonCountryCode
            OutPutData = fbSOAP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_GetVoIPCommonCountryCode")
            If Not OutPutData.Contains("Error") Then LKZ = OutPutData.Item("NewX_AVM-DE_LKZ").ToString()

            ' Ortskennzahl ermitteln: X_AVM-DE_GetVoIPCommonAreaCode
            OutPutData = fbSOAP.Start(KnownSOAPFile.x_voipSCPD, "X_AVM-DE_GetVoIPCommonAreaCode")
            If Not OutPutData.Contains("Error") Then OKZ = OutPutData.Item("NewX_AVM-DE_OKZ").ToString()

        End Using
    End Sub


End Class
