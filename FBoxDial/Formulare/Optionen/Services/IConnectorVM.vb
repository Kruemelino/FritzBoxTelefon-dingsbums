Public Interface IConnectorVM
    ReadOnly Property Name As String
    Property Connector As IPPhoneConnector
    Property OptVM As OptionenViewModel
    Sub Init(Connector As IPPhoneConnector, OptVM As OptionenViewModel)
End Interface
