﻿Friend NotInheritable Class NutzerDaten
    Friend Shared Property XMLData As OutlookXML

    Public Sub New()
        ' Initialisiere die Nutzerdaten
        Laden(XMLData)
    End Sub

End Class