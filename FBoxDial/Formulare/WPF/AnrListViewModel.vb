Imports System.Windows.Threading

Public Class AnrListViewModel
    Inherits NotifyBase

    Private Shared Property NLogger As Logger = LogManager.GetCurrentClassLogger
    ''' <summary>
    ''' Returns Or sets a list as FritzBoxXMLCall             
    ''' </summary>
    Private _CallList As New ObservableCollectionEx(Of FritzBoxXMLCall)
    Public Property CallList As ObservableCollectionEx(Of FritzBoxXMLCall)
        Get
            Return _CallList
        End Get
        Set(value As ObservableCollectionEx(Of FritzBoxXMLCall))
            SetProperty(_CallList, value)
        End Set
    End Property

    Private _StartZeit As Date
    Public Property StartZeit As Date
        Get
            Return _StartZeit
        End Get
        Set(value As Date)
            SetProperty(_StartZeit, value)
        End Set
    End Property

    Private _EndZeit As Date
    Public Property EndZeit As Date
        Get
            Return _EndZeit
        End Get
        Set(value As Date)
            SetProperty(_EndZeit, value)
        End Set
    End Property

    Public Sub New()

        CallList = New ObservableCollectionEx(Of FritzBoxXMLCall)

    End Sub



End Class
