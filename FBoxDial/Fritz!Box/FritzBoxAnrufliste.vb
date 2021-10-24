﻿Imports System.Threading.Tasks

Friend Module FritzBoxAnrufliste
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
#Region "Anrufliste Laden"
    Friend Async Function LadeFritzBoxAnrufliste(FBoxTR064 As SOAP.FritzBoxTR64) As Task(Of FritzBoxXMLCallList)

        ' Prüfe, ob Fritz!Box verfügbar
        If Ping(XMLData.POptionen.ValidFBAdr) Then
            Dim Pfad As String = DfltStringEmpty

            ' Ermittle Pfad zur Anrufliste
            If FBoxTR064.X_contact.GetCallList(Pfad) Then
                Return Await DeserializeAsyncXML(Of FritzBoxXMLCallList)(Pfad, True)
            Else
                NLogger.Warn("Pfad zur XML-Anrufliste konnte nicht ermittelt werden.")
                Return Nothing
            End If
        Else
            NLogger.Warn($"Fritz!Box nicht verfügbar: '{XMLData.POptionen.ValidFBAdr}'")
            Return Nothing
        End If

    End Function
#End Region

#Region "Anrufliste auswerten"
    Friend Async Function ErstelleJournal(Anrufe As IEnumerable(Of FritzBoxXMLCall), ct As Threading.CancellationToken, progress As IProgress(Of Integer)) As Task(Of Integer)
        Return Await Task.Run(Async Function()
                                  Dim Einträge As Integer = 0

                                  For Each Anruf In Anrufe
                                      ' Journaleintrag erstellen
                                      Using t = Await Anruf.ErstelleTelefonat
                                          ' Erstelle einen Journaleintrag
                                          t.ErstelleJournalEintrag()

                                          ' Anruflisten aktualisieren
                                          t.UpdateRingCallList()
                                      End Using

                                      ' Zählvariable hochsetzen
                                      Einträge += 1

                                      ' Status weitergeben
                                      progress.Report(1)

                                      ' Abbruch überwachen
                                      If ct.IsCancellationRequested Then Exit For
                                  Next

                                  Return Einträge
                              End Function, ct)

    End Function
#End Region
End Module
