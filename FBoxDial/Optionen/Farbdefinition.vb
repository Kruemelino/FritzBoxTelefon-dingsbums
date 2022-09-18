Imports System.Xml.Serialization
Public Class Farbdefinition
    Inherits NotifyBase

    ''' <summary>
    ''' Kontext für die Farbzuweisung.
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute()> Public Property Kontext As String = Nothing

    ''' <summary>
    ''' Angabe, ob die Hintergrundfarbe des Anrufmonitors geändert werden soll
    ''' </summary>
    <XmlAttribute()> Public Property CBSetBackgroundColor As Boolean = False

    ''' <summary>
    ''' Angabe, ob die Schriftfarbe des Anrufmonitors geändert werden soll
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute()> Public Property CBSetForegroundColor As Boolean = False

    Private _TBBackgoundColor As String
    ''' <summary>
    ''' Hintergrundfarbe als HEX-String #00000000
    ''' </summary>
    <XmlElement()> Public Property TBBackgoundColor As String
        Get
            Return _TBBackgoundColor
        End Get
        Set
            SetProperty(_TBBackgoundColor, Value)
        End Set
    End Property

    Private _TBForegoundColor As String
    ''' <summary>
    ''' Schriftfarbe als HEX-String #00000000
    ''' </summary>
    <XmlElement()> Public Property TBForegoundColor As String
        Get
            Return _TBForegoundColor
        End Get
        Set
            SetProperty(_TBForegoundColor, Value)
        End Set
    End Property
End Class
