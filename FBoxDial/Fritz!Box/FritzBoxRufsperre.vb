Imports System.Threading.Tasks
Imports Microsoft.Office.Interop.Outlook

Module FritzBoxRufsperre
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    ''' <summary>
    ''' Fügt einen Sperrlisteneintrag (<see cref="FritzBoxXMLKontakt"/>) zu der Sperrliste hinzu.
    ''' </summary>
    ''' <param name="Sperreintrag">Sperrlisteneintrag</param>
    ''' <param name="UID">Rückgabewert: UID des neuen Sperreintrages</param>
    Friend Function AddToCallBarring(Sperreintrag As FritzBoxXMLKontakt, Optional ByRef UID As Integer = 0) As Boolean
        ' Prüfe, ob Fritz!Box verfügbar
        If Ping(XMLData.POptionen.ValidFBAdr) Then
            Dim strXMLEintrag As String = DfltStringEmpty


            Using fboxTR064 As New SOAP.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)

                Return fboxTR064.SetCallBarringEntry(Sperreintrag.GetXMLKontakt, UID)

            End Using
        Else
            NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
            Return False
        End If
    End Function

    ''' <summary>
    ''' Fügt eine Auflistung von Outlook Kontakten (<see cref="ContactItem"/>) zu der Sperrliste hinzu.
    ''' </summary>
    ''' <param name="OutlookKontakte">Auflistung von Sperrlisteneinträgen</param>
    Friend Async Sub AddToCallBarring(OutlookKontakte As IEnumerable(Of ContactItem))
        Const SperrlistenID As Integer = 258
        ' Prüfe, ob Fritz!Box verfügbar
        If Ping(XMLData.POptionen.ValidFBAdr) Then
            Using fbtr064 As New SOAP.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)

                ' Erzeuge für jeden Kontakt einen Eintrag
                For Each Kontakt In OutlookKontakte
                    Await Task.Run(Sub()

                                       With Kontakt
                                           ' Überprüfe, ob es in diesem Telefonbuch bereits einen verknüpften Kontakt gibt
                                           Dim UID As Integer = Kontakt.GetUniqueID(SperrlistenID)

                                           If UID.AreEqual(-1) Then
                                               NLogger.Debug($"Sperreintrag { .FullName} wird neu angelegt.")
                                           Else
                                               NLogger.Debug($"Sperreintrag { .FullName} wird überschrieben ({UID}).")
                                           End If

                                           ' Erstelle ein entsprechendes XML-Datenobjekt und lade es hoch
                                           If fbtr064.SetCallBarringEntry(.ErstelleXMLKontakt(UID), UID) Then
                                               ' Stelle die Verknüpfung her
                                               .SetUniqueID(SperrlistenID.ToString, UID.ToString)

                                               NLogger.Info($"Kontakt { .FullName} mit der ID '{UID}' in der Sperrliste der Fritz!Box angelegt.")

                                           End If
                                       End With
                                   End Sub)
                Next

            End Using
        Else
            NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
        End If
    End Sub

    ''' <summary>
    ''' Löscht den Sperrlisteneintrag mit der entsprechenden <paramref name="UID"/>.
    ''' </summary>
    ''' <param name="UID">UID des zu entfernenden Sperrlisteneintrages</param>
    ''' <returns>True, wenn erfolgreich</returns>
    Friend Function DeleteCallBarring(UID As Integer) As Boolean
        ' Prüfe, ob Fritz!Box verfügbar
        If Ping(XMLData.POptionen.ValidFBAdr) Then

            Dim strXMLEintrag As String = DfltStringEmpty

            Using fboxTR064 As New SOAP.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)

                Return fboxTR064.DeleteCallBarringEntryUID(UID)

            End Using
        Else
            NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
            Return False
        End If
    End Function

    ''' <summary>
    ''' Löscht die Sperrlisteneinträge von der Fritz!Box.
    ''' </summary>
    ''' <param name="Einträge">Zu entferndende Sperrlisteneinträge.</param>
    ''' <returns>True, wenn erfolgreich</returns>
    Friend Function DeleteCallBarrings(Einträge As IEnumerable(Of FritzBoxXMLKontakt)) As Boolean
        ' Prüfe, ob Fritz!Box verfügbar
        If Ping(XMLData.POptionen.ValidFBAdr) Then
            Using fbtr064 As New SOAP.FritzBoxTR64(XMLData.POptionen.ValidFBAdr, FritzBoxDefault.Anmeldeinformationen)
                With fbtr064
                    For Each Kontakt In Einträge
                        If .DeleteCallBarringEntryUID(Kontakt.Uniqueid) Then
                            NLogger.Info($"Eintrag mit der ID '{Kontakt.Uniqueid}' in den Rufsperren der Fritz!Box gelöscht.")
                            Return True

                        Else
                            NLogger.Warn($"Kontakt mit der ID '{Kontakt.Uniqueid}' in den Rufsperren der Fritz!Box nicht gelöscht.")
                            Return False

                        End If
                    Next
                End With
            End Using
            Return True
        Else
            NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
            Return False
        End If
    End Function
End Module
