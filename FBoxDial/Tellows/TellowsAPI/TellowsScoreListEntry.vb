Imports Newtonsoft.Json
Public Class TellowsScoreListEntry
    Implements ITellowsResult

    ''' <summary>
    ''' Phone Number
    ''' </summary>
    <JsonProperty("number")> Public Property Number As String Implements ITellowsResult.Number

    ''' <summary>
    '''tellows Score for Number:
    '''<list type="bullet">
    '''<item>5 - neutral</item>
    '''<item>score &lt; 5: positive</item>
    '''<item>score &gt; 5: negative</item>
    '''</list>
    ''' </summary>
    <JsonProperty("score")> Public Property Score As Integer Implements ITellowsResult.Score

    ''' <summary>
    ''' Count Comments for phone number
    ''' </summary>
    <JsonProperty("complains")> Public Property Complains As Integer Implements ITellowsResult.Comments

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
    <JsonProperty("searches")> Public Property Searches As Integer Implements ITellowsResult.Searches

    ''' <summary>
    ''' Most Tagged Callertype
    ''' </summary>
    <JsonProperty("callertype")> Public Property CallerType As String Implements ITellowsResult.CallerType

    ''' <summary>
    ''' Most possible Name of Caller (identified by user comments)
    ''' </summary>
    ''' <remarks>Optional</remarks>
    <JsonProperty("callername")> Public Property CallerName As String Implements ITellowsResult.CallerName

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

    ''' <summary>
    ''' <list type="table">
    ''' <listheader>
    ''' <term>Score</term>
    ''' <description>Color</description>
    ''' </listheader>
    ''' <item>
    ''' <term>1</term>
    ''' <description>#00fc00</description>
    ''' </item>
    '''  <item>
    ''' <term>2</term>
    ''' <description>#30f90a</description>
    ''' </item>
    ''' <item>
    ''' <term>3</term>
    ''' <description>#68ff0b</description>
    ''' </item>
    ''' <item>
    ''' <term>4</term>
    ''' <description>#8dfc08</description>
    ''' </item>
    ''' <item>
    ''' <term>5</term>
    ''' <description>#d6ff18</description>
    ''' </item>
    ''' <item>
    ''' <term>6</term>
    ''' <description>#f4d11f</description>
    ''' </item>
    ''' <item>
    ''' <term>7</term>
    ''' <description>#f79a01</description>
    ''' </item>
    ''' <item>
    ''' <term>8</term>
    ''' <description>#fb6703</description>
    ''' </item>
    ''' <item>
    ''' <term>9</term>
    ''' <description>#ff3505</description>
    ''' </item>
    ''' </list>
    ''' </summary>
    <JsonIgnore> Public ReadOnly Property ScoreColor As String Implements ITellowsResult.ScoreColor
        Get
            Select Case Score
                Case 1
                    Return "#00fc00"
                Case 2
                    Return "#30f90a"
                Case 3
                    Return "#68ff0b"
                Case 4
                    Return "#8dfc08"
                Case 5
                    Return "#d6ff18"
                Case 6
                    Return "#f4d11f"
                Case 7
                    Return "#f79a01"
                Case 8
                    Return "#fb6703"
                Case 9
                    Return "#ff3505"
                Case Else
                    Return "-1"
            End Select
        End Get
    End Property

End Class
