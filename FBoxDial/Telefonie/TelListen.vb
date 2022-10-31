Imports System.Xml.Serialization
Imports System.Reflection

<Serializable()> Public Class TelListen

    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

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

    ''' <summary>
    ''' Entfernt alle Elemente der gewünschten Liste
    ''' </summary>
    ''' <param name="KeyDelete">Name der Liste. Es sind nur die Zeichenfolgen RINGListe, CALLListe und VIPListe erlaubt.</param>
    Friend Sub ClearList(KeyDelete As String)

        ' Liste anhand des übergeben Parameter ermitteln
        Dim ListPropertyInfo As PropertyInfo = Array.Find([GetType].GetProperties, Function(PropertyInfo As PropertyInfo) PropertyInfo.Name.IsEqual(KeyDelete))

        ' Frage den User, ob er das wirklich will
        If ListPropertyInfo IsNot Nothing AndAlso
            AddinMsgBox(Localize.resRibbon.ResourceManager.GetString($"{KeyDelete}_Clear"), MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then

            ' Suche die Methode Clear und führe sie aus
            ListPropertyInfo.PropertyType.GetMethod("Clear").Invoke(ListPropertyInfo.GetValue(Me), Nothing)

            ' Schreibe einen Log-Eintrag
            NLogger.Info($"Liste {KeyDelete} gelöscht.")
        End If

    End Sub

End Class
