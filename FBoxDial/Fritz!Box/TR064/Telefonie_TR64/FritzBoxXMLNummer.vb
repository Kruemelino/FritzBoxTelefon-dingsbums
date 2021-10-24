Imports System.Xml.Serialization

Namespace TR064
    <Serializable(), XmlType("number")> Public Class FritzBoxXMLNummer
        Inherits NotifyBase

        Private _Typ As XMLTelNrTyp
        Private _Vanity As String
        Private _Prio As String
        Private _Schnellwahl As String
        Private _Nummer As String

        <XmlIgnore> Public Property Typ As XMLTelNrTyp
            Get
                Return _Typ
            End Get
            Set
                SetProperty(_Typ, Value)
            End Set
        End Property

        <XmlAttribute("type")> Public Property TelNrTyp As String
            Get
                Return EnumToString(Typ)
            End Get
            Set
                Typ = StringToEnum(Of XMLTelNrTyp)(Value)
            End Set
        End Property

        <XmlAttribute("vanity")> Public Property Vanity As String
            Get
                Return _Vanity
            End Get
            Set
                SetProperty(_Vanity, Value)
            End Set
        End Property

        <XmlAttribute("prio")> Public Property Prio As String
            Get
                Return _Prio
            End Get
            Set
                SetProperty(_Prio, Value)
            End Set
        End Property

        <XmlAttribute("quickdial")> Public Property Schnellwahl As String
            Get
                Return _Schnellwahl
            End Get
            Set
                SetProperty(_Schnellwahl, Value)
            End Set
        End Property

        <XmlText()> Public Property Nummer As String
            Get
                Return _Nummer
            End Get
            Set
                SetProperty(_Nummer, Value)
            End Set
        End Property

    End Class

End Namespace