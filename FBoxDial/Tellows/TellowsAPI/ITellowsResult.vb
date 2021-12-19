Public Interface ITellowsResult

    ''' <summary>
    ''' Phonenumber
    ''' </summary>
    ReadOnly Property Number As String

    ''' <summary>
    ''' tellows Score of Phonenumber 1-9 (1=best, 9=worst)
    ''' </summary>
    ReadOnly Property Score As Integer

    ''' <summary>
    ''' Count Searches for this number
    ''' </summary>
    ReadOnly Property Searches As Integer

    ''' <summary>
    ''' Count Comments for Phonenumber
    ''' </summary>
    ReadOnly Property Comments As Integer

    ''' <summary>
    ''' Color of tellows Score
    ''' </summary>
    ReadOnly Property ScoreColor As String

    ''' <summary>
    ''' Name of Caller
    ''' </summary>
    ReadOnly Property CallerName As String

    ''' <summary>
    ''' Most Tagged Callertype
    ''' </summary>
    ReadOnly Property CallerType As String
End Interface
