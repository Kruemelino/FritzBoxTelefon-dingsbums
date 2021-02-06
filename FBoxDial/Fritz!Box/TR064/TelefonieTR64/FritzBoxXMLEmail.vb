﻿Imports System.Xml.Serialization
<Serializable()> Public Class FritzBoxXMLEmail
    Inherits NotifyBase

    Private _Klassifizierer As String
    Private _EMail As String

    <XmlAttribute("classifier")> Public Property Klassifizierer As String
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
