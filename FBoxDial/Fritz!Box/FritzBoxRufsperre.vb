
Module FritzBoxRufsperre

    Friend Function AddToCallBarring(Sperreintrag As FritzBoxXMLKontakt, Optional ByRef UID As Integer = 0) As Boolean

        Dim strXMLEintrag As String = DfltStringEmpty

        Using fboxTR064 As New FritzBoxTR64

            Return fboxTR064.SetCallBarringEntry(Sperreintrag.GetXMLKontakt, UID)

        End Using

    End Function

    Friend Function DeleteCallBarring(UID As Integer) As Boolean

        Dim strXMLEintrag As String = DfltStringEmpty

        Using fboxTR064 As New FritzBoxTR64

            Return fboxTR064.DeleteCallBarringEntryUID(UID)

        End Using

    End Function

End Module
