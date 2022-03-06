Imports System.Threading.Tasks

Public Interface IFBoxData
    ReadOnly Property Name As String
    Property FBoxDataVM As FBoxDataViewModel
    ReadOnly Property InitialSelected As Boolean
    Property DebugBeginnLadeDaten As Date
    Sub Init()

End Interface

