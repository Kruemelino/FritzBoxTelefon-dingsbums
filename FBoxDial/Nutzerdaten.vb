Friend NotInheritable Class NutzerDaten
    Friend Shared Property XMLData As OutlookXML

    Public Sub New()
        ' Initialisiere die Nutzerdaten
        XMLData.Laden
    End Sub

End Class