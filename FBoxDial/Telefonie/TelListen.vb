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

    Friend Sub DistictList()
        RINGListe = RINGListe.Distinct(New EqualityComparer).ToList
        CALLListe = CALLListe.Distinct(New EqualityComparer).ToList
        VIPListe = VIPListe.Distinct(New EqualityComparer).ToList
    End Sub

    ''' <summary>
    ''' Entfernt einen Eintrag aus der Liste
    ''' </summary>
    ''' <param name="Tag">Name der Liste gefolgt von einem _ und einer Nummer. Dies ist der Index des Eintrages in der Liste.</param>
    Friend Sub ClearListEntry(Tag As String)
        Dim ID As String() = Tag.Split("_")

        ' Liste anhand des übergeben Parameter ermitteln
        Dim ListPropertyInfo As PropertyInfo = Array.Find([GetType].GetProperties, Function(PropertyInfo As PropertyInfo) PropertyInfo.Name.IsEqual(ID.First))

        ' Frage den User, ob er das wirklich will
        If ListPropertyInfo IsNot Nothing AndAlso
            AddinMsgBox(Localize.resRibbon.ResourceManager.GetString($"{ID.First}Entry_Clear"), MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then

            ' Suche die Methode RemoveAt und führe sie aus
            ListPropertyInfo.PropertyType.GetMethod("RemoveAt").Invoke(ListPropertyInfo.GetValue(Me), {ID.Last.ToInt})

            ' Schreibe einen Log-Eintrag
            NLogger.Info($"Eintrag {ID.Last} der Liste {ID.First} gelöscht.")
        End If
    End Sub

    Friend Sub CreateAppointment(Tag As String)
        Dim ID As String() = Tag.Split("_")

        ' Liste anhand des übergeben Parameter ermitteln
        Dim ListPropertyInfo As PropertyInfo = Array.Find([GetType].GetProperties, Function(PropertyInfo As PropertyInfo) PropertyInfo.Name.IsEqual(ID.First))

        If ListPropertyInfo IsNot Nothing Then

            ' Suche die Eigenschaft Item und löse sie mit dem Index auf.
            Dim o As Telefonat = CType(ListPropertyInfo.PropertyType.GetProperty("Item").GetValue(ListPropertyInfo.GetValue(Me), {ID.Last.ToInt}), Telefonat)

            ' Erstelle eine Terminerinnerung
            If o IsNot Nothing Then o.ErstelleErinnerungEintrag()
        End If
    End Sub

    Friend Sub CreateContact(Tag As String)
        Dim ID As String() = Tag.Split("_")

        ' Liste anhand des übergeben Parameter ermitteln
        Dim ListPropertyInfo As PropertyInfo = Array.Find([GetType].GetProperties, Function(PropertyInfo As PropertyInfo) PropertyInfo.Name.IsEqual(ID.First))

        If ListPropertyInfo IsNot Nothing Then

            ' Suche die Eigenschaft Item und löse sie mit dem Index auf.
            Dim o As Telefonat = CType(ListPropertyInfo.PropertyType.GetProperty("Item").GetValue(ListPropertyInfo.GetValue(Me), {ID.Last.ToInt}), Telefonat)

            ' Erstelle eine Terminerinnerung
            If o IsNot Nothing Then o.ZeigeKontakt()
        End If
    End Sub
End Class
