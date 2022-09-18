Imports System.Xml.Serialization

''' <summary>
''' Hier sind alle Eigenschaften enthalten, die für eigene Nummern relevant sind.
''' </summary>
<Serializable()> Public Class EigeneNrInfo
    Inherits NotifyBase
    ''' <summary>
    ''' Angabe, ob die Nummer im Anrufmonitor überwacht wird.
    ''' </summary>
    <XmlElement> Public Property Überwacht As Boolean

    ''' <summary>
    ''' SIP ID der Nummer
    ''' </summary>
    <XmlElement> Public Property SIP As Integer

    ''' <summary>
    ''' Farbdefinitionen
    ''' </summary>
    <XmlElement> Public Property Farben As Farbdefinition

End Class
