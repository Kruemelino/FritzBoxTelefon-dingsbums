Imports System.Windows.Threading

Public Class AnrListViewModel
    Inherits NotifyBase

#Region "Felder"

    ''' <summary>
    ''' Returns Or sets a list as FritzBoxXMLCall             
    ''' </summary>
    Private _CallList As New ObservableCollectionEx(Of FritzBoxXMLCall)
    Public Property CallList As ObservableCollectionEx(Of FritzBoxXMLCall)
        Get
            Return _CallList
        End Get
        Set
            SetProperty(_CallList, Value)
        End Set
    End Property

    Private _StartZeit As Date
    Public Property StartZeit As Date
        Get
            Return _StartZeit
        End Get
        Set
            SetProperty(_StartZeit, Value)
        End Set
    End Property

    Private _EndZeit As Date
    Public Property EndZeit As Date
        Get
            Return _EndZeit
        End Get
        Set
            SetProperty(_EndZeit, Value)
        End Set
    End Property

    Public Sub New()

        CallList = New ObservableCollectionEx(Of FritzBoxXMLCall)

    End Sub

#End Region

End Class
