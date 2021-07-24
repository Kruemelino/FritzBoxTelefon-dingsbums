Imports Newtonsoft.Json
Public Class TellowsScoreListEntry

    ''' <summary>
    ''' Phone Number
    ''' </summary>
    <JsonProperty("number")> Public Property Number As String

    ''' <summary>
    '''tellows Score for Number:
    '''<list type="bullet">
    '''<item>5 - neutral</item>
    '''<item>score &lt; 5: positive</item>
    '''<item>score &gt; 5: negative</item>
    '''</list>
    ''' </summary>
    <JsonProperty("score")> Public Property Score As Integer

    ''' <summary>
    ''' Count Comments for phone number
    ''' </summary>
    <JsonProperty("complains")> Public Property Complains As Integer

    ''' <summary>
    ''' Country Code for Phone Number
    ''' </summary>
    <JsonProperty("country")> Public Property Country As Integer

    ''' <summary>
    ''' Phone Number Prefix
    ''' </summary>
    <JsonProperty("prefix")> Public Property Prefix As Integer

    ''' <summary>
    ''' Count Searchrequests for Phonenumber
    ''' </summary>
    <JsonProperty("searches")> Public Property Searches As Integer

    ''' <summary>
    ''' Most Tagged Callertype
    ''' </summary>
    <JsonProperty("callertype")> Public Property CallerType As String

    ''' <summary>
    ''' Most possible Name of Caller (identified by user comments)
    ''' </summary>
    ''' <remarks>Optional</remarks>
    <JsonProperty("callername")> Public Property CallerName As String

    ''' <summary>
    ''' Name of Prefix for Phonenumber
    ''' </summary>
    ''' <remarks>Optional</remarks>
    <JsonProperty("prefixname")> Public Property PrefixName As String

    ''' <summary>
    ''' Timestamp of the last Comments
    ''' </summary>
    ''' <remarks>Optional</remarks>
    <JsonProperty("lastcomment")> Public Property LastComment As String

    ''' <summary>
    ''' Link to URL of Phone number on tellows Domain
    ''' </summary>
    ''' <remarks>Optional</remarks>
    <JsonProperty("deeplink")> Public Property DeepLink As String

    ''' <summary>
    ''' ID of Callertype
    ''' </summary>
    ''' <remarks>Optional</remarks>
    <JsonProperty("callertypeid")> Public Property CallerTypeID As Integer

End Class
