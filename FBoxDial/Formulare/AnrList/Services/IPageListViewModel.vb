Public Interface IPageListViewModel
    ReadOnly Property Name As String
    Property ListVM As ListViewModel
    Property InitialSelected As Boolean
    Sub Init()

End Interface