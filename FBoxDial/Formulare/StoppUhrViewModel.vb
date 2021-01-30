Public Class StoppUhrViewModel
    Inherits NotifyBase

#Region "Felder"
    Private _Beginn As Date
    Public Property Beginn As Date
        Get
            Return _Beginn
        End Get
        Set
            SetProperty(_Beginn, Value)
        End Set
    End Property

    Private _Ende As Date
    Public Property Ende As Date
        Get
            Return _Ende
        End Get
        Set
            SetProperty(_Ende, Value)
        End Set
    End Property

    Private _Name As String
    Public Property Name As String
        Get
            Return _Name
        End Get
        Set
            SetProperty(_Name, Value)
        End Set
    End Property

    Private _AutomatischAusblenden As Boolean
    Public Property AutomatischAusblenden As Boolean
        Get
            Return _AutomatischAusblenden
        End Get
        Set
            SetProperty(_AutomatischAusblenden, Value)
        End Set
    End Property

    Private _Ausblendverzögerung As Integer
    Public Property Ausblendverzögerung As Integer
        Get
            Return _Ausblendverzögerung
        End Get
        Set
            SetProperty(_Ausblendverzögerung, Value)
        End Set
    End Property

    Private _Tlfnt As Telefonat
    Public Property Tlfnt As Telefonat
        Get
            Return _Tlfnt
        End Get
        Set
            SetProperty(_Tlfnt, Value)
        End Set
    End Property
#End Region

    Public Sub New()

    End Sub
End Class


