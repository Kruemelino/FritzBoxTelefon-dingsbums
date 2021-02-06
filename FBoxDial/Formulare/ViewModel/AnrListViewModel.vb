Public Class AnrListViewModel
    Inherits NotifyBase

#Region "Felder"

    ''' <summary>
    ''' Returns Or sets a list as FritzBoxXMLCall             
    ''' </summary>
    Private _CallList As ObservableCollectionEx(Of FritzBoxXMLCall)
    Public Property CallList As ObservableCollectionEx(Of FritzBoxXMLCall)
        Get
            Return _CallList
        End Get
        Set
            SetProperty(_CallList, Value)
        End Set
    End Property

    Private _StartDatum As Date
    Public Property StartDatum As Date
        Get
            Return _StartDatum
        End Get
        Set
            SetProperty(_StartDatum, Value)
        End Set
    End Property

    Private _StartZeit As TimeSpan
    Public Property StartZeit As TimeSpan
        Get
            Return _StartZeit
        End Get
        Set
            SetProperty(_StartZeit, Value)
        End Set
    End Property

    Private _EndDatum As Date
    Public Property EndDatum As Date
        Get
            Return _EndDatum
        End Get
        Set
            SetProperty(_EndDatum, Value)
        End Set
    End Property

    Private _EndZeit As TimeSpan
    Public Property EndZeit As TimeSpan
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
