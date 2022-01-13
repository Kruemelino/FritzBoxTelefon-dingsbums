Public Class EMailViewModel
    Inherits NotifyBase

#Region "Models"
    Public Property Mail As FBoxAPI.Email
#End Region

    Private _Classifier As XMLEMailTyp
    Public Property Classifier As XMLEMailTyp
        Get
            Return _Classifier
        End Get
        Set
            SetProperty(_Classifier, Value)
            Mail.Classifier = CType(Value, FBoxAPI.EMailTypEnum)
        End Set
    End Property

    Private _EMail As String
    Public Property EMail As String
        Get
            Return _EMail
        End Get
        Set
            SetProperty(_EMail, Value)
            Mail.EMail = Value
        End Set
    End Property

    Public Sub New(oMail As FBoxAPI.Email)
        _Mail = oMail
        ' Setze Felder
        EMail = Mail.EMail
        Classifier = CType(Mail.Classifier, XMLEMailTyp)
    End Sub
End Class



