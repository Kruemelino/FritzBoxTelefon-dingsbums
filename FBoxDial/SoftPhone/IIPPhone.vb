Friend Interface IIPPhone

    ReadOnly Property IPPhoneReady As Boolean
    ''' <summary>
    ''' Angabe, ob die Raute # an die zu wählende Nummer angehangen werden soll.
    ''' </summary>
    Property AppendSuffix As Boolean
    Function Dial(DialCode As String, Hangup As Boolean) As Boolean
End Interface
