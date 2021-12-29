Imports System.Windows.Media

Public Class PersonViewModel
    Inherits NotifyBase

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
    Public Sub New(person As FBoxAPI.Person)
        _Person = person
        ' Setze Felder
        RealName = person.RealName
    End Sub
End Class