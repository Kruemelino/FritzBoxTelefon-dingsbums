Imports System.ComponentModel
Imports System.Windows.Controls
Imports Microsoft.Office.Interop
Public Class OptSearchContactView
    Inherits UserControl

    ' TODO: Code in eigene Routine Klasse verschieben
    Private Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private BWIndexerList As List(Of BackgroundWorker)

#Region "Indizierung"

    Private Structure Indizierungsdaten
        Dim Erstellen As Boolean
        Dim olFolder As Outlook.MAPIFolder
    End Structure

    Private Sub StarteIndizierung(OrdnerListe As List(Of OutlookOrdner), Erstellen As Boolean)
        ' Initialisiere die Progressbar
        InitProgressbar(0)

        If Not OrdnerListe?.Any Then
            With XMLData.POptionen.OutlookOrdner
                OrdnerListe.Add(New OutlookOrdner(.GetDefaultMAPIFolder(Outlook.OlDefaultFolders.olFolderContacts), OutlookOrdnerVerwendung.KontaktSuche))
            End With

            NLogger.Debug($"Es wurde kein Outlookordner für die Kontaktsuche gewählt. Füge Standarkkontaktornder hinzu.")
        End If

        If BWIndexerList Is Nothing Then BWIndexerList = New List(Of BackgroundWorker)

        ' Schleife durch jeden Ordner der indiziert werden soll
        For Each Ordner As OutlookOrdner In OrdnerListe

            ' Buttons einschalten
            BIndizierungAbbrechen.IsEnabled = True
            BIndizierungStart.IsEnabled = False

            Dim BWIndexer As New BackgroundWorker

            With BWIndexer
                ' Füge Ereignishandler hinzu
                AddHandler .DoWork, AddressOf BWIndexer_DoWork
                AddHandler .ProgressChanged, AddressOf BWIndexer_ProgressChanged
                AddHandler .RunWorkerCompleted, AddressOf BWIndexer_RunWorkerCompleted

                ' Setze Flags
                .WorkerSupportsCancellation = True
                .WorkerReportsProgress = True
                ' Und los...
                NLogger.Debug($"Starte {BWIndexerList.Count}. Backgroundworker für Kontaktindizierung im Ordner {Ordner.Name}.")
                .RunWorkerAsync(New Indizierungsdaten With {.Erstellen = Erstellen, .olFolder = Ordner.MAPIFolder})
            End With

            ' Füge dern Backgroundworker der Liste hinzu
            BWIndexerList.Add(BWIndexer)
        Next

    End Sub

    Private Sub BWIndexer_DoWork(sender As Object, e As DoWorkEventArgs)
        Dim BWIndexer As BackgroundWorker = CType(sender, BackgroundWorker)

        Dim Daten As Indizierungsdaten = CType(e.Argument, Indizierungsdaten)
        Dim AddtoMaxValue As Integer = ZähleOutlookKontakte(Daten.olFolder)

        Dispatcher.Invoke(Sub()
                              SetProgressbarMax(AddtoMaxValue)
                          End Sub)

        KontaktIndexer(Daten.olFolder, Daten.Erstellen, BWIndexer)
    End Sub

    Private Sub KontaktIndexer(Ordner As Outlook.MAPIFolder, Erstellen As Boolean, BWIndexer As BackgroundWorker)

        Dim aktKontakt As Outlook.ContactItem  ' aktueller Kontakt

        If Ordner.DefaultItemType = Outlook.OlItemType.olContactItem Then

            For Each item In Ordner.Items
                If BWIndexer IsNot Nothing AndAlso BWIndexer.CancellationPending Then Exit For

                ' nur Kontakte werden durchsucht
                If TypeOf item Is Outlook.ContactItem Then
                    aktKontakt = CType(item, Outlook.ContactItem)

                    If Erstellen Then
                        IndiziereKontakt(aktKontakt)
                    Else
                        DeIndiziereKontakt(aktKontakt)
                    End If

                    aktKontakt.Speichern

                    aktKontakt.ReleaseComObject

                End If

                If BWIndexer?.IsBusy Then BWIndexer.ReportProgress(1)
            Next

            If Not Erstellen Then
                ' Entfernt alle Indizierungseinträge aus den Ordnern des Kontaktelementes.
                DeIndizierungOrdner(Ordner)
            End If

            ' Unterordner werden rekursiv durchsucht und indiziert
            If XMLData.POptionen.CBSucheUnterordner Then
                Dim iOrdner As Integer = 1
                Do While (iOrdner.IsLessOrEqual(Ordner.Folders.Count)) Or (BWIndexer IsNot Nothing AndAlso BWIndexer.CancellationPending)
                    KontaktIndexer(Ordner.Folders.Item(iOrdner), Erstellen, BWIndexer)
                    iOrdner += 1
                Loop
            End If

            Ordner.ReleaseComObject
        End If

    End Sub

    Private Sub BWIndexer_ProgressChanged(sender As Object, e As ProgressChangedEventArgs)
        Dispatcher.Invoke(Sub()
                              SetProgressbar(e.ProgressPercentage)
                          End Sub)

    End Sub

    Private Sub BWIndexer_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)
        Dim BWIndexer As BackgroundWorker = CType(sender, BackgroundWorker)

        ' Backgroundworker aus der Liste entfernen
        BWIndexerList.Remove(BWIndexer)

        With BWIndexer
            ' Ereignishandler entfernen
            RemoveHandler .DoWork, AddressOf BWIndexer_DoWork
            RemoveHandler .ProgressChanged, AddressOf BWIndexer_ProgressChanged
            RemoveHandler .RunWorkerCompleted, AddressOf BWIndexer_RunWorkerCompleted

            BWIndexer.Dispose()
        End With
        NLogger.Info("Indizierung eines Ordners ist abgeschlossen.")

        ' Liste leeren, wenn kein Element mehr enthalten
        If Not BWIndexerList.Any Then
            BWIndexerList = Nothing
            NLogger.Info("Die komplette Indizierung ist abgeschlossen.")

            BIndizierungAbbrechen.IsEnabled = False
            BIndizierungStart.IsEnabled = True
        End If

    End Sub

    Private Sub InitProgressbar(Initialwert As Integer)
        ProgressBarIndex.Value = Initialwert
        ProgressBarIndex.Maximum = Initialwert
        LabelAnzahl.Text = $"Status: {Initialwert}/{ProgressBarIndex.Maximum}"
    End Sub

    Private Sub SetProgressbar(Anzahl As Integer)
        ProgressBarIndex.Value += Anzahl
        LabelAnzahl.Text = $"Status: {ProgressBarIndex.Value}/{ProgressBarIndex.Maximum}"
    End Sub

    Private Sub SetProgressbarMax(NeuesMaximum As Integer)
        ProgressBarIndex.Maximum += NeuesMaximum
    End Sub

    Private Sub BIndizierungStart_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BIndizierungStart.Click
        StarteIndizierung(OLFolderKontaktsSuche.ÜberwachteOrdnerListe.ToList, CBool(RBErstellen.IsChecked))
    End Sub

    Private Sub BIndizierungAbbrechen_Click(sender As Object, e As Windows.RoutedEventArgs) Handles BIndizierungAbbrechen.Click
        ' Indizierung abbrechen
        If BWIndexerList IsNot Nothing AndAlso BWIndexerList.Any Then
            BWIndexerList.ForEach(Sub(r) r.CancelAsync())
        End If
        ' Buttons wieder umschalten
        BIndizierungAbbrechen.IsEnabled = False
        BIndizierungStart.IsEnabled = True
    End Sub

#End Region

End Class
