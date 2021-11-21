Imports System.Threading.Tasks
Imports Microsoft.Office.Interop.Outlook
Public Class KontaktsucheViewModel
    Inherits NotifyBase
    'Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IDataKontaktsuche

#Region "ICommand"

#End Region

#Region "Properties"
    Private SearchTask As Task(Of List(Of ContactItem))

    Private _SearchText As String
    Public Property SearchText As String
        Get
            Return _SearchText
        End Get
        Set
            SetProperty(_SearchText, Value)
            ' Führe die Suche aus
            StarteKontaktSuche()
        End Set
    End Property

    Public Property Ergebnisse As New ObservableCollectionEx(Of ContactItemViewModel)

    Private _olKontaktVM As ContactItemViewModel
    Public Property OLKontaktVM As ContactItemViewModel
        Get
            Return _olKontaktVM
        End Get
        Set
            SetProperty(_olKontaktVM, Value)

            ' Wählcomando absenden
            If OLKontaktVM IsNot Nothing Then DatenService.DialContact(OLKontaktVM?.OlKontakt)
        End Set
    End Property
#End Region

    Public Sub New()
        ' Window Command

        ' Interface
        DatenService = New DataKontaktsuche
        'DialogService = New DialogService

    End Sub

    ''' <summary>
    ''' Startet due Kontaktsuche
    ''' </summary>
    Private Async Sub StarteKontaktSuche()

        If SearchText.IsNotStringNothingOrEmpty Then

            SearchTask = DatenService.KontaktSuche(SearchText)

            If Not SearchTask.IsCanceled Then
                ' Führe die Suche durch
                Dim NeuesErgebnis As List(Of ContactItemViewModel) = (Await SearchTask).Select(Function(K) New ContactItemViewModel With {.OlKontakt = K}).ToList
                ' Füge alle neuen Kontakte hinzu
                Ergebnisse.AddRange(NeuesErgebnis.Except(Ergebnisse))

                ' Entferne alle nicht mehr passenden Kontakte
                Ergebnisse.RemoveRange(Ergebnisse.Except(NeuesErgebnis))
            End If

        Else
            Ergebnisse.Clear()
        End If

    End Sub

End Class
