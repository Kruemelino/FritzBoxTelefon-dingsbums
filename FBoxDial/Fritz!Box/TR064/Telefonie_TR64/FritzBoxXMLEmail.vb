Imports System.Xml.Serialization

Namespace TR064
    <Serializable(), XmlType("email")> Public Class FritzBoxXMLEmail
        Inherits NotifyBase

        Private _Klassifizierer As XMLEMailTyp
        <XmlIgnore> Public Property Klassifizierer As XMLEMailTyp
            Get
                Return _Klassifizierer
            End Get
            Set
                SetProperty(_Klassifizierer, Value)
            End Set
        End Property

        Private _EMail As String
        <XmlText()> Public Property EMail As String
            Get
                Return _EMail
            End Get
            Set
                SetProperty(_EMail, Value)
            End Set
        End Property

        <XmlAttribute("classifier")> Public Property EMailTyp As String
            Get
                Return EnumToString(Klassifizierer)
            End Get
            Set
                Klassifizierer = StringToEnum(Of XMLEMailTyp)(Value)
            End Set
        End Property

    End Class
End Namespace


