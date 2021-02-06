Imports System.Threading
Imports System.Windows
Imports System.Windows.Markup

Partial Public Class AnrListWPF
    Inherits Window

    Private Anrufliste As FritzBoxXMLCallList
    Private CancelationPending As Boolean = False
    Private Property NLogger As Logger = LogManager.GetCurrentClassLogger

    Public Sub New()

        InitializeComponent()

        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)


        With CType(DataContext, AnrListViewModel)
            .StartDatum = XMLData.POptionen.LetzterJournalEintrag.Date
            .StartZeit = XMLData.POptionen.LetzterJournalEintrag.TimeOfDay

            .EndDatum = Now.Date
            .EndZeit = Now.TimeOfDay

        End With

        LadeAnrufliste()


    End Sub

    Private Async Sub LadeAnrufliste()

        ' Anrufliste asynchron herunterladen
        Anrufliste = Await LadeFritzBoxAnrufliste()

        ' Anrufliste im korrekten Thread in das Datagrid laden
        Dispatcher.Invoke(Sub()
                              With CType(DataContext, AnrListViewModel)
                                  ' Anrufliste in die ObservableCollection laden
                                  .CallList.AddRange(Anrufliste?.Calls)
                              End With

                              AddHandler DPSDatum.SelectedDateChanged, AddressOf DPDatum_SelectedDateChanged
                              AddHandler DPEDatum.SelectedDateChanged, AddressOf DPDatum_SelectedDateChanged
                              AddHandler TPSZeit.SelectedTimeChanged, AddressOf TPZeit_SelectedTimeChanged
                              AddHandler TPEZeit.SelectedTimeChanged, AddressOf TPZeit_SelectedTimeChanged

                          End Sub)

    End Sub


#Region "Eventhandlers"
    Private Sub BtnStartJournalImport_Click(sender As Object, e As RoutedEventArgs) Handles btnStartJournalImport.Click
        With CType(DataContext, AnrListViewModel)
            Dim AusgewählteAnrufe As IEnumerable(Of FritzBoxXMLCall) = .CallList.Where(Function(x) x.Export = True)

            If AusgewählteAnrufe.Any Then

                progress.Maximum = AusgewählteAnrufe.Count

                NLogger.Debug($"Starte manueller Journalimport mit {AusgewählteAnrufe.Count} Einträgen.")

                For Each Anruf In AusgewählteAnrufe
                    If CancelationPending Then Exit For

                    Anruf.ErstelleTelefonat.ErstelleJournalEintrag()

                    progress.Value += 1
                Next
            End If

        End With
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As RoutedEventArgs)
        CancelationPending = True
        NLogger.Debug("Manueller Journalimport abgebrochen.")
    End Sub

    Private Sub CmiCheckAll_Click(sender As Object, e As RoutedEventArgs)
        SelectAll(True)
    End Sub

    Private Sub CmiUncheckAll_Click(sender As Object, e As RoutedEventArgs)
        SelectAll(False)
    End Sub

    Private Sub DPDatum_SelectedDateChanged(sender As Object, e As Controls.SelectionChangedEventArgs)
        SelectItems()
    End Sub

    Private Sub TPZeit_SelectedTimeChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of TimeSpan))
        SelectItems()
    End Sub

    Private Sub SelectAll([Select] As Boolean)
        With CType(DataContext, AnrListViewModel)
            For Each Anruf In .CallList
                Anruf.Export = [Select]
            Next
        End With
    End Sub

    Private Sub SelectItems()
        ' Ausgewählten Zeitraum ermitteln
        ' Startpunkt
        Dim ImportStart As Date = DPSDatum.SelectedDate.Value.Add(TPSZeit.SelectedTime)

        ' Endzeitpunkt
        Dim ImportEnde As Date = DPEDatum.SelectedDate.Value.Add(TPEZeit.SelectedTime)

        Dim AusgewählteAnrufe As IEnumerable(Of FritzBoxXMLCall)
        With CType(DataContext, AnrListViewModel)
            ' Ermittle alle Einträge, die im ausgewählten Bereich liegen
            AusgewählteAnrufe = .CallList.Where(Function(x) ImportStart <= x.Datum And x.Datum <= ImportEnde)

            ' Entferne die Exportmarkierung, bei allen Einträgen, die nicht im Bereich liegen
            For Each Anruf In .CallList.Except(AusgewählteAnrufe)
                Anruf.Export = False
            Next

            ' Füge die Exportmarkierung, bei allen Einträgen, die im Bereich liegen hinzu
            For Each Anruf In AusgewählteAnrufe
                Anruf.Export = True
            Next
        End With
    End Sub


#End Region

End Class


