Public Interface IFBoxData
    ReadOnly Property Name As String
    Property FBoxDataVM As FBoxDataViewModel
    ReadOnly Property InitialSelected As Boolean
    Sub Init()
End Interface

