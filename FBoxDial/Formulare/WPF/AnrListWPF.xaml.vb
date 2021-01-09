Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Markup

Partial Public Class AnrListWPF
    Inherits Window

    Dim Anrufliste As FritzBoxXMLCallList

    Public Sub New()

        InitializeComponent()

        Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name)

        DataContext = New AnrListViewModel

        LadeAnrufliste()

    End Sub

    Private Async Sub LadeAnrufliste()

        Dim getStringTask As Task(Of FritzBoxXMLCallList) = LadeFritzBoxAnrufliste()

        ' Anrufliste asynchron herunterladen
        Anrufliste = Await getStringTask

        ' Anrufliste im korrekten Thread in das Datagrid laden
        Dispatcher.Invoke(Sub()
                              With CType(DataContext, AnrListViewModel)
                                  ' Anrufliste in die ObservableCollection laden
                                  .CallList.AddRange(Anrufliste.Calls)
                              End With
                          End Sub)

    End Sub
#Region "Eventhandlers"
    Private Sub BtnStartJournalImport_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        'get datacontext
        'Dim dc = TryCast(DataContext, cCallerListViewModel)
        'Dim checkedList = dc.CallerEntries.Where(Function(x) x.Export = True).ToArray()

        'If checkedList.Length = 0 Then
        '    MessageBox.Show(resCommon.strNoEntriesSelected)
        '    Return
        'End If

        'Dim result = New StringBuilder()
        'progress.Maximum = checkedList.Length

        'For Each entry As cCallerEntry In checkedList
        '    result.AppendLine(entry.Name)
        '    progress.Value += 1
        'Next

        'MessageBox.Show(result.ToString())
    End Sub

    Private Sub BtnCancel_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        'MessageBox.Show(resCommon.strCancelClicked)
    End Sub

    Private Sub CmiCheckAll_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ''get datacontext
        'Dim dc = TryCast(DataContext, cCallerListViewModel)

        'For Each entry As cCallerEntry In dc.CallerEntries
        '    entry.Export = True
        'Next
    End Sub

    Private Sub CmiUncheckAll_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ''get datacontext
        'Dim dc = TryCast(DataContext, cCallerListViewModel)

        'For Each entry As cCallerEntry In dc.CallerEntries
        '    entry.Export = False
        'Next
    End Sub

#End Region

End Class


