Imports System.Windows
Imports System.Windows.Controls

Public Class OptionenWPF
    Inherits Window
    Private ReadOnly OptVM As OptionenViewModel
    Public Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        ' Initialisiere das ViewModel. Die Daten werden aus den Optionen geladen.
        OptVM = New OptionenViewModel
        ' zeige die Grunddaten an
        NavigationCtrl.Content = New UserCtrlGrund With {.DataContext = OptVM}
    End Sub

    Private Sub RadioButton_Checked(sender As Object, e As RoutedEventArgs)
        Select Case CType(sender, RadioButton).Name
            Case RBGrunde.Name
                NavigationCtrl.Content = New UserCtrlGrund With {.DataContext = OptVM}

            Case RBAnrMon.Name
                NavigationCtrl.Content = New UserCtrlAnrMon With {.DataContext = OptVM}

            Case RBWählhilfe.Name
                NavigationCtrl.Content = New UserCtrlWählhilfe With {.DataContext = OptVM}

            Case RBKontaktSuche.Name
                NavigationCtrl.Content = New UserCtrlKontaktsuche With {.DataContext = OptVM}

            Case RBKontakterstellung.Name
                NavigationCtrl.Content = New UserCtrlKontakterstellung With {.DataContext = OptVM}

            Case RBJournal.Name
                NavigationCtrl.Content = New UserCtrlJournalerstellung With {.DataContext = OptVM}

            Case RBTelefone.Name
                NavigationCtrl.Content = New UserCtrlTelefone With {.DataContext = OptVM}

            Case RBPhoner.Name
                NavigationCtrl.Content = New UserCtrlPhoner With {.DataContext = OptVM}

            Case RBInfo.Name
                NavigationCtrl.Content = New UserCtrlInfo With {.DataContext = OptVM}

            Case Else
                NavigationCtrl.Content = Nothing
        End Select
    End Sub

    Private Sub BSave_Click(sender As Object, e As RoutedEventArgs)
        ' Daten speichern
        OptVM.Speichern()
        ' Formular schließen
        Me.Close()
    End Sub

    Private Sub BCancel_Click(sender As Object, e As RoutedEventArgs)
        ' Formular schließen
        Me.Close()
    End Sub

    Private Sub BReset_Click(sender As Object, e As RoutedEventArgs) Handles BReset.Click
        ' Daten erneut
        OptVM.LadeDaten()
    End Sub
End Class
