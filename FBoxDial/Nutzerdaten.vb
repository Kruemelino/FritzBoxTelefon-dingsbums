Friend NotInheritable Class NutzerDaten
    Friend Shared Property XMLData As OutlookXML

    Public Sub New()
        ' Initialisiere die Nutzerdaten
        XMLData.Laden

        If CVorwahlen.Kennzahlen Is Nothing Then Dim MainVorwahlen As New CVorwahlen
    End Sub

End Class