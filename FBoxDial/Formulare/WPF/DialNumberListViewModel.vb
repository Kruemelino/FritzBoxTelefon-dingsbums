Imports System.ComponentModel

Public Class DialNumberListViewModel
    Inherits BindableBase

    Private _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set(value As String)
            SetProperty(_Name, value)
        End Set
    End Property

    ''' <summary>
    ''' Returns Or sets a list as Telefonnummern             
    ''' </summary>
    Private _DialNumberList As New ObservableCollectionEx(Of Telefonnummer)
    Public Property DialNumberList As ObservableCollectionEx(Of Telefonnummer)
        Get
            Return _DialNumberList
        End Get
        Set(value As ObservableCollectionEx(Of Telefonnummer))
            SetProperty(_DialNumberList, value)
        End Set
    End Property

End Class


