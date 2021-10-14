Public Class KontaktsucheViewModel
    Inherits NotifyBase
    'Private Property NLogger As Logger = LogManager.GetCurrentClassLogger
    Private Property DatenService As IDataKontaktsuche
    'Private Property DialogService As IDialogService

#Region "ICommand"

#End Region

#Region "Properties"

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

    Private _Ergebnisse As ObservableCollectionEx(Of ContactItemViewModel)
    Public Property Ergebnisse As ObservableCollectionEx(Of ContactItemViewModel)
        Get
            Return _Ergebnisse
        End Get
        Set
            SetProperty(_Ergebnisse, Value)
        End Set
    End Property

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

        If Ergebnisse Is Nothing Then
            ' Erstelle eine neue ObserverCollection
            Ergebnisse = New ObservableCollectionEx(Of ContactItemViewModel)
        End If

        If SearchText.IsNotStringNothingOrEmpty Then
            ' Führe die Suche durch
            Dim NeuesErgebnis As List(Of ContactItemViewModel) = (Await DatenService.KontaktSuche(SearchText)).Select(Function(K) New ContactItemViewModel With {.OlKontakt = K}).ToList

            ' Füge alle neuen Kontakte hinzu
            Ergebnisse.AddRange(NeuesErgebnis.Except(Ergebnisse))

            ' Entferne alle nicht mehr passenden Kontakte
            Ergebnisse.RemoveRange(Ergebnisse.Except(NeuesErgebnis))
        Else
            Ergebnisse.Clear()
        End If

    End Sub

End Class
