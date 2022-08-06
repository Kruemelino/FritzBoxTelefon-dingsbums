Imports System.Xml.Serialization

''' <summary>
''' Hier sind alle Eigenschaften enthalten, die für eigene Nummern relevant sind.
''' </summary>
<Serializable()> Public Class EigeneNrInfo
    ''' <summary>
    ''' Angabe, ob die Nummer im Anrufmonitor überwacht wird.
    ''' </summary>
    <XmlElement> Public Property Überwacht As Boolean

    ''' <summary>
    ''' SIP ID der Nummer
    ''' </summary>
    <XmlElement> Public Property SIP As Integer

    ''' <summary>
    ''' Angabe, ob die Hintergrundfarbe des Anrufmonitors geändert werden soll
    ''' </summary>
    <XmlElement()> Public Property CBSetBackgroundColorByNumber As Boolean = False

    ''' <summary>
    ''' Angabe, ob die Schriftfarbe des Anrufmonitors geändert werden soll
    ''' </summary>
    ''' <returns></returns>
    <XmlElement()> Public Property CBSetForegroundColorByNumber As Boolean = False

    ''' <summary>
    ''' Hintergrundfarbe als HEX-String #00000000
    ''' </summary>
    <XmlElement()> Public Property TBBackgoundColorHex As String
        Get
            Return _TBBackgoundColor.ToString
        End Get
        Set
            _TBBackgoundColor = CType(Windows.Media.ColorConverter.ConvertFromString(Value), Windows.Media.Color)
        End Set
    End Property

    <XmlIgnore()> Public Property TBBackgoundColor As Windows.Media.Color

    ''' <summary>
    ''' Schriftfarbe als HEX-String #00000000
    ''' </summary>
    <XmlElement()> Public Property TBForegoundColorHex As String
        Get
            Return _TBForegoundColor.ToString
        End Get
        Set
            _TBForegoundColor = CType(Windows.Media.ColorConverter.ConvertFromString(Value), Windows.Media.Color)
        End Set
    End Property

    <XmlIgnore()> Public Property TBForegoundColor As Windows.Media.Color
End Class
