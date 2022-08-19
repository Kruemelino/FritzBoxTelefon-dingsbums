﻿Imports System.Xml.Serialization

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
    <XmlElement()> Public Property TBBackgoundColor As String

    ''' <summary>
    ''' Schriftfarbe als HEX-String #00000000
    ''' </summary>
    <XmlElement()> Public Property TBForegoundColor As String

End Class
