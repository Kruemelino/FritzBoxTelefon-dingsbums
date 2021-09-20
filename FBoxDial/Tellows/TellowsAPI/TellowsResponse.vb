Imports System.Xml.Serialization
<Serializable()>
<XmlRoot("tellows"), XmlType("tellows")> Public Class TellowsResponse
#Region "Tellows Eigenschaften"
    ''' <summary>
    ''' Phonenumber
    ''' </summary>
    <XmlElement("number")> Public Property Number As String

    ''' <summary>
    ''' normalized Phonenumber (Prefix, Countrycode)
    ''' </summary>
    <XmlElement("normalizedNumber")> Public Property NormalizedNumber As String

    ''' <summary>
    ''' tellows Score of Phonenumber 1-9 (1=best, 9=worst)
    ''' </summary>
    <XmlElement("score")> Public Property Score As Integer

    ''' <summary>
    ''' Count Searches for this number
    ''' </summary>
    <XmlElement("searches")> Public Property Searches As Integer

    ''' <summary>
    ''' Count Comments for Phonenumber
    ''' </summary>
    <XmlElement("comments")> Public Property Comments As Integer

    ''' <summary>
    ''' Color of tellows Score
    ''' </summary>
    <XmlElement("scoreColor")> Public Property ScoreColor As String

    ''' <summary>
    ''' Path to image of tellows Score
    ''' </summary>
    <XmlElement("scorePath")> Public Property ScorePath As String

    ''' <summary>
    ''' Name of Prefix (Location for non mobile/service numbers)
    ''' </summary>
    <XmlElement("location")> Public Property Location As String

    ''' <summary>
    ''' Name of Country for Phonenumber (depending on Countrycode)
    ''' </summary>
    <XmlElement("country")> Public Property Country As String

    ''' <summary>
    ''' Array of tagged Callertypes for this number, ordered by most tagged type
    ''' </summary>
    <XmlArray("callerTypes"), XmlArrayItem("caller")> Public Property CallerTypes As List(Of TellowsCaller)

    ''' <summary>
    ''' Array of possible names of the caller, orderd by relevance
    ''' </summary>
    <XmlArray("callerNames"), XmlArrayItem("caller")> Public Property CallerNames As List(Of String)

    ''' <summary>
    ''' List of Comments for given Phonenumber
    ''' </summary>
    <XmlArray("commentList"), XmlArrayItem("comment")> Public Property CommentList As List(Of TellowsComment)

    <XmlElement("numberDetails")> Public Property NumberDetails As TellowsNumberDetails

    ''' <summary>
    ''' Show tellows API Partner information for your account.
    ''' </summary>
    <XmlElement("partnerinfo")> Public Property Partnerinfo As TellowsPartnerInfo
#End Region

End Class
