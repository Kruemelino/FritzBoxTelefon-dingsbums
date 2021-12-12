Public Class CallViewModel
    Inherits NotifyBase

#Region "Model"
    Private _Call As FBoxAPI.Call
    Public Property CallItem As FBoxAPI.Call
        Get
            Return _Call
        End Get
        Set
            SetProperty(_Call, Value)
        End Set
    End Property
#End Region

#Region "Eigene Eigenschaften"
    Private _Export As Boolean
    Public Property Export As Boolean
        Get
            Return _Export
        End Get
        Set
            SetProperty(_Export, Value)
        End Set
    End Property

    ''' <summary>
    ''' Gibt die Gegenstellennummer (ferne Nummer, NICHT die eigene Nummer) zurück.
    ''' Es wird je nach Telefonatstyp <c>Type</c> unterschschieden werden.    ''' 
    ''' </summary>
    Public ReadOnly Property Gegenstelle As String
        Get
            Select Case CallItem.Type
                Case 1, 2, 9, 10
                    Return CallItem.Caller
                Case Else '3, 11
                    Return CallItem.Called
            End Select
        End Get
    End Property

    Public ReadOnly Property Typ As Integer
        Get
            Return CallItem.Type
        End Get
    End Property

    ''' <summary>
    ''' Gibt die Eigene Nummer zurück.
    ''' </summary>
    Public ReadOnly Property EigeneNummer As String
        Get
            Return $"{CallItem.CalledNumber}{CallItem.CallerNumber}"
        End Get
    End Property

    Public ReadOnly Property Name As String
        Get
            Return CallItem.Name
        End Get
    End Property

    Public ReadOnly Property Datum As Date
        Get
            Return CDate(CallItem.[Date])
        End Get
    End Property

    Public ReadOnly Property Dauer As TimeSpan
        Get
            With Datum
                Return New TimeSpan(.Hour, .Minute, .Second)
            End With
        End Get
    End Property

    Public ReadOnly Property Gerät As String
        Get
            Return CallItem.Device
        End Get
    End Property
#End Region

End Class
