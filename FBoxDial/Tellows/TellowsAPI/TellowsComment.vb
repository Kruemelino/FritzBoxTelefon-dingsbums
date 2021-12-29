Imports System.Xml.Serialization

<Serializable(), XmlType("comment")> Public Class TellowsComment
    ''' <summary>
    ''' Deeplink to comment on tellows website
    ''' </summary>
    <XmlElement("moreInfo")> Public Property MoreInfo As String

    ''' <summary>
    ''' Subject of comment
    ''' </summary>
    <XmlElement("subject")> Public Property Subject As String

    ''' <summary>
    ''' Text of comment
    ''' </summary>
    <XmlElement("text")> Public Property Text As String

    ''' <summary>
    ''' Creation Date of comment
    ''' </summary>
    <XmlElement("created")> Public Property Created As String

    ''' <summary>
    ''' tellows Score given by user for the Phonenumber in this comment
    ''' </summary>
    <XmlElement("userScore")> Public Property UserScore As Integer

    ''' <summary>
    ''' Nickname of the user who wrote the comment
    ''' </summary>
    <XmlElement("userName")> Public Property UserName As String

    ''' <summary>
    ''' Possible name of the caller, given by user in this comment
    ''' </summary>
    <XmlElement("callerName")> Public Property CallerName As String

    ''' <summary>
    ''' Tag for the caller, given by user in this comment
    ''' </summary>
    <XmlElement("callerType")> Public Property CallerType As String

    <XmlElement("callerTypeID")> Public Property CallerTypeID As Integer

    ''' <summary>
    ''' Rating for the comment by other users (higher is better, 0=no rating)
    ''' </summary>
    <XmlElement("helpful")> Public Property Helpful As Integer

    ''' <summary>
    ''' The ID of the tellows API-Partner that created the comment.
    ''' </summary>
    <XmlElement("creatorPartnerID")> Public Property CreatorPartnerID As Integer

    ''' <summary>
    ''' Optional parameter to indicate the country, where the comment was created, if it is different from the current targeted tellows domain/country.
    ''' </summary>
    ''' <remarks>Optional</remarks>
    <XmlElement("differentCountry")> Public Property DifferentCountry As Integer

End Class
