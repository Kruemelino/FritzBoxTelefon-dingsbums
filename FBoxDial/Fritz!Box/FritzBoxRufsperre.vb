Imports System.Threading.Tasks

Module FritzBoxRufsperre
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    ''' <summary>
    ''' Fügt einen Sperrlisteneintrag (<see cref="FritzBoxXMLKontakt"/>) zu der Sperrliste hinzu.
    ''' </summary>
    ''' <param name="Sperreintrag">Sperrlisteneintrag</param>
    ''' <param name="UID">Rückgabewert: UID des neuen Sperreintrages</param>
    Friend Function AddToCallBarring(Sperreintrag As FritzBoxXMLKontakt, Optional ByRef UID As Integer = 0) As Boolean

        Dim strXMLEintrag As String = DfltStringEmpty

        Using fboxTR064 As New FritzBoxTR64

            Return fboxTR064.SetCallBarringEntry(Sperreintrag.GetXMLKontakt, UID)

        End Using

    End Function

    ''' <summary>
    ''' Fügt eine Auflistung von Sperrlisteneinträgen (<see cref="FritzBoxXMLKontakt"/>) zu der Sperrliste hinzu.
    ''' </summary>
    ''' <param name="XMLDaten">Auflistung von Sperrlisteneinträgen</param>
    Friend Async Sub AddToCallBarring(XMLDaten As IEnumerable(Of String))

        Using fbtr064 As New FritzBoxTR64

            For Each Kontakt In XMLDaten
                If Kontakt.IsNotStringNothingOrEmpty Then
                    Await Task.Run(Sub()
                                       Dim UID As Integer = -1
                                       If fbtr064.SetCallBarringEntry(Kontakt, UID) Then
                                           NLogger.Info($"Eintrag mit der ID '{UID}' in der Rufsperre der Fritz!Box angelegt.")
                                       End If
                                   End Sub)

                End If
            Next

        End Using

    End Sub

    ''' <summary>
    ''' Löscht den Sperrlisteneintrag mit der entsprechenden <paramref name="UID"/>.
    ''' </summary>
    ''' <param name="UID">UID des zu entfernenden Sperrlisteneintrages</param>
    ''' <returns>Boolean, wenn erfolgreich</returns>
    Friend Function DeleteCallBarring(UID As Integer) As Boolean

        Dim strXMLEintrag As String = DfltStringEmpty

        Using fboxTR064 As New FritzBoxTR64

            Return fboxTR064.DeleteCallBarringEntryUID(UID)

        End Using

    End Function

End Module
