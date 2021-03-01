Imports System.ComponentModel
Imports System.Xml.Serialization
<Serializable(), XmlType("email")> Public Class FritzBoxXMLEmail
    Inherits NotifyBase

    Private _Klassifizierer As XMLEMailTyp
    Private _EMail As String

    <XmlAttribute("classifier")> Public Property Klassifizierer As XMLEMailTyp
        Get
            Return _Klassifizierer
        End Get
        Set
            SetProperty(_Klassifizierer, Value)
        End Set
    End Property

    <XmlText()> Public Property EMail As String
        Get
            Return _EMail
        End Get
        Set
            SetProperty(_EMail, Value)
        End Set
    End Property

End Class

<TypeConverter(GetType(EnumDescriptionConverter))>
Public Enum XMLEMailTyp
    <Description("Sonstige")>
    <XmlEnum("")> notset = 0

    <Description("Privat")>
    <XmlEnum("private")> [private] = 1

    <Description("Arbeit")>
    <XmlEnum("work")> work = 2


End Enum
