Imports System.Xml.Serialization
<Serializable()>
<XmlRoot("tellows"), XmlType("tellows")> Public Class TellowsResponse
    Implements ITellowsResult
#Region "Tellows Eigenschaften"
    ''' <summary>
    ''' Phonenumber
    ''' </summary>
    <XmlElement("number")> Public Property Number As String Implements ITellowsResult.Number

    ''' <summary>
    ''' normalized Phonenumber (Prefix, Countrycode)
    ''' </summary>
    <XmlElement("normalizedNumber")> Public Property NormalizedNumber As String

    ''' <summary>
    ''' tellows Score of Phonenumber 1-9 (1=best, 9=worst)
    ''' </summary>
    <XmlElement("score")> Public Property Score As Integer Implements ITellowsResult.Score

    ''' <summary>
    ''' Count Searches for this number
    ''' </summary>
    <XmlElement("searches")> Public Property Searches As Integer Implements ITellowsResult.Searches

    ''' <summary>
    ''' Count Comments for Phonenumber
    ''' </summary>
    <XmlElement("comments")> Public Property Comments As Integer Implements ITellowsResult.Comments

    ''' <summary>
    ''' Color of tellows Score
    ''' </summary>
    <XmlElement("scoreColor")> Public Property ScoreColor As String Implements ITellowsResult.ScoreColor

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

    <XmlIgnore> Public ReadOnly Property CallerName As String Implements ITellowsResult.CallerName
        Get
            Return String.Join(", ", CallerNames)
        End Get
    End Property

    <XmlIgnore> Public ReadOnly Property CallerType As String Implements ITellowsResult.CallerType
        Get
            Return String.Join(", ", CallerTypes.Select(Function(CT) $"{CT.Name} ({CT.Count})"))
        End Get
    End Property
End Class
