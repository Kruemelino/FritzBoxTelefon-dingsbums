Public Class NumberViewModel
    Inherits NotifyBase

#Region "Models"
    Public Property Nummer As FBoxAPI.NumberType
#End Region

    Private _Type As XMLTelNrTyp
    Public Property Type As XMLTelNrTyp
        Get
            Return _Type
        End Get
        Set
            SetProperty(_Type, Value)
            Nummer.Type = CType(Value, FBoxAPI.TelNrTypEnum)
        End Set
    End Property

    Private _Vanity As String
    Public Property Vanity As String
        Get
            Return _Vanity
        End Get
        Set
            SetProperty(_Vanity, Value)
            Nummer.Vanity = Value
        End Set
    End Property

    Private _Prio As String
    Public Property Prio As String
        Get
            Return _Prio
        End Get
        Set
            SetProperty(_Prio, Value)
            Nummer.Prio = Value
        End Set
    End Property

    Private _QuickDial As String
    Public Property QuickDial As String
        Get
            Return _QuickDial
        End Get
        Set
            SetProperty(_QuickDial, Value)
            Nummer.QuickDial = Value
        End Set
    End Property

    Private _Number As String

    Public Property Number As String
        Get
            Return _Number
        End Get
        Set
            SetProperty(_Number, Value)
            Nummer.Number = Value
        End Set
    End Property

    Public Sub New(oNummer As FBoxAPI.NumberType)
        _Nummer = oNummer
        ' Setze Felder
        With Nummer
            Type = CType(.Type, XMLTelNrTyp)
            Vanity = .Vanity
            Prio = .Prio
            QuickDial = .QuickDial
            Number = .Number
        End With


    End Sub

End Class
