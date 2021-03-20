Imports System.Threading
Imports System.Windows.Threading
Imports Microsoft.Office.Interop.Outlook

Friend Class OptionenService
    Implements IOptionenService
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

#Region "Import Telefoniedaten"
    Private Property FritzBoxDaten As Telefonie
    Friend Event Beendet As EventHandler(Of NotifyEventArgs(Of Telefonie)) Implements IOptionenService.Beendet
    Friend Event Status As EventHandler(Of NotifyEventArgs(Of String)) Implements IOptionenService.Status


    Friend Sub StartImport() Implements IOptionenService.StartImport

        ' Neue Telefonie erstellen
        FritzBoxDaten = New Telefonie

        ' Ereignishandler hinzufügen
        AddHandler FritzBoxDaten.Beendet, AddressOf FritzBoxDatenImportBeendet
        AddHandler FritzBoxDaten.Status, AddressOf FritzBoxDatenStatus

        NLogger.Debug($"Einlesen der Telefoniedaten gestartet")

        Dispatcher.CurrentDispatcher.BeginInvoke(Sub()
                                                     ' Starte das Einlesen
                                                     If Ping(XMLData.POptionen.ValidFBAdr) Then
                                                         FritzBoxDaten.GetFritzBoxDaten()
                                                     End If
                                                 End Sub)


    End Sub

    Private Sub FritzBoxDatenStatus(sender As Object, e As NotifyEventArgs(Of String))
        RaiseEvent Status(Me, e)
    End Sub

    Private Sub FritzBoxDatenImportBeendet()

        ' Signalisiere, das beenden des Einlesens
        RaiseEvent Beendet(Me, New NotifyEventArgs(Of Telefonie)(FritzBoxDaten))

        ' Ereignishandler entfernen
        RemoveHandler FritzBoxDaten.Beendet, AddressOf FritzBoxDatenImportBeendet
        RemoveHandler FritzBoxDaten.Status, AddressOf FritzBoxDatenStatus

        NLogger.Debug($"Einlesen der Telefoniedaten beendet")
    End Sub


#End Region

#Region "Indizierung"
    Friend Event IndexStatus As EventHandler(Of NotifyEventArgs(Of Integer)) Implements IOptionenService.IndexStatus
    Friend Property CancelationPending As Boolean Implements IOptionenService.CancelationPending

    Public Function ZähleKontakte(olFolder As MAPIFolder) As Integer Implements IOptionenService.ZähleOutlookKontakte
        Return ZähleOutlookKontakte(olFolder)
    End Function

    Friend Sub Indexer(Ordner As MAPIFolder, IndexModus As Boolean, Unterordner As Boolean) Implements IOptionenService.Indexer

        For Each Item In Ordner.Items
            If CancelationPending Then Exit For

            If TypeOf Item Is ContactItem Then

                Dim aktKontakt As ContactItem = CType(Item, ContactItem)

                If IndexModus Then
                    IndiziereKontakt(aktKontakt)
                Else
                    DeIndiziereKontakt(aktKontakt)
                End If

                aktKontakt.Speichern

                aktKontakt.ReleaseComObject

            End If

            ' Erhöhe Wert für Progressbar
            RaiseEvent IndexStatus(Me, New NotifyEventArgs(Of Integer)(1))
        Next

        If Not IndexModus Then
            ' Entfernt alle Indizierungseinträge aus den Ordnern des Kontaktelementes.
            DeIndizierungOrdner(Ordner)
        End If

        ' Unterordner werden rekursiv durchsucht und indiziert
        If Unterordner Then
            Dim iOrdner As Integer = 1

            Do While (iOrdner.IsLessOrEqual(Ordner.Folders.Count)) And Not CancelationPending
                Indexer(Ordner.Folders.Item(iOrdner), IndexModus, Unterordner)
                iOrdner += 1
            Loop
        End If

        NLogger.Info($"{If(IndexModus, "Indizierung", "Deindizierung")} des Ordners {Ordner.Name} ist abgeschlossen.")
    End Sub

#End Region
End Class
