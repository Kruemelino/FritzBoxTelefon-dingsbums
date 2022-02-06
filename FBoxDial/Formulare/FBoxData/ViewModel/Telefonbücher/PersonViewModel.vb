Imports System.Windows.Media

Public Class PersonViewModel
    Inherits NotifyBase
    Private Property DatenService As IFBoxDataService
#Region "Models"
    Public Property Person As FBoxAPI.Person
#End Region

#Region "Eigenschaften"
    Private _RealName As String
    ''' <summary>
    ''' Name of Contact 
    ''' </summary>
    Public Property RealName As String
        Get
            Return _RealName
        End Get
        Set
            SetProperty(_RealName, Value)
            Person.RealName = Value
        End Set
    End Property

    Private _ImageData As ImageSource
    Public Property ImageData As ImageSource
        Get
            Return _ImageData
        End Get
        Set
            SetProperty(_ImageData, Value)
        End Set
    End Property

#End Region
    Public Sub New(dataservice As IFBoxDataService, person As FBoxAPI.Person)
        DatenService = dataservice

        _Person = person
        ' Setze Felder
        RealName = person.RealName

        ' Kontaktbiler
        If person.ImageURL.IsNotStringNothingOrEmpty Then LadeBild()
    End Sub

#Region "Routinen für Personen"
    Private Async Sub LadeBild()
        ImageData = Await DatenService.LadeKontaktbild(Person)
    End Sub
#End Region
End Class