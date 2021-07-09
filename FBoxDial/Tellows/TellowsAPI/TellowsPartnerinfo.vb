Imports System.Xml.Serialization

<Serializable(), XmlType("partnerinfo")> Public Class TellowsPartnerInfo

    ''' <summary>
    ''' Info
    ''' </summary>
    <XmlElement("info")> Public Property Info As String

    ''' <summary>
    ''' Company name
    ''' </summary>
    <XmlElement("company")> Public Property Company As String

    ''' <summary>
    ''' Apikey MD5 encoded
    ''' </summary>
    <XmlElement("apikeyMd5")> Public Property ApikeyMd5 As String

    ''' <summary>
    ''' Requests (all API-Requests so far)
    ''' </summary>
    <XmlElement("requests")> Public Property Requests As Integer

    ''' <summary>
    ''' Premium enabled
    ''' </summary>
    <XmlElement("premium")> Public Property Premium As Boolean

    ''' <summary>
    ''' Allow fetch Scorelist
    ''' </summary>
    <XmlElement("allowscorelist")> Public Property Allowscorelist As Boolean

    ''' <summary>
    ''' API-Key is valid until this date
    ''' </summary>
    <XmlElement("validuntil")> Public Property Validuntil As String

    ''' <summary>
    ''' Your ID for the tellows API. Also used as referer to target and flag user comments when using tellows frontend.
    ''' </summary>
    <XmlElement("apipartnerid")> Public Property Apipartnerid As String

End Class
