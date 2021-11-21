Imports System.Threading.Tasks

Public Class AnrListItemViewModel
    Inherits NotifyBase

#Region "Model"
    Private _Call As FBoxAPI.Call
    Public Property [Call] As FBoxAPI.Call
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
            Select Case [Call].Type
                Case 1, 2, 9, 10
                    Return [Call].Caller
                Case Else '3, 11
                    Return [Call].Called
            End Select
        End Get
    End Property

    Public ReadOnly Property EigeneNummer As String
        Get
            Return $"{[Call].CalledNumber}{[Call].CallerNumber}"
        End Get
    End Property

    Public ReadOnly Property Datum As Date
        Get
            Return CDate([Call].[Date].ToString)
        End Get
    End Property
    Public ReadOnly Property Dauer As TimeSpan
        Get
            With CDate([Call].Duration)
                Return New TimeSpan(.Hour, .Minute, .Second)
            End With
        End Get
    End Property
#End Region

End Class
