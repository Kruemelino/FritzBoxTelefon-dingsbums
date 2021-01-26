Imports System.Xml.Serialization
<Serializable()> Public Class SIPClient
    <XmlElement("X_AVM-DE_ClientIndex")> Public Property ClientIndex As Integer
    <XmlElement("X_AVM-DE_ClientUsername")> Public Property ClientUsername As String
    <XmlElement("X_AVM-DE_PhoneName")> Public Property PhoneName As String
    <XmlElement("X_AVM-DE_ClientId")> Public Property ClientId As String
    <XmlElement("X_AVM-DE_OutGoingNumber")> Public Property OutGoingNumber As String
    <XmlArray("X_AVM-DE_InComingNumbers"), XmlArrayItem("Item")> Property InComingNumbers As List(Of SIPTelNr)
    <XmlElement("X_AVM-DE_ExternalRegistration")> Public Property ExternalRegistration As Boolean
    <XmlElement("X_AVM-DE_InternalNumber")> Public Property InternalNumber As Integer
    <XmlElement("X_AVM-DE_DelayedCallNotification")> Public Property DelayedCallNotification As String

End Class
